using System;
using FalconNet.Common;
using FalconNet.FalcLib;
using FalconNet.VU;
using VU_BYTE = System.Byte;
using VU_ID_NUMBER = System.UInt64;
using VU_TIME = System.UInt64;
using Team = System.SByte;
using GridIndex = System.Int16;
using Control = System.Byte;
using System.Diagnostics;
using FalconNet.CampaignBase;
using FalconNet.Common.Maths;
using log4net;
//using Unit = FalconNet.Campaign.UnitClass;

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

        public static CampBaseClass GetFirstEntity(VuListIterator list)
        {
            VuEntity e;

            e = list.GetFirst();

            while (e != null)
            {
                //if (e.VuState() != VU_MEM_DELETED)
                if (GetEntityClass(e) == Classes.CLASS_UNIT || GetEntityClass(e) == Classes.CLASS_OBJECTIVE)
                {
                    return (CampBaseClass)e;
                }

                e = list.GetNext();
            }

            return null;
        }

        public static CampBaseClass GetNextEntity(VuListIterator list)
        {
            VuEntity e;

            e = list.GetNext();

            while (e != null)
            {
                //if (e.VuState() != VU_MEM_DELETED)
                if (GetEntityClass(e) == Classes.CLASS_UNIT || GetEntityClass(e) == Classes.CLASS_OBJECTIVE)
                {
                    return (CampBaseClass)e;
                }

                e = list.GetNext();
            }

            return null;
        }

        public static int Parent(CampBaseClass e)
        { throw new NotImplementedException(); }

        public static int Real(ClassTypes type)
        {
            if (type == ClassTypes.TYPE_BATTALION || type == ClassTypes.TYPE_FLIGHT || type == ClassTypes.TYPE_TASKFORCE)
                return 1;

            return 0;
        }

        public static Classes GetEntityClass(VuEntity e)
        {
            if (e == null)
                return 0;

            return (Classes)(e.EntityType()).classInfo_[(int)Vu_CLASS.VU_CLASS];
        }

        public static short GetEntityDomain(VuEntity e)
        {
            if (e == null)
                return 0;

            return (e.EntityType()).classInfo_[(int)Vu_CLASS.VU_DOMAIN];
        }

        public static CampBaseClass /* TODO was Unit */ GetEntityUnit(VuEntity e)
        {
            if (GetEntityClass(e) == Classes.CLASS_UNIT)
                return (CampBaseClass /* TODO was Unit */ )e;

            return null;
        }

#if TODO
        public static Objective GetEntityObjective(VuEntity h)
        {
    if (GetEntityClass(e) == CLASS_OBJECTIVE)
        return (Objective)e;

    return null;
}
#endif
        // My global for last assigned id
        static short gLastId = 32767;

        public static short FindUniqueID()
        {
            CampBaseClass e;
            short id, eid;

            if (gLastId < Camplib.MAX_CAMP_ENTITIES - 1)
            {
                // simple algorythm to find a unique id
                gLastId++;
                id = gLastId;
                return id;
            }
            else
            {
                // more complex algorythm if we're out of space
                short highest = 0;
                for (int i = 0; i < Camplib.MAX_CAMP_ENTITIES; i++)
                    CampSearch[i] = 0;

                VuListIterator myit = new VuListIterator(CampListStatic.AllCampList);
                e = (CampBaseClass)myit.GetFirst();

                while (e != null)
                {
                    eid = e.GetCampID();
                    Debug.Assert(eid < Camplib.MAX_CAMP_ENTITIES);
                    CampSearch[eid] = 1;

                    if (e.GetCampID() > highest)
                    {
                        gLastId = e.GetCampID();
                        highest = gLastId;
                    }

                    e = (CampBaseClass)myit.GetNext();
                }


                for (id = 1; id < Camplib.MAX_CAMP_ENTITIES; id++)
                {
                    if (CampSearch[id] == 0)
                        return id;
                }

                log.Error("Error! Exceeded max entity count!\n");
            }

            return 0;

        }
        static int tod = 0;

        public static int GetVisualDetectionRange(int mt)
        {
#if TODO
		            int dr;

            dr = CampaignStatic.VisualDetectionRange[mt];

            //Cobra Expensive TOD check, we will limit how often it gets checked
            VU_TIME timer = 0;


            if ((timer == 0) || (SimLibStatic.SimLibElapsedTime > timer))
            {
                tod = TimeOfDayGeneral();
                timer = SimLibStatic.SimLibElapsedTime + 900000;//15 minutes
            }

            if (tod == 1)
            {
                dr = (dr + 3) / 4;
            }
            else if (tod == 2)
            {
                dr = (dr + 1) / 2;
            }

            return dr;

            //end cobra
            //Cobra below is the old code
            /*switch (TimeOfDayGeneral()) // Time of day modifiers
             {
             case TOD_NIGHT:
             dr = (dr+3)/4;
             break;
             case TOD_DAWNDUSK:
             dr = (dr+1)/2;
             break;
             default:
             break;
             }*/
            //return dr;
 
#endif
            throw new NotImplementedException();
        }

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    }
}
