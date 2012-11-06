using System;
using System.IO;

namespace FalconNet.FalcLib
{
	public class F4Find
	{
		public static  string FalconDataDirectory;
		public static  string FalconTerrainDataDir;
		public static  string FalconObjectDataDir;
		public static  string Falcon3DDataDir;
		public static  string FalconMiscTexDataDir;
		public static  string FalconCampaignSaveDirectory;
		public static  string FalconCampUserSaveDirectory;
		
		public static void InitDirectories()
		{
			FalconDataDirectory = "." + Path.DirectorySeparatorChar;
			FalconTerrainDataDir = FalconDataDirectory + Path.DirectorySeparatorChar + "theater";
			FalconObjectDataDir = FalconDataDirectory + Path.DirectorySeparatorChar + "object";
			FalconMiscTexDataDir = FalconDataDirectory + Path.DirectorySeparatorChar + "FalconMiscTexData";
			string campaignDir = FalconDataDirectory + Path.DirectorySeparatorChar + "Campaign"; 
			FalconCampaignSaveDirectory = campaignDir + Path.DirectorySeparatorChar + "Save";
			FalconCampUserSaveDirectory = campaignDir + Path.DirectorySeparatorChar + "Save";
	
		}
		
		// Returns path of file data is in, an offset and a length.
		public static  string  F4FindFile (string filename, string buffer, int bufLen, out int offset, out int len)
		{
			string path, tmp;
			string tmpStr;
		
			path = FalconDataDirectory + Path.DirectorySeparatorChar + "files.dir";
			throw new NotImplementedException ();
#if TODO
	   if (GetPrivateProfileString ("Files", filename, "", tmpStr, 1024, path))
			{
		   if (strchr(tmpStr, ','))
		      *(strchr(tmpStr, ',')) = ' ';
		   if (strchr(tmpStr, ','))
		      *(strchr(tmpStr, ',')) = ' ';
		   sscanf (tmpStr, "%s %d %d", tmp, fileOffset, fileLen);
			sprintf(path,"%s\\%s",FalconDataDirectory,tmp);
		   strncpy (buffer, path, min (strlen(path) + 1, (size_t)bufSize - 1));
		   buffer[bufSize-1] = 0;
			}
		else
			{
	//		sprintf(buffer,"");
	      strncpy (buffer, filename, bufSize);
		  buffer[bufSize - 1] = 0;
		   *fileLen = 0;
	   	*fileOffset = 0;
	//		return NULL;
			}
	   return (buffer);
#endif
		}

		// Returns a FileStream to a file created with passed name, path and mode
		public static  FileStream F4CreateFile (string  filename, string  path, FileMode  mode)
		{
		
			string filedir, ppath;
			string tmpStr;
			FileStream fp;
#if TODO			
			// Strip the FalconDataDirectory off the path, if it's there
			ppath = path;
			if (!strncmp(FalconDataDirectory,ppath,strlen(FalconDataDirectory)))
				ppath += strlen(FalconDataDirectory)+1;
		
			// Check if the file's already there
		   sprintf (filedir, "%s\\files.dir", FalconDataDirectory);
			if (!GetPrivateProfileString ("Files", filename, "", tmpStr, 1024, filedir))
				{
				sprintf(tmpStr,"%s\\%s,0,0",ppath,filename);
				if (!WritePrivateProfileString("Files", filename, tmpStr, filedir))
					return NULL;
				}
			sprintf(path,"%s\\%s",path,filename);
			if ((fp = fopen(path, mode)) == NULL)
				{
				sprintf(tmpStr,"Unable to create file: %s",path);
				F4Warning(tmpStr);
				}
#endif
			path = path + Path.DirectorySeparatorChar + filename;
			fp = File.Open (path, mode);
			return fp;
		}

		public static  FileStream F4OpenFile (string filename, string mode)
		{
			throw new NotImplementedException ();
		}

		public static  int F4ReadFile (FileStream fp, byte[] buffer, int size)
		{
			throw new NotImplementedException ();
		}
		
		public static  int F4WriteFile (FileStream fp, byte[] buffer, int size)
		{
			throw new NotImplementedException ();
		}
		
		public static  int F4CloseFile (FileStream fp)
		{
			throw new NotImplementedException ();
		}

		public static  string  F4ExtractPath (string path)
		{
			throw new NotImplementedException ();
		}

		public static  int F4LoadData (string filename, byte[] buffer, int length)
		{
			throw new NotImplementedException ();
		}
	
		public static  int F4LoadData (string path, byte[] buffer, int offset, int length)
		{
			throw new NotImplementedException ();
		}
	
		public static  int F4SaveData (string filename, byte[] buffer, int length)
		{
			throw new NotImplementedException ();
		}
	
		public static  int F4SaveData (string path, byte[] buffer, int offset, int length)
		{
			throw new NotImplementedException ();
		}
		
		public static  string  F4LoadDataID (string filename, int dataID, string  buffer)
		{
			throw new NotImplementedException ();
		}
 
		public static  int F4GetRegistryString (string  keyName, out string  dataPtr, int dataSize)
		{
#if TODO
			// In linux we don't have that. 
			string dataPtr = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\FALCON_REGISTRY_KEY\AppPath", keyName, null);    
			if (dataPtr != null)
			{
			    // Do stuff
			}
			
			
			int retval = true;
			DWORD type, size;
			HKEY theKey;
		
		   size = dataSize;
		   retval = RegOpenKeyEx(HKEY_LOCAL_MACHINE, FALCON_REGISTRY_KEY,
		      0, KEY_ALL_ACCESS, &theKey);
		   retval = RegQueryValueEx(theKey, keyName, 0, &type, (LPBYTE)dataPtr, &size);
		   if (retval != ERROR_SUCCESS)
		      {
			  memset (dataPtr, 0, dataSize);
		      retval = false;
		      }
		   else
		      retval = true;
		   RegCloseKey(theKey);
		
		   return retval;
#endif
			throw new NotImplementedException();
		}

	}
}

