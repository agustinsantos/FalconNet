using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.Sim
{
    public class SimGroundDefinition : SimMoverDefinition
    {

        public SimGroundDefinition(string fileName)
        {
#if TODO
            int i;
            SimlibFileClass gndFile;

            gndFile = SimlibFileClass.Open(fileName, SIMLIB_READ);

            numSensors = atoi(gndFile.GetNext());

#if USE_SH_POOLS
	sensorData = (int *)MemAllocPtr(gReadInMemPool, sizeof(int)*numSensors * 2,0);
#else
            sensorData = new int[numSensors * 2];
#endif
            for (i = 0; i < numSensors; i++)
            {
                sensorData[i * 2] = atoi(gndFile.GetNext());
                sensorData[i * 2 + 1] = atoi(gndFile.GetNext());
            }
            gndFile.Close();
            //TODO delete gndFile;
#endif
            throw new NotImplementedException();
        }
        //TODO public ~SimGroundDefinition (void);
    }
}
