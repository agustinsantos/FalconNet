using FalconNet.VU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.Sim
{
    public class SimVehicleClass : SimMoverClass
    {

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


        public void SendFireMessage(SimWeaponClass* curWeapon, int type, int startFlag, SimObjectType* targetPtr, VU_ID tgtId = FalconNullId);
        // Avionics
        public PilotInputs* theInputs;
        public void InitWeapons(ushort* type, ushort* num);
        public enum SOI { SOI_HUD, SOI_RADAR, SOI_WEAPON, SOI_FCC }; //MI added SOI_FCC for HSD
        public SOI curSOI;
        public void SOIManager(SOI newSOI);
        public SOI GetSOI() { return curSOI; }	//MI
        public void StepSOI(int dir);	//MI

        public BaseBrain Brain() { return theBrain; }

        //Steering Info
        public WayPointClass curWaypoint;
        public WayPointClass waypoint;
        public Int32 numWaypoints;

        public WayPointClass GetWayPointNo(int n);
        public virtual void ReceiveOrders(FalconEvent* evnt) { }

        public void ApplyProximityDamage();

        // for dying, we no longer send the death message as soon as
        // strength goes to 0, we delay until object explodes
        public FalconDeathMessage* deathMessage;

        public SimVehicleClass(int type);
        public SimVehicleClass(VU_BYTE** stream);
        public SimVehicleClass(FILE* filePtr);
        //TODO public virtual ~SimVehicleClass ();
        public virtual void Init(SimInitDataClass* initData);
        public virtual int Wake();
        public virtual int Sleep();
        public virtual void MakeLocal();
        public virtual void MakeRemote();
        public virtual int Exec();
        public virtual void SetDead(int dead);
        public virtual void ApplyDamage(FalconDamageMessage* damageMessage);
        public virtual FireControlComputer* GetFCC() { return NULL; }
        public virtual SMSBaseClass* GetSMS() { return NULL; }
        public virtual float GetRCSFactor();
        public virtual float GetIRFactor();
        public virtual int GetRadarType();
        public virtual long GetTotalFuel() { return -1; } // KCK: -1 means "unfueled vehicle"

        //all ground vehicles and helicopters have pilots until they die
        //this determines whether they can talk on the radio or not
        public virtual int HasPilot() { return TRUE; }
        public virtual int IsVehicle() { return TRUE; }

        // this function can be called for entities which aren't necessarily
        // exec'd in a frame (ie ground units), but need to have their
        // gun tracers and (possibly other) weapons serviced
        public virtual void WeaponKeepAlive() { return; }

        // virtual function interface
        // serialization functions
        public virtual int SaveSize();
        public virtual int Save(VU_BYTE** stream);	// returns bytes written
        public virtual int Save(FILE* file);		// returns bytes written

        // event handlers
        public virtual int Handle(VuFullUpdateEvent* _event);
        public virtual int Handle(VuPositionUpdateEvent* _event);
        public virtual int Handle(VuTransferEvent* _event);
        public virtual VU_ERRCODE InsertionCallback();
    }
}
