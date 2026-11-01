using System;
namespace Grimoire
{   
    /// <summary>
    /// Abstract base class for status effects applied to players.
    /// </summary> <summary>
    /// 
    /// </summary>
    public abstract class Effect
    {
        
        private int _tickValue; // Damage or healing per turn/tick
        private int _duration; // Duration in turns

        /// <summary>
        /// Initializes a new instance of the Effect class.
        /// </summary>
        /// <param name="tickValue"></param>
        /// <param name="duration"></param>
        public Effect(int tickValue, int duration)
        {
            _tickValue = tickValue;
            _duration = duration;
        }

        // Property
        public int TickValue
        {
            get { return _tickValue; }
            set { _tickValue = value; }
        }

        /// <summary>
        /// Decrements the duration of the effect by one turn.
        /// </summary>
        public void DecrementDuration()
        {
            _duration--;
        }

        /// <summary>
        /// Checks if the effect has expired.
        /// </summary>
        /// <returns></returns>
        public bool IsExpired()
        {
            return _duration <= 0;
        }   
        
        /// <summary>
        /// Applies the effect to the target player.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="caster"></param>
        /// <param name="battleLog"></param>
        public abstract void Apply(Player target, Player caster, BattleLog? battleLog);
    }
}
    
