using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.F4Common
{
    public struct InputDataDesc
    {
        public string name; // name of the key in the .dat file
        public format type; // what format it is
        public Action<object, object> action; 
        public object defvalue; // default value
    }
}
