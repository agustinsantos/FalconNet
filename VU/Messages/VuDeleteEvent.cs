using System;
using VU_BOOL = System.Boolean;
using VU_MSG_TYPE = System.Byte;

namespace FalconNet.VU
{
    //--------------------------------------------------
    public class VuDeleteEvent : VuEvent
    {

        public VuDeleteEvent(VuEntity entity)
            : base(VU_MSG_DEF.VU_DELETE_EVENT, entity.Id(),
      entity.IsGlobal() ? (VuTargetEntity)VUSTATIC.vuGlobalGroup
                         : (VuTargetEntity)VUSTATIC.vuLocalSessionEntity.Game(), true)
        {
            SetEntity(entity);
        }

        public VuDeleteEvent(VU_ID senderid, VU_ID target)
            : base(VU_MSG_DEF.VU_DELETE_EVENT, senderid, target)
        {
            // empty
        }

        //TODO public virtual ~VuDeleteEvent();

        protected override VU_ERRCODE Process(VU_BOOL autod)
        {
            if (Entity() != null)
            {
                Entity().Handle(this);
                return VU_ERRCODE.VU_SUCCESS;
            }
            return VU_ERRCODE.VU_NO_OP;
        }

        protected override VU_ERRCODE Activate(VuEntity ent)
        {
#if TODO
		            if (ent != null)
            {
                SetEntity(ent);
            }
            if (Entity() != null)
            {
                if (Entity().VuState() == VU_MEM_STATE.VU_MEM_ACTIVE)
                {
                    VUSTATIC.vuDatabase.DeleteRemove(Entity());
                    return VU_ERRCODE.VU_SUCCESS;
                }
                else if (VUSTATIC.vuAntiDB.Find(entityId_) != null)
                {
                    VUSTATIC.vuAntiDB.Remove(entityId_);
                    return VU_ERRCODE.VU_SUCCESS;
                }
                else if (!Entity().IsLocal())
                {
                    // prevent duplicate delete event from remote source
                    SetEntity(null);
                }
            }
            return VU_ERRCODE.VU_NO_OP;  
#endif
            throw new NotImplementedException();
        }

    }
}
