using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalconDB
{
    /// <summary>
    /// Computer controled pilots
    /// </summary>
    public class PilotInfo
    {
        /// <summary>
        /// Index into the PilotInfoClass table
        /// </summary>
        public short PilotInfoId { get; set; }

        /// <summary>
        /// LowByte: Skill, HiByte: Rating
        /// </summary>
        private byte pilot_skill_and_rating { get; set; }

        public int GetPilotSkill
        {
            get { return (pilot_skill_and_rating & 0xF); }
        }

        public int GetPilotRating
        {
            get { return ((byte)((pilot_skill_and_rating & 0xF0) >> 4)); }
        }

        public void SetPilotSR(byte skill, byte rating)
        {
            pilot_skill_and_rating = (byte)((rating << 4) | skill);
        }

        public byte PilotStatus { get; set; }
        public byte AAKills { get; set; }
        public byte AGKills { get; set; }
        public byte ASKills { get; set; }
        public byte ANKills { get; set; }
        public short MissionsFlown { get; set; }

        /// <summary>
        /// How many times this pilot is being used
        /// </summary>
        public short Usage { get; set; }

        /// <summary>
        /// Which voice data to use
        /// </summary>
        public Voice VoiceID { get; set; }

        /// <summary>
        ///  Assigned through the UI
        /// </summary>
        public PilotPhoto PilotPhotoID { get; set; }

    }

    public enum PilotRank
    {
        SEC_LT,
        LEIUTENANT,
        CAPTAIN,
        MAJOR,
        LT_COL,
        COLONEL,
        BRIG_GEN,
        NUM_RANKS,
    }

    //define value identifying medals for array index
    public enum PilotMedal
    {
        AIR_FORCE_CROSS,
        SILVER_STAR,
        DIST_FLY_CROSS,
        AIR_MEDAL,
        KOREA_CAMPAIGN,
        LONGEVITY
    }

    public struct DogfightStats
    {
        public short MatchesWon { get; set; }
        public short MatchesLost { get; set; }
        public short MatchesWonVHum { get; set; }
        public short MatchesLostVHum { get; set; }
        public short Kills { get; set; }
        public short Killed { get; set; }
        public short HumanKills { get; set; }
        public short KilledByHuman { get; set; }
    }

    public struct CampaignStats
    {
        public short GamesWon { get; set; }
        public short GamesLost { get; set; }
        public short GamesTied { get; set; }
        public short Missions { get; set; }
        public long TotalScore { get; set; }
        public long TotalMissionScore { get; set; }
        public short ConsecMissions { get; set; }
        public short Kills { get; set; }
        public short Killed { get; set; }
        public short HumanKills { get; set; }
        public short KilledByHuman { get; set; }
        public short KilledBySelf { get; set; }
        public short AirToGround { get; set; }
        public short Static { get; set; }
        public short Naval { get; set; }
        public short FriendliesKilled { get; set; }
        public short MissSinceLastFriendlyKill { get; set; }
    }

    public class PlayerPilot
    {
        public int ID { get; set; }

        public string Name { get; set; }
        public string Callsign { get; set; }
        public string Password { get; set; }
        public DateTime Commissioned { get; set; }
        public string OptionsFile { get; set; }
        public float FlightHours { get; set; }
        public float AceFactor { get; set; }
        public PilotRank Rank { get; set; }
        public DogfightStats Dogfight { get; set; }
        public CampaignStats Campaign { get; set; }
        public PilotPhoto Picture { get; set; }
        public long PatchResource { get; set; }
        public string Patch { get; set; }
        public string Personal { get; set; }
        public string Squadron { get; set; }

        /// <summary>
        /// index from 0 - 11 indicating which voice they want
        /// </summary>
        public Voice Voice { get; set; }

        /// <summary>
        ///  If this value is ever NON zero after Decrypting, the Data has been modified
        /// </summary>
        internal long CheckSum { get; set; }

        public byte[] Medals = new byte[Enum.GetNames(typeof(PilotMedal)).Length];

    }

    public class Voice
    {
        public short ID { get; set; }

        public string VoicePath { get; set; }
    }

    public class PilotPhoto
    {
        public short ID { get; set; }

        public string PhotoPath { get; set; }
    }
}
