using System;
using Raylib_cs;

namespace Grimoire
{
    /// <summary>
    /// Defines the Cure spell.
    /// </summary> <summary>
    /// 
    /// </summary>
    public class Cure : Spell
    {
        /// <summary>
        /// Initializes a new instance of the Cure class.
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
        public Cure(string name, string description, int castLimit, int damage, float castTime, SpellEffect effect) 
            : base(name, description, castLimit, damage, castTime, effect)
        {
        }

        /// <summary>
        /// Casts the Cure spell, removing all negative effects from the caster.
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

            caster.ClearAllEffects();
            Raylib.DrawText($"{Name} removed all effects from {target.Name}!", 50, 200, 30, Color.Gold);
        }
    }
}