using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FalconNet.Sim
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
        private int rights;
        private int lastOp;
        private string fName;


        public SimlibFileClass()
        {
            fptr = null;
            rights = 0;
            lastOp = -1;
        }

        public static SimlibFileClass Open(string fname, FileMode mode, FileAccess access)
        {
            throw new NotImplementedException();
        }

        public int ReadLine(ref char[] buf, int maxLen)
        {
            throw new NotImplementedException();
        }

        public int WriteLine(char[] buf)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public int Close()
        {
            throw new NotImplementedException();
        }

        public int Position(int offset, int origin)
        {
            throw new NotImplementedException();
        }

    }
}
