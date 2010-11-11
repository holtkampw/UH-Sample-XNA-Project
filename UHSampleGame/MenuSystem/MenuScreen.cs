using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UHSampleGame.ScreenManagement;
using UHSampleGame.InputManagement;

namespace UHSampleGame.MenuSystem
{
    public class MenuScreen : Screen
    {
        List<MenuEntry> menuEntries;
        Vector2 menuEntryStartPosition;
        int selectedEntry;
        string menuTitle;

        protected IList<MenuEntry> MenuEntries
        {
            get { return menuEntries; }
        }

        public MenuScreen(string menuTitle)
            :base(menuTitle)
        {
            this.menuTitle = menuTitle;
            this.selectedEntry = 0;
            this.menuEntries = new List<MenuEntry>();
            this.menuEntryStartPosition = new Vector2(90, 220);
        }

        public override void LoadContent()
        {
           
        }

        public override void Reload()
        {
            
        }

        public override void HandleInput(InputManager input)
        {
            // Move to the previous menu entry
            if (input.CheckNewAction(InputAction.MenuUp))
            {
                selectedEntry--;

                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;
            }

            // Move to the next menu entry
            if (input.CheckNewAction(InputAction.MenuDown))
            {
                selectedEntry++;

                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;
            }

            if (input.CheckNewAction(InputAction.MenuSelect))
                OnSelectEntry(selectedEntry);
            else if (input.CheckNewAction(InputAction.MenuCancel))
                OnCancel();
        }

        public void AddMenuEntry(MenuEntry menuEntry)
        {
            menuEntries.Add(menuEntry);
        }

        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex)
        {
            menuEntries[selectedEntry].OnSelectEntry();
        }

        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected virtual void OnCancel()
        {
            ExitScreen();
        }


        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        /// </summary>
        protected void OnCancel(object sender, EventArgs e)
        {
            OnCancel();
        }

        public override void Update(GameTime gameTime)
        {

            for (int i = 0; i < menuEntries.Count; i++)
            {
                bool isSelected = IsVisible && (i == selectedEntry);

                menuEntries[i].Update(this, isSelected);
            }
        }

        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Game.Content.Load<SpriteFont>("DummyText\\Font");

            spriteBatch.Begin();

            Vector2 position = new Vector2(menuEntryStartPosition.X, menuEntryStartPosition.Y);

            // Draw each menu entry in turn.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];

                bool isSelected = IsVisible && (i == selectedEntry);

                menuEntry.Draw(this, position, isSelected);

                position.Y += menuEntry.GetHeight();
            }

            // Draw the menu title.
            //Vector2 titlePosition = new Vector2(ScreenManager.GraphicsDeviceManager.GraphicsDevice.Viewport.Width / 2, 50);
            //Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
            //Color titleColor = Color.White;
            //float titleScale = 1.25f;

            //spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0,
            //                       titleOrigin, titleScale, SpriteEffects.None, 0);

            spriteBatch.End();
        }

        public override void UnloadContent()
        {
           
        }

        

    }
}
