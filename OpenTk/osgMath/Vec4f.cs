﻿using System;
using System.Globalization;

namespace OSGNet.Osg
{
    public class Vec4f
    {
        /** Vec member variable. */
        float[] _v = new float[4];


        /** Constructor that sets all components of the vector to zero */
        public Vec4f() { _v[0] = 0; _v[1] = 0; _v[2] = 0; _v[3] = 0; }
        public Vec4f(float x, float y, float z, float w) { _v[0] = x; _v[1] = y; _v[2] = z; _v[3] = w; }
        public Vec4f(Vec3f v3, float w)
        {
            _v[0]=v3[0];
            _v[1]=v3[1];
            _v[2]=v3[2];
            _v[3]=w;
        }

        public static bool operator ==(Vec4f l, Vec4f r) { return l._v[0] == r._v[0] && l._v[1] == r._v[1] && l._v[2] == r._v[2] && l._v[3] == r._v[3]; }

        public static bool operator !=(Vec4f l, Vec4f r) { return l._v[0] != r._v[0] || l._v[1] != r._v[1] || l._v[2] != r._v[2] || l._v[3] != r._v[3]; }

        public static bool operator <(Vec4f l, Vec4f r)
        {
            if (l._v[0] < r._v[0]) return true;
            else if (l._v[0] > r._v[0]) return false;
            else if (l._v[1] < r._v[1]) return true;
            else if (l._v[1] > r._v[1]) return false;
            else if (l._v[2] < r._v[2]) return true;
            else if (l._v[2] > r._v[2]) return false;
            else return (l._v[3] < r._v[3]);
        }
        public static bool operator >(Vec4f l, Vec4f r)
        {
            if (l._v[0] > r._v[0]) return true;
            else if (l._v[0] < r._v[0]) return false;
            else if (l._v[1] > r._v[1]) return true;
            else if (l._v[1] < r._v[1]) return false;
            else if (l._v[2] > r._v[2]) return true;
            else if (l._v[2] < r._v[2]) return false;
            else return (l._v[3] > r._v[3]);
        }

        public void Set(float x, float y, float z, float w) { _v[0] = x; _v[1] = y; _v[2] = z; _v[3] = z; }
        public float this[int i]
        {
            get
            {
                return _v[i];
            }
            set
            {
                _v[i] = value;
            }
        }

        public float X { get { return _v[0]; } set { _v[0] = value; } }
        public float Y { get { return _v[1]; } set { _v[1] = value; } }
        public float Z { get { return _v[2]; } set { _v[2] = value; } }
        public float W { get { return _v[3]; } set { _v[3] = value; } }

        public float R { get { return _v[0]; } set { _v[0] = value; } }
        public float G { get { return _v[1]; } set { _v[1] = value; } }
        public float B { get { return _v[2]; } set { _v[2] = value; } }
        public float A { get { return _v[3]; } set { _v[3] = value; } }

        private float ClampTo(float v, float min, float max)
        {
            if (v < min) return min;
            if (v > max) return max;
            return v;
        }

        public uint AsABGR()  
        {
            return (uint)ClampTo((_v[0]*255.0f),0.0f,255.0f)<<24 |
                   (uint)ClampTo((_v[1]*255.0f),0.0f,255.0f)<<16 |
                   (uint)ClampTo((_v[2]*255.0f),0.0f,255.0f)<<8  |
                   (uint)ClampTo((_v[3]*255.0f),0.0f,255.0f);
        }

        /** Returns true if all components have values that are not NaN. */
        public bool IsValid() { return !IsNaN(); }
        /** Returns true if at least one component has value NaN. */
        public bool IsNaN() { return float.IsNaN(_v[0]) || float.IsNaN(_v[1]) || float.IsNaN(_v[2]) || float.IsNaN(_v[3]); }

        /** Dot product. */
        public static float operator *(Vec4f lhs, Vec4f rhs)
        {
            return lhs._v[0] * rhs._v[0] + lhs._v[1] * rhs._v[1] + lhs._v[2] * rhs._v[2] + lhs._v[3] * rhs._v[3];
        }

        /** Multiply by scalar. */
        public static Vec4f operator *(Vec4f lhs, float rhs)
        {
            return new Vec4f(lhs._v[0] * rhs, lhs._v[1] * rhs, lhs._v[2] * rhs, lhs._v[3] * rhs);
        }


        /** Divide by scalar. */
        public static Vec4f operator /(Vec4f lhs, float rhs)
        {
            return new Vec4f(lhs._v[0] / rhs, lhs._v[1] / rhs, lhs._v[2] / rhs, lhs._v[3] / rhs);
        }


        /** Binary vector add. */
        public static Vec4f operator +(Vec4f lhs, Vec4f rhs)
        {
            return new Vec4f(lhs._v[0] + rhs._v[0], lhs._v[1] + rhs._v[1], lhs._v[2] + rhs._v[2], lhs._v[3] + rhs._v[3]);
        }


        /** Binary vector subtract. */
        public static Vec4f operator -(Vec4f lhs, Vec4f rhs)
        {
            return new Vec4f(lhs._v[0] - rhs._v[0], lhs._v[1] - rhs._v[1], lhs._v[2] - rhs._v[2], lhs._v[3] - rhs._v[3]);
        }

        /** Unary vector operations. Slightly more efficient because no temporary
         * floatermediate object.
       */
        public Vec4f Add(Vec4f rhs)
        {
            _v[0] += rhs._v[0];
            _v[1] += rhs._v[1];
            _v[2] += rhs._v[2];
            _v[3] += rhs._v[3];
            return this;
        }
        public Vec4f Substract(Vec4f rhs)
        {
            _v[0] -= rhs._v[0];
            _v[1] -= rhs._v[1];
            _v[2] += rhs._v[2];
            _v[3] += rhs._v[3]; 
            return this;
        }
        public Vec4f Mul(float rhs)
        {
            _v[0] *= rhs;
            _v[1] *= rhs;
            _v[2] *= rhs;
            _v[3] *= rhs;
            return this;
        }
        public Vec4f Div(float rhs)
        {
            _v[0] /= rhs;
            _v[1] /= rhs;
            _v[2] /= rhs;
            _v[3] /= rhs;
            return this;
        }
        /** multiply by vector components. */
        public static Vec4f ComponentMultiply(Vec4f lhs, Vec4f rhs)
        {
            return new Vec4f(lhs[0] * rhs[0], lhs[1] * rhs[1], lhs[2] * rhs[2], lhs[3] * rhs[3]);
        }

        /** divide rhs components by rhs vector components. */
        public static Vec4f ComponentDivide(Vec4f lhs, Vec4f rhs)
        {
            return new Vec4f(lhs[0] / rhs[0], lhs[1] / rhs[1], lhs[2] / rhs[2], lhs[3] / rhs[3]);
        }
        /** Length of the vector = sqrt( vec . vec ) */
        public float Length
        {
            get { return (float)Math.Sqrt(_v[0] * _v[0] + _v[1] * _v[1] + _v[2] * _v[2] + _v[3] * _v[3]); }
        }

        /** Length squared of the vector = vec . vec */
        public float Length2
        {
            get { return _v[0] * _v[0] + _v[1] * _v[1] + _v[2] * _v[2] + _v[3] * _v[3]; }
        }

        public float Normalize()
        {
            float norm = this.Length;
            if (norm>0.0)
            {
                float inv = 1.0f / norm;
                _v[0] *= inv;
                _v[1] *= inv;
                _v[2] *= inv;
                _v[3] *= inv;
            }
            return( norm );
        }
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "({0}, {1}, {2}, {3})", _v[0], _v[1], _v[2], _v[3]);
        }
        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Pofloat return false.
            Vec4f p = obj as Vec4f;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (_v[0] == p._v[0]) && (_v[1] == p._v[1]) && (_v[2] == p._v[2]) && (_v[3] == p._v[3]);
        }

        public bool Equals(Vec4f p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (_v[0] == p._v[0]) && (_v[1] == p._v[1]) && (_v[2] == p._v[2]) && (_v[3] == p._v[3]);
        }

        public override int GetHashCode()
        {
            return _v[0].GetHashCode() ^ _v[1].GetHashCode() ^ _v[2].GetHashCode() ^ _v[3].GetHashCode();
        }
    }    // end of class Vec4f

}
