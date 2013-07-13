using System;
using FalconNet.VU;
using VU_MSG_TYPE = System.Byte;

namespace FalconNet.FalcLib
{
    /*
     * Machine Generated include file for message "Objective Message".
     * NOTE: This file is read only. DO NOT ATTEMPT TO MODIFY IT BY HAND.
     * Generated on 28-August-1997 at 17:24:21
     * Generated from file EVENTS.XLS by MicroProse
     */

    /*
     * Message Type Objective Message
     */
    public class FalconObjectiveMessage : FalconEvent
    {

        public enum ObjMessage
        {
            objCaptured,
            objSetSupply,
            objSetLosses
        }

        public FalconObjectiveMessage(VU_ID entityId, VuTargetEntity target, bool loopback = true) :
            base((VU_MSG_TYPE)FalconMsgID.ObjectiveMsg, HandlingThread.CampaignThread, entityId, target, loopback)
        { }

        public FalconObjectiveMessage(VU_MSG_TYPE type, VU_ID senderid, VU_ID target) :
            base((VU_MSG_TYPE)FalconMsgID.ObjectiveMsg, HandlingThread.CampaignThread, senderid, target)
        {
        }
        //TODO public ~FalconObjectiveMessage(void);
#if TODO
        public override int Size ()
		{

			return sizeof(DATA_BLOCK) + base.Size ();
		}

		public override int Decode (byte[] buf, ref int pos, int length)
		{
	
			int size;

			size = base.Decode (buf, ref pos, length);
			dataBlock = new dataBlock ();
			buf += sizeof(dataBlock);
			size += sizeof(dataBlock);
			return size;
		}

		public override int Encode (byte[] buf, ref int pos)
		{
			int size;

			size = base.Encode (buf, ref pos);
			memcpy (*buf, &dataBlock, sizeof(dataBlock));
			buf += sizeof(dataBlock);
			size += sizeof(dataBlock);
			return size;

		}
#endif
        public class DATA_BLOCK
        {

            public VU_ID from;
            public VU_ID to;
            public uint message;
            public short data1;
            public short data2;
            public short data3;
        } ;

        public DATA_BLOCK dataBlock;

        protected override VU_ERRCODE Process(bool autodisp)
        {
            throw new NotImplementedException();
        }
    }
}

