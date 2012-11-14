using System;
using FalconNet.Common;
using System.IO;

namespace FalconNet.Ui95
{
	public class C_Button : C_Control
	{
	#if USE_SH_POOLS
		public:
			// Overload new/delete to use a SmartHeap pool
			void *operator new(size_t size) { return MemAllocPtr(UI_Pools[UI_CONTROL_POOL],size,false);	};
			void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
	#endif
		
			// Save from here
			private long origx_,origy_;
			private long LabelFlags_;
			private long DefaultFlags_;
			private long Font_;
			private short state_; // 0=Current,1=Up,2=Down,3=Disabled (Current means... if user hits return... this will be the button)
			private short laststate_;
			private short Percent_;
			private short FixedHotSpot_;
			private UI95_RECT HotSpot_;
	
			// Don't save from here
			private C_Hash  Root_;
			private O_Output BgImage_;
			private C_Base Owner_;
		
	#if _UI95_PARSER_
			public short UseHotSpot_;
	#endif // PARSER
			public C_Button()
				{ throw new NotImplementedException(); }
			public C_Button(string stream)
				{ throw new NotImplementedException(); }
			public C_Button(FileStream fp)
				{ throw new NotImplementedException(); }
			//TODO public ~C_Button();
			public long Size()
				{ throw new NotImplementedException(); }
			public void Save(string f)	{ ; }
			public void Save(FileStream f)	{ ; }
	
			public void SetDefaultFlags() { SetFlags(DefaultFlags_); }
			public long GetDefaultFlags() { return(DefaultFlags_); }
			// Setup Functions 
			public void Setup(long ID,short Type,long x,long y)
				{ throw new NotImplementedException(); }
		#if _UI95_PARSER_
			public void SetHotSpot(UI95_RECT hotspot) { HotSpot_=hotspot; if(hotspot.left == -1 && hotspot.right == -1) UseHotSpot_=0; else UseHotSpot_=1; }
			public void SetHotSpot(long x,long y,long x2,long y2) { HotSpot_.left=x;HotSpot_.top=y;HotSpot_.right=x2;HotSpot_.bottom=y2; if(x == -1 && x2 == -1) UseHotSpot_=0; else UseHotSpot_=1; }
		#endif	
			public UI95_RECT GetHotSpot() { return(HotSpot_); }
			public void SetBackImage(long ImageID)
				{ throw new NotImplementedException(); }
			public void SetLabel(long ID,string txt)
				{ throw new NotImplementedException(); }
			public void SetLabel(long ID,long txtID)
				{ throw new NotImplementedException(); }
	//!		void SetLabel(short ID,_TCHAR *txt);
	//!		void SetLabel(short ID,long txtID);
			public string GetLabel(short ID)
				{ throw new NotImplementedException(); }
			public void SetAllLabel(string txt)
				{ throw new NotImplementedException(); }
			public void SetAllLabel(long txtID)
				{ throw new NotImplementedException(); }
			public void SetColor(short ID,COLORREF color)
				{ throw new NotImplementedException(); }
			public void SetLabelOffset(short ID,long x,long y)
				{ throw new NotImplementedException(); }
			public void SetLabelColor(short ID,COLORREF color)
				{ throw new NotImplementedException(); }
			public void SetState(short state) { state_=state; }
			public short GetState() { return(state_); }
			public void SetImage(short ID,long ImageID)
				{ throw new NotImplementedException(); }
			public void ClearImage(short ID,long ImageID)
				{ throw new NotImplementedException(); }
			public void SetImage(short ID,IMAGE_RSC img)
				{ throw new NotImplementedException(); }
			public void SetAnim(short BtnID,long AnimID,short animtype,short dir)
				{ throw new NotImplementedException(); }
			public void SetText(short ID, string str)
				{ throw new NotImplementedException(); }
			public void SetText(short ID,long txtID)
				{ throw new NotImplementedException(); }
			public void SetFill(short ID,short w,short h)
				{ throw new NotImplementedException(); }
			public void SetXY(long x,long y)
				{ throw new NotImplementedException(); }
			public void SetPercent(short perc) { Percent_=perc; }
			public short GetPercent() { return(Percent_); }
			public bool TimerUpdate()
				{ throw new NotImplementedException(); }
	
			public string GetText(short ID)
				{ throw new NotImplementedException(); }
			public void SetFgColor(short ID,COLORREF color)
				{ throw new NotImplementedException(); }
			public void SetBgColor(short ID,COLORREF color)
				{ throw new NotImplementedException(); }
			public void SetFont(long FontID)
				{ throw new NotImplementedException(); }
			public void SetFlags(long flags)
				{ throw new NotImplementedException(); }
			public void SetOwner(C_Base me) { Owner_=me; }
	
			// Cleanup Functions
			public void Cleanup()
				{ throw new NotImplementedException(); }
	
			// Query Functions
			public long GetFont() { return(Font_); }
			public short GetImageW(short ID)
				{ throw new NotImplementedException(); }
			public short GetImageH(short ID)
				{ throw new NotImplementedException(); }
			public O_Output GetImage(short ID)
				{ throw new NotImplementedException(); }
	
			public void SetLabelInfo()
				{ throw new NotImplementedException(); }
			public void SetLabelFlagBitsOn(long flags)
				{ throw new NotImplementedException(); }
			public void SetLabelFlagBitsOff(long flags)
				{ throw new NotImplementedException(); }
	
			public void SetFixedHotSpot(short val) { FixedHotSpot_=val; }
	
			// Handler/Window Functions
			public long CheckHotSpots(long relx,long rely)
				{ throw new NotImplementedException(); }
			public bool CheckKeyboard(byte DKScanCode, byte pb, byte ShiftStates, long pl) { if((DKScanCode | (ShiftStates << 8)) == GetHotKey()) return(true); return(false); }
			public bool Process(long ID,short ButtonHitType)
				{ throw new NotImplementedException(); }
			public bool Dragable(long pb) {return(GetFlags().IsFlagSet(UI95_BITTABLE.C_BIT_DRAGABLE));}
			public void Refresh()
				{ throw new NotImplementedException(); }
			public void Draw(SCREEN surface, UI95_RECT  cliprect)
				{ throw new NotImplementedException(); }
			public void HighLite(SCREEN surface,UI95_RECT liprect)
				{ throw new NotImplementedException(); }
			public bool MouseOver(long relX,long relY,C_Base me)
				{ throw new NotImplementedException(); }
			public void GetItemXY(long p, ref long x,ref long y) { x=GetX(); y=GetY();}
			public bool Drag(GRABBER g,WORD MouseX,WORD MouseY,C_Window w)
				{ throw new NotImplementedException(); }
			public bool Drop(GRABBER g,WORD p1,WORD p2,C_Window w) { return false; } 
			public void SetSubParents(C_Window w)
				{ throw new NotImplementedException(); }
	
	#if _UI95_PARSER_
	
			public short LocalFind(string token)
				{ throw new NotImplementedException(); }
			public void LocalFunction(short ID,long P[], string p,C_Handler ch)
				{ throw new NotImplementedException(); }
			public void SaveText(HANDLE h,C_Parser ch)	{ ; }
	
	#endif // PARSER
	}

}

