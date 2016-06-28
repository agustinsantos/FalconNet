using System;
using System.IO;

namespace FalconNet.Common
{
    public struct RECT
    {
        public long left;
        public long top;
        public long right;
        public long bottom;
    }

    public class SOUND_RES { }
    public struct POINT { }
    public struct CONTROLLIST { }
    public struct F4THREADHANDLE { }
    public struct HCURSOR { }



    public class DeviceManager { }
    public class DXContext { }
    public class HWND { }
    public struct HANDLE { }
    public class IDirectDraw7 { }

    public class TransformMatrix { }
    public class SimInitDataClass { }
    public class ObjectGeometry { }
    public class DrawablePoint { }
    public class DrawableRoadbed { }
    public class DrawablePlatform { }
    public class DrawableBuilding { }
    public class DrawableBridge { }
    public class DrawablePoled { }
    public class SfxClass
    {
        public void ACMIExec(float _simTime)
        {
            throw new NotImplementedException();
        }
    }
    public class StarData { }
    public struct OTWDriver
    {
        public static bool IsActive() { throw new NotImplementedException(); }
    }
    public class List
    {
        public object node; /* pointer to node data */
        public object user; /* pointer to user data */

        public List next;   /* next list node */
    }

    public struct CampUIEventElement { }
    public struct SquadUIInfoClass { }
    public struct EventElement { }
    public class DrawableTracer { }

    public struct FalconDeathMessage { }
    public struct UnitType { }
    public class ATCBrain
    {
        public ATCBrain(Object o) { }
    }
    public struct FalconDivertMessage { }
    public struct FalconPlayerStatusMessage { }

    public struct FalconDamageMessage { }
    public struct FalconEjectMessage { }
    public struct FalconLandingMessage { }
    public struct LoadoutStruct { }


    public struct SmallPathClass
    {
        public void ClearPath()
        {
            throw new NotImplementedException();
        }

        public void StepPath()
        {
            throw new NotImplementedException();
        }

        public int GetNextDirection()
        {
            throw new NotImplementedException();
        }
    }


    public struct AirframeClass { }
    public struct FackClass { }
    public struct FireControlComputer { }
    public struct SMSClass { }
    public struct GunClass { }


}

