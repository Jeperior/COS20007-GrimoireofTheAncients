using Raylib_cs;
using System;
namespace Grimoire
{
    /// <summary>
    /// Manages the user interface components and screens.
    /// </summary>
    public class UIManager
    {
        private BattleModifier _battleModifier; // global battle modifiers affecting rules/effects
        
        // Particle system
        private ParticleManager _particleManager; // manages particle effects (spells, hits, VFX)
        
        // Animation system
        private Animation _animation; // shared animation utilities for UI and entities
        
        // Audio system
        private AudioManager _audioManager; // handles sound effects and music playback

        // Screen delegates
        private ProfileScreen _profileScreen; // profile selection and management screen
        private MainMenuScreen _mainMenuScreen; // main menu UI
        private WizardSelectScreen _wizardSelectScreen; // wizard selection UI
        private LoadoutScreen _loadoutScreen; // loadout and spellbook configuration UI
        private CoinFlipScreen _coinFlipScreen; // coin flip UI to determine first turn
        private EndScreen _endScreen; // end-of-match summary and results screen
        private BattleScreen _battleScreen; // active battle UI and turn handling

        // Battle log
        private BattleLog _battleLog; // stores recent battle messages for display

        /// <summary>
        /// Initializes a new instance of the UIManager class.
        /// </summary>
        /// <param name="battleModifier"></param>
        public UIManager(BattleModifier battleModifier)
        {
            _battleModifier = battleModifier;
            _particleManager = new ParticleManager();
            _animation = new Animation();
            _audioManager = new AudioManager();
            _battleLog = new BattleLog(100, 5);
            _profileScreen = new ProfileScreen(_particleManager, _audioManager);
            _mainMenuScreen = new MainMenuScreen(_particleManager, _audioManager);
            _wizardSelectScreen = new WizardSelectScreen(_animation, _particleManager, _audioManager);
            _loadoutScreen = new LoadoutScreen(_particleManager, _audioManager);
            _coinFlipScreen = new CoinFlipScreen();
            _endScreen = new EndScreen(_animation, _particleManager);
            _battleScreen = new BattleScreen(_particleManager, _animation, _battleLog, _coinFlipScreen, _audioManager);
        }

        // Profile Screen

        /// <summary>
        /// Updates the profile screen based on player input.
        /// </summary>
        /// <param name="gameManager"></param> <summary>
        /// 
        /// </summary>
        /// <param name="gameManager"></param>
        public void UpdateProfileScreen(GameManager gameManager)
        {
            _profileScreen.Update(gameManager);
        }

        /// <summary>
        /// Draws the profile screen.
        /// </summary>
        public void DrawProfileScreen()
        {
            _profileScreen.Draw();
        }

        /// <summary>
        /// Gets the selected profile for player 1.
        /// </summary>
        /// <returns></returns>
        public PlayerProfile? GetPlayer1Profile()
        {
            return _profileScreen.Player1Profile;
        }

        /// <summary>
        /// Gets the selected profile for player 2.
        /// </summary>
        /// <returns></returns>
        public PlayerProfile? GetPlayer2Profile()
        {
            return _profileScreen.Player2Profile;
        }

        /// <summary>
        /// Saves the game result to the player profiles.
        /// </summary>
        /// <param name="winner"></param>
        /// <param name="loser"></param>
        public void SaveGameResult(Player winner, Player loser)
        {
            PlayerProfile? winnerProfile = null;
            PlayerProfile? loserProfile = null;
            
            // Find which profile belongs to winner/loser
            if (_profileScreen.Player1Profile != null && _profileScreen.Player1Profile.Username == winner.Name)
            {
                winnerProfile = _profileScreen.Player1Profile;
                loserProfile = _profileScreen.Player2Profile;
            }
            else if (_profileScreen.Player2Profile != null && _profileScreen.Player2Profile.Username == winner.Name)
            {
                winnerProfile = _profileScreen.Player2Profile;
                loserProfile = _profileScreen.Player1Profile;
            }
            
            // Update and save winner profile
            if (winnerProfile != null)
            {
                winnerProfile.RecordGame(true, 50); // 50 XP for winning
                ProfileManager profileManager = new ProfileManager();
                profileManager.SaveProfile(winnerProfile);
            }
            
            // Update and save loser profile
            if (loserProfile != null)
            {
                loserProfile.RecordGame(false, 10); // 10 XP for participating
                ProfileManager profileManager = new ProfileManager();
                profileManager.SaveProfile(loserProfile);
            }
        }
        
        // Main Menu

        /// <summary>
        /// Updates the main menu screen based on player input.
        /// </summary>
        /// <param name="gameManager"></param>
        public void UpdateMainMenu(GameManager gameManager)
        {
            _mainMenuScreen.Update(gameManager);
        }

        /// <summary>
        /// Draws the main menu screen.
        /// </summary>
        public void DrawMainMenu()
        {
            _mainMenuScreen.Draw(_profileScreen.Player1Profile, _profileScreen.Player2Profile);
        }

        // Coin Flip Screen

        /// <summary>
        /// Updates the coin flip screen logic.
        /// </summary>
        /// <param name="gameManager"></param>
        public void UpdateCoinFlipScreen(GameManager gameManager)
        {
            _coinFlipScreen.Update(gameManager);
            
            // Set current turn from coin flip result
            if (_coinFlipScreen.BattleStarted && _battleScreen.CurrentTurn == 1)
            {
                _battleScreen.CurrentTurn = _coinFlipScreen.FirstTurn;
            }
        }
        
        /// <summary>
        /// Resets the coin flip screen state.
        /// </summary>
        public void ResetCoinFlipScreen()
        {
            _coinFlipScreen.Reset();
        }

        /// <summary>
        /// Draws the coin flip screen.
        /// </summary>
        public void DrawCoinFlipScreen()
        {
            _coinFlipScreen.Draw();
        }

        // Wizard Selection Screen

        /// <summary>
        /// Updates the wizard selection screen based on player input.
        /// </summary>
        /// <param name="gameManager"></param>
        public void UpdateWizardSelectScreen(GameManager gameManager)
        {
            if (_wizardSelectScreen.Update(gameManager))
            {
                // Both players confirmed - proceed to loadout
                gameManager.ChangeState(GameState.LoadoutSelect);
            }
        }

        /// <summary>
        /// Draws the wizard selection screen.
        /// </summary>
        public void DrawWizardSelectScreen()
        {
            _wizardSelectScreen.Draw();
        }

        /// <summary>
        /// Initializes the wizard selection screen with player profiles.
        /// </summary>
        /// <param name="player1Profile"></param>
        /// <param name="player2Profile"></param> <summary>
        /// 
        /// </summary>
        /// <param name="player1Profile"></param>
        /// <param name="player2Profile"></param>
        public void InitializeWizardSelectScreen(PlayerProfile player1Profile, PlayerProfile player2Profile)
        {
            _wizardSelectScreen.Initialize(player1Profile, player2Profile);
        }
        
        // Loadout Screen

        /// <summary>
        /// Draws the loadout screen.
        /// </summary>
        public void DrawLoadoutScreen()
        {
            _loadoutScreen.Draw();
        }

        /// <summary>
        /// Updates the loadout screen based on player input.
        /// </summary>
        /// <param name="gameManager"></param>
        public void UpdateLoadoutScreen(GameManager gameManager)
        {
            _loadoutScreen.Update(gameManager);
        }


        /// <summary>
        /// Initializes the loadout screen with the players' spellbooks.
        /// </summary>
        /// <param name="player1Spellbook"></param>
        /// <param name="player2Spellbook"></param>
        public void InitializeSpellbooks(Spellbook player1Spellbook, Spellbook player2Spellbook)
        {
            _loadoutScreen.Initialize(player1Spellbook, player2Spellbook);
        }

        // Battle Screen
        /// <summary>
        /// Draws the battle screen.
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <param name="modifier"></param> <summary>
        /// 
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <param name="modifier"></param>
        public void DrawBattleScreen(Player player1, Player player2, BattleModifier modifier)
        {
            _battleScreen.Draw(player1, player2, modifier);
        }

        /// <summary>
        /// Updates the battle screen logic.
        /// </summary>
        /// <param name="gameManager"></param>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        public void UpdateBattleScreen(GameManager gameManager, Player player1, Player player2)
        {
            _battleScreen.Update(gameManager, player1, player2, _battleModifier);
        }

        /// <summary>
        /// Resets the battle screen to initial state.
        /// </summary> <summary>
        /// 
        /// </summary>
        public void ResetBattleScreen()
        {
            _battleScreen.ResetBattle();
        }
        
        // End Screen

        /// <summary>
        /// Updates the end screen logic.
        /// </summary>
        /// <param name="gameManager"></param> <summary>
        /// 
        /// </summary>
        /// <param name="gameManager"></param>
        public void UpdateEndScreen(GameManager gameManager)
        {
            _endScreen.Update(gameManager);
        }

        /// <summary>
        /// Draws the end screen.
        /// </summary> <summary>
        /// 
        /// </summary>
        public void DrawEndScreen()
        {
            _endScreen.Draw();
        }

        /// <summary>
        /// Sets the winner and loser for the end screen display.
        /// </summary>
        /// <param name="winner"></param>
        /// <param name="loser"></param>
        public void SetEndScreenWinner(Player winner, Player loser)
        {
            _endScreen.SetWinner(winner, loser);
        }

        /// <summary>
        /// Sets the end screen draw data.
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        public void SetEndScreenDraw(Player player1, Player player2)
        {
            _endScreen.SetDraw(player1, player2);
        }
    }
}