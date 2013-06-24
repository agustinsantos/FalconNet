using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.Sim
{
    public class SimACDefinition : SimMoverDefinition
    {
#if USE_SH_POOLS
	public:
		// Overload new/delete to use a SmartHeap pool
		void *operator new(size_t size) { return MemAllocPtr(gReadInMemPool,size,FALSE);	};
		void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
#endif

        // NOTE!!!: This matches the list in digi.h for Maneuver class
        public enum CombatClass { F4, F5, F14, F15, F16, Mig25, Mig27, A10, Bomber };
        public CombatClass combatClass;

        public SimACDefinition(string fileName)
        {
            int i;
            SimlibFileClass* acFile;

            acFile = SimlibFileClass.Open(fileName, SIMLIB_READ);

            // What type of combat does it do?
            combatClass = (CombatClass)atoi(acFile.GetNext());

            airframeIndex = atoi(acFile.GetNext());
            numPlayerSensors = atoi(acFile.GetNext());

#if USE_SH_POOLS
	playerSensorData = (int *)MemAllocPtr(gReadInMemPool, sizeof(int)*numPlayerSensors * 2,0);
#else
            playerSensorData = new int[numPlayerSensors * 2];
#endif
            for (i = 0; i < numPlayerSensors; i++)
            {
                playerSensorData[i * 2] = atoi(acFile.GetNext());
                playerSensorData[i * 2 + 1] = atoi(acFile.GetNext());
            }

            numSensors = atoi(acFile.GetNext());

#if USE_SH_POOLS
	sensorData = (int *)MemAllocPtr(gReadInMemPool, sizeof(int)*numSensors * 2,0);
#else
            sensorData = new int[numSensors * 2];
#endif
            for (i = 0; i < numSensors; i++)
            {
                sensorData[i * 2] = atoi(acFile.GetNext());
                sensorData[i * 2 + 1] = atoi(acFile.GetNext());
            }

            acFile.Close();
            //TODO delete acFile;
        }

        // TODO public ~SimACDefinition (void);
        public int airframeIndex;
        public int numPlayerSensors;
        public int[] playerSensorData;
    }
}
