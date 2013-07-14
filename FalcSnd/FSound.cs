using FalconNet.FalcLib;
using System;

namespace FalconNet.FalcSnd
{
    public static class FSound
    {
        // KCK: Used to convert flight and wing callnumbers to the correct CALLNUMBER eval value
        public const int VF_SHORTCALLSIGN_OFFSET = 45;
        public const int VF_FLIGHTNUMBER_OFFSET = 36;
#if TODO
public static int  F4LoadFXSound(char filename[], long Flags, SfxDef sfx);
#endif
        public static int F4LoadSound(string filename, long Flags)
        {
            throw new NotImplementedException();
        }
        public static int F4LoadRawSound(int flags, byte[] data, int len)
        {
            throw new NotImplementedException();
        }
        public static int F4IsSoundPlaying(int soundIdx)
        {
            throw new NotImplementedException();
        }
        public static int F4GetVolume(int soundIdx)
        {
            throw new NotImplementedException();
        }
        public static void F4FreeSound(ref int sound)
        {
            throw new NotImplementedException();
        }
        public static void F4PlaySound(int soundIdx)
        {
            throw new NotImplementedException();
        }
        public static void F4LoopSound(int soundIdx)
        {
            throw new NotImplementedException();
        }
        public static void F4StopSound(int soundIdx)
        {
            throw new NotImplementedException();
        }
        public static void ExitSoundManager()
        {
            throw new NotImplementedException();
        }
        public static void F4SoundStop()
        {
            throw new NotImplementedException();
        }
        public static void F4SoundStart()
        {
            throw new NotImplementedException();
        }
        public static void F4PitchBend(int soundIdx, float PitchMultiplier) // new frequency = orignal freq * PitchMult
        {
            throw new NotImplementedException();
        }
        public static void F4PanSound(int soundIdx, int Direction) // dBs -10000 to 10000 (0=Center)
        {
            throw new NotImplementedException();
        }
        public static long F4SetVolume(int soundIdx, int Volume) // dBs -10000 -> 0
        {
            throw new NotImplementedException();
        }
        //int F4CreateStream(int Flags); // NOT Necessary (OBSOLETE)
#if TODO
public static int F4CreateStream(WAVEFORMATEX *fmt,float seconds);
public static long F4StreamPlayed(int StreamID);
public static void F4RemoveStream(int StreamID);
public static int F4StartStream(char *filename,long flags=0);
public static int F4StartRawStream(int StreamID,char *Data,long size);
public static bool F4StartCallbackStream(int StreamID,void *ptr,DWORD (*cb)(void *,char *,DWORD));
#endif
        public static void F4StopStream(int StreamID)
        {
            throw new NotImplementedException();
        }
        public static long F4SetStreamVolume(int StreamID, long volume)
        {
            throw new NotImplementedException();
        }
        public static void F4HearVoices()
        {
            throw new NotImplementedException();
        }
        public static void F4SilenceVoices()
        {
            throw new NotImplementedException();
        }
        public static void F4StopAllStreams()
        {
            throw new NotImplementedException();
        }
        public static void F4ChatToggleXmitReceive()
        {
            throw new NotImplementedException();
        }

        /*
        ** Sound Groups
        */
        /*
        #define FX_SOUND_GROUP					0
        #define ENGINE_SOUND_GROUP				1
        #define SIDEWINDER_SOUND_GROUP			2
        #define RWR_SOUND_GROUP					3
        #define NUM_SOUND_GROUPS				4
        */
        //TODO #include "SoundGroups.h"

        public static void F4SetGroupVolume(int group, int vol) // dBs -10000 -> 0   
        {
            throw new NotImplementedException();
        }

        //TODO public static extern long gSoundFlags; // defined in top of fsound

        public enum FSND
        {
            FSND_SOUND = 0x00000001,
            FSND_REPETE = 0x00000002, // Pete's BRA bearing voice stuff
        };


        // for positional sound effects stuff
        public static void F4SoundFXSetPos(int sfxId, int _override, float x, float y, float z, float pscale, float volume = 0.0F)
        {
            throw new NotImplementedException();
        }
        public static void F4SoundFXSetDist(SFX_TYPES sfxId, bool _override, float dist, float pscale)
        {
            throw new NotImplementedException();
        }
        //void F4SoundFXSetCamPos( float x, float y, float z ); // obsolete JPO
#if TODO
public static void F4SoundFXSetCamPosAndOrient(Tpoint *campos, Trotation *camrot);
public static void F4SoundFXPositionDriver( uint begFrame, uint endFrame );
public static void F4SoundFXInit(   );
public static void F4SoundFXEnd(   );
public static bool F4SoundFXPlaying(int sfxId);

public static  int  InitSoundManager (HWND hWnd, int mode, char *falconDataDir);
#endif
    }
}
