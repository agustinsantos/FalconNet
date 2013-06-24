using FalconNet.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.Sim
{
    public struct GunTracerType
    {
        public float x, y, z;
        public float xdot, ydot, zdot;
        public int flying;
    }

    public enum TracerCollisionMode
    {
        COLLIDE_CHECK_ALL,
        COLLIDE_CHECK_ENEMY,
        COLLIDE_CHECK_NOFEATURE
    }




    public class GunClass : SimWeaponClass
    {
        // returned by tracer checks
        public const int TRACER_HIT_NOTHING = 0x00000000;
        public const int TRACER_HIT_FEATURE = 0x00000001;
        public const int TRACER_HIT_UNIT = 0x00000002;
        public const int TRACER_HIT_GROUND = 0x00000004;

#if USE_SH_POOLS
   public:
      // Overload new/delete to use a SmartHeap fixed size pool
      void *operator new(size_t size) { ShiAssert( size == sizeof(GunClass) ); return MemAllocFS(pool);	};
      void operator delete(void *mem) { if (mem) MemFreeFS(mem); };
      static void InitializeStorage()	{ pool = MemPoolInitFS( sizeof(GunClass), 200, 0 ); };
      static void ReleaseStorage()	{ MemPoolFree( pool ); };
      static MEM_POOL	pool;
#endif


        private float xPos, yPos, zPos;
        private float pitch, yaw;
        private float dragFactor;
        private int fireCount, bursts;
        private int initialRounds;
        private float fractionalRoundsRemaining;	// Could make numRoundsRemaining a float, but this changed less code...
        private DrawableTracer** tracers;
        private Drawable2D** bullets;
        private DrawableTrail* smokeTrail;
        private TracerCollisionMode tracerMode;

        // these variables are used to do the series of muzzle tracers
        private DrawableTracer** firstTracer;
        private Tpoint* muzzleLoc;
        private Tpoint* muzzleEnd;
        private float* muzzleAlpha;
        private float* muzzleWidth;

        private int* trailState;
        private int muzzleStart;
        private float qTimer;
        private void UpdateTracers(int firing);



        public void InitTracers();
        public void CleanupTracers();
        public GunClass(int type);
        //TODO public virtual ~GunClass( );
        public enum GunStatus { Ready, Sim, Safe };
        public float initBulletVelocity;
        public GunStatus status;
        public GunTracerType* bullet;
        public float roundsPerSecond;
        public int numRoundsRemaining;
        public int numTracers;
        public int numFirstTracers;
        public int unlimitedAmmo;
        public int numFlying;
        public ulong FiremsgsendTime;

        public void Init(float muzzleVel, int numRounds);
        public void SetPosition(float xOffset, float yOffset, float zOffset, float pitch, float yaw);
        public int Exec(int* fire, TransformMatrix dmx, ObjectGeometry* geomData, SimObjectType* objList, BOOL isOwnship);

        public void NewBurst() { bursts++; }
        public int GetCurrentBurst() { return bursts; }

        public void SetTracerCollisionMode(TracerCollisionMode mode)
        {
            tracerMode = mode;
        }

        // new stuff for to support shells

        // typpedefs and enums
        public enum GunType
        {
            GUN_SHELL,
            GUN_TRACER,
            GUN_TRACER_BALL
        }

        public virtual bool IsGun() { return true; }

        // member functions
        public bool IsShell();
        public bool IsTracer();
        public bool ReadyToFire();
        public float GetDamageAssessment(SimBaseClass* target, float range);
        public void FireShell(SimObjectType* target);
        public void UpdateShell();
        public WeaponDomain GetSMSDomain();

        //MI to add a timer to AAA
        public bool CheckAltitude;	//Do we need to update the target Altitude?
        public VU_TIME AdjustForAlt;	//How long it takes the AAA to adjust for our new Alt
        public float TargetAlt;		//How high our target currently is

        // member variables
        public WeaponDomain gunDomain;				// air, land, both
        public SimObjectType* shellTargetPtr;		// set when shell flying
        public GunType typeOfGun;				// tracer or shell
        public VU_TIME shellDetonateTime;		// when it goes cablooey
        public float minShellRange;			// minimum for shell
        public float maxShellRange;			// max for shell
        public WeaponClassDataType* wcPtr;					// pointer to weapon class data
    }
}
