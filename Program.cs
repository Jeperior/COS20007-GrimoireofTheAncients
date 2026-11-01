using Raylib_cs;
using System;
using System.Numerics;


namespace Grimoire
{
    class Program
    {
        static void Main()
        {
            Raylib.InitWindow(1920, 1080, "Grimoire of the Ancients");
            Raylib.SetTargetFPS(60);
            Raylib.InitAudioDevice();

            // Initialize game, animation, and audio
            GameManager gameManager = GameManager.Initialize();
            Animation animation = new Animation();
            AudioManager audioManager = new AudioManager();
            audioManager.PlayMenuMusic();
            
            GameState previousState = gameManager.State;

            while (!Raylib.WindowShouldClose())
            {
                audioManager.UpdateMusicStream();
                
                // Handle music transitions when state changes
                if (gameManager.State != previousState)
                {
                    if (gameManager.State == GameState.Battle)
                    {
                        audioManager.StopMenuMusic();
                        audioManager.PlayBattleMusic();
                    }
                    else if (previousState == GameState.Battle)
                    {
                        audioManager.StopBattleMusic();
                        audioManager.PlayMenuMusic();
                    }
                    previousState = gameManager.State;
                }
                
                gameManager.Update();

                Raylib.BeginDrawing();
                
                // Draw appropriate background based on game state
                Texture2D currentBackground = animation.GetBackgroundForState(gameManager.State);
                Raylib.DrawTextureEx(currentBackground, new Vector2(0, 0), 0.0f, 
                    Math.Min(1920f / currentBackground.Width, 1080f / currentBackground.Height), Color.White);
                
                gameManager.Draw();   // Will call DrawMainMenu(), DrawLoadoutScreen(), etc.
                Raylib.EndDrawing();
            }

            animation.UnloadAllAssets();
            audioManager.UnloadAudio();
            Raylib.CloseAudioDevice();
            Raylib.CloseWindow();
        }
    }
}