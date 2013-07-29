using FalconNet.Common.Graphics;
using System;

namespace FalconNet.Graphics
{
    public class Matrix
    {
        // Handy constants to have available
        public static readonly Tpoint Origin = new Tpoint { x = 0.0f, y = 0.0f, z = 0.0f };
        public static readonly Trotation IMatrix = new Trotation
        {
            M11 = 1.0f,
            M12 = 0.0f,
            M13 = 0.0f,
            M21 = 0.0f,
            M22 = 1.0f,
            M23 = 0.0f,
            M31 = 0.0f,
            M32 = 0.0f,
            M33 = 1.0f
        };
        /***************************************************************************\
    Initialize the contents of a matrix provided by the caller
\***************************************************************************/
        public static void MatrixLoad(ref Trotation T, float a11, float a12, float a13,
                                        float a21, float a22, float a23,
                                        float a31, float a32, float a33)
        {
            T.M11 = a11; T.M12 = a12; T.M13 = a13;
            T.M21 = a21; T.M22 = a22; T.M23 = a23;
            T.M31 = a31; T.M32 = a32; T.M33 = a33;
        }

        /***************************************************************************\
            Swap the rows and columns of the matrix
        \***************************************************************************/
        public static void MatrixTranspose(Trotation Src, ref Trotation T)
        {
            T.M11 = Src.M11; T.M12 = Src.M21; T.M13 = Src.M31;
            T.M21 = Src.M12; T.M22 = Src.M22; T.M23 = Src.M32;
            T.M31 = Src.M13; T.M32 = Src.M23; T.M33 = Src.M33;
        }

        /***************************************************************************\
            Multiply the two provided matricies and store the result in the target
        \***************************************************************************/
        public static void MatrixMult(Trotation S1, Trotation S2, ref Trotation T)
        {
            T.M11 = S1.M11 * S2.M11 + S1.M12 * S2.M21 + S1.M13 * S2.M31;
            T.M12 = S1.M11 * S2.M12 + S1.M12 * S2.M22 + S1.M13 * S2.M32;
            T.M13 = S1.M11 * S2.M13 + S1.M12 * S2.M23 + S1.M13 * S2.M33;

            T.M21 = S1.M21 * S2.M11 + S1.M22 * S2.M21 + S1.M23 * S2.M31;
            T.M22 = S1.M21 * S2.M12 + S1.M22 * S2.M22 + S1.M23 * S2.M32;
            T.M23 = S1.M21 * S2.M13 + S1.M22 * S2.M23 + S1.M23 * S2.M33;

            T.M31 = S1.M31 * S2.M11 + S1.M32 * S2.M21 + S1.M33 * S2.M31;
            T.M32 = S1.M31 * S2.M12 + S1.M32 * S2.M22 + S1.M33 * S2.M32;
            T.M33 = S1.M31 * S2.M13 + S1.M32 * S2.M23 + S1.M33 * S2.M33;
        }


        /***************************************************************************\
            Multiply the two provided matricies and store the result in the target
        \***************************************************************************/
        public static void MatrixMult(Trotation M, float k, ref Trotation T)
        {
            T.M11 = M.M11 * k; T.M12 = M.M12 * k; T.M13 = M.M13 * k;
            T.M21 = M.M21 * k; T.M22 = M.M22 * k; T.M23 = M.M23 * k;
            T.M31 = M.M31 * k; T.M32 = M.M32 * k; T.M33 = M.M33 * k;
        }


        /***************************************************************************\
            Multiply the matrix with the point and store the result in the target
        \***************************************************************************/
        public static void MatrixMult(Trotation M, Tpoint P, ref Tpoint Tgt)
        {
            Tgt.x = M.M11 * P.x + M.M12 * P.y + M.M13 * P.z;
            Tgt.y = M.M21 * P.x + M.M22 * P.y + M.M23 * P.z;
            Tgt.z = M.M31 * P.x + M.M32 * P.y + M.M33 * P.z;
        }


        /***************************************************************************\
            Multiply the transpose of the matrix with the point and store the 
            result in the target
        \***************************************************************************/
        public static void MatrixMultTranspose(Trotation M, Tpoint P, ref Tpoint Tgt)
        {
            Tgt.x = M.M11 * P.x + M.M21 * P.y + M.M31 * P.z;
            Tgt.y = M.M12 * P.x + M.M22 * P.y + M.M32 * P.z;
            Tgt.z = M.M13 * P.x + M.M23 * P.y + M.M33 * P.z;
        }


        public void SetMatrixCPUMode(int nMode)		// 0 - Generic (default), 1- 3DNow, 2- ISSE
        {
            //Do nothing in C#
        }
    }
}

