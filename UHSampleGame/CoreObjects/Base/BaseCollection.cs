using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using UHSampleGame.ScreenManagement;
using UHSampleGame.CameraManagement;
using UHSampleGame.TileSystem;

namespace UHSampleGame.CoreObjects.Base
{
    public static class BaseCollection
    {
        public static List<Base> Bases;

        public static void Initialize()
        {
            Bases = new List<Base>();
            for(int i =0; i<8; i++)
                Bases.Add(new Base(i,i, BaseType.type1, Tile.NullTile));
        }

        public static void Add(ref Base playerBase)
        {
            Bases[playerBase.PlayerNum] = playerBase;
        }

        public static Tile GetBaseTileForPlayer(int playerNum)
        {
            return Bases[playerNum].GetTile();
        }
    }
}
