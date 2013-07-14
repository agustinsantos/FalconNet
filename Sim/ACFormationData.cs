using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FalconNet.Sim
{
    /*----------------------------------------------------------*/
    /* Formation information.  All data is relative to the lead */
    /*----------------------------------------------------------*/
    public class ACFormationData
    {
        private const string FORMATION_DATA_FILE = "formdat.fil";

        public struct PositionData
        {
            public int formNum;		// Enum of the Wingman message
            public float relAz;
            public float relEl;
            public float range;
        }


        public ACFormationData()
        {
#if TODO
            SimlibFileClass formFile;
            int i, j;
            int num4Slots;
            int num2Slots;
            int formNum;

            /*---------------------*/
            /* open formation file */
            /*---------------------*/
            formFile = SimlibFileClass.Open(FORMATION_DATA_FILE, SIMLIB.SIMLIB_READ);
            Debug.Assert(formFile != null);
            numFormations = int.Parse(formFile.GetNext());

#if USE_SH_POOLS
	positionData = (PositionData **)MemAllocPtr(gReadInMemPool, sizeof(PositionData*)*numFormations,0);
	twoposData = (PositionData *)MemAllocPtr(gReadInMemPool, sizeof(PositionData)*numFormations,0);
#else
            positionData = new PositionData[numFormations][];
            twoposData = new PositionData[numFormations];
#endif


            for (i = 0; i < numFormations; i++)
            {
                num4Slots = int.Parse(formFile.GetNext());
                num2Slots = int.Parse(formFile.GetNext());
                formNum = int.Parse(formFile.GetNext());

                formFile.GetNext();	// Skip the formation name

#if USE_SH_POOLS
		positionData[i] = (PositionData *)MemAllocPtr(gReadInMemPool, sizeof(PositionData)*num4Slots,0);
#else
                positionData[i] = new PositionData[num4Slots];
#endif

                for (j = 0; j < num4Slots; j++)
                {
                    positionData[i][j].relAz = (float)float.Parse(formFile.GetNext()) * Phyconst.DTR;
                    positionData[i][j].relEl = (float)float.Parse(formFile.GetNext()) * Phyconst.DTR;
                    positionData[i][j].range = (float)float.Parse(formFile.GetNext()) * Phyconst.NM_TO_FT;
                    positionData[i][j].formNum = formNum;
                }
                if (num2Slots != 0)
                {
                    twoposData[i].relAz = (float)float.Parse(formFile.GetNext()) * Phyconst.DTR;
                    twoposData[i].relEl = (float)float.Parse(formFile.GetNext()) * Phyconst.DTR;
                    twoposData[i].range = (float)float.Parse(formFile.GetNext()) * Phyconst.NM_TO_FT;
                    twoposData[i].formNum = formNum;
                }
                else
                {
                    twoposData[i].relAz = positionData[i][0].relAz;
                    twoposData[i].relEl = positionData[i][0].relEl;
                    twoposData[i].range = positionData[i][0].range;
                    twoposData[i].formNum = formNum;
                }
            }

            formFile.Close();
            //TODO delete formFile;
#endif
            throw new NotImplementedException();
        }
        public int numFormations;
        //TODO public ~ACFormationData ();
        public PositionData[][] positionData;
        public PositionData[] twoposData;
        public int FindFormation(int msgNum)
        {
            int i = 0;
            bool done = false;

            while (!done && i < numFormations)
            {
                if (positionData[i][0].formNum == msgNum)
                {
                    done = true;
                }
                else
                {
                    i++;
                }
            }
            if (i == numFormations)
                i = 1;
            Debug.Assert(done == true);	// invalid msgNum or formation file needs updating
            return i;
        }
    }


}
