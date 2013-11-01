using System;
using System.Globalization;

namespace OSGNet.Osg
{
    public class Vec2i
    {
        /** Vec member variable. */
        int[] _v = new int[2];


        /** Constructor that sets all components of the vector to zero */
        public Vec2i() { _v[0] = 0; _v[1] = 0; }
        public Vec2i(int x, int y) { _v[0] = x; _v[1] = y; }


        public static bool operator ==(Vec2i l, Vec2i r) { return l._v[0] == r._v[0] && l._v[1] == r._v[1]; }

        public static bool operator !=(Vec2i l, Vec2i r) { return l._v[0] != r._v[0] || l._v[1] != r._v[1]; }

        public static bool operator <(Vec2i l, Vec2i r)
        {
            if (l._v[0] < r._v[0]) return true;
            else if (l._v[0] > r._v[0]) return false;
            else return (l._v[1] < r._v[1]);
        }
        public static bool operator >(Vec2i l, Vec2i r)
        {
            if (l._v[0] > r._v[0]) return true;
            else if (l._v[0] < r._v[0]) return false;
            else return (l._v[1] > r._v[1]);
        }

        public void Set(int x, int y) { _v[0] = x; _v[1] = y; }
        public int this[int i]
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

        public int X { get { return _v[0]; } set { _v[0] = value; } }
        public int Y { get { return _v[1]; } set { _v[1] = value; } }

        public int R { get { return _v[0]; } set { _v[0] = value; } }
        public int G { get { return _v[1]; } set { _v[1] = value; } }

        /** Returns true if all components have values that are not NaN. */
        public bool IsValid() { return !IsNaN(); }
        /** Returns true if at least one component has value NaN. */
        public bool IsNaN() { return false; }

        /** Dot product. */
        public static int operator *(Vec2i lhs, Vec2i rhs)
        {
            return lhs._v[0] * rhs._v[0] + lhs._v[1] * rhs._v[1];
        }

        /** Multiply by scalar. */
        public static Vec2i operator *(Vec2i lhs, int rhs)
        {
            return new Vec2i(lhs._v[0] * rhs, lhs._v[1] * rhs);
        }


        /** Divide by scalar. */
        public static Vec2i operator /(Vec2i lhs, int rhs)
        {
            return new Vec2i(lhs._v[0] / rhs, lhs._v[1] / rhs);
        }


        /** Binary vector add. */
        public static Vec2i operator +(Vec2i lhs, Vec2i rhs)
        {
            return new Vec2i(lhs._v[0] + rhs._v[0], lhs._v[1] + rhs._v[1]);
        }


        /** Binary vector subtract. */
        public static Vec2i operator -(Vec2i lhs, Vec2i rhs)
        {
            return new Vec2i(lhs._v[0] - rhs._v[0], lhs._v[1] - rhs._v[1]);
        }

        /** Unary vector operations. Slightly more efficient because no temporary
         * intermediate object.
       */
        public Vec2i Add(Vec2i rhs)
        {
            _v[0] += rhs._v[0];
            _v[1] += rhs._v[1];
            return this;
        }
        public Vec2i Substract(Vec2i rhs)
        {
            _v[0] -= rhs._v[0];
            _v[1] -= rhs._v[1];
            return this;
        }
        public Vec2i Mul(int rhs)
        {
            _v[0] *= rhs;
            _v[1] *= rhs;
            return this;
        }
        public Vec2i Div(int rhs)
        {
            _v[0] /= rhs;
            _v[1] /= rhs;
            return this;
        }
        /** multiply by vector components. */
        public static Vec2i ComponentMultiply(Vec2i lhs, Vec2i rhs)
        {
            return new Vec2i(lhs[0] * rhs[0], lhs[1] * rhs[1]);
        }

        /** divide rhs components by rhs vector components. */
        public static Vec2i ComponentDivide(Vec2i lhs, Vec2i rhs)
        {
            return new Vec2i(lhs[0] / rhs[0], lhs[1] / rhs[1]);
        }
        /** Length of the vector = sqrt( vec . vec ) */
        public int Length
        {
            get { return (int)Math.Sqrt(_v[0] * _v[0] + _v[1] * _v[1]); }
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

            // If parameter cannot be cast to Point return false.
            Vec2i p = obj as Vec2i;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (_v[0] == p._v[0]) && (_v[1] == p._v[1]);
        }

        public bool Equals(Vec2i p)
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
    }    // end of class Vec2i

}
