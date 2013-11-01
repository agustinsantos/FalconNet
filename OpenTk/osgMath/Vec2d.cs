using System;
using System.Globalization;

namespace OSGNet.Osg
{
    public class Vec2d
    {
        /** Vec member variable. */
        double[] _v = new double[2];


        /** Constructor that sets all components of the vector to zero */
        public Vec2d() { _v[0] = 0.0; _v[1] = 0.0; }
        public Vec2d(double x, double y) { _v[0] = x; _v[1] = y; }


        public static bool operator ==(Vec2d l, Vec2d r) { return l._v[0] == r._v[0] && l._v[1] == r._v[1]; }

        public static bool operator !=(Vec2d l, Vec2d r) { return l._v[0] != r._v[0] || l._v[1] != r._v[1]; }

        public static bool operator <(Vec2d l, Vec2d r)
        {
            if (l._v[0] < r._v[0]) return true;
            else if (l._v[0] > r._v[0]) return false;
            else return (l._v[1] < r._v[1]);
        }
        public static bool operator >(Vec2d l, Vec2d r)
        {
            if (l._v[0] > r._v[0]) return true;
            else if (l._v[0] < r._v[0]) return false;
            else return (l._v[1] > r._v[1]);
        }

        public void Set(double x, double y) { _v[0] = x; _v[1] = y; }
        public double this[int i]
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

        public double X { get { return _v[0]; } set { _v[0] = value; } }
        public double Y { get { return _v[1]; } set { _v[1] = value; } }

        /** Returns true if all components have values that are not NaN. */
        public bool IsValid() { return !IsNaN(); }
        /** Returns true if at least one component has value NaN. */
        public bool IsNaN() { return double.IsNaN(_v[0]) || double.IsNaN(_v[1]); }

        /** Dot product. */
        public static double operator *(Vec2d lhs, Vec2d rhs)
        {
            return lhs._v[0] * rhs._v[0] + lhs._v[1] * rhs._v[1];
        }

        /** Multiply by scalar. */
        public static Vec2d operator *(Vec2d lhs, double rhs)
        {
            return new Vec2d(lhs._v[0] * rhs, lhs._v[1] * rhs);
        }


        /** Divide by scalar. */
        public static Vec2d operator /(Vec2d lhs, double rhs)
        {
            return new Vec2d(lhs._v[0] / rhs, lhs._v[1] / rhs);
        }


        /** Binary vector add. */
        public static Vec2d operator +(Vec2d lhs, Vec2d rhs)
        {
            return new Vec2d(lhs._v[0] + rhs._v[0], lhs._v[1] + rhs._v[1]);
        }


        /** Binary vector subtract. */
        public static Vec2d operator -(Vec2d lhs, Vec2d rhs)
        {
            return new Vec2d(lhs._v[0] - rhs._v[0], lhs._v[1] - rhs._v[1]);
        }

        /** Unary vector operations. Slightly more efficient because no temporary
         * intermediate object.
       */
        public Vec2d Add(Vec2d rhs)
        {
            _v[0] += rhs._v[0];
            _v[1] += rhs._v[1];
            return this;
        }
        public Vec2d Substract(Vec2d rhs)
        {
            _v[0] -= rhs._v[0];
            _v[1] -= rhs._v[1];
            return this;
        }
        public Vec2d Mul(double rhs)
        {
            _v[0] *= rhs;
            _v[1] *= rhs;
            return this;
        }
        public Vec2d Div(double rhs)
        {
            _v[0] /= rhs;
            _v[1] /= rhs;
            return this;
        }
        /** multiply by vector components. */
        public static Vec2d ComponentMultiply(Vec2d lhs, Vec2d rhs)
        {
            return new Vec2d(lhs[0] * rhs[0], lhs[1] * rhs[1]);
        }

        /** divide rhs components by rhs vector components. */
        public static Vec2d ComponentDivide(Vec2d lhs, Vec2d rhs)
        {
            return new Vec2d(lhs[0] / rhs[0], lhs[1] / rhs[1]);
        }
        /** Length of the vector = sqrt( vec . vec ) */
        public double Length
        {
            get { return (double)Math.Sqrt(_v[0] * _v[0] + _v[1] * _v[1]); }
        }

        /** Length squared of the vector = vec . vec */
        public double Length2
        {
            get { return _v[0] * _v[0] + _v[1] * _v[1]; }
        }

        /** Normalize the vector so that it has length unity.
          * Returns the previous length of the vector.
        */
        public double Normalize()
        {
            double norm = this.Length;
            if (norm > 0.0)
            {
                double inv = 1.0f / norm;
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
            Vec2d p = obj as Vec2d;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (_v[0] == p._v[0]) && (_v[1] == p._v[1]);
        }

        public bool Equals(Vec2d p)
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
    }    // end of class Vec2d

}
