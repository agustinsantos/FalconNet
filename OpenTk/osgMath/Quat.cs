using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using value_type = System.Double;

namespace OSGNet.Osg
{
    /// <summary>
    ///  A quaternion class. It can be used to represent an orientation in 3D space.
    /// Good introductions to Quaternions at:
    /// http://www.gamasutra.com/features/programming/19980703/quaternions_01.htm
    /// http://mathworld.wolfram.com/Quaternion.html
    /// </summary>
    public class Quat
    {
        public Quat() { _v[0] = 0.0; _v[1] = 0.0; _v[2] = 0.0; _v[3] = 1.0; }

        public Quat(value_type x, value_type y, value_type z, value_type w)
        {
            _v[0] = x;
            _v[1] = y;
            _v[2] = z;
            _v[3] = w;
        }

        public Quat(Quat v)
        {
            _v[0] = v.X;
            _v[1] = v.Y;
            _v[2] = v.Z;
            _v[3] = v.W;
        }

        public Quat(Vec4f v)
        {
            _v[0] = v.X;
            _v[1] = v.Y;
            _v[2] = v.Z;
            _v[3] = v.W;
        }

        public Quat(Vec4d v)
        {
            _v[0] = v.X;
            _v[1] = v.Y;
            _v[2] = v.Z;
            _v[3] = v.W;
        }

        public Quat(value_type angle, Vec3f axis)
        {
            MakeRotate(angle, axis);
        }

        public Quat(value_type angle, Vec3d axis)
        {
            MakeRotate(angle, axis);
        }

        public Quat(value_type angle1, Vec3f axis1,
                     value_type angle2, Vec3f axis2,
                     value_type angle3, Vec3f axis3)
        {
            MakeRotate(angle1, axis1, angle2, axis2, angle3, axis3);
        }

        public Quat(value_type angle1, Vec3d axis1,
                     value_type angle2, Vec3d axis2,
                     value_type angle3, Vec3d axis3)
        {
            MakeRotate(angle1, axis1, angle2, axis2, angle3, axis3);
        }

        public Quat Copy(Quat v) { _v[0] = v._v[0]; _v[1] = v._v[1]; _v[2] = v._v[2]; _v[3] = v._v[3]; return this; }

        public static bool operator ==(Quat l, Quat v) { return l._v[0] == v._v[0] && l._v[1] == v._v[1] && l._v[2] == v._v[2] && l._v[3] == v._v[3]; }

        public static bool operator !=(Quat l, Quat v) { return l._v[0] != v._v[0] || l._v[1] != v._v[1] || l._v[2] != v._v[2] || l._v[3] != v._v[3]; }

        public static bool operator <(Quat l, Quat v)
        {
            if (l._v[0] < v._v[0]) return true;
            else if (l._v[0] > v._v[0]) return false;
            else if (l._v[1] < v._v[1]) return true;
            else if (l._v[1] > v._v[1]) return false;
            else if (l._v[2] < v._v[2]) return true;
            else if (l._v[2] > v._v[2]) return false;
            else return (l._v[3] < v._v[3]);
        }

        public static bool operator >(Quat l, Quat v)
        {
            if (l._v[0] > v._v[0]) return true;
            else if (l._v[0] < v._v[0]) return false;
            else if (l._v[1] > v._v[1]) return true;
            else if (l._v[1] < v._v[1]) return false;
            else if (l._v[2] > v._v[2]) return true;
            else if (l._v[2] < v._v[2]) return false;
            else return (l._v[3] > v._v[3]);
        }

        /* ----------------------------------
           Methods to access data members
        ---------------------------------- */

        public Vec4d AsVec4()
        {
            return new Vec4d(_v[0], _v[1], _v[2], _v[3]);
        }

        public Vec3d AsVec3()
        {
            return new Vec3d(_v[0], _v[1], _v[2]);
        }

        public void Set(value_type x, value_type y, value_type z, value_type w)
        {
            _v[0] = x;
            _v[1] = y;
            _v[2] = z;
            _v[3] = w;
        }

        public void Set(Vec4f v)
        {
            _v[0] = v.X;
            _v[1] = v.Y;
            _v[2] = v.Z;
            _v[3] = v.W;
        }

        public void Set(Vec4d v)
        {
            _v[0] = v.X;
            _v[1] = v.Y;
            _v[2] = v.Z;
            _v[3] = v.W;
        }

        public void Set(Matrixf matrix)
        {
            this._v = matrix.GetRotate()._v;
        }

        public void Set(Matrixd matrix)
        {
            this._v = matrix.GetRotate()._v;
        }

        public void Get(Matrixf matrix)
        {
            matrix.MakeRotate(this);
        }

        public void Get(Matrixd matrix)
        {
            matrix.MakeRotate(this);
        }


        public value_type this[int i]
        {
            get { return _v[i]; }
            set { _v[i] = value; }
        }

        public value_type X { get { return _v[0]; } }
        public value_type Y { get { return _v[1]; } }
        public value_type Z { get { return _v[2]; } }
        public value_type W { get { return _v[3]; } }


        /** return true if the Quat represents a zero rotation, and therefore can be ignored in computations.*/
        public bool ZeroRotation() { return _v[0] == 0.0 && _v[1] == 0.0 && _v[2] == 0.0 && _v[3] == 1.0; }


        /* -------------------------------------------------------------
                  BASIC ARITHMETIC METHODS
       Implemented in terms of Vec4s.  Some Vec4 operators, e.g.
       operator* are not appropriate for quaternions (as
       mathematical objects) so they are implemented differently.
       Also define methods for conjugate and the multiplicative inverse.
       ------------------------------------------------------------- */
        /// Multiply by scalar
        public static Quat operator *(Quat lhs, value_type rhs)
        {
            return new Quat(lhs._v[0] * rhs, lhs._v[1] * rhs, lhs._v[2] * rhs, lhs._v[3] * rhs);
        }

        /// Unary multiply by scalar
        public Quat Mult(value_type rhs)
        {
            _v[0] *= rhs;
            _v[1] *= rhs;
            _v[2] *= rhs;
            _v[3] *= rhs;
            return this;        // enable nesting
        }

        /// Binary multiply
        public static Quat operator *(Quat lhs, Quat rhs)
        {
            return new Quat(rhs._v[3] * lhs._v[0] + rhs._v[0] * lhs._v[3] + rhs._v[1] * lhs._v[2] - rhs._v[2] * lhs._v[1],
                 rhs._v[3] * lhs._v[1] - rhs._v[0] * lhs._v[2] + rhs._v[1] * lhs._v[3] + rhs._v[2] * lhs._v[0],
                 rhs._v[3] * lhs._v[2] + rhs._v[0] * lhs._v[1] - rhs._v[1] * lhs._v[0] + rhs._v[2] * lhs._v[3],
                 rhs._v[3] * lhs._v[3] - rhs._v[0] * lhs._v[0] - rhs._v[1] * lhs._v[1] - rhs._v[2] * lhs._v[2]);
        }

        /// Unary multiply
        public Quat Mult(Quat rhs)
        {
            value_type x = rhs._v[3] * _v[0] + rhs._v[0] * _v[3] + rhs._v[1] * _v[2] - rhs._v[2] * _v[1];
            value_type y = rhs._v[3] * _v[1] - rhs._v[0] * _v[2] + rhs._v[1] * _v[3] + rhs._v[2] * _v[0];
            value_type z = rhs._v[3] * _v[2] + rhs._v[0] * _v[1] - rhs._v[1] * _v[0] + rhs._v[2] * _v[3];
            _v[3] = rhs._v[3] * _v[3] - rhs._v[0] * _v[0] - rhs._v[1] * _v[1] - rhs._v[2] * _v[2];

            _v[2] = z;
            _v[1] = y;
            _v[0] = x;

            return this;            // enable nesting
        }

        /// Divide by scalar
        public static Quat operator /(Quat lhs, value_type rhs)
        {
            value_type div = 1.0 / rhs;
            return new Quat(lhs._v[0] * div, lhs._v[1] * div, lhs._v[2] * div, lhs._v[3] * div);
        }

        /// Unary divide by scalar
        public Quat Div(value_type rhs)
        {
            value_type div = 1.0 / rhs;
            _v[0] *= div;
            _v[1] *= div;
            _v[2] *= div;
            _v[3] *= div;
            return this;
        }

        /// Binary divide
        public Quat Div(Quat denom)
        {
            return (this * denom.Inverse());
        }



        /// Binary addition
        public static Quat operator +(Quat lhs, Quat rhs)
        {
            return new Quat(lhs._v[0] + rhs._v[0], lhs._v[1] + rhs._v[1],
                lhs._v[2] + rhs._v[2], lhs._v[3] + rhs._v[3]);
        }

        /// Unary addition
        public Quat Add(Quat rhs)
        {
            _v[0] += rhs._v[0];
            _v[1] += rhs._v[1];
            _v[2] += rhs._v[2];
            _v[3] += rhs._v[3];
            return this;            // enable nesting
        }

        /// Binary subtraction
        public static Quat operator -(Quat lhs, Quat rhs)
        {
            return new Quat(lhs._v[0] - rhs._v[0], lhs._v[1] - rhs._v[1],
                lhs._v[2] - rhs._v[2], lhs._v[3] - rhs._v[3]);
        }

        /// Unary subtraction
        public Quat Sub(Quat rhs)
        {
            _v[0] -= rhs._v[0];
            _v[1] -= rhs._v[1];
            _v[2] -= rhs._v[2];
            _v[3] -= rhs._v[3];
            return this;            // enable nesting
        }

        /** Negation operator - returns the negative of the quaternion.
        Basically just calls operator - () on the Vec4 */
        public static Quat operator -(Quat lhs)
        {
            return new Quat(-lhs._v[0], -lhs._v[1], -lhs._v[2], -lhs._v[3]);
        }

        /// Length of the quaternion = sqrt( vec . vec )
        public value_type Length
        {
            get { return Math.Sqrt(_v[0] * _v[0] + _v[1] * _v[1] + _v[2] * _v[2] + _v[3] * _v[3]); }
        }

        /// Length of the quaternion = vec . vec
        public value_type Length2
        {
            get { return _v[0] * _v[0] + _v[1] * _v[1] + _v[2] * _v[2] + _v[3] * _v[3]; }
        }

        /// Conjugate
        public Quat Conjugate()
        {
            return new Quat(-_v[0], -_v[1], -_v[2], _v[3]);
        }

        /// Multiplicative inverse method: q^(-1) = q^*/(q.q^*)
        public Quat Inverse()
        {
            return Conjugate() / Length2;
        }
        const value_type epsilon = 0.0000001;

        /* --------------------------------------------------------
                 METHODS RELATED TO ROTATIONS
          Set a quaternion which will perform a rotation of an
          angle around the axis given by the vector (x,y,z).
          Should be written to also accept an angle and a Vec3?

          Define Spherical Linear interpolation method also

          Not inlined - see the Quat.cpp file for implementation
          -------------------------------------------------------- */
        public void MakeRotate(value_type angle,
                                value_type x, value_type y, value_type z)
        {

            value_type length = Math.Sqrt(x * x + y * y + z * z);
            if (length < epsilon)
            {
                // ~zero length axis, so reset rotation to zero.
                this._v = new value_type[4] { 0, 0, 0, 1 };
                return;
            }

            value_type inversenorm = 1.0 / length;
            value_type coshalfangle = Math.Cos(0.5 * angle);
            value_type sinhalfangle = Math.Sin(0.5 * angle);

            _v[0] = x * sinhalfangle * inversenorm;
            _v[1] = y * sinhalfangle * inversenorm;
            _v[2] = z * sinhalfangle * inversenorm;
            _v[3] = coshalfangle;
        }

        public void MakeRotate(value_type angle, Vec3f vec)
        {
            MakeRotate(angle, vec[0], vec[1], vec[2]);
        }

        public void MakeRotate(value_type angle, Vec3d vec)
        {
            MakeRotate(angle, vec[0], vec[1], vec[2]);
        }

        public void MakeRotate(value_type angle1, Vec3f axis1,
                          value_type angle2, Vec3f axis2,
                          value_type angle3, Vec3f axis3)
        {
            MakeRotate(angle1, new Vec3d(axis1),
                       angle2, new Vec3d(axis2),
                       angle3, new Vec3d(axis3));
        }
        public void MakeRotate(value_type angle1, Vec3d axis1,
                          value_type angle2, Vec3d axis2,
                          value_type angle3, Vec3d axis3)
        {
            Quat q1 = new Quat(); q1.MakeRotate(angle1, axis1);
            Quat q2 = new Quat(); q2.MakeRotate(angle2, axis2);
            Quat q3 = new Quat(); q3.MakeRotate(angle3, axis3);

            this.Copy(q1 * q2 * q3);
        }


        /** Make a rotation Quat which will rotate vec1 to vec2.
            Generally take a dot product to get the angle between these
            and then use a cross product to get the rotation axis
            Watch out for the two special cases when the vectors
            are co-incident or opposite in direction.*/
        public void MakeRotate(Vec3f from, Vec3f to)
        {
            MakeRotate(new Vec3d(from), new Vec3d(to));
        }

        /** Make a rotation Quat which will rotate vec1 to vec2.
            Generally take a dot product to get the angle between these
            and then use a cross product to get the rotation axis
            Watch out for the two special cases of when the vectors
            are co-incident or opposite in direction.*/
        public void MakeRotate(Vec3d from, Vec3d to)
        {

            // This routine takes any vector as argument but normalized
            // vectors are necessary, if only for computing the dot product.
            // Too bad the API is that generic, it leads to performance loss.
            // Even in the case the 2 vectors are not normalized but same length,
            // the sqrt could be shared, but we have no way to know beforehand
            // at this point, while the caller may know.
            // So, we have to test... in the hope of saving at least a sqrt
            Vec3d sourceVector = from;
            Vec3d targetVector = to;

            value_type fromLen2 = from.Length2;
            value_type fromLen;
            // normalize only when necessary, epsilon test
            if ((fromLen2 < 1.0 - 1e-7) || (fromLen2 > 1.0 + 1e-7))
            {
                fromLen = Math.Sqrt(fromLen2);
                sourceVector /= fromLen;
            }
            else fromLen = 1.0;

            value_type toLen2 = to.Length2;
            // normalize only when necessary, epsilon test
            if ((toLen2 < 1.0 - 1e-7) || (toLen2 > 1.0 + 1e-7))
            {
                value_type toLen;
                // re-use fromLen for case of mapping 2 vectors of the same length
                if ((toLen2 > fromLen2 - 1e-7) && (toLen2 < fromLen2 + 1e-7))
                {
                    toLen = fromLen;
                }
                else toLen = Math.Sqrt(toLen2);
                targetVector /= toLen;
            }


            // Now let's get into the real stuff
            // Use "dot product plus one" as test as it can be re-used later on
            double dotProdPlus1 = 1.0 + sourceVector * targetVector;

            // Check for degenerate case of full u-turn. Use epsilon for detection
            if (dotProdPlus1 < 1e-7)
            {

                // Get an orthogonal vector of the given vector
                // in a plane with maximum vector coordinates.
                // Then use it as quaternion axis with pi angle
                // Trick is to realize one value at least is >0.6 for a normalized vector.
                if (Math.Abs(sourceVector.X) < 0.6)
                {
                      double norm = Math.Sqrt(1.0 - sourceVector.X * sourceVector.X);
                    _v[0] = 0.0;
                    _v[1] = sourceVector.Z / norm;
                    _v[2] = -sourceVector.Y / norm;
                    _v[3] = 0.0;
                }
                else if (Math.Abs(sourceVector.Y) < 0.6)
                {
                    double norm = Math.Sqrt(1.0 - sourceVector.Y * sourceVector.Y);
                    _v[0] = -sourceVector.Z / norm;
                    _v[1] = 0.0;
                    _v[2] = sourceVector.X / norm;
                    _v[3] = 0.0;
                }
                else
                {
                    double norm = Math.Sqrt(1.0 - sourceVector.Z * sourceVector.Z);
                    _v[0] = sourceVector.Y / norm;
                    _v[1] = -sourceVector.X / norm;
                    _v[2] = 0.0;
                    _v[3] = 0.0;
                }
            }

            else
            {
                // Find the shortest angle quaternion that transforms normalized vectors
                // into one other. Formula is still valid when vectors are colinear
                double s = Math.Sqrt(0.5 * dotProdPlus1);
                Vec3d tmp = sourceVector.Cross(targetVector / (2.0 * s));
                _v[0] = tmp.X;
                _v[1] = tmp.Y;
                _v[2] = tmp.Z;
                _v[3] = s;
            }
        }

        public void MakeRotate_original(Vec3d vec1, Vec3d vec2)
        {
            throw new NotImplementedException();
        }

        /** Return the angle and vector components represented by the quaternion.*/
        public void GetRotate(out value_type angle, out value_type x, out value_type y, out value_type z)
        {
            value_type sinhalfangle = Math.Sqrt(_v[0] * _v[0] + _v[1] * _v[1] + _v[2] * _v[2]);

            angle = 2.0 * Math.Atan2(sinhalfangle, _v[3]);
            if (sinhalfangle != 0)
            {
                x = _v[0] / sinhalfangle;
                y = _v[1] / sinhalfangle;
                z = _v[2] / sinhalfangle;
            }
            else
            {
                x = 0.0;
                y = 0.0;
                z = 1.0;
            }

        }

        /** Return the angle and vector represented by the quaternion.*/
        public void GetRotate(value_type angle, ref Vec3f vec)
        {
            value_type x, y, z;
            GetRotate(out angle, out x, out y, out z);
            vec[0] = (float)x;
            vec[1] = (float)y;
            vec[2] = (float)z;
        }

        /** Return the angle and vector represented by the quaternion.*/
        public void GetRotate(value_type angle, ref Vec3d vec)
        {
            value_type x, y, z;
            GetRotate(out angle, out x, out y, out z);
            vec[0] = x;
            vec[1] = y;
            vec[2] = z;
        }

        /// <summary>
        /// Spherical Linear Interpolation
        /// As t goes from 0 to 1, the Quat object goes from "from" to "to"
        /// Reference: Shoemake at SIGGRAPH 89
        /// See also
        /// http://www.gamasutra.com/features/programming/19980703/quaternions_01.htm
        /// </summary>
        /// <param name="t"></param>
        /// <param name="?"></param>
        public void Slerp(value_type t, Quat from, Quat to)
        {
            const double epsilon = 0.00001;
            double omega, cosomega, sinomega, scale_from, scale_to;

            Quat quatTo = new Quat(to);
            // this is a dot product

            cosomega = from.AsVec4() * to.AsVec4();

            if (cosomega < 0.0)
            {
                cosomega = -cosomega;
                quatTo = -to;
            }

            if ((1.0 - cosomega) > epsilon)
            {
                omega = Math.Acos(cosomega);  // 0 <= omega <= Pi (see man acos)
                sinomega = Math.Sin(omega);  // this sinomega should always be +ve so
                // could try sinomega=sqrt(1-cosomega*cosomega) to avoid a sin()?
                scale_from = Math.Sin((1.0 - t) * omega) / sinomega;
                scale_to = Math.Sin(t * omega) / sinomega;
            }
            else
            {
                /* --------------------------------------------------
                   The ends of the vectors are very close
                   we can use simple linear interpolation - no need
                   to worry about the "spherical" interpolation
                   -------------------------------------------------- */
                scale_from = 1.0 - t;
                scale_to = t;
            }

            this.Copy((from * scale_from) + (quatTo * scale_to));

            // so that we get a Vec4
        }

        /** Rotate a vector by this quaternion.*/
        public static Vec3f operator *(Quat lhs, Vec3f v)
        {
            // nVidia SDK implementation
            Vec3f uv, uuv;
            Vec3f qvec = new Vec3f((float)lhs._v[0], (float)lhs._v[1], (float)lhs._v[2]);
            uv = qvec.Cross(v);
            uuv = qvec.Cross(uv);
            uv *= (float)(2.0f * lhs._v[3]);
            uuv *= 2.0f;
            return v + uv + uuv;
        }

        /** Rotate a vector by this quaternion.*/
        public static Vec3d operator *(Quat lhs, Vec3d v)
        {
            // nVidia SDK implementation
            Vec3d uv, uuv;
            Vec3d qvec = new Vec3d(lhs._v[0], lhs._v[1], lhs._v[2]);
            uv = qvec.Cross(v);
            uuv = qvec.Cross(uv);
            uv *= (2.0f * lhs._v[3]);
            uuv *= 2.0f;
            return v + uv + uuv;
        }


        protected internal value_type[] _v = new value_type[4];    // a four-vector
    }
}
