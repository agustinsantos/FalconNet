using System;
using FalconNet.Common;
using System.IO;

namespace FalconNet.Ui95
{
// Parser Stuff
#if _UI95_PARSER_
	
	public enum CNTL_ENUM
	{
		CNTL_NOTHING=0,
		CNTL_SETID,
		CNTL_SETTYPE,
		CNTL_SETCLIENT,
		CNTL_SETX,
		CNTL_SETY,
		CNTL_SETW,
		CNTL_SETH,
		CNTL_SETXY,
		CNTL_SETWH,
		CNTL_SETXYWH,
		CNTL_SETGROUP,
		CNTL_SETCLUSTER,
		CNTL_SETFLAGS,
		CNTL_SETFLAGBITON,
		CNTL_SETFLAGBITOFF,
		CNTL_SETFLAGTOGGLE,
		CNTL_SETSOUND,
		CNTL_SETMENU,
		CNTL_USERDATA,
		CNTL_SETFONT,
		CNTL_SETHELP,
		CNTL_CURSOR,
		CNTL_SETMOVIE,
		CNTL_SETHOTKEY,
		CNTL_SETMOUSECOLOR,
		CNTL_SETMOUSEPERC,
		CNTL_SETDRAGCURSOR,
	};
	
	string[] C_Cntl_Tokens=
	{
		"[NOTHING]",
		"[ID]",
		"[TYPE]",
		"[CLIENT]",
		"[X]",
		"[Y]",
		"[W]",
		"[H]",
		"[XY]",
		"[WH]",
		"[XYWH]",
		"[GROUP]",
		"[CLUSTER]",
		"[FLAGS]",
		"[FLAGBITON]",
		"[FLAGBITOFF]",
		"[FLAGTOGGLE]",
		"[SOUNDBITE]",
		"[OPENMENU]",
		"[USERDATA]",
		"[FONT]",
		"[HELPTEXT]",
		"[CURSOR]",
		"[PLAYMOVIE]",
		"[HOTKEY]",
		"[OVERCOLOR]",
		"[OVERPERC]",
		"[DRAGCURSOR]",
		0,
	};
	
#endif
	
public class C_Control :  C_Base
{
	
		public enum CSB
		{
			CSB_IS_VALUE=1,
			CSB_IS_PTR,
			CSB_IS_CLEANUP_PTR,
			CSB_MAX_USERDATA=8,
		};
		// Save from here
	
		protected long		Cursor_;
		protected long		DragCursor_;
		protected long		MenuID_;
		protected long		HelpTextID_;
		protected COLORREF	MouseOverColor_;
		protected long		RelX_,RelY_;
		protected short		HotKey_;
		protected short		MouseOver_;
		protected short		MouseOverPercent_;

		// Don't save from here
		protected C_Hash	Sound_;
		// TODO protected void		(*Callback_)(long,short,C_Base*);

	
		public C_Control()
		{ throw new NotImplementedException(); }
		public C_Control(string stream)
		{ throw new NotImplementedException(); }
		public C_Control(FileStream fp)
		{ throw new NotImplementedException(); }

#if TODO
		public virtual ~C_Control()
		{
			if(Sound_)
			{
				Sound_->Cleanup();
				delete Sound_;
			}
		}
#endif		
		public override long Size()
		{ throw new NotImplementedException(); }
		public override void Save(string stream)
		{ throw new NotImplementedException(); }
		public override void Save(FileStream fp)
		{ throw new NotImplementedException(); }


		public override bool MouseOver(long relX,long relY,C_Base me)
		{ throw new NotImplementedException(); }

// Assignment Functions
		public override void SetRelX(long x)							{ RelX_=x; }
		public override void SetRelY(long y)							{ RelY_=y; }
		public override void SetRelXY(long x,long y)					{ RelX_=x; RelY_=y; }
		public override void SetCursorID(long id)						{ Cursor_=id; }
		public override void SetDragCursorID(long id)					{ DragCursor_=id; }
		public override void SetMenu(long id)							{ MenuID_=id; }
		public override void SetHelpText(long id)						{ HelpTextID_=id; }
		public override void SetHotKey(short key)						{ HotKey_=key; }
		//TODO public void SetCallback(void (*cb)(long,short,C_Base*)) { Callback_=cb; }
		public override void SetSound(long ID,short type)
		{ throw new NotImplementedException(); }
		public override void SetMouseOver(short state)					{ MouseOver_=state; }
		public override void SetMouseOverColor(COLORREF color)			{ MouseOverColor_=color; }
		public override void SetMouseOverPerc(short perc)				{ MouseOverPercent_=perc; }

		// Querry Functions
		public override bool  IsBase()					{ return(false); }
		public override bool  IsControl()				{ return(true); }
		public override long GetRelX()					{ return(RelX_); }
		public override long GetRelY()					{ return(RelY_); }
		public override long  GetCursorID()				{ return(Cursor_); }
		public override long  GetDragCursorID()			{ return(DragCursor_); }
		public override long  GetMenu()					{ return(MenuID_); }
		public override long  GetHelpText()				{ return(HelpTextID_); }
		public override short GetHotKey()				{ return(HotKey_); }
		public override short GetMouseOver()			{ return(MouseOver_); }
		public override void HighLite(SCREEN surface, UI95_RECT cliprect)
		{ throw new NotImplementedException(); }
		//TODO public void (*GetCallback())(long,short,C_Base*)	{ return(Callback_); }
		public override SOUND_RES GetSound(short type)
		{ throw new NotImplementedException(); }
	}
}

