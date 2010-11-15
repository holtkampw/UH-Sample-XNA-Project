#region Using Statements
using System;
using System.Threading;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UHSampleGame.ScreenManagement;
using UHSampleGame.LevelManagement;
using UHSampleGame.Players;
#endregion

namespace UHSampleGame.Screens
{
    public class LoadScreen : Screen
    {
        #region Class Variables
        Texture2D loading_screen;
        Texture2D loading_screen_hl;
        float currentLoadingOpacity = 0;
        Vector2 currentLoadingPosition = Vector2.Zero;
        Vector3 whiteVector3 = new Vector3(255, 255, 255);

        Thread backgroundThread;
        EventWaitHandle backgroundThreadExit;

        GraphicsDevice graphicsDevice;

        GameTime loadStartTime;
        TimeSpan loadAnimationTimer;
        PlayerSetup[] playerTypes;
        LevelType levelType;
        #endregion

        public LoadScreen(LevelType levelType, PlayerSetup[] playerTypes) :
            base("LoadScreen")
        {
            backgroundThread = new Thread(BackgroundWorkerThread);
            backgroundThreadExit = new ManualResetEvent(false);

            graphicsDevice = ScreenManager.Game.GraphicsDevice;
            this.playerTypes = playerTypes;
            this.levelType = levelType;
        }

        public override void Reload()
        {
            
        }

        public override void LoadContent()
        {
            switch(levelType)
            {
                case LevelType.SingleOne:
                    loading_screen = ScreenManager.Game.Content.Load<Texture2D>("LoadScreen\\levelLoader01");
                    loading_screen_hl = ScreenManager.Game.Content.Load<Texture2D>("LoadScreen\\levelLoader01_hl");
                    break;
                case LevelType.MultiTwo:
                case LevelType.MultiThree:
                case LevelType.MultiFour:
                    loading_screen = ScreenManager.Game.Content.Load<Texture2D>("LoadScreen\\levelLoaderMultiplay");
                    loading_screen_hl = ScreenManager.Game.Content.Load<Texture2D>("LoadScreen\\levelLoaderMultiplay_hl");
                    break;
                default:
                    loading_screen = ScreenManager.Game.Content.Load<Texture2D>("LoadScreen\\levelLoader01");
                    loading_screen_hl = ScreenManager.Game.Content.Load<Texture2D>("LoadScreen\\levelLoader01_hl");
                    break;
            }
        }

        public override void UnloadContent()
        {
            
        }

        public override void HandleInput()
        {
        }

        public override void Update(GameTime gameTime)
        {
            // Start up the background thread, which will update the network
            // session and draw the animation while we are loading.
            if (backgroundThread != null)
            {
                loadStartTime = gameTime;
                backgroundThread.Start();
            }

            
            // Perform the load operation.
            ScreenManager.RemoveScreen(this);

            if (levelType == LevelType.SingleOne)
                ScreenManager.ShowScreen(new PlayScreen(playerTypes, PlayerScreenType.Scenario));
            else
                ScreenManager.ShowScreen(new PlayScreen(playerTypes, PlayerScreenType.Multiplayer));
            // Signal the background thread to exit, then wait for it to do so.
            if (backgroundThread != null)
            {
               
                backgroundThreadExit.Set();
                backgroundThread.Join();

            }

           // loading_screen.Dispose();
            //loading_screen_hl.Dispose();
            // Once the load has finished, we use ResetElapsedTime to tell
            // the  game timing mechanism that we have just finished a very
            // long frame, and that it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }

        public override void Draw(GameTime gameTime)
        {
            currentLoadingPosition.X += 16.0f;
            if (currentLoadingPosition.X < 600.0f)
                currentLoadingOpacity += 30f;    
            else
                currentLoadingOpacity -= 30f;


            if (currentLoadingPosition.X >= 1280.0f)
            {
                currentLoadingPosition.X = -900.0f;
                currentLoadingOpacity = 0;
            }
            currentLoadingOpacity = MathHelper.Clamp(currentLoadingOpacity, 0, 255);

            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            ScreenManager.SpriteBatch.Draw(loading_screen, Vector2.Zero, Color.White);
            ScreenManager.SpriteBatch.Draw(loading_screen_hl, currentLoadingPosition,
                new Color(255, 255 , 255, currentLoadingOpacity));
            ScreenManager.SpriteBatch.End();
        }

        #region Background Thread


        /// <summary>
        /// Worker thread draws the loading animation and updates the network
        /// session while the load is taking place.
        /// </summary>
        void BackgroundWorkerThread()
        {
            long lastTime = Stopwatch.GetTimestamp();

            // EventWaitHandle.WaitOne will return true if the exit signal has
            // been triggered, or false if the timeout has expired. We use the
            // timeout to update at regular intervals, then break out of the
            // loop when we are signalled to exit.
            while (!backgroundThreadExit.WaitOne(1000 / 30))
            {
                GameTime gameTime = GetGameTime(ref lastTime);

                DrawLoadAnimation(gameTime);
            }
        }


        /// <summary>
        /// Works out how long it has been since the last background thread update.
        /// </summary>
        GameTime GetGameTime(ref long lastTime)
        {
            long currentTime = Stopwatch.GetTimestamp();
            long elapsedTicks = currentTime - lastTime;
            lastTime = currentTime;

            TimeSpan elapsedTime = TimeSpan.FromTicks(elapsedTicks *
                                                      TimeSpan.TicksPerSecond /
                                                      Stopwatch.Frequency);

            return new GameTime(loadStartTime.TotalGameTime + elapsedTime, elapsedTime);
        }


        /// <summary>
        /// Calls directly into our Draw method from the background worker thread,
        /// so as to update the load animation in parallel with the actual loading.
        /// </summary>
        void DrawLoadAnimation(GameTime gameTime)
        {
            if ((graphicsDevice == null) || graphicsDevice.IsDisposed)
                return;

            try
            {
                graphicsDevice.Clear(Color.Black);

                // Draw the loading screen.
                Draw(gameTime);

                graphicsDevice.Present();
            }
            catch
            {
                // If anything went wrong (for instance the graphics device was lost
                // or reset) we don't have any good way to recover while running on a
                // background thread. Setting the device to null will stop us from
                // rendering, so the main game can deal with the problem later on.
                graphicsDevice = null;
            }
        }
        #endregion
    }
}
