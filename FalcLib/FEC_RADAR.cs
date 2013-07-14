
namespace FalconNet.FalcLib
{
    // Radar Modes
    public enum FEC_RADAR
    {
        FEC_RADAR_OFF = 0x00,	   	// Radar always off
        FEC_RADAR_SEARCH_100 = 0x01,	   	// Search Radar - 100 % of the heading (always on)
        FEC_RADAR_SEARCH_1 = 0x02,	   	// Search Sequence #1
        FEC_RADAR_SEARCH_2 = 0x03,	   	// Search Sequence #2
        FEC_RADAR_SEARCH_3 = 0x04,	   	// Search Sequence #3
        FEC_RADAR_AQUIRE = 0x05,	   	// Aquire Mode (looking for a target)
        FEC_RADAR_GUIDE = 0x06,	   	// Missile in flight. Death is imminent
        FEC_RADAR_CHANGEMODE = 0x07	   	// Missile in flight. Death is imminent
    }
}
