using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DWORD = System.Int16;

namespace FalconNet.CampaignBase
{
    //TODO Rename this class to something like TerrainBlock
    public class TBlock
    {

        public TBlock()
        {
            owners = 0;
        }
        //TODO public ~TBlock()
        //{
        //    ShiAssert(owners == 0);
        //};

        public TLevel Level()
        {
            return level;
        }

        public float GetMaxZ()
        {
            return maxZ;
        }

        public float GetMinZ()
        {
            return minZ;
        }

        // Return the row and column address of this physical block in the map grid
        public uint Row()
        {
            return blockRow;
        }

        public uint Col()
        {
            return blockCol;
        }

        // Return a pointer to the South West post in this block (array element 0)
        public Tpost Posts()
        {
            return posts[0];
        }

        // Return a pointer to the specified post (in local row/col coordinates)
        public Tpost Post(uint r, uint c)
        {
            Debug.Assert(r < (uint)Tpost.POSTS_ACROSS_BLOCK);
            Debug.Assert(c < (uint)Tpost.POSTS_ACROSS_BLOCK);
            return posts[r * Tpost.POSTS_ACROSS_BLOCK + c];
        }


        // Block coordinates of this block within its level
        protected uint blockRow;
        protected uint blockCol;

        // Offset of this block's data in the disk file
        protected DWORD fileOffset;

        // Max and min z values within this block
        protected float maxZ;
        protected float minZ;

        // The level to which this block belongs
        protected TLevel level;

        // Pointer to the array of posts (NULL when not loaded in memory)
        protected Tpost[] posts;

        // How many viewers are using this block -- if none, this block should be removed
        protected int owners;


        // Intialize and release block headers - only allowed from within a TLevel's critical section.
        protected void Setup(TLevel Level, uint r, uint c)
        {
            Debug.Assert(!IsOwned()); // Shouldn't be setting up an already owned block

            // Initialize the members of the block header structure
            level = Level;
            blockRow = r;
            blockCol = c;
            posts = null;
            minZ = 1e6f;
            maxZ = -1e6f;
            fileOffset = 0;

            // NOTE:  The posts pointer will be set when the data arrives
            // in the TLevel::PreProcessBlock() function
        }

        protected void Cleanup()
        {
            Debug.Assert(!IsOwned()); // Shouldn't be cleaning up a still owned block

            if (posts != null)
            {
#if USE_SH_POOLS
        // MemFreeFS( posts );
        MemFreePtr(posts);
#else
                //TODO delete posts;
#endif
            }

            posts = null;
            level = null;
        }

        // Set and check ownership of this block (block may belong to multiple viewers)
        // These should only be called from within the level's critical section
        protected void Reference() // Request use of this block
        {
            Debug.Assert(owners >= 0);
            owners++;
        }

        protected int Release() // Express disinterest in this block
        {
            Debug.Assert(IsOwned());
            owners--;

            // If this call returns 0, the caller should Cleanup() and then release this block's memory
            return owners;
        }

        protected bool IsOwned()
        {
            return (owners > 0);
        } // Are we in use by any entity?


        // Allow the level which owns each block to get at the private functions of this class.
        // Specificly, the reference and release functions.
        //friend TLevel;


#if USE_SH_POOLS
public:
    // Overload new/delete to use a SmartHeap fixed size pool
    void *operator new(size_t size)
    {
        ShiAssert(size == sizeof(TBlock));
        return MemAllocFS(pool);
    };
    void operator delete(void *mem)
    {
        if (mem) MemFreeFS(mem);
    };
    static void InitializeStorage()
    {
        pool = MemPoolInitFS(sizeof(TBlock), 4, 0);
    };
    static void ReleaseStorage()
    {
        MemPoolFree(pool);
    };
    static MEM_POOL pool;
#endif
    }
}
