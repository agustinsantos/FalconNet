using System;

using BIG_SCALAR=System.Single;
//typedef float BIG_SCALAR;
using SM_SCALAR=System.Single;
using VU_DAMAGE= System.UInt64;
using VU_BYTE=System.Byte;

using System.IO;//typedef float SM_SCALAR;

namespace FalconNet.VU
{  
	public struct VuEntityType 
	{
	  public ushort id_;
	  public ushort collisionType_;
	  public SM_SCALAR collisionRadius_;
	  public VU_BYTE[] classInfo_; //[CLASS_NUM_BYTES];
	  public VU_TIME updateRate_;
	  public VU_TIME updateTolerance_;
	  public SM_SCALAR fineUpdateRange_;	// max distance to send position updates
	  public SM_SCALAR fineUpdateForceRange_; // distance to force position updates
	  public SM_SCALAR fineUpdateMultiplier_; // multiplier for noticing position updates
	  public VU_DAMAGE damageSeed_;
	  public int hitpoints_;
	  public ushort majorRevisionNumber_;
	  public ushort minorRevisionNumber_;
	  public ushort createPriority_;
	  public byte managementDomain_;
	  public bool transferable_;
	  public bool private_;
	  public bool tangible_;
	  public bool collidable_;
	  public bool global_;
	  public bool persistent_;
	}
	
	//typedef SM_SCALAR VU_QUAT[4];
	//typedef SM_SCALAR VU_VECT[3];
	
	// shared data
	public struct ShareData
	{
		public ushort entityType_;	// id (table index)
	#if TODO
	    union {
	      ushort value_;
	      VuFlagBits breakdown_;
	    } flags_;
	#endif
		public ushort flags_;
		public VU_ID id_;
		public VU_ID ownerId_;	// owning session
		public VU_ID assoc_;	// id of ent which must be local to this ent
	}
	
	public struct OrientationData
	{
		#if VU_USE_QUATERNION
		    public VU_QUAT quat_;	// quaternion indicating current facing
		    public VU_VECT dquat_;	// unit vector expressing quaternion delta
		    public SM_SCALAR theta_;	// scalar indicating rate of above delta
		#else // !VU_USE_QUATERNION
		public SM_SCALAR yaw_, pitch_, roll_;
		public SM_SCALAR dyaw_, dpitch_, droll_;
		#endif
	}
	
	public struct PositionData
	{
		public BIG_SCALAR x_, y_, z_;
		public SM_SCALAR dx_, dy_, dz_;
	}
	
	public class VuEntity
	{
		public const int VU_UNKNOWN_ENTITY_TYPE			=0;
		public const int  VU_SESSION_ENTITY_TYPE			=1;
		public const int  VU_GROUP_ENTITY_TYPE			=2;
		public const int  VU_GLOBAL_GROUP_ENTITY_TYPE	        =3;
		public const int  VU_GAME_ENTITY_TYPE             	=4;
		public const int  VU_PLAYER_POOL_GROUP_ENTITY_TYPE	=5;
		public const int  VU_LAST_ENTITY_TYPE			=100;
		
		public const int  VU_CREATE_PRIORITY_BASE		=100;
		
		public VuEntity() {}
		
		public VuEntity (int t)
		{
		}

		public VuEntity (byte[] stream, ref int pos)
		{
		}

		public VuEntity (FileStream filePtr)
		{
		}
		
		public virtual int Save (byte[] stream, ref int pos)
		{
			throw new NotImplementedException();
		}
		
		public virtual int Save (FileStream filePtr)
		{
			throw new NotImplementedException();
		}
		
		public virtual int SaveSize ()
		{
			throw new NotImplementedException();
		}
		
		public float XPos ()
		{
			return pos_.x_;
		}

		public float YPos ()
		{
			return pos_.y_;
		}

		public float ZPos ()
		{
			return pos_.z_;
		}
		
		public void SetAssociation(VU_ID assoc) { 
			share_.assoc_ = assoc; 
		}
		
		public VU_ID Id()		
		{
			return share_.id_; 
		}
		public VuEntityType EntityType()
		{
			return entityTypePtr_; 
		}
		public bool  IsLocal()	
		{ 
			//TODO return (bool)((vuLocalSession == OwnerId()) ? true : false);
			throw new NotImplementedException();
		}

		protected ShareData share_;
		protected PositionData pos_;
		protected OrientationData orient_;
		protected VuEntityType entityTypePtr_;
	}
}