using FalconNet.Campaign;
using FalconNet.Common;
using FalconNet.Ui95;
using FalconNet.UI;
using System;
using FalconNet.UIExt;
using COLORREF = System.Int32;
using FalconNet.VU;
using C_Window = FalconNet.UI.C_Window;

namespace FalconNet.UICampaign
{
    public enum DRAWFLAGS // Draw Flags
    {
        _MAP_TEAM_0 = 0x00000001, // Calculation= (1 << i) (where i>=0 && i < _MAX_TEAMS_)
        _MAP_TEAM_1 = 0x00000002,
        _MAP_TEAM_2 = 0x00000004,
        _MAP_TEAM_3 = 0x00000008,
        _MAP_TEAM_4 = 0x00000010,
        _MAP_TEAM_5 = 0x00000020,
        _MAP_TEAM_6 = 0x00000040,
        _MAP_TEAM_7 = 0x00000080,
        _MAP_OBJECTIVES_ = 0x01000000, // Objectives on/off bit
        _MAP_UNITS_ = 0x02000000, // Units on/off bit
        _MAP_AIR_UNITS_ = 0x04000000, // Air Units on/off bit
        _MAP_NAVAL_UNITS_ = 0x08000000, // Naval Units on/off
        _MAP_WAYPOINTS_ = 0x10000000,
        _MAP_THREATS_ = 0x20000000,
        _MAP_BULLSEYE_ = 0x40000000, // Show Bullseye
    }

    public struct THREAT_LIST
    {
        public THREAT_CIRCLE SamLow;
        public THREAT_CIRCLE SamHigh;
        public THREAT_CIRCLE RadarLow;
        public THREAT_CIRCLE RadarHigh;
    }

    public class THR_LIST
    { // This class is NEVER actually added to a window... used to make an overlay
        public long[] Flags = new long[filters._MAP_NUM_THREAT_TYPES_];
        public C_Threat[] Type = new C_Threat[filters._MAP_NUM_THREAT_TYPES_];
    }

    public class OBJ_LIST
    {
        public long[] Flags = new long[filters._MAP_NUM_OBJ_TYPES_];
        public C_MapIcon[] Type = new C_MapIcon[filters._MAP_NUM_OBJ_TYPES_];
    }

    public class AIR_LIST
    {
        public long[] Flags = new long[filters._MAP_NUM_AIR_TYPES_];
        public C_MapIcon[] Type = new C_MapIcon[filters._MAP_NUM_AIR_TYPES_];
    }

    public class GND_SIZE
    {
        public long[] Flags = new long[filters._MAP_NUM_GND_LEVELS_];
        public C_MapIcon[] Levels = new C_MapIcon[filters._MAP_NUM_GND_LEVELS_];
    }

    public class GND_LIST
    {
        public long[] Flags = new long[filters._MAP_NUM_GND_TYPES_];
        public GND_SIZE[] Type = new GND_SIZE[filters._MAP_NUM_GND_TYPES_];
    }

    public class NAV_LIST
    {
        public long[] Flags = new long[filters._MAP_NUM_NAV_TYPES_];
        public C_MapIcon[] Type = new C_MapIcon[filters._MAP_NUM_NAV_TYPES_];
    }

    public struct MAPICONS
    {
        public THR_LIST Threats; // These are used to create an overlay
        public AIR_LIST AirUnits;
        public GND_LIST Units;
        public NAV_LIST NavalUnits;
        public OBJ_LIST Objectives;
        public C_Waypoint Waypoints;
    }

    //TODO class ObjectiveClass; typedef ObjectiveClass *Objective;
    //TODO class UnitClass; typedef UnitClass *Unit;
    //TODO class FlightClass; typedef FlightClass *Flight;

    public class C_Map
    {

        private long[, ,] AirIconIDs_ = new long[filters._MAX_TEAMS_, filters._MAX_DIRECTIONS_, 2]; // [Team 0-7][Heading][0 = Not Selected,1 = Selected]
        private long[,] ArmyIconIDs_ = new long[filters._MAX_TEAMS_, 2]; // [Team 0-7][0 = Not Selected,1 = Selected]
        private long[,] NavyIconIDs_ = new long[filters._MAX_TEAMS_, 2]; // [Team 0-7][0 = Not Selected,1 = Selected]
        private long[,] ObjIconIDs_ = new long[filters._MAX_TEAMS_, 2]; // [Team 0-7][0 = Not Selected,1 = Selected]
        private float CenterX_, CenterY_;
        private long ZoomLevel_;
        private long MinZoomLevel_;
        private long MaxZoomLevel_;
        private long ZoomStep_;
        private float scale_; // calculated
        private float maxy; // calculated... used for converting to my coordinates
        private long flags_;
        private short Circles_;

        private float BullsEyeX_, BullsEyeY_;

        private float LogMinX_, LogMinY_, LogMaxX_, LogMaxY_; // Min/Max ranges for WaypointZs
        private float StrtMinX_, StrtMinY_, StrtMaxX_, StrtMaxY_; // Min/Max ranges for WaypointZs

        private long ObjectiveMask_; // masks for displaying map icons
        private long UnitMask_;
        private long NavalUnitMask_;
        private long AirUnitMask_;
        private long ThreatMask_;

        private long MapID;
        private C_ScaleBitmap Map_; // 1536x2048 16 bit map
        private UI95_RECT MapRect_;

        private long[] TeamFlags_ = new long[filters._MAX_TEAMS_];
        private MAPICONS[] Team_ = new MAPICONS[filters._MAX_TEAMS_];
        private COLORREF[] TeamColor_ = new COLORREF[filters._MAX_TEAMS_];

        private VU_ID WPUnitID_;  // Flight ID of current waypoints
        private C_Waypoint CurWP_; // Currently selected WP list
        private C_Waypoint CurWPZ_; // Currently selected WP list (altitudes only)
        private RECT CurWPArea_; // Needs to be RECT not UI95_RECT

        private C_DrawList CurIcons_; // current icons for targets & airbases (will always be on when CurWP_ is displayed)

        private C_Cursor SmallMapCtrl_; // Keeps small map up to date :)
        private C_Window DrawWindow_, WPZWindow_;
        private UI95_RECT DrawRect_;

        private C_BullsEye BullsEye_;

        private VU_ID CurFlight_;

        private void CalculateDrawingParams()
        { throw new NotImplementedException(); }
        private void BuildWPList(C_Waypoint wplist, C_Waypoint wpzlist, UnitClass unit)
        { throw new NotImplementedException(); }
        private void BuildCurrentWPList(UnitClass unit)
        { throw new NotImplementedException(); }

        private void AddListsToWindow()
        { throw new NotImplementedException(); }
        private void RemoveListsFromWindow()
        { throw new NotImplementedException(); }
        private void SetTeamScales()
        { throw new NotImplementedException(); }
        private void ScaleMap()
        { throw new NotImplementedException(); }


        public C_Map()
        { throw new NotImplementedException(); }
        // TODO public ~C_Map();

        public void SetLogRanges(float minx, float miny, float maxx, float maxy) { LogMinX_ = minx; LogMinY_ = miny; LogMaxX_ = maxx; LogMaxY_ = maxy; } // Min/Max ranges for WaypointZs
        public void SetStrtRanges(float minx, float miny, float maxx, float maxy) { StrtMinX_ = minx; StrtMinY_ = miny; StrtMaxX_ = maxx; StrtMaxY_ = maxy; } // Min/Max ranges for WaypointZs
        public void SetAirIcons(long TeamNo, long Dir, long OffID, long OnID) { AirIconIDs_[TeamNo & 7, Dir & 7, 0] = OffID; AirIconIDs_[TeamNo & 7, Dir & 7, 1] = OnID; }
        public void SetArmyIcons(long TeamNo, long OffID, long OnID) { ArmyIconIDs_[TeamNo & 7, 0] = OffID; ArmyIconIDs_[TeamNo & 7, 1] = OnID; }
        public void SetNavyIcons(long TeamNo, long OffID, long OnID) { NavyIconIDs_[TeamNo & 7, 0] = OffID; NavyIconIDs_[TeamNo & 7, 1] = OnID; }
        public void SetObjectiveIcons(long TeamNo, long OffID, long OnID) { ObjIconIDs_[TeamNo & 7, 0] = OffID; ObjIconIDs_[TeamNo & 7, 1] = OnID; }

        public MAPICONS GetTeam(int i) { return Team_[i]; } // 2002-02-23 ADDED BY S.G. Need to exteriorize Team_
        public void SetupOverlay()
        { throw new NotImplementedException(); }
        public void Cleanup()
        { throw new NotImplementedException(); }
        public void SetMapImage(long ID)
        { throw new NotImplementedException(); }
        public C_Base GetMapControl() { return (Map_); }
        public void SetWindow(C_Window win)
        { throw new NotImplementedException(); }
        public C_Window GetWindow() { return (DrawWindow_); }
        public C_Window GetZWindow() { return (WPZWindow_); }
        public C_Waypoint GetCurWP() { return (CurWP_); }
        public C_Waypoint GetCurWPZ() { return (CurWPZ_); }
        public VU_ID GetCurWPID() { return (WPUnitID_); }
        public void SetWPZWindow(C_Window win)
        { throw new NotImplementedException(); }
        public void SetTeamFlags(long TeamID, long flags) { if (TeamID >= 0 && TeamID < filters._MAX_TEAMS_) TeamFlags_[TeamID] = flags; }
        public long GetTeamFlags(long TeamID) { if (TeamID >= 0 && TeamID < filters._MAX_TEAMS_) return (TeamFlags_[TeamID]); return (0); }
        public void SetTeamColor(long TeamID, COLORREF color) { if (TeamID >= 0 && TeamID < filters._MAX_TEAMS_) TeamColor_[TeamID] = color; }
        public COLORREF GetTeamColor(long TeamID) { if (TeamID >= 0 && TeamID < filters._MAX_TEAMS_) return (TeamColor_[TeamID]); return (0); }
        public void FitFlightPlan()
        { throw new NotImplementedException(); }
        public void SetZoomLevel(short zoom)
        { throw new NotImplementedException(); }
        public void ZoomIn()
        { throw new NotImplementedException(); }
        public void ZoomOut()
        { throw new NotImplementedException(); }
        public void SetFlags(long flag) { flags_ |= flag; }
        public void SetFlight(VU_ID ID) { CurFlight_ = ID; }
        public void SetBullsEye(float x, float y)
        { throw new NotImplementedException(); }
        public long GetZoomLevel() { return (ZoomLevel_); }
        public void SetUnitLevel(long level)
        { throw new NotImplementedException(); }
        public void ShowObjectiveType(long mask)
        { throw new NotImplementedException(); }
        public void HideObjectiveType(long mask)
        { throw new NotImplementedException(); }
        public void ShowUnitType(long mask)
        { throw new NotImplementedException(); }
        public void HideUnitType(long mask)
        { throw new NotImplementedException(); }
        public void ShowAirUnitType(long mask)
        { throw new NotImplementedException(); }
        public void HideAirUnitType(long mask)
        { throw new NotImplementedException(); }
        public void RefreshAllAirUnitType()
        { throw new NotImplementedException(); } // 2002-02-21 ADDED BY S.G.
        public void ShowNavalUnitType(long mask)
        { throw new NotImplementedException(); }
        public void HideNavalUnitType(long mask)
        { throw new NotImplementedException(); }
        public void ShowThreatType(long mask)
        { throw new NotImplementedException(); }
        public void HideThreatType(long mask)
        { throw new NotImplementedException(); }
        public void SetMapCenter(long x, long y)
        { throw new NotImplementedException(); }
        public void MoveCenter(long x, long y)
        { throw new NotImplementedException(); }
        public long GetMapCenterX() { return ((int)(CenterX_)); }
        public long GetMapCenterY() { return ((int)(CenterY_)); }
        public void SetSmallMap(C_Cursor smap) { SmallMapCtrl_ = smap; }
        public float GetMaxY() { return (maxy); }
        public float GetMapScale() { return (scale_); }
        public bool SetWaypointList(VU_ID UnitID)
        { throw new NotImplementedException(); }
        public bool SetCurrentWaypointList(VU_ID UnitID)
        { throw new NotImplementedException(); }
        public void RemoveCurWPList()
        { throw new NotImplementedException(); }
        public void RemoveAllWaypoints(short team)
        { throw new NotImplementedException(); }
        public void RemoveWaypoints(short team, long group)
        { throw new NotImplementedException(); }
        public void RemoveAllEntities()
        { throw new NotImplementedException(); }
        public void RemoveFromCurIcons(long ID)
        { throw new NotImplementedException(); }
        public void AddToCurIcons(MAPICONLIST MapItem)
        { throw new NotImplementedException(); }
        public void CenterOnIcon(MAPICONLIST MapItem)
        { throw new NotImplementedException(); }
        public THREAT_LIST AddThreat(CampEntity ent)
        { throw new NotImplementedException(); }
        public MAPICONLIST AddObjective(ObjectiveClass Obj)
        { throw new NotImplementedException(); }
        public MAPICONLIST AddDivision(DivisionClass div)
        { throw new NotImplementedException(); }
        public MAPICONLIST AddUnit(UnitClass u)
        { throw new NotImplementedException(); }
        public MAPICONLIST AddFlight(FlightClass flight)
        { throw new NotImplementedException(); }
        public MAPICONLIST AddSquadron(SquadronClass squadron)
        { throw new NotImplementedException(); }
        public MAPICONLIST AddPackage(PackageClass package)
        { throw new NotImplementedException(); }
        public MAPICONLIST AddVC(victory_condition vc)
        { throw new NotImplementedException(); }
        public void UpdateWaypoint(FlightClass flt)
        { throw new NotImplementedException(); }
        public void UpdateVC(victory_condition vc)
        { throw new NotImplementedException(); }
        public void RemoveVC(long team, long ID)
        { throw new NotImplementedException(); }
        public void RemoveOldWaypoints()
        { throw new NotImplementedException(); }
        public void TurnOnNames()
        { throw new NotImplementedException(); }
        public void TurnOnBoundaries()
        { throw new NotImplementedException(); }
        public void TurnOnArrows()
        { throw new NotImplementedException(); }
        public void TurnOffNames()
        { throw new NotImplementedException(); }
        public void TurnOffBoundaries()
        { throw new NotImplementedException(); }
        public void TurnOffArrows()
        { throw new NotImplementedException(); }
        public void TurnOnBullseye()
        { throw new NotImplementedException(); }
        public void TurnOffBullseye()
        { throw new NotImplementedException(); }
        public void DrawMap()
        { throw new NotImplementedException(); }
#if TODO
		public void SetAllObjCallbacks(void (*cb)(long,short,C_Base*));
		public void SetAllAirUnitCallbacks(void (*cb)(long,short,C_Base*));
		public void SetAllGroundUnitCallbacks(void (*cb)(long,short,C_Base*));
		public void SetAllNavalUnitCallbacks(void (*cb)(long,short,C_Base*));
		public void SetObjCallbacks(long type,void (*cb)(long,short,C_Base*));
		public void SetAirUnitCallbacks(long type,void (*cb)(long,short,C_Base*));
		public void SetGroundUnitCallbacks(long level,long type,void (*cb)(long,short,C_Base*));
		public void SetNavalUnitCallbacks(long type,void (*cb)(long,short,C_Base*));
		public void SetUnitCallbacks(long level,long type,void (*cb)(long,short,C_Base*));
#endif
        public C_MapIcon GetObjIconList(long team, long type)
        { throw new NotImplementedException(); }
        public void RecalcWaypointZs(long scaletype)
        { throw new NotImplementedException(); } // 1=Log, 2=straight
        public void GetMapRelativeXY(ref short x, ref short y)
        {
#if TODO
            if (DrawWindow_ != null)
            {
                x = (short)(x - DrawWindow_.GetX() - DrawWindow_.VX_[0]);
                y = (short)(y - DrawWindow_.GetY() - DrawWindow_.VY_[0]);
            }
#endif
            throw new NotImplementedException();
        }

        void RemapTeamColors(long team)
        { throw new NotImplementedException(); }
    }
}
