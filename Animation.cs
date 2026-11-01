using System;
using System.Collections.Generic;
using Raylib_cs;

namespace Grimoire
{
    /// <summary>
    /// Manages all sprite animations, textures, and audio resources.
    /// Centralizes asset loading/unloading and animation state management.
    /// Follows the Resource Manager pattern for proper resource lifecycle management.
    /// </summary>
    public class Animation
    {
        // Textures
        private Texture2D _menuBackground;
        private Texture2D _battleBackground;

        // Sprite animation settings
        private int _wizardFrameWidth;
        private int _wizardFrameHeight;
        private int _wizardFramesPerRow;
        private int _wizardWinFrames;
        private float _wizardFrameSpeed;
        
        // Wizard sprite sets (3 wizards: 0=Wiz1, 1=Wiz2, 2=Wiz3)
        private Dictionary<int, Texture2D> _wizardIdleSprites;
        private Dictionary<int, Texture2D> _wizardRunSprites;
        private Dictionary<int, Texture2D> _wizardHurtSprites;
        private Dictionary<int, Texture2D> _wizardDeadSprites;
        private Dictionary<int, Texture2D> _wizardWinSprites;
        private Dictionary<int, Texture2D> _castingSprites;
        private Dictionary<int, Texture2D> _projectileSprites;
        
        /// <summary>
        /// Initializes and loads all animation assets.
        /// Should be called once during program startup.
        /// </summary>
        public Animation()
        {
            // Initialize animation settings
            _wizardFrameWidth = 128;
            _wizardFrameHeight = 128;
            _wizardFramesPerRow = 7;
            _wizardWinFrames = 10;
            _wizardFrameSpeed = 0.1f;
            
            // Initialize sprite dictionaries
            _wizardIdleSprites = new Dictionary<int, Texture2D>();
            _wizardRunSprites = new Dictionary<int, Texture2D>();
            _wizardHurtSprites = new Dictionary<int, Texture2D>();
            _wizardDeadSprites = new Dictionary<int, Texture2D>();
            _wizardWinSprites = new Dictionary<int, Texture2D>();
            _castingSprites = new Dictionary<int, Texture2D>();
            _projectileSprites = new Dictionary<int, Texture2D>();
            LoadAllAssets();
        }
        
        // Animation Properties 
        public int WizardFrameWidth
        {
            get { return _wizardFrameWidth; }
        }

        public int WizardFrameHeight
        {
            get { return _wizardFrameHeight; }
        }

        public int WizardFramesPerRow
        {
            get { return _wizardFramesPerRow; }
        }

        public int WizardWinFrames
        {
            get { return _wizardWinFrames; }
        }

        public float WizardFrameSpeed
        {
            get { return _wizardFrameSpeed; }
        }

        /// <summary>
        /// Loads all textures and audio files.
        /// </summary>
        public void LoadAllAssets()
        {
            try
            {
                _menuBackground = Raylib.LoadTexture("Asset\\Menu.png");
                _battleBackground = Raylib.LoadTexture("Asset\\Battle.png");
                
                // Load all 3 wizard sprite sets
                string[] wizardFolders = { "Wiz1", "Wiz2", "Wiz3" };
                for (int i = 0; i < wizardFolders.Length; i++)
                {
                    string folder = wizardFolders[i];
                    _wizardIdleSprites[i] = Raylib.LoadTexture($"Asset\\{folder}\\Idle.png");
                    _wizardRunSprites[i] = Raylib.LoadTexture($"Asset\\{folder}\\Run.png");
                    _wizardHurtSprites[i] = Raylib.LoadTexture($"Asset\\{folder}\\Hurt.png");
                    _wizardDeadSprites[i] = Raylib.LoadTexture($"Asset\\{folder}\\Dead.png");
                    _wizardWinSprites[i] = Raylib.LoadTexture($"Asset\\{folder}\\Win.png");
                    _castingSprites[i] = Raylib.LoadTexture($"Asset\\{folder}\\Light_ball.png");
                    _projectileSprites[i] = Raylib.LoadTexture($"Asset\\{folder}\\Charge.png");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AnimationManager] Error loading assets: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Creates a new spell animation with casting and projectile sprites.
        /// </summary>
        public SpellAnimation CreateSpellAnimation(int wizardId = 0)
        {
            int chargeFrames = GetChargeFrameCount(wizardId);
            return new SpellAnimation(_castingSprites[wizardId], _projectileSprites[wizardId], chargeFrames);
        }

        /// <summary>
        /// Gets the appropriate wizard sprite based on character condition and wizard ID.
        /// Returns Dead sprite if Dead, Hurt sprite if Hit, otherwise Idle sprite.
        /// </summary>
        public Texture2D GetWizardSpriteByCondition(CharCondition condition, int wizardId = 0)
        {
            if (condition == CharCondition.Dead)
                return _wizardDeadSprites[wizardId];
            else if (condition == CharCondition.Hit)
                return _wizardHurtSprites[wizardId];
            else
                return _wizardIdleSprites[wizardId];
        }

        /// <summary>
        /// Gets the Win sprite for the victory screen.
        /// </summary>
        public Texture2D GetWizardWinSprite(int wizardId = 0)
        {
            return _wizardWinSprites[wizardId];
        }

        /// <summary>
        /// Gets the Run sprite for the wizard selection screen.
        /// </summary>
        public Texture2D GetWizardRunSprite(int wizardId = 0)
        {
            return _wizardRunSprites[wizardId];
        }

        /// <summary>
        /// Gets the death animation frame count for a specific wizard.
        /// Wiz1 (id=0): 5 frames, Wiz2 (id=1): 6 frames, Wiz3 (id=2): 4 frames
        /// </summary>
        public int GetDeathFrameCount(int wizardId)
        {
            switch (wizardId)
            {
                case 0: return 5; // Wiz1 - 5 frames total
                case 1: return 6; // Wiz2 - 6 frames total
                case 2: return 4; // Wiz3 - 4 frames (default)
                default: return 4;
            }
        }

        /// <summary>
        /// Gets the charge animation frame count for a specific wizard.
        /// Wiz1 (id=0): 10 frames (default), Wiz2 (id=1): 12 frames, Wiz3 (id=2): 9 frames
        /// </summary>
        public int GetChargeFrameCount(int wizardId)
        {
            switch (wizardId)
            {
                case 0: return 10; // Wiz1 - 10 frames (default)
                case 1: return 12; // Wiz2 - 12 frames
                case 2: return 9;  // Wiz3 - 9 frames
                default: return 10;
            }
        }

        /// <summary>
        /// Gets the appropriate background texture based on game state.
        /// </summary>
        public Texture2D GetBackgroundForState(GameState state)
        {
            return state == GameState.Battle || state == GameState.CoinFlip ? _battleBackground : _menuBackground;
        }
        
        /// <summary>
        /// Unloads all assets. Should be called at program shutdown.
        /// </summary>
        public void UnloadAllAssets()
        {
            if (_menuBackground.Width > 0)
                Raylib.UnloadTexture(_menuBackground);
            if (_battleBackground.Width > 0)
                Raylib.UnloadTexture(_battleBackground);
                
            // Unload all wizard sprite sets
            foreach (Texture2D sprite in _wizardIdleSprites.Values)
                if (sprite.Width > 0) Raylib.UnloadTexture(sprite);
            foreach (Texture2D sprite in _wizardRunSprites.Values)
                if (sprite.Width > 0) Raylib.UnloadTexture(sprite);
            foreach (Texture2D sprite in _wizardHurtSprites.Values)
                if (sprite.Width > 0) Raylib.UnloadTexture(sprite);
            foreach (Texture2D sprite in _wizardDeadSprites.Values)
                if (sprite.Width > 0) Raylib.UnloadTexture(sprite);
            foreach (Texture2D sprite in _wizardWinSprites.Values)
                if (sprite.Width > 0) Raylib.UnloadTexture(sprite);
            foreach (Texture2D sprite in _castingSprites.Values)
                if (sprite.Width > 0) Raylib.UnloadTexture(sprite);
            foreach (Texture2D sprite in _projectileSprites.Values)
                if (sprite.Width > 0) Raylib.UnloadTexture(sprite);
                
        }
    }
}
