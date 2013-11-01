using System;
using System.Globalization;

namespace OSGNet.Osg
{
    public class Vec3us
    {
        /** Vec member variable. */
        ushort[] _v = new ushort[3];


        /** Constructor that sets all components of the vector to zero */
        public Vec3us() { _v[0] = 0; _v[1] = 0; _v[2] = 0; }
        public Vec3us(ushort x, ushort y, ushort z) { _v[0] = x; _v[1] = y; _v[2] = z; }
        public Vec3us(int x, int y, int z) { _v[0] = (ushort)x; _v[1] = (ushort)y; _v[2] = (ushort)y; }

        public static bool operator ==(Vec3us l, Vec3us r) { return l._v[0] == r._v[0] && l._v[1] == r._v[1] && l._v[2] == r._v[2]; }

        public static bool operator !=(Vec3us l, Vec3us r) { return l._v[0] != r._v[0] || l._v[1] != r._v[1] || l._v[2] != r._v[2]; }

        public static bool operator <(Vec3us l, Vec3us r)
        {
            if (l._v[0] < r._v[0]) return true;
            else if (l._v[0] > r._v[0]) return false;
            else if (l._v[1] < r._v[1]) return true;
            else if (l._v[1] > r._v[1]) return false;
            else return (l._v[2] < r._v[2]);
        }
        public static bool operator >(Vec3us l, Vec3us r)
        {
            if (l._v[0] > r._v[0]) return true;
            else if (l._v[0] < r._v[0]) return false;
            else if (l._v[1] > r._v[1]) return true;
            else if (l._v[1] < r._v[1]) return false;
            else return (l._v[2] > r._v[2]);
        }

        public void Set(ushort x, ushort y, ushort z) { _v[0] = x; _v[1] = y; _v[2] = z; }
        public ushort this[ushort i]
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

        public ushort R { get { return _v[0]; } set { _v[0] = value; } }
        public ushort G { get { return _v[1]; } set { _v[1] = value; } }
        public ushort B { get { return _v[2]; } set { _v[2] = value; } }

        /** Returns true if all components have values that are not NaN. */
        public bool IsValid() { return !IsNaN(); }
        /** Returns true if at least one component has value NaN. */
        public bool IsNaN() { return false; }

        /** Dot product. */
        public static int operator *(Vec3us lhs, Vec3us rhs)
        {
            return lhs._v[0] * rhs._v[0] + lhs._v[1] * rhs._v[1] + lhs._v[2] * rhs._v[2];
        }

        /** Multiply by scalar. */
        public static Vec3us operator *(Vec3us lhs, ushort rhs)
        {
            return new Vec3us(lhs._v[0] * rhs, lhs._v[1] * rhs, lhs._v[2] * rhs);
        }


        /** Divide by scalar. */
        public static Vec3us operator /(Vec3us lhs, ushort rhs)
        {
            return new Vec3us(lhs._v[0] / rhs, lhs._v[1] / rhs, lhs._v[2] / rhs);
        }


        /** Binary vector add. */
        public static Vec3us operator +(Vec3us lhs, Vec3us rhs)
        {
            return new Vec3us(lhs._v[0] + rhs._v[0], lhs._v[1] + rhs._v[1], lhs._v[2] + rhs._v[2]);
        }


        /** Binary vector subtract. */
        public static Vec3us operator -(Vec3us lhs, Vec3us rhs)
        {
            return new Vec3us(lhs._v[0] - rhs._v[0], lhs._v[1] - rhs._v[1], lhs._v[2] - rhs._v[2]);
        }

        /** Unary vector operations. Slightly more efficient because no temporary
         * ushortermediate object.
       */
        public Vec3us Add(Vec3us rhs)
        {
            _v[0] += rhs._v[0];
            _v[1] += rhs._v[1];
            _v[2] += rhs._v[2];
            return this;
        }
        public Vec3us Substract(Vec3us rhs)
        {
            _v[0] -= rhs._v[0];
            _v[1] -= rhs._v[1];
            _v[2] -= rhs._v[2];
            return this;
        }
        public Vec3us Mul(ushort rhs)
        {
            _v[0] *= rhs;
            _v[1] *= rhs;
            _v[2] *= rhs;
            return this;
        }
        public Vec3us Div(ushort rhs)
        {
            _v[0] /= rhs;
            _v[1] /= rhs;
            _v[2] /= rhs;
            return this;
        }
        /** multiply by vector components. */
        public static Vec3us ComponentMultiply(Vec3us lhs, Vec3us rhs)
        {
            return new Vec3us(lhs[0] * rhs[0], lhs[1] * rhs[1], lhs[2] * rhs[2]);
        }

        /** divide rhs components by rhs vector components. */
        public static Vec3us ComponentDivide(Vec3us lhs, Vec3us rhs)
        {
            return new Vec3us(lhs[0] / rhs[0], lhs[1] / rhs[1], lhs[2] / rhs[2]);
        }
        /** Length of the vector = sqrt( vec . vec ) */
        public int Length
        {
            get { return (ushort)Math.Sqrt(_v[0] * _v[0] + _v[1] * _v[1] + _v[2] * _v[2]); }
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

            // If parameter cannot be cast to Poushort return false.
            Vec3us p = obj as Vec3us;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (_v[0] == p._v[0]) && (_v[1] == p._v[1]) && (_v[2] == p._v[2]);
        }

        public bool Equals(Vec3us p)
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
    }    // end of class Vec3us

}
