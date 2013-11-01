using FalconNet.Common.Encoding;
using FalconNet.Common.Graphics;
using FalconNet.F4Common;
using log4net;
using System;
using System.IO;
using Ppoint = FalconNet.Common.Graphics.Tpoint;

namespace FalconNet.Graphics
{
    /***************************************************************************
        Provides structures and definitions for 3D objects.
    \***************************************************************************/
    // RED - this is the single record stucture in the parents file
    public class ParentFileRecord
    {

        public float radius;
        public float minX, maxX;
        public float minY, maxY;
        public float minZ, maxZ;

        public float RadarSign;
        public float IRSign;

        public short nTextureSets;
        public short nDynamicCoords;
        public byte nLODs;
        public byte nSwitch;
        public byte nDOF;
        public byte nSlots;
        public short nSwitches;
        public short nDOFs;

        //DWORD Unused_2;
    }

    public struct LODrecord
    {
        public ObjectLOD objLOD;
        public float maxRange;
    }


    public class ObjectParent
    {

        // Update this each heading the object file formats change
        public const UInt32 FORMAT_VERSION = 0x03087000;
        public const UInt32 DXver = 0xFEEF;

        public static ObjectParent[] TheObjectList;
        public static int TheObjectListLength;

        public ObjectParent()
        {
            refCount = 0;
            pLODs = null;
            pSlotAndDynamicPositions = null;
        }

        //public ~ObjectParent();

        public static void SetupTable(string filename)
        {
            string filepath = F4File.F4FindFile(filename, "hdr");
            using (Stream file = new FileStream(filepath, FileMode.Open))
            {
                // Read the format version
                VerifyVersion(file);

                // Read the Color Table from the master file
                ColorBankClass.ReadPool(file);

                // Read the Palette Table from the master file
                PaletteBankClass.ReadPool(file);

                // Read the Texture Table from the master file
                TextureBankClass.ReadPool(file, filename);

                // Read the object LOD headers from the master file
                ObjectLOD.SetupTable(file, filename);

                // Read the parent object records from the master file
                ReadParentList(file);

                // Close the master file
                file.Close();
            }
        }

        public static void CleanupTable()
        { throw new NotImplementedException(); }
        public static void FlushReferences()
        { throw new NotImplementedException(); }

        public void ReferenceWithFetch()
        { throw new NotImplementedException(); }
        public void Reference()
        { throw new NotImplementedException(); }
        public void Release()
        { throw new NotImplementedException(); }


        protected static void ReadParentList(Stream file)
        {
            // Read the length of the parent object array
            TheObjectListLength = Int32EncodingLE.Decode(file);


            // Allocate memory for the parent object array
#if USE_SH_POOLS
	TheObjectList = (ObjectParent *)MemAllocPtr(gBSPLibMemPool, sizeof(ObjectParent)*TheObjectListLength, 0 );
#else
            TheObjectList = new ObjectParent[TheObjectListLength];
#endif
            // Now read the elements of the parent array
            for (int i = 0; i < TheObjectListLength; i++)
            {
                TheObjectList[i] = new ObjectParent();
                ObjectParentEncodingLE.Decode(file, TheObjectList[i]);
            }


            // Finally, read the reference arrays for each parent in order
            for (int i = 0; i < TheObjectListLength; i++)
            {
                ObjectParent objParent = TheObjectList[i];
                // Skip any unused objects
                if (objParent.nLODs == 0)
                {
                    continue;
                }

                // Allocate and read this parent's slot and dynamic position array
                if (objParent.nSlots != 0)
                {

#if USE_SH_POOLS
			objParent.pSlotAndDynamicPositions = (Tpoint *)MemAllocPtr(gBSPLibMemPool, sizeof(Ppoint)*(objParent.nSlots+objParent.nDynamicCoords), 0 );
#else
                    objParent.pSlotAndDynamicPositions = new Ppoint[objParent.nSlots + objParent.nDynamicCoords];
#endif
                    for (int j = 0; j < objParent.nSlots + objParent.nDynamicCoords; j++)
                        TpointEncodingLE.Decode(file, ref objParent.pSlotAndDynamicPositions[j]);
                }
                else
                    objParent.pSlotAndDynamicPositions = null; // JPO - jsut in case its wrong in the file

                // Allocate memory for this parent's reference list

#if USE_SH_POOLS
		objParent.pLODs = (LODrecord *)MemAllocPtr(gBSPLibMemPool, sizeof(LODrecord)*(objParent.nLODs), 0 );
#else
                objParent.pLODs = new LODrecord[objParent.nLODs];
#endif
                // Read the reference list
                for (int j = 0; j < objParent.nLODs; j++)
                    LODrecordEncodingLE.Decode(file, ref objParent.pLODs[j]);

                // Fixup the LOD references
                for (i = 0; i < objParent.nLODs; i++)
                {

                    // Replace the offset of the LOD with a pointer into TheObjectLOD array.
                    // NOTE:  We're shifting the offset right one bit to clear our special
                    // marker.
#if TODO
			objParent.pLODs[i].objLOD = TheObjectLODs[ ((int)(objParent.pLODs[i].objLOD)>>1) ];
 
#endif
                }
            }
        }

        public ObjectLOD ChooseLOD(float range, ref int lod_used, ref float max_range)
        { throw new NotImplementedException(); }

        protected static void VerifyVersion(Stream file)
        {
            // Read the magic number at the head of the file
            UInt32 fileVersion = UInt32EncodingLE.Decode(file);

            // If the version doesn't match our expectations, report an error
            if (fileVersion != FORMAT_VERSION)
            {
                //Beep( 2000, 500 );
                //Beep( 2000, 500 );
                log.ErrorFormat("Got object format version 0x{0:X}, want 0x{1:X}", fileVersion, FORMAT_VERSION);
            }

            // New version of KO,dxh which uses UINT's for nSwitches and nDOFs to handle the increased number of Switch and DOF ID's.
            if (TextureBankClass.nVer != (int)DXver)
                TextureBankClass.nVer = 0; // old KO.dxh version
        }

        public float radius;
        public float minX, maxX;
        public float minY, maxY;
        public float minZ, maxZ;
        public float RadarSign, IRSign;

        public LODrecord[] pLODs;
        public Ppoint[] pSlotAndDynamicPositions;

        public short nTextureSets;
        public short nDynamicCoords;
        public byte nLODs;
        public short nSwitches;
        public short nDOFs;
        public byte nSlots;

        public bool Locked; // RED - The object is locked, can not be released
        public int refCount;

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }

    #region Encoding
    public static class ObjectParentEncodingLE
    {
        public static void Encode(Stream stream, ObjectParent val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, ObjectParent rst)
        {
            // init the parameters
            rst.Locked = false;
            rst.pLODs = null;
            rst.refCount = 0;

            // read it, and check result
            //TODO int result = ReadParentRecord(TheObjectList[Object], stream);
            ParentFileRecord Record = new ParentFileRecord();
            ParentFileRecordEncodingLE.Decode(stream, Record);
            // Assign data
            rst.radius = Record.radius;
            rst.minX = Record.minX;
            rst.maxX = Record.maxX;
            rst.minY = Record.minY;
            rst.maxY = Record.maxY;
            rst.minZ = Record.minZ;
            rst.maxZ = Record.maxZ;

            rst.RadarSign = Record.RadarSign;
            rst.IRSign = Record.IRSign;

            rst.nTextureSets = Record.nTextureSets;
            rst.nDynamicCoords = Record.nDynamicCoords;
            rst.nLODs = Record.nLODs;

            // old nSwitches and nDOFs?
            if (TextureBankClass.nVer != 0)
            {
                rst.nSwitches = Record.nSwitches; // new
                rst.nDOFs = Record.nDOFs;
            }
            else
            {
                rst.nSwitches = (short)Record.nSwitch; // old
                rst.nDOFs = (short)Record.nDOF;
            }

            rst.nSlots = Record.nSlots;

        }

        public static int Size
        {
            get { throw new NotImplementedException(); }
        }
    }

    public static class ParentFileRecordEncodingLE
    {
        public static void Encode(Stream stream, ParentFileRecord val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, ParentFileRecord rst)
        {
            rst.radius = SingleEncodingLE.Decode(stream);
            rst.minX = SingleEncodingLE.Decode(stream);
            rst.maxX = SingleEncodingLE.Decode(stream);
            rst.minY = SingleEncodingLE.Decode(stream);
            rst.maxY = SingleEncodingLE.Decode(stream);
            rst.minZ = SingleEncodingLE.Decode(stream);
            rst.maxZ = SingleEncodingLE.Decode(stream);

            rst.RadarSign = SingleEncodingLE.Decode(stream);
            rst.IRSign = SingleEncodingLE.Decode(stream);

            rst.nTextureSets = Int16EncodingLE.Decode(stream);
            rst.nDynamicCoords = Int16EncodingLE.Decode(stream);
            rst.nLODs = (byte)stream.ReadByte();
            rst.nSwitch = (byte)stream.ReadByte();
            rst.nDOF = (byte)stream.ReadByte();
            rst.nSlots = (byte)stream.ReadByte();
            rst.nSwitches = Int16EncodingLE.Decode(stream);
            rst.nDOFs = Int16EncodingLE.Decode(stream);
        }

        public static int Size
        {
            get { throw new NotImplementedException(); }
        }
    }

    public static class LODrecordEncodingLE
    {
        public static void Encode(Stream stream, LODrecord val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, ref LODrecord rst)
        {
            rst.objLOD  = new ObjectLOD();
            ObjectLODEncodingLE.Decode(stream, rst.objLOD);
            rst.maxRange = SingleEncodingLE.Decode(stream);
        }

        public static int Size
        {
            get { throw new NotImplementedException(); }
        }
    }
    
    #endregion
}
