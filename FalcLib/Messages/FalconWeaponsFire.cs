﻿using System;
using System.IO;
using FalconNet.VU;
using VU_MSG_TYPE = System.Byte;
using VU_BOOL = System.Boolean;

namespace FalconNet.FalcLib
{
    public class FalconWeaponsFire : FalconEvent
    {
        public enum WeaponType
        {
            GUN,
            //       LRM, // I don't think this is used (or handled)  SCR 10/7/98
            MRM,
            SRM,
            BMB,
            ARM,
            AGM,
            Rocket,
            Recon
        };

        public FalconWeaponsFire(VU_ID entityId, VuTargetEntity target, VU_BOOL loopback = true) :
            base((VU_MSG_TYPE)FalconMsgID.WeaponFireMsg, HandlingThread.SimThread, entityId, target, loopback)
        {
            // RequestReliableTransmit ();
            // RequestOutOfBandTransmit ();
            // Your Code Goes Here
        }
        public FalconWeaponsFire(VU_MSG_TYPE type, VU_ID senderid, VU_ID target)
            : base((VU_MSG_TYPE)FalconMsgID.WeaponFireMsg, HandlingThread.SimThread, senderid, target)
        {
            // Your Code Goes Here
        }

        //TODO public ~FalconWeaponsFire(void);
#if TODO
    public virtual int Size()
    {
        return sizeof(dataBlock) + FalconEvent::Size();
    }

    //sfr: added long *
    public int Decode(VU_BYTE **buf, long *rem)
    {
        long init = *rem;

        FalconEvent::Decode(buf, rem);
        memcpychk(&dataBlock, buf, sizeof(dataBlock), rem);
        return init - *rem;
    }

    public int Encode(VU_BYTE **buf)
    {
        int size;

        size = FalconEvent::Encode(buf);
        memcpy(*buf, &dataBlock, sizeof(dataBlock));
        *buf += sizeof(dataBlock);
        size += sizeof(dataBlock);
        return size;
    }
#endif

        public class DATA_BLOCK
        {
            public WeaponType weaponType;
            public VU_ID fEntityID;
            public ushort fCampID;
            public byte fPilotID;
            public ushort fIndex;
            public byte fSide;
            ushort fWeaponID;
            public VU_ID fWeaponUID;
            public VU_ID targetId;
            public float dx;
            public float dy;
            public float dz;
            public uint gameTime;
        }
        public DATA_BLOCK dataBlock;

        protected override VU_ERRCODE Process(bool autodisp)
        {
#if TODO
            FalconEntity theEntity;
            FalconEntity theTarget = null;
            SimBaseClass simEntity = null;

            if (autodisp)
                return 0;

            theTarget = (FalconEntity)VUSTATIC.vuDatabase.Find(dataBlock.targetId);
            theEntity = (FalconEntity)VUSTATIC.vuDatabase.Find(dataBlock.fEntityID);

            if (theEntity != null)
            {
                if (theEntity.IsSim())
                {
                    simEntity = (SimBaseClass)theEntity;

                    if (simEntity && !simEntity.IsLocal() && dataBlock.weaponType == WeaponType.GUN)
                    {
                        if (simEntity.nonLocalData)
                        {
                            simEntity.nonLocalData.dx = dataBlock.dx;
                            simEntity.nonLocalData.dy = dataBlock.dy;
                            simEntity.nonLocalData.dz = dataBlock.dz;
                            simEntity.nonLocalData.flags |= NONLOCAL_GUNS_FIRING;
                            simEntity.nonLocalData.timer2 = 0;
                        }

                        if (dataBlock.fWeaponUID.num_ == 0) simEntity.SetFiring(false);
                        else simEntity.SetFiring(true);
                    }

                    //Cobra test
                    if ((theEntity.IsAirplane() || theEntity.IsHelicopter()) /*&& dataBlock.weaponType != FalconWeaponsFire::GUN*/)
                    {
                        FalconRadioChatterMessage radioMessage = new FalconRadioChatterMessage(simEntity.Id(), FalconLocalSession);
                        radioMessage.dataBlock.to = MESSAGE_FOR_TEAM;
                        radioMessage.dataBlock.from = theEntity.Id();
                        // radioMessage.dataBlock.voice_id = (uchar)((SimBaseClass)theEntity).GetPilotVoiceId();
                        radioMessage.dataBlock.voice_id = ((FlightClass)(((AircraftClass)theEntity).GetCampaignObject())).GetPilotVoiceID(((AircraftClass)theEntity).GetCampaignObject().GetComponentIndex(((AircraftClass)theEntity)));

                        // JPO - special case AMRAAM call
                        // 2001-09-16 M.N. CHANGED FROM FalconWeaponsFire::MRM TO CT ID CHECK . Aim-7 IS ALSO A MRM, BUT HAS A FOX-1 CALL
                        if (WeaponDataTable[GetWeaponIdFromDescriptionIndex(dataBlock.fWeaponID - VU_LAST_ENTITY_TYPE)].Index == 227 ||
                            WeaponDataTable[GetWeaponIdFromDescriptionIndex(dataBlock.fWeaponID - VU_LAST_ENTITY_TYPE)].Index == 228)
                        {
                            radioMessage.dataBlock.message = rcFIREAMRAAM;
                            radioMessage.dataBlock.edata[0] = ((SimMoverClass)theEntity).vehicleInUnit;

                            if (theTarget != null)   // we have a target - how far away?
                            {
                                float dx = theTarget.XPos() - theEntity.XPos();
                                float dy = theTarget.YPos() - theEntity.YPos();
                                float dz = theTarget.ZPos() - theEntity.ZPos();

                                float distance = (float)Math.Sqrt(dx * dx + dy * dy + dz * dz) * FT_TO_NM;

                                if (distance < 5)
                                    radioMessage.dataBlock.edata[1] = 1; // short
                                else if (distance > 15)
                                    radioMessage.dataBlock.edata[1] = 3; // long
                                else
                                    radioMessage.dataBlock.edata[1] = 2; // medium
                            }
                            else radioMessage.dataBlock.edata[1] = 0; // maddog!
                        }
                        else
                        {
                            radioMessage.dataBlock.message = rcFIRING;

                            //Total Fucking HACK!!!
                            if (dataBlock.weaponType == WeaponType.Rocket)
                                radioMessage.dataBlock.edata[0] = 887; // 2001-09-16 M.N. "Rockets" EVAL INDEX = 887, NOT 163
                            else
                                radioMessage.dataBlock.edata[0] = WeaponDataTable[GetWeaponIdFromDescriptionIndex(dataBlock.fWeaponID - VU_LAST_ENTITY_TYPE)].Index;
                        }

                        FalconSendMessage(radioMessage, false);
                    }

                    //if theEntity shot at someone on their own team complain
                    //edg: there seems to be a crash here if the target is NOT a SIm
                    // GetPilotVoiceId is a SimBaseClass member.
                    // check for Sim.  I'm not sure what needs to be done if its a camp entity
                    //Cobra added weaponType check so bombs don't fall through into this section
                    if (theTarget != null /* && theTarget.IsSim()*/ && (int)dataBlock.weaponType < 3
                        && (theTarget.IsAirplane() || theTarget.IsHelicopter()) && (GetTTRelations(theTarget.GetTeam(), simEntity.GetTeam()) < Hostile))
                    {
                        bool playMessage = true;

                        if (dataBlock.weaponType == WeaponType.GUN)
                        {
                            float az, el;

                            // Check rel geom
                            CalcRelAzEl(simEntity, theTarget.XPos(), theTarget.YPos(), theTarget.ZPos(), ref az, ref el);

                            if (fabs(az) > 10.0F * DTR || fabs(el) > 10.0F * DTR)
                                playMessage = false;
                        }

                        if (playMessage && theTarget != null)
                        {
                            FalconRadioChatterMessage radioMessage = new FalconRadioChatterMessage(simEntity.Id(), FalconLocalSession);
                            radioMessage.dataBlock.to = MESSAGE_FOR_TEAM;
                            radioMessage.dataBlock.from = theTarget.Id();

                            if (!((AircraftClass)theTarget).GetCampaignObject())
                                radioMessage.dataBlock.voice_id = (byte)((SimBaseClass)theTarget).GetPilotVoiceId();
                            else
                                radioMessage.dataBlock.voice_id = ((FlightClass)(((AircraftClass)theTarget).GetCampaignObject())).GetPilotVoiceID(((AircraftClass)theTarget).GetCampaignObject().GetComponentIndex(((AircraftClass)theTarget)));

                            radioMessage.dataBlock.message = rcSHOTWINGMAN;
                            radioMessage.dataBlock.edata[0] = 32767;
                            FalconSendMessage(radioMessage, false);
                        }
                    }

                    // Register the shot if it was by someone in our package me123 or we are server
                    if (dataBlock.fWeaponUID != FalconNullId && simEntity.GetCampaignObject() && ((simEntity.GetCampaignObject()).InPackage() || FalconLocalGame.GetGameType() == game_InstantAction || g_bLogEvents))
                        TheCampaign.MissionEvaluator.RegisterShot(this);

                }
                else
                {
                    // Register the shot if it was by someone in our package
                    if (dataBlock.fWeaponUID != FalconNullId && theEntity.IsCampaign() && (((CampEntity)theEntity).InPackage() || FalconLocalGame.GetGameType() == game_InstantAction || g_bLogEvents))
                        TheCampaign.MissionEvaluator.RegisterShot(this);
                }
            }

            // Register the shot if it was at the player
            if (TheCampaign.MissionEvaluator && dataBlock.targetId == FalconLocalSession.GetPlayerEntityID() || g_bLogEvents)
                if (theTarget && theTarget.IsSetFalcFlag(FEC_HASPLAYERS) && theTarget.IsSim() && ((SimBaseClass)theTarget).GetCampaignObject())
                    TheCampaign.MissionEvaluator.RegisterShotAtPlayer(this, ((SimBaseClass)theTarget).GetCampaignObject().GetCampID(), ((SimMoverClass*)theTarget).pilotSlot); //me123 added

            //else TheCampaign.MissionEvaluator.RegisterShotAtPlayer(this,NULL,NULL);//me123 maddog

            // if the weapon is a missile and we have a target ID, tell the target
            // there's an incoming
            if (dataBlock.targetId != vuNullId &&
                (dataBlock.weaponType == WeaponType.SRM || dataBlock.weaponType == WeaponType.MRM))
            {
                SimBaseClass theMissile = (SimBaseClass)(vuDatabase.Find(dataBlock.fWeaponUID));

                if (theTarget && theMissile && theTarget.IsSim())
                {
                    ((SimBaseClass)theTarget).SetIncomingMissile(theMissile);

                    if (simEntity && !simEntity.OnGround())
                        ((SimBaseClass)theTarget).SetThreat(simEntity, THREAT_MISSILE);
                }
            }
            else if (dataBlock.targetId != vuNullId && dataBlock.weaponType == WeaponType.GUN)
            {
                if (theTarget && theTarget.IsSim())
                {
                    if (simEntity && !simEntity.OnGround())
                        ((SimBaseClass)theTarget).SetThreat(simEntity, THREAT_GUN);
                }
            }

            // WM 11-12-03  Fix the annoying missile bug where you can't see other client's missiles if
            //   the host doesn't join the 3D world.
            if (dataBlock.weaponType == WeaponType.SRM ||
                dataBlock.weaponType == WeaponType.MRM ||
                dataBlock.weaponType == WeaponType.AGM || 
                dataBlock.weaponType == WeaponType.BMB)
            {
                gRebuildBubbleNow = 2;
            }

            return VU_ERRCODE.VU_SUCCESS;
#endif 
            throw new NotImplementedException();
        }
    }
}
