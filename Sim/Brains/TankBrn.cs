using FalconNet.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.Sim
{
public enum BOOM {
	BOOM_AZIMUTH,
	BOOM_ELEVATION,
	BOOM_EXTENSION,
}

public struct BoomData {
	public float			rx;				//relative to tanker center in ft
	public float			ry;				//relative to tanker center in ft
	public float			rz;				//relative to tanker center in ft
	public DrawableBSP	drawPointer;	//pointer to our boom
	public float			az;				//in radians
	public float			el;				//in radians
	public float			ext;			//how far boom is extended in ft
}
#if TODO
public class TankerBrain : DigitalBrain
{

	public enum TnkrType{
		TNKR_KC10,
		TNKR_KC135,
		TNKR_IL78,
		TNKR_UNKNOWN
	}

   
      private TailInsertList *thirstyQ;
	  private HeadInsertList *waitQ;
      private SimVehicleClass* curThirsty;
	  private int		numBooms;
	  private DrawableBSP*	rack[3];
	  private BoomData	boom[3];
	  private TnkrType	type;	
      private int flags;
      private float holdAlt;
      private SimObjectType* tankingPtr;
      private long lastStabalize;
      private long lastBoomCommand;
	  private int turnallow;	// M.N.
	  private int directionsetup; // M.N.
	  private int HeadsUp; // MN
	  private float desSpeed; // MN
	  private vector TrackPoints[4]; // 2002-03-13 MN
	  private int currentTP; // 2002-03-13 MN
	  private bool advancedirection; // if we go from TrackPoint 0.1.2.3 or 0.3.2.1 (latter is the case if tanker is outside min max tanker range envelope - would do a 180° turn in the other case)
	  private bool reachedFirstTrackpoint; // when we reached the first trackpoint, we limit rStick and pStich in wingmnvers.cpp
	  private float trackPointDistance; // contains closest distance at < 5 nm trackpoint distance

      private void CallNext ();
      private void DriveBoom ();
      private void DriveLights ();
      private void BreakAway ();
      private void TurnTo (float newHeading);
      private void FollowThirsty ();
	  private void TurnToTrackPoint (int trackPoint); // 2002-03-13 MN
      

   
	   
   public    enum TankerFlags {
         IsRefueling = 0x1,
         PatternDefined = 0x2,
         GivingGas = 0x4,
         PrecontactPos = 0x10,
         ContactPos = 0x20,
		 ClearingPlane = 0x40};
      public TankerBrain (AircraftClass myPlatform, AirframeClass myAf);
      public virtual ~TankerBrain ();
      public void FrameExec(SimObjectType objtype1, SimObjectType objtype1);
      public int  AddToQ (SimVehicleClass thirstyOne);
      public void RemoveFromQ (SimVehicleClass thirstyOne);
	  public int AddToWaitQ (SimVehicleClass doneOne);
	  public void PurgeWaitQ ();
	  public int TankingPosition(SimVehicleClass thirstyOne);
	  public void DoneRefueling ();
	  public bool IsSet(int flag)				{return (flags & flag) != 0 && true;}
	  public SimObjectType TankingPtr()	{return tankingPtr;}

	  public void OptTankingPosition(Tpoint pos);
	  public void BoomWorldPosition(Tpoint pos);
	  public void ReceptorRelPosition(Tpoint pos, SimVehicleClass thirsty);
	  public void BoomTipPosition(Tpoint pos);
	  public virtual bool IsTanker()	{return true;}
	  public virtual void InitBoom();
	  public virtual void CleanupBoom();
	  public void SetInitial()	{ turnallow = 0; HeadsUp = 0; directionsetup = 1; currentTP = 0; }
	  public float GetDesSpeed() {return desSpeed; }
	  public bool ReachedFirstTrackPoint() {return reachedFirstTrackpoint; }

#if USE_SH_POOLS
public:
	// Overload new/delete because our parent class does (and assumes a fixed size)
	void *operator new(size_t size) { return MemAllocPtr(pool, size, 0); };
	void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
#endif
}
#endif
}
