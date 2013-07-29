using System;
using System.Diagnostics;
using Ppoint = FalconNet.Common.Graphics.Tpoint;

namespace FalconNet.Graphics
{
    public struct DOFvalue
    {
        public float rotation;
        public float translation;
    }
    public class ObjectInstance
    {

        public ObjectInstance(int id)
        { throw new NotImplementedException(); }
        //TODO public  ~ObjectInstance();

        public float Radius() { return ParentObject.radius; }
        public float BoxLeft() { return ParentObject.minY; }
        public float BoxRight() { return ParentObject.maxY; }
        public float BoxTop() { return ParentObject.minZ; }
        public float BoxBottom() { return ParentObject.maxZ; }
        public float BoxFront() { return ParentObject.maxX; }
        public float BoxBack() { return ParentObject.minX; }

        public void SetSwitch(int id, UInt32 value) { Debug.Assert(id < ParentObject.nSwitches); SwitchValues[id] = value; }
        public void SetDOFrotation(int id, float r) { Debug.Assert(id < ParentObject.nDOFs); DOFValues[id].rotation = r; }
        public void SetDOFxlation(int id, float x) { Debug.Assert(id < ParentObject.nDOFs); DOFValues[id].translation = x; }
        public void SetSlotChild(int id, ObjectInstance o) { Debug.Assert(id < ParentObject.nSlots); SlotChildren[id] = o; }
        public void SetTextureSet(int id)
        {
            // edg: sanity check texture setting
            if (id < ParentObject.nTextureSets)
                TextureSet = id;
        }
        public int GetNTextureSet() { return ParentObject.nTextureSets; }
        public void SetDynamicVertex(int id, float dx, float dy, float dz)
        { throw new NotImplementedException(); }
        public void GetDynamicVertex(int id, ref float dx, ref float dy, ref float dz)
        { throw new NotImplementedException(); }


        public UInt32[] SwitchValues;
        public DOFvalue[] DOFValues;
        public ObjectInstance[] SlotChildren;
        public Ppoint[] DynamicCoords;
        public int TextureSet;
        public int id;

        public ObjectParent ParentObject;
    }
}
