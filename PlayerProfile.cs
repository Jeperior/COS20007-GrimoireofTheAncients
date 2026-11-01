using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Grimoire
{
    /// <summary>
    /// Represents a player's profile including progress, unlocks, and statistics.
    /// Contains simple XP/level thresholds, unlocked wizards list and basic serialization.
    /// </summary>
    public class PlayerProfile
    {
        // Backing fields for properties
        private string _username;                // Player's display name
        private int _level;                      // Current player level (1..3)
        private int _xp;                         // Total accumulated XP
        private List<int> _unlockedWizards;      // IDs of unlocked wizards
        private int _selectedWizardId;           // Currently selected wizard ID
        private int _gamesWon;                   // Total games won
        private int _gamesPlayed;                // Total games played
        private int _xpForLevel2;                // XP threshold for level 2
        private int _xpForLevel3;                // XP threshold for level 3

        /// <summary>
        /// Default constructor - initializes a new profile with sane defaults.
        /// Starts at level 1 with wizard 0 unlocked.
        /// </summary>
        public PlayerProfile()
        {
            _username = "";
            _level = 1;
            _xp = 0;
            _unlockedWizards = new List<int> { 0 }; // Start with wizard 0 unlocked
            _selectedWizardId = 0;
            _gamesWon = 0;
            _gamesPlayed = 0;

            // Level thresholds (hard-coded here for simplicity)
            _xpForLevel2 = 100;
            _xpForLevel3 = 250;
        }

        // Public properties expose the profile data. These are plain getters/setters
        // so a consumer can modify them directly (could be changed to methods if validation is required).

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        public int Level
        {
            get { return _level; }
            set { _level = value; }
        }

        public int XP
        {
            get { return _xp; }
            set { _xp = value; }
        }

        public List<int> UnlockedWizards
        {
            get { return _unlockedWizards; }
            set { _unlockedWizards = value; }
        }

        public int SelectedWizardId
        {
            get { return _selectedWizardId; }
            set { _selectedWizardId = value; }
        }

        public int GamesWon
        {
            get { return _gamesWon; }
            set { _gamesWon = value; }
        }

        public int GamesPlayed
        {
            get { return _gamesPlayed; }
            set { _gamesPlayed = value; }
        }

        /// <summary>
        /// Adds XP and triggers level update logic.
        /// Use this instead of directly manipulating XP when you want level/locks to be recalculated.
        /// </summary>
        public void AddXP(int amount)
        {
            _xp += amount;
            UpdateLevel();
        }

        /// <summary>
        /// Updates level based on current XP and unlocks wizards when leveling up.
        /// Only supports levels 1..3 with fixed thresholds.
        /// </summary>
        public void UpdateLevel()
        {
            int oldLevel = _level;

            // Promote to level 3 if XP meets the level 3 threshold and player is not already 3
            if (_xp >= _xpForLevel3 && _level < 3)
            {
                _level = 3;
            }
            // Otherwise promote to level 2 if XP meets level 2 threshold and player is not already >=2
            else if (_xp >= _xpForLevel2 && _level < 2)
            {
                _level = 2;
            }

            // If the player leveled up, perform unlocks and logging
            if (_level > oldLevel)
            {
                UnlockWizardsForLevel(_level);
            }
        }

        /// <summary>
        /// Adds wizard unlocks tied to specific levels.
        /// This method centralizes which wizard IDs are unlocked at which levels.
        /// </summary>
        public void UnlockWizardsForLevel(int level)
        {
            if (level == 2 && !_unlockedWizards.Contains(1))
            {
                _unlockedWizards.Add(1);
            }
            else if (level == 3 && !_unlockedWizards.Contains(2))
            {
                _unlockedWizards.Add(2);
            }
        }

        /// <summary>
        /// Records a game result, increments game counters and awards XP.
        /// Use this to track statistics and award XP in one call.
        /// </summary>
        public void RecordGame(bool won, int xpGained)
        {
            _gamesPlayed++;
            if (won)
            {
                _gamesWon++;
            }
            AddXP(xpGained);

            // Log summary for debugging; in production consider using a proper logging framework.
        }

        /// <summary>
        /// Returns the XP threshold required to reach the next level.
        /// If at max level, returns the max level threshold.
        /// </summary>
        public int GetXPForNextLevel()
        {
            if (_level == 1)
                return _xpForLevel2;
            else if (_level == 2)
                return _xpForLevel3;
            else
                return _xpForLevel3; // At max level, next threshold is the same (no further progression)
        }

        /// <summary>
        /// Returns win rate percentage (0-100). If no games played returns 0.
        /// </summary>
        public float GetWinRate()
        {
            if (_gamesPlayed == 0)
                return 0f;
            return ((float)_gamesWon / _gamesPlayed) * 100f;
        }

        /// <summary>
        /// Serializes this profile to a JSON string using System.Text.Json.
        /// Uses pretty-printing for readability.
        /// </summary>
        public string ToJson()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        }

        /// <summary>
        /// Deserializes a PlayerProfile from JSON. Returns null on error and logs the exception message.
        /// </summary>
        public static PlayerProfile? FromJson(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<PlayerProfile>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PlayerProfile] Error deserializing profile: {ex.Message}");
                return null;
            }
        }
    }
}
