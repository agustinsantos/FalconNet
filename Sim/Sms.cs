using FalconNet.Common;
using FalconNet.FalcLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FalconNet.Sim
{
    // ==================================================================
    // SMSBaseClass
    // This holds the minimum needed to keep track of weapons
    // ==================================================================

    public class SMSBaseClass
    {

#if USE_SH_POOLS
      // Overload new/delete to use a SmartHeap fixed size pool
      void *operator new(size_t size) { Debug.Assert( size == sizeof(SMSBaseClass) ); return MemAllocFS(pool);	};
      void operator delete(void *mem) { if (mem) MemFreeFS(mem); };
      static void InitializeStorage()	{ pool = MemPoolInitFS( sizeof(SMSBaseClass), 200, 0 ); };
      static void ReleaseStorage()	{ MemPoolFree( pool ); };
      static MEM_POOL	pool;
#endif
        public enum MasterArmState { Safe, Sim, Arm };
        public enum CommandFlags
        {
            Loftable = 0x0001,
            HasDisplay = 0x0002,
            HasBurstHeight = 0x0004,
            UnlimitedAmmoFlag = 0x0008,
            EmergencyJettisonFlag = 0x0010,
            Firing = 0x0020,
            LGBOnBoard = 0x0040,
            HTSOnBoard = 0x0080,
            SPJamOnBoard = 0x0100,
            GunOnBoard = 0x0200,
            Trainable = 0x0400,
            TankJettisonFlag = 0x0800 // 2002-02-20 ADDED BY S.G. Flag it if we have jettisoned our tanks (mainly for digis)
        }

        public BasicWeaponStation[] hardPoint;

        public SMSBaseClass(SimVehicleClass newOwnship, short[] weapId, byte[] weapCnt, bool advanced = false)
        {
            int i, j;
            WeaponClassDataType wd;
            Falcon4EntityClassType* classPtr;
            //
            VehicleClassDataType* vc;
            int rackFlag = 0, createCount;



            flags = 0;
            numCurrentWpn = 0;
            //MI change for default state
            if (!g_bRealisticAvionics)
                masterArm = MasterArmState.Arm;
            else
                masterArm = MasterArmState.Safe;
            //
            ownship = newOwnship;

            if (ownship.IsAirplane() && ((AircraftClass*)ownship).IsF16())
            {
                numHardpoints = 10;
            }
            else
            {
                for (numHardpoints = HARDPOINT_MAX - 1; numHardpoints >= 0; numHardpoints--)
                {
                    if (weapId[numHardpoints] != 0)
                        break;
                }
                if (numHardpoints >= 0)
                    numHardpoints++;
                else
                    numHardpoints = 0;
            }

            curHardpoint = -1;
            if (numHardpoints != 0)
                hardPoint = new BasicWeaponStation[numHardpoints];
            else
                hardPoint = null;
            //
            vc = GetVehicleClassData(newOwnship.Type() - VU_LAST_ENTITY_TYPE);
            rackFlag = vc.RackFlags;
            //
            for (i = 0; i < numHardpoints; i++)
            {
                if (advanced)
                    hardPoint[i] = new AdvancedWeaponStation();
                else
                    hardPoint[i] = new BasicWeaponStation();
                hardPoint[i].weaponPointer = null;
                hardPoint[i].weaponId = (short)(weapId[i]);
                hardPoint[i].weaponCount = (short)(weapCnt[i]);

                //LRKLUDGE
                //      if (hardPoint[i].weaponId == 184)
                //         hardPoint[i].weaponId = 185;
                if (hardPoint[i].weaponId != 0 && hardPoint[i].weaponCount != 0)
                {
                    wd = &WeaponDataTable[hardPoint[i].weaponId];
                    Debug.Assert(wd);
                    if (wd.Flags & WEAP_ONETENTH)
                        hardPoint[i].weaponCount *= 10;
                    //
                    if (rackFlag & (1 << i))
                        createCount = weapCnt[i];
                    else
                        createCount = 1;
                    //
                    classPtr = &Falcon4ClassTable[wd.Index];
                    if (classPtr.vuClassData.classInfo_[VU_TYPE] == TYPE_GUN &&
                        classPtr.vuClassData.classInfo_[VU_CLASS] == CLASS_WEAPON)
                    {
                        // This is a gun, initialize some extra data
                        hardPoint[i].weaponPointer = InitAGun(newOwnship, hardPoint[i].weaponId, weapCnt[i]);
                        SetFlag(GunOnBoard);
                    }
                    else if (classPtr.vuClassData.classInfo_[VU_TYPE] == TYPE_MISSILE || classPtr.vuClassData.classInfo_[VU_TYPE] == TYPE_ROCKET)
                    {
                        // This is a Missile, create one for each shot (linked list)
                        hardPoint[i].weaponPointer = InitWeaponList(newOwnship, hardPoint[i].weaponId,
                       hardPoint[i].GetWeaponClass(), createCount, InitAMissile);
                    }
                    else if (classPtr.vuClassData.classInfo_[VU_TYPE] == TYPE_BOMB ||
                            (classPtr.vuClassData.classInfo_[VU_TYPE] == TYPE_ELECTRONICS && classPtr.vuClassData.classInfo_[VU_CLASS] == CLASS_VEHICLE) ||
                            (classPtr.vuClassData.classInfo_[VU_TYPE] == TYPE_FUEL_TANK && classPtr.vuClassData.classInfo_[VU_STYPE] == STYPE_FUEL_TANK) ||
                            (classPtr.vuClassData.classInfo_[VU_TYPE] == TYPE_RECON && classPtr.vuClassData.classInfo_[VU_STYPE] == STYPE_CAMERA))
                    {
                        // This is a Bomb or other dropable object, create one for each shot (linked list)
                        hardPoint[i].weaponPointer = InitWeaponList(newOwnship, hardPoint[i].weaponId,
                       hardPoint[i].GetWeaponClass(), createCount, InitABomb);
                    }
                    else if (classPtr.vuClassData.classInfo_[VU_TYPE] == TYPE_LAUNCHER && classPtr.vuClassData.classInfo_[VU_STYPE] == STYPE_ROCKET)
                    {
                        // This is a Launcher type Rocket.. Which is special cased

                        hardPoint[i].SetRackId(hardPoint[i].weaponId);

                        // Change hacks to datafile: terrdata\objects\falcon4.rkt
                        int entryfound = 0;
                        for (j = 0; j < NumRocketTypes; j++)
                        {
                            if (hardPoint[i].weaponId == RocketDataTable[j].weaponId)
                            {
                                if (RocketDataTable[j].nweaponId) // 0 = don't change weapon ID
                                    hardPoint[i].weaponId = RocketDataTable[j].nweaponId;
                                hardPoint[i].weaponCount = RocketDataTable[j].weaponCount;
                                hardPoint[i].weaponPointer = InitWeaponList(newOwnship, hardPoint[i].weaponId,
                                    hardPoint[i].GetWeaponClass(), hardPoint[i].weaponCount, InitAMissile);
                                entryfound = 1;
                                break;
                            }
                        }
                        if (!entryfound)	// use generic 2.75mm rocket
                        {
                            hardPoint[i].weaponId = gRocketId;
                            hardPoint[i].weaponCount = 19;
                            hardPoint[i].weaponPointer = InitWeaponList(newOwnship, hardPoint[i].weaponId,
                            hardPoint[i].GetWeaponClass(), hardPoint[i].weaponCount, InitAMissile);
                        }

                        /*
                                        // First, UB-57-38s
                                        if (hardPoint[i].weaponId == 93)
                                        {
                                            hardPoint[i].weaponId = 181;
                                            hardPoint[i].weaponCount = 38;
                                            hardPoint[i].weaponPointer = InitWeaponList (newOwnship, hardPoint[i].weaponId,
                                            hardPoint[i].GetWeaponClass(), hardPoint[i].weaponCount, InitAMissile);
                                        }
                                        else if (hardPoint[i].weaponId == 94)
                                        {
                                            hardPoint[i].weaponId = 181;
                                            hardPoint[i].weaponCount = 19;
                                            hardPoint[i].weaponPointer = InitWeaponList (newOwnship, hardPoint[i].weaponId,
                                            hardPoint[i].GetWeaponClass(), hardPoint[i].weaponCount, InitAMissile);
                                        }
                                        else if (hardPoint[i].weaponId == 128) // S-24
                                        {
                                            hardPoint[i].weaponCount = 10;
                                            hardPoint[i].weaponPointer = InitWeaponList (newOwnship, hardPoint[i].weaponId,
                                            hardPoint[i].GetWeaponClass(), hardPoint[i].weaponCount, InitAMissile);
                                        }
                                        else
                                        {
                                            hardPoint[i].weaponId = gRocketId;
                                            hardPoint[i].weaponCount = 19;
                                            hardPoint[i].weaponPointer = InitWeaponList (newOwnship, hardPoint[i].weaponId,
                                            hardPoint[i].GetWeaponClass(), hardPoint[i].weaponCount, InitAMissile);
                                        }*/
                    }
                    else
                    {
                        Debug.WriteLine("Vehicle has unknown weapon type %d.\n", wd.Index);
                    }
                }
            }
        }
        // TODO public virtual ~SMSBaseClass();
        public virtual void AddWeaponGraphics()
        {
            int i, visFlag;
            DrawableBSP* drawPtr = (DrawableBSP*)ownship.drawPointer;
            VehicleClassDataType* vc;

            if (!hardPoint || !drawPtr)
                return;

            vc = GetVehicleClassData(ownship.Type() - VU_LAST_ENTITY_TYPE);
            visFlag = vc.VisibleFlags;
            for (i = 0; i < numHardpoints; i++)
            {
                if (hardPoint[i] && hardPoint[i].weaponPointer)
                {
                    if (visFlag & (1 << i))
                    {
                        // This is a visible weapon, however, only the first one should get a drawPointer
                        OTWDriver.CreateVisualObject(hardPoint[i].weaponPointer);
                        OTWDriver.AttachObject(drawPtr, (DrawableBSP*)hardPoint[i].weaponPointer.drawPointer, i);
                    }
                    else if (hardPoint[i].weaponPointer.IsGun())
                    {
                        // Just init gun's tracers. We don't draw the gun
                        ((GunClass*)hardPoint[i].weaponPointer).InitTracers();
                    }
                    // Otherwise, this is a non-visible weapon - it'll init it's draw pointer on launch
                }
            }
        }

        public virtual void FreeWeaponGraphics()
        {
            int i;
            SimWeaponClass* weaponPtr;
            DrawableBSP* drawPtr = (DrawableBSP*)ownship.drawPointer;

            if (!hardPoint || !drawPtr)
                return;

            for (i = 0; i < numHardpoints; i++)
            {
                weaponPtr = null;
                if (hardPoint[i])
                    weaponPtr = hardPoint[i].weaponPointer;

                while (weaponPtr)
                {

                    if (weaponPtr.IsMissile())
                    {
                        ((MissileClass*)weaponPtr).SetTarget(null);
                        ((MissileClass*)weaponPtr).ClearReferences();
                    }
                    else if (weaponPtr.IsBomb())
                        ((BombClass*)weaponPtr).SetTarget(null);

                    if (weaponPtr.drawPointer)
                    {
                        // Detach anything with a draw pointer from the vehicle's drawpointer
                        OTWDriver.DetachObject(drawPtr, (DrawableBSP*)(weaponPtr.drawPointer), i);
                        OTWDriver.RemoveObject(weaponPtr.drawPointer, true);
                        weaponPtr.drawPointer = null;
                    }
                    else if (GetGun(i))
                        GetGun(i).CleanupTracers();
                    weaponPtr = weaponPtr.GetNextOnRail();
                }
            }
        }
        public virtual SimWeaponClass* GetCurrentWeapon()
        {
            if (hardPoint && curHardpoint > -1)
                return hardPoint[curHardpoint].weaponPointer;
            else
                return null;
        }

        public GunClass* GetGun(int station)
        {
            if (hardPoint && station >= 0 && hardPoint[station].weaponPointer && hardPoint[station].weaponPointer.IsGun())
                return (GunClass*)hardPoint[station].weaponPointer;
            return null;
        }
        public MissileClass* GetMissile(int hardpoint)
        {
            if (hardPoint && hardPoint[hardpoint].weaponPointer && hardPoint[hardpoint].weaponPointer.IsMissile())
                return (MissileClass*)hardPoint[hardpoint].weaponPointer;
            return null;
        }
        public BombClass* GetBomb(int hardpoint)
        {
            if (hardPoint && hardPoint[hardpoint].weaponPointer && hardPoint[hardpoint].weaponPointer.IsBomb())
                return (BombClass*)hardPoint[hardpoint].weaponPointer;
            return null;
        }

        public int GetCurrentWeaponHardpoint() { return curHardpoint; }
        public short GetCurrentWeaponType() { return (short)hardPoint[curHardpoint].weaponId; }
        public short GetCurrentWeaponIndex()
        {
            if (curHardpoint > -1)
                return WeaponDataTable[hardPoint[curHardpoint].weaponId].Index;
            return 0;
        }
        public float GetCurrentWeaponRangeFeet()
        {
            if (curHardpoint > -1)
                return WeaponDataTable[hardPoint[curHardpoint].weaponId].Range * KM_TO_FT;
            return 0.0F;
        }

        public void LaunchWeapon()
        {
            SimWeaponClass* theWeapon;
            Tpoint simLoc;
            //	float			dx,dy,dz,xydist,yaw,pitch;
            SimObjectType* tmpTargetPtr = ownship.targetPtr;
            int visFlag;
            VehicleClassDataType* vc;
            int slotId;

            if (!ownship.drawPointer || !tmpTargetPtr)
                return;

            // edg: there was a problem.  Some ground vehicles do NOT have guns
            // at all.  All this code assumes a gun in Slot 0 and therefore subtracts
            // 1 from curhardpoint to get the right slot.  Don't do this if there
            // are no guns on board.
            if (IsSet(GunOnBoard))
                slotId = max(0, curHardpoint - 1);
            else
                slotId = curHardpoint;

            vc = GetVehicleClassData(ownship.Type() - VU_LAST_ENTITY_TYPE);
            visFlag = vc.VisibleFlags;
            if (hardPoint && curHardpoint > -1 && hardPoint[curHardpoint].weaponPointer && hardPoint[curHardpoint].weaponCount > 0)
            {
                theWeapon = hardPoint[curHardpoint].weaponPointer;
                hardPoint[curHardpoint].weaponCount--;
                DetachWeapon(curHardpoint, theWeapon);

                // Detach visual from parent
                if (theWeapon.drawPointer)
                {
                    // KCK: Call detach directly, so we can use it's updated position/orientation
                    ((DrawableBSP*)ownship.drawPointer).DetachChild((DrawableBSP*)theWeapon.drawPointer, slotId);
                }

                if (theWeapon.IsMissile() && ownship.drawPointer)
                {
                    if (visFlag & (1 << curHardpoint))
                        ((DrawableBSP*)ownship.drawPointer).GetChildOffset(slotId, &simLoc);
                    else
                        simLoc.x = simLoc.y = simLoc.z = 0.0F;

                    // The weapon's position/orientation are being set relative to the parent's postion and orientation
                    ((MissileClass*)theWeapon).SetLaunchPosition(simLoc.x, simLoc.y, simLoc.z);
                    ((MissileClass*)theWeapon).SetLaunchRotation(ownship.GetDOFValue(0), ownship.GetDOFValue(1));
                }
                else
                {
                    // Just use parent object's position/orientation
                    // KCK: These take world coodinates, but I can't really think of a non-missile ground weapon
                    // which launches... 
                    theWeapon.SetPosition(ownship.XPos(), ownship.YPos(), ownship.ZPos());
                    theWeapon.SetYPR(ownship.Yaw(), ownship.Pitch(), 0.0F);
                }
                theWeapon.SetDelta(ownship.XDelta(), ownship.YDelta(), ownship.ZDelta());

                //
                // If we're direct mounted weapons and have more weapons on this hardpoint, but no weapon pointers, 
                // replace the weapon with an identical copy
                if (!hardPoint[curHardpoint].GetRack() && hardPoint[curHardpoint].weaponCount && !hardPoint[curHardpoint].weaponPointer)
                {
                    if (theWeapon.IsMissile())
                        ReplaceMissile(curHardpoint, (MissileClass*)theWeapon);
                    if (theWeapon.IsBomb())
                        ReplaceBomb(curHardpoint, (BombClass*)theWeapon);
                }
                /*		// If next weapon is a visible weapon, attach it to the hardpoint (note: some delay to this would be cool)
                        if (hardPoint[curHardpoint].weaponPointer && visFlag & (1 << curHardpoint))
                            {
                            OTWDriver.CreateVisualObject(hardPoint[curHardpoint].weaponPointer);
                            OTWDriver.AttachObject(ownship.drawPointer, (DrawableBSP*)hardPoint[curHardpoint].weaponPointer.drawPointer, curHardpoint);
                            }
                */
                //
            }
        }
        public void DetachWeapon(int hardpoint, SimWeaponClass* theWeapon)
        {
            SimWeaponClass* weapPtr = hardPoint[hardpoint].weaponPointer;
            SimWeaponClass* lastPtr = null;

            while (weapPtr)
            {
                if (weapPtr == theWeapon)
                {
                    if (lastPtr)
                        lastPtr.nextOnRail = theWeapon.nextOnRail;
                    else
                        hardPoint[hardpoint].weaponPointer = theWeapon.nextOnRail;
                    theWeapon.nextOnRail = null;
                    return;
                }
                lastPtr = weapPtr;
                weapPtr = weapPtr.nextOnRail;
            }
        }

        public int CurStationOK() { return StationOK(curHardpoint); }
        public int StationOK(int n)
        {
            int retval = true;
            FackClass* mFaults;
            int broken;

            if (ownship.IsAirplane() && ((AircraftClass*)ownship).mFaults)
            {
                mFaults = ((AircraftClass*)ownship).mFaults;
                broken = mFaults.GetFault(FaultClass.sms_fault);
                if (broken & FaultClass.bus & FaultClass.fail)
                {
                    retval = false;
                }
                else if (n >= 1)
                {
                    if (broken & (FaultClass.sta1 << (n - 1)) & FaultClass.fail)
                    {
                        retval = false;
                    }
                }
            }

            return retval;
        }
        public int NumHardpoints() { return numHardpoints; }
        public int CurHardpoint() { return curHardpoint; }
        public int NumCurrentWpn() { return numCurrentWpn; }
        public void SetCurHardpoint(int newPoint) { curHardpoint = newPoint; }  // This should go one day
        public SimVehicleClass* Ownship() { return ownship; }
        public MasterArmState MasterArm() { return masterArm; }
        public void SetMasterArm(MasterArmState newState) { masterArm = newState; }
        public void StepMasterArm()
        {
            switch (masterArm)
            {
                case Safe:
                    SetMasterArm(Sim);
                    break;

                case Arm:
                    SetMasterArm(Safe);
                    break;

                default:
                    SetMasterArm(Arm);
                    break;
            }
        }
        public void StepCatIII()
        {
            //if(g_bEnableCATIIIExtension)	MI
            if (g_bRealisticAvionics)
            {
                if (((AircraftClass*)ownship).af.IsSet(AirframeClass.CATLimiterIII))
                    ((AircraftClass*)ownship).af.ClearFlag(AirframeClass.CATLimiterIII);
                else
                    ((AircraftClass*)ownship).af.SetFlag(AirframeClass.CATLimiterIII);
            }
        }

        public void ReplaceMissile(int station, MissileClass* theMissile)
        {
            SimWeaponClass weapPtr, newMissile, lastPtr;
            int visFlag;
            int slotId;

            if (IsSet(GunOnBoard))
                slotId = max(0, station - 1);
            else
                slotId = station;

            newMissile = InitAMissile(ownship, hardPoint[station].weaponId, theMissile.GetRackSlot());
            ((MissileClass*)(newMissile)).isCaged = TRUE;
            ((MissileClass*)(newMissile)).isSpot = FALSE;
            ((MissileClass*)(newMissile)).isSlave = TRUE;
            ((MissileClass*)(newMissile)).isTD = FALSE;
            visFlag = GetVehicleClassData(ownship.Type() - VU_LAST_ENTITY_TYPE).VisibleFlags;
            if (visFlag & (1 << station))
            {
                Debug.Assert(slotId >= 0);

                CreateDrawable(newMissile, OTWDriver.Scale());

                // Fix up drawable parent/child relations
                if (hardPoint[station].GetRack())
                    OTWDriver.AttachObject(hardPoint[station].GetRack(), (DrawableBSP*)(newMissile.drawPointer), theMissile.GetRackSlot());
                else if (ownship.drawPointer)
                    OTWDriver.AttachObject((DrawableBSP*)(ownship.drawPointer), (DrawableBSP*)(newMissile.drawPointer), slotId);
            }

            weapPtr = hardPoint[station].weaponPointer;
            while (weapPtr)
            {
                if (weapPtr == theMissile)
                {
                    if (lastPtr)
                        lastPtr.nextOnRail = newMissile;
                    else
                        hardPoint[station].weaponPointer = newMissile;
                    newMissile.nextOnRail = theMissile.nextOnRail;
                    theMissile.nextOnRail = NULL;
                    return;
                }
                lastPtr = weapPtr;
                weapPtr = weapPtr.nextOnRail;
            }

            //edg: we neglected to account for the case where the replaced missile
            //is already detatched (as it is in SMS BAse Launch Weapon) and thereby
            // leak missile mem.  If we got here, we need to attach the missile at
            // the end of the chain
            newMissile.nextOnRail = NULL;
            if (lastPtr)
                lastPtr.nextOnRail = newMissile;
            else
                hardPoint[station].weaponPointer = newMissile;
        }

        public void ReplaceRocket(int station, MissileClass* theMissile)
        {
            /*
                hardPoint[station].weaponCount = 19;
                hardPoint[station].weaponPointer = InitWeaponList (ownship, hardPoint[station].weaponId,
                  hardPoint[station].GetWeaponClass(), hardPoint[station].weaponCount, InitAMissile);
            */


            // Change hacks to datafile: terrdata\objects\falcon4.rkt
            int entryfound = 0;
            for (int j = 0; j < NumRocketTypes; j++)
            {
                if (hardPoint[station].weaponId == RocketDataTable[j].weaponId)
                {
                    hardPoint[station].weaponCount = RocketDataTable[j].weaponCount;
                    hardPoint[station].weaponPointer = InitWeaponList(ownship, hardPoint[station].weaponId,
                      hardPoint[station].GetWeaponClass(), hardPoint[station].weaponCount, InitAMissile);
                    entryfound = 1;
                    break;
                }
            }
            if (!entryfound)	// use generic 2.75mm rocket
            {
                hardPoint[station].weaponCount = 19;
                hardPoint[station].weaponPointer = InitWeaponList(ownship, hardPoint[station].weaponId,
                  hardPoint[station].GetWeaponClass(), hardPoint[station].weaponCount, InitAMissile);
            }
        }

        public void ReplaceBomb(int station, BombClass* theBomb)
        {
            VehicleClassDataType* vc;
            int visFlag;
            SimWeaponClass weapPtr, newBomb, lastPtr;

            newBomb = InitABomb(ownship, hardPoint[station].weaponId, theBomb.GetRackSlot());

            vc = GetVehicleClassData(ownship.Type() - VU_LAST_ENTITY_TYPE);
            visFlag = vc.VisibleFlags;

            if (visFlag & 1 << curHardpoint)
            {
                CreateDrawable(newBomb, OTWDriver.Scale());

                // Fix up drawable parent/child relations
                if (hardPoint[station].GetRack())
                {
                    OTWDriver.AttachObject(hardPoint[station].GetRack(), (DrawableBSP*)(newBomb.drawPointer), theBomb.GetRackSlot());
                }
                else if (ownship.drawPointer)
                {
                    OTWDriver.AttachObject((DrawableBSP*)(ownship.drawPointer), (DrawableBSP*)(newBomb.drawPointer), max(station - 1, 0));
                }
            }

            weapPtr = hardPoint[station].weaponPointer;
            while (weapPtr)
            {
                if (weapPtr == theBomb)
                {
                    if (lastPtr)
                        lastPtr.nextOnRail = newBomb;
                    else
                        hardPoint[station].weaponPointer = newBomb;
                    newBomb.nextOnRail = theBomb.nextOnRail;
                    theBomb.nextOnRail = NULL;
                    return;
                }
                lastPtr = weapPtr;
                weapPtr = weapPtr.nextOnRail;
            }
        }

        public void SetFlag(int newFlag) { flags |= newFlag; }
        public void ClearFlag(int newFlag) { flags &= ~newFlag; }
        public int IsSet(int newFlag) { return flags & newFlag; }

        public float GetWeaponRangeFeet(int hardpoint)
        {
            Debug.Assert(hardpoint >= 0 && hardpoint < numHardpoints);

            return WeaponDataTable[hardPoint[hardpoint].weaponId].Range * KM_TO_FT;
        }


        public void SelectBestWeapon(uchar* dam, int mt, int range_km, int guns_only = false, int alt_feet = -1) // 2002-03-09 MODIFIED BY S.G. Added the alt_feet variable so it knows the altitude of the target as well as it range
        {
            int i, str;
            int bhp = -1;
            int bw = 0, bs = 0;
            int wrange;
#if CHECK_PROC_TIMES
	ulong whole = GetTickCount();
#endif
            for (i = 0; i < numHardpoints; i++)
            {
                if (hardPoint[i] && hardPoint[i].weaponId && hardPoint[i].weaponCount)
                {
#if CHECK_PROC_TIMES
	ulong procTime = GetTickCount();
#endif
                    if (range_km >= 0 && !((Unit)(ownship.GetCampaignObject())).CanShootWeapon(hardPoint[i].weaponId))
                    {
                        str = 0;
                    } //JPO check 
                    else if (guns_only && hardPoint[i].weaponPointer && !hardPoint[i].weaponPointer.IsGun())
                    {
                        str = 0;
                    }
                    else if (range_km >= 0)
                    {
#if CHECK_PROC_TIMES
	procTime = GetTickCount() - procTime;
	gCanShoot += procTime;
	numCanShoot++;
	procTime = GetTickCount();
#endif
                        wrange = GetWeaponRange(hardPoint[i].weaponId, mt);
#if CHECK_PROC_TIMES
	procTime = GetTickCount() - procTime;
	gWeapRng += procTime;
	numWRng++;
	procTime = GetTickCount();
#endif
                        if (wrange > 2)
                        {
                            // 2000-10-12 MODIFIED BY S.G. DIVIDER OF 11 INSTEAD IF IT'S A GUN. THAT WAY, RANGE * LowAirModifier MIGHT STILL SELECT THIS WEAPON IF THE RANGE IS 15 NM.
                            //					if ( range_km < min(wrange/4, 2) || range_km > wrange )
                            //						continue;
                            if (hardPoint[i].weaponPointer.IsGun())
                            {
                                if (range_km < min(wrange / 11, 2) || range_km > wrange)
                                    continue;
                            }
                            // 2002-03-09 MODIFIED BY S.G. New weapon selection code based on the target min/max alt and range. If g_bAdvancedGroundChooseWeapon is false or alt_feet is -1, use the original code
                            // else if ( range_km < min(wrange/4, 2) || range_km > wrange )
                            //	continue;
                            else
                            {
                                // If we're shooting at an air thingy and this weapon is a ... STYPE_MISSILE_SURF_AIR, we might be restricted to a min/max engagement range/altitude if we asked for it
                                if (g_bAdvancedGroundChooseWeapon && alt_feet >= 0 && (mt == LowAir || mt == Air))
                                {
                                    // If we're outside the weapon's range, no point going further, no matter what the weapon is...
                                    if (range_km > wrange)
                                        continue;

                                    // If it's a SAM...
                                    VU_BYTE* classInfoPtr = Falcon4ClassTable[WeaponDataTable[hardPoint[i].weaponId].Index].vuClassData.classInfo_;
                                    if (classInfoPtr[VU_DOMAIN] == DOMAIN_AIR && classInfoPtr[VU_CLASS] == CLASS_VEHICLE && classInfoPtr[VU_TYPE] == TYPE_MISSILE && classInfoPtr[VU_STYPE] == STYPE_MISSILE_SURF_AIR)
                                    {
                                        MissileAuxData* auxData = null;
                                        SimWeaponDataType* wpnDefinition = &SimWeaponDataTable[Falcon4ClassTable[WeaponDataTable[hardPoint[i].weaponId].Index].vehicleDataIndex];
                                        if (wpnDefinition.dataIdx < numMissileDatasets)
                                            auxData = missileDataset[wpnDefinition.dataIdx].auxData;
                                        if (auxData)
                                        {
                                            float minAlt = auxData.MinEngagementAlt;
                                            float minRange = auxData.MinEngagementRange;
                                            float maxAlt = (float)WeaponDataTable[hardPoint[i].weaponId].MaxAlt * 1000.0f;
                                            // If our range is less than the min range, don't consider this weapon (used range squared to save a FPU costly sqrt)
                                            if (range_km * KM_TO_FT < minRange)
                                                continue;

                                            // If we haven't entered the MinEngagementAlt yet, use the one in the Falcon4.WCD file
                                            if (minAlt < 0.0f)
                                                minAlt = (float)(WeaponDataTable[hardPoint[i].weaponId].Name[18]) * 32.0F;

                                            // If less than min altitude or more than max altitude (in this case alt_feet is POSITIVE if we're below the target), don't consider this weapon
                                            if (alt_feet < minAlt || alt_feet > maxAlt)
                                                continue;
                                        }
                                        // No auxiliary data, default to orinal min range test...
                                        else if (range_km < min(wrange / 4, 2))
                                            continue;
                                    }
                                    // Not a SAM but was within wrange, check if too close
                                    else if (range_km < min(wrange / 4, 2))
                                        continue;
                                }
                                // Original line if we don't want advanced weapon selection or our target isn't in the air or alt_feet wasn't passed (ie, is -1)
                                else if (range_km < min(wrange / 4, 2) || range_km > wrange)
                                    continue;
                            }
                            // END OF MODIFIED SECTION 2002-03-09
                            // END OF MODIFIED SECTION
                        }

                        str = GetWeaponScore(hardPoint[i].weaponId, dam, mt, range_km, wrange);
#if CHECK_PROC_TIMES
	procTime = GetTickCount() - procTime;
	gWeapScore += procTime;
	numWeapScore++;
#endif
                    }
                    else
                        str = WeaponDataTable[hardPoint[i].weaponId].HitChance[mt]; //GetWeaponScore (hardPoint[i].weaponId, dam, mt, 1);

                    if (str > bs)
                    {
                        bw = hardPoint[i].weaponId;
                        bs = str;
                        bhp = i;
                    }
                }
            }
            curHardpoint = bhp;
#if CHECK_PROC_TIMES
	whole = GetTickCount() - whole;
	gSelBestWeap += whole;
	numCalls++;
#endif
        }

        public MasterArmState masterArm;	//MI moved from protected
        //MI
        public bool BHOT;
        public bool GndJett;
        public bool FEDS;
        public bool DrawFEDS;
        public bool Powered;	//for mav's
        public float MavCoolTimer;
        public enum MavSubModes { PRE, VIS, BORE };
        public MavSubModes MavSubMode;
        public void ToggleMavPower() { Powered = !Powered; }
        public void StepMavSubMode(bool init = false)
        {
            RadarDopplerClass* theRadar = (RadarDopplerClass*)FindSensor(SimDriver.playerEntity, SensorClass.Radar);
            FireControlComputer* FCC = ownship.GetFCC();
            if (!theRadar || !FCC)
                return;

            if (init)
            {
                MavSubMode = SMSBaseClass.PRE;
                theRadar.SelectGM();
                theRadar.SetScanDir(1.0F);
                FCC.SetSubMode(FireControlComputer.SLAVE);
                return;
            }
            if (MavSubMode == SMSBaseClass.PRE)
            {
                MavSubMode = SMSBaseClass.VIS;
                theRadar.SelectAGR();
                theRadar.SetScanDir(1.0F);
                FCC.SetSubMode(FireControlComputer.BSGT);
            }
            else if (MavSubMode == SMSBaseClass.VIS)
            {
                MavSubMode = SMSBaseClass.BORE;
                theRadar.SelectGM();
                theRadar.SetScanDir(1.0F);
                FCC.SetSubMode(FireControlComputer.BSGT);
            }
            else
            {
                MavSubMode = SMSBaseClass.PRE;
                theRadar.SelectGM();
                theRadar.SetScanDir(1.0F);
                FCC.SetSubMode(FireControlComputer.SLAVE);
            }
        }

        protected int numHardpoints;
        protected int curHardpoint;
        protected int numCurrentWpn;
        protected int flags;
        protected SimVehicleClass* ownship;
    }

    // ==================================================================
    // SMSClass
    // 
    // This is Leon's origional class, now used only for aircraft/helos
    // ==================================================================

    public class SMSClass : SMSBaseClass
    {
#if USE_SH_POOLS
   public:
      // Overload new/delete to use a SmartHeap fixed size pool
      void *operator new(size_t size) { Debug.Assert( size == sizeof(SMSClass) ); return MemAllocFS(pool);	};
      void operator delete(void *mem) { if (mem) MemFreeFS(mem); };
      static void InitializeStorage()	{ pool = MemPoolInitFS( sizeof(SMSClass), 200, 0 ); };
      static void ReleaseStorage()	{ MemPoolFree( pool ); };
      static MEM_POOL	pool;
#endif



        protected char flash;
        protected int rippleInterval, pair, rippleCount, curRippleCount;
        protected float burstHeight;
        protected ulong nextDrop;
        protected void ReleaseCurWeapon(int stationUnderTest)
        {
            MissileClass* theMissile;

            if (curWeapon)
            {
                switch (curWeaponType)
                {
                    case wtAgm65:
                    case wtAgm88:
                        Debug.Assert(curWeapon.IsMissile());
                        theMissile = (MissileClass*)curWeapon;
                        if (theMissile.display)
                        {
                            theMissile.display.DisplayExit();
                        }

                    case wtGBU:
                        {
                            // MN blind shot from JPO - does this fix the LGB crash/hardlock ?
                            // MN commented back in - I think not performing the DisplayExit will result in memory leaks,
                            // as each missile seems to have its own display initialised

                            SensorClass* laserPod = FindLaserPod(ownship);

                            if (laserPod)
                            {
                                if (laserPod.GetDisplay())
                                    laserPod.DisplayExit();
                            }
                        }
                        break;
                }

            }
            curHardpoint = newStation;
            curWpnNum = -1;
            curWeapon = null;
            curWeaponType = wtNone;
            curWeaponClass = wcNoWpn;
            curWeaponId = -1;
        }


        protected void JettisonStation(int stationNum, int rippedOff = false)
        {
            SimWeaponClass tempPtr, weapPtr;
            DrawableBSP* droppedThing;

            // only works for valid stations with positive G's on a local machine
            if (stationNum < 0)
            {
                return;
            }

            //MI
            if (!g_bRealisticAvionics)
            {
                if ((ownship.IsLocal()) && (((AircraftClass*)ownship).af.nzcgb <= 0.0F))
                {
                    return;
                }
            }
            else
            {
                if (ownship.IsLocal() && !ownship.OnGround())
                {
                    if (((AircraftClass*)ownship).af.nzcgb <= 0.0F)
                        return;
                }
            }

            if (hardPoint[stationNum].weaponPointer || hardPoint[stationNum].GetRack())
            {
                if (hardPoint[stationNum].GetRack())
                {
                    droppedThing = hardPoint[stationNum].GetRack();
                    weapPtr = hardPoint[stationNum].weaponPointer;
                    RemoveStore(stationNum, hardPoint[stationNum].GetRackId());
                    hardPoint[stationNum].SetRack(null);
                }
                else
                {
                    droppedThing = (DrawableBSP*)hardPoint[stationNum].weaponPointer.drawPointer;
                    hardPoint[stationNum].weaponPointer.drawPointer = null;
                    weapPtr = null;
                    RemoveStore(stationNum, hardPoint[stationNum].weaponId);
                }

                if (droppedThing)
                {
                    Tpoint pos, vec;

                    // Detach the thing from the parent object
                    OTWDriver.DetachObject((DrawableBSP*)(ownship.drawPointer), (DrawableBSP*)droppedThing, stationNum - 1);

                    // Get initial location and velocity for the "SFX" container
                    droppedThing.GetPosition(&pos);
                    vec.x = ownship.XDelta();
                    vec.y = ownship.YDelta();
                    vec.z = ownship.ZDelta();

                    // Create and add the "SFX" container
                    droppedThing.SetLabel("", 0xff00ff00);
                    OTWDriver.AddSfxRequest(new SfxClass(
                        SFX_MOVING_BSP,				// type
                        &pos,						// world pos
                        &vec,						// vector
                        droppedThing,				// BSP
                        30.0f,						// time to live
                        1.0f));					// scale

                    // Play the jettison sound unless the thing is being ripped off, in which case it takes care of itself
                    if (!rippedOff)
                        F4SoundFXSetPos(SFX_JETTISON, true, pos.x, pos.y, pos.z, 1.0f);

                    // If this was a rack, we will drop all its children seperatly to ensure proper cleanup.
                    // SCR:  It would be nice if this could be handled by a sub-class of SfxClass.
                    while (weapPtr)
                    {

                        // Get the child drawable (if any)
                        DrawableBSP* child = (DrawableBSP*)weapPtr.drawPointer;
                        if (child)
                        {

                            // Detach the child from the parent rack
                            OTWDriver.DetachObject(droppedThing, child, weapPtr.GetRackSlot());
                            weapPtr.drawPointer = null;

                            // Get initial location for the "SFX" container
                            child.GetPosition(&pos);

                            // Create and add the "SFX" container
                            droppedThing.SetLabel("", 0xff00ff00);
                            OTWDriver.AddSfxRequest(new SfxClass(
                                SFX_MOVING_BSP,				// type
                                &pos,						// world pos
                                &vec,						// vector
                                child,						// BSP
                                30.0f,						// time to live
                                1.0f));					// scale
                        }

                        // Get the next weapon
                        weapPtr = weapPtr.GetNextOnRail();
                    }
                }

                // If it's  fuel tank and it has anything in it, remove it
                if (hardPoint[stationNum].GetWeaponClass() == wcTank && ownship.IsAirplane())
                {
                    //			float lostFuel = ((AircraftClass*)ownship).af.ExternalFuel() / numOnBoard[hardPoint[stationNum].GetWeaponClass()];
                    // JPO redo with ne fuel stuff
                    int center = (numHardpoints - 1) / 2 + 1;
                    if (stationNum < center)
                        ((AircraftClass*)ownship).af.DropTank(AirframeClass.TANK_LEXT); // XXX DROP
                    else if (stationNum > center)
                        ((AircraftClass*)ownship).af.DropTank(AirframeClass.TANK_REXT); // XXX DROP
                    else ((AircraftClass*)ownship).af.DropTank(AirframeClass.TANK_CLINE); // XXX DROP

                    //			((AircraftClass*)ownship).af.AddExternalFuel (-lostFuel);
                }

                // Iterate all the weapons on this station and clean them up
                weapPtr = hardPoint[stationNum].weaponPointer;
                tempPtr = null;
                while (weapPtr)
                {
                    Debug.Assert(weapPtr.drawPointer == null);
                    if (FalconLocalGame.GetGameType() != game_InstantAction)
                        tempPtr = weapPtr.GetNextOnRail();
                    RemoveStore(stationNum, hardPoint[stationNum].weaponId);
                    // 2002-02-08 ADDED BY S.G. Before we delete it, we must check if the drawable.thePrevMissile is pointing to it but NOT referenced so we can clear it as well otherwise it will CTD in UpdateGroundSpot
                    if (drawable && drawable.thePrevMissile && drawable.thePrevMissile == weapPtr && !drawable.thePrevMissileIsRef)
                        drawable.thePrevMissile = null; // Clear it as well
                    delete weapPtr;
                    //			vuAntiDB.Remove(weapPtr);
                    weapPtr = tempPtr;
                }
                hardPoint[stationNum].weaponPointer = null;
                numOnBoard[hardPoint[stationNum].GetWeaponClass()] -= hardPoint[stationNum].weaponCount;
                hardPoint[stationNum].weaponCount = 0;
            }


            {
                ChooseLimiterMode(1); // (me1234 lets check airframe_g-limit after stores jettison)
            }
        }

        protected void SetCurrentWeapon(int station, SimWeaponClass* weapon)
        {
            lastWpnStation = curHardpoint;
            lastWpnNum = curWpnNum;

            if (station >= 0)
            {
                curHardpoint = station;
                if (weapon)
                    curWpnNum = weapon.GetRackSlot();
                else
                    curWpnNum = -1;
                curWeapon = weapon;
                curWeaponType = hardPoint[station].GetWeaponType();
                curWeaponClass = hardPoint[station].GetWeaponClass();
                curWeaponId = hardPoint[station].weaponId;
                curWeaponDomain = hardPoint[station].Domain();

                if (curWeapon && (curWeaponClass == wcHARMWpn))
                {
                    Debug.Assert(curWeapon.IsMissile());
                    ((MissileClass*)curWeapon).display = FindSensor(ownship, SensorClass.HTS);
                }
            }
            else
            {
                curWeapon = null;
                curWeaponType = wtNone;
                curWeaponClass = wcNoWpn;
                curWeaponId = -1;
                curWeaponDomain = wdNoDomain;
            }
            //MI since this get's called AFTER the ChooseLimiterMode() function above when launching AG
            //missiles, we're not checking with the correct loadout. So let's just check here again.
            //This fixes the bug with AG missiles and OverG when we only have the rack left
            ChooseLimiterMode(1);
        }

        protected void SetupHardpointImage(BasicWeaponStation* hp, int count)
        {
            Debug.Assert(hp);
            if (!hp)
                return;
            // Find the proper rack id && max points
            if (hp.GetWeaponClass() == wcRocketWpn)
            {
                // The rack id should have already been set up in SMSBaseClass
                hp.SetupPoints(1);
            }
            else if (count == 1)
            {
                if (hp.GetWeaponClass() == wcAimWpn)
                {
                    hp.SetupPoints(1);
                    hp.SetRackId(gRackId_Single_AA_Rack);
                }
                else
                {
                    hp.SetupPoints(1);
                    hp.SetRackId(gRackId_Single_Rack);
                }
            }
            else if (count <= 2 && hp.GetWeaponClass() == wcAimWpn)
            {
                hp.SetupPoints(2);
                hp.SetRackId(gRackId_Two_Rack);
            }
            else if (count <= 3)
            {
                if (hp.GetWeaponClass() == wcAgmWpn)
                {
                    hp.SetupPoints(3);
                    hp.SetRackId(gRackId_Mav_Rack);
                }
                else
                {
                    hp.SetupPoints(3);
                    hp.SetRackId(gRackId_Triple_Rack);
                }
            }
            else if (count <= 4)
            {
                hp.SetupPoints(4);
                hp.SetRackId(gRackId_Quad_Rack);
            }
            else
            {
                hp.SetupPoints(6);
                hp.SetRackId(gRackId_Six_Rack);
            }
        }



        public int[] numOnBoard = new int[wcNoWpn + 1];
        public int curWpnNum;
        public int lastWpnStation, lastWpnNum;
        public WeaponType curWeaponType;
        public WeaponDomain curWeaponDomain;
        public WeaponClass curWeaponClass;
        public SmsDrawable* drawable;
        public SimWeaponClass* curWeapon;
        public short curWeaponId;

        public SMSClass(SimVehicleClass* newOwnship, short* weapId, uchar* weapCnt)
            : base(newOwnship, weapId, weapCnt, true)
        {
            int i;
            Falcon4EntityClassType* classPtr;
            SimWeaponDataType* wpnDefinition;
            int dataIndex;
            VehicleClassDataType* vc;
            WeaponClassDataType* wc;
            int rackFlag, visFlag;
            GunClass* gun;

            flash = false;
            curHardpoint = -1;
            curWpnNum = -1;
            curWeaponId = -1;
            curWeapon = null;
            curWeaponType = wtNone;
            curWeaponClass = wcNoWpn;
            curWeaponDomain = wdNoDomain;
            flags = false;
            rippleCount = 0;
            //rippleInterval = 25;
            rippleInterval = 125; // JB 010701
            curRippleCount = 0;
            pair = false;
            nextDrop = 0;
            // default burst height to middle of 300 to 3000 range
            burstHeight = 1500.0F;
            armingdelay = 480; //me123 status ok. addet
            aim120id = 0; // JPO added.
            aim9mode = WARM;
            aim9cooltime = 3.0F;
            aim9warmtime = 0.0F;
            aim9coolingtimeleft = 1.5 * 60 * 60;//1.5 * 60.0 * 60.0 * CLOCKS_PER_SEC;
            // Test - 10 seconds of coolant
            // aim9coolingtimeleft = 10.0F;// * CLOCKS_PER_SEC;
            aim9LastRunTime = 0;
            drawable = null;

            //MI
            angle = 23;
            Prof1RP = 0;
            Prof2RP = 3;
            Prof1RS = 175;
            Prof2RS = 25;
            Prof1Pair = false;
            Prof2Pair = false;
            Prof1NSTL = 0;
            Prof2NSTL = 1;
            C1AD1 = 600;
            C1AD2 = 150;
            C2AD = 400;
            C2BA = 500;
            Prof1SubMode = FireControlComputer.CCRP;
            Prof2SubMode = FireControlComputer.CCIP;
            Prof1Pair = false;
            Prof2Pair = true;
            Prof1 = false;

            //MI
            BHOT = true;
            GndJett = false;
            FEDS = false;
            DrawFEDS = false;
            Powered = false;	//for Mav's
            MavCoolTimer = 5.0F;	//5 seconds cooling time (a guess)
            MavSubMode = PRE;

            Debug.Assert(weapCnt);

            for (i = 0; i <= wtNone; i++)
                numOnBoard[i] = 0;

            // Set up advanced hardpoint data
            vc = GetVehicleClassData(newOwnship.Type() - VU_LAST_ENTITY_TYPE);
            rackFlag = vc.RackFlags;
            visFlag = vc.VisibleFlags;
            for (i = 0; i < numHardpoints; i++)
            {
                if (hardPoint[i] && hardPoint[i].weaponId && hardPoint[i].weaponPointer)
                {
                    wc = &WeaponDataTable[hardPoint[i].weaponId];
                    classPtr = &(Falcon4ClassTable[wc.Index]);
                    dataIndex = classPtr.vehicleDataIndex;
                    // 2002-03-21 MN catch data errors
                    Debug.Assert(wc);
                    Debug.Assert(classPtr);


                    //LRKLUDGE
                    if ((classPtr.vuClassData.classInfo_[VU_TYPE] == TYPE_RECON &&
                        classPtr.vuClassData.classInfo_[VU_STYPE] == STYPE_CAMERA))
                    {
                        dataIndex = Rpod_DEF;
                    }

                    //			wpnDefinition = (SimWpnDefinition*)moverDefinitionData[dataIndex];
                    wpnDefinition = &SimWeaponDataTable[classPtr.vehicleDataIndex];
                    hardPoint[i].SetWeaponClass((WeaponClass)wpnDefinition.weaponClass);
                    hardPoint[i].SetWeaponType((WeaponType)wpnDefinition.weaponType);
                    hardPoint[i].GetWeaponData().domain = (WeaponDomain)wpnDefinition.domain;
                    // edg kludge, look for durandal and set its drag to value
                    // the weapon def files (at least for bombs) all seem to point
                    // to the same thing -- mkxxx
                    hardPoint[i].GetWeaponData().cd = wpnDefinition.cd;
                    if (classPtr.vuClassData.classInfo_[VU_TYPE] == TYPE_BOMB)
                    {
                        if (classPtr.vuClassData.classInfo_[VU_STYPE] == STYPE_BOMB &&
                            classPtr.vuClassData.classInfo_[VU_SPTYPE] == SPTYPE_DURANDAL)
                        {
                            hardPoint[i].GetWeaponData().cd = 1.0f;
                        }
                        // JB 010707 Why?? This makes the drag insignificant.  Make it configurable
                        else if (hardPoint[i].GetWeaponData().cd < 1.0f)
                        {
                            //hardPoint[i].GetWeaponData().cd *= 0.01f;
                            hardPoint[i].GetWeaponData().cd *= 0.2f * g_fDragDilutionFactor;
                        }
                        else
                        {
                            // if it's not a durandal and it's got a value >= 1
                            // make it a a drag of 0.9 so that the bombclass doesn't
                            // think it's a durandal. did I hear kludge?
                            hardPoint[i].GetWeaponData().cd = 0.9f;
                        }
                    }

                    //LRKLUDGE problem w/ AA7R's
                    if ((classPtr.vuClassData.classInfo_[VU_TYPE] == TYPE_MISSILE &&
                        classPtr.vuClassData.classInfo_[VU_STYPE] == STYPE_MISSILE_AIR_AIR &&
                        classPtr.vuClassData.classInfo_[VU_SPTYPE] == SPTYPE_AA7R))
                    {
                        hardPoint[i].SetWeaponType(wtAim120);
                    }

                    hardPoint[i].GetWeaponData().area = wpnDefinition.area;
                    hardPoint[i].GetWeaponData().weight = wpnDefinition.weight;
                    hardPoint[i].GetWeaponData().xEjection = wpnDefinition.xEjection;
                    hardPoint[i].GetWeaponData().yEjection = wpnDefinition.yEjection;
                    hardPoint[i].GetWeaponData().zEjection = wpnDefinition.zEjection;
                    hardPoint[i].GetWeaponData().flags = wpnDefinition.flags;

                    // Set CBU flag appropriatly
                    if (wc.Flags & WEAP_CLUSTER)
                        hardPoint[i].GetWeaponData().flags |= HasBurstHeight;

                    strcpy(hardPoint[i].GetWeaponData().mnemonic, wpnDefinition.mnemonic);
                    IncrementStores(hardPoint[i].GetWeaponClass(), hardPoint[i].weaponCount);
                    gun = GetGun(i);
                    if (gun)
                    {
                        // Special stuff for guns
                        SetFlag(GunOnBoard);
                        hardPoint[i].SetGun(gun);
                        hardPoint[i].GetWeaponData().xEjection = gun.initBulletVelocity;
                        if (!g_bUseDefinedGunDomain) // 2002-04-17 ADDED BY S.G. Why fudge the weapon domain of guns instead of relying on what's in the data file?
                            hardPoint[i].GetWeaponData().domain = gun.GetSMSDomain();
                    }

                    // Setup rack data
                    if (rackFlag & (1 << i))
                    {
                        // 2002-03-24 MN Helicopter also create a SMS class, but don't have auxaerodata, so add a check for Airplane
                        if (g_bNewRackData && ownship.IsAirplane() && ((AircraftClass*)ownship).af && wc)
                        { // JPO new scheme
                            int rack = FindBestRackIDByPlaneAndWeapon(((AircraftClass*)ownship).af.GetRackGroup(i),
                                wc.SimweapIndex, weapCnt[i]);
                            Debug.Assert(rack < MaxRackObjects); // -1 means nothing defined currently fallback to old scheme
                            if (rack > 0 && rack < MaxRackObjects)
                            {
                                RackObject* rackp = &RackObjectTable[rack];
                                hardPoint[i].SetupPoints(rackp.maxoccupancy);
                                if ((wc.Flags & WEAP_ALWAYSRACK) == 0)
                                {
                                    int rackid = (short)(((int)Falcon4ClassTable[rackp.ctind].dataPtr - (int)WeaponDataTable) / sizeof(WeaponClassDataType));
                                    hardPoint[i].SetRackId(rackid);
                                }
                            }
                            else
                            { // emergency default
                                SetupHardpointImage(hardPoint[i], weapCnt[i]);
                            }
                        }
                        else
                        {
                            SetupHardpointImage(hardPoint[i], weapCnt[i]);
                        }
                    }
                    else
                    {
                        hardPoint[i].SetupPoints(1);
                        hardPoint[i].SetRackId(0);
                    }
                }
            }

            // Check for HARM, LGB, ECM
            if (numOnBoard[wcHARMWpn] > 0)
            {
                SetFlag(HTSOnBoard);
            }

            if (numOnBoard[wcGbuWpn] > 0)
            {
                SetFlag(LGBOnBoard);
            }

            // 2000-11-17 MODIFIED BY S.G. SO INTERNAL ECMS ARE ACCOUNTING FOR
            //	if (numOnBoard[wcECM] > 0)
            if (numOnBoard[wcECM] > 0 || vc.Flags & VEH_HAS_JAMMER)
            {
                SetFlag(SPJamOnBoard);
            }
        }

        public virtual ~SMSClass();

        public virtual void AddWeaponGraphics()
        {
            int i, j;
            Tpoint simView;
            Trotation viewRot = IMatrix;
            float xOff, yOff, zOff;
            VehicleClassDataType* vc;
            int rackFlag, visFlag;
            SimWeaponClass* weapPtr;
            DrawableBSP* drawPtr = (DrawableBSP*)ownship.drawPointer;

            if (!hardPoint)
                return;

            vc = GetVehicleClassData(ownship.Type() - VU_LAST_ENTITY_TYPE);
            rackFlag = vc.RackFlags;
            visFlag = vc.VisibleFlags;
            for (i = 0; i < numHardpoints; i++)
            {
                if (hardPoint[i] && hardPoint[i].weaponPointer)
                {
                    if (hardPoint[i].weaponPointer.IsGun())
                        ((GunClass*)hardPoint[i].weaponPointer).InitTracers();
                    else if (i > 0)
                    {
                        if (visFlag & (1 << i))
                        {
                            drawPtr.GetChildOffset(i - 1, &simView);
                        }
                        else
                        {
                            simView.x = 0.0F;
                            simView.y = 0.0F;
                            simView.z = 0.0F;
                        }
                        hardPoint[i].SetPosition(simView.x, simView.y, simView.z);

                        if (rackFlag & (1 << i))
                        {
                            // Create the proper type of rack (from rack Id)
                            int rackid = hardPoint[i].GetRackId();
                            Debug.Assert(rackid >= 0 && rackid < NumWeaponTypes);
                            if (rackid >= 0 && rackid < NumWeaponTypes && WeaponDataTable[rackid].Index >= 0) // JB 010805 CTD
                            {
                                hardPoint[i].SetRack(new DrawableBSP(Falcon4ClassTable[WeaponDataTable[rackid].Index].visType[0], &simView, &viewRot, OTWDriver.Scale()));
                                OTWDriver.AttachObject(drawPtr, hardPoint[i].GetRack(), i - 1);
                                // DAVE: This is where you want to add the Rack Drag call.
                                AddStore(i, hardPoint[i].GetRackId(), (visFlag & (1 << i)));
                            }
                        }

                        if (hardPoint[i].GetWeaponClass() == wcRocketWpn && hardPoint[i].GetRack())
                        {
                            hardPoint[i].GetRack().SetSwitchMask(0, 1);
                            continue;
                        }
                        else if (hardPoint[i].GetRack())
                        {
                            // Use the rack offset in addition to our location offset
                            hardPoint[i].GetPosition(&xOff, &yOff, &zOff);
                            // MN check sanity of data
                            Debug.Assert(zOff > -10000.0F);
                            Debug.Assert(yOff < 10000.0F);
                            Debug.Assert(xOff < 10000.0F);
                            weapPtr = hardPoint[i].weaponPointer;
                            while (weapPtr)
                            {
                                j = weapPtr.GetRackSlot();
                                // 2002-03-27 MN Catch the "AdvancedWeaponStation" crash
                                F4Assert(j < hardPoint[i].NumPoints());
                                if (j >= hardPoint[i].NumPoints())
                                    j = hardPoint[i].NumPoints() - 1;
                                if (visFlag & (1 << i))
                                {
                                    OTWDriver.CreateVisualObject(weapPtr);
                                    OTWDriver.AttachObject(hardPoint[i].GetRack(), (DrawableBSP*)(weapPtr.drawPointer), j);
                                }
                                simView.x = simView.y = simView.z = 0.0F;
                                hardPoint[i].GetRack().GetChildOffset(j, &simView);
                                // MN data sanity check
                                Debug.Assert(simView.z > -10000.0F);
                                Debug.Assert(simView.y < 10000.0F);
                                Debug.Assert(simView.x < 10000.0F);
                                hardPoint[i].SetSubPosition(j, xOff + simView.x, yOff + simView.y, zOff + simView.z);
                                hardPoint[i].SetSubRotation(j, 0.0F, 0.0F);
                                AddStore(i, hardPoint[i].weaponId, (visFlag & (1 << i)));
                                weapPtr = weapPtr.GetNextOnRail();
                            }
                        }
                        else
                        {
                            // Use only our location offset, and only create drawable for first weapon
                            weapPtr = hardPoint[i].weaponPointer;
                            while (weapPtr)
                            {
                                if (weapPtr == hardPoint[i].weaponPointer && visFlag & (1 << i))
                                {
                                    // First visible weapon is drawn
                                    OTWDriver.CreateVisualObject(weapPtr);
                                    OTWDriver.AttachObject(drawPtr, (DrawableBSP*)(weapPtr.drawPointer), i - 1);
                                }
                                hardPoint[i].SetSubPosition(0, simView.x, simView.y, simView.z);
                                hardPoint[i].SetSubRotation(0, 0.0F, 0.0F);
                                AddStore(i, hardPoint[i].weaponId, (visFlag & (1 << i)));
                                weapPtr = weapPtr.GetNextOnRail();
                            }
                        }
                    }
                }
            }
        }

        public virtual void FreeWeaponGraphics()
        {
            int i;
            DrawableBSP* drawPtr = (DrawableBSP*)ownship.drawPointer;
            SimWeaponClass* weapPtr;

            for (i = 0; i < numHardpoints; i++)
            {
                if (hardPoint[i] && hardPoint[i].GetRack())
                {
                    // Detach all weapons from the rack and remove drawables
                    weapPtr = hardPoint[i].weaponPointer;
                    while (weapPtr)
                    {
                        if (weapPtr.IsMissile())
                        {
                            ((MissileClass*)weapPtr).SetTarget(null);
                            ((MissileClass*)weapPtr).ClearReferences();
                        }
                        else if (weapPtr.IsBomb())
                            ((BombClass*)weapPtr).SetTarget(null);

                        RemoveStore(i, hardPoint[i].weaponId);
                        if (weapPtr.drawPointer)
                        {
                            OTWDriver.DetachObject(hardPoint[i].GetRack(), (DrawableBSP*)(weapPtr.drawPointer), weapPtr.GetRackSlot());
                            OTWDriver.RemoveObject(weapPtr.drawPointer, true);
                            weapPtr.drawPointer = null;
                        }
                        weapPtr = weapPtr.GetNextOnRail();
                    }
                    // Detach rack from the parent
                    OTWDriver.DetachObject(drawPtr, hardPoint[i].GetRack(), i - 1);
                    // Remove the rack's drawable
                    RemoveStore(i, hardPoint[i].GetRackId());
                    OTWDriver.RemoveObject(hardPoint[i].GetRack(), true);
                    hardPoint[i].SetRack(null);
                }
                else
                {
                    // Detach all weapons from the parent and remove drawables
                    weapPtr = hardPoint[i].weaponPointer;
                    while (weapPtr)
                    {
                        if (weapPtr.IsMissile())
                        {
                            ((MissileClass*)weapPtr).SetTarget(null);
                            ((MissileClass*)weapPtr).ClearReferences();
                        }
                        else if (weapPtr.IsBomb())
                            ((BombClass*)weapPtr).SetTarget(null);

                        RemoveStore(i, hardPoint[i].weaponId);
                        if (weapPtr.drawPointer)
                        {
                            OTWDriver.DetachObject(drawPtr, (DrawableBSP*)(weapPtr.drawPointer), i - 1);
                            OTWDriver.RemoveObject(weapPtr.drawPointer, true);
                            weapPtr.drawPointer = null;
                        }
                        weapPtr = weapPtr.GetNextOnRail();
                    }
                }
                if (GetGun(i))
                    GetGun(i).CleanupTracers();
            }
        }
        public virtual SimWeaponClass* GetCurrentWeapon() { return curWeapon; }

        public void SetWeaponType(WeaponType newMode)
        {
            curWeaponType = newType;
        }
        public void IncrementStores(WeaponClass wClass, int count)
        {
            Debug.Assert(wClass <= wcNoWpn && wClass >= 0);
            numOnBoard[wClass] += count;
            Debug.Assert(numOnBoard[wClass] >= 0);
            if (numOnBoard[wClass] < 0)
                numOnBoard[wClass] = 0;
        }

        public void DecrementStores(WeaponClass wClass, int count)
        {
            Debug.Assert(wClass <= wcNoWpn && wClass >= 0);
            numOnBoard[wClass] -= count;
            Debug.Assert(numOnBoard[wClass] >= 0);
            if (numOnBoard[wClass] < 0)
                numOnBoard[wClass] = 0;
        }

        public void SelectiveJettison()
        {
            int curStation;

            if (drawable)
            {
                for (curStation = numHardpoints - 1; curStation > 0; curStation--)
                {
                    if (drawable.hardPointSelected & (1 << curStation) && MasterArm() != Safe)
                    {
                        MonoPrint("Jettison station %d at %ld\n", curStation, SimLibElapsedTime);
                        ReleaseCurWeapon(-1);
                        JettisonStation(curStation);
                        drawable.hardPointSelected -= (1 << curStation);
                    }
                }
            }

            /*   // Can't jettison the gun
                if (curHardpoint > 0)
                {
            //		MonoPrint ("Jettison station %d\n", curStation);
                    ReleaseCurWeapon (-1);
                    JettisonStation (curHardpoint);
                }
               */
        }

        public void EmergencyJettison()
        {
            int curStation;
            //me123 make sure we don't keep doing this...a mp messages is tranmitted every time.
            if (flags & EmergencyJettisonFlag) return;
            //MI
            if (!g_bRealisticAvionics)
            {
                if (ownship.OnGround())
                    return;
            }
            else
            {
                if (ownship.OnGround() && !GndJett)
                    return;
            }

            for (curStation = numHardpoints - 1; curStation > 0; curStation--)
            {
                // OW Jettison fix
                /*
                        if( !(((AircraftClass *)ownship).IsF16() &&
                         (curStation == 1 || curStation == 9 || hardPoint[curStation].GetWeaponClass() == wcECM)) &&
                         hardPoint[curStation].GetRack())
                */
                // 2002-04-21 MN this fixes release of AA weapons for F-16's, but other aircraft drop all the stuff . crap
                // new code by Pogo - just check not to drop station 1 and 9 and no AA and ECM for F-16's, no AA and ECM for all

                if (!g_bEmergencyJettisonFix)
                {
                    if (hardPoint[curStation] && (!(((AircraftClass*)ownship).IsF16() &&
                        (curStation == 1 || curStation == 9 || hardPoint[curStation].GetWeaponClass() == wcECM || hardPoint[curStation].GetWeaponClass() == wcAimWpn)) &&
                        (hardPoint[curStation].GetRack() || curStation == 5 && hardPoint[curStation].GetWeaponClass() == wcTank)))//me123 in the line above addet a check so we don't emergency jettison a-a missiles
                    {
                        MonoPrint("Jettison station %d at %ld\n", curStation, SimLibElapsedTime);
                        ReleaseCurWeapon(-1);
                        JettisonStation(curStation);
                    }
                }
                else
                {

                    if (((AircraftClass*)ownship).IsF16())
                    {
                        if (!(curStation == 1 || curStation == 9 || hardPoint[curStation].GetWeaponClass() == wcECM || hardPoint[curStation].GetWeaponClass() == wcAimWpn) &&
                            (hardPoint[curStation].GetRack() || curStation == 5 && hardPoint[curStation].GetWeaponClass() == wcTank))
                        {
                            ReleaseCurWeapon(-1);
                            JettisonStation(curStation);
                        }
                    }
                    else
                    {
                        if (hardPoint[curStation] && !(hardPoint[curStation].GetWeaponClass() == wcECM ||
                            hardPoint[curStation].GetWeaponClass() == wcAimWpn))
                        {
                            ReleaseCurWeapon(-1);
                            JettisonStation(curStation);
                        }

                    }
                }
            }

            if (ownship.IsLocal())
            {
                // Create and fill in the message structure
                FalconTrackMessage* trackMsg = new FalconTrackMessage(1, ownship.Id(), FalconLocalGame);
                trackMsg.dataBlock.trackType = Track_JettisonAll;
                trackMsg.dataBlock.hardpoint = 0;
                trackMsg.dataBlock.id = ownship.Id();

                // Send our track list
                FalconSendMessage(trackMsg, true);
            }

            // Set a permanent flag indicating that we've done the deed
            flags |= EmergencyJettisonFlag;
        }

        public void JettisonWeapon(int hardpoint)
        {
            ReleaseCurWeapon(-1);
            JettisonStation(hp);
        }

        public void RemoveWeapon(int hardpoint)
        {
            SimWeaponClass
                * weapPtr;

            DrawableBSP
                * drawPtr;

            int
                slot;

            if
            (
                (hardPoint) &&
                (hp > -1) &&
                (hardPoint[hp].weaponPointer) &&
                (hardPoint[hp].weaponCount > 0)
            )
            {
                weapPtr = hardPoint[hp].weaponPointer;

                if (!weapPtr || F4IsBadReadPtr(weapPtr, sizeof(SimWeaponClass))) // JB 010222 CTD
                    return; // JB 010222 CTD

                if (hardPoint[hp].GetRack())
                {

                    //while (!weapPtr.drawPointer) // JB 010222 CTD
                    while (!F4IsBadReadPtr(weapPtr, sizeof(SimWeaponClass)) && !weapPtr.drawPointer) // JB 010222 CTD
                    {
                        weapPtr = weapPtr.GetNextOnRail();
                    }

                    if (F4IsBadReadPtr(weapPtr, sizeof(SimWeaponClass))) // JB 010222 CTD
                        return; // JB 010222 CTD

                    drawPtr = hardPoint[hp].GetRack();
                    slot = weapPtr.GetRackSlot();
                    // 2002-03-27 MN Catch the "AdvancedWeaponStation" crash
                    Debug.Assert(slot < hardPoint[hp].NumPoints());
                    if (slot >= hardPoint[hp].NumPoints())
                        slot = hardPoint[hp].NumPoints() - 1;

                }
                else
                {
                    drawPtr = (DrawableBSP*)ownship.drawPointer;
                    slot = hp - 1;
                }

                if (weapPtr.IsMissile())
                {
                    ((MissileClass*)weapPtr).SetTarget(null);
                    ((MissileClass*)weapPtr).ClearReferences();
                }
                else if (weapPtr.IsBomb())
                {
                    ((BombClass*)weapPtr).SetTarget(null);
                }

                RemoveStore(hp, hardPoint[hp].weaponId);

                if (weapPtr.drawPointer)
                {
                    OTWDriver.DetachObject(drawPtr, (DrawableBSP*)(weapPtr.drawPointer), slot);
                    OTWDriver.RemoveObject(weapPtr.drawPointer, true);
                    weapPtr.drawPointer = null;
                }
            }
        }

        public void AGJettison()
        {
            int curStation;

            for (curStation = numHardpoints - 1; curStation > 0; curStation--)
            {
                if (hardPoint[curStation] && hardPoint[curStation].GetRack() && (hardPoint[curStation].Domain() & wdGround))
                {
                    //			MonoPrint ("Jettison station %d at %ld\n", curStation, SimLibElapsedTime);
                    ReleaseCurWeapon(-1);
                    JettisonStation(curStation);
                }
            }

            // Set a permanent flag indicating that we've done the deed
            flags |= EmergencyJettisonFlag;
        }

        public int DidEmergencyJettison() { return flags & EmergencyJettisonFlag; }
        public int DidJettisonedTank() { return flags & TankJettisonFlag; } // 2002-02-20 ADDED BY S.G. Helper to know if our tanks where jettisoned
        public void TankJettison() // 2002-02-20 ADDED BY S.G. Will jettison the tanks (if empty) and set TankJettisonFlag
        {
            int curStation;

            if (!(flags & TankJettisonFlag) && ownship.IsAirplane() && ((AircraftClass*)ownship).af.ExternalFuel() < 0.1f)
            { // We're an airplane and our external fuel is almost zero (to trap floating point precision error), then jettison the tanks
                for (curStation = numHardpoints - 1; curStation > 0; curStation--)
                {
                    if (hardPoint[curStation] && hardPoint[curStation].GetWeaponClass() == wcTank)
                    {
                        //				MonoPrint ("Jettison station %d at %ld\n", curStation, SimLibElapsedTime);
                        JettisonStation(curStation);
                    }
                }

                // Set a permanent flag indicating that we've done the deed
                flags |= TankJettisonFlag;
            }
        }

        public void WeaponStep(int symFlag = false)
        {
            int i, idDesired;
            int stationUnderTest;
            WeaponClass classDesired;
            WeaponType typeDesired;
            SimWeaponClass weapPtr, found;

            if (curHardpoint < 0)
            {
                // Do nothing if no station is currently selected
                return;
            }

            // Symetric or same?
            if (!symFlag)
            {
                stationUnderTest = curHardpoint;
                i = curWpnNum;
            }
            else
            {
                stationUnderTest = numHardpoints - 1;
                i = -1;
            }

            // Otherwise, start with next weapon on this hardpoint
            ReleaseCurWeapon(curHardpoint);

            // KCK: Why are we returning here? I guess we can't step to our next gun for
            // multiple gun vehicles...
            if (!hardPoint[curHardpoint] || hardPoint[curHardpoint].GetWeaponClass() == wcGunWpn)
                return;

            classDesired = hardPoint[curHardpoint].GetWeaponClass();
            typeDesired = hardPoint[curHardpoint].GetWeaponType();
            idDesired = hardPoint[curHardpoint].weaponId;

            // First try and find next weapon on the current hardpoint
            if (i >= 0)
            {
                weapPtr = hardPoint[curHardpoint].weaponPointer;
                while (weapPtr)
                {
                    if (weapPtr.GetRackSlot() > i)
                    {
                        found = weapPtr;
                        break;
                    }
                    weapPtr = weapPtr.GetNextOnRail();
                }
            }

            // Next look for anything w/ the same weapon Id
            if (!found)
            {
                if (!symFlag)
                {
                    for (i = 0; i < numHardpoints; i++)
                    {
                        stationUnderTest = (i + 1 + curHardpoint) % numHardpoints;
                        if (hardPoint[stationUnderTest] && hardPoint[stationUnderTest].weaponId == idDesired)
                        {
                            if (hardPoint[stationUnderTest].weaponPointer)
                            {
                                found = hardPoint[stationUnderTest].weaponPointer;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (curHardpoint > numHardpoints / 2)
                    {
                        for (i = 0; i < numHardpoints; i++)
                        {
                            stationUnderTest = i;
                            if (hardPoint[stationUnderTest] && hardPoint[stationUnderTest].weaponId == idDesired)
                            {
                                if (hardPoint[stationUnderTest].weaponPointer)
                                {
                                    found = hardPoint[stationUnderTest].weaponPointer;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        for (i = numHardpoints - 1; i >= 0; i--)
                        {
                            stationUnderTest = i;
                            if (hardPoint[stationUnderTest] && hardPoint[stationUnderTest].weaponId == idDesired)
                            {
                                if (hardPoint[stationUnderTest].weaponPointer)
                                {
                                    found = hardPoint[stationUnderTest].weaponPointer;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            // Next try and find first weapon of the same class on any other hardpoint
            if (!found && classDesired != wcGbuWpn)
            {
                for (i = 0; i < numHardpoints; i++)
                {
                    stationUnderTest = (i + 1 + curHardpoint) % numHardpoints;
                    if (hardPoint[stationUnderTest] && hardPoint[stationUnderTest].GetWeaponClass() == classDesired)
                    {
                        if (hardPoint[stationUnderTest].GetWeaponType() == typeDesired && hardPoint[stationUnderTest].weaponPointer)
                        {
                            found = hardPoint[stationUnderTest].weaponPointer;
                            break;
                        }
                    }
                }
            }

            if (found || classDesired == wcHARMWpn) // JB 010726 Stick with the HARM
                SetCurrentWeapon(stationUnderTest, found);
            else
                SetCurrentWeapon(-1, null);
        }

        public int FindWeapon(int indexDesired)
        {
            int i = 0;
            int stationUnderTest = 0;
            SimWeaponClass weapPtr = null, found = null;

            if (curHardpoint >= 0)
            {
                ReleaseCurWeapon(curHardpoint);
                if (hardPoint[curHardpoint].weaponPointer && hardPoint[curHardpoint].weaponPointer.Type() == indexDesired)
                {
                    // Try and get the next weapon on current hardpoint
                    weapPtr = hardPoint[curHardpoint].weaponPointer.GetNextOnRail();
                    if (weapPtr)
                    {
                        stationUnderTest = curHardpoint;
                        found = weapPtr;
                    }
                }
            }

            // Try and get the first weapon on any other hardpoint
            if (!found)
            {
                for (i = 0; i < numHardpoints; i++)
                {
                    stationUnderTest = (i + 1 + curHardpoint) % numHardpoints;
                    if (hardPoint[stationUnderTest] && hardPoint[stationUnderTest].weaponPointer && hardPoint[stationUnderTest].weaponPointer.Type() == indexDesired)
                    {
                        found = hardPoint[stationUnderTest].weaponPointer;
                        break;
                    }
                }
            }

            if (found)
                SetCurrentWeapon(stationUnderTest, found);
            else
                SetCurrentWeapon(-1, null);

            if (found)
                return 1;
            return 0;
        }

        public int FindWeaponClass(WeaponClass weaponDesired, int needWeapon = true)
        {
            int i, retval;
            int stationUnderTest;
            SimWeaponClass weapPtr, found;
            int notNeedStation = -1;
            int foundStation = -1;

            // Release the current weapon;
            if (curHardpoint >= 0)
            {
                ReleaseCurWeapon(curHardpoint);

                // Do we have the right thing on the current station?
                weapPtr = hardPoint[curHardpoint].weaponPointer;
                if (hardPoint[curHardpoint].GetWeaponClass() == weaponDesired && weapPtr)
                {
                    foundStation = curHardpoint;
                    found = weapPtr;
                }
            }

            // Try and get the first weapon on any other hardpoint
            if (!found)
            {
                for (i = 0; i < numHardpoints; i++)
                {
                    stationUnderTest = (i + 1 + curHardpoint) % numHardpoints;
                    if (hardPoint[stationUnderTest] && hardPoint[stationUnderTest].GetWeaponClass() == weaponDesired)
                    {
                        if (!found)
                        {
                            found = hardPoint[stationUnderTest].weaponPointer;
                            foundStation = stationUnderTest;
                        }
                        else if (notNeedStation < 0)
                        {
                            notNeedStation = stationUnderTest;
                        }
                    }
                }
            }

            // Report status
            if (found)
            {
                // Found one
                SetCurrentWeapon(foundStation, found);
                retval = true;
            }
            else if (!needWeapon && notNeedStation != -1)
            {
                // Found where one was, and thats good enough
                SetCurrentWeapon(notNeedStation, null);
                retval = true;
            }
            else
            {
                // Never had one.
                SetCurrentWeapon(-1, null);
                retval = false;
            }

            return retval;
        }

        public int FindWeaponType(WeaponType weaponDesired)
        {
            int i;
            int stationUnderTest;
            SimWeaponClass weapPtr, found;
            int foundStation = -1;
            int emptyStation = -1;

            if (curHardpoint >= 0)
            {
                ReleaseCurWeapon(curHardpoint);
                if (hardPoint[curHardpoint].GetWeaponType() == weaponDesired && hardPoint[curHardpoint].weaponPointer)
                {
                    // Try and get the next weapon on current hardpoint
                    weapPtr = hardPoint[curHardpoint].weaponPointer.GetNextOnRail();
                    if (weapPtr)
                    {
                        stationUnderTest = curHardpoint;
                        found = weapPtr;
                    }
                }
            }

            // Try and get the first weapon on any other hardpoint
            if (!found)
            {
                for (i = 0; i < numHardpoints; i++)
                {
                    stationUnderTest = (i + 1 + curHardpoint) % numHardpoints;
                    if (hardPoint[stationUnderTest] && hardPoint[stationUnderTest].GetWeaponType() == weaponDesired)
                    {
                        if (!found)
                        {
                            found = hardPoint[stationUnderTest].weaponPointer;
                            foundStation = stationUnderTest;
                        }
                        else if (emptyStation == -1)
                        {
                            emptyStation = stationUnderTest;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            if (found)
                SetCurrentWeapon(foundStation, found);
            else if (emptyStation >= 0)
                SetCurrentWeapon(emptyStation, null);
            else
                SetCurrentWeapon(-1, null);

            if (found)
                return 1;
            return 0;
        }

        public int DropBomb(int allowRipple = true)
        {
            BombClass* theBomb;
            SimBaseClass* lastWeapon;
            int retval = FALSE;
            vector pos, posDelta;
            float initXloc, initYloc, initZloc;
            int matchingStation, i;
            VehicleClassDataType* vc;
            int visFlag;
            float dragCoeff = 0.0f;
            int slotId;


            // Check for SMS Failure or other reason not to drop
            if (!CurStationOK() || //Weapon Station failure
                 ownship.OnGround() || // Weight on wheels inhibit
                !curWeapon || // No Weapon
                ownship.GetNz() < 0.0F || // Negative Gs
                MasterArm() != MasterArmState.Arm) // Not in arm mode
            {
                ownship.GetFCC().bombPickle = FALSE;
                return retval;
            }

            // edg: there was a problem.  Some ground vehicles do NOT have guns
            // at all.  All this code assumes a gun in Slot 0 and therefore subtracts
            // 1 from curhardpoint to get the right slot.  Don't do this if there
            // are no guns on board.
            if (IsSet(GunOnBoard))
                slotId = max(0, curHardpoint - 1);
            else
                slotId = curHardpoint;

            // Verify curWeapon on curHardpoint
            F4Assert(curHardpoint >= 0);
            if (curHardpoint < 0 || hardPoint[curHardpoint].weaponPointer != curWeapon)
            {
                for (i = 0; i < numHardpoints; i++)
                {
                    if (hardPoint[i].weaponPointer == curWeapon)
                    {
                        MonoPrint("Adjusting hardpoint to %d\n", i);
                        curHardpoint = i;
                        if (IsSet(GunOnBoard))
                            slotId = max(0, curHardpoint - 1);
                        else
                            slotId = curHardpoint;
                        break;
                    }
                }
            }

            if (curWeapon && fabs(ownship.Roll()) < 60.0F * DTR)
            {
                F4Assert(curWeapon.IsBomb());

                dragCoeff = hardPoint[curHardpoint].GetWeaponData().cd;

                theBomb = (BombClass*)(curWeapon);
                // edg: it's been observed that theBomb will be NULL at times?!
                // leonr: This happens when you ask for too many release pulses.
                if (theBomb == NULL)
                    return retval;
                SetFlag(Firing);
                hardPoint[curHardpoint].GetSubPosition(curWpnNum, &initXloc, &initYloc, &initZloc);
                pos.x = ownship.XPos() + ownship.dmx[0][0] * initXloc + ownship.dmx[1][0] * initYloc + ownship.dmx[2][0] * initZloc;
                pos.y = ownship.YPos() + ownship.dmx[0][1] * initXloc + ownship.dmx[1][1] * initYloc + ownship.dmx[2][1] * initZloc;
                pos.z = ownship.ZPos() + ownship.dmx[0][2] * initXloc + ownship.dmx[1][2] * initYloc + ownship.dmx[2][2] * initZloc;
                posDelta.x = ownship.XDelta();
                posDelta.y = ownship.YDelta();
                posDelta.z = ownship.ZDelta();
                theBomb.SetBurstHeight(burstHeight);
                theBomb.Start(&pos, &posDelta, dragCoeff, (ownship ? ((DigitalBrain*)ownship.Brain() ? ((DigitalBrain*)ownship.Brain()).GetGroundTarget() : NULL) : NULL)); // 2002-02-26 MODIFIED BY S.G. Added the ownship... in case the target is aggregated and an AI is bombing it

                vc = GetVehicleClassData(ownship.Type() - VU_LAST_ENTITY_TYPE);
                visFlag = vc.VisibleFlags;

                if (visFlag & (1 << curHardpoint) && theBomb.drawPointer)
                {
                    // Detach visual from parent
                    if (hardPoint[curHardpoint].GetRack())
                    {
                        OTWDriver.DetachObject(hardPoint[curHardpoint].GetRack(), (DrawableBSP*)(theBomb.drawPointer), theBomb.GetRackSlot());
                    }
                    else if (ownship.drawPointer && curHardpoint && theBomb.drawPointer)
                    {
                        OTWDriver.DetachObject((DrawableBSP*)(ownship.drawPointer), (DrawableBSP*)(theBomb.drawPointer), slotId);
                    }
                }

                if (UnlimitedAmmo())
                {
                    ReplaceBomb(curHardpoint, theBomb);
                }
                else
                {
                    DecrementStores(hardPoint[curHardpoint].GetWeaponClass(), 1);
                    hardPoint[curHardpoint].weaponCount--;
                    if (hardPoint[curHardpoint].weaponCount >= hardPoint[curHardpoint].NumPoints())
                        ReplaceBomb(curHardpoint, theBomb);
                    else
                    {
                        DetachWeapon(curHardpoint, theBomb);

                        if (ownship.IsLocal())
                        {
                            FalconTrackMessage* trackMsg = new FalconTrackMessage(1, ownship.Id(), FalconLocalGame);
                            trackMsg.dataBlock.trackType = Track_RemoveWeapon;
                            trackMsg.dataBlock.hardpoint = curHardpoint;
                            trackMsg.dataBlock.id = ownship.Id();

                            // Send our track list
                            FalconSendMessage(trackMsg, TRUE);
                        }

                    }
                }

                RemoveStore(curHardpoint, hardPoint[curHardpoint].weaponId);

                // Make it live
                vuDatabase.Insert(theBomb);

                if (ownship == FalconLocalSession.GetPlayerEntity())
                    g_intellivibeData.BombDropped++;
                // Note: The drawable for this object has already been created!
                theBomb.Wake();
                // Record the drop
                ownship.SendFireMessage(theBomb, FalconWeaponsFire.BMB, TRUE, ownship.targetPtr);

                lastWeapon = curWeapon;
                matchingStation = curHardpoint;
                WeaponStep(TRUE);
                if (lastWeapon == curWeapon)
                {
                    curWeapon = NULL;
                }

                // Want to drop a pair - No pairs w/ only one rack of bombs unless from centerline
                if (pair && allowRipple && (curHardpoint != matchingStation || curHardpoint == (numHardpoints / 2 + 1)))
                {
                    theBomb = (BombClass*)(curWeapon);

                    if (theBomb)
                    {
                        hardPoint[curHardpoint].GetSubPosition(curWpnNum, &initXloc, &initYloc, &initZloc);
                        pos.x = ownship.XPos() + ownship.dmx[0][0] * initXloc + ownship.dmx[1][0] * initYloc + ownship.dmx[2][0] * initZloc;
                        pos.y = ownship.YPos() + ownship.dmx[0][1] * initXloc + ownship.dmx[1][1] * initYloc + ownship.dmx[2][1] * initZloc;
                        pos.z = ownship.ZPos() + ownship.dmx[0][2] * initXloc + ownship.dmx[1][2] * initYloc + ownship.dmx[2][2] * initZloc;
                        posDelta.x = ownship.XDelta();
                        posDelta.y = ownship.YDelta();
                        posDelta.z = ownship.ZDelta();
                        theBomb.SetBurstHeight(burstHeight);
                        theBomb.Start(&pos, &posDelta, dragCoeff, (ownship ? ((DigitalBrain*)ownship.Brain() ? ((DigitalBrain*)ownship.Brain()).GetGroundTarget() : NULL) : NULL)); // 2002-02-26 MODIFIED BY S.G. Added the ownship... in case the target is aggregated and an AI is bombing it

                        if (IsSet(GunOnBoard))
                            slotId = max(0, curHardpoint - 1);
                        else
                            slotId = curHardpoint;

                        // Detach visual from parent
                        if (hardPoint[curHardpoint].GetRack())
                        {
                            OTWDriver.DetachObject(hardPoint[curHardpoint].GetRack(), (DrawableBSP*)(theBomb.drawPointer), theBomb.GetRackSlot());
                        }
                        else if (ownship.drawPointer && theBomb.drawPointer && curHardpoint) // Keeps from detaching hidden (in bomb bay)
                        {
                            OTWDriver.DetachObject((DrawableBSP*)(ownship.drawPointer), (DrawableBSP*)(theBomb.drawPointer), slotId);
                        }

                        if (UnlimitedAmmo())
                        {
                            ReplaceBomb(curHardpoint, theBomb);
                        }
                        else
                        {
                            DecrementStores(hardPoint[curHardpoint].GetWeaponClass(), 1);
                            hardPoint[curHardpoint].weaponCount--;
                            if (hardPoint[curHardpoint].weaponCount >= hardPoint[curHardpoint].NumPoints())
                                ReplaceBomb(curHardpoint, theBomb);
                            else
                            {
                                DetachWeapon(curHardpoint, theBomb);

                                if (ownship.IsLocal())
                                {
                                    FalconTrackMessage* trackMsg = new FalconTrackMessage(1, ownship.Id(), FalconLocalGame);
                                    trackMsg.dataBlock.trackType = Track_RemoveWeapon;
                                    trackMsg.dataBlock.hardpoint = curHardpoint;
                                    trackMsg.dataBlock.id = ownship.Id();

                                    // Send our track list
                                    FalconSendMessage(trackMsg, TRUE);
                                }
                            }
                        }

                        RemoveStore(curHardpoint, hardPoint[curHardpoint].weaponId);

                        // Make it live
                        vuDatabase.QuickInsert(theBomb);
                        theBomb.Wake();

                        // Record the drop
                        ownship.SendFireMessage(theBomb, FalconWeaponsFire.BMB, TRUE, ownship.targetPtr);

                        // Step again
                        lastWeapon = curWeapon;
                        WeaponStep(TRUE);
                        if (lastWeapon == curWeapon)
                        {
                            curWeapon = NULL;
                        }
                    }
                }

                retval = TRUE;

                if (!curRippleCount)
                {
                    // Can we do a ripple drop?
                    if (allowRipple)
                        curRippleCount = max(min(rippleCount, numCurrentWpn - 1), 0);
                    // 2001-04-27 MODIFIED BY S.G. SO THE RIPPLE DISTANCE IS INCREASED WITH ALTITUDE (SINCE BOMBS SLOWS DOWN)
                    //         nextDrop = SimLibElapsedTime + FloatToInt32((rippleInterval)/(float)sqrt(ownship.XDelta()*ownship.XDelta() + ownship.YDelta()*ownship.YDelta()) * SEC_TO_MSEC + (float)sqrt(ownship.ZPos() * -4.0f));
                    // JB 011017 Use old nextDrop ripple code instead of altitude dependant code.  I don't see why bombs slowing down would have an effect on ripple distance
                    // If anything is effected it would cause bombs to fall short which is not happening.  
                    // Besides this really screws up the code that decides when to start the ripple, and all the bombs don't fall off properly.
                    nextDrop = SimLibElapsedTime + FloatToInt32((rippleInterval) /
                             (float)sqrt(ownship.XDelta() * ownship.XDelta() + ownship.YDelta() * ownship.YDelta()) * SEC_TO_MSEC);
                    if (!curRippleCount)
                    {
                        ClearFlag(Firing);
                    }
                }
                else
                {
                    if (curRippleCount == 1) // Last bomb, ripple count will be decremented in .Exec
                    {
                        ClearFlag(Firing);
                    }
                }

            }

            return (retval);
        }
        public int LaunchMissile()
        {
            MissileClass* theMissile;
            int retval = FALSE;
            SimBaseClass* lastWeapon;
            FireControlComputer* FCC = ownship.GetFCC();
            SimObjectType* tmpTargetPtr;
            int isCaged, wpnStation, wpnNum, isSpot, isSlave, isTD;
            float x, y, z, az, el;
            VehicleClassDataType* vc;
            int visFlag;

            if (!FCC)
            {
                ClearFlag(Firing);
                return retval;
            }

            // Check for SMS Failure or other reason not to launch
            if (!CurStationOK() ||
               !curWeapon ||
               Ownship().OnGround() ||
               MasterArm() != MasterArmState.Arm)
            {
                return retval;
            }


            tmpTargetPtr = FCC.TargetPtr();
            if (curWeapon)
            {
                F4Assert(curWeapon.IsMissile());
                SetFlag(Firing);
                theMissile = (MissileClass*)curWeapon;

                //MI only fire Mav's if they are powered
                //M.N. these conditionals were also true for AA-2's for example, added Type and SType check
                //always use all three checks, as the same values of stype and sptype are not unique!
                //M.N. don't need to uncage or power when in combat AP mode
                if (g_bRealisticAvionics)
                {
                    if ((curWeapon.parent &&
                    ((AircraftClass*)curWeapon.parent).IsPlayer() &&
                    !(((AircraftClass*)curWeapon.parent).AutopilotType() == AircraftClass.CombatAP) &&
                    !Powered) && curWeapon.GetType() == TYPE_MISSILE &&
                    curWeapon.GetSType() == STYPE_MISSILE_AIR_GROUND &&
                    (curWeapon.GetSPType() == SPTYPE_AGM65A ||
                    curWeapon.GetSPType() == SPTYPE_AGM65B ||
                    curWeapon.GetSPType() == SPTYPE_AGM65D ||
                    curWeapon.GetSPType() == SPTYPE_AGM65G))
                        return FALSE;
                }


                // JB 010109 CTD sanity check
                //if (theMissile.launchState == MissileClass.PreLaunch)
                if (theMissile.launchState == MissileClass.PreLaunch && curHardpoint != -1)
                // JB 010109 CTD sanity check
                {
                    // Set the missile position on the AC
                    wpnNum = min(hardPoint[curHardpoint].NumPoints() - 1, curWpnNum);
                    hardPoint[curHardpoint].GetSubPosition(wpnNum, &x, &y, &z);
                    hardPoint[curHardpoint].GetSubRotation(wpnNum, &az, &el);
                    theMissile.SetLaunchPosition(x, y, z);
                    theMissile.SetLaunchRotation(az, el);

                    // 2001-03-02 MOVED HERE BY S.G.
                    if (theMissile.sensorArray && theMissile.sensorArray[0].Type() == SensorClass.RadarHoming)
                    {
                        // Have the missile use the launcher's radar for guidance
                        ((BeamRiderClass*)theMissile.sensorArray[0]).SetGuidancePlatform(ownship);
                    }
                    // END OF MOVED SECTION

                    // Don't hand off ground targets to radar guided air to air missiles
                    if (curWeaponType != wtAim120 || (tmpTargetPtr && !tmpTargetPtr.BaseData().OnGround()))
                    {
                        theMissile.Start(tmpTargetPtr);
                    }
                    else
                    {
                        theMissile.Start(NULL);
                    }

                    /* 2001-03-02 MOVED BY S.G. ABOVE THE ABOVE IF. I NEED radarPlatform TO BE SET BEFORE I CAN CALL theMissile.Start SINCE I'LL CALL THE BeamRiderClass SendTrackMsg MESSAGE WHICH REQUIRES IT
                                // Need to give beam riders a pointer to the illuminating radar platform
                                if ( theMissile.sensorArray && theMissile.sensorArray[0].Type() == SensorClass.RadarHoming )
                                {
                                    // Have the missile use the launcher's radar for guidance
                                    ((BeamRiderClass*)theMissile.sensorArray[0]).SetGuidancePlatform( ownship );
                                }
                    */

                    // 2001-04-14 MN moved here from AircraftClass.DoWeapons - play bomb drop sound when flag is set and we are an AG missile
                    if (theMissile && theMissile.parent && FCC && (FCC.GetMasterMode() == FireControlComputer.AirGroundMissile ||
                                FCC.GetMasterMode() == FireControlComputer.AirGroundHARM))
                    {
                        if (g_nMissileFix & 0x80)
                        {
                            Falcon4EntityClassType* classPtr;
                            classPtr = (Falcon4EntityClassType*)theMissile.EntityType();
                            WeaponClassDataType* wc = NULL;

                            if (classPtr)
                                wc = (WeaponClassDataType*)classPtr.dataPtr; // this is important

                            if (wc && (wc.Flags & WEAP_BOMBDROPSOUND))	// for JSOW, JDAM...
                                F4SoundFXSetPos(SFX_BOMBDROP, TRUE, theMissile.parent.XPos(), theMissile.parent.YPos(), theMissile.parent.ZPos(), 1.0f);
                            else
                                F4SoundFXSetPos(SFX_MISSILE2, TRUE, theMissile.parent.XPos(), theMissile.parent.YPos(), theMissile.parent.ZPos(), 1.0f);
                        }
                        else
                            F4SoundFXSetPos(SFX_MISSILE2, TRUE, theMissile.parent.XPos(), theMissile.parent.YPos(), theMissile.parent.ZPos(), 1.0f);
                    }
                    else if (theMissile && !theMissile.parent)
                        F4SoundFXSetPos(SFX_MISSILE2, TRUE, theMissile.XPos(), theMissile.YPos(), theMissile.ZPos(), 1.0f);



                    // Leonr moved here to avoid per frame database search
                    // edg: moved insertion into vudatabse here.  I need
                    // the missile to be retrievable from the db when
                    // processing the weapon fire message
                    vuDatabase.QuickInsert(theMissile);
                    theMissile.Wake();

                    FCC.lastMissileImpactTime = FCC.nextMissileImpactTime;

                    lastWeapon = curWeapon;
                    isCaged = theMissile.isCaged;
                    isSpot = theMissile.isSpot;	// Marco Edit - SPOT/SCAN Support
                    isSlave = theMissile.isSlave;	// Marco Edit - BORE/SLAVE Support
                    isTD = theMissile.isTD;		// Marco Edit - TD/BP Support

                    wpnStation = curHardpoint;
                    wpnNum = curWpnNum;
                    WeaponStep(TRUE);
                    if (lastWeapon == curWeapon)
                    {
                        SetCurrentWeapon(-1, NULL);
                    }
                    else
                    {
                        //MI if we launch one uncaged, the next one's caged again!
                        if (curWeapon)
                        {
                            ((MissileClass*)curWeapon).isCaged = TRUE;
                            ((MissileClass*)curWeapon).isSpot = isSpot;
                            ((MissileClass*)curWeapon).isSlave = isSlave;
                            ((MissileClass*)curWeapon).isTD = isTD;

                        }
                    }
                    retval = TRUE;

                    vc = GetVehicleClassData(ownship.Type() - VU_LAST_ENTITY_TYPE);
                    visFlag = vc.VisibleFlags;

                    if (visFlag & (1 << curHardpoint) && theMissile.drawPointer)
                    {
                        // Detach visual from parent
                        if (hardPoint[wpnStation].GetRack())
                        {
                            OTWDriver.DetachObject(hardPoint[wpnStation].GetRack(), (DrawableBSP*)(theMissile.drawPointer), theMissile.GetRackSlot());
                        }
                        else if (ownship.drawPointer && wpnStation)
                        {
                            OTWDriver.DetachObject((DrawableBSP*)(ownship.drawPointer), (DrawableBSP*)(theMissile.drawPointer), wpnStation - 1);
                        }
                    }

                    if (UnlimitedAmmo())
                        ReplaceMissile(wpnStation, theMissile);
                    else
                    {
                        DecrementStores(hardPoint[wpnStation].GetWeaponClass(), 1);
                        RemoveStore(wpnStation, hardPoint[wpnStation].weaponId);
                        hardPoint[wpnStation].weaponCount--;
                        if (hardPoint[wpnStation].weaponCount >= hardPoint[wpnStation].NumPoints())
                        {
                            ReplaceMissile(wpnStation, theMissile);
                        }
                        else
                        {
                            DetachWeapon(wpnStation, theMissile);

                            if (ownship.IsLocal())
                            {
                                FalconTrackMessage* trackMsg = new FalconTrackMessage(1, ownship.Id(), FalconLocalGame);
                                trackMsg.dataBlock.trackType = Track_RemoveWeapon;
                                trackMsg.dataBlock.hardpoint = wpnStation;
                                trackMsg.dataBlock.id = ownship.Id();

                                // Send our track list
                                FalconSendMessage(trackMsg, TRUE);
                            }
                        }
                    }
                }

                ClearFlag(Firing);
            }

            //MI SOI after firing Mav
            if (g_bRealisticAvionics)
            {
                if (curWeapon && curWeapon.parent &&
                ((AircraftClass*)curWeapon.parent).IsPlayer() &&
                (curWeapon.GetSPType() == SPTYPE_AGM65A ||
                curWeapon.GetSPType() == SPTYPE_AGM65B ||
                curWeapon.GetSPType() == SPTYPE_AGM65D ||
                curWeapon.GetSPType() == SPTYPE_AGM65G))
                {
                    if (SimDriver.playerEntity)
                    {
                        SimDriver.playerEntity.SOIManager(SimVehicleClass.SOI_WEAPON);
                    }
                }
            }

            return (retval);
        }
        static int count = 0;
        public int LaunchRocket()
        {
            MissileClass* theMissile;
            int retval = FALSE;
            SimBaseClass* lastWeapon;
            FireControlComputer* FCC = ownship.GetFCC();
            SimObjectType* tmpTargetPtr;
            int wpnNum;

            float az, el, x, y, z;

            if (!FCC)
            {
                ClearFlag(Firing);
                return retval;
            }

            // Check for SMS Failure
            if (!CurStationOK() ||
                !curWeapon ||
                 Ownship().OnGround() ||
                 MasterArm() != MasterArmState.Arm)
            {
                return retval;
            }

            tmpTargetPtr = FCC.TargetPtr();
            Debug.Assert(hardPoint[curHardpoint].weaponPointer.IsMissile());
            theMissile = (MissileClass*)(hardPoint[curHardpoint].weaponPointer);

            if (theMissile && count == 0)
            {
                if (hardPoint[curHardpoint].GetRack())
                    hardPoint[curHardpoint].GetRack().SetSwitchMask(0, 0);
                SetFlag(Firing);
                count = 2;
                wpnNum = min(hardPoint[curHardpoint].NumPoints() - 1, curWpnNum);
                hardPoint[curHardpoint].GetSubPosition(wpnNum, &x, &y, &z);
                hardPoint[curHardpoint].GetSubRotation(wpnNum, &az, &el);
                theMissile.SetLaunchPosition(x, y, z);
                theMissile.SetLaunchRotation(az, el);
                if (tmpTargetPtr)
                {
                    theMissile.Start(tmpTargetPtr);
                }
                else
                {
                    theMissile.Start(NULL);
                }

                // Leonr moved here to avoid per frame database search
                // edg: moved insertion into vudatabse here.  I need
                // the missile to be retrievable from the db when
                // processing the weapon fire message
                vuDatabase.QuickInsert(theMissile);
                // KCK: WARNING: these things already have drawables BEFORE wake is called.
                // Make Wake() not assign new drawables if one exists?
                theMissile.Wake();

                //		MonoPrint ("Inserting rocket %p\n", theMissile);
                FCC.lastMissileImpactTime = FCC.nextMissileImpactTime;

                // Remove it from the parent
                hardPoint[curHardpoint].weaponCount--;
                DetachWeapon(curHardpoint, theMissile);

                curWeapon = hardPoint[curHardpoint].weaponPointer;
                // Only the last rocket has a nametag
                if (hardPoint[curHardpoint].weaponPointer)
                    theMissile.drawPointer.SetLabel("", 0xff00ff00);
                // End TYPE_ROCKET section
            }
            count--;

            if (hardPoint[curHardpoint].weaponCount == 0)
            {
                retval = TRUE;
                wpnNum = curHardpoint;

                // Reload if possible / wanted
                if (theMissile && UnlimitedAmmo())
                    ReplaceRocket(wpnNum, theMissile);

                lastWeapon = curWeapon;
                WeaponStep(TRUE);
                if (lastWeapon == curWeapon)
                {
                    ReleaseCurWeapon(-1);
                }
                ClearFlag(Firing);
            }
            return (retval);
        }

        public void SetUnlimitedGuns(int flag)
        {
            int i;

            for (i = 0; i < numHardpoints; i++)
            {
                if (hardPoint[i] && hardPoint[i].GetGun())
                {
                    hardPoint[i].GetGun().unlimitedAmmo = flag;
                }
            }
        }

        public int UnlimitedAmmo() { return flags & UnlimitedAmmoFlag; }
        public void SetUnlimitedAmmo(int newFlag)
        {
            if (newFlag)
                flags |= UnlimitedAmmoFlag;
            else
                flags &= ~UnlimitedAmmoFlag;
            SetUnlimitedGuns(newFlag);
        }

        public int HasHarm() { return (flags & HTSOnBoard ? true : false); }
        public int HasLGB() { return (flags & LGBOnBoard ? true : false); }
        public int HasTrainable()
        {
            int i;
            int retval = false;

            for (i = 0; i < numHardpoints; i++)
            {
                if (hardPoint[i] && hardPoint[i].GetWeaponData().flags & SMSClass.Trainable)
                {
                    retval = true;
                    break;
                }
            }

            return retval;
        }

        public int HasSPJammer() { return (flags & SPJamOnBoard ? true : false); }
        public int HasWeaponClass(WeaponClass classDesired)
        {
            int i;

            for (i = 0; i < numHardpoints; i++)
            {
                if (hardPoint[i] && hardPoint[i].GetWeaponClass() == classDesired && hardPoint[i].weaponPointer)
                {
                    break;
                }
            }

            return (i < numHardpoints ? true : false);
        }

        public void FreeWeapons()
        {
            int i;

            // KCK: The actual deletion of this stuff is done in the base class destructor
            ReleaseCurWeapon(-1);
            for (i = 0; i < numHardpoints; i++)
                if (hardPoint[i])
                    hardPoint[i].SetGun(null);
        }
        public void Exec()
        {
            int i;
            GunClass* gun;

            // 2002-01-27 ADDED BY S.G. Reset curRippleCount if the current weapon is NOT a bomb...
            if (curRippleCount && curWeapon && !curWeapon.IsBomb())
            {
                curRippleCount = 0;
                nextDrop = 0;
            }
            // END OF ADDED SECTION

            // Do ripple stuff here
            if (curRippleCount && SimLibElapsedTime > nextDrop)
            {
                DropBomb();

                curRippleCount--;

                if (curRippleCount)
                    // 2001-04-27 MODIFIED BY S.G. SO THE RIPPLE DISTANCE IS INCREASED WITH ALTITUDE (SINCE BOMBS SLOWS DOWN)
                    //         nextDrop = SimLibElapsedTime + FloatToInt32((rippleInterval)/(float)sqrt(ownship.XDelta()*ownship.XDelta() + ownship.YDelta()*ownship.YDelta()) * SEC_TO_MSEC + (float)sqrt(ownship.ZPos() * -4.0f));
                    // JB 011017 Use old nextDrop ripple code instead of altitude dependant code.  I don't see why bombs slowing down would have an effect on ripple distance
                    // If anything is effected it would cause bombs to fall short which is not happening.  
                    // Besides this really screws up the code that decides when to start the ripple, and all the bombs don't fall off properly.
                    nextDrop = SimLibElapsedTime + FloatToInt32((rippleInterval) /
                             (float)sqrt(ownship.XDelta() * ownship.XDelta() + ownship.YDelta() * ownship.YDelta()) * SEC_TO_MSEC);
                else
                {
                    nextDrop = 0;

                    // Make sure we can fire in the future
                    ClearFlag(Firing);
                }

            }
            //MI Mav cooling
            if (g_bRealisticAvionics)
            {
                if (Powered && MavCoolTimer >= -1.0F)
                    MavCoolTimer -= SimLibMajorFrameTime;
                else if (!Powered && MavCoolTimer <= 5.0F)
                    MavCoolTimer += SimLibMajorFrameTime;
            }

            // Marco Edit - Check overall cooling amount left
            if (GetCoolState() == COOLING)
            { // JPO aim9 is cooling down
                aim9cooltime -= SimLibMajorFrameTime;
                if (aim9cooltime <= 0.0F)
                {
                    SetCoolState(COOL);
                    aim9cooltime = 0.0F;
                }
            }
            if (GetCoolState() == COOL || GetCoolState() == COOLING)
            {
                aim9coolingtimeleft -= SimLibMajorFrameTime;
                aim9warmtime -= SimLibMajorFrameTime;
                if (aim9warmtime <= 0.0F)
                    aim9warmtime = 0.0F;
            }

            // Marco Edit - Warming up back to normal
            if (GetCoolState() == WARMING)
            {
                aim9warmtime += SimLibMajorFrameTime;
                aim9cooltime += SimLibMajorFrameTime;	//reset our cooling timer.

                if (aim9cooltime > 3.0F)
                    aim9cooltime = 3.0F;

                if (aim9warmtime >= 60.0F)
                {
                    SetCoolState(WARM);
                    aim9cooltime = 3.0F;	//reset our cooling timer.
                }
            }

            // We're out of coolant! We start to warm up....
            if (aim9coolingtimeleft <= 0 && GetCoolState() != WARM)
                SetCoolState(WARMING);

            // Okay, we've warmed up enough....
            // can you say dead weight???
            /*else if(aim9coolingtimeleft == 0 && SimLibElapsedTime > aim9warmtime)
            {
                SetCoolState(WARM);
                aim9warmtime = 0;
                aim9cooltime = 0;
            }*/

            if (drawable)
            {
                drawable.UpdateGroundSpot();
                if (ownship && // JB 010710 CTD?
                          !ownship.OnGround())
                {
                    drawable.frameCount += FloatToInt32(SimLibMajorFrameTime * SEC_TO_MSEC * 0.1F);
                }
            }

            // Count how many of these we have
            if (curHardpoint >= 0)
            {
                numCurrentWpn = 0;
                if (curWeaponClass == wcGunWpn)
                {
                    gun = GetGun(curHardpoint);
                    if (gun)
                        numCurrentWpn = gun.numRoundsRemaining / 10;
                }
                else
                {
                    for (i = 0; i < numHardpoints; i++)
                    {
                        if (hardPoint[i] && hardPoint[i].weaponId == curWeaponId)
                        {
                            numCurrentWpn += hardPoint[i].weaponCount;
                        }
                    }
                }
            }
            else
            {
                numCurrentWpn = -1;
            }
        }
        public void SetPlayerSMS(int flag)
        {
            if (flag && !drawable)
            {
                drawable = new SmsDrawable(this);
            }
            else if (!flag)
            {
                delete drawable;
                drawable = null;
            }
        }
        public void SetPair(int flag)
        {
            if (curWeaponClass == wcBombWpn)
            {
                pair = flag;
            }
            //MI
            if (g_bRealisticAvionics)
            {
                if (curWeaponClass == wcGbuWpn)
                    pair = flag;
                if (Prof1)
                    Prof1Pair = pair ? 1 : 0;
                else
                    Prof2Pair = pair ? 1 : 0;
            }
        }

        public void IncrementRippleCount()
        {
            if (curWeaponClass == wcBombWpn)
            {
                // 2000-08-31 ADDED BY S.G. SO BOMBER HAVE A HIGHER RIPPLE COUNT AVAILABLE
                if (ownship.GetSType() == STYPE_AIR_BOMBER)
                    rippleCount = (rippleCount + 1) % MAX_RIPPLE_COUNT_BOMBER;
                else
                    // END OF ADDED SECTION
                    rippleCount = (rippleCount + 1) % MAX_RIPPLE_COUNT;
            }
            //MI
            if (g_bRealisticAvionics)
            {
                if (curWeaponClass == wcGbuWpn)
                {
                    // 2000-08-31 ADDED BY S.G. SO BOMBER HAVE A HIGHER RIPPLE COUNT AVAILABLE
                    if (ownship.GetSType() == STYPE_AIR_BOMBER)
                        rippleCount = (rippleCount + 1) % MAX_RIPPLE_COUNT_BOMBER;
                    else
                        // END OF ADDED SECTION
                        rippleCount = (rippleCount + 1) % MAX_RIPPLE_COUNT;
                }

                if (Prof1)
                    Prof1RP = rippleCount;
                else
                    Prof2RP = rippleCount;
            }
        }

        public void DecrementRippleCount()
        {
            if (curWeaponClass == wcBombWpn)
            {
                rippleCount--;
                if (rippleCount < 0)
                    // 2000-08-31 ADDED BY S.G. SO BOMBER HAVE A HIGHER RIPPLE COUNT AVAILABLE
                    if (ownship.GetSType() == STYPE_AIR_BOMBER)
                        rippleCount = MAX_RIPPLE_COUNT_BOMBER - 1;
                    else
                        // END OF ADDED SECTION
                        rippleCount = MAX_RIPPLE_COUNT - 1;
            }
            //MI
            if (g_bRealisticAvionics)
            {
                if (curWeaponClass == wcGbuWpn)
                {
                    rippleCount--;
                    if (rippleCount < 0)
                        // 2000-08-31 ADDED BY S.G. SO BOMBER HAVE A HIGHER RIPPLE COUNT AVAILABLE
                        if (ownship.GetSType() == STYPE_AIR_BOMBER)
                            rippleCount = MAX_RIPPLE_COUNT_BOMBER - 1;
                        else
                            // END OF ADDED SECTION
                            rippleCount = MAX_RIPPLE_COUNT - 1;
                }
                if (Prof1)
                    Prof1RP = rippleCount;
                else
                    Prof2RP = rippleCount;
            }
        }

        public void SetRippleInterval(int rippledistance) // Marco Edit - for AI A2G
        {
            if (curWeaponClass == wcBombWpn)
            {
                rippleInterval = rippledistance;
            }
            if (rippleInterval > 175)
            {
                rippleInterval = 175;
            }
            if (rippleInterval < 0)
            {
                rippleInterval = 0;
            }
        }

        public void IncrementRippleInterval()
        {
            if (curWeaponClass == wcBombWpn)
            {
                rippleInterval = (rippleInterval + 50) % 200;
            }
            //MI
            if (g_bRealisticAvionics)
            {
                if (curWeaponClass == wcGbuWpn)
                {
                    rippleInterval = (rippleInterval + 50) % 200;
                }
                if (Prof1)
                    Prof1RS = rippleInterval;
                else
                    Prof2RS = rippleInterval;
            }
        }

        public void DecrementRippleInterval()
        {
            if (curWeaponClass == wcBombWpn)
            {
                rippleInterval -= 50;
                if (rippleInterval < 0)
                    rippleInterval = 175;
            }
            //MI
            if (g_bRealisticAvionics)
            {
                if (curWeaponClass == wcGbuWpn)
                {
                    rippleInterval -= 50;
                    if (rippleInterval < 0)
                        rippleInterval = 175;
                }
                if (Prof1)
                    Prof1RS = rippleInterval;
                else
                    Prof2RS = rippleInterval;
            }
        }

        public void IncrementBurstHeight()
        {
            if (curHardpoint >= 0 && hardPoint[curHardpoint].GetWeaponData().flags & SMSClass.HasBurstHeight)
            {
                if (burstHeight < 900)
                    burstHeight += 200;
                else if (burstHeight < 1800)
                    burstHeight += 300;
                else if (burstHeight < 3000)
                    burstHeight += 400;
                else
                    burstHeight = 300;
            }
            //MI
            if (g_bRealisticAvionics)
            {
                C2BA = burstHeight;
            }

        }

        public void DecrementBurstHeight()
        {
            if (curHardpoint >= 0 && hardPoint[curHardpoint].GetWeaponData().flags & SMSClass.HasBurstHeight)
            {
                if (burstHeight > 1800)
                    burstHeight -= 400;
                else if (burstHeight > 900)
                    burstHeight -= 300;
                else if (burstHeight > 300)
                    burstHeight -= 200;
                else
                    burstHeight = 3000;
            }
        }

        public void Incrementarmingdelay()//me123 status test. addet
        {
            if (armingdelay >= 0)
            {
                if (armingdelay < 300)
                    armingdelay += 20;
                else if (armingdelay < 500)
                    armingdelay += 30;
                else if (armingdelay < 1000)
                    armingdelay += 40;
                else
                    armingdelay = 75;
            }
        }

        public void ResetCurrentWeapon();
        public void SetRippleCount(int newVal) { rippleCount = newVal; }
        public int RippleCount() { return rippleCount; }
        public int CurRippleCount() { return curRippleCount; } // JB 010708
        public int RippleInterval() { return rippleInterval; }
        public int Pair() { return pair; }
        public WeaponType GetNextWeapon(WeaponDomain domainDesired)
        {
            WeaponType newType = curWeaponType;
            FireControlComputer* FCC = ownship.GetFCC();
            int i = 0;
            int stationUnderTest = 0;

            // Don't do this if we're in the process of firing a weapon
            if (IsSet(Firing))
            {
                return curWeaponType;
            }

            // Find the next hardpoint with a weapon of the desired type on it
            for (i = 0; i < numHardpoints; i++)
            {
                stationUnderTest = (i + 1 + curHardpoint) % numHardpoints;
                // Marco edit - non-zero weapon check due to problems with weapon cycling
                if (hardPoint[stationUnderTest] && (hardPoint[stationUnderTest].GetWeaponData().domain & domainDesired) &&
                    (hardPoint[stationUnderTest].weaponCount != 0 ||
                    hardPoint[stationUnderTest].GetWeaponType() == wtGuns
                    || hardPoint[stationUnderTest].GetWeaponType() == wtAgm88
                    || hardPoint[stationUnderTest].GetWeaponType() == wtGBU)) // JB 010726 Allow HTS/LaserPod to be selected even when out of weapons
                {
                    newType = hardPoint[stationUnderTest].GetWeaponType();

                    // Set the current hardpoint so that this weapon is actually used
                    //MI 
                    if (g_bRealisticAvionics)
                    {
                        if (newType == wtAgm65 && curWeaponType != wtAgm65)
                            StepMavSubMode(true);
                    }
                    ReleaseCurWeapon(stationUnderTest);
                    SetCurrentWeapon(stationUnderTest, hardPoint[stationUnderTest].weaponPointer);
                    break;
                }
            }
            SelectWeapon(newType, domainDesired);
            //MI
            if (g_bRealisticAvionics && domainDesired == wdGround)
            {
                RadarDopplerClass* pradar = (RadarDopplerClass*)FindSensor(ownship, SensorClass.Radar);
                if (pradar)
                    pradar.SetScanDir(1.0F);
            }
            return (newType);
        }

        public WeaponType GetNextWeaponSpecific(WeaponDomain domainDesired)
        {
            WeaponType newType = curWeaponType;
            FireControlComputer* FCC = ownship.GetFCC();
            int i = 0;
            int stationUnderTest = 0;

            // Don't do this if we're in the process of firing a weapon
            if (IsSet(Firing))
            {
                return curWeaponType;
            }

            // Find the next hardpoint with a weapon of the desired type on it
            for (i = 0; i < numHardpoints; i++)
            {
                stationUnderTest = (i + 1 + curHardpoint) % numHardpoints;
                if (hardPoint[stationUnderTest] && (hardPoint[stationUnderTest].GetWeaponData().domain == domainDesired))
                {
                    newType = hardPoint[stationUnderTest].GetWeaponType();

                    // Set the current hardpoint so that this weapon is actually used
                    ReleaseCurWeapon(stationUnderTest);
                    SetCurrentWeapon(stationUnderTest, hardPoint[stationUnderTest].weaponPointer);
                    break;
                }
            }
            SelectWeapon(newType, domainDesired);
            return (newType);
        }
        public void SelectWeapon(WeaponType ntype, WeaponDomain domainDesired)
        {
            FireControlComputer* FCC = ownship.GetFCC();
            // Tell the FCC about the new weapon selection
            switch (newtype)
            {
                case wtNone:
                    FCC.SetMasterMode(FireControlComputer.Nav);
                    break;

                case wtAim9:
                    FCC.SetMasterMode(FireControlComputer.Missile);
                    FCC.SetSubMode(FireControlComputer.Aim9);
                    break;

                case wtAim120:
                    FCC.SetMasterMode(FireControlComputer.Missile);
                    FCC.SetSubMode(FireControlComputer.Aim120);
                    break;

                case wtGuns:
                    if (domainDesired == wdAir)
                    {
                        FCC.SetMasterMode(FireControlComputer.Gun);
                        FCC.SetSubMode(FireControlComputer.EEGS);
                    }
                    else
                    {
                        FCC.SetMasterMode(FireControlComputer.Gun);
                        FCC.SetSubMode(FireControlComputer.STRAF);
                    }
                    break;

                case wtAgm88:
                    FCC.SetMasterMode(FireControlComputer.AirGroundHARM);
                    FCC.SetSubMode(FireControlComputer.HTS);
                    break;

                case wtAgm65:
                    FCC.SetMasterMode(FireControlComputer.AirGroundMissile);
                    //MI done elsewhere
                    if (!g_bRealisticAvionics)
                    {
                        // M.N. no ATRealisticAV check needed here as in AtRealisticAV mode: g_bRealisticAvionics == true
                        if (PlayerOptions.GetAvionicsType() == ATRealistic)
                            FCC.SetSubMode(FireControlComputer.BSGT);
                        else
                            FCC.SetSubMode(FireControlComputer.SLAVE);
                    }
                    break;
                case wtMk82:
                case wtMk84:
                    if (FCC.GetMasterMode() != FireControlComputer.AirGroundBomb ||
                        FCC.GetSubMode() == FireControlComputer.STRAF ||
                        FCC.GetSubMode() == FireControlComputer.RCKT)
                    {
                        FCC.SetMasterMode(FireControlComputer.AirGroundBomb);
                        FCC.SetSubMode(FireControlComputer.CCRP);//me123 don't go to ccip everytime we select a mk82/84 hmm this might be ai stuff
                    }
                    break;

                case wtLAU:
                    FCC.SetMasterMode(FireControlComputer.AirGroundBomb);
                    FCC.SetSubMode(FireControlComputer.RCKT);
                    break;

                case wtGBU:
                    FCC.SetMasterMode(FireControlComputer.AirGroundLaser);
                    //MI in realistic we start in slave
                    if (!g_bRealisticAvionics)
                    {
                        if (PlayerOptions.GetAvionicsType() == ATRealistic)
                            FCC.SetSubMode(FireControlComputer.BSGT);
                        else
                            FCC.SetSubMode(FireControlComputer.SLAVE);
                    }
                    else
                    {
                        // M.N. added full realism mode
                        if (PlayerOptions.GetAvionicsType() != ATRealistic && PlayerOptions.GetAvionicsType() != ATRealisticAV)
                            FCC.SetSubMode(FireControlComputer.BSGT);
                        else
                            FCC.SetSubMode(FireControlComputer.SLAVE);
                    }
                    break;

                case wtFixed:
                    if (hardPoint[curHardpoint].GetWeaponClass() == wcCamera)
                    {
                        FCC.SetMasterMode(FireControlComputer.AirGroundCamera);
                    }
                    break;
            }

        }

        public void RemoveStore(int station, int storeId)
        {
            VehicleClassDataType* vc;
            float x, y, z;

            //Debug.Assert(hardPoint[station].weaponCount > 0);
            if (hardPoint[station].weaponCount >= 0)
            {
                if (ownship.IsAirplane() && !UnlimitedAmmo())
                {
                    vc = GetVehicleClassData(ownship.Type() - VU_LAST_ENTITY_TYPE);
                    Debug.Assert(((AircraftClass*)ownship).af);

                    hardPoint[station].GetPosition(&x, &y, &z);

                    if (((AircraftClass*)ownship).IsF16() && (station == 2 || station == 8) && storeId == gRackId_Single_Rack)
                        ((AircraftClass*)ownship).af.RemoveWeapon(WeaponDataTable[storeId].Weight - 283.0F, WeaponDataTable[storeId].DragIndex - 11.0F, y);
                    else if (((AircraftClass*)ownship).IsF16() && (station == 1 || station == 9))
                        ((AircraftClass*)ownship).af.RemoveWeapon(WeaponDataTable[storeId].Weight, 0.0F, y);
                    else if (vc.VisibleFlags & (1 << station))
                        ((AircraftClass*)ownship).af.RemoveWeapon(WeaponDataTable[storeId].Weight,
                                                                        WeaponDataTable[storeId].DragIndex,
                                                                        y);
                    else
                        ((AircraftClass*)ownship).af.RemoveWeapon(WeaponDataTable[storeId].Weight, 0.0F, 0.0F);

                    ChooseLimiterMode(station);
                }

                if (ownship == SimDriver.playerEntity)
                {
                    if (station < numHardpoints / 2)
                        JoystickPlayEffect(JoyLeftDrop, 0);
                    else
                        JoystickPlayEffect(JoyRightDrop, 0);
                }
            }
        }

        public void AddStore(int station, int storeId, int visible)
        {
            float x, y, z;

            if (ownship.IsAirplane() && !UnlimitedAmmo())
            {
                Debug.Assert(((AircraftClass*)ownship).af);

                hardPoint[station].GetPosition(&x, &y, &z);

                if (((AircraftClass*)ownship).IsF16() && (station == 1 || station == 9))
                    ((AircraftClass*)ownship).af.AddWeapon(WeaponDataTable[storeId].Weight, 0.0F, y);
                else if (visible)
                    ((AircraftClass*)ownship).af.AddWeapon(WeaponDataTable[storeId].Weight,
                                                                WeaponDataTable[storeId].DragIndex,
                                                                y);
                else
                    ((AircraftClass*)ownship).af.AddWeapon(WeaponDataTable[storeId].Weight, 0.0F, 0.0F);

                if (gLimiterMgr.HasLimiter(CatIIICommandType, ((AircraftClass*)ownship).af.VehicleIndex()))
                {
                    if (hardPoint[station].GetWeaponClass() == wcRocketWpn ||
                        hardPoint[station].GetWeaponClass() == wcBombWpn ||
                        hardPoint[station].GetWeaponClass() == wcTank ||
                        hardPoint[station].GetWeaponClass() == wcAgmWpn ||
                        hardPoint[station].GetWeaponClass() == wcHARMWpn ||
                        hardPoint[station].GetWeaponClass() == wcSamWpn ||
                        hardPoint[station].GetWeaponClass() == wcGbuWpn)
                    {

                        // OW CATIII Fix 
                        //if(!g_bEnableCATIIIExtension)	MI
                        if (!g_bRealisticAvionics)
                        {
                            ((AircraftClass*)ownship).af.SetFlag(AirframeClass.CATLimiterIII);
                            if (hardPoint[station].GetWeaponClass() == wcTank)
                            {
                                if (((AircraftClass*)ownship).af.curMaxGs > 7.5F)
                                    ((AircraftClass*)ownship).af.curMaxGs = 7.5F;
                            }
                            else
                            {
                                if (((AircraftClass*)ownship).af.curMaxGs > 6.0F)
                                    ((AircraftClass*)ownship).af.curMaxGs = 6.0F;
                            }
                        }

                        else
                        {
                            if (((AircraftClass*)ownship).af.IsSet(AirframeClass.IsDigital))//me123 let's only set the cat switch for ai
                                ((AircraftClass*)ownship).af.SetFlag(AirframeClass.CATLimiterIII);

                            if (hardPoint[station].GetWeaponClass() == wcTank)
                            {
                                if (station == 5) // centerline tank
                                {
                                    if (((AircraftClass*)ownship).af.curMaxGs > 7.0F)//me123 from 7.5
                                    {
                                        ((AircraftClass*)ownship).af.curMaxGs = 7.0F;//me123 from 7.5
                                        ((AircraftClass*)ownship).af.curMaxStoreSpeed = 600.0f;//me123
                                        ((AircraftClass*)ownship).af.ClearFlag(AirframeClass.CATLimiterIII);
                                    }
                                }
                                else // dollys
                                {
                                    if (((AircraftClass*)ownship).af.curMaxGs > 6.5F)
                                    {
                                        ((AircraftClass*)ownship).af.curMaxGs = 6.5F;
                                        ((AircraftClass*)ownship).af.curMaxStoreSpeed = 600.0f;//me123
                                        ((AircraftClass*)ownship).af.SetFlag(AirframeClass.CATLimiterIII);
                                    }
                                }
                            }
                            else
                            {
                                if (((AircraftClass*)ownship).af.curMaxGs > 5.5F)//me123 from 6.0
                                {
                                    ((AircraftClass*)ownship).af.curMaxGs = 5.5F;//me123 from 6.0
                                    ((AircraftClass*)ownship).af.curMaxStoreSpeed = 550.0f;//me123
                                }
                            }
                        }
                    }
                }

            }
        }
        public void ChooseLimiterMode(int hardpoint)
        {
            int i;
            float gLimit;
            float storespeed;//me123 

            gLimit = 9.0f;//me123
            storespeed = 800.0f;//me123

            if (ownship.IsAirplane() && ((AircraftClass*)ownship).af &&
                !((AircraftClass*)ownship).af.IsSet(AirframeClass.IsDigital))
            {
                gLimit = ((AircraftClass*)ownship).af.MaxGs();

                //MI
                //if(!g_bEnableCATIIIExtension || ((AircraftClass *)ownship).af.IsSet(AirframeClass.IsDigital))
                if (!g_bRealisticAvionics || ((AircraftClass*)ownship).af.IsSet(AirframeClass.IsDigital))
                    ((AircraftClass*)ownship).af.ClearFlag(AirframeClass.CATLimiterIII);

                if (gLimiterMgr.HasLimiter(CatIIICommandType, ((AircraftClass*)ownship).af.VehicleIndex()))
                {
                    for (i = 0; i < numHardpoints; i++)
                    {

                        if (hardPoint[i] && hardPoint[i].weaponPointer &&
                            hardPoint[i].weaponCount > 0 &&
                            (hardPoint[i].GetWeaponClass() == wcRocketWpn ||
                            hardPoint[i].GetWeaponClass() == wcBombWpn ||
                            hardPoint[i].GetWeaponClass() == wcTank ||
                            hardPoint[i].GetWeaponClass() == wcAgmWpn ||
                            hardPoint[i].GetWeaponClass() == wcHARMWpn ||
                            hardPoint[i].GetWeaponClass() == wcSamWpn ||
                            hardPoint[i].GetWeaponClass() == wcGbuWpn))
                        {
                            // OW CATIII Fix 
                            //if(!g_bEnableCATIIIExtension)	MI
                            if (!g_bRealisticAvionics)
                            {
                                if (hardPoint[i].GetWeaponClass() == wcTank)
                                {
                                    ((AircraftClass*)ownship).af.SetFlag(AirframeClass.CATLimiterIII);
                                    if (gLimit > 7.5F)
                                        gLimit = 7.5F;
                                }
                                else
                                {
                                    ((AircraftClass*)ownship).af.SetFlag(AirframeClass.CATLimiterIII);
                                    if (gLimit > 6.0F)
                                        gLimit = 6.0F;
                                }
                            }

                            else
                            {
                                if (hardPoint[i].GetWeaponClass() == wcTank)
                                {
                                    if (((AircraftClass*)ownship).af.IsSet(AirframeClass.IsDigital))//me123 don't change cat for player check addet
                                    {
                                        ((AircraftClass*)ownship).af.SetFlag(AirframeClass.CATLimiterIII);
                                    }
                                    /*if(gLimit > 7.0F)//me123 addet fuel check
                                    {
                                        storespeed = 600.0f;//me123
                                        if (((AircraftClass *)ownship).af.ExternalFuel() >=1.0f)
                                        gLimit = 7.0F;//me123 tanks g limit is 7Gz			
                                    }*/
                                    //MI me123... we need to check which tank we have here too
                                    if (i == numHardpoints / 2) // centerline tank
                                    {
                                        if (gLimit > 7.0F)//me123 from 7.5
                                        {
                                            gLimit = 7.0F;//me123 from 7.5
                                            storespeed = 600.0f;//me123
                                        }
                                    }
                                    else // dollys
                                    {
                                        if (gLimit > 6.5F)
                                        {
                                            gLimit = 6.5F;
                                            storespeed = 600.0f;//me123
                                        }
                                    }
                                }
                                else
                                {
                                    if (((AircraftClass*)ownship).af.IsSet(AirframeClass.IsDigital))//me123 don't change cat for player check addet
                                    {
                                        ((AircraftClass*)ownship).af.SetFlag(AirframeClass.CATLimiterIII);
                                    }
                                    if (gLimit > 5.5F)
                                    {
                                        gLimit = 5.5F;//me123 stores in general has 5.5 g limit
                                        storespeed = 550.0f;//me123
                                    }
                                }
                            }
                        }
                    }
                }
                ((AircraftClass*)ownship).af.curMaxGs = gLimit;
                ((AircraftClass*)ownship).af.curMaxStoreSpeed = storespeed;//me123
            }

        }

        public void RipOffWeapons(float noseAngle)
        {
            //we have only a gun or no weapons
            if (numHardpoints <= 1)
                return;

            int lwing = 1;
            int rwing = numHardpoints - 1;
            int center = 0;
            int count = numHardpoints - 1;
            int i;

            if (count % 2)
            {
                center = count / 2 + 1;
            }

            if (fabs(noseAngle) < 0.2588F)
            {
                //remove left wingtip weapon
                if (ownship.platformAngles.sinphi < -0.5F)
                {
                    if (curHardpoint == lwing)
                        ReleaseCurWeapon(-1);

                    JettisonStation(lwing);
                }

                //remove left wing stores
                if (ownship.Roll() > -30.0F * DTR && ownship.Roll() < -10.0F * DTR)
                    for (i = lwing + 1; i < count / 2 + 1; i++)
                    {
                        if (curHardpoint == i)
                            ReleaseCurWeapon(-1);

                        JettisonStation(i);
                    }

                //remove centerline stores
                if (center && ownship.Roll() > -20.0F * DTR && ownship.Roll() < 20.0F * DTR)
                {
                    if (curHardpoint == center)
                        ReleaseCurWeapon(-1);

                    JettisonStation(center);
                }

                //remove right wing stores
                if (ownship.Roll() > 10.0F * DTR && ownship.Roll() < 30.0F * DTR)
                    for (i = count / 2 + 1 + (center ? 1 : 0); i < rwing; i++)
                    {
                        if (curHardpoint == i)
                            ReleaseCurWeapon(-1);

                        JettisonStation(i);
                    }

                //remove right wingtip weapon
                if (rwing && ownship.platformAngles.sinphi > 0.5F)
                {
                    if (curHardpoint == rwing)
                        ReleaseCurWeapon(-1);

                    JettisonStation(rwing);
                }
            }
        }

        public float armingdelay;//me123 status ok. armingdelay addet
        public int aim120id; // JPO Aim120 Id no.
        public int AimId() { return aim120id + 1; }
        public void NextAimId() { aim120id = (aim120id + 1) % 4; }
        public enum Aim9Mode { WARM, COOLING, COOL, WARMING }
        public Aim9Mode aim9mode;
        public Aim9Mode GetCoolState() { return aim9mode; }
        public void SetCoolState(Aim9Mode state) { aim9mode = state; }
        // AIM9 time left for cooling
        /*VU_TIME aim9cooltime;
        // AIM9 cooling time left - approx. 3.5 hours worth
        VU_TIME aim9coolingtimeleft;
        // AIM9 - time to warm up after removing coolant
        VU_TIME aim9warmtime;*/
        public float aim9cooltime;
        public float aim9coolingtimeleft;
        public float aim9warmtime;

        //MI
        public int Prof1RP, Prof2RP;
        public int Prof1RS, Prof2RS;
        public int Prof1NSTL, Prof2NSTL;
        public int C2BA;
        public int angle;
        public float C1AD1, C1AD2, C2AD;
        public bool Prof1Pair, Prof2Pair, Prof1;
        public FireControlComputer.FCCSubMode Prof1SubMode, Prof2SubMode;
        public void StepWeaponClass()
        {
            int i, idDesired;
            int stationUnderTest;
            WeaponClass classDesired;
            WeaponType typeDesired, newType = curWeaponType;
            SimWeaponClass* found = null;

            // 2002-02-08 ADDED BY S.G. From SMSClass.WeaponStep, other it will CTD. This will fix the CTD but not the original cause which is why curHardpoint is -1.
            if (curHardpoint < 0)
            {
                // Do nothing if no station is currently selected
                return;
            }

            stationUnderTest = numHardpoints - 1;
            i = -1;

            // Otherwise, start with next weapon on this hardpoint
            ReleaseCurWeapon(curHardpoint);
            if (!hardPoint[curHardpoint])
                return;

            classDesired = hardPoint[curHardpoint].GetWeaponClass();
            typeDesired = hardPoint[curHardpoint].GetWeaponType();
            idDesired = hardPoint[curHardpoint].weaponId;

            bool HasAim9P = false;
            bool HasAim9M = false;
            bool HasAim120 = false;
            bool HasGun = false;

            if (SimDriver.playerEntity && SimDriver.playerEntity.FCC)
            {
                if (SimDriver.playerEntity.FCC.IsAAMasterMode())
                {
                    if (classDesired == wcGunWpn)
                    {
                        FindAim120(&stationUnderTest, &newType, &found);

                        if (!found)
                            FindAim9M(&stationUnderTest, &newType, &found);
                        if (!found)
                            FindAim9P(&stationUnderTest, &newType, &found);
                    }
                    else if (typeDesired == wtAim120)
                    {
                        FindAim9M(&stationUnderTest, &newType, &found);

                        if (!found)
                            FindAim9P(&stationUnderTest, &newType, &found);
                        if (!found)
                            FindGun(&stationUnderTest, &newType, &found);
                    }
                    else if (typeDesired == wtAim9 && hardPoint[curHardpoint].weaponCount > 0 &&
                        ((CampBaseClass*)hardPoint[curHardpoint].weaponPointer).GetSPType() != SPTYPE_AIM9P)
                    {
                        FindAim9P(&stationUnderTest, &newType, &found);

                        if (!found)
                            FindGun(&stationUnderTest, &newType, &found);
                        if (!found)
                            FindAim120(&stationUnderTest, &newType, &found);
                        if (!found)
                            FindAim9M(&stationUnderTest, &newType, &found);
                    }
                    else
                    {
                        FindGun(&stationUnderTest, &newType, &found);

                        if (!found)
                            FindAim120(&stationUnderTest, &newType, &found);
                        if (!found)
                            FindAim9M(&stationUnderTest, &newType, &found);
                    }
                }
                else if (SimDriver.playerEntity.FCC.GetMasterMode() == FireControlComputer.Dogfight)
                {
                    if (typeDesired == wtAim9 && hardPoint[curHardpoint].weaponCount > 0 &&
                        ((CampBaseClass*)hardPoint[curHardpoint].weaponPointer).GetSPType() != SPTYPE_AIM9P)
                    {
                        FindAim9P(&stationUnderTest, &newType, &found);

                        if (!found)
                            FindAim120(&stationUnderTest, &newType, &found);
                        if (!found)
                            FindAim9M(&stationUnderTest, &newType, &found);
                    }
                    else if (typeDesired == wtAim120)
                    {
                        FindAim9M(&stationUnderTest, &newType, &found);

                        if (!found)
                            FindAim9P(&stationUnderTest, &newType, &found);
                        if (!found)
                            FindAim120(&stationUnderTest, &newType, &found);
                    }
                    else
                    {
                        FindAim120(&stationUnderTest, &newType, &found);
                        if (!found)
                            FindAim9M(&stationUnderTest, &newType, &found);
                        if (!found)
                            FindAim9P(&stationUnderTest, &newType, &found);
                    }
                }
                else if (SimDriver.playerEntity.FCC.GetMasterMode() == FireControlComputer.MissileOverride)
                {
                    if (typeDesired == wtAim120)
                    {
                        FindAim9M(&stationUnderTest, &newType, &found);

                        if (!found)
                            FindAim9P(&stationUnderTest, &newType, &found);
                        if (!found)
                            FindGun(&stationUnderTest, &newType, &found);
                        if (!found)
                            FindAim120(&stationUnderTest, &newType, &found);
                    }
                    else if (typeDesired == wtAim9 && hardPoint[curHardpoint].weaponCount > 0 &&
                        ((CampBaseClass*)hardPoint[curHardpoint].weaponPointer).GetSPType() != SPTYPE_AIM9P)
                    {
                        FindAim9P(&stationUnderTest, &newType, &found);

                        if (!found)
                            FindGun(&stationUnderTest, &newType, &found);
                        if (!found)
                            FindAim120(&stationUnderTest, &newType, &found);
                        if (!found)
                            FindAim9M(&stationUnderTest, &newType, &found);
                    }
                    else if (classDesired == wcGunWpn)
                    {
                        FindAim120(&stationUnderTest, &newType, &found);

                        if (!found)
                            FindAim9M(&stationUnderTest, &newType, &found);
                        if (!found)
                            FindAim9P(&stationUnderTest, &newType, &found);
                        if (!found)
                            FindGun(&stationUnderTest, &newType, &found);
                    }
                    else
                    {
                        FindGun(&stationUnderTest, &newType, &found);
                        if (!found)
                            FindAim120(&stationUnderTest, &newType, &found);
                        if (!found)
                            FindAim9M(&stationUnderTest, &newType, &found);
                        if (!found)
                            FindAim9P(&stationUnderTest, &newType, &found);
                    }
                }
            }

            if (found)
            {
                SetCurrentWeapon(stationUnderTest, found);
                if (SimDriver.playerEntity && SimDriver.playerEntity.FCC &&
                    SimDriver.playerEntity.FCC.IsAAMasterMode())

                    SelectWeapon(newType, wdAir);
            }
            else
                SetCurrentWeapon(-1, null);
        }


        //MI
        public void FindAim120(int* stationUnderTest, WeaponType* newType, SimWeaponClass** found)
        {
            for (int i = 0; i < numHardpoints; i++)
            {
                if (hardPoint[i] && hardPoint[i].GetWeaponType() == wtAim120 && hardPoint[i].weaponCount > 0)
                {
                    *found = hardPoint[i].weaponPointer;
                    *stationUnderTest = i;
                    ReleaseCurWeapon(i);
                    *newType = hardPoint[i].GetWeaponType();
                    break;
                }
            }
        }

        public void FindAim9M(int* stationUnderTest, WeaponType* newType, SimWeaponClass** found)
        {
            for (int i = 0; i < numHardpoints; i++)
            {
                if (hardPoint[i] && hardPoint[i].GetWeaponType() == wtAim9 && hardPoint[i].weaponPointer &&
                    ((CampBaseClass*)hardPoint[i].weaponPointer).GetSPType() != SPTYPE_AIM9P &&
                    hardPoint[i].weaponCount > 0)
                {
                    *found = hardPoint[i].weaponPointer;
                    *stationUnderTest = i;
                    ReleaseCurWeapon(i);
                    *newType = hardPoint[i].GetWeaponType();
                    break;
                }
            }
        }

        public void FindAim9P(int* stationUnderTest, WeaponType* newType, SimWeaponClass** found)
        {
            for (int i = 0; i < numHardpoints; i++)
            {
                if (hardPoint[i] && hardPoint[i].GetWeaponType() == wtAim9 && hardPoint[i].weaponPointer &&
                    ((CampBaseClass*)hardPoint[i].weaponPointer).GetSPType() == SPTYPE_AIM9P &&
                    hardPoint[i].weaponCount > 0)
                {
                    *found = hardPoint[i].weaponPointer;
                    *stationUnderTest = i;
                    ReleaseCurWeapon(i);
                    *newType = hardPoint[i].GetWeaponType();
                    break;
                }
            }
        }

        public void FindGun(int* stationUnderTest, WeaponType* newType, SimWeaponClass** found)
        {
            for (int i = 0; i < numHardpoints; i++)
            {
                if (hardPoint[i] && hardPoint[i].GetWeaponClass() == wcGunWpn)
                {
                    *found = hardPoint[i].weaponPointer;
                    *stationUnderTest = i;
                    ReleaseCurWeapon(i);
                    *newType = hardPoint[i].GetWeaponType();
                    break;
                }
            }
        }

        //friend SmsDrawable;
        public delegate SimWeaponClass InitFunc(FalconEntity* parent, ushort type, int slot);
        public static SimWeaponClass* InitWeaponList(FalconEntity* parent, ushort weapid, int weapClass, int num,
                                        InitFunc initFunc)
        {
            SimWeaponClass* weapPtr = null;
            SimWeaponClass* lastPtr = null;
            int i = 0, rackSize = 1;

            // Find the real weapon class
            weapClass = SimWeaponDataTable[Falcon4ClassTable[WeaponDataTable[weapid].Index].vehicleDataIndex].weaponClass;

            // Determine rack size;
            if (num)
            {
                if (num > 6)
                    rackSize = num;
                else if (num > 4)
                    rackSize = 6;
                else if (num == 4)
                    rackSize = 4;
                else if (num == 2 && weapClass == wcAimWpn)
                    rackSize = 2;
                else if (num == 1 && weapClass == wcAimWpn)
                    rackSize = 1;
                else if (num > 1)
                    rackSize = 3;
                else if (num > 0)
                    rackSize = 1;

                for (i = 0; i < num; i++)
                {
                    // Load from back of rack to front (ie, 2 missiles on a tri-rack will
                    // load into slot 1 and 2, not 0 and 1)
                    weapPtr = initFunc(parent, weapid, rackSize - (i + 1));
                    if (lastPtr)
                        weapPtr.nextOnRail = lastPtr;
                    lastPtr = weapPtr;
                }
            }

            return weapPtr;
        }
    }



}
