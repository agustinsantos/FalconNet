using FalconNet.F4Common;
using FalconNet.VU;
using System;
using System.IO;
using VU_BYTE = System.Byte;

namespace FalconNet.FalcLib
{


	public class FalconGameEntity : VuGameEntity
	{
		public FalconGameType	gameType;
		public RulesClass		rules;
		
		// constructors & destructor
		public FalconGameEntity (ulong domainMask, string gameName) : base(domainMask,gameName)
		{
			throw new NotImplementedException ();
		}
#if TODO
		public FalconGameEntity (VU_BYTE[] stream)
		{
			throw new NotImplementedException ();
		}

		public FalconGameEntity (FileStream file)
		{
			throw new NotImplementedException ();
		}
#endif
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

