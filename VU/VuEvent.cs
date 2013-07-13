using System;

namespace FalconNet.VU
{

    public static class VU_MSG_DEF
    {
        public const int VU_UNKNOWN_MESSAGE = 0;

        // error message
        public const int VU_ERROR_MESSAGE = 1;		// 0x00000002

        // request messages
        public const int VU_GET_REQUEST_MESSAGE = 2;		// 0x00000004
        public const int VU_PUSH_REQUEST_MESSAGE = 3;		// 0x00000008
        public const int VU_PULL_REQUEST_MESSAGE = 4;		// 0x00000010

        // internal events
        public const int VU_TIMER_EVENT = 5;		// 0x00000020
        public const int VU_RELEASE_EVENT = 6;		// 0x00000040

        // event messages
        public const int VU_DELETE_EVENT = 7;		// 0x00000080
        public const int VU_UNMANAGE_EVENT = 8;		// 0x00000100
        public const int VU_MANAGE_EVENT = 9;		// 0x00000200
        public const int VU_CREATE_EVENT = 10;		// 0x00000400
        public const int VU_SESSION_EVENT = 11;		// 0x00000800
        public const int VU_TRANSFER_EVENT = 12;		// 0x00001000
        public const int VU_BROADCAST_GLOBAL_EVENT = 13;		// 0x00002000
        public const int VU_POSITION_UPDATE_EVENT = 14;		// 0x00004000
        public const int VU_FULL_UPDATE_EVENT = 15;		// 0x00008000
        public const int VU_RESERVED_UPDATE_EVENT = 16;		// 0x00010000 ***
        public const int VU_ENTITY_COLLISION_EVENT = 17;		// 0x00020000 ***
        public const int VU_GROUND_COLLISION_EVENT = 18;		// 0x00040000 ***

        // shutdown event
        public const int VU_SHUTDOWN_EVENT = 19;		// 0x00080000

        // latency/timing message
        public const int VU_TIMING_MESSAGE = 20;		// 0x00100000
        public const int VU_REQUEST_DUMMY_BLOCK_MESSAGE = 21;
        public const int VU_LAST_EVENT = 21;
    }

    public enum VUBITS : uint
    {
        VU_VU_MESSAGE_BITS = 0x001ffffe,
        VU_REQUEST_MSG_BITS = 0x0000001c,
        VU_VU_EVENT_BITS = 0x000fffe0,
        VU_DELETE_EVENT_BITS = 0x000800c0,
        VU_CREATE_EVENT_BITS = 0x00000600,
        VU_TIMER_EVENT_BITS = 0x00000020,
        VU_INTERNAL_EVENT_BITS = 0x00080060,
        VU_EXTERNAL_EVENT_BITS = 0x0017ff82,
        VU_USER_MESSAGE_BITS = 0xffe00000
    }

    public enum VU_MSG_FLAG
    {
        // message flags
        VU_NORMAL_PRIORITY_MSG_FLAG = 0x01,	// send normal priority
        VU_OUT_OF_BAND_MSG_FLAG = 0x02,	// send unbuffered
        VU_KEEPALIVE_MSG_FLAG = 0x04,	// this is a keepalive msg
        VU_RELIABLE_MSG_FLAG = 0x08,	// attempt to send reliably
        VU_LOOPBACK_MSG_FLAG = 0x10,	// post msg to self as well
        VU_REMOTE_MSG_FLAG = 0x20,	// msg came from outside
        VU_SEND_FAILED_MSG_FLAG = 0x40,	// msg has been sent
        VU_PROCESSED_MSG_FLAG = 0x80	// msg has been processed
    }

    public enum VUERROR
    {
        VU_UNKNOWN_ERROR = 0,
        VU_NO_SUCH_ENTITY_ERROR = 1,
        VU_CANT_MANAGE_ENTITY_ERROR = 2,	// for push request denial
        VU_DONT_MANAGE_ENTITY_ERROR = 3,	// for pull request denial
        VU_CANT_TRANSFER_ENTITY_ERROR = 4,	// for non-transferrable ents
        VU_TRANSFER_ASSOCIATION_ERROR = 5,	// for association errors
        VU_NOT_AVAILABLE_ERROR = 6	// session too busy or exiting
    }

    // timer types
    public enum TIMERTYPES
    {
        VU_UNKNOWN_TIMER = 0,
        VU_DELETE_TIMER = 1,
        VU_DELAY_TIMER = 2
    }

    // session event subtypes
    public enum vuEventTypes
    {
        VU_SESSION_UNKNOWN_SUBTYPE = 0,
        VU_SESSION_CLOSE,
        VU_SESSION_JOIN_GAME,
        VU_SESSION_CHANGE_GAME,
        VU_SESSION_JOIN_GROUP,
        VU_SESSION_LEAVE_GROUP,
        VU_SESSION_CHANGE_CALLSIGN,
        VU_SESSION_DISTRIBUTE_ENTITIES,
        VU_SESSION_TIME_SYNC,
        VU_SESSION_LATENCY_NOTICE,
    }

    public enum VU_SPECIAL_GET_TYPE
    {
        VU_GET_GAME_ENTS,
        VU_GET_GLOBAL_ENTS
    }


#if VU_SIMPLE_LATENCY
//--------------------------------------------------
class VuTimingMessage : public VuMessage {
public:
  VuTimingMessage(VU_ID entityId, VuTargetEntity *target, VU_BOOL loopback=false);
  VuTimingMessage(VU_ID senderid, VU_ID target);
  virtual ~VuTimingMessage();

  virtual int Size();
  virtual int Decode(VU_BYTE **buf, int length);
  virtual int Encode(VU_BYTE **buf);

protected:
  virtual VU_ERRCODE Process(VU_BOOL autod);

// data
public:
	VU_TIME	sessionRealSendTime_;
	VU_TIME sessionGameSendTime_;
	VU_TIME remoteGameTime_;
};
#endif


}