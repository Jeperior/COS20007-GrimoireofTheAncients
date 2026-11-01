using System;
namespace Grimoire
{
    public class BurnEffect : Effect
    {
        /// <summary>
        /// Initializes a new instance of the BurnEffect class.
        /// </summary>
        /// <param name="tickValue"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public BurnEffect(int tickValue, int duration) : base(tickValue, duration)
        {
        }

        /// <summary>
        /// Applies the burn effect to the target player.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="caster"></param>
        /// <param name="battleLog"></param> <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="caster"></param>
        /// <param name="battleLog"></param>
        public override void Apply(Player target, Player caster, BattleLog? battleLog)
        {
            target.TakeDamage(TickValue); // Apply burn damage
            battleLog?.AddMessage($"{target.Name} takes {TickValue} burn damage.");
            DecrementDuration(); // Decrease the duration of the effect per turn
        }
    }
}