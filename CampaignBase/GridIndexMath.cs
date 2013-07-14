using FalconNet.Campaign;
using FalconNet.Common;
using FalconNet.Common.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridIndex = System.Int16;
using BIG_SCALAR = System.Single;

namespace FalconNet.CampaignBase
{
    public static class GridIndexMath
    {
        private const float GRID_SIZE_FT = Phyconst.FEET_PER_KM; // Grid size, in feet (standard sim unit)
        private const float GRID_SIZE_KM = 1.0F; // Grid size, in km (standard campaign unit)
        public const int GRIDZ_SCALE_FACTOR = 10; // How many feet per pt of Z.
        private const float OffsetToMiddle = GRID_SIZE_FT / 2.0F;

        public static int DistSqu(GridIndex ox, GridIndex oy, GridIndex dx, GridIndex dy)
        {
            return ((ox - dx) * (ox - dx) + (oy - dy) * (oy - dy));
        }

        public static float Distance(GridIndex ox, GridIndex oy, GridIndex dx, GridIndex dy)
        {
            return (float)Math.Sqrt((float)((ox - dx) * (ox - dx) + (oy - dy) * (oy - dy))) * GRID_SIZE_KM;
        }

        /// <summary>
        /// returns distance in feet
        /// </summary>
        /// <param name="ox"></param>
        /// <param name="oy"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static float Distance(float ox, float oy, float dx, float dy)
        {
            return (float)Math.Sqrt(((ox - dx) * (ox - dx) + (oy - dy) * (oy - dy)));
        }

        public static float DistSqu(float ox, float oy, float dx, float dy)
        {
            return (float)((ox - dx) * (ox - dx) + (oy - dy) * (oy - dy));
        }

        public static int GetBearingDeg(float x, float y, float tx, float ty)
        {
            float theta;
            theta = (float)Math.Atan((tx - x) / (ty - y));
            return (int)(Angle.RadianToDegree(theta));
        }

        public static int GetRangeFt(float x, float y, float tx, float ty)
        {
            return (int)(Distance(x, y, tx, ty));
        }

        public static long PackXY(GridIndex x, GridIndex y)
        {
            long t = (UInt16)x | ((UInt16)y << 16);
            return t;
        }

        public static void UnpackXY(long n, out GridIndex x, out GridIndex y)
        {
            x = (GridIndex)(n & 0xFFFF);
            y = (GridIndex)((n >> 16) & 0xFFFF);
        }

        public static void Trim(ref GridIndex x, ref GridIndex y)
        {
            if (x < 0)
                x = 0;

            if (x >= CampTerrStatic.Map_Max_X)
                x = (GridIndex)(CampTerrStatic.Map_Max_X - 1);

            if (y < 0)
                y = 0;

            if (y >= CampTerrStatic.Map_Max_Y)
                y = (GridIndex)(CampTerrStatic.Map_Max_Y - 1);
        }

        public static float AngleTo(GridIndex ox, GridIndex oy, GridIndex tx, GridIndex ty)
        {
            int dx, dy;
            float deg;

            dx = tx - ox;
            dy = ty - oy;

            if (dx == 0 && dy == 0)
                return 0.0F;

            deg = (float)Math.Atan2((float)dx, (float)dy);

            if (deg < 0)
                deg += (float)(2.0 * Math.PI);

            return deg;
        }

        public static CampaignHeading DirectionTo(GridIndex ox, GridIndex oy, GridIndex tx, GridIndex ty)
        {
            int dx, dy;
            float deg;
            CampaignHeading h;

            dx = tx - ox;
            dy = ty - oy;

            if (dx == 0 && dy == 0)
            {
                return CampaignHeading.Here;
            }

            deg = (float)Math.Atan2((float)dx, (float)dy);

            if (deg < 0)
            {
                deg += (float)(2.0 * Math.PI);
            }

            deg += .3839F; // Shift by 22 degress;
            h = (int)((deg * 1.273F) % 8); // convert from 6.28 = 360 (2 PI) to 8=360;
            return h;
        }

        public static CampaignHeading DirectionTo(GridIndex ox, GridIndex oy,
                                                    GridIndex tx, GridIndex ty,
                                                    GridIndex cx, GridIndex cy)
        {
            int dx, dy;
            GridIndex nx, ny;
            float d, td;

            dx = tx - ox;
            dy = ty - oy;

            if (cx == tx && cy == ty)
                return CampaignHeading.Here;

            td = Distance(ox, oy, tx, ty);
            d = Distance(ox, oy, cx, cy);

            if (d < 1.9 || d > td)
                return DirectionTo(cx, cy, tx, ty);

            d += 1.9F;
            nx = (short)(ox + (GridIndex)(d / td * dx));
            ny = (short)(oy + (GridIndex)(d / td * dy));
            return DirectionTo(cx, cy, nx, ny);
        }

        public static int OctantTo(GridIndex ox, GridIndex oy, GridIndex tx, GridIndex ty)
        {
            if (ox > tx)
            {
                if (oy > ty)
                {
                    if (ox - tx > oy - ty)
                        return 5;
                    else
                        return 4;
                }
                else
                {
                    if (ox - tx > ty - oy)
                        return 6;
                    else
                        return 7;
                }
            }
            else
            {
                if (oy > ty)
                {
                    if (tx - ox > oy - ty)
                        return 2;
                    else
                        return 3;
                }
                else
                {
                    if (tx - ox > ty - oy)
                        return 1;
                    else
                        return 0;
                }
            }
        }

        public static int OctantTo(float ox, float oy, float tx, float ty)
        {
            if (oy > ty)
            {
                if (ox > tx)
                {
                    if (oy - ty > ox - tx)
                        return 5;
                    else
                        return 4;
                }
                else
                {
                    if (oy - ty > tx - ox)
                        return 6;
                    else
                        return 7;
                }
            }
            else
            {
                if (ox > tx)
                {
                    if (ty - oy > ox - tx)
                        return 2;
                    else
                        return 3;
                }
                else
                {
                    if (ty - oy > tx - ox)
                        return 1;
                    else
                        return 0;
                }
            }
        }

        public static CampaignTime TimeToArrive(float distance, float speed)
        {
            if (distance == 0)
            {
                return 0;
            }

            if (speed == 0)
            {
                return 0xffffffff;
            }

            return (CampaignTime)(distance * CampaignTime.CampaignHours / speed);
        }

        // Speed should be in [grid units]/[hour]
        public static CampaignTime TimeTo(GridIndex x, GridIndex y, GridIndex tx, GridIndex ty, int speed)
        {
            return TimeToArrive(Distance(x, y, tx, ty), (float)speed);
        }

        public static CampaignTime TimeBetween(GridIndex x, GridIndex y, GridIndex tx, GridIndex ty, int speed)
        {
            float d;

            d = Distance(x, y, tx, ty);
            return (CampaignTime)(d / speed * CampaignTime.CampaignHours);
        }

        public static float GridToSim(GridIndex x)
        {
            return (float)(x * GRID_SIZE_FT) + OffsetToMiddle;
        }

        public static GridIndex SimToGrid(float x)
        {
            // sfr: added fast float function
            return (GridIndex)(x / GRID_SIZE_FT);
        }

        /* sfr: VERY IMPORTANT NOTE:
        * Sim and Grid coordinates are XY inverted. So this convertion is correct.
        * Boths starts at lower left though.
        * This explains why in sim coords Z- is up. (right hand from vertical to horizontal, thumbs down
        */
        // Convert from Grid to sim coordinate systems
        public static void ConvertGridToSim(GridIndex x, GridIndex y, out vector pos)
        {
            pos = new vector();
            pos.x = GridToSim(y);
            pos.y = GridToSim(x);
        }

        // Converts Sim to Grid coordinates
        public static void ConvertSimToGrid(vector pos, out GridIndex x, out GridIndex y)
        {
            x = SimToGrid(pos.y);
            y = SimToGrid(pos.x);
        }

        public static int GetAltitudeFromLevel(int level, int seed)
        {
            // This is a no-brainer.
            if (level == 0)
            {
                return 0;
            }

            // Just in case this was set already (ie: a mission replan)
            // Kinda hackish - I should really fix this eventually
            if (level >= ALT_LEVELS)
            {
                return level * GRIDZ_SCALE_FACTOR;
            }

            return MinAltAtLevel[level] + LevelIncrement[level] * (seed % IncrementMax[level]);
        }

        public static BIG_SCALAR ConvertGridToSimZ(GridIndex gz)
        {
            return (BIG_SCALAR)(gz * -GRIDZ_SCALE_FACTOR);
        }


        private const int ALT_LEVELS = 5;
        // This stuff is used to convert from altitude to altitude level and vice-versa,
        // as well as a way to randomize altitudes within a level reasonably
        private static readonly int[] MaxAltAtLevel = new int[ALT_LEVELS] { 99, 4999, 19999, 39999, 99999 };
        private static readonly int[] MinAltAtLevel = new int[ALT_LEVELS] { 0, 100, 5000, 20000, 40000 };
        private static readonly int[] LevelIncrement = new int[ALT_LEVELS] { 0, 0, 1000, 2000, 2500 };
        private static readonly int[] IncrementMax = new int[ALT_LEVELS] { 0, 1, 8, 8, 8 };
    }
}
