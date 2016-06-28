using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VU_BYTE = System.Byte;
using VU_TIME = System.UInt64;
using VU_ID_NUMBER = System.UInt32;
using VU_KEY = System.UInt64;
using VU_BOOL = System.Boolean;
using BIG_SCALAR = System.Single;
using SM_SCALAR = System.Single;
using VuMutex = System.Object;

namespace FalconNet.VU
{
    /// <summary>
    /// functions defined by application, used by VU
    /// these are like pure virtual functions which the app must define to use vu
    /// </summary>
    public interface IVux
    {
        // app provided globals
        string vuxWorldName { get; set; }
        uint vuxLocalDomain { get; set; }
        VU_TIME vuxGameTime { get; set; }
        VU_TIME vuxRealTime { get; set; }


        VuEntityType VuxType(ushort id);
        VuMutex VuxCreateMutex(string name);
        void VuxDestroyMutex(VuMutex mutex);
        void VuxLockMutex(VuMutex mutex);
        void VuxUnlockMutex(VuMutex mutex);
        bool VuxAddDanglingSession(VU_ID owner, VU_ADDRESS address);
        int VuxSessionConnect(VuSessionEntity session);
        void VuxSessionDisconnect(VuSessionEntity session);
        int VuxGroupConnect(VuGroupEntity group);
        void VuxGroupDisconnect(VuGroupEntity group);
        int VuxGroupAddSession(VuGroupEntity group, VuSessionEntity session);
        int VuxGroupRemoveSession(VuGroupEntity group, VuSessionEntity session);
        void VuxAdjustLatency(VU_TIME time1, VU_TIME time2);
        VU_ID_NUMBER VuxGetId();
    }
}
