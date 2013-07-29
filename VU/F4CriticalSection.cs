using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.VU
{
    public static class F4CriticalSection
    {
        /** creates a critical section object (mutex) */
        public static F4CSECTIONHANDLE F4CreateCriticalSection(string mutexName)
        {
            return new F4CSECTIONHANDLE() { name = mutexName };
        }

    /** destroys a critical section object */
        public static void F4DestroyCriticalSection(F4CSECTIONHANDLE theSection)
        {
            throw new NotImplementedException();
        }

    /** locks the critical section or sleeps until the lock is released */
        public static void F4EnterCriticalSection(F4CSECTIONHANDLE theSection)
        {
            throw new NotImplementedException();
        }

    /** leaves the critical section */
        public static void F4LeaveCriticalSection(F4CSECTIONHANDLE theSection)
        {
            throw new NotImplementedException();
        }

    /** returns if the thread has the critical section locked. */
        public static int F4CheckHasCriticalSection(F4CSECTIONHANDLE theSection)
        {
            throw new NotImplementedException();
        }
    }
}
