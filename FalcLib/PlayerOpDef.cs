using System;

namespace FalconNet.FalcLib
{
// =====================
// Defines & enums
// =====================


// Display Flags
	[Flags]
	public enum PO_DISP_FLAGS
	{
		DISP_GOURAUD		=	0x01,
		DISP_HAZING			=	0x02,
		DISP_PERSPECTIVE	=	0x04,
		DISP_ALPHA_BLENDING =	0x08,
		DISP_BILINEAR		=	0x10,
	};

// Object Flags
	[Flags]
public enum PO_OBJ_FLAGS
	{
		DISP_OBJ_TEXTURES	=	0x01,
		DISP_OBJ_SHADING	=	0x02,
		DISP_OBJ_DYN_SCALING =	0x04,
	};

// Sim flags
	[Flags]
public enum PO_SIM_FLAGS
	{
		SIM_AUTO_TARGET		=	0x01,
		SIM_NO_BLACKOUT		=	0x02,
		SIM_UNLIMITED_FUEL	=	0x04,
		SIM_UNLIMITED_AMMO	=	0x08,
		SIM_UNLIMITED_CHAFF	=	0x10,
		SIM_NO_COLLISIONS	=	0x20,
		SIM_NAMETAGS		=	0x40,
		SIM_LIFTLINE_CUE	=	0x80,
		SIM_BULLSEYE_CALLS	=	0x100,
		SIM_INVULNERABLE	=	0x200,
//	SIM_CAMPAIGNTAGS	=	0x400,

		SIM_PRESET_FLAGS	=	0x27E,
		SIM_RULES_FLAGS		=	0x277,
	};

// Other flags
	[Flags]
public enum PO_GEN_FLAGS
	{
		GEN_NO_WEATHER		=	0x01,
		GEN_MFD_TERRAIN		=	0x02,
		GEN_HAWKEYE_TERRAIN	=	0x04,
		GEN_PADLOCK_VIEW	=	0x08,
		GEN_HAWKEYE_VIEW	=	0x10,
		GEN_EXTERNAL_VIEW	=	0x20,
		GEN_RULES_FLAGS		=	0x21,
	};

//TODO enum{	PL_FNAME_LEN		=	32};

// Flight model enum
	public enum  FlightModelType
	{
		FMSimplified,
		FMModerated,
		FMAccurate
	};

// Weapon effectiveness enum
	public enum WeaponEffectType
	{	
		WEExaggerated,
		WEEnhanced,
		WEAccurate
	} ;

// Avionics Difficulty enum
	public enum AvionicsType
	{	
		ATEasy,
		ATSimplified,
		ATRealistic,
		ATRealisticAV
	} ;		// Advanced realism (g_bRealisticAvionics)

// Autopilot mode
	public enum AutopilotModeType
	{
		APIntelligent,
		APEnhanced,
		APNormal
	} ;

// Air Refeuling mode
	public enum RefuelModeType
	{
		ARRealistic = 1,
		ARModerated,
		ARSimplistic
	} ;

// Padlock mode
	public enum PadlockModeType
	{	//PDSuper,
		PDEnhanced,
		PDRealistic,
		PDDisabled
	} ;

// Visual cues
	public enum VisualCueType
	{
		VCNone,
		VCLiftLine,
		VCReflection,
		VCBoth
	} ;


}

