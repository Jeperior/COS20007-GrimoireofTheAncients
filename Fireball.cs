using System;
using Raylib_cs;

namespace Grimoire
{
    /// <summary>
    /// Defines the Fireball spell.
    /// </summary> <summary>
    /// 
    /// </summary>
    public class Fireball : Spell
    {
        private Random _random; // Random number generator for effect chance
        
        /// <summary>
        /// Initializes a new instance of the Fireball class.
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
        public Fireball(string name, string description, int castLimit, int damage, float castTime, SpellEffect effect)
            : base(name, description, castLimit, damage, castTime, effect)
        {
            _random = new Random();
        }

        /// <summary>
        /// Casts the Fireball spell, dealing damage and potentially applying Burn effect.
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

            bool applyBurn = _random.NextDouble() <= EffectChance;

            if (applyBurn)
            {
                Effect burn = new BurnEffect(20, 5); // 20 damage per turn for 5 turns
                target.ApplyEffect(burn);
            }
        }
    }
}