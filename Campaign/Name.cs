using FalconNet.Common.Encoding;
using FalconNet.F4Common;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using VU_BYTE = System.Byte;
namespace FalconNet.Campaign
{

    // Stuff used to deal with naming
    // ***************************************************************************

    // ===============================
    // Global functions
    // ===============================	
    public static class Name
    {
        public static string NameFile;
        public static short NameEntries;
        public static ushort[] NameIndex;
        public static string NameStream = null;

        public static void LoadNames(string filename)
        {
            MemoryStream buf = F4File.ReadCampFile(filename, "idx");
            NameFile = filename;
            NameEntries = Int16EncodingLE.Decode(buf);
            NameIndex = new ushort[NameEntries];
            for (int i = 0; i < NameEntries; i++)
                NameIndex[i] = UInt16EncodingLE.Decode(buf);
            LoadNameStream();
        }

        public static void LoadNameStream()
        {
            Stream fp;

            CampaignStatic.CampEnterCriticalSection();
            fp = F4File.OpenCampFile(NameFile, "wch", FileAccess.Read);

            if (fp != null)
            {
                byte[] buf = new byte[NameIndex[NameEntries - 1]];
                fp.Read(buf, 0, NameIndex[NameEntries - 1]);
                NameStream = Encoding.ASCII.GetString(buf);
                F4File.CloseCampFile(fp);
            }

            CampaignStatic.CampLeaveCriticalSection();
        }

        public static int SaveNames(string filename)
        {
            Stream fp;

            if ((fp = F4File.OpenCampFile(filename, "idx", FileAccess.Write)) == null)
                return 0;
            Int16EncodingLE.Encode(fp, NameEntries);
            for (int i = 0; i < NameEntries; i++)
                UInt16EncodingLE.Encode(fp, NameIndex[i]);

            F4File.CloseCampFile(fp);

            if (NameStream != null)
            {
                if ((fp = F4File.OpenCampFile(filename, "wch", FileAccess.Write)) == null)
                    return 0;
                byte[] buf = Encoding.ASCII.GetBytes(NameStream);
                Debug.Assert(buf.Length != NameIndex[NameEntries - 1]);
                fp.Write(buf, 0, buf.Length);
                F4File.CloseCampFile(fp);
            }

            if ((fp = F4File.OpenCampFile(filename, "txt", FileAccess.Write)) == null)
                return 0;
            TextWriter tw = new StreamWriter(fp);
            for (int i = 0; i < NameEntries - 1; i++)
            {
                string txt = ReadNameString(i);
                //fp.Write"%d %s\n", i, buffer);
                tw.WriteLine(i + " " + txt);
            }

            tw.WriteLine("-1");

            F4File.CloseCampFile(fp);

            return 1;

        }

        public static void FreeNames()
        {
            if (NameIndex != null)
            {
                //delete[] NameIndex;
                NameIndex = null;
            }

            if (NameStream != null)
            {
                //delete[] NameStream;
                NameStream = null;
            }
        }
        public static string ReadNameString(int sid)
        {
            if (NameStream == null)
                LoadNameStream();
            if (sid > NameEntries - 1 || NameIndex[sid] == 0 || NameIndex[sid + 1] - NameIndex[sid] <= 0)
                return "";
            else
                return NameStream.Substring(NameIndex[sid], NameIndex[sid + 1] - NameIndex[sid]);
        }

        public static int AddName(string name)
        {
#if TODO
            int i, nid = 0, len, lastoffset = 0, offset = 0, movesize;
            _TCHAR* newstream;

            len = _tcslen(name);

            // Load our wch file if we don't already have it in memory
            if (!NameStream)
                LoadNameStream();

            // Find a free spot
            for (i = 2; i < NameEntries && !nid; i++)
            {
                // And entry is free if it has a zero (or negative) length
                if (NameIndex[i + 1] - NameIndex[i] <= 0)
                {
                    offset = lastoffset;
                    nid = i;
                }

                if (NameIndex[i])
                    lastoffset = NameIndex[i];
            }

            if (nid)
            {
                // Found a free spot, insert this string
                if (!NameIndex[nid])
                    NameIndex[nid] = (short)offset;

                for (i = nid + 1; i < NameEntries; i++)
                    NameIndex[i] += len;

                movesize = NameIndex[NameEntries - 1] - NameIndex[nid + 1];
            }
            // Otherwise tack on a new entry
            else
            {
                short* TmpIdx;
                nid = NameEntries;
                // Reallocate our memory
                TmpIdx = new short[NameEntries + 1];
                memcpy(TmpIdx, NameIndex, sizeof(short) * NameEntries);
                TmpIdx[nid + 1] = (short)(TmpIdx[nid] + len);
                delete[] NameIndex;
                NameIndex = TmpIdx;
                NameEntries++;
                movesize = 0;
            }

            // Update our name stream
            newstream = new _TCHAR[NameIndex[NameEntries - 1]];
            memcpy(newstream, NameStream, sizeof(_TCHAR) * NameIndex[nid]);

            if (movesize)
                memcpy(&newstream[NameIndex[nid + 1]], &NameStream[NameIndex[nid]], movesize);

            memcpy(&newstream[NameIndex[nid]], name, len);
            delete[] NameStream;
            NameStream = newstream;
            return nid;
#endif
            throw new NotImplementedException();
        }

        public static int SetName(int nameid, string name)
        {
            // It's actually easier to remove our name and add a new one
            if (nameid != 0)
                RemoveName(nameid);

            return AddName(name);
        }

        public static int FindName(string name)
        {
            for (int i = 0; i < NameEntries; i++)
            {
                string entry = ReadNameString(i);

                if (name == entry)
                    return i;
            }

            return 0;
        }

        public static void RemoveName(int nid)
        {
#if TODO
            int movesize, i, len;
            _TCHAR* tmp;

            if (nid < 2)
                return;

            // Load our wch file if we don't already have it in memory
            if (!NameStream)
                LoadNameStream();

            movesize = NameIndex[NameEntries - 1] - NameIndex[nid + 1];
            tmp = new _TCHAR[movesize];
            memcpy(tmp, &NameStream[NameIndex[nid + 1]], movesize);
            memcpy(&NameStream[NameIndex[nid]], tmp, movesize);
            delete[] tmp;

            len = NameIndex[nid + 1] - NameIndex[nid];

            for (i = nid + 1; i < NameEntries; i++)
                NameIndex[i] -= len;
#endif
            throw new NotImplementedException();
        }

        public static void RemoveName(string name)
        {
            int nid;

            nid = FindName(name);
            RemoveName(nid);
        }
    }
}

