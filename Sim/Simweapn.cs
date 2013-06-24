using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.Sim
{
    public class SimWeaponClass : SimMoverClass
    {

        public SimWeaponClass* nextOnRail;
        public virtual int Sleep();
        public virtual int Wake();

        // public  virtual ~SimWeaponClass ();


        public char parentReferenced;
        public int rackSlot;
        public byte shooterPilotSlot;				// The pilotSlot of the pilot who shot this weapon
        public float lethalRadiusSqrd;
        public FalconEntity* parent;

        public SimWeaponClass(int type);
        public SimWeaponClass(VU_BYTE** stream);
        public SimWeaponClass(FILE* filePtr);
        public void InitData();
        public virtual void Init();
        public virtual int Exec();
        public virtual void SetDead(int d);

        public SimWeaponClass GetNextOnRail() { return nextOnRail; }
        public void SetRackSlot(int slot) { rackSlot = slot; }
        public int GetRackSlot() { return rackSlot; }
        public void SetParent(FalconEntity* newParent) { parent = newParent; }
        public FalconEntity* Parent() { return parent; }

        // virtual function interface
        // serialization functions
        public virtual int SaveSize();
        public virtual int Save(VU_BYTE** stream);	// returns bytes written
        public virtual int Save(FILE* file);		// returns bytes written

        // event handlers
        public virtual int Handle(VuFullUpdateEvent* evnt);
        public virtual int Handle(VuPositionUpdateEvent* evnt);
        public virtual int Handle(VuTransferEvent* evnt);

        // other stuff
        public void SendDamageMessage(FalconEntity* testObject, float rangeSquare, int damageType);

        public virtual bool IsWeapon() { return true; }
        public virtual int GetRadarType();
    }
}
