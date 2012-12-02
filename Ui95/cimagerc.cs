using System;
using WORD=System.UInt16;

namespace FalconNet.Ui95
{
	public class IMAGE_RSC
	{
	}
	
	public class C_Image
	{
		public enum CIMG
		{
			CIMG_NOTHING=0,
			CIMG_LOADIMAGE,
			CIMG_LOADFILE,
			CIMG_ADDIMAGE,
			CIMG_LOADRES,
			CIMG_LOADPRIVATERES,
		};
		private static readonly string[] C_Img_Tokens =
		{
			"[NOTHING]",
			"[LOADIMAGE]",
			"[LOADFILE]",
			"[ADDIMAGE]",
			"[LOADRES]",
			"[LOADPRIVATERES]"
		};
		private const int _IMAGE_HASH_SIZE_ = 512;
		private const int _IMAGE_SUB_HASH_SIZE_ = 20;
		
		static void DelResCB (object me)
		{
			C_Resmgr res;
		
			res = (C_Resmgr)me;
			if (res != null) {
				res.Cleanup ();
				res = null;
			}
		}
		
#if USE_SH_POOLS
	public:
		// Overload new/delete to use a SmartHeap pool
		void *operator new(size_t size) { return MemAllocPtr(UI_Pools[UI_CONTROL_POOL],size,FALSE);	};
		void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
#endif
#if !_UI95_PARSER_
		private long	LastID_;
#endif
		private C_Hash	Root_;
		private C_Hash	Finder_;
		private C_Hash	ColorOrder_;
		private C_Hash	IDOrder_;
		private WORD	ColorKey_;
		private short	red_shift_, green_shift_, blue_shift_;

#if TODO
		private long BuildColorTable(WORD p1,long p2 ,long p3,long );
		private void MakePalette(WORD *,long );
		private void ConvertTo8Bit(WORD *,unsigned char *,long ,long );
#endif
		private void CopyArea (ref WORD src, ref WORD dest, long w, long h)
		{
		#if TODO
			long i,j,didx,start,sidx;
		
			if(!src || !w || !h || !dest)
				return;
		
			if(dest)
			{
				didx=0;
				start=(h-1)*w;
				for(i=0;i<h;i++)
				{
					sidx=start;
					for(j=0;j<w;j++)
						dest[didx++]=src[sidx++];
					start-=w;
				}
			}
		#endif
		}
	
		public C_Image ()
		{
			Root_ = null;
			Finder_ = null;
			ColorKey_ = 0x7c1f;
			red_shift_ = 0;
			green_shift_ = 0;
			blue_shift_ = 0;
			ColorOrder_ = null;
			IDOrder_ = null;
		
		#if ! _UI95_PARSER_
			LastID_ = 0;
		#endif
		}
		//TODO public ~C_Image();

		public void Setup ()
		{
			if (Root_ != null || Finder_ != null)
				Cleanup ();
		
			Root_ = new C_Hash ();
			Root_.Setup (_IMAGE_HASH_SIZE_);
			Root_.SetFlags (UI95_BITTABLE.C_BIT_REMOVE);
			Root_.SetCallback (DelResCB);
		
			Finder_ = new C_Hash ();
			Finder_.Setup (_IMAGE_HASH_SIZE_);
		}

		public void Cleanup ()
		{
			if (Root_ != null) {
				Root_.Cleanup ();
				Root_ = null;
			}
			if (Finder_ != null) {
				Finder_.Cleanup ();
				Finder_ = null;
			}
			if (ColorOrder_ != null) {
				ColorOrder_.Cleanup ();
				ColorOrder_ = null;
			}
			if (IDOrder_ != null) {
				IDOrder_.Cleanup ();
				IDOrder_ = null;
			}
		}

		public void SetScreenFormat (short rs, short gs, short bs)
		{
			red_shift_ = rs;
			green_shift_ = gs;
			blue_shift_ = bs;
		}

		public C_Resmgr AddImage (long ID, long LastID, UI95_RECT rect, short x, short y)
		{
#if TODO	
	C_Resmgr *newres=null;
	IMAGE_RSC *newentry=null;
	IMAGE_RSC *prior=null;
	char *orig8=null;
	char *data8=null;
	char *dptr8=null;
	char *sptr8=null;
	WORD *orig16=null;
	WORD *data16=null;
	WORD *dptr16=null;
	WORD *sptr16=null;
	WORD *Palette=null;
	int neww=0,newh=0;	//! 
	int i=0,j=0;		//! 

	if(Root_.Find(ID) || Finder_.Find(ID))
	{
		MonoPrint("cimagerc error: [ID %1ld] Already used (AddImage)\n",ID);
		return(null);
	}
	prior=(IMAGE_RSC*)Finder_.Find(LastID);
	if(!prior)
	{
		MonoPrint("NO prior image to reference (%1ld)\n",ID);
		return(null);
	}
	if(prior.Header.Type != _RSC_IS_IMAGE_)
	{
		MonoPrint("(%1ld) is NOT an IMAGE_RSC (type=%1d)\n",ID,prior.Header.Type);
		return(null);
	}

	if(rect.left >= prior.Header.w || rect.top >= prior.Header.h)
	{
		MonoPrint("AddImage [ID %1ld] is outside of prior image's [ID %1ld] area\n",ID,LastID);
		return(null);
	}
	if(rect.right > prior.Header.w)
		rect.right=prior.Header.w;
	if(rect.bottom > prior.Header.h)
		rect.bottom=prior.Header.h;

	neww=rect.right  - rect.left;
	newh=rect.bottom - rect.top;

	if(prior.Header.flags & _RSC_8_BIT_)
	{
#if USE_SH_POOLS
		data8=(char*)MemAllocPtr(UI_Pools[UI_ART_POOL],sizeof(char)*(neww*newh+(prior.Header.palettesize*2)),FALSE);
#else
		data8=new char[neww*newh+(prior.Header.palettesize*2)];
#endif
		Palette=(WORD*)(data8+neww*newh);
		memcpy((char *)Palette,(char *)(prior.Owner.GetData()+prior.Header.paletteoffset),prior.Header.palettesize*2);
		orig8=(char *)(prior.Owner.GetData() + prior.Header.imageoffset);
		dptr8=data8;
		orig8+=rect.top*prior.Header.w;
		for(i=rect.top;i<rect.bottom;i++)
		{
			sptr8=orig8+rect.left;
			for(j=rect.left;j <rect.right;j++)
				*dptr8++=*sptr8++;
			orig8+=prior.Header.w;
		}
	}
	else
	{
#if USE_SH_POOLS
		data16=(WORD*)MemAllocPtr(UI_Pools[UI_ART_POOL],sizeof(WORD)*(neww*newh),FALSE);
#else
		data16=new WORD[neww*newh];
#endif
		orig16=(WORD *)(prior.Owner.GetData() + prior.Header.imageoffset);
		dptr16=data16;
		orig16+=rect.top*prior.Header.w;
		for(i=rect.top;i<rect.bottom;i++)
		{
			sptr16=orig16+rect.left;
			for(j=rect.left;j <rect.right;j++)
				*dptr16++=*sptr16++;
			orig16+=prior.Header.w;
		}
	}

	newres=new C_Resmgr();
	newres.Setup(ID);
	newres.SetScreenFormat(red_shift_,green_shift_,blue_shift_);
	newres.SetColorKey(ColorKey_);

	newentry=new IMAGE_RSC();
	newentry.ID=ID;
	newentry.Owner=newres;
	newentry.Header=new ImageHeader();
	newentry.Header.Type=_RSC_IS_IMAGE_;
	newentry.Header.ID[0]=0;
	newentry.Header.flags=prior.Header.flags|_RSC_USECOLORKEY_;
	newentry.Header.centerx=x;
	newentry.Header.centery=y;
	newentry.Header.w=(short)neww;//! 
	newentry.Header.h=(short)newh;//! 
	newentry.Header.imageoffset=0;
	if(newentry.Header.flags & _RSC_8_BIT_)
	{
		newres.SetData(data8);
		newentry.Header.palettesize=prior.Header.palettesize;
		newentry.Header.paletteoffset=neww*newh;
	}
	else
	{
		newentry.Header.palettesize=0;
		newentry.Header.paletteoffset=0;
		newres.SetData((char *)data16);
	}
	newres.AddIndex(ID,newentry);

	Root_.Add(ID,newres);
	Finder_.Add(ID,newentry);

	return(newres);
#endif 
			throw new NotImplementedException ();
		}

		public C_Resmgr AddImage (long ID, long LastID, short x, short y, short w, short h, short cx, short cy)
		{
#if TODO			
	C_Resmgr *newres=null;
	IMAGE_RSC *newentry=null;
	IMAGE_RSC *prior=null;
	char *orig8=null;
	char *data8=null;
	char *dptr8=null;
	char *sptr8=null;
	WORD *orig16=null;
	WORD *data16=null;
	WORD *dptr16=null;
	WORD *sptr16=null;
	WORD *Palette=null;
	int neww=0,newh=0;	//! 
	int i=0,j=0;		//! 

	if(Root_.Find(ID) || Finder_.Find(ID))
	{
		MonoPrint("cimagerc error: [ID %1ld] Already used (AddImage)\n",ID);
		return(null);
	}
	prior=(IMAGE_RSC*)Finder_.Find(LastID);
	if(!prior)
	{
		MonoPrint("NO prior image to reference (%1ld)\n",ID);
		return(null);
	}
	if(prior.Header.Type != _RSC_IS_IMAGE_)
	{
		MonoPrint("(%1ld) is NOT an IMAGE_RSC (type=%1d)\n",ID,prior.Header.Type);
		return(null);
	}
	if(!prior.Owner)
	{
		MonoPrint("(%1ld) Data_ not loaded\n",ID,prior.Header.Type);
		return(null);
	}

	if(x >= prior.Header.w || y >= prior.Header.h)
	{
		MonoPrint("AddImage [ID %1ld] is outside of prior image's [ID %1ld] area\n",ID,LastID);
		return(null);
	}
	if((x+w) > prior.Header.w)
		w = (short)(prior.Header.w-x);//! 
	if((y+h) > prior.Header.h)
		h = (short)(prior.Header.h-y);//! 

	neww=w;
	newh=h;

	if(prior.Header.flags & _RSC_8_BIT_)
	{
#if USE_SH_POOLS
		data8=(char*)MemAllocPtr(UI_Pools[UI_ART_POOL],sizeof(char)*(neww*newh+(prior.Header.palettesize*2)),FALSE);
#else
		data8=new char[neww*newh+(prior.Header.palettesize*2)];
#endif
		Palette=(WORD*)(data8+neww*newh);
		memcpy((char *)Palette,(char *)(prior.Owner.GetData()+prior.Header.paletteoffset),prior.Header.palettesize*2);
		orig8=(char *)(prior.Owner.GetData() + prior.Header.imageoffset);
		dptr8=data8;
		orig8+=y*prior.Header.w;
		for(i=y;i<(y+h);i++)
		{
			sptr8=orig8+x;
			for(j=x;j <(x+w);j++)
				*dptr8++=*sptr8++;
			orig8+=prior.Header.w;
		}
	}
	else
	{
#if USE_SH_POOLS
		data16=(WORD*)MemAllocPtr(UI_Pools[UI_ART_POOL],sizeof(WORD)*(neww*newh),FALSE);
#else
		data16=new WORD[neww*newh];
#endif
		orig16=(WORD *)(prior.Owner.GetData() + prior.Header.imageoffset);
		dptr16=data16;
		orig16+=y*prior.Header.w;
		for(i=y;i<(y+h);i++)
		{
			sptr16=orig16+x;
			for(j=x;j <(x+w);j++)
				*dptr16++=*sptr16++;
			orig16+=prior.Header.w;
		}
	}

	newres=new C_Resmgr;
	newres.Setup(ID);
	newres.SetScreenFormat(red_shift_,green_shift_,blue_shift_);
	newres.SetColorKey(ColorKey_);

	newentry=new IMAGE_RSC;
	newentry.ID=ID;
	newentry.Owner=newres;
	newentry.Header=new ImageHeader;
	newentry.Header.Type=_RSC_IS_IMAGE_;
	newentry.Header.ID[0]=0;
	newentry.Header.flags=prior.Header.flags|_RSC_USECOLORKEY_;
	newentry.Header.centerx=cx;
	newentry.Header.centery=cy;
	newentry.Header.w=(short)neww;//! 
	newentry.Header.h=(short)newh;//! 
	newentry.Header.imageoffset=0;
	if(newentry.Header.flags & _RSC_8_BIT_)
	{
		newres.SetData(data8);
		newentry.Header.palettesize=prior.Header.palettesize;
		newentry.Header.paletteoffset=neww*newh;
	}
	else
	{
		newentry.Header.palettesize=0;
		newentry.Header.paletteoffset=0;
		newres.SetData((char*)data16);
	}
	newres.AddIndex(ID,newentry);

	Root_.Add(ID,newres);
	Finder_.Add(ID,newentry);

	return(newres);
#endif	
			throw new NotImplementedException ();
		}

		public C_Resmgr LoadImage (long ID, string file, short x, short y)
		{
			throw new NotImplementedException ();
		}

		public C_Resmgr LoadFile (long ID, string file, short x, short y)
		{
			throw new NotImplementedException ();
		}

		public C_Resmgr LoadRes (long ID, string filename)
		{
			//TODO C_HASHNODE current;
			long curidx;
			C_Resmgr res;
			C_Hash   resIDs;
			IMAGE_RSC rec;
		
			res=LoadPrivateRes(ID,filename);
#if TODO			
			if(res != null)
			{
				resIDs=res.GetIDList();
				if(resIDs)
				{
					rec=(IMAGE_RSC)resIDs.GetFirst(ref current, ref curidx);
					while(rec)
					{
						Finder_.Add(rec.ID,rec);
						rec=(IMAGE_RSC)resIDs.GetNext(ref current,ref curidx);
					}
					return(res);
				}
			}
#endif
			return(null);
		}


		public C_Resmgr LoadPrivateRes (long ID, string filename)
		{
			C_Resmgr res;
		
			if(ID == 0 || string.IsNullOrWhiteSpace(filename) || Root_ == null)
				return(null);
		
			if(Root_.Find(ID) != null)
				return(null);
		
			res=new C_Resmgr();
		
			res.Setup(ID,filename,null);//TOODO UI_Main.gMainParser.GetTokenHash());
			res.SetScreenFormat(red_shift_, green_shift_, blue_shift_);
			res.SetColorKey(ColorKey_);
			res.LoadData();
		
			Root_.Add(ID,res);
			return(res);
		}

		public void SetColorKey (WORD colorkey)
		{
			ColorKey_ = colorkey;
		}

		public IMAGE_RSC GetImage (long ID)
		{
			throw new NotImplementedException ();
		}

		public C_Resmgr GetImageRes (long ID)
		{
			throw new NotImplementedException ();
		}

		public bool RemoveImage (long ID)
		{
			throw new NotImplementedException ();
		}
#if ! _UI95_PARSER_
		public short LocalFind (string  token)
		{
			for (short i = 0; i < C_Img_Tokens.Length; i++) {
				if (token == C_Img_Tokens [i])
					return(i);
			}
			return(0);
		}

		public void LocalFunction (CIMG ID, long[] P, string str, C_Handler h)
		{
			switch (ID) {
			case CIMG.CIMG_LOADIMAGE:
				LoadImage (P [0], str, (short)P [1], (short)P [2]);
				break;
			case CIMG.CIMG_LOADFILE:
				LoadFile (P [0], str, (short)P [1], (short)P [2]);
				break;
			case CIMG.CIMG_ADDIMAGE:
				AddImage (P [0], LastID_, (short)P [1], (short)P [2], (short)P [3], (short)P [4], (short)P [5], (short)P [6]);
				break;
			case CIMG.CIMG_LOADRES:
				LoadRes (P [0], str);
				break;
			case CIMG.CIMG_LOADPRIVATERES:
				LoadPrivateRes (P [0], str);
				break;
			}
		}

		//TODO  void SaveText (HANDLE h, C_Parser p) {throw new NotImplementedException();}
#endif // PARSER
		public static C_Image gImageMgr;

	}
}

