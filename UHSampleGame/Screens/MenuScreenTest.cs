using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using UHSampleGame.MenuSystem;
using UHSampleGame.ScreenManagement;
using UHSampleGame.CameraManagement;
using UHSampleGame.InputManagement;
using UHSampleGame.LevelManagement;
using UHSampleGame.Players;
using Microsoft.Xna.Framework.Audio;

namespace UHSampleGame.Screens
{
    public class MenuScreenTest : Screen
    {

        Texture2D titleTexture;

        Model menuModel;
        Matrix[] menuTransforms;
        Vector3 menuPosition;
        float modelRotation = 14.9f;
        float rotationOffset = 1.57f;
        //float modelRotation = 0f;
        //float rotationOffset = 1.57f;
        //float rotationX = -30f;
        //float rotationZ = 30f;

        CameraManager cameraManager;

        List<MenuEntry> items;
        int selected;

        Texture2D[] arrows;
        Vector2[] arrow_locations_min;
        Vector2[] arrow_locations_max;
        float arrowUpdateTime;
        float maxArrowUpdateTime = 200;
        int currentArrowIndex;
        Texture2D a_button;
        Vector2 a_button_position;

        string text;
        SpriteFont font;
        Vector2 text_position;
        SoundEffect tickEffect;

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
            
            items = new List<MenuEntry>();
            items.Add(one);
            items.Add(two);
            items.Add(three);
            items.Add(four);

            menuModel = ScreenManager.Game.Content.Load<Model>("MainMenu\\oilBarrel_MainMenu");
            menuTransforms = new Matrix[menuModel.Bones.Count];
            menuModel.CopyAbsoluteBoneTransformsTo(menuTransforms);

            cameraManager = (CameraManager)ScreenManager.Game.Services.GetService(typeof(CameraManager));
            cameraManager.SetPosition(new Vector3(0.0f, 0.0f, 500.0f));
            cameraManager.SetLookAtPoint(new Vector3(0.0f, 0.0f, 50.0f));
            //cameraManager.SetPosition(new Vector3(0.0f, 1700.0f, 500.0f));
            //cameraManager.SetLookAtPoint(new Vector3(0.0f, 0.0f, -500.0f));

            menuPosition = new Vector3(100.0f, 0.0f, 0.0f);

            arrows = new Texture2D[2];
            arrows[0] = ScreenManager.Game.Content.Load<Texture2D>("MainMenu\\left_arrow");
            arrows[1] = ScreenManager.Game.Content.Load<Texture2D>("MainMenu\\right_arrow");

            arrow_locations_min = new Vector2[2];
            arrow_locations_max = new Vector2[2];
            arrow_locations_min[0] = new Vector2(550.0f, 320.0f);
            arrow_locations_min[1] = new Vector2(1050.0f, 320.0f);
            arrow_locations_max[0] = new Vector2(530.0f, 320.0f);
            arrow_locations_max[1] = new Vector2(1070.0f, 320.0f);
            selected = 0;
            arrowUpdateTime = 0;
            currentArrowIndex = 0;

            a_button = ScreenManager.Game.Content.Load<Texture2D>("MainMenu\\a_button");
            a_button_position = new Vector2(720.0f, 600.0f);
            font = ScreenManager.Game.Content.Load<SpriteFont>("MainMenu\\font");
            text_position = new Vector2(790.0f, 600.0f);

            tickEffect = ScreenManager.Game.Content.Load<SoundEffect>("Sounds\\Effects\\explosion");
        }

        public override void HandleInput()
        {
            if (ScreenManager.InputManager.CheckNewAction(InputAction.MenuLeft))
            {
                if (selected - 1 < 0)
                    selected = 3;
                else
                    selected--;

                modelRotation += rotationOffset;
                tickEffect.Play(0.15f, 1.0f, 0.0f);
            }
            
            if (ScreenManager.InputManager.CheckNewAction(InputAction.MenuRight))
            {
                if (selected + 1 > 3)
                    selected = 0;
                else
                    selected++;

                modelRotation -= rotationOffset;
                tickEffect.Play(0.15f, 1.0f, 0.0f);
            }
            
            if (ScreenManager.InputManager.CheckNewAction(InputAction.Selection))
            {
                items[selected].OnSelectEntry();
            }
        }

        
        void one_Selected(object sender, EventArgs e)
        {
            
            screenManager.ShowScreen(new SinglePlayerIntroMovie());
        }

        void two_Selected(object sender, EventArgs e)
        {
            screenManager.ShowScreen(new MultiplayerLobby());
        }

        void three_Selected(object sender, EventArgs e)
        {
            screenManager.ShowScreen(new ControlsScreen());
        }

        void four_Selected(object sender, EventArgs e)
        {
            //screenManager.ShowScreen(new PlayScreen());
        }

        

        public override void LoadContent()
        {

            titleTexture = ScreenManager.Game.Content.Load<Texture2D>("MainMenu\\mainMenu_Cover");

        }

        public override void UnloadContent()
        {
        }

        public override void Reload()
        {
            cameraManager.SetPosition(new Vector3(0.0f, 0.0f, 500.0f));
            cameraManager.SetLookAtPoint(new Vector3(0.0f, 0.0f, 50.0f));
        }

        public override void Update(GameTime gameTime)
        {
            cameraManager.Update();

            arrowUpdateTime += gameTime.ElapsedGameTime.Milliseconds;
            if (arrowUpdateTime > maxArrowUpdateTime)
            {
                if(currentArrowIndex == 1)
                    currentArrowIndex = 0;
                else
                    currentArrowIndex = 1;

                arrowUpdateTime = 0;
            }            
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();

            spriteBatch.Draw(titleTexture, new Vector2(0, 0), Color.White);

            if (currentArrowIndex == 0)
            {
                spriteBatch.Draw(arrows[0], arrow_locations_min[0], Color.White);
                spriteBatch.Draw(arrows[1], arrow_locations_min[1], Color.White);
            }
            else
            {
                spriteBatch.Draw(arrows[0], arrow_locations_max[0], Color.White);
                spriteBatch.Draw(arrows[1], arrow_locations_max[1], Color.White);
            }

            spriteBatch.Draw(a_button, a_button_position, Color.White);
            switch(selected)
            {
                case 0:
                    text = "Single Player";
                    break;
                case 1:
                    text = "Multiplayer";
                    break;
                case 2:
                    text = "Controls";
                    break;
                case 3:
                    text = "Exit";
                    break;
            }
            spriteBatch.DrawString(font, text, text_position - new Vector2(1.0f, 0.0f), Color.Black);
            spriteBatch.DrawString(font, text, text_position, Color.White);
            spriteBatch.End();

            ResetRenderStates();
            DrawMenuModel();
        }

        private void DrawMenuModel()
        {
            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in menuModel.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = menuTransforms[mesh.ParentBone.Index] *
                        Matrix.CreateScale(16.0f) *
                      //  Matrix.CreateRotationX(rotationX) * 
                        Matrix.CreateRotationY(modelRotation) * 
                      //  Matrix.CreateRotationZ(rotationZ) * 
                        Matrix.CreateTranslation(menuPosition);
                       // * Matrix.CreateScale(10.0f);// *
                        /*Matrix.CreateRotationY(modelRotation)
                        * Matrix.CreateTranslation(modelPosition);*/
                    effect.View = cameraManager.ViewMatrix;
                    effect.Projection = cameraManager.ProjectionMatrix;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }
    }
}
