using Raylib_cs;
using System;
namespace Grimoire
{   
    /// <summary>
    /// Defines the base Spell class.
    /// </summary>
    public abstract class Spell // base class for all spells
    {
        private string _name; // spell name
        private string _description; // short description of spell
        private int _castLimit; // maximum number of times this spell can be cast before reset
        private int _castRemaining; // remaining casts available
        private int _effectValue; // numeric strength/value of the spell effect
        private float _effectChance; // chance (0.0-1.0) that the effect will occur
        private SpellEffect _effect; // the effect object/enum associated with the spell

        /// <summary>
        /// Initializes a new instance of the Spell class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="castLimit"></param>
        /// <param name="effectValue"></param>
        /// <param name="effectChance"></param>
        /// <param name="effect"></param>
        public Spell(string name, string description, int castLimit, int effectValue, float effectChance, SpellEffect effect)
        {
            _name = name; // set name
            _description = description; // set description
            _castLimit = castLimit; // set maximum casts
            _castRemaining = castLimit; // initialize remaining casts to the limit
            _effectValue = effectValue; // set effect value
            _effectChance = effectChance; // set effect chance
            _effect = effect; // set effect type
        }

        public string Name
        {
            get { return _name; } // get spell name
            set { _name = value; } // set spell name
        }

        public string Description
        {
            get { return _description; } // get description
            set { _description = value; } // set description
        }

        public int CastLimit
        {
            get { return _castLimit; } // get max casts
            set { _castLimit = value; } // set max casts
        }

        public int CastRemaining
        {
            get { return _castRemaining; } // get remaining casts
            set { _castRemaining = value; } // set remaining casts
        }

        public int EffectValue
        {
            get { return _effectValue; } // get effect value
            set { _effectValue = value; } // set effect value
        }

        public float EffectChance
        {
            get { return _effectChance; } // get effect chance
            set { _effectChance = value; } // set effect chance
        }

        public SpellEffect Effect
        {
            get { return _effect; } // get the effect (read-only)
            
        }

        /// <summary>
        /// Checks if the spell is castable (has remaining casts).
        /// </summary>
        /// <returns></returns> <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Castable()
        {
           return _castRemaining > 0; // true if at least one cast remains
        }
        
        /// <summary>
        /// Uses one cast of the spell, reducing remaining casts by one.
        /// </summary>
        public void UseCast()
        {
            if (_castRemaining > 0)
            {
                _castRemaining--; // consume one cast if available
            }
        }

        /// <summary>
        /// Resets the spell's remaining casts to the cast limit.
        /// </summary>
        public void Reset()
        {
            _castRemaining = _castLimit; // restore casts to the limit
        }
        
        /// <summary>
        /// Casts the spell on the target player.
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="target"></param>
        public virtual void Cast(Player caster, Player target)
        {
            if (Castable())
            {
                UseCast(); // default behavior: consume a cast when used
            }
        }

    }

}