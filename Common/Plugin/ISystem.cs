using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.Common
{
    public interface ISystem
    {
        string Name { get; }
        bool Initialize();
        void Shutdown();
    }
}
