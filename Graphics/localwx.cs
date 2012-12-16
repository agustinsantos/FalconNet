using System;
using DWORD = System.Int16;

namespace FalconNet.Graphics
{
    /***************************************************************************\
        LocalWX.h
        Scott Randolph
        July 19, 1996

        Manage the "deaggregated" clouds in a given local.
    \***************************************************************************/



    public class LocalWeather
    {
        public LocalWeather() { cellArray = null; }
        //TODO public ~LocalWeather() { if (cellArray != null) Cleanup(); }

        public void Setup(float visRange, ObjectDisplayList objList)
        { throw new NotImplementedException(); }
        public void Cleanup()
        { throw new NotImplementedException(); }
        public void Update(Tpoint position, DWORD currentTime)
        { throw new NotImplementedException(); }

        public float GetAreaFloor() { return cloudBase; }
        public float GetAreaCeiling() { return cloudTops; }

        public float LineOfSight(Tpoint p1, Tpoint p2)
        { throw new NotImplementedException(); }

        public float GetMaxRange() { return range; }

        public float GetLocalCloudTops()
        { throw new NotImplementedException(); }
        public float GetRainFactor()
        { throw new NotImplementedException(); }
        public float GetVisibility()
        { throw new NotImplementedException(); }
        public bool HasLightning()
        { throw new NotImplementedException(); }


        protected void UpdateForDrift()
        { throw new NotImplementedException(); }
        protected void SlideLocalCellsVertical(int vx)
        { throw new NotImplementedException(); }
        protected void SlideLocalCellsHorizontal(int vy)
        { throw new NotImplementedException(); }
        protected void RebuildList()
        { throw new NotImplementedException(); }
        protected void ComputeAreaFloorAndCeiling()
        { throw new NotImplementedException(); }

        protected bool horizontalEdgeTest(int row, int col, float x, float y, float z, bool downward)
        { throw new NotImplementedException(); }
        protected bool verticalEdgeTest(int row, int col, float x, float y, float z, bool downward)
        { throw new NotImplementedException(); }
        protected float LineSquareIntersection(int row, int col, float x1, float y1, float z1, float dx, float dy, float dz, bool downward)
        { throw new NotImplementedException(); }
        protected WeatherCell GetLocalCell(int r, int c)
        {
            if (Math.Abs(r) > cellRange) return null;
            if (Math.Abs(c) > cellRange) return null;
            return cellArray[(r + cellRange) * rowLen + c + cellRange];
        }

        protected ObjectDisplayList objMgr;	// Object list which will sort and draw cloud objects

        protected float centerX, centerY;	// What is our current center point	(world space)
        protected int cellRow, cellCol;	// What is our current center point	(cell coordinates)

        protected int rowShiftHistory;	// Checked against TheWeather to detect map shifts
        protected int colShiftHistory;	// Checked against TheWeather to detect map shifts

        protected float range;				// How far away can clouds be seen
        protected int cellRange;			// How many weather cells into the distance do we need
        protected int rowLen;				// How many cells accross (and high) is the cell array

        protected float cloudBase;			// Z value at base of local clouds (-Z is up)
        protected float cloudTops;			// Z value at tops of local clouds (-Z is up)

        WeatherCell[] cellArray;		// ROWxCOL array of local cells
    }

}
