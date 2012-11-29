using System;
using System.IO;
using WORD=System.UInt16;
using System.Diagnostics;
using System.Text;

namespace FalconNet.Ui95
{
	public class KerningStr
	{
#if USE_SH_POOLS
	public:
		// Overload new/delete to use a SmartHeap pool
		void *operator new(size_t size) { return MemAllocPtr(UI_Pools[UI_CONTROL_POOL],size,FALSE);	};
		void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
#endif
	
		public short first;
		public short second;
		public short  add_;
	};

	public class CharStr
	{
#if USE_SH_POOLS
	public:
		// Overload new/delete to use a SmartHeap pool
		void *operator new(size_t size) { return MemAllocPtr(UI_Pools[UI_CONTROL_POOL],size,FALSE);	};
		void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
#endif
	
		public byte flags;
		public byte w;
		public char lead;
		public char trail;
	};

	public class C_Fontmgr
	{
		public const int FNT_CHECK_KERNING_ = 0x40;
#if USE_SH_POOLS
	public:
		// Overload new/delete to use a SmartHeap pool
		void *operator new(size_t size) { return MemAllocPtr(UI_Pools[UI_CONTROL_POOL],size,FALSE);	};
		void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
#endif
	
		private long		ID_;
		private string		name_;
		private int			height_;
		private long		bytesperline_;
		private long		fNumChars_;
		private CharStr[]		fontTable_;
		private long		dSize_;
		private byte[]		fontData_;
		private long		kNumKerns_;
		private KerningStr[]	kernList_;
		private char		first_;
		private char		last_;

		public C_Fontmgr ()
		{
			ID_ = 0;

			name_ = "";
			height_ = 0;

			bytesperline_ = 0;

			fNumChars_ = 0;
			fontTable_ = null;

			dSize_ = 0;
			fontData_ = null;

			kNumKerns_ = 0;
			kernList_ = null;
		}
		//TODO public ~C_Fontmgr();

		public void Setup (long ID, string fontfile)
		{
			FileStream fp;
			try {
				ID_ = ID;

				fp = C_Parser.OpenArtFile (fontfile);
				if (fp == null) {
					Debug.WriteLine ("FONT error: " + fontfile + " not opened\n");
					return;
				}
				byte[] buff = new byte[32];
				fp.Read(buff, 0, 32);
				name_ = Encoding.ASCII.GetString(buff, 0, 32);
				fp.Read(buff, 0, sizeof(long));
				height_ = (int)BitConverter.ToInt64(buff, 0);
				fp.Read(buff, 0, sizeof(short));
				first_ = (char)BitConverter.ToInt16(buff,0);
				fp.Read(buff, 0, sizeof(short));
				last_ = (char)BitConverter.ToInt16(buff,0);
				fp.Read(buff, 0, sizeof(long));
				bytesperline_ = BitConverter.ToInt64(buff,0);
				fp.Read(buff, 0, sizeof(long));
				fNumChars_ = BitConverter.ToInt32(buff,0);
				fp.Read(buff, 0, sizeof(long));
				kNumKerns_ = BitConverter.ToInt64(buff,0);
				fp.Read(buff, 0, sizeof(long));
				dSize_ = BitConverter.ToInt64(buff,0);
#if TODO
				if (fNumChars_ != 0)
				{
					fontTable_=new CharStr[fNumChars_];
					buff = new byte[fNumChars_]; //TODO 
					fp.Read(buff, 0, (int)fNumChars_);
				}
				if (kNumKerns_ != 0)
				{
					kernList_=new KerningStr[kNumKerns_];
					buff = new byte[fNumChars_]; //TODO 
					fp.Read(buff, 0, (int)kNumKerns_);
				}
				if (dSize_ != 0)
				{
					fontData_=new byte[dSize_];
					fp.Read(fontData_, 0, (int)dSize_);
				}
#endif	
				fp.Close ();
			} catch (IOException e) {
				Debug.WriteLine ("Error opening font file" + fontfile);
				throw e;
			}
		}

		public void Cleanup ()
		{
			ID_ = 0;
			bytesperline_ = 0;

			fNumChars_ = 0;
			if (fontTable_ != null) {
				fontTable_ = null;
			}

			kNumKerns_ = 0;
			if (kernList_ != null) {
				kernList_ = null;
			}

			dSize_ = 0;
			if (fontData_ != null) {
#if USE_SH_POOLS
		MemFreePtr(fontData_);
#endif
				fontData_ = null;
			}
		}

		public long GetID ()
		{
			return(ID_);
		}

		public char First ()
		{
			return(first_);
		}

		public char Last ()
		{
			return(last_);
		}

		public long ByteWidth ()
		{
			return(bytesperline_);
		}

		public int Width (string str)
		{
			int i;
			int size;

			if (string.IsNullOrWhiteSpace (str))
				return(0);
			size = 0;
			i = 0;

			foreach (char thechar in str) {
				//while(!F4IsBadReadPtr(&(str[i]), sizeof(_TCHAR)) && str[i]) // JB 010401 CTD (too much CPU)
				if (thechar >= first_ && thechar <= last_) {
					int charindx = thechar - first_;
					size += fontTable_ [thechar].lead + fontTable_ [thechar].w + fontTable_ [thechar].trail;
				}
				i++;
			}
			return(size + 1);
		}

		public int Width (string str, int length)
		{
			int i;
			int size;
			int thechar;

			if (string.IsNullOrWhiteSpace (str))
				return(0);
			size = 0;
			i = 0;
			while (i < str.Length &&  i < length) {
				thechar = str [i] & 0xff;
				if (thechar >= first_ && thechar <= last_) {
					thechar -= first_;
					size += fontTable_ [thechar].lead + fontTable_ [thechar].w + fontTable_ [thechar].trail;
				}
				i++;
			}
			return(size + 1);
		}

		public int Height ()
		{
			return(height_);
		}

		public CharStr  GetChar (short ID)
		{
			if (fontTable_ != null && ID >= first_ && ID <= last_)
				return(fontTable_ [ID - first_]);
			return(null);
		}

		public byte[] GetData ()
		{
			return(fontData_);
		}

		public string GetName ()
		{
			return(name_);
		}

		// no cliping version (except for screen)
		public void Draw (SCREEN surface, string str, WORD color, long x, long y)
		{
			throw new NotImplementedException ();
		}

		public void Draw (SCREEN surface, string str, long length, WORD color, long x, long y)
		{
			throw new NotImplementedException ();
		}
//!		void Draw(SCREEN *surface,_TCHAR *str,short length,WORD color,long x,long y);
		public void DrawSolid (SCREEN surface, string str, long length, WORD color, WORD bgcolor, long x, long y)
		{
			throw new NotImplementedException ();
		}
//!		void DrawSolid(SCREEN *surface,_TCHAR *str,short length,WORD color,WORD bgcolor,long x,long y);
		public void DrawSolid (SCREEN surface, string str, WORD color, WORD bgcolor, long x, long y)
		{
			throw new NotImplementedException ();
		}
		// clipping version (use cliprect)
		public void Draw (SCREEN surface, string str, WORD color, long x, long y, UI95_RECT cliprect)
		{
			throw new NotImplementedException ();
		}

		public void Draw (SCREEN surface, string str, long length, WORD color, long x, long y, UI95_RECT cliprect)
		{
			throw new NotImplementedException ();
		}
//!		void Draw(SCREEN *surface,_TCHAR *str,short length,WORD color,long x,long y,UI95_RECT *cliprect);
		public void DrawSolid (SCREEN surface, string str, long length, WORD color, WORD bgcolor, long x, long y, UI95_RECT cliprect)
		{
			throw new NotImplementedException ();
		}
//!		void DrawSolid(SCREEN *surface,_TCHAR *str,short length,WORD color,WORD bgcolor,long x,long y,UI95_RECT *cliprect);
		public void DrawSolid (SCREEN surface, string str, WORD color, WORD bgcolor, long x, long y, UI95_RECT cliprect)
		{
			throw new NotImplementedException ();
		}

		// Font creation Functions (for converting winders fonts to my BFT format)
		public void SetID (long ID)
		{
			ID_ = ID;
		}

		public void SetName (string name)
		{
			name_ = name;
		}

		public void SetHeight (int height)
		{
			height_ = height;
		}

		public void SetRange (long first, long last)
		{
			first_ = (char)(first);
			last_ = (char)(last);
		}

		public void SetBPL (long BPL)
		{
			bytesperline_ = BPL;
		}

		public void SetTable (long count, CharStr[] table)
		{
			fNumChars_ = count;
			if (count != 0)
				fontTable_ = table;
			else
				fontTable_ = null;
		}

		public void SetData (long size, byte[] data)
		{
			dSize_ = size;
			if (size != 0)
				fontData_ = data;
			else
				fontData_ = null;
		}

		public void SetKerning (long count, KerningStr[] kernlist)
		{
			kNumKerns_ = count;
			if (count != 0)
				kernList_ = kernlist;
			else
				kernList_ = null;
		}

		public void Save (string filename)
		{
			throw new NotImplementedException ();
		}
	}
}

