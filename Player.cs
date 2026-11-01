using System;
using Raylib_cs;
using System.Collections.Generic;

namespace Grimoire
{
    /// <summary>
    /// Defines a player character in the game.
    /// </summary>
    public class Player
    {
        private string _name;                                           // Player's name
        private int _currentHP;                                         // Player's current health points
        private int _maxHP;                                             // Player's maximum health points
        private float _hitTimer;                                        // Timer to track how long to display Hit status
        private int _selectedWizardId;                                  // Selected wizard sprite ID
        private CharCondition _condition;                               // Player's current condition/status
        private List<Spell> _activeSpells;                              // List of spells the player can cast
        private List<Effect> _activeEffects;                            // List of active effects on the player
        
       
        /// <summary>
        /// Initializes a new instance of the Player class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="currentHP"></param>
        /// <param name="maxHP"></param>
        /// <param name="condition"></param> <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="currentHP"></param>
        /// <param name="maxHP"></param>
        /// <param name="condition"></param>
        public Player(string name, int currentHP, int maxHP, CharCondition condition)
        {
            _name = name;
            _currentHP = currentHP;
            _maxHP = maxHP;
            _hitTimer = 0f;                 // No hit animation initially
            _selectedWizardId = 0;          // Default to first wizard
            _condition = condition;
            _activeSpells = new List<Spell>();
            _activeEffects = new List<Effect>();   
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int CurrentHP
        {
            get { return _currentHP; }
            set { _currentHP = value; }
        }

        public int MaxHP
        {
            get { return _maxHP; }
            set { _maxHP = value; }
        }
        public int SelectedWizardId {
            get { return _selectedWizardId;}
            set { _selectedWizardId = value;}
        }

        public CharCondition Condition
        {
            get {return _condition;}
            set { _condition = value;}
        }

        public List<Spell> ActiveSpells
        {
            get { return _activeSpells; }
        }

        /// <summary>
        /// Reduces the player's HP by the specified value and updates condition if necessary.
        /// </summary>
        /// <param name="value"></param>
        public void TakeDamage(int value)
        {
            _currentHP -= value;
            if (_currentHP < 0) _currentHP = 0;

            if (_currentHP == 0)
            {
                _condition = CharCondition.Dead;
            }
            else if (_condition != CharCondition.Paralyzed) // Don't override Paralyzed status
            {
                // Show hit animation for 1 second
                _condition = CharCondition.Hit;
                _hitTimer = 1.0f; // 1 second duration
            }
        }

        /// <summary>
        /// Heals the player by the specified value, not exceeding max HP.
        /// </summary>
        /// <param name="value"></param>
        public void Heal(int value)
        {
            _currentHP += value;
            if (_currentHP > _maxHP) _currentHP = _maxHP;
        }

        /// <summary>
        /// Checks if the player is alive (HP > 0).
        /// </summary>
        /// <returns></returns> <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsAlive()
        {
            return _currentHP > 0;
        }

        /// <summary>
        /// Sets the player's condition/status.
        /// </summary>
        /// <param name="newCondition"></param> <summary>
        /// 
        /// </summary>
        /// <param name="newCondition"></param>
        public void SetCondition(CharCondition newCondition)
        {
            _condition = newCondition;
            
            // Clear hit timer when setting to Paralyzed to prevent Hit status from overriding it
            if (newCondition == CharCondition.Paralyzed)
            {
                _hitTimer = 0f;
            }
        }

        /// <summary>
        /// Clears the player's condition/status to Idle.
        /// </summary>
        public void ClearCondition()
        {
            _condition = CharCondition.Idle;
        }

        /// <summary>
        /// Applies an effect to the player.
        /// </summary>
        /// <param name="effect"></param> <summary>
        /// 
        /// </summary>
        /// <param name="effect"></param>
        public void ApplyEffect(Effect effect)
        {
            _activeEffects.Add(effect);
        }

        /// <summary>
        /// Clears all active effects from the player.
        /// </summary> <summary>
        /// 
        /// </summary>
        public void ClearAllEffects()
        {
            _activeEffects.Clear();
        }

        /// <summary>
        /// Processes all active effects on the player.
        /// </summary>
        /// <param name="battleLog"></param>
        public void ProcessEffect(BattleLog? battleLog)
        {
            for (int i=0; i < _activeEffects.Count; i++) 
            {
                Effect effect = _activeEffects[i];
                effect.Apply(this, this, battleLog);

                if (effect.IsExpired())
                {
                    _activeEffects.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// Updates the player's status each frame.
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(float deltaTime)
        {
            // Update hit animation timer (but don't override Paralyzed)
            if (_condition == CharCondition.Hit)
            {
                _hitTimer -= deltaTime;
                if (_hitTimer <= 0f)
                {
                    _condition = CharCondition.Idle;
                    _hitTimer = 0f;
                }
            }
        }

        /// <summary>
        /// Indexer to access active spells by index.
        /// </summary>
        /// <value></value>
        public Spell this[int i]
        {
            get { return _activeSpells[i]; }
        }

        public List<Effect> ActiveEffects
        {
            get { return _activeEffects; }
        }
        
    }
}