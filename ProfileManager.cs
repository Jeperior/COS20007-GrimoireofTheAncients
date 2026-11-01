using System;

namespace Grimoire
{
    /// <summary>
    /// Manages saving and loading player profiles.
    /// </summary>
    public class ProfileManager
    {
        private string _profilesDirectory;  // Directory to store profiles
        private string _profileExtension;   // File extension for profile files

        /// <summary>
        /// Initializes a new instance of the ProfileManager class.
        /// </summary>
        public ProfileManager()
        {
            _profilesDirectory = "Profiles";
            _profileExtension = ".json";
            EnsureProfilesDirectoryExists();
        }

        /// <summary>
        /// Ensures the profiles directory exists.
        /// </summary>
        public void EnsureProfilesDirectoryExists()
        {
            if (!Directory.Exists(_profilesDirectory))
            {
                Directory.CreateDirectory(_profilesDirectory);
            }
        }

        /// <summary>
        /// Saves a player profile to disk.
        /// </summary>
        public bool SaveProfile(PlayerProfile profile)
        {
            try
            {
                string filename = GetProfileFilename(profile.Username);
                string json = profile.ToJson();
                File.WriteAllText(filename, json);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProfileManager] Error saving profile: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Loads a player profile from disk.
        /// </summary>
        public PlayerProfile? LoadProfile(string username)
        {
            try
            {
                string filename = GetProfileFilename(username);
                if (!File.Exists(filename))
                {
                    return null;
                }

                string json = File.ReadAllText(filename);
                PlayerProfile? profile = PlayerProfile.FromJson(json);
                
                if (profile != null)
                {
                    Console.WriteLine($"[ProfileManager] Loaded profile: {username} (Level {profile.Level}, XP {profile.XP})");
                }
                
                return profile;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProfileManager] Error loading profile: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Checks if a profile exists.
        /// </summary>
        public bool ProfileExists(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;
                
            string filename = GetProfileFilename(username);
            return File.Exists(filename);
        }

        /// <summary>
        /// Creates a new profile with the given username.
        /// </summary>
        public PlayerProfile CreateNewProfile(string username)
        {
            PlayerProfile profile = new PlayerProfile();
            profile.Username = username;
            SaveProfile(profile);
            return profile;
        }

        /// <summary>
        /// Gets a list of all existing profile usernames.
        /// </summary>
        public List<string> GetAllProfileNames()
        {
            try
            {
                if (!Directory.Exists(_profilesDirectory))
                    return new List<string>();

                string[] files = Directory.GetFiles(_profilesDirectory, $"*{_profileExtension}");
                List<string> profileNames = files
                    .Select(f => Path.GetFileNameWithoutExtension(f))
                    .ToList();
                
                return profileNames;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProfileManager] Error getting profile names: {ex.Message}");
                return new List<string>();
            }
        }

        /// <summary>
        /// Deletes a profile.
        /// </summary>
        public bool DeleteProfile(string username)
        {
            try
            {
                string filename = GetProfileFilename(username);
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ProfileManager] Error deleting profile: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets the filename for a profile.
        /// </summary>
        public string GetProfileFilename(string username)
        {
            // Sanitize username to be safe for filesystem
            string safeUsername = string.Join("_", username.Split(Path.GetInvalidFileNameChars()));
            return Path.Combine(_profilesDirectory, $"{safeUsername}{_profileExtension}");
        }
    }
}
