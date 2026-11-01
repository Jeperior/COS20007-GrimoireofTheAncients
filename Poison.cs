using System;
using Raylib_cs;

namespace Grimoire
{
    public class Poison : Spell
    {
        /// <summary>
        /// Initializes a new instance of the Poison class.
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
        public Poison(string name, string description, int castLimit, int damage, float castTime, SpellEffect effect)
            : base(name, description, castLimit, damage, castTime, effect)
        {
        }

        /// <summary>
        /// Casts the Poison spell, dealing damage and applying Poison effect.
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="target"></param>
        public override void Cast(Player caster, Player target)
        {
            CastWithModifier(caster, target, null);
        }

        /// <summary>
        /// Casts the Poison spell with a battle modifier, dealing damage and applying Poison effect.
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="target"></param>
        /// <param name="modifier"></param>
        public void CastWithModifier(Player caster, Player target, BattleModifier? modifier)
        {
            if (!Castable()) return;
            UseCast();

            target.TakeDamage(EffectValue);

            // 50 damage per turn for 4 turns, +5 with Toxic Mist
            int damagePerTurn = 50;
            if (modifier != null && modifier.Type == ModifierType.ToxicMist)
            {
                damagePerTurn = 55; // 50 + 5 bonus
            }

            Effect poison = new PoisonEffect(damagePerTurn, 4); // 4 turns of poison
            target.ApplyEffect(poison);
        }


    }
}
            