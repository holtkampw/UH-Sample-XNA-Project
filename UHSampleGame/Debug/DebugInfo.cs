using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using UHSampleGame.ScreenManagement;
using UHSampleGame.CoreObjects.Units;

namespace UHSampleGame.Debug
{
    class DebugInfo
    {
        static int frames = 0;
        static int frameRate = 0;
        static Vector2 fpsPos = new Vector2(1100, 10);
        static Vector2 fpsOffset = new Vector2(1100, 11);
        static Vector2 unitPos = new Vector2(1100, 30);
        static Vector2 unitOffset = new Vector2(1100, 31);
        static private TimeSpan elapsedTime = new TimeSpan();
        static SpriteFont font = ScreenManager.Game.Content.Load<SpriteFont>("font");

        public static void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frames;
                frames = 0;
            }

        }

        public static void Draw()
        {
            string print = String.Concat("FPS: ", frameRate);
            ScreenManager.SpriteBatch.DrawString(font, print, fpsOffset, Color.Black);
            ScreenManager.SpriteBatch.DrawString(font, print, fpsPos, Color.White);
            print = String.Concat("Units: ", UnitCollection.AllUnitCount());
            ScreenManager.SpriteBatch.DrawString(font,print, unitOffset, Color.Black);
            ScreenManager.SpriteBatch.DrawString(font, print, unitPos, Color.White);

            frames++;
        }
    }
}
