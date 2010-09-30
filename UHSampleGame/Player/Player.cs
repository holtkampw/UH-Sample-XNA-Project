using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using UHSampleGame.CoreObjects;
using UHSampleGame.CoreObjects.Base;
using UHSampleGame.CoreObjects.Towers;
using UHSampleGame.CoreObjects.Units;
using UHSampleGame.TileSystem;
using UHSampleGame.InputManagement;
using UHSampleGame.ScreenManagement;
using UHSampleGame.CameraManagement;
using UHSampleGame.Events;

namespace UHSampleGame.Players
{
    public enum InstancingTechnique
    {
        HardwareInstancing,
        NoInstancing,
        NoInstancingOrStateBatching
    }

    public enum PlayerType { Human, AI };

    public class Player
    {

        static int MAX_UNITS = 5000;
        static VertexDeclaration instanceVertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 1),
            new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 2),
            new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 3)
        );

        public Base PlayerBase;
        protected List<Tower> towers;
        protected Dictionary<int, List<Unit>> units;
        protected Dictionary<int, int> unitCount;
        protected Dictionary<int, Model> instancedModels;
        protected Dictionary<int, Matrix[]> instancedModelBones;
        public static Enum[] unitEnumType = EnumHelper.EnumToArray(new UnitType());

        protected int money;
        protected int PlayerNum;
        protected int TeamNum;

        protected SkinnedEffect genericEffect;
        protected CameraManager cameraManager;
        DynamicVertexBuffer instanceVertexBuffer = null;

        InstancingTechnique instancingTechnique = InstancingTechnique.HardwareInstancing;

        public event GetNewGoalBase GetNewGoalBase;

        Matrix[] universalTransforms = new Matrix[5000];

        public PlayerType Type;
        public static Enum[] playerEnumType = EnumHelper.EnumToArray(new PlayerType());

        //HumanPlayer
        TeamableAnimatedObject avatar;

        int updateCount = 0;
        #region Properties
        public int TowerCount
        {
            get { return towers.Count; }
        }

        public int UnitCount
        {
            get
            {
                int count = 0;
                for (int i = 0; i < unitEnumType.Length; i++ )
                {
                    
                    count += unitCount[i];
                }
                return count;
            }
        }
        #endregion

        public Player(int playerNum, int teamNum, Tile startTile, PlayerType type)
        {
            this.PlayerNum = playerNum;
            this.TeamNum = teamNum;
            this.PlayerBase = new TestBase(playerNum, teamNum, startTile);
            SetBase(new TestBase(playerNum, teamNum, startTile));
            this.towers = new List<Tower>();
            this.units = new Dictionary<int, List<Unit>>();
            instancedModels = new Dictionary<int, Model>();
            instancedModelBones = new Dictionary<int, Matrix[]>();
            unitCount = new Dictionary<int, int>();
            for (int i = 0; i < unitEnumType.Length; i++)
            {
                units[i] = new List<Unit>();
                for (int j = 0; j < MAX_UNITS; j++)
                {
                    units[i].Add(new Unit());
                }

                unitCount[i] = 0;

                switch ((UnitType)unitEnumType[i])
                {
                    case UnitType.TestUnit:
                        instancedModels[i] = Unit.Models[i];
                        instancedModelBones[i] = new Matrix[instancedModels[i].Bones.Count];
                        instancedModels[i].CopyAbsoluteBoneTransformsTo(instancedModelBones[i]);
                        break;
                }
            }

            this.money = 0;
            this.cameraManager = (CameraManager)ScreenManager.Game.Services.GetService(typeof(CameraManager));

            //HumanPlayer
            if (Type == PlayerType.Human)
            {
                avatar = new TeamableAnimatedObject(playerNum, teamNum,
                    ScreenManager.Game.Content.Load<Model>("AnimatedModel\\dude"));

                avatar.Scale = 2.0f;
                avatar.PlayClip("Take 001");
                avatar.SetPosition(PlayerBase.Position);
            }
        }

        public void SetBase(Base playerBase)
        {
            this.PlayerBase = playerBase;
           // TileMap.SetBase(playerBase);
        }

        public void SetTargetBase(Base target)
        {
            PlayerBase.SetGoalBase(target);
            target.baseDestroyed += GetNewTargetBase;
        }

        protected void GetNewTargetBase(Base destroyedBase)
        {
            if (GetNewGoalBase != null)
                GetNewGoalBase();
        }

        public void HandleInput(InputManager input)
        {
            //Human Player
            if (Type == PlayerType.Human)
            {
                if (input.CheckAction(InputAction.TileMoveUp))
                    avatar.SetPosition(avatar.Position + new Vector3(0, 0, -3));

                if (input.CheckAction(InputAction.TileMoveDown))
                    avatar.SetPosition(avatar.Position + new Vector3(0, 0, 3));

                if (input.CheckAction(InputAction.TileMoveLeft))
                    avatar.SetPosition(avatar.Position + new Vector3(-3, 0, 0));

                if (input.CheckAction(InputAction.TileMoveRight))
                    avatar.SetPosition(avatar.Position + new Vector3(3, 0, 0));

                if (avatar.Position.X < TileMap.Left)
                    avatar.SetPosition(new Vector3(TileMap.Left, avatar.Position.Y, avatar.Position.Z));

                if (avatar.Position.Z < TileMap.Top)
                    avatar.SetPosition(new Vector3(avatar.Position.X, avatar.Position.Y, TileMap.Top));

                if (avatar.Position.X > TileMap.Right)
                    avatar.SetPosition(new Vector3(TileMap.Right, avatar.Position.Y, avatar.Position.Z));

                if (avatar.Position.Z > TileMap.Bottom)
                    avatar.SetPosition(new Vector3(avatar.Position.X, avatar.Position.Y, TileMap.Bottom));

                if (input.CheckAction(InputAction.Selection))
                    AddUnit(UnitType.TestUnit, 1, 1, PlayerBase.Position, PlayerBase.GoalBase);

                if (input.CheckNewAction(InputAction.TowerBuild))
                    BuildTower(TileMap.GetTileFromPos(avatar.Position));
            }
        }

        public void SetTowerForLevelMap(Tower tower)
        {
            TileMap.SetTowerForLevelMap(tower, tower.Tile);
            towers.Add(tower);
        }

        protected void BuildTower(Tile tile)
        {
            Tower tower = new Tower(TowerType.TowerA, PlayerNum, TeamNum, tile);
            if (TileMap.SetTower(tower, tile))
                towers.Add(tower);
        }

        protected void AddUnit(UnitType type, int playerNum, int teamNum, Vector3 position, Base goalBase)
        {
            if (unitCount[(int)type] >= MAX_UNITS - 1)
                return;

            for(int i = 0; i < units[(int)type].Count; i++)
            {
                if(!units[(int)type][i].Alive)
                {
                    units[(int)type][i].Setup(type, i, playerNum, teamNum, position, goalBase);
                    units[(int)type][i].Died += RemoveUnit;
                    TileMap.TowerCreated += units[(int)type][i].UpdatePath;
                    unitCount[(int)type] += 1;
                    return;
                }
            }
            //unit.Died += RemoveUnit;
            //units[(int)type].Add(unit);
            //TileMap.TowerCreated += unit.UpdatePath;
        }

        protected void RemoveUnit(UnitType type, Unit unit)
        {
            //units[(int)type].Remove(unit);
            units[(int)type][unit.Index].Alive = false;
            unitCount[(int)type] -= 1;
            TileMap.TowerCreated -= unit.UpdatePath;
        }

        public void Update(GameTime gameTime)
        {
            PlayerBase.Update(gameTime);

            for (int i = 0; i < towers.Count; i++)
                towers[i].Update(gameTime);


            for (int key = 0; key < unitEnumType.Length; key++)
            {
                updateCount = 0;
                for (int i = 0; i < units[key].Count; i++)
                {
                    if (units[key][i].Alive)
                    {
                        units[key][i].Update(gameTime);
                        updateCount++;
                    }

                    if (updateCount == unitCount[key])
                    {
                        break;
                    }
                }
            }

            //HumanPlayer
            if (Type == PlayerType.Human)
            {
                avatar.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime)
        {
            PlayerBase.Draw(gameTime);

            for (int i = 0; i < towers.Count; i++)
                towers[i].Draw(gameTime);

            DrawUnits(gameTime);

            //HumanPlayer
            if (Type == PlayerType.Human)
            {
                avatar.Draw(gameTime);
            }
        }

        private void DrawUnits(GameTime gameTime)
        {
            // Draw all the instances, using the currently selected rendering technique.
            for (int key = 0; key < unitEnumType.Length; key++)
            {
                for (int i = 0; i < units[key].Count; i++)
                {
                    if(units[key][i].Alive)
                        universalTransforms[i] = units[key][i].Transforms;
                }
                DrawModelHardwareInstancing(key);
                //switch (instancingTechnique)
                //{
                //    case InstancingTechnique.HardwareInstancing:
                //        DrawModelHardwareInstancing(int key);
                //        break;

                //    //case InstancingTechnique.NoInstancing:
                //    //    DrawModelNoInstancing(instancedModels[key], instancedModelBones[key],
                //    //                          transforms, cameraManager.ViewMatrix, cameraManager.ProjectionMatrix);
                //    //    break;

                //    //case InstancingTechnique.NoInstancingOrStateBatching:
                //    //    DrawModelNoInstancingOrStateBatching(instancedModels[key], instancedModelBones[key],
                //    //                          transforms, cameraManager.ViewMatrix, cameraManager.ProjectionMatrix);
                //    //    break;
                //}
            }
        }

        #region Instancing Helpers
        /// <summary>
        /// Efficiently draws several copies of a piece of geometry using hardware instancing.
        /// </summary>
        void DrawModelHardwareInstancing(int key)
        {
            int amount = units[key].Count;
            
            if (amount == 0)
                return;

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
            instanceVertexBuffer.SetData(universalTransforms, 0, amount, SetDataOptions.Discard);

            foreach (ModelMesh mesh in instancedModels[key].Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    // Tell the GPU to read from both the model vertex buffer plus our instanceVertexBuffer.
                    ScreenManager.Game.GraphicsDevice.SetVertexBuffers(
                        new VertexBufferBinding(meshPart.VertexBuffer, meshPart.VertexOffset, 0),
                        new VertexBufferBinding(instanceVertexBuffer, 0, 1)
                    );

                    ScreenManager.Game.GraphicsDevice.Indices = meshPart.IndexBuffer;

                    // Set up the instance rendering effect.
                    Effect effect = meshPart.Effect;

                    effect.CurrentTechnique = effect.Techniques["HardwareInstancing"];

                    effect.Parameters["World"].SetValue(instancedModelBones[key][mesh.ParentBone.Index]);
                    effect.Parameters["View"].SetValue(cameraManager.ViewMatrix);
                    effect.Parameters["Projection"].SetValue(cameraManager.ProjectionMatrix);

                    // Draw all the instance copies in a single call.
                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        ScreenManager.Game.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                               meshPart.NumVertices, meshPart.StartIndex,
                                                               meshPart.PrimitiveCount, amount);
                    }
                }
            }
        }


        /// <summary>
        /// Draws several copies of a piece of geometry without using any
        /// special GPU instancing techniques at all. This just does a
        /// regular loop and issues several draw calls one after another.
        /// </summary>
        void DrawModelNoInstancing(Model model, Matrix[] modelBones,
                                   Matrix[] instances, Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    ScreenManager.Game.GraphicsDevice.SetVertexBuffer(meshPart.VertexBuffer, meshPart.VertexOffset);
                    ScreenManager.Game.GraphicsDevice.Indices = meshPart.IndexBuffer;

                    // Set up the rendering effect.
                    Effect effect = meshPart.Effect;

                    effect.CurrentTechnique = effect.Techniques["NoInstancing"];

                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);

                    EffectParameter transformParameter = effect.Parameters["World"];

                    // Draw a single instance copy each time around this loop.
                    for (int i = 0; i < instances.Length; i++)
                    {
                        transformParameter.SetValue(modelBones[mesh.ParentBone.Index] * instances[i]);

                        foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                        {
                            pass.Apply();

                            ScreenManager.Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                                 meshPart.NumVertices, meshPart.StartIndex,
                                                                 meshPart.PrimitiveCount);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// This technique is NOT a good idea! It is only included in the sample
        /// for comparison purposes, so you can compare its performance with the
        /// other more sensible approaches. This uses the exact same shader code
        /// as the preceding NoInstancing technique, but with a key difference.
        /// Where the NoInstancing technique worked like this:
        /// 
        ///     SetRenderStates()
        ///     foreach instance
        ///     {
        ///         Update effect with per-instance transform matrix
        ///         DrawIndexedPrimitives()
        ///     }
        /// 
        /// NoInstancingOrStateBatching works like so:
        /// 
        ///     foreach instance
        ///     {
        ///         Set per-instance transform matrix into the effect
        ///         SetRenderStates()
        ///         DrawIndexedPrimitives()
        ///     }
        ///      
        /// As you can see, this is repeatedly setting the same renderstates.
        /// Not so efficient.
        /// 
        /// In other words, the built-in Model.Draw method is pretty inefficient when
        /// it comes to drawing more than one instance! Even without using any fancy
        /// shader techniques, you can get a significant speed boost just by rearranging
        /// your drawing code to work more like the earlier NoInstancing technique.
        /// </summary>
        void DrawModelNoInstancingOrStateBatching(Model model, Matrix[] modelBones,
                                                  Matrix[] instances, Matrix view, Matrix projection)
        {
            for (int i = 0; i < instances.Length; i++)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (Effect effect in mesh.Effects)
                    {
                        effect.CurrentTechnique = effect.Techniques["NoInstancing"];

                        effect.Parameters["World"].SetValue(modelBones[mesh.ParentBone.Index] * instances[i]);
                        effect.Parameters["View"].SetValue(view);
                        effect.Parameters["Projection"].SetValue(projection);
                    }

                    mesh.Draw();
                }
            }
        }

        #endregion
    }
}
