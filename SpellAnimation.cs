using System;
using System.Numerics;
using Raylib_cs;

namespace Grimoire
{
    /// <summary>
    /// Defines the spell animation including casting and projectile effects.
    /// </summary>
    public class SpellAnimation
    {
        private Texture2D _castingSprite; // Light_ball.png - wizard casting animation (casting sprite)
        private Texture2D _projectileSprite; // Charge.png - projectile effect (projectile sprite)
        private float _animationTime; // elapsed animation time (seconds)
        private int _currentFrame; // current frame index for casting animation
        private float _frameCounter; // accumulator for frame timing
        private float _frameSpeed; // frame duration in seconds (faster frame speed for charge animation)
        private int _castingFramesPerRow; // number of frames in casting sprite sheet row
        private int _projectileFramesPerRow; // number of frames in projectile sprite sheet row
        private int _castingFrameWidth; // width of a single casting frame (pixels)
        private int _castingFrameHeight; // height of a single casting frame (pixels)
        private int _projectileFrameWidth; // width of a single projectile frame (pixels)
        private int _projectileFrameHeight; // height of a single projectile frame (pixels)
        
        // Projectile movement
        private Vector2 _projectileStart; // projectile start position (caster)
        private Vector2 _projectileEnd; // projectile target position
        private Vector2 _projectilePos; // current projectile position
        private float _projectileProgress; // interpolation progress 0..1 along path
        private float _projectileDuration; // time in seconds for projectile to travel
        private bool _isProjectileActive; // whether projectile is currently active/moving
        private bool _hasHit; // whether projectile has reached the target

        /// <summary>
        /// Initializes a new instance of the SpellAnimation class.
        /// </summary>
        /// <param name="castingSprite"></param>
        /// <param name="projectileSprite"></param>
        /// <param name="projectileFrameCount"></param>
        public SpellAnimation(Texture2D castingSprite, Texture2D projectileSprite, int projectileFrameCount)
        {
            _castingSprite = castingSprite;
            _projectileSprite = projectileSprite;
            _projectileFramesPerRow = projectileFrameCount;
            _animationTime = 0f;
            _currentFrame = 0;
            _frameCounter = 0f;
            _frameSpeed = 0.04f;
            _castingFramesPerRow = 7;
            _castingFrameWidth = 128;
            _castingFrameHeight = 128;
            _projectileFrameWidth = 64;
            _projectileFrameHeight = 64;
            _projectileProgress = 0f;
            _projectileDuration = 0.7f;
            _isProjectileActive = false;
            _hasHit = false;
        }

        public bool IsAnimating
        {
            get { return _animationTime < 2.0f; } // Total animation duration (increased to prevent wizard disappearing)
        }

        public bool HasHit
        {
            get { return _hasHit; }
        }

        /// <summary>
        /// Starts the spell animation from caster to target positions.
        /// </summary>
        /// <param name="casterPos"></param>
        /// <param name="targetPos"></param>
        public void StartAnimation(Vector2 casterPos, Vector2 targetPos)
        {
            _animationTime = 0f;
            _currentFrame = 0;
            _frameCounter = 0f;
            _projectileProgress = 0f;
            _isProjectileActive = false;
            _hasHit = false;
            
            // Use the provided positions directly (already adjusted in UIManager)
            _projectileStart = casterPos;
            _projectileEnd = targetPos;
            _projectilePos = casterPos;
        }

        /// <summary>
        /// Updates the spell animation state.
        /// </summary>
        /// <param name="deltaTime"></param> <summary>
        /// 
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(float deltaTime)
        {
            if (!IsAnimating) return;

            _animationTime += deltaTime;
            _frameCounter += deltaTime;

            // Wizard casting animation (first 0.7 seconds) - update frame
            if (_frameCounter >= _frameSpeed)
            {
                _frameCounter = 0f;
                _currentFrame = (_currentFrame + 1) % _castingFramesPerRow;
            }

            // Start projectile movement around 0.3 seconds (after wizard starts casting)
            if (_animationTime >= 0.3f && !_isProjectileActive && !_hasHit)
            {
                _isProjectileActive = true;
                _projectileProgress = 0f;
            }

            // Update projectile position - ensure it reaches target
            if (_isProjectileActive && !_hasHit)
            {
                _projectileProgress += deltaTime / _projectileDuration;
                if (_projectileProgress >= 1.0f)
                {
                    _projectileProgress = 1.0f;
                    _hasHit = true;
                }

                // Linear interpolation between start and end positions
                _projectilePos = Vector2.Lerp(_projectileStart, _projectileEnd, _projectileProgress);
            }
            
            // Keep projectile at target position after hit
            if (_hasHit)
            {
                _projectilePos = _projectileEnd;
            }
        }

        /// <summary>
        /// Draws the casting animation at the specified destination rectangle.
        /// </summary>
        /// <param name="destRec"></param>
        /// <param name="origin"></param>
        /// <param name="tintColor"></param>
        /// <param name="flipHorizontal"></param> <summary>
        /// 
        /// </summary>
        /// <param name="destRec"></param>
        /// <param name="origin"></param>
        /// <param name="tintColor"></param>
        /// <param name="flipHorizontal"></param>
        public void DrawCastingAnimation(Rectangle destRec, Vector2 origin, Color tintColor, bool flipHorizontal)
        {
            if (_castingSprite.Width > 0) // Always show casting sprite during animation
            {
                // Clamp frame to last frame if animation time exceeds casting frames
                int frameToShow = _animationTime < 0.3f ? _currentFrame : (_castingFramesPerRow - 1);
                
                Rectangle sourceRec = new Rectangle(frameToShow * _castingFrameWidth, 0, flipHorizontal ? -_castingFrameWidth : _castingFrameWidth,_castingFrameHeight);
                Raylib.DrawTexturePro(_castingSprite, sourceRec, destRec, origin, 0.0f, tintColor);
            }
        }

        /// <summary>
        /// Draws the projectile animation at its current position.
        /// </summary>
        /// <param name="scale"></param>
        public void DrawProjectile(float scale)
        {
            if (_isProjectileActive && !_hasHit && _projectileSprite.Width > 0)
            {
                float projectileScale = scale * 0.5f; // Adjust scale for 64x64 projectile
                float scaledWidth = _projectileFrameWidth * projectileScale;
                float scaledHeight = _projectileFrameHeight * projectileScale;
                
                Rectangle destRec = new Rectangle(_projectilePos.X - scaledWidth / 2, _projectilePos.Y - scaledHeight / 2, scaledWidth, scaledHeight);
                    
                // Draw projectile animation - cycle through all 10 frames
                int frameIndex = (int)((_animationTime - 0.3f) / _frameSpeed) % _projectileFramesPerRow;
                Rectangle sourceRec = new Rectangle(frameIndex * _projectileFrameWidth, 0, _projectileFrameWidth, _projectileFrameHeight );
               

                Raylib.DrawTexturePro(_projectileSprite, sourceRec, destRec, Vector2.Zero, 0.0f, Color.White);
            }
        }
    }
}
