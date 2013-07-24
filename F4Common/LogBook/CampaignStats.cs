
using FalconNet.Common.Encoding;
using System.IO;
namespace FalconNet.F4Common
{
    public struct CAMP_STATS //CampaignStats
    {
        public short GamesWon;
        public short GamesLost;
        public short GamesTied;
        public short Missions;
        public long TotalScore;
        public long TotalMissionScore;
        public short ConsecMissions;
        public short Kills;
        public short Killed;
        public short HumanKills;
        public short KilledByHuman;
        public short KilledBySelf;
        public short AirToGround;
        public short Static;
        public short Naval;
        public short FriendliesKilled;
        public short MissSinceLastFriendlyKill;
    }

    public static class CAMP_STATSEncodingLE
    {
        public static void Encode(ByteWrapper buffer, CAMP_STATS val)
        {
            Int16EncodingLE.Encode(buffer, val.GamesWon);
            Int16EncodingLE.Encode(buffer, val.GamesLost);
            Int16EncodingLE.Encode(buffer, val.GamesTied);
            Int16EncodingLE.Encode(buffer, val.Missions);
            Int32EncodingLE.Encode(buffer, (int)val.TotalScore);
            Int32EncodingLE.Encode(buffer, (int)val.TotalMissionScore);
            Int16EncodingLE.Encode(buffer, val.ConsecMissions);
            Int16EncodingLE.Encode(buffer, val.Kills);
            Int16EncodingLE.Encode(buffer, val.Killed);
            Int16EncodingLE.Encode(buffer, val.HumanKills);
            Int16EncodingLE.Encode(buffer, val.KilledByHuman);
            Int16EncodingLE.Encode(buffer, val.KilledBySelf);
            Int16EncodingLE.Encode(buffer, val.AirToGround);
            Int16EncodingLE.Encode(buffer, val.Static);
            Int16EncodingLE.Encode(buffer, val.Naval);
            Int16EncodingLE.Encode(buffer, val.FriendliesKilled);
            Int16EncodingLE.Encode(buffer, val.MissSinceLastFriendlyKill);
        }
        public static void Encode(Stream stream, CAMP_STATS val)
        {
            Int16EncodingLE.Encode(stream, val.GamesWon);
            Int16EncodingLE.Encode(stream, val.GamesLost);
            Int16EncodingLE.Encode(stream, val.GamesTied);
            Int16EncodingLE.Encode(stream, val.Missions);
            Int32EncodingLE.Encode(stream, (int)val.TotalScore);
            Int32EncodingLE.Encode(stream, (int)val.TotalMissionScore);
            Int16EncodingLE.Encode(stream, val.ConsecMissions);
            Int16EncodingLE.Encode(stream, val.Kills);
            Int16EncodingLE.Encode(stream, val.Killed);
            Int16EncodingLE.Encode(stream, val.HumanKills);
            Int16EncodingLE.Encode(stream, val.KilledByHuman);
            Int16EncodingLE.Encode(stream, val.KilledBySelf);
            Int16EncodingLE.Encode(stream, val.AirToGround);
            Int16EncodingLE.Encode(stream, val.Static);
            Int16EncodingLE.Encode(stream, val.Naval);
            Int16EncodingLE.Encode(stream, val.FriendliesKilled);
            Int16EncodingLE.Encode(stream, val.MissSinceLastFriendlyKill);
        }

        public static CAMP_STATS Decode(ByteWrapper buffer)
        {
            CAMP_STATS rst = new CAMP_STATS();
            rst.GamesWon = Int16EncodingLE.Decode(buffer);
            rst.GamesLost = Int16EncodingLE.Decode(buffer);
            rst.GamesTied = Int16EncodingLE.Decode(buffer);
            rst.Missions = Int16EncodingLE.Decode(buffer);
            rst.TotalScore = Int32EncodingLE.Decode(buffer);
            rst.TotalMissionScore = Int32EncodingLE.Decode(buffer);
            rst.ConsecMissions = Int16EncodingLE.Decode(buffer);
            rst.Kills = Int16EncodingLE.Decode(buffer);
            rst.Killed = Int16EncodingLE.Decode(buffer);
            rst.HumanKills = Int16EncodingLE.Decode(buffer);
            rst.KilledByHuman = Int16EncodingLE.Decode(buffer);
            rst.KilledBySelf = Int16EncodingLE.Decode(buffer);
            rst.AirToGround = Int16EncodingLE.Decode(buffer);
            rst.Static = Int16EncodingLE.Decode(buffer);
            rst.Naval = Int16EncodingLE.Decode(buffer);
            rst.FriendliesKilled = Int16EncodingLE.Decode(buffer);
            rst.MissSinceLastFriendlyKill = Int16EncodingLE.Decode(buffer);
            return rst;
        }

        public static CAMP_STATS Decode(Stream stream)
        {
            CAMP_STATS rst = new CAMP_STATS();
            rst.GamesWon = Int16EncodingLE.Decode(stream);
            rst.GamesLost = Int16EncodingLE.Decode(stream);
            rst.GamesTied = Int16EncodingLE.Decode(stream);
            rst.Missions = Int16EncodingLE.Decode(stream);
            rst.TotalScore = Int32EncodingLE.Decode(stream);
            rst.TotalMissionScore = Int32EncodingLE.Decode(stream);
            rst.ConsecMissions = Int16EncodingLE.Decode(stream);
            rst.Kills = Int16EncodingLE.Decode(stream);
            rst.Killed = Int16EncodingLE.Decode(stream);
            rst.HumanKills = Int16EncodingLE.Decode(stream);
            rst.KilledByHuman = Int16EncodingLE.Decode(stream);
            rst.KilledBySelf = Int16EncodingLE.Decode(stream);
            rst.AirToGround = Int16EncodingLE.Decode(stream);
            rst.Static = Int16EncodingLE.Decode(stream);
            rst.Naval = Int16EncodingLE.Decode(stream);
            rst.FriendliesKilled = Int16EncodingLE.Decode(stream);
            rst.MissSinceLastFriendlyKill = Int16EncodingLE.Decode(stream);
            return rst;
        }

        public static int Size
        {
            get { return Int16EncodingLE.Size * 15 + Int32EncodingLE.Size * 2; }
        }
    }
}
