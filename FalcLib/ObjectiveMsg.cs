using System;
using FalconNet.VU;
using VU_MSG_TYPE=System.Byte;

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
	public class FalconObjectiveMessage :   FalconEvent
	{
   
		public enum ObjMessage
		{
			objCaptured,
			objSetSupply,
			objSetLosses
		}

		public FalconObjectiveMessage (VU_ID entityId, VuTargetEntity target, bool loopback=true) :
			base (FalconMsgID.ObjectiveMsg, HandlingThread.CampaignThread, entityId, target, loopback)
		{}
		
		public FalconObjectiveMessage (VU_MSG_TYPE type, VU_ID senderid, VU_ID target)	:
			base (FalconMsgID.ObjectiveMsg, HandlingThread.CampaignThread, senderid, target)
	{
	}
		//TODO public ~FalconObjectiveMessage(void);
		public override int Size ()
		{
#if TODO
			return sizeof(DATA_BLOCK) + base.Size ();
#endif
			throw new NotImplementedException();
		}

		public override int Decode (byte[] buf, ref int pos, int length)
		{
#if TODO	
			int size;

			size = base.Decode (buf, ref pos, length);
			dataBlock = new dataBlock ();
			buf += sizeof(dataBlock);
			size += sizeof(dataBlock);
			return size;
#endif
			throw new NotImplementedException ();
		}

		public override int Encode (byte[] buf, ref int pos)
		{
#if TODO
			int size;

			size = base.Encode (buf, ref pos);
			memcpy (*buf, &dataBlock, sizeof(dataBlock));
			buf += sizeof(dataBlock);
			size += sizeof(dataBlock);
			return size;
#endif
			throw new NotImplementedException ();
		}
		public class DATA_BLOCK
		{
         
			public VU_ID from;
			public VU_ID to;
			public uint message;
			public short data1;
			public  short data2;
			public short data3;
		} ;

		public DATA_BLOCK	dataBlock;

		protected override int Process (byte autodisp)
		{
			throw new NotImplementedException ();
		}
	}
}

