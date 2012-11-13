using System;
using System.IO;
using FalconNet.Common;
using FalconNet.FalcLib;
using FalconNet.VU;

namespace FalconNet.Campaign
{
    public class CampManagerClass : FalconEntity
    {

        public short managerFlags;				// Various user flags
        public Team owner;						// Controlling country/team


        // Constructors
        public CampManagerClass(ushort type, Team t)
		{throw new NotImplementedException();}
        public CampManagerClass(VU_BYTE[] stream)
		{throw new NotImplementedException();}
        public CampManagerClass(FileStream file)
		{throw new NotImplementedException();}
        //TODO public ~CampManagerClass (void);
        public virtual int SaveSize()
		{throw new NotImplementedException();}
        public virtual int Save(VU_BYTE[] stream)
		{throw new NotImplementedException();}
        public virtual int Save(FileStream file)
		{throw new NotImplementedException();}

        // event handlers
        public virtual int Handle(VuEvent evnt)
		{throw new NotImplementedException();}
        public virtual int Handle(VuFullUpdateEvent evnt)
		{throw new NotImplementedException();}
        public virtual int Handle(VuPositionUpdateEvent evnt)
		{throw new NotImplementedException();}
        public virtual int Handle(VuEntityCollisionEvent evnt)
		{throw new NotImplementedException();}
        public virtual int Handle(VuTransferEvent evnt)
		{throw new NotImplementedException();}
        public virtual int Handle(VuSessionEvent evnt)
		{throw new NotImplementedException();}
        public virtual VU_ERRCODE InsertionCallback()
		{throw new NotImplementedException();}
        public virtual VU_ERRCODE RemovalCallback()
		{throw new NotImplementedException();}
        public override int Wake() { return 0; }
        public override int Sleep() { return 0; }

        // Required pure virtuals
        public virtual int Task() { return 0; }
        public virtual void DoCalculations() { }

        // Core functions
        public int MyTasker(ushort p) { return IsLocal(); }
        public int GetTaskTeam() { return owner; }
        public void SendMessage(VU_ID id, short msg, short d1, short d2, short d3)
		{throw new NotImplementedException();}


        // ===========================
        // Global functions
        // ===========================

        public static VuEntity NewManager(short tid, VU_BYTE[] stream)
		{throw new NotImplementedException();}
    }
}

