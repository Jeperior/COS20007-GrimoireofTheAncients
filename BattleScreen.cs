using System;
using System.Numerics;
using Raylib_cs;

namespace Grimoire
{
    // Manages the in-battle UI, animations, timers, input and resolving spell casts between two players.
    // This class coordinates drawing the battle screen and updating all battle-related state each frame.
    public class BattleScreen
    {
        // Managers and helpers
        private ParticleManager _particleManager;      // For healing particles (no snow during battle)
        private Animation _animation;                  // Sprite/animation helper
        private AudioManager _audioManager;            // Play SFX (cast, hit, UI)
        private BattleLog _battleLog;                  // Scrolling combat log
        private CoinFlipScreen _coinFlipScreen;        // Used to check if battle started

        // Turn and selection state
        private int _currentTurn;                      // 1 = player1, 2 = player2
        private int _selectedBattleSpellIndex;         // Index of selected spell in UI

        // Animation & timing
        private float _battleAnimationTime;            // Continuous time for blinking effects
        private int _currentFrame;                     // Current frame index for wizard sprite sheet
        private float _frameCounter;                   // Counts time to advance _currentFrame
        private int _castingPlayer;                    // Which player is currently casting (1 or 2)
        private bool _spellDamageApplied;              // Ensures damage is applied only once per hit
        private SpellAnimation? _currentSpellAnimation;// Active spell animation (projectiles, casting)
        private Spell? _pendingSpell;                  // Spell to actually apply when projectile hits
        private Player? _pendingCaster;                // Caster saved for deferred application
        private Player? _pendingTarget;                // Target saved for deferred application
       

        // Turn/match timers and limits
        private float _turnTimer;                      // Remaining seconds for current player's turn
        private float _matchTimer;                     // Remaining seconds for the match
        private float _turnTimeLimit;                  // Default turn time reset value

        // Paralyze (skip-turn) handling
        private bool _skipNextParalyzeCheck;           // Used to allow caster to get the extra turn after skipping
        private float _paralyzeDisplayTimer;           // How long to show paralyzed status after clearing effects
        private float _paralyzeDisplayDuration;        // Constant display duration for paralyze visuals

        // Death animation frames (track independent progress for each player)
        private int _player1DeathFrame;
        private int _player2DeathFrame;

        // Pause state
        private bool _isPaused;

        /// <summary>
        /// Initializes the battle screen with required managers and default state.
        /// </summary>
        /// <param name="particleManager"></param>
        /// <param name="animation"></param>
        /// <param name="battleLog"></param>
        /// <param name="coinFlipScreen"></param>
        /// <param name="audioManager"></param>
        public BattleScreen(ParticleManager particleManager, Animation animation, BattleLog battleLog, CoinFlipScreen coinFlipScreen, AudioManager audioManager)
        {
            // Initialize dependencies and default values
            _particleManager = particleManager;
            _animation = animation;
            _audioManager = audioManager;
            _battleLog = battleLog;
            _coinFlipScreen = coinFlipScreen;
            _currentTurn = 1;
            _selectedBattleSpellIndex = 0;
            _battleAnimationTime = 0f;
            _currentFrame = 0;
            _frameCounter = 0f;
            _currentSpellAnimation = null;
            _castingPlayer = 0;
            _pendingSpell = null;
            _pendingCaster = null;
            _pendingTarget = null;
            _spellDamageApplied = false;
            _turnTimer = 15f;
            _matchTimer = 300f;
            _turnTimeLimit = 10f;
            _skipNextParalyzeCheck = false;
            _paralyzeDisplayTimer = 0f;
            _paralyzeDisplayDuration = 2.0f;
            _player1DeathFrame = 0;
            _player2DeathFrame = 0;
            _isPaused = false;
        }

        // Expose current turn for other systems
        public int CurrentTurn
        {
            get { return _currentTurn; }
            set { _currentTurn = value; }
        }

        /// <summary>
        /// Resets the battle state to initial values for a new match.
        /// </summary>
        public void ResetBattle()
        {
            _currentTurn = 1;
            _selectedBattleSpellIndex = 0;
            _battleAnimationTime = 0f;
            _currentFrame = 0;
            _frameCounter = 0f;
            _currentSpellAnimation = null;
            _castingPlayer = 0;
            _pendingSpell = null;
            _pendingCaster = null;
            _pendingTarget = null;
            _spellDamageApplied = false;
            _turnTimer = 15f;
            _matchTimer = 300f;
            _skipNextParalyzeCheck = false;
            _paralyzeDisplayTimer = 0f;
            _player1DeathFrame = 0;
            _player2DeathFrame = 0;
            _isPaused = false;
            _battleLog.Clear();
        }

        /// <summary>
        /// Draws the entire battle screen UI, including timers, player stats, spell selection, and animations.
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <param name="modifier"></param>
        public void Draw(Player player1, Player player2, BattleModifier modifier)
        {
            // Draw healing particles only (no snow)
            _particleManager.DrawHealing();
            
            // Turn indicator (top-center)
            string currentPlayerName = _currentTurn == 1 ? player1.Name : player2.Name;
            string turnText = $"TURN: {currentPlayerName}";
            int turnTextWidth = Raylib.MeasureText(turnText, 40);
            Raylib.DrawRectangle((1920 - turnTextWidth) / 2 - 20, 20, turnTextWidth + 40, 60, new Color(0, 0, 0, 180));
            Raylib.DrawRectangleLines((1920 - turnTextWidth) / 2 - 20, 20, turnTextWidth + 40, 60, Color.Yellow);
            Raylib.DrawText(turnText, (1920 - turnTextWidth) / 2, 32, 40, Color.Yellow);

            // Turn Timer (top-center, below turn indicator)
            int turnSeconds = (int)Math.Ceiling(_turnTimer);
            Color turnTimerColor = _turnTimer <= 3f ? Color.Red : (_turnTimer <= 5f ? Color.Yellow : Color.White);
            string turnTimerText = $"Time: {turnSeconds}s";
            int turnTimerWidth = Raylib.MeasureText(turnTimerText, 28);
            Raylib.DrawRectangle((1920 - turnTimerWidth) / 2 - 15, 90, turnTimerWidth + 30, 45, new Color(0, 0, 0, 180));
            Raylib.DrawRectangleLines((1920 - turnTimerWidth) / 2 - 15, 90, turnTimerWidth + 30, 45, turnTimerColor);
            Raylib.DrawText(turnTimerText, (1920 - turnTimerWidth) / 2, 98, 28, turnTimerColor);

            // Match Timer (top-center, below turn timer)
            int matchMinutes = (int)(_matchTimer / 60f);
            int matchSeconds = (int)(_matchTimer % 60f);
            Color matchTimerColor = _matchTimer <= 60f ? Color.Red : (_matchTimer <= 120f ? Color.Yellow : Color.White);
            string matchTimerText = $"Match: {matchMinutes}:{matchSeconds:D2}";
            int matchTimerWidth = Raylib.MeasureText(matchTimerText, 40);
            Raylib.DrawRectangle((1920 - matchTimerWidth) / 2 - 20, 145, matchTimerWidth + 40, 60, new Color(0, 0, 0, 180));
            Raylib.DrawRectangleLines((1920 - matchTimerWidth) / 2 - 20, 145, matchTimerWidth + 40, 60, matchTimerColor);
            Raylib.DrawText(matchTimerText, (1920 - matchTimerWidth) / 2, 157, 40, matchTimerColor);

            // Draw wizard sprites with animation (if texture is loaded)
            Texture2D player1Sprite = _animation.GetWizardSpriteByCondition(player1.Condition, player1.SelectedWizardId);
            Texture2D player2Sprite = _animation.GetWizardSpriteByCondition(player2.Condition, player2.SelectedWizardId);
            
            if (player1Sprite.Width > 0 && player2Sprite.Width > 0)
            {
                // Calculate blinking effects for Hit (red) and Paralyzed (blue) status
                // Use a sine wave based on battle animation time to make the tint pulse
                int blinkValue1 = (player1.Condition == CharCondition.Hit || player1.Condition == CharCondition.Paralyzed)?
                    (int)(Math.Sin(_battleAnimationTime * 10f) * 127 + 128): 255;
                    
                    
                Color player1TintColor;
                if (player1.Condition == CharCondition.Hit)
                {
                    player1TintColor = new Color(255, blinkValue1, blinkValue1, 255); // Red blink
                }
                else if (player1.Condition == CharCondition.Paralyzed)
                {
                    player1TintColor = new Color(blinkValue1, blinkValue1, 255, 255); // Blue blink
                }
                else
                {
                    player1TintColor = Color.White;
                }
                
                int blinkValue2 = (player2.Condition == CharCondition.Hit || player2.Condition == CharCondition.Paralyzed)? 
                    (int)(Math.Sin(_battleAnimationTime * 10f) * 127 + 128): 255;
                    
                    
                Color player2TintColor;
                if (player2.Condition == CharCondition.Hit)
                {
                    player2TintColor = new Color(255, blinkValue2, blinkValue2, 255); // Red blink
                }
                else if (player2.Condition == CharCondition.Paralyzed)
                {
                    player2TintColor = new Color(blinkValue2, blinkValue2, 255, 255); // Blue blink
                }
                else
                {
                    player2TintColor = Color.White;
                }
                
                // Only draw normal wizard sprites if no spell animation is active for that player
                bool player1Casting = _currentSpellAnimation != null && _currentSpellAnimation.IsAnimating && _castingPlayer == 1;
                bool player2Casting = _currentSpellAnimation != null && _currentSpellAnimation.IsAnimating && _castingPlayer == 2;
                
                // Player 1 wizard (left side) - show idle/hurt sprite when not casting
                if (!player1Casting)
                {
                    int frameToUse = _currentFrame;
                    // For dead animation, use death frame counter and freeze at frame 4
                    if (player1.Condition == CharCondition.Dead)
                    {
                        frameToUse = _player1DeathFrame;
                    }
                    
                    Rectangle sourceRec1 = new Rectangle(frameToUse * _animation.WizardFrameWidth, 0, _animation.WizardFrameWidth, _animation.WizardFrameHeight);
                    float scale1 = 500f / _animation.WizardFrameHeight;
                    Rectangle destRec1 = new Rectangle(400, 200, _animation.WizardFrameWidth * scale1, _animation.WizardFrameHeight * scale1);
                    Raylib.DrawTexturePro(player1Sprite, sourceRec1, destRec1, new Vector2(0, 0), 0.0f, player1TintColor);
                }
                
                // Player 2 wizard (right side) - show idle/hurt sprite when not casting
                if (!player2Casting)
                {
                    int frameToUse = _currentFrame;
                    // For dead animation, use death frame counter and freeze at frame 4
                    if (player2.Condition == CharCondition.Dead)
                    {
                        frameToUse = _player2DeathFrame;
                        // For dead animation, use the exact frame without offset
                        Rectangle sourceRec2Dead = new Rectangle(frameToUse * _animation.WizardFrameWidth, 0, -_animation.WizardFrameWidth, _animation.WizardFrameHeight);
                        float scale2 = 500f / _animation.WizardFrameHeight;
                        Rectangle destRec2 = new Rectangle(1020, 200, _animation.WizardFrameWidth * scale2, _animation.WizardFrameHeight * scale2);
                        Raylib.DrawTexturePro(player2Sprite, sourceRec2Dead, destRec2, new Vector2(0, 0), 0.0f, player2TintColor);
                    }
                    else
                    {
                        // For other animations, use offset for mirroring
                        Rectangle sourceRec2 = new Rectangle((frameToUse + 1) * _animation.WizardFrameWidth, 0, -_animation.WizardFrameWidth, _animation.WizardFrameHeight);
                        float scale2 = 500f / _animation.WizardFrameHeight;
                        Rectangle destRec2 = new Rectangle(1020, 200, _animation.WizardFrameWidth * scale2, _animation.WizardFrameHeight * scale2);
                        Raylib.DrawTexturePro(player2Sprite, sourceRec2, destRec2, new Vector2(0, 0), 0.0f, player2TintColor);
                    }
                }
                
                // Draw spell animations if active - casting animation ONLY on caster
                if (_currentSpellAnimation != null && _currentSpellAnimation.IsAnimating)
                {
                    if (_castingPlayer == 1)
                    {
                        // Player 1 is casting - show Light_ball animation on player 1 (always white, no red blink)
                        float scale1 = 500f / _animation.WizardFrameHeight;
                        Rectangle castRec1 = new Rectangle(400, 200, _animation.WizardFrameWidth * scale1, _animation.WizardFrameHeight * scale1);
                        _currentSpellAnimation.DrawCastingAnimation(castRec1, new Vector2(0, 0), Color.White, false);
                        _currentSpellAnimation.DrawProjectile(scale1);
                    }
                    else if (_castingPlayer == 2)
                    {
                        // Player 2 is casting - show Light_ball animation on player 2 (flipped, always white, no red blink)
                        float scale2 = 500f / _animation.WizardFrameHeight;
                        Rectangle castRec2 = new Rectangle(1020, 200, _animation.WizardFrameWidth * scale2, _animation.WizardFrameHeight * scale2);
                        _currentSpellAnimation.DrawCastingAnimation(castRec2, new Vector2(0, 0), Color.White, true);
                        _currentSpellAnimation.DrawProjectile(scale2);
                    }
                }
            }

            // Player 1 Stats (Left side, below timers)
            DrawPlayerStats(player1, 20, 20, _currentTurn == 1);

            // Player 2 Stats (Right side, below timer)
            DrawPlayerStats(player2, 1420, 20, _currentTurn == 2);

            // Spell Selection (Bottom)
            Player currentPlayer = _currentTurn == 1 ? player1 : player2;
            DrawSpellSelection(currentPlayer, 200, 650);

            // Game Over Message - centered overlay if game is over
            if (player1.Condition == CharCondition.Dead || player2.Condition == CharCondition.Dead)
            {
                string gameOverText = "GAME OVER - PRESS ENTER TO CONTINUE";
                int gameOverWidth = Raylib.MeasureText(gameOverText, 40);
                Raylib.DrawRectangle((1920 - gameOverWidth) / 2 - 40, 400, gameOverWidth + 80, 80, new Color(0, 0, 0, 220));
                Raylib.DrawRectangleLines((1920 - gameOverWidth) / 2 - 40, 400, gameOverWidth + 80, 80, Color.Red);
                Raylib.DrawText(gameOverText, (1920 - gameOverWidth) / 2, 420, 40, Color.Red);
            }

            // Instructions - positioned below spell selection
            Raylib.DrawText("ARROW KEYS - Select Spell | ENTER - Cast Spell | I / O - Scroll Log | P - Pause", 70, 880, 18, Color.LightGray);

            // Battle Log (left side)
            _battleLog.Draw(50, 910, 1000, 150, 16);

            // Battle Modifier (right side of battle log)
            string modifierTitle = $"MODIFIER: {modifier.Type}";
            string modifierDesc = modifier.Description;
            int modifierTitleWidth = Raylib.MeasureText(modifierTitle, 28);
            int modifierDescWidth = Raylib.MeasureText(modifierDesc, 20);
            Raylib.DrawRectangle(1080, 910, 790, 150, new Color(128, 0, 128, 180));
            Raylib.DrawRectangleLines(1080, 910, 790, 150, Color.Purple);
            Raylib.DrawText(modifierTitle, 1080 + (790 - modifierTitleWidth) / 2, 930, 28, Color.White);
            Raylib.DrawText(modifierDesc, 1080 + (790 - modifierDescWidth) / 2, 970, 20, Color.LightGray);

            // Draw pause overlay if paused
            if (_isPaused)
            {
                // Semi-transparent dark overlay
                Raylib.DrawRectangle(0, 0, 1920, 1080, new Color(0, 0, 0, 180));
                
                // Pause text
                string pauseText = "PAUSED";
                int pauseWidth = Raylib.MeasureText(pauseText, 80);
                Raylib.DrawText(pauseText, (1920 - pauseWidth) / 2, 400, 80, Color.Yellow);
                
                // Instructions
                string instructionText = "Press P to Resume";
                int instructionWidth = Raylib.MeasureText(instructionText, 40);
                Raylib.DrawText(instructionText, (1920 - instructionWidth) / 2, 520, 40, Color.White);
            }
        }

        /// <summary>
        /// Draws the stats panel for a player, including HP bar, status, and equipped spells.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="isActive"></param>
        public void DrawPlayerStats(Player player, int x, int y, bool isActive)
        {
            Color borderColor = isActive ? Color.Yellow : Color.White;
            
            // Stats panel - slightly taller for more space
            Raylib.DrawRectangle(x, y, 480, 280, new Color(0, 0, 0, 180));
            Raylib.DrawRectangleLines(x, y, 480, 280, borderColor);

            // Player name
            Raylib.DrawText(player.Name, x + 20, y + 20, 30, Color.SkyBlue);

            // HP Bar
            Raylib.DrawText($"HP: {player.CurrentHP}/{player.MaxHP}", x + 20, y + 60, 22, Color.White);
            int hpBarWidth = 440;
            int hpBarHeight = 25;
            float hpPercent = (float)player.CurrentHP / player.MaxHP;
            Raylib.DrawRectangle(x + 20, y + 90, hpBarWidth, hpBarHeight, Color.DarkGray);
            Raylib.DrawRectangle(x + 20, y + 90, (int)(hpBarWidth * hpPercent), hpBarHeight, Color.Green);
            Raylib.DrawRectangleLines(x + 20, y + 90, hpBarWidth, hpBarHeight, Color.White);

            // Add a gap between HP bar and status
            Raylib.DrawText($"Status: {player.Condition}", x + 20, y + 130, 18, Color.White);

            // Active Spells list
            Raylib.DrawText("EQUIPPED SPELLS:", x + 20, y + 160, 18, Color.Yellow);
            for (int i = 0; i < player.ActiveSpells.Count; i++)
            {
                string spellInfo = $"{i + 1}. {player.ActiveSpells[i].Name} ({player.ActiveSpells[i].CastRemaining}/{player.ActiveSpells[i].CastLimit})";
                Raylib.DrawText(spellInfo, x + 30, y + 185 + (i * 20), 16, Color.White);
            }
        }

        /// <summary>
        /// Draws the spell selection UI for the current player.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void DrawSpellSelection(Player player, int x, int y)
        {
            Raylib.DrawRectangle(x, y, 1520, 220, new Color(0, 0, 0, 180));
            Raylib.DrawRectangleLines(x, y, 1520, 220, Color.Yellow);

            Raylib.DrawText("SELECT SPELL TO CAST:", x + 20, y + 20, 25, Color.Yellow);

            for (int i = 0; i < player.ActiveSpells.Count; i++)
            {
                Color textColor = i == _selectedBattleSpellIndex ? Color.Yellow : Color.White;
                Color bgColor = i == _selectedBattleSpellIndex ? new Color(255, 255, 0, 50) : new Color(255, 255, 255, 0);

                if (i == _selectedBattleSpellIndex)
                {
                    // Highlight the selected entry
                    Raylib.DrawRectangle(x + 10, y + 55 + (i * 40), 1500, 35, bgColor);
                }

                string spellText = $"{i + 1}. {player.ActiveSpells[i].Name} - Casts: {player.ActiveSpells[i].CastRemaining}/{player.ActiveSpells[i].CastLimit}";
                Raylib.DrawText(spellText, x + 30, y + 60 + (i * 40), 22, textColor);
            }
        }

        /// <summary>
        /// Updates the battle screen state each frame, handling input, timers, spell animations, and turn logic.
        /// </summary>
        /// <param name="gameManager"></param>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <param name="battleModifier"></param>
        public void Update(GameManager gameManager, Player player1, Player player2, BattleModifier battleModifier)
        {
            // Handle pause toggle - pressing P toggles pause and plays UI sound
            if (Raylib.IsKeyPressed(KeyboardKey.P))
            {
                _isPaused = !_isPaused;
                _audioManager.PlaySelectSound();
                return;
            }

            // Skip all game logic if paused
            if (_isPaused)
            {
                return;
            }

            Player currentPlayer = _currentTurn == 1 ? player1 : player2;
            Player targetPlayer = _currentTurn == 1 ? player2 : player1;

            float deltaTime = Raylib.GetFrameTime();
            
            // Check if either player is dead - pause timers if game is over
            bool isGameOver = player1.Condition == CharCondition.Dead || player2.Condition == CharCondition.Dead;
            
            // Only update timers if battle has started (after coin flip) and game is not over
            if (_coinFlipScreen.BattleStarted && !isGameOver)
            {
                // Update timers
                _turnTimer -= deltaTime;
                _matchTimer -= deltaTime;
            }
            
            // Check match timer - if time runs out, player with more HP wins (only if battle started)
            if (_coinFlipScreen.BattleStarted && _matchTimer <= 0f)
            {
                _matchTimer = 0f;
                _battleLog.AddMessage("Time's up! Match ended!");
                
                // Determine winner by HP and force the loser to die to maintain consistency
                if (player1.CurrentHP > player2.CurrentHP)
                {
                    player2.TakeDamage(player2.CurrentHP); // Force player 2 to lose
                    Player winner = player1;
                    Player loser = player2;
                    gameManager.UIManager.SetEndScreenWinner(winner, loser);
                    gameManager.ChangeState(GameState.EndScreen);
                    return;
                }
                else if (player2.CurrentHP > player1.CurrentHP)
                {
                    player1.TakeDamage(player1.CurrentHP); // Force player 1 to lose
                    Player winner = player2;
                    Player loser = player1;
                    gameManager.UIManager.SetEndScreenWinner(winner, loser);
                    gameManager.ChangeState(GameState.EndScreen);
                    return;
                }
                else
                {
                    // Tie - it's a draw
                    _battleLog.AddMessage("It's a draw!");
                    gameManager.UIManager.SetEndScreenDraw(player1, player2);
                    gameManager.ChangeState(GameState.EndScreen);
                    return;
                }
            }
            
            // Check turn timer - auto skip turn if time runs out (only if battle started)
            if (_coinFlipScreen.BattleStarted && _turnTimer <= 0f)
            {
                _battleLog.AddMessage($"{currentPlayer.Name} ran out of time!");
                _turnTimer = _turnTimeLimit; // Reset turn timer
                
                // Clear paralysis if paralyzed player's turn ended
                if (currentPlayer.Condition == CharCondition.Paralyzed)
                {
                    currentPlayer.ClearCondition();
                }
                
                _currentTurn = _currentTurn == 1 ? 2 : 1;
                _selectedBattleSpellIndex = 0;
                
                // Process effects when turn changes (apply ongoing effects like regen/poison)
                player1.ProcessEffect(_battleLog);
                player2.ProcessEffect(_battleLog);
                return;
            }

            // Check if current player is paralyzed - auto skip turn and give it back to opponent
            if (currentPlayer.Condition == CharCondition.Paralyzed && !_skipNextParalyzeCheck)
            {
                _battleLog.AddMessage($"{currentPlayer.Name} is paralyzed and cannot move!");
                
                // Remove the ParalyzeEffect but keep the paralyzed status visible briefly
                currentPlayer.ClearAllEffects(); // Remove the ParalyzeEffect from active effects
                
                _turnTimer = _turnTimeLimit; // Reset turn timer
                _currentTurn = _currentTurn == 1 ? 2 : 1;
                _selectedBattleSpellIndex = 0;
                _skipNextParalyzeCheck = true; // Skip the check on the next turn (caster's extra turn)
                _paralyzeDisplayTimer = _paralyzeDisplayDuration; // Start timer to show paralyzed status
                
                // Process effects when turn changes (won't reapply paralyze since effect was removed)
                player1.ProcessEffect(_battleLog);
                player2.ProcessEffect(_battleLog);
                return;
            }
            
            // Update paralyze display timer and clear status after duration
            if (_paralyzeDisplayTimer > 0f)
            {
                _paralyzeDisplayTimer -= deltaTime;
                
                if (_paralyzeDisplayTimer <= 0f)
                {
                    // Clear paralyzed status after timer expires
                    if (player1.Condition == CharCondition.Paralyzed)
                    {
                        player1.ClearCondition();
                    }
                    if (player2.Condition == CharCondition.Paralyzed)
                    {
                        player2.ClearCondition();                      
                    }
                }
            }
            
            // Reset skip flag at start of each turn (after paralysis handling)
            if (_skipNextParalyzeCheck && _paralyzeDisplayTimer <= 0f)
            {
                _skipNextParalyzeCheck = false;
            }

            // Update player internal timers/logic (e.g., effect durations)
            player1.Update(deltaTime);
            player2.Update(deltaTime);

            // Update healing particles only
            _particleManager.UpdateHealing(deltaTime);

            // Update sprite animation timing for wizard frames
            _battleAnimationTime += Raylib.GetFrameTime();
            _frameCounter += Raylib.GetFrameTime();
            
            if (_frameCounter >= _animation.WizardFrameSpeed)
            {
                _frameCounter = 0f;
                _currentFrame = (_currentFrame + 1) % _animation.WizardFramesPerRow;
                
                // Update death animation frames - each wizard has different max frames
                // Wiz1 (id=0): 3 frames, Wiz2 (id=1): 2 frames, Wiz3 (id=2): 4 frames
                if (player1.Condition == CharCondition.Dead)
                {
                    int maxFrames = _animation.GetDeathFrameCount(player1.SelectedWizardId);
                    if (_player1DeathFrame < maxFrames - 1)
                    {
                        _player1DeathFrame++;
                    }
                }
                if (player2.Condition == CharCondition.Dead)
                {
                    int maxFrames = _animation.GetDeathFrameCount(player2.SelectedWizardId);
                    if (_player2DeathFrame < maxFrames - 1)
                    {
                        _player2DeathFrame++;
                    }
                }
            }

            // Update spell animation (projectile movement, hit detection, etc.)
            if (_currentSpellAnimation != null)
            {
                _currentSpellAnimation.Update(Raylib.GetFrameTime());
                
                // Apply spell damage when projectile hits
                if (_currentSpellAnimation.HasHit && !_spellDamageApplied && _pendingSpell != null)
                {
                    // Play appropriate hit sound based on spell type
                    if (_pendingSpell.Name == "Fireball")
                    {
                        _audioManager.PlayFireballSound();
                    }
                    else if (_pendingSpell.Name == "Lightning")
                    {
                        _audioManager.PlayLightningSound();
                    }
                    else if (_pendingSpell.Name == "Poison")
                    {
                        _audioManager.PlayPoisonSound();
                    }
                    
                    // Cast spell (Poison uses modifier overload)
                    if (_pendingSpell is Poison poisonSpell && _pendingCaster != null && _pendingTarget != null)
                    {
                        poisonSpell.CastWithModifier(_pendingCaster, _pendingTarget, battleModifier);
                    }
                    else if (_pendingCaster != null && _pendingTarget != null)
                    {
                        _pendingSpell.Cast(_pendingCaster, _pendingTarget);
                    }
                    
                    // Process ongoing effects after spell lands (poison ticks, regen, etc.)
                    player1.ProcessEffect(_battleLog);
                    player2.ProcessEffect(_battleLog);
                    
                    _spellDamageApplied = true;
                    
                    // Reset turn timer after attack spell hits
                    _turnTimer = _turnTimeLimit;
                }
                
                // If spell hit and animation finished, clear casting player and pending spell data
                if (_currentSpellAnimation.HasHit && _currentSpellAnimation.IsAnimating == false)
                {
                    _currentSpellAnimation = null;
                    _castingPlayer = 0;
                    _pendingSpell = null;
                    _pendingCaster = null;
                    _pendingTarget = null;
                    _spellDamageApplied = false;
                }
            }

            // Navigate spells with Up/Down keys
            if (Raylib.IsKeyPressed(KeyboardKey.Down))
            {
                if (currentPlayer.ActiveSpells.Count > 0)
                {
                    _audioManager.PlayKeySound();
                    _selectedBattleSpellIndex = (_selectedBattleSpellIndex + 1) % currentPlayer.ActiveSpells.Count;
                }
            }
            else if (Raylib.IsKeyPressed(KeyboardKey.Up))
            {
                if (currentPlayer.ActiveSpells.Count > 0)
                {
                    _audioManager.PlayKeySound();
                    _selectedBattleSpellIndex = (_selectedBattleSpellIndex - 1 + currentPlayer.ActiveSpells.Count) % currentPlayer.ActiveSpells.Count;
                }
            }

            // Scroll battle log with I/O keys
            if (Raylib.IsKeyPressed(KeyboardKey.I))
            {
                _audioManager.PlayKeySound();
                _battleLog.ScrollUp();
            }
            else if (Raylib.IsKeyPressed(KeyboardKey.O))
            {
                _audioManager.PlayKeySound();
                _battleLog.ScrollDown();
            }

            // If game is over, pressing Enter goes to end screen
            if (isGameOver && Raylib.IsKeyPressed(KeyboardKey.Enter))
            {
                _audioManager.PlaySelectSound();
                Player winner = player1.IsAlive() ? player1 : player2;
                Player loser = player1.IsAlive() ? player2 : player1;
                gameManager.UIManager.SetEndScreenWinner(winner, loser);
                gameManager.ChangeState(GameState.EndScreen);
                return;
            }

            // Cast spell (only if game is not over)
            if (!isGameOver && Raylib.IsKeyPressed(KeyboardKey.Enter) && _currentSpellAnimation == null)
            {
                _audioManager.PlaySelectSound();
                // Check if spell list is valid and index is in bounds
                if (currentPlayer.ActiveSpells.Count == 0)
                {
                    return;
                }
                
                if (_selectedBattleSpellIndex >= currentPlayer.ActiveSpells.Count)
                {
                    _selectedBattleSpellIndex = 0;
                }
                
                Spell selectedSpell = currentPlayer.ActiveSpells[_selectedBattleSpellIndex];
                
                if (selectedSpell.Castable())
                {
                    // Determine target based on spell type (healing spells target caster)
                    Player target = selectedSpell.Effect == SpellEffect.Heal || selectedSpell.Effect == SpellEffect.Regen || selectedSpell.Effect == SpellEffect.Cure
                        ? currentPlayer
                        : targetPlayer;
                    
                    bool isHealingSpell = selectedSpell.Effect == SpellEffect.Heal || selectedSpell.Effect == SpellEffect.Regen || selectedSpell.Effect == SpellEffect.Cure;

                    // Store which player is casting before turn switches
                    _castingPlayer = _currentTurn;

                    // For healing spells, apply effect immediately without projectile animation
                    if (isHealingSpell)
                    {
                        // Play cast sound and apply healing immediately
                        _audioManager.PlayCastSound();
                        selectedSpell.Cast(currentPlayer, target);
                        
                        // Play appropriate healing sound
                        if (selectedSpell.Effect == SpellEffect.Cure)
                        {
                            _audioManager.PlayCureSound();
                        }
                        else // Heal or Regen
                        {
                            _audioManager.PlayHealSound();
                        }
                        
                        // Process effects (in case regen or other status applied)
                        player1.ProcessEffect(_battleLog);
                        player2.ProcessEffect(_battleLog);
                        
                        // Spawn healing particles around the caster
                        Vector2 wizardPos = _currentTurn == 1 
                            ? new Vector2(400 + 250, 200 + 250)  // Player 1 center
                            : new Vector2(1020 + 250, 200 + 250); // Player 2 center
                        
                        Color particleColor;
                        if (selectedSpell.Effect == SpellEffect.Heal)
                        {
                            particleColor = new Color(0, 100, 0, 255); // Dark green
                        }
                        else if (selectedSpell.Effect == SpellEffect.Cure)
                        {
                            particleColor = new Color(255, 215, 0, 255); // Gold
                        }
                        else // Regen
                        {
                            particleColor = new Color(0, 255, 0, 255); // Bright green
                        }
                        
                        _particleManager.SpawnHealingParticles(wizardPos.X, wizardPos.Y, particleColor, 250);
                        
                        _battleLog.AddMessage($"{currentPlayer.Name} cast {selectedSpell.Name}!");
                        
                        // Reset turn timer after healing spell cast
                        _turnTimer = _turnTimeLimit;
                    }
                    else
                    {
                        // For attack spells, start projectile animation and defer damage until hit
                        _audioManager.PlayCastSound();
                        
                        Vector2 casterPos = _currentTurn == 1 
                            ? new Vector2(400 + 320, 200 + 280)  // Player 1 hand position
                            : new Vector2(1020 + 180, 200 + 280); // Player 2 hand position
                        Vector2 targetPos = _currentTurn == 1 
                            ? new Vector2(1020 + 250, 200 + 300) // Aim at Player 2 chest center
                            : new Vector2(400 + 250, 200 + 300);  // Aim at Player 1 chest center
                        
                        // Create and start spell animation (projectile travel)
                        int casterWizardId = currentPlayer.SelectedWizardId;
                        _currentSpellAnimation = _animation.CreateSpellAnimation(casterWizardId);
                        _currentSpellAnimation.StartAnimation(casterPos, targetPos);

                        // Store spell to be cast when projectile hits
                        _pendingSpell = selectedSpell;
                        _pendingCaster = currentPlayer;
                        _pendingTarget = target;
                        _spellDamageApplied = false;
                        
                        _battleLog.AddMessage($"{currentPlayer.Name} cast {selectedSpell.Name}!");
                    }

                    // Check win condition immediately after casting (some spells might be instant)
                    if (!player1.IsAlive() || !player2.IsAlive())
                    {
                        Player winner = player1.IsAlive() ? player1 : player2;
                        Player loser = player1.IsAlive() ? player2 : player1;
                        gameManager.UIManager.SetEndScreenWinner(winner, loser);
                        gameManager.ChangeState(GameState.EndScreen);
                        return;
                    }

                    // Switch turn (and reset casting player if healing spell)
                    if (isHealingSpell)
                    {
                        _castingPlayer = 0;
                    }
                    _currentTurn = _currentTurn == 1 ? 2 : 1;
                    _selectedBattleSpellIndex = 0;
                }
                else
                {
                    // Spell had no casts left
                    _battleLog.AddMessage($"{selectedSpell.Name} has no casts remaining!");
                }
            }
        }
    }
}
