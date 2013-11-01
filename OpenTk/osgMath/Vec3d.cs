using System;
using System.Globalization;

namespace OSGNet.Osg
{
    public class Vec3d
    {
        /** Vec member variable. */
        double[] _v = new double[3];


        /** Constructor that sets all components of the vector to zero */
        public Vec3d() { _v[0] = 0; _v[1] = 0; _v[2] = 0; }
        public Vec3d(double x, double y, double z) { _v[0] = x; _v[1] = y; _v[2] = z; }
        public Vec3d(Vec3f v) { _v[0] = v.X; _v[1] = v.Y; _v[2] = v.Z; }

        public static bool operator ==(Vec3d l, Vec3d r) { return l._v[0] == r._v[0] && l._v[1] == r._v[1] && l._v[2] == r._v[2]; }

        public static bool operator !=(Vec3d l, Vec3d r) { return l._v[0] != r._v[0] || l._v[1] != r._v[1] || l._v[2] != r._v[2]; }

        public static bool operator <(Vec3d l, Vec3d r)
        {
            if (l._v[0] < r._v[0]) return true;
            else if (l._v[0] > r._v[0]) return false;
            else if (l._v[1] < r._v[1]) return true;
            else if (l._v[1] > r._v[1]) return false;
            else return (l._v[2] < r._v[2]);
        }
        public static bool operator >(Vec3d l, Vec3d r)
        {
            if (l._v[0] > r._v[0]) return true;
            else if (l._v[0] < r._v[0]) return false;
            else if (l._v[1] > r._v[1]) return true;
            else if (l._v[1] < r._v[1]) return false;
            else return (l._v[2] > r._v[2]);
        }

        public void Set(double x, double y, double z) { _v[0] = x; _v[1] = y; _v[2] = z; }
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
        public double Z { get { return _v[2]; } set { _v[2] = value; } }

        public double R { get { return _v[0]; } set { _v[0] = value; } }
        public double G { get { return _v[1]; } set { _v[1] = value; } }
        public double B { get { return _v[2]; } set { _v[2] = value; } }

        /** Returns true if all components have values that are not NaN. */
        public bool IsValid() { return !IsNaN(); }
        /** Returns true if at least one component has value NaN. */
        public bool IsNaN() { return double.IsNaN(_v[0]) || double.IsNaN(_v[1]) || double.IsNaN(_v[2]); }

        /** Dot product. */
        public static double operator *(Vec3d lhs, Vec3d rhs)
        {
            return lhs._v[0] * rhs._v[0] + lhs._v[1] * rhs._v[1] + lhs._v[2] * rhs._v[2];
        }

        /// <summary>
        /// Cross product
        /// </summary>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public Vec3d Cross(Vec3d rhs)
        {
            return new Vec3d(_v[1] * rhs._v[2] - _v[2] * rhs._v[1],
                         _v[2] * rhs._v[0] - _v[0] * rhs._v[2],
                         _v[0] * rhs._v[1] - _v[1] * rhs._v[0]);
        }

        /// <summary>
        /// Negation
        /// </summary>
        /// <param name="lhs"></param>
        /// <returns></returns>
        public static Vec3d operator -(Vec3d lhs)
        {
            return new Vec3d(- lhs._v[0], - lhs._v[1] , lhs._v[2] );
        }

        /** Multiply by scalar. */
        public static Vec3d operator *(Vec3d lhs, double rhs)
        {
            return new Vec3d(lhs._v[0] * rhs, lhs._v[1] * rhs, lhs._v[2] * rhs);
        }


        /** Divide by scalar. */
        public static Vec3d operator /(Vec3d lhs, double rhs)
        {
            return new Vec3d(lhs._v[0] / rhs, lhs._v[1] / rhs, lhs._v[2] / rhs);
        }


        /** Binary vector add. */
        public static Vec3d operator +(Vec3d lhs, Vec3d rhs)
        {
            return new Vec3d(lhs._v[0] + rhs._v[0], lhs._v[1] + rhs._v[1], lhs._v[2] + rhs._v[2]);
        }


        /** Binary vector subtract. */
        public static Vec3d operator -(Vec3d lhs, Vec3d rhs)
        {
            return new Vec3d(lhs._v[0] - rhs._v[0], lhs._v[1] - rhs._v[1], lhs._v[2] - rhs._v[2]);
        }

        /** Unary vector operations. Slightly more efficient because no temporary
         * doubleermediate object.
       */
        public Vec3d Add(Vec3d rhs)
        {
            _v[0] += rhs._v[0];
            _v[1] += rhs._v[1];
            _v[2] += rhs._v[2];
            return this;
        }
        public Vec3d Substract(Vec3d rhs)
        {
            _v[0] -= rhs._v[0];
            _v[1] -= rhs._v[1];
            _v[2] -= rhs._v[2];
            return this;
        }
        public Vec3d Mul(double rhs)
        {
            _v[0] *= rhs;
            _v[1] *= rhs;
            _v[2] *= rhs;
            return this;
        }
        public Vec3d Div(double rhs)
        {
            _v[0] /= rhs;
            _v[1] /= rhs;
            _v[2] /= rhs;
            return this;
        }
        /** multiply by vector components. */
        public static Vec3d ComponentMultiply(Vec3d lhs, Vec3d rhs)
        {
            return new Vec3d(lhs[0] * rhs[0], lhs[1] * rhs[1], lhs[2] * rhs[2]);
        }

        /** divide rhs components by rhs vector components. */
        public static Vec3d ComponentDivide(Vec3d lhs, Vec3d rhs)
        {
            return new Vec3d(lhs[0] / rhs[0], lhs[1] / rhs[1], lhs[2] / rhs[2]);
        }
        /** Length of the vector = sqrt( vec . vec ) */
        public double Length
        {
            get { return (double)Math.Sqrt(_v[0] * _v[0] + _v[1] * _v[1] + _v[2] * _v[2]); }
        }

        /** Length squared of the vector = vec . vec */
        public double Length2
        {
            get { return _v[0] * _v[0] + _v[1] * _v[1] + _v[2] * _v[2]; }
        }

        public double Normalize()
        {
            double norm = this.Length;
            if (norm > 0.0)
            {
                double inv = 1.0f / norm;
                _v[0] *= inv;
                _v[1] *= inv;
                _v[2] *= inv;
            }
            return (norm);
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

            // If parameter cannot be cast to Podouble return false.
            Vec3d p = obj as Vec3d;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (_v[0] == p._v[0]) && (_v[1] == p._v[1]) && (_v[2] == p._v[2]);
        }

        public bool Equals(Vec3d p)
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
    }    // end of class Vec3d

}
