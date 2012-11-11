using System;
using System.IO;

namespace FalconNet.Common
{
	public struct DWORD
	{
		public DWORD(uint v)
		{
			val = v;
		}
		public UInt32 val;
	}
	
	public struct WORD
	{
		public WORD(ushort v)
		{
			val = v;
		}
		public UInt16 val;
	}
	
	public struct COLORREF {}
	public class SOUND_RES {}
	public struct POINT {}
	public struct IMAGE_RSC {}
	public struct CONTROLLIST {}
	public struct F4CSECTIONHANDLE {}
	public struct F4THREADHANDLE {}
	public struct HCURSOR {}
	
	public class VuEntity {
        public VuEntity(int t) { }
        public VuEntity (byte[] stream, ref int pos){}
        public VuEntity (FileStream filePtr){}
		protected float posx, posy, posz;
		public float XPos() { return posx;}
		public float YPos() { return posy;}
		public float ZPos() { return posz;}
	}
	public class VuEntityType {}
	public struct VuThread {}
	public class FalconSessionEntity {}
	public struct VU_BYTE {}
	public struct VU_ID {}
	public class VU_TIME {}
    public struct VU_ERRCODE { }
	public struct Unit {}
	public struct C_Handler {}
	public class DeviceManager {}
	public class DXContext {}
	public class HWND {}
	public class IDirectDraw7 {}
	public struct DamType {}
	public struct vector {}
	public struct C_ScrollBar {}
	public class C_Hash {}
	public class O_Output {}

	public struct CampUIEventElement {}
	public struct SquadUIInfoClass {}
	public struct EventElement {}
	public struct FalconGameEntity {}
	public struct FalconGameType {}
	public struct Team {}
}

