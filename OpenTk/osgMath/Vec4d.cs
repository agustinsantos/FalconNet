using System;
using System.Globalization;

namespace OSGNet.Osg
{
    public class Vec4d
    {
        /** Vec member variable. */
        double[] _v = new double[4];


        /** Constructor that sets all components of the vector to zero */
        public Vec4d() { _v[0] = 0; _v[1] = 0; _v[2] = 0; _v[3] = 0; }
        public Vec4d(double x, double y, double z, double w) { _v[0] = x; _v[1] = y; _v[2] = z; _v[3] = w; }
        public Vec4d(Vec3d v3, double w)
        {
            _v[0]=v3[0];
            _v[1]=v3[1];
            _v[2]=v3[2];
            _v[3]=w;
        }

        public static bool operator ==(Vec4d l, Vec4d r) { return l._v[0] == r._v[0] && l._v[1] == r._v[1] && l._v[2] == r._v[2] && l._v[3] == r._v[3]; }

        public static bool operator !=(Vec4d l, Vec4d r) { return l._v[0] != r._v[0] || l._v[1] != r._v[1] || l._v[2] != r._v[2] || l._v[3] != r._v[3]; }

        public static bool operator <(Vec4d l, Vec4d r)
        {
            if (l._v[0] < r._v[0]) return true;
            else if (l._v[0] > r._v[0]) return false;
            else if (l._v[1] < r._v[1]) return true;
            else if (l._v[1] > r._v[1]) return false;
            else if (l._v[2] < r._v[2]) return true;
            else if (l._v[2] > r._v[2]) return false;
            else return (l._v[3] < r._v[3]);
        }
        public static bool operator >(Vec4d l, Vec4d r)
        {
            if (l._v[0] > r._v[0]) return true;
            else if (l._v[0] < r._v[0]) return false;
            else if (l._v[1] > r._v[1]) return true;
            else if (l._v[1] < r._v[1]) return false;
            else if (l._v[2] > r._v[2]) return true;
            else if (l._v[2] < r._v[2]) return false;
            else return (l._v[3] > r._v[3]);
        }

        public void Set(double x, double y, double z, double w) { _v[0] = x; _v[1] = y; _v[2] = z; _v[3] = z; }
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
        public double W { get { return _v[3]; } set { _v[3] = value; } }

        public double R { get { return _v[0]; } set { _v[0] = value; } }
        public double G { get { return _v[1]; } set { _v[1] = value; } }
        public double B { get { return _v[2]; } set { _v[2] = value; } }
        public double A { get { return _v[3]; } set { _v[3] = value; } }

        /** Returns true if all components have values that are not NaN. */
        public bool IsValid() { return !IsNaN(); }
        /** Returns true if at least one component has value NaN. */
        public bool IsNaN() { return double.IsNaN(_v[0]) || double.IsNaN(_v[1]) || double.IsNaN(_v[2]) || double.IsNaN(_v[3]); }

        /** Dot product. */
        public static double operator *(Vec4d lhs, Vec4d rhs)
        {
            return lhs._v[0] * rhs._v[0] + lhs._v[1] * rhs._v[1] + lhs._v[2] * rhs._v[2] + lhs._v[3] * rhs._v[3];
        }

        /** Multiply by scalar. */
        public static Vec4d operator *(Vec4d lhs, double rhs)
        {
            return new Vec4d(lhs._v[0] * rhs, lhs._v[1] * rhs, lhs._v[2] * rhs, lhs._v[3] * rhs);
        }


        /** Divide by scalar. */
        public static Vec4d operator /(Vec4d lhs, double rhs)
        {
            return new Vec4d(lhs._v[0] / rhs, lhs._v[1] / rhs, lhs._v[2] / rhs, lhs._v[3] / rhs);
        }


        /** Binary vector add. */
        public static Vec4d operator +(Vec4d lhs, Vec4d rhs)
        {
            return new Vec4d(lhs._v[0] + rhs._v[0], lhs._v[1] + rhs._v[1], lhs._v[2] + rhs._v[2], lhs._v[3] + rhs._v[3]);
        }


        /** Binary vector subtract. */
        public static Vec4d operator -(Vec4d lhs, Vec4d rhs)
        {
            return new Vec4d(lhs._v[0] - rhs._v[0], lhs._v[1] - rhs._v[1], lhs._v[2] - rhs._v[2], lhs._v[3] - rhs._v[3]);
        }

        /** Unary vector operations. Slightly more efficient because no temporary
         * doubleermediate object.
       */
        public Vec4d Add(Vec4d rhs)
        {
            _v[0] += rhs._v[0];
            _v[1] += rhs._v[1];
            _v[2] += rhs._v[2];
            _v[3] += rhs._v[3];
            return this;
        }
        public Vec4d Substract(Vec4d rhs)
        {
            _v[0] -= rhs._v[0];
            _v[1] -= rhs._v[1];
            _v[2] += rhs._v[2];
            _v[3] += rhs._v[3]; 
            return this;
        }
        public Vec4d Mul(double rhs)
        {
            _v[0] *= rhs;
            _v[1] *= rhs;
            _v[2] *= rhs;
            _v[3] *= rhs;
            return this;
        }
        public Vec4d Div(double rhs)
        {
            _v[0] /= rhs;
            _v[1] /= rhs;
            _v[2] /= rhs;
            _v[3] /= rhs;
            return this;
        }
        /** multiply by vector components. */
        public static Vec4d ComponentMultiply(Vec4d lhs, Vec4d rhs)
        {
            return new Vec4d(lhs[0] * rhs[0], lhs[1] * rhs[1], lhs[2] * rhs[2], lhs[3] * rhs[3]);
        }

        /** divide rhs components by rhs vector components. */
        public static Vec4d ComponentDivide(Vec4d lhs, Vec4d rhs)
        {
            return new Vec4d(lhs[0] / rhs[0], lhs[1] / rhs[1], lhs[2] / rhs[2], lhs[3] / rhs[3]);
        }
        /** Length of the vector = sqrt( vec . vec ) */
        public double Length
        {
            get { return (double)Math.Sqrt(_v[0] * _v[0] + _v[1] * _v[1] + _v[2] * _v[2] + _v[3] * _v[3]); }
        }

        /** Length squared of the vector = vec . vec */
        public double Length2
        {
            get { return _v[0] * _v[0] + _v[1] * _v[1] + _v[2] * _v[2] + _v[3] * _v[3]; }
        }

        public double Normalize()
        {
            double norm = this.Length;
            if (norm>0.0)
            {
                double inv = 1.0f / norm;
                _v[0] *= inv;
                _v[1] *= inv;
                _v[2] *= inv;
                _v[3] *= inv;
            }
            return( norm );
        }
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "({0}, {1}, {2}, {3})", _v[0], _v[1], _v[2], _v[3]);
        }
        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Podouble return false.
            Vec4d p = obj as Vec4d;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (_v[0] == p._v[0]) && (_v[1] == p._v[1]) && (_v[2] == p._v[2]) && (_v[3] == p._v[3]);
        }

        public bool Equals(Vec4d p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (_v[0] == p._v[0]) && (_v[1] == p._v[1]) && (_v[2] == p._v[2]) && (_v[3] == p._v[3]);
        }

        public override int GetHashCode()
        {
            return _v[0].GetHashCode() ^ _v[1].GetHashCode() ^ _v[2].GetHashCode() ^ _v[3].GetHashCode();
        }
    }    // end of class Vec4d

}
