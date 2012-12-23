using FalconNet.Common;
using FalconNet.GrLib;
using System;
using System.IO;

namespace FalconNet.Graphics
{
    public struct StarRecord
    {
        public float ra, dec;
        public int color;
    }

    public struct StarCoord
    {
        public float x, y, z;
        public int color;
        public float az, alt;
        public int flag;
    }

    public struct StarData
    {
        public int totalstar, totalcoord;
        public StarRecord star;
        public StarCoord coord;
    }
    public class CStar
    {
        public const int STAR_BEHIND_SUN = 1;
        public const int STAR_BEHIND_MOON = 2;

        private const float EPSILON = 1e-6f;
        private const float elonge = 278.833540f;     /* Ecliptic longitude of the Sun at epoch 1980.0 */
        private const float elongp = 282.596403f;     /* Ecliptic longitude of the Sun at perigee */
        private const float eccent = 0.016718f;       /* Eccentricity of Earth's orbit */
        private const float mmlong = 64.975464f;      /* Moon's mean longitude at the epoch */
        private const float mmlongp = 349.383063f;     /* Mean longitude of the perigee at the epoch */
        private const float mlnode = 151.950429f;     /* Mean longitude of the node at the epoch */
        private const float minc = 5.145396f;       /* Inclination of the Moon's orbit */
        private const float MAXRANGE = 2.5f;

        protected static float Latitude, Longitude, sinLatitude, cosLatitude;
        protected static float UniversalTime, UniversalTimeDegree;
        protected static float deltaJulian, CurrentJulian, Julian1980, Julian2000;
        protected static float LocalSiderialTime;
        protected static int Year, Month, Day, ExtraDay;
        protected static bool mustSetLocalSiderialTime, mustSetdeltaJulian;
        protected static int[] DaysInMonth = new int[12];
        protected static StarData[] CurrentStarData;
        protected static float SunAz, SunAlt, MoonAz, MoonAlt;
        protected static float Horizon, HorizonRange, IntensityRange;
        protected static int minStarIntensity;

        protected static void CalculateDeltaJulian()
        {
            int year = Year;
            int month = Month;
            int day = Day;
            CalculateDate(ref day, ref month, ref year, ExtraDay);
            CurrentJulian = Julian(year, month, day + UniversalTime);
            deltaJulian = CurrentJulian - Julian2000;
            mustSetdeltaJulian = false;
        }

        protected static void CalculateLocalSiderialTime()
        {
            LocalSiderialTime = GetRangeRad(grmath.degtorad(100.46f) + grmath.degtorad(0.985647f) * deltaJulian + Longitude + UniversalTimeDegree);
            mustSetLocalSiderialTime = false;
        }

        protected static bool CalculateStarCoord(float ra, float dec, StarCoord star)
        {
            if (mustSetdeltaJulian) CalculateDeltaJulian();
            if (mustSetLocalSiderialTime) CalculateLocalSiderialTime();
            float HourAngle = GetRangeRad(LocalSiderialTime - ra);
            float sinDEC = (float)Math.Sin(dec);
            float cosDEC = (float)Math.Cos(dec);
            float cosHA = (float)Math.Cos(HourAngle);
            float sinHA = (float)Math.Sin(HourAngle);
            float sinALT = sinDEC * sinLatitude + cosDEC * cosLatitude * cosHA;
            float altitude = (float)Math.Asin(sinALT);
            if (altitude < Horizon) return false;
            float cosALT = (float)Math.Cos(altitude);
            float azimuth = (float)Math.Acos((sinDEC - sinALT * sinLatitude) / (cosALT * cosLatitude));
            if (sinHA >= 0) azimuth = (float)(2 * Math.PI - azimuth);
            float sinAZ = (float)Math.Sin(azimuth);
            float cosAZ = (float)Math.Cos(azimuth);

            star.az = (float)azimuth;
            star.alt = (float)altitude;
            star.flag = 0;

            // X = North Y = East Z = Down
            star.x = (float)(cosAZ * cosALT);
            star.y = (float)(sinAZ * cosALT);
            star.z = (float)(-sinALT);
            return true;
        }

        protected static float Julian(int year, int month, float day)
        {
            if (month > 2)
            {
                month = month - 3;
            }
            else
            {
                month = month + 9;
                year--;
            }
            int c = year / 100;
            year -= (100 * c);
            c = (c * 146097) >> 2;
            year = (year * 1461) >> 2;
            month = (month * 153 + 2) / 5;
            day += (c + year + month + 1721119);
            return (day);
        }

        protected static float FixAngle(float N)
        {
            return N - 360.0f * (float)Math.Floor(N / 360.0f);
        }
        protected static float Kepler(float m, float ecc)
        {
            float e, delta;

            e = m = grmath.degtorad(m);
            do
            {
                delta = e - ecc * (float)Math.Sin(e) - m;
                e -= (delta / (1 - ecc * (float)Math.Cos(e)));
            } while (Math.Abs(delta) > EPSILON);
            return e;
        }

        protected static float GetRange(float angle)
        {
            while (angle >= 360.0f) angle -= 360.0f;
            while (angle < 0) angle += 360.0f;
            return angle;
        }

        protected static float GetRangeRad(float angle)
        {
            while (angle >= Math.PI * 2) angle -= (float)(Math.PI * 2);
            while (angle < 0) angle += (float)(Math.PI * 2);
            return angle;
        }

        protected static int InsideRange(float starpos, float pos)
        {
            if (starpos < pos - grmath.degtorad(MAXRANGE)) return 0;
            if (starpos > pos + grmath.degtorad(MAXRANGE)) return 0;
            return 1;
        }

#if STAND_ALONE
static int GetMaxDay (int month, int year);
static void ConvertLocation (char *string, float loc, char c);

public:
static int GetTime (int *hour, int *minute, float *second, float timezone=0.0f);
static void GetDateTime (char *string, float timezone=0.0f);
static void GetLatitude (char *string);
static void GetLongitude (char *string);
static void GetLocation (char *string);
static float ConvertUnit (float deg, float min = 0.0f, float sec = 0.0f);
static void UpdateTime (int hour, int minute, float second = 0.0f);
static void UpdateTime (float hour);
static void UpdateTime (unsigned int mseconds);
static void CalculateSunCoord (float *x, float *y, float *z);
static void CalculateMoonCoord (float *x, float *y, float *z);
#endif


        public CStar() { }
        //tODO public virtual ~CStar() { Cleanup (); }

        public static int Setup(string starfile, float maxmagnitude = 12.0f)
        {
            Cleanup();
            StreamReader @in = new StreamReader(starfile);
            if (@in == null) return 1;

            string buffer;

            int totalcons = 0, totalstar = 0;
            float minmag = 0.0F, maxmag = 0.0F;
            float minint, maxint;

            minint = 0.5f;
            maxint = 1.0f;
            FileScanner.fscanf(@in, "%s", out buffer);		// StarInfo
            while (true)
            {
                FileScanner.fscanf(@in, "%s", out buffer);
                buffer = buffer.ToUpperInvariant();
                if (buffer == "ZZZZ") break;
                else if (buffer == "TOTALCONSTELLATION")
                {
                    FileScanner.fscanf(@in, "%d", out totalcons);
                }
                else if (buffer == "TOTALSTAR")
                {
                    FileScanner.fscanf(@in, "%d", out totalstar);
                }
                else if (buffer == "MINMAG")
                {
                    FileScanner.fscanf(@in, "%f", out minmag);
                }
                else if (buffer == "MAXMAG")
                {
                    FileScanner.fscanf(@in, "%f", out maxmag);
                    if (maxmag > maxmagnitude) maxmag = maxmagnitude;
                }
                else if (buffer == "MININTENSITY")
                {
                    FileScanner.fscanf(@in, "%f", out minint);
                }
                else if (buffer == "MAXINTENSITY")
                {
                    FileScanner.fscanf(@in, "%f", out maxint);
                }
            }
            try
            {
                StarData data = new StarData();
            }
            catch (Exception e)
            {
                @in.Close();
                return 2;
            }
#if TODO
	StarRecord[] star = new StarRecord[totalstar];
	if (!star) {
		FREE(data);
		fclose (@in);
		return 2;
	}
	data . star = star;
	data . totalstar = totalstar;

	float	deltamag = Math.Max(0.01F, maxmag - minmag);
	float	deltaint = (maxint - minint) / deltamag;

	StarRecord	*curstar = star;
	int i, j;
	float	mag;
	for (i=0; i < totalcons;i++) {
		FileScanner.fscanf (@in, "%s", buffer);		// Constellation
		FileScanner.fscanf (@in, "%[^\n]", buffer);
		FileScanner.fscanf (@in, "%s", buffer);		// TotalStar
		FileScanner.fscanf (@in, "%d", &totalstar);
		for (j=0; j < totalstar;j++) {
			FileScanner.fscanf (@in, "%s", buffer);	// Mag
			FileScanner.fscanf (@in, "%f", &mag);
			FileScanner.fscanf (@in, "%s", buffer);	// RaDec
			FileScanner.fscanf (@in, "%f %f", &curstar . ra, &curstar . dec);
			curstar . ra = hourtorad(curstar . ra);
			curstar . dec = grmath.degtorad(curstar . dec);
			FileScanner.fscanf (@in, "%s", buffer);	// ID
			FileScanner.fscanf (@in, "%[^\n]", buffer);
			if (mag < maxmagnitude) {
				mag = (mag - minmag) * deltaint;
				if (mag > 1.0f) mag = 1.0f;	// just in case
				curstar . color = FloatToInt32((1.0f - mag) * 255.0f);
				curstar++;
			}
			else data . totalstar--;
		}
	}
	fclose (@in);

	star = NEWARRAY (StarRecord, data . totalstar);
	if (!star) {
		FREE(data . star);
		FREE(data);
		return 2;
	}
	memcpy (star, data . star, sizeof(StarRecord) * data . totalstar);
	FREE(data . star);
	data . star = star;

	StarCoord *coord = NEWARRAY (StarCoord, data . totalstar);
	if (!coord) {
		FREE(data . star);
		FREE(data);
		return 2;
	}
	data . coord = coord;
	data . totalcoord = 0;

	int max;
	for (i=0; i < data . totalstar; i++) {
		max = i;
		for (j=i + 1; j < data . totalstar; j++) {
			if (data . star[j].color >  data . star[max].color) max = j;
		}
		if (i != max) {
			StarRecord tempstar = data . star[max];
			data . star[max] = data . star[i];
			data . star[i] = tempstar;
		}
	}

	minStarIntensity = 0;
	CurrentStarData = data;
	return 0;
#endif
            throw new NotImplementedException();
        }

        public static void Cleanup()
        {
#if TODO
            if (CurrentStarData != null)
            {
                FREE(CurrentStarData.coord);
                FREE(CurrentStarData.star);
                FREE(CurrentStarData);
                CurrentStarData = null;
            }
#endif
            throw new NotImplementedException();
        }
        public static StarData[] GetStarData() { return CurrentStarData; }

        public static int LeapYear(int year)
        {
            int leap = 0;
            if ((year & 3) == 0)
            {
                if ((year % 400) == 0) leap = 1;
                else if ((year % 100) != 0) leap = 1;
            }
            return leap;
        }

        public static int GetTotalDay(int month, int year)
        {
            if (month > 2)
            {
                if (LeapYear(year) != 0) DaysInMonth[1] = 29;
                else DaysInMonth[1] = 28;
            }
            int i;
            int days = 0;
            for (i = 0; i < month; i++) days += DaysInMonth[i];
            return days;
        }

        public static float ConvertHour(int hour, int min = 0, float sec = 0.0f)
        {
            return hour + min / 60.0f + sec / 3600.0f;
        }

        public static void CalculateDate(ref int day, ref int month, ref int year, int extraday = 0)
        {
            if (extraday == 0) return;
            int d = day + extraday;
            int m = month;
            int y = year;
            if (m > 1)
            {
                d += GetTotalDay(m - 1, y);
                m = 1;
            }
            while (true)
            {
                int maxday = 365 + LeapYear(y);
                if (d <= maxday) break;
                else
                {
                    y++;
                    d -= maxday;
                }
            }
            while (true)
            {
                if (d <= DaysInMonth[m - 1]) break;
                d -= DaysInMonth[m - 1];
                m++;
            }
            day = d;
            month = m;
            year = y;
        }


        public static void SetDate(int day, int month = 1, int year = 2000)
        {
            Year = year;
            Month = month;
            Day = day;
            ExtraDay = 0;
            mustSetdeltaJulian = true;
        }

        public static void SetUniversalTime(uint mseconds)
        {
            float hour = mseconds * (1.0f / (24.0f * 3600000.0f));
            SetUniversalTime(hour);
        }
        public static void SetUniversalTime(float curtime)
        {
            UniversalTime = curtime;
            if (UniversalTime >= 1.0f)
            {
                ExtraDay = (int)(UniversalTime);
                UniversalTime -= ExtraDay;
            }
            UniversalTimeDegree = grmath.degtorad(360.0f) * UniversalTime;
            CalculateDeltaJulian();
            CalculateLocalSiderialTime();
        }

        public static void SetUniversalTime(int hour, int minute, float second)
        {
            float curtime = ConvertHour(hour, minute, second) / 24.0f;
            SetUniversalTime(curtime);
        }

        public static void SetLocation(float latitude = 38.0f, float longitude = 0.0f)
        {
            Latitude = grmath.degtorad(latitude);
            Longitude = grmath.degtorad(longitude);
            sinLatitude = (float)Math.Sin(Latitude);
            cosLatitude = (float)Math.Cos(Latitude);
            mustSetLocalSiderialTime = true;
        }

        public static void SetHorizon(float horizon, float range = 0.0f)
        {
            Horizon = horizon;
            HorizonRange = horizon + range;
            IntensityRange = 1.0f;
            if (range != 0) IntensityRange /= range;
        }

        public static void UpdateStar()
        {
#if TODO
            StarRecord star = CurrentStarData.star;
            StarCoord coord = CurrentStarData.coord;
            CurrentStarData.totalcoord = 0;
            int i;
            for (i = 0; i < CurrentStarData.totalstar; i++)
            {
                //TODO star++;
                star = CurrentStarData[i];
                if (star.color < minStarIntensity) continue;	// skip dim star
                if (CalculateStarCoord(star.ra, star.dec, coord))
                {
                    if (InsideRange(coord.az, SunAz) && InsideRange(coord.alt, SunAlt))
                        coord.flag |= STAR_BEHIND_SUN;
                    if (InsideRange(coord.az, MoonAz) && InsideRange(coord.alt, MoonAlt))
                        coord.flag |= STAR_BEHIND_SUN;
                    if (coord.alt < HorizonRange)
                    {
                        float intensity = (coord.alt - Horizon) * IntensityRange;
                        coord.color = FloatToInt32(intensity * star.color);
                    }
                    else coord.color = star.color;
                    coord++;
                    CurrentStarData.totalcoord++;
                }
            }
#endif
            throw new NotImplementedException();
        }

        public static void GetSunRaDec(out float ra, out float dec)
        {
            float g = GetRangeRad(grmath.degtorad(357.528f) + grmath.degtorad(0.9856003f) * deltaJulian);
            float g2 = g * 2;
            float L = GetRangeRad(grmath.degtorad(280.461f) + grmath.degtorad(0.9856474f) * deltaJulian);
            float lambda = GetRangeRad(L + grmath.degtorad(1.915f) * (float)Math.Sin(g) + grmath.degtorad(0.02f) * (float)Math.Sin(g2));
            float epsilon = GetRangeRad(grmath.degtorad(23.439f) - grmath.degtorad(0.0000004f) * deltaJulian);
            float sinlambda = (float)Math.Sin(lambda);
            ra = GetRangeRad((float)Math.Atan2(Math.Cos(epsilon) * sinlambda, Math.Cos(lambda)));
            dec = (float)Math.Asin(Math.Sin(epsilon) * sinlambda);
        }
        public static void GetMoonRaDec(out float ra, out float dec)
        {
            float t = grmath.degtorad(deltaJulian) / 36525.0f;
            float l = GetRangeRad(grmath.degtorad(218.32f) + 481267.883f * t)
                        + grmath.degtorad(6.29f) * (float)Math.Sin(GetRangeRad(grmath.degtorad(134.9f) + 477198.85f * t))
                        - grmath.degtorad(1.27f) * (float)Math.Sin(GetRangeRad(grmath.degtorad(259.2f) - 413335.38f * t))
                        + grmath.degtorad(0.66f) * (float)Math.Sin(GetRangeRad(grmath.degtorad(235.7f) + 890534.23f * t))
                        + grmath.degtorad(0.21f) * (float)Math.Sin(GetRangeRad(grmath.degtorad(269.9f) + 954397.7f * t))
                        - grmath.degtorad(0.19f) * (float)Math.Sin(GetRangeRad(grmath.degtorad(357.5f) + 35999.05f * t))
                        - grmath.degtorad(0.11f) * (float)Math.Sin(GetRangeRad(grmath.degtorad(186.6f) + 966404.05f * t));
            l = GetRangeRad(l);
            float bm = grmath.degtorad(5.13f) * (float)Math.Sin(GetRangeRad(grmath.degtorad(93.3f) + 483202.03f * t))
                        + grmath.degtorad(0.28f) * (float)Math.Sin(GetRangeRad(grmath.degtorad(228.2f) + 960400.87f * t))
                        - grmath.degtorad(0.28f) * (float)Math.Sin(GetRangeRad(grmath.degtorad(318.3f) + 6003.18f * t))
                        - grmath.degtorad(0.17f) * (float)Math.Sin(GetRangeRad(grmath.degtorad(217.6f) - 407332.2f * t));
            float gp = grmath.degtorad(0.9508f)
                        + grmath.degtorad(0.0518f) * (float)Math.Cos(GetRangeRad(grmath.degtorad(134.9f) + 477198.85f * t))
                        + grmath.degtorad(0.0095f) * (float)Math.Cos(GetRangeRad(grmath.degtorad(259.2f) - 413335.38f * t))
                        + grmath.degtorad(0.0078f) * (float)Math.Cos(GetRangeRad(grmath.degtorad(235.7f) + 890534.23f * t))
                        + grmath.degtorad(0.0028f) * (float)Math.Cos(GetRangeRad(grmath.degtorad(269.9f) + 954397.7f * t));
            //	float sdia = 0.2725f * gp;
            float rm = 1.0f / (float)Math.Sin(gp);
            float cosbm = (float)Math.Cos(bm);
            float xg = rm * (float)Math.Cos(l) * cosbm;
            float yg = rm * (float)Math.Sin(l) * cosbm;
            float zg = rm * (float)Math.Sin(bm);
            float ecl = grmath.degtorad(23.4393f) - grmath.degtorad(3.563e-7f) * deltaJulian;
            float cosecl = (float)Math.Cos(ecl);
            float sinecl = (float)Math.Sin(ecl);
            float xe = xg;
            float ye = yg * cosecl - zg * sinecl;
            float ze = yg * sinecl + zg * cosecl;
            ra = GetRangeRad((float)Math.Atan2(ye, xe));
            dec = (float)Math.Atan(ze / Math.Sqrt(xe * xe + ye * ye));
        }
        public static void ConvertCoord(float ra, float dec, out  float x, out float y, out float z)
        {
            float HourAngle = GetRangeRad(LocalSiderialTime - ra);
            float sinDEC = (float)Math.Sin(dec);
            float cosDEC = (float)Math.Cos(dec);
            float cosHA = (float)Math.Cos(HourAngle);
            float sinHA = (float)Math.Sin(HourAngle);
            float sinALT = sinDEC * sinLatitude + cosDEC * cosLatitude * cosHA;
            float altitude = (float)Math.Asin(sinALT);
            float cosALT = (float)Math.Cos(altitude);
            float azimuth = (float)Math.Acos((sinDEC - sinALT * sinLatitude) / (cosALT * cosLatitude));
            if (sinHA >= 0) azimuth = (float)(2 * Math.PI - azimuth);
            float sinAZ = (float)Math.Sin(azimuth);
            float cosAZ = (float)Math.Cos(azimuth);

            // X = North Y = East Z = Down
            x = (float)(cosAZ * cosALT);
            y = (float)(sinAZ * cosALT);
            z = (float)(-sinALT);
        }
        public static void ConvertPosition(float ra, float dec, out float az, out  float alt)
        {
            float HourAngle = GetRangeRad(LocalSiderialTime - ra);
            float sinDEC = (float)Math.Sin(dec);
            float cosDEC = (float)Math.Cos(dec);
            float cosHA = (float)Math.Cos(HourAngle);
            float sinHA = (float)Math.Sin(HourAngle);
            float sinALT = sinDEC * sinLatitude + cosDEC * cosLatitude * cosHA;
            float altitude = (float)Math.Asin(sinALT);
            float cosALT = (float)Math.Cos(altitude);
            float azimuth = (float)Math.Acos((sinDEC - sinALT * sinLatitude) / (cosALT * cosLatitude));
            if (sinHA >= 0) azimuth = (float)(2 * Math.PI - azimuth);

            az = (float)azimuth;
            alt = (float)altitude;
        }

        public static void CalculateSunPosition(out float az, out float alt)
        {
            float ra, dec;
            GetSunRaDec(out ra, out dec);
            ConvertPosition(ra, dec, out SunAz, out SunAlt);
            az = SunAz;
            alt = SunAlt;
        }

        public static void CalculateMoonPosition(out float az, out float alt)
        {
            float ra, dec;
            GetMoonRaDec(out ra, out dec);
            ConvertPosition(ra, dec, out MoonAz, out MoonAlt);
            az = MoonAz;
            alt = MoonAlt;
        }

        public static float GetMoonPhase()
        {
            float Day = CurrentJulian - Julian1980;
            float N = FixAngle((360.0f / 365.2422f) * Day);
            float M = FixAngle(N + elonge - elongp);
            float Ec = Kepler(M, eccent);
            Ec = (float)Math.Sqrt((1 + eccent) / (1 - eccent)) * (float)Math.Tan(Ec / 2);
            Ec = grmath.radtodeg(2) * (float)Math.Atan(Ec);
            float Lambdasun = FixAngle(Ec + elongp);
            float ml = FixAngle(13.1763966f * Day + mmlong);
            float MM = FixAngle(ml - 0.1114041f * Day - mmlongp);
            //	float MN = FixAngle (mlnode - 0.0529539f * Day);
            float Ev = 1.2739f * (float)Math.Sin(grmath.degtorad(2 * (ml - Lambdasun) - MM));
            float sinM = (float)Math.Sin(grmath.degtorad(M));
            float Ae = 0.1858f * sinM;
            float A3 = 0.37f * sinM;
            float MmP = grmath.degtorad((MM + Ev - Ae - A3));
            float mEc = 6.2886f * (float)Math.Sin(MmP);
            float A4 = 0.214f * (float)Math.Sin(2 * MmP);
            float lP = ml + Ev + mEc - Ae + A4;
            float V = 0.6583f * (float)Math.Sin(grmath.degtorad(2) * (lP - Lambdasun));
            float MoonAge = lP + V - Lambdasun;
            MoonAge = GetRange(MoonAge + 180.0f);
            return MoonAge / 360.0f;
        }

        public static void RemoveDimStar(float minintensity)
        {
            minStarIntensity = (int)(minintensity * 255.0f);
        }
        public static void SetSunPosition(float az, float alt) { SunAz = az; SunAlt = alt; }
        public static void SetMoonPosition(float az, float alt) { MoonAz = az; MoonAlt = alt; }
        public static void GetSunPosition(out float az, out float alt) { az = SunAz; alt = SunAlt; }
        public static void GetMoonPosition(out float az, out float alt) { az = MoonAz; alt = MoonAlt; }
    }
}
