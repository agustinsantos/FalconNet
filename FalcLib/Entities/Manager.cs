using System;
using System.IO;
using FalconNet.Common;
using FalconNet.FalcLib;
using FalconNet.VU;
using VU_BYTE = System.Byte;
using Team = System.SByte;

namespace FalconNet.Campaign
{
    public class CampManagerClass : FalconEntity
    {

        public short managerFlags;				// Various user flags
        public Team owner;						// Controlling country/team


        // Constructors
        public CampManagerClass(ushort type, Team t)
            : base(type, 0)
        { throw new NotImplementedException(); }
#if TODO
        public CampManagerClass(VU_BYTE[] stream): base(FalconEntity.VU_LAST_ENTITY_TYPE)
		{throw new NotImplementedException();}
        public CampManagerClass(FileStream file): base(FalconEntity.VU_LAST_ENTITY_TYPE)
		{throw new NotImplementedException();}
		public CampManagerClass(byte[] bytes, ref int offset, int version)
			: base(bytes, ref offset, version)
        {
            managerFlags = BitConverter.ToInt16(bytes, offset);
            offset += 2;

            owner = bytes[offset];
            offset++;
		}


        //TODO public ~CampManagerClass (void);
        public override int SaveSize()
		{throw new NotImplementedException();}
        public virtual int Save(VU_BYTE[] stream)
		{throw new NotImplementedException();}
        public override int Save(FileStream file)
		{throw new NotImplementedException();}
#endif
        // event handlers
        public override VU_ERRCODE Handle(VuEvent evnt)
        { throw new NotImplementedException(); }
        public override VU_ERRCODE Handle(VuFullUpdateEvent evnt)
        { throw new NotImplementedException(); }
        public override VU_ERRCODE Handle(VuPositionUpdateEvent evnt)
        { throw new NotImplementedException(); }
        public override VU_ERRCODE Handle(VuEntityCollisionEvent evnt)
        { throw new NotImplementedException(); }
        public override VU_ERRCODE Handle(VuTransferEvent evnt)
        { throw new NotImplementedException(); }
        public override VU_ERRCODE Handle(VuSessionEvent evnt)
        { throw new NotImplementedException(); }
        public virtual VU_ERRCODE InsertionCallback()
        { throw new NotImplementedException(); }
        public override VU_ERRCODE RemovalCallback()
        { throw new NotImplementedException(); }
        public override int Wake() { return 0; }
        public override int Sleep() { return 0; }

        // Required pure virtuals
        public virtual int Task() { return 0; }
        public virtual void DoCalculations() { }

        // Core functions
        public bool MyTasker(ushort p) { return IsLocal(); }
        public Team GetTaskTeam() { return owner; }
        public void SendMessage(VU_ID id, short msg, short d1, short d2, short d3)
        { throw new NotImplementedException(); }


        // ===========================
        // Global functions
        // ===========================

        public static VuEntity NewManager(short tid, VU_BYTE[] stream)
        { throw new NotImplementedException(); }

        public override short GetCampID()
        {
            throw new NotImplementedException();
        }

        public override Team GetTeam()
        {
            throw new NotImplementedException();
        }

        public override VU_BYTE GetCountry()
        {
            throw new NotImplementedException();
        }
    }
}

