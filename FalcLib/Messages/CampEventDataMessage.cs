using FalconNet.VU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VU_MSG_TYPE = System.Byte;
using VU_BOOL = System.Boolean;

namespace FalconNet.FalcLib
{

    public enum eventMsgType
    {
        eventMessage,
        victoryConditionMessage,
        playMovie
    }

    public class CampEventDataMessage : FalconEvent
    {

        public CampEventDataMessage(VU_ID entityId, VuTargetEntity target, VU_BOOL loopback = true) :
            base((VU_MSG_TYPE)FalconMsgID.CampEventDataMsg, HandlingThread.CampaignThread, entityId, target, loopback)
        {
            throw new NotImplementedException();
        }

        public CampEventDataMessage(VU_MSG_TYPE type, VU_ID senderid, VU_ID target)
            :  base((VU_MSG_TYPE)FalconMsgID.CampEventDataMsg, HandlingThread.CampaignThread, senderid, target)
        {
            throw new NotImplementedException();
        }

        //TODO  public ~CampEventDataMessage(void);
#if TODO
    virtual int Size()  
    {
        return sizeof(dataBlock) + FalconEvent::Size();
    }
    //sfr: long *
    int Decode(VU_BYTE **buf, long *rem)
    {
        long init = *rem;

        FalconEvent::Decode(buf, rem);
        memcpychk(&dataBlock, buf, sizeof(dataBlock), rem);
        return init - *rem;
    };
    int Encode(VU_BYTE **buf)
    {
        int size;

        size = FalconEvent::Encode(buf);
        memcpy(*buf, &dataBlock, sizeof(dataBlock));
        *buf += sizeof(dataBlock);
        size += sizeof(dataBlock);
        return size;
    };
#endif

        public eventMsgType message;
        public short event_;
        public bool status;

        protected override VU_ERRCODE Process(bool autodisp)
        {
            throw new NotImplementedException();
        }

    }
}
