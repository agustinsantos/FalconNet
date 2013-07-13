using System;
using VU_BOOL = System.Boolean;

namespace FalconNet.VU
{

    //--------------------------------------------------
    public class VuGroundCollisionEvent : VuEvent
    {

        public VuGroundCollisionEvent(VuEntity entity, VuTargetEntity target, VU_BOOL loopback = false)
            : base(VU_MSG_DEF.VU_GROUND_COLLISION_EVENT, entity.Id(), target, loopback)
        {
            // empty
        }
        public VuGroundCollisionEvent(VU_ID senderid, VU_ID target)
            : base(VU_MSG_DEF.VU_GROUND_COLLISION_EVENT, senderid, target)
        {
            // empty
        }
        //TODO public virtual ~VuGroundCollisionEvent();


        protected override VU_ERRCODE Process(VU_BOOL autod) { throw new NotImplementedException(); }

    }

}
