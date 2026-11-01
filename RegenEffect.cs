using System;
namespace Grimoire
{
    /// <summary>
    /// Represents a regeneration effect that heals the target over time.
    /// </summary>
    public class RegenEffect : Effect
    {
        /// <summary>
        /// Initializes a new instance of the RegenEffect class.
        /// </summary>
        /// <param name="tickValue"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public RegenEffect(int tickValue, int duration) : base(tickValue, duration)
        {
        }
        
        /// <summary>
        /// Applies the regeneration effect to the target player.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="caster"></param>
        /// <param name="battleLog"></param>
        public override void Apply(Player target, Player caster, BattleLog? battleLog)
        {
            caster.Heal(TickValue);
            battleLog?.AddMessage($"{caster.Name} regenerates {TickValue} health.");
            DecrementDuration();
        }
    }
}