using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using UHSampleGame.ScreenManagement;
using UHSampleGame.CameraManagement;
using UHSampleGame.TileSystem;
using UHSampleGame.CoreObjects.Base;
using UHSampleGame.ProjectileManagment;
using UHSampleGame.Players;

namespace UHSampleGame.CoreObjects.Units
{

    static class UnitCollection
    {

        #region Class Variables
        const int MAX_UNITS = 2000;
        static int UnitCounter;

        static int NumPlayers;
        static Enum[] unitTypes = EnumHelper.EnumToArray(new UnitType());

        //units[playerNum][unitType][index]
        static List<List<List<Unit>>> units;

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

        //public static List<String> unitCountForPlayerString;
        public static List<int> unitCountForPlayer;
        public static List<List<int>> unitDeployedForPlayer;
        #endregion

        public static void Dispose()
        {
            units = null;
            unitsCount = null;
            unitsMaxIndex = null;
            unitTransforms = null;
            unitCountForPlayer = null;
            unitDeployedForPlayer = null;
            instancedModelBones = null;
            instancedModels = null;
            GC.Collect();
        }

        public static void Initialize(int numPlayers)
        {
            cameraManager = (CameraManager)ScreenManager.Game.Services.GetService(typeof(CameraManager));
            NumPlayers =   numPlayers;
            units = new List<List<List<Unit>>>();
            unitsCount = new List<List<int>>();
            unitsMaxIndex = new List<List<int>>();
            unitTransforms = new Matrix[MAX_UNITS];
            //unitCountForPlayerString = new List<string>();
            unitCountForPlayer = new List<int>();
            unitDeployedForPlayer = new List<List<int>>();

            for (int i = 0; i < numPlayers; i++)
            {
                units.Add(new List<List<Unit>>());
                unitsCount.Add(new List<int>());
                unitsMaxIndex.Add(new List<int>());
                //unitCountForPlayerString.Add("0");
                unitCountForPlayer.Add(0);
                unitDeployedForPlayer.Add(new List<int>());

                for (int j = 0; j < unitTypes.Length; j++)
                {
                    units[i].Add(new List<Unit>());
                    unitsCount[i].Add(0);
                    unitsMaxIndex[i].Add(0);
                    unitDeployedForPlayer[i].Add(0);

                    for (int k = 0; k < MAX_UNITS; k++)
                    {
                        units[i][j].Add(new Unit((UnitType)j));
                        unitDeployedForPlayer[i].Add(0);
                    }
                }
            }

            instancedModels = new List<Model>();
            instancedModelBones = new List<Matrix[]>();
            for (int j = 0; j < unitTypes.Length; j++)
            {
                switch ((UnitType)j)
                {
                    case UnitType.SpeedBoat:
                        instancedModels.Add(ScreenManagement.ScreenManager.Game.Content.Load<Model>("Objects\\Units\\speedBoat01_BLUE"));//"Objects\\Units\\enemyShip01"));
                        instancedModelBones.Add(new Matrix[instancedModels[j].Bones.Count]);
                        instancedModels[j].CopyAbsoluteBoneTransformsTo(instancedModelBones[j]);
                        break;
                    case UnitType.SpeederBoat:
                        instancedModels.Add(ScreenManagement.ScreenManager.Game.Content.Load<Model>("Objects\\Units\\speederBoat02_BLUE"));//"Objects\\Units\\enemyShip01"));
                        instancedModelBones.Add(new Matrix[instancedModels[j].Bones.Count]);
                        instancedModels[j].CopyAbsoluteBoneTransformsTo(instancedModelBones[j]);
                        break;
                }
            }


        }
        public static int TotalPossibleUnitCount()
        {
            return MAX_UNITS * NumPlayers * unitTypes.Length;
        }

        public static Unit GetUnitByID(int id)
        {
            return units[id / (unitTypes.Length * NumPlayers *MAX_UNITS)][(id / MAX_UNITS) % unitTypes.Length][id % MAX_UNITS];
        }

        public static int AllUnitCount()
        {
            UnitCounter = 0;

            for (int i = 0; i < NumPlayers; i++)
                for (int j = 0; j < unitTypes.Length; j++)
                    UnitCounter += unitsCount[i][j];

            return UnitCounter;
        }

        public static void Build(int playerNum, int teamNum, UnitType unitType)
        {
            Unit u;
            //////////////////////////////////////////////////////////REFACTOR FOR EFFICIENCY
            for (int i = 0; i <= unitsMaxIndex[playerNum][(int)unitType]; i++)
            {
                u = units[playerNum][(int)unitType][i];
                if (!u.IsActive() && !u.IsDeployed())
                {
                    u.Activate();

                    u.PlayerNum = playerNum;
                    u.TeamNum = teamNum;

                    unitCountForPlayer[playerNum]++;
                    //unitCountForPlayerString[playerNum] = unitCountForPlayer[playerNum].ToString();

                    unitsCount[playerNum][(int)unitType]++;

                    if (i == unitsMaxIndex[playerNum][(int)unitType] && i < MAX_UNITS - 1)
                        unitsMaxIndex[playerNum][(int)unitType]++;

                    break;
                }
            }
        }

        internal static void SetAllUnitsImmovable(int playerNum)
        {
            Unit u;
            for (int j = 0; j < unitTypes.Length; j++)
            {
                for (int i = 0; i < MAX_UNITS; i++)
                {

                    u = units[playerNum][j][i];
                    if (u.IsDeployed())
                    {
                        u.Status = UnitStatus.Immovable;
                    }
                }
            }
        }


        internal static bool Destroy(int playerNum)
        {
            Unit u;
            int count = 0;
            int maxCount = 3;
            Vector3 nv = new Vector3();
            for (int j = 0; j < unitTypes.Length; j++)
            {
                for (int i = 0; i < MAX_UNITS; i++)
                {
                    u = units[playerNum][j][i];
                    if (u.IsDeployed() || u.Status == UnitStatus.Immovable)
                    {
                        u.Status = UnitStatus.Inactive;
                        nv.X = u.Position.X;
                        nv.Y = u.Position.Y + 5;
                        nv.Z = u.Position.Z;
                        ProjectileManager.AddParticle(u.Position, nv);
                        count++;
                        unitDeployedForPlayer[playerNum][j]--;
                        if (count >= maxCount)
                            return false;
                    }
                    else if (u.Status == UnitStatus.Active)
                        u.Status = UnitStatus.Inactive;
                }
            }
            return true;
        }

        public static void Deploy(int playerNum, int teamNum, int attackPlayerNum, UnitType unitType)
        {
            Unit u;
            //////////////////////////////////////////////////////////REFACTOR FOR EFFICIENCY
            for (int i = 0; i < MAX_UNITS /* unitsMaxIndex[playerNum][(int)unitType]*/; i++)
            {
                u = units[playerNum][(int)unitType][i];
                if (u.IsActive())
                {
                    u.Deploy(BaseCollection.GetBaseTileForPlayer(playerNum),
                        BaseCollection.GetBaseTileForPlayer(attackPlayerNum), attackPlayerNum);

                    u.PlayerNum = playerNum;
                    u.TeamNum = teamNum;
                    unitDeployedForPlayer[playerNum][(int)unitType]++;

                    // unitCountForPlayer[playerNum]++;
                    //unitCountForPlayerString[playerNum] = unitCountForPlayer[playerNum].ToString();

                    // unitsCount[playerNum][(int)unitType]++;

                    //if (i == unitsMaxIndex[playerNum][(int)unitType] && i <MAX_UNITS-1)
                    //     unitsMaxIndex[playerNum][(int)unitType]++;

                    break;
                }
            }
        }

        public static void Remove(ref Unit unit)
        {
            unitsCount[unit.PlayerNum][(int)unit.Type]--;
            unit.Status = UnitStatus.Inactive;
            unitCountForPlayer[unit.PlayerNum]--;
            unitDeployedForPlayer[unit.PlayerToAttack][(int)unit.Type]--;
            //unitCountForPlayerString[unit.PlayerNum] = unitCountForPlayer[unit.PlayerNum].ToString();
        }

        public static void Update(GameTime gameTime)
        {
            Unit u;
            int uCount;
            for (int i = 0; i < NumPlayers; i++)
                for (int j = 0; j < unitTypes.Length; j++)
                {
                    updateCount = 0;
                    uCount = unitsCount[i][j];
                    for (int k = 0; k < MAX_UNITS && updateCount < uCount; k++)
                    {
                        u = units[i][j][k];
                        if (u.Status == UnitStatus.Deployed)
                        {
                            if (u.Update(gameTime))
                                return;
                            updateCount++;
                        }
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
            int uCount = unitsCount[i][j];
            int uMaxIndex = unitsMaxIndex[i][j];
            Unit u;
            for (int k = 0; k < MAX_UNITS && drawCount < uCount; k++)
            {
                u = units[i][j][k];
                if (u.Status == UnitStatus.Deployed || u.Status == UnitStatus.Immovable)
                {
                    unitTransforms[drawCount] = u.Transforms;
                    drawCount++;
                }
            }

            DrawInstancedUnits(i, j, drawCount);

        }



        private static void DrawInstancedUnits(int playerNum, int unitType, int amount)
        {

            if (amount == 0)
                return;
            if (instanceVertexBuffer != null)
                instanceVertexBuffer.Dispose();

            instanceVertexBuffer = null;// If we have more instances than room in our vertex buffer, grow it to the neccessary size.
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

            // Set up the instance rendering effect.
            Effect effect = instancedModels[unitType].Meshes[0].MeshParts[0].Effect;
            effect.CurrentTechnique = effect.Techniques["HardwareInstancing"];
            effect.Parameters["View"].SetValue(cameraManager.ViewMatrix);
            effect.Parameters["Projection"].SetValue(cameraManager.ProjectionMatrix);

            for (int i = 0; i < instancedModels[unitType].Meshes.Count; i++)
            {
                for (int j = 0; j < instancedModels[unitType].Meshes[i].MeshParts.Count; j++)
                {
                    // Tell the GPU to read from both the model vertex buffer plus our instanceVertexBuffer.
                    ScreenManager.Game.GraphicsDevice.SetVertexBuffers(
                        new VertexBufferBinding(instancedModels[unitType].Meshes[i].MeshParts[j].VertexBuffer,
                            instancedModels[unitType].Meshes[i].MeshParts[j].VertexOffset, 0),
                        new VertexBufferBinding(instanceVertexBuffer, 0, 1)
                    );

                    ScreenManager.Game.GraphicsDevice.Indices = instancedModels[unitType].Meshes[i].MeshParts[j].IndexBuffer;

                    effect.Parameters["World"].SetValue(instancedModelBones[unitType][instancedModels[unitType].Meshes[i].ParentBone.Index]);

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

        public static int UnitCountForPlayer2(int PlayerNum)
        {
            UnitCounter = 0;

            for (int j = 0; j < unitTypes.Length; j++)
                UnitCounter += unitsCount[PlayerNum][j];
            return UnitCounter;
        }

        public static int UnitCountForPlayer(int PlayerNum)
        {
            return unitCountForPlayer[PlayerNum];
        }

        //public static string UnitCountForPlayerString(int playerNum)
        //{
        //   // return unitCountForPlayerString[playerNum];
        //}

        internal static int MaxUnitsToDeployFor(int PlayerNum, UnitType unitType)
        {
            //int unitsOut = UnitCountForPlayer(PlayerNum);
            /////////////////////////////////////////FIX THIS TO USE THE NUMBER OF UNITS CREATED VIA UNIT TOWERS//////////////////////////////////////////////////
            // return MAX_UNITS - unitsOut;
            return unitsCount[PlayerNum][(int)unitType] - unitDeployedForPlayer[PlayerNum][(int)unitType];
        }


        internal static void SetOtherUnitsToNewTarget(int oldPlayerNum)
        {
            Unit u;
            Player player;
            Player target;
            for (int i = 1; i <= PlayerCollection.NumPlayers; i++)
            {
                if (i == oldPlayerNum || PlayerCollection.Players[i].IsDead)
                    continue;

                player = PlayerCollection.Players[i];
                target = PlayerCollection.Players[player.TargetPlayerNum];

                if (target.IsDead)
                    continue;
                for (int j = 0; j < unitTypes.Length; j++)
                {
                    for (int k = 0; k < MAX_UNITS; k++)
                    {
                        u = units[i][j][k];
                        if (u.Status == UnitStatus.Deployed && u.PlayerToAttack == oldPlayerNum)
                        {
                            u.UpdateTargetPlayer(ref target.PlayerBase.Tile, player.TargetPlayerNum);
                        }
                    }
                }
            }
        }
    }
}
