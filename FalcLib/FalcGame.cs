using FalconNet.F4Common;
using FalconNet.VU;
using System;
using System.IO;
using VU_BYTE = System.Byte;

namespace FalconNet.FalcLib
{


    public class FalconGameEntity : VuGameEntity
    {
        public FalconGameType gameType;
        public RulesClass rules;
        public static int F4GameType;

        // constructors & destructor
        public FalconGameEntity(ulong domainMask, string gameName)
            : base(domainMask, gameName)
        {
            gameType = FalconGameType.game_PlayerPool;

            // KCK: To keep this from conflicting with entities we plan to load, force
            // the creater to something non-zero for single player games.
            if (share_.id_.creator_ == 0)
            {
                share_.id_.creator_ = 1;
                // Make a new collection with a filter to match
                VuSessionFilter filter = new VuSessionFilter(Id());
                sessionCollection_.Unregister();
                //delete sessionCollection_;
                sessionCollection_ = new VuOrderedList(filter);
                sessionCollection_.Register();
            }

            SetEntityType((ushort)(F4GameType + VU_LAST_ENTITY_TYPE));
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
        public virtual int SaveSize()
        {
            throw new NotImplementedException();
        }

        public virtual int Save(VU_BYTE[] stream)
        {
            throw new NotImplementedException();
        }

        public virtual int Save(FileStream file)
        {
            throw new NotImplementedException();
        }

        public void DoFullUpdate()
        {
            throw new NotImplementedException();
        }

        // accessors
        public FalconGameType GetGameType()
        {
            throw new NotImplementedException();
        }

        public RulesClass GetRules()
        {
            return rules;
        }

        // setters
        public void SetGameType(FalconGameType type)
        {
            throw new NotImplementedException();
        }

        public void UpdateRules(RulesStruct newrules)
        {
            throw new NotImplementedException();
        }

        public void EncipherPassword(ref string pw, long size)
        { throw new NotImplementedException(); }

        public long CheckPassword(string passwd)
        {
            throw new NotImplementedException();
        }

        // event Handlers
        public virtual VU_ERRCODE Handle(VuFullUpdateEvent evnt)
        {
            throw new NotImplementedException();
        }

        // Other crap
        public virtual VU_ERRCODE Distribute(VuSessionEntity sess)
        {
            throw new NotImplementedException();
        }

        private int LocalSize()
        {
            throw new NotImplementedException();
        }
    }
}

