using System;

namespace FalconNet.Graphics
{
	public class Texture
	{
#if TODO
		public Texture()
		{
			texHandle = null; 
			imageData = null; 
			palette = null; 
			flags = 0;
			
			#if _DEBUG
			InterlockedIncrement((long *) &m_dwNumHandles);		// Number of instances
			InterlockedExchangeAdd((long *) &m_dwTotalBytes, sizeof(*this));
			#endif
		}

		// public ~Texture();
	
		public int dimensions;
		public void *imageData;
		public Palette *palette;
		public DWORD flags;
		public DWORD chromaKey;
	
		#if _DEBUG
		public static DWORD m_dwNumHandles = 0;		// Number of instances
		public static DWORD m_dwBitmapBytes = 0;		// Bytes allocated for bitmap copies
		public static DWORD m_dwTotalBytes = 0;			// Total number of bytes allocated (including bitmap copies and object size)
		#endif
	
		
		// OW
		protected TextureHandle *texHandle;
	
		/***************************************************************************\
			Store some useful global information.  The path is used for all
			texture loads through this interface and the RC is used for loading.
			This means that at present, only one device at a time can load textures
			through this interface.
		\***************************************************************************/
		public static void SetupForDevice( DXContext *texRC, string path)
		{
			// Store the texture path for future reference
			if(strlen( path )+1 >= sizeof( TexturePath ))
				ShiError( "Texture path name overflow!" );
		
			strcpy( TexturePath, path );
			if(TexturePath[strlen(TexturePath)-1] != '\\')
				strcat( TexturePath, "\\" );
		
			rc = texRC;
			Palette.SetupForDevice( texRC );
		
			TextureHandle.StaticInit(texRC.m_pD3DD);
		}
		/***************************************************************************\
			This is called when we're done working with a given device (as 
			represented by an RC).
		\***************************************************************************/
		public static void CleanupForDevice(DXContext *texRC)
		{
			Palette.CleanupForDevice( texRC );
			rc = null;
		
			TextureHandle.StaticCleanup();
		}

		/***************************************************************************\
			This is called to check whether the device is setup.
		\***************************************************************************/
		public static bool IsSetup() // JB 010616
		{
			return rc != null;
		}
		
		/***************************************************************************\
			Read a data file and store its information.
		\***************************************************************************/
		public BOOL LoadImage(string filename, DWORD newFlags = 0, BOOL addDefaultPath = true)
		{
			string fullname;
			CImageFileMemory 	texFile;
			int result;
		
			Debug.Assert( filename );
			Debug.Assert( imageData == null );
		
			// Add in the users requested flags to any already set
			flags |= newFlags;
		
			// Add the texture path to the filename provided
			if (addDefaultPath)
			{
				strcpy( fullname, TexturePath );
				strcat( fullname, filename );
			}
		
			else
			{
				strcpy( fullname, filename );
			}
		
			// Make sure we recognize this file type
			texFile.imageType = CheckImageType( fullname );
			if (texFile.imageType == IMAGE_TYPE_UNKNOWN)
			{
				ShiWarning( "Unrecognized image type" );
				return false;
			}
		
			// If the image type has alpha in it, create an alpha per texel texture
			if (texFile.imageType == IMAGE_TYPE_APL)
				flags |= MPR_TI_ALPHA;
		
			// Open the input file
			result = texFile.glOpenFileMem( fullname );
			if(result != 1)
			{
				ShiWarning( "Failed texture open" );
				return false;
			}
		
			// Read the image data (note that ReadTextureImage will close texFile for us)
			texFile.glReadFileMem();
			result = ReadTextureImage( &texFile );
		
			if(result != GOOD_READ)
			{
				ShiWarning( "Failed texture read" );
				return false;
			}
		
			// Store the image properties in our local storage
			if(texFile.image.width != texFile.image.height)
			{
				ShiWarning( "Texture isn't square" );
				return false;
			}
		
			dimensions = texFile.image.width;
			Debug.Assert( dimensions == 32 || dimensions == 64 || dimensions == 128 || dimensions == 256 );
		
			if(texFile.image.palette)
				chromaKey = texFile.image.palette[0];
			else
				chromaKey = 0xFFFF0000;		// Default to blue chroma key color
		
			// We only deal with 8 bit textures
			Debug.Assert(flags & MPR_TI_PALETTE);
		
			imageData = texFile.image.image;
		
			// Create a palette object if we don't already have one
			Debug.Assert( texFile.image.palette );
			if(!palette)
			{
				palette = new Palette();
				palette.Setup32( (DWORD*)texFile.image.palette );
			}
		
			else
				palette.Reference();
		
			// Release the image's palette data now that we've got our own copy
			glReleaseMemory( texFile.image.palette );
		
			#if _DEBUG
			InterlockedExchangeAdd((long *) &m_dwTotalBytes, dimensions * dimensions);
			#endif
		
			return true;
		}
		

		/***************************************************************************\
			Release the MPR texture we're holding.
		\***************************************************************************/
		public void FreeImage()
		{
			if(texHandle)
			{
				delete texHandle;
				texHandle = null;
			}
		
			if(!imageData)
				FreePalette(); 	// We're totally gone, so get rid of our palette if we had one
		}
		
		public bool CreateTexture(char *strName = null);
		public void FreeTexture();
		
		/***************************************************************************\
			Release the MPR palette and palette data we're holding.
		\***************************************************************************/
		public BOOL LoadAndCreate(string filename, DWORD newFlags = 0)
		{ 
			if(LoadImage(filename, newFlags))
			{
				CreateTexture(filename);
				return true;
			}
		
			return false; 
		}

		
		/***************************************************************************\
			Reload the MPR texels with the ones we have stored locally.
		\***************************************************************************/
		public bool UpdateMPR(string strName = null)
		{
			Debug.Assert( rc != null);
			Debug.Assert( imageData );
			Debug.Assert( texHandle );
		
			if(!texHandle || !imageData)
				return false;
		
			if(flags & MPR_TI_PALETTE)
				return texHandle.Load(0, chromaKey, (BYTE*) imageData);
			else
				return texHandle.Load(0, chromaKey, (BYTE*) imageData);
		}

		
		/***************************************************************************\
			Release the MPR palette and palette data we're holding.
		\***************************************************************************/
		public void FreePalette()
		{
			if(palette)
			{
				if(palette.Release() == 0)
					palette = null;
		
				palette = null;
			}
		}

		public void FreeAll() { FreeTexture(); FreeImage(); }
		public DWORD TexHandle() { return (DWORD) texHandle; }
		
		// OW
		public void RestoreAll() 
		{
			if(texHandle) texHandle.RestoreAll();
		}

		#if _DEBUG
		void Texture::MemoryUsageReport()
		{
		}
		#endif

		
		private static string TexturePath = "";
		private static DXContext *rc = null; 
#endif
         public void FreeAll() { throw new NotImplementedException(); }
	}
}

