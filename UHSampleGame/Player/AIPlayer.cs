using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UHSampleGame.TileSystem;

namespace UHSampleGame.Player
{
    public class AIPlayer : Player
    {
        public AIPlayer(int playerNum, int teamNum, Tile baseTile)
            : base(playerNum, teamNum, baseTile) { }
    }
}
