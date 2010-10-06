using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace UHSampleGame.CoreObjects.Units
{
    static class UnitCollection
    {
        const int MAX_UNITS = 5000;

        static int NumPlayers;
        static Enum[] unitTypes = EnumHelper.EnumToArray(new UnitType());

        //units[playerNum][unitType][index]
        static List<List<List<Unit2>>> units;

        //unitsCount[playerNum][unitType]
        static List<List<int>> unitsCount;

        static List<List<int>> unitsMaxIndex;

        public static void Initialize(int numPlayers)
        {
            NumPlayers = numPlayers;
            units = new List<List<List<Unit2>>>();
            unitsCount = new List<List<int>>();
            unitsMaxIndex = new List<List<int>>();

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
            for (int i = 0; i <= unitsMaxIndex[playerNum][(int)unitType]; i++)
            {
                if (!units[playerNum][(int)unitType][i].IsActive())
                {
                    units[playerNum][(int)unitType][i].Activate();
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
                    for (int k = 0; k < unitsMaxIndex[i][j]; k++)
                        if(units[i][j][k].IsActive())
                            units[i][j][k].Update(gameTime);
        }

        public static void Draw(GameTime gameTime)
        {
            for (int i = 0; i < NumPlayers; i++)
                for (int j = 0; j < unitTypes.Length; j++)
                    for (int k = 0; k < unitsMaxIndex[i][j]; k++)
                        if (units[i][j][k].IsActive())
                            units[i][j][k].Draw(gameTime);
        }
    }
}
