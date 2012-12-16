using System;
using Ppoint = FalconNet.Graphics.Tpoint;

namespace FalconNet.Graphics
{
    /***************************************************************************\
        ObjectParent.h
        Scott Randolph
        February 9, 1998

        Provides structures and definitions for 3D objects.
    \***************************************************************************/

    public struct LODrecord
    {
        public ObjectLOD objLOD;
        public float maxRange;
    }


    public class ObjectParent
    {

        // Update this each time the object file formats change
        public const UInt32 FORMAT_VERSION = 0x03087000;


        public static ObjectParent TheObjectList;
        public static int TheObjectListLength;

        public ObjectParent()
        { throw new NotImplementedException(); }
        //public ~ObjectParent();

        public static void SetupTable(string filename)
        { throw new NotImplementedException(); }
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


        protected static void ReadParentList(int file)
        { throw new NotImplementedException(); }
        protected void VerifyVersion(int file)
        { throw new NotImplementedException(); }


        public ObjectLOD ChooseLOD(float range, ref int lod_used, ref float max_range)
        { throw new NotImplementedException(); }

        public float radius;
        public float minX, maxX;
        public float minY, maxY;
        public float minZ, maxZ;

        public LODrecord pLODs;
        public Ppoint pSlotAndDynamicPositions;

        public short nTextureSets;
        public short nDynamicCoords;
        public sbyte nLODs;
        public sbyte nSwitches;
        public sbyte nDOFs;
        public sbyte nSlots;


        protected int refCount;

    }
}
