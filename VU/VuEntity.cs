using System;

using BIG_SCALAR=System.Single;
//typedef float BIG_SCALAR;
using SM_SCALAR=System.Single;
using System.IO;//typedef float SM_SCALAR;

namespace FalconNet.VU
{  
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

		protected ShareData share_;
		protected PositionData pos_;
		protected OrientationData orient_;
	}
}