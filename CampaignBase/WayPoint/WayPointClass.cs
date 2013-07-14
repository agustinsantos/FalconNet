using System;
using System.IO;
using FalconNet.Common;
using FalconNet.VU;
using VU_BYTE = System.Byte;
using GridIndex = System.Int16;
using BIG_SCALAR = System.Single;
using FalconNet.Common.Maths;
using log4net;
using FalconNet.F4Common;

namespace FalconNet.CampaignBase
{
    // ============================================
    // WayPoint Class
    // ============================================

    public class WayPointClass
    {
#if USE_SH_POOLS
   
      public // Overload new/delete to use a SmartHeap fixed size pool
      public void *operator new(size_t size) { Debug.Assert( size == sizeof(WayPointClass) ); return MemAllocFS(pool);	};
      public void operator delete(void *mem) { if (mem) MemFreeFS(mem); };
      public static void InitializeStorage()	{ pool = MemPoolInitFS( sizeof(WayPointClass), 200, 0 ); };
      public static void ReleaseStorage()	{ MemPoolFree( pool ); };
      public static MEM_POOL	pool;
#endif

        private GridIndex GridX;					// Waypoint's X,Y and Z coordinates (in km from southwest corner)
        private GridIndex GridY;
        private short GridZ;					// Z is in 10s of feet
        private CampaignTime Arrive;
        private CampaignTime Depart;					// This is only used for loiter waypoints 
        private VU_ID TargetID;
        private WPAction Action;
        private WPAction RouteAction;
        private byte Formation;
        private byte TargetBuilding;
        private WPFlags Flags;					// Various wp flags
        private short Tactic;					// Tactic to use here

        protected float Speed;
        protected WayPointClass PrevWP;					// Make this one public for kicks..
        protected WayPointClass NextWP;					// Make this one public for kicks..
        private BIG_SCALAR SimX;
        private BIG_SCALAR SimY;
        private BIG_SCALAR SimZ;

        public WayPointClass()
        {
            GridX = GridY = GridZ = 0;
            SimX = SimY = SimZ = 0.0f;
            Arrive = new CampaignTime(0);
            Depart = new CampaignTime(0);
            Action = 0;
            RouteAction = 0;
            Formation = 0;
            TargetBuilding = 255;
            TargetID = VU_ID.vuNullId;
            Flags = 0;
            Speed = 0.0f;
            PrevWP = null;
            NextWP = null;
            Tactic = 0;
        }

        public WayPointClass(GridIndex x, GridIndex y, int alt, int speed, CampaignTime arr, CampaignTime station, WPAction action, WPFlags flags)
        {
            GridX = x;
            GridY = y;
            // sfr: update SimXYZ too
            vector pos;
            GridIndexMath.ConvertGridToSim(GridX, GridY, out pos);
            SimX = pos.x;
            SimY = pos.y;

            if (alt > GridIndexMath.GRIDZ_SCALE_FACTOR)
            {
                GridZ = (short)(alt / GridIndexMath.GRIDZ_SCALE_FACTOR);
            }
            else
            {
                GridZ = (short)alt;
            }

            SimZ = GridIndexMath.ConvertGridToSimZ(GridZ);

            Arrive = arr;
            Depart = arr + station;
            Action = RouteAction = action;
            TargetBuilding = 255;
            Formation = 0;
            Flags = flags;
            Speed = (float)speed;
            PrevWP = null;
            NextWP = null;
            Tactic = 0;
        }

#if TODO
        public WayPointClass(ref VU_BYTE[] stream)
        { throw new NotImplementedException(); }
        public WayPointClass(FileStream fp)
        { throw new NotImplementedException(); }

        public WayPointClass(byte[] bytes, ref int offset, int version)
        {

            haves = bytes[offset];
            offset++;
            GridX = BitConverter.ToInt16(bytes, offset);
            offset += 2;
            GridY = BitConverter.ToInt16(bytes, offset);
            offset += 2;
            GridZ = BitConverter.ToInt16(bytes, offset);
            offset += 2;
            Arrive = BitConverter.ToUInt32(bytes, offset);
            offset += 4;
            Action = bytes[offset];
            offset++;
            RouteAction = bytes[offset];
            offset++;
            Formation = bytes[offset];
            offset++;

            if (version < FLAGS_WIDENED_AT_VERSION)
            {
                Flags = BitConverter.ToUInt16(bytes, offset);
                offset += 2;
            }
            else
            {
                Flags = BitConverter.ToUInt32(bytes, offset);
                //TODO: SOME NEW FIELD, 2 BYTES LONG, COMES HERE, OR ELSE FLAGS IS EXPANDED IN AT LATEST V73 (PROBABLY EARLIER?) TO BE 4 BYTES LONG INSTEAD OF 2 BYTES LONG
                offset += 4;
            }
            if ((haves & WP_HAVE_TARGET) != 0)
            {
                TargetID = new VU_ID();
                TargetID.num_ = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
                TargetID.creator_ = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
                TargetBuilding = bytes[offset];
                offset++;
            }
            else
            {
                TargetID = new VU_ID();
                TargetBuilding = 255;
            }
            if ((haves & WP_HAVE_DEPTIME) != 0)
            {
                Depart = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
            }
            else
            {
                Depart = Arrive;
            }

        }
        public int SaveSize()
        { throw new NotImplementedException(); }
        public int Save(ref VU_BYTE[] stream)
        { throw new NotImplementedException(); }
        public int Save(FileStream fp)
        { throw new NotImplementedException(); }
#endif
        // These functions are intended for general use
        public void SetWPTarget(VU_ID e)
        {
            TargetID = e;
        }

        public void SetWPTargetBuilding(byte t)
        {
            TargetBuilding = t;
        }

        public void SetWPAction(WPAction a)
        {
            Action = a;
        }

        public void SetWPRouteAction(WPAction a)
        {
            RouteAction = a;
        }

        public void SetWPFormation(int f)
        {
            Formation = (byte)f;
        }

        public void SetWPFlags(WPFlags f)
        {
            Flags = f;
        }

        public void SetWPFlag(WPFlags f)
        {
            Flags |= f;
        }

        public void UnSetWPFlag(WPFlags f)
        {
            Flags &= ~(f);
        }

        public void SetWPTactic(int f)
        {
            Tactic = (short)f;
        }

        public VU_ID GetWPTargetID()
        {
            return TargetID;
        }

        //TODO public CampBaseClass GetWPTarget() { return (CampBaseClass)VuDatabase.vuDatabase.Find(TargetID); }
        public byte GetWPTargetBuilding()
        {
            return TargetBuilding;
        }

        public WPAction GetWPAction()
        {
            return Action;
        }

        public WPAction GetWPRouteAction()
        {
            return RouteAction;
        }

        public int GetWPFormation()
        {
            return (int)Formation;
        }

        public WPFlags GetWPFlags()
        {
            return Flags;
        }

        public int GetWPTactic()
        {
            return (int)Tactic;
        }

        public WayPointClass GetNextWP()
        {
            return NextWP;
        }

        public WayPointClass GetPrevWP()
        {
            return PrevWP;
        }

        public void SetNextWP(WayPointClass next)
        {
            float dx, dy, dz, sx, sy, sz, delta_x, delta_y, time, dist, speed;

            if (NextWP != null)
            {
                log.Warn("Trying to Set Next WP on a waypoint that already has a next waypoint");
            }
            else
            {
                NextWP = next;

                if (next != null)
                {
                    next.PrevWP = this;

                    if (NextWP != null)
                    {
                        NextWP.GetLocation(out dx, out dy, out dz);
                        GetLocation(out sx, out sy, out sz);

                        delta_x = dx - sx;
                        delta_y = dy - sy;

                        dist = (float)Math.Sqrt(delta_x * delta_x + delta_y * delta_y);
                        time = (float)(NextWP.GetWPArrivalTime() - GetWPArrivalTime()) / CampaignTime.CampaignHours; // Hours

                        if (time != 0.0)
                        {
                            // JB 010413 CTD
                            speed = (dist / time) / Phyconst.NM_TO_FT;
                        }
                        else
                        {
                            // JB 010413 CTD
                            speed = 0.0f; // JB 010413 CTD
                        }

                        if (speed > 0)
                        {
                            NextWP.Speed = speed;
                        }
                    }
                }
            }
        }

        public void SetPrevWP(WayPointClass prev)
        {
            if (PrevWP != null)
            {
                log.Warn("Trying to Set Prev WP on a waypoint that already has a previous waypoint");
            }
            else
            {
                PrevWP = prev;

                if (prev != null)
                {
                    prev.NextWP = this;
                }
            }
        }

        public void UnlinkNextWP()
        {
            if (NextWP != null)
            {
                NextWP.PrevWP = null;
                NextWP = null;
            }
        }

        public void SplitWP()
        {
            WayPointClass first_wp, new_wp, second_wp;
            GridIndex x, y, z;
            CampaignTime time;

            if (NextWP == null)
            {
                first_wp = PrevWP;
                second_wp = this;
            }
            else
            {
                first_wp = this;
                second_wp = NextWP;
            }

            x = (short)((first_wp.GridX + second_wp.GridX) / 2);
            y = (short)((first_wp.GridY + second_wp.GridY) / 2);

            if (second_wp.GetWPFlags().HasFlag(WPFlags.WPF_HOLDCURRENT))
            {
                z = first_wp.GridZ;
            }
            else
            {
                z = second_wp.GridZ;
            }

            time = (second_wp.GetWPArrivalTime() - first_wp.GetWPDepartureTime()) / 2;

            new_wp = new WayPointClass();

            new_wp.PrevWP = first_wp;
            first_wp.NextWP = new_wp;

            new_wp.NextWP = second_wp;
            second_wp.PrevWP = new_wp;

            new_wp.GridX = x;
            new_wp.GridY = y;
            new_wp.GridZ = z;
            // sfr: update Sim coorindates too
            vector pos;
            GridIndexMath.ConvertGridToSim(x, y, out pos);
            new_wp.SimX = pos.x;
            new_wp.SimY = pos.y;
            new_wp.SimZ = GridIndexMath.ConvertGridToSimZ(z);
            new_wp.SetWPTimes(time);
        }

        public void InsertWP(WayPointClass new_wp)
        {
            WayPointClass last = new_wp;

            new_wp.PrevWP = this;

            // If new_wp has a next, find the last in the list
            while (last.NextWP != null)
            {
                last = last.NextWP;
            }

            last.NextWP = NextWP;

            if (NextWP != null)
            {
                NextWP.PrevWP = last;
            }

            NextWP = new_wp;
        }

        public void DeleteWP()
        {
            if (PrevWP != null)
            {
                PrevWP.NextWP = NextWP;
            }

            if (NextWP != null)
            {
                NextWP.PrevWP = PrevWP;
            }

            //delete this;
        }

        public void CloneWP(WayPointClass w)
        {
            GridX = w.GridX;
            GridY = w.GridY;
            GridZ = w.GridZ;
            // sfr: update SimXY too
            SimX = w.SimX;
            SimY = w.SimY;
            SimZ = GridIndexMath.ConvertGridToSimZ(GridZ);

            Arrive = w.Arrive;
            Depart = w.Depart;
            Action = w.Action;
            RouteAction = w.RouteAction;
            Formation = w.Formation;
            Tactic = w.Tactic;
            Flags = w.Flags;
            Speed = 0.0f;
            PrevWP = null;
            NextWP = null;
            TargetBuilding = w.TargetBuilding;
            TargetID = w.TargetID;
        }

        public void SetWPTimes(CampaignTime t)
        {
            Depart = t + (Depart - Arrive);
            Arrive = t;
        }

        public float DistanceTo(WayPointClass w)
        {
            return GridIndexMath.Distance(GridX, GridY, w.GridX, w.GridY);
        }


        // These functions are intended for use by the campaign (They use Campaign Coordinates and times)
        public void SetWPAltitude(int alt)
        {
            GridZ = (short)(alt / GridIndexMath.GRIDZ_SCALE_FACTOR);
            SimZ = (BIG_SCALAR)(-alt);
        }

        public void SetWPAltitudeLevel(int alt)
        {
            GridZ = (short)alt;
            SimZ = GridIndexMath.ConvertGridToSimZ(GridZ);
        }

        public void SetWPStationTime(CampaignTime t) { Depart = Arrive + t; }
        public void SetWPDepartTime(CampaignTime t) { Depart = t; }
        public void SetWPArrive(CampaignTime t) { Arrive = t; }
        public void SetWPSpeed(float s) { Speed = s; }
        public float GetWPSpeed() { return Speed; }
        public void SetWPLocation(GridIndex x, GridIndex y)
        {
            GridX = x;
            GridY = y;
            // sfr: update SimXY too
            vector pos;
            GridIndexMath.ConvertGridToSim(GridX, GridY, out pos);
            SimX = pos.x;
            SimY = pos.y;
        }

        public int GetWPAltitude()
        {
            return (int)(GridZ * GridIndexMath.GRIDZ_SCALE_FACTOR);
        }

        public int GetWPAltitudeLevel()
        {
            return GridZ;
        }

        public CampaignTime GetWPStationTime()
        {
            return Depart - Arrive;
        }

        public CampaignTime GetWPArrivalTime()
        {
            return Arrive;
        }

        public CampaignTime GetWPDepartureTime()
        {
            return Depart;
        }

        public void AddWPTimeDelta(CampaignTime dt)
        {
            Arrive += dt; Depart += dt;
        }

        public void GetWPLocation(ref GridIndex x, ref GridIndex y)
        {
            x = GridX; y = GridY;
        }

        // These functions are intended for use by the Sim (They use sim coordinates and times)
        public void SetLocation(float x, float y, float z)
        {
            // sfr: fix xy order
            //GridX = SimToGrid(y); GridY = SimToGrid(x);
            vector pos = new vector { x = x, y = y };
            GridIndexMath.ConvertSimToGrid(pos, out GridX, out GridY);
            GridZ = (short)((-1.0F * z) / GridIndexMath.GRIDZ_SCALE_FACTOR);

            //If the option is set, update the SimX/Y/Z variables without converting to grid
            if (F4Config.g_bPrecisionWaypoints)
            {
                // sfr: xy order
                //SimX = y; SimY=x; SimZ=z;
                SimX = x;
                SimY = y;
                SimZ = z;
            }
        }

        public void GetLocation(out float x, out float y, out float z)
        {
            x = y = z = 0;
            // sfr: xy order
            // this is the current waypoint in grid coordinates
            GridIndex gx, gy;
            // FRB - CTD's Here
            vector pos = new vector { x = SimX, y = SimY };
            GridIndexMath.ConvertSimToGrid(pos, out gx, out gy);

            if (F4Config.g_bPrecisionWaypoints)
            {
                //Check that the Sim position and the Grid position are in sync.  If so, return the sim position
                if (
                    //(GridX == SimToGrid(SimX)) && (GridY == SimToGrid(SimY)) &&
                    (GridX == gx) && (GridY == gy) &&
                    (GridZ == (short)((-1.0F * SimZ) / GridIndexMath.GRIDZ_SCALE_FACTOR))
                )
                {
                    //*x = SimY;  *y = SimX; *z = SimZ;
                    x = SimX;
                    y = SimY;
                    z = SimZ;
                }
                // Otherwise, return the grid position converted
                // to a sim position as usual, and update the sim position to the grid position.
                else
                {
                    // sfr: this shouldnt be necessary anymore, since the waypoints are always synched now
                    // MonoPrint("Campwp.cpp: break here");
                    /*
                    // sfr: @todo
                    // f***, a Get function modifying this
                    // im gonna const_cast this for now to allow the const
                    // but this needs serious thinking about it
                    WayPointClass *t = const_cast<WayPointClass*>(this);
                    //*x = t.SimY = GridToSim(GridY);
                    //*y = t.SimX = GridToSim(GridX);
                    // sfr: xy order
                    ::vector spos;
                    ConvertGridToSim(GridX, GridY, &spos);
                    //*x = t.SimX = GridToSim(GridY);
                    //*y = t.SimY = GridToSim(GridX);
                    *x = t.SimX = spos.x;
                    *y = t.SimY = spos.y;
                    *z = t.SimZ = ConvertGridToSimZ(GridZ);
                    */
                }
            }
            //If the option is disabled, return the grid position converted to sim as before.
            else
            {
                //*x = GridToSim(GridY); *y = GridToSim(GridX); *z = -1.0F * GridZ * GRIDZ_SCALE_FACTOR;
                vector spos;
                GridIndexMath.ConvertGridToSim(GridX, GridY, out spos);
                x = spos.x;
                y = spos.y;
                //*z = -1.0F * GridZ * GRIDZ_SCALE_FACTOR;
                z = GridIndexMath.ConvertGridToSimZ(GridZ);
            }
        }

        void DeleteWPList(WayPointClass w)
        {
            WayPointClass t;

            while (w != null)
            {
                t = w;
                w = w.GetNextWP();
                //delete t;
            }
        }
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }

}
