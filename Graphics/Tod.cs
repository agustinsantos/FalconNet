using System;
using FalconNet.Common;
using DWORD = System.Int16;

namespace FalconNet.Graphics
{




    //#define	USE_TRANSPARENT_MOON

    public class TimeOfDayStruct
    {
        public DWORD Time;
        public Tcolor SkyColor;
        public Tcolor HazeSkyColor;
        public Tcolor GroundColor;
        public Tcolor HazeGroundColor;
        public Tcolor TextureLighting;
        public float Ambient, Diffuse, StarIntensity;
        public int Flag;
        public float SunPitch, MoonPitch;
        // JPO additions
        public Tcolor RainColor;
        public Tcolor SnowColor;
        public Tcolor LightningColor;
        public Tcolor VisColor;
        public float MinVis;
    }


    //--------------------------------------------------------------

    public class CTimeOfDay
    {
        public const int GL_TIME_OF_DAY_USE_SUN = 0x001;
        public const int GL_TIME_OF_DAY_USE_MOON = 0x002;
        public const int GL_TIME_OF_DAY_USE_STAR = 0x004;

        public const int GL_TIME_OF_DAY_HAS_SUNBITMAP = 0x100;
        public const int GL_TIME_OF_DAY_HAS_MOONBITMAP = 0x200;

        public const int GL_STAR_NEW_COLOR = 0x2;
        public const int GL_STAR_END_OF_LIST = 0x4;

        public const float NVG_SKY_LEVEL = 0.390625f;	// 100 256ths
        public const float NVG_LIGHT_LEVEL = 0.703125f;	// 180 256ths

        public const int MOON_PHASE_SIZE = 128;			// max is 128
        public const int NEW_MOON_PHASE = (MOON_PHASE_SIZE / 2);

        //-------------------------------------------------------------
        public CTimeOfDay() { TimeOfDay = null; }
        //TODO public  ~CTimeOfDay()		{ if (IsReady())	Cleanup (); };

        public void Setup(string dataPath)
        { throw new NotImplementedException(); }
        public void Cleanup()
        { throw new NotImplementedException(); }

        public bool IsReady() { return (TimeOfDay != null); }

        public void SetNVGmode(bool state)
        { throw new NotImplementedException(); }
        public bool GetNVGmode() { return NVGmode; }

        public void GetLightDirection(ref Tpoint LightDirection)
        { throw new NotImplementedException(); }
        public void CalculateSunMoonPos(ref Tpoint pos, bool ismoon = false)
        { throw new NotImplementedException(); }
        public void CalculateSunGroundPos(ref Tpoint pos)
        { throw new NotImplementedException(); }
        public void SetSunGlareAngle(int angle)
        { throw new NotImplementedException(); }
        public float GetSunGlare(int yaw, int pitch)
        { throw new NotImplementedException(); }
        public int GetStarData(Tpoint[] vtx, Tcolor color)
        { throw new NotImplementedException(); }
        public float GetStarIntensity() { return StarIntensity; }
        public void CreateMoonPhaseMask(string image, int phase)
        { throw new NotImplementedException(); }



        public void CalculateMoonPhase()
        { throw new NotImplementedException(); }
        public void RotateMoonMask(int angle)
        { throw new NotImplementedException(); }
        public void SetCurrentSkyColor(Tcolor rgb) { CurrentSkyColor = rgb; }
        public void GetCurrentSkyColor(ref Tcolor rgb) { rgb = CurrentSkyColor; }
        public void GetSkyColor(ref Tcolor rgb) { rgb = SkyColor; }
        public void GetHazeSkyColor(ref Tcolor rgb) { rgb = HazeSkyColor; }
        public void GetGroundColor(ref Tcolor rgb) { rgb = GroundColor; }
        public void GetHazeGroundColor(ref Tcolor rgb) { rgb = HazeGroundColor; }
        public void GetTextureLightingColor(ref Tcolor rgb) { rgb = TextureLighting; }
        public void GetHazeSunriseColor(ref Tcolor rgb) { rgb = HazeSunriseColor; }
        public void GetHazeSunsetColor(ref Tcolor rgb) { rgb = HazeSunsetColor; }
        public void GetHazeSunHorizonColor(ref Tcolor rgb)
        {
            if (ISunPitch > 4096) rgb = HazeSunsetColor;
            else rgb = HazeSunriseColor;
        }
        public float GetAmbientValue() { return Ambient; }
        public float GetDiffuseValue() { return Diffuse; }
        public float GetLightLevel() { return Ambient + Diffuse; }
        public float GetMinVisibility() { return MinVis; }
        public void GetVisColor(ref Tcolor rgb) { rgb = VisColor; }
        public bool ThereIsASun() { return (ISunPitch > 0); }
        public bool ThereIsAMoon() { return (IMoonPitch > 0); }
        public int GetSunPitch()
        {
            int pitch = ISunPitch;
            if (pitch > 4096) pitch = 8192 - pitch;
            return pitch;
        }
        public int GetSunYaw()
        {
            int yaw = ISunYaw;
            if (ISunPitch > 4096) yaw += 8192;
            return yaw & 0x3fff;
        }
        public int GetMoonPitch()
        {
            int pitch = IMoonPitch;
            if (pitch > 4096) pitch = 8192 - pitch;
            return pitch;
        }
        public int GetMoonYaw()
        {
            int yaw = IMoonYaw;
            if (IMoonPitch > 4096) yaw += 8192;
            return yaw & 0x3fff;
        }
        public StarData[] GetStarData() { return TheStarData; }
        public float CalculateMoonBlend(float glare)
        { throw new NotImplementedException(); }
        public int CalculateMoonPercent()
        { throw new NotImplementedException(); }
        public void CreateMoonPhase(string src, string dest)
        { throw new NotImplementedException(); }

        public DWORD GetRainColor() { return RainColor; }
        public DWORD GetSnowColor() { return SnowColor; }
        public Tcolor GetLightningColor() { return LightningColor; }
        public int GetTotalTimeOfDay() { return TotalTimeOfDay; }
        public int GetTimeOfDay(int i) { return TimeOfDay[i].Time; }
        public float GetGroundColoring(int c) { return (TimeOfDay[c].GroundColor.r + TimeOfDay[c].GroundColor.g + TimeOfDay[c].GroundColor.b); }


        protected static byte[] MoonPhaseMask = new byte[8 * 64];
        protected static byte[] CurrentMoonPhaseMask = new byte[8 * 64];
        protected Tpoint SunCoord, MoonCoord;
        protected int MoonPhase;
        protected uint lastMoonTime;
        protected StarData[] TheStarData;
        protected TimeOfDayStruct[] TimeOfDay;
        protected int TotalTimeOfDay;
        protected Tcolor CurrentSkyColor;
        protected Tcolor SkyColor, HazeSkyColor;
        protected Tcolor GroundColor, HazeGroundColor;
        protected Tcolor HazeSunriseColor, HazeSunsetColor;
        protected Tcolor TextureLighting;
        protected DWORD RainColor, SnowColor;
        protected Tcolor LightningColor;
        protected Tcolor VisColor;
        protected float MinVis;
        protected float Ambient, Diffuse;
        protected int Flag;
        protected int ISunPitch, IMoonPitch;
        protected int ISunYaw, IMoonYaw;
        protected int ISunTilt, IMoonTilt;
        protected float SunGlareCosine, SunGlareFactor;
        protected float StarIntensity;
        protected bool NVGmode;

#if TODO
	protected static void TimeUpdateCallback( object self );
	protected void	UpdateSkyProperties();
	protected int		ReadTODFile (FILE *in, TimeOfDayStruct *tod, int countflag=0);
	protected void	SetDefaultColor(Tcolor *col, Tcolor *defcol);
	protected void	SetVar (TimeOfDayStruct *tod);
	protected static DWORD	MakeColor(Tcolor *col);
#endif
        public static CTimeOfDay TheTimeOfDay;
    }
}
