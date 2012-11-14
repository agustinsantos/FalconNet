using System;
using FalconNet.Common;

namespace FalconNet.Graphics
{
	[Flags]
	public enum PRIM_COLOR_OP
	{
		PRIM_COLOP_NONE = 0,
		PRIM_COLOP_COLOR = 1,
		PRIM_COLOP_INTENSITY = 2,
		PRIM_COLOP_TEXTURE = 4,
		PRIM_COLOP_FOG = 8
	};

//___________________________________________________________________________

public enum  MPRState{
	STATE_SOLID = 0,
    STATE_GOURAUD,
    STATE_TEXTURE,
    STATE_TEXTURE_PERSPECTIVE,
    STATE_TEXTURE_TRANSPARENCY,
    STATE_TEXTURE_TRANSPARENCY_PERSPECTIVE,
	STATE_TEXTURE_LIT,
    STATE_TEXTURE_LIT_PERSPECTIVE,
    STATE_TEXTURE_LIT_TRANSPARENCY,
    STATE_TEXTURE_LIT_TRANSPARENCY_PERSPECTIVE,
    STATE_TEXTURE_GOURAUD,
    STATE_TEXTURE_GOURAUD_TRANSPARENCY,
    STATE_TEXTURE_GOURAUD_PERSPECTIVE,
    STATE_TEXTURE_GOURAUD_TRANSPARENCY_PERSPECTIVE,

	STATE_ALPHA_SOLID,
    STATE_ALPHA_GOURAUD,
    STATE_ALPHA_TEXTURE,
    STATE_ALPHA_TEXTURE_PERSPECTIVE,
    STATE_ALPHA_TEXTURE_TRANSPARENCY,
    STATE_ALPHA_TEXTURE_TRANSPARENCY_PERSPECTIVE,
	STATE_ALPHA_TEXTURE_LIT,
    STATE_ALPHA_TEXTURE_LIT_PERSPECTIVE,
    STATE_ALPHA_TEXTURE_LIT_TRANSPARENCY,
    STATE_ALPHA_TEXTURE_LIT_TRANSPARENCY_PERSPECTIVE,
    STATE_ALPHA_TEXTURE_SMOOTH,
    STATE_ALPHA_TEXTURE_SMOOTH_TRANSPARENCY,
    STATE_ALPHA_TEXTURE_SMOOTH_PERSPECTIVE,
    STATE_ALPHA_TEXTURE_SMOOTH_TRANSPARENCY_PERSPECTIVE,
    STATE_ALPHA_TEXTURE_GOURAUD,
    STATE_ALPHA_TEXTURE_GOURAUD_PERSPECTIVE,
    STATE_ALPHA_TEXTURE_GOURAUD_TRANSPARENCY,
    STATE_ALPHA_TEXTURE_GOURAUD_TRANSPARENCY_PERSPECTIVE,

	STATE_FOG_TEXTURE,
    STATE_FOG_TEXTURE_PERSPECTIVE,
    STATE_FOG_TEXTURE_TRANSPARENCY,
    STATE_FOG_TEXTURE_TRANSPARENCY_PERSPECTIVE,
	STATE_FOG_TEXTURE_LIT,
    STATE_FOG_TEXTURE_LIT_PERSPECTIVE,
    STATE_FOG_TEXTURE_LIT_TRANSPARENCY,
    STATE_FOG_TEXTURE_LIT_TRANSPARENCY_PERSPECTIVE,
    STATE_FOG_TEXTURE_SMOOTH,
    STATE_FOG_TEXTURE_SMOOTH_TRANSPARENCY,
    STATE_FOG_TEXTURE_SMOOTH_PERSPECTIVE,
    STATE_FOG_TEXTURE_SMOOTH_TRANSPARENCY_PERSPECTIVE,
	STATE_FOG_TEXTURE_GOURAUD,
    STATE_FOG_TEXTURE_GOURAUD_PERSPECTIVE,
    STATE_FOG_TEXTURE_GOURAUD_TRANSPARENCY,
    STATE_FOG_TEXTURE_GOURAUD_TRANSPARENCY_PERSPECTIVE,

    STATE_TEXTURE_FILTER,
    STATE_TEXTURE_FILTER_PERSPECTIVE,

	STATE_TEXTURE_POLYBLEND,
    STATE_TEXTURE_POLYBLEND_GOURAUD,
    STATE_TEXTURE_FILTER_POLYBLEND,
    STATE_TEXTURE_FILTER_POLYBLEND_GOURAUD,

    STATE_APT_TEXTURE_FILTER_PERSPECTIVE,
    STATE_APT_FOG_TEXTURE_FILTER_PERSPECTIVE,

	STATE_APT_FOG_TEXTURE_PERSPECTIVE,
	STATE_APT_FOG_TEXTURE_SMOOTH_PERSPECTIVE,

	// OW
	STATE_TEXTURE_LIT_FILTER,
	STATE_TEXTURE_GOURAUD_FILTER,
	STATE_TEXTURE_TRANSPARENCY_FILTER,
	STATE_TEXTURE_LIT_TRANSPARENCY_FILTER,
	STATE_TEXTURE_GOURAUD_TRANSPARENCY_FILTER,
	STATE_TEXTURE_GOURAUD_TRANSPARENCY_PERSPECTIVE_FILTER,
	STATE_TEXTURE_LIT_TRANSPARENCY_PERSPECTIVE_FILTER,
	STATE_TEXTURE_TRANSPARENCY_PERSPECTIVE_FILTER,
	STATE_TEXTURE_GOURAUD_PERSPECTIVE_FILTER,
	STATE_TEXTURE_LIT_PERSPECTIVE_FILTER,
	STATE_TEXTURE_PERSPECTIVE_FILTER,
	STATE_ALPHA_TEXTURE_FILTER,
	STATE_ALPHA_TEXTURE_LIT_FILTER,
	STATE_ALPHA_TEXTURE_SMOOTH_FILTER,
	STATE_ALPHA_TEXTURE_GOURAUD_FILTER,
	STATE_ALPHA_TEXTURE_TRANSPARENCY_FILTER,
	STATE_ALPHA_TEXTURE_LIT_TRANSPARENCY_FILTER,
	STATE_ALPHA_TEXTURE_SMOOTH_TRANSPARENCY_FILTER,
	STATE_ALPHA_TEXTURE_GOURAUD_TRANSPARENCY_FILTER,
	STATE_ALPHA_TEXTURE_PERSPECTIVE_FILTER,
	STATE_ALPHA_TEXTURE_LIT_PERSPECTIVE_FILTER,
	STATE_ALPHA_TEXTURE_SMOOTH_PERSPECTIVE_FILTER,
	STATE_ALPHA_TEXTURE_GOURAUD_PERSPECTIVE_FILTER,
	STATE_ALPHA_TEXTURE_TRANSPARENCY_PERSPECTIVE_FILTER,
	STATE_ALPHA_TEXTURE_LIT_TRANSPARENCY_PERSPECTIVE_FILTER,
	STATE_ALPHA_TEXTURE_SMOOTH_TRANSPARENCY_PERSPECTIVE_FILTER,
	STATE_ALPHA_TEXTURE_GOURAUD_TRANSPARENCY_PERSPECTIVE_FILTER,

	MAXIMUM_MPR_STATE											
};


//#if (MAXIMUM_MPR_STATE >= 64)
//#error "Can have at most 64 prestored states (#define'd inside MPR) & we need one free"
//#endif

public struct TLVERTEX
{ 
	float sx; 
	float sy; 
	float sz; 
	float rhw; 
	float tu; 
	float tv; 
} ; 

public  struct TTLVERTEX
{ 
	float sx; 
	float sy; 
	float sz; 
	float rhw; 
	DWORD color; 
	float tu; 
	float tv; 
}  ;

//___________________________________________________________________________

public class ContextMPR
{
#if TODO
	// values for SetupMPRState flag argument
	public const int CHECK_PREVIOUS_STATE = 0x01;
	public const int  ENABLE_DITHERING_STATE = 0x02;
	
	public ContextMPR ()
	{ 
		#if _DEBUG
		m_nInstCount++;
		#endif
	
		m_pCtxDX = null;
		m_pDD = null;
		m_pD3DD = null;
		m_pVB = null;
		m_dwVBSize = 0;
		m_pIdx = null;
		m_dwNumVtx = 0;
		m_dwNumIdx = 0;
		m_dwStartVtx = 0;
		m_nCurPrimType = 0;
		m_VtxInfo = 0;
		m_pRenderTarget = null;
		m_bEnableScissors = false;
		m_pDDSP = null;
		m_bNoD3DStatsAvail = false;
		m_bUseSetStateInternal = false;
		m_pIB = null;
		m_nFrameDepth = 0;
		m_pVtx = null;
		m_bRenderTargetHasZBuffer = false;
		m_bViewportLocked = false;
	
		m_colFG = m_colBG = 0; // JPO initialise to something
		m_colFG_Raw = m_colBG_Raw = 0; // and again
		#if _DEBUG
		m_pVtxEnd = null;
		#endif
	}
	// public virtual ~ContextMPR();

	public BOOL Setup(ImageBuffer *pIB, DXContext *c)
	{
	BOOL bRetval = false;

	#if _CONTEXT_TRACE_ALL
	MonoPrint("ContextMPR::Setup(0x%X, 0x%X)\n", pIB, c);
	#endif

	try
	{
		m_pCtxDX = c;
//		m_pCtxDX.AddRef();

		if(!m_pCtxDX)
		{
			ShiWarning("Failed to create device!");
			return false;
		}

		Debug.Assert(m_pVtx == null);	// Cleanup() wasnt called if not zero

		Debug.Assert(pIB);
		if(!pIB)
			return false;

		m_pIB = pIB;
//		m_pIB.Lock(true);
		IDirectDrawSurface7 *lpDDSBack = pIB.targetSurface();
		NewImageBuffer((UInt) lpDDSBack);

		// Keep own references to important interfaces
		m_pDD = m_pCtxDX.m_pDD;
//		m_pDD.AddRef();
		m_pD3DD = m_pCtxDX.m_pD3DD;
//		m_pD3DD.AddRef();

		// Get a pointer to the primary surface (for gamma control if supported)
/* Warning this generates an addref on the primary surface
		if(m_pCtxDX.m_pcapsDD.dwCaps2 & DDCAPS2_PRIMARYGAMMA)
			m_pDD.EnumSurfaces(DDENUMSURFACES_DOESEXIST | DDENUMSURFACES_ALL,
				null, this, EnumSurfacesCB2);
*/

		// Create the main VB
		IDirect3DVertexBuffer7Ptr p;
		D3DVERTEXBUFFERDESC vbdesc;
		ZeroMemory(&vbdesc, sizeof(vbdesc));
		vbdesc.dwSize = sizeof(vbdesc);
		vbdesc.dwFVF = D3DFVF_XYZRHW | D3DFVF_DIFFUSE | D3DFVF_TEX1;
		vbdesc.dwCaps = D3DVBCAPS_WRITEONLY | D3DVBCAPS_DONOTCLIP;

		if(m_pCtxDX.m_eDeviceCategory >= DXContext::D3DDeviceCategory_Hardware_TNL)
		{
		    //			m_dwVBSize = 1000;	// Any larger than 1000 and the driver refuses to put it in vidmem due to DMA restrictions
		    m_dwVBSize = min(2000, 0x10000 / sizeof(TTLVERTEX));
		}
		
		else
		{
		    m_dwVBSize = D3DMAXNUMVERTICES >> 2;
		    vbdesc.dwCaps |= D3DVBCAPS_SYSTEMMEMORY;  // Non HW TNL devices require that VBs reside in system memory
		}

		vbdesc.dwNumVertices = m_dwVBSize;	// MPR_MAX_VERTICES

		CheckHR(m_pCtxDX.m_pD3D.CreateVertexBuffer(&vbdesc, &m_pVB, null));

		// Alloc index buffer
		m_pIdx = new WORD[vbdesc.dwNumVertices * 3];	// we intend to sent triangle lists
		if(!m_pIdx) throw _com_error(E_OUTOFMEMORY);

		#if _DEBUG
		D3DVERTEXBUFFERDESC ddsd;
		ZeroMemory(&ddsd, sizeof(ddsd));
		ddsd.dwSize = sizeof(ddsd);
		HRESULT hr = m_pVB.GetVertexBufferDesc(&ddsd);
		Debug.Assert(SUCCEEDED(hr));

//		MonoPrint("ContextMPR::Setup - m_pVB created in %s memory (%d vtx)\n",
//			ddsd.dwCaps & D3DVBCAPS_SYSTEMMEMORY ? "SYSTEM" : "VIDEO",
//			ddsd.dwNumVertices);
		Debug.Assert(ddsd.dwNumVertices == m_dwVBSize);
		#endif

		bool bCanDoAnisotropic = (m_pCtxDX.m_pD3DHWDeviceDesc.dpcTriCaps.dwTextureFilterCaps & D3DPTFILTERCAPS_MAGFANISOTROPIC) &&
			(m_pCtxDX.m_pD3DHWDeviceDesc.dpcTriCaps.dwTextureFilterCaps & D3DPTFILTERCAPS_MINFANISOTROPIC);

		m_bLinear2Anisotropic = bCanDoAnisotropic && PlayerOptions.bHQFiltering;

// Hack for GF3 users - The D3D stuff doesn't see the GF3 as anisotropic capable with some drivers...
		if (g_bAlwaysAnisotropic)
		{
			bCanDoAnisotropic = true;
			if (PlayerOptions.bHQFiltering)
				m_bLinear2Anisotropic = true;
		}

		// Setup our set of cached rendering states
		SetupMPRState(CHECK_PREVIOUS_STATE);

		// Start out in simple flat shaded mode
		m_pD3DD.SetRenderState(D3DRENDERSTATE_COLORVERTEX, true);
		m_pD3DD.SetRenderState(D3DRENDERSTATE_LIGHTING, false);
		m_pD3DD.SetRenderState(D3DRENDERSTATE_CULLMODE, D3DCULL_NONE);
		m_pD3DD.SetRenderState(D3DRENDERSTATE_ZENABLE, false);
		m_pD3DD.SetRenderState(D3DRENDERSTATE_FOGENABLE, false);
		m_pD3DD.SetRenderState(D3DRENDERSTATE_STIPPLEDALPHA, false);
		m_pD3DD.SetRenderState(D3DRENDERSTATE_COLORKEYENABLE, false);
		m_pD3DD.SetRenderState(D3DRENDERSTATE_STENCILENABLE, false);   

		// Disable all stages
		for(int i=0;i<8;i++)
		{
			m_pD3DD.SetTextureStageState(i, D3DTSS_COLOROP, D3DTOP_DISABLE);
			m_pD3DD.SetTextureStageState(i, D3DTSS_ALPHAOP, D3DTOP_DISABLE);
		}

		// Setup stage 0
		m_pD3DD.SetTextureStageState(0, D3DTSS_COLORARG2, D3DTA_TEXTURE);
		m_pD3DD.SetTextureStageState(0, D3DTSS_COLORARG2, D3DTA_CURRENT);
		m_pD3DD.SetTextureStageState(0, D3DTSS_ALPHAARG1, D3DTA_TEXTURE);
		m_pD3DD.SetTextureStageState(0, D3DTSS_ALPHAARG2, D3DTA_CURRENT);

		if(g_bUseMipMaps)
		{
			m_pD3DD.SetTextureStageState(0, D3DTSS_MIPMAPLODBIAS, *((LPDWORD) (&g_fMipLodBias)));
			m_pD3DD.SetTextureStageState(0, D3DTSS_MIPFILTER, D3DTFP_LINEAR);
		}

		InvalidateState();
		RestoreState( STATE_SOLID );
		ZeroMemory(&m_rcVP, sizeof(m_rcVP));
		m_bViewportLocked = false;

		#if _CONTEXT_ENABLE_STATS
		m_stats.Init();
		m_stats.StartBatch();
		#endif


		bRetval = true;
	}

	catch(_com_error e)
	{
		MonoPrint("ContextMPR::Setup - Error 0x%X\n", e.Error());
	}

//	if(m_pIB)
//		m_pIB.Unlock();

	return bRetval;
}

	public void Cleanup(   )
{
	#if _CONTEXT_TRACE_ALL
	MonoPrint("ContextMPR::Cleanup()\n");
	#endif

	#if _DEBUG
	#if _CONTEXT_ENABLE_STATS
	m_stats.Report();
	#endif
	#endif

/*
	if(m_nFrameDepth && m_pIB)
	{
		Debug.Assert(false);	// our image buffer is still locked!!
		m_pIB.Unlock();
	}
*/

	if(StateSetupCounter)
		CleanupMPRState(CHECK_PREVIOUS_STATE);

	// Warning: The SIM code uses a shared DXContext which might be already toast when this function gets called!!
	// Under no circumstances access m_pCtxDX here
	// Btw: this was causing the infamous LGB CTD

	// JPO - now added reference counting so that it is shareable and releaseable - I think.
//	m_pCtxDX.Release();
	m_pCtxDX = null;



/*	Can be all toast now dont touch anything ;(  (Note this is causing VertexBufffer Memory leaks)
*/
#if NOTHING
	if(m_pRenderTarget)
	{
		m_pRenderTarget.Release();
		m_pRenderTarget = null;
	}
	
	if(m_pVB)
	{
		m_pVB.Release();
		m_pVB = null;
	}

	if(m_pDDSP)
	{
		m_pDDSP.Release();
		m_pDDSP = null;
	}

	if(m_pD3DD)
	{
		DWORD dwRefCnt = m_pD3DD.Release();
		m_pD3DD = null;
	}

	if(m_pDD)
	{
		DWORD dwRefCnt = m_pDD.Release();
		m_pDD = null;
	}
#endif
// Only this release seems to work without problems, good that it is the largest one...
	if(m_pVB)
	{
		m_pVB.Release();
		m_pVB = null;
	}

	if(m_pIdx)
	{
		delete[] m_pIdx;
		m_pIdx = null;
	}

	m_pIdx = null;
	m_dwNumVtx = 0;
	m_dwNumIdx = 0;
	m_dwStartVtx = 0;
	m_nCurPrimType = 0;
	m_VtxInfo = 0;
	m_pIB = null;
	m_nFrameDepth = 0;
	m_pVtx = null;
	m_bRenderTargetHasZBuffer = false;

	#if _DEBUG
	m_pVtxEnd = null;
	#endif

	#if _CONTEXT_RECORD_USED_STATES
	MonoPrint("ContextMPR::Cleanup - Report of used states follows\n	");
	std::set<int>::iterator it;
	for(it = m_setStatesUsed.begin(); it != m_setStatesUsed.end(); it++)
		MonoPrint("%d, ", *it);
	m_setStatesUsed.clear();
	MonoPrint("\nContextMPR::Cleanup - End of report\n	");
	#endif
}
	public void NewImageBuffer( UInt lpDDSBack )
	{
	#if _CONTEXT_TRACE_ALL
	MonoPrint("ContextMPR::NewImageBuffer(0x%X)\n", lpDDSBack);
	#endif

	if(m_pRenderTarget)
	{
//		m_pRenderTarget.Release();
		m_pRenderTarget = null;
	}

	m_pRenderTarget = (IDirectDrawSurface7 *) lpDDSBack;
//	m_pRenderTarget.AddRef();

	// weird, some drivers (like the 3.68 detonators) implicitely create Z buffers
	if(m_pRenderTarget)
	{
		IDirectDrawSurface7Ptr pDDS;

		DDSCAPS2 ddscaps;
		ZeroMemory(&ddscaps, sizeof(ddscaps));
		ddscaps.dwCaps = DDSCAPS_ZBUFFER; 
		m_bRenderTargetHasZBuffer = SUCCEEDED(m_pRenderTarget.GetAttachedSurface(&ddscaps, &pDDS)); 
	}
}
	
		public void SetState(WORD State, DWORD Value)
{
	if(m_bUseSetStateInternal)
	{
		SetStateInternal(State, Value);
		return;
	}

	#if _CONTEXT_TRACE_ALL
	MonoPrint("ContextMPR::SetState(%d, 0x%X)\n", State, Value);
	#endif
	//TODO Debug.Assert(false == F4IsBadReadPtr(m_pD3DD, sizeof *m_pD3DD));
	Debug.Assert (State != MPR_STA_TEX_ID);
	Debug.Assert (State != MPR_STA_FG_COLOR);

	if (!m_pD3DD)
		return;

	switch(State)
	{
		case MPR_STA_ENABLES:               /* use MPR_SE_...   */
		{
			if(Value & MPR_SE_MODULATION)
			{
				FlushVB();

				// Handled via FVF change 
				m_pD3DD.SetTextureStageState(0, D3DTSS_COLOROP, D3DTOP_MODULATE);
			}

			if(Value & MPR_SE_CLIPPING)
			{
				FlushVB();

				m_pD3DD.SetRenderState(D3DRENDERSTATE_CLIPPING, true);
			}

			if(Value & MPR_SE_BLENDING)
			{
				FlushVB();

				m_pD3DD.SetRenderState(D3DRENDERSTATE_ALPHABLENDENABLE, true);
				m_pD3DD.SetRenderState(D3DRENDERSTATE_ALPHATESTENABLE, false);

				// Also modulate alpha from vertex and texture
// OW, removed 23-07-2000 to fix cloud rendering problem when the transparency options was enabled in falcon
//				m_pD3DD.SetTextureStageState(0, D3DTSS_ALPHAOP, D3DTOP_MODULATE);
			}

			if(Value & MPR_SE_TRANSPARENCY)
			{
				FlushVB();

				// Use alpha testing to speed it up (if supported) - note: this is not redundant
				if(m_pCtxDX.m_pD3DHWDeviceDesc.dpcTriCaps.dwAlphaCmpCaps & D3DCMP_GREATEREQUAL)
				{
					m_pD3DD.SetRenderState(D3DRENDERSTATE_ALPHATESTENABLE, true);
					m_pD3DD.SetRenderState(D3DRENDERSTATE_ALPHAREF, (DWORD) 1);
					m_pD3DD.SetRenderState(D3DRENDERSTATE_ALPHAFUNC, D3DCMP_GREATEREQUAL);
				}

				m_pD3DD.SetRenderState(D3DRENDERSTATE_ALPHABLENDENABLE, true);
				m_pD3DD.SetRenderState(D3DRENDERSTATE_SRCBLEND, D3DBLEND_SRCALPHA);
				m_pD3DD.SetRenderState(D3DRENDERSTATE_DESTBLEND, D3DBLEND_INVSRCALPHA);
			}

			if(Value & MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE)
			{
				FlushVB();

				m_pD3DD.SetRenderState(D3DRENDERSTATE_TEXTUREPERSPECTIVE , false);
			}

			if(Value & MPR_SE_SHADING)
			{
				FlushVB();

				m_pD3DD.SetRenderState(D3DRENDERSTATE_SHADEMODE, D3DSHADE_GOURAUD);
			}

			if(Value & MPR_SE_DITHERING)
			{
				FlushVB();

				m_pD3DD.SetRenderState(D3DRENDERSTATE_DITHERENABLE , true);

				// see MPRcfg.h (MPR_BILINEAR_CLAMPED)
				m_pD3DD.SetTextureStageState(0, D3DTSS_ADDRESS, D3DTADDRESS_CLAMP);
			}

			if((Value & MPR_SE_SCISSORING) && !m_bEnableScissors)
			{
				FlushVB();

				m_bEnableScissors = true;
				UpdateViewport();
			}

			#if _DEBUG
			if(Value & MPR_SE_TEXTURING)
			{
				// Enabled by selecting a texture into the device
			}

			if(Value & MPR_SE_FOG)
			{
#if NOTHING
				m_pD3DD.SetRenderState(D3DRENDERSTATE_FOGENABLE, true);

				float fStart = 0.1f;    // for linear mode
				float fEnd   = 0.8f;
				float fDensity = 0.66;  // for exponential modes
				m_pD3DD.SetRenderState(D3DRENDERSTATE_FOGTABLEMODE, D3DFOG_LINEAR);	// D3DFOG_EXP, D3DFOG_EXP2
				m_pD3DD.SetRenderState(D3DRENDERSTATE_FOGSTART, *(DWORD *)(&fStart));
				m_pD3DD.SetRenderState(D3DRENDERSTATE_FOGEND,   *(DWORD *)(&fEnd));
#endif
			}

			if(Value & MPR_SE_Z_BUFFERING)
			{
				// FlushVB();

				Debug.Assert(false);		// we do not create Z buffers for now
				// HRESULT hr = m_pD3DD.SetRenderState(D3DRENDERSTATE_ZENABLE, true);
				// Debug.Assert(SUCCEEDED(hr));
			}

			if(Value & MPR_SE_FILTERING)
			{
				// Handled by filter function setting
			}

			if(Value & MPR_SE_PIXEL_MASKING)
			{
				Debug.Assert(false);
			}

			if(Value & MPR_SE_Z_MASKING)
			{
				Debug.Assert(false);
			}

			if(Value & MPR_SE_PATTERNING)
			{
				Debug.Assert(false);
			}

			if(Value & MPR_SE_NON_SUBPIXEL_PRECISION_MODE)
			{
				Debug.Assert(false);
			}

			if(Value & MPR_SE_HARDWARE_OFF)
			{
				Debug.Assert(false);
			}

			if(Value & MPR_SE_PALETTIZED_TEXTURES)
			{
				// OW FIXME: No idea
			}

			if(Value & MPR_SE_DECAL_TEXTURES)
			{
				Debug.Assert(false);
			}

			if(Value & MPR_SE_ANTIALIASING)
			{
				Debug.Assert(false);
			}
			#endif

			break;
		}

		case MPR_STA_DISABLES:              /* use MPR_SE_...   */
		{
			if(Value & MPR_SE_MODULATION)
			{
				FlushVB();

				m_pD3DD.SetTextureStageState(0, D3DTSS_COLOROP, D3DTOP_SELECTARG1);
			}

			if(Value & MPR_SE_TEXTURING)
			{
				FlushVB();

				m_pD3DD.SetTexture(0, null);
			}

			if(Value & MPR_SE_SHADING)
			{
				FlushVB();

				m_pD3DD.SetRenderState(D3DRENDERSTATE_SHADEMODE, D3DSHADE_FLAT);
			}

			if(Value & MPR_SE_BLENDING)
			{
				FlushVB();

				m_pD3DD.SetRenderState(D3DRENDERSTATE_ALPHABLENDENABLE, false);

				// Only use texture alpha
				m_pD3DD.SetTextureStageState(0, D3DTSS_ALPHAOP, D3DTOP_SELECTARG1);
			}

			if(Value & MPR_SE_FILTERING)
			{
				FlushVB();

				m_pD3DD.SetTextureStageState(0, D3DTSS_MAGFILTER, D3DTFG_POINT);
				m_pD3DD.SetTextureStageState(0, D3DTSS_MINFILTER, D3DTFN_POINT);
			}

			if(Value & MPR_SE_TRANSPARENCY)
			{
				FlushVB();

				// Use alpha testing to speed it up (if supported)
				if(m_pCtxDX.m_pD3DHWDeviceDesc.dpcTriCaps.dwAlphaCmpCaps & D3DCMP_GREATEREQUAL)
				{
					m_pD3DD.SetRenderState(D3DRENDERSTATE_ALPHATESTENABLE, false);

					// For Voodoo 1
					m_pD3DD.SetRenderState(D3DRENDERSTATE_ALPHAFUNC, D3DCMP_ALWAYS);
				}

				m_pD3DD.SetRenderState(D3DRENDERSTATE_ALPHABLENDENABLE, false);
			}

			if(Value & MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE)
			{
				FlushVB();

				m_pD3DD.SetRenderState(D3DRENDERSTATE_TEXTUREPERSPECTIVE, true);
			}

			if(Value & MPR_SE_DITHERING)
			{
				FlushVB();

				m_pD3DD.SetRenderState(D3DRENDERSTATE_DITHERENABLE , false);
			}

			if((Value & MPR_SE_SCISSORING) && m_bEnableScissors)
			{
				FlushVB();

				m_bEnableScissors = false;
				UpdateViewport();
			}

			if(Value & MPR_SE_CLIPPING)
			{
				FlushVB();

				m_pD3DD.SetRenderState(D3DRENDERSTATE_CLIPPING, false);
			}

			if(Value & MPR_SE_FOG)
			{
				FlushVB();

				m_pD3DD.SetRenderState(D3DRENDERSTATE_FOGENABLE, false);
			}

			if(Value & MPR_SE_Z_BUFFERING)
			{
				FlushVB();

				m_pD3DD.SetRenderState(D3DRENDERSTATE_ZENABLE, false);
			}

			#if _DEBUG
			if(Value & MPR_SE_PIXEL_MASKING)
			{
				Debug.Assert(false);
			}

			if(Value & MPR_SE_Z_MASKING)
			{
				Debug.Assert(false);
			}

			if(Value & MPR_SE_PATTERNING)
			{
				Debug.Assert(false);
			}

			if(Value & MPR_SE_NON_SUBPIXEL_PRECISION_MODE)
			{
				Debug.Assert(false);
			}

			if(Value & MPR_SE_HARDWARE_OFF)
			{
				Debug.Assert(false);
			}

			if(Value & MPR_SE_PALETTIZED_TEXTURES)
			{
				// OW FIXME: No idea
			}

			if(Value & MPR_SE_DECAL_TEXTURES)
			{
				Debug.Assert(false);
			}

			if(Value & MPR_SE_ANTIALIASING)
			{
				Debug.Assert(false);
			}
			#endif

			break;
		}

		case MPR_STA_SRC_BLEND_FUNCTION:    /* use MPR_BF_...   */
		{
			FlushVB();

			m_pD3DD.SetRenderState(D3DRENDERSTATE_SRCBLEND, Value);

			break;
		}

		case MPR_STA_DST_BLEND_FUNCTION:    /* use MPR_BF_...   */
		{
			FlushVB();

			m_pD3DD.SetRenderState(D3DRENDERSTATE_DESTBLEND, Value);

			break;
		}

		case MPR_STA_TEX_FILTER:            /* use MPR_TX_...   */
		{
			FlushVB();

			switch(Value)
			{
				case MPR_TX_NONE:
				case MPR_TX_DITHER:                /* Dither the colors */
				{
					m_pD3DD.SetTextureStageState(0, D3DTSS_MAGFILTER, D3DTFG_POINT);
					m_pD3DD.SetTextureStageState(0, D3DTSS_MINFILTER, D3DTFN_POINT);
					break;
				}

				case MPR_TX_BILINEAR:              /* interpolate 4 pixels     */
				case MPR_TX_BILINEAR_NOCLAMP:              /* interpolate 4 pixels     */
				{
					if(m_bLinear2Anisotropic)
					{
						m_pD3DD.SetTextureStageState(0, D3DTSS_MAGFILTER, D3DTFG_ANISOTROPIC);
						m_pD3DD.SetTextureStageState(0, D3DTSS_MINFILTER, D3DTFN_ANISOTROPIC);
						m_pD3DD.SetTextureStageState(0, D3DTSS_MAXANISOTROPY, m_pCtxDX.m_pD3DHWDeviceDesc.dwMaxAnisotropy );
					}

					else
					{
						m_pD3DD.SetTextureStageState(0, D3DTSS_MAGFILTER, D3DTFG_LINEAR);
						m_pD3DD.SetTextureStageState(0, D3DTSS_MINFILTER, D3DTFN_LINEAR);
					}

					if(Value == MPR_TX_BILINEAR)
					{
						// see MPRcfg.h (MPR_BILINEAR_CLAMPED)
						m_pD3DD.SetTextureStageState(0, D3DTSS_ADDRESS, D3DTADDRESS_CLAMP);
					}

					break;
				}

				case MPR_TX_MIPMAP_NEAREST:   /* nearest mipmap       */
				{
					m_pD3DD.SetTextureStageState(0, D3DTSS_MIPFILTER, D3DTFP_POINT);
					break;
				}

				case MPR_TX_MIPMAP_LINEAR:         /* interpolate between mipmaps  */
				case MPR_TX_MIPMAP_BILINEAR:       /* interpolate 4x within mipmap */
				case MPR_TX_MIPMAP_TRILINEAR:      /* interpolate mipmaps,4 pixels */
				{
					m_pD3DD.SetTextureStageState(0, D3DTSS_MIPFILTER, D3DTFP_LINEAR);
					break;
				}
			}

			break;
		}

		case MPR_STA_TEX_FUNCTION:          /* use MPR_TF_...   */
		{
			FlushVB();

			switch(Value)
			{
				case MPR_TF_MULTIPLY:
				{
					m_pD3DD.SetTextureStageState(0, D3DTSS_COLOROP, D3DTOP_MODULATE);
					break;
				}

				case MPR_TF_ALPHA:
				{
					m_pD3DD.SetTextureStageState(0, D3DTSS_COLOROP, D3DTOP_BLENDDIFFUSEALPHA);
					break;
				}

				case MPR_TF_NONE:
				case MPR_TF_ADD:
				case MPR_TF_FOO:
				case MPR_TF_SHADE:
				{
					Debug.Assert(false);		// unused
					break;
				}
			}

			break;
		}

		#if MPR_MASKING_ENABLED
		case MPR_STA_AREA_MASK:             /* Area pattern bitmask */
		{
			Debug.Assert(false);
			break;
		}

		case MPR_STA_LINE_MASK:             /* Line pattern bitmask */
		{
			Debug.Assert(false);
			break;
		}

		case MPR_STA_PIXEL_MASK:            /* RGBA or bitmask  */
		{
			Debug.Assert(false);
			break;
		}

		#endif
		case MPR_STA_FG_COLOR:              /* long: RGBA or index  */
		{
			SelectForegroundColor(Value);
			break;
		}

		case MPR_STA_BG_COLOR:              /* long: RGBA or index  */
		{
			SelectBackgroundColor(Value);
			break;
		}


		#if  MPR_DEPTH_BUFFER_ENABLED
		case MPR_STA_Z_FUNCTION:            /* use MPR_ZF_...   */
		{
			Debug.Assert(false);
			break;
		}

		case MPR_STA_FG_DEPTH:              /* FIXED 16.16 for raster*/
		{
			Debug.Assert(false);
			break;
		}

		case MPR_STA_BG_DEPTH:              /* FIXED 16.16 for zclear*/
		{
			Debug.Assert(false);
			break;
		}

		#endif

		case MPR_STA_TEX_ID:                /* Handle for current texture.*/
		{
			Debug.Assert(false);
			break;
		}

		case MPR_STA_FOG_COLOR:             /* long: RGBA       */
		{
			FlushVB();
			// OW FIXME: hmm whats the correlation of this state with the fogColor value used in TheStateStack.fogValue ???
			if (g_nFogRenderState & 0x01)
				m_pD3DD.SetRenderState(D3DRENDERSTATE_FOGCOLOR, MPRColor2D3DRGBA(Value));
			break;
		}

		case MPR_STA_HARDWARE_ON:           /* Read only - set if hardware supports mode */
		{
			Debug.Assert(false);
			break;
		}

		//#if !defined(MPR_GAMMA_FAKE)
		case MPR_STA_GAMMA_RED:             /* Gamma correction term for red (set before blue)  */
		{
// called for software devices
//			Debug.Assert(false);
			break;
		}

		case MPR_STA_GAMMA_GREEN:           /* Gamma correction term for green (set before blue) */
		{
// called for software devices
//			Debug.Assert(false);
			break;
		}

		case MPR_STA_GAMMA_BLUE:            /* Gamma correction term for blue (set last) */
		{
// called for software devices
//			Debug.Assert(false);
			break;
		}

		//#else
		case MPR_STA_RAMP_COLOR:           /* Packed color for the ramp table        */
		{
			/*
			IDirectDrawGammaControlPtr p(m_pDDSP);

			if(p)
			{
			}
			*/

			break;
		}

		case MPR_STA_RAMP_PERCENT:         /* fractional (0.0=normal: 1.0=saturated) */
		{
			// OW FIXME: todo
			break;
		}

		//#endif

		case MPR_STA_SCISSOR_LEFT:  
		{
			if(Value != m_rcVP.left)
			{
				FlushVB();

				m_rcVP.left = Value;
				UpdateViewport();
			}
			break;
		}

		case MPR_STA_SCISSOR_TOP:  
		{
			if(Value != m_rcVP.top)
			{
				FlushVB();

				m_rcVP.top = Value;
				UpdateViewport();
			}

			break;
		}

		case MPR_STA_SCISSOR_RIGHT:         /* right:bottom: not inclusive*/
		{
			if(Value != m_rcVP.right)
			{
				FlushVB();

				m_rcVP.right = Value;	// not inclusive
				UpdateViewport();
			}

			break;
		}

		case MPR_STA_SCISSOR_BOTTOM:        /* Validity Check done here.    */
		{
			if(Value != m_rcVP.bottom)
			{
				FlushVB();

				m_rcVP.bottom = Value;	// not inclusive
				UpdateViewport();
			}

			break;
		}

		case MPR_STA_NONE:
		{
			break;
		}
	}
}
	public void SetStateInternal(WORD State, DWORD Value)
{
	// This is for internal state tracking - currently only enable/disable is tracked
	Debug.Assert (State != MPR_STA_TEX_ID);
	Debug.Assert (State != MPR_STA_FG_COLOR);

	switch(State)
	{
		case MPR_STA_NONE:
		{
			break;
		}

		case MPR_STA_DISABLES:              /* use MPR_SE_...   */
		case MPR_STA_ENABLES:               /* use MPR_SE_...   */
		{
			bool bNewVal = (State == MPR_STA_ENABLES) ? true : false;

			if(Value & MPR_SE_SCISSORING)
				StateTableInternal[currentState].SE_SCISSORING = bNewVal;

			if(Value & MPR_SE_MODULATION)
				StateTableInternal[currentState].SE_MODULATION = bNewVal;

			if(Value & MPR_SE_TEXTURING)
				StateTableInternal[currentState].SE_TEXTURING = bNewVal;

			if(Value & MPR_SE_SHADING)
				StateTableInternal[currentState].SE_SHADING = bNewVal;

			if(Value & MPR_SE_FOG)
				StateTableInternal[currentState].SE_FOG = bNewVal;

			if(Value & MPR_SE_PIXEL_MASKING)
				StateTableInternal[currentState].SE_PIXEL_MASKING = bNewVal;

			if(Value & MPR_SE_Z_BUFFERING)
				StateTableInternal[currentState].SE_Z_BUFFERING = bNewVal;

			if(Value & MPR_SE_Z_MASKING)
				StateTableInternal[currentState].SE_Z_MASKING = bNewVal;

			if(Value & MPR_SE_PATTERNING)
				StateTableInternal[currentState].SE_PATTERNING = bNewVal;

			if(Value & MPR_SE_CLIPPING)
				StateTableInternal[currentState].SE_CLIPPING = bNewVal;

			if(Value & MPR_SE_BLENDING)
				StateTableInternal[currentState].SE_BLENDING = bNewVal;

			if(Value & MPR_SE_FILTERING)
				StateTableInternal[currentState].SE_FILTERING = bNewVal;

			if(Value & MPR_SE_TRANSPARENCY)
				StateTableInternal[currentState].SE_TRANSPARENCY = bNewVal;

			if(Value & MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE)
				StateTableInternal[currentState].SE_NON_PERSPECTIVE_CORRECTION_MODE = bNewVal;

			if(Value & MPR_SE_NON_SUBPIXEL_PRECISION_MODE)
				StateTableInternal[currentState].SE_NON_SUBPIXEL_PRECISION_MODE = bNewVal;

			if(Value & MPR_SE_HARDWARE_OFF)
				StateTableInternal[currentState].SE_HARDWARE_OFF = bNewVal;

			if(Value & MPR_SE_PALETTIZED_TEXTURES)
				StateTableInternal[currentState].SE_PALETTIZED_TEXTURES = bNewVal;

			if(Value & MPR_SE_DECAL_TEXTURES)
				StateTableInternal[currentState].SE_DECAL_TEXTURES = bNewVal;

			if(Value & MPR_SE_ANTIALIASING)
				StateTableInternal[currentState].SE_ANTIALIASING = bNewVal;

			if(Value & MPR_SE_DITHERING)
				StateTableInternal[currentState].SE_DITHERING = bNewVal;

			break;
		}
	}
}
		
	public void ClearBuffers( WORD ClearInfo )
{
	#if _CONTEXT_TRACE_ALL
	MonoPrint("ContextMPR::ClearBuffers(0x%X)\n", ClearInfo);
	#endif

	DWORD dwClearFlags = 0;
	if(ClearInfo & MPR_CI_DRAW_BUFFER) dwClearFlags |= D3DCLEAR_TARGET;
	if(ClearInfo & MPR_CI_ZBUFFER) dwClearFlags |= D3DCLEAR_ZBUFFER;

	HRESULT hr = m_pD3DD.Clear(null, null, dwClearFlags, m_colBG, 1.0f, null);
	Debug.Assert(SUCCEEDED(hr));
}

	public void SwapBuffers( WORD SwapInfo );
	public void StartFrame(   )
{
	#if _CONTEXT_TRACE_ALL
	MonoPrint("ContextMPR::StartFrame()\n");
	#endif

	// OW FIXME: is this ok?
	if(m_pCtxDX.SetRenderTarget(m_pRenderTarget))	// returns false if render target unchanged
		UpdateViewport();

	InvalidateState();
//	m_pIB.Lock(true);

	HRESULT hr;
	
	if(m_bRenderTargetHasZBuffer)
		hr = m_pD3DD.Clear(null, null, D3DCLEAR_ZBUFFER, 0, 0.0f, null);   // Clear the ZBuffer

	hr = m_pD3DD.BeginScene();
	if(FAILED(hr))
	{
		MonoPrint("ContextMPR::FinishFrame - BeginScene failed 0x%X\n", hr);

		if(hr == DDERR_SURFACELOST)
		{
			MonoPrint("ContextMPR::StartFrame - Restoring all surfaces\n", hr);

			TheTextureBank.RestoreAll();
			TheTerrTextures.RestoreAll();
			TheFarTextures.RestoreAll();

			hr = m_pD3DD.EndScene();

			if(FAILED(hr))
			{
				MonoPrint("ContextMPR::StartFrame - Retry for BeginScene failed 0x%X\n", hr);
				return;
			}
		}
	}

	#if _DEBUG &&  _CONTEXT_ENABLE_RENDERSTATE_HIGHLIGHT_REPLACE
	if(bEnableRenderStateHighlightReplace)
	{
		Sleep(1000);
		if(GetKeyState(VK_F4) & ~1)
			DebugBreak();
		bRenderStateHighlightReplaceTargetState++;
	}
	#endif

#if NOTHING
#if _DEBUG
	static int n = D3DTFP_LINEAR;
	m_pD3DD.SetTextureStageState(0, D3DTSS_MIPFILTER, n);
	static float f = 0;
	m_pD3DD.SetTextureStageState(0, D3DTSS_MIPMAPLODBIAS, *((LPDWORD) (&f)) );
#endif
#endif
}
	public void FinishFrame( void *lpFnPtr )
{
	#if _CONTEXT_TRACE_ALL
	MonoPrint("ContextMPR::FinishFrame(0x%X)\n", lpFnPtr);
	#endif

	FlushVB();

	HRESULT hr = m_pD3DD.EndScene();
	//Debug.Assert(SUCCEEDED(hr));
	if(FAILED(hr))
	{
		MonoPrint("ContextMPR::FinishFrame - EndScene failed 0x%X\n", hr);

		if(hr == DDERR_SURFACELOST)
		{
			MonoPrint("ContextMPR::FinishFrame - Restoring all surfaces\n", hr);

			TheTextureBank.RestoreAll();
			TheTerrTextures.RestoreAll();
			TheFarTextures.RestoreAll();

			hr = m_pD3DD.EndScene();

			if(FAILED(hr))
			{
				MonoPrint("ContextMPR::FinishFrame - Retry for EndScene failed 0x%X\n", hr);
				return;
			}
		}
	}

	#if _CONTEXT_ENABLE_STATS
	m_stats.StartFrame();
	#if NOTHING
	char buf[20];
	sprintf(buf, "%.2d FPS", m_stats.dwLastFPS);
	TextOut(0, 40, RGB(0xff, 0xff, 0xff), (char *) buf);
	#endif
	#endif

//	m_pIB.Unlock();

	Debug.Assert(lpFnPtr == null);		// no idea what to do with it
	Stats();
}
	public void SetColorCorrection( DWORD color, float percent )
{
	SetState( MPR_STA_RAMP_COLOR, color);
	SetState( MPR_STA_RAMP_PERCENT, *(DWORD*)(&percent));
}

	public void SetupMPRState(GLint flag=0)
{
	if(flag & CHECK_PREVIOUS_STATE)
	{
		StateSetupCounter++;
		if(StateSetupCounter > 1)
			return;
	}

	else if(StateSetupCounter)
		CleanupMPRState();

	// Record one stateblock per poly type
	MonoPrint("ContextMPR - Setting up state table\n");
	for (currentState=STATE_SOLID; currentState<MAXIMUM_MPR_STATE; currentState++)
		SetStateTable(currentState, flag );

	InvalidateState ();
}

	public void SelectForegroundColor (GLint color)
{
	if(color != m_colFG_Raw)
	{
		m_colFG_Raw = color;
		m_colFG = MPRColor2D3DRGBA(color);
	}
}
	public void SelectBackgroundColor (GLint color)
	{
	if(color != m_colBG_Raw)
	{
		m_colBG_Raw = color;
		m_colBG = MPRColor2D3DRGBA(color);
	}
}

	public void SelectTexture(GLint texID)
		{
	#if _CONTEXT_TRACE_ALL
	MonoPrint("ContextMPR::SelectTexture(0x%X)\n", texID);
	#endif

	if (g_bSlowButSafe && F4IsBadReadPtr((TextureHandle *) texID, sizeof(TextureHandle))) // JB 010326 CTD (too much CPU)
		return;

	if(texID)
		texID = (GLint) ((TextureHandle *) texID).m_pDDS;

	if(texID != currentTexture)
	{
		FlushVB();

		#if _CONTEXT_ENABLE_STATS
		m_stats.PutTexture(false);
		#endif // _CONTEXT_ENABLE_STATS

		currentTexture = texID;

		// m_pD3DD.PreLoad((IDirectDrawSurface7 *) texID);

		HRESULT hr = m_pD3DD.SetTexture(0, (IDirectDrawSurface7 *) texID); 	// Make sure all textures get released
		Debug.Assert(SUCCEEDED(hr));
	}

	#if _CONTEXT_ENABLE_STATS
	else m_stats.PutTexture(true);
	#endif // _CONTEXT_ENABLE_STATS
}
	public void RestoreState(GLint state)
{
	Debug.Assert(state != -1);		// Use InvalidateState() !
	Debug.Assert(state >= 0 && state < MAXIMUM_MPR_STATE);

	#if  _DEBUG &&  _CONTEXT_ENABLE_RENDERSTATE_HIGHLIGHT_REPLACE
	if(GetKeyState(VK_F4) & ~1)
	{
		if(!bEnableRenderStateHighlightReplace)
			bEnableRenderStateHighlightReplace = true;
	}
	
	if(bEnableRenderStateHighlightReplace && (state == bRenderStateHighlightReplaceTargetState))
	{
		state = STATE_SOLID;
		m_colFG = 0xffff0000;
		currentState = -1;
	}
	#endif

	if(state != currentState)
	{
		#if _CONTEXT_TRACE_ALL
		MonoPrint("ContextMPR::RestoreState(%d)\n", state);
		#endif

		#if _CONTEXT_RECORD_USED_STATES
		m_setStatesUsed.insert(state);
		#endif

		FlushVB();

		// if texturing gets disabled by this state change, force texture reload
		if(currentState == -1 || (StateTableInternal[currentState].SE_TEXTURING && !StateTableInternal[state].SE_TEXTURING))
			currentTexture = -1;

		currentState = state;

		HRESULT hr = m_pD3DD.ApplyStateBlock(StateTable[currentState]);
		Debug.Assert(SUCCEEDED(hr));
	}
}

	public void InvalidateState () { currentTexture = -1; m_colFG_Raw = 0x00ffffff; currentState = -1; } // JPO -1 for FG is white, which happens!
	public int CurrentForegroundColor( ) {return m_colFG_Raw;}
	public void Render2DBitmap( int sX, int sY, int dX, int dY, int w, int h, int totalWidth, DWORD *pSrc )
{
	DWORD *pDst = null;

	#if _CONTEXT_TRACE_ALL
	MonoPrint("ContextMPR::Render2DBitmap(%d, %d, %d, %d, %d, %d, %d, 0x%X)\n", sX, sY, dX, dY, w, h, totalWidth, pSrc);
	#endif
	//TODO Debug.Assert(false == F4IsBadReadPtr(m_pD3DD, sizeof *m_pD3DD));
	try
	{
			// Convert from ABGR to ARGB ;(
		pSrc = (DWORD *) (((BYTE *) pSrc) + (sY * (totalWidth << 2)) + sX);
		pDst = new DWORD[w * h];
		if(!pDst) throw _com_error(E_OUTOFMEMORY);

		for(int y=0;y<h;y++)
		{
			for(int x=0;x<w;x++)
				pDst[y * w + x] = RGBA_MAKE(RGBA_GETBLUE(pSrc[x]), RGBA_GETGREEN(pSrc[x]),
					RGBA_GETRED(pSrc[x]), RGBA_GETALPHA(pSrc[x]));

			pSrc = (DWORD *) (((BYTE *) pSrc) + (totalWidth << 2));
		}

		// Create tmp texture
		DWORD dwFlags = D3DX_TEXTURE_NOMIPMAP;
		DWORD dwActualWidth = w;
		DWORD dwActualHeight = h;
		D3DX_SURFACEFORMAT fmt = D3DX_SF_A8R8G8B8;
		DWORD dwNumMipMaps = 0;

		IDirectDrawSurface7Ptr pDDSTex;
		CheckHR(D3DXCreateTexture(m_pD3DD, &dwFlags, &dwActualWidth, &dwActualHeight,
			&fmt, null, &pDDSTex, &dwNumMipMaps));
		//TODO Debug.Assert(false==F4IsBadReadPtr(pDDSTex, sizeof *pDDSTex));
		CheckHR(D3DXLoadTextureFromMemory(m_pD3DD, pDDSTex, 0, pDst, null,
			D3DX_SF_A8R8G8B8, w << 2, null, D3DX_FT_LINEAR));

		// Setup vertices
		TwoDVertex[] pVtx = new TwoDVertex[4];
		ZeroMemory(pVtx, sizeof(pVtx));
		pVtx[0].x = dX; pVtx[0].y = dY; pVtx[0].u = 0.0f; pVtx[0].v = 0.0f;
		pVtx[1].x = dX + w; pVtx[1].y = dY; pVtx[1].u = 1.0f; pVtx[1].v = 0.0f;
		pVtx[2].x = dX + w; pVtx[2].y = dY + h; pVtx[2].u = 1.0f; pVtx[2].v = 1.0f;
		pVtx[3].x = dX; pVtx[3].y = dY + h; pVtx[3].u = 0.0f; pVtx[3].v = 1.0f;

		// Setup state
		RestoreState(STATE_TEXTURE_FILTER);
		CheckHR(m_pD3DD.SetTexture(0, pDDSTex));

		// Render it (finally)
#if TODO
		DrawPrimitive(MPR_PRM_TRIFAN, MPR_VI_COLOR | MPR_VI_TEXTURE, 4, pVtx, sizeof(pVtx[0]));
#endif				
		FlushVB();
		InvalidateState();
	}

	catch(_com_error e)
	{
		MonoPrint("ContextMPR::Render2DBitmap - Error 0x%X\n", e.Error());
	}

	//TODO if(pDst) delete[] pDst;
}

	// This should be protected, but we need it for debugging elsewhere (SCR 12/5/97)
	public static int	StateSetupCounter;

	public class State
	{
		
		public State() { } //TODO? ZeroMemory(this, sizeof(*this)); }

		public bool SE_SHADING;
		public bool SE_TEXTURING;
		public bool SE_MODULATION;
		public bool SE_Z_BUFFERING;
		public bool SE_FOG;
		public bool SE_PIXEL_MASKING;
		public bool SE_Z_MASKING;
		public bool SE_PATTERNING;
		public bool SE_SCISSORING;
		public bool SE_CLIPPING;
		public bool SE_BLENDING;
		public bool SE_FILTERING;
		public bool SE_TRANSPARENCY;
		public bool SE_NON_PERSPECTIVE_CORRECTION_MODE;
		public bool SE_NON_SUBPIXEL_PRECISION_MODE;
		public bool SE_HARDWARE_OFF;
		public bool SE_PALETTIZED_TEXTURES;
		public bool SE_DECAL_TEXTURES;
		public bool SE_ANTIALIASING;
		public bool SE_DITHERING;
	};

	public static UInt32[] StateTable = new UInt[MAXIMUM_MPR_STATE];			// D3D State block table
	public static State[] StateTableInternal= new State[MAXIMUM_MPR_STATE];		// Internal state table
	public bool m_bUseSetStateInternal;

	public int m_colFG_Raw;
	public int m_colBG_Raw;
	public int currentState;
	public int currentTexture;
 
	
	protected void SetStateTable (GLint state, GLint flag)
{
	if (!m_pD3DD)
		return;
		
	// Record a stateblock
	HRESULT hr = m_pD3DD.BeginStateBlock();
	Debug.Assert(SUCCEEDED(hr));

	SetCurrentState (state, flag);

	hr = m_pD3DD.EndStateBlock((DWORD *) &StateTable[state]);
	Debug.Assert(SUCCEEDED(hr) && StateTable[state]);

	// OW - record internal state
	m_bUseSetStateInternal = true;
	SetCurrentState(state, flag);
	m_bUseSetStateInternal = false;
}

	protected void ClearStateTable (GLint state)
{
	HRESULT hr = m_pD3DD.DeleteStateBlock(StateTable[state]);
	Debug.Assert(SUCCEEDED(hr));

	StateTable[state] = 0;
}
// flag & 0x01  -. skip StateSetupCount checking -. reset/set state
// flag & 0x02  -. enable texture filtering
// flag & 0x04  -. enable dithering
	protected void SetCurrentState (GLint state, GLint flag)
{
	UInt32	i = 0;

	#if _CONTEXT_TRACE_ALL
	MonoPrint("ContextMPR::SetCurrentState (%d, 0x%X)\n", state, flag);
	#endif

	// TODO Debug.Assert(false == F4IsBadReadPtr(m_pD3DD, sizeof *m_pD3DD));

	// OW FIXME: optimize
	// see MPRcfg.h (MPR_BILINEAR_CLAMPED)
	m_pD3DD.SetTextureStageState(0, D3DTSS_ADDRESS, D3DTADDRESS_WRAP);

	switch(state)
	{
		case STATE_SOLID:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_TEXTURING | 
				MPR_SE_MODULATION | 
				MPR_SE_SHADING |
				MPR_SE_FOG );

			// Never dither in flat shaded mode...
			SetState (MPR_STA_DISABLES, MPR_SE_DITHERING);
			break;

		case STATE_GOURAUD:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_TEXTURING | 
				MPR_SE_MODULATION | 
				MPR_SE_FOG );

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			SetState (MPR_STA_ENABLES, MPR_SE_SHADING);
			break;

		case STATE_TEXTURE:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING;
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_TEXTURE_LIT:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_MODULATION;
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_TEXTURE_LIT_PERSPECTIVE:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_MODULATION;
			SetState ( MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE );
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_TEXTURE_GOURAUD:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_SHADING | MPR_SE_MODULATION;
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_TEXTURE_TRANSPARENCY:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_TRANSPARENCY;
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_TEXTURE_LIT_TRANSPARENCY:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_TRANSPARENCY | MPR_SE_MODULATION;
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_TEXTURE_LIT_TRANSPARENCY_PERSPECTIVE:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_TRANSPARENCY | MPR_SE_MODULATION;
			SetState ( MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE );
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_TEXTURE_GOURAUD_TRANSPARENCY:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_TRANSPARENCY | 
				MPR_SE_SHADING | MPR_SE_MODULATION;
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_TEXTURE_PERSPECTIVE:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING;
			SetState (	MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE);
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_TEXTURE_GOURAUD_PERSPECTIVE:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_SHADING | MPR_SE_MODULATION;
			SetState (	MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE);
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_TEXTURE_TRANSPARENCY_PERSPECTIVE:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_TRANSPARENCY;
			SetState (	MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE);
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_TEXTURE_GOURAUD_TRANSPARENCY_PERSPECTIVE:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_TRANSPARENCY | 
				MPR_SE_SHADING | MPR_SE_MODULATION;
			SetState (	MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE);
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_ALPHA_SOLID:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_TEXTURING | 
				MPR_SE_MODULATION | 
				MPR_SE_SHADING |
				MPR_SE_FOG );

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			i |= MPR_SE_BLENDING;
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_ALPHA_GOURAUD:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_TEXTURING | 
				MPR_SE_MODULATION | 
				MPR_SE_FOG );

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			i |= MPR_SE_BLENDING;
			i |= MPR_SE_SHADING;
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_ALPHA_TEXTURE:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			i |= MPR_SE_BLENDING;
			i |= MPR_SE_TEXTURING;
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_ALPHA_TEXTURE_LIT:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			i |= MPR_SE_BLENDING;
			i |= MPR_SE_TEXTURING | MPR_SE_MODULATION;
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_ALPHA_TEXTURE_LIT_PERSPECTIVE:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			i |= MPR_SE_BLENDING;
			i |= MPR_SE_TEXTURING | MPR_SE_MODULATION;
			SetState (	MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE);
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_ALPHA_TEXTURE_SMOOTH:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			i |= MPR_SE_BLENDING| MPR_SE_TEXTURING | MPR_SE_SHADING;
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_ALPHA_TEXTURE_SMOOTH_PERSPECTIVE:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			i |= MPR_SE_BLENDING| MPR_SE_TEXTURING | MPR_SE_SHADING;
			SetState (	MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE);
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_ALPHA_TEXTURE_SMOOTH_TRANSPARENCY:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			i |= MPR_SE_BLENDING| MPR_SE_TEXTURING | MPR_SE_SHADING | MPR_SE_TRANSPARENCY;
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_ALPHA_TEXTURE_SMOOTH_TRANSPARENCY_PERSPECTIVE:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			i |= MPR_SE_BLENDING| MPR_SE_TEXTURING | MPR_SE_SHADING | MPR_SE_TRANSPARENCY;
			SetState (	MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE);
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_ALPHA_TEXTURE_GOURAUD:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			i |= MPR_SE_BLENDING;
			i |= MPR_SE_TEXTURING | MPR_SE_SHADING | MPR_SE_MODULATION;
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_ALPHA_TEXTURE_TRANSPARENCY:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			i |= MPR_SE_BLENDING | MPR_SE_TEXTURING | MPR_SE_TRANSPARENCY;
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_ALPHA_TEXTURE_LIT_TRANSPARENCY:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			i |= MPR_SE_BLENDING | MPR_SE_TEXTURING | MPR_SE_TRANSPARENCY;
			i |= MPR_SE_MODULATION;
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_ALPHA_TEXTURE_LIT_TRANSPARENCY_PERSPECTIVE:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			i |= MPR_SE_BLENDING | MPR_SE_TEXTURING | MPR_SE_TRANSPARENCY;
			i |= MPR_SE_MODULATION;
			SetState (	MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE);
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_ALPHA_TEXTURE_GOURAUD_TRANSPARENCY:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			i |= MPR_SE_BLENDING;
			i |= MPR_SE_TEXTURING | MPR_SE_TRANSPARENCY;
			i |= MPR_SE_MODULATION;
			i |= MPR_SE_SHADING;
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_ALPHA_TEXTURE_PERSPECTIVE:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			i |= MPR_SE_BLENDING;
			i |= MPR_SE_TEXTURING;
			SetState (MPR_STA_ENABLES, i);
			SetState ( MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE );
			break;

		case STATE_ALPHA_TEXTURE_GOURAUD_PERSPECTIVE:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			i |= MPR_SE_BLENDING;
			i |= MPR_SE_TEXTURING | MPR_SE_SHADING | MPR_SE_MODULATION;
			SetState (MPR_STA_ENABLES, i);
			SetState ( MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE );

// OW, added 23-07-2000 to fix cloud rendering problem and dont break missile trails
			m_pD3DD.SetTextureStageState(0, D3DTSS_ALPHAOP, D3DTOP_MODULATE);
			break;

		case STATE_ALPHA_TEXTURE_TRANSPARENCY_PERSPECTIVE:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			i |= MPR_SE_BLENDING;
			i |= MPR_SE_TEXTURING | MPR_SE_TRANSPARENCY;
			SetState (MPR_STA_ENABLES, i);
			SetState ( MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE );
			break;

		case STATE_ALPHA_TEXTURE_GOURAUD_TRANSPARENCY_PERSPECTIVE:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			i |= MPR_SE_BLENDING;
			i |= MPR_SE_TEXTURING | MPR_SE_TRANSPARENCY;
			i |= MPR_SE_MODULATION;
			i |= MPR_SE_SHADING;
			SetState (MPR_STA_ENABLES, i);
			SetState ( MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE );
			break;

		case STATE_FOG_TEXTURE:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING);

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_FOG;
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_FOG_TEXTURE_PERSPECTIVE:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING);

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_FOG;
			SetState (MPR_STA_ENABLES, i);
			SetState (	MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE);
			break;

		case STATE_FOG_TEXTURE_TRANSPARENCY:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING);

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_TRANSPARENCY | MPR_SE_FOG;
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_FOG_TEXTURE_TRANSPARENCY_PERSPECTIVE:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING);

			SetState (MPR_STA_ENABLES,	MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_TRANSPARENCY | MPR_SE_FOG;
			SetState (MPR_STA_ENABLES, i);
			SetState ( MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE );
			break;

		case STATE_FOG_TEXTURE_LIT:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_SHADING);

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_MODULATION | MPR_SE_FOG;
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_FOG_TEXTURE_LIT_PERSPECTIVE:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_SHADING);

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_MODULATION | MPR_SE_FOG;
			SetState (MPR_STA_ENABLES, i);
			SetState (	MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE);
			break;

		case STATE_FOG_TEXTURE_LIT_TRANSPARENCY:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_SHADING);

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_MODULATION | MPR_SE_TRANSPARENCY | MPR_SE_FOG;
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_FOG_TEXTURE_LIT_TRANSPARENCY_PERSPECTIVE:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_SHADING);

			SetState (MPR_STA_ENABLES,	MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_MODULATION | MPR_SE_TRANSPARENCY | MPR_SE_FOG;
			SetState (MPR_STA_ENABLES, i);
			SetState ( MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE );
			break;

		case STATE_FOG_TEXTURE_SMOOTH:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING);

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_SHADING | MPR_SE_FOG;
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_FOG_TEXTURE_SMOOTH_PERSPECTIVE:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING);

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_SHADING | MPR_SE_FOG;
			SetState (	MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE);
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_FOG_TEXTURE_SMOOTH_TRANSPARENCY:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING);

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_TRANSPARENCY | MPR_SE_SHADING | MPR_SE_FOG;
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_FOG_TEXTURE_SMOOTH_TRANSPARENCY_PERSPECTIVE:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING);

			SetState (MPR_STA_ENABLES,	MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_TRANSPARENCY | MPR_SE_SHADING | MPR_SE_FOG;
			SetState (MPR_STA_ENABLES, i);
			SetState ( MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE );
			break;

		case STATE_FOG_TEXTURE_GOURAUD:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY);

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_SHADING | MPR_SE_MODULATION | MPR_SE_FOG;
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_FOG_TEXTURE_GOURAUD_PERSPECTIVE:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY);

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_SHADING | MPR_SE_MODULATION | MPR_SE_FOG;
			SetState (	MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE);
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_FOG_TEXTURE_GOURAUD_TRANSPARENCY:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING);

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_TRANSPARENCY | MPR_SE_SHADING | MPR_SE_MODULATION | MPR_SE_FOG;
			SetState (MPR_STA_ENABLES, i);
			break;

		case STATE_FOG_TEXTURE_GOURAUD_TRANSPARENCY_PERSPECTIVE:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING);

			SetState (MPR_STA_ENABLES,	MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_TRANSPARENCY | MPR_SE_SHADING | MPR_SE_MODULATION | MPR_SE_FOG;
			SetState (MPR_STA_ENABLES, i);
			SetState ( MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE );
			break;

		case STATE_TEXTURE_FILTER:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING;
			i |= MPR_SE_PALETTIZED_TEXTURES;
			i |= MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE;
			i |= MPR_SE_FILTERING;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_TEX_FILTER, MPR_TX_BILINEAR);
			break;

		case STATE_TEXTURE_FILTER_PERSPECTIVE:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING;
			i |= MPR_SE_FILTERING;
			SetState ( MPR_STA_ENABLES, i);
			SetState (MPR_STA_TEX_FILTER, MPR_TX_BILINEAR);
			SetState ( MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE );
			break;


		case STATE_TEXTURE_POLYBLEND:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING;
			i |= MPR_SE_MODULATION;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_ALPHA );
			break;

		case STATE_TEXTURE_POLYBLEND_GOURAUD:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING;
			i |= MPR_SE_SHADING;
			i |= MPR_SE_MODULATION;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_ALPHA );
			break;

		case STATE_TEXTURE_FILTER_POLYBLEND:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING;
			i |= MPR_SE_FILTERING;
			i |= MPR_SE_MODULATION;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_ALPHA );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_BILINEAR);
			break;

		case STATE_TEXTURE_FILTER_POLYBLEND_GOURAUD:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING;
			i |= MPR_SE_SHADING;
			i |= MPR_SE_FILTERING;
			i |= MPR_SE_MODULATION;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_ALPHA );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_BILINEAR);
			break;

		case STATE_APT_TEXTURE_FILTER_PERSPECTIVE:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING;
			i |= MPR_SE_BLENDING;
			i |= MPR_SE_FILTERING;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_TEX_FILTER, MPR_TX_BILINEAR);
			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			SetState (MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE);
			break;

		case STATE_APT_FOG_TEXTURE_FILTER_PERSPECTIVE:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING);

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING;
			i |= MPR_SE_BLENDING;
			i |= MPR_SE_FILTERING;
			i |= MPR_SE_FOG;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_TEX_FILTER, MPR_TX_BILINEAR);
			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			SetState (MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE);
			break;

		case STATE_APT_FOG_TEXTURE_PERSPECTIVE:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING);

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING;
			i |= MPR_SE_BLENDING;
			i |= MPR_SE_FOG;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			SetState (MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE);
			break;

		case STATE_APT_FOG_TEXTURE_SMOOTH_PERSPECTIVE:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING);

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING;
			i |= MPR_SE_BLENDING;
			i |= MPR_SE_SHADING;
			i |= MPR_SE_FOG;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			SetState (MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE);
			break;

		// OW
		case STATE_TEXTURE_LIT_FILTER:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_MODULATION;
			i |= MPR_SE_FILTERING;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_TEX_FILTER, MPR_TX_BILINEAR_NOCLAMP);
			break;

		case STATE_TEXTURE_GOURAUD_FILTER:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_SHADING | MPR_SE_MODULATION;
			i |= MPR_SE_FILTERING;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_TEX_FILTER, MPR_TX_BILINEAR_NOCLAMP);
			break;

		case STATE_TEXTURE_TRANSPARENCY_FILTER:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_TRANSPARENCY;
			i |= MPR_SE_FILTERING;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_TEX_FILTER, MPR_TX_BILINEAR_NOCLAMP);
			break;

		case STATE_TEXTURE_LIT_TRANSPARENCY_FILTER:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_TRANSPARENCY | MPR_SE_MODULATION;
			i |= MPR_SE_FILTERING;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_TEX_FILTER, MPR_TX_BILINEAR_NOCLAMP);
			break;

		case STATE_TEXTURE_GOURAUD_TRANSPARENCY_FILTER:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_TRANSPARENCY | 
				MPR_SE_SHADING | MPR_SE_MODULATION;
			i |= MPR_SE_FILTERING;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_TEX_FILTER, MPR_TX_BILINEAR_NOCLAMP);
			break;

		case STATE_TEXTURE_GOURAUD_TRANSPARENCY_PERSPECTIVE_FILTER:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_TRANSPARENCY | 
				MPR_SE_SHADING | MPR_SE_MODULATION;
			SetState (	MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE);
			i |= MPR_SE_FILTERING;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_TEX_FILTER, MPR_TX_BILINEAR_NOCLAMP);
			break;

		case STATE_TEXTURE_LIT_TRANSPARENCY_PERSPECTIVE_FILTER:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_TRANSPARENCY | MPR_SE_MODULATION;
			SetState ( MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE );
			i |= MPR_SE_FILTERING;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_TEX_FILTER, MPR_TX_BILINEAR_NOCLAMP);

		case STATE_TEXTURE_TRANSPARENCY_PERSPECTIVE_FILTER:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_TRANSPARENCY;
			SetState (	MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE);
			i |= MPR_SE_FILTERING;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_TEX_FILTER, MPR_TX_BILINEAR_NOCLAMP);
			break;

		case STATE_TEXTURE_GOURAUD_PERSPECTIVE_FILTER:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_SHADING | MPR_SE_MODULATION;
			SetState (	MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE);
			i |= MPR_SE_FILTERING;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_TEX_FILTER, MPR_TX_BILINEAR_NOCLAMP);
			break;

		case STATE_TEXTURE_LIT_PERSPECTIVE_FILTER:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING | MPR_SE_MODULATION;
			SetState ( MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE );
			i |= MPR_SE_FILTERING;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_TEX_FILTER, MPR_TX_BILINEAR_NOCLAMP);
			break;

		case STATE_TEXTURE_PERSPECTIVE_FILTER:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			i |= MPR_SE_TEXTURING;
			SetState (	MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE);
			i |= MPR_SE_FILTERING;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_TEX_FILTER, MPR_TX_BILINEAR_NOCLAMP);
			break;

		case STATE_ALPHA_TEXTURE_FILTER:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			i |= MPR_SE_BLENDING;
			i |= MPR_SE_TEXTURING;
			i |= MPR_SE_FILTERING;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_TEX_FILTER, MPR_TX_BILINEAR_NOCLAMP);
			break;

		case STATE_ALPHA_TEXTURE_LIT_FILTER:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			i |= MPR_SE_BLENDING;
			i |= MPR_SE_TEXTURING | MPR_SE_MODULATION;
			i |= MPR_SE_FILTERING;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_TEX_FILTER, MPR_TX_BILINEAR_NOCLAMP);
			break;

		case STATE_ALPHA_TEXTURE_SMOOTH_FILTER:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			i |= MPR_SE_BLENDING| MPR_SE_TEXTURING | MPR_SE_SHADING;
			i |= MPR_SE_FILTERING;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_TEX_FILTER, MPR_TX_BILINEAR_NOCLAMP);
			break;

		case STATE_ALPHA_TEXTURE_GOURAUD_FILTER:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			i |= MPR_SE_BLENDING;
			i |= MPR_SE_TEXTURING | MPR_SE_SHADING | MPR_SE_MODULATION;
			i |= MPR_SE_FILTERING;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_TEX_FILTER, MPR_TX_BILINEAR_NOCLAMP);
			break;

		case STATE_ALPHA_TEXTURE_TRANSPARENCY_FILTER:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			i |= MPR_SE_BLENDING | MPR_SE_TEXTURING | MPR_SE_TRANSPARENCY;
			i |= MPR_SE_FILTERING;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_TEX_FILTER, MPR_TX_BILINEAR_NOCLAMP);
			break;

		case STATE_ALPHA_TEXTURE_LIT_TRANSPARENCY_FILTER:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			i |= MPR_SE_BLENDING | MPR_SE_TEXTURING | MPR_SE_TRANSPARENCY;
			i |= MPR_SE_MODULATION;
			i |= MPR_SE_FILTERING;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_TEX_FILTER, MPR_TX_BILINEAR_NOCLAMP);
			break;

		case STATE_ALPHA_TEXTURE_SMOOTH_TRANSPARENCY_FILTER:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			i |= MPR_SE_BLENDING| MPR_SE_TEXTURING | MPR_SE_SHADING | MPR_SE_TRANSPARENCY;
			i |= MPR_SE_FILTERING;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_TEX_FILTER, MPR_TX_BILINEAR_NOCLAMP);
			break;

		case STATE_ALPHA_TEXTURE_GOURAUD_TRANSPARENCY_FILTER:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			i |= MPR_SE_BLENDING;
			i |= MPR_SE_TEXTURING | MPR_SE_TRANSPARENCY;
			i |= MPR_SE_MODULATION;
			i |= MPR_SE_SHADING;
			i |= MPR_SE_FILTERING;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_TEX_FILTER, MPR_TX_BILINEAR_NOCLAMP);
			break;

		case STATE_ALPHA_TEXTURE_PERSPECTIVE_FILTER:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			i |= MPR_SE_BLENDING;
			i |= MPR_SE_TEXTURING;
			SetState ( MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE );
			i |= MPR_SE_FILTERING;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_TEX_FILTER, MPR_TX_BILINEAR_NOCLAMP);
			break;

		case STATE_ALPHA_TEXTURE_LIT_PERSPECTIVE_FILTER:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			i |= MPR_SE_BLENDING;
			i |= MPR_SE_TEXTURING | MPR_SE_MODULATION;
			SetState (	MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE);
			i |= MPR_SE_FILTERING;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_TEX_FILTER, MPR_TX_BILINEAR_NOCLAMP);
			break;

		case STATE_ALPHA_TEXTURE_SMOOTH_PERSPECTIVE_FILTER:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			i |= MPR_SE_BLENDING| MPR_SE_TEXTURING | MPR_SE_SHADING;
			SetState (	MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE);
			i |= MPR_SE_FILTERING;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_TEX_FILTER, MPR_TX_BILINEAR_NOCLAMP);
			break;

		case STATE_ALPHA_TEXTURE_GOURAUD_PERSPECTIVE_FILTER:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			i |= MPR_SE_BLENDING;
			i |= MPR_SE_TEXTURING | MPR_SE_SHADING | MPR_SE_MODULATION;
			SetState ( MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE );
			i |= MPR_SE_FILTERING;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_TEX_FILTER, MPR_TX_BILINEAR_NOCLAMP);
			break;

		case STATE_ALPHA_TEXTURE_TRANSPARENCY_PERSPECTIVE_FILTER:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_TRANSPARENCY |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			i |= MPR_SE_BLENDING;
			i |= MPR_SE_TEXTURING | MPR_SE_SHADING | MPR_SE_MODULATION;
			SetState ( MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE );
			i |= MPR_SE_FILTERING;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_TEX_FILTER, MPR_TX_BILINEAR_NOCLAMP);
			break;

		case STATE_ALPHA_TEXTURE_LIT_TRANSPARENCY_PERSPECTIVE_FILTER:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			i |= MPR_SE_BLENDING | MPR_SE_TEXTURING | MPR_SE_TRANSPARENCY;
			i |= MPR_SE_MODULATION;
			SetState (	MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE);
			i |= MPR_SE_FILTERING;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_TEX_FILTER, MPR_TX_BILINEAR_NOCLAMP);
			break;

		case STATE_ALPHA_TEXTURE_SMOOTH_TRANSPARENCY_PERSPECTIVE_FILTER:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_MODULATION | 
				MPR_SE_SHADING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE | MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			i |= MPR_SE_BLENDING| MPR_SE_TEXTURING | MPR_SE_SHADING | MPR_SE_TRANSPARENCY;
			SetState (	MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE);
			i |= MPR_SE_FILTERING;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_TEX_FILTER, MPR_TX_BILINEAR_NOCLAMP);
			break;

		case STATE_ALPHA_TEXTURE_GOURAUD_TRANSPARENCY_PERSPECTIVE_FILTER:
			SetState(MPR_STA_DISABLES, 
				MPR_SE_Z_BUFFERING |
				MPR_SE_CLIPPING |
				MPR_SE_BLENDING |
				MPR_SE_DITHERING |
				MPR_SE_FILTERING |
				MPR_SE_FOG );

			SetState (MPR_STA_ENABLES,	MPR_SE_PALETTIZED_TEXTURES );
			SetState (MPR_STA_TEX_FILTER, MPR_TX_MIPMAP_NEAREST);
			SetState (MPR_STA_TEX_FUNCTION, MPR_TF_MULTIPLY);

			if(flag & ENABLE_DITHERING_STATE)
				SetState (MPR_STA_ENABLES, MPR_SE_DITHERING);

			SetState (MPR_STA_SRC_BLEND_FUNCTION, MPR_BF_SRC_ALPHA);
			SetState (MPR_STA_DST_BLEND_FUNCTION, MPR_BF_SRC_ALPHA_INV);
			i |= MPR_SE_BLENDING;
			i |= MPR_SE_TEXTURING | MPR_SE_TRANSPARENCY;
			i |= MPR_SE_MODULATION;
			i |= MPR_SE_SHADING;
			SetState ( MPR_STA_DISABLES, MPR_SE_NON_PERSPECTIVE_CORRECTION_MODE );
			i |= MPR_SE_FILTERING;
			SetState (MPR_STA_ENABLES, i);
			SetState (MPR_STA_TEX_FILTER, MPR_TX_BILINEAR_NOCLAMP);
			break;

		default:
			ShiWarning( "BAD OR MISSING CONTEXT STATE" );	// Should never get here!
	}
}
	protected void SetCurrentStateInternal(GLint state, GLint flag);
	protected void CleanupMPRState (GLint flag=0)
{
	if(!StateSetupCounter)
	{
		ShiWarning("MPR not initialized!");
		return;	// mpr is not initialized yet
	}

	if(flag & CHECK_PREVIOUS_STATE)
	{
		StateSetupCounter--;
		if(StateSetupCounter > 0)
			return;
	}

	MonoPrint("ContextMPR - Clearing state table\n");
	for(int i=STATE_SOLID; i<MAXIMUM_MPR_STATE; i++)
		ClearStateTable(i);
}

	protected void SetPrimitiveType(int nType)
			{
	#if _CONTEXT_TRACE_ALL
	MonoPrint("ContextMPR::SetPrimitiveType(%d)\n", nType);
	#endif

	if(m_nCurPrimType != nType)
	{
		// Flush on changed primitive type
		FlushVB();
		m_nCurPrimType = nType;
	}
}


	// OW
	//////////////////////////////////////

	// Attributes
	
	public DXContext *m_pCtxDX;
	
	protected int m_nFrameDepth;		// EndScene is called when this value reaches zero
	protected ImageBuffer *m_pIB;
	protected IDirectDraw7 *m_pDD;
	protected IDirect3DDevice7 *m_pD3DD;
	protected IDirect3DVertexBuffer7 *m_pVB;
	protected IDirectDrawSurface7 *m_pRenderTarget;
	protected DWORD m_colFG;
	protected DWORD m_colBG;
	protected RECT m_rcVP;
	protected bool m_bEnableScissors;
	protected DWORD m_dwVBSize;
	protected WORD *m_pIdx;
	protected DWORD m_dwNumVtx;
	protected DWORD m_dwNumIdx;
	protected DWORD m_dwStartVtx;
	protected DWORD m_dwStartIdx;
	protected short m_nCurPrimType;
	protected WORD m_VtxInfo;		// copy of argument for Primitive() (used during Primitive.StorePrimitiveVertexData phase)
	protected TTLVERTEX *m_pVtx;	// points to locked VB (only used with Primitive() call)
	#if _DEBUG
	protected BYTE *m_pVtxEnd;	// points to end of locked VB (only used with Primitive() call)
	#endif
	protected IDirectDrawSurface7 *m_pDDSP;	// pointer to primary surface
	protected bool m_bNoD3DStatsAvail;	// D3D Texture management stats (dx debug runtime only)
	protected bool m_bLinear2Anisotropic;		// Set anisotropic filtering when bilinear is requested
	protected bool m_bRenderTargetHasZBuffer;
	protected bool m_bViewportLocked;

	// Stats
	protected class Stats
	{
		
		public Stats()
				{
	#if  _CONTEXT_ENABLE_STATS
	Init();
	#endif
}

		// Attributes
		public DWORD dwLastFPS;
		public DWORD dwMinFPS;
		public DWORD dwMaxFPS;
		public DWORD dwAverageFPS;
		public DWORD dwTotalPrimitives;
		public DWORD[] arrPrimitives = new DWORD[6];
		public DWORD dwMaxVtxBatchSize;
		public DWORD dwAvgVtxBatchSize;
		public DWORD dwMaxPrimBatchSize;
		public DWORD dwAvgPrimBatchSize;
		public DWORD dwMaxVtxCountPerSecond;
		public DWORD dwAvgVtxCountPerSecond;
		public DWORD dwMaxPrimCountPerSecond;
		public DWORD dwAvgPrimCountPerSecond;
		public DWORD dwPutTextureTotal;
		public DWORD dwPutTextureCached;

		protected DWORD dwTicks;
		protected DWORD dwTotalSeconds;
		protected DWORD dwCurrentFPS;
		protected DWORD dwTotalFPS;
		protected __int64 dwTotalVtxCount;
		protected __int64 dwTotalPrimCount;
		protected __int64 dwTotalVtxBatchSize;
		protected __int64 dwTotalPrimBatchSize;
		protected DWORD dwTotalBatches;
		protected DWORD dwCurVtxBatchSize;
		protected DWORD dwCurPrimBatchSize;
		protected DWORD dwCurVtxCountPerSecond;
		protected DWORD dwCurPrimCountPerSecond;

		// Implementation
		protected void Check(){
	DWORD Ticks = GetTickCount();

	if(Ticks - dwTicks > 1000)
	{
		dwTicks = Ticks;
		dwLastFPS = dwCurrentFPS;
		dwCurrentFPS = 0;

		if(dwCurPrimCountPerSecond > dwMaxPrimCountPerSecond)
			dwMaxPrimCountPerSecond = dwCurPrimCountPerSecond;

		if(dwCurVtxCountPerSecond > dwMaxVtxCountPerSecond)
			dwMaxVtxCountPerSecond = dwCurVtxCountPerSecond;

		if(dwTotalSeconds)
		{
			dwAvgVtxCountPerSecond = dwTotalVtxCount / dwTotalSeconds;
			dwAvgPrimCountPerSecond = dwTotalPrimCount / dwTotalSeconds;
		}

		if(dwTotalBatches)
		{
			dwAvgVtxBatchSize = dwTotalVtxBatchSize / dwTotalBatches;
			dwAvgPrimBatchSize = dwTotalPrimBatchSize / dwTotalBatches;
		}

		if(dwLastFPS < dwMinFPS)
			dwMinFPS = dwLastFPS;
		else if(dwLastFPS > dwMaxFPS)
			dwMaxFPS = dwLastFPS;

		dwTotalFPS += dwLastFPS;
		dwTotalSeconds++;
		dwAverageFPS = dwTotalFPS / dwTotalSeconds;
	}
}


		
		public void Init()
				{
//TODO	ZeroMemory(this, sizeof(*this));
}

		public void StartFrame(){
	dwCurrentFPS++;
	Check();
}

		
		public void StartBatch()
				{
	dwTotalBatches++;

	if(dwCurVtxBatchSize > dwMaxVtxBatchSize)
		dwMaxVtxBatchSize = dwCurVtxBatchSize;
	dwTotalVtxBatchSize += dwCurVtxBatchSize;
	dwCurVtxBatchSize = 0;

	if(dwCurPrimBatchSize > dwMaxPrimBatchSize)
		dwMaxPrimBatchSize = dwCurPrimBatchSize;
	dwTotalPrimBatchSize += dwCurPrimBatchSize;
	dwCurPrimBatchSize = 0;
}

		public void Primitive(DWORD dwType, DWORD dwNumVtx)
				{
	arrPrimitives[dwType - 1]++;
	dwTotalPrimitives++;
	dwCurPrimCountPerSecond++;
	dwCurVtxCountPerSecond += dwNumVtx;
	dwCurVtxBatchSize += dwNumVtx;
	dwCurPrimBatchSize++;
	dwTotalPrimCount++;
	dwTotalVtxCount += dwNumVtx;
}

		public void PutTexture(bool bCached)
				{
	dwPutTextureTotal++;
	if(bCached) dwPutTextureCached++;
}

		public void Report()
				{
#if TODO
	MonoPrint("Stats report follows\n");

	float fT = dwTotalPrimitives / 100.0f;

	MonoPrint("	MinFPS: %d\n", dwMinFPS);
	MonoPrint("	MaxFPS: %d\n", dwMaxFPS);
	MonoPrint("	AverageFPS: %d\n", dwAverageFPS);
	MonoPrint("	TotalPrimitives: %d\n", dwTotalPrimitives);
	MonoPrint("	Triangle Lists: %d (%.2f %%)\n", arrPrimitives[3], arrPrimitives[3] / fT);
	MonoPrint("	Triangle Strips: %d (%.2f %%)\n", arrPrimitives[4], arrPrimitives[4] / fT);
	MonoPrint("	Triangle Fans: %d (%.2f %%)\n", arrPrimitives[5], arrPrimitives[5] / fT);
	MonoPrint("	Point Lists: %d (%.2f %%)\n", arrPrimitives[0], arrPrimitives[0] / fT);
	MonoPrint("	Line Lists: %d (%.2f %%)\n", arrPrimitives[1], arrPrimitives[1] / fT);
	MonoPrint("	Line Strips: %d (%.2f %%)\n", arrPrimitives[2], arrPrimitives[2] / fT);
	MonoPrint("	AvgVtxBatchSize: %d\n", dwAvgVtxBatchSize);
	MonoPrint("	MaxVtxBatchSize: %d\n", dwMaxVtxBatchSize);
	MonoPrint("	AvgPrimBatchSize: %d\n", dwAvgPrimBatchSize);
	MonoPrint("	MaxPrimBatchSize: %d\n", dwMaxPrimBatchSize);
	MonoPrint("	AvgVtxCountPerSecond: %d\n", dwAvgVtxCountPerSecond);
	MonoPrint("	MaxVtxCountPerSecond: %d\n", dwMaxVtxCountPerSecond);
	MonoPrint("	AvgPrimCountPerSecond: %d\n", dwAvgPrimCountPerSecond);
	MonoPrint("	MaxPrimCountPerSecond: %d\n", dwMaxPrimCountPerSecond);
	MonoPrint("	TextureChangesTotal: %d\n", dwPutTextureTotal);
	MonoPrint("	TextureChangesCached: %d (%.2f %%)\n", dwPutTextureCached, (float) dwPutTextureCached / (dwPutTextureTotal / 100.0f));

	MonoPrint("End of stats report\n");
#endif
			}

	};

	protected Stats m_stats;

	// Implementation
	protected   DWORD MPRColor2D3DRGBA(GLint color)
{
	return RGBA_MAKE(RGBA_GETBLUE(color), RGBA_GETGREEN(color), RGBA_GETRED(color), RGBA_GETALPHA(color));
}
	protected   void UpdateViewport()
{
	#if _CONTEXT_TRACE_ALL
	MonoPrint("ContextMPR::UpdateViewport()\n");
	#endif

	if(m_bViewportLocked || !m_pD3DD)
		return;

	// get current viewport
	D3DVIEWPORT7 vp;
	HRESULT hr = m_pD3DD.GetViewport(&vp);
	Debug.Assert(SUCCEEDED(hr));
	if(FAILED(hr)) return;	

	if(m_bEnableScissors)
	{
		// Set the viewport to the specified dimensions
		vp.dwX = m_rcVP.left;
		vp.dwY = m_rcVP.top;
		vp.dwWidth = m_rcVP.right - m_rcVP.left;
		vp.dwHeight = m_rcVP.bottom - m_rcVP.top;

		if(!vp.dwWidth || !vp.dwHeight)
			return;		// incomplete
	}

	else
	{
		// Set the viewport to the full target surface dimensions
		DDSURFACEDESC2 ddsd;
		ZeroMemory(&ddsd, sizeof(ddsd));
		ddsd.dwSize = sizeof(ddsd);
		hr = m_pRenderTarget.GetSurfaceDesc(&ddsd);
		Debug.Assert(SUCCEEDED(hr));
		if(FAILED(hr)) return;	

		vp.dwX = 0;
		vp.dwY = 0;
		vp.dwWidth = ddsd.dwWidth;
		vp.dwHeight = ddsd.dwHeight;
	}	

	hr = m_pD3DD.SetViewport(&vp);
	Debug.Assert(SUCCEEDED(hr));
}

	protected   bool LockVB(int nVtxCount, void **p)
			{
	#if _CONTEXT_TRACE_ALL
	MonoPrint("ContextMPR::LockVB(%d, 0x%X) (m_dwStartVtx = %d, m_dwNumVtx = %d)\n", nVtxCount, p, m_dwStartVtx, m_dwNumVtx);
	#endif

	HRESULT hr;
	DWORD dwSize = 0;

	//TODO Debug.Assert(false == F4IsBadReadPtr (m_pVB, sizeof *m_pVB));
	// Check for VB overflow
	if((m_dwStartVtx + m_dwNumVtx + nVtxCount) >= m_dwVBSize)
	{
		// would overflow
		FlushVB();
		m_dwStartVtx = 0;

		// we are done with this VB, hint driver that he can use a another memory block to prevent breaking DMA activity
		hr = m_pVB.Lock(DDLOCK_SURFACEMEMORYPTR | DDLOCK_WRITEONLY | DDLOCK_WAIT | 
			DDLOCK_DISCARDCONTENTS, p, &dwSize);
	}

	else if(m_pVtx)
	{
		// already locked, excellent
		return true;
	}

	else
	{
		// we will only append data, dont interrupt DMA
		if(m_dwStartVtx) hr = m_pVB.Lock(DDLOCK_SURFACEMEMORYPTR | DDLOCK_WRITEONLY | DDLOCK_WAIT | 
				DDLOCK_NOOVERWRITE, p, &dwSize);

		// ok this is the first lock
		else hr = m_pVB.Lock(DDLOCK_SURFACEMEMORYPTR | DDLOCK_WRITEONLY | DDLOCK_WAIT | 
				DDLOCK_DISCARDCONTENTS, p, &dwSize);
	}

	Debug.Assert(SUCCEEDED(hr));

	#if _DEBUG
	if(SUCCEEDED(hr)) m_pVtxEnd = (BYTE *) *p + dwSize;
	else m_pVtxEnd = null;
	#endif

	return SUCCEEDED(hr);
}

	protected   void UnlockVB()
			{
	#if _CONTEXT_TRACE_ALL
	MonoPrint("ContextMPR::UnlockVB()\n");
	#endif

	// Unlock VB
	HRESULT hr = m_pVB.Unlock();
	Debug.Assert(SUCCEEDED(hr));
	m_pVtx = null;
}

	protected   void FlushVB()
			{
	if(!m_dwNumVtx) return;
	Debug.Assert(m_nCurPrimType != 0);

	#if  _CONTEXT_TRACE_ALL
	MonoPrint("ContextMPR::FlushVB()\n");
	#endif

	int nPrimType = m_nCurPrimType;

	// Convert triangle fans to triangle lists to make them batchable
	if(nPrimType == D3DPT_TRIANGLEFAN)
	{
		nPrimType = D3DPT_TRIANGLELIST;
		Debug.Assert(m_dwNumIdx);
	}

	// Convert line strips to line lists to make them batchable
	else if(nPrimType == D3DPT_LINESTRIP)
	{
		nPrimType = D3DPT_LINELIST;
		Debug.Assert(m_dwNumIdx);
	}

	HRESULT hr;

	UnlockVB();

#if _VALIDATE_DEVICE
	if(!m_pCtxDX.ValidateD3DDevice())
		MonoPrint("ContextMPR::FlushVB() - Validate Device failed - currentState=%d, currentTexture=0x%\n",
			currentState, currentTexture);
	#endif

	#if _CONTEXT_ENABLE_RENDERSTATE_HIGHLIGHT
	//if(currentState == STATE_ALPHA_GOURAUD)
	{
		hr = m_pD3DD.ApplyStateBlock(StateTable[STATE_SOLID]);
		Debug.Assert(SUCCEEDED(hr));
	}
	#endif

	if(m_dwNumIdx) hr = m_pD3DD.DrawIndexedPrimitiveVB((D3DPRIMITIVETYPE) nPrimType,
		m_pVB, m_dwStartVtx, m_dwNumVtx, m_pIdx, m_dwNumIdx, null);

	else hr = m_pD3DD.DrawPrimitiveVB((D3DPRIMITIVETYPE) nPrimType,
		m_pVB, m_dwStartVtx, m_dwNumVtx, null);

	Debug.Assert(SUCCEEDED(hr));

	#if _CONTEXT_ENABLE_STATS
	m_stats.StartBatch();
	#endif // _CONTEXT_ENABLE_STATS

	m_dwStartVtx += m_dwNumVtx;
	m_dwNumVtx = 0;
	m_dwNumIdx = 0;
	m_VtxInfo = 0;		// invalid
}

	protected static HRESULT EnumSurfacesCB2(IDirectDrawSurface7 *lpDDSurface, _DDSURFACEDESC2 *lpDDSurfaceDesc, LPVOID lpContext)
{
	ContextMPR *pThis = (ContextMPR *) lpContext;
	//TODO Debug.Assert(false == F4IsBadReadPtr(pThis, sizeof *pThis));
	//TODO Debug.Assert(false == F4IsBadReadPtr(lpDDSurfaceDesc, sizeof *lpDDSurfaceDesc));

	if(lpDDSurfaceDesc.ddsCaps.dwCaps & DDSCAPS_PRIMARYSURFACE)
	{
		pThis.m_pDDSP = lpDDSurface;	// already addref'd by ddraw
		return DDENUMRET_CANCEL;	// we got what we wanted
	}

	return DDENUMRET_OK;	// continue
}

	// New Interface
	public void DrawPoly(DWORD opFlag, Poly *poly, int *xyzIdxPtr, int *rgbaIdxPtr, int *IIdxPtr, Ptexcoord *uv, bool bUseFGColor = false)
			{
	// Note: incoming type is always MPR_PRM_TRIFAN
	// OW FIXME: optimize loop
	//TODO Debug.Assert(false == F4IsBadReadPtr(poly, sizeof *poly));
	Debug.Assert(poly.nVerts >= 3);
	Debug.Assert(xyzIdxPtr);
	Debug.Assert(!bUseFGColor || (bUseFGColor && rgbaIdxPtr == null));

	#if _CONTEXT_TRACE_ALL
	MonoPrint("ContextMPR::DrawPoly(0x%X, 0x%X, 0x%X, 0x%X, 0x%X, 0x%X, %s)\n",
		opFlag, poly, xyzIdxPtr, rgbaIdxPtr, IIdxPtr, uv, bUseFGColor ? "true" : "false");
	#endif

	SetPrimitiveType(D3DPT_TRIANGLEFAN);

	#if _CONTEXT_ENABLE_STATS
	m_stats.Primitive(m_nCurPrimType, poly.nVerts);
	#endif // _CONTEXT_ENABLE_STATS

	TTLVERTEX *pVtx;	// points to locked VB
	if(!LockVB(poly.nVerts, (void **) &m_pVtx)) return; 	// Lock VB

	Pcolor *rgba;
	Tpoint *xyz;
	float *I;
	Pcolor foggedColor;

	//TODO Debug.Assert(false == F4IsBadWritePtr(m_pVtx, sizeof *m_pVtx));	
	Debug.Assert(m_dwStartVtx < m_dwVBSize);
	pVtx = &m_pVtx[m_dwStartVtx + m_dwNumVtx];
	//TODO Debug.Assert(false == F4IsBadWritePtr(pVtx, poly.nVerts * sizeof *pVtx));

	if (!pVtx) // JB 011124 CTD
		return;

	// Iterate for each vertex
	for(int i=0;i<poly.nVerts;i++)
	{
		Debug.Assert((BYTE *) pVtx < m_pVtxEnd);	// check for overrun

		/*if (F4IsBadWritePtr(pVtx, sizeof(TTLVERTEX))) // JB 010222 CTD (too much CPU)
		{
			pVtx++;
			continue;
		}*/

		xyz  = &TheStateStack.XformedPosPool[*xyzIdxPtr++];

		/*if (F4IsBadReadPtr(xyz, sizeof(Tpoint))) // JB 010317 CTD (too much CPU)
		{
			pVtx++;
			continue;
		}*/

		pVtx.sx = xyz.x;
		pVtx.sy = xyz.y;
		pVtx.sz = 1.0;
		if (xyz.z) // JB 010222
			pVtx.rhw = 1.0f / xyz.z;
		pVtx.color = 0xffffffff;	// do not remove
		Debug.Assert(xyz.z != 0.0f);

		if(opFlag & PRIM_COLOP_COLOR)
		{
			Debug.Assert(rgbaIdxPtr);
			rgba = &TheColorBank.ColorPool[*rgbaIdxPtr++];
			
			Debug.Assert(rgba);
			if (rgba)
			{
				if((opFlag & PRIM_COLOP_FOG) && !(opFlag & PRIM_COLOP_TEXTURE))
				{
					foggedColor.r = rgba.r * TheStateStack.fogValue_inv + TheStateStack.fogColor_premul.r;
					foggedColor.g = rgba.g * TheStateStack.fogValue_inv + TheStateStack.fogColor_premul.g;
					foggedColor.b = rgba.b * TheStateStack.fogValue_inv + TheStateStack.fogColor_premul.b;
					foggedColor.a = rgba.a;
					rgba = &foggedColor;
				}

				if(opFlag & PRIM_COLOP_INTENSITY)
				{
					Debug.Assert(IIdxPtr);
					I = &TheStateStack.IntensityPool[*IIdxPtr++];
					// Debug.Assert((rgba.r * *I <= 1.0f) && (rgba.g * *I <= 1.0f) && (rgba.b * *I <= 1.0f) && (rgba.a <= 1.0f));

					pVtx.color = D3DRGBA(rgba.r * *I, rgba.g * *I, rgba.b * *I, rgba.a);
				}

				else
				{
					// Debug.Assert((rgba.r <= 1.0f) && (rgba.g <= 1.0f) && (rgba.b <= 1.0f) && (rgba.a <= 1.0f));
					pVtx.color = D3DRGBA(rgba.r, rgba.g, rgba.b, rgba.a);
				}
			}
		}

		else if(opFlag & PRIM_COLOP_INTENSITY)
		{
			Debug.Assert(IIdxPtr);

			I = &TheStateStack.IntensityPool[*IIdxPtr++];
			// Debug.Assert(*I <= 1.0f);
			pVtx.color = D3DRGBA(*I, *I, *I, 1.0f);
		}

		if(opFlag & PRIM_COLOP_TEXTURE)
		{
			Debug.Assert(uv);

			// OW FIXME
			// pVtx.q = xyz.z;
			pVtx.tu = uv.u;
			pVtx.tv = uv.v;
			uv++;

			if(opFlag & PRIM_COLOP_FOG) 
				*(3+ (BYTE *) &pVtx.color) = (BYTE) (TheStateStack.fogValue * 255);	// VB locked write only!!
		}

		else
		{
			// OW FIXME: should be
			pVtx.tu = 0;
			pVtx.tv = 0;
		}

		if(bUseFGColor)
			pVtx.color = m_colFG;

		#if _CONTEXT_ENABLE_RENDERSTATE_HIGHLIGHT
		pVtx.color = currentState != -1 ? RGBA_MAKE((currentState << 1) + 50, (currentState << 1) + 50, (currentState << 1) + 50, (currentState << 1) + 50) : D3DRGBA(1.0f, 1.0f, 1.0f, 1.0f);
		#endif

		pVtx++;
	}

	// Generate Indices
	WORD *pIdx = &m_pIdx[m_dwNumIdx];

	for(int x=0;x<poly.nVerts-2;x++)
	{
		pIdx[0] = m_dwNumVtx;
		pIdx[1] = m_dwNumVtx + x + 1;
		pIdx[2] = m_dwNumVtx + x + 2;
		pIdx += 3;
	}

	m_dwNumIdx += pIdx - &m_pIdx[m_dwNumIdx];
	m_dwNumVtx += poly.nVerts;

	#if _CONTEXT_FLUSH_EVERY_PRIMITIVE
	FlushVB();
	#endif
}

	public void Draw2DPoint(Tpoint *v0)
			{
	Debug.Assert(v0);

	#if _CONTEXT_TRACE_ALL
	MonoPrint("ContextMPR::Draw2DPoint(0x%X)\n", v0);
	#endif

	SetPrimitiveType(D3DPT_POINTLIST);

	#if _CONTEXT_ENABLE_STATS
	m_stats.Primitive(m_nCurPrimType, 1);
	#endif // _CONTEXT_ENABLE_STATS

	TTLVERTEX *pVtx;	// points to locked VB
	if(!LockVB(1, (void **) &m_pVtx)) return; 	// Lock VB

	//TODO Debug.Assert(false == F4IsBadWritePtr(m_pVtx, sizeof *m_pVtx));	
	Debug.Assert(m_dwStartVtx < m_dwVBSize);
	pVtx = &m_pVtx[m_dwStartVtx + m_dwNumVtx];
	//TODO Debug.Assert(false == F4IsBadWritePtr(pVtx, sizeof *pVtx));

	Debug.Assert((BYTE *) pVtx < m_pVtxEnd);	// check for overrun

	pVtx.sx = v0.x;
	pVtx.sy = v0.y;
	pVtx.sz = 1.0;
	pVtx.rhw = 1.0f;
	pVtx.color = m_colFG;
	pVtx.tu = 0;
	pVtx.tv = 0;

	#if _CONTEXT_ENABLE_RENDERSTATE_HIGHLIGHT
	pVtx.color = currentState != -1 ? RGBA_MAKE((currentState << 1) + 50, (currentState << 1) + 50, (currentState << 1) + 50, (currentState << 1) + 50) : D3DRGBA(1.0f, 1.0f, 1.0f, 1.0f);
	#endif

	m_dwNumVtx++;

	#if _CONTEXT_FLUSH_EVERY_PRIMITIVE
	FlushVB();
	#endif
}

	public void Draw2DPoint(float x, float y)
			{
	#if _CONTEXT_TRACE_ALL
	MonoPrint("ContextMPR::Draw2DPoint(%f, %f)\n", x, y);
	#endif

	SetPrimitiveType(D3DPT_POINTLIST);

	#if _CONTEXT_ENABLE_STATS
	m_stats.Primitive(m_nCurPrimType, 1);
	#endif // _CONTEXT_ENABLE_STATS

	TTLVERTEX *pVtx;	// points to locked VB
	if(!LockVB(1, (void **) &m_pVtx)) return; 	// Lock VB

	//TODO Debug.Assert(false == F4IsBadWritePtr(m_pVtx, sizeof *m_pVtx));	
	Debug.Assert(m_dwStartVtx < m_dwVBSize);
	pVtx = &m_pVtx[m_dwStartVtx + m_dwNumVtx];

	Debug.Assert((BYTE *) pVtx < m_pVtxEnd);	// check for overrun
	//TODO Debug.Assert(false == F4IsBadWritePtr(pVtx, sizeof *pVtx));

	pVtx.sx = x;
	pVtx.sy = y;
	pVtx.sz = 1.0;
	pVtx.rhw = 1.0f;
	pVtx.color = m_colFG;
	pVtx.tu = 0;
	pVtx.tv = 0;

	#if _CONTEXT_ENABLE_RENDERSTATE_HIGHLIGHT
	pVtx.color = currentState != -1 ? RGBA_MAKE((currentState << 1) + 50, (currentState << 1) + 50, (currentState << 1) + 50, (currentState << 1) + 50) : D3DRGBA(1.0f, 1.0f, 1.0f, 1.0f);
	#endif

	m_dwNumVtx++;

	#if _CONTEXT_FLUSH_EVERY_PRIMITIVE
	FlushVB();
	#endif
}

	public void Draw2DLine(Tpoint *v0, Tpoint *v1)
			{
	Debug.Assert(v0 && v1);

	#if  _CONTEXT_TRACE_ALL
	MonoPrint("ContextMPR::Draw2DLine(0x%X, 0x%X)\n", v0, v1);
	#endif

	SetPrimitiveType(D3DPT_LINESTRIP);

	#if  _CONTEXT_ENABLE_STATS
	m_stats.Primitive(m_nCurPrimType, 2);
	#endif // _CONTEXT_ENABLE_STATS

	TTLVERTEX *pVtx;	// points to locked VB
	if(!LockVB(2, (void **) &m_pVtx)) return; 	// Lock VB

	//TODO Debug.Assert(false == F4IsBadWritePtr(m_pVtx, sizeof *m_pVtx));	
	Debug.Assert(m_dwStartVtx < m_dwVBSize);
	pVtx = &m_pVtx[m_dwStartVtx + m_dwNumVtx];

	Debug.Assert((BYTE *) pVtx < m_pVtxEnd);	// check for overrun
	//TODO Debug.Assert(false == F4IsBadWritePtr(pVtx, 2*sizeof *pVtx));

	/*if (F4IsBadWritePtr(pVtx, sizeof(TTLVERTEX)) || F4IsBadReadPtr(v0, sizeof(Tpoint))) // JB 010305 CTD (too much CPU)
		return;*/

	pVtx.sx = v0.x;
	pVtx.sy = v0.y;
	pVtx.sz = 1.0;
	if (v0.z) // JB 010220 CTD
		pVtx.rhw = 1.0f / v0.z;
	pVtx.color = m_colFG;
	pVtx.tu = 0;
	pVtx.tv = 0;
	pVtx++;

	#if  _CONTEXT_ENABLE_RENDERSTATE_HIGHLIGHT
	pVtx.color = currentState != -1 ? RGBA_MAKE((currentState << 1) + 50, (currentState << 1) + 50, (currentState << 1) + 50, (currentState << 1) + 50) : D3DRGBA(1.0f, 1.0f, 1.0f, 1.0f);
	#endif

	pVtx.sx = v1.x;
	pVtx.sy = v1.y;
	pVtx.sz = 1.0;
	pVtx.rhw = 1.0f / v1.z;
	pVtx.color = m_colFG;
	pVtx.tu = 0;
	pVtx.tv = 0;

	#if  _CONTEXT_ENABLE_RENDERSTATE_HIGHLIGHT
	pVtx.color = currentState != -1 ? RGBA_MAKE((currentState << 1) + 50, (currentState << 1) + 50, (currentState << 1) + 50, (currentState << 1) + 50) : D3DRGBA(1.0f, 1.0f, 1.0f, 1.0f);
	#endif

	WORD *pIdx = &m_pIdx[m_dwNumIdx];
	*pIdx++ = m_dwNumVtx;
	*pIdx++ = m_dwNumVtx + 1;

	m_dwNumIdx += 2;
	m_dwNumVtx += 2;

	#if  _CONTEXT_FLUSH_EVERY_PRIMITIVE
	FlushVB();
	#endif
}

	public void Draw2DLine(float x0, float y0, float x1, float y1)
			{
	#if  _CONTEXT_TRACE_ALL
	MonoPrint("ContextMPR::Draw2DLine(0x%X, 0x%X)\n", v0, v1);
	#endif

	SetPrimitiveType(D3DPT_LINESTRIP);

	#if  _CONTEXT_ENABLE_STATS
	m_stats.Primitive(m_nCurPrimType, 2);
	#endif // _CONTEXT_ENABLE_STATS

	TTLVERTEX *pVtx;	// points to locked VB
	if(!LockVB(2, (void **) &m_pVtx)) return; 	// Lock VB

	//TODO Debug.Assert(false == F4IsBadWritePtr(m_pVtx, sizeof *m_pVtx));	
	Debug.Assert(m_dwStartVtx < m_dwVBSize);
	pVtx = &m_pVtx[m_dwStartVtx + m_dwNumVtx];
	//TODO Debug.Assert(false == F4IsBadWritePtr(pVtx, 2* sizeof *pVtx));

	Debug.Assert((BYTE *) pVtx < m_pVtxEnd);	// check for overrun

	pVtx.sx = x0;
	pVtx.sy = y0;
	pVtx.sz = 1.0;
	pVtx.rhw = 1.0f;
	pVtx.color = m_colFG;
	pVtx.tu = 0;
	pVtx.tv = 0;
	pVtx++;

	/*if (F4IsBadWritePtr(pVtx, sizeof(TTLVERTEX))) // JB 010222 CTD (too much CPU)
		return; // JB 010222 CTD*/

	#if  _CONTEXT_ENABLE_RENDERSTATE_HIGHLIGHT
	pVtx.color = currentState != -1 ? RGBA_MAKE((currentState << 1) + 50, (currentState << 1) + 50, (currentState << 1) + 50, (currentState << 1) + 50) : D3DRGBA(1.0f, 1.0f, 1.0f, 1.0f);
	#endif

	pVtx.sx = x1;
	pVtx.sy = y1;
	pVtx.sz = 1.0;
	pVtx.rhw = 1.0f;
	pVtx.color = m_colFG;
	pVtx.tu = 0;
	pVtx.tv = 0;

	#if  _CONTEXT_ENABLE_RENDERSTATE_HIGHLIGHT
	pVtx.color = currentState != -1 ? RGBA_MAKE((currentState << 1) + 50, (currentState << 1) + 50, (currentState << 1) + 50, (currentState << 1) + 50) : D3DRGBA(1.0f, 1.0f, 1.0f, 1.0f);
	#endif

	WORD *pIdx = &m_pIdx[m_dwNumIdx];
	*pIdx++ = m_dwNumVtx;
	*pIdx++ = m_dwNumVtx + 1;

	m_dwNumIdx += 2;
	m_dwNumVtx += 2;

	#if  _CONTEXT_FLUSH_EVERY_PRIMITIVE
	FlushVB();
	#endif
}
	
	public void DrawPrimitive2D(int type, int nVerts, int *xyzIdxPtr)
			{
	Debug.Assert(xyzIdxPtr);

	#if  _CONTEXT_TRACE_ALL
	MonoPrint("ContextMPR::DrawPrimitive2D(%d, %d, 0x%X)\n", type, nVerts, xyzIdxPtr);
	#endif

	SetPrimitiveType(type == LineF ? D3DPT_LINESTRIP : D3DPT_POINTLIST);

	#if  _CONTEXT_ENABLE_STATS
	m_stats.Primitive(m_nCurPrimType, nVerts);
	#endif // _CONTEXT_ENABLE_STATS

	TTLVERTEX *pVtx;	// points to locked VB
	if(!LockVB(nVerts, (void **) &m_pVtx)) return; 	// Lock VB

	//TODO Debug.Assert(false == F4IsBadWritePtr(m_pVtx, sizeof *m_pVtx));	
	Debug.Assert(m_dwStartVtx < m_dwVBSize);
	pVtx = &m_pVtx[m_dwStartVtx + m_dwNumVtx];
	//TODO Debug.Assert(false == F4IsBadWritePtr(pVtx, nVerts * sizeof *pVtx));

	Tpoint *xyz;

	// Iterate for each vertex
	for(int i=0;i<nVerts;i++)
	{
	    Debug.Assert(*xyzIdxPtr < MAX_VERT_POOL_SIZE);
		xyz = &TheStateStack.XformedPosPool[*xyzIdxPtr++];
		Debug.Assert((BYTE *) pVtx < m_pVtxEnd);	// check for overrun

		/*if (F4IsBadWritePtr(pVtx, sizeof(TTLVERTEX)) || F4IsBadReadPtr(xyz, sizeof(Tpoint))) // JB 010305 CTD (too much CPU)
		{
			pVtx++;
			continue;
		}*/

		pVtx.sx = xyz.x;
		pVtx.sy = xyz.y;
		pVtx.sz = 1.0;
		if (xyz.z) // JB 010305
			pVtx.rhw = 1.0f / xyz.z;
		pVtx.color = m_colFG;
		pVtx.tu = 0;
		pVtx.tv = 0;

		#if  _CONTEXT_ENABLE_RENDERSTATE_HIGHLIGHT
		pVtx.color = currentState != -1 ? RGBA_MAKE((currentState << 1) + 50, (currentState << 1) + 50, (currentState << 1) + 50, (currentState << 1) + 50) : D3DRGBA(1.0f, 1.0f, 1.0f, 1.0f);
		#endif

		pVtx++;
	}

	// Generate Indices
	if(m_nCurPrimType == D3DPT_LINESTRIP)
	{
		WORD *pIdx = &m_pIdx[m_dwNumIdx];

		for(int x=0;x<nVerts;x++)
			*pIdx++ = m_dwNumVtx + x;

		m_dwNumIdx += nVerts;
	}

	m_dwNumVtx += nVerts;

	#if  _CONTEXT_FLUSH_EVERY_PRIMITIVE
	FlushVB();
	#endif
}

	public void DrawPrimitive(int type, WORD VtxInfo, WORD Count, MPRVtx_t *data, WORD Stride)
			{
	Debug.Assert(!(VtxInfo & MPR_VI_COLOR));	// impossible
	Debug.Assert((nVerts >=3) || (nPrimType==MPR_PRM_POINTS && nVerts >=1) || (nPrimType<=MPR_PRM_POLYLINE && nVerts >=2));		// Ensure no degenerate nPrimTypeitives

	#if  _CONTEXT_TRACE_ALL
	MonoPrint("ContextMPR::DrawPrimitive(%d, 0x%X, %d, 0x%X, %d)\n", nPrimType, VtxInfo, nVerts, pData, Stride);
	#endif

	SetPrimitiveType(nPrimType);

	#if  _CONTEXT_ENABLE_STATS
	m_stats.Primitive(m_nCurPrimType, nVerts);
	#endif // _CONTEXT_ENABLE_STATS

	TTLVERTEX *pVtx;	// points to locked VB
	if(!LockVB(nVerts, (void **) &m_pVtx)) return; 	// Lock VB

	//TODO Debug.Assert(false == F4IsBadWritePtr(m_pVtx, sizeof *m_pVtx));	
	Debug.Assert(m_dwStartVtx < m_dwVBSize);
	pVtx = &m_pVtx[m_dwStartVtx + m_dwNumVtx];
	//TODO Debug.Assert(false == F4IsBadWritePtr(pVtx, nVerts * sizeof *pVtx));

	// Iterate for each vertex
	for(int i=0;i<nVerts;i++)
	{
		Debug.Assert((BYTE *) pVtx < m_pVtxEnd);	// check for overrun

		pVtx.sx = pData.x;
		pVtx.sy = pData.y;
		pVtx.sz = 0.0f;
		pVtx.rhw = 1.0f;
		pVtx.color = m_colFG;
		pVtx.tu = 0;
		pVtx.tv = 0;

		#if  _CONTEXT_ENABLE_RENDERSTATE_HIGHLIGHT
		pVtx.color = currentState != -1 ? RGBA_MAKE((currentState << 1) + 50, (currentState << 1) + 50, (currentState << 1) + 50, (currentState << 1) + 50) : D3DRGBA(1.0f, 1.0f, 1.0f, 1.0f);
		#endif

		pVtx++;
		pData = (MPRVtx_t *) ((BYTE *) pData + Stride);
	}

	// Generate Indices
	if(m_nCurPrimType == D3DPT_TRIANGLEFAN)
	{
		WORD *pIdx = &m_pIdx[m_dwNumIdx];

		for(int x=0;x<nVerts-2;x++)
		{
			pIdx[0] = m_dwNumVtx;
			pIdx[1] = m_dwNumVtx + x + 1;
			pIdx[2] = m_dwNumVtx + x + 2;
			pIdx += 3;
		}

		m_dwNumIdx += pIdx - &m_pIdx[m_dwNumIdx];
	}

	else if(m_nCurPrimType == D3DPT_LINESTRIP)
	{
		WORD *pIdx = &m_pIdx[m_dwNumIdx];

		for(int x=0;x<nVerts;x++)
			*pIdx++ = m_dwNumVtx + x;

		m_dwNumIdx += nVerts;
	}

	m_dwNumVtx += nVerts;

	#if  _CONTEXT_FLUSH_EVERY_PRIMITIVE
	FlushVB();
	#endif
}

	public void DrawPrimitive(int type, WORD VtxInfo, WORD Count, MPRVtxTexClr_t *data, WORD Stride)
			{
	Debug.Assert((nVerts >=3) || (nPrimType==MPR_PRM_POINTS && nVerts >=1) || (nPrimType<=MPR_PRM_POLYLINE && nVerts >=2));		// Ensure no degenerate nPrimTypeitives

	#if  _CONTEXT_TRACE_ALL
	MonoPrint("ContextMPR::DrawPrimitive2(%d, 0x%X, %d, 0x%X, %d)\n", nPrimType, VtxInfo, nVerts, pData, Stride);
	#endif

	SetPrimitiveType(nPrimType);

	#if  _CONTEXT_ENABLE_STATS
	m_stats.Primitive(m_nCurPrimType, nVerts);
	#endif // _CONTEXT_ENABLE_STATS

	TTLVERTEX *pVtx;	// points to locked VB
	if(!LockVB(nVerts, (void **) &m_pVtx)) return; 	// Lock VB

	//TODO Debug.Assert(false == F4IsBadWritePtr(m_pVtx, sizeof *m_pVtx));	
	Debug.Assert(m_dwStartVtx < m_dwVBSize);
	pVtx = &m_pVtx[m_dwStartVtx + m_dwNumVtx];

	//TODO Debug.Assert(false == F4IsBadWritePtr(pVtx, nVerts * sizeof *pVtx));

	if (!pVtx) // JB 011124 CTD
		return;

	switch(VtxInfo)
	{
		case MPR_VI_COLOR | MPR_VI_TEXTURE:
		{
			// Iterate for each vertex
			for(int i=0;i<nVerts;i++)
			{
				Debug.Assert((BYTE *) pVtx < m_pVtxEnd);	// check for overrun

				/*if (F4IsBadWritePtr(pVtx, sizeof(TTLVERTEX)) || F4IsBadReadPtr(pData, sizeof(MPRVtxTexClr_t))) // JB 010305 CTD (too much CPU)
				{
					pVtx++;
					pData = (MPRVtxTexClr_t *) ((BYTE *) pData + Stride);
					continue;
				}*/

				pVtx.sx = pData.x;
				pVtx.sy = pData.y;
				pVtx.sz = 0.0f;
				pVtx.rhw = 1.0f;	// OW FIXME: this should be 1.0f / pData.z
				pVtx.color = D3DRGBA(pData.r, pData.g, pData.b, pData.a);
				pVtx.tu = pData.u;
				pVtx.tv = pData.v;

				#if  _CONTEXT_ENABLE_RENDERSTATE_HIGHLIGHT
				pVtx.color = currentState != -1 ? RGBA_MAKE((currentState << 1) + 50, (currentState << 1) + 50, (currentState << 1) + 50, (currentState << 1) + 50) : D3DRGBA(1.0f, 1.0f, 1.0f, 1.0f);
				#endif

				pVtx++;
				pData = (MPRVtxTexClr_t *) ((BYTE *) pData + Stride);
			}

			break;
		}

		case MPR_VI_COLOR:
		{
			// Iterate for each vertex
			for(int i=0;i<nVerts;i++)
			{
				Debug.Assert((BYTE *) pVtx < m_pVtxEnd);	// check for overrun

				pVtx.sx = pData.x;
				pVtx.sy = pData.y;
				pVtx.sz = 0.0f;
				pVtx.rhw = 1.0f;	// OW FIXME: this should be 1.0f / pData.z
				pVtx.color = D3DRGBA(pData.r, pData.g, pData.b, pData.a);

				#if  _CONTEXT_ENABLE_RENDERSTATE_HIGHLIGHT
				pVtx.color = currentState != -1 ? RGBA_MAKE((currentState << 1) + 50, (currentState << 1) + 50, (currentState << 1) + 50, (currentState << 1) + 50) : D3DRGBA(1.0f, 1.0f, 1.0f, 1.0f);
				#endif

				pVtx++;
				pData = (MPRVtxTexClr_t *) ((BYTE *) pData + Stride);
			}

			break;
		}

		default:
		{
			Debug.Assert(VtxInfo == null);

			// Iterate for each vertex
			for(int i=0;i<nVerts;i++)
			{
				Debug.Assert((BYTE *) pVtx < m_pVtxEnd);	// check for overrun

				pVtx.sx = pData.x;
				pVtx.sy = pData.y;
				pVtx.sz = 0.0f;
				pVtx.rhw = 1.0f;	// OW FIXME: this should be 1.0f / pData.z
				pVtx.color = m_colFG;

				#if  _CONTEXT_ENABLE_RENDERSTATE_HIGHLIGHT
				pVtx.color = currentState != -1 ? RGBA_MAKE((currentState << 1) + 50, (currentState << 1) + 50, (currentState << 1) + 50, (currentState << 1) + 50) : D3DRGBA(1.0f, 1.0f, 1.0f, 1.0f);
				#endif

				pVtx++;
				pData = (MPRVtxTexClr_t *) ((BYTE *) pData + Stride);
			}

			break;
		}
	}

	// Generate Indices (in advance)
	if(m_nCurPrimType == D3DPT_TRIANGLEFAN)
	{
		WORD *pIdx = &m_pIdx[m_dwNumIdx];

		for(int x=0;x<nVerts-2;x++)
		{
			pIdx[0] = m_dwNumVtx;
			pIdx[1] = m_dwNumVtx + x + 1;
			pIdx[2] = m_dwNumVtx + x + 2;
			pIdx += 3;
		}

		m_dwNumIdx += pIdx - &m_pIdx[m_dwNumIdx];
	}

	else if(m_nCurPrimType == D3DPT_LINESTRIP)
	{
		WORD *pIdx = &m_pIdx[m_dwNumIdx];

		for(int x=0;x<nVerts;x++)
			*pIdx++ = m_dwNumVtx + x;

		m_dwNumIdx += nVerts;
	}

	m_dwNumVtx += nVerts;

	#if  _CONTEXT_FLUSH_EVERY_PRIMITIVE
	FlushVB();
	#endif
}

	public void DrawPrimitive(int type, WORD VtxInfo, WORD Count, MPRVtxTexClr_t **data)
			{
	Debug.Assert((nVerts >=3) || (nPrimType==MPR_PRM_POINTS && nVerts >=1) || (nPrimType<=MPR_PRM_POLYLINE && nVerts >=2));		// Ensure no degenerate nPrimTypeitives

	#if  _CONTEXT_TRACE_ALL
	MonoPrint("ContextMPR::DrawPrimitive3(%d, 0x%X, %d, 0x%X)\n", nPrimType, VtxInfo, nVerts, pData);
	#endif

	SetPrimitiveType(nPrimType);

	#if  _CONTEXT_ENABLE_STATS
	m_stats.Primitive(m_nCurPrimType, nVerts);
	#endif // _CONTEXT_ENABLE_STATS

	TTLVERTEX *pVtx;	// points to locked VB
	if(!LockVB(nVerts, (void **) &m_pVtx)) return; 	// Lock VB

	//TODO Debug.Assert(false == F4IsBadWritePtr(m_pVtx, sizeof *m_pVtx));	
	Debug.Assert(m_dwStartVtx < m_dwVBSize);
	pVtx = &m_pVtx[m_dwStartVtx + m_dwNumVtx];

	//TODO Debug.Assert(false == F4IsBadWritePtr(pVtx, nVerts * sizeof *pVtx));

	if (!pVtx) // JB 011124 CTD
		return;

	switch(VtxInfo)
	{
		case MPR_VI_COLOR | MPR_VI_TEXTURE:
		{
			// Iterate for each vertex
			for(int i=0;i<nVerts;i++)
			{
				Debug.Assert((BYTE *) pVtx < m_pVtxEnd);	// check for overrun

				/*if (F4IsBadWritePtr(pVtx, sizeof(TTLVERTEX))) // JB 010222 CTD (too much CPU)
				{
					pVtx++;
					continue;
				}*/
				if (!pData[i]) // JB 010712 CTD second try
					break;

				pVtx.sx = pData[i].x;
				pVtx.sy = pData[i].y;
				pVtx.sz = 0.0f;
				pVtx.rhw = pData[i].q > 0.0f ? 1.0f / (pData[i].q / Q_SCALE) : 1.0f;
				pVtx.color = D3DRGBA(pData[i].r, pData[i].g, pData[i].b, pData[i].a);
				pVtx.tu = pData[i].u;
				pVtx.tv = pData[i].v;

				#if  _CONTEXT_ENABLE_RENDERSTATE_HIGHLIGHT
				pVtx.color = currentState != -1 ? RGBA_MAKE((currentState << 1) + 50, (currentState << 1) + 50, (currentState << 1) + 50, (currentState << 1) + 50) : D3DRGBA(1.0f, 1.0f, 1.0f, 1.0f);
				#endif

				pVtx++;
			}

			break;
		}

		case MPR_VI_COLOR:
		{
			// Iterate for each vertex
			for(int i=0;i<nVerts;i++)
			{
				Debug.Assert((BYTE *) pVtx < m_pVtxEnd);	// check for overrun

				pVtx.sx = pData[i].x;
				pVtx.sy = pData[i].y;
				pVtx.sz = 0.0f;
				pVtx.rhw = pData[i].q > 0.0f ? 1.0f / (pData[i].q / Q_SCALE) : 1.0f;
				pVtx.color = D3DRGBA(pData[i].r, pData[i].g, pData[i].b, pData[i].a);

				#if  _CONTEXT_ENABLE_RENDERSTATE_HIGHLIGHT
				pVtx.color = currentState != -1 ? RGBA_MAKE((currentState << 1) + 50, (currentState << 1) + 50, (currentState << 1) + 50, (currentState << 1) + 50) : D3DRGBA(1.0f, 1.0f, 1.0f, 1.0f);
				#endif

				pVtx++;
			}

			break;
		}

		default:
		{
			Debug.Assert(VtxInfo == null);

			// Iterate for each vertex
			for(int i=0;i<nVerts;i++)
			{
				Debug.Assert((BYTE *) pVtx < m_pVtxEnd);	// check for overrun

				pVtx.sx = pData[i].x;
				pVtx.sy = pData[i].y;
				pVtx.sz = 0.0f;
				pVtx.rhw = pData[i].q > 0.0f ? 1.0f / (pData[i].q / Q_SCALE) : 1.0f;
				pVtx.color = m_colFG;

				#if  _CONTEXT_ENABLE_RENDERSTATE_HIGHLIGHT
				pVtx.color = currentState != -1 ? RGBA_MAKE((currentState << 1) + 50, (currentState << 1) + 50, (currentState << 1) + 50, (currentState << 1) + 50) : D3DRGBA(1.0f, 1.0f, 1.0f, 1.0f);
				#endif

				pVtx++;
			}

			break;
		}
	}

	// Generate Indices (in advance)
	if(m_nCurPrimType == D3DPT_TRIANGLEFAN)
	{
		WORD *pIdx = &m_pIdx[m_dwNumIdx];

		for(int x=0;x<nVerts-2;x++)
		{
			pIdx[0] = m_dwNumVtx;
			pIdx[1] = m_dwNumVtx + x + 1;
			pIdx[2] = m_dwNumVtx + x + 2;
			pIdx += 3;
		}

		m_dwNumIdx += pIdx - &m_pIdx[m_dwNumIdx];
	}

	else if(m_nCurPrimType == D3DPT_LINESTRIP)
	{
		WORD *pIdx = &m_pIdx[m_dwNumIdx];

		for(int x=0;x<nVerts;x++)
			*pIdx++ = m_dwNumVtx + x;

		m_dwNumIdx += nVerts;
	}

	m_dwNumVtx += nVerts;

	#if  _CONTEXT_FLUSH_EVERY_PRIMITIVE
	FlushVB();
	#endif
}

	public void TextOut(short x, short y, DWORD col, LPSTR str)
{
	#if _CONTEXT_TRACE_ALL
	MonoPrint("ContextMPR::TextOut(%d, %d, 0x%X, %s)\n", x, y, col, str);
	#endif

	if(!str) return;

	try
	{
		HDC hdc;
		CheckHR(m_pRenderTarget.GetDC(&hdc));	// Get GDI Device context for Surface

		if(hdc)
		{
			SetBkMode(hdc, TRANSPARENT);
			SetTextColor(hdc, col);
			MoveToEx(hdc, x, y, null);

			DrawText(hdc, str, strlen(str), &m_rcVP, DT_LEFT);

			CheckHR(m_pRenderTarget.ReleaseDC(hdc));
		}
	}

	catch(_com_error e)
	{
	}
}
		
	public void SetViewportAbs(int nLeft, int nTop, int nRight, int nBottom)
{
	if(m_bViewportLocked)
		return;

	m_rcVP.left = nLeft;
	m_rcVP.right = nRight;
	m_rcVP.top = nTop;
	m_rcVP.bottom = nBottom;

	UpdateViewport();
}
	public void LockViewport()
{
//	Debug.Assert(!m_bViewportLocked);
	m_bViewportLocked = true;
}

	public void UnlockViewport()
{
//	Debug.Assert(m_bViewportLocked);
	m_bViewportLocked = false;
}

public void GetViewport(RECT *prc)
{
    //TODO Debug.Assert(false == F4IsBadWritePtr(prc, sizeof *prc));
	*prc = m_rcVP;
}
		public void Stats__TODO()
{
	#if _DEBUG
	if(m_bNoD3DStatsAvail)
		return;

	HRESULT hr;
	#if _CONTEXT_USE_MANAGED_TEXTURES
	D3DDEVINFO_TEXTUREMANAGER ditexman;
	hr = m_pD3DD.GetInfo(D3DDEVINFOID_TEXTUREMANAGER, &ditexman, sizeof(ditexman));
	m_bNoD3DStatsAvail = FAILED(hr) || hr == S_false;
	#endif

	D3DDEVINFO_TEXTURING ditex;
	hr = m_pD3DD.GetInfo(D3DDEVINFOID_TEXTURING, &ditex, sizeof(ditex));
	m_bNoD3DStatsAvail = FAILED(hr) || hr == S_false;
	#endif
}
#endif
	}
}

