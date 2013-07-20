using FalconNet.Common.Encoding;
using FalconNet.VU;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VU_MSG_TYPE = System.Byte;

namespace FalconNet.FalcLib
{
    /*
     * Message Type Unit Message
     */
    public class FalconUnitMessage : FalconEvent
    {

        public enum UnitMsgType
        {
            unitDetach,
            unitSupply,
            unitNewOrders,
            unitRequestMet,
            unitScheduleAC,
            unitSchedulePilots,
            unitSetVehicles,
            unitActivate,
            unitSupport,
            unitRatings,
            unitStatistics,
            unitScramble
        };

        public FalconUnitMessage(VU_ID entityId, VuTargetEntity target, bool loopback = true) :
            base((byte)FalconMsgID.UnitMsg, HandlingThread.CampaignThread, entityId, target, loopback)
        {

            // Your Code Goes Here
        }

        public FalconUnitMessage(VU_MSG_TYPE type, VU_ID senderid, VU_ID target) :
            base((byte)FalconMsgID.UnitMsg, HandlingThread.CampaignThread, senderid, target)
        { }

#if TODO
    ~FalconUnitMessage(void);
    virtual int Size() const
    {
        return sizeof(dataBlock) + FalconEvent::Size();
    };
    //sfr: changed long*
    int Decode(VU_BYTE **buf, long *rem)
    {
        long init = *rem;

        FalconEvent::Decode(buf, rem);
        memcpychk(&dataBlock, buf, sizeof(dataBlock), rem);
        return init - *rem;
    }
    int Encode(VU_BYTE **buf)
    {
        int size;

        size = FalconEvent.Encode(buf);
        memcpy(*buf, &dataBlock, sizeof(dataBlock));
        *buf += sizeof(dataBlock);
        size += sizeof(dataBlock);
        return size;
    }
#endif
        public class DATA_BLOCK
        {

            public VU_ID from;
            public short message;
            public short data1;
            public short data2;
            public short data3;
        }
        public DATA_BLOCK dataBlock;

        protected override VU_ERRCODE Process(bool autodisp)
        {
            throw new NotImplementedException();
        }
    }

    public static class FalconUnitMessageEncodingLE
    {
        public static void Encode(ByteWrapper buffer, FalconUnitMessage val)
        {
            throw new NotImplementedException();
        }
        public static void Encode(Stream stream, FalconUnitMessage val)
        {
            throw new NotImplementedException();
        }

        public static FalconUnitMessage Decode(ByteWrapper buffer)
        {
            throw new NotImplementedException();
        }
        public static FalconUnitMessage Decode(Stream stream)
        {
            throw new NotImplementedException();
        }

        public static int Size
        {
            get { throw new NotImplementedException(); }
        }
    }

}
