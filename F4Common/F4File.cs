using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FalconNet.Common;
using System.Diagnostics;
using log4net;
using FalconNet.Common.Encoding;

namespace FalconNet.F4Common
{
    public static class F4File
    {
        public static string falconDataDirectory;
        public static string FalconTerrainDataDir;
        public static string FalconObjectDataDir;
        public static string Falcon3DDataDir;
        public static string FalconMiscTexDataDir;
        public static string FalconCampaignSaveDirectory;
        public static string FalconCampUserSaveDirectory;

        private static readonly char Sep = Path.DirectorySeparatorChar;

        public static string FalconDataDirectory
        {
            get { return falconDataDirectory; }
        }

        public static void InitDirectories()
        {
            falconDataDirectory = ".";
            FalconTerrainDataDir = FalconDataDirectory + "theater";
            FalconObjectDataDir = FalconDataDirectory + "object";
            FalconMiscTexDataDir = FalconDataDirectory + "FalconMiscTexData";
            string campaignDir = FalconDataDirectory + "Campaign";
            FalconCampaignSaveDirectory = campaignDir + Sep + "Save";
            FalconCampUserSaveDirectory = campaignDir + Sep + "Save";
        }

        public static int CreateCampFile(string filename, string path)
        {
            string fullname;
            FileStream fp;

            // This filename doesn't exist yet (At least res manager doesn't think so)
            // Create it, so that the manager can find it -
            fullname = path + Sep + filename;
            fp = new FileStream(fullname, FileMode.OpenOrCreate, FileAccess.Write);
            fp.Close();
            // Now add the current save directory path, if we still can't find this file
            //	if (!ResExistFile(filename))
            //		ResAddPath(path, false);
            return 1;
        }

        public static FileStream OpenCampFile(string filename, string ext, FileAccess access)
        {
            string fullname, path;
            int index;

            string buffer;

            FileStream fp;
            ext = ext.ToLowerInvariant();

            if (ext == "wch")
            {
                for (index = 0; index < MAX_WCH_FILES; index++)
                {
                    if ((wch_fp[index] != null) && (wch_filename[index] == filename))
                    {
                        wch_fp[index].Seek(0, 0);

                        return wch_fp[index];
                    }
                }
            }

            buffer = string.Format("OpenCampFile {0}.{1} {2}\n", filename, ext, access);
            // MonoPrint (buffer);
            // OutputDebugString (buffer);

            // 2002-03-25 MN added check for not being WCH file - otherwise can crash sometimes
            // especially after theater switching situations !
            if ((reading_campressed_file) && (access == FileAccess.Read) && ext != "wch")
            {
                if (filename != camp_file_name && IsCampFile(camp_game_type, filename))
                {
                    EndReadCampFile();
                    StartReadCampFile(camp_game_type, filename);
                    Debug.Assert(reading_campressed_file);
                }

                fullname = filename + "." + ext;

                for (index = 0; index < camp_num_files; index++)
                {
                    if (string_compare_extensions(fullname, camp_names[index]))
                    {
                        camp_fp.Seek(camp_offset[index], 0);

                        return camp_fp;
                    }
                }

                // MonoPrint ("Cannot OpenCampFile while we are reading a campressed file\n");
            }

            if ((writing_campressed_file) && (access == FileAccess.Write))
            {
                fullname = filename + "." + ext;
                camp_fp.Seek(0, SeekOrigin.End);
                camp_names[camp_num_files] = fullname;
                camp_offset[camp_num_files] = camp_fp.Position;
                camp_size[camp_num_files] = 0;
                return camp_fp;
            }

            switch (ext)
            {
                case "cmp":
                case "obj":
                case "obd":
                case "uni":
                case "tea":
                case "wth":
                case "plt":
                case "mil":
                case "tri":
                case "evl":
                case "smd":
                case "sqd":
                case "pol":
                    path = FalconCampUserSaveDirectory;
                    break;
                case "ct":
                case "ini":
                case "ucd":
                case "ocd":
                case "fcd":
                case "vcd":
                case "wcd":
                case "rcd":
                case "icd":
                case "rwd":
                case "vsd":
                case "swd":
                case "acd":
                case "wld":
                case "phd":
                case "pd":
                case "fed":
                case "ssd":
                case "rkt":
                case "ddp":
                    path = FalconObjectDataDir;
                    break;
                default:
                    path = FalconCampUserSaveDirectory;
                    break;
            }

            // Outdated by resmgr:
            // if (!ResExistFile(filename))
            // ResAddPath(path, FALSE);

            fullname = path + Path.DirectorySeparatorChar + filename + "." + ext;
            fp = new FileStream(fullname, FileMode.OpenOrCreate, access);

            if ((fp != null) && ext == "wch")
            {
                index = next_wch_file++;

                Debug.Assert(next_wch_file <= MAX_WCH_FILES);

                wch_fp[index] = fp;
                wch_filename[index] = filename;
            }

            return fp;
        }

        public static void CloseCampFile(FileStream fp)
        {
            int index;

            for (index = 0; index < MAX_WCH_FILES; index++)
            {
                if (fp == wch_fp[index])
                {
                    // do nothing - we want to keep this file open...
                    // MonoPrint ("Keeping WCH File %s.wch\n", wch_filename);
                    return;
                }
            }

            if (fp == camp_fp)
            {
                if (writing_campressed_file)
                {
                    camp_fp.Seek(0, SeekOrigin.End);
                    camp_size[camp_num_files] = camp_fp.Position - camp_offset[camp_num_files];
                    camp_num_files++;
                }
                else if (reading_campressed_file)
                {
                }
            }
            else
            {
                if (fp != null)
                {
                    fp.Close();
                }
            }
        }

        public static void StartReadCampFile(FalconGameType type, string filename)
        {
            int index, offset;

            string path;

            if (reading_campressed_file)
            {
                log.Debug("Already Reading Campressed File");
                Debug.Assert(!reading_campressed_file);
                return;
            }

            reading_campressed_file = true;
            GetCampFilePath(type, filename, out path);
            camp_game_type = type;
            camp_file_name = filename;

            try
            {
                camp_fp = new FileStream(path, FileMode.Open, FileAccess.Read);

                log.Debug("Opening Campressed File " + path);

                offset = Int32EncodingLE.Decode(camp_fp);
                camp_fp.Seek(offset, SeekOrigin.Begin);
                camp_num_files = Int32EncodingLE.Decode(camp_fp);

                for (index = 0; index < camp_num_files; index++)
                {
                    camp_names[index] = StringASCIIEncodingLE.Decode(camp_fp);
                    camp_offset[index] = Int32EncodingLE.Decode(camp_fp);
                    camp_size[index] = Int32EncodingLE.Decode(camp_fp);

                    log.Debug(camp_names[index] + ", offset = " + camp_offset[index] + ", size = " + camp_size[index]);
                }
            }
            catch (Exception e)
            {
                log.Error("Cannot Open Campressed File " + path + ", reason " + e);
            }
        }

        public static ByteWrapper ReadCampFile(string filename, string ext)
        {
            int index;

            byte[] data;
            string buffer;

            FileStream fp;

            if (reading_campressed_file)
            {
                if (filename != camp_file_name && IsCampFile(camp_game_type, filename))
                {
                    EndReadCampFile();
                    StartReadCampFile(camp_game_type, filename);
                    Debug.Assert(reading_campressed_file);
                }
                buffer = filename + "." + ext;

                for (index = 0; index < camp_num_files; index++)
                {
                    if (string_compare_extensions(buffer, camp_names[index]))
                    {
                        camp_fp.Seek(camp_offset[index], SeekOrigin.Begin);
                        data = camp_fp.ReadBytes((int)camp_size[index]);
                        return new ByteWrapper(data);
                    }
                }

                reading_campressed_file = false;
                fp = OpenCampFile(filename, ext, FileAccess.Read);
                reading_campressed_file = true;
            }
            else
            {
                fp = OpenCampFile(filename, ext, FileAccess.Read);
            }

            if (fp != null)
            {
                fp.Seek(0, SeekOrigin.Begin);
                data = fp.ReadBytes((int)fp.Length);
                fp.Close();

                return new ByteWrapper(data);
            }

            return null;
        }

        public static void EndReadCampFile()
        {
            if (camp_fp != null)
            {
                camp_fp.Close();
                camp_fp = null;
                camp_num_files = 0;
            }
            reading_campressed_file = false;
        }

        public static void StartWriteCampFile(FalconGameType type, string filename)
        {
            string path;

            writing_campressed_file = true;
            if (type == FalconGameType.game_TacticalEngagement)
            {
                path = FalconCampUserSaveDirectory + Sep + filename + ".tac";
            }
            else
            {
                path = FalconCampUserSaveDirectory + Sep + filename + ".cam";
            }

            camp_fp = new FileStream(path, FileMode.Open, FileAccess.Write);

            try
            {
                camp_num_files = 0;
                Int32EncodingLE.Encode(camp_fp, camp_num_files);
            }
            catch (Exception e)
            {
                log.Error("Cannot Open Campressed File " + path + ", reason " + e);
            }
        }

        public static void WriteCampFile(string filename, string ext, string data, int size)
        {
            throw new NotImplementedException();
        }

        public static void EndWriteCampFile()
        {
            int index, offset;

            if (camp_fp != null)
            {
                // MonoPrint ("Writing Campressed File\n");
                offset = (int)camp_fp.Length;

                camp_fp.Seek(0, SeekOrigin.Begin);
                Int32EncodingLE.Encode(camp_fp, offset);
                camp_fp.Seek(offset, SeekOrigin.Begin);
                Int32EncodingLE.Encode(camp_fp, camp_num_files);

                for (index = 0; index < camp_num_files; index++)
                {
                    StringASCIIEncodingLE.Encode(camp_fp, camp_names[index]);
                    Int32EncodingLE.Encode(camp_fp, (int)camp_offset[index]);
                    Int32EncodingLE.Encode(camp_fp, (int)camp_size[index]);
                    // MonoPrint ("  %s %d %d\n", camp_names[index], camp_offset[index], camp_size[index]);
                }

                camp_fp.Close();
                camp_fp = null;
            }

            writing_campressed_file = false;
        }

        public static void ClearCampCache()
        {
            for (int index = 0; index < MAX_WCH_FILES; index++)
            {
                if (wch_fp[index] != null)
                {
                    wch_fp[index].Close();
                    wch_fp[index] = null;
                    wch_filename[index] = null;
                    next_wch_file--;
                }
            }
        }

        #region Private
        static void GetCampFilePath(FalconGameType type, string filename, out string path)
        {
            path = null;

            if (type == FalconGameType.game_TacticalEngagement)
            {
                path = FalconCampUserSaveDirectory + Sep + filename + ".tac";
                if (!File.Exists(path))
                    path = FalconCampUserSaveDirectory + Sep + filename + ".trn";
            }
            else
            {
                path = FalconCampUserSaveDirectory + Sep + filename + ".cam";
            }
        }

        static bool IsCampFile(FalconGameType type, string filename)
        {
            string path;

            GetCampFilePath(type, filename, out path);

            return File.Exists(path);
        }

        private static bool string_compare_extensions(string one, string two)
        {
            string one_ext = Path.GetExtension(one);
            string two_ext = Path.GetExtension(two);
            return one_ext == two_ext;

        }
        // WCH files are constantly opened and closed, since they hold string
        // data... These will now only be closed if another "different" wch file
        // is requested.

        // We allow 4 WCH files - we only currently use two, but given future products
        // I'm setting it to 4 so that they don't not work.
        private const int MAX_WCH_FILES = 4;
        private static int next_wch_file = 0;
        private static FileStream[] wch_fp = new FileStream[MAX_WCH_FILES];
        private static string[] wch_filename = new string[MAX_WCH_FILES];
        // Campressed files ".CAM" & ".TAC" files information is stored in these
        // variables. These are all static, and adjusted by the functions below.
        private static bool writing_campressed_file = false, reading_campressed_file = false;

        private static FileStream camp_fp;
        private static string[] camp_names = new string[32];
        private static string camp_file_name;
        private static int camp_num_files;
        private static long[] camp_offset = new long[32];
        private static long[] camp_size = new long[32];

        private static FalconGameType camp_game_type;

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion
    }
}
