using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FalconNet.Common.Maths
{
    public static class MathUtil
    {
        public static float Smooth(float newVal, float curVal, float maxChange)
        {
            float diff = newVal - curVal;

            if (Math.Abs(diff) > maxChange)
            {
                if (diff > 0.0f)
                {
                    curVal += maxChange;

                    if (curVal > newVal)
                    {
                        curVal = newVal;
                    }
                }
                else if (diff < 0.0f)
                {
                    curVal -= maxChange;

                    if (curVal < newVal)
                    {
                        curVal = newVal;
                    }
                }
            }
            else
            {
                curVal = newVal;
            }

            return curVal;
        }

        public static float Clamp(float val, float lower, float upper)
        {
            if (val < lower) return lower;
            else if (val > upper) return upper;
            else return val;
        }
        public static void Clamp(ref float val, float lower, float upper)
        {
            if (val < lower) val = lower;
            else if (val > upper) val = upper;
         }

        public static float Clamp(float val)
        {
            if (val < 0.0f) return 0.0f;
            else if (val > 1.0f) return 1.0f;
            else return val;
        }
    }
}
