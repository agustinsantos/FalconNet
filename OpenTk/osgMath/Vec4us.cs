﻿using System;
using System.Globalization;

namespace OSGNet.Osg
{
    public class Vec4us
    {
        /** Vec member variable. */
        ushort[] _v = new ushort[4];


        /** Constructor that sets all components of the vector to zero */
        public Vec4us() { _v[0] = 0; _v[1] = 0; _v[2] = 0; _v[3] = 0; }
        public Vec4us(ushort x, ushort y, ushort z, ushort w) { _v[0] = x; _v[1] = y; _v[2] = z; _v[3] = w; }
        public Vec4us(int x, int y, int z, int w) { _v[0] = (ushort)x; _v[1] = (ushort)y; _v[2] = (ushort)z; _v[3] = (ushort)w; }
        public Vec4us(Vec3us v3, ushort w)
        {
            _v[0]=v3[0];
            _v[1]=v3[1];
            _v[2]=v3[2];
            _v[3]=w;
        }

        public static bool operator ==(Vec4us l, Vec4us r) { return l._v[0] == r._v[0] && l._v[1] == r._v[1] && l._v[2] == r._v[2] && l._v[3] == r._v[3]; }

        public static bool operator !=(Vec4us l, Vec4us r) { return l._v[0] != r._v[0] || l._v[1] != r._v[1] || l._v[2] != r._v[2] || l._v[3] != r._v[3]; }

        public static bool operator <(Vec4us l, Vec4us r)
        {
            if (l._v[0] < r._v[0]) return true;
            else if (l._v[0] > r._v[0]) return false;
            else if (l._v[1] < r._v[1]) return true;
            else if (l._v[1] > r._v[1]) return false;
            else if (l._v[2] < r._v[2]) return true;
            else if (l._v[2] > r._v[2]) return false;
            else return (l._v[3] < r._v[3]);
        }
        public static bool operator >(Vec4us l, Vec4us r)
        {
            if (l._v[0] > r._v[0]) return true;
            else if (l._v[0] < r._v[0]) return false;
            else if (l._v[1] > r._v[1]) return true;
            else if (l._v[1] < r._v[1]) return false;
            else if (l._v[2] > r._v[2]) return true;
            else if (l._v[2] < r._v[2]) return false;
            else return (l._v[3] > r._v[3]);
        }

        public void Set(ushort x, ushort y, ushort z, ushort w) { _v[0] = x; _v[1] = y; _v[2] = z; _v[3] = z; }
        public ushort this[int i]
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

        public ushort X { get { return _v[0]; } set { _v[0] = value; } }
        public ushort Y { get { return _v[1]; } set { _v[1] = value; } }
        public ushort Z { get { return _v[2]; } set { _v[2] = value; } }
        public ushort W { get { return _v[3]; } set { _v[3] = value; } }

        public ushort R { get { return _v[0]; } set { _v[0] = value; } }
        public ushort G { get { return _v[1]; } set { _v[1] = value; } }
        public ushort B { get { return _v[2]; } set { _v[2] = value; } }
        public ushort A { get { return _v[3]; } set { _v[3] = value; } }

        /** Returns true if all components have values that are not NaN. */
        public bool IsValid() { return !IsNaN(); }
        /** Returns true if at least one component has value NaN. */
        public bool IsNaN() { return false; }

        /** Dot product. */
        public static int operator *(Vec4us lhs, Vec4us rhs)
        {
            return lhs._v[0] * rhs._v[0] + lhs._v[1] * rhs._v[1] + lhs._v[2] * rhs._v[2] + lhs._v[3] * rhs._v[3];
        }

        /** Multiply by scalar. */
        public static Vec4us operator *(Vec4us lhs, ushort rhs)
        {
            return new Vec4us(lhs._v[0] * rhs, lhs._v[1] * rhs, lhs._v[2] * rhs, lhs._v[3] * rhs);
        }


        /** Divide by scalar. */
        public static Vec4us operator /(Vec4us lhs, ushort rhs)
        {
            return new Vec4us(lhs._v[0] / rhs, lhs._v[1] / rhs, lhs._v[2] / rhs, lhs._v[3] / rhs);
        }


        /** Binary vector add. */
        public static Vec4us operator +(Vec4us lhs, Vec4us rhs)
        {
            return new Vec4us(lhs._v[0] + rhs._v[0], lhs._v[1] + rhs._v[1], lhs._v[2] + rhs._v[2], lhs._v[3] + rhs._v[3]);
        }


        /** Binary vector subtract. */
        public static Vec4us operator -(Vec4us lhs, Vec4us rhs)
        {
            return new Vec4us(lhs._v[0] - rhs._v[0], lhs._v[1] - rhs._v[1], lhs._v[2] - rhs._v[2], lhs._v[3] - rhs._v[3]);
        }

        /** Unary vector operations. Slightly more efficient because no temporary
         * ushortermediate object.
       */
        public Vec4us Add(Vec4us rhs)
        {
            _v[0] += rhs._v[0];
            _v[1] += rhs._v[1];
            _v[2] += rhs._v[2];
            _v[3] += rhs._v[3];
            return this;
        }
        public Vec4us Substract(Vec4us rhs)
        {
            _v[0] -= rhs._v[0];
            _v[1] -= rhs._v[1];
            _v[2] += rhs._v[2];
            _v[3] += rhs._v[3]; 
            return this;
        }
        public Vec4us Mul(ushort rhs)
        {
            _v[0] *= rhs;
            _v[1] *= rhs;
            _v[2] *= rhs;
            _v[3] *= rhs;
            return this;
        }
        public Vec4us Div(ushort rhs)
        {
            _v[0] /= rhs;
            _v[1] /= rhs;
            _v[2] /= rhs;
            _v[3] /= rhs;
            return this;
        }
        /** multiply by vector components. */
        public static Vec4us ComponentMultiply(Vec4us lhs, Vec4us rhs)
        {
            return new Vec4us(lhs[0] * rhs[0], lhs[1] * rhs[1], lhs[2] * rhs[2], lhs[3] * rhs[3]);
        }

        /** divide rhs components by rhs vector components. */
        public static Vec4us ComponentDivide(Vec4us lhs, Vec4us rhs)
        {
            return new Vec4us(lhs[0] / rhs[0], lhs[1] / rhs[1], lhs[2] / rhs[2], lhs[3] / rhs[3]);
        }
        /** Length of the vector = sqrt( vec . vec ) */
        public int Length
        {
            get { return (ushort)Math.Sqrt(_v[0] * _v[0] + _v[1] * _v[1] + _v[2] * _v[2] + _v[3] * _v[3]); }
        }

        /** Length squared of the vector = vec . vec */
        public int Length2
        {
            get { return _v[0] * _v[0] + _v[1] * _v[1] + _v[2] * _v[2] + _v[3] * _v[3]; }
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

            // If parameter cannot be cast to Poushort return false.
            Vec4us p = obj as Vec4us;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (_v[0] == p._v[0]) && (_v[1] == p._v[1]) && (_v[2] == p._v[2]) && (_v[3] == p._v[3]);
        }

        public bool Equals(Vec4us p)
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
    }    // end of class Vec4us

}
