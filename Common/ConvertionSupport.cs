using System;

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
	public struct SCREEN {}
	public class SOUND_RES {}
	public struct POINT {}
	public struct CONTROLLIST {}
	public struct F4CSECTIONHANDLE {}
	public struct F4THREADHANDLE {}
	public struct HCURSOR {}
	
	public class VuEntity {
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
	public struct Unit {}
	public struct C_Handler {}
	public struct DamType {}
	public struct Dirtyness {}
	public struct Dirty_Class {}
	public struct Dirty_Falcon_Entity {}
	public struct vector {}
	public struct C_ScrollBar {}
	public class C_Hash {}
	public struct ImageBuffer {}
	public struct GridIndex {}
	public struct CampUIEventElement {}
	public struct SquadUIInfoClass {}
	public struct MissionEvaluationClass {}
	public struct FalconGameEntity {}
	public struct FalconGameType {}
}

