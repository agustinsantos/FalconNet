using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.Graphics
{
    /// <summary>
    /// Provides custom code for use by specific BSPlib objects.
    /// </summary>
    public static class Scripts
    {
        public delegate void ScriptFunction();

        public static ScriptFunction[] ScriptArray;
        public static int ScriptArrayLength;
    }
}
