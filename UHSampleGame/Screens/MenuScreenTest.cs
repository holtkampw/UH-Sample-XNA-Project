using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using UHSampleGame.MenuSystem;
using UHSampleGame.ScreenManagement;

namespace UHSampleGame.Screens
{
    public class MenuScreenTest : MenuScreen
    {
        Video video;
        VideoPlayer videoPlayer;

        public MenuScreenTest()
            :base("MenuScreenTest")
        {
            MenuEntry one = new MenuEntry("PlayScreen");
            MenuEntry two = new MenuEntry("ModelAndText");
            MenuEntry three = new MenuEntry("ModelScreen");
            MenuEntry four = new MenuEntry("PlayScreen");

            one.Selected += new EventHandler<EventArgs>(one_Selected);
            two.Selected += new EventHandler<EventArgs>(two_Selected);
            three.Selected += new EventHandler<EventArgs>(three_Selected);
            four.Selected += new EventHandler<EventArgs>(four_Selected);

            AddMenuEntry(one);
            AddMenuEntry(two);
            AddMenuEntry(three);
            AddMenuEntry(four);
        }

        
        void one_Selected(object sender, EventArgs e)
        {
            screenManager.ShowScreen(new PlayScreen());
        }

        void two_Selected(object sender, EventArgs e)
        {
            //screenManager.ShowScreen(new ModelAndText());
        }

        void three_Selected(object sender, EventArgs e)
        {
            screenManager.ShowScreen(new ModelScreen());
        }

        void four_Selected(object sender, EventArgs e)
        {
            screenManager.ShowScreen(new PlayScreen());
        }

        

        public override void LoadContent()
        {
            base.LoadContent();
            video = ScreenManager.Game.Content.Load<Video>("Video\\intro");
            videoPlayer = new VideoPlayer();
            videoPlayer.IsLooped = true;
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            videoPlayer.Stop();
            videoPlayer.Dispose();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!IsVisible)
            {
                videoPlayer.Stop();
                return;
            }

            if (videoPlayer.State != MediaState.Playing)
                videoPlayer.Play(video);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDeviceManager.GraphicsDevice.Viewport;
            spriteBatch.Begin();
            if (videoPlayer.State == MediaState.Playing || videoPlayer.State == MediaState.Stopped)
            {
                //spriteBatch.Draw(videoPlayer.GetTexture(), new Rectangle(0, 0, video.Width, video.Height), Color.White);
                spriteBatch.Draw(videoPlayer.GetTexture(), new Rectangle(0, 0, viewport.Width , viewport.Height), Color.White);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
