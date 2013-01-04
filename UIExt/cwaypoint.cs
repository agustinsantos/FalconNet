using FalconNet.Common;
using FalconNet.Ui95;
using System;
using System.IO;
using WORD = System.Int16;
using COLORREF = System.Int32;
using F4CSECTIONHANDLE = System.Object;

namespace FalconNet.UIExt
{

    //TODO #define WAYPOINT_LABEL 30

    public class WAYPOINTLIST
    {
#if  USE_SH_POOLS
	public:
		// Overload new/delete to use a SmartHeap pool
		void *operator new(size_t size) { return MemAllocPtr(UI_Pools[UI_CONTROL_POOL],size,false);	};
		void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
#endif

        public long ID;
        public long Type;
        public UI95_BITTABLE Flags;
        public long Group;
        public short state;
        public short x, y;
        public float worldx, worldy;
        public C_Button Icon;
        public bool Dragable;
        public COLORREF[] LineColor_ = new COLORREF[3];
        public WAYPOINTLIST Next;
    }

    public class C_Waypoint : C_Control
    {
#if  USE_SH_POOLS
	public:
		// Overload new/delete to use a SmartHeap pool
		void *operator new(size_t size) { return MemAllocPtr(UI_Pools[UI_CONTROL_POOL],size,false);	};
		void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
#endif

        private WAYPOINTLIST Root_, LastWP_;

        private long Font_;

        private IMAGE_RSC[] Images_ = new IMAGE_RSC[8];

        private float MinWorldX_, MinWorldY_;
        private float MaxWorldX_, MaxWorldY_;

        private short WPScaleType_;

        private bool Dragging_;
        private UI95_RECT last_;
        private float scale_;
        private UI95_BITTABLE DefaultFlags_;


        public C_Waypoint()
            : base()
        {
            Root_ = null;
            _SetCType_(UI95_ENUM._CNTL_WAYPOINT_);
            SetReady(false);
            DefaultFlags_ = UI95_BITTABLE.C_BIT_ENABLED | UI95_BITTABLE.C_BIT_SELECTABLE | UI95_BITTABLE.C_BIT_MOUSEOVER;
            WPScaleType_ = 0;
            MinWorldX_ = 0;
            MinWorldY_ = 0;
            MaxWorldX_ = 0;
            MaxWorldY_ = 0;
            scale_ = 1.0f;
            Dragging_ = false;
            LastWP_ = null;
            last_ = new UI95_RECT();
        }
        public C_Waypoint(byte[] stream, ref int pos)
            : base(stream, ref pos)
        {
        }
        public C_Waypoint(FileStream fp)
            : base(fp)
        {
        }
        //TODO public ~C_Waypoint();
        public long Size()
        {
            return (0);
        }
        public void Save(byte[] stream, ref int pos) { ; }
        public void Save(FileStream fp) { ; }

        // Setup Functions
        public void Setup(long ID, UI95_ENUM Type)
        {
            SetID(ID);
            SetType(Type);
            SetDefaultFlags();
            SetGroup(0);
            SetReady(true);
        }
        public void Cleanup()
        {
            if (Root_ != null)
                EraseWaypointList();
            Root_ = null;
            LastWP_ = null;
        }
        public void SetMainImage(long imageID)
        { throw new NotImplementedException(); }
        public void SetDefaultFlags() { SetFlags(DefaultFlags_); }
        public UI95_BITTABLE GetDefaultFlags() { return (DefaultFlags_); }
        public void SetWorldRange(float minx, float miny, float maxx, float maxy) { MinWorldX_ = minx; MinWorldY_ = miny; MaxWorldX_ = maxx; MaxWorldY_ = maxy; }

        public void SetFont(long ID) { Font_ = ID; }
        public long GetFont() { return (Font_); }

        public WAYPOINTLIST AddWaypointToList(long CampID, short type, long NormID, long SelID, long OthrID, float x, float y, bool Dragable)
        {
            WAYPOINTLIST newitem, cur;

            newitem = new WAYPOINTLIST();

            if (newitem == null)
                return (null);

            newitem.Icon = new C_Button();

            newitem.ID = CampID;
            newitem.Group = 0; // JPO initialise
            newitem.Type = type;
            newitem.Icon.Setup(CampID, UI95_ENUM.C_TYPE_CUSTOM, 0, 0);
            newitem.Icon.SetImage(UI95_ENUM.C_STATE_0, NormID);
            newitem.Icon.SetImage(UI95_ENUM.C_STATE_1, SelID);
            newitem.Icon.SetImage(UI95_ENUM.C_STATE_2, OthrID);
            if (Dragable)
                newitem.Icon.SetFlags((GetFlags() & ~(UI95_BITTABLE.C_BIT_DRAGABLE)) | UI95_BITTABLE.C_BIT_DRAGABLE);
            else
                newitem.Icon.SetFlags((GetFlags() & ~(UI95_BITTABLE.C_BIT_DRAGABLE)));
            newitem.Icon.SetClient(GetClient());
            newitem.Icon.SetParent(Parent_);
            newitem.Flags = UI95_BITTABLE.C_BIT_ENABLED;
            if (Dragable)
                newitem.Flags |= UI95_BITTABLE.C_BIT_DRAGABLE;
            newitem.Dragable = Dragable;
            newitem.state = 0;
            newitem.LineColor_[0] = 0;
            newitem.LineColor_[1] = 0;
            newitem.LineColor_[2] = 0;
            newitem.worldx = x;
            newitem.worldy = y;
            newitem.x = (short)(scale_ * x);
            if (WPScaleType_ == 0)
                newitem.y = (short)(scale_ * y);
            else if (WPScaleType_ == 1)
                newitem.y = (short)(MaxWorldY_ - (28.853f * (Math.Log(-y * 0.0001f + 1.0f))));
            else if (WPScaleType_ == 2)
                newitem.y = (short)(MaxWorldY_ + y * 0.001f);

            newitem.Icon.SetXY(newitem.x - newitem.Icon.GetW() / 2, newitem.y - newitem.Icon.GetH() / 2);
            newitem.Next = null;

            if (Root_ == null)
                Root_ = newitem;
            else
            {
                cur = Root_;
                while (cur.Next != null)
                    cur = cur.Next;
                cur.Next = newitem;
            }
            return (newitem);
        }

        public bool UpdateInfo(long ID, float x, float y)
        {
#if TODO
            WAYPOINTLIST cur, wk1, wk2;
            short ox, oy;
            float dx, dy;
            string buf;

            cur = FindID(ID);
            if (cur == null)
                return (false);

            if (cur.worldx != x || cur.worldy != y)
            {
                cur.worldx = x;
                cur.worldy = y;
                ox = cur.x;
                oy = cur.y;
                cur.x = (short)(cur.worldx * scale_);
                cur.y = (short)(cur.worldy * scale_);
                cur.Icon.SetXY(cur.x - cur.Icon.GetW() / 2, cur.y - cur.Icon.GetH() / 2);

                if ((ID & 0x60000000) == 0)
                {
                    wk1 = FindID(ID - 1);
                    wk2 = FindID(0x40000000 + ID);
                    if (wk1 != null && wk2 != null)
                    {
                        dx = (cur.worldx - wk1.worldx);
                        dy = (cur.worldy - wk1.worldy);

                        wk2.worldx = wk1.worldx + dx * .5F;
                        wk2.worldy = wk1.worldy + dy * .5F;
                        wk2.x = (short)(wk2.worldx * scale_);
                        wk2.y = (short)(wk2.worldy * scale_);
                        buf = String.Format("%6.1f", Math.Sqrt(dx * dx + dy * dy) * FT_TO_NM);
                        wk2.Icon.SetXY(wk2.x - wk2.Icon.GetW() / 2, wk2.y - wk2.Icon.GetH() / 2);
                        wk2.Icon.SetAllLabel(gStringMgr.GetText(gStringMgr.AddText(buf)));
                    }
                    wk1 = FindID(ID + 1);
                    wk2 = FindID(0x60000000 + ID + 1);
                    if (wk1 != null && wk2 != null)
                    {
                        dx = (wk1.worldx - cur.worldx);
                        dy = (wk1.worldy - cur.worldy);

                        wk2.worldx = cur.worldx + dx * .5F;
                        wk2.worldy = cur.worldy + dy * .5F;
                        wk2.x = (short)(wk2.worldx * scale_);
                        wk2.y = (short)(wk2.worldy * scale_);
                        buf = String.Format("%6.1f", Math.Sqrt(dx * dx + dy * dy) * FT_TO_NM);
                        wk2.Icon.SetXY(wk2.x - wk2.Icon.GetW() / 2, wk2.y - wk2.Icon.GetH() / 2);
                        wk2.Icon.SetAllLabel(gStringMgr.GetText(gStringMgr.AddText(buf)));
                    }
                }
                if ((ox != cur.x || oy != cur.y) && !(cur.Flags.IsFlagSet( UI95_BITTABLE.C_BIT_INVISIBLE)))
                    return (true);
            }
            return (false);
#endif
            throw new NotImplementedException();

        }

        public void EraseWaypointList()
        {
            WAYPOINTLIST cur, last;

            cur = Root_;

            if (Root_ == null || Parent_ == null)
                return;

            Root_ = null;
            LastWP_ = null;
            while (cur != null)
            {
                last = cur;
                cur = cur.Next;
                if (last.Icon != null)
                {
                    last.Icon.Cleanup();
                    last.Icon = null;
                }
                last = null;
            }
        }

        public void EraseWaypointGroup(long groupid)
        {
            WAYPOINTLIST cur, last, prev;

            if (Root_ == null || Parent_ == null)
                return;

            while (Root_ != null && Root_.Group == groupid)
            {
                last = Root_;
                Root_ = Root_.Next;
                if (last.Icon != null)
                {
                    last.Icon.Cleanup();
                    last.Icon = null;
                }
                last = null;
            }

            cur = Root_;
            while (cur != null)
            {
                prev = cur;
                cur = cur.Next;
                if (cur != null)
                {
                    if (cur.Group == groupid)
                    {
                        last = cur;
                        prev.Next = cur.Next;
                        if (last.Icon != null)
                        {
                            last.Icon.Cleanup();
                            last.Icon = null;
                        }
                        last = null;
                        cur = prev;
                    }
                }
            }
        }

        public bool ShowByType(long mask)
        {
            WAYPOINTLIST cur;
            bool retval = false;

            cur = Root_;

            while (cur != null)
            {
                if ((cur.Type & mask) != 0)
                {
                    cur.Flags &= (UI95_BITTABLE)(0xffffffff ^ (uint)UI95_BITTABLE.C_BIT_INVISIBLE);
                    retval = true;
                }
                cur = cur.Next;
            }
            return (retval);
        }

        public bool HideByType(long mask)
        {
            WAYPOINTLIST cur;
            bool retval = false;

            cur = Root_;

            while (cur != null)
            {
                if ((cur.Type & mask) != 0)
                {
                    cur.Flags |= UI95_BITTABLE.C_BIT_INVISIBLE;
                    retval = true;
                }
                cur = cur.Next;
            }
            return (retval);
        }

        public void SetScaleFactor(float scale)
        {
            WAYPOINTLIST cur;

            if (scale <= 0.0f || scale == scale_ || WPScaleType_ != 0)
                return;

            scale_ = scale;

            cur = Root_;
            while (cur != null)
            {
                cur.x = (short)(cur.worldx * scale_);
                cur.y = (short)(cur.worldy * scale_);
                cur.Icon.SetXY(cur.x - cur.Icon.GetW() / 2, cur.y - cur.Icon.GetH() / 2);
                cur = cur.Next;
            }
        }

        public void SetScaleType(short scaletype)
        {
            WAYPOINTLIST cur;

            WPScaleType_ = scaletype;


            if (scaletype == 1)
            {
                cur = Root_;
                while (cur != null)
                {
                    cur.y = (short)(MaxWorldY_ - (28.853f * (Math.Log(-cur.worldy * 0.0001f + 1.0f)))); // Only care about the Z
                    cur.Icon.SetXY(cur.x - cur.Icon.GetW() / 2, cur.y - cur.Icon.GetH() / 2);
                    cur = cur.Next;
                }
            }
            else if (scaletype == 2)
            {
                cur = Root_;
                while (cur != null)
                {
                    cur.y = (short)(MaxWorldY_ + cur.worldy * 0.01f);
                    cur.Icon.SetXY(cur.x - cur.Icon.GetW() / 2, cur.y - cur.Icon.GetH() / 2);
                    cur = cur.Next;
                }
            }
        }

        public WAYPOINTLIST FindID(long ID)
        {
            WAYPOINTLIST cur;

            cur = Root_;
            while (cur != null)
            {
                if (cur.ID == ID)
                    return (cur);
                cur = cur.Next;
            }
            return (null);
        }

        public void SetWPGroup(long ID, long group)
        {
            WAYPOINTLIST cur;

            cur = FindID(ID);
            if (cur != null)
                cur.Group = group;
        }

        public void SetState(long ID, short state)
        {
            WAYPOINTLIST cur;

            cur = FindID(ID);
            if (cur != null)
            {
                cur.state = state;
                cur.Icon.SetState(state);
            }
        }

        public void SetGroupState(long group, short state)
        {
            WAYPOINTLIST cur;

            cur = Root_;
            while (cur != null)
            {
                if (cur.Group == group)
                {
                    cur.state = state;
                    cur.Icon.SetState(state);
                }
                cur = cur.Next;
            }
        }

        public void SetLabel(long ID, string txt)
        {
#if TODO
            WAYPOINTLIST cur;

            cur = FindID(ID);
            if (cur != null)
                cur.Icon.SetAllLabel(gStringMgr.GetText(gStringMgr.AddText(txt)));
#endif
            throw new NotImplementedException();
        }

        public void SetLabelColor(long ID, COLORREF norm, COLORREF sel, COLORREF othr)
        {
            WAYPOINTLIST cur;

            cur = FindID(ID);
            if (cur != null)
            {
                cur.Icon.SetLabelColor((WORD)UI95_ENUM.C_STATE_0, norm);
                cur.Icon.SetLabelColor((WORD)UI95_ENUM.C_STATE_1, sel);
                cur.Icon.SetLabelColor((WORD)UI95_ENUM.C_STATE_2, othr);
            }
        }

        public void SetLineColor(long ID, COLORREF norm, COLORREF sel, COLORREF othr)
        {
            WAYPOINTLIST cur;

            cur = FindID(ID);
            if (cur != null)
            {
                cur.LineColor_[0] = norm;
                cur.LineColor_[1] = sel;
                cur.LineColor_[2] = othr;
            }
        }

        public string GetLabel(long ID)
        {
            WAYPOINTLIST cur;

            cur = FindID(ID);
            if (cur != null)
                return (cur.Icon.GetLabel((WORD)UI95_ENUM.C_STATE_0));
            return (null);
        }

        public void SetTextOffset(long ID, short x, short y)
        {
            WAYPOINTLIST cur;

            cur = FindID(ID);
            if (cur != null)
            {
                cur.Icon.SetLabelOffset((WORD)UI95_ENUM.C_STATE_0, x, y);
                cur.Icon.SetLabelOffset((WORD)UI95_ENUM.C_STATE_1, x, y);
                cur.Icon.SetLabelOffset((WORD)UI95_ENUM.C_STATE_2, x, y);
                cur.Icon.SetLabelInfo();
            }
        }

        public WAYPOINTLIST GetRoot() { return (Root_); }
        public WAYPOINTLIST GetLast() { return (LastWP_); }

        // Handler/Window Functions
        public override long CheckHotSpots(long relX, long relY)
        {
            WAYPOINTLIST cur;

            LastWP_ = null;
            cur = Root_;
            while (cur != null)
            {
                if (!(cur.Flags.IsFlagSet(UI95_BITTABLE.C_BIT_INVISIBLE)) && cur.Flags.IsFlagSet(UI95_BITTABLE.C_BIT_ENABLED))
                {
                    if (cur.Icon.CheckHotSpots(relX, relY) != 0)
                        LastWP_ = cur;
                }
                cur = cur.Next;
            }
            if (LastWP_ != null)
            {
                SetRelXY(relX - GetX(), relY - GetY());
                return (LastWP_.ID);
            }
            return (0);
        }

        public override bool Dragable(long p) { return (GetFlags().IsFlagSet(UI95_BITTABLE.C_BIT_DRAGABLE)); }
        public override bool Process(long ID, short ButtonHitType)
        {
#if TODO
            gSoundMgr.PlaySound(GetSound(HitType));
            if (Callback_ != null)
                (*Callback_)(ID, HitType, this);
            if (LastWP_ != null)
                return (LastWP_.Icon.Process(ID, HitType));
            return (false);
#endif
            throw new NotImplementedException();
        }

        public bool MouseOver(long relX, long relY, C_Base b)
        {
            WAYPOINTLIST cur;

            cur = Root_;
            while (cur != null)
            {
                if (!(cur.Flags.IsFlagSet(UI95_BITTABLE.C_BIT_INVISIBLE)) && cur.Flags.IsFlagSet(UI95_BITTABLE.C_BIT_ENABLED))
                {
                    if (cur.Icon != null && cur.Icon.MouseOver(relX, relY, cur.Icon)) // possible CTD fix
                    {
                        return (true);
                    }
                }
                cur = cur.Next;
            }
            return (false);
        }

        public void GetItemXY(long ID, ref long x, ref long y)
        {
            WAYPOINTLIST Waypoint;

            Waypoint = FindID(ID);
            if (Waypoint == null)
                return;

            x = Waypoint.x;
            y = Waypoint.y;
        }
        public bool Drag(GRABBER Drag, WORD MouseX, WORD MouseY, C_Window over)
        {
#if TODO
            WAYPOINTLIST Waypoint, wk1, wk2;
            long relx, rely;
            long x, y;
            float dx, dy;
            string buf;
            F4CSECTIONHANDLE Leave;

            if (GetFlags().IsFlagSet(UI95_BITTABLE.C_BIT_INVISIBLE) || !(GetFlags().IsFlagSet(UI95_BITTABLE.C_BIT_ENABLED)) || !Dragable(0))
                return (false);

            if (over != Parent_)
                return (false);

            if (!(GetFlags().IsFlagSet(UI95_BITTABLE.C_BIT_ABSOLUTE)))
            {
                relx = MouseX - over.GetX();
                rely = MouseY - over.GetY();

                if (relx < over.ClientArea_[GetClient()].left || relx > over.ClientArea_[GetClient()].right)
                    return (false);
                if (rely < over.ClientArea_[GetClient()].top || rely > over.ClientArea_[GetClient()].bottom)
                    return (false);
            }

            if (LastWP_ == null)
                return (false);

            if (!(LastWP_.Flags.IsFlagSet(UI95_BITTABLE.C_BIT_DRAGABLE)))
                return (false);

            Leave = UI_Enter(Parent_);
            Waypoint = LastWP_;

#if NOTHING
	cur=Root_;
	prev=null;
	if(cur != Waypoint)
	{
		prev=cur;
		while(cur.Next && cur.Next != Waypoint)
		{
			prev=cur;
			cur=cur.Next;
		}
	}

	next=Waypoint.Next;

	rect.left=Waypoint.x;
	rect.top=Waypoint.y;
	rect.right=Waypoint.x;
	rect.bottom=Waypoint.y;

	if(prev)
	{
		if(prev.x < rect.left)
			rect.left=prev.x;
		if(prev.y < rect.top)
			rect.top=prev.y;
		if(prev.x > rect.right)
			rect.right=prev.x;
		if(prev.y > rect.bottom)
			rect.bottom=prev.y;
//		prev.Icon.Refresh();
	}
	if(next)
	{
		if(next.x < rect.left)
			rect.left=next.x;
		if(next.y < rect.top)
			rect.top=next.y;
		if(next.x > rect.right)
			rect.right=next.x;
		if(next.y > rect.bottom)
			rect.bottom=next.y;
//		next.Icon.Refresh();
	}
//	Waypoint.Icon.Refresh();
#endif
            Dragging_ = true;


            if (GetCType() == UI95_ENUM.C_TYPE_DRAGXY || GetCType() == UI95_ENUM.C_TYPE_DRAGX)
                x = Drag.ItemX_ + (MouseX - Drag.StartX_);
            else
                x = Waypoint.x;

            if (GetCType() == UI95_ENUM.C_TYPE_DRAGXY || GetCType() == UI95_ENUM.C_TYPE_DRAGY)
                y = Drag.ItemY_ + (MouseY - Drag.StartY_);
            else
                y = Waypoint.y;

            if (x < (over.ClientArea_[GetClient()].left - over.VX_[GetClient()]))
                x = over.ClientArea_[GetClient()].left - over.VX_[GetClient()];
            if (x > (over.ClientArea_[GetClient()].right - over.VX_[GetClient()]))
                x = over.ClientArea_[GetClient()].right - over.VX_[GetClient()];
            if (y < (over.ClientArea_[GetClient()].top - over.VY_[GetClient()]))
                y = over.ClientArea_[GetClient()].top - over.VY_[GetClient()];
            if (y > (over.ClientArea_[GetClient()].bottom - over.VY_[GetClient()]))
                y = over.ClientArea_[GetClient()].bottom - over.VY_[GetClient()];

            Waypoint.x = (short)(x);
            Waypoint.y = (short)(y);

            if (WPScaleType_ == 0)
            {
                Waypoint.worldx = x / scale_;
                Waypoint.worldy = y / scale_;
            }
            else if (WPScaleType_ == 1)
            {
                Waypoint.worldy = -((float)exp((MaxWorldY_ - y) * 0.034658441f) - 1.0f) * 10000.0f; //(0.03nnn = 1/28.853)
                if (Waypoint.worldy > 0)
                    Waypoint.worldy = 0;
            }
            else if (WPScaleType_ == 2)
            {
                Waypoint.worldy = -(float)((MaxWorldY_ - y) * 100);
                if (Waypoint.worldy > 0)
                    Waypoint.worldy = 0;
            }
            Waypoint.Icon.SetXY(x - Waypoint.Icon.GetW() / 2, y - Waypoint.Icon.GetH() / 2);

            if (!(Waypoint.ID & 0x60000000))
            {
                wk1 = FindID(Waypoint.ID - 1);
                wk2 = FindID(0x40000000 + Waypoint.ID);
                if (wk1 != null && wk2 != null)
                {
                    dx = (Waypoint.worldx - wk1.worldx);
                    dy = (Waypoint.worldy - wk1.worldy);

                    wk2.worldx = wk1.worldx + dx * .5F;
                    wk2.worldy = wk1.worldy + dy * .5F;
                    wk2.x = (short)(wk2.worldx * scale_);
                    wk2.y = (short)(wk2.worldy * scale_);
                    wk2.Icon.SetXY(wk2.x - wk2.Icon.GetW() / 2, wk2.y - wk2.Icon.GetH() / 2);
                    buf = String.Format("%6.1f", Math.Sqrt(dx * dx + dy * dy) * FT_TO_NM);
                    wk2.Icon.SetAllLabel(gStringMgr.GetText(gStringMgr.AddText(buf)));
                }
                wk1 = FindID(Waypoint.ID + 1);
                wk2 = FindID(0x40000000 + Waypoint.ID + 1);
                if (wk1 != null && wk2 != null)
                {
                    dx = (wk1.worldx - Waypoint.worldx);
                    dy = (wk1.worldy - Waypoint.worldy);

                    wk2.worldx = Waypoint.worldx + dx * .5F;
                    wk2.worldy = Waypoint.worldy + dy * .5F;
                    wk2.x = (short)(wk2.worldx * scale_);
                    wk2.y = (short)(wk2.worldy * scale_);
                    wk2.Icon.SetXY(wk2.x - wk2.Icon.GetW() / 2, wk2.y - wk2.Icon.GetH() / 2);
                    buf = String.Format("%6.1f", Math.Sqrt(dx * dx + dy * dy) * FT_TO_NM);
                    wk2.Icon.SetAllLabel(gStringMgr.GetText(gStringMgr.AddText(buf)));
                }
            }
            Parent_.RefreshClient(GetClient());

            if (Callback_)
                (*Callback_)(LastWP_.ID, C_TYPE_MOUSEMOVE, this);
            UI_Leave(Leave);

            return (true);
#endif 
            throw new NotImplementedException();
        }

        public bool Drop(GRABBER g, WORD p1, WORD p2, C_Window w)
        {
            Dragging_ = false;
            return false;
        }

        public override void Refresh()
        {
            WAYPOINTLIST cur;
            UI95_RECT rect = new UI95_RECT();
            if (!Ready() || GetFlags().IsFlagSet(UI95_BITTABLE.C_BIT_INVISIBLE) || Parent_ == null)
                return;

            rect.left = 5000;
            rect.top = 5000;
            rect.bottom = 0;
            rect.right = 0;

            cur = Root_;
            while (cur != null)
            {
                if (!(cur.Flags.IsFlagSet(UI95_BITTABLE.C_BIT_INVISIBLE)))
                {
                    if (cur.Icon.GetX() < rect.left)
                        rect.left = cur.Icon.GetX();
                    if (cur.Icon.GetY() < rect.top)
                        rect.top = cur.Icon.GetY();
                    if ((cur.Icon.GetX() + cur.Icon.GetW()) > rect.right)
                        rect.right = cur.Icon.GetX() + cur.Icon.GetW();
                    if ((cur.Icon.GetY() + cur.Icon.GetH()) > rect.bottom)
                        rect.bottom = cur.Icon.GetY() + cur.Icon.GetH();
                }
                cur = cur.Next;
            }
            Parent_.SetUpdateRect(last_.left, last_.top, last_.right, last_.bottom, GetFlags(), GetClient());
            Parent_.SetUpdateRect(rect.left, rect.top, rect.right, rect.bottom, GetFlags(), GetClient());
            last_ = rect;
        }

        public void Draw(SCREEN surface, UI95_RECT cliprect)
        {
            WAYPOINTLIST cur, prev;

            if (!Ready()) return;

            cur = Root_;
            prev = cur;
            while (cur != null)
            {
                if (cur.ID != 0)
                {
                    if (!(cur.Flags.IsFlagSet(UI95_BITTABLE.C_BIT_INVISIBLE)))
                    {
                        if (cur.Flags.IsFlagSet(UI95_BITTABLE.C_BIT_USELINE))
                            if (prev != cur)
                                if (prev.Group == cur.Group)
                                    Parent_.DrawLine(surface, cur.LineColor_[cur.state], prev.x, prev.y, cur.x, cur.y, (long)GetFlags(), (long)GetClient(), cliprect);

                        if (cur.Icon != null)
                            cur.Icon.Draw(surface, cliprect);
                    }
                    prev = cur;
                }
                cur = cur.Next;
            }
        }

        public bool Dragging()
        {
            return (Dragging_);
        }

        public void SetSubParents(C_Window par)
        {
            WAYPOINTLIST cur;

            cur = Root_;
            while (cur != null)
            {
                cur.Icon.SetParent(par);
                cur.Icon.SetSubParents(par);
                cur = cur.Next;
            }
        }
    }

}
