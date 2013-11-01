using System;
using System.Globalization;

namespace OSGNet.Osg
{
    public class Vec2ub
    {
        /** Vec member variable. */
        byte[] _v = new byte[2];


        /** Constructor that sets all components of the vector to zero */
        public Vec2ub() { _v[0] = 0; _v[1] = 0; }
        public Vec2ub(byte x, byte y) { _v[0] = x; _v[1] = y; }
        public Vec2ub(int x, int y) { _v[0] = (byte)x; _v[1] = (byte)y; }

        public static bool operator ==(Vec2ub l, Vec2ub r) { return l._v[0] == r._v[0] && l._v[1] == r._v[1]; }

        public static bool operator !=(Vec2ub l, Vec2ub r) { return l._v[0] != r._v[0] || l._v[1] != r._v[1]; }

        public static bool operator <(Vec2ub l, Vec2ub r)
        {
            if (l._v[0] < r._v[0]) return true;
            else if (l._v[0] > r._v[0]) return false;
            else return (l._v[1] < r._v[1]);
        }
        public static bool operator >(Vec2ub l, Vec2ub r)
        {
            if (l._v[0] > r._v[0]) return true;
            else if (l._v[0] < r._v[0]) return false;
            else return (l._v[1] > r._v[1]);
        }

        public void Set(byte x, byte y) { _v[0] = x; _v[1] = y; }
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

        public byte R { get { return _v[0]; } set { _v[0] = value; } }
        public byte G { get { return _v[1]; } set { _v[1] = value; } }

        /** Returns true if all components have values that are not NaN. */
        public bool IsValid() { return !IsNaN(); }
        /** Returns true if at least one component has value NaN. */
        public bool IsNaN() { return false; }

        /** Dot product. */
        public static int operator *(Vec2ub lhs, Vec2ub rhs)
        {
            return lhs._v[0] * rhs._v[0] + lhs._v[1] * rhs._v[1];
        }

        /** Multiply by scalar. */
        public static Vec2ub operator *(Vec2ub lhs, byte rhs)
        {
            return new Vec2ub(lhs._v[0] * rhs, lhs._v[1] * rhs);
        }


        /** Divide by scalar. */
        public static Vec2ub operator /(Vec2ub lhs, byte rhs)
        {
            return new Vec2ub(lhs._v[0] / rhs, lhs._v[1] / rhs);
        }


        /** Binary vector add. */
        public static Vec2ub operator +(Vec2ub lhs, Vec2ub rhs)
        {
            return new Vec2ub(lhs._v[0] + rhs._v[0], lhs._v[1] + rhs._v[1]);
        }


        /** Binary vector subtract. */
        public static Vec2ub operator -(Vec2ub lhs, Vec2ub rhs)
        {
            return new Vec2ub(lhs._v[0] - rhs._v[0], lhs._v[1] - rhs._v[1]);
        }

        /** Unary vector operations. Slightly more efficient because no temporary
         * byteermediate object.
       */
        public Vec2ub Add(Vec2ub rhs)
        {
            _v[0] += rhs._v[0];
            _v[1] += rhs._v[1];
            return this;
        }
        public Vec2ub Substract(Vec2ub rhs)
        {
            _v[0] -= rhs._v[0];
            _v[1] -= rhs._v[1];
            return this;
        }
        public Vec2ub Mul(byte rhs)
        {
            _v[0] *= rhs;
            _v[1] *= rhs;
            return this;
        }
        public Vec2ub Div(byte rhs)
        {
            _v[0] /= rhs;
            _v[1] /= rhs;
            return this;
        }
        /** multiply by vector components. */
        public static Vec2ub ComponentMultiply(Vec2ub lhs, Vec2ub rhs)
        {
            return new Vec2ub(lhs[0] * rhs[0], lhs[1] * rhs[1]);
        }

        /** divide rhs components by rhs vector components. */
        public static Vec2ub ComponentDivide(Vec2ub lhs, Vec2ub rhs)
        {
            return new Vec2ub(lhs[0] / rhs[0], lhs[1] / rhs[1]);
        }
        /** Length of the vector = sqrt( vec . vec ) */
        public int Length
        {
            get { return (byte)Math.Sqrt(_v[0] * _v[0] + _v[1] * _v[1]); }
        }

        /** Length squared of the vector = vec . vec */
        public int Length2
        {
            get { return _v[0] * _v[0] + _v[1] * _v[1]; }
        }


        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "({0}, {1})", _v[0], _v[1]);
        }
        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Pobyte return false.
            Vec2ub p = obj as Vec2ub;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (_v[0] == p._v[0]) && (_v[1] == p._v[1]);
        }

        public bool Equals(Vec2ub p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (_v[0] == _v[0]) && (_v[1] == p._v[1]);
        }

        public override int GetHashCode()
        {
            return _v[0].GetHashCode() ^ _v[1].GetHashCode();
        }
    }    // end of class Vec2ub

}
