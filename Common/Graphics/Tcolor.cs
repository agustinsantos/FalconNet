using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.Common.Graphics
{
    // RGB color
    public struct Tcolor
    {
        public float r;
        public float g;
        public float b;

        // Pack Functions
        public uint Pack()
        {
            return (((uint)((1.0f) * 255)) << 24) |
                (((uint)((r) * 255)) << 16) |
                (((uint)((g) * 255)) << 8) |
                (uint)((b) * 255);
        }

        public uint Pack(float fScale)
        {
            return (((uint)((1.0f) * 255)) << 24) |
                (((uint)((r * fScale) * 255)) << 16) |
                (((uint)((g * fScale) * 255)) << 8) |
                (uint)((b * fScale) * 255);
        }

        // Static Pack Functions
        public static uint PackRGBA(float r, float g, float b, float a)
        {
            return (((uint)((1.0f) * 255)) << 24) |
                (((uint)((r) * 255)) << 16) |
                (((uint)((g) * 255)) << 8) |
                (uint)((b) * 255);
        }

        public static uint PackRGBA(float r, float g, float b, float a, float fScale)
        {
            return (((uint)((1.0f) * 255)) << 24) |
                (((uint)((r * fScale) * 255)) << 16) |
                (((uint)((g * fScale) * 255)) << 8) |
                (uint)((b * fScale) * 255);
        }
    }
}
