using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.Common.Maths
{
    //TODO this class was originally at F4VU. It should be completed and renamed
    public struct vector
    {
        public float x;
        public float y;
        public float z;

        public vector(float px, float py, float pz)
        {
            x = px;
            y = py;
            z = pz;
        }
    }
}
