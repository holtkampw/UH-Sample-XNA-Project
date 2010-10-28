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
        Selection, Back, Rotation, MenuUp, MenuDown,
        MenuSelect, MenuCancel, TileMoveUp, TileMoveDown,
        TileMoveLeft, TileMoveRight, 
        TowerBuild, TowerDestroy, TowerRepair, TowerUpgrade,
        ExitGame, MenuLeft, MenuRight,
        PlayerMenuLeft, PlayerMenuRight, PlayerMenuUp, PlayerMenuDown,
        UnitBuild, UnitLeft, UnitRight, UnitUp, UnitDown,
        JoinGame, BackToMainMenu, StartGame, TeamUp, TeamDown,
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
        Dictionary<InputAction, List<Keys>> keyActionDictionary;
        Dictionary<InputAction, List<Buttons>> buttonActionDictionary;

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

            keyActionDictionary = new Dictionary<InputAction, List<Keys>>();
            buttonActionDictionary = new Dictionary<InputAction, List<Buttons>>();

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

            if (keyActionDictionary.ContainsKey(action))
            {
                if (!keyActionDictionary[action].Contains(key))
                    keyActionDictionary[action].Add(key);
            }
            else
            {
                List<Keys> keysList = new List<Keys>();
                keysList.Add(key);
                keyActionDictionary.Add(action, keysList);
            }

        }

        /// <summary>
        /// Allows the action to be triggered by the key
        /// </summary>
        /// <param name="action">The input action to be triggered</param>
        /// <param name="button">The button to trigger the action</param>
        public void AddInput(InputAction action, Buttons button)
        {

            if (buttonActionDictionary.ContainsKey(action))
            {
                if (!buttonActionDictionary[action].Contains(button))
                    buttonActionDictionary[action].Add(button);
            }
            else
            {
                List<Buttons> buttonsList = new List<Buttons>();
                buttonsList.Add(button);
                buttonActionDictionary.Add(action, buttonsList);
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
                   for(int j = 0; j<buttonActionDictionary[action].Count; j++)
                        if (IsNewButtonPressed(buttonActionDictionary[action][j], playerIndexes[i]))
                            return true;
            }
            else
            {
                for (int j = 0; j < buttonActionDictionary[action].Count; j++)
                    if (IsNewButtonPressed(buttonActionDictionary[action][j], playerIndex.Value))
                        return true;
            }

           for(int i =0; i<keyActionDictionary[action].Count; i++)
                if (IsNewKeyPressed(keyActionDictionary[action][i]))
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
                    for (int j = 0; j < buttonActionDictionary[action].Count; j++)
                        if (IsButtonPressed(buttonActionDictionary[action][j], playerIndexes[i]))
                            return true;
            }
            else
            {
                for (int j = 0; j < buttonActionDictionary[action].Count; j++)
                    if (IsButtonPressed(buttonActionDictionary[action][j], playerIndex.Value))
                        return true;
            }

            for (int i = 0; i < keyActionDictionary[action].Count; i++)
                if (IsKeyPressed(keyActionDictionary[action][i]))
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

        public bool CheckNewReleaseAction(InputAction action)
        {
            return CheckNewReleaseAction(action, null);
        }

        public bool CheckNewReleaseAction(InputAction action, PlayerIndex? playerIndex)
        {
            if (!playerIndex.HasValue)
            {
                for (int i = 0; i < playerIndexes.Length; i++)
                    for (int j = 0; j < buttonActionDictionary[action].Count; j++)
                        if (IsNewButtonReleased(buttonActionDictionary[action][j], playerIndexes[i]))
                            return true;
            }
            else
            {
                for (int j = 0; j < buttonActionDictionary[action].Count; j++)
                    if (IsNewButtonReleased(buttonActionDictionary[action][j], playerIndex.Value))
                        return true;
            }

            for (int i = 0; i < keyActionDictionary[action].Count; i++)
                if (IsNewKeyReleased(keyActionDictionary[action][i]))
                    return true;

            return false;
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

        private bool IsNewButtonReleased(Buttons button, PlayerIndex playerIndex)
        {
            return IsButtonReleased(button, playerIndex) && previousGamePadStates[playerIndex].IsButtonDown(button);
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

        private bool IsNewKeyReleased(Keys key)
        {
            return IsKeyReleased(key) && previousKeyboardState.IsKeyDown(key);
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
