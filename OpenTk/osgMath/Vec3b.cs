﻿using System;
using System.Globalization;

namespace OSGNet.Osg
{
    public class Vec3b
    {
        /** Vec member variable. */
        sbyte[] _v = new sbyte[3];


        /** Constructor that sets all components of the vector to zero */
        public Vec3b() { _v[0] = 0; _v[1] = 0; _v[2] = 0; }
        public Vec3b(sbyte x, sbyte y, sbyte z) { _v[0] = x; _v[1] = y; _v[2] = z; }
        public Vec3b(int x, int y, int z) { _v[0] = (sbyte)x; _v[1] = (sbyte)y; _v[2] = (sbyte)y; }

        public static bool operator ==(Vec3b l, Vec3b r) { return l._v[0] == r._v[0] && l._v[1] == r._v[1] && l._v[2] == r._v[2]; }

        public static bool operator !=(Vec3b l, Vec3b r) { return l._v[0] != r._v[0] || l._v[1] != r._v[1] || l._v[2] != r._v[2]; }

        public static bool operator <(Vec3b l, Vec3b r)
        {
            if (l._v[0] < r._v[0]) return true;
            else if (l._v[0] > r._v[0]) return false;
            else if (l._v[1] < r._v[1]) return true;
            else if (l._v[1] > r._v[1]) return false;
            else return (l._v[2] < r._v[2]);
        }
        public static bool operator >(Vec3b l, Vec3b r)
        {
            if (l._v[0] > r._v[0]) return true;
            else if (l._v[0] < r._v[0]) return false;
            else if (l._v[1] > r._v[1]) return true;
            else if (l._v[1] < r._v[1]) return false;
            else return (l._v[2] > r._v[2]);
        }

        public void Set(sbyte x, sbyte y, sbyte z) { _v[0] = x; _v[1] = y; _v[2] = z; }
        public sbyte this[sbyte i]
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

        public sbyte X { get { return _v[0]; } set { _v[0] = value; } }
        public sbyte Y { get { return _v[1]; } set { _v[1] = value; } }
        public sbyte Z { get { return _v[2]; } set { _v[2] = value; } }

        public sbyte R { get { return _v[0]; } set { _v[0] = value; } }
        public sbyte G { get { return _v[1]; } set { _v[1] = value; } }
        public sbyte B { get { return _v[2]; } set { _v[2] = value; } }

        /** Returns true if all components have values that are not NaN. */
        public bool IsValid() { return !IsNaN(); }
        /** Returns true if at least one component has value NaN. */
        public bool IsNaN() { return false; }

        /** Dot product. */
        public static int operator *(Vec3b lhs, Vec3b rhs)
        {
            return lhs._v[0] * rhs._v[0] + lhs._v[1] * rhs._v[1] + lhs._v[2] * rhs._v[2];
        }

        /** Multiply by scalar. */
        public static Vec3b operator *(Vec3b lhs, sbyte rhs)
        {
            return new Vec3b(lhs._v[0] * rhs, lhs._v[1] * rhs, lhs._v[2] * rhs);
        }


        /** Divide by scalar. */
        public static Vec3b operator /(Vec3b lhs, sbyte rhs)
        {
            return new Vec3b(lhs._v[0] / rhs, lhs._v[1] / rhs, lhs._v[2] / rhs);
        }


        /** Binary vector add. */
        public static Vec3b operator +(Vec3b lhs, Vec3b rhs)
        {
            return new Vec3b(lhs._v[0] + rhs._v[0], lhs._v[1] + rhs._v[1], lhs._v[2] + rhs._v[2]);
        }


        /** Binary vector subtract. */
        public static Vec3b operator -(Vec3b lhs, Vec3b rhs)
        {
            return new Vec3b(lhs._v[0] - rhs._v[0], lhs._v[1] - rhs._v[1], lhs._v[2] - rhs._v[2]);
        }

        /** Unary vector operations. Slightly more efficient because no temporary
         * sbyteermediate object.
       */
        public Vec3b Add(Vec3b rhs)
        {
            _v[0] += rhs._v[0];
            _v[1] += rhs._v[1];
            _v[2] += rhs._v[2];
            return this;
        }
        public Vec3b Substract(Vec3b rhs)
        {
            _v[0] -= rhs._v[0];
            _v[1] -= rhs._v[1];
            _v[2] -= rhs._v[2];
            return this;
        }
        public Vec3b Mul(sbyte rhs)
        {
            _v[0] *= rhs;
            _v[1] *= rhs;
            _v[2] *= rhs;
            return this;
        }
        public Vec3b Div(sbyte rhs)
        {
            _v[0] /= rhs;
            _v[1] /= rhs;
            _v[2] /= rhs;
            return this;
        }
        /** multiply by vector components. */
        public static Vec3b ComponentMultiply(Vec3b lhs, Vec3b rhs)
        {
            return new Vec3b(lhs[0] * rhs[0], lhs[1] * rhs[1], lhs[2] * rhs[2]);
        }

        /** divide rhs components by rhs vector components. */
        public static Vec3b ComponentDivide(Vec3b lhs, Vec3b rhs)
        {
            return new Vec3b(lhs[0] / rhs[0], lhs[1] / rhs[1], lhs[2] / rhs[2]);
        }
        /** Length of the vector = sqrt( vec . vec ) */
        public int Length
        {
            get { return (sbyte)Math.Sqrt(_v[0] * _v[0] + _v[1] * _v[1] + _v[2] * _v[2]); }
        }

        /** Length squared of the vector = vec . vec */
        public int Length2
        {
            get { return _v[0] * _v[0] + _v[1] * _v[1] + _v[2] * _v[2]; }
        }


        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "({0}, {1}, {2})", _v[0], _v[1], _v[2]);
        }
        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Posbyte return false.
            Vec3b p = obj as Vec3b;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (_v[0] == p._v[0]) && (_v[1] == p._v[1]) && (_v[2] == p._v[2]);
        }

        public bool Equals(Vec3b p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (_v[0] == p._v[0]) && (_v[1] == p._v[1]) && (_v[2] == p._v[2]);
        }

        public override int GetHashCode()
        {
            return _v[0].GetHashCode() ^ _v[1].GetHashCode() ^ _v[2].GetHashCode();
        }
    }    // end of class Vec3b

}
