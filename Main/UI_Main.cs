using System;
using FalconNet.FalcLib;
using FalconNet.UI;
using FalconNet.Common;
using FalconNet.Graphics;
using FalconNet.Ui95;
using System.IO;
using WORD=System.UInt16;

namespace FalconNet.Main
{
	public class UI_Main
	{
		public void UI_Startup ()
		{
			//	BITMAPINFO	bmi;
			FalconNet.Ui95.C_Window win;
			//	char		*cptr;
			ImageBuffer Primary;
			int i;
			//	long		count;
			//	WORD		*color,*dest;
			DWORD r_mask, g_mask, b_mask;
#if TODO			

			// M.N. Large UI
			if (F4Config.g_bHiResUI)
				FalconDisplayConfiguration.FalconDisplay.EnterMode (DisplayMode.UILarge,
														DisplayOptionsClass.DisplayOptions.DispVideoCard, 
				                                        DisplayOptionsClass.DisplayOptions.DispVideoDriver);
			else 
				FalconDisplayConfiguration.FalconDisplay.EnterMode (DisplayMode.UI,
														DisplayOptionsClass.DisplayOptions.DispVideoCard, 
				                                        DisplayOptionsClass.DisplayOptions.DispVideoDriver);
	
			Primary = FalconDisplayConfiguration.FalconDisplay.GetImageBuffer ();
			Primary.GetColorMasks( ref r_mask, ref g_mask, ref b_mask );
			UI95_SetScreenColorInfo((WORD)(r_mask), (WORD)(g_mask), (WORD)(b_mask));

			if(gScreenShotEnabled)
				{
					if (F4Config.g_bHiResUI)
						gScreenShotBuffer=new WORD[1024l*768l];
					else
						gScreenShotBuffer=new WORD[800l*600l];
				}
			
				gMainHandler=new C_Handler;
				gMainHandler.Setup(FalconDisplay.appWin,NULL,Primary);
			//	gMainHandler.SetCallback(UIMainMouse);
			
				GlobalSetup();
				LoadArtwork();
				LoadSoundFiles();
				LoadStringFiles();
				LoadMovieFiles();
				SetStartupFlags();
#endif			
				LoadMainWindow();
#if TODO			
				LoadCommsWindows();
				LoadHelpGuideWindows();
				RealLoadLogbook(); // without daves extra garbage
#endif
			throw new NotImplementedException ();
		}
		
		public void UI_Cleanup ()
		{
			throw new NotImplementedException ();
		}
		
		public void GlobalSetup ()
		{
			WORD r_mask;
			WORD r_shift;
			WORD g_mask;
			WORD g_shift;
			WORD b_mask;
			WORD b_shift;
#if TODO
			char *mouse;
		//	FILE *fp;
		
			mouse=MAKEINTRESOURCE(UI_F16);
			gCursors[1]=LoadCursor(hInst,mouse);
			gCursors[0]=gCursors[1];
			mouse=MAKEINTRESOURCE(UI_F16_ON);
			gCursors[2]=LoadCursor(hInst,mouse);
			mouse=MAKEINTRESOURCE(UI_F16_ON_RM);
			gCursors[3]=LoadCursor(hInst,mouse);
			mouse=MAKEINTRESOURCE(UI_F16_RM);
			gCursors[4]=LoadCursor(hInst,mouse);
			mouse=MAKEINTRESOURCE(UI_DRAG);
			gCursors[5]=LoadCursor(hInst,mouse);
			mouse=MAKEINTRESOURCE(UI_DRAG_RM);
			gCursors[6]=LoadCursor(hInst,mouse);
			mouse=MAKEINTRESOURCE(UI_DRAG_STEERPOINT);
			gCursors[7]=LoadCursor(hInst,mouse);
			mouse=MAKEINTRESOURCE(UI_HDRAG);
			gCursors[8]=LoadCursor(hInst,mouse);
			mouse=MAKEINTRESOURCE(UI_HDRAG_ON);
			gCursors[9]=LoadCursor(hInst,mouse);
			mouse=MAKEINTRESOURCE(UI_HDRAG_RM);
			gCursors[10]=LoadCursor(hInst,mouse);
			mouse=MAKEINTRESOURCE(UI_VDRAG);
			gCursors[11]=LoadCursor(hInst,mouse);
			mouse=MAKEINTRESOURCE(UI_VDRAG_ON);
			gCursors[12]=LoadCursor(hInst,mouse);
			mouse=MAKEINTRESOURCE(UI_VDRAG_RM);
			gCursors[13]=LoadCursor(hInst,mouse);
			mouse=MAKEINTRESOURCE(UI_STEERPOINT);
			gCursors[14]=LoadCursor(hInst,mouse);
			mouse=MAKEINTRESOURCE(UI_LIST_F16);
			gCursors[15]=LoadCursor(hInst,mouse);
			mouse=MAKEINTRESOURCE(UI_NOT_ALLOWED);
			gCursors[16]=LoadCursor(hInst,mouse);
			mouse=MAKEINTRESOURCE(UI_MAP_ZOOM);
			gCursors[17]=LoadCursor(hInst,mouse);
			mouse=MAKEINTRESOURCE(UI_TARGET);
			gCursors[18]=LoadCursor(hInst,mouse);
			mouse=MAKEINTRESOURCE(UI_WAIT);
			gCursors[19]=LoadCursor(hInst,mouse);
			mouse=MAKEINTRESOURCE(UI_TEXT);
			gCursors[20]=LoadCursor(hInst,mouse);
		
			SetCursor(gCursors[CRSR_WAIT]);
		
		//	fp=fopen("art\\main\\ascii.bin","rb");
		//	if(fp)
		//	{
		//		fread(Key_Chart,sizeof(Key_Chart),1,fp);
		//		fclose(fp);
		//	}
			
			if(gImageMgr == null)
				gImageMgr=new C_Image();
		
			gImageMgr.Setup();
			gImageMgr.SetColorKey(UI95_RGB24Bit(0x00ff00ff));
			UI95_GetScreenColorInfo(r_mask, r_shift, g_mask, g_shift, b_mask, b_shift);
		//!	UI95_GetScreenColorInfo(&r_mask,&r_shift,&g_mask,&g_shift,&b_mask,&b_shift);
			gImageMgr.SetScreenFormat(r_shift,g_shift,b_shift);
		
			gFontList=new C_Font();
			gFontList.Setup(gMainHandler);
		
			gAnimMgr=new C_Animation();
			gAnimMgr.Setup();
		
			gSoundMgr=new C_Sound();
			gSoundMgr.Setup();
		
			gStringMgr=new C_String();
			gStringMgr.Setup(TXT_LAST_TEXT_ID);
		
			gPopupMgr=new C_PopupMgr();
			gPopupMgr.Setup(gMainHandler);
		
			gMovieMgr=new C_Movie();
			gMovieMgr.Setup();
#endif
			// TODO These variables are defined somewhere ..??
			C_Handler gMainHandler = null;
			 C_Image gImageMgr = null;
			C_Font gFontList = null;
			C_Sound gSoundMgr = null;
			C_PopupMgr gPopupMgr = null;
			C_Animation gAnimMgr = null;
			C_String gStringMgr = null;
			C_Movie gMovieMgr = null;
			// END TODO
			gMainParser=new C_Parser();
			gMainParser.Setup(gMainHandler,gImageMgr,gFontList,gSoundMgr,gPopupMgr,gAnimMgr,gStringMgr,gMovieMgr);
		
			gMainParser.SetCheck(0); // Used to find which IDs are NOT used
		
			gMainParser.LoadIDList("userids.lst");
			gMainParser.LoadIDList("fontids.lst");
			gMainParser.LoadIDList("imageids.lst");
			gMainParser.LoadIDList("soundids.lst");
			gMainParser.LoadIDList("textids.lst");
			gMainParser.LoadIDList("movieids.lst");
		
			gMainParser.SetCheck(1); // Used to find which IDs are NOT used
		
			gMainParser.ParseFont("art"+Path.DirectorySeparatorChar+"fonts"+Path.DirectorySeparatorChar+"fontrc.irc");
#if TODO		
		#if DEBUG
			if(gMainParser.FindID("TXT_LAST_TEXT_ID") > TXT_LAST_TEXT_ID)
		        Debug.WriteLine( "String database Out of Date");
		#endif
		
			gUBuffer=gTrackBuffer[0];

			// This function goes through the class table searching for VIS IDs,
			// and returns the array index of the Vis ID... only needs to happen ONCE per execute
			ValidateRackData();
#endif	
		}
		
		void SetStartupFlags ()
		{
			throw new NotImplementedException ();
		}
		
		void PlayUIMusic ()
		{
			throw new NotImplementedException ();
		}

		void UI_LoadSkyWeatherData ()
		{
			throw new NotImplementedException ();
		}

		protected void HookupControls (long ID)
		{
			throw new NotImplementedException ();
		}

		protected void CloseAllRenderers (long openID)
		{
			throw new NotImplementedException ();
		}
		
		protected void LeaveCurrentGame ()
		{
			throw new NotImplementedException ();
		}
		
		protected void LoadMainWindow ()
		{
			long ID;
		
			if (MainLoaded)
				return;
		
			if (CHandlerStatic._LOAD_ART_RESOURCES_)
				gMainParser.LoadImageList ("main_res.lst");
			else
				gMainParser.LoadImageList ("main_art.lst");	// these aren't loaded anymore

			gMainParser.LoadSoundList("main_snd.lst");
			gMainParser.LoadWindowList("main_scf.lst"); // Modified by M.N. - add art/art1024 by LoadWindowList
#if TODO			
			ID=gMainParser.GetFirstWindowLoaded();
			while(ID)
			{
				HookupControls(ID);
				ID=gMainParser.GetNextWindowLoaded();
			}
			gMainParser.LoadPopupMenuList("art\\pop_scf.lst");
			HookupDogFightMenus();
			HookupCampaignMenus();
			LoadPeopleInfo(1);
			MainLoaded++;
			gUBuffer=&gTrackBuffer[0];
			
		/*	// Skycolor data readin
			sprintf(file,"%s\\weather\\todtable.dat",FalconTerrainDataDir);
		
			/* format:
				[NumberOfSkyColors]
				[UI display name]
				[TOD file name] [Image1] [Image2] [Image3] [Image4]
			
		
			if(!(fp=fopen(file,"rt")))
				return;
			NumberOfSkyColors = atoi(fgets(file,1024,fp));
		
			skycolor = new SkyColorDataType[NumberOfSkyColors];
			for (int i=0; i<NumberOfSkyColors; i++)
			{
				fgets(file,1024,fp);
				if (file[0] == '\r' || file[0] == '#' || file[0] == ';' || file[0] == '\n')
					continue;
				strcpy(name,file);
				strcpy(skycolor[i].name,name);
				fgets(file,1024,fp);
				sscanf(file, "%s %s %s %s %s",filename,image1,image2,image3,image4);
				strcpy(skycolor[i].todname,filename);
				strcpy(skycolor[i].image1,image1);
				if (!strlen(image2))		// If we have no entry, use the main image
					strcpy(image2,image1);
				strcpy(skycolor[i].image2,image2);
				if (!strlen(image3))
					strcpy(image3,image1);
				strcpy(skycolor[i].image3,image3);
				if (!strlen(image4))
					strcpy(image4,image1);
				strcpy(skycolor[i].image4,image4);
				strcpy(image1,"");
				strcpy(image2,"");
				strcpy(image3,"");
				strcpy(image4,"");
			}
			fclose(fp);
		
			prevskycol = PlayerOptions.skycol;
		
			sprintf(file,"%s\\weather\\weathertable.dat",FalconTerrainDataDir);
		
			if(!(fp=fopen(file,"rt")))
				return;
			NumWeatherPatterns = atoi(fgets(file,1024,fp));
		
			weatherPatternData = new WeatherPatternDataType[NumWeatherPatterns];
			for (i=0; i<NumWeatherPatterns; i++)
			{
				fgets(file,1024,fp);
				strcpy(weatherPatternData[i].name,file);
				fgets(file,1024,fp);
				sscanf(file,"%s %s",filename, picname); // filename = RAW, name = picture
				strcpy(weatherPatternData[i].filename,filename);
				strcpy(weatherPatternData[i].picfname,picname);
			}
			fclose(fp);		*/
#endif 
			throw new NotImplementedException ();
		}
		
		static C_Parser gMainParser;
		static bool IALoaded = false;
		static bool ACMILoaded = false;
		static bool DFLoaded = false;
		static bool TACLoaded = false;
		static bool CPLoaded = false;
		static bool CPSelectLoaded = false;
		static bool LBLoaded = false;
		static bool STPLoaded = false;
		static bool COLoaded = false;
		static bool MainLoaded = false;
		static bool PlannerLoaded = false;
		static bool TACREFLoaded = false;
		static bool CommonLoaded = false;
		static bool INFOLoaded = false;
		static bool HelpLoaded = false;
		static bool TACSelLoaded = false;
	}
}

