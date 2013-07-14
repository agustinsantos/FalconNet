using FalconNet.FalcLib;
using FalconNet.VU;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FalconNet.Sim
{
#if TODO
    public class DigitalBrain : BaseBrain
    {
        private const string MANEUVER_DATA_FILE = "sim\\acdata\\brain\\mnvrdata.dat";

        // Pete Style Maneuver Stuff
        public enum BVRInterceptType
        {
            BvrFollowWaypoints,
            BvrFlyFormation,
            BvrSingleSideOffset,
            BvrPince,
            BvrPursuit,
            BvrNoIntercept,
            BvrPump,
            BvrCrank,
            BvrCrankRight,
            BvrCrankLeft,
            BvrNotch,
            BvrNotchRight,
            BvrNotchRightHigh,
            BvrNotchLeft,
            BvrNotchLeftHigh,
            BvrGrind,
            NumInterceptTypes
        }
        public enum BVRProfileType
        {
            Pnone,
            Plevel1a,
            Plevel1b,
            Plevel1c,
            Plevel2a,
            Plevel2b,
            Plevel2c,
            Plevel3a,
            Plevel3b,
            Plevel3c,
            Pbeamdeploy,
            Pbeambeam,
            Pwall,
            Pgrinder,
            Pwideazimuth,
            Pshortazimuth,
            PwideLT,
            PShortLT,
            PSweep,
            PDefensive
        }

        public enum WVRMergeManeuverType
        {
            WvrMergeHitAndRun,
            WvrMergeLimited,
            WvrMergeUnlimited,
            NumWVRMergeMnverTypes
        }

        public enum SpikeReactionType
        {
            SpikeReactNone,
            SpikeReactECM,
            SpikeReactBeam,
            SpikeReactDrag,
            NumSpikeReactionTypes
        }

        public enum ACMnverClass
        {
            MnvrClassF4,
            MnvrClassF5,
            MnvrClassF14,
            MnvrClassF15,
            MnvrClassF16,
            MnvrClassMig25,
            MnvrClassMig27,
            MnvrClassA10,
            MnvrClassBomber,
            NumMnvrClasses
        }

        public enum ACMnverClassFlags
        {
            CanLevelTurn = 0x1,
            CanSlice = 0x2,
            CanUseVertical = 0x4,
            CanOneCircle = 0x10,
            CanTwoCircle = 0x20,
            CanJinkSnake = 0x100,
            CanJinkLoaded = 0x200,
            CanJinkUnloaded = 0x400
        }

        public enum OffsetDirs
        {
            offForward,
            offRight,
            offBack,
            offLeft,
            taxiLeft,
            taxiRight,
            downRunway,
            upRunway,
            rightRunway,
            leftRunway,
            centerRunway,
        }

        /*--------------*/
        /* Mnvr Element */
        /*--------------*/
        public struct ManeuverChoiceTable
        {
            public BVRInterceptType[] intercept;
            public WVRMergeManeuverType[] merge;
            public SpikeReactionType[] spikeReact;
            public short numIntercepts;
            public short numMerges;
            public short numReacts;
        }

        public struct ManeuverClassData
        {
            public int flags;
        }

        // 2002-03-11 ADDED BY S.G. Which ManeuverChoiceTable element to use in CanEngage
        public enum MaveuverType
        {
            BVRManeuver = 0x01,
            WVRManeuver = 0x02
        }

        private const int NumMnvrClasses = (int)(DigitalBrain.ACMnverClass.NumMnvrClasses);
        public static ManeuverChoiceTable[,] maneuverData = new ManeuverChoiceTable[NumMnvrClasses, NumMnvrClasses];
        public static ManeuverClassData[] maneuverClassData = new ManeuverClassData[NumMnvrClasses];
        public static void ReadManeuverData()
        {

            SimlibFileClass mnvrFile;
            int i, j, k;
            byte[] fileType = new byte[1];

            /*---------------------*/
            /* open formation file */
            /*---------------------*/
            mnvrFile = SimlibFileClass.Open(MANEUVER_DATA_FILE, SIMLIB.SIMLIB_READ);
            mnvrFile.Read(fileType, 1);

            if (fileType[0] == 'B') // Binary
            {
                for (i = 0; i < NumMnvrClasses; i++)
                {
                    for (j = 0; j < NumMnvrClasses; j++)
                    {
                    }
                }
            }
            else if (fileType[0] == 'A') // Ascii
            {
                for (i = 0; i < NumMnvrClasses; i++)
                {
                    // Get the limits for this Maneuver type
                    sscanf(mnvrFile.GetNext(), "%x", maneuverClassData[i]);

                    for (j = 0; j < NumMnvrClasses; j++)
                    {
                        maneuverData[i, j].numIntercepts = short.Parse(mnvrFile.GetNext());
                        if (maneuverData[i, j].numIntercepts != 0)
                        {
                            maneuverData[i, j].intercept =
#if USE_SH_POOLS
						(BVRInterceptType *)MemAllocPtr(gReadInMemPool, sizeof(BVRInterceptType)*maneuverData[i, j].numIntercepts,0);
#else
 new BVRInterceptType[maneuverData[i, j].numIntercepts];
#endif
                        }
                        else
                            maneuverData[i, j].intercept = null;

                        maneuverData[i, j].numMerges = short.Parse(mnvrFile.GetNext());
                        if (maneuverData[i, j].numMerges != 0)
                        {
                            maneuverData[i, j].merge =
#if USE_SH_POOLS
						(WVRMergeManeuverType *)MemAllocPtr(gReadInMemPool, sizeof(WVRMergeManeuverType)*maneuverData[i, j].numMerges,0);
#else
 new WVRMergeManeuverType[maneuverData[i, j].numMerges];
#endif
                        }
                        else
                            maneuverData[i, j].merge = null;

                        maneuverData[i, j].numReacts = short.Parse(mnvrFile.GetNext());
                        if (maneuverData[i, j].numReacts != 0)
                        {
                            maneuverData[i, j].spikeReact =
#if USE_SH_POOLS
						(SpikeReactionType *)MemAllocPtr(gReadInMemPool, sizeof(SpikeReactionType)*maneuverData[i, j].numReacts,0);
#else
 new SpikeReactionType[maneuverData[i, j].numReacts];
#endif
                        }
                        else
                            maneuverData[i, j].spikeReact = null;

                        for (k = 0; k < maneuverData[i, j].numIntercepts; k++)
                        {
                            maneuverData[i, j].intercept[k] =
                               (BVRInterceptType)(int.Parse(mnvrFile.GetNext()) - 1);
                        }

                        for (k = 0; k < maneuverData[i, j].numMerges; k++)
                        {
                            maneuverData[i, j].merge[k] =
                               (WVRMergeManeuverType)(int.Parse(mnvrFile.GetNext()) - 1);
                        }

                        for (k = 0; k < maneuverData[i, j].numReacts; k++)
                        {
                            maneuverData[i, j].spikeReact[k] =
                               (SpikeReactionType)(int.Parse(mnvrFile.GetNext()) - 1);
                        }
                    }
                }
            }
            else
            {
                Debug.WriteLine("Bad Maneuver Data File Format");
            }
            mnvrFile.Close(); // JPO Handle/memory leak fix
            //TODO delete mnvrFile;
            mnvrFile = null;
        }

        public static void FreeManeuverData()
        {
            int i, j;

            for (i = 0; i < (int)(DigitalBrain.ACMnverClass.NumMnvrClasses); i++)
            {
                for (j = 0; j < (int)(DigitalBrain.ACMnverClass.NumMnvrClasses); j++)
                {
                    //TODO delete maneuverData[i, j].intercept;
                    //TODO  delete maneuverData[i, j].merge;
                    //TODO  delete maneuverData[i, j].spikeReact;
                    maneuverData[i, j].intercept = null;
                    maneuverData[i, j].merge = null;
                    maneuverData[i, j].spikeReact = null;
                }
            }

        }

        //for now landing is a high priority mode so they won't get distracted
        public enum DigiMode
        {
            TakeoffMode, GroundAvoidMode, CollisionAvoidMode, GunsJinkMode, MissileDefeatMode,
            LandingMode, DefensiveModes, RefuelingMode,
            SeparateMode, AccelMode, MergeMode, MissileEngageMode, GunsEngageMode,
            RoopMode, OverBMode, WVREngageMode, BVREngageMode,
            LoiterMode, FollowOrdersMode, RTBMode, WingyMode,
            BugoutMode, WaypointMode, GroundMnvrMode,
            LastValidMode, NoMode
        };

        public enum WvrTacticType
        {
            WVR_NONE, WVR_RANDP, WVR_OVERB, WVR_ROOP, WVR_GUNJINK, WVR_STRAIGHT,
            WVR_BUGOUT, WVR_AVOID, WVR_BEAM, WVR_BEAM_RETURN, WVR_RUNAWAY
        };

        // engagement tactic from campaign
        public int campTactic;

        public DigiMode GetCurrentMode() { return curMode; }

        public enum RefuelStatus
        {
            refNoTanker,
            refVectorTo,
            refWaiting,
            refRefueling,
            refDone,
        }

        public int IsAtFirstTaxipoint()
        {
            Debug.Assert(self != null);
            FlightClass flight = (FlightClass)self.GetCampaignObject();
            Debug.Assert(flight != null);
            WayPointClass w = flight.GetFirstUnitWP(); // this is takeoff time
            ObjectiveClass* Airbase = (ObjectiveClass*)vuDatabase.Find(airbase);
            Debug.Assert(Airbase);
            // so takeoff time, - deag time (taxi point time) - if we're past that - we're ready.
            if (SimLibElapsedTime > w.GetWPDepartureTime() - Airbase.brain.MinDeagTime())
            {
                return 0;
            }
            else
                return 1;

        }





        protected AircraftClass self;
        protected AirframeClass  af;

        protected enum WaypointState
        {
            NotThereYet, Arrived, Stabalizing, OnStation, PreRoll, Departing, HoldingShort, HoldInPlace,
            TakeRunway, Takeoff, Taxi, Upwind, Crosswind, Downwind, Base, Final, Final1
        };

        // Controls
        protected SAVE_ARRAY pstickSave;
        protected float autoThrottle;
        protected int trackMode;
        protected float maxGs, cornerSpeed;

        // Ground Avoid
        protected int groundAvoidNeeded;
        protected float gaRoll, gaGs;

        // Mode Stuff
        protected int waypointMode;
        protected WaypointState onStation;
        protected DigiMode curMode;
        protected DigiMode lastMode;
        protected DigiMode nextMode;

        protected void AddMode(DigiMode newMode)
        {
            // 2000-11-17 ADDED BY S.G. SO AI CAN BE MORE AGRESSIVE WHEN RTBing
            // Now if the new mode asked is 'LandingMode', and the mode we are asked to go to is a defensive or engagement mode, leave it alone
            if (newMode == DigiMode.LandingMode &&
                (nextMode == DigiMode.DefensiveModes ||
                (nextMode >= DigiMode.MissileEngageMode && nextMode <= DigiMode.WVREngageMode)))


                return;

            //ME123  if this is not done you will suffer severe floodign becourse resolvemodeconflict funktion
            // will send an atcstatus = NOATC when entering wvr engage and it will alternate between landing and wvrengage each frame in some situations.
            if (curMode == DigiMode.LandingMode && newMode == DigiMode.WVREngageMode) return;

            // So we're not asking to land but are we in 'LandingMode' already? If so, check if we are engaged or should engage
            if (nextMode == DigiMode.LandingMode && newMode >= DigiMode.MissileEngageMode && newMode <= DigiMode.WVREngageMode)
            {
                nextMode = newMode;
                return;
            }

            // None of the above, to the normal coding
            // END OF ADDED SECTION
            if (newMode < nextMode)
                nextMode = newMode;
        }

        protected void ResolveModeConflicts()
        {
            if (threatPtr == null && curMode != DigiMode.WVREngageMode)
            {
                wvrCurrTactic = WvrTacticType.WVR_NONE;
                wvrPrevTactic = WvrTacticType.WVR_NONE;
            }

            /*--------------------*/
            /* What were we doing */
            /*--------------------*/
            lastMode = curMode;
            curMode = nextMode;
            nextMode = NoMode;

            //we appear to be getting distracted while landing
            //Debug.Assert( (atcstatus == noATC) || (curMode == LandingMode || curMode == TakeoffMode || curMode == WaypointMode) );
            if (atcstatus != AtcStatusEnum.noATC && curMode != DigiMode.LandingMode && curMode != DigiMode.TakeoffMode && curMode != DigiMode.WaypointMode)
            {
                SendATCMsg(noATC);
                ResetATC();
            }
        }

        protected void PrtMode()
        {
            ulong tid;

            // for only checking ownship....
            if (self != SimDriver.playerEntity)
                return;

            if (curMode != lastMode)
            {
                if (targetPtr)
                    tid = targetPtr.BaseData().Id();
                else
                    tid = 0;

                switch (curMode)
                {
                    case DigiMode.RTBMode:
                        PrintOnline("DIGI RTB");
                        break;
                    case DigiMode.WingyMode:
                        PrintOnline("DIGI Wingman");
                        break;
                    case DigiMode.WaypointMode:
                        PrintOnline("DIGI Waypoint");
                        break;
                    case DigiMode.GunsEngageMode:
                        PrintOnline("DIGI Guns Engage");
                        break;
                    case DigiMode.MergeMode:
                        PrintOnline("DIGI Merge");
                        break;
                    case DigiMode.BVREngageMode:
                        PrintOnline("DIGI BVR Engage");
                        break;
                    case DigiMode.WVREngageMode:
                        PrintOnline("DIGI WVR Engage");
                        break;
                    case DigiMode.MissileDefeatMode:
                        PrintOnline("DIGI Missile Defeat");
                        break;
                    case DigiMode.MissileEngageMode:
                        PrintOnline("DIGI Missile Engage");
                        break;
                    case DigiMode.GunsJinkMode:
                        PrintOnline("DIGI Guns Jink");
                        break;
                    case DigiMode.GroundAvoidMode:
                        PrintOnline("DIGI Ground Avoid");
                        break;
                    case DigiMode.LoiterMode:
                        PrintOnline("DIGI Loiter");
                        break;
                    case DigiMode.CollisionAvoidMode:
                        PrintOnline("DIGI Collision");
                        break;
                    case DigiMode.SeparateMode:
                        PrintOnline("DIGI Separate");
                        break;
                    case DigiMode.BugoutMode:
                        PrintOnline("DIGI Bug Out");
                        break;
                    case DigiMode.RoopMode:
                        PrintOnline("DIGI Roop");
                        break;
                    case DigiMode.OverBMode:
                        PrintOnline("DIGI Overb");
                        break;
                    case DigiMode.AccelMode:
                        PrintOnline("DIGI Accelerate");
                        break;
                }
                /*
                AirAIModeMsg* modeMsg;

                   modeMsg = new AirAIModeMsg (self.Id(), FalconLocalGame);
                   modeMsg.dataBlock.gameTime = SimLibElapsedTime;
                   modeMsg.dataBlock.whoDidIt = self.Id();
                   modeMsg.dataBlock.newMode = curMode;
                   FalconSendMessage (modeMsg,false);
                      */
            }
        }


        protected void PrintOnline(string str)
        {
            int att = self.Id().num_;
            int tid = 0;

            if (targetPtr)
            {
                tid = targetPtr.BaseData().Id().num_;
            }
            MonoPrint("%8ld %-25s %3d - %-3d . %3d - %-3d\n", SimLibElapsedTime, str,
               att & 0xFFFF, att >> 16, tid & 0xFFFF, tid >> 16);
        }

        protected void SetCurrentTactic()
        {
            // What should I do ?
            //if (!isWing || self.curWaypoint.GetWPAction() != WP_TAKEOFF)
            //{
            DecisionLogic();/*
   }
   else if (isWing)
   {
      //if (self.curWaypoint.GetWPAction() == WP_TAKEOFF ||
       //   (self.curWaypoint.GetWPAction() == WP_LAND))
		if(atcstatus != noATC)
		{
			if(nextMode == WingyMode)
				nextMode = WaypointMode;
			else
				AddMode (WaypointMode);
		}
      else if(mpActionFlags[AI_FOLLOW_FORMATION] == true) {
         AddMode (WingyMode);
		}
		else {
			AddMode (WaypointMode);
		}

      // Select highest priority mode Resolve mode conflicts
      ResolveModeConflicts();
   }*/
            // Select flight model
            af.SetSimpleMode(SelectFlightModel());
        }



        // Maneuvers
        protected float trackX, trackY, trackZ;
        protected float holdAlt, holdPsi;
        protected float gammaHoldIError;
        protected float reactiont;//me123 reaction time for ai
        protected int MissileEvade();
        protected void ChoiceProfile();// me123 canned profiles
        protected void DoProfile();// me123 canned profiles
        protected void level1a();// me123 canned profiles
        protected void level1b();//me123
        protected void level1c();//me123
        protected void level2a();//me123
        protected void level2b();//me123
        protected void level2c();//me123
        protected void level3a();//me123
        protected void level3b();//me123
        protected void level3c();//me123
        protected void beamdeploy();//me123
        protected void beambeam();//me123
        protected void wall();//me123
        protected void grinder();//me123
        protected void wideazimuth();//me123
        protected void shortazimuth();//me123
        protected void wideLT();//me123
        protected void ShortLT();//me123
        protected void Defensive();//me123
        protected void Sweep();//me123
        protected void StickandThrottle(float DesiredSpeed, float DesiredAltitude);//me123
        protected int MissileBeamManeuver();
        protected void MissileCrankManeuver();//me123
        protected void BaseLineIntercept();//me123
        protected int BeamManeuver(int direction, int Height);//me123
        protected void CrankManeuver(int direction);//me123
        protected void DragManeuver();//me123
        protected int MachHold(float m1, float m2, int pitchStick)
        {
#if TODO
            float eProp = 0.0F, thr = 0.0F;
            float maxDelta = 0.0F, cornerDelta = 0.0F, burnerDelta = 0.0F;
            float dx = 0, dy = 0, dist = 0;

            eProp = m1 - m2;

            /*-----------------*/
            /* knots indicated */
            /*-----------------*/

            // 2001-10-28 ADDED "* FTPSEC_TO_KNOTS" to MinVCas by M.N. this function needs knots 2002-03-14 REMOVED BY S.G. MinVcas is ALREADY in KNOTS and NOT in FTPSEC

            if (m1 < (af.MinVcas() /* * FTPSEC_TO_KNOTS */) && af.IsSet(AirframeClass.InAir))
            {
                m1 = af.MinVcas() /* * FTPSEC_TO_KNOTS */;
                eProp = m1 - m2;
            }
            //me123 addet the max check. we don't wanna exceed the airframe max speed
            if (m1 > af.curMaxStoreSpeed - 20.0f && af.IsSet(AirframeClass.InAir))
            {
                m1 = af.curMaxStoreSpeed - 20.0f;
                eProp = m1 - m2 - g_fePropFactor;
            }

            af.SetFlaps(curMode == LandingMode);
            if (eProp < -100.0F)
            {
                thr = 0.0F;                        /* idle and boards */
                if (curMode > DefensiveModes && curMode < LoiterMode)
                    af.speedBrake = 1.0F;
                else
                    af.speedBrake = -1.0F;
            }
            else if (eProp < -50.0F)
            {
                thr = 0.0F;                        /* idle  */
                af.speedBrake = -1.0F;
            }
            else
            {
                // If in combat take vtDot into account
                // For waypoint stuff you need to be really slow to hit burner
                if (curMode < LoiterMode && curMode != LandingMode) // 2002-02-12 MODIFIED BY S.G. And not in landing mode
                {
                    eProp -= min(2.0F * af.VtDot(), 0.0F);
                    burnerDelta = 100.0F;
                }
                else
                {
                    burnerDelta = 500.0F;
                    // 2002-03-26 MN make it harder to go into afterburner when in waypoint- or wingymode
                    if (curMode == WingyMode || curMode == WaypointMode)
                        burnerDelta = g_fWaypointBurnerDelta;
                }

                if (eProp >= burnerDelta)
                {
                    thr = 1.5F;                        /* burner     */
                    af.speedBrake = -1.0F;
                }
                else
                {
                    // 2002-03-14 MODIFIED BY S.G. Lets fine tune this throttle thing
                    //       thr = (eProp + 100.0F) * 0.008F;     /* linear in-between */
                    //       autoThrottle += eProp * 0.001F * SimLibMajorFrameTime;
                    //       autoThrottle += eProp * timeStep * SimLibMajorFrameTime;
                    // Here we take tVtDot in consideration but clip it at +-5 so it doesn't affect too much
                    float usedVtDot = af.VtDot();
                    if (usedVtDot > g_fFuelVtClip)
                        usedVtDot = g_fFuelVtClip;
                    else if (usedVtDot < -g_fFuelVtClip)
                        usedVtDot = -g_fFuelVtClip;

                    thr = (eProp + g_fFuelBaseProp) * g_fFuelMultProp; /* linear in-between */
                    autoThrottle += (eProp - usedVtDot * g_fFuelVtDotMult) * g_fFuelTimeStep * SimLibMajorFrameTime;

                    // Now see if we're asking to increase/cut the throtle because of speed difference too much (don't go the other direction)
                    if (g_bFuelLimitBecauseVtDot)
                    {
                        if (eProp > 0.0f && autoThrottle < 0.0f)
                            autoThrottle = 0.0f;
                        else if (eProp < 0.0f && autoThrottle > 0.0f)
                            autoThrottle = 0.0f;
                    }
                    // END OF MODFIED SECTION 2002-03-14

                    autoThrottle = max(min(autoThrottle, 1.5F), -1.5F);
                    thr += autoThrottle;
                    if (flightLead)
                    {
                        dx = ((AircraftClass*)flightLead).af.x - af.x;
                        dy = ((AircraftClass*)flightLead).af.y - af.y;
                        dist = (float)sqrt(dx * dx + dy * dy);
                    }
                    // no burner unless in combat
                    if ((curMode >= LoiterMode || curMode == LandingMode) &&											// 2002-02-12 MODIFIED BY S.G. Don't go in AB if you're in landing mode either
                               m2 > aeroDataset[self.af.VehicleIndex()].inputData[AeroDataSet.MinVcas] * 0.9f &&	// JB 011018 If we can't keep our speed up, use the buner 2002-02-12 MODIFIED BY S.G. Use a percentage of MinVcas instead.
                               (!flightLead || flightLead && ((AircraftClass*)flightLead).af) &&
                               (!flightLead || (((AircraftClass*)flightLead).af == af || ((((AircraftClass*)flightLead).af.rpm < 1.0F)) &&										// JB 011025 If the lead is using his burner, we can use ours 2002-02-12 MODIFIED BY S.G. Don't look at lead's burner or g_fFormationBurnerDistance if we're RTBing...
                        dist < g_fFormationBurnerDistance * Phyconst.NM_TO_FT) || curMode == RTBMode) ||					// allow usage of burner if lead is more than defined distance away
                               self.OnGround())																		// never use AB on ground
                    {
                        // Flight lead goes even slower so wingies can catch up
                        /* 2002-02-12 MODIFIED BY S.G. Take the wings 'mInPositionFlag' flag before limiting ourself
                                    if (!isWing)
                                       thr = min (thr, 0.9F);
                                    else
                                       thr = min (thr, 0.975F); */
                        if (!isWing)
                        {
                            // The lead will look at everybody else's position and push faster if everyone is in position.
                            int size = self.GetCampaignObject().NumberOfComponents();
                            int i;
                            for (i = 1; i < size; i++)
                            {
                                AircraftClass* flightMember = (AircraftClass*)self.GetCampaignObject().GetComponentEntity(i);
                                // This code is assuming the lead and the AI are on the same PC... Should be no problem unless another player is in Combat AP...
                                if (flightMember && flightMember.DBrain() && !flightMember.DBrain().mInPositionFlag)
                                    break;
                            }
                            if (i == size)
                                thr = min(thr, 0.99F);
                            else
                                thr = min(thr, 0.9F);
                        }
                        else
                        {
                            // While wingmen unlike look after themself...
                            // 2002-04-07 MN limit wingmen only not to use afterburner, but they may catch up with full MIL power
                            //				if (mInPositionFlag)
                            thr = min(thr, 0.99F);
                            //				else
                            //					thr = min (thr, 0.975F); 
                        }

                    }
                    af.speedBrake = -1.0F;
                }

                // Scale pStick if way off
                if (targetPtr && HoldCorner(self.CombatClass(), targetPtr))
                {
                    cornerDelta = cornerSpeed - m2;
                    cornerDelta -= 2.0F * af.VtDot();
                    switch (SkillLevel())
                    {
                        case 0:  // Recruit, pull till you drop
                            maxDelta = 1000.0F;
                            break;

                        case 1:  // Rookie, pull almost till you drop
                            maxDelta = 250.0F;
                            break;

                        case 2:  // Average, pull too far
                            maxDelta = 200.0F;
                            break;

                        case 3:  // Good, pull a little to far
                            maxDelta = 100.0F;
                            break;

                        case 4:  // Ace, back off and hold desired speed
                        default:
                            maxDelta = 0.0F;
                            break;
                    }
                    if (pStick > 1.0f) pStick = 1.0f;//me123

                    if (!af.IsSet(AirframeClass.IsDigital)) maxDelta = 0.0F;

                    if (pitchStick && cornerDelta > maxDelta && pStick > 0.0F)// && fabs(self.Roll()) < 110.0F * DTR)
                    {
                        pStick *= max(0.55F, (1.0F - (cornerDelta - maxDelta) / (cornerSpeed * 0.75F)));//me123 0.55 from 0.25
                    }
                }
            }

            /*-----------------------------*/
            /* add pitch stick interaction */
            /*-----------------------------*/
            if (pitchStick)
                throtl = thr + ((float)Math.Abs(pStick) / 15.0F);
            else
                throtl = thr;
            //me123 status test. IRCM STUFF.

            if (curMode == MissileEngageMode || curMode == GunsEngageMode || curMode == WVREngageMode)
            {   //me123 status test. we are inside 6nm, somebody is pointing at us and we are head on.

                if (
                    targetData && !F4IsBadReadPtr(targetData, sizeof(SimObjectLocalData)) && // JB 010318 CTD
                    targetData.ataFrom < 40.0f * DTR &&
                    targetData.ata < 40.0f * DTR &&
                    targetData.range < 10.0f * Phyconst.NM_TO_FT &&
                    targetData.range > 1.0 * Phyconst.NM_TO_FT)
                {
                    if (SkillLevel() >= 1 && targetData.range > 8.0 * Phyconst.NM_TO_FT)
                    {
                        throtl = min(0.99f, throtl);// let's cancel burner inside 10nm
                    }
                    else if (SkillLevel() >= 3 && targetData.range > 5.0 * Phyconst.NM_TO_FT)
                    {
                        throtl = min(0.40f, throtl);// let's go midrange inside 8 
                    }
                    else if (SkillLevel() >= 2)
                        throtl = 0.0f;// let's go idle between 4 and 1nm
                }
            }
            /*-------------------*/
            /* limit 0.0 ... 1.5 */
            /*-------------------*/
            throtl = min(max(throtl, 0.0F), 1.5F);
#if MANEUVER_DEBUG
if (g_nShowDebugLabels & 0x10000)
{
	char tmpchr[128];
	sprintf(tmpchr, "%.0f %.0f %.0f %.3f %.2f %.2f", m1, m2, eProp, thr, autoThrottle, throtl);
	if ( self.drawPointer )
		((DrawableBSP*)self.drawPointer).SetLabel (tmpchr, ((DrawableBSP*)self.drawPointer).LabelColor());
}
#endif

            if (fabs(eProp) < 0.1F * m1)
                return (true);
            else
                return (false);
#endif
            throw new NotImplementedException();
        }

        protected void PullUp();
        protected void RollAndPull();
        protected void PullToCollisionPoint();
        protected void PullToControlPoint();
        protected void EnergyManagement();//me123
        protected void EagManage();//me123
        protected void MaintainClosure();
        protected void MissileDefeat();
        protected void MissileDragManeuver();
        protected void MissileLastDitch(float xft, float yft, float zft);
        protected void GunsEngage();
        protected void TrainableGunsEngage();
        protected void AccelManeuver();
        protected void GunsJink();
        protected void MissileEngage();
        protected void FollowWaypoints();
        protected void Loiter()
        {
            //we don't want to loiter just above the ground
            trackZ = min(max(-20000.0F, trackZ), -5000.0F);

            if (((AircraftClass*)self).af.GetSimpleMode())
            {		// do simple flight model

                throtl = SimpleScaleThrottle(af.MinVcas() * KNOTS_TO_FTPSEC);
                //pStick = 0.0F;		// level
                pStick = SimpleTrackElevation(trackZ - self.ZPos(), 10000.0F);
                pStick = min(0.2f, max(pStick, -0.3F));
                rStick = 0.15F;	// 13.5 degree bank turn
            }
            else
            {
                /*-----------*/
                /* MACH HOLD */
                /*-----------*/
                if (curMode != lastMode)
                {
                    onStation = Stabalizing;
                    holdAlt = -self.ZPos();
                }


                if (onStation == Stabalizing)
                {
                    if (MachHold(cornerSpeed, self.Kias(), true))
                    {
                        onStation = OnStation;
                        LevelTurn(2.0F, 1.0F, true);
                    }
                    AltitudeHold(holdAlt);
                }
                else
                {
                    LevelTurn(2.0F, 1.0F, false);
                }
            }
        }

        protected void LevelTurn(float load_factor, float turnDir, int newTurn)
        {
            float edroll, elerr, alterr;

            /*-------------------------------------------*/
            /* if your not flying level, level out first */
            /*-------------------------------------------*/
            if (newTurn)
            {
                gammaHoldIError = 0.0F;
                trackMode = 0;
            }

            if (trackMode != 0)
            {
                edroll = (float)atan(sqrt(load_factor * load_factor - 1));
                ResetMaxRoll();
                SetMaxRollDelta(edroll * RTD);
                edroll *= turnDir - af.mu;

                SetRstick(edroll * RTD * 2.50F);
                if (fabs(edroll) < 5.0 * DTR || trackMode == 2)
                {
                    alterr = (holdAlt + self.ZPos() - self.ZDelta()) * 0.015F;
                    GammaHold(alterr);
                    trackMode = 2;
                }
                else
                    SetPstick(0.0F, 5.0F, AirframeClass.GCommand);
            }
            else
            {
                SetRstick(-self.Roll() * RTD);
                SetMaxRoll(0.0F);
                SetMaxRollDelta(5.0F * RTD);
                elerr = -af.gmma;
                SetPstick(elerr * RTD, 2.5F, AirframeClass.ErrorCommand);
                if (fabs(af.gmma) < 2.0 * DTR && fabs(self.Roll()) < 10.0 * DTR)
                    trackMode = 1;
            }

            SetYpedal(0.0F);
        }
        protected void GammaHold(float desGamma)
        {
            float elevCmd;
            float gammaCmd;

            desGamma = max(min(desGamma, 30.0F), -30.0F);
            elevCmd = desGamma - af.gmma * RTD;

            elevCmd *= 0.25F * self.Kias() / 350.0F;

            if (fabs(af.gmma) < (45.0F * DTR))
                elevCmd /= self.platformAngles.cosphi;

            if (elevCmd > 0.0F)
                elevCmd *= elevCmd;
            else
                elevCmd *= -elevCmd;

            gammaHoldIError += 0.0025F * elevCmd;
            if (gammaHoldIError > 1.0F)
                gammaHoldIError = 1.0F;
            else if (gammaHoldIError < -1.0F)
                gammaHoldIError = -1.0F;

            gammaCmd = gammaHoldIError + elevCmd + (1.0F / self.platformAngles.cosphi);
            SetPstick(min(max(gammaCmd, -2.0F), 6.5F), maxGs, AirframeClass.GCommand);
        }

        protected int AltitudeHold(float desAlt)
        {
            float alterr;
            int retval;

            SetYpedal(0.0F);
            SetRstick(-self.Roll() * 2.0F * RTD);
            SetMaxRoll(0.0F);


            alterr = desAlt + self.ZPos();
            if (fabs(alterr) < 25.0F)
            {
                retval = true;
            }
            else
            {
                retval = false;
            }
            alterr -= self.ZDelta();
            GammaHold(alterr * 0.015F);

            return (retval);
        }

        protected int HeadingAndAltitudeHold(float desPsi, float desAlt)
        {
            float altErr, psiErr;
            bool retval = false;
            int newTurn;
            float turnDir;

            psiErr = desPsi - self.Yaw();
            if (psiErr > 180.0F * DTR)
                psiErr -= 360.0F * DTR;
            else if (psiErr < -180.0F * DTR)
                psiErr += 360.0F * DTR;

            SetYpedal(0.0F);
            if (fabs(psiErr) < 5.0F * DTR)
            {
                SetRstick(-self.Roll() * 2.0F * RTD);
                SetMaxRoll(0.0F);
                SetMaxRollDelta(-self.Roll() * 2.0F * RTD);


                altErr = desAlt + self.ZPos();
                if (fabs(altErr) < 25.0F)
                {
                    retval = true;
                }
                altErr -= self.ZDelta();
                GammaHold(altErr * 0.015F);
            }
            else
            {
                if (psiErr > 0.0F)
                    turnDir = 1.0F;
                else
                    turnDir = -1.0F;

                if (wvrCurrTactic == wvrPrevTactic)
                    newTurn = false;
                else
                    newTurn = true;
                LevelTurn(2.0F, turnDir, newTurn);
            }

            return (retval);
        }

        protected float CollisionTime();
        protected void GoToCurrentWaypoint();
        protected void SelectNextWaypoint();
        protected void SetWaypointSpecificStuff();
        protected void RollOutOfPlane()
        {
            float eroll;

            /*-----------------------*/
            /* first pass, save roll */
            /*-----------------------*/
            if (lastMode != RoopMode)
            {
                mnverTime = 1.0F;//me123 from 4

                /*----------------------------------------------------*/
                /* want to roll toward the vertical but limit to keep */
                /* droll < 45 degrees.                                */
                /*----------------------------------------------------*/
                if (self.Roll() >= 0.0)
                {
                    newroll = self.Roll() - 30.0F * DTR;//me123 don't do a fucking quarterplane :-) from 45
                }
                else
                {
                    newroll = self.Roll() + 30.0F * DTR;//me123 don't do a fucking quarterplane :-) from 45
                }
            }

            /*------------*/
            /* roll error */
            /*------------*/
            eroll = newroll - self.Roll();

            /*-----------------------------*/
            /* roll the shortest direction */
            /*-----------------------------*/
            if (eroll < -180.0F * DTR)
                eroll += 360.0F * DTR;
            else if (eroll > 180.0F * DTR)
                eroll -= 360.0F * DTR;

            SetPstick(af.GsAvail(), maxGs, AirframeClass.GCommand);
            SetRstick(eroll * RTD);

            /*-----------*/
            /* exit mode */
            /*-----------*/
            mnverTime -= SimLibMajorFrameTime;

            if (mnverTime > 0.0)
            {
                AddMode(RoopMode);
            }
        }


        protected void OverBank(float delta)
        {
            float eroll = 0.0F;

            if (targetData == null)
                return;

            /*-------------------------*/
            /* not in a vertical fight */
            /*-------------------------*/
            if (fabs(self.Pitch()) < 45.0 * DTR)//me123 from 70
            {
                /*-----------------------*/
                /* Find a new roll angle */
                /*-----------------------*/
                if (lastMode != OverBMode)
                {
                    if (self.Roll() > 0.0F)
                        newroll = targetData.droll + delta;
                    else
                        newroll = targetData.droll - delta;

                    if (newroll > 180.0F * DTR)
                        newroll -= 360.0F * DTR;
                    else if (newroll < -180.0F * DTR)
                        newroll += 360.0F * DTR;
                }

                eroll = newroll - self.Roll();
                SetRstick(eroll * RTD);
            }

            /*------*/
            /* exit */
            /*------*/
            if (fabs(eroll) > 1.0)
            {
                AddMode(OverBMode);
            }
        }

        // ground attack stuff....
        protected void GroundAttackMode()
        {
            FireControlComputer* FCC = self.FCC;
            SMSClass* Sms = self.Sms;
            RadarClass* theRadar = (RadarClass*)FindSensor(self, SensorClass.Radar);
            float approxRange, dx, dy;
            float pitchDesired;
            float desSpeed;
            float xft, yft, zft;
            float rx, ata, ry;
            bool shootMissile;
            bool diveOK;
            float curGroundAlt;

            // 2001-06-01 ADDED BY S.G. I'LL USE missileShotTimer TO PREVENT THIS ROUTINE FROM DOING ANYTHING. THE SIDE EFFECT IS WEAPONS HOLD SHOULD DO SOMETHING AS WELL
            if (SimLibElapsedTime < missileShotTimer)
                return;
            // END OF ADDED SECTION

            // 2002-03-08 ADDED BY S.G. Turn off the lasing flag we were lasing (fail safe)
            if (SimLibElapsedTime > waitingForShot && (moreFlags & KeepLasing))
                moreFlags &= ~KeepLasing;
            // END OF ADDED SECTION 2002-03-08

            // 2001-06-18 ADDED BY S.G. AI NEED TO RE-EVALUATE ITS TARGET FROM TIME TO TIME, UNLESS THE LEAD IS THE PLAYER
            // 2001-06-18 ADDED BY S.G. NOPE, AI NEED TO RE-EVALUATE THEIR WEAPONS ONLY
            // 2001-07-10 ADDED BY S.G. IF OUR TARGET IS A RADAR AND WE ARE NOT CARRYING HARMS, SEE IF SOMEBODY ELSE COULD DO THE JOB FOR US
            // 2001-07-15 S.G. This was required when an non emitting radar could be targeted. Now check every 5 seconds if the campaign target has changed if we're the lead
            if (SimLibElapsedTime > nextAGTargetTime
                && Sms.CurRippleCount() == 0) // JB 011017 Only reevaluate when we've dropped our ripple.
            {
                // 2001-07-20 S.G. ONLY CHANGE 'onStation' IF OUR WEAPON STATUS CHANGED...
                int tempWeapon = (hasHARM << 24) | (hasAGMissile << 16) | (hasGBU << 8) | hasBomb;

                SelectGroundWeapon();

                // So if we choose a different weapon, we'll set our FCC
                if (((hasHARM << 24) | (hasAGMissile << 16) | (hasGBU << 8) | hasBomb) != tempWeapon)
                    if (onStation == Final)
                        onStation = HoldInPlace;

                // 2001-07-12 S.G. Moved below the above code
                // 2001-07-12 S.G. Simply clear the target. Code below will do the selection (and a new attack profile for us)
                // See if we should change target if we're the lead of the flight (but first we must already got a target)
                if (!isWing && lastGndCampTarget)
                {
                    UnitClass* campUnit = (UnitClass*)self.GetCampaignObject();
                    if (campUnit)
                    {
                        campUnit.UnsetChecked();
                        if (campUnit.ChooseTarget())
                        {
                            if (lastGndCampTarget != campUnit.GetTarget())
                                SetGroundTargetPtr(null);
                        }
                    }

                }

                // 2001-07-15 S.G. This was required when an non emitting radar could be targeted. This is not the case anymore so skip the whole thing
                // Try again in 5 seconds.
                nextAGTargetTime = SimLibElapsedTime + 5000;
            }

            // 2001-07-12 S.G. Need to force a target reselection under some conditions. If we're in GroundAttackMode, it's because we are already in agDoctrine != AGD_NONE which mean we already got a target originally
            // This was done is some 'onStation' modes but I moved it here instead so all modes runs it
            if (groundTargetPtr &&
                 ((groundTargetPtr.BaseData().IsSim() && // For a SIM entity
                     (groundTargetPtr.BaseData().IsDead() ||
                       groundTargetPtr.BaseData().IsExploding() ||
                       (!isWing && IsNotMainTargetSEAD() &&
                         !((SimBaseClass*)groundTargetPtr.BaseData()).GetCampaignObject().IsEmitting()))) ||
                   (groundTargetPtr.BaseData().IsCampaign() && // For a CAMPAIGN entity
                     (!isWing && IsNotMainTargetSEAD() &&
                       !((CampBaseClass*)groundTargetPtr.BaseData()).IsEmitting()))))
                SetGroundTarget(null);

            int reSetup = false;
            if (!groundTargetPtr)
            {
                // Wings run a limited version of the target selection (so they don't switch campaign target)
                if (isWing)
                    AiRunTargetSelection();
                else
                    SelectGroundTarget(TARGET_ANYTHING);

                // Since 'AiRunTargetSelection' doesn't set it but 'SelectGroundTarget' does, force it to false
                // Nope, let 'Final' and SetupAGMode deal with this
                //		madeAGPass = false;

                // Force a run of SetupAGMode if we finally got a target
                if (groundTargetPtr)
                    reSetup = true;
            }

            // If we got a deaggregated campaign object, find a sim object within
            if (groundTargetPtr && groundTargetPtr.BaseData().IsCampaign() && !((CampBaseClass*)groundTargetPtr.BaseData()).IsAggregate())
            {
                SelectGroundTarget(TARGET_ANYTHING);
                reSetup = true;
            }

            if (reSetup)
            {
                nextAGTargetTime = 0;
                SetupAGMode(null, null);
                onStation = NotThereYet;
            }
            // END OF ADDED SECTION

            // NOTES ON MODES (in approximate order):
            // 1) OnStation: Initial setup for ground run.  Track to IP.
            //	  Also determine if another run is to be made, or end ground run.
            // 2) CrossWind: head to IP
            // 3) HoldInPlace: Track Towards target
            // 4) Final: ready weapons and fire when within parms.
            // 5) Final1: release more weapoons if appropriate then go back to OnStation

            curGroundAlt = OTWDriver.GetGroundLevel(self.XPos(), self.YPos());

            if (agDoctrine == AGD_NEED_SETUP)
            {
                SetupAGMode(null, null);
            }

            // 2001-07-18 ADDED BY S.G. IF ON A LOOK_SHOOT_LOOK MODE WITH A TARGET, SEND AN ATTACK RIGHT AWAY AND DON'T WAIT TO BE CLOSE TO/ON FINAL TO DO IT BECAUSE YOU'LL MAKE SEVERAL PASS ANYWAY...

            if (groundTargetPtr && agDoctrine == AGD_LOOK_SHOOT_LOOK)
            {
                if (!isWing)
                {
                    if (sentWingAGAttack == AG_ORDER_NONE)
                    {
                        AiSendCommand(self, FalconWingmanMsg.WMTrail, AiFlight, FalconNullId);
                        sentWingAGAttack = AG_ORDER_FORMUP;
                        // 2002-01-20 ADDED BY S.G. Added the new nextAttackCommandToSend variable check to force the lead to reissue an attack in case wings went back into formation (can we say HACK?)
                        nextAttackCommandToSend = SimLibElapsedTime + 60 * SEC_TO_MSEC;
                    }
                    // 2002-01-20 ADDED BY S.G. Either we haven't sent the attack order or we sent it a while ago, check if we should send it again
                    else if (sentWingAGAttack != AG_ORDER_ATTACK || SimLibElapsedTime > nextAttackCommandToSend)
                    {
                        VU_ID targetId;

                        if (groundTargetPtr.BaseData().IsSim())
                        {
                            targetId = ((SimBaseClass*)groundTargetPtr.BaseData()).GetCampaignObject().Id();
                        }
                        else
                            targetId = groundTargetPtr.BaseData().Id();

                        if (targetId != FalconNullId)
                        {
                            // 2002-01-20 ADDED BY S.G. Check the wing's AI to see if they have weapon but in formation
                            int sendAttack = false;
                            if (SimLibElapsedTime > nextAttackCommandToSend)
                            { // timed out
                                int i;
                                int usComponents = self.GetCampaignObject().NumberOfComponents();
                                for (i = 0; i < usComponents; i++)
                                {
                                    AircraftClass* flightMember = (AircraftClass*)self.GetCampaignObject().GetComponentEntity(i);
                                    // This code is assuming the lead and the AI are on the same PC... Should be no problem unless another player is in Combat AP...
                                    if (flightMember && flightMember.DBrain() && flightMember.DBrain().IsSetATC(IsNotMainTargetSEAD() ? HasAGWeapon : HasCanUseAGWeapon) && flightMember.DBrain().agDoctrine == AGD_NONE)
                                    {
                                        sendAttack = true;
                                        break;
                                    }
                                }
                            }
                            else
                                sendAttack = true;

                            if (sendAttack)
                            {
                                AiSendCommand(self, FalconWingmanMsg.WMAssignTarget, AiFlight, targetId);
                                AiSendCommand(self, FalconWingmanMsg.WMShooterMode, AiFlight, targetId);
                                sentWingAGAttack = AG_ORDER_ATTACK;
                            }
                            // 2002-01-20 ADDED BY S.G. Added the new nextAttackCommandToSend variable check to force the lead to reissue an attack in case wings went back into formation (can we say HACK?)
                            nextAttackCommandToSend = SimLibElapsedTime + 60 * SEC_TO_MSEC;
                        }
                    }
                }
            }
            // END OF ADDED SECTION

            // modes for ground attack
            switch (onStation)
            {
                // protect against invalid state by going to our start condition
                // then fall thru
                default:
                    onStation = NotThereYet;
                // #1 setup....
                // We Should have an insertion point set up at ipX,Y and Z.....
                case NotThereYet:

                    // have we already made an AG Attack run?
                    if (madeAGPass)
                    {
                        // 2001-05-13 ADDED BY S.G. SO AI DROPS SOME COUNTER MEASURE AFTER A PASS
                        self.DropProgramed();
                        // END OF ADDED SECTION

                        // clear weapons and target
                        FCC.SetMasterMode(FireControlComputer.Nav);
                        SetGroundTarget(null);
                        FCC.preDesignate = true;

                        // if we've already made 1 pass and the doctrine is
                        // shoot_run, then we're done with this waypoint go to
                        // next.  Otherwise we're in look_shoot_look.   We'll remain
                        // at the current waypoint and allow the campaign to retarget
                        // for us
                        if (agDoctrine == AGD_SHOOT_RUN)
                        {
                            if (agImprovise == false)
                                SelectNextWaypoint();
                            // go back to initial AG state
                            agDoctrine = AGD_NONE;
                            missionComplete = true;
                            // if we're a wingie, rejoin the lead
                            if (isWing)
                            {
                                // 2001-05-03 ADDED BY S.G. WE WANT WEDGE AFTER GROUND PASS!
                                mFormation = FalconWingmanMsg.WMWedge;
                                // END OF ADDED SECTION
                                AiRejoin(null);
                                // make sure wing's designated target is null'd out
                                mDesignatedObject = FalconNullId;
                            }
                            return;
                        }

                        // 2001-06-18 MODIFIED BY S.G. I'LL USE THIS TO HAVE THE AI FORCE A RETARGET AND 15 SECONDS IS TOO HIGH
                        //				nextAGTargetTime = SimLibElapsedTime + 15000;
                        nextAGTargetTime = SimLibElapsedTime + 5000;

                        // 2001-07-12 ADDED BY S.G. SINCE WE'RE GOING AFTER SOMETHING NEW, GET A NEW TARGET RIGHT NOW SO agDoctrine DOESN'T GET SET TO AGD_NONE WHICH WILL END UP SetupAGMode BEING CALLED BEFORE THE AI HAS A CHANCE TO REACH ITS TURN POINT
                        if (isWing)
                            AiRunTargetSelection();
                        else
                            SelectGroundTarget(TARGET_ANYTHING);

                        // Since 'AiRunTargetSelection' doesn't set it but 'SelectGroundTarget' does, force it to false
                        // Nope, let 'Final' and SetupAGMode deal with this
                        //				madeAGPass = false;

                        // If we got a deaggregated campaign object, find a sim object within
                        if (groundTargetPtr && groundTargetPtr.BaseData().IsCampaign() && !((CampBaseClass*)groundTargetPtr.BaseData()).IsAggregate())
                            SelectGroundTarget(TARGET_ANYTHING);
                    }

                    // 2001-07-12 REMOVED BY S.G. DONE ABOVE BEFORE DOING onStation SPECIFIC THINGS
                    // choose how we are going to attack and whom....
                    //			SelectGroundTarget(TARGET_ANYTHING);

                    // select weapons....
                    //			SelectGroundWeapon();

                    if (!isWing && sentWingAGAttack == AG_ORDER_NONE)
                    {
                        AiSendCommand(self, FalconWingmanMsg.WMTrail, AiFlight, FalconNullId);
                        sentWingAGAttack = AG_ORDER_FORMUP;
                        // 2002-01-20 ADDED BY S.G. Added the new nextAttackCommandToSend variable check to force the lead to reissue an attack in case wings went back into formation (can we say HACK?)
                        nextAttackCommandToSend = SimLibElapsedTime + 60 * SEC_TO_MSEC;
                    }


                    // check to see if we're too close to target (if we've got one)
                    // if so set up new IP
                    if (groundTargetPtr)
                    {
                        trackX = groundTargetPtr.BaseData().XPos();
                        trackY = groundTargetPtr.BaseData().YPos();
                        dx = (self.XPos() - trackX);
                        dy = (self.YPos() - trackY);
                        approxRange = (float)sqrt(dx * dx + dy * dy);
                        // 2001-07-24 MODIFIED BY S.G. DON'T DISCRIMINATE AGMISSILES. IT'S 3.5 NM FOR EVERY WEAPON
                        //				if ( (hasAGMissile && approxRange < 7.5f * Phyconst.NM_TO_FT) || approxRange < 3.5F * Phyconst.NM_TO_FT )
                        if (approxRange < 3.5F * Phyconst.NM_TO_FT)
                        {
                            dx /= approxRange;
                            dy /= approxRange;
                            ipX = trackX + dy * 6.5f * Phyconst.NM_TO_FT;
                            ipY = trackY - dx * 6.5f * Phyconst.NM_TO_FT;
                            Debug.Assert(ipX > 0.0F);
                        }
                    }

                    // track to insertion point
                    trackX = ipX;
                    trackY = ipY;
                    trackZ = ipZ;


                    FCC.SetMasterMode(FireControlComputer.Nav);
                    FCC.preDesignate = true;


                    dx = (float)fabs(self.XPos() - trackX);
                    dy = (float)fabs(self.YPos() - trackY);
                    approxRange = (float)sqrt(dx * dx + dy * dy);


                    // Terrain follow around 1000 ft
                    // 2001-07-12 MODIFIED BY S.G. SO SEAD STAY LOW UNTIL READY TO ATTACK
                    //			if ( agApproach == AGA_LOW )
                    if (agApproach == AGA_LOW || missionType == AMIS_SEADESCORT || missionType == AMIS_SEADSTRIKE)
                    {
                        // see if we're too close to set up the ground run
                        // if so, we're going to head to a new point perpendicular to
                        // our current direction and make a run from there
                        // this is kind of a sanity check
                        trackZ = OTWDriver.GetGroundLevel(self.XPos() + self.XDelta(),
                                                           self.YPos() + self.YDelta());
                        // if we're below track alt, kick us up a bit harder so we don't plow
                        // into steeper slopes
                        if (self.ZPos() - trackZ > -1000.0f)
                            trackZ = trackZ - 1000.0f - (self.ZPos() - trackZ + 1000.0f) * 2.0f;
                        else
                            trackZ -= 1000.0f;
                        desSpeed = 650.0f;
                    }
                    else
                    {
                        // see if we're too close to set up the ground run
                        // if so, we're going to head to a new point perpendicular to
                        // our current direction and make a run from there
                        // this is kind of a sanity check
                        desSpeed = 450.0f;
                        if (self.ZPos() - curGroundAlt > -500.0f)
                        {
                            trackZ = OTWDriver.GetGroundLevel(self.XPos() + self.XDelta(),
                                                           self.YPos() + self.YDelta());
                            if (self.ZPos() - trackZ > -500.0f)
                                trackZ = trackZ - 500.0f - (self.ZPos() - trackZ + 500.0f) * 2.0f;
                            // 2001-06-18 ADDED S.G. WHY DO THIS IF WE'RE GOING UP ALREADY?
                            else if (agApproach == AGA_HIGH && trackZ > ipZ) // Are we going up? (don't forget negative is UP)
                                trackZ = ipZ;
                            // END OF ADDED SECTION
                            else
                                trackZ -= 500.0f;
                        }
                    }


                    // bombs dropped in pairs
                    Sms.SetPair(true);

                    // 2001-05-13 ADDED BY S.G. WITHOUT THIS, I WOULD GO BACK REDO THE CROSSWIND AFTER THE MISSION IS OVER! WAYPOINTS WILL OVERIDE trackXYZ SO DON'T WORRY ABOUT THEM
                    if (!missionComplete)
                        // END OF ADDED SECTION
                        // head to IP
                        onStation = Crosswind;

                    desSpeed = max(200.0F, min(desSpeed, 700.0F));	// Knots
                    TrackPoint(0.0F, desSpeed * KNOTS_TO_FTPSEC);

                    break;

                // #2 heading towards target -- we've reached the IP and are heading in
                // set up out available weapons (pref to missiles) and head in
                // also set up our final approach tracking
                case HoldInPlace:

        // 2001-07-12 REMOVED BY S.G. DONE ABOVE
                //			if ( groundTargetPtr == null || groundTargetPtr.BaseData().IsCampaign() )
                //				SelectGroundTarget(TARGET_ANYTHING);

        // 2001-07-12 S.G. Moved so a retargeting is not done
                FinalSG: // 2001-06-24 ADDED BY S.G. JUMPS BACK HERE IF TOO CLOSE FOR HARMS AND TARGET NOT EMITTING
                    dx = (float)fabs(self.XPos() - trackX);
                    dy = (float)fabs(self.YPos() - trackY);
                    approxRange = (float)sqrt(dx * dx + dy * dy);

                    if (groundTargetPtr)
                    {
                        trackX = groundTargetPtr.BaseData().XPos();
                        trackY = groundTargetPtr.BaseData().YPos();
                    }
                    trackZ = OTWDriver.GetGroundLevel(trackX, trackY);

                    // 2001-10-31 ADDED by M.N. hope this fixes the SEAD circling bug
                    if (groundTargetPtr)
                    {
                        float approxTargetRange;
                        xft = trackX - self.XPos();
                        yft = trackY - self.YPos();
                        zft = trackZ - self.ZPos();
                        approxTargetRange = (float)sqrt(xft * xft + yft * yft + zft * zft);
                        approxTargetRange = max(approxTargetRange, 1.0F);


                        /*			xft = trackX - self.XPos();
                                    yft = trackY - self.YPos();
                                    zft = trackZ - self.ZPos();
                                    approxRange = (float)sqrt(xft*xft + yft*yft + zft*zft);
                                    approxRange = max (approxRange, 1.0F);
                                    rx = self.dmx[0][0]*xft + self.dmx[0][1]*yft + self.dmx[0][2]*zft;
                                    ry = self.dmx[1][0]*xft + self.dmx[1][1]*yft + self.dmx[1][2]*zft;
                                    ata =  (float)acos(rx/approxRange); */

                        // take ata into account ??
                        // need to know minimum engagement distance for a HARM...


                        if (approxTargetRange < 3.0f * Phyconst.NM_TO_FT /*&& ata > 75.0f * DTR*/)
                        {
                            // Bail and try again
                            dx = (self.XPos() - trackX);
                            dy = (self.YPos() - trackY);
                            approxRange = (float)sqrt(dx * dx + dy * dy);
                            dx /= approxRange;
                            dy /= approxRange;
                            ipX = trackX + dy * 5.5f * Phyconst.NM_TO_FT;
                            ipY = trackY - dx * 5.5f * Phyconst.NM_TO_FT;
                            Debug.Assert(ipX > 0.0F);
                            // go to IP
                            onStation = Crosswind;
                            break;
                        }
                    }

                    // 2001-07-18 REMOVED BY S.G. agApproach SHOULDN'T MAKE A DIFFERENCE HERE
                    //			if ( hasAGMissile && agApproach != AGA_HIGH )
                    if (hasAGMissile)
                    {
                        FCC.SetMasterMode(FireControlComputer.AirGroundMissile);
                        FCC.SetSubMode(FireControlComputer.SLAVE);
                        FCC.preDesignate = true;
                        //MonoPrint ("Setup For Maverick\n");
                    }
                    else if (hasHARM)
                    {
                        FCC.SetMasterMode(FireControlComputer.AirGroundHARM);
                        FCC.SetSubMode(FireControlComputer.HTS);
                        //MonoPrint ("Setup For HARM\n");
                    }
                    else if (hasGBU)
                    {
                        FCC.SetMasterMode(FireControlComputer.AirGroundLaser);
                        FCC.SetSubMode(FireControlComputer.SLAVE);
                        FCC.preDesignate = true;
                        FCC.designateCmd = true;
                        //MonoPrint ("Setup For GBU\n");
                    }
                    else if (hasBomb)
                    {
                        FCC.SetMasterMode(FireControlComputer.AirGroundBomb);
                        FCC.SetSubMode(FireControlComputer.CCRP);

                        FCC.groundDesignateX = trackX;
                        FCC.groundDesignateY = trackX;
                        FCC.groundDesignateZ = trackZ;
                        //MonoPrint ("Setup For Iron Bomb\n");
                    }

                    // Point the radar at the target
                    if (theRadar)
                    {
                        // 2001-07-23 MODIFIED BY S.G. MOVERS ARE ONLY 3D ENTITIES WHILE BATTALIONS WILL INCLUDE 2D AND 3D VEHICLES...
                        //          if (groundTargetPtr && groundTargetPtr.BaseData().IsMover())
                        if (groundTargetPtr && ((groundTargetPtr.BaseData().IsSim() && ((SimBaseClass*)groundTargetPtr.BaseData()).GetCampaignObject().IsBattalion()) || (groundTargetPtr.BaseData().IsCampaign() && groundTargetPtr.BaseData().IsBattalion())))
                            theRadar.SetMode(RadarClass.GMT);
                        else
                            theRadar.SetMode(RadarClass.GM);
                        theRadar.SetDesiredTarget(groundTargetPtr);
                        theRadar.SetAGSnowPlow(true);
                    }

                    // Terrain follow around 1000 ft
                    // 2001-07-12 MODIFIED BY S.G. SO SEAD STAY LOW UNTIL READY TO ATTACK
                    //			if ( agApproach == AGA_LOW )
                    if (agApproach == AGA_LOW || missionType == AMIS_SEADESCORT || missionType == AMIS_SEADSTRIKE)
                    {
                        trackZ = OTWDriver.GetGroundLevel(self.XPos() + self.XDelta(),
                                                           self.YPos() + self.YDelta());
                        // if we're below track alt, kick us up a bit harder so we don't plow
                        // into steeper slopes
                        if (self.ZPos() - trackZ > -1000.0f)
                            trackZ = trackZ - 1000.0f - (self.ZPos() - trackZ + 1000.0f) * 2.0f;
                        else
                            trackZ -= 1000.0f;
                        desSpeed = 650.0f;
                    }
                    else if (agApproach == AGA_DIVE)
                    {
                        trackZ = OTWDriver.GetGroundLevel(trackX, trackY);
                        if (Sms.curWeapon)
                            trackZ -= 100.0f;
                        desSpeed = 450.0f;
                        if (self.ZPos() - curGroundAlt > -1000.0f)
                        {
                            trackZ = OTWDriver.GetGroundLevel(self.XPos() + self.XDelta(),
                                                           self.YPos() + self.YDelta());
                            if (self.ZPos() - trackZ > -500.0f)
                                trackZ = trackZ - 500.0f - (self.ZPos() - trackZ + 500.0f) * 2.0f;
                            else
                                trackZ -= 500.0f;
                        }
                    }
                    else
                    {
                        // 2002-03-28 MN fix for AI handling of JSOW/JDAM missiles - fire from defined altitude instead of 4000ft like Mavericks
                        if ((g_nMissileFix & 0x01) && Sms && Sms.curWeapon)
                        {
                            Falcon4EntityClassType* classPtr = null;
                            WeaponClassDataType* wc = null;

                            classPtr = (Falcon4EntityClassType*)Sms.curWeapon.EntityType();
                            if (classPtr)
                            {
                                wc = (WeaponClassDataType*)classPtr.dataPtr;
                                if (wc && (wc.Flags & WEAP_BOMBWARHEAD))
                                {
                                    ipZ = -g_fBombMissileAltitude;
                                }
                            }
                        }
                        trackZ = ipZ;
                        desSpeed = 450.0f;
                        if (self.ZPos() - curGroundAlt > -500.0f)
                        {
                            trackZ = OTWDriver.GetGroundLevel(self.XPos() + self.XDelta(),
                                                           self.YPos() + self.YDelta());
                            if (self.ZPos() - trackZ > -500.0f)
                                trackZ = trackZ - 500.0f - (self.ZPos() - trackZ + 500.0f) * 2.0f;
                            // 2001-06-18 ADDED S.G. WHY DO THIS IF WE'RE GOING UP ALREADY?
                            else if (trackZ > ipZ) // Are we going up? (don't forget negative is UP)
                                trackZ = ipZ;
                            // END OF ADDED SECTION
                            else
                                trackZ -= 500.0f;
                        }
                    }

                    onStation = Final;
                    // 2001-05-21 ADDED BY S.G. ONLY DO THIS IF NOT WITHIN TIMEOUT PERIOD. TO BE SAFE, I'LL SET waitingForShot TO 0 IN digimain SO IT IS INITIALIZED
                    if (waitingForShot < SimLibElapsedTime - 5000)
                        // Say you can fire when ready
                        // END OF ADDED SECTION
                        waitingForShot = SimLibElapsedTime - 1;

                    TrackPoint(0.0F, desSpeed * KNOTS_TO_FTPSEC);
                    // SimpleTrack(SimpleTrackSpd, desSpeed * KNOTS_TO_FTPSEC);

                    break;

                // case 1a: head to good start location (IP)
                case Crosswind:
                    trackX = ipX;
                    trackY = ipY;
                    trackZ = ipZ;

                    dx = (float)fabs(self.XPos() - trackX);
                    dy = (float)fabs(self.YPos() - trackY);
                    approxRange = (float)sqrt(dx * dx + dy * dy);

                    // Terrain follow around 1000 ft
                    // 2001-07-12 MODIFIED BY S.G. SO SEAD STAY LOW UNTIL READY TO ATTACK
                    //			if ( agApproach == AGA_LOW )
                    if (agApproach == AGA_LOW || missionType == AMIS_SEADESCORT || missionType == AMIS_SEADSTRIKE)
                    {
                        trackZ = OTWDriver.GetGroundLevel(self.XPos() + self.XDelta(),
                                                           self.YPos() + self.YDelta());
                        // if we're below track alt, kick us up a bit harder so we don't plow
                        // into steeper slopes
                        if (self.ZPos() - trackZ > -1000.0f)
                            trackZ = trackZ - 1000.0f - (self.ZPos() - trackZ + 1000.0f) * 2.0f;
                        else
                            trackZ -= 1000.0f;
                        desSpeed = 650.0f;
                    }
                    else
                    {
                        desSpeed = 450.0f;
                        if (self.ZPos() - curGroundAlt > -500.0f)
                        {
                            trackZ = OTWDriver.GetGroundLevel(self.XPos() + self.XDelta(),
                                                           self.YPos() + self.YDelta());
                            if (self.ZPos() - trackZ > -500.0f)
                                trackZ = trackZ - 500.0f - (self.ZPos() - trackZ + 500.0f) * 2.0f;
                            // 2001-06-18 ADDED S.G. WHY DO THIS IF WE'RE GOING UP ALREADY?
                            else if (agApproach == AGA_HIGH && trackZ > ipZ) // Are we going up? (don't forget negative is UP)
                                trackZ = ipZ;
                            // END OF ADDED SECTION
                            else
                                trackZ -= 500.0f;
                        }
                    }

                    TrackPoint(0.0F, desSpeed * KNOTS_TO_FTPSEC);

                    // 2001-05-05 ADDED BY S.G. THIS IS TO MAKE THE AI PULL AGGRESIVELY AFTER A PASS. I WOULD HAVE LIKE TESTING madeAGPass BUT IT IS CLEARED BEFORE :-(
                    // Increase the gains on final approach
                    rStick *= 3.0f;
                    if (rStick > 1.0f)
                        rStick = 1.0f;
                    else if (rStick < -1.0f)
                        rStick = -1.0f;
                    // END OF ADDED SECTION

                    // 2001-07-12 ADDED BY S.G. IF CLOSE TO THE FINAL POINT, SEND AN ATTACK COMMAND TO THE WINGS
                    // tell our wing to attack
                    if (groundTargetPtr && approxRange < 5.0f * Phyconst.NM_TO_FT)
                    {
                        if (!isWing && sentWingAGAttack != AG_ORDER_ATTACK)
                        {
                            VU_ID targetId;

                            if (groundTargetPtr.BaseData().IsSim())
                            {
                                targetId = ((SimBaseClass*)groundTargetPtr.BaseData()).GetCampaignObject().Id();
                            }
                            else
                                targetId = groundTargetPtr.BaseData().Id();

                            if (targetId != FalconNullId)
                            {
                                AiSendCommand(self, FalconWingmanMsg.WMAssignTarget, AiFlight, targetId);
                                AiSendCommand(self, FalconWingmanMsg.WMShooterMode, AiFlight, targetId);
                                sentWingAGAttack = AG_ORDER_ATTACK;
                                // 2002-01-20 ADDED BY S.G. Added the new nextAttackCommandToSend variable check to force the lead to reissue an attack in case wings went back into formation (can we say HACK?)
                                nextAttackCommandToSend = SimLibElapsedTime + 60 * SEC_TO_MSEC;
                            }
                        }
                    }
                    // END OF ADDED SECTION

                    // are we about at our IP?
                    // 2001-07-23 MODIFIED BY S.G. IF NO WEAPON AVAIL FOR TARGET, SWITCH TO FINAL RIGHT AWAY
                    //			if ( approxRange < 1.3f * Phyconst.NM_TO_FT)
                    if (approxRange < 1.3f * Phyconst.NM_TO_FT || !IsSetATC(HasCanUseAGWeapon))
                    {
                        // next mode
                        onStation = HoldInPlace;

                        // 2001-07-15 ADDED BY S.G. IF madeAGPass IS true, WE MADE AN A2G PASS AND WAS TURNING AWAY. REDO AN ATTACK PROFILE FOR A NEW PASS
                        if (madeAGPass)
                        {
                            madeAGPass = false;
                            onStation = NotThereYet;
                        }

                        // 2001-07-12 ADDED BY S.G. MAKE SURE OUR IP ALTITUDE IS RIGHT BEFORE SWITCHING TO 'HoldInPlace' (WHICH IS A PRESET FOR Final)
                        SetupAGMode(null, null);
                    }

                    break;

                case Downwind:
                    break;

                case Base:
                    break;

                // #3 -- final attack approach
                case Final:

                    ClearFlag(GunFireFlag);

                    // double check to make sure ground target is still alive if
                    // it's a sim target
                    // 2001-07-12 REMOVED BY S.G. DONE BEFORE THE switch STATEMENT
                    /*
                                if ( groundTargetPtr &&
                                     groundTargetPtr.BaseData().IsSim() &&
                                     ( groundTargetPtr.BaseData().IsDead() ||
                                       groundTargetPtr.BaseData().IsExploding() ) )
                                {
                                    SetGroundTarget( null );
                                }

                                if ( groundTargetPtr == null || groundTargetPtr.BaseData().IsCampaign() )
                                    SelectGroundTarget(TARGET_ANYTHING);
                    */
                    if (groundTargetPtr)
                    {
                        trackX = groundTargetPtr.BaseData().XPos();
                        trackY = groundTargetPtr.BaseData().YPos();

                        // tell our wing to attack
                        if (!isWing && sentWingAGAttack != AG_ORDER_ATTACK)
                        {
                            VU_ID targetId;

                            if (groundTargetPtr.BaseData().IsSim())
                            {
                                targetId = ((SimBaseClass*)groundTargetPtr.BaseData()).GetCampaignObject().Id();
                            }
                            else
                                targetId = groundTargetPtr.BaseData().Id();

                            if (targetId != FalconNullId)
                            {
                                AiSendCommand(self, FalconWingmanMsg.WMAssignTarget, AiFlight, targetId);
                                AiSendCommand(self, FalconWingmanMsg.WMShooterMode, AiFlight, targetId);
                                sentWingAGAttack = AG_ORDER_ATTACK;
                                // 2002-01-20 ADDED BY S.G. Added the new nextAttackCommandToSend variable check to force the lead to reissue an attack in case wings went back into formation (can we say HACK?)
                                nextAttackCommandToSend = SimLibElapsedTime + 60 * SEC_TO_MSEC;
                            }
                        }
                    }

                    diveOK = false;

                    // Terrain follow around 1000 ft
                    if (agApproach == AGA_LOW)
                    {
                        // if we're below track alt, kick us up a bit harder so we don't plow
                        // into steeper slopes
                        if (Sms.curWeapon && !Sms.curWeapon.IsMissile() && Sms.curWeapon.IsBomb())
                        {
                            if (self.ZPos() - curGroundAlt > -1000.0f)
                            {
                                trackZ = OTWDriver.GetGroundLevel(self.XPos() + self.XDelta(),
                                                               self.YPos() + self.YDelta());
                                if (self.ZPos() - trackZ > -1000.0f)
                                    trackZ = trackZ - 1000.0f - (self.ZPos() - trackZ + 1000.0f) * 2.0f;
                                else
                                    trackZ -= 1000.0f;
                            }
                            else
                            {
                                trackZ = OTWDriver.GetGroundLevel(trackX, trackY) - 4000.0f;
                            }
                        }
                        else
                        {
                            trackZ = OTWDriver.GetGroundLevel(self.XPos() + self.XDelta(),
                                                           self.YPos() + self.YDelta());
                            if (self.ZPos() - trackZ > -1000.0f)
                                trackZ = trackZ - 1000.0f - (self.ZPos() - trackZ + 1000.0f) * 2.0f;
                            else
                                trackZ -= 1000.0f;
                        }
                        if (madeAGPass)
                            desSpeed = 450.0f;
                        else
                            desSpeed = 650.0f;
                    }
                    else if (agApproach == AGA_DIVE)
                    {
                        if (self.ZPos() - curGroundAlt > -1000.0f)
                        {
                            trackZ = OTWDriver.GetGroundLevel(self.XPos() + self.XDelta(),
                                                           self.YPos() + self.YDelta());
                            if (self.ZPos() - trackZ > -500.0f)
                                trackZ = trackZ - 500.0f - (self.ZPos() - trackZ + 500.0f) * 2.0f;
                            else
                                trackZ -= 500.0f;
                        }
                        else
                        {
                            trackZ = OTWDriver.GetGroundLevel(trackX, trackY);
                            diveOK = true;
                            if (Sms.curWeapon && !Sms.curWeapon.IsMissile() && Sms.curWeapon.IsBomb())
                                trackZ -= 2000.0f;
                            else if (Sms.curWeapon)
                                trackZ -= 50.0f;
                        }
                        desSpeed = 450.0f;
                    }
                    else
                    {
                        trackZ = ipZ;
                        desSpeed = 450.0f;
                        if (self.ZPos() - curGroundAlt > -1000.0f)
                        {
                            trackZ = OTWDriver.GetGroundLevel(self.XPos() + self.XDelta(),
                                                           self.YPos() + self.YDelta());
                            if (self.ZPos() - trackZ > -500.0f)
                                trackZ = trackZ - 500.0f - (self.ZPos() - trackZ + 500.0f) * 2.0f;
                            // 2001-06-18 ADDED S.G. WHY DO THIS IF WE'RE GOING UP ALREADY?
                            else if (trackZ > ipZ) // Are we going up? (don't forget negative is UP)
                                trackZ = ipZ;
                            // END OF ADDED SECTION
                            else
                                trackZ -= 500.0f;
                        }
                    }

                    // if we've got a missile get r mmax and min (which should be
                    // fairly accurate now.
                    // also we're going to predetermine if we'll take a missile shot
                    // or not (mostly for harms).
                    shootMissile = false;
                    droppingBombs = false;

                    // get accurate range and ata to target
                    xft = trackX - self.XPos();
                    yft = trackY - self.YPos();
                    zft = trackZ - self.ZPos();
                    approxRange = (float)sqrt(xft * xft + yft * yft + zft * zft);
                    approxRange = max(approxRange, 1.0F);

                    // check for target
                    if (groundTargetPtr == null)
                    {
                        if (approxRange < 1000.0f)
                            onStation = NotThereYet;
                        break;
                    }

                    rx = self.dmx[0][0] * xft + self.dmx[0][1] * yft + self.dmx[0][2] * zft;
                    ry = self.dmx[1][0] * xft + self.dmx[1][1] * yft + self.dmx[1][2] * zft;
                    ata = (float)acos(rx / approxRange);

                    // check for a photo mission
                    if ((missionType == AMIS_BDA || missionType == AMIS_RECON) && hasCamera)
                    {
                        TakePicture(approxRange, ata);
                    }
                    // Might shoot a missile
                    else
                    {
                        // preference given to stand-off missiles unless our
                        // approach is high alt (bombing run)
                        // 2001-07-18 ADDED BY S.G. DON'T GO TO THE TARGET BUT DO ANOTHER PASS FROM IP IF YOU HAVE NO WEAPONS LEFT!
                        if (!IsSetATC(HasCanUseAGWeapon))
                        {
                            onStation = NotThereYet;
                            break;
                        }
                        // END OF ADDED SECTION

                        if ((hasAGMissile || hasHARM) && groundTargetPtr)
                        {
                            if (hasHARM)
                                shootMissile = HARMSetup(rx, ry, ata, approxRange);
                            else
                                shootMissile = MaverickSetup(rx, ry, ata, approxRange, theRadar);

                            // Fire when ready Gridley
                            // 2001-06-24 MODIFIED BY S.G. shootMissile is true, YOU'RE CLEAR TO SHOOT
                            //			      if ( shootMissile )
                            if (shootMissile == true)
                            {
                                FireAGMissile(approxRange, ata);
                            }
                            // 2001-06-24 ADDED BY S.G. IF shootMissile IS NOT false (AND ALSO NOT true), IF WE'RE CLOSE AND CAN'T LAUNCH HARM, SAY 'NO HARMS' AND TRY AGAIN RIGHT AWAY
                            else if (shootMissile != false)
                            {
                                hasHARM = false;
                                goto FinalSG;
                            }
                            // END OF ADDED SECTION
                        }
                        else if (hasGBU)
                        {
                            DropGBU(approxRange, ata, theRadar);
                        }
                        else if (hasBomb)
                        {
                            DropBomb(approxRange, ata, theRadar);
                        }
                        // rocket strafe attack
                        else if (hasRocket)
                        {
                            FireRocket(approxRange, ata);
                        }
                        // gun strafe attack
                        else if (hasGun && agApproach == AGA_DIVE)
                        {
                            GunStrafe(approxRange, ata);
                        }
                        // too close ?
                        // we're within a certain range and our ATA is not good
                        else if (approxRange < 1.2f * Phyconst.NM_TO_FT && ata > 75.0f * DTR)
                        {
                            waitingForShot = SimLibElapsedTime + 5000;
                            onStation = Final1;
                        }
                    }

                    TrackPoint(0.0F, desSpeed * KNOTS_TO_FTPSEC);

                    dx = (float)fabs(self.XPos() - trackX);
                    dy = (float)fabs(self.YPos() - trackY);
                    approxRange = (float)sqrt(dx * dx + dy * dy);

                    // Increase the gains on final approach
                    rStick *= 3.0f;
                    if (rStick > 1.0f)
                        rStick = 1.0f;
                    else if (rStick < -1.0f)
                        rStick = -1.0f;

                    // pitch stick setting is based on our desired angle normalized to
                    // 90 deg when in a dive
                    if (agApproach == AGA_DIVE && diveOK)
                    {
                        // check hitting the ground and pull out of dive
                        pitchDesired = (float)atan2(self.ZPos() - trackZ, approxRange);
                        pitchDesired /= (90.0f * DTR);

                        // keep stick at reasonable values.
                        pStick = max(-0.7f, pitchDesired);
                        pStick = min(0.7f, pStick);
                    }


                    break;

                // #4 -- final attack approach hold for a sec and then head to next
                case Final1:

                    if (Sms.CurRippleCount() > 0) // JB 011013
                        break;

                    ClearFlag(GunFireFlag);
                    diveOK = false;

                    FCC.releaseConsent = PilotInputs.Off;

                    // mark that we've completed an AG pass
                    madeAGPass = true;

                    trackX = self.XPos() + self.dmx[0][0] * 1000.0f;
                    trackY = self.YPos() + self.dmx[0][1] * 1000.0f;
                    trackZ = self.ZPos();

                    // Terrain follow around 1000 ft
                    if (agApproach == AGA_LOW)
                    {
                        trackZ = OTWDriver.GetGroundLevel(self.XPos() + self.XDelta(),
                                                           self.YPos() + self.YDelta());
                        // if we're below track alt, kick us up a bit harder so we don't plow
                        // into steeper slopes
                        if (self.ZPos() - trackZ > -1000.0f)
                            trackZ = trackZ - 1000.0f - (self.ZPos() - trackZ + 1000.0f) * 2.0f;
                        else
                            trackZ -= 1000.0f;
                        desSpeed = 650.0f;
                    }
                    else if (agApproach == AGA_DIVE)
                    {
                        desSpeed = 450.0f;
                        if (self.ZPos() - curGroundAlt > -1000.0f)
                        {
                            trackZ = OTWDriver.GetGroundLevel(self.XPos() + self.XDelta(),
                                                           self.YPos() + self.YDelta());
                            if (self.ZPos() - trackZ > -500.0f)
                                trackZ = trackZ - 500.0f - (self.ZPos() - trackZ + 500.0f) * 2.0f;
                            else
                                trackZ -= 500.0f;
                        }
                        else
                        {
                            trackZ = OTWDriver.GetGroundLevel(trackX, trackY);
                            diveOK = true;
                            if (Sms.curWeapon && !Sms.curWeapon.IsMissile() && Sms.curWeapon.IsBomb())
                                trackZ -= 2000.0f;
                            else if (Sms.curWeapon)
                                trackZ -= 50.0f;
                        }
                    }
                    else
                    {
                        // trackZ = ipZ;
                        desSpeed = 450.0f;
                        if (self.ZPos() - curGroundAlt > -500.0f)
                        {
                            trackZ = OTWDriver.GetGroundLevel(self.XPos() + self.XDelta(),
                                                           self.YPos() + self.YDelta());
                            if (self.ZPos() - trackZ > -500.0f)
                                trackZ = trackZ - 500.0f - (self.ZPos() - trackZ + 500.0f) * 2.0f;
                            else
                                trackZ -= 500.0f;
                        }
                    }

                    SimpleTrackSpeed(desSpeed * KNOTS_TO_FTPSEC);

                    // dx = ( self.XPos() - trackX );
                    // dy = ( self.YPos() - trackY );
                    // approxRange = sqrt( dx*dx + dy*dy );

                    dx = (float)fabs(self.XPos() - trackX);
                    dy = (float)fabs(self.YPos() - trackY);
                    approxRange = (float)sqrt(dx * dx + dy * dy);



                    // pitch stick setting is based on our desired angle normalized to
                    // 90 deg
                    if (agApproach == AGA_DIVE && diveOK)
                    {
                        pitchDesired = (float)atan2(self.ZPos() - trackZ, approxRange);
                        pitchDesired /= (90.0f * DTR);

                        // keep stick at reasonable values.
                        pStick = max(-0.7f, pitchDesired);
                        pStick = min(0.7f, pStick);
                    }

                    if (missionType == AMIS_BDA || missionType == AMIS_RECON)
                    {
                        // clear and get new target next pass
                        madeAGPass = true;
                        onStation = NotThereYet;
                    }
                    else if ((hasAGMissile || hasHARM) && !droppingBombs)
                    {
                        if (SimLibElapsedTime > waitingForShot)
                        {
                            if (agDoctrine == AGD_SHOOT_RUN && groundTargetPtr)
                            {
                                // clear and get new target next pass
                                SetGroundTarget(null);

                                // this takes us back to missile fire
                                onStation = Final;
                            }
                            else // LOOK_SHOOT_LOOK
                            {
                                // this takes us back to 1st state
                                SetGroundTarget(null);
                                onStation = NotThereYet;
                            }
                        }
                    }
                    else if (droppingBombs == wcBombWpn)
                    {
                        DropBomb(approxRange, 0.0F, theRadar);
                    }
                    else if (droppingBombs == wcGbuWpn)
                    {
                        DropGBU(approxRange, 0.0F, theRadar);
                    }
                    else if (SimLibElapsedTime > waitingForShot || !hasWeapons)
                    {
                        // this takes us back to 1st state
                        SetGroundTarget(null);
                        onStation = NotThereYet;
                    }

                    TrackPoint(0.0F, desSpeed * KNOTS_TO_FTPSEC);

                    break;

                // after bombing run, we come here to pull up
                case Stabalizing:
                    dx = (float)fabs(self.XPos() - trackX);
                    dy = (float)fabs(self.YPos() - trackY);
                    approxRange = (float)sqrt(dx * dx + dy * dy);

                    if (approxRange < 1000.0f)
                        onStation = NotThereYet;

                    SetGroundTarget(null);

                    TrackPoint(0.0F, 650.0f * KNOTS_TO_FTPSEC);

                    break;

                case Taxi:
                    break;
            }

            // Been doing this long enough, go to the next waypoint
            if (groundTargetPtr && SimLibElapsedTime > mergeTimer)
            {
                // 2001-06-04 ADDED BY S.G. FORGET YOU WERE ON A GROUND PASS OR YOU'LL KEEP CIRCLING!
                agDoctrine = AGD_NONE;
                // END OF ADDED SECTION
                SelectNextWaypoint();
            }
        }

        protected void SetGroundTarget(FalconEntity* obj)
        {
            if (obj != null)
            {
                if (groundTargetPtr != null)
                {
                    // release existing target data if different object
                    if (groundTargetPtr.BaseData() != obj)
                    {
                        groundTargetPtr.Release(SIM_OBJ_REF_ARGS);
                        groundTargetPtr = null;
                    }
                    else
                    {
                        // already targeting this object
                        return;
                    }
                }

                // create new target data and reference it
#if DEBUG
                groundTargetPtr = new SimObjectType(OBJ_TAG, self, obj);
#else
		groundTargetPtr = new SimObjectType( obj );
#endif
                groundTargetPtr.Reference(SIM_OBJ_REF_ARGS);
                // SetTarget( groundTargetPtr );
            }
            else // obj is null
            {
                if (groundTargetPtr != null)
                {
                    groundTargetPtr.Release(SIM_OBJ_REF_ARGS);
                    groundTargetPtr = null;
                }
            }
        }


        protected void SetGroundTargetPtr(SimObjectType* obj)
        {
            if (obj != null)
            {
                if (groundTargetPtr != null)
                {
                    // release existing target data if different object
                    if (groundTargetPtr != obj)
                    {
                        groundTargetPtr.Release(SIM_OBJ_REF_ARGS);
                        groundTargetPtr = null;
                    }
                    else
                    {
                        // already targeting this object
                        return;
                    }
                }

                // set and reference
                groundTargetPtr = obj;
                groundTargetPtr.Reference(SIM_OBJ_REF_ARGS);
                // SetTarget( groundTargetPtr );
            }
            else // obj is null
            {
                if (groundTargetPtr != null)
                {
                    groundTargetPtr.Release(SIM_OBJ_REF_ARGS);
                    groundTargetPtr = null;
                }
            }
        }


        protected void SelectGroundTarget(int selectFilter)
        {
            CampBaseClass* campTarg;
            Tpoint pos;
            VuGridIterator* gridIt = null;
            UnitClass* campUnit = (UnitClass*)self.GetCampaignObject();
            WayPointClass dwp, cwp;
            int relation;

            // No targeting when RTB
            if (curMode == RTBMode)
            {
                SetGroundTarget(null);
                return;
            }

            // 1st let camp select target
            SelectCampGroundTarget();


            // if we've got one we're done
            if (groundTargetPtr)
                return;

            // if we're not on interdiction type mission, return....
            if (missionType != AMIS_SAD && missionType != AMIS_INT && missionType != AMIS_BAI)
                return;

            if (!campUnit)
                return;

            // divert waypoint overrides everything else
            dwp = ((FlightClass*)campUnit).GetOverrideWP();
            if (!dwp)
                cwp = self.curWaypoint;
            else
                cwp = dwp;

            if (!cwp || cwp.GetWPAction() != WP_SAD)
                return;


            // choose how we are going to attack and whom....
            cwp.GetLocation(&pos.x, &pos.y, &pos.z);

#if VU_GRID_TREE_Y_MAJOR
    gridIt = new VuGridIterator(RealUnitProxList, pos.y, pos.x, 5.0F * Phyconst.NM_TO_FT);
#else
            gridIt = new VuGridIterator(RealUnitProxList, pos.x, pos.y, 5.0F * Phyconst.NM_TO_FT);
#endif


            // get the 1st objective that contains the bomb
            campTarg = (CampBaseClass*)gridIt.GetFirst();
            while (campTarg != null)
            {
                relation = TeamInfo[self.GetTeam()].TStance(campTarg.GetTeam());
                if (relation == Hostile || relation == War)
                {
                    break;
                }
                campTarg = (CampBaseClass*)gridIt.GetNext();
            }

            SetGroundTarget(campTarg);

            //TODO if (gridIt)
            //TODO delete gridIt;
            return;
        }


        protected void SelectCampGroundTarget()
        {
            UnitClass* campUnit = (UnitClass*)self.GetCampaignObject();
            FalconEntity* target = null;
            // int i, numComponents;
            SimBaseClass* simTarg;
            WayPointClass* dwp;

            // 2001-07-15 REMOVED BY S.G. THIS IS CLEARED IN THE 'Final' STAGE ONLY
            //	madeAGPass = false;

            agImprovise = false;

            // sanity check
            if (!campUnit)
                return;

            // divert waypoint overrides everything else
            dwp = ((FlightClass*)campUnit).GetOverrideWP();

            // check to see if our current ground target is a sim and exploding or
            // dead, if so let's get a new target from the campaign
            if (groundTargetPtr &&
                 groundTargetPtr.BaseData().IsSim() &&
                 (groundTargetPtr.BaseData().IsExploding() ||
                 !((SimBaseClass*)groundTargetPtr.BaseData()).IsAwake()))
            {
                SetGroundTarget(null);
            }

            // see if we've already got a target
            if (groundTargetPtr)
            {
                target = groundTargetPtr.BaseData();

                // is it a campaign object? if not we can return....
                if (target.IsSim())
                {
                    return;
                }

                // itsa campaign object.  Check to see if its deagg'd
                if (((CampBaseClass*)target).IsAggregate())
                {
                    // still aggregated, return
                    return;
                }

                // the campaign object is now deaggregated, choose a sim entity
                // to target on it
                // 2001-04-11 MODIFIED BY S.G. SO LEAD USES THE ASSIGNED TARGET IF IT'S AN OBJECTIVE AND MAKES A BETTER SELECTION ON MOVERS
                /*		numComponents = ((CampBaseClass*)target).NumberOfComponents();

                        for ( i = 0; i < numComponents; i++ )
                        {
                            simTarg = ((CampBaseClass*)target).GetComponentEntity( rand() % numComponents );
                            if ( !simTarg ) //sanity check
                                continue;

                            // don't target runways (yet)
                            if ( // !simTarg.IsSetCampaignFlag (FEAT_FLAT_CONTAINER) &&
                                !simTarg.IsExploding() &&
                                !simTarg.IsDead() &&
                                simTarg.pctStrength > 0.0f )
                            {
                                SetGroundTarget( simTarg );
                                break;
                            }
                        } // end for # components
                */
                int targetNum = 0;

                // First, the lead will go for the assigned target, if any...
                if (!isWing && target.IsObjective())
                {
                    FalconEntity* wpTarget = null;
                    WayPointClass* twp = self.curWaypoint;

                    // First prioritize the divert waypoint target
                    if (dwp)
                        wpTarget = dwp.GetWPTarget();

                    // If wpTarget is not null, our waypoint will be the divert waypoint
                    if (wpTarget)
                        twp = dwp;
                    else
                    {
                        // Our target will be the current waypoint target if any
                        if (self.curWaypoint)
                            wpTarget = self.curWaypoint.GetWPTarget();
                    }

                    // If we have a waypoint target and it is our current target
                    if (wpTarget && wpTarget == target)
                        // Our feature is the one assigned to us by the mission planner
                        targetNum = twp.GetWPTargetBuilding();
                }

                // Use our helper function
                simTarg = FindSimGroundTarget((CampBaseClass*)target, ((CampBaseClass*)target).NumberOfComponents(), targetNum);

                // Hopefully, we have one...
                SetGroundTarget(simTarg);
                // END OF ADDED SECTION

                return;

            } // end if already targetPtr

            // priority goes to the waypoint target
            if (dwp)
            {
                target = dwp.GetWPTarget();
                if (!target)
                {
                    dwp = null;
                    if (self.curWaypoint)
                    {
                        target = self.curWaypoint.GetWPTarget();
                    }
                }
            }
            else if (self.curWaypoint)
            {
                target = self.curWaypoint.GetWPTarget();
            }

            if (target && target.OnGround())
            {
                SetGroundTarget(target);
                return;
            }


            // at this point we have no target, we're going to ask the campaign
            // to find out what we're supposed to hit

            // tell unit we haven't done any checking on it yet
            campUnit.UnsetChecked();

            // choose target.  I assume if this returns 0, no target....
            // 2001-06-09 MODIFIED BY S.G. NEED TO SEE IF WE ARE CHANGING CAMPAIGN TARGET AND ON A SEAD ESCORT MISSION. IF SO, DEAL WITH IT
            /*	if ( !campUnit.ChooseTarget() )
                {
                    // alternately try and choose the waypoint's target
                    // SetGroundTarget( self.curWaypoint.GetWPTarget() );
                    return;
                }

                // get the target
                target = campUnit.GetTarget();
            */
            // Choose and get this target
            int ret;
            ret = campUnit.ChooseTarget();
            target = campUnit.GetTarget();

            // If ChooseTarget returned false or we changed campaign target and we're the lead  (but we must had a previous campaign target first)
            if (!isWing && lastGndCampTarget && (!ret || target != lastGndCampTarget))
            {
                agDoctrine = AGD_NONE;														// Need to setup our next ground attack
                onStation = NotThereYet;													// Need to do a new pass next time
                sentWingAGAttack = AG_ORDER_NONE;											// Next time, direct wingmen on target
                lastGndCampTarget = null;													// No previous campaign target
                AiSendCommand(self, FalconWingmanMsg.WMWedge, AiFlight, FalconNullId);	// Ask wingmen to resume a wedge formation
                AiSendCommand(self, FalconWingmanMsg.WMRejoin, AiFlight, FalconNullId);	// Ask wingmen to rejoin
                AiSendCommand(self, FalconWingmanMsg.WMCoverMode, AiFlight, FalconNullId);// And stop what they were doing
            }
            else
                // Keep track of this campaign target
                lastGndCampTarget = (CampBaseClass*)target;

            if (!ret)
                return;

            // END OF MODIFIED SECTION

            // get tactic -- not doing anything with it now
            campUnit.ChooseTactic();
            campTactic = campUnit.GetUnitTactic();

            // sanity check and make sure its on ground, what to do if not?!...
            if (!target ||
                 !target.OnGround() ||
                 (campTactic != ATACTIC_ENGAGE_STRIKE &&
                  campTactic != ATACTIC_ENGAGE_SURFACE &&
                  campTactic != ATACTIC_ENGAGE_DEF &&
                  campTactic != ATACTIC_ENGAGE_NAVAL))
                return;


            // set it as our target
            SetGroundTarget(target);

        }

        protected void DoPickupAirdrop();
        protected void TakePicture(float approxRange, float ata)
        {
            FireControlComputer* FCC = self.FCC;

            // Go to camera mode
            if (FCC.GetMasterMode() != FireControlComputer.AirGroundCamera)
            {
                FCC.SetMasterMode(FireControlComputer.AirGroundCamera);
            }

            if (groundTargetPtr == null)
            {
                onStation = Final1;
            }
            else if (approxRange < 3.0f * Phyconst.NM_TO_FT && ata < 10.0f * DTR)
            {
                // take picture
                waitingForShot = SimLibElapsedTime;
                onStation = Final1;
                SetFlag(MslFireFlag);
            }
        }
        protected void DropBomb(float approxRange, float ata, RadarClass* theRadar)
        {
            FireControlComputer* FCC = self.FCC;
            SMSClass* Sms = self.Sms;
            float dx, dy;

            F4Assert(!Sms.curWeapon || Sms.curWeapon.IsBomb());

            // Make sure the FCC is in the right mode/sub mode
            if (FCC.GetMasterMode() != FireControlComputer.AirGroundBomb ||
                 FCC.GetSubMode() != FireControlComputer.CCRP)
            {
                FCC.SetMasterMode(FireControlComputer.AirGroundBomb);
                FCC.SetSubMode(FireControlComputer.CCRP);
            }

            if (!Sms.curWeapon || !Sms.curWeapon.IsBomb())
            {
                if (Sms.FindWeaponClass(wcBombWpn))
                    Sms.SetWeaponType(Sms.hardPoint[Sms.CurHardpoint()].GetWeaponType());
                else
                    Sms.SetWeaponType(wtNone);
            }

            // Point the radar at the target
            if (theRadar)
            {
                if (groundTargetPtr && groundTargetPtr.BaseData().IsMover())
                    theRadar.SetMode(RadarClass.GMT);
                else
                    theRadar.SetMode(RadarClass.GM);
                theRadar.SetDesiredTarget(groundTargetPtr);
                theRadar.SetAGSnowPlow(true);
            }

            // Mode the SMS
            Sms.SetPair(true);

            // Give the FCC permission to release if in parameters
            SetFlag(MslFireFlag);

            // Adjust for wind/etc
            if (fabs(FCC.airGroundBearing) < 5.0F * DTR)
            {
                trackX = 2.0F * FCC.groundDesignateX - FCC.groundImpactX;
                trackY = 2.0F * FCC.groundDesignateY - FCC.groundImpactY;
            }

            if (agApproach == AGA_BOMBER)
            {
                if (!droppingBombs)
                {
                    // Try to put the middle drop on target
                    Sms.SetRippleCount((int)(Sms.NumCurrentWpn() / 2.0F + 0.5F) - 1);

                    int rcount = Sms.RippleCount() + 1;
                    if (!(rcount & 1)) // If not odd
                        rcount--;

                    //if (FCC.airGroundRange < Sms.NumCurrentWpn() * 2.0F * Sms.RippleInterval())
                    if (FCC.airGroundRange < (rcount * Sms.RippleInterval()) / 2) // JB 010408 010708 Space the ripples correctly over the target
                    {
                        droppingBombs = wcBombWpn;
                        FCC.SetBombReleaseOverride(true);
                        onStation = Final1;
                    }
                }
                else
                {

                    if (Sms.NumCurrentWpn() == 0)
                    {
                        FCC.SetBombReleaseOverride(false);
                        // Out of this weapon, find another/get out of dodge
                        agDoctrine = AGD_LOOK_SHOOT_LOOK;
                        hasRocket = false;
                        hasGun = false;
                        hasBomb = false;
                        hasGBU = false;

                        // Start over again
                        madeAGPass = true;
                        onStation = NotThereYet;
                    }
                    // too close ?
                    // we're within a certain range and our ATA is not good
                    else if (approxRange < 1.2f * Phyconst.NM_TO_FT && ata > 75.0f * DTR)
                    {
                        waitingForShot = SimLibElapsedTime + 5000;
                        onStation = Final1;
                    }
                }

            }
            else
            {
                // JB 010708 start Drop all your dumb bombs of the current type or if 
                // the lead is a player (not in autopilot) use the lead's ripple setting.
                if (!droppingBombs)
                {
                    if (flightLead && ((AircraftClass*)flightLead).AutopilotType() != AircraftClass.CombatAP && ((AircraftClass*)flightLead).Sms)
                        Sms.SetRippleCount(min(((AircraftClass*)flightLead).Sms.RippleCount(), (int)(Sms.NumCurrentWpn() / 2.0F + 0.5F) - 1));
                    else
                        Sms.SetRippleCount((int)(Sms.NumCurrentWpn() / 2.0F + 0.5F) - 1);

                    int rcount = Sms.RippleCount() + 1;
                    if (!(rcount & 1)) // If not odd
                        rcount--;

                    if (Sms.RippleCount() > 0 && FCC.airGroundRange < (rcount * Sms.RippleInterval()) / 2)
                    {
                        droppingBombs = wcBombWpn;
                        FCC.SetBombReleaseOverride(true);
                        onStation = Final1;
                    }

                    // 2001-10-24 ADDED BY M.N. Planes can start to circle around their target if we don't do
                    // a range & ata check to the target here.

                    if (approxRange < 1.2f * Phyconst.NM_TO_FT && ata > 75.0f * DTR)
                    {
                        // Bail and try again
                        dx = (self.XPos() - trackX);
                        dy = (self.YPos() - trackY);
                        approxRange = (float)sqrt(dx * dx + dy * dy);
                        dx /= approxRange;
                        dy /= approxRange;
                        ipX = trackX + dy * 5.5f * Phyconst.NM_TO_FT;
                        ipY = trackY - dx * 5.5f * Phyconst.NM_TO_FT;
                        Debug.Assert(ipX > 0.0F);

                        // Try bombing run again
                        onStation = Crosswind;
                    }
                    // End of added section
                }
                // JB 010708 end
                else
                {
                    // if ( FCC.postDrop)
                    if (FCC.postDrop && Sms.CurRippleCount() == 0) // JB 010708
                    {
                        FCC.SetBombReleaseOverride(false); // JB 010708
                        droppingBombs = wcBombWpn;

                        // Out of this weapon, find another/get out of dodge
                        agDoctrine = AGD_LOOK_SHOOT_LOOK;
                        hasRocket = false;
                        hasGun = false;
                        hasBomb = false;
                        hasGBU = false;

                        // Start over again
                        madeAGPass = true;
                        onStation = NotThereYet;
                    }
                    // too close ?
                    // we're within a certain range and our ATA is not good
                    if (approxRange < 1.2f * Phyconst.NM_TO_FT && ata > 75.0f * DTR)
                    {
                        waitingForShot = SimLibElapsedTime + 5000;
                        onStation = Final1;
                    }
                }
            }
        }

        protected void DropGBU(float approxRange, float ata, RadarClass* theRadar)
        {
            LaserPodClass* targetingPod = null;
            FireControlComputer* FCC = self.FCC;
            SMSClass* Sms = self.Sms;
            float dx, dy, angle;
            mlTrig trig;

            F4Assert(!Sms.curWeapon || Sms.curWeapon.IsBomb());

            // Don't stop in the middle
            droppingBombs = wcGbuWpn;

            // Make sure the FCC is in the right mode/sub mode
            if (FCC.GetMasterMode() != FireControlComputer.AirGroundLaser ||
                 FCC.GetSubMode() != FireControlComputer.SLAVE)
            {
                FCC.SetMasterMode(FireControlComputer.AirGroundLaser);
                FCC.SetSubMode(FireControlComputer.SLAVE);
            }

            if (!Sms.curWeapon || !Sms.curWeapon.IsBomb())
            {
                if (Sms.FindWeaponClass(wcGbuWpn, false))
                {
                    Sms.SetWeaponType(Sms.hardPoint[Sms.CurHardpoint()].GetWeaponType());
                }
                else
                {
                    Sms.SetWeaponType(wtNone);
                }
            }


            // Get the targeting pod locked on to the target
            targetingPod = (LaserPodClass*)FindLaserPod(self);
            if (targetingPod && targetingPod.CurrentTarget())
            {
                if (!targetingPod.IsLocked())
                {
                    // Designate needs to go down then up then down to make it work
                    if (FCC.designateCmd)
                        FCC.designateCmd = false;
                    else
                        FCC.designateCmd = true;
                }
                else
                {
                    FCC.designateCmd = false;
                }

                FCC.preDesignate = false;
            }

            // Point the radar at the target
            if (theRadar)
            {
                if (groundTargetPtr && groundTargetPtr.BaseData().IsMover())
                    theRadar.SetMode(RadarClass.GMT);
                else
                    theRadar.SetMode(RadarClass.GM);
                theRadar.SetDesiredTarget(groundTargetPtr);
                theRadar.SetAGSnowPlow(true);
            }

            // Mode the SMS
            Sms.SetPair(false);
            Sms.SetRippleCount(0);

            // Adjust for wind/etc
            if (fabs(FCC.airGroundBearing) < 10.0F * DTR)
            {
                trackX = 2.0F * FCC.groundDesignateX - FCC.groundImpactX;
                trackY = 2.0F * FCC.groundDesignateY - FCC.groundImpactY;
            }

            // Give the FCC permission to release if in parameters
            if (onStation == Final)
            {
                if (SimLibElapsedTime > waitingForShot)
                {
                    // 2001-08-31 REMOVED BY S.G. NOT USED ANYWAY AND I NEED THE FLAG FOR SOMETHING ELSE
                    //			if (approxRange < 1.2F * FCC.airGroundRange)
                    //				SetATCFlag(InhibitDefensive);

                    // Check for too close
                    if (approxRange < 0.5F * FCC.airGroundRange)
                    {
                        // Bail and try again
                        dx = (self.XPos() - trackX);
                        dy = (self.YPos() - trackY);
                        approxRange = (float)sqrt(dx * dx + dy * dy);
                        dx /= approxRange;
                        dy /= approxRange;
                        ipX = trackX + dy * 7.5f * Phyconst.NM_TO_FT;
                        ipY = trackY - dx * 7.5f * Phyconst.NM_TO_FT;
                        Debug.Assert(ipX > 0.0F);

                        // Start again
                        onStation = Crosswind;
                        //MonoPrint ("Too close to GBU, head to IP and try again\n");
                    }

                    SetFlag(MslFireFlag);

                    if (FCC.postDrop)
                    {
                        // 1/2 second between bombs, 10 seconds after last bomb
                        if (Sms.NumCurrentWpn() % 2 != 0)
                        {
                            waitingForShot = SimLibElapsedTime + (SEC_TO_MSEC / 2);
                        }
                        else
                        {
                            // Keep Lasing
                            madeAGPass = true;
                            onStation = Final1;
                            waitingForShot = SimLibElapsedTime;
                        }
                    }
                }
                else // Are we too close?
                {
                    if (onStation == Final && approxRange < 0.7F * FCC.airGroundRange || ata > 60.0F * DTR)
                    {
                        // Bail and try again
                        dx = (self.XPos() - trackX);
                        dy = (self.YPos() - trackY);
                        approxRange = (float)sqrt(dx * dx + dy * dy);
                        dx /= approxRange;
                        dy /= approxRange;
                        ipX = trackX + dy * 7.5f * Phyconst.NM_TO_FT;
                        ipY = trackY - dx * 7.5f * Phyconst.NM_TO_FT;
                        Debug.Assert(ipX > 0.0F);

                        // Start again
                        onStation = Crosswind;
                        //MonoPrint ("Too close to bomb, head to IP and try again\n");
                    }
                }
            }

            // Out of this weapon, find another/get out of dodge
            if (onStation == Final1)
            {
                if (SimLibElapsedTime > waitingForShot) // Bomb has had time to fall.
                {
                    agDoctrine = AGD_LOOK_SHOOT_LOOK;
                    hasRocket = false;
                    hasGun = false;
                    hasBomb = false;
                    hasGBU = false;
                    droppingBombs = false;

                    // Force a weapon/target selection
                    madeAGPass = true;
                    onStation = NotThereYet;
                    moreFlags &= ~KeepLasing; // 2002-03-08 ADDED BY S.G. Not lasing anymore
                }
                else if (SimLibElapsedTime == waitingForShot) // Turn but keep designating
                {
                    dx = (trackX - self.XPos());
                    dy = (trackY - self.YPos());
                    angle = 45.0F * DTR + (float)atan2(dy, dx);
                    mlSinCos(&trig, angle);
                    // 2001-07-10 MODIFIED BY S.G. SINCE WE DROP FROM HIGHER, NEED TO DESIGNATE LONGER
                    //			ipX = trackX = trackX + trig.cos * 6.5f * Phyconst.NM_TO_FT;
                    //			ipY = trackY = trackY + trig.sin * 6.5f * Phyconst.NM_TO_FT;
                    ipX = trackX = trackX + trig.cos * 7.5f * Phyconst.NM_TO_FT;
                    ipY = trackY = trackY + trig.sin * 7.5f * Phyconst.NM_TO_FT;
                    ipZ = self.ZPos();
                    //			waitingForShot = SimLibElapsedTime + 20 * SEC_TO_MSEC;
                    waitingForShot = SimLibElapsedTime + 27 * SEC_TO_MSEC;
                    Debug.Assert(ipX > 0.0F);
                    moreFlags |= KeepLasing; // 2002-03-08 ADDED BY S.G. Flag this AI as lasing so he sticks to it
                }
                else
                {
                    trackX = ipX;
                    trackY = ipY;
                    trackZ = ipZ;
                }
            }
            //MI null out our Target
            if (groundTargetPtr &&
                groundTargetPtr.BaseData().IsSim() &&
                (groundTargetPtr.BaseData().IsDead() ||
                groundTargetPtr.BaseData().IsExploding()))
            {
                SetGroundTarget(null);
            }

        }

        protected void FireAGMissile(float approxRange, float ata)
        {
            SMSClass* Sms = self.Sms;

            F4Assert(!Sms.curWeapon || Sms.curWeapon.IsMissile());

            // Check Timer
            // 2001-07-12 MODIFIED BY S.G. DON'T LAUNCH UNTIL CLOSE TO OUR ATTACK ALTITUDE
            //	if ( SimLibElapsedTime >= waitingForShot )
            if (SimLibElapsedTime >= waitingForShot && self.ZPos() - trackZ >= -500.0f)
            {
                SetFlag(MslFireFlag);

                // if we're out of missiles and bombs and our
                // doctrine is look shoot look, we don't want to
                // continue with guns/rockets only -- reset
                // agDoctrine if this is the case
                // 2001-05-03 MODIFIED BY S.G. WE STOP FIRING WHEN WE HAVE AN ODD NUMBER OF MISSILE LEFT (MEANT WE FIRED ONE ALREADY) THIS WILL LIMIT IT TO 2 MISSILES PER TARGET
                //		if (Sms.NumCurrentWpn() == 1 )
                if (Sms.NumCurrentWpn() & 1)
                {
                    hasRocket = false;
                    hasGun = false;
                    hasHARM = false;
                    hasAGMissile = false;

                    // Force a weapon/target selection
                    madeAGPass = true;
                    onStation = NotThereYet;

                    // 2001-06-01 ADDED BY S.G. THAT WAY, AI WILL KEEP GOING STRAIGHT FOR A SECOND BEFORE PULLING
                    missileShotTimer = SimLibElapsedTime + 1000;
                    // END OF ADDED SECTION
                    // 2001-06-16 ADDED BY S.G. THAT WAY, AI WILL NOT RETURN TO THEIR IP ONCE THEY FIRED BUT WILL GO "PERPENDICULAR FOR 2 NM"...
                    // 2001-07-16 MODIFIED BY S.G. DEPENDING IF WE HAVE WEAPONS LEFT, PULL MORE OR LESS
                    if (groundTargetPtr)
                    {
                        float dx = groundTargetPtr.BaseData().XPos() - self.XPos();
                        float dy = groundTargetPtr.BaseData().YPos() - self.YPos();

                        // x-y get range
                        float range = (float)sqrt(dx * dx + dy * dy) + 0.1f;

                        // normalize the x and y vector
                        dx /= range;
                        dy /= range;

                        if (Sms.NumCurrentWpn() == 1)
                        {
                            ipX = self.XPos() + dy * 5.0f * Phyconst.NM_TO_FT - dx * 1.5f * Phyconst.NM_TO_FT;
                            ipY = self.YPos() - dx * 5.0f * Phyconst.NM_TO_FT - dy * 1.5f * Phyconst.NM_TO_FT;
                        }
                        else
                        {
                            ipX = self.XPos() + dy * 3.0f * Phyconst.NM_TO_FT - dx * 1.5f * Phyconst.NM_TO_FT;
                            ipY = self.YPos() - dx * 3.0f * Phyconst.NM_TO_FT - dy * 1.5f * Phyconst.NM_TO_FT;
                        }
                    }
                    // END OF ADDED SECTION
                }

                // determine if we shoot and run or not
                waitingForShot = SimLibElapsedTime + 5000;
            }
            // too close or already fired, try again
            // we're within a certain range and our ATA is not good
            else if (approxRange < 1.2f * Phyconst.NM_TO_FT && ata > 75.0f * DTR)
            {
                waitingForShot = SimLibElapsedTime + 5000;
                onStation = Final1;
            }
        }

        protected void FireRocket(float approxRange, float ata)
        {
            FireControlComputer* FCC = self.FCC;
            SMSClass* Sms = self.Sms;

            if (FCC.GetMasterMode() != FireControlComputer.AirGroundBomb ||
                FCC.GetSubMode() != FireControlComputer.RCKT)
            {
                FCC.SetMasterMode(FireControlComputer.AirGroundBomb);
                FCC.SetSubMode(FireControlComputer.RCKT);
            }

            // JB 000102 from Marco
            // the following lines are commented out -- AI will now fire rockets
            //if (Sms.NumCurrentWpn() <= 0)
            //{
            //   hasRocket = false;
            //   madeAGPass = true;
            //  onStation = NotThereYet;
            //}
            // JB 000102 from Marco

            if (ata < 5.0f * DTR && approxRange < 1.8f * Phyconst.NM_TO_FT)
            {
                SetFlag(MslFireFlag);
            }
            else if (approxRange < 1.2f * Phyconst.NM_TO_FT && ata > 75.0f * DTR)
            {
                waitingForShot = SimLibElapsedTime + 5000;
                onStation = Final1;
            }
        }
        protected void GunStrafe(float approxRange, float ata)
        {
            FireControlComputer* FCC = self.FCC;

            if (FCC.GetMasterMode() != FireControlComputer.Gun ||
                FCC.GetSubMode() != FireControlComputer.STRAF)
            {
                FCC.SetMasterMode(FireControlComputer.Gun);
                FCC.SetSubMode(FireControlComputer.STRAF);
            }

            if (ata < 5.0f * DTR && approxRange < 1.2f * Phyconst.NM_TO_FT)
            {
                SetFlag(GunFireFlag);
            }
            else if (approxRange < 1.2f * Phyconst.NM_TO_FT && ata > 75.0f * DTR)
            {
                waitingForShot = SimLibElapsedTime + 5000;
                onStation = Final1;
            }
        }

        protected int MaverickSetup(float rx, float ry, float ata, float approxRange, RadarClass* theRadar)
        {
            FireControlComputer* FCC = self.FCC;
            SMSClass* Sms = self.Sms;
            float dx, dy, az, rMin, rMax;
            int retval = true;

            if (FCC.postDrop)
            {
                // Force a retarget
                if (groundTargetPtr && groundTargetPtr.BaseData().IsSim())
                {
                    SetGroundTarget(((SimBaseClass*)groundTargetPtr.BaseData()).GetCampaignObject());
                }
                SelectGroundTarget(TARGET_ANYTHING);
                retval = false;
            }
            else
            {
                // Point the radar at the target
                if (theRadar)
                {
                    // 2001-07-23 MODIFIED BY S.G. MOVERS ARE ONLY 3D ENTITIES WHILE BATTALIONS WILL INCLUDE 2D AND 3D VEHICLES...
                    //       if (groundTargetPtr && groundTargetPtr.BaseData().IsMover())
                    if (groundTargetPtr && (groundTargetPtr.BaseData().IsSim() && ((SimBaseClass*)groundTargetPtr.BaseData()).GetCampaignObject().IsBattalion()) || (groundTargetPtr.BaseData().IsCampaign() && groundTargetPtr.BaseData().IsBattalion()))
                        theRadar.SetMode(RadarClass.GMT);
                    else
                        theRadar.SetMode(RadarClass.GM);
                    theRadar.SetDesiredTarget(groundTargetPtr);
                    theRadar.SetAGSnowPlow(true);
                }

                F4Assert(!Sms.curWeapon || Sms.curWeapon.IsMissile());


                // Set up FCC for maverick shot
                if (FCC.GetMasterMode() != FireControlComputer.AirGroundMissile)
                {
                    FCC.SetMasterMode(FireControlComputer.AirGroundMissile);
                    FCC.SetSubMode(FireControlComputer.SLAVE);
                    FCC.designateCmd = false;
                    FCC.preDesignate = true;
                }
                // 2001-07-23 REMOVED BY S.G. DO LIKE THE HARMSetup. ONCE SET, DO YOUR STUFF
                //    else
                {
                    // Has the current weapon locked?
                    if (Sms.curWeapon)
                    {
                        if (Sms.curWeapon.targetPtr)
                        {
                            if (self.curSOI != SimVehicleClass.SOI_WEAPON)
                            {
                                FCC.designateCmd = true;
                            }
                            else
                            {
                                FCC.designateCmd = false;
                            }

                            FCC.preDesignate = false;
                        }
                        // 2001-07-23 ADDED BY S.G. DON'T LAUNCH IF OUR MISSILE DO NOT HAVE A LOCK!
                        else
                            retval = false;

                        // fcc target needs to be set cuz that's the target
                        // that will be used in sms launch missile
                        FCC.SetTarget(groundTargetPtr);

                        az = (float)atan2(ry, rx);
                        Debug.Assert(Sms.curWeapon.IsMissile());
                        rMax = ((MissileClass*)Sms.curWeapon).GetRMax(-self.ZPos(), self.Vt(), az, 0.0f, 0.0f);

                        // 2001-08-31 REMOVED BY S.G. NOT USED ANYWAY AND I NEED THE FLAG FOR SOMETHING ELSE
                        //				if (approxRange < rMax)
                        //					SetATCFlag(InhibitDefensive);

                        // rmin is just a percent of rmax
                        rMin = rMax * 0.1f;
                        // get the sweet spot
                        rMax *= 0.8f;

                        // Check for firing solution
                        if (!(ata < 15.0f * DTR && approxRange > rMin && approxRange < rMax))
                        {
                            retval = false;
                        }
                        // 2001-07-23 ADDED BY S.G. MAKE SURE WE CAN SEE THE TARGET
                        else if (Sms.curWeapon)
                        {
                            // First make sure the relative geometry is valid
                            if (Sms.curWeapon.targetPtr)
                            {
                                CalcRelGeom(self, Sms.curWeapon.targetPtr, null, 1.0F / SimLibMajorFrameTime);
                                // Then run the seeker if we already have a target
                                ((MissileClass*)Sms.curWeapon).RunSeeker();
                            }

                            // If we have no target, don't shoot!
                            if (!Sms.curWeapon || !Sms.curWeapon.targetPtr)
                                retval = false;
                        }
                        // END OF ADDED SECTION

                        // Check for Min Range
                        if (approxRange < 1.1F * rMin || ata > 165.0F * DTR)
                        {
                            // Bail and try again
                            dx = (self.XPos() - trackX);

                            dy = (self.YPos() - trackY);
                            approxRange = (float)sqrt(dx * dx + dy * dy);
                            dx /= approxRange;
                            dy /= approxRange;
                            ipX = trackX + dy * 7.5f * Phyconst.NM_TO_FT;
                            ipY = trackY - dx * 7.5f * Phyconst.NM_TO_FT;
                            Debug.Assert(ipX > 0.0F);

                            // Start again
                            onStation = Crosswind;
                            //MonoPrint ("Too close to Maverick, head to IP and try again\n");
                            retval = false;
                        }
                    }
                    else
                    {
                        retval = false;
                    }
                }
            }

            return retval;
        }

        protected int HARMSetup(float rx, float ry, float ata, float approxRange)
        {
            FireControlComputer* FCC = self.FCC;
            SMSClass* Sms = self.Sms;
            float dx, dy, az, rMin, rMax;
            int retval = true;
            HarmTargetingPod* theHTS;

            theHTS = (HarmTargetingPod*)FindSensor(self, SensorClass.HTS);

            F4Assert(!Sms.curWeapon || Sms.curWeapon.IsMissile());
            // Set up FCC for harm shot
            if (FCC.GetMasterMode() != FireControlComputer.AirGroundHARM)
            {
                FCC.SetMasterMode(FireControlComputer.AirGroundHARM);
                FCC.SetSubMode(FireControlComputer.HTS);
            }

            // fcc target needs to be set cuz that's the target
            // that will be used in sms launch missile
            FCC.SetTarget(groundTargetPtr);

            az = (float)atan2(ry, rx);

            // Got this null in multiplayer - RH
            Debug.Assert(Sms.curWeapon);

            if (Sms.curWeapon)
            {
                Debug.Assert(Sms.curWeapon.IsMissile());
                rMax = ((MissileClass*)Sms.curWeapon).GetRMax(-self.ZPos(), self.Vt(), az, 0.0f, 0.0f);
            }
            else
            {
                rMax = 0.1F;
            }

            // 2002-01-21 ADDED BY S.G. GetRMax is not enough, need to see if the HARM seeker will see the target as well
            //                          Adjust rMax accordingly.
            int radarRange;
            if (groundTargetPtr.BaseData().IsSim())
                radarRange = ((SimBaseClass*)groundTargetPtr.BaseData()).RdrRng();
            else
            {
                if (groundTargetPtr.BaseData().IsEmitting())
                    radarRange = RadarDataTable[groundTargetPtr.BaseData().GetRadarType()].NominalRange;
                else
                    radarRange = 0.0f;
            }
            rMax = min(rMax, radarRange);
            rMax = max(rMax, 0.1f);
            // END OF ADDED SECTION 2002-01-21

            // 2001-08-31 REMOVED BY S.G. NOT USED ANYWAY AND I NEED THE FLAG FOR SOMETHING ELSE
            //	if (approxRange < rMax)
            //		SetATCFlag(InhibitDefensive);

            // rmin is just a percent of rmax
            rMin = rMax * 0.1f;
            // get the sweet spot
            rMax *= 0.8f;

            // Make sure the HTS has the target
            if (theHTS)
            {
                theHTS.SetDesiredTarget(groundTargetPtr);
                if (!theHTS.CurrentTarget())
                    retval = false;

                // JB 020121 Make sure the HTS can detect the target otherwise HARMs will immediately fail to guide.
                // 2002-01-21 REMOVED BY S.G. Done differently above, CanDetectObject calls CheckLOS which I'm doing below as well.
                /*if (!theHTS.CanDetectObject(groundTargetPtr))
                   retval = false; */

                // 2001-06-18 ADDED BY S.G. JUST DO A LOS CHECK :-( I CAN'T RELIABLY GET THE POD LOCK STATUS
                if (!self.CheckLOS(groundTargetPtr))
                    retval = false;
                // END OF ADDED SECTION
            }

            // Check for firing conditions
            // 2001-07-05 MODIFIED BY S.G. HARMS CAN BE FIRED BACKWARD BUT LETS LIMIT AI TO A MORE REALISTIC LAUNCH PARAMETER
            // if ( !( ata < 15.0f * DTR && approxRange > rMin && approxRange < rMax ) )
            if (!(ata < 60.0f * DTR && approxRange > rMin && approxRange < rMax))
            {
                retval = false;
            }

            // Check for Min Range
            // 2001-07-05 MODIFIED BY S.G. USE ONLY rMin AND ata DOESN'T MATTER
            // if (approxRange < 1.1F * rMin || ata > 135.0F * DTR)
            if (approxRange < rMin)
            {
                // Bail and try again
                dx = (self.XPos() - trackX);
                dy = (self.YPos() - trackY);
                approxRange = (float)sqrt(dx * dx + dy * dy);
                dx /= approxRange;
                dy /= approxRange;
                // 2001-07-05 MODIFIED BY S.G. USE A FRACTION OF rMax INSTEAD OF A FIX VALUE
                //	   ipX = trackX + dy * 15.0f * Phyconst.NM_TO_FT;
                //	   ipY = trackY - dx * 15.0f * Phyconst.NM_TO_FT;
                ipX = trackX + dy * rMax * 0.5f;
                ipY = trackY - dx * rMax * 0.5f;
                Debug.Assert(ipX > 0.0F);

                // Start again
                onStation = Crosswind;
                //MonoPrint ("Too close to HARM, head to IP and try again\n");
            }

            // we want to see what the target campaign
            // entity is doing
            if (groundTargetPtr.BaseData().IsSim())
            {
                // 2001-06-25 ADDED BY S.G. IF I HAVE SOMETHING IN shotAt, IT COULD MEAN SOMEONE SHOT WHILE THE TARGET WAS AGGREGATED. DEAL WITH THIS
                // If shotAt has something, someone is/was targeting the aggregated entity. If it wasn't me, don't fire at it once it is deaggregated as well.
                if (((FlightClass*)self.GetCampaignObject()).shotAt == ((SimBaseClass*)groundTargetPtr.BaseData()).GetCampaignObject() && ((FlightClass*)self.GetCampaignObject()).whoShot != self)
                    retval = false - 1;
                // END OF ADDED SECTION
                // 2001-05-27 MODIFIED BY S.G. LAUNCH AT A CLOSER RANGE IF NOT EMITTING (AND IT'S THE ONLY WEAPONS ON BOARD - TESTED SOMEWHERE ELSE)
                //		if ( !((SimBaseClass *)groundTargetPtr.BaseData()).GetCampaignObject().IsEmitting() && approxRange > 0.5F * rMax)
                //			retval = false;
                // 2002-01-20 MODIFIED BY S.G. If RdrRng() is zero, this means the radar is off. Can't fire at it if it's off! (only valid for sim object)
                //		if ( !((SimBaseClass *)groundTargetPtr.BaseData()).GetCampaignObject().IsEmitting())
                if (!((SimBaseClass*)groundTargetPtr.BaseData()).GetCampaignObject().IsEmitting() || ((SimBaseClass*)groundTargetPtr.BaseData()).RdrRng() == 0)
                {
                    retval = false;
                    // 2001-07-02 MODIFIED BY S.G. IT'S NOW 0.25 SO TWICE AS CLOSE AS BEFORE
                    //			if (approxRange < 0.5F * rMax)
                    if (approxRange < 0.25F * rMax)
                    {
                        // 2001-06-24 ADDED BY S.G. TRY WITH SOMETHING ELSE IF YOU CAN
                        if (hasAGMissile | hasBomb | hasRocket | hasGun | hasGBU)
                            retval = false - 1;
                        else
                        {
                            // END OF ADDED SECTION
                            agDoctrine = AGD_NONE;
                            missionComplete = true;
                            self.FCC.SetMasterMode(FireControlComputer.Nav);
                            self.FCC.preDesignate = true;
                            SetGroundTarget(null);
                            SelectNextWaypoint();
                            // if we're a wingie, rejoin the lead
                            if (isWing)
                            {
                                mFormation = FalconWingmanMsg.WMWedge;
                                AiRejoin(null);
                                // make sure wing's designated target is null'd out
                                mDesignatedObject = FalconNullId;
                            }
                            else // So the player's wingmen still know they have something
                                hasWeapons = false; // Got here so nothing else than HARMS was available anyway
                        }
                    }
                }
                // END OF MODIFIED SECTION
            }
            else
            {
                // campaign entity
                // 2001-06-25 ADDED BY S.G. IF IT IS AGGREGATED, ONLY ONE PLANE CAN SHOOT AT IT WITH HARMS
                if (((CampBaseClass*)groundTargetPtr.BaseData()).IsAggregate())
                {
                    // 2002-01-20 ADDED BY S.G. Unless it's an AAA since it has more than one radar.
                    if (!groundTargetPtr.BaseData().IsBattalion() || ((BattalionClass*)groundTargetPtr.BaseData()).class_data.RadarVehicle < 16)
                    {
                        // END OF ADDED SECTION 2002-01-20
                        // If it's not at what we shot last, then it's valid
                        if (((FlightClass*)self.GetCampaignObject()).shotAt != groundTargetPtr.BaseData())
                        {
                            ((FlightClass*)self.GetCampaignObject()).shotAt = groundTargetPtr.BaseData();
                            ((FlightClass*)self.GetCampaignObject()).whoShot = self;
                        }
                        // If one of us is shooting, make sure it's me, otherwise no HARMS for me please.
                        else if (((FlightClass*)self.GetCampaignObject()).whoShot != self)
                            retval = false - 1;
                    }
                }
                // END OF ADDED SECTION
                // 2001-05-27 MODIFIED BY S.G. LAUNCH AT A CLOSER RANGE IF NOT EMITTING (AND IT'S THE ONLY WEAPONS ON BOARD - TESTED SOMEWHERE ELSE)
                //		if ( !groundTargetPtr.BaseData().IsEmitting() && approxRange > 0.5F * rMax)
                //			retval = false;
                // 2001-06-05 MODIFIED BY S.G. THAT'S IT IF YOU CAN CONNECT WITH IT...
                // 2001-06-21 MODIFIED BY S.G. EVEN IF EMITTING, IF IT'S NOT AGGREGATED, DON'T FIRE (IE retval = false)
                if (!groundTargetPtr.BaseData().IsEmitting() || !((CampBaseClass*)groundTargetPtr.BaseData()).IsAggregate())
                {
                    retval = false;
                    // 2001-07-02 MODIFIED BY S.G. IT'S NOW 0.25 SO TWICE AS CLOSE AS BEFORE
                    //			if (approxRange < 0.5F * rMax) {
                    if (approxRange < 0.25F * rMax)
                    {
                        // 2001-06-24 ADDED BY S.G. TRY WITH SOMETHING ELSE IF YOU CAN
                        if (hasAGMissile | hasBomb | hasRocket | hasGun | hasGBU)
                            retval = false - 1;
                        else
                        {
                            // END OF ADDED SECTION
                            agDoctrine = AGD_NONE;
                            missionComplete = true;
                            self.FCC.SetMasterMode(FireControlComputer.Nav);
                            self.FCC.preDesignate = true;
                            SetGroundTarget(null);
                            SelectNextWaypoint();
                            // if we're a wingie, rejoin the lead
                            if (isWing)
                            {
                                mFormation = FalconWingmanMsg.WMWedge;
                                AiRejoin(null);
                                // make sure wing's designated target is null'd out
                                mDesignatedObject = FalconNullId;
                            }
                            else // So the player's wingmen still know they have something
                                hasWeapons = false; // Got here so nothing else than HARMS was available anyway
                        }
                    }
                }
                // END OF MODIFIED SECTION
            }

            // if we use missiles we don't drop bombs
            // unless we shot a harm
            if (agDoctrine != AGD_LOOK_SHOOT_LOOK)
            {
                hasBomb = false;
                hasGBU = false;
                hasRocket = false;
            }

            return retval;
        }

        protected float rangeToIP;

        protected enum AG_TARGET_TYPE { TARGET_ANYTHING, TARGET_FEATURE, TARGET_UNIT };

        protected void SelectGroundWeapon()
        {
            int i;
            Falcon4EntityClassType* classPtr;
            int runAway = false;
            SMSClass* Sms = self.Sms;

            hasAGMissile = false;
            hasBomb = false;
            hasHARM = false;
            hasGun = false;
            hasCamera = false;
            hasRocket = false;
            hasGBU = false;

            // always make sure the FCC is in a weapons neutral mode when a
            // weapon selection is made.  Potentially we may be out of missiles
            // and have a SMS current bomb selected by this function and be in
            // FCC air-air mode which will cause a crash.
            self.FCC.SetMasterMode(FireControlComputer.Nav);
            self.FCC.preDesignate = true;

            // Set no AG weapon, set true if found
            ClearATCFlag(HasAGWeapon);

            // look for a bomb and/or a missile
            for (i = 0; i < self.Sms.NumHardpoints(); i++)
            {
                // Check for AG Missiles
                if (!hasAGMissile && Sms.hardPoint[i].weaponPointer && Sms.hardPoint[i].GetWeaponClass() == wcAgmWpn)
                {
                    hasAGMissile = true;
                }
                else if (!hasHARM && Sms.hardPoint[i].weaponPointer && Sms.hardPoint[i].GetWeaponClass() == wcHARMWpn)
                {
                    hasHARM = true;
                }
                else if (!hasBomb && Sms.hardPoint[i].weaponPointer && Sms.hardPoint[i].GetWeaponClass() == wcBombWpn)
                {
                    // 2001-07-11 ADDED BY S.G. SO DURANDAL AND CLUSTER ARE ACCOUNTED FOR DIFFERENTLY THAN NORMAL BOMBS
                    //		 hasBomb = true;

                    hasBomb = true;
                    if (Sms.hardPoint[i].GetWeaponData().cd >= 0.9f) // S.G. used edg kludge: drag coeff >= 1.0 is a durandal (w/chute) BUT 0.9 is hardcode for high drag :-(
                        hasBomb = true + 1;
                    else if (Sms.hardPoint[i].GetWeaponData().flags & SMSClass.HasBurstHeight) // S.G. If it has burst height, it's a cluster bomb
                        hasBomb = true + 2;
                    // END OF MODIFIED SECTION (except for the indent of the next line)
                }
                else if (!hasGBU && Sms.hardPoint[i].weaponPointer && Sms.hardPoint[i].GetWeaponClass() == wcGbuWpn)
                {
                    hasGBU = true;
                }
                else if (!hasRocket && Sms.hardPoint[i].weaponPointer && Sms.hardPoint[i].GetWeaponClass() == wcRocketWpn)
                {
                    hasRocket = true;
                }
                else if (!hasCamera && Sms.hardPoint[i].weaponPointer && Sms.hardPoint[i].GetWeaponClass() == wcCamera)
                {
                    hasCamera = true;
                }
            }


            // finally look for guns
            // only the A-10 and SU-25 are guns-capable for A-G
            classPtr = (Falcon4EntityClassType*)self.EntityType();
            if (classPtr.vuClassData.classInfo_[VU_STYPE] == STYPE_AIR_ATTACK &&
                (classPtr.vuClassData.classInfo_[VU_SPTYPE] == SPTYPE_A10 ||
                classPtr.vuClassData.classInfo_[VU_SPTYPE] == SPTYPE_SU25))
            {
                if (self.Guns &&
                     self.Guns.numRoundsRemaining)
                {
                    hasGun = true;
                }
            }

            if (hasAGMissile | hasBomb | hasHARM | hasRocket | hasGBU)
            {
                SetATCFlag(HasAGWeapon);
                // 2001-05-27 ADDED BY S.G. IF WE HAVE HARMS AND OUR TARGET IS NOT EMITTING, CLEAR hasHARM ONLY IF WE HAVE SOMETHING ELSE ON BOARD
                // 2001-06-20 MODIFIED BY S.G. EVEN IF ONLY HAVE HARMS, DO THIS. HOPEFULLY WING WILL REJOIN AND LEAD WILL TERMINATE IF ON SEAD STRIKES
                //	  if (hasHARM && groundTargetPtr && (hasAGMissile | hasBomb | hasRocket | hasGBU)) {
                if (hasHARM && groundTargetPtr)
                {
                    // If it's a sim entity, look at its radar type)
                    if (groundTargetPtr.BaseData().IsSim())
                    {
                        // 2001-06-20 MODIFIED BY S.G. IT DOESN'T MATTER AT THIS POINT IF IT'S EMITTING OR NOT. ALL THAT MATTERS IS - IS IT A RADAR VEHICLE/FEATURE FOR A SIM OR DOES IT HAVE A RADAR VEHICLE IF A CAMPAIGN ENTITY
                        // No need to check if they exists because if they got selected, it's because they exists. Only check if they have a radar
                        if (!((groundTargetPtr.BaseData().IsVehicle() && ((SimVehicleClass*)groundTargetPtr.BaseData()).GetRadarType() != RDR_NO_RADAR) ||	// It's a vehicle and it has a radar
                            (groundTargetPtr.BaseData().IsStatic() && ((SimStaticClass*)groundTargetPtr.BaseData()).GetRadarType() != RDR_NO_RADAR)))	// It's a feature and it has a radar
                            hasHARM = false;
                    }
                    // NOPE - It's a campaign object, if it's aggregated, can't use HARM unless no one has chosen it yet.
                    else if (((CampBaseClass*)groundTargetPtr.BaseData()).IsAggregate())
                    {
                        // 2002-01-20 ADDED BY S.G. Unless it's an AAA since it has more than one radar.
                        if (!groundTargetPtr.BaseData().IsBattalion() || ((BattalionClass*)groundTargetPtr.BaseData()).class_data.RadarVehicle < 16)
                        {
                            // END OF ADDED SECTION 2002-01-20
                            if (((FlightClass*)self.GetCampaignObject()).shotAt != groundTargetPtr.BaseData())
                            {
                                ((FlightClass*)self.GetCampaignObject()).shotAt = groundTargetPtr.BaseData();
                                ((FlightClass*)self.GetCampaignObject()).whoShot = self;
                            }
                            else if (((FlightClass*)self.GetCampaignObject()).whoShot != self)
                                hasHARM = false;
                        }
                    }
                }
                // END OF ADDED SECTION
            }

            // 2001-06-30 ADDED BY S.G. SO THE true WEAPON STATE IS KEPT...
            if (hasAGMissile | hasBomb | hasHARM | hasRocket | hasGBU)
                SetATCFlag(HasCanUseAGWeapon);
            else
                ClearATCFlag(HasCanUseAGWeapon);
            // END OF ADDED SECTION

            hasWeapons = hasAGMissile | hasBomb | hasHARM | hasRocket | hasGun | hasGBU;

            // 2001-06-20 ADDED BY S.G. LEAD WILL TAKE ITS WING WEAPON LOADOUT INTO CONSIDERATION BEFORE ABORTING
            // 2001-06-30 MODIFIED BY S.G. IF NOT A ENROUTE SEAD TARGET, SKIP HARMS AS AVAILABLE IF IT CAN'T BE FIRED
            int ret;
            if (!hasWeapons && !isWing && ((ret = IsNotMainTargetSEAD()) || sentWingAGAttack != AG_ORDER_ATTACK))
            {
                int i;
                int usComponents = self.GetCampaignObject().NumberOfComponents();
                for (i = 0; i < usComponents; i++)
                {
                    AircraftClass* flightMember = (AircraftClass*)self.GetCampaignObject().GetComponentEntity(i);
                    if (flightMember && flightMember.DBrain() && flightMember.DBrain().IsSetATC(ret ? HasAGWeapon : HasCanUseAGWeapon))
                    {
                        hasWeapons = true;
                        SetATCFlag(HasAGWeapon);
                        break;
                    }
                }
            }
            // END OF ADDED SECTION
            // make sure, if we're guns or rockets only, that are approach is
            // a dive
            if (!hasBomb && !hasGBU && !hasAGMissile && !hasHARM && (hasGun || hasRocket))
            {
                agApproach = AGA_DIVE;
                ipZ = -7000.0f;
            }

            // Check for run-away case
            if (missionType == AMIS_BDA || missionType == AMIS_RECON)
            {
                if (!hasCamera)
                {
                    runAway = true;
                }
            }
            // 2001-06-20 MODIFIED BY S.G. SO AI DO NOT RUN AWAY IF YOU STILL HAVE HARMS AND ON A SEAD TYPE MISSION
            // else if (!hasWeapons)
            else if (!hasWeapons && !(IsSetATC(HasAGWeapon) && IsNotMainTargetSEAD()))
            {
                runAway = true;
            }

            // 2002-03-08 ADDED BY S.G. Don't run away if designating...
            if ((moreFlags & KeepLasing) && runAway == true)
                runAway = false;
            // END OF ADDED SECTION 2002-03-08

            if (runAway && missionClass == AGMission)// Nothing to attack with or Recon/BDA mission w/o camera
            {
                // no AG weapons, next waypoint....
                agDoctrine = AGD_NONE;
                // 2001-08-04 MODIFIED BY S.G. SET missionComplete ONLY ONCE WE TEST IT (ADDED THAT TEST FOR THE IF AS WELL)
                //		missionComplete = true;
                self.FCC.SetMasterMode(FireControlComputer.Nav);
                self.FCC.preDesignate = true;
                SetGroundTarget(null);
                if ( /*S.G.*/!missionComplete && /*S.G.*/agImprovise == false && !self.OnGround())
                {
                    // JB 020315 Only skip to the waypoint after the target waypoint. Otherwise we may go into landing mode too early.
                    WayPointClass* tmpWaypoint = self.curWaypoint;
                    while (tmpWaypoint)
                    {
                        if (tmpWaypoint.GetWPFlags() & WPF_TARGET)
                        {
                            tmpWaypoint = tmpWaypoint.GetNextWP();
                            break;
                        }
                        tmpWaypoint = tmpWaypoint.GetNextWP();
                    }

                    if (tmpWaypoint)
                        SelectNextWaypoint();
                }

                missionComplete = true; /*S.G.*/
                // if we're a wingie, rejoin the lead
                if (isWing)
                {
                    // 2001-05-03 ADDED BY S.G. WE WANT WEDGE AFTER GROUND PASS!
                    mFormation = FalconWingmanMsg.WMWedge;
                    // END OF ADDED SECTION
                    AiRejoin(null);
                    // make sure wing's designated target is null'd out
                    mDesignatedObject = FalconNullId;
                }
            }
        }

        protected int hasBomb;
        protected int hasAGMissile;
        protected int hasHARM;
        protected int hasRocket;
        protected int hasGun;
        protected int hasCamera;
        protected int hasGBU;
        // Marco Edit - check for Clusters/Durandals specifically
        protected int hasCluster;
        protected int hasDurandal;

        protected int hasWeapons;

        protected SimObjectType* groundTargetPtr;
        protected SimObjectType* airtargetPtr;	// for air diverts
        protected int sentWingAGAttack;
        protected bool droppingBombs;
        protected bool agImprovise;
        protected enum AG_ORDERS { AG_ORDER_NONE, AG_ORDER_FORMUP, AG_ORDER_ATTACK };
        protected enum AG_DOCTRINE { AGD_NONE, AGD_SHOOT_RUN, AGD_LOOK_SHOOT_LOOK, AGD_NEED_SETUP };
        protected enum AG_APPROACH { AGA_LOW, AGA_TOSS, AGA_HIGH, AGA_DIVE, AGA_BOMBER };

        protected void SetupAGMode(WayPointClass* cwp, WayPointClass* wp)
        {
            int wpAction;
            UnitClass* campUnit = (UnitClass*)self.GetCampaignObject();
            CampBaseClass* campBaseTarg;
            float dx, dy, dz, range;
            Falcon4EntityClassType* classPtr;
            WayPointClass* dwp;

            // So we don't think our mission is complete and forget to go to CrossWind from 'NotThereYet'
            missionComplete = false;
            agImprovise = false;

            // First, lets get a target if we're the lead, otherwise use the target provided by the lead...

            if (!isWing)
            {
                dwp = null;
                if (campUnit)
                    dwp = ((FlightClass*)campUnit).GetOverrideWP();

                // If we were passed a target wayp
                if (wp)
                {
                    // First have the lead fly toward the IP waypoint until he can see a target
                    if (cwp != wp)
                    {
                        cwp.GetLocation(&ipX, &ipY, &ipZ);

                        // Next waypoint is our target waypoint
                        SelectNextWaypoint();

                        // If we have HARM on board (even if we can't use them), start your attack from here
                        if (hasHARM)
                        {
                            ipX = self.XPos();
                            ipY = self.YPos();
                        }
                    }
                    else
                    {
                        if (dwp)
                            dwp.GetLocation(&ipX, &ipY, &ipZ);
                        else
                            wp.GetLocation(&ipX, &ipY, &ipZ);

                    }

                    wpAction = wp.GetWPAction();
                    // If we have nothing, look at our enroute action
                    if (wpAction == WP_NOTHING)
                        wpAction = wp.GetWPRouteAction();

                    // If it's a SEAD or CASCP waypoint, do the following...
                    if ((wpAction == WP_SEAD || wpAction == WP_CASCP) && cwp == wp)
                    {
                        // But only if it is time to retarget, otherwise stay quiet
                        if (SimLibElapsedTime > nextAGTargetTime)
                        {
                            // Next retarget in 5 seconds
                            nextAGTargetTime = SimLibElapsedTime + 5000;

                            // First, lets release our current target and target history
                            SetGroundTarget(null);
                            gndTargetHistory[0] = null;

                            // The first call should get a campaign entity while the second one will fetch a sim entity within
                            SelectGroundTarget(TARGET_ANYTHING);
                            if (groundTargetPtr == null)
                                return;
                            if (groundTargetPtr.BaseData().IsCampaign() && !((CampBaseClass*)groundTargetPtr.BaseData()).IsAggregate())
                                SelectGroundTarget(TARGET_ANYTHING);

                        }
                        else
                            return;
                    }
                    else
                    {
                        // divert waypoint overrides everything else
                        if (dwp)
                        {
                            campBaseTarg = dwp.GetWPTarget();
                            if (!campBaseTarg)
                            {
                                dwp = null;
                                campBaseTarg = wp.GetWPTarget();
                            }
                        }
                        else
                            campBaseTarg = wp.GetWPTarget();

                        // See if we got a target waypoint target, if not, try and see if we can select one by using the campaign target selector
                        if (campBaseTarg == null)
                        {
                            if (SimLibElapsedTime > nextAGTargetTime)
                            {
                                // Next retarget in 5 seconds
                                nextAGTargetTime = SimLibElapsedTime + 5000;

                                // First, lets release our current target and target history
                                SetGroundTarget(null);
                                gndTargetHistory[0] = null;

                                // The first call should get a campaign entity while the second one will fetch a sim entity within
                                SelectGroundTarget(TARGET_ANYTHING);
                                if (groundTargetPtr == null)
                                    return;
                                if (groundTargetPtr.BaseData().IsCampaign() && !((CampBaseClass*)groundTargetPtr.BaseData()).IsAggregate())
                                    SelectGroundTarget(TARGET_ANYTHING);
                            }
                            else
                                return;
                        }
                        // set ground target to camp base if ground target is null at this point
                        else if (groundTargetPtr == null)
                        {
                            SetGroundTarget(campBaseTarg);

                            if (groundTargetPtr == null)
                                return;
                            // 2001-10-26 ADDED by M.N. If player changed mission type in TE planner, and below the target WP
                            // we find a package object, it will become a ground target. If a package is engaged, CTD.
                            if (groundTargetPtr.BaseData().IsPackage())
                            {
                                SetGroundTarget(null);
                                gndTargetHistory[0] = null;
                                SelectGroundTarget(TARGET_ANYTHING);	// choose something else
                            }
                            // END of added section

                            // Then get a sim entity from it
                            if (groundTargetPtr.BaseData().IsCampaign() && !((CampBaseClass*)groundTargetPtr.BaseData()).IsAggregate())
                                SelectGroundTarget(TARGET_ANYTHING);
                        }
                    }
                }
                // It's not from a waypoint action (could be a target of opportunity, even from A2A mission if it has A2G weapons as well)
                else if (SimLibElapsedTime > nextAGTargetTime)
                {
                    // Next retarget in 5 seconds
                    nextAGTargetTime = SimLibElapsedTime + 5000;

                    // First, lets release our current target and target history
                    SetGroundTarget(null);
                    gndTargetHistory[0] = null;

                    // The first call should get a campaign entity while the second one will fetch a sim entity within
                    SelectGroundTarget(TARGET_ANYTHING);
                    if (groundTargetPtr == null)
                        return;
                    SelectGroundTarget(TARGET_ANYTHING);

                    // Don't ask me, that's how they had it in the orininal code
                    agImprovise = true;
                }
                // Not the time to retarget, so get out
                else
                    return;
            }
            // Don't ask me, that's how they had it in the orininal code
            else
                agImprovise = true;

            // No ground target? do nothing
            if (!groundTargetPtr)
                return;

            // After all this, make sure we have a sim target if we can
            if (groundTargetPtr.BaseData().IsCampaign() && !((CampBaseClass*)groundTargetPtr.BaseData()).IsAggregate())
                SelectGroundTarget(TARGET_ANYTHING);

            // do we have any ground weapons?
            SelectGroundWeapon();
            if (!IsSetATC(HasAGWeapon))
            {
                // Nope, can't really attack can't we? so bail out
                SetGroundTarget(null);
                agDoctrine = AGD_NONE;
                return;
            }

            // Better be safe than sory...
            if (groundTargetPtr == null)
            {
                // Nope, somehow we lost our target so bail out...
                agDoctrine = AGD_NONE;
                return;
            }

            // Tell the AI it hasn't done a ground pass yet so it can redo its attack profile
            madeAGPass = false;

            // set doctrine and approach to default value, calc an insertion point loc
            agDoctrine = AGD_LOOK_SHOOT_LOOK;
            if (missionType == AMIS_SEADESCORT || missionType == AMIS_SEADSTRIKE)
            {
                agApproach = AGA_LOW;
                ipZ = 0.0f;
            }
            else
            {
                agApproach = AGA_DIVE;
                ipZ = -7000.0f;
            }

            dx = groundTargetPtr.BaseData().XPos() - self.XPos();
            dy = groundTargetPtr.BaseData().YPos() - self.YPos();
            dz = groundTargetPtr.BaseData().ZPos() - self.ZPos();

            // x-y get range
            range = (float)sqrt(dx * dx + dy * dy) + 0.1f;

            // normalize the x and y vector
            dx /= range;
            dy /= range;

            // see if we're too close in and set ipX/ipY accordingly
            if (range < 5.0f * Phyconst.NM_TO_FT)
            {
                // too close, get IP at a perpendicular point to current loc
                ipX = groundTargetPtr.BaseData().XPos() + dy * 7.0f * Phyconst.NM_TO_FT;
                ipY = groundTargetPtr.BaseData().YPos() - dx * 7.0f * Phyconst.NM_TO_FT;
                Debug.Assert(ipX > 0.0F);
            }
            else
            {
                // get point between us and target
                ipX = groundTargetPtr.BaseData().XPos() - dx * 4.0f * Phyconst.NM_TO_FT;
                ipY = groundTargetPtr.BaseData().YPos() - dy * 4.0f * Phyconst.NM_TO_FT;
                Debug.Assert(ipX > 0.0F);
            }

            // Depending on the type of plane, adjust our attack profile
            classPtr = (Falcon4EntityClassType*)self.EntityType();
            if (classPtr.vuClassData.classInfo_[VU_STYPE] == STYPE_AIR_BOMBER)
            {
                agApproach = AGA_BOMBER;
                agDoctrine = AGD_SHOOT_RUN;
                ipZ = self.ZPos();
            }
            else
            {
                // Order of fire is: HARMs, AGMissiles, GBUs, bombs, rockets then gun so do it similarely
                if (hasHARM)
                {
                    agApproach = AGA_LOW;
                    agDoctrine = AGD_LOOK_SHOOT_LOOK;
                    ipX = self.XPos();
                    ipY = self.YPos();
                    ipZ = 0.0f;
                }
                else if (hasAGMissile)
                {
                    agApproach = AGA_HIGH;
                    agDoctrine = AGD_LOOK_SHOOT_LOOK;

                    // Wings shoots Mavericks as soon as asked.
                    ipX = self.XPos();
                    ipY = self.YPos();
                    ipZ = -4000.0f;
                }
                else if (hasGBU)
                {
                    agApproach = AGA_HIGH;
                    agDoctrine = AGD_SHOOT_RUN;
                    ipZ = -13000.0f;
                }
                else if (hasBomb)
                {
                    if (hasBomb == true + 1)
                    {		// It's a durandal
                        agApproach = AGA_HIGH;		// Because if 'low', he will 'pop up' on final...
                        ipZ = -250.0f;
                    }
                    else
                    {
                        if (hasBomb == true + 2)
                        {	// It's a cluster bomb
                            agApproach = AGA_HIGH;
                            ipZ = -5000.0f;
                        }
                        else
                        {						// It's any other type of bombs
                            agApproach = AGA_HIGH;
                            ipZ = -11000.0f;
                        }

                        // Now check if we are within the plane's min/max altitude and snap if not...
                        VehicleClassDataType* vc = GetVehicleClassData(self.Type() - VU_LAST_ENTITY_TYPE);
                        ipZ = max(vc.HighAlt * -100.0f, ipZ);
                        ipZ = min(vc.LowAlt * -100.0f, ipZ);
                    }
                    agDoctrine = AGD_SHOOT_RUN;
                }
                else if (hasGun || hasRocket)
                {
                    agDoctrine = AGD_LOOK_SHOOT_LOOK;
                    agApproach = AGA_DIVE;
                    ipZ = -7000.0f;
                }

                // For these, it's always a LOOK SHOOT LOOK
                if (missionType == AMIS_SAD || missionType == AMIS_INT || missionType == AMIS_BAI || hasAGMissile || hasHARM || IsNotMainTargetSEAD())
                    agDoctrine = AGD_LOOK_SHOOT_LOOK;

                // Just in case it was changed by a weapon type earlier
                if (missionType == AMIS_SEADESCORT || missionType == AMIS_SEADSTRIKE)
                {
                    agApproach = AGA_LOW;
                    ipZ = 0.0f;
                }

                // Then if we have a camera, 
                if ((missionType == AMIS_BDA || missionType == AMIS_RECON) && hasCamera)
                {
                    ipZ = -7000.0f;
                    agDoctrine = AGD_SHOOT_RUN;
                    agApproach = AGA_DIVE;
                }
            }

            // Time this attack, and don't stick around too long
            mergeTimer = SimLibElapsedTime + g_nGroundAttackTime * 60 * SEC_TO_MSEC;
        }

        protected AG_DOCTRINE agDoctrine;
        protected AG_APPROACH agApproach;

        protected VU_TIME nextAGTargetTime;
        protected VU_TIME nextAttackCommandToSend; // 2002-01-20 ADDED BY S.G.
        protected bool madeAGPass;

        protected float ipX, ipY, ipZ;				// insertion point loc for AG run


        public enum MissionClassEnum { AGMission, AAMission, SupportMission, AirliftMission }; // 2002-03-05 MODIFIED BY S.G. Added 'Enum' to differentiate it from the getter MissionClass

        protected MissionTypeEnum missionType;
        protected MissionClassEnum missionClass;
        protected bool missionComplete;

        //ATC stuff
        protected VU_TIME createTime;				//when this brain was created in terms of SimLibElapsedTime
        protected uint atcFlags;				//need to know what has happened to them already
        protected int rwIndex;				//0 if none, else index of runway to use
        protected VU_ID airbase;				//airbase to land at
        protected VU_TIME rwtime;					//time scheduled to use runway
        protected VU_TIME updateTime;				//time to update distance info
        protected float distAirbase;			// how far are we from our desired airbase?

        protected AtcStatusEnum atcstatus;				//at what point in the takeoff/landing process are you?
        protected int curTaxiPoint;			//index into PtDataTable, 0 means none
        protected float desiredSpeed;			//speed at which we want to move so we arrive at next point on time
        protected float turnDist;				//when distance to point is less than this we execute a 
        //		turn onto our new heading
        protected VU_TIME waittimer;				//time at which we have been in this state too long

        //Refueling stuff
        protected RefuelStatus refuelstatus;			//current status
        protected VU_ID tankerId;				//which tanker we are using
        protected int tnkposition;			//where are we in the queue to tank, 0 == currently tanking, -1 == don't know yet


        protected void Land()
        {
            SimBaseClass* inTheWay = null;
            ObjectiveClass* Airbase = (ObjectiveClass*)vuDatabase.Find(airbase);
            AircraftClass* leader = null;
            AircraftClass* component = null;
            float cosAngle, heading, deltaTime, testDist;
            ulong min, max;
            float baseX, baseY, finalX, finalY, finalZ, x, y, z, dx, dy, dist, speed, minSpeed, relx, rely, cosHdg, sinHdg;
            mlTrig Trig;

            if (!Airbase)
            {
                //need to find something or we don't know where to go
                if (self.af.GetSimpleMode())
                {
                    SimpleGoToCurrentWaypoint();
                }
                else
                {
                    GoToCurrentWaypoint();
                }
                return;
            }
            else if (!Airbase.IsObjective() || !Airbase.brain)
            {
                if (self.curWaypoint.GetWPAction() == WP_LAND && !self.IsPlayer())
                {
                    dx = self.XPos() - trackX;
                    dy = self.YPos() - trackY;
                    if (dx * dx + dy * dy < 3000.0F * 3000.0F)
                    {
                        //for carriers we just disappear when we get close enough
                        //do carrier landings for F-18
                        RegroupAircraft(self);
                        return;
                    }
                }
                if (self.af.GetSimpleMode())
                {
                    SimpleGoToCurrentWaypoint();
                }
                else
                {
                    GoToCurrentWaypoint();
                }
                return;
            }

            if (self.IsSetFlag(ON_GROUND) && af.Fuel() <= 0.0F && af.vt < 5.0F)
            {
                if (!self.IsPlayer())
                    RegroupAircraft(self); //no gas get him out of the way
            }
            SetDebugLabel(Airbase);

            speed = af.MinVcas() * KNOTS_TO_FTPSEC;

            dx = self.XPos() - trackX;
            dy = self.YPos() - trackY;
            if (rwIndex > 0)
            { // jpo - only valid if we have a runway.
                cosAngle = Airbase.brain.DetermineAngle(self, rwIndex, atcstatus);

                Airbase.brain.CalculateMinMaxTime(self, rwIndex, atcstatus, &min, &max, cosAngle);
            }
            else
            {
                cosAngle = 0;
                min = max = 0;
            }
            // edg: project out 1 sec to get alt for possible ground avoid
            float gAvoidZ = OTWDriver.GetGroundLevel(self.XPos() + self.XDelta(),
                                              self.YPos() + self.YDelta());
            float minZ = Airbase.brain.GetAltitude(self, atcstatus);

            switch (atcstatus)
            {
                case noATC:
                    if ((GetTTRelations(Airbase.GetTeam(), self.GetTeam()) >= Neutral))
                    {
                        airbase = self.DivertAirbase();
                        Airbase = (ObjectiveClass*)vuDatabase.Find(airbase);
                        if (!Airbase || (GetTTRelations(Airbase.GetTeam(), self.GetTeam()) >= Neutral))
                        {
                            GridIndex X, Y;

                            X = SimToGrid(af.y);
                            Y = SimToGrid(af.x);

                            Airbase = FindNearestFriendlyRunway(self.GetTeam(), X, Y);
                            if (Airbase)
                                airbase = Airbase.Id();
                            else
                            {
                                //need to find the airbase or we don't know where to go
                                if (self.af.GetSimpleMode())
                                {
                                    SimpleGoToCurrentWaypoint();
                                }
                                else
                                {
                                    GoToCurrentWaypoint();
                                }
                                return;
                            }

                        }
                    }
                    rwIndex = Airbase.brain.FindBestLandingRunway(self, true);
                    Airbase.brain.FindFinalPt(self, rwIndex, &trackX, &trackY);
                    trackZ = Airbase.brain.GetAltitude(self, atcstatus);
                    CalculateNextTurnDistance();
                    waittimer = SimLibElapsedTime + 2 * TAKEOFF_TIME_DELTA;
                    TrackPointLanding(af.CalcTASfromCAS(af.MinVcas() * 1.2F) * KNOTS_TO_FTPSEC);
                    dx = self.XPos() - Airbase.XPos();
                    dy = self.YPos() - Airbase.YPos();
                    //me123
                    if (!(curMode == LandingMode ||
                        curMode == TakeoffMode ||
                        curMode == WaypointMode ||
                        curMode == RTBMode))
                    break;

                    if (dx * dx + dy * dy < APPROACH_RANGE * Phyconst.NM_TO_FT * Phyconst.NM_TO_FT * 0.95F)
                    {
                        atcstatus = lReqClearance;
                        if (!isWing)
                            SendATCMsg(lReqClearance);
                    }
                    else
                    {
                        atcstatus = lIngressing;
                        SendATCMsg(lIngressing);
                    }
                    if (self.ZPos() - gAvoidZ > minZ)
                        trackZ = gAvoidZ + minZ - (self.ZPos() - gAvoidZ - minZ) * 2.0f;
                    break;

                //////////////////////////////////////////////////////////////////////////////////////
                case lIngressing:
                    dx = self.XPos() - Airbase.XPos();
                    dy = self.YPos() - Airbase.YPos();

                    if (dx * dx + dy * dy < APPROACH_RANGE * Phyconst.NM_TO_FT * Phyconst.NM_TO_FT * 0.95F)
                    {
                        rwIndex = Airbase.brain.FindBestLandingRunway(self, true);
                        Airbase.brain.FindFinalPt(self, rwIndex, &trackX, &trackY);
                        trackZ = Airbase.brain.GetAltitude(self, atcstatus);
                        CalculateNextTurnDistance();
                        atcstatus = lReqClearance;
                        if (!isWing)
                            SendATCMsg(lReqClearance);
                        waittimer = SimLibElapsedTime + LAND_TIME_DELTA;
                        //if emergency
                        //SendATCMsg(lReqEmerClearance);
                    }
                    if (self.ZPos() - gAvoidZ > minZ)
                        trackZ = gAvoidZ + minZ - (self.ZPos() - gAvoidZ - minZ) * 2.0f;
                    //TrackPointLanding( af.MinVcas() * 1.2F * KNOTS_TO_FTPSEC);
                    TrackPointLanding(af.CalcTASfromCAS(af.MinVcas() * 1.2F) * KNOTS_TO_FTPSEC);
                    //TrackPoint( 0.0F, af.MinVcas() * 1.2F * KNOTS_TO_FTPSEC);

                    break;

                //////////////////////////////////////////////////////////////////////////////////////
                case lTakingPosition:
                    //need to drag out formation
                    //the atc will clear us when we get within range
                    dx = self.XPos() - Airbase.XPos();
                    dy = self.YPos() - Airbase.YPos();
                    if (dx * dx + dy * dy < TOWER_RANGE * Phyconst.NM_TO_FT * Phyconst.NM_TO_FT * 0.5F && waittimer < SimLibElapsedTime)
                    {
                        atcstatus = lReqClearance;
                        if (!isWing)
                            SendATCMsg(lReqClearance);
                        waittimer = SimLibElapsedTime + LAND_TIME_DELTA;
                    }
                    speed = af.CalcTASfromCAS(af.MinVcas() * 1.2F) * KNOTS_TO_FTPSEC;

                    if (((Unit)self.GetCampaignObject()).GetTotalVehicles() > 1)
                    {
                        VuListIterator cit = new VuListIterator(self.GetCampaignObject().GetComponents());
                        component = (AircraftClass*)cit.GetFirst();
                        while (component && component.vehicleInUnit != self.vehicleInUnit)
                        {
                            leader = component;
                            component = (AircraftClass*)cit.GetNext();
                        }

                        if (leader
                            && !mpActionFlags[AI_RTB]) // JB 010527 (from MN)
                        {
                            dx = self.XPos() - leader.XPos();
                            dy = self.YPos() - leader.YPos();
                            dist = dx * dx + dy * dy;

                            if (dist < Phyconst.NM_TO_FT * Phyconst.NM_TO_FT)
                                speed = af.CalcTASfromCAS(af.MinVcas()) * KNOTS_TO_FTPSEC;

                            trackX = leader.XPos();
                            trackY = leader.YPos();
                            trackZ = leader.ZPos();
                        }
                        else
                        {
                            Airbase.brain.FindFinalPt(self, rwIndex, &trackX, &trackY);
                            trackZ = Airbase.brain.GetAltitude(self, lTakingPosition);
                        }
                    }
                    if (self.ZPos() - gAvoidZ > minZ)
                        trackZ = gAvoidZ + minZ - (self.ZPos() - gAvoidZ - minZ) * 2.0f;
                    TrackPointLanding(speed);
                    break;

                //////////////////////////////////////////////////////////////////////////////////////
                case lReqClearance:
                case lReqEmerClearance:
                    if (self.ZPos() - gAvoidZ > minZ)
                        trackZ = gAvoidZ + minZ - (self.ZPos() - gAvoidZ - minZ) * 2.0f;
                    if (SimLibElapsedTime > waittimer)
                    {
                        //we've been waiting too long, call again
                        rwIndex = Airbase.brain.FindBestLandingRunway(self, true);
                        Airbase.brain.FindFinalPt(self, rwIndex, &trackX, &trackY);
                        trackZ = Airbase.brain.GetAltitude(self, atcstatus);
                        if (self.ZPos() - gAvoidZ > minZ)
                            trackZ = gAvoidZ + minZ - (self.ZPos() - gAvoidZ - minZ) * 2.0f;
                        CalculateNextTurnDistance();

                        // JB 010802 RTBing AI aircraft won't land.
                        dx = self.XPos() - Airbase.XPos();
                        dy = self.YPos() - Airbase.YPos();
                        if (dx * dx + dy * dy < (TOWER_RANGE + 100) * Phyconst.NM_TO_FT * Phyconst.NM_TO_FT * 0.95F)
                        {
                            waittimer = SimLibElapsedTime + LAND_TIME_DELTA;
                            SendATCMsg(lReqClearance);
                        }
                        else
                            waittimer = SimLibElapsedTime + LAND_TIME_DELTA / 2;
                    }
                    //we're waiting to get a response back
                    TrackPointLanding(af.CalcTASfromCAS(af.MinVcas() * 1.2F) * KNOTS_TO_FTPSEC);
                    af.gearHandle = -1.0F;
                    break;

                //////////////////////////////////////////////////////////////////////////////////////
                case lAborted:
                    if (self.ZPos() - gAvoidZ > minZ)
                        trackZ = gAvoidZ + minZ - (self.ZPos() - gAvoidZ - minZ) * 2.0f;

                    if (dx * dx + dy * dy < 0.25F * Phyconst.NM_TO_FT * Phyconst.NM_TO_FT)
                    {
                        waittimer = SimLibElapsedTime + LAND_TIME_DELTA;
                        rwIndex = Airbase.brain.FindBestLandingRunway(self, true);
                        Airbase.brain.FindFinalPt(self, rwIndex, &trackX, &trackY);
                        trackZ = Airbase.brain.GetAltitude(self, lReqClearance);
                        z = TheMap.GetMEA(trackX, trackY);
                        if (self.ZPos() - gAvoidZ > minZ)
                            trackZ = gAvoidZ + minZ - (self.ZPos() - gAvoidZ - minZ) * 2.0f;
                        atcstatus = lReqClearance;
                        SendATCMsg(lReqClearance);
                    }

                    TrackPoint(0.0F, af.CalcTASfromCAS(af.MinVcas() * 1.2F) * KNOTS_TO_FTPSEC);
                    af.gearHandle = -1.0F;
                    break;

                //////////////////////////////////////////////////////////////////////////////////////
                case lEmerHold:
                    if (self.ZPos() - gAvoidZ > minZ)
                        trackZ = gAvoidZ + minZ - (self.ZPos() - gAvoidZ - minZ) * 2.0f;

                    if (waittimer < SimLibElapsedTime)
                    {
                        atcstatus = lEmerHold;
                        SendATCMsg(lEmerHold);
                        waittimer = SimLibElapsedTime + LAND_TIME_DELTA;
                    }
                    Loiter();
                    af.gearHandle = -1.0F;
                    break;

                //////////////////////////////////////////////////////////////////////////////////////
                case lHolding:
                    if (self.ZPos() - gAvoidZ > minZ)
                        trackZ = gAvoidZ + minZ - (self.ZPos() - gAvoidZ - minZ) * 2.0f;

                    if (rwtime < SimLibElapsedTime + max - CampaignSeconds * 5)
                    {
                        Airbase.brain.FindFinalPt(self, rwIndex, &finalX, &finalY);
                        waittimer = rwtime + CampaignSeconds * 15;

                        if (cosAngle < 0.0F)
                        {
                            Airbase.brain.FindBasePt(self, rwIndex, finalX, finalY, &baseX, &baseY);
                            atcstatus = Airbase.brain.FindFirstLegPt(self, rwIndex, rwtime, baseX, baseY, true, &trackX, &trackY);
                            trackZ = Airbase.brain.GetAltitude(self, atcstatus);
                            if (atcstatus != lHolding)
                                SendATCMsg(atcstatus);
                            CalculateNextTurnDistance();
                        }
                        else
                        {
                            atcstatus = Airbase.brain.FindFirstLegPt(self, rwIndex, rwtime, finalX, finalY, false, &trackX, &trackY);
                            trackZ = Airbase.brain.GetAltitude(self, atcstatus);
                            if (atcstatus != lHolding)
                                SendATCMsg(atcstatus);
                            CalculateNextTurnDistance();
                        }
                        if (self.ZPos() - gAvoidZ > minZ)
                            trackZ = gAvoidZ + minZ - (self.ZPos() - gAvoidZ - minZ) * 2.0f;

                        if (atcstatus == lHolding)
                            Loiter();
                    }
                    else
                    {
                        Loiter();
                    }
                    af.gearHandle = -1.0F;
                    break;

                //////////////////////////////////////////////////////////////////////////////////////
                case lFirstLeg:
                    if (self.pctStrength < 0.4F)
                        Airbase.brain.RequestEmerClearance(self);

                    if (self.ZPos() - gAvoidZ > minZ)
                        trackZ = gAvoidZ + minZ - (self.ZPos() - gAvoidZ - minZ) * 2.0f;

                    cosHdg = self.platformAngles.cossig;
                    sinHdg = self.platformAngles.sinsig;

                    relx = (cosHdg * dx + sinHdg * dy);
                    rely = (-sinHdg * dx + cosHdg * dy);

                    if (fabs(relx) < turnDist && fabs(rely) < turnDist * 3.0F)
                    {
                        Airbase.brain.FindFinalPt(self, rwIndex, &finalX, &finalY);
                        if (cosAngle < 0.0F)
                        {
                            atcstatus = lToBase;
                            Airbase.brain.FindBasePt(self, rwIndex, finalX, finalY, &baseX, &baseY);
                            trackX = baseX;
                            trackY = baseY;
                            trackZ = Airbase.brain.GetAltitude(self, atcstatus);
                            SendATCMsg(atcstatus);
                            CalculateNextTurnDistance();
                        }
                        else
                        {
                            atcstatus = lToFinal;
                            trackX = finalX;
                            trackY = finalY;
                            trackZ = Airbase.brain.GetAltitude(self, atcstatus);
                            SendATCMsg(atcstatus);
                            CalculateNextTurnDistance();
                        }
                        if (self.ZPos() - gAvoidZ > minZ)
                            trackZ = gAvoidZ + minZ - (self.ZPos() - gAvoidZ - minZ) * 2.0f;
                    }

                    TrackPointLanding(af.CalcTASfromCAS(af.MinVcas()) * KNOTS_TO_FTPSEC);
                    break;

                //////////////////////////////////////////////////////////////////////////////////////
                case lToBase:
                    if (self.pctStrength < 0.4F)
                        Airbase.brain.RequestEmerClearance(self);
                case lEmergencyToBase:

                    if (self.ZPos() - gAvoidZ > minZ)
                        trackZ = gAvoidZ + minZ - (self.ZPos() - gAvoidZ - minZ) * 2.0f;

                    //if(dx*dx + dy*dy < turnDist*turnDist)
                    cosHdg = self.platformAngles.cossig;
                    sinHdg = self.platformAngles.sinsig;

                    relx = (cosHdg * dx + sinHdg * dy);
                    rely = (-sinHdg * dx + cosHdg * dy);

                    if (fabs(relx) < turnDist && fabs(rely) < turnDist * 3.0F)
                    {
                        Airbase.brain.FindFinalPt(self, rwIndex, &finalX, &finalY);
                        if (atcstatus == lEmergencyToBase)
                            atcstatus = lEmergencyToFinal;
                        else
                            atcstatus = lToFinal;
                        trackX = finalX;
                        trackY = finalY;
                        trackZ = Airbase.brain.GetAltitude(self, atcstatus);
                        if (self.ZPos() - gAvoidZ > minZ)
                            trackZ = gAvoidZ + minZ - (self.ZPos() - gAvoidZ - minZ) * 2.0f;
                        SendATCMsg(atcstatus);
                        CalculateNextTurnDistance();
                    }

                    if (rwtime > SimLibElapsedTime + FINAL_TIME + BASE_TIME)
                    {
                        deltaTime = (rwtime - SimLibElapsedTime - FINAL_TIME - BASE_TIME) / (float)CampaignSeconds;
                        speed = (float)sqrt(dx * dx + dy * dy) / deltaTime;
                        speed = min(af.CalcTASfromCAS(af.MaxVcas() * 0.8F) * KNOTS_TO_FTPSEC,
                                        max(speed, af.CalcTASfromCAS(af.MinVcas() * 0.8F) * KNOTS_TO_FTPSEC));
                    }
                    else
                        speed = af.CalcTASfromCAS(af.MaxVcas() * 0.8F) * KNOTS_TO_FTPSEC;

                    TrackPointLanding(speed);
                    /*	if(TrackPointLanding(speed) > speed + 50.0F)
                        {
                            atcstatus = lHolding;
                            trackX = af.x;
                            trackY = af.y;
                            trackZ = Airbase.brain.GetAltitude(self, atcstatus);
                            SendATCMsg(atcstatus);
                        }*/
                    break;

                //////////////////////////////////////////////////////////////////////////////////////
                case lToFinal:
                    if (self.pctStrength < 0.4F)
                        Airbase.brain.RequestEmerClearance(self);
                case lEmergencyToFinal:

                    if (self.ZPos() - gAvoidZ > minZ)
                        trackZ = gAvoidZ + minZ - (self.ZPos() - gAvoidZ - minZ) * 2.0f;

                    cosHdg = PtHeaderDataTable[rwIndex].cosHeading;
                    sinHdg = PtHeaderDataTable[rwIndex].sinHeading;

                    relx = (cosHdg * dx + sinHdg * dy);
                    rely = (-sinHdg * dx + cosHdg * dy);

                    if (relx < 3.0F * Phyconst.NM_TO_FT && relx > -1.0F * Phyconst.NM_TO_FT && fabs(rely) < turnDist)
                    {
                        curTaxiPoint = GetFirstPt(rwIndex);
                        TranslatePointData(Airbase, curTaxiPoint, &trackX, &trackY);
                        atcstatus = lOnFinal;
                        if (atcstatus == lEmergencyToFinal)
                            atcstatus = lEmergencyOnFinal;
                        else
                            atcstatus = lOnFinal;

                        trackZ = Airbase.brain.GetAltitude(self, atcstatus);
                        SendATCMsg(atcstatus);
                        af.gearHandle = 1.0F;
                    }

                    if (rwtime > SimLibElapsedTime + FINAL_TIME)
                    {
                        deltaTime = (rwtime - SimLibElapsedTime - FINAL_TIME) / (float)CampaignSeconds;
                        speed = (float)sqrt(dx * dx + dy * dy) / deltaTime;
                        speed = min(af.CalcTASfromCAS(af.MaxVcas() * 0.8F) * KNOTS_TO_FTPSEC,
                                            max(speed, af.CalcTASfromCAS(af.MinVcas() * 0.8F) * KNOTS_TO_FTPSEC));
                    }
                    else
                        speed = af.CalcTASfromCAS(af.MaxVcas() * 0.8F) * KNOTS_TO_FTPSEC;

                    TrackPointLanding(speed);
                    /*	if(TrackPointLanding(speed) > speed + 20.0F)
                        {
                            atcstatus = lHolding;
                            trackX = af.x;
                            trackY = af.y;
                            trackZ = Airbase.brain.GetAltitude(self, atcstatus);
                            SendATCMsg(atcstatus);
                        }*/
                    break;

                //////////////////////////////////////////////////////////////////////////////////////
                case lEmergencyOnFinal:
                case lOnFinal:
                    // now we just wait until touchdown
                    if (self.IsSetFlag(ON_GROUND))
                    {
                        atcstatus = lLanded;
                        SendATCMsg(atcstatus);
                        ClearATCFlag(RequestTakeoff);
                        SetATCFlag(Landed);
                        curTaxiPoint = GetFirstPt(rwIndex);
                        curTaxiPoint = GetNextPtLoop(curTaxiPoint);
                        TranslatePointData(Airbase, curTaxiPoint, &trackX, &trackY);
                        trackZ = af.groundZ;
                        SimpleGroundTrack(100.0F * KNOTS_TO_FTPSEC);
                        break;
                    }

                    //don't do ground avoidance
                    groundAvoidNeeded = false;
                    if (af.vt < af.CalcTASfromCAS(af.MinVcas() - 30.0F) * KNOTS_TO_FTPSEC ||
                        (af.gearPos > 0.0F && af.vt < af.CalcTASfromCAS(af.MinVcas() + 10.0F) * KNOTS_TO_FTPSEC))
                        af.gearHandle = 1.0F;
                    if (cosAngle > 0.0F && af.groundZ - self.ZPos() > 50.0F)
                    {
                        curTaxiPoint = GetFirstPt(rwIndex);
                        TranslatePointData(Airbase, curTaxiPoint, &trackX, &trackY);

                        //until chris moves the landing points
                        //trackX -= 500.0F * PtHeaderDataTable[rwIndex].cosHeading;
                        //trackY -= 500.0F * PtHeaderDataTable[rwIndex].sinHeading;

                        dx = trackX - self.XPos();
                        dy = trackY - self.YPos();
                        dist = (float)sqrt(dx * dx + dy * dy);

                        //decelerate as we approach
                        minSpeed = af.CalcTASfromCAS(af.MinVcas()) * KNOTS_TO_FTPSEC;

                        deltaTime = ((float)rwtime - (float)SimLibElapsedTime) / CampaignSeconds;
                        testDist = (minSpeed * 0.2F / (FINAL_TIME / CampaignSeconds) * deltaTime + minSpeed * 0.6F) * deltaTime;
                        desiredSpeed = (minSpeed * 0.4F / (FINAL_TIME / CampaignSeconds) * deltaTime + minSpeed * 0.6F);

                        if (dist > minSpeed * 19.5F)
                            desiredSpeed += (dist - testDist) / (testDist * 0.8F) * desiredSpeed;
                        else if (desiredSpeed > af.vt)
                            desiredSpeed = af.vt;

                        desiredSpeed = max(min(minSpeed * 1.2F, desiredSpeed), minSpeed * 0.6F);
                        finalZ = Airbase.brain.GetAltitude(self, lOnFinal);
                        trackZ = finalZ - dist * TAN_THREE_DEG_GLIDE;

                        //recalculate track point to help line up better
                        trackX += dist * 0.8F * PtHeaderDataTable[rwIndex].cosHeading;
                        trackY += dist * 0.8F * PtHeaderDataTable[rwIndex].sinHeading;

                        testDist = 0;
                        x = self.XPos();
                        y = self.YPos();

                        while (testDist < dist * 0.2F && dist > 3000.0F)
                        {
                            x -= 200.0F * PtHeaderDataTable[rwIndex].cosHeading;
                            y -= 200.0F * PtHeaderDataTable[rwIndex].sinHeading;

                            z = OTWDriver.GetGroundLevel(x, y);
                            if (dist < 6000.0F)
                            {
                                if (z - 100.0F < trackZ)
                                    trackZ = z - 100.0F;
                            }
                            else
                            {
                                if (z - 200.0F < trackZ)
                                    trackZ = z - 200.0F;
                            }

                            testDist += 200.0F;
                        }
                        if (af.groundZ - self.ZPos() > 200.0F)
                            TrackPointLanding(desiredSpeed);
                        else
                        {
                            TrackPointLanding(af.GetLandingAoASpd());
#if NOTHING // JPO removed
				if(	self.GetSType() == STYPE_AIR_ATTACK ||
					self.GetSType() == STYPE_AIR_FIGHTER ||
					self.GetSType() == STYPE_AIR_FIGHTER_BOMBER ||
					self.GetSType() == STYPE_AIR_ECM)
						TrackPointLanding(af.CalcDesSpeed(12.5F));
				else
						TrackPointLanding(af.CalcDesSpeed(8.0F));
#endif
                        }
                    }
                    else
                    {
                        //flare at the last minute so we hit softly
                        curTaxiPoint = GetFirstPt(rwIndex);
                        curTaxiPoint = GetNextPt(curTaxiPoint);
                        TranslatePointData(Airbase, curTaxiPoint, &trackX, &trackY);
                        trackZ = Airbase.brain.GetAltitude(self, atcstatus);
                        TrackPointLanding(af.GetLandingAoASpd());
#if NOTHING
			if(	self.GetSType() == STYPE_AIR_ATTACK ||
				self.GetSType() == STYPE_AIR_FIGHTER ||
				self.GetSType() == STYPE_AIR_FIGHTER_BOMBER ||
				self.GetSType() == STYPE_AIR_ECM)
					TrackPointLanding(af.CalcDesSpeed(12.5F));
			else
					TrackPointLanding(af.CalcDesSpeed(8.0F));
#endif
                        /*
			if(af.isF16)
				TrackPointLanding(af.MinVcas()* KNOTS_TO_FTPSEC*0.55F);
			else
				TrackPointLanding(af.MinVcas()* KNOTS_TO_FTPSEC*0.6F);
				*/
                        pStick = -0.01685393258427F;
                    }
                    break;

                //////////////////////////////////////////////////////////////////////////////////////
                case lLanded:
                    // edg: If we're not on the ground, we're fucked (which has
                    // been seen).  We can't stay in this state.  Go to aborted.
                    if (!self.OnGround())
                    {
                        ShiWarning("Please show this to Dave P (x4373)");

                        float rx = self.dmx[0][0] * dx + self.dmx[0][1] * dy;

                        if (rx > 3000.0F && af.IsSet(AirframeClass.OverRunway))
                        {
                            atcstatus = lOnFinal;
                            SendATCMsg(atcstatus);
                            ClearATCFlag(Landed);
                            TrackPointLanding(af.MinVcas() * KNOTS_TO_FTPSEC * 0.6F);
                            pStick = -0.01685393258427F;
                        }
                        else
                        {
                            atcstatus = lAborted;
                            SendATCMsg(atcstatus);
                            Airbase.brain.FindAbortPt(self, &trackX, &trackY, &trackZ);
                            TrackPoint(0.0F, af.MinVcas() * 1.2F * KNOTS_TO_FTPSEC);
                            af.gearHandle = -1.0F;
                        }
                        break;
                    }

                    trackZ = af.groundZ;
                    if (CloseToTrackPoint())
                    {
                        // Are we there yet?
                        switch (PtDataTable[GetNextPtLoop(curTaxiPoint)].type)
                        {
                            case TaxiPt:
                                curTaxiPoint = GetNextPtLoop(curTaxiPoint);
                                atcstatus = lTaxiOff;
                                waittimer = 0;
                                SendATCMsg(atcstatus);
                                break;

                            case TakeRunwayPt:
                            case TakeoffPt:
                                curTaxiPoint = GetNextPtLoop(curTaxiPoint);
                                break;

                            case RunwayPt:
                                //we shouldn't be here
                                curTaxiPoint = GetNextPtLoop(curTaxiPoint);
                                break;
                        }

                        trackZ = af.groundZ;
                        TranslatePointData(Airbase, curTaxiPoint, &trackX, &trackY);
                    }

                    inTheWay = CheckTaxiTrackPoint();
                    if (inTheWay && inTheWay.Vt() < 10.0F)
                    {
                        heading = (float)atan2(trackX - self.XPos(), trackY - self.YPos());
                        mlSinCos(&Trig, heading);

                        trackX += TAXI_CHECK_DIST * Trig.cos;
                        trackY += TAXI_CHECK_DIST * Trig.sin;
                    }

                    // JPO pop the chute
                    if (af.HasDragChute() &&
                        af.dragChute == AirframeClass.DRAGC_STOWED &&
                        af.vcas < af.DragChuteMaxSpeed())
                    {
                        af.dragChute = AirframeClass.DRAGC_DEPLOYED;
                    }
                    if (af.vt < af.MinVcas() * 0.5F)
                    { // clean up
                        if (af.speedBrake > -1.0F)
                            af.speedBrake = -1.0F;
                        if (af.tefPos > 0)
                            af.TEFClose();
                        if (af.lefPos > 0)
                            af.LEFClose();
                        if (af.dragChute == AirframeClass.DRAGC_DEPLOYED)
                            af.dragChute = AirframeClass.DRAGC_JETTISONNED;
                    }


                    dx = trackX - af.x;
                    dy = trackY - af.y;
                    if (dx * dx + dy * dy > 2000.0F * 2000.0F)
                    {
                        SimpleGroundTrack(100.0F * KNOTS_TO_FTPSEC);
                    }
                    else if (dx * dx + dy * dy > 1000.0F * 1000.0F)
                    {
                        SimpleGroundTrack(75.0F * KNOTS_TO_FTPSEC);
                    }
                    else
                    {
                        SimpleGroundTrack(20.0F * KNOTS_TO_FTPSEC);
                    }
                    break;

                //////////////////////////////////////////////////////////////////////////////////////
                case lTaxiOff:

                    // JPO - only halt when we really get there, so the above condition continues to fire.
                    if (AtFinalTaxiPoint())
                    {
                        if (waittimer == 0)
                            waittimer = SimLibElapsedTime + g_nReagTimer * CampaignMinutes;
                        dx = trackX - af.x;
                        dy = trackY - af.y;
                        if (dx * dx + dy * dy < 10 * 10)
                            desiredSpeed = 0.0F;
                        else
                            desiredSpeed = 20.0F * KNOTS_TO_FTPSEC * (float)sqrt(dx * dx + dy * dy) / TAXI_CHECK_DIST;
                        desiredSpeed = min(20.0F * KNOTS_TO_FTPSEC, desiredSpeed);
                        if (waittimer < SimLibElapsedTime || (desiredSpeed == 0 && af.vt < 0.1f))
                        {
                            // then clean up
                            if (!af.canopyState)
                                af.CanopyToggle();
                            else if (!af.IsSet(AirframeClass.EngineStopped))
                            {
                                af.SetFlag(AirframeClass.EngineStopped);
                            }
                            else if (af.rpm < 0.05f &&
                                self.MainPower() != AircraftClass.MainPowerOff)
                            {
                                self.DecMainPower();
                            }
                            else if (self != SimDriver.playerEntity && (g_nReagTimer <= 0 || waittimer < SimLibElapsedTime))
                                RegroupAircraft(self); //end of the line, time to pull you
                        }
                    }
                    else if (CloseToTrackPoint()) // time to step along
                    {
                        switch (PtDataTable[GetNextPtLoop(curTaxiPoint)].type)
                        {
                            case TaxiPt: // nothing special
                            default:
                                curTaxiPoint = GetNextPtLoop(curTaxiPoint);
                                break;
                            case SmallParkPt:
                            case LargeParkPt: // possible parking spot
                                FindParkingSpot(Airbase);
                                break;
                        }
                        // Are we there yet?
                        switch (PtDataTable[curTaxiPoint].flags)
                        {
                            //this taxi point is in middle of list
                            case 0:
                                break;

                            //this should be the runway pt, we shouldn't be here
                            case PT_FIRST:
                                break;

                            case PT_LAST:
                                if (waittimer == 0) // first time
                                    waittimer = SimLibElapsedTime + g_nReagTimer * CampaignMinutes;
                                if (self != SimDriver.playerEntity && (g_nReagTimer <= 0 || waittimer < SimLibElapsedTime))
                                    RegroupAircraft(self); //end of the line, time to pull you
                                break;
                        }

                        trackZ = af.groundZ;
                        TranslatePointData(Airbase, curTaxiPoint, &trackX, &trackY);
                        desiredSpeed = 20.0F * KNOTS_TO_FTPSEC;
                    }
                    else
                    {
                        desiredSpeed = 20.0F * KNOTS_TO_FTPSEC;
                    }
                    inTheWay = CheckTaxiTrackPoint();
                    if (inTheWay && inTheWay.Vt() < 10.0F)
                    {
                        switch (PtDataTable[curTaxiPoint].type)
                        {
                            case SmallParkPt: //was a possible parking spot, alas no more.
                            case LargeParkPt: // someone beat us to it.
                                if (PtDataTable[curTaxiPoint].flags != PT_LAST)
                                {
                                    FindParkingSpot(Airbase); // try again
                                    TranslatePointData(Airbase, curTaxiPoint, &trackX, &trackY);
                                    break;
                                }
                            // else fall
                            default:
                                OffsetTrackPoint(TAXI_CHECK_DIST, offRight); // JPO from offRight
                                break;
                        }

                    }

                    SimpleGroundTrack(desiredSpeed);
                    break;

                default:
                    break;
            }
        }

        protected void TakeOff()
        {
            SimBaseClass* inTheWay = null;
            ObjectiveClass* Airbase = (ObjectiveClass*)vuDatabase.Find(airbase);
            float xft, yft, rx, distSq;

            if (!Airbase || !Airbase.IsObjective() || !Airbase.brain)
            {
                //need to find the airbase or we don't know where to go
                // JB carrier ShiWarning("Show this to Dave P. (no airbase)");
                return;
            }

            if (self.IsSetFlag(ON_GROUND) && af.Fuel() <= 0.0F && af.vt < 5.0F)
            {
                if (self != SimDriver.playerEntity)
                    RegroupAircraft(self); //no gas get him out of the way
            }

            if (af.IsSet(AirframeClass.EngineOff))
                af.ClearFlag(AirframeClass.EngineOff);
            if (af.IsSet(AirframeClass.ThrottleCheck))
                af.ClearFlag(AirframeClass.ThrottleCheck);

            SetDebugLabel(Airbase);

            //me123 make sure ap if off for player in multiplay
            if (self == SimDriver.playerEntity && IsSetATC(StopPlane))
            {
                ClearATCFlag(StopPlane);
                af.ClearFlag(AirframeClass.WheelBrakes);
                self.SetAutopilot(AircraftClass.APOff);
                return;
            }
            // JPO - should we run preflight checks...
            if (!IsSetATC(DonePreflight) && curTaxiPoint)
            {
                VU_TIME t2t; // time to takeoff
                if (rwtime > 0) // value given by ATC
                    t2t = rwtime;
                else
                    t2t = self.curWaypoint.GetWPDepartureTime(); // else original scheduled time

                if (SimLibElapsedTime > t2t - 3 * CampaignMinutes) // emergency pre-flight
                    QuickPreFlight();
                else if (SimLibElapsedTime < t2t - PlayerOptionsClass.RAMP_MINUTES * CampaignMinutes)
                    return; // not time for startup yet
                else if (PtDataTable[curTaxiPoint].flags == PT_LAST ||
                    PtDataTable[curTaxiPoint].type == SmallParkPt ||
                    PtDataTable[curTaxiPoint].type == LargeParkPt)
                {
                    if (!PreFlight()) // slow preflight
                        return;
                }
                else
                    QuickPreFlight();
                SetATCFlag(DonePreflight);
            }

            // if we're damaged taxi back
            if (self.pctStrength < 0.7f && self.IsSetFlag(ON_GROUND))
            {
                TaxiBack(Airbase);
                return;
            }

            xft = trackX - af.x;
            yft = trackY - af.y;
            rx = self.dmx[0][0] * xft + self.dmx[0][1] * yft;
            distSq = xft * xft + yft * yft;

            groundAvoidNeeded = false;

            if (!curTaxiPoint)
            {
                Flight flight = (Flight)self.GetCampaignObject();
                if (flight)
                {
                    WayPoint w;

                    w = flight.GetFirstUnitWP();
                    if (w)
                    {
                        curTaxiPoint = FindDesiredTaxiPoint(w.GetWPDepartureTime());
                        TranslatePointData(Airbase, curTaxiPoint, &trackX, &trackY);
                        CalcWaitTime(Airbase.brain);
                    }
                }
            }

            if (g_nShowDebugLabels & 0x80000)
            {
                string label;
                sprintf(label, "TaxiPt %d, type: ", curTaxiPoint);
                switch (PtDataTable[curTaxiPoint].type)
                {
                    case SmallParkPt:
                        strcat(label, "SmallParkPt");
                        break;
                    case LargeParkPt:
                        strcat(label, "LargeParkPt");
                        break;
                    case TakeoffPt:
                        strcat(label, "TakeoffPt");
                        break;
                    case RunwayPt:
                        strcat(label, "RunwayPt");
                        break;
                    case TaxiPt:
                        strcat(label, "TaxiPt");
                        break;
                    case CritTaxiPt:
                        strcat(label, "CritTaxiPt");
                        break;
                    case TakeRunwayPt:
                        strcat(label, "TakeRunwayPt");
                        break;
                }
                if (self.drawPointer)
                    ((DrawableBSP*)self.drawPointer).SetLabel(label, ((DrawableBSP*)self.drawPointer).LabelColor());
            }


            switch (atcstatus)
            {
                case noATC:
                    if (!self.IsSetFlag(ON_GROUND))
                    {
                        //			Debug.Assert(!"Show this to Dave P. (not on ground)");
                        if (isWing)
                            self.curWaypoint = self.curWaypoint.GetNextWP();
                        else
                            SelectNextWaypoint();
                        break;
                    }

                    trackZ = af.groundZ;
                    ClearATCFlag(RequestTakeoff);
                    ClearATCFlag(PermitRunway);
                    ClearATCFlag(PermitTakeoff);

                    switch (PtDataTable[curTaxiPoint].type)
                    {
                        case SmallParkPt:
                        case LargeParkPt:
                            atcstatus = tReqTakeoff;
                            waittimer = SimLibElapsedTime + TAKEOFF_TIME_DELTA;
                            TranslatePointData(Airbase, curTaxiPoint, &trackX, &trackY);
                            if (!isWing)
                            {
                                SendATCMsg(tReqTakeoff);
                            }
                            break;

                        case TakeoffPt:
                            atcstatus = tReqTakeoff;
                            rwIndex = Airbase.brain.IsOnRunway(self);
                            if (GetFirstPt(rwIndex) == curTaxiPoint - 1)
                                curTaxiPoint = Airbase.brain.FindTakeoffPt((Flight)self.GetCampaignObject(), self.vehicleInUnit, rwIndex, &trackX, &trackY);
                            else
                            {
                                rwIndex = Airbase.brain.GetOppositeRunway(rwIndex);
                                curTaxiPoint = Airbase.brain.FindTakeoffPt((Flight)self.GetCampaignObject(), self.vehicleInUnit, rwIndex, &trackX, &trackY);
                            }
                            waittimer = SimLibElapsedTime;
                            if (!isWing)
                            {
                                SendATCMsg(atcstatus);
                            }
                            break;

                        case RunwayPt:
                            atcstatus = tReqTakeoff;
                            rwIndex = Airbase.brain.IsOnRunway(self);
                            if (GetFirstPt(rwIndex) == curTaxiPoint)
                                curTaxiPoint = Airbase.brain.FindRunwayPt((Flight)self.GetCampaignObject(), self.vehicleInUnit, rwIndex, &trackX, &trackY);
                            else
                            {
                                rwIndex = Airbase.brain.GetOppositeRunway(rwIndex);
                                curTaxiPoint = Airbase.brain.FindRunwayPt((Flight)self.GetCampaignObject(), self.vehicleInUnit, rwIndex, &trackX, &trackY);
                            }
                            waittimer = SimLibElapsedTime;
                            if (!isWing)
                            {
                                SendATCMsg(tReqTakeoff);
                            }
                            break;

                        case TaxiPt:
                        case CritTaxiPt:
                        case TakeRunwayPt:
                        default:
                            atcstatus = tReqTakeoff;
                            TranslatePointData(Airbase, curTaxiPoint, &trackX, &trackY);
                            waittimer = SimLibElapsedTime + TAKEOFF_TIME_DELTA;
                            if (!isWing)
                            {
                                SendATCMsg(tReqTakeoff);
                            }
                            break;
                    }

                    SimpleGroundTrack(0.0F);
                    break;

                //////////////////////////////////////////////////////////////////////////////////////
                case tReqTakeoff:
                case tReqTaxi:
                    if (SimLibElapsedTime > waittimer + TAKEOFF_TIME_DELTA)
                    {
                        //we've been waiting too long, call again
                        SendATCMsg(atcstatus);
                        waittimer = SimLibElapsedTime;
                    }
                    //we're waiting to get a response back
                    SimpleGroundTrack(0.0F);
                    break;

                //////////////////////////////////////////////////////////////////////////////////////
                case tWait:
                    desiredSpeed = 0.0F;
                    if ((distSq < TAXI_CHECK_DIST * TAXI_CHECK_DIST) || (rx < 1.0F && distSq < TAXI_CHECK_DIST * TAXI_CHECK_DIST * 4.0F))
                        ChooseNextPoint(Airbase);
                    else
                    {
                        inTheWay = CheckTaxiTrackPoint();
                        if (inTheWay)
                        {
                            if (isWing)
                                DealWithBlocker(inTheWay, Airbase);
                        }
                        else
                        {
                            //default speed
                            if (SimLibElapsedTime > waittimer + TAKEOFF_TIME_DELTA)
                                CalculateTaxiSpeed(3.0F);
                            else
                                CalculateTaxiSpeed(5.0F);
                        }
                        trackZ = af.groundZ;
                    }

                    SimpleGroundTrack(desiredSpeed);
                    break;

                //////////////////////////////////////////////////////////////////////////////////////
                case tTaxi:
                    desiredSpeed = 0.0F;
                    //if we haven't reached our desired taxi point, we need to move
                    if ((distSq < TAXI_CHECK_DIST * TAXI_CHECK_DIST) || (rx < 1.0F && distSq < TAXI_CHECK_DIST * TAXI_CHECK_DIST * 4.0F))
                        ChooseNextPoint(Airbase);
                    else
                    {
                        inTheWay = CheckTaxiTrackPoint();
                        if (inTheWay)
                        {
                            //someone is in the way
                            DealWithBlocker(inTheWay, Airbase);
                        }
                        else
                        {
                            //default speed
                            if (SimLibElapsedTime > waittimer + TAKEOFF_TIME_DELTA)
                                CalculateTaxiSpeed(3.0F);
                            else
                                CalculateTaxiSpeed(5.0F);
                        }
                        trackZ = af.groundZ;
                    }

                    SimpleGroundTrack(desiredSpeed);
                    break;

                //////////////////////////////////////////////////////////////////////////////////////
                case tHoldShort:
                    desiredSpeed = 0.0F;
                    if (rwtime < 5 * CampaignSeconds + SimLibElapsedTime && waittimer < SimLibElapsedTime)
                    {
                        SendATCMsg(atcstatus);
                        waittimer = CalcWaitTime(Airbase.brain);
                    }

                    ChooseNextPoint(Airbase);

                    if (desiredSpeed == 0.0F && Airbase.brain.IsOnRunway(trackX, trackY))
                    {
                        OffsetTrackPoint(50.0F, rightRunway);
                        CalculateTaxiSpeed(5.0F);
                    }

                    SimpleGroundTrack(desiredSpeed);
                    break;

                //////////////////////////////////////////////////////////////////////////////////////
                case tTakeRunway:
                    desiredSpeed = 0.0F;
                    //if we haven't reached our desired taxi point, we need to move
                    if ((distSq < TAXI_CHECK_DIST * TAXI_CHECK_DIST) || (rx < 0.0F && distSq < TAXI_CHECK_DIST * TAXI_CHECK_DIST * 4.0F))
                    {
                        if (PtDataTable[curTaxiPoint].type != RunwayPt)
                            ChooseNextPoint(Airbase);
                    }
                    else
                    {
                        inTheWay = CheckTaxiTrackPoint();
                        if (inTheWay)
                            DealWithBlocker(inTheWay, Airbase);
                        else
                            CalculateTaxiSpeed(5.0F);
                        trackZ = af.groundZ;
                    }

                    if (PtDataTable[curTaxiPoint].type == RunwayPt)
                    {
                        if (isWing && self.af.IsSet(AirframeClass.OverRunway) && !WingmanTakeRunway(Airbase))
                        {
                            curTaxiPoint = Airbase.brain.FindTakeoffPt((Flight)self.GetCampaignObject(), self.vehicleInUnit, rwIndex, &trackX, &trackY);
                            OffsetTrackPoint(50.0F, rightRunway);
                            CalculateTaxiSpeed(5.0F);
                            curTaxiPoint++;
                        }
                        else if (!self.af.IsSet(AirframeClass.OverRunway))
                        {
                            OffsetTrackPoint(0.0F, centerRunway);
                            curTaxiPoint++;
                        }
                        else
                        {
                            float cosAngle = self.platformAngles.sinsig * PtHeaderDataTable[rwIndex].sinHeading +
                                                self.platformAngles.cossig * PtHeaderDataTable[rwIndex].cosHeading;

                            if (cosAngle > 0.99619F)
                            {
                                if (ReadyToGo())
                                {
                                    waittimer = 0;
                                    atcstatus = tTakeoff;
                                    SendATCMsg(atcstatus);
                                    trackZ = af.groundZ - 500.0F;
                                }
                                else
                                    desiredSpeed = 0.0F;
                            }
                        }
                    }

                    SimpleGroundTrack(desiredSpeed);

                    // edg: test for fuckupedness -- I've seen planes taking the runway
                    // which are already in the air (bad trackX and Y?).  They never
                    // get out of this take runway cycle.   If we find ourselves in
                    // this state go to take off since we're off already....
                    if (!self.OnGround())
                    {
                        ShiWarning("Show this to Dave P. (not on ground)");
                        waittimer = 0;
                        atcstatus = tTakeoff;
                        SendATCMsg(atcstatus);
                    }
                    break;

                //////////////////////////////////////////////////////////////////////////////////////
                case tTakeoff:
                    if (self.OnGround() && !self.af.IsSet(AirframeClass.OverRunway))
                    {
                        curTaxiPoint = Airbase.brain.FindTakeoffPt((Flight)self.GetCampaignObject(), self.vehicleInUnit, rwIndex, &trackX, &trackY);
                        atcstatus = tTakeRunway;
                        SendATCMsg(atcstatus);
                        CalculateTaxiSpeed(5.0F);
                        SimpleGroundTrack(desiredSpeed);
                        return;
                    }

                    if (self.OnGround())
                        SimpleGroundTrack(af.MinVcas() * KNOTS_TO_FTPSEC);
                    else
                        TrackPoint(0.0F, (af.MinVcas() + 50.0F) * KNOTS_TO_FTPSEC);

                    if (af.z - af.groundZ < -20.0F && af.gearHandle > -1.0F)
                        af.gearHandle = -1.0F;

                    if (af.z - af.groundZ < -50.0F)
                    {
                        if (af.gearHandle > -1.0F)
                            af.gearHandle = -1.0F;
                        atcstatus = tFlyOut;
                        SendATCMsg(atcstatus);
                    }
                    break;

                //////////////////////////////////////////////////////////////////////////////////////
                case tFlyOut:
                    int elements;
                    trackZ = af.groundZ - 500.0F;
                    if (af.gearHandle > -1.0F)
                        af.gearHandle = -1.0F;

                    // 2001-10-16 added by M.N. #1 performs a 90° base leg to waypoint 2 at takeoff

                    // Needed so that lead that will perform the leg will first fly out before it starts the leg
                    if (af.z - af.groundZ > -200.0F || (fabs(xft) < 200.0F && fabs(yft) < 200.0F))
                        break;

                    elements = self.GetCampaignObject().NumberOfComponents();

                    // 2001-10-16 M.N. added elemleader check . perform a 90° base leg until element lead has taken off
                    if (!isWing && elements > 1) // Code for the flightlead
                    {
                        AircraftClass* elemleader = (AircraftClass*)self.GetCampaignObject().GetComponentNumber(elements == 2 ? 1 : 2); // wingy or lead
                        if (elemleader) // is #3 in a 4- and 3-ship flight, #2 in a 2-ship flight
                        {
                            airbase = self.LandingAirbase(); // JPO - now we set to go home
                            ObjectiveClass* Airbase = (ObjectiveClass*)vuDatabase.Find(airbase);
                            if (!Airbase || elemleader.af.z - elemleader.af.groundZ < -50.0F) // #3 has taken off . lead continue to next WP
                            {
                                onStation = NotThereYet;
                                SelectNextWaypoint();
                                atcstatus = noATC;
                                SendATCMsg(atcstatus);

                            }
                            else
                            {	// #1 and #2 do a takeoff leg - find direction to next waypoint

                                int dir;
                                float tx, ty, dx, dy, dz, dist;
                                float deltaHdg, hdgToPt, acHeading, legHeading;

                                acHeading = self.Yaw(); // fix, use aircrafts real heading instead of runway heading

                                WayPointClass* tmpWaypoint = self.curWaypoint;
                                if (tmpWaypoint)
                                {
                                    Debug.Assert(tmpWaypoint.GetWPAction() == WP_TAKEOFF);
                                    // add this if we have	if (tmpWaypoint.GetWPAction() != WP_TAKEOFF && tmpWaypoint.GetPrevWP())
                                    // a failed assertion		tmpWaypoint = tmpWaypoint.GetPrevWP();

                                    tmpWaypoint = tmpWaypoint.GetNextWP();
                                    tmpWaypoint.GetLocation(&dx, &dy, &dz);

                                    tx = dx - Airbase.XPos();
                                    ty = dy - Airbase.YPos();
                                    hdgToPt = (float)atan2(ty, tx);
                                    if (hdgToPt < 0.0F)
                                        hdgToPt += PI * 2.0F;


                                    if (acHeading < 0.0F)
                                        acHeading += PI * 2.0F;

                                    deltaHdg = hdgToPt - acHeading;
                                    if (deltaHdg > PI)
                                        deltaHdg -= (2.0F * PI);
                                    else if (deltaHdg < -PI)
                                        deltaHdg += (2.0F * PI);
                                    if (deltaHdg < -PI)
                                        dir = 1;
                                    else if (deltaHdg > PI)
                                        dir = 0;
                                    else if (deltaHdg < 0.0F)
                                        dir = 0;//left
                                    else
                                        dir = 1;//right

                                    legHeading = hdgToPt;

                                    // MN CTD fix #2
                                    AircraftClass* wingman = null;
                                    FlightClass* flight = null;
                                    flight = (FlightClass*)self.GetCampaignObject();
                                    Debug.Assert(!F4IsBadReadPtr(flight, sizeof(FlightClass)));
                                    float factor = 0.0F;
                                    if (flight)
                                    {
                                        wingman = (AircraftClass*)flight.GetComponentNumber(1); // my wingy = 1 in each case
                                        Debug.Assert(!F4IsBadReadPtr(wingman, sizeof(AircraftClass)));

                                        if (wingman && wingman.af && (wingman.af.z - wingman.af.groundZ < -200.0F))	// My wingman has flown out
                                            factor = 45.0F * DTR;

                                        Debug.Assert(Airbase.brain);
                                        if (Airbase.brain && Airbase.brain.UseSectionTakeoff(flight, rwIndex))	// If our wingman took off with us, stay on a 90° leg
                                            factor = 0.0F;
                                    }

                                    if (dir)
                                        legHeading = legHeading - (90.0F * DTR - factor);
                                    else
                                        legHeading = legHeading + (90.0F * DTR - factor);

                                    if (legHeading >= 360.0F * DTR)
                                        legHeading -= 360.0F * DTR;
                                    if (legHeading < 0.0F)
                                        legHeading += 360.0F * DTR;

                                    dist = 10.0F * Phyconst.NM_TO_FT;

                                    // Set up a new trackpoint

                                    dx = Airbase.XPos() + dist * cos(legHeading);
                                    dy = Airbase.YPos() + dist * sin(legHeading);

                                    trackX = dx;
                                    trackY = dy;
                                    trackZ = -2000.0F + af.groundZ;

                                    SetMaxRollDelta(75.0F); // don't roll too much
                                    SimpleTrack(SimpleTrackSpd, (af.MinVcas() * 1.2)); // fly as slow as possible ~ 178 kts
                                    SetMaxRollDelta(100.0F);
                                    break;
                                }

                            }
                        }
                    }
                    else
                        // In the air and ready to go
                        if (af.z - af.groundZ < -200.0F || (fabs(xft) < 200.0F && fabs(yft) < 200.0F))
                        {
                            onStation = NotThereYet;
                            if (isWing)
                                self.curWaypoint = self.curWaypoint.GetNextWP();
                            else
                                SelectNextWaypoint();
                            atcstatus = noATC;
                            SendATCMsg(atcstatus);
#if DAVE_DBG
			 SetLabel(self);
#endif

                            if (isWing)
                                AiRejoin(null, AI_TAKEOFF); // JPO actually a takeoff signal
                            airbase = self.LandingAirbase(); // JPO - now we set to go home
                        }

                    TrackPoint(0.0F, (af.MinVcas() + 50.0F) * KNOTS_TO_FTPSEC);
                    break;

                //////////////////////////////////////////////////////////////////////////////////////
                case tEmerStop:
                    desiredSpeed = 0.0F;
                    if (waittimer < SimLibElapsedTime)
                    {
                        SendATCMsg(atcstatus);
                        waittimer = CalcWaitTime(Airbase.brain);
                    }

                    if (!isWing || (flightLead && flightLead.OnGround()))
                    {
                        while (Airbase.brain.IsOnRunway(trackX, trackY))
                        {
                            OffsetTrackPoint(20.0F, rightRunway);
                            desiredSpeed = 20.0F;
                        }

                        if (Airbase.brain.IsOnRunway(self) && af.vt < 5.0F)
                        {
                            OffsetTrackPoint(20.0F, rightRunway);
                            desiredSpeed = 20.0F;
                        }
                    }

                    if (fabs(trackX - af.x) > TAXI_CHECK_DIST || fabs(trackY - af.y) > TAXI_CHECK_DIST)
                    {
                        inTheWay = CheckTaxiTrackPoint();
                        if (inTheWay)
                        {
                            if (isWing)
                                DealWithBlocker(inTheWay, Airbase);
                            else
                                desiredSpeed = 0.0F;
                        }
                        else
                        {
                            //default speed
                            CalculateTaxiSpeed(5.0F);
                        }
                    }
                    else
                        ChooseNextPoint(Airbase);

                    SimpleGroundTrack(desiredSpeed);
                    break;

                //////////////////////////////////////////////////////////////////////////////////////
                case tTaxiBack:
                    //this will cause them to taxi back, will only occur if ordered by ATC
                    TaxiBack(Airbase);
                    break;

                default:
                    break;
            }
        }

        protected int PreFlight()
        {
            Debug.Assert(af != null);
            if (SimLibElapsedTime < mNextPreflightAction) return 0;

            switch (PreFlightTable[mActionIndex].action)
            {
                case PreflightActions.FNX:
                    if ((*PreFlightTable[mActionIndex].fnx)(self) == 0)
                        return 0;
                    mNextPreflightAction = SimLibElapsedTime + PreFlightTable[mActionIndex].timedelay * CampaignSeconds;
                    break;

                case PreflightActions.CANOPY:
                    if (af.canopyState)
                    {
                        af.CanopyToggle();
                        mNextPreflightAction = SimLibElapsedTime + PreFlightTable[mActionIndex].timedelay * CampaignSeconds;
                    }
                    break;
                case PreflightActions.FUEL1:
                    if (af.IsEngineFlag(AirframeClass.MasterFuelOff))
                    {
                        af.ClearEngineFlag(AirframeClass.MasterFuelOff);
                        mNextPreflightAction = SimLibElapsedTime + PreFlightTable[mActionIndex].timedelay * CampaignSeconds;
                    }
                    break;
                case PreflightActions.MAINPOWER:
                    if (self.MainPower() != AircraftClass.MainPowerMain)
                    {
                        self.IncMainPower();
                        mNextPreflightAction = SimLibElapsedTime + PreFlightTable[mActionIndex].timedelay * CampaignSeconds;
                        return 0; //keep clicking til we get to the right state
                    }
                    break;
                case PreflightActions.FUEL2:
                    af.SetFuelPump(AirframeClass.FP_NORM);
                    af.SetFuelSwitch(AirframeClass.FS_NORM);
                    af.ClearEngineFlag(AirframeClass.WingFirst);
                    af.SetAirSource(AirframeClass.AS_NORM);
                    mNextPreflightAction = SimLibElapsedTime + PreFlightTable[mActionIndex].timedelay * CampaignSeconds;
                    break;
                case PreflightActions.RALTON:
                    if (self.RALTStatus == AircraftClass.RaltStatus.ROFF)
                    {
                        self.RaltOn();
                        mNextPreflightAction = SimLibElapsedTime + PreFlightTable[mActionIndex].timedelay * CampaignSeconds;
                    }
                    break;
                case PreflightActions.AFFLAGS:
                    if (!af.IsSet(PreFlightTable[mActionIndex].data))
                    {
                        af.SetFlag(PreFlightTable[mActionIndex].data);
                        mNextPreflightAction = SimLibElapsedTime + PreFlightTable[mActionIndex].timedelay * CampaignSeconds;
                    }
                    break;
                case PreflightActions.POWERON:
                    if (!self.PowerSwitchOn((AircraftClass.AvionicsPowerFlags)PreFlightTable[mActionIndex].data))
                    {
                        self.PowerOn((AircraftClass.AvionicsPowerFlags)PreFlightTable[mActionIndex].data);
                        //MI additional check for HUD, now that the dial indicates the status
                        if (PreFlightTable[mActionIndex].data == AircraftClass.HUDPower)
                        {
                            if (TheHud && self == SimDriver.playerEntity)
                                TheHud.SymWheelPos = 1.0F;
                        }
                        mNextPreflightAction = SimLibElapsedTime + PreFlightTable[mActionIndex].timedelay * CampaignSeconds;
                    }
                    break;
                case PreflightActions.RADAR:
                    {
                        RadarClass* theRadar = (RadarClass*)FindSensor(self, SensorClass.Radar);
                        if (theRadar)
                            theRadar.SetMode(RadarClass.AA);
                        mNextPreflightAction = SimLibElapsedTime + PreFlightTable[mActionIndex].timedelay * CampaignSeconds;
                    }
                    break;
                case PreflightActions.SEAT:
                    {
                        self.SeatArmed = true;
                        mNextPreflightAction = SimLibElapsedTime + PreFlightTable[mActionIndex].timedelay * CampaignSeconds;
                    }
                    break;
                case PreflightActions.EWSPGM:
                    {
                        self.SetPGM(AircraftClass.EWSPGMSwitch.Man);
                        mNextPreflightAction = SimLibElapsedTime + PreFlightTable[mActionIndex].timedelay * CampaignSeconds;
                    }
                    break;
                case PreflightActions.MASTERARM:
                    {
                        self.Sms.SetMasterArm(SMSBaseClass.Arm);
                        mNextPreflightAction = SimLibElapsedTime + PreFlightTable[mActionIndex].timedelay * CampaignSeconds;
                    }
                    break;
                case PreflightActions.EXTLON:
                    if (!self.ExtlState((AircraftClass.ExtlLightFlags)PreFlightTable[mActionIndex].data))
                    {
                        self.ExtlOn((AircraftClass.ExtlLightFlags)PreFlightTable[mActionIndex].data);
                        mNextPreflightAction = SimLibElapsedTime + PreFlightTable[mActionIndex].timedelay * CampaignSeconds;
                    }
                    break;
                case PreflightActions.INS:
                    //turn on aligning flag
                    if (PreFlightTable[mActionIndex].data == AircraftClass.INS_AlignNorm)
                    {
                        self.INSOff(AircraftClass.INS_PowerOff);
                        if (self == SimDriver.playerEntity && OTWDriver.pCockpitManager)
                        {
                            self.SwitchINSToAlign();
                            self.INSAlignmentTimer = 0.0F;
                            self.HasAligned = false;
                            //Set the UFC
                            OTWDriver.pCockpitManager.mpIcp.ClearStrings();
                            OTWDriver.pCockpitManager.mpIcp.LeaveCNI();
                            OTWDriver.pCockpitManager.mpIcp.SetICPFlag(ICPClass.MODE_LIST);
                            OTWDriver.pCockpitManager.mpIcp.SetICPSecondaryMode(23);	//SIX Button, INS Page
                        }
                    }
                    self.INSOn((AircraftClass.INSAlignFlags)PreFlightTable[mActionIndex].data);
                    mNextPreflightAction = SimLibElapsedTime + PreFlightTable[mActionIndex].timedelay * CampaignSeconds;

                    if (self.INSState(AircraftClass.INS_Aligned) && self.INSState(AircraftClass.INS_AlignNorm))
                        self.INSOff(AircraftClass.INS_AlignNorm);

                    if (self == SimDriver.playerEntity && OTWDriver.pCockpitManager &&
                        PreFlightTable[mActionIndex].data == AircraftClass.INS_Nav)
                    {
                        //CNI page
                        OTWDriver.pCockpitManager.mpIcp.ChangeToCNI();
                        //Mark us as aligned
                        self.SwitchINSToNav();
                    }
                    break;
                case PreflightActions.VOLUME:
                    if (PreFlightTable[mActionIndex].data == 1)
                        self.MissileVolume = 0;
                    else if (PreFlightTable[mActionIndex].data == 2)
                        self.ThreatVolume = 0;
                    break;
                case PreflightActions.FLAPS:
                    af.TEFTakeoff();
                    af.LEFTakeoff();
                    break;
                default:
                    ShiWarning("Bad Preflight Table");
                    break;
            }
            // force switch positions.
            if (self == SimDriver.playerEntity && OTWDriver.pCockpitManager)
                OTWDriver.pCockpitManager.InitialiseInstruments();

            mActionIndex++;
            if (mActionIndex < MAX_PF_ACTIONS)
                return 0;
            mActionIndex = 0;
            mNextPreflightAction = 0;
            return 1;
        }

        protected void QuickPreFlight()
        {
            Debug.Assert(af != null && self != null);
            self.PreFlight();
        }


        protected void TaxiBack(ObjectiveClass* Airbase)
        {
            if (atcstatus != lTaxiOff)
            {
                atcstatus = lTaxiOff;
                SendATCMsg(noATC);
            }

            switch (PtDataTable[curTaxiPoint].type)
            {
                case TakeRunwayPt:
                case TakeoffPt:
                case RunwayPt:
                    if (curTaxiPoint)
                    {
                        curTaxiPoint = GetNextTaxiPt(curTaxiPoint);
                        TranslatePointData(Airbase, curTaxiPoint, &trackX, &trackY);
                        OffsetTrackPoint(120.0F, offRight);
                    }
                    break;
            }

            if (fabs(trackX - af.x) < TAXI_CHECK_DIST && fabs(trackY - af.y) < TAXI_CHECK_DIST)
            {
                curTaxiPoint = GetNextTaxiPt(curTaxiPoint);
                if (curTaxiPoint)
                {
                    TranslatePointData(Airbase, curTaxiPoint, &trackX, &trackY);
                    OffsetTrackPoint(120.0F, offRight);
                }
            }

            if (self != SimDriver.playerEntity)
            {
                if (!curTaxiPoint || PtDataTable[curTaxiPoint].flags & 2)
                {
                    RegroupAircraft(self);
                }
            }

            if (CheckTaxiTrackPoint())
            {
                OffsetTrackPoint(80.0F, offRight);
            }
            CalculateTaxiSpeed(10.0F);
            SimpleGroundTrack(desiredSpeed);
        }

        protected SimBaseClass* CheckTaxiTrackPoint()
        {
            return CheckPoint(trackX, trackY);
        }


        protected SimBaseClass* CheckPoint(float x, float y)
        {
            return CheckTaxiPointGlobal(self, x, y);
        }

        protected bool CheckTaxiCollision()
        {
            Tpoint org, pos, vec;
            float rangeSquare;
            SimBaseClass* testObject;
            CampBaseClass* campBase;
#if VU_GRID_TREE_Y_MAJOR
	VuGridIterator gridIt(RealUnitProxList, af.y, af.x, Phyconst.NM_TO_FT * 3.0F);
#else
            VuGridIterator gridIt = new VuGridIterator(RealUnitProxList, af.x, af.y, Phyconst.NM_TO_FT * 3.0F);
#endif

            // get the 1st objective that contains the bomb
            campBase = (CampBaseClass*)gridIt.GetFirst();

            // main loop through campaign units
            while (campBase)
            {
                // skip campaign unit if no sim components
                if (!campBase.GetComponents())
                {
                    campBase = (CampBaseClass*)gridIt.GetNext();
                    continue;
                }
                // loop thru each element in the objective
                VuListIterator unitWalker = new VuListIterator(campBase.GetComponents());
                testObject = (SimBaseClass*)unitWalker.GetFirst();

                while (testObject)
                {
                    // ignore objects under these conditions:
                    //		Ourself
                    //		Not on ground
                    //		no drawpointer
                    if (!testObject.OnGround() ||
                         testObject == self ||
                         !testObject.drawPointer)
                    {
                        testObject = (SimBaseClass*)unitWalker.GetNext();
                        continue;
                    }

                    // range from tracking point to object
                    pos.x = testObject.XPos() - af.x;
                    pos.y = testObject.YPos() - af.y;
                    rangeSquare = pos.x * pos.x + pos.y * pos.y;

                    // if object is greater than given range don't check
                    // also, perhaps a degenerate case, if too close and overlapping
                    if (rangeSquare > 80.0f * 80.0f || rangeSquare < 10.0f * 10.0f)
                    {
                        testObject = (SimBaseClass*)unitWalker.GetNext();
                        continue;
                    }

                    // origin of ray
                    rangeSquare = self.drawPointer.Radius();
                    org.x = af.x + self.dmx[0][0] * rangeSquare * 1.1f;
                    org.y = af.y + self.dmx[0][1] * rangeSquare * 1.1f;
                    org.z = af.z + self.dmx[0][2] * rangeSquare * 1.1f;

                    // vector of ray
                    vec.x = self.dmx[0][0] * 80.0f;
                    vec.y = self.dmx[0][1] * 80.0f;
                    vec.z = self.dmx[0][2] * 80.0f;

                    // do ray, box intersection test
                    if (testObject.drawPointer.GetRayHit(&org, &vec, &pos, 1.0f))
                    {
                        return true;
                    }

                    testObject = (SimBaseClass*)unitWalker.GetNext();
                }

                // get the next objective that contains the bomb
                campBase = (CampBaseClass*)gridIt.GetNext();

            } // end objective loop

            return false;
        }

        protected bool SimpleGroundTrack(float speed)
        {
            float tmpX, tmpY;
            float rx, ry;
            float az, azErr;
            float stickError;
            SimBaseClass* testObject;
            int numOnLeft, numOnRight;
            float myRad, testRad, xft, yft, dist;
            float minAz = 1000.0f;

            af.ClearFlag(AirframeClass.WheelBrakes);
            xft = trackX - af.x;
            yft = trackY - af.y;
            // get relative position and az
            rx = self.dmx[0][0] * xft + self.dmx[0][1] * yft;
            ry = self.dmx[1][0] * xft + self.dmx[1][1] * yft;

            if (self == SimDriver.playerEntity && IsSetATC(StopPlane))
            {
                // call simple track to set the stick
                if (rx < 10.0F)
                {
                    int taxiPoint = GetPrevPtLoop(curTaxiPoint);
                    ObjectiveClass* Airbase = (ObjectiveClass*)vuDatabase.Find(airbase);

                    TranslatePointData(Airbase, taxiPoint, &tmpX, &tmpY);
                    xft = tmpX - af.x;
                    yft = tmpY - af.y;
                    // get relative position and az
                    rx = self.dmx[0][0] * xft + self.dmx[0][1] * yft;
                    ry = self.dmx[1][0] * xft + self.dmx[1][1] * yft;
                    if (gameCompressionRatio)
                        // JB 020315 Why divide by gameCompressionRation?  It screws up taxing for one thing.
                        rStick = SimpleTrackAzimuth(rx, ry, self.Vt());///gameCompressionRatio;
                    else
                        rStick = 0.0f;
                    pStick = 0.0F;
                    throtl = 0.0F;
                    af.SetFlag(AirframeClass.WheelBrakes);
                }
                else
                    TrackPoint(0.0f, 0.0F);

                if (self.af.vt < 1.0F)
                {
                    ClearATCFlag(StopPlane);
                    af.ClearFlag(AirframeClass.WheelBrakes);
                    self.SetAutopilot(AircraftClass.APOff);
                }
                return false;
            }

            if (atcstatus == tTakeoff)
            {
                // once we're taking off just do it....
                //aim for a five degree climb	
                af.LEFTakeoff();
                af.TEFTakeoff();
                rStick = SimpleTrackAzimuth(rx + 1000.0F, ry, self.Vt());

                pStick = 5 * DTR;
                throtl = 1.5f;
            }
            else
            {
                // cheat a bit so we don't chase around in a circle
                // if we're getting close to our track point, slow and
                // rotate towards it

                dist = xft * xft + yft * yft;
                if (dist < TAXI_CHECK_DIST * TAXI_CHECK_DIST * 4.0F)
                {
                    az = (float)atan2(ry, rx);
                    if (fabs(az) > 30.0f * DTR)
                        speed *= 0.5f;
                }

                // call simple track to set the stick
                TrackPoint(0.0f, speed);
            }

            if (speed == 0.0f)
            {
                // if no speed we're done
                if (rx < 10.0F)
                {
                    int taxiPoint = GetPrevPtLoop(curTaxiPoint);
                    ObjectiveClass* Airbase = (ObjectiveClass*)vuDatabase.Find(airbase);

                    TranslatePointData(Airbase, taxiPoint, &tmpX, &tmpY);
                    xft = tmpX - af.x;
                    yft = tmpY - af.y;
                    // get relative position and az
                    rx = self.dmx[0][0] * xft + self.dmx[0][1] * yft;
                    ry = self.dmx[1][0] * xft + self.dmx[1][1] * yft;
                    if (gameCompressionRatio)
                        // JB 020315 Why divide by gameCompressionRation?  It screws up taxing for one thing.
                        rStick = SimpleTrackAzimuth(rx, ry, self.Vt());///gameCompressionRatio;
                    else
                        rStick = 0.0f;
                    pStick = 0.0F;
                }
                return false;
            }

            if (!self.OnGround())
                return false;

            // init the stick error
            stickError = 0.0f;
            numOnLeft = 0;
            numOnRight = 0;

            if (self.drawPointer)
                myRad = self.drawPointer.Radius();
            else
                myRad = 40.0f;

            // loop thru all sim objects
            VuListIterator unitWalker = new VuListIterator(SimDriver.objectList);
            testObject = (SimBaseClass*)unitWalker.GetFirst();

            while (testObject)
            {
                // ignore objects under these conditions:
                //		Ourself
                //		Not on ground
                if (!testObject.OnGround() ||
                     testObject == self)
                {
                    testObject = (SimBaseClass*)unitWalker.GetNext();
                    continue;
                }

                // range from us to object
                tmpX = testObject.XPos() - af.x;
                tmpY = testObject.YPos() - af.y;

                if (testObject.drawPointer)
                    testRad = testObject.drawPointer.Radius() + myRad;
                else
                    testRad = 40.0f + myRad;

                dist = (float)sqrt(tmpX * tmpX + tmpY * tmpY);
                float range = dist - testRad - MAX_RANGE_COLL;

                //rangeSquare = tmpX*tmpX + tmpY*tmpY - testRad * testRad - MAX_RANGE_SQ;

                // if object is greater than 2 x max range continue to next
                //if ( rangeSquare > MAX_RANGE_SQ )
                if (range > MAX_RANGE_COLL)
                {
                    testObject = (SimBaseClass*)unitWalker.GetNext();
                    continue;
                }

                // get relative position and az
                rx = self.dmx[0][0] * tmpX + self.dmx[0][1] * tmpY;
                ry = self.dmx[1][0] * tmpX + self.dmx[1][1] * tmpY;

                az = (float)atan2(ry, rx);

                // reject anything more than 90 deg off our nose
                if (fabs(az) > MAX_AZ)
                {
                    testObject = (SimBaseClass*)unitWalker.GetNext();
                    continue;
                }

                if (rx > 0.0F && fabs(ry) > testRad && range < 0.0F &&
                    testObject.GetCampaignObject() == self.GetCampaignObject() &&
                    self.vehicleInUnit > ((AircraftClass*)testObject).vehicleInUnit)
                {
                    xft = trackX - af.x;
                    yft = trackY - af.y;
                    // get relative position and az
                    rx = self.dmx[0][0] * xft + self.dmx[0][1] * yft;
                    ry = self.dmx[1][0] * xft + self.dmx[1][1] * yft;
                    if (rx < 10.0F)
                    {
                        int taxiPoint = GetPrevPtLoop(curTaxiPoint);
                        ObjectiveClass* Airbase = (ObjectiveClass*)vuDatabase.Find(airbase);

                        TranslatePointData(Airbase, taxiPoint, &tmpX, &tmpY);
                        xft = tmpX - af.x;
                        yft = tmpY - af.y;
                        // get relative position and az
                        rx = self.dmx[0][0] * xft + self.dmx[0][1] * yft;
                        ry = self.dmx[1][0] * xft + self.dmx[1][1] * yft;
                        if (gameCompressionRatio)
                            // JB 020315 Why divide by gameCompressionRation?  It screws up taxing for one thing.
                            rStick = SimpleTrackAzimuth(rx, ry, self.Vt());///gameCompressionRatio;
                        else
                            rStick = 0.0f;
                        pStick = 0.0F;
                        throtl = SimpleGroundTrackSpeed(0.0F);
                    }
                    else
                        TrackPoint(0.0f, speed);
                    return true;
                }

                if (fabs(az) < fabs(minAz))
                {
                    minAz = az;
                }
                // have we reached a situation where it's impossible to
                // move forward without colliding?
                if (fabs(az) < 80.0F * DTR && rx > 5.0f && rx < testRad * 1.5F && fabs(ry) < testRad * 1.25F)
                {
                    // count the number of blocks to our left and right
                    // to be used later
                    if (ry > 0.0f)
                        numOnRight++;
                    else
                        numOnLeft++;

                    testObject = (SimBaseClass*)unitWalker.GetNext();
                    continue;
                }

                // OK, we've got an object in front of our 3-6 line and
                // within range.  The potential field will work by deflecting
                // our nose (rstick).  The closer the object and the more towards
                // our nose, the stronger is the deflection


                if (az > 0.0f)
                    azErr = -1.0F + ry / dist;
                else if (az < 0.0f)
                    azErr = 1.0F + ry / dist;
                else if (rStick > 0.0f)
                    azErr = 1.0f;
                else
                    azErr = -1.0f;

                // this deflection is now modulated by the proximity -- the closer
                // the stronger the force
                // weight the range higher
                azErr *= (MAX_RANGE_COLL - range) / (MAX_RANGE_COLL * 0.4F);

                // now accumulate the stick error
                stickError += azErr;


                testObject = (SimBaseClass*)unitWalker.GetNext();
            }

            // test blockages directly in front
            // cheat: if we get stuck just rotate in place
            if ((numOnLeft || numOnRight) && fabs(minAz) > 70.0f * DTR)
            {
                rStick = 0.0f;
                pStick = 0.0f;
                throtl *= 0.5f;
                tiebreaker = 0;
            }
            else if (tiebreaker > 10)
            {
                af.sigma -= 10.0f * DTR * SimLibMajorFrameTime;
                af.SetFlag(AirframeClass.WheelBrakes);
                throtl = 0.0f;
                if (tiebreaker++ - 10 > rand() % 20)
                    tiebreaker = 0;
                return true;
            }
            else if (numOnLeft && numOnRight)
            {
                if (fabs(minAz) > 50.0f * DTR)
                {
                    rStick = 0.0f;
                    pStick = 0.0f;
                    throtl *= 0.5f;
                }
                else
                {
                    af.sigma -= 10.0f * DTR * SimLibMajorFrameTime;
                    af.SetFlag(AirframeClass.WheelBrakes);
                    throtl = 0.0f;
                }
                tiebreaker++;
                return true;
            }
            else if (numOnRight)
            {
                af.sigma -= 10.0f * DTR * SimLibMajorFrameTime;
                af.SetFlag(AirframeClass.WheelBrakes);
                throtl = 0.0f;
                tiebreaker++;
                return true;
            }
            else if (numOnLeft)
            {
                af.sigma += 10.0f * DTR * SimLibMajorFrameTime;
                af.SetFlag(AirframeClass.WheelBrakes);
                throtl = 0.0f;
                tiebreaker++;
                return true;
            }
            tiebreaker = 0;
            // we now apply the stick error to rstick
            // make sure we clamp rstick
            rStick += stickError;
            if (rStick > 1.0f)
                rStick = 1.0f;
            else if (rStick < -1.0f)
                rStick = -1.0f;

            // readjust throttle if our stick error is large
            if (fabs(stickError) > 0.5f)
            {
                throtl = SimpleGroundTrackSpeed(speed * 0.75f);
            }

            return false;
        }

        protected void FindParkingSpot(ObjectiveClass* Airbase)
        {
            int bestpt = BestParkSpot(Airbase);
            int pt = GetNextPtLoop(curTaxiPoint);
            while (PtDataTable[pt].flags != PT_LAST)
            {
                if (pt == bestpt)
                { // next taxi point is our favoured parking spot
                    curTaxiPoint = pt;
                    return;
                }
                else if (PtDataTable[pt].type == SmallParkPt || // this is not our aim, skip
                    PtDataTable[pt].type == LargeParkPt)
                {
                    pt = GetNextPtLoop(pt);
                }
                else
                {
                    curTaxiPoint = pt; // keep on trucking
                    return;
                }
            }
            curTaxiPoint = pt;
        }


        protected bool CloseToTrackPoint()
        {
            if (fabs(trackX - af.x) < TAXI_CHECK_DIST &&
                fabs(trackY - af.y) < TAXI_CHECK_DIST)
                return true;
            return false;
        }


        protected bool AtFinalTaxiPoint()
        {
            // final point is one of these
            if (PtDataTable[curTaxiPoint].type == SmallParkPt ||
                PtDataTable[curTaxiPoint].type == LargeParkPt ||
                PtDataTable[curTaxiPoint].flags == PT_LAST)
            {
                return CloseToTrackPoint();
            }
            return false;
        }


        protected int BestParkSpot(ObjectiveClass* Airbase)
        {
            float x, y;
            int pktype = af.GetParkType();
            int npt, pt = curTaxiPoint;
            while ((npt = GetNextParkTypePt(pt, pktype)) != 0) // find last possible parking spot
                pt = npt;
            if (PtDataTable[pt].type != pktype) // just in case we found none
                return 0;
            while (pt > curTaxiPoint)
            {
                TranslatePointData(Airbase, pt, &x, &y);
                if (CheckPoint(x, y) == null)
                { // this is good?
                    return pt;
                }
                pt = GetPrevParkTypePt(pt, pktype); // ok try another
            }
            return 0;
        }

        protected void FuelCheck();
        protected void IPCheck()
        {
            WayPointClass* tmpWaypoint = self.waypoint;
            float wpX, wpY, wpZ;
            float dX, dY;
            float rangeSq;
            short[] edata = new short[6];
            int response;

            // Only for the player's wingmen
            // JB 020315 All aircraft will now all check to see if they have passed the IP.  
            // Only checking for the IP if the lead is a player is silly.  
            // Things depend on the ReachedIP flag being set properly.
            //if (flightLead && flightLead.IsSetFlag(MOTION_OWNSHIP)) 
            {
                // Periodically check for IP and if so, ask for permission to engage
                if (!IsSetATC(ReachedIP))
                {
                    // Find the IP waypoint
                    while (tmpWaypoint)
                    {
                        if (tmpWaypoint.GetWPFlags() & WPF_TARGET)
                            break;
                        tmpWaypoint = tmpWaypoint.GetNextWP();
                    }

                    // From the target, find the IP
                    if (tmpWaypoint)
                        tmpWaypoint = tmpWaypoint.GetPrevWP();

                    // Have an IP
                    if (tmpWaypoint)
                    {
                        tmpWaypoint.GetLocation(&wpX, &wpY, &wpZ);
                        dX = self.XPos() - wpX;
                        dY = self.YPos() - wpY;

                        // Original code was looking for the SLANT distance, I'm not...
                        // Check for within 5 NM of IP
                        rangeSq = dX * dX + dY * dY;
                        if (rangeSq < (5.0F * Phyconst.NM_TO_FT) * (5.0F * Phyconst.NM_TO_FT))
                        {
                            // In Range and getting closer?
                            if (rangeSq < rangeToIP)
                            {
                                // Yes, update our range to IP
                                rangeToIP = rangeSq;
                            }
                            else
                            {
                                // The range is INCREASING so we assume (may be wronly if we turned away from the IP waypoint) we've reached it, say so and wait for a target
                                SetATCFlag(ReachedIP);
                                // JB 020315 Only set the WaitForTarget flag if the lead is a player.
                                if (flightLead && flightLead.IsSetFlag(MOTION_OWNSHIP))
                                    SetATCFlag(WaitForTarget);
                            }
                        }
                    }
                }
                // We have reached our IP waypoint, are we waiting for a target?
                else if (IsSetATC(WaitForTarget))
                {
                    // Yes, ask for permission if we are an incomplete AGMission that doesn't already have a designated target and it holds a ground target
                    if (missionClass == AGMission && !missionComplete && !mpActionFlags[AI_ENGAGE_TARGET] && groundTargetPtr)
                    {
                        // Flag we are waiting for permission and we have a ground target to shoot at
                        SetATCFlag(WaitingPermission);
                        ClearATCFlag(WaitForTarget);

                        // Ask for permission to engage
                        SetATCFlag(AskedToEngage);
                        edata[0] = ((FlightClass*)self.GetCampaignObject()).callsign_id;
                        edata[1] = (((FlightClass*)self.GetCampaignObject()).callsign_num - 1) * 4 + isWing + 1;
                        response = rcREQUESTTOENGAGE;
                        AiMakeRadioResponse(self, response, edata);
                    }
                }
            }
        }



        // Targeting
        protected float ataDot;
        protected float lastAta;
        protected SimObjectType* targetList;

        protected virtual void SetTarget(SimObjectType* newTarget)
        {
            short[] edata = new short[6];
            int response, navAngle;
            float rz;

            // 2000-10-09 REMOVED BY S.G. BECAUSE OF THIS, AI WON'T SWITCH TARGET WHEN ASKED
            // No targeting when on ground attack run(i.e. After IP)
            // 2001-05-05 MODIFIED BY S.G. LETS TRY SOMETHING ELSE INSTEAD
            // if (agDoctrine != AGD_NONE && !madeAGPass)
            if (newTarget && newTarget.BaseData().GetTeam() == self.GetTeam() && (agDoctrine != AGD_NONE || missionComplete))
            {
                return;
            }

            // 2000-09-21 ADDED BY S.G. DON'T CHANGE TARGET IF WE ARE SUPPORTING OUR SARH MISSILE DAMN IT!
            // TODO: Check if 'HandleThreat' is not screwing stuff since it calls WVREngage DIRECTLY
            if (newTarget && // Assigning a target
                newTarget != targetPtr && // It's a new target
                missileFiredEntity && // we launched a missile already
                !((SimWeaponClass*)missileFiredEntity).IsDead() && // it's not dead
                ((SimWeaponClass*)missileFiredEntity).targetPtr && // it's still homing to a target
                ((SimWeaponClass*)missileFiredEntity).sensorArray && // the missile is local (it has a sensor array)
                ((SimWeaponClass*)missileFiredEntity).sensorArray[0].Type() == SensorClass.RadarHoming) // It's still being guided by us
                return; // That's it, don't change target (support your missile)
            // END OF ADDED SECTION

            // Tell someone we're enaging/want to engage an air target of our own volition
            if (newTarget && // Assigning a target
                newTarget != targetPtr && // It's a new target
               !newTarget.BaseData().OnGround() && // It's not on the ground
                (!mpActionFlags[AI_ENGAGE_TARGET] && missionClass == AAMission || missionComplete) && // We're not busy doing A/G stuff
                newTarget != threatPtr && // It's not a threat we're reacting to
                isWing && // We're a wingy
                mDesignatedObject == FalconNullId && // We're not being directed
                !self.OnGround()) // We're in the air
            {

                // F4Assert (!newTarget.BaseData().IsHelicopter()); // 2002-03-05 Choppers are fare game now under some conditions

                // Ask for permission?
                // 2000-09-25 MODIFIED BY S.G. WHY ASK PERMISSION IF WE HAVE WEAPON FREE?
                if (!mpActionFlags[AI_ENGAGE_TARGET] && mWeaponsAction == AI_WEAPONS_HOLD)
                {
                    if (!IsSetATC(AskedToEngage))
                    {
                        SetATCFlag(AskedToEngage);
                        edata[0] = ((FlightClass*)self.GetCampaignObject()).callsign_id;
                        edata[1] = (((FlightClass*)self.GetCampaignObject()).callsign_num - 1) * 4 + isWing;
                        response = rcREQUESTTOENGAGE;
                        AiMakeRadioResponse(self, response, edata);
                        missileShotTimer = SimLibElapsedTime + FloatToInt32(30.0F * SEC_TO_MSEC);
                    }
                    else if (SimLibElapsedTime > missileShotTimer)
                    {
                        // We've waited long enough, go kill something
                        ClearATCFlag(AskedToEngage);
                        missileShotTimer = 0;
                        AiGoShooter();

                    }
                    return;
                }
                else if (newTarget && (targetPtr == null || (newTarget.BaseData() != targetPtr.BaseData())) && newTarget.localData.range < 2.0F * Phyconst.NM_TO_FT)
                {
                    ClearATCFlag(AskedToEngage);
                    // 2000-09-25 ADDED BY S.G. NEED TO FORCE THE AI TO SHOOT RIGHT AWAY
                    missileShotTimer = 0;
                    // END OF ADDED SECTION
                    if (PlayerOptions.BullseyeOn())
                    {
                        response = rcENGAGINGA;
                    }
                    else
                    {
                        response = rcENGAGINGB;
                    }

                    edata[0] = ((FlightClass*)self.GetCampaignObject()).callsign_id;
                    edata[1] = (((FlightClass*)self.GetCampaignObject()).callsign_num - 1) * 4 + isWing;
                    edata[2] = 2 * (newTarget.BaseData().Type() - VU_LAST_ENTITY_TYPE);
                    edata[3] = (short)SimToGrid(newTarget.BaseData().XPos());
                    edata[4] = (short)SimToGrid(newTarget.BaseData().YPos());
                    edata[5] = (short)newTarget.BaseData().ZPos();
                    // 2000-09-25 MODIFIED BY S.G. SO AI SAY WHAT IT IS SUPPOSED TO SAY INSTEAD OF 'HELP!'
                    //			AiMakeRadioResponse( self, rcHELPNOW, edata );
                    AiMakeRadioResponse(self, response, edata);
                }
                else
                {
                    // 2000-09-25 ADDED BY S.G. NEED TO FORCE THE AI TO SHOOT RIGHT AWAY
                    missileShotTimer = 0;
                    // END OF ADDED SECTION
                    edata[0] = 2 * (newTarget.BaseData().Type() - VU_LAST_ENTITY_TYPE);
                    navAngle = FloatToInt32(RTD * TargetAz(self, newTarget.BaseData()));
                    if (navAngle < 0)
                    {
                        navAngle = 360 + navAngle;
                    }

                    edata[1] = navAngle / 30;									// scale compass angle for radio eData
                    if (edata[1] >= 12)
                    {
                        edata[1] = 0;
                    }

                    rz = newTarget.BaseData().ZPos() - self.ZPos();

                    if (rz < 300.0F && rz > -300.0F)
                    {							// check relative alt and select correct frag
                        edata[2] = 1;
                    }
                    else if (rz < -300.0F && rz > -1000.0F)
                    {
                        edata[2] = 2;
                    }
                    else if (rz < -1000.0F)
                    {
                        edata[2] = 3;
                    }
                    else
                    {
                        edata[2] = 0;
                    }
                    response = rcENGAGINGC;
                    // 2000-09-25 MODIFIED BY S.G. SO AI SAY WHAT IT IS SUPPOSED TO SAY INSTEAD OF 'HELP!'
                    //			AiMakeRadioResponse( self, rcHELPNOW, edata );
                    AiMakeRadioResponse(self, response, edata);
                }
            }

            // edg: don't set ground targets via this mechanism, ground targeting
            // should always use groundTargetPtr
            // We now divert to SetGroundTargetPtr if target on ground.  Potentially
            // if don't do something with the target is may cause a memory leak of
            // SimObjectTypes.
            if (newTarget && newTarget.BaseData().OnGround())
            {
                // presumably if we're setting to a new target here, we want to
                // clear the air target (?)
                ClearTarget();
                //		Debug.Assert(curMode != GunsEngageMode);
                SetGroundTargetPtr(newTarget);
                return;
            }

            BaseBrain.SetTarget(newTarget);

            // Make sure the radar is pointed at the desired target
            // Special case, people w/ heaters and an IRST and sometimes ACE level w/ heaters
            RadarClass* theRadar = (RadarClass*)FindSensor(self, SensorClass.Radar);
            if (theRadar)
            {
                if (SkillLevel() < 3)
                {
                    theRadar.SetDesiredTarget(newTarget);
                }
                else
                {
                    IrstClass* theIrst = (IrstClass*)FindSensor(self, SensorClass.IRST);

                    if (!theIrst)
                    {
                        theRadar.SetDesiredTarget(newTarget);
                    }
                    else if (!curMissile || (curMissile.sensorArray && curMissile.sensorArray[0].Type() != SensorClass.IRST))
                    {
                        theRadar.SetDesiredTarget(newTarget);
                    }
                }
            }

            if (targetPtr)
            {
                // Get all our sensors tracking this guy
                for (int i = 0; i < self.numSensors; i++)
                {
                    Debug.Assert(self.sensorArray[i]);
                    if (self.sensorArray[i].Type() != SensorClass.TargetingPod)
                        self.sensorArray[i].SetDesiredTarget(targetPtr);
                }
            }
            //edg: don't we want to clear sensor targets when no target?!
            else
            {
                for (int i = 0; i < self.numSensors; i++)
                {
                    Debug.Assert(self.sensorArray[i]);
                    self.sensorArray[i].ClearSensorTarget();
                }
            }
        }
        protected void DoTargeting();
        protected SimObjectType* InsertIntoTargetList(SimObjectType* root, SimObjectType* newObj);


        // Threat Avoid
        protected VU_ID missileDefeatId;
        protected int missileFindDragPt;
        protected int missileFinishedBeam;
        protected int missileShouldDrag;
        protected float missileDefeatTtgo;

        // Missile Engage
        protected MissileClass* curMissile;
        protected int curMissileStation;
        protected int curMissileNum;
        // 2000-11-17 ADDED BY S.G. I NEED THIS TWO VARIABLE IN AIRCRAFT CLASS TO STORE THE LATEST MISSILE LAUNCHED. SINCE THEY'RE NOT USED, I'll RENAME THEM TO SOMETHING THAT MAKE MORE SENSE. 

        public VuEntity* missileFiredEntity;
        public VU_TIME missileFiredTime;
        // S.G. BACK TO PROTECTED
        //MI taking this opportunity....
        public float destRoll, destPitch, currAlt;

        // 2001-08-31 ADDED BY S.G. NEED TO KNOW THE LAST TWO GROUND TARGET AN AI TARGETED SO OTHER AI IN THE FLIGHT CAN SKIP THEM
        protected SimBaseClass[] gndTargetHistory = new SimBaseClass[2];
        // 2001-08-31 ADDED BY S.G. NEED TO KNOW THE LAST GROUND CAMPAIGN TARGET IT GOT
        protected CampBaseClass* lastGndCampTarget;

        protected float jammertime; //me123
        protected VU_TIME holdlongrangeshot;//me123
        protected VU_TIME missileShotTimer;
        protected float maxEngageRange;

        // 2002-02-24 added by MN do pullup for g_nPullupTime seconds before reevaluating
        protected VU_TIME pullupTimer;

        // used in actions.cpp, "AirbaseCheck" function
        protected int airbasediverted;
        protected VU_TIME nextFuelCheck;

        // Guns Engage
        protected VU_TIME jinkTime;
        protected VU_TIME waitingForShot;
        protected float ataddot, rangeddot, mnverTime;
        protected float newroll, pastAta, pastPstick, pastPipperAta;

        protected float GunsAutoTrack(float maxGs);
        protected void FineGunsTrack(float speed, float* lagAngle);
        protected void CoarseGunsTrack(float speed, float leadTof, float* newata);

        // Simple flight model
        protected enum SimpleTrackMode { SimpleTrackDist, SimpleTrackSpd, SimpleTrackTanker };
        protected void SimpleTrack(SimpleTrackMode mode, float value)
        {
            float xft;
            float yft;
            float zft;
            float rx;
            float ry;
            float rz;

            if (!self.OnGround())
            {
                if (mode == SimpleTrackDist)
                {
                    CalculateRelativePos(&xft, &yft, &zft, &rx, &ry, &rz);

                    if (flightLead)
                    {
                        if (flightLead.LastUpdateTime() == vuxGameTime)
                        {
                            // Correct for different frame rates
                            xft -= flightLead.XDelta() * SimLibMajorFrameTime;
                            yft -= flightLead.YDelta() * SimLibMajorFrameTime;
                            //            xft += flightLead.XDelta()*SimLibLastMajorFrameTime;
                            //            yft += flightLead.YDelta()*SimLibLastMajorFrameTime;
                        }
                        SimpleTrackDistance(flightLead.Vt(), (float)sqrt(xft * xft + yft * yft));	// Get Leader's speed, relative position
                    }

                    rStick = SimpleTrackAzimuth(rx, ry, self.Vt());
                    pStick = SimpleTrackElevation(zft, 5000.0F);
                }
                else if (mode == SimpleTrackSpd)
                {
                    CalculateRelativePos(&xft, &yft, &zft, &rx, &ry, &rz);

                    SimpleTrackSpeed(value);											// value = desired speed

                    rStick = SimpleTrackAzimuth(rx + 1000.0F, ry, self.Vt());
                    pStick = SimpleTrackElevation(zft, 5000.0F);

                    // Capture heading first, then pitch
                    /*
                    ** edg: I don't know where this comes from but its totally porked
                    if (fabs(rStick) < 10.0F * DTR)
                       pStick = SimpleTrackElevation(zft, 10000.0F);
                    else
                       pStick = 0.0F;
                    */
                }
                else if (mode == SimpleTrackTanker)
                {

                    bool sticklimitation = false;

                    if (self.TBrain() && self.TBrain().ReachedFirstTrackPoint())
                        sticklimitation = true; // tanker is entering the track pattern

                    SimpleTrackSpeed(value);											// value = desired speed

                    CalculateRelativePos(&xft, &yft, &zft, &rx, &ry, &rz);

                    rStick = SimpleTrackAzimuth(rx + 1000.0F, ry, self.Vt());
                    if (sticklimitation)
                        rStick = min(g_fTankerRStick, max(rStick, -g_fTankerRStick));
                    else
                        rStick = min(0.2f, max(rStick, -0.2f));
                    //rStick = 0.0F;

                    //it's better if the tanker just goes to the desired altitude and stays there
                    //pStick = SimpleTrackElevation(trackZ - self.ZPos(), 100000.0F);
                    pStick = SimpleTrackElevation(trackZ - self.ZPos(), 10000.0F);
                    if (sticklimitation)
                        pStick = min(g_fTankerPStick, max(pStick, -g_fTankerPStick));
                    else
                        pStick = min(0.02f, max(pStick, -0.02f));
                }
            }
            else
            {
                CalculateRelativePos(&xft, &yft, &zft, &rx, &ry, &rz);

                if (gameCompressionRatio)
                    // JB 020315 Why divide by gameCompressionRation?  It screws up taxing for one thing.
                    rStick = SimpleTrackAzimuth(rx, ry, self.Vt());///gameCompressionRatio;
                else
                    rStick = 0.0f;
                pStick = SimpleTrackElevation(zft, 5000.0F);
                throtl = SimpleGroundTrackSpeed(value);
            }
            yPedal = 0.0f;

        }
        protected float SimpleTrackAzimuth(float rx, float ry, float p)
        {
            float azErr;


            // Clamp/limit for in air
            if (!self.OnGround())
            {
                // Calculate azimuth error
                azErr = (float)atan2(ry, rx);

                if (rx < 0.0F && (fabs(rx) < 3.0F * Phyconst.NM_TO_FT))
                {

                    // If our track point is to the right and behind us
                    if ((azErr > 0.0F) && (azErr > 90 * DTR))
                    {

                        // Change the azErr to be infront so that we don't backtrack
                        azErr = (180 * DTR) - azErr;
                    }
                    else if ((azErr < 0.0F) && (azErr < -90 * DTR))
                    {	// else to the left and behind

                        // Change the azErr to be infrom so that we don't backtrack
                        azErr = (-180 * DTR) - azErr;
                    }
                }

                // azErr *= 0.75F;  // smooth it out a little
                // edg: the azimuth error should really be "normalized" to some arc of
                // rotation.  Let's make this 180 deg

                // 2002-01-31 ADDED BY S.G. Lets limit the roll of an AI controlled plane when going from one waypoint to the next
                if (g_bPitchLimiterForAI &&														// We are asking to limit AI's turn agressiveness when flying to waypoints
                    !groundAvoidNeeded &&														// We're not trying to avoid the ground
                    (curMode == WingyMode || curMode == WaypointMode || curMode == RTBMode) &&  // Following waypoint or lead
                    agDoctrine == AGD_NONE &&													// Not doing any A2G attack (since it's in FollowWaypoints during that time)
                    (!flightLead || !flightLead.IsSetFlag(MOTION_OWNSHIP)))
                {					// The lead isn't the player (we must follow him whatever he does)

                    azErr /= ((180.0f) * DTR);

                    float maxRoll = self.af.GetRollLimitForAiInWP() * DTR;
                    float curRoll = (float)fabs(self.Roll()) * 0.85f; // Current Roll with some leadway
                    float scale;

                    // scale the role based on on the difference of roll from max
                    if (curRoll > maxRoll)
                        scale = 0.0;
                    else
                        scale = (float)sqrt((maxRoll - curRoll) / maxRoll); // Give more weights being toward zero

                    azErr *= scale;
                }
                else
                {
                    // END OF ADDED SECTION 2002-01-31
                    // 2001-10-25 CHANGED BACK by M.N. 40° caused planes to jink around when changing to another trackpoint
                    azErr /= ((180.0f) * DTR);//ME123 HOW ABOUT 40
                }
            }
            else
            {
                // Calculate azimuth error
                azErr = (float)atan2(ry, rx);
            }


            return max(-1.0F, min(1.0F, azErr));		// return the roll command
        }

        protected float SimpleTrackElevation(float zft, float scale)
        {
            float altErr;

            // JPO - don't mess with stuff if we're taking avoiding action
            if (groundAvoidNeeded || pullupTimer)
                return pStick;

            // Scale and limit the altitude error	
            altErr = -zft / scale; 					// Use 5000ft for error slope

            if (fabs(zft) > 2000.0f)
                altErr *= 0.5f;

            // limit climb based on airspeed
            if (-zft > 0.0f && af.vt < 600.0f * KNOTS_TO_FTPSEC)
            {
                altErr *= af.vt / (600.0f * KNOTS_TO_FTPSEC);
            }

            // 2002-01-30 ADDED BY S.G. Lets limit the pitch when we're at max climb angle
            if (g_bPitchLimiterForAI &&														// We are asking to limit AI's turn agressiveness when flying to waypoints
                !groundAvoidNeeded &&														// We're not trying to avoid the ground
                (curMode == WingyMode || curMode == WaypointMode || curMode == RTBMode) &&  // Following waypoint or lead
                agDoctrine == AGD_NONE &&													// Not doing any A2G attack (since it's in FollowWaypoints during that time)
                (!flightLead || !flightLead.IsSetFlag(MOTION_OWNSHIP) || ((AircraftClass*)flightLead).autopilotType == AircraftClass.CombatAP) &&	// The lead isn't the player (we must follow him whatever he does)
                altErr > 0.0f && self.Pitch() > 0.0f)
            {									// Pitching up and going up
                // First lets find the max pitch angle we can use
                float maxPitch = min(MAX_AF_PITCH, aeroDataset[self.af.VehicleIndex()].inputData[AeroDataSet.ThetaMax]);
                float curPitch = self.Pitch() * 0.85f; // Current pitch with some leadway

                float scale;

                // scale the pitch based on on the difference of pitch from max
                if (curPitch > maxPitch)
                    scale = 0.0f;
                else
                    scale = (float)sqrt((maxPitch - curPitch) / maxPitch); // Give more weights being toward zero
                altErr *= scale;

                // scale the pitch based on on the difference of speed
                float minVcas = aeroDataset[self.af.VehicleIndex()].inputData[AeroDataSet.MinVcas];
                float curKias = self.Kias(); // Current speed

                // If we're way above our best climb speed, lets pitch up a bit more to drain some speed and get some altitude
                if (minVcas * 1.9f < curKias && minVcas != 0.0f)
                    altErr *= curKias / (minVcas * 1.9f);
                else
                {
                    // If we are below the stall speed, wait until you get their before pitching up
                    if (minVcas > curKias)
                        scale = 0.0f;
                    // If we're above or best climb speed, adjust the pitch
                    else if (minVcas * 1.5f > curKias)
                        scale = (float)sqrt((curKias - minVcas) / (minVcas * 0.5f)); // Give more weights being toward zero
                    else
                        scale = 1.0f; // You're fine, for now
                    altErr *= scale;
                }
            }
            // END OF ADDED SECTION 2002-01-30

            return max(-0.5F, min(0.5F, altErr));		// Return the pitch command
        }

        protected float SimpleTrackDistance(float d, float rx)
        {
            float desiredClosure, actualClosure;

            //desiredClosure = 200.0F * rx/(1.0F*Phyconst.NM_TO_FT) - 200.0F;
            desiredClosure = 350.0F * rx / (1.0F * Phyconst.NM_TO_FT) - 350.0F; // JB 011016 Increase closure rates to adjust for the vt/kias bug fix below

            // get actual closure
            actualClosure = -(rx - velocitySlope) / SimLibLastMajorFrameTime;
            //me123 machhold needs knots ! chenged af.vt() to self.Kias
            MachHold(self.Kias() + desiredClosure - actualClosure, self.Kias(), false);
            velocitySlope = rx;
            return throtl;
        }
        protected float SimpleTrackSpeed(float v)
        {
            if (af.Qsom() * af.Cnalpha() < 1.55F && v < af.vt + 5.0F)
                v = af.vt /* * */ + 5.0F;	// 2001-10-27 M.N. removed "*", caused af.vt * 5
            // Lets Try MachHold on velocity
            MachHold(v * FTPSEC_TO_KNOTS, af.vt * FTPSEC_TO_KNOTS, false);

            return throtl;
        }

        protected float SimpleScaleThrottle(float v)
        {
            return 1.0F + (v - (450.0F * KNOTS_TO_FTPSEC)) / (450.0F * KNOTS_TO_FTPSEC);
        }
        protected void SimpleGoToCurrentWaypoint();
        protected void SimplePullUp()
        {

            pStick = 0.1F;
            throtl = 1.0F;
            rStick = 0.0F;
            yPedal = 0.0F;
        }

        protected float SimpleGroundTrackSpeed(float v)
        {
            if (af.vt > v + 2.0F)
                af.SetFlag(AirframeClass.WheelBrakes);
            else
                af.ClearFlag(AirframeClass.WheelBrakes);

            if (af.vt > 20.0F && v > 20.0F)
            {
                float eProp = v - af.vt;
                if (eProp >= 50.0F)
                {
                    autoThrottle = 1.5F;
                    throtl = 1.5F;                        /* burner     */
                }
                else if (eProp < -50.0F)
                {
                    autoThrottle = 0.0F;
                    throtl = 0.0F;                        /* idle and boards */
                }

                autoThrottle += eProp * 0.005F * SimLibMajorFrameTime;
                autoThrottle = max(0.0F, min(1.5F, autoThrottle));
                throtl = eProp * 0.005F + autoThrottle - af.vtDot * SimLibMajorFrameTime * 0.005F;
            }
            else
            {
                autoThrottle = throtl = af.CalcThrotlPos(v);
            }

            return throtl;
        }

        protected void CalculateRelativePos(float* xft, float* yft, float* zft, float* rx, float* ry, float* rz)
        {

            // Calculate relative position and range to track point
            *xft = trackX - self.XPos();
            *yft = trackY - self.YPos();
            *zft = trackZ - self.ZPos();

            *rx = self.dmx[0][0] * *xft + self.dmx[0][1] * *yft + self.dmx[0][2] * *zft;
            *ry = self.dmx[1][0] * *xft + self.dmx[1][1] * *yft + self.dmx[1][2] * *zft;
            *rz = self.dmx[2][0] * *xft + self.dmx[2][1] * *yft + self.dmx[2][2] * *zft;
        }

        protected VU_TIME mLastOrderTime;

        // Wing Radio Calls
        //TODO protected void RespondShortCallSign(int);

        // Controls for Simple flight Model
        protected int SelectFlightModel()
        {

            int simplifiedModel;

            // edg: if we're on the ground, we always want to be in simple
            // mode,  Otherwise we may cause a qnan crash in the flight model.
            // observed: simple model off, digi in separate mode and plane
            // was on ground -- x and y were qnan.  This doesn't fix the root of
            // the prob.
            if (self.OnGround())
                return SIMPLE_MODE_AF;


            // turn off simple mode if pilot has ejected or dying....
            if (self.IsSetFlag(PILOT_EJECTED) || self.pctStrength <= 0.0f)
                return SIMPLE_MODE_OFF;

            // override if we're deling with a threat
            if (threatPtr != null)
                return SIMPLE_MODE_OFF;

            switch (curMode)
            {

                case FollowOrdersMode:
                case WingyMode:
                    if (mpActionFlags[AI_USE_COMPLEX])
                        simplifiedModel = SIMPLE_MODE_OFF;
                    else
                        simplifiedModel = SIMPLE_MODE_AF;
                    break;

                case WaypointMode:
                case LoiterMode:
                case LandingMode:
                case TakeoffMode:
                    simplifiedModel = SIMPLE_MODE_AF;
                    break;
                case RefuelingMode:
                    // 2002-02-20 ADDED BY S.G. Have the AI use complex flight model if in refuel
                    if (g_bAIRefuelInComplexAF)
                        simplifiedModel = SIMPLE_MODE_OFF;
                    else
                        // END OF ADDED SECTION
                        simplifiedModel = SIMPLE_MODE_AF;
                    break;

                case RTBMode:
                case BVREngageMode:
                case GunsEngageMode:
                case MissileEngageMode:
                case GunsJinkMode:
                case CollisionAvoidMode:
                case OverBMode:
                case RoopMode:
                case WVREngageMode:
                default:
                    simplifiedModel = SIMPLE_MODE_OFF;
                    break;
            }



            return simplifiedModel;
        }


        // Decision Stuff
        protected void Actions();
        protected void DecisionLogic()
        {
            UnitClass* campUnit = (UnitClass*)self.GetCampaignObject();
            WayPointClass* dwp = null;
            CampBaseClass* diverttarget = null;
            SimBaseClass* airtarget = null;

            // Ground avoid check
            //if (!af.GetSimpleMode())
            if (curMode != LandingMode)
                GroundCheck();
            else
                groundAvoidNeeded = false;

            // MN Handle air divert waypoint here - set targetPtr to divert waypoint target
            // We have too many RequestIntercepts in CAMPAIGN code that sets flights to divert status while
            // there seems to be no code to really change a flight's mission to intercept a target


            if (g_bRequestHelp)
            {
                // 2002-01-14 MODIFIED BY S.G. pctStrength only belongs to SimBaseClass. Make sure it's one before checking
                //	if (airtargetPtr && (airtargetPtr.BaseData().IsDead() || airtargetPtr.BaseData().IsExploding() ||
                //		((SimBaseClass *)airtargetPtr.BaseData()).pctStrength <= 0.0f))
                if (airtargetPtr && (airtargetPtr.BaseData().IsDead() || airtargetPtr.BaseData().IsExploding() ||
                    (airtargetPtr.BaseData().IsSim() && ((SimBaseClass*)airtargetPtr.BaseData()).pctStrength <= 0.0f)))
                {
                    airtargetPtr.Release(SIM_OBJ_REF_ARGS);
                    airtargetPtr = null;
                    if (isWing)
                        AiGoCover();
                }
                Flight flight = ((FlightClass*)campUnit);
                dwp = flight.GetOverrideWP();
                // only if we're not threatened...
                if (threatPtr == null && dwp && (dwp.GetWPFlags() & WPF_REQHELP)) // we've a divert waypoint from a help request
                {
                    diverttarget = dwp.GetWPTarget();
                    if (diverttarget && diverttarget.IsFlight())
                    {
                        airtarget = FindSimAirTarget((CampBaseClass*)diverttarget, ((CampBaseClass*)diverttarget).NumberOfComponents(), 0);
                        if (!airtarget) // We've all targets assigned now, clear the divert waypoint
                            flight.SetOverrideWP(null);
                        if (airtarget) // it's a new one no other flight member has chosen yet
                        {
                            if (airtargetPtr != null)
                            {
                                // release existing target data if different object
                                if (airtargetPtr.BaseData() != airtarget)
                                {
                                    airtargetPtr.Release(SIM_OBJ_REF_ARGS);
                                    airtargetPtr = null;
                                }
                                else
                                {
                                    // already targeting this object
                                    return;
                                }
                            }
#if DEBUG
                            airtargetPtr = new SimObjectType(OBJ_TAG, self, (FalconEntity*)airtarget);
#else
				airtargetPtr = new SimObjectType((FalconEntity*) airtarget );
#endif
                            airtargetPtr.Reference(SIM_OBJ_REF_ARGS);
                            SetTarget(airtargetPtr);
                            if (isWing)				// let them loose...
                                AiGoShooter();
                            if (!isWing)			// make a radio call to the team
                            {
                                int flightIdx = self.GetCampaignObject().GetComponentIndex(self);
                                FalconRadioChatterMessage* radioMessage = new FalconRadioChatterMessage(self.Id(), FalconLocalSession);
                                radioMessage.dataBlock.from = self.Id();
                                radioMessage.dataBlock.to = MESSAGE_FOR_TEAM;
                                radioMessage.dataBlock.voice_id = ((Flight)(self.GetCampaignObject())).GetPilotVoiceID(self.vehicleInUnit);
                                radioMessage.dataBlock.message = rcREQHELPANSWER;
                                radioMessage.dataBlock.edata[0] = ((FlightClass*)self.GetCampaignObject()).callsign_id;
                                radioMessage.dataBlock.edata[1] = (((FlightClass*)self.GetCampaignObject()).callsign_num - 1) * 4 + flightIdx + 1;
                                radioMessage.dataBlock.edata[2] = 2 * (airtargetPtr.BaseData().Type() - VU_LAST_ENTITY_TYPE);
                                radioMessage.dataBlock.edata[3] = (short)SimToGrid(airtargetPtr.BaseData().XPos());
                                radioMessage.dataBlock.edata[4] = (short)SimToGrid(airtargetPtr.BaseData().YPos());
                                radioMessage.dataBlock.edata[5] = (short)airtargetPtr.BaseData().ZPos();
                                radioMessage.dataBlock.time_to_play = 4000; // 4 seconds
                                FalconSendMessage(radioMessage, false);
                            }
                        }
                    }
                }
            }
            // Targeting
            // no new targeting while dealing with threat
            if (threatPtr == null && airtargetPtr == null) // M.N. only retarget if we aren't threatened and don't have a divert air target
            {
                // 2000-09-25 MODIFIED BY S.G. SO WEAPON FREE COMMAND WITH NO DESIGNATED TARGET MAKES THE AI GO AFTER THEIR TARGET...
                //		if(isWing && mpActionFlags[AI_ENGAGE_TARGET]) {
                if (isWing && (mpActionFlags[AI_ENGAGE_TARGET] || mWeaponsAction == AI_WEAPONS_FREE))
                {
                    AiRunTargetSelection();
                }
                else
                {
                    TargetSelection();
                }
            }

            // calculate the relative geom on our target if we have one
            // always when defensive
            if (targetPtr)
            {
                // edg: check for dead targets!

                // 2000-09-21 MODIFIED BY S.G. THIS IS NOT ENOUGH. IF THE PLANE IS *DYING*, STOP TARGETING IT. NOT IF IT'S EXPLODING!
                //		if ( targetPtr.BaseData().IsExploding() )
                // 2002-01-14 MODIFIED BY S.G. targetPtr.BaseData CAN BE A *CAMPAIGN OBJECT* Don't assume it's a SimBaseClass!!!
                //                             Campaign object do not even have a pctStrength variable which will returned in garbage being used!
                if (targetPtr.BaseData().IsSim() && ((SimBaseClass*)targetPtr.BaseData()).pctStrength <= 0.0f) // Dying SIM target have a damage less than 0.0f
                {
                    // 2000-09-21 MODIFIED BY S.G. SetTarget DOES TOO MUCH. NEED TO CALL ClearTarget INSTEAD WHICH SIMPLY CLEARS IT, NO MATTER WHAT
                    //			SetTarget( null );
                    ClearTarget();
                }
                else
                {
                    if (curMode <= DefensiveModes || curMode == GunsEngageMode || SimLibElapsedTime > self.nextGeomCalc)
                    {
                        self.nextGeomCalc += self.geomCalcRate;
                        // hack! to avoid traversing a list, set the targetPtr's next var
                        // to null, then restore it
                        SimObjectType* savenext;

                        savenext = targetPtr.next;
                        targetPtr.next = null;
                        CalcRelGeom(self, targetPtr, ((AircraftClass*)self).vmat, 1.0F / SimLibMajorFrameTime);
                        targetPtr.next = savenext;

                        // Monitor rates to check for stagnation
                        if (targetPtr == lastTarget)
                        {
                            ataddot = ataddot * 0.85F + ataDot * 0.15F;
                            rangeddot = rangeddot * 0.85F + targetPtr.localData.rangedot * 0.15F;
                        }
                        else
                        {
                            ataddot = 10.0F;
                            rangeddot = 10.0F;
                        }
                    }
                }
            }

            // Maneuver control
            RunDecisionRoutines();

            // Select highest priority mode Resolve mode conflicts
            ResolveModeConflicts();

            // Print mode changes as they occur
            //	PrtMode();               

            // If I'm a leader or a wingman with permission to shoot and not defensive or in waypoint mode
            // MODIFIED BY S.G. SO AI CAN STILL DEFEND THEMSELF WHEN RETURNING TO BASE (ODDLY ENOUGH, LandingMode IS WHEN RTBing
            //  if((!isWing || mWeaponsAction == AI_WEAPONS_FREE) && targetPtr && curMode > DefensiveModes && 
            if ((!isWing || mWeaponsAction == AI_WEAPONS_FREE) && targetPtr && (curMode > DefensiveModes || curMode == LandingMode) &&
                (curMode != WaypointMode || agDoctrine == AGD_NONE))
            {
                // Weapon selection
                WeaponSelection();
            }
            else
            {
                // Never hold a missile over multiple frames
                if (curMissile)
                {
                    if (curMissile.launchState == MissileClass.PreLaunch)
                        curMissile.SetTarget(null);
                    curMissile = null;
                }
            }

            // Now that we know what we are doing tell our wingmen if we have them
            if (CommandTest())
            {
                CommandFlight();
            }

            // 2002-02-20 ADDED BY S.G. Check if we should jettison our tanks...
            if (self.Sms && !self.Sms.DidJettisonedTank())
            {
                if (SkillLevel() > 2)
                { // Smart one will do it under most condition
                    if ((curMode >= GroundAvoidMode && curMode <= MissileDefeatMode) || (curMode >= MissileEngageMode && curMode <= BVREngageMode) || curMode == BugoutMode)
                        self.Sms.TankJettison(); // will take care if tanks are empty
                }
                else if (SkillLevel() > 0)
                { // Not so smart will do it if threathened while dumb one won't do it...
                    if ((curMode >= GroundAvoidMode && curMode <= MissileDefeatMode))
                        self.Sms.TankJettison(); // will take care if tanks are empty
                }
            }

            // 2002-02-20 ADDED BY S.G. When damaged and going home, why bring the bombs with us...
            if (self.Sms && !self.Sms.DidEmergencyJettison() && self.pctStrength < 0.50F)
            {
                curMissile = null;
                self.Sms.EmergencyJettison();
                SelectGroundWeapon();
            }
        }


        protected void TargetSelection();
        protected FalconEntity* CampTargetSelection();
        protected void WeaponSelection();
        protected void FireControl()
        {
            float shootShootPct = 0.0F, pct = 0.0F;

            // basic check for firing, time to shoot, have a missile, have a target
            if (SimLibElapsedTime < missileShotTimer ||
               !curMissile || !targetPtr
                     || F4IsBadReadPtr(curMissile, sizeof(MissileClass)) // JB 010223 CTD
                     || F4IsBadReadPtr(self.FCC, sizeof(FireControlComputer)) // JB 010326 CTD
                     || F4IsBadReadPtr(self.Sms, sizeof(SMSClass)) // JB 010326 CTD
                     || F4IsBadReadPtr(targetPtr, sizeof(SimObjectType)) // JB 010326 CTD
                     || F4IsBadReadPtr(targetPtr.localData, sizeof(SimObjectLocalData)) // JB 010326 CTD
                     || !curMissile.sensorArray || F4IsBadReadPtr(curMissile.sensorArray, sizeof(SensorClass*)) // M.N. 011114 CTD
                     )
            {
                return;
            }

            // Are we cleared to fire?
            if (curMode != MissileEngageMode && !mWeaponsAction == AI_WEAPONS_FREE)
            {
                return;
            }

            // 2000-09-20 S.G. I CHANGED THE CODE SO ONLY ONE AIRPLANE CAN LAUNCH AT ANOTHER AIRPLANE (SAME CODE I ADDED TO 'TargetSelection')
            // me123 commented out for now. it seems the incomign missiles are not getting cleared !
            // 2001-08-31 S.G. FIXED PREVIOUS CODE WAS ASSUMING targetPtr WAS ALWAYS A SIM. IT CAN BE A CAMPAIGN OBJECT AS WELL, HENCE THE CTD.
            // if ((((SimBaseClass *)targetPtr.BaseData()).incomingMissile && ((SimWeaponClass *)((SimBaseClass *)targetPtr.BaseData()).incomingMissile).parent != self))
            if ((targetPtr.BaseData().IsAirplane() && ((SimBaseClass*)targetPtr.BaseData()).incomingMissile[1]) || (!targetPtr.BaseData().IsAirplane() && ((SimBaseClass*)targetPtr.BaseData()).incomingMissile[0]))
                return;
            //END OF ADDED SECTION

            // Check firing parameters
            // MODIFIED BY S.G. SO IR MISSILE HAVE A VARIABLE ATA
            //   if ( targetData.ata > 20.0f * DTR ||

            if
                (
                self.FCC.inRange == false ||
                targetData.range < self.FCC.missileRMin * 1.01F ||
                targetData.range > self.FCC.missileRMax * ((0.99F - isWing * 0.05f) + (4 - SkillLevel()) * 0.03F)
                )
                return;


            if // weapons management
                  (curMissile.sensorArray[0] && // JB 010715 CTD?
                        (// decrease max range on long range missiles (high trash risk shots when number of weapons decrease
                        curMissile.sensorArray[0].Type() == SensorClass.RadarHoming ||
                        curMissile.sensorArray[0].Type() == SensorClass.Radar
                        )
                        &&

                        (
                             targetData.range >
                             (self.FCC.missileRMax *
                                  (
                                        ( //         0.89 to 0.99      
                                             (0.99F - isWing * 0.05f)
                                        ) *
                                        (
                                        1.30 -
                                                  1.00f * min(
                                                                  ( //when 1 they shoot at 20% , when 0 shoot at 100%
                                                                        (((float)SkillLevel() / 2) / ((float)self.Sms.numOnBoard[wcAimWpn]))  // aces are holdign shoots for high pk
                                                                  ), 1.0f
                                                                )
                                                                  *
                                                                  (// head on 1, tail on 0
                                                                   (float)cos(targetPtr.localData.ataFrom / 2) * (float)cos(targetPtr.localData.ataFrom / 2)
                                                                  )
                                        )
                                  )
                             )
                        )

                    )
                return;

            if (targetData.range > 10 * Phyconst.NM_TO_FT)
            {
                if (holdlongrangeshot == false || holdlongrangeshot + 5 * CampaignSeconds < SimLibElapsedTime)// hold the longrange shoot so we can point and loft
                {
                    holdlongrangeshot = SimLibElapsedTime;
                    return;
                }
                else if (holdlongrangeshot + 4 * CampaignSeconds > SimLibElapsedTime)
                {
                    return;
                }

            }

            if // stuff like mavs has 20 degree off bore cabability
                    (
                        curMissile.sensorArray[0].Type() != SensorClass.RadarHoming &&
                        curMissile.sensorArray[0].Type() != SensorClass.Radar &&
                        curMissile.sensorArray[0].Type() != SensorClass.IRST &&
                        targetData.ata > 20.0f * DTR
                    )
                return;

            if // off bore or getting closer to bore
                    (curMissile.sensorArray[0].Type() == SensorClass.RadarHoming && (targetData.ata > 35.0f * DTR || targetData.atadot < 0.0f)) // 2002-03-12 MODIFIED BY S.G. && has HIGHER precedence than ||
                return;

            //	if // don't shoot semis if agregated
            //			(curMissile.sensorArray[0].Type() == SensorClass.RadarHoming && 
            //			((CampBaseClass*)curMissile.parent).IsAggregate()) 
            //			return;

            if // off bore or getting closer to bore
                    (curMissile.sensorArray[0].Type() == SensorClass.Radar && (targetData.ata > 35.0f * DTR || targetData.atadot < 0.0f))  // 2002-03-12 MODIFIED BY S.G. && has HIGHER precedence than ||
                return;

            if // irst iff bore
                    (
                    curMissile.sensorArray[0].Type() == SensorClass.IRST && targetData.ata >
                    ((IrstClass*)curMissile.sensorArray[0]).GetTypeData().GimbalLimitHalfAngle * 0.75f
                    )  //me123 ir's to 75% of gimbal limit
                return;

            // ADDED BY S.G. TO MAKE SURE WE DON'T FIRE BEAM RIDER IF THE MAIN RADAR IS JAMMED (NEW: USES SensorTrack INSTEAD of noTrack)
            if (curMissile.sensorArray && curMissile.sensorArray[0].Type() == SensorClass.RadarHoming ||
                 curMissile.sensorArray[0].Type() == SensorClass.Radar
                )
            {
                // Find the radar attached to us
                if (targetPtr.localData.sensorState[SensorClass.Radar] != SensorClass.SensorTrack)
                {
                    return;
                }

            }


            // WARNING: MIGHT HAVE TO DEAL WITH ARH MISSILE (LIKE AIM120) SINCE GetSeekerType WOULD RETURN SensorClass.Radar
            // END OF ADDED SECTIION
            curMissile.SetTarget(targetPtr);
            self.FCC.SetTarget(targetPtr);

            // Set the flag
            SetFlag(MslFireFlag);

            // Check doctrine
            switch (curMissile.GetSeekerType())
            {
                case SensorClass.Radar:
                case SensorClass.RWR:
                case SensorClass.HTS:
                    shootShootPct = TeamInfo[self.GetCountry()].GetDoctrine().RadarShootShootPct();
                    break;

                case SensorClass.IRST:
                case SensorClass.Visual:
                default:
                    shootShootPct = TeamInfo[self.GetCountry()].GetDoctrine().HeatShootShootPct();
                    break;
            }

            // Roll the 'dice'
            pct = ((float)rand()) / RAND_MAX * 100.0F;

            if (pct < shootShootPct && !IsSetATC(InShootShoot))
            {
                missileShotTimer = SimLibElapsedTime + 4 * SEC_TO_MSEC;
                //      MonoPrint ("DIGI BRAIN Firing Missile at Air Unit rng = %.0F: Shoot Shoot\n", targetData.range);
                SetATCFlag(InShootShoot);
            }
            else
            {
                float delay;

                delay = curMissile.GetTOF((-self.ZPos()), self.Vt(), targetData.ataFrom, targetPtr.BaseData().Vt(),
                      targetData.range) + 5.0F;
                delay += min(delay * 0.5F, 5.0F);
                missileShotTimer = SimLibElapsedTime + FloatToInt32(delay * SEC_TO_MSEC);
                //      MonoPrint ("DIGI BRAIN Firing Missile at Air Unit rng = %.0f: Shoot Look next %.2f\n", targetData.range, delay);
                ClearATCFlag(InShootShoot);
            }
            if (!IsSetATC(InShootShoot)) holdlongrangeshot = false;
        }


        protected void RunDecisionRoutines()
        {
            // If you're on the ground, just taxi
            if (!self.OnGround())
            {
                // 2001-07-10 REDONE BY S.G. SOME STUFF IS DONE IN AiRunDecisionRoutines SO WHY DO IT TWICE FOR WINGS?
                /*    CollisionCheck();
                      GunsJinkCheck();
                // 2001-07-10 REMOVED BY S.G. THE WINGS DON'T EVEN DO THAT SO WHY RESTRICT THE LEAD?
                //		if (!IsSetATC(InhibitDefensive))
                //		{
                            MissileDefeatCheck();
                //		}
                //		else
                //		{
                            ClearATCFlag(InhibitDefensive);
                //		}
                        SeparateCheck();

                      if (!isWing) // || (mpActionFlags[AI_ENGAGE_TARGET] && !mpActionFlags[AI_EXECUTE_MANEUVER]))
                      {
                         // Currently flight lead only
                         MergeCheck();
                         GunsEngageCheck();
                         MissileEngageCheck();

                         // if we're not on an air-air type mission -- no bvr!
                         if ( (missionClass == AAMission || missionComplete) && maxAAWpnRange != 0.0F)
                         {
                            WvrEngageCheck();
                            BvrEngageCheck();
                         }
                         AccelCheck();
                      }

                      // Don't die trying to bomb something!
                      ClearATCFlag(InhibitDefensive);
                */
                // Not done in AiRunDecisionRoutines and must be done by all flight members
                CollisionCheck();
                SeparateCheck();
                // 2002-03-11 MN added - if "SaidFumes", head for nearest friendly airbase
                if (g_nAirbaseCheck)
                    AirbaseCheck();

                if (!isWing)
                {
                    // Done in AiRunDecisionRoutines as well so limit it to lead in here
                    GunsJinkCheck();
                    MissileDefeatCheck();

                    // Currently flight lead only
                    MergeCheck();
                    GunsEngageCheck();
                    MissileEngageCheck();

                    // if we're not on an air-air type mission -- no bvr!
                    //me123 bvr wil react defensive now too         
                    // 2002-04-12 MN put back in with config variable - if people prefer AG to not do any BVR/WVR checks 
                    // - switch might not be made public, but better have the hook in...
                    // still look if we have still a weapon before checking for an engagement
                    // 2002-04-14 MN saw a flight lead in TakeoffMode asking wingman to engage, wingy taxied to the target...
                    if (g_bCheckForMode && curMode != TakeoffMode)
                    {
                        if ((g_bAGNoBVRWVR && ((missionClass == AAMission || missionComplete) && maxAAWpnRange != 0.0F))
                            || maxAAWpnRange != 0.0F)
                        {
                            WvrEngageCheck();
                            BvrEngageCheck();
                        }
                    }
                    else
                    {
                        if ((g_bAGNoBVRWVR && ((missionClass == AAMission || missionComplete) && maxAAWpnRange != 0.0F))
                            || maxAAWpnRange != 0.0F)
                        {
                            WvrEngageCheck();
                            BvrEngageCheck();
                        }
                    }
                    AccelCheck();
                }
                // END OF MODIFIED SECTION
                else
                {
                    AiRunDecisionRoutines();
                }

                //if we decided to refuel and there isn't anything more important,
                //refuel
                if (IsSetATC(NeedToRefuel))
                    AddMode(RefuelingMode);
            }

            // Check if I should be landing or taking off
            AiCheckLandTakeoff();

            /*------------------*/
            /* default behavior */
            /*------------------*/
            AddMode(WaypointMode);
        }

        protected void GroundCheck();
        protected void GunsEngageCheck();
        protected void GunsJinkCheck();
        protected void CollisionCheck();
        protected void MissileDefeatCheck();
        protected void MissileEngageCheck();
        protected void MergeCheck();
        protected void AccelCheck();
        protected void MergeManeuver();
        protected void SeparateCheck();
        protected void SensorFusion();

        protected float SetPstick(float pitchError, float gLimit, int commandType)
        {
            float stickFact = 0.0F, stickCmd = 0.0F;

            af.ClearFlag(AirframeClass.GCommand | AirframeClass.AlphaCommand |
               AirframeClass.AutoCommand | AirframeClass.ErrorCommand);
            if (commandType == AirframeClass.ErrorCommand)
            {
                if (pitchError > 30.0F)
                    stickCmd = gLimit;
                else if (pitchError > 0.0F)
                    stickCmd = gLimit * pitchError / 30.0F + 1.0F;
                else if (pitchError > -30.0F)
                    stickCmd = gLimit * 0.5F * pitchError / 30.0F;
                else
                    stickCmd = -gLimit * 0.5F;
                af.SetFlag(AirframeClass.GCommand);
            }
            else if (commandType == AirframeClass.GCommand)
            {
                /*
                   if (pitchError <= 1.0F)
                   stickCmd = -(float)sqrt((1.0F - pitchError) / (4.0F + self.platformAngles.costhe));
                   else
                   stickCmd = (float)sqrt((pitchError - 1.0F) / (gLimit - self.platformAngles.costhe));
                 af.SetFlag(AirframeClass.GCommand);
                 */
                stickCmd = max(min(pitchError, gLimit), -gLimit);
                af.SetFlag(AirframeClass.GCommand);
            }
            else if (commandType == AirframeClass.AlphaCommand)
            {
                stickCmd = pitchError * 0.75F;
                af.SetFlag(AirframeClass.AlphaCommand);
            }
            else
            {
                MonoPrint("digi.w: : BAD COMMAND MODE IN stickCmd!!!!!!!!!\n");
                stickCmd = 0.0F;
            }

            if (stickCmd <= 1.0F)
            {
                stickCmd = -(float)sqrt((1.0F - stickCmd) / (4.0F + self.platformAngles.costhe));
            }
            else
            {
                stickCmd = (float)sqrt((stickCmd - 1.0F) / (af.MaxGs() - self.platformAngles.costhe));
            }

            // Limit stick for low airspeeds

            stickFact = min(150.0F, self.Kias() - 150.0F);
            stickFact = 0.5F + stickFact / 300.0F;
            stickFact = max(0.0F, stickFact);
            stickCmd *= stickFact;

            // Smooth the command
            pStick = 0.2F * pStick + 0.8F * stickCmd;
            return pitchError;
        }

        protected float SetRstick(float rollError)
        {
            float maxRoll = af.MaxRoll();
            float stickCmd;

            if (fabs(self.Roll()) > maxRoll)
            {
                if (self.Roll() > 0.0F)
                {
                    rollError = min(rollError, (maxRoll - self.Roll()) * RTD);
                }
                else
                {
                    rollError = max(rollError, (maxRoll + self.Roll()) * RTD);
                }
            }

            stickCmd = rollError * DTR * 0.75F / max((af.kr01 * af.tr01), 0.1F);

            rStick = 0.2F * rStick + 0.8F * stickCmd;

            return rollError;
        }

        protected float SetYpedal(float yawError)
        {
            yPedal = 0.2F * yPedal - 0.8F * yawError * RTD * 0.0125F;

            return yawError;
        }

        //TODO protected float    SetRollCapture (float);
        protected void SetMaxRoll(float maxRoll)
        {
            af.SetMaxRoll(maxRoll);
        }

        protected void SetMaxRollDelta(float maxRoll)
        {
            af.SetMaxRollDelta(maxRoll);
        }

        protected void ResetMaxRoll()
        {
            af.ReSetMaxRoll();
        }

        protected void CollisionAvoid();
        protected float AutoTrack(float maxMnvrGs)
        {
            float xft, yft, zft, rx, ry, rz;
            float elerr, ata, droll, azerr;

            /*-----------------------------*/
            /* calculate relative position */
            /*-----------------------------*/

            xft = trackX - self.XPos();
            yft = trackY - self.YPos();
            zft = trackZ - self.ZPos();
            rx = self.dmx[0][0] * xft + self.dmx[0][1] * yft + self.dmx[0][2] * zft;
            ry = self.dmx[1][0] * xft + self.dmx[1][1] * yft + self.dmx[1][2] * zft;
            rz = self.dmx[2][0] * xft + self.dmx[2][1] * yft + self.dmx[2][2] * zft;

            ata = (float)atan2(sqrt(ry * ry + rz * rz), rx) * RTD;


            /*---------------*/
            /* roll and pull */
            /*---------------*/
            droll = (float)atan2(ry, -rz);
            elerr = (float)atan2(-rz, sqrt(rx * rx + ry * ry)) * RTD;
            azerr = (float)atan2(ry, rx) * RTD;

            /* set pstick this way for better return from a out of plane maneuver 
            SetPstick( (float)ata, maxMnvrGs, AirframeClass.ErrorCommand);
            */

            if (ata < 5.0F)
            {
                SetPstick(1.5F * elerr, maxMnvrGs, AirframeClass.ErrorCommand);
                SetYpedal(azerr / 4.0F);
                SetRstick(-self.Roll() * 5.0F);
            }
            else if (ata < 10.0f && curMode >= BVREngageMode)
            {
                // edg note: what this does is to roll in the opposite direction
                // and do a neg G push rather than roll all the way around and pull
                // if our roll error is large ( in this case beyond 150deg ).   This
                // will keep it from flip-flopping around as seen previously, plus
                // I think it's the way a pilot would likely do things.
                if (droll > 150.0f * DTR && rStick < 0.5F)
                {
                    SetRstick(droll * RTD - 180.0f);
                    SetPstick(-ata, maxMnvrGs, AirframeClass.ErrorCommand);
                }
                else if (droll < -150.0f * DTR && rStick > -0.5F)
                {
                    SetRstick(droll * RTD + 180.0f);
                    SetPstick(-ata, maxMnvrGs, AirframeClass.ErrorCommand);
                }
                else
                {
                    SetRstick(droll * RTD);
                    SetPstick(ata * RTD, maxMnvrGs, AirframeClass.ErrorCommand);
                }
                SetYpedal(0.0F);
            }
            else
            {
                // If we're stupid
                if (SkillLevel() < 2 && fabs(ata) > 90.0F && fabs(self.Pitch()) < 45.0F * DTR)
                {
                    elerr = maxMnvrGs * 0.85F;
                    droll = (float)acos(1.0F / elerr);
                    SetMaxRoll(droll * RTD);
                    droll -= self.Roll();
                    SetRstick(droll * RTD);
                    SetPstick(maxMnvrGs, maxMnvrGs, AirframeClass.GCommand);
                }
                else
                {
                    SetRstick(droll * RTD);
                    SetPstick(ata * min((30.0F * DTR) / (float)fabs(droll), 1.0F), maxMnvrGs, AirframeClass.AlphaCommand);//me123 from errorcommand
                    SetYpedal(0.0F);
                    SetMaxRoll((float)fabs(self.Roll() + droll) * RTD);
                    SetMaxRollDelta(droll * RTD);
                }
            }

            /*-------------------------*/
            /* return nose angle error */
            /*-------------------------*/
            return (ata);
        }
        protected float TrackPoint(float maxGs, float speed)
        {
            float retval = 0.0F;

            if (!self.OnGround())
                af.SetFlaps(curMode == LandingMode);
            if (self.af.GetSimpleMode())
            {
                // do simple flight model
                SimpleTrack(SimpleTrackSpd, speed);
            }
            else
            {
                retval = AutoTrack(maxGs);
                MachHold(speed * FTPSEC_TO_KNOTS, self.Kias(), true);
            }

            return retval;
        }
        protected float TrackPointLanding(float speed)
        {
            float xft, yft, zft, rx, ry, rz, elErr;
            float eProp, minSpeed;

            CalculateRelativePos(&xft, &yft, &zft, &rx, &ry, &rz);

            rStick = SimpleTrackAzimuth(rx, ry, self.Vt());

            //clamp yaw rate to 10 deg/s max
            //simple model uses a max yaw rate of 20 deg/s
            rStick = max(min(rStick, 0.15F), -0.15F);

            elErr = SimpleTrackElevation(trackZ - self.ZPos(), (float)sqrt(xft * xft + yft * yft));
            // keep stick at reasonable values.
            pStick = min(0.2f, max(elErr, -0.3F));

            eProp = speed - af.vt;

            if (af.z - af.groundZ < -200.0F)
            {
                minSpeed = af.CalcDesSpeed(10.0F);
                if (speed < minSpeed)
                {
                    eProp = minSpeed - af.vt;
                    speed = minSpeed;
                }
            }

            //if we're going to stall out, hit the gas a bit
            if (af.Qsom() * af.Cnalpha() < 1.55F && eProp < 20.0F)
            {
                eProp = 20.0F;
                speed = af.vt + 20.0F;
            }

            if (eProp >= 150.0F)
            {
                autoThrottle = 1.5F;
                throtl = 1.5F;                        /* burner     */
                af.speedBrake = -1.0F;
            }
            else if (eProp < -100.0F)
            {
                autoThrottle = 0.0F;
                throtl = 0.0F;                        /* idle and boards */
                af.speedBrake = 1.0F;
            }
            else
            {
                if (atcstatus == lOnFinal || (throtl == 0.0F && af.vtDot > -5.0F && eProp < -10.0F))
                    //deploy speed brakes on final
                    af.speedBrake = 1.0F;
                else
                    af.speedBrake = -1.0F;
                autoThrottle += eProp * 0.01F * SimLibMajorFrameTime;
                autoThrottle = max(0.0F, min(1.5F, autoThrottle));
                throtl = eProp * 0.02F + autoThrottle - af.vtDot * SimLibMajorFrameTime * 0.005F;
            }
            af.SetFlaps(true);
            //MonoPrint("Eprop:  %6.3f  autoTh: %6.3f  vtDot: %6.3f  throtl: %6.3f\n", eProp, autoThrottle, af.vtDot, throtl);
            throtl = min(max(throtl, 0.0F), 1.5F);

            return speed;
        }

        protected float VectorTrack(float maxMnvrGs, int fineTrack = false)
        {
#if NOTHING
double xft,yft,zft, rx,ry,rz, range, xyRange;
double azerr, elerr, ata, droll;

   /*-----------------------------*/
   /* calculate relative position */
   /*-----------------------------*/
   
   xft = trackX - af.x;
   yft = trackY - af.y;
   zft = trackZ - af.z;
   rx = self.vmat[0][0]*xft + self.vmat[0][1]*yft + self.vmat[0][2]*zft;
   ry = self.vmat[1][0]*xft + self.vmat[1][1]*yft + self.vmat[1][2]*zft;
   rz = self.vmat[2][0]*xft + self.vmat[2][1]*yft + self.vmat[2][2]*zft;


   /** MBR: If this code is turned back 'ON', it **/
   /** should be modified....                    **/

   range = sqrt (rx*rx + ry*ry + rz*rz);

   /*--------------*/        
   /* Sanity Check */
   /*--------------*/        
   if (range < 0.1F)
      return (0.0F);

   rx = max ( min (rx, range), -range);        
   ry = max ( min (ry, range), -range);        
   rz = max ( min (rz, range), -range);        

   /*-------------------*/
   /* relative geometry */
   /*-------------------*/
   if (rx != 0.0F)
      ata      = (float)acos (rx/range) * RTD;
   else
      ata = 0.0F;     
   droll    = atan2 (ry,-rz);
   xyRange = sqrt (rx*rx + ry*ry);
   azerr    = atan2 (ry,rx)*RTD;
   elerr    = atan (-rz/xyRange) * RTD;

   /*---------------*/
   /* roll and pull */
   /*---------------*/
   if (trackMode == 1)
   {
      /* alternative method for setting pstick allows for unloaded rolls...  */
      SetPstick(elerr * 0.75F, maxMnvrGs, AirframeClass.ErrorCommand);

      /* set pstick this way for better return from a out of plane maneuver 
      SetPstick( (float)ata, maxGs, AirframeClass.ErrorCommand);
      */

      SetRstick(droll*RTD);
      SetYpedal( 0.0F);
      SetMaxRollDelta (droll*RTD);

      if (ata < 5.0) trackMode = 2;
   }
   /*----------------------------*/
   /* pitch and yaw, wings level */
   /*----------------------------*/
   else
   {
      SetPstick( (float)elerr, maxMnvrGs, AirframeClass.ErrorCommand);
      SetYpedal( (float)azerr/3.0F);
            
      if (ata > 8.0F) trackMode = 1;
   }

   /*-------------------------*/
   /* return nose angle error */
   /*-------------------------*/
   return ((float)ata);
#else
            return 0.0F;
#endif
        }

        protected int Stagnated()
        {
            int retval = false;

            if (fabs(ataddot) < 4.0F * DTR && fabs(rangeddot) < 50.0F &&
                fabs(self.YawDelta()) > 8.0F * DTR)
            {
                retval = true;
                ataddot = 10.0F;
                rangeddot = 10.0F;
            }

            return (retval);
        }

        // 2002-03-11 MN Check for fuel state, head to closest airbase if SaidFumes
        protected void AirbaseCheck();

        // WVR Stuff
        protected void WvrEngageCheck();
        protected void WvrChooseTactic();
        protected void WvrEngage();
        protected void WvrRollOutOfPlane();
        protected void WvrStraight();
        protected void WvrBugOut();
        protected void WvrGunJink();
        protected void WvrOverBank(float delta);
        protected void WvrAvoid();
        protected void WvrBeam();
        protected void WvrRunAway();
        protected VU_TIME mergeTimer;
        protected VU_TIME wvrTacticTimer;
        protected VU_TIME engagementTimer;
        protected WvrTacticType wvrCurrTactic;
        protected WvrTacticType wvrPrevTactic;
        protected float maxAAWpnRange;
        protected bool buggedOut;

        // Bvr Stuff
        protected void BvrEngageCheck();
        protected void BvrChooseTactic();
        protected void BvrEngage();
        protected int IsSupportignmissile();// are self or our wingie suporting a missile
        protected int WhoIsSpiked();//me123
        protected int WhoIsHotnosed();//me123
        protected int HowManySpiked();//me123
        protected int HowManyHotnosed();//me123
        protected int HowManyTargetet();//me123
        protected int IsSplitup();//me123
        protected void CalculateMAR();//me123
        protected void AiFlyBvrFOrmation();//me123
        protected VU_TIME bvrTacticTimer;//me123
        protected bool spiked;//me123
        protected int offsetdir;//me123
        protected VU_TIME spikeseconds;//me123
        protected VU_TIME spikesecondselement;//me123
        protected float MAR;//me123
        protected float TGTMAR;//me123
        protected float DOR;//me123
        protected bool Isflightlead;//me123
        protected bool IsElementlead;//me123
        protected int bvractionstep;//me123
        protected BVRProfileType bvrCurrProfile;//me123
        protected BVRInterceptType bvrCurrTactic;
        protected BVRInterceptType bvrPrevTactic;
        protected VuEntity* missilelasttime;
        protected VU_TIME spiketframetime;
        protected FalconEntity* lastspikeent;
        protected VU_TIME spiketframetimewingie;
        protected FalconEntity* lastspikeentwingie;

        // Threat handling
        protected SimObjectType threatPtr;
        protected SimObjectType preThreatPtr;
        protected float threatTimer;
        protected void SetThreat(FalconEntity* obj);
        protected bool HandleThreat();


        public DigitalBrain(AircraftClass* myPlatform, AirframeClass* myAf)
            : base()
        {
            af = myAf;
            self = myPlatform;
            flightLead = null;
            targetPtr = null;
            targetList = null;
            isWing = false; // JPO initialise
            mCurFormation = -1; // JPO
            SetLeader(self);
            onStation = NotThereYet;
            mLastOrderTime = 0;
            wvrCurrTactic = WVR_NONE;
            wvrPrevTactic = WVR_NONE;
            wvrTacticTimer = 0;
            lastAta = 0;
            engagementTimer = 0;

            // ADDED BY S.G. SO DIGI KNOWS IT HASN'T LAUNCHED A MISSILE YET (UNUSED BEFORE - NAME IS MEANINGLESS BUT WHAT THE HECK)
            missileFiredEntity = null;
            missileFiredTime = 0;
            rwIndex = 0;
            if (self.OnGround())
                airbase = self.TakeoffAirbase(); // we take off from this base!
            else
                airbase = self.HomeAirbase(); // original code.
            if (airbase == FalconNullId)
            {
                GridIndex x, y;
                vector pos;
                pos.x = self.XPos();
                pos.y = self.YPos();

                //find nearest airbase
                ConvertSimToGrid(&pos, &x, &y);
                Objective obj = FindNearestFriendlyAirbase(self.GetTeam(), x, y);
                //if(obj)
                if (obj && !F4IsBadReadPtr(obj, sizeof(ObjectiveClass))) // JB 010326 CTD
                    airbase = obj.Id();
            }
            atcstatus = noATC;
            curTaxiPoint = 0;
            rwtime = 0;
            desiredSpeed = 0.0F;
            turnDist = 0.0F;
            atcFlags = 0;
            waittimer = 0;
            distAirbase = 0.0F;
            updateTime = 0;
            createTime = SimLibElapsedTime;

            if (self.OnGround())
            {
                SetATCFlag(Landed);
                SetATCFlag(RequestTakeoff);
                ClearATCFlag(PermitTakeoff);
                SetATCFlag(StopPlane);
            }
            else
            {
                SetATCFlag(PermitTakeoff);
                ClearATCFlag(Landed);
                ClearATCFlag(RequestTakeoff);
            }

            autoThrottle = 0.0F;
            velocitySlope = 0.0F;

            tankerId = FalconNullId;
            tnkposition = -1;
            refuelstatus = refNoTanker;
            tankerRelPositioning.x = 0.0F;
            tankerRelPositioning.y = 0.0F;
            tankerRelPositioning.z = -3.0F;
            lastBoomCommand = 0;

            Package package;
            Flight flight;
            escortFlightID = FalconNullId; // 2002-02-27 S.G.

            flight = (Flight)self.GetCampaignObject();
            if (flight)
            {
                package = flight.GetUnitPackage();
                if (package)
                {
                    tankerId = package.GetTanker();

                    // 2002-02-27 ADDED BY S.G. Lets save our escort flight pointer if we have one. It's going to come handy in BvrEngageCheck...
                    for (UnitClass* un = package.GetFirstUnitElement(); un; un = package.GetNextUnitElement())
                    {
                        if (un.IsFlight())
                        {
                            if (((FlightClass*)un).GetUnitMission() == AMIS_ESCORT)
                                escortFlightID = un.Id(); // We got one!
                        }
                    }
                    // END OF ADDED SECTION 2002-02-27
                }
            }


            trackX = self.XPos();
            trackY = self.YPos();
            trackZ = self.ZPos();

            agDoctrine = AGD_NONE;
            agImprovise = false; ;
            nextAGTargetTime = SimLibElapsedTime;
            missileShotTimer = SimLibElapsedTime;
            curMissile = null;
            sentWingAGAttack = AG_ORDER_NONE;
            nextAttackCommandToSend = 0; // 2002-01-20 ADED BY S.G. Make sure it's initialized to something we can handle
            jinkTime = 0;
            jammertime = false;//ME123
            holdlongrangeshot = 0.0f;
            cornerSpeed = af.CornerVcas();
            rangeToIP = FLT_MAX;
            madeAGPass = false;
            // 2001-05-21 ADDED BY S.G. INIT waitingForShot SO IT'S NOT GARBAGE TO START WITH
            waitingForShot = 0;
            // END OF ADDED SECTION
            pastAta = 0;
            pastPipperAta = 0;


            // 2001-10-12 ADDED BY S.G. INIT gndTargetHistory[2] SO IT'S NOT GARBAGE TO START WITH
            gndTargetHistory[0] = gndTargetHistory[1] = null;
            // END OF ADDED SECTION

            // WingAi inits

            if (self.OnGround())
            {
                mpActionFlags[AI_FOLLOW_FORMATION] = false;
            }
            else
            {
                mpActionFlags[AI_FOLLOW_FORMATION] = true;
            }

            mLeaderTookOff = false;
            mpActionFlags[AI_ENGAGE_TARGET] = AI_NONE;
            mpActionFlags[AI_EXECUTE_MANEUVER] = false;
            mpActionFlags[AI_USE_COMPLEX] = false;
            mpActionFlags[AI_RTB] = false;
            mpActionFlags[AI_LANDING] = false;

            mpSearchFlags[AI_SEARCH_FOR_TARGET] = false;
            mpSearchFlags[AI_MONITOR_TARGET] = false;
            mpSearchFlags[AI_FIXATE_ON_TARGET] = false;

            mCurrentManeuver = FalconWingmanMsg.WMTotalMsg;
            mDesignatedObject = FalconNullId;
            mFormation = acFormationData.FindFormation(FalconWingmanMsg.WMWedge);
            mDesignatedType = AI_NO_DESIGNATED;
            mSearchDomain = DOMAIN_ABSTRACT;
            mWeaponsAction = AI_WEAPONS_HOLD;
            mInPositionFlag = false;

            mFormRelativeAltitude = 0.0F;
            mFormSide = 1;
            mFormLateralSpaceFactor = 1.0F;
            mSplitFlight = false;

            mLastReportTime = 0;
            mpLastTargetPtr = null;

            mAzErrInt = 0.0F;
            mLeadGearDown = -1;
            groundAvoidNeeded = false;

            groundTargetPtr = null;
            airtargetPtr = null;
            preThreatPtr = null;
            threatPtr = null;
            threatTimer = 0.0f;

            pStick = 0.0F;
            rStick = 0.0F;
            throtl = 0.0F; // jpo - from 1 - start at 0
            yPedal = 0.0F;

            // Modes and rules
            maxGs = af.MaxGs();
            maxGs = max(maxGs, 2.5F);

            curMode = WaypointMode;
            lastMode = WaypointMode;
            nextMode = NoMode;
            trackMode = 0;
            // Init Target data
            targetPtr = null;
            lastTarget = null;

            // Setup BVR Stuff
            bvrCurrTactic = BvrNoIntercept;
            bvrPrevTactic = BvrNoIntercept;
            bvrTacticTimer = 0;
            spiked = 0;
            MAR = 0.0f;
            TGTMAR = 0.0f;
            DOR = 0.0f;
            Isflightlead = false;
            IsElementlead = false;
            bvractionstep = 0;
            bvrCurrProfile = Pnone;
            offsetdir = 0;
            spikeseconds = 0;
            spikesecondselement = 0;
            spikeseconds = 0;
            missilelasttime = null;
            spiketframetime = null;
            lastspikeent = null;
            spiketframetimewingie = null;
            lastspikeentwingie = null;
            gammaHoldIError = 0;
            reactiont = 0;
            // start out assuming we at least have guns
            maxAAWpnRange = 6000.0f;

            // once we've bugged out of WVR our tactics will change
            buggedOut = false;

            // edg missionType and missionClass are now in the digi class since
            // they're used a lot (rather than being locals everywhere)
            missionType = ((UnitClass*)(self.GetCampaignObject())).GetUnitMission();
            switch (missionType)
            {
                case AMIS_BARCAP:
                case AMIS_BARCAP2:
                    // 2002-03-05 ADDED BY S.G. These need to attack bombers as well and if OnSweep isn't set, SensorFusion will ignore them
                    maxEngageRange = 45.0F * Phyconst.NM_TO_FT;//me123 from 20
                    missionClass = AAMission;
                    SetATCFlag(OnSweep);
                    break;
                // END OF ADDED SECTION 2002-03-05

                case AMIS_HAVCAP:
                case AMIS_TARCAP:
                case AMIS_RESCAP:
                case AMIS_AMBUSHCAP:
                case AMIS_NONE:
                    maxEngageRange = 45.0F * Phyconst.NM_TO_FT;//me123 from 20
                    missionClass = AAMission;
                    ClearATCFlag(OnSweep);
                    break;

                case AMIS_SWEEP:
                    maxEngageRange = 60.0F * Phyconst.NM_TO_FT;//me123 from 80
                    missionClass = AAMission;
                    SetATCFlag(OnSweep);
                    break;

                case AMIS_ALERT:
                case AMIS_INTERCEPT:
                case AMIS_ESCORT:
                    maxEngageRange = 45.0F * Phyconst.NM_TO_FT;//me123 from 30
                    missionClass = AAMission;
                    ClearATCFlag(OnSweep);
                    break;

                case AMIS_AIRLIFT:
                    maxEngageRange = 40.0F * Phyconst.NM_TO_FT;//me123 from 10 bvrengage will now crank beam and drag defensive
                    missionClass = AirliftMission;
                    ClearATCFlag(OnSweep);
                    break;

                case AMIS_TANKER:
                case AMIS_AWACS:
                case AMIS_JSTAR:
                case AMIS_ECM:
                case AMIS_SAR:
                    maxEngageRange = 40.0F * Phyconst.NM_TO_FT;//me123 from 10bvrengage will now crank beam and drag defensive
                    missionClass = SupportMission;
                    ClearATCFlag(OnSweep);
                    break;

                default:
                    maxEngageRange = 40.0F * Phyconst.NM_TO_FT;//me123 from 10
                    missionClass = AGMission;
                    ClearATCFlag(OnSweep);

                    // Engage sooner after mission complete

                    // JB 010719 missionComplete has not been initialized yet.
                    //if (missionComplete)
                    //   maxEngageRange *= 1.2F;//me123 from 2.0
                    break;
            }
            // 2002-02-27 ADDED BY S.G. What about flights on egress that deaggregates? Look up their Eval flags and set missionComplete accordingly...
            if (((FlightClass*)self.GetCampaignObject()).GetEvalFlags() & FEVAL_GOT_TO_TARGET)
                missionComplete = true;
            else
                // END OF ADDED SECTION 2002-02-27
                missionComplete = false;

            // Check for trainable guns
            if (self.Sms.HasTrainable())
                SetATCFlag(HasTrainable);

            moreFlags = 0; // 2002-03-08 ADDED BY S.G. (Before SelectGroundWeapon which will query it

            // Check for AG weapons
            SelectGroundWeapon();

            // Check for AA Weapons
            SetATCFlag(AceGunsEngage);
            // JPO - for startup actions.
            mActionIndex = 0;
            mNextPreflightAction = 0;
            lastGndCampTarget = null;

            destRoll = 0;
            destPitch = 0;
            currAlt = 0;

            // 2002-01-29 ADDED BY S.G. Init our targetSpot and associated
            targetSpotFlight = null;
            targetSpotFlightTarget = null;
            targetSpotFlightTimer = 0;
            targetSpotElement = null;
            targetSpotElementTarget = null;
            targetSpotElementTimer = 0;
            targetSpotWing = null;
            targetSpotWingTarget = null;
            targetSpotWingTimer = 0;
            // END OF ADDED SECTION
            radarModeTest = 0; // 2002-02-10 ADDED BY S.G.
            // 2002-02-24 MN
            pullupTimer = 0.0f;
            tiebreaker = 0;
            nextFuelCheck = SimLibElapsedTime; // 2002-03-02 MN fix airbase check NOT 0 - set the time here.. aargh...
            airbasediverted = 0;
        }
        //TODO public virtual ~DigitalBrain	();
        public virtual void Sleep()
        {
            SetLeader(null);
            ClearTarget();

            // NOTE: This is only legal if the platorms target list is already cleared.
            // Currently, SimVehicle.Sleep call SimMover.Sleep which clears the list,
            // then it calls theBrain.Sleep.  As long as this doesn't change this will
            // not cause a leak.
            if (groundTargetPtr)
            {
#if DEBUG
                if (groundTargetPtr.prev || groundTargetPtr.next)
                {
                    MonoPrint("Ground target still in list at sleep\n");
                }
#endif
                groundTargetPtr.prev = null;
                groundTargetPtr.next = null;
            }

            SetGroundTarget(null);
        }

        public void ClearCurrentMissile() { curMissile = null; }
        public void SetBvrCurrProfile(BVRProfileType newProfile) { bvrCurrProfile = newProfile; }
        public float PIDLoop(float error, float K, float KD, float KI, float Ts, float* lastErr, float* MX, float Output_Top, float Output_Bottom, bool LimitMX);

        public void ReSetLabel(SimBaseClass* theObject);

        public void ReceiveOrders(FalconEvent* theEvent);
        public void JoinFlight()
        {
            SimBaseClass* newLead = self.GetCampaignObject().GetComponentLead();

            if (newLead == self)
            {
                SetLead(true);
            }
            else
            {
                SetLead(false);
            }
        }


        public void CheckLead()
        {
            SimBaseClass* pobj;
            SimBaseClass* newLead = null;

            bool done = false;
            int i = 0;

            if (flightLead &&
                 flightLead.VuState() == VU_MEM_ACTIVE &&
                 !flightLead.IsDead())
                return;

            VuListIterator cit = new VuListIterator(self.GetCampaignObject().GetComponents());
            pobj = (SimBaseClass*)cit.GetFirst();
            while (!done)
            {
                if (pobj &&
                   pobj.VuState() == VU_MEM_ACTIVE &&
                   !pobj.IsDead())
                {
                    done = true;
                    newLead = pobj;
                }
                else if (i > 3)
                {
                    done = true;
                    newLead = self;
                }

                i++;
                pobj = (SimBaseClass*)cit.GetNext();
            }
            assert(newLead);
            SetLeader(newLead);
        }

        public void SetLead(int flag)
        {
            if (flag == true)
            {
                isWing = false;
                SetLeader(self);
            }
            else
            {
                isWing = self.GetCampaignObject().GetComponentIndex(self);
                SetLeader(self.GetCampaignObject().GetComponentLead());
            }
        }

        public void FrameExec(SimObjectType* curTargetList, SimObjectType* curTarget)
        {
#if CHECK_PROC_TIMES
	gPart1 = 0;
	gPart2 = 0;
	gPart3 = 0;
	gPart4 = 0;
	gPart5 = 0;
	gPart6 = 0;
	gPart7 = 0;
	gPart8 = 0;
	gWhole = GetTickCount();
#endif
            // Modify max gs based on skill level
            // Rookies (level 0) has .5 max gs, top of the line has max gs
            // Keep max gs >= 2.5
            //maxGs = af.MaxGs()  * (SkillLevel() / 4.0F * 0.5F + 0.5F);
            //me123 they might be stupid , but hey give em the g's at least.
            maxGs = af.MaxGs();  //* (SkillLevel() / 4.0F * 0.5F + 0.8F);
            maxGs = max(maxGs, 2.5F);


#if DAVE_DBG
   // Update the label in debug
   //char tmpStr[40];
/* ECTS HACK
   
   if (targetPtr && targetPtr.BaseData() == SpikeCheck(self))
      missionType = -missionType;
   sprintf (tmpStr, "%s %d-%d", ((VehicleClassDataType*)((Falcon4EntityClassType*)self.EntityType()).dataPtr).Name,
      missionType, (self.curWaypoint ? self.curWaypoint.GetWPAction() : 0));
   if (targetPtr && targetPtr.BaseData().IsSim())
      sprintf (tmpStr, "%s %s", tmpStr, 
      ((VehicleClassDataType*)((Falcon4EntityClassType*)targetPtr.BaseData().EntityType()).dataPtr).Name);

   if(atcstatus == noATC)
		sprintf (tmpStr,"noATC");
   else
   {
	   int delta = waittimer - Camp_GetCurrentTime();
	   int rwdelta = rwtime - Camp_GetCurrentTime();

	   sprintf (tmpStr,"%d  %d  %4.2f  %4.2f", self.share_.id_.num_, atcstatus, delta/1000.0F, rwdelta/1000.0F);
   }
   if ( self.drawPointer )
   	  ((DrawableBSP*)self.drawPointer).SetLabel (tmpStr, ((DrawableBSP*)self.drawPointer).LabelColor());
	  */
#endif

            /*   // Turn on/off external lights according to 
	if (self.IsF16() && atcstatus == noATC)
		self.ExtlOff(self.Extl_Main_Power);
	else
		self.ExtlOn(self.Extl_Main_Power);
*/

            // 2002-03-15 MN if we've flamed out and our speed is below minvcas, put the wheels out so that there is a chance to land
            if (self.af.Fuel() <= 0.0F && self.af.vcas < self.af.MinVcas())
            {
                // Set Landed flag now, so that RegroupAircraft can be called in Eom.cpp - have the maintenance crew tow us back to the airbase ;-)
                SetATCFlag(Landed);
                af.gearHandle = 1.0F;
            }
            else
                // make sure the wheels are up after takeoff
                if (self.curWaypoint && self.curWaypoint.GetWPAction() != WP_TAKEOFF)
                    af.gearHandle = -1.0F;

            // assume we're not going to be firing in this frame
            ClearFlag(MslFireFlag | GunFireFlag);

            // check to see if our leader is dead and if not set leader to next in
            // flight (and/or ourself)
            CheckLead();

#if CHECK_PROC_TIMES
	gPart1 = GetTickCount();
#endif
            // Find a threat/target
            DoTargeting();
#if CHECK_PROC_TIMES
	gPart1 = GetTickCount() - gPart1;
	gPart2 = GetTickCount();
#endif
            // Make Decisions
            SetCurrentTactic();
#if  CHECK_PROC_TIMES
	gPart2 = GetTickCount() - gPart2;
	gPart3 = GetTickCount();
#endif
            // Set the controls 
            Actions();
#if  CHECK_PROC_TIMES
	gPart3 = GetTickCount() - gPart3;
	gPart4 = GetTickCount();
#endif
            // has the target changed?
            if (targetPtr != lastTarget)
            {
                lastTarget = targetPtr;
                ataddot = 10.0F;
                rangeddot = 10.0F;
            }

            // edg: check stick settings for bad values
            if (rStick < -1.0f)
                rStick = -1.0f;
            else if (rStick > 1.0f)
                rStick = 1.0f;

            if (pStick < -1.0f)
                pStick = -1.0f;
            else if (pStick > 1.0f)
                pStick = 1.0f;
            //me123 unload if roll input is 1 (to rool faster and eleveate the f4 bug)
            if (fabs(rStick) > 0.9f && groundAvoidNeeded == false)
                pStick = 0.0f;

            if (throtl < 0.0f)
                throtl = 0.0f;
            else if (throtl > 1.5f)
                throtl = 1.5f;


#if  CHECK_PROC_TIMES
	gPart4 = GetTickCount() - gPart4;
	gWhole = GetTickCount() - gWhole;

	if(gameCompressionRatio && self.IsPlayer())
	{
		MonoPrint("Whole:  %3d  Targ: %3d  Tac: %3d Action: %3d  Rest: %3d\n", gWhole, gPart1, gPart2, gPart3, gPart4);
	}
#endif
        }

        public void ThreeAxisAP();
        public void WaypointAP();
        public void LantirnAP();
        public void RealisticAP();
        public void HDGSel();
        public void PitchRollHold();
        public void RollHold();
        public void AltHold();
        public void FollowWP();
        public void CheckForTurn();
        public int CheckAPParameters();
        public void AcceptManual();
        public float HeadingDifference, bank;
        public void SetHoldAltitude(float newAlt) { holdAlt = newAlt; }
        public float GetHoldAltitude() { return holdAlt; }
        public void SetHoldHeading(float newPsi) { holdPsi = newPsi; }
        public float GetHoldHeading() { return holdPsi; }
        public SimObjectType* GetGroundTarget() { return groundTargetPtr; }
        public MissionTypeEnum MissionType() { return missionType; }
        public MissionClassEnum MissionClass() { return missionClass; } // 2002-03-05 ADDED BY S.G. Need it public

#if DAVE_DBG
	void	SetDebugLabel(ObjectiveClass*);
#else
        public void SetDebugLabel(ObjectiveClass* p) { }
#endif
        public VU_TIME CreateTime() { return createTime; }
        public int CalcWaitTime(ATCBrain* Atc)
        {
            VU_TIME count = 0;
            VU_TIME time = rwtime;

            switch (atcstatus)
            {
                case tPrepToTakeRunway:
                case tTakeRunway:
                case tTakeoff:
                    time = SimLibElapsedTime + 5 * CampaignSeconds;
                    break;

                case tEmerStop:
                    time = SimLibElapsedTime + 2 * TAKEOFF_TIME_DELTA;
                    break;

                case tHoldShort:
                case tReqTaxi:
                case tReqTakeoff:
                    time = SimLibElapsedTime + TAKEOFF_TIME_DELTA;
                    break;

                case tTaxi:
                default:
                    count = GetTaxiPosition(curTaxiPoint, rwIndex);
                    if (rwtime > count * TAKEOFF_TIME_DELTA/* + SimLibElapsedTime*/)
                        time = rwtime - (count * TAKEOFF_TIME_DELTA /*+ SimLibElapsedTime*/);
                    else if (PtDataTable[curTaxiPoint].type <= TakeoffPt)
                        time = SimLibElapsedTime + 5 * CampaignSeconds;
                    else
                        time = SimLibElapsedTime + TAKEOFF_TIME_DELTA;

                    if ((isWing == 1 || isWing == 3) && Atc.UseSectionTakeoff((Flight)self.GetCampaignObject(), rwIndex))
                        time += TAKEOFF_TIME_DELTA;
                    break;
            }

            return time;
        }

        public void ResetTaxiState()
        {
            int takeoffpt, runwaypt, rwindex;
            float x1, y1, x2, y2;
            float dx, dy, relx;
            //float deltaHdg;
            //ObjectiveClass *Airbase = (ObjectiveClass *)vuDatabase.Find(airbase);

            GridIndex X, Y;

            X = SimToGrid(af.y);
            Y = SimToGrid(af.x);

            Objective Airbase = FindNearbyAirbase(X, Y);

            if (!self.OnGround() || !Airbase)
            {
                if (atcstatus >= tReqTaxi)
                    ResetATC();
                return;
            }

            float dist, closestDist = 4000000.0F;
            int taxiPoint, closest = curTaxiPoint;

            taxiPoint = GetFirstPt(rwIndex);
            while (taxiPoint)
            {
                TranslatePointData(Airbase, taxiPoint, &x1, &y1);
                dx = x1 - af.x;
                dy = y1 - af.y;
                dist = dx * dx + dy * dy;
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = taxiPoint;
                }
                taxiPoint = GetNextPt(taxiPoint);
            }

            if (closest != curTaxiPoint)
            {
                curTaxiPoint = closest;
                TranslatePointData(Airbase, curTaxiPoint, &trackX, &trackY);
            }

            if (self.AutopilotType() == AircraftClass.APOff)
                return;

            if ((atcstatus == tTakeRunway || atcstatus == tTakeoff) &&
                 (PtDataTable[curTaxiPoint].type == TakeoffPt || PtDataTable[curTaxiPoint].type == RunwayPt))
            {
                rwindex = Airbase.brain.IsOnRunway(self);

                takeoffpt = Airbase.brain.FindTakeoffPt((Flight)self.GetCampaignObject(), self.vehicleInUnit, rwIndex, &x1, &y1);
                runwaypt = Airbase.brain.FindRunwayPt((Flight)self.GetCampaignObject(), self.vehicleInUnit, rwIndex, &x2, &y2);

                float cosAngle = self.platformAngles.sinsig * PtHeaderDataTable[rwIndex].sinHeading +
                                    self.platformAngles.cossig * PtHeaderDataTable[rwIndex].cosHeading;

                runwayStatsStruct* runwayStats = Airbase.brain.GetRunwayStats();
                dx = runwayStats[PtHeaderDataTable[rwIndex].runwayNum].centerX - self.XPos();
                dy = runwayStats[PtHeaderDataTable[rwIndex].runwayNum].centerY - self.YPos();

                relx = PtHeaderDataTable[rwIndex].cosHeading * dx + PtHeaderDataTable[rwIndex].sinHeading * dy;

                if (cosAngle > 0.99619F && PtHeaderDataTable[rwIndex].runwayNum == PtHeaderDataTable[rwindex].runwayNum &&
                    runwayStats[PtHeaderDataTable[rwIndex].runwayNum].halfheight + relx > 3000.0F)
                {
                    trackX = x2;
                    trackY = y2;
                    curTaxiPoint = runwaypt;
                }
                else if (relx > 0.0F)
                {
                    trackX = x1 - relx * PtHeaderDataTable[rwIndex].cosHeading;
                    trackY = y1 - relx * PtHeaderDataTable[rwIndex].sinHeading;
                    curTaxiPoint = takeoffpt;
                }
                else
                {
                    trackX = x1;
                    trackY = y1;
                    curTaxiPoint = takeoffpt;
                }
                atcstatus = tTakeRunway;
            }
            else
            {
                if (PtDataTable[curTaxiPoint].type == TakeoffPt || PtDataTable[curTaxiPoint].type == RunwayPt)
                {
                    takeoffpt = Airbase.brain.FindTakeoffPt((Flight)self.GetCampaignObject(), self.vehicleInUnit, rwIndex, &trackX, &trackY);
                }
                else
                {
                    TranslatePointData(Airbase, curTaxiPoint, &x1, &y1);
                    dx = x1 - af.x;
                    dy = y1 - af.y;
                    relx = self.platformAngles.cospsi * dx + self.platformAngles.sinpsi * dy;
                    if (relx < 0.0F)
                    {
                        ChooseNextPoint(Airbase);
                    }
                    else
                    {
                        trackX = x1;
                        trackY = y1;
                    }
                }
            }

            // Make sure ground weapon list is up to date
            SelectGroundWeapon();
        }

        public void SetTaxiPoint(int pt) { curTaxiPoint = pt; }
        public int GetTaxiPoint() { return curTaxiPoint; }
        public void UpdateTaxipoint()
        {
            if (!self.OnGround())
                return;

            ObjectiveClass* Airbase = (ObjectiveClass*)vuDatabase.Find(airbase);
            float tmpX, tmpY, dx, dy, dist, closestDist = 4000000.0F;
            int taxiPoint, i, closest = curTaxiPoint;

            SetDebugLabel(Airbase);

            taxiPoint = GetPrevPtLoop(curTaxiPoint);
            for (i = 0; i < 3; i++)
            {
                TranslatePointData(Airbase, taxiPoint, &tmpX, &tmpY);
                dx = tmpX - af.x;
                dy = tmpY - af.y;
                dist = dx * dx + dy * dy;
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = taxiPoint;
                }
                taxiPoint = GetNextPtLoop(taxiPoint);
            }

            if (closest != curTaxiPoint)
            {
                if (closest == GetNextPtLoop(curTaxiPoint) && self.AutopilotType() == AircraftClass.APOff)
                {
                    if (IsSetATC(CheckTaxiBack) && atcstatus != tTaxiBack)
                    {
                        atcstatus = tTaxiBack;
                        SendATCMsg(atcstatus);
                    }
                    else
                        SetATCFlag(CheckTaxiBack);
                }
                else if (closest == GetPrevPtLoop(curTaxiPoint) && atcstatus == tTaxiBack)
                {
                    if (IsSetATC(PermitRunway))
                        atcstatus = tTakeRunway;
                    else
                        atcstatus = tTaxi;
                    SendATCMsg(atcstatus);
                    ClearATCFlag(CheckTaxiBack);
                }
                curTaxiPoint = closest;
                TranslatePointData(Airbase, curTaxiPoint, &trackX, &trackY);
                CalculateTaxiSpeed(5.0F);
                waittimer = CalcWaitTime(Airbase.brain);
            }
        }

        public AircraftClass* GetLeader()
        {
            VuListIterator cit = new VuListIterator(self.GetCampaignObject().GetComponents());
            VuEntity* cur;
            VuEntity* prev = null;

            cur = cit.GetFirst();
            while (cur)
            {
                if (cur == self)
                {
                    if (prev)
                        return ((AircraftClass*)prev);
                    else
                        return null;
                }
                prev = cur;

                cur = cit.GetNext();
            }
            return null;
        }


        public int FindDesiredTaxiPoint(ulong takeoffTime)
        {
            int time_til_takeoff = 0, tp, prevPt;			// in 15 second blocks

            if (takeoffTime > SimLibElapsedTime)
                time_til_takeoff = (takeoffTime - Camp_GetCurrentTime()) / (TAKEOFF_TIME_DELTA);
            if (time_til_takeoff < 0)
                time_til_takeoff = 0;
            tp = GetFirstPt(rwIndex);
            prevPt = tp;
            tp = GetNextPt(tp);
            while (tp && time_til_takeoff)
            {
                prevPt = tp;
                tp = GetNextTaxiPt(tp);

                if (PtDataTable[tp].type == CritTaxiPt)
                    break;

                time_til_takeoff--;
            }
            if (tp)
                return tp;
            else
                return prevPt;
        }

        public void GetTrackPoint(float* x, float* y, float* z)
        {
            *x = trackX;
            *y = trackY;
            *z = trackZ;
        }

        public void SetTrackPoint(float x, float y, float z)
        {
            trackX = x;
            trackY = y;
            trackZ = z;
        }


        public void ChooseNextPoint(ObjectiveClass* Airbase)
        {
            desiredSpeed = 0.0F;
            AircraftClass* leader = null;
            int minPoint = GetFirstPt(rwIndex);
            //we arrived at our point what next?
            if (isWing)
            {
                leader = (AircraftClass*)self.GetCampaignObject().GetComponentNumber(self.vehicleInUnit - 1);
                if (leader && leader.OnGround() && leader.DBrain().ATCStatus() != tTaxiBack)
                    minPoint = leader.DBrain().GetTaxiPoint();

                if (flightLead && flightLead.OnGround() &&
                    ((AircraftClass*)flightLead).DBrain().GetTaxiPoint() > minPoint &&
                    ((AircraftClass*)flightLead).DBrain().ATCStatus() != tTaxiBack)
                    minPoint = ((AircraftClass*)flightLead).DBrain().GetTaxiPoint();
            }
            else if (SimLibElapsedTime < waittimer && !IsSetATC(PermitRunway) && !IsSetATC(PermitTakeRunway))
                return;

            switch (PtDataTable[GetPrevPtLoop(curTaxiPoint)].type)
            {
                case LargeParkPt:
                case SmallParkPt: // skip these on taxi
                    if (isWing && GetPrevPtLoop(curTaxiPoint) <= minPoint)
                        return;
                    else
                    {
                        //just taxi along
                        int pt = GetPrevTaxiPt(curTaxiPoint);
                        if (pt == 0) return;
                        curTaxiPoint = pt;
                        TranslatePointData(Airbase, curTaxiPoint, &trackX, &trackY);
                        CalculateTaxiSpeed(5.0F);
                        waittimer = CalcWaitTime(Airbase.brain);
                    }
                    break;

                default:
                case CritTaxiPt:
                case TaxiPt:
                    if (isWing && GetPrevPtLoop(curTaxiPoint) <= minPoint)
                        return;
                    //just taxi along
                    curTaxiPoint = GetPrevPtLoop(curTaxiPoint);
                    TranslatePointData(Airbase, curTaxiPoint, &trackX, &trackY);
                    CalculateTaxiSpeed(5.0F);
                    waittimer = CalcWaitTime(Airbase.brain);
                    break;

                case TakeRunwayPt:
                    //take runway if we have permission, else holdshort
                    if (isWing)
                    {
                        if (GetPrevPtLoop(curTaxiPoint) == minPoint && !IsSetATC(PermitRunway))
                            return;
                        if (GetPrevPtLoop(curTaxiPoint) < minPoint)
                            return;
                        if (WingmanTakeRunway(Airbase, (AircraftClass*)flightLead, leader))
                        {
                            curTaxiPoint = GetPrevPtLoop(curTaxiPoint);
                            TranslatePointData(Airbase, curTaxiPoint, &trackX, &trackY);
                            CalculateTaxiSpeed(5.0F);
                            waittimer = CalcWaitTime(Airbase.brain);
                        }
                        else if (!Airbase.brain.IsOnRunway(GetPrevPtLoop(GetPrevPtLoop(curTaxiPoint))))
                        {
                            curTaxiPoint = GetPrevPtLoop(GetPrevPtLoop(curTaxiPoint));
                            TranslatePointData(Airbase, curTaxiPoint, &trackX, &trackY);
                            CalculateTaxiSpeed(3.0F);
                            waittimer = CalcWaitTime(Airbase.brain);
                        }
                        else if (self.af.IsSet(AirframeClass.OverRunway))
                        {
                            OffsetTrackPoint(50.0F, rightRunway);
                            CalculateTaxiSpeed(5.0F);
                        }
                    }
                    else if (IsSetATC(PermitRunway) && !self.IsSetFalcFlag(FEC_HOLDSHORT))
                    {
                        atcstatus = tTakeRunway;
                        curTaxiPoint = GetPrevPtLoop(curTaxiPoint);
                        TranslatePointData(Airbase, curTaxiPoint, &trackX, &trackY);
                        CalculateTaxiSpeed(5.0F);
                        waittimer = CalcWaitTime(Airbase.brain);
                    }
                    else if (IsSetATC(PermitTakeRunway) && !self.IsSetFalcFlag(FEC_HOLDSHORT))
                    {
                        int pt = GetPrevPtLoop(curTaxiPoint);
                        float tempX, tempY;
                        TranslatePointData(Airbase, pt, &tempX, &tempY);
                        if (!Airbase.brain.IsOnRunway(tempX, tempY))
                        {
                            curTaxiPoint = pt;
                            trackX = tempX;
                            trackY = tempY;
                            CalculateTaxiSpeed(5.0F);
                            waittimer = CalcWaitTime(Airbase.brain);
                        }
                    }
                    else if (PtDataTable[curTaxiPoint].type == TakeRunwayPt && !self.IsSetFalcFlag(FEC_HOLDSHORT))
                    {
                        SetATCFlag(PermitTakeRunway);
                        curTaxiPoint = GetPrevPtLoop(curTaxiPoint);
                        TranslatePointData(Airbase, curTaxiPoint, &trackX, &trackY);
                        CalculateTaxiSpeed(5.0F);
                        if (atcstatus != tTakeRunway)
                        {
                            SendATCMsg(tPrepToTakeRunway);
                            atcstatus = tTaxi;
                        }
                        waittimer = CalcWaitTime(Airbase.brain);
                    }
                    else if (!Airbase.brain.IsOnRunway(GetPrevPtLoop(GetPrevPtLoop(curTaxiPoint))))
                    {
                        curTaxiPoint = GetPrevPtLoop(GetPrevPtLoop(curTaxiPoint));
                        TranslatePointData(Airbase, curTaxiPoint, &trackX, &trackY);
                        CalculateTaxiSpeed(3.0F);
                        waittimer = CalcWaitTime(Airbase.brain);
                    }
                    else if (self.af.IsSet(AirframeClass.OverRunway))
                    {
                        OffsetTrackPoint(50.0F, rightRunway);
                        CalculateTaxiSpeed(5.0F);
                    }
                    else
                    {
                        if (atcstatus != tTakeRunway && !self.IsSetFalcFlag(FEC_HOLDSHORT))
                        {
                            atcstatus = tHoldShort;
                            SendATCMsg(atcstatus);
                        }
                    }
                    break;

                case TakeoffPt:
                    //take runway if we have permission, else holdshort
                    if (isWing)
                    {
                        if (GetPrevPtLoop(curTaxiPoint) < minPoint)
                            return;

                        if (WingmanTakeRunway(Airbase, (AircraftClass*)flightLead, leader))
                        {
                            curTaxiPoint = Airbase.brain.FindTakeoffPt((Flight)self.GetCampaignObject(), self.vehicleInUnit, rwIndex, &trackX, &trackY);
                            CalculateTaxiSpeed(5.0F);
                            if (atcstatus != tTakeRunway)
                            {
                                atcstatus = tTakeRunway;
                                SendATCMsg(atcstatus);
                            }
                            waittimer = CalcWaitTime(Airbase.brain);
                        }
                        else if (self.af.IsSet(AirframeClass.OverRunway))
                        {
                            OffsetTrackPoint(50.0F, rightRunway);
                            CalculateTaxiSpeed(5.0F);
                        }
                    }
                    else if (IsSetATC(PermitRunway) && !self.IsSetFalcFlag(FEC_HOLDSHORT))
                    {
                        SetATCFlag(PermitRunway);
                        curTaxiPoint = Airbase.brain.FindTakeoffPt((Flight)self.GetCampaignObject(), self.vehicleInUnit, rwIndex, &trackX, &trackY);
                        CalculateTaxiSpeed(5.0F);
                        if (atcstatus != tTakeRunway)
                        {
                            atcstatus = tTakeRunway;
                            SendATCMsg(atcstatus);
                        }
                        waittimer = CalcWaitTime(Airbase.brain);
                    }
                    else if (self.af.IsSet(AirframeClass.OverRunway))
                    {
                        OffsetTrackPoint(50.0F, rightRunway);
                        CalculateTaxiSpeed(5.0F);
                    }
                    else
                    {
                        if (PtDataTable[curTaxiPoint].type != TakeRunwayPt && !IsSetATC(PermitTakeRunway) &&
                            !self.IsSetFalcFlag(FEC_HOLDSHORT))
                        {
                            atcstatus = tHoldShort;
                            SendATCMsg(atcstatus);
                        }
                    }
                    break;

                case RunwayPt:
                    if (isWing && !WingmanTakeRunway(Airbase, (AircraftClass*)flightLead, leader) && self.af.IsSet(AirframeClass.OverRunway))
                    {
                        OffsetTrackPoint(50.0F, rightRunway);
                        CalculateTaxiSpeed(5.0F);
                        break;
                    }
                    else if (!isWing && !IsSetATC(PermitRunway))
                    {
                        if (self.af.IsSet(AirframeClass.OverRunway))
                        {
                            OffsetTrackPoint(50.0F, rightRunway);
                            CalculateTaxiSpeed(5.0F);
                        }
                        break;
                    }
                    //takeoff and get out of the way
                    SetATCFlag(PermitRunway);
                    if (atcstatus != tTakeRunway && atcstatus != tTakeoff)
                    {
                        atcstatus = tTakeRunway;
                    }
                    curTaxiPoint = Airbase.brain.FindRunwayPt((Flight)self.GetCampaignObject(), self.vehicleInUnit, rwIndex, &trackX, &trackY);
                    desiredSpeed = 30.0F * KNOTS_TO_FTPSEC;
                    waittimer = CalcWaitTime(Airbase.brain);
                    break;
            }
        }

        public void DealWithBlocker(SimBaseClass* inTheWay, ObjectiveClass* Airbase)
        {
            float tmpX = 0.0F, tmpY = 0.0F, ry = 0.0F;
            int extraWait = 0;

            SimBaseClass* inTheWay2 = null;

            desiredSpeed = 0.0F;

            if (inTheWay.GetCampaignObject() == self.GetCampaignObject() && self.vehicleInUnit > ((AircraftClass*)inTheWay).vehicleInUnit)
            {
                return;//we never taxi around fellow flight members
            }

            switch (PtDataTable[curTaxiPoint].type)
            {
                case TakeoffPt:
                    curTaxiPoint = Airbase.brain.FindTakeoffPt((Flight)self.GetCampaignObject(), self.vehicleInUnit, rwIndex, &tmpX, &tmpY);
                    break;
                case RunwayPt:
                    curTaxiPoint = Airbase.brain.FindRunwayPt((Flight)self.GetCampaignObject(), self.vehicleInUnit, rwIndex, &trackX, &trackY);
                    break;

                default:
                case TaxiPt:
                    TranslatePointData(Airbase, curTaxiPoint, &tmpX, &tmpY);
                    break;
            }

            if (tmpX != trackX || tmpY != trackY)
                inTheWay2 = CheckPoint(tmpX, tmpY);
            else
                inTheWay2 = inTheWay;

            if (atcstatus != tTakeRunway)
            {
                extraWait = FalconLocalGame.rules.AiPatience;

                if (rwtime > SimLibElapsedTime)
                    extraWait += (rwtime - SimLibElapsedTime) / 10;
            }

            if (!inTheWay2 && (!inTheWay.IsAirplane() || ((AircraftClass*)inTheWay).DBrain().RwTime() > rwtime))
            {
                trackX = tmpX;
                trackY = tmpY;
                waittimer = CalcWaitTime(Airbase.brain);
                if (SimLibElapsedTime > waittimer + TAKEOFF_TIME_DELTA)
                    CalculateTaxiSpeed(3.0F);
                else
                    CalculateTaxiSpeed(5.0F);
            }
            else if ((isWing && (!inTheWay2 || inTheWay2.GetCampaignObject() != self.GetCampaignObject() || self.vehicleInUnit < ((AircraftClass*)inTheWay2).vehicleInUnit)) ||
                    (inTheWay.Vt() < 5.0F && SimLibElapsedTime > waittimer + extraWait) ||
                    (inTheWay.IsAirplane() && ((AircraftClass*)inTheWay).DBrain().RwTime() > rwtime))
            {
                tmpX = inTheWay.XPos() - self.XPos();
                tmpY = inTheWay.YPos() - self.YPos();
                ry = self.dmx[1][0] * tmpX + self.dmx[1][1] * tmpY;

                switch (PtDataTable[GetPrevPtLoop(curTaxiPoint)].type)
                {
                    default:
                    case CritTaxiPt:
                    case TaxiPt:
                        while (CheckTaxiTrackPoint() == inTheWay)
                        {
                            if (ry > 0.0F)
                                OffsetTrackPoint(10.0F, offLeft);
                            else
                                OffsetTrackPoint(10.0F, offRight);
                        }
                        CalculateTaxiSpeed(5.0F);
                        break;

                    case TakeRunwayPt:
                        //take runway if we have permission, else holdshort
                        if (isWing || IsSetATC(PermitRunway) || IsSetATC(PermitTakeRunway))
                        {
                            while (CheckTaxiTrackPoint() == inTheWay)
                            {
                                if (ry > 0.0F)
                                    OffsetTrackPoint(10.0F, offLeft);
                                else
                                    OffsetTrackPoint(10.0F, offRight);
                            }
                            CalculateTaxiSpeed(3.0F);
                        }
                        else if (PtDataTable[curTaxiPoint].type == TakeRunwayPt)
                        {
                            while (CheckTaxiTrackPoint() == inTheWay)
                            {
                                if (ry > 0.0F)
                                    OffsetTrackPoint(10.0F, offLeft);
                                else
                                    OffsetTrackPoint(10.0F, offRight);
                            }
                            CalculateTaxiSpeed(3.0F);
                        }
                        break;

                    case RunwayPt:
                    case TakeoffPt:
                        //take runway if we have permission, else holdshort
                        if (isWing || IsSetATC(PermitRunway))
                        {
                            while (CheckTaxiTrackPoint() == inTheWay)
                                OffsetTrackPoint(20.0F, downRunway);
                            CalculateTaxiSpeed(3.0F);
                        }
                        break;
                }
            }
        }

        public int WingmanTakeRunway(ObjectiveClass* Airbase, AircraftClass* flightLead, AircraftClass* leader)
        {
            int pt;
            float tempX, tempY;
            //when this function is called, I already know that the point will not move me past any wingmen in front
            //of me unless they are gone or off the ground

            switch (self.vehicleInUnit)
            {
                case 0:
                    ShiWarning("This should never happen");
                    return true;
                    break;

                case 1:
                    pt = GetPrevPtLoop(curTaxiPoint);

                    TranslatePointData(Airbase, pt, &tempX, &tempY);
                    if (!Airbase.brain.IsOnRunway(tempX, tempY))
                        return true;
                    else if (!FlightLead)
                        return true;
                    else if (!FlightLead.OnGround())
                        return true;
                    else if (Airbase.brain.IsOnRunway(FlightLead) && Airbase.brain.UseSectionTakeoff((Flight)self.GetCampaignObject(), rwIndex))
                        return true;
                    break;

                case 2:
                    pt = GetPrevPtLoop(curTaxiPoint);

                    TranslatePointData(Airbase, pt, &tempX, &tempY);
                    if (!Airbase.brain.IsOnRunway(tempX, tempY))
                        return true;

                    if (FlightLead && FlightLead.OnGround())
                        return false;

                    if (leader && leader.OnGround())
                        return false;

                    return true;
                    break;

                default:
                case 3:
                    pt = GetPrevPtLoop(curTaxiPoint);

                    TranslatePointData(Airbase, pt, &tempX, &tempY);
                    if (!Airbase.brain.IsOnRunway(tempX, tempY))
                        return true;

                    if (FlightLead && FlightLead.OnGround())
                        return false;

                    FlightLead = (AircraftClass*)self.GetCampaignObject().GetComponentNumber(1);
                    if (FlightLead && FlightLead.OnGround())
                        return false;

                    if (!leader)
                        return true;
                    else if (!leader.OnGround())
                        return true;
                    else if (Airbase.brain.IsOnRunway(leader) && Airbase.brain.UseSectionTakeoff((Flight)self.GetCampaignObject(), rwIndex))
                        return true;
                    break;
            }

            return false;
        }


        public int WingmanTakeRunway(ObjectiveClass* Airbase)
        {
            AircraftClass* leader = (AircraftClass*)self.GetCampaignObject().GetComponentNumber(self.vehicleInUnit - 1);

            return WingmanTakeRunway(Airbase, (AircraftClass*)flightLead, leader);
        }

        public void OffsetTrackPoint(float dist, int dir)
        {
            float dx = 0.0F, dy = 0.0F, dist = 0.0F, relx = 0.0F, x1 = 0.0F, y1 = 0.0F;
            float cosHdg = 1.0F, sinHdg = 0.0F;
            int point = 0;
            float tmpX = 0.0F, tmpY = 0.0F;
            ObjectiveClass* Airbase = null;
            runwayStatsStruct* runwayStats = null;

            if (dir == centerRunway)
            {
                Airbase = (ObjectiveClass*)vuDatabase.Find(airbase);
                if (Airbase)
                {
                    int queue = GetQueue(rwIndex);
                    runwayStats = Airbase.brain.GetRunwayStats();
                    float length = runwayStats[queue].halfheight;
                    //TranslatePointData(Airbase, pt, &x1, &y1);
                    x1 = runwayStats[queue].centerX;
                    y1 = runwayStats[queue].centerY;

                    dx = x1 - self.XPos();
                    dy = y1 - self.YPos();

                    relx = (PtHeaderDataTable[rwIndex].cosHeading * dx +
                                PtHeaderDataTable[rwIndex].sinHeading * dy);

                    relx = max(min(relx, length - TAXI_CHECK_DIST), 0.0F);

                    trackX = x1 - relx * PtHeaderDataTable[rwIndex].cosHeading;
                    trackY = y1 - relx * PtHeaderDataTable[rwIndex].sinHeading;
                }
                return;
            }

            dx = trackX - self.XPos();
            dy = trackY - self.YPos();
            dist = (float)sqrt(dx * dx + dy * dy);

            //these are cos and sin of hdg to offset point along
            switch (dir)
            {
                case offForward: //forward
                    cosHdg = dx / dist;
                    sinHdg = dy / dist;
                    break;
                case offRight: //right
                    cosHdg = -dy / dist;
                    sinHdg = dx / dist;
                    break;
                case offBack: //back
                    cosHdg = -dx / dist;
                    sinHdg = -dy / dist;
                    break;
                case offLeft: //left
                    cosHdg = dy / dist;
                    sinHdg = -dx / dist;
                    break;
                case downRunway:
                    cosHdg = PtHeaderDataTable[rwIndex].cosHeading;
                    sinHdg = PtHeaderDataTable[rwIndex].sinHeading;
                    break;
                case upRunway:
                    cosHdg = -PtHeaderDataTable[rwIndex].cosHeading;
                    sinHdg = -PtHeaderDataTable[rwIndex].sinHeading;
                    break;
                case rightRunway:
                    cosHdg = -PtHeaderDataTable[rwIndex].sinHeading;
                    sinHdg = PtHeaderDataTable[rwIndex].cosHeading;
                    break;
                case leftRunway:
                    cosHdg = PtHeaderDataTable[rwIndex].sinHeading;
                    sinHdg = -PtHeaderDataTable[rwIndex].cosHeading;
                    break;
                case taxiLeft:
                    Airbase = (ObjectiveClass*)vuDatabase.Find(airbase);
                    if (Airbase)
                    {
                        point = GetPrevPtLoop(curTaxiPoint);
                        TranslatePointData(Airbase, point, &tmpX, &tmpY);
                        dx = tmpX - trackX;
                        dy = tmpY - trackY;
                        dist = (float)sqrt(dx * dx + dy * dy);
                        cosHdg = dy / dist;
                        sinHdg = -dx / dist;
                    }
                    break;
                case taxiRight:
                    Airbase = (ObjectiveClass*)vuDatabase.Find(airbase);
                    if (Airbase)
                    {
                        if (PtDataTable[curTaxiPoint].type == RunwayPt)
                            point = GetNextPtLoop(curTaxiPoint);
                        else
                            point = GetPrevPtLoop(curTaxiPoint);
                        TranslatePointData(Airbase, point, &tmpX, &tmpY);
                        dx = tmpX - trackX;
                        dy = tmpY - trackY;
                        dist = (float)sqrt(dx * dx + dy * dy);
                        cosHdg = -dy / dist;
                        sinHdg = dx / dist;
                    }
                    break;
            }

            trackX = trackX + cosHdg * offDist;
            trackY = trackY + sinHdg * offDist;
        }

        public void ResetTimer(int delta);
        public void SetATCFlag(int flag) { atcFlags |= flag; }
        public void ClearATCFlag(int flag) { atcFlags &= ~flag; }
        public int IsSetATC(int flag) { return (atcFlags & flag) && true; }
        public void ResetATC()
        {
            SetATCStatus(noATC);
            if (!(moreFlags & NewHomebase))	// we set a new airbase to head to (for example because of fumes fuel . Actions.cpp)
                airbase = self.HomeAirbase();
            rwIndex = 0;
            rwtime = 0;
            waittimer = 0;
            curTaxiPoint = 0;
            desiredSpeed = 0;
            turnDist = 0;
#if DAVE_DBG
	SetLabel(self);
#endif
        }
        public void FlightMemberWantsFuel(int state);

        public enum ATCFlags
        {
            Landed = 0x01,
            PermitRunway = 0x02,
            PermitTakeoff = 0x04,
            HoldShort = 0x08,
            EmerStop = 0x10,
            TakeoffAborted = 0x20,
            MissionCanceled = 0x40,
            RequestTakeoff = 0x80,
            Refueling = 0x100,
            NeedToRefuel = 0x200,
            ClearToLand = 0x400,
            PermitTakeRunway = 0x800,
            WingmanReady = 0x1000,
            AceGunsEngage = 0x2000,
            SaidJoker = 0x4000,
            SaidBingo = 0x8000,
            SaidFumes = 0x10000,
            SaidFlameout = 0x20000,
            HasTrainable = 0x40000,
            FireTrainable = 0x80000,
            AskedToEngage = 0x100000,
            ReachedIP = 0x200000,
            HasAGWeapon = 0x400000,
            OnSweep = 0x800000,
            InShootShoot = 0x1000000,
            CheckTaxiBack = 0x2000000,
            WaitingPermission = 0x4000000,
            StopPlane = 0x8000000,
            SaidRTB = 0x10000000,
            // 2001-08-31 MODIFIED BY S.G. I USED 0x80000000 BUT JULIAN ALREADY USED IT. InhibitDefensive IS NOT USED ANYMORE SO I'LL REUSE IT INSTEAD
            //	   InhibitDefensive  = 0x20000000,
            HasCanUseAGWeapon = 0x20000000,
            // 2000-11-17 ADDED BY S.G. SO AI WILL WAIT FOR TARGET TO BE IN RANGE BEFORE ASKING TO ENGAGE
            WaitForTarget = 0x40000000,
            DonePreflight = 0x80000000 // JPO - done all preflight checks
        }

        public enum MoreFlags
        {
            KeepTryingRejoin = 0x00000001,
            KeepTryingAttack = 0x00000002,
            KeepLasing = 0x00000004,
            SaidImADot = 0x00000008,	// MN 
            NewHomebase = 0x00000010,	// MN
            SaidSunrise = 0x00000020,	// MN
            HUDSetup = 0x00000040
        }

        public void SendATCMsg(AtcStatusEnum msg)
        {
#if DAVE_DBG

	//MonoPrint("From Digi: Aircraft: %p  Wingman: %p  Status: %d\n", self, MyWingman(), (int)atcstatus);
#endif
            //atcstatus = msg;
            //hack so we don't send atc messages to taskforces
            CampBaseClass* atc = (CampBaseClass*)vuDatabase.Find(airbase);
            if (!atc || !atc.IsObjective())
                return;

            FalconATCMessage* ATCMessage;
            if (g_bMPFix)
                ATCMessage = new FalconATCMessage(airbase, (VuTargetEntity*)vuDatabase.Find(vuLocalSessionEntity.Game().OwnerId()));
            else
                ATCMessage = new FalconATCMessage(airbase, FalconLocalGame);

            ATCMessage.dataBlock.from = self.Id();
            ATCMessage.dataBlock.status = (short)msg;

            switch (msg)
            {
                case lReqClearance:
                    ATCMessage.dataBlock.type = FalconATCMessage.RequestClearance;
                    break;
                case lTakingPosition:
                    ATCMessage.dataBlock.type = FalconATCMessage.ContactApproach;
                    break;
                case lReqEmerClearance:
                    ATCMessage.dataBlock.type = FalconATCMessage.RequestEmerClearance;
                    break;
                case tReqTaxi:
                    ATCMessage.dataBlock.type = FalconATCMessage.RequestTaxi;
                    break;
                case tReqTakeoff:
                    ATCMessage.dataBlock.type = FalconATCMessage.RequestTakeoff;
                    break;

                case tEmerStop:
                case lAborted:
                case lIngressing:
                case lHolding:
                case lFirstLeg:
                case lToBase:
                case lToFinal:
                case lOnFinal:
                case lLanded:
                case lTaxiOff:
                case lEmerHold:
                case lEmergencyToBase:
                case lEmergencyToFinal:
                case lEmergencyOnFinal:
                case lCrashed:
                case tTaxi:
                case tHoldShort:
                case tPrepToTakeRunway:
                case tTakeRunway:
                case tTakeoff:
                case tFlyOut:
                case noATC:
                case tTaxiBack:
                    ATCMessage.dataBlock.type = FalconATCMessage.UpdateStatus;
                    break;
                default:
                    //we shouldn't get here
                    ShiWarning("Sending unknown ATC message type");
            }

            FalconSendMessage(ATCMessage, true);
        }

        public void SetATCStatus(AtcStatusEnum status) { atcstatus = status; }
        public AtcStatusEnum ATCStatus() { return atcstatus; }
        public void SetWaitTimer(VU_TIME timer) { waittimer = timer; }
        public VU_TIME WaitTime() { return waittimer; }

        public void SetRunwayInfo(VU_ID Airbase, int rwindex, ulong time)
        {
            airbase = Airbase;
            rwIndex = rwindex;
            rwtime = time;


            if (self == SimDriver.playerEntity)
            { // vwf
                gNavigationSys.SetIlsData(airbase, rwIndex);
            }
        }

        public VU_ID Airbase() { return airbase; }
        public int Runway() { return rwIndex; }
        public VU_TIME RwTime() { return rwtime; }
        public float CalculateNextTurnDistance()
        {
            float curHeading = 0.0F, newHeading = 0.0F, cosAngle = 1.0F, deltaHdg = 0.0F;
            float baseX = 0.0F, baseY = 0.0F, finalX = 0.0F, finalY = 0.0F, dx = 0.0F, dy = 0.0F, vt = 0.0F;
            ObjectiveClass* Airbase = (ObjectiveClass*)vuDatabase.Find(airbase);

            turnDist = 500.0F;

            if (Airbase && Airbase.brain)
            {
                //vt = sqrt(self.XDelta() * self.XDelta() + self.YDelta() * self.YDelta());
                vt = af.MinVcas() * KNOTS_TO_FTPSEC;

                cosAngle = Airbase.brain.DetermineAngle(self, rwIndex, atcstatus);

                switch (atcstatus)
                {
                    case lFirstLeg:
                        dx = self.XPos() - trackX;
                        dy = self.YPos() - trackY;
                        curHeading = (float)atan2(dy, dx);
                        if (curHeading < 0.0F)
                            curHeading += PI * 2.0F;

                        Airbase.brain.FindFinalPt(self, rwIndex, &finalX, &finalY);
                        if (cosAngle < 0.0F)
                        {
                            Airbase.brain.FindBasePt(self, rwIndex, finalX, finalY, &baseX, &baseY);
                            dx = trackX - baseX;
                            dy = trackY - baseY;
                        }
                        else
                        {
                            dx = trackX - finalX;
                            dy = trackY - finalY;
                        }
                        newHeading = (float)atan2(dy, dx);
                        if (newHeading < 0.0F)
                            newHeading += PI * 2.0F;

                        deltaHdg = newHeading - curHeading;
                        if (deltaHdg > PI)
                            deltaHdg = -(deltaHdg - PI);
                        else if (deltaHdg < -PI)
                            deltaHdg = -(deltaHdg + PI);
                        turnDist = (float)fabs(deltaHdg * 12.15854203708F * vt);
                        break;

                    case lToBase:
                        dx = self.XPos() - trackX;
                        dy = self.YPos() - trackY;
                        curHeading = (float)atan2(dy, dx);
                        if (curHeading < 0.0F)
                            curHeading += PI * 2.0F;

                        Airbase.brain.FindFinalPt(self, rwIndex, &finalX, &finalY);
                        dx = trackX - finalX;
                        dy = trackY - finalY;
                        newHeading = (float)atan2(dy, dx);
                        if (newHeading < 0.0F)
                            newHeading += PI * 2.0F;

                        deltaHdg = newHeading - curHeading;
                        if (deltaHdg > PI)
                            deltaHdg = -(deltaHdg - PI);
                        else if (deltaHdg < -PI)
                            deltaHdg = -(deltaHdg + PI);
                        turnDist = (float)fabs(deltaHdg * 12.15854203708F * vt);
                        break;

                    case lToFinal:
                    case lOnFinal:
                        dx = self.XPos() - trackX;
                        dy = self.YPos() - trackY;
                        curHeading = (float)atan2(dy, dx);
                        if (curHeading < 0.0F)
                            curHeading += PI * 2.0F;

                        newHeading = PtHeaderDataTable[rwIndex].data * DTR;

                        deltaHdg = newHeading - curHeading;
                        if (deltaHdg > PI)
                            deltaHdg = -(deltaHdg - PI);
                        else if (deltaHdg < -PI)
                            deltaHdg = -(deltaHdg + PI);
                        turnDist = (float)fabs(deltaHdg * 12.15854203708F * vt);
                        if (turnDist < 4000.0F)
                            turnDist = 4000.0F;
                        break;
                }
                turnDist += (0.5F * vt);
            }

            if (self.IsPlayer())
            {
                if (turnDist < 400.0F)
                    turnDist = 400.0F;
            }
            else if (turnDist < 300.0F)
            {
                turnDist = 300.0F;
            }

            return turnDist;
        }


        public float TurnDistance() { return turnDist; }
        public int ReadyToGo()
        {
            int retval = false, runway;
            FlightClass* flight = (FlightClass*)self.GetCampaignObject();
            ObjectiveClass* Airbase = (ObjectiveClass*)vuDatabase.Find(airbase);
            AircraftClass* wingman = null;

            if (!Airbase)
                return true;

            if (!isWing && !IsSetATC(PermitTakeoff))
                return false;

            if ((!isWing || self.vehicleInUnit == 2) &&
                rwtime + WINGMAN_WAIT_TIME < SimLibElapsedTime &&
                waittimer <= SimLibElapsedTime)
            {
                retval = true;
            }
            else if (self.af.vt < 2.0F && waittimer <= SimLibElapsedTime)
            {
                if (Airbase.brain.UseSectionTakeoff(flight, rwIndex))
                {
                    wingman = (AircraftClass*)MyWingman();

                    if (wingman && Airbase.brain)
                    {
                        runway = Airbase.brain.IsOnRunway(wingman);
                        if (!wingman.OnGround())
                            retval = true;
                        else if (wingman.af.vt > 40.0F * KNOTS_TO_FTPSEC && (runway == rwIndex || Airbase.brain.GetOppositeRunway(runway) == rwIndex))
                            retval = true;

                        if (isWing == 0 || self.vehicleInUnit == 2)
                        {
                            if (wingman.af.vt < 2.0F && (runway == rwIndex || Airbase.brain.GetOppositeRunway(runway) == rwIndex))
                                retval = true;
                        }
                    }
                    else
                        retval = true;
                }
                else
                    retval = true;
            }

            if (wingman && retval && !IsSetATC(WingmanReady))
            {
                SetATCFlag(WingmanReady);
                waittimer = CampaignSeconds + SimLibElapsedTime;

                retval = false;
            }

            return retval;
        }

        public float CalculateTaxiSpeed(float time)
        {
            ObjectiveClass* Airbase = (ObjectiveClass*)vuDatabase.Find(airbase);

            Debug.Assert(Airbase);

            float prevX, prevY, dx, dy;
            //float nextX, nextY;
            int point;
            point = GetNextPt(curTaxiPoint);

            if (point && time)
            {
                //TranslatePointData(Airbase, curTaxiPoint, &nextX, &nextY);
                TranslatePointData(Airbase, point, &prevX, &prevY);
                //dx = prevX - nextX;
                //dy = prevY - nextY;
                dx = prevX - trackX;
                dy = prevY - trackY;

                //how fast do we go to cover the distance in (time) seconds?
                desiredSpeed = (float)sqrt(dx * dx + dy * dy) / time;

                Debug.Assert(!_isnan(desiredSpeed));
            }
            else
                desiredSpeed = 5.0F * KNOTS_TO_FTPSEC;

            //no matter how late, we don't taxi at more than 30 knots or less than 5
            desiredSpeed = max(5.0F * KNOTS_TO_FTPSEC, min(desiredSpeed, 30.0F * KNOTS_TO_FTPSEC));

            return desiredSpeed;
        }

        public void StartRefueling();
        public void DoneRefueling();
        public void SetRefuelStatus(RefuelStatus newstatus) { refuelstatus = newstatus; }
        public RefuelStatus RefuelStatus() { return refuelstatus; }
        public void SetTanker(VU_ID tanker) { tankerId = tanker; }
        public void SetTnkPosition(int pos) { tnkposition = pos; }
        public VU_ID Tanker() { return tankerId; }
        public void HelpRefuel(AircraftClass* tanker);
        public Tpoint tankerRelPositioning; // JB 020311 respond to tanker "commands"
        public ulong lastBoomCommand;

        ///////////////////////////////////////////////////////////////////////
        // Begin Wingman Stuff


        // Wingman Stuff
        public float velocitySlope;
        public float velocityIntercept;
        public void SetLeader(SimBaseClass* newLead)
        {
            // edg: I've encountered some over-referencing problems that I think
            // is associated with flight lead setting.  So what I put in was a
            // check for self and not doing the reference -- shouldn;t be needed in
            // this case, right?
            if (flightLead != newLead)
            {
                if (flightLead && flightLead != self)
                    VuDeReferenceEntity(flightLead);

                flightLead = newLead;

                if (flightLead && flightLead != self)
                    VuReferenceEntity(flightLead);

                // edg: what a confusing mess... Why do we have SetLead and
                // SetLeader?!
                // anyway, overreferencing has been occurring on Sleep(), when
                // SetLeader( null ) is called.  We then called SetLead() below,
                // which resulted in a new flight lead!!
                // check for null flight lead and just return
                if (flightLead != null)
                    SetLead(flightLead == self ? true : false);
            }
        }

        public int IsMyWingman(SimBaseClass* testEntity)
        {
            return self.GetCampaignObject().GetComponentNumber(WingmanTable[self.vehicleInUnit]) == testEntity;
        }
        public int IsMyWingman(VU_ID testId)
        {
            SimBaseClass* testEntity;
            testEntity = self.GetCampaignObject().GetComponentNumber(WingmanTable[self.vehicleInUnit]);
            if (testEntity && testEntity.Id() == testId)
                return true;
            return false;
        }

        public SimBaseClass* MyWingman()
        {
            return self.GetCampaignObject().GetComponentNumber(WingmanTable[self.vehicleInUnit]);
        }

        public SimBaseClass* flightLead;
        public TransformMatrix threatAxis;
        public int pointCounter;
        public void FollowOrders();
        public void CommandFlight();
        public bool CommandTest()
        {
            int flightIdx;

            // 2000-09-21 S.G. NO NEED TO USE THIS FUNCTION CALL, isWing ALREADY HAS THAT VALUE FOR US...
            //	flightIdx		= ((FlightClass*)self.GetCampaignObject()).GetComponentIndex(self);
            flightIdx = isWing;

            // If Leader, issue orders to wingmen

            if (flightIdx == AiFlightLead || (flightIdx == AiElementLead && mSplitFlight && mpActionFlags[AI_ENGAGE_TARGET] && mCurrentManeuver == FalconWingmanMsg.WMTotalMsg))
            {	// VWF or rtb should be added
                return true;
            }
            else
            {
                return false;
            }
        }

        public float mAzErrInt;	// Azimuth error integrator

        //for tankers
        public virtual int IsTanker() { return false; }
        public virtual void InitBoom() { }

        public int mLeadGearDown;	//1 = true, 0 = false, -1 = uninitalized, -2 = waiting for message

        public enum AiActionModeTypes
        {
            AI_RTB,
            AI_LANDING,
            AI_FOLLOW_FORMATION,
            AI_ENGAGE_TARGET,
            AI_EXECUTE_MANEUVER,
            AI_USE_COMPLEX,
            AI_TOTAL_ACTION_TYPES
        }

        public enum AiSearchModeTypes
        {
            AI_SEARCH_FOR_TARGET,
            AI_MONITOR_TARGET,
            AI_FIXATE_ON_TARGET,
            AI_TOTAL_SEARCH_TYPES
        }

        public enum AiDesignatedTypes
        {
            AI_NO_DESIGNATED,
            AI_TARGET,
            AI_GROUP,
            AI_TOTAL_DESIGNATED_TYPES
        }

        public enum AiWeaponsAction
        {
            AI_WEAPONS_HOLD,
            AI_WEAPONS_FREE,
            AI_WEAPONS_ACTION_TOTAL
        }

        public enum AiThreatPosition
        {
            AI_THREAT_NONE,
            AI_THREAT_LEFT,
            AI_THREAT_RIGHT,
            AI_TOTAL_THREAT_POSITIONS
        }

        public enum AiHint
        { // JPO some hints for what were doing.
            AI_NOHINT, AI_TAKEOFF, AI_REJOIN, AI_ATTACK
        }

        public enum AiTargetType
        { // 2002-03-04 ADDED BY S.G. Need to know what kind of target we're going after...
            AI_NONE = false, AI_AIR_TARGET, AI_GROUND_TARGET
        }



        // Mode Arrays
        private int[] mpActionFlags = new int[AI_TOTAL_ACTION_TYPES]; // 2002-03-04 MODIFIED BY S.G. Used to be type bool and now needs 0, 1 or 2 so for best practice, make it an int
        private bool[] mpSearchFlags = new bool[AI_TOTAL_SEARCH_TYPES];
        private int mCurrentManeuver;

        // Assigned Target
        //	SimObjectType*		mpDesignatedTarget;
        private VU_ID mDesignatedObject;
        private int mFormation;
        // Marco Edit - for Current Formation
        private int mCurFormation;
        private bool mLeaderTookOff;

        // Mode Basis
        private AiDesignatedTypes mDesignatedType;
        private char mSearchDomain;
        private AiWeaponsAction mWeaponsAction;
        private AiWeaponsAction mSavedWeapons;

        // Saved mode Basis
        //	AiDesignatedTypes	mSaveDesignatedType;
        private char mSaveSearchDomain;
        //	AiWeaponsAction		mSaveWeaponsAction;

        private bool mInPositionFlag;

        private float mHeadingOrdered;
        private float mAltitudeOrdered;
        private float mSpeedOrdered;

        private int mPointCounter;
        private float[,] mpManeuverPoints = new float[3, 2];

        // Last time wingman gave a scheduled report
        private VU_TIME mLastReportTime;
        private SimObjectType* mpLastTargetPtr;

        //	Formation Offsets
        private bool mSplitFlight;
        private float mFormRelativeAltitude;
        private int mFormSide;
        private float mFormLateralSpaceFactor;

        private VU_TIME mNextPreflightAction;	// JPO when to do the next action
        private int mActionIndex;		// what to do.

        // Breaking the flight
        private void AiSplitFlight(int extent, VU_ID from, int idx)
        {
            if (vuDatabase.Find(from) == self.GetCampaignObject().GetComponentLead())
            {	// if from the flight lead
                if (extent == AiElement || extent == AiFlight)
                {
                    if (idx == AiElementLead || idx == AiSecondWing)
                    {
                        mSplitFlight = TRUE;
                    }
                }
            }
            else
            {	// Otherwise the order is coming from the element lead, we want to follow him
                if (extent == AiWingman && idx == AiSecondWing)
                {
                    mSplitFlight = TRUE;
                }
            }
        }


        private void AiGlueFlight(int extent, VU_ID from, int idx)
        {
            if (vuDatabase.Find(from) == self.GetCampaignObject().GetComponentLead())
            {	// if from the flight lead
                if (extent == AiElement || extent == AiFlight)
                {
                    if (idx == AiElementLead || idx == AiSecondWing)
                    {
                        mSplitFlight = FALSE;
                    }
                }
            }
            else
            {	// Otherwise the order is coming from the element lead, we want to follow him
                if (extent == AiWingman && idx == AiSecondWing)
                {
                    mSplitFlight = TRUE;
                }
            }
        }

        // Targeting
        private void AiRunTargetSelection()
        {
            VuEntity* pnewTarget;
            FalconEntity* curSpike = SpikeCheck(self);

            // 2001-06-27 ADDED BY S.G. SO WING WILL SET THEIR ECM AS WELL
            if (curSpike || (flightLead && flightLead != self && flightLead.IsSPJamming()))
            {
                if (self.HasSPJamming())
                {
                    self.SetFlag(ECM_ON);
                }
            }
            else
            {
                self.UnSetFlag(ECM_ON);
            }
            // END OF ADDED SECTION

            if (curSpike)
            {
                SetThreat(curSpike);
            }
            else
            {
                if (mDesignatedObject != FalconNullId)
                {					// If target has been designated by the leader

                    pnewTarget = vuDatabase.Find(mDesignatedObject);	// Lookup target in database
                    if (pnewTarget)
                    {
#if NOTHING		   // Not working yet, commented out in case it creates more problem than it solves...
			   // 2002-03-15 ADDED BY S.G. Special case when everyone in the unit is dead... Should help the AI not targeting chutes when that's all is left...
			   // If it's a NON aggregated UNIT CAMPAIGN object, it SHOULD have components... If it doesn't, clear it's designated target.
			   if (((FalconEntity *)pnewTarget).IsCampaign() && ((CampBaseClass *)pnewTarget).IsUnit() && !((CampBaseClass *)pnewTarget).IsAggregate() && ((CampBaseClass *)pnewTarget).NumberOfComponents() == 0) {
				   Debug.Assert(!"Empty deaggregated object as a target?!?");
				   mDesignatedObject	= FalconNullId;
			   }
			   else
			   // END OF ADDED SECTION
#endif
                        AiSearchTargetList(pnewTarget);							// Run targeting for wingman with designated target
                    }
                    else
                    {
                        mDesignatedObject = FalconNullId;
                    }
                }
                else if (mpSearchFlags[AI_SEARCH_FOR_TARGET] ||
                            mDesignatedObject == FalconNullId)
                {			// If we are ordered to scan for targets
                    TargetSelection();											// Run the full selection routine
                }
                else
                {
                    Debug.Assert(curMode != GunsEngageMode);// Otherwise just chill out
                    ClearTarget();													// No targets of interest
                    AiRestoreWeaponState();
                    mpActionFlags[AI_ENGAGE_TARGET] = AI_NONE; // 2002-03-04 MODIFIED BY S.G. Use new enum type
                }
            }
        }

        private void AiSearchTargetList(VuEntity* pentity)
        {
            CampBaseClass* theTargetGroup = (CampBaseClass*)pentity;
            FalconEntity* theTarget = null;
            SimObjectType* objectPtr;

            if (!pentity)
                return;

            if (((FalconEntity*)pentity).IsSim())
            {
                theTarget = (FalconEntity*)pentity;
                // 2002-04-02 ADDED BY S.G. If it's dead or a chute, leave it alone...
                if (!theTarget.OnGround() && (theTarget.IsDead() || theTarget.IsEject()))
                    theTarget = null;
                // END OF ADDED SECTION 2002-04-02
            }
            else
            {
                if (theTargetGroup.NumberOfComponents())
                // 2001-06-04 MODIFIED BY S.G. NEED TO ACCOUNT FOR GROUND TARGETS DIFFERENTLY THAN AIR TARGETS
                //       theTarget = theTargetGroup.GetComponentEntity (isWing % theTargetGroup.NumberOfComponents());
                {
                    if (theTargetGroup.OnGround())
                    {
                        // If we already have a ground target and it is part of the unit we are being asked to target and it's still alive, keep using it (ie, don't switch)
                        if (groundTargetPtr && groundTargetPtr.BaseData().IsSim() && ((SimBaseClass*)groundTargetPtr.BaseData()).GetCampaignObject() == theTargetGroup && !groundTargetPtr.BaseData().IsExploding() && !groundTargetPtr.BaseData().IsDead() && ((SimBaseClass*)groundTargetPtr.BaseData()).pctStrength > 0.0f)
                            return;

                        // Use our hep function
                        theTarget = FindSimGroundTarget(theTargetGroup, theTargetGroup.NumberOfComponents(), 0);
                    }
                    // Target is not on the ground, select your opponent in the flight, wrapping if the opponent has less planes than us
                    else
                        theTarget = theTargetGroup.GetComponentEntity(isWing % theTargetGroup.NumberOfComponents());
                    // 2002-04-02 ADDED BY S.G. If it's dead or a chute, leave it alone...
                    if (theTarget && (theTarget.IsDead() || theTarget.IsEject()))
                        theTarget = null;
                    // END OF ADDED SECTION 2002-04-02
                }
                else
                    theTarget = theTargetGroup;
            }

            if (theTarget)
            {
                // 2001-05-10 MODIFIED BY S.G. IF WE HAVE A GROUND TARGET AND IT'S OUR TARGET, AVOID DOING THIS AS WELL (WE'RE FINE)
                //    if (!targetPtr || targetPtr.BaseData() != theTarget)
                if ((!targetPtr || targetPtr.BaseData() != theTarget) && (!groundTargetPtr || groundTargetPtr.BaseData() != theTarget))
                {
#if DEBUG
                    objectPtr = new SimObjectType(OBJ_TAG, self, theTarget);
#else
         objectPtr = new SimObjectType (theTarget);
#endif
                    SetTarget(objectPtr);
                    // 2000-09-18 ADDED BY S.G. SO AI STARTS SHOOTING RIGHT NOW AND STOP WAITING THAT STUPID 30 SECONDS!
                    missileShotTimer = 0;
                    // END OF ADDED SECTION
                }
            }
            else
            {
                Debug.Assert(curMode != GunsEngageMode);
                ClearTarget();
                mDesignatedObject = FalconNullId; // 2002-04-04 ADDED BY S.G. If we are clearing the target, we might as well clear the designated target as well so we leave it alone...
                AddMode(WaypointMode);
            }
        }


        // Wingman Decision Fuctions
        private void AiRunDecisionRoutines()
        {
            //if (curMode == GunsJinkMode)
            //{
            GunsJinkCheck();
            //}

            //if (curMode == MissileDefeatMode)
            //{
            MissileDefeatCheck();
            //}

            // Check if I should be landing
            Debug.Assert(flightLead);
            if (flightLead)
                AiCheckForUnauthLand(flightLead.Id());
            //AiCheckLand();

            // Check if I was ordered RTB
            AiCheckRTB();

            // Check If I was ordered to kill somthing
            AiCheckEngage();

            // Check if we have been ordered to perform a fancy maneuver
            AiCheckManeuvers();

            // Check if we should be in formation
            AiCheckFormation();


            // Always follow waypoints
            AddMode(WaypointMode);
        }

        private void AiCheckManeuvers()
        {

            if (mpActionFlags[AI_EXECUTE_MANEUVER])
            {					// If we are ordered to do a maneuver
                AddMode(FollowOrdersMode);								// Add maneuvers to stack
            }
        }


        private void AiCheckFormation()
        {
            //temp Hack until I can talk to Vince about a better way. DSP
            //	if(mpActionFlags[AI_FOLLOW_FORMATION] && self.curWaypoint.GetWPAction() != WP_LAND) {					// If we are ordered to fly in formation

            // edg: if the wingy was told to engage a ground target, we must make sure that they
            // will continue on waypoint mode so that they can go thru the ground attack logic
            if (mpActionFlags[AI_ENGAGE_TARGET] && !mpActionFlags[AI_EXECUTE_MANEUVER] && (agDoctrine != AGD_NONE || groundTargetPtr))
            {
                AddMode(WaypointMode);
            }
            else if (mpActionFlags[AI_FOLLOW_FORMATION])
            {
                AddMode(WingyMode);										// Add formation mode to stack
            }
        }

        private void AiCheckEngage()
        {
            // 2000-09-18 MODIFIED BY S.G. NEED THE WINGMEN TO DO ITS DUTY EVEN WHILE EXECUTING MANEUVERS...
            // 2000-09-25 MODIFIED BY S.G. NEED THE WINGMEN TO DO ITS STUFF WHEN HE HAS WEAPON FREE AS WELL
            // 2002-03-15 MODIFIED BY S.G. Perform this if mpActionFlags[AI_EXECUTE_MANEUVER] is NOT TRUE+1, since TRUE+1 mean we are doing a maneuver that's limiting the AI's ACTION to specific functions
            //	if(mpActionFlags[AI_ENGAGE_TARGET] /* REMOVED BY S.G. && !mpActionFlags[AI_EXECUTE_MANEUVER] */) {
            //	if(mpActionFlags[AI_ENGAGE_TARGET] || mWeaponsAction == AI_WEAPONS_FREE) {
            if ((mpActionFlags[AI_ENGAGE_TARGET] || mWeaponsAction == AI_WEAPONS_FREE) && mpActionFlags[AI_EXECUTE_MANEUVER] != TRUE + 1)
            {
                MergeCheck();
                BvrEngageCheck();
                GunsEngageCheck();
                WvrEngageCheck();
                MissileEngageCheck();
                AccelCheck();
            }
        }

        private void AiCheckRTB()
        {
            // set waypoint to home
            // check distance to home, contact tower it necessary

            if (mpActionFlags[AI_RTB])
            {									// If we are ordered to do a maneuver
                AddMode(RTBMode);								// Add maneuvers to stack

                // if(mpActionFlags[AI_LANDING] == FALSE && distance to airbase < 15 nm, && no atc) {
                // contact atc
                //	mpActionFlags[AI_LANDING] = TRUE;
                //}
            }
        }



        private void AiCheckLandTakeoff()
        {
            if (self.curWaypoint == null)
            {
                return;
            }

            if (SimLibElapsedTime > updateTime)
            {
                ObjectiveClass* Airbase = (ObjectiveClass*)vuDatabase.Find(airbase);
                if (Airbase)
                {
                    float dx = self.XPos() - Airbase.XPos();
                    float dy = self.YPos() - Airbase.YPos();
                    distAirbase = (float)sqrt(dx * dx + dy * dy);
                }
                updateTime = SimLibElapsedTime + 15 * CampaignSeconds;
            }

            if (atcstatus >= tReqTaxi && atcstatus <= tTaxiBack)
            {
                mpActionFlags[AI_FOLLOW_FORMATION] = FALSE;
                AddMode(TakeoffMode);
            }
            else if (mpActionFlags[AI_LANDING]
                || (atcstatus >= lReqClearance && atcstatus <= lCrashed))
            {
                if (atcstatus > lIngressing)
                    mpActionFlags[AI_FOLLOW_FORMATION] = FALSE;
                AddMode(LandingMode);
            }
            else if (self.curWaypoint.GetWPAction() == WP_LAND && !self.OnGround() && distAirbase < 30.0F * Phyconst.NM_TO_FT
                && (missionComplete || IsSetATC(SaidRTB) || IsSetATC(SaidBingo) || mpActionFlags[AI_RTB])) // don't land if one of these conditions isn't met
            {
                if (atcstatus > lIngressing)
                    mpActionFlags[AI_FOLLOW_FORMATION] = FALSE;
                AddMode(LandingMode);
            }
            else if (self.curWaypoint.GetWPAction() == WP_TAKEOFF && self.OnGround())
            {
                mpActionFlags[AI_FOLLOW_FORMATION] = FALSE;
                AddMode(TakeoffMode);
            }
            else if (self.OnGround())
            {
                atcstatus = lTaxiOff;
                mpActionFlags[AI_FOLLOW_FORMATION] = FALSE;
                AddMode(LandingMode);
            }
        }

        private void AiCheckForUnauthLand(VU_ID lead)
        {
            GridIndex x, y;
            vector pos;

            AircraftClass* leader = (AircraftClass*)vuDatabase.Find(lead);

            if (!self.OnGround() && leader && mpActionFlags[AI_FOLLOW_FORMATION] == TRUE && leader.DBrain().ATCStatus() < tReqTaxi && leader.OnGround() && atcstatus == noATC)
            {

                pos.x = leader.XPos();
                pos.y = leader.YPos();
                ConvertSimToGrid(&pos, &x, &y);

                Objective obj = FindNearestFriendlyAirbase(self.GetTeam(), x, y);
                if (obj)
                {
                    airbase = obj.Id();
                    atcstatus = lReqClearance;
                    SendATCMsg(atcstatus);
                    mpActionFlags[AI_FOLLOW_FORMATION] = FALSE;
                }
            }
        }


        // Wingman Actions 
        private void AiPerformManeuver();
        private void AiFollowLead();

        // Wingman Monitor for targets
        private void AiMonitorTargets();

        // Wingman Utility Functions
        private void AiSaveWeaponState()
        {
            mSavedWeapons = mWeaponsAction;
        }

        private void AiRestoreWeaponState()
        {
            mWeaponsAction = mSavedWeapons;
        }


        private void AiSaveSetSearchDomain(char domain)
        {

            mSaveSearchDomain = mSearchDomain;
            mSearchDomain = domain;
        }
        private void AiRestoreSearchDomain()
        {
            mSearchDomain = mSaveSearchDomain;
        }


        private void AiSetManeuver(int maneuver)
        {
            mpActionFlags[AI_EXECUTE_MANEUVER] = TRUE;
            mpActionFlags[AI_RTB] = FALSE;
            mCurrentManeuver = maneuver;
            mnverTime = 10.0F;
        }

        private void AiClearManeuver()
        {
            mpActionFlags[AI_USE_COMPLEX] = FALSE;
            mpActionFlags[AI_EXECUTE_MANEUVER] = FALSE;
            mpActionFlags[AI_RTB] = FALSE;
            mCurrentManeuver = FalconWingmanMsg.WMTotalMsg;
        }


        // Commands that modify Action and Search States
        private void AiClearLeadersSix(FalconWingmanMsg* msg)
        {

            int flightIdx;
            short[] edata = new short[10];
            AircraftClass* pfrom;
            AircraftClass* ptgt;
            mlTrig trig;
            float xpos;
            float ypos;
            float rz;
            int navangle;
            float angle;
            float xdiff, ydiff;
            int random;

            flightIdx = self.GetCampaignObject().GetComponentIndex(self);
            pfrom = (AircraftClass*)vuDatabase.Find(msg.dataBlock.from);

            if (msg.dataBlock.newTarget == FalconNullId)
            {

                // angles of the aircraft we are clearing
                mlSinCos(&trig, pfrom.Yaw());

                xpos = pfrom.XPos() - trig.cos * 1000.0F;	// 1000 feet behind aircraft we are clearing
                ypos = pfrom.YPos() - trig.sin * 1000.0F;

                xpos = xpos - self.XPos();
                ypos = ypos - self.YPos();

                mHeadingOrdered = (float)atan2(xpos, ypos);

                mSpeedOrdered = self.Vt() * FTPSEC_TO_KNOTS;
                mAltitudeOrdered = self.ZPos();
                mnverTime = 15.0F;

                AiSetManeuver(FalconWingmanMsg.WMClearSix);

                if (AiIsFullResponse(flightIdx, msg.dataBlock.to))
                {
                    edata[0] = flightIdx;
                    edata[1] = 2;
                    AiMakeRadioResponse(self, rcROGER, edata);
                    AiCheckFormStrip();
                }
                else
                {
                    edata[0] = -1;
                    edata[1] = -1;
                    AiMakeRadioResponse(self, rcROGER, edata);
                    AiCheckFormStrip();
                }

            }
            else
            {

                AiCheckFormStrip();

                mDesignatedObject = msg.dataBlock.newTarget;
                ptgt = (AircraftClass*)vuDatabase.Find(mDesignatedObject);

                if (ptgt && pfrom && !F4IsBadReadPtr(ptgt, sizeof(AircraftClass)) && !F4IsBadReadPtr(pfrom, sizeof(AircraftClass))) // JB 010318 CTD
                {
                    if (ptgt.ZPos() - pfrom.ZPos() < -500.0F)
                    {
                        edata[0] = 7;	// break low
                    }
                    else if (ptgt.ZPos() - pfrom.ZPos() > 500.0F)
                    {
                        edata[0] = 6; // break hi
                    }
                    else
                    {
                        random = 2 * FloatToInt32((float)rand() / (float)RAND_MAX);
                        if (random)
                        {
                            edata[0] = 0; // break hi
                        }
                        else
                        {
                            edata[0] = 3; // break hi
                        }
                    }
                    AiMakeRadioResponse(self, rcBREAK, edata);


                    mpActionFlags[AI_ENGAGE_TARGET] = AI_AIR_TARGET; // 2002-03-04 MODIFIED BY S.G. Use new enum type
                    AiSaveWeaponState();
                    mWeaponsAction = AI_WEAPONS_FREE;


                    edata[0] = 2 * (ptgt.Type() - VU_LAST_ENTITY_TYPE);

                    // convert to compass angle

                    xdiff = ptgt.XPos() - pfrom.XPos();
                    ydiff = ptgt.YPos() - pfrom.YPos();

                    angle = (float)atan2(ydiff, xdiff);
                    angle = angle - pfrom.Yaw();
                    navangle = FloatToInt32(RTD * angle);
                    if (navangle < 0)
                    {
                        navangle = 360 + navangle;
                    }

                    edata[1] = navangle / 30;									// scale compass angle for radio eData
                    if (edata[1] >= 12)
                    {
                        edata[1] = 0;
                    }

                    rz = ptgt.ZPos() - pfrom.ZPos();

                    if (rz < 300.0F && rz > -300.0F)
                    {							// check relative alt and select correct frag
                        edata[2] = 1;
                    }
                    else if (rz < -300.0F && rz > -1000.0F)
                    {
                        edata[2] = 2;
                    }
                    else if (rz < -1000.0F)
                    {
                        edata[2] = 3;
                    }
                    else
                    {
                        edata[2] = 0;
                    }


                    AiMakeRadioResponse(self, rcENGAGINGC, edata);
                }
            }
        }

        private void AiCheckOwnSix(FalconWingmanMsg* msg)
        {

            int flightIdx;
            short[] edata = new short[10];
            float az;
            VU_ID threat;
            int direction;
            AircraftClass* pfrom;

            flightIdx = self.GetCampaignObject().GetComponentIndex(self);
            pfrom = (AircraftClass*)vuDatabase.Find(msg.dataBlock.from);

            AiSaveSetSearchDomain(DOMAIN_AIR);
            threat = AiCheckForThreat(self, DOMAIN_AIR, 1, &az);

            if (threat == FalconNullId)
            {

                direction = 2 * FloatToInt32((float)rand() / (float)RAND_MAX);

                if (direction)
                {
                    AiSetManeuver(FalconWingmanMsg.WMBreakRight);
                }
                else
                {
                    AiSetManeuver(FalconWingmanMsg.WMBreakLeft);
                }


                if (AiIsFullResponse(flightIdx, msg.dataBlock.to))
                {
                    edata[0] = ((FlightClass*)self.GetCampaignObject()).callsign_id;
                    edata[1] = (((FlightClass*)self.GetCampaignObject()).callsign_num - 1) * 4 + self.GetCampaignObject().GetComponentIndex(self) + 1;
                    edata[2] = -1;
                    edata[3] = -1;
                    AiMakeRadioResponse(self, rcCOPY, edata);
                    AiCheckFormStrip();
                }
                else
                {
                    AiRespondShortCallSign(self);
                }
            }
            else
            {
                AiEngageThreatAtSix(threat);
            }

            AiRestoreSearchDomain();
        }

        private void AiEngageThreatAtSix(VU_ID threat)
        {

            short[] edata = new short[10];
            AircraftClass* ptgt;
            int navangle;
            float angle;
            float xdiff, ydiff;
            float rz;

            mDesignatedObject = threat;
            mpActionFlags[AI_ENGAGE_TARGET] = AI_AIR_TARGET; // 2002-03-04 MODIFIED BY S.G. Use new enum type
            AiSaveWeaponState();
            mWeaponsAction = AI_WEAPONS_FREE;

            AiCheckFormStrip();


            ptgt = (AircraftClass*)vuDatabase.Find(mDesignatedObject);

            if (ptgt)
            {
                edata[0] = 2 * (ptgt.Type() - VU_LAST_ENTITY_TYPE);


                xdiff = ptgt.XPos() - self.XPos();
                ydiff = ptgt.YPos() - self.YPos();

                angle = (float)atan2(ydiff, xdiff);
                angle = angle - self.Yaw();
                navangle = FloatToInt32(RTD * angle);
                if (navangle < 0)
                {
                    navangle = 360 + navangle;
                }

                edata[1] = navangle / 30;									// scale compass angle for radio eData
                if (edata[1] >= 12)
                {
                    edata[1] = 0;
                }

                rz = ptgt.ZPos() - self.ZPos();

                if (rz < 300.0F && rz > -300.0F)
                {							// check relative alt and select correct frag
                    edata[2] = 1;
                }
                else if (rz < -300.0F && rz > -1000.0F)
                {
                    edata[2] = 2;
                }
                else if (rz < -1000.0F)
                {
                    edata[2] = 3;
                }
                else
                {
                    edata[2] = 0;
                }

                AiMakeRadioResponse(self, rcENGAGINGC, edata);
            }
        }

        private void AiBreakLeft()
        {

            MonoPrint("\tin AiBreakLeft\n");

            AiSetManeuver(FalconWingmanMsg.WMBreakLeft);
            mHeadingOrdered = self.Yaw() - 90.0F * DTR;
            if (mHeadingOrdered <= -180.0F * DTR)
            {
                mHeadingOrdered += 360.0F * DTR;
            }

            mSpeedOrdered = self.Vt() * FTPSEC_TO_KNOTS;
            mAltitudeOrdered = self.ZPos();
            mnverTime = 15.0F;

        }

        private void AiBreakRight()
        {
            MonoPrint("\tin AiBreakRight\n");

            AiSetManeuver(FalconWingmanMsg.WMBreakRight);
            mHeadingOrdered = self.Yaw() + 90.0F * DTR;
            if (mHeadingOrdered > 180.0F * DTR)
            {
                mHeadingOrdered -= 360.0F * DTR;
            }

            mSpeedOrdered = self.Vt() * FTPSEC_TO_KNOTS;
            mAltitudeOrdered = self.ZPos();
            mnverTime = 15.0F;

        }

        private void AiGoShooter()
        {
            if (mpActionFlags[AI_ENGAGE_TARGET] == AI_NONE) // 2002-03-04 ADDED BY S.G. Change it if not already set, assume an air target (can't tell)
                mpActionFlags[AI_ENGAGE_TARGET] = AI_AIR_TARGET; // 2002-03-04 MODIFIED BY S.G. Use new enum type
            mpActionFlags[AI_EXECUTE_MANEUVER] = FALSE;
            mWeaponsAction = AI_WEAPONS_FREE;

            mpSearchFlags[AI_FIXATE_ON_TARGET] = TRUE;
            mpSearchFlags[AI_MONITOR_TARGET] = FALSE;
            mpActionFlags[AI_RTB] = FALSE;

            AiClearManeuver();

            //MonoPrint("Going Shooter\n");
        }

        private void AiGoCover()
        {

            mpActionFlags[AI_ENGAGE_TARGET] = AI_NONE; // 2002-03-04 MODIFIED BY S.G. Use new enum type
            mpActionFlags[AI_EXECUTE_MANEUVER] = FALSE;
            mpActionFlags[AI_RTB] = FALSE;
            mWeaponsAction = AI_WEAPONS_HOLD;

            mpSearchFlags[AI_FIXATE_ON_TARGET] = FALSE;
            mpSearchFlags[AI_MONITOR_TARGET] = TRUE;

            AiClearManeuver();
            // 2001-06-16 ADDED BY S.G. NEED TO GO BACK IN NAV MODE.
            if (self.AutopilotType() == AircraftClass.CombatAP || self.isDigital) // 2002-01-28 ADDED BY S.G But only if in CombatAP!!!
                self.FCC.SetMasterMode(FireControlComputer.Nav);
            // END OF ADDED SECTION

            //MonoPrint("Going Cover\n");
        }

        private void AiSearchForTargets(char domain)
        {
            mSearchDomain = domain;
            // 2000-09-13 MODIFIED BY S.G. PRETTY USELESS LINE IF YOU ASK ME... NOT IN RP4
            mpSearchFlags[AI_SEARCH_FOR_TARGET];
            //	mpSearchFlags[AI_SEARCH_FOR_TARGET] = TRUE;
            mLastReportTime = 0;
        }

        private void AiResumeFlightPlan(FalconWingmanMsg* msg)
        {
            int flightIdx;
            short[] edata = new short[10];

            mpActionFlags[AI_ENGAGE_TARGET] = AI_NONE; // 2002-03-04 MODIFIED BY S.G. Use new enum type
            mpActionFlags[AI_EXECUTE_MANEUVER] = FALSE;
            mpActionFlags[AI_RTB] = FALSE;

            mpSearchFlags[AI_FIXATE_ON_TARGET] = FALSE;
            mpSearchFlags[AI_MONITOR_TARGET] = FALSE;

            mDesignatedType = AI_NO_DESIGNATED;
            mWeaponsAction = AI_WEAPONS_HOLD;

            AiClearManeuver();

            flightIdx = self.GetCampaignObject().GetComponentIndex(self);
            AiGlueFlight(msg.dataBlock.to, msg.dataBlock.from, flightIdx);

            if (AiIsFullResponse(flightIdx, msg.dataBlock.to))
            {
                edata[0] = flightIdx;
                edata[1] = 1;
                AiMakeRadioResponse(self, rcROGER, edata);
            }
            else
            {
                AiRespondShortCallSign(self);
            }
        }

        private void AiRejoin(FalconWingmanMsg* msg, AiHint hint = AI_NOHINT)
        {
            short[] edata = new short[10];
            int flightIdx;

            //we can't rejoin if we're on the ground still!
            if (self.OnGround() || atcstatus >= lOnFinal)
                return;

            AiCheckPosition();
            //	mInPositionFlag = FALSE;

            mpActionFlags[AI_ENGAGE_TARGET] = AI_NONE; // 2002-03-04 MODIFIED BY S.G. Use new enum type
            mpActionFlags[AI_EXECUTE_MANEUVER] = FALSE;
            mpActionFlags[AI_FOLLOW_FORMATION] = TRUE;
            mpActionFlags[AI_RTB] = FALSE;
            mpActionFlags[AI_LANDING] = FALSE;

            SendATCMsg(noATC);
            atcstatus = noATC;
            // cancel atc here

            // 2001-07-11 ADDED BY S.G. NEED TO SET THE SAME WAYPOINT AS THE LEAD ONCE WE REJOIN...
            WayPointClass* wlistUs = self.waypoint;
            WayPointClass* wlistLead = null;
            if (flightLead)
                wlistLead = ((AircraftClass*)flightLead).waypoint;

            // This will set our current waypoint to the leads waypoint
            // 2001-10-20 Modified by M.N. Added .GetNextWP() to assure that we get a valid waypoint
            while (wlistUs.GetNextWP() && wlistLead && wlistLead.GetNextWP() && wlistLead != ((AircraftClass*)flightLead).curWaypoint)
            {
                wlistUs = wlistUs.GetNextWP();
                wlistLead = wlistLead.GetNextWP();
            }
            self.curWaypoint = wlistUs;
            // END OF ADDED SECTION

            rwIndex = 0;
            self.af.gearHandle = -1.0F; //up

            mpActionFlags[AI_USE_COMPLEX] = FALSE;

            mpSearchFlags[AI_FIXATE_ON_TARGET] = FALSE;

            AiClearManeuver();

            mFormLateralSpaceFactor = 1.0F;
            mFormSide = 1;
            mFormRelativeAltitude = 0.0F;
            mDesignatedObject = FalconNullId;

            // 2001-05-22 ADDED BY S.G. NEED TO TELL AI TO STOP THEIR GROUND ATTACK
            agDoctrine = AGD_NONE;
            SetGroundTarget(null);
            // 2001-06-30 ADDED BY S.G. WHEN REJOINING, THE HasAGWeapon IS REFLECTED INTO HasCanUseAGWeapon SO IF THE LEAD WITH ONLY HARMS FIRED THEM AT A ENROUTE TARGET, WHEN SWITCHING TO THE ATTACK TARGET, HE DOESN'T ABORT THINKING NO ONE CAN FIRE ANYTHING...
            // 2001-10-23 MODIFIED BY S.G. Only if the rejoin comes from the lead and not from yourself rejoining on your own so the lead doesn't think your HARMS can be used on the target you were bombing
            if (hint == AI_REJOIN && IsSetATC(HasAGWeapon))
                SetATCFlag(HasCanUseAGWeapon);
            // END OF ADDED SECTION

            flightIdx = self.GetCampaignObject().GetComponentIndex(self);

            // 2002-03-10 MN when ordered AI to rejoin, rejoin immediately, not only when connected to the boom!
            //	if(self.af.IsSet(Refueling)) {
            if (refuelstatus != refNoTanker && refuelstatus != refDone)
            {
                VuEntity* theTanker = vuDatabase.Find(tankerId);
                FalconTankerMessage* TankerMsg;

                if (theTanker)
                    TankerMsg = new FalconTankerMessage(theTanker.Id(), FalconLocalGame);
                else
                    TankerMsg = new FalconTankerMessage(FalconNullId, FalconLocalGame);

                TankerMsg.dataBlock.type = FalconTankerMessage.DoneRefueling;
                TankerMsg.dataBlock.data1 = 1;
                TankerMsg.dataBlock.caller = self.Id();
                FalconSendMessage(TankerMsg);
            }


            // edg: can't msg be null?  It looks like some calls use it and I
            // want to use this function to have the wingies return to formation
            // after their AG attack is done.
            if (msg)
                AiGlueFlight(msg.dataBlock.to, msg.dataBlock.from, flightIdx);

            if (msg && AiIsFullResponse(flightIdx, msg.dataBlock.to))
            {
                edata[0] = -1;
                edata[1] = -1;
                edata[2] = ((FlightClass*)self.GetCampaignObject()).callsign_id;
                edata[3] = (((FlightClass*)self.GetCampaignObject()).callsign_num - 1) * 4 + flightIdx + 1;
                AiMakeRadioResponse(self, rcONMYWAY, edata);
            }
            else if (hint == AI_TAKEOFF)
            { // JPO take the hint!
                short[] edata = new short[10];

                edata[0] = ((FlightClass*)self.GetCampaignObject()).GetComponentIndex(self);
                edata[1] = -1;
                edata[2] = -1;
                edata[3] = -1;
                edata[4] = -1;
                edata[5] = -1;
                edata[6] = -1;
                edata[7] = -1;
                edata[8] = -1;
                edata[9] = -1;
                AiMakeRadioResponse(self, rcLIFTOFF, edata);
            }
            else
            {
                AiRespondShortCallSign(self);
            }
            // 2002-02-23 MN when ordered to rejoin, AI must make weapons safe
            mWeaponsAction = AI_WEAPONS_HOLD;
        }

        private void AiSetRadarActive(FalconWingmanMsg* msg)
        {
            int flightIdx;
            short[] edata = new short[10];
            RadarClass* theRadar = (RadarClass*)FindSensor(self, SensorClass.Radar);

            // Make sure the radar is on
            if (theRadar)
            {
                theRadar.SetEmitting(TRUE);
            }

            // set a radar flag here
            flightIdx = self.GetCampaignObject().GetComponentIndex(self);

            if (AiIsFullResponse(flightIdx, msg.dataBlock.to))
            {
                edata[0] = flightIdx;
                edata[1] = 1;
                AiMakeRadioResponse(self, rcROGER, edata);
            }
            else
            {
                AiRespondShortCallSign(self);
            }
        }

        private void AiSetRadarStby(FalconWingmanMsg* msg)
        {
            int flightIdx;
            short[] edata = new short[10];
            RadarClass* theRadar = (RadarClass*)FindSensor(self, SensorClass.Radar);

            // Make sure the radar is off
            if (theRadar)
            {
                theRadar.SetEmitting(FALSE);
            }

            // clear a radar flag here
            flightIdx = self.GetCampaignObject().GetComponentIndex(self);

            if (AiIsFullResponse(flightIdx, msg.dataBlock.to))
            {
                edata[0] = flightIdx;
                edata[1] = 1;
                AiMakeRadioResponse(self, rcROGER, edata);
            }
            else
            {
                AiRespondShortCallSign(self);
            }
        }

        private void AiBuddySpikeReact(FalconWingmanMsg* msg)
        {
            int flightIdx;
            short[] edata = new short[10];

            // set a radar flag here
            flightIdx = self.GetCampaignObject().GetComponentIndex(self);

            if (AiIsFullResponse(flightIdx, msg.dataBlock.to))
            {
                edata[0] = flightIdx;
                edata[1] = 1;
                AiMakeRadioResponse(self, rcROGER, edata);
            }
            else
            {
                AiRespondShortCallSign(self);
            }
        }

        private void AiRaygun(FalconWingmanMsg* msg)
        {
            int flightIdx;
            short[] edata = new short[10];

            // set a radar flag here
            flightIdx = self.GetCampaignObject().GetComponentIndex(self);

            if (msg.dataBlock.newTarget == self.Id())
            {
                edata[0] = -1;
                edata[1] = flightIdx;
                AiMakeRadioResponse(self, rcBUDDYSPIKE, edata);
            }
            else
            {
                edata[0] = -1;
                edata[1] = CALLSIGN_NUM_OFFSET + flightIdx + 1;
                edata[2] = -1;
                edata[3] = -1;
                edata[4] = 1;
                AiMakeRadioResponse(self, rcUNABLE, edata);
            }
        }

        private void AiRTB(FalconWingmanMsg* msg)
        {
            int flightIdx;
            short[] edata = new short[10];
            WayPointClass* pWaypoint = self.waypoint;
            BOOL done = FALSE;

            mpActionFlags[AI_ENGAGE_TARGET] = AI_NONE; // 2002-03-04 MODIFIED BY S.G. Use new enum type
            mpActionFlags[AI_EXECUTE_MANEUVER] = FALSE;
            mpActionFlags[AI_FOLLOW_FORMATION] = FALSE;
            mpActionFlags[AI_RTB] = TRUE;

            while (!done)
            {
                if (pWaypoint)
                {
                    if (pWaypoint.GetWPAction() == WP_LAND)
                    {
                        self.curWaypoint = pWaypoint;
                        done = TRUE;
                    }
                    else
                    {
                        pWaypoint = pWaypoint.GetNextWP();
                    }
                }
                else
                {
                    // unable
                    done = TRUE;
                }
            }

            flightIdx = self.GetCampaignObject().GetComponentIndex(self);
            AiSplitFlight(msg.dataBlock.to, msg.dataBlock.from, flightIdx);

            if (AiIsFullResponse(flightIdx, msg.dataBlock.to))
            {
                edata[0] = flightIdx;
                if (!IsSetATC(SaidRTB))
                {
                    SetATCFlag(SaidRTB);
                    AiMakeRadioResponse(self, rcIMADOT, edata);
                }
                AiCheckFormStrip();
            }
            else
            {
                AiCheckFormStrip();
                AiRespondShortCallSign(self);
            }
        }


        // Commands that modify the state basis
        private void AiDesignateTarget(FalconWingmanMsg* msg)
        {
            int flightIdx;
            short[] edata = new short[10];
            FalconEntity* newTarg = (FalconEntity*)vuDatabase.Find(msg.dataBlock.newTarget);

            flightIdx = self.GetCampaignObject().GetComponentIndex(self);

            if (newTarg)
            {
                // 2001-07-17 ADDED BY S.G. WHEN TOLD TO DESIGNATE AND IT'S A PLAYER'S WING, LOOSE YOUR HISTORY BECAUSE YOU MIGHT HAVE CHOSEN SOMETHING TO HIT BY YOURSELF ALREADY
                //            THE TARGET HERE WILL BE A CAMPAIGN OBJECT BUT ONCE THE HISTORY IS REMOVED, THE SAME SIM OBJECT CAN BE CHOSEN.
                if (flightLead && flightLead.IsSetFlag(MOTION_OWNSHIP))
                    gndTargetHistory[0] = null;
                // END OF ADDED SECTION

                // Try not to attack friendlies
                if (newTarg.GetTeam() != self.GetTeam() || (SkillLevel() < 2 && rand() % 10 > SkillLevel() + 8))
                {
                    mWeaponsAction = AI_WEAPONS_FREE;
                    // 2000-09-26 ADDED BY S.G. SO ASSIGN GROUP WORKS BY ASSIGNING TARGETS ACCORDING TO THEIR POSITION IN FLIGHT (LIKE FOR THE AI)
                    if ((FalconWingmanMsg.WingManCmd)msg.dataBlock.command == FalconWingmanMsg.WMAssignGroup)
                    {
                        // If it's a sim object, get the corresponding campaign object and assign it
                        if (((FalconEntity*)newTarg).IsSim())
                            AiSearchTargetList(((SimBaseClass*)newTarg).GetCampaignObject());
                        else
                            AiSearchTargetList(newTarg);

                        if (targetPtr)
                            mDesignatedObject = targetPtr.BaseData().Id();
                    }
                    else
                        // END OF ADDED SECTION (EXCEPT FOR INDENT OF THE NEXT LINE)
                        mDesignatedObject = msg.dataBlock.newTarget;

                    //			mpActionFlags[AI_ENGAGE_TARGET]		= TRUE; // 2002-03-04 REMOVED BY S.G. Done within the "if (newTarg.OnGround())" test below now

                    mpActionFlags[AI_RTB] = FALSE;
                    mCurrentManeuver = FalconWingmanMsg.WMTotalMsg;


                    if (newTarg.OnGround())
                    {
                        mpActionFlags[AI_ENGAGE_TARGET] = AI_GROUND_TARGET; // 2002-03-04 ADDED BY S.G. It's a ground target, say that's what we're engaging
                        // 2001-06-20 ADDED BY S.G. NEED TO TELL AI THERE AG MISSION IS NOT COMPLETE ANYMORE
                        missionComplete = FALSE;
                        // END OF ADDED SECTION

                        SetGroundTarget(newTarg);
                        if (self.AutopilotType() == AircraftClass.CombatAP)
                        {
                            SetupAGMode(null, null);

                            if (groundTargetPtr == null)
                            {
                                mDesignatedObject = FalconNullId;
                                mpActionFlags[AI_ENGAGE_TARGET] = AI_NONE; // 2002-03-04 MODIFIED BY S.G. Use new enum type

                                edata[0] = ((FlightClass*)self.GetCampaignObject()).callsign_id;
                                edata[1] = (((FlightClass*)self.GetCampaignObject()).callsign_num - 1) * 4 + self.GetCampaignObject().GetComponentIndex(self) + 1;
                                edata[2] = -1;
                                edata[3] = -1;
                                edata[4] = 0;
                                AiMakeRadioResponse(self, rcUNABLE, edata);
                            }
                            else
                            {
                                edata[0] = flightIdx;
                                edata[1] = 2;
                                AiMakeRadioResponse(self, rcROGER, edata);
                                AiSplitFlight(msg.dataBlock.to, msg.dataBlock.from, flightIdx);
                            }
                        }
                        else
                        {
                            agDoctrine = AGD_NEED_SETUP;
                        }
                    }
                    else
                    {
                        // 2000-10-11 ADDED BY S.G. NEED TO SET agDoctrine TO AGD_NONE AND CLEAR OUT THE GROUND TARGET SO IT WON'T ATTACK GROUND TARGET INSTEAD OF THE DESIGNATED AIR ONE
                        // 2002-03-04 ADDED BY S.G. Only reset the agDoctrine and groundTargetPtr if it's comming from the player. That way, AI will not react too quickly to target change
                        // 2002-03-07 MODIFIED BY S.G. Make sure flightlead and myLead are non null before doing a .IsPlayer on them...
                        int leadIsPlayer = FALSE;

                        if (flightLead && flightLead.IsPlayer())
                            leadIsPlayer = TRUE;
                        else
                        {
                            if (isWing == AiSecondWing)
                            {
                                AircraftClass* myLead;
                                myLead = (AircraftClass*)self.GetCampaignObject().GetComponentNumber(2);
                                if (myLead && myLead.IsPlayer())
                                    leadIsPlayer = TRUE;
                            }
                        }
                        if (leadIsPlayer)
                        // END OF ADDED SECTION 2002-03-04
                        {
                            agDoctrine = AGD_NONE;
                            SetGroundTarget(null);
                        }
                        // END OF ADDED SECTION 2000-10-11
                        // 2002-03-04 ADDED BY S.G. Prioritize ground target over air target but if needed be, it will attack anyway
                        if (mpActionFlags[AI_ENGAGE_TARGET] == AI_NONE)
                            mpActionFlags[AI_ENGAGE_TARGET] = AI_AIR_TARGET;
                        // END OF ADDED SECTION 2002-03-04
                        edata[0] = flightIdx;
                        edata[1] = 2;
                        AiMakeRadioResponse(self, rcROGER, edata);

                        AiSplitFlight(msg.dataBlock.to, msg.dataBlock.from, flightIdx);
                    }
                }
                else
                {
                    edata[0] = ((FlightClass*)self.GetCampaignObject()).callsign_id;
                    edata[1] = (((FlightClass*)self.GetCampaignObject()).callsign_num - 1) * 4 +
                    self.GetCampaignObject().GetComponentIndex(self) + 1;
                    edata[2] = -1;
                    edata[3] = -1;
                    edata[4] = 1;
                    AiMakeRadioResponse(self, rcUNABLE, edata);
                    return;
                }
            }
            else
            {
                mDesignatedObject = FalconNullId;
                mpActionFlags[AI_ENGAGE_TARGET] = AI_NONE; // 2002-03-04 MODIFIED BY S.G. Use new enum type
            }
        }

        private void AiDesignateGroup(FalconWingmanMsg* msg)
        {
#if NOTHING
	SimBaseClass*	psimBase;
	SimBaseClass*	ptarget;

	mDesignatedObject	= FalconNullId;


	psimBase			= (SimBaseClass*) vuDatabase.Find(msg.dataBlock.newTarget);

// VWF caution	what about things that are not vehicles?

// if it is a vehicle
	if(psimBase && psimBase.campaignObject.components) {		
		
		VuListIterator		elementWalker(psimBase.campaignObject.components);

		// pick the closest to my side of formation
		ptarget				= (SimBaseClass*)elementWalker.GetFirst();					
//		ptarget				= (SimBaseClass*)elementWalker.GetNext();
		
		SetTarget(ptarget);
	}
	else {
		SetTarget(null);
	}
#endif
        }

        private void AiSetWeaponsAction(FalconWingmanMsg* msg, DigitalBrain.AiWeaponsAction action)
        {
            WayPointClass* tmpWaypoint = self.waypoint;
            int flightIdx;
            short[] edata = new short[10];

            mWeaponsAction = action;

            flightIdx = self.GetCampaignObject().GetComponentIndex(self);

            // 2000-09-28 ADDED BY S.G. IF ASKED TO GO WEAPON HOLD, SET 'missileShotTimer' AT 2 HOURS FROM NOW
            if (action == AI_WEAPONS_HOLD)
            {
                missileShotTimer = SimLibElapsedTime + 2 * 60 * 60 * SEC_TO_MSEC;
                // END OF ADDED SECTION
                // 2001-06-04 ADDED BY S.G. FORE A REJOIN IF IN WEAPONS HOLD
                AiRejoin(null);
                // END OF ADDED SECTION
            }

            if (action == AI_WEAPONS_FREE && missionClass == AGMission && IsSetATC(WaitingPermission))
            {
                // 2001-06-04 ADDED BY S.G. IF ASKED TO GO WEAPON FREE, SET 'missileShotTimer' 0
                missileShotTimer = 0;
                // END OF ADDED SECTION

                // Find the IP waypoint
                while (tmpWaypoint)
                {
                    // 2000-09-28 MODIFIED BY S.G. WHAT IF WE DON'T HAVE AN IP? IN THAT CASE, CHECK IF TARGET, IF SUCH, GO BACK ONE WAYPOINT AND BREAK
                    if (tmpWaypoint.GetWPFlags() & WPF_TARGET)
                    {
                        tmpWaypoint = tmpWaypoint.GetPrevWP();
                        break;
                    }
                    // END OF ADDED SECTION
                    if (tmpWaypoint.GetWPFlags() & WPF_IP)
                        break;
                    tmpWaypoint = tmpWaypoint.GetNextWP();
                }

                // Have an IP
                if (tmpWaypoint)
                    self.curWaypoint = tmpWaypoint.GetNextWP();

                // 2000-09-28 MODIFIED BY S.G. WE SAVE THE CURRENT GROUND TARGET, CLEAR IT, CALL THE ROUTINE AND IF IT CAN'T FIND ONE, RESTORE IT
                //		SelectGroundTarget (TARGET_ANYTHING);
                SimObjectType* tmpGroundTargetPtr = groundTargetPtr;
                groundTargetPtr = 0;
                SelectGroundTarget(TARGET_ANYTHING);
                if (groundTargetPtr == null)
                    groundTargetPtr = tmpGroundTargetPtr;
                // END OF ADDED SECTION

                SetupAGMode(null, null);
                // 2000-09-27 REMOVED BY S.G. WE'LL CLEAR THE FLAG *ONLY* IF WE HAVE A TARGET
                //		ClearATCFlag(WaitingPermission);

                if (groundTargetPtr == null)
                {
                    mDesignatedObject = FalconNullId;
                    mpActionFlags[AI_ENGAGE_TARGET] = AI_NONE; // 2002-03-04 MODIFIED BY S.G. Use new enum type

                    edata[0] = ((FlightClass*)self.GetCampaignObject()).callsign_id;
                    edata[1] = (((FlightClass*)self.GetCampaignObject()).callsign_num - 1) * 4 +
                    self.GetCampaignObject().GetComponentIndex(self) + 1;
                    edata[2] = -1;
                    edata[3] = -1;
                    edata[4] = 0;
                    AiMakeRadioResponse(self, rcUNABLE, edata);
                }
                else
                {
                    // 2000-09-27 ADDED BY S.G. WE'LL CLEAR THE FLAG *ONLY* IF WE HAVE A TARGET
                    ClearATCFlag(WaitingPermission);

                    mDesignatedObject = groundTargetPtr.BaseData().Id();
                    mpActionFlags[AI_ENGAGE_TARGET] = AI_GROUND_TARGET; // 2002-03-04 MODIFIED BY S.G. Use new enum type
                    mpActionFlags[AI_RTB] = FALSE;
                    edata[0] = flightIdx;
                    edata[1] = 2;
                    AiMakeRadioResponse(self, rcROGER, edata);
                }
            }
            else
            {
                mpActionFlags[AI_RTB] = FALSE;
                if (AiIsFullResponse(flightIdx, msg.dataBlock.to))
                {
                    edata[0] = flightIdx;
                    edata[1] = 2;
                    AiMakeRadioResponse(self, rcROGER, edata);
                }
                else
                {
                    AiRespondShortCallSign(self);
                }
            }
        }

        // Commands that modify formation
        private void AiSetFormation(FalconWingmanMsg* msg)
        {

            short[] edata = new short[10];
            int flightIdx;

            //we can't fly in formation if we're on the ground still!
            if (self.OnGround() || atcstatus >= lOnFinal)
                return;

            //	mInPositionFlag = FALSE;
            AiCheckPosition();

            //	mFormLateralSpaceFactor	= 1.0F;

            flightIdx = self.GetCampaignObject().GetComponentIndex(self);
            mFormation = acFormationData.FindFormation(msg.dataBlock.command);
            mpActionFlags[AI_FOLLOW_FORMATION] = TRUE;
            SendATCMsg(noATC);
            atcstatus = noATC;
            // cancel atc here

            AiGlueFlight(msg.dataBlock.to, msg.dataBlock.from, flightIdx);

            if (AiIsFullResponse(flightIdx, msg.dataBlock.to))
            {

                edata[1] = -1;
                edata[2] = -1;
                edata[3] = -1;
                edata[4] = -1;
                edata[5] = -1;
                edata[6] = -1;
                edata[7] = -1;
                edata[8] = -1;
                edata[9] = -1;

                edata[0] = flightIdx;


                int radioform = mFormation;

                // M.N. hack the next formation message indexes (currently 59,60,61)
                if (radioform > 8)
                    radioform = mFormation + 50;

                switch (radioform)
                {
                    case FalconWingmanMsg.WMSpread:
                        edata[1] = 1;
                        break;
                    case FalconWingmanMsg.WMWedge:
                        edata[1] = 2;
                        break;
                    case FalconWingmanMsg.WMTrail:
                        edata[1] = 3;
                        break;
                    case FalconWingmanMsg.WMLadder:
                        edata[1] = 4;
                        break;
                    case FalconWingmanMsg.WMStack:
                        edata[1] = 5;
                        break;
                    case FalconWingmanMsg.WMResCell:
                        edata[1] = 6;
                        break;
                    case FalconWingmanMsg.WMBox:
                        edata[1] = 7;
                        break;
                    case FalconWingmanMsg.WMArrowHead:
                        edata[1] = 8;
                        break;
                    case FalconWingmanMsg.WMFluidFour:
                        edata[1] = 14;
                        break;
                    case FalconWingmanMsg.WMVic:
                        edata[1] = 10;
                        break;
                    case FalconWingmanMsg.WMEchelon:
                        edata[1] = 11;
                        break;
                    case FalconWingmanMsg.WMFinger4:
                        edata[1] = 13;
                        break;
                    case FalconWingmanMsg.WMForm1:
                        edata[1] = 15;
                        break;
                    case FalconWingmanMsg.WMForm2:
                        edata[1] = 16;
                        break;
                    case FalconWingmanMsg.WMForm3:
                        edata[1] = 17;
                        break;
                    case FalconWingmanMsg.WMForm4:
                        edata[1] = 18;
                        break;
                }

                AiMakeRadioResponse(self, rcFORMRESPONSEB, edata);
            }
            else
            {
                AiRespondShortCallSign(self);
            }
        }

        private void AiKickout(FalconWingmanMsg* msg)
        {

            short[] edata = new short[10];
            int flightIdx;

            //	mInPositionFlag = FALSE;
            AiCheckPosition();

            mFormLateralSpaceFactor *= 2.0F;
            flightIdx = self.GetCampaignObject().GetComponentIndex(self);

            if (AiIsFullResponse(flightIdx, msg.dataBlock.to))
            {

                edata[1] = -1;
                edata[2] = -1;
                edata[3] = -1;
                edata[4] = -1;
                edata[5] = -1;
                edata[6] = -1;
                edata[7] = -1;
                edata[8] = -1;
                edata[9] = -1;

                edata[0] = flightIdx;
                edata[1] = 2;

                AiMakeRadioResponse(self, rcFORMRESPONSEA, edata);
            }
            else
            {
                AiRespondShortCallSign(self);
            }
        }

        private void AiCloseup(FalconWingmanMsg* msg)
        {

            short[] edata = new short[10];
            int flightIdx;

            //	mInPositionFlag = FALSE;
            AiCheckPosition();

            mFormLateralSpaceFactor *= 0.5F;
            flightIdx = self.GetCampaignObject().GetComponentIndex(self);

            //MI give us a negative if we can't get closer
            if (mFormLateralSpaceFactor < 0.0625F)
            {
                edata[2] = -1;
                edata[3] = -1;
                edata[4] = -1;
                edata[5] = -1;
                edata[6] = -1;
                edata[7] = -1;
                edata[8] = -1;
                edata[9] = -1;

                edata[0] = flightIdx;
                edata[1] = 5;

                AiMakeRadioResponse(self, rcFORMRESPONSEA, edata);
            }
            else
            {
                if (AiIsFullResponse(flightIdx, msg.dataBlock.to))
                {

                    edata[1] = -1;
                    edata[2] = -1;
                    edata[3] = -1;
                    edata[4] = -1;
                    edata[5] = -1;
                    edata[6] = -1;
                    edata[7] = -1;
                    edata[8] = -1;
                    edata[9] = -1;

                    edata[0] = flightIdx;
                    edata[1] = 3;

                    AiMakeRadioResponse(self, rcFORMRESPONSEA, edata);
                }
                else
                {
                    AiRespondShortCallSign(self);
                }
            }
        }

        private void AiToggleSide()
        {
            mFormSide *= -1;	// +1 normal formation, -1 is mirrored formation
            AiRespondShortCallSign(self);
        }

        private void AiIncreaseRelativeAltitude()
        {
            mFormRelativeAltitude -= 1000.0F;
            // do radio response here
            //MI making reply
            short[] edata = new short[10];
            int flightIdx = self.GetCampaignObject().GetComponentIndex(self);
            edata[1] = -1;
            edata[2] = -1;
            edata[3] = -1;
            edata[4] = -1;
            edata[5] = -1;
            edata[6] = -1;
            edata[7] = -1;
            edata[8] = -1;
            edata[9] = -1;

            edata[0] = flightIdx;
            edata[1] = 0;

            AiMakeRadioResponse(self, rcFORMRESPONSEA, edata);

        }

        private void AiDecreaseRelativeAltitude()
        {
            mFormRelativeAltitude += 1000.0F;
            // do radio response here
            //MI making reply
            short[] edata = new short[10];
            int flightIdx = self.GetCampaignObject().GetComponentIndex(self);
            edata[1] = -1;
            edata[2] = -1;
            edata[3] = -1;
            edata[4] = -1;
            edata[5] = -1;
            edata[6] = -1;
            edata[7] = -1;
            edata[8] = -1;
            edata[9] = -1;

            edata[0] = flightIdx;
            edata[1] = 1;

            AiMakeRadioResponse(self, rcFORMRESPONSEA, edata);
        }

        // Transient Commands
        private void AiGiveBra(FalconWingmanMsg* msg)
        {
            short[] edata = new short[10];
            float rx, ry, rz;
            float dsq;
            int response;
            float xdiff, ydiff;
            float angle;
            int navangle;

            AircraftClass* psender;

            psender = (AircraftClass*)vuDatabase.Find(msg.dataBlock.from);


            rx = self.XPos() - psender.XPos();
            ry = self.YPos() - psender.YPos();
            rz = self.ZPos() - psender.ZPos();

            navangle = FloatToInt32(ConvertRadtoNav((float)atan2(ry, rx)));		// convert to compass angle

            dsq = rx * rx + ry * ry;

            if (dsq < Phyconst.NM_TO_FT * Phyconst.NM_TO_FT)
            {

                edata[0] = self.GetCampaignObject().GetComponentIndex(self);

                xdiff = self.XPos() - psender.XPos();
                ydiff = self.YPos() - psender.YPos();

                angle = (float)atan2(ydiff, xdiff);
                angle = angle - psender.Yaw();
                navangle = FloatToInt32(RTD * angle);
                if (navangle < 0)
                {
                    navangle = 360 + navangle;
                }

                edata[1] = navangle / 30;									// scale compass angle for radio eData
                if (edata[1] >= 12)
                {
                    edata[1] = 0;
                }

                /*
                        edata[1] = navangle / 30;									// scale compass angle for radio eData
                        if(edata[1] >= 12) {
                            edata[1] = 0;
                        }
                */
                if (rz < 300.0F && rz > -300.0F)
                {							// check relative alt and select correct frag
                    edata[2] = 1;
                }
                else if (rz < -300.0F && rz > -1000.0F)
                {
                    edata[2] = 2;
                }
                else if (rz < -1000.0F)
                {
                    edata[2] = 3;
                }
                else
                {
                    edata[2] = 0;
                }

                response = rcPOSITIONRESPONSEB;
            }
            else
            {
                edata[0] = ((FlightClass*)psender.GetCampaignObject()).callsign_id;
                edata[1] = (((FlightClass*)psender.GetCampaignObject()).callsign_num - 1) * 4 + psender.GetCampaignObject().GetComponentIndex(psender) + 1;
                edata[2] = ((FlightClass*)self.GetCampaignObject()).callsign_id;
                edata[3] = (((FlightClass*)self.GetCampaignObject()).callsign_num - 1) * 4 + self.GetCampaignObject().GetComponentIndex(self) + 1;
                edata[4] = (short)SimToGrid(self.YPos());
                edata[5] = (short)SimToGrid(self.XPos());
                edata[6] = (short)-self.ZPos();

                response = rcPOSITIONRESPONSEA;

            }

            AiMakeRadioResponse(self, response, edata);
        }

        private void AiGiveStatus(FalconWingmanMsg* msg)
        {
            short[] edata = new short[10];
            int response;
            int random;
            float xdiff;
            float ydiff;
            float rz;
            int flightIdx;
            FalconEntity* pmytarget = null;
            AircraftClass* pfrom;
            float angle;
            int navangle;

            if (targetPtr)
            {
                pmytarget = targetPtr.BaseData();
            }
            flightIdx = self.GetCampaignObject().GetComponentIndex(self);

            if ((curMode == GunsJinkMode || curMode == MissileDefeatMode) && pmytarget && (pmytarget.IsAirplane() || pmytarget.IsHelicopter()))
            {

                xdiff = self.XPos() - pmytarget.XPos();
                ydiff = self.YPos() - pmytarget.YPos();

                if (xdiff * xdiff + ydiff * ydiff > Phyconst.NM_TO_FT * Phyconst.NM_TO_FT)
                {
                    if (PlayerOptions.BullseyeOn())
                    {
                        response = rcENGDEFENSIVEA;
                    }
                    else
                    {
                        response = rcENGDEFENSIVEB;
                    }
                    edata[0] = ((FlightClass*)self.GetCampaignObject()).callsign_id;
                    edata[1] = (((FlightClass*)self.GetCampaignObject()).callsign_num - 1) * 4 + flightIdx + 1;
                    edata[2] = (short)SimToGrid(pmytarget.YPos());
                    edata[3] = (short)SimToGrid(pmytarget.XPos());
                    edata[4] = (short)pmytarget.ZPos();
                }
                else
                {
                    edata[0] = flightIdx;
                    response = rcENGDEFENSIVEC;
                }
            }
            else if (pmytarget && (pmytarget.IsAirplane() || pmytarget.IsHelicopter()))
            {

                edata[0] = -1;
                edata[1] = -1;
                edata[2] = ((FlightClass*)self.GetCampaignObject()).callsign_id;
                edata[3] = (((FlightClass*)self.GetCampaignObject()).callsign_num - 1) * 4 + flightIdx + 1;
                edata[4] = (short)SimToGrid(pmytarget.YPos());
                edata[5] = (short)SimToGrid(pmytarget.XPos());
                edata[6] = (short)pmytarget.ZPos();

                response = rcAIRTARGETBRA;

                // rcBANDIT
            }
            else if (mpActionFlags[AI_EXECUTE_MANEUVER]/* == TRUE *//* 2002-03-15 REMOVED BY S.G. Can be TRUE or TRUE+1 now */)
            {

                edata[0] = self.GetCampaignObject().GetComponentIndex(self);

                switch (mCurrentManeuver)
                {

                    case FalconWingmanMsg.WMChainsaw:
                        edata[1] = 6;
                        break;

                    case FalconWingmanMsg.WMPince:
                        edata[1] = 4;
                        break;

                    case FalconWingmanMsg.WMPosthole:
                        edata[1] = 5;
                        break;

                    case FalconWingmanMsg.WMFlex:
                    case FalconWingmanMsg.WMSkate:
                    case FalconWingmanMsg.WMSSOffset:
                        edata[1] = 8;
                        break;
                }
                // status = performing maneuver
                response = rcEXECUTERESPONSE;
            }
            else if ((curMode == GunsEngageMode ||
                   curMode == MissileEngageMode ||
                   curMode == WVREngageMode ||
                   curMode == BVREngageMode) && pmytarget && (pmytarget.IsAirplane() || pmytarget.IsHelicopter()))
            {
                xdiff = self.XPos() - pmytarget.XPos();
                ydiff = self.YPos() - pmytarget.YPos();

                if (xdiff * xdiff + ydiff * ydiff > Phyconst.NM_TO_FT * Phyconst.NM_TO_FT)
                {
                    if (PlayerOptions.BullseyeOn())
                    {
                        response = rcENGAGINGA;
                    }
                    else
                    {
                        response = rcENGAGINGB;
                    }
                    edata[0] = ((FlightClass*)self.GetCampaignObject()).callsign_id;
                    edata[1] = (((FlightClass*)self.GetCampaignObject()).callsign_num - 1) * 4 + flightIdx + 1;
                    edata[2] = 2 * (pmytarget.Type() - VU_LAST_ENTITY_TYPE);
                    edata[3] = (short)SimToGrid(pmytarget.YPos());
                    edata[4] = (short)SimToGrid(pmytarget.XPos());
                    edata[5] = (short)pmytarget.ZPos();
                }
                else
                {
                    pfrom = (AircraftClass*)vuDatabase.Find(msg.dataBlock.from);
                    edata[0] = 2 * (pmytarget.Type() - VU_LAST_ENTITY_TYPE);

                    xdiff = pmytarget.XPos() - pfrom.XPos();
                    ydiff = pmytarget.YPos() - pfrom.YPos();

                    angle = (float)atan2(ydiff, xdiff);
                    angle = angle - pfrom.Yaw();
                    navangle = FloatToInt32(RTD * angle);
                    if (navangle < 0)
                    {
                        navangle = 360 + navangle;
                    }

                    edata[1] = navangle / 30;									// scale compass angle for radio eData
                    if (edata[1] >= 12)
                    {
                        edata[1] = 0;
                    }

                    rz = pmytarget.ZPos() - pfrom.ZPos();

                    if (rz < 300.0F && rz > -300.0F)
                    {							// check relative alt and select correct frag
                        edata[2] = 1;
                    }
                    else if (rz < -300.0F && rz > -1000.0F)
                    {
                        edata[2] = 2;
                    }
                    else if (rz < -1000.0F)
                    {
                        edata[2] = 3;
                    }
                    else
                    {
                        edata[2] = 0;
                    }
                    response = rcENGAGINGC;
                }
            }
            else if (pmytarget && (pmytarget.IsAirplane() || pmytarget.IsHelicopter()))
            {
                // and i am spiked
                response = rcSPIKE;
            }
            //	else if(have stuff on radar) {
            //		response = rcPICTUREBRA;
            //	}
            else
            {
                // status = clean, clear & naked
                random = 4 * (FloatToInt32((float)rand() / (float)RAND_MAX));

                edata[0] = flightIdx;
                edata[1] = random;

                response = rcGENERALRESPONSEC;
            }

            AiMakeRadioResponse(self, response, edata);
        }

        struct DamageEntryStruct
        {
            FaultClass.type_FSubSystem subSystem;
            int status;
        }

        private void AiGiveDamageReport(FalconWingmanMsg* msg)
        {
            short[] edata = new short[10];
            int lastFault = 0;
            int count = 0;
            int i;
            DamageEntryStruct DamageEntry;

            DamageEntry[] pFaultList = new DamageEntry[DAMAGELIST] {{FaultClass.eng_fault, FALSE},
									{FaultClass.fcr_fault, FALSE},
									{FaultClass.flcs_fault, FALSE},
									{FaultClass.sms_fault, FALSE},
									{FaultClass.ins_fault, FALSE},
									{FaultClass.rwr_fault, FALSE},
									{FaultClass.tcn_fault, FALSE},
									{FaultClass.ufc_fault, FALSE},
									{FaultClass.amux_fault, FALSE}};

            edata[0] = self.GetCampaignObject().GetComponentIndex(self);			// Get my slot in the flight
            count = ((AircraftClass*)self).mFaults.GetFFaultCount();			// Check how many faults are set

            if (count == 0)
            {																			// If is no damage, say a-okay
                edata[1] = 3;
                AiMakeRadioResponse(self, rcGENERALRESPONSEC, edata);
                return;
            }

            i = count;

            while (lastFault < DAMAGELIST)
            {

                if (((AircraftClass*)self).mFaults.GetFault(pFaultList[lastFault].subSystem) == TRUE)
                {

                    pFaultList[lastFault].status = TRUE;

                    if (pFaultList[lastFault].subSystem == FaultClass.eng_fault)
                    {	// Evaluate each system
                        edata[1] = 0;
                        edata[2] = 4;
                    }
                    else if (pFaultList[lastFault].subSystem == FaultClass.fcr_fault)
                    {
                        edata[1] = 1;
                        edata[2] = 1;
                    }
                    else if (pFaultList[lastFault].subSystem == FaultClass.flcs_fault)
                    {
                        edata[1] = 2;
                        edata[2] = 2;
                    }
                    else if (pFaultList[lastFault].subSystem == FaultClass.sms_fault)
                    {
                        edata[1] = 3;
                        edata[2] = 2;
                    }
                    else if (pFaultList[lastFault].subSystem == FaultClass.ins_fault)
                    {
                        edata[1] = 4;
                        edata[2] = 1;
                    }
                    else if (pFaultList[lastFault].subSystem == FaultClass.rwr_fault)
                    {
                        edata[1] = 5;
                        edata[2] = 2;
                    }
                    else if (pFaultList[lastFault].subSystem == FaultClass.tcn_fault)
                    {
                        edata[1] = 6;
                        edata[2] = 1;
                    }
                    else if (pFaultList[lastFault].subSystem == FaultClass.ufc_fault)
                    {
                        edata[1] = 7;
                        edata[2] = 2;
                    }
                    else if (pFaultList[lastFault].subSystem == FaultClass.amux_fault)
                    {
                        edata[1] = 8;
                        edata[2] = 1;
                    }

                    if (i != count)
                    {
                        edata[0] = -1;
                    }

                    AiMakeRadioResponse(self, rcDAMREPORT, edata);
                    i--;
                }
                lastFault++;
            }

            edata[0] = -1;

            if (i > 0)
            {
                for (lastFault = 0; lastFault < DAMAGELIST; lastFault++, i--)
                {

                    if (pFaultList[lastFault].status == FALSE)
                    {

                        if (pFaultList[lastFault].subSystem == FaultClass.eng_fault)
                        {	// Evaluate each system
                            edata[1] = 0;
                            edata[2] = 4;
                        }
                        else if (pFaultList[lastFault].subSystem == FaultClass.fcr_fault)
                        {
                            edata[1] = 1;
                            edata[2] = 1;
                        }
                        else if (pFaultList[lastFault].subSystem == FaultClass.flcs_fault)
                        {
                            edata[1] = 2;
                            edata[2] = 2;
                        }
                        else if (pFaultList[lastFault].subSystem == FaultClass.sms_fault)
                        {
                            edata[1] = 3;
                            edata[2] = 2;
                        }
                        else if (pFaultList[lastFault].subSystem == FaultClass.ins_fault)
                        {
                            edata[1] = 4;
                            edata[2] = 1;
                        }
                        else if (pFaultList[lastFault].subSystem == FaultClass.rwr_fault)
                        {
                            edata[1] = 5;
                            edata[2] = 2;
                        }
                        else if (pFaultList[lastFault].subSystem == FaultClass.tcn_fault)
                        {
                            edata[1] = 6;
                            edata[2] = 1;
                        }
                        else if (pFaultList[lastFault].subSystem == FaultClass.ufc_fault)
                        {
                            edata[1] = 7;
                            edata[2] = 2;
                        }
                        else if (pFaultList[lastFault].subSystem == FaultClass.amux_fault)
                        {
                            edata[1] = 8;
                            edata[2] = 1;
                        }
                    }

                    if (i > 0)
                    {
                        AiMakeRadioResponse(self, rcDAMREPORT, edata);
                    }
                }
            }
        }

        private void AiGiveFuelStatus(FalconWingmanMsg* msg)
        {
            short[] edata = new short[10];
            int response;

            edata[0] = self.GetCampaignObject().GetComponentIndex(self);

            if (self.af.Fuel() <= self.bingoFuel)
            {
                response = rcGENERALRESPONSEC;
                edata[1] = 4;
            }
            else
            {
                response = rcFUELCHECKRSP;
                edata[1] = (FloatToInt32(self.af.Fuel() + self.af.ExternalFuel()));// / 1000;
            }

            AiMakeRadioResponse(self, response, edata);
        }

        private void AiGiveWeaponsStatus()
        {
            // what happens if i have multiple weapon types?
            short[] edata = new short[10];
            int hp;
            int flightIdx;

            int hasRadar = 0;
            int hasHeat = 0;
            int hasBomb = 0;
            int hasHARM = 0;
            int hasAGM = 0;
            int hasLGB = 0;
            int hasRockets = 0;
            int hasWeapons = 0;
            int response;

            flightIdx = self.GetCampaignObject().GetComponentIndex(self);

            short e0 = ((FlightClass*)self.GetCampaignObject()).callsign_id;
            short e1 = (((FlightClass*)self.GetCampaignObject()).callsign_num - 1) * 4 + flightIdx + 1;


            // 2001-10-16 M.N. only one time the full callsign
            edata[0] = e0;
            edata[1] = e1;
            edata[2] = -1;
            AiMakeRadioResponse(self, rcEXECUTE, edata);


            // Do a search for Heaters and Radars
            SMSClass* sms = (SMSClass*)self.GetSMS();

            if (sms)
            {
                for (hp = 1; hp < sms.NumHardpoints(); hp++)
                {
                    if (sms.hardPoint[hp] && sms.hardPoint[hp].weaponPointer && sms.hardPoint[hp].GetWeaponType() == wtAim120)
                        hasRadar += sms.hardPoint[hp].weaponCount;
                    if (sms.hardPoint[hp] && sms.hardPoint[hp].weaponPointer && sms.hardPoint[hp].GetWeaponType() == wtAim9)
                        hasHeat += sms.hardPoint[hp].weaponCount;
                    if (sms.hardPoint[hp] && sms.hardPoint[hp].weaponPointer && sms.hardPoint[hp].GetWeaponType() == wtAgm88)
                        hasHARM += sms.hardPoint[hp].weaponCount;
                    if (sms.hardPoint[hp] && sms.hardPoint[hp].weaponPointer && sms.hardPoint[hp].GetWeaponType() == wtAgm65)
                        hasAGM += sms.hardPoint[hp].weaponCount;
                    if (sms.hardPoint[hp] && sms.hardPoint[hp].weaponPointer && sms.hardPoint[hp].GetWeaponType() == wtMk82)
                        hasBomb += sms.hardPoint[hp].weaponCount;
                    if (sms.hardPoint[hp] && sms.hardPoint[hp].weaponPointer && sms.hardPoint[hp].GetWeaponType() == wtMk84)
                        hasBomb += sms.hardPoint[hp].weaponCount;
                    if (sms.hardPoint[hp] && sms.hardPoint[hp].weaponPointer && sms.hardPoint[hp].GetWeaponType() == wtGBU)
                        hasLGB += sms.hardPoint[hp].weaponCount;
                    if (sms.hardPoint[hp] && sms.hardPoint[hp].weaponPointer && sms.hardPoint[hp].GetWeaponType() == wtLAU && sms.hardPoint[hp].GetWeaponClass() == wcRocketWpn)
                        // hasRockets += sms.hardPoint[hp].weaponCount;
                        hasRockets++;
                    //if (sms.hardPoint[hp] && sms.hardPoint[hp].weaponPointer && sms.hardPoint[hp].GetWeaponType() == wtGuns)
                    //	hasGuns = 1;
                }
            }
            if (hasRadar > 24)
                hasRadar = 24;
            if (hasHARM > 24)
                hasHARM = 24;
            if (hasHeat > 24)
                hasHeat = 24;
            if (hasAGM > 24)
                hasAGM = 24;
            if (hasBomb > 24)
                hasBomb = 24;
            if (hasLGB > 24)
                hasLGB = 24;
            if (hasRockets > 24)
                hasRockets = 24;

            hasWeapons = hasRadar + hasHARM + hasHeat + hasAGM + hasBomb + hasLGB + hasRockets;

            if (hasAGM)
            {
                edata[0] = (short)hasAGM--;
                edata[1] = 242;
                AiMakeRadioResponse(self, rcWEAPONSCHECKRSP, edata);	// Commsequence changed 01-11-15
            }

            if (hasHARM)
            {
                edata[0] = (short)hasHARM--;
                edata[1] = 245;
                AiMakeRadioResponse(self, rcWEAPONSCHECKRSP, edata);
            }

            if (hasLGB)
            {
                edata[0] = (short)hasLGB--;
                edata[1] = 838;
                AiMakeRadioResponse(self, rcWEAPONSCHECKRSP, edata);
            }

            if (hasBomb)
            {
                edata[0] = (short)hasBomb--;
                edata[1] = 914;
                AiMakeRadioResponse(self, rcWEAPONSCHECKRSP, edata);
            }

            if (hasRockets)
            {
                edata[0] = (short)hasRockets--;
                edata[1] = 887;
                AiMakeRadioResponse(self, rcWEAPONSCHECKRSP, edata);
            }

            if (hasRadar)
            {
                edata[0] = (short)hasRadar--;
                edata[1] = 231;
                AiMakeRadioResponse(self, rcWEAPONSCHECKRSP, edata);
            }

            if (hasHeat)
            {
                edata[0] = (short)hasHeat--;
                edata[1] = 233;
                AiMakeRadioResponse(self, rcWEAPONSCHECKRSP, edata);
            }

            if (!hasWeapons)
            {
                edata[0] = -1;
                edata[1] = 0;	// Winchester
                AiMakeRadioResponse(self, rcWEAPONSLOW, edata);
            }

            // Finally say fuel

            edata[0] = -1;	// Turn off flight position

            if (self.af.Fuel() <= self.bingoFuel)
            {
                response = rcGENERALRESPONSEC;
                edata[1] = 4;
            }
            else
            {
                response = rcFUELCHECKRSP;
                edata[1] = (FloatToInt32(self.af.Fuel() + self.af.ExternalFuel()));// / 1000;
            }

            AiMakeRadioResponse(self, response, edata);

        }


        // Other Commands
        private void AiRefuel();
        private void AiPromote()
        {
            //MI for radio response
            short[] edata = new short[10];
            int response;
            int flightIdx;

            flightIdx = self.GetCampaignObject().GetComponentIndex(self);

            if (isWing > 0)
            {
                isWing--;

                if (!isWing)
                {
                    SetLead(TRUE);
                }
                else
                {
                    SetLead(FALSE);
                }
                //MI for radio response
                response = rcEXECUTERESPONSE;
                edata[0] = flightIdx;
                edata[1] = 11;
                AiMakeRadioResponse(self, response, edata);
            }
        }

        private void AiCheckInPositionCall(float trX, float trY, float trZ)
        {
            short[] edata = new short[10];
            float xdiff, ydiff, zdiff;
            int vehInFlight;
            int flightIdx;


            if (mInPositionFlag == FALSE)
            {
                // 2002-02-12 ADDED BY S.G. If the lead is climbing (or is the player since I can't tell what altitude he wants), be more relax about z
                float maxZDiff;

                if (g_bPitchLimiterForAI && flightLead && (flightLead.IsSetFlag(MOTION_OWNSHIP) || (((AircraftClass*)flightLead).DBrain() && fabs(flightLead.ZPos() - ((AircraftClass*)flightLead).DBrain().trackZ) > 2000.0f)))
                    maxZDiff = 2000.0f;
                else
                    maxZDiff = 250.0f;
                // END OF ADDED SECTION 2002-02-12

                xdiff = trX - self.XPos();
                ydiff = trY - self.YPos();
                zdiff = trZ - self.ZPos();

                if ((xdiff * xdiff + ydiff * ydiff < 250.0F * 250.0F) && fabs(zdiff) < maxZDiff)
                { // 2002-02-12 MODIFIED BY S.G. It's "ydiff * ydiff" not "ydiff + ydiff"! plus replaced 250.0f for maxZDiff
                    mInPositionFlag = TRUE;
                    edata[0] = self.GetCampaignObject().GetComponentIndex(self);
                    AiMakeRadioResponse(self, rcINPOSITION, edata);

                    vehInFlight = ((FlightClass*)self.GetCampaignObject()).GetTotalVehicles();
                    flightIdx = ((FlightClass*)self.GetCampaignObject()).GetComponentIndex(self);
                    if (flightIdx == AiElementLead && vehInFlight == 4)
                    {
                        AiMakeCommandMsg((SimBaseClass*)self, FalconWingmanMsg.WMGlue, AiWingman, FalconNullId);
                    }
                }
            }
        }

        private void AiCheckPosition()
        {
            float xdiff, ydiff, zdiff;
            float trX, trY, trZ, rangeFactor;
            ACFormationData.PositionData* curPosition;
            AircraftClass* paircraft;
            int vehInFlight, flightIdx;

            if (flightLead && flightLead != self)
            {
                // Get wingman slot position relative to the leader
                vehInFlight = ((FlightClass*)self.GetCampaignObject()).GetTotalVehicles();
                flightIdx = ((FlightClass*)self.GetCampaignObject()).GetComponentIndex(self);

                if (flightIdx == AiFirstWing && vehInFlight == 2)
                {
                    curPosition = &(acFormationData.twoposData[mFormation]);	// The four ship #2 slot position is copied in to the 2 ship formation array.
                    paircraft = (AircraftClass*)flightLead;
                }
                else if (flightIdx == AiSecondWing && mSplitFlight)
                {
                    curPosition = &(acFormationData.twoposData[mFormation]);
                    paircraft = (AircraftClass*)((FlightClass*)self.GetCampaignObject()).GetComponentEntity(AiElementLead);
                }
                else
                {
                    curPosition = &(acFormationData.positionData[mFormation][flightIdx - 1]);
                    paircraft = (AircraftClass*)flightLead;
                }

                rangeFactor = curPosition.range * (2.0F * mFormLateralSpaceFactor);

                // Get my leader's position
                Debug.Assert(paircraft);
                if (paircraft)
                {
                    trX = paircraft.XPos();
                    trY = paircraft.YPos();
                    trZ = paircraft.ZPos();

                    // Calculate position relative to the leader
                    trX += rangeFactor * (float)cos(curPosition.relAz * mFormSide + paircraft.af.sigma);
                    trY += rangeFactor * (float)sin(curPosition.relAz * mFormSide + paircraft.af.sigma);
                }

                if (curPosition.relEl)
                {
                    trZ += rangeFactor * (float)sin(-curPosition.relEl);
                }
                else
                {
                    trZ += (flightIdx * -100.0F);
                }

                xdiff = trX - self.XPos();
                ydiff = trY - self.YPos();
                zdiff = trZ - self.ZPos();

                if ((xdiff * xdiff + ydiff + ydiff > 250.0F * 250.0F) || fabs(zdiff) < 250.0F)
                {
                    mInPositionFlag = FALSE;
                }
                else
                {
                    mInPositionFlag = TRUE;
                }
            }
        }

        private void AiCheckFormStrip()
        {
            short[] edata = new short[10];

            if (mpActionFlags[AI_FOLLOW_FORMATION] == TRUE && mInPositionFlag)
            {
                mInPositionFlag = FALSE;
                edata[0] = self.GetCampaignObject().GetComponentIndex(self);
                AiMakeRadioResponse(self, rcSTRIPING, edata);
            }
        }


        private void AiSmokeOn(FalconWingmanMsg* msg)
        {
            short[] edata = new short[10];
            int flightIdx;

            if (SimDriver.playerEntity)
            {
                FalconTrackMessage* trackMsg = new FalconTrackMessage(1, SimDriver.playerEntity.Id(), FalconLocalGame);
                Debug.Assert(trackMsg);
                trackMsg.dataBlock.trackType = Track_SmokeOn;
                trackMsg.dataBlock.id = self.Id();

                FalconSendMessage(trackMsg, TRUE);
            }


            // set a radar flag here
            flightIdx = self.GetCampaignObject().GetComponentIndex(self);

            if (AiIsFullResponse(flightIdx, msg.dataBlock.to))
            {
                edata[0] = flightIdx;
                edata[1] = 1;
                AiMakeRadioResponse(self, rcROGER, edata);
            }
            else
            {
                AiRespondShortCallSign(self);
            }
        }
        private void AiSmokeOff(FalconWingmanMsg* msg)
        {
            short[] edata = new short[10];
            int flightIdx;

            if (SimDriver.playerEntity)
            {
                FalconTrackMessage* trackMsg = new FalconTrackMessage(1, SimDriver.playerEntity.Id(), FalconLocalGame);
                Debug.Assert(trackMsg);
                trackMsg.dataBlock.trackType = Track_SmokeOff;
                trackMsg.dataBlock.id = self.Id();

                FalconSendMessage(trackMsg, TRUE);
            }

            // set a radar flag here
            flightIdx = self.GetCampaignObject().GetComponentIndex(self);

            if (AiIsFullResponse(flightIdx, msg.dataBlock.to))
            {
                edata[0] = flightIdx;
                edata[1] = 1;
                AiMakeRadioResponse(self, rcROGER, edata);
            }
            else
            {
                AiRespondShortCallSign(self);
            }
        }

        private void AiSendUnauthNotice();
        //TODO private void AiHandleUnauthNotice(FalconWingmanMsg*);
        //	void AiQueryLeadGear();
        //	void AiHandleLeadGearUp(FalconWingmanMsg*);
        //	void AiHandleLeadGearDown(FalconWingmanMsg*);
        private void AiGlueWing()
        {
            mSplitFlight = FALSE;
        }

        private void AiSplitWing()
        {
            mSplitFlight = TRUE;
        }

        private void AiDropStores(FalconWingmanMsg* msg)
        {
            short[] edata = new short[10];
            int flightIdx;

            self.Sms.EmergencyJettison();

            flightIdx = self.GetCampaignObject().GetComponentIndex(self);
            if (AiIsFullResponse(flightIdx, msg.dataBlock.to))
            {
                edata[0] = flightIdx;
                edata[1] = 1;
                AiMakeRadioResponse(self, rcROGER, edata);
            }
            else
            {
                AiRespondShortCallSign(self);
            }
        }


        //TODO private void AiLightsOn(FalconWingmanMsg*);			// MN 2002-02-11
        //TODO private void AiLightsOff(FalconWingmanMsg*);

        private void AiECMOn(FalconWingmanMsg* msg)
        {
            short[] edata = new short[10];
            int flightIdx;


            // set a radar flag here
            self.SetFlag(ECM_ON);

            flightIdx = self.GetCampaignObject().GetComponentIndex(self);

            if (AiIsFullResponse(flightIdx, msg.dataBlock.to))
            {
                edata[0] = flightIdx;
                edata[1] = 0;
                AiMakeRadioResponse(self, rcECMON, edata);
            }
            else
            {
                AiRespondShortCallSign(self);
            }
        }


        private void AiECMOff(FalconWingmanMsg* msg)
        {
            short[] edata = new short[10];
            int flightIdx;

            // turn ECM off
            self.UnSetFlag(ECM_ON);
            // set a radar flag here
            flightIdx = self.GetCampaignObject().GetComponentIndex(self);

            if (AiIsFullResponse(flightIdx, msg.dataBlock.to))
            {
                edata[0] = flightIdx;
                edata[1] = 0;
                AiMakeRadioResponse(self, rcECMOFF, edata);
            }
            else
            {
                AiRespondShortCallSign(self);
            }
        }

        public void AiCheckPlayerInPosition();
        public void AiSetInPosition();
        //	void AiHandleGearQuery();


        // Execution for Maneuvers
        private void AiInitSSOffset(FalconWingmanMsg* msg)
        {
            float trigYaw;
            float XSelf;
            float YSelf;
            float ZSelf;
            mlTrig firstTrig;
            int flightIdx;
            short[] edata = new short[10];
            float side;

            AiSetManeuver(FalconWingmanMsg.WMPince);

            XSelf = self.XPos();
            YSelf = self.YPos();
            ZSelf = self.ZPos();

            // 2002-04-07 ADDED BY S.G. Since SSOffset is available on the menu while we have something bugged, why not use it?
            if (vuDatabase.Find(msg.dataBlock.newTarget))
            {
                mDesignatedObject = msg.dataBlock.newTarget;
                mpActionFlags[AI_ENGAGE_TARGET] = AI_AIR_TARGET; // 2002-03-04 MODIFIED BY S.G. Use new enum type
                mWeaponsAction = AI_WEAPONS_FREE;
                mpSearchFlags[AI_FIXATE_ON_TARGET] = TRUE;
                AiRunTargetSelection();
                //		mpActionFlags[AI_USE_COMPLEX]       = TRUE;
                //		if (mpActionFlags[AI_EXECUTE_MANEUVER])
                //			mpActionFlags[AI_EXECUTE_MANEUVER] = TRUE + 1;
            }

            // END OF ADDED SECTION
            // Find maneuver Axis
            // Start w/ bearing to target/threat,
            // if none use then lead's heading, else
            // use ownship heading
            //LRKLUDGE Need to check passed target here as well
            if (targetPtr)
            {
                trigYaw = self.Yaw() + TargetAz(self, targetPtr);
                mSpeedOrdered = self.Vt();
                mAltitudeOrdered = self.ZPos();
            }
            else if (flightLead)
            {
                trigYaw = flightLead.Yaw();
                mSpeedOrdered = flightLead.Vt();
                mAltitudeOrdered = flightLead.ZPos();
            }
            else
            {
                trigYaw = self.Yaw();
                mSpeedOrdered = self.Vt() * FTPSEC_TO_KNOTS;
                mAltitudeOrdered = self.ZPos();
            }

            // Get the angles
            mlSinCos(&firstTrig, trigYaw);

            // S.G. isWing has the index position of the plane in the flight. It is ALWAYS less than the number of planes
            // if (isWing > self.GetCampaignObject().NumberOfComponents())
            // Instead, odd plane number (wingmen) have the 1.0F side. Leaders (flight and element) have the -1.0F side
            // if (isWing & 1)
            // 2001-8-03 BUT INSTEAD, I'LL REVERSE IT SO THE WINGS GO TO THE LEFT
            if (!(isWing & 1))
                side = 1.0F;
            else
                side = -1.0F;

            // 2002-04-07 MODIFIED BY S.G. Replaced the constant by FalconSP.cfg vars
            mpManeuverPoints[0][0] = XSelf + firstTrig.cos * g_fSSoffsetManeuverPoints1a * Phyconst.NM_TO_FT - firstTrig.sin * g_fSSoffsetManeuverPoints1b * Phyconst.NM_TO_FT * side;
            mpManeuverPoints[0][1] = YSelf + firstTrig.sin * g_fSSoffsetManeuverPoints1a * Phyconst.NM_TO_FT + firstTrig.cos * g_fSSoffsetManeuverPoints1b * Phyconst.NM_TO_FT * side;
            // S.G. SECOND LEG IS JUST 4 NM, NOT 20 NM
            //	mpManeuverPoints[1][0]	= XSelf + firstTrig.cos * 20.0F * Phyconst.NM_TO_FT - firstTrig.sin * 5.0F * Phyconst.NM_TO_FT * side;
            //	mpManeuverPoints[1][1]	= YSelf + firstTrig.sin * 20.0F * Phyconst.NM_TO_FT + firstTrig.cos * 5.0F * Phyconst.NM_TO_FT * side;
            mpManeuverPoints[1][0] = XSelf + firstTrig.cos * g_fSSoffsetManeuverPoints2a * Phyconst.NM_TO_FT - firstTrig.sin * g_fSSoffsetManeuverPoints2b * Phyconst.NM_TO_FT * side;
            mpManeuverPoints[1][1] = YSelf + firstTrig.sin * g_fSSoffsetManeuverPoints2a * Phyconst.NM_TO_FT + firstTrig.cos * g_fSSoffsetManeuverPoints2b * Phyconst.NM_TO_FT * side;

            mPointCounter = 0;

            flightIdx = self.GetCampaignObject().GetComponentIndex(self);
            if (flightIdx && msg)
            {
                AiSplitFlight(msg.dataBlock.to, msg.dataBlock.from, flightIdx); // this has nothing to do witht he doSplit variable
            }


            if (msg == null)
            {
                AiRespondShortCallSign(self);
            }
            else
            {
                if (AiIsFullResponse(flightIdx, msg.dataBlock.to))
                {
                    edata[0] = flightIdx;
                    edata[1] = 9;	// fixed from "bracketing"
                    AiMakeRadioResponse(self, rcEXECUTERESPONSE, edata);
                    AiCheckFormStrip();
                }
                else
                {
                    AiRespondShortCallSign(self);
                }
            }
        }

        private void AiInitPince(FalconWingmanMsg* msg, int doSplit)
        {
            float trigYaw;
            float XSelf;
            float YSelf;
            float ZSelf;
            mlTrig firstTrig;
            int flightIdx;
            short[] edata = new short[10];
            float side;

            AiSetManeuver(FalconWingmanMsg.WMPince);

            XSelf = self.XPos();
            YSelf = self.YPos();
            ZSelf = self.ZPos();

            // 2002-04-07 ADDED BY S.G. Since Pince is available on the menu while we have something bugged, why not use it?
            if (vuDatabase.Find(msg.dataBlock.newTarget))
            {
                mDesignatedObject = msg.dataBlock.newTarget;
                mpActionFlags[AI_ENGAGE_TARGET] = AI_AIR_TARGET; // 2002-03-04 MODIFIED BY S.G. Use new enum type
                mWeaponsAction = AI_WEAPONS_FREE;
                mpSearchFlags[AI_FIXATE_ON_TARGET] = TRUE;
                AiRunTargetSelection();
                //		mpActionFlags[AI_USE_COMPLEX]       = TRUE;
                //		if (mpActionFlags[AI_EXECUTE_MANEUVER])
                //			mpActionFlags[AI_EXECUTE_MANEUVER] = TRUE + 1;
            }
            // END OF ADDED SECTION

            // Find maneuver Axis
            // Start w/ bearing to target/threat,
            // if none use then lead's heading, else
            // use ownship heading
            //LRKLUDGE Need to check passed target here as well
            if (targetPtr)
            {
                trigYaw = self.Yaw() + TargetAz(self, targetPtr);
                mSpeedOrdered = self.Vt();
                mAltitudeOrdered = self.ZPos();
            }
            else if (flightLead)
            {
                trigYaw = flightLead.Yaw();
                mSpeedOrdered = flightLead.Vt();
                mAltitudeOrdered = flightLead.ZPos();
            }
            else
            {
                trigYaw = self.Yaw();
                mSpeedOrdered = self.Vt() * FTPSEC_TO_KNOTS;
                mAltitudeOrdered = self.ZPos();
            }

            // Get the angles
            mlSinCos(&firstTrig, trigYaw);

            // S.G. isWing has the index position of the plane in the flight. It is ALWAYS less than the number of planes
            // if (doSplit && isWing > self.GetCampaignObject().NumberOfComponents())
            // Instead, odd plane number (wingmen) have the 1.0F side. Leaders (flight and element) have the -1.0F side
            if (doSplit && (isWing & 1))
                side = 1.0F;
            else
                side = -1.0F;

            // 2002-04-07 MODIFIED BY S.G. Replaced the constant by FalconSP.cfg vars
            mpManeuverPoints[0][0] = XSelf + firstTrig.cos * g_fPinceManeuverPoints1a * Phyconst.NM_TO_FT - firstTrig.sin * g_fPinceManeuverPoints1b * Phyconst.NM_TO_FT * side;
            mpManeuverPoints[0][1] = YSelf + firstTrig.sin * g_fPinceManeuverPoints1a * Phyconst.NM_TO_FT + firstTrig.cos * g_fPinceManeuverPoints1b * Phyconst.NM_TO_FT * side;
            // S.G. SECOND LEG IS JUST 4 NM, NOT 20 NM
            //	mpManeuverPoints[1][0]	= XSelf + firstTrig.cos * 20.0F * Phyconst.NM_TO_FT - firstTrig.sin * 5.0F * Phyconst.NM_TO_FT * side;
            //	mpManeuverPoints[1][1]	= YSelf + firstTrig.sin * 20.0F * Phyconst.NM_TO_FT + firstTrig.cos * 5.0F * Phyconst.NM_TO_FT * side;
            mpManeuverPoints[1][0] = XSelf + firstTrig.cos * g_fPinceManeuverPoints2a * Phyconst.NM_TO_FT - firstTrig.sin * g_fPinceManeuverPoints2b * Phyconst.NM_TO_FT * side;
            mpManeuverPoints[1][1] = YSelf + firstTrig.sin * g_fPinceManeuverPoints2a * Phyconst.NM_TO_FT + firstTrig.cos * g_fPinceManeuverPoints2b * Phyconst.NM_TO_FT * side;

            mPointCounter = 0;

            flightIdx = self.GetCampaignObject().GetComponentIndex(self);
            if (flightIdx && msg)
            {
                AiSplitFlight(msg.dataBlock.to, msg.dataBlock.from, flightIdx); // this has nothing to do witht he doSplit variable
            }


            if (msg == null)
            {
                AiRespondShortCallSign(self);
            }
            else
            {
                if (AiIsFullResponse(flightIdx, msg.dataBlock.to))
                {
                    edata[0] = flightIdx;
                    edata[1] = 4;
                    AiMakeRadioResponse(self, rcEXECUTERESPONSE, edata);
                    AiCheckFormStrip();
                }
                else
                {
                    AiRespondShortCallSign(self);
                }
            }
        }

        private void AiInitFlex()
        {
            mlTrig firstTrig;
            mlTrig secondTrig;
            float XSelf;
            float YSelf;
            float ZSelf;
            float spacing = Phyconst.NM_TO_FT;

            AiSetManeuver(FalconWingmanMsg.WMPince);

            XSelf = self.XPos();
            YSelf = self.YPos();
            ZSelf = self.ZPos();

            AiInitTrig(&firstTrig, &secondTrig);

            mpManeuverPoints[0][0] = XSelf + secondTrig.cos * spacing;
            mpManeuverPoints[0][1] = YSelf + firstTrig.sin * spacing;
            mpManeuverPoints[1][0] = XSelf - secondTrig.cos * 2.0F * spacing;
            mpManeuverPoints[1][1] = YSelf - secondTrig.sin * 2.0F * spacing;
            mpManeuverPoints[2][0] = XSelf - secondTrig.cos * 2.1F * spacing;
            mpManeuverPoints[2][1] = YSelf - secondTrig.sin * 2.1F * spacing;

            mAltitudeOrdered = self.ZPos();
            mSpeedOrdered = self.Vt() * FTPSEC_TO_KNOTS;
            mPointCounter = 0;
        }

        private void AiInitChainsaw(FalconWingmanMsg* msg)
        {
            int flightIdx;
            short[] edata = new short[10];

            // Find the target given

            flightIdx = self.GetCampaignObject().GetComponentIndex(self);

            if (vuDatabase.Find(msg.dataBlock.newTarget))
            {
                mDesignatedObject = msg.dataBlock.newTarget;
                mpActionFlags[AI_ENGAGE_TARGET] = AI_AIR_TARGET; // 2002-03-04 MODIFIED BY S.G. Use new enum type
                mWeaponsAction = AI_WEAPONS_FREE;
                mpSearchFlags[AI_FIXATE_ON_TARGET] = TRUE;
                AiSetManeuver(FalconWingmanMsg.WMChainsaw);
                AiRunTargetSelection();
                mpActionFlags[AI_USE_COMPLEX] = TRUE;
                AiSplitFlight(msg.dataBlock.to, msg.dataBlock.from, flightIdx);
                // 2002-03-15 ADDED BY S.G.
                if (mpActionFlags[AI_EXECUTE_MANEUVER])
                    mpActionFlags[AI_EXECUTE_MANEUVER] = TRUE + 1;
            }
            else
            {
                mDesignatedObject = FalconNullId;
                mpActionFlags[AI_ENGAGE_TARGET] = AI_NONE; // 2002-03-04 MODIFIED BY S.G. Use new enum type
                mWeaponsAction = AI_WEAPONS_HOLD;
                mpSearchFlags[AI_FIXATE_ON_TARGET] = FALSE;
                AiClearManeuver();
            }


            if (AiIsFullResponse(flightIdx, msg.dataBlock.to))
            {
                edata[0] = flightIdx;
                edata[1] = 6;
                AiMakeRadioResponse(self, rcEXECUTERESPONSE, edata);
                AiCheckFormStrip();
            }
            else
            {
                AiRespondShortCallSign(self);
            }
        }

        private void AiInitPosthole(FalconWingmanMsg* msg)
        {
            int flightIdx;
            short[] edata = new short[10];

            // Find the target given
            if (vuDatabase.Find(msg.dataBlock.newTarget))
            {
                mDesignatedObject = msg.dataBlock.newTarget;
                mpActionFlags[AI_ENGAGE_TARGET] = AI_AIR_TARGET; // 2002-03-04 MODIFIED BY S.G. Use new enum type
                mWeaponsAction = AI_WEAPONS_FREE;
                mpSearchFlags[AI_FIXATE_ON_TARGET] = TRUE;
                AiSetManeuver(FalconWingmanMsg.WMPosthole);
                AiRunTargetSelection();
                mpActionFlags[AI_USE_COMPLEX] = TRUE;
                mSpeedOrdered = cornerSpeed;
                trackX = self.XPos();
                trackY = self.YPos();
                trackZ = OTWDriver.GetGroundLevel(trackX, trackY) - 4000.0F;
                mPointCounter = 0;

                flightIdx = self.GetCampaignObject().GetComponentIndex(self);
                AiSplitFlight(msg.dataBlock.to, msg.dataBlock.from, flightIdx);

                if (msg == null)
                {
                    AiRespondShortCallSign(self);
                }
                else
                {
                    if (AiIsFullResponse(flightIdx, msg.dataBlock.to))
                    {
                        edata[0] = flightIdx;
                        edata[1] = 5;
                        AiMakeRadioResponse(self, rcEXECUTERESPONSE, edata);
                        AiCheckFormStrip();
                    }
                    else
                    {
                        AiCheckFormStrip();
                        AiRespondShortCallSign(self);
                    }
                }
                // 2002-03-15 ADDED BY S.G.
                if (mpActionFlags[AI_EXECUTE_MANEUVER])
                    mpActionFlags[AI_EXECUTE_MANEUVER] = TRUE + 1;
            }
            else
            {
                mDesignatedObject = FalconNullId;
                mpActionFlags[AI_ENGAGE_TARGET] = AI_NONE; // 2002-03-04 MODIFIED BY S.G. Use new enum type
                mWeaponsAction = AI_WEAPONS_HOLD;
                mpSearchFlags[AI_FIXATE_ON_TARGET] = FALSE;
                AiClearManeuver();
                edata[0] = ((FlightClass*)self.GetCampaignObject()).callsign_id;
                edata[1] = (((FlightClass*)self.GetCampaignObject()).callsign_num - 1) * 4 +
                self.GetCampaignObject().GetComponentIndex(self) + 1;
                edata[2] = -1;
                edata[3] = -1;
                edata[4] = 1;
                AiMakeRadioResponse(self, rcUNABLE, edata);
            }
        }
        private void AiInitTrig(mlTrig* firstTrig, mlTrig* secondTrig)
        {
            float trigYaw;

            // Find maneuver Axis
            // Start w/ bearing to target/threat,
            // if none use then lead's heading, else
            // use ownship heading
            if (targetPtr)
            {
                trigYaw = self.Yaw() + TargetAz(self, targetPtr);
            }
            else if (flightLead)
            {
                trigYaw = flightLead.Yaw();
            }
            else
            {
                trigYaw = self.Yaw();
            }

            // Get the angles
            mlSinCos(firstTrig, trigYaw);

            trigYaw += 90.0F * DTR;

            if (trigYaw > 180.0F * DTR)
            {
                trigYaw -= (360.0F * DTR);
            }
            mlSinCos(secondTrig, trigYaw);

            // Go left/right based on position in flight and
            // number of people in flight.
            // For a 2 ship 0 goes right, 1 goes left
            // In a 4 ship 0 & 1 go right, 2 & 3 go left

            if (isWing >= 2 || (isWing == 1 && self.GetCampaignObject().NumberOfComponents() < 3))
            {
                firstTrig.cos = -firstTrig.cos;
                firstTrig.sin = -firstTrig.sin;
            }

#if NOTHING
float		dx, dy, dz;
float		ry;
float		YawSelf;

	dx			= flightLead.XPos() - XSelf;
	dy			= flightLead.YPos() - YSelf;
	dz			= flightLead.ZPos() - ZSelf;

   ry			= self.dmx[1][0] * dx + self.dmx[1][1] * dy + self.dmx[1][2] * dz;

	YawSelf	= self.Yaw();

   mlSinCos(firstTrig, YawSelf);

	YawSelf += 90.0F * DTR;

	if(YawSelf > 180.0F * DTR) {
		YawSelf -= (360.0F * DTR);
	}

   mlSinCos(secondTrig, YawSelf);

	if(ry > 0.0F) {		// Break left
		firstTrig.cos			= -firstTrig.cos;
		firstTrig.sin			= -firstTrig.sin;
	}
#endif
        }

        private void AiExecBreakRL();
        private void AiExecPosthole();
        private void AiExecChainsaw()
        {
            if (mpActionFlags[AI_ENGAGE_TARGET] == AI_NONE) // 2002-03-04 ADDED BY S.G. Change it if not already set, assume an air target (can't tell)
                mpActionFlags[AI_ENGAGE_TARGET] = AI_AIR_TARGET; // 2002-03-04 MODIFIED BY S.G. Use new enum type
            mpActionFlags[AI_EXECUTE_MANEUVER] = FALSE;
            mWeaponsAction = AI_WEAPONS_FREE;

            mpSearchFlags[AI_FIXATE_ON_TARGET] = TRUE;
            mpSearchFlags[AI_MONITOR_TARGET] = FALSE;
            mpActionFlags[AI_RTB] = FALSE;

            AiClearManeuver();

            //MonoPrint("Going Shooter\n");
        }
        private void AiExecPince();
        private void AiExecFlex();
        private void AiExecClearSix();
        //TODO private void AiExecSearchForTargets(char);

        // End Wingman Stuff


#if USE_SH_POOLS
public:
	// Overload new/delete to use a SmartHeap fixed size pool
	void *operator new(size_t size) { Debug.Assert( size == sizeof(DigitalBrain) ); return MemAllocFS(pool);	};
	void operator delete(void *mem) { if (mem) MemFreeFS(mem); };
	static void InitializeStorage()	{ pool = MemPoolInitFS( sizeof(DigitalBrain), 200, 0 ); };
	static void ReleaseStorage()	{ MemPoolFree( pool ); };
	static MEM_POOL	pool;
#endif

        // 2001-06-04 ADDED BY S.G. HELP FUNCTION TO SEARCH FOR A GROUND TARGET
        private SimBaseClass* FindSimGroundTarget(CampBaseClass* targetGroup, int targetNumComponents, int startPos)
        {
            int i, gotRadar = FALSE;
            int usComponents = self.GetCampaignObject().NumberOfComponents();
            int haveHARMS = FALSE;
            int otherHaveHARMS = FALSE;
            SimBaseClass* simTarg = null;
            SimBaseClass* firstSimTarg = null;
            AircraftClass[] flightMember = new AircraftClass[4]; // Maximum of 4 planes per flight with no target as default

            // Get the flight aircrafts (once per call instead of once per target querried)
            for (i = 0; i < usComponents; i++)
            {
                // I onced tried to get the player's current target so it could be skipped by the AI but
                // all the player's are not on the same PC as the AI so this is not valid.
                // Therefore, only get this from digital planes, or the player if he is local
                // 2002-03-08 MODIFIED BY S.G. Code change so I'm only calling GetComponentEntity once and checking if it returns non null
                flightMember[i] = (AircraftClass*)self.GetCampaignObject().GetComponentEntity(i);
                if (flightMember[i] && (flightMember[i].isDigital || flightMember[i].IsLocal()))
                {
                    // Now that we know it's local (hopefully, digitals are also local), see if they are carrying HARMS
                    // By first looking at their 'hasHARM' status
                    if (flightMember[i].DBrain().hasHARM)
                    {
                        if (i == isWing)
                            haveHARMS = TRUE;
                        else
                            otherHaveHARMS = TRUE;
                    }
                    // Can't rely on 'hasHARM' so check the loadout of each plane yourself...
                    else
                    {
                        // Try the loadout as long I didn't find an HARM on mine or another plane higher than us has an HARM
                        for (int j = 0; !haveHARMS && !(otherHaveHARMS && i > isWing) && flightMember[i].Sms && j < flightMember[i].Sms.NumHardpoints(); j++)
                        {
                            if (flightMember[i].Sms.hardPoint[j].weaponPointer && flightMember[i].Sms.hardPoint[j].GetWeaponClass() == wcHARMWpn)
                            {
                                if (i == isWing)
                                    haveHARMS = TRUE;
                                else
                                    otherHaveHARMS = TRUE;
                            }
                        }
                    }
                }
                else
                    flightMember[i] = null;
            }

            // Check each sim entity in the campaign entity in succession, starting at startPos.
            // When incrementing i, use 0 if we had a 'startPos' but it wasn't valid

            //#define SG_TEST_NO_RANDOM_TARGET // Define this to make sure an AI always goes at the same target. Makes it easier to debug with constant behaviour
#if  !SG_TEST_NO_RANDOM_TARGET
            // 2001-10-19 ADDED BY S.G. IF WE ARE USING AN HARM OR A MAVERICK, DON'T RANDOMIZE
            if (!haveHARMS && !hasAGMissile && targetNumComponents)
            {
                // JB 011014 Target "randomly" if its just a long line of vehicles
                if (startPos == 0)
                {
                    startPos = rand() % targetNumComponents;
                    for (i = startPos; i < targetNumComponents && startPos > 0; i++)
                        if (!targetGroup.GetComponentEntity(i).IsVehicle() || !targetGroup.GetComponentEntity(i).OnGround())
                            startPos = 0;
                }
                // JB 011014
            }
#endif

            for (i = startPos; i < targetNumComponents; i = startPos ? 0 : i + 1, startPos = 0)
            {
                // Get the sim object associated to this entity number
                simTarg = targetGroup.GetComponentEntity(i);

                if (!simTarg) //sanity check
                    continue;

                // Is it alive?
                if (simTarg.IsExploding() || simTarg.IsDead() || simTarg.pctStrength <= 0.0f)
                    continue; // Dead thing, ignore it.

                // Are flight members already using it (was using it) as a target?
                for (int j = 0; j < usComponents; j++)
                    if (flightMember[j] && flightMember[j].DBrain() && ((flightMember[j].DBrain().groundTargetPtr && flightMember[j].DBrain().groundTargetPtr.BaseData() == simTarg) || flightMember[j].DBrain().gndTargetHistory[0] == simTarg || flightMember[j].DBrain().gndTargetHistory[1] == simTarg))
                        break;  // Yes, ignore it.

                // If we didn't reach the end, someone else is using it so skip it.
                if (j != usComponents)
                    continue;

                // Mark this sim entity as the first target with a match (in case no emitting targets are left standing, or it's a feature)
                if (!firstSimTarg)
                    firstSimTarg = simTarg;

                // Is it an objective and we are not carrying HARMS (HARMS will go for the radar feature)? If so, stop right now and use that feature
                if (targetGroup.IsObjective() && !hasHARM)
                    break;

                // If I have HARMS or no one has any and the entity has a radar, choose it
                // 2001-07-12 S.G. Testing if radar first so it's not becoming an air defense if i have no harms
                if ((simTarg.IsVehicle() && ((SimVehicleClass*)simTarg).GetRadarType() != RDR_NO_RADAR) ||	// It's a vehicle and it has a radar
                     (simTarg.IsStatic() && ((SimStaticClass*)simTarg).GetRadarType() != RDR_NO_RADAR))
                {	// It's a feature and it has a radar
                    // 2001-07-29 S.G. If I was shooting at the campaign object, then I stick to it
                    if ((((FlightClass*)self.GetCampaignObject()).shotAt == targetGroup && ((FlightClass*)self.GetCampaignObject()).whoShot == self) || ((((FlightClass*)self.GetCampaignObject()).whoShot == null) && (haveHARMS || !otherHaveHARMS)))
                    {
                        gotRadar = TRUE;
                        firstSimTarg = simTarg; // Yes, use it for a target
                        break; // and stop looking
                    }
                }
                // 2001-07-12 S.G. DON'T CHECK IF AIR DEFENSE IF IT'S A RADAR
                else
                {
                    // Prioritize air defense thingies...
                    if (simTarg.IsGroundVehicle() && ((GroundClass*)simTarg).isAirDefense)
                    {
                        firstSimTarg = simTarg; // Yes, use it for a target
                        // 2001-07-12 S.G. DON'T CONTINUE IF I HAVE NO HARMS
                        if (!haveHARMS)
                            break;
                    }
                }
                // Look for the next one...
            }
            if (startPos < targetNumComponents)
            {
                // Now after all this, see if the AI is too dumb for selecting a valid target... 
                // If he is, select at random
                if (!gotRadar && (unsigned)((unsigned)rand() % (unsigned)32) > (unsigned)((unsigned)SkillLevel() + (unsigned)28))
                    firstSimTarg = targetGroup.GetComponentEntity(rand() % targetNumComponents);

                // Keep track of the last two targets but only if we have one, otherwise, leave our previous targets alone
                if (firstSimTarg)
                {
                    gndTargetHistory[1] = gndTargetHistory[0];
                    gndTargetHistory[0] = firstSimTarg;
                }
            }
            else
                firstSimTarg = 0;

            // 2001-07-29 S.G. If I was shooting at a (not necessarely THIS one) campaign object, say I'm not anymore since the object deaggregated.
            if (((FlightClass*)self.GetCampaignObject()).whoShot == self)
            {
                ((FlightClass*)self.GetCampaignObject()).shotAt = null;
                ((FlightClass*)self.GetCampaignObject()).whoShot = null;
            }

            // JB 011017 from Schumi if targetNumComponents is less than usComponents, then of course there is no target anymore for the wingmen to bomb, and firstSimTarg is null.
            if (firstSimTarg == null && targetNumComponents && targetNumComponents < usComponents)
                firstSimTarg = targetGroup.GetComponentEntity(rand() % targetNumComponents);

            return firstSimTarg;
        }

        // 2001-06-28 ADDED BY S.G. HELPER FUNCTION TO TEST IF IT'S A SEAD MISSION ON ITS MAIN TARGET OR NOT
        private int IsNotMainTargetSEAD()
        {
            // SEADESCORT has no main target
            if (missionType == AMIS_SEADESCORT)
                return true;

            // Only for SEADS missions
            if (missionType != AMIS_SEADSTRIKE)
                return false;

            // If not ground target, we can't check if it's our main target, right? So assume it's not our main target
            if (!groundTargetPtr)
                return true;

            // Get the target campaign object if it's a sim
            CampBaseClass* campBaseTarget = ((CampBaseClass*)groundTargetPtr.BaseData());
            if (((SimBaseClass*)groundTargetPtr.BaseData()).IsSim())
                campBaseTarget = ((SimBaseClass*)groundTargetPtr.BaseData()).GetCampaignObject();

            // We should always have a waypoint but if we don't, can't be our attack waypoint right?
            if (!self.curWaypoint)
                return true;

            // If it's not our main target, it's ok to stop attacking it
            if (self.curWaypoint.GetWPTarget() != campBaseTarget)
                return true;

            // It's our main target, keep pounding it to death...
            return false;
        }

        // 2001-12-17 Added by M.N. To get a sim object from a divert target
        private SimBaseClass* FindSimAirTarget(CampBaseClass* targetGroup, int targetNumComponents, int startPos)
        {
            int i, gotRadar = FALSE;
            int usComponents = self.GetCampaignObject().NumberOfComponents();
            int haveHARMS = FALSE;
            int otherHaveHARMS = FALSE;
            SimBaseClass* simTarg = null;
            SimBaseClass* firstSimTarg = null;
            AircraftClass[] flightMember = new AircraftClass[4]; // Maximum of 4 planes per flight with no target as default

            // Get the flight aircrafts (once per call instead of once per target querried)
            for (i = 0; i < usComponents; i++)
            {
                // I onced tried to get the player's current target so it could be skipped by the AI but
                // all the player's are not on the same PC as the AI so this is not valid.
                // Therefore, only get this from digital planes, or the player if he is local
                // 2002-03-08 MODIFIED BY S.G. Code change so I'm only calling GetComponentEntity once and checking if it returns non null
                flightMember[i] = (AircraftClass*)self.GetCampaignObject().GetComponentEntity(i);
                if (flightMember[i] && (!flightMember[i].isDigital && !flightMember[i].IsLocal()))
                {
                    flightMember[i] = null;
                }
            }

            // Check each sim entity in the campaign entity in succession, starting at startPos.
            // When incrementing i, use 0 if we had a 'startPos' but it wasn't valid

            for (i = startPos; i < targetNumComponents; i = startPos ? 0 : i + 1, startPos = 0)
            {
                // Get the sim object associated to this entity number
                simTarg = targetGroup.GetComponentEntity(i);

                if (!simTarg) //sanity check
                    continue;

                // Is it alive?
                if (simTarg.IsExploding() || simTarg.IsDead() || simTarg.pctStrength <= 0.0f)
                    continue; // Dead thing, ignore it.

                // Are flight members already using it (was using it) as a target?
                for (int j = 0; j < usComponents; j++)		// MN - use the gndtargethistory for divert air targets, too.
                    if (flightMember[j] && flightMember[j].DBrain() && ((flightMember[j].DBrain().airtargetPtr && flightMember[j].DBrain().airtargetPtr.BaseData() == simTarg) || flightMember[j].DBrain().gndTargetHistory[0] == simTarg || flightMember[j].DBrain().gndTargetHistory[1] == simTarg))
                        break;  // Yes, ignore it.

                // If we didn't reach the end, someone else is using it so skip it.
                if (j != usComponents)
                    continue;

                // Mark this sim entity as the first target with a match (in case no emitting targets are left standing, or it's a feature)
                if (!firstSimTarg)
                    firstSimTarg = simTarg;

                // Look for the next one...
            }
            if (startPos < targetNumComponents)
            {
                // Keep track of the last two targets but only if we have one, otherwise, leave our previous targets alone
                if (firstSimTarg)
                {
                    gndTargetHistory[1] = gndTargetHistory[0];
                    gndTargetHistory[0] = firstSimTarg;
                }
            }
            else
                firstSimTarg = 0;

            // JB 011017 from Schumi if targetNumComponents is less than usComponents, then of course there is no target anymore for the wingmen to bomb, and firstSimTarg is null.
            if (firstSimTarg == null && targetNumComponents && targetNumComponents < usComponents)
                firstSimTarg = targetGroup.GetComponentEntity(rand() % targetNumComponents);

            return firstSimTarg;
        }

        // 2002-01-29 ADDED BY S.G. To set the various target spot used to keep target deaggregated after the player issue an Attack target command

        public VuEntity* targetSpotWing;
        public VuEntity* targetSpotElement;
        public VuEntity* targetSpotFlight;
        public FalconEntity* targetSpotWingTarget;
        public FalconEntity* targetSpotElementTarget;
        public FalconEntity* targetSpotFlightTarget;
        public VU_TIME targetSpotWingTimer;
        public VU_TIME targetSpotElementTimer;
        public VU_TIME targetSpotFlightTimer;
        public VU_TIME radarModeTest; // 2002-02-10 ADDED BY S.G.
        public int tiebreaker; // JPO - for ground manuveur
        public VU_ID escortFlightID; // 2002-02-27 ADDED BY S.G. Instead of calculating it everytime in BVR, let's find it here...
        public AG_DOCTRINE GetAGDoctrine() { return agDoctrine; } // 2002-03-08 MN so leader can check if wingmen are still engaging a ground target
        public uint moreFlags;	// 2002-03-08 ADDED BY S.G. atcFlags ran out of bits...

        /////////////////////////////////////////////////////////
        public static int[] WingmanTable = new int[4];

        public static ACFormationData* acFormationData;

        private static int HoldCorner(int combatClass, SimObjectType* targetPtr)
        {
            int i, hisCombatClass;
            DigitalBrain.ManeuverChoiceTable* theIntercept;
            int retval = true; // Assume you can engage
            return true;
            //me123 hack hack request from saint 
            //always alow corner hold until we fix the disengage stuff

            // Only check for A/C
            // if (targetPtr.BaseData().IsSim() && targetPtr.BaseData().IsAirplane())
            if (targetPtr.BaseData().IsAirplane() || targetPtr.BaseData().IsFlight()) // 2002-02-26 MODIFIED BY S.G. airplane and fligth are ok in here
            {
                // Find the data table for these two types of A/C
                hisCombatClass = targetPtr.BaseData().CombatClass(); // 2002-02-26 MODIFIED BY S.G. Removed the AircraftClass cast
                theIntercept = &(DigitalBrain.maneuverData[combatClass][hisCombatClass]);

                for (i = 0; i < theIntercept.numMerges; i++)
                {
                    if (theIntercept.merge[0] == DigitalBrain.WvrMergeHitAndRun)
                    {
                        retval = false;
                        break;
                    }
                }
                //me123 don't blow corner for now, i wanna test them 
                //     if (!retval)
                //     {
                // Chance of blowing corner speed is proportional to your number of choices
                //        if ((float)rand()/(float)RAND_MAX > 1.0F / theIntercept.numMerges)
                //           retval = true;
                //     }
            }

            return retval;
        }
        static int EngineStart(AircraftClass* self)
        {
            if (self.af.IsSet(AirframeClass.EngineStopped))
            {
                if (self.af.rpm > 0.2f)
                { // Jfs is runnning time to light
                    self.af.SetThrotl(0.1f); // advance the throttle to light.
                    self.af.ClearFlag(AirframeClass.EngineStopped);
                }
                else if (self.af.IsSet(AirframeClass.JfsStart))
                { // nothing much happening
                }
                else
                { // start the JFS
                    self.af.JfsEngineStart();
                }
                return 0;
            }
            else
            {
                if (self.af.rpm > 0.69f)
                { // at idle
                    self.af.SetThrotl(0.0f);
                    return 1; // finished engine start up
                }
                else if (self.af.rpm > 0.5f)
                { // engine now running
                    self.af.SetThrotl(0.0f);
                }
            }
            return 0;
        }
        public enum Actions
        {
            CANOPY, FUEL1, FUEL2, FNX, RALTON, POWERON, AFFLAGS, RADAR, SEAT, MAINPOWER, EWSPGM,
            MASTERARM, EXTLON, INS, VOLUME, FLAPS
        };

        public struct PreflightActions
        {
            int action; // what to do, 
            int data;	// any data
            int timedelay; // how many seconds til next one
            PreFlightFnx fnx;
        }

        PreflightActions[] PreFlightTable = {
    { PreflightActions.CANOPY, 0, 1, null},
    { PreflightActions.FUEL1, 0, 1, null},
    { PreflightActions.FUEL2, 0, 1, null},
    { PreflightActions.MAINPOWER, 0, 1, null},
    { PreflightActions.FNX, 0, 0, EngineStart},
    { PreflightActions.RALTON, 0, 1, null },
    { PreflightActions.POWERON, AircraftClass.HUDPower, 6, null},
    { PreflightActions.POWERON, AircraftClass.MFDPower, 2, null},
    { PreflightActions.POWERON, AircraftClass.FCCPower, 1, null},
    { PreflightActions.POWERON, AircraftClass.SMSPower, 1, null},
    { PreflightActions.POWERON, AircraftClass.UFCPower, 1, null},
    { PreflightActions.POWERON, AircraftClass.FCRPower, 3, null},
    { PreflightActions.POWERON, AircraftClass.TISLPower, 1, null},
    { PreflightActions.POWERON, AircraftClass.LeftHptPower, 1, null},
    { PreflightActions.POWERON, AircraftClass.RightHptPower, 1, null},
    { PreflightActions.POWERON, AircraftClass.GPSPower, 1, null},
    { PreflightActions.POWERON, AircraftClass.DLPower, 1, null},
    { PreflightActions.POWERON, AircraftClass.MAPPower, 1, null},
    { PreflightActions.POWERON, AircraftClass.RwrPower, 1, null},
    { PreflightActions.RADAR, 0, 1, null},
	{ PreflightActions.POWERON, AircraftClass.EWSRWRPower, 2, null},
	{ PreflightActions.POWERON, AircraftClass.EWSJammerPower, 2, null},
	{ PreflightActions.POWERON, AircraftClass.EWSChaffPower, 2, null},
	{ PreflightActions.POWERON, AircraftClass.EWSFlarePower, 2, null},
	{ PreflightActions.POWERON, AircraftClass.IFFPower, 2, null},
	{ PreflightActions.EWSPGM, 0,1,null},
	{ PreflightActions.EXTLON, AircraftClass.Extl_Main_Power, 1, null},	//exterior lighting
	{ PreflightActions.EXTLON, AircraftClass.Extl_Anit_Coll, 1, null},	//exterior lighting
	{ PreflightActions.EXTLON, AircraftClass.Extl_Wing_Tail, 1, null},	//exterior lighting
	{ PreflightActions.INS, AircraftClass.INS_AlignNorm, 485, null},	//time to align
	{ PreflightActions.INS, AircraftClass.INS_Nav,0, null},
	{ PreflightActions.VOLUME, 1,1, null},
	{ PreflightActions.VOLUME, 2,1, null},
	{ PreflightActions.AFFLAGS, AirframeClass.NoseSteerOn, 3, null }, //shortly before taxi
	{ PreflightActions.MASTERARM, 0, 1, null},	//MasterArm
	{ PreflightActions.SEAT, 0, 1, null},	//this comes all at the end
    { PreflightActions.FLAPS, 0, 1, null},
};
        static readonly int MAX_PF_ACTIONS = PreFlightTable.Length; // sizeof(PreFlightTable) / sizeof(PreFlightTable[0]);


    }
#endif
}
