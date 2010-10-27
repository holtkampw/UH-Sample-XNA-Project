﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using UHSampleGame.ScreenManagement;
using UHSampleGame.CameraManagement;
using UHSampleGame.TileSystem;

namespace UHSampleGame.CoreObjects.Towers
{
    public class TowerCollection
    {

        #region Class Variables

        const int MAX_TOWERS = 100;

        static int NumPlayers;
        static Enum[] towerTypes = EnumHelper.EnumToArray(new TowerType());

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

        //instancedModels[unitType]
        static List<Model> instancedModels;

        //instancedModelBones[unitType]
        static List<Matrix[]> instancedModelBones;

        //unitTransforms[playerNum][unitType][index]
        static Matrix[] towerTransforms;

        static DynamicVertexBuffer instanceVertexBuffer = null;

        static CameraManager cameraManager;

        static int updateCount;
        static int drawCount;
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

            instancedModels = new List<Model>();
            instancedModelBones = new List<Matrix[]>();
            for (int j = 0; j < towerTypes.Length; j++)
            {
                switch ((TowerType)j)
                {
                    case TowerType.TowerA:
                        instancedModels.Add(ScreenManagement.ScreenManager.Game.Content.Load<Model>("Objects\\Towers\\towerA_red"));
                        instancedModelBones.Add(new Matrix[instancedModels[j].Bones.Count]);
                        instancedModels[j].CopyAbsoluteBoneTransformsTo(instancedModelBones[j]);
                        break;
                }
            }


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
                return 0 ;

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
            }

        }

        private static void DrawTowers(int i, int j)
        {
            drawCount = 0;
            for (int k = 0; k < MAX_TOWERS && drawCount < towerCount[i][j]; k++)
            {
                if (towers[i][j][k].IsActive())
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

            for (int i = 0; i < instancedModels[towerType].Meshes.Count; i++)
            {
                for (int j = 0; j < instancedModels[towerType].Meshes[i].MeshParts.Count; j++)
                {

                    // Tell the GPU to read from both the model vertex buffer plus our instanceVertexBuffer.
                    ScreenManager.Game.GraphicsDevice.SetVertexBuffers(
                        new VertexBufferBinding(instancedModels[towerType].Meshes[i].MeshParts[j].VertexBuffer,
                            instancedModels[towerType].Meshes[i].MeshParts[j].VertexOffset, 0),
                        new VertexBufferBinding(instanceVertexBuffer, 0, 1)
                    );

                    ScreenManager.Game.GraphicsDevice.Indices = instancedModels[towerType].Meshes[i].MeshParts[j].IndexBuffer;

                    // Set up the instance rendering effect.
                    Effect effect = instancedModels[towerType].Meshes[i].MeshParts[j].Effect;

                    effect.CurrentTechnique = effect.Techniques["HardwareInstancing"];

                    effect.Parameters["World"].SetValue(instancedModelBones[towerType][instancedModels[towerType].Meshes[i].ParentBone.Index]);
                    effect.Parameters["View"].SetValue(cameraManager.ViewMatrix);
                    effect.Parameters["Projection"].SetValue(cameraManager.ProjectionMatrix);

                    // Draw all the instance copies in a single call.
                    for (int k = 0; k < effect.CurrentTechnique.Passes.Count; k++)
                    {
                        effect.CurrentTechnique.Passes[k].Apply();

                        ScreenManager.Game.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                                instancedModels[towerType].Meshes[i].MeshParts[j].NumVertices,
                                                                instancedModels[towerType].Meshes[i].MeshParts[j].StartIndex,
                                                                instancedModels[towerType].Meshes[i].MeshParts[j].PrimitiveCount,
                                                                amount);
                    }
                }
            }
        }
        #endregion
    }
}
