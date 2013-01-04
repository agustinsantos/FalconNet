using FalconNet.Ui95;
using FalconNet.VU;
using System;


namespace FalconNet.UI
{

    public enum victory_type
    {
        vt_unknown,
        vt_occupy,
        vt_destroy,
        vt_attrit,
        vt_intercept,
        vt_degrade,
        vt_kills,
        vt_frags,
        vt_deaths,
        vt_landing,
        vt_tos,
        vt_airspeed,
        vt_altitude,
        vt_position
    }

    ///////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////

    public enum mission_rating
    {
        mr_success,
        mr_decisive_victory,
        mr_marginal_victory,
        mr_tie,
        mr_stalemate,
        mr_marginal_defeat,
        mr_crushing_defeat,
        mr_failure,
    }
    //////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////

    public enum kill_category
    {
        kc_unknown,
        kc_aircraft,
        kc_ground_vehicles,
        kc_air_defences,
        kc_static_targets,
        kc_any
    }

    //////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////

    public enum condition_parameters
    {
        cp_unknown,
        cp_tos,
        cp_airspeed,
        cp_altitude,
        cp_position
    }

    //////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////

    public class victory_condition
    {


        private tactical_mission mission;

        private int active;

        private victory_condition pred;
        private victory_condition succ;

        private int team; // Team # of team who can benefit from this VC

        private victory_type type;

        private VU_ID id;

        private int feature_id; // this is the Feature ID (if vu_id is an Objective)

        private int tolerance; // this is the % for degrade/attrit or # for intercept (if vu_id is flight or unit)

        private int points; // This is the # points you get for doing the VC

        private int number; // this is the VC ID

        private int max_vehicles;	// number of aircraft in intercept



        public C_Base control;



        public victory_condition(tactical_mission tm)
        { throw new NotImplementedException(); }
        // TODO public ~victory_condition ();

        public void set_team(int t)
        { throw new NotImplementedException(); }
        public int get_team()
        { throw new NotImplementedException(); }

        public void set_type(victory_type vt)
        { throw new NotImplementedException(); }
        public victory_type get_type()
        { throw new NotImplementedException(); }

        public void set_vu_id(VU_ID id)
        { throw new NotImplementedException(); }
        public VU_ID get_vu_id()
        { throw new NotImplementedException(); }

        public void set_sub_objective(int sb)
        { throw new NotImplementedException(); }
        public int get_sub_objective()
        { throw new NotImplementedException(); }

        public void set_tolerance(int t)
        { throw new NotImplementedException(); }
        public int get_tolerance()
        { throw new NotImplementedException(); }

        public void set_points(int p)
        { throw new NotImplementedException(); }
        public int get_points()
        { throw new NotImplementedException(); }

        public void set_number(int n)
        { throw new NotImplementedException(); }
        public int get_number()
        { throw new NotImplementedException(); }

        public void set_active(int a)
        { throw new NotImplementedException(); }
        public int get_active()
        { throw new NotImplementedException(); }

        public void test()
        { throw new NotImplementedException(); }

        public static void enter_critical_section()
        { throw new NotImplementedException(); }
        public static void leave_critical_section()
        { throw new NotImplementedException(); }
    }

    //////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////

    public enum tactical_mode
    {
        tm_unknown,
        tm_training,
        tm_load,
        tm_join
    }

    //////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////

    public enum tactical_type
    {
        tt_unknown,
        tt_engagement,
        tt_single,
        tt_training
    }

    ///////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////

    public enum victory_condition_filter
    {
        vcf_unknown,
        vcf_all,
        vcf_team,
        vcf_all_achieved,
        vcf_all_remaining,
        vcf_team_achieved,
        vcf_team_remaining
    }

    ///////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////

    public enum tac_flags
    {
        tf_hide_enemy = 0x01,
        tf_lock_ato = 0x02,
        tf_lock_oob = 0x04,
        tf_start_paused = 0x08
    }

    ///////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////

    public class tactical_mission
    {
        private string filename;

        private int is_new;
        private int is_online;

        private int number_teams;
        private string team_name;
        private byte[] team_flag = new byte[8];
        private int[] number_aircraft = new int[8];
        private int[] number_players = new int[8];
        private int[] number_f16s = new int[8];

        private victory_condition[] conditions;
        private victory_condition current_vc;

        private int team;
        private int[] team_pts = new int[8];

        victory_condition_filter filter;

        private int points_required;

        private int game_over;
        //	static tactical_mode		search_mode;
        //	static tactical_mission		*search_mission;

        private string read_te_file(string filename, out int size)
        { throw new NotImplementedException(); }
        private void process_load(byte[] data, int size, int full_load)
        { throw new NotImplementedException(); }


        public tactical_mission(string filename)
        { throw new NotImplementedException(); }
        public tactical_mission()
        { throw new NotImplementedException(); }		// Online
        //TODO public ~tactical_mission ();

        public void load()
        { throw new NotImplementedException(); }
        public void preload()
        { throw new NotImplementedException(); }
        public void new_setup()
        { throw new NotImplementedException(); }
        public void info_load(string filename)
        { throw new NotImplementedException(); }
        public void revert()
        { throw new NotImplementedException(); }

        public void save_data(string filename)
        { throw new NotImplementedException(); }
        public void save(string filename)
        { throw new NotImplementedException(); }

        public void set_game_over(int val) { game_over = val; }
        public int get_game_over() { return (game_over); }

        public tactical_type get_type()
        { throw new NotImplementedException(); }
        public void set_type(tactical_type type)
        { throw new NotImplementedException(); }

        public string get_title()
        { throw new NotImplementedException(); }

        public int get_number_of_teams()
        { throw new NotImplementedException(); }
        public int get_number_of_aircraft(int team)
        { throw new NotImplementedException(); }
        public int get_number_of_players(int team)
        { throw new NotImplementedException(); }
        public int get_number_of_f16s(int team)
        { throw new NotImplementedException(); }

        public int get_team_points(int theteam) { return team_pts[theteam]; }

        public string get_team_name(int team)
        { throw new NotImplementedException(); }
        public int get_team_colour(int team)
        { throw new NotImplementedException(); }
        public int get_team() { return team; }
        public int get_team_flag(int team)
        { throw new NotImplementedException(); }

        public int hide_enemy_on()
        { throw new NotImplementedException(); }
        public int lock_ato_on()
        { throw new NotImplementedException(); }
        public int lock_oob_on()
        { throw new NotImplementedException(); }
        public int start_paused_on()
        { throw new NotImplementedException(); }

        public int is_flag_on(long value)
        { throw new NotImplementedException(); }

        // Moved the routines themselves into tac_class.cpp just incase any other insestuos(sp?) headers are necessary
        public void set_flag(long value)
        { throw new NotImplementedException(); }
        public void clear_flag(long value)
        { throw new NotImplementedException(); }

        public void set_team_name(int team, string name)
        { throw new NotImplementedException(); }
        public void set_team_colour(int team, int colour)
        { throw new NotImplementedException(); }
        public void set_team(int newteam) { team = newteam; }
        public void set_team_flag(int team, int flag)
        { throw new NotImplementedException(); }

        public void setup_victory_condition(byte[] data)
        { throw new NotImplementedException(); }

        public void evaluate_victory_conditions()
        { throw new NotImplementedException(); }
        //	NOT supported... some variables have been removed or changed
        //	void evaluate_parameters (void *wp, double x, double y, double z, double s);

        public void calculate_victory_points()
        { throw new NotImplementedException(); }
        public int determine_victor()
        { throw new NotImplementedException(); }
        public int determine_rating()
        { throw new NotImplementedException(); }

        public void set_victory_condition_filter(victory_condition_filter vcf)
        { throw new NotImplementedException(); }
        public void set_victory_condition_team_filter(int team)
        { throw new NotImplementedException(); }

        public victory_condition get_first_victory_condition()
        { throw new NotImplementedException(); }
        public victory_condition get_next_victory_condition()
        { throw new NotImplementedException(); }

        public victory_condition get_first_unfiltered_victory_condition()
        { throw new NotImplementedException(); }
        public victory_condition get_next_unfiltered_victory_condition()
        { throw new NotImplementedException(); }

        public void set_points_required(int value)
        { throw new NotImplementedException(); }
        public int get_points_required()
        { throw new NotImplementedException(); }

    }
}
