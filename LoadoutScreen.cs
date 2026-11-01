using System;
using Raylib_cs;

namespace Grimoire
{
    /// <summary>
    /// Handles the loadout selection screen where players choose their spell loadouts.
    /// </summary>
    public class LoadoutScreen
    {
        
        private int _currentPlayer;                 // Which player is currently selecting (1 or 2)
        private int _selectedSpellIndex;            // Index of the currently highlighted spell in the available list
        private Spellbook? _player1Spellbook;       // Player 1's spellbook (nullable until Initialize is called)
        private Spellbook? _player2Spellbook;       // Player 2's spellbook (nullable until Initialize is called)
        private ParticleManager _particleManager;   // Particle manager used for background visual effects
        private AudioManager _audioManager;         // Audio manager used for UI sound effects

        /// <summary>
        /// Initializes a new instance of the LoadoutScreen class.
        /// </summary>
        /// <param name="particleManager">Particle manager for background effects</param>
        /// <param name="audioManager">Audio manager for UI sounds</param>
        public LoadoutScreen(ParticleManager particleManager, AudioManager audioManager)
        {
            _particleManager = particleManager;
            _audioManager = audioManager;

            // Default to player 1 and first spell selected
            _currentPlayer = 1;
            _selectedSpellIndex = 0;
        }

        /// <summary>
        /// Initializes the loadout screen with the players' spellbooks.
        /// Must be called before Update/Draw.
        /// </summary>
        /// <param name="player1Spellbook">Spellbook for player 1</param>
        /// <param name="player2Spellbook">Spellbook for player 2</param>
        public void Initialize(Spellbook player1Spellbook, Spellbook player2Spellbook)
        {
            // Store references to both players' spellbooks
            _player1Spellbook = player1Spellbook;
            _player2Spellbook = player2Spellbook;

            // Always start with player 1 and reset selection index
            _currentPlayer = 1;
            _selectedSpellIndex = 0;
        }

        /// <summary>
        /// Updates the loadout screen based on player input.
        /// Handles navigation, equipping/unequipping, and confirmation.
        /// </summary>
        /// <param name="gameManager">Reference to the game manager to change state</param>
        public void Update(GameManager gameManager)
        {
            // If not initialized yet, do nothing
            if (_player1Spellbook == null || _player2Spellbook == null) return;

            // Get the currently active spellbook (player 1 or player 2)
            Spellbook currentSpellbook = _currentPlayer == 1 ? _player1Spellbook : _player2Spellbook;

            // Navigate down through available spells
            if (Raylib.IsKeyPressed(KeyboardKey.Down))
            {
                _audioManager.PlayKeySound();
                // Wrap around using modulo
                _selectedSpellIndex = (_selectedSpellIndex + 1) % currentSpellbook.Spells.Count;
            }
            // Navigate up through available spells
            else if (Raylib.IsKeyPressed(KeyboardKey.Up))
            {
                _audioManager.PlayKeySound();
                // Add Count before modulo to avoid negative values
                _selectedSpellIndex = (_selectedSpellIndex - 1 + currentSpellbook.Spells.Count) % currentSpellbook.Spells.Count;
            }

            // Equip the currently selected spell into the loadout
            if (Raylib.IsKeyPressed(KeyboardKey.Space))
            {
                _audioManager.PlaySelectSound();
                currentSpellbook.EquipSpell(currentSpellbook.Spells[_selectedSpellIndex]);
            }

            // Remove the last equipped spell from the loadout
            if (Raylib.IsKeyPressed(KeyboardKey.Backspace) && currentSpellbook.SpellLoadout.Count > 0)
            {
                _audioManager.PlayBackSound();
                currentSpellbook.UnequipSpell(currentSpellbook.SpellLoadout.Count - 1);
            }

            // Confirm the loadout when 4 spells are equipped
            if (Raylib.IsKeyPressed(KeyboardKey.Enter) && currentSpellbook.SpellLoadout.Count == 4)
            {
                _audioManager.PlaySelectSound();

                if (_currentPlayer == 1)
                {
                    // Switch to player 2 selection phase
                    _currentPlayer = 2;
                    _selectedSpellIndex = 0; // reset index for second player
                }
                else
                {
                    // Both players finished: proceed to next game state
                    gameManager.ChangeState(GameState.CoinFlip);
                }
            }
        }

        /// <summary>
        /// Draws the loadout screen UI.
        /// Renders background particles, available spells, descriptions, and current loadout.
        /// </summary>
        public void Draw()
        {
            // If not initialized yet, do nothing
            if (_player1Spellbook == null || _player2Spellbook == null) return;

            // Ensure particles are initialized once (heavy allocation)
            if (_particleManager.ParticleCount == 0)
            {
                _particleManager.InitializeParticles(1920, 1080, 2500);
            }

            // Update and draw particle background
            _particleManager.Update(Raylib.GetFrameTime(), 1920, 1080);
            _particleManager.DrawSnow();

            // Semi-transparent overlay to darken background
            Raylib.DrawRectangle(0, 0, 1920, 1080, new Color(0, 0, 0, 120));

            // Determine which player's UI to draw
            Spellbook currentSpellbook = _currentPlayer == 1 ? _player1Spellbook : _player2Spellbook;

            // Title centered across the top
            string title = $"PLAYER {_currentPlayer} - SELECT LOADOUT";
            int titleWidth = Raylib.MeasureText(title, 60);
            Raylib.DrawRectangle((1920 - titleWidth) / 2 - 20, 40, titleWidth + 40, 80, new Color(0, 0, 0, 180));
            Raylib.DrawRectangleLines((1920 - titleWidth) / 2 - 20, 40, titleWidth + 40, 80, Color.White);
            Raylib.DrawText(title, (1920 - titleWidth) / 2, 50, 60, Color.White);

            // Left panel: available spells list
            Raylib.DrawRectangle(50, 160, 800, 450, new Color(0, 0, 0, 180));
            Raylib.DrawRectangleLines(50, 160, 800, 450, Color.SkyBlue);

            Raylib.DrawText("AVAILABLE SPELLS:", 100, 190, 30, Color.SkyBlue);
            for (int i = 0; i < currentSpellbook.Spells.Count; i++)
            {
                // Highlight currently selected item
                Color textColor = i == _selectedSpellIndex ? Color.Yellow : Color.White;
                Color bgColor = i == _selectedSpellIndex ? new Color(255, 255, 0, 50) : new Color(255, 255, 255, 0);

                if (i == _selectedSpellIndex)
                {
                    // Draw a subtle highlighted background for the selected spell
                    Raylib.DrawRectangle(80, 235 + (i * 50), 740, 40, bgColor);
                }

                // Draw the spell name with index
                Raylib.DrawText($"{i + 1}. {currentSpellbook.Spells[i].Name}", 100, 240 + (i * 50), 25, textColor);
            }

            // Description box for the currently selected spell
            if (currentSpellbook.Spells.Count > 0)
            {
                Raylib.DrawRectangle(50, 630, 800, 80, new Color(0, 0, 0, 200));
                Raylib.DrawRectangleLines(50, 630, 800, 80, Color.Gray);
                Raylib.DrawText("Description:", 70, 640, 20, Color.LightGray);
                Raylib.DrawText(currentSpellbook.Spells[_selectedSpellIndex].Description, 70, 670, 18, Color.White);
            }

            // Right panel: currently equipped loadout for the active player
            Raylib.DrawRectangle(950, 160, 900, 450, new Color(0, 0, 0, 180));
            Raylib.DrawRectangleLines(950, 160, 900, 450, Color.Yellow);

            Raylib.DrawText($"PLAYER {_currentPlayer} LOADOUT ({currentSpellbook.SpellLoadout.Count}/4):", 1000, 190, 30, Color.Yellow);

            // List the equipped spells in order
            for (int i = 0; i < currentSpellbook.SpellLoadout.Count; i++)
            {
                Raylib.DrawText($"{i + 1}. {currentSpellbook.SpellLoadout[i].Name}", 1000, 240 + (i * 50), 25, Color.White);
            }

            // Bottom instruction bar
            Raylib.DrawRectangle(50, 870, 1820, 100, new Color(0, 0, 0, 180));
            Raylib.DrawRectangleLines(50, 870, 1820, 100, Color.Gray);

            Raylib.DrawText("ARROW KEYS - Navigate | SPACE - Add Spell | BACKSPACE - Remove Last", 100, 900, 20, Color.LightGray);
            Raylib.DrawText("ENTER - Confirm Loadout", 100, 930, 20, Color.LightGray);

            // If loadout is full, show a confirmation prompt on the right
            if (currentSpellbook.SpellLoadout.Count == 4)
            {
                Raylib.DrawRectangle(980, 430, 835, 40, new Color(0, 128, 0, 150));
                Raylib.DrawText("LOADOUT COMPLETE - Press ENTER", 1000, 437, 25, Color.Green);
            }
        }
    }
}
