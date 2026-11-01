using System;
namespace Grimoire
{
    /// <summary>
    /// Represents a poison effect that deals damage over time.
    /// </summary>
    public class PoisonEffect : Effect
    {   
        /// <summary>
        /// Initializes a new instance of the PoisonEffect class.
        /// </summary>
        /// <param name="tickValue"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public PoisonEffect(int tickValue, int duration) : base(tickValue, duration)
        {
        }

        /// <summary>
        /// Applies the poison effect to the target player.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="caster"></param>
        /// <param name="battleLog"></param>
        public override void Apply(Player target, Player caster, BattleLog? battleLog)
        {
            target.TakeDamage(TickValue);
            battleLog?.AddMessage($"{target.Name} takes {TickValue} poison damage.");
            DecrementDuration();
        }
    }
}