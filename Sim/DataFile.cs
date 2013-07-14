using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.Sim
{
          public enum format { ID_INT, ID_FLOAT, ID_STRING, ID_CHAR, ID_VECTOR }
  public struct InputDataDesc
    {
        string name; // name of the key in the .dat file
        public format type; // what format it is
        public uint offset; // offset into the associated structure
        public object defvalue; // default value
    }

    public class FileReader
    {
        private readonly InputDataDesc m_desc;
        private InputDataDesc FindField(string key)
        {
            throw new NotImplementedException();
        }

        public FileReader(InputDataDesc desc) { m_desc = desc; }
        
        public virtual void Initialise(byte[] dataPtr)
        {
            throw new NotImplementedException();
        }

        public virtual bool ParseField(byte[] dataPtr, string line)
        {
            throw new NotImplementedException();
        }

        public bool AssignField(InputDataDesc desc, byte[] dataPtr, object value)
        {
            throw new NotImplementedException();
        }

        public static InputDataDesc FindField(InputDataDesc desc, string key)
        {
            throw new NotImplementedException();
        }

        public static bool ParseField(byte[] dataPtr, string line, InputDataDesc desc)
        {
            throw new NotImplementedException();
        }

        public static void Initialise(byte[] dataPtr, InputDataDesc desc)
        {
            throw new NotImplementedException();
        }

        public static bool ParseSimlibFile(byte[] dataPtr, InputDataDesc desc, SimlibFileClass inputFile)
        {
            throw new NotImplementedException();
        }
    }
}
