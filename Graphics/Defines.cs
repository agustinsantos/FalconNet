using System;
using DWORD = System.UInt32;
using WORD = System.UInt16;
using BOOL = System.Boolean;
using BYTE = System.Byte;

namespace FalconNet.Graphics
{
    public enum ColorDepth
    {
        // color depth
        COLOR_256 = 0, // 256 color
        COLOR_32K = 1, // 32K color --  0rrrrrgggggbbbbb
        COLOR_64K = 2, // 64K color --  rrrrrggggggbbbbb
        COLOR_16M = 3 // 16M color --  8r8g8b
    }

    public enum ImageType
    {
        // image type
        IMAGE_TYPE_UNKNOWN = -1,
        IMAGE_TYPE_GIF = 1,
        IMAGE_TYPE_LBM = 2,
        IMAGE_TYPE_PCX = 3,
        IMAGE_TYPE_BMP = 4,
        IMAGE_TYPE_APL = 5,
        IMAGE_TYPE_TGA = 6,
        IMAGE_TYPE_DDS = 7
    }

   public struct GLImageInfo
    {
       public int width;
       public int height;
       public ulong[] palette;
       public byte[] image;
        //DDSURFACEDESC2 ddsd;
    }

    public static class DDraw
{
            public static DWORD MAKEFOURCC(char ch0, char ch1, char ch2, char ch3)
            {
                return ((DWORD)(BYTE)(ch0)        | ((DWORD)(BYTE)(ch1) << 8) |  
                       ((DWORD)(BYTE)(ch2) << 16) | ((DWORD)(BYTE)(ch3) << 24 ));
            }
}
}
