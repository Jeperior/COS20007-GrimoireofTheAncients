using System;
using System.Collections.Generic;

namespace Grimoire
{
    public class BMRandomizer
    {   
        // Pool of available battle modifiers
        private List<ModifierType> _modifierPool;       // List of modifier types
        private Random _randomizer;                     // Random number generator

        /// <summary>
        /// Initializes a new instance of the BMRandomizer class.
        /// </summary>
        /// <param name="randomizer"></param>
        public BMRandomizer(Random randomizer)
        {
            _modifierPool = new List<ModifierType>
            {
                ModifierType.ToxicMist,
                ModifierType.Inferno,
                ModifierType.HealSurge,
                ModifierType.Thunderstorm   
            };

            _randomizer = randomizer;
        }

        /// <summary>
        /// Gets a random battle modifier from the pool.
        /// </summary>
        /// <returns></returns>
        public ModifierType GetRandomModifier()
        {
            int index = _randomizer.Next(_modifierPool.Count);
            return _modifierPool[index];
        }

        /// <summary>
        /// Indexer to access modifier types by index.
        /// </summary>
        /// <value></value>
        public ModifierType this[int i]
        {
            get { return _modifierPool[i]; }
        }
    }
}