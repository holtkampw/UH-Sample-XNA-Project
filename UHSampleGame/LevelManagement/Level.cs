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
        //List<Player> humanPlayers;
        //List<Player> aiPlayers;
        List<Player> players;
        List<int> humanPlayers = new List<int>();
        List<int> computerPlayers = new List<int>();
        List<PlayerSetup> playerSetup;

        public int ID
        {
            get { return id; }
        }

        public Level(int id, List<List<int>> map, /*List<Player> humanPlayers, List<Player> aiPlayers*/ List<Player> players)
        {
            this.id = id;
            this.map = map;
            this.players = players;
            //this.humanPlayers = humanPlayers;
            //this.aiPlayers = aiPlayers;
        }

        public Level(int id, List<List<int>> map, List<PlayerSetup> playerSetup)
        {
            //Created Map!
            this.id = id;
            this.map = map;
            this.playerSetup = playerSetup;
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

            //for (int i = 0; i < humanPlayers.Count; i++)
            //    players[humanPlayers[i] - 1].SetTargetBase(players[computerPlayers[0] - 1].PlayerBase);

            //for (int i = 0; i < computerPlayers.Count; i++)
            //    players[computerPlayers[i] - 1].SetTargetBase(players[humanPlayers[0] - 1].PlayerBase);

            int activePlayers = 0;
            for (int i = 0; i < playerSetup.Count; i++)
                if (playerSetup[i].active)
                    activePlayers++;

            int[] targetPlayerNum = new int[activePlayers];

            for (int p = 0; p < playerSetup.Count; p++)
            {
                for (int next = 0; next < playerSetup.Count; next++)
                {
                    if (p != next && playerSetup[next].active)
                    {
                        if (playerSetup[p].teamNum != playerSetup[next].teamNum)
                        {
                            targetPlayerNum[p] = next;
                        }
                    }
                }
            }


            int index = 0;
            for (int i = 0; i < playerSetup.Count; i++)
            {
                if (playerSetup[i].active)
                {
                    PlayerCollection.SetTargetFor(playerSetup[i].playerNum, playerSetup[targetPlayerNum[index]].playerNum);
                    index++;
                }
            }

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
                int playerType, playerNum, teamNum, towerNum, upgradeNum;

                ExtractObjectKeyInfo(objectKey, out playerType, out playerNum,
                    out teamNum, out towerNum, out upgradeNum);

                //Player currentPlayer = GetPlayer(playerType, playerNum);

                if (towerNum == 0)
                {
                    for(int i = 0; i < playerSetup.Count; i++)
                    {
                        if(playerNum == playerSetup[i].playerNum) //Found Player!
                        {
                            Base gameObject = new Base(playerNum, teamNum, BaseType.type1, tile);
                            PlayerType newPlayerType = PlayerType.AI;
                            if (playerType == 1)
                                newPlayerType = PlayerType.Human;

                            PlayerCollection.AddPlayer(new Player(playerNum, teamNum, gameObject, newPlayerType));
                            BaseCollection.Add(ref gameObject);
                            //PlayerCollection.SetBaseFor(playerNum, gameObject);
                            TileMap.SetObject(gameObject, tile);
                            break;
                        }
                    }
                }
                else
                {
                    //Handle Multiple towers here
                    //Tower gameObject = new Tower(TowerType.TowerA);
                    
                    
                    //currentPlayer.SetTowerForLevelMap(TowerType.Plasma, tile);
                    //TileMap.SetObject(gameObject, tile);
                }


            }
        }

        protected Player GetPlayer(int playerType, int playerNum)
        {
            if (playerType == 1)
            {
                humanPlayers.Add(playerNum);
                return players[playerNum - 1];
            }
            else
            {
                computerPlayers.Add(playerNum);
                return players[playerNum - 1];
            }
        }

        private void ExtractObjectKeyInfo(int objectKey, out int playerType, out int playerNum, out int teamNum, out int towerNum, out int upgradeNum)
        {
            upgradeNum = objectKey % 10;
            objectKey = objectKey / 10;

            towerNum = objectKey % 10;
            objectKey = objectKey / 10;

            teamNum = objectKey % 10;
            objectKey = objectKey / 10;

            playerNum = objectKey % 10;
            objectKey = objectKey / 10;

            playerType = objectKey % 10;
            objectKey = objectKey / 10;
        }
    }
}
