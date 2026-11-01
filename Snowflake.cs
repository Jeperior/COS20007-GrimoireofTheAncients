using System;
using Raylib_cs;

namespace Grimoire
{
    /// <summary>
    /// Defines a snowflake for the snowfall effect.
    /// </summary>
    public class Snowflake
    {
        private float _x; // horizontal position
        private float _y; // vertical position
        private float _speed; // fall speed
        private float _size; // radius for drawing
        private float _swayOffset; // phase offset for horizontal sway

        /// <summary>
        /// Initializes a new instance of the Snowflake class.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="speed"></param>
        /// <param name="size"></param>
        /// <param name="swayOffset"></param>
        public Snowflake(float x, float y, float speed, float size, float swayOffset)
        {
            _x = x; // init x
            _y = y; // init y
            _speed = speed; // init speed
            _size = size; // init size
            _swayOffset = swayOffset; // init sway offset
        }

        public float X
        {
            get { return _x; } // horizontal position accessor
            set { _x = value; } // horizontal position setter
        }

        public float Y
        {
            get { return _y; } // vertical position accessor
            set { _y = value; } // vertical position setter
        }

        public float Speed
        {
            get { return _speed; } // fall speed accessor
            set { _speed = value; } // fall speed setter
        }

        public float Size
        {
            get { return _size; } // flake size accessor
            set { _size = value; } // flake size setter
        }

        public float SwayOffset
        {
            get { return _swayOffset; } // sway phase accessor
            set { _swayOffset = value; } // sway phase setter
        }

        /// <summary>
        /// Updates the snowflake's position based on speed and sway.
        /// </summary>
        /// <param name="animationTime"></param> <summary>
        /// 
        /// </summary>
        /// <param name="animationTime"></param>
        public void Update(float animationTime)
        {
            Y += Speed; // move down by speed
            X += (float)Math.Sin(animationTime + SwayOffset) * 0.5f; // apply horizontal sway
        }

        /// <summary>
        /// Resets the snowflake to the top with a new random horizontal position.
        /// </summary>
        /// <param name="screenWidth"></param>
        /// <param name="random"></param>
        public void Reset(int screenWidth, Random random)
        {
            Y = -10; // place just above the top of the screen
            X = random.Next(0, screenWidth); // random horizontal position across screen
        }

        /// <summary>
        /// Draws the snowflake on the screen.
        /// </summary> <summary>
        /// 
        /// </summary>
        public void Draw()
        {
            Raylib.DrawCircle((int)X, (int)Y, Size, new Color(255, 255, 255, 200)); // draw semi-transparent white flake
        }

        /// <summary>
        /// Checks if the snowflake is off the bottom of the screen.
        /// </summary>
        /// <param name="screenHeight"></param>
        /// <returns></returns>
        public bool IsOffScreen(int screenHeight)
        {
            return Y > screenHeight; // true if below the bottom of the screen
        }
    }
}
