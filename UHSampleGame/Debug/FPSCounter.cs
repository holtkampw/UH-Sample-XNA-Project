using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using UHSampleGame.ScreenManagement;

namespace UHSampleGame.Debug
{
    class FPSCounter
    {
        static int frames = 0;
        static int frameRate = 0;
        static Vector2 fpsPos = new Vector2(10, 10);
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
            ScreenManager.SpriteBatch.DrawString(font, "FPS: " + frameRate, fpsPos, Color.White); 

            frames++;
        }
    }
}
