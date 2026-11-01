using System;
using System.Collections.Generic;
using Raylib_cs;

namespace Grimoire
{
    /// <summary>
    /// Manages particle systems like snowflakes.
    /// Handles creation, initialization, updating, and rendering of particles.
    /// </summary>
    public class ParticleManager
    {
        private List<Snowflake> _particles;                 // List of snowflake particles
        private List<HealingParticle> _healingParticles;    // List of healing particles
        private Random _random;                             // Random number generator
        private float _animationTime;                       // Animation time for particle effects

        // Configuration for snowflake generation
        private float _minSpeed;    // Minimum speed of snowflakes
        private float _maxSpeed;    // Maximum speed of snowflakes
        private float _minSize;     // Minimum size of snowflakes
        private float _maxSize;     // Maximum size of snowflakes

        /// <summary>
        /// Initializes a new instance of the ParticleManager class.
        /// </summary>
        public ParticleManager()
        {
            _particles = new List<Snowflake>();
            _healingParticles = new List<HealingParticle>();
            _random = new Random();
            _animationTime = 0f;
            
            // Configuration for snowflake generation
            _minSpeed = 1.0f;
            _maxSpeed = 3.0f;
            _minSize = 1.0f;
            _maxSize = 3.0f;
        }

        /// <summary>
        /// Gets the particle count.
        /// </summary>
        public int ParticleCount
        {
            get { return _particles.Count; }
        }

        /// <summary>
        /// Initializes particles with specified count.
        /// </summary>
        public void InitializeParticles(int screenWidth, int screenHeight, int particleCount)
        {
            _particles.Clear();
            for (int i = 0; i < particleCount; i++)
            {
                float speed = (float)(_random.NextDouble() * (_maxSpeed - _minSpeed) + _minSpeed);
                float size = (float)(_random.NextDouble() * (_maxSize - _minSize) + _minSize);
                float sway = (float)(_random.NextDouble() * 3 + 2);
                float swayOffset = (float)(_random.NextDouble() * Math.PI * 2);

                _particles.Add(new Snowflake(
                    _random.Next(0, screenWidth),
                    _random.Next(-screenHeight, 0),
                    speed,
                    size,
                    swayOffset
                ));
            }
        }

        /// <summary>
        /// Updates all particles and handles their lifecycle.
        /// </summary>
        public void Update(float deltaTime, int screenWidth, int screenHeight)
        {
            _animationTime += deltaTime;

            foreach (Snowflake particle in _particles)
            {
                particle.Update(_animationTime);

                // Reset particle if it goes off screen
                if (particle.IsOffScreen(screenHeight))
                {
                    particle.Reset(screenWidth, _random);
                }
            }
            
            // Update healing particles
            for (int i = _healingParticles.Count - 1; i >= 0; i--)
            {
                _healingParticles[i].Update(deltaTime);
                if (_healingParticles[i].IsExpired())
                {
                    _healingParticles.RemoveAt(i);
                }
            }
        }
        
        /// <summary>
        /// Updates only snow particles (for menu/loadout screens).
        /// </summary>
        public void UpdateSnow(float deltaTime, int screenWidth, int screenHeight)
        {
            _animationTime += deltaTime;

            foreach (Snowflake particle in _particles)
            {
                particle.Update(_animationTime);

                // Reset particle if it goes off screen
                if (particle.IsOffScreen(screenHeight))
                {
                    particle.Reset(screenWidth, _random);
                }
            }
        }
        
        /// <summary>
        /// Updates only healing particles (for battle screen).
        /// </summary>
        public void UpdateHealing(float deltaTime)
        {
            for (int i = _healingParticles.Count - 1; i >= 0; i--)
            {
                _healingParticles[i].Update(deltaTime);
                if (_healingParticles[i].IsExpired())
                {
                    _healingParticles.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Renders only snow particles (for menu/loadout/end screens).
        /// </summary>
        public void DrawSnow()
        {
            foreach (Snowflake particle in _particles)
            {
                particle.Draw();
            }
        }
        
        /// <summary>
        /// Renders only healing particles (for battle screen).
        /// </summary>
        public void DrawHealing()
        {
            foreach (HealingParticle healingParticle in _healingParticles)
            {
                healingParticle.Draw();
            }
        }

        /// <summary>
        /// Spawns healing particles around a specific position with a specific color.
        /// </summary>
        public void SpawnHealingParticles(float x, float y, Color color, int count)
        {
            for (int i = 0; i < count; i++)
            {
                // Spawn particles in a circular area around the position (wizard body)
                float angle = (float)(_random.NextDouble() * Math.PI * 2);
                float radius = (float)(_random.NextDouble() * 60 + 40); // 40 to 100 pixels from center (tighter around wizard)
                float particleX = x + (float)Math.Cos(angle) * radius;
                float particleY = y + (float)Math.Sin(angle) * radius;
                
                _healingParticles.Add(new HealingParticle(particleX, particleY, color));
            }
        }
    }
}
