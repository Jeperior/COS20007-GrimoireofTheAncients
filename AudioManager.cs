using System;
using Raylib_cs;

namespace Grimoire
{
    /// <summary>
    /// Manages all audio resources including music and sound effects.
    /// Centralizes audio loading/unloading and playback management.
    /// Follows the Resource Manager pattern for proper resource lifecycle management.
    /// </summary>
    public class AudioManager
    {
        // Music
        private Music _menuMusic;
        private Music _battleMusic;
        
        // Sound Effects
        private Sound _keySound;
        private Sound _selectSound;
        private Sound _backSound;
        private Sound _castSound;
        private Sound _fireballSound;
        private Sound _lightningSound;
        private Sound _poisonSound;
        private Sound _cureSound;
        private Sound _healSound;

        /// <summary>
        /// Initializes and loads all audio assets.
        /// Should be called once during program startup.
        /// </summary>
        public AudioManager()
        {
            LoadAllAudio();
        }

        /// <summary>
        /// Loads all music and sound effect files.
        /// </summary>
        public void LoadAllAudio()
        {
            try
            {
                // Load music streams
                _menuMusic = Raylib.LoadMusicStream("Asset\\MenuMusic.wav");
                _battleMusic = Raylib.LoadMusicStream("Asset\\BattleMusic.wav");
                
                // Load sound effects
                _keySound = Raylib.LoadSound("Asset\\Key.wav");
                _selectSound = Raylib.LoadSound("Asset\\Select.wav");
                _backSound = Raylib.LoadSound("Asset\\Back.wav");
                _castSound = Raylib.LoadSound("Asset\\Cast.wav");
                _fireballSound = Raylib.LoadSound("Asset\\Fireball.wav");
                _lightningSound = Raylib.LoadSound("Asset\\Lightning.wav");
                _poisonSound = Raylib.LoadSound("Asset\\Poison.wav");
                _cureSound = Raylib.LoadSound("Asset\\Cure.wav");
                _healSound = Raylib.LoadSound("Asset\\Heal.wav");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AudioManager] Error loading audio assets: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Updates all music streams. Should be called every frame.
        /// </summary>
        public void UpdateMusicStream()
        {
            Raylib.UpdateMusicStream(_menuMusic);
            Raylib.UpdateMusicStream(_battleMusic);
        }

        // ===== Music Playback Methods =====

        /// <summary>
        /// Plays the menu music.
        /// </summary>
        public void PlayMenuMusic()
        {
            Raylib.PlayMusicStream(_menuMusic);
        }

        /// <summary>
        /// Stops the menu music.
        /// </summary>
        public void StopMenuMusic()
        {
            Raylib.StopMusicStream(_menuMusic);
        }

        /// <summary>
        /// Plays the battle music.
        /// </summary>
        public void PlayBattleMusic()
        {
            Raylib.PlayMusicStream(_battleMusic);
        }

        /// <summary>
        /// Stops the battle music.
        /// </summary>
        public void StopBattleMusic()
        {
            Raylib.StopMusicStream(_battleMusic);
        }

        // ===== Sound Effect Playback Methods =====

        /// <summary>
        /// Plays the key navigation sound effect.
        /// </summary>
        public void PlayKeySound()
        {
            Raylib.PlaySound(_keySound);
        }

        /// <summary>
        /// Plays the selection/confirmation sound effect.
        /// </summary>
        public void PlaySelectSound()
        {
            Raylib.PlaySound(_selectSound);
        }

        /// <summary>
        /// Plays the back/cancel sound effect.
        /// </summary>
        public void PlayBackSound()
        {
            Raylib.PlaySound(_backSound);
        }

        /// <summary>
        /// Plays the spell casting sound effect.
        /// </summary>
        public void PlayCastSound()
        {
            Raylib.PlaySound(_castSound);
        }

        /// <summary>
        /// Plays the fireball hit sound effect.
        /// </summary>
        public void PlayFireballSound()
        {
            Raylib.PlaySound(_fireballSound);
        }

        /// <summary>
        /// Plays the lightning hit sound effect.
        /// </summary>
        public void PlayLightningSound()
        {
            Raylib.PlaySound(_lightningSound);
        }

        /// <summary>
        /// Plays the poison hit sound effect.
        /// </summary>
        public void PlayPoisonSound()
        {
            Raylib.PlaySound(_poisonSound);
        }

        /// <summary>
        /// Plays the cure sound effect.
        /// </summary>
        public void PlayCureSound()
        {
            Raylib.PlaySound(_cureSound);
        }

        /// <summary>
        /// Plays the heal/regen sound effect.
        /// </summary>
        public void PlayHealSound()
        {
            Raylib.PlaySound(_healSound);
        }

        /// <summary>
        /// Unloads all audio assets. Should be called at program shutdown.
        /// </summary>
        public void UnloadAudio()
        {
            // Unload music streams
            if (_menuMusic.FrameCount > 0)
                Raylib.UnloadMusicStream(_menuMusic);
            if (_battleMusic.FrameCount > 0)
                Raylib.UnloadMusicStream(_battleMusic);
            
            // Unload sound effects
            Raylib.UnloadSound(_keySound);
            Raylib.UnloadSound(_selectSound);
            Raylib.UnloadSound(_backSound);
            Raylib.UnloadSound(_castSound);
            Raylib.UnloadSound(_fireballSound);
            Raylib.UnloadSound(_lightningSound);
            Raylib.UnloadSound(_poisonSound);
            Raylib.UnloadSound(_cureSound);
            Raylib.UnloadSound(_healSound);
        }
    }
}
