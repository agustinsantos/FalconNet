using FalconNet.Common;
using FalconNet.Ui95;
using System;
using System.IO;
using WORD = System.Int16;
using COLORREF = System.Int32;

namespace FalconNet.UIExt
{


    public class C_BullsEye : C_Base
    {
#if  USE_SH_POOLS
	public:
		// Overload new/delete to use a SmartHeap pool
		void *operator new(size_t size) { return MemAllocPtr(UI_Pools[UI_CONTROL_POOL],size,FALSE);	};
		void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
#endif

        private UI95_BITTABLE DefaultFlags_;
        private COLORREF Color_;
        private float Scale_;
        private float WorldX_, WorldY_;


        public C_BullsEye()
            : base()
        {
            _SetCType_(UI95_ENUM._CNTL_BULLSEYE_);
            Color_ = 0;
            Scale_ = 1.0f;
            DefaultFlags_ = UI95_BITTABLE.C_BIT_ENABLED;
            WorldX_ = WorldY_ = 0;
        }
        public C_BullsEye(byte[] stream, ref int pos)
            : base(stream, ref pos)
        {
        }
        public C_BullsEye(FileStream fp)
            : base(fp)
        {
        }
        //TODO public ~C_BullsEye();
        public override long Size()
        {
            return (0);
        }

        public override void Save(byte[] stream, ref int pos) { ; }
        public override void Save(FileStream fp) { ; }
        public void Setup(long ID, UI95_ENUM Type)
        {
            SetID(ID);
            SetType(Type);
            SetDefaultFlags();
            SetReady(true);
        }

        public void SetColor(COLORREF color) { Color_ = color; }
        public void SetPos(float x, float y)
        {
            WorldX_ = x;
            WorldY_ = y;

            SetXY((long)(WorldX_ * Scale_), (long)(WorldY_ * Scale_));//! 
        }

        public void SetScale(float scl)
        {
            Scale_ = scl;
            SetXY((long)(WorldX_ * Scale_), (long)(WorldY_ * Scale_));
            SetWH((short)(BullsEyeLines[0, 1] * Scale_) * 2, (short)(BullsEyeLines[0, 1] * Scale_) * 2);
        }

        public void Cleanup()
        {
        }

        public void SetDefaultFlags() { SetFlags(DefaultFlags_); }
        public UI95_BITTABLE GetDefaultFlags() { return (DefaultFlags_); }
        public void Refresh()
        {
            if (GetFlags().IsFlagSet(UI95_BITTABLE.C_BIT_INVISIBLE) || Parent_ == null)
                return;

            Parent_.SetUpdateRect(GetX() - GetW() / 2, GetY() - GetH() / 2, GetX() + GetW() / 2 + 1, GetY() + GetH() / 2 + 1, GetFlags(), GetClient());
        }

        public void Draw(SCREEN surface, UI95_RECT cliprect)
        {
            long i;
            long x1, y1, x2, y2;

            if (GetFlags().IsFlagSet(UI95_BITTABLE.C_BIT_INVISIBLE) || Parent_ == null)
                return;

            for (i = 0; i < NUM_BE_CIRCLES; i++)
            {
                Parent_.DrawCircle(surface, Color_, GetX(), GetY(), BullsEyeRadius[i] * Scale_, GetFlags(), GetClient(), cliprect);
            }

            for (i = 0; i < NUM_BE_LINES; i++)
            {
                x1 = GetX() + (short)(BullsEyeLines[i, 0] * Scale_);
                y1 = GetY() + (short)(BullsEyeLines[i, 1] * Scale_);
                x2 = GetX() + (short)(BullsEyeLines[i, 2] * Scale_);
                y2 = GetY() + (short)(BullsEyeLines[i, 3] * Scale_);
                Parent_.DrawLine(surface, Color_, x1, y1, x2, y2, (long)GetFlags(), (long)GetClient(), cliprect);
            }
        }

        public const int NUM_BE_LINES = (6);
        public const int NUM_BE_CIRCLES = (6);
        private const float BE_RAD = 190.0f;// in feet
        private const float NM_TO_FT = 6076.211F;
        private static float[,] BullsEyeLines = new float[NUM_BE_LINES, 4]{
	                { 0.0000000f * BE_RAD * NM_TO_FT, -1.0000000f * BE_RAD * NM_TO_FT,  0.0000000f * BE_RAD * NM_TO_FT,  1.0000000f * BE_RAD * NM_TO_FT }, // 0
	                { 0.5000000f * BE_RAD * NM_TO_FT, -0.8660254f * BE_RAD * NM_TO_FT, -0.5000000f * BE_RAD * NM_TO_FT,  0.8660254f * BE_RAD * NM_TO_FT }, // 30
	                { 0.8660254f * BE_RAD * NM_TO_FT, -0.5000000f * BE_RAD * NM_TO_FT, -0.8660254f * BE_RAD * NM_TO_FT,  0.5000000f * BE_RAD * NM_TO_FT }, // 60
	                { 1.0000000f * BE_RAD * NM_TO_FT, 0.0000000f  * BE_RAD * NM_TO_FT, -1.0000000f * BE_RAD * NM_TO_FT,  0.0000000f * BE_RAD * NM_TO_FT }, // 90
	                { 0.8660254f * BE_RAD * NM_TO_FT, 0.5000000f  * BE_RAD * NM_TO_FT, -0.8660254f * BE_RAD * NM_TO_FT, -0.5000000f * BE_RAD * NM_TO_FT }, // 120
	                { 0.5000000f * BE_RAD * NM_TO_FT, 0.8660254f  * BE_RAD * NM_TO_FT, -0.5000000f * BE_RAD * NM_TO_FT, -0.8660254f * BE_RAD * NM_TO_FT }, // 150
                };

        private static float[] BullsEyeRadius = new float[NUM_BE_CIRCLES]{
	                    30.0f * NM_TO_FT,
	                    60.0f * NM_TO_FT,
	                    90.0f * NM_TO_FT,
	                    120.0f * NM_TO_FT,
	                    150.0f * NM_TO_FT,
	                    180.0f * NM_TO_FT,
                };
    }

}
