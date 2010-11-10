using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using UHSampleGame.ScreenManagement;
using UHSampleGame.CameraManagement;
using UHSampleGame.TileSystem;
using UHSampleGame.ProjectileManagment;
using UHSampleGame.Players;

namespace UHSampleGame.CoreObjects.Towers
{
    public class TowerCollection
    {

        #region Class Variables

        const int MAX_TOWERS = 100;

        static int NumPlayers;
        static Enum[] towerTypes = EnumHelper.EnumToArray(new TowerType());
        static string[] mapTeamNumToColor = { " ", "red", "Blue", "Green", "yellow" };

        //units[playerNum][unitType][index]
        static List<List<List<Tower>>> towers;

        //unitsCount[playerNum][unitType]
        static List<List<int>> towerCount;

        //Instancing Verticies
        static VertexDeclaration instanceVertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 1),
            new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 2),
            new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 3)
        );

        //instancedModels[unitType][teamNum]
        static List<List<Model>> instancedModels;

        //instancedModelBones[unitType][teamNum]
        static List<List<Matrix[]>> instancedModelBones;

        //unitTransforms[playerNum][unitType][index]
        static Matrix[] towerTransforms;

        static DynamicVertexBuffer instanceVertexBuffer = null;

        static CameraManager cameraManager;

        static int updateCount;
<<<<<<< HEAD
        static int drawCount;

        static Texture2D[] hudBackground;
        static Rectangle hudBackgroundLocation = new Rectangle(0, 0, 40, 20);
        static Rectangle hudBackgroundSourceLocation = new Rectangle(0, 0, 1280, 394);
        static Vector2 hudBackgroundOffsetFromTower = new Vector2(-20, 8);
        static Color hudColor = new Color(255, 255, 255, 255);
        static Vector3 hudTempLocation;

        static Vector2 levelLocation = Vector2.Zero;
        static Vector2 levelOffset = new Vector2(1, 1);
        static SpriteFont levelFont;

        static Texture2D hudHealthBar;
        static Rectangle hudHealthSource = new Rectangle(0, 0, 904, 136);
        static Rectangle hudHealthDestination = new Rectangle(0, 0, 40, 10);
        static Rectangle hudHealthMaxDestination = new Rectangle(0, 0, 40, 10);
        static Vector2 hudHealthOffset = new Vector2(0, 0);

=======
        static int drawCount;

        static Texture2D[] hudBackground;
        static Rectangle hudBackgroundLocation = new Rectangle(0, 0, 40, 20);
        static Rectangle hudBackgroundSourceLocation = new Rectangle(0, 0, 1280, 394);
        static Vector2 hudBackgroundOffsetFromTower = new Vector2(-20, 8);
        static Color hudColor = new Color(255, 255, 255, 255);
        static Vector3 hudTempLocation;

        static Vector2 levelLocation = Vector2.Zero;
        static Vector2 levelOffset = new Vector2(1, 1);
        static SpriteFont levelFont;

        static Texture2D hudHealthBar;
        static Rectangle hudHealthSource = new Rectangle(0, 0, 904, 136);
        static Rectangle hudHealthDestination = new Rectangle(0, 0, 40, 10);
        static Rectangle hudHealthMaxDestination = new Rectangle(0, 0, 40, 10);
        static Vector2 hudHealthOffset = new Vector2(0, 0);

>>>>>>> ea2a09c22888654baa4f32283fd69020834b5209
        static Texture2D[] hudUpgradeBar;
        static Rectangle hudUpgradeSource = new Rectangle(0, 0, 904, 136);
        static Rectangle hudUpgradeDestination = new Rectangle(0, 0, 40, 10);
        static Rectangle hudUpgradeMaxDestination = new Rectangle(0, 0, 40, 10);
        static Vector2 hudUpgradeOffset = new Vector2(0, 10);
        #endregion

        #region Initialize
        public static void Initialize(int numPlayers)
        {
            cameraManager = (CameraManager)ScreenManager.Game.Services.GetService(typeof(CameraManager));
            NumPlayers = numPlayers;
            towers = new List<List<List<Tower>>>();
            towerCount = new List<List<int>>();
            towerTransforms = new Matrix[MAX_TOWERS];

            for (int i = 0; i < numPlayers; i++)
            {
                towers.Add(new List<List<Tower>>());
                towerCount.Add(new List<int>());

                for (int j = 0; j < towerTypes.Length; j++)
                {
                    towers[i].Add(new List<Tower>());
                    towerCount[i].Add(0);

                    for (int k = 0; k < MAX_TOWERS; k++)
                    {
                        towers[i][j].Add(new Tower((TowerType)j));
                    }
                }
            }

            instancedModels = new List<List<Model>>();
            instancedModelBones = new List<List<Matrix[]>>();
            for (int j = 0; j < towerTypes.Length; j++)
            {
                instancedModelBones.Add(new List<Matrix[]>());
                instancedModels.Add(new List<Model>());
                instancedModels[j].Add(null);
                instancedModelBones[j].Add(null);
                for (int p = 1; p < 5; p++)
                {
                    switch ((TowerType)j)
                    {
                        case TowerType.Plasma:
                            instancedModels[j].Add(ScreenManagement.ScreenManager.Game.Content.Load<Model>("Objects\\Towers\\towerA_" + mapTeamNumToColor[p]));
                            instancedModelBones[j].Add(new Matrix[instancedModels[j][p].Bones.Count]);
                            instancedModels[j][p].CopyAbsoluteBoneTransformsTo(instancedModelBones[j][p]);
                            break;
                        case TowerType.Electric:
                            instancedModels[j].Add(ScreenManagement.ScreenManager.Game.Content.Load<Model>("Objects\\Towers\\towerB_" + mapTeamNumToColor[p]));
                            instancedModelBones[j].Add(new Matrix[instancedModels[j][p].Bones.Count]);
                            instancedModels[j][p].CopyAbsoluteBoneTransformsTo(instancedModelBones[j][p]);
                            break;
                        case TowerType.Cannon:
                            instancedModels[j].Add(ScreenManagement.ScreenManager.Game.Content.Load<Model>("Objects\\Towers\\towerC_" + mapTeamNumToColor[p]));
                            instancedModelBones[j].Add(new Matrix[instancedModels[j][p].Bones.Count]);
                            instancedModels[j][p].CopyAbsoluteBoneTransformsTo(instancedModelBones[j][p]);
                            break;
                        case TowerType.SmallUnit:
                            instancedModels[j].Add(ScreenManagement.ScreenManager.Game.Content.Load<Model>("Objects\\Towers\\baseTowerA"));//+ mapTeamNumToColor[p]));
                            instancedModelBones[j].Add(new Matrix[instancedModels[j][p].Bones.Count]);
                            instancedModels[j][p].CopyAbsoluteBoneTransformsTo(instancedModelBones[j][p]);
                            break;
                        case TowerType.LargeUnit:
                            instancedModels[j].Add(ScreenManagement.ScreenManager.Game.Content.Load<Model>("Objects\\Towers\\baseTowerB"));//+ mapTeamNumToColor[p]));
                            instancedModelBones[j].Add(new Matrix[instancedModels[j][p].Bones.Count]);
                            instancedModels[j][p].CopyAbsoluteBoneTransformsTo(instancedModelBones[j][p]);
                            break;
                    }
                }
<<<<<<< HEAD
            }

=======
            }

>>>>>>> ea2a09c22888654baa4f32283fd69020834b5209
            hudBackground = new Texture2D[5];
            hudBackground[1] = ScreenManager.Game.Content.Load<Texture2D>("HUD\\towerStatus1");
            hudBackground[2] = ScreenManager.Game.Content.Load<Texture2D>("HUD\\towerStatus2");
            hudBackground[3] = ScreenManager.Game.Content.Load<Texture2D>("HUD\\towerStatus3");
            hudBackground[4] = ScreenManager.Game.Content.Load<Texture2D>("HUD\\towerStatus4");
<<<<<<< HEAD
            levelFont = ScreenManager.Game.Content.Load<SpriteFont>("HUD\\levelFont");
            hudHealthBar = ScreenManager.Game.Content.Load<Texture2D>("HUD\\towerStatus_Health");
=======
            levelFont = ScreenManager.Game.Content.Load<SpriteFont>("HUD\\levelFont");
            hudHealthBar = ScreenManager.Game.Content.Load<Texture2D>("HUD\\towerStatus_Health");
>>>>>>> ea2a09c22888654baa4f32283fd69020834b5209
            hudUpgradeBar = new Texture2D[5];
            hudUpgradeBar[1] = ScreenManager.Game.Content.Load<Texture2D>("HUD\\towerStatus_Growth1");
            hudUpgradeBar[2] = ScreenManager.Game.Content.Load<Texture2D>("HUD\\towerStatus_Growth2");
            hudUpgradeBar[3] = ScreenManager.Game.Content.Load<Texture2D>("HUD\\towerStatus_Growth3");
            hudUpgradeBar[4] = ScreenManager.Game.Content.Load<Texture2D>("HUD\\towerStatus_Growth3");
        }

        public static int AllTowerCount()
        {
            int sum = 0;

            for (int i = 0; i < NumPlayers; i++)
                for (int j = 0; j < towerTypes.Length; j++)
                    sum += towerCount[i][j];

            return sum;
        }
        #endregion

        #region Manipulation
        public static void SetAllNoShoot(int playerNum)
        {
            for (int j = 0; j < towerTypes.Length; j++)
            {
                for (int i = 0; i < MAX_TOWERS; i++)
                {

                    if (towers[playerNum][j][i].IsActive())
                    {
                        towers[playerNum][j][i].Status = TowerStatus.ActiveNoShoot;
                    }
                }
            }
        }

        public static bool Destroy(int playerNum)
        {
            Tower t;
            Vector3 nv = new Vector3();
            int count = 0;
            int maxCount = 3;
            for (int j = 0; j < towerTypes.Length; j++)
            {
                for (int i = 0; i < MAX_TOWERS; i++)
                {

                    t = towers[playerNum][j][i];
                    if (t.IsActive() || t.Status == TowerStatus.ActiveNoShoot)
                    {
                        t.Status = TowerStatus.Inactive;
                        nv.X = t.Position.X;
                        nv.Y = t.Position.Y + 5;
                        nv.Z = t.Position.Z;
                        ProjectileManager.AddParticle(t.Position, nv);
                        count++;
                        if (count >= maxCount)
                            return false;
                    }
                }
               
            }
            return true;
        }

        public static Tower Add(int playerNum, int teamNum, int money, TowerType towerType, Vector3 position)
        {
            //////////////////////REFACTOR FOR EFFICIENCY
            Tile tile = TileMap.GetTileFromPos(position);
            Tower tower;
            if (!tile.IsWalkable() || tile.IsBase())
                return null;

            if (towers[playerNum][(int)towerType][0].Cost > money)
                return null;

            for (int i = 0; i < MAX_TOWERS; i++)
            {
                tower = towers[playerNum][(int)towerType][i];
                if (!tower.IsActive())
                {

                    if (TileMap.SetTower(ref tower, ref tile))
                    {


                        towers[playerNum][(int)towerType][i].Activate(playerNum, teamNum);
                        towers[playerNum][(int)towerType][i].Type = towerType;
                        towers[playerNum][(int)towerType][i].Setup(position);
                        towerCount[playerNum][(int)towerType]++;
                        return towers[playerNum][(int)towerType][i];
                    }

                }
            }
            return null;
        }

        public static int Remove(int teamNum, ref Vector3 position)
        {
            Tile tile = TileMap.GetTileFromPos(position);
            int moneyBack = 0;

            if (tile.Tower == null)
                return 0;

            if (tile.Tower.TeamNum == teamNum)
            {
                tile.Tower.Status = TowerStatus.Inactive;

                //do we need to d this???? test without :)
                //for (int i = 0; i < MAX_TOWERS; i++)
                //{
                //    if (towers[playerNum][(int)tile.Tower.Type][i].ID == tile.Tower.ID)
                //        towers[playerNum][(int)tile.Tower.Type][i].Status = TowerStatus.Inactive;
                //}
                moneyBack = tile.Tower.DestroyCost();
                TileMap.RemoveTower(ref tile);
            }
            return moneyBack;
        }

        public static int Repair(int teamNum, int money, ref Vector3 position)
        {
            Tile tile = TileMap.GetTileFromPos(position);
            if (tile.Tower != null)
            {
                if (tile.Tower.TeamNum == teamNum)
                    tile.Tower.Repair(money);
                //Do we need this??
                //for (int i = 0; i < MAX_TOWERS; i++)
                //{
                //    if (towers[playerNum][(int)tile.Tower.Type][i].ID == tile.Tower.ID)
                //        return towers[playerNum][(int)tile.Tower.Type][i].Repair(money);
                //}
            }
            return 0;
        }

        public static int Upgrade(int teamNum, int money, ref Vector3 position)
        {
            Tile tile = TileMap.GetTileFromPos(position);
            if (tile.Tower != null)
            {
                if (tile.Tower.TeamNum == teamNum)
                    return tile.Tower.Upgrade(money);
                //DO we need this??
                //for (int i = 0; i < MAX_TOWERS; i++)
                //{
                //    if (towers[playerNum][(int)tile.Tower.Type][i].ID == tile.Tower.ID)
                //        towers[playerNum][(int)tile.Tower.Type][i].Upgrade(money);
                //}
            }
            return 0;

        }
        #endregion

        #region Update/Draw
        public static void Update(GameTime gameTime)
        {
            for (int i = 0; i < NumPlayers; i++)
                for (int j = 0; j < towerTypes.Length; j++)
                {
                    updateCount = 0;
                    for (int k = 0; k < MAX_TOWERS && updateCount < towerCount[i][j]; k++)
                        if (towers[i][j][k].IsActive())
                        {
                            towers[i][j][k].Update(gameTime);
                            updateCount++;
                        }
                }
        }

        public static void Draw(GameTime gameTime)
        {
            for (int i = 0; i < NumPlayers; i++)
            {
                for (int j = 0; j < towerTypes.Length; j++)
                {
                    DrawTowers(i, j);
                }
<<<<<<< HEAD
            }

            for (int p = 1; p <= PlayerCollection.NumPlayers; p++)
            {
                if (PlayerCollection.ShowHUDFor(p))
                {
                    ScreenManager.SpriteBatch.Begin();
                    for (int j = 0; j < towerTypes.Length; j++)
                    {
                        drawCount = 0;
                        for (int k = 0; k < MAX_TOWERS && updateCount < towerCount[p][j]; k++)
                            if (towers[p][j][k].IsActive())
                            {
                                hudTempLocation = ScreenManager.GraphicsDeviceManager.GraphicsDevice.Viewport.Project(
                                         towers[p][j][k].Position,
                                         cameraManager.ProjectionMatrix,
                                         cameraManager.ViewMatrix,
                                         Matrix.Identity
                                );
                                hudBackgroundLocation.X = (int)(hudTempLocation.X + hudBackgroundOffsetFromTower.X);
                                hudBackgroundLocation.Y = (int)(hudTempLocation.Y + hudBackgroundOffsetFromTower.Y);

                                levelLocation.X = hudBackgroundLocation.X;
                                levelLocation.Y = hudBackgroundLocation.Y;
                                levelLocation += levelOffset;

                                //draw background
                                ScreenManager.SpriteBatch.Draw(hudBackground[towers[p][j][k].Level],
                                    hudBackgroundLocation, hudBackgroundSourceLocation, hudColor);

                                //draw level num
                                ScreenManager.SpriteBatch.DrawString(levelFont, towers[p][j][k].LevelString,
                                    levelLocation, hudColor);

                                //draw health bar
                                hudHealthDestination.X = (int)(hudBackgroundLocation.X + hudHealthOffset.X);
                                hudHealthDestination.Y = (int)(hudBackgroundLocation.Y + hudHealthOffset.Y);
                                ScreenManager.SpriteBatch.Draw(hudHealthBar,
                                    hudHealthDestination, hudHealthSource, hudColor);

                                //draw upgrade bar
                                hudUpgradeDestination.X = (int)(hudBackgroundLocation.X + hudUpgradeOffset.X);
                                hudUpgradeDestination.Y = (int)(hudBackgroundLocation.Y + hudUpgradeOffset.Y);
                                hudUpgradeDestination.Width = (int)((towers[p][j][k].XP / 100) * hudUpgradeMaxDestination.Width);
                                ScreenManager.SpriteBatch.Draw(hudUpgradeBar[towers[p][j][k].Level],
                                    hudUpgradeDestination, hudUpgradeSource, hudColor);

                                drawCount++;
                            }
                    }

                    ScreenManager.SpriteBatch.End();
                }
=======
            }

            for (int p = 1; p <= PlayerCollection.NumPlayers; p++)
            {
                if (PlayerCollection.ShowHUDFor(p))
                {
                    ScreenManager.SpriteBatch.Begin();
                    for (int j = 0; j < towerTypes.Length; j++)
                    {
                        drawCount = 0;
                        for (int k = 0; k < MAX_TOWERS && updateCount < towerCount[p][j]; k++)
                            if (towers[p][j][k].IsActive())
                            {
                                hudTempLocation = ScreenManager.GraphicsDeviceManager.GraphicsDevice.Viewport.Project(
                                         towers[p][j][k].Position,
                                         cameraManager.ProjectionMatrix,
                                         cameraManager.ViewMatrix,
                                         Matrix.Identity
                                );
                                hudBackgroundLocation.X = (int)(hudTempLocation.X + hudBackgroundOffsetFromTower.X);
                                hudBackgroundLocation.Y = (int)(hudTempLocation.Y + hudBackgroundOffsetFromTower.Y);

                                levelLocation.X = hudBackgroundLocation.X;
                                levelLocation.Y = hudBackgroundLocation.Y;
                                levelLocation += levelOffset;

                                //draw background
                                ScreenManager.SpriteBatch.Draw(hudBackground[towers[p][j][k].Level],
                                    hudBackgroundLocation, hudBackgroundSourceLocation, hudColor);

                                //draw level num
                                ScreenManager.SpriteBatch.DrawString(levelFont, towers[p][j][k].LevelString,
                                    levelLocation, hudColor);

                                //draw health bar
                                hudHealthDestination.X = (int)(hudBackgroundLocation.X + hudHealthOffset.X);
                                hudHealthDestination.Y = (int)(hudBackgroundLocation.Y + hudHealthOffset.Y);
                                ScreenManager.SpriteBatch.Draw(hudHealthBar,
                                    hudHealthDestination, hudHealthSource, hudColor);

                                //draw upgrade bar
                                hudUpgradeDestination.X = (int)(hudBackgroundLocation.X + hudUpgradeOffset.X);
                                hudUpgradeDestination.Y = (int)(hudBackgroundLocation.Y + hudUpgradeOffset.Y);
                                hudUpgradeDestination.Width = (int)((towers[p][j][k].XP / 100) * hudUpgradeMaxDestination.Width);
                                ScreenManager.SpriteBatch.Draw(hudUpgradeBar[towers[p][j][k].Level],
                                    hudUpgradeDestination, hudUpgradeSource, hudColor);

                                drawCount++;
                            }
                    }

                    ScreenManager.SpriteBatch.End();
                }
>>>>>>> ea2a09c22888654baa4f32283fd69020834b5209
            }

        }

        private static void DrawTowers(int i, int j)
        {
            drawCount = 0;
            for (int k = 0; k < MAX_TOWERS && drawCount < towerCount[i][j]; k++)
            {
                if (towers[i][j][k].IsActive() || towers[i][j][k].Status == TowerStatus.ActiveNoShoot)
                {
                    towerTransforms[drawCount] = towers[i][j][k].Transforms;
                    drawCount++;
                }
            }

            DrawInstancedTowers(i, j, drawCount);

            

        }


        private static void DrawInstancedTowers(int playerNum, int towerType, int amount)
        {
            //int amount = units[playerNum][unitType].Count;

            if (amount == 0)
                return;

            instanceVertexBuffer = null; ///////////////////////////////////////////////////////////////////FIX THIS!
            // If we have more instances than room in our vertex buffer, grow it to the neccessary size.
            if ((instanceVertexBuffer == null) ||
                (amount > instanceVertexBuffer.VertexCount))
            {
                if (instanceVertexBuffer != null)
                    instanceVertexBuffer.Dispose();

                instanceVertexBuffer = new DynamicVertexBuffer(ScreenManager.Game.GraphicsDevice, instanceVertexDeclaration,
                                                               amount, BufferUsage.WriteOnly);
            }

            // Transfer the latest instance transform matrices into the instanceVertexBuffer.
            instanceVertexBuffer.SetData(towerTransforms, 0, amount, SetDataOptions.Discard);

            for (int i = 0; i < instancedModels[towerType][playerNum].Meshes.Count; i++)
            {
                for (int j = 0; j < instancedModels[towerType][playerNum].Meshes[i].MeshParts.Count; j++)
                {

                    // Tell the GPU to read from both the model vertex buffer plus our instanceVertexBuffer.
                    ScreenManager.Game.GraphicsDevice.SetVertexBuffers(
                        new VertexBufferBinding(instancedModels[towerType][playerNum].Meshes[i].MeshParts[j].VertexBuffer,
                            instancedModels[towerType][playerNum].Meshes[i].MeshParts[j].VertexOffset, 0),
                        new VertexBufferBinding(instanceVertexBuffer, 0, 1)
                    );

                    ScreenManager.Game.GraphicsDevice.Indices = instancedModels[towerType][playerNum].Meshes[i].MeshParts[j].IndexBuffer;

                    // Set up the instance rendering effect.
                    Effect effect = instancedModels[towerType][playerNum].Meshes[i].MeshParts[j].Effect;

                    effect.CurrentTechnique = effect.Techniques["HardwareInstancing"];

                    effect.Parameters["World"].SetValue(instancedModelBones[towerType][playerNum][instancedModels[towerType][playerNum].Meshes[i].ParentBone.Index]);
                    effect.Parameters["View"].SetValue(cameraManager.ViewMatrix);
                    effect.Parameters["Projection"].SetValue(cameraManager.ProjectionMatrix);

                    // Draw all the instance copies in a single call.
                    for (int k = 0; k < effect.CurrentTechnique.Passes.Count; k++)
                    {
                        effect.CurrentTechnique.Passes[k].Apply();

                        ScreenManager.Game.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                                instancedModels[towerType][playerNum].Meshes[i].MeshParts[j].NumVertices,
                                                                instancedModels[towerType][playerNum].Meshes[i].MeshParts[j].StartIndex,
                                                                instancedModels[towerType][playerNum].Meshes[i].MeshParts[j].PrimitiveCount,
                                                                amount);
                    }
                }
            }
        }
        #endregion
    }
}
