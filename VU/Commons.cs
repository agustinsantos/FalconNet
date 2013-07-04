using System;

namespace FalconNet.VU
{
    public enum VU_ERRCODE
    {
        VU_ERROR = -1,
        VU_NO_OP = 0,
        VU_SUCCESS = 1
    }

    public class F4LIt { }

    public struct VuFullUpdateevnt { }
    public struct VuPositionUpdateevnt { }
    public struct VuTransferevnt { }

    public static class CriticalSection
    {
        public static void VuEnterCriticalSection()
        {
            throw new NotImplementedException();
        }
        
        public static void VuExitCriticalSection()
        {
            throw new NotImplementedException();
        }
        
        public static bool VuHasCriticalSection()
        {
            throw new NotImplementedException();
        }
    }
}

