using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.Sim
{
    // Public data types (used exclusively by AdvancedWeaponStation, however)
    public enum WeaponType { wtGuns, wtAim9, wtAim120, wtAgm88, wtAgm65, wtMk82, wtMk84, wtGBU, wtSAM, wtLAU, wtFixed, wtNone, wtGPS };
    public enum WeaponClass { wcAimWpn, wcRocketWpn, wcBombWpn, wcGunWpn, wcECM, wcTank, wcAgmWpn, wcHARMWpn, wcSamWpn, wcGbuWpn, wcCamera, wcNoWpn };
    public enum WeaponDomain { wdAir = 0x1, wdGround = 0x2, wdBoth = 0x3, wdNoDomain = 0 };

    public class WeaponData
    {
        public int flags;
        public float cd;
        public float weight;
        public float area;
        public float xEjection;
        public float yEjection;
        public float zEjection;
        public string mnemonic; //[8];
        public WeaponClass weaponClass;
        public WeaponDomain domain;
    }


    // This is for ground vehicles and ships.
    // They don't need a complex thingamabob
    public class BasicWeaponStation
    {
#if USE_SH_POOLS
   public:
      // Overload new/delete to use a SmartHeap fixed size pool
      void *operator new(size_t size) { ShiAssert( size == sizeof(BasicWeaponStation) ); return MemAllocFS(pool);	};
      void operator delete(void *mem) { if (mem) MemFreeFS(mem); };
      static void InitializeStorage()	{ pool = MemPoolInitFS( sizeof(BasicWeaponStation), 200, 0 ); };
      static void ReleaseStorage()	{ MemPoolFree( pool ); };
      static MEM_POOL	pool;
#endif

        public short weaponId;
        public short weaponCount;
        public SimWeaponClass weaponPointer;


        public BasicWeaponStation();
        // TODO public virtual ~BasicWeaponStation(void);

        public virtual void Cleanup() { }

        // Virtualized access functions
        public abstract void SetPosition(float x, float y, float z);
        public abstract void SetRotation(float a, float e);
        public abstract void GetPosition(out float x, out float y, out float z);
        public abstract void GetRotation(out float a, out float e);
        public abstract void SetSubPosition(int i, float x, float y, float z);
        public abstract void SetSubRotation(int i, float a, float e);
        public abstract void GetSubPosition(int i, out float x, out float y, out float z);
        public abstract void GetSubRotation(int i, out float a, float e);
        public abstract void SetupPoints(int p);
        public virtual int NumPoints() { return 0; }

        public virtual DrawableBSP GetRack() { return null; }
        public virtual void SetRack(DrawableBSP d) { }
        public virtual int GetRackId() { return 0; }
        public virtual void SetRackId(int id) { }
        public virtual WeaponData GetWeaponData() { return null; }
        public virtual void SetWeaponData(WeaponData wd) { }
        public virtual WeaponType GetWeaponType() { return (WeaponType)0; }
        public virtual void SetWeaponType(WeaponType wt) { }
        public virtual WeaponClass GetWeaponClass() { return (WeaponClass)0; }
        public virtual WeaponDomain Domain() { return WeaponDomain.wdNoDomain; }
        public virtual void SetWeaponClass(WeaponClass wc) { }
        public abstract GunClass GetGun();
        public virtual void SetGun(GunClass gun) { }
    }

    public class AdvancedWeaponStation : BasicWeaponStation
    {
#if USE_SH_POOLS
   public:
      // Overload new/delete to use a SmartHeap fixed size pool
      void *operator new(size_t size) { ShiAssert( size == sizeof(AdvancedWeaponStation) ); return MemAllocFS(pool);	};
      void operator delete(void *mem) { if (mem) MemFreeFS(mem); };
      static void InitializeStorage()	{ pool = MemPoolInitFS( sizeof(AdvancedWeaponStation), 200, 0 ); };
      static void ReleaseStorage()	{ MemPoolFree( pool ); };
      static MEM_POOL	pool;
#endif

        private float xPos;
        private float yPos;
        private float zPos;
        private float az;
        private float el;
        private float[] xSub;
        private float[] ySub;
        private float[] zSub;
        private float[] azSub;
        private float[] elSub;
        private int numPoints;

        private DrawableBSP theRack;
        private int rackId;
        private GunClass aGun;
        private WeaponData weaponData;
        private WeaponType weaponType;


        public AdvancedWeaponStation();
        public ~AdvancedWeaponStation();

        public virtual void Cleanup();

        // Virtualized access functions
        public virtual void SetPosition(float x, float y, float z) { xPos = x; yPos = y; zPos = z; }
        public virtual void SetRotation(float a, float e) { az = a; el = e; }
        public virtual void GetPosition(out float x, out float y, out float z) { x = xPos; y = yPos; z = zPos; }
        public virtual void GetRotation(out float a, out float e) { a = az; e = el; }
        public virtual void SetSubPosition(int i, float x, float y, float z) { xSub[i] = x; ySub[i] = y; zSub[i] = z; }
        public virtual void SetSubRotation(int i, float a, float e) { azSub[i] = a; elSub[i] = e; }
        public virtual void GetSubPosition(int i, out float x, out float y, out float z) { x = xSub[i]; y = ySub[i]; z = zSub[i]; }
        public virtual void GetSubRotation(int i, out float a, out float e) { a = azSub[i]; e = elSub[i]; }
        public virtual void SetupPoints(int num);
        public virtual int NumPoints() { return numPoints; }

        public virtual DrawableBSP GetRack() { return theRack; }
        public virtual void SetRack(DrawableBSP rack) { theRack = rack; }
        public virtual int GetRackId() { return rackId; }
        public virtual void SetRackId(int id) { rackId = id; }
        public virtual WeaponData GetWeaponData() { return weaponData; }
        public virtual void SetWeaponData(WeaponData wd) { weaponData = wd; }
        public virtual WeaponType GetWeaponType() { return weaponType; }
        public virtual void SetWeaponType(WeaponType wt) { weaponType = wt; }
        public virtual WeaponClass GetWeaponClass() { return weaponData.weaponClass; }
        public virtual WeaponDomain Domain() { return weaponData.domain; }
        public virtual void SetWeaponClass(WeaponClass wc) { weaponData.weaponClass = wc; }
        public virtual GunClass GetGun() { return aGun; }
        public virtual void SetGun(GunClass gun) { aGun = gun; }
    }
}
