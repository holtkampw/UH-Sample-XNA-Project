#region Using Statements
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#endregion

namespace UHSampleGame.InputManagement
{
    public enum InputAction
    {
        RotateLeft, RotateRight, RotateUp, RotateDown,
        StrafeLeft, StrafeRight, StrafeUp, StrafeDown,
        Selection, Back, Rotation, MenuUp, MenuDown, 
        MenuSelect, MenuCancel, TileMoveUp, TileMoveDown,
        TileMoveLeft, TileMoveRight, TowerBuild
    };

    public class InputManager
    {
        #region Class Variables
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;
        MouseState previousMouseState;
        MouseState currentMouseState;
        Dictionary<PlayerIndex, GamePadState> currentGamePadStates;
        Dictionary<PlayerIndex, GamePadState> previousGamePadStates;
        Dictionary<InputAction, Keys> items;
        Dictionary<InputAction, List<object>> actionDictionary;
        PlayerIndex[] playerIndexes;
        #endregion

        #region Initialization
        public InputManager()
        {
            previousKeyboardState = new KeyboardState();
            currentKeyboardState = new KeyboardState();
            previousMouseState = new MouseState();
            currentMouseState = new MouseState();
            currentGamePadStates = new Dictionary<PlayerIndex, GamePadState>();
            previousGamePadStates = new Dictionary<PlayerIndex, GamePadState>();
            items = new Dictionary<InputAction, Keys>();
            actionDictionary = new Dictionary<InputAction, List<object>>();
            playerIndexes = new PlayerIndex[4];
            playerIndexes[0] = PlayerIndex.One;
            playerIndexes[1] = PlayerIndex.Two;
            playerIndexes[2] = PlayerIndex.Three;
            playerIndexes[3] = PlayerIndex.Four;

            for (int i = 0; i < playerIndexes.Length; i++)
            {
                currentGamePadStates.Add(playerIndexes[i], new GamePadState());
                previousGamePadStates.Add(playerIndexes[i], new GamePadState());
            }
        }
        #endregion

        #region Update
        /// <summary>
        /// Updates previous and current input devices
        /// </summary>
        public void Update()
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            for (int i = 0; i < playerIndexes.Length; i++)
            {
                previousGamePadStates[playerIndexes[i]] = currentGamePadStates[playerIndexes[i]];
                currentGamePadStates[playerIndexes[i]] = GamePad.GetState(playerIndexes[i]);
            }
        }
        #endregion

        #region Helper Functions
        /// <summary>
        /// Allows the action to be triggered by the key
        /// </summary>
        /// <param name="action">The input action to be triggered</param>
        /// <param name="key">The key to trigger the action</param>
        public void AddInput(InputAction action, Keys key)
        {
            if (actionDictionary.ContainsKey(action))
            {
                if (!actionDictionary[action].Contains(key))
                    actionDictionary[action].Add(key);
            }
            else
            {
                List<object> newList = new List<object>();
                newList.Add(key);
                actionDictionary.Add(action, newList);
            }
        }

        /// <summary>
        /// Allows the action to be triggered by the key
        /// </summary>
        /// <param name="action">The input action to be triggered</param>
        /// <param name="button">The button to trigger the action</param>
        public void AddInput(InputAction action, Buttons button)
        {
            if (actionDictionary.ContainsKey(action))
            {
                if (!actionDictionary[action].Contains(button))
                    actionDictionary[action].Add(button);
            }
            else
            {
                List<object> newList = new List<object>();
                newList.Add(button);
                actionDictionary.Add(action, newList);
            }
        }

        /// <summary>
        /// Checks if the action has just been triggered
        /// </summary>
        /// <param name="action">The action to check</param>
        /// <returns>Returns true if the action has just been triggered</returns>
        public bool CheckNewAction(InputAction action)
        {
            return CheckNewAction(action, null);
        }

        /// <summary>
        /// Checks if the action has just been triggered
        /// </summary>
        /// <param name="action">The action to check</param>
        /// <param name="playerIndex">The player index to check</param>
        /// <returns>Returns true if the action has just been triggered</returns>
        public bool CheckNewAction(InputAction action, PlayerIndex? playerIndex)
        {
            if (!playerIndex.HasValue)
            {
                for (int i = 0; i < playerIndexes.Length; i++)
                    foreach (Buttons button in actionDictionary[action])
                        if (IsNewButtonPressed(button, playerIndexes[i]))
                            return true;
            }
            else
            {
                foreach (Buttons button in actionDictionary[action])
                    if (IsNewButtonPressed(button, playerIndex.Value))
                        return true;
            }

            foreach (Keys key in actionDictionary[action])
                if (IsNewKeyPressed(key))
                    return true;

            return false;
        }

        /// <summary>
        /// Checks if the action has is triggered
        /// </summary>
        /// <param name="action">The action to check</param>
        /// <returns>Returns true if the action is triggered</returns>
        public bool CheckAction(InputAction action)
        {
            return CheckAction(action, null);
        }

        /// <summary>
        /// Checks if the action has is triggered
        /// </summary>
        /// <param name="action">The action to check</param>
        /// <param name="playerIndex">The player index to check</param>
        /// <returns>Returns true if the action is triggered</returns>
        public bool CheckAction(InputAction action, PlayerIndex? playerIndex)
        {
            if (!playerIndex.HasValue)
            {
                for (int i = 0; i < playerIndexes.Length; i++)
                    foreach (Buttons button in actionDictionary[action])
                        if (IsButtonPressed(button, playerIndexes[i]))
                            return true;
            }
            else
            {
                foreach (Buttons button in actionDictionary[action])
                    if (IsButtonPressed(button, playerIndex.Value))
                        return true;
            }

            foreach (Keys key in actionDictionary[action])
                if (IsKeyPressed(key))
                    return true;

            return false;
        }

        /// <summary>
        /// Checks if the action has just been released
        /// </summary>
        /// <param name="action">The action to check</param>
        /// <returns>Returns true if the aciton has just been released</returns>
        public bool CheckReleaseAction(InputAction action)
        {
            return CheckReleaseAction(action, null);
        }

        /// <summary>
        /// Checks if the action has just been released
        /// </summary>
        /// <param name="action">The action to check</param>
        /// <param name="playerIndex">The player index to check</param>
        /// <returns>Returns true if the aciton has just been released</returns>
        public bool CheckReleaseAction(InputAction action, PlayerIndex? playerIndex)
        {
            return true;
        }

        #region ButtonHelpers
        private bool IsNewButtonPressed(Buttons button, PlayerIndex playerIndex)
        {
            return IsButtonPressed(button, playerIndex) && previousGamePadStates[playerIndex].IsButtonUp(button);
        }

        private bool IsButtonPressed(Buttons button, PlayerIndex playerIndex)
        {
            return currentGamePadStates[playerIndex].IsButtonDown(button);
        }

        private bool IsButtonReleased(Buttons button, PlayerIndex playerIndex)
        {
            return !IsButtonPressed(button, playerIndex) && previousGamePadStates[playerIndex].IsButtonDown(button);
        }
        #endregion ButtonHelpers

        #region KeyHelpers
        private bool IsNewKeyPressed(Keys key)
        {
            return IsKeyPressed(key) && previousKeyboardState.IsKeyUp(key);
        }

        private bool IsKeyPressed(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key);
        }

        private bool IsKeyReleased(Keys key)
        {
            return !IsKeyPressed(key) && previousKeyboardState.IsKeyDown(key);
        }
        #endregion KeyHelpers


        
        #endregion
    }
}
