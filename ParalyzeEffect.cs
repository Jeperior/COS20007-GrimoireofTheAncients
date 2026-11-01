using System;
namespace Grimoire
{
    /// <summary>
    /// Represents a paralyze effect that prevents the target from acting for a duration.
    /// </summary>
    public class ParalyzeEffect : Effect
    {   
        /// <summary>
        /// Initializes a new instance of the ParalyzeEffect class.
        /// </summary>
        /// <param name="duration"></param> <summary>
        /// 
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        public ParalyzeEffect(int tickValue,int duration) : base(tickValue,duration)
        {
        }

        /// <summary>
        /// Applies the paralyze effect to the target player.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="caster"></param>
        /// <param name="battleLog"></param>
        public override void Apply(Player target, Player caster, BattleLog? battleLog)
        {
            // Only set paralyzed condition if not already paralyzed
            if (target.Condition != CharCondition.Paralyzed)
            {
                target.SetCondition(CharCondition.Paralyzed);
            }
        }
    }
}