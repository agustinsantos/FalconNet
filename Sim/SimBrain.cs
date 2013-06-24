using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.Sim
{
public class BaseBrain
{
   
      private int flags;
      private int skillLevel;

   
      public enum FireFlag
      {
         GunFireFlag = 0x1,
         MslFireFlag = 0x2
      }
      public BaseBrain ()
          {
	targetPtr = null;
	targetData = NULL;
	flags = 0;
   isWing = 0;
   skillLevel = 0;
}
      //TODO public virtual ~BaseBrain ();
      public SimObjectType targetPtr;
      public SimObjectType* lastTarget;
      public SimObjectLocalData* targetData;
      public int isWing;
      public float pStick, rStick, yPedal, throtl;
      public virtual void ReceiveOrders (FalconEvent*) {}
      public virtual void JoinFlight () {}
      public virtual void SetLead (int) {}
      public virtual void FrameExec(SimObjectType*, SimObjectType*) {}
      public virtual void PostInsert () {}
      public virtual void Sleep () {ClearTarget();}
      public void SetTarget (SimObjectType* newTarget)
          {
	if (newTarget == targetPtr)
		return;

	ClearTarget();
	if (newTarget)
		{
        ShiAssert( newTarget->BaseData() != (FalconEntity*)0xDDDDDDDD );
		newTarget->Reference( SIM_OBJ_REF_ARGS );
		targetData = newTarget->localData;
		}
	targetPtr = newTarget;
}

      public void ClearTarget ()
          {
	if (targetPtr)
		targetPtr->Release( SIM_OBJ_REF_ARGS );
	targetPtr = NULL;
    targetData = NULL;
}
      public void SetFlag (int val) {flags |= val;}
      public void ClearFlag (int val) {flags &= ~val;}
      public int IsSetFlag (int val) {return (flags & val ? TRUE : FALSE);}
      public int SkillLevel() {return skillLevel;}
      public void SetSkill (int newLevel) {skillLevel = newLevel;}

	  public virtual int	IsTanker()	{return FALSE;}
	  public virtual void	InitBoom()	{}
	  public virtual void CleanupBoom() {}
}

public class FeatureBrain
{
     public  FeatureBrain (){
}

     //TODO public  virtual ~FeatureBrain();

     public  virtual void Exec() {}
}
}
