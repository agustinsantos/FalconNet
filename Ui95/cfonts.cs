using System;
using System.IO;
using System.Collections.Generic;

namespace FalconNet.Ui95
{
	public class FONTLIST
	{
//TODO 
	};

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
		//TODO private FONTLIST *Root_;
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
			Fonts_.SetFlags (C_BIT_REMOVE);
			Fonts_.SetCallback (HashDelCB);
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

		public List<C_Font> FindID (long ID)
		{
			List<C_Font> cur;

			cur = Root_;
			while (cur) {
				if (cur.ID_ == ID)
					return(cur);
				cur = cur.Next;
			}
			return(Root_);
		}
		// Query Functions
		public HFONT GetFont (long ID)
		{
			List<C_Font> cur;

			cur = FindID (ID);
			if (cur)
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
			if (found)
				return(found.Height ());
			found = Find (1);
			if (found)
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
			if (found)
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
			if (found)
				return(found.Width (Str, len));
			return(0);
#endif
		}

		public C_Fontmgr Find (long ID)
		{
			C_Fontmgr cur;
			if (Fonts_) {
				cur = (C_Fontmgr*)Fonts_.Find (ID);
				if (!cur)
					cur = (C_Fontmgr*)Fonts_.Find (1);
				return(cur);
			}
			return(null);
		}

		public void LoadFont (long ID, string filename)
		{
			C_Fontmgr * newfont;

			if (Fonts_.Find (ID)) // Check to see if ID used
				return;

			newfont = new C_Fontmgr ();
			newfont.Setup (ID, filename);

			if (!newfont.Height ()) {
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
			FONTLIST cur, last;

			cur = Root_;
			while (cur) {
				last = cur;
				cur = cur.Next;
				DeleteObject (last.Font_);
				last.Widths_ = null;
				last = null;
			}
			Root_ = null;
			if (Fonts_) {
				Fonts_.Cleanup ();
				delete Fonts_;
				Fonts_ = null;
			}
		}

#if _UI95_PARSER_

		public short FontFind(string token)
			{
	short i=0;

	while(C_Fnt_Tokens[i])
	{
		if(strnicmp(token,C_Fnt_Tokens[i],strlen(C_Fnt_Tokens[i])) == 0)
			return(i);
		i++;
	}
	return(0);
}
		public void FontFunction(short ID,long P[],_TCHAR *str,LOGFONT *lgfnt,long *NewID)
			{
	switch(ID)
	{
		case CFNT_ID:
			*NewID=P[0];
			break;
		case CFNT_lfHeight:
			if(lgfnt)
				lgfnt.lfHeight|=P[0];
			break;
		case CFNT_lfWidth:
			if(lgfnt)
				lgfnt.lfWidth|=P[0];
			break;
		case CFNT_lfEscapement:
			if(lgfnt)
				lgfnt.lfEscapement|=P[0];
			break;
		case CFNT_lfOrientation:
			if(lgfnt)
				lgfnt.lfOrientation|=P[0];
			break;
		case CFNT_lfWeight:
			if(lgfnt)
				lgfnt.lfWeight|=P[0];
			break;
		case CFNT_lfItalic:
			if(lgfnt)
				lgfnt.lfItalic|=(BYTE)P[0];
			break;
		case CFNT_lfUnderline:
			if(lgfnt)
				lgfnt.lfUnderline|=(BYTE)P[0];
			break;
		case CFNT_lfStrikeOut:
			if(lgfnt)
				lgfnt.lfStrikeOut|=(BYTE)P[0];
			break;
		case CFNT_lfCharSet:
			if(lgfnt)
				lgfnt.lfCharSet|=(BYTE)P[0];
			break;
		case CFNT_lfOutPrecision:
			if(lgfnt)
				lgfnt.lfOutPrecision|=(BYTE)P[0];
			break;
		case CFNT_lfClipPrecision:
			if(lgfnt)
				lgfnt.lfClipPrecision|=(BYTE)P[0];
			break;
		case CFNT_lfQuality:
			if(lgfnt)
				lgfnt.lfQuality|=(BYTE)P[0];
			break;
		case CFNT_lfPitchAndFamily:
			if(lgfnt)
				lgfnt.lfPitchAndFamily|=(BYTE)P[0];
			break;
		case CFNT_lfFaceName:
			if(lgfnt)
				_tcsncpy(lgfnt.lfFaceName,str,LF_FACESIZE);
			break;
		case CFNT_SPACING:
			Spacing_=P[0];
			break;
		case CFNT_ADDFONT:
			AddFont(*NewID,lgfnt);
			Spacing_=0;
			memset(lgfnt,0,sizeof(LOGFONT));
			break;
		case CFNT_LOAD:
			LoadFont(P[0],str);
			break;
	}
}
		public void SaveText(HANDLE,C_Parser *) { ; }

#endif
		public static List<C_Font> gFontList;
		
		private enum CFNT
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
			"[LOADFONT]",
			0,
		};
	}
}

