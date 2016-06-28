using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalconNet.VU.Comms
{
    public class BW
    { }

    public static class CApiBandwithControl
    {
        /** starts bandwidth control */
        public static void start_bandwidth()
        { throw new NotImplementedException(); }

        /** enters a given state, adjusting bandwidth */
        public static void enter_state(bwstates st)
        { throw new NotImplementedException(); }

        /** called when a new player joins, adjusting bw */
        public static void player_joined()
        { throw new NotImplementedException(); }

        /** called when a player leaves, adjusting bw */
        public static void player_left()
        { throw new NotImplementedException(); }

        /** consume bandwidth
        * @param size amount of bytes to consume
        * @param isReliable indicates we are consuming reliable data. This means size will be adjusted,
        * @param type of bandwidth being used
        * since reliable consumes more
        */
        public static void use_bandwidth(int size, int isReliable, int type)
        { throw new NotImplementedException(); }

        /** checks if there is available bandwidth. if isRUDP, size is adjusted since RUDP consumes more
        * @param size amount of bytes
        * @param isReliable indicates we are consuming RUDP, meaning size will be adjusted
        * @param type of bandwidth being used
        * @return false if there is no BW available, different otherwise
        */
        public static bool check_bandwidth(int size, bool isReliable, int type)
        { throw new NotImplementedException(); }

        /** we call this when we want to stop sending (for example, when a send would block because of a full queue */
        public static void cut_bandwidth()
        { throw new NotImplementedException(); }

        /** gets status regarding BW usage
        * @param isReliable status for reliable connection? or regular?
        * @return ok: 0, warning: 1, critical: 0
        */
        public static int get_status(int isReliable)
        { throw new NotImplementedException(); }

        /** object holding bw information */
        static BW bw_object;

    }

    /** these are possible states for bw control
    * WARNING: if changed here, change also capi.h
    */
    public enum bwstates
    {
        LOBBY_ST = 0,      ///< lobby
        CAS_ST = 1,      ///< inside CA or TE as server
        CAC_ST = 2,      ///< inside CA or TE as client
        DF_ST = 3,      ///< in dogfight
        NOSTATE_ST         ///< number of states
    }
}
