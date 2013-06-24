using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.Graphics
{
    public class DrawableBSP : DrawableObject
    {

        public DrawableBSP(int type, Tpoint pos, Trotation rot, float scale = 1.0f);
        //TODO public virtual ~DrawableBSP();

        // This constructor is used only by derived classes who do their own setup
        protected DrawableBSP(float s, int ID)
            : base(s)
        { instance = ID; inhibitDraw = FALSE; labelLen = 0; id = ID; radius = instance.Radius(); }


        public void Update(Tpoint pos, Trotation rot);

        public bool IsLegalEmptySlot(int slotNumber) { return (slotNumber < instance.ParentObject.nSlots) && (instance.SlotChildren) && (instance.SlotChildren[slotNumber] == NULL); }

        public int GetNumSlots() { return instance.ParentObject.nSlots; }
        public int GetNumDOFs() { return instance.ParentObject.nDOFs; }
        public int GetNumSwitches() { return instance.ParentObject.nSwitches; }
        public int GetNumDynamicVertices() { return instance.ParentObject.nDynamicCoords; }

        public void AttachChild(DrawableBSP* child, int slotNumber);
        public void DetachChild(DrawableBSP* child, int slotNumber);
        public void GetChildOffset(int slotNumber, Tpoint* offset);

        public void SetDOFangle(int DOF, float radians);
        public void SetDOFoffset(int DOF, float offset);
        public void SetDynamicVertex(int vertID, float dx, float dy, float dz);
        public void SetSwitchMask(int switchNumber, UInt32 mask);
        public void SetTextureSet(UInt32 set) { instance.SetTextureSet(set); }
        public int GetNTextureSet() { return instance.GetNTextureSet(); }

        public float GetDOFangle(int DOF);
        public float GetDOFoffset(int DOF);
        public void GetDynamicVertex(int vertID, float* dx, float* dy, float* dz);
        public UInt32 GetSwitchMask(int switchNumber);
        public UInt32 GetTextureSet() { return instance.TextureSet; }

        public string Label() { return label; }
        public DWORD LabelColor() { return labelColor; }
        public virtual void SetLabel(char* labelString, DWORD color);
        public virtual void SetInhibitFlag(bool state) { inhibitDraw = state; }

        public virtual bool GetRayHit(Tpoint from, Tpoint vector, Tpoint collide, float boxScale = 1.0f);

        public virtual void Draw(RenderOTW renderer, int LOD);
        public virtual void Draw(Render3D renderer);

        public int GetID() { return id; }

        // These two functions are used to handle preloading BSP objects for quick drawing later
        public static void LockAndLoad(int id) { TheObjectList[id].ReferenceWithFetch(); }
        public static void Unlock(int id) { TheObjectList[id].Release(); }

        // get object's bounding box
        public void GetBoundingBox(Tpoint* minB, Tpoint* maxB);

        // This one is for internal use only.  Don't use it or you'll break things...
        public void ForceZ(float z) { position.z = z; }


        public Trotation orientation;
        public static bool drawLabels;		// Shared by ALL drawable BSPs

        public ObjectInstance instance;


        protected int id;				// TODO: With the new BSP lib, this id could go...
        //ObjectInstance		instance;

        protected bool inhibitDraw;	// When TRUE, the Draw function just returns

        protected string label;
        protected int labelLen;
        protected DWORD labelColor;

        // Handle time of day notifications
        protected static void TimeUpdateCallback(void* unused);


        public static void SetupTexturesOnDevice(DXContext* rc);
        public static void ReleaseTexturesOnDevice(DXContext* rc);



        protected void DrawBoundingBox(Render3D renderer);

#if  USE_SH_POOLS
  public:
	// Overload new/delete to use a SmartHeap fixed size pool
	void *operator new(size_t size) { ShiAssert( size == sizeof(DrawableBSP) ); return MemAllocFS(pool);	};
	void operator delete(void *mem) { if (mem) MemFreeFS(mem); };
	static void InitializeStorage()	{ pool = MemPoolInitFS( sizeof(DrawableBSP), 50, 0 ); };
	static void ReleaseStorage()	{ MemPoolFree( pool ); };
	static MEM_POOL	pool;
#endif
    }
}
