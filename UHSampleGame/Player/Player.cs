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

namespace UHSampleGame.Player
{
    public enum InstancingTechnique
    {
        HardwareInstancing,
        NoInstancing,
        NoInstancingOrStateBatching
    }

    public abstract class Player
    {
        static VertexDeclaration instanceVertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 1),
            new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 2),
            new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 3)
        );

        protected Base playerBase;
        protected List<Tower> towers;
        protected Dictionary<UnitType, List<Unit>> units;
        protected Dictionary<UnitType, int> previousCount;
        protected Dictionary<UnitType, Model> instancedModels;
        protected Dictionary<UnitType, Matrix[]> instancedModelBones;
        protected Enum[] unitTypes;

        protected int money;
        protected int playerNum;
        protected int teamNum;

        protected SkinnedEffect genericEffect;
        protected CameraManager cameraManager;

        InstancingTechnique instancingTechnique = InstancingTechnique.HardwareInstancing;

        //Transforms
        Dictionary<UnitType, List<Matrix>> unitTransforms;

        public event GetNewGoalBase GetNewGoalBase;

        public int PlayerNum
        {
            get { return playerNum; }
        }

        public int TeamNum
        {
            get { return teamNum; }
        }

        public Base Base
        {
            get { return playerBase; }
        }

        public int TowerCount
        {
            get { return towers.Count; }
        }

        public int UnitCount
        {
            get {
                int count = 0;
                foreach (UnitType key in unitTypes)
                {
                    count += units[key].Count;
                }
                return count; 
            }
        }

        public Player(int playerNum, int teamNum, Tile startTile)
        {
            this.unitTypes = EnumHelper.EnumToArray(new UnitType());
            this.playerNum = playerNum;
            this.teamNum = teamNum;
            this.playerBase = new TestBase(playerNum, teamNum, startTile);
            SetBase(new TestBase(playerNum, teamNum, startTile));
            this.towers = new List<Tower>();
            this.units = new Dictionary<UnitType, List<Unit>>();
            previousCount = new Dictionary<UnitType, int>();
            unitTransforms = new Dictionary<UnitType, List<Matrix>>();
            instancedModels = new Dictionary<UnitType, Model>();
            instancedModelBones = new Dictionary<UnitType, Matrix[]>();
            foreach (UnitType key in unitTypes)
            {
                units[key] = new List<Unit>();
                previousCount[key] = 0;
                unitTransforms[key] = new List<Matrix>();

                switch (key)
                {
                    case UnitType.TestUnit:
                        instancedModels[key] = TestUnit.Model;
                        instancedModelBones[key] = new Matrix[instancedModels[key].Bones.Count];
                        instancedModels[key].CopyAbsoluteBoneTransformsTo(instancedModelBones[key]);
                        break;
                }
            }
            this.money = 0;
            this.cameraManager = (CameraManager)ScreenManager.Game.Services.GetService(typeof(CameraManager));
            this.genericEffect = new SkinnedEffect(ScreenManager.Game.GraphicsDevice);         
        }

        public void SetBase(Base playerBase)
        {
            this.playerBase = playerBase;
           // TileMap.SetBase(playerBase);
        }

        public void SetTargetBase(Base target)
        {
            playerBase.SetGoalBase(target);
            target.baseDestroyed += GetNewTargetBase;
        }

        protected void GetNewTargetBase(Base destroyedBase)
        {
            if (GetNewGoalBase != null)
                GetNewGoalBase();
        }

        public virtual void HandleInput(InputManager input)
        {
           
        }

        public void SetTowerForLevelMap(Tower tower)
        {
            TileMap.SetTowerForLevelMap(tower, tower.Tile);
            towers.Add(tower);
        }

        protected void BuildTower(Tile tile)
        {
            Tower tower = new TowerAGood(playerNum, teamNum, tile);
            if (TileMap.SetTower(tower, tile))
                towers.Add(tower);
        }

        protected void AddUnit(UnitType type, Unit unit)
        {
            unit.Died += RemoveUnit;
            units[type].Add(unit);
            TileMap.TowerCreated += unit.UpdatePath;
        }

        protected void RemoveUnit(UnitType type, Unit unit)
        {
            units[type].Remove(unit);
            TileMap.TowerCreated -= unit.UpdatePath;
        }

        public virtual void Update(GameTime gameTime)
        {
            playerBase.Update(gameTime);

            for (int i = 0; i < towers.Count; i++)
                towers[i].Update(gameTime);

            foreach (UnitType key in unitTypes)
                for (int i = 0; i < units[key].Count; i++)
                    units[key][i].Update(gameTime);
        }

        public virtual void Draw(GameTime gameTime)
        {
            playerBase.Draw(gameTime);

            for (int i = 0; i < towers.Count; i++)
                towers[i].Draw(gameTime);

            foreach (UnitType key in unitTypes)
            {
                bool clear = false;
                if (units[key].Count != previousCount[key])
                {
                    ClearTransforms(key);
                    clear = true;
                }
                previousCount[key] = units[key].Count;

                for (int i = 0; i < units[key].Count; i++)
                {
                    if(clear)
                        unitTransforms[key].Add(units[key][i].Transforms);
                    else
                        unitTransforms[key][i] = units[key][i].Transforms;
                } 

            }

            DrawUnits(gameTime);
        }

        private void ClearTransforms(UnitType type)
        {
           unitTransforms[type].Clear();
        }

        private void DrawUnits(GameTime gameTime)
        {
            // Draw all the instances, using the currently selected rendering technique.
            foreach (UnitType key in unitTypes)
            {
                Matrix[] transforms = new Matrix[units[key].Count];
                for(int i =0 ; i < units[key].Count; i++) {
                    transforms[i] = units[key][i].Transforms;
                }

                switch (instancingTechnique)
                {
                    case InstancingTechnique.HardwareInstancing:
                        DrawModelHardwareInstancing(instancedModels[key], instancedModelBones[key],
                                                    transforms, cameraManager.ViewMatrix, cameraManager.ProjectionMatrix);
                        break;

                    case InstancingTechnique.NoInstancing:
                        DrawModelNoInstancing(instancedModels[key], instancedModelBones[key],
                                              transforms, cameraManager.ViewMatrix, cameraManager.ProjectionMatrix);
                        break;

                    case InstancingTechnique.NoInstancingOrStateBatching:
                        DrawModelNoInstancingOrStateBatching(instancedModels[key], instancedModelBones[key],
                                              transforms, cameraManager.ViewMatrix, cameraManager.ProjectionMatrix);
                        break;
                }
            }
        }

        #region Instancing Helpers
        /// <summary>
        /// Efficiently draws several copies of a piece of geometry using hardware instancing.
        /// </summary>
        void DrawModelHardwareInstancing(Model model, Matrix[] modelBones,
                                         Matrix[] instances, Matrix view, Matrix projection)
        {
            DynamicVertexBuffer instanceVertexBuffer = null;
            if (instances.Length == 0)
                return;

            // If we have more instances than room in our vertex buffer, grow it to the neccessary size.
            if ((instanceVertexBuffer == null) ||
                (instances.Length > instanceVertexBuffer.VertexCount))
            {
                if (instanceVertexBuffer != null)
                    instanceVertexBuffer.Dispose();

                instanceVertexBuffer = new DynamicVertexBuffer(ScreenManager.Game.GraphicsDevice, instanceVertexDeclaration,
                                                               instances.Length, BufferUsage.WriteOnly);
            }

            // Transfer the latest instance transform matrices into the instanceVertexBuffer.
            instanceVertexBuffer.SetData(instances, 0, instances.Length, SetDataOptions.Discard);

            foreach (ModelMesh mesh in model.Meshes)
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

                    effect.Parameters["World"].SetValue(modelBones[mesh.ParentBone.Index]);
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);

                    // Draw all the instance copies in a single call.
                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        ScreenManager.Game.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                               meshPart.NumVertices, meshPart.StartIndex,
                                                               meshPart.PrimitiveCount, instances.Length);
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
