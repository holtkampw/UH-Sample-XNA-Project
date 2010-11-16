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
        HUD, PickEnemyTarget, TowerInformation, 
        Pause, PowerActivate
    };

    public class InputManager
    {
        #region Class Variables
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;
       /* Dictionary<PlayerIndex, GamePadState> currentGamePadStates;
        Dictionary<PlayerIndex, GamePadState> previousGamePadStates;
        Dictionary<InputAction, List<Keys>> keyActionDictionary;
        Dictionary<InputAction, List<Buttons>> buttonActionDictionary;
        */
        List<GamePadState> currGamePadStates;
        List<GamePadState> prevGamePadStates;
        List<List<Keys>> keyActionList;
        List<List<Buttons>> buttonActionList;

        Enum[] inputActionsArray = EnumHelper.EnumToArray(new InputAction());

        PlayerIndex[] playerIndexes;
        #endregion

        #region Initialization
        public InputManager()
        {
            previousKeyboardState = new KeyboardState();
            currentKeyboardState = new KeyboardState();
            //currentGamePadStates = new Dictionary<PlayerIndex, GamePadState>();
            //previousGamePadStates = new Dictionary<PlayerIndex, GamePadState>();

            //keyActionDictionary = new Dictionary<InputAction, List<Keys>>();
            //buttonActionDictionary = new Dictionary<InputAction, List<Buttons>>();

            playerIndexes = new PlayerIndex[4];
            playerIndexes[0] = PlayerIndex.One;
            playerIndexes[1] = PlayerIndex.Two;
            playerIndexes[2] = PlayerIndex.Three;
            playerIndexes[3] = PlayerIndex.Four;

            //for (int i = 0; i < playerIndexes.Length; i++)
            //{
            //    currentGamePadStates.Add(playerIndexes[i], new GamePadState());
            //    previousGamePadStates.Add(playerIndexes[i], new GamePadState());
            //}


            //New AwesomeNess
            currGamePadStates = new List<GamePadState>();
            prevGamePadStates = new List<GamePadState>();
            keyActionList = new List<List<Keys>>();
            buttonActionList = new List<List<Buttons>>();

            for (int i = 0; i < inputActionsArray.Length; i++)
            {
                keyActionList.Add(new List<Keys>());
                buttonActionList.Add(new List<Buttons>());
            }

            for (int i = 0; i < 4; i++)
            {
                currGamePadStates.Add(new GamePadState());
                prevGamePadStates.Add(new GamePadState());
            }
        }
        #endregion

        #region Update
        /// <summary>
        /// Updates previous and current input devices
        /// </summary>
        public void Update()
        {
            //GamePad.GetState(PlayerIndex.One).Buttons.
#if !XBOX
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
#else
            for (int i = 0; i < playerIndexes.Length; i++)
            {
                //previousGamePadStates[playerIndexes[i]] = currentGamePadStates[playerIndexes[i]];
                //currentGamePadStates[playerIndexes[i]] = GamePad.GetState(playerIndexes[i]);

                prevGamePadStates[i] = currGamePadStates[i];
                currGamePadStates[i] = GamePad.GetState((PlayerIndex)i);
            }
#endif
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

            //if (keyActionDictionary.ContainsKey(action))
            //{
            //    if (!keyActionDictionary[action].Contains(key))
            //        keyActionDictionary[action].Add(key);
            //}
            //else
            //{
            //    List<Keys> keysList = new List<Keys>();
            //    keysList.Add(key);
            //    keyActionDictionary.Add(action, keysList);
            //}

            if (keyActionList[(int)action].Count > 0)
            {
                if (!keyActionList[(int)action].Contains(key))
                    keyActionList[(int)action].Add(key);
            }
            else
            {
                keyActionList[(int)action].Add(key);
            }

        }

        /// <summary>
        /// Allows the action to be triggered by the key
        /// </summary>
        /// <param name="action">The input action to be triggered</param>
        /// <param name="button">The button to trigger the action</param>
        public void AddInput(InputAction action, Buttons button)
        {

            //if (buttonActionDictionary.ContainsKey(action))
            //{
            //    if (!buttonActionDictionary[action].Contains(button))
            //        buttonActionDictionary[action].Add(button);
            //}
            //else
            //{
            //    List<Buttons> buttonsList = new List<Buttons>();
            //    buttonsList.Add(button);
            //    buttonActionDictionary.Add(action, buttonsList);
            //}

            if (buttonActionList[(int)action].Count > 0)
            {
                if (!buttonActionList[(int)action].Contains(button))
                    buttonActionList[(int)action].Add(button);
            }
            else
            {
                buttonActionList[(int)action].Add(button);
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
//#if XBOX
//            if (!playerIndex.HasValue)
//            {
//                for (int i = 0; i < playerIndexes.Length; i++)
//                   for(int j = 0; j<buttonActionDictionary[action].Count; j++)
//                        if (IsNewButtonPressed(buttonActionDictionary[action][j], playerIndexes[i]))
//                            return true;
//            }
//            else
//            {
//                for (int j = 0; j < buttonActionDictionary[action].Count; j++)
//                    if (IsNewButtonPressed(buttonActionDictionary[action][j], playerIndex.Value))
//                        return true;
//            }
//#else
//           for(int i =0; i<keyActionDictionary[action].Count; i++)
//                if (IsNewKeyPressed(keyActionDictionary[action][i]))
//                    return true;
//#endif          
//            return false;

#if XBOX
            if (!playerIndex.HasValue)
            {
                for (int i = 0; i < playerIndexes.Length; i++)
                    for (int j = 0; j < buttonActionList[(int)action].Count; j++)
                        if (IsNewButtonPressed(buttonActionList[(int)action][j], playerIndexes[i]))
                            return true;
            }
            else
            {
                for (int j = 0; j < buttonActionList[(int)action].Count; j++)
                    if (IsNewButtonPressed(buttonActionList[(int)action][j], playerIndex.Value))
                        return true;
            }
#else
           for(int i =0; i<keyActionList[(int)action].Count; i++)
                if (IsNewKeyPressed(keyActionList[(int)action][i]))
                    return true;
#endif
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
//#if XBOX
//            if (!playerIndex.HasValue)
//            {
//                for (int i = 0; i < playerIndexes.Length; i++)
//                    for (int j = 0; j < buttonActionDictionary[action].Count; j++)
//                        if (IsButtonPressed(buttonActionDictionary[action][j], playerIndexes[i]))
//                            return true;
//            }
//            else
//            {
//                for (int j = 0; j < buttonActionDictionary[action].Count; j++)
//                    if (IsButtonPressed(buttonActionDictionary[action][j], playerIndex.Value))
//                        return true;
//            }

//#else
//            for (int i = 0; i < keyActionDictionary[action].Count; i++)
//                if (IsKeyPressed(keyActionDictionary[action][i]))
//                    return true;
//#endif
//            return false;

#if XBOX
            if (!playerIndex.HasValue)
            {
                for (int i = 0; i < playerIndexes.Length; i++)
                    for (int j = 0; j < buttonActionList[(int)action].Count; j++)
                        if (IsButtonPressed(buttonActionList[(int)action][j], playerIndexes[i]))
                            return true;
            }
            else
            {
                for (int j = 0; j < buttonActionList[(int)action].Count; j++)
                    if (IsButtonPressed(buttonActionList[(int)action][j], playerIndex.Value))
                        return true;
            }

#else
            for (int i = 0; i < keyActionList[(int)action].Count; i++)
                if (IsKeyPressed(keyActionList[(int)action][i]))
                    return true;
#endif
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
//#if XBOX
//            if (!playerIndex.HasValue)
//            {
//                for (int i = 0; i < playerIndexes.Length; i++)
//                    for (int j = 0; j < buttonActionDictionary[action].Count; j++)
//                        if (IsNewButtonReleased(buttonActionDictionary[action][j], playerIndexes[i]))
//                            return true;
//            }
//            else
//            {
//                for (int j = 0; j < buttonActionDictionary[action].Count; j++)
//                    if (IsNewButtonReleased(buttonActionDictionary[action][j], playerIndex.Value))
//                        return true;
//            }
//#else
//            for (int i = 0; i < keyActionDictionary[action].Count; i++)
//                if (IsNewKeyReleased(keyActionDictionary[action][i]))
//                    return true;
//#endif
//            return false;
#if XBOX
            if (!playerIndex.HasValue)
            {
                for (int i = 0; i < playerIndexes.Length; i++)
                    for (int j = 0; j < buttonActionList[(int)action].Count; j++)
                        if (IsNewButtonReleased(buttonActionList[(int)action][j], playerIndexes[i]))
                            return true;
            }
            else
            {
                for (int j = 0; j < buttonActionList[(int)action].Count; j++)
                    if (IsNewButtonReleased(buttonActionList[(int)action][j], playerIndex.Value))
                        return true;
            }
#else
            for (int i = 0; i < keyActionList[(int)action].Count; i++)
                if (IsNewKeyReleased(keyActionList[(int)action][i]))
                    return true;
#endif
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
            return IsButtonPressed(button, playerIndex) && prevGamePadStates[(int)playerIndex].IsButtonUp(button);
        }

        private bool IsNewButtonReleased(Buttons button, PlayerIndex playerIndex)
        {
            return IsButtonReleased(button, playerIndex) && prevGamePadStates[(int)playerIndex].IsButtonDown(button);
        }

        private bool IsButtonPressed(Buttons button, PlayerIndex playerIndex)
        {
            return currGamePadStates[(int)playerIndex].IsButtonDown(button);
        }

        private bool IsButtonReleased(Buttons button, PlayerIndex playerIndex)
        {
            return !IsButtonPressed(button, playerIndex) && prevGamePadStates[(int)playerIndex].IsButtonDown(button);
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
