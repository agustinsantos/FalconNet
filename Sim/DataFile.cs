using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.Sim
{
    public struct InputDataDesc
    {
        string name; // name of the key in the .dat file
        public format type; // what format it is
        public uint offset; // offset into the associated structure
        public char* defvalue; // default value
        public enum format { ID_INT, ID_FLOAT, ID_STRING, ID_CHAR, ID_VECTOR }
    }

    public class FileReader
    {
        private readonly InputDataDesc m_desc;
        private InputDataDesc FindField(char* key);

        public FileReader(InputDataDesc desc) { m_desc = desc; }
        public virtual void Initialise(void* dataPtr);
        public virtual bool ParseField(void* dataPtr, char* line);
        public bool AssignField(InputDataDesc desc, void* dataPtr, char* value);


        public static bool AssignField(InputDataDesc field, void* dataPtr, char* value);
        public static InputDataDesc FindField(InputDataDesc desc, char* key);
        public static bool ParseField(void* dataPtr, char* line, InputDataDesc desc);
        public static void Initialise(void* dataPtr, InputDataDesc desc);
        public static bool ParseSimlibFile(void* dataPtr, InputDataDesc desc, SimlibFileClass inputFile);
    }
}
