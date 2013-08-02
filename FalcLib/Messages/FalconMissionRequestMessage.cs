using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FalconNet.VU;
using VU_MSG_TYPE = System.Byte;
using Team = System.SByte;

namespace FalconNet.FalcLib
{
    /*
     * Message Type Mission Request
     */
    public class FalconMissionRequestMessage : FalconEvent
    {

        public FalconMissionRequestMessage(VU_ID entityId, VuTargetEntity target, bool loopback = true)
            : base((byte)FalconMsgID.MissionRequestMsg, HandlingThread.CampaignThread, entityId, target, loopback)
        {
        }

        public FalconMissionRequestMessage(VU_MSG_TYPE type, VU_ID senderid, VU_ID target)
            : base((byte)FalconMsgID.MissionRequestMsg, HandlingThread.CampaignThread, senderid, target)
        {
        }

#if TODO
    //TODO public ~FalconMissionRequestMessage(void);
    virtual int Size() const
    {
        return sizeof(dataBlock) + FalconEvent::Size();
    };
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
       public  Team team;
       public MissionRequestClass request;

        protected override VU_ERRCODE Process(bool autodisp)
        {
            // F4Assert(vuCritical->count == 0);

            if (autodisp)
                return VU_ERRCODE.VU_ERROR;
#if TODO
            if (TeamStatic.TeamInfo[team] != null && TeamStatic.TeamInfo[team].atm != null && TeamStatic.TeamInfo[team].atm.IsLocal())
                TeamStatic.TeamInfo[team].atm.ProcessRequest(request);
#endif
            return VU_ERRCODE.VU_NO_OP;
        }
    }
}
