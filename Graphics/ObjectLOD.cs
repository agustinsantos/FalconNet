using FalconNet.Common;
using System;
using System.Diagnostics;


namespace FalconNet.Graphics
{
    public class LoaderQ { public LoaderQ() { throw new NotImplementedException(); } }

    public class ObjectLOD
    {
        public static ObjectLOD TheObjectLODs;
        public static int TheObjectLODsCount;

        public ObjectLOD()
        { throw new NotImplementedException(); }
        // TODO public  ~ObjectLOD();

        public static void SetupEmptyTable(int numEntries)
        { throw new NotImplementedException(); }
        public static void SetupTable(int file, string basename)
        { throw new NotImplementedException(); }
        public static void CleanupTable()
        { throw new NotImplementedException(); }

        //TODO public void	Reference( )		{ refCount++; };
        //TODO public void	Release( )		{ refCount--; if (refCount==0) Unload(); };

        public bool Fetch()
        { throw new NotImplementedException(); }	// True means ready to draw.  False means still waiting.
        public void Draw() { Debug.Assert(root != null); root.Draw(); }

        public static Object cs_ObjectLOD;


        // Handle asychronous data loading
        protected void RequestLoad()
        { throw new NotImplementedException(); }
        protected static void LoaderCallBack(LoaderQ request)
        { throw new NotImplementedException(); }
        protected void Unload()
        { throw new NotImplementedException(); }

        protected static FileMemMap ObjectLodMap; // JPO - MMFILE
        //	static int				objectFile;
        protected static BNodeType[] tagListBuffer;
        protected int refCount;		// How many instances of this LOD are in use
        protected short onOrder;		// TRUE when IO is pending (normally 1, -1 means no longer needed)


        // Object flag values
        public enum FlagVal { NONE = 0, PERSP_CORR = 1, };

        public short flags;			// Special handling flags for this visual
        public BRoot root;			// NULL until loaded, then pointer to node tree
        public UInt32 fileoffset;		// Where in the disk file is this record's tree stored
        public UInt32 filesize;		// How big the disk representation of this record's tree

#if USE_SMART_HEAP
  public:
	static MEM_POOL	pool;
#endif
#if _DEBUG
	public static int lodsLoaded; // JPO - some stats
#endif
    }
}
