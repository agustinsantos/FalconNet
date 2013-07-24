using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.F4Common
{
    public class PlayerManager
    {
        private static readonly PlayerManager instance = new PlayerManager();

        private PlayerManager() { }

        public static PlayerManager Instance { get { return instance; } }

    }
}
