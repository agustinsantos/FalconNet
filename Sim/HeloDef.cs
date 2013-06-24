using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.Sim
{
    public class SimHeloDefinition : SimMoverDefinition
    {
#if USE_SH_POOLS
	public:
		// Overload new/delete to use a SmartHeap pool
		void *operator new(size_t size) { return MemAllocPtr(gReadInMemPool,size,FALSE);	};
		void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
#endif

        public SimHeloDefinition(string fileName)
        {
            int i;
            SimlibFileClass* heloFile;

            heloFile = SimlibFileClass.Open(fileName, SIMLIB_READ);

            airframeIndex = atoi(heloFile.GetNext());
            numSensors = atoi(heloFile.GetNext());

#if USE_SH_POOLS
	sensorData = (int *)MemAllocPtr(gReadInMemPool, sizeof(int)*numSensors * 2,0);
#else
            sensorData = new int[numSensors * 2];
#endif
            for (i = 0; i < numSensors; i++)
            {
                sensorData[i * 2] = atoi(heloFile.GetNext());
                sensorData[i * 2 + 1] = atoi(heloFile.GetNext());
            }
            heloFile.Close();
            //TODO delete heloFile;
        }
        //TODO public ~SimHeloDefinition (void);
        int airframeIndex;
    }
}
