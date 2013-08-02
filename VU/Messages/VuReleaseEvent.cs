using System;
using VU_BOOL = System.Boolean;


namespace FalconNet.VU
{
    //--------------------------------------------------
    public class VuReleaseEvent : VuEvent
    {

        public VuReleaseEvent(VuEntity entity)
            : base(VU_MSG_DEF.VU_RELEASE_EVENT, entity.Id(), VUSTATIC.vuLocalSessionEntity, true)
        {
            SetEntity(entity);
        }
        //TODO public virtual ~VuReleaseEvent();

        public override VU_BOOL DoSend()     // returns false
        {
            return false;
        }

        protected override VU_ERRCODE Activate(VuEntity ent)
        {
            if (ent != null)
            {
                SetEntity(ent);
            }
            if (Entity() != null && Entity().VuState() == VU_MEM_STATE.VU_MEM_ACTIVE)
            {
                if (Entity().Id().creator_ == VUSTATIC.vuLocalSession.creator_)
                {
                    // anti db will check for presense in db prior to insert
#if TODO
		                    VUSTATIC.vuAntiDB.Insert(Entity());  
#endif
                    throw new NotImplementedException();
                }
                Entity().share_.ownerId_ = VU_ID.vuNullId;
#if TODO
		                VUSTATIC.vuDatabase.DeleteRemove(Entity());  
#endif
                throw new NotImplementedException();
                return VU_ERRCODE.VU_SUCCESS;
            }
            return VU_ERRCODE.VU_NO_OP;
        }
        protected override VU_ERRCODE Process(VU_BOOL autod)
        {
            if (Entity() != null)
            {
                Entity().Handle(this);
                return VU_ERRCODE.VU_SUCCESS;
            }
            return VU_ERRCODE.VU_NO_OP;
        }


    }
}
