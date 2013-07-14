using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.Sim
{
    public class SimMoverDefinition
    {
#if USE_SH_POOLS
	public:
		// Overload new/delete to use a SmartHeap pool
		void *operator new(size_t size) { return MemAllocPtr(gReadInMemPool,size,FALSE);	};
		void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
#endif

        public enum MoverType { Aircraft, Ground, Helicopter, Weapon, Sea };
        public SimMoverDefinition()
        {
            numSensors = 0;
            sensorData = null;
        }

        //TODO public virtual ~SimMoverDefinition();
        public static void ReadSimMoverDefinitionData()
        {
#if TODO
            int i;
            SimlibFileClass vehList;
            MoverType vehicleType;

            vehList = SimlibFileClass.Open(SIM_VEHICLE_DEFINITION_FILE, SIMLIB_READ);

            NumSimMoverDefinitions = atoi(vehList.GetNext());
#if USE_SH_POOLS
	moverDefinitionData = (SimMoverDefinition **)MemAllocPtr(gReadInMemPool, sizeof(SimMoverDefinition*)*NumSimMoverDefinitions,0);
#else
            moverDefinitionData = new SimMoverDefinition[NumSimMoverDefinitions];
#endif

            for (i = 0; i < NumSimMoverDefinitions; i++)
            {
                vehicleType = Enum.Parse(typeof(MoverType), vehList.GetNext());
                switch (vehicleType)
                {
                    case MoverType.Aircraft:
                        moverDefinitionData[i] = new SimACDefinition(vehList.GetNext());
                        break;

                    case MoverType.Ground:
                        moverDefinitionData[i] = new SimGroundDefinition(vehList.GetNext());
                        break;

                    case MoverType.Helicopter:
                        moverDefinitionData[i] = new SimHeloDefinition(vehList.GetNext());
                        break;

                    case MoverType.Weapon:
                        moverDefinitionData[i] = new SimWpnDefinition(vehList.GetNext());
                        break;

                    case MoverType.Sea:
                        vehList.GetNext();
                        moverDefinitionData[i] = new SimMoverDefinition();
                        break;

                    default:
                        vehList.GetNext();
                        moverDefinitionData[i] = new SimMoverDefinition();
                        break;
                }
            }
            vehList.Close();
            //TODO delete vehList;
#endif
            throw new NotImplementedException();
        }

        public static void FreeSimMoverDefinitionData()
        {
#if TODO
int i;

	for (i=0; i<NumSimMoverDefinitions; i++)
	{
      delete moverDefinitionData[i];
   }

#if USE_SH_POOLS
	MemFreePtr( moverDefinitionData );
#else
    delete [] moverDefinitionData;
#endif
#endif
        }
        public int numSensors;
        public int[] sensorData;

        public static SimMoverDefinition[] moverDefinitionData;
        public static int NumSimMoverDefinitions;

    }
}
