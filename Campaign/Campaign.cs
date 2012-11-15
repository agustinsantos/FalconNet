using System;
using System.IO;
using FalconNet.FalcLib;
using FalconNet.Common;
using Objective=FalconNet.Campaign.ObjectiveClass;
using Path=FalconNet.Campaign.BasePathClass;

namespace FalconNet.Campaign
{
    public static class CampaignStatic
    {
        // ============
        // Map Deltas
        // ============

        public static GridIndex[] dx = new GridIndex[17];     // dx per direction
        public static GridIndex[] dy = new GridIndex[17];     // dy per direction

        // ============
        // Placeholders
        // ============
        //TODO typedef UnitClass* Unit;
        //TODOtypedef ObjectiveClass* Objective;

        // =====================
        // Campaign wide globals
        // =====================

        public static F4CSECTIONHANDLE campCritical;
        public static int[] VisualDetectionRange = new int[(int)DamageDataType.OtherDam];
        public static byte[] DefaultDamageMods = new byte[(int)DamageDataType.OtherDam + 1];

        // ================
        // Defines & macros
        // ================

        public const int InfiniteCost = 32000;

        // TODO #define CampEnterCriticalSection()	F4EnterCriticalSection(campCritical)
        // TODO #define CampLeaveCriticalSection()	F4LeaveCriticalSection(campCritical)

        // ======================
        // public static al functions
        // ======================

        public static Objective AddObjectiveToCampaign(GridIndex I, GridIndex J)
		{throw new NotImplementedException();}

        public static void RemoveObjective(Objective O)
		{throw new NotImplementedException();}

        public static int LoadTheater(string filename)
		{throw new NotImplementedException();}

        public static int SaveTheater(string filename)
		{throw new NotImplementedException();}

        public static int LinkCampaignObjectives(Path p, Objective O1, Objective O2)
		{throw new NotImplementedException();}

        public static int UnLinkCampaignObjectives(Objective O1, Objective O2)
		{throw new NotImplementedException();}

        public static int RecalculateLinks(Objective o)
		{throw new NotImplementedException();}

        public static UnitClass GetUnitByXY(GridIndex I, GridIndex J)
		{throw new NotImplementedException();}

        public static UnitClass AddUnit(GridIndex I, GridIndex J, char Side)
		{throw new NotImplementedException();}

        public static UnitClass CreateUnit(Control who, int Domain, UnitType Type, byte SType, byte SPType, Unit Parent)
		{throw new NotImplementedException();}

        public static void RemoveUnit(UnitClass u)
		{throw new NotImplementedException();}

        public static int TimeOfDayGeneral(CampaignTime time)
		{throw new NotImplementedException();}

        public static int TimeOfDayGeneral()
		{throw new NotImplementedException();}

        public static CampaignTime TimeOfDay()
		{throw new NotImplementedException();}

        public static int CreateCampFile(string filename, string path)
		{throw new NotImplementedException();}

        public static FileStream OpenCampFile(string filename, string ext, string mode)
		{throw new NotImplementedException();}
        public static void CloseCampFile(FileStream fs)
		{throw new NotImplementedException();}

        public static void StartReadCampFile(FalconGameType type, string filename)
		{throw new NotImplementedException();}
        public static string ReadCampFile(string filename, string ext)
		{throw new NotImplementedException();}
        public static void EndReadCampFile()
		{throw new NotImplementedException();}

        public static void StartWriteCampFile(FalconGameType type, string filename)
		{throw new NotImplementedException();}
        public static void WriteCampFile(string filename, string ext, string data, int size)
		{throw new NotImplementedException();}
        public static void EndWriteCampFile()
		{throw new NotImplementedException();}

        // Bubble rebuilding stuff
        public static void CampaignRequestSleep()
		{throw new NotImplementedException();}
        public static int CampaignAllAsleep()
		{throw new NotImplementedException();}

    }
}

