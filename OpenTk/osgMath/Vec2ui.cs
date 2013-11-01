using System;
using System.Globalization;

namespace OSGNet.Osg
{
    public class Vec2ui
    {
        /** Vec member variable. */
        uint[] _v = new uint[2];


        /** Constructor that sets all components of the vector to zero */
        public Vec2ui() { _v[0] = 0; _v[1] = 0; }
        public Vec2ui(uint x, uint y) { _v[0] = x; _v[1] = y; }


        public static bool operator ==(Vec2ui l, Vec2ui r) { return l._v[0] == r._v[0] && l._v[1] == r._v[1]; }

        public static bool operator !=(Vec2ui l, Vec2ui r) { return l._v[0] != r._v[0] || l._v[1] != r._v[1]; }

        public static bool operator <(Vec2ui l, Vec2ui r)
        {
            if (l._v[0] < r._v[0]) return true;
            else if (l._v[0] > r._v[0]) return false;
            else return (l._v[1] < r._v[1]);
        }
        public static bool operator >(Vec2ui l, Vec2ui r)
        {
            if (l._v[0] > r._v[0]) return true;
            else if (l._v[0] < r._v[0]) return false;
            else return (l._v[1] > r._v[1]);
        }

        public void Set(uint x, uint y) { _v[0] = x; _v[1] = y; }
        public uint this[uint i]
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

        public uint X { get { return _v[0]; } set { _v[0] = value; } }
        public uint Y { get { return _v[1]; } set { _v[1] = value; } }

        public uint R { get { return _v[0]; } set { _v[0] = value; } }
        public uint G { get { return _v[1]; } set { _v[1] = value; } }

        /** Returns true if all components have values that are not NaN. */
        public bool IsValid() { return !IsNaN(); }
        /** Returns true if at least one component has value NaN. */
        public bool IsNaN() { return false; }

        /** Dot product. */
        public static uint operator *(Vec2ui lhs, Vec2ui rhs)
        {
            return lhs._v[0] * rhs._v[0] + lhs._v[1] * rhs._v[1];
        }

        /** Multiply by scalar. */
        public static Vec2ui operator *(Vec2ui lhs, uint rhs)
        {
            return new Vec2ui(lhs._v[0] * rhs, lhs._v[1] * rhs);
        }


        /** Divide by scalar. */
        public static Vec2ui operator /(Vec2ui lhs, uint rhs)
        {
            return new Vec2ui(lhs._v[0] / rhs, lhs._v[1] / rhs);
        }


        /** Binary vector add. */
        public static Vec2ui operator +(Vec2ui lhs, Vec2ui rhs)
        {
            return new Vec2ui(lhs._v[0] + rhs._v[0], lhs._v[1] + rhs._v[1]);
        }


        /** Binary vector subtract. */
        public static Vec2ui operator -(Vec2ui lhs, Vec2ui rhs)
        {
            return new Vec2ui(lhs._v[0] - rhs._v[0], lhs._v[1] - rhs._v[1]);
        }

        /** Unary vector operations. Slightly more efficient because no temporary
         * uintermediate object.
       */
        public Vec2ui Add(Vec2ui rhs)
        {
            _v[0] += rhs._v[0];
            _v[1] += rhs._v[1];
            return this;
        }
        public Vec2ui Substract(Vec2ui rhs)
        {
            _v[0] -= rhs._v[0];
            _v[1] -= rhs._v[1];
            return this;
        }
        public Vec2ui Mul(uint rhs)
        {
            _v[0] *= rhs;
            _v[1] *= rhs;
            return this;
        }
        public Vec2ui Div(uint rhs)
        {
            _v[0] /= rhs;
            _v[1] /= rhs;
            return this;
        }
        /** multiply by vector components. */
        public static Vec2ui ComponentMultiply(Vec2ui lhs, Vec2ui rhs)
        {
            return new Vec2ui(lhs[0] * rhs[0], lhs[1] * rhs[1]);
        }

        /** divide rhs components by rhs vector components. */
        public static Vec2ui ComponentDivide(Vec2ui lhs, Vec2ui rhs)
        {
            return new Vec2ui(lhs[0] / rhs[0], lhs[1] / rhs[1]);
        }
        /** Length of the vector = sqrt( vec . vec ) */
        public uint Length
        {
            get { return (uint)Math.Sqrt(_v[0] * _v[0] + _v[1] * _v[1]); }
        }

        /** Length squared of the vector = vec . vec */
        public uint Length2
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

            // If parameter cannot be cast to Pouint return false.
            Vec2ui p = obj as Vec2ui;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (_v[0] == p._v[0]) && (_v[1] == p._v[1]);
        }

        public bool Equals(Vec2ui p)
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
    }    // end of class Vec2ui

}
