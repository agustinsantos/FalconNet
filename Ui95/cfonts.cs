using System;
using System.IO;
using System.Collections.Generic;

namespace FalconNet.Ui95
{
	// TODO: LOGFONT is defined in windows
	public class LOGFONT
	{
		  public long  lfHeight;
		  public long  lfWidth;
		  public long  lfEscapement;
		  public long  lfOrientation;
		  public long  lfWeight;
		  public byte  lfItalic;
		  public byte  lfUnderline;
		  public byte  lfStrikeOut;
		  public byte  lfCharSet;
		  public byte  lfOutPrecision;
		  public byte  lfClipPrecision;
		  public byte  lfQuality;
		  public byte  lfPitchAndFamily;
		  public string lfFaceName;
	}
	
	// TODO: HFONT is defined in windows
	public struct HFONT
	{
	}
	
	public struct TEXTMETRIC
	{
	}
	
	public class FONTINFO
	{
	#if USE_SH_POOLS
		public:
			// Overload new/delete to use a SmartHeap pool
			void *operator new(size_t size) { return MemAllocPtr(UI_Pools[UI_CONTROL_POOL],size,FALSE);	};
			void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
	#endif
		 
		public long ID_;
		public HFONT Font_;
		public long Spacing_;
		public TEXTMETRIC Metrics_;
		public int[] Widths_;
	#if _UI95_PARSER_
			LOGFONT logfont;
	#endif
	}

	public class C_Font
	{
#if USE_SH_POOLS
	public:
		// Overload new/delete to use a SmartHeap pool
		void *operator new(size_t size) { return MemAllocPtr(UI_Pools[UI_CONTROL_POOL],size,false);	};
		void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
#endif
	
		// Save from here
		private long Spacing_;

		// Don't save from here
		private C_Handler Handler_;
		private List<FONTINFO> Root_;
		private C_Hash Fonts_; // Font Resource List (Actual ones used)

	
		public C_Font ()
		{
			Root_ = null;
			Spacing_ = 0;
			Fonts_ = null;
		}
#if TODO
		public C_Font(byte[] stream, ref int pos);
		public C_Font(FileStream fp);
		// TODO public ~C_Font();
		public long Size();
		public void Save(byte[] stream, ref int pos);
		public void Save(FileStream fp);
#endif
		// Setup functions
		public void Setup (C_Handler handler)
		{
			Handler_ = handler;
			Fonts_ = new C_Hash ();
			Fonts_.Setup (5);
			Fonts_.SetFlags (UI95_BITTABLE.C_BIT_REMOVE);
			Fonts_.SetCallback (HashDelCB);
		}
		
		private static void HashDelCB (object me)
		{
			C_Fontmgr fnt;
		
			fnt = (C_Fontmgr)me;
			if (fnt != null) {
				fnt.Cleanup ();
				fnt = null;
			}
		}
		
		public bool AddFont (long i, LOGFONT id)
		{

#if NOTHING
	FONTLIST *newfont,*cur;
	HDC hdc;

	newfont=new FONTLIST;
	newfont.Spacing_=Spacing_;
	newfont.Font_=CreateFontIndirect(reqs);
	if(newfont.Font_ == null)
	{
		delete newfont;
		return(false);
	}
	memcpy(&newfont.logfont,reqs,sizeof(LOGFONT));

	newfont.ID_=ID;
	Handler_.GetDC(&hdc);
	SelectObject(hdc,newfont.Font_);
	GetTextMetrics(hdc,&newfont.Metrics_);
	newfont.Widths_=new INT[newfont.Metrics_.tmLastChar+1];
	if(!GetCharWidth(hdc,0,newfont.Metrics_.tmLastChar,&newfont.Widths_[0]))
	{
		VOID *lpMsgBuf;

		FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM,
					  null,GetLastError(),MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT),(LPTSTR) &lpMsgBuf,0,null );

		// Display the string.
		MessageBox( null, (char *)lpMsgBuf, "GetLastError", MB_OK|MB_ICONINFORMATION );

		// Free the buffer.
		LocalFree( lpMsgBuf );
	}
	Handler_.ReleaseDC(hdc);

	newfont.Next=null;

	if(Root_ == null)
	{
		Root_=newfont;
	}
	else
	{
		cur=Root_;
		while(cur.Next)
			cur=cur.Next;
		cur.Next=newfont;
	}

	return(TRUE);
#endif
			return(false);
		}

		public void RemoveFont (long od)
		{
			;
		}

		public FONTINFO FindID (long ID)
		{
			foreach (FONTINFO cur in Root_) {
				if (cur.ID_ == ID)
					return(cur);
			}
			return null;
		}
		// Query Functions
		public HFONT? GetFont (long ID)
		{
			FONTINFO cur;

			cur = FindID (ID);
			if (cur != null)
				return(cur.Font_);
			return(null);
		}

		public int GetHeight (long ID)
		{
#if NOTHING
	// Replacing OLD code
	FONTLIST *cur;

	cur=FindID(ID);
	if(cur)
		return(cur.Metrics_.tmHeight);
#else // New method
			C_Fontmgr found;

			found = Find (ID);
			if (found != null)
				return(found.Height ());
			found = Find (1);
			if (found != null)
				return(found.Height ());
#endif
			return(0);
		}

		public int StrWidth (long fontID, string Str)
		{
#if NOTHING
	// Removing OLD method
	FONTLIST *cur;
	int w=0,i;
	_TCHAR c;

	cur=FindID(fontID);
	if(cur)
	{
		i=0;

		while(Str[i])
		{
			c=Str[i];
			if(c > cur.Metrics_.tmLastChar) c=0;
			w+=cur.Widths_[c] + cur.Spacing_;
			i++;
		}
		return(w+1);
	}
	return(0);
#else
			C_Fontmgr found;

			found = Find (fontID);
			if (found != null)
				return(found.Width (Str));
			return(0);
#endif
		}

		public int StrWidth (long fontID, string Str, int len)
		{
#if NOTHING // Replacing OLD method
	FONTLIST *cur;
	int w=0,i;
	_TCHAR c;

	cur=FindID(fontID);
	if(cur)
	{
		i=0;

		while(Str[i] && i < len)
		{
			c=Str[i];
			if(c > cur.Metrics_.tmLastChar) c=0;
			w+=cur.Widths_[c]+cur.Spacing_;
			i++;
		}
		return(w+1);
	}
	return(0);
#else // New method
			C_Fontmgr found;

			found = Find (fontID);
			if (found != null)
				return(found.Width (Str, len));
			return(0);
#endif
		}

		public C_Fontmgr Find (long ID)
		{
			C_Fontmgr cur;
			if (Fonts_ != null) {
				cur = (C_Fontmgr)Fonts_.Find (ID);
				if (cur == null)
					cur = (C_Fontmgr)Fonts_.Find (1);
				return(cur);
			}
			return(null);
		}

		public void LoadFont (long ID, string filename)
		{
			C_Fontmgr newfont;

			if (Fonts_.Find (ID) != null) // Check to see if ID used
				return;

			newfont = new C_Fontmgr ();
			newfont.Setup (ID, filename);

			if (newfont.Height () == 0) {
				newfont = null;
				return;
			}
			Fonts_.Add (ID, newfont);
		}

		public C_Hash GetHash ()
		{
			return(Fonts_);
		}

		// Cleanup Functions
		public void Cleanup ()
		{
			Root_ = null;
			if (Fonts_ != null) {
				Fonts_.Cleanup ();
				Fonts_ = null;
			}
		}

#if !_UI95_PARSER_

		public short FontFind (string token)
		{
			short i = 0;

			while (i < C_Fnt_Tokens.Length) {
				if (token == C_Fnt_Tokens [i])
					return(i);
				i++;
			}
			return(0);
		}

		public void FontFunction (CFNT ID, long[] P, string str, ref LOGFONT lgfnt, ref long NewID)
		{
			switch (ID) {
			case CFNT.CFNT_ID:
				NewID = P [0];
				break;
			case CFNT.CFNT_lfHeight:
				if (lgfnt != null)
					lgfnt.lfHeight |= P [0];
				break;
			case CFNT.CFNT_lfWidth:
				if (lgfnt!= null)
					lgfnt.lfWidth |= P [0];
				break;
			case CFNT.CFNT_lfEscapement:
				if (lgfnt!= null)
					lgfnt.lfEscapement |= P [0];
				break;
			case CFNT.CFNT_lfOrientation:
				if (lgfnt!= null)
					lgfnt.lfOrientation |= P [0];
				break;
			case CFNT.CFNT_lfWeight:
				if (lgfnt!= null)
					lgfnt.lfWeight |= P [0];
				break;
			case CFNT.CFNT_lfItalic:
				if (lgfnt!= null)
					lgfnt.lfItalic |= (byte)P [0];
				break;
			case CFNT.CFNT_lfUnderline:
				if (lgfnt!= null)
					lgfnt.lfUnderline |= (byte)P [0];
				break;
			case CFNT.CFNT_lfStrikeOut:
				if (lgfnt!= null)
					lgfnt.lfStrikeOut |= (byte)P [0];
				break;
			case CFNT.CFNT_lfCharSet:
				if (lgfnt!= null)
					lgfnt.lfCharSet |= (byte)P [0];
				break;
			case CFNT.CFNT_lfOutPrecision:
				if (lgfnt!= null)
					lgfnt.lfOutPrecision |= (byte)P [0];
				break;
			case CFNT.CFNT_lfClipPrecision:
				if (lgfnt!= null)
					lgfnt.lfClipPrecision |= (byte)P [0];
				break;
			case CFNT.CFNT_lfQuality:
				if (lgfnt!= null)
					lgfnt.lfQuality |= (byte)P [0];
				break;
			case CFNT.CFNT_lfPitchAndFamily:
				if (lgfnt!= null)
					lgfnt.lfPitchAndFamily |= (byte)P [0];
				break;
			case CFNT.CFNT_lfFaceName:
				if (lgfnt!= null)
					lgfnt.lfFaceName = str;
				break;
			case CFNT.CFNT_SPACING:
				Spacing_ = P [0];
				break;
			case CFNT.CFNT_ADDFONT:
				AddFont (NewID, lgfnt);
				Spacing_ = 0;
				lgfnt = null;//TODO memset (lgfnt, 0, sizeof(LOGFONT));
				break;
			case CFNT.CFNT_LOAD:
				LoadFont (P [0], str);
				break;
			}
		}
		//TODO public void SaveText(HANDLE h,C_Parser parser) { ; }

#endif
		public static List<C_Font> gFontList;
		
		public enum CFNT
		{
			CFNT_NOTHING=0,
			CFNT_ID,
			CFNT_lfHeight,
			CFNT_lfWidth,
			CFNT_lfEscapement,
			CFNT_lfOrientation,
			CFNT_lfWeight,
			CFNT_lfItalic,
			CFNT_lfUnderline,
			CFNT_lfStrikeOut,
			CFNT_lfCharSet,
			CFNT_lfOutPrecision,
			CFNT_lfClipPrecision,
			CFNT_lfQuality,
			CFNT_lfPitchAndFamily,
			CFNT_lfFaceName,
			CFNT_SPACING,
			CFNT_ADDFONT,
			CFNT_FONT,
			CFNT_LOAD
		}
		
		private static string[] C_Fnt_Tokens =
		{
			"[NOTHING]",
			"[ID]",
			"[lfHeight]",
			"[lfWidth]",
			"[lfEscapement]",
			"[lfOrientation]",
			"[lfWeight]",
			"[lfItalic]",
			"[lfUnderline]",
			"[lfStrikeOut]",
			"[lfCharSet]",
			"[lfOutPrecision]",
			"[lfClipPrecision]",
			"[lfQuality]",
			"[lfPitchAndFamily]",
			"[lfFaceName]",
			"[SPACING]",
			"[ADDFONT]",
			"[FONT]", // nothing done here
			"[LOADFONT]"
		};
	}
}

