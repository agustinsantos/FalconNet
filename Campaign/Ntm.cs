﻿using System;
using Team = System.Byte;
using FalconNet.VU;
using FalconNet.Common;
using System.IO;
using F4PFList = FalconNet.FalcLib.FalconPrivateList;
using F4POList = FalconNet.FalcLib.FalconPrivateOrderedList;


namespace FalconNet.Campaign
{
    // ==================================
    // Naval Tasking Manager class
    // ==================================

    public class NavalTaskingManagerClass : CampManagerClass
    {
        public short flags;
        // These don't need to be transmitted
        public F4PFList unitList;							// Collection of available ground assets
        public short tod;									// Time of day (temp variable)
        public short topPriority;						// Highest PO priority (for scaling)
        public short done;									// Flagged when all units assigne

        // constructors
        public NavalTaskingManagerClass(ushort type, Team t)
            : base(type, t)
        {
            flags = 0;
            //	unitList = new FalconPrivateList(&AllNavalFilter);	
            //	unitList.Init();
            unitList = null;
            tod = 0;
            topPriority = 0;
            done = 0;
        }
#if TOD
        public NavalTaskingManagerClass(byte[] stream)
            : base(stream)
        {

            memcpy(&flags, *stream, sizeof(short)); *stream += sizeof(short);
            //	unitList = new FalconPrivateList(&AllNavalFilter);	
            //	unitList.Init();
            unitList = null;
            tod = 0;
            topPriority = 0;
            done = 0;
 
            throw new NotImplementedException();
        }

        public NavalTaskingManagerClass(FileStream file)
            : base(file)
        {
 
            fread(&flags, sizeof(short), 1, file);
            //	unitList = new FalconPrivateList(&AllNavalFilter);	
            //	unitList.Init();
            unitList = null;
            tod = 0;
            topPriority = 0;
            done = 0;
 
            throw new NotImplementedException();
        }


        // TODO public virtual ~NavalTaskingManagerClass();
        public override int SaveSize()
        {
            return base.SaveSize()
                + sizeof(short);
        }
        public override int Save(byte[] stream)
        {
 
            base.Save(stream);
            memcpy(*stream, &flags, sizeof(short)); *stream += sizeof(short);
            return SaveSize();
 
            throw new NotImplementedException();
        }
        public override int Save(FileStream file)
        {
 
            int retval = 0;

            if (!file)
                return 0;
            retval += base.Save(file);
            retval += fwrite(&flags, sizeof(short), 1, file);
            return retval;

            throw new NotImplementedException();
        }
#endif
        public override VU_ERRCODE Handle(VuFullUpdateEvent evnt)
        {
#if TODO
            NavalTaskingManagerClass tmpGTM = (NavalTaskingManagerClass)(evnt.expandedData_);

            // Copy in new data
            memcpy(&flags, &tmpGTM.flags, sizeof(short));
            return (VuEntity.Handle(evnt));
#endif
            throw new NotImplementedException();
        }

        // Required pure virtuals
        public override void DoCalculations()
        {
#if TODO
            MissionRequestClass mis;
            Unit unit;
            VuListIterator mit = new VuListIterator(AllRealList);
            int j;
            Objective o;

            // Target all naval units
            unit = (Unit)mit.GetFirst();
            while (unit)
            {
                if (unit.IsTaskForce() && GetRoE(owner, unit.GetTeam(), ROE_NAVAL_FIRE) == ROE_ALLOWED)
                {
                    mis.requesterID = FalconnullId;
                    unit.GetLocation(&mis.tx, &mis.ty);
                    mis.vs = unit.GetTeam();
                    mis.who = owner;
                    j = 30 + (rand() + unit.GetCampID()) % 60;
                    mis.tot = Camplib.Camp_GetCurrentTime() + j * CampaignMinutes;
                    mis.tot_type = TYPE_NE;
                    mis.targetID = unit.Id();
                    mis.target_num = 255;
                    mis.mission = AMIS_ASHIP;
                    mis.roe_check = ROE_NAVAL_FIRE;
                    // Determine if they're active or static
                    o = FindNearestObjective(mis.tx, mis.ty, null, 2);
                    if (o && o.GetFalconType() == TYPE_PORT)
                    {
                        if (unit.GetSType() == STYPE_UNIT_SEA_TANKER || unit.GetSType() == STYPE_UNIT_SEA_TRANSPORT)
                            mis.context = enemyNavalForceUnloading;
                        else
                            mis.context = enemyNavalForceStatic;
                    }
                    else
                        mis.context = enemyNavalForceActive;
                    mis.RequestMission();
                }
                unit = (Unit)mit.GetNext();
            }
#endif
            throw new NotImplementedException();
        }

        public override int Task()
        {
            return 0;
        }

        public void SendNTMMessage(VU_ID from, short message, short data1, short data2, VU_ID data3)
        {
#if TODO
            VuTargetEntity target = (VuTargetEntity)VuDatabase.vuDatabase.Find(OwnerId());
            FalconNavalTaskingMessage tontm = new FalconNavalTaskingMessage(Id(), target);

            if (this)
            {
                tontm.dataBlock.from = from;
                tontm.dataBlock.team = owner;
                tontm.dataBlock.messageType = message;
                tontm.dataBlock.data1 = data1;
                tontm.dataBlock.data2 = data2;
                tontm.dataBlock.enemy = data3;
                FalconSendMessage(tontm, true);
            }
#endif
            throw new NotImplementedException();
        }

    }

    /* TODO 
typedef NavalTaskingManagerClass *NavalTaskingManager;
typedef NavalTaskingManagerClass *NTM;}
    TODO */
}