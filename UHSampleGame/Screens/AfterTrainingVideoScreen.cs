using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using UHSampleGame.ScreenManagement;
using UHSampleGame.InputManagement;
using UHSampleGame.Players;

namespace UHSampleGame.Screens
{
    public class AfterTrainingVideoScreen : Screen
    {
        Video video;
        VideoPlayer videoPlayer;

        public AfterTrainingVideoScreen()
            : base("AfterTrainingVideoScreen")
        {

        }

        public override void HandleInput()
        {
            if (ScreenManager.InputManager.CheckNewAction(InputAction.Selection))
            {
                screenManager.RemoveScreen(this);
                screenManager.ShowScreen(new MoreLevelsLater());
            }

        }

        public override void LoadContent()
        {
            video = ScreenManager.Game.Content.Load<Video>("SinglePlayerVids\\scene0102");
            videoPlayer = new VideoPlayer();
            videoPlayer.IsLooped = false;

            if (videoPlayer.State != MediaState.Playing)
                videoPlayer.Play(video);
        }

        public override void UnloadContent()
        {
            videoPlayer.Stop();
            videoPlayer.Dispose();
        }

        public override void Reload()
        {

        }

        public override void Update(GameTime gameTime)
        {
            if (!IsVisible)
            {
                if (videoPlayer.State != MediaState.Stopped)
                    videoPlayer.Stop();

                return;
            }

            if (videoPlayer.State == MediaState.Stopped)
            {

                screenManager.RemoveScreen(this);
                screenManager.ShowScreen(new MoreLevelsLater());
            }


        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDeviceManager.GraphicsDevice.Viewport;
            spriteBatch.Begin();

            if (videoPlayer.State == MediaState.Playing || videoPlayer.State == MediaState.Stopped)
            {
                spriteBatch.Draw(videoPlayer.GetTexture(), new Rectangle(0, 0, viewport.Width, viewport.Height), Color.White);
            }
            spriteBatch.End();
        }
    }
}
