using System;

namespace FalconNet.Graphics
{
	// TODO:  Roll all this into the drawable class itself and eliminate the middle man.
	public class WeatherCell
	{
#if TODO
		public WeatherCell ()
		{
			drawable = null;
		}
		//public ~WeatherCell()	{ Debug.Assert( drawable == null ); };

		public void	Setup (int row, int col, ObjectDisplayList* objList)
		{
			DWORD code;
			float thickness;

			lightning = false;
			rainFactor = 0;
			visDistance = 1;
			ltimer = 0;

			// Construct the overcast tiling code
			code = 0;
			if (TheWeather.TypeAt (r, c) >= FIRST_OVC_TYPE)
				code = 1;
			if (TheWeather.TypeAt (r, c + 1) >= FIRST_OVC_TYPE)
				code |= 2;
			if (TheWeather.TypeAt (r + 1, c) >= FIRST_OVC_TYPE)
				code |= 4;
			if (TheWeather.TypeAt (r + 1, c + 1) >= FIRST_OVC_TYPE)
				code |= 8;

			// If we're not overcast, see if we should do a scattered tile
			if (!code) {
				if ((TheWeather.TypeAt (r - 1, c) >= FIRST_OVC_TYPE) ||
			(TheWeather.TypeAt (r, c - 1) >= FIRST_OVC_TYPE) ||
			(TheWeather.TypeAt (r + 2, c + 1) >= FIRST_OVC_TYPE) ||
			(TheWeather.TypeAt (r + 1, c + 2) >= FIRST_OVC_TYPE)) {
					// We want to draw a border scud tile
					code = 16 + rand () % 4;
				}
			}

			// Decide which type of cloud cell to create (if any)
			if (!code) {
				// Clear sky
				thickness = 0.0f;
				drawable = null;
			} else {
				// Get our thickness
				thickness = TheWeather.ThicknessAt (r, c);

				// Create the drawable overcast object
				drawable = new DrawableOvercast (code, r, c);
				if (!drawable) {
					ShiError ("Failed to create the overcast drawable object");
				}

				// Add the drawable object to the object manager's list
				objList.InsertObject (drawable);
				rainFactor = TheWeather.RainAt (r, c);
				lightning = TheWeather.LightningAt (r, c);
				visDistance = TheWeather.VisRangeAt (r, c);
				if (lightning) {
					ltimer = vuxRealTime + (rand () % 30) * 1000;
				}
			}


			// Store our top and bottom z values
			base_ = TheWeather.BaseAt (r, c);
//	top		= base -= thickness;				// -Z is up!
			if (thickness <= 0)
				top = base_;
			else
				top = TheWeather.TopsAt (r, c); // JPO - think this is the same as the cloud drawing routine.
			// it seems to give better results anyway.

		}

		public void	Cleanup (ObjectDisplayList* objList)
		{
			Debug.Assert (objList);

			if (drawable) {
				// Remove the visual object for the object manager list
				objList.RemoveObject (drawable);
				delete drawable;
				drawable = null;
			}
		}

		public void	Reset (int row, int col, ObjectDisplayList* objList)
		{
			Setup (row, col, objList);
		}

		public void	UpdateForDrift (int cellRow, int cellCol)
		{
			if (drawable)
				drawable.UpdateForDrift (cellRow, cellCol);
		}

		public float	GetBase ()
		{
			return base_;
		}

		public float	GetTop ()
		{
			return top;
		}

		public float	GetNWtop ()
		{
			return drawable ? drawable.GetNWtop () : top;
		}

		public float	GetNEtop ()
		{
			return drawable ? drawable.GetNEtop () : top;
		}

		public float	GetSWtop ()
		{
			return drawable ? drawable.GetSWtop () : top;
		}

		public float	GetSEtop ()
		{
			return drawable ? drawable.GetSEtop () : top;
		}

		public float	GetNWbottom ()
		{
			return drawable ? drawable.GetNWbottom () : base_;
		}

		public float	GetNEbottom ()
		{
			return drawable ? drawable.GetNEbottom () : base_;
		}

		public float	GetSWbottom ()
		{
			return drawable ? drawable.GetSWbottom () : base_;
		}

		public float	GetSEbottom ()
		{
			return drawable ? drawable.GetSEbottom () : base_;
		}

		public bool	IsEmpty ()
		{
			return drawable == null;
		}

		public float	GetAlpha (float x, float y)
		{
			return drawable ? drawable.GetTextureAlpha (x, y) : 0.0f;
		}

		public float GetRainFactor ()
		{
			return rainFactor;
		}

		public float GetVisibility ()
		{
			return visDistance;
		}

		public bool GetLightning ()
		{
			if (lightning == true && ltimer < vuxRealTime) {
				ltimer = vuxRealTime + (rand () % 30) * 1000;
				return true;
			}
			return false;
		}
  
		protected DrawableOvercast	drawable;
		protected float	base_;
		protected float	top;
// jpo additions
		protected float	visDistance;
		protected float rainFactor;
		protected bool lightning;
		protected float ltimer;
#endif
	}
}

