using System;
using System.Globalization;

namespace OSGNet.Osg
{
    public class Vec3ui
    {
        /** Vec member variable. */
        uint[] _v = new uint[3];


        /** Constructor that sets all components of the vector to zero */
        public Vec3ui() { _v[0] = 0; _v[1] = 0; _v[2] = 0; }
        public Vec3ui(uint x, uint y, uint z) { _v[0] = x; _v[1] = y; _v[2] = z; }
        public Vec3ui(int x, int y, int z) { _v[0] = (uint)x; _v[1] = (uint)y; _v[2] = (uint)y; }

        public static bool operator ==(Vec3ui l, Vec3ui r) { return l._v[0] == r._v[0] && l._v[1] == r._v[1] && l._v[2] == r._v[2]; }

        public static bool operator !=(Vec3ui l, Vec3ui r) { return l._v[0] != r._v[0] || l._v[1] != r._v[1] || l._v[2] != r._v[2]; }

        public static bool operator <(Vec3ui l, Vec3ui r)
        {
            if (l._v[0] < r._v[0]) return true;
            else if (l._v[0] > r._v[0]) return false;
            else if (l._v[1] < r._v[1]) return true;
            else if (l._v[1] > r._v[1]) return false;
            else return (l._v[2] < r._v[2]);
        }
        public static bool operator >(Vec3ui l, Vec3ui r)
        {
            if (l._v[0] > r._v[0]) return true;
            else if (l._v[0] < r._v[0]) return false;
            else if (l._v[1] > r._v[1]) return true;
            else if (l._v[1] < r._v[1]) return false;
            else return (l._v[2] > r._v[2]);
        }

        public void Set(uint x, uint y, uint z) { _v[0] = x; _v[1] = y; _v[2] = z; }
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
        public uint Z { get { return _v[2]; } set { _v[2] = value; } }

        public uint R { get { return _v[0]; } set { _v[0] = value; } }
        public uint G { get { return _v[1]; } set { _v[1] = value; } }
        public uint B { get { return _v[2]; } set { _v[2] = value; } }

        /** Returns true if all components have values that are not NaN. */
        public bool IsValid() { return !IsNaN(); }
        /** Returns true if at least one component has value NaN. */
        public bool IsNaN() { return false; }

        /** Dot product. */
        public static uint operator *(Vec3ui lhs, Vec3ui rhs)
        {
            return lhs._v[0] * rhs._v[0] + lhs._v[1] * rhs._v[1] + lhs._v[2] * rhs._v[2];
        }

        /** Multiply by scalar. */
        public static Vec3ui operator *(Vec3ui lhs, uint rhs)
        {
            return new Vec3ui(lhs._v[0] * rhs, lhs._v[1] * rhs, lhs._v[2] * rhs);
        }


        /** Divide by scalar. */
        public static Vec3ui operator /(Vec3ui lhs, uint rhs)
        {
            return new Vec3ui(lhs._v[0] / rhs, lhs._v[1] / rhs, lhs._v[2] / rhs);
        }


        /** Binary vector add. */
        public static Vec3ui operator +(Vec3ui lhs, Vec3ui rhs)
        {
            return new Vec3ui(lhs._v[0] + rhs._v[0], lhs._v[1] + rhs._v[1], lhs._v[2] + rhs._v[2]);
        }


        /** Binary vector subtract. */
        public static Vec3ui operator -(Vec3ui lhs, Vec3ui rhs)
        {
            return new Vec3ui(lhs._v[0] - rhs._v[0], lhs._v[1] - rhs._v[1], lhs._v[2] - rhs._v[2]);
        }

        /** Unary vector operations. Slightly more efficient because no temporary
         * uintermediate object.
       */
        public Vec3ui Add(Vec3ui rhs)
        {
            _v[0] += rhs._v[0];
            _v[1] += rhs._v[1];
            _v[2] += rhs._v[2];
            return this;
        }
        public Vec3ui Substract(Vec3ui rhs)
        {
            _v[0] -= rhs._v[0];
            _v[1] -= rhs._v[1];
            _v[2] -= rhs._v[2];
            return this;
        }
        public Vec3ui Mul(uint rhs)
        {
            _v[0] *= rhs;
            _v[1] *= rhs;
            _v[2] *= rhs;
            return this;
        }
        public Vec3ui Div(uint rhs)
        {
            _v[0] /= rhs;
            _v[1] /= rhs;
            _v[2] /= rhs;
            return this;
        }
        /** multiply by vector components. */
        public static Vec3ui ComponentMultiply(Vec3ui lhs, Vec3ui rhs)
        {
            return new Vec3ui(lhs[0] * rhs[0], lhs[1] * rhs[1], lhs[2] * rhs[2]);
        }

        /** divide rhs components by rhs vector components. */
        public static Vec3ui ComponentDivide(Vec3ui lhs, Vec3ui rhs)
        {
            return new Vec3ui(lhs[0] / rhs[0], lhs[1] / rhs[1], lhs[2] / rhs[2]);
        }
        /** Length of the vector = sqrt( vec . vec ) */
        public uint Length
        {
            get { return (uint)Math.Sqrt(_v[0] * _v[0] + _v[1] * _v[1] + _v[2] * _v[2]); }
        }

        /** Length squared of the vector = vec . vec */
        public uint Length2
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

            // If parameter cannot be cast to Pouint return false.
            Vec3ui p = obj as Vec3ui;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (_v[0] == p._v[0]) && (_v[1] == p._v[1]) && (_v[2] == p._v[2]);
        }

        public bool Equals(Vec3ui p)
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
    }    // end of class Vec3ui

}
