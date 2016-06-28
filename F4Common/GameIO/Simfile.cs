using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FalconNet.F4Common
{
    public enum SIMLIB
    {
        SIMLIB_MAX_OPEN_FILES = 20,
        SIMLIB_UPDATE = 0x1,
        SIMLIB_CREATE = 0x2,
        SIMLIB_READ = 0x4,
        SIMLIB_WRITE = 0x8,
        SIMLIB_BINARY = 0x10,
        SIMLIB_READWRITE = (SIMLIB_READ & SIMLIB_WRITE)
    }

    public class SimlibFileClass
    {
        private FileStream fptr;
        private StreamReader txtReader;
        private StreamWriter txtWriter;
        private FileMode mode;
        private FileAccess access;
        private int lastOp;
        private string fName;


        public SimlibFileClass()
        {
            fptr = null;
            lastOp = -1;
        }

        public static SimlibFileClass Open(string fname, FileMode m, FileAccess acc)
        {
            SimlibFileClass file = new SimlibFileClass();
            file.mode = m;
            file.access = acc;
            if (!File.Exists(fname))
                return null;
               // throw new ArgumentException("Check your installation. File not found:" + fname);
            file.fptr = new FileStream(fname, file.mode, file.access);
            if (file.access == FileAccess.Read)
            {
                file.txtReader = new StreamReader(file.fptr);
            }
            else if (file.access == FileAccess.Write)
            {
                file.txtWriter = new StreamWriter(file.fptr);
            }

            file.fName = fname;
            return file;
        }

        public string ReadLine()
        {
            return txtReader.ReadLine();
        }

        public void WriteLine(string line)
        {
            txtWriter.WriteLine(line);
        }

        public int Read(byte[] buffer, uint maxLen)
        {
            throw new NotImplementedException();
        }

        public int Write(byte[] buffer, int maxLen)
        {
            throw new NotImplementedException();
        }

        public string GetNext()
        {
            bool isComment = true; 
            string line;
            do
            {
                line = ReadLine();
                if (line == null) return null;
                if (line.StartsWith(";") || line.StartsWith("#"))
                    isComment = true;
                else
                    isComment = false;
            } while (isComment);
            return line;
        }

        public void Close()
        {
            fptr.Close();
            fptr = null;
        }

        public int Position(int offset, int origin)
        {
            throw new NotImplementedException();
        }

    }
}
