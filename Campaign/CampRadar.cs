using System;
//using FalconNet.Sim;

namespace FalconNet.Campaign
{
	public class RadarRangeClass
	{
		// KCK: This class in placeholder until I get around to actually coding this stuff

		public const int NUM_RADAR_ARCS = 8;			// How many arcs each radar data will store
		public const float  ALT_FOR_RANGE_DETERMINATION = 2500.0F;		// when we return range, it'll be how far we can
		// see something at this altitude
		public const float MINIMUM_RADAR_RATIO = 0.022F;		// Minimum ratio (about 1 deg angle)
	
		public float[]	detect_ratio = new float[NUM_RADAR_ARCS];	// tan(detection_angle)- basically, ratio of 
		// altitude to distance above which we can see


		public int GetNumberOfArcs ()
		{
			return NUM_RADAR_ARCS;
		}

		public float GetArcRatio (int anum)
		{
			return detect_ratio [anum];
		}
		
		// 2001-03-14 MODIFIED BY S.G. detect_ratio[anum] CAN BE ZERO. NEED TO CHECK FOR THAT FIRST
		//		float GetArcRange (int anum)						{ return ALT_FOR_RANGE_DETERMINATION/detect_ratio[anum]; };
		public float GetArcRange (int anum)
		{
			return detect_ratio [anum] != 0.0f ? ALT_FOR_RANGE_DETERMINATION / detect_ratio [anum] : 0.0f;
		}

		public int CanDetect (float dx, float dy, float dz)
		{
			int oct;

			oct = FindStatic.OctantTo (0.0F, 0.0F, dx, dy);
			if ((dz * dz) > ((dx * dx) + (dy * dy)) * (detect_ratio [oct] * detect_ratio [oct]))
				return 1;
			return 0;
		}

		public float GetRadarRange (float dx, float dy, float dz)
		{
			int oct;

			oct = FindStatic.OctantTo (0.0F, 0.0F, dx, dy);
			return dz / detect_ratio [oct];
		}

		public void GetArcAngle (int anum, ref float a1, ref float a2)
		{ 
			a1 = anum * (360 / NUM_RADAR_ARCS) * Phyconst.DTR;
			a2 = (anum + 1) * (360 / NUM_RADAR_ARCS) * Phyconst.DTR;
		}

		public void SetArcRatio (int anum, float ratio)
		{
			detect_ratio [anum] = ratio;
		}
	}
}

