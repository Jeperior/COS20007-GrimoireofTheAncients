using System;
using System.Numerics;
using Raylib_cs;

namespace Grimoire
{
    /// <summary>
    /// Handles the main menu screen UI and interactions.
    /// </summary>
    public class MainMenuScreen
    {
        private float _titleAnimationTime;          // Timer for title animation
        private ParticleManager _particleManager;   // Particle system
        private AudioManager _audioManager;         // Audio system

        /// <summary>
        /// Initializes a new instance of the MainMenuScreen class.
        /// </summary>
        /// <param name="particleManager">Reference to the particle system used for background effects.</param>
        /// <param name="audioManager">Reference to the audio manager for playing UI sounds.</param>
        public MainMenuScreen(ParticleManager particleManager, AudioManager audioManager)
        {
            // Store references to shared systems and reset animation timer.
            _particleManager = particleManager;
            _audioManager = audioManager;
            _titleAnimationTime = 0f;
        }

        /// <summary>
        /// Update the main menu logic (input handling and state transitions).
        /// </summary>
        /// <param name="gameManager">Game manager used to change game states.</param>
        public void Update(GameManager gameManager)
        {
            // Get current mouse position for hover / click detection.
            Vector2 mousePos = Raylib.GetMousePosition();

            // Define button hitboxes (centered horizontally).
            Rectangle startButton = new Rectangle(1920 / 2 - 200, 795, 400, 55);
            Rectangle changeProfileButton = new Rectangle(1920 / 2 - 200, 860, 400, 55);
            Rectangle exitButton = new Rectangle(1920 / 2 - 200, 925, 400, 55);

            // If hovering/clicking START -> go to wizard select.
            if (Raylib.CheckCollisionPointRec(mousePos, startButton))
            {
            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                _audioManager.PlaySelectSound();
                gameManager.ChangeState(GameState.WizardSelect);
            }
            }
            // If hovering/clicking CHANGE PROFILE -> go to profile select.
            else if (Raylib.CheckCollisionPointRec(mousePos, changeProfileButton))
            {
            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                _audioManager.PlaySelectSound();
                gameManager.ChangeState(GameState.ProfileSelect);
            }
            }
            // If hovering/clicking QUIT -> close window.
            else if (Raylib.CheckCollisionPointRec(mousePos, exitButton))
            {
            if (Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                _audioManager.PlaySelectSound();
                Raylib.CloseWindow();
            }
            }
        }

        /// <summary>
        /// Draw the main menu UI, including animated title, buttons, and player profiles.
        /// </summary>
        /// <param name="player1Profile">Profile for player 1 (can be null).</param>
        /// <param name="player2Profile">Profile for player 2 (can be null).</param>
        public void Draw(PlayerProfile? player1Profile, PlayerProfile? player2Profile)
        {
            // Ensure particles are initialized once; use a large number for background snow.
            if (_particleManager.ParticleCount == 0)
            {
            _particleManager.InitializeParticles(1920, 1080, 3000);
            }

            // Update and render snow particle system.
            _particleManager.UpdateSnow(Raylib.GetFrameTime(), 1920, 1080);
            _particleManager.DrawSnow();

            // Advance title float animation timer (controls bobbing and small button pulse).
            _titleAnimationTime += 0.05f;

            // Compute three phase offsets for the multi-line title bobbing effect.
            float floatOffset1 = (float)Math.Sin(_titleAnimationTime) * 10f;
            float floatOffset2 = (float)Math.Sin(_titleAnimationTime + 0.5f) * 10f;
            float floatOffset3 = (float)Math.Sin(_titleAnimationTime + 1.0f) * 10f;

            // Draw the title with a shadow (offset by +5 pixels).
            int grimoireWidth = Raylib.MeasureText("GRIMOIRE", 100);
            Raylib.DrawText("GRIMOIRE", (1920 - grimoireWidth) / 2 + 5, (int)(50 + floatOffset1) + 5, 100, Color.Black);
            Raylib.DrawText("GRIMOIRE", (1920 - grimoireWidth) / 2, (int)(50 + floatOffset1), 100, Color.White);

            int ofWidth = Raylib.MeasureText("OF", 100);
            Raylib.DrawText("OF", (1920 - ofWidth) / 2 + 5, (int)(130 + floatOffset2) + 5, 100, Color.Black);
            Raylib.DrawText("OF", (1920 - ofWidth) / 2, (int)(130 + floatOffset2), 100, Color.White);

            int ancientsWidth = Raylib.MeasureText("THE ANCIENTS", 100);
            Raylib.DrawText("THE ANCIENTS", (1920 - ancientsWidth) / 2 + 5, (int)(210 + floatOffset3) + 5, 100, Color.Black);
            Raylib.DrawText("THE ANCIENTS", (1920 - ancientsWidth) / 2, (int)(210 + floatOffset3), 100, Color.White);

            // Recompute mouse and button rectangles for hover logic while drawing.
            Vector2 mousePos = Raylib.GetMousePosition();
            Rectangle startButton = new Rectangle(1920 / 2 - 200, 795, 400, 55);
            Rectangle changeProfileButton = new Rectangle(1920 / 2 - 200, 860, 400, 55);
            Rectangle exitButton = new Rectangle(1920 / 2 - 200, 925, 400, 55);

            // START button — change appearance when hovered.
            Color startColor = Color.White;
            float startScale = 1.0f;
            if (Raylib.CheckCollisionPointRec(mousePos, startButton))
            {
            // Slight translucent highlight behind button and subtle pulse scale.
            startColor = Color.LightGray;
            startScale = 1.0f + (float)Math.Sin(_titleAnimationTime * 3) * 0.0325f;
            Raylib.DrawRectangleRec(startButton, new Color(211, 211, 211, 75));
            }
            int scaledFontSize = (int)(50 * startScale);
            int scaledStartWidth = Raylib.MeasureText("START", scaledFontSize);
            // Draw drop shadow and text centered for START.
            Raylib.DrawText("START", (1920 - scaledStartWidth) / 2 + 4, 800 + 4, scaledFontSize, Color.Black);
            Raylib.DrawText("START", (1920 - scaledStartWidth) / 2, 800, scaledFontSize, startColor);

            // CHANGE PROFILE button — same hover/pulse behavior as START.
            Color changeProfileColor = Color.White;
            float changeProfileScale = 1.0f;
            if (Raylib.CheckCollisionPointRec(mousePos, changeProfileButton))
            {
            changeProfileColor = Color.LightGray;
            changeProfileScale = 1.0f + (float)Math.Sin(_titleAnimationTime * 3) * 0.0325f;
            Raylib.DrawRectangleRec(changeProfileButton, new Color(211, 211, 211, 75));
            }
            int scaledChangeProfileFontSize = (int)(50 * changeProfileScale);
            int scaledChangeProfileWidth = Raylib.MeasureText("CHANGE PROFILE", scaledChangeProfileFontSize);
            Raylib.DrawText("CHANGE PROFILE", (1920 - scaledChangeProfileWidth) / 2 + 4, 868 + 4, scaledChangeProfileFontSize, Color.Black);
            Raylib.DrawText("CHANGE PROFILE", (1920 - scaledChangeProfileWidth) / 2, 868, scaledChangeProfileFontSize, changeProfileColor);

            // QUIT button — same hover/pulse behavior as other buttons.
            Color exitColor = Color.White;
            float exitScale = 1.0f;
            if (Raylib.CheckCollisionPointRec(mousePos, exitButton))
            {
            exitColor = Color.LightGray;
            exitScale = 1.0f + (float)Math.Sin(_titleAnimationTime * 3) * 0.0325f;
            Raylib.DrawRectangleRec(exitButton, new Color(211, 211, 211, 75));
            }
            int scaledQuitFontSize = (int)(50 * exitScale);
            int scaledQuitWidth = Raylib.MeasureText("QUIT", scaledQuitFontSize);
            Raylib.DrawText("QUIT", (1920 - scaledQuitWidth) / 2 + 4, 930 + 4, scaledQuitFontSize, Color.Black);
            Raylib.DrawText("QUIT", (1920 - scaledQuitWidth) / 2, 930, scaledQuitFontSize, exitColor);

            // Draw Player 1 profile block on the left if available.
            if (player1Profile != null)
            {
            // Background box with translucent fill and outline color.
            Raylib.DrawRectangle(50, 50, 330, 160, new Color(0, 0, 0, 200));
            Raylib.DrawRectangleLines(50, 50, 330, 160, Color.SkyBlue);
            Raylib.DrawText("PLAYER 1", 70, 60, 22, Color.SkyBlue);

            // Display username, level, XP, and win stats.
            Raylib.DrawText(player1Profile.Username, 70, 90, 35, Color.White);
            Raylib.DrawText($"Level {player1Profile.Level}", 70, 135, 20, Color.LightGray);
            Raylib.DrawText($"XP: {player1Profile.XP}/{player1Profile.GetXPForNextLevel()}", 70, 160, 20, Color.LightGray);
            Raylib.DrawText($"Wins: {player1Profile.GamesWon}/{player1Profile.GamesPlayed} ({player1Profile.GetWinRate():F1}%)", 70, 185, 18, Color.LightGray);
            }

            // Draw Player 2 profile block on the right if available.
            if (player2Profile != null)
            {
            Raylib.DrawRectangle(1540, 50, 330, 160, new Color(0, 0, 0, 200));
            Raylib.DrawRectangleLines(1540, 50, 330, 160, Color.Orange);
            Raylib.DrawText("PLAYER 2", 1560, 60, 22, Color.Orange);

            Raylib.DrawText(player2Profile.Username, 1560, 90, 35, Color.White);
            Raylib.DrawText($"Level {player2Profile.Level}", 1560, 135, 20, Color.LightGray);
            Raylib.DrawText($"XP: {player2Profile.XP}/{player2Profile.GetXPForNextLevel()}", 1560, 160, 20, Color.LightGray);
            Raylib.DrawText($"Wins: {player2Profile.GamesWon}/{player2Profile.GamesPlayed} ({player2Profile.GetWinRate():F1}%)", 1560, 185, 18, Color.LightGray);
            }
        }
        }
    }
