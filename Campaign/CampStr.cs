using FalconNet.CampaignBase;
using FalconNet.Common.Encoding;
using FalconNet.F4Common;
using FalconNet.FalcLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace FalconNet.Campaign
{
    public static class CampStr
    {
        public const int NUM_CAMERA_LABELS = 16;
        // Index and string information
        public static string[] ObjectiveStr = new string[33];
        public static string[] MissStr = new string[(int)MissionTypeEnum.AMIS_OTHER];
        public static string[] WPActStr = new string[(int)WPAction.WP_LAST];
        public static string[] AirSTypesStr = new string[20];
        public static string[] GroundSTypesStr = new string[20];
        public static string[] NavalSTypesStr = new string[20];
        public static string[] CountryNameStr = new string[(int)CountryListEnum.NUM_COUNS];
        public static string gUnitNameFormat;
        public static string[] TOTStr = { "NA", "<", "<=", "=", ">=", ">", "NA" };
        public static string[] TargetTypeStr = { "Location", "Objective", "Unit" };
        public static string[] OrderStr = { "Reserve", "Capture", "Secure", "Assault", "Airborne", "Commando", "Defend", "Support", "Repair", "Air Defense", "Recon", "Radar" };
        public static string[] FormStr = { "None", "Line Abreast", "Wedge" };
        public static string[] Side = { "XX", "US", "SK", "JA", "RU", "CH", "NK", "GO" };
        public static string[] CompressionStr = new string[20];
        public static string[] CameraLabel = new string[NUM_CAMERA_LABELS] { "FLY-BY CAMERA", "CHASE CAMERA", "ORBIT CAMERA", "SATELLITE CAMERA", "WEAPON CAMERA", "TARGET TO WEAPON CAMERA", "ENEMY AIRCRAFT CAMERA", "FRIENDLY AIRCRAFT CAMERA", "ENEMY GROUND UNIT CAMERA", "FRIENDLY GROUND UNIT CAMERA", "INCOMING MISSILE CAMERA", "TARGET CAMERA", "TARGET TO SELF CAMERA", "ACTION CAMERA", "RECORDING", "" };


        public static string[] SpecialStr = { "General", "Air to Air", "Air to Ground" };

        // Functions
        public static string GetSTypeName(Domains domain, ClassTypes type, int stype)
        {
            string buffer = null;
            if (domain == Domains.DOMAIN_AIR)
                buffer = AirSTypesStr[stype];
            else if (domain == Domains.DOMAIN_LAND)
                buffer = GroundSTypesStr[stype];
            else if (domain == Domains.DOMAIN_SEA)
                buffer = NavalSTypesStr[stype];
            else
                buffer = AirSTypesStr[0];

            return buffer;
        }

        public static string GetNumberName(int nameid)
        {
            string tmp = null;

            if (nameid % 10 == 1 && nameid != 11)
            {
                if (F4Config.gLangIDNum == F4LANGUAGE.F4LANG_FRENCH)
                {
                    if (nameid != 1)
                        tmp = Name.ReadNameString(16);
                    else
                        tmp = Name.ReadNameString(15);
                }
                else
                    tmp = Name.ReadNameString(15);
            }
            else if (nameid % 10 == 2 && nameid != 12)
                tmp = Name.ReadNameString(16);
            else if (nameid % 10 == 3 && nameid != 13)
                tmp = Name.ReadNameString(17);
            else
                tmp = Name.ReadNameString(18);

            return nameid + tmp;
        }

        public static string GetTimeString(CampaignTime time, bool seconds = true)
        {
            int d, h, m, s;
            int time_ = (int)time.time;
            string format, hour, minute, second = null;

            d = time_ / (int)CampaignTime.CampaignDay;
            time_ -= d * (int)CampaignTime.CampaignDay;
            h = time_ / (int)CampaignTime.CampaignHours;
            hour = h.ToString("D2");
            time_ -= h * (int)CampaignTime.CampaignHours;
            m = time_ / (int)CampaignTime.CampaignMinutes;
            minute = m.ToString("D2");

            if (seconds)
            {
                time_ -= m * (int)CampaignTime.CampaignMinutes;
                s = time_ / (int)CampaignTime.CampaignSeconds;
                second = s.ToString("D2");
            }

            // Construct the string
            if (seconds)
                format = Name.ReadNameString(50);
            else
                format = Name.ReadNameString(57);
            string rst = ConstructOrderedSentence(format, hour, minute, second); // PJW: my size is 10 characters
            return rst;
        }

        public static void ReadIndex(string filename)
        {
            Name.LoadNames(filename);
            // Now fill some of our string arrays.
            for (int i = 0; i <= EntityDB.NumObjectiveTypes; i++)
                ObjectiveStr[i] = Name.ReadNameString(500 + i);

            for (int i = 0; i < (int)MissionTypeEnum.AMIS_OTHER; i++)
                MissStr[i] = Name.ReadNameString(300 + i);

            for (int i = 0; i < (int)WPAction.WP_LAST; i++)
                WPActStr[i] = Name.ReadNameString(350 + i);

            for (int i = 0; i < 20; i++)
                AirSTypesStr[i] = Name.ReadNameString(540 + i);

            for (int i = 0; i < 20; i++)
                GroundSTypesStr[i] = Name.ReadNameString(560 + i);

            for (int i = 0; i < 20; i++)
                NavalSTypesStr[i] = Name.ReadNameString(580 + i);

            for (int i = 0; i < (int)CountryListEnum.NUM_COUNS; i++)
                CountryNameStr[i] = Name.ReadNameString(40 + i);

            for (int i = 0; i < 5; i++)
                CompressionStr[i] = Name.ReadNameString(75 + i);

            for (int i = 0; i < NUM_CAMERA_LABELS; i++)
                CameraLabel[i] = Name.ReadNameString(60 + i);

            gUnitNameFormat = Name.ReadNameString(58);
            if (gUnitNameFormat == "%s, %s")
                gUnitNameFormat = "{0}, {1}";
        }

        public static void FreeIndex()
        {
            //delete[] StringIndex;
            //StringIndex = null;
            //delete[] StringTable;
            //StringTable = null;
        }

        public static string ReadIndexedString(int sid)
        {
            return Name.ReadNameString(sid);
        }

        public static string ConstructOrderedSentence(string format, params object[] args)
        {
            StringBuilder result = new StringBuilder();
            int ptr = 0;
            int start = ptr;
            while (ptr < format.Length)
            {
                if (format[ptr] == '#')
                {
                    ptr++;
                    result.Append(format, start, ptr - start - 1);
                    // read and add the numbered argument
                    int count = format[ptr] - '0'; // arg #

                    start = ptr+1;
                    object arg = args[count];
                    if (arg is double)
                    {
                        double val = (double)arg;
                        result.Append(val.ToString(CultureInfo.InvariantCulture));
                    }
                    else if (arg is Single)
                    {
                        float val = (float)arg;
                        result.Append(val.ToString(CultureInfo.InvariantCulture));
                    }
                    else
                        result.Append(arg.ToString());

                }
                ptr++;
            }
            return result.ToString();
        }
    }
}
