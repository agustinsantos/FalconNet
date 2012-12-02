using System;
using WORD=System.UInt16;
using System.IO;
using System.Diagnostics;

namespace FalconNet.Ui95
{
	public class SOUND_RSC
	{
	}

	public class FLAT_RSC
	{
	}
	
	public enum RSC
	{
		_RSC_8_BIT_			=0x00000001,
		_RSC_16_BIT_		=0x00000002,
		_RSC_USECOLORKEY_	=0x40000000,
		_RSC_SINGLE_		=0x00000001,
		_RSC_MULTIPLE_		=0x00000002,
	
		// Add types as needed
		_RSC_IS_IMAGE_		=100,
		_RSC_IS_SOUND_		=101,
		_RSC_IS_FLAT_		=102,
	};
	
	public class C_Resmgr
	{
	#if USE_SH_POOLS
		public:
			// Overload new/delete to use a SmartHeap pool
			void *operator new(size_t size) { return MemAllocPtr(UI_Pools[UI_CONTROL_POOL],size,FALSE);	};
			void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
	#endif
		
		private long ID_;
		private RSC Type_;
		private long ResIndexVersion_;
		private long ResDataVersion_;
		private short reds, greens, blues; // shift values to convert to SCREEN format
	
		private C_Hash	IDTable_;
		private C_Hash	Index_;
		private byte[]	Idx_;
		private byte[]	Data_;
		private WORD	ColorKey_;
		private string	name_;

		private FileStream OpenResFile (string name, string sfx, FileMode mode, FileAccess access)
		{
			string filename;
			FileStream fp;
			
			filename = C_Parser.FalconUIArtThrDirectory + Path.DirectorySeparatorChar + name + "." + sfx;
			if ((fp = File.Open (filename, mode, access)) != null)
				return fp;
			
			filename = C_Parser.FalconUIArtDirectory + Path.DirectorySeparatorChar + name + "." + sfx;
			return File.Open (filename, mode, access);
		}
	
		public C_Resmgr ()
		{
			ID_ = 0;
			Type_ = 0;
			
			IDTable_ = null;
			Index_ = null;
			Idx_ = null;
			Data_ = null;
			
			name_ = "";
			
			reds = greens = blues = 0;	// OW
		}
		//TODO ~C_Resmgr();
	
		public long GetID ()
		{
			return(ID_);
		}

		public RSC GetType_ ()
		{
			return(Type_);
		}
	
		// User callable functions
		public void Setup (long ID, string filename, C_Hash IDList)
		{
			ID_ = ID;
			Type_ = RSC._RSC_MULTIPLE_;
			name_ = filename;
			IDTable_ = IDList;
			
			if (IDTable_ != null)
				LoadIndex ();
		}

		public void Setup (long ID)
		{
			ID_ = ID;
			Type_ = RSC._RSC_SINGLE_;
		}

		public void Cleanup ()
		{
			if (IDTable_ != null)
				IDTable_ = null;
			
			if (Index_ != null) {
				Index_.Cleanup ();
				Index_ = null;
			}
			if (Idx_ != null) {
			#if USE_SH_POOLS
					MemFreePtr(Idx_);
			#else
				//TODO delete Idx_;
			#endif
				Idx_ = null;
			}
			if (Data_ != null) {
			#if USE_SH_POOLS
					MemFreePtr(Data_);
			#else
				//TODO delete Data_;
			#endif
				Data_ = null;
			}
		}
					
		public long Status ()
		{
			if (Data_ == null) {
				if (Index_ != null)
					return(0x01);
				return(0);
			}
			return(0x03);
		}
	
		public void SetColorKey (WORD Key)
		{
			ColorKey_ = Key;
		}
		// Convert Data_ to Screen format
		public void ConvertToScreen ()
		{
			throw new NotImplementedException ();
		}

		public void SetScreenFormat (short rs, short gs, short bs)
		{
			reds = rs;
			greens = gs;
			blues = bs;
		}

		public void LoadIndex ()
		{
			string buffer = "";
			FileStream fp;
			long recID = 0;
			int size = 0;
			RSC rectype;
			byte ptr = 0;
			IMAGE_RSC irec = null;
			SOUND_RSC srec = null;
			FLAT_RSC frec = null;
			
			buffer = name_;
			buffer += ".idx";
			fp = OpenResFile (name_, "idx", FileMode.Open, FileAccess.Read);
			using (BinaryReader reader = new BinaryReader(fp)) {
				if (fp != null) {
					Debug.WriteLine ("Error opening index file " + buffer);
					return;
				}
			
				size = (int)reader.ReadInt64 ();
				if (size == 0) {
					fp.Close ();
					return;
				}
			
				ResIndexVersion_ = reader.ReadInt64 ();
			
				if (Index_ != null) {
					Index_.Cleanup ();
					//TODOO delete Index_;
				}
			
				Index_ = new C_Hash ();
				Index_.Setup (_IDX_HASH_SIZE_);
				Index_.SetFlags (UI95_BITTABLE.C_BIT_REMOVE);
				Index_.SetCallback (null);
			
			#if USE_SH_POOLS
				Idx_=(char*)MemAllocPtr(UI_Pools[UI_ART_POOL],sizeof(char)*(size),FALSE);
			#else
				Idx_ = new byte[size];
			#endif
				if (Idx_ == null) {
					fp.Close ();
					Index_.Cleanup ();
					//TODO delete Index_;
					Index_ = null;
					return;
				}
				reader.Read (Idx_, 0, size);
				fp.Close ();
#if TODO
				ptr=Idx_;
				while(ptr != 0 && size != 0)
				{
					rectype=(RSC)ptr;
					switch(rectype)
					{
						case RSC._RSC_IS_IMAGE_:
							irec=new IMAGE_RSC();
							irec.Header=(ImageHeader)ptr;
							irec.Owner=this;
			
							recID=IDTable_.FindTextID(irec.Header.ID);
							if(recID >= 0)
							{
								irec.ID=recID;
								if(Index_.Find(recID))
								{
									MonoPrint("ERROR: %s already in Index\n",irec.Header.ID);
									//TODO delete irec;
								}
								else
									Index_.Add(recID,irec);
							}
							else
							{
								gMainParser.AddNewID(irec.Header.ID,100);
								recID=IDTable_.FindTextID(irec.Header.ID);
								if(recID)
								{
									irec.ID=recID;
									if(Index_.Find(recID))
									{
										MonoPrint("ERROR: %s already in Index\n",irec.Header.ID);
										//TODO delete irec;
									}
									else
										Index_.Add(recID,irec);
								}
							}
							size-=sizeof(ImageHeader);
							ptr+=sizeof(ImageHeader);
							break;
						case RSC._RSC_IS_SOUND_:
							srec=new SOUND_RSC();
							srec.Header=(SoundHeader)ptr;
							srec.Owner=this;
			
							recID=IDTable_.FindTextID(srec.Header.ID);
							if(recID >= 0)
							{
								srec.ID=recID;
								if(Index_.Find(recID))
								{
									MonoPrint("ERROR: %s already in Index\n",frec.Header.ID);
									//TODO delete srec;
								}
								else
									Index_.Add(recID,srec);
							}
							else
							{
								gMainParser.AddNewID(srec.Header.ID,50);
								recID=IDTable_.FindTextID(srec.Header.ID);
								if(recID)
								{
									srec.ID=recID;
									if(Index_.Find(recID))
									{
										MonoPrint("ERROR: %s already in Index\n",srec.Header.ID);
										//TODO delete srec;
									}
									else
										Index_.Add(recID,srec);
								}
							}
							size-=sizeof(SoundHeader);
							ptr+=sizeof(SoundHeader);
							break;
						case RSC._RSC_IS_FLAT_:
							frec=new FLAT_RSC();
							frec.Header=(FlatHeader*)ptr;
							frec.Owner=this;
			
							recID=IDTable_.FindTextID(frec.Header.ID);
							if(recID >= 0)
							{
								frec.ID=recID;
								if(Index_.Find(recID))
								{
									MonoPrint("ERROR: %s already in Index\n",frec.Header.ID);
									//TODO delete frec;
								}
								else
									Index_.Add(recID,frec);
							}
							else
							{
								gMainParser.AddNewID(frec.Header.ID,50);
								recID=IDTable_.FindTextID(frec.Header.ID);
								if(recID)
								{
									frec.ID=recID;
									if(Index_.Find(recID))
									{
										MonoPrint("ERROR: %s already in Index\n",frec.Header.ID);
										//TODO delete frec;
									}
									else
										Index_.Add(recID,frec);
								}
							}
							size-=sizeof(FlatHeader);
							ptr+=sizeof(FlatHeader);
							break;
						default:
							ptr=null;
							size=0;
							break;
					}
				}
#endif
			}
		}
			
		public void AddIndex (long ID, IMAGE_RSC resheader)
		{
			if (resheader == null || Type_ == RSC._RSC_MULTIPLE_)
				return;
			
			if (Index_ == null) {
				Index_ = new C_Hash ();
				Index_.Setup (1);
				Index_.SetFlags (UI95_BITTABLE.C_BIT_REMOVE);
				Index_.SetCallback (ImageCleanupCB);
			}
			Index_.Add (ID, resheader);
		}
		
		public void AddIndex (long p, SOUND_RSC s)
		{
		}

		public void SetData (byte[] data)
		{
			if (Data_ != null)
				Data_ = null;
			Data_ = data;
		}

		public byte[] GetData ()
		{
			return(Data_);
		}

		public void LoadData ()
		{
			long size;
			FileStream fp;
			string buffer;

			if (Index_ == null)
				return;

			if (Data_ != null)
				Data_ = null;

			buffer = name_ + ".rsc";

			fp = OpenResFile (name_, "rsc", FileMode.Open, FileAccess.Read);
			if (fp == null) {
				Debug.WriteLine ("Error: Can't open Datafile :" + buffer);
				return;
			}
						
			using (BinaryReader reader = new BinaryReader(fp)) {
				size = reader.ReadInt64 ();
				if (size == 0) {
					fp.Close ();
					return;
				}
			
				ResDataVersion_ = reader.ReadInt64 ();
				Debug.Assert (ResIndexVersion_ == ResDataVersion_);

#if USE_SH_POOLS
	Data_=(char*)MemAllocPtr(UI_Pools[UI_ART_POOL],sizeof(char)*(size),FALSE);
#else
				Data_ = new byte[size];
#endif
				if (Data_ != null)
					reader.Read (Data_, 0, (int)size);
				else
					Debug.WriteLine ("Error allocating (" + size + ") bytes for Datafile");

			}
			
			fp.Close ();
#if TODO
			if (Data_ != null)
				ConvertToScreen ();
#endif
		}

		public void UnloadData ()
		{
			throw new NotImplementedException ();
		}

		public object Find (long ID)
		{
			if (Index_ != null)
				return(Index_.Find (ID));
			return(null);
		}
	
		public  string ResName ()
		{
			return(name_);
		}
	
		C_Hash GetIDList ()
		{
			return(Index_);
		}

		private const int _IDX_HASH_SIZE_ = 10;

		void ImageCleanupCB (object rec)
		{
			IMAGE_RSC data = (IMAGE_RSC)rec;
		
			if (data != null) {
				//data.Header = null;
				data = null;
			}
		}
	}
}

