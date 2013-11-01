using FalconNet.Common;
using FalconNet.Common.Encoding;
using FalconNet.F4Common;
using log4net;
using System;
using System.Diagnostics;
using System.IO;
using DWORD = System.UInt32;

namespace FalconNet.Graphics
{
    public class LoaderQ { public LoaderQ() { throw new NotImplementedException(); } }
    public class HdrParent // HDR parents
    {
        public float[] shadowSize = new float[7];
        public uint fileOffset;
        public uint fileSize;
        public ushort nTextures;
        public ushort nDynamics;
        public byte nLODs;
        public byte nSwitches;
        public byte nDOFs;
        public byte nSlots;
        public ushort Unk1;
        public ushort Unk2;
    }
    public class ObjectLOD
    {
        public static ObjectLOD[] TheObjectLODs;
        public static int TheObjectLODsCount;
        public static int maxTagList;

        public ObjectLOD()
        {
            //throw new NotImplementedException();
        }

        // TODO public  ~ObjectLOD();

        public static void SetupEmptyTable(int numEntries)
        { throw new NotImplementedException(); }

        public static void SetupTable(Stream file, string basename)
        {     // Read the number of elements in the longest tag list we saw in the LOD data
            maxTagList = Int32EncodingLE.Decode(file);

            // Read the length of the LOD header array
            TheObjectLODsCount = Int32EncodingLE.Decode(file);

            // Allocate memory for the LOD array
            TheObjectLODs = new ObjectLOD[TheObjectLODsCount];

            // Allocate memory for the tag list buffer (read time use only)
            tagListBuffer = new BNodeType[maxTagList + 1];

            for (int i = 0; i < TheObjectLODsCount; i++)
            {
                // Spare unused data - 12 bytes
                file.Position += 12;

                // Read data serially
                TheObjectLODs[i] = new ObjectLOD();
                TheObjectLODs[i].fileoffset = UInt32EncodingLE.Decode(file);
                TheObjectLODs[i].filesize = UInt32EncodingLE.Decode(file);
            }
            // Read the elements of the header array
            //result = read( file, TheObjectLODs, sizeof(*TheObjectLODs)*TheObjectLODsCount );

            int nParent = Int32EncodingLE.Decode(file);
            if (nParent < 1)
                throw new ApplicationException();
            HdrParent[] hdrParent = new HdrParent[nParent];
            for (int c = 0; c < nParent; c++)
            {
                hdrParent[c] = new HdrParent();
                for (int d = 0; d < 7; d++)
                    hdrParent[c].shadowSize[d] = SingleEncodingLE.Decode(file);	// Hitbox +"radius" (shadow size???)
                hdrParent[c].fileOffset = UInt32EncodingLE.Decode(file);
                hdrParent[c].fileSize = UInt32EncodingLE.Decode(file);
                hdrParent[c].nTextures = UInt16EncodingLE.Decode(file);
                hdrParent[c].nDynamics = UInt16EncodingLE.Decode(file);
                hdrParent[c].nLODs = (byte)file.ReadByte();
                hdrParent[c].nSwitches = (byte)file.ReadByte();
                hdrParent[c].nDOFs = (byte)file.ReadByte();
                hdrParent[c].nSlots = (byte)file.ReadByte();
                hdrParent[c].Unk1 = UInt16EncodingLE.Decode(file);
                hdrParent[c].Unk2 = UInt16EncodingLE.Decode(file);
            }
            for (int c = 0; c <  nParent; c++)
            {
                if (hdrParent[c].nSlots > 100)
                    throw new ApplicationException();
                if (hdrParent[c].nSlots > 0)// Load Slots
                {
                    for (int d = 0; d < hdrParent[c].nSlots; d++)
                    {
                        float slot1 = SingleEncodingLE.Decode(file);
                        float slot2 = SingleEncodingLE.Decode(file);
                        float slot3 = SingleEncodingLE.Decode(file);
                    }
                }
                if (hdrParent[c].nDynamics > 10)
                    throw new ApplicationException();
                if (hdrParent[c].nDynamics > 0)// Load Dynamic
                {
                    for (int d = 0; d < hdrParent[c].nDynamics; d++)
                    {
                        float dyn1 = SingleEncodingLE.Decode(file);
                        float dyn2 = SingleEncodingLE.Decode(file);
                        float dyn3 = SingleEncodingLE.Decode(file);
                    }
                }
                if (hdrParent[c].nLODs > 10)
                    throw new ApplicationException();

                if (hdrParent[c].nLODs > 0)// Load LOD indexes
                {
                    for (int d = 0; d < hdrParent[c].nLODs; d++)
                    {
                        int lodind  = Int32EncodingLE.Decode(file);
                        float distance  = SingleEncodingLE.Decode(file);
                    }
                }
            }

            // Open the data file we'll read from at run time as object LODs are required
            string filename = basename + ".lod";
#if TODO
            if (!ObjectLodMap.Open(filename, false, !F4Config.g_bUseMappedFiles))
            {
                log.Debug("Failed to open object LOD database" + filename);
            }
#else

#endif

            string path = F4File.F4FindFile(basename, "lod");
            using (Stream filelod = new FileStream(path, FileMode.Open))
            {
                for (int c = 0; c < TheObjectLODsCount; c++)
                {
                    if (c == 188 || c == 190 || c == 883 ||
                        c == 1259 || c == 1264 || c == 1841 ||
                        c == 1842 || c == 1843)
                        continue;
                    // ONLY IF IT HAS A SIZE...
                    if (TheObjectLODs[c].filesize <= 0) continue;
                    filelod.Position = TheObjectLODs[c].fileoffset;
                    log.DebugFormat("Processing node number = {0}, fileoffset = {1}, filesize={2}", c, TheObjectLODs[c].fileoffset, TheObjectLODs[c].filesize);
                    BNode node = BNodeEncodingLE.Decode(filelod);
                }
                filelod.Close();
            }
#if TODO
            // Init our critical section
            InitializeCriticalSection(&cs_ObjectLOD);
            RatedLoad = true;
            // Allocte acche with a little safety margin
            CacheLoad = (short*)malloc(sizeof(short) * (TheObjectLODsCount + CACHE_MARGIN));
            CacheRelease = (short*)malloc(sizeof(short) * (TheObjectLODsCount + CACHE_MARGIN));
            LoadIn = LoadOut = ReleaseIn = ReleaseOut = 0;
            LODsLoaded = 0;   
#endif
            //throw new NotImplementedException();

        }

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

        public bool OnOrder, OnRelease;
        public int refCount; // How many instances of this LOD are in use

        // Object flag values
        public enum FlagVal { NONE = 0, PERSP_CORR = 1, };

        public BRoot root; // NULL until loaded, then pointer to node tree
        public UInt32 fileoffset; // Where in the disk file is this record's tree stored
        public UInt32 filesize; // How big the disk representation of this record's tree
        public DWORD[] TexBank; // The copy of the textures Bank of the Model
        public DWORD NrTextures; // Nr of textures available in the model

#if USE_SMART_HEAP
  public:
	static MEM_POOL	pool;
#endif
#if _DEBUG
	public static int lodsLoaded; // JPO - some stats
#endif


        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }


    public static class ObjectLODEncodingLE
    {
        public static void Encode(Stream stream, ObjectLOD val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, ObjectLOD rst)
        {
            long posinit = stream.Position;
            rst.OnOrder = stream.ReadByte() != 0;
            rst.OnRelease = stream.ReadByte() != 0;
            stream.Position += 2;
            rst.refCount = Int32EncodingLE.Decode(stream);
            int ptr = Int32EncodingLE.Decode(stream);
            rst.root = null; // NULL until loaded, then pointer to node tree
            rst.fileoffset = UInt32EncodingLE.Decode(stream);
            rst.filesize = UInt32EncodingLE.Decode(stream);
            ptr = Int32EncodingLE.Decode(stream);
            rst.TexBank = null;
            rst.NrTextures = UInt32EncodingLE.Decode(stream);
            long posfinal = stream.Position;
            Debug.Assert(posfinal - posinit == 28);
        }

        public static int Size
        {
            get { throw new NotImplementedException(); }
        }
    }
}