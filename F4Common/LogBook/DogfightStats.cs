
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

        public static void Decode(Stream stream, ref DF_STATS rst)
        {
            Int16EncodingLE.Decode(stream, ref rst.MatchesWon);
            Int16EncodingLE.Decode(stream, ref rst.MatchesLost);
            Int16EncodingLE.Decode(stream, ref rst.MatchesWonVHum);
            Int16EncodingLE.Decode(stream, ref rst.MatchesLostVHum);
            Int16EncodingLE.Decode(stream, ref rst.Kills);
            Int16EncodingLE.Decode(stream, ref rst.Killed);
            Int16EncodingLE.Decode(stream, ref rst.HumanKills);
            Int16EncodingLE.Decode(stream, ref rst.KilledByHuman);
        }

        public static int Size
        {
            get { return Int16EncodingLE.Size * 8; }
        }
    }
}
