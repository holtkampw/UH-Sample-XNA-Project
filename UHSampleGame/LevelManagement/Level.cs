using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using UHSampleGame.CoreObjects;
using UHSampleGame.CoreObjects.Towers;
using UHSampleGame.CoreObjects.Base;
using UHSampleGame.TileSystem;

namespace UHSampleGame.LevelManagement
{
    public class Level
    {
        int id;
        List<List<int>> map;

        public int ID
        {
            get { return id; }
        }

        public Level(int id, List<List<int>>map)
        {
            this.id = id;
            this.map = map;
        }

        protected void SetTileMap()
        {
            Vector2 numTiles = new Vector2(map[0].Count, map.Count);
            TileMap.InitializeTileMap(Vector3.Zero, numTiles, new Vector2(100, 100));

            for (int row = 0; row < map.Count; row++)
            {
                for (int col = 0; col < map[0].Count; col++)
                {
                    if (map[row][col] != 0)
                    {
                        SetGameObjectOnTile(map[row][col], TileMap.Tiles[(int)(row * numTiles.X) + col]);
                    }
                }
            }

        }

        /// <summary>
        /// Sets the appropriate GameObject on the tile given the object key
        /// 
        /// The objectKey is coded as follows:  The first digit represents the player with 9 being 
        /// the AI.  The second digit represents the type of team that player belongs to.
        /// The third digit represents the type of tower or base to build with 0 being
        /// a base.  The fourth digit represents the number of upgrades the tower has.
        /// 
        /// A negative number represents a terrain object that blocks a path with no association
        /// to AI or players
        /// 
        /// Example: 1112 - Player one, team one, tower one, two upgrades on the tower
        /// Example: 9000 - AI base
        /// </summary>
        /// <param name="objectKey">Refers to the object to place on the tile</param>
        /// <param name="tile">The tile to receive the object</param>
        protected void SetGameObjectOnTile(int objectKey, Tile tile)
        {
            if (objectKey < 0)
            {
                //Handle static terrain objects
            }
            else
            {
                int playerNum, teamNum, towerNum, upgradeNum;

                ExtractObjectKeyInfo(objectKey, out playerNum, 
                    out teamNum, out towerNum, out upgradeNum);

                GameObject gameObject;

                if (towerNum == 0)
                {
                    gameObject = new TestBase(playerNum, teamNum, tile);
                }
                else
                {
                    //Handle Multiple towers here
                    gameObject = new TowerAGood(playerNum, teamNum, tile);
                }

                //Tilemap.Set......
            }
        }

        private void ExtractObjectKeyInfo(int objectKey, out int playerNum, out int teamNum, out int towerNum, out int upgradeNum)
        {
            upgradeNum = objectKey % 10;
            objectKey = objectKey / 10;

            towerNum = objectKey % 10;
            objectKey = objectKey / 10;

            teamNum = objectKey % 10;
            objectKey = objectKey / 10;

            playerNum = objectKey % 10;
            objectKey = objectKey / 10;
        }
    }
}
