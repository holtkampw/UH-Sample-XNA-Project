﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UHSampleGame.TileSystem;

namespace UHSampleGame.CoreObjects
{
    public interface IPathableObject : ITileableObject
    {
        List<Tile> GetPath();
    }
}
