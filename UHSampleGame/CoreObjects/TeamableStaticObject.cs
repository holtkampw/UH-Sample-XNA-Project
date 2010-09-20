using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace UHSampleGame.CoreObjects
{
    public abstract class TeamableStaticObject : StaticTileObject
    {
        protected int playerNum;
        protected int teamNum;

        public int PlayerNum
        {
            get { return playerNum; }
        }

        public int TeamNum
        {
            get { return teamNum; }
        }

        public TeamableStaticObject(int playerNum, int teamNum, Model model )
            :base(model)
        {
            this.playerNum = playerNum;
            this.teamNum = teamNum;
        }

        public void SetPlayerNum(int newPlayerNum)
        {
            playerNum = newPlayerNum;
        }

        public void SetTeamNum(int newTeamNum)
        {
            teamNum = newTeamNum;
        }
    }
}
