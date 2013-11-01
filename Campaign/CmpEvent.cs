using FalconNet.FalcLib;
using FalconNet.VU;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FalconNet.Campaign
{
    public class EventClass
    {
        public const int CE_ONETIME = 0x01; // Set if this is a onetime only event
        public const int CE_FIRED = 0x08; // This event has been used

        public short event_; // The Event ID 
        public short flags; // event flags

        public EventClass(short id)
        {
            event_ = id;
            flags = 0;
        }

        public EventClass(Stream stream)
        {
            throw new NotImplementedException();
        }

        //TODO public ~EventClass(void);
        public int Save(Stream fp)
        {
            throw new NotImplementedException();
        }

        public void DoEvent()
        {
            SetEvent(true);
            // MonoPrint("CampEvent: Event %d activated.\n", event);
        }

        public void SetEvent(bool status)
        {
            CampEventDataMessage msg = new CampEventDataMessage(VUSTATIC.vuLocalSession, FalconSessionEntity.FalconLocalGame);
            msg.message = eventMsgType.eventMessage;
            msg.event_ = event_;

            if (status)
            {
                flags |= CE_FIRED;
                msg.status = true;
            }
            else
            {
                flags &= ~CE_FIRED;
                msg.status = false;
            }

            FalcMesgStatic.FalconSendMessage(msg, true);
        }

        public int HasFired()
        {
            return flags & CE_FIRED;
        }
    }

    public static class CmpEvent
    {
        public static int CheckTriggers(string filename)
        { throw new NotImplementedException();
        }

        public static int NewCampaignEvents(string filename)
        {
            throw new NotImplementedException();
        }

        public static int LoadCampaignEvents(string filename, string scenario)
        {
            throw new NotImplementedException();
        }

        public static int SaveCampaignEvents(string filename)
        {
            throw new NotImplementedException();
        }

        public static void DisposeCampaignEvents()
        {
            throw new NotImplementedException();
        }
    }
}
