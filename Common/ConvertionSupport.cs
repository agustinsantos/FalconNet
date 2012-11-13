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

	public struct Unit {}
	public struct C_Handler {}
	public class DeviceManager {}
	public class DXContext {}
	public class HWND {}
	public struct HANDLE {}
	public class IDirectDraw7 {}
	public struct DamType {}
	public struct C_ScrollBar {}
	public class C_Hash {}
	public class O_Output {}
	public class TransformMatrix {}
	public class DrawableObject {}
	public class SimInitDataClass {}
	public class ObjectGeometry {}
	public class DrawablePoint {}
	
	public struct List {}
	public struct CampUIEventElement {}
	public struct SquadUIInfoClass {}
	public struct EventElement {}
	public struct Team {}
	public struct DrawableTrail {}
	public class SimVehicleClass {}
	public struct CampEntity {}
	public struct Control {}
	public struct TailInsertList {}
	public struct FalconCampWeaponsFire {}
	public struct FalconDeathMessage {}
	public struct F4PFList {}
	public struct UnitType {}
	public class Package {}
	public class PackageClass {}
	public class MissionRequest{}
	public struct RadarRangeClass {}
	public struct ATCBrain {}
	public struct PriorityLevel {}
	public struct ObjectiveType {}
	public struct FalconDivertMessage {}
	public struct FalconPlayerStatusMessage {}
	public struct FalconWeaponsFire {}
	public struct FalconDamageMessage {}
	public struct FalconEjectMessage {}
	public struct FalconLandingMessage {}
	public struct WayPoint {}
	public struct LoadoutStruct {}
	public struct CampaignHeading {}
	public struct Path {}
	public struct AircraftClass {}
	public struct Percentage {}
	public struct CAMP_MISS_STRUCT {}
	public struct VehicleID {}
	public struct SmallPathClass {}
	public struct PilotClass {}
	public struct PilotInfoClass {}
	public struct NTM {}
	
	public struct AirframeClass {}
	public struct FackClass {}
	public struct FireControlComputer {}
	public struct SMSClass {}
	public struct GunClass {}

}

