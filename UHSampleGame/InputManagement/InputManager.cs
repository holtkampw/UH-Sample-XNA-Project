#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#endregion

namespace UHSampleGame.InputManagement
{
    public enum InputAction { Left, Right, Up, Down, Selection, Back, Rotation };

    public class InputManager
    {
        #region Class Variables
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;
        MouseState previousMouseState;
        MouseState currentMouseState;
        Dictionary<InputAction, Keys> items;
        #endregion

        #region Initialization
        public InputManager()
        {
            previousKeyboardState = new KeyboardState();
            currentKeyboardState = new KeyboardState();
            previousMouseState = new MouseState();
            currentMouseState = new MouseState();
            items = new Dictionary<InputAction, Keys>();
        }
        #endregion

        #region Update
        public void Update()
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
        }
        #endregion

        #region Helper Functions
        public void AddKey(InputAction action, Keys key)
        {
            items.Add(action, key);
        }

        public bool CheckKeyboardAction(InputAction action)
        {

            return currentKeyboardState.IsKeyDown(items[action]) && previousKeyboardState.IsKeyUp(items[action]);
        }
        #endregion
    }
}
