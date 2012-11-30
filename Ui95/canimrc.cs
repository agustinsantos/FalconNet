using System;
using System.IO;
using WORD=System.UInt16;
using System.Diagnostics;
using System.Collections.Generic;

namespace FalconNet.Ui95
{
	public class ANIMATION
	{
#if USE_SH_POOLS
	public:
		// Overload new/delete to use a SmartHeap pool
		void *operator new(size_t size) { return MemAllocPtr(UI_Pools[UI_CONTROL_POOL],size,false);	};
		void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
#endif
	
		public byte[]  Header = new byte[4];
		public long  Version;
		public long  Width;
		public long  Height;
		public long  Frames;
		public short Compression;
		public short BytesPerPixel;
		public long  Background;
		public char[]  Start;
	}

	public struct ANIM_FRAME
	{
		public long Size;
		public byte[] Data;
	};

	public class ANIM_RES
	{
#if USE_SH_POOLS
	public:
		// Overload new/delete to use a SmartHeap pool
		void *operator new(size_t size) { return MemAllocPtr(UI_Pools[UI_CONTROL_POOL],size,false);	};
		void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
#endif
	
		public long ID;
		public long flags;
		public ANIMATION Anim;
		public ANIM_RES Next;
	};

	public class C_Animation
	{
		
		public enum CANM
		{
			CANM_NOTHING=0,
			CANM_LOADANIM,
		};

		public string[] C_Anim_Tokens =
		{
			"[NOTHING]",
			"[LOADANIM]"
		};
		public const int  RLE_END = 0x8000;
		public const int  RLE_SKIPROW = 0x4000;
		public const int  RLE_SKIPCOL = 0x2000;
		public const int  RLE_REPEAT = 0x1000;
		public const int  RLE_NO_DATA = 0xffff;
		public const int  RLE_KEYMASK = 0xf000;
		public const int  RLE_COUNTMASK = 0x0fff;
		public const int  COMP_NONE = 0;
		public const int  COMP_RLE = 1;
		public const int  COMP_DELTA = 2;
#if USE_SH_POOLS
	public:
		// Overload new/delete to use a SmartHeap pool
		void *operator new(size_t size) { return MemAllocPtr(UI_Pools[UI_CONTROL_POOL],size,false);	};
		void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
#endif
	
		private List<ANIM_RES> Root_;

		private void ConvertAnim (ANIMATION Data)
		{
			switch (Data.BytesPerPixel) {
			case 2:
				switch (Data.Compression) {
				case 0:
					Convert16Bit (Data);
					break;
				case 1:
				case 2:
				case 3:
				case 4:
					Convert16BitRLE (Data);
					break;
				}
				break;
			}
		}

		private void Convert16BitRLE (ANIMATION Data)
		{
			throw new NotImplementedException ();
		}

		private void Convert16Bit (ANIMATION Data)
		{
			#if TODO			
				ANIM_FRAME AnimPtr;
				long i,j;
				WORD dptr;
			
			
				AnimPtr=(ANIM_FRAME *)&Data.Start[0];
			
				for(i=0;i<Data.Frames;i++)
				{
					dptr=(WORD *)&AnimPtr.Data[0];
					
					for(j=0;j<Data.Height * Data.Width;j++)
					{
							*dptr=UI95_RGB15Bit(*dptr);
							dptr++;
					}
					AnimPtr=(ANIM_FRAME *)&AnimPtr.Data[AnimPtr.Size];
				}
			#endif 
			throw new NotImplementedException ();
		}
	
		public C_Animation ()
		{
			Root_ = null;
		}

		public C_Animation (byte[] stream, ref int pos)
		{
			throw new NotImplementedException ();
		}

		public C_Animation (FileStream fp)
		{
			throw new NotImplementedException ();
		}
		// public ~C_Animation();
		public long Size ()
		{
			throw new NotImplementedException ();
		}

		public void Save (byte[] stream, ref int pos)
		{
			throw new NotImplementedException ();
		}

		public void Save (FileStream fp)
		{
			throw new NotImplementedException ();
		}

		public void Setup ()
		{
			if (Root_ != null)
				Cleanup ();
		
			Root_ = null;
		}

		public void Cleanup ()
		{
			Root_ = null;
		}

		public ANIM_RES LoadAnim (long ID, string filename)
		{
			ANIM_RES cur, NewAnim = new ANIM_RES ();;
			FileStream ifp;
			long size;
		
			if (GetAnim (ID) != null)
				return(null);
			try {

				ifp = C_Parser.OpenArtFile (filename);
				if (ifp == null) {
					Debug.WriteLine ("C_Animation error: " + filename + " not opened\n");
					return NewAnim;
				}
		
				size = ifp.Length;

				if (size == 0) {
					ifp.Close ();
					Debug.WriteLine ("C_Animation seek end failed (" + filename + ")");
					return NewAnim;
				}

				
				NewAnim.ID = ID;
				NewAnim.Anim = new ANIMATION(); //TODO (ANIMATION *)((void*)new char [size + 1]);
				NewAnim.flags = 0;
				NewAnim.Next = null;
		
				if (NewAnim.Anim == null) {
					NewAnim = null;
					ifp.Close ();
					return(null);
				}
#if TODO				
				if (UI_READ (NewAnim.Anim, size, 1, ifp) != 1) {
					NewAnim.Anim = null;
					NewAnim = null;
					ifp.Close ();
					return(null);
				}
				ifp.Close ();
		
				ConvertAnim (NewAnim.Anim);
				cur = Root_;
				if (cur == null) {
					Root_ = NewAnim;
				} else {
					while (cur.Next)
						cur = cur.Next;
					cur.Next = NewAnim;
				}		
#endif
				throw new NotImplementedException();
			} catch (Exception e) {
			}
			return(NewAnim);
				
		}

		public ANIM_RES GetAnim (long ID)
		{
			foreach (ANIM_RES cur in Root_) {
				if (cur.ID == ID)
					return(cur);
			}
			return(null);
		}

		public void SetFlags (long ID, long flags)
		{
			ANIM_RES anim;
		
			anim = GetAnim (ID);
			if (anim != null)
				anim.flags = flags;
		}

		public long GetFlags (long ID)
		{
			ANIM_RES anim;
		
			anim = GetAnim (ID);
			if (anim != null)
				return(anim.flags);
			return(0);
		}

		public bool RemoveAnim (long ID)
		{
#if TODO			
			ANIM_RES cur, prev;

			if (Root_ == null)
				return(false);
			
			if (Root_.ID == ID) {
				cur = Root_;
				Root_ = Root_.Next;
				cur.Anim = null;
				cur = null;
				return(true);
			} else {
				cur = Root_;

				while (cur.Next) {
					if (cur.Next.ID == ID) {
						prev = cur.Next;
						cur.Next = prev.Next;
						prev.Anim = null;
						prev = null;
					}
					cur = cur.Next;
				}
				return(true);
			}		
			return(false);
#endif
			throw new NotImplementedException();
		}

#if ! _UI95_PARSER_

		public short LocalFind (string token)
		{
			for (short i = 0; i <C_Anim_Tokens.Length ; i++) {
				if (token == C_Anim_Tokens [i])
					return(i);
			}
			return(0);
		}

		public void LocalFunction (CANM ID, long[] P, string str, C_Handler h)
		{
			switch (ID) {
			case CANM.CANM_LOADANIM:
				LoadAnim (P [0], str);
				break;
			}
		}
		//TODO public void SaveText(HANDLE h, C_Parser p) { ; }

#endif // PARSER
		public static  C_Animation gAnimMgr;
	}


}

