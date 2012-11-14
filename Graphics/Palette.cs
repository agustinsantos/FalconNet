using System;

namespace FalconNet.Graphics
{
	public class Palette
	{
#if TODO
		#if USE_SH_POOLS
		// Overload new/delete to use a SmartHeap fixed size pool
		public void *operator new(size_t size) { Debug.Assert( size == sizeof(Palette) ); return MemAllocFS(pool);	};
		public void operator delete(void *mem) { if (mem) MemFreeFS(mem); };
		public static void InitializeStorage()	{ pool = MemPoolInitFS( sizeof(Palette), 100, 0 ); };
		public static void ReleaseStorage()	{ MemPoolFree( pool ); };
		public static MEM_POOL	pool;
		#endif
		
		public Palette()	{ refCount = 0; palHandle = null; memset(paletteData, 0, sizeof(paletteData)); }
		// public ~Palette()	{ Debug.Assert( refCount == 0); };
	
		 
		public DWORD[] paletteData = new DWORD[256];
		public PaletteHandle *palHandle;
	
		protected int		refCount;
		protected static DXContext	*rc = null; 	


		/***************************************************************************\
			Store some useful global information.  The RC is used for loading.
			This means that at present, only one device at a time can load textures
			through this interface.
		\***************************************************************************/
		public static void SetupForDevice( DXContext *texRC )
		{
			rc = texRC;
		}

		/***************************************************************************\
			This is called when we're done working with a given device (as 
			represented by an RC).
		\***************************************************************************/
		public static void CleanupForDevice( DXContext *texRC )
		{
			rc = null;
		}
		
		
		/***************************************************************************\
			Given a 256 entry, 24 bit per element palette, initialize our data
			structures.  The data that is passed in is copied, so can be freed
			by the caller after we return.
		\***************************************************************************/
		public void Setup24( BYTE  *data24 )
		{
			DWORD	*to;
			BYTE	*stop;
			
			Debug.Assert( palHandle == null );
			Debug.Assert( data24 );
		
			// Start our reference count at 1
			refCount = 1;
		
			// Convert from 24 to 32 bit palette entries
			to		= &paletteData[0];
			stop	= data24 + 768;
			while (	data24 < stop ) {
				*to =   ((BYTE)(*(data24  ))      ) |
						((BYTE)(*(data24+1)) <<  8) |
						((BYTE)(*(data24+2)) << 16) |
						((BYTE)(*(data24  )) << 24);		// Repeat Red component for alpha channel
				to++;
				data24 += 3;
			}
		}
		
		/***************************************************************************\
			Given a 256 entry, 32 bit per element palette, initialize our data
			structures.  The data that is passed in is copied, so can be freed
			by the caller after we return.
		\***************************************************************************/
		public void Setup32( DWORD *data32 )
		{
			Debug.Assert( palHandle == null );
			Debug.Assert( data32 );
		
			// Start our reference count at 1
			refCount = 1;
		
			// Copy the palette entries
			memcpy( paletteData, data32, sizeof(paletteData) );
		}

		public void Cleanup();
		
		/***************************************************************************\
			Note interest in this palette.
		\***************************************************************************/
		public void Reference()
		{
			Debug.Assert( refCount >= 0 );
			refCount++;
		}
		
		
		/***************************************************************************\
			Free MPR palette resources (if we were the last one using it).
		\***************************************************************************/
		public int Release()
		{
			Debug.Assert( refCount > 0 );
		
			refCount--;
		
			if (refCount == 0) {
				if (palHandle) {
					Debug.Assert( rc != null);
		
					delete palHandle;
					palHandle = null;
				}
			}
		
			return refCount;
		}

		public void Activate()	{ if (!palHandle) UpdateMPR(); }
		
		/***************************************************************************\
			Set the MPR palette entries for the given palette.
		\***************************************************************************/
		public void UpdateMPR( DWORD *pal )
		{
			Debug.Assert( rc != null );
			Debug.Assert( pal );
		
			if (!rc) // JB 010615 CTD
				return;
		
			// OW FIXME Error checking
			if(palHandle == null)
				palHandle = new PaletteHandle(rc.m_pDD, 32, 256);
		
			Debug.Assert(palHandle);
			palHandle.Load(MPR_TI_PALETTE,	32, 0, 256, (BYTE*)pal );
		}

		public void UpdateMPR() { UpdateMPR( paletteData ); }
		
		/***************************************************************************\
		    Update the light levels on our MPR palette without affecting our
			stored palette entries.
		\***************************************************************************/
		public void LightTexturePalette(Tcolor *light)
		{
			DWORD[]	pal = new DWORD[256];
			DWORD	*to, *stop;
			BYTE	*from;
			float	r, g, b;
		
			r = light.r;
			g = light.g;
			b = light.b;
		
			// Set up the loop control
			to = pal+1;
			from = (BYTE*)(paletteData+1);
			stop = pal + 256;
		
			// We skip the first entry to allow for chroma keying
			pal[0] = paletteData[0];
		
			// Build the lite version of the palette in temporary storage
			while (to < stop) {
				*to  =    (FloatToInt32(*(from)   * r))			// Red
						| (FloatToInt32(*(from+1) * g) << 8)	// Green
						| (FloatToInt32(*(from+2) * b) << 16)	// Blue
						| ((*(from+3)) << 24);					// Alpha
				from += 4;
				to ++;
			}
		
			// Send the new lite palette to MPR
			UpdateMPR( pal );
		}

		
		/***************************************************************************\
		    Update the light levels on our MPR palette without affecting our
			stored palette entries.
		
			Works within a palette entry range
		\***************************************************************************/
		public void LightTexturePaletteRange(Tcolor *light, int start, int end)
		{
			DWORD[]	pal = new DWORD[256];
			DWORD	*to, *stop;
			BYTE	*from;
			float	r, g, b;
		
			r = light.r;
			g = light.g;
			b = light.b;
		
			// Set up the loop control
			to = pal;
			from = (BYTE*)(paletteData);
			stop = pal + palStart;
		
			// Just copy the entries until we reach the start index
			while (to < stop) {
				*to++ = *(DWORD*)from;			
				from += 4;
			}
			
			// Now light the specified range
			stop = pal + palEnd+1;
			while (to < stop) {
				*to++  =  (FloatToInt32(*(from)   * r))			// Red
						| (FloatToInt32(*(from+1) * g) << 8)	// Green
						| (FloatToInt32(*(from+2) * b) << 16)	// Blue
						| ((*(from+3)) << 24);					// Alpha
				from += 4;
			}
		
			// Now copy the values beyond the ending index
			stop = pal + 256;
			while (to < stop) {
				*to++ = *(DWORD*)from;			
				from += 4;
			}
		
			// Send the new lite palette to MPR
			UpdateMPR( pal );
		}

		
			/***************************************************************************\
		    Update the light levels on our MPR palette without affecting our
			stored palette entries.  This one does special processing to brighten
			certain palette entries at night.
		\***************************************************************************/
		public void LightTexturePaletteBuilding(Tcolor *light)
		{
			DWORD[]	pal = new DWORD[256];
			DWORD	*to, *stop;
			BYTE	*from;
			float	r, g, b;
		
			r = light.r;
			g = light.g;
			b = light.b;
		
			// Special case for NVG mode
			if (TheTimeOfDay.GetNVGmode()) {
				r = 0.0f;
				g = NVG_LIGHT_LEVEL;
				b = 0.0f;
			}
		
			// Set up the loop control
			to = pal+1;
			from = (BYTE*)(paletteData+1);
			stop = pal + 248;
		
			// We skip the first entry to allow for chroma keying
			pal[0] = paletteData[0];
		
			// Darken the "normal" palette entries
			while (to < stop) {
				*to  =    (FloatToInt32(*(from)   * r))			// Red
						| (FloatToInt32(*(from+1) * g) << 8)	// Green
						| (FloatToInt32(*(from+2) * b) << 16)	// Blue
						| ((*(from+3)) << 24);					// Alpha
				from += 4;
				to ++;
			}
		
			// Only turn on the lights if it is dark enough
			if (light.g > 0.5f) {
				stop = pal + 256;
				while (to < stop) {
					*to  =    (FloatToInt32(*(from)   * r))			// Red
							| (FloatToInt32(*(from+1) * g) << 8)	// Green
							| (FloatToInt32(*(from+2) * b) << 16)	// Blue
							| ((*(from+3)) << 24);					// Alpha
					from += 4;
					to ++;
				}
			} else {
				// TODO: Blend these in gradually
				if (TheTimeOfDay.GetNVGmode()) {
					*to	= 0xFF00FF00;	to++;
					*to	= 0xFF00FF00;	to++;
					*to	= 0xFF00FF00;	to++;
					*to	= 0xFF00FF00;	to++;
					*to	= 0xFF00FF00;	to++;
					*to	= 0xFF00FF00;	to++;
					*to	= 0xFF00FF00;	to++;
					*to	= 0xFF00FF00;	to++;
				} else {
					*to	= 0xFF0000FF;	to++;
					*to	= 0xFF0F30BE;	to++;
					*to	= 0xFFFF0000;	to++;
					*to	= 0xFFAD0000;	to++;
					*to	= 0xFFABD34C;	to++;
					*to	= 0xFF9BB432;	to++;
					*to	= 0xFF87C5F0;	to++;
					*to	= 0xFF61B2EA;	to++;
				}
			}
		
			// Send the new lite palette to MPR
			UpdateMPR( pal );
		}
		
		
		/***************************************************************************\
		    Update the light levels on our MPR palette without affecting our
			stored palette entries.
		\***************************************************************************/
		public void LightTexturePaletteReflection(Tcolor *light)
		{
			DWORD[]	pal = new DWORD[256];
			DWORD	*to, *stop;
			BYTE	*from;
			float	r, g, b;
		
			r = light.r;
			g = light.g;
			b = light.b;
		
			// Special case for NVG mode
			if (TheTimeOfDay.GetNVGmode()) {
				r = 0.0f;
				g = NVG_LIGHT_LEVEL;
				b = 0.0f;
			}
		
			// Set up the loop control
			to = pal+1;
			from = (BYTE*)(paletteData+1);
			stop = pal + 256;
		
			// We skip the first entry to allow for chroma keying
			pal[0] = paletteData[0];
		
			// Build the lite version of the palette in temporary storage
			while (to < stop) {
				*to  =    (FloatToInt32(*(from)   * r))			// Red
						| (FloatToInt32(*(from+1) * g) << 8)	// Green
						| (FloatToInt32(*(from+2) * b) << 16)	// Blue
						| (0x26000000);							// Alpha
				from += 4;
				to ++;
			}
		
			// Send the new lite palette to MPR
			UpdateMPR( pal );
		}
#endif
	};

}

