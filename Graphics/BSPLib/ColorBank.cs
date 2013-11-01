using FalconNet.Common.Encoding;
using FalconNet.F4Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using DWORD = System.UInt32;

namespace FalconNet.Graphics
{
    public enum ColorMode { NormalMode, UnlitMode, GreenMode, UnlitGreenMode };

    public class ColorBankClass
    {
        // The one and only color bank.  This would need to be replaced
        // by pointers to instances of ColorBankClass passed to each call
        // if more than one color store were to be simultaniously maintained.
        public static ColorBankClass TheColorBank = new ColorBankClass();

        public ColorBankClass()
        {
            nColors = nDarkendColors = 0;
            ColorBuffer = null;
            ColorPool = 0;
        }
        //TODO public  ~ColorBankClass() {};



        // Management functions
        public static void Setup(int nclrs, int ndarkclrs)
        {
            // Make sure we're not clobbering a pre-existing table
            Debug.Assert(ColorBuffer == null);

            // Record how many colors we've got total and prelit
            nColors = nclrs;
            nDarkendColors = ndarkclrs;

            // Allocate space for the colors
#if USE_SH_POOLS
    ColorBuffer = (Pcolor *)MemAllocPtr(gBSPLibMemPool, sizeof(Pcolor) * 4 * (nclrs + MAX_VERTS_PER_POLYGON + MAX_CLIP_VERTS), 0);
#else
            ColorBuffer = new Pcolor[4 * (nclrs + StateStackClass.MAX_VERTS_PER_POLYGON + StateStackClass.MAX_CLIP_VERTS)];
#endif
            DarkenedBuffer = (nclrs + StateStackClass.MAX_VERTS_PER_POLYGON + StateStackClass.MAX_CLIP_VERTS);
            GreenIRBuffer = DarkenedBuffer + (nclrs + StateStackClass.MAX_VERTS_PER_POLYGON + StateStackClass.MAX_CLIP_VERTS);
            GreenTVBuffer = GreenIRBuffer + (nclrs + StateStackClass.MAX_VERTS_PER_POLYGON + StateStackClass.MAX_CLIP_VERTS);
        }

        public static void Cleanup()
        {
            nColors = 0;
            nDarkendColors = 0;
#if USE_SH_POOLS
    MemFreePtr(ColorBuffer);
#else
            //delete[] ColorBuffer;
#endif
            ColorBuffer = null;
            DarkenedBuffer = -1;
            GreenIRBuffer = -1;
            GreenTVBuffer = -1;
        }

        public static void ReadPool(Stream file)
        {
            // Read our total color and darkened color count
            nColors = Int32EncodingLE.Decode(file);
            nDarkendColors = Int32EncodingLE.Decode(file);

            // Setup our internal storage
            Setup(nColors, nDarkendColors);

            // Read our color array
            for (int i = 0; i < nColors; i++)
            {
                PcolorEncodingLE.Decode(file, ref ColorBuffer[i]);
            }

            // Setup the darkened and green TV and IR versions
            int dst1 = DarkenedBuffer;
            int dst2 = GreenTVBuffer;
            int dst3 = GreenIRBuffer;
            int src = 0;
            int end = nColors;

            while (src < end)
            {
                ColorBuffer[dst1] = ColorBuffer[src];
                ColorBuffer[dst2].g = ColorBuffer[src].g;
                ColorBuffer[dst2].a = ColorBuffer[src].a;
                ColorBuffer[dst2].r = 0.0f;
                ColorBuffer[dst2].b = 0.0f;

                // FRB - B&W display?
                if ((F4Config.g_bGreyMFD) && (!F4Config.bNVGmode))
                    ColorBuffer[dst2].r = ColorBuffer[dst2].b = ColorBuffer[dst2].g;

                ColorBuffer[dst3] = ColorBuffer[dst2];
                src++;
                dst1++;
                dst2++;
                dst3++;
            }
        }

        public static void SetLight(float red, float green, float blue)
        {
            int src;
            int end;
            int dst;
            int greenTv;

            Debug.Assert(red <= 1.0f);
            Debug.Assert(green <= 1.0f);
            Debug.Assert(blue <= 1.0f);

            // Now darken the staticly lit colors
            src = 0;
            end = 0 + nDarkendColors;
            dst = DarkenedBuffer;
            greenTv = GreenTVBuffer;

            while (src < end)
            {
                ColorBuffer[dst].r = red * ColorBuffer[src].r;
                ColorBuffer[dst].g = green * ColorBuffer[src].g;
                ColorBuffer[dst].b = blue * ColorBuffer[src].b;
                ColorBuffer[greenTv].g = ColorBuffer[dst].g;

                // FRB - B&W display?
                if ((F4Config.g_bGreyMFD) && (!F4Config.bNVGmode))
                    ColorBuffer[greenTv].r = ColorBuffer[greenTv].b = ColorBuffer[greenTv].g;

                src++;
                dst++;
                greenTv++;
            }

            //JAM 12Oct03
            TODcolor = (uint)((0xFF << 24) + ((int)(red * 255.0f) << 16) + ((int)(green * 255.0f) << 8) + (int)(blue * 255.0f));
        }

        public static void SetColorMode(ColorMode mode)
        {
            switch (mode)
            {
                case ColorMode.NormalMode:
                    ColorPool = DarkenedBuffer;
                    break;

                case ColorMode.UnlitMode:
                    ColorPool = 0;
                    break;

                case ColorMode.GreenMode:
                    ColorPool = GreenTVBuffer;
                    break;

                case ColorMode.UnlitGreenMode:
                    ColorPool = GreenIRBuffer;
                    break;
            }
        }

        // Debug parameter validation
        public static bool IsValidColorIndex(int i)
        {
            return (i < nColors + StateStackClass.MAX_VERTS_PER_POLYGON + StateStackClass.MAX_CLIP_VERTS);
        }



        // Publicly used color array (set when color mode is chosen)
        public static int ColorPool;

        // Color counts
        public static int nColors; // Total number of colors in each set
        public static int nDarkendColors; // Number of colors which are staticly lit

        // These are the color pools for each mode
        public static Pcolor[] ColorBuffer; // Normal (original) colors
        public static int DarkenedBuffer; // Processed for static lighting on some colors
        public static int GreenIRBuffer; // Processed for green without lighting
        public static int GreenTVBuffer; // Processed for green with static lighting on some colors

        public static DWORD TODcolor; //JAM 12Oct03

        public static int PitLightLevel; // Cobra - 3D cockpit light level (0, 1, 2)

        // Light levels for the staticly lit colors
        // float redLevel;
        // float greenLevel;
        // float blueLevel;
    }
}
