using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.Sim
{
    public class SimWpnDefinition : SimMoverDefinition
    {
#if USE_SH_POOLS
	public:
		// Overload new/delete to use a SmartHeap pool
		void *operator new(size_t size) { return MemAllocPtr(gReadInMemPool,size,FALSE);	};
		void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
#endif

        public SimWpnDefinition(string fileName)
        {
#if TODO
            SimlibFileClass* wpnFile;

            wpnFile = SimlibFileClass.Open(fileName, SIMLIB_READ);

            flags = atoi(wpnFile.GetNext());
            cd = (float)atof(wpnFile.GetNext());
            weight = (float)atof(wpnFile.GetNext());
            area = (float)atof(wpnFile.GetNext());
            xEjection = (float)atof(wpnFile.GetNext());
            yEjection = (float)atof(wpnFile.GetNext());
            zEjection = (float)atof(wpnFile.GetNext());
            strcpy(mnemonic, wpnFile.GetNext());
            weaponClass = atoi(wpnFile.GetNext());
            domain = atoi(wpnFile.GetNext());
            weaponType = atoi(wpnFile.GetNext());
            dataIdx = atoi(wpnFile.GetNext());

            wpnFile.Close();
            //TODO delete wpnFile;
#endif
            throw new NotImplementedException();
        }

        //TODO public ~SimWpnDefinition (void);
        public int flags;
        public float cd;
        public float weight;
        public float area;
        public float xEjection;
        public float yEjection;
        public float zEjection;
        public string mnemonic; //[8];
        public int weaponClass;
        public int domain;
        public int weaponType;
        public int dataIdx;
    }
}
