
using FalconNet.Common.Encoding;
using System;
using System.IO;
namespace FalconNet.F4Common
{
    public class LB_PILOT //Pilot
    {
        internal string Name;
        internal string Callsign;
        internal string Password;
        internal string Commissioned;
        internal string OptionsFile;
        internal float FlightHours;
        internal float AceFactor;
        internal LB_RANK Rank;
        internal DF_STATS Dogfight;
        internal CAMP_STATS Campaign;
        internal byte[] Medals = new byte[(int)LB_MEDAL.NUM_MEDALS];
        internal long PictureResource;
        internal string Picture;
        internal long PatchResource;
        internal string Patch;
        internal string Personal;
        internal string Squadron;
        internal short voice;							// index from 0 - 11 indicating which voice they want
        internal long CheckSum; // If this value is ever NON zero after Decrypting, the Data has been modified


        public const long NOPATCH = 70050;
        public const long NOFACE = 60000;
        public const long LOGBOOK_PICTURE_ID = 8649144;
        public const long LOGBOOK_PICTURE_ID_2 = 8649145;
        public const long LOGBOOK_SQUADRON_ID = 8649146;
        public const long LOGBOOK_SQUADRON_ID_2 = 8649147;

        public const long PATCHES_RESOURCE = 59998;
        public const long PILOTS_RESOURCE = 59999;
    }

    public static class LB_PILOTEncodingLE
    {
        public static void Encode(ByteWrapper buffer, LB_PILOT val)
        {
            StringEncodingLE.Encode(buffer, val.Name);
            StringEncodingLE.Encode(buffer, val.Callsign);
            StringEncodingLE.Encode(buffer, val.Password);
            StringEncodingLE.Encode(buffer, val.Commissioned);
            StringEncodingLE.Encode(buffer, val.OptionsFile);
            SingleEncodingLE.Encode(buffer, val.FlightHours);
            SingleEncodingLE.Encode(buffer, val.AceFactor);
            Int16EncodingLE.Encode(buffer, (Int16)val.Rank);
            DF_STATSEncodingLE.Encode(buffer, val.Dogfight);
            CAMP_STATSEncodingLE.Encode(buffer, val.Campaign);
            buffer.Put(val.Medals);
            Int64EncodingLE.Encode(buffer, val.PictureResource);
            StringEncodingLE.Encode(buffer, val.Picture);
            Int64EncodingLE.Encode(buffer, val.PatchResource);
            StringEncodingLE.Encode(buffer, val.Patch);
            StringEncodingLE.Encode(buffer, val.Personal);
            StringEncodingLE.Encode(buffer, val.Squadron);
            SingleEncodingLE.Encode(buffer, val.voice);
            Int64EncodingLE.Encode(buffer, val.CheckSum);
        }
        public static void Encode(Stream stream, LB_PILOT val)
        {
            StringEncodingLE.Encode(stream, val.Name);
            StringEncodingLE.Encode(stream, val.Callsign);
            StringEncodingLE.Encode(stream, val.Password);
            StringEncodingLE.Encode(stream, val.Commissioned);
            StringEncodingLE.Encode(stream, val.OptionsFile);
            SingleEncodingLE.Encode(stream, val.FlightHours);
            SingleEncodingLE.Encode(stream, val.AceFactor);
            Int16EncodingLE.Encode(stream, (Int16)val.Rank);
            DF_STATSEncodingLE.Encode(stream, val.Dogfight);
            CAMP_STATSEncodingLE.Encode(stream, val.Campaign);
            stream.Write(val.Medals, 0, val.Medals.Length);
            Int64EncodingLE.Encode(stream, val.PictureResource);
            StringEncodingLE.Encode(stream, val.Picture);
            Int64EncodingLE.Encode(stream, val.PatchResource);
            StringEncodingLE.Encode(stream, val.Patch);
            StringEncodingLE.Encode(stream, val.Personal);
            StringEncodingLE.Encode(stream, val.Squadron);
            Int16EncodingLE.Encode(stream, val.voice);
            Int64EncodingLE.Encode(stream, val.CheckSum);
        }

        public static LB_PILOT Decode(ByteWrapper buffer)
        {
            LB_PILOT rst = new LB_PILOT();
            rst.Name = StringEncodingLE.Decode(buffer);
            rst.Callsign = StringEncodingLE.Decode(buffer);
            rst.Password = StringEncodingLE.Decode(buffer);
            rst.Commissioned = StringEncodingLE.Decode(buffer);
            rst.OptionsFile = StringEncodingLE.Decode(buffer);
            rst.FlightHours = SingleEncodingLE.Decode(buffer);
            rst.AceFactor = SingleEncodingLE.Decode(buffer);
            rst.Rank = (LB_RANK)Int16EncodingLE.Decode(buffer);
            rst.Dogfight = DF_STATSEncodingLE.Decode(buffer);
            rst.Campaign = CAMP_STATSEncodingLE.Decode(buffer);
            buffer.GetBytes(rst.Medals);
            rst.PictureResource = Int64EncodingLE.Decode(buffer);
            rst.Picture = StringEncodingLE.Decode(buffer);
            rst.PatchResource = Int64EncodingLE.Decode(buffer);
            rst.Patch = StringEncodingLE.Decode(buffer);
            rst.Personal = StringEncodingLE.Decode(buffer);
            rst.Squadron = StringEncodingLE.Decode(buffer);
            rst.voice = Int16EncodingLE.Decode(buffer);
            rst.CheckSum = Int64EncodingLE.Decode(buffer);
            return rst;
        }

        public static LB_PILOT Decode(Stream stream)
        {
            LB_PILOT rst = new LB_PILOT();
            rst.Name = StringEncodingLE.Decode(stream);
            rst.Callsign = StringEncodingLE.Decode(stream);
            rst.Password = StringEncodingLE.Decode(stream);
            rst.Commissioned = StringEncodingLE.Decode(stream);
            rst.OptionsFile = StringEncodingLE.Decode(stream);
            rst.FlightHours = SingleEncodingLE.Decode(stream);
            rst.AceFactor = SingleEncodingLE.Decode(stream);
            rst.Rank = (LB_RANK)Int16EncodingLE.Decode(stream);
            rst.Dogfight = DF_STATSEncodingLE.Decode(stream);
            rst.Campaign = CAMP_STATSEncodingLE.Decode(stream);
            stream.Read(rst.Medals, 0, rst.Medals.Length);
            rst.PictureResource = Int64EncodingLE.Decode(stream);
            rst.Picture = StringEncodingLE.Decode(stream);
            rst.PatchResource = Int64EncodingLE.Decode(stream);
            rst.Patch = StringEncodingLE.Decode(stream);
            rst.Personal = StringEncodingLE.Decode(stream);
            rst.Squadron = StringEncodingLE.Decode(stream);
            rst.voice = Int16EncodingLE.Decode(stream);
            rst.CheckSum = Int64EncodingLE.Decode(stream);
            return rst;
        }

        public static int Size
        {
            get { return -1; } //Size not defined
        }
    }
}
