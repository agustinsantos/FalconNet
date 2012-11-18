using System;
using System.IO;
using FalconNet.VU;
using VU_MSG_TYPE=System.Byte;

namespace FalconNet.FalcLib
{
    public struct EventIdData
    {
        public ushort size;
        public ushort type;
    }

    public static class FalcMesgStatic
    {
        public static FileStream F4EventFile;
        // ==================================
        // Functions
        // ==================================

        public static void FalconSendMessage(VuMessage theEvent, bool reliableTransmit = false)
        { throw new NotImplementedException(); }
    }

    // ==================================
    // Falcon 4 Event stuff
    // ==================================
    public enum HandlingThread
    {
        NoThread = 0x0,				// This would be rather pointless
        SimThread = 0x1,
        CampaignThread = 0x2,
        UIThread = 0x4,
        VuThread = 0x8,			// Realtime thread! carefull with what you send here
        AllThreads = 0xff
    };
    public abstract class FalconEvent :   VuMessage
	{

		protected HandlingThread handlingThread;

        public virtual int Size()
        { throw new NotImplementedException(); }
        public virtual int Decode(byte[] buf, ref int pos, int length)
        { throw new NotImplementedException(); }
        public virtual int Encode(byte[] buf, ref int pos)
        { throw new NotImplementedException(); }


        protected FalconEvent(FalconMsgID type, HandlingThread threadID, VU_ID entityId, VuTargetEntity target, bool loopback = true)
        { throw new NotImplementedException(); }
        protected FalconEvent(FalconMsgID type, HandlingThread threadID, VU_ID senderid, VU_ID target)
        { throw new NotImplementedException(); }
		//TODO protected virtual ~FalconEvent ();
        protected virtual int Activate(VuEntity ent)
        { throw new NotImplementedException(); }
        protected abstract int Process(byte autodisp);

        protected virtual int LocalSize()
        { throw new NotImplementedException(); }
	} 

    // ==================================
    // Falcon 4 Message filter
    // ==================================
    public class FalconMessageFilter :  VuMessageFilter
	{
	
		// TODO private FalconEvent::HandlingThread filterThread;
		private ulong vuFilterBits;

	
		//TODO public FalconMessageFilter(FalconEvent::HandlingThread theThread, ulong vuMessageBits);
		//TODO public virtual ~FalconMessageFilter();
        public virtual bool Test(VuMessage evnt)
        { throw new NotImplementedException(); }
        public virtual VuMessageFilter Copy()
        { throw new NotImplementedException(); }
	} 
}
