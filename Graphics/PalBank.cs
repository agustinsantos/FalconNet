using System;
using System.Diagnostics;
using System.IO;


namespace FalconNet.Graphics
{
    /***************************************************************************\
        PalBank.h
        Scott Randolph
        March 9, 1998

        Provides the bank of palettes used by all the BSP objects.
    \***************************************************************************/



    public class PaletteBankClass
    {
        // The one and only palette bank.  This would need to be replaced
        // by pointers to instances of PaletteBankClass passed to each call
        // if more than one color store were to be simultaniously maintained.
        //TODO public static PaletteBankClass ThePaletteBank = new PaletteBankClass();


        public PaletteBankClass() { nPalettes = 0; PalettePool = null; }
        //TODO public ~PaletteBankClass()	{};

        // Management functions
        public static void Setup(int nEntries)
        {
            // Create our array of palettes
            nPalettes = nEntries;
            if (nEntries != 0)
            {
#if USE_SH_POOLS
		PalettePool = (Palette *)MemAllocPtr(gBSPLibMemPool, sizeof(Palette)*(nEntries), 0 );
#else
                PalettePool = new Palette[nEntries];
#endif
            }
            else
            {
                PalettePool = null;
            }
        }
        public static void Cleanup()
        {
            // Clear our extra reference which was holding the data in memory
            for (int i = 0; i < nPalettes; i++)
            {
                //TODO		PalettePool[i].Release();
            }

            // Clean up our array of palettes
#if USE_SH_POOLS
	MemFreePtr( PalettePool );
#else
            //TODO delete[] PalettePool;
#endif
            PalettePool = null;
            nPalettes = 0;
        }

        public static void ReadPool(FileStream file)
        {
            int result;
            try
            {
                using (BinaryReader binfile = new BinaryReader(file))
                {
                    // Read how many palettes we have
                    nPalettes = binfile.ReadInt32();

                    // Allocate memory for our palette list
#if USE_SH_POOLS
	PalettePool = (Palette *)MemAllocPtr(gBSPLibMemPool, sizeof(Palette)*(nPalettes), 0 );
#else
                    PalettePool = new Palette[nPalettes];
#endif
                    Debug.Assert(PalettePool != null);

                    // Read the data for each palette
                    for (int i = 0; i <= nPalettes; i++)
                    {
                        PalettePool[i] = new Palette(binfile.ReadBytes(Palette.Sizeof()));
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Reading object palette bank: " + e);
            }
        }
        public static void FlushHandles()
        {
            int id;
            int cnt ;

            // Clear our extra reference which was holding the MPR instances of palettes
            for (id = 0; id < nPalettes; id++)
            {
                cnt = PalettePool[id].Release();
                Debug.Assert(cnt == 0);
#if NOTHING // If a quick hack is required to clean up, this would be it...
		while (PalettePool[id].Release());
#endif
            }

            // Now put the extra reference back for next time (to hold the MPR palette once its loaded)
            for (id = 0; id < nPalettes; id++)
            {
                PalettePool[id].Reference();
            }
        }

        // Set the light level on the specified palette
        public static void LightPalette(int id, Tcolor light)
        {
            if (!IsValidIndex(id))
            {
                return;
            }

            PalettePool[id].LightTexturePalette(light);
        }
        public static void LightBuildingPalette(int id, Tcolor light)
        {
            if (!IsValidIndex(id))
            {
                return;
            }

            PalettePool[id].LightTexturePaletteBuilding(light);
        }
        public static void LightReflectionPalette(int id, Tcolor light)
        {
            if (!IsValidIndex(id))
            {
                return;
            }

            PalettePool[id].LightTexturePaletteReflection(light);
        }

        // Check if palette ID is within the legal range of loaded palettes
        public static bool IsValidIndex(int id)
        {
            return ((id >= 0) && (id < nPalettes));
        }


        public static Palette[] PalettePool;
        public static int nPalettes;
    }
}
