using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VuMutex = System.Object;

namespace FalconNet.VU
{
    public struct F4CSECTIONHANDLE
    {
#if TODO
        CRITICAL_SECTION criticalSection;
    HANDLE owningThread;
    //PVOID owningThread;
#endif
        public int count;
        public string name;
#if _DEBUG
    DWORD time;
#endif

    }

    public static class VuxMutex
    {
        public static void VuxLockMutex(VuMutex m)
        {
            F4CriticalSection.F4EnterCriticalSection((F4CSECTIONHANDLE)(m));
        }

        public static void VuxUnlockMutex(VuMutex m)
        {
            F4CriticalSection.F4LeaveCriticalSection((F4CSECTIONHANDLE)(m));
        }

        public static VuMutex VuxCreateMutex(string name)
        {
            return (VuMutex)(F4CriticalSection.F4CreateCriticalSection(name));
        }

        public static void VuxDestroyMutex(VuMutex m)
        {
            F4CriticalSection.F4DestroyCriticalSection((F4CSECTIONHANDLE)(m));
        }
    }
}
