using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UHSampleGame.ScreenManagement;

namespace UHSampleGame.MenuSystem
{
    public class MenuEntry
    {
        string text;
        Color color;
        Color selectedColor;
        Color disabledColor;
        bool enabled;
        SpriteFont font;

        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        public Color SelectedColor
        {
            get { return selectedColor; }
            set { selectedColor = value; }
        }

        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public event EventHandler<EventArgs> Selected;

        protected internal virtual void OnSelectEntry()
        {
            if (Selected != null)
                Selected(this, new EventArgs());
        }

        public MenuEntry(string text)
            : this(text, true)
        {

        }

        public MenuEntry(string text, bool enabled)
        {
            this.text = text;
            this.enabled = enabled;

            color = Color.White;
            selectedColor = Color.Yellow;
            disabledColor = Color.Gray;
            font = ScreenManager.Game.Content.Load<SpriteFont>("DummyText\\Font");
        }

        public virtual void Update(MenuScreen screen, bool isSelected) { }

        public virtual void Draw(MenuScreen screen, Vector2 position, bool isSelected)
        {
            // Draw text, centered on the middle of each line.
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Game.Content.Load<SpriteFont>("DummyText\\Font");
            if (!enabled)
            {
                spriteBatch.DrawString(font, text, position, disabledColor);
            }
            else
            {
                spriteBatch.DrawString(font, text, position, (isSelected ? selectedColor : color));
            }
        }

        public int GetHeight()
        {
            return font.LineSpacing;
        }
    }
}
