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
using UHSampleGame.LevelManagement;

namespace UHSampleGame.Screens
{
    public class SinglePlayerIntroMovie : Screen
    {
        Video video;
        VideoPlayer videoPlayer;
        PlayerSetup[] playerSetup;

        public SinglePlayerIntroMovie()
            : base("SinglePlayerIntroMovie")
        {
            playerSetup = new PlayerSetup[2];
            playerSetup[0] = new PlayerSetup();
            playerSetup[0].type = PlayerType.Human;
            playerSetup[0].playerNum = 1; /////////////////////////////////////////////////////////////////////////SHOULD CHANGE TO ACTIVE PLAYER CONTROLLER?
            playerSetup[0].teamNum = 1;
            playerSetup[0].active = true;

            playerSetup[1] = new PlayerSetup();
            playerSetup[1].type = PlayerType.AI;
            playerSetup[1].playerNum = 2;
            playerSetup[1].teamNum = 2;
            playerSetup[1].active = true;
        }

        public override void HandleInput()
        {
            if (ScreenManager.InputManager.CheckNewAction(InputAction.Selection))
            {
                screenManager.RemoveScreen(this);
                screenManager.ShowScreen(new LoadScreen(LevelType.SingleOne, playerSetup));
            }

        }

        public override void LoadContent()
        {
            video = ScreenManager.Game.Content.Load<Video>("SinglePlayerVids\\scene0101");
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
                //Next Screen
                screenManager.RemoveScreen(this);
                screenManager.ShowScreen(new LoadScreen(LevelType.SingleOne, playerSetup));
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
