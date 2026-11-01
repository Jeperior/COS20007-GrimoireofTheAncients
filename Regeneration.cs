using Raylib_cs;
using System;
namespace Grimoire
{
    /// <summary>
    /// Defines the Regeneration spell.
    /// </summary> <summary>
    /// 
    /// </summary>
    public class Regeneration : Spell
    {
        /// <summary>
        /// Initializes a new instance of the Regeneration class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="castLimit"></param>
        /// <param name="damage"></param>
        /// <param name="castTime"></param>
        /// <param name="effect"></param>
        public Regeneration(string name, string description, int castLimit, int damage, float castTime, SpellEffect effect)
            : base(name, description, castLimit, damage, castTime, effect)
        {
        }

        /// <summary>
        /// Casts the Regeneration spell, applying a regeneration effect to the caster.
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="target"></param>
        public override void Cast(Player caster, Player target)
        {
            if (!Castable()) return;
            UseCast();

            Effect regen = new RegenEffect(EffectValue, 3); // 80 HP per turn for 3 turns
            caster.ApplyEffect(regen);
        }
    }
}