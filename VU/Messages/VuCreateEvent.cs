using FalconNet.Common.Encoding;
using System.IO;
using VU_BOOL = System.Boolean;
using VU_BYTE = System.Byte;
using VU_MSG_TYPE = System.Byte;

namespace FalconNet.VU
{

    //--------------------------------------------------
    public class VuCreateEvent : VuEvent
    {

        public VuCreateEvent(VuEntity entity, VuTargetEntity target, VU_BOOL loopback = false)
            : base(VU_MSG_DEF.VU_CREATE_EVENT, entity.Id(), target, loopback)
        {
#if VU_USE_CLASS_INFO
  memcpy(classInfo_, entity.EntityType().classInfo_, CLASS_NUM_BYTES);
#endif
            vutype_ = entity.Type();
            size_ = 0;
            data_ = null;
            expandedData_ = null;
        }
        public VuCreateEvent(VU_ID senderid, VU_ID target)
            : base(VU_MSG_DEF.VU_CREATE_EVENT, senderid, target)
        {
#if  VU_USE_CLASS_INFO
  memset(classInfo_, '\0', CLASS_NUM_BYTES);
#endif
            size_ = 0;
            data_ = null;
            vutype_ = 0;
            expandedData_ = null;
        }
        //TODO public virtual ~VuCreateEvent();

        public override VU_BOOL DoSend()     // returns true if ent is in database
        {
            if (Entity() != null && Entity().VuState() == VU_MEM_STATE.VU_MEM_ACTIVE)
            {
                return true;
            }
            return false;
        }


        public VuEntity EventData() { return expandedData_; }

        protected VuCreateEvent(VU_MSG_TYPE type, VuEntity ent, VuTargetEntity target,
                              VU_BOOL loopback = false)
            : base(type, ent.Id(), target, loopback)
        {
            SetEntity(ent);
#if VU_USE_CLASS_INFO
  memcpy(classInfo_, ent.EntityType().classInfo_, CLASS_NUM_BYTES);
#endif
            vutype_ = ent.Type();
            size_ = 0;
            data_ = null;
            expandedData_ = null;

#if _DEBUG
//  MonoPrint("VuCreateEvent id target loopback %d,%d,%d\n", entity.Id(), target,loopback);//me123
#endif
        }

        protected VuCreateEvent(VU_MSG_TYPE type, VU_ID senderid, VU_ID target)
            : base(type, senderid, target)
        {
#if VU_USE_CLASS_INFO
  memset(classInfo_, '\0', CLASS_NUM_BYTES);
#endif
            size_ = 0;
            data_ = null;
            vutype_ = 0;
            expandedData_ = null;

#if _DEBUG
//MonoPrint("VuCreateEvent1 senderid target %d,%d\n", senderid, target);//me123
#endif
        }

        protected override VU_ERRCODE Activate(VuEntity ent)
        {
            return base.Activate(ent);
        }

        protected override VU_ERRCODE Process(VU_BOOL autod)
        {
            if (expandedData_ != null)
            {
                return VU_ERRCODE.VU_NO_OP;    // already done...
            }
            if (vutype_ < VuEntity.VU_LAST_ENTITY_TYPE)
            {
                expandedData_ = VuCreateEntity(vutype_, size_, new MemoryStream(data_));
            }
            else
            {
                expandedData_ = VUSTATIC.VuxCreateEntity(vutype_, size_, data_);
            }
            if (expandedData_ != null)
            {
                VuEntity.VuReferenceEntity(expandedData_);
                if (!expandedData_.IsLocal())
                {
                    expandedData_.SetTransmissionTime(postTime_);
                }
                if (Entity() != null && (Entity().OwnerId() != expandedData_.OwnerId()) &&
                    Entity() != expandedData_)
                {
                    if (Entity().IsPrivate())
                    {
                        Entity().ChangeId(expandedData_);
                        SetEntity(null);
                    }
                    else
                    {
                        VuEntity winner = ResolveWinner(Entity(), expandedData_);
                        if (winner == Entity())
                        {
                            // this will prevent a db insert of expandedData
                            SetEntity(expandedData_);
                        }
                        else if (winner == expandedData_)
                        {
                            Entity().SetOwnerId(expandedData_.OwnerId());
                            if (Entity().Type() == expandedData_.Type())
                            {
                                // if we have the same type, then just transfer to winner
                                VuTargetEntity dest = null;
                                if (Entity().IsGlobal())
                                {
                                    dest = VUSTATIC.vuGlobalGroup;
                                }
                                else
                                {
                                    dest = VUSTATIC.vuLocalSessionEntity.Game();
                                }
                                VuTransferEvent event_ = new VuTransferEvent(Entity(), dest);
                                event_.Ref();
                                VuMessageQueue.PostVuMessage(event_);
                                Entity().Handle(event_);
                                VUSTATIC.vuDatabase.Handle(event_);
                                event_.UnRef();
                                SetEntity(expandedData_);
                            }
                            else
                            {
                                type_ = VU_MSG_DEF.VU_CREATE_EVENT;
                                if (Entity().VuState() == VU_MEM_STATE.VU_MEM_ACTIVE)
                                {
                                    // note: this will cause a memory leak! (but is extrememly rare)
                                    //   Basically, we have two ents with the same id, and we cannot
                                    //   keep track of both, even to know when it is safe to delete
                                    //   the abandoned entity -- so we remove it from VU, but don't
                                    //   call its destructor... the last thing we do with it is call 
                                    //   VuxRetireEntity, leaving ultimate cleanup up to the app
                                    VuEntity.VuReferenceEntity(Entity());
                                    VUSTATIC.vuDatabase.Remove(Entity());
                                    VUSTATIC.vuAntiDB.Remove(Entity());
                                }
                                VUSTATIC.VuxRetireEntity(Entity());
                                SetEntity(null);
                            }
                        }
                    }
                }
                if (Entity() != null && type_ == VU_MSG_DEF.VU_FULL_UPDATE_EVENT)
                {
                    Entity().Handle((VuFullUpdateEvent)this);
                    return VU_ERRCODE.VU_SUCCESS;
                }
                else if (Entity() == null)
                {
                    SetEntity(expandedData_);

                    // OW: me123 MP Fix
#if NOTHING
      vuDatabase.Insert(Entity());
#else
                    VUSTATIC.vuDatabase.SilentInsert(Entity());	 //me123 to silent otherwise this will
#endif

                    return VU_ERRCODE.VU_SUCCESS;
                }
                return VU_ERRCODE.VU_NO_OP;
            }
            return VU_ERRCODE.VU_ERROR;
        }

        protected static VuEntity VuCreateEntity(ushort type,
               ushort p,
               Stream data)
        {

            switch (type)
            {
                case VuEntity.VU_SESSION_ENTITY_TYPE:
                    VuSessionEntity retval = new VuSessionEntity();
                    VuSessionEntityEncodingLE.Decode(data, ref retval);
                    return retval;
                case VuEntity.VU_GROUP_ENTITY_TYPE:
                    VuGroupEntity retval2 = new VuGroupEntity();
                    VuGroupEntityEncodingLE.Decode(data, ref retval2);
                    return retval2;
                case VuEntity.VU_GAME_ENTITY_TYPE:
                    VuGameEntity retval3 = new VuGameEntity();
                    VuGameEntityEncodingLE.Decode(data, ref retval3);
                    return retval3;
                case VuEntity.VU_GLOBAL_GROUP_ENTITY_TYPE:
                case VuEntity.VU_PLAYER_POOL_GROUP_ENTITY_TYPE:
                    return null;
                default:
                    return null;
            }
        }

        protected static VuEntity ResolveWinner(VuEntity ent1, VuEntity ent2)
        {
            VuEntity retval = null;

            if (ent1.EntityType().createPriority_ > ent2.EntityType().createPriority_)
            {
                retval = ent1;
            }
            else if (ent1.EntityType().createPriority_ < ent2.EntityType().createPriority_)
            {
                retval = ent2;
            }
            else if (ent1.OwnerId().creator_ == ent1.Id().creator_)
            {
                retval = ent1;
            }
            else if (ent2.OwnerId().creator_ == ent2.Id().creator_)
            {
                retval = ent2;
            }
            else if (ent1.OwnerId().creator_ < ent2.OwnerId().creator_)
            {
                retval = ent1;
            }
            else
            {
                retval = ent2;
            }

            return retval;
        }
        // data

        public VuEntity expandedData_;
#if  VU_USE_CLASS_INFO
  VU_BYTE classInfo_[CLASS_NUM_BYTES];	// entity class type
#endif
        public ushort vutype_;			// entity type
        public ushort size_;
        public VU_BYTE[] data_;
    }


}
