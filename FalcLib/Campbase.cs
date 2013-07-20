using System;
using FalconNet.Common;
using FalconNet.FalcLib;
using FalconNet.VU;
using VU_BYTE = System.Byte;
using VU_ID_NUMBER = System.UInt64;
using Team = System.SByte;
using GridIndex = System.Int16;
using Control = System.Byte;
using System.Diagnostics;
using FalconNet.CampaignBase;
using FalconNet.Common.Maths;

namespace FalconNet.Campaign
{
    


    public static class CampbaseStatic
    {
        // ===================================
        // Camp base defines
        // ===================================

        // ===================================
        // Base class flags
        // ===================================




        // ===================================
        // Name space shit
        // ===================================
        /* TODO
public static int  FIRST_OBJECTIVE_VU_ID_NUMBER	(VU_FIRST_ENTITY_ID)
public static int  LAST_OBJECTIVE_VU_ID_NUMBER		(VU_FIRST_ENTITY_ID+MAX_NUMBER_OF_OBJECTIVES)
public static int  FIRST_NON_VOLITILE_VU_ID_NUMBER	(LAST_OBJECTIVE_VU_ID_NUMBER+1)
public static int  LAST_NON_VOLITILE_VU_ID_NUMBER	(FIRST_NON_VOLITILE_VU_ID_NUMBER+(MAX_NUMBER_OF_UNITS))
public static int  FIRST_LOW_VOLITILE_VU_ID_NUMBER	(LAST_NON_VOLITILE_VU_ID_NUMBER+1)
public static int  LAST_LOW_VOLITILE_VU_ID_NUMBER	(FIRST_LOW_VOLITILE_VU_ID_NUMBER+(MAX_NUMBER_OF_VOLITILE_UNITS))
public static int  FIRST_VOLITILE_VU_ID_NUMBER		(LAST_LOW_VOLITILE_VU_ID_NUMBER+1)
public static int  LAST_VOLITILE_VU_ID_NUMBER		~((VU_ID_NUMBER)0);


public static  VU_ID_NUMBER vuAssignmentId;
public static  VU_ID_NUMBER vuLowWrapNumber;
public static  VU_ID_NUMBER vuHighWrapNumber;
public static  VU_ID_NUMBER lastObjectiveId;
public static  VU_ID_NUMBER lastNonVolitileId;
public static  VU_ID_NUMBER lastLowVolitileId;
public static  VU_ID_NUMBER lastFlightId;
public static  VU_ID_NUMBER lastPackageId;
public static  VU_ID_NUMBER lastVolitileId;
    TODO */
        // ===================================
        // Camp base globals
        // ===================================

        public static byte[] CampSearch = new byte[Camplib.MAX_CAMP_ENTITIES];	// Search data - Could reduce to bitwise

        // ===========================
        // Global functions
        // ===========================

        public static CampBaseClass GetFirstEntity(F4LIt list)
        { throw new NotImplementedException(); }

        public static CampBaseClass GetNextEntity(F4LIt list)
        { throw new NotImplementedException(); }

        public static int Parent(CampBaseClass e)
        { throw new NotImplementedException(); }

        public static int Real(int type)
        { throw new NotImplementedException(); }

        public static short GetEntityClass(VuEntity h)
        { throw new NotImplementedException(); }

        public static short GetEntityDomain(VuEntity h)
        { throw new NotImplementedException(); }

        public static Unit GetEntityUnit(VuEntity h)
        { throw new NotImplementedException(); }

#if TODO
        public static Objective GetEntityObjective(VuEntity h)
        { throw new NotImplementedException(); }
#endif
        public static short FindUniqueID()
        { throw new NotImplementedException(); }

        public static int GetVisualDetectionRange(int mt)
        { throw new NotImplementedException(); }
    }

  
}
