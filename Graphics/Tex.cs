using System;
using DWORD = System.UInt32;
using WORD = System.UInt16;
using BOOL = System.Boolean;
using BYTE = System.Byte;
using System.Diagnostics;
using System.IO;
using FalconNet.Common;
using FalconNet.Common.Encoding;

namespace FalconNet.Graphics
{
    [Flags]
    public enum TexInfo
    {
        MPR_TI_DEFAULT = 0x000000,
        MPR_TI_MIPMAP = 0x000001,
        MPR_TI_CHROMAKEY = 0x000020,
        MPR_TI_ALPHA = 0x000040,
        MPR_TI_PALETTE = 0x000080,
        MPR_TI_DDS = 0x000100,
        MPR_TI_DXT1 = 0x000200,
        MPR_TI_DXT3 = 0x000400,
        MPR_TI_DXT5 = 0x000800,
        MPR_TI_16 = 0x001000,
        MPR_TI_32 = 0x002000,
        MPR_TI_64 = 0x004000,
        MPR_TI_128 = 0x008000,
        MPR_TI_256 = 0x010000,
        MPR_TI_512 = 0x020000,
        MPR_TI_1024 = 0x040000,
        MPR_TI_2048 = 0x080000,
        MPR_TI_INVALID = 0x100000,

        // ASSO
        MPR_TI_RGB16 = 0x200000,
        MPR_TI_RGB24 = 0x400000,
        MPR_TI_ARGB32 = 0x800000
    }

    public class Texture
    {
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
        public byte[] imageData;
        public Palette palette;
        public TexInfo flags;
        public DWORD chromaKey;

#if _DEBUG
		public static DWORD m_dwNumHandles = 0;		// Number of instances
		public static DWORD m_dwBitmapBytes = 0;		// Bytes allocated for bitmap copies
		public static DWORD m_dwTotalBytes = 0;			// Total number of bytes allocated (including bitmap copies and object size)
#endif


        // OW
        protected TextureHandle texHandle;

        /***************************************************************************\
            Store some useful global information.  The path is used for all
            texture loads through this interface and the RC is used for loading.
            This means that at present, only one device at a heading can load textures
            through this interface.
        \***************************************************************************/
        public static void SetupForDevice(DXContext texRC, string path)
        {
            // Store the texture path for future reference
            TexturePath = path;
            if (!TexturePath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                TexturePath += Path.DirectorySeparatorChar;

            rc = texRC;
            Palette.SetupForDevice(texRC);

#if TODO
		    TextureHandle.StaticInit(texRC.m_pD3DD);
  
#endif
        }

        /***************************************************************************\
            This is called when we're done working with a given device (as 
            represented by an RC).
        \***************************************************************************/
        public static void CleanupForDevice(DXContext texRC)
        {
            Palette.CleanupForDevice(texRC);
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
        public BOOL LoadImage(string filename, TexInfo newFlags = TexInfo.MPR_TI_DEFAULT, BOOL addDefaultPath = true)
        {
#if TODO
            string fullname;
            CImageFileMemory texFile;
            int result;

            Debug.Assert(filename != null);
            Debug.Assert(imageData == null);

            // Add in the users requested flags to any already set
            flags |= newFlags;

            // Add the texture path to the filename provided
            if (addDefaultPath)
            {
                fullname = TexturePath + filename;
            }

            else
            {
                fullname = filename;
            }

            // Make sure we recognize this file type
            texFile.imageType = CheckImageType(fullname);
            if (texFile.imageType == ImageType.IMAGE_TYPE_UNKNOWN)
            {
                Debug.WriteLine("Unrecognized image type");
                return false;
            }

            // If the image type has alpha in it, create an alpha per texel texture
            if (texFile.imageType == ImageType.IMAGE_TYPE_APL)
                flags |= TexInfo.MPR_TI_ALPHA;

            // Open the input file
            result = texFile.glOpenFileMem(fullname);
            if (result != 1)
            {
                Debug.WriteLine("Failed texture open");
                return false;
            }

            // Read the image data (note that ReadTextureImage will close texFile for us)
            texFile.glReadFileMem();
            result = ReadTextureImage(&texFile);

            if (result != GOOD_READ)
            {
                Debug.WriteLine("Failed texture read");
                return false;
            }

            // Store the image properties in our local storage
            if (texFile.image.width != texFile.image.height)
            {
                Debug.WriteLine("Texture isn't square");
                return false;
            }

            dimensions = texFile.image.width;
            Debug.Assert(dimensions == 32 || dimensions == 64 || dimensions == 128 || dimensions == 256);

            if (texFile.image.palette)
                chromaKey = texFile.image.palette[0];
            else
                chromaKey = 0xFFFF0000;		// Default to blue chroma key color

            // We only deal with 8 bit textures
            Debug.Assert((flags & TexInfo.MPR_TI_PALETTE) != 0);

            imageData = texFile.image.image;

            // Create a palette object if we don't already have one
            Debug.Assert(texFile.image.palette);
            if (palette == null)
            {
                palette = new Palette();
                palette.Setup32((DWORD*)texFile.image.palette);
            }

            else
                palette.Reference();

            // Release the image's palette data now that we've got our own copy
            glReleaseMemory(texFile.image.palette);

#if _DEBUG
			InterlockedExchangeAdd((long *) &m_dwTotalBytes, dimensions * dimensions);
#endif

            return true;
#endif
            throw new NotImplementedException();
        }


        /***************************************************************************\
            Release the MPR texture we're holding.
        \***************************************************************************/
        public void FreeImage()
        {
            if (texHandle != null)
            {
                //delete texHandle;
                texHandle = null;
            }

            if (imageData == null)
                FreePalette(); 	// We're totally gone, so get rid of our palette if we had one
        }

        public bool CreateTexture(string strName = null)
        {
            Debug.Assert(rc != null);
            Debug.Assert(imageData != null);
            Debug.Assert(texHandle == null);

            // JB 010318 CTD
            if (/* !F4IsBadReadPtr(palette,sizeof(Palette)) &&*/ (flags & TexInfo.MPR_TI_PALETTE) != 0)
            {
                palette.Activate();
                Debug.Assert(palette.palHandle != null);

                // JB 010616
                if (palette.palHandle == null)
                {
                    return false;
                }

                texHandle = new TextureHandle();
                Debug.Assert(texHandle != null);

                palette.palHandle.AttachToTexture(texHandle);
                texHandle.Create(strName, (WORD)flags, 8, (UInt16)(dimensions), (UInt16)(dimensions));

                // OW: Prevent a crash
                if (imageData != null)
                {
                    if (!texHandle.Load(0, chromaKey, imageData))
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                int width = 0;

                if ((flags & TexInfo.MPR_TI_16) != 0)
                    width = 16;
                else if ((flags & TexInfo.MPR_TI_32) != 0)
                    width = 32;
                else if ((flags & TexInfo.MPR_TI_64) != 0)
                    width = 64;
                else if ((flags & TexInfo.MPR_TI_128) != 0)
                    width = 128;
                else if ((flags & TexInfo.MPR_TI_256) != 0)
                    width = 256;
                else if ((flags & TexInfo.MPR_TI_512) != 0)
                    width = 512;
                else if ((flags & TexInfo.MPR_TI_1024) != 0)
                    width = 1024;
                else if ((flags & TexInfo.MPR_TI_2048) != 0)
                    width = 2048;

                texHandle = new TextureHandle();
                texHandle.Create(strName, (WORD)flags, 32, (UInt16)width, (UInt16)width);
                return texHandle.Load(0, 0, imageData, false, false, dimensions);
            }
        }

        public void FreeTexture()
        {
            if (texHandle != null)
            {
                //delete texHandle;
                texHandle = null;
            }

            // We're totally gone, so get rid of our palette if we had one
            if (imageData == null)
            {
                FreePalette();
            }
        }

        /***************************************************************************\
            Release the MPR palette and palette data we're holding.
        \***************************************************************************/
        public BOOL LoadAndCreate(string filename, TexInfo newFlags = TexInfo.MPR_TI_DEFAULT)
        {
            if (LoadImage(filename, newFlags))
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
            Debug.Assert(rc != null);
            Debug.Assert(imageData != null);
            Debug.Assert(texHandle != null);

            if (texHandle == null || imageData == null)
                return false;

            if ((flags & TexInfo.MPR_TI_PALETTE) != 0)
                return texHandle.Load(0, chromaKey, imageData);
            else
                return texHandle.Load(0, chromaKey, imageData);
        }


        /***************************************************************************\
            Release the MPR palette and palette data we're holding.
        \***************************************************************************/
        public void FreePalette()
        {
            if (palette != null)
            {
                if (palette.Release() == 0)
                    palette = null;

                palette = null;
            }
        }

        public void FreeAll() { FreeTexture(); FreeImage(); }
        public TextureHandle TexHandle() { return texHandle; }

        // OW
        public void RestoreAll()
        {
            if (texHandle != null) texHandle.RestoreAll();
        }

#if _DEBUG
		void Texture::MemoryUsageReport()
		{
		}
#endif


        private static string TexturePath = "";
        private static DXContext rc = null;

    }

    #region Encoding
    public static class TextureEncodingLE
    {
        public static void Encode(Stream stream, Texture val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, Texture rst)
        {
            rst.dimensions = Int32EncodingLE.Decode(stream);
            int val = Int32EncodingLE.Decode(stream); //imageData
            rst.flags = (TexInfo)UInt32EncodingLE.Decode(stream);
            rst.chromaKey = UInt32EncodingLE.Decode(stream);
            val = Int32EncodingLE.Decode(stream); //palette
            val = Int32EncodingLE.Decode(stream); //texHandle
        }

        public static int Size
        {
            get { return 4 * Int32EncodingLE.Size; }
        }
    }
    #endregion
}

