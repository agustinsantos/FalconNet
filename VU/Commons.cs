using System;

namespace FalconNet.VU
{
	public class VuGameEntity {

	}
	public class VuSessionEntity :VuEntity {
		public void SetVuState(byte newState) {
			vuState_ = newState; 
		}

		protected byte vuState_;
		protected object game_;
	}
	public struct VuThread {}
	public struct VuMainThread {}
	public struct VuFullUpdateEvent {}
	public struct VuPositionUpdateEvent {}
	public struct VuEntityCollisionEvent {}
	public struct VuTransferEvent {}
	public struct VuSessionEvent {}
	public struct VuEvent {}
	public class VuMaster {}
	public struct VU_ID {
		public static VU_ID FalconNullId = new VU_ID();
	}
	public class VU_TIME {}
    public struct VU_ERRCODE { }
	public class F4LIt {}
	public class VuListIterator {}
	public class VuTargetEntity {}
	public class VuGridIterator {}
	public class VuFilteredList {}
	public struct VuFullUpdateevnt {}
	public struct VuPositionUpdateevnt {}
	public struct VuTransferevnt {}
	public class VuDatabase 
	{
		public Object Find (VU_ID targetID)
		{
			throw new NotImplementedException ();
		}
		
		public static VuDatabase vuDatabase;
	}
}

