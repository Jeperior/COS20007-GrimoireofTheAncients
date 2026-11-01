using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace Grimoire
{
    /// <summary>
    /// Handles wizard selection for both players based on their unlocked wizards.
    /// </summary>
    public class WizardSelectScreen
    {
        private Animation _animation;                     // Animation system used to get wizard sprites and frame info
        private ParticleManager _particleManager;         // Manages snow/particle effects on the screen
        private AudioManager _audioManager;               // Plays UI/key/select/back sounds
        private ProfileManager _profileManager;           // Loads/saves player profiles
        private PlayerProfile? _player1Profile;           // Currently loaded profile for player 1
        private PlayerProfile? _player2Profile;           // Currently loaded profile for player 2
        private int _player1SelectedIndex;                // Index into PlayerProfile.UnlockedWizards for player 1
        private int _player2SelectedIndex;                // Index into PlayerProfile.UnlockedWizards for player 2
        private bool _player1Confirmed;                   // Whether player 1 has confirmed their selection
        private bool _player2Confirmed;                   // Whether player 2 has confirmed their selection
        private float _animationTime;                     // Time accumulator used to advance animation frames
        private int _currentFrame;                        // Current animation frame index for displaying sprites
        private int _wizardDisplayWidth;                  // Width of the wizard display area (pixels)
        private int _wizardDisplayHeight;                 // Height of the wizard display area (pixels)
        
        /// <summary>
        /// Initializes a new instance of the WizardSelectScreen class.
        /// </summary>
        /// <param name="animation"></param>
        /// <param name="particleManager"></param>
        /// <param name="audioManager"></param>
        public WizardSelectScreen(Animation animation, ParticleManager particleManager, AudioManager audioManager)
        {
            _animation = animation;
            _particleManager = particleManager;
            _audioManager = audioManager;
            _profileManager = new ProfileManager();
            
            _player1SelectedIndex = 0;
            _player2SelectedIndex = 0;
            _player1Confirmed = false;
            _player2Confirmed = false;
            _animationTime = 0f;
            _currentFrame = 0;
            _wizardDisplayWidth = 350;
            _wizardDisplayHeight = 400;
        }
        
        /// <summary>
        /// Initializes the wizard selection screen with the given player profiles.
        /// </summary>
        /// <param name="player1Profile"></param>
        /// <param name="player2Profile"></param> <summary>
        /// 
        /// </summary>
        /// <param name="player1Profile"></param>
        /// <param name="player2Profile"></param>
        public void Initialize(PlayerProfile player1Profile, PlayerProfile player2Profile)
        {
            _player1Profile = player1Profile;
            _player2Profile = player2Profile;
            
            // Start with each player's currently selected wizard
            _player1SelectedIndex = _player1Profile.UnlockedWizards.IndexOf(_player1Profile.SelectedWizardId);
            _player2SelectedIndex = _player2Profile.UnlockedWizards.IndexOf(_player2Profile.SelectedWizardId);
            
            if (_player1SelectedIndex < 0) _player1SelectedIndex = 0;
            if (_player2SelectedIndex < 0) _player2SelectedIndex = 0;
            
            _player1Confirmed = false;
            _player2Confirmed = false;
            _animationTime = 0f;
            _currentFrame = 0;
        }
        
        /// <summary>
        /// Updates the wizard selection screen logic.
        /// </summary>
        /// <param name="gameManager"></param>
        /// <returns></returns> <summary>
        /// 
        /// </summary>
        /// <param name="gameManager"></param>
        /// <returns></returns>
        public bool Update(GameManager gameManager)
        {
            if (_player1Profile == null || _player2Profile == null) return false;
            
            _particleManager.Update(Raylib.GetFrameTime(), 1920, 1080);
            
            // Update animation
            _animationTime += Raylib.GetFrameTime();
            if (_animationTime >= 0.1f)
            {
                _animationTime = 0f;
                _currentFrame = (_currentFrame + 1) % 7; // Idle animation has 7 frames
            }
            
            // Player 1 controls (left side)
            if (!_player1Confirmed)
            {
                if (Raylib.IsKeyPressed(KeyboardKey.A))
                {
                    _audioManager.PlayKeySound();
                    _player1SelectedIndex--;
                    if (_player1SelectedIndex < 0) _player1SelectedIndex = _player1Profile.UnlockedWizards.Count - 1;
                }
                else if (Raylib.IsKeyPressed(KeyboardKey.D))
                {
                    _audioManager.PlayKeySound();
                    _player1SelectedIndex++;
                    if (_player1SelectedIndex >= _player1Profile.UnlockedWizards.Count) _player1SelectedIndex = 0;
                }
                else if (Raylib.IsKeyPressed(KeyboardKey.Space))
                {
                    _audioManager.PlaySelectSound();
                    _player1Confirmed = true;
                    _player1Profile.SelectedWizardId = _player1Profile.UnlockedWizards[_player1SelectedIndex];
                    _profileManager.SaveProfile(_player1Profile);
                    Console.WriteLine($"[WizardSelect] {_player1Profile.Username} selected wizard {_player1Profile.SelectedWizardId}");
                }
            }
            
            // Player 2 controls (right side)
            if (!_player2Confirmed)
            {
                if (Raylib.IsKeyPressed(KeyboardKey.Left))
                {
                    _audioManager.PlayKeySound();
                    _player2SelectedIndex--;
                    if (_player2SelectedIndex < 0) _player2SelectedIndex = _player2Profile.UnlockedWizards.Count - 1;
                }
                else if (Raylib.IsKeyPressed(KeyboardKey.Right))
                {
                    _audioManager.PlayKeySound();
                    _player2SelectedIndex++;
                    if (_player2SelectedIndex >= _player2Profile.UnlockedWizards.Count) _player2SelectedIndex = 0;
                }
                else if (Raylib.IsKeyPressed(KeyboardKey.Enter))
                {
                    _audioManager.PlaySelectSound();
                    _player2Confirmed = true;
                    _player2Profile.SelectedWizardId = _player2Profile.UnlockedWizards[_player2SelectedIndex];
                    _profileManager.SaveProfile(_player2Profile);
                    Console.WriteLine($"[WizardSelect] {_player2Profile.Username} selected wizard {_player2Profile.SelectedWizardId}");
                }
            }
            
            // Backspace to unconfirm both players
            if (Raylib.IsKeyPressed(KeyboardKey.Backspace))
            {
                if (_player1Confirmed || _player2Confirmed)
                {
                    _audioManager.PlayBackSound();
                    _player1Confirmed = false;
                    _player2Confirmed = false;
                    Console.WriteLine("[WizardSelect] Both wizard selections cleared");
                }
            }
            
            // Both players confirmed - proceed to loadout
            if (_player1Confirmed && _player2Confirmed)
            {
                return true; // Signal completion
            }
            
            return false;
        }
        
        /// <summary>
        /// Draws the wizard selection screen UI.
        /// </summary> <summary>
        /// 
        /// </summary>
        public void Draw()
        {
            Raylib.ClearBackground(new Color(15, 15, 35, 255));
            _particleManager.DrawSnow();
            
            // Semi-transparent black overlay (same as loadout screen)
            Raylib.DrawRectangle(0, 0, 1920, 1080, new Color(0, 0, 0, 120));
            
            // Title with background
            string title = "SELECT YOUR WIZARD";
            int titleWidth = Raylib.MeasureText(title, 80);
            int titleX = (1920 - titleWidth) / 2;
            Raylib.DrawRectangle(titleX - 30, 35, titleWidth + 60, 90, new Color(0, 0, 0, 150));
            Raylib.DrawRectangleLines(titleX - 30, 35, titleWidth + 60, 90, Color.Gold);
            Raylib.DrawText(title, titleX, 50, 80, Color.Gold);
            
            // Calculate center positions for both boxes side-by-side
            int boxSpacing = 100; // Space between the two boxes
            int totalWidth = (_wizardDisplayWidth + 40) * 2 + boxSpacing;
            int startX = (1920 - totalWidth) / 2;
            
            // Player 1 section (left box)
            DrawPlayerWizardSelection(
                _player1Profile,
                _player1SelectedIndex,
                _player1Confirmed,
                startX + 20, // X position
                "Player 1",
                "A/D to select, SPACE to confirm",
                false // Not mirrored
            );
            
            // Player 2 section (right box)
            DrawPlayerWizardSelection(
                _player2Profile,
                _player2SelectedIndex,
                _player2Confirmed,
                startX + (_wizardDisplayWidth + 40) + boxSpacing + 20, // X position
                "Player 2",
                "LEFT/RIGHT to select, ENTER to confirm",
                true // Mirrored
            );
            
            // Backspace instruction at bottom center
            string backspaceText = "Press BACKSPACE to unselect both wizards";
            int backspaceWidth = Raylib.MeasureText(backspaceText, 24);
            int backspaceX = (1920 - backspaceWidth) / 2;
            Raylib.DrawText(backspaceText, backspaceX, 1020, 24, Color.Gold);
        }
        
        /// <summary>
        /// Draws the wizard selection UI for a single player.
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="selectedIndex"></param>
        /// <param name="confirmed"></param>
        /// <param name="baseX"></param>
        /// <param name="playerLabel"></param>
        /// <param name="controls"></param>
        /// <param name="mirrored"></param>
        public void DrawPlayerWizardSelection(PlayerProfile? profile, int selectedIndex, bool confirmed, int baseX, string playerLabel, string controls, bool mirrored)
        {
            if (profile == null) return;
            
            int baseY = 170;
            
            // Draw main selection box
            Raylib.DrawRectangle(baseX - 20, baseY - 20, _wizardDisplayWidth + 40, 680, new Color(0, 0, 0, 120));
            Color borderColor = confirmed ? Color.Lime : Color.Gold;
            Raylib.DrawRectangleLines(baseX - 20, baseY - 20, _wizardDisplayWidth + 40, 680, borderColor);
            
            // Player name and level in header
            string nameText = $"{playerLabel}: {profile.Username}";
            int nameWidth = Raylib.MeasureText(nameText, 28);
            Raylib.DrawText(nameText, baseX + (_wizardDisplayWidth - nameWidth) / 2, baseY, 28, Color.White);
            
            string levelText = $"Level {profile.Level}";
            int levelWidth = Raylib.MeasureText(levelText, 24);
            Raylib.DrawText(levelText, baseX + (_wizardDisplayWidth - levelWidth) / 2, baseY + 36, 24, Color.Yellow);
            
            // Wizard display area
            int wizardY = baseY + 100;
            
            // Draw available wizards indicators - centered
            int totalIndicatorWidth = (profile.UnlockedWizards.Count - 1) * 70; // Spacing between circles
            int indicatorStartX = baseX + (_wizardDisplayWidth - totalIndicatorWidth) / 2;
            
            for (int i = 0; i < profile.UnlockedWizards.Count; i++)
            {
                int circleX = indicatorStartX + i * 70;
                
                Color indicatorColor = i == selectedIndex ? Color.Gold : new Color(80, 80, 80, 255);
                Raylib.DrawCircle(circleX, wizardY - 20, 18, indicatorColor);
                if (i == selectedIndex)
                {
                    Raylib.DrawCircleLines(circleX, wizardY - 20, 20, Color.Gold);
                }
                
                // Wizard number - centered in circle
                string wizardNum = $"{profile.UnlockedWizards[i] + 1}";
                int numWidth = Raylib.MeasureText(wizardNum, 24);
                Raylib.DrawText(wizardNum, circleX - numWidth / 2, wizardY - 32, 24, Color.White);
            }
            
            // Draw currently selected wizard sprite
            int wizardId = profile.UnlockedWizards[selectedIndex];
            Texture2D wizardSprite = _animation.GetWizardRunSprite(wizardId);
            
            if (wizardSprite.Width > 0)
            {
                // Mirror the sprite if needed (for Player 2)
                float spriteWidth = mirrored ? -_animation.WizardFrameWidth : _animation.WizardFrameWidth;
                Rectangle sourceRec = new Rectangle(_currentFrame * _animation.WizardFrameWidth, 0, spriteWidth, _animation.WizardFrameHeight);
                float scale = (_wizardDisplayHeight - 50) / (float)_animation.WizardFrameHeight;
                float wizardWidth = _animation.WizardFrameWidth * scale;
                float wizardHeight = _animation.WizardFrameHeight * scale;
                
                // Center wizard horizontally and vertically in the display area
                float wizardX = baseX + (_wizardDisplayWidth - wizardWidth) / 2;
                float wizardDisplayY = wizardY + 20;
                
                // Inner display box for wizard - centered
                int boxY = wizardY + 10;
                int boxHeight = 400;
                Raylib.DrawRectangle((int)wizardX - 10, boxY, (int)wizardWidth + 20, boxHeight, new Color(0, 0, 0, 80));
                
                // Center wizard vertically within the box
                float centeredWizardY = boxY + (boxHeight - wizardHeight) / 2;
                
                Rectangle destRec = new Rectangle(wizardX, centeredWizardY, wizardWidth, wizardHeight);
                Vector2 origin = new Vector2(0, 0);
                
                // Glow effect for selected wizard
                if (!confirmed)
                {
                    float glowAlpha = (float)(Math.Sin(Raylib.GetTime() * 3f) * 0.3f + 0.5f);
                    Color glowColor = new Color(255, 215, 0, (int)(glowAlpha * 255));
                    Raylib.DrawRectangleLinesEx(new Rectangle(wizardX - 15, centeredWizardY - 10, wizardWidth + 30, wizardHeight + 20), 4, glowColor);
                }
                
                Raylib.DrawTexturePro(wizardSprite, sourceRec, destRec, origin, 0f, Color.White);
            }
            
            // Wizard name based on ID
            string[] wizardNames = { "Velthra", "Rucandel", "Thalorian" };
            string wizardName = wizardNames[wizardId];
            int wizardNameWidth = Raylib.MeasureText(wizardName, 36);
            Raylib.DrawText(wizardName, baseX + (_wizardDisplayWidth - wizardNameWidth) / 2, wizardY + _wizardDisplayHeight + 30, 36, new Color(0, 255, 255, 255));
            
            // Status or controls
            if (confirmed)
            {
                string confirmedText = "Selected";
                int confirmedWidth = Raylib.MeasureText(confirmedText, 28);
                Raylib.DrawRectangle(baseX + (_wizardDisplayWidth - confirmedWidth) / 2 - 15, wizardY + _wizardDisplayHeight + 80, confirmedWidth + 30, 40, new Color(0, 200, 0, 100));
                Raylib.DrawText(confirmedText, baseX + (_wizardDisplayWidth - confirmedWidth) / 2, wizardY + _wizardDisplayHeight + 88, 28, Color.Lime);
            }
            else
            {
                int controlsWidth = Raylib.MeasureText(controls, 18);
                Raylib.DrawText(controls, baseX + (_wizardDisplayWidth - controlsWidth) / 2, wizardY + _wizardDisplayHeight + 90, 18, new Color(200, 200, 200, 255));
            }
            
            // Locked wizards message
            if (profile.UnlockedWizards.Count < 3)
            {
                string unlockText = "Level up to unlock more wizards!";
                int unlockWidth = Raylib.MeasureText(unlockText, 16);
                Raylib.DrawText(unlockText, baseX + (_wizardDisplayWidth - unlockWidth) / 2, wizardY + _wizardDisplayHeight + 135, 16, new Color(180, 180, 100, 255));
            }
        }
    }
}
