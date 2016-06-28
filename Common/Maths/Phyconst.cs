using System;

namespace FalconNet.Common
{
    /*--------------------*/
    /* Physical Constants */
    /*--------------------*/
    public static class Phyconst
    {
        public const float RTD = 57.2957795F;
        public const float DTR = 0.01745329F;
        public const float VIS_RANGE = 50000.0F;
        public const float BIGGEST_RANDOM_NUMBER = 32767.0F;
        public const double BOLTZ = 1.38E-23;
        public const float PI = (float)Math.PI;//3.141592654F;
        public const float HALF_PI = 1.570796326795F;
        public const float FOUR_PI_CUBED = 1984.402F;

        public const float FEET_PER_KM = 3279.98f;
        public const float FEET_PER_METER = 3.27998f;
        public const float FT_TO_METERS = 0.30488F;
        public const float FT_TO_NM = 0.0001646F;
        public const float FT_TO_KM = 0.0003048F;
        public const float NM_TO_FT = 6076.211F;
        public const float KM_TO_FT = 3279.98f;      /* This define is also in constant.h  FEET_PER_KM */
        public const float NM_TO_KM = 1.85224731f;
        public const float KM_TO_NM = 0.53988471f;

        public const float FIVE_G_TURN_RATE = 15.9F;
        public const float LIGHTSPEED = 983319256.3F;      /* feet per sec */
        public const float TASL = 518.7F;
        public const float PASL = 2116.22F;
        public const float RHOASL = 0.0023769F;
        public const float AASL = 1116.44F;
        public const float AASLK = 661.48F;
        public const float GRAVITY = 32.177F;
        public const float FTPSEC_TO_KNOTS = 0.592474F;
        public const float KNOTS_TO_FTPSEC = 1.687836F;
        public const float MILS_TO_DEGREES = 0.057296F;
        public const float DTMR = 17.45F;
        public const float MRTD = 0.057296F;
        public const float RTMR = 1000.0F;
        public const float MRTR = 0.001F;
        public const float KPH_TO_FPS = 0.9111053F;
        public const float EARTH_RADIUS_NM = 3443.92228F;   //Mean Equatorial Radius
        public const float EARTH_RADIUS_FT = 2.09257E7F;    //Mean Equatorial Radius
        public const float NM_PER_MINUTE = 1.00018F;        //Nautical Mile per Minute of Latitude (Or Longitude at Equator)
        public const float MINUTE_PER_NM = 0.99820F;        //Minutes of Latitude (or Longitude at Equator) per NM
        public const float FT_PER_MINUTE = 6087.03141F; //Feet per Minute of Latitude (Or Longitude at Equator)							
        public const float MINUTE_PER_FT = 1.64283E-4F; //Minutdes of Latitude (or Longitude at Equator) per foot
        public const float FT_PER_DEGREE = FT_PER_MINUTE * 60.0F;
        public const float DEG_TO_MIN = 60;
        public const float MIN_TO_DEG = 0.01666666F;
        public const float MIN_TO_SEC = DEG_TO_MIN;
        public const float SEC_TO_MIN = MIN_TO_DEG;
        public const float DEG_TO_SEC = 3600;
        public const float SEC_TO_DEG = 2.7777777E-4F;
        public const float SEC_TO_MSEC = 1000;
        public const float MSEC_TO_SEC = 0.001f;
    }
}

