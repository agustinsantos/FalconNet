using FalconNet.FalcLib;
using System;

namespace FalconNet.Sim
{
    public enum FireFlag
    {
        GunFireFlag = 0x1,
        MslFireFlag = 0x2
    }


    public abstract class BaseBrain
    {
        private FireFlag flags;
        private int skillLevel;
        public SimObjectType targetPtr;
        public SimObjectType lastTarget;
        public SimObjectLocalData targetData;
        public int isWing;
        public float pStick, rStick, yPedal, throtl;

        public BaseBrain()
        {
            targetPtr = null;
            targetData = null;
            flags = 0;
            isWing = 0;
            skillLevel = 0;
        }

        //TODO public virtual ~BaseBrain ();


        public abstract void ReceiveOrders(FalconEvent evnt);
        public abstract void JoinFlight();
        public abstract void SetLead(int l);
        public abstract void FrameExec(SimObjectType objtype1, SimObjectType objtype2);
        public abstract void PostInsert();

        public virtual void Sleep()
        {
            ClearTarget();
        }

        public void SetTarget(SimObjectType newTarget)
        {
#if TODO
            if (newTarget == targetPtr)
                return;

            ClearTarget();
            if (newTarget != null)
            {
                //Debug.Assert(newTarget.BaseData() != (FalconEntity)0xDDDDDDDD);
                newTarget.Reference();
                targetData = newTarget.localData;
            }
            targetPtr = newTarget;
#endif
            throw new NotImplementedException();
        }

        public void ClearTarget()
        {
#if TODO
            if (targetPtr != null)
                targetPtr.Release();
            targetPtr = null;
            targetData = null;
#endif
            throw new NotImplementedException();
        }
        public void SetFlag(FireFlag val) { flags |= val; }
        public void ClearFlag(FireFlag val) { flags &= ~val; }
        public bool IsSetFlag(FireFlag val) { return ((flags & val) != 0 ? true : false); }
        public int SkillLevel() { return skillLevel; }
        public void SetSkill(int newLevel) { skillLevel = newLevel; }

        public virtual bool IsTanker() { return false; }
        public virtual void InitBoom() { }
        public virtual void CleanupBoom() { }
    }

    public class FeatureBrain
    {
        public FeatureBrain()
        {
        }

        //TODO public  virtual ~FeatureBrain();

        public virtual void Exec() { }
    }
}
