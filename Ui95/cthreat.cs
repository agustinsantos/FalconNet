using System;


namespace FalconNet.Ui95
{

    public class THREAT_CIRCLE
    {
        public long ID;
        public long Type;
        public long Flags;
        public long x, y;
        public long[] Radius = new long[8];
        public C_Threat Owner;
    }

    public struct CircleEdge
    {
        public long Left, Right;
    }

    public class Circle
    {
        public static readonly float HALFSQUAREROOT2 = (float)(0.5 * Math.Sqrt(2.0));
        public const int MAXCIRCLESIZE = 2048;
        public const int MAXOVERLAP = 8;


        protected static CircleEdge[] Edge = new CircleEdge[MAXCIRCLESIZE];
        protected static long MaxHeight, MaxWidth;
        protected static long MaxHeight1, MaxWidth1;
        protected static long CenterX, CenterY, Radius, Diagonal;
        protected static long CircleTop, CircleSize;
        protected static int CircleTopAddress;
        protected static byte[] CircleBuffer;

#if USE_SH_POOLS
	public:
		// Overload new/delete to use a SmartHeap pool
		void *operator new(size_t size) { return MemAllocPtr(UI_Pools[UI_CONTROL_POOL],size,FALSE);	};
		void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
#endif

        public void SetBuffer(byte[] buffer)
        {
            CircleBuffer = buffer;
        }

        public void SetDimension(long width, long height)
        {
            MaxWidth = width;
            MaxHeight = height;
            MaxWidth1 = width - 1;
            MaxHeight1 = height - 1;
        }

        public void SetCenter(long centerx, long centery)
        {
            CenterX = centerx;
            CenterY = centery;
        }

        public void SetRadius(long radius)
        {
            Radius = radius;
            Diagonal = (long)((double)radius * HALFSQUAREROOT2 + 0.5);
        }

        public void FillLeftEdge(long cx, long cy)
        {
            if (cy >= 0 && cy < MaxHeight && cx < MaxWidth)
            {
                if (cx < 0) cx = 0;
                Edge[cy].Left = cx;
            }
        }

        public void FillRightEdge(long cx, long cy)
        {
            if (cy >= 0 && cy < MaxHeight && cx > 0)
            {
                if (cx > MaxWidth1) cx = MaxWidth1;
                Edge[cy].Right = cx;
            }
        }

        public void FillLeftEdgeY(long cx, long cy)
        {
            if (cy >= 0 && cy < MaxHeight)
                Edge[cy].Left = cx;
        }

        public void FillRightEdgeY(long cx, long cy)
        {
            if (cy >= 0 && cy < MaxHeight)
                Edge[cy].Right = cx;
        }

        public void FillCirclePoints(long x, long y, long width)
        {
            if (y >= 0 && y < MaxHeight && x < MaxWidth)
            {
                long x1 = x + width;
                if (x1 >= 0)
                {
                    CircleEdge edge = Edge[y];
                    if (x < 0) x = 0;
                    edge.Left = x;
                    if (x1 > MaxWidth1) x1 = MaxWidth1;
                    edge.Right = x1;
                }
            }
        }

        public void FillVerticalLineUpLeft(long cx, long cy)
        {
            long i;

            if (cx < 0) cx = 0;
            else if (cx > MaxWidth1) cx = MaxWidth1;

            cy--;
            for (i = 0; i < Radius; i++)
            {
                FillLeftEdgeY(cx, cy);
                cy--;
            }
        }

        public void FillVerticalLineUpRight(long cx, long cy)
        {
            long i;

            if (cx < 0) cx = 0;
            else if (cx > MaxWidth1) cx = MaxWidth1;

            cy--;
            for (i = 0; i < Radius; i++)
            {
                FillRightEdgeY(cx, cy);
                cy--;
            }
        }

        public void FillVerticalLineDownLeft(long cx, long cy)
        {
            long i;

            if (cx < 0) cx = 0;
            else if (cx > MaxWidth1) cx = MaxWidth1;

            for (i = 0; i <= Radius; i++)
            {
                FillLeftEdgeY(cx, cy);
                cy++;
            }
        }

        public void FillVerticalLineDownRight(long cx, long cy)
        {
            long i;

            if (cx < 0) cx = 0;
            else if (cx > MaxWidth1) cx = MaxWidth1;

            for (i = 0; i <= Radius; i++)
            {
                FillRightEdgeY(cx, cy);
                cy++;
            }
        }

        public void FillDiagonalLineLeftUpLeft(long cx, long cy)
        {
            long i;
            for (i = 0; i < Diagonal; i++)
            {
                FillLeftEdge(cx, cy);
                cy--;
                cx--;
            }
        }

        public void FillDiagonalLineLeftUpRight(long cx, long cy)
        {
            long i;
            for (i = 0; i < Diagonal; i++)
            {
                FillRightEdge(cx, cy);
                cy--;
                cx--;
            }
        }

        public void FillDiagonalLineLeftDownLeft(long cx, long cy)
        {
            long i;
            for (i = 0; i < Diagonal; i++)
            {
                FillLeftEdge(cx, cy);
                cy++;
                cx--;
            }
        }

        public void FillDiagonalLineLeftDownRight(long cx, long cy)
        {
            long i;
            for (i = 0; i < Diagonal; i++)
            {
                FillRightEdge(cx, cy);
                cy++;
                cx--;
            }
        }

        public void FillDiagonalLineRightUpLeft(long cx, long cy)
        {
            long i;
            for (i = 0; i < Diagonal; i++)
            {
                FillLeftEdge(cx, cy);
                cy--;
                cx++;
            }
        }

        public void FillDiagonalLineRightUpRight(long cx, long cy)
        {
            long i;
            for (i = 0; i < Diagonal; i++)
            {
                FillRightEdge(cx, cy);
                cy--;
                cx++;
            }
        }

        public void FillDiagonalLineRightDownLeft(long cx, long cy)
        {
            long i;
            for (i = 0; i < Diagonal; i++)
            {
                FillLeftEdge(cx, cy);
                cy++;
                cx++;
            }
        }

        public void FillDiagonalLineRightDownRight(long cx, long cy)
        {
            long i;
            for (i = 0; i < Diagonal; i++)
            {
                FillRightEdge(cx, cy);
                cy++;
                cx++;
            }
        }

        public void InitBuffer()
        { throw new NotImplementedException(); }

        public void FillBuffer()
        { throw new NotImplementedException(); }


        public void CreateFilledArc0()
        { throw new NotImplementedException(); }

        public void CreateFilledArc1()
        { throw new NotImplementedException(); }

        public void CreateFilledArc2()
        { throw new NotImplementedException(); }

        public void CreateFilledArc3()
        { throw new NotImplementedException(); }

        public void CreateFilledArc4()
        { throw new NotImplementedException(); }

        public void CreateFilledArc5()
        { throw new NotImplementedException(); }

        public void CreateFilledArc6()
        { throw new NotImplementedException(); }

        public void CreateFilledArc7()
        { throw new NotImplementedException(); }


        public void CreateFilledArc(long octant)
        { throw new NotImplementedException(); }


        public void CreateFilledCircle()
        { throw new NotImplementedException(); }

        public void CreateFilledCirclePoints(long x, long y)
        { throw new NotImplementedException(); }
    }

    public class C_Threat : C_Base
    {

        protected static Circle myCircle;
        protected C_Hash Root_;


        public enum THR
        {
            THR_CIRCLE = 0,
            THR_SLICE = 1,
        };
        public C_Threat()
            : base()
        {
            Root_ = null;
        }
        //TODO public ~C_Threat();

        public void Setup(long ID, long type)
        {
            SetID(ID);
            SetType((UI95_ENUM)(type));
            SetReady(true);

            Root_ = new C_Hash();
            Root_.Setup(10);
            Root_.SetFlags(UI95_BITTABLE.C_BIT_REMOVE);
        }

        public void Cleanup()
        { throw new NotImplementedException(); }

        public void AddCircle(long ID, long type, long worldx, long worldy, long radius) // all units are KM
        {
            THREAT_CIRCLE circle;
            long i;

            if (Root_ == null || ID < 1 || radius == 0)
                return;

            if (Root_.Find(ID) != null)
                return;

            circle = new THREAT_CIRCLE();
            circle.ID = ID;
            circle.Type = type;
            circle.Flags = 0;
            for (i = 0; i < 8; i++)
                circle.Radius[i] = radius;
            circle.x = worldx;
            circle.y = worldy;
            circle.Owner = this;

            Root_.Add(ID, circle);
        }

        public void UpdateCircle(long ID, long worldx, long worldy)
        {
            THREAT_CIRCLE circle;

            if (Root_ == null || ID < 1)
                return;

            circle = (THREAT_CIRCLE)Root_.Find(ID);
            if (circle != null)
            {
                circle.x = worldx;
                circle.y = worldy;
            }
        }

        public void SetRadius(long ID, long slice, long radius)
        {
            THREAT_CIRCLE circle;

            if (slice < 0 || slice >= 8)
                return;

            circle = (THREAT_CIRCLE)Root_.Find(ID);
            if (circle != null)
                circle.Radius[slice] = radius;
        }

        public void Remove(long ID)
        {
#if TODO
            if (Root_ == null || ID < 1)
                return;

            Root_.Remove(ID);
#endif
            throw new NotImplementedException();
        }

        public THREAT_CIRCLE GetThreat(long ID) { if (Root_ != null) return ((THREAT_CIRCLE)Root_.Find(ID)); return (null); }
        public void BuildOverlay(byte[] overlay, long w, long h, float kmperpixel)
        {
#if TODO
            THREAT_CIRCLE circle;
            C_HASHNODE  cur;
            long curidx;
            long i;

            if (Flags_ & C_BIT_INVISIBLE || !Root_ || !overlay)
                return;

            myCircle.SetBuffer(overlay);
            myCircle.SetDimension(w, h);

            circle = (THREAT_CIRCLE*)Root_.GetFirst(&cur, &curidx);
            while (circle)
            {
                // 2001-05-10 MODIFIED BY S.G. NEED TO LOOK UP THE MAP ITEM FLAG TO SEE IF IT CAN BE DISPLAYED OR NOT...
                //		if(!(circle.Flags & C_BIT_INVISIBLE)) {
                UI_Refresher* gpsItem = NULL;
                if (!(circle.Flags & C_BIT_INVISIBLE) && ((gpsItem = (UI_Refresher*)gGps.Find(circle.ID)) && gpsItem.MapItem_ && !(gpsItem.MapItem_.Flags & C_BIT_INVISIBLE)))
                {
                    myCircle.SetCenter(static_cast<long>(static_cast<float>(circle.x) * pixelsperkm),
                                       static_cast<long>(static_cast<float>(circle.y) * pixelsperkm));
                    if (circle.Type == THR_CIRCLE)
                    {
                        if (circle.Radius[0] > 3)
                        {
                            myCircle.SetRadius(static_cast<long>(static_cast<float>(circle.Radius[0]) * pixelsperkm));
                            myCircle.CreateFilledCircle();
                        }
                    }
                    else if (circle.Type == THR_SLICE)
                    {
                        for (i = 0; i < 8; i++)
                        {
                            if (circle.Radius[i] > 3)
                            {
                                myCircle.SetRadius(static_cast<long>(static_cast<float>(circle.Radius[i]) * pixelsperkm));
                                myCircle.CreateFilledArc(i);
                            }
                        }
                    }
                }
                circle = (THREAT_CIRCLE*)Root_.GetNext(&cur, &curidx);
            }
#endif
            throw new NotImplementedException();
        }
    }
}
