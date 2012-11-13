using System;
using System.IO;
using FalconNet.VU;

namespace FalconNet.FalcLib
{
	// ==========================================
	// Game types
	// ==========================================
	public enum FalconGameType
	{
		game_PlayerPool=0,
		game_InstantAction,
		game_Dogfight,
		game_TacticalEngagement,
		game_Campaign,
		game_MaxGameTypes, // This MUST be the last type (I use it as an array size) Please don't assign values individually
		// (Except for playerpool = 0)
	};

	public class FalconGameEntity : VuGameEntity
	{
		public FalconGameType	gameType;
		public RulesClass		rules;
		
		// constructors & destructor
		public FalconGameEntity (ulong domainMask, string gameName)
		{
			throw new NotImplementedException ();
		}
		
		public FalconGameEntity (VU_BYTE[] stream)
		{
			throw new NotImplementedException ();
		}

		public FalconGameEntity (FileStream file)
		{
			throw new NotImplementedException ();
		}
		//TODO public virtual ~FalconGameEntity();

		// encoders
		public virtual int SaveSize ()
		{
			throw new NotImplementedException ();
		}

		public virtual int Save (VU_BYTE[] stream)
		{
			throw new NotImplementedException ();
		}

		public virtual int Save (FileStream file)
		{
			throw new NotImplementedException ();
		}

		public void DoFullUpdate ()
		{
			throw new NotImplementedException ();
		}

		// accessors
		public FalconGameType GetGameType ()
		{
			throw new NotImplementedException ();
		}

		public RulesClass GetRules ()
		{
			return rules;
		}

		// setters
		public void SetGameType (FalconGameType type)
		{
			throw new NotImplementedException ();
		}

		public void UpdateRules (RulesStruct newrules)
		{
			throw new NotImplementedException ();
		}

		public void EncipherPassword (ref string pw, long size)
		{throw new NotImplementedException();}

		public long CheckPassword (string passwd)
		{
			throw new NotImplementedException ();
		}

		// event Handlers
		public virtual VU_ERRCODE Handle (VuFullUpdateEvent evnt)
		{
			throw new NotImplementedException ();
		}

		// Other crap
		public virtual VU_ERRCODE Distribute (VuSessionEntity sess)
		{
			throw new NotImplementedException ();
		}
	
		private int LocalSize ()
		{
			throw new NotImplementedException ();
		} 
	}
}

