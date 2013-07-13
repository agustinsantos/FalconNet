using System;
using FalconNet.Common;
using DWORD = System.UInt32;
using WORD = System.UInt16;
using BOOL = System.Boolean;
using BYTE = System.Byte;
using System.Diagnostics;
using System.IO;

namespace FalconNet.Graphics
{
    public struct TexBankEntry
    {
        public long fileOffset; // How far into the .TEX file does the compressed data start?
        public long fileSize; // How big is the compressed data on disk?
        public Texture tex; // The container class which manages the texture data
        public Texture texN; // The container class which manages the texture data
        public int palID; // The offset into ThePaletteBank to use with this texture
        public int refCount; // How many objects want this texture right now
    }

    public struct TempTexBankEntry
    {
        public long fileOffset; // How far into the .TEX file does the compressed data start?
        public long fileSize; // How big is the compressed data on disk?
        public Texture tex; // The container class which manages the texture data
        public int palID; // The offset into ThePaletteBank to use with this texture
        public int refCount; // How many objects want this texture right now
    }

    public struct TexFlagsType
    {
        public bool OnOrder;
        public bool OnRelease;
    }

    public class TextureBankClass
    {
        public TextureBankClass()
        {
            nTextures = 0;
            TexturePool = null;
        }
        //TODO public ~TextureBankClass() {};


        public static int nTextures;
        public static TexBankEntry[] TexturePool;
        public static TempTexBankEntry[] TempTexturePool;

#if _DEBUG
    static int textureCount;
#endif


        protected static string baseName;
        protected static int deferredLoadState;
        protected static FileMemMap TexFileMap;
        protected static byte[] CompressedBuffer;
        protected static byte[] TexBuffer;
        protected static DWORD TexBufferSize;


        public static void Setup(int nEntries)
        {
            // Create our array of texture headers
            nTextures = nEntries;

            if (nEntries != 0)
            {
#if USE_SH_POOLS
        TexturePool = (TexBankEntry *)MemAllocPtr(gBSPLibMemPool, sizeof(TexBankEntry) * nEntries, 0);
        TempTexturePool = (TempTexBankEntry *)MemAllocPtr(gBSPLibMemPool, sizeof(TempTexBankEntry) * nEntries, 0);
#else
                TexturePool = new TexBankEntry[nEntries];
                TempTexturePool = new TempTexBankEntry[nEntries];
#endif

                TexFlags = new TexFlagsType[nTextures];
                // Allocte acche with a little safety margin
                CacheLoad = new short[nTextures + CACHE_MARGIN];
                CacheRelease = new short[nTextures + CACHE_MARGIN];
            }
            else
            {
                TexturePool = null;
                TempTexturePool = null;
                TexFlags = null;
                TexBuffer = null;
                CacheLoad = null;
                CacheRelease = null;
            }

            RatedLoad = true;
            LoadIn = LoadOut = ReleaseIn = ReleaseOut = 0;
        }

        public static void Cleanup()
        {
            // wait for any update on textures
            WaitUpdates();
            // Clean up our array of texture headers
#if USE_SH_POOLS
    MemFreePtr(TexturePool);
    MemFreePtr(TempTexturePool);
#else
            //delete[] TexturePool;
            //delete[] TempTexturePool;
#endif
            TexturePool = null;
            TempTexturePool = null;
            nTextures = 0;

            if (TexFlags != null)
                TexFlags = null;

            // Clean up the decompression buffer
#if USE_SH_POOLS
    MemFreePtr(CompressedBuffer);
#else
            //delete[] CompressedBuffer;
#endif
            CompressedBuffer = null;

            RatedLoad = false;

            if (CacheLoad != null)
                CacheLoad = null;

            if (CacheRelease != null)
                CacheRelease = null;

            LoadIn = LoadOut = ReleaseIn = ReleaseOut = 0;

            // Close our texture resource file
            if (TexFileMap.IsReady())
            {
                CloseTextureFile();
            }
        }

        public static void ReadPool(FileStream file, string basename)
        {
#if TODO
            int result;
            int maxCompressedSize;

            baseName = basename;

            // Read the number of textures in the pool
            result = file.Read(nTextures, sizeof(nTextures));

            if (nTextures == 0) return;

            // Read the size of the biggest compressed texture in the pool
            result = file.Read(maxCompressedSize, sizeof(maxCompressedSize));

            // HACK - KO.dxh version - Note maxCompressedSize is used for the old BMP
            // textures, not DDS textures.
            nVer = maxCompressedSize;

#if USE_SH_POOLS
    CompressedBuffer = (BYTE *)MemAllocPtr(gBSPLibMemPool, sizeof(BYTE) * maxCompressedSize, 0);
#else
            CompressedBuffer = new BYTE[maxCompressedSize];
#endif
            Debug.Assert(CompressedBuffer != null);

            if (CompressedBuffer != null)
            {
                ZeroMemory(CompressedBuffer, maxCompressedSize);
            }

            // Setup the pool
            Setup(nTextures);

            // sfr: Ok some ????? did this and now we cannot change Texture class a bit
            // since this stop working
            result = file.Read(TempTexturePool, sizeof(TempTexturePool) * nTextures);

            if (result < 0)
            {
                string message = "Reading object texture bank: " + strerror(errno);
                Debug.WriteLine(message);
            }

            for (int i = 0; i < nTextures; i++)
            {
                TexturePool[i].fileOffset = TempTexturePool[i].fileOffset;
                TexturePool[i].fileSize = TempTexturePool[i].fileSize;

                TexturePool[i].tex = TempTexturePool[i].tex;
                TexturePool[i].tex.flags |= TexInfo.MPR_TI_INVALID;
                TexturePool[i].texN = TempTexturePool[i].tex;
                TexturePool[i].texN.flags |= TexInfo.MPR_TI_INVALID;

                if (DisplayOptions.bMipmapping)
                {
                    TexturePool[i].tex.flags |= TexInfo.MPR_TI_MIPMAP;
                    TexturePool[i].texN.flags |= TexInfo.MPR_TI_MIPMAP;
                }

                TexturePool[i].palID = 0;//TempTexturePool[i].palID;
                TexturePool[i].refCount = 0;//TempTexturePool[i].refCount;
            }

            OpenTextureFile();
#endif
        }

        public static void FlushHandles()
        {
            int id;

            for (id = 0; id < nTextures; id++)
            {
                Debug.Assert(TexturePool[id].refCount == 0);

                while (TexturePool[id].refCount > 0)
                {
                    Release(id);
                }
            }

            WaitUpdates();
        }

        public static void Reference(int id)
        {
#if TODO
            int isLoaded;

            gDebugTextureID = id;

            Debug.Assert(IsValidIndex(id));

            // Get our reference to this texture recorded to ensure it doesn't disappear out from under us
            //EnterCriticalSection(&ObjectLOD::cs_ObjectLOD);

            isLoaded = TexturePool[id].refCount;
            Debug.Assert(isLoaded >= 0);
            TexturePool[id].refCount++;

            // If we already have the data, just verify that fact. Otherwise, load it.
            if (isLoaded)
            {
                ;
            }
            else
            {
                Debug.Assert(TexFileMap.IsReady());
                Debug.Assert(CompressedBuffer != null);
                if (TexturePool[id].tex.imageData != null)
                    return;
                Debug.Assert(TexturePool[id].tex.TexHandle() == null);

                // Get the palette pointer
                // would be great if we could set a flag saying this palette comes from bank...
                // but since we cannot add anything to texture structure (because Jammer read them
                // directly from file instead of from a method) I make the check when releasing
                // the palette.
                TexturePool[id].tex.SetPalette(&ThePaletteBank.PalettePool[TexturePool[id].palID]);
                Debug.Assert(TexturePool[id].tex.GetPalette());
                TexturePool[id].tex.GetPalette().Reference();

                // Mark for the request if not already marked
                if (!TexFlags[id].OnOrder)
                {
                    TexFlags[id].OnOrder = true;
                    // put into load cache
                    CacheLoad[LoadIn++] = id;

                    // Ring the pointer
                    if (LoadIn >= (nTextures + CACHE_MARGIN)) LoadIn = 0;

                    // Kick the Loader
                    TheLoader.WakeUp();

                }

            }

            gDebugTextureID = -1;

#endif
            throw new NotImplementedException();
        }

        public static void Release(int id)
        {
#if TODO
            Debug.Assert(IsValidIndex(id));
            Debug.Assert(TexturePool[id].refCount > 0);

            // RED - no reference, no party... !!!!!
            if (!TexturePool[id].refCount)
                return;

            TexturePool[id].refCount--;

            if (TexturePool[id].refCount == 0)
            {
                if (!TexFlags[id].OnRelease)
                {
                    TexFlags[id].OnRelease = true;
                    // put into load cache
                    CacheRelease[ReleaseIn++] = id;

                    // Ring the pointer
                    if (ReleaseIn >= (nTextures + CACHE_MARGIN)) ReleaseIn = 0;

                    // Kick the Loader
                    TheLoader.WakeUp();
                }
            }
#endif
            throw new NotImplementedException();
        }

        public static void Select(int id) { }

        public static void RestoreAll() { }

        public static void SetDeferredLoad(BOOL state)
        {
#if TODO
            LoaderQ request;

            // Allocate space for the async request
            request = new LoaderQ();

            if (!request)
                Debug.WriteLine("Failed to allocate memory for a object texture load state change request");

            // Build the data transfer request to get the required object data
            request.filename = null;
            request.fileoffset = 0;
            request.callback = LoaderCallBack;
            request.parameter = (void*)state;

            // Submit the request to the asynchronous loader
            TheLoader.EnqueueRequest(request);
#endif
            throw new NotImplementedException();
        }

        public static BOOL IsValidIndex(int id)
        {
            return ((id >= 0) && (id < nTextures));
        }

        public static void SyncDDSTextures(bool bForce = false)
        {
            string szFile;
            FileStream fp;

            Debug.Assert(TexturePool != null);

            Directory.CreateDirectory(baseName);

            for (DWORD id = 0; id < (DWORD)nTextures; id++)
            {
                szFile = baseName + Path.DirectorySeparatorChar + id + ".dds";
                fp = new FileStream(szFile, FileMode.Open);

                if (fp == null || bForce)
                {
                    if (fp != null)
                        fp.Close();

                    UnpackPalettizedTexture(id);
                }
                else
                    fp.Close();

                TexturePool[id].tex.flags |= TexInfo.MPR_TI_DDS;
                TexturePool[id].tex.flags &= ~TexInfo.MPR_TI_PALETTE;
                TexturePool[id].texN.flags |= TexInfo.MPR_TI_DDS;
                TexturePool[id].texN.flags &= ~TexInfo.MPR_TI_PALETTE;
            }
        }

        public static void RestoreTexturePool()
        {
            for (int i = 0; i < nTextures; i++)
            {
                TexturePool[i].fileOffset = TempTexturePool[i].fileOffset;
                TexturePool[i].fileSize = TempTexturePool[i].fileSize;
                TexturePool[i].tex = TempTexturePool[i].tex;
                TexturePool[i].texN = TempTexturePool[i].tex;
                TexturePool[i].palID = TempTexturePool[i].palID;
                TexturePool[i].refCount = TempTexturePool[i].refCount;
            }
        }

        public static void SelectHandle(DWORD h)
        {
#if TODO
            TheStateStack.context.SelectTexture1(TexHandle);
#endif
            throw new NotImplementedException();
        }

        public static DWORD GetHandle(DWORD id)
        {
#if TODO
            // if already on release, avoid using or requesting it
            if (TexFlags[id].OnRelease) return null;

            // if the Handle is prsent, return it
            if (IsValidIndex(id) && TexturePool[id].tex.TexHandle()) return TexturePool[id].tex.TexHandle();

            // return  a null pointer that means BLANK SURFACE
            return null;
#endif
            throw new NotImplementedException();
        }



        protected static void AllocCompressedBuffer(int size)
        {
#if TODO
#if USE_SH_POOLS
    CompressedBuffer = (BYTE *)MemAllocPtr(gBSPLibMemPool, sizeof(BYTE) * maxCompressedSize, 0);
#else
            CompressedBuffer = new byte[maxCompressedSize];
#endif

            Debug.Assert(CompressedBuffer != null);

            if (CompressedBuffer != null)
            {
                ZeroMemory(CompressedBuffer, maxCompressedSize);
            }
#endif
        }

        protected static void FreeCompressedBuffer()
        {
#if USE_SH_POOLS
    MemFreePtr(CompressedBuffer);
#else
            //delete[] CompressedBuffer;
#endif

            CompressedBuffer = null;
        }

        protected static void OpenTextureFile()
        {
#if TODO
            string filename;

            Debug.Assert(!TexFileMap.IsReady());

            filename = baseName + ".tex";

            if (!TexFileMap.Open(filename, false, !g_bUseMappedFiles))
            {
                string message = "Failed to open object texture file " + filename;
                Debug.Fail(message);
            }
#endif
        }

        protected static void ReadImageData(int id, bool forceNoDDS = false)
        {
#if TODO
            int retval;
            int size;
            byte[] cdata;
            //sfr: added for more control
            int cdataSize;


            Debug.Assert(TexturePool[id].refCount != 0);

            if (!forceNoDDS && DisplayOptions.m_texMode == DisplayOptionsClass.TEX_MODE_DDS)
            {
                ReadImageDDS(id);
                ReadImageDDSN(id);
                return;
            }

            if (g_bUseMappedFiles)
            {
                cdata = TexFileMap.GetData(TexturePool[id].fileOffset, TexturePool[id].fileSize);
                cdataSize = TexturePool[id].fileSize - TexturePool[id].fileOffset;
                Debug.Assert(cdata != null );
            }
            else
            {
                if (!TexFileMap.ReadDataAt(TexturePool[id].fileOffset, CompressedBuffer, TexturePool[id].fileSize))
                {
                    string message;
                    message = strerror(errno) + ": Bad object texture seek (" + TexturePool[id].fileOffset + ")";
                    Debug.WriteLine(message);
                }

                cdata = CompressedBuffer;
                //sfr: in this case, im not sure size is this
                // FRB - fileOffset - fileOffset ????
                //cdataSize = TexturePool[id].fileOffset - TexturePool[id].fileOffset;
                cdataSize = TexturePool[id].fileSize - TexturePool[id].fileOffset;
            }

            // Allocate memory for the new texture
            size = TexturePool[id].tex.dimensions;
            size = size * size;

            TexturePool[id].tex.imageData = glAllocateMemory(size, false);
            Debug.Assert(TexturePool[id].tex.imageData != null);

            // Uncompress the data into the texture structure
            //sfr: using new cdataSize for control
            retval = LZSS_Expand(cdata, cdataSize, (BYTE*)TexturePool[id].tex.imageData, size);
            Debug.Assert(retval == TexturePool[id].fileSize);

#if _DEBUG
    textureCount++;
#endif
#endif
            throw new NotImplementedException();
        }

        protected static void CloseTextureFile()
        {
            TexFileMap.Close();
        }

        protected static void LoaderCallBack(LoaderQ request)
        {
#if TODO
            BOOL state = (int)request.parameter;

            //EnterCriticalSection(&ObjectLOD::cs_ObjectLOD);

            // If we're turning deferred loads off, go back and do all the loads we held up
            if (deferredLoadState && !state)
            {
                DWORD Count = 5;

                // Check each texture
                for (int id = 0; id < nTextures; id++)
                {
                    // See if it is in use
                    if (TexturePool[id].refCount)

                        // This one is in use. Is it already loaded?
                        if (/*!TexturePool[id].tex.imageData &&*/ !TexturePool[id].tex.TexHandle())
                        {

                            // Nope, go get it.
                            if (!TexturePool[id].tex.imageData) ReadImageData(id);

                            TexturePool[id].tex.CreateTexture();
                            Count--;
                            //TexturePool[id].tex.FreeImage();
                        }

                    if (!Count) break;
                }
            }

            // Now store the new state
            deferredLoadState = state;

            //LeaveCriticalSection(&ObjectLOD::cs_ObjectLOD);

            // Free the request queue entry
            //delete request;
#endif
            throw new NotImplementedException();
        }

        protected static void UnpackPalettizedTexture(DWORD id)
        {
#if TODO
            string szFile;

            CreateDirectory(baseName, null);

            if (TexturePool[id].tex.dimensions > 0)
            {
                //sfr: (see my comment regarding palette origin above)
                TexturePool[id].tex.SetPalette(&ThePaletteBank.PalettePool[TexturePool[id].palID]);
                Debug.Assert(TexturePool[id].tex.GetPalette());
                TexturePool[id].tex.GetPalette().Reference();

                ReadImageData(id, true);
                sprintf(szFile, "%s\\%d", baseName, id);
                TexturePool[id].tex.DumpImageToFile(szFile, TexturePool[id].palID);
                Release(id);
            }
            else
            {
                sprintf(szFile, "%s\\%d.dds", baseName, id);
                FileStream fp = fopen(szFile, "wb");
                fp.Close();
            }
#endif
            throw new NotImplementedException();
        }

        protected static void ReadImageDDS(DWORD id)
        {
#if TODO
            DDSURFACEDESC2 ddsd;
            DWORD dwSize, dwMagic;
            string szFile;
            FileStream fp;

            TexturePool[id].tex.flags |= TexInfo.MPR_TI_DDS;
            TexturePool[id].tex.flags &= ~TexInfo.MPR_TI_PALETTE;

            sprintf(szFile, "%s\\%d.dds", baseName, id);
            fp = fopen(szFile, "rb");

            // RV - RED - Avoid CTD if a missing texture
            if (!fp) return;

            fread(&dwMagic, 1, sizeof(DWORD), fp);
            Debug.Assert(dwMagic == DDraw.MAKEFOURCC('D', 'D', 'S', ' '));

            // Read first compressed mipmap
            fread(&ddsd, 1, sizeof(DDSURFACEDESC2), fp);

            // MLR 1/25/2004 - Little kludge so F4 can read DDS files made by dxtex
            if (ddsd.dwLinearSize == 0)
            {
                if (ddsd.ddpfPixelFormat.dwFourCC == DDraw.MAKEFOURCC('D', 'X', 'T', '3') ||
                    ddsd.ddpfPixelFormat.dwFourCC == DDraw.MAKEFOURCC('D', 'X', 'T', '5'))
                {
                    ddsd.dwLinearSize = ddsd.dwWidth * ddsd.dwWidth;
                    ddsd.dwFlags |= DDSD_LINEARSIZE;
                }

                if (ddsd.ddpfPixelFormat.dwFourCC == DDraw.MAKEFOURCC('D', 'X', 'T', '1'))
                {
                    ddsd.dwLinearSize = ddsd.dwWidth * ddsd.dwWidth / 2;
                    ddsd.dwFlags |= DDSD_LINEARSIZE;
                }
           }


            Debug.Assert(ddsd.dwFlags & DDSD_LINEARSIZE);

            switch (ddsd.ddpfPixelFormat.dwFourCC)
            {
                case DDraw.MAKEFOURCC('D', 'X', 'T', '1'):
                    TexturePool[id].tex.flags |= MPR_TI_DXT1;
                    break;

                case DDraw.MAKEFOURCC('D', 'X', 'T', '3'):
                    TexturePool[id].tex.flags |= MPR_TI_DXT3;
                    break;

                case DDraw.MAKEFOURCC('D', 'X', 'T', '5'):
                    TexturePool[id].tex.flags |= MPR_TI_DXT5;
                    break;

                default:
                    Debug.Assert(false);
            }

            switch (ddsd.dwWidth)
            {
                case 16:
                    TexturePool[id].tex.flags |= TexInfo.MPR_TI_16;
                    break;

                case 32:
                    TexturePool[id].tex.flags |= TexInfo.MPR_TI_32;
                    break;

                case 64:
                    TexturePool[id].tex.flags |= TexInfo.MPR_TI_64;
                    break;

                case 128:
                    TexturePool[id].tex.flags |= TexInfo.MPR_TI_128;
                    break;

                case 256:
                    TexturePool[id].tex.flags |= TexInfo.MPR_TI_256;
                    break;

                case 512:
                    TexturePool[id].tex.flags |= TexInfo.MPR_TI_512;
                    break;

                case 1024:
                    TexturePool[id].tex.flags |= TexInfo.MPR_TI_1024;
                    break;

                case 2048:
                    TexturePool[id].tex.flags |= TexInfo.MPR_TI_2048;
                    break;

                default:
                    Debug.Assert(false);
            }

            dwSize = ddsd.dwLinearSize;
            TexturePool[id].tex.imageData = (BYTE*)glAllocateMemory(dwSize, false);
            fread(TexturePool[id].tex.imageData, 1, dwSize, fp);

            TexturePool[id].tex.dimensions = dwSize;

            fp.Close();

#if _DEBUG
    textureCount++;
#endif
#endif
            throw new NotImplementedException();
        }
        protected static void ReadImageDDSN(DWORD id)
        {
#if TODO
            DDSURFACEDESC2 ddsd;
            DWORD dwSize, dwMagic;
            string szFile;
            FileStream fp;

            sprintf(szFile, "%s\\%dN.dds", baseName, id);
            fp = fopen(szFile, "rb");

            if (!fp)
            {
                return;
            }

            TexturePool[id].texN.flags |= TexInfo.MPR_TI_DDS;
            TexturePool[id].texN.flags &= ~TexInfo.MPR_TI_PALETTE;
            TexturePool[id].texN.flags &= ~TexInfo.MPR_TI_INVALID;

            fread(&dwMagic, 1, sizeof(DWORD), fp);
            Debug.Assert(dwMagic == DDraw.MAKEFOURCC('D', 'D', 'S', ' '));

            // Read first compressed mipmap
            fread(&ddsd, 1, sizeof(DDSURFACEDESC2), fp);

            // MLR 1/25/2004 - Little kludge so F4 can read DDS files made by dxtex
            if (ddsd.dwLinearSize == 0)
            {
                if (ddsd.ddpfPixelFormat.dwFourCC == DDraw.MAKEFOURCC('D', 'X', 'T', '3') ||
                    ddsd.ddpfPixelFormat.dwFourCC == DDraw.MAKEFOURCC('D', 'X', 'T', '5'))
                {
                    ddsd.dwLinearSize = ddsd.dwWidth * ddsd.dwWidth;
                    ddsd.dwFlags |= DDSD_LINEARSIZE;
                }

                if (ddsd.ddpfPixelFormat.dwFourCC == DDraw.MAKEFOURCC('D', 'X', 'T', '1'))
                {
                    ddsd.dwLinearSize = ddsd.dwWidth * ddsd.dwWidth / 2;
                    ddsd.dwFlags |= DDSD_LINEARSIZE;
                }
            }

            Debug.Assert(ddsd.dwFlags & DDSD_LINEARSIZE);

            switch (ddsd.ddpfPixelFormat.dwFourCC)
            {
                case DDraw.MAKEFOURCC('D', 'X', 'T', '1'):
                    TexturePool[id].texN.flags |= TexInfo.MPR_TI_DXT1;
                    break;

                case DDraw.MAKEFOURCC('D', 'X', 'T', '3'):
                    TexturePool[id].tex.flags |= TexInfo.MPR_TI_DXT3;
                    break;

                case DDraw.MAKEFOURCC('D', 'X', 'T', '5'):
                    TexturePool[id].texN.flags |= TexInfo.MPR_TI_DXT5;
                    break;

                default:
                    Debug.Assert(false);
            }

            switch (ddsd.dwWidth)
            {
                case 16:
                    TexturePool[id].texN.flags |= TexInfo.MPR_TI_16;
                    break;

                case 32:
                    TexturePool[id].texN.flags |= TexInfo.MPR_TI_32;
                    break;

                case 64:
                    TexturePool[id].texN.flags |= TexInfo.MPR_TI_64;
                    break;

                case 128:
                    TexturePool[id].texN.flags |= TexInfo.MPR_TI_128;
                    break;

                case 256:
                    TexturePool[id].texN.flags |= TexInfo.MPR_TI_256;
                    break;

                case 512:
                    TexturePool[id].texN.flags |= TexInfo.MPR_TI_512;
                    break;

                case 1024:
                    TexturePool[id].texN.flags |= TexInfo.MPR_TI_1024;
                    break;

                case 2048:
                    TexturePool[id].texN.flags |= TexInfo.MPR_TI_2048;
                    break;

                default:
                    Debug.Assert(false);
            }

            dwSize = ddsd.dwLinearSize;
            TexturePool[id].texN.imageData = (BYTE*)glAllocateMemory(dwSize, false);
            fread(TexturePool[id].texN.imageData, 1, dwSize, fp);

            TexturePool[id].texN.dimensions = dwSize;

            fp.Close();

#if _DEBUG
    textureCount++;
#endif
#endif
            throw new NotImplementedException();
        }

        // New texture management...
        protected static TexFlagsType[] TexFlags;
        protected static void CreateCallBack(LoaderQ request) { }
        protected static bool RatedLoad; // This flag makes textures loaded once x loader frame
        protected static short[] CacheLoad, CacheRelease;
        protected static short LoadIn, LoadOut, ReleaseIn, ReleaseOut;


        public static bool UpdateBank()
        {
#if TODO
            DWORD id;

            // till when data to update into caches
            while (LoadIn != LoadOut || ReleaseIn != ReleaseOut)
            {

                // check for textures to be released
                if (ReleaseIn != ReleaseOut)
                {
                    // get the 1st texture Id from cache
                    id = CacheRelease[ReleaseOut++];

                    // if not an order again, and no Referenced, release it
                    if (!TexFlags[id].OnOrder && !TexturePool[id].refCount && TexFlags[id].OnRelease) TexturePool[id].tex.FreeAll();

                    // clear flag, in any case
                    TexFlags[id].OnRelease = false;

                    // ring the pointer
                    if (ReleaseOut >= (nTextures + CACHE_MARGIN)) ReleaseOut = 0;

                    // if any action, terminate here
                    if (RatedLoad) return true;
                }

                // check for textures to be released
                if (LoadIn != LoadOut)
                {
                    // get the 1st texture Id from cache
                    id = CacheLoad[LoadOut++];

                    // if Texture not yet loaded, load it
                    if (TexturePool[id].tex.imageData == null) ReadImageData(id);

                    // if Texture not yet crated, crate it
                    if (TexturePool[id].tex.TexHandle() == 0) TexturePool[id].tex.CreateTexture();

                    // clear flag, in any case
                    TexFlags[id].OnOrder = false;

                    // ring the pointer
                    if (LoadOut >= (nTextures + CACHE_MARGIN)) LoadOut = 0;

                    // if any action, terminate here
                    if (RatedLoad) return true;
                }

            }

            // if here, nothing done, back is up to date
            return false;
#endif
            throw new NotImplementedException();
        }

        public static void WaitUpdates()
        {
#if TODO
            // if no data to wait, exit here
            if (LoadIn == LoadOut && ReleaseIn == ReleaseOut) return;

            // Pause the Loader...
            TheLoader.SetPause(true);

            while (!TheLoader.Paused()) ;

            // Not slow loading
            RatedLoad = false;

            // Parse all objects till any opration to do
            while (UpdateBank()) ;

            // Restore rated loading
            RatedLoad = true;
            // Run the Loader again
            TheLoader.SetPause(false);
#endif
            throw new NotImplementedException();
        }


        public static void ReferenceTexSet(DWORD[] TexList, DWORD Nr)
        {
#if TODO
            for (int i = 0; i < Nr; i++)
            {
                Reference(TexList[i]);
            }
#endif
            throw new NotImplementedException();
        }

        public static void ReleaseTexSet(DWORD[] TexList, DWORD Nr)
        {
#if TODO
            for (int i = 0; i < Nr; i++)
            {
                Release(TexList[i]);
            }
#endif
            throw new NotImplementedException();
        }

        public static void SetRatedLoad(bool Status)
        {
            RatedLoad = false;
        }
        public const int TEX_ON_ORDER = 0x00000001;
        public const int TEX_ON_RELEASE = 0x00000002;
        public const int CACHE_MARGIN = 32;
    }

}
