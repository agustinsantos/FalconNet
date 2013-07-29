using System;
using System.IO;
using FalconNet.VU;
using VU_MSG_TYPE = System.Byte;
using FalconNet.Common.Encoding;

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
    }


    public abstract class FalconEvent : VuMessage
    {
        public virtual int Size()
        { throw new NotImplementedException(); }
        public virtual int Decode(byte[] buf, ref int pos, int length)
        { throw new NotImplementedException(); }
        public virtual int Encode(byte[] buf, ref int pos)
        { throw new NotImplementedException(); }


        protected FalconEvent(VU_MSG_TYPE type, HandlingThread threadID, VU_ID entityId, VuTargetEntity target, bool loopback = true) :
            base(type, entityId, target, false)
        {
            // KCK NOTE: VU will not send a message to ourselves unless loopback is set to false - regardless of the target.
            // Also, it will send to ourselves if loopback is set to true - again, regarless of the target.
            // So we'd better be sure to set it correctly.
            // I'd suggest always setting loopback to true unless you know you don't want to get the message,
            // because I'll explicitly set it to FALSE here if you're not included in the target.
            if (loopback)
            {
                // if (!target && !FalconLocalGame)
                // RequestLoopback();
                if (target == null)
                    loopback = false;
                else if (target.IsGroup() && !((VuGroupEntity)target).SessionInGroup((FalconSessionEntity)VUSTATIC.vuLocalSessionEntity))
                    loopback = false;
                else if (target.IsSession() && target != VUSTATIC.vuLocalSessionEntity)
                    loopback = false;
                else
                    RequestLoopback();
            }

            handlingThread = threadID;
        }

        protected FalconEvent(VU_MSG_TYPE type, HandlingThread threadID, VU_ID senderid, VU_ID target) :
            base(type, senderid, target)
        {
            handlingThread = threadID;
        }

        //TODO protected virtual ~FalconEvent ();
        protected override VU_ERRCODE Activate(VuEntity theEntity)
        {
#if TODO
            byte[] buffer;
            int savePos;
            EventIdData idData;

            base.Activate(theEntity);

    // Only record sim events to disk if ACMI is recording
    if (F4EventFile && gACMIRec.IsRecording() && handlingThread == SimThread && Type() != ControlSurfaceMsg)
    {
        idData.size = (ushort)Size();
        idData.type = Type();
        buffer = new byte[idData.size];
        savePos = 0;
        Encode(&buffer);
        fwrite(&idData, sizeof(EventIdData), 1, F4EventFile);
        fwrite(savePos, idData.size, 1, F4EventFile);
        delete [] savePos;
    }
#endif
            return 0;
        }

        //protected abstract int Process(bool autodisp);

        protected virtual int LocalSize()
        { throw new NotImplementedException(); }


        internal HandlingThread handlingThread;
    }

    // ==================================
    // Falcon 4 Message filter
    // ==================================
    public class FalconMessageFilter : VuMessageFilter
    {

        public FalconMessageFilter(HandlingThread theThread, bool processVu_ = false)
        {
            filterThread = theThread;
            processVu = processVu_;
        }
        //TODO public virtual ~FalconMessageFilter();
        public override bool Test(VuMessage evnt)
        {
            bool retval = true;

            if (evnt.Type() > VU_MSG_DEF.VU_LAST_EVENT)
            {
                // This is a Falcon Event
                if ((((FalconEvent)evnt).handlingThread & filterThread) == 0)
                {
                    // message not intended for this thread
                    retval = false;
                }
            }
            else
            {


                // This is a Vu Event. Compare vs filter bits
                // sfr: fixed shift adding -1
                if (!processVu)
                {
                    retval = false;
                }

            }

            return (retval);
        }
        public override VuMessageFilter Copy()
        {
            return new FalconMessageFilter(filterThread, processVu);
        }

        private HandlingThread filterThread;
        private bool processVu;
    }

    public static class FalconEventEncodingLE
    {
        public static void Encode(Stream stream, FalconEvent val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, ref vuEventTypes rst)
        {
            throw new NotImplementedException();
        }

        public static int Size
        {
            get { throw new NotImplementedException(); }
        }
    }


}