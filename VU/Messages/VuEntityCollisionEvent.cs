using System;
using VU_BOOL = System.Boolean;
using VU_DAMAGE = System.UInt64;

namespace FalconNet.VU
{
    //--------------------------------------------------
    public class VuEntityCollisionEvent : VuEvent
    {

        public VuEntityCollisionEvent(VuEntity entity, VU_ID otherId,
                                VU_DAMAGE hitLocation, int hitEffect,
                                VuTargetEntity target, VU_BOOL loopback = false)
            : base(VU_MSG_DEF.VU_ENTITY_COLLISION_EVENT, entity.Id(), target, loopback)
        {

            otherId_ = otherId;
            hitLocation_ = hitLocation;
            hitEffect_ = hitEffect;
            SetEntity(entity);
        }
        public VuEntityCollisionEvent(VU_ID senderid, VU_ID target)
            : base(VU_MSG_DEF.VU_ENTITY_COLLISION_EVENT, senderid, target)
        {

            otherId_ = new VU_ID(0, 0);
        }

        //TODO public virtual ~VuEntityCollisionEvent();

        protected override VU_ERRCODE Process(VU_BOOL autod) { throw new NotImplementedException(); }

        private int LocalSize() { throw new NotImplementedException(); }

        // data
        public VU_ID otherId_;
        public VU_DAMAGE hitLocation_;	// affects damage
        public int hitEffect_;		// affects hitpoints/health
    }

}
