using System;
using System.Collections.Generic;
using System.Diagnostics;
using FalconNet.Common;
using FalconNet.FalcLib;
using FalconNet.UI;
using FalconNet.VU;
using Flight = FalconNet.Campaign.FlightClass;
using Objective = FalconNet.Campaign.ObjectiveClass;
using WayPoint=FalconNet.Campaign.WayPointClass;
using Team=System.Int32;
using GridIndex = System.Int16;
namespace FalconNet.Campaign
{
	// Flight specific status flags
	[Flags]
 	public enum FlightStatus
	{
		MISEVAL_FLIGHT_LOSSES			=0x00000001,
		MISEVAL_FLIGHT_DESTROYED		=0x00000002,
		MISEVAL_FLIGHT_ABORTED			=0x00000004,
		MISEVAL_FLIGHT_GOT_AKILL		=0x00000010,		// Air kill
		MISEVAL_FLIGHT_GOT_GKILL		=0x00000020,		// Ground kill
		MISEVAL_FLIGHT_GOT_NKILL		=0x00000040,		// Naval kill
		MISEVAL_FLIGHT_GOT_SKILL		=0x00000080,		// Static kill
		MISEVAL_FLIGHT_HIT_HIGH_VAL		=0x00000100,		// Hit a high value target (Feature with value)
		MISEVAL_FLIGHT_HIT_BY_AIR		=0x00001000,		// Suffered loss to air (during ingress only!)
		MISEVAL_FLIGHT_HIT_BY_GROUND	=0x00002000,		// Suffered loss to ground (during ingress only!)
		MISEVAL_FLIGHT_HIT_BY_NAVAL		=0x00004000,		// Suffered loss to naval (during ingress only!)
		MISEVAL_FLIGHT_TARGET_HIT		=0x00010000,		// We hit our target
		MISEVAL_FLIGHT_TARGET_KILLED	=0x00020000,		// We killed out target
		MISEVAL_FLIGHT_TARGET_ABORTED	=0x00040000,		// We forced our target to abort
		MISEVAL_FLIGHT_AREA_HIT			=0x00080000,		// We hit an enemy in our target area
		MISEVAL_FLIGHT_F_TARGET_HIT		=0x00100000,		// The friendly we were assigned to was hit
		MISEVAL_FLIGHT_F_TARGET_KILLED	=0x00200000,		// The friendly we were assigned to was killed
		MISEVAL_FLIGHT_F_TARGET_ABORTED	=0x00400000,		// The friendly we were assigned to aborted
		MISEVAL_FLIGHT_F_AREA_HIT		=0x00800000,		// Our friendly target region was hit
		MISEVAL_FLIGHT_STARTED_LATE		=0x01000000,		// This mission wasn't started in time to count full
		MISEVAL_FLIGHT_GOT_TO_TARGET	=0x02000000,		// Flight got to target area
		MISEVAL_FLIGHT_STATION_OVER		=0x04000000,		// Station/mission time is over
		MISEVAL_FLIGHT_GOT_HOME			=0x08000000,		// We returned to friendly territory
		MISEVAL_FLIGHT_RELIEVED			=0x10000000,		// Flight was allowed to leave by AWACS/FAC
		MISEVAL_FLIGHT_OFF_STATION		=0x20000000,		// Flight left its station area
	
		// 2002-02-13 MN 
		MISEVAL_FLIGHT_ABORT_BY_AWACS	=0x40000000		// flight aborted by AWACS instruction - we occupied the target
	}
	
	public class MissEvakStatic
	{
		// ====================================
		// Some defines (maybe move to AIInput)
		// ====================================

		public const int MAX_TARGET_FEATURES = 5;
		public const int  MAX_POTENTIAL_TARGETS = 5;
		public const int  MAX_COLLECTED_THREATS = 5;
		public const int  MAX_RELATED_EVENTS = 5;
		public const int  MINIMUM_VIABLE_THREAT = 9;
		// Kill tracking vs..
		public const int  VS_AI = 0;
		public const int  VS_HUMAN = 1;
		public const int  VS_EITHER = 2;
		
		/* TODO
	// Mission Evaluator status flags
	#define MISEVAL_MISSION_IN_PROGRESS		0x01			// We want to start evaluating stuff
	#define MISEVAL_EVALUATE_HITS_IN_AREA	0x02			// Check to see if someone has bombed our area
	#define MISEVAL_GAME_COMPLETED			0x04			// We finished a dogfight game
	#define MISEVAL_ONLINE_GAME				0x08			// This was a multi-player game
	
	// Pilot flags
	#define PFLAG_PLAYER_CONTROLLED			0x01			// Pilot is a player
	#define PFLAG_WON_GAME					0x02			// Pilot is on the winning side
*/
	}
		
	// ===================================
	// success enums
	// ===================================
	
	public enum SuccessType
	{
		Failed,
		PartFailed,
		PartSuccess,
		Success,
		Incomplete,
		AWACSAbort		// 2002-02-15 MN Added
	};
	
	public enum RatingType
	{
		Horrible,
		Poor,
		Average,
		Good,
		Excellent
	};
	
	// ===================================
	// Data Storage
	// ===================================
	
	public class WeaponDataClass
	{
		public string		weapon_name;
		public short		weapon_id;
		public short		starting_load;
		public byte			fired;
		public byte			missed;
		public byte			hit;
		public short		events;								// number of events
		public List<EventElement>	root_event;						// List of relevant events
		
		public WeaponDataClass ()
		{
			starting_load = 0;
			fired = 0;
			missed = 0;
			hit = 0;
			events = 0;	
			root_event = null;
			weapon_id = 0;
		}
		//TODO public ~WeaponDataClass();
	}
	
	public class PilotDataClass
	{	
		public string		pilot_name;
		public string		pilot_callsign;
		public byte			aircraft_slot;						// Which aircraft we're occupying
		public byte			pilot_slot;							// Pilot's position in the pilot_list
		public short		pilot_id;							// ID of the pre-assigned non-player pilot
		public byte			pilot_flags;
		public byte			pilot_status;
		public byte			aircraft_status;
		public byte			aa_kills;
		public byte			ag_kills;
		public byte			as_kills;
		public byte			an_kills;
		public byte			player_kills;
		public byte			shot_at;							// Times this player was shot at (only tracks for local player)
		public short[]		deaths = new short[MissEvakStatic.VS_EITHER];					// Dogfight statistics [AI/PLAYER]
		public short		score;
		public byte			rating;
		public byte			weapon_types;						// number of actually different weapons
		public bool			donefiledebrief;					//me123 has a file debrief already been made
		public WeaponDataClass[]	weapon_data = new WeaponDataClass[(CampWeapons.HARDPOINT_MAX / 2) + 2];	// Weapon data for this pilot/aircraft
		public PilotDataClass	next_pilot;
		
		public PilotDataClass ()
		{
			aircraft_slot = 0;
			pilot_slot = 0;
			pilot_id = 0;
			pilot_flags = 0;
			pilot_status = 0;
			aircraft_status = 0;
			aa_kills = ag_kills = as_kills = an_kills = 0;
			donefiledebrief = false;
			player_kills = 0;
			//	memset(kills,0,DogfightStatic.MAX_DOGFIGHT_TEAMS*VS_EITHER*sizeof(short));
			// memset(deaths,0,VS_EITHER*sizeof(short));
			shot_at = 0;
			score = 0;
			rating = 0;
			weapon_types = 0;
			next_pilot = null;
			pilot_callsign = "";
			pilot_name = "";
		}

		// TODO public ~PilotDataClass();
	}
	
	public class FlightDataClass
	{
		public string		name;
		public ulong		status_flags;
		public short		camp_id;
		public VU_ID		flight_id;
		public string		aircraft_name;
		public byte			start_aircraft;
		public byte			finish_aircraft;
		public Team			flight_team;
		public MissionTypeEnum			mission;
		public byte			old_mission;						// Old mission, if we were diverted
		public VU_ID		requester_id;						// ID of entity which caused this mission
		public VU_ID		target_id;
		public short		target_camp_id;
		public byte			target_building;
		public GridIndex	target_x;
		public GridIndex	target_y;
		public byte[]		target_features = new byte[MissEvakStatic.MAX_TARGET_FEATURES];
		public byte			target_status;
		public byte			mission_context;
		public byte			mission_success;
		public byte			failure_code;
		public short		failure_data;
		public VU_ID		failure_id;
		public byte			num_pilots;
		public string		context_entity_name;			// Name of the entity this mission was about
		public PilotDataClass	pilot_list;
		public short			events;								// number of events
		public List<EventElement>		root_event;						// List of relevant events
		public FlightDataClass	next_flight;
	
		public FlightDataClass ()
		{
			camp_id = 0;
            flight_id = target_id = VU_ID.FalconNullId;
            requester_id = VU_ID.FalconNullId;
			start_aircraft = finish_aircraft = 0;
			flight_team = 0;
			mission = 0;
			target_camp_id = 0;
			target_building = 0;
			//TODO target_x = target_y = null;
			status_flags = 0;
			mission_success = 0;
			mission_context = 0;
			failure_code = 0;
			failure_data = 0;
			num_pilots = 0;
			pilot_list = null;
			events = 0;
			root_event = null;
			next_flight = null;
			//memset(target_features,0,MAX_TARGET_FEATURES);
			target_status = 0;
			context_entity_name = "";
		}

		// public ~FlightDataClass();
	}
	
	public class MissionEvaluationClass
	{
		public ulong			contact_score;						// Total AA score of all actual contacts
		public byte				pack_success;						// Package success
		public byte				package_mission;
		public byte				package_context;
		public FlightDataClass	package_element;
		public FlightDataClass	player_element;
		public PilotDataClass	player_pilot;
		public byte				player_aircraft_slot;
		public byte				friendly_losses;
		public byte				friendly_aa_losses;
		public byte				friendly_ga_losses;
		public byte				action_type;
		public Team				team;
		public short			flags;
		public short			responses;
		public short[]			threat_ids = new short[MissEvakStatic.MAX_COLLECTED_THREATS];	// weapon ids of threats
		public GridIndex[]		threat_x = new GridIndex[MissEvakStatic.MAX_COLLECTED_THREATS];
		public GridIndex[]		threat_y = new GridIndex[MissEvakStatic.MAX_COLLECTED_THREATS];
		public VU_ID			alternate_strip_id;
		public VU_ID			requesting_ent;
		public VU_ID			intercepting_ent;
		public VU_ID			awacs_id;
		public VU_ID			jstar_id;
		public VU_ID			ecm_id;
		public VU_ID			tanker_id;
		public VU_ID			package_target_id;
		public VU_ID[]			potential_targets = new VU_ID[MissEvakStatic.MAX_POTENTIAL_TARGETS];
		public CampaignTime		assigned_tot;
		public CampaignTime		actual_tot;
		public CampaignTime		patrol_time;
		public CampaignTime		player_start_time;					// Time player started flying the mission
		public CampaignTime		player_end_time;					// Time player stopped flying the mission
		public GridIndex		tx, ty, abx, aby, awx, awy, jsx, jsy, tankx, tanky;
		public FlightDataClass	flight_data;
		public short			curr_data;							// Floating data point
		public byte				curr_flight;						// What flight we're talking about
		public byte				curr_weapon;						// What weapon we're talking about
		public PilotDataClass	curr_pilot;						// What pilot we're talking about
		public byte[]			parse_types = new byte[((int)FalconMsgID.LastFalconEvent + 7) / 8];	// messages types to parse
		public byte[]		 	rounds_won = new byte[DogfightStatic.MAX_DOGFIGHT_TEAMS];		// Dogfight rounds won
		public byte				last_related_event;
		public string[]			related_events = new string[MissEvakStatic.MAX_RELATED_EVENTS];
		public CAMP_MISS_STRUCT	logbook_data;						// Structure we'll pass to the logbook
		
		public MissionEvaluationClass ()
		{
			#if FUNKY_KEVIN_DEBUG_STUFF
				Debug.Assert(!inMission);
			#endif
			
			//TODO memset (this, 0, sizeof (MissionEvaluationClass));
			player_pilot = null;
			package_element = null;
			player_element = null;
			flags = 0;
			//TODO memset(rounds_won, 0, DogfightStatic.MAX_DOGFIGHT_TEAMS*sizeof(byte));
			for (int i=0; i<MissEvakStatic.MAX_RELATED_EVENTS; i++)
				related_events [i] = null;
		}

		// TODO public ~MissionEvaluationClass();
	
		public void CleanupFlightData ()
		{
			FlightDataClass flight_ptr = flight_data, next_ptr;
		
			while (flight_ptr != null) {
				next_ptr = flight_ptr.next_flight;
				flight_ptr = null;
				flight_ptr = next_ptr;
			}
			flight_data = null;
		}

		public void CleanupPilotData ()
		{
#if TODO
			int k;
			PilotDataClass pilot_data, soon_to_die;
			FlightDataClass flight_ptr = flight_data;
		
			while (flight_ptr != null) {
				pilot_data = flight_ptr.pilot_list;
				while (pilot_data != null) {
					soon_to_die = pilot_data;
					for (k=0; k<(CampWeapons.HARDPOINT_MAX/2)+2; k++) {
						DisposeEventList (pilot_data.weapon_data [k].root_event);
						pilot_data.weapon_data [k].root_event = null;
						pilot_data.weapon_data [k].events = 0;
					}
					pilot_data = pilot_data.next_pilot;
					soon_to_die = null;
				}
				flight_ptr.pilot_list = null;
				flight_ptr = flight_ptr.next_flight;
			}
#endif
			throw 
				new NotImplementedException ();
		}

		public void PreDogfightEval ()
		{
#if TODO
			// Called only upon entering/resetting a dogfight game
			CampEnterCriticalSection();
			
			#if FUNKY_KEVIN_DEBUG_STUFF
				Debug.Assert(!inMission || FalconSessionEntity.FalconLocalGame.GetGameType() == game_Dogfight);
			#endif
			
				CleanupFlightData();
				ClearPackageData();
			
				for (int i=0; i<MissEvakStatic.MAX_RELATED_EVENTS; i++)
					{
					if (related_events[i] != null)
						//delete(related_events[i]);
						related_events[i] = null;
					}
				last_related_event = 0;
			
				contact_score = 0;
				pack_success = 0;
				package_element = null;
				player_element = null;
				player_aircraft_slot = 255;
				friendly_losses = 0;
				actual_tot = new CampaignTime(0);
				patrol_time = new CampaignTime(0);
				package_mission = MissionTypeEnum.AMIS_SWEEP;
				package_context = 0;
				responses = 0;
				requesting_ent = VU_ID.FalconnullId;
				intercepting_ent = VU_ID.FalconnullId;
				awacs_id = VU_ID.FalconnullId;
				jstar_id = VU_ID.FalconnullId;
				ecm_id = VU_ID.FalconnullId;
				tanker_id = VU_ID.FalconnullId;
				action_type = 0;
				flags = 0;
				team = 0;
				alternate_strip_id = VU_ID.FalconnullId;
				abx = aby = -1;
				player_start_time = vuxGameTime;
				player_pilot = null;
				curr_data = 0;
				ClearPotentialTargets();
				// Analyse all flights
				VuListIterator		flit = new VuListIterator(AllAirList);
				Unit				uelement;
				uelement = (Unit) flit.GetFirst();
				while (uelement)
					{
					if (uelement.IsFlight())
						PreEvalFlight((Flight)uelement, null);
					uelement.SetInPackage(1);
					uelement = (Unit) flit.GetNext();
					}
			
				memset(rounds_won, 0, DogfightStatic.MAX_DOGFIGHT_TEAMS*sizeof(byte));
			
				friendly_losses = friendly_aa_losses = friendly_ga_losses = 0;
				logbook_data.KilledByHuman = 0;
				logbook_data.KilledBySelf = 0;
				logbook_data.FriendlyFireKills = 0;
				logbook_data.Flags = 0;
			
				CampLeaveCriticalSection();
			}

		public int PreMissionEval (Flight flight, byte aircraft_slot)
		{
			Package				package;
			Flight				element;
			Objective			aas;
		
		#if DEBUG
			if (testDebrief)
				return PostMissionEval();
		#endif
		
		#if FUNKY_KEVIN_DEBUG_STUFF
			Debug.Assert(!inMission);
		#endif
		
		#if DEBUG
			while (inMission && (!g_bLogEvents))
			{
				Sleep (100);
			}
		#endif
		
			Debug.Assert (doUI|| (g_bLogEvents));
		
			CampEnterCriticalSection();
		
			ClearPackageData();
			CleanupFlightData();
		
			for (int i=0; i<MAX_RELATED_EVENTS; i++)
				{
				if (related_events[i])
					delete(related_events[i]);
				related_events[i] = null;
				}
			last_related_event = 0;
		
			contact_score = 0;
			package = (Package)flight.GetUnitParent();
			pack_success = 0;
			package_element = null;
			player_element = null;
			player_aircraft_slot = 255;
			friendly_losses = 0;
			assigned_tot = flight.GetUnitTOT();
			actual_tot = new CampaignTime(0);
			patrol_time = new CampaignTime(0);
			if (package != null)
				{
				package.FindSupportFlights(package.GetMissionRequest(), 0);
				package_mission = package.GetMissionRequest().mission;
				package_context = package.GetMissionRequest().context;
				responses = package.GetResponses();
				requesting_ent = package.GetMissionRequest().requesterID;
				intercepting_ent = package.GetInterceptor();
				awacs_id = package.GetAwacs();		// FindAwacs(flight, &awx, &awy);
				jstar_id = package.GetJStar();		// FindJStar(flight, &jsx, &jsy);
				ecm_id = package.GetECM();
				tanker_id = package.GetTanker();	// FindTanker(flight, &tankx, &tanky);
				package.GetUnitDestination(&tx,&ty);
				action_type = package.GetMissionRequest().action_type;
				}
			else
				{
				package_mission = MissionTypeEnum.AMIS_SWEEP;
				package_context = 0;
				responses = 0;
				requesting_ent = VU_ID.FalconnullId;
				intercepting_ent = VU_ID.FalconnullId;
				awacs_id = VU_ID.FalconnullId;
				jstar_id = VU_ID.FalconnullId;
				ecm_id = VU_ID.FalconnullId;
				tanker_id = VU_ID.FalconnullId;
				flight.GetLocation(ref tx, ref ty);
				action_type = 0;
				}
			flags = 0;
			team = flight.GetTeam();
			aas = FindAlternateLandingStrip(flight);
			if (aas)
				{
				alternate_strip_id = aas.Id();
				aas.GetLocation(ref abx, ref aby);
				}
			else
				{
				alternate_strip_id = VU_ID.FalconnullId;
				abx = aby = -1;
				}
			player_start_time = vuxGameTime;
			player_pilot = null;
			curr_data = 0;
			FindPotentialTargets();
			if (package)
				{
				// Analyse child flights
				element = (Flight) package.GetFirstUnitElement();
				while (element)
					{
					PreEvalFlight(element, flight);
					element = (Flight) package.GetNextUnitElement();
					}
				}
			else
				{
				// Analyse all flights
				VuListIterator		flit = new VuListIterator(AllAirList);
				Unit				uelement;
				uelement = (Unit) flit.GetFirst();
				while (uelement)
					{
					if (uelement.IsFlight())
						PreEvalFlight((Flight)uelement, flight);
					uelement.SetInPackage(1);
					uelement = (Unit) flit.GetNext();
					}
				}
		
			friendly_losses = friendly_aa_losses = friendly_ga_losses = 0;
			if (player_pilot)
				player_pilot.shot_at = 0;
			logbook_data.KilledByHuman = 0;
			logbook_data.KilledBySelf = 0;
			logbook_data.FriendlyFireKills = 0;
			logbook_data.Flags = 0;
		
			if (aircraft_slot < 255)
				SetPackageData();
		
			Debug.Assert(package_element);
		
			CampLeaveCriticalSection();
		
			return 0;
#endif
			throw 
				new NotImplementedException ();
		}

		public void PreEvalFlight (Flight element, Flight flight)
		{
#if TODO			
			VehicleClassDataType	vc;
			WayPoint				tw,w;
			FlightDataClass			flight_ptr, tmp_ptr;
			CampEntity				target = null;
		
		//	Debug.Assert (FalconLocalSession.GetFlyState() == FLYSTATE_IN_UI);	- not in dogfight at least
		
			if (element)
				{
				flight_ptr = new FlightDataClass();
				if (flight && element.Id() == flight.Id())
					player_element = flight_ptr;
				vc = GetVehicleClassData(element.GetVehicleID(0));
				if (vc) // JB 010113
					_stprintf(flight_ptr.aircraft_name,vc.Name);
				else
					_stprintf(flight_ptr.aircraft_name,"");
				_stprintf(flight_ptr.name,flight_ptr.aircraft_name);
				GetCallsign (element.callsign_id, element.callsign_num, flight_ptr.name);
				flight_ptr.camp_id = element.GetCampID();
				flight_ptr.flight_id = element.Id();
				flight_ptr.start_aircraft = element.GetTotalVehicles();		
				flight_ptr.finish_aircraft = flight_ptr.start_aircraft;
				if (element.GetEvalFlags() & FEVAL_MISSION_STARTED)
					flight_ptr.status_flags = MISEVAL_FLIGHT_STARTED_LATE;
				else
					flight_ptr.status_flags = 0;
				flight_ptr.old_mission = element.GetOriginalMission();
				flight_ptr.mission = element.GetUnitMission();
				flight_ptr.mission_context = element.GetMissionContext();
				flight_ptr.requester_id = element.GetRequesterID();
				flight_ptr.flight_team = element.GetTeam();
				flight_ptr.target_x = tx;
				flight_ptr.target_y = ty;
				tw = element.GetFirstUnitWP();
				while (tw && !(tw.GetWPFlags() & WPF_TARGET))
					tw = tw.GetNextWP();
				if (!tw || !tw.GetWPTarget())
					tw = element.GetOverrideWP();
				if (tw)
					{
					flight_ptr.target_id = tw.GetWPTargetID();
					flight_ptr.target_building = tw.GetWPTargetBuilding(); 
					target = (CampEntity) FindEntity(flight_ptr.target_id);
					}
				else
					{
					flight_ptr.target_id = FalconnullId;
					flight_ptr.target_building = 255; 
					}
				if (flight_ptr.mission == MissionTypeEnum.AMIS_INTERCEPT || flight_ptr.mission == MissionTypeEnum.AMIS_CAS)
					{
					// Assign target to immediate target if it's intercepting/cas
					if (flight)
						{
						FalconEntity* tmp_target = flight.GetTarget();
						if (tmp_target && tmp_target.IsCampaign())
							target = (CampEntity) tmp_target;
						}
					}
				if (target)
					{
					if (target.IsUnit())
						{
						Unit	target_parent = ((Unit)target).GetUnitParent();
						if (target_parent)
							{
							target = target_parent;
							flight_ptr.target_id = target_parent.Id();
							}
						}
					flight_ptr.target_camp_id = target.GetCampID();
					}
				if (!package_element)
					{
					package_element = flight_ptr;
		//			package_mission = element.GetUnitMission();
					package_target_id = flight_ptr.target_id;
					RecordTargetStatus(flight_ptr, target);
					// Traverse our waypoints and collect data
					tw = 0;
					w = element.GetFirstUnitWP();
					while (w)
						{
						CollectThreats (element, w);
						if (w.GetWPFlags() & WPF_REPEAT)
							tw = w;
						w = w.GetNextWP();
						}
					if (tw)
						{
						WayPoint	pw = tw.GetPrevWP();
						patrol_time = tw.GetWPDepartureTime() - pw.GetWPArrivalTime();
						}
					}
				flight_ptr.failure_code = 0;
				flight_ptr.failure_data = 0;
				flight_ptr.failure_id = FalconnullId;
				flight_ptr.mission_success = 0;
				flight_ptr.num_pilots = 0;
				// Add to end of list.
				tmp_ptr = flight_data;
				while (tmp_ptr && tmp_ptr.next_flight)
					tmp_ptr = tmp_ptr.next_flight;
				if (tmp_ptr)
					tmp_ptr.next_flight = flight_ptr;
				else
					flight_data = flight_ptr;
				SetupPilots(flight_ptr, element);
				}
			#endif
			throw 
				new NotImplementedException ();
		}

		public void RecordTargetStatus (FlightDataClass flight_ptr, CampBaseClass target)
		{
#if TODO			
			if (!target)
				return;
		
			memset(flight_ptr.target_features,255,MAX_TARGET_FEATURES*sizeof(byte));
			if (target && target.IsObjective())
				{
				// Find top targets
				Objective			to = (Objective)target;
				ObjClassDataType	oc = to.GetObjectiveClassData();
				byte[]				targeted = { 0 };
				int					i,f,tf=0;
		
				for (i=0; i<20 && tf<MAX_TARGET_FEATURES; i++)
					{
					f = BestTargetFeature(to, targeted);
					if (f < 128 && to.GetFeatureValue(f))
						{
						targeted[f] = 1;
						flight_ptr.target_features[tf] = f;
						tf++;
						}
					}			
				// Record target status
				flight_ptr.target_status = to.GetObjectiveStatus();
				}
			else if (target && target.IsUnit())
				{
				target.GetName(flight_ptr.context_entity_name, 39, false);
				flight_ptr.target_status = ((Unit)target).GetTotalVehicles();
				}
			// Special case related unit data
			if (flight_ptr.mission_context == enemyUnitAdvanceBridge || flight_ptr.mission_context == enemyUnitMoveBridge || flight_ptr.mission_context == friendlyUnitAirborneMovement)
				{
				CampEntity	relEnt = (CampEntity) VuDatabase.vuDatabase.Find(flight_ptr.requester_id);
				if (relEnt)
					relEnt.GetName(flight_ptr.context_entity_name,39,false);
				}
			#endif
			throw 
				new NotImplementedException ();
		}

		public int	PostMissionEval ()
		{
#if TODO
			int					i;
			PilotDataClass		pilot_data;
			FlightDataClass		flight_ptr;
		
			CampEnterCriticalSection();
		#if DEBUG
			gPlayerPilotLock = 0;
		#endif
			player_end_time = vuxGameTime;
			logbook_data.AircraftInPackage = 0;
			friendly_losses = 0;
			pack_success = MissionSuccess(package_element);	
		
			flight_ptr = flight_data;
			while (flight_ptr != null)
				{
				if (flight_ptr.camp_id)
					{
					// This flight was active at the mission start
					flight_ptr.mission_success = MissionSuccess(flight_ptr);
					pilot_data = flight_ptr.pilot_list;
					flight_ptr.finish_aircraft = flight_ptr.start_aircraft;		
					while (pilot_data != null)
						{
						if (FalconSessionEntity.FalconLocalGame.GetGameType () != game_Dogfight)
							{
							pilot_data.score += ScoreAdjustment[flight_ptr.mission_success];
							pilot_data.score += ScoreAdjustment[pack_success]/2;
							}
						switch (pilot_data.pilot_status)
							{
							case PILOT_KIA:
							case PILOT_MIA:
								pilot_data.score += CalcScore (SCORE_PILOT_LOST, 0);
								flight_ptr.finish_aircraft--;		
								break;
							case PILOT_RESCUED:
								pilot_data.score += CalcScore (SCORE_PILOT_FOUND, 0);
								flight_ptr.finish_aircraft--;		
								break;
							default:
								break;
							}
						if (pilot_data.score > 16)
							pilot_data.rating = Excellent;
						else if (pilot_data.score > 6)
							pilot_data.rating = Good;
						else if (pilot_data.score > -6)
							pilot_data.rating = Average;
						else if (pilot_data.score > -16)
							pilot_data.rating = Poor;
						else 
							pilot_data.rating = Horrible;
		
						// KCK HACK: Per Gilman - cap success to PartSuccess if player didn't land
						if (pilot_data.pilot_slot >= PilotStatic.PILOTS_PER_FLIGHT && !(logbook_data.Flags & LANDED_AIRCRAFT) && flight_ptr.mission_success == Success && (!gCommsMgr || !gCommsMgr.Online()))
							flight_ptr.mission_success = PartSuccess;
						// END HACK
		
						// Record the rating (for records sake)
						// KCK:We probably don't have the flight at this point though...
		// 2002-02-16 MN Only rate a pilot if we didn't have to abort mission
						if (!(flight_ptr.mission_success == AWACSAbort))
							RatePilot((Flight)VuDatabase.vuDatabase.Find(flight_ptr.flight_id),pilot_data.aircraft_slot,pilot_data.rating);
						pilot_data = pilot_data.next_pilot;
						}
					friendly_losses += flight_ptr.start_aircraft - flight_ptr.finish_aircraft;
					logbook_data.AircraftInPackage += flight_ptr.start_aircraft;
					}
		
				// On Call CAS package succeed if any of the components succeeded
				if (package_mission == MissionTypeEnum.AMIS_ONCALLCAS && flight_ptr.mission != MissionTypeEnum.AMIS_FAC && flight_ptr.mission_success > pack_success)
					pack_success = flight_ptr.mission_success;
		
				flight_ptr = flight_ptr.next_flight;
				}
		
			// WARNING: Weird place for this, but for now this will work
			if (player_aircraft_slot < 255)
				{
				Objective	o;
				// KCK: Techinally, we should probably use the parent PO if there is one, but whatever...
				o = FindNearestObjective (POList, tx, ty, null);
				if (o && player_pilot)
					ApplyPlayerInput(team, o.Id(), player_pilot.score);
				}
			// Update logbook data
			Debug.Assert (player_pilot);
			if (player_pilot)
			{
				logbook_data.WeaponsExpended = 0;
				for (i=0; i<player_pilot.weapon_types; i++)
					{
					// Only count guns once - regardless of number of bursts
					if (player_pilot.weapon_data[i].weapon_id >= 0) // sanity check
						{
						if (!(WeaponDataTable[player_pilot.weapon_data[i].weapon_id].Flags & WEAP_ONETENTH))
							logbook_data.WeaponsExpended += player_pilot.weapon_data[i].fired;
						else if (player_pilot.weapon_data[i].fired)
							logbook_data.WeaponsExpended++;
						}
					}
				logbook_data.ShotsAtPlayer = player_pilot.shot_at;
				logbook_data.Score = player_pilot.rating + 1;
				logbook_data.Kills = player_pilot.aa_kills;
				logbook_data.HumanKills = player_pilot.player_kills;
				// Check for player survival
				if (player_pilot.pilot_status == PILOT_KIA)
					logbook_data.Killed = 1;
				else
					logbook_data.Killed = 0;
		
				// KCK QUICK HACK: Fix very large flight hours problem. The real fix should be done later.
				if (player_end_time < player_start_time)
					player_end_time = player_start_time + CampaignMinutes;
				logbook_data.FlightHours = (float)((float)(player_end_time - player_start_time)/CampaignHours);
				logbook_data.GroundUnitsKilled = player_pilot.ag_kills;
				logbook_data.FeaturesDestroyed = player_pilot.as_kills;
				logbook_data.NavalUnitsKilled = player_pilot.an_kills;
				logbook_data.WingmenLost = friendly_losses;
		
				switch(FalconSessionEntity.FalconLocalGame.GetGameType())
					{
					case FalconGameType.game_Dogfight:
						{
						int won = 0,vsHuman = 0;
						if (flags & MISEVAL_GAME_COMPLETED)
							{
							if (player_pilot.pilot_flags & PFLAG_WON_GAME)
								won = 1;
							else
								won = -1;
							}
						if (flags & MISEVAL_ONLINE_GAME)
							vsHuman = 1;
						LogBook.SetAceFactor(FalconLocalSession.GetAceFactor());
						LogBook.UpdateDogfight(won, logbook_data.FlightHours, vsHuman, player_pilot.aa_kills, player_pilot.deaths[MissEvakStatic.VS_AI]+player_pilot.deaths[MissEvakStatic.VS_HUMAN], player_pilot.player_kills, player_pilot.deaths[MissEvakStatic.VS_HUMAN] );
						}
						break;
					case FalconGameType.game_Campaign:
						LogBook.SetAceFactor(FalconLocalSession.GetAceFactor());
		// 2002-02-13 MN added AWACSAbort condition for "don't score mission"
						if (!logbook_data.Killed && (player_element.mission_success == Incomplete || player_element.mission_success == AWACSAbort))
							{
							logbook_data.Flags |= DONT_SCORE_MISSION;
							LogBook.UpdateCampaign(&logbook_data);
							}
						else
							{
							LogBook.UpdateCampaign(&logbook_data);
							if(FalconLocalSession.GetMissions())
							{
								int missions,rating;
								missions=FalconLocalSession.GetMissions();
								rating=(FalconLocalSession.GetRating() * missions + (player_pilot.rating*25)) / (missions+1);
								FalconLocalSession.SetRating(rating);
								FalconLocalSession.SetMissions(missions+1);
							}
							else
							{
								FalconLocalSession.SetRating((player_pilot.rating*25));
								FalconLocalSession.SetMissions(1);
							}
						}
						
						FalconLocalSession.SetKill(FalconSessionEntity._AIR_KILLS_,FalconLocalSession.GetKill(FalconSessionEntity._AIR_KILLS_)+logbook_data.Kills+logbook_data.HumanKills);
						FalconLocalSession.SetKill(FalconSessionEntity._GROUND_KILLS_,FalconLocalSession.GetKill(FalconSessionEntity._GROUND_KILLS_)+logbook_data.GroundUnitsKilled);
						FalconLocalSession.SetKill(FalconSessionEntity._NAVAL_KILLS_,FalconLocalSession.GetKill(FalconSessionEntity._NAVAL_KILLS_)+logbook_data.NavalUnitsKilled);
						FalconLocalSession.SetKill(FalconSessionEntity._STATIC_KILLS_,FalconLocalSession.GetKill(FalconSessionEntity._STATIC_KILLS_)+logbook_data.FeaturesDestroyed);
						break;
					case FalconGameType.game_TacticalEngagement:
		// 2002-02-13 MN added AWACSAbort condition
						if (logbook_data.Killed || (player_element.mission_success != Incomplete) && (player_element.mission_success != AWACSAbort))
						{
							if(FalconLocalSession.GetMissions())
							{
								int missions,rating;
								missions=FalconLocalSession.GetMissions();
								rating=(FalconLocalSession.GetRating() * missions + (player_pilot.rating*25)) / (missions+1);
								FalconLocalSession.SetRating(rating);
								FalconLocalSession.SetMissions(missions+1);
							}
							else
							{
								FalconLocalSession.SetRating((player_pilot.rating*25));
								FalconLocalSession.SetMissions(1);
							}
						}
						LogBook.UpdateFlightHours(logbook_data.FlightHours);
						LogBook.SetAceFactor(FalconLocalSession.GetAceFactor());
						LogBook.SaveData();
						FalconLocalSession.SetKill(FalconSessionEntity._AIR_KILLS_,FalconLocalSession.GetKill(FalconSessionEntity._AIR_KILLS_)+logbook_data.Kills+logbook_data.HumanKills);
						FalconLocalSession.SetKill(FalconSessionEntity._GROUND_KILLS_,FalconLocalSession.GetKill(FalconSessionEntity._GROUND_KILLS_)+logbook_data.GroundUnitsKilled);
						FalconLocalSession.SetKill(FalconSessionEntity._NAVAL_KILLS_,FalconLocalSession.GetKill(FalconSessionEntity._NAVAL_KILLS_)+logbook_data.NavalUnitsKilled);
						FalconLocalSession.SetKill(FalconSessionEntity._STATIC_KILLS_,FalconLocalSession.GetKill(FalconSessionEntity._STATIC_KILLS_)+logbook_data.FeaturesDestroyed);
						break;
					case FalconGameType.game_InstantAction:
						LogBook.UpdateFlightHours(logbook_data.FlightHours);
						LogBook.SaveData();
						break;
					default:
						break;
					}
			}
			ClearPackageData();
			flags = 0;
			CampLeaveCriticalSection();
		
			return 0;
#endif
			throw 
				new NotImplementedException ();			
		}

		public void ServerFileLog (FalconPlayerStatusMessage fpsm)
		{
			throw new NotImplementedException ();
		}
		
		public int MissionSuccess (FlightDataClass flight_ptr)
		{
#if TODO			
			int		retval = Failed, losses;
		
			if (!flight_ptr)
				return retval;
		
		// 2002-02-13 MN Check if we got an Abort from AWACS and the target has not been engaged
		// If it has been engaged, mission failed
			if (flight_ptr.status_flags & MISEVAL_FLIGHT_ABORT_BY_AWACS)
				if (!(flight_ptr.status_flags & MISEVAL_FLIGHT_TARGET_HIT))
				{
					flight_ptr.failure_code = 1;
					return AWACSAbort;
				}
				else
		// We hit the target, but got AWACS order to abort before..
				{
					int			statloss = 0;
					CampEntity	e = FindEntity(flight_ptr.target_id);
					if (e && e.IsObjective())
						statloss = flight_ptr.target_status - ((Objective)e).GetObjectiveStatus();
					if (flight_ptr.status_flags & MISEVAL_FLIGHT_HIT_HIGH_VAL || statloss > 35)
						{
						flight_ptr.failure_code = 4;
						}
					else if (statloss > 10)
						{
						flight_ptr.failure_code = 3;
						}
					else
						{
						flight_ptr.failure_code = 2;
						}
					return retval;
				}
		
			// Check for in progress
			if (!(flight_ptr.status_flags & FlightStatus.MISEVAL_FLIGHT_GOT_TO_TARGET) &&
				!(flight_ptr.status_flags & FlightStatus.MISEVAL_FLIGHT_STATION_OVER) &&
				!(flight_ptr.status_flags & FlightStatus.MISEVAL_FLIGHT_OFF_STATION) &&
				!(flight_ptr.status_flags & FlightStatus.MISEVAL_FLIGHT_DESTROYED) &&
				!(flight_ptr.status_flags & FlightStatus.MISEVAL_FLIGHT_ABORTED))
				{
				flight_ptr.failure_code = 0;
				return Incomplete;
				}
		
			switch (flight_ptr.mission)
				{
				case MissionTypeEnum.AMIS_BARCAP:
				case MissionTypeEnum.AMIS_BARCAP2:
				case MissionTypeEnum.AMIS_TARCAP:
				case MissionTypeEnum.AMIS_RESCAP:
				case MissionTypeEnum.AMIS_AMBUSHCAP:
					// Determine if we stayed in the area or not
					if (!(flight_ptr.status_flags & MISEVAL_FLIGHT_OFF_STATION))
						{
						// Determine if our vol period is over or not
						if (flight_ptr.status_flags & MISEVAL_FLIGHT_STATION_OVER)
							{
							// We completed the voll - simply check for damage or not
							if (flight_ptr.status_flags & MISEVAL_FLIGHT_AREA_HIT)
								{
								retval = PartFailed;
								flight_ptr.failure_code = 40;
								}
							else
								{
								retval = Success;
								flight_ptr.failure_code = 41;
								}
							}
						// Check if we had permission to leave early or not
						else if (flight_ptr.status_flags & MISEVAL_FLIGHT_RELIEVED)
							{
							if (flight_ptr.status_flags & MISEVAL_FLIGHT_AREA_HIT)
								{
								retval = PartFailed;
								flight_ptr.failure_code = 40;
								}
							else if (flight_ptr.status_flags & MISEVAL_FLIGHT_GOT_AKILL)
								{
								retval = Success;
								flight_ptr.failure_code = 42;
								}
							else
								{
								retval = PartSuccess;
								flight_ptr.failure_code = 43;
								}
							}
						else
							flight_ptr.failure_code = 57;
						}
					else
						flight_ptr.failure_code = 58;
					break;
				case MissionTypeEnum.AMIS_HAVCAP: 
					// Check if our target was killed
					if (flight_ptr.status_flags & MISEVAL_FLIGHT_F_TARGET_KILLED)
						flight_ptr.failure_code = 48;
					// Check if our target was hit
					else if (flight_ptr.status_flags & MISEVAL_FLIGHT_F_TARGET_HIT)
						{
						retval = PartFailed;
						flight_ptr.failure_code = 49;
						}
					// Check if our target aborted
					else if (flight_ptr.status_flags & MISEVAL_FLIGHT_F_TARGET_ABORTED)
						{
						retval = PartFailed;
						flight_ptr.failure_code = 50;
						}
					else
						{
						// Check if we completed our time
						if (flight_ptr.status_flags & MISEVAL_FLIGHT_STATION_OVER)
							{
							retval = Success;
							flight_ptr.failure_code = 46;
							}
						// Check if we were relieved
						else if (flight_ptr.status_flags & MISEVAL_FLIGHT_RELIEVED)
							{
							retval = PartSuccess;
							flight_ptr.failure_code = 47;
							}
						else
							{
							retval = PartFailed;
							flight_ptr.failure_code = 57;
							}
						}
					break;
				case MissionTypeEnum.AMIS_INTERCEPT: 
					if (flight_ptr.status_flags & MISEVAL_FLIGHT_TARGET_KILLED)
						{
						retval = Success;
						flight_ptr.failure_code = 35;
						}
					else if (flight_ptr.status_flags & MISEVAL_FLIGHT_TARGET_ABORTED)
						{
						retval = PartSuccess;
						flight_ptr.failure_code = 36;
						}
					else if (flight_ptr.status_flags & FlightStatus.MISEVAL_FLIGHT_TARGET_HIT)
						{
						retval = PartFailed;
						flight_ptr.failure_code = 37;
						}
					else
						flight_ptr.failure_code = 38;
					break;
				case MissionTypeEnum.AMIS_SWEEP:
					if (flight_ptr.status_flags & FlightStatus.MISEVAL_FLIGHT_DESTROYED)
						flight_ptr.failure_code = 30;
					else if ((flight_ptr.status_flags & FlightStatus.MISEVAL_FLIGHT_LOSSES) || !(flight_ptr.status_flags & MISEVAL_FLIGHT_GOT_TO_TARGET))
						{
						if (flight_ptr.status_flags & FlightStatus.MISEVAL_FLIGHT_GOT_AKILL)
							{
							retval = PartFailed;
							flight_ptr.failure_code = 32;
							}
						else
							{
							retval = Failed;
							flight_ptr.failure_code = 33;
							}
						}
					else
						{
						if (flight_ptr.status_flags & MISEVAL_FLIGHT_GOT_AKILL)
							{
							retval = Success;
							flight_ptr.failure_code = 31;
							}
						else 
							{
							retval = PartSuccess;
							flight_ptr.failure_code = 34;
							}
						}
					break;
				case MissionTypeEnum.AMIS_ESCORT:
				case MissionTypeEnum.AMIS_SEADESCORT:
					if (flight_ptr.mission == MissionTypeEnum.AMIS_ESCORT)
						losses = friendly_aa_losses;
					else
						losses = friendly_ga_losses;
					if (package_element.status_flags & MISEVAL_FLIGHT_GOT_TO_TARGET ||
						package_element.status_flags & MISEVAL_FLIGHT_ABORT_BY_AWACS)
						{
						if (package_element.status_flags & MISEVAL_FLIGHT_HIT_BY_GROUND && losses)
							{
							retval = PartSuccess;
							if (losses == 1)
								flight_ptr.failure_code = 25;
							else
								flight_ptr.failure_code = 26;
							}
						else
							{
							retval = Success;
							flight_ptr.failure_code = 27;
							}
						}
					else if (package_element.status_flags & MISEVAL_FLIGHT_ABORTED)
						{
						if (package_element.status_flags & MISEVAL_FLIGHT_HIT_BY_GROUND && losses)
							{
							retval = PartFailed;
							if (losses == 1)
								flight_ptr.failure_code = 25;
							else
								flight_ptr.failure_code = 26;
							}
						else
							{
							retval = PartSuccess;
							flight_ptr.failure_code = 28;
							}
						}
					else
						{
						retval = Failed;
						if (package_element.status_flags & FlightStatus.MISEVAL_FLIGHT_STATION_OVER || package_element.status_flags & FlightStatus.MISEVAL_FLIGHT_DESTROYED)
							flight_ptr.failure_code = 25;
						else
							flight_ptr.failure_code = 24;
						}
					break;
				case MissionTypeEnum.AMIS_OCASTRIKE:
				case MissionTypeEnum.AMIS_INTSTRIKE:
				case MissionTypeEnum.AMIS_STRIKE:	
				case MissionTypeEnum.AMIS_DEEPSTRIKE:
				case MissionTypeEnum.AMIS_STSTRIKE:
				case MissionTypeEnum.AMIS_STRATBOMB:
					if (flight_ptr.status_flags & MISEVAL_FLIGHT_TARGET_HIT)
						{
						int			statloss = 0;
						CampEntity	e = FindEntity(flight_ptr.target_id);
						if (e && e.IsObjective())
							statloss = flight_ptr.target_status - ((Objective)e).GetObjectiveStatus();
						if (flight_ptr.status_flags & MISEVAL_FLIGHT_HIT_HIGH_VAL || statloss > 35)
							{
							retval = Success;
							flight_ptr.failure_code = 4;
							}
						else if (statloss > 10)
							{
							retval = Success;
							flight_ptr.failure_code = 3;
							}
						else
							{
							retval = PartSuccess;
							flight_ptr.failure_code = 2;
							}
						}
					else
						flight_ptr.failure_code = 1;
					break;			
				case MissionTypeEnum.AMIS_SEADSTRIKE:
				case MissionTypeEnum.AMIS_PRPLANCAS: 
				case MissionTypeEnum.AMIS_CAS:	
				case MissionTypeEnum.AMIS_ASW:     
				case MissionTypeEnum.AMIS_ASHIP:   
					if (flight_ptr.status_flags & MISEVAL_FLIGHT_TARGET_HIT)
						{
						int			statloss = 0;
						CampEntity	e;
						e = FindEntity(flight_ptr.target_id);
						if (e && e.IsUnit())
							statloss = FloatToInt32((float)(flight_ptr.target_status - ((Unit)e).GetTotalVehicles())*100.0F/flight_ptr.target_status);
						if (flight_ptr.status_flags & MISEVAL_FLIGHT_HIT_HIGH_VAL || statloss > 8)
							{
							retval = Success;
							flight_ptr.failure_code = 8;
							}
						else if (statloss > 2)
							{
							retval = Success;
							flight_ptr.failure_code = 7;
							}
						else
							{
							retval = PartSuccess;
							flight_ptr.failure_code = 6;
							}
						}
					else
						flight_ptr.failure_code = 5;
					break;			
				case MissionTypeEnum.AMIS_SAD:	
				case MissionTypeEnum.AMIS_INT:
				case MissionTypeEnum.AMIS_BAI:
				case MissionTypeEnum.AMIS_PATROL:	
					if (flight_ptr.status_flags & MISEVAL_FLIGHT_GOT_AKILL ||
						flight_ptr.status_flags & MISEVAL_FLIGHT_GOT_GKILL ||
						flight_ptr.status_flags & MISEVAL_FLIGHT_GOT_NKILL)
						{
						// Calculate hit ratio & kills
						int				hit=0,fired=0,kills=0,i;
						PilotDataClass	*pilot_data;
						pilot_data = flight_ptr.pilot_list;
						while (pilot_data)
							{
							for (i=0; i<pilot_data.weapon_types; i++)
								{
								hit += pilot_data.weapon_data[i].hit;
								if (!(WeaponDataTable[pilot_data.weapon_data[i].weapon_id].GuidanceFlags & WEAP_GUIDED_MASK))
									hit += pilot_data.weapon_data[i].hit;			// double credit for unguided hits
								if (!(WeaponDataTable[pilot_data.weapon_data[i].weapon_id].Flags & WEAP_ONETENTH))
									fired += pilot_data.weapon_data[i].fired;		// only count nongun weapons as fired
								}
							kills += pilot_data.ag_kills + pilot_data.an_kills*4;
							pilot_data = pilot_data.next_pilot;
							}
						if (kills > 4)
							{
							retval = Success;
							flight_ptr.failure_code = 13;
							}
						if (kills > 0 && ((fired > 0 && ((hit*100)/fired) > 49) || (hit > 0 && !fired)))
							{
							retval = Success;
							flight_ptr.failure_code = 11;
							}
						else
							{
							retval = PartSuccess;
							flight_ptr.failure_code = 12;
							}
						}
					else
						flight_ptr.failure_code = 10;
					break;
				case MissionTypeEnum.AMIS_ONCALLCAS:
					// Check to see that contact was made with FAC
					if (flight_ptr.status_flags & MISEVAL_FLIGHT_GOT_TO_TARGET)
						{
						if (flight_ptr.status_flags & MISEVAL_FLIGHT_OFF_STATION)
							{
							retval = Failed;
							flight_ptr.failure_code = 58;
							}
						else
							{
							retval = PartSuccess;
							flight_ptr.failure_code = 52;
							}
						}
					else
						flight_ptr.failure_code = 51;
					break;
				case MissionTypeEnum.AMIS_RECON: 	
				case MissionTypeEnum.AMIS_BDA:
				case MissionTypeEnum.AMIS_RECONPATROL:
					if (flight_ptr.status_flags & MISEVAL_FLIGHT_TARGET_HIT)
						{
						flight_ptr.failure_code = 15;
						retval = Success;
						}
					else
						flight_ptr.failure_code = 16;
					break;
				case MissionTypeEnum.AMIS_AWACS:    
				case MissionTypeEnum.AMIS_JSTAR:     
				case MissionTypeEnum.AMIS_TANKER:    
				case MissionTypeEnum.AMIS_ECM:	
				case MissionTypeEnum.AMIS_FAC:
					if (flight_ptr.status_flags & MISEVAL_FLIGHT_OFF_STATION)
						{
						retval = Failed;
						flight_ptr.failure_code = 58;
						}
					else if (flight_ptr.status_flags & MISEVAL_FLIGHT_RELIEVED)
						{
						retval = PartSuccess;
						flight_ptr.failure_code = 56;
						}
					else if (flight_ptr.status_flags & MISEVAL_FLIGHT_STATION_OVER)
						{
						retval = Success;
						flight_ptr.failure_code = 55;
						}
					else
						flight_ptr.failure_code = 57;
					break;
				case MissionTypeEnum.AMIS_SAR:       
				case MissionTypeEnum.AMIS_AIRCAV:  
				case MissionTypeEnum.AMIS_AIRLIFT:   
					if (flight_ptr.status_flags & MISEVAL_FLIGHT_DESTROYED)
						flight_ptr.failure_code = 30;
					else if (flight_ptr.status_flags & MISEVAL_FLIGHT_GOT_TO_TARGET)
						{
						flight_ptr.failure_code = 60;
						retval = Success;
						}
					else
						flight_ptr.failure_code = 61;
					break;
				default:
					flight_ptr.failure_code = 99;
					break;
				}
		
			// If we didn't fly it all the way home, knock off one success level
			if (retval < Failed && !(flight_ptr.status_flags & MISEVAL_FLIGHT_GOT_HOME))
				retval -= 1;
		
			// If we didn't fly it from the start, cap success at Partial.
			if (retval == Success && (flight_ptr.status_flags & MISEVAL_FLIGHT_STARTED_LATE))
				retval = PartSuccess;
		
			return retval;
			#endif
			throw 
				new NotImplementedException ();
		}

		public void SetPackageData ()
		{
#if TOD	
			// Set the package pointer for everything associated with the local player's package
			CampEntity			ent;
			FlightDataClass		flight_ptr;
		
			flight_ptr = flight_data;
			while (flight_ptr != null)
				{
				ent = (CampEntity) VuDatabase.vuDatabase.Find(flight_ptr.flight_id);
				if (ent)
					ent.SetInPackage(1);
				flight_ptr = flight_ptr.next_flight;
				}
	#endif
			throw 
				new NotImplementedException ();
		}

		public void ClearPackageData ()
		{
#if TOD
			// Clear the package pointer for everything associated with the local player's package
			CampEntity			ent;
			FlightDataClass		*flight_ptr;
		
			flight_ptr = flight_data;
			while (flight_ptr)
				{
				ent = (CampEntity) VuDatabase.vuDatabase.Find(flight_ptr.flight_id);
				if (ent)
					ent.SetInPackage(0);
				flight_ptr = flight_ptr.next_flight;
				}
		#endif
			throw 
				new NotImplementedException ();
		}

		public void ClearPotentialTargets ()
		{
			for (int i=0; i<MissEvakStatic.MAX_POTENTIAL_TARGETS; i++)
                potential_targets[i] = VU_ID.FalconNullId;
		}

		public void FindPotentialTargets ()
		{
#if TODO
			int					i,j;
			Objective			o;
		
			ClearPotentialTargets();
		
			if (package_mission == MissionTypeEnum.AMIS_BARCAP || package_mission == MissionTypeEnum.AMIS_BARCAP2)
				{
		#if VU_GRID_TREE_Y_MAJOR
				VuGridIterator*		myit = new VuGridIterator(ObjProxList,(BIG_SCALAR)GridToSim(tx),(BIG_SCALAR)GridToSim(ty),(BIG_SCALAR)GridToSim(MissionData[package_mission].mindistance));
		#else
				VuGridIterator*		myit = new VuGridIterator(ObjProxList,(BIG_SCALAR)GridToSim(ty),(BIG_SCALAR)GridToSim(tx),(BIG_SCALAR)GridToSim(MissionData[package_mission].mindistance));
		#endif
				float[]				d,wd,dists = new float[MissEvakStatic.MAX_POTENTIAL_TARGETS];
				GridIndex			x,y;
		
				for (i=0; i<MissEvakStatic.MAX_POTENTIAL_TARGETS; i++)
					dists[i] = 999.9F;
				o = (Objective) myit.GetFirst();
				while (o)
					{
					if (o.GetTeam() == team && o.GetObjectiveStatus() > 30 && (o.GetFalconType() == TYPE_BRIDGE || o.GetType() == Classtable_Types.TYPE_AIRBASE || o.GetType() == TYPE_DEPOT || o.GetType() == TYPE_ARMYBASE || o.GetType() == TYPE_FACTORY || o.GetType() == TYPE_RADAR || o.GetType() == TYPE_PORT || o.GetType() == TYPE_REFINERY || o.GetType() == TYPE_POWERPLANT))
						{
						o.GetLocation(&x,&y);
						d = Distance(x,y,tx,ty);
						// find the best distance to replace
						for (i=0,j=-1,wd=0.0F; i<MissEvakStatic.MAX_POTENTIAL_TARGETS; i++)
							{
							if (dists[i] > wd && d < dists[i])
								{
								j = i;
								wd = dists[i];
								}
							}
						if (j >= 0)
							{
							dists[j] = d;
							potential_targets[j] = o.Id();
							}
						}
					o = (Objective) myit.GetNext();
					}
				}
#endif
            throw 
				new NotImplementedException ();
		}

		public void CollectThreats (Flight flight, WayPoint tw)
		{
#if TODO
			WayPoint		w,nw;
			int				step,i;
			GridIndex		x,y,fx,fy,nx,ny;
			float			xd,yd,d;
			int[]				dist,dists = new int[MAX_COLLECTED_THREATS];
		
			memset(threat_ids,0,sizeof(short)*MAX_COLLECTED_THREATS);
			for (i=0; i<MAX_COLLECTED_THREATS; i++)
				{
				dists[i] = 999;
				threat_x[i] = threat_y[i] = 0;
				}
		
			// Collect threats at target first;
			if (tw)
				{
				tw.GetWPLocation(&x,&y);
				CollectThreats (x, y, tw.GetWPAltitude(), FIND_NOAIR | FIND_FINDUNSPOTTED, dists);
				}
		
			// Collect threats along route
			w = flight.GetFirstUnitWP();
			nw = w.GetNextWP();
			while (w && nw && w != tw)
				{
				w.GetWPLocation(&fx,&fy);
				nw.GetWPLocation(&nx,&ny);
				d = Distance(fx,fy,nx,ny);
				dist = FloatToInt32(d);
				if (d != 0.0) // JB 010614 CTD
				{
					xd = (float)(nx-fx)/d;
					yd = (float)(ny-fy)/d;
				}
				for (step=0; step<=dist; step+=5)
					{
					x = fx + (GridIndex)(xd*step + 0.5F);
					y = fy + (GridIndex)(yd*step + 0.5F);
					CollectThreats (x, y, tw.GetWPAltitude(), FIND_NOAIR | FIND_FINDUNSPOTTED, dists);
					}
				w = nw;
				nw = w.GetNextWP();
				}
				#endif
			throw 
				new NotImplementedException ();
		}

		public void CollectThreats (GridIndex X, GridIndex Y, int Z, int flags, int[] dists)
		{
#if TODO
	int				d,hc,alt=0,i,j,k,wd,wid;
	MoveType		mt;
	GridIndex		x,y;
	Unit			e;
	byte[]			tteam = new byte[(int)TeamDataEnum.NUM_TEAMS];
#if VU_GRID_TREE_Y_MAJOR
	VuGridIterator	myit(RealUnitProxList,(BIG_SCALAR)GridToSim(X),(BIG_SCALAR)GridToSim(Y),(BIG_SCALAR)GridToSim(MAX_AIR_SEARCH));
#else
	VuGridIterator	myit = new VuGridIterator(RealUnitProxList,(BIG_SCALAR)GridToSim(Y),(BIG_SCALAR)GridToSim(X),(BIG_SCALAR)GridToSim(MAX_AIR_SEARCH));
#endif

	alt = 3 * FloatToInt32(Z * 0.000303F);		// Convert feet to km, then adjust heavy (1 km alt = 3 km range)
	alt = alt*alt;						// pre square, to save calculations
 	if (Z > LOW_ALTITUDE_CUTOFF)
		mt = Air;
	else
		mt = LowAir;
	
	// Set up roe checks
	for (d=0; d<(int)TeamDataEnum.NUM_TEAMS && TeamStatic.TeamInfo[d]; d++)
		tteam[d] = GetRoE(d,team,ROE_AIR_ENGAGE);

	// Tranverse our list
	e = (Unit) myit.GetFirst();
	while (e && !threat_ids[MAX_COLLECTED_THREATS-1])
		{
		if (tteam[e.GetTeam()])
			{
			if (!(flags & FIND_NOMOVERS && e.Moving()) && 
				!(flags & FIND_NOAIR && e.GetDomain() == DOMAIN_AIR) &&
				(flags & FIND_FINDUNSPOTTED || e.GetSpotted(team)))
				{
				// Find the distance
				e.GetLocation(&x,&y);
				d = FloatToInt32(Distance(X,Y,x,y));

				// Find hitchance
				hc = e.GetAproxHitChance(mt,d);
				if (hc && mt == Air)					// Do a reasonable altitude adjusted guess
					hc = e.GetAproxHitChance(mt,FloatToInt32((float)sqrt((float)(alt+d*d))));

				if (hc > 0)
					{
					j = -1;
					// Check if we've already found it before
					for (i=0; i<MAX_COLLECTED_THREATS; i++)
						{
						if (threat_x[i] == x && threat_y[i] == y)
							j=0;
						}
					// Now find which weapon it was which can hit us
					for (i=0; i<Camplib.VEHICLES_PER_UNIT && j<0; i++)
						{
						wid = e.GetBestVehicleWeapon(i, DefaultDamageMods, mt, d, &k);
						if (wid && CampWeapons.GetWeaponHitChance (wid, mt) > MINIMUM_VIABLE_THREAT)
							{
							// find the best distance to replace
							for (k=0,wd=0; k<MAX_COLLECTED_THREATS; k++)
								{
								if (dists[k] > wd && d < dists[k])
									{
									j = k;
									wd = dists[k];
									}
								}
							if (j >= 0)
								{
								dists[j] = d;
								threat_ids[j] = e.class_data.VehicleType[i];
								threat_x[j] = x;
								threat_y[j] = y;
								}
							}
						}
					}
				}
			}
		e = (Unit) myit.GetNext();
		}
#endif
            throw 
				new NotImplementedException ();
		}

	
		// Dogfight kill evalutators
		public void GetTeamKills (ref short kills)
		{
#if TODO
	FlightDataClass		flight_ptr;
	PilotDataClass		pilot_data;
	int					t;

	CampEnterCriticalSection();

	for (t=0; t<DogfightStatic.MAX_DOGFIGHT_TEAMS; t++)
		kills[t] = 0;

	// Check if someone in the package fired this.
	flight_ptr = flight_data;
	while (flight_ptr)
		{
		pilot_data = flight_ptr.pilot_list;
		while (pilot_data)
			{
			kills[flight_ptr.flight_team] += pilot_data.aa_kills;
			pilot_data = pilot_data.next_pilot;
			}
		flight_ptr = flight_ptr.next_flight;
		}
	CampLeaveCriticalSection();
			#endif
			throw 
				new NotImplementedException ();
		}

		public void GetTeamDeaths (ref short deaths)
		{
#if TODO
	FlightDataClass		flight_ptr;
	PilotDataClass		pilot_data;
	int					t;

	CampEnterCriticalSection();

	for (t=0; t<DogfightStatic.MAX_DOGFIGHT_TEAMS; t++)
		deaths[t] = 0;

	// Check if someone in the package fired this.
	flight_ptr = flight_data;
	while (flight_ptr)
		{
		pilot_data = flight_ptr.pilot_list;
		while (pilot_data)
			{
			deaths[flight_ptr.flight_team] += pilot_data.deaths[MissEvakStatic.VS_HUMAN] + pilot_data.deaths[MissEvakStatic.VS_AI];
			pilot_data = pilot_data.next_pilot;
			}
		flight_ptr = flight_ptr.next_flight;
		}

	CampLeaveCriticalSection();
			#endif
			throw 
				new NotImplementedException ();
		}

		public void GetTeamScore (ref short score)
		{
#if TODO
	FlightDataClass		flight_ptr;
	PilotDataClass		pilot_data;
	int					t;

	CampEnterCriticalSection();

	// KCK: If we're in match play, we need to gather our 'score' from rounds won
	if (SimDogfight.GetGameType() == dog_TeamMatchplay)
		{
		for (t=0; t<DogfightStatic.MAX_DOGFIGHT_TEAMS; t++)
			score[t] = rounds_won[t];
		}
	else
		{
		for (t=0; t<DogfightStatic.MAX_DOGFIGHT_TEAMS; t++)
			score[t] = 0;

		// Check if someone in the package fired this.
		flight_ptr = flight_data;
		while (flight_ptr)
			{
			pilot_data = flight_ptr.pilot_list;
			while (pilot_data)
				{
				score[flight_ptr.flight_team] += pilot_data.score;
				pilot_data = pilot_data.next_pilot;
				}
			flight_ptr = flight_ptr.next_flight;
			}
		}
	CampLeaveCriticalSection();
			#endif
			throw 
				new NotImplementedException ();
		}

		public int GetKills (FalconSessionEntity player)
		{
#if TODO
	FlightDataClass		flight_ptr;
	PilotDataClass		pilot_data;

	CampEnterCriticalSection();

	// Check if someone in the package fired this.
	flight_ptr = flight_data;
	while (flight_ptr)
		{
		pilot_data = flight_ptr.pilot_list;
		while (pilot_data)
			{
			if (flight_ptr.flight_id == player.GetPlayerFlightID() && pilot_data.pilot_slot == player.GetPilotSlot())
				{
				CampLeaveCriticalSection();
				return pilot_data.aa_kills;
				}
			pilot_data = pilot_data.next_pilot;
			}
		flight_ptr = flight_ptr.next_flight;
		}
	CampLeaveCriticalSection();
	return 0;
			#endif
			throw 
				new NotImplementedException ();
		}

		public int GetMaxKills ()
		{
#if TODO
	FlightDataClass		flight_ptr;
	PilotDataClass		pilot_data;
	int					best=0;

	CampEnterCriticalSection();

	// Check if someone in the package fired this.
	flight_ptr = flight_data;
	while (flight_ptr)
		{
		pilot_data = flight_ptr.pilot_list;
		while (pilot_data)
			{
			if (pilot_data.aa_kills > best)
				best = pilot_data.aa_kills;
			pilot_data = pilot_data.next_pilot;
			}
		flight_ptr = flight_ptr.next_flight;
		}

	CampLeaveCriticalSection();
	return best;
					#endif
			throw 
				new NotImplementedException ();
		}

		public int GetMaxScore ()
		{
#if TODO
	FlightDataClass		flight_ptr;
	PilotDataClass		pilot_data;
	int					best=0;

	CampEnterCriticalSection();

	// Check if someone in the package fired this.
	flight_ptr = flight_data;
	while (flight_ptr)
		{
		pilot_data = flight_ptr.pilot_list;
		while (pilot_data)
			{
			if (pilot_data.score > best)
				best = pilot_data.score;
			pilot_data = pilot_data.next_pilot;
			}
		flight_ptr = flight_ptr.next_flight;
		}

	CampLeaveCriticalSection();
	return best;
			#endif
			throw 
				new NotImplementedException ();
		}

	
		// Event list builders
		public void RegisterDivert (FalconDivertMessage dm)
		{
			throw new NotImplementedException ();
		}
		
		public void RegisterShotAtPlayer (FalconWeaponsFire wfm, ushort CampID, byte fPilotID)
		{
#if TODO
	EventElement			theEvent;
	string					time_str,format,pnum;

// 2002-04-07 MN don't record gun shots at us or when someone takes a picture from us ;-)....
	if (g_bNoAAAEventRecords && (wfm.dataBlock.weaponType == FalconWeaponsFire.GUN || wfm.dataBlock.weaponType == FalconWeaponsFire.Recon))
		return;

	CampEnterCriticalSection();

	// Check if the target is also someone we're tracking messages for
	PilotDataClass	target_data = null;
	FlightDataClass	target_flight = flight_data;
	PilotDataClass	shooter_data = null;
	FlightDataClass	shooter_flight = flight_data;

//find the target
	while (target_flight && !target_data)
		{
		if (target_flight.camp_id == CampID)
			{
			target_data = FindPilotData(target_flight,fPilotID);
			break;
			}
		target_flight = target_flight.next_flight;
		}

	if (!target_data) 
	{
			CampLeaveCriticalSection();
			return;
	}

// find the shooter
	while (shooter_flight && !shooter_data)
		{
		if (shooter_flight.camp_id == wfm.dataBlock.fCampID)
			{
			shooter_data = FindPilotData(shooter_flight,wfm.dataBlock.fPilotID);
			break;
			}
		shooter_flight = shooter_flight.next_flight;
		}

	// Record a event
	theEvent = new EventElement();
	ParseTime(vuxGameTime,time_str);
	ReadIndexedString(target_data.aircraft_slot+1,pnum,4);
	//int weaponindx = WeaponDataTable[GetWeaponIdFromDescriptionIndex(wfm.dataBlock.fWeaponID-VU_LAST_ENTITY_TYPE)].Index;
	int weaponindx;
	for (int loop =0; loop<203;loop++)
	{
		weaponindx = WeaponDataTable[loop].Index +VuEntity.VU_LAST_ENTITY_TYPE;
		if (wfm.dataBlock.fWeaponID == weaponindx) break;
	}
if (shooter_data)
	sprintf(format,"%s %s launched %s at %s %s",shooter_flight.aircraft_name,shooter_data.pilot_callsign,WeaponDataTable[loop].Name,target_data.pilot_callsign ,time_str );
else
	sprintf(format,"%s launched at %s %s",WeaponDataTable[loop].Name,target_data.pilot_callsign ,time_str );

	ConstructOrderedSentence(MAX_EVENT_STRING_LEN, theEvent.eventString,format,target_data.pilot_callsign,target_flight.name,pnum,time_str);
	theEvent.eventTime = vuxGameTime;
	AddEventToList(theEvent,target_flight,0,0);

	Debug.Assert(player_pilot);

	if (target_data)
		target_data.shot_at++;
	CampLeaveCriticalSection();
			#endif
			throw 
				new NotImplementedException ();
		}

		public void RegisterShot (FalconWeaponsFire wfm)
		{
#if TODO
	string				time_str,format,target_name;
	PilotDataClass		pilot_data;
	EventElement		theEvent;
	int					wn,windex;
	FlightDataClass		flight_ptr;

	CampEnterCriticalSection();

	// Check if someone in the package fired this.
	flight_ptr = flight_data;
	while (flight_ptr)
		{
		if (!wfm.dataBlock.fCampID || wfm.dataBlock.fCampID != flight_ptr.camp_id)
			{
			flight_ptr = flight_ptr.next_flight;
			continue;
			}
		pilot_data = flight_ptr.pilot_list;
		while (pilot_data)
			{
			if (wfm.dataBlock.fPilotID == pilot_data.pilot_slot)
				{

				for (wn=0; wn<pilot_data.weapon_types; wn++)
					{
					windex = WeaponDataTable[pilot_data.weapon_data[wn].weapon_id].Index + VuEntity.VU_LAST_ENTITY_TYPE;
					if (wfm.dataBlock.fWeaponID == windex) // && wfm.dataBlock.fireOnOff == 1)
						{
						// We're interested
						theEvent = new EventElement();
						// Only subtract from our score for non guns and non-photos
						if (!(WeaponDataTable[pilot_data.weapon_data[wn].weapon_id].Flags & WEAP_ONETENTH) && wfm.dataBlock.weaponType != FalconWeaponsFire.Recon)
							pilot_data.score += CalcScore (SCORE_FIRE_WEAPON, windex - VuEntity.VU_LAST_ENTITY_TYPE);
						else if (CampaignClass.TheCampaign.Flags & CAMP_LIGHT)
							{
							// Don't record gun shots in IA and dogfight
							CampLeaveCriticalSection();
							return;
							}
						// Chalk up a weapon fired mark (assume a miss):
						pilot_data.weapon_data[wn].fired++;
						ParseTime(CampaignClass.TheCampaign.CurrentTime,time_str);
#if DEBUG
						// Try and find a bug where weapon shots are being doubled
						Debug.Assert (wfm.dataBlock.fWeaponUID.num_ != 0);
						EventElement *tmpevent = pilot_data.weapon_data[wn].root_event;
						while (tmpevent)
							{
							Debug.Assert (tmpevent.vuIdData1 != wfm.dataBlock.fWeaponUID);
							tmpevent = tmpevent.next;
							}
#endif
						// In case of camera "shots", evaluate hit immediately
						if (wfm.dataBlock.weaponType == FalconWeaponsFire.Recon)
							{
							FalconEntity	entity = (FalconEntity) VuDatabase.vuDatabase.Find(wfm.dataBlock.targetId);
							if (entity && entity.IsSim())
								{
								if (((SimBaseClass*)entity).GetCampaignObject().Id() == flight_ptr.target_id)
									flight_ptr.status_flags |= MISEVAL_FLIGHT_TARGET_HIT;
								if (EntityDB.Falcon4ClassTable[entity.Type() - VuEntity.VU_LAST_ENTITY_TYPE].dataType == DTYPE_VEHICLE)
									_stprintf(target_name,GetVehicleClassData(entity.Type() - VuEntity.VU_LAST_ENTITY_TYPE).Name);
								else if (EntityDB.Falcon4ClassTable[entity.Type() - VuEntity.VU_LAST_ENTITY_TYPE].dataType == DTYPE_FEATURE)
									_stprintf(target_name,FeatureStatic.GetFeatureClassData(entity.Type() - VuEntity.VU_LAST_ENTITY_TYPE).Name);
								pilot_data.weapon_data[wn].hit++;
								GetFormatString(FET_PHOTO_TAKEN_HIT,format);
								}
							else
								{
								pilot_data.weapon_data[wn].missed++;
								GetFormatString(FET_PHOTO_TAKEN_MISSED,format);
								}
							}
						else
							{
							pilot_data.weapon_data[wn].missed++;
							if (EntityDB.Falcon4ClassTable[wfm.dataBlock.fWeaponID-VuEntity.VU_LAST_ENTITY_TYPE].vuClassData.classInfo_[VU_TYPE] == TYPE_GUN)
								GetFormatString(FET_FIRED_MISSED,format);
							else
								GetFormatString(FET_RELEASED_MISSED,format);
							}
						ConstructOrderedSentence(MAX_EVENT_STRING_LEN, theEvent.eventString,format,pilot_data.weapon_data[wn].weapon_name,time_str,pilot_data.pilot_callsign,target_name);
						theEvent.vuIdData1 = wfm.dataBlock.fWeaponUID;
						theEvent.eventTime = CampaignClass.TheCampaign.CurrentTime;
						AddEventToList(theEvent,flight_ptr,pilot_data,wn);
						CampLeaveCriticalSection();
						return;
						}
					}
				}
			pilot_data = pilot_data.next_pilot;
			}
		flight_ptr = flight_ptr.next_flight;
		}
	CampLeaveCriticalSection();
#endif
            throw 
				new NotImplementedException ();
		}

		public void RegisterHit (FalconDamageMessage dmm)
		{
#if TODO
	string					time_str,format,tmp;
	PilotDataClass			pilot_data;
	int						wn,windex;
	VehicleClassDataType	vc;
	FeatureClassDataType	fc;
	FlightDataClass			flight_ptr;
	EventElement			tmpevent,baseevent=null;

// 2002-02-08 MN don't evaluate if FEAT_NO_HITEVAL (like trees...)
	if (EntityDB.Falcon4ClassTable[dmm.dataBlock.dIndex-VuEntity.VU_LAST_ENTITY_TYPE].dataType == DTYPE_FEATURE)
	{
		// get classtbl entry for feature
		fc = FeatureStatic.GetFeatureClassData (dmm.dataBlock.dIndex-VuEntity.VU_LAST_ENTITY_TYPE);
		if (fc && (fc.Flags & FEAT_NO_HITEVAL))
			return;
	}
	
	CampEnterCriticalSection();

	flight_ptr = flight_data;
	while (flight_ptr)
		{
		// Did we cause this damage?
		if (dmm.dataBlock.fCampID && dmm.dataBlock.fCampID == flight_ptr.camp_id)
			{
			pilot_data = flight_ptr.pilot_list;
			while (pilot_data)
				{
				if (dmm.dataBlock.fPilotID == pilot_data.pilot_slot)
					{
					for (wn=0; wn<pilot_data.weapon_types; wn++)
						{
						windex = WeaponDataTable[pilot_data.weapon_data[wn].weapon_id].Index + VuEntity.VU_LAST_ENTITY_TYPE;
						// Check if fired by this flight
						if (dmm.dataBlock.fWeaponID == windex)
							{
							int			foundEvent=false;
							// We hit with one of these weapons, match to a weapon fire event
							tmpevent = pilot_data.weapon_data[wn].root_event;
							while (tmpevent && !foundEvent)
								{
								if (tmpevent.vuIdData1 == dmm.dataBlock.fWeaponUID)
									{
									_TCHAR	*sptr;
									// Several options can have happened here:
									// 1) We haven't recorded the weapon hitting anything yet - 
									//    so we want to replace the 'miss' message with a 'hit' message
									// 3) The weapon hit something else and either damaged or destroyed
									//    it, and now another entity has been hit - so we want to make
									//    a new message with the result here.
									// 
									// Check for a miss event first (this is the easy case)
									ReadIndexedString(1799,tmp,79);
									sptr = _tcsstr(tmpevent.eventString,tmp);
									if (sptr)
										{
										// The first time we hit something with a weapon, we need to turn
										// a miss into a hit.
										if (GetRoE(GetTeam(dmm.dataBlock.fSide),GetTeam(dmm.dataBlock.dSide),ROE_AIR_FIRE) == ROE_ALLOWED)
											pilot_data.score += CalcScore (SCORE_HIT_ENEMY, 0);		// Hit enemy
										else
											pilot_data.score += CalcScore (SCORE_HIT_FRIENDLY, 0);		// Hit friendly/neutral
										pilot_data.weapon_data[wn].hit++;
										pilot_data.weapon_data[wn].missed--;
										foundEvent = true;
										*sptr = 0;
										}
									else
										{
										// Check for a hit event next
										ReadIndexedString(1798,tmp,79);
										sptr = _tcsstr(tmpevent.eventString,tmp);
										// We'd better have one or something is very wrong
										Debug.Assert(sptr);
										// Now determine if it's the same target or not
										if (sptr && tmpevent.vuIdData2 == dmm.dataBlock.dEntityID)
											{
											foundEvent = true;
											*sptr = 0;
											}
										}
									// If we haven't found the first event for this weapon yet, record
									// it now.
									if (!baseevent)
										baseevent = tmpevent;
									}
								if (!foundEvent)
									tmpevent = tmpevent.next;
								}
							// Make a new event if we need to
							if (!tmpevent)
								{
								// In IA or dogfight, don't list multiple hits
								if (CampaignClass.TheCampaign.Flags & CAMP_LIGHT)
									{
									CampLeaveCriticalSection();
									return;
									}
								tmpevent = new EventElement();
								tmpevent.vuIdData1 = dmm.dataBlock.fWeaponUID;
								tmpevent.eventTime = CampaignClass.TheCampaign.CurrentTime;
								ReadIndexedString(1726,tmpevent.eventString,MAX_EVENT_STRING_LEN);
								InsertEventToList (tmpevent, baseevent);
								}
							tmpevent.vuIdData2 = dmm.dataBlock.dEntityID;
							ParseTime(CampaignClass.TheCampaign.CurrentTime,time_str);
							if (EntityDB.Falcon4ClassTable[dmm.dataBlock.dIndex-VuEntity.VU_LAST_ENTITY_TYPE].dataType == DTYPE_VEHICLE)
								{
								vc = GetVehicleClassData (dmm.dataBlock.dIndex-VuEntity.VU_LAST_ENTITY_TYPE);
								if (vc) // JB 010113
									_stprintf(tmp,vc.Name);
								else
									_stprintf(tmp,"");
								}
							else if (EntityDB.Falcon4ClassTable[dmm.dataBlock.dIndex-VuEntity.VU_LAST_ENTITY_TYPE].dataType == DTYPE_FEATURE)
								{
								fc = FeatureStatic.GetFeatureClassData (dmm.dataBlock.dIndex-VuEntity.VU_LAST_ENTITY_TYPE);
								if (fc) // JB 010113
									_stprintf(tmp,fc.Name);
								else
									_stprintf(tmp,"");
								}
							// Add on the damaged message
							GetFormatString(FET_DAMAGED,format);
							ConstructOrderedSentence(128, time_str,format,tmpevent.eventString,tmp);
							_sntprintf(tmpevent.eventString,MAX_EVENT_STRING_LEN,time_str);
							CampLeaveCriticalSection();
							return;
							}
						}
					}
				pilot_data = pilot_data.next_pilot;
				}
			}
		// Did we take this damage?
		else if (dmm.dataBlock.dCampID && dmm.dataBlock.dCampID == flight_ptr.camp_id)
			{
			pilot_data = flight_ptr.pilot_list;
			while (pilot_data)
				{
				if (dmm.dataBlock.dPilotID == pilot_data.pilot_slot)
					{
					// Just mark us as damaged
					if (pilot_data.aircraft_status != VIS_TYPES.VIS_DESTROYED)
						pilot_data.aircraft_status = VIS_TYPES.VIS_DAMAGED;
					}				
				pilot_data = pilot_data.next_pilot;
				}
			}
		flight_ptr = flight_ptr.next_flight;
		}
	CampLeaveCriticalSection();
#endif
            throw 
				new NotImplementedException ();
		}

		public void RegisterKill (FalconDeathMessage dtm, int type, int pilot_status)
		{
			throw new NotImplementedException ();
		}

		public void RegisterPlayerJoin (FalconPlayerStatusMessage fpsm)
		{
#if TODO
	string					time_str,format,pnum;
	PilotDataClass			*pilot_data;
	EventElement			*theEvent;
	FlightDataClass			*flight_ptr;

	CampEnterCriticalSection();

	// Check if someone in the package fired this.
	flight_ptr = flight_data;
	while (flight_ptr)
		{
		if (flight_ptr.flight_id == FalconLocalSession.GetPlayerFlightID())
			flags |= MISEVAL_MISSION_IN_PROGRESS;
		pilot_data = flight_ptr.pilot_list;
		while (pilot_data)
			{
			if (fpsm.dataBlock.campID && fpsm.dataBlock.campID == flight_ptr.camp_id &&	
				fpsm.dataBlock.pilotID == pilot_data.pilot_slot)
				{
				// We're interested
				theEvent = new EventElement();
				ParseTime(vuxGameTime,time_str);
				ReadIndexedString(pilot_data.aircraft_slot+1,pnum,4);
				if (fpsm.dataBlock.state == PSM_STATE_ENTERED_SIM)
					GetFormatString(FET_PILOT_JOINED,format);
				else if (fpsm.dataBlock.state == PSM_STATE_LEFT_SIM)
					GetFormatString(FET_PILOT_EXITED,format);
				ConstructOrderedSentence(MAX_EVENT_STRING_LEN, theEvent.eventString,format,fpsm.dataBlock.callsign,flight_ptr.name,pnum,time_str);
				theEvent.eventTime = vuxGameTime;
				AddEventToList(theEvent,flight_ptr,0,0);

				{
					UI_SendChatMessage
						*chat;

					chat = new UI_SendChatMessage (FalconnullId, FalconLocalSession);

					chat.dataBlock.from = FalconnullId;
					chat.dataBlock.size = (strlen (theEvent.eventString) + 1) * sizeof (char);
					chat.dataBlock.message = new char [strlen (theEvent.eventString) + 1];
					memcpy (chat.dataBlock.message, theEvent.eventString, chat.dataBlock.size);
					FalcMesgStatic.FalconSendMessage (chat, true);
				}

				CampLeaveCriticalSection();
				return;
				}          
			pilot_data = pilot_data.next_pilot;
			}
		flight_ptr = flight_ptr.next_flight;
		}
	CampLeaveCriticalSection();
			#endif
			throw 
				new NotImplementedException ();
		}

		public void RegisterEjection (FalconEjectMessage em, int pilot_status)
		{
#if TODO
	string					time_str,format;
	PilotDataClass			*pilot_data;
	EventElement			*theEvent;
	FlightDataClass			*flight_ptr;

	CampEnterCriticalSection();

	// Check if someone in the package fired this.
	flight_ptr = flight_data;
	while (flight_ptr)
		{
		pilot_data = flight_ptr.pilot_list;
		while (pilot_data)
			{
			if (em.dataBlock.eCampID && em.dataBlock.eCampID == flight_ptr.camp_id &&	
				em.dataBlock.ePilotID == pilot_data.pilot_slot)
				{
				// We're interested
				theEvent = new EventElement();
				ParseTime(CampaignClass.TheCampaign.CurrentTime, time_str);
				GetFormatString(FET_PILOT_EJECTED,format);
				// Only record pilot status in non-dogfight games
				if (pilot_data.pilot_status == PILOT_IN_USE && FalconSessionEntity.FalconLocalGame.GetGameType() != game_Dogfight)
					pilot_data.pilot_status = pilot_status;
				pilot_data.aircraft_status = VIS_TYPES.VIS_DESTROYED;
				if (em.dataBlock.hadLastShooter)	// Ejected after taking damage
					{
					pilot_data.score += CalcScore (SCORE_VEHICLE_KILL, 0);			// Score vehicle death
					pilot_data.score += CalcScore (SCORE_PILOT_EJECTED_KILL, 0);	// Score the ejection
					}
				else								// Ejected undamaged
					{
					pilot_data.score += CalcScore (SCORE_GROUND_COLLISION, 0);		// Score ground collistion
					pilot_data.score += CalcScore (SCORE_PILOT_EJECTED, 0);		// Score the ejection
					if (pilot_data == player_pilot)
						logbook_data.Flags |= EJECT_UNDAMAGED;
					}
				ConstructOrderedSentence(MAX_EVENT_STRING_LEN, theEvent.eventString,format,pilot_data.pilot_callsign,time_str);
				theEvent.eventTime = CampaignClass.TheCampaign.CurrentTime;
				AddEventToList(theEvent,flight_ptr,0,0);
				CampLeaveCriticalSection();
				return;
				}
			pilot_data = pilot_data.next_pilot;
			}
		flight_ptr = flight_ptr.next_flight;
		}
	CampLeaveCriticalSection();
#endif
            throw 
				new NotImplementedException ();
		}

		public void RegisterLanding (FalconLandingMessage lm, int pilot_status)
		{
#if TODO
	string					time_str,format;
	PilotDataClass			pilot_data;
	EventElement			theEvent;
	FlightDataClass			flight_ptr;

	CampEnterCriticalSection();

	// Check if someone in the package fired this.
	flight_ptr = flight_data;
	while (flight_ptr)
		{
		pilot_data = flight_ptr.pilot_list;
		while (pilot_data)
			{
			if (lm.dataBlock.campID && lm.dataBlock.campID == flight_ptr.camp_id &&	
				lm.dataBlock.pilotID == pilot_data.pilot_slot)
				{
				// We're interested
				theEvent = new EventElement();
				ParseTime(CampaignClass.TheCampaign.CurrentTime, time_str);
				GetFormatString(FET_PILOT_LANDED,format);
				if (pilot_data.pilot_status == PILOT_IN_USE && FalconSessionEntity.FalconLocalGame.GetGameType() != game_Dogfight)
					pilot_data.pilot_status = pilot_status;
//				pilot_data.aircraft_status = VIS_TYPES.VIS_DESTROYED;
				pilot_data.score += CalcScore (SCORE_PILOT_LANDED, 0);
				if (pilot_data == player_pilot)
					logbook_data.Flags |= LANDED_AIRCRAFT;
				ConstructOrderedSentence(MAX_EVENT_STRING_LEN, theEvent.eventString,format,pilot_data.pilot_callsign,time_str);
				theEvent.eventTime = CampaignClass.TheCampaign.CurrentTime;
				AddEventToList(theEvent,flight_ptr,0,0);
				CampLeaveCriticalSection();
				return;
				}
			pilot_data = pilot_data.next_pilot;
			}
		flight_ptr = flight_ptr.next_flight;
		}
	CampLeaveCriticalSection();
#endif
            throw 
				new NotImplementedException ();
		}

		public void RegisterContact (Unit contact)
		{
#if TODO
	ulong		score;
	// KCK NOTE: This assumes that Register Contact is only called once per contact!

	// KCK: This only scores threats currently.
	// Possibly, the score should vary by mission type
	// i.e: SEAD missions score Air Defenses highest
	//		ESCORT score Air to Air threats highest
	//		BARCAP score enemy attack/bomber aircraft highest
	score = contact.GetHitChance(LowAir,0);
	if (contact.IsFlight())
		score *= contact.GetTotalVehicles();
	if (contact.Broken())
		score /= 4;
	contact_score += score;
			#endif
			throw new NotImplementedException ();
		}

		public void ParseTime (CampaignTime time, string time_str)
		{
			// TODO GetTimeString (time, time_str);
			throw new NotImplementedException ();
		}

		public void ParseTime (double time, string time_str)
		{
			//TODO GetTimeString (FloatToInt32 ((float)time * VU_TICS_PER_SECOND), time_str);
			throw new NotImplementedException ();
		}

		public void AddEventToList (EventElement theEvent, FlightDataClass flight_ptr, PilotDataClass pilot_data, int wn)
		{
#if TODO
	EventElement* curEvent = null;

	theEvent.next = null;
	if (pilot_data)
		{
		pilot_data.weapon_data[wn].events++;
		if (!pilot_data.weapon_data[wn].root_event)
			pilot_data.weapon_data[wn].root_event = theEvent;
		else
			curEvent = pilot_data.weapon_data[wn].root_event;
		}
	else
		{
		flight_ptr.events++;
		if (!flight_ptr.root_event)
			flight_ptr.root_event = theEvent;
		else
			curEvent = flight_ptr.root_event;
		}
	if (!curEvent)							// Added directly to root, we're done
		return;

	while (curEvent.next && curEvent.next != curEvent)
		curEvent = curEvent.next;

	Debug.Assert (curEvent != theEvent);

	curEvent.next = theEvent;
			#endif
			throw 
				new NotImplementedException ();
		}

	
		// Event trigger checks
		public void RegisterKill (FalconEntity shooter, FalconEntity target, int targetEl)
		{
#if TODO
			int sid, tid, i;
			CampEntity campTarget, campShooter;
			FlightDataClass flight_ptr;

			if (!(flags & MISEVAL_MISSION_IN_PROGRESS))
				return;

			CampEnterCriticalSection ();

			// Get their indexes
			tid = target.Type () - VuEntity.VU_LAST_ENTITY_TYPE;
			sid = shooter.Type () - VuEntity.VU_LAST_ENTITY_TYPE;

			// Get campaign objects
			if (target.IsCampaign ())
				campTarget = (CampBaseClass)target;
			else {
				campTarget = ((SimBaseClass)target).GetCampaignObject ();
				targetEl = ((SimBaseClass)target).GetSlot ();
			}
			if (shooter.IsCampaign ())
				campShooter = (CampBaseClass)shooter;
			else
				campShooter = ((SimBaseClass)shooter).GetCampaignObject ();

			// Check for hits by us
			if (campShooter.IsFlight () && campShooter.InPackage ()) {
				flight_ptr = FindFlightData ((FlightClass)campShooter);

				if (flight_ptr) {
					if (EntityDB.Falcon4ClassTable [tid].vuClassData.classInfo_ [(int)VU_CLASS.VU_DOMAIN] == DOMAIN_AIR && GetRoE (campShooter.GetTeam (), campTarget.GetTeam (), ROE_AIR_FIRE))
						flight_ptr.status_flags |= MISEVAL_FLIGHT_GOT_AKILL;
					else if (EntityDB.Falcon4ClassTable [tid].vuClassData.classInfo_ [(int)VU_CLASS.VU_DOMAIN] == DOMAIN_LAND && GetRoE (campShooter.GetTeam (), campTarget.GetTeam (), ROE_GROUND_FIRE)) {
						if (EntityDB.Falcon4ClassTable [tid].dataType == DTYPE_OBJECTIVE || Falcon4ClassTable [tid].dataType == DTYPE_FEATURE) {
							flight_ptr.status_flags |= MISEVAL_FLIGHT_GOT_SKILL;
							// High value for hitting target feature
							for (i=0; i<MAX_TARGET_FEATURES; i++) {
								if (flight_ptr.target_features [i] < 255 && flight_ptr.target_features [i] == targetEl)
									flight_ptr.status_flags |= MISEVAL_FLIGHT_HIT_HIGH_VAL;
							}
						} else if (EntityDB.Falcon4ClassTable [tid].dataType == DTYPE_UNIT || Falcon4ClassTable [tid].dataType == DTYPE_VEHICLE) {
							flight_ptr.status_flags |= MISEVAL_FLIGHT_GOT_GKILL;
							// High value for killing radar vehicle
							if (targetEl == ((UnitClass*)campTarget).GetUnitClassData ().RadarVehicle)
								flight_ptr.status_flags |= MISEVAL_FLIGHT_HIT_HIGH_VAL;
							// For most air to ground missions, we're ok as long as we've hit the correct brigade
							if (flight_ptr.target_id && (flight_ptr.target_id == ((UnitClass*)campTarget).GetUnitParentID () || flight_ptr.target_id == campTarget.Id ()))
								flight_ptr.status_flags |= MISEVAL_FLIGHT_TARGET_HIT;
						}
					} else if (EntityDB.Falcon4ClassTable [tid].vuClassData.classInfo_ [(int)VU_CLASS.VU_DOMAIN] == DOMAIN_SEA && GetRoE (campShooter.GetTeam (), campTarget.GetTeam (), ROE_NAVAL_FIRE)) {
						flight_ptr.status_flags |= MISEVAL_FLIGHT_GOT_NKILL;
						// High value for hitting capital ship
						if (!targetEl)
							flight_ptr.status_flags |= MISEVAL_FLIGHT_HIT_HIGH_VAL;
					}

					if (campTarget.Id () == flight_ptr.target_id || campTarget.GetCampID () == flight_ptr.target_camp_id)
						flight_ptr.status_flags |= MISEVAL_FLIGHT_TARGET_HIT;
					if (campTarget.IsUnit () && ((UnitClass*)campTarget).Dead ())
						flight_ptr.status_flags |= MISEVAL_FLIGHT_TARGET_KILLED;
				}
			}

			// Check for hits against us
			if (campTarget.IsFlight () && campTarget.InPackage ()) {
				flight_ptr = FindFlightData ((FlightClass*)campTarget);

				if (flight_ptr) {
					flight_ptr.status_flags |= MISEVAL_FLIGHT_LOSSES;
					if (campTarget.IsUnit () && ((UnitClass*)campTarget).Dead ())
						flight_ptr.status_flags |= MISEVAL_FLIGHT_DESTROYED;
					if (EntityDB.Falcon4ClassTable [sid].vuClassData.classInfo_ [(int)VU_CLASS.VU_DOMAIN] == DOMAIN_AIR)
						flight_ptr.status_flags |= MISEVAL_FLIGHT_HIT_BY_AIR;
					else if (EntityDB.Falcon4ClassTable [sid].vuClassData.classInfo_ [(int)VU_CLASS.VU_DOMAIN] == DOMAIN_LAND)
						flight_ptr.status_flags |= MISEVAL_FLIGHT_HIT_BY_GROUND;
					else if (EntityDB.Falcon4ClassTable [sid].vuClassData.classInfo_ [(int)VU_CLASS.VU_DOMAIN] == DOMAIN_SEA)
						flight_ptr.status_flags |= MISEVAL_FLIGHT_HIT_BY_NAVAL;

					if (package_element && campTarget.Id () == package_element.flight_id) {
						// Package element hit - set flags on all elements
						FlightDataClass		* tmp_ptr;
						int copyFlags;

						copyFlags = MISEVAL_FLIGHT_F_TARGET_HIT;
						if (flight_ptr.status_flags & MISEVAL_FLIGHT_DESTROYED)
							copyFlags |= MISEVAL_FLIGHT_F_TARGET_KILLED;
						tmp_ptr = flight_data;
						while (tmp_ptr) {
							tmp_ptr.status_flags |= copyFlags;
							tmp_ptr = tmp_ptr.next_flight;
						}
					}
				}
			}

			// Check for area hits
			if (flags & MISEVAL_EVALUATE_HITS_IN_AREA) {
				GridIndex cx, cy;
				campTarget.GetLocation (&cx, &cy);
				//if (DistSqu(tx,ty,cx,cy) < 0.5F * STATION_DIST_LENIENCY*STATION_DIST_LENIENCY) // JB 010215
				if (DistSqu (tx, ty, cx, cy) < 0.5F * STATION_DIST_HITS_LENIENCY * STATION_DIST_HITS_LENIENCY) { // JB 010215
					// Area hit - set flags on all elements which were covering VOL
					FlightDataClass		* tmp_ptr;
					Flight flight;
					tmp_ptr = flight_data;
					while (tmp_ptr) {
						flight = (Flight)VuDatabase.vuDatabase.Find (tmp_ptr.flight_id);
						if (flight.GetEvalFlags () & FEVAL_ON_STATION)
							tmp_ptr.status_flags |= MISEVAL_FLIGHT_F_AREA_HIT;
						tmp_ptr = tmp_ptr.next_flight;
					}
				}
			}

			CampLeaveCriticalSection ();
#endif
            throw 
				new NotImplementedException ();
		}

		public void RegisterMove (Flight flight)
		{
#if TODO
			WayPoint w, nw;
			GridIndex fx, fy, wx, wy;
			float ds, cds = 1000000.0F;
			int feflags, meflags = 0;
			CampaignTime now = CampaignClass.TheCampaign.CurrentTime;
			FlightDataClass	* flight_ptr = null;

			Debug.Assert (flight);
			if (!flight)
				return;

			CampEnterCriticalSection ();

			// Determine stuff about the flight
			flight.GetLocation (&fx, &fy);
			feflags = flight.GetEvalFlags ();
			if ((flags & MISEVAL_MISSION_IN_PROGRESS) && flight.InPackage ()) {
				// Detailed check for flights in our package
				flight_ptr = FindFlightData (flight);
				if (flight_ptr)
					meflags = flight_ptr.status_flags;
				else
					flight.SetInPackage (0);
			}

			// Check for friendly territory
			if (GetOwner (CampaignClass.TheCampaign.CampMapData, fx, fy) == flight.GetTeam ()) {
				// Check for getting home if we've been at our target, and either our mission time is over, or we've been relieved
				if (meflags & MISEVAL_FLIGHT_GOT_TO_TARGET && (meflags & MISEVAL_FLIGHT_STATION_OVER || meflags & MISEVAL_FLIGHT_RELIEVED))
					meflags |= MISEVAL_FLIGHT_GOT_HOME;
			} else {
				// Set mission started if we're over enemy territory
				feflags |= FEVAL_MISSION_STARTED;
			}

			// Check waypoint related stuff
			w = flight.GetFirstUnitWP ();
			while (w) {
				w.GetWPLocation (&wx, &wy);
				if (w.GetWPFlags () & WPF_TARGET) {
// 2002-02-13 MN check if target got occupied by us and has not been engaged yet - for 2D flights
// 2002-03-03 MN fix - only for strike missions
					if (w.GetWPTarget () && w.GetWPTarget ().GetTeam () == flight.GetTeam () &&
				(flight.GetUnitMission () > MissionTypeEnum.AMIS_SEADESCORT && flight.GetUnitMission () < MissionTypeEnum.AMIS_FAC))
						meflags |= MISEVAL_FLIGHT_ABORT_BY_AWACS;
					// Determine station times
					CampaignTime tont, tofft;
					ds = (float)DistSqu (fx, fy, wx, wy);
					tont = w.GetWPArrivalTime ();
					tofft = w.GetWPDepartureTime ();
					nw = w.GetNextWP ();
					if (nw && nw.GetWPFlags () & WPF_TARGET)
						tofft = nw.GetWPDepartureTime ();
					// Determine if we're in our VOL or not
					if (now > tont && now < tofft)
						feflags |= FEVAL_ON_STATION;
			// Check if our VOL time is over
					else if (now > tofft) {
						feflags &= ~FEVAL_ON_STATION;
						meflags |= MISEVAL_FLIGHT_STATION_OVER;
					}
					// Determine if we're close enough for government work
					if (now > tont + STATION_TIME_LENIENCY && now < tofft - STATION_TIME_LENIENCY && ds > STATION_DIST_LENIENCY * STATION_DIST_LENIENCY)
						meflags |= MISEVAL_FLIGHT_OFF_STATION;
					// Check if we got to our target
					if (ds < TARGET_DIST_LENIENCY * TARGET_DIST_LENIENCY) {
						if (flight_ptr && flight_ptr == player_element && !(meflags & MISEVAL_FLIGHT_GOT_TO_TARGET))
							actual_tot = CampaignClass.TheCampaign.CurrentTime;
						feflags |= FEVAL_GOT_TO_TARGET;
						meflags |= MISEVAL_FLIGHT_GOT_TO_TARGET;
						// Set mission started
						feflags |= FEVAL_MISSION_STARTED;
					}
				}
				w = w.GetNextWP ();
			}

			// copy the flags back in
			flight.SetEvalFlag (feflags, 1); // 2002-02-19 MODIFIED BY S.G. Added the 1 at then end of the function to specify feflags contains all the flags.
			if (flight_ptr)
				flight_ptr.status_flags = meflags;

			CampLeaveCriticalSection ();
		#endif
			throw 
				new NotImplementedException ();
		}

		public void RegisterAbort (Flight flight)
		{
#if TODO
			FlightDataClass		* flight_ptr;
			int copyFlags = 0;

			if (!(flags & MISEVAL_MISSION_IN_PROGRESS))
				return;

			if (package_element && package_element.flight_id == flight.Id ())
				copyFlags = MISEVAL_FLIGHT_F_TARGET_ABORTED;

			CampEnterCriticalSection ();
			flight_ptr = flight_data;
			while (flight_ptr) {
				if (flight_ptr.flight_id == flight.Id ())
					flight_ptr.status_flags |= MISEVAL_FLIGHT_ABORTED;
				if (flight_ptr.target_id == flight.Id ())
					flight_ptr.status_flags |= MISEVAL_FLIGHT_TARGET_ABORTED;
				flight_ptr.status_flags |= copyFlags;
				flight_ptr = flight_ptr.next_flight;
			}
			CampLeaveCriticalSection ();
	#endif
			throw 
				new NotImplementedException ();
		}

		public void RegisterRelief (Flight flight)
		{
#if TODO
			FlightDataClass		* flight_ptr;

			if (!(flags & MISEVAL_MISSION_IN_PROGRESS))
				return;

			CampEnterCriticalSection ();
			flight_ptr = FindFlightData (flight);
			if (flight_ptr)
				flight_ptr.status_flags |= MISEVAL_FLIGHT_RELIEVED;
			CampLeaveCriticalSection ();
	#endif
			throw 
			new NotImplementedException ();
		}

		public void RegisterDivert (Flight flight, MissionRequestClass mis)
		{
#if TODO
			FlightDataClass		 flight_ptr;
			CampEntity target = null;

			if (!(flags & MISEVAL_MISSION_IN_PROGRESS))
				return;

			CampEnterCriticalSection ();
			flight_ptr = FindFlightData (flight);
			if (flight_ptr) {
				// KCK TODO: Convert this flight's mission to the divert mission
				flight_ptr.mission = mis.mission;
				if (flight_ptr == package_element) {
					// Trying to track down a potential bug here.. It's hard enough to
					// get diverts I figure I'll let QA do the testing..
					// Debug.Assert (!"Show this to Kevin K."); - Not any more - RH

					flight_ptr.mission_context = flight.GetMissionContext ();
					flight_ptr.requester_id = flight.GetRequesterID ();
					flight_ptr.target_id = flight.GetUnitMissionTargetID ();
					Debug.Assert (flight_ptr.target_id == mis.targetID);
					if (flight_ptr.target_id != FalconnullId)
						target = (CampEntity)VuDatabase.vuDatabase.Find (flight_ptr.target_id);
					RecordTargetStatus (flight_ptr, target);
				}
			}
			CampLeaveCriticalSection ();
				#endif
			throw 
				new NotImplementedException ();
		}

		public void RegisterRoundWon (int team)
		{
			rounds_won [team]++;
		}

		public void RegisterWin (int team)
		{
#if TODO
			// This team won (-1 means player with highest score won)
			FlightDataClass		* flight_ptr;
			PilotDataClass		* pilot_data;
			int best = GetMaxScore ();

			CampEnterCriticalSection ();
			flags |= MISEVAL_GAME_COMPLETED;
			flight_ptr = flight_data;
			while (flight_ptr) {
				if (team == -1 || flight_ptr.flight_team == team) {
					pilot_data = flight_ptr.pilot_list;
					while (pilot_data) {
						if (team != -1)
							pilot_data.pilot_flags |= PFLAG_WON_GAME;
						else if (pilot_data.score >= best)
							pilot_data.pilot_flags |= PFLAG_WON_GAME;
						if (pilot_data.player_kills)
							flags |= MISEVAL_ONLINE_GAME;
						pilot_data = pilot_data.next_pilot;
					}
				}
				flight_ptr = flight_ptr.next_flight;
			}
			CampLeaveCriticalSection ();
	#endif
			throw 
				new NotImplementedException ();
		}

		public void RegisterEvent (GridIndex x, GridIndex y, int team, int type, string evt)
		{
#if TODO
			int role;

			if (!(flags & MISEVAL_MISSION_IN_PROGRESS))
				return;

			if (team != eteam)
				return;

			// Filter out types by role
			role = MissionData [package_mission].skill;
			if (type != FalconCampEventMessage.campStrike) {
				if (role == ARO_CA && type != FalconCampEventMessage.campAirCombat)
					return;
				else if (role == ARO_GA && type != FalconCampEventMessage.campGroundAttack)
					return;
			}

			if (DistSqu (x, y, tx, ty) < RELATED_EVENT_RANGE_SQ) {
				// Check if already in our list
				for (int i=0; i<MAX_RELATED_EVENTS; i++) {
					if (related_events [i] && _tcscmp (related_events [i], evnt) == 0)
						return;
				}
				if (related_events [last_related_event])
					delete (related_events [last_related_event]);
		#if USE_SH_POOLS
		related_events[last_related_event] = (_TCHAR *)MemAllocPtr( gTextMemPool, sizeof(_TCHAR)*(_tcslen(event)+1), false );
		#else
				related_events [last_related_event] = new _TCHAR[_tcslen (evnt) + 1];
		#endif
				strcpy (related_events [last_related_event], evnt);
				last_related_event++;
				if (last_related_event >= MAX_RELATED_EVENTS)
					last_related_event = 0;
			}
				#endif
			throw 
				new NotImplementedException ();
		}

	
		// Register 3D AWACS abort call
		public void Register3DAWACSabort (Flight flight)
		{
#if TODO
			FlightDataClass		* flight_ptr;
			int copyFlags = 0;

			if (!(flags & MISEVAL_MISSION_IN_PROGRESS))
				return;

			if (package_element && package_element.flight_id == flight.Id ())
				copyFlags = MISEVAL_FLIGHT_F_TARGET_ABORTED;

			CampEnterCriticalSection ();
			flight_ptr = flight_data;
			while (flight_ptr) {
				if (flight_ptr.flight_id == flight.Id ())
					flight_ptr.status_flags |= MISEVAL_FLIGHT_ABORTED;
				if (flight_ptr.target_id == flight.Id ())
					flight_ptr.status_flags |= MISEVAL_FLIGHT_TARGET_ABORTED;
				flight_ptr.status_flags |= copyFlags;
				flight_ptr = flight_ptr.next_flight;
			}
			CampLeaveCriticalSection ();
				#endif
			throw 
				new NotImplementedException ();
		}
	
		public void SetFinalAircraft (Flight flight)
		{
#if TODO
			short campId, numAC, i;
			FlightDataClass		* flight_ptr;

			CampEnterCriticalSection ();

			campId = flight.GetCampID ();
			numAC = 0;
			flight_ptr = flight_data;
			while (flight_ptr) {
				if (flight_ptr.camp_id == campId) {
					// Count remaining aircraft
					for (i=0; i<PilotStatic.PILOTS_PER_FLIGHT; i++) {
						if (flight.plane_stats [i] == AIRCRAFT_AVAILABLE)
							numAC++;
					}
					// Record remaining aircraft
					flight_ptr.finish_aircraft = static_cast<byte> (numAC);
				}		
				flight_ptr = flight_ptr.next_flight;
			}

			CampLeaveCriticalSection ();
	#endif
			throw 
				new NotImplementedException ();
		}
	
		public int GetPilotName (int pilot_num, ref string buffer)
		{
#if TODO
			PilotDataClass	* pilot_ptr = null;
			int retval = 0;

			CampEnterCriticalSection ();

			if (player_element) {
				pilot_ptr = player_element.pilot_list;

				if (pilot_ptr) {
					while (pilot_ptr && pilot_ptr.pilot_slot != pilot_num)
						pilot_ptr = pilot_ptr.next_pilot;
				}
			}
			if (pilot_ptr && pilot_ptr.pilot_id != NO_PILOT) {
				_tcscpy (buffer, pilot_ptr.pilot_name);
				retval = 1;
			} else
				_tcscpy (buffer, "");

			CampLeaveCriticalSection ();

			return retval;
	#endif
			throw 
				new NotImplementedException ();
		}

		public int GetFlightName (ref string buffer)
		{
#if TODO
			int retval = 0;

			CampEnterCriticalSection ();

			if (player_element) {
				_tcscpy (buffer, player_element.name);
				retval = 1;
			} else
				_tcscpy (buffer, "");

			CampLeaveCriticalSection ();

			return retval;
	#endif
			throw 
				new NotImplementedException ();
		}
	
		public PilotDataClass AddNewPlayerPilot (FlightDataClass flight_ptr, int pilot_num, Flight flight, FalconSessionEntity player)
		{
#if TODO
			if (player == null)
				return null;

#if FUNKY_KEVIN_DEBUG_STUFF
	Debug.Assert(!inMission || player != FalconLocalSession);
#endif

			// Tack on a new slot
			PilotDataClass	* pilot_data = AddNewPilot (flight_ptr, player.GetPilotSlot (), ac_num, flight);
			sprintf (pilot_data.pilot_name, player.GetPlayerName ());
			sprintf (pilot_data.pilot_callsign, player.GetPlayerCallsign ());
			pilot_data.pilot_flags |= PFLAG_PLAYER_CONTROLLED;
			if (player == FalconLocalSession) {
				player_pilot = pilot_data;
				player_aircraft_slot = ac_num;
			}
			return pilot_data;
				#endif
			throw 
				new NotImplementedException ();
		}

		public PilotDataClass AddNewPilot (FlightDataClass flight_ptr, int pilot_num, int ac_num, Flight flight)
		{
#if TODO
			PilotDataClass	* pilot_data = new PilotDataClass ();
			PilotDataClass	* prev_pilot = null;
			int k = 0, w = 0, wid = 0, wi = 0, nw = 0, new_weap = 0, load = 0;
			LoadoutStruct	* loadout = null;

			// Assign a new pilot_slot
			pilot_data.pilot_slot = pilot_num;
			flight_ptr.num_pilots++;

			// Insert it into the list properly
			prev_pilot = flight_ptr.pilot_list;
			while (prev_pilot && prev_pilot.next_pilot && prev_pilot.next_pilot.pilot_slot <= pilot_data.pilot_slot)
				prev_pilot = prev_pilot.next_pilot;

			if (prev_pilot) {
				pilot_data.next_pilot = prev_pilot.next_pilot;
				prev_pilot.next_pilot = pilot_data;
			} else {
				flight_ptr.pilot_list = pilot_data;
				pilot_data.next_pilot = null;
			}

			pilot_data.aircraft_slot = ac_num;
			if (ac_num == pilot_num)
				pilot_data.pilot_id = flight.GetPilotID (ac_num);
			else
				pilot_data.pilot_id = -1;							// Player Pilot
			pilot_data.pilot_status = PILOT_IN_USE;		
			for (k=0; k<(CampWeapons.HARDPOINT_MAX/2)+2; k++) {
				pilot_data.weapon_data [k].root_event = null;
				pilot_data.weapon_data [k].events = 0;
			}
			// Check for player defined loadouts..
			loadout = flight.GetLoadout (ac_num);
			// Move to constructor
//	memset(pilot_data.weapon_data,0,sizeof(WeaponDataClass)*(HARDPOINT_MAX/2)+1);
			for (nw=0,w=0; w<CampWeapons.HARDPOINT_MAX; w++) {
				wid = flight.GetUnitWeaponId (w, ac_num);
				if (!wid)
					continue;
				wi = 0;

/*		// JB 010104 Marco Edit 
		// Check if LAU3A - if so set to 2.75in FFAR
		if (wid == 71)
			wid = 163 ;
		// Check if UB-38-57 or UB-19-57 - if so set to 57mm S5 Rocket
		if (wid == 93 || wid == 94)
			wid = 163; //wid = 181 ;
		// JB 010104 Marco Edit 
*/

// 2002-04-14 MN use RocketDataType instead of hack
				bool entryfound = false;
				for (int j=0; j<NumRocketTypes; j++) {
					if (wid == RocketDataTable [j].weaponId) {
						if (RocketDataTable [j].nweaponId) // 0 = don't change weapon ID
							wid = RocketDataTable [j].nweaponId;
						entryfound = true;
						break;
					}
				}
// 2002-04-16 MN sh*t... what should that do here ? copy and paste error...
/*		if (!entryfound)	// use generic 2.75mm rocket
		{
			wid = gRocketId;
		}
*/
				for (k=CampWeapons.HARDPOINT_MAX/2+1; k>=0; k--) {
					if (pilot_data.weapon_data [k].weapon_id == wid) {
						wi = k;
						new_weap = 0;
					}
					if (!pilot_data.weapon_data [k].weapon_id) {
						wi = k;
						new_weap = 1;
					}
				}
				if (new_weap) {
					nw++;
					_stprintf (pilot_data.weapon_data [wi].weapon_name, WeaponDataTable [wid].Name);
					pilot_data.weapon_data [wi].weapon_id = wid;
				}
				load = flight.GetUnitWeaponCount (w, ac_num);
				if (WeaponDataTable [wid].Flags & WEAP_ONETENTH)
					pilot_data.weapon_data [wi].starting_load += 10 * load;
				else
					pilot_data.weapon_data [wi].starting_load += load;
			}
			pilot_data.weapon_types = nw;

			return pilot_data;
				#endif
			throw 
				new NotImplementedException();
		}

		public PilotDataClass FindPilotData (FlightDataClass flight_ptr, int pilot_num)
		{
#if TODO
			PilotDataClass	* pilot_data;

			CampEnterCriticalSection ();

			pilot_data = flight_ptr.pilot_list;
			while ((pilot_data) && (pilot_data.pilot_slot != pilot_num))
				pilot_data = pilot_data.next_pilot;

			CampLeaveCriticalSection ();
			return pilot_data;
#endif
			throw 
				new NotImplementedException();
		}

		public PilotDataClass FindPilotData (int flight_id, int pilot_num)
		{
#if TODO
			FlightDataClass			* flight_ptr;
			PilotDataClass			* retval = null;

			CampEnterCriticalSection ();

			flight_ptr = flight_data;
			while (flight_ptr && !retval) {
				if (flight_ptr.camp_id == flight_id)
					retval = FindPilotData (flight_ptr, pilot_num);
				flight_ptr = flight_ptr.next_flight;
			}

			CampLeaveCriticalSection ();
			return retval;
				#endif
			throw 
				new NotImplementedException();
		}

		public PilotDataClass FindPilotDataFromAC (FlightDataClass flight_ptr, int aircraft_slot)
		{
#if TODO
			// Find the current pilot for this aircraft/flight combo.
			PilotDataClass * pilot_data = null;
			PilotDataClass * ret_data = null;

			if (!flight_ptr) // JB 010628 CTD
				return null;

			CampEnterCriticalSection ();

			// Since several players can have the same ac number, we take the last we find.
			pilot_data = flight_ptr.pilot_list;
			while (pilot_data) {
				if (pilot_data.aircraft_slot == aircraft_slot)
					ret_data = pilot_data;
				pilot_data = pilot_data.next_pilot;
			}

			CampLeaveCriticalSection ();
			return ret_data;
	#endif
			throw 
				new NotImplementedException();
		}
	
		public FlightDataClass FindFlightData (Flight flight)
		{
#if TODO
			FlightDataClass	* flight_ptr;

			CampEnterCriticalSection ();

			flight_ptr = CampaignClass.TheCampaign.MissionEvaluator.flight_data;
			while (flight_ptr) {
				if (flight_ptr.flight_id == flight.Id ()) {
					CampLeaveCriticalSection ();
					return flight_ptr;
				}
				flight_ptr = flight_ptr.next_flight;
			}

			CampLeaveCriticalSection ();
			return null;
#endif
			throw 
				new NotImplementedException();
		}
	
		public void SetupPilots (FlightDataClass flight_ptr, Flight flight)
		{
#if TODO		
			int i, p;
			FalconSessionEntity	* session;

			CampEnterCriticalSection ();

			if (FalconSessionEntity.FalconLocalGame.GetGameType () == game_Dogfight) {
				// Add all pilots
				for (i=0; i<PilotStatic.PILOTS_PER_FLIGHT; i++) {
					if (flight.plane_stats [i] == AIRCRAFT_AVAILABLE) {
						session = gCommsMgr.FindCampaignPlayer (flight.Id (), i);
						if (session)
							AddNewPlayerPilot (flight_ptr, i, flight, session);
						else {
							PilotDataClass	* pilot_data = AddNewPilot (flight_ptr, i, i, flight);
							// AI Pilots named by callsign
							_stprintf (pilot_data.pilot_name, "%s%d", flight_ptr.name, i + 1);
							_stprintf (pilot_data.pilot_callsign, "%s%d", flight_ptr.name, i + 1);
						}
					}
				}
			} else {
				// Add all default pilots
				for (i=0; i<PilotStatic.PILOTS_PER_FLIGHT; i++) {
					if (flight.plane_stats [i] == AIRCRAFT_AVAILABLE) {
						PilotDataClass	* pilot_data = AddNewPilot (flight_ptr, i, i, flight);
						p = flight.GetPilotID (i);
						GetPilotName (p, pilot_data.pilot_name, 29);
						_stprintf (pilot_data.pilot_callsign, "%s%d", flight_ptr.name, i + 1);
					}
				}
				// Now add any current player pilots
				for (i=0; i<PilotStatic.PILOTS_PER_FLIGHT; i++) {
					if (flight.plane_stats [i] == AIRCRAFT_AVAILABLE) {
						session = gCommsMgr.FindCampaignPlayer (flight.Id (), i);
						if (session)
							AddNewPlayerPilot (flight_ptr, i, flight, session);
					}
				}
			}
			CampLeaveCriticalSection ();
		#endif
			throw 
				new NotImplementedException();
		}

		public void SetPlayerPilot (Flight flight, byte aircraft_slot)
		{
#if TODO	
			Flight element;
			FlightDataClass		* flight_ptr;

			// KCK: Currently, this function isn't being called.
			CampEnterCriticalSection ();
			CleanupPilotData ();

			flight_ptr = flight_data;
			while (flight_ptr) {
				element = (Flight)FindStatic.FindUnit (flight_ptr.flight_id);
				if (element && flight_ptr.camp_id) {
					if (element == flight)
						player_element = flight_ptr;
					flight_ptr.num_pilots = 0;
					SetupPilots (flight_ptr, element);
				} else
					flight_ptr.camp_id = 0;		
				flight_ptr = flight_ptr.next_flight;
			}
			CampLeaveCriticalSection ();
		#endif
			throw 
				new NotImplementedException();
		}

		static int			evalCount = 0;

		public void RebuildEvaluationData ()
		{
#if TODO
			// KCK: This function will traverse all our data structures, throwing out any which 
			// we don't need anymore and adding any new ones which have popped up

			// NOTE: Dogfight only
			if (FalconSessionEntity.FalconLocalGame == null || FalconSessionEntity.FalconLocalGame.GetGameType () != game_Dogfight)
				return;
			else {
				VuListIterator flit = new VuListIterator (AllAirList);
				Unit uelement;
				FlightDataClass flight_ptr, last_ptr = null, tmp_ptr;
				PilotDataClass pilot_data, last_pilot, tmp_pilot;
				Flight flight;
				int i, kill;
		
				FalconSessionEntity session;

				//TODO Debug.Assert(flight_data == null || false == F4IsBadReadPtr(flight_data, sizeof *flight_data));
				if (flight_data && F4IsBadReadPtr (flight_data, sizeof(FlightDataClass))) // JB 010305 CTD
					return;

				CampEnterCriticalSection ();

				// Traverse all our lists and remove anything we don't have references to anymore
				flight_ptr = flight_data;
				while (flight_ptr) {
					flight = (Flight)FindStatic.FindUnit (flight_ptr.flight_id);
					if (!flight) {
						if (last_ptr)
							last_ptr.next_flight = flight_ptr.next_flight;
						else
							flight_data = flight_ptr.next_flight;
						tmp_ptr = flight_ptr;
						if (player_element == tmp_ptr)
							player_element = null;
						if (package_element == tmp_ptr)
							package_element = null;
						flight_ptr = flight_ptr.next_flight;
						delete (tmp_ptr);
					} else {
						// Check for team change
						flight_ptr.flight_team = flight.GetTeam ();
						// Check pilots
						last_pilot = null;
						pilot_data = flight_ptr.pilot_list;
						while (pilot_data) {
							kill = 0;
							if (flight.pilots [pilot_data.aircraft_slot] == NO_PILOT && flight.player_slots [pilot_data.aircraft_slot] == NO_PILOT)
								kill++;		// Neither slot
							else if (flight.pilots [pilot_data.aircraft_slot] != NO_PILOT && flight.player_slots [pilot_data.aircraft_slot] == NO_PILOT && pilot_data.pilot_slot != pilot_data.aircraft_slot)
								kill++;		// Player in AI slot
							else if (flight.player_slots [pilot_data.aircraft_slot] != NO_PILOT && pilot_data.pilot_slot == pilot_data.aircraft_slot)
								kill++;		// AI in player slot
							if (kill) {
								if (last_pilot)
									last_pilot.next_pilot = pilot_data.next_pilot;
								else
									flight_ptr.pilot_list = pilot_data.next_pilot;
								tmp_pilot = pilot_data;
								if (player_pilot == tmp_pilot) {
#if FUNKY_KEVIN_DEBUG_STUFF
							Debug.Assert(!inMission);
#endif
									player_pilot = null;
								}
								pilot_data = pilot_data.next_pilot;
								delete (tmp_pilot);
							} else {
								last_pilot = pilot_data;
								pilot_data = pilot_data.next_pilot;
							}
						}
						last_ptr = flight_ptr;
						flight_ptr = flight_ptr.next_flight;
					}
				}

				// Now add any additional flights which matter
				uelement = (Unit)flit.GetFirst ();
				while (uelement) {
					if (uelement.IsFlight ()) {
						flight = (FlightClass*)uelement;
						flight_ptr = FindFlightData (flight);
						if (flight_ptr) {
							// Already exists - just check for new players
							for (i=0; i<PilotStatic.PILOTS_PER_FLIGHT; i++) {
								if (flight.plane_stats [i] == AIRCRAFT_AVAILABLE && !FindPilotDataFromAC (flight_ptr, i)) {
									// Add this pilot
									session = FindPlayer ((Flight)uelement, i);
									if (session)
										AddNewPlayerPilot (flight_ptr, i, (Flight)uelement, session);
									else {
										PilotDataClass pilot_data2 = AddNewPilot (flight_ptr, i, i, flight);
										// AI Pilots named by callsign
										_stprintf (pilot_data2.pilot_name, "%s%d", flight_ptr.name, i + 1);
										_stprintf (pilot_data2.pilot_callsign, "%s%d", flight_ptr.name, i + 1);
									}
								}
							}
						} else {
							// Add the whole thing
							PreEvalFlight (flight, null);
						}
						uelement.SetInPackage (1);
					}
					uelement = (Unit)flit.GetNext ();
				}

//		evalCount++;
//		if (evalCount > 500 && !(SimDogfight.flags & DF_GAME_OVER))
//			{
//			// Periodically send any evaluation data we may have
//			SendAllEvalData();
//			}

				CampLeaveCriticalSection ();
			}
			#endif
			throw 
				new NotImplementedException();
		}

	}
	
	// ===========================================================
	// class used for temporary pilot list building/sorting
	// ===========================================================
	
	public class PilotSortClass
	{
		public short team;
		public PilotDataClass pilot_data;
		public PilotSortClass next;
	
		public PilotSortClass (PilotDataClass pilot_ptr)
		{
			pilot_data = pilot_ptr;
			next = null;
		}
	}
	
	// =============================
	// Global setter/query functions
	// =============================
	public static class MissEval
	{
		public static int OverFriendlyTerritory (Flight flight)
		{
			throw new NotImplementedException ();
		}
	}
}

