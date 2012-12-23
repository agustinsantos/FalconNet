using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using DWORD = System.UInt32;

namespace FalconNet.Graphics
{
    public class GLContext : IContext
    {

        public bool Setup(ImageBuffer pIB, Common.DXContext c)
        {
            throw new NotImplementedException();
        }

        public void Cleanup()
        {
            throw new NotImplementedException();
        }

        public void NewImageBuffer(uint lpDDSBack)
        {
            throw new NotImplementedException();
        }

        public void SetState(MPR_STA State, long Value)
        {
            throw new NotImplementedException();
        }

        public void SetStateInternal(ushort State, uint Value)
        {
            throw new NotImplementedException();
        }

        public void ClearBuffers(ushort ClearInfo)
        {
            throw new NotImplementedException();
        }

        public void SwapBuffers(ushort SwapInfo)
        {
            throw new NotImplementedException();
        }

        public void StartFrame()
        {
            throw new NotImplementedException();
        }

        public void FinishFrame(object lpFnPtr)
        {
            throw new NotImplementedException();
        }

        public void SetColorCorrection(uint color, float percent)
        {
            throw new NotImplementedException();
        }

        public void SetupMPRState(int flag = 0)
        {
            throw new NotImplementedException();
        }

        public void SelectForegroundColor(DWORD color)
        {
            throw new NotImplementedException();
        }

        public void SelectBackgroundColor(int color)
        {
            throw new NotImplementedException();
        }

        public void SelectTexture(int texID)
        {
            throw new NotImplementedException();
        }

        public void RestoreState(MPRState state)
        {
            throw new NotImplementedException();
        }

        public void InvalidateState()
        {
            throw new NotImplementedException();
        }

        public int CurrentForegroundColor()
        {
            throw new NotImplementedException();
        }

        public void Render2DBitmap(int sX, int sY, int dX, int dY, int w, int h, int totalWidth, byte[] source)
        {
            throw new NotImplementedException();
        }

        public void Draw2DPoint(Tpoint v0)
        {
            throw new NotImplementedException();
        }

        public void Draw2DPoint(float x, float y)
        {
            throw new NotImplementedException();
        }

        public void Draw2DLine(Tpoint v0, Tpoint v1)
        {
            throw new NotImplementedException();
        }

        public void Draw2DLine(float x0, float y0, float x1, float y1)
        {
            throw new NotImplementedException();
        }

        public void DrawPrimitive2D(int type, int nVerts, int[] xyzIdxPtr)
        {
            throw new NotImplementedException();
        }

        public void DrawPrimitive(int type, ushort VtxInfo, ushort Count, MPRVtx_t[] data, ushort Stride)
        {
            throw new NotImplementedException();
        }

        public void SetViewportAbs(int nLeft, int nTop, int nRight, int nBottom)
        {
            throw new NotImplementedException();
        }

        public void LockViewport()
        {
            throw new NotImplementedException();
        }

        public void UnlockViewport()
        {
            throw new NotImplementedException();
        }
    }
}
