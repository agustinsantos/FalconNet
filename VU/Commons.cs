using System;

namespace FalconNet.VU
{
    public enum VU_ERRCODE
    {
        VU_ERROR = -1,
        VU_NO_OP = 0,
        VU_SUCCESS = 1
    }

    public static class CriticalSection
    {
        public static void VuEnterCriticalSection()
        {
#if !DEBUG
            throw new NotImplementedException();
#endif
        }
        
        public static void VuExitCriticalSection()
        {
#if !DEBUG
            throw new NotImplementedException();
#endif
        }
        
        public static bool VuHasCriticalSection()
        {
#if !DEBUG
            throw new NotImplementedException();
#endif
            return true;
        }
    }
}

