using System.Collections.Generic;
using System;
namespace Grimoire
{
    /// <summary>
    /// Represents a collection of spells available to the player.
    /// </summary>
    public class Spellbook
    {
        private List<Spell> _spells; // all known spells available to the player
        private List<Spell> _spellLoadout; // currently equipped spells (up to 4)
        
        /// <summary>
        /// Initializes a new instance of the Spellbook class.
        /// </summary>
        public Spellbook()
        {
            _spells = new List<Spell>();
            _spellLoadout = new List<Spell>();
        }
        
        public List<Spell> Spells
        {
            get { return _spells; }
        }

        public List<Spell> SpellLoadout
        {
            get { return _spellLoadout; }
        }

        /// <summary>
        /// Equips a spell to the loadout if there is space and it's not already equipped.
        /// </summary>
        /// <param name="spell"></param>
        public void EquipSpell(Spell spell)
        {
            if (_spellLoadout.Count < 4 && !_spellLoadout.Contains(spell))
            {
                _spellLoadout.Add(spell);
            }
        }

        /// <summary>
        /// Unequips a spell from the specified loadout slot.
        /// </summary>
        /// <param name="slot"></param>
        public void UnequipSpell(int slot)
        {
            if (slot >= 0 && slot < _spellLoadout.Count)
            {
                _spellLoadout.RemoveAt(slot);
            }
        }

        /// <summary>
        /// Initializes the spellbook with predefined spells.
        /// </summary>
        public void InitializeSpells()
        {
            _spells = new List<Spell>
            {
                new Fireball("Fireball", "-375 HP | 15% chance of Burn (20 damage per turn for 5 turns)", 5, 375, 0.15f, SpellEffect.Burn),
                new Lightning("Lightning", "-200 HP | 20% chance of Paralyze (skip 1 turn)", 4, 200, 0.2f, SpellEffect.Paralyze),
                new Poison("Poison", "-50 HP | Poison (50 damage per turn for 4 turns)", 3, 50, 1.0f, SpellEffect.Poison),
                new Heal("Heal", "+300 HP instant heal", 3, 300, 1.0f, SpellEffect.Heal),
                new Cure("Cure", "Remove all negative effects", 2, 0, 1.0f, SpellEffect.Cure),
                new Regeneration("Regeneration", "+80 HP per turn for 3 turns (240 HP total)", 3, 80, 1.0f, SpellEffect.Regen)
            };
        }
        
        /// <summary>
        /// Indexer to access spells by index.
        /// </summary>
        /// <value></value>
        public Spell this[int i]
        {
            get { return _spells[i]; }
        }
    }
}
