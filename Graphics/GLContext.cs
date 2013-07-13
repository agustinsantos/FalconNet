using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;

namespace FalconNet.Graphics
{
    public class GLContext : IContext
    {
        public bool Setup()
        {
            context = new GameWindow();
            context.Load += HandleOnLoadEvent;
            context.RenderFrame += HandleRenderEvent;
            return true;
        }

        public void Cleanup()
        {
            context.Load -= HandleOnLoadEvent;
            context.RenderFrame -= HandleRenderEvent;
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
            context.SwapBuffers(); 
        }

        public void StartFrame()
        {
            GL.Clear(ClearBufferMask.DepthBufferBit |
                    ClearBufferMask.ColorBufferBit |
                    ClearBufferMask.AccumBufferBit |
                    ClearBufferMask.StencilBufferBit);

        }

        public void FinishFrame(object lpFnPtr)
        {
            context.SwapBuffers(); 
        }

        public void SetColorCorrection(uint color, float percent)
        {
            throw new NotImplementedException();
        }

        public void SetupMPRState(int flag = 0)
        {
            throw new NotImplementedException();
        }

        public void SelectForegroundColor(uint color)
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

        public bool Setup(ImageBuffer pIB, Common.DXContext c)
        {
            throw new NotImplementedException();
        }

        protected void HandleOnLoadEvent(object sender, EventArgs e)
        {
            context.WindowBorder = WindowBorder.Hidden;
            context.WindowState = WindowState.Fullscreen;
            GL.ClearColor(new Color4(0.1f, 0.0f, 0.0f, 1.0f));
        }

        protected void HandleRenderEvent(object sender, FrameEventArgs e)
        {
            this.StartFrame();

            this.FinishFrame(null);
        }

        GameWindow context;

    }
}
