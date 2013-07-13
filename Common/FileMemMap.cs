using System;
using DWORD = System.UInt32;
using WORD = System.UInt16;
using BOOL = System.Boolean;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;


namespace FalconNet.Common
{

    public class FileMemMap
    {
        FileStream m_hFile; // the open file
        MemoryMappedFile m_hMap; // the mapping handle
        MemoryMappedViewAccessor m_Data; // the mapped data
        long m_len; // the length of the data

        void Clear()
        {
            m_hFile = null;
            m_hMap = null;
            m_Data = null;
            m_len = 0;
        }


        public FileMemMap()
        {
            Clear();
        }
        //public ~FileMemMap();

        public BOOL Open(string filename, BOOL rw = false, BOOL nomap = false)
        {
            FileAccess access = (rw ? FileAccess.Write : FileAccess.Read);
            FileShare share = (rw ? FileShare.Write : FileShare.Read);
            m_hFile = new FileStream(filename, FileMode.Open, access, share);

            m_len = m_hFile.Length;

            if (nomap)
                return true; // we don't really want to map it

            m_hMap = MemoryMappedFile.CreateFromFile(filename);
            MemoryMappedViewAccessor m_Data = m_hMap.CreateViewAccessor();

            return m_Data == null ? false : true;
        }


        // release stoarage and stuff
        public void Close()
        {
            if (m_Data != null) m_Data.Dispose();

            if (m_hMap != null) m_hMap.Dispose();

            if (m_hFile != null) m_hFile.Close();

            Clear();
        }

        public byte[] GetData(int offset, int len)
        {
            if (m_hMap == null || offset < 0 || offset + len > m_len)
                return null;
            throw new NotImplementedException();
            //return m_Data[offset];
        }

        public FileStream GetFileHandle()
        {
            return m_hFile;
        }

        public BOOL ReadDataAt(DWORD offset, byte[] buffer, DWORD size)
        {
            throw new NotImplementedException();
        }

        public BOOL IsReady()
        {
            return m_hFile != null;
        }
    }
}
