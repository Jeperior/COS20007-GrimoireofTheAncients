using System;
using System.Numerics;
using Raylib_cs;

namespace Grimoire
{
    /// <summary>
    /// Handles the end screen UI and interactions.
    /// </summary>
    public class EndScreen
    {

        private int _currentFrame;                  // Current frame for wizard animation
        private float _frameCounter;                // Timer for frame switching
        private bool _isDraw;                       // Is the game a draw
        private int _player1XPGain;                 // XP gained by player 1
        private int _player2XPGain;                 // XP gained by player 2
        private Animation _animation;               // Animation system
        private ParticleManager _particleManager;   // Particle system
        private Player? _winner;                    // Winner player
        private Player? _loser;                     // Loser player
        
        /// <summary>
        /// Initializes a new instance of the EndScreen class.
        /// </summary>
        /// <param name="animation"></param>
        /// <param name="particleManager"></param>
        public EndScreen(Animation animation, ParticleManager particleManager)
        {
            _animation = animation;
            _particleManager = particleManager;
            _winner = null;
            _loser = null;
            _currentFrame = 0;
            _frameCounter = 0f;
            _isDraw = false;
            _player1XPGain = 0;
            _player2XPGain = 0;
        }

        /// <summary>
        /// Sets the winner and loser for the end screen.
        /// </summary>
        /// <param name="winner"></param>
        /// <param name="loser"></param>
        public void SetWinner(Player winner, Player loser)
        {
            _winner = winner;
            _loser = loser;
            _isDraw = false;
            
            // Winner gets 50 XP, loser gets 10
            _player1XPGain = 50;
            _player2XPGain = 10;
        }
        
        /// <summary>
        /// Sets the end screen for a draw.
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param> <summary>
        /// 
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        public void SetDraw(Player player1, Player player2)
        {
            _winner = player1;
            _loser = player2;
            _isDraw = true;
            
            // Both players get 10 XP for draw
            _player1XPGain = 10;
            _player2XPGain = 10;
        }

        /// <summary>
        /// Updates the end screen logic.
        /// </summary>
        /// <param name="gameManager"></param> <summary>
        /// 
        /// </summary>
        /// <param name="gameManager"></param>
        public void Update(GameManager gameManager)
        {
            float deltaTime = Raylib.GetFrameTime();
            _frameCounter += deltaTime;

            // Animate the wizard sprite (Win.png has 10 frames)
            if (_frameCounter >= _animation.WizardFrameSpeed)
            {
                _frameCounter = 0f;
                _currentFrame = (_currentFrame + 1) % _animation.WizardWinFrames;
            }

            if (Raylib.IsKeyPressed(KeyboardKey.Enter))
            {
                gameManager.ChangeState(GameState.MainMenu);
            }
        }

        /// <summary>
        /// Draws the end screen.
        /// </summary> <summary>
        /// 
        /// </summary>
        public void Draw()
        {
            // Initialize and draw snow particles
            if (_particleManager.ParticleCount == 0)
            {
                _particleManager.InitializeParticles(1920, 1080, 3000);
            }

            _particleManager.UpdateSnow(Raylib.GetFrameTime(), 1920, 1080);
            _particleManager.DrawSnow();

            // Dark background overlay
            Raylib.DrawRectangle(0, 0, 1920, 1080, new Color(0, 0, 0, 200));

            if (_winner != null && _loser != null)
            {
                if (_isDraw)
                {
                    
                    // Draw title
                    string drawText = "DRAW!";
                    int drawWidth = Raylib.MeasureText(drawText, 80);
                    Raylib.DrawText(drawText, (1920 - drawWidth) / 2, 50, 80, Color.Orange);

                    // Draw both players side by side
                    string player1Text = _winner.Name;
                    string player2Text = _loser.Name;
                    
                    int player1Width = Raylib.MeasureText(player1Text, 50);
                    int player2Width = Raylib.MeasureText(player2Text, 50);
                    
                    // Center names within their boxes
                    Raylib.DrawText(player1Text, 200 + (500 - player1Width) / 2, 160, 50, Color.Yellow);
                    Raylib.DrawText(player2Text, 1220 + (500 - player2Width) / 2, 160, 50, Color.Yellow);

                    // Draw both wizard sprites
                    Texture2D player1Sprite = _animation.GetWizardSpriteByCondition(CharCondition.Idle, _winner.SelectedWizardId);
                    Texture2D player2Sprite = _animation.GetWizardSpriteByCondition(CharCondition.Idle, _loser.SelectedWizardId);
                    
                    if (player1Sprite.Width > 0)
                    {
                        Rectangle sourceRec = new Rectangle(_currentFrame * _animation.WizardFrameWidth, 0, _animation.WizardFrameWidth, _animation.WizardFrameHeight);
                        float scale = 500f / _animation.WizardFrameHeight;
                        float wizardWidth = _animation.WizardFrameWidth * scale;
                        // Center within left box (200 to 700, width 500)
                        float wizardX = 200 + (500 - wizardWidth) / 2;
                        Rectangle destRec = new Rectangle(wizardX, 300, wizardWidth, _animation.WizardFrameHeight * scale);
                        Raylib.DrawTexturePro(player1Sprite, sourceRec, destRec, new Vector2(0, 0), 0.0f, Color.White);
                    }
                    
                    if (player2Sprite.Width > 0)
                    {
                        Rectangle sourceRec = new Rectangle(_currentFrame * _animation.WizardFrameWidth, 0, -_animation.WizardFrameWidth, _animation.WizardFrameHeight);
                        float scale = 500f / _animation.WizardFrameHeight;
                        float wizardWidth = _animation.WizardFrameWidth * scale;
                        // Center within right box (1220 to 1720, width 500)
                        float wizardX = 1220 + (500 - wizardWidth) / 2;
                        Rectangle destRec = new Rectangle(wizardX, 300, wizardWidth, _animation.WizardFrameHeight * scale);
                        Raylib.DrawTexturePro(player2Sprite, sourceRec, destRec, new Vector2(0, 0), 0.0f, Color.White);
                    }

                    // Display XP gains
                    Raylib.DrawRectangle(200, 820, 500, 100, new Color(0, 0, 0, 180));
                    Raylib.DrawRectangleLines(200, 820, 500, 100, Color.Orange);
                    Raylib.DrawText($"{_winner.Name}", 250, 840, 25, Color.White);
                    Raylib.DrawText($"XP Gain: +{_player1XPGain}", 250, 875, 25, Color.Gold);
                    
                    Raylib.DrawRectangle(1220, 820, 500, 100, new Color(0, 0, 0, 180));
                    Raylib.DrawRectangleLines(1220, 820, 500, 100, Color.Orange);
                    Raylib.DrawText($"{_loser.Name}", 1270, 840, 25, Color.White);
                    Raylib.DrawText($"XP Gain: +{_player2XPGain}", 1270, 875, 25, Color.Gold);
                }
                else
                {
                    // Victory title
                    string victoryText = "VICTORY!";
                    int victoryWidth = Raylib.MeasureText(victoryText, 80);
                    Raylib.DrawText(victoryText, (1920 - victoryWidth) / 2, 50, 80, Color.Gold);

                    // Winner name
                    string winnerText = $"{_winner.Name} WINS!";
                    int winnerWidth = Raylib.MeasureText(winnerText, 60);
                    Raylib.DrawText(winnerText, (1920 - winnerWidth) / 2, 160, 60, Color.Yellow);

                    // Draw winner's wizard sprite (large, centered)
                    Texture2D winnerSprite = _animation.GetWizardWinSprite(_winner.SelectedWizardId);
                    if (winnerSprite.Width > 0)
                    {
                        Rectangle sourceRec = new Rectangle(_currentFrame * _animation.WizardFrameWidth, 0, _animation.WizardFrameWidth, _animation.WizardFrameHeight);
                        float scale = 600f / _animation.WizardFrameHeight; // Scale for victory screen
                        float wizardWidth = _animation.WizardFrameWidth * scale;
                        float wizardHeight = _animation.WizardFrameHeight * scale;
                        
                        // Center the wizard horizontally and position higher vertically
                        float wizardX = (1920 - wizardWidth) / 2;
                        float wizardY = (1080 - wizardHeight) / 2 - 150;
                        
                        Rectangle destRec = new Rectangle(wizardX, wizardY, wizardWidth, wizardHeight);
                        Raylib.DrawTexturePro(winnerSprite, sourceRec, destRec, new Vector2(0, 0), 0.0f, Color.White);
                    }

                    // Winner stats with XP gain
                    Raylib.DrawRectangle(660, 820, 600, 150, new Color(0, 0, 0, 180));
                    Raylib.DrawRectangleLines(660, 820, 600, 150, Color.Gold);
                    Raylib.DrawText($"Final HP: {_winner.CurrentHP}/{_winner.MaxHP}", 700, 840, 30, Color.Green);
                    Raylib.DrawText($"Defeated: {_loser.Name}", 700, 875, 25, Color.Gray);
                    Raylib.DrawText($"XP Gain: +{_player1XPGain}", 700, 910, 30, Color.Gold);
                }
            }
            else
            {
                // Fallback if no winner set
                string gameOverText = "GAME OVER";
                int gameOverWidth = Raylib.MeasureText(gameOverText, 80);
                Raylib.DrawText(gameOverText, (1920 - gameOverWidth) / 2, 400, 80, Color.Red);
            }

            // Instructions
            string instructionText = "Press ENTER to Return to Main Menu";
            int instructionWidth = Raylib.MeasureText(instructionText, 25);
            Raylib.DrawText(instructionText, (1920 - instructionWidth) / 2, 980, 25, Color.LightGray);
        }
    }
}
