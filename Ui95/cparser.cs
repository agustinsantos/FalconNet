using System;
using FalconNet.Common;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System.Diagnostics;

namespace FalconNet.Ui95
{
	public struct ID_TABLE
	{
		public string Label;
		public long Value;
	}

	public class C_Parser
	{
		public const int PARSE_MAX_PARAMS = 12;
		public const int  MAX_WINDOWS_IN_LIST = 200;
		public const int  PARSE_HASH_SIZE = 1024;
#if USE_SH_POOLS
	public:
		// Overload new/delete to use a SmartHeap pool
		void *operator new(size_t size) { return MemAllocPtr(UI_Pools[UI_CONTROL_POOL],size,false);	};
		void operator delete(void *mem) { if (mem) MemFreePtr(mem); };
#endif
	
		private int Idx_; // index into script
		private string script_; // script file (read into memory)
		private long scriptlen_;
		private long[] P_ = new long[PARSE_MAX_PARAMS]; // used for passing parameters to Setup routines for the new windows/controls
		private string str_; // string pointer (only 1 allowed per command)

		private C_Handler	Handler_;	// pointer to Window Handler (from Setup())
		private C_Window	Window_;	// pointer to current window (from script)
		private C_Base		Control_;	// pointer to current control (from script)
		private C_Font		Font_;		// Pointer to current Font (from script)
		private C_Image		Image_;	// Pointer to current Image Mgr (from script)
		private C_Animation	Anim_;		// Pointer to current Animation Mgr (from Script)
		private C_Sound		Sound_;	// Pointer to current Sound Mgr (from script)
		private C_PopupMgr	Popup_;	// Pointer to Popup Menu Manager
		private C_String	String_;	// Pointer to String Manager (from script)
		private C_Movie		Movie_;	// Pointer to Movie Manager (from script)

		private long[]       WindowList_ = new long[MAX_WINDOWS_IN_LIST];
		private C_Hash	IDOrder_;		// Hash List in ID order... for finding tokens
		private C_Hash	TokenOrder_;	// Hash List in "Token" order

		private string ValueStr;		// string to contain values of IDs NOT found in table

		private FileStream Perror_;
		// Current Token;
		private short  tokenlen_;

		// Parameters
		private short P_Idx_; // pointer to current parameter

		private short        WinIndex_, WinLoaded_;

		private C_Window WindowParser ()
		{
#if TODO
			bool Done = false, Comment = false, Found = false, InString = false, Finished = false;
			int TokenID;
			TOKEN TokenType;
			SECTION Section;

			Section = SECTION.SECTION_PROCESSTOKEN;
			TokenType = TOKEN.TOKEN_NOTHING;

			while (!Done) {
				switch (Section) {
				case SECTION.SECTION_FINDTOKEN:
			// Look for token starting with '['
					Found = false;
					while (!Found && !Done) {
						switch (script_ [Idx_]) {
						case '[':
							if (!Comment && !InString) {
								Found = true;
								break;
							}
							Idx_++;
							break;
						case '"':
							InString = !InString;
							Idx_++;
							break;
						case '#':
							Comment = true;
							Idx_++;
							break;
						case (char)0x0a:
						case (char)0x0d:
							Comment = false;
							Idx_++;
							break;
						default:
							Idx_++;
							break;
						}
						if (Idx_ >= scriptlen_)
							Done = true;
					}
							
					tokenlen_ = 0;
					while (script_[Idx_+tokenlen_] != ']' && (Idx_+tokenlen_) < scriptlen_)
						tokenlen_++;
					tokenlen_++;

					if ((Idx_ + tokenlen_) >= scriptlen_) {
						Done = true;
						break;
					}

					if (Found)
						Section = SECTION.SECTION_PROCESSTOKEN;
					break;
				case SECTION.SECTION_PROCESSTOKEN:
					TokenID = FindToken (script_, Idx_);
					if (TokenID != 0) {
						switch ((CPARSE)TokenID) {
						case CPARSE.CPARSE_WINDOW:
							TokenType = TOKEN.TOKEN_WINDOW;
							Window_ = new C_Window ();
							Section = SECTION.SECTION_FINDSUBTOKEN;
							Idx_ += tokenlen_;
							tokenlen_ = 0;
							break;
						case CPARSE.CPARSE_BUTTON:
						case CPARSE.CPARSE_TEXT:
						case CPARSE.CPARSE_BOX:
						case CPARSE.CPARSE_LINE:
						case CPARSE.CPARSE_CLOCK:
						case CPARSE.CPARSE_FILL:
						case CPARSE.CPARSE_TREE:
						case CPARSE.CPARSE_EDITBOX:
						case CPARSE.CPARSE_LISTBOX:
						//case CPARSE.CPARSE_ACMI:
						case CPARSE.CPARSE_PANNER:
						case CPARSE.CPARSE_SLIDER:
						case CPARSE.CPARSE_TREELIST:
						case CPARSE.CPARSE_BITMAP:
						case CPARSE.CPARSE_TILE:
						case CPARSE.CPARSE_ANIM:
						case CPARSE.CPARSE_CURSOR:
						case CPARSE.CPARSE_MARQUE:
						case CPARSE.CPARSE_ANIMATION:
							Window_.AddControl (ControlParser ());
							Section = SECTION.SECTION_FINDTOKEN;
							break;
						case CPARSE.CPARSE_SCROLLBAR:
							Window_.AddScrollBar ((C_ScrollBar)ControlParser ());
							Section = SECTION.SECTION_FINDTOKEN;
							break;
						case CPARSE.CPARSE_FONT:
							TokenType = TOKEN.TOKEN_FONT;
							Idx_ += tokenlen_;
							tokenlen_ = 0;
							break;
						case CPARSE.CPARSE_SOUND:
							TokenType = TOKEN.TOKEN_SOUND;
							Idx_ += tokenlen_;
							tokenlen_ = 0;
							break;
						case CPARSE.CPARSE_STRING:
							TokenType = TOKEN.TOKEN_STRING;
							Idx_ += tokenlen_;
							tokenlen_ = 0;
							break;
						case CPARSE.CPARSE_IMAGE:
							TokenType = TOKEN.TOKEN_IMAGE;
							Idx_ += tokenlen_;
							tokenlen_ = 0;
							break;
						default:
							//TODO MonoPrint ("ControlParser: Token NOT FOUND [%s]\n", &script_ [Idx_]);
							Section = SECTION.SECTION_FINDTOKEN;
							Idx_ += tokenlen_;
							tokenlen_ = 0;
							break;
						}
					} else {
						Section = SECTION.SECTION_FINDTOKEN;
						Idx_ += tokenlen_;
						tokenlen_ = 0;
					}
					break;
				case SECTION.SECTION_FINDSUBTOKEN:
			// Look for token starting with '['
					Found = false;
					while (!Found && !Done) {
						switch (script_ [Idx_]) {
						case '[':
							if (!Comment && !InString) {
								Found = true;
								break;
							}
							Idx_++;
							break;
						case '"':
							InString = !InString;
							Idx_++;
							break;
						case '#':
							Comment = true;
							Idx_++;
							break;
						case (char)0x0a:
						case (char)0x0d:
							Comment = false;
							Idx_++;
							break;
						default:
							Idx_++;
							break;
						}
						if (Idx_ >= scriptlen_)
							Done = true;
					}
							
					tokenlen_ = 0;
					while (script_[Idx_+tokenlen_] != ']' && (Idx_+tokenlen_) < scriptlen_)
						tokenlen_++;
					tokenlen_++;

					if ((Idx_ + tokenlen_) >= scriptlen_) {
						Done = true;
						break;
					}

					if (Found == true)
						Section = SECTION.SECTION_PROCESSSUBTOKEN;
					break;
				case SECTION.SECTION_PROCESSSUBTOKEN:
					TokenID = FindToken (script_, Idx_);
					if (TokenID != 0) { // if found... this is a MAIN keyword NOT a Control/Window keyword
						Section = SECTION.SECTION_PROCESSTOKEN;
						switch ((CPARSE)TokenID) {
						case CPARSE.CPARSE_WINDOW:
						case CPARSE.CPARSE_FONT:
							Done = true;
							break;
						default:
							Section = SECTION.SECTION_FINDTOKEN;
							break;
						}
						break;
						Idx_ += tokenlen_;
						tokenlen_ = 0;
					}
					switch (TokenType) {
					case TOKEN.TOKEN_WINDOW:
						TokenID = Window_.LocalFind (script_ .Substring (Idx_));
						if (TokenID != 0) {
							Section = SECTION.SECTION_FINDPARAMS;
							Idx_ += tokenlen_;
							tokenlen_ = 0;
						} else {
							Section = SECTION.SECTION_FINDSUBTOKEN;
							Idx_ += tokenlen_;
							tokenlen_ = 0;
						}
						break;
					}

					break;
				case SECTION.SECTION_FINDPARAMS:
					P_Idx_ = 0; // start with 0 parameters

				// Repeat until token char '[' found (or EOF)
					Finished = false;
					while (!Finished) {
						// Find NON white space
						Found = false;
						while (!Found && !Done && !Finished) {
							switch (script_ [Idx_]) {
							case ' ':
							case ',':
							case (char)0x09:
							case (char)0x0a:
							case (char)0x0d:
								Idx_++;
								break;
							case '[':
								Finished = true;
								Section = SECTION.SECTION_PROCESSPARAMS;
								break;
							default:
								Found = true;
								break;
							}
							if (Idx_ >= scriptlen_) {
								Finished = true;
								Section = SECTION.SECTION_PROCESSPARAMS;
							}
						}

						if (Found) {
							Found = false;
							if (script_ [Idx_] == '"') { // string
								tokenlen_ = 1;
								str_ = script_.Substring (Idx_ + tokenlen_);
								// Find closing (")
								while (!Found && !Finished) {
									if (script_ [Idx_ + tokenlen_] == '"')
										Found = true;
									else {
										if ((Idx_ + tokenlen_) >= scriptlen_) {
											Finished = true;
											Section = SECTION.SECTION_PROCESSPARAMS;
										} else
											tokenlen_++;
									}
								}
								if (Found)
									script_ [Idx_ + tokenlen_] = 0; // make null terminated string
								Idx_ += tokenlen_ + 1;
								tokenlen_ = 0;
							} else if (isdigit (script_ [Idx_]) || script_ [Idx_] == '-') { // Number
								// find white space
								Found = false;
								tokenlen_ = 1;
								while (!Found) {
									switch (script_ [Idx_ + tokenlen_]) {
									case ' ':
									case ',':
									case (char)0x09:
									case (char)0x0a:
									case (char)0x0d:
										Found = true;
										break;
									default:
										tokenlen_++;
										break;
									}
									if ((Idx_ + tokenlen_) >= scriptlen_) {
										Found = true;
										Finished = true;
										Section = SECTION.SECTION_PROCESSPARAMS;
									}
								}
								if (Found && P_Idx_ < PARSE_MAX_PARAMS)
									P_ [P_Idx_++] = atol (&script_ [Idx_]);

								Idx_ += tokenlen_;
								tokenlen_ = 0;
							} else {
								// Look for ID in tables
								// Look for white space
								Found = false;
								tokenlen_ = 0;
								while (!Found && !Finished) {
									switch (script_ [Idx_ + tokenlen_]) {
									case ' ':
									case ',':
									case (char)0x09:
									case (char)0x0a:
									case (char)0x0d:
										Found = true;
										break;
									default:
										tokenlen_++;
										break;
									}
									if ((Idx_ + tokenlen_) >= scriptlen_) {
										Finished = true;
										Found = true;
										Section = SECTION.SECTION_PROCESSPARAMS;
									}
								}
								if (Found && P_Idx_ < PARSE_MAX_PARAMS) {
									script_ [Idx_ + tokenlen_] = 0;
									P_ [P_Idx_++] = FindID (script_ [Idx_]);
									if (P_ [P_Idx_ - 1] < 0 && script_ [Idx_] != "NID")
										TokenErrorList.AddText (script_ [Idx_]);
								}
								Idx_ += tokenlen_ + 1;
								tokenlen_ = 0;
							}
						}
					}
					break;
				case SECTION.SECTION_PROCESSPARAMS:
					switch (TokenType) {
					case TOKEN.TOKEN_WINDOW:
						Window_.LocalFunction (static_cast<short> (TokenID), P_, str_, Handler_);
						break;
					}
					P_Idx_ = 0;
					P_ [0] = 0;
					P_ [1] = 0;
					P_ [2] = 0;
					P_ [3] = 0;
					P_ [4] = 0;
					P_ [5] = 0;
					P_ [6] = 0;
					P_ [7] = 0;
					str_ = null;
					Section = SECTION.SECTION_FINDSUBTOKEN;
					break;
				}
			}
			return(Window_);
#endif
			throw new NotImplementedException ();
		}

		private C_Base ControlParser ()
		{
#if TODO
			bool Done = false, Comment = false, Found = false, InString = false, Finished = false;
			long TokenID = 0;
			TOKEN TokenType;
			SECTION Section;
			Control_ = null;
			Section = SECTION.SECTION_PROCESSTOKEN;
			TokenType = TOKEN.TOKEN_NOTHING;

			while (!Done) {
				switch (Section) {
				case SECTION.SECTION_FINDTOKEN:
			// Look for token starting with '['
					Found = false;
					while (!Found && !Done) {
						switch (script_ [Idx_]) {
						case '[':
							if (!Comment && !InString) {
								Found = true;
								break;
							}
							Idx_++;
							break;
						case '"':
							InString = !InString;
							Idx_++;
							break;
						case '#':
							Comment = true;
							Idx_++;
							break;
						case (char)0x0a:
						case (char)0x0d:
							Comment = false;
							Idx_++;
							break;
						default:
							Idx_++;
							break;
						}
						if (Idx_ >= scriptlen_)
							Done = true;
					}
							
					tokenlen_ = 0;
					while (script_[Idx_+tokenlen_] != ']' && (Idx_+tokenlen_) < scriptlen_)
						tokenlen_++;
					tokenlen_++;

					if ((Idx_ + tokenlen_) >= scriptlen_) {
						Done = true;
						break;
					}

					if (Found == true)
						Section = SECTION.SECTION_PROCESSTOKEN;
					break;
				case SECTION.SECTION_PROCESSTOKEN:
					TokenID = FindToken (script_, Idx_);
					if (TokenID != 0) {
						Section = SECTION.SECTION_FINDSUBTOKEN;
						switch ((CPARSE)TokenID) {
						case CPARSE.CPARSE_WINDOW:
							Done = true;
							break;
						case CPARSE.CPARSE_BUTTON:
							TokenType = TOKEN.TOKEN_COMMON;
							if (Control_ != null)
								Control_ = null;
							Control_ = new C_Button ();
							break;
						case CPARSE.CPARSE_TEXT:
							TokenType = TOKEN.TOKEN_COMMON;
							if (Control_ != null)
								Control_ = null;
							Control_ = new C_Text ();
							break;
						case CPARSE.CPARSE_EDITBOX:
							TokenType = TOKEN.TOKEN_COMMON;
							if (Control_ != null)
								Control_ = null;
							Control_ = new C_EditBox ();
							break;
						case CPARSE.CPARSE_LISTBOX:
							TokenType = TOKEN.TOKEN_COMMON;
							if (Control_ != null)
								Control_ = null;
							Control_ = new C_ListBox ();
							break;
						case CPARSE.CPARSE_SLIDER:
							TokenType = TOKEN.TOKEN_COMMON;
							if (Control_ != null)
								Control_ = null;
							Control_ = new C_Slider ();
							break;
						//case CPARSE.CPARSE_ACMI:
						//	TokenType=TOKEN.TOKEN_COMMON;
						//	Control_=new C_Acmi;
						//	break;
						case CPARSE.CPARSE_PANNER:
							TokenType = TOKEN.TOKEN_COMMON;
							if (Control_ != null)
								Control_ = null;
							Control_ = new C_Panner ();
							break;
						case CPARSE.CPARSE_SCROLLBAR:
							TokenType = TOKEN.TOKEN_COMMON;
							if (Control_ != null)
								Control_ = null;
							Control_ = new C_ScrollBar ();
							break;
						case CPARSE.CPARSE_TREELIST:
							TokenType = TOKEN.TOKEN_COMMON;
							if (Control_ != null)
								Control_ = null;
							Control_ = new C_TreeList ();
							break;
						case CPARSE.CPARSE_BITMAP:
							TokenType = TOKEN.TOKEN_COMMON;
							if (Control_ != null)
								Control_ = null;
							Control_ = new C_Bitmap ();
							break;
						case CPARSE.CPARSE_TILE:
							TokenType = TOKEN.TOKEN_COMMON;
							if (Control_ != null)
								Control_ = null;
							Control_ = new C_Tile ();
							break;
						case CPARSE.CPARSE_ANIM:
							TokenType = TOKEN.TOKEN_COMMON;
							if (Control_ != null)
								Control_ = null;
							Control_ = new C_Anim ();
							break;
						case CPARSE.CPARSE_CURSOR:
							TokenType = TOKEN.TOKEN_COMMON;
							if (Control_ != null)
								Control_ = null;
							Control_ = new C_Cursor ();
							break;
						case CPARSE.CPARSE_MARQUE:
							TokenType = TOKEN.TOKEN_COMMON;
							if (Control_ != null)
								Control_ = null;
							Control_ = new C_Marque ();
							break;
						case CPARSE.CPARSE_BOX:
							TokenType = TOKEN.TOKEN_COMMON;
							if (Control_ != null)
								Control_ = null;
							Control_ = new C_Box ();
							break;
						case CPARSE.CPARSE_LINE:
							TokenType = TOKEN.TOKEN_COMMON;
							if (Control_ != null)
								Control_ = null;
							Control_ = new C_Line ();
							break;
						case CPARSE.CPARSE_CLOCK:
							TokenType = TOKEN.TOKEN_COMMON;
							if (Control_ != null)
								Control_ = null;
							Control_ = new C_Clock ();
							break;
						case CPARSE.CPARSE_FILL:
							TokenType = TOKEN.TOKEN_COMMON;
							if (Control_ != null)
								Control_ = null;
							Control_ = new C_Fill ();
							break;
						case CPARSE.CPARSE_TREE:
							TokenType = TOKEN.TOKEN_COMMON;
							if (Control_ != null)
								Control_ = null;
							Control_ = new C_TreeList ();
							break;
						case CPARSE.CPARSE_ANIMATION:
						case CPARSE.CPARSE_FONT:
						case CPARSE.CPARSE_SOUND:
						case CPARSE.CPARSE_STRING:
							Done = true;
							break;
						default:
							//TODO MonoPrint ("ControlParser: Token NOT FOUND [%s]\n", &script_ [Idx_]);
							Section = SECTION.SECTION_FINDTOKEN;
							break;
						}
						Idx_ += tokenlen_;
						tokenlen_ = 0;
					} else {
						Section = SECTION.SECTION_FINDTOKEN;
						Idx_ += tokenlen_;
						tokenlen_ = 0;
					}
					break;
				case SECTION.SECTION_FINDSUBTOKEN:
			// Look for token starting with '['
					Found = false;
					while (!Found && !Done) {
						switch (script_ [Idx_]) {
						case '[':
							if (!Comment && !InString) {
								Found = true;
								break;
							}
							Idx_++;
							break;
						case '"':
							InString = !InString;
							Idx_++;
							break;
						case '#':
							Comment = true;
							Idx_++;
							break;
						case (char)0x0a:
						case (char)0x0d:
							Comment = false;
							Idx_++;
							break;
						default:
							Idx_++;
							break;
						}
						if (Idx_ >= scriptlen_)
							Done = true;
					}
							
					tokenlen_ = 0;
					while (script_[Idx_+tokenlen_] != ']' && (Idx_+tokenlen_) < scriptlen_)
						tokenlen_++;
					tokenlen_++;

					if ((Idx_ + tokenlen_) >= scriptlen_) {
						Done = true;
						break;
					}

					if (Found == true)
						Section = SECTION.SECTION_PROCESSSUBTOKEN;
					break;
				case SECTION.SECTION_PROCESSSUBTOKEN:
					TokenID = FindToken (script_, Idx_);
					if (TokenID != 0) { // if found... this is a MAIN keyword NOT a Control/Window keyword
						Done = true;
						break;
					}
					switch (TokenType) {
					case TOKEN.TOKEN_COMMON:
					case TOKEN.TOKEN_LOCAL:
						TokenID = Control_.BaseFind (script_, Idx_);
						if (TokenID != 0) {
							TokenType = TOKEN.TOKEN_COMMON;
							Section = SECTION.SECTION_FINDPARAMS;
							Idx_ += tokenlen_;
							tokenlen_ = 0;
						} else {
							TokenID = Control_.LocalFind (script_, Idx_);
							if (TokenID != 0) {
								Section = SECTION.SECTION_FINDPARAMS;
								TokenType = TOKEN.TOKEN_LOCAL;
								Idx_ += tokenlen_;
								tokenlen_ = 0;
							} else {
								Section = SECTION.SECTION_FINDSUBTOKEN;
								Idx_++;
							}
						}
						break;
					}
					break;
				case SECTION.SECTION_FINDPARAMS:
					P_Idx_ = 0; // start with 0 parameters

				// Repeat until token char '[' found (or EOF)
					Finished = false;
					while (!Finished) {
						// Find NON white space
						Found = false;
						while (!Found && !Done && !Finished) {
							switch (script_ [Idx_]) {
							case ' ':
							case ',':
							case (char)0x09:
							case (char)0x0a:
							case (char)0x0d:
								Idx_++;
								break;
							case '[':
								Finished = true;
								Section = SECTION.SECTION_PROCESSPARAMS;
								break;
							default:
								Found = true;
								break;
							}
							if (Idx_ >= scriptlen_) {
								Finished = true;
								Section = SECTION.SECTION_PROCESSPARAMS;
							}
						}

						if (Found) {
							Found = false;
							if (script_ [Idx_] == '"') { // string
								tokenlen_ = 1;
								str_ = script_.Substring (Idx_ + tokenlen_);
								// Find closing (")
								while (!Found && !Finished) {
									if (script_ [Idx_ + tokenlen_] == '"')
										Found = true;
									else {
										if ((Idx_ + tokenlen_) >= scriptlen_) {
											Finished = true;
											Section = SECTION.SECTION_PROCESSPARAMS;
										} else
											tokenlen_++;
									}
								}
								if (Found)
									script_ [Idx_ + tokenlen_] = 0; // make null terminated string
								Idx_ += tokenlen_ + 1;
								tokenlen_ = 0;
							} else if (isdigit (script_ [Idx_]) || script_ [Idx_] == '-') { // Number
								// find white space
								Found = false;
								tokenlen_ = 1;
								while (!Found) {
									switch (script_ [Idx_ + tokenlen_]) {
									case ' ':
									case ',':
									case (char)0x09:
									case (char)0x0a:
									case (char)0x0d:
										Found = true;
										break;
									default:
										tokenlen_++;
										break;
									}
									if ((Idx_ + tokenlen_) >= scriptlen_) {
										Found = true;
										Finished = true;
										Section = SECTION.SECTION_PROCESSPARAMS;
									}
								}
								if (Found && P_Idx_ < PARSE_MAX_PARAMS)
									P_ [P_Idx_++] = int.Parse (script_ [Idx_]);

								Idx_ += tokenlen_;
								tokenlen_ = 0;
							} else {
								// Look for ID in tables
								// Look for white space
								Found = false;
								tokenlen_ = 0;
								while (!Found && !Finished) {
									switch (script_ [Idx_ + tokenlen_]) {
									case ' ':
									case ',':
									case (char)0x09:
									case (char)0x0a:
									case (char)0x0d:
										Found = true;
										break;
									default:
										tokenlen_++;
										break;
									}
									if ((Idx_ + tokenlen_) >= scriptlen_) {
										Finished = true;
										Found = true;
										Section = SECTION.SECTION_PROCESSPARAMS;
									}
								}
								if (Found && P_Idx_ < PARSE_MAX_PARAMS) {
									script_ [Idx_ + tokenlen_] = 0;
									P_ [P_Idx_++] = FindID (&script_ [Idx_]);
									if (P_ [P_Idx_ - 1] < 0 && script_ [Idx_] != "NID")
										TokenErrorList.AddText (&script_ [Idx_]);
								}
								Idx_ += tokenlen_ + 1;
								tokenlen_ = 0;
							}
						}
					}
					break;
				case SECTION.SECTION_PROCESSPARAMS:
					switch (TokenType) {
					case TOKEN.TOKEN_COMMON:
						Control_.BaseFunction ((short)(TokenID), P_, str_, Handler_);
						break;
					case TOKEN.TOKEN_LOCAL:
						Control_.LocalFunction ((short)(TokenID), P_, str_, Handler_);
						break;
					}
					P_Idx_ = 0;
					P_ [0] = 0;
					P_ [1] = 0;
					P_ [2] = 0;
					P_ [3] = 0;
					P_ [4] = 0;
					P_ [5] = 0;
					P_ [6] = 0;
					P_ [7] = 0;
					str_ = null;
					Section = SECTION.SECTION_FINDSUBTOKEN;
					break;
				}
			}
			return(Control_);
#endif
			throw new NotImplementedException ();
		}

		private C_Base PopupParser ()
		{
#if TODO
			bool InString = false;
			bool Done = false, Comment = false, Found = false, Finished = false;
			
			TOKEN TokenType;
			int TokenID = 0;
			SECTION Section;
			Section = SECTION.SECTION_PROCESSTOKEN;
			TokenType = TOKEN.TOKEN_NOTHING;

			while (!Done) {
				switch (Section) {
				case SECTION.SECTION_FINDTOKEN:
			// Look for token starting with '['
					Found = false;
					while (!Found && !Done) {
						switch (script_ [Idx_]) {
						case '[':
							if (!Comment && !InString) {
								Found = true;
								break;
							}
							Idx_++;
							break;
						case '"':
							InString = !InString;
							Idx_++;
							break;
						case '#':
							Comment = true;
							Idx_++;
							break;
						case (char)0x0a:
						case (char)0x0d:
							Comment = false;
							Idx_++;
							break;
						default:
							Idx_++;
							break;
						}
						if (Idx_ >= scriptlen_)
							Done = true;
					}
							
					tokenlen_ = 0;
					while (script_[Idx_+tokenlen_] != ']' && (Idx_+tokenlen_) < scriptlen_)
						tokenlen_++;
					tokenlen_++;

					if ((Idx_ + tokenlen_) >= scriptlen_) {
						Done = true;
						break;
					}

					if (Found == true)
						Section = SECTION.SECTION_PROCESSTOKEN;
					break;
				case SECTION.SECTION_PROCESSTOKEN:
					TokenID = FindToken (script_, Idx_);
					if (TokenID != 0) {
						Section = SECTION.SECTION_FINDSUBTOKEN;
						switch ((CPARSE)TokenID) {
						case CPARSE.CPARSE_POPUP:
							TokenType = TOKEN.TOKEN_COMMON;
							Control_ = new C_PopupList ();

							break;
						default:
							Section = SECTION.SECTION_FINDTOKEN;
							break;
						}
						Idx_ += tokenlen_;
						tokenlen_ = 0;
					} else {
						Section = SECTION.SECTION_FINDTOKEN;
						Idx_ += tokenlen_;
						tokenlen_ = 0;
					}
					break;
				case SECTION.SECTION_FINDSUBTOKEN:
			// Look for token starting with '['
					Found = false;
					while (!Found && !Done) {
						switch (script_ [Idx_]) {
						case '[':
							if (!Comment && !InString) {
								Found = true;
								break;
							}
							Idx_++;
							break;
						case '"':
							InString = !InString;
							Idx_++;
							break;
						case '#':
							Comment = true;
							Idx_++;
							break;
						case (char)0x0a:
						case (char)0x0d:
							Comment = false;
							Idx_++;
							break;
						default:
							Idx_++;
							break;
						}
						if (Idx_ >= scriptlen_)
							Done = true;
					}
							
					tokenlen_ = 0;
					while (script_[Idx_+tokenlen_] != ']' && (Idx_+tokenlen_) < scriptlen_)
						tokenlen_++;
					tokenlen_++;

					if ((Idx_ + tokenlen_) >= scriptlen_) {
						Done = true;
						break;
					}

					if (Found == true)
						Section = SECTION.SECTION_PROCESSSUBTOKEN;
					break;
				case SECTION.SECTION_PROCESSSUBTOKEN:
					TokenID = FindToken (script_, Idx_);
					if (TokenID != 0) { // if found... this is a MAIN keyword NOT a Control/Window keyword
						Done = true;
						break;
					}
					switch (TokenType) {
					case TOKEN.TOKEN_COMMON:
					case TOKEN.TOKEN_LOCAL:
						TokenID = Control_.BaseFind (script_, Idx_);
						if (TokenID != 0) {
							TokenType = TOKEN.TOKEN_COMMON;
							Section = SECTION.SECTION_FINDPARAMS;
							Idx_ += tokenlen_;
							tokenlen_ = 0;
						} else {
							TokenID = Control_.LocalFind (script_, Idx_);
							if (TokenID != 0) {
								Section = SECTION.SECTION_FINDPARAMS;
								TokenType = TOKEN.TOKEN_LOCAL;
								Idx_ += tokenlen_;
								tokenlen_ = 0;
							} else {
								Section = SECTION.SECTION_FINDSUBTOKEN;
								Idx_++;
							}
						}
						break;
					}
					break;
				case SECTION.SECTION_FINDPARAMS:
					P_Idx_ = 0; // start with 0 parameters

				// Repeat until token char '[' found (or EOF)
					Finished = false;
					while (!Finished) {
						// Find NON white space
						Found = false;
						while (!Found && !Done && !Finished) {
							switch (script_ [Idx_]) {
							case ' ':
							case ',':
							case (char)0x09:
							case (char)0x0a:
							case (char)0x0d:
								Idx_++;
								break;
							case '[':
								Finished = true;
								Section = SECTION.SECTION_PROCESSPARAMS;
								break;
							default:
								Found = true;
								break;
							}
							if (Idx_ >= scriptlen_) {
								Finished = true;
								Section = SECTION.SECTION_PROCESSPARAMS;
							}
						}

						if (Found) {
							Found = false;
							if (script_ [Idx_] == '"') { // string
								tokenlen_ = 1;
								str_ = script_.Substring (Idx_ + tokenlen_);
								// Find closing (")
								while (!Found && !Finished) {
									if (script_ [Idx_ + tokenlen_] == '"')
										Found = true;
									else {
										if ((Idx_ + tokenlen_) >= scriptlen_) {
											Finished = true;
											Section = SECTION.SECTION_PROCESSPARAMS;
										} else
											tokenlen_++;
									}
								}
								if (Found)
									script_ [Idx_ + tokenlen_] = 0; // make null terminated string
								Idx_ += tokenlen_ + 1;
								tokenlen_ = 0;
							} else if (isdigit (script_ [Idx_]) || script_ [Idx_] == '-') { // Number
								// find white space
								Found = false;
								tokenlen_ = 1;
								while (!Found) {
									switch (script_ [Idx_ + tokenlen_]) {
									case ' ':
									case ',':
									case (char)0x09:
									case (char)0x0a:
									case (char)0x0d:
										Found = true;
										break;
									default:
										tokenlen_++;
										break;
									}
									if ((Idx_ + tokenlen_) >= scriptlen_) {
										Found = true;
										Finished = true;
										Section = SECTION.SECTION_PROCESSPARAMS;
									}
								}
								if (Found && P_Idx_ < PARSE_MAX_PARAMS)
									P_ [P_Idx_++] = long.Parse (script_ [Idx_]);

								Idx_ += tokenlen_;
								tokenlen_ = 0;
							} else {
								// Look for ID in tables
								// Look for white space
								Found = false;
								tokenlen_ = 0;
								while (!Found && !Finished) {
									switch (script_ [Idx_ + tokenlen_]) {
									case ' ':
									case ',':
									case (char)0x09:
									case (char)0x0a:
									case (char)0x0d:
										Found = true;
										break;
									default:
										tokenlen_++;
										break;
									}
									if ((Idx_ + tokenlen_) >= scriptlen_) {
										Finished = true;
										Found = true;
										Section = SECTION.SECTION_PROCESSPARAMS;
									}
								}
								if (Found && P_Idx_ < PARSE_MAX_PARAMS) {
									script_ [Idx_ + tokenlen_] = 0;
									P_ [P_Idx_++] = FindID (script_ .Substring (Idx_));
									if (P_ [P_Idx_ - 1] < 0 && script_ [Idx_] != "NID")
										TokenErrorList.AddText (script_ [Idx_]);
								}
								Idx_ += tokenlen_ + 1;
								tokenlen_ = 0;
							}
						}
					}
					break;
				case SECTION.SECTION_PROCESSPARAMS:
					switch (TokenType) {
					case TOKEN.TOKEN_COMMON:
						Control_.BaseFunction ((short)(TokenID), P_, str_, Handler_);
						break;
					case TOKEN.TOKEN_LOCAL:
						Control_.LocalFunction ((short)(TokenID), P_, str_, Handler_);
						break;
					}
					P_Idx_ = 0;
					P_ [0] = 0;
					P_ [1] = 0;
					P_ [2] = 0;
					P_ [3] = 0;
					P_ [4] = 0;
					P_ [5] = 0;
					P_ [6] = 0;
					P_ [7] = 0;
					str_ = null;
					Section = SECTION.SECTION_FINDSUBTOKEN;
					break;
				}
			}
			return(Control_);
#endif
			throw new NotImplementedException ();
		}

		private void AddInternalIDs <T> (Dictionary<string, T> tbl) where T: struct, IConvertible
		{
			short i;

			i = 0;
			foreach (KeyValuePair<string, T> e in tbl) {

				TokenOrder_.AddTextID (e.Value.ToInt64 (CultureInfo.InvariantCulture), e.Key);
				i++;
			}
		}

		private long TokenizeIDs (string idfile, long size)
		{
#if TODO
			long idx, count, expecting;

			idx = 0;

// remove ALL white space
			while (idx < size) {
				if (idfile [idx] <= ' ' || idfile [idx] == ',')
					idfile [idx] = 0;
				idx++;
			}

			idx = 0;
			count = 0;
			expecting = 0;

// expecting: 0-looking for ID,1-looking for end of id
//            2-looking for value,3-looking for end of value

			while (idx <= size) {
				if (!idfile [idx]) {
					if (expecting == 1)
						expecting = 2;
					else if (expecting == 3) {
						expecting = 0;
						count++;
					}
				} else {
					if (expecting == 0) {
						if (isdigit (idfile [idx]) || idfile [idx] == '-')
							idfile [idx] = 0;
						else
							expecting = 1;
					} else if (expecting == 2) {
						if (isdigit (idfile [idx]) || idfile [idx] == '-')
							expecting = 3;
						else
							idfile [idx] = 0;
					} else if (expecting == 3) {
						if (!isdigit (idfile [idx])) {
							idfile [idx] = 0;
							count++;
							expecting = 0;
						}
					}
				}
				idx++;
			}
			return(count);
#endif
			throw new NotImplementedException ();
		}

		private void LoadIDTable (string filename)
		{
			FileStream ifp;
			long size;
			try {
				//	ifp=UI_OPEN(filename,"rb");
				ifp = OpenArtFile (filename, FalconUIArtThrDirectory, FalconUIArtDirectory, false);

				if (ifp == null) {
					LogError ("LoadIDTable load failed (" + filename + ")");
				}

				size = ifp.Length;

				if (size == 0) {
					ifp.Close ();
					LogError ("LoadIDTable seek end failed (" + filename + ")");
				
				}

				using (StreamReader sr = new StreamReader (ifp)) {
					string strLine = sr.ReadLine ();
					while (strLine != null) {
						strLine = sr.ReadLine ();
						if (!string.IsNullOrWhiteSpace (strLine)) {
							List<string> words = strLine.SplitWords ();
							int ID = int.Parse (words [1]);
							string token = words [0];
							TokenOrder_.AddTextID (ID, token);
						}
					}
					sr.Close ();
				}
			} catch (IOException e) {
				Debug.WriteLine ("An IO exception has been thrown!");
				Debug.WriteLine (e.ToString ());
				return;
			}
#if TODO	
			idfile = new char [size + 5]; // just in case :)
			if (UI_READ (idfile, size, 1, ifp) != 1) {
				if (g_bLogUiErrors) {
					if (Perror_)
						fprintf (Perror_, "LoadIDTable read failed (%s)\n", filename);
				}
				UI_CLOSE (ifp);
				idfile = null;
				return;
			}
			idfile [size] = 0;

			UI_CLOSE (ifp);

			count = TokenizeIDs (idfile, size);
			if (count) {
				i = 0;
				idx = 0;
				while (i < count) {
					while (!idfile[idx] && idx < size)
						idx++;

					token = &idfile [idx];

					while (idfile[idx] && idx < size)
						idx++;

					while (!idfile[idx] && idx < size)
						idx++;

					ID = atol (&idfile [idx]);

					while (idfile[idx] && idx < size)
						idx++;

					TokenOrder_.AddTextID (ID, token);
					i++;
				}
			}

			idfile = null;
#endif
		}
		public static FileStream OpenArtFile (string filename)
		{
			string path;
			if (filename.StartsWith("art\\"))
				path = filename.Substring(4);
			else
				path = filename;
			return OpenArtFile(path.Replace('\\', Path.DirectorySeparatorChar), FalconUIArtThrDirectory, FalconUIArtDirectory);
		}
		
		public static FileStream OpenArtFile (string filename, string thrdir, string maindir, bool hirescapable = true)
		{
			FileStream ifp;
	
			if (char.IsLetter (filename [0]) && filename [1] == ':' && filename [2] == Path.DirectorySeparatorChar)
				return File.Open (filename, FileMode.Open, FileAccess.Read);

			filebuf = thrdir; // Falcon thr root dir
			if (hirescapable) {
				if (false) // TODO F4Config.g_bHiResUI)
					filebuf += Path.DirectorySeparatorChar + "art1024";		// HiResUI
				else
					filebuf += Path.DirectorySeparatorChar + "art";			// LoResUI
			}
			filebuf += Path.DirectorySeparatorChar + filename;
			ifp = File.Open (filebuf, FileMode.Open, FileAccess.Read);
			if (ifp != null)
				return ifp; // got the main one
		
			filebuf = maindir; // Falcon main root dir
			if (hirescapable) {
				if (false) // TODO F4Config.g_bHiResUI)
					filebuf += Path.DirectorySeparatorChar + "art1024";		// HiResUI
				else
					filebuf += Path.DirectorySeparatorChar + "art";			// LoResUI
			}
			filebuf += Path.DirectorySeparatorChar + filename;
			return File.Open (filebuf, FileMode.Open, FileAccess.Read);
		}
	
		public C_Parser ()
		{
			Idx_ = 0; // index into script
			script_ = null; // script file (read into memory)
			scriptlen_ = 0;
			tokenlen_ = 0;
			P_Idx_ = 0;
			str_ = null;

			Perror_ = null;

			Handler_ = null;
			Window_ = null;
			Control_ = null;
			Anim_ = null;
			Font_ = null;
			Image_ = null;
			Anim_ = null;
			Sound_ = null;
			Popup_ = null;
			String_ = null;

			TokenOrder_ = null;

			P_ = new long[PARSE_MAX_PARAMS];
		}

		//TODO public ~C_Parser();

		public void Setup (C_Handler handler, C_Image ImgMgr, C_Font FontList, C_Sound SndMgr, C_PopupMgr PopupMgr, C_Animation AnimMgr, C_String StringMgr, C_Movie MovieMgr)
		{

			Handler_ = handler;
			Image_ = ImgMgr;
			Anim_ = AnimMgr;
			Font_ = FontList;
			Sound_ = SndMgr;
			Popup_ = PopupMgr;
			String_ = StringMgr;
			Movie_ = MovieMgr;

			TokenOrder_ = new C_Hash ();
			TokenOrder_.Setup (PARSE_HASH_SIZE);
			TokenOrder_.SetFlags (UI95_BITTABLE.C_BIT_REMOVE);

			TokenErrorList = new C_Hash ();
			TokenErrorList.Setup (512);
			TokenErrorList.SetFlags (UI95_BITTABLE.C_BIT_REMOVE);

			AddInternalIDs (UI95_BitTable);
			AddInternalIDs (UI95_Table);
#if TODO			
			AddInternalIDs (UI95_FontTable);

			Sound_.SetIDTable (TokenOrder_);
	
			if (g_bLogUiErrors) {
				Perror_ = UI_OPEN ("ui95err.log", "a");
				if (Perror_)
					fprintf (Perror_, "Setup Parser\n");
			}
#endif
			// TODO throw new NotImplementedException ();
		}

		public void Cleanup ()
		{
#if TODO
			if (g_bLogUiErrors) {
	
				if (Perror_)
					fprintf (Perror_, "Cleanup Parser\n");
				if (Perror_)
					UI_CLOSE (Perror_);
			}
			Perror_ = null;

			TokenOrder_.Cleanup ();
			delete TokenOrder_;
			TokenOrder_ = null;

			TokenErrorList.Cleanup ();
			delete TokenErrorList;
			TokenErrorList = null;

			Handler_ = null;
			Window_ = null;
			Control_ = null;
			Font_ = null;
			Image_ = null;
			Anim_ = null;
			Sound_ = null;
			String_ = null;
#endif
			throw new NotImplementedException ();
		}

		public string FindIDStr (long ID)
		{
			ValueStr = ID.ToString ();
			return ValueStr;
		}

		public long FindID (string token)
		{
		
			return(TokenOrder_.FindTextID (token));
		}

		private int FindToken (string token, int pos)
		{
			int i = 0;
			foreach (String str in C_All_Tokens) {
				if (str.Equals (token.Substring (pos, str.Length).ToUpperInvariant ()))
					return i;
				i++;
			}
			return 0;
		}

		public void SetCheck (long ID)
		{			
			if (TokenOrder_ != null)
				TokenOrder_.SetCheck (ID);
		}

		public void LoadIDList (string filename)
		{
			FileStream ifp;
			long size;
			string listfile, lfp;
			long i;

			WindowList_ = new long[MAX_WINDOWS_IN_LIST];
			WinIndex_ = 0;
			WinLoaded_ = 0;

#if NOTHING
	char filebuf[_MAX_PATH];
	strcpy(filebuf,FalconUIArtDirectory); // Falcon root
	if (F4Config.g_bHiResUI)
		strcat(filebuf,"\\art1024");		// HiResUI
	else
		strcat(filebuf,"\\art");			// LoResUI
	strcat(filebuf,"\\");
	strcat(filebuf,filename);
	ifp=UI_OPEN(filebuf,"rb");
#endif
			try {
				ifp = OpenArtFile (filename, FalconUIArtThrDirectory, FalconUIArtDirectory);

				if (ifp == null) {
					throw new ArgumentException ("LoadIDTable read failed (" + filename + ")");
				}

			
				size = ifp.Length;

				if (size == 0) {
					ifp.Close ();
					return;
				}
			
				using (StreamReader sr = new StreamReader (ifp)) {
					string strLine = sr.ReadLine ();
					while (strLine != null) {
						strLine = sr.ReadLine ();
						if (!string.IsNullOrWhiteSpace (strLine)) {
							strLine = strLine.Trim ();
							if (!strLine.StartsWith ("#")) {
								LoadIDTable (strLine.Replace ('\\', Path.DirectorySeparatorChar));
							}
						}
					}
					sr.Close ();
				}
			} catch (IOException e) {
				Debug.WriteLine ("An IO exception has been thrown!");
				Debug.WriteLine (e.ToString ());
				return;
			}
#if TODO
			listfile = new char [size + 5]; // just in case :)
			if (UI_READ (listfile, size, 1, ifp) != 1) {
				listfile = null;
				UI_CLOSE (ifp);
				return;
			}
			listfile [size] = 0;

			UI_CLOSE (ifp);

			for (i=0; i<size; i++)
				if (listfile [i] < 32)
					listfile [i] = 0;

			lfp = listfile;
			i = 0;
			while (i < size) {
				while (!(*lfp) && i < size) {
					lfp++;
					i++;
				}
				if (*lfp) {
					if (*lfp != '#') {
						//strcpy(filebuf,FalconUIArtDirectory);
						//strcat(filebuf,"\\");
						//strcat(filebuf,lfp);
						LoadIDTable (lfp);
					}
					while ((*lfp) && i < size) {
						lfp++;
						i++;
					}
				}
			}
			listfile = null;
#endif
		}
		
		public FileStream OpenScript (string filename)
		{
			FileStream ifp = OpenArtFile (filename, FalconUIArtThrDirectory, FalconUIArtDirectory, false);

			//	ifp=UI_OPEN(filename,"rb");
			if (ifp == null) {
				LogError ("LoadScript load failed (" + filename + ")");
			}

			long size = ifp.Length;
	
			if (size == 0) {
				ifp.Close ();
				LogError ("LoadScript seek start failed (" + filename + ")");
			}

			return ifp;
		}

		public bool LoadScript (string filename)
		{
			FileStream ifp;
			long size;

			ifp = OpenArtFile (filename, FalconUIArtThrDirectory, FalconUIArtDirectory, false);


			//	ifp=UI_OPEN(filename,"rb");
			if (ifp == null) {
				LogError ("LoadScript load failed (" + filename + ")");
				return(false);
			}

			size = ifp.Length;
	
			if (size == 0) {
				ifp.Close ();
				LogError ("LoadScript seek start failed (" + filename + ")");
				return(false);
			}


			scriptlen_ = size;
			
			return true;
#if TODO	
			if (script_)
				script_ = null;
			script_ = new char [size + 5]; // just in case :)
				if (script_)
				memset (script_, 0, size + 5);	// OW
			if (UI_READ (script_, size, 1, ifp) != 1) {
				if (g_bLogUiErrors) {
					if (Perror_)
						fprintf (Perror_, "LoadScript read failed (%s)\n", filename);
				}
				script_ = null;
				UI_CLOSE (ifp);
				return(false);
			}
			script_ [size] = 0;

			UI_CLOSE (ifp);
			return(true);
#endif
		}

		public bool  ParseScript (string filename)
		{
#if TODO
			int Done, Comment, Found, InString;//!,Finished=0;;
			int TokenID, Section, TokenType;

			if (LoadScript (filename) == false)
				return(false);

			Idx_ = 0;
			P_Idx_ = 0;
			tokenlen_ = 0;

			Done = 0;
			Comment = 0;
			InString = 0;
			Section = SECTION.SECTION_FINDTOKEN;
			TokenType = TOKEN.TOKEN_NOTHING;

			while (!Done) {
				switch (Section) {
				case SECTION.SECTION_FINDTOKEN:
			// Look for token starting with '['
					Found = false;
					while (!Found && !Done) {
						switch (script_ [Idx_]) {
						case '[':
							if (!Comment && !InString) {
								Found = true;
								break;
							}
							Idx_++;
							break;
						case '"':
							InString = 1 - InString;
							Idx_++;
							break;
						case '#':
							Comment = true;
							Idx_++;
							break;
						case 0x0a:
						case 0x0d:
							Comment = 0;
							Idx_++;
							break;
						default:
							Idx_++;
						}
						if (Idx_ >= scriptlen_)
							Done = true;
					}
							
					tokenlen_ = 1;
					while (script_[Idx_+tokenlen_] != ']' && (Idx_+tokenlen_) < scriptlen_)
						tokenlen_++;

					tokenlen_++;

					if ((Idx_ + tokenlen_) >= scriptlen_) {
						Done = true;
						break;
					}

					if (Found == 1)
						Section = SECTION.SECTION_PROCESSTOKEN;
					break;
				case SECTION.SECTION_PROCESSTOKEN:
					TokenID = FindToken (&script_ [Idx_]);
					if (TokenID) {
						Section = SECTION.SECTION_FINDSUBTOKEN;
						switch (TokenID) {
						case CPARSE.CPARSE_WINDOW:
							Handler_.AddWindow (WindowParser (), C_BIT_NOTHING);
							break;
						case CPARSE.CPARSE_FONT:
							TokenType = TOKEN.TOKEN_FONT;
							Idx_ += tokenlen_;
							tokenlen_ = 0;
							break;
						case CPARSE.CPARSE_SOUND:
							TokenType = TOKEN.TOKEN_SOUND;
							Idx_ += tokenlen_;
							tokenlen_ = 0;
							break;
						case CPARSE.CPARSE_STRING:
							TokenType = TOKEN.TOKEN_STRING;
							Idx_ += tokenlen_;
							tokenlen_ = 0;
							break;
						default:
							Section = SECTION.SECTION_FINDTOKEN;
							Idx_ += tokenlen_;
							tokenlen_ = 0;
							break;
						}
					} else {
						Section = SECTION.SECTION_FINDTOKEN;
						Idx_ += tokenlen_;
						tokenlen_ = 0;
					}
					break;
				}
			}
			return(true);
#endif
			throw new NotImplementedException ();
		}

		public bool  LoadWindowList (string filename)
		{
#if TODO
			UI_HANDLE ifp;
			long size;
			string listfile, lfp;
			long i;
			C_Window win;

			memset (&WindowList_ [0], 0, sizeof(long) * MAX_WINDOWS_IN_LIST);
			WinIndex_ = 0;
			WinLoaded_ = 0;
	
			if (g_bLogUiErrors) {
				if (Perror_)
					fprintf (Perror_, "LoadWindowList processing (%s)\n", filename);
			}
			ifp = OpenArtFile (filename, FalconUIArtThrDirectory, FalconUIArtDirectory);
			if (ifp == null) {
				if (g_bLogUiErrors) {
					if (Perror_)
						fprintf (Perror_, "LoadWindowList open failed (%s)\n", filename);
				}
				return(false);
			}
			if (g_bLogUiErrors) {
				if (Perror_)
					fprintf (Perror_, "Open Art file found as %s\n", filebuf);
			}


			size = UI_FILESIZE (ifp);
	
			if (!size) {
				if (g_bLogUiErrors) {
					if (Perror_)
						fprintf (Perror_, "LoadWindowList seek start failed (%s)\n", filename);
				}
				UI_CLOSE (ifp);
				return(false);
			}
	
			listfile = new char [size + 5]; // just in case :)
			if (UI_READ (listfile, size, 1, ifp) != 1) {
				if (g_bLogUiErrors) {
					if (Perror_)
						fprintf (Perror_, "LoadWindowList read failed (%s)\n", filename);
				}
				listfile = null;
				UI_CLOSE (ifp);
				return(false);
			}
			listfile [size] = 0;

			UI_CLOSE (ifp);

			for (i=0; i<size; i++)
				if (listfile [i] < 32)
					listfile [i] = 0;

			lfp = listfile;
			i = 0;
			while (i < size) {
				while (!(*lfp) && i < size) {
					lfp++;
					i++;
				}
				if (*lfp) {
					if (g_bLogUiErrors) {
						if (Perror_)
							fprintf (Perror_, "LoadWindowList Parsing Window (%s)\n", lfp);
					}
					if (*lfp != '#') {
						//strcpy(filebuf,FalconUIArtDirectory);
						//strcat(filebuf,"\\");
						//strcat(filebuf,lfp);
						win = ParseWindow (lfp);

						if (win) {
							WindowList_ [WinLoaded_ ++] = win.GetID ();
							Handler_.AddWindow (win, win.GetFlags ());
							win.ScanClientAreas ();
						} else {
				    
							if (g_bLogUiErrors) {
					
								if (Perror_) {
									fprintf (Perror_, "LoadWindowList NO Window returned (%s)\n", lfp);
								}
							}
						}
					}

					while ((*lfp) && i < size) {
						lfp++;
						i++;
					}
				}
			}
			listfile = null;
			return(true);
#endif
			throw new NotImplementedException ();
		}

		public bool  LoadSoundList (string filename)
		{
#if TODO
			UI_HANDLE ifp;
			long size;
			string listfile, lfp;
			long i;

#if NOTHING
	strcpy(filebuf,FalconUIArtDirectory); // Falcon root
	if (F4Config.g_bHiResUI)
		strcat(filebuf,"\\art1024");		// HiResUI
	else
		strcat(filebuf,"\\art");			// LoResUI
	strcat(filebuf,"\\");
	strcat(filebuf,filename);
	ifp=UI_OPEN(filebuf,"rb");
#endif
			ifp = OpenArtFile (filename, FalconUIArtThrDirectory, FalconUIArtDirectory);
			if (ifp == null) {
				if (g_bLogUiErrors) {
					if (Perror_)
						fprintf (Perror_, "LoadSoundList open failed (%s)\n", filename);
				}
				return(false);
			}
	
			size = UI_FILESIZE (ifp);

			if (!size) {
				if (g_bLogUiErrors) {
					if (Perror_)
						fprintf (Perror_, "LoadSoundList seek start failed (%s)\n", filename);
				}
				UI_CLOSE (ifp);
				return(false);
			}
	
			listfile = new char [size + 5]; // just in case :)
			if (UI_READ (listfile, size, 1, ifp) != 1) {
				if (g_bLogUiErrors) {
					if (Perror_)
						fprintf (Perror_, "LoadSoundList read failed (%s)\n", filename);
				}
				listfile = null;
				UI_CLOSE (ifp);
				return(false);
			}
			listfile [size] = 0;

			UI_CLOSE (ifp);

			for (i=0; i<size; i++)
				if (listfile [i] < 32)
					listfile [i] = 0;

			lfp = listfile;
			i = 0;
			while (i < size) {
				while (!(*lfp) && i < size) {
					lfp++;
					i++;
				}
				if (*lfp) {
					byte[] filebuf = new byte[_MAX_PATH];
					strcpy (filebuf, FalconUISoundDirectory);
					strcat (filebuf, "\\");
					strcat (filebuf, lfp);
					ParseSound (filebuf);
					while ((*lfp) && i < size) {
						lfp++;
						i++;
					}
				}
			}
			listfile = null;
			return(true);
#endif
			throw new NotImplementedException ();
		}

		public bool  LoadStringList (string filename)
		{
#if TODO
			UI_HANDLE ifp;
			long size;
			string listfile, fp;
			long i;

#if NOTHING
	strcpy(filebuf,FalconUIArtDirectory); // Falcon root
	if (F4Config.g_bHiResUI)
		strcat(filebuf,"\\art1024");		// HiResUI
	else
	    strcat(filebuf,"\\art");			// LoResUI
	strcat(filebuf,"\\");
	strcat(filebuf,filename);
	ifp=UI_OPEN(filebuf,"rb");
#endif
			ifp = OpenArtFile (filename, FalconUIArtThrDirectory, FalconUIArtDirectory);
			if (ifp == null) {
				if (g_bLogUiErrors) {
					if (Perror_)
						fprintf (Perror_, "LoadStringList open failed (%s)\n", filename);
				}
				return(false);
			}

			size = UI_FILESIZE (ifp);
	
			if (!size) {
				if (g_bLogUiErrors) {
					if (Perror_)
						fprintf (Perror_, "LoadStringList seek start failed (%s)\n", filename);
				}
				UI_CLOSE (ifp);
				return(false);
			}

			listfile = new char [size + 5]; // just in case :)
			if (UI_READ (listfile, size, 1, ifp) != 1) {
				if (g_bLogUiErrors) {
					if (Perror_)
						fprintf (Perror_, "LoadStringList read failed (%s)\n", filename);
				}
				listfile = null;
				UI_CLOSE (ifp);
				return(false);
			}
			listfile [size] = 0;

			UI_CLOSE (ifp);

			for (i=0; i<size; i++)
				if (listfile [i] < 32)
					listfile [i] = 0;

			lfp = listfile;
			i = 0;
			while (i < size) {
				while (!(*lfp) && i < size) {
					lfp++;
					i++;
				}
				if (*lfp) {
					//strcpy(filebuf,FalconUIArtDirectory);
					//strcat(filebuf,"\\");
					//strcat(filebuf,lfp);
					ParseString (lfp);
					while ((*lfp) && i < size) {
						lfp++;
						i++;
					}
				}
			}
			listfile = null;
			return(true);
#endif
			throw new NotImplementedException ();
		}

		public bool  LoadMovieList (string filename)
		{
#if TODO
			UI_HANDLE ifp;
			long size;
			string listfile, lfp;
			long i;

#if NOTHING
	strcpy(filebuf,FalconUIArtDirectory); // Falcon root
	if (F4Config.g_bHiResUI)
		strcat(filebuf,"\\art1024");		// HiResUI
	else
		strcat(filebuf,"\\art");			// LoResUI
	strcat(filebuf,"\\");
	strcat(filebuf,filename);
	ifp=UI_OPEN(filebuf,"rb");
#endif
			ifp = OpenArtFile (filename, FalconUIArtThrDirectory, FalconUIArtDirectory);

			if (ifp == null) {
				if (g_bLogUiErrors) {
					if (Perror_)
						fprintf (Perror_, "LoadMovieList open failed (%s)\n", filename);
				}
				return(false);
			}

			size = UI_FILESIZE (ifp);
	
			if (!size) {
				if (g_bLogUiErrors) {
					if (Perror_)
						fprintf (Perror_, "LoadMovieList seek start failed (%s)\n", filename);
				}
				UI_CLOSE (ifp);
				return(false);
			}
	
			listfile = new char [size + 5]; // just in case :)
			if (UI_READ (listfile, size, 1, ifp) != 1) {
				if (g_bLogUiErrors) {
					if (Perror_)
						fprintf (Perror_, "LoadMovieList read failed (%s)\n", filename);
				}
				listfile = null;
				UI_CLOSE (ifp);
				return(false);
			}
			listfile [size] = 0;

			UI_CLOSE (ifp);

			for (i=0; i<size; i++)
				if (listfile [i] < 32)
					listfile [i] = 0;

			lfp = listfile;
			i = 0;
			while (i < size) {
				while (!(*lfp) && i < size) {
					lfp++;
					i++;
				}
				if (*lfp) {
					ParseMovie (lfp); // Don't tack on movie directory... handled by PlayMovie function
					while ((*lfp) && i < size) {
						lfp++;
						i++;
					}
				}
			}
			listfile = null;
			return(true);
#endif
			throw new NotImplementedException ();
		}

		public bool  LoadImageList (string filename)
		{
#if TODO
			UI_HANDLE ifp;
			long size;
			string listfile, lfp;
			long i;

#if NOTHING
	strcpy(filebuf,FalconUIArtDirectory); // Falcon root
	if (F4Config.g_bHiResUI)
		strcat(filebuf,"\\art1024");		// HiResUI
	else
		strcat(filebuf,"\\art");			// LoResUI
	strcat(filebuf,"\\");
	strcat(filebuf,filename);
	ifp=UI_OPEN(filebuf,"rb");
#endif
			ifp = OpenArtFile (filename, FalconUIArtThrDirectory, FalconUIArtDirectory);
			if (ifp == null) {
				if (g_bLogUiErrors) {
					if (Perror_)
						fprintf (Perror_, "LoadImageList open failed (%s)\n", filename);
				}
				return(false);
			}
	
			size = UI_FILESIZE (ifp);

			if (!size) {
				if (g_bLogUiErrors) {
					if (Perror_)
						fprintf (Perror_, "LoadImageList seek start failed (%s)\n", filename);
				}
				UI_CLOSE (ifp);
				return(false);
			}

			listfile = new char [size + 5]; // just in case :)
			if (UI_READ (listfile, size, 1, ifp) != 1) {
				if (g_bLogUiErrors) {
					if (Perror_)
						fprintf (Perror_, "LoadImageList read failed (%s)\n", filename);
				}
				listfile = null;
				UI_CLOSE (ifp);
				return(false);
			}
			listfile [size] = 0;

			UI_CLOSE (ifp);

			for (i=0; i<size; i++)
				if (listfile [i] < 32)
					listfile [i] = 0;

			lfp = listfile;
			i = 0;
			while (i < size) {
				while (!(*lfp) && i < size) {
					lfp++;
					i++;
				}
				if (*lfp) {
					//strcpy(filebuf,FalconUIArtDirectory);
					//strcat(filebuf,"\\");
					//strcat(filebuf,lfp);
					ParseImage (lfp);
					while ((*lfp) && i < size) {
						lfp++;
						i++;
					}
				}
			}
			listfile = null;
			return(true);
#endif
			throw new NotImplementedException ();
		}

		public bool  LoadPopupMenuList (string filename)
		{
#if TODO
			UI_HANDLE ifp;
			long size;
			string listfile, lfp;
			long i;
			C_PopupList Menu;

			if (g_bLogUiErrors) {
				if (Perror_)
					fprintf (Perror_, "LoadPopupMenuList processing (%s)\n", filename);
			}
#if NOTHING
	strcpy(filebuf,FalconUIArtDirectory);
	strcat(filebuf,"\\");
	strcat(filebuf,filename);
	ifp=UI_OPEN(filebuf,"rb");
#endif
			ifp = OpenArtFile (filename, FalconUIArtThrDirectory, FalconUIArtDirectory, 0);
			if (ifp == null) {
				if (g_bLogUiErrors) {
					if (Perror_)
						fprintf (Perror_, "LoadPopupMenuList open failed (%s)\n", filename);
				}
				return(false);
			}

			size = UI_FILESIZE (ifp);

			if (!size) {
				if (g_bLogUiErrors) {
					if (Perror_)
						fprintf (Perror_, "LoadPopupMenuList seek start failed (%s)\n", filename);
				}

				UI_CLOSE (ifp);
				return(false);
			}

			listfile = new char [size + 5]; // just in case :)
			if (UI_READ (listfile, size, 1, ifp) != 1) {
				if (g_bLogUiErrors) {
					if (Perror_)
						fprintf (Perror_, "LoadPopupMenuList read failed (%s)\n", filename);
				}
				listfile = null;
				UI_CLOSE (ifp);
				return(false);
			}
			listfile [size] = 0;

			UI_CLOSE (ifp);

			for (i=0; i<size; i++)
				if (listfile [i] < 32)
					listfile [i] = 0;

			lfp = listfile;
			i = 0;
			while (i < size) {
				while (!(*lfp) && i < size) {
					lfp++;
					i++;
				}
				if (*lfp) {
					if (g_bLogUiErrors) {
						if (Perror_)
							fprintf (Perror_, "LoadPopupMenuList Parsing PopMenu (%s)\n", lfp);
					}

					//strcpy(filebuf,FalconUIArtDirectory);
					//strcat(filebuf,"\\");
					//strcat(filebuf,lfp);
					Menu = (C_PopupList *)ParsePopupMenu (lfp);
					if (Menu)
						Popup_.AddMenu (Menu);
					else {
						if (g_bLogUiErrors) {
							if (Perror_)
								fprintf (Perror_, "LoadPopupMenuList NO Popup Menu returned (%s)\n", lfp);
						}
					}

					while ((*lfp) && i < size) {
						lfp++;
						i++;
					}
				}
			}
			listfile = null;
			return(true);
#endif
			throw new NotImplementedException ();
		}

		public C_SoundBite ParseSoundBite (string filename)
		{
#if TODO
			C_SoundBite Bite = null;
			bool Done = false, Comment = false, Found = false, InString = false, Finished = false;
			SECTION Section = 0;

			if (LoadScript (filename) == false)
				return(null);

			Bite = new C_SoundBite ();
			if (Bite == null)
				return(null);
			Bite.Setup ();

			Section = SECTION.SECTION_FINDPARAMS;

			while (!Done) {
				switch (Section) {
				case SECTION.SECTION_FINDPARAMS:
					P_Idx_ = 0; // start with 0 parameters

				// Repeat until token char '[' found (or EOF)
					Finished = false;
					while (!Finished) {
						// Find NON white space
						Found = false;
						while (!Found && !Done && !Finished) {
							switch (script_ [Idx_]) {
							case ' ':
							case ',':
							case (char)0x09: 
								Idx_++;
								break;
							case (char)0x0a:
							case (char)0x0d:
								Comment = 0;
								Finished = true;
								Section = SECTION.SECTION_PROCESSPARAMS;
								Idx_++;
								break;
							case '#': // Comment
								Comment = true;
								Idx_++;
								break;
							default:
								if (!Comment) {
									Found = true;
									break;
								}
								Idx_++;
								break;
							}
							if (Idx_ >= scriptlen_) {
								Finished = true;
								Section = SECTION.SECTION_PROCESSPARAMS;
							}
						}

						if (Found) {
							Found = false;
							if (script_ [Idx_] == '"') { // string
								tokenlen_ = 1;
								str_ = &script_ [Idx_ + tokenlen_];
								// Find closing (")
								while (!Found && !Finished) {
									if (script_ [Idx_ + tokenlen_] == '"')
										Found = true;
									else {
										if ((Idx_ + tokenlen_) >= scriptlen_) {
											Finished = true;
											Section = SECTION.SECTION_PROCESSPARAMS;
										} else
											tokenlen_++;
									}
								}
								if (Found)
									script_ [Idx_ + tokenlen_] = 0; // make null terminated string
								Idx_ += tokenlen_ + 1;
								tokenlen_ = 0;
							} else if (isdigit (script_ [Idx_]) || script_ [Idx_] == '-') { // Number
								// find white space
								Found = false;
								tokenlen_ = 1;
								while (!Found) {
									switch (script_ [Idx_ + tokenlen_]) {
									case ' ':
									case ',':
									case (char)0x09:
									case (char)0x0a:
									case (char)0x0d:
										Found = true;
										break;
									default:
										tokenlen_++;
										break;
									}
									if ((Idx_ + tokenlen_) >= scriptlen_) {
										Found = true;
										Finished = true;
										Section = SECTION.SECTION_PROCESSPARAMS;
									}
								}
								if (Found && P_Idx_ < PARSE_MAX_PARAMS)
									P_ [P_Idx_++] = atol (&script_ [Idx_]);

								Idx_ += tokenlen_;
								tokenlen_ = 0;
							} else {
								// Look for ID in tables
								// Look for white space
								Found = false;
								tokenlen_ = 0;
								while (!Found && !Finished) {
									switch (script_ [Idx_ + tokenlen_]) {
									case ' ':
									case ',':
									case (char)0x09:
									case (char)0x0a:
									case (char)0x0d:
										Found = true;
										break;
									default:
										tokenlen_++;
										break;
									}
									if ((Idx_ + tokenlen_) >= scriptlen_) {
										Finished = true;
										Found = true;
										Section = SECTION.SECTION_PROCESSPARAMS;
									}
								}
								if (Found && P_Idx_ < PARSE_MAX_PARAMS) {
									script_ [Idx_ + tokenlen_] = 0;
									P_ [P_Idx_++] = FindID (&script_ [Idx_]);
									if (P_ [P_Idx_ - 1] < 0 && strcmp (&script_ [Idx_], "NID"))
										TokenErrorList.AddText (&script_ [Idx_]);
								}
								Idx_ += tokenlen_ + 1;
								tokenlen_ = 0;
							}
						}
					}
					break;
				case SECTION.SECTION_PROCESSPARAMS:
					if (P_Idx_)
						Bite.Add (P_ [0], P_ [1]);
					P_Idx_ = 0;
					P_ [0] = 0;
					P_ [1] = 0;
					P_ [2] = 0;
					P_ [3] = 0;
					P_ [4] = 0;
					P_ [5] = 0;
					P_ [6] = 0;
					P_ [7] = 0;
					str_ = null;
					if (Idx_ >= scriptlen_)
						Done = true;
					Section = SECTION.SECTION_FINDPARAMS;
					break;
				}
			}
			return(Bite);
#endif
			throw new NotImplementedException ();
		}

		public C_Base ParseControl (string filename)
		{
#if TODO			
			long Done = 0, Comment = 0, Found = 0, InString = 0;//!,Finished=0;;
			long TokenID = 0, Section = 0, TokenType = 0;

			if (LoadScript (filename) == false)
				return null;

			Idx_ = 0;
			P_Idx_ = 0;
			tokenlen_ = 0;

			Done = 0;
			Comment = 0;
			InString = 0;
			Section = SECTION.SECTION_FINDTOKEN;
			TokenType = TOKEN.TOKEN_NOTHING;

			while (!Done) {
				switch (Section) {
				case SECTION.SECTION_FINDTOKEN:
			// Look for token starting with '['
					Found = false;
					while (!Found && !Done) {
						switch (script_ [Idx_]) {
						case '[':
							if (!Comment && !InString) {
								Found = true;
								break;
							}
							Idx_++;
							break;
						case '"':
							InString = 1 - InString;
							Idx_++;
							break;
						case '#':
							Comment = true;
							Idx_++;
							break;
						case 0x0a:
						case 0x0d:
							Comment = 0;
							Idx_++;
							break;
						default:
							Idx_++;
						}
						if (Idx_ >= scriptlen_)
							Done = true;
					}
							
					tokenlen_ = 0;
					while (script_[Idx_+tokenlen_] != ']' && (Idx_+tokenlen_) < scriptlen_)
						tokenlen_++;
					tokenlen_++;

					if ((Idx_ + tokenlen_) >= scriptlen_) {
						Done = true;
						break;
					}

					if (Found == 1)
						Section = SECTION.SECTION_PROCESSTOKEN;
					break;
				case SECTION.SECTION_PROCESSTOKEN:
					TokenID = FindToken (&script_ [Idx_]);
					if (TokenID) {
						switch (TokenID) {
						case CPARSE.CPARSE_BUTTON:
						case CPARSE.CPARSE_TEXT:
						case CPARSE.CPARSE_BOX:
						case CPARSE.CPARSE_LINE:
						case CPARSE.CPARSE_CLOCK:
						case CPARSE.CPARSE_FILL:
						case CPARSE.CPARSE_TREE:
						case CPARSE.CPARSE_EDITBOX:
						case CPARSE.CPARSE_LISTBOX:
						//case CPARSE.CPARSE_ACMI:
						case CPARSE.CPARSE_PANNER:
						case CPARSE.CPARSE_SLIDER:
						case CPARSE.CPARSE_SCROLLBAR:
						case CPARSE.CPARSE_TREELIST:
						case CPARSE.CPARSE_BITMAP:
						case CPARSE.CPARSE_TILE:
						case CPARSE.CPARSE_ANIM:
						case CPARSE.CPARSE_CURSOR:
						case CPARSE.CPARSE_MARQUE:
						case CPARSE.CPARSE_ANIMATION:
							return(ControlParser ());
							break;
						}
						Idx_ += tokenlen_;
						tokenlen_ = 0;
						Section = SECTION.SECTION_FINDTOKEN;
					} else {
						Section = SECTION.SECTION_FINDTOKEN;
						Idx_ += tokenlen_;
						tokenlen_ = 0;
					}
					break;
				}
			}
			return(null);
#endif
			throw new NotImplementedException ();
		}

		public C_Window ParseWindow (string filename)
		{
#if TODO
			bool Done = false, Comment = false, Found = false, InString = false;//!,Finished=0;;
			long TokenID = 0;
			SECTION Section = 0;
			TOKEN TokenType;

			if (LoadScript (filename) == false)
				return null;

			Idx_ = 0;
			P_Idx_ = 0;
			tokenlen_ = 0;

			Done = 0;
			Comment = 0;
			InString = 0;
			Section = SECTION.SECTION_FINDTOKEN;
			TokenType = TOKEN.TOKEN_NOTHING;

			while (!Done) {
				switch (Section) {
				case SECTION.SECTION_FINDTOKEN:
			// Look for token starting with '['
					Found = false;
					while (!Found && !Done) {
						switch (script_ [Idx_]) {
						case '[':
							if (!Comment && !InString) {
								Found = true;
								break;
							}
							Idx_++;
							break;
						case '"':
							InString = 1 - InString;
							Idx_++;
							break;
						case '#':
							Comment = true;
							Idx_++;
							break;
						case 0x0a:
						case 0x0d:
							Comment = 0;
							Idx_++;
							break;
						default:
							Idx_++;
						}
						if ((Idx_) >= scriptlen_)
							Done = true;
					}
							
					tokenlen_ = 0;
					while (script_[Idx_+tokenlen_] != ']' && (Idx_+tokenlen_) < scriptlen_)
						tokenlen_++;
					tokenlen_++;

					if ((Idx_ + tokenlen_) >= scriptlen_) {
						Done = true;
						break;
					}

					if (Found == 1)
						Section = SECTION.SECTION_PROCESSTOKEN;
					break;
				case SECTION.SECTION_PROCESSTOKEN:
					TokenID = FindToken (&script_ [Idx_]);
					if (TokenID) {
						switch (TokenID) {
						case CPARSE.CPARSE_WINDOW:
							return(WindowParser ());
							break;
						default:
							Idx_ += tokenlen_;
							tokenlen_ = 0;
							Section = SECTION.SECTION_FINDTOKEN;
							break;
						}
					} else {
						Section = SECTION.SECTION_FINDTOKEN;
						Idx_ += tokenlen_;
						tokenlen_ = 0;
					}
					break;
				}
			}
			return(null);
#endif
			throw new NotImplementedException ();
		}

		public C_Image ParseImage (string filename)
		{
#if TODO
			long Done = 0, Comment = 0, Found = 0, InString = 0, Finished = false;
			
			long TokenID = 0, Section = 0, TokenType = 0;
			long ImageID = 0;

			Idx_ = 0;
			P_Idx_ = 0;
			tokenlen_ = 0;

			if (Image_ == null) {
				Image_ = new C_Image ();
				Image_.Setup ();
			}
			if (Anim_ == null) {
				Anim_ = new C_Animation ();
				Anim_.Setup ();
			}

			if (LoadScript (filename) == false)
				return(null);

			Done = 0;
			Comment = 0;
			InString = 0;
			Section = SECTION.SECTION_FINDTOKEN;
			TokenType = TOKEN.TOKEN_NOTHING;
	
			while (!Done) {
				switch (Section) {
				case SECTION.SECTION_FINDTOKEN:
			// Look for token starting with '['
					Found = false;
					while (!Found && !Done) {
						switch (script_ [Idx_]) {
						case '[':
							if (!Comment && !InString) {
								Found = true;
								break;
							}
							Idx_++;
							break;
						case '"':
							InString = 1 - InString;
							Idx_++;
							break;
						case '#':
							Comment = true;
							Idx_++;
							break;
						case 0x0a:
						case 0x0d:
							Comment = 0;
							Idx_++;
							break;
						default:
							Idx_++;
						}
						if ((Idx_) >= scriptlen_)
							Done = true;
					}
							
					tokenlen_ = 0;
					while (script_[Idx_+tokenlen_] != ']' && (Idx_+tokenlen_) < scriptlen_)
						tokenlen_++;
					tokenlen_++;

					if ((Idx_ + tokenlen_) >= scriptlen_) {
						Done = true;
						break;
					}

					if (Found == 1)
						Section = SECTION.SECTION_PROCESSTOKEN;
					break;
				case SECTION.SECTION_PROCESSTOKEN:
					TokenID = Image_.LocalFind (&script_ [Idx_]);
					if (TokenID) {
						Section = SECTION.SECTION_FINDPARAMS;
						Idx_ += tokenlen_;
						tokenlen_ = 0;
						TokenType = 1;
						break;
					}
					TokenID = Anim_.LocalFind (&script_ [Idx_]);
					if (TokenID) {
						Section = SECTION.SECTION_FINDPARAMS;
						Idx_ += tokenlen_;
						tokenlen_ = 0;
						TokenType = 2;
						break;
					}
					TokenType = 0;
					Section = SECTION.SECTION_FINDTOKEN;
					Idx_ += tokenlen_;
					tokenlen_ = 0;
					break;
				case SECTION.SECTION_FINDPARAMS:
					P_Idx_ = 0; // start with 0 parameters

				// Repeat until token char '[' found (or EOF)
					Finished = false;
					while (!Finished) {
						// Find NON white space
						Found = false;
						while (!Found && !Done && !Finished) {
							switch (script_ [Idx_]) {
							case ' ':
							case ',':
							case 0x09:
							case 0x0a:
							case 0x0d:
								Idx_++;
								break;
							case '[':
								Finished = true;
								Section = SECTION.SECTION_PROCESSPARAMS;
								break;
							default:
								Found = true;
								break;
							}
							if (Idx_ >= scriptlen_) {
								Finished = true;
								Section = SECTION.SECTION_PROCESSPARAMS;
							}
						}

						if (Found) {
							Found = false;
							if (script_ [Idx_] == '"') { // string
								tokenlen_ = 1;
								str_ = &script_ [Idx_ + tokenlen_];
								// Find closing (")
								while (!Found && !Finished) {
									if (script_ [Idx_ + tokenlen_] == '"')
										Found = true;
									else {
										if ((Idx_ + tokenlen_) >= scriptlen_) {
											Finished = true;
											Section = SECTION.SECTION_PROCESSPARAMS;
										} else
											tokenlen_++;
									}
								}
								if (Found)
									script_ [Idx_ + tokenlen_] = 0; // make null terminated string
								Idx_ += tokenlen_ + 1;
								tokenlen_ = 0;
							} else if (isdigit (script_ [Idx_]) || script_ [Idx_] == '-') { // Number
								// find white space
								Found = false;
								tokenlen_ = 1;
								while (!Found) {
									switch (script_ [Idx_ + tokenlen_]) {
									case ' ':
									case ',':
									case 0x09:
									case 0x0a:
									case 0x0d:
										Found = true;
										break;
									default:
										tokenlen_++;
										break;
									}
									if ((Idx_ + tokenlen_) >= scriptlen_) {
										Found = true;
										Finished = true;
										Section = SECTION.SECTION_PROCESSPARAMS;
									}
								}
								if (Found && P_Idx_ < PARSE_MAX_PARAMS)
									P_ [P_Idx_++] = atol (&script_ [Idx_]);

								Idx_ += tokenlen_;
								tokenlen_ = 0;
							} else {
								// Look for ID in tables
								// Look for white space
								Found = false;
								tokenlen_ = 0;
								while (!Found && !Finished) {
									switch (script_ [Idx_ + tokenlen_]) {
									case ' ':
									case ',':
									case 0x09:
									case 0x0a:
									case 0x0d:
										Found = true;
										break;
									default:
										tokenlen_++;
										break;
									}
									if ((Idx_ + tokenlen_) >= scriptlen_) {
										Finished = true;
										Found = true;
										Section = SECTION.SECTION_PROCESSPARAMS;
									}
								}
								if (Found && P_Idx_ < PARSE_MAX_PARAMS) {
									script_ [Idx_ + tokenlen_] = 0;
									ImageID = FindID (&script_ [Idx_]);
									if (ImageID == -1)
										ImageID = AddNewID (&script_ [Idx_], _START_BASE_ID_);
									P_ [P_Idx_++] = ImageID;
								}
								Idx_ += tokenlen_ + 1;
								tokenlen_ = 0;
							}
						}
					}
					break;
				case SECTION.SECTION_PROCESSPARAMS:
					if (TokenType == 1)
						Image_.LocalFunction (static_cast<short> (TokenID), P_, str_, Handler_);
					else if (TokenType == 2)
						Anim_.LocalFunction (static_cast<short> (TokenID), P_, str_, Handler_);
					TokenType = 0;
					P_Idx_ = 0;
					P_ [0] = 0;
					P_ [1] = 0;
					P_ [2] = 0;
					P_ [3] = 0;
					P_ [4] = 0;
					P_ [5] = 0;
					P_ [6] = 0;
					P_ [7] = 0;
					str_ = null;
					Section = SECTION.SECTION_FINDTOKEN;
					break;
				}
			}
			return(Image_);
#endif
			throw new NotImplementedException ();
		}

		public C_Sound ParseSound (string filename)
		{
#if TODO
			bool Done = 0, Comment = 0, Found = 0, InString = 0, Finished = false;
			
			long TokenID = 0, Section = 0, TokenType = 0;
			long SoundID = 0;

			Idx_ = 0;
			P_Idx_ = 0;
			tokenlen_ = 0;

			if (Sound_ == null) {
				Sound_ = new C_Sound ();
				Sound_.Setup ();
			}

			if (LoadScript (filename) == false)
				return(null);

			Done = 0;
			Comment = 0;
			InString = 0;
			Section = SECTION.SECTION_FINDTOKEN;
			TokenType = TOKEN.TOKEN_NOTHING;
	
			while (!Done) {
				switch (Section) {
				case SECTION.SECTION_FINDTOKEN:
			// Look for token starting with '['
					Found = false;
					while (!Found && !Done) {
						switch (script_ [Idx_]) {
						case '[':
							if (!Comment && !InString) {
								Found = true;
								break;
							}
							Idx_++;
							break;
						case '"':
							InString = 1 - InString;
							Idx_++;
							break;
						case '#':
							Comment = true;
							Idx_++;
							break;
						case 0x0a:
						case 0x0d:
							Comment = 0;
							Idx_++;
							break;
						default:
							Idx_++;
						}
						if ((Idx_) >= scriptlen_)
							Done = true;
					}
							
					tokenlen_ = 0;
					while (script_[Idx_+tokenlen_] != ']' && (Idx_+tokenlen_) < scriptlen_)
						tokenlen_++;
					tokenlen_++;

					if ((Idx_ + tokenlen_) >= scriptlen_) {
						Done = true;
						break;
					}

					if (Found == 1)
						Section = SECTION.SECTION_PROCESSTOKEN;
					break;
				case SECTION.SECTION_PROCESSTOKEN:
					TokenID = Sound_.LocalFind (&script_ [Idx_]);
					if (TokenID) {
						Section = SECTION.SECTION_FINDPARAMS;
						Idx_ += tokenlen_;
						tokenlen_ = 0;
					} else {
						Section = SECTION.SECTION_FINDTOKEN;
						Idx_ += tokenlen_;
						tokenlen_ = 0;
					}
					break;
				case SECTION.SECTION_FINDPARAMS:
					P_Idx_ = 0; // start with 0 parameters

				// Repeat until token char '[' found (or EOF)
					Finished = false;
					while (!Finished) {
						// Find NON white space
						Found = false;
						while (!Found && !Done && !Finished) {
							switch (script_ [Idx_]) {
							case ' ':
							case ',':
							case 0x09:
							case 0x0a:
							case 0x0d:
								Idx_++;
								break;
							case '[':
								Finished = true;
								Section = SECTION.SECTION_PROCESSPARAMS;
								break;
							default:
								Found = true;
								break;
							}
							if (Idx_ >= scriptlen_) {
								Finished = true;
								Section = SECTION.SECTION_PROCESSPARAMS;
							}
						}

						if (Found) {
							Found = false;
							if (script_ [Idx_] == '"') { // string
								tokenlen_ = 1;
								str_ = &script_ [Idx_ + tokenlen_];
								// Find closing (")
								while (!Found && !Finished) {
									if (script_ [Idx_ + tokenlen_] == '"')
										Found = true;
									else {
										if ((Idx_ + tokenlen_) >= scriptlen_) {
											Finished = true;
											Section = SECTION.SECTION_PROCESSPARAMS;
										} else
											tokenlen_++;
									}
								}
								if (Found)
									script_ [Idx_ + tokenlen_] = 0; // make null terminated string
								Idx_ += tokenlen_ + 1;
								tokenlen_ = 0;
							} else if (isdigit (script_ [Idx_]) || script_ [Idx_] == '-') { // Number
								// find white space
								Found = false;
								tokenlen_ = 1;
								while (!Found) {
									switch (script_ [Idx_ + tokenlen_]) {
									case ' ':
									case ',':
									case 0x09:
									case 0x0a:
									case 0x0d:
										Found = true;
										break;
									default:
										tokenlen_++;
										break;
									}
									if ((Idx_ + tokenlen_) >= scriptlen_) {
										Found = true;
										Finished = true;
										Section = SECTION.SECTION_PROCESSPARAMS;
									}
								}
								if (Found && P_Idx_ < PARSE_MAX_PARAMS)
									P_ [P_Idx_++] = atol (&script_ [Idx_]);

								Idx_ += tokenlen_;
								tokenlen_ = 0;
							} else {
								// Look for ID in tables
								// Look for white space
								Found = false;
								tokenlen_ = 0;
								while (!Found && !Finished) {
									switch (script_ [Idx_ + tokenlen_]) {
									case ' ':
									case ',':
									case 0x09:
									case 0x0a:
									case 0x0d:
										Found = true;
										break;
									default:
										tokenlen_++;
										break;
									}
									if ((Idx_ + tokenlen_) >= scriptlen_) {
										Finished = true;
										Found = true;
										Section = SECTION.SECTION_PROCESSPARAMS;
									}
								}
								if (Found && P_Idx_ < PARSE_MAX_PARAMS) {
									script_ [Idx_ + tokenlen_] = 0;
									SoundID = FindID (&script_ [Idx_]);
									if (SoundID == -1)
										SoundID = AddNewID (&script_ [Idx_], 1);
									P_ [P_Idx_++] = SoundID;
								}
								Idx_ += tokenlen_ + 1;
								tokenlen_ = 0;
							}
						}
					}
					break;
				case SECTION.SECTION_PROCESSPARAMS:
					Sound_.LocalFunction (static_cast<short> (TokenID), P_, str_, Handler_);
					P_Idx_ = 0;
					P_ [0] = 0;
					P_ [1] = 0;
					P_ [2] = 0;
					P_ [3] = 0;
					P_ [4] = 0;
					P_ [5] = 0;
					P_ [6] = 0;
					P_ [7] = 0;
					str_ = null;
					Section = SECTION.SECTION_FINDTOKEN;
					break;
				}
			}
			return(Sound_);
#endif
			throw new NotImplementedException ();
		}

		public C_String ParseString (string filename)
		{
#if TODO
			int InString = 0;
			short Done = 0, Comment = 0, Found = 0, Finished = false;
			short TokenID = 0, Section = 0, TokenType = 0;
			long StringID = 0;
			BOOL AddFlag = false;
			byte buffer = new byte[80];

			Idx_ = 0;
			P_Idx_ = 0;
			tokenlen_ = 0;

			if (String_ == null) {
				String_ = new C_String ();
				String_.Setup (TXT_LAST_TEXT_ID);
			}

			if (LoadScript (filename) == false)
				return(null);

			Done = 0;
			Comment = 0;
			InString = 0;
			Section = SECTION.SECTION_FINDTOKEN;
			TokenType = TOKEN.TOKEN_NOTHING;
	
			while (!Done) {
				switch (Section) {
				case SECTION.SECTION_FINDTOKEN:
			// Look for token starting with '['
					Found = false;
					while (!Found && !Done) {
						switch (script_ [Idx_]) {
						case '[':
							if (!Comment && !InString) {
								Found = true;
								break;
							}
							Idx_++;
							break;
						case '"':
							InString = 1 - InString;
							Idx_++;
							break;
						case '#':
							Comment = true;
							Idx_++;
							break;
						case 0x0a:
						case 0x0d:
							Comment = 0;
							Idx_++;
							break;
						default:
							Idx_++;
						}
						if ((Idx_) >= scriptlen_)
							Done = true;
					}
							
					tokenlen_ = 0;
					while (script_[Idx_+tokenlen_] != ']' && (Idx_+tokenlen_) < scriptlen_)
						tokenlen_++;
					tokenlen_++;

					if ((Idx_ + tokenlen_) >= scriptlen_) {
						Done = true;
						break;
					}

					if (Found == 1)
						Section = SECTION.SECTION_PROCESSTOKEN;
					break;
				case SECTION.SECTION_PROCESSTOKEN:
					TokenID = String_.LocalFind (&script_ [Idx_]);
					if (TokenID) {
						Section = SECTION.SECTION_FINDPARAMS;
						Idx_ += tokenlen_;
						tokenlen_ = 0;
					} else {
						Section = SECTION.SECTION_FINDTOKEN;
						Idx_ += tokenlen_;
						tokenlen_ = 0;
					}
					break;
				case SECTION.SECTION_FINDPARAMS:
					P_Idx_ = 0; // start with 0 parameters

				// Repeat until token char '[' found (or EOF)
					Finished = false;
					while (!Finished) {
						// Find NON white space
						Found = false;
						while (!Found && !Done && !Finished) {
							switch (script_ [Idx_]) {
							case ' ':
							case ',':
							case 0x09:
							case 0x0a:
							case 0x0d:
								Idx_++;
								break;
							case '[':
								Finished = true;
								Section = SECTION.SECTION_PROCESSPARAMS;
								break;
							default:
								Found = true;
								break;
							}
							if (Idx_ >= scriptlen_) {
								Finished = true;
								Section = SECTION.SECTION_PROCESSPARAMS;
							}
						}

						if (Found) {
							Found = false;
							if (script_ [Idx_] == '"') { // string
								tokenlen_ = 1;
								str_ = &script_ [Idx_ + tokenlen_];
								// Find closing (")
								while (!Found && !Finished) {
									if (script_ [Idx_ + tokenlen_] == '"')
										Found = true;
									else {
										if ((Idx_ + tokenlen_) >= scriptlen_) {
											Finished = true;
											Section = SECTION.SECTION_PROCESSPARAMS;
										} else
											tokenlen_++;
									}
								}
								if (Found)
									script_ [Idx_ + tokenlen_] = 0; // make null terminated string
								Idx_ += tokenlen_ + 1;
								tokenlen_ = 0;
							} else if (isdigit (script_ [Idx_]) || script_ [Idx_] == '-') { // Number
								// find white space
								Found = false;
								tokenlen_ = 1;
								while (!Found) {
									switch (script_ [Idx_ + tokenlen_]) {
									case ' ':
									case ',':
									case 0x09:
									case 0x0a:
									case 0x0d:
										Found = true;
										break;
									default:
										tokenlen_++;
										break;
									}
									if ((Idx_ + tokenlen_) >= scriptlen_) {
										Found = true;
										Finished = true;
										Section = SECTION.SECTION_PROCESSPARAMS;
									}
								}
								if (Found && P_Idx_ < PARSE_MAX_PARAMS)
									P_ [P_Idx_++] = atol (&script_ [Idx_]);

								Idx_ += tokenlen_;
								tokenlen_ = 0;
							} else {
								// Look for ID in tables
								// Look for white space
								Found = false;
								tokenlen_ = 0;
								while (!Found && !Finished) {
									switch (script_ [Idx_ + tokenlen_]) {
									case ' ':
									case ',':
									case 0x09:
									case 0x0a:
									case 0x0d:
										Found = true;
										break;
									default:
										tokenlen_++;
										break;
									}
									if ((Idx_ + tokenlen_) >= scriptlen_) {
										Finished = true;
										Found = true;
										Section = SECTION.SECTION_PROCESSPARAMS;
									}
								}
								if (Found && P_Idx_ < PARSE_MAX_PARAMS) {
									script_ [Idx_ + tokenlen_] = 0;
									StringID = FindID (&script_ [Idx_]);
									if (StringID == -1) {
										_tcscpy (buffer, &script_ [Idx_]);
										StringID = -2;
									}
									P_ [P_Idx_++] = StringID;
								}
								Idx_ += tokenlen_ + 1;
								tokenlen_ = 0;
							}
						}
					}
					break;
				case SECTION.SECTION_PROCESSPARAMS:
					if (P_ [0] == -2)
						AddFlag = true;
					else
						AddFlag = false;
					String_.LocalFunction (static_cast<short> (TokenID), P_, str_, Handler_);
					if (AddFlag)
						TokenOrder_.AddTextID (String_.GetLastID (), buffer);
					P_Idx_ = 0;
					P_ [0] = 0;
					P_ [1] = 0;
					P_ [2] = 0;
					P_ [3] = 0;
					P_ [4] = 0;
					P_ [5] = 0;
					P_ [6] = 0;
					P_ [7] = 0;
					str_ = null;
					Section = SECTION.SECTION_FINDTOKEN;
					break;
				}
			}
			return(String_);
#endif
			throw new NotImplementedException ();
		}

		public C_Movie ParseMovie (string filename)
		{
#if TODO
			int InString = 0;
			bool Done = 0, Comment = 0, Found = 0, Finished = false;
			
			short TokenID = 0, Section = 0, TokenType = 0;
			long MovieID = 0;

			Idx_ = 0;
			P_Idx_ = 0;
			tokenlen_ = 0;

			if (Movie_ == null) {
				Movie_ = new C_Movie ();
				Movie_.Setup ();
			}

			if (LoadScript (filename) == false)
				return(null);

			Done = 0;
			Comment = 0;
			InString = 0;
			Section = SECTION.SECTION_FINDTOKEN;
			TokenType = TOKEN.TOKEN_NOTHING;
	
			while (!Done) {
				switch (Section) {
				case SECTION.SECTION_FINDTOKEN:
			// Look for token starting with '['
					Found = false;
					while (!Found && !Done) {
						switch (script_ [Idx_]) {
						case '[':
							if (!Comment && !InString) {
								Found = true;
								break;
							}
							Idx_++;
							break;
						case '"':
							InString = 1 - InString;
							Idx_++;
							break;
						case '#':
							Comment = true;
							Idx_++;
							break;
						case 0x0a:
						case 0x0d:
							Comment = 0;
							Idx_++;
							break;
						default:
							Idx_++;
						}
						if ((Idx_) >= scriptlen_)
							Done = true;
					}
							
					tokenlen_ = 0;
					while (script_[Idx_+tokenlen_] != ']' && (Idx_+tokenlen_) < scriptlen_)
						tokenlen_++;
					tokenlen_++;

					if ((Idx_ + tokenlen_) >= scriptlen_) {
						Done = true;
						break;
					}

					if (Found == 1)
						Section = SECTION.SECTION_PROCESSTOKEN;
					break;
				case SECTION.SECTION_PROCESSTOKEN:
					TokenID = Movie_.LocalFind (&script_ [Idx_]);
					if (TokenID) {
						Section = SECTION.SECTION_FINDPARAMS;
						Idx_ += tokenlen_;
						tokenlen_ = 0;
					} else {
						Section = SECTION.SECTION_FINDTOKEN;
						Idx_ += tokenlen_;
						tokenlen_ = 0;
					}
					break;
				case SECTION.SECTION_FINDPARAMS:
					P_Idx_ = 0; // start with 0 parameters

				// Repeat until token char '[' found (or EOF)
					Finished = false;
					while (!Finished) {
						// Find NON white space
						Found = false;
						while (!Found && !Done && !Finished) {
							switch (script_ [Idx_]) {
							case ' ':
							case ',':
							case 0x09:
							case 0x0a:
							case 0x0d:
								Idx_++;
								break;
							case '[':
								Finished = true;
								Section = SECTION.SECTION_PROCESSPARAMS;
								break;
							default:
								Found = true;
								break;
							}
							if (Idx_ >= scriptlen_) {
								Finished = true;
								Section = SECTION.SECTION_PROCESSPARAMS;
							}
						}

						if (Found) {
							Found = false;
							if (script_ [Idx_] == '"') { // string
								tokenlen_ = 1;
								str_ = &script_ [Idx_ + tokenlen_];
								// Find closing (")
								while (!Found && !Finished) {
									if (script_ [Idx_ + tokenlen_] == '"')
										Found = true;
									else {
										if ((Idx_ + tokenlen_) >= scriptlen_) {
											Finished = true;
											Section = SECTION.SECTION_PROCESSPARAMS;
										} else
											tokenlen_++;
									}
								}
								if (Found)
									script_ [Idx_ + tokenlen_] = 0; // make null terminated string
								Idx_ += tokenlen_ + 1;
								tokenlen_ = 0;
							} else if (isdigit (script_ [Idx_]) || script_ [Idx_] == '-') { // Number
								// find white space
								Found = false;
								tokenlen_ = 1;
								while (!Found) {
									switch (script_ [Idx_ + tokenlen_]) {
									case ' ':
									case ',':
									case 0x09:
									case 0x0a:
									case 0x0d:
										Found = true;
										break;
									default:
										tokenlen_++;
										break;
									}
									if ((Idx_ + tokenlen_) >= scriptlen_) {
										Found = true;
										Finished = true;
										Section = SECTION.SECTION_PROCESSPARAMS;
									}
								}
								if (Found && P_Idx_ < PARSE_MAX_PARAMS)
									P_ [P_Idx_++] = atol (&script_ [Idx_]);

								Idx_ += tokenlen_;
								tokenlen_ = 0;
							} else {
								// Look for ID in tables
								// Look for white space
								Found = false;
								tokenlen_ = 0;
								while (!Found && !Finished) {
									switch (script_ [Idx_ + tokenlen_]) {
									case ' ':
									case ',':
									case 0x09:
									case 0x0a:
									case 0x0d:
										Found = true;
										break;
									default:
										tokenlen_++;
										break;
									}
									if ((Idx_ + tokenlen_) >= scriptlen_) {
										Finished = true;
										Found = true;
										Section = SECTION.SECTION_PROCESSPARAMS;
									}
								}
								if (Found && P_Idx_ < PARSE_MAX_PARAMS) {
									script_ [Idx_ + tokenlen_] = 0;
									MovieID = FindID (&script_ [Idx_]);
									if (MovieID == -1)
										MovieID = AddNewID (&script_ [Idx_], 1);
									P_ [P_Idx_++] = MovieID;
								}
								Idx_ += tokenlen_ + 1;
								tokenlen_ = 0;
							}
						}
					}
					break;
				case SECTION.SECTION_PROCESSPARAMS:
					Movie_.LocalFunction (static_cast<short> (TokenID), P_, str_, Handler_);
					P_Idx_ = 0;
					P_ [0] = 0;
					P_ [1] = 0;
					P_ [2] = 0;
					P_ [3] = 0;
					P_ [4] = 0;
					P_ [5] = 0;
					P_ [6] = 0;
					P_ [7] = 0;
					str_ = null;
					Section = SECTION.SECTION_FINDTOKEN;
					break;
				}
			}
			return(Movie_);
#endif
			throw new NotImplementedException ();
		}

		public C_Font ParseFont (string filename)
		{

			bool Done = false, Comment = false, Found = false, InString = false, Finished = false;
			
			long TokenID = 0, Section = 0, TokenType = 0;
			long FontID = 0, NewID = 0;
			LOGFONT logfont = new LOGFONT();

			Idx_ = 0;
			P_Idx_ = 0;
			tokenlen_ = 0;

			if (Font_ == null) {
				Font_ = new C_Font ();
				Font_.Setup (Handler_);
			}

			//if (LoadScript (filename) == false)
			//	return(null);
			FileStream script = OpenScript (filename);
			try {
				using (StreamReader sr = new StreamReader (script)) {
					string strLine = sr.ReadLine ();
					while (strLine != null) {
						strLine = sr.ReadLine ();
						if (!string.IsNullOrWhiteSpace (strLine)) {
							strLine = strLine.Trim ();
							if (!strLine.StartsWith ("#")) {
								List<string> tokens = strLine.SplitWords ();
								Console.WriteLine ("Para");
								TokenID = Font_.FontFind (tokens [0]);
								FontID = FindID (tokens [1]);
								if (FontID == -1) {
									FontID = AddNewID (tokens [1], 1);
								}
								P_ [0] = FontID;
								Font_.FontFunction ((C_Font.CFNT)TokenID, P_, tokens [2], ref logfont, ref NewID);
								//P_ [P_Idx_++] = atol (&script_ [Idx_]);
							}
						}
					}
					sr.Close ();
				}
			} catch (IOException e) {
				Debug.WriteLine ("An IO exception has been thrown!");
				Debug.WriteLine (e.ToString ());
				return null;
			}
#if TODO
			Done = 0;
			Comment = 0;
			InString = 0;
			Section = SECTION.SECTION_FINDTOKEN;
			TokenType = TOKEN.TOKEN_NOTHING;

			memset (&logfont, 0, sizeof(LOGFONT));
	
			while (!Done) {
				switch (Section) {
				case SECTION.SECTION_FINDTOKEN:
			// Look for token starting with '['
					Found = false;
					while (!Found && !Done) {
						switch (script_ [Idx_]) {
						case '[':
							if (!Comment && !InString) {
								Found = true;
								break;
							}
							Idx_++;
							break;
						case '"':
							InString = 1 - InString;
							Idx_++;
							break;
						case '#':
							Comment = true;
							Idx_++;
							break;
						case 0x0a:
						case 0x0d:
							Comment = 0;
							Idx_++;
							break;
						default:
							Idx_++;
						}
						if ((Idx_) >= scriptlen_)
							Done = true;
					}
							
					tokenlen_ = 0;
					while (script_[Idx_+tokenlen_] != ']' && (Idx_+tokenlen_) < scriptlen_)
						tokenlen_++;
					tokenlen_++;

					if ((Idx_ + tokenlen_) >= scriptlen_) {
						Done = true;
						break;
					}

					if (Found == 1)
						Section = SECTION.SECTION_PROCESSTOKEN;
					break;
				case SECTION.SECTION_PROCESSTOKEN:
					TokenID = Font_.FontFind (&script_ [Idx_]);
					if (TokenID) {
						Section = SECTION.SECTION_FINDPARAMS;
						Idx_ += tokenlen_;
						tokenlen_ = 0;
					} else {
						Section = SECTION.SECTION_FINDTOKEN;
						Idx_ += tokenlen_;
						tokenlen_ = 0;
					}
					break;
				case SECTION.SECTION_FINDPARAMS:
					P_Idx_ = 0; // start with 0 parameters

				// Repeat until token char '[' found (or EOF)
					Finished = false;
					while (!Finished) {
						// Find NON white space
						Found = false;
						while (!Found && !Done && !Finished) {
							switch (script_ [Idx_]) {
							case ' ':
							case ',':
							case 0x09:
							case 0x0a:
							case 0x0d:
								Idx_++;
								break;
							case '[':
								Finished = true;
								Section = SECTION.SECTION_PROCESSPARAMS;
								break;
							default:
								Found = true;
								break;
							}
							if (Idx_ >= scriptlen_) {
								Finished = true;
								Section = SECTION.SECTION_PROCESSPARAMS;
							}
						}

						if (Found) {
							Found = false;
							if (script_ [Idx_] == '"') { // string
								tokenlen_ = 1;
								str_ = &script_ [Idx_ + tokenlen_];
								// Find closing (")
								while (!Found && !Finished) {
									if (script_ [Idx_ + tokenlen_] == '"')
										Found = true;
									else {
										if ((Idx_ + tokenlen_) >= scriptlen_) {
											Finished = true;
											Section = SECTION.SECTION_PROCESSPARAMS;
										} else
											tokenlen_++;
									}
								}
								if (Found)
									script_ [Idx_ + tokenlen_] = 0; // make null terminated string
								Idx_ += tokenlen_ + 1;
								tokenlen_ = 0;
							} else if (isdigit (script_ [Idx_]) || script_ [Idx_] == '-') { // Number
								// find white space
								Found = false;
								tokenlen_ = 1;
								while (!Found) {
									switch (script_ [Idx_ + tokenlen_]) {
									case ' ':
									case ',':
									case 0x09:
									case 0x0a:
									case 0x0d:
										Found = true;
										break;
									default:
										tokenlen_++;
										break;
									}
									if ((Idx_ + tokenlen_) >= scriptlen_) {
										Found = true;
										Finished = true;
										Section = SECTION.SECTION_PROCESSPARAMS;
									}
								}
								if (Found && P_Idx_ < PARSE_MAX_PARAMS)
									P_ [P_Idx_++] = atol (&script_ [Idx_]);

								Idx_ += tokenlen_;
								tokenlen_ = 0;
							} else {
								// Look for ID in tables
								// Look for white space
								Found = false;
								tokenlen_ = 0;
								while (!Found && !Finished) {
									switch (script_ [Idx_ + tokenlen_]) {
									case ' ':
									case ',':
									case 0x09:
									case 0x0a:
									case 0x0d:
										Found = true;
										break;
									default:
										tokenlen_++;
										break;
									}
									if ((Idx_ + tokenlen_) >= scriptlen_) {
										Finished = true;
										Found = true;
										Section = SECTION.SECTION_PROCESSPARAMS;
									}
								}
								if (Found && P_Idx_ < PARSE_MAX_PARAMS) {
									script_ [Idx_ + tokenlen_] = 0;
									FontID = FindID (&script_ [Idx_]);
									if (FontID == -1) {
										if (FontID == -1)
											FontID = AddNewID (&script_ [Idx_], 1);
									}
									P_ [P_Idx_++] = FontID;
								}
								Idx_ += tokenlen_ + 1;
								tokenlen_ = 0;
							}
						}
					}
					break;
				case SECTION.SECTION_PROCESSPARAMS:
					Font_.FontFunction (static_cast<short> (TokenID), P_, str_, &logfont, &NewID);
					P_Idx_ = 0;
					P_ [0] = 0;
					P_ [1] = 0;
					P_ [2] = 0;
					P_ [3] = 0;
					P_ [4] = 0;
					P_ [5] = 0;
					P_ [6] = 0;
					P_ [7] = 0;
					str_ = null;
					Section = SECTION.SECTION_FINDTOKEN;
					break;
				}
			}
			return(Font_);
#endif
			return Font_;
		}

		public C_Base ParsePopupMenu (string filename)
		{
#if TODO
			//short Finished=0;
			bool Done = false, Comment = false, Found = false;
			SECTION Section = 0;
			TOKEN TokenType;
			bool InString = false;
			long TokenID = 0;


			if (Popup_ == null) {
				Popup_ = new C_PopupMgr ();
				Popup_->Setup (Handler_);
			}

			if (LoadScript (filename) == false)
				return(false);

			Idx_ = 0;
			P_Idx_ = 0;
			tokenlen_ = 0;

			InString = 0;
			Section = SECTION.SECTION_FINDTOKEN;
			TokenType = TOKEN.TOKEN_NOTHING;

			while (!Done) {
				switch (Section) {
				case SECTION.SECTION_FINDTOKEN:
			// Look for token starting with '['
					Found = false;
					while (!Found && !Done) {
						switch (script_ [Idx_]) {
						case '[':
							if (!Comment && !InString) {
								Found = true;
								break;
							}
							Idx_++;
							break;
						case '"':
							InString = 1 - InString;
							Idx_++;
							break;
						case '#':
							Comment = true;
							Idx_++;
							break;
						case 0x0a:
						case 0x0d:
							Comment = 0;
							Idx_++;
							break;
						default:
							Idx_++;
						}
						if (Idx_ >= scriptlen_)
							Done = true;
					}
							
					tokenlen_ = 0;
					while (script_[Idx_+tokenlen_] != ']' && (Idx_+tokenlen_) < scriptlen_)
						tokenlen_++;
					tokenlen_++;

					if ((Idx_ + tokenlen_) >= scriptlen_) {
						Done = true;
						break;
					}

					if (Found == true)
						Section = SECTION.SECTION_PROCESSTOKEN;
					break;
				case SECTION.SECTION_PROCESSTOKEN:
					TokenID = FindToken (&script_ [Idx_]);
					if (TokenID != 0) {
						switch ((CPARSE)TokenID) {
						case CPARSE.CPARSE_POPUP:
							return(PopupParser ());
							break;
						}
						Idx_ += tokenlen_;
						tokenlen_ = 0;
						Section = SECTION.SECTION_FINDTOKEN;
					} else {
						Section = SECTION.SECTION_FINDTOKEN;
						Idx_ += tokenlen_;
						tokenlen_ = 0;
					}
					break;
				}
			}
			return(null);
#endif
			throw new NotImplementedException ();
		}

		public C_Hash GetTokenHash ()
		{
			return(TokenOrder_);
		}

		public C_Hash GetIDHash ()
		{
			return(IDOrder_);
		}

		public long GetFirstWindowLoaded ()
		{
			WinIndex_ = 0;
			if (WinIndex_ < WinLoaded_)
				return(WindowList_ [WinIndex_]);
			else
				return(0);
		}

		public long GetNextWindowLoaded ()
		{
			WinIndex_++;
			if (WinIndex_ < WinLoaded_)
				return(WindowList_ [WinIndex_]);
			else
				return(0);
		}

		public long AddNewID (string label, long id)
		{
#if TODO			
			return(TokenOrder_.AddText (label));
#endif
			throw new NotImplementedException ();
		}

		public void LogError (string str)
		{
#if TODO
			if (g_bLogUiErrors) {
				if (Perror_ != null)
					fprintf (Perror_, "%s\n", str);
			}
#endif
			throw new ApplicationException (str);
		}
	
		C_Hash TokenErrorList = null;
		FileStream errorfp;
		
		//TODO Defined somewhere
		public static string FalconUIArtDirectory = @"../../../data";
		public static string FalconUIArtThrDirectory = @"../../../data";
		public static string FalconUISoundDirectory = @"../../../data";
		public static string filebuf;
		private static string[] C_All_Tokens =
		{
			"[NOTHING]",
			"[WINDOW]",
			"[BUTTON]",
			"[TEXT]",
			"[EDITBOX]",
			"[LISTBOX]",
			"[SCROLLBAR]",
			"[TREELIST]",
			"[MAKEFONT]",
			"[IMAGE]",
			"[BITMAP]",
			"[LINE]",
			"[BOX]",
			"[MARQUE]",
			"[ANIMATION]",
			"[LOCATOR]",
			"[SOUND]",
			"[SLIDER]",
			"[POPUPMENU]",
			"[PANNER]",
			"[ANIM]",
			"[STRINGLIST]",
			"[MOVIE]",
			"[FILL]",
			"[TREELIST]",
			"[CLOCK]",
			"[TILE]"
		};
		private static Dictionary<String, UI95_ENUM> UI95_Table = new Dictionary<String, UI95_ENUM> ()
		{
		//TODO {"NID",						UI95_ENUM.C_DONT_CARE},
			{"C_DONT_CARE",				UI95_ENUM.C_DONT_CARE},
			{"C_STATE_0",				UI95_ENUM.C_STATE_0},
			{"C_STATE_1",				UI95_ENUM.C_STATE_1},
			{"C_STATE_2",				UI95_ENUM.C_STATE_2},
			{"C_STATE_3",				UI95_ENUM.C_STATE_3},
			{"C_STATE_4",				UI95_ENUM.C_STATE_4},
			{"C_STATE_5",				UI95_ENUM.C_STATE_5},
			{"C_STATE_6",				UI95_ENUM.C_STATE_6},
			{"C_STATE_7",				UI95_ENUM.C_STATE_7},
			{"C_STATE_8",				UI95_ENUM.C_STATE_8},
			{"C_STATE_9",				UI95_ENUM.C_STATE_9},
			{"C_STATE_10",				UI95_ENUM.C_STATE_10},
			{"C_STATE_11",				UI95_ENUM.C_STATE_11},
			{"C_STATE_12",				UI95_ENUM.C_STATE_12},
			{"C_STATE_13",				UI95_ENUM.C_STATE_13},
			{"C_STATE_14",				UI95_ENUM.C_STATE_14},
			{"C_STATE_15",				UI95_ENUM.C_STATE_15},
			{"C_STATE_16",				UI95_ENUM.C_STATE_16},
			{"C_STATE_17",				UI95_ENUM.C_STATE_17},
			{"C_STATE_18",				UI95_ENUM.C_STATE_18},
			{"C_STATE_19",				UI95_ENUM.C_STATE_19},
			{"C_STATE_20",				UI95_ENUM.C_STATE_20},
			{"C_STATE_UP",				UI95_ENUM.C_STATE_0},
			{"C_STATE_DOWN",			UI95_ENUM.C_STATE_1},
			{"C_STATE_DISABLED",		UI95_ENUM.C_STATE_DISABLED},
			{"C_STATE_SELECTED",		UI95_ENUM.C_STATE_SELECTED},
			{"C_STATE_MOUSE",			UI95_ENUM.C_STATE_MOUSE},
			{"C_TYPE_NOTHING",			UI95_ENUM.C_TYPE_NOTHING},
			{"C_TYPE_NORMAL",			UI95_ENUM.C_TYPE_NORMAL},
			{"C_TYPE_TOGGLE",			UI95_ENUM.C_TYPE_TOGGLE},
			{"C_TYPE_SELECT",			UI95_ENUM.C_TYPE_SELECT},
			{"C_TYPE_RADIO",			UI95_ENUM.C_TYPE_RADIO},
			{"C_TYPE_CUSTOM",			UI95_ENUM.C_TYPE_CUSTOM},
			{"C_TYPE_SIZEX",			UI95_ENUM.C_TYPE_SIZEX},
			{"C_TYPE_SIZEY",			UI95_ENUM.C_TYPE_SIZEY},
			{"C_TYPE_SIZEXY",			UI95_ENUM.C_TYPE_SIZEXY},
			{"C_TYPE_SIZEW",			UI95_ENUM.C_TYPE_SIZEW},
			{"C_TYPE_SIZEH",			UI95_ENUM.C_TYPE_SIZEH},
			{"C_TYPE_SIZEWH",			UI95_ENUM.C_TYPE_SIZEWH},
			{"C_TYPE_DRAGX",			UI95_ENUM.C_TYPE_DRAGX},
			{"C_TYPE_DRAGY",			UI95_ENUM.C_TYPE_DRAGY},
			{"C_TYPE_DRAGXY",			UI95_ENUM.C_TYPE_DRAGXY},
			{"C_TYPE_TEXT",				UI95_ENUM.C_TYPE_TEXT},
			{"C_TYPE_PASSWORD",			UI95_ENUM.C_TYPE_PASSWORD},
			{"C_TYPE_INTEGER",			UI95_ENUM.C_TYPE_INTEGER},
			{"C_TYPE_FLOAT",			UI95_ENUM.C_TYPE_FLOAT},
			{"C_TYPE_FILENAME",			UI95_ENUM.C_TYPE_FILENAME},
			{"C_TYPE_MENU",				UI95_ENUM.C_TYPE_MENU},
			{"C_TYPE_LEFT",				UI95_ENUM.C_TYPE_LEFT},
			{"C_TYPE_CENTER",			UI95_ENUM.C_TYPE_CENTER},
			{"C_TYPE_RIGHT",			UI95_ENUM.C_TYPE_RIGHT},
			{"C_TYPE_ROOT",				UI95_ENUM.C_TYPE_ROOT},
			{"C_TYPE_INFO",				UI95_ENUM.C_TYPE_INFO},
			{"C_TYPE_ITEM",				UI95_ENUM.C_TYPE_ITEM},
			{"C_TYPE_LMOUSEDOWN",		UI95_ENUM.C_TYPE_LMOUSEDOWN},
			{"C_TYPE_LMOUSEUP",			UI95_ENUM.C_TYPE_LMOUSEUP},
			{"C_TYPE_LMOUSEDBLCLK",		UI95_ENUM.C_TYPE_LMOUSEDBLCLK},
			{"C_TYPE_RMOUSEDOWN",		UI95_ENUM.C_TYPE_RMOUSEDOWN},
			{"C_TYPE_RMOUSEUP",			UI95_ENUM.C_TYPE_RMOUSEUP},
			{"C_TYPE_RMOUSEDBLCLK",		UI95_ENUM.C_TYPE_RMOUSEDBLCLK},
			{"C_TYPE_MOUSEOVER",		UI95_ENUM.C_TYPE_MOUSEOVER},
			{"C_TYPE_MOUSEREPEAT",		UI95_ENUM.C_TYPE_MOUSEREPEAT},
			{"C_TYPE_EXCLUSIVE",		UI95_ENUM.C_TYPE_EXCLUSIVE},
			{"C_TYPE_MOUSEMOVE",		UI95_ENUM.C_TYPE_MOUSEMOVE},
			{"C_TYPE_VERTICAL",			UI95_ENUM.C_TYPE_VERTICAL},
			{"C_TYPE_HORIZONTAL",		UI95_ENUM.C_TYPE_HORIZONTAL},
			{"C_TYPE_LOOP",				UI95_ENUM.C_TYPE_LOOP},
			{"C_TYPE_STOPATEND",		UI95_ENUM.C_TYPE_STOPATEND},
			{"C_TYPE_PINGPONG",			UI95_ENUM.C_TYPE_PINGPONG},
			{"C_TYPE_TIMER",			UI95_ENUM.C_TYPE_TIMER},
			{"C_TYPE_TRANSLUCENT",		UI95_ENUM.C_TYPE_TRANSLUCENT},
			{"C_TYPE_IPADDRESS",		UI95_ENUM.C_TYPE_IPADDRESS}
		};
		private static Dictionary<string, UI95_BITTABLE> UI95_BitTable = new Dictionary<string, UI95_BITTABLE> ()
		{
		//TODO {"null",					UI95_BITTABLE.C_BIT_NOTHING},
			{"C_BIT_NOTHING",			UI95_BITTABLE.C_BIT_NOTHING},
			{"C_BIT_FIXEDSIZE",			UI95_BITTABLE.C_BIT_FIXEDSIZE},
			{"C_BIT_LEADINGZEROS",		UI95_BITTABLE.C_BIT_LEADINGZEROS},
			{"C_BIT_VERTICAL",			UI95_BITTABLE.C_BIT_VERTICAL},
			{"C_BIT_HORIZONTAL",		UI95_BITTABLE.C_BIT_HORIZONTAL},
			{"C_BIT_USEOUTLINE",		UI95_BITTABLE.C_BIT_USEOUTLINE},
			{"C_BIT_LEFT",				UI95_BITTABLE.C_BIT_LEFT},
			{"C_BIT_RIGHT",				UI95_BITTABLE.C_BIT_RIGHT},
			{"C_BIT_TOP",				UI95_BITTABLE.C_BIT_TOP},
			{"C_BIT_BOTTOM",			UI95_BITTABLE.C_BIT_BOTTOM},
			{"C_BIT_HCENTER",			UI95_BITTABLE.C_BIT_HCENTER},
			{"C_BIT_VCENTER",			UI95_BITTABLE.C_BIT_VCENTER},
			{"C_BIT_ENABLED",			UI95_BITTABLE.C_BIT_ENABLED},
			{"C_BIT_DRAGABLE",			UI95_BITTABLE.C_BIT_DRAGABLE},
			{"C_BIT_INVISIBLE",			UI95_BITTABLE.C_BIT_INVISIBLE},
			{"C_BIT_FORCEMOUSEOVER",	UI95_BITTABLE.C_BIT_FORCEMOUSEOVER},
			{"C_BIT_USEBGIMAGE",		UI95_BITTABLE.C_BIT_USEBGIMAGE},
			{"C_BIT_TIMER",				UI95_BITTABLE.C_BIT_TIMER},
			{"C_BIT_ABSOLUTE",			UI95_BITTABLE.C_BIT_ABSOLUTE},
			{"C_BIT_SELECTABLE",		UI95_BITTABLE.C_BIT_SELECTABLE},
			{"C_BIT_OPAQUE",			UI95_BITTABLE.C_BIT_OPAQUE},
			{"C_BIT_CANTMOVE",			UI95_BITTABLE.C_BIT_CANTMOVE},
			{"C_BIT_USELINE",			UI95_BITTABLE.C_BIT_USELINE},
			{"C_BIT_WORDWRAP",			UI95_BITTABLE.C_BIT_WORDWRAP},
			{"C_BIT_REMOVE",			UI95_BITTABLE.C_BIT_REMOVE},
			{"C_BIT_NOCLEANUP",			UI95_BITTABLE.C_BIT_NOCLEANUP},
			{"C_BIT_TRANSLUCENT",		UI95_BITTABLE.C_BIT_TRANSLUCENT},
			{"C_BIT_USEBGFILL",			UI95_BITTABLE.C_BIT_USEBGFILL},
			{"C_BIT_MOUSEOVER",			UI95_BITTABLE.C_BIT_MOUSEOVER},
			{"C_BIT_NOLABEL",			UI95_BITTABLE.C_BIT_NOLABEL}
		};
#if TODO
private static Dictionary<string, ??>  UI95_FontTable[]=
{
	{"false",					false},
	{"true",					true},
	{"FW_DONTCARE",				FW_DONTCARE},
	{"FW_THIN",					FW_THIN},
	{"FW_EXTRALIGHT",			FW_EXTRALIGHT},
	{"FW_ULTRALIGHT",			FW_ULTRALIGHT},
	{"FW_LIGHT",				FW_LIGHT},
	{"FW_NORMAL",				FW_NORMAL},
	{"FW_REGULAR",				FW_REGULAR},
	{"FW_MEDIUM",				FW_MEDIUM},
	{"FW_SEMIBOLD",				FW_SEMIBOLD},
	{"FW_DEMIBOLD",				FW_DEMIBOLD},
	{"FW_BOLD",					FW_BOLD},
	{"FW_EXTRABOLD",			FW_EXTRABOLD},
	{"FW_ULTRABOLD",			FW_ULTRABOLD},
	{"FW_HEAVY",				FW_HEAVY},
	{"FW_BLACK",				FW_BLACK},
	{"ANSI_CHARSET",			ANSI_CHARSET},
	{"DEFAULT_CHARSET",			DEFAULT_CHARSET},
	{"SYMBOL_CHARSET",			SYMBOL_CHARSET},
	{"SHIFTJIS_CHARSET",		SHIFTJIS_CHARSET},
	{"GB2312_CHARSET",			GB2312_CHARSET},
	{"HANGEUL_CHARSET",			HANGEUL_CHARSET},
	{"CHINESEBIG5_CHARSET",		CHINESEBIG5_CHARSET},
	{"OEM_CHARSET",				OEM_CHARSET},
	{"JOHAB_CHARSET",			JOHAB_CHARSET},
	{"HEBREW_CHARSET",			HEBREW_CHARSET},
	{"ARABIC_CHARSET",			ARABIC_CHARSET},
	{"GREEK_CHARSET",			GREEK_CHARSET},
	{"TURKISH_CHARSET",			TURKISH_CHARSET},
	{"THAI_CHARSET",			THAI_CHARSET},
	{"EASTEUROPE_CHARSET",		EASTEUROPE_CHARSET},
	{"RUSSIAN_CHARSET",			RUSSIAN_CHARSET},
	{"MAC_CHARSET",				MAC_CHARSET},
	{"BALTIC_CHARSET",			BALTIC_CHARSET},
	{"OUT_CHARACTER_PRECIS",	OUT_CHARACTER_PRECIS},
	{"OUT_DEFAULT_PRECIS",		OUT_DEFAULT_PRECIS},
	{"OUT_DEVICE_PRECIS",		OUT_DEVICE_PRECIS},
	{"OUT_OUTLINE_PRECIS",		OUT_OUTLINE_PRECIS},
	{"OUT_RASTER_PRECIS",		OUT_RASTER_PRECIS},
	{"OUT_STRING_PRECIS",		OUT_STRING_PRECIS},
	{"OUT_STROKE_PRECIS",		OUT_STROKE_PRECIS},
	{"OUT_TT_ONLY_PRECIS",		OUT_TT_ONLY_PRECIS},
	{"OUT_TT_PRECIS",			OUT_TT_PRECIS},
	{"CLIP_DEFAULT_PRECIS",		CLIP_DEFAULT_PRECIS},
	{"CLIP_CHARACTER_PRECIS",	CLIP_CHARACTER_PRECIS},
	{"CLIP_STROKE_PRECIS",		CLIP_STROKE_PRECIS},
	{"CLIP_MASK",				CLIP_MASK},
	{"CLIP_EMBEDDED",			CLIP_EMBEDDED},
	{"CLIP_LH_ANGLES",			CLIP_LH_ANGLES},
	{"CLIP_TT_ALWAYS",			CLIP_TT_ALWAYS},
	{"DEFAULT_QUALITY",			DEFAULT_QUALITY},
	{"DRAFT_QUALITY",			DRAFT_QUALITY},
	{"PROOF_QUALITY",			PROOF_QUALITY},
	{"DEFAULT_PITCH",			DEFAULT_PITCH},
	{"FIXED_PITCH",				FIXED_PITCH},
	{"VARIABLE_PITCH",			VARIABLE_PITCH},
	{"FF_DECORATIVE",			FF_DECORATIVE},
	{"FF_DONTCARE",				FF_DONTCARE},
	{"FF_MODERN",				FF_MODERN},
	{"FF_ROMAN",				FF_ROMAN},
	{"FF_SCRIPT",				FF_SCRIPT},
	{"FF_SWISS",				FF_SWISS}
};
#endif
		private enum SECTION
		{
			SECTION_FINDTOKEN=0,
			SECTION_PROCESSTOKEN,
			SECTION_FINDSUBTOKEN,
			SECTION_PROCESSSUBTOKEN,
			SECTION_FINDPARAMS,
			SECTION_PROCESSPARAMS,
		};
		
		private  enum TOKEN
		{
			TOKEN_NOTHING=0,
			TOKEN_WINDOW,
			TOKEN_COMMON,
			TOKEN_LOCAL,
			TOKEN_FONT,
			TOKEN_IMAGE,
			TOKEN_SOUND,
			TOKEN_STRING,
		};

						
	}

	internal enum CPARSE
	{
		CPARSE_NOTHING=0,
		CPARSE_WINDOW,
		CPARSE_BUTTON,
		CPARSE_TEXT,
		CPARSE_EDITBOX,
		CPARSE_LISTBOX,
		CPARSE_SCROLLBAR,
		CPARSE_TREELIST,
		CPARSE_FONT,
		CPARSE_IMAGE,
		CPARSE_BITMAP,
		CPARSE_LINE,
		CPARSE_BOX,
		CPARSE_MARQUE,
		CPARSE_ANIMATION,
		CPARSE_CURSOR,
		CPARSE_SOUND,
		CPARSE_SLIDER,
		CPARSE_POPUP,
		CPARSE_PANNER,
		CPARSE_ANIM,
		CPARSE_STRING,
		CPARSE_MOVIE,
		CPARSE_FILL,
		CPARSE_TREE,
		CPARSE_CLOCK,
		CPARSE_TILE,
	}
// User ID Table file format: (MAX ID Length=35 chars) All values are signed longs
//                             IDs must start with a non-numeric/non-"white space" character
//                             Multiple IDs may have identical values (if you'd like)
//                             Sorry, No spaces in the ID name, the ID can be separated from it's value
//                             these characters only (" ",TAB,",",<cr>,<lf>) (as many as you'd like))
// 
// Line 1: SOMEID 5001    (where n is the value) 
//   .
//   .
//   .
// Line n: ANOTHERID, 23
//
// EACH Parser instance (parser=new C_Parser; parser.Setup() ...)
// maintains its own set of User-definable IDs
//
}

