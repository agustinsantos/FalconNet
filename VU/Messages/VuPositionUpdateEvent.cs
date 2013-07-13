using BIG_SCALAR = System.Single;
using SM_SCALAR = System.Single;
using VU_BOOL = System.Boolean;

namespace FalconNet.VU
{
    //--------------------------------------------------
    public class VuPositionUpdateEvent : VuEvent
    {

        public VuPositionUpdateEvent(VuEntity entity, VuTargetEntity target,
                              VU_BOOL loopback = false)
            : base(VU_MSG_DEF.VU_POSITION_UPDATE_EVENT, entity.Id(), target, loopback)
        {
            if (entity != null)
            {
                SetEntity(entity);
                updateTime_ = entity.LastUpdateTime();

                x_ = entity.XPos();
                y_ = entity.YPos();
                z_ = entity.ZPos();

                dx_ = entity.XDelta();
                dy_ = entity.YDelta();
                dz_ = entity.ZDelta();

#if VU_USE_QUATERNION
    VU_QUAT *quat = entity.Quat();
    ML_QuatCopy(quat_, *quat);
    VU_VECT *dquat = entity.DeltaQuat();
    ML_VectCopy(dquat_, *dquat);
    theta_ = entity.Theta();
#else
                yaw_ = entity.Yaw();
                pitch_ = entity.Pitch();
                roll_ = entity.Roll();

                //dyaw_   = entity.YawDelta();
                //dpitch_ = entity.PitchDelta();
                //droll_  = entity.RollDelta();
                //VU_PRINT("yaw=%3.3f, pitch=%3.3f, roll=%3.3f, dyaw=%3.3f, dpitch=%3.3f, droll=%3.3f\n", yaw_, pitch_, roll_, dyaw_, dpitch_, droll_);
#endif
            }
        }
        public VuPositionUpdateEvent(VU_ID senderid, VU_ID target)
            : base(VU_MSG_DEF.VU_POSITION_UPDATE_EVENT, senderid, target)
        {
            // empty
        }
        //TODO public virtual ~VuPositionUpdateEvent();

        public override VU_BOOL DoSend()
        {
            // test is done in vudriver.cpp, prior to generation of event
            return true;
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

        // data

#if  VU_USE_QUATERNION
  VU_QUAT quat_;	// quaternion indicating current facing
  VU_VECT dquat_;	// unit vector expressing quaternion delta
  SM_SCALAR theta_;	// scalar indicating rate of above delta
#else // !VU_USE_QUATERNION
        public SM_SCALAR yaw_, pitch_, roll_;
        public SM_SCALAR dyaw_, dpitch_, droll_;
#endif
        public BIG_SCALAR x_, y_, z_;
        public SM_SCALAR dx_, dy_, dz_;
    }


}
