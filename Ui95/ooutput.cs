using System;
using System.IO;
using WORD = System.Int16;
using COLORREF = System.Int32;

namespace FalconNet.Ui95
{


    public class WORDWRAP
    {
#if USE_SH_POOLS
	public:
		// Overload new/delete to use a SmartHeap pool
		void *operator new(size_t size) { return MemAllocPtr(UI_Pools[UI_GENERAL_POOL],size,FALSE);	};
		void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
#endif

        public short Index;
        public short Length;
        public short y;
    }

    public class O_Output
    {
#if USE_SH_POOLS
	public:
		// Overload new/delete to use a SmartHeap pool
		void *operator new(size_t size) { return MemAllocPtr(UI_Pools[UI_GENERAL_POOL],size,FALSE);	};
		void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
#endif

        // Stuff that can be saved
        protected long flags_;
        protected long Font_;
        protected long ScaleSet_;
        protected COLORREF FgColor_, BgColor_;
        protected UI95_RECT Src_, Dest_;
        protected long origx_, origy_;
        protected long x_, y_; // relative x,y (offset from the Draw(x,y,...) parameters
        protected long w_, h_; // relative w,h
        protected long lastx_, lasty_, lastw_, lasth_;
        protected OUT _OType_;
        protected short animtype_;
        protected short frame_;
        protected short direction_;
        protected short ready_;
        protected short fperc_, bperc_;
        protected short LabelLen_;
        protected short WWWidth_;
        protected short WWCount_;
        protected short OpStart_, OpEnd_;

        // Don't save this stuff
        protected long[] Rows_;
        protected long[] Cols_;
        protected WORDWRAP Wrap_;
        protected string Label_;
        protected IMAGE_RSC Image_;
        protected ANIM_RES Anim_;
        protected C_Base Owner_; // pointer to creator control

        protected void ExtractAnim(SCREEN surface, long FrameNo, long x, long y, UI95_RECT src, UI95_RECT dest)
        {
            switch (Anim_.Anim.BytesPerPixel)
            {
                case 2:
                    switch (Anim_.Anim.Compression)
                    {
                        case 0:
                            Extract16Bit(surface, FrameNo, x, y, src, dest);
                            break;
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                            Extract16BitRLE(surface, FrameNo, x, y, src, dest);
                            break;
                    }
                    break;
            }
        }

        protected void Extract16BitRLE(SCREEN surface, long FrameNo, long x, long y, UI95_RECT src, UI95_RECT dest)
        {
#if TODO
	//src;		//!	Unused

	long i,dx,dy,done;
	WORD Key,count;
	ANIM_FRAME FramePtr;
	WORD *sptr;
	WORD *dptr,*lptr;

	i=0;
	FramePtr=(ANIM_FRAME *)&Anim_.Anim.Start[0];
	while(i < FrameNo && i < Anim_.Anim.Frames)
	{
		FramePtr=(ANIM_FRAME *)&FramePtr.Data[FramePtr.Size];
		i++;
	}
	sptr=(WORD *)&FramePtr.Data[0];
	lptr=(WORD *)surface.mem;
	lptr+=(desty * surface.width + destx);
	dptr=lptr;
	dx=destx;
	dy=desty;
	done=0;

	while(!done)
	{
		Key   = (WORD)(*sptr & RLE_KEYMASK);
		count = (WORD)(*sptr & RLE_COUNTMASK);
		sptr++;

		if(Key & RLE_END)
			done=1;
		else if(dy < clip.top)
		{
			// go through compressed stuff, & don't do anything for output
			if(!(Key & RLE_KEYMASK))
			{
				sptr+=count;
			}
			else if(Key & RLE_REPEAT)
			{
				sptr++;
			}
			else if(Key & RLE_SKIPCOL)
			{
			}
			else if(Key & RLE_SKIPROW)
			{
				lptr+=(count * surface.width);
				dptr=lptr;
				dx=destx;
				dy+=count;
			}
		}
		else
		{
			if(!(Key & RLE_KEYMASK))
			{
				while(count > 0)
				{
					if(dx >= clip.left && dx < clip.right)
					{
						*dptr=*sptr;
						dptr++;
					}
					else
						dptr++;
					dx++;
					sptr++;
					count--;
				}
			}
			else if(Key & RLE_REPEAT)
			{
				while(count > 0)
				{
					if(dx >= clip.left && dx < clip.right)
					{
						*dptr=*sptr;
						dptr++;
					}
					else
						dptr++;
					dx++;
					count--;
				}
				sptr++;
			}
			else if(Key & RLE_SKIPCOL)
			{
				dptr+=count;
				dx+=count;
			}
			else if(Key & RLE_SKIPROW)
			{
				lptr+=(count * surface.width);
				dptr=lptr;
				dx=destx;
				dy+=count;
			}
			if(dy >= clip.bottom || dy >= surface.height)
				done=1;
		}
	}
#endif
            throw new NotImplementedException();
        }


        protected void Extract16Bit(SCREEN s, long p1, long p2, long p3, UI95_RECT r1, UI95_RECT r2)
        {
#if TODO
#if NOTHING
	ANIM_FRAME *Frame;
	WORD *sptr;
	WORD *dptr;
	long i;

	i=0;
	Frame=(ANIM_FRAME *)&Anim_.Anim.Start[0];
	dptr=(WORD *)Mem;
	while(i < FrameNo && i < Anim_.Anim.Frames)
	{
		Frame=(ANIM_FRAME *)&Frame.Data[Frame.Size];
		i++;
	}

	sptr=(WORD *)&Frame.Data[0];
	dptr+=(y+dy*w)+x+dx;
	sptr+=(dy*Anim_.Anim.Width)+dx;
	for(i=0;i<dh;i++)
	{
		memmove(dptr,sptr,dw*Anim_.Anim.BytesPerPixel);
		dptr+=w;
		sptr+=Anim_.Anim.Width;
	}
#endif
#endif
            throw new NotImplementedException();
        }

        protected int FitString(int idx) // returns # characters to keep on this line
        { // idx is the index into the start of the string
#if TODO 
            int count, space;
            long w;	//! 

            if (!Label_[idx])
                return (0);

            w = WWWidth_;
            if (!w)
                w = Owner_.GetW();
            if (!w)
                return ((short)_tcsclen(&Label_[idx]));	   //! 
            space = 0;
            count = 1;
            while (Label_[idx + count] && gFontList.StrWidth(Font_, &Label_[idx], (short)count) < w)//! 
            {
                if (Label_[idx + count] == ' ')
                    space = count;
                // 2002-02-24 ADDED BY S.G. If we get a '\n', that's it for this line!
                if (Label_[idx + count] == '\n')
                {
                    space = count;
                    break;
                }
                // END OF ADDED SECTION 2002-02-24
                count++;
            }

            if (gFontList.StrWidth(Font_, &Label_[idx], (short)count) < w)//! 
                return (count);
            if (space)
                return (space);
            if (count > 1)
                return ((short)(count - 1));	//! 
            return (1);
#endif
            throw new NotImplementedException();
        }

        protected void WordWrap() // handles the word wrapping stuff
        {
#if TODO
	int idx=0, len=0,fontheight=0, linew=0, maxw=0;
	short count=0,lenstr=0;

	F4CSECTIONHANDLE Leave=null;

	if(Owner_ && Owner_.Parent_)
		Leave=UI_Enter(Owner_.Parent_);

	if(Label_[0]) // pre-calc wordwrapping - 2 pass, 1st to figure out how many lines, 2nd to actually do it
	{
		idx=0;
		count=0;
		lenstr=(short)_tcsclen(Label_);//! 

		len=FitString(idx); 
		while((idx+len) < lenstr)
		{
			count++;
			idx += len;
			while(Label_[idx] == ' ' || Label_[idx] == '\n') // 2002-02-24 MODIFIED BY S.G. Skip '\n' as well since these will 'terminate' word wrapped line (like they should) and need to be skipped for the next one
				idx++;
			len=FitString(idx);
			linew=gFontList.StrWidth(Font_,&Label_[idx],len);  
			if(linew > maxw)
				maxw=linew;
		}
		if(count)
		{
			count++;
			if(WWCount_ && WWCount_ != count)
			{
				WWCount_=0;
				if(Wrap_)
					delete Wrap_ ;
				Wrap_=null;
			}
			WWCount_=count;
			if(!Wrap_)
				Wrap_=new WORDWRAP[WWCount_];

			fontheight=gFontList.GetHeight(Font_);
			idx=0;
			count=0;
			len=FitString(idx);
			linew=gFontList.StrWidth(Font_,&Label_[idx],len); 
			if(linew > maxw)
				maxw=linew;
			while((idx+len) < lenstr)
			{
				Wrap_[count].Index=(short)idx;				//! 
				Wrap_[count].Length=(short)len;				//! 
				Wrap_[count].y=(short)(count * fontheight);	//! 

				count++;
				idx+=len;
				while(Label_[idx] == ' ')
					idx++;
				len=FitString(idx);
				linew=gFontList.StrWidth(Font_,&Label_[idx],len);
				if(linew > maxw)
					maxw=linew;
			}
			Wrap_[count].Index	= (short)idx;					//! 
			Wrap_[count].Length	= (short)len;					//! 
			Wrap_[count].y		= (short)(count * fontheight);	//! 
		}
		else
		{
			WWCount_=0;
			if(Wrap_)
				delete Wrap_;
			Wrap_=null;
			count=0;
			maxw=gFontList.StrWidth(Font_,Label_);
		}
	}
	SetWH(maxw,(count+1)*gFontList.GetHeight(Font_));
	UI_Leave(Leave);
#endif
            throw new NotImplementedException();
        }



        public enum OUT
        {
            _OUT_TEXT_ = 100,
            _OUT_BITMAP_,
            _OUT_SCALEBITMAP_,
            _OUT_ANIM_,
            _OUT_FILL_,
        };

        public O_Output()
        {
            origx_ = 0;
            origy_ = 0;
            x_ = 0;
            y_ = 0;
            w_ = 0;
            h_ = 0;
            lastx_ = 0;
            lasty_ = 0;
            lastw_ = 0;
            lasth_ = 0;
            _OType_ = 0;
            flags_ = 0;
            animtype_ = 0;
            frame_ = 0;
            direction_ = 0;
            ready_ = 0;
            fperc_ = 100;
            bperc_ = 0;
            FgColor_ = 0xcccccc;
            BgColor_ = 0;
            Font_ = 1;
            LabelLen_ = 0;
            Src_.left = 0; Src_.top = 0; Src_.right = 0; Src_.bottom = 0;
            Dest_.left = 0; Dest_.top = 0; Dest_.right = 0; Dest_.bottom = 0;
            Rows_ = null;
            Cols_ = null;
            ScaleSet_ = 1;
            OpStart_ = 0;
            OpEnd_ = 10000;
            WWWidth_ = 0;
            WWCount_ = 0;
            Wrap_ = null;
            Label_ = null;
            Image_ = null;
            Anim_ = null;
            Owner_ = null;
        }

        public O_Output(byte[] stream, ref int pos)
        { throw new NotImplementedException(); }

        public O_Output(FileStream fp)
        { throw new NotImplementedException(); }

        //TODO public ~O_Output() { }
        public long Size()
        { throw new NotImplementedException(); }

        public void Save(byte[] stream, ref int pos)
        { throw new NotImplementedException(); }

        public void Save(FileStream fp)
        { throw new NotImplementedException(); }

        // Setting Functions
        public void _SetOType_(OUT type) { _OType_ = type; } // Private...Called by setup

        public void SetXY(long x, long y) { origx_ = x; origy_ = y; }
        public void SetX(long x) { origx_ = x; }
        public void SetY(long y) { origy_ = y; }
        public void SetWH(long w, long h) { w_ = w; h_ = h; }
        public void SetW(long w) { w_ = w; }
        public void SetH(long h) { h_ = h; }
        public void SetOwner(C_Base owner) { Owner_ = owner; SetInfo(); }
        public void SetReady(short val) { ready_ = val; }
        public void SetFlags(long flags) { flags_ = flags; }
        public void SetFont(long FID) { Font_ = FID; }
        public void SetFgColor(COLORREF color) { FgColor_ = color; }
        public void SetBgColor(COLORREF color) { BgColor_ = color; }
        public void SetFrame(long frame) { frame_ = (short)frame; }		//! 
        public void SetDirection(long dir) { direction_ = (short)dir; }		//! 
        public void SetAnimType(long type) { animtype_ = (short)type; }		//! 
        public void SetSrcRect(UI95_RECT rect) { Src_ = rect; }
        public void SetDestRect(UI95_RECT rect) { Dest_ = rect; }
        public void SetScaleInfo(long scale)
        {
#if TODO
            long i, k, dd;
            long st, sb, sl, sr;
            if (scale > 500) // Shrink
            {
                if (scale == ScaleSet_)
                    return;
                st = 0;
                sb = Image_.Header.h;
                sl = 0;
                sr = Image_.Header.w;
            }
            else // Grow
            {
                st = Src_.top;
                sb = Src_.bottom;
                sl = Src_.left;
                sr = Src_.right;
            }
            ScaleSet_ = scale;

            k = 0;
            dd = 0;
            for (i = st; i <= sb; i++)
            {
                dd -= 1000;
                while (dd <= 0)
                {
                    if (!F4IsBadWritePtr(&(Rows_[k + 1]), sizeof(short))) // JB 010304 CTD
                        Rows_[k++] = i;
                    dd += scale;
                }
            }
            k = 0;
            dd = 0;
            for (i = sl; i <= sr; i++)
            {
                dd -= 1000;
                while (dd <= 0)
                {
                    if (!F4IsBadWritePtr(&(Cols_[k + 1]), sizeof(short))) // JB 010304 CTD
                        Cols_[k++] = i;
                    dd += scale;
                }
            }
            if (Image_)
                SetReady(1);
            else
                SetReady(0);
#endif
            throw new NotImplementedException();
        }

        public void SetFrontPerc(long perc) { fperc_ = (short)perc; }			//! 
        public void SetBackPerc(long perc) { bperc_ = (short)perc; }			//! 
        public void SetOpaqueRange(short os, short oe) { OpStart_ = os; OpEnd_ = oe; }

        public void SetWordWrapWidth(long w) { WWWidth_ = (short)w; }			//! 

        // Query Functions
        public OUT _GetOType_() { return (_OType_); }

        public long GetX() { return (x_); }
        public long GetY() { return (y_); }
        public long GetW() { return (w_); }
        public long GetH() { return (h_); }
        public short Ready() { return (ready_); }
        public long GetFlags() { return (flags_); }
        public string GetText() { return (Label_); }
        public short GetTextBufferLen() { return (LabelLen_); }
        public IMAGE_RSC GetImage() { return (Image_); }
        public ANIM_RES GetAnim() { return (Anim_); }
        public COLORREF GetFgColor() { return (FgColor_); }
        public COLORREF GetBgColor() { return (BgColor_); }
        public short GetFrame() { return (frame_); }
        public short GetDirection() { return (direction_); }
        public short GetAnimType() { return (animtype_); }
        public C_Base Owner() { return (Owner_); }
        public UI95_RECT GetSrcRect() { return (Src_); }
        public UI95_RECT GetDestRect() { return (Dest_); }

        // Non Inline functions
        public long GetCursorPos(long relx, long rely) // Based on mouse location
        {
#if TODO
            C_Fontmgr cur;
            ulong i, j;//! 
            long x, y, w;

            if (_GetOType_() != OUT._OUT_TEXT_)
                return (0);

            cur = gFontList.Find(Font_);
            if (cur == null)
                return (0);

            if (WWCount_ && (flags_ & C_BIT_WORDWRAP))
            {
                if (rely < 0)
                    return (0);
                if (rely > (Wrap_[WWCount_ - 1].y + cur.Height()))
                    return ((short)_tcsclen(Label_));//! 

                i = 0;

                while (rely >= (Wrap_[i].y + cur.Height()) && i < (ulong)WWCount_)//! 
                    i++;

                j = 0;
                w = (cur.Width(&Label_[Wrap_[i].Index + j], 1) - 1) / 2;
                while (relx >= (cur.Width(&Label_[Wrap_[i].Index], j) - 1 + w) && j < (ulong)Wrap_[i].Length)//! 
                {
                    j++;
                    if (j < (ulong)Wrap_[i].Length)
                        w = (cur.Width(&Label_[Wrap_[i].Index + j], 1) - 1) / 2;
                }

                return ((short)(Wrap_[i].Index + j));//! 
            }
            else
            {
                x = 0;
                y = 0;

                if (rely < y)
                    return (0);
                if (rely > y + cur.Height())
                    return ((short)_tcsclen(Label_));//! 

                j = 0;
                w = (cur.Width(Label_, 1) - 1) / 2;
                while (relx >= (cur.Width(Label_, j) - 1 + x + w) && j < _tcsclen(Label_))
                {
                    j++;
                    if (j < _tcsclen(Label_))
                        w = (cur.Width(&Label_[j], 1) - 1) / 2;
                }
                return (j);
            }
            return (0);
#endif
            throw new NotImplementedException();
        }

        public void GetCharXY(short idx, ref long cx, ref long cy) // Based on cursor location
        {
#if TODO
            C_Fontmgr cur;
            short i;

            if (_GetOType_() != OUT._OUT_TEXT_)
                return;

            cur = gFontList.Find(Font_);
            if (!cur)
                return;

            cx = GetX();
            cy = GetY();

            if (WWCount_ && (flags_ & C_BIT_WORDWRAP))
            {
                for (i = 0; i < WWCount_; i++)
                {
                    if (idx <= Wrap_[i].Index + Wrap_[i].Length)
                    {
                        cx = cur.Width(&Label_[Wrap_[i].Index], idx - Wrap_[i].Index);
                        cy = Wrap_[i].y;
                        return;
                    }
                }
            }
            else
            {
                if (GetFlags() & C_BIT_PASSWORD)
                {
                    cx += cur.Width("*") * idx;
                }
                else
                    cx += cur.Width(Label_, idx);
            }
#endif
            throw new NotImplementedException();
        }

        public void Refresh()
        {
#if TODO
            long x, y, w, h;
            if (!Ready()) return;

            if (_GetOType_() == OUT._OUT_SCALEBITMAP_)
            {
                Owner_.Parent_.update_ |= C_DRAW_REFRESH;
                Owner_.Parent_.SetUpdateRect(Owner_.Parent_.ClientArea_[Owner_.GetClient()].left,
                                               Owner_.Parent_.ClientArea_[Owner_.GetClient()].top,
                                               Owner_.Parent_.ClientArea_[Owner_.GetClient()].right,
                                               Owner_.Parent_.ClientArea_[Owner_.GetClient()].bottom,
                                               C_BIT_ABSOLUTE, Owner_.GetClient());
            }
            else
            {
                x = Owner_.GetX() + GetX();
                y = Owner_.GetY() + GetY();
                w = x + GetW();
                h = y + GetH();

                if (x < lastx_)
                    lastx_ = x;
                if (y < lasty_)
                    lasty_ = y;
                if (w > lastw_)
                    lastw_ = w;
                if (h > lasth_)
                    lasth_ = h;
                Owner_.Parent_.SetUpdateRect(lastx_, lasty_, lastw_, lasth_, Owner_.GetFlags(), Owner_.GetClient());
                lastx_ = x;
                lasty_ = y;
                lastw_ = w;
                lasty_ = h;
            }
#endif
            throw new NotImplementedException();
        }

        public void Draw(SCREEN surface, UI95_RECT cliprect)
        {
#if TODO
            if (!Ready()) return;

            switch (_GetOType_())
            {
                case OUT._OUT_FILL_:
                    UI95_RECT src, dest;

                    dest.left = Owner_.GetX() + GetX();
                    dest.top = Owner_.GetY() + GetY();
                    dest.right = dest.left + GetW();
                    dest.bottom = dest.top + GetH();

                    if (!(Owner_.GetFlags() & C_BIT_ABSOLUTE))
                    {
                        dest.left += Owner_.Parent_.VX_[Owner_.GetClient()];
                        dest.top += Owner_.Parent_.VY_[Owner_.GetClient()];
                        dest.right += Owner_.Parent_.VX_[Owner_.GetClient()];
                        dest.bottom += Owner_.Parent_.VY_[Owner_.GetClient()];
                    }
                    if (!Owner_.Parent_.ClipToArea(&src, &dest, cliprect))
                        return;

                    if (!(Owner_.GetFlags() & C_BIT_ABSOLUTE))
                        if (!Owner_.Parent_.ClipToArea(&src, &dest, &Owner_.Parent_.ClientArea_[Owner_.GetClient()]))
                            break;

                    Owner_.Parent_.BlitFill(surface, FgColor_, &dest, C_BIT_ABSOLUTE, 0);
                    break;

                case OUT._OUT_TEXT_:
                    {
                        long x, y, origx, origy, i;
                        long idx, len, lenout, nx;
                        UI95_RECT rect, dummy;
                        C_Fontmgr* cur;

                        x = GetX() + Owner_.GetX();
                        y = GetY() + Owner_.GetY();

                        if (WWCount_ && (flags_ & C_BIT_WORDWRAP))
                        {
                            if (!(Owner_.GetFlags() & C_BIT_ABSOLUTE))
                            {
                                x += Owner_.Parent_.VX_[Owner_.GetClient()];
                                y += Owner_.Parent_.VY_[Owner_.GetClient()];
                            }
                            rect.left = x;
                            rect.top = y;
                            rect.right = rect.left + GetW();
                            rect.bottom = rect.top + GetH();

                            if (!Owner_.Parent_.ClipToArea(&dummy, &rect, cliprect))
                                return;

                            if (!(Owner_.GetFlags() & C_BIT_ABSOLUTE))
                                if (!Owner_.Parent_.ClipToArea(&dummy, &rect, &Owner_.Parent_.ClientArea_[Owner_.GetClient()]))
                                    return;

                            x += Owner_.Parent_.GetX();
                            y += Owner_.Parent_.GetY();
                            rect.left += Owner_.Parent_.GetX();
                            rect.top += Owner_.Parent_.GetY();
                            rect.right += Owner_.Parent_.GetX();
                            rect.bottom += Owner_.Parent_.GetY();

                            cur = gFontList.Find(Font_);
                            if (cur)
                            {
                                for (i = 0; i < WWCount_; i++)
                                {
                                    if (GetFlags() & C_BIT_OPAQUE)
                                    {
                                        idx = Wrap_[i].Index;
                                        len = Wrap_[i].Length;
                                        nx = 0;
                                        lenout = 0;

                                        if (len && idx < OpStart_)
                                        {
                                            len = min(len, OpStart_ - idx);
                                            cur.Draw(surface, &Label_[idx], len, UI95_RGB24Bit(FgColor_), x, y + Wrap_[i].y, &rect);
                                            idx += len;
                                            lenout += len;
                                            len = Wrap_[i].Length - lenout;
                                            nx = cur.Width(&Label_[Wrap_[i].Index], lenout) - 1;
                                        }
                                        if (len && idx < OpEnd_)
                                        {
                                            len = min(len, OpEnd_ - idx);
                                            cur.DrawSolid(surface, &Label_[idx], len, UI95_RGB24Bit(FgColor_), UI95_RGB24Bit(BgColor_), x + nx, y + Wrap_[i].y, &rect);
                                            idx += len;
                                            lenout += len;
                                            len = Wrap_[i].Length - lenout;
                                            nx = cur.Width(&Label_[Wrap_[i].Index], lenout) - 1;
                                        }
                                        if (len)
                                        {
                                            cur.Draw(surface, &Label_[idx], len, UI95_RGB24Bit(FgColor_), x + nx, y + Wrap_[i].y, &rect);
                                        }
                                    }
                                    else
                                        cur.Draw(surface, &Label_[Wrap_[i].Index], Wrap_[i].Length, UI95_RGB24Bit(FgColor_), x, y + Wrap_[i].y, &rect);
                                }
                                if (flags_ & C_BIT_USELINE)
                                {
                                    for (i = 0; i < WWCount_; i++)
                                        Owner_.Parent_.DrawHLine(surface, FgColor_, GetX() + Owner_.GetX(), GetY() + Owner_.GetY() + Wrap_[i].y + cur.Height() - 1, Wrap_[i].Length, Owner_.GetFlags(), Owner_.GetClient(), cliprect);
                                }
                            }
                        }
                        else
                        {
                            origx = x;
                            origy = y;

                            if (!(Owner_.GetFlags() & C_BIT_ABSOLUTE))
                            {
                                x += Owner_.Parent_.VX_[Owner_.GetClient()];
                                y += Owner_.Parent_.VY_[Owner_.GetClient()];
                            }
                            rect.left = x;
                            rect.top = y;
                            rect.right = rect.left + GetW();
                            rect.bottom = rect.top + GetH();

                            if (!Owner_.Parent_.ClipToArea(&dummy, &rect, cliprect))
                                return;

                            if (!(Owner_.GetFlags() & C_BIT_ABSOLUTE))
                                if (!Owner_.Parent_.ClipToArea(&dummy, &rect, &Owner_.Parent_.ClientArea_[Owner_.GetClient()]))
                                    return;

                            x += Owner_.Parent_.GetX();
                            y += Owner_.Parent_.GetY();
                            rect.left += Owner_.Parent_.GetX();
                            rect.top += Owner_.Parent_.GetY();
                            rect.right += Owner_.Parent_.GetX();
                            rect.bottom += Owner_.Parent_.GetY();

                            cur = gFontList.Find(Font_);
                            if (cur)
                            {
                                if (GetFlags() & C_BIT_PASSWORD) // Password... draw asterixs
                                {
                                    memset(_password_, _T('*'), _tcsclen(Label_));
                                    if (GetFlags() & C_BIT_OPAQUE)
                                    {
                                        idx = 0;
                                        len = _tcsclen(_password_);
                                        nx = 0;
                                        lenout = 0;

                                        if (len && idx < OpStart_)
                                        {
                                            len = min(len, OpStart_ - idx);
                                            cur.Draw(surface, _password_, len, UI95_RGB24Bit(FgColor_), x, y, &rect);
                                            idx += len;
                                            lenout += len;
                                            len = _tcsclen(_password_) - lenout;
                                            nx = cur.Width(_password_, lenout) - 1;
                                        }
                                        if (len && idx < OpEnd_)
                                        {
                                            len = min(len, OpEnd_ - idx);
                                            cur.DrawSolid(surface, &_password_[idx], len, UI95_RGB24Bit(FgColor_), UI95_RGB24Bit(BgColor_), x + nx, y, &rect);
                                            idx += len;
                                            lenout += len;
                                            len = _tcsclen(_password_) - lenout;
                                            nx = cur.Width(_password_, lenout) - 1;
                                        }
                                        if (len)
                                        {
                                            cur.Draw(surface, &_password_[idx], len, UI95_RGB24Bit(FgColor_), x + nx, y, &rect);
                                        }
                                    }
                                    else
                                        cur.Draw(surface, _password_, UI95_RGB24Bit(FgColor_), x, y, &rect);
                                }
                                else
                                {
                                    if (GetFlags() & C_BIT_OPAQUE)
                                    {
                                        idx = 0;
                                        len = _tcsclen(Label_);
                                        nx = 0;
                                        lenout = 0;

                                        if (len && idx < OpStart_)
                                        {
                                            len = min(len, OpStart_ - idx);
                                            cur.Draw(surface, Label_, len, UI95_RGB24Bit(FgColor_), x, y, &rect);
                                            idx += len;
                                            lenout += len;
                                            len = _tcsclen(Label_) - lenout;
                                            nx = cur.Width(Label_, lenout) - 1;
                                        }
                                        if (len && idx < OpEnd_)
                                        {
                                            len = min(len, OpEnd_ - idx);
                                            cur.DrawSolid(surface, &Label_[idx], len, UI95_RGB24Bit(FgColor_), UI95_RGB24Bit(BgColor_), x + nx, y, &rect);
                                            idx += len;
                                            lenout += len;
                                            len = _tcsclen(Label_) - lenout;
                                            nx = cur.Width(Label_, lenout) - 1;
                                        }
                                        if (len)
                                        {
                                            cur.Draw(surface, &Label_[idx], len, UI95_RGB24Bit(FgColor_), x + nx, y, &rect);
                                        }
                                    }
                                    else
                                        cur.Draw(surface, Label_, UI95_RGB24Bit(FgColor_), x, y, &rect);
                                }
                                if (flags_ & C_BIT_USELINE)
                                    Owner_.Parent_.DrawHLine(surface, FgColor_, origx, origy + cur.Height() - 1, cur.Width(Label_), Owner_.GetFlags(), Owner_.GetClient(), cliprect);
                            }
                        }
                    }
                    break;
                case OUT._OUT_BITMAP_:
                    {
                        UI95_RECT src, dest;
                        src.left = 0;
                        src.top = 0;
                        src.right = Image_.Header.w;
                        src.bottom = Image_.Header.h;
                        dest.left = Owner_.GetX() + GetX();
                        dest.top = Owner_.GetY() + GetY();
                        dest.right = dest.left + GetW();
                        dest.bottom = dest.top + GetH();

                        if (!(Owner_.GetFlags() & C_BIT_ABSOLUTE))
                        {
                            dest.left += Owner_.Parent_.VX_[Owner_.GetClient()];
                            dest.top += Owner_.Parent_.VY_[Owner_.GetClient()];
                            dest.right += Owner_.Parent_.VX_[Owner_.GetClient()];
                            dest.bottom += Owner_.Parent_.VY_[Owner_.GetClient()];
                        }
                        if (!Owner_.Parent_.ClipToArea(&src, &dest, cliprect))
                            return;

                        if (!(Owner_.GetFlags() & C_BIT_ABSOLUTE))
                            if (!Owner_.Parent_.ClipToArea(&src, &dest, &Owner_.Parent_.ClientArea_[Owner_.GetClient()]))
                                break;

                        dest.left += Owner_.Parent_.GetX();
                        dest.top += Owner_.Parent_.GetY();
                        if (flags_ & C_BIT_TRANSLUCENT && fperc_ < 100)
                            Image_.Blend(surface, src.left, src.top, src.right - src.left, src.bottom - src.top, dest.left, dest.top, fperc_, 100 - fperc_);
                        else
                            Image_.Blit(surface, src.left, src.top, src.right - src.left, src.bottom - src.top, dest.left, dest.top);
                    }
                    break;
                case OUT._OUT_SCALEBITMAP_:
                    {
                        UI95_RECT dummy, clip;
                        clip = *cliprect;
                        if (!(Owner_.GetFlags() & C_BIT_ABSOLUTE))
                            if (!Owner_.Parent_.ClipToArea(&dummy, &clip, &Owner_.Parent_.ClientArea_[Owner_.GetClient()]))
                                break;

                        if (ScaleSet_ > 500)
                            Image_.ScaleDown8(surface, Rows_, Cols_, clip.left + Owner_.Parent_.GetX(), clip.top + Owner_.Parent_.GetY(), (clip.right - clip.left), (clip.bottom - clip.top), Src_.left * 1000 / ScaleSet_ + (clip.left - Dest_.left), Src_.top * 1000 / ScaleSet_ + (clip.top - Dest_.top));
                        else
                            Image_.ScaleUp8(surface, &Rows_[clip.top - Dest_.top], &Cols_[clip.left - Dest_.left], clip.left + Owner_.Parent_.GetX(), clip.top + Owner_.Parent_.GetY(), (clip.right - clip.left), (clip.bottom - clip.top));
                    }
                    break;
                case OUT._OUT_ANIM_:
                    {
                        UI95_RECT dest, src;
                        long dx, dy;

                        dest.left = Owner_.GetX() + GetX();
                        dest.top = Owner_.GetY() + GetY();

                        dest.right = dest.left + GetW();
                        dest.bottom = dest.top + GetH();

                        src.left = 0;
                        src.top = 0;
                        src.right = GetW();
                        src.bottom = GetH();

                        if (!(Owner_.GetFlags() & C_BIT_ABSOLUTE))
                        {
                            dest.left += Owner_.Parent_.VX_[Owner_.GetClient()];
                            dest.top += Owner_.Parent_.VY_[Owner_.GetClient()];
                            dest.right += Owner_.Parent_.VX_[Owner_.GetClient()];
                            dest.bottom += Owner_.Parent_.VY_[Owner_.GetClient()];
                        }

                        dx = dest.left + Owner_.Parent_.GetX();
                        dy = dest.top + Owner_.Parent_.GetY();

                        if (!Owner_.Parent_.ClipToArea(&src, &dest, cliprect))
                            return;

                        if (!(Owner_.GetFlags() & C_BIT_ABSOLUTE))
                            if (!Owner_.Parent_.ClipToArea(&src, &dest, &Owner_.Parent_.ClientArea_[Owner_.GetClient()]))
                                return;

                        dest.left += Owner_.Parent_.GetX();
                        dest.right += Owner_.Parent_.GetX();
                        dest.top += Owner_.Parent_.GetY();
                        dest.bottom += Owner_.Parent_.GetY();

                        ExtractAnim(surface, frame_, dx, dy, &src, &dest);
                    }
                    break;
            }
#endif
            throw new NotImplementedException();
        }

        public void Blend4Bit(SCREEN surface, byte[] overlay, WORD[] Palette, UI95_RECT cliprect)
        {
#if TODO
            UI95_RECT dummy, clip;
            clip = *cliprect;
            if (!(Owner_.GetFlags() & C_BIT_ABSOLUTE))
                if (!Owner_.Parent_.ClipToArea(&dummy, &clip, &Owner_.Parent_.ClientArea_[Owner_.GetClient()]))
                    return;

            if (ScaleSet_ > 500)
                Image_.ScaleDown8Overlay(surface, Rows_, Cols_, clip.left + Owner_.Parent_.GetX(), clip.top + Owner_.Parent_.GetY(), (clip.right - clip.left), (clip.bottom - clip.top), Src_.left * 1000 / ScaleSet_ + (clip.left - Dest_.left), Src_.top * 1000 / ScaleSet_ + (clip.top - Dest_.top), overlay, Palette);
            else
                Image_.ScaleUp8Overlay(surface, &Rows_[clip.top - Dest_.top], &Cols_[clip.left - Dest_.left], clip.left + Owner_.Parent_.GetX(), clip.top + Owner_.Parent_.GetY(), (clip.right - clip.left), (clip.bottom - clip.top), overlay, Palette);
#endif
            throw new NotImplementedException();
        }

        public void Cleanup()
        { throw new NotImplementedException(); }

        public void SetInfo()
        {
#if TODO
            switch (_GetOType_())
            {
                case OUT._OUT_FILL_:
                    SetReady(1);
                    if (flags_ & C_BIT_HCENTER)
                        x_ = (origx_ - (w_ >> 1));
                    else if (flags_ & C_BIT_RIGHT)
                        x_ = (origx_ - w_);
                    else
                        x_ = (origx_);

                    if (flags_ & C_BIT_VCENTER)
                        y_ = (origy_ - (h_ >> 1));
                    else
                        y_ = (origy_);
                    break;
                case OUT._OUT_TEXT_:
                    if (Label_)
                    {
                        SetReady(1);
                        if (GetFlags() & C_BIT_PASSWORD)
                            SetWH(gFontList.StrWidth(Font_, "*") * _tcsclen(Label_), gFontList.GetHeight(Font_));
                        else
                            SetWH(gFontList.StrWidth(Font_, Label_), gFontList.GetHeight(Font_));
                        if (Label_[0] && (flags_ & C_BIT_WORDWRAP) && Owner_ && GetW() > 50)
                            WordWrap(); // Sets WH internally
                        if (flags_ & C_BIT_HCENTER)
                            x_ = (origx_ - (w_ >> 1));
                        else if (flags_ & C_BIT_RIGHT)
                            x_ = (origx_ - w_);
                        else
                            x_ = (origx_);

                        if (flags_ & C_BIT_VCENTER)
                            y_ = (origy_ - (h_ >> 1));
                        else
                            y_ = (origy_);
                    }
                    else
                        SetReady(0);
                    break;
                case OUT._OUT_BITMAP_:
                    if (Image_)
                    {
                        SetReady(1);
                        if (flags_ & C_BIT_HCENTER)
                            x_ = (origx_ - Image_.Header.centerx);
                        else if (flags_ & C_BIT_RIGHT)
                            x_ = (origx_ - w_);
                        else
                            x_ = (origx_);

                        if (flags_ & C_BIT_VCENTER)
                            y_ = (origy_ - Image_.Header.centery);
                        else
                            y_ = (origy_);
                    }
                    else
                        SetReady(0);
                    break;
                case OUT._OUT_SCALEBITMAP_:
                    if (Image_ && ScaleSet_)
                        SetReady(1);
                    else
                        SetReady(0);
                    return; // centering Doesn't apply here
                    break;
                case OUT._OUT_ANIM_:
                    if (Anim_)
                    {
                        SetReady(1);
                        if (flags_ & C_BIT_HCENTER)
                            x_ = (origx_ - (w_ >> 1));
                        else if (flags_ & C_BIT_RIGHT)
                            x_ = (origx_ - w_);
                        else
                            x_ = (origx_);

                        if (flags_ & C_BIT_VCENTER)
                            y_ = (origy_ - (h_ >> 1));
                        else
                            y_ = (origy_);
                    }
                    else
                        SetReady(0);
                    break;
            }
#endif
            throw new NotImplementedException();
        }

        public void SetText(string str)
        {
#if TODO
            _SetOType_(OUT._OUT_TEXT_);

            if (Label_ == null)
            {
                if (flags_ & C_BIT_FIXEDSIZE && LabelLen_ > 0)
#if USE_SH_POOLS
			Label_=(_TCHAR*)MemAllocPtr(UI_Pools[UI_GENERAL_POOL],sizeof(_TCHAR)*(LabelLen_),FALSE);
#else
                    Label_ = new _TCHAR[LabelLen_];
#endif
            }
            if (txt)
            {
                if (flags_ & C_BIT_FIXEDSIZE)
                {
                    _tcsncpy(Label_, txt, LabelLen_ - 1);
                    Label_[LabelLen_ - 1] = 0;
                }
                else
                    Label_ = txt;
            }
            else if (Label_ && (flags_ & C_BIT_FIXEDSIZE))
                memset(Label_, 0, sizeof(_TCHAR) * (LabelLen_));

            SetInfo();
#endif
            throw new NotImplementedException();
        }

        public void SetTextWidth(long w)
        {
#if TODO
            _SetOType_(OUT._OUT_TEXT_);

            if (Label_)
            {
                if (flags_ & C_BIT_FIXEDSIZE)
                {
                    if (w == LabelLen_)
                        return;
#if USE_SH_POOLS
			MemFreePtr(Label_);
#else
                    delete Label_;
#endif
                }
                else
                {
                    MonoPrint("ERROR:Calling SetFixedWidth() when a string has already been assigned\n");
                }
            }
            LabelLen_ = (short)(w);
#if USE_SH_POOLS
	Label_=(_TCHAR*)MemAllocPtr(UI_Pools[UI_GENERAL_POOL],sizeof(_TCHAR)*(LabelLen_+1),FALSE);
#else
            Label_ = new _TCHAR[LabelLen_ + 1];
#endif
            memset(Label_, 0, sizeof(_TCHAR) * (LabelLen_));
            SetFlags(flags_ | C_BIT_FIXEDSIZE);
            SetInfo();
#endif
            throw new NotImplementedException();
        }

        public void SetImage(long ID)
        {
#if TODO
            IMAGE_RSC image;

            image = gImageMgr.GetImage(ID);
            if (image == null)
            {
                if (ID > 0)
                    MonoPrint("Image [%1ld] Not found in O_Output::SetImage(ID) Control=(%1ld)\n", ID, Owner_.GetID());
                SetReady(0);
                return;
            }
            SetImage(image);
#endif
            throw new NotImplementedException();
        }

        public void SetImage(IMAGE_RSC img)
        {
#if TODO
            _SetOType_(_OUT_BITMAP_);

            Image_ = newimage;
            if (Image_ == null)
            {
                MonoPrint("Image Pointer is null in O_Output::SetImage(image*) Control=(%1ld)\n", Owner_.GetID());
                SetReady(0);
                return;
            }
            SetWH(Image_.Header.w, Image_.Header.h);
            SetInfo();
#endif
            throw new NotImplementedException();
        }

        public void SetImagePtr(IMAGE_RSC img)
        {
#if TODO
            _SetOType_(_OUT_BITMAP_);

            Image_ = newimage;
            if (Image_ == null)
            {
                MonoPrint("Image Pointer is null in O_Output::SetImage(image*) Control=(%1ld)\n", Owner_.GetID());
                SetReady(0);
                return;
            }
            SetInfo();
#endif
            throw new NotImplementedException();
        }

        public void SetScaleImage(long ID)
        {
#if TODO
            IMAGE_RSC  image;

            image = gImageMgr.GetImage(ID);
            if (image == null)
            {
                if (ID > 0)
                    MonoPrint("Image [%1ld] Not found in O_Output::SetImage(ID) Control=(%1ld)\n", ID, Owner_.GetID());
                SetReady(0);
                return;
            }
            SetScaleImage(image);
#endif
            throw new NotImplementedException();
        }

        public void SetScaleImage(IMAGE_RSC img)
        {
#if TODO
            _SetOType_(_OUT_SCALEBITMAP_);

            Image_ = newimage;
            if (Image_ == null)
            {
                MonoPrint("Image Pointer is null in O_Output::SetImage(image*) Control=(%1ld)\n", Owner_.GetID());
                SetReady(0);
                return;
            }

            SetXY(0, 0);
            if (Rows_ == null)
            {
#if USE_SH_POOLS
		Rows_=(long*)MemAllocPtr(UI_Pools[UI_ART_POOL],sizeof(long)*(Image_.Header.h*2),FALSE);
#else
                Rows_ = new long[Image_.Header.h * 2];
#endif
                memset(Rows_, 0, sizeof(long) * Image_.Header.h * 2);
            }
            if (Cols_ == null)
            {
#if USE_SH_POOLS
		Cols_=(long*)MemAllocPtr(UI_Pools[UI_ART_POOL],sizeof(long)*(Image_.Header.w*2),FALSE);
#else
                Cols_ = new long[Image_.Header.w * 2];
#endif
                memset(Cols_, 0, sizeof(long) * Image_.Header.w * 2);
            }
            ScaleSet_ = 0;
            SetWH(Image_.Header.w, Image_.Header.h);
            SetInfo();
#endif
            throw new NotImplementedException();
        }

        public void SetAnim(long ID)
        {
#if TODO
            ANIM_RES  anim;

            anim = gAnimMgr.GetAnim(ID);
            if (anim == null)
            {
                MonoPrint("Anim [%1ld] Not found in O_Output::SetAnim(ID) Control=(%1ld)\n", ID, Owner_.GetID());
                SetReady(0);
                return;
            }
            SetAnim(anim);
#endif
            throw new NotImplementedException();
        }

        public void SetAnim(ANIM_RES anim)
        {
#if TODO
            _SetOType_(_OUT_ANIM_);

            Anim_ = newanim;
            if (Anim_ == null)
            {
                MonoPrint("Anim Pointer is null in O_Output::SetAnim(image*) Control=(%1ld)\n", Owner_.GetID());
                SetReady(0);
                return;
            }
            SetWH(Anim_.Anim.Width, Anim_.Anim.Height);
            SetInfo();
#endif
            throw new NotImplementedException();
        }

        public void SetFill()
        {
            _SetOType_(OUT._OUT_FILL_);

            SetInfo();
        }
    }
}
