using System;
using System.Collections.Generic;
using System.IO;
using FalconNet.Common;

namespace FalconNet.Ui95
{
	public enum UserDataType
	{
		// Userdata stuff
		CSB_IS_VALUE=1,
		CSB_IS_PTR,
		CSB_IS_CLEANUP_PTR,
	}

	public struct USERDATA
	{
		public UserDataType type;
		public long data_number;
		public object data_ptr;
	}

	public class C_Base
	{
		protected const int _GROUP_ = 0;
		protected const int _CLUSTER_ = 1;
		protected const int NUM_SECTIONS = 2;
		
		// Save from here
		protected long	ID_;
		protected long[]	Section_ = new long[NUM_SECTIONS];
		protected UI95_BITTABLE	Flags_;
		protected short	_CType_;
		protected short	Type_;
		protected long	x_, y_, w_, h_;
		protected short	Client_;

		// Don't save
		protected short	Ready_;
		protected Dictionary<long, USERDATA> User_;
		public C_Window Parent_;

		public C_Base ()
		{
			short i;

			ID_ = 0;
			_CType_ = 0;
			Type_ = 0;

			for (i=0; i<NUM_SECTIONS; i++)
				Section_ [i] = 0;

			Client_ = 0;

			x_ = 0;
			y_ = 0;
			w_ = 0;
			h_ = 0;

			Flags_ = 0;
			Parent_ = null;
			User_ = null;
			Ready_ = 0;		// OW
		}

		public C_Base (string stream)
		{
		#if TODO
			short count,i;
			long  idx,value;
		
			memcpy(&ID_,*stream,sizeof(long));					*stream += sizeof(long);
			memcpy(Section_,*stream,sizeof(long)*NUM_SECTIONS);	*stream += sizeof(long)*NUM_SECTIONS;
			memcpy(&Flags_,*stream,sizeof(long));				*stream += sizeof(long);
			memcpy(&_CType_,*stream,sizeof(short));				*stream += sizeof(short);
			memcpy(&Type_,*stream,sizeof(short));				*stream += sizeof(short);
			memcpy(&x_,*stream,sizeof(long));					*stream += sizeof(long);
			memcpy(&y_,*stream,sizeof(long));					*stream += sizeof(long);
			memcpy(&w_,*stream,sizeof(long));					*stream += sizeof(long);
			memcpy(&h_,*stream,sizeof(long));					*stream += sizeof(long);
			memcpy(&Client_,*stream,sizeof(short));				*stream += sizeof(short);
			memcpy(&Ready_,*stream,sizeof(short));				*stream += sizeof(short);
		
			User_=null;
			// User Data...
			memcpy(&count,*stream,sizeof(short));				*stream += sizeof(short);
			for(i=0;i<count;i++)
			{
				memcpy(&idx,*stream,sizeof(long));				*stream += sizeof(long);
				memcpy(&value,*stream,sizeof(long));			*stream += sizeof(long);
				SetUserNumber(idx,value);
			}
		#endif
					throw new NotImplementedException();
		}

		public C_Base (FileStream fp)
		{
		#if TODO
			short count,i;
			long  idx,value;
		
			fread(&ID_,sizeof(long),1,fp);
			fread(Section_,sizeof(long)*NUM_SECTIONS,1,fp);
			fread(&Flags_,sizeof(long),1,fp);
			fread(&_CType_,sizeof(short),1,fp);
			fread(&Type_,sizeof(short),1,fp);
			fread(&x_,sizeof(long),1,fp);
			fread(&y_,sizeof(long),1,fp);
			fread(&w_,sizeof(long),1,fp);
			fread(&h_,sizeof(long),1,fp);
			fread(&Client_,sizeof(short),1,fp);
			fread(&Ready_,sizeof(short),1,fp);
		
			User_=null;
			// User Data...
			fread(&count,sizeof(short),1,fp);
			for(i=0;i<count;i++)
			{
				fread(&idx,sizeof(long),1,fp);
				fread(&value,sizeof(long),1,fp);
				SetUserNumber(idx,value);
			}
		#endif
					throw new NotImplementedException();
		}


#if TODO
		public virtual ~C_Base()
		{
			if(User_)
			{
				User_->Cleanup();
				delete User_;
			}
		}
#endif

		public virtual long Size ()
		{
#if TODO
			long size,curidx;
			C_HASHNODE *cur;
			USERDATA *rec;
		
			size= sizeof(long)
				+ sizeof(long)*NUM_SECTIONS
				+ sizeof(long)
				+ sizeof(short)
				+ sizeof(short)
				+ sizeof(long)
				+ sizeof(long)
				+ sizeof(long)
				+ sizeof(long)
				+ sizeof(short)
				+ sizeof(short)
			// User Data...
				+ sizeof(short); // # UserData elements
		
			if(User_)
			{
				rec=(USERDATA*)User_->GetFirst(&cur,&curidx);
				while(rec)
				{
					if(rec->type == CSB_IS_VALUE)
						size += sizeof(long)*2;
					rec=(USERDATA*)User_->GetNext(&cur,&curidx);
				}
			}
			return(size);
		#endif
					throw new NotImplementedException();
		}

		public virtual void Save (string stream)
		{
#if TODO
			short count;
			long curidx;
			C_HASHNODE *cur;
			USERDATA *rec;
		
			memcpy(*stream,&ID_,sizeof(long));					*stream += sizeof(long);
			memcpy(*stream,Section_,sizeof(long)*NUM_SECTIONS);	*stream += sizeof(long)*NUM_SECTIONS;
			memcpy(*stream,&Flags_,sizeof(long));				*stream += sizeof(long);
			memcpy(*stream,&_CType_,sizeof(short));				*stream += sizeof(short);
			memcpy(*stream,&Type_,sizeof(short));				*stream += sizeof(short);
			memcpy(*stream,&x_,sizeof(long));					*stream += sizeof(long);
			memcpy(*stream,&y_,sizeof(long));					*stream += sizeof(long);
			memcpy(*stream,&w_,sizeof(long));					*stream += sizeof(long);
			memcpy(*stream,&h_,sizeof(long));					*stream += sizeof(long);
			memcpy(*stream,&Client_,sizeof(short));				*stream += sizeof(short);
			memcpy(*stream,&Ready_,sizeof(short));				*stream += sizeof(short);
		
			// User Data...
			count=0;
			if(User_)
			{
				rec=(USERDATA*)User_->GetFirst(&cur,&curidx);
				while(rec)
				{
					if(rec->type == CSB_IS_VALUE)
						count++;
					rec=(USERDATA*)User_->GetNext(&cur,&curidx);
				}
				if(count)
				{
					memcpy(*stream,&count,sizeof(short));					*stream += sizeof(short);
					rec=(USERDATA*)User_->GetFirst(&cur,&curidx);
					while(rec)
					{
						if(rec->type == CSB_IS_VALUE)
						{
							memcpy(*stream,&curidx,sizeof(long));			*stream += sizeof(long);
							memcpy(*stream,&rec->data.number,sizeof(long));	*stream += sizeof(long);
						}
						rec=(USERDATA*)User_->GetNext(&cur,&curidx);
					}
				}
			}
			else
				memcpy(*stream,&count,sizeof(short));						*stream += sizeof(short);
				#endif
					throw new NotImplementedException();
		}


		public virtual void Save (FileStream fp)
		{
#if TODO
			short count;
			long curidx;
			C_HASHNODE *cur;
			USERDATA *rec;
		
			fwrite(&ID_,sizeof(long),1,fp);
			fwrite(Section_,sizeof(long)*NUM_SECTIONS,1,fp);
			fwrite(&Flags_,sizeof(long),1,fp);
			fwrite(&_CType_,sizeof(short),1,fp);
			fwrite(&Type_,sizeof(short),1,fp);
			fwrite(&x_,sizeof(long),1,fp);
			fwrite(&y_,sizeof(long),1,fp);
			fwrite(&w_,sizeof(long),1,fp);
			fwrite(&h_,sizeof(long),1,fp);
			fwrite(&Client_,sizeof(short),1,fp);
			fwrite(&Ready_,sizeof(short),1,fp);
		
			// User Data...
			count=0;
			if(User_)
			{
				rec=(USERDATA*)User_->GetFirst(&cur,&curidx);
				while(rec)
				{
					if(rec->type == CSB_IS_VALUE)
						count++;
					rec=(USERDATA*)User_->GetNext(&cur,&curidx);
				}
				if(count)
				{
					fwrite(&count,sizeof(short),1,fp);
					rec=(USERDATA*)User_->GetFirst(&cur,&curidx);
					while(rec)
					{
						if(rec->type == CSB_IS_VALUE)
						{
							fwrite(&curidx,sizeof(long),1,fp);
							fwrite(&rec->data.number,sizeof(long),1,fp);
						}
						rec=(USERDATA*)User_->GetNext(&cur,&curidx);
					}
				}
			}
			else
				fwrite(&count,sizeof(short),1,fp);
				#endif
					throw new NotImplementedException();
		}

		// Assignment Functions
		public void SetID (long id)
		{
			ID_ = id;
		}

		public void SetType (short type)
		{
			Type_ = type;
		}

		public void _SetCType_ (short ctype)
		{
			_CType_ = ctype;
		}

		public void SetGroup (long id)
		{
			Section_ [_GROUP_] = id;
		}

		public void SetCluster (long id)
		{
			Section_ [_CLUSTER_] = id;
		}

		public void SetClient (short client)
		{
			Client_ = client;
		}

		public void SetControlFlags (UI95_BITTABLE flags)
		{
			Flags_ = flags;
		}

		public virtual void SetFlags (UI95_BITTABLE flags)
		{
			Flags_ = flags;
		}

		public virtual void SetFlagBitOn (UI95_BITTABLE bits)
		{
			Flags_ |= bits;
		}

		public virtual void SetFlagBitOff (UI95_BITTABLE bits)
		{
			Flags_ &= ~bits;
		}

		public virtual void SetX (long x)
		{
			x_ = x;
		}

		public virtual void SetY (long y)
		{
			y_ = y;
		}

		public virtual void SetW (long w)
		{
			w_ = w;
		}

		public virtual void SetH (long h)
		{
			h_ = h;
		}

		public virtual void SetXY (long x, long y)
		{
			x_ = x;
			y_ = y;
		}

		public virtual void SetWH (long w, long h)
		{
			w_ = w;
			h_ = h;
		}

		public virtual void SetXYWH (long x, long y, long w, long h)
		{
			x_ = x;
			y_ = y;
			w_ = w;
			h_ = h;
		}

		public virtual void SetRelX (long x)
		{
		}

		public virtual void SetRelY (long y)
		{
		}

		public virtual void SetRelXY (long x, long y)
		{
		}

		public void EnableGroup (long ID)
		{
			SetFlags (Flags_ & ~UI95_BITTABLE.C_BIT_INVISIBLE);
		}

		public void DisableGroup (long ID)
		{
			SetFlags (Flags_ | UI95_BITTABLE.C_BIT_INVISIBLE);
		}

		public void SetParent (C_Window win)
		{
			Parent_ = win;
		}

		public void SetReady (short r)
		{
			Ready_ = r;
		}

		public void SetUserNumber (long idx, long val)
		{
			USERDATA usr;
		
			if(User_ == null)
			{
				User_= new Dictionary<long, USERDATA>();
#if TODO
				User_.Setup(1);
				User_.SetFlags(UI95_BITTABLE.C_BIT_REMOVE);
				User_.SetCallback(DelUserDataCB);
#endif
			}
			if(User_ != null)
			{
				bool found=User_.TryGetValue(idx, out usr);
				if(found)
				{
					if(usr.type == UserDataType.CSB_IS_CLEANUP_PTR)
						usr.data_ptr = null;
				}
				else
				{
					#if USE_SH_POOLS
					usr = (USERDATA *)MemAllocPtr(UI_Pools[UI_GENERAL_POOL],sizeof(USERDATA),FALSE);
					#else
					usr=new USERDATA();
					#endif
					User_.Add(idx,usr);
				}
				usr.type=UserDataType.CSB_IS_VALUE;
				usr.data_number=val;
			}
		}

		public void SetUserPtr (long idx, object val)
		{
			USERDATA usr;
		
			if(User_ == null)
			{
				User_= new Dictionary<long, USERDATA>();
#if TODO
				User_.Setup(1);
				User_.SetFlags(UI95_BITTABLE.C_BIT_REMOVE);
				User_.SetCallback(DelUserDataCB);
#endif
			}
			if(User_ != null)
			{
				bool found=User_.TryGetValue(idx, out usr);
				if(found)
				{
					if(usr.type == UserDataType.CSB_IS_CLEANUP_PTR)
						usr.data_ptr = null;
				}
				else
				{
					#if USE_SH_POOLS
					usr = (USERDATA *)MemAllocPtr(UI_Pools[UI_GENERAL_POOL],sizeof(USERDATA),FALSE);
					#else
					usr=new USERDATA();
					#endif
					User_.Add(idx,usr);
				}
				usr.type=UserDataType.CSB_IS_PTR;
				usr.data_ptr=val;
			}
		}


		public void SetUserCleanupPtr (long idx, object val)
		{
			USERDATA usr;
		
			if(User_ == null)
			{
				User_= new Dictionary<long, USERDATA>();
#if TODO
				User_.Setup(1);
				User_.SetFlags(UI95_BITTABLE.C_BIT_REMOVE);
				User_.SetCallback(DelUserDataCB);
#endif
			}
			if(User_ != null)
			{
				bool found=User_.TryGetValue(idx, out usr);
				if(found)
				{
					if(usr.type == UserDataType.CSB_IS_CLEANUP_PTR)
						usr.data_ptr = null;
				}
				else
				{
					#if USE_SH_POOLS
					usr = (USERDATA *)MemAllocPtr(UI_Pools[UI_GENERAL_POOL],sizeof(USERDATA),FALSE);
					#else
					usr=new USERDATA();
					#endif
					User_.Add(idx,usr);
				}
				usr.type=UserDataType.CSB_IS_CLEANUP_PTR;
				usr.data_ptr=val;
			}
		}

		public virtual void SetState (short state)
		{
		}

		public virtual void SetHotKey (short key)
		{
		}

		public virtual void SetMenu (long ID)
		{
		}

		public virtual void SetFont (long ID)
		{
		}

		public virtual void SetSound (long ID, short type)
		{
		}

		public virtual void SetCursorID (long id)
		{
		}

		public virtual void SetDragCursorID (long id)
		{
		}

		public virtual void SetHelpText (long id)
		{
		}

		public virtual void SetMouseOver (short state)
		{
		}

		public virtual void SetMouseOverColor (COLORREF color)
		{
		}

		public virtual void SetMouseOverPerc (short perc)
		{
		}
		public delegate void Callback (long p1,short p2,C_Base b);

		public virtual void SetCallback (Callback cb)
		{
		}

// Querry Functions
		public long  GetID ()
		{
			return(ID_);
		}

		public virtual short GetCType ()
		{
			return(Type_);
		}

		public short _GetCType_ ()
		{
			return(_CType_);
		}

		public long  GetGroup ()
		{
			return(Section_ [_GROUP_]);
		}

		public long  GetCluster ()
		{
			return(Section_ [_CLUSTER_]);
		}

		public UI95_BITTABLE  GetFlags ()
		{
			return(Flags_);
		}

		public short GetClient ()
		{
			return(Client_);
		}

		public long GetX ()
		{
			return(x_);
		}

		public long GetY ()
		{
			return(y_);
		}

		public long GetW ()
		{
			return(w_);
		}

		public long GetH ()
		{
			return(h_);
		}

		public long  GetUserNumber (long idx)
		{
			USERDATA usr;
			if(User_ != null)
			{
				bool found=User_.TryGetValue(idx, out usr);
				if(found && usr.type == UserDataType.CSB_IS_VALUE)
					return(usr.data_number);
			}
			return(0);
		}

		public object GetUserPtr (long idx)
		{
			USERDATA usr;
			if(User_ != null)
			{
				bool found=User_.TryGetValue(idx, out usr);
				if(found && usr.type == UserDataType.CSB_IS_VALUE)
					return(usr.data_ptr);
			}
			return(null);
		}

		public virtual long GetRelX ()
		{
			return(0);
		}

		public virtual long GetRelY ()
		{
			return(0);
		}

		public C_Window GetParent ()
		{
			return(Parent_);
		}

		public short  Ready ()
		{
			return(Ready_);
		}

		public virtual short GetState ()
		{
			return(0);
		}

		public virtual short GetHotKey ()
		{
			return(0);
		}

		public virtual long  GetMenu ()
		{
			return(0);
		}

		public virtual long  GetFont ()
		{
			return(0);
		}

		public virtual long  GetHelpText ()
		{
			return(0);
		}

		public virtual SOUND_RES GetSound (short Type)
		{
			return(null);
		}

		public virtual short GetMouseOver ()
		{
			return(0);
		}

		public virtual long  GetCursorID ()
		{
			return(0);
		}

		public virtual long  GetDragCursorID ()
		{
			return(0);
		}
		// TODO public virtual void (*GetCallback())(long,short,C_Base*) { return(null); }

		// Other Functions
		public virtual bool IsBase ()
		{
			return true;
		}

		public virtual bool IsControl ()
		{
			return false;
		}

		public virtual void Refresh ()
		{
		}

		public virtual void Draw (SCREEN surface, UI95_RECT cliprect)
		{
		}

		public virtual void HighLite (SCREEN surface, UI95_RECT cliprect)
		{
		}

		public virtual void SetSubParents (C_Window Parent)
		{
		}

		public virtual void Cleanup ()
		{
		}

		public virtual bool TimerUpdate ()
		{
			return(false);
		}

		public virtual void Activate ()
		{
		}

		public virtual void Deactivate ()
		{
		}

		public virtual long CheckHotSpots (long relx, long rely)
		{
			return(0);
		}

		public virtual bool CheckKeyboard (byte DKScanCode, byte Ascii, byte ShiftStates, long RepeatCount)
		{
			return(false);
		}

		public virtual bool Process (long ID, short HitType)
		{
			return(false);
		}

		public virtual bool CloseWindow ()
		{
			return(false);
		}

		public virtual bool MouseOver (long relX, long relY, C_Base me)
		{
			return(false);
		}

		public virtual C_Base GetMe ()
		{
			return(this);
		}

		public virtual bool Dragable (long ID)
		{
			return(false);
		}

		public virtual void GetItemXY (long ID, ref long x, ref long y)
		{
		}

		public virtual bool Drag (GRABBER Drag, WORD MouseX, WORD MouseY, C_Window  over)
		{
			return(false);
		}

		public virtual bool Drop (GRABBER Drag, WORD MouseX, WORD MouseY, C_Window over)
		{
			return(false);
		}

// Parser Stuff
#if _UI95_PARSER_
		public virtual short LocalFind(char *str) { return(-1); } // Search local token list
		public virtual void LocalFunction(short ID,long P[],_TCHAR *str,C_Handler *Hndlr) {};
		public virtual void SaveText(HANDLE ofp,C_Parser *Parser) {};

		public short BaseFind(char *token);
		public void BaseFunction(short ID,long P[],_TCHAR *,C_Handler *);
		public void SaveTextCommon(HANDLE ofp,C_Parser *Parser,long DefFlags);
#endif // parser
	}
}

