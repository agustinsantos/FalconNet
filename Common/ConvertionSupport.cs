using System;
using System.IO;

namespace FalconNet.Common
{
	
	public struct COLORREF {}
	public class SOUND_RES {}
	public struct POINT {}
	public struct IMAGE_RSC {}
	public struct CONTROLLIST {}
	public struct F4CSECTIONHANDLE {}
	public struct F4THREADHANDLE {}
	public struct HCURSOR {}


	
	public class DeviceManager {}
	public class DXContext {}
	public class HWND {}
	public struct HANDLE {}
	public class IDirectDraw7 {}

	public class O_Output {}
	public class TransformMatrix {}
	public class DrawableObject {}
	public class SimInitDataClass {}
	public class ObjectGeometry {}
	public class DrawablePoint {}
	
	public struct OTWDriver {
		public static bool IsActive(){throw new NotImplementedException();}
	}
	public struct List {}
	public struct CampUIEventElement {}
	public struct SquadUIInfoClass {}
	public struct EventElement {}
	public struct DrawableTrail {}
	public class SimVehicleClass {}
	public class CampEntity {}
	public class TailInsertList {}
	public struct FalconDeathMessage {}
	public class F4PFList {}
    public class F4POList{}
	public class FalconPrivateOrderedList {}
	public class FalconPrivateList {}
	public class VuLinkedList {}
	public struct UnitType {}
	public class PackageClass {}
	public class MissionRequest{}
	public class ATCBrain {
		public ATCBrain(Object o){}
	}
	public struct FalconDivertMessage {}
	public struct FalconPlayerStatusMessage {}
	public struct FalconWeaponsFire {}
	public struct FalconDamageMessage {}
	public struct FalconEjectMessage {}
	public struct FalconLandingMessage {}
	public struct LoadoutStruct {}
	public struct CampaignHeading {}
	
	public struct SmallPathClass {
		public void ClearPath ()
		{
			throw new NotImplementedException ();
		}
		
		public void StepPath ()
		{
			throw new NotImplementedException ();
		}
		
		public int GetNextDirection () { 
		throw new NotImplementedException();
		}
	}
	public struct SquadronClass {}
	public struct FlightClass {}
	
	public struct AirframeClass {}
	public struct FackClass {}
	public struct FireControlComputer {}
	public struct SMSClass {}
	public struct GunClass {}

}

