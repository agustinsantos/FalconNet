using System;
using System.Collections.Generic;
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

        private FILE* fptr;
        private int rights;
        private int lastOp;
        private SimlibFileName fName;


        public SimlibFileClass();
        public static SimlibFileClass Open(string fname, SIMLIB flags);
        public int ReadLine(char* buf, int maxLen);
        public int WriteLine(char* buf);
        public int Read( byte[] buffer, uint maxLen);
        public int Write(void* buffer, int maxLen);
        public string GetNext();
        public int Close();
        public int Position(int offset, int origin);
    }
}
