using System;
using WORD=System.UInt16;
using System.IO;

namespace FalconNet.Ui95
{
	public struct SOUND_RSC
	{}
	
	public enum RSC
	{
		_RSC_8_BIT_			=0x00000001,
		_RSC_16_BIT_		=0x00000002,
		_RSC_USECOLORKEY_	=0x40000000,
		_RSC_SINGLE_		=0x00000001,
		_RSC_MULTIPLE_		=0x00000002,
	
	// Add types as needed
		_RSC_IS_IMAGE_		=100,
		_RSC_IS_SOUND_		=101,
		_RSC_IS_FLAT_		=102,
	};
	
	public class C_Resmgr
	{
	#if USE_SH_POOLS
		public:
			// Overload new/delete to use a SmartHeap pool
			void *operator new(size_t size) { return MemAllocPtr(UI_Pools[UI_CONTROL_POOL],size,FALSE);	};
			void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
	#endif
		
			private long ID_;
			private long Type_;
	
			private long ResIndexVersion_;
			private long ResDataVersion_;
	
			private short reds,greens,blues; // shift values to convert to SCREEN format
	
			private C_Hash	IDTable_;
			private C_Hash	Index_;
			private byte[]	Idx_;
			private byte[]	Data_;
			private WORD	ColorKey_;
	
			private string	name_;
			private FileStream OpenResFile(string name, string sfx, string mode)
				{throw new NotImplementedException();}
		
	
			public C_Resmgr()
				{throw new NotImplementedException();}
			//TODO ~C_Resmgr();
	
			public long GetID()	{ return(ID_); }
			public long GetType_()	{ return(Type_); }
	
	// User callable functions
			public void Setup(long ID,string filename,C_Hash IDList)
				{throw new NotImplementedException();}
			public void Setup(long ID)
				{throw new NotImplementedException();}
			public void Cleanup()
				{throw new NotImplementedException();}
	
			public long Status() { if(Data_ == null) { if(Index_ != null) return(0x01); return(0); } return(0x03); }
	
			public void SetColorKey(WORD Key) { ColorKey_=Key; }
			// Convert Data_ to Screen format
			public void ConvertToScreen()
				{throw new NotImplementedException();}
			public void SetScreenFormat(short rs,short gs,short bs) { reds=rs; greens=gs; blues=bs; }
			public void LoadIndex()
				{throw new NotImplementedException();}
			public void AddIndex(long ID,IMAGE_RSC resheader)
				{throw new NotImplementedException();}
			public void AddIndex(long p,SOUND_RSC s) {}
			public void SetData(byte[] data) { if(Data_ != null) Data_ = null; Data_=data; }
			public byte[] GetData() { return(Data_); }
			public void LoadData()
				{throw new NotImplementedException();}
			public void UnloadData()
				{throw new NotImplementedException();}
			public object Find(long ID) { if(Index_ != null) return(Index_.Find(ID)); return(null ); }
	
			public  string ResName() { return(name_); }
	
			C_Hash GetIDList() { return(Index_); }
	}
}

