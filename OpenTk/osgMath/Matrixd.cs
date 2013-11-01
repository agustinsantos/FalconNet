using System;
using value_type = System.Double;

namespace OSGNet.Osg
{
    public class Matrixd
    {
        public Matrixd() { MakeIdentity(); }
        public Matrixd(Matrixf mat) { Set(mat.Data); }
        public Matrixd(Matrixd mat)
        {
            Set(mat.Data);
        }
        public Matrixd(float[,] ptr) { Set(ptr); }
        public Matrixd(double[,] ptr) { Set(ptr); }
        public Matrixd(Quat quat) { MakeRotate(quat); }

        public Matrixd(value_type a00, value_type a01, value_type a02, value_type a03,
                 value_type a10, value_type a11, value_type a12, value_type a13,
                 value_type a20, value_type a21, value_type a22, value_type a23,
                 value_type a30, value_type a31, value_type a32, value_type a33)
        {
            SetRow(0, a00, a01, a02, a03);
            SetRow(1, a10, a11, a12, a13);
            SetRow(2, a20, a21, a22, a23);
            SetRow(3, a30, a31, a32, a33);
        }


        public int Compare(Matrixd m)
        {
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    if (this._mat[i, j] < m._mat[i, j]) return -1;
                    if (this._mat[i, j] < m._mat[i, j]) return 1;
                }
            return 0;
        }

        public static bool operator <(Matrixd l, Matrixd r) { return l.Compare(r) < 0; }
        public static bool operator >(Matrixd l, Matrixd r) { return l.Compare(r) > 0; }
        public static bool operator ==(Matrixd l, Matrixd r) { return l.Compare(r) == 0; }
        public static bool operator !=(Matrixd l, Matrixd r) { return l.Compare(r) != 0; }

        public value_type this[int row, int col] { get { return _mat[row, col]; } }

        public bool IsValid { get { return !this.IsNaN; } }
        public bool IsNaN
        {
            get
            {
                return double.IsNaN(_mat[0, 0]) || double.IsNaN(_mat[0, 1]) || double.IsNaN(_mat[0, 2]) || double.IsNaN(_mat[0, 3]) ||
                               double.IsNaN(_mat[1, 0]) || double.IsNaN(_mat[1, 1]) || double.IsNaN(_mat[1, 2]) || double.IsNaN(_mat[1, 3]) ||
                               double.IsNaN(_mat[2, 0]) || double.IsNaN(_mat[2, 1]) || double.IsNaN(_mat[2, 2]) || double.IsNaN(_mat[2, 3]) ||
                               double.IsNaN(_mat[3, 0]) || double.IsNaN(_mat[3, 1]) || double.IsNaN(_mat[3, 2]) || double.IsNaN(_mat[3, 3]);
            }
        }

        //public Matrixf operator = (  Matrixf rhs)
        //{
        //    if( &rhs == this ) return *this;
        //    set(rhs.ptr());
        //    return *this;
        //}

        //public Matrixf operator = (  Matrixd& other);

        public void Set(Matrixf rhs)
        {
            Set(rhs.Data);
        }

        public void Set(Matrixd rhs)
        {
            Set(rhs.Data);
        }

        public void Set(float[,] data)
        {
            for (int i = 0; i < 4; ++i)
                for (int j = 0; j < 4; ++j)
                    _mat[i, j] = data[i, j];
        }

        public void Set(double[,] data)
        {
            for (int i = 0; i < 4; ++i)
                for (int j = 0; j < 4; ++j)
                    _mat[i, j] = (value_type)data[i, j];
        }

        public void Set(value_type a00, value_type a01, value_type a02, value_type a03,
                 value_type a10, value_type a11, value_type a12, value_type a13,
                 value_type a20, value_type a21, value_type a22, value_type a23,
                 value_type a30, value_type a31, value_type a32, value_type a33)
        {
            SetRow(0, a00, a01, a02, a03);
            SetRow(1, a10, a11, a12, a13);
            SetRow(2, a20, a21, a22, a23);
            SetRow(3, a30, a31, a32, a33);
        }

        public value_type[,] Data { get { return _mat; } }

        public bool IsIdentity()
        {
            return _mat[0, 0] == 1.0f && _mat[0, 1] == 0.0f && _mat[0, 2] == 0.0f && _mat[0, 3] == 0.0f &&
                   _mat[1, 0] == 0.0f && _mat[1, 1] == 1.0f && _mat[1, 2] == 0.0f && _mat[1, 3] == 0.0f &&
                   _mat[2, 0] == 0.0f && _mat[2, 1] == 0.0f && _mat[2, 2] == 1.0f && _mat[2, 3] == 0.0f &&
                   _mat[3, 0] == 0.0f && _mat[3, 1] == 0.0f && _mat[3, 2] == 0.0f && _mat[3, 3] == 1.0f;
        }

        public void MakeIdentity()
        {
            SetRow(0, 1, 0, 0, 0);
            SetRow(1, 0, 1, 0, 0);
            SetRow(2, 0, 0, 1, 0);
            SetRow(3, 0, 0, 0, 1);
        }

        public void MakeScale(Vec3f v)
        {
            MakeScale(v[0], v[1], v[2]);
        }

        public void MakeScale(Vec3d v)
        {
            MakeScale(v[0], v[1], v[2]);
        }

        public void MakeScale(value_type x, value_type y, value_type z)
        {
            SetRow(0, x, 0, 0, 0);
            SetRow(1, 0, y, 0, 0);
            SetRow(2, 0, 0, z, 0);
            SetRow(3, 0, 0, 0, 1);
        }

        public void MakeTranslate(Vec3f v)
        {
            MakeTranslate(v[0], v[1], v[2]);
        }

        public void MakeTranslate(Vec3d v)
        {
            MakeTranslate(v[0], v[1], v[2]);
        }

        public void MakeTranslate(value_type x, value_type y, value_type z)
        {
            SetRow(0, 1, 0, 0, 0);
            SetRow(1, 0, 1, 0, 0);
            SetRow(2, 0, 0, 1, 0);
            SetRow(3, x, y, z, 1);
        }

        public void MakeRotate(Vec3f from, Vec3f to)
        {
            MakeIdentity();

            Quat quat = new Quat();
            quat.MakeRotate(from, to);
            SetRotate(quat);
        }

        public void MakeRotate(Vec3d from, Vec3d to)
        {
            MakeIdentity();

            Quat quat = new Quat();
            quat.MakeRotate(from, to);
            SetRotate(quat);
        }

        public void MakeRotate(value_type angle, Vec3f axis)
        {
            MakeIdentity();

            Quat quat = new Quat();
            quat.MakeRotate(angle, axis);
            SetRotate(quat);
        }

        public void MakeRotate(value_type angle, Vec3d axis)
        {
            MakeIdentity();

            Quat quat = new Quat();
            quat.MakeRotate(angle, axis);
            SetRotate(quat);
        }

        public void MakeRotate(value_type angle, value_type x, value_type y, value_type z)
        {
            MakeIdentity();

            Quat quat = new Quat();
            quat.MakeRotate(angle, x, y, z);
            SetRotate(quat);
        }

        public void MakeRotate(Quat quat)
        {
            MakeIdentity();

            SetRotate(quat);
        }

        public void MakeRotate(value_type angle1, Vec3f axis1,
                         value_type angle2, Vec3f axis2,
                         value_type angle3, Vec3f axis3)
        {
            MakeIdentity();

            Quat quat = new Quat();
            quat.MakeRotate(angle1, axis1,
                            angle2, axis2,
                            angle3, axis3);
            SetRotate(quat);
        }

        public void MakeRotate(value_type angle1, Vec3d axis1,
                         value_type angle2, Vec3d axis2,
                         value_type angle3, Vec3d axis3)
        {
            MakeIdentity();

            Quat quat = new Quat();
            quat.MakeRotate(angle1, axis1,
                            angle2, axis2,
                            angle3, axis3);
            SetRotate(quat);
        }


        /// <summary>
        /// decompose the matrix into translation, rotation, scale and scale orientation
        /// </summary>
        /// <param name="translation"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        /// <param name="so"></param>
        public void Decompose(out Vec3f translation,
                              out Quat rotation,
                              out Vec3f scale,
                              out Quat so)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// decompose the matrix into translation, rotation, scale and scale orientation
        /// </summary>
        /// <param name="translation"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        /// <param name="so"></param>
        public void Decompose(out Vec3d translation,
                         out Quat rotation,
                         out Vec3d scale,
                         out Quat so)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Set to an orthographic projection.
        /// See glOrtho for further details.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        /// <param name="top"></param>
        /// <param name="zNear"></param>
        /// <param name="zFar"></param>
        public void MakeOrtho(double left, double right,
                       double bottom, double top,
                       double zNear, double zFar)
        {
            // note transpose of Matrix_implementation wr.t OpenGL documentation, since the OSG use post multiplication rather than pre.
            double tx = -(right + left) / (right - left);
            double ty = -(top + bottom) / (top - bottom);
            double tz = -(zFar + zNear) / (zFar - zNear);
            SetRow(0, 2.0 / (right - left), 0.0, 0.0, 0.0);
            SetRow(1, 0.0, 2.0 / (top - bottom), 0.0, 0.0);
            SetRow(2, 0.0, 0.0, -2.0 / (zFar - zNear), 0.0);
            SetRow(3, tx, ty, tz, 1.0);
        }


        /// <summary>
        /// Get the orthographic settings of the orthographic projection matrix.
        ///  Note, if matrix is not an orthographic matrix then invalid values
        ///  will be returned.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        /// <param name="top"></param>
        /// <param name="zNear"></param>
        /// <param name="zFar"></param>
        /// <returns></returns>
        public bool GetOrtho(out double left, out double right,
                      out double bottom, out double top,
                      out double zNear, out double zFar)
        {
            left = right = bottom = top = zNear = zFar = 0.0;
            if (_mat[0, 3] != 0.0 || _mat[1, 3] != 0.0 || _mat[2, 3] != 0.0 || _mat[3, 3] != 1.0) return false;

            zNear = (_mat[3, 2] + 1.0) / _mat[2, 2];
            zFar = (_mat[3, 2] - 1.0) / _mat[2, 2];

            left = -(1.0 + _mat[3, 0]) / _mat[0, 0];
            right = (1.0 - _mat[3, 0]) / _mat[0, 0];

            bottom = -(1.0 + _mat[3, 1]) / _mat[1, 1];
            top = (1.0 - _mat[3, 1]) / _mat[1, 1];

            return true;
        }

        /// <summary>
        /// float version of getOrtho(..)
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        /// <param name="top"></param>
        /// <param name="zNear"></param>
        /// <param name="zFar"></param>
        /// <returns></returns>
        public bool GetOrtho(out float left, out float right,
                      out float bottom, out  float top,
                      out float zNear, out float zFar)
        {
            left = right = bottom = top = zNear = zFar = 0.0f;

            if (_mat[0, 3] != 0.0 || _mat[1, 3] != 0.0 || _mat[2, 3] != 0.0 || _mat[3, 3] != 1.0) return false;

            zNear = (float)((_mat[3, 2] + 1.0) / _mat[2, 2]);
            zFar = (float)((_mat[3, 2] - 1.0) / _mat[2, 2]);

            left = -(float)((1.0 + _mat[3, 0]) / _mat[0, 0]);
            right = (float)((1.0 - _mat[3, 0]) / _mat[0, 0]);

            bottom = -(float)((1.0 + _mat[3, 1]) / _mat[1, 1]);
            top = (float)((1.0 - _mat[3, 1]) / _mat[1, 1]);

            return true;
        }


        new        /// <summary>
            /// Set to a 2D orthographic projection.
            /// See glOrtho2D for further details.
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            /// <param name="bottom"></param>
            /// <param name="top"></param>
               public void MakeOrtho2D(double left, double right,
                                       double bottom, double top)
        {
            MakeOrtho(left, right, bottom, top, -1.0, 1.0);
        }


        /// <summary>
        /// Set to a perspective projection.
        /// See glFrustum for further details.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        /// <param name="top"></param>
        /// <param name="zNear"></param>
        /// <param name="zFar"></param>
        public void MakeFrustum(double left, double right,
                         double bottom, double top,
                         double zNear, double zFar)
        {
            // note transpose of Matrix_implementation wr.t OpenGL documentation, since the OSG use post multiplication rather than pre.
            double A = (right + left) / (right - left);
            double B = (top + bottom) / (top - bottom);
            double C = (Math.Abs(zFar) > float.MaxValue) ? -1.0 : -(zFar + zNear) / (zFar - zNear);
            double D = (Math.Abs(zFar) > float.MaxValue) ? -2.0 * zNear : -2.0 * zFar * zNear / (zFar - zNear);
            SetRow(0, 2.0 * zNear / (right - left), 0.0, 0.0, 0.0);
            SetRow(1, 0.0, 2.0 * zNear / (top - bottom), 0.0, 0.0);
            SetRow(2, A, B, C, -1.0);
            SetRow(3, 0.0, 0.0, D, 0.0);
        }


        /// <summary>
        /// Get the frustum settings of a perspective projection matrix.
        ///  Note, if matrix is not a perspective matrix then invalid values
        ///  will be returned.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        /// <param name="top"></param>
        /// <param name="zNear"></param>
        /// <param name="zFar"></param>
        /// <returns></returns>
        public bool GetFrustum(out double left, out  double right,
                               out double bottom, out double top,
                               out double zNear, out  double zFar)
        {
            left = right = bottom = top = zNear = zFar = 0.0f;

            if (_mat[0, 3] != 0.0 || _mat[1, 3] != 0.0 || _mat[2, 3] != -1.0 || _mat[3, 3] != 0.0)
                return false;

            // note: near and far must be used inside this method instead of zNear and zFar
            // because zNear and zFar are references and they may point to the same variable.
            value_type temp_near = _mat[3, 2] / (_mat[2, 2] - 1.0);
            value_type temp_far = _mat[3, 2] / (1.0 + _mat[2, 2]);

            left = temp_near * (_mat[2, 0] - 1.0) / _mat[0, 0];
            right = temp_near * (1.0 + _mat[2, 0]) / _mat[0, 0];

            top = temp_near * (1.0 + _mat[2, 1]) / _mat[1, 1];
            bottom = temp_near * (_mat[2, 1] - 1.0) / _mat[1, 1];

            zNear = temp_near;
            zFar = temp_far;

            return true;
        }


        /// <summary>
        /// float version of getFrustum(..)
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        /// <param name="top"></param>
        /// <param name="zNear"></param>
        /// <param name="zFar"></param>
        /// <returns></returns>
        public bool GetFrustum(out float left, out   float right,
                                out float bottom, out  float top,
                                out float zNear, out float zFar)
        {
            left = right = bottom = top = zNear = zFar = 0.0f;

            if (_mat[0, 3] != 0.0 || _mat[1, 3] != 0.0 || _mat[2, 3] != -1.0 || _mat[3, 3] != 0.0)
                return false;

            // note: near and far must be used inside this method instead of zNear and zFar
            // because zNear and zFar are references and they may point to the same variable.
            value_type temp_near = _mat[3, 2] / (_mat[2, 2] - 1.0);
            value_type temp_far = _mat[3, 2] / (1.0 + _mat[2, 2]);

            left = (float)(temp_near * (_mat[2, 0] - 1.0) / _mat[0, 0]);
            right = (float)(temp_near * (1.0 + _mat[2, 0]) / _mat[0, 0]);

            top = (float)(temp_near * (1.0 + _mat[2, 1]) / _mat[1, 1]);
            bottom = (float)(temp_near * (_mat[2, 1] - 1.0) / _mat[1, 1]);

            zNear = (float)temp_near;
            zFar = (float)temp_far;

            return true;
        }


        /// <summary>
        ///  Set to a symmetrical perspective projection.
        ///   See gluPerspective for further details.
        ///  Aspect ratio is defined as width/height.
        /// </summary>
        /// <param name="fovy"></param>
        /// <param name="aspectRatio"></param>
        /// <param name="zNear"></param>
        /// <param name="zFar"></param>
        public void MakePerspective(double fovy, double aspectRatio,
                                    double zNear, double zFar)
        {
            // calculate the appropriate left, right etc.
            double tan_fovy = Math.Tan((fovy * 0.5) * Math.PI / 180.0);
            double right = tan_fovy * aspectRatio * zNear;
            double left = -right;
            double top = tan_fovy * zNear;
            double bottom = -top;
            MakeFrustum(left, right, bottom, top, zNear, zFar);
        }

        /// <summary>
        /// Get the frustum settings of a symmetric perspective projection
        ///  matrix.
        ///  Return false if matrix is not a perspective matrix,
        ///  where parameter values are undefined.
        ///  Note, if matrix is not a symmetric perspective matrix then the
        /// shear will be lost.
        ///  Asymmetric matrices occur when stereo, power walls, caves and
        ///  reality center display are used.
        ///  In these configuration one should use the AsFrustum method instead.
        /// </summary>
        /// <param name="fovy"></param>
        /// <param name="aspectRatio"></param>
        /// <param name="zNear"></param>
        /// <param name="zFar"></param>
        /// <returns></returns>
        public bool GetPerspective(out double fovy, out double aspectRatio,
                            out  double zNear, out  double zFar)
        {
            fovy = aspectRatio = zNear = zFar = 0.0;
            value_type right = 0.0;
            value_type left = 0.0;
            value_type top = 0.0;
            value_type bottom = 0.0;

            // note: near and far must be used inside this method instead of zNear and zFar
            // because zNear and zFar are references and they may point to the same variable.
            value_type temp_near = 0.0;
            value_type temp_far = 0.0;

            // get frustum and compute results
            bool r = GetFrustum(out left, out right, out bottom, out top, out temp_near, out temp_far);
            if (r)
            {
                fovy = (Math.Atan(top / temp_near) - Math.Atan(bottom / temp_near)) * Math.PI / 180.0;
                aspectRatio = (right - left) / (top - bottom);
            }
            zNear = temp_near;
            zFar = temp_far;
            return r;
        }

        /// <summary>
        /// float version of getPerspective(..)
        /// </summary>
        /// <param name="fovy"></param>
        /// <param name="aspectRatio"></param>
        /// <param name="zNear"></param>
        /// <param name="zFar"></param>
        /// <returns></returns>
        public bool GetPerspective(out float fovy, out  float aspectRatio,
                            out float zNear, out float zFar)
        {
            fovy = aspectRatio = zNear = zFar = 0.0f;

            value_type right = 0.0;
            value_type left = 0.0;
            value_type top = 0.0;
            value_type bottom = 0.0;

            // note: near and far must be used inside this method instead of zNear and zFar
            // because zNear and zFar are references and they may point to the same variable.
            value_type temp_near = 0.0;
            value_type temp_far = 0.0;

            // get frustum and compute results
            bool r = GetFrustum(out left, out right, out bottom, out top, out temp_near, out temp_far);
            if (r)
            {
                fovy = (float)((Math.Atan(top / temp_near) - Math.Atan(bottom / temp_near)) * Math.PI / 180.0);
                aspectRatio = (float)((right - left) / (top - bottom));
            }
            zNear = (float)temp_near;
            zFar = (float)temp_far;
            return r;
        }

        public void MakeLookAt(Vec3f eye, Vec3f center, Vec3f up)
        {
            MakeLookAt(new Vec3d(eye), new Vec3d(center), new Vec3d(up));
        }

        /** Set the position and orientation to be a view matrix,
          * using the same convention as gluLookAt.
        */
        public void MakeLookAt(Vec3d eye, Vec3d center, Vec3d up)
        {
            Vec3d f = center - eye;
            f.Normalize();
            Vec3d s = f.Cross(up);
            s.Normalize();
            Vec3d u = s.Cross(f);
            u.Normalize();

            Set(s[0], u[0], -f[0], 0.0,
                s[1], u[1], -f[1], 0.0,
                s[2], u[2], -f[2], 0.0,
                0.0, 0.0, 0.0, 1.0);

            PreMultTranslate(-eye);
        }

        /// <summary>
        /// Get to the position and orientation of a modelview matrix,
        ///  using the same convention as gluLookAt.
        /// </summary>
        /// <param name="eye"></param>
        /// <param name="center"></param>
        /// <param name="up"></param>
        /// <param name="lookDistance"></param>
        public void GetLookAt(out Vec3f eye, out Vec3f center, out Vec3f up,
                              value_type lookDistance = 1.0f)
        {
            Matrixd inv = new Matrixd();
            inv.Invert(this);

            // note: e and c variables must be used inside this method instead of eye and center
            // because eye and center are references and they may point to the same variable.
            Vec3f e = new Vec3f(0.0f, 0.0f, 0.0f) * inv;
            up = Transform3x3(this, new Vec3f(0.0f, 1.0f, 0.0f));
            Vec3f c = Transform3x3(this, new Vec3f(0.0f, 0.0f, -1));
            c.Normalize();
            c = e + c * lookDistance;

            // assign the results
            eye = e;
            center = c;
        }

        /** Get to the position and orientation of a modelview matrix,
          * using the same convention as gluLookAt.
        */
        public void GetLookAt(out Vec3d eye, out Vec3d center, out Vec3d up,
                              value_type lookDistance = 1.0f)
        {
            Matrixd inv = new Matrixd();
            inv.Invert(this);

            // note: e and c variables must be used inside this method instead of eye and center
            // because eye and center are references and they may point to the same variable.
            Vec3d e = new Vec3d(0.0, 0.0, 0.0) * inv;
            up = Transform3x3(this, new Vec3d(0.0, 1.0, 0.0));
            Vec3d c = Transform3x3(this, new Vec3d(0.0, 0.0, -1));
            c.Normalize();
            c = e + c * lookDistance;

            // assign the results
            eye = e;
            center = c;
        }

        /// <summary>
        /// invert the matrix rhs, automatically select invert_4x3 or invert_4x4.
        /// </summary>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public bool Invert(Matrixd rhs)
        {
            bool is_4x3 = (rhs._mat[0, 3] == 0.0f && rhs._mat[1, 3] == 0.0f && rhs._mat[2, 3] == 0.0f && rhs._mat[3, 3] == 1.0f);
            return is_4x3 ? Invert_4x3(rhs) : Invert_4x4(rhs);
        }

        /// <summary>
        /// 4x3 matrix invert, not right hand column is assumed to be 0,0,0,1. 
        ///   Matrix inversion technique:
        /// Given a matrix mat, we want to invert it.
        ///mat = [ r00 r01 r02 a
        ///        r10 r11 r12 b
        ///        r20 r21 r22 c
        ///        tx  ty  tz  d ]
        /// We note that this matrix can be split into three matrices.
        /// mat = rot * trans * corr, where rot is rotation part, trans is translation part, and corr
        /// is the correction due to perspective (if any).
        ///rot = [ r00 r01 r02 0
        ///        r10 r11 r12 0
        ///        r20 r21 r22 0
        ///        0   0   0   1 ]
        ///trans = [ 1  0  0  0
        ///          0  1  0  0
        ///          0  0  1  0
        ///          tx ty tz 1 ]
        ///corr = [ 1 0 0 px
        ///         0 1 0 py
        ///         0 0 1 pz
        ///         0 0 0 s ]
        /// where the elements of corr are obtained from linear combinations of the elements of rot, trans, and mat.
        /// So the inverse is mat' = (trans * corr)' * rot', where rot' must be computed the traditional way, which 
        /// is easy since it is only a 3x3 matrix.
        /// This problem is simplified if [px py pz s] = [0 0 0 1], which will happen if mat was composed only of rotations,
        /// scales, and translations (which is common).  In this case, we can ignore corr entirely which saves on a lot of computations.
        ///
        /// </summary>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public bool Invert_4x3(Matrixd mat)
        {
            if (mat == this)
            {
                Matrixd tm = new Matrixd(mat);
                return Invert_4x3(tm);
            }

            value_type r00, r01, r02,
                                r10, r11, r12,
                                r20, r21, r22;
            // Copy rotation components directly into registers for speed
            r00 = mat._mat[0, 0]; r01 = mat._mat[0, 1]; r02 = mat._mat[0, 2];
            r10 = mat._mat[1, 0]; r11 = mat._mat[1, 1]; r12 = mat._mat[1, 2];
            r20 = mat._mat[2, 0]; r21 = mat._mat[2, 1]; r22 = mat._mat[2, 2];

            // Partially compute inverse of rot
            _mat[0, 0] = r11 * r22 - r12 * r21;
            _mat[0, 1] = r02 * r21 - r01 * r22;
            _mat[0, 2] = r01 * r12 - r02 * r11;

            // Compute determinant of rot from 3 elements just computed
            value_type one_over_det = 1.0 / (r00 * _mat[0, 0] + r10 * _mat[0, 1] + r20 * _mat[0, 2]);
            r00 *= one_over_det; r10 *= one_over_det; r20 *= one_over_det;  // Saves on later computations

            // Finish computing inverse of rot
            _mat[0, 0] *= one_over_det;
            _mat[0, 1] *= one_over_det;
            _mat[0, 2] *= one_over_det;
            _mat[0, 3] = 0.0;
            _mat[1, 0] = r12 * r20 - r10 * r22; // Have already been divided by det
            _mat[1, 1] = r00 * r22 - r02 * r20; // same
            _mat[1, 2] = r02 * r10 - r00 * r12; // same
            _mat[1, 3] = 0.0;
            _mat[2, 0] = r10 * r21 - r11 * r20; // Have already been divided by det
            _mat[2, 1] = r01 * r20 - r00 * r21; // same
            _mat[2, 2] = r00 * r11 - r01 * r10; // same
            _mat[2, 3] = 0.0;
            _mat[3, 3] = 1.0;

            // We no longer need the rxx or det variables anymore, so we can reuse them for whatever we want.  But we will still rename them for the sake of clarity.

            r22 = mat._mat[3, 3];

            if ((r22 - 1.0) * (r22 - 1.0) > 1.0e-6)  // Involves perspective, so we must
            {                       // compute the full inverse

                Matrixd TPinv = new Matrixd();
                _mat[3, 0] = _mat[3, 1] = _mat[3, 2] = 0.0f;

                r10 = mat._mat[0, 3]; r11 = mat._mat[1, 3]; r12 = mat._mat[2, 3];
                r00 = _mat[0, 0] * r10 + _mat[0, 1] * r11 + _mat[0, 2] * r12;
                r01 = _mat[1, 0] * r10 + _mat[1, 1] * r11 + _mat[1, 2] * r12;
                r02 = _mat[2, 0] * r10 + _mat[2, 1] * r11 + _mat[2, 2] * r12;


                r10 = mat._mat[3, 0]; r11 = mat._mat[3, 1]; r12 = mat._mat[3, 2];
                one_over_det = 1.0 / (r22 - (r10 * r00 + r11 * r01 + r12 * r02));

                r10 *= one_over_det; r11 *= one_over_det; r12 *= one_over_det;  // Reduces number of calculations later on

                // Compute inverse of trans*corr
                TPinv._mat[0, 0] = r10 * r00 + 1.0;
                TPinv._mat[0, 1] = r11 * r00;
                TPinv._mat[0, 2] = r12 * r00;
                TPinv._mat[0, 3] = -r00 * one_over_det;
                TPinv._mat[1, 0] = r10 * r01;
                TPinv._mat[1, 1] = r11 * r01 + 1.0;
                TPinv._mat[1, 2] = r12 * r01;
                TPinv._mat[1, 3] = -r01 * one_over_det;
                TPinv._mat[2, 0] = r10 * r02;
                TPinv._mat[2, 1] = r11 * r02;
                TPinv._mat[2, 2] = r12 * r02 + 1.0;
                TPinv._mat[2, 3] = -r02 * one_over_det;
                TPinv._mat[3, 0] = -r10;
                TPinv._mat[3, 1] = -r11;
                TPinv._mat[3, 2] = -r12;
                TPinv._mat[3, 3] = one_over_det;

                PreMult(TPinv); // Finish computing full inverse of mat

            }
            else // Rightmost column is [0; 0; 0; 1] so it can be ignored
            {
                r10 = mat._mat[3, 0]; r11 = mat._mat[3, 1]; r12 = mat._mat[3, 2];

                // Compute translation components of mat'
                _mat[3, 0] = -(r10 * _mat[0, 0] + r11 * _mat[1, 0] + r12 * _mat[2, 0]);
                _mat[3, 1] = -(r10 * _mat[0, 1] + r11 * _mat[1, 1] + r12 * _mat[2, 1]);
                _mat[3, 2] = -(r10 * _mat[0, 2] + r11 * _mat[1, 2] + r12 * _mat[2, 2]);
            }

            return true;
        }


        /** full 4x4 matrix invert. */
        public bool Invert_4x4(Matrixd mat)
        {
            if (mat == this)
            {
                Matrixd tm = new Matrixd(mat);
                return Invert_4x4(tm);
            }

            int[] indxc = new int[4], indxr = new int[4], ipiv = new int[4];
            int i, j, k, l, ll;
            int icol = 0;
            int irow = 0;
            double temp, pivinv, dum, big;

            // copy in place this may be unnecessary
            //*this = mat;

            for (j = 0; j < 4; j++) ipiv[j] = 0;

            for (i = 0; i < 4; i++)
            {
                big = 0.0;
                for (j = 0; j < 4; j++)
                    if (ipiv[j] != 1)
                        for (k = 0; k < 4; k++)
                        {
                            if (ipiv[k] == 0)
                            {
                                if (Math.Abs(this[j, k]) >= big)
                                {
                                    big = Math.Abs(this[j, k]);
                                    irow = j;
                                    icol = k;
                                }
                            }
                            else if (ipiv[k] > 1)
                                return false;
                        }
                ++(ipiv[icol]);
                if (irow != icol)
                    for (l = 0; l < 4; l++)
                    {
                        temp = this[irow, l];
                        this._mat[irow, l] = this[icol, l];
                        this._mat[icol, l] = temp;
                    }
                indxr[i] = irow;
                indxc[i] = icol;
                if (this[icol, icol] == 0)
                    return false;

                pivinv = 1.0 / this[icol, icol];
                this._mat[icol, icol] = 1;
                for (l = 0; l < 4; l++) this._mat[icol, l] *= pivinv;
                for (ll = 0; ll < 4; ll++)
                    if (ll != icol)
                    {
                        dum = this[ll, icol];
                        this._mat[ll, icol] = 0;
                        for (l = 0; l < 4; l++) this._mat[ll, l] -= this[icol, l] * dum;
                    }
            }
            for (int lx = 4; lx > 0; --lx)
            {
                if (indxr[lx - 1] != indxc[lx - 1])
                    for (k = 0; k < 4; k++)
                    {
                        temp = this[k, indxr[lx - 1]];
                        this._mat[k, indxr[lx - 1]] = this[k, indxc[lx - 1]];
                        this._mat[k, indxc[lx - 1]] = temp;
                    }
            }

            return true;
        }


        /** ortho-normalize the 3x3 rotation & scale matrix */
        public void OrthoNormalize(Matrixd rhs)
        {
            value_type x_colMag = (rhs._mat[0, 0] * rhs._mat[0, 0]) + (rhs._mat[1, 0] * rhs._mat[1, 0]) + (rhs._mat[2, 0] * rhs._mat[2, 0]);
            value_type y_colMag = (rhs._mat[0, 1] * rhs._mat[0, 1]) + (rhs._mat[1, 1] * rhs._mat[1, 1]) + (rhs._mat[2, 1] * rhs._mat[2, 1]);
            value_type z_colMag = (rhs._mat[0, 2] * rhs._mat[0, 2]) + (rhs._mat[1, 2] * rhs._mat[1, 2]) + (rhs._mat[2, 2] * rhs._mat[2, 2]);

            if (!Equivalent((double)x_colMag, 1.0) && !Equivalent((double)x_colMag, 0.0))
            {
                x_colMag = Math.Sqrt(x_colMag);
                _mat[0, 0] = rhs._mat[0, 0] / x_colMag;
                _mat[1, 0] = rhs._mat[1, 0] / x_colMag;
                _mat[2, 0] = rhs._mat[2, 0] / x_colMag;
            }
            else
            {
                _mat[0, 0] = rhs._mat[0, 0];
                _mat[1, 0] = rhs._mat[1, 0];
                _mat[2, 0] = rhs._mat[2, 0];
            }

            if (!Equivalent((double)y_colMag, 1.0) && !Equivalent((double)y_colMag, 0.0))
            {
                y_colMag = Math.Sqrt(y_colMag);
                _mat[0, 1] = rhs._mat[0, 1] / y_colMag;
                _mat[1, 1] = rhs._mat[1, 1] / y_colMag;
                _mat[2, 1] = rhs._mat[2, 1] / y_colMag;
            }
            else
            {
                _mat[0, 1] = rhs._mat[0, 1];
                _mat[1, 1] = rhs._mat[1, 1];
                _mat[2, 1] = rhs._mat[2, 1];
            }

            if (!Equivalent((double)z_colMag, 1.0) && !Equivalent((double)z_colMag, 0.0))
            {
                z_colMag = Math.Sqrt(z_colMag);
                _mat[0, 2] = rhs._mat[0, 2] / z_colMag;
                _mat[1, 2] = rhs._mat[1, 2] / z_colMag;
                _mat[2, 2] = rhs._mat[2, 2] / z_colMag;
            }
            else
            {
                _mat[0, 2] = rhs._mat[0, 2];
                _mat[1, 2] = rhs._mat[1, 2];
                _mat[2, 2] = rhs._mat[2, 2];
            }

            _mat[3, 0] = rhs._mat[3, 0];
            _mat[3, 1] = rhs._mat[3, 1];
            _mat[3, 2] = rhs._mat[3, 2];

            _mat[0, 3] = rhs._mat[0, 3];
            _mat[1, 3] = rhs._mat[1, 3];
            _mat[2, 3] = rhs._mat[2, 3];
            _mat[3, 3] = rhs._mat[3, 3];

        }

        //basic utility functions to create new matrices
        public static Matrixd Identity
        {
            get
            {
                Matrixd m = new Matrixd();
                m.MakeIdentity();
                return m;
            }
        }
        public static Matrixd Scale(Vec3f sv)
        {
            return Scale(sv.X, sv.Y, sv.Z);
        }
        public static Matrixd Scale(Vec3d sv)
        {
            return Scale(sv.X, sv.Y, sv.Z);
        }
        public static Matrixd Scale(value_type sx, value_type sy, value_type sz)
        {
            Matrixd m = new Matrixd();
            m.MakeScale(sx, sy, sz);
            return m;
        }
        public static Matrixd Translate(Vec3f v)
        {
            return Translate(v.X, v.Y, v.Z);
        }
        public static Matrixd Translate(Vec3d v)
        {
            return Translate(v.X, v.Y, v.Z);
        }
        public static Matrixd Translate(value_type tx, value_type ty, value_type tz)
        {
            Matrixd m = new Matrixd();
            m.MakeTranslate(tx, ty, tz);
            return m;
        }
        public static Matrixd Rotate(Vec3f from, Vec3f to)
        {
            Matrixd m = new Matrixd();
            m.MakeRotate(from, to);
            return m;
        }
        public static Matrixd Rotate(Vec3d from, Vec3d to)
        {
            Matrixd m = new Matrixd();
            m.MakeRotate(from, to);
            return m;
        }
        public static Matrixd Rotate(value_type angle, value_type x, value_type y, value_type z)
        {
            Matrixd m = new Matrixd();
            m.MakeRotate(angle, x, y, z);
            return m;
        }

        public static Matrixd Rotate(value_type angle, Vec3f axis)
        {
            Matrixd m = new Matrixd();
            m.MakeRotate(angle, axis);
            return m;
        }
        public static Matrixd Rotate(value_type angle, Vec3d axis)
        {
            Matrixd m = new Matrixd();
            m.MakeRotate(angle, axis);
            return m;
        }

        public static Matrixd Rotate(value_type angle1, Vec3f axis1,
                                      value_type angle2, Vec3f axis2,
                                      value_type angle3, Vec3f axis3)
        {
            Matrixd m = new Matrixd();
            m.MakeRotate(angle1, axis1, angle2, axis2, angle3, axis3);
            return m;
        }

        public static Matrixd Rotate(value_type angle1, Vec3d axis1,
                                      value_type angle2, Vec3d axis2,
                                      value_type angle3, Vec3d axis3)
        {
            Matrixd m = new Matrixd();
            m.MakeRotate(angle1, axis1, angle2, axis2, angle3, axis3);
            return m;
        }
        public static Matrixd Rotate(Quat quat)
        {
            return new Matrixd(quat);
        }
        public static Matrixd Inverse(Matrixd matrix)
        {
            Matrixd m = new Matrixd();
            m.Invert(matrix);
            return m;
        }
        public static Matrixd OrthoNormal(Matrixd matrix)
        {
            Matrixd m = new Matrixd();
            m.OrthoNormalize(matrix);
            return m;
        }

        /** Create an orthographic projection matrix.
          * See glOrtho for further details.
        */
        public static Matrixd Ortho(double left, double right,
                                    double bottom, double top,
                                    double zNear, double zFar)
        {
            Matrixd m = new Matrixd();
            m.MakeOrtho(left, right, bottom, top, zNear, zFar);
            return m;
        }

        /** Create a 2D orthographic projection.
          * See glOrtho for further details.
        */
        public static Matrixd Ortho2D(double left, double right,
                                      double bottom, double top)
        {
            Matrixd m = new Matrixd();
            m.MakeOrtho2D(left, right, bottom, top);
            return m;
        }

        /** Create a perspective projection.
          * See glFrustum for further details.
        */
        public static Matrixd Frustum(double left, double right,
                                      double bottom, double top,
                                      double zNear, double zFar)
        {
            Matrixd m = new Matrixd();
            m.MakeFrustum(left, right, bottom, top, zNear, zFar);
            return m;
        }

        /** Create a symmetrical perspective projection.
          * See gluPerspective for further details.
          * Aspect ratio is defined as width/height.
        */
        public static Matrixd Perspective(double fovy, double aspectRatio,
                                          double zNear, double zFar)
        {
            Matrixd m = new Matrixd();
            m.MakePerspective(fovy, aspectRatio, zNear, zFar);
            return m;
        }

        /** Create the position and orientation as per a camera,
          * using the same convention as gluLookAt.
        */
        public static Matrixd LookAt(Vec3f eye,
                                       Vec3f center,
                                       Vec3f up)
        {
            Matrixd m = new Matrixd();
            m.MakeLookAt(eye, center, up);
            return m;
        }

        /** Create the position and orientation as per a camera,
          * using the same convention as gluLookAt.
        */
        public static Matrixd LookAt(Vec3d eye,
                                       Vec3d center,
                                       Vec3d up)
        {
            Matrixd m = new Matrixd();
            m.MakeLookAt(eye, center, up);
            return m;
        }

        public Vec3f PreMult(Vec3f v)
        {
            value_type d = 1.0f / (_mat[0, 3] * v.X + _mat[1, 3] * v.Y + _mat[2, 3] * v.Z + _mat[3, 3]);
            return new Vec3f((float)((_mat[0, 0] * v.X + _mat[1, 0] * v.Y + _mat[2, 0] * v.Z + _mat[3, 0]) * d),
                (float)((_mat[0, 1] * v.X + _mat[1, 1] * v.Y + _mat[2, 1] * v.Z + _mat[3, 1]) * d),
                (float)((_mat[0, 2] * v.X + _mat[1, 2] * v.Y + _mat[2, 2] * v.Z + _mat[3, 2]) * d));
        }

        public Vec3d PreMult(Vec3d v)
        {
            value_type d = 1.0f / (_mat[0, 3] * v.X + _mat[1, 3] * v.Y + _mat[2, 3] * v.Z + _mat[3, 3]);
            return new Vec3d((_mat[0, 0] * v.X + _mat[1, 0] * v.Y + _mat[2, 0] * v.Z + _mat[3, 0]) * d,
                (_mat[0, 1] * v.X + _mat[1, 1] * v.Y + _mat[2, 1] * v.Z + _mat[3, 1]) * d,
                (_mat[0, 2] * v.X + _mat[1, 2] * v.Y + _mat[2, 2] * v.Z + _mat[3, 2]) * d);
        }

        public Vec3f PostMult(Vec3f v)
        {
            value_type d = 1.0f / (_mat[3, 0] * v.X + _mat[3, 1] * v.Y + _mat[3, 2] * v.Z + _mat[3, 3]);
            return new Vec3f((float)((_mat[0, 0] * v.X + _mat[0, 1] * v.Y + _mat[0, 2] * v.Z + _mat[0, 3]) * d),
                (float)((_mat[1, 0] * v.X + _mat[1, 1] * v.Y + _mat[1, 2] * v.Z + _mat[1, 3]) * d),
               (float)((_mat[2, 0] * v.X + _mat[2, 1] * v.Y + _mat[2, 2] * v.Z + _mat[2, 3]) * d));
        }

        public Vec3d PostMult(Vec3d v)
        {
            value_type d = 1.0f / (_mat[3, 0] * v.X + _mat[3, 1] * v.Y + _mat[3, 2] * v.Z + _mat[3, 3]);
            return new Vec3d((_mat[0, 0] * v.X + _mat[0, 1] * v.Y + _mat[0, 2] * v.Z + _mat[0, 3]) * d,
                (_mat[1, 0] * v.X + _mat[1, 1] * v.Y + _mat[1, 2] * v.Z + _mat[1, 3]) * d,
                (_mat[2, 0] * v.X + _mat[2, 1] * v.Y + _mat[2, 2] * v.Z + _mat[2, 3]) * d);
        }
        public static Vec3f operator *(Vec3f v, Matrixd m)
        {
            return m.PreMult(v);
        }
        public static Vec3d operator *(Vec3d v, Matrixd m)
        {
            return m.PreMult(v);
        }
        public static Vec4f operator *(Vec4f v, Matrixd m)
        {
            return m.PreMult(v);
        }

        public static Vec4d operator *(Vec4d v, Matrixd m)
        {
            return m.PreMult(v);
        }

        public static Vec3f operator *(Matrixd lhs, Vec3f v)
        {
            return lhs.PostMult(v);
        }
        public static Vec3d operator *(Matrixd lhs, Vec3d v)
        {
            return lhs.PostMult(v);
        }
        public static Vec4f operator *(Matrixd lhs, Vec4f v)
        {
            return lhs.PostMult(v);
        }
        public static Vec4d operator *(Matrixd lhs, Vec4d v)
        {
            return lhs.PostMult(v);
        }


        public Vec4f PreMult(Vec4f v)
        {
            return new Vec4f((float)((_mat[0, 0] * v.X + _mat[1, 0] * v.Y + _mat[2, 0] * v.Z + _mat[3, 0] * v.W)),
               (float)((_mat[0, 1] * v.X + _mat[1, 1] * v.Y + _mat[2, 1] * v.Z + _mat[3, 1] * v.W)),
                (float)((_mat[0, 2] * v.X + _mat[1, 2] * v.Y + _mat[2, 2] * v.Z + _mat[3, 2] * v.W)),
               (float)((_mat[0, 3] * v.X + _mat[1, 3] * v.Y + _mat[2, 3] * v.Z + _mat[3, 3] * v.W)));
        }
        public Vec4d PreMult(Vec4d v)
        {
            return new Vec4d((_mat[0, 0] * v.X + _mat[1, 0] * v.Y + _mat[2, 0] * v.Z + _mat[3, 0] * v.W),
                (_mat[0, 1] * v.X + _mat[1, 1] * v.Y + _mat[2, 1] * v.Z + _mat[3, 1] * v.W),
                (_mat[0, 2] * v.X + _mat[1, 2] * v.Y + _mat[2, 2] * v.Z + _mat[3, 2] * v.W),
                (_mat[0, 3] * v.X + _mat[1, 3] * v.Y + _mat[2, 3] * v.Z + _mat[3, 3] * v.W));
        }
        public Vec4f PostMult(Vec4f v)
        {
            return new Vec4f((float)((_mat[0, 0] * v.X + _mat[0, 1] * v.Y + _mat[0, 2] * v.Z + _mat[0, 3] * v.W)),
                (float)((_mat[1, 0] * v.X + _mat[1, 1] * v.Y + _mat[1, 2] * v.Z + _mat[1, 3] * v.W)),
                (float)((_mat[2, 0] * v.X + _mat[2, 1] * v.Y + _mat[2, 2] * v.Z + _mat[2, 3] * v.W)),
                (float)((_mat[3, 0] * v.X + _mat[3, 1] * v.Y + _mat[3, 2] * v.Z + _mat[3, 3] * v.W)));
        }
        public Vec4d PostMult(Vec4d v)
        {
            return new Vec4d((_mat[0, 0] * v.X + _mat[0, 1] * v.Y + _mat[0, 2] * v.Z + _mat[0, 3] * v.W),
                (_mat[1, 0] * v.X + _mat[1, 1] * v.Y + _mat[1, 2] * v.Z + _mat[1, 3] * v.W),
                (_mat[2, 0] * v.X + _mat[2, 1] * v.Y + _mat[2, 2] * v.Z + _mat[2, 3] * v.W),
                (_mat[3, 0] * v.X + _mat[3, 1] * v.Y + _mat[3, 2] * v.Z + _mat[3, 3] * v.W));
        }
       

        public void SetRotate(Quat q)
        {
            double length2 = q.Length2;
            if (Math.Abs(length2) <= double.MinValue)
            {
                _mat[0, 0] = 0.0; _mat[1, 0] = 0.0; _mat[2, 0] = 0.0;
                _mat[0, 1] = 0.0; _mat[1, 1] = 0.0; _mat[2, 1] = 0.0;
                _mat[0, 2] = 0.0; _mat[1, 2] = 0.0; _mat[2, 2] = 0.0;
            }
            else
            {
                double rlength2;
                // normalize quat if required.
                // We can avoid the expensive sqrt in this case since all 'coefficients' below are products of two q components.
                // That is a square of a square root, so it is possible to avoid that
                if (length2 != 1.0)
                {
                    rlength2 = 2.0 / length2;
                }
                else
                {
                    rlength2 = 2.0;
                }

                // Source: Gamasutra, Rotating Objects Using Quaternions
                //
                //http://www.gamasutra.com/features/19980703/quaternions_01.htm

                double wx, wy, wz, xx, yy, yz, xy, xz, zz, x2, y2, z2;

                // calculate coefficients
                x2 = rlength2 * q._v[0];
                y2 = rlength2 * q._v[1];
                z2 = rlength2 * q._v[2];

                xx = q._v[0] * x2;
                xy = q._v[0] * y2;
                xz = q._v[0] * z2;

                yy = q._v[1] * y2;
                yz = q._v[1] * z2;
                zz = q._v[2] * z2;

                wx = q._v[3] * x2;
                wy = q._v[3] * y2;
                wz = q._v[3] * z2;

                // Note.  Gamasutra gets the matrix assignments inverted, resulting
                // in left-handed rotations, which is contrary to OpenGL and OSG's
                // methodology.  The matrix assignment has been altered in the next
                // few lines of code to do the right thing.
                // Don Burns - Oct 13, 2001
                _mat[0, 0] = 1.0 - (yy + zz);
                _mat[1, 0] = xy - wz;
                _mat[2, 0] = xz + wy;


                _mat[0, 1] = xy + wz;
                _mat[1, 1] = 1.0 - (xx + zz);
                _mat[2, 1] = yz - wx;

                _mat[0, 2] = xz - wy;
                _mat[1, 2] = yz + wx;
                _mat[2, 2] = 1.0 - (xx + yy);
            }
        }

        /** Get the matrix rotation as a Quat. Note that this function
          * assumes a non-scaled matrix and will return incorrect results
          * for scaled matrixces. Consider decompose() instead.
          */
        public Quat GetRotate()
        {
            Quat q = new Quat();

            value_type s;
            value_type[] tq = new value_type[4];
            int i, j;

            // Use tq to store the largest trace
            tq[0] = 1 + _mat[0, 0] + _mat[1, 1] + _mat[2, 2];
            tq[1] = 1 + _mat[0, 0] - _mat[1, 1] - _mat[2, 2];
            tq[2] = 1 - _mat[0, 0] + _mat[1, 1] - _mat[2, 2];
            tq[3] = 1 - _mat[0, 0] - _mat[1, 1] + _mat[2, 2];

            // Find the maximum (could also use stacked if's later)
            j = 0;
            for (i = 1; i < 4; i++) j = (tq[i] > tq[j]) ? i : j;

            // check the diagonal
            if (j == 0)
            {
                /* perform instant calculation */
                q._v[3] = tq[0];
                q._v[0] = _mat[1, 2] - _mat[2, 1];
                q._v[1] = _mat[2, 0] - _mat[0, 2];
                q._v[2] = _mat[0, 1] - _mat[1, 0];
            }
            else if (j == 1)
            {
                q._v[3] = _mat[1, 2] - _mat[2, 1];
                q._v[0] = tq[1];
                q._v[1] = _mat[0, 1] + _mat[1, 0];
                q._v[2] = _mat[2, 0] + _mat[0, 2];
            }
            else if (j == 2)
            {
                q._v[3] = _mat[2, 0] - _mat[0, 2];
                q._v[0] = _mat[0, 1] + _mat[1, 0];
                q._v[1] = tq[2];
                q._v[2] = _mat[1, 2] + _mat[2, 1];
            }
            else /* if (j==3) */
            {
                q._v[3] = _mat[0, 1] - _mat[1, 0];
                q._v[0] = _mat[2, 0] + _mat[0, 2];
                q._v[1] = _mat[1, 2] + _mat[2, 1];
                q._v[2] = tq[3];
            }

            s = Math.Sqrt(0.25 / tq[j]);
            q._v[3] *= s;
            q._v[0] *= s;
            q._v[1] *= s;
            q._v[2] *= s;

            return q;

        }


        public void SetTrans(value_type tx, value_type ty, value_type tz)
        {
            _mat[3, 0] = tx;
            _mat[3, 1] = ty;
            _mat[3, 2] = tz;
        }

        public void SetTrans(Vec3f v)
        {
            _mat[3, 0] = v[0];
            _mat[3, 1] = v[1];
            _mat[3, 2] = v[2];
        }

        public void SetTrans(Vec3d v)
        {
            _mat[3, 0] = v[0];
            _mat[3, 1] = v[1];
            _mat[3, 2] = v[2];
        }

        public Vec3d GetTrans() { return new Vec3d(_mat[3, 0], _mat[3, 1], _mat[3, 2]); }

        public Vec3d GetScale()
        {
            Vec3d x_vec = new Vec3d(_mat[0, 0], _mat[1, 0], _mat[2, 0]);
            Vec3d y_vec = new Vec3d(_mat[0, 1], _mat[1, 1], _mat[2, 1]);
            Vec3d z_vec = new Vec3d(_mat[0, 2], _mat[1, 2], _mat[2, 2]);
            return new Vec3d(x_vec.Length, y_vec.Length, z_vec.Length);
        }

        /** apply a 3x3 transform of v*M[0..2,0..2]. */
        public static Vec3f Transform3x3(Vec3f v, Matrixd m)
        {
            return new Vec3f((float)((m._mat[0, 0] * v.X + m._mat[1, 0] * v.Y + m._mat[2, 0] * v.Z)),
                         (float)((m._mat[0, 1] * v.X + m._mat[1, 1] * v.Y + m._mat[2, 1] * v.Z)),
                         (float)((m._mat[0, 2] * v.X + m._mat[1, 2] * v.Y + m._mat[2, 2] * v.Z)));
        }

        /** apply a 3x3 transform of v*M[0..2,0..2]. */
        public static Vec3d Transform3x3(Vec3d v, Matrixd m)
        {
            return new Vec3d((m._mat[0, 0] * v.X + m._mat[1, 0] * v.Y + m._mat[2, 0] * v.Z),
                         (m._mat[0, 1] * v.X + m._mat[1, 1] * v.Y + m._mat[2, 1] * v.Z),
                         (m._mat[0, 2] * v.X + m._mat[1, 2] * v.Y + m._mat[2, 2] * v.Z));
        }

        /** apply a 3x3 transform of M[0..2,0..2]*v. */
        public static Vec3f Transform3x3(Matrixd m, Vec3f v)
        {
            return new Vec3f((float)((m._mat[0, 0] * v.X + m._mat[0, 1] * v.Y + m._mat[0, 2] * v.Z)),
                         (float)((m._mat[1, 0] * v.X + m._mat[1, 1] * v.Y + m._mat[1, 2] * v.Z)),
                         (float)((m._mat[2, 0] * v.X + m._mat[2, 1] * v.Y + m._mat[2, 2] * v.Z)));
        }

        /** apply a 3x3 transform of M[0..2,0..2]*v. */
        public static Vec3d Transform3x3(Matrixd m, Vec3d v)
        {
            return new Vec3d((m._mat[0, 0] * v.X + m._mat[0, 1] * v.Y + m._mat[0, 2] * v.Z),
                         (m._mat[1, 0] * v.X + m._mat[1, 1] * v.Y + m._mat[1, 2] * v.Z),
                         (m._mat[2, 0] * v.X + m._mat[2, 1] * v.Y + m._mat[2, 2] * v.Z));
        }

        // basic Matrixd multiplication, our workhorse methods.
        public void Mult(Matrixd lhs, Matrixd rhs)
        {
            if (lhs == this)
            {
                PostMult(rhs);
                return;
            }
            if (rhs == this)
            {
                PreMult(lhs);
                return;
            }

            // PRECONDITION: We assume neither &lhs nor &rhs == this
            // if it did, use preMult or postMult instead
            _mat[0, 0] = InnerProduct(lhs, rhs, 0, 0);
            _mat[0, 1] = InnerProduct(lhs, rhs, 0, 1);
            _mat[0, 2] = InnerProduct(lhs, rhs, 0, 2);
            _mat[0, 3] = InnerProduct(lhs, rhs, 0, 3);
            _mat[1, 0] = InnerProduct(lhs, rhs, 1, 0);
            _mat[1, 1] = InnerProduct(lhs, rhs, 1, 1);
            _mat[1, 2] = InnerProduct(lhs, rhs, 1, 2);
            _mat[1, 3] = InnerProduct(lhs, rhs, 1, 3);
            _mat[2, 0] = InnerProduct(lhs, rhs, 2, 0);
            _mat[2, 1] = InnerProduct(lhs, rhs, 2, 1);
            _mat[2, 2] = InnerProduct(lhs, rhs, 2, 2);
            _mat[2, 3] = InnerProduct(lhs, rhs, 2, 3);
            _mat[3, 0] = InnerProduct(lhs, rhs, 3, 0);
            _mat[3, 1] = InnerProduct(lhs, rhs, 3, 1);
            _mat[3, 2] = InnerProduct(lhs, rhs, 3, 2);
            _mat[3, 3] = InnerProduct(lhs, rhs, 3, 3);
        }

        public void PreMult(Matrixd other)
        {
            // brute force method requiring a copy
            //Matrix_implementation tmp(other* *this);
            // *this = tmp;

            // more efficient method just use a value_type[4] for temporary storage.
            value_type[] t = new value_type[4];
            for (int col = 0; col < 4; ++col)
            {
                t[0] = InnerProduct(other, this, 0, col);
                t[1] = InnerProduct(other, this, 1, col);
                t[2] = InnerProduct(other, this, 2, col);
                t[3] = InnerProduct(other, this, 3, col);
                _mat[0, col] = t[0];
                _mat[1, col] = t[1];
                _mat[2, col] = t[2];
                _mat[3, col] = t[3];
            }

        }

        public void PostMult(Matrixd other)
        {
            // brute force method requiring a copy
            //Matrix_implementation tmp(*this * other);
            // *this = tmp;

            // more efficient method just use a value_type[4] for temporary storage.
            value_type[] t = new value_type[4];
            for (int row = 0; row < 4; ++row)
            {
                t[0] = InnerProduct(this, other, row, 0);
                t[1] = InnerProduct(this, other, row, 1);
                t[2] = InnerProduct(this, other, row, 2);
                t[3] = InnerProduct(this, other, row, 3);
                SetRow(row, t[0], t[1], t[2], t[3]);
            }
        }

        /** Optimized version of preMult(translate(v)); */
        public void PreMultTranslate(Vec3d v)
        {
            for (int i = 0; i < 3; ++i)
            {
                double tmp = v[i];
                if (tmp == 0)
                    continue;
                _mat[3, 0] += tmp * _mat[i, 0];
                _mat[3, 1] += tmp * _mat[i, 1];
                _mat[3, 2] += tmp * _mat[i, 2];
                _mat[3, 3] += tmp * _mat[i, 3];
            }
        }
        public void PreMultTranslate(Vec3f v)
        {
            for (int i = 0; i < 3; ++i)
            {
                float tmp = v[i];
                if (tmp == 0)
                    continue;
                _mat[3, 0] += tmp * _mat[i, 0];
                _mat[3, 1] += tmp * _mat[i, 1];
                _mat[3, 2] += tmp * _mat[i, 2];
                _mat[3, 3] += tmp * _mat[i, 3];
            }
        }
        /** Optimized version of postMult(translate(v)); */
        public void PostMultTranslate(Vec3d v)
        {
            for (int i = 0; i < 3; ++i)
            {
                double tmp = v[i];
                if (tmp == 0)
                    continue;
                _mat[0, i] += tmp * _mat[0, 3];
                _mat[1, i] += tmp * _mat[1, 3];
                _mat[2, i] += tmp * _mat[2, 3];
                _mat[3, i] += tmp * _mat[3, 3];
            }
        }
        public void PostMultTranslate(Vec3f v)
        {
            for (int i = 0; i < 3; ++i)
            {
                float tmp = v[i];
                if (tmp == 0)
                    continue;
                _mat[0, i] += tmp * _mat[0, 3];
                _mat[1, i] += tmp * _mat[1, 3];
                _mat[2, i] += tmp * _mat[2, 3];
                _mat[3, i] += tmp * _mat[3, 3];
            }
        }

        /** Optimized version of preMult(scale(v)); */
        public void PreMultScale(Vec3d v)
        {
            _mat[0, 0] *= v[0]; _mat[0, 1] *= v[0]; _mat[0, 2] *= v[0]; _mat[0, 3] *= v[0];
            _mat[1, 0] *= v[1]; _mat[1, 1] *= v[1]; _mat[1, 2] *= v[1]; _mat[1, 3] *= v[1];
            _mat[2, 0] *= v[2]; _mat[2, 1] *= v[2]; _mat[2, 2] *= v[2]; _mat[2, 3] *= v[2];
        }

        public void PreMultScale(Vec3f v)
        {
            _mat[0, 0] *= v[0]; _mat[0, 1] *= v[0]; _mat[0, 2] *= v[0]; _mat[0, 3] *= v[0];
            _mat[1, 0] *= v[1]; _mat[1, 1] *= v[1]; _mat[1, 2] *= v[1]; _mat[1, 3] *= v[1];
            _mat[2, 0] *= v[2]; _mat[2, 1] *= v[2]; _mat[2, 2] *= v[2]; _mat[2, 3] *= v[2];
        }

        /** Optimized version of postMult(scale(v)); */
        public void PostMultScale(Vec3d v)
        {
            _mat[0, 0] *= v[0]; _mat[1, 0] *= v[0]; _mat[2, 0] *= v[0]; _mat[3, 0] *= v[0];
            _mat[0, 1] *= v[1]; _mat[1, 1] *= v[1]; _mat[2, 1] *= v[1]; _mat[3, 1] *= v[1];
            _mat[0, 2] *= v[2]; _mat[1, 2] *= v[2]; _mat[2, 2] *= v[2]; _mat[3, 2] *= v[2];
        }

        public void PostMultScale(Vec3f v)
        {
            _mat[0, 0] *= v[0]; _mat[1, 0] *= v[0]; _mat[2, 0] *= v[0]; _mat[3, 0] *= v[0];
            _mat[0, 1] *= v[1]; _mat[1, 1] *= v[1]; _mat[2, 1] *= v[1]; _mat[3, 1] *= v[1];
            _mat[0, 2] *= v[2]; _mat[1, 2] *= v[2]; _mat[2, 2] *= v[2]; _mat[3, 2] *= v[2];
        }

        /** Optimized version of preMult(rotate(q)); */
        public void PreMultRotate(Quat q)
        {
            if (q.ZeroRotation())
                return;
            Matrixd r = new Matrixd();
            r.SetRotate(q);
            PreMult(r);
        }
        /** Optimized version of postMult(rotate(q)); */
        public void PostMultRotate(Quat q)
        {
            if (q.ZeroRotation())
                return;
            Matrixd r = new Matrixd();
            r.SetRotate(q);
            PostMult(r);
        }

        public void Mult(Matrixd other)
        {
            if (this == other)
            {
                Matrixd temp = new Matrixd(other);
                PostMult(temp);
            }
            else PostMult(other);
        }

        public static Matrixd operator *(Matrixd l, Matrixd r)
        {
            r.Mult(l, r);
            return r;
        }

        /* Multiply by scalar. */
        public static Matrixd operator *(Matrixd lhs, value_type rhs)
        {
            return new Matrixd(
                lhs._mat[0, 0] * rhs, lhs._mat[0, 1] * rhs, lhs._mat[0, 2] * rhs, lhs._mat[0, 3] * rhs,
                lhs._mat[1, 0] * rhs, lhs._mat[1, 1] * rhs, lhs._mat[1, 2] * rhs, lhs._mat[1, 3] * rhs,
                lhs._mat[2, 0] * rhs, lhs._mat[2, 1] * rhs, lhs._mat[2, 2] * rhs, lhs._mat[2, 3] * rhs,
                lhs._mat[3, 0] * rhs, lhs._mat[3, 1] * rhs, lhs._mat[3, 2] * rhs, lhs._mat[3, 3] * rhs);
        }

        /* Unary multiply by scalar. */
        public Matrixd Mult(value_type rhs)
        {
            _mat[0, 0] *= rhs;
            _mat[0, 1] *= rhs;
            _mat[0, 2] *= rhs;
            _mat[0, 3] *= rhs;
            _mat[1, 0] *= rhs;
            _mat[1, 1] *= rhs;
            _mat[1, 2] *= rhs;
            _mat[1, 3] *= rhs;
            _mat[2, 0] *= rhs;
            _mat[2, 1] *= rhs;
            _mat[2, 2] *= rhs;
            _mat[2, 3] *= rhs;
            _mat[3, 0] *= rhs;
            _mat[3, 1] *= rhs;
            _mat[3, 2] *= rhs;
            _mat[3, 3] *= rhs;
            return this;
        }

        /* Divide by scalar. */
        public static Matrixd operator /(Matrixd lhs, value_type rhs)
        {
            return new Matrixd(
               lhs._mat[0, 0] / rhs, lhs._mat[0, 1] / rhs, lhs._mat[0, 2] / rhs, lhs._mat[0, 3] / rhs,
                lhs._mat[1, 0] / rhs, lhs._mat[1, 1] / rhs, lhs._mat[1, 2] / rhs, lhs._mat[1, 3] / rhs,
                lhs._mat[2, 0] / rhs, lhs._mat[2, 1] / rhs, lhs._mat[2, 2] / rhs, lhs._mat[2, 3] / rhs,
                lhs._mat[3, 0] / rhs, lhs._mat[3, 1] / rhs, lhs._mat[3, 2] / rhs, lhs._mat[3, 3] / rhs);
        }

        /* Unary divide by scalar. */
        public Matrixd Div(value_type rhs)
        {
            _mat[0, 0] /= rhs;
            _mat[0, 1] /= rhs;
            _mat[0, 2] /= rhs;
            _mat[0, 3] /= rhs;
            _mat[1, 0] /= rhs;
            _mat[1, 1] /= rhs;
            _mat[1, 2] /= rhs;
            _mat[1, 3] /= rhs;
            _mat[2, 0] /= rhs;
            _mat[2, 1] /= rhs;
            _mat[2, 2] /= rhs;
            _mat[2, 3] /= rhs;
            _mat[3, 0] /= rhs;
            _mat[3, 1] /= rhs;
            _mat[3, 2] /= rhs;
            _mat[3, 3] /= rhs;
            return this;
        }

        /** Binary vector add. */
        public static Matrixd operator +(Matrixd lhs, Matrixd rhs)
        {
            return new Matrixd(
                lhs._mat[0, 0] + rhs._mat[0, 0],
                lhs._mat[0, 1] + rhs._mat[0, 1],
                lhs._mat[0, 2] + rhs._mat[0, 2],
                lhs._mat[0, 3] + rhs._mat[0, 3],
                lhs._mat[1, 0] + rhs._mat[1, 0],
                lhs._mat[1, 1] + rhs._mat[1, 1],
                lhs._mat[1, 2] + rhs._mat[1, 2],
                lhs._mat[1, 3] + rhs._mat[1, 3],
                lhs._mat[2, 0] + rhs._mat[2, 0],
                lhs._mat[2, 1] + rhs._mat[2, 1],
                lhs._mat[2, 2] + rhs._mat[2, 2],
                lhs._mat[2, 3] + rhs._mat[2, 3],
                lhs._mat[3, 0] + rhs._mat[3, 0],
                lhs._mat[3, 1] + rhs._mat[3, 1],
                lhs._mat[3, 2] + rhs._mat[3, 2],
                lhs._mat[3, 3] + rhs._mat[3, 3]);
        }

        /* Unary vector add. Slightly more efficient because no temporary
        * intermediate object.
        */
        public Matrixd Add(Matrixd rhs)
        {
            _mat[0, 0] += rhs._mat[0, 0];
            _mat[0, 1] += rhs._mat[0, 1];
            _mat[0, 2] += rhs._mat[0, 2];
            _mat[0, 3] += rhs._mat[0, 3];
            _mat[1, 0] += rhs._mat[1, 0];
            _mat[1, 1] += rhs._mat[1, 1];
            _mat[1, 2] += rhs._mat[1, 2];
            _mat[1, 3] += rhs._mat[1, 3];
            _mat[2, 0] += rhs._mat[2, 0];
            _mat[2, 1] += rhs._mat[2, 1];
            _mat[2, 2] += rhs._mat[2, 2];
            _mat[2, 3] += rhs._mat[2, 3];
            _mat[3, 0] += rhs._mat[3, 0];
            _mat[3, 1] += rhs._mat[3, 1];
            _mat[3, 2] += rhs._mat[3, 2];
            _mat[3, 3] += rhs._mat[3, 3];
            return this;
        }

        private void SetRow(int row, value_type v1, value_type v2, value_type v3, value_type v4)
        {
            _mat[row, 0] = v1;
            _mat[row, 1] = v2;
            _mat[row, 2] = v3;
            _mat[row, 3] = v4;
        }

        private value_type InnerProduct(Matrixd a, Matrixd b, int r, int c)
        {
            return (a._mat[r, 0] * b._mat[0, c])
           + (a._mat[r, 1] * b._mat[1, c])
           + (a._mat[r, 2] * b._mat[2, c])
           + (a._mat[r, 3] * b._mat[3, c]);
        }

        /** return true if double lhs and rhs are equivalent,
  * meaning that the difference between them is less than an epsilon value
  * which defaults to 1e-6.
*/
        public bool Equivalent(double lhs, double rhs, double epsilon = 1e-6)
        {
            double delta = rhs - lhs;
            return delta < 0.0 ? delta >= -epsilon : delta <= epsilon;
        }


        protected value_type[,] _mat = new value_type[4, 4];

    }
}
