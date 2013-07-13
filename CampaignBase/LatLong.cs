
namespace FalconNet.CampaignBase
{
    public struct LatLong
    {
        public sbyte EastOfGreenwich;    // East Longitude is "Negative"
        public sbyte SouthOfEquator;     // South Latitude is "Negative"
        public sbyte DegreesOfLatitude;  // 0 to 90
        public byte MinutesOfLatitude;  // 0 to 60
        public byte SecondsOfLatitude;  // 0 to 60
        public byte DegreesOfLongitude; // 0 to 180
        public byte MinutesOfLongitude; // 0 to 60
        public byte SecondsOfLongitude; // 0 to 60
    }
}
