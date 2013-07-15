
using FalconNet.Common.Encoding;
using System.IO;
namespace FalconNet.F4Common
{
    public struct DF_STATS //DogfightStats
    {
        public short MatchesWon;
        public short MatchesLost;
        public short MatchesWonVHum;
        public short MatchesLostVHum;
        public short Kills;
        public short Killed;
        public short HumanKills;
        public short KilledByHuman;
    }

    public static class DF_STATSEncodingLE
    {
        public static void Encode(ByteWrapper buffer, DF_STATS val)
        {
            Int16EncodingLE.Encode(buffer, val.MatchesWon);
            Int16EncodingLE.Encode(buffer, val.MatchesLost);
            Int16EncodingLE.Encode(buffer, val.MatchesWonVHum);
            Int16EncodingLE.Encode(buffer, val.MatchesLostVHum);
            Int16EncodingLE.Encode(buffer, val.Kills);
            Int16EncodingLE.Encode(buffer, val.Killed);
            Int16EncodingLE.Encode(buffer, val.HumanKills);
            Int16EncodingLE.Encode(buffer, val.KilledByHuman);

        }
        public static void Encode(Stream stream, DF_STATS val)
        {
            Int16EncodingLE.Encode(stream, val.MatchesWon);
            Int16EncodingLE.Encode(stream, val.MatchesLost);
            Int16EncodingLE.Encode(stream, val.MatchesWonVHum);
            Int16EncodingLE.Encode(stream, val.MatchesLostVHum);
            Int16EncodingLE.Encode(stream, val.Kills);
            Int16EncodingLE.Encode(stream, val.Killed);
            Int16EncodingLE.Encode(stream, val.HumanKills);
            Int16EncodingLE.Encode(stream, val.KilledByHuman);
        }

        public static DF_STATS Decode(ByteWrapper buffer)
        {
            DF_STATS rst = new DF_STATS();
            rst.MatchesWon = Int16EncodingLE.Decode(buffer);
            rst.MatchesLost = Int16EncodingLE.Decode(buffer);
            rst.MatchesWonVHum = Int16EncodingLE.Decode(buffer);
            rst.MatchesLostVHum = Int16EncodingLE.Decode(buffer);
            rst.Kills = Int16EncodingLE.Decode(buffer);
            rst.Killed = Int16EncodingLE.Decode(buffer);
            rst.HumanKills = Int16EncodingLE.Decode(buffer);
            rst.KilledByHuman = Int16EncodingLE.Decode(buffer);
            return rst;
        }
        public static DF_STATS Decode(Stream stream)
        {
            DF_STATS rst = new DF_STATS();
            rst.MatchesWon = Int16EncodingLE.Decode(stream);
            rst.MatchesLost = Int16EncodingLE.Decode(stream);
            rst.MatchesWonVHum = Int16EncodingLE.Decode(stream);
            rst.MatchesLostVHum = Int16EncodingLE.Decode(stream);
            rst.Kills = Int16EncodingLE.Decode(stream);
            rst.Killed = Int16EncodingLE.Decode(stream);
            rst.HumanKills = Int16EncodingLE.Decode(stream);
            rst.KilledByHuman = Int16EncodingLE.Decode(stream);
            return rst;
        }

        public static int Size
        {
            get { return Int64EncodingLE.Size * 8; }
        }
    }
}
