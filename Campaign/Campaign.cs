using System;
using System.IO;
using FalconNet.FalcLib;

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

        public static F4CSECTIONHANDLE* campCritical;
        public static int[] VisualDetectionRange = new int[OtherDam];
        public static byte[] DefaultDamageMods = new byte[OtherDam + 1];

        // ================
        // Defines & macros
        // ================

        public const int InfiniteCost = 32000;

        // TODO #define CampEnterCriticalSection()	F4EnterCriticalSection(campCritical)
        // TODO #define CampLeaveCriticalSection()	F4LeaveCriticalSection(campCritical)

        // ======================
        // public static al functions
        // ======================

        public static Objective AddObjectiveToCampaign(GridIndex I, GridIndex J);

        public static void RemoveObjective(Objective O);

        public static int LoadTheater(string filename);

        public static int SaveTheater(string filename);

        public static int LinkCampaignObjectives(Path p, Objective O1, Objective O2);

        public static int UnLinkCampaignObjectives(Objective O1, Objective O2);

        public static int RecalculateLinks(Objective o);

        public static UnitClass GetUnitByXY(GridIndex I, GridIndex J);

        public static UnitClass AddUnit(GridIndex I, GridIndex J, char Side);

        public static UnitClass CreateUnit(Control who, int Domain, UnitType Type, uchar SType, uchar SPType, Unit Parent);

        public static void RemoveUnit(UnitClass u);

        public static int TimeOfDayGeneral(CampaignTime time);

        public static int TimeOfDayGeneral();

        public static CampaignTime TimeOfDay();

        public static int CreateCampFile(string filename, string path);

        public static FileStream OpenCampFile(string filename, string ext, string mode);
        public static void CloseCampFile(FileStream fs);

        public static void StartReadCampFile(FalconGameType type, string filename);
        public static string ReadCampFile(string filename, string ext);
        public static void EndReadCampFile();

        public static void StartWriteCampFile(FalconGameType type, string filename);
        public static void WriteCampFile(string filename, string ext, string data, int size);
        public static void EndWriteCampFile();

        // Bubble rebuilding stuff
        public static void CampaignRequestSleep();
        public static int CampaignAllAsleep();

    }
}

