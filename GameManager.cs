using System;
using Raylib_cs;

namespace Grimoire
{   
    /// <summary>
    /// Manages the overall game state and transitions.
    /// </summary> <summary>
    /// 
    /// </summary>
    public class GameManager
    {
        private UIManager _uIManager;  // UI manager
        private BattleModifier _battleModifier; // Current battle modifier
        private CoinFlipManager _coinFlipManager; // Coin flip manager
        private GameState _state; // Current game state
        private Player _player1; // Player 1
        private Player _player2; // Player 2
        private Spellbook _player1Spellbook; // Player 1's spellbook
        private Spellbook _player2Spellbook; // Player 2's spellbook

        /// <summary>
        /// Initializes a new instance of the GameManager class.
        /// </summary>
        /// <param name="uIManager"></param>
        /// <param name="battleModifier"></param>
        /// <param name="coinFlipManager"></param>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <param name="player1Spellbook"></param>
        /// <param name="player2Spellbook"></param>
        public GameManager(UIManager uIManager, BattleModifier battleModifier, CoinFlipManager coinFlipManager, Player player1, Player player2, Spellbook player1Spellbook, Spellbook player2Spellbook)
        {
            _uIManager = uIManager;
            _battleModifier = battleModifier;
            _coinFlipManager = coinFlipManager;
            _state = GameState.ProfileSelect;
            _player1 = player1;
            _player2 = player2;
            _player1Spellbook = player1Spellbook;
            _player2Spellbook = player2Spellbook;
        }
        
        // Properties
        public UIManager UIManager
        {
            get { return _uIManager; }
            set { _uIManager = value; }
        }

        public CoinFlipManager CoinFlipManager
        {
            get { return _coinFlipManager; }
            set { _coinFlipManager = value; }
        }

        public GameState State
        {
            get { return _state; }
        }

        /// <summary>
        /// Initializes the GameManager with default players, spellbooks, and a randomized battle modifier.
        /// </summary>
        /// <returns></returns> <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static GameManager Initialize()
        {
            Random random = new Random();
            
            // Create players
            Player player1 = new Player("Player 1", 1000, 1000, CharCondition.Idle);
            Player player2 = new Player("Player 2", 1000, 1000, CharCondition.Idle);
            
            // Create randomized battle modifier
            ModifierType[] modifierTypes = { ModifierType.ToxicMist, ModifierType.Inferno, ModifierType.HealSurge, ModifierType.Thunderstorm };
            ModifierType randomModifierType = modifierTypes[random.Next(modifierTypes.Length)];
            BattleModifier battleModifier = new BattleModifier(randomModifierType, "");
            
            // Create spellbooks for both players
            Spellbook player1Spellbook = new Spellbook();
            player1Spellbook.InitializeSpells();
            Spellbook player2Spellbook = new Spellbook();
            player2Spellbook.InitializeSpells();
            
            // Create managers
            UIManager uiManager = new UIManager(battleModifier);
            uiManager.InitializeSpellbooks(player1Spellbook, player2Spellbook);
            
            CoinFlipManager coinFlipManager = new CoinFlipManager(CoinSide.Heads, CoinSide.Tails);
            
            return new GameManager(uiManager, battleModifier, coinFlipManager, player1, player2, player1Spellbook, player2Spellbook);
        }

        /// <summary>
        /// Changes the current game state.
        /// </summary>
        /// <param name="state"></param>
        public void ChangeState(GameState state)
        {
            _state = state;
            
            // When entering main menu from profile select, update player names from profiles
            if (state == GameState.MainMenu)
            {
                PlayerProfile? profile1 = _uIManager.GetPlayer1Profile();
                PlayerProfile? profile2 = _uIManager.GetPlayer2Profile();
                
                if (profile1 != null)
                {
                    _player1.Name = profile1.Username;
                    _player1.SelectedWizardId = profile1.SelectedWizardId;
                }
                
                if (profile2 != null)
                {
                    _player2.Name = profile2.Username;
                    _player2.SelectedWizardId = profile2.SelectedWizardId;
                }
            }
            
            // When entering wizard select, initialize with current profiles
            if (state == GameState.WizardSelect)
            {
                PlayerProfile? profile1 = _uIManager.GetPlayer1Profile();
                PlayerProfile? profile2 = _uIManager.GetPlayer2Profile();
                
                if (profile1 != null && profile2 != null)
                {
                    _uIManager.InitializeWizardSelectScreen(profile1, profile2);
                }
                
                // Reset game state for new game
                _player1Spellbook.SpellLoadout.Clear();
                _player2Spellbook.SpellLoadout.Clear();
                _player1.ActiveSpells.Clear();
                _player2.ActiveSpells.Clear();
                _player1.ClearAllEffects(); // Clear burn, poison, regen, etc.
                _player2.ClearAllEffects();
                _player1.CurrentHP = _player1.MaxHP;
                _player2.CurrentHP = _player2.MaxHP;
                _player1.Condition = CharCondition.Idle;
                _player2.Condition = CharCondition.Idle;
                _uIManager.ResetBattleScreen(); // Reset battle log and battle state
            }
            
            // When returning to profile select, reset profiles and loadouts
            if (state == GameState.ProfileSelect)
            {
                _player1Spellbook.SpellLoadout.Clear();
                _player2Spellbook.SpellLoadout.Clear();
            }
            
            // When entering loadout select, reset the loadout screen
            if (state == GameState.LoadoutSelect)
            {
                _uIManager.InitializeSpellbooks(_player1Spellbook, _player2Spellbook);
            }
            
            // When entering coin flip, reset the coin flip screen
            if (state == GameState.CoinFlip)
            {
                _uIManager.ResetCoinFlipScreen();
            }
            
            // When entering end screen, save game results to profiles
            if (state == GameState.EndScreen)
            {
                // Determine winner/loser from player HP
                Player winner = _player1.CurrentHP > 0 ? _player1 : _player2;
                Player loser = _player1.CurrentHP > 0 ? _player2 : _player1;
                _uIManager.SaveGameResult(winner, loser);
            }
            
            // When entering battle, transfer spells from spellbook to players
            if (state == GameState.Battle)
            {
                // Update wizard selection from profiles
                PlayerProfile? profile1 = _uIManager.GetPlayer1Profile();
                PlayerProfile? profile2 = _uIManager.GetPlayer2Profile();
                
                if (profile1 != null)
                {
                    _player1.SelectedWizardId = profile1.SelectedWizardId;
                }
                
                if (profile2 != null)
                {
                    _player2.SelectedWizardId = profile2.SelectedWizardId;
                }
                
                TransferSpellsToPlayers();
            }
        }

        /// <summary>
        /// Transfers spells from spellbooks to players at the start of battle.
        /// </summary>
        public void TransferSpellsToPlayers()
        {
            // Clear existing spells
            _player1.ActiveSpells.Clear();
            _player2.ActiveSpells.Clear();
            
            // Transfer from spellbook to player and reset casts
            foreach (Spell spell in _player1Spellbook.SpellLoadout)
            {
                spell.Reset(); // Reset spell casts to full
                _player1.ActiveSpells.Add(spell);
            }
            
            foreach (Spell spell in _player2Spellbook.SpellLoadout)
            {
                spell.Reset(); // Reset spell casts to full
                _player2.ActiveSpells.Add(spell);
            }
        }

        /// <summary>
        /// Draws the current game state.
        /// </summary>
        public void Draw()
        {
            switch (_state)
            {
                case GameState.ProfileSelect:
                    _uIManager.DrawProfileScreen();
                    break;
                case GameState.MainMenu:
                    _uIManager.DrawMainMenu();
                    break;
                case GameState.WizardSelect:
                    _uIManager.DrawWizardSelectScreen();
                    break;
                case GameState.LoadoutSelect:
                    _uIManager.DrawLoadoutScreen();
                    break;
                case GameState.CoinFlip:
                    _uIManager.DrawCoinFlipScreen();
                    break;
                case GameState.Battle:
                    _uIManager.DrawBattleScreen(_player1, _player2, _battleModifier);
                    break;
                case GameState.EndScreen:
                    _uIManager.DrawEndScreen();
                    break; 
            }

        }

        /// <summary>
        /// Updates the current game state.
        /// </summary>
        public void Update()
        {  
         switch (_state)
            {
                case GameState.ProfileSelect:
                    _uIManager.UpdateProfileScreen(this);
                    break;
                case GameState.MainMenu:
                    _uIManager.UpdateMainMenu(this);
                    break;
                case GameState.WizardSelect:
                    _uIManager.UpdateWizardSelectScreen(this);
                    break;
                case GameState.LoadoutSelect:
                    _uIManager.UpdateLoadoutScreen(this);
                    break;
                case GameState.CoinFlip:
                    _uIManager.UpdateCoinFlipScreen(this);
                    break;
                case GameState.Battle:
                    _uIManager.UpdateBattleScreen(this, _player1, _player2);
                    break;
                case GameState.EndScreen:
                    _uIManager.UpdateEndScreen(this);
                    break; 
            }
        }
    }
}