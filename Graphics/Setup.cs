using System;
using System.IO;

namespace FalconNet.Graphics
{
    /***************************************************************************\
        This is a one stop source for the terrain/weather/graphics system
        startup and shutdown sequences.  Just call these functions and you're
        set.
    \***************************************************************************/
    public static class Setup
    {
        /***************************************************************************\
            Load all data and create all structures which do not depend on a
            specific graphics device.  This should be done only once.  This must
            be done before any of the other setup calls are made.
        \***************************************************************************/
        public static void DeviceIndependentGraphicsSetup(string theater, string objects, string misctex)
        {
            string fullPath;
            string zipName;


            // Store the data path and the map name
            theaterPath = theater;
            objectPath = objects;
            misctexPath = misctex;

#if USE_SH_POOLS
			// Initialize our Smart Heap pools
			Palette.InitializeStorage();
			TBlock.InitializeStorage();
			TListEntry.InitializeStorage();
			TBlockList.InitializeStorage();
			glMemPool = MemPoolInit( 0 );
#endif

#if GRAPHICS_USE_RES_MGR
			// Setup our attach points
			sprintf( fullPath, "%s\\Texture", theaterPath );
			ResAddPath( fullPath, false );
			sprintf( fullPath, "%s\\Weather", theaterPath );
			ResAddPath( fullPath, false );
			ResAddPath( objectPath, false );
			ResAddPath( misctexPath, false );
		
			// Attach our resource files
			sprintf( fullPath, "%s\\texture\\", theaterPath );
			sprintf( zipName, "%s\\texture\\%s", theaterPath, TerrainTexArchiveName );
			ResHandleTerrainTex = ResAttach_Open ( fullPath, zipName, false );
			if(ResHandleTerrainTex < 0)
			{
				//Debug.Assert( ResHandleTerrainTex >= 0 );
				// we need to exit cleanly... cuz the file couldn't be opened which we need
			}
#endif
#if TODO
			// Setup the font tables
			VirtualDisplay.InitializeFonts();
		
			// Setup the Asynchronous loader object
			TheLoader.Setup();
		
			// Setup the environmental time manager object
			TheTimeManager.Setup( g_nYear, g_nDay );		// TODO:  Get a day of the month in here or somewhere
			TheTimeManager.SetTime( 0 );			// TODO:  Get a time of day in here or somewhere
		
		// M.N. Turn around setup - first terrain, then TOD - we need theater.map's LAT/LONG for the stars
		
			// Setup the terrain database
			fullPath =  theaterPath + Path.DirectorySeparatorChar + "terrain";
			TheMap.Setup( fullPath );
			
			// Setup the time of day manager
			fullPath = theaterPath + Path.DirectorySeparatorChar + "weather";
			TheTimeOfDay.Setup ( fullPath );
			
			// Setup the weather database
			Debug.Assert( TheWeather );
			TheWeather.Setup();
		
			// Setup the BSP object library
			fullPath=  objectPath + Path.DirectorySeparatorChar + "KoreaObj";
			ObjectParent.SetupTable( fullPath );
#endif
            throw new NotImplementedException();
        }


        /***************************************************************************\
            Load all data and create all structures which require a specific 
            graphics device to be identified.  For now, this can only be done
            for one device at a time.  In the future, we might allow multiple
            simultanious graphics devices through this interface.
        \***************************************************************************/
        public static void DeviceDependentGraphicsSetup(DisplayDevice dd)
        {
            string fullPath;
#if TODO	
			// OW - must initialize Textures first for pools to work 
#if !NOTHING
			// Setup the miscellanious texture database
			fullPath = misctexPath + Path.DirectorySeparatorChar;
			Texture.SetupForDevice( device.GetDefaultRC(), fullPath );
		
			// Setup the terrain texture database
			fullPath =  theaterPath + Path.DirectorySeparatorChar +"texture" + Path.DirectorySeparatorChar;
			TheTerrTextures.Setup( device.GetDefaultRC(), fullPath );
			TheFarTextures.Setup( device.GetDefaultRC(), fullPath );
#else
			// Setup the terrain texture database
			sprintf( fullPath, "%s\\texture\\", theaterPath );
			TheTerrTextures.Setup( device.GetDefaultRC(), fullPath );
			TheFarTextures.Setup( device.GetDefaultRC(), fullPath );
		
			// Setup the miscellanious texture database
			sprintf( fullPath, "%s\\", misctexPath );
			Texture.SetupForDevice( device.GetDefaultRC(), fullPath );
#endif
		
			// Setup all the miscellanious textures we need to pre-load
			DrawableBSP.SetupTexturesOnDevice( device.GetDefaultRC() );
			Drawable2D.SetupTexturesOnDevice( device.GetDefaultRC() );
			DrawableTrail.SetupTexturesOnDevice( device.GetDefaultRC() );
			DrawableOvercast.SetupTexturesOnDevice( device.GetDefaultRC() );
			RenderOTW.SetupTexturesOnDevice( device.GetDefaultRC() );
			Load2DFontTextures();
#endif
            throw new NotImplementedException();
        }

        /***************************************************************************\
            Clean up all graphics device dependent data and structures.
        \***************************************************************************/
        public static void DeviceDependentGraphicsCleanup(DisplayDevice dd)
        {
#if TODO			
			// Clean up all the pre-loaded textures we have.
			DrawableBSP.ReleaseTexturesOnDevice( device.GetDefaultRC() );
			Drawable2D.ReleaseTexturesOnDevice( device.GetDefaultRC() );
			DrawableTrail.ReleaseTexturesOnDevice( device.GetDefaultRC() );
			DrawableOvercast.ReleaseTexturesOnDevice( device.GetDefaultRC() );
			RenderOTW.ReleaseTexturesOnDevice( device.GetDefaultRC() );
			Release2DFontTextures();
		
			// Wait for loader here to ensure everything which depends of this repositories is gone
			TheLoader.WaitForLoader();
		
			// Clean up the graphics dependent portions of these data sources
			TheFarTextures.Cleanup();
			TheTerrTextures.Cleanup();
			ObjectParent.FlushReferences();
			TheTextureBank.FlushHandles();
			ThePaletteBank.FlushHandles();
			Texture.CleanupForDevice( device.GetDefaultRC() );
#endif
            throw new NotImplementedException();
        }


        /***************************************************************************\
            Clean up all graphics device independent data and structures.
            This should not be done until all device dependent stuff has been
            cleaned up.
        \***************************************************************************/
        public static void DeviceIndependentGraphicsCleanup()
        {
#if TODO		
			TheLoader.Cleanup();
			TheTimeOfDay.Cleanup();
		    TheWeather.Cleanup();
			ObjectParent.CleanupTable();
			TheMap.Cleanup();
			TheTimeManager.Cleanup();
		
#if GRAPHICS_USE_RES_MGR
			// Detach our resource file
			ResDetach( ResHandleTerrainTex );
#endif
		
#if USE_SH_POOLS
			// Release our Smart Heap pools
			Palette.ReleaseStorage();
			TBlock.ReleaseStorage();
			TListEntry.ReleaseStorage();
			TBlockList.ReleaseStorage();
			MemPoolFree( glMemPool );
#endif
#endif
            throw new NotImplementedException();
        }



        static string theaterPath;
        static string objectPath;
        static string misctexPath;
    }
}