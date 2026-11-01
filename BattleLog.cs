using System;
using System.Collections.Generic;
using Raylib_cs;

namespace Grimoire
{
    public class BattleLog
    {
        // Battle log to display messages during battle
        private List<string> _messages;     // List of log messages
        private int _maxMessages;           // Maximum number of messages to retain
        private int _scrollOffset;          // Current scroll offset
        private int _visibleLines;          // Number of visible lines in the log
        private int _messageCounter;        // Counter for messages

        /// <summary>
        /// Initializes a new instance of the BattleLog class.
        /// </summary>
        /// <param name="maxMessages"></param>
        /// <param name="visibleLines"></param> <summary>
        /// 
        /// </summary>
        /// <param name="maxMessages"></param>
        /// <param name="visibleLines"></param>
        public BattleLog(int maxMessages, int visibleLines)
        {
            _messages = new List<string>();
            _maxMessages = maxMessages;
            _scrollOffset = 0;
            _visibleLines = visibleLines;
            _messageCounter = 0;
        }

        /// <summary>
        /// Adds a new message to the battle log.
        /// </summary>
        /// <param name="message"></param>
        public void AddMessage(string message)
        {
            _messageCounter++;
            _messages.Add($"[{_messageCounter}] {message}");
            
            // Keep only the last maxMessages
            if (_messages.Count > _maxMessages)
            {
                _messages.RemoveAt(0);
                if (_scrollOffset > 0)
                {
                    _scrollOffset--;
                }
            }

            // Auto-scroll to bottom when new message arrives
            _scrollOffset = Math.Max(0, _messages.Count - _visibleLines);
        }

        /// <summary>
        /// Clears all messages from the battle log.
        /// </summary>
        public void Clear()
        {
            _messages.Clear();
            _scrollOffset = 0;
            _messageCounter = 0; // Reset counter when clearing
        }

        /// <summary>
        /// Scrolls the battle log up by one line.
        /// </summary> <summary>
        /// 
        /// </summary>
        public void ScrollUp()
        {
            if (_scrollOffset > 0)
            {
                _scrollOffset--;
            }
        }

        /// <summary>
        /// Scrolls the battle log down by one line.
        /// </summary> <summary>
        /// 
        /// </summary>
        public void ScrollDown()
        {
            int maxScroll = Math.Max(0, _messages.Count - _visibleLines);
            if (_scrollOffset < maxScroll)
            {
                _scrollOffset++;
            }
        }

        /// <summary>
        /// Draws the battle log on the screen.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="fontSize"></param> <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="fontSize"></param>
        public void Draw(int x, int y, int width, int height, int fontSize)
        {
            // Draw background
            Raylib.DrawRectangle(x, y, width, height, new Color(0, 0, 0, 180));
            Raylib.DrawRectangleLines(x, y, width, height, Color.Gray);
            
            // Draw title
            Raylib.DrawText("BATTLE LOG:", x + 20, y + 10, 20, Color.Yellow);
            
            // Draw scroll indicators if needed
            if (_scrollOffset > 0)
            {
                Raylib.DrawText("^", x + width - 40, y + 35, 20, Color.White);
            }
            
            int maxScroll = Math.Max(0, _messages.Count - _visibleLines);
            if (_scrollOffset < maxScroll)
            {
                Raylib.DrawText("v", x + width - 40, y + height - 30, 20, Color.White);
            }

            // Draw messages
            int messageY = y + 40;
            int lineHeight = fontSize + 5;
            
            for (int i = 0; i < _visibleLines && (_scrollOffset + i) < _messages.Count; i++)
            {
                string message = _messages[_scrollOffset + i];
                Color color = GetMessageColor(message);
                Raylib.DrawText(message, x + 20, messageY + (i * lineHeight), fontSize, color);
            }

            // Draw scroll position indicator
            if (_messages.Count > _visibleLines)
            {
                Raylib.DrawText($"({_scrollOffset + 1}-{Math.Min(_scrollOffset + _visibleLines, _messages.Count)}/{_messages.Count})", 
                    x + width - 150, y + 10, 16, Color.LightGray);
            }
        }

        /// <summary>
        /// Determines the color of a message based on its content.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns> <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Color GetMessageColor(string message)
        {
            if (message.Contains("regenerates") || message.Contains("heals"))
                return Color.Green;
            else if (message.Contains("burn damage"))
                return Color.Orange;
            else if (message.Contains("poison damage"))
                return Color.Purple;
            else if (message.Contains("paralyzed"))
                return Color.Yellow;
            else if (message.Contains("damage") || message.Contains("takes"))
                return Color.Red;
            else if (message.Contains("cast"))
                return Color.SkyBlue;
            else
                return Color.White;
        }
    }
}
