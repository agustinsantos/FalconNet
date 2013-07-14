using FalconNet.CampaignBase;
using FalconNet.FalcLib;
using FalconNet.VU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VU_TIME = System.UInt64;
using VU_BYTE = System.Byte;
using FalconNet.Common;
using System.IO;
using FalconNet.Common.Encoding;

namespace FalconNet.Sim
{
    public class SimVehicleClass : SimMoverClass
    {
#if TODO
        // Controls
        protected BaseBrain theBrain;
        //      int numHardpoints;
        protected float ioPerturb;
        protected float irOutput;//me123
        // JB 000814
        protected float rBias;
        protected float pBias;
        protected float yBias;
        // JB 000814

        // how does the thing die
        protected int dyingType;
        protected enum DIE
        {
            DIE_SMOKE = 0,
            DIE_FIREBALL,
            DIE_SHORT_FIREBALL,
            DIE_INTERMITTENT_FIRE,
            DIE_INTERMITTENT_SMOKE
        };


        // rates and timers for targeting
        public VU_TIME nextTargetUpdate;
        public VU_TIME targetUpdateRate;
        public VU_TIME nextGeomCalc;
        public VU_TIME geomCalcRate;

        public virtual void InitData();
        public virtual void Cleanup();


        public void SendFireMessage(SimWeaponClass curWeapon, int type, int startFlag, SimObjectType targetPtr, VU_ID tgtId = FalconNullId)
        { throw new NotImplementedException(); }
        // Avionics
        public PilotInputs theInputs;
        public void InitWeapons(ushort* type, ushort* num)
        { throw new NotImplementedException(); }
        public enum SOI { SOI_HUD, SOI_RADAR, SOI_WEAPON, SOI_FCC }; //MI added SOI_FCC for HSD
        public SOI curSOI;
        public void SOIManager(SOI newSOI);
        public SOI GetSOI() { return curSOI; }	//MI
        public void StepSOI(int dir)
        { throw new NotImplementedException(); }	//MI

        public BaseBrain Brain() { return theBrain; }

        //Steering Info
        public WayPointClass curWaypoint;
        public WayPointClass waypoint;
        public Int32 numWaypoints;

        public WayPointClass GetWayPointNo(int n)
        { throw new NotImplementedException(); }
        public virtual void ReceiveOrders(FalconEvent evnt) { }

        public void ApplyProximityDamage()
        { throw new NotImplementedException(); }

        // for dying, we no longer send the death message as soon as
        // strength goes to 0, we delay until object explodes
        public FalconDeathMessage deathMessage;
#endif
        public SimVehicleClass(int type): base(type)
        { throw new NotImplementedException(); }
        public SimVehicleClass(ByteWrapper buf) : base(buf)
        { throw new NotImplementedException(); }
        public SimVehicleClass(FileStream stream)
            : base(stream)
        { throw new NotImplementedException(); }
#if TODO
        //TODO public virtual ~SimVehicleClass ();
        public virtual void Init(SimInitDataClass initData)
        { throw new NotImplementedException(); }
        public virtual int Wake()
        { throw new NotImplementedException(); }
        public virtual int Sleep()
        { throw new NotImplementedException(); }
        public virtual void MakeLocal()
        { throw new NotImplementedException(); }
        public virtual void MakeRemote()
        { throw new NotImplementedException(); }
        public virtual int Exec()
        { throw new NotImplementedException(); }
        public virtual void SetDead(int dead)
        { throw new NotImplementedException(); }
        public virtual void ApplyDamage(FalconDamageMessage damageMessage);
        public virtual FireControlComputer GetFCC() { return null; }
        public virtual SMSBaseClass GetSMS() { return null; }
        public virtual float GetRCSFactor()
        { throw new NotImplementedException(); }
        public virtual float GetIRFactor()
        { throw new NotImplementedException(); }
        public virtual int GetRadarType()
        { throw new NotImplementedException(); }
        public virtual long GetTotalFuel() { return -1; } // KCK: -1 means "unfueled vehicle"

        //all ground vehicles and helicopters have pilots until they die
        //this determines whether they can talk on the radio or not
        public virtual bool HasPilot() { return true; }
        public virtual bool IsVehicle() { return true; }

        // this function can be called for entities which aren't necessarily
        // exec'd in a frame (ie ground units), but need to have their
        // gun tracers and (possibly other) weapons serviced
        public virtual void WeaponKeepAlive() { return; }

        // virtual function interface
        // serialization functions
        public virtual int SaveSize()
        { throw new NotImplementedException(); }
        public virtual int Save(ByteWrapper buf)
        { throw new NotImplementedException(); }	// returns bytes written
        public virtual int Save(FileStream file)
        { throw new NotImplementedException(); }		// returns bytes written

        // event handlers
        public virtual int Handle(VuFullUpdateEvent _event)
        { throw new NotImplementedException(); }
        public virtual int Handle(VuPositionUpdateEvent _event)
        { throw new NotImplementedException(); }
        public virtual int Handle(VuTransferEvent _event)
        { throw new NotImplementedException(); }
        public virtual VU_ERRCODE InsertionCallback()
        { throw new NotImplementedException(); }
#endif
    }
}
