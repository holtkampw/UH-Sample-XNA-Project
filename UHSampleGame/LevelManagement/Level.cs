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
using UHSampleGame.Players;

namespace UHSampleGame.LevelManagement
{
    public class Level
    {
        int id;
        List<List<int>> map;
        Vector2 numTiles;
        List<Player> humanPlayers;
        List<Player> aiPlayers;

        public int ID
        {
            get { return id; }
        }

        public Level(int id, List<List<int>> map, List<Player> humanPlayers, List<Player> aiPlayers)
        {
            this.id = id;
            this.map = map;
            this.humanPlayers = humanPlayers;
            this.aiPlayers = aiPlayers;
        }

        public void Load()
        {
            SetTileMap();
        }

        public Vector2 NumTiles
        {
            get { return numTiles; }
        }

        protected void SetTileMap()
        {
            this.numTiles = new Vector2(map[0].Count, map.Count);
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

            for (int i = 0; i < humanPlayers.Count; i++)
                humanPlayers[i].SetTargetBase(aiPlayers[0].PlayerBase);

            for (int i = 0; i < aiPlayers.Count; i++)
                aiPlayers[i].SetTargetBase(humanPlayers[0].PlayerBase);

            TileMap.UpdateTilePaths();

        }

        /// <summary>
        /// Sets the appropriate GameObject on the tile given the object key
        /// 
        /// The objectKey is coded as follows:  The first digit represents the player with 5+ being 
        /// the AI.  The second digit represents the type of team that player belongs to.
        /// The third digit represents the type of tower or base to build with 0 being
        /// a base.  The fourth digit represents the number of upgrades the tower has.
        /// 
        /// A negative number represents a terrain object that blocks a path with no association
        /// to AI or players
        /// 
        /// Example: 1112 - Player one, team one, tower one, two upgrades on the tower
        /// Example: 5000 - AI base
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

                Player currentPlayer = GetPlayer(playerNum);

                if (towerNum == 0)
                {
                    Base gameObject = new TestBase(playerNum, teamNum, tile);
                    currentPlayer.SetBase((TestBase)gameObject);
                    TileMap.SetObject(gameObject, tile);
                }
                else
                {
                    //Handle Multiple towers here
                    Tower gameObject = new Tower(TowerType.TowerA, playerNum, teamNum, tile);
                    currentPlayer.SetTowerForLevelMap((Tower)gameObject);
                    TileMap.SetObject(gameObject, tile);
                }

               
            }
        }

        protected Player GetPlayer(int playerNum)
        {
            if (playerNum < 5)
            {
                return humanPlayers[playerNum - 1];
            }
            else
            {
                return aiPlayers[playerNum - 5];
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
