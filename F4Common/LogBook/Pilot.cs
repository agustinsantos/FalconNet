
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
        public const int _NAME_LEN_ = 20;
        public const int _CALLSIGN_LEN_ = 12;
        public const int PASSWORD_LEN = 10;
        public const int PERSONAL_TEXT_LEN = 120;
        public const int COMM_LEN = 12;
        public const int FILENAME_LEN = 32;
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
            rst.Name = StringFixedASCIIEncoding.Decode(buffer, LB_PILOT._NAME_LEN_+1);
            rst.Callsign = StringFixedASCIIEncoding.Decode(buffer, LB_PILOT._CALLSIGN_LEN_+1);
            rst.Password = StringFixedASCIIEncoding.Decode(buffer, LB_PILOT.PASSWORD_LEN+1);
            rst.Commissioned = StringFixedASCIIEncoding.Decode(buffer, LB_PILOT.COMM_LEN+1);
            rst.OptionsFile = StringFixedASCIIEncoding.Decode(buffer, LB_PILOT._CALLSIGN_LEN_+1);
            buffer.GetByte();
            rst.FlightHours = SingleEncodingLE.Decode(buffer);
            rst.AceFactor = SingleEncodingLE.Decode(buffer);
            rst.Rank = (LB_RANK)Int32EncodingLE.Decode(buffer);
            rst.Dogfight = DF_STATSEncodingLE.Decode(buffer);
            rst.Campaign = CAMP_STATSEncodingLE.Decode(buffer);
            buffer.GetBytes(2);
            buffer.GetBytes(rst.Medals);
            buffer.GetBytes(2);
            rst.PictureResource = Int32EncodingLE.Decode(buffer);
            rst.Picture = StringFixedASCIIEncoding.Decode(buffer, LB_PILOT.FILENAME_LEN + 1);
            buffer.GetBytes(3);
            rst.PatchResource = Int32EncodingLE.Decode(buffer);
            rst.Patch = StringFixedASCIIEncoding.Decode(buffer, LB_PILOT.FILENAME_LEN+1);
            rst.Personal = StringFixedASCIIEncoding.Decode(buffer, LB_PILOT.PERSONAL_TEXT_LEN+1);
            rst.Squadron = StringFixedASCIIEncoding.Decode(buffer, LB_PILOT._NAME_LEN_);
            rst.voice = Int16EncodingLE.Decode(buffer);
            rst.CheckSum = Int32EncodingLE.Decode(buffer);
            return rst;
        }

        public static LB_PILOT Decode(Stream stream)
        {
            LB_PILOT rst = new LB_PILOT();
            rst.Name = StringFixedASCIIEncoding.Decode(stream, LB_PILOT._NAME_LEN_ + 1);
            rst.Callsign = StringFixedASCIIEncoding.Decode(stream, LB_PILOT._CALLSIGN_LEN_ + 1);
            rst.Password = StringFixedASCIIEncoding.Decode(stream, LB_PILOT.PASSWORD_LEN + 1);
            rst.Commissioned = StringFixedASCIIEncoding.Decode(stream, LB_PILOT.COMM_LEN + 1);
            rst.OptionsFile = StringFixedASCIIEncoding.Decode(stream, LB_PILOT._CALLSIGN_LEN_ + 1);
            stream.ReadByte();
            rst.FlightHours = SingleEncodingLE.Decode(stream);
            rst.AceFactor = SingleEncodingLE.Decode(stream);
            rst.Rank = (LB_RANK)Int32EncodingLE.Decode(stream);
            rst.Dogfight = DF_STATSEncodingLE.Decode(stream);
            rst.Campaign = CAMP_STATSEncodingLE.Decode(stream);
            stream.Position += 2;
            stream.Read(rst.Medals, 0, (int)LB_MEDAL.NUM_MEDALS);
            stream.Position += 2;
            rst.PictureResource = Int32EncodingLE.Decode(stream);
            rst.Picture = StringFixedASCIIEncoding.Decode(stream, LB_PILOT.FILENAME_LEN + 1);
            stream.Position += 3;
            rst.PatchResource = Int32EncodingLE.Decode(stream);
            rst.Patch = StringFixedASCIIEncoding.Decode(stream, LB_PILOT.FILENAME_LEN + 1);
            rst.Personal = StringFixedASCIIEncoding.Decode(stream, LB_PILOT.PERSONAL_TEXT_LEN + 1);
            rst.Squadron = StringFixedASCIIEncoding.Decode(stream, LB_PILOT._NAME_LEN_);
            rst.voice = Int16EncodingLE.Decode(stream);
            rst.CheckSum = Int32EncodingLE.Decode(stream);
            return rst;
        }

        public static int Size
        {
            get
            {
                return 372;
                        //LB_PILOT._NAME_LEN_ + 1 + LB_PILOT._CALLSIGN_LEN_ + 1 +
                        // LB_PILOT.PASSWORD_LEN + 1 + LB_PILOT.COMM_LEN + 1 +
                        // LB_PILOT._CALLSIGN_LEN_ + 1 + SingleEncodingLE.Size * 2 +
                        // Int16EncodingLE.Size * 2 + DF_STATSEncodingLE.Size +
                        // CAMP_STATSEncodingLE.Size + (int)LB_MEDAL.NUM_MEDALS +
                        // Int64EncodingLE.Size * 2 + 27 + 1 +
                        // LB_PILOT.FILENAME_LEN + 1 + LB_PILOT.PERSONAL_TEXT_LEN + 1 +
                        // LB_PILOT._NAME_LEN_ + 1 + Int32EncodingLE.Size -2;
            } 
        }
    }
}
