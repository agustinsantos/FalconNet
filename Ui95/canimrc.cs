using System;
using System.IO;

namespace FalconNet.Ui95
{
	public class ANIMATION
	{
#if USE_SH_POOLS
	public:
		// Overload new/delete to use a SmartHeap pool
		void *operator new(size_t size) { return MemAllocPtr(UI_Pools[UI_CONTROL_POOL],size,FALSE);	};
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
		void *operator new(size_t size) { return MemAllocPtr(UI_Pools[UI_CONTROL_POOL],size,FALSE);	};
		void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
#endif
	
		public long ID;
		public long flags;
		public ANIMATION Anim;
		public ANIM_RES Next;
	};

	public class C_Animation
	{
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
		void *operator new(size_t size) { return MemAllocPtr(UI_Pools[UI_CONTROL_POOL],size,FALSE);	};
		void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
#endif
	
		private ANIM_RES Root_;

		private void ConvertAnim (ANIMATION Data)
		{
			throw new NotImplementedException ();
		}

		private void Convert16BitRLE (ANIMATION Data)
		{
			throw new NotImplementedException ();
		}

		private void Convert16Bit (ANIMATION Data)
		{
			throw new NotImplementedException ();
		}
	
		public C_Animation ()
		{
			throw new NotImplementedException ();
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
			throw new NotImplementedException ();
		}

		public void Cleanup ()
		{
			throw new NotImplementedException ();
		}

		public ANIM_RES LoadAnim (long ID, string file)
		{
			throw new NotImplementedException ();
		}

		public ANIM_RES GetAnim (long ID)
		{
			throw new NotImplementedException ();
		}

		public void SetFlags (long ID, long flags)
		{
			throw new NotImplementedException ();
		}

		public long GetFlags (long ID)
		{
			throw new NotImplementedException ();
		}

		public bool RemoveAnim (long ID)
				{throw new NotImplementedException();}

#if ! _UI95_PARSER_

		public short LocalFind (string token)
		{
			throw new NotImplementedException ();
		}

		public void LocalFunction (short ID, long[] P, string str, C_Handler h)
		{
			throw new NotImplementedException ();
		}
		//TODO public void SaveText(HANDLE h, C_Parser p) { ; }

#endif // PARSER
		public static  C_Animation gAnimMgr;
	}


}

