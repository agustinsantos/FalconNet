using FalconNet.Ui95;
using System;
using System.IO;


namespace FalconNet.UIExt
{


public class C_DrawList : C_Control
{
#if USE_SH_POOLS
	public:
		// Overload new/delete to use a SmartHeap pool
		void *operator new(size_t size) { return MemAllocPtr(UI_Pools[UI_CONTROL_POOL],size,FALSE);	};
		void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
#endif
	
		private long DefaultFlags_;
		private C_Hash Root_;
		private MAPICONLIST Last_;


        public C_DrawList()
        { throw new NotImplementedException(); }
        public C_DrawList(byte[] stream, ref int pos)
        { throw new NotImplementedException(); }
        public C_DrawList(FileStream fp)
        { throw new NotImplementedException(); }
		//public  ~C_DrawList();
        public long Size()
        { throw new NotImplementedException(); }
		public void Save(byte[] stream, ref int pos)	{ ; }
		public void Save(FileStream fp)	{ ; }

        public void Setup(long ID, short Type)
        { throw new NotImplementedException(); }
        public void Cleanup()
        { throw new NotImplementedException(); }
        public void Add(MAPICONLIST item)
        { throw new NotImplementedException(); }
        public void Remove(long ID)
        { throw new NotImplementedException(); }

		public long GetIconID() { if(Last_ != null) return(Last_.ID); return(0); }
		public long GetIconType() { if(Last_ != null) return(Last_.Type); return(0); }
 		public MAPICONLIST	 GetLastItem() { return(Last_); }

        public long CheckHotSpots(long relX, long relY)
        { throw new NotImplementedException(); }
        public bool Process(long ID, short HitType)
        { throw new NotImplementedException(); }

		public void SetDefaultFlags() { SetFlags((UI95_BITTABLE)DefaultFlags_); }
		public long GetDefaultFlags() { return(DefaultFlags_); }
        public void Refresh()
        { throw new NotImplementedException(); }
        public void Draw(SCREEN surface, UI95_RECT cliprect)
        { throw new NotImplementedException(); }
}

}
