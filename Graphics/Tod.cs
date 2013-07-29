using System;
using FalconNet.Common;
using DWORD = System.UInt32;
using System.Diagnostics;
using FalconNet.GrLib;
using System.IO;
using FalconNet.Common.Graphics;

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
        {
            Tcolor paletteEffect;
            int id;

            // Update the current state
            NVGmode = state;

            // Convert all the object texture palettes appropriatly
            paletteEffect.g = 1.0f;
            if (NVGmode)
            {
                paletteEffect.r = 0.0f;
                paletteEffect.b = 0.0f;
            }
            else
            {
                paletteEffect.r = 1.0f;
                paletteEffect.b = 1.0f;
            }
            for (id = 0; PaletteBankClass.IsValidIndex(id); id++)
            {
                PaletteBankClass.LightPalette(id, paletteEffect);
            }

            // Force a lighting refresh to reflect the changes
            TimeManager.TheTimeManager.Refresh();
        }

        public bool GetNVGmode() { return NVGmode; }

        public void GetLightDirection(ref Tpoint LightDirection)
        {
            float sH, cH, sC, cC;

            //Debug.Assert(LightDirection != null);

            // See if the sun is up
            if (ThereIsASun())
            {
                grmath.glGetSinCos(out sH, out cH, ISunYaw);
                grmath.glGetSinCos(out sC, out cC, ISunPitch);
            }
            else if (ThereIsAMoon())
            {
                grmath.glGetSinCos(out sH, out cH, IMoonYaw);
                grmath.glGetSinCos(out sC, out cC, IMoonPitch);
            }
            else
            {
                cC = 0.0f;
                sC = 1.0f;
                cH = 0.0F;
                sH = 0.0F;
            }
            LightDirection.x = cC * cH;
            LightDirection.y = cC * sH;
            LightDirection.z = -sC;
        }

        public void CalculateSunMoonPos(ref Tpoint pos, bool ismoon = false)
        {
            if (ismoon) pos = MoonCoord;
            else pos = SunCoord;
        }

        public void CalculateSunGroundPos(ref Tpoint pos)
        {
            pos.x = SunCoord.x;
            pos.y = SunCoord.y;
            pos.z = 0.0f;
        }

        public void SetSunGlareAngle(int angle)
        {
            SunGlareCosine = (float)grmath.glGetCosine(angle);
            SunGlareFactor = 1.0f / (1.0f - SunGlareCosine);
        }
        public float GetSunGlare(int yaw, int pitch)
        {
            // TODO: Instead of all this, just do a dot product with the light vector...
            int pitch1 = GetSunPitch();
            int yaw1 = GetSunYaw();
            float sin1, sin2, cos1, cos2, cos3;
            grmath.glGetSinCos(out sin1, out cos1, pitch);
            grmath.glGetSinCos(out sin2, out cos2, pitch1);
            cos3 = (float)grmath.glGetCosine(yaw - yaw1);
            float alpha = sin1 * sin2 + cos1 * cos2 * cos3;

            alpha -= SunGlareCosine;
            alpha *= SunGlareFactor;

            // just to make sure, clamp value
            if (alpha > 1.0f) alpha = 1.0f;
            else if (alpha < 0.0f) alpha = 0.0f;

            return alpha;
        }
        public int GetStarData(Tpoint[] vtx, Tcolor color)
        { throw new NotImplementedException(); }
        public float GetStarIntensity() { return StarIntensity; }
        public void CreateMoonPhaseMask(byte[] image, int phase)
        {
#if TODO
            if (phase == NEW_MOON_PHASE) 			// new moon -. all moon dark
                // TODO memset ((void *) image, 0, 8*64);
                throw new NotImplementedException();
            else
            {					// part of moon dark
                int[] array = new int[64];

                int reverse = 0;
                int sizex = NEW_MOON_PHASE / 2 - phase;
                if (phase > NEW_MOON_PHASE)
                {
                    sizex += NEW_MOON_PHASE;
                    reverse = 1;
                }

                int counter = 0;
                int flag = 1;
                int x = 0;
                int y = 32;
                int xpos = 32;
                float aa = (float)sizex * sizex;
                float bb = (float)32 * 32;
                float d1 = bb - aa * 32 + aa / 4.0f;
                while (aa * ((float)y - 0.5f) > bb * ((float)x + 1.0f))
                {
                    if (d1 < 0.0f)
                    {
                        d1 += bb * ((float)(x << 1) + 3.0f);
                        x++;
                        xpos++;
                    }
                    else
                    {
                        if (flag != 0)
                        {
                            array[counter++] = xpos;
                            flag = 0;
                        }
                        d1 += bb * ((float)(x << 1) + 3.0f) + aa * ((float)(-y << 1) + 2.0f);
                        x++;
                        xpos++;
                        y--;
                        array[counter++] = xpos;
                    }
                }

                float x1 = (float)x + 0.5f;
                float y1 = (float)y - 1.0f;
                float d2 = bb * x1 * x1 + aa * y1 * y1 - aa * bb;
                while (y > 0)
                {
                    if (d2 < 0.0f)
                    {
                        if (flag != 0)
                        {
                            array[counter++] = xpos;
                            flag = 0;
                        }
                        d2 += bb * ((float)(x << 1) + 2.0f) + aa * ((float)(-y << 1) + 3.0f);
                        x++;
                        xpos++;
                        y--;
                    }
                    else
                    {
                        d2 += aa * ((float)(-y << 1) + 3.0f);
                        y--;
                    }
                    array[counter++] = xpos;
                }

                int j;
                for (j = 0; j < 32; j++) array[63 - j] = array[j];
                if (sizex < 0)
                {
                    for (j = 0; j < 64; j++) array[j] = 64 - array[j];
                }

                int row, col, col1;
                int start, stop;
                byte[] dest = image;
                for (row = 0; row < 64; row++)
                {
                    if (reverse != 0)
                    {
                        start = 0; stop = array[row];
                    }
                    else
                    {
                        start = array[row]; stop = 64;
                    }
                    int col2 = 0;
                    for (col = 0; col < 8; col++)
                    {
                        byte c = 0;
                        for (col1 = 0; col1 < 8; col1++)
                        {
                            c <<= 1;
                            if ((col2 < start) || (col2 >= stop)) c |= 1;
                            col2++;
                        }
                        *dest++ = c;
                    }
                }
            }
#endif 
            throw new NotImplementedException();
        }



        public void CalculateMoonPhase()
        {
            int angle = 0;
            float dy = SunCoord.y - MoonCoord.y;
            if (dy > 1.0f)
            {
                dy -= 2.0f;
                angle = -4096;
            }
            else if (dy < -1.0f)
            {
                dy += 2.0f;
                angle = 4096;
            }
            if (dy < 0) angle -= 4096;
            else angle += 4096;

            float dz = SunCoord.z - MoonCoord.z;
            angle += (int)(grmath.radtoangle((float)Math.Atan2(dz, dy)));
            //MI moon phase fix
#if NOTHING
	dy = MoonCoord.z;
	dz = MoonCoord.y;
#else
            dy = -MoonCoord.y;
            dz = -MoonCoord.z;
#endif
            int angle1 = (int)(grmath.radtoangle((float)Math.Atan2(dz, dy)));
            angle -= angle1;

            RotateMoonMask(angle);
        }

        public void RotateMoonMask(int angle)
        {
#if TODO
            float sine, cosine;
            grmath.glGetSinCos(out sine, out cosine, -angle);

            int[] u1 = new int[3], v1 = new int[3];

            float c32, s32;
            c32 = 65536.0f * 32.0f * cosine;
            s32 = 65536.0f * 32.0f * sine;
            u1[0] = (int)(-c32 + s32 + 65536.0f * 32.0f);
            v1[0] = (int)(-s32 - c32 + 65536.0f * 32.0f);
            u1[1] = (int)(c32 + s32 + 65536.0f * 32.0f);
            v1[1] = (int)(s32 - c32 + 65536.0f * 32.0f);
            u1[2] = (int)(-c32 - s32 + 65536.0f * 32.0f);
            v1[2] = (int)(-s32 + c32 + 65536.0f * 32.0f);

            int i, j, k;
            byte[] dest = CurrentMoonPhaseMask;
            int uu = u1[0];
            int vv = v1[0];
            int duu = (u1[1] - u1[0]) >> 6;
            int duv = (v1[1] - v1[0]) >> 6;
            int dvu = (u1[2] - u1[0]) >> 6;
            int dvv = (v1[2] - v1[0]) >> 6;
            for (i = 0; i < 64; i++)
            {
                int uuu = uu;
                int vvv = vv;
                for (j = 0; j < 8; j++)
                {
                    byte c1 = 0;
                    for (k = 0; k < 8; k++)
                    {
                        int tu = uuu >> 16;
                        int tv = vvv >> 16;
                        c1 <<= 1;
                        uuu += duu;
                        vvv += duv;
                        if (tu >= 0 && tu < 64 && tv >= 0 && tv < 64)
                        {
                            int l = (tv << 3) + (tu >> 3);
                            byte c = (byte)(1 << (7 - (tu & 7)));
                            if (MoonPhaseMask[l] & c) c1 |= 1;
                        }
                    }
                    *dest++ = c1;
                }
                uu += dvu;
                vv += dvv;
            }
       #endif 
            throw new NotImplementedException();
        }
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
        {
            if (IMoonPitch < 0) return 1.0f;
            float alpha = 0.0f;
            if (ISunPitch >= 0)
            {
                int pitch = GetMoonPitch();
                int yaw = GetMoonYaw();
                int pitch1 = GetSunPitch();
                int yaw1 = GetSunYaw();
                float sin1, sin2, cos1, cos2, cos3;
                grmath.glGetSinCos(out sin1, out cos1, pitch);
                grmath.glGetSinCos(out sin2, out cos2, pitch1);
                cos3 = (float)grmath.glGetCosine(yaw - yaw1);
                alpha = sin1 * sin2 + cos1 * cos2 * cos3;
                alpha -= SunGlareCosine;
                alpha *= SunGlareFactor;
                if (alpha > 1.0f) alpha = 1.0f;
                else if (alpha < 0.0f) alpha = 0.0f;
                if (sin2 > 0.0f)
                {
                    //sin2 *= 4.0f;
                    alpha += sin2;
                }
            }
            if (glare < 0.0f) glare = 0.0f;
            else if (glare > 1.0f) glare = 1.0f;
            alpha = 1.0f - (alpha + glare);
            if (alpha < 0.0f) alpha = 0.0f;
            else if (alpha > 1.0f) alpha = 1.0f;
            return alpha;
        }

        public int CalculateMoonPercent()
        {
            if (MoonPhase == -1)
                MoonPhase = (int)(CStar.GetMoonPhase() * MOON_PHASE_SIZE);
            return MoonPhase;
        }

        public void CreateMoonPhase(string src, string dest)
        {
#if TODO
            int i, j, k;
            byte[] mask = CurrentMoonPhaseMask;
            for (i = 0; i < 64; i++)
            {
                for (j = 0; j < 8; j++)
                {
                    byte c = *mask++;
                    for (k = 0; k < 8; k++)
                    {
                        byte c1 = *src++;
#if USE_TRANSPARENT_MOON
				if (c1 && !(c & 0x80)) c1 = 0;
#else
                        if (c1 && !(c & 0x80)) c1 += 48;
#endif
                        c <<= 1;
                        *dest++ = c1;
                    }
                }
            }
#endif
            throw new NotImplementedException();
        }


        public DWORD GetRainColor() { return RainColor; }
        public DWORD GetSnowColor() { return SnowColor; }
        public Tcolor GetLightningColor() { return LightningColor; }
        public int GetTotalTimeOfDay() { return TotalTimeOfDay; }
        public DWORD GetTimeOfDay(int i) { return TimeOfDay[i].Time; }
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


        protected static void TimeUpdateCallback(object self)
        {
            ((CTimeOfDay)self).UpdateSkyProperties();
        }
        protected void UpdateSkyProperties()
        {
#if TODO
            int i, c, n;
            TimeOfDayStruct tod, ntod;
            uint now;
            float t;

            // Convert from time since clock start to time since midnight
            now = TheTimeManager.GetTimeOfDay();

            uint curtime = TheTimeManager.GetClockTime();
            TheStar.SetUniversalTime(curtime);
            if (curtime != 0)
            {
                if (lastMoonTime == 0 || lastMoonTime > curtime || ((curtime - lastMoonTime) > 60 * 60 * 1000))
                {
                    lastMoonTime = curtime;
                    MoonPhase = -1;
                    MoonPhase = CalculateMoonPercent();
                    CreateMoonPhaseMask(MoonPhaseMask, MoonPhase);
                }
            }


            // Identify the Current time step in the TOD table
            for (i = 0; i < TotalTimeOfDay; i++)
            {
                if (TimeOfDay[i].Time > now) break;
            }
            if (i == 0) c = TotalTimeOfDay - 1;
            else if (i >= TotalTimeOfDay) c = TotalTimeOfDay - 1;
            else c = i - 1;

            // Identify the Next time step in the TOD table
            n = c + 1;
            if (n >= TotalTimeOfDay) n = 0;

            // This should only happen if the table has less than two entries
            if (c == n) return;


            // Get pointers to the current and next TOD table entries
            tod = &(TimeOfDay[c]);
            ntod = &(TimeOfDay[n]);


            // No two table entries should have the same time stamp
            c = tod.Time;
            n = ntod.Time;
            ShiAssert(c != n);

            // Calculate the time between the two table entries
            if (n < c) n += MSEC_PER_DAY;
            n -= c;

            // Calculate the time between now and the current table entry
            if (now < (DWORD)c) now += MSEC_PER_DAY;
            c = now - c;

            // Calculate the interpolation control variable "t"
            t = (float)c / (float)n;

            // Set all our variable from the first record
            SetVar(tod);

            // Add in deltas toward the second record
            SkyColor.r += t * (ntod.SkyColor.r - SkyColor.r);
            SkyColor.g += t * (ntod.SkyColor.g - SkyColor.g);
            SkyColor.b += t * (ntod.SkyColor.b - SkyColor.b);
            HazeSkyColor.r += t * (ntod.HazeSkyColor.r - HazeSkyColor.r);
            HazeSkyColor.g += t * (ntod.HazeSkyColor.g - HazeSkyColor.g);
            HazeSkyColor.b += t * (ntod.HazeSkyColor.b - HazeSkyColor.b);
            GroundColor.r += t * (ntod.GroundColor.r - GroundColor.r);
            GroundColor.g += t * (ntod.GroundColor.g - GroundColor.g);
            GroundColor.b += t * (ntod.GroundColor.b - GroundColor.b);
            HazeGroundColor.r += t * (ntod.HazeGroundColor.r - HazeGroundColor.r);
            HazeGroundColor.g += t * (ntod.HazeGroundColor.g - HazeGroundColor.g);
            HazeGroundColor.b += t * (ntod.HazeGroundColor.b - HazeGroundColor.b);
            TextureLighting.r += t * (ntod.TextureLighting.r - TextureLighting.r);
            TextureLighting.g += t * (ntod.TextureLighting.g - TextureLighting.g);
            TextureLighting.b += t * (ntod.TextureLighting.b - TextureLighting.b);
            VisColor.r += t * (ntod.VisColor.r - VisColor.r);
            VisColor.g += t * (ntod.VisColor.g - VisColor.g);
            VisColor.b += t * (ntod.VisColor.b - VisColor.b);

            Tcolor Color;
            Color = tod.RainColor;
            Color.r += t * (ntod.RainColor.r - tod.RainColor.r);
            Color.g += t * (ntod.RainColor.g - tod.RainColor.g);
            Color.b += t * (ntod.RainColor.b - tod.RainColor.b);
            RainColor = MakeColor(&Color);

            Color = tod.SnowColor;
            Color.r += t * (ntod.SnowColor.r - tod.SnowColor.r);
            Color.g += t * (ntod.SnowColor.g - tod.SnowColor.g);
            Color.b += t * (ntod.SnowColor.b - tod.SnowColor.b);
            SnowColor = MakeColor(&Color);

            LightningColor.r += t * (ntod.LightningColor.r - tod.LightningColor.r);
            LightningColor.g += t * (ntod.LightningColor.g - tod.LightningColor.g);
            LightningColor.b += t * (ntod.LightningColor.b - tod.LightningColor.b);

            Ambient += t * (ntod.Ambient - Ambient);
            Diffuse += t * (ntod.Diffuse - Diffuse);
            MinVis += t * (ntod.MinVis - MinVis);

            float ra, dec, az, alt;
            TheStar.GetSunRaDec(&ra, &dec);
            TheStar.ConvertPosition(ra, dec, &az, &alt);
            TheStar.SetSunPosition(az, alt);
            TheStar.ConvertCoord(ra, dec, &SunCoord.x, &SunCoord.y, &SunCoord.z);
            ISunYaw = FloatToInt32(radtoangle(az));
            ISunPitch = FloatToInt32(radtoangle(alt));
            TheStar.GetMoonRaDec(&ra, &dec);
            TheStar.ConvertPosition(ra, dec, &az, &alt);
            TheStar.SetMoonPosition(az, alt);
            TheStar.ConvertCoord(ra, dec, &MoonCoord.x, &MoonCoord.y, &MoonCoord.z);
            IMoonYaw = FloatToInt32(radtoangle(az));
            IMoonPitch = FloatToInt32(radtoangle(alt));

            if (ISunPitch < 256 || ISunPitch > (8192 - 256))
            {
                // Adjust the light level for the moon
                // (original levels are assumed to have been for a full moon)
                // At new moon and/or moon rise/set, we darken by at most 1/2
                // (at little more, actually, since the SIN can become negative just as the moon sets/rises)
                float t1 = (float)abs(NEW_MOON_PHASE - CalculateMoonPercent());
                t1 = (t1 / NEW_MOON_PHASE) * (float)Math.Sin(alt);//angletorad(IMoonPitch));
                t1 = (1.0f + t1) / 2.0f;
                if (t1 < 0.45f) t1 = 0.45f;		// limit the darkness level
                HazeGroundColor.r *= t1;
                HazeGroundColor.g *= t1;
                HazeGroundColor.b *= t1;
                TextureLighting.r *= t1;
                TextureLighting.g *= t1;
                TextureLighting.b *= t1;
                Ambient *= t1;
                Diffuse *= t1;
            }

            // Update the positions and effects of the celstial objects
            StarIntensity += t * (ntod.StarIntensity - StarIntensity);
            TheStar.UpdateStar();

            // Adjust the light level and convert to green in NVG mode
            if (GetNVGmode())
            {
#if NOTHING
		SkyColor.r			= 0.0f;
		SkyColor.g			= NVG_SKY_LEVEL;
		SkyColor.b			= 0.0f;
		HazeSkyColor.r		= 0.0f;
		HazeSkyColor.g		= NVG_SKY_LEVEL;
		HazeSkyColor.b		= 0.0f;
		GroundColor.r		= 0.0f;
		GroundColor.g		= NVG_SKY_LEVEL;
		GroundColor.b		= 0.0f;
		HazeGroundColor.r	= 0.0f;
		HazeGroundColor.g	= NVG_SKY_LEVEL;
		HazeGroundColor.b	= 0.0f;
		Ambient				= NVG_LIGHT_LEVEL;
		Diffuse				= 0.0f;
		TextureLighting.r	= 0.0f;
		TextureLighting.g	= NVG_LIGHT_LEVEL;
		TextureLighting.b	= 0.0f;
#endif
            }

#endif
            throw new NotImplementedException();
        }
        
        protected int ReadTODFile(StreamReader @in, TimeOfDayStruct[] tods, int countflag = 0)
        {
            float fvar;
            int total;
            string buffer;
            int cnt = 0;

            total = 0;
            while (true)
            {
                TimeOfDayStruct tod = tods[cnt];
                FileScanner.fscanf(@in, "%s", out buffer);
                buffer = buffer.ToUpperInvariant();
                if (buffer== "ZZZZ")
                {
                    SetDefaultColor(ref tod.RainColor, tod.HazeGroundColor);
                    SetDefaultColor(ref tod.SnowColor, tod.HazeGroundColor);
                    SetDefaultColor(ref tod.VisColor, tod.HazeSkyColor);
                    break;
                }
                else if (buffer=="TIME")
                {
                    DWORD ivar1, ivar2, ivar3;
                    if (total != 0)
                    {
                        SetDefaultColor(ref tod.RainColor, tod.HazeGroundColor);
                        SetDefaultColor(ref tod.SnowColor, tod.HazeGroundColor);
                        SetDefaultColor(ref tod.VisColor, tod.HazeSkyColor);
                    }
                    total++;
                    if (countflag == 0)
                    {
                        cnt++;
                        tod = tods[cnt];
                    }

                    FileScanner.fscanf(@in, "%ld:%ld:%ld", out ivar1, out ivar2, out ivar3);
                    ivar1 *= 3600000;
                    ivar2 *= 60000;
                    ivar3 *= 1000;
                    tod.Time = ivar1 + ivar2 + ivar3;
                    tod.Flag = 0;
                    tod.StarIntensity = 0.0f;
                    tod.LightningColor.r = tod.LightningColor.g = tod.LightningColor.b = 1;
                    tod.RainColor.r = tod.RainColor.g = tod.RainColor.b = 1;
                    tod.SnowColor.r = tod.SnowColor.g = tod.SnowColor.b = 1;
                    tod.VisColor.r = tod.VisColor.g = tod.VisColor.b = -1;
                    tod.MinVis = 0.1f;
                }
                else if (buffer == "SUNTILT")
                {
                    FileScanner.fscanf(@in, "%f", out fvar);
                    ISunTilt = grmath.glConvertFromDegree(fvar);
                }
                else if (buffer== "MOONTILT")
                {
                    FileScanner.fscanf(@in, "%f", out fvar);
                    IMoonTilt = grmath.glConvertFromDegree(fvar);
                }

        //---- ignore these ----
                else if (buffer == "SUNYAW")
                {
                    FileScanner.fscanf(@in, "%f", out fvar);
                }
                else if (buffer=="MOONYAW")
                {
                    FileScanner.fscanf(@in, "%f", out fvar);
                }
                //----------------------
                else if (buffer=="HAZESUNSETCOLOR")
                {
                    FileScanner.fscanf(@in, "%f %f %f", out HazeSunsetColor.r, out HazeSunsetColor.g, out HazeSunsetColor.b);
                }
                else if (buffer=="HAZESUNRISECOLOR")
                {
                    FileScanner.fscanf(@in, "%f %f %f", out HazeSunriseColor.r, out HazeSunriseColor.g, out HazeSunriseColor.b);
                }
                else if (buffer=="SKYCOLOR")
                {
                    FileScanner.fscanf(@in, "%f %f %f", out tod.SkyColor.r, out tod.SkyColor.g, out tod.SkyColor.b);
                }
                else if (buffer=="HAZESKYCOLOR")
                {
                    FileScanner.fscanf(@in, "%f %f %f", out tod.HazeSkyColor.r, out tod.HazeSkyColor.g, out tod.HazeSkyColor.b);
                }
                else if (buffer=="GROUNDCOLOR")
                {
                    FileScanner.fscanf(@in, "%f %f %f", out tod.GroundColor.r, out tod.GroundColor.g, out tod.GroundColor.b);
                }
                else if (buffer=="HAZEGROUNDCOLOR")
                {
                    FileScanner.fscanf(@in, "%f %f %f", out tod.HazeGroundColor.r, out tod.HazeGroundColor.g, out tod.HazeGroundColor.b);
                }
                else if (buffer=="TEXTURELIGHTING")
                {
                    FileScanner.fscanf(@in, "%f %f %f", out tod.TextureLighting.r, out tod.TextureLighting.g, out tod.TextureLighting.b);
                }
                else if (buffer=="AMBIENT")
                {
                    FileScanner.fscanf(@in, "%f", out tod.Ambient);
                }
                else if (buffer=="DIFFUSE")
                {
                    FileScanner.fscanf(@in, "%f", out tod.Diffuse);
                }
                else if (buffer=="SUNPITCH")
                {
                    FileScanner.fscanf(@in, "%f", out tod.SunPitch);
                    tod.SunPitch = grmath.glConvertFromDegreef(tod.SunPitch);
                    tod.Flag |= GL_TIME_OF_DAY_USE_SUN;
                }
                else if (buffer=="MOONPITCH")
                {
                    FileScanner.fscanf(@in, "%f", out tod.MoonPitch);
                    tod.MoonPitch = grmath.glConvertFromDegreef(tod.MoonPitch);
                    tod.Flag |= GL_TIME_OF_DAY_USE_MOON;
                }
                else if (buffer=="STAR")
                {
                    tod.StarIntensity = 1.0f;
                }
                // JPO additions)
                else if (buffer=="RAINCOLOR")
                {
                    FileScanner.fscanf(@in, "%f %f %f", out tod.RainColor.r, out tod.RainColor.g, out tod.RainColor.b);
                }
                else if (buffer=="SNOWCOLOR")
                {
                    FileScanner.fscanf(@in, "%f %f %f", out tod.SnowColor.r, out tod.SnowColor.g, out tod.SnowColor.b);
                }
                else if (buffer=="LIGHTNINGCOLOR")
                {
                    FileScanner.fscanf(@in, "%f %f %f", out tod.LightningColor.r, out tod.LightningColor.g, out tod.LightningColor.b);
                }
                else if (buffer=="MINVISIBILITY")
                {
                    FileScanner.fscanf(@in, "%f", out tod.MinVis);
                }
                else if (buffer=="VISCOLOR")
                {
                    FileScanner.fscanf(@in, "%f %f %f", out tod.VisColor.r, out tod.VisColor.g, out tod.VisColor.b);
                }
                else
                {
                    throw new FormatException("Ignoring TOD item " + buffer);
                }
            }
            return total;
        }
        
        protected void SetDefaultColor(ref Tcolor col, Tcolor defcol)
        {
            if (col.r == -1)
                col = defcol;
        }

        protected void SetVar(TimeOfDayStruct tod)
        {
            SkyColor = tod.SkyColor;
            HazeSkyColor = tod.HazeSkyColor;
            GroundColor = tod.GroundColor;
            HazeGroundColor = tod.HazeGroundColor;
            TextureLighting = tod.TextureLighting;
            Ambient = tod.Ambient;
            Diffuse = tod.Diffuse;
            Flag = tod.Flag;
            StarIntensity = tod.StarIntensity;
            RainColor = MakeColor(tod.RainColor);
            SnowColor = MakeColor(tod.SnowColor);
            LightningColor = tod.LightningColor;
            MinVis = tod.MinVis;
            VisColor = tod.VisColor;
        }
        protected static DWORD MakeColor(Tcolor col)
        {
            return (DWORD)(
            ((int)(col.r * 255.9f) & 0xFF) |
            (((int)(col.g * 255.9f) & 0xFF) << 8) |
            (((int)(col.b * 255.9f) & 0xFF) << 16) |
            0xff000000);
        }

        public static CTimeOfDay TheTimeOfDay;
        private static CStar TheStar = new CStar();
    }
}
