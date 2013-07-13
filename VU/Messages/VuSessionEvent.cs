using System;
using VU_BOOL = System.Boolean;
using VU_TIME = System.UInt64;

namespace FalconNet.VU
{
    //--------------------------------------------------
    public class VuSessionEvent : VuEvent
    {

        public VuSessionEvent(VuEntity ent, vuEventTypes subtype, VuTargetEntity target,
                       VU_BOOL loopback = false)
            : base(VU_MSG_DEF.VU_SESSION_EVENT, ent.Id(), target, loopback)
        {
            subtype_ = subtype;
            group_ = new VU_ID(0, 0);
            callsign_ = null;
            syncState_ = VuSessionSync.VU_NO_SYNC;
            gameTime_ = VUSTATIC.vuxGameTime;
            string name = "bad session";
            if (ent.IsSession())
            {
                name = ((VuSessionEntity)ent).Callsign();
                group_ = ((VuSessionEntity)ent).GameId();

#if VU_TRACK_LATENCY
    syncState_ = ((VuSessionEntity*)ent).TimeSyncState();
#endif

            }
            else if (ent.IsGroup())
            {
                name = ((VuGroupEntity)ent).GroupName();
            }
            callsign_ = name;

            SetEntity(ent);
            RequestReliableTransmit();
            RequestOutOfBandTransmit();
        }

        public VuSessionEvent(VU_ID senderid, VU_ID target)
            : base(VU_MSG_DEF.VU_SESSION_EVENT, senderid, target)
        {
            subtype_ = vuEventTypes.VU_SESSION_UNKNOWN_SUBTYPE;
            group_ = new VU_ID(0, 0);
            callsign_ = null;
            syncState_ = VuSessionSync.VU_NO_SYNC;
            gameTime_ = VUSTATIC.vuxGameTime;
            RequestReliableTransmit();
        }
        //TODO public virtual ~VuSessionEvent();

        protected override VU_ERRCODE Process(VU_BOOL autod) { throw new NotImplementedException(); }

        private int LocalSize() { throw new NotImplementedException(); }

        // data

        public vuEventTypes subtype_;
        public VU_ID group_;
        public string callsign_;
        public VuSessionSync syncState_;
        public VU_TIME gameTime_;
    }


}
