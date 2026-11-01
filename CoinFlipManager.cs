using System;
using Raylib_cs;

namespace Grimoire
{
    /// <summary>
    /// Defines the sides of a coin.
    /// </summary> <summary>
    /// 
    /// </summary>
    public class CoinFlipManager
    {
        private CoinSide _playerChoice; // Player's chosen side
        private CoinSide _result;      // Result of the coin flip

        /// <summary>
        /// Initializes a new instance of the CoinFlipManager class.
        /// </summary>
        /// <param name="playerChoice"></param>
        /// <param name="result"></param>
        public CoinFlipManager(CoinSide playerChoice, CoinSide result)
        {
            _playerChoice = playerChoice;
            _result = result;
        }

        /// <summary>
        /// Flips the coin and returns the result.
        /// </summary>
        /// <returns></returns>
        public CoinSide Flip()
        {
            Random random = new Random();
            if (random.Next(0, 2) == 0)
            {
                _result = CoinSide.Heads;
            }
            else
            {
                _result = CoinSide.Tails;
            }
            return _result;
        }

        /// <summary>
        /// Determines if the player's choice matches the coin flip result.
        /// </summary>
        /// <returns></returns> <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool PlayerChoiceResult()
        {
            return _playerChoice == _result;
        }


    }
}