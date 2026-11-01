using System;
using System.Collections.Generic;
using Raylib_cs;

namespace Grimoire
{
    /// <summary>
    /// Handles the profile selection/creation screen.
    /// </summary>
    public class ProfileScreen
    {
        private ProfileManager _profileManager; // Profile manager (load/save/create/delete profiles)
        private ParticleManager _particleManager; // Particle system for background visuals
        private AudioManager _audioManager; // Plays key/select/back sounds

        private List<string> _existingProfiles; // Ordered list of all profile names shown in UI

        private Dictionary<string, PlayerProfile> _cachedProfiles; // Cache of loaded profiles keyed by username

        private int _selectedProfileIndex; // Current selection index (0..existingProfiles.Count, last = "Create New")

        private bool _isCreatingNew; // True when user is entering a new profile name
        private string _newUsername; // Buffer for typed username during creation
        private int _maxUsernameLength; // Maximum allowed username length

        private int _scrollOffset; // Index of first visible profile when scrolling long lists
        private int _maxVisibleProfiles; // How many profiles to show at once in the list

        private bool _isConfirmingDelete; // True when delete confirmation prompt is active
        private string _profileToDelete; // Username pending deletion confirmation

        private PlayerProfile? _player1Profile; // Selected profile for Player 1 (nullable)
        private PlayerProfile? _player2Profile; // Selected profile for Player 2 (nullable)

        private int _currentPlayer; // Which player is currently selecting a profile (1 or 2)

        /// <summary>
        /// Initializes a new instance of the ProfileScreen class.
        /// </summary>
        /// <param name="particleManager"></param>
        /// <param name="audioManager"></param> <summary>
        /// 
        /// </summary>
        /// <param name="particleManager"></param>
        /// <param name="audioManager"></param>
        public ProfileScreen(ParticleManager particleManager, AudioManager audioManager)
        {
            _particleManager = particleManager;
            _audioManager = audioManager;
            _profileManager = new ProfileManager();

            _existingProfiles = new List<string>();
            _cachedProfiles = new Dictionary<string, PlayerProfile>();

            // Initialize UI/state defaults
            _selectedProfileIndex = 0;
            _isCreatingNew = false;
            _newUsername = "";
            _maxUsernameLength = 15;
            _currentPlayer = 1;
            _scrollOffset = 0;
            _maxVisibleProfiles = 8;
            _isConfirmingDelete = false;
            _profileToDelete = "";
            
            // Populate the profile list and cache on creation
            RefreshProfileList();
        }

        /// <summary>
        /// Currently selected profile for Player 1 (may be null).
        /// </summary>
        public PlayerProfile? Player1Profile 
        { 
            get { return _player1Profile; } 
        }
        
        /// <summary>
        /// Currently selected profile for Player 2 (may be null).
        /// </summary>
        public PlayerProfile? Player2Profile 
        { 
            get { return _player2Profile; } 
        }

        /// <summary>
        /// Refresh the in-memory list/cache of profiles from persistent storage.
        /// Clears and rebuilds the cache and clamps selection index if needed.
        /// </summary>
        public void RefreshProfileList()
        {
            _existingProfiles = _profileManager.GetAllProfileNames();
            _cachedProfiles.Clear();
            
            // Cache all profiles to avoid repeated loads while drawing
            foreach (string username in _existingProfiles)
            {
                PlayerProfile? profile = _profileManager.LoadProfile(username);
                if (profile != null)
                {
                    _cachedProfiles[username] = profile;
                }
            }
            
            // Ensure selected index is valid after refresh
            if (_selectedProfileIndex >= _existingProfiles.Count)
            {
                _selectedProfileIndex = Math.Max(0, _existingProfiles.Count - 1);
            }
        }

        /// <summary>
        /// Main update entrypoint for the screen. Delegates to appropriate input handlers.
        /// </summary>
        public void Update(GameManager gameManager)
        {
            if (_isCreatingNew)
            {
                HandleNewProfileInput(gameManager);
            }
            else
            {
                HandleProfileSelection(gameManager);
            }
        }

        /// <summary>
        /// Handle navigation, selection, deletion confirmation and backing out when not creating a new profile.
        /// </summary>
        public void HandleProfileSelection(GameManager gameManager)
        {
            // Handle delete confirmation keyboard responses first
            if (_isConfirmingDelete)
            {
                if (Raylib.IsKeyPressed(KeyboardKey.Y))
                {
                    _audioManager.PlaySelectSound();
                    _profileManager.DeleteProfile(_profileToDelete);
                    RefreshProfileList();
                    _isConfirmingDelete = false;
                    _profileToDelete = "";
                    _selectedProfileIndex = 0;
                    _scrollOffset = 0;
                }
                else if (Raylib.IsKeyPressed(KeyboardKey.N) || Raylib.IsKeyPressed(KeyboardKey.Escape))
                {
                    _audioManager.PlayBackSound();
                    _isConfirmingDelete = false;
                    _profileToDelete = "";
                }
                return;
            }

            // Navigate down through the list (wraps including the "Create New" entry)
            if (Raylib.IsKeyPressed(KeyboardKey.Down) && _existingProfiles.Count > 0)
            {
                _audioManager.PlayKeySound();
                _selectedProfileIndex = (_selectedProfileIndex + 1) % (_existingProfiles.Count + 1);
                
                // Adjust scroll offset when selection moves past visible range
                if (_selectedProfileIndex < _existingProfiles.Count && _selectedProfileIndex >= _scrollOffset + _maxVisibleProfiles)
                {
                    _scrollOffset = Math.Min(_selectedProfileIndex - _maxVisibleProfiles + 1, Math.Max(0, _existingProfiles.Count - _maxVisibleProfiles));
                }
                else if (_selectedProfileIndex < _scrollOffset)
                {
                    _scrollOffset = _selectedProfileIndex;
                }
            }
            // Navigate up through the list (wraps as well)
            else if (Raylib.IsKeyPressed(KeyboardKey.Up) && _existingProfiles.Count > 0)
            {
                _audioManager.PlayKeySound();
                _selectedProfileIndex = (_selectedProfileIndex - 1 + _existingProfiles.Count + 1) % (_existingProfiles.Count + 1);
                
                // Adjust scroll offset for upward movement
                if (_selectedProfileIndex < _scrollOffset)
                {
                    _scrollOffset = _selectedProfileIndex;
                }
                else if (_selectedProfileIndex >= _scrollOffset + _maxVisibleProfiles)
                {
                    _scrollOffset = Math.Min(_selectedProfileIndex - _maxVisibleProfiles + 1, Math.Max(0, _existingProfiles.Count - _maxVisibleProfiles));
                }
            }

            // Enter -> select existing profile or switch to create-new mode
            if (Raylib.IsKeyPressed(KeyboardKey.Enter))
            {
                _audioManager.PlaySelectSound();
                if (_selectedProfileIndex < _existingProfiles.Count)
                {
                    // Load existing profile
                    string selectedUsername = _existingProfiles[_selectedProfileIndex];
                    
                    // Prevent selecting the same profile for both players
                    if (_currentPlayer == 2 && _player1Profile != null && _player1Profile.Username == selectedUsername)
                    {
                        return;
                    }
                    
                    PlayerProfile? profile = _profileManager.LoadProfile(selectedUsername);
                    
                    if (profile != null)
                    {
                        if (_currentPlayer == 1)
                        {
                            _player1Profile = profile;
                            _currentPlayer = 2;
                            _selectedProfileIndex = 0;
                        }
                        else
                        {
                            _player2Profile = profile;
                            gameManager.ChangeState(GameState.MainMenu);
                        }
                    }
                }
                else
                {
                    // Switch to new profile creation input
                    _isCreatingNew = true;
                    _newUsername = "";
                }
            }

            // Delete key triggers confirmation flow for the currently selected existing profile
            if (Raylib.IsKeyPressed(KeyboardKey.Delete) && _selectedProfileIndex < _existingProfiles.Count)
            {
                _audioManager.PlaySelectSound();
                _profileToDelete = _existingProfiles[_selectedProfileIndex];
                _isConfirmingDelete = true;
            }

            // Backspace while selecting player 2 cancels back to player 1 selection
            if (Raylib.IsKeyPressed(KeyboardKey.Backspace) && _currentPlayer == 2)
            {
                _audioManager.PlayBackSound();
                _currentPlayer = 1;
                _player1Profile = null;
                _selectedProfileIndex = 0;
                _scrollOffset = 0;
            }
        }

        /// <summary>
        /// Handle keyboard input while creating a new profile (text entry, confirm, cancel).
        /// </summary>
        public void HandleNewProfileInput(GameManager gameManager)
        {
            // Gather typed characters (raw char input)
            int key = Raylib.GetCharPressed();
            while (key > 0)
            {
                // Accept printable ASCII characters up to the configured length
                if ((key >= 32 && key <= 126) && _newUsername.Length < _maxUsernameLength)
                {
                    _newUsername += (char)key;
                }
                key = Raylib.GetCharPressed();
            }

            // Backspace removes last character from the typed username
            if (Raylib.IsKeyPressed(KeyboardKey.Backspace) && _newUsername.Length > 0)
            {
                _audioManager.PlayBackSound();
                _newUsername = _newUsername.Substring(0, _newUsername.Length - 1);
            }

            // Enter confirms username (only if non-empty) and creates the profile
            if (Raylib.IsKeyPressed(KeyboardKey.Enter) && _newUsername.Length > 0)
            {
                _audioManager.PlaySelectSound();
                _audioManager.PlaySelectSound();
                if (_profileManager.ProfileExists(_newUsername))
                {
                    // Username already exists; currently logs to console (could show UI feedback)
                }
                else
                {
                    PlayerProfile newProfile = _profileManager.CreateNewProfile(_newUsername);
                    
                    if (_currentPlayer == 1)
                    {
                        // Assign to player 1 and switch to player 2 selection
                        _player1Profile = newProfile;
                        _currentPlayer = 2;
                        _isCreatingNew = false;
                        _newUsername = "";
                        _selectedProfileIndex = 0;
                        RefreshProfileList();
                    }
                    else
                    {
                        // Assign to player 2 and return to main menu
                        _player2Profile = newProfile;
                        _isCreatingNew = false;
                        RefreshProfileList();
                        gameManager.ChangeState(GameState.MainMenu);
                    }
                }
            }

            // Escape cancels new profile creation
            if (Raylib.IsKeyPressed(KeyboardKey.Escape))
            {
                _isCreatingNew = false;
                _newUsername = "";
            }
        }

        /// <summary>
        /// Draws the full profile screen including particles background and either selection or new-profile UI.
        /// </summary>
        public void Draw()
        {
            // Ensure particles are initialized before drawing
            if (_particleManager.ParticleCount == 0)
            {
                _particleManager.InitializeParticles(1920, 1080, 2500);
            }

            _particleManager.Update(Raylib.GetFrameTime(), 1920, 1080);
            _particleManager.DrawSnow();

            // Dark overlay
            Raylib.DrawRectangle(0, 0, 1920, 1080, new Color(0, 0, 0, 120));

            // Title centered at top
            string title = $"PLAYER {_currentPlayer} - SELECT PROFILE";
            int titleWidth = Raylib.MeasureText(title, 60);
            Raylib.DrawRectangle((1920 - titleWidth) / 2 - 20, 40, titleWidth + 40, 80, new Color(0, 0, 0, 180));
            Raylib.DrawRectangleLines((1920 - titleWidth) / 2 - 20, 40, titleWidth + 40, 80, Color.White);
            Raylib.DrawText(title, (1920 - titleWidth) / 2, 50, 60, Color.White);

            // Draw appropriate UI block
            if (_isCreatingNew)
            {
                DrawNewProfileInput();
            }
            else
            {
                DrawProfileSelection();
            }

            // When selecting for Player 2, show Player 1's chosen profile summary on the left
            if (_currentPlayer == 2 && _player1Profile != null)
            {
                Raylib.DrawRectangle(50, 50, 330, 160, new Color(0, 0, 0, 200));
                Raylib.DrawRectangleLines(50, 50, 330, 160, Color.SkyBlue);
                Raylib.DrawText("PLAYER 1", 70, 60, 22, Color.SkyBlue);
                Raylib.DrawText(_player1Profile.Username, 70, 90, 35, Color.White);
                Raylib.DrawText($"Level {_player1Profile.Level}", 70, 135, 20, Color.LightGray);
                Raylib.DrawText($"XP: {_player1Profile.XP}/{_player1Profile.GetXPForNextLevel()}", 70, 160, 20, Color.LightGray);
                Raylib.DrawText($"Wins: {_player1Profile.GamesWon}/{_player1Profile.GamesPlayed} ({_player1Profile.GetWinRate():F1}%)", 70, 185, 18, Color.LightGray);
            }
        }

        /// <summary>
        /// Draws the list of existing profiles, handles highlighting and "Create New" entry at bottom.
        /// </summary>
        public void DrawProfileSelection()
        {
            // Background box for profile list
            Raylib.DrawRectangle(400, 200, 1120, 600, new Color(0, 0, 0, 180));
            Raylib.DrawRectangleLines(400, 200, 1120, 600, Color.SkyBlue);

            // Header, optionally showing current range when scrolling
            string headerText = "SELECT PROFILE";
            if (_existingProfiles.Count > _maxVisibleProfiles)
            {
                int endIndex = Math.Min(_scrollOffset + _maxVisibleProfiles, _existingProfiles.Count);
                headerText += $" ({_scrollOffset + 1}-{endIndex} of {_existingProfiles.Count})";
            }
            Raylib.DrawText(headerText, 450, 230, 30, Color.SkyBlue);

            // Compute how many entries to display (clamp by remaining items and visible limit)
            int displayCount = Math.Min(_existingProfiles.Count - _scrollOffset, _maxVisibleProfiles);
            for (int i = 0; i < displayCount; i++)
            {
                int profileIndex = i + _scrollOffset;
                string username = _existingProfiles[profileIndex];
                
                // If player 1 already selected this profile, render it as disabled for player 2
                bool isAlreadySelected = _currentPlayer == 2 && _player1Profile != null && _player1Profile.Username == username;
                
                // Color selection: highlight for current selection, gray out if already taken
                Color textColor = profileIndex == _selectedProfileIndex ? Color.Yellow : (isAlreadySelected ? Color.DarkGray : Color.White);
                Color bgColor = profileIndex == _selectedProfileIndex ? new Color(255, 255, 0, 50) : new Color(255, 255, 255, 0);

                if (profileIndex == _selectedProfileIndex)
                {
                    // Highlight background for selected row
                    Raylib.DrawRectangle(430, 275 + (i * 60), 1060, 50, bgColor);
                }

                // Use cached profile data to draw name and statistics
                if (_cachedProfiles.ContainsKey(username))
                {
                    PlayerProfile profile = _cachedProfiles[username];
                    string displayText = isAlreadySelected ? $"{profile.Username} (Selected by P1)" : $"{profile.Username}";
                    
                    // Draw index number
                    Raylib.DrawText($"{profileIndex + 1}.", 430, 285 + (i * 60), 22, textColor);
                    
                    // Draw profile name and summary stats
                    Raylib.DrawText(displayText, 470, 285 + (i * 60), 25, textColor);
                    Raylib.DrawText($"Level {profile.Level} | XP: {profile.XP} | W/L: {profile.GamesWon}/{profile.GamesPlayed} ({profile.GetWinRate():F1}%)", 920, 288 + (i * 60), 18, textColor);
                }
            }

            // Separator and fixed-position "Create New Profile" entry at bottom of the list box
            Raylib.DrawLine(430, 755, 1490, 755, Color.Gray);

            int createNewY = 760; // Fixed position with separator
            bool isCreateNewSelected = _selectedProfileIndex == _existingProfiles.Count;
            Color createColor = isCreateNewSelected ? Color.Yellow : Color.Green;
            Color createBgColor = isCreateNewSelected ? new Color(255, 255, 0, 50) : new Color(255, 255, 255, 0);

            if (isCreateNewSelected)
            {
                Raylib.DrawRectangle(430, createNewY, 1060, 35, createBgColor);
            }

            Raylib.DrawText("+ CREATE NEW PROFILE", 450, createNewY + 7, 25, createColor);

            // Instructions / delete confirmation footer
            Raylib.DrawRectangle(400, 850, 1120, 120, new Color(0, 0, 0, 200));
            Raylib.DrawRectangleLines(400, 850, 1120, 120, Color.Gray);
            
            if (_isConfirmingDelete)
            {
                // Show confirmation prompt when deleting
                Raylib.DrawText($"DELETE PROFILE '{_profileToDelete}'?", 450, 870, 28, Color.Red);
                Raylib.DrawText("Y - Confirm Delete | N - Cancel", 450, 915, 25, Color.White);
            }
            else
            {
                // Show controls reminder
                Raylib.DrawText("ARROW KEYS - Navigate | ENTER - Select | DELETE - Remove Profile", 450, 880, 22, Color.White);
                if (_currentPlayer == 2)
                {
                    Raylib.DrawText("BACKSPACE - Go Back to Player 1", 450, 920, 25, Color.White);
                }
            }
        }

        /// <summary>
        /// Draw UI for entering a new username (text input box, char limit, instructions).
        /// </summary>
        public void DrawNewProfileInput()
        {
            // Input modal background and border
            Raylib.DrawRectangle(460, 350, 1000, 300, new Color(0, 0, 0, 200));
            Raylib.DrawRectangleLines(460, 350, 1000, 300, Color.Yellow);

            Raylib.DrawText("ENTER USERNAME:", 500, 390, 30, Color.Yellow);

            // Text entry field background and border
            Raylib.DrawRectangle(500, 450, 880, 60, new Color(50, 50, 50, 255));
            Raylib.DrawRectangleLines(500, 450, 880, 60, Color.White);
            
            // Show typed username with a caret indicator
            string displayText = _newUsername + "_";
            Raylib.DrawText(displayText, 520, 465, 30, Color.White);

            // Show character limit count
            Raylib.DrawText($"{_newUsername.Length}/{_maxUsernameLength}", 1350, 520, 18, Color.Gray);

            // Instructions for confirming/canceling
            Raylib.DrawText("Type username and press ENTER", 500, 560, 20, Color.LightGray);
            Raylib.DrawText("ESC - Cancel", 500, 590, 20, Color.LightGray);
        }

    }
}
