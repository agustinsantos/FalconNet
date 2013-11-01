using System;
using System.Globalization;

namespace OSGNet.Osg
{
    public class Vec2f
    {
        /** Vec member variable. */
        float[] _v = new float[2];


        /** Constructor that sets all components of the vector to zero */
        public Vec2f() { _v[0] = 0.0f; _v[1] = 0.0f; }
        public Vec2f(float x, float y) { _v[0] = x; _v[1] = y; }


        public static bool operator ==(Vec2f l, Vec2f r) { return l._v[0] == r._v[0] && l._v[1] == r._v[1]; }

        public static bool operator !=(Vec2f l, Vec2f r) { return l._v[0] != r._v[0] || l._v[1] != r._v[1]; }

        public static bool operator <(Vec2f l, Vec2f r)
        {
            if (l._v[0] < r._v[0]) return true;
            else if (l._v[0] > r._v[0]) return false;
            else return (l._v[1] < r._v[1]);
        }
        public static bool operator >(Vec2f l, Vec2f r)
        {
            if (l._v[0] > r._v[0]) return true;
            else if (l._v[0] < r._v[0]) return false;
            else return (l._v[1] > r._v[1]);
        }

        public void Set(float x, float y) { _v[0] = x; _v[1] = y; }
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

        /** Returns true if all components have values that are not NaN. */
        public bool IsValid() { return !IsNaN(); }
        /** Returns true if at least one component has value NaN. */
        public bool IsNaN() { return float.IsNaN(_v[0]) || float.IsNaN(_v[1]); }

        /** Dot product. */
        public static float operator *(Vec2f lhs, Vec2f rhs)
        {
            return lhs._v[0] * rhs._v[0] + lhs._v[1] * rhs._v[1];
        }

        /** Multiply by scalar. */
        public static Vec2f operator *(Vec2f lhs, float rhs)
        {
            return new Vec2f(lhs._v[0] * rhs, lhs._v[1] * rhs);
        }


        /** Divide by scalar. */
        public static Vec2f operator /(Vec2f lhs, float rhs)
        {
            return new Vec2f(lhs._v[0] / rhs, lhs._v[1] / rhs);
        }


        /** Binary vector add. */
        public static Vec2f operator +(Vec2f lhs, Vec2f rhs)
        {
            return new Vec2f(lhs._v[0] + rhs._v[0], lhs._v[1] + rhs._v[1]);
        }


        /** Binary vector subtract. */
        public static Vec2f operator -(Vec2f lhs, Vec2f rhs)
        {
            return new Vec2f(lhs._v[0] - rhs._v[0], lhs._v[1] - rhs._v[1]);
        }

        /** Unary vector operations. Slightly more efficient because no temporary
         * intermediate object.
       */
        public Vec2f Add(Vec2f rhs)
        {
            _v[0] += rhs._v[0];
            _v[1] += rhs._v[1];
            return this;
        }
        public Vec2f Substract(Vec2f rhs)
        {
            _v[0] -= rhs._v[0];
            _v[1] -= rhs._v[1];
            return this;
        }
        public Vec2f Mul(float rhs)
        {
            _v[0] *= rhs;
            _v[1] *= rhs;
            return this;
        }
        public Vec2f Div(float rhs)
        {
            _v[0] /= rhs;
            _v[1] /= rhs;
            return this;
        }
        /** multiply by vector components. */
        public static Vec2f ComponentMultiply(Vec2f lhs, Vec2f rhs)
        {
            return new Vec2f(lhs[0] * rhs[0], lhs[1] * rhs[1]);
        }

        /** divide rhs components by rhs vector components. */
        public static Vec2f ComponentDivide(Vec2f lhs, Vec2f rhs)
        {
            return new Vec2f(lhs[0] / rhs[0], lhs[1] / rhs[1]);
        }
        /** Length of the vector = sqrt( vec . vec ) */
        public float Length
        {
            get { return (float)Math.Sqrt(_v[0] * _v[0] + _v[1] * _v[1]); }
        }

        /** Length squared of the vector = vec . vec */
        public float Length2
        {
            get { return _v[0] * _v[0] + _v[1] * _v[1]; }
        }

        /** Normalize the vector so that it has length unity.
          * Returns the previous length of the vector.
        */
        public float Normalize()
        {
            float norm = this.Length;
            if (norm > 0.0)
            {
                float inv = 1.0f / norm;
                _v[0] *= inv;
                _v[1] *= inv;
            }
            return (norm);
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
            Vec2f p = obj as Vec2f;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (_v[0] == p._v[0]) && (_v[1] == p._v[1]);
        }

        public bool Equals(Vec2f p)
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
    }    // end of class Vec2f

}
