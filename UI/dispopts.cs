using System;
using FalconNet.FalcLib;

namespace FalconNet.UI
{
	public class DisplayOptionsClass
	{
		
			public ushort DispWidth;			//
			public ushort DispHeight;
			public byte DispVideoCard;			//
			public byte DispVideoDriver;			//
	
			// OW
			public int DispDepth;		// Display Mode depth
	
			public DisplayOptionsClass()
			{
				Initialize();
			}

			public void Initialize()
			{
				//DispWidth = 640;
				//DispHeight = 480;
				DispWidth = 1024; // JB 011124
				DispHeight = 768; // JB 011124
				DispDepth = 16;	// OW
				DispVideoCard = 0;
				DispVideoDriver = 0;
			
				FalconDisplayConfiguration.FalconDisplay.SetSimMode(DispWidth, DispHeight, DispDepth);	// OW
			}

			public int LoadOptions(string filename ="display")
			{
#if TODO
				DWORD size;
				FILE *fp;
				size_t		success = 0;
				char		path[_MAX_PATH];
			
				sprintf(path,"%s\\config\\%s.dsp",FalconDataDirectory,filename);
				fp = fopen(path,"rb");
				if(!fp)
				{
					MonoPrint("Couldn't open display options\n");
					return FALSE;
				}
			
				fseek(fp,0,SEEK_END);
				size = ftell(fp);
				fseek(fp,0,SEEK_SET);
			
				if(size != sizeof(class DisplayOptionsClass))
				{
					MonoPrint("Display options are in old file format\n",filename);
					return FALSE;
				}
			
				success = fread(this, sizeof(class DisplayOptionsClass), 1, fp);
				fclose(fp);
				if(success != 1)
				{
					MonoPrint("Failed to read display options\n",filename);
					Initialize();
					return FALSE;
				}
			
				const char	*buf;
				int		i = 0;
			
				// Make sure the chosen sim video driver is still legal
				buf = FalconDisplay.devmgr.GetDriverName(i);
				while(buf)
				{
					i++;
					buf = FalconDisplay.devmgr.GetDriverName(i);
				}
			
				if(i<=DispVideoDriver)
				{
					DispVideoDriver = 0;
					DispVideoCard = 0;
				}
			
				// Make sure the chosen sim video device is still legal
				buf = FalconDisplay.devmgr.GetDeviceName(DispVideoDriver, i);
				while(buf)
				{
					i++;
					buf = FalconDisplay.devmgr.GetDeviceName(DispVideoDriver, i);
				}
			
				if(i<=DispVideoCard)
				{
					DispVideoDriver = 0;
					DispVideoCard = 0;
				}
			
				FalconDisplay.SetSimMode(DispWidth, DispHeight, DispDepth);
			
				return TRUE;
			#endif 
				throw new NotImplementedException();
			}

			public int SaveOptions( )
			{
#if TODO
				FILE *fp;
				size_t		success = 0;
				char		path[_MAX_PATH];
			
				sprintf(path,"%s\\config\\display.dsp",FalconDataDirectory);
					
				if((fp = fopen(path,"wb")) == NULL)
				{
					MonoPrint("Couldn't save display options");
					return FALSE;
				}
				success = fwrite(this, sizeof(class DisplayOptionsClass), 1, fp);
				fclose(fp);
				if(success == 1)
					return TRUE;
			
				return FALSE;
		#endif			
				throw new NotImplementedException();
			}
	
			public static DisplayOptionsClass DisplayOptions = new DisplayOptionsClass();
	
	}
}

