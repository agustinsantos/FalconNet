using System;
using System.Globalization;

namespace OSGNet.Osg
{
    public class Vec2b
    {
        /** Vec member variable. */
        sbyte[] _v = new sbyte[2];


        /** Constructor that sets all components of the vector to zero */
        public Vec2b() { _v[0] = 0; _v[1] = 0; }
        public Vec2b(sbyte x, sbyte y) { _v[0] = x; _v[1] = y; }
        public Vec2b(int x, int y) { _v[0] = (sbyte)x; _v[1] = (sbyte)y; }

        public static bool operator ==(Vec2b l, Vec2b r) { return l._v[0] == r._v[0] && l._v[1] == r._v[1]; }

        public static bool operator !=(Vec2b l, Vec2b r) { return l._v[0] != r._v[0] || l._v[1] != r._v[1]; }

        public static bool operator <(Vec2b l, Vec2b r)
        {
            if (l._v[0] < r._v[0]) return true;
            else if (l._v[0] > r._v[0]) return false;
            else return (l._v[1] < r._v[1]);
        }
        public static bool operator >(Vec2b l, Vec2b r)
        {
            if (l._v[0] > r._v[0]) return true;
            else if (l._v[0] < r._v[0]) return false;
            else return (l._v[1] > r._v[1]);
        }

        public void Set(sbyte x, sbyte y) { _v[0] = x; _v[1] = y; }
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

        public sbyte R { get { return _v[0]; } set { _v[0] = value; } }
        public sbyte G { get { return _v[1]; } set { _v[1] = value; } }

        /** Returns true if all components have values that are not NaN. */
        public bool IsValid() { return !IsNaN(); }
        /** Returns true if at least one component has value NaN. */
        public bool IsNaN() { return false; }

        /** Dot product. */
        public static int operator *(Vec2b lhs, Vec2b rhs)
        {
            return lhs._v[0] * rhs._v[0] + lhs._v[1] * rhs._v[1];
        }

        /** Multiply by scalar. */
        public static Vec2b operator *(Vec2b lhs, sbyte rhs)
        {
            return new Vec2b(lhs._v[0] * rhs, lhs._v[1] * rhs);
        }


        /** Divide by scalar. */
        public static Vec2b operator /(Vec2b lhs, sbyte rhs)
        {
            return new Vec2b(lhs._v[0] / rhs, lhs._v[1] / rhs);
        }


        /** Binary vector add. */
        public static Vec2b operator +(Vec2b lhs, Vec2b rhs)
        {
            return new Vec2b(lhs._v[0] + rhs._v[0], lhs._v[1] + rhs._v[1]);
        }


        /** Binary vector subtract. */
        public static Vec2b operator -(Vec2b lhs, Vec2b rhs)
        {
            return new Vec2b(lhs._v[0] - rhs._v[0], lhs._v[1] - rhs._v[1]);
        }

        /** Unary vector operations. Slightly more efficient because no temporary
         * sbyteermediate object.
       */
        public Vec2b Add(Vec2b rhs)
        {
            _v[0] += rhs._v[0];
            _v[1] += rhs._v[1];
            return this;
        }
        public Vec2b Substract(Vec2b rhs)
        {
            _v[0] -= rhs._v[0];
            _v[1] -= rhs._v[1];
            return this;
        }
        public Vec2b Mul(sbyte rhs)
        {
            _v[0] *= rhs;
            _v[1] *= rhs;
            return this;
        }
        public Vec2b Div(sbyte rhs)
        {
            _v[0] /= rhs;
            _v[1] /= rhs;
            return this;
        }
        /** multiply by vector components. */
        public static Vec2b ComponentMultiply(Vec2b lhs, Vec2b rhs)
        {
            return new Vec2b(lhs[0] * rhs[0], lhs[1] * rhs[1]);
        }

        /** divide rhs components by rhs vector components. */
        public static Vec2b ComponentDivide(Vec2b lhs, Vec2b rhs)
        {
            return new Vec2b(lhs[0] / rhs[0], lhs[1] / rhs[1]);
        }
        /** Length of the vector = sqrt( vec . vec ) */
        public int Length
        {
            get { return (sbyte)Math.Sqrt(_v[0] * _v[0] + _v[1] * _v[1]); }
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

            // If parameter cannot be cast to Posbyte return false.
            Vec2b p = obj as Vec2b;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (_v[0] == p._v[0]) && (_v[1] == p._v[1]);
        }

        public bool Equals(Vec2b p)
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
    }    // end of class Vec2b

}
