using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using UHSampleGame.ScreenManagement;
using UHSampleGame.CameraManagement;
using UHSampleGame.TileSystem;

namespace UHSampleGame.CoreObjects.Units
{
    static class UnitCollection
    {

        #region Class Variables
        const int MAX_UNITS = 5000;

        static int NumPlayers;
        static Enum[] unitTypes = EnumHelper.EnumToArray(new UnitType());

        //units[playerNum][unitType][index]
        static List<List<List<Unit2>>> units;

        //unitsCount[playerNum][unitType]
        static List<List<int>> unitsCount;

        static List<List<int>> unitsMaxIndex;

        //Instancing Verticies
        static VertexDeclaration instanceVertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 1),
            new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 2),
            new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 3)
        );

        //instancedModels[unitType]
        static List<Model> instancedModels;

        //instancedModelBones[unitType]
        static List<Matrix[]> instancedModelBones;

        //unitTransforms[playerNum][unitType][index]
        static Matrix[] unitTransforms;

        static DynamicVertexBuffer instanceVertexBuffer = null;

        static CameraManager cameraManager;

        static int updateCount;
        static int drawCount;
        #endregion

        public static void Initialize(int numPlayers)
        {
            cameraManager = (CameraManager)ScreenManager.Game.Services.GetService(typeof(CameraManager));
            NumPlayers = numPlayers;
            units = new List<List<List<Unit2>>>();
            unitsCount = new List<List<int>>();
            unitsMaxIndex = new List<List<int>>();
            unitTransforms = new Matrix[MAX_UNITS];

            for (int i = 0; i < numPlayers; i++)
            {
                units.Add(new List<List<Unit2>>());
                unitsCount.Add(new List<int>());
                unitsMaxIndex.Add(new List<int>());

                for (int j = 0; j < unitTypes.Length; j++)
                {
                    units[i].Add(new List<Unit2>());
                    unitsCount[i].Add(0);
                    unitsMaxIndex[i].Add(0);

                    for (int k = 0; k < MAX_UNITS; k++)
                    {
                        units[i][j].Add(new Unit2((UnitType)j));
                    }
                }
            }

            instancedModels = new List<Model>();
            instancedModelBones = new List<Matrix[]>();
            for (int j = 0; j < unitTypes.Length; j++)
            {
                switch ((UnitType)j)
                {
                    case UnitType.TestUnit:
                        instancedModels.Add(ScreenManagement.ScreenManager.Game.Content.Load<Model>("Objects\\Units\\enemyShip01"));
                        instancedModelBones.Add(new Matrix[instancedModels[j].Bones.Count]);
                        instancedModels[j].CopyAbsoluteBoneTransformsTo(instancedModelBones[j]);
                        break;
                }
            }


        }

        public static int AllUnitCount()
        {
            int sum = 0;

            for (int i = 0; i < NumPlayers; i++)
                for (int j = 0; j < unitTypes.Length; j++)
                    sum += unitsCount[i][j];

            return sum;
        }

        public static void Add(int playerNum, UnitType unitType)
        {
            //////////////////////////////////////////////////////////REFACTOR FOR EFFICIENCY
            for (int i = 0; i <= unitsMaxIndex[playerNum][(int)unitType]; i++)
            {
                if (!units[playerNum][(int)unitType][i].IsActive())
                {
                    units[playerNum][(int)unitType][i].Deploy(TileMap2.Tiles[0], TileMap2.Tiles[TileMap2.Tiles.Count-2]);
                    unitsCount[playerNum][(int)unitType]++;

                    if(i == unitsMaxIndex[playerNum][(int)unitType])
                        unitsMaxIndex[playerNum][(int)unitType]++;

                    break;
                }
            } 
        }

        public static void Remove()
        {

        }

        public static void Update(GameTime gameTime)
        {
            for (int i = 0; i < NumPlayers; i++)
                for (int j = 0; j < unitTypes.Length; j++)
                {
                    updateCount = 0;
                    for (int k = 0; k < unitsMaxIndex[i][j] && updateCount < unitsCount[i][j]; k++)
                        if (units[i][j][k].IsActive())
                        {
                            units[i][j][k].Update(gameTime);
                            updateCount++;
                        }
                }
        }

        public static void Draw(GameTime gameTime)
        {
            for (int i = 0; i < NumPlayers; i++)
            {
                for (int j = 0; j < unitTypes.Length; j++)
                {
                    DrawUnits(i, j);
                }
            }             

        }

        private static void DrawUnits(int i, int j)
        {
            drawCount = 0;
            for (int k = 0; k < unitsMaxIndex[i][j] && drawCount < unitsCount[i][j]; k++)
            {
                if (units[i][j][k].IsActive())
                {
                    unitTransforms[drawCount] = units[i][j][k].Transforms;
                    drawCount++;
                }
            }

            DrawInstancedUnits(i, j, drawCount);

        }

       

        private static void DrawInstancedUnits(int playerNum, int unitType, int amount)
        {
            //int amount = units[playerNum][unitType].Count;

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
            instanceVertexBuffer.SetData(unitTransforms, 0, amount, SetDataOptions.Discard);

            for (int i = 0; i < instancedModels[unitType].Meshes.Count; i++)
            {
                for(int j = 0; j < instancedModels[unitType].Meshes[i].MeshParts.Count; j++)
                {
                    // Tell the GPU to read from both the model vertex buffer plus our instanceVertexBuffer.
                    ScreenManager.Game.GraphicsDevice.SetVertexBuffers(
                        new VertexBufferBinding(instancedModels[unitType].Meshes[i].MeshParts[j].VertexBuffer, 
                            instancedModels[unitType].Meshes[i].MeshParts[j].VertexOffset, 0),
                        new VertexBufferBinding(instanceVertexBuffer, 0, 1)
                    );

                    ScreenManager.Game.GraphicsDevice.Indices = instancedModels[unitType].Meshes[i].MeshParts[j].IndexBuffer;

                    // Set up the instance rendering effect.
                    Effect effect = instancedModels[unitType].Meshes[i].MeshParts[j].Effect;

                    effect.CurrentTechnique = effect.Techniques["HardwareInstancing"];

                    effect.Parameters["World"].SetValue(instancedModelBones[unitType][instancedModels[unitType].Meshes[i].ParentBone.Index]);
                    effect.Parameters["View"].SetValue(cameraManager.ViewMatrix);
                    effect.Parameters["Projection"].SetValue(cameraManager.ProjectionMatrix);

                    // Draw all the instance copies in a single call.
                    for (int k = 0; k < effect.CurrentTechnique.Passes.Count; k++)
                    {
                        effect.CurrentTechnique.Passes[k].Apply();

                        ScreenManager.Game.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                                instancedModels[unitType].Meshes[i].MeshParts[j].NumVertices,
                                                                instancedModels[unitType].Meshes[i].MeshParts[j].StartIndex,
                                                                instancedModels[unitType].Meshes[i].MeshParts[j].PrimitiveCount,
                                                                amount);
                    }
                }
            }
        }
    }
}
