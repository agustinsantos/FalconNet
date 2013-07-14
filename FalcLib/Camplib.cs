using System;
using System.IO;
using FalconNet.Common;
using FalconNet.VU;
using System.Diagnostics;
using FalconNet.CampaignBase;
using FalconNet.F4Common;


namespace FalconNet.FalcLib
{

    // =====================================
    // Campaign defines and typedefs
    // =====================================

    /* TODO
    #define MONOMODE_OFF		0
    #define MONOMODE_TEXT		1
    #define MONOMODE_MAP		2

    class UnitClass;
    typedef UnitClass* Unit;

    // =====================================
    // Vu shortcuts
    // =====================================

    typedef FalconPrivateList*			F4PFList;
    typedef FalconPrivateOrderedList*	F4POList;
    typedef VuListIterator*				F4LIt;
    TODO */

    // =====================================
    // Campaign Library Functions
    // =====================================

    public static class Camplib
    {
        public const int VEHICLES_PER_UNIT = 16;
        public const int FEATURES_PER_OBJ = 32;
        public const int MAXIMUM_ROLES = 16;
        public const int MAXIMUM_OBJTYPES = 32;
        public const int MAXIMUM_WEAPTYPES = 600;
        public const int MAX_UNIT_CHILDREN = 5;
        public const int MAX_FEAT_DEPEND = 5;

        // JB 010709 Increase for larger theaters
        /*
        #define MAX_NUMBER_OF_OBJECTIVES		4000
        #define MAX_NUMBER_OF_UNITS				2000	// Max # of NON volitile units only
        #define MAX_NUMBER_OF_VOLITILE_UNITS	8000
        #define MAX_CAMP_ENTITIES				(MAX_NUMBER_OF_OBJECTIVES+MAX_NUMBER_OF_UNITS+MAX_NUMBER_OF_VOLITILE_UNITS)
        */
        public const int MAX_NUMBER_OF_OBJECTIVES = 8000;
        public const int MAX_NUMBER_OF_UNITS = 4000;	// Max # of NON volitile units only
        public const int MAX_NUMBER_OF_VOLITILE_UNITS = 16000;
        public const int MAX_CAMP_ENTITIES = (MAX_NUMBER_OF_OBJECTIVES + MAX_NUMBER_OF_UNITS + MAX_NUMBER_OF_VOLITILE_UNITS);

        public static void Camp_Init(int processor)
        {
            throw new NotImplementedException();
        }

        public static void Camp_Exit()
        {
            throw new NotImplementedException();
        }

        public static void Camp_SetPlayerSquadron(Unit squadron)
        {
            throw new NotImplementedException();
        }

        public static Unit Camp_GetPlayerSquadron()
        {
            throw new NotImplementedException();
        }

        public static VuEntity Camp_GetPlayerEntity()
        {
            throw new NotImplementedException();
        }

        public static CampaignTime Camp_GetCurrentTime()
        {
            throw new NotImplementedException();
        }

        public static void Camp_SetCurrentTime(double newTime)
        {
            throw new NotImplementedException();
        }

        public static void Camp_FreeMemory()
        {
            throw new NotImplementedException();
        }

        public static FileStream OpenCampFile(string filename, string ext, FileAccess access)
        {
#if TODO
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
#endif 
            throw new NotImplementedException();
        }

        static bool string_compare_extensions(string one, string two)
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

        public const int MAX_WCH_FILES = 4;

        public static int next_wch_file = 0;

        public static FileStream[] wch_fp = new FileStream[MAX_WCH_FILES];

        public static string[] wch_filename = new string[MAX_WCH_FILES];

        // Campressed files ".CAM" & ".TAC" files information is stored in these
        // variables. These are all static, and adjusted by the functions below.

        static bool writing_campressed_file = false, reading_campressed_file = false;

        static FileStream camp_fp;
        static string[] camp_names = new string[32];
        static string camp_file_name;
        static int camp_num_files;
        static long[] camp_offset = new long[32];
        static int[] camp_size = new int[32];

        static FalconGameType camp_game_type;
    }

    public class Unit
    {
        public VU_ID id() { throw new NotImplementedException(); }
        //TODO There is another definition in Campaign lib
    }
}