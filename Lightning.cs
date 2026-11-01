using System;

namespace Grimoire
{   
    /// <summary>
    /// Represents a lightning spell that deals damage and may paralyze the target.
    /// </summary> <summary>
    /// 
    /// </summary>
    public class Lightning : Spell
    {
        private Random _random; // Random number generator for effect chance

        /// <summary>
        /// Initializes a new instance of the Lightning class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="castLimit"></param>
        /// <param name="damage"></param>
        /// <param name="castTime"></param>
        /// <param name="effect"></param> <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="castLimit"></param>
        /// <param name="damage"></param>
        /// <param name="castTime"></param>
        /// <param name="effect"></param>
        public Lightning(string name, string description, int castLimit, int damage, float castTime, SpellEffect effect)
            : base(name, description, castLimit, damage, castTime, effect)
        {
            _random = new Random();
        }

        /// <summary>
        /// Casts the Lightning spell, dealing damage and potentially applying Paralyze effect.
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="target"></param> <summary>
        /// 
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="target"></param>
        public override void Cast(Player caster, Player target)
        {
            if (!Castable()) return;
            UseCast();

            target.TakeDamage(EffectValue);

            bool applyParalyze = _random.NextDouble() <= EffectChance;

            if (applyParalyze)
            {
                Effect paralyze = new ParalyzeEffect(0,1); // 1 turn duration
                target.ApplyEffect(paralyze);
            }
        }
    }
}