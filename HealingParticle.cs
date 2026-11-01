using System;
using System.Numerics;
using Raylib_cs;

namespace Grimoire
{
    /// <summary>
    /// Represents a healing particle effect that floats upward around a wizard.
    /// </summary>
    public class HealingParticle
    {
        
        private float _lifetime;            // Current lifetime of the particle
        private float _maxLifetime;         // Maximum lifetime of the particle
        private float _size;                // Size of the particle
        private float _alpha;               // Transparency of the particle
        private Color _color;               // Color of the particle
        private Vector2 _position;          // Current position of the particle
        private Vector2 _velocity;          // Velocity of the particle


        /// <summary>
        /// Initializes a new instance of the HealingParticle class.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public HealingParticle(float x, float y, Color color)
        {
            _position = new Vector2(x, y);
            Random random = new Random();
            
            // Random velocity - upward with slight horizontal drift
            _velocity = new Vector2(
                (float)(random.NextDouble() * 40 - 20),  // -20 to 20 horizontal
                (float)(random.NextDouble() * -80 - 40)  // -120 to -40 upward
            );
            
            _maxLifetime = (float)(random.NextDouble() * 1.0f + 0.5f); // 0.5 to 1.5 seconds
            _lifetime = 0f;
            _color = color;
            _size = (float)(random.NextDouble() * 3 + 2); // 2 to 5 pixels
            _alpha = 255;
        }

        /// <summary>
        /// Updates the particle's position and lifetime.
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(float deltaTime)
        {
            _lifetime += deltaTime;
            _position += _velocity * deltaTime;
            
            // Fade out as lifetime progresses
            float lifeRatio = _lifetime / _maxLifetime;
            _alpha = (byte)((1.0f - lifeRatio) * 255);
        }

        /// <summary>
        /// Draws the particle on the screen.
        /// </summary>
        public void Draw()
        {
            Color drawColor = new Color(_color.R, _color.G, _color.B, _alpha);
            Raylib.DrawCircleV(_position, _size, drawColor);
        }

        /// <summary>
        /// Determines if the particle's lifetime has expired.
        /// </summary>
        /// <returns></returns>
        public bool IsExpired()
        {
            return _lifetime >= _maxLifetime;
        }
    }
}
