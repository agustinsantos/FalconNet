using System;
using System.Globalization;

namespace OSGNet.Osg
{
    public class Vec3ub
    {
        /** Vec member variable. */
        byte[] _v = new byte[3];


        /** Constructor that sets all components of the vector to zero */
        public Vec3ub() { _v[0] = 0; _v[1] = 0; _v[2] = 0; }
        public Vec3ub(byte x, byte y, byte z) { _v[0] = x; _v[1] = y; _v[2] = z; }
        public Vec3ub(int x, int y, int z) { _v[0] = (byte)x; _v[1] = (byte)y; _v[2] = (byte)y; }

        public static bool operator ==(Vec3ub l, Vec3ub r) { return l._v[0] == r._v[0] && l._v[1] == r._v[1] && l._v[2] == r._v[2]; }

        public static bool operator !=(Vec3ub l, Vec3ub r) { return l._v[0] != r._v[0] || l._v[1] != r._v[1] || l._v[2] != r._v[2]; }

        public static bool operator <(Vec3ub l, Vec3ub r)
        {
            if (l._v[0] < r._v[0]) return true;
            else if (l._v[0] > r._v[0]) return false;
            else if (l._v[1] < r._v[1]) return true;
            else if (l._v[1] > r._v[1]) return false;
            else return (l._v[2] < r._v[2]);
        }
        public static bool operator >(Vec3ub l, Vec3ub r)
        {
            if (l._v[0] > r._v[0]) return true;
            else if (l._v[0] < r._v[0]) return false;
            else if (l._v[1] > r._v[1]) return true;
            else if (l._v[1] < r._v[1]) return false;
            else return (l._v[2] > r._v[2]);
        }

        public void Set(byte x, byte y, byte z) { _v[0] = x; _v[1] = y; _v[2] = z; }
        public byte this[byte i]
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

        public byte X { get { return _v[0]; } set { _v[0] = value; } }
        public byte Y { get { return _v[1]; } set { _v[1] = value; } }
        public byte Z { get { return _v[2]; } set { _v[2] = value; } }

        public byte R { get { return _v[0]; } set { _v[0] = value; } }
        public byte G { get { return _v[1]; } set { _v[1] = value; } }
        public byte B { get { return _v[2]; } set { _v[2] = value; } }

        /** Returns true if all components have values that are not NaN. */
        public bool IsValid() { return !IsNaN(); }
        /** Returns true if at least one component has value NaN. */
        public bool IsNaN() { return false; }

        /** Dot product. */
        public static int operator *(Vec3ub lhs, Vec3ub rhs)
        {
            return lhs._v[0] * rhs._v[0] + lhs._v[1] * rhs._v[1] + lhs._v[2] * rhs._v[2];
        }

        /** Multiply by scalar. */
        public static Vec3ub operator *(Vec3ub lhs, byte rhs)
        {
            return new Vec3ub(lhs._v[0] * rhs, lhs._v[1] * rhs, lhs._v[2] * rhs);
        }


        /** Divide by scalar. */
        public static Vec3ub operator /(Vec3ub lhs, byte rhs)
        {
            return new Vec3ub(lhs._v[0] / rhs, lhs._v[1] / rhs, lhs._v[2] / rhs);
        }


        /** Binary vector add. */
        public static Vec3ub operator +(Vec3ub lhs, Vec3ub rhs)
        {
            return new Vec3ub(lhs._v[0] + rhs._v[0], lhs._v[1] + rhs._v[1], lhs._v[2] + rhs._v[2]);
        }


        /** Binary vector subtract. */
        public static Vec3ub operator -(Vec3ub lhs, Vec3ub rhs)
        {
            return new Vec3ub(lhs._v[0] - rhs._v[0], lhs._v[1] - rhs._v[1], lhs._v[2] - rhs._v[2]);
        }

        /** Unary vector operations. Slightly more efficient because no temporary
         * byteermediate object.
       */
        public Vec3ub Add(Vec3ub rhs)
        {
            _v[0] += rhs._v[0];
            _v[1] += rhs._v[1];
            _v[2] += rhs._v[2];
            return this;
        }
        public Vec3ub Substract(Vec3ub rhs)
        {
            _v[0] -= rhs._v[0];
            _v[1] -= rhs._v[1];
            _v[2] -= rhs._v[2];
            return this;
        }
        public Vec3ub Mul(byte rhs)
        {
            _v[0] *= rhs;
            _v[1] *= rhs;
            _v[2] *= rhs;
            return this;
        }
        public Vec3ub Div(byte rhs)
        {
            _v[0] /= rhs;
            _v[1] /= rhs;
            _v[2] /= rhs;
            return this;
        }
        /** multiply by vector components. */
        public static Vec3ub ComponentMultiply(Vec3ub lhs, Vec3ub rhs)
        {
            return new Vec3ub(lhs[0] * rhs[0], lhs[1] * rhs[1], lhs[2] * rhs[2]);
        }

        /** divide rhs components by rhs vector components. */
        public static Vec3ub ComponentDivide(Vec3ub lhs, Vec3ub rhs)
        {
            return new Vec3ub(lhs[0] / rhs[0], lhs[1] / rhs[1], lhs[2] / rhs[2]);
        }
        /** Length of the vector = sqrt( vec . vec ) */
        public int Length
        {
            get { return (byte)Math.Sqrt(_v[0] * _v[0] + _v[1] * _v[1] + _v[2] * _v[2]); }
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

            // If parameter cannot be cast to Pobyte return false.
            Vec3ub p = obj as Vec3ub;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (_v[0] == p._v[0]) && (_v[1] == p._v[1]) && (_v[2] == p._v[2]);
        }

        public bool Equals(Vec3ub p)
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
    }    // end of class Vec3ub

}
