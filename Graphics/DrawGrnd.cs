using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.Graphics
{
    public class DrawableGroundVehicle : DrawableBSP
    {
#if USE_SH_POOLS
  public:
      // Overload new/delete to use a SmartHeap fixed size pool
      void *operator new(size_t size) { ShiAssert( size == sizeof(DrawableGroundVehicle) ); return MemAllocFS(pool);	};
      void operator delete(void *mem) { if (mem) MemFreeFS(mem); };
      static void InitializeStorage()	{ pool = MemPoolInitFS( sizeof(DrawableGroundVehicle), 50, 0 ); };
      static void ReleaseStorage()	{ MemPoolFree( pool ); };
      static MEM_POOL	pool;
#endif


        public DrawableGroundVehicle(int type, Tpoint* pos, float heading, float scale = 1.0f);
        //TODO public virtual ~DrawableGroundVehicle()	{};

        public virtual void SetParentList(ObjectDisplayList* list);
        public void Update(Tpoint* pos, float heading);
        public void SetUpon(DrawableBridge* bridge) { drivingOn = bridge; previousLOD = -1; }

        public virtual void Draw(RenderOTW renderer, int LOD);

        public float GetHeading() { return yaw; }


        protected int previousLOD;
        protected float yaw;
        protected float cosYaw;
        protected float sinYaw;
        protected DrawableBridge* drivingOn;
    }
}
