using FalconNet.Campaign;
using FalconNet.FalcLib;
using System;
using System.Collections.Generic;


namespace FalconNet.UICampaign
{
    [Flags]
    public enum OBTV // Objective Filter Types
    {
        NOOBTV = 0,
        _OBTV_AIR_DEFENSE = 0x00000001,
        _OBTV_AIR_FIELDS = 0x00000002,
        _OBTV_ARMY = 0x00000004,
        _OBTV_CCC = 0x00000008,
        _OBTV_INFRASTRUCTURE = 0x00000010,
        _OBTV_LOGISTICS = 0x00000020,
        _OBTV_OTHER = 0x00000040,
        _OBTV_NAVIGATION = 0x00000080,
        _OBTV_POLITICAL = 0x00000100,
        _OBTV_WAR_PRODUCTION = 0x00000200,
        _OBTV_NAVAL = 0x00000400,
        _UNIT_SQUADRON = 0x00000800,
        _UNIT_PACKAGE = 0x00001000,
        _VC_CONDITION_ = 0x00002000,
    }

    [Flags]
    public enum UNIT
    {
        NOUNIT = 0x00000000,
        _UNIT_AIR_DEFENSE = 0x00000001,
        _UNIT_COMBAT = 0x00000002,
        _UNIT_SUPPORT = 0x00000004,
        _UNIT_ARTILLERY = 0x00000008,
        _UNIT_ATTACK = 0x00000010,
        _UNIT_HELICOPTER = 0x00000020,
        _UNIT_BOMBER = 0x00000040,
        _UNIT_FIGHTER = 0x00000080,
        _UNIT_BATTALION = 0x01000000,
        _UNIT_BRIGADE = 0x02000000,
        _UNIT_DIVISION = 0x04000000,
        _UNIT_NAVAL = 0x08000000,
        _UNIT_GROUND_MASK = 0x07000000,
        _UNIT_NAVAL_MASK = 0x08000000,
    }

    [Flags]
    public enum OOB:uint
    {

        OOB_AIRFORCE = 0x00010000,
        OOB_ARMY = 0x00020000,
        OOB_NAVY = 0x00040000,
        OOB_OBJECTIVE = 0x00080000,
        OOB_TEAM_MASK = 0xff000000,
    }

    [Flags]
    public enum THR
    {
        _THR_SAM_LOW = 0x01,
        _THR_SAM_HIGH = 0x02,
        _THR_RADAR_LOW = 0x04,
        _THR_RADAR_HIGH = 0x08,
    }

    [Flags]
    public enum THREAT
    {
        _THREAT_SAM_LOW_ = 0,
        _THREAT_SAM_HIGH_,
        _THREAT_RADAR_LOW_,
        _THREAT_RADAR_HIGH_,
    }

    [Flags]
    public enum ICONID // Ship/Air Unit Icon IDs
    {
        TGT_CUR = 10501,
        TGT_CUR_SEL = 10502,
        TGT_CUR_ERROR = 10503,
        IP_CUR = 10504,
        IP_CUR_SEL = 10505,
        IP_CUR_ERROR = 10506,
        STPT_CUR = 10507,
        STPT_CUR_SEL = 10508,
        STPT_CUR_ERROR = 10509,
        TGT_OTR = 10510,
        TGT_OTR_SEL = 10511,
        TGT_OTR_OTHER = 10512,
        IP_OTR = 10513,
        IP_OTR_SEL = 10514,
        IP_OTR_OTHER = 10515,
        STPT_OTR = 10516,
        STPT_OTR_SEL = 10517,
        STPT_OTR_OTHER = 10518,
        ASSIGNED_TGT_CUR = 10519,
        HOME_BASE_CUR = 10520,
        ADDLINE_CUR = 10521,
        ADDLINE_CUR_SEL = 10522,
    }

    public struct FILTER_TABLE
    {
        public Domains Domain;
        public Classes Class;
        public ClassTypes Type;
        public SubTypes SubType;
        public OBTV UIType;
        public OOB OOBCategory;
        public UNIT unit;
        public FILTER_TABLE(Domains pdomain,
                              Classes pclass,
                              ClassTypes ptype,
                              SubTypes psubtype,
                               OBTV puitype,
                               OOB poob)
        {
            Domain = pdomain;
            Class = pclass;
            Type = ptype;
            SubType = psubtype;
            UIType = puitype;
            OOBCategory = poob;
            unit = UNIT.NOUNIT;
        }
        public FILTER_TABLE(Domains pdomain,
                      Classes pclass,
                      ClassTypes ptype,
                      SubTypes psubtype,
                       UNIT punit,
                       OOB poob)
        {
            Domain = pdomain;
            Class = pclass;
            Type = ptype;
            SubType = psubtype;
            UIType = OBTV.NOOBTV;
            OOBCategory = poob;
            unit = punit;
        }
    }

    public class AIR_ICONS
    {
        public SubTypes SubType;
        public long[] IconID; // 0=Friendly,1=Enemy,2=Neutral
        public UNIT UIType;
        public AIR_ICONS()
        {
            SubType = SubTypes.NOSUBTYPE;
            IconID = new long[3];
            UIType = UNIT.NOUNIT;
        }
        public AIR_ICONS(SubTypes stype, UNIT unit)
        {
            SubType = stype;
            IconID = new long[3];
            UIType = unit;
        }
    }

    public class filters
    {
        public const int _MAX_TEAMS_ = (int)(TeamDataEnum.NUM_TEAMS); // (8) Kevin only has 7 defined
        public const int _MAX_DIRECTIONS_ = (8);
        public const int _MIN_ZOOM_LEVEL_ = (1);
        public const int _MAX_ZOOM_LEVEL_ = (32);
        public const int I_NEED_TO_DRAW = (0x01);
        public const int I_NEED_TO_DRAW_MAP = (0x02);

        public const int _MAP_NUM_OBJ_TYPES_ = (14);
        public const int _MAP_NUM_AIR_TYPES_ = (5);
        public const int _MAP_NUM_GND_TYPES_ = (4);
        public const int _MAP_NUM_GND_LEVELS_ = (3);
        public const int _MAP_NUM_NAV_TYPES_ = (2);
        public const int _MAP_NUM_THREAT_TYPES_ = (4);

        public static List<FILTER_TABLE> ObjectiveFilters = new List<FILTER_TABLE>()
                {
	            new FILTER_TABLE( Domains.DOMAIN_LAND,Classes.CLASS_OBJECTIVE,ClassTypes.TYPE_RADAR,(SubTypes)0,OBTV._OBTV_AIR_DEFENSE, OOB.OOB_ARMY),
	            new FILTER_TABLE(Domains.DOMAIN_LAND,Classes.CLASS_OBJECTIVE,ClassTypes.TYPE_SAM_SITE,(SubTypes)0,OBTV._OBTV_AIR_DEFENSE, OOB.OOB_ARMY),
	            new FILTER_TABLE( Domains.DOMAIN_LAND,Classes.CLASS_OBJECTIVE,ClassTypes.TYPE_AIRBASE,(SubTypes)0,OBTV._OBTV_AIR_FIELDS, OOB.OOB_AIRFORCE),
	            new FILTER_TABLE(  Domains.DOMAIN_LAND,Classes.CLASS_OBJECTIVE,ClassTypes.TYPE_AIRSTRIP,(SubTypes)0,OBTV._OBTV_AIR_FIELDS, OOB.OOB_AIRFORCE),
	            new FILTER_TABLE( Domains.DOMAIN_LAND,Classes.CLASS_OBJECTIVE,ClassTypes.TYPE_ARMYBASE,(SubTypes)0,OBTV._OBTV_ARMY, OOB.OOB_ARMY),
	            new FILTER_TABLE(  Domains.DOMAIN_LAND,Classes.CLASS_OBJECTIVE,ClassTypes.TYPE_FORTIFICATION,(SubTypes)0,OBTV._OBTV_ARMY, OOB.OOB_ARMY),
	            new FILTER_TABLE(  Domains.DOMAIN_LAND,Classes.CLASS_OBJECTIVE,ClassTypes.TYPE_HARTS,(SubTypes)0,OBTV._OBTV_ARMY, OOB.OOB_ARMY),
	            new FILTER_TABLE(  Domains.DOMAIN_LAND,Classes.CLASS_OBJECTIVE,ClassTypes.TYPE_COM_CONTROL,(SubTypes)0,OBTV._OBTV_CCC, OOB.OOB_OBJECTIVE),
	            new FILTER_TABLE(  Domains.DOMAIN_LAND,Classes.CLASS_OBJECTIVE,ClassTypes.TYPE_RADIO_TOWER,(SubTypes)0,OBTV._OBTV_CCC, OOB.OOB_OBJECTIVE),
	            new FILTER_TABLE( Domains.DOMAIN_LAND,Classes.CLASS_OBJECTIVE,ClassTypes.TYPE_BRIDGE,(SubTypes)0,OBTV._OBTV_INFRASTRUCTURE, OOB.OOB_OBJECTIVE),
	            new FILTER_TABLE(  Domains.DOMAIN_LAND,Classes.CLASS_OBJECTIVE,ClassTypes.TYPE_POWERPLANT,(SubTypes)0,OBTV._OBTV_WAR_PRODUCTION, OOB.OOB_OBJECTIVE),
	            new FILTER_TABLE(  Domains.DOMAIN_LAND,Classes.CLASS_OBJECTIVE,ClassTypes.TYPE_RAIL_TERMINAL,(SubTypes)0,OBTV._OBTV_INFRASTRUCTURE, OOB.OOB_OBJECTIVE),
	            new FILTER_TABLE(  Domains.DOMAIN_LAND,Classes.CLASS_OBJECTIVE,ClassTypes.TYPE_RAILROAD,(SubTypes)0,OBTV._OBTV_INFRASTRUCTURE, OOB.OOB_OBJECTIVE),
	            new FILTER_TABLE(  Domains.DOMAIN_LAND,Classes.CLASS_OBJECTIVE,ClassTypes.TYPE_DEPOT,(SubTypes)0,OBTV._OBTV_LOGISTICS, OOB.OOB_OBJECTIVE),
	            new FILTER_TABLE(  Domains.DOMAIN_LAND,Classes.CLASS_OBJECTIVE,ClassTypes.TYPE_PORT,(SubTypes)0,OBTV._OBTV_LOGISTICS, OOB.OOB_NAVY),
	            new FILTER_TABLE(  Domains.DOMAIN_LAND,Classes.CLASS_OBJECTIVE,ClassTypes.TYPE_BEACH,(SubTypes)0,OBTV._OBTV_OTHER, OOB.OOB_OBJECTIVE),
	            new FILTER_TABLE(  Domains.DOMAIN_LAND,Classes.CLASS_OBJECTIVE,ClassTypes.TYPE_FORD,(SubTypes)0,OBTV._OBTV_OTHER, OOB.OOB_OBJECTIVE),
	            new FILTER_TABLE(  Domains.DOMAIN_LAND,Classes.CLASS_OBJECTIVE,ClassTypes.TYPE_HILL_TOP,(SubTypes)0,OBTV._OBTV_OTHER, OOB.OOB_OBJECTIVE),
	            new FILTER_TABLE(  Domains.DOMAIN_LAND,Classes.CLASS_OBJECTIVE,ClassTypes.TYPE_PASS,(SubTypes)0,OBTV._OBTV_OTHER, OOB.OOB_OBJECTIVE),
	            new FILTER_TABLE(  Domains.DOMAIN_LAND,Classes.CLASS_OBJECTIVE,ClassTypes.TYPE_ROAD,(SubTypes)0,OBTV._OBTV_OTHER, OOB.OOB_OBJECTIVE),
	            new FILTER_TABLE(  Domains.DOMAIN_LAND,Classes.CLASS_OBJECTIVE,ClassTypes.TYPE_SEA,(SubTypes)0,OBTV._OBTV_OTHER, OOB.OOB_NAVY),
	            new FILTER_TABLE(  Domains.DOMAIN_LAND,Classes.CLASS_OBJECTIVE,ClassTypes.TYPE_NAV_BEACON,(SubTypes)0,OBTV._OBTV_NAVIGATION, OOB.OOB_OBJECTIVE),
	            new FILTER_TABLE(  Domains.DOMAIN_LAND,Classes.CLASS_OBJECTIVE,ClassTypes.TYPE_BORDER,(SubTypes)0,OBTV._OBTV_OTHER, OOB.OOB_OBJECTIVE),
	            new FILTER_TABLE(  Domains.DOMAIN_LAND,Classes.CLASS_OBJECTIVE,ClassTypes.TYPE_CITY,(SubTypes)0,OBTV._OBTV_POLITICAL, OOB.OOB_OBJECTIVE),
	            new FILTER_TABLE(  Domains.DOMAIN_LAND,Classes.CLASS_OBJECTIVE,ClassTypes.TYPE_TOWN,(SubTypes)0,OBTV._OBTV_POLITICAL, OOB.OOB_OBJECTIVE),
	            new FILTER_TABLE(  Domains.DOMAIN_LAND,Classes.CLASS_OBJECTIVE,ClassTypes.TYPE_VILLAGE,(SubTypes)0,OBTV._OBTV_POLITICAL, OOB.OOB_OBJECTIVE),
	            new FILTER_TABLE(  Domains.DOMAIN_LAND,Classes.CLASS_OBJECTIVE,ClassTypes.TYPE_CHEMICAL,(SubTypes)0,OBTV._OBTV_WAR_PRODUCTION, OOB.OOB_OBJECTIVE),
	            new FILTER_TABLE( Domains.DOMAIN_LAND,Classes.CLASS_OBJECTIVE,ClassTypes.TYPE_FACTORY,(SubTypes)0,OBTV._OBTV_WAR_PRODUCTION, OOB.OOB_OBJECTIVE),
	            new FILTER_TABLE(  Domains.DOMAIN_LAND,Classes.CLASS_OBJECTIVE,ClassTypes.TYPE_NUCLEAR,(SubTypes)0,OBTV._OBTV_WAR_PRODUCTION, OOB.OOB_OBJECTIVE),
	            new FILTER_TABLE(  Domains.DOMAIN_LAND,Classes.CLASS_OBJECTIVE,ClassTypes.TYPE_REFINERY,(SubTypes)0,OBTV._OBTV_WAR_PRODUCTION, OOB.OOB_OBJECTIVE),
	            new FILTER_TABLE(  Domains.DOMAIN_AIR, Classes.CLASS_UNIT,     ClassTypes.TYPE_SQUADRON,(SubTypes)0,OBTV._UNIT_SQUADRON, (OOB)0),
	            new FILTER_TABLE(  Domains.DOMAIN_AIR, Classes.CLASS_UNIT,     ClassTypes.TYPE_PACKAGE,(SubTypes)0,OBTV._UNIT_PACKAGE, (OOB)0)
                };

        public static List<FILTER_TABLE> UnitFilters = new List<FILTER_TABLE>()
            {
                 new FILTER_TABLE( Domains.DOMAIN_LAND,Classes.CLASS_UNIT,ClassTypes.TYPE_BATTALION,SubTypes.STYPE_UNIT_AIRBORNE,UNIT._UNIT_COMBAT|UNIT._UNIT_BATTALION, OOB.OOB_ARMY),
                 new FILTER_TABLE( Domains.DOMAIN_LAND,Classes.CLASS_UNIT,ClassTypes.TYPE_BATTALION,SubTypes.STYPE_UNIT_AIRMOBILE,UNIT._UNIT_COMBAT|UNIT._UNIT_BATTALION, OOB.OOB_ARMY),
                 new FILTER_TABLE( Domains.DOMAIN_LAND,Classes.CLASS_UNIT,ClassTypes.TYPE_BATTALION,SubTypes.STYPE_UNIT_AIR_DEFENSE,UNIT._UNIT_AIR_DEFENSE|UNIT._UNIT_BATTALION, OOB.OOB_ARMY),
                 new FILTER_TABLE( Domains.DOMAIN_LAND,Classes.CLASS_UNIT,ClassTypes.TYPE_BATTALION,SubTypes.STYPE_UNIT_ARMOR,UNIT._UNIT_COMBAT|UNIT._UNIT_BATTALION, OOB.OOB_ARMY),
                 new FILTER_TABLE( Domains.DOMAIN_LAND,Classes.CLASS_UNIT,ClassTypes.TYPE_BATTALION,SubTypes.STYPE_UNIT_ARMORED_CAV,UNIT._UNIT_COMBAT|UNIT._UNIT_BATTALION, OOB.OOB_ARMY),
                 new FILTER_TABLE( Domains.DOMAIN_LAND,Classes.CLASS_UNIT,ClassTypes.TYPE_BATTALION,SubTypes.STYPE_UNIT_ENGINEER,UNIT._UNIT_SUPPORT|UNIT._UNIT_BATTALION, OOB.OOB_ARMY),
                 new FILTER_TABLE( Domains.DOMAIN_LAND,Classes.CLASS_UNIT,ClassTypes.TYPE_BATTALION,SubTypes.STYPE_UNIT_HQ,UNIT._UNIT_SUPPORT|UNIT._UNIT_BATTALION, OOB.OOB_ARMY),
                 new FILTER_TABLE( Domains.DOMAIN_LAND,Classes.CLASS_UNIT,ClassTypes.TYPE_BATTALION,SubTypes.STYPE_UNIT_INFANTRY,UNIT._UNIT_COMBAT|UNIT._UNIT_BATTALION, OOB.OOB_ARMY),
                 new FILTER_TABLE( Domains.DOMAIN_LAND,Classes.CLASS_UNIT,ClassTypes.TYPE_BATTALION,SubTypes.STYPE_UNIT_MARINE,UNIT._UNIT_COMBAT|UNIT._UNIT_BATTALION, OOB.OOB_ARMY),
                 new FILTER_TABLE( Domains.DOMAIN_LAND,Classes.CLASS_UNIT,ClassTypes.TYPE_BATTALION,SubTypes.STYPE_UNIT_MECHANIZED,UNIT._UNIT_COMBAT|UNIT._UNIT_BATTALION, OOB.OOB_ARMY),
                 new FILTER_TABLE( Domains.DOMAIN_LAND,Classes.CLASS_UNIT,ClassTypes.TYPE_BATTALION,SubTypes.STYPE_UNIT_ROCKET,UNIT._UNIT_SUPPORT|UNIT._UNIT_BATTALION, OOB.OOB_ARMY),
                 new FILTER_TABLE( Domains.DOMAIN_LAND,Classes.CLASS_UNIT,ClassTypes.TYPE_BATTALION,SubTypes.STYPE_UNIT_SP_ARTILLERY,UNIT._UNIT_SUPPORT|UNIT._UNIT_BATTALION, OOB.OOB_ARMY),
                 new FILTER_TABLE( Domains.DOMAIN_LAND,Classes.CLASS_UNIT,ClassTypes.TYPE_BATTALION,SubTypes.STYPE_UNIT_SS_MISSILE,UNIT._UNIT_SUPPORT|UNIT._UNIT_BATTALION, OOB.OOB_ARMY),
                 new FILTER_TABLE( Domains.DOMAIN_LAND,Classes.CLASS_UNIT,ClassTypes.TYPE_BATTALION,SubTypes.STYPE_UNIT_SUPPLY,UNIT._UNIT_SUPPORT|UNIT._UNIT_BATTALION, OOB.OOB_ARMY),
                 new FILTER_TABLE( Domains.DOMAIN_LAND,Classes.CLASS_UNIT,ClassTypes.TYPE_BATTALION,SubTypes.STYPE_UNIT_TOWED_ARTILLERY,UNIT._UNIT_SUPPORT|UNIT._UNIT_BATTALION, OOB.OOB_ARMY),
                 new FILTER_TABLE( Domains.DOMAIN_LAND,Classes.CLASS_UNIT,ClassTypes.TYPE_BRIGADE,SubTypes.STYPE_UNIT_AIRBORNE,UNIT._UNIT_COMBAT|UNIT._UNIT_BRIGADE, OOB.OOB_ARMY),
                 new FILTER_TABLE( Domains.DOMAIN_LAND,Classes.CLASS_UNIT,ClassTypes.TYPE_BRIGADE,SubTypes.STYPE_UNIT_AIRMOBILE,UNIT._UNIT_COMBAT|UNIT._UNIT_BRIGADE, OOB.OOB_ARMY),
                 new FILTER_TABLE( Domains.DOMAIN_LAND,Classes.CLASS_UNIT,ClassTypes.TYPE_BRIGADE,SubTypes.STYPE_UNIT_AIR_DEFENSE,UNIT._UNIT_AIR_DEFENSE|UNIT._UNIT_BRIGADE, OOB.OOB_ARMY),
                 new FILTER_TABLE( Domains.DOMAIN_LAND,Classes.CLASS_UNIT,ClassTypes.TYPE_BRIGADE,SubTypes.STYPE_UNIT_ARMOR,UNIT._UNIT_COMBAT|UNIT._UNIT_BRIGADE, OOB.OOB_ARMY),
                 new FILTER_TABLE( Domains.DOMAIN_LAND,Classes.CLASS_UNIT,ClassTypes.TYPE_BRIGADE,SubTypes.STYPE_UNIT_ARMORED_CAV,UNIT._UNIT_COMBAT|UNIT._UNIT_BRIGADE, OOB.OOB_ARMY),
                 new FILTER_TABLE( Domains.DOMAIN_LAND,Classes.CLASS_UNIT,ClassTypes.TYPE_BRIGADE,SubTypes.STYPE_UNIT_ENGINEER,UNIT._UNIT_SUPPORT|UNIT._UNIT_BRIGADE, OOB.OOB_ARMY),
                 new FILTER_TABLE( Domains.DOMAIN_LAND,Classes.CLASS_UNIT,ClassTypes.TYPE_BRIGADE,SubTypes.STYPE_UNIT_HQ,UNIT._UNIT_SUPPORT|UNIT._UNIT_BRIGADE, OOB.OOB_ARMY),
                 new FILTER_TABLE( Domains.DOMAIN_LAND,Classes.CLASS_UNIT,ClassTypes.TYPE_BRIGADE,SubTypes.STYPE_UNIT_INFANTRY,UNIT._UNIT_COMBAT|UNIT._UNIT_BRIGADE, OOB.OOB_ARMY),
                 new FILTER_TABLE( Domains.DOMAIN_LAND,Classes.CLASS_UNIT,ClassTypes.TYPE_BRIGADE,SubTypes.STYPE_UNIT_MARINE,UNIT._UNIT_COMBAT|UNIT._UNIT_BRIGADE, OOB.OOB_ARMY),
                 new FILTER_TABLE( Domains.DOMAIN_LAND,Classes.CLASS_UNIT,ClassTypes.TYPE_BRIGADE,SubTypes.STYPE_UNIT_MECHANIZED,UNIT._UNIT_COMBAT|UNIT._UNIT_BRIGADE, OOB.OOB_ARMY),
                 new FILTER_TABLE( Domains.DOMAIN_LAND,Classes.CLASS_UNIT,ClassTypes.TYPE_BRIGADE,SubTypes.STYPE_UNIT_ROCKET,UNIT._UNIT_SUPPORT|UNIT._UNIT_BRIGADE, OOB.OOB_ARMY),
                 new FILTER_TABLE( Domains.DOMAIN_LAND,Classes.CLASS_UNIT,ClassTypes.TYPE_BRIGADE,SubTypes.STYPE_UNIT_SP_ARTILLERY,UNIT._UNIT_SUPPORT|UNIT._UNIT_BRIGADE, OOB.OOB_ARMY),
                 new FILTER_TABLE( Domains.DOMAIN_LAND,Classes.CLASS_UNIT,ClassTypes.TYPE_BRIGADE,SubTypes.STYPE_UNIT_SS_MISSILE,UNIT._UNIT_SUPPORT|UNIT._UNIT_BRIGADE, OOB.OOB_ARMY),
                 new FILTER_TABLE( Domains.DOMAIN_LAND,Classes.CLASS_UNIT,ClassTypes.TYPE_BRIGADE,SubTypes.STYPE_UNIT_SUPPLY,UNIT._UNIT_SUPPORT|UNIT._UNIT_BRIGADE, OOB.OOB_ARMY),
                 new FILTER_TABLE( Domains.DOMAIN_LAND,Classes.CLASS_UNIT,ClassTypes.TYPE_BRIGADE,SubTypes.STYPE_UNIT_TOWED_ARTILLERY,UNIT._UNIT_SUPPORT|UNIT._UNIT_BRIGADE, OOB.OOB_ARMY),
                 new FILTER_TABLE( Domains.DOMAIN_SEA,Classes.CLASS_UNIT,ClassTypes.TYPE_TASKFORCE,SubTypes.STYPE_UNIT_AMPHIBIOUS,UNIT._UNIT_COMBAT|UNIT._UNIT_NAVAL, OOB.OOB_NAVY),
                 new FILTER_TABLE( Domains.DOMAIN_SEA,Classes.CLASS_UNIT,ClassTypes.TYPE_TASKFORCE,SubTypes.STYPE_UNIT_CARRIER,UNIT._UNIT_COMBAT|UNIT._UNIT_NAVAL, OOB.OOB_NAVY),
                 new FILTER_TABLE( Domains.DOMAIN_SEA,Classes.CLASS_UNIT,ClassTypes.TYPE_TASKFORCE,SubTypes.STYPE_UNIT_CRUISER,UNIT._UNIT_COMBAT|UNIT._UNIT_NAVAL, OOB.OOB_NAVY),
                 new FILTER_TABLE( Domains.DOMAIN_SEA,Classes.CLASS_UNIT,ClassTypes.TYPE_TASKFORCE,SubTypes.STYPE_UNIT_DESTROYER,UNIT._UNIT_COMBAT|UNIT._UNIT_NAVAL, OOB.OOB_NAVY),
                 new FILTER_TABLE( Domains.DOMAIN_SEA,Classes.CLASS_UNIT,ClassTypes.TYPE_TASKFORCE,SubTypes.STYPE_UNIT_FRIGATE,UNIT._UNIT_COMBAT|UNIT._UNIT_NAVAL, OOB.OOB_NAVY),
                 new FILTER_TABLE( Domains.DOMAIN_SEA,Classes.CLASS_UNIT,ClassTypes.TYPE_TASKFORCE,SubTypes.STYPE_UNIT_PATROL,UNIT._UNIT_COMBAT|UNIT._UNIT_NAVAL, OOB.OOB_NAVY),
                 new FILTER_TABLE( Domains.DOMAIN_SEA,Classes.CLASS_UNIT,ClassTypes.TYPE_TASKFORCE,SubTypes.STYPE_UNIT_SEA_SUPPLY,UNIT._UNIT_SUPPORT|UNIT._UNIT_NAVAL, OOB.OOB_NAVY),
                 new FILTER_TABLE( Domains.DOMAIN_SEA,Classes.CLASS_UNIT,ClassTypes.TYPE_TASKFORCE,SubTypes.STYPE_UNIT_SEA_TANKER,UNIT._UNIT_SUPPORT|UNIT._UNIT_NAVAL, OOB.OOB_NAVY),
                 new FILTER_TABLE( Domains.DOMAIN_AIR,Classes.CLASS_UNIT,ClassTypes.TYPE_SQUADRON, SubTypes.UI_VU_ANY,            UNIT.NOUNIT,                         OOB.OOB_AIRFORCE)
            };

        public static List<AIR_ICONS> AirIcons = new List<AIR_ICONS> {
                new AIR_ICONS ( SubTypes.STYPE_UNIT_AIR_TRANSPORT, UNIT._UNIT_SUPPORT),
                new AIR_ICONS ( SubTypes.STYPE_UNIT_ASW,			UNIT._UNIT_SUPPORT),
                new AIR_ICONS ( SubTypes.STYPE_UNIT_ATTACK,			UNIT._UNIT_ATTACK),
                new AIR_ICONS ( SubTypes.STYPE_UNIT_ATTACK_HELO,	UNIT._UNIT_HELICOPTER),
                new AIR_ICONS ( SubTypes.STYPE_UNIT_AWACS,			 UNIT._UNIT_SUPPORT),
                new AIR_ICONS ( SubTypes.STYPE_UNIT_BOMBER,		    UNIT._UNIT_BOMBER),
                new AIR_ICONS ( SubTypes.STYPE_UNIT_ECM,			UNIT. _UNIT_SUPPORT),
                new AIR_ICONS ( SubTypes.STYPE_UNIT_FIGHTER,	    UNIT._UNIT_FIGHTER),
                new AIR_ICONS ( SubTypes.STYPE_UNIT_FIGHTER_BOMBER,	 UNIT._UNIT_FIGHTER),
                new AIR_ICONS ( SubTypes.STYPE_UNIT_JSTAR,			 UNIT._UNIT_SUPPORT),
                new AIR_ICONS ( SubTypes.STYPE_UNIT_RECON,			 UNIT._UNIT_SUPPORT),
                new AIR_ICONS ( SubTypes.STYPE_UNIT_RECON_HELO,		 UNIT._UNIT_HELICOPTER),
                new AIR_ICONS ( SubTypes.STYPE_UNIT_TANKER,			UNIT. _UNIT_SUPPORT),
                new AIR_ICONS ( SubTypes.STYPE_UNIT_TRANSPORT_HELO,	 UNIT._UNIT_HELICOPTER)
            };

        public static OBTV[] OBJ_TypeList = new OBTV[_MAP_NUM_OBJ_TYPES_]
                                            {
	                                            OBTV._VC_CONDITION_,
	                                            OBTV._OBTV_AIR_DEFENSE,
	                                            OBTV._OBTV_AIR_FIELDS,
	                                            OBTV._OBTV_ARMY,
	                                            OBTV._OBTV_CCC,
	                                            OBTV._OBTV_INFRASTRUCTURE,
	                                            OBTV._OBTV_LOGISTICS,
	                                            OBTV._OBTV_OTHER,
	                                            OBTV._OBTV_NAVIGATION,
	                                            OBTV._OBTV_POLITICAL,
	                                            OBTV._OBTV_WAR_PRODUCTION,
	                                            OBTV._OBTV_NAVAL,
	                                            OBTV._UNIT_PACKAGE,
	                                            OBTV._UNIT_SQUADRON,
                                            };

        public static UNIT[] NAV_TypeList = new UNIT[_MAP_NUM_NAV_TYPES_]
                                            {
	                                            UNIT._UNIT_COMBAT,
	                                            UNIT._UNIT_SUPPORT,
                                            };

        public static UNIT[] AIR_TypeList = new UNIT[_MAP_NUM_AIR_TYPES_]
                                            {
	                                            UNIT._UNIT_SUPPORT,
	                                            UNIT._UNIT_ATTACK,
	                                            UNIT._UNIT_HELICOPTER,
	                                            UNIT._UNIT_BOMBER,
	                                            UNIT._UNIT_FIGHTER,
                                            };
        public static UNIT[] GND_TypeList = new UNIT[_MAP_NUM_GND_TYPES_]
                                            {
	                                            UNIT._UNIT_AIR_DEFENSE,
	                                            UNIT._UNIT_COMBAT,
	                                            UNIT._UNIT_SUPPORT,
	                                            UNIT._UNIT_ARTILLERY,
                                            };
        public static UNIT[] GND_LevelList = new UNIT[_MAP_NUM_GND_LEVELS_]
                                            {
	                                            UNIT._UNIT_DIVISION,
	                                            UNIT._UNIT_BRIGADE,
	                                            UNIT._UNIT_BATTALION,
                                            };
        public static THR[] THR_TypeList = new THR[_MAP_NUM_THREAT_TYPES_]
                                            {
	                                            THR._THR_SAM_LOW,
	                                            THR._THR_SAM_HIGH,
	                                            THR._THR_RADAR_LOW,
	                                            THR._THR_RADAR_HIGH,
                                            };
      

        public static long FindTypeIndex(long type, long[] TypeList, int size)
        {
            int i;

            for (i = 0; i < size; i++)
                if (type == TypeList[i])
                    return (i);
            return (-1);
        }
        public static long FindObjectiveIndex(ObjectiveClass Obj)
        {
            for (int i = 0; i < ObjectiveFilters.Count; i++)
            {
                if ((Classes)Obj.GetClass() == ObjectiveFilters[i].Class && Obj.GetFalconType() == ObjectiveFilters[i].Type)
                    return (i);
            }
            return (-1);
        }
        public static OBTV GetObjectiveType(CampBaseClass ent)
        {
            for (int i = 0; i < ObjectiveFilters.Count; i++ )
            {
                if ((Domains)ent.GetDomain() == ObjectiveFilters[i].Domain && (Classes)ent.GetClass() == ObjectiveFilters[i].Class && ent.GetFalconType() == ObjectiveFilters[i].Type)
                    return (ObjectiveFilters[i].UIType);
            }
            return (0);
        }
        public static OOB GetObjectiveCategory(ObjectiveClass Obj)
        {
            for (int i = 0; i < ObjectiveFilters.Count; i++)
            {
                if ((Classes)Obj.GetClass() == ObjectiveFilters[i].Class && Obj.GetFalconType() == ObjectiveFilters[i].Type)
                    return (ObjectiveFilters[i].OOBCategory);
            }
            return (0);
        }
        public static long GetAirIcon(SubTypes STYPE)
        {
            for (int i = 0; i < AirIcons.Count; i++)
            {
                if (AirIcons[i].SubType == STYPE)
                    return (i);
                i++;
            }
            return (0);
        }
        public static long FindDivisionType(byte type)
        {
            for (int i = 0; i < UnitFilters.Count; i++)
            {
                if (type == ((int)UnitFilters[i].SubType & 0x00ffffff))
                    return (((int)UnitFilters[i].UIType & 0x00ffffff) | (int)UNIT._UNIT_DIVISION);
                i++;
            }
            return (0);
        }
        public static OBTV FindUnitType(UnitClass unit)
        {
            for (int i = 0; i < UnitFilters.Count; i++)
            {
                if ((Domains)unit.GetDomain() == UnitFilters[i].Domain && (Classes)unit.GetClass() == UnitFilters[i].Class && unit.GetFalconType() == UnitFilters[i].Type && unit.GetSType() == UnitFilters[i].SubType)
                    return (UnitFilters[i].UIType);
                i++;
            }
            return (0);
        }
        public static OOB FindUnitCategory(UnitClass unit)
        {
            for (int i = 0; i < UnitFilters.Count; i++)
            {
                if ((Domains)unit.GetDomain() == UnitFilters[i].Domain && (Classes)unit.GetClass() == UnitFilters[i].Class && unit.GetFalconType() == UnitFilters[i].Type && ((SubTypes)unit.GetSType() == UnitFilters[i].SubType || UnitFilters[i].SubType == SubTypes.UI_VU_ANY))
                    return (UnitFilters[i].OOBCategory);
                i++;
            }
            return (0);
        }
    }
}
