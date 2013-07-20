using FalconNet.Common.Encoding;
using FalconNet.VU;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FalconNet.FalcLib
{
    /*
     * Message Type Flight Plan Message
     */
    public class FalconFlightPlanMessage : FalconEvent
    {

        public enum DataType { waypointData, loadoutData, squadronStores };

        public FalconFlightPlanMessage(VU_ID entityId, VuTargetEntity target, bool loopback = true)
            : base((byte)FalconMsgID.FalconFlightPlanMsg, HandlingThread.CampaignThread, entityId, target, loopback)
        {
            dataBlock.data = null;
            dataBlock.size = 0;
            //me123 RequestOutOfBandTransmit ();
            RequestReliableTransmit();
        }
#if TODO
         public FalconFlightPlanMessage(VU_MSG_TYPE type, VU_ID senderid, VU_ID target);
         public ~FalconFlightPlanMessage( );
         virtual int Size() const;
         //sfr: changed to long *length
         //int Decode (VU_BYTE **buf, int length);
        public  virtual int Decode(VU_BYTE **buf, long *rem);
         public virtual int Encode(VU_BYTE **buf);
#endif

        public class DATA_BLOCK
        {

            public DataType type;
            public long size;
            public byte[] data;
        }
        public DATA_BLOCK dataBlock;


        protected override VU_ERRCODE Process(bool autodisp)
        {
            throw new NotImplementedException();
        }
    }
    public static class FalconFlightPlanMessageEncodingLE
    {
        public static void Encode(ByteWrapper buffer, FalconFlightPlanMessage val)
        {
            throw new NotImplementedException();
        }
        public static void Encode(Stream stream, FalconFlightPlanMessage val)
        {
            throw new NotImplementedException();
        }

        public static FalconFlightPlanMessage Decode(ByteWrapper buffer)
        {
            throw new NotImplementedException();
        }
        public static FalconFlightPlanMessage Decode(Stream stream)
        {
            throw new NotImplementedException();
        }

        public static int Size
        {
            get { throw new NotImplementedException(); }
        }
    }
}
