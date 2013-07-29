using FalconNet.Common.Encoding;
using FalconNet.Common.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace FalconNet.CampaignBase
{
    //TODO renma this class to something like TerrainMapInfo
    public class TMap
    {
        // Number of feet between posts at highest detail - since we have one map, we can have on of these
        public static float FeetPerPost;
        public static bool g_LargeTerrainFormat;
        public static float g_MaximumTheaterAltitude;
        public static bool g_LargeTheater;

        public TMap()
        {
            initialized = false;
        }
        //TODO public ~TMap()
        //{
        //    initialized = FALSE;
        //};

        public int Setup(string mapPath)
        {
            throw new NotImplementedException();
        }
        public void Cleanup()
        {
            throw new NotImplementedException();
        }

        // Flag to indicate that this map has or has not been initialized
        public bool IsReady()
        {
            return initialized;
        }

        // Provide access to the levels which make up this map
        public TLevel Level(int level)
        {
#if TODO
            Debug.Assert(level < nLevels);
            return (Levels[level]);
#endif
            throw new NotImplementedException();
        }
        public int NumLevels()
        {
            return nLevels;
        }
        public int LastNearTexLOD()
        {
            return lastNearTexturedLOD;
        }
        public int LastFarTexLOD()
        {
            return lastFarTexturedLOD;
        }

        // Return the world space dimensions of this map
        public float NorthEdge()
        {
            Debug.Assert(IsReady());
            return Levels.BlocksHigh() * Levels.FTperBLOCK();
        }
        public float EastEdge()
        {
            Debug.Assert(IsReady());
            return Levels.BlocksWide() * Levels.FTperBLOCK();
        }
        public float WestEdge()
        {
            Debug.Assert(IsReady());
            return 0.0f;
        }
        public float SouthEdge()
        {
            Debug.Assert(IsReady());
            return 0.0f;
        }
        public uint BlocksHigh()
        {
            Debug.Assert(IsReady());
            return Levels.BlocksHigh();
        }
        public uint BlocksWide()
        {
            Debug.Assert(IsReady());
            return Levels.BlocksWide();
        }
        public float FeetPerBlock()
        {
            Debug.Assert(IsReady());
            return Levels.FTperBLOCK();
        }
        // Return a course appoximation of terrain height (should be max, positive up)
        public float GetMEA(float FTnorth, float FTeast)
        {
            throw new NotImplementedException();
        }

        // Probably should be protected, but things are easier (faster?) this way...
        Tcolor[] ColorTable = new Tcolor[256];
        Tcolor[] DarkColorTable = new Tcolor[256];
        Tcolor[] GreenTable = new Tcolor[256];
        Tcolor[] DarkGreenTable = new Tcolor[256];


        protected bool initialized;

        protected int lastNearTexturedLOD;
        protected int lastFarTexturedLOD;

        protected int nLevels;
        protected TLevel Levels;

        protected Int16[] MEAarray; // Array of height values
        protected int MEAwidth; // Width of MEA array
        protected int MEAheight; // Height of MEA array
        protected float FTtoMEAcell; // Conversion factor for indexing into the array
        protected enum TMAPENUM
        {
            TMAP_LARGETERRAIN = 0x1, // using larger terrain maps
            TMAP_LARGEUIMAP = 0x2, // double sized squad selection map
        }
        protected TMAPENUM flags; // misc flags

        protected void LoadMEAtable(string mapPath)
        {
            throw new NotImplementedException();
        }
#if TODO
    protected void LoadColorTable(HANDLE inputFile);
#endif
        protected static void TimeUpdateCallback(object self)
        {
            throw new NotImplementedException();
        }
        protected void UpdateLighting()
        {
            throw new NotImplementedException();
        }
    }
    
    public class TMapEncodingLE
    {
        /// <summary> 
        /// Encodes the specified value, returning the result as a byte array.
        /// </summary>
        /// <param name="parameterValue">the value to Encode
        /// </param>
        /// <returns> 
        /// a byte array containing the encoded value
        /// </returns>
        public static void Encode(Stream stream, TMap val)
        {
            throw new NotImplementedException();
        }

        /// <summary> 
        /// Decodes and returns the parameterValue stored in the specified bufferStream.
        /// </summary>
        /// <param name="bufferStream">the bufferStream containing the encoded parameterValue
        /// </param>
        /// <returns> the decoded parameterValue
        /// </returns>
        public static void Decode(Stream stream, ref TMap val)
        {
            throw new NotImplementedException();
        }

        public static int Size
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
