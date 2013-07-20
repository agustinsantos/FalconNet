using System;

namespace FalconNet.Campaign
{
	// Dogfight flags
	public enum DogfightFlags
	{
		DF_UNLIMITED_GUNS	=0x01,
		DF_ECM_AVAIL		=0x02,
		DF_GAME_OVER		=0x04
	}
	
	// Local Dogfight flags
	public enum DogfightLocalFlags
	{
		DF_VIEWED_SCORES	=0x01,			// User has viewed the scores, and is ready to reset
		DF_PLAYER_REQ_REGEN	=0x02,			// The player has requested regeneration (by keystroke)
	}
	
	
	// Dogfight game types
	public enum DogfightType  { dog_Furball, dog_TeamFurball, dog_TeamMatchplay };
	
	// GameStatus (Note: This is the local game status)
	public enum DogfightStatus { dog_Waiting, dog_Starting, dog_Flying, dog_EndRound };

// =====================================================================================
// KCK: This class will take care of all functionality associated with the various types
// of dogfight games.
// =====================================================================================
#if TODO
	public class DogfightClass
	{
	
		public DogfightType		gameType;
		public DogfightStatus		gameStatus;
		public DogfightStatus		localGameStatus;
		public CampaignTime		startTime;
		public float				startRange;
		public float				startAltitude;
		public float				startX;
		public float				startY;
		public float				xRatio;
		public float				yRatio;
		public short				flags;
		public short				localFlags;
		public byte				rounds;
		public byte				currentRound;
		public byte				numRadarMissiles;
		public byte				numRearAspectMissiles;
		public byte				numAllAspectMissiles;
		public static string		settings_filename;

	
		private CampaignTime		lastUpdateTime;
		private TailInsertList		*regenerationQueue;

	
		public DogfightClass( );
		//TODO public ~DogfightClass( );

		// Game setup
		public void SetGameType (DogfightType type)			{ gameType = type; }
		public void SetGameStatus (DogfightStatus stat)		{ gameStatus = stat; }
		public void SetRounds(byte numr)						{ rounds=numr; }
		public void SetStartRange (float newRange)				{ startRange = newRange * 0.5F;}
		public void SetStartAltitude (float newAltitude)		{ startAltitude = -newAltitude;}
		public void SetNumRadarMissiles (byte newRadar)		{ numRadarMissiles = newRadar;}
		public void SetNumRearMissiles (byte newRear)			{ numRearAspectMissiles = newRear;}
		public void SetNumAllAspectMissiles (byte newAll)		{ numAllAspectMissiles = newAll;}
		public void SetStartLocation (float newX, float newY)	{ startX = newX; startY = newY;}
		public void SetFlag (int flag)							{ flags |= flag;}
		public void UnSetFlag (int flag)						{ flags &= ~flag;}
		public int  IsSetFlag (int flag)						{ return (flags & flag) ? 1 : 0;}
		public void SetLocalFlag (int flag)					{ localFlags |= flag;}
		public void UnSetLocalFlag (int flag)					{ localFlags &= ~flag;}
		public int  IsSetLocalFlag (int flag)					{ return (localFlags & flag) ? 1 : 0;}
		
		public DogfightType GetGameType ()					{ return gameType; }
		public DogfightStatus GetLocalGameStatus ()		{ return localGameStatus; }
		public DogfightStatus GetDogfightGameStatus ()		{ return gameStatus; }
		public int GameStarted ()							{ if (gameStatus != dog_Waiting) return TRUE; return FALSE; }
		public int GetRounds ()							{ return rounds; }
		public float StartX ()								{ return startX;}
		public float StartY ()								{ return startY;}
		public float StartZ ()								{ return startAltitude;}

		// Functionality
		public void ApplySettings ();
		public void SendSettings (FalconSessionEntity *target);
		public void ReceiveSettings (DogfightClass *tmpSettings);
		public void ApplySettingsToFlight (FlightClass *flight);
		public void RequestSettings (FalconGameEntity *game);
		public int ReadyToStart ();

		public void SetFilename (string filename);
		public void LoadSettings ();
		public void SaveSettings (string filename);

		public void UpdateDogfight ();
		public void UpdateGameStatus ();
		public void RegenerateAircraft (AircraftClass *aircraft);
		public int AdjustClassId(int oldid, int team);
		public void EndGame ();

	
		// Private functions
		private int GameOver ();
		private void RestartGame ();
		private int CheckRoundOver ();
		private void RoundOver ();
		private void EndRound();
		private void ResetRound();
		private void RegenerateAvailableAircraft();

		public static DogfightClass SimDogfight;

	}


#endif
}

