using System;
using FalconNet.Common;
using System.IO;

namespace FalconNet.Ui95
{
	/* TODO
	#define WIN_MAX_CLIENTS (8)
	#define WIN_MAX_RECTS   (200)
	#define WIN_HASH_SIZE	(50)
	
	#define MAX_CURSORS (30)

	F4CSECTIONHANDLE* UI_Enter(C_Window *win); // Must be defined prior to cwindow.h
	void UI_Leave(F4CSECTIONHANDLE* section);

	TODO */
	public class SCREEN
	{
		public WORD mem;
		public short width;
		public short height;
		//		short pitch;	// OW
	}

	public struct GRABBER
	{
		long ID_;
		long StartX_, StartY_;
		long ItemX_, ItemY_;
		//!	short GrabType_;
		long GrabType_;
		C_Window Window_;
		C_Base Control_;
	}

	public struct TOOL_TIP
	{
		C_Base Control_;
		long Time_;
		string Tip_;
		short MouseX_, MouseY_;
		short HelpOn_;
		long HelpFont_;
		UI95_RECT Area_;
	}
	// TODO typedef struct ControlListStr CONTROLLIST;

	public struct ControlListStr
	{
		C_Base Control_;
		CONTROLLIST Prev, Next;
	}

	public class C_Window
	{
		public const int WIN_MAX_CLIENTS = 8;
		public const int WIN_MAX_RECTS = 200;
		public const int WIN_HASH_SIZE = 50;
	
		public const int MAX_CURSORS = 30;
		
#if USE_SH_POOLS
	public:
		// Overload new/delete to use a SmartHeap pool
		void *operator new(size_t size) { return MemAllocPtr(UI_Pools[UI_CONTROL_POOL],size,FALSE);	};
		void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
#endif
	
		private enum CHR_ENUM
		{
			_CHR_CLIP_LEFT=0x01,
			_CHR_CLIP_RIGHT=0x02,
			_CHR_CLIP_TOP=0x04,
			_CHR_CLIP_BOTTOM=0x08,
		};

		// Save stuff starting here
		private long		ID_;
		private long		Flags_; // Flags for window
		private long		Section_; // Used for sections of the game
		private long		Group_; // Group ID
		private long		Cluster_; // Cluster ID (Similar use as group)
		private long		DefaultFlags_;
		private long		MenuID_;
		private long		RemoveImage_;
		private long		CursorID_;
		private long		MenuFlags_;
		private long[]		ClientMenuID_ = new long[WIN_MAX_CLIENTS];
		private long[]		ClientFlags_ = new long[WIN_MAX_CLIENTS];
		public long		update_;
		public long		Font_;
		public UI95_RECT	Area_;
		public COLORREF	BorderLite;	// Brightest color for border line
		public COLORREF	BorderMedium;	// normal color
		public COLORREF	BorderDark;	// Shadow color
		public COLORREF	BGColor;		// Background of client area color
		public COLORREF	SelectBG;		// Background color if selected
		public COLORREF	NormalText;	// Normal Text Color
		public COLORREF	ReverseText;	// Reverse color for text (when drawn over SelColor?)
		public COLORREF	DisabledText;	// Normal Text Color
		public COLORREF	TextColor_;
		public COLORREF	BgColor_;

		// Client Areas (upto WIN_MAX_CLIENTS (8) supported)
		public UI95_RECT[]	ClientArea_ = new UI95_RECT[WIN_MAX_CLIENTS];
		public UI95_RECT[]	FullClientArea_ = new UI95_RECT[WIN_MAX_CLIENTS]; // Used to restore client area when scrollbars are used


	
		protected short		Type_; // 0=Unknown,1=Window,2=Toolbar...
		protected short		Depth_; // used to determine which windows are in front of which
		protected short		x_, y_, w_, h_; // User Setable
		protected short		MinX_, MinY_, MaxX_, MaxY_; // Min/Max values of WindowX,WindowY
		protected short		MinW_, MinH_, MaxW_, MaxH_;
		protected short		DragH_; // Used to determine whether we can drag this window or not

		// Don't save from here down
		protected WORD		r_mask_, r_shift_, r_max_; // AND flag,shift values to convert 16bit RGB to usable value
		protected WORD		g_mask_, g_shift_, g_max_;
		protected WORD		b_mask_, b_shift_, b_max_;
		public long[]		VX_ = new long[WIN_MAX_CLIENTS], VY_ = new long[WIN_MAX_CLIENTS]; // x,y relative to scrollbar (Use for drawing EVERYTHING in client area)
		public long[]		VW_ = new long[WIN_MAX_CLIENTS], VH_ = new long[WIN_MAX_CLIENTS]; // w,h relative to scrollbar

	
		private short		Width_, Height_; // DDraw surface w/h
		private short		FontHeight_;
		private short		ControlCount_;
		public short[]		rectflag_ = new short[WIN_MAX_RECTS];
		public short		rectcount_;
		public UI95_RECT[]	rectlist_ = new UI95_RECT[WIN_MAX_RECTS];
		private POINT		Cursor_;
		private C_ScrollBar[] VScroll_ = new C_ScrollBar[WIN_MAX_CLIENTS], HScroll_ = new C_ScrollBar[WIN_MAX_CLIENTS];
		private ImageBuffer imgBuf_;
		private C_Base		Owner_;
		private C_Hash		Hash_;
		private CONTROLLIST Controls_;
		private CONTROLLIST Last_;
		private C_Base		CurControl_;
		// TODO private void (*DragCallback_)(C_Window *win);
		private F4CSECTIONHANDLE Critical;
		// TODO private bool (*KBCallback_)(unsigned char DKScanCode,unsigned char Ascii,unsigned char ShiftStates,long RepeatCount);

		private CONTROLLIST FindControlInList (C_Base cntrl)
		{
			throw new NotImplementedException();
		}

		private void GetScreenFormat ()
		{
			throw new NotImplementedException();
		}

		private long SetCheckedUpdateRect (long x1, long y1, long x2, long y2)
		{
			throw new NotImplementedException();
		}

		private void Fill (SCREEN surface, WORD Color, UI95_RECT rect)
		{
			throw new NotImplementedException();
		}
	
		public C_Handler Handler_; // Pointer to Handler class

		public C_Window ()
		{
			throw new NotImplementedException();
		}
		
		public C_Window (string p)
		{
			;
		}

		public C_Window (FileStream f)
		{
			;
		}
		// TODO public ~C_Window()			{ ; }
		public long Size ()
		{
			return 0;
		}

		public void Save (string p)
		{
			;
		}

		public void Save (FileStream f)
		{
			;
		}

		public void SetCritical (F4CSECTIONHANDLE section)
		{
			Critical = section;
		}

		public F4CSECTIONHANDLE GetCritical ()
		{
			return(Critical);
		}

		// Setup Functions
		public void Setup (long wID, short Type, short w, short h)
		{
			throw new NotImplementedException ();
		}

		public void ResizeSurface (short w, short h)
		{
			throw new NotImplementedException ();
		}

		public void SetType (short Type)
		{
			Type_ = Type;
		}

		public void SetX (short x)
		{
			throw new NotImplementedException ();
		}

		public void SetY (short y)
		{
			throw new NotImplementedException ();
		}

		public void SetW (short w)
		{
			throw new NotImplementedException ();
		}

		public void SetH (short h)
		{
			throw new NotImplementedException ();
		}

		public void SetRanges (short x1, short y1, short x2, short y2, short w, short h)
		{
			throw new NotImplementedException ();
		}

		public void SetDepth (short newdepth)
		{
			Depth_ = newdepth;
		}

		public void SetDragH (short h)
		{
			DragH_ = h;
		}

		public short GetDepth ()
		{
			return(Depth_);
		}

		public short GetDragH ()
		{
			return(DragH_);
		}

		public void ScanClientArea (long client)
		{
			throw new NotImplementedException ();
		}

		public void ScanClientAreas ()
		{
			throw new NotImplementedException ();
		}

		public void SetClientArea (UI95_RECT rect, short ID)
		{
			if (ID >= WIN_MAX_CLIENTS)
				return;
			ClientArea_ [ID] = rect;
			VX_ [ID] = ClientArea_ [ID].left;
			VY_ [ID] = ClientArea_ [ID].top;
		}

		public void SetClientArea (long x, long y, long w, long h, short ID)
		{
			if (ID >= WIN_MAX_CLIENTS)
				return;
			ClientArea_ [ID].left = x;
			ClientArea_ [ID].top = y;
			ClientArea_ [ID].right = x + w;
			ClientArea_ [ID].bottom = y + h;
			VX_ [ID] = x;
			VY_ [ID] = y;
		}

		public void SetVirtual (long x, long y, long w, long h, short ID)
		{
			if (ID >= WIN_MAX_CLIENTS)
				return;
			VX_ [ID] = -x;
			VY_ [ID] = -y;
			VW_ [ID] = w;
			VH_ [ID] = h;
		}

		public void SetVirtualX (long x, long Client)
		{
			if (Client >= WIN_MAX_CLIENTS)
				return;
			VX_ [Client] = -x;
		}

		public void SetVirtualY (long y, long Client)
		{
			if (Client >= WIN_MAX_CLIENTS)
				return;
			VY_ [Client] = -y;
		}

		public void SetVirtualW (long w, long Client)
		{
			if (Client >= WIN_MAX_CLIENTS)
				return;
			VW_ [Client] = w;
		}

		public void SetVirtualH (long h, long Client)
		{
			if (Client >= WIN_MAX_CLIENTS)
				return;
			VH_ [Client] = h;
		}

		public void SetFlags (long flag)
		{
			throw new NotImplementedException ();
		}

		public void DeactivateControl ()
		{
			throw new NotImplementedException ();
		}

		public void Activate ()
		{
			throw new NotImplementedException ();
		}

		public void Deactivate ()
		{
			throw new NotImplementedException ();
		}

		public void SetCursorID (long ID)
		{
			if (ID < MAX_CURSORS)
				CursorID_ = ID;
		}

		public long GetCursorID ()
		{
			return(CursorID_);
		}

		public long GetFlags ()
		{
			return(Flags_);
		}

		public void SetFlagBitOn (UI95_BITTABLE flag)
		{
			throw new NotImplementedException ();
		}

		public void SetFlagBitOff (UI95_BITTABLE flag)
		{
			throw new NotImplementedException ();
		}

		public void SetBgColor (COLORREF color)
		{
			BgColor_ = color;
			SetFlagBitOn (UI95_BITTABLE.C_BIT_USEBGFILL);
		}

		public void SetMenu (long ID)
		{
			MenuID_ = ID;
		}

		public void SetClientMenu (long Client, long ID)
		{
			if (Client < WIN_MAX_CLIENTS)
				ClientMenuID_ [Client] = ID;
		}
		// TODO public void SetDragCallback(void (*cb)(C_Window *)) { DragCallback_=cb; }
		public long GetMenu ()
		{
			return(MenuID_);
		}

		public long GetClientMenu (long Client)
		{
			if (Client < WIN_MAX_CLIENTS)
				return(ClientMenuID_ [Client]);
			return(0);
		}

		public void SetClientFlags (long Client, long flags)
		{
			if (Client < WIN_MAX_CLIENTS)
				ClientFlags_ [Client] = flags;
		}

		public long GetClientFlags (long Client)
		{
			if (Client < WIN_MAX_CLIENTS)
				return(ClientFlags_ [Client]);
			return(0);
		}

		public void AdjustScrollbar (long client)
		{
			throw new NotImplementedException();
		}

		public void AddScrollBar (C_ScrollBar scroll)
		{
			throw new NotImplementedException ();
		}

		public void AddControl (C_Base NewButton)
		{
			throw new NotImplementedException ();
		}

		public void AddControlTop (C_Base NewButton)
		{
			throw new NotImplementedException ();
		}

		public void RemoveControl (long ControlID)
		{
			throw new NotImplementedException ();
		}

		public CONTROLLIST RemoveControl (CONTROLLIST ctrl)
		{
			throw new NotImplementedException ();
		}

		public void RemoveAllControls ()
		{
			throw new NotImplementedException ();
		}

		public bool SetFont (long ID)
		{
			Font_ = ID;
			return(true);
		}

		public void SetHandler (C_Handler handler)
		{
			Handler_ = handler;
		}
		//TODO public void SetKBCallback(bool (*cb)(unsigned char DKScanCode,unsigned char Ascii,unsigned char ShiftStates,long RepeatCount)) { KBCallback_=cb; }
		public CONTROLLIST GetControlList ()
		{
			return(Controls_);
		}

		public void SetOwner (C_Base ctrl)
		{
			Owner_ = ctrl;
		}

		public C_Base GetOwner ()
		{
			return(Owner_);
		}

		// Keyboard Support Routines
		public bool CheckKeyboard (byte DKScanCode, byte Ascii, byte ShiftStates, long RepeatCount)
		{
			throw new NotImplementedException ();
		} // Called whenever a key is pressed
		public bool CheckHotKeys (byte DKScanCode, byte Ascii, byte ShiftStates, long RepeatCount)
		{
			throw new NotImplementedException ();
		} // Called whenever a key is pressed & CheckKeyboard returned FALSE
		public void SetControl (long ID)
		{
			throw new NotImplementedException ();
		} // Called when mouse is used over this control
		public void SetPrevControl ()
		{
			throw new NotImplementedException ();
		} // Called when SHIFT & TAB are pressed
		public void SetNextControl ()
		{
			throw new NotImplementedException ();
		} // Called when TAB is pressed

		// Cleanup Functions
		public void Cleanup ()
		{
			throw new NotImplementedException ();
		}

		public void SetDefaultFlags ()
		{
			SetFlags (DefaultFlags_);
		}

		public long GetDefaultFlags ()
		{
			return(DefaultFlags_);
		}

		// Query Functions
		public long GetID ()
		{
			return(ID_);
		}

		public void AddUpdateRect (long x1, long y1, long x2, long y2)
		{
			throw new NotImplementedException ();
		}

		public void SetUpdateRect (long x1, long y1, long x2, long y2, long flags, long client)
		{
			throw new NotImplementedException ();
		}

		public void ClearCheckedUpdateRect (long x1, long y1, long x2, long y2)
		{
			throw new NotImplementedException ();
		}

		public void ClearUpdateRect (long x1, long y1, long x2, long y2)
		{
			throw new NotImplementedException ();
		}

		public void SetSection (long sctn)
		{
			Section_ = sctn;
		}

		public void SetGroup (long grp)
		{
			Group_ = grp;
		}

		public void SetCluster (long clst)
		{
			Cluster_ = clst;
		}

		public short GetX ()
		{
			return(x_);
		}

		public short GetY ()
		{
			return(y_);
		}

		public short GetW ()
		{
			return(w_);
		}

		public short GetH ()
		{
			return(h_);
		}

		public short GetType ()
		{
			return(Type_);
		}

		public short GetPrimaryW ()
		{
			throw new NotImplementedException ();
		}

		public long GetSection ()
		{
			return(Section_);
		}

		public long GetGroup ()
		{
			return(Group_);
		}

		public long GetCluster ()
		{
			return(Cluster_);
		}

		public UI95_RECT GetClientArea (long ID)
		{
			if (ID < WIN_MAX_CLIENTS)
				return ClientArea_ [ID];
			return(ClientArea_ [0]);
		}

		public bool Minimized ()
		{
			if (w_ == MinW_ && h_ == MinH_)
				return(true);
			return(false);
		}

		public void Minimize ()
		{
			throw new NotImplementedException ();
		}

		public void Maximize ()
		{
			throw new NotImplementedException ();
		}

		public C_Handler GetHandler ()
		{
			return(Handler_);
		}

		public bool ClipToArea (UI95_RECT src, UI95_RECT dst, UI95_RECT ClipArea)
		{
			throw new NotImplementedException ();
		}

		public bool InsideClientWidth (long left, long right, long Client)
		{
			throw new NotImplementedException ();
		}

		public bool InsideClientHeight (long top, long bottom, long Client)
		{
			throw new NotImplementedException ();
		}

		public bool BelowClient (long y, long Client)
		{
			throw new NotImplementedException ();
		}

		public void SetMenuFlags (long flag)
		{
			MenuFlags_ = flag;
		}

		public long IsMenu ()
		{
			return(MenuFlags_);
		}

		public void EnableGroup (long ID)
		{
			throw new NotImplementedException ();
		}

		public void DisableGroup (long ID)
		{
			throw new NotImplementedException ();
		}

		public void UnHideGroup (long ID)
		{
			throw new NotImplementedException ();
		}

		public void HideGroup (long ID)
		{
			throw new NotImplementedException ();
		}

		public void EnableCluster (long ID)
		{
			throw new NotImplementedException ();
		}

		public void DisableCluster (long ID)
		{
			throw new NotImplementedException ();
		}

		public void UnHideCluster (long ID)
		{
			throw new NotImplementedException ();
		}

		public void HideCluster (long ID)
		{
			throw new NotImplementedException ();
		}

		public void SetGroupState (long GroupID, short state)
		{
			throw new NotImplementedException ();
		}

		public void GetRGBValues (ref WORD rm, ref WORD rs, ref WORD gm, ref WORD gs, ref WORD bm, ref WORD bs)
		{ 
			rm = r_mask_;
			rs = r_shift_;
			gm = g_mask_;
			gs = g_shift_;
			bm = b_mask_;
			bs = b_shift_; 
		}

		// Internally supported Drawing functions
		public void RefreshWindow ()
		{
			throw new NotImplementedException ();
		}

		public void RefreshClient (long Client)
		{
			throw new NotImplementedException ();
		}

		public void DrawWindow (SCREEN surface)
		{
			throw new NotImplementedException ();
		}

		public bool UpdateTimerControls ()
		{
			throw new NotImplementedException ();
		}

		public void DrawTimerControls ()
		{
			throw new NotImplementedException ();
		}

		public void TextColor (COLORREF color)
		{
			TextColor_ = color;
		}

		public void TextBkColor (COLORREF color)
		{
			BgColor_ = color;
		}

		public void Blend (WORD front, UI95_RECT frect, short fwidth, WORD back, UI95_RECT brect, short bwidth, ref WORD dest, UI95_RECT drect, short dwidth, short fperc, short bperc)
		{
			throw new NotImplementedException();
		}

		public void BlendTransparent (WORD Mask, ref WORD front, UI95_RECT frect, short fwidth, ref WORD back, UI95_RECT brect, short bwidth, ref WORD dest, UI95_RECT drect, short dwidth, short fperc, short bperc)
		{
			throw new NotImplementedException();
		}

		public void Translucency (ref WORD front, UI95_RECT frect, short fwidth, ref WORD dest, UI95_RECT drect, short dwidth)
		{
			throw new NotImplementedException();
		}
		public void BlitTranslucent (SCREEN surface, COLORREF color, long Perc, UI95_RECT rect, long Flags, long client)
		{
			throw new NotImplementedException();
		}

		public void CustomBlitTranslucent (SCREEN surface, COLORREF color, long Perc, UI95_RECT rect, long Flags, long Client)
		{
			throw new NotImplementedException ();
		}

		public void DitherFill (SCREEN surface, COLORREF color, long perc, short size, ref char pattern, UI95_RECT rect, long Flags, long client)
		{
			throw new NotImplementedException ();
		}

		public void GradientFill (SCREEN surface, COLORREF Color, long Perc, UI95_RECT dst, long Flags, long Client)
		{
			throw new NotImplementedException ();
		}

		public void BlitFill (SCREEN surface, COLORREF Color, UI95_RECT dst, long Flags, long Client)
		{
			throw new NotImplementedException ();
		}

		public void BlitFill (SCREEN surface, COLORREF Color, long x, long y, long w, long h, long Flags, long Client, UI95_RECT clip)
		{
			throw new NotImplementedException ();
		}

		public void DrawHLine (SCREEN surface, COLORREF color, long x, long y, long w, long Flags, long Client, UI95_RECT clip)
		{
			throw new NotImplementedException ();
		}

		public void DrawVLine (SCREEN surface, COLORREF color, long x, long y, long h, long Flags, long Client, UI95_RECT clip)
		{
			throw new NotImplementedException ();
		}

		public bool CheckLine (long x1, long y1, long x2, long y2, long minx, long miny, long maxx, long maxy)
		{
			throw new NotImplementedException ();
		}

		public void DrawLine (SCREEN surface, COLORREF color, long x1, long y1, long x2, long y2, long Flags, long Client, UI95_RECT clip)
		{
			throw new NotImplementedException ();
		}

		public bool ClipLine (ref long x1, ref long y1, ref long x2, ref long y2, UI95_RECT clip)
		{
			throw new NotImplementedException ();
		}

		public void DrawClipLine (SCREEN surface, long x1, long y1, long x2, long y2, UI95_RECT clip, WORD color)
		{
			throw new NotImplementedException();
		}

		public void DrawCircle (SCREEN surface, COLORREF color, long x, long y, float radius, long Flags, long Client, UI95_RECT clip)
		{
			throw new NotImplementedException ();
		}

		public void DrawArc (SCREEN surface, COLORREF color, long x, long y, float radius, short section, long Flags, long Client, UI95_RECT clip)
		{
			throw new NotImplementedException ();
		}

		public void ClearWindow (SCREEN surface, long Client)
		{
			throw new NotImplementedException ();
		}

		public void ClearArea (SCREEN surface, long x1, long y1, long w, long h, long flags, long Client)
		{
			throw new NotImplementedException ();
		}

		public ImageBuffer GetSurface ()
		{
			return(imgBuf_);
		}

		public void SetSurface (ImageBuffer newsurface)
		{
			imgBuf_ = newsurface;
		}

		public bool KeyboardMode ()
		{
			throw new NotImplementedException ();
		}

		public void RemovingControl (C_Base control)
		{
			throw new NotImplementedException ();
		}
		// Handler Functions
		public C_Base FindControl (long ID)
		{
			throw new NotImplementedException ();
		}

		public C_Base GetControl (ref long ID, long relX, long relY)
		{
			throw new NotImplementedException ();
		}

		public C_Base MouseOver (long relX, long relY, C_Base last)
		{
			throw new NotImplementedException ();
		}

		public bool Drag (GRABBER Drag, WORD MouseX, WORD MouseY, C_Window over)
		{
			throw new NotImplementedException ();
		}

		public bool Drop (GRABBER Drag, WORD MouseX, WORD MouseY, C_Window over)
		{
			throw new NotImplementedException ();
		}

#if _UI95_PARSER_

		public short LocalFind(char *token);
		public void LocalFunction(short ID,long P[],_TCHAR *,C_Handler *);
		public void SaveTextControls(HANDLE ofp,C_Parser *Parser);
		public void SaveText(HANDLE ,C_Parser *)	{ ; }

#endif // PARSER
		public static HCURSOR[] gCursors = new HCURSOR[MAX_CURSORS];

	}


}

