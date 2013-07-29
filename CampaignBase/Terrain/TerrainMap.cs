using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using DWORD = System.Int16;

namespace FalconNet.CampaignBase
{
    //TODO Rename this class to something like TerrainMap
    public class TLevel
    {
        public TLevel()
        {
            blocks = null;
        }
        //TODO  public ~TLevel() {};

        public void Setup(int level, uint width, uint height, string mapPath)
        {
            string filename;
            FileStream  offsetFile;
            DWORD bytes;

#if USE_SH_POOLS
    // gTPostMemPool = MemPoolInitFS( sizeof(Tpost)*POSTS_PER_BLOCK, 24, 0 );
    gTPostMemPool = MemPoolInit(0);
#endif

            // Setup the properties of this level
            feet_per_post = Tpost.LEVEL_POST_TO_WORLD(1, level);
            feet_per_block = Tpost.LEVEL_BLOCK_TO_WORLD(1, level);
            myLevel = level;
            blocks_wide = width;
            blocks_high = height;

#if TODO
            // Create the synchronization object we'll need
            InitializeCriticalSection(cs_blockArray);


            // Allocate memory for the block pointer array
            blocks = new TBlock[blocks_wide * blocks_high];

            if (blocks == null)
            {
                throw new ApplicationException("Failed to allocate memory for block pointer array");
            }

            // Open the block offset file for this level
             filename  = mapPath + "\\Theater.o" + level;

            if (File.Exists(filename))
            {
                offsetFile = new FileStream(filename, FileMode.Open, FileAccess.Read);

                // Read the file offsets into the post pointer array
                bytes = read(offsetFile, blocks, sizeof(TBlock) * blocks_wide * blocks_high);

                if (bytes != sizeof(TBlock) * blocks_wide * blocks_high)
                {
                    throw new ApplicationException("Couldn't read block offset data");
                }

                offsetFile.Close();

            }
#endif
#if TODO
            else
            {
                // We need to exit cleanly... since this path fails anyway
                return;

                // We couldn't open the offsets file, so we'll assume regular spacing
                for (int i = 0; i < blocks_wide * blocks_high; i++)
                {
                    blocks[i].offset = i * sizeof(TdiskPost) * POSTS_PER_BLOCK;
                }

            }

            // Walk through the offsets and shift them up to clear the low bit.
            for (int i = 0; i < blocks_wide * blocks_high; i++)
            {

                // We're dropping the top bit, so no legal offset can have it set
                Debug.Assert(!(blocks[i].offset & 0x80000000));
                blocks[i].offset = (blocks[i].offset << 1) | 1;
                Debug.Assert((blocks[i].offset & 0x00000001));
            }


            // Open the post file for this level
            filename = mapPath +"\\Theater.l" + level;

            if (postFileMap.Open(filename, false, !g_bUseMappedFiles) == false)
                return;

            // Initialize the lighting conditions and register for future time of day updates
            TimeUpdateCallback(this);
            TheTimeManager.RegisterTimeUpdateCB(TimeUpdateCallback, this);
#endif
        }

        public void Cleanup()
        {
#if TODO
            Debug.Assert(IsReady());

            blocks_wide = 0;
            blocks_high = 0;

            feet_per_post = 0.0f;
            feet_per_block = 0.0f;

            // Wait for all outstanding TLoader requests to complete or be canceled.
            // Note: We only really have to wait for all _our_ requests to complete, but
            // waiting for an empty queue is easier, though _could_ starve us.
            TheLoader.WaitLoader();

            // Stop receiving time updates
            TheTimeManager.ReleaseTimeUpdateCB(TimeUpdateCallback, this);

            // Close the post file for this level
            postFileMap.Close();

            // Release the block pointer array memory
            // TODO delete[] blocks;
            blocks = null;

            // Release the sychronization objects we've been using
            DeleteCriticalSection(&cs_blockArray);

#if USE_SH_POOLS

    if (gTPostMemPool)
    {
        MemPoolFree(gTPostMemPool);
        gTPostMemPool = null;
    }

#endif
#endif
        }


        public bool IsReady()
        {
            return (blocks != null);
        }

        public int LOD()
        {
            return myLevel;
        }

        public float FTperPOST()
        {
            return feet_per_post;
        }
        public float FTperBLOCK()
        {
            return feet_per_block;
        }

        public uint PostsWide()
        {
            return blocks_wide * Tpost.POSTS_PER_BLOCK;
        } // How many posts across

        public uint PostsHigh()
        {
            return blocks_high * Tpost.POSTS_PER_BLOCK;
        } // How many posts high

        public uint BlocksWide()
        {
            return blocks_wide;
        }   // How many blocks across is this level

        public uint BlocksHigh()
        {
            return blocks_high;
        } // How many blocks high is this level


        // This function loads and/or increments the reference count of the given block
        public TBlock RequestBlockOwnership(int r, int c)
        {
#if TODO
    TBlock block;

    Debug.Assert(IsReady());


    EnterCriticalSection(&cs_blockArray);

    // Clamp the row and column address if required
    VirtualToPhysicalBlockAddress(&r, &c);

    // Look for a pointer to this block in memory -- reference it if found
    block = GetBlockPtr(r, c);

    if (block)
    {

        // Reference the block on behalf of our caller
        block.Reference();

    }
    else
    {

        if ((r < 0) || (r >= (int)BlocksHigh()) || (c < 0) || (c >= (int)BlocksWide()))
        {
            {
                // We're off the map, so don't return a block
                LeaveCriticalSection(&cs_blockArray);
                return null;
            }

        }
        else
        {

            // Block wasn't found, we we'll create one and ask the async loader to fetch the data.
            block = new TBlock;

            if (!block)
            {
                ShiError("Failed to allocate memory for a block header");
            }

            block.Setup(this, r, c);

            // Mark this block as owned by the requestor
            block.Reference();

            // Put the block header into the block array to indicate that the data is on order
            SetBlockPtr(r, c, block);

            // Allocate space for the data transfer and the message requesting it
            LoaderQ *request = new LoaderQ;

            if (!request)
            {
                ShiError("Failed to allocate memory for a block read request");
            }

            // Build the data transfer request to get the post data
            request.filename = null;
            request.fileoffset = block.fileOffset >> 1;
            request.callback = LoaderCallBack;
            request.parameter = block;

            // Submit the request to the asynchronous loader
            TheLoader.EnqueueRequest(request);
        }
    }


    LeaveCriticalSection(&cs_blockArray);

    Debug.Assert(block);

    return block;
#endif
            throw new NotImplementedException();
        }

        // This function assumes the block is already owned by the caller
        public TBlock GetOwnedBlock(int r, int c)
        {
#if TODO
            TBlock block;

            Debug.Assert(IsReady());


            // Clamp the row and column address if required
            VirtualToPhysicalBlockAddress(&r, &c);

            // Look for a pointer to this block in memory
            block = GetBlockPtr(r, c);
            Debug.Assert(block && block.IsOwned());

            return block; 
#endif            
            throw new NotImplementedException();
        }

        // The following function decrements the reference count of the given block and frees it (if 0)
        public void ReleaseBlock(TBlock block)
        {
#if TODO
            int i;

            Debug.Assert(IsReady());
            Debug.Assert(block);

            EnterCriticalSection(&cs_blockArray);

            // Express disinterest in the block.  If no one else owns it, we may be able to free it
            if (block.Release() == 0)
            {

                // We can free the block if TheLoader is either already done, or hasn't started yet.
                if (block.Posts() ||
                    TheLoader.CancelRequest(LoaderCallBack, block, null, block.fileOffset >> 1))
                {

                    SetBlockPtr(block.Row(), block.Col(), null);

                    // If we actually have the block data already, free the associated textures
                    if (block.Posts())
                    {
                        // Release any textures this block of posts requested
                        if (LOD() <= TheMap.LastNearTexLOD())
                        {
                            for (i = POSTS_PER_BLOCK - 1; i >= 0; i--)
                            {
                                TheTerrTextures.Release((block.Posts() + i).texID);
                            }
                        }
                        else
                        {
                            for (i = POSTS_PER_BLOCK - 1; i >= 0; i--)
                            {
                                TheFarTextures.Release((block.Posts() + i).texID);
                            }
                        }
                    }

                    // Cleanup the block header
                    block.Cleanup();
                    delete block;
                }

            }

            LeaveCriticalSection(&cs_blockArray); 
#endif
            throw new NotImplementedException();
        }




        protected uint blocks_wide;   // How many blocks across is this level
        protected uint blocks_high; // How many blocks high is this level

        protected TBlock[] blocks; // Point to an array of pointers to blocks (null means not loaded)

        protected float feet_per_post;
        protected float feet_per_block;

        protected float lightLevel; // Light level from 0 to 1

        protected int myLevel; // (0 is highest detail, goes up from there by ones)

#if TODO
        protected PostFile postFileMap;     // mem mapped post file

        protected CRITICAL_SECTION cs_blockArray;


        // Handle asychronous block loading
        protected static void LoaderCallBack(LoaderQ request); // Dummy front end
        protected void PreProcessBlock(LoaderQ request); // Actual worker function

        // Handle time of day and lighting notifications
        protected static void TimeUpdateCallback(void* self);
#endif

        // Map from virutal block addresses (unbounded) to physical ones (one in the map)
        protected void VirtualToPhysicalBlockAddress(ref int r, ref int c)
        {
            if ((r >= (int)blocks_high) || (r < 0) || (c >= (int)blocks_wide) || (c < 0))
            {
                r = 0;
                c = 0;
            }
        }

        // Set/get the block pointer associated with a physcial block address
        protected void SetBlockPtr(uint r, uint c, TBlock block)
        {
            throw new NotImplementedException();
        }
        protected TBlock GetBlockPtr(uint r, uint c)
        {
            throw new NotImplementedException();
        }


        // The following functions should not be compiled into the final game...
        public void SaveBlock(TBlock block)
        {
            throw new NotImplementedException();
        }
        public static void DebugDisplayInit()
        {
            throw new NotImplementedException();
        }
        public static void DebugDisplayOutput()
        {
            throw new NotImplementedException();
        }
        public void DebugDisplayLevel()
        {
            throw new NotImplementedException();
        }

    }
}
