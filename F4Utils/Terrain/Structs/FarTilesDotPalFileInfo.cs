﻿using System;
using System.Drawing;

namespace F4Utils.Terrain.Structs
{
    [Serializable]
    public struct FarTilesDotPalFileInfo
    {
        public uint numTextures;
        public Color[] pallete;
    }
}