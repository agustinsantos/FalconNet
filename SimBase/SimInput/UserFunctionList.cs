using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalconNet.SimBase.SimInput
{
    public struct UserFunctionListEntry
    {
        public UserFunctionListEntry(InputFunctionType f)
        {
            this.theFunc = f;
            this.funcName = f.Method.Name;
        }

        public InputFunctionType theFunc;
        public string funcName;
    }
    public static class UserFunction
    {
        public static UserFunctionListEntry[] UserFunctionList = new UserFunctionListEntry[]
         {
                 new UserFunctionListEntry(Commands.OTWTrackExternal),
                 new UserFunctionListEntry(Commands.OTWTrackTargetToWeapon),
                 new UserFunctionListEntry(Commands.OTWToggleScoreDisplay),
                 new UserFunctionListEntry(Commands.OTWToggleSidebar),
                 new UserFunctionListEntry(Commands.SimRadarAAModeStep),
                 new UserFunctionListEntry(Commands.SimRadarAGModeStep),
                 new UserFunctionListEntry(Commands.SimRadarGainUp),
                 new UserFunctionListEntry(Commands.SimRadarGainDown),
                 new UserFunctionListEntry(Commands.SimRadarStandby),
                 new UserFunctionListEntry(Commands.SimRadarRangeStepUp),
                 new UserFunctionListEntry(Commands.SimRadarRangeStepDown),
                 new UserFunctionListEntry(Commands.SimRadarNextTarget),
                 new UserFunctionListEntry(Commands.SimRadarPrevTarget),
                 new UserFunctionListEntry(Commands.SimRadarBarScanChange),
                 new UserFunctionListEntry(Commands.SimRadarAzimuthScanChange),
                 new UserFunctionListEntry(Commands.SimRadarFOVStep),
                 new UserFunctionListEntry(Commands.SimMaverickFOVStep),
                 new UserFunctionListEntry(Commands.SimSOIFOVStep),
                 new UserFunctionListEntry(Commands.SimRadarFreeze),
                 new UserFunctionListEntry(Commands.SimRadarSnowplow),
                 new UserFunctionListEntry(Commands.SimRadarCursorZero),
                 new UserFunctionListEntry(Commands.SimACMBoresight),
                 new UserFunctionListEntry(Commands.SimDesignate),
                 new UserFunctionListEntry(Commands.SimACMVertical),
                 new UserFunctionListEntry(Commands.SimDropTrack),
                 new UserFunctionListEntry(Commands.SimACMSlew),
                 new UserFunctionListEntry(Commands.SimACM30x20),
                 new UserFunctionListEntry(Commands.SimRadarElevationDown),
                 new UserFunctionListEntry(Commands.SimRadarElevationUp),
                 new UserFunctionListEntry(Commands.SimRWRSetPriority),
                 //new UserFunctionListEntry(Commands.SimRWRSetSound),
                 new UserFunctionListEntry(Commands.SimRWRSetTargetSep),
                 new UserFunctionListEntry(Commands.SimRWRSetUnknowns),
                 new UserFunctionListEntry(Commands.SimRWRSetNaval),
                 new UserFunctionListEntry(Commands.SimRWRSetGroundPriority),
                 new UserFunctionListEntry(Commands.SimRWRSetSearch),
                 new UserFunctionListEntry(Commands.SimRWRHandoff),
                 new UserFunctionListEntry(Commands.SimNextWaypoint),
                 new UserFunctionListEntry(Commands.SimPrevWaypoint),
                 new UserFunctionListEntry(Commands.SimTogglePaused),
                 new UserFunctionListEntry(Commands.SimPickle),
                 new UserFunctionListEntry(Commands.SimTrigger),
                 new UserFunctionListEntry(Commands.SimMissileStep),
                 new UserFunctionListEntry(Commands.SimCursorUp),
                 new UserFunctionListEntry(Commands.SimCursorDown),
                 new UserFunctionListEntry(Commands.SimCursorLeft),
                 new UserFunctionListEntry(Commands.SimCursorRight),
                 new UserFunctionListEntry(Commands.SimToggleAutopilot),
                 new UserFunctionListEntry(Commands.SimStepSMSLeft),
                 new UserFunctionListEntry(Commands.SimStepSMSRight),
                 new UserFunctionListEntry(Commands.SimSelectSRMOverride),
                 new UserFunctionListEntry(Commands.SimSelectMRMOverride),
                 new UserFunctionListEntry(Commands.SimDeselectOverride),
                 new UserFunctionListEntry(Commands.SimToggleMissileCage),
                // Marco Edit - AIM9 Spot/Scan Support
                 new UserFunctionListEntry(Commands.SimToggleMissileSpotScan),
                 new UserFunctionListEntry(Commands.SimToggleMissileBoreSlave),
                 new UserFunctionListEntry(Commands.SimToggleMissileTDBPUncage),

                 new UserFunctionListEntry(Commands.SimDropChaff),
                 new UserFunctionListEntry(Commands.SimDropFlare),
                 new UserFunctionListEntry(Commands.SimHSDRangeStepUp),
                 new UserFunctionListEntry(Commands.SimHSDRangeStepDown),
                 new UserFunctionListEntry(Commands.SimToggleInvincible),
                 new UserFunctionListEntry(Commands.SimFCCSubModeStep),
                 new UserFunctionListEntry(Commands.SimEndFlight),
                 new UserFunctionListEntry(Commands.SimNextAAWeapon),
                 new UserFunctionListEntry(Commands.SimNextAGWeapon),
                 new UserFunctionListEntry(Commands.SimNextNavMode),
                 new UserFunctionListEntry(Commands.SimEject),
                 new UserFunctionListEntry(Commands.AFBrakesOut),
                 new UserFunctionListEntry(Commands.AFBrakesIn),
                 new UserFunctionListEntry(Commands.AFBrakesToggle),
                 new UserFunctionListEntry(Commands.AFGearToggle),
                 new UserFunctionListEntry(Commands.AFGearUp),  // MD
                 new UserFunctionListEntry(Commands.AFGearDown),  // MD
                 new UserFunctionListEntry(Commands.AFElevatorUp),
                 new UserFunctionListEntry(Commands.AFElevatorDown),
                 new UserFunctionListEntry(Commands.AFAileronLeft),
                 new UserFunctionListEntry(Commands.AFAileronRight),
                 new UserFunctionListEntry(Commands.AFThrottleUp),
                 new UserFunctionListEntry(Commands.AFThrottleDown),
                 new UserFunctionListEntry(Commands.AFRudderRight),
                 new UserFunctionListEntry(Commands.AFRudderLeft),
                 new UserFunctionListEntry(Commands.AFCoarseThrottleUp),
                 new UserFunctionListEntry(Commands.AFCoarseThrottleDown),
                 new UserFunctionListEntry(Commands.AFABOn),
                 new UserFunctionListEntry(Commands.AFIdle),
                 new UserFunctionListEntry(Commands.OTWTimeOfDayStep),
                 new UserFunctionListEntry(Commands.OTWStepNextAC),
                 new UserFunctionListEntry(Commands.OTWStepPrevAC),
                 new UserFunctionListEntry(Commands.OTWStepNextPadlock),
                 new UserFunctionListEntry(Commands.OTWStepPrevPadlock),
                 new UserFunctionListEntry(Commands.OTWStepNextPadlockAA), // 2002-03-12 S.G.
                 new UserFunctionListEntry(Commands.OTWStepPrevPadlockAA), // 2002-03-12 S.G.
                 new UserFunctionListEntry(Commands.OTWStepNextPadlockAG), // 2002-03-12 S.G.
                 new UserFunctionListEntry(Commands.OTWStepPrevPadlockAG), // 2002-03-12 S.G.
                 new UserFunctionListEntry(Commands.OTWToggleNames),
                 new UserFunctionListEntry(Commands.OTWToggleCampNames),
                 new UserFunctionListEntry(Commands.OTWSelectF3PadlockMode),
                 new UserFunctionListEntry(Commands.OTWSelectF3PadlockModeAA), // 2002-03-12 S.G.
                 new UserFunctionListEntry(Commands.OTWSelectF3PadlockModeAG), // 2002-03-12 S.G.
                 new UserFunctionListEntry(Commands.OTWSelectEFOVPadlockMode),
                 new UserFunctionListEntry(Commands.OTWSelectEFOVPadlockModeAA), // 2002-03-12 S.G.
                 new UserFunctionListEntry(Commands.OTWSelectEFOVPadlockModeAG), // 2002-03-12 S.G.
                 new UserFunctionListEntry(Commands.OTWRadioMenuStep),
                 new UserFunctionListEntry(Commands.OTWRadioMenuStepBack),
                 new UserFunctionListEntry(Commands.OTWStepMFD1),
                 new UserFunctionListEntry(Commands.OTWStepMFD2),
                 new UserFunctionListEntry(Commands.OTWStepMFD3),
                 new UserFunctionListEntry(Commands.OTWStepMFD4),
                 new UserFunctionListEntry(Commands.OTWToggleScales),
                 new UserFunctionListEntry(Commands.OTWToggleActionCamera),
                 new UserFunctionListEntry(Commands.OTWTogglePitchLadder),
                 new UserFunctionListEntry(Commands.SimPitchLadderOff),  // MD
                 new UserFunctionListEntry(Commands.SimPitchLadderFPM),  // MD
                 new UserFunctionListEntry(Commands.SimPitchLadderATTFPM),  // MD
                 new UserFunctionListEntry(Commands.OTWStepHeadingScale),
                 new UserFunctionListEntry(Commands.OTWSelectHUDMode),
                 new UserFunctionListEntry(Commands.OTWToggleGLOC),
                 new UserFunctionListEntry(Commands.OTWSelectChaseMode),
                 new UserFunctionListEntry(Commands.OTWSelectOrbitMode),
                 new UserFunctionListEntry(Commands.OTWSelectAirFriendlyMode),
                 new UserFunctionListEntry(Commands.OTWSelectGroundFriendlyMode),
                 new UserFunctionListEntry(Commands.OTWSelectAirEnemyMode),
                 new UserFunctionListEntry(Commands.OTWSelectGroundEnemyMode),
                 new UserFunctionListEntry(Commands.OTWSelectTargetMode),
                 new UserFunctionListEntry(Commands.OTWSelectWeaponMode),
                 new UserFunctionListEntry(Commands.OTWSelectSatelliteMode),
                 new UserFunctionListEntry(Commands.OTWSelectFlybyMode),
                 new UserFunctionListEntry(Commands.OTWSelectIncomingMode),
                 new UserFunctionListEntry(Commands.OTWShowTestVersion),
                 new UserFunctionListEntry(Commands.OTWShowVersion),
                 new UserFunctionListEntry(Commands.OTWSelect2DCockpitMode),
                 new UserFunctionListEntry(Commands.OTWSelect3DCockpitMode),
                 new UserFunctionListEntry(Commands.OTWToggleBilinearFilter),
                 new UserFunctionListEntry(Commands.OTWToggleShading),
                 new UserFunctionListEntry(Commands.OTWToggleHaze),
                 new UserFunctionListEntry(Commands.OTWToggleLocationDisplay),
                 new UserFunctionListEntry(Commands.OTWToggleAeroDisplay), // JPO
                 new UserFunctionListEntry(Commands.OTWToggleFlapDisplay), // TJL 11/09/03 On/Off Flap display
                 new UserFunctionListEntry(Commands.OTWToggleEngineDisplay), // Retro 1Feb2004
                 new UserFunctionListEntry(Commands.OTWScaleDown),
                 new UserFunctionListEntry(Commands.OTWScaleUp),
                 new UserFunctionListEntry(Commands.OTWSetObjDetail),
                 new UserFunctionListEntry(Commands.OTWObjDetailDown),
                 new UserFunctionListEntry(Commands.OTWObjDetailUp),
                //JAM 01Dec03 - Removing this USER_FUNCTION(OTWTextureIncrease),
                //JAM 01Dec03 - Removing this USER_FUNCTION(OTWTextureDecrease),
                 new UserFunctionListEntry(Commands.OTWToggleClouds),
                 new UserFunctionListEntry(Commands.OTWStepHudContrastDn),
                 new UserFunctionListEntry(Commands.OTWStepHudContrastUp),
                 new UserFunctionListEntry(Commands.OTWToggleEyeFly),
                 new UserFunctionListEntry(Commands.OTWEnterPosition),
                 new UserFunctionListEntry(Commands.OTWToggleFrameRate),
                 new UserFunctionListEntry(Commands.OTWToggleAutoScale),
                 new UserFunctionListEntry(Commands.OTWSetScale),
                 new UserFunctionListEntry(Commands.OTWViewLeft),
                 new UserFunctionListEntry(Commands.OTWViewRight),
                 new UserFunctionListEntry(Commands.OTWViewUp),
                 new UserFunctionListEntry(Commands.OTWViewDown),
                 new UserFunctionListEntry(Commands.OTWViewReset),
                 new UserFunctionListEntry(Commands.OTWViewUpRight),
                 new UserFunctionListEntry(Commands.OTWViewUpLeft),
                 new UserFunctionListEntry(Commands.OTWViewDownRight),
                 new UserFunctionListEntry(Commands.OTWViewDownLeft),
                 new UserFunctionListEntry(Commands.OTWViewZoomIn),
                 new UserFunctionListEntry(Commands.OTWViewZoomOut),
                 new UserFunctionListEntry(Commands.OTWSwapMFDS),
                 new UserFunctionListEntry(Commands.OTWGlanceForward),
                 new UserFunctionListEntry(Commands.OTWCheckSix),
                 new UserFunctionListEntry(Commands.OTWStateStep),
                 new UserFunctionListEntry(Commands.CommandsSetKeyCombo),
                 new UserFunctionListEntry(Commands.KevinsFistOfGod),
                 new UserFunctionListEntry(Commands.SuperCruise),
                 new UserFunctionListEntry(Commands.OTW1200View),
                 new UserFunctionListEntry(Commands.OTW1200DView),
                 new UserFunctionListEntry(Commands.OTW1200HUDView),
                 new UserFunctionListEntry(Commands.OTW1200LView),
                 new UserFunctionListEntry(Commands.OTW1000View),
                 new UserFunctionListEntry(Commands.OTW200View),
                 new UserFunctionListEntry(Commands.OTW900View),
                 new UserFunctionListEntry(Commands.OTW300View),
                 new UserFunctionListEntry(Commands.OTW800View),
                 new UserFunctionListEntry(Commands.OTW400View),
                 new UserFunctionListEntry(Commands.OTW1200RView),
                 new UserFunctionListEntry(Commands.RadioMessageSend),
                 new UserFunctionListEntry(Commands.SimToggleChatMode),
                 new UserFunctionListEntry(Commands.SimMotionFreeze),
                 new UserFunctionListEntry(Commands.ScreenShot),
                 new UserFunctionListEntry(Commands.PrettyScreenShot), // Retro 7May2004
                 new UserFunctionListEntry(Commands.FOVToggle),
                 new UserFunctionListEntry(Commands.FOVDecrease), //Wombat778 9-27-2003
                 new UserFunctionListEntry(Commands.FOVIncrease), //Wombat778 9-27-2003
                 new UserFunctionListEntry(Commands.FOVDefault), //Wombat778 9-27-2003
                //JAM 01Dec03 - Removing this USER_FUNCTION(OTWToggleAlpha),
                 new UserFunctionListEntry(Commands.SimAVTRToggle),
                 new UserFunctionListEntry(Commands.SimSelectiveJettison),
                 new UserFunctionListEntry(Commands.SimEmergencyJettison),
                 new UserFunctionListEntry(Commands.SimWheelBrakes),
                 new UserFunctionListEntry(Commands.SimECMOn),
                 new UserFunctionListEntry(Commands.SimECMStandby), //Wombat778 11-3-2003 + MD 20031128
                 new UserFunctionListEntry(Commands.SimECMConsent), //Wombat778 11-3-2003 + MD 20031128
                 new UserFunctionListEntry(Commands.SimRadarElevationCenter),
                 new UserFunctionListEntry(Commands.SimHsiCourseInc),
                 new UserFunctionListEntry(Commands.SimHsiCourseDec),
                 new UserFunctionListEntry(Commands.SimHsiHeadingInc),
                 new UserFunctionListEntry(Commands.SimHsiHeadingDec),
                 new UserFunctionListEntry(Commands.SimHsiCrsIncBy1),  // MD -- 20040118
                 new UserFunctionListEntry(Commands.SimHsiCrsDecBy1),  // MD -- 20040118
                 new UserFunctionListEntry(Commands.SimHsiHdgIncBy1),  // MD -- 20040118
                 new UserFunctionListEntry(Commands.SimHsiHdgDecBy1),  // MD -- 20040118
                 new UserFunctionListEntry(Commands.SimAVTRToggle),
                 new UserFunctionListEntry(Commands.SimMPOToggle),
                 new UserFunctionListEntry(Commands.SimMPO),  // MD
                 new UserFunctionListEntry(Commands.SimSilenceHorn),
                 new UserFunctionListEntry(Commands.SimStepHSIMode),
                 new UserFunctionListEntry(Commands.SimHSIIlsTcn),  // MD
                 new UserFunctionListEntry(Commands.SimHSITcn),  // MD
                 new UserFunctionListEntry(Commands.SimHSINav),  // MD
                 new UserFunctionListEntry(Commands.SimHSIIlsNav),  // MD
                 new UserFunctionListEntry(Commands.SimCBEOSB_1L),
                 new UserFunctionListEntry(Commands.SimCBEOSB_2L),
                 new UserFunctionListEntry(Commands.SimCBEOSB_3L),
                 new UserFunctionListEntry(Commands.SimCBEOSB_4L),
                 new UserFunctionListEntry(Commands.SimCBEOSB_5L),
                 new UserFunctionListEntry(Commands.SimCBEOSB_6L),
                 new UserFunctionListEntry(Commands.SimCBEOSB_7L),
                 new UserFunctionListEntry(Commands.SimCBEOSB_8L),
                 new UserFunctionListEntry(Commands.SimCBEOSB_9L),
                 new UserFunctionListEntry(Commands.SimCBEOSB_10L),
                 new UserFunctionListEntry(Commands.SimCBEOSB_11L),
                 new UserFunctionListEntry(Commands.SimCBEOSB_12L),
                 new UserFunctionListEntry(Commands.SimCBEOSB_13L),
                 new UserFunctionListEntry(Commands.SimCBEOSB_14L),
                 new UserFunctionListEntry(Commands.SimCBEOSB_15L),
                 new UserFunctionListEntry(Commands.SimCBEOSB_16L),
                 new UserFunctionListEntry(Commands.SimCBEOSB_17L),
                 new UserFunctionListEntry(Commands.SimCBEOSB_18L),
                 new UserFunctionListEntry(Commands.SimCBEOSB_19L),
                 new UserFunctionListEntry(Commands.SimCBEOSB_20L),
                 new UserFunctionListEntry(Commands.SimCBEOSB_1R),
                 new UserFunctionListEntry(Commands.SimCBEOSB_2R),
                 new UserFunctionListEntry(Commands.SimCBEOSB_3R),
                 new UserFunctionListEntry(Commands.SimCBEOSB_4R),
                 new UserFunctionListEntry(Commands.SimCBEOSB_5R),
                 new UserFunctionListEntry(Commands.SimCBEOSB_6R),
                 new UserFunctionListEntry(Commands.SimCBEOSB_7R),
                 new UserFunctionListEntry(Commands.SimCBEOSB_8R),
                 new UserFunctionListEntry(Commands.SimCBEOSB_9R),
                 new UserFunctionListEntry(Commands.SimCBEOSB_10R),
                 new UserFunctionListEntry(Commands.SimCBEOSB_11R),
                 new UserFunctionListEntry(Commands.SimCBEOSB_12R),
                 new UserFunctionListEntry(Commands.SimCBEOSB_13R),
                 new UserFunctionListEntry(Commands.SimCBEOSB_14R),
                 new UserFunctionListEntry(Commands.SimCBEOSB_15R),
                 new UserFunctionListEntry(Commands.SimCBEOSB_16R),
                 new UserFunctionListEntry(Commands.SimCBEOSB_17R),
                 new UserFunctionListEntry(Commands.SimCBEOSB_18R),
                 new UserFunctionListEntry(Commands.SimCBEOSB_19R),
                 new UserFunctionListEntry(Commands.SimCBEOSB_20R),
                 new UserFunctionListEntry(Commands.SimCBEOSB_GAINUP_L),
                 new UserFunctionListEntry(Commands.SimCBEOSB_GAINUP_R),
                 new UserFunctionListEntry(Commands.SimCBEOSB_GAINDOWN_L),
                 new UserFunctionListEntry(Commands.SimCBEOSB_GAINDOWN_R),

                //Wombat778 4-12-04
                 new UserFunctionListEntry(Commands.SimCBEOSB_1T),
                 new UserFunctionListEntry(Commands.SimCBEOSB_2T),
                 new UserFunctionListEntry(Commands.SimCBEOSB_3T),
                 new UserFunctionListEntry(Commands.SimCBEOSB_4T),
                 new UserFunctionListEntry(Commands.SimCBEOSB_5T),
                 new UserFunctionListEntry(Commands.SimCBEOSB_6T),
                 new UserFunctionListEntry(Commands.SimCBEOSB_7T),
                 new UserFunctionListEntry(Commands.SimCBEOSB_8T),
                 new UserFunctionListEntry(Commands.SimCBEOSB_9T),
                 new UserFunctionListEntry(Commands.SimCBEOSB_10T),
                 new UserFunctionListEntry(Commands.SimCBEOSB_11T),
                 new UserFunctionListEntry(Commands.SimCBEOSB_12T),
                 new UserFunctionListEntry(Commands.SimCBEOSB_13T),
                 new UserFunctionListEntry(Commands.SimCBEOSB_14T),
                 new UserFunctionListEntry(Commands.SimCBEOSB_15T),
                 new UserFunctionListEntry(Commands.SimCBEOSB_16T),
                 new UserFunctionListEntry(Commands.SimCBEOSB_17T),
                 new UserFunctionListEntry(Commands.SimCBEOSB_18T),
                 new UserFunctionListEntry(Commands.SimCBEOSB_19T),
                 new UserFunctionListEntry(Commands.SimCBEOSB_20T),
                 new UserFunctionListEntry(Commands.SimCBEOSB_1F),
                 new UserFunctionListEntry(Commands.SimCBEOSB_2F),
                 new UserFunctionListEntry(Commands.SimCBEOSB_3F),
                 new UserFunctionListEntry(Commands.SimCBEOSB_4F),
                 new UserFunctionListEntry(Commands.SimCBEOSB_5F),
                 new UserFunctionListEntry(Commands.SimCBEOSB_6F),
                 new UserFunctionListEntry(Commands.SimCBEOSB_7F),
                 new UserFunctionListEntry(Commands.SimCBEOSB_8F),
                 new UserFunctionListEntry(Commands.SimCBEOSB_9F),
                 new UserFunctionListEntry(Commands.SimCBEOSB_10F),
                 new UserFunctionListEntry(Commands.SimCBEOSB_11F),
                 new UserFunctionListEntry(Commands.SimCBEOSB_12F),
                 new UserFunctionListEntry(Commands.SimCBEOSB_13F),
                 new UserFunctionListEntry(Commands.SimCBEOSB_14F),
                 new UserFunctionListEntry(Commands.SimCBEOSB_15F),
                 new UserFunctionListEntry(Commands.SimCBEOSB_16F),
                 new UserFunctionListEntry(Commands.SimCBEOSB_17F),
                 new UserFunctionListEntry(Commands.SimCBEOSB_18F),
                 new UserFunctionListEntry(Commands.SimCBEOSB_19F),
                 new UserFunctionListEntry(Commands.SimCBEOSB_20F),
                 new UserFunctionListEntry(Commands.SimCBEOSB_GAINUP_T),
                 new UserFunctionListEntry(Commands.SimCBEOSB_GAINUP_F),
                 new UserFunctionListEntry(Commands.SimCBEOSB_GAINDOWN_T),
                 new UserFunctionListEntry(Commands.SimCBEOSB_GAINDOWN_F),

                //Wombat778 End

                 new UserFunctionListEntry(Commands.SimICPTILS),
                 new UserFunctionListEntry(Commands.SimICPALOW),
                 new UserFunctionListEntry(Commands.SimICPFAck),
                 new UserFunctionListEntry(Commands.SimICPPrevious),
                 new UserFunctionListEntry(Commands.SimICPNext),
                 new UserFunctionListEntry(Commands.SimICPLink),
                 new UserFunctionListEntry(Commands.SimICPCrus),
                 new UserFunctionListEntry(Commands.SimICPStpt),
                 new UserFunctionListEntry(Commands.SimICPMark),
                 new UserFunctionListEntry(Commands.SimICPEnter),
                 new UserFunctionListEntry(Commands.SimICPCom1),
                 new UserFunctionListEntry(Commands.SimICPNav),
                 new UserFunctionListEntry(Commands.SimICPAA),
                 new UserFunctionListEntry(Commands.SimICPAG),
                 new UserFunctionListEntry(Commands.SimHUDScales),
                 new UserFunctionListEntry(Commands.SimScalesVVVAH),  // MD
                 new UserFunctionListEntry(Commands.SimScalesVAH),  // MD
                 new UserFunctionListEntry(Commands.SimScalesOff),  // MD
                 new UserFunctionListEntry(Commands.SimHUDFPM),
                 new UserFunctionListEntry(Commands.SimHUDDED),
                 new UserFunctionListEntry(Commands.SimHUDDEDOff),  // MD
                 new UserFunctionListEntry(Commands.SimHUDDEDPFL),  // MD
                 new UserFunctionListEntry(Commands.SimHUDDEDDED),  // MD
                 new UserFunctionListEntry(Commands.SimHUDVelocity),
                 new UserFunctionListEntry(Commands.SimHUDVelocityCAS),  // MD
                 new UserFunctionListEntry(Commands.SimHUDVelocityTAS),  // MD
                 new UserFunctionListEntry(Commands.SimHUDVelocityGND),  // MD
                 new UserFunctionListEntry(Commands.SimHUDRadar),
                 new UserFunctionListEntry(Commands.SimHUDAltRadar),  // MD
                 new UserFunctionListEntry(Commands.SimHUDAltBaro),  // MD
                 new UserFunctionListEntry(Commands.SimHUDAltAuto),  // MD
                 new UserFunctionListEntry(Commands.OTWStepHudColor),
                 new UserFunctionListEntry(Commands.SimHUDBrightness),
                 new UserFunctionListEntry(Commands.SimHUDBrtDay),  // MD
                 new UserFunctionListEntry(Commands.SimHUDBrtAuto),  // MD
                 new UserFunctionListEntry(Commands.SimHUDBrtNight),  // MD
                 new UserFunctionListEntry(Commands.SimHUDBrightnessUp), //MI
                 new UserFunctionListEntry(Commands.SimHUDBrightnessDown), //MI
                 new UserFunctionListEntry(Commands.SimCycleRadioChannel),
                 new UserFunctionListEntry(Commands.SimDecRadioChannel),  // MD
                 new UserFunctionListEntry(Commands.SimToggleRadioVolume),
                 new UserFunctionListEntry(Commands.RadioTankerCommand),
                 new UserFunctionListEntry(Commands.RadioTowerCommand),
                 new UserFunctionListEntry(Commands.RadioAWACSCommand),
                 new UserFunctionListEntry(Commands.RadioWingCommand),
                 new UserFunctionListEntry(Commands.RadioElementCommand),
                 new UserFunctionListEntry(Commands.RadioFlightCommand),
                 new UserFunctionListEntry(Commands.WingmanClearSix),
                 new UserFunctionListEntry(Commands.ElementClearSix),
                 new UserFunctionListEntry(Commands.FlightClearSix),
                 new UserFunctionListEntry(Commands.WingmanCheckSix),
                 new UserFunctionListEntry(Commands.ElementCheckSix),
                 new UserFunctionListEntry(Commands.FlightCheckSix),
                 new UserFunctionListEntry(Commands.WingmanBreakLeft),
                 new UserFunctionListEntry(Commands.ElementBreakLeft),
                 new UserFunctionListEntry(Commands.FlightBreakLeft),
                 new UserFunctionListEntry(Commands.WingmanBreakRight),
                 new UserFunctionListEntry(Commands.ElementBreakRight),
                 new UserFunctionListEntry(Commands.FlightBreakRight),
                 new UserFunctionListEntry(Commands.WingmanPince),
                 new UserFunctionListEntry(Commands.ElementPince),
                 new UserFunctionListEntry(Commands.FlightPince),
                 new UserFunctionListEntry(Commands.WingmanPosthole),
                 new UserFunctionListEntry(Commands.ElementPosthole),
                 new UserFunctionListEntry(Commands.FlightPosthole),
                 new UserFunctionListEntry(Commands.WingmanChainsaw),
                 new UserFunctionListEntry(Commands.ElementChainsaw),
                 new UserFunctionListEntry(Commands.FlightChainsaw),
                 new UserFunctionListEntry(Commands.WingmanFlex),
                 new UserFunctionListEntry(Commands.ElementFlex),
                 new UserFunctionListEntry(Commands.FlightFlex),
                 new UserFunctionListEntry(Commands.WingmanGoShooterMode),
                 new UserFunctionListEntry(Commands.ElementGoShooterMode),
                 new UserFunctionListEntry(Commands.FlightGoShooterMode),
                 new UserFunctionListEntry(Commands.WingmanGoCoverMode),
                 new UserFunctionListEntry(Commands.ElementGoCoverMode),
                 new UserFunctionListEntry(Commands.FlightGoCoverMode),
                 new UserFunctionListEntry(Commands.WingmanSearchGround),
                 new UserFunctionListEntry(Commands.ElementSearchGround),
                 new UserFunctionListEntry(Commands.FlightSearchGround),
                 new UserFunctionListEntry(Commands.WingmanSearchAir),
                 new UserFunctionListEntry(Commands.ElementSearchAir),
                 new UserFunctionListEntry(Commands.FlightSearchAir),
                 new UserFunctionListEntry(Commands.WingmanResumeNormal),
                 new UserFunctionListEntry(Commands.ElementResumeNormal),
                 new UserFunctionListEntry(Commands.FlightResumeNormal),
                 new UserFunctionListEntry(Commands.WingmanRejoin),
                 new UserFunctionListEntry(Commands.ElementRejoin),
                 new UserFunctionListEntry(Commands.FlightRejoin),
                 new UserFunctionListEntry(Commands.WingmanDesignateTarget),
                 new UserFunctionListEntry(Commands.ElementDesignateTarget),
                 new UserFunctionListEntry(Commands.FlightDesignateTarget),
                 new UserFunctionListEntry(Commands.WingmanDesignateGroup),
                 new UserFunctionListEntry(Commands.ElementDesignateGroup),
                 new UserFunctionListEntry(Commands.FlightDesignateGroup),
                 new UserFunctionListEntry(Commands.WingmanWeaponsHold),
                 new UserFunctionListEntry(Commands.ElementWeaponsHold),
                 new UserFunctionListEntry(Commands.FlightWeaponsHold),
                 new UserFunctionListEntry(Commands.WingmanWeaponsFree),
                 new UserFunctionListEntry(Commands.ElementWeaponsFree),
                 new UserFunctionListEntry(Commands.FlightWeaponsFree),
                 new UserFunctionListEntry(Commands.WingmanWedge),
                 new UserFunctionListEntry(Commands.ElementWedge),
                 new UserFunctionListEntry(Commands.FlightWedge),
                 new UserFunctionListEntry(Commands.WingmanTrail),
                 new UserFunctionListEntry(Commands.ElementTrail),
                 new UserFunctionListEntry(Commands.FlightTrail),
                 new UserFunctionListEntry(Commands.WingmanResCell),
                 new UserFunctionListEntry(Commands.ElementResCell),
                 new UserFunctionListEntry(Commands.FlightResCell),
                 new UserFunctionListEntry(Commands.WingmanBox),
                 new UserFunctionListEntry(Commands.ElementBox),
                 new UserFunctionListEntry(Commands.FlightBox),
                 new UserFunctionListEntry(Commands.WingmanArrow),
                 new UserFunctionListEntry(Commands.ElementArrow),
                 new UserFunctionListEntry(Commands.FlightArrow),
                 new UserFunctionListEntry(Commands.WingmanKickout),
                 new UserFunctionListEntry(Commands.ElementKickout),
                 new UserFunctionListEntry(Commands.FlightKickout),
                 new UserFunctionListEntry(Commands.WingmanCloseup),
                 new UserFunctionListEntry(Commands.ElementCloseup),
                 new UserFunctionListEntry(Commands.FlightCloseup),
                 new UserFunctionListEntry(Commands.WingmanToggleSide),
                 new UserFunctionListEntry(Commands.ElementToggleSide),
                 new UserFunctionListEntry(Commands.FlightToggleSide),
                 new UserFunctionListEntry(Commands.WingmanIncreaseRelAlt),
                 new UserFunctionListEntry(Commands.ElementIncreaseRelAlt),
                 new UserFunctionListEntry(Commands.FlightIncreaseRelAlt),
                 new UserFunctionListEntry(Commands.WingmanDecreaseRelAlt),
                 new UserFunctionListEntry(Commands.ElementDecreaseRelAlt),
                 new UserFunctionListEntry(Commands.FlightDecreaseRelAlt),
                 new UserFunctionListEntry(Commands.WingmanGiveBra),
                 new UserFunctionListEntry(Commands.ElementGiveBra),
                 new UserFunctionListEntry(Commands.FlightGiveBra),
                 new UserFunctionListEntry(Commands.WingmanGiveStatus),
                 new UserFunctionListEntry(Commands.ElementGiveStatus),
                 new UserFunctionListEntry(Commands.FlightGiveStatus),
                 new UserFunctionListEntry(Commands.WingmanGiveDamageReport),
                 new UserFunctionListEntry(Commands.ElementGiveDamageReport),
                 new UserFunctionListEntry(Commands.FlightGiveDamageReport),
                 new UserFunctionListEntry(Commands.WingmanGiveFuelState),
                 new UserFunctionListEntry(Commands.ElementGiveFuelState),
                 new UserFunctionListEntry(Commands.FlightGiveFuelState),
                 new UserFunctionListEntry(Commands.WingmanGiveWeaponsCheck),
                 new UserFunctionListEntry(Commands.ElementGiveWeaponsCheck),
                 new UserFunctionListEntry(Commands.FlightGiveWeaponsCheck),
                 new UserFunctionListEntry(Commands.WingmanRTB),
                 new UserFunctionListEntry(Commands.ElementRTB),
                 new UserFunctionListEntry(Commands.FlightRTB),
                 new UserFunctionListEntry(Commands.SimSpeedyGonzalesUp),
                 new UserFunctionListEntry(Commands.SimSpeedyGonzalesDown),
                 new UserFunctionListEntry(Commands.ATCRequestClearance),
                 new UserFunctionListEntry(Commands.ATCRequestEmergencyClearance),
                 new UserFunctionListEntry(Commands.ATCRequestTakeoff),
                 new UserFunctionListEntry(Commands.ATCRequestTaxi),
                 new UserFunctionListEntry(Commands.ATCTaxiing),
                 new UserFunctionListEntry(Commands.ATCReadyToGo),
                 new UserFunctionListEntry(Commands.ATCRotate),
                 new UserFunctionListEntry(Commands.ATCGearUp),
                 new UserFunctionListEntry(Commands.ATCGearDown),
                 new UserFunctionListEntry(Commands.ATCBrake),
                 new UserFunctionListEntry(Commands.ATCAbortApproach),
                 new UserFunctionListEntry(Commands.FACCheckIn),
                 new UserFunctionListEntry(Commands.FACWilco),
                 new UserFunctionListEntry(Commands.FACUnable),
                 new UserFunctionListEntry(Commands.FACReady),
                 new UserFunctionListEntry(Commands.FACIn),
                 new UserFunctionListEntry(Commands.FACOut),
                 new UserFunctionListEntry(Commands.FACRequestMark),
                 new UserFunctionListEntry(Commands.FACRequestTarget),
                 new UserFunctionListEntry(Commands.FACRequestBDA),
                 new UserFunctionListEntry(Commands.FACRequestLocation),
                 new UserFunctionListEntry(Commands.FACRequestTACAN),
                 new UserFunctionListEntry(Commands.TankerRequestFuel),
                 new UserFunctionListEntry(Commands.TankerReadyForGas),
                 new UserFunctionListEntry(Commands.TankerDoneRefueling),
                 new UserFunctionListEntry(Commands.TankerBreakaway),
                 new UserFunctionListEntry(Commands.AWACSRequestPicture),
                 new UserFunctionListEntry(Commands.AWACSRequestTanker),
                 new UserFunctionListEntry(Commands.AWACSWilco),
                 new UserFunctionListEntry(Commands.AWACSUnable),
                 new UserFunctionListEntry(Commands.AWACSRequestHelp),
                 new UserFunctionListEntry(Commands.AWACSRequestRelief),
                 new UserFunctionListEntry(Commands.TimeAccelerate),
                 new UserFunctionListEntry(Commands.TimeAccelerateMaxToggle),
                 new UserFunctionListEntry(Commands.TimeAccelerateInc), // JB 010109
                 new UserFunctionListEntry(Commands.TimeAccelerateDec), // JB 010109
                 new UserFunctionListEntry(Commands.SimFuelDump), // JB 020313
                 new UserFunctionListEntry(Commands.SimCycleDebugLabels), // JB 020316
                 new UserFunctionListEntry(Commands.AFABFull),
                 new UserFunctionListEntry(Commands.BombRippleIncrement),
                 new UserFunctionListEntry(Commands.BombIntervalIncrement),
                 new UserFunctionListEntry(Commands.BombRippleDecrement),
                 new UserFunctionListEntry(Commands.BombIntervalDecrement),
                 new UserFunctionListEntry(Commands.BombPairRelease),
                 new UserFunctionListEntry(Commands.BombSGLRelease),
                 new UserFunctionListEntry(Commands.BombBurstIncrement),
                 new UserFunctionListEntry(Commands.BombBurstDecrement),
                 new UserFunctionListEntry(Commands.BreakToggle),
                 new UserFunctionListEntry(Commands.SimICPCom2),
                 new UserFunctionListEntry(Commands.SimToggleDropPattern),
                 new UserFunctionListEntry(Commands.KneeboardTogglePage),
                 new UserFunctionListEntry(Commands.ToggleNVGMode),
                 new UserFunctionListEntry(Commands.ToggleSmoke),
                 new UserFunctionListEntry(Commands.WingmanSpread),
                 new UserFunctionListEntry(Commands.ElementSpread),
                 new UserFunctionListEntry(Commands.FlightSpread),
                 new UserFunctionListEntry(Commands.WingmanStack),
                 new UserFunctionListEntry(Commands.ElementStack),
                 new UserFunctionListEntry(Commands.FlightStack),
                 new UserFunctionListEntry(Commands.WingmanLadder),
                 new UserFunctionListEntry(Commands.ElementLadder),
                 new UserFunctionListEntry(Commands.FlightLadder),
                 new UserFunctionListEntry(Commands.WingmanFluid),
                 new UserFunctionListEntry(Commands.ElementFluid),
                 new UserFunctionListEntry(Commands.FlightFluid),
                 new UserFunctionListEntry(Commands.SimOpenChatBox),
                 new UserFunctionListEntry(Commands.ExtinguishMasterCaution),
                 new UserFunctionListEntry(Commands.SoundOff),
                 new UserFunctionListEntry(Commands.SimToggleExtLights),
                 new UserFunctionListEntry(Commands.IncreaseAlow),
                 new UserFunctionListEntry(Commands.DecreaseAlow),
                 new UserFunctionListEntry(Commands.SaveCockpitDefaults),
                 new UserFunctionListEntry(Commands.LoadCockpitDefaults),
                 new UserFunctionListEntry(Commands.SimStepMasterArm),
                 new UserFunctionListEntry(Commands.SimArmMasterArm),
                 new UserFunctionListEntry(Commands.SimSafeMasterArm),
                 new UserFunctionListEntry(Commands.SimSimMasterArm),
                 new UserFunctionListEntry(Commands.SimSetBubbleSize), // JB 000509
                 new UserFunctionListEntry(Commands.SimHookToggle), // JB carrier
                 new UserFunctionListEntry(Commands.SimHookUp),  // MD
                 new UserFunctionListEntry(Commands.SimHookDown),  // MD
                 new UserFunctionListEntry(Commands.SimThrottleIdleDetent), // JPO
                 new UserFunctionListEntry(Commands.SimJfsStart), // JPO
                 new UserFunctionListEntry(Commands.SimEpuToggle), // JPO
                 new UserFunctionListEntry(Commands.SimEpuOff), // MD
                 new UserFunctionListEntry(Commands.SimEpuAuto), // MD
                 new UserFunctionListEntry(Commands.SimEpuOn), // MD
                 new UserFunctionListEntry(Commands.AFRudderTrimLeft), // JPO
                 new UserFunctionListEntry(Commands.AFRudderTrimRight), // JPO
                 new UserFunctionListEntry(Commands.AFAileronTrimLeft), // JPO
                 new UserFunctionListEntry(Commands.AFAileronTrimRight), // JPO
                 new UserFunctionListEntry(Commands.AFElevatorTrimUp), // JPO
                 new UserFunctionListEntry(Commands.AFElevatorTrimDown), // JPO
                 new UserFunctionListEntry(Commands.AFResetTrim), // JPO
                 new UserFunctionListEntry(Commands.AFAlternateGear), // JPO
                 new UserFunctionListEntry(Commands.AFAlternateGearReset), // JPO
                 new UserFunctionListEntry(Commands.SimFLIRToggle), // JPO
                 new UserFunctionListEntry(Commands.SimToggleTFR), // JPO
                 new UserFunctionListEntry(Commands.SimMainPowerInc), // JPO
                 new UserFunctionListEntry(Commands.SimMainPowerDec), // JPO
                 new UserFunctionListEntry(Commands.SimMainPowerOff), // MD
                 new UserFunctionListEntry(Commands.SimMainPowerBatt), // MD
                 new UserFunctionListEntry(Commands.SimMainPowerMain), // MD
                 new UserFunctionListEntry(Commands.AFFullFlap), // JPO
                 new UserFunctionListEntry(Commands.AFNoFlap), // JPO
                 new UserFunctionListEntry(Commands.AFIncFlap), // JPO
                 new UserFunctionListEntry(Commands.AFDecFlap), // JPO
                 new UserFunctionListEntry(Commands.AFFullLEF), // JPO
                 new UserFunctionListEntry(Commands.AFNoLEF), // JPO
                 new UserFunctionListEntry(Commands.AFIncLEF), // JPO
                 new UserFunctionListEntry(Commands.AFDecLEF), // JPO
                 new UserFunctionListEntry(Commands.AFDragChute),// JPO
                 new UserFunctionListEntry(Commands.AFCanopyToggle),// JPO
                 new UserFunctionListEntry(Commands.Sim3DCkptHelpOnOff),
                //MI
                 new UserFunctionListEntry(Commands.SimICPIFF),
                 new UserFunctionListEntry(Commands.SimICPLIST),
                 new UserFunctionListEntry(Commands.SimICPTHREE),
                 new UserFunctionListEntry(Commands.SimICPSIX),
                 new UserFunctionListEntry(Commands.SimICPEIGHT),
                 new UserFunctionListEntry(Commands.SimICPNINE),
                 new UserFunctionListEntry(Commands.SimICPZERO),
                 new UserFunctionListEntry(Commands.SimICPResetDED),
                 new UserFunctionListEntry(Commands.SimICPDEDUP),
                 new UserFunctionListEntry(Commands.SimICPDEDDOWN),
                 new UserFunctionListEntry(Commands.SimICPDEDSEQ),
                 new UserFunctionListEntry(Commands.SimICPCLEAR),
                 new UserFunctionListEntry(Commands.SimRALTSTDBY),
                 new UserFunctionListEntry(Commands.SimRALTON),
                 new UserFunctionListEntry(Commands.SimRALTOFF),
                 new UserFunctionListEntry(Commands.SimLandingLightToggle),
                 new UserFunctionListEntry(Commands.SimLandingLightOn),  // MD
                 new UserFunctionListEntry(Commands.SimLandingLightOff),  // MD
                 new UserFunctionListEntry(Commands.SimParkingBrakeToggle),
                 new UserFunctionListEntry(Commands.SimParkingBrakeOn),  // MD
                 new UserFunctionListEntry(Commands.SimParkingBrakeOff),  // MD
                 new UserFunctionListEntry(Commands.SimLaserArmToggle),
                 new UserFunctionListEntry(Commands.SimLaserArmOn),  // MD
                 new UserFunctionListEntry(Commands.SimLaserArmOff),  // MD
                 new UserFunctionListEntry(Commands.SimFuelDoorToggle),
                 new UserFunctionListEntry(Commands.SimFuelDoorOpen),  // MD
                 new UserFunctionListEntry(Commands.SimFuelDoorClose),  // MD
                 new UserFunctionListEntry(Commands.SimRightAPSwitch),
                 new UserFunctionListEntry(Commands.SimLeftAPSwitch),  // MD
                 new UserFunctionListEntry(Commands.SimLeftAPUp),  // MD
                 new UserFunctionListEntry(Commands.SimLeftAPMid),  // MD
                 new UserFunctionListEntry(Commands.SimLeftAPDown),  // MD
                 new UserFunctionListEntry(Commands.SimRightAPUp),  // MD
                 new UserFunctionListEntry(Commands.SimRightAPMid),  // MD
                 new UserFunctionListEntry(Commands.SimRightAPDown),  // MD
                 new UserFunctionListEntry(Commands.SimAPOverride),
                 new UserFunctionListEntry(Commands.SimWarnReset),
                 new UserFunctionListEntry(Commands.SimReticleSwitch),
                 new UserFunctionListEntry(Commands.SimReticlePri),  // MD
                 new UserFunctionListEntry(Commands.SimReticleStby),  // MD
                 new UserFunctionListEntry(Commands.SimReticleOff),  // MD
                 new UserFunctionListEntry(Commands.SimTMSUp),
                 new UserFunctionListEntry(Commands.SimTMSLeft),
                 new UserFunctionListEntry(Commands.SimTMSDown),
                 new UserFunctionListEntry(Commands.SimTMSRight),
                 new UserFunctionListEntry(Commands.SimSeatArm),
                 new UserFunctionListEntry(Commands.SimSeatOn),  // MD
                 new UserFunctionListEntry(Commands.SimSeatOff),  // MD
                 new UserFunctionListEntry(Commands.SimEWSRWRPower),
                 new UserFunctionListEntry(Commands.SimEWSRWROn),  // MD
                 new UserFunctionListEntry(Commands.SimEWSRWROff),  // MD
                 new UserFunctionListEntry(Commands.SimEWSJammerPower),
                 new UserFunctionListEntry(Commands.SimEWSJammerOn),  // MD
                 new UserFunctionListEntry(Commands.SimEWSJammerOff),  // MD
                 new UserFunctionListEntry(Commands.SimEWSChaffPower),
                 new UserFunctionListEntry(Commands.SimEWSChaffOn),  // MD
                 new UserFunctionListEntry(Commands.SimEWSChaffOff),  // MD
                 new UserFunctionListEntry(Commands.SimEWSFlarePower),
                 new UserFunctionListEntry(Commands.SimEWSFlareOn),  // MD
                 new UserFunctionListEntry(Commands.SimEWSFlareOff),  // MD
                 new UserFunctionListEntry(Commands.SimEWSPGMInc),
                 new UserFunctionListEntry(Commands.SimEWSPGMDec),
                 new UserFunctionListEntry(Commands.SimEWSModeOff),  // MD
                 new UserFunctionListEntry(Commands.SimEWSModeStby),  // MD
                 new UserFunctionListEntry(Commands.SimEWSModeMan),  // MD
                 new UserFunctionListEntry(Commands.SimEWSModeSemi),  // MD
                 new UserFunctionListEntry(Commands.SimEWSModeAuto),  // MD
                 new UserFunctionListEntry(Commands.SimEWSProgInc),
                 new UserFunctionListEntry(Commands.SimEWSProgDec),
                 new UserFunctionListEntry(Commands.SimEWSProgOne),  // MD
                 new UserFunctionListEntry(Commands.SimEWSProgTwo),  // MD
                 new UserFunctionListEntry(Commands.SimEWSProgThree),  // MD
                 new UserFunctionListEntry(Commands.SimEWSProgFour),  // MD
                 new UserFunctionListEntry(Commands.SimInhibitVMS),
                 new UserFunctionListEntry(Commands.SimVMSOn),  // MD
                 new UserFunctionListEntry(Commands.SimVMSOff),  // MD
                 new UserFunctionListEntry(Commands.SimRFSwitch),
                 new UserFunctionListEntry(Commands.SimRFNorm),  // MD
                 new UserFunctionListEntry(Commands.SimRFQuiet),  // MD
                 new UserFunctionListEntry(Commands.SimRFSilent),  // MD
                 new UserFunctionListEntry(Commands.SimDropProgrammed),
                 new UserFunctionListEntry(Commands.SimPinkySwitch),
                 new UserFunctionListEntry(Commands.SimGndJettEnable),
                 new UserFunctionListEntry(Commands.SimGndJettOn),  // MD
                 new UserFunctionListEntry(Commands.SimGndJettOff),  // MD
                 new UserFunctionListEntry(Commands.SimExtlPower),
                 new UserFunctionListEntry(Commands.SimExtlMasterNorm),  // MD
                 new UserFunctionListEntry(Commands.SimExtlMasterOff),  // MD
                 new UserFunctionListEntry(Commands.SimExtlAntiColl),
                 new UserFunctionListEntry(Commands.SimAntiCollOn),  // MD
                 new UserFunctionListEntry(Commands.SimAntiCollOff),  // MD
                 new UserFunctionListEntry(Commands.SimExtlSteady),
                 new UserFunctionListEntry(Commands.SimLightsSteady),  // MD
                 new UserFunctionListEntry(Commands.SimLightsFlash),  // MD
                 new UserFunctionListEntry(Commands.SimExtlWing),
                 new UserFunctionListEntry(Commands.SimWingLightBrt),  // MD
                 new UserFunctionListEntry(Commands.SimWingLightOff),  // MD
                 new UserFunctionListEntry(Commands.SimDMSUp),
                 new UserFunctionListEntry(Commands.SimDMSLeft),
                 new UserFunctionListEntry(Commands.SimDMSDown),
                 new UserFunctionListEntry(Commands.SimDMSRight),
                 new UserFunctionListEntry(Commands.SimAVTRSwitch),
                 new UserFunctionListEntry(Commands.SimAVTRSwitchOff),  // MD
                 new UserFunctionListEntry(Commands.SimAVTRSwitchAuto),  // MD
                 new UserFunctionListEntry(Commands.SimAVTRSwitchOn),  // MD
                 new UserFunctionListEntry(Commands.SimAutoAVTR),
                 new UserFunctionListEntry(Commands.SimIFFPower),
                 new UserFunctionListEntry(Commands.SimIFFIn),
                 new UserFunctionListEntry(Commands.SimINSInc),
                 new UserFunctionListEntry(Commands.SimINSDec),
                 new UserFunctionListEntry(Commands.SimINSOff),  // MD
                 new UserFunctionListEntry(Commands.SimINSNorm),  // MD
                 new UserFunctionListEntry(Commands.SimINSNav),  // MD
                 new UserFunctionListEntry(Commands.SimINSInFlt),  // MD
                 new UserFunctionListEntry(Commands.SimLEFLockSwitch),
                 new UserFunctionListEntry(Commands.SimLEFLock),  // MD
                 new UserFunctionListEntry(Commands.SimLEFAuto),  // MD
                 new UserFunctionListEntry(Commands.SimDigitalBUP),
                 new UserFunctionListEntry(Commands.SimAltFlaps),
                 new UserFunctionListEntry(Commands.SimAltFlapsNorm), // MD
                 new UserFunctionListEntry(Commands.SimAltFlapsExtend), // MD
                 new UserFunctionListEntry(Commands.SimManualFlyup),
                 new UserFunctionListEntry(Commands.SimFLCSReset),
                 new UserFunctionListEntry(Commands.SimFLTBIT),
                 new UserFunctionListEntry(Commands.SimOBOGSBit),
                 new UserFunctionListEntry(Commands.SimMalIndLights),
                 new UserFunctionListEntry(Commands.SimProbeHeat),
                 new UserFunctionListEntry(Commands.SimEPUGEN),
                 new UserFunctionListEntry(Commands.SimTestSwitch),
                 new UserFunctionListEntry(Commands.SimOverHeat),
                 new UserFunctionListEntry(Commands.SimTrimAPDisc),
                 new UserFunctionListEntry(Commands.SimTrimAPDISC),  // MD
                 new UserFunctionListEntry(Commands.SimTrimAPNORM),  // MD
                 new UserFunctionListEntry(Commands.SimMaxPower),
                 new UserFunctionListEntry(Commands.SimABReset),
                 new UserFunctionListEntry(Commands.SimTrimNoseUp),
                 new UserFunctionListEntry(Commands.SimTrimNoseDown),
                 new UserFunctionListEntry(Commands.SimTrimYawLeft),
                 new UserFunctionListEntry(Commands.SimTrimYawRight),
                 new UserFunctionListEntry(Commands.SimTrimRollLeft),
                 new UserFunctionListEntry(Commands.SimTrimRollRight),
                 new UserFunctionListEntry(Commands.SimStepMissileVolumeUp),
                 new UserFunctionListEntry(Commands.SimStepMissileVolumeDown),
                 new UserFunctionListEntry(Commands.SimStepThreatVolumeUp),
                 new UserFunctionListEntry(Commands.SimStepThreatVolumeDown),
                 new UserFunctionListEntry(Commands.SimTriggerFirstDetent),
                 new UserFunctionListEntry(Commands.SimTriggerSecondDetent),
                 new UserFunctionListEntry(Commands.SimRetUp),
                 new UserFunctionListEntry(Commands.SimRetDn),
                 new UserFunctionListEntry(Commands.SimCursorEnable),
                 new UserFunctionListEntry(Commands.SimStepComm1VolumeUp),
                 new UserFunctionListEntry(Commands.SimStepComm1VolumeDown),
                 new UserFunctionListEntry(Commands.SimStepComm2VolumeUp),
                 new UserFunctionListEntry(Commands.SimStepComm2VolumeDown),
    #if DEBUG
                 new UserFunctionListEntry(Commands.SimSwitchTextureOnOff),
    #endif
                 new UserFunctionListEntry(Commands.SimSymWheelUp),
                 new UserFunctionListEntry(Commands.SimSymWheelDn),
                 new UserFunctionListEntry(Commands.SimRandomError), // MD -- looks like adding this func here was missed.
                 new UserFunctionListEntry(Commands.SimToggleCockpit),
                 new UserFunctionListEntry(Commands.SimToggleGhostMFDs),
                 new UserFunctionListEntry(Commands.SimRangeKnobUp),
                 new UserFunctionListEntry(Commands.SimRangeKnobDown),
                //MI

                // M.N.
                 new UserFunctionListEntry(Commands.AWACSRequestCarrier),
                 new UserFunctionListEntry(Commands.WingmanDropStores),
                 new UserFunctionListEntry(Commands.ElementDropStores),
                 new UserFunctionListEntry(Commands.FlightDropStores),
                 new UserFunctionListEntry(Commands.WingmanVic),
                 new UserFunctionListEntry(Commands.ElementVic),
                 new UserFunctionListEntry(Commands.FlightVic),
                 new UserFunctionListEntry(Commands.WingmanFinger4),
                 new UserFunctionListEntry(Commands.ElementFinger4),
                 new UserFunctionListEntry(Commands.FlightFinger4),
                 new UserFunctionListEntry(Commands.WingmanEchelon),
                 new UserFunctionListEntry(Commands.ElementEchelon),
                 new UserFunctionListEntry(Commands.FlightEchelon),
                 new UserFunctionListEntry(Commands.WingmanForm1),
                 new UserFunctionListEntry(Commands.ElementForm1),
                 new UserFunctionListEntry(Commands.FlightForm1),
                 new UserFunctionListEntry(Commands.WingmanForm2),
                 new UserFunctionListEntry(Commands.ElementForm2),
                 new UserFunctionListEntry(Commands.FlightForm2),
                 new UserFunctionListEntry(Commands.WingmanForm3),
                 new UserFunctionListEntry(Commands.ElementForm3),
                 new UserFunctionListEntry(Commands.FlightForm3),
                 new UserFunctionListEntry(Commands.WingmanForm4),
                 new UserFunctionListEntry(Commands.ElementForm4),
                 new UserFunctionListEntry(Commands.FlightForm4),


                // JPO Avionics Power Panel
                 new UserFunctionListEntry(Commands.SimSMSPower),
                 new UserFunctionListEntry(Commands.SimSMSOn),  // MD
                 new UserFunctionListEntry(Commands.SimSMSOff),  // MD
                 new UserFunctionListEntry(Commands.SimFCCPower),
                 new UserFunctionListEntry(Commands.SimFCCOn),  // MD
                 new UserFunctionListEntry(Commands.SimFCCOff),  // MD
                 new UserFunctionListEntry(Commands.SimMFDPower),
                 new UserFunctionListEntry(Commands.SimMFDOn),  // MD
                 new UserFunctionListEntry(Commands.SimMFDOff),  // MD
                 new UserFunctionListEntry(Commands.SimUFCPower),
                 new UserFunctionListEntry(Commands.SimUFCOn),  // MD
                 new UserFunctionListEntry(Commands.SimUFCOff),  // MD
                 new UserFunctionListEntry(Commands.SimGPSPower),
                 new UserFunctionListEntry(Commands.SimGPSOn),  // MD
                 new UserFunctionListEntry(Commands.SimGPSOff),  // MD
                 new UserFunctionListEntry(Commands.SimDLPower),
                 new UserFunctionListEntry(Commands.SimDLOn),  // MD
                 new UserFunctionListEntry(Commands.SimDLOff),  // MD
                 new UserFunctionListEntry(Commands.SimMAPPower),
                 new UserFunctionListEntry(Commands.SimMAPOn),  // MD
                 new UserFunctionListEntry(Commands.SimMAPOff),  // MD
                 new UserFunctionListEntry(Commands.SimLeftHptPower),
                 new UserFunctionListEntry(Commands.SimLeftHptOn),  // MD
                 new UserFunctionListEntry(Commands.SimLeftHptOff),  // MD
                 new UserFunctionListEntry(Commands.SimRightHptPower),
                 new UserFunctionListEntry(Commands.SimRightHptOn),  // MD
                 new UserFunctionListEntry(Commands.SimRightHptOff),  // MD
                 new UserFunctionListEntry(Commands.SimTISLPower),
                 new UserFunctionListEntry(Commands.SimFCRPower),
                 new UserFunctionListEntry(Commands.SimFCROn),  // MD
                 new UserFunctionListEntry(Commands.SimFCROff),  // MD
                 new UserFunctionListEntry(Commands.SimHUDPower),
                 new UserFunctionListEntry(Commands.SimHUDOn),  // MD
                 new UserFunctionListEntry(Commands.SimHUDOff),  // MD
                 new UserFunctionListEntry(Commands.SimToggleRealisticAvionics),
                 new UserFunctionListEntry(Commands.SimIncFuelSwitch),
                 new UserFunctionListEntry(Commands.SimDecFuelSwitch),
                 new UserFunctionListEntry(Commands.SimFuelSwitchTest),  // MD
                 new UserFunctionListEntry(Commands.SimFuelSwitchNorm),  // MD
                 new UserFunctionListEntry(Commands.SimFuelSwitchResv),  // MD
                 new UserFunctionListEntry(Commands.SimFuelSwitchWingInt),  // MD
                 new UserFunctionListEntry(Commands.SimFuelSwitchWingExt),  // MD
                 new UserFunctionListEntry(Commands.SimFuelSwitchCenterExt),  // MD
                 new UserFunctionListEntry(Commands.SimIncFuelPump),
                 new UserFunctionListEntry(Commands.SimDecFuelPump),
                 new UserFunctionListEntry(Commands.SimFuelPumpOff),  // MD
                 new UserFunctionListEntry(Commands.SimFuelPumpNorm), // MD
                 new UserFunctionListEntry(Commands.SimFuelPumpAft),  // MD
                 new UserFunctionListEntry(Commands.SimFuelPumpFwd),  // MD
                 new UserFunctionListEntry(Commands.SimToggleMasterFuel),
                 new UserFunctionListEntry(Commands.SimMasterFuelOn),  // MD
                 new UserFunctionListEntry(Commands.SimMasterFuelOff),  // MD
                 new UserFunctionListEntry(Commands.SimExtFuelTrans),
                 new UserFunctionListEntry(Commands.SimFuelTransNorm),  // MD
                 new UserFunctionListEntry(Commands.SimFuelTransWing),  // MD
                 new UserFunctionListEntry(Commands.SimIncAirSource),
                 new UserFunctionListEntry(Commands.SimDecAirSource),
                 new UserFunctionListEntry(Commands.SimAirSourceOff),
                 new UserFunctionListEntry(Commands.SimAirSourceNorm),  // MD
                 new UserFunctionListEntry(Commands.SimAirSourceDump),  // MD
                 new UserFunctionListEntry(Commands.SimAirSourceRam),  // MD
                 new UserFunctionListEntry(Commands.SimDecLeftAuxComDigit),
                 new UserFunctionListEntry(Commands.SimDecCenterAuxComDigit),
                 new UserFunctionListEntry(Commands.SimDecRightAuxComDigit),
                 new UserFunctionListEntry(Commands.SimInteriorLight),
                 new UserFunctionListEntry(Commands.SimInstrumentLight),
                 new UserFunctionListEntry(Commands.SimSpotLight),
                 new UserFunctionListEntry(Commands.SimRwrPower),

                // Retro 16/10/03
                 new UserFunctionListEntry(Commands.Profiler_CursorDown),
                 new UserFunctionListEntry(Commands.Profiler_CursorUp),
                 new UserFunctionListEntry(Commands.Profiler_Parent),
                 new UserFunctionListEntry(Commands.Profiler_Select),
                 new UserFunctionListEntry(Commands.Profiler_Hier),
                 new UserFunctionListEntry(Commands.Profiler_Self),
                 new UserFunctionListEntry(Commands.ToggleProfilerDisplay),
                 new UserFunctionListEntry(Commands.ToggleProfiler),
                //new UserFunctionListEntry(Commands.Profiler_HistoryBack), // Retro 22May2004
                //new UserFunctionListEntry(Commands.Profiler_HistoryFwd), // Retro 22May2004
                //new UserFunctionListEntry(Commands.Profiler_HistoryBackFast), // Retro 22May2004
                //new UserFunctionListEntry(Commands.Profiler_HistoryFwdFast), // Retro 22May2004
                // end Retro

                // Retro 21Dec2003
                 new UserFunctionListEntry(Commands.ToggleSubTitles),
                 new UserFunctionListEntry(Commands.ToggleInfoBar),
                // end Retro

                // Retro 24Dec2003
                 new UserFunctionListEntry(Commands.ToggleDisplacementCam), // a dumb name for a dumb function implemented by a dumbass...
                // end Retro

                // Retro 4Jan2004
                 new UserFunctionListEntry(Commands.WinAmpNextTrack),
                 new UserFunctionListEntry(Commands.WinAmpPreviousTrack),
                 new UserFunctionListEntry(Commands.WinAmpStopPlayback),
                 new UserFunctionListEntry(Commands.WinAmpStartPlayback),
                 new UserFunctionListEntry(Commands.WinAmpTogglePlayback),
                 new UserFunctionListEntry(Commands.WinAmpVolumeUp),
                 new UserFunctionListEntry(Commands.WinAmpVolumeDown),
                // Retro 4Jan2004 end

                // Retro 12Jan2004
                 new UserFunctionListEntry(Commands.CycleEngine),
                 new UserFunctionListEntry(Commands.selectLeftEngine),
                 new UserFunctionListEntry(Commands.selectRightEngine),
                 new UserFunctionListEntry(Commands.selectBothEngines),
            // Retro 12Jan2004 end

                 new UserFunctionListEntry(Commands.ToggleTIR),
                 new UserFunctionListEntry(Commands.ToggleClickablePitMode), //Wombat778 1-22-2004
                 new UserFunctionListEntry(Commands.SimToggleRearView), //Wombat778 4-13-2004
                 new UserFunctionListEntry(Commands.SimToggleAltView), //Wombat778 4-13-2004


    #if  _DO_VTUNE_
                 new UserFunctionListEntry(Commands.ToggleVtune),
    #endif
                // 2000-05-17 ADDED BY S.G. TO HANDLE THE 'AuxCom' switches
                 new UserFunctionListEntry(Commands.SimCycleLeftAuxComDigit),
                 new UserFunctionListEntry(Commands.SimCycleCenterAuxComDigit),
                 new UserFunctionListEntry(Commands.SimCycleRightAuxComDigit),
                 new UserFunctionListEntry(Commands.SimCycleBandAuxComDigit),
                 new UserFunctionListEntry(Commands.SimToggleAuxComMaster),
                 new UserFunctionListEntry(Commands.SimAuxComBackup),  // MD
                 new UserFunctionListEntry(Commands.SimAuxComUFC),  // MD
                 new UserFunctionListEntry(Commands.SimToggleAuxComAATR),
                 new UserFunctionListEntry(Commands.SimTACANTR),  // MD
                 new UserFunctionListEntry(Commands.SimTACANAATR),  // MD
                 new UserFunctionListEntry(Commands.SimToggleUHFMaster),
                //me123
                 new UserFunctionListEntry(Commands.SimTransmitCom1),
                 new UserFunctionListEntry(Commands.SimTransmitCom2),
                // END OF ADDED SECTION
                // 2000-11-10 ADDED BY S.G. TO HANDLE THE 'driftCO' switch
                 new UserFunctionListEntry(Commands.SimDriftCO),
                 new UserFunctionListEntry(Commands.SimDriftCOOn),  // MD
                 new UserFunctionListEntry(Commands.SimDriftCOOff),  // MD
                // END OF ADDED SECTION
                // 2000-11-17 ADDED BY S.G. TO HANDLE THE 'Cat I/III switch'
                 new UserFunctionListEntry(Commands.SimCATSwitch),
                 new UserFunctionListEntry(Commands.SimCATI),  // MD
                 new UserFunctionListEntry(Commands.SimCATIII),  // MD
                 new UserFunctionListEntry(Commands.SimRegen), // 2002-03-22 S.G. Force a regen in dogfight

                //sfr: added night light keystroke
                 new UserFunctionListEntry(Commands.SimInteriorLight),
                 new UserFunctionListEntry(Commands.SimReverseThrusterOn),
                 new UserFunctionListEntry(Commands.SimReverseThrusterOff),
                 new UserFunctionListEntry(Commands.SimReverseThrusterToggle)
         };

        public static InputFunctionType FindFunctionFromString(string str)
        {
            foreach (var entry in UserFunctionList)
            {
                if (entry.funcName == str)
                {
                    return entry.theFunc;
                }
            }

            return null;
        }

        public static string FindStringFromFunction(InputFunctionType func)
        {
            foreach (var entry in UserFunctionList)
            {
                if (func == entry.theFunc)
                {
                    return entry.funcName;
                }
            }

            return null;
        }
    }
}
