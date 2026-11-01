using System;
namespace Grimoire
{
    public class BattleModifier
    {
        // Defines different types of battle modifiers
        private ModifierType _type;
        private string _description;

        /// <summary>
        /// Initializes a new instance of the BattleModifier class.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="description"></param>
        public BattleModifier(ModifierType type,string description)
        {
            _type = type;
            _description = description;
            Apply();
        }

        // Properties to access modifier details
        public ModifierType Type
        {
            get { return _type; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Applies the description of the modifier based on its type.
        /// </summary> <summary>
        /// 
        /// </summary>
        public void Apply()
        {
             switch (_type)
             {
            case ModifierType.ToxicMist:
                _description = "Poison deals +10% damage";
                break;
            case ModifierType.Inferno:
                _description = "Fireball has +15% burn chance";
                break;
            case ModifierType.HealSurge:
                _description = "Healing spells restore +20% more HP";
                break;
            case ModifierType.Thunderstorm:
                _description = "Lightning has +15% paralyze chance";
                break;
            }
        }

    }
}