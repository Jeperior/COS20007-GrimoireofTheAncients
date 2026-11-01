using System;
using Raylib_cs;

namespace Grimoire
{
    /// <summary>
    /// Handles the coin flip screen UI and logic.
    /// Displays an animated magical circle for the coin flip phase.
    /// </summary>
    public class CoinFlipScreen
    {
        private bool _coinFlipped;          // Has the coin been flipped
        private float _coinFlipTimer;       // Timer for coin flip result display
        private string _coinFlipMessage;    // Message showing coin flip result
        private float _coinFlipRotation;    // Rotation for magical circle effect
        private float _coinFlipPulse;       // Pulse animation timer
        private bool _battleStarted;        // Has the battle started
        private int _firstTurn;             // Who goes first (1 or 2)
        
        /// <summary>
        /// Initializes a new instance of the CoinFlipScreen class.
        /// </summary>
        public CoinFlipScreen()
        {           
            _coinFlipped = false;
            _coinFlipTimer = 0f;
            _coinFlipMessage = "";
            _coinFlipRotation = 0f;
            _coinFlipPulse = 0f;
            _firstTurn = 1;
            _battleStarted = false;
        }

        // Properties
        public bool BattleStarted 
        { 
            get { return _battleStarted; }
            set { _battleStarted = value; }
        }
        // Properties
        public int FirstTurn 
        { 
            get { return _firstTurn; }
            set { _firstTurn = value; }
        }

        /// <summary>
        /// Resets the coin flip screen state.
        /// </summary>
        public void Reset()
        {
            _coinFlipped = false;
            _coinFlipTimer = 0f;
            _coinFlipMessage = "";
            _coinFlipRotation = 0f;
            _coinFlipPulse = 0f;
            BattleStarted = false;
        }

        /// <summary>
        /// Updates the coin flip screen logic.
        /// </summary>
        /// <param name="gameManager"></param>
        public void Update(GameManager gameManager)
        {
            // Update magical circle rotation and pulse
            _coinFlipRotation += 50f * Raylib.GetFrameTime();
            _coinFlipPulse += Raylib.GetFrameTime() * 3f;
             
            if (!_coinFlipped)
            {
                if (Raylib.IsKeyPressed(KeyboardKey.Space))
                {
                    // Flip the coin
                    CoinSide result = gameManager.CoinFlipManager.Flip();
                    _coinFlipped = true;
                    
                    // Determine who gets first turn based on result
                    bool player1Wins = gameManager.CoinFlipManager.PlayerChoiceResult();
                    FirstTurn = player1Wins ? 1 : 2;
                    
                    _coinFlipMessage = $"Result: {result}! Player {FirstTurn} goes first!";
                    Console.WriteLine(_coinFlipMessage);
                }
            }
            else
            {
                // Timer to auto-transition to battle
                _coinFlipTimer += Raylib.GetFrameTime();
                
                if (_coinFlipTimer >= 3.0f)
                {
                    BattleStarted = true;
                    gameManager.ChangeState(GameState.Battle);
                }
            }
        }

        /// <summary>
        /// Draws the coin flip screen UI.
        /// </summary>
        public void Draw()
        {
            // Draw title
            string titleText = "FATE DECIDER";
            int titleWidth = Raylib.MeasureText(titleText, 60);
            Raylib.DrawText(titleText, (1920 - titleWidth) / 2, 250, 60, Color.Yellow);
            
            // Draw magical circle effect
            int centerX = 1920 / 2;
            int centerY = 450;
            
            // Pulsing effect
            float pulseScale = 1f + (float)Math.Sin(_coinFlipPulse) * 0.1f;
            
            // Draw outer rotating circle
            int outerRadius = (int)(120 * pulseScale);
            Raylib.DrawCircleLines(centerX, centerY, outerRadius, new Color(255, 215, 0, 150));
            Raylib.DrawCircleLines(centerX, centerY, outerRadius - 2, new Color(255, 215, 0, 100));
            
            // Draw rotating stars/points around the circle
            for (int i = 0; i < 8; i++)
            {
                float angle = (_coinFlipRotation + i * 45f) * (float)Math.PI / 180f;
                int starX = centerX + (int)(outerRadius * Math.Cos(angle));
                int starY = centerY + (int)(outerRadius * Math.Sin(angle));
                Raylib.DrawCircle(starX, starY, 6, Color.Gold);
                Raylib.DrawCircle(starX, starY, 4, Color.Yellow);
            }
            
            // Draw inner rotating circle (opposite direction)
            int innerRadius = (int)(80 * pulseScale);
            Raylib.DrawCircleLines(centerX, centerY, innerRadius, new Color(255, 255, 255, 180));
            
            // Draw rotating runes/symbols
            for (int i = 0; i < 6; i++)
            {
                float angle = (-_coinFlipRotation * 1.5f + i * 60f) * (float)Math.PI / 180f;
                int runeX = centerX + (int)(innerRadius * Math.Cos(angle));
                int runeY = centerY + (int)(innerRadius * Math.Sin(angle));
                Raylib.DrawCircle(runeX, runeY, 5, new Color(100, 200, 255, 200));
            }
            
            // Draw center glow
            Raylib.DrawCircleGradient(centerX, centerY, (int)(40 * pulseScale), 
                new Color(255, 255, 200, 150), new Color(255, 215, 0, 0));
            
            // Draw center lightning bolt or result symbol
            if (!_coinFlipped)
            {
                string symbol = "X";
                int symbolSize = 80;
                int symbolWidth = Raylib.MeasureText(symbol, symbolSize);
                Raylib.DrawText(symbol, centerX - symbolWidth / 2, centerY - symbolSize / 2, symbolSize, Color.White);
            }
            else
            {
                // Draw result icon (checkmark based on who won)
                string symbol = "V";
                int symbolSize = 80;
                int symbolWidth = Raylib.MeasureText(symbol, symbolSize);
                Raylib.DrawText(symbol, centerX - symbolWidth / 2, centerY - symbolSize / 2, symbolSize, Color.Lime);
            }
            
            if (!_coinFlipped)
            {
                // Instructions
                string instructionText = "Press SPACE to determine fate!";
                int instructionWidth = Raylib.MeasureText(instructionText, 30);
                Raylib.DrawText(instructionText, (1920 - instructionWidth) / 2, 650, 30, Color.White);
            }
            else
            {
                // Show result
                int messageWidth = Raylib.MeasureText(_coinFlipMessage, 40);
                Raylib.DrawText(_coinFlipMessage, (1920 - messageWidth) / 2, 650, 40, Color.Green);
                
                // Show countdown
                int timeLeft = (int)(3.0f - _coinFlipTimer);
                string countdownText = $"Battle starts in {timeLeft + 1}...";
                int countdownWidth = Raylib.MeasureText(countdownText, 30);
                Raylib.DrawText(countdownText, (1920 - countdownWidth) / 2, 700, 30, Color.Yellow);
            }
        }
    }
}
