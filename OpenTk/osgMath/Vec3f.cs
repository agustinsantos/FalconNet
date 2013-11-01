using System;
using System.Globalization;

namespace OSGNet.Osg
{
    public class Vec3f
    {
        /** Vec member variable. */
        float[] _v = new float[3];


        /** Constructor that sets all components of the vector to zero */
        public Vec3f() { _v[0] = 0; _v[1] = 0; _v[2] = 0; }
        public Vec3f(float x, float y, float z) { _v[0] = x; _v[1] = y; _v[2] = z; }

        public static bool operator ==(Vec3f l, Vec3f r) { return l._v[0] == r._v[0] && l._v[1] == r._v[1] && l._v[2] == r._v[2]; }

        public static bool operator !=(Vec3f l, Vec3f r) { return l._v[0] != r._v[0] || l._v[1] != r._v[1] || l._v[2] != r._v[2]; }

        public static bool operator <(Vec3f l, Vec3f r)
        {
            if (l._v[0] < r._v[0]) return true;
            else if (l._v[0] > r._v[0]) return false;
            else if (l._v[1] < r._v[1]) return true;
            else if (l._v[1] > r._v[1]) return false;
            else return (l._v[2] < r._v[2]);
        }
        public static bool operator >(Vec3f l, Vec3f r)
        {
            if (l._v[0] > r._v[0]) return true;
            else if (l._v[0] < r._v[0]) return false;
            else if (l._v[1] > r._v[1]) return true;
            else if (l._v[1] < r._v[1]) return false;
            else return (l._v[2] > r._v[2]);
        }

        public void Set(float x, float y, float z) { _v[0] = x; _v[1] = y; _v[2] = z; }
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

        public float R { get { return _v[0]; } set { _v[0] = value; } }
        public float G { get { return _v[1]; } set { _v[1] = value; } }
        public float B { get { return _v[2]; } set { _v[2] = value; } }

        /** Returns true if all components have values that are not NaN. */
        public bool IsValid() { return !IsNaN(); }
        /** Returns true if at least one component has value NaN. */
        public bool IsNaN() { return float.IsNaN(_v[0]) || float.IsNaN(_v[1]) || float.IsNaN(_v[2]); }

        /** Dot product. */
        public static float operator *(Vec3f lhs, Vec3f rhs)
        {
            return lhs._v[0] * rhs._v[0] + lhs._v[1] * rhs._v[1] + lhs._v[2] * rhs._v[2];
        }

        /// <summary>
        /// Cross product
        /// </summary>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public Vec3f Cross(Vec3f rhs)
        {
            return new Vec3f(_v[1] * rhs._v[2] - _v[2] * rhs._v[1],
                         _v[2] * rhs._v[0] - _v[0] * rhs._v[2],
                         _v[0] * rhs._v[1] - _v[1] * rhs._v[0]);
        }

        /** Multiply by scalar. */
        public static Vec3f operator *(Vec3f lhs, double rhs)
        {
            return new Vec3f((float)(lhs._v[0] * rhs), (float)(lhs._v[1] * rhs), (float)(lhs._v[2] * rhs));
        }
        public static Vec3f operator *(Vec3f lhs, float rhs)
        {
            return new Vec3f(lhs._v[0] * rhs, lhs._v[1] * rhs, lhs._v[2] * rhs);
        }

        /** Divide by scalar. */
        public static Vec3f operator /(Vec3f lhs, float rhs)
        {
            return new Vec3f(lhs._v[0] / rhs, lhs._v[1] / rhs, lhs._v[2] / rhs);
        }


        /** Binary vector add. */
        public static Vec3f operator +(Vec3f lhs, Vec3f rhs)
        {
            return new Vec3f(lhs._v[0] + rhs._v[0], lhs._v[1] + rhs._v[1], lhs._v[2] + rhs._v[2]);
        }


        /** Binary vector subtract. */
        public static Vec3f operator -(Vec3f lhs, Vec3f rhs)
        {
            return new Vec3f(lhs._v[0] - rhs._v[0], lhs._v[1] - rhs._v[1], lhs._v[2] - rhs._v[2]);
        }

        /** Unary vector operations. Slightly more efficient because no temporary
         * floatermediate object.
       */
        public Vec3f Add(Vec3f rhs)
        {
            _v[0] += rhs._v[0];
            _v[1] += rhs._v[1];
            _v[2] += rhs._v[2];
            return this;
        }
        public Vec3f Substract(Vec3f rhs)
        {
            _v[0] -= rhs._v[0];
            _v[1] -= rhs._v[1];
            _v[2] -= rhs._v[2];
            return this;
        }
        public Vec3f Mul(float rhs)
        {
            _v[0] *= rhs;
            _v[1] *= rhs;
            _v[2] *= rhs;
            return this;
        }
        public Vec3f Div(float rhs)
        {
            _v[0] /= rhs;
            _v[1] /= rhs;
            _v[2] /= rhs;
            return this;
        }
        /** multiply by vector components. */
        public static Vec3f ComponentMultiply(Vec3f lhs, Vec3f rhs)
        {
            return new Vec3f(lhs[0] * rhs[0], lhs[1] * rhs[1], lhs[2] * rhs[2]);
        }

        /** divide rhs components by rhs vector components. */
        public static Vec3f ComponentDivide(Vec3f lhs, Vec3f rhs)
        {
            return new Vec3f(lhs[0] / rhs[0], lhs[1] / rhs[1], lhs[2] / rhs[2]);
        }
        /** Length of the vector = sqrt( vec . vec ) */
        public float Length
        {
            get { return (float)Math.Sqrt(_v[0] * _v[0] + _v[1] * _v[1] + _v[2] * _v[2]); }
        }

        /** Length squared of the vector = vec . vec */
        public float Length2
        {
            get { return _v[0] * _v[0] + _v[1] * _v[1] + _v[2] * _v[2]; }
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
            }
            return( norm );
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

            // If parameter cannot be cast to Pofloat return false.
            Vec3f p = obj as Vec3f;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (_v[0] == p._v[0]) && (_v[1] == p._v[1]) && (_v[2] == p._v[2]);
        }

        public bool Equals(Vec3f p)
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
    }    // end of class Vec3f

}
