using FalconNet.Common;
using FalconNet.Ui95;
using System;
using WORD = System.Int16;
using COLORREF = System.Int32;
using System.IO;

namespace FalconNet.UIExt
{



    public class ARC_REC
    {
#if USE_SH_POOLS
	public:
		// Overload new/delete to use a SmartHeap pool
		void *operator new(size_t size) { return MemAllocPtr(UI_Pools[UI_CONTROL_POOL],size,FALSE);	};
		void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
#endif

        public short arc;
        public float range;
    }

    public struct ARC_LIST
    {
        public short numarcs;
        public ARC_REC arcs;
    }

    public class DETECTOR
    {
        public ARC_LIST LowRadar;

        public float LowSam;
        public float HighSam;
        public float HighRadar;
    }



    public class MAPICONLIST
    {
#if USE_SH_POOLS
	public:
		// Overload new/delete to use a SmartHeap pool
		void *operator new(size_t size) { return MemAllocPtr(UI_Pools[UI_CONTROL_POOL],size,FALSE);	};
		void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
#endif

        public long ID;
        public long Type;
        public long Flags;
        public long state;
        public short x, y;
        public float worldx, worldy;
        public long Status;
        public long ImageID;
        public short Dragable;
        public DETECTOR Detect;
        public C_MapIcon Owner;
        public O_Output Icon;
        public O_Output Div;
        public O_Output Brig;
        public O_Output Bat;
        public O_Output Label;
    }

    public class C_MapIcon : C_Control
    {
        public const int ICON_LABEL = 30;
        public const int NUM_DIRECTIONS = 8;

#if USE_SH_POOLS
	public:
		// Overload new/delete to use a SmartHeap pool
		void *operator new(size_t size) { return MemAllocPtr(UI_Pools[UI_CONTROL_POOL],size,FALSE);	};
		void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
#endif

        private long Font_;
        private short Team_;
        private float scale_;
        private short ShowCircles_;
        private long LastTime_, CurTime_;
        private UI95_BITTABLE DefaultFlags_;
        private C_Hash Root_;
        private MAPICONLIST Last_;
        private MAPICONLIST OverLast_;
        private C_Resmgr[,] Icons_ = new C_Resmgr[NUM_DIRECTIONS, 2];



        public enum SAMRADAR
        {
            LOW_SAM = 0x0001,
            HIGH_SAM = 0x0002,
            LOW_RADAR = 0x0004,
            HIGH_RADAR = 0x0008,
        };

        public C_MapIcon()
        { throw new NotImplementedException(); }
        public C_MapIcon(byte[] stream, ref int pos)
        { throw new NotImplementedException(); }
        public C_MapIcon(FileStream fp)
        { throw new NotImplementedException(); }
        //TODO public ~C_MapIcon();
        public long Size()
        { throw new NotImplementedException(); }
        public void Save(byte[] stream, ref int pos) { ; }
        public void Save(FileStream file) { ; }

        // Setup Functions
        public void Setup(long ID, short Type)
        { throw new NotImplementedException(); }
        public void Cleanup()
        { throw new NotImplementedException(); }
        public void SetMainImage(short idx, long OffID, long OnID)
        { throw new NotImplementedException(); }
        public void SetMainImage(long OffID, long OnID)
        { throw new NotImplementedException(); }
        public void SetDefaultFlags() { SetFlags(DefaultFlags_); }
        public UI95_BITTABLE GetDefaultFlags() { return (DefaultFlags_); }

        public void ShowCircles(short val) { ShowCircles_ = val; }
        public short GetShowCircles() { return (ShowCircles_); }

        public MAPICONLIST AddIconToList(long CampID, short type, long ImageID, float x, float y, short Dragable, string str, long DivID, long BrigID, long BatID, long newstatus, long newstate, DETECTOR detptr)
        { throw new NotImplementedException(); }
        public MAPICONLIST AddIconToList(long CampID, short type, long ImageID, float x, float y, short Dragable, string str, long DivID, long BrigID, long BatID, long newstatus, long newstate)
        { return (AddIconToList(CampID, type, ImageID, x, y, Dragable, str, DivID, BrigID, BatID, newstatus, newstate, null)); }
        public MAPICONLIST AddIconToList(long CampID, short type, long ImageID, float x, float y, short Dragable, string str, long newstatus, long newstate)
        { return (AddIconToList(CampID, type, ImageID, x, y, Dragable, str, 0, 0, 0, newstatus, newstate, null)); }
        public bool UpdateInfo(long ID, float x, float y, long newstatus, long newstate)
        { throw new NotImplementedException(); }
        public bool UpdateInfo(MAPICONLIST icon, float x, float y, long newstatus, long newstate)
        { throw new NotImplementedException(); }
        public void RemoveIcon(long CampID)
        { throw new NotImplementedException(); }
        public bool ShowByType(long mask)
        { throw new NotImplementedException(); }
        public bool HideByType(long mask)
        { throw new NotImplementedException(); }
        public void Show()
        { throw new NotImplementedException(); }
        public void Hide()
        { throw new NotImplementedException(); }
        public void SetScaleFactor(float scale)
        { throw new NotImplementedException(); }
        public MAPICONLIST FindID(long ID)
        { throw new NotImplementedException(); }
        public MAPICONLIST GetLastItem() { return (Last_); }
        public void SetLabel(long ID, string txt)
        { throw new NotImplementedException(); }
        public void SetColor(long ID, COLORREF color)
        { throw new NotImplementedException(); }
        public string GetLabel(long ID)
        { throw new NotImplementedException(); }
        public void SetTextOffset(long ID, short x, short y)
        { throw new NotImplementedException(); }
        public long GetIconID() { if (Last_ != null) return (Last_.ID); return (0); }
        public void Refresh(MAPICONLIST icon)
        { throw new NotImplementedException(); }

        // Use SetMainImage... and when done, call this to update the IMAGE_RSC pointers
        public void RemapIconImages()
        { throw new NotImplementedException(); }

        public C_Hash GetRoot() { return (Root_); }

        public void SetTeam(short team) { Team_ = team; }
        public short GetTeam() { return (Team_); }

        public void SetFont(long ID) { Font_ = ID; }
        public long GetFont() { return (Font_); }

        public long GetHelpText()
        { throw new NotImplementedException(); }

        // Handler/Window Functions
        public long CheckHotSpots(long relX, long relY)
        { throw new NotImplementedException(); }
        public bool Dragable(long p) { return (GetFlags().IsFlagSet(UI95_BITTABLE.C_BIT_DRAGABLE)); }
        public bool Process(long ID, short ButtonHitType)
        { throw new NotImplementedException(); }
        public bool MouseOver(long relX, long relY, C_Base base_)
        { throw new NotImplementedException(); }
        public void GetItemXY(long ID, out long x, out long y)
        { throw new NotImplementedException(); }
        public bool Drag(GRABBER Drag, WORD MouseX, WORD MouseY, C_Window over)
        { throw new NotImplementedException(); }
        public bool Drop(GRABBER g, WORD p1, WORD p2, C_Window w) { return false; }
        public void Refresh()
        { throw new NotImplementedException(); }
        public void Draw(SCREEN surface, UI95_RECT cliprect)
        { throw new NotImplementedException(); }
    }


}
