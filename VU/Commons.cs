using System;

namespace FalconNet.VU
{
    public enum VU_ERRCODE
    {
        VU_ERROR = -1,
        VU_NO_OP = 0,
        VU_SUCCESS = 1
    }

    public class VuGameEntity
    {

    }
    public class VuSessionEntity : VuEntity
    {
        public void SetVuState(byte newState)
        {
            vuState_ = newState;
        }

        protected byte vuState_;
        protected object game_;
    }
    public struct VuThread { }
    public struct VuMainThread { }
    public struct VuErrorMessage { }
    public struct VuPushRequest { }
    public struct VuPullRequest { }
    public struct VuFullUpdateEvent { }
    public struct VuPositionUpdateEvent { }
    public struct VuEntityCollisionEvent { }
    public struct VuTransferEvent { }
    public struct VuSessionEvent { }
    public struct VuEvent { }
    public class VuMaster { }
    public class VuFilter { }
    public class VuBiKeyFilter : VuFilter { }
    public class VuGridTree { }
    public class VU_TIME { }

    public class F4LIt { }
    public class VuListIterator { }
    public class VuTargetEntity { }
    public class VuGridIterator { }
    public class VuFilteredList { }
    public struct VuFullUpdateevnt { }
    public struct VuPositionUpdateevnt { }
    public struct VuTransferevnt { }
    public struct VuDriver { }
    public class VuMessage { }
    public class VuMessageFilter {}
    public class VuDatabase
    {
        public Object Find(VU_ID targetID)
        {
            throw new NotImplementedException();
        }

        public static VuDatabase vuDatabase;

        public VU_ERRCODE Remove(VuEntity ent)
        {
            throw new NotImplementedException();
        }
    }
}

