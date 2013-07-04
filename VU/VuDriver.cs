using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VU_BYTE = System.Byte;
using VU_TIME = System.UInt64;
using VU_BOOL = System.Boolean;
using BIG_SCALAR = System.Single;
using SM_SCALAR = System.Single;

namespace FalconNet.VU
{
    public class VuDriverSettings
    {

        public VuDriverSettings(SM_SCALAR x, SM_SCALAR y, SM_SCALAR z,
#if VU_USE_QUATERNION
			SM_SCALAR quatDist,
#else // !VU_USE_QUATERNION
 SM_SCALAR yaw, SM_SCALAR pitch, SM_SCALAR roll,
#endif
 SM_SCALAR maxJumpDist, SM_SCALAR maxJumpAngle,
                  VU_TIME lookaheadTime
#if VU_QUEUE_DR_UPDATES
            , VuFilter *filter = 0
#endif
) { throw new NotImplementedException(); }
        //TODO  public ~VuDriverSettings();

        public SM_SCALAR xtol_, ytol_, ztol_;	// fine tolerance values
#if VU_USE_QUATERNION
  SM_SCALAR quattol_;
#else // !VU_USE_QUATERNION
        public SM_SCALAR yawtol_, pitchtol_, rolltol_;
#endif
        public SM_SCALAR maxJumpDist_;
        public SM_SCALAR maxJumpAngle_;
        public SM_SCALAR globalPosTolMod_;
        public SM_SCALAR globalAngleTolMod_;
        public VU_TIME lookaheadTime_;
#if VU_QUEUE_DR_UPDATES
  public VuFifoQueue *updateQueue_;
  public VuFifoQueue *roughUpdateQueue_;
#endif
    }

    //-----------------------------------------
    public abstract class VuDriver
    {

        //TODO public virtual ~VuDriver();

        public abstract int Type();

        public abstract void NoExec(VU_TIME timestamp);
        public abstract void Exec(VU_TIME timestamp);
        public abstract void AutoExec(VU_TIME timestamp);

        public abstract VU_ERRCODE Handle(VuEvent evnt);
        public abstract VU_ERRCODE Handle(VuFullUpdateEvent evnt);
        public abstract VU_ERRCODE Handle(VuPositionUpdateEvent evnt);

        public abstract void AlignTimeAdd(VU_TIME timeDelta);
        public abstract void AlignTimeSubtract(VU_TIME timeDelta);
        public abstract void ResetLastUpdateTime(VU_TIME time);

        // Accessor
        public VuEntity Entity() { return entity_; }

        // AI hooks
        public virtual int AiType()		// returns 0 if no AI is present
            { throw new NotImplementedException(); }
        public virtual VU_BOOL AiExec()	// executes Ai component
            { throw new NotImplementedException(); }
        public virtual Object AiPointer()	// returns pointer to Ai data (game specific)
        { throw new NotImplementedException(); }

        // debug hooks
        public virtual int DebugString(string str) { throw new NotImplementedException(); }

        protected VuDriver(VuEntity entity) { throw new NotImplementedException(); }

        // Data
        protected VuEntity entity_;
    }

    //-----------------------------------------
    public abstract class VuDeadReckon : VuDriver
    {

        public VuDeadReckon(VuEntity entity):base(entity) { throw new NotImplementedException(); }
        //TODO public virtual ~VuDeadReckon();

        //public abstract int Type() { throw new NotImplementedException(); }

        //public abstract VU_ERRCODE Handle(VuEvent evnt) { throw new NotImplementedException(); }
        //public abstract VU_ERRCODE Handle(VuFullUpdateEvent evnt) { throw new NotImplementedException(); }
        //public abstract VU_ERRCODE Handle(VuPositionUpdateEvent evnt) { throw new NotImplementedException(); }

        public override void AlignTimeAdd(VU_TIME timeDelta) { throw new NotImplementedException(); }
        public override void AlignTimeSubtract(VU_TIME timeDelta) { throw new NotImplementedException(); }
        public override void ResetLastUpdateTime(VU_TIME time) { throw new NotImplementedException(); }

        public override void NoExec(VU_TIME timestamp) { throw new NotImplementedException(); }
        public override void Exec(VU_TIME timestamp) { throw new NotImplementedException(); }
        public override void AutoExec(VU_TIME timestamp) { throw new NotImplementedException(); }

        public virtual void ExecDR(VU_TIME timestamp) { throw new NotImplementedException(); }

        // DATA

        // dead reckoning
        protected BIG_SCALAR drx_, dry_, drz_;
        protected SM_SCALAR drxdelta_, drydelta_, drzdelta_;
#if VU_USE_QUATERNION
  protected VU_QUAT drquat_;	// quaternion indicating current facing
  protected VU_VECT drquatdelta_;	// unit vector expressing quaternion delta
  protected SM_SCALAR drtheta_;	// scalar indicating rate of above delta
#else // !VU_USE_QUATERNION
        protected SM_SCALAR dryaw_, drpitch_, drroll_;
        //SM_SCALAR dryawdelta_, drpitchdelta_, drrolldelta_;
#endif

        protected VU_TIME lastUpdateTime_;
    }

    //-----------------------------------------
    public abstract class VuMaster : VuDeadReckon
    {

        public VuMaster(VuEntity entity) : base(entity){ throw new NotImplementedException(); }
        //TODO public virtual ~VuMaster();

        public override int Type() { throw new NotImplementedException(); }
        public override void NoExec(VU_TIME timestamp) { throw new NotImplementedException(); }
        public override void Exec(VU_TIME timestamp) { throw new NotImplementedException(); }

        public int CheckTolerance() { throw new NotImplementedException(); }
        public SM_SCALAR CalcError() { throw new NotImplementedException(); }
        public SM_SCALAR CheckForceUpd(VuSessionEntity session)//me123 010115
        { throw new NotImplementedException(); }
        public VU_TIME lastFinePositionUpdateTime() { return lastFinePositionUpdateTime_; }
        public void SetpendingUpdate(VU_BOOL pendingUpdate) { pendingUpdate_ = pendingUpdate; }//me123
        public void SetpendingRoughUpdate(VU_BOOL pendingRoughUpdate) { pendingRoughUpdate_ = pendingRoughUpdate; }//me123
        public void UpdateDrdata(VU_BOOL registrer)//me123
        { throw new NotImplementedException(); }
        public void SetRoughPosTolerance(SM_SCALAR x, SM_SCALAR y, SM_SCALAR z) { throw new NotImplementedException(); }
        public void SetPosTolerance(SM_SCALAR x, SM_SCALAR y, SM_SCALAR z) { throw new NotImplementedException(); }
#if VU_USE_QUATERNION
  public void SetQuatTolerance(SM_SCALAR tol);
#else // !VU_USE_QUATERNION
        public void SetRotTolerance(SM_SCALAR yaw, SM_SCALAR pitch, SM_SCALAR roll) { throw new NotImplementedException(); }
#endif

        // ExecModel must update Pos, YPR & deltas
        //   returns whether model was run
        public abstract VU_BOOL ExecModel(VU_TIME timestamp);

        public void ExecModelWithDR() { throw new NotImplementedException(); }

        public override VU_ERRCODE Handle(VuEvent evnt) { throw new NotImplementedException(); }
        public override VU_ERRCODE Handle(VuFullUpdateEvent evnt) { throw new NotImplementedException(); }
        public override VU_ERRCODE Handle(VuPositionUpdateEvent evnt) { throw new NotImplementedException(); }

        public virtual VU_ERRCODE GeneratePositionUpdate(int oob, VU_TIME timestamp, VU_BOOL registrer, VuTargetEntity target) { throw new NotImplementedException(); }

        // debug hooks
        public override int DebugString(string str) { throw new NotImplementedException(); }

        // DATA

        protected VU_TIME lastPositionUpdateTime_;
        protected VU_TIME lastFinePositionUpdateTime_;
        // dead reckoning tolerances
        protected VU_BOOL pendingUpdate_;
        protected VU_BOOL pendingRoughUpdate_;
        protected BIG_SCALAR xtol_, ytol_, ztol_;
#if VU_USE_QUATERNION
  protected SM_SCALAR quattol_;
#else // !VU_USE_QUATERNION
        protected SM_SCALAR yawtol_, pitchtol_, rolltol_;
#endif
    }

    //-----------------------------------------
    public class VuSlave : VuDeadReckon
    {
        protected enum DeadReckonMode
        {
            ROUGH,
            TRANSITION,
            FINE,
        }

        public VU_TIME lastPositionUpdateTime_;
        public VU_TIME lastFinePositionUpdateTime_;
        public VU_BOOL pendingUpdate_;
        public VU_BOOL pendingRoughUpdate_;
        public int CheckTolerance() { throw new NotImplementedException(); }



        public VU_TIME lastFinePositionUpdateTime() { return lastFinePositionUpdateTime_; }
        public SM_SCALAR CheckForceUpd(VuSessionEntity sess)//me123 010115
        { throw new NotImplementedException(); }
        public SM_SCALAR CalcError() { throw new NotImplementedException(); }
        public void SetpendingUpdate(VU_BOOL pendingUpdate) { pendingUpdate_ = pendingUpdate; }//me123
        public void SetpendingRoughUpdate(VU_BOOL pendingRoughUpdate) { pendingRoughUpdate_ = pendingRoughUpdate; }//me123
        public void UpdateDrdata(VU_BOOL registrer)//me123
        { throw new NotImplementedException(); }
        public virtual VU_ERRCODE GeneratePositionUpdate(int oob, VU_TIME timestamp, VU_BOOL registrer, VuTargetEntity target) { throw new NotImplementedException(); }
        public VuSlave(VuEntity entity) :base(entity) { throw new NotImplementedException(); }
        //TODO public virtual ~VuSlave();
        public override int Type() { throw new NotImplementedException(); }
        public BIG_SCALAR LinearError(BIG_SCALAR value, BIG_SCALAR truevalue) { throw new NotImplementedException(); }
        public SM_SCALAR SmoothLinear(BIG_SCALAR value,
                BIG_SCALAR truevalue, SM_SCALAR truedelta, SM_SCALAR timeInverse) { throw new NotImplementedException(); }
#if VU_USE_QUATERNION
  public SM_SCALAR QuatError(VU_QUAT value, VU_QUAT truevalue);
#else // !VU_USE_QUATERNION
        public SM_SCALAR AngleError(SM_SCALAR value, SM_SCALAR truevalue) { throw new NotImplementedException(); }
        public SM_SCALAR SmoothAngle(SM_SCALAR value, SM_SCALAR truevalue,
                      SM_SCALAR truedelta, SM_SCALAR timeInverse) { throw new NotImplementedException(); }
#endif

        public override void NoExec(VU_TIME timestamp) { throw new NotImplementedException(); }
        public override void Exec(VU_TIME timestamp) { throw new NotImplementedException(); }
        public override void AutoExec(VU_TIME timestamp) { throw new NotImplementedException(); }

        public override VU_ERRCODE Handle(VuEvent evnt) { throw new NotImplementedException(); }
        public override VU_ERRCODE Handle(VuFullUpdateEvent evnt) { throw new NotImplementedException(); }
        public override VU_ERRCODE Handle(VuPositionUpdateEvent evnt) { throw new NotImplementedException(); }

        // debug hooks
        public override int DebugString(string str) { throw new NotImplementedException(); }

        protected virtual VU_BOOL DoSmoothing(VU_TIME lookahead, VU_TIME timestamp) { throw new NotImplementedException(); }

        // DATA

        // smoothing
        protected BIG_SCALAR truex_, truey_, truez_;
        protected SM_SCALAR truexdelta_, trueydelta_, truezdelta_;
#if VU_USE_QUATERNION
  protected  VU_QUAT truequat_;	// quaternion indicating current facing
  protected  VU_VECT truequatdelta_; // unit vector expressing quaternion delta
  protected  SM_SCALAR truetheta_;	// scalar indicating rate of above delta
#else // !VU_USE_QUATERNION
        protected SM_SCALAR trueyaw_, truepitch_, trueroll_;
        //SM_SCALAR trueyawdelta_, truepitchdelta_, truerolldelta_;
#endif
        protected VU_TIME lastSmoothTime_;
        protected VU_TIME lastRemoteUpdateTime_;
    }

    //-----------------------------------------
    public class VuDelaySlave : VuSlave
    {
        public VuDelaySlave(VuEntity entity) :base(entity) { throw new NotImplementedException(); }
        //TODO public virtual ~VuDelaySlave();

        public override int Type() { throw new NotImplementedException(); }

        public override void NoExec(VU_TIME timestamp) { throw new NotImplementedException(); }
        public override void Exec(VU_TIME timestamp) { throw new NotImplementedException(); }
        public override void AutoExec(VU_TIME timestamp) { throw new NotImplementedException(); }

        public override VU_ERRCODE Handle(VuPositionUpdateEvent evnt) { throw new NotImplementedException(); }

        protected override VU_BOOL DoSmoothing(VU_TIME lookahead, VU_TIME timestamp) { throw new NotImplementedException(); }

        // DATA
        protected SM_SCALAR ddrxdelta_, ddrydelta_, ddrzdelta_;        // accel values
    }
}
