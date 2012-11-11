using System;
using FalconNet.FalcLib;

namespace FalconNet.Campaign
{
		public class CampManagerClass : FalconEntity 
		{
		
			public short			managerFlags;				// Various user flags
			public Team			owner;						// Controlling country/team
	
		
			// Constructors
			public CampManagerClass (ushort type, Team t);
			public CampManagerClass (VU_BYTE **stream);
			public CampManagerClass (FILE *file);
			//TODO public ~CampManagerClass (void);
			public virtual int SaveSize ();
			public virtual int Save (VU_BYTE **stream);
			public virtual int Save (FILE *file);
	
			// event handlers
			public virtual int Handle(VuEvent *evnt);
			public virtual int Handle(VuFullUpdateEvent *evnt);
			public virtual int Handle(VuPositionUpdateEvent *evnt);
			public virtual int Handle(VuEntityCollisionEvent *evnt);
			public virtual int Handle(VuTransferEvent *evnt);
			public virtual int Handle(VuSessionEvent *evnt);
			public virtual VU_ERRCODE InsertionCallback();
			public virtual VU_ERRCODE RemovalCallback();
			public virtual int Wake () {return 0;}
			public virtual int Sleep () {return 0;}
	
			// Required pure virtuals
			public virtual int Task()							{	return 0; }
			public virtual void DoCalculations()				{}
	
			// Core functions
			public int MyTasker (ushort p)				{	return IsLocal(); }
			public int GetTaskTeam ()						{	return owner; }
			public void SendMessage (VU_ID id, short msg, short d1, short d2, short d3);
		

	// ===========================
	// Global functions
	// ===========================
	
	public static VuEntity NewManager (short tid, VU_BYTE *stream);
	}
}

