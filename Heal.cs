using System;

namespace Grimoire
{
    /// <summary>
    /// Represents a healing spell.
    /// </summary>
    public class Heal : Spell
    {
        public Heal(string name, string description, int castLimit, int damage, float castTime, SpellEffect effect)
            : base(name, description, castLimit, damage, castTime, effect)
        {
        }

        /// <summary>
        /// Casts the Heal spell, restoring health to the caster.
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

            caster.Heal(EffectValue);     
        }
    }
}