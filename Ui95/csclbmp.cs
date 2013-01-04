using System;
using System.IO;
using WORD = System.UInt16;
using COLORREF = System.Int32;

namespace FalconNet.Ui95
{

    public class C_ScaleBitmap : C_Base
    {
#if USE_SH_POOLS
	public:
		// Overload new/delete to use a SmartHeap pool
		void *operator new(size_t size) { return MemAllocPtr(UI_Pools[UI_CONTROL_POOL],size,FALSE);	};
		void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
#endif

        // Save from Here
        private long ImageID_;
        private long DefaultFlags_;
        private bool UseOverlay_;

        // Don't Save from here down
        private short[] Rows_ = new short[800];
        private short[] Cols_ = new short[800];
        private WORD r_shift_;
        private WORD g_shift_;
        private WORD b_shift_;

        private byte[] Overlay_;
        private WORD[] Palette_ = new WORD[16];
        private O_Output Image_;


        public C_ScaleBitmap()
        { throw new NotImplementedException(); }
        public C_ScaleBitmap(byte[] stream, ref int pos)
        { throw new NotImplementedException(); }
        public C_ScaleBitmap(FileStream fp)
        { throw new NotImplementedException(); }
        // public  ~C_ScaleBitmap();
        public long Size()
        { throw new NotImplementedException(); }
        public void Save(byte[] stream, ref int pos) { ; }
        public void Save(FileStream fp) { ; }

        // Initialization Function
        public void Setup(long ID, short Type, long ImageID)
        { throw new NotImplementedException(); }
        public void InitOverlay()
        { throw new NotImplementedException(); }
        public byte[] GetOverlay() { return (Overlay_); }
        public void UseOverlay() { if (Overlay_ != null) UseOverlay_ = true; }
        public void NoOverlay() { UseOverlay_ = false; }
        public void ClearOverlay()
        { throw new NotImplementedException(); }
        public void PreparePalette(COLORREF color)
        { throw new NotImplementedException(); }
        public void SetImage(long ID)
        { throw new NotImplementedException(); }
        public void SetImage(IMAGE_RSC image)
        { throw new NotImplementedException(); }
        public void SetSrcRect(UI95_RECT rect) { if (Image_ != null) Image_.SetSrcRect(rect); }
        public void SetDestRect(UI95_RECT rect) { if (Image_ != null) Image_.SetDestRect(rect); }
        public void SetScaleInfo(long scale) { if (Image_ != null) Image_.SetScaleInfo(scale); }
        public UI95_RECT GetSrcRect() { if (Image_ != null) return (Image_.GetSrcRect()); return (null); }
        public UI95_RECT GetDestRect() { if (Image_ != null) return (Image_.GetDestRect()); return (null); }

        public IMAGE_RSC GetImage() { if (Image_ != null) return (Image_.GetImage()); return (null); }
        // Free Function
        public void Cleanup()
        { throw new NotImplementedException(); }
        public void SetDefaultFlags() { SetFlags(DefaultFlags_); }
        public long GetDefaultFlags() { return (DefaultFlags_); }
        public void SetFlags(long flags)
        { throw new NotImplementedException(); }

        public void Refresh()
        { throw new NotImplementedException(); }
        public void Draw(SCREEN surface, UI95_RECT cliprect)
        { throw new NotImplementedException(); }

#if _UI95_PARSER_
		
		short LocalFind(char *token);
		void LocalFunction(short ID,long P[],_TCHAR *,C_Handler *);
		void SaveText(HANDLE ,C_Parser *)	{ ; }

#endif // PARSER
    }

}
