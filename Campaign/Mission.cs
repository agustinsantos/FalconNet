using System;
using FalconNet.Common;
using FalconNet.FalcLib;
using FalconNet.VU;
using Flight = FalconNet.Campaign.FlightClass;
using Team = System.SByte;
using GridIndex = System.Int16;
using FalconNet.CampaignBase;
using F4PFList = FalconNet.FalcLib.FalconPrivateList;
using F4POList = FalconNet.FalcLib.FalconPrivateOrderedList;

namespace FalconNet.Campaign
{
    // ===================================================
    // Mission defines
    // ===================================================


    // Returned by TargetThreats
    // Flags for what we really need
    [Flags]
    public enum TargetThreatReturnFlagEnum
    {
        NEED_SEAD = 0x01,
        NEED_ECM = 0x02,

        // Specifics as to the threat types
        THREAT_LALT_SAM = 0x10,
        THREAT_HALT_SAM = 0x20,
        THREAT_AAA = 0x40,

        // Specifics as to the threat location
        THREAT_ENROUTE = 0x100,
        THREAT_TARGET = 0x200,
    }

    // Mission profiles
    [Flags]
    public enum MissionProfileEnum : byte
    {
        MPROF_LOW = 0x01,				// Low engress profile
        MPROF_HIGH = 0x02,				// High engress profile
    }

    // Target area profiles
    public enum TargetAreaProfileEnum : byte
    {
        TPROF_NONE = 0,					// No target WP
        TPROF_ATTACK = 1,					// IP, Target, Turn point  (Assembly, Break point)
        TPROF_HPATTACK = 2,					// IP, Target (Assembly, Break point)
        TPROF_LOITER = 3,					// 2 Turn points (Assembly)
        TPROF_TARGET = 4,					// Target only (Assembly, Break point)
        TPROF_AVOID = 5,					// Break point, Turn point (Assembly)
        TPROF_SWEEP = 6,					// 3 circular Turn points
        TPROF_FLYBY = 7,					// Pass over target, at lowest possible threat
        TPROF_SEARCH = 8,					// Like loiter, but at full speed
        TPROF_LAND = 9,					// Land at target for station time, then takeoff again
    }

    // Target desctiption types (when to use target actions)
    public enum TargetDescriptionType : byte
    {
        TDESC_NONE = 0,
        TDESC_TTL = 1,					// Takeoff To Landing (mission actions during whole route)
        TDESC_ATA = 2,					// Assembly To Assembly
        TDESC_TAO = 3,					// Target Area Only
    }

    // Special time on target types
    public enum SpecialTOTTypeEnum
    {
        TOT_TAKEOFF = 10,
        TOT_ENROUTE = 11,
        TOT_INGRESS = 12,
    }



    // ===================================================

    // TODO typedef MissionRequestClass MissionRequest;

    // ======================================================
    // Mission Data Type - Stores data used to build missions
    // ======================================================

    public class MissionDataType
    {
        public MissionTypeEnum type;
        public MissionTargetTypeEnum target;						// Target type
        public MissionRollEnum skill;						// Primary skill to look for
        public MissionProfileEnum mission_profile;			// Allowable mission profiles
        public TargetAreaProfileEnum target_profile;				// Type of target profile
        public TargetDescriptionType target_desc;				// When to use our target action (target area/whole mission/etc)
        public WPAction routewp;					// Waypoint type along route
        public WPAction targetwp;					// What to do at target location
        public short minalt;						// Minimum alt at target (in hundreds of feet)
        public short maxalt;						// Maximum alt at target (in hundreds of feet)
        public short missionalt;					// Suggested mission altitude (in hundreds of feet)
        public short separation;					// Seperation from main flight, in seconds
        public short loitertime;					// Loiter time in minutes
        public byte str;						// Aircraft strength typically assigned
        public byte min_time;					// Minimum # of minutes in advance to consider a planned mission
        public byte max_time;					// Maximum # of minutes in advance to consider a planned mission
        public MissionTypeEnum escorttype;					// What sort of mission to build for an escort
        public byte mindistance;				// Minimum distance between two similar missions
        public byte mintime;					// Minimum time between missions of similar types
        public byte caps;						// Special capibilities required (stealth, navy, etc)
        public MissionDataFlagEnum flags;						// flags

        public MissionDataType() { }
        public MissionDataType(MissionTypeEnum type_, MissionTargetTypeEnum target_, MissionRollEnum skill_, MissionProfileEnum mission_profile_,
                               TargetAreaProfileEnum target_profile_,
                              TargetDescriptionType target_desc_, WPAction routewp_, WPAction targetwp_, short minalt_, short maxalt_,
                              short missionalt_, short separation_, short loitertime_, byte str_, byte min_time_,
                              byte max_time_, MissionTypeEnum escorttype_, byte mindistance_, byte mintime_, byte caps_, MissionDataFlagEnum flags_)
        {

            this.type = type_;
            this.target = target_;
            this.skill = skill_;
            this.mission_profile = mission_profile_;
            this.target_profile = target_profile_;
            this.target_desc = target_desc_;
            this.routewp = routewp_;
            this.targetwp = targetwp_;
            this.minalt = minalt_;
            this.maxalt = maxalt_;
            this.missionalt = missionalt_;
            this.separation = separation_;
            this.loitertime = loitertime_;
            this.str = str_;
            this.min_time = min_time_;
            this.max_time = max_time_;
            this.escorttype = escorttype_;
            this.mindistance = mindistance_;
            this.mintime = mintime_;
            this.caps = caps_;
            this.flags = flags_;
        }
    };

    public static class MissionStatic
    {
        public const MissionProfileEnum MPROF_STANDARD = (MissionProfileEnum.MPROF_LOW | MissionProfileEnum.MPROF_HIGH);
        public static MissionDataType[] MissionData = new MissionDataType[(int)MissionTypeEnum.AMIS_OTHER]
        {
    new MissionDataType( MissionTypeEnum.AMIS_NONE, MissionTargetTypeEnum.AMIS_TAR_NONE, MissionRollEnum.ARO_NOTHING, MPROF_STANDARD, TargetAreaProfileEnum.TPROF_ATTACK, TargetDescriptionType.TDESC_NONE, WPAction.WP_NOTHING, WPAction.WP_LAST, 0,   0,   0,  0,  0, 0,  0,  0, MissionTypeEnum.AMIS_NONE, 0,  0, 0, MissionDataFlagEnum.NOTHING ),
    new MissionDataType( MissionTypeEnum.AMIS_BARCAP, MissionTargetTypeEnum.AMIS_TAR_LOCATION, MissionRollEnum.ARO_CA, MissionProfileEnum.MPROF_HIGH, TargetAreaProfileEnum.TPROF_SEARCH, TargetDescriptionType.TDESC_TTL, WPAction.WP_NOTHING, WPAction.WP_CAP, 100, 400, 200,  0, 15, 2,  5, 30, MissionTypeEnum.AMIS_NONE,    15, 15, 0, MissionDataFlagEnum.AMIS_ADDAWACS | MissionDataFlagEnum.AMIS_NOTHREAT | MissionDataFlagEnum.AMIS_ADDTANKER | MissionDataFlagEnum.AMIS_DONT_COORD | MissionDataFlagEnum.AMIS_EXPECT_DIVERT | MissionDataFlagEnum.AMIS_NO_BREAKPT | MissionDataFlagEnum.AMIS_FLYALWAYS ),
    new MissionDataType( MissionTypeEnum.AMIS_BARCAP2, MissionTargetTypeEnum.AMIS_TAR_LOCATION, MissionRollEnum.ARO_CA, MissionProfileEnum.MPROF_HIGH, TargetAreaProfileEnum.TPROF_SEARCH, TargetDescriptionType.TDESC_TTL, WPAction.WP_NOTHING, WPAction.WP_CAP, 100, 400, 200,  0, 15, 2,  5, 30, MissionTypeEnum.AMIS_NONE,    15, 15, 0, MissionDataFlagEnum.AMIS_ADDAWACS | MissionDataFlagEnum.AMIS_NOTHREAT | MissionDataFlagEnum.AMIS_ADDTANKER | MissionDataFlagEnum.AMIS_DONT_COORD | MissionDataFlagEnum.AMIS_EXPECT_DIVERT | MissionDataFlagEnum.AMIS_NO_BREAKPT | MissionDataFlagEnum.AMIS_FLYALWAYS ),
    new MissionDataType( MissionTypeEnum.AMIS_HAVCAP, MissionTargetTypeEnum.AMIS_TAR_UNIT, MissionRollEnum.ARO_CA, MissionProfileEnum.MPROF_HIGH, TargetAreaProfileEnum.TPROF_SEARCH, TargetDescriptionType.TDESC_ATA, WPAction.WP_ESCORT, WPAction.WP_CAP, 100, 400, 300, -60, 60, 2,  5, 30, MissionTypeEnum.AMIS_NONE, 1, 15, 0, MissionDataFlagEnum.AMIS_ADDAWACS | MissionDataFlagEnum.AMIS_NOTHREAT | MissionDataFlagEnum.AMIS_ADDTANKER | MissionDataFlagEnum.AMIS_NO_BREAKPT | MissionDataFlagEnum.AMIS_FLYALWAYS ), //AMIS_EXPECT_DIVERT | removed, don't divert HAVCAPs
    new MissionDataType( MissionTypeEnum.AMIS_TARCAP, MissionTargetTypeEnum.AMIS_TAR_LOCATION, MissionRollEnum.ARO_CA, MissionProfileEnum.MPROF_HIGH, TargetAreaProfileEnum.TPROF_SEARCH, TargetDescriptionType.TDESC_TAO, WPAction.WP_NOTHING, WPAction.WP_CAP, 100, 300, 200, -60, 15, 2,  5, 30, MissionTypeEnum.AMIS_NONE,    30, 15, 0, MissionDataFlagEnum.AMIS_ADDAWACS | MissionDataFlagEnum.AMIS_HIGHTHREAT | MissionDataFlagEnum.AMIS_EXPECT_DIVERT | MissionDataFlagEnum.AMIS_NO_BREAKPT | MissionDataFlagEnum.AMIS_FLYALWAYS ),
    new MissionDataType( MissionTypeEnum.AMIS_RESCAP, MissionTargetTypeEnum.AMIS_TAR_LOCATION, MissionRollEnum.ARO_CA, MissionProfileEnum.MPROF_HIGH, TargetAreaProfileEnum.TPROF_SEARCH, TargetDescriptionType.TDESC_TAO, WPAction.WP_ESCORT, WPAction.WP_CAP, 50, 100, 100,  0, 15, 2,  5, 30, MissionTypeEnum.AMIS_NONE,    30, 15, 0, MissionDataFlagEnum.AMIS_ADDAWACS | MissionDataFlagEnum.AMIS_EXPECT_DIVERT | MissionDataFlagEnum.AMIS_NO_BREAKPT | MissionDataFlagEnum.AMIS_FLYALWAYS ),
    new MissionDataType( MissionTypeEnum.AMIS_AMBUSHCAP, MissionTargetTypeEnum.AMIS_TAR_LOCATION, MissionRollEnum.ARO_CA, MissionProfileEnum.MPROF_LOW, TargetAreaProfileEnum.TPROF_SEARCH, TargetDescriptionType.TDESC_TTL, WPAction.WP_NOTHING, WPAction.WP_CAP, 2,  10,   7,  0, 15, 2,  5, 30, MissionTypeEnum.AMIS_NONE,    30, 15, 0, MissionDataFlagEnum.AMIS_ADDAWACS | MissionDataFlagEnum.AMIS_NOTHREAT | MissionDataFlagEnum.AMIS_DONT_COORD | MissionDataFlagEnum.AMIS_EXPECT_DIVERT | MissionDataFlagEnum.AMIS_NO_BREAKPT | MissionDataFlagEnum.AMIS_FLYALWAYS ),
    new MissionDataType( MissionTypeEnum.AMIS_SWEEP, MissionTargetTypeEnum.AMIS_TAR_LOCATION, MissionRollEnum.ARO_CA, MissionProfileEnum.MPROF_HIGH, TargetAreaProfileEnum.TPROF_SWEEP, TargetDescriptionType.TDESC_TTL, WPAction.WP_CA, WPAction.WP_CA, 100, 400, 200,  0,  0, 4,  5, 30, MissionTypeEnum.AMIS_NONE, 1, 30, 0, MissionDataFlagEnum.AMIS_ADDAWACS | MissionDataFlagEnum.AMIS_EXPECT_DIVERT | MissionDataFlagEnum.AMIS_NO_BREAKPT ),
    new MissionDataType( MissionTypeEnum.AMIS_ALERT, MissionTargetTypeEnum.AMIS_TAR_LOCATION, MissionRollEnum.ARO_CA, MissionProfileEnum.MPROF_LOW, TargetAreaProfileEnum.TPROF_NONE, TargetDescriptionType.TDESC_TAO, WPAction.WP_NOTHING, WPAction.WP_NOTHING, 0,  10,   0,  0,  0, 2,  0, 10, MissionTypeEnum.AMIS_NONE, 1,  0, 0, MissionDataFlagEnum.AMIS_NOTHREAT | MissionDataFlagEnum.AMIS_TARGET_ONLY | MissionDataFlagEnum.AMIS_EXPECT_DIVERT | MissionDataFlagEnum.AMIS_NO_BREAKPT | MissionDataFlagEnum.AMIS_FLYALWAYS ),
    new MissionDataType( MissionTypeEnum.AMIS_INTERCEPT, MissionTargetTypeEnum.AMIS_TAR_UNIT, MissionRollEnum.ARO_CA, MissionProfileEnum.MPROF_HIGH, TargetAreaProfileEnum.TPROF_TARGET, TargetDescriptionType.TDESC_TAO, WPAction.WP_NOTHING, WPAction.WP_INTERCEPT, 100, 400, 200,  0,  0, 2,  0, 20, MissionTypeEnum.AMIS_NONE, 0,  0, 0, MissionDataFlagEnum.AMIS_IMMEDIATE | MissionDataFlagEnum.AMIS_ASSIGNED_TAR | MissionDataFlagEnum.AMIS_EXPECT_DIVERT | MissionDataFlagEnum.AMIS_FLYALWAYS ),
    new MissionDataType( MissionTypeEnum.AMIS_ESCORT, MissionTargetTypeEnum.AMIS_TAR_UNIT, MissionRollEnum.ARO_CA, MPROF_STANDARD, TargetAreaProfileEnum.TPROF_FLYBY, TargetDescriptionType.TDESC_ATA, WPAction.WP_ESCORT, WPAction.WP_ESCORT, 50, 600, 200, -60,  0, 2, 10, 20, MissionTypeEnum.AMIS_NONE, 1,  0, 0, MissionDataFlagEnum.AMIS_ADDAWACS | MissionDataFlagEnum.AMIS_HIGHTHREAT | MissionDataFlagEnum.AMIS_MATCHSPEED | MissionDataFlagEnum.AMIS_NO_DIST_BONUS | MissionDataFlagEnum.AMIS_FLYALWAYS ),
    // 2001-06-30 MODIFIED BY S.G. SO SEAD STRIKES BEHAVES LIKE SEAD ESCORTS FOR EN ROUTE SAM THREAT
    // { AMIS_SEADSTRIKE, MissionTargetTypeEnum.AMIS_TAR_UNIT, MissionRollEnum.ARO_SEAD, MPROF_STANDARD, TargetAreaProfileEnum.TPROF_ATTACK, TargetDescriptionType.TDESC_TAO, WPAction.WP_NOTHING, WP_SEAD, 20, 120,  40,  0,  0, 4, 10, 40, AMIS_ESCORT, 1,255, 0, AMIS_ADDECM | AMIS_ADDBDA | AMIS_AVOIDTHREAT | AMIS_ADDBARCAP | AMIS_MATCHSPEED | AMIS_HIGHTHREAT | AMIS_NO_TARGETABORT ),
    // Fixed by M.N. forgot to add the RP5 SEADSTRIKE line...
    new MissionDataType( MissionTypeEnum.AMIS_SEADSTRIKE, MissionTargetTypeEnum.AMIS_TAR_UNIT, MissionRollEnum.ARO_SEAD, MPROF_STANDARD, TargetAreaProfileEnum.TPROF_ATTACK, TargetDescriptionType.TDESC_ATA, WPAction.WP_SEAD, WPAction.WP_SEAD, 20, 120,  40,  0,  0, 4, 10, 40, MissionTypeEnum.AMIS_ESCORT, 1, 255, 0, MissionDataFlagEnum.AMIS_ADDECM | MissionDataFlagEnum.AMIS_ADDBDA | MissionDataFlagEnum.AMIS_AVOIDTHREAT | MissionDataFlagEnum.AMIS_ADDBARCAP | MissionDataFlagEnum.AMIS_MATCHSPEED | MissionDataFlagEnum.AMIS_HIGHTHREAT | MissionDataFlagEnum.AMIS_NO_TARGETABORT ),
    new MissionDataType( MissionTypeEnum.AMIS_SEADESCORT, MissionTargetTypeEnum.AMIS_TAR_UNIT, MissionRollEnum.ARO_SEAD, MPROF_STANDARD, TargetAreaProfileEnum.TPROF_FLYBY, TargetDescriptionType.TDESC_ATA, WPAction.WP_SEAD, WPAction.WP_SEAD, 20, 120,  40, -60,  0, 2, 10, 40, MissionTypeEnum.AMIS_NONE, 1,  0, 0, MissionDataFlagEnum.AMIS_ADDECM | MissionDataFlagEnum.AMIS_HIGHTHREAT | MissionDataFlagEnum.AMIS_MATCHSPEED | MissionDataFlagEnum.AMIS_NO_TARGETABORT | MissionDataFlagEnum.AMIS_NO_DIST_BONUS | MissionDataFlagEnum.AMIS_FLYALWAYS ),
   new MissionDataType( MissionTypeEnum.AMIS_OCASTRIKE, MissionTargetTypeEnum.AMIS_TAR_OBJECTIVE, MissionRollEnum.ARO_S, MPROF_STANDARD, TargetAreaProfileEnum.TPROF_ATTACK, TargetDescriptionType.TDESC_TAO, WPAction.WP_NOTHING, WPAction.WP_STRIKE, 5, 120,  80,  0,  0, 4, 10, 40, MissionTypeEnum.AMIS_ESCORT, 1, 255, 0, MissionDataFlagEnum.AMIS_ADDBDA | MissionDataFlagEnum.AMIS_AVOIDTHREAT | MissionDataFlagEnum.AMIS_ADDSEAD | MissionDataFlagEnum.AMIS_ADDESCORT | MissionDataFlagEnum.AMIS_ADDBARCAP | MissionDataFlagEnum.AMIS_ADDOCASTRIKE | MissionDataFlagEnum.AMIS_MATCHSPEED | MissionDataFlagEnum.AMIS_NO_TARGETABORT ),
    new MissionDataType( MissionTypeEnum.AMIS_INTSTRIKE, MissionTargetTypeEnum.AMIS_TAR_OBJECTIVE, MissionRollEnum.ARO_S, MPROF_STANDARD, TargetAreaProfileEnum.TPROF_ATTACK, TargetDescriptionType.TDESC_TAO, WPAction.WP_NOTHING, WPAction.WP_STRIKE, 5, 120,  50,  0,  0, 4, 10, 40, MissionTypeEnum.AMIS_ESCORT, 1, 255, 0, MissionDataFlagEnum.AMIS_ADDBDA | MissionDataFlagEnum.AMIS_AVOIDTHREAT | MissionDataFlagEnum.AMIS_ADDSEAD | MissionDataFlagEnum.AMIS_ADDBARCAP | MissionDataFlagEnum.AMIS_ADDOCASTRIKE | MissionDataFlagEnum.AMIS_MATCHSPEED | MissionDataFlagEnum.AMIS_NO_TARGETABORT ),
    new MissionDataType( MissionTypeEnum.AMIS_STRIKE, MissionTargetTypeEnum.AMIS_TAR_OBJECTIVE, MissionRollEnum.ARO_S, MPROF_STANDARD, TargetAreaProfileEnum.TPROF_ATTACK, TargetDescriptionType.TDESC_TAO, WPAction.WP_NOTHING, WPAction.WP_STRIKE, 5, 120,  80,  0,  0, 4, 10, 40, MissionTypeEnum.AMIS_ESCORT, 1, 255, 0, MissionDataFlagEnum.AMIS_ADDAWACS | MissionDataFlagEnum.AMIS_ADDBDA | MissionDataFlagEnum.AMIS_AVOIDTHREAT | MissionDataFlagEnum.AMIS_ADDSEAD | MissionDataFlagEnum.AMIS_ADDESCORT | MissionDataFlagEnum.AMIS_ADDBARCAP | MissionDataFlagEnum.AMIS_ADDOCASTRIKE | MissionDataFlagEnum.AMIS_MATCHSPEED | MissionDataFlagEnum.AMIS_NO_TARGETABORT ),
    new MissionDataType( MissionTypeEnum.AMIS_DEEPSTRIKE, MissionTargetTypeEnum.AMIS_TAR_OBJECTIVE, MissionRollEnum.ARO_S, MPROF_STANDARD, TargetAreaProfileEnum.TPROF_ATTACK, TargetDescriptionType.TDESC_TAO, WPAction.WP_NOTHING, WPAction.WP_STRIKE, 5, 120,  80,  0,  0, 4, 10, 40, MissionTypeEnum.AMIS_ESCORT, 1, 255, 0, MissionDataFlagEnum.AMIS_ADDAWACS | MissionDataFlagEnum.AMIS_ADDECM | MissionDataFlagEnum.AMIS_AVOIDTHREAT | MissionDataFlagEnum.AMIS_HIGHTHREAT | MissionDataFlagEnum.AMIS_ADDSEAD | MissionDataFlagEnum.AMIS_ADDBARCAP | MissionDataFlagEnum.AMIS_MATCHSPEED | MissionDataFlagEnum.AMIS_NO_TARGETABORT ),
    new MissionDataType( MissionTypeEnum.AMIS_STSTRIKE, MissionTargetTypeEnum.AMIS_TAR_OBJECTIVE, MissionRollEnum.ARO_S, MPROF_STANDARD, TargetAreaProfileEnum.TPROF_ATTACK, TargetDescriptionType.TDESC_TAO, WPAction.WP_NOTHING, WPAction.WP_STRIKE, 5, 120,  80,  0,  0, 4, 10, 40, MissionTypeEnum.AMIS_NONE, 1, 255, (byte)VEH_FLAGS.VEH_STEALTH, MissionDataFlagEnum.AMIS_AVOIDTHREAT | MissionDataFlagEnum.AMIS_HIGHTHREAT | MissionDataFlagEnum.AMIS_ADDBARCAP | MissionDataFlagEnum.AMIS_MATCHSPEED | MissionDataFlagEnum.AMIS_NO_TARGETABORT ),
   new MissionDataType( MissionTypeEnum.AMIS_STRATBOMB, MissionTargetTypeEnum.AMIS_TAR_OBJECTIVE, MissionRollEnum.ARO_SB, MissionProfileEnum.MPROF_HIGH, TargetAreaProfileEnum.TPROF_ATTACK, TargetDescriptionType.TDESC_TAO, WPAction.WP_NOTHING, WPAction.WP_BOMB, 200, 600, 300,  0,  0, 2, 10, 120, MissionTypeEnum.AMIS_ESCORT, 1, 255, 0, MissionDataFlagEnum.AMIS_ADDSEAD | MissionDataFlagEnum.AMIS_ADDESCORT | MissionDataFlagEnum.AMIS_ADDBARCAP | MissionDataFlagEnum.AMIS_ADDOCASTRIKE | MissionDataFlagEnum.AMIS_MATCHSPEED | MissionDataFlagEnum.AMIS_NO_TARGETABORT ),
    new MissionDataType( MissionTypeEnum.AMIS_FAC, MissionTargetTypeEnum.AMIS_TAR_LOCATION, MissionRollEnum.ARO_FAC, MissionProfileEnum.MPROF_LOW, TargetAreaProfileEnum.TPROF_LOITER, TargetDescriptionType.TDESC_TAO, WPAction.WP_NOTHING, WPAction.WP_FAC, 5, 100,  50,  0, 30, 1,  5, 60, MissionTypeEnum.AMIS_SWEEP,    10, 20, 0, MissionDataFlagEnum.AMIS_ADDJSTAR | MissionDataFlagEnum.AMIS_AVOIDTHREAT | MissionDataFlagEnum.AMIS_ADDOCASTRIKE | MissionDataFlagEnum.AMIS_NOTHREAT | MissionDataFlagEnum.AMIS_ADDTANKER | MissionDataFlagEnum.AMIS_DONT_COORD | MissionDataFlagEnum.AMIS_NPC_ONLY | MissionDataFlagEnum.AMIS_FLYALWAYS ),
    new MissionDataType( MissionTypeEnum.AMIS_ONCALLCAS, MissionTargetTypeEnum.AMIS_TAR_LOCATION, MissionRollEnum.ARO_GA, MPROF_STANDARD, TargetAreaProfileEnum.TPROF_LOITER, TargetDescriptionType.TDESC_TAO, WPAction.WP_NOTHING, WPAction.WP_CASCP, 2, 100,  50,  0, 15, 2,  5, 60, MissionTypeEnum.AMIS_SWEEP,    10, 20, 0, MissionDataFlagEnum.AMIS_ADDJSTAR | MissionDataFlagEnum.AMIS_AVOIDTHREAT | MissionDataFlagEnum.AMIS_ADDOCASTRIKE | MissionDataFlagEnum.AMIS_ADDFAC | MissionDataFlagEnum.AMIS_ADDBARCAP | MissionDataFlagEnum.AMIS_NOTHREAT | MissionDataFlagEnum.AMIS_ADDTANKER | MissionDataFlagEnum.AMIS_DONT_COORD | MissionDataFlagEnum.AMIS_NO_BREAKPT | MissionDataFlagEnum.AMIS_EXPECT_DIVERT | MissionDataFlagEnum.AMIS_FLYALWAYS ),
    new MissionDataType( MissionTypeEnum.AMIS_PRPLANCAS, MissionTargetTypeEnum.AMIS_TAR_UNIT, MissionRollEnum.ARO_GA, MPROF_STANDARD, TargetAreaProfileEnum.TPROF_TARGET, TargetDescriptionType.TDESC_TAO, WPAction.WP_NOTHING, WPAction.WP_GNDSTRIKE, 2, 100,  50,  0,  0, 2,  5, 60, MissionTypeEnum.AMIS_SWEEP,    15, 20, 0, MissionDataFlagEnum.AMIS_ADDJSTAR | MissionDataFlagEnum.AMIS_AVOIDTHREAT | MissionDataFlagEnum.AMIS_ADDOCASTRIKE | MissionDataFlagEnum.AMIS_ADDBARCAP | MissionDataFlagEnum.AMIS_NOTHREAT | MissionDataFlagEnum.AMIS_NO_TARGETABORT | MissionDataFlagEnum.AMIS_FLYALWAYS ),
    new MissionDataType( MissionTypeEnum.AMIS_CAS, MissionTargetTypeEnum.AMIS_TAR_LOCATION, MissionRollEnum.ARO_GA, MPROF_STANDARD, TargetAreaProfileEnum.TPROF_TARGET, TargetDescriptionType.TDESC_TAO, WPAction.WP_NOTHING, WPAction.WP_GNDSTRIKE, 2, 100,  50,  0,  0, 2,  0, 20, MissionTypeEnum.AMIS_SWEEP,     0,  0, 0, MissionDataFlagEnum.AMIS_ADDJSTAR | MissionDataFlagEnum.AMIS_AVOIDTHREAT | MissionDataFlagEnum.AMIS_ADDOCASTRIKE | MissionDataFlagEnum.AMIS_IMMEDIATE | MissionDataFlagEnum.AMIS_DONT_COORD | MissionDataFlagEnum.AMIS_ASSIGNED_TAR | MissionDataFlagEnum.AMIS_EXPECT_DIVERT | MissionDataFlagEnum.AMIS_FLYALWAYS ),
    new MissionDataType( MissionTypeEnum.AMIS_SAD, MissionTargetTypeEnum.AMIS_TAR_LOCATION, MissionRollEnum.ARO_GA, MPROF_STANDARD, TargetAreaProfileEnum.TPROF_SEARCH, TargetDescriptionType.TDESC_ATA, WPAction.WP_SAD, WPAction.WP_SAD, 5, 200, 100,  0,  0, 2,  5, 60, MissionTypeEnum.AMIS_NONE,    20, 30, 0, MissionDataFlagEnum.AMIS_ADDAWACS | MissionDataFlagEnum.AMIS_ADDJSTAR | MissionDataFlagEnum.AMIS_ADDBARCAP | MissionDataFlagEnum.AMIS_ADDTANKER | MissionDataFlagEnum.AMIS_DONT_COORD | MissionDataFlagEnum.AMIS_NO_BREAKPT ),
    new MissionDataType( MissionTypeEnum.AMIS_INT, MissionTargetTypeEnum.AMIS_TAR_LOCATION, MissionRollEnum.ARO_GA, MPROF_STANDARD, TargetAreaProfileEnum.TPROF_SEARCH, TargetDescriptionType.TDESC_ATA, WPAction.WP_NOTHING, WPAction.WP_SAD, 5, 200, 100,  0,  5, 4,  5, 60, MissionTypeEnum.AMIS_NONE, 1, 30, 0, MissionDataFlagEnum.AMIS_ADDAWACS | MissionDataFlagEnum.AMIS_ADDJSTAR | MissionDataFlagEnum.AMIS_ADDBARCAP | MissionDataFlagEnum.AMIS_DONT_COORD | MissionDataFlagEnum.AMIS_NO_BREAKPT ),
    new MissionDataType( MissionTypeEnum.AMIS_BAI, MissionTargetTypeEnum.AMIS_TAR_LOCATION, MissionRollEnum.ARO_GA, MPROF_STANDARD, TargetAreaProfileEnum.TPROF_SEARCH, TargetDescriptionType.TDESC_ATA, WPAction.WP_NOTHING, WPAction.WP_SAD, 5, 200, 100,  0, 15, 4,  0, 60, MissionTypeEnum.AMIS_NONE, 1, 30, 0, MissionDataFlagEnum.AMIS_AVOIDTHREAT | MissionDataFlagEnum.AMIS_DONT_COORD | MissionDataFlagEnum.AMIS_NO_BREAKPT ),
    new MissionDataType( MissionTypeEnum.AMIS_AWACS, MissionTargetTypeEnum.AMIS_TAR_LOCATION, MissionRollEnum.ARO_AWACS, MissionProfileEnum.MPROF_HIGH, TargetAreaProfileEnum.TPROF_LOITER, TargetDescriptionType.TDESC_TAO, WPAction.WP_ELINT, WPAction.WP_ELINT, 300, 400, 400,  0, 300, 1, 20, 120, MissionTypeEnum.AMIS_HAVCAP,   50, 120, 0, MissionDataFlagEnum.AMIS_ADDESCORT | MissionDataFlagEnum.AMIS_ADDSWEEP | MissionDataFlagEnum.AMIS_NOTHREAT | MissionDataFlagEnum.AMIS_AIR_LAUNCH_OK | MissionDataFlagEnum.AMIS_NO_DIST_BONUS | MissionDataFlagEnum.AMIS_FLYALWAYS ),
    new MissionDataType( MissionTypeEnum.AMIS_JSTAR, MissionTargetTypeEnum.AMIS_TAR_LOCATION, MissionRollEnum.ARO_JSTAR, MissionProfileEnum.MPROF_HIGH, TargetAreaProfileEnum.TPROF_LOITER, TargetDescriptionType.TDESC_TAO, WPAction.WP_ELINT, WPAction.WP_ELINT, 300, 400, 400,  0, 300, 1, 20, 120, MissionTypeEnum.AMIS_HAVCAP,   50, 120, 0, MissionDataFlagEnum.AMIS_ADDESCORT | MissionDataFlagEnum.AMIS_ADDSWEEP | MissionDataFlagEnum.AMIS_NOTHREAT | MissionDataFlagEnum.AMIS_AIR_LAUNCH_OK | MissionDataFlagEnum.AMIS_NO_DIST_BONUS | MissionDataFlagEnum.AMIS_FLYALWAYS ),
    new MissionDataType( MissionTypeEnum.AMIS_TANKER, MissionTargetTypeEnum.AMIS_TAR_LOCATION, MissionRollEnum.ARO_TANK, MissionProfileEnum.MPROF_HIGH, TargetAreaProfileEnum.TPROF_LOITER, TargetDescriptionType.TDESC_TAO, WPAction.WP_TANKER, WPAction.WP_TANKER, 100, 300, 200,  0, 300, 1, 20, 120, MissionTypeEnum.AMIS_HAVCAP,   40, 120, 0, MissionDataFlagEnum.AMIS_ADDESCORT | MissionDataFlagEnum.AMIS_ADDSWEEP | MissionDataFlagEnum.AMIS_NOTHREAT | MissionDataFlagEnum.AMIS_AIR_LAUNCH_OK | MissionDataFlagEnum.AMIS_NO_DIST_BONUS | MissionDataFlagEnum.AMIS_FLYALWAYS ),
    new MissionDataType( MissionTypeEnum.AMIS_RECON, MissionTargetTypeEnum.AMIS_TAR_OBJECTIVE, MissionRollEnum.ARO_REC, MissionProfileEnum.MPROF_LOW, TargetAreaProfileEnum.TPROF_ATTACK, TargetDescriptionType.TDESC_TAO, WPAction.WP_NOTHING, WPAction.WP_RECON, 2, 600, 400,  0,  0, 2, 10, 40, MissionTypeEnum.AMIS_ESCORT, 1, 90, 0, MissionDataFlagEnum.AMIS_ADDBARCAP ),
    new MissionDataType( MissionTypeEnum.AMIS_BDA, MissionTargetTypeEnum.AMIS_TAR_OBJECTIVE, MissionRollEnum.ARO_REC, MPROF_STANDARD, TargetAreaProfileEnum.TPROF_ATTACK, TargetDescriptionType.TDESC_TAO, WPAction.WP_NOTHING, WPAction.WP_RECON, 2, 200, 100, 120,  0, 2, 10, 40, MissionTypeEnum.AMIS_NONE, 1,  0, 0, MissionDataFlagEnum.AMIS_AVOIDTHREAT | MissionDataFlagEnum.AMIS_MATCHSPEED | MissionDataFlagEnum.AMIS_NO_DIST_BONUS ),
    new MissionDataType( MissionTypeEnum.AMIS_ECM, MissionTargetTypeEnum.AMIS_TAR_LOCATION, MissionRollEnum.ARO_ECM, MissionProfileEnum.MPROF_HIGH, TargetAreaProfileEnum.TPROF_LOITER, TargetDescriptionType.TDESC_TAO, WPAction.WP_JAM, WPAction.WP_JAM, 100, 500, 200,  0, 60, 1, 10, 60, MissionTypeEnum.AMIS_HAVCAP,   30, 60, 0, MissionDataFlagEnum.AMIS_ADDESCORT | MissionDataFlagEnum.AMIS_ADDSWEEP | MissionDataFlagEnum.AMIS_NOTHREAT | MissionDataFlagEnum.AMIS_NO_DIST_BONUS | MissionDataFlagEnum.AMIS_FLYALWAYS ),
    new MissionDataType( MissionTypeEnum.AMIS_AIRCAV, MissionTargetTypeEnum.AMIS_TAR_UNIT, MissionRollEnum.ARO_TACTRANS, MissionProfileEnum.MPROF_LOW, TargetAreaProfileEnum.TPROF_LAND, TargetDescriptionType.TDESC_ATA, WPAction.WP_NOTHING, WPAction.WP_AIRDROP, 5,  25,   5,  0,  2, 4,  5, 40, MissionTypeEnum.AMIS_SWEEP, 1,  0, (byte)VEH_FLAGS.VEH_VTOL, MissionDataFlagEnum.AMIS_AVOIDTHREAT | MissionDataFlagEnum.AMIS_ADDSWEEP | MissionDataFlagEnum.AMIS_ADDESCORT | MissionDataFlagEnum.AMIS_MATCHSPEED | MissionDataFlagEnum.AMIS_DONT_USE_AC | MissionDataFlagEnum.AMIS_FUDGE_RANGE | MissionDataFlagEnum.AMIS_NO_TARGETABORT | MissionDataFlagEnum.AMIS_FLYALWAYS | MissionDataFlagEnum.AMIS_HIGHTHREAT /* KCK: TO ALLOW MORE TO FLY */ ),
    new MissionDataType( MissionTypeEnum.AMIS_AIRLIFT, MissionTargetTypeEnum.AMIS_TAR_LOCATION, MissionRollEnum.ARO_TRANS, MissionProfileEnum.MPROF_HIGH, TargetAreaProfileEnum.TPROF_LAND, TargetDescriptionType.TDESC_TTL, WPAction.WP_NOTHING, WPAction.WP_LAND, 100, 300, 200,  0, 30, 1,  5, 40, MissionTypeEnum.AMIS_ESCORT, 1,  0, 0, MissionDataFlagEnum.AMIS_NOTHREAT | MissionDataFlagEnum.AMIS_MATCHSPEED | MissionDataFlagEnum.AMIS_DONT_USE_AC | MissionDataFlagEnum.AMIS_AIR_LAUNCH_OK | MissionDataFlagEnum.AMIS_NO_TARGETABORT | MissionDataFlagEnum.AMIS_NO_DIST_BONUS | MissionDataFlagEnum.AMIS_FLYALWAYS ),
   new MissionDataType( MissionTypeEnum.AMIS_SAR, MissionTargetTypeEnum.AMIS_TAR_LOCATION, MissionRollEnum.ARO_TACTRANS, MissionProfileEnum.MPROF_LOW, TargetAreaProfileEnum.TPROF_TARGET, TargetDescriptionType.TDESC_TAO, WPAction.WP_NOTHING, WPAction.WP_RESCUE, 1,  25,   5,  0,  0, 1,  5, 40, MissionTypeEnum.AMIS_RESCAP, 1,  0, (byte)VEH_FLAGS.VEH_VTOL, MissionDataFlagEnum.AMIS_AVOIDTHREAT | MissionDataFlagEnum.AMIS_HIGHTHREAT | MissionDataFlagEnum.AMIS_ADDESCORT | MissionDataFlagEnum.AMIS_ADDBARCAP | MissionDataFlagEnum.AMIS_NO_TARGETABORT | MissionDataFlagEnum.AMIS_FLYALWAYS ),
    new MissionDataType( MissionTypeEnum.AMIS_ASW, MissionTargetTypeEnum.AMIS_TAR_UNIT, MissionRollEnum.ARO_ASW, MissionProfileEnum.MPROF_LOW, TargetAreaProfileEnum.TPROF_SEARCH, TargetDescriptionType.TDESC_ATA, WPAction.WP_ASW, WPAction.WP_ASW, 5, 100,  50,  0,  0, 1,  5, 40, MissionTypeEnum.AMIS_SWEEP,    20,  0, (byte)VEH_FLAGS.VEH_NAVY, MissionDataFlagEnum.AMIS_ADDSWEEP | MissionDataFlagEnum.AMIS_MATCHSPEED | MissionDataFlagEnum.AMIS_NO_TARGETABORT | MissionDataFlagEnum.AMIS_NO_DIST_BONUS | MissionDataFlagEnum.AMIS_FLYALWAYS ),
    new MissionDataType( MissionTypeEnum.AMIS_ASHIP, MissionTargetTypeEnum.AMIS_TAR_UNIT, MissionRollEnum.ARO_ASHIP, MPROF_STANDARD, TargetAreaProfileEnum.TPROF_ATTACK, TargetDescriptionType.TDESC_TAO, WPAction.WP_NOTHING, WPAction.WP_NAVSTRIKE, 5, 100,  80,  0,  0, 2, 10, 40, MissionTypeEnum.AMIS_ESCORT, 1,  0, 0, MissionDataFlagEnum.AMIS_ADDESCORT | MissionDataFlagEnum.AMIS_ADDBARCAP | MissionDataFlagEnum.AMIS_MATCHSPEED | MissionDataFlagEnum.AMIS_NO_TARGETABORT | MissionDataFlagEnum.AMIS_NO_DIST_BONUS ),
    new MissionDataType( MissionTypeEnum.AMIS_PATROL, MissionTargetTypeEnum.AMIS_TAR_LOCATION, MissionRollEnum.ARO_REC, MPROF_STANDARD, TargetAreaProfileEnum.TPROF_TARGET, TargetDescriptionType.TDESC_ATA, WPAction.WP_RECON, WPAction.WP_RECON, 100, 500,  50,  0, 30, 1, 10, 60, MissionTypeEnum.AMIS_SWEEP,    20, 60, (byte)VEH_FLAGS.VEH_NAVY, MissionDataFlagEnum.AMIS_ADDSWEEP | MissionDataFlagEnum.AMIS_MATCHSPEED | MissionDataFlagEnum.AMIS_NO_DIST_BONUS ),
    new MissionDataType( MissionTypeEnum.AMIS_RECONPATROL, MissionTargetTypeEnum.AMIS_TAR_LOCATION, MissionRollEnum.ARO_REC, MissionProfileEnum.MPROF_LOW, TargetAreaProfileEnum.TPROF_LOITER, TargetDescriptionType.TDESC_TAO, WPAction.WP_RECON, WPAction.WP_RECON, 5,  25,   5,  0, 30, 2,  5, 60, MissionTypeEnum.AMIS_SWEEP,    10, 60, (byte)(VEH_FLAGS.VEH_VTOL | VEH_FLAGS.VEH_ARMY), MissionDataFlagEnum.AMIS_ADDSWEEP | MissionDataFlagEnum.AMIS_NPC_ONLY | MissionDataFlagEnum.AMIS_DONT_COORD | MissionDataFlagEnum.AMIS_TARGET_ONLY ),
    new MissionDataType( MissionTypeEnum.AMIS_ABORT, MissionTargetTypeEnum.AMIS_TAR_LOCATION, 0, MissionProfileEnum.MPROF_LOW, TargetAreaProfileEnum.TPROF_TARGET, TargetDescriptionType.TDESC_NONE, WPAction.WP_NOTHING, WPAction.WP_NOTHING, 5, 500, 100,  0,  0, 0,  0, 60, MissionTypeEnum.AMIS_NONE, 0,  0, 0, MissionDataFlagEnum.AMIS_FLYALWAYS ),
   new MissionDataType( MissionTypeEnum.AMIS_TRAINING, MissionTargetTypeEnum.AMIS_TAR_NONE, 0, MPROF_STANDARD, TargetAreaProfileEnum.TPROF_ATTACK, TargetDescriptionType.TDESC_NONE, WPAction.WP_NOTHING, WPAction.WP_NOTHING, 0,   0,   0,  0,  0, 0,  0,  0, MissionTypeEnum.AMIS_NONE, 0,  0, 0, MissionDataFlagEnum.AMIS_FLYALWAYS )
};


        // ===================================================
        // Global functions
        // ===================================================

        public static int BuildPathToTarget(Flight u, MissionRequestClass mis, VU_ID airbaseID)
        {
            throw new NotImplementedException();
        }

        public static void BuildDivertPath(Flight u, MissionRequestClass mis)
        {
            throw new NotImplementedException();
        }

        public static void AddInformationWPs(Flight u, MissionRequestClass mis)
        {
            throw new NotImplementedException();
        }

        public static void ClearDivertWayPoints(Flight flight)
        {
            throw new NotImplementedException();
        }

        public static int AddTankerWayPoint(Flight u, int refuel)
        {
            throw new NotImplementedException();
        } // M.N.

        public static long SetWPTimes(Flight u, MissionRequestClass mis)
        {
            throw new NotImplementedException();
        }

        public static long SetWPTimesTanker(Flight u, MissionRequestClass mis, bool type, CampaignTime time)
        {
            throw new NotImplementedException();
        }

        public static int SetWPAltitudes(Flight u)
        {
            throw new NotImplementedException();
        }

        public static int CheckPathThreats(Unit u)
        {
            throw new NotImplementedException();
        }

        public static int TargetThreats(Team team, int priority, F4PFList list, MoveType mt, CampaignTime time, long target_flags, ref short targeted)
        {
            throw new NotImplementedException();
        }

        public static bool LoadMissionData()
        {
            throw new NotImplementedException();
        }
    }
}

