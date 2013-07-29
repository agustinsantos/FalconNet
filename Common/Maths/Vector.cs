using FalconNet.Common.Encoding;
using System;
using System.Collections.Generic;
using System.IO;
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

    public static class vectorEncodingLE
    {
        public static void Encode(Stream stream, vector val)
        {
            SingleEncodingLE.Encode(stream, val.x);
            SingleEncodingLE.Encode(stream, val.y);
            SingleEncodingLE.Encode(stream, val.z);
        }

        public static void Decode(Stream stream, ref vector rst)
        {
            rst.x = SingleEncodingLE.Decode(stream);
            rst.y = SingleEncodingLE.Decode(stream);
            rst.z = SingleEncodingLE.Decode(stream);
        }

        public static int Size
        {
            get { return 12; }
        }
    }
}
