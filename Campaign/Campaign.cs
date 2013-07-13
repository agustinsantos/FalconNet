using System;
using System.IO;
using FalconNet.FalcLib;
using FalconNet.Common;
using Objective = FalconNet.Campaign.ObjectiveClass;
using Path = FalconNet.Campaign.BasePathClass;
using GridIndex = System.Int16;
using Control = System.Byte;
using System.Diagnostics;
using FalconNet.CampaignBase;

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

		private const int MAX_WCH_FILES = 4;
		private static int next_wch_file = 0;
		private static FileStream[] wch_fp = new FileStream[MAX_WCH_FILES];
		private static string[] wch_filename = new string[MAX_WCH_FILES];
		public const int InfiniteCost = 32000;

		// TODO #define CampEnterCriticalSection()	F4EnterCriticalSection(campCritical)
		// TODO #define CampLeaveCriticalSection()	F4LeaveCriticalSection(campCritical)

		// ======================
		// public static al functions
		// ======================

		public static Objective AddObjectiveToCampaign (GridIndex x, GridIndex y)
		{
			Objective o;

			o = ObjectivStatic.NewObjective ();
			o.SetLocation (x, y);
			o.UpdateObjectiveLists ();
			CampListStatic.RebuildObjectiveLists ();
			CampListStatic.RebuildParentsList ();
			return o;
		}

		public static void RemoveObjective (Objective O)
		{
			throw new NotImplementedException ();
		}

		public static bool LoadTheater (string theater)
		{
			// This assumes the Class Table was loaded elsewhere
			CampEnterCriticalSection ();
			if (!CampTerrStatic.LoadTheaterTerrain (theater)) {
				Debug.WriteLine ("Failed to open theater: {0}, using default theater.", theater);
				CampTerrStatic.Map_Max_X = CampTerrStatic.Map_Max_Y = 500;
				CampTerrStatic.InitTheaterTerrain ();
				CampLeaveCriticalSection ();
				return false;
			}
			Name.LoadNames (theater);
			CampLeaveCriticalSection ();
			return true;
		}

		public static int SaveTheater (string filename)
		{
#if TODO
            CampEnterCriticalSection();
            SaveTheaterTerrain(theater);
            SaveNames(theater);
            CampLeaveCriticalSection();
            return 1;
#endif
			throw new NotImplementedException ();
		}

		public static int LinkCampaignObjectives (Path p, Objective O1, Objective O2)
		{
#if TODO
            int i = 0, cost = 0, found = 0;
            byte[] costs = new byte[MOVEMENT_TYPES]; // TODO={254};
            GridIndex x = 0, y = 0;

            for (i = 0; i < MOVEMENT_TYPES; i++)
            {
                if (FindLinkPath(path, O1, O2, (MoveType)i) < 1)
                {
                    if (FindLinkPath(path, O2, O1, (MoveType)i) < 1)
                        costs[i] = 255;
                    else
                    {
                        O2.GetLocation(&x, &y);
                        cost = (FloatToInt32(path.GetCost()) + cost) / 2;
                        if (cost > 254)
                            cost = 254;			// If it's possible to move, but very expensive, mark it as our max movable cost (254)
                        costs[i] = (byte)cost;
                        found = 1;
                    }
                }
                else
                {
                    O1.GetLocation(&x, &y);
                    cost = FloatToInt32(path.GetCost());
                    if (cost > 254)
                        cost = 254;				// If it's possible to move, but very expensive, mark it as our max movable cost (254)
                    costs[i] = (byte)cost;
                    found = 1;
                }
                // KCK Hack: If we can get there by road, fool it into thinking it's a 254 cost link for
                // any otherwise unfound paths.
                if (costs[i] == 255 && (i == Foot || i == Wheeled || i == Tracked) && costs[NoMove] < 255)
                    costs[i] = 254;
            }
            if (found)
            {
                O1.AddObjectiveNeighbor(O2, costs);
                O2.AddObjectiveNeighbor(O1, costs);
                return 1;
            }
            else
                return 0;
#endif
			throw new NotImplementedException ();
		}

		public static int UnLinkCampaignObjectives (Objective O1, Objective O2)
		{
#if TODO
            int i, unlinked = 0;

            for (i = 0; i < O1.NumLinks(); i++)
            {
                if (O1.GetNeighborId(i) == O2.Id())
                {
                    O1.RemoveObjectiveNeighbor(i);
                    unlinked = 1;
                }
            }
            for (i = 0; i < O2.NumLinks(); i++)
            {
                if (O2.GetNeighborId(i) == O1.Id())
                {
                    O2.RemoveObjectiveNeighbor(i);
                    unlinked = 1;
                }
            }
            return unlinked;
#endif
			throw new NotImplementedException ();
		}

		public static int RecalculateLinks (Objective o)
		{
#if TODO
            PathClass path;
            Objective n;
            char nn;

            for (nn = 0; nn < o.NumLinks(); nn++)
            {
                n = o.GetNeighbor(nn);
                if (n)
                    LinkCampaignObjectives(&path, o, n);
            }
            return 1;
#endif
			throw new NotImplementedException ();
		}

		public static UnitClass GetUnitByXY (GridIndex I, GridIndex J)
		{
			throw new NotImplementedException ();
		}

		public static UnitClass AddUnit (GridIndex I, GridIndex J, char Side)
		{
#if TODO
            Unit nu;

            nu = NewUnit(DOMAIN_LAND, TYPE_BRIGADE, STYPE_UNIT_ARMOR, 1, null);
            nu.SetOwner(Side);
            nu.ResetLocations(x, y);
            nu.ResetDestinations(x, y);
            nu.SetUnitOrders(GORD_DEFEND);
            return nu;
#endif
			throw new NotImplementedException ();
		}

		public static UnitClass CreateUnit (Control who, int Domain, UnitType Type, byte SType, byte SPType, Unit Parent)
		{
			throw new NotImplementedException ();
		}

		public static void RemoveUnit (UnitClass u)
		{
			u.Remove ();
		}

		public static int TimeOfDayGeneral (CampaignTime time)
		{
#if TODO
            // 2001-04-10 MODIFIED BY S.G. SO IT USES MILISECOND AND NOT MINUTES! I COULD CHANGE THE .H FILE BUT IT WOULD TAKE TOO LONG TO RECOMPILE :-(
            /*	if (time < TOD_SUNUP)
                    return TOD_NIGHT;
                else if (time < TOD_SUNUP + 120*CampaignMinutes)
                    return TOD_DAWNDUSK;
                else if (time < TOD_SUNDOWN - 120*CampaignMinutes)
                    return TOD_DAY;
                else if (time < TOD_SUNDOWN)
                    return TOD_DAWNDUSK;
                else
                    return TOD_NIGHT;
            */
            // time can be over one day so I need to keep just the time in the current day...
            time = time % CampaignTime.CampaignDay;

            if (time < TOD_SUNUP * CampaignMinutes)
                return TOD_NIGHT;
            else if (time < TOD_SUNUP * CampaignMinutes + 120 * CampaignMinutes)
                return TOD_DAWNDUSK;
            else if (time < TOD_SUNDOWN * CampaignMinutes - 120 * CampaignMinutes)
                return TOD_DAY;
            else if (time < TOD_SUNDOWN * CampaignMinutes)
                return TOD_DAWNDUSK;
            else
                return TOD_NIGHT;
#endif
			throw new NotImplementedException ();
		}

		public static int TimeOfDayGeneral ()
		{
#if TODO
            return TimeOfDayGeneral(TheCampaign.TimeOfDay);
#endif
			throw new NotImplementedException ();
		}

		public static CampaignTime TimeOfDay ()
		{
#if TODO
            return TheCampaign.TimeOfDay;
#endif
			throw new NotImplementedException ();
		}


		// Bubble rebuilding stuff
		public static void CampaignRequestSleep ()
		{
			throw new NotImplementedException ();
		}

		public static int CampaignAllAsleep ()
		{
			throw new NotImplementedException ();
		}

		private static void ResizeBubble (int moversInBubble)
		{
#if TODO
            float br, new_ratio;

            br = (float)(PLAYER_BUBBLE_MOVERS - moversInBubble);
            new_ratio = ((FalconSessionEntity*)vuLocalSessionEntity).GetBubbleRatio() * 1.0F + (br * 0.01F);

            if (new_ratio > 1.2F)
            {
                new_ratio = 1.2F;
            }

            if (new_ratio < 0.25F)
            {
                new_ratio = 0.25F;
            }

            ((FalconSessionEntity*)vuLocalSessionEntity).SetBubbleRatio(new_ratio);
#endif
			throw new NotImplementedException ();
		}

		private static void CampEnterCriticalSection ()
		{
			throw new NotImplementedException ();
		}

		private static void CampLeaveCriticalSection ()
		{
			throw new NotImplementedException ();
		}

		private static void GetCampFilePath (FalconGameType type, string filename, out string path)
		{
			if (type == FalconGameType.game_TacticalEngagement) {
				path = F4Find.FalconCampUserSaveDirectory + System.IO.Path.DirectorySeparatorChar + filename + ".tac";
				if (!File.Exists (path))
					path = F4Find.FalconCampUserSaveDirectory + System.IO.Path.DirectorySeparatorChar + filename + ".trn";
			} else {
				path = F4Find.FalconCampUserSaveDirectory + System.IO.Path.DirectorySeparatorChar + filename + ".cam";
			}
		}

		private static bool IsCampFile (FalconGameType type, string filename)
		{
			string path;

			GetCampFilePath (type, filename, out path);
			return File.Exists (path);
		}
	}
}

