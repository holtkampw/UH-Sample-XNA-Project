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

        static Enum[] unitTypes = EnumHelper.EnumToArray(new UnitType());

        //units[playerNum][unitType][index]
        static List<List<List<Unit2>>> units;

        //unitsCount[playerNum][unitType]
        static List<List<int>> unitsCount;

        static void Initialize(int numPlayers)
        {
            units = new List<List<List<Unit2>>>();
            unitsCount = new List<List<int>>();

            for (int i = 0; i < numPlayers; i++)
            {
                units[i] = new List<List<Unit2>>();
                unitsCount[i] = new List<int>();

                for (int j = 0; j < unitTypes.Length; j++)
                {
                    units[i][j] = new List<Unit2>();
                    unitsCount[i][j] = 0;

                    for (int k = 0; k < MAX_UNITS; k++)
                    {
                        units[i][j].Add(new Unit2((UnitType)j));
                    }
                }
            }

        }

        static void Add(int playerNum, UnitType unitType)
        {
            for (int i = 0; i < unitsCount[playerNum][(int)unitType]; i++)
            {
                if (!units[playerNum][(int)unitType][i].IsActive())
                {
                    units[playerNum][(int)unitType][i].Activate();
                    unitsCount[playerNum][(int)unitType]++;
                    break;
                }
            } 
        }

        static void Remove()
        {

        }

        static void Update(GameTime gameTime)
        {

        }

        static void Draw(GameTime gameTime)
        {

        }
    }
}
