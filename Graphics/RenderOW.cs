using System;
using System.Diagnostics;
using FalconNet.Common;
using DWORD = System.UInt32;

namespace FalconNet.Graphics
{
    /***************************************************************************\
        RenderOW.h
        Scott Randolph
        January 2, 1996

        This class provides 3D drawing functions specific to rendering out the
        window views including terrain.

        The implementation of this class is split into multiple .cpp files
        with names of the form OTW*.cpp
    \***************************************************************************/


    //#define ENABLE_LIGHTING		// Define this to light the terrain when hazing is on
    /*
    //#define TWO_D_MAP_AVAILABLE // Compile with this on to make the 2D debugging "map" view available
    //#if  TWO_D_MAP_AVAILABLE
    //extern bool	twoDmode;
    //extern int  TWODSCALE;
    //#endif
    */

    public struct TerrainVertex
    {
        public float x, y, z; 			// screen space	x & y, camera space z
        public float q, u, v;
        public float r, g, b, a;
        public ClippingFlags clipFlag;
        public float csX, csY, csZ;		// Camera space coordinates (csZ could go)

        public Tpost post;
        public int RenderingStateHandle;
    };


    public struct SpanMinMax
    {
        public float insideEdge;
        public float minEndPoint;
        public float maxEndPoint;
        public int startDraw;
        public int stopDraw;
        public int startXform;
        public int stopXform;
    };


    public class SpanListEntry
    {
        public int ring;
        public int LOD;
        public SpanMinMax Tsector;
        public SpanMinMax Rsector;
        public SpanMinMax Bsector;
        public SpanMinMax Lsector;
    };


    // Used to construct the set of visible terrain posts
    public struct BoundSegment
    {
        public Edge edge;
        public float end;
    };


    // Hold the rotated axes vectors and viewer location in units of level posts at each LOD
    public class LODdataBlock
    {
        public float[] Xstep = new float[3];
        public float[] Ystep = new float[3];
        public float[] Zstep = new float[3];
        public int availablePostRange;
        public int centerRow;
        public int centerCol;
        public bool glueOnBottom;
        public bool glueOnLeft;

        //	int		RenderingStateHandle;
    } ;


    // Used to pass information for drawing the sky among the various helper routines
    public struct HorizonRecord
    {
        public float bandAngleUp;	// Angular size (in radians) of sky haze band
        public float hx;				// 	  *	Location of the horizon line in screen space
        public float hy;				// 	  *
        public float vx;				// 	  *
        public float vy;				// 	  ^
        public float vxUp;			//	  *	Location of the top of the haze band
        public float vyUp;			// 	  ^
        public float vxDn;			//	  *	Location of the bottom of the terrain filler band
        public float vyDn;			// 	  ^
        public int horeffect;		// Flag indicating type of sun effect on horizon
        public float hazescale;		// Strength of sun effect at sun location
        public float rhazescale;		// Strength of sun effect at right edge
        public float lhazescale;		// Strength of sun effect at left edge
        public Tpoint sunEffectPos;	// Sun position with respect to horizon
        public Tcolor sunEffectColor;	// Color of sun at horizon
    };


    public class RenderOTW : Render3D
    {

        public RenderOTW() { skyRoof = false; }
        //TODO public virtual ~RenderOTW()	{};

        // Setup and Cleanup need to have additions here, but still call the parent versions
        public virtual void Setup(ImageBuffer imageBuffer, RViewPoint vp)
        {
#if TODO
            int usedLODcount;
            int LOD;
            int LODbufferSize;


            // Call our parents Setup code (win is available only after this call)
            base.Setup(imageBuffer);

            // Retain a pointer to the TViewPoint we are to use
            viewpoint = vp;

            // Start with the default light source position (over head)
            lightTheta = 0.0f;
            lightPhi = Constants.PI_OVER_2;
            SunGlareValue = 0.0f;
            SunGlareWashout = 0.0f;
            smoothed = false; // intialise to something
            textureLevel = 0;
            filtering = hazed = 0;

            // Start with no tunnel vision or cloud effects
            SetTunnelPercent(0.0f, (short)0x80808080);
            PreSceneCloudOcclusion(0.0f, (short)0x80808080);

            // Adjust our back clipping plane based on the range defined for this viewpoint
            SetFar(viewpoint.GetMaxRange() * 0.707f);	// far = maxRange * cos(half_angle)

            // Set the default sky and haze properties
            SetDitheringMode(true);
            SetTerrainTextureLevel(TheMap.LastNearTexLOD());
            SetObjectTextureState(true);	// Default to using textured objects
            SetFilteringMode(false);
            SetHazeMode(true);			// (true = ON, false = OFF)
            SetRoofMode(false);
            SetSmoothShadingMode(true); // JPO - this uses the value from previous state

            // Default to full alpha blending for special effects
            alphaMode = true;

            // Setup a default sky (will be replaced by Time of Day processing)
            sky_color.r = sky_color.g = sky_color.b = 0.5f;
            haze_sky_color.r = haze_sky_color.g = haze_sky_color.b = 0.4f;
            haze_ground_color.r = haze_ground_color.g = haze_ground_color.b = 0.3f;
            earth_end_color.r = earth_end_color.g = earth_end_color.b = 0.2f;


            // Figure out how many posts outward we might ever see at any detail level
            maxSpanOffset = 0;
            for (LOD = viewpoint.GetMinLOD(); LOD <= viewpoint.GetMaxLOD(); LOD++)
            {
                maxSpanOffset = Math.Max(maxSpanOffset, viewpoint.GetMaxPostRange(LOD));
            }
            maxSpanExtent = maxSpanOffset * 2 + 1;

            // Figure the total number of rings to deal with assuming max number of posts
            // across each of the detail levels.  Include room for a few extra padding rings
            usedLODcount = viewpoint.GetMaxLOD() - viewpoint.GetMinLOD() + 1;
            spanListMaxEntries = (maxSpanOffset + 1) * usedLODcount + 2;


            // Allocate memory for our list of vertex spans
            spanList = new SpanListEntry[spanListMaxEntries];
            firstEmptySpan = spanList;
            if (!spanList)
            {
                ShiError("Failed to allocate span buffer");
            }
            // TODO memset (spanList, 0, sizeof(*spanList) * spanListMaxEntries); // JPO zero out

            // Allocate memory for the the transformed vertex buffers we need
            LODbufferSize = (maxSpanExtent) * (maxSpanExtent);
            vertexMemory = new TerrainVertex[usedLODcount * LODbufferSize];
            if (!vertexMemory)
            {
                ShiError("Failed to allocate transformed vertex buffer");
            }
            //TODO memset(vertexMemory, 0, sizeof(*vertexMemory) * usedLODcount * LODbufferSize); // JPO start with 0

            // Allocate memory for the array of transformed vertex buffer pointers
            vertexBuffer = new TerrainVertex*[(viewpoint.GetMaxLOD() + 1)];
            if (!vertexBuffer)
            {
                ShiError("Failed to allocate transformed vertex buffer list");
            }


            // Setup the + and - indexable vertex pointers (they point into the vertexMemory)
            for (LOD = 0; LOD <= viewpoint.GetMaxLOD(); LOD++)
            {
                if (LOD < viewpoint.GetMinLOD())
                {
                    vertexBuffer[LOD] = null;
                }
                else
                {
                    vertexBuffer[LOD] = vertexMemory +
                                        (LOD - viewpoint.GetMinLOD()) * LODbufferSize +
                                        maxSpanExtent * maxSpanOffset + maxSpanOffset;
                }
            }


            // Allocate memory for the array of information stored for each LOD (viewer location & vectors)
            LODdata = new LODdataBlock[(viewpoint.GetMaxLOD() + 1)];
            if (!LODdata)
            {
                ShiError("Failed to allocate memory for LOD step vector array");
            }

            brightness = 1.0; // JB 010618 vary the brightness on cloud thickness
            visibility = 1.0;
            rainFactor = 0;
            snowFactor = 0;
            thunderAndLightning = false;
            thundertimer = 0;
            thunder = false;
            Lightning = false;
            lightningtimer = 0.0F;

            // Initialize the lighting conditions and register for future time of day updates
            TimeUpdateCallback(this);
            TheTimeManager.RegisterTimeUpdateCB(TimeUpdateCallback, this);
#endif
            throw new NotImplementedException();
        }

        /***************************************************************************\
        Shutdown the renderer.
        \***************************************************************************/
        public virtual void Cleanup()
        {
#if TODO
            // Stop receiving time updates
            TheTimeManager.ReleaseTimeUpdateCB(TimeUpdateCallback, this);

            // Release the memory used to hold the LOD data blocks
            Debug.Assert(LODdata!= null);
            delete[] LODdata;
            LODdata = null;

            // Release the memory for the array of transformed vertex buffer pointers
            Debug.Assert(vertexBuffer);
            delete[] vertexBuffer;
            vertexBuffer = null;

            // Release the memory for the list of vertex spans
            Debug.Assert(spanList);
            delete[] spanList;
            spanList = null;

            // Release the memory for the the transformed vertex buffers
            Debug.Assert(vertexMemory);
            delete[] vertexMemory;
            vertexMemory = null;

            // Set our allowable ranges to 0
            maxSpanOffset = 0;
            maxSpanExtent = 0;
            spanListMaxEntries = 0;

            // Discard the pointer to our associated TViewPoint object
            viewpoint = null;

            // Call our parent's cleanup
            Render3D.Cleanup();
#endif
            throw new NotImplementedException();
        }

        // Overload this function to get extra work done at start frame
        public virtual void StartFrame()
        {
#if TODO
            base.StartFrame();

            if (CTimeOfDay.TheTimeOfDay.GetNVGmode())
            {
                Drawable2D.SetGreenMode(true);
                DrawableOvercast.SetGreenMode(true);
                TheColorBank.SetColorMode(ColorBankClass.GreenMode);
            }
#endif
            throw new NotImplementedException();
        }

        // Select the amount of terrain texturing employed
        public void SetTerrainTextureLevel(int level)
        {
#if TODO
            // Limit the texture level to legal values
            textureLevel = Math.Max(level, viewpoint.GetMinLOD() - 1);
            textureLevel = Math.Min(textureLevel, viewpoint.GetMaxLOD());
            textureLevel = Math.Min(textureLevel, TheMap.LastFarTexLOD());

            // Rearrange the fog settings
            haze_start = far_clip * 0.4f;
            haze_depth = far_clip * 0.85f;

            // Rearrange the texture blend settings
            blend_start = viewpoint.GetMaxRange(textureLevel - 1);
            blend_depth = viewpoint.GetMaxRange(textureLevel) * 0.8f;

            // Convert from range from viewer to range from start
            blend_depth -= blend_start;
            haze_depth -= haze_start;

            // Reevaluate which rendering states to use for each LOD
            SetupStates();
#endif
            throw new NotImplementedException();
        }

        public int GetTerrainTextureLevel() { return textureLevel; }

        // Set/get rendering settings
        public void SetHazeMode(bool state) { hazed = state; SetupStates(); }
        public bool GetHazeMode() { return hazed; }

        // alpha setting
        public void SetAlphaMode(bool state) { alphaMode = state; }
        public bool GetAlphaMode() { return alphaMode; }

        public void SetRoofMode(bool state)
        {
#if TODO
            // Don't bother if the roof state isn't changing
            if (state == skyRoof)
            {
                return;
            }

            // Get the textures we'll need if the roof is being turned on
            if (state && !texRoofTop.TexHandle())
            {
                Tcolor light;

                texRoofTop.LoadAndCreate("OVClayerT.gif", MPR_TI_PALETTE);
                texRoofBottom.LoadAndCreate("OVClayerB.gif", MPR_TI_PALETTE);

                CTimeOfDay.TheTimeOfDay.GetTextureLightingColor(&light);
                texRoofTop.palette.LightTexturePalette(&light);
                texRoofBottom.palette.LightTexturePalette(&light);
            }

            // Store the new state
            skyRoof = state;
#endif
            throw new NotImplementedException();
        }
        public bool GetRoofMode() { return skyRoof; }

        public void SetSmoothShadingMode(bool state) { smoothed = state; SetupStates(); }
        public bool GetSmoothShadingMode() { return smoothed; }

        public void SetDitheringMode(bool state) { dithered = state; SetupStates(); }
        public bool GetDitheringMode() { return dithered; }

        public void SetFilteringMode(bool state) { filtering = state; SetupStates(); }
        public bool GetFilteringMode() { return filtering; }

        public float GetRangeOnlyFog(float range) { return (range - haze_start) / haze_depth; }
        public float GetValleyFog(float distance, float worldZ)
        {
            float valleyFog;
            float fog;

            // Valley fog
            const float VALLEY_HAZE_TOP = 1000.0f;
            const float VALLEY_HAZE_MAX = 0.75f;
            const float VALLEY_HAZE_START_RANGE = PERSPECTIVE_RANGE;
            const float VALLEY_HAZE_FULL_RANGE = PERSPECTIVE_RANGE + 36000.0f;

            if (worldZ > -VALLEY_HAZE_TOP)
            {	// -Z is upward
                // We're below the top of the valley fog layer
                valleyFog = (VALLEY_HAZE_TOP + worldZ) / VALLEY_HAZE_TOP;
                valleyFog = Math.Min(valleyFog, VALLEY_HAZE_MAX);

                if (distance < VALLEY_HAZE_FULL_RANGE)
                {
                    valleyFog *= (distance - VALLEY_HAZE_START_RANGE) / (VALLEY_HAZE_FULL_RANGE - VALLEY_HAZE_START_RANGE);
                }

                // Distance fog
                fog = GetRangeOnlyFog(distance);

                // Mixing
                return Math.Max(fog, valleyFog);
            }
            else
            {
                // We're above the valley fog layer
                return GetRangeOnlyFog(distance);
            }
        }
        public Tcolor GetFogColor() { return haze_ground_color; }

        // Draw the out the window view, including terrain and all registered objects
        public void DrawScene(Tpoint offset, Trotation orientation)
        {
#if TODO
            Tpoint position = { 0.0F };
            int containingList = 0;
            float prevFOV = 0.0F, prevLeft = 0.0F, prevRight = 0.0F, prevTop = 0.0F, prevBottom = 0.0F;

            prevFOV = GetFOV();
            GetViewport(&prevLeft, &prevTop, &prevRight, &prevBottom);

            // Reduce the viewport size to save on overdraw costs if there's a tunnel in effect
            if (tunnelSolidWidth > 0.0f)
            {
                float visible = (1.0f - tunnelSolidWidth) * big;

                if (visible <= 1.0f)
                {
                    float left, top, right, bottom;
                    float fov;

                    right = Math.Min(visible, prevRight);
                    left = Math.Max(-visible, prevLeft);
                    top = Math.Min(visible, prevTop);
                    bottom = Math.Max(-visible, prevBottom);
                    fov = 2.0f * (float)atan(right / oneOVERtanHFOV);

                    SetFOV(fov);
                    SetViewport(left, top, right, bottom);
                }
            }


            // Get our world space position from our viewpoint
            viewpoint.GetPos(&position);

            // Apply the offset (if provided) -- in world space for now (TODO: camera space?)
            if (offset)
            {
                position.x += offset.x;
                position.y += offset.y;
                position.z += offset.z;
            }

            // Call Render3D's cammera update function with the new position
            SetCamera(&position, orientation);


            // Update the sky color based on our current attitude and position
            AdjustSkyColor();


            float opacity = viewpoint.CloudOpacity();
            if (g_bEnableWeatherExtensions)
            {
                if (position.z > viewpoint.GetLocalCloudTops()) // are we below clouds? -ve z's
                {
                    float tvis = viewpoint.GetVisibility();
                    visibility = 0.95f * visibility + 0.05f * tvis; // phase in new vis
                    float train = viewpoint.GetRainFactor();
                    rainFactor = 0.95 * rainFactor + 0.05f * train; // phase in rain
                    float temp = TheWeather.TemperatureAt(&position);
                    const float TEMP_RANGE = 10;
                    const float TEMP_MIN = TEMP_RANGE / 2.0f;

                    if (temp < -TEMP_MIN) // all rain turns to snow
                    {
                        snowFactor = rainFactor;
                        rainFactor = 0;
                    }
                    else if (temp < TEMP_MIN) // maybe do snow
                    {
                        temp += TEMP_MIN;
                        snowFactor = rainFactor * (TEMP_RANGE - temp) / TEMP_RANGE;
                        rainFactor = rainFactor * (temp / TEMP_RANGE);
                    }
                    else
                        snowFactor = 0;

                    if (viewpoint.GetLightning())
                    {
                        thunderAndLightning = true;
                        opacity = Math.Max(opacity, 0.97); // a flash of lightning.
                        thundertimer = vuxRealTime + 10000.0f * PRANDFloatPos(); // in 10 seconds time
                    }
                    else
                        thunderAndLightning = false;
                }
                else
                {
                    visibility = 1;
                    rainFactor = 0;
                    snowFactor = 0;
                }

                if (thundertimer > 0 && thundertimer < vuxRealTime)
                {
                    thunder = true;
                    thundertimer = 0;
                }
                else
                    thunder = false;
            }

            if (visibility < 1) // less than perfect.
                opacity = Math.Max(opacity, 1.0 - visibility);

            // Handle the entering/inside/leaving cloud effects
            if (opacity <= 0.0f && !Lightning)
            {
                // We're not being affected by a cloud, the only effect is sun glare (if any)
                PreSceneCloudOcclusion(SunGlareWashout, 0xFFFFFFFF);
            }
            else
            {
                // We're being affected by a cloud.
                Tcolor color;
                DWORD c;
                float scaler;
                float blend;

                if (thunderAndLightning) // Lightning for nuke
                {
                    color = CTimeOfDay.TheTimeOfDay.GetLightningColor();
                }
                else if (Lightning)
                {
                    color.r = 1.0F;
                    color.b = 1.0F;
                    color.g = 1.0F;
                    lightningtimer += SimLibMajorFrameTime;
                    if (lightningtimer > 5.0F)	// 5 seconds full white
                        opacity = (0.97F - ((lightningtimer - 6) * 0.068F));
                    else
                        opacity = 0.97F;
                    if (lightningtimer >= 20.0F)
                    {
                        lightningtimer = 0.0F;
                        Lightning = false;
                    }
                }
                else
                {
                    // Get the cloud properties
                    color = viewpoint.CloudColor();
                }

                // Factor in sun glare (if any)
                scaler = 1.0f / (opacity + SunGlareWashout);
                blend = Math.Max(opacity, SunGlareWashout);

                // Decide on the composite blending color
                color.r = (opacity * color.r + SunGlareWashout) * scaler;
                color.g = (opacity * color.g + SunGlareWashout) * scaler;
                color.b = (opacity * color.b + SunGlareWashout) * scaler;

                // JB 010618 vary the brightness on cloud thickness
                if (g_bEnableWeatherExtensions && !thunderAndLightning)
                {
                    float thickness = fabs(viewpoint.GetLocalCloudTops() - position.z);
                    if (thickness > 0)
                    {
                        float tbrt = Math.Max(.2, Math.Min(1, g_fCloudThicknessFactor / thickness));
                        brightness = 0.95f * brightness + 0.05f * tbrt; // phase in new brt
                    }
                    else
                        brightness = 1.0;

                    color.r *= brightness;
                    color.g *= brightness;
                    color.b *= brightness;
                }

                // Construct a 32 bit RGB value
                ProcessColor(&color);
                c = (int)(color.r * 255.9f);
                c |= ((int)(color.g * 255.9f)) << 8;
                c |= ((int)(color.b * 255.9f)) << 16;

                // Are we IN it our NEAR it?
                if (blend >= 1.0f)
                {
                    // Clear the screen to cloud color
                    context.SetState(MPR_STA_BG_COLOR, c);
                    ClearFrame();

                    // Draw the tunnel vision effect if any
                    if (tunnelSolidWidth > 0.0f)
                    {
                        SetFOV(prevFOV);
                        SetViewport(prevLeft, prevTop, prevRight, prevBottom);
                    }

                    // And we're done!
                    return;

                }
                else
                {
                    // We're entering or leaving the cloud, so "fuzz" things
                    PreSceneCloudOcclusion(blend, c);
                }
            }


            // Draw the sky
            DrawSky();

            // Sort the object list based on our location
            viewpoint.ResetObjectTraversal();


            // Figure out which list would contain our eye point
            containingList = viewpoint.GetContainingList(position.z);

            // Special case if we're above the roof and the roof is diplayed
            if ((containingList == 4) && (skyRoof))
            {
                viewpoint.ObjectsAboveRoof().DrawBeyond(0.0f, 0, this);

                // Restore the FOV if it was changed by the tunnel code
                if (tunnelSolidWidth > 0.0f)
                {
                    SetFOV(prevFOV);
                    SetViewport(prevLeft, prevTop, prevRight, prevBottom);
                }

                return;
            }

            // Draw scene components in height sorted groups dependent of our altitude
            // Upward order (don't draw the one we're in)
            if (containingList > 0)
            {
                DrawGroundAndObjects(viewpoint.ObjectsInTerrain());
                if (containingList > 1)
                {
                    viewpoint.ObjectsBelowClouds().DrawBeyond(0.0f, 0, this);
                    if (containingList > 2)
                    {
                        DrawCloudsAndObjects(viewpoint.Clouds(), viewpoint.ObjectsInClouds());
                        if (containingList > 3)
                        {
                            viewpoint.ObjectsAboveClouds().DrawBeyond(0.0f, 0, this);
                        }
                    }
                }
            }

            // Downward order (finishing with the one we're in)
            viewpoint.ObjectsAboveRoof().DrawBeyond(0.0f, 0, this);
            if (containingList < 4)
            {
                viewpoint.ObjectsAboveClouds().DrawBeyond(0.0f, 0, this);
                if (containingList < 3)
                {
                    DrawCloudsAndObjects(viewpoint.Clouds(), viewpoint.ObjectsInClouds());
                    if (containingList < 2)
                    {
                        viewpoint.ObjectsBelowClouds().DrawBeyond(0.0f, 0, this);
                        if (containingList < 1)
                        {
                            DrawGroundAndObjects(viewpoint.ObjectsInTerrain());
                        }
                    }
                }
            }

            if (g_bEnableWeatherExtensions)
                DrawWeather(orientation); // JPO experiment

            // Restore the FOV if it was changed by the tunnel code
            if (tunnelSolidWidth > 0.0f)
            {
                SetFOV(prevFOV);
                SetViewport(prevLeft, prevTop, prevRight, prevBottom);
                SetCamera(&position, orientation);
            }
#endif
            throw new NotImplementedException();
        }


        // Special calls used for tunnel vision and cloud occulsion effects
        public float GetTunnelPercent() { return tunnelPercent; }

        /***************************************************************************\
        Setup the tunnel vision effect.  This should only be called
        on the primary out the window view.  The percent supplied is
        is relative to the total size of the underlying image object.
        THIS CALL IS TO BE CALLED _BEFORE_ DrawScene()
        \***************************************************************************/
        public void SetTunnelPercent(float percent, DWORD color)
        {
#if TODO
            float startpct = percent;
            // Clamp the percent value to the allowable range
            if (percent > 1.0f) percent = 1.0f;
            else if (percent < 0.0f) percent = 0.0f;

            // Apply adjustments if we're in NVG mode
            if (CTimeOfDay.TheTimeOfDay.GetNVGmode() && (percent < NVG_TUNNEL_PERCENT))
            {
                percent = NVG_TUNNEL_PERCENT;

                if (percent <= 0.0f)
                {
                    color = 0;
                }
                else
                {
                    float t = percent / NVG_TUNNEL_PERCENT;
                    color = (short)(((int)((color & 0x00FF0000) * t) & 0x000000FF) |
                            ((int)((color & 0x00FF0000) * t) & 0x0000FF00) |
                            ((int)((color & 0x00FF0000) * t) & 0x00FF0000));
                }
                if (F4Config.g_bFullScreenNVG)
                    percent = startpct;
            }

            // Store the new tunnel vision parameters
            tunnelColor = color;
            tunnelPercent = percent;
            tunnelAlphaWidth = percent * PercentScale;
            tunnelSolidWidth = tunnelAlphaWidth - PercentBlend;
        #endif
            throw new NotImplementedException();
        }

        public void PostSceneCloudOcclusion()						// CALL between DrawScene() and FinishFrame()
        {
#if TODO
            // If we're in software, we turn off the special color munging function
            if (image.GetDisplayDevice().IsHardware())
            {

                // Drop out if alpha is 0
                if ((cloudColor & 0xFF000000) == 0)
                {
                    return;
                }

                // OW
#if !NOTHING
                // Draw the viewport sized alpha blended polygon
                MPRVtx_t[] pVtx = new MPRVtx_t[4];

                // Set the foreground color and drawing state
                context.SelectForegroundColor(cloudColor);
                context.RestoreState(STATE_ALPHA_SOLID);

                // Now intialize the four corners of the rectangle to fill
                pVtx[0].x = leftPixel; pVtx[0].y = bottomPixel;
                pVtx[1].x = leftPixel; pVtx[1].y = topPixel;
                pVtx[2].x = rightPixel; pVtx[2].y = topPixel;
                pVtx[3].x = rightPixel; pVtx[3].y = bottomPixel;

                context.DrawPrimitive(MPR_PRM_TRIFAN, 0, 4, pVtx, sizeof(MPRVtx_t));
#else
		// Draw the viewport sized alpha blended polygon
		MPRVtx_t	*p;

		// Set the foreground color and drawing state
		context.SelectForegroundColor( cloudColor );
		context.RestoreState( STATE_ALPHA_SOLID );

		// Start the primitive and get a pointer to the target vertex data
		context.Primitive( MPR_PRM_TRIFAN, 0, 4, sizeof(*p) );
		p = (MPRVtx_t*)context.GetContextBufferPtr();

		// Now intialize the four corners of the rectangle to fill
		p.x = leftPixel;	p.y = bottomPixel;		p++;
		p.x = leftPixel;	p.y = topPixel;		p++;
		p.x = rightPixel;	p.y = topPixel;		p++;
		p.x = rightPixel;	p.y = bottomPixel;		p++;

		// Finish off the primitive and send it (since it'll be slow)
		context.SetContextBufferPtr( (BYTE*)p );
		context.SendCurrentPacket();
#endif

            }
            else
            {

                // Set the color correction terms back to normal
                // TODO:  Update this once Marc's changes are in...
                context.SetState(MPR_STA_GAMMA_RED, (DWORD)(1.0f));
                context.SetState(MPR_STA_GAMMA_GREEN, (DWORD)(1.0f));
                context.SetState(MPR_STA_GAMMA_BLUE, (DWORD)(1.0f));

            }
        }

        public void DrawTunnelBorder()
        {
            TwoDVertex[] vert = new TwoDVertex[NumPoints * 2 + 1];
            TwoDVertex vertPointers = new TwoDVertex[4];
            int i;
            int j1, j2;
            float x, y;
            float alpha;

            // Quit now if the tunnel isn't being drawn
            if (tunnelAlphaWidth <= 0.0f)
                return;

            // OW 
            ZeroMemory(vert, sizeof(vert));

            // Restart the rasterizer to draw the tunnel border
            context.StartFrame();

            // Put the clip rectangle at full size to allow drawing of the border
            SetViewport(-1.0f, 1.0f, 1.0f, -1.0f);

            // Initialize all the verticies
            float r = (float)((tunnelColor) & 0xFF) / 255.9f;
            float g = (float)((tunnelColor >> 8) & 0xFF) / 255.9f;
            float b = (float)((tunnelColor >> 16) & 0xFF) / 255.9f;

            for (i = 0; i < NumPoints; i++)
            {
                j1 = i << 1;
                j2 = j1 + 1;

                // Color
                vert[j1].r = vert[j2].r = r;
                vert[j1].g = vert[j2].g = g;
                vert[j1].b = vert[j2].b = b;
                vert[j1].a = vert[j2].a = 1.0f;

                // Root location
                vert[j1].x = viewportXtoPixel(OutsidePoints[i].x);
                vert[j1].y = viewportYtoPixel(OutsidePoints[i].y);
                SetClipFlags(&vert[j1]);

                // Inside edge
                x = (1.0f - tunnelSolidWidth) * OutsidePoints[i].x;
                y = (1.0f - tunnelSolidWidth) * OutsidePoints[i].y;
                vert[j2].x = viewportXtoPixel(x);
                vert[j2].y = viewportYtoPixel(y);
                SetClipFlags(&vert[j2]);
            }

            // Special pickup for the one vertex which wasn't colored this time, but will be used next
            vert[i * 2].r = r;
            vert[i * 2].g = g;
            vert[i * 2].b = b;

            // Draw the flat colored mesh if it is visible
            if (tunnelSolidWidth > 0.0f)
            {
                context.RestoreState(STATE_SOLID);

                for (i = NumPoints - 2; i >= 0; i--)
                {
                    j1 = i << 1;
                    vertPointers[0] = &vert[j1];
                    vertPointers[1] = &vert[j1 + 2];
                    vertPointers[2] = &vert[j1 + 3];
                    vertPointers[3] = &vert[j1 + 1];
                    ClipAndDraw2DFan(vertPointers, 4);
                }
            }

            // Update the ending alpha value and alpha percent for the closing out the view case
            if (tunnelAlphaWidth > 1.0f)
            {
                alpha = (tunnelAlphaWidth - 1.0f) / PercentBlend;
                tunnelAlphaWidth = 1.0f;
            }

            else
                alpha = 0.0f;

            // Fill the blended portion of the border
            // NOTE:  The inside of the solid mesh is the outside of the blending mesh
            // therefore, the odd numbered vertices from above can be reused

            for (i = 0; i < NumPoints; i++)
            {
                j1 = (i << 1) + 1;		// Index of vertex to be reused (last times inside edge)
                j2 = j1 + 1;			// Index of vertex to replace (last times outside edge)

                // Alpha
                vert[j2].a = alpha;

                // Inside edge
                x = (1.0f - tunnelAlphaWidth) * OutsidePoints[i].x;
                y = (1.0f - tunnelAlphaWidth) * OutsidePoints[i].y;
                vert[j2].x = viewportXtoPixel(x);
                vert[j2].y = viewportYtoPixel(y);
                SetClipFlags(&vert[j2]);
            }

            // Draw the blended mesh
            context.RestoreState(STATE_ALPHA_GOURAUD);

            for (i = NumPoints - 2; i >= 0; i--)
            {
                j1 = (i << 1) + 1;
                vertPointers[0] = &vert[j1];
                vertPointers[1] = &vert[j1 + 2];
                vertPointers[2] = &vert[j1 + 3];
                vertPointers[3] = &vert[j1 + 1];
                ClipAndDraw2DFan(vertPointers, 4);
            }

            // Close down the renderer and flush the queue
            context.FinishFrame(null);
#endif
            throw new NotImplementedException();
        }


        public bool IsThunder() { return thunder; }
        public float RainFactor() { return rainFactor; }
        public float SnowFactor() { return snowFactor; }
        public float Visibility() { return visibility; }
        public void SetLightning() { Lightning = true; }



        public const float PERSPECTIVE_RANGE = 6000.0f;
        public const float NVG_TUNNEL_PERCENT = 0.2f;
        public const float FOG_MIN = 0.05f;
        public const float FOG_MAX = 0.95f;

        public const float MOON_DIST = 40.0f;
        public const float SUN_DIST = 30.0f;

        public const float MOST_SUN_GLARE_DIST = 12.0f;
        public const float MIN_SUN_GLARE = 0.0f;

        public const float ROOF_REPEAT_COUNT = 6.0f;

        public const float HAZE_ALTITUDE_FACTOR = (1.0f / (WeatherMap.SKY_MAX_HEIGHT - WeatherMap.SKY_ROOF_HEIGHT));
        public const float GLARE_FACTOR = (4096.0f / WeatherMap.SKY_MAX_HEIGHT);

        public static Texture texRoofTop;
        public static Texture texRoofBottom;

        public RViewPoint viewpoint;


        // Control states
        protected bool smoothed;		// Smooth shading state (on or off)
        protected bool dithered;		// Dithering state (on or off)
        protected bool hazed;			// Turns hazing on or off (true => "On")
        protected bool skyRoof;		// Turns the "roof" over the world on or off
        protected bool filtering;		// Turns texture filtering on or off
        protected bool alphaMode;      // Enables alpha blending in special effects

        // MPR state handles used to draw terrain at various distances
        protected DWORD state_fore;		// Normally perspective corrected   
        protected DWORD state_near;		// Normally plain texture           
        protected DWORD state_mid;		// Normally fading texture          
        protected DWORD state_far;		// Normally gouraud shaded          

        // Lighting angles
        protected float lightTheta;
        protected float lightPhi;

        // Sky properties
        protected Tcolor sky_color;			// This is the color of the sky above the horizon
        protected Tcolor haze_ground_color;	// This is the color distant terrain blends toward
        protected Tcolor earth_end_color;	// This is the color of the ground at the horizon
        protected Tcolor haze_sky_color;		// This is the color at which the sky blend starts
        internal float haze_start;			// The distance (in feet) from the viewer at which hazing starts
        internal float haze_depth;
        internal float blend_start;
        internal float blend_depth;
        protected float SunGlareValue;
        protected float SunGlareWashout;

        // weather properties
        protected float visibility;		// what sort of vis there is.
        protected float rainFactor;		// what sort of rain, 0 none, else 0-1 gives heaviness
        protected float snowFactor;		// what sort of snow, 0 none, else 0-1 gives heaviness
        protected float brightness;   // how bright things are given the cloud thickness overhead
        protected bool thunderAndLightning;	// is this likely....
        protected bool thunder;		// set if we should hear thunder
        protected ulong thundertimer;	// when to play
        protected bool Lightning;	// only for nukes
        protected float lightningtimer; // for nuke lightning

        // Terrain drawing state
        protected int textureLevel;

        // How much of the players vision has been lost (normally 0s)
        protected float tunnelPercent;
        protected float tunnelAlphaWidth;
        protected float tunnelSolidWidth;
        protected DWORD tunnelColor;
        protected DWORD cloudColor;

        // Points which define the corners of the viewing volume
        protected float rightX1, rightX2;
        protected float rightY1, rightY2;
        protected float leftX1, leftX2;
        protected float leftY1, leftY2;

        // Edges which define the viewing volume
        protected Edge front_edge;
        protected Edge back_edge;
        protected Edge left_edge;
        protected Edge right_edge;


        // The maximum expected length of any one span in total and measured outward from zero
        protected int maxSpanExtent;
        protected int maxSpanOffset;

        // The maximum expected number of rings we'll deal with
        protected int spanListMaxEntries;

        // Pointers to the memory we use to build the span lists
        protected SpanListEntry spanList;
        protected SpanListEntry firstEmptySpan;

        // An array of pointers to transformed vertex buffers - one per LOD
        // These point into the vertexMemory and are setup for + and - indexing
        protected TerrainVertex[] vertexBuffer;

        // An array of transformed vertices (portions used by each LOD)
        protected TerrainVertex vertexMemory;

        // Array which records the axes vectors and viewer location in units of level posts at each LOD
        protected LODdataBlock[] LODdata;


        // Utility functions used within this class
        protected void SetupStates()
        {
#if TODO
            // Decide if the default terrain mode is flat or interpolated color
            if (smoothed)
            {
                state_far = STATE_GOURAUD;
            }
            else
            {
                state_far = STATE_SOLID;
            }

            // Setup the terrain rendering states
            if (textureLevel == 0)
            {
                // Define all the states the same as the far state
                state_mid = state_far;
                state_near = state_far;
                state_fore = state_far;
            }
            else
            {
                // Use textures
                if (filtering)
                {
                    state_mid = STATE_TEXTURE_FILTER;
                    state_near = STATE_TEXTURE_FILTER;
                    state_fore = STATE_TEXTURE_FILTER_PERSPECTIVE;

                    if (hazed)
                    {
                        if (smoothed) state_mid = STATE_TEXTURE_FILTER_POLYBLEND_GOURAUD;
                        else state_mid = STATE_TEXTURE_FILTER_POLYBLEND;
                    }
                }
                else
                {
                    state_mid = STATE_TEXTURE;
                    state_near = STATE_TEXTURE;
                    state_fore = STATE_TEXTURE_PERSPECTIVE;

                    if (hazed)
                    {
                        if (smoothed) state_mid = STATE_TEXTURE_POLYBLEND_GOURAUD;
                        else state_mid = STATE_TEXTURE_POLYBLEND;
                    }
                }
            }
#endif
            throw new NotImplementedException();
        }

        protected void ComputeBounds()
        {
#if TODO
            // To avoid rounding errors (or possibly some other minor computational bug)
            // I compute culling assuming a viewpoint somewhat behind the real
            // camera postion which will always yeild a conservative estimate.
            // To be really paranoid, the CULL_BACKUP_DISTANCE is
            // a function of field of view to get a constant linear margin.
            float cullMargin = LEVEL_POST_TO_WORLD(2, viewpoint.GetHighLOD());
            float backupDistance = cullMargin / (float)Math.Sin(diagonal_half_angle);
            Tpoint CullPoint;
            GetAt(ref CullPoint);
            CullPoint.x = X() - CullPoint.x * backupDistance;
            CullPoint.y = Y() - CullPoint.y * backupDistance;
            CullPoint.z = Z() - CullPoint.z * backupDistance;
            float distanceLimit = far_clip + backupDistance;


            // Get the vertical bounds of the terrain arround our viewpoint
            float areaFloor;
            float areaCeiling;
            areaFloor = viewpoint.GetTerrainFloor();		// -Z is up
            areaCeiling = viewpoint.GetTerrainCeiling();	// -Z is up


            //	First compute the front and back distances of the ground patch
            //  (front may be behind the viewer, in which case it will be negative)
            //  (back will never be behind the viewer and will never be negative)
            float top = Pitch() + diagonal_half_angle;
            float bottom = Pitch() - diagonal_half_angle;

            float aboveMin = areaFloor - CullPoint.z;		// -Z is up
            float aboveMax = areaCeiling - CullPoint.z;		// -Z is up

            float front = -1e30f;
            float back = -1e30f;

            if (bottom < -Constants.PI_OVER_2)
            {
                // bottom vector points down and backward
                if (aboveMin >= 0.0f)
                {
                    // We're above ground so intersect the bottom with min height
                    front = -aboveMin * (float)Math.Tan(-bottom - Constants.PI_OVER_2);
                    if (top > 0)
                    {
                        // top vector points up
                        back = distanceLimit;
                    }
                    else
                    {
                        // top vector points down
                        back = aboveMin * (float)Math.Tan(top + Constants.PI_OVER_2);
                    }
                }
                else
                {
                    // We're below ground 
                    if (top > 0)
                    {
                        // top is pointing upward, so intersect the top with min height
                        front = -aboveMin / (float)Math.Tan(top);
                        back = distanceLimit;
                    }
                    else
                    {
                        // We're below all terrain
                        front = 0.0f;
                        back = 0.0f;
                    }
                }
            }
            else if (bottom < 0)
            {
                // bottom vector points downward, but not backward
                if (aboveMin >= 0.0f)
                {
                    // We're above ground
                    if (top >= 0)
                    {
                        // the top vector points upward
                        back = distanceLimit;
                    }
                    else
                    {
                        // the top vector points downward as well
                        back = aboveMin * (float)Math.Tan(top + Constants.PI_OVER_2);
                    }
                    if (aboveMax > 0)
                    {
                        // we're above all terrain
                        front = aboveMax * (float)Math.Tan(bottom + Constants.PI_OVER_2);
                    }
                    else
                    {
                        // we're down in it
                        if (top > Constants.PI_OVER_2)
                        {
                            // top vector points backward
                            front = aboveMax / (float)Math.Tan(PI - top);
                        }
                        else
                        {
                            // top vector points upward, but not backward
                            front = 0.0f;
                        }
                    }
                }
                else
                {
                    // We're below ground
                    if (top >= 0)
                    {
                        // top is pointing upward, so intersect the top with min height
                        front = -aboveMin / (float)Math.Tan(top);
                        back = distanceLimit;
                    }
                    else
                    {
                        // the top vector points downward as well -- we're below all terrain
                        front = 0.0f;
                        back = 0.0f;
                    }
                }
            }
            else
            {
                // bottom vector points upward
                if (aboveMax > 0)
                {
                    // we're above all terrain
                    front = 0.0f;
                    back = 0.0f;
                }
                else if (aboveMin >= 0)
                {
                    // we're down in it
                    back = -aboveMax / (float)Math.Tan(bottom);
                    if (top > Constants.PI_OVER_2)
                    {
                        // top vector points backward
                        front = aboveMax * (float)Math.Tan(top - Constants.PI_OVER_2);
                    }
                    else
                    {
                        // top vector points up, but not backward
                        front = 0.0f;
                    }
                }
                else
                {
                    // We're below ground
                    back = -aboveMax / (float)Math.Tan(bottom);
                    if (top > Constants.PI_OVER_2)
                    {
                        // top vector points backward
                        front = aboveMax * (float)Math.Tan(top - Constants.PI_OVER_2);
                    }
                    else
                    {
                        // top vector points up, but not backward, so intersect the top with min height
                        front = -aboveMin / (float)Math.Tan(top);
                    }
                }
            }

            // Clamp the front and back values at the largest distance we could ever see
            if (back > distanceLimit) { back = distanceLimit; }
            if (front < -distanceLimit) { front = -distanceLimit; }
            if (front > distanceLimit) { front = distanceLimit; }

            //
            // Now go compute the left and right edges
            //
            float A;
            float B;
            float C;
            float temp;


            // Intersect the min elevation plane and the side of the view volume to get the right edge.
            // (left side is a mirror of the right side)

            // With the viewer looking down the x axis,
            // the right side of view volume is defined by the origin (eye point) and 
            Tpoint Corner1 = { 1.0f, (float)tan(diagonal_half_angle), 1.0f };
            Tpoint Corner2 = { 1.0f, (float)tan(diagonal_half_angle), -1.0f };


            // This plane should then be rotated in pitch about the y axis (look up/down)
            // Rotate the two points
            // NOTE:  We're computing the sides with pitch always pointing downward because
            //		  the plane/plane intersection does the wrong thing when looking up.
            // NOTE2: It would seem to me that I should have a minus sign in front of the pitch
            //		  term in the following two lines.  I may have a sign wrong somewhere else, 
            //        though, since it seems to work correctly as written...
            float sinPitch = (float)sin(fabs(Pitch()));
            float cosPitch = (float)cos(fabs(Pitch()));
            float sinYaw = (float)sin(Yaw());
            float cosYaw = (float)cos(Yaw());

            temp = Corner1.x * cosPitch - Corner1.z * sinPitch;
            Corner1.z = Corner1.x * sinPitch + Corner1.z * cosPitch;
            Corner1.x = temp;

            temp = Corner2.x * cosPitch - Corner2.z * sinPitch;
            Corner2.z = Corner2.x * sinPitch + Corner2.z * cosPitch;
            Corner2.x = temp;


            // Construct the normal to the plane using the cross product of the vectors from the eye point
            // (Note:  The normal components ARE the A, B, and C coefficients of the plane equation)
            A = Corner1.y * Corner2.z - Corner1.z * Corner2.y;
            B = Corner1.z * Corner2.x - Corner1.x * Corner2.z;
            C = Corner1.x * Corner2.y - Corner1.y * Corner2.x;

            // Since the origin is one of the points on the plane, D is necessarily zero
            //
            // The intersection of the plane with the minimum terrain feature height
            // is the worst case.  This is the plane z = (alt - areaFloor)
            // We use height above minimum since we set the eyepoint at the origin and positive z is down.
            //
            // The line equation given by performing the substitution for z in the plane equation is:
            // Ax + By + C*aboveMin + D = 0;
            // Where A B C and D are the coefficients of the plane equation
            //
            // The Line equation coefficients are:
            // a = A;
            // b = B;
            // c = C*aboveMin + D;
            //
            // Convert C in place so A, B and C become the line equation coefficients
            C *= aboveMin;


            // Now get two points on this line and two points on its reflection across the X axis
            // point one on each side is at X = front.  Point two on each side is at X = back.
            rightY1 = -(A * front + C) / B; rightY2 = -(A * back + C) / B;
            leftY1 = -rightY1; leftY2 = -rightY2;

            // Rotate the four points about the z axis to account for the viewer's yaw angle
            // and shift them out to the viewers location in world space
            rightX1 = cosYaw * front - sinYaw * rightY1 + CullPoint.x;
            rightY1 = sinYaw * front + cosYaw * rightY1 + CullPoint.y;

            rightX2 = cosYaw * back - sinYaw * rightY2 + CullPoint.x;
            rightY2 = sinYaw * back + cosYaw * rightY2 + CullPoint.y;

            leftX1 = cosYaw * front - sinYaw * leftY1 + CullPoint.x;
            leftY1 = sinYaw * front + cosYaw * leftY1 + CullPoint.y;

            leftX2 = cosYaw * back - sinYaw * leftY2 + CullPoint.x;
            leftY2 = sinYaw * back + cosYaw * leftY2 + CullPoint.y;


            //
            // Build the four edges bounding the ground patch
            //
            right_edge.SetupWithPoints(rightX1, rightY1, rightX2, rightY2);
            right_edge.Normalize();
            left_edge.SetupWithPoints(leftX1, leftY1, leftX2, leftY2);
            left_edge.Normalize();
            front_edge.SetupWithPoints(leftX1, leftY1, rightX1, rightY1);
            front_edge.Normalize();
            back_edge.SetupWithPoints(leftX2, leftY2, rightX2, rightY2);
            back_edge.Normalize();

#if NOTHING 
	// Sometimes we end up here with a bow tie shaped area -- bad -- so we hack a fix
	if (right_edge.LeftOf( leftX1, leftY1 )) {
		float t;
		t = leftX1;
		leftX1 = rightX1;
		rightX1 = t;
		t = leftY1;
		leftY1 = rightY1;
		rightY1 = t;

		right_edge.SetupWithPoints( rightX1, rightY1, rightX2, rightY2 );
		right_edge.Normalize();
		left_edge.SetupWithPoints( leftX1, leftY1, leftX2, leftY2 );
		left_edge.Normalize();
		front_edge.SetupWithPoints( leftX1, leftY1, rightX1, rightY1 );
		front_edge.Normalize();
		back_edge.SetupWithPoints( leftX2, leftY2, rightX2, rightY2 );
		back_edge.Normalize();
	}

	Debug.Assert( (leftX1 == rightX1 && leftY1 == rightY1) || right_edge.RightOf( leftX1, leftY1 ) );
	Debug.Assert( (leftX1 == rightX1 && leftY1 == rightY1) || left_edge.LeftOf( rightX1, rightY1 ) );
	Debug.Assert( right_edge.RightOf( leftX2, leftY2 ) );
	Debug.Assert( left_edge.LeftOf( rightX2, rightY2 ) );
#endif


#if  TWO_D_MAP_AVAILABLE
	if (twoDmode) {
		// Draw the view volume representation (assuming its in world space)
		SetColor( 0xFF0000A0 );
		Render2DLine((UInt16)((xRes>>1) + TWODSCALE*( WORLD_TO_FLOAT_GLOBAL_POST(rightY1-viewpoint.Y()) )),	(UInt16)((yRes>>1) - TWODSCALE*( WORLD_TO_GLOBAL_POST(rightX1-viewpoint.X()) )), 
					 (UInt16)((xRes>>1) + TWODSCALE*( WORLD_TO_FLOAT_GLOBAL_POST(rightY2-viewpoint.Y()) )),	(UInt16)((yRes>>1) - TWODSCALE*( WORLD_TO_GLOBAL_POST(rightX2-viewpoint.X()) )) );                           
		Render2DLine((UInt16)((xRes>>1) + TWODSCALE*( WORLD_TO_FLOAT_GLOBAL_POST(leftY1 -viewpoint.Y()) )),	(UInt16)((yRes>>1) - TWODSCALE*( WORLD_TO_GLOBAL_POST(leftX1 -viewpoint.X()) )),  
					 (UInt16)((xRes>>1) + TWODSCALE*( WORLD_TO_FLOAT_GLOBAL_POST(leftY2 -viewpoint.Y()) )),	(UInt16)((yRes>>1) - TWODSCALE*( WORLD_TO_GLOBAL_POST(leftX2 -viewpoint.X()) )) );                           
		Render2DLine((UInt16)((xRes>>1) + TWODSCALE*( WORLD_TO_FLOAT_GLOBAL_POST(leftY1 -viewpoint.Y()) )),	(UInt16)((yRes>>1) - TWODSCALE*( WORLD_TO_GLOBAL_POST(leftX1 -viewpoint.X()) )),  
					 (UInt16)((xRes>>1) + TWODSCALE*( WORLD_TO_FLOAT_GLOBAL_POST(rightY1-viewpoint.Y()) )),	(UInt16)((yRes>>1) - TWODSCALE*( WORLD_TO_GLOBAL_POST(rightX1-viewpoint.X()) )) );                           
		Render2DLine((UInt16)((xRes>>1) + TWODSCALE*( WORLD_TO_FLOAT_GLOBAL_POST(leftY2 -viewpoint.Y()) )),	(UInt16)((yRes>>1) - TWODSCALE*( WORLD_TO_GLOBAL_POST(leftX2 -viewpoint.X()) )),  
					 (UInt16)((xRes>>1) + TWODSCALE*( WORLD_TO_FLOAT_GLOBAL_POST(rightY2-viewpoint.Y()) )),	(UInt16)((yRes>>1) - TWODSCALE*( WORLD_TO_GLOBAL_POST(rightX2-viewpoint.X()) )) );
	}
#endif
#endif
            throw new NotImplementedException();
        }
        protected void BuildRingList()
        {
#if TODO
            int LOD;
            int startRing;
            int stopRing;
            int ring;


            Debug.Assert(IsReady());


            //
            // Start a new span list
            //
            firstEmptySpan = spanList;


            //
            // Setup the data we need for each LOD
            //
            for (LOD = viewpoint.GetLowLOD(); LOD >= viewpoint.GetHighLOD(); LOD--)
            {

                // Get the available range for each LOD
                LODdata[LOD].availablePostRange = viewpoint.GetAvailablePostRange(LOD);

                // Decide which post is to be the center of the rings at this level
                LODdata[LOD].centerRow = (int)WORLD_TO_LEVEL_POST(viewpoint.X(), LOD);
                LODdata[LOD].centerCol = (int)WORLD_TO_LEVEL_POST(viewpoint.Y(), LOD);

                // Determine the location of the glue row and column at the outside of this LOD region
                // NOTE:  These two must evaluate to zero or one, as they are used arithmetically
                LODdata[LOD].glueOnBottom = LODdata[LOD].centerRow & 1;
                LODdata[LOD].glueOnLeft = LODdata[LOD].centerCol & 1;


#if  TWO_D_MAP_AVAILABLE
		if (twoDmode) {
			char	message[80];
			SetColor( 0xFFFFFFFF );
			sprintf( message, "LOD%0d Glue: %s %s", LOD, 
				LODdata[LOD].glueOnBottom ? "Bottom" : "Top",
				LODdata[LOD].glueOnLeft   ? "Left"   : "Right" );
			ScreenText( 0.0f, 8.0f * LOD, message );
		}
#endif


                // Compute and record the camera space versions of the X, Y, and Z world space axes
                // Scale for LOD's horizontal step size
                LODdata[LOD].Xstep[0] = T.M11 * LEVEL_POST_TO_WORLD(1, LOD);
                LODdata[LOD].Xstep[1] = T.M21 * LEVEL_POST_TO_WORLD(1, LOD);
                LODdata[LOD].Xstep[2] = T.M31 * LEVEL_POST_TO_WORLD(1, LOD);

                // Scale for LOD's horizontal step size
                LODdata[LOD].Ystep[0] = T.M12 * LEVEL_POST_TO_WORLD(1, LOD);
                LODdata[LOD].Ystep[1] = T.M22 * LEVEL_POST_TO_WORLD(1, LOD);
                LODdata[LOD].Ystep[2] = T.M32 * LEVEL_POST_TO_WORLD(1, LOD);

                // Leave in world space units of feet
                LODdata[LOD].Zstep[0] = T.M13;
                LODdata[LOD].Zstep[1] = T.M23;
                LODdata[LOD].Zstep[2] = T.M33;
            }


            //
            // Determine and record which rings we need to draw at each LOD
            //

            // Start the rings at the most distant available post
            startRing = LODdata[viewpoint.GetLowLOD()].availablePostRange;

            // We will consider rings at each level of detail from lowest detail to highest
            for (LOD = viewpoint.GetLowLOD(); LOD >= viewpoint.GetHighLOD(); LOD--)
            {

                // Figure where to stop this LOD and move to the next
                if (LOD != viewpoint.GetHighLOD())
                {

                    // The stop ring is one ring in from the first available ring at the next LOD
                    stopRing = (LODdata[LOD - 1].availablePostRange >> 1) - 1;

                    // Make sure we have enough space for connectors at each LOD
                    if (stopRing > startRing - 3)
                    {
                        stopRing = startRing - 3;
                    }

                    // Stop at zero in any case
                    if (stopRing < 0)
                    {
                        stopRing = 0;
                    }
                }
                else
                {
                    stopRing = 0;
                }

                Debug.Assert(startRing <= maxSpanOffset);
                Debug.Assert(stopRing >= 0);

                for (ring = startRing; ring >= stopRing; ring--)
                {

                    Debug.Assert(firstEmptySpan < spanList + spanListMaxEntries);
                    Debug.Assert(ring <= LODdata[LOD].availablePostRange);

                    firstEmptySpan.ring = ring;
                    firstEmptySpan.LOD = LOD;

                    // Figure out the actual location of each side of this ring
                    firstEmptySpan.Tsector.insideEdge = LEVEL_POST_TO_WORLD(LODdata[LOD].centerRow + ring, LOD);
                    firstEmptySpan.Bsector.insideEdge = LEVEL_POST_TO_WORLD(LODdata[LOD].centerRow - ring + 1, LOD);
                    firstEmptySpan.Rsector.insideEdge = LEVEL_POST_TO_WORLD(LODdata[LOD].centerCol + ring, LOD);
                    firstEmptySpan.Lsector.insideEdge = LEVEL_POST_TO_WORLD(LODdata[LOD].centerCol - ring + 1, LOD);

                    firstEmptySpan++;
                }

                // If we've reached the viewer, stop now
                if (stopRing == 0)
                {
                    break;
                }

                // What is the next ring at the next LOD (back out 1 to get an extra ring for the glue)
                startRing = (stopRing << 1) + 1;
            }

            // Pad the end of the list to avoid overrunning the list when performing look ahead
            Debug.Assert(firstEmptySpan < spanList + spanListMaxEntries);
            if (firstEmptySpan != spanList)
            {
                firstEmptySpan.ring = -1;
                firstEmptySpan.LOD = (firstEmptySpan - 1).LOD;
                firstEmptySpan.Tsector.insideEdge = 0.0f;
                firstEmptySpan.Bsector.insideEdge = 0.0f;
                firstEmptySpan.Rsector.insideEdge = 0.0f;
                firstEmptySpan.Lsector.insideEdge = 0.0f;
            }
#endif
            throw new NotImplementedException();
        }
        protected void ClipHorizontalSectors()
        {
#if TODO
            SpanListEntry span = null;
            int LOD = 0;
            int w = 0, e = 0;
            float startLocation = 0.0F;
            BoundSegment[] eastBoundry = new BoundSegment[4];
            BoundSegment[] westBoundry = new BoundSegment[4];


            // Construct the ordered edge list (horizontal span case - X buckets (since X is North/Up))
            if (Yaw() > Math.PI)
            {
                // Necessarily, rightX2 >= leftX2
                if (rightX1 > rightX2)
                {
                    // Right front corner is north most
                    startLocation = rightX1;
                    westBoundry[0].edge = right_edge;
                    westBoundry[1].edge = back_edge;
                    westBoundry[2].edge = left_edge;
                    westBoundry[0].end = rightX2;
                    westBoundry[1].end = leftX2;
                    westBoundry[2].end = leftX1;
                    eastBoundry[0].edge = front_edge;
                    eastBoundry[1].edge = left_edge;
                    eastBoundry[2].edge = back_edge;
                    eastBoundry[0].end = leftX1;
                    eastBoundry[1].end = leftX2;
                    eastBoundry[2].end = rightX2;
                }
                else
                {
                    // Right back corner is north most
                    startLocation = rightX2;
                    westBoundry[0].edge = back_edge;
                    westBoundry[1].edge = left_edge;
                    westBoundry[2].edge = front_edge;
                    westBoundry[0].end = leftX2;
                    westBoundry[1].end = leftX1;
                    westBoundry[2].end = rightX1;
                    eastBoundry[0].edge = right_edge;
                    eastBoundry[1].edge = front_edge;
                    eastBoundry[2].edge = left_edge;
                    eastBoundry[0].end = rightX1;
                    eastBoundry[1].end = leftX1;
                    eastBoundry[2].end = leftX2;
                }
            }
            else
            {
                // Necessarily, rightX2 <= leftX2
                if (leftX1 > leftX2)
                {
                    // Left front corner is north most
                    startLocation = leftX1;
                    westBoundry[0].edge = front_edge;
                    westBoundry[1].edge = right_edge;
                    westBoundry[2].edge = back_edge;
                    westBoundry[0].end = rightX1;
                    westBoundry[1].end = rightX2;
                    westBoundry[2].end = leftX2;
                    eastBoundry[0].edge = left_edge;
                    eastBoundry[1].edge = back_edge;
                    eastBoundry[2].edge = right_edge;
                    eastBoundry[0].end = leftX2;
                    eastBoundry[1].end = rightX2;
                    eastBoundry[2].end = rightX1;
                }
                else
                {
                    // Left back corner is north most
                    startLocation = leftX2;
                    westBoundry[0].edge = left_edge;
                    westBoundry[1].edge = front_edge;
                    westBoundry[2].edge = right_edge;
                    westBoundry[0].end = leftX1;
                    westBoundry[1].end = rightX1;
                    westBoundry[2].end = rightX2;
                    eastBoundry[0].edge = back_edge;
                    eastBoundry[1].edge = right_edge;
                    eastBoundry[2].edge = front_edge;
                    eastBoundry[0].end = rightX2;
                    eastBoundry[1].end = rightX1;
                    eastBoundry[2].end = leftX1;
                }
            }

            // Terminated the edge chain with a point greater than any possible to ensure we see an "upturn"
            eastBoundry[3].end = MAX_POSTIVE_F;
            westBoundry[3].end = MAX_POSTIVE_F;


            // Force a fresh start by noting an illegal "current" LOD
            LOD = -1;

            // Now clip the top horizontal spans to the bounding region
            for (span = spanList; span != firstEmptySpan; span++)
            {

                // Spans are  empty until we get to the top corner of the bounding region
                if (span.Tsector.insideEdge > startLocation)
                {
                    span.Tsector.maxEndPoint = MAX_NEGATIVE_F;
                    span.Tsector.minEndPoint = MAX_POSTIVE_F;
                    continue;
                }

                // Restart the edge traversal since the spans can take a step backward at LOD change
                if (span.LOD != LOD)
                {
                    LOD = span.LOD;
                    e = w = 0;
                }

                // Switch controlling edges when required
                if (span.Tsector.insideEdge <= westBoundry[w].end)
                {

                    // Try to find an edge that will advance us at least one row
                    while (westBoundry[w + 1].end <= westBoundry[w].end)
                    {
                        w++;

                        if (span.Tsector.insideEdge > westBoundry[w].end)
                        {
                            break;
                        }
                    }

                    // Nothing on these spans if we've exhausted our edges
                    if (span.Tsector.insideEdge <= westBoundry[w].end)
                    {
                        span.Tsector.maxEndPoint = MAX_NEGATIVE_F;
                        span.Tsector.minEndPoint = MAX_POSTIVE_F;
                        continue;
                    }

                    Debug.Assert(w < 3);
                    Debug.Assert(westBoundry[w].end <= westBoundry[w - 1].end);
                }
                if (span.Tsector.insideEdge <= eastBoundry[e].end)
                {

                    // Try to find an edge that will advance us at least one row
                    while (eastBoundry[e + 1].end <= eastBoundry[e].end)
                    {
                        e++;

                        if (span.Tsector.insideEdge > eastBoundry[e].end)
                        {
                            break;
                        }
                    }

                    // We can't exhaust this edge without having already exhausted the one above
                    //			Debug.Assert( span.Tsector.insideEdge > eastBoundry[e].end );
                    //			Debug.Assert( e > 0 );		// We _must_ have taken a step in the while above
                    //			Debug.Assert( e < 3 );
                    //			Debug.Assert( eastBoundry[e].end <= eastBoundry[e-1].end );
                }


                // Compute the intersection of this span with the bounding region
                span.Tsector.minEndPoint = westBoundry[w].edge.Y((float)span.Tsector.insideEdge);
                span.Tsector.maxEndPoint = eastBoundry[e].edge.Y((float)span.Tsector.insideEdge);
            }

            // Force a fresh start by noting an illegal "current" LOD
            LOD = -1;

            // Now fill in the extents of spans at the bottom of the bounding region
            for (span = firstEmptySpan - 1; span >= spanList; span--)
            {

                // Spans are empty until we get to the top corner of the bounding region
                if (span.Bsector.insideEdge > startLocation)
                {
                    span.Bsector.maxEndPoint = MAX_NEGATIVE_F;
                    span.Bsector.minEndPoint = MAX_POSTIVE_F;
                    continue;
                }

                // Restart the edge traversal since the spans can take a step backward at LOD change
                if (span.LOD != LOD)
                {
                    LOD = span.LOD;
                    e = w = 0;
                }

                // Switch controlling edges when required
                if (span.Bsector.insideEdge <= westBoundry[w].end)
                {

                    // Try to find an edge that will advance us at least one row
                    while (westBoundry[w + 1].end <= westBoundry[w].end)
                    {
                        w++;

                        if (span.Bsector.insideEdge > westBoundry[w].end)
                        {
                            break;
                        }
                    }

                    // Nothing on these spans if we've exhausted our edges
                    if (span.Bsector.insideEdge <= westBoundry[w].end)
                    {
                        span.Bsector.maxEndPoint = MAX_NEGATIVE_F;
                        span.Bsector.minEndPoint = MAX_POSTIVE_F;
                        continue;
                    }

                    Debug.Assert(w < 3);
                    Debug.Assert(westBoundry[w].end <= westBoundry[w - 1].end);
                }
                if (span.Bsector.insideEdge <= eastBoundry[e].end)
                {

                    // Try to find an edge that will advance us at least one row
                    while (eastBoundry[e + 1].end <= eastBoundry[e].end)
                    {
                        e++;

                        if (span.Bsector.insideEdge > eastBoundry[e].end)
                        {
                            break;
                        }
                    }

                    // We can't exhaust this edge without having already exhausted the one above
                    //			Debug.Assert( e > 0 );		// We _must_ have taken a step in the while above
                    //			Debug.Assert( e < 3 );
                    //			Debug.Assert( eastBoundry[e].end <= eastBoundry[e-1].end );
                }


                // Compute the intersection of this span with the bounding region
                span.Bsector.minEndPoint = westBoundry[w].edge.Y((float)span.Bsector.insideEdge);
                span.Bsector.maxEndPoint = eastBoundry[e].edge.Y((float)span.Bsector.insideEdge);
            }
#endif 
            throw new NotImplementedException();
        }
        protected void ClipVerticalSectors()
        {
#if TODO
            SpanListEntry span = null;
            int LOD = 0;
            int n = 0, s = 0;
            float startLocation = 0.0F;
            BoundSegment[] northBoundry = new BoundSegment[4];
            BoundSegment[] southBoundry = new BoundSegment[4];


            // Construct the ordered edge list (horizontal span case - Y buckets (since Y is East/Right))
            if ((Yaw() < Constants.PI_OVER_2) || (Yaw() > 3.0f * Constants.PI_OVER_2))
            {
                // Necessarily, rightY2 > leftY2
                if (rightY1 > rightY2)
                {
                    // Right front corner is east most
                    startLocation = rightY1;
                    northBoundry[0].edge = right_edge;
                    northBoundry[1].edge = back_edge;
                    northBoundry[2].edge = left_edge;
                    northBoundry[0].end = rightY2;
                    northBoundry[1].end = leftY2;
                    northBoundry[2].end = leftY1;
                    southBoundry[0].edge = front_edge;
                    southBoundry[1].edge = left_edge;
                    southBoundry[2].edge = back_edge;
                    southBoundry[0].end = leftY1;
                    southBoundry[1].end = leftY2;
                    southBoundry[2].end = rightY2;
                }
                else
                {
                    // Right back corner is east most
                    startLocation = rightY2;
                    northBoundry[0].edge = back_edge;
                    northBoundry[1].edge = left_edge;
                    northBoundry[2].edge = front_edge;
                    northBoundry[0].end = leftY2;
                    northBoundry[1].end = leftY1;
                    northBoundry[2].end = rightY1;
                    southBoundry[0].edge = right_edge;
                    southBoundry[1].edge = front_edge;
                    southBoundry[2].edge = left_edge;
                    southBoundry[0].end = rightY1;
                    southBoundry[1].end = leftY1;
                    southBoundry[2].end = leftY2;
                }
            }
            else
            {
                // Necessarily, rightY2 <= leftY2
                if (leftY1 > leftY2)
                {
                    // Left front corner is east most
                    startLocation = leftY1;
                    northBoundry[0].edge = front_edge;
                    northBoundry[1].edge = right_edge;
                    northBoundry[2].edge = back_edge;
                    northBoundry[0].end = rightY1;
                    northBoundry[1].end = rightY2;
                    northBoundry[2].end = leftY2;
                    southBoundry[0].edge = left_edge;
                    southBoundry[1].edge = back_edge;
                    southBoundry[2].edge = right_edge;
                    southBoundry[0].end = leftY2;
                    southBoundry[1].end = rightY2;
                    southBoundry[2].end = rightY1;
                }
                else
                {
                    // Left back corner is east most
                    startLocation = leftY2;
                    northBoundry[0].edge = left_edge;
                    northBoundry[1].edge = front_edge;
                    northBoundry[2].edge = right_edge;
                    northBoundry[0].end = leftY1;
                    northBoundry[1].end = rightY1;
                    northBoundry[2].end = rightY2;
                    southBoundry[0].edge = back_edge;
                    southBoundry[1].edge = right_edge;
                    southBoundry[2].edge = front_edge;
                    southBoundry[0].end = rightY2;
                    southBoundry[1].end = rightY1;
                    southBoundry[2].end = leftY1;
                }
            }

            // Terminated the edge chain with a point greater than any possible to ensure we see an "upturn"
            northBoundry[3].end = MAX_POSTIVE_F;
            southBoundry[3].end = MAX_POSTIVE_F;


            // Force a fresh start by noting an illegal "current" LOD
            LOD = -1;

            // Now fill in the extents of spans along the right edge of the bounding region
            for (span = spanList; span != firstEmptySpan; span++)
            {

                // Spans are empty until we get to the top corner of the bounding region
                if (span.Rsector.insideEdge > startLocation)
                {
                    span.Rsector.minEndPoint = MAX_POSTIVE_F;
                    span.Rsector.maxEndPoint = MAX_NEGATIVE_F;
                    continue;
                }

                // Restart the edge traversal since the spans can take a step backward at LOD change
                if (span.LOD != LOD)
                {
                    LOD = span.LOD;
                    n = s = 0;
                }

                // Switch controlling edges when required
                if (span.Rsector.insideEdge <= southBoundry[s].end)
                {

                    // Try to find an edge that will advance us at least one row
                    while (southBoundry[s + 1].end <= southBoundry[s].end)
                    {
                        s++;

                        if (span.Rsector.insideEdge > southBoundry[s].end)
                        {
                            break;
                        }
                    }

                    // Nothing on these spans if we've exhausted our edges
                    if (span.Rsector.insideEdge <= southBoundry[s].end)
                    {
                        span.Rsector.minEndPoint = MAX_POSTIVE_F;
                        span.Rsector.maxEndPoint = MAX_NEGATIVE_F;
                        continue;
                    }

                    Debug.Assert(s < 3);
                    Debug.Assert(southBoundry[s].end <= southBoundry[s - 1].end);
                }
                if (span.Rsector.insideEdge <= northBoundry[n].end)
                {

                    // Try to find an edge that will advance us at least one row
                    while (northBoundry[n + 1].end <= northBoundry[n].end)
                    {
                        n++;

                        if (span.Rsector.insideEdge > northBoundry[n].end)
                        {
                            break;
                        }
                    }

                    // We can't exhaust this edge without having already exhausted the one above
                    //			Debug.Assert( n > 0 );		// We _must_ have taken a step in the while above
                    //			Debug.Assert( n < 3 );
                    //			Debug.Assert( northBoundry[n].end <= northBoundry[n-1].end );
                }


                // Compute the intersection of this span with the bounding region
                span.Rsector.minEndPoint = southBoundry[s].edge.X((float)span.Rsector.insideEdge);
                span.Rsector.maxEndPoint = northBoundry[n].edge.X((float)span.Rsector.insideEdge);
            }


            // Force a fresh start by noting an illegal "current" LOD
            LOD = -1;

            // Now fill in the extents of spans along the left edge of the bounding region
            for (span = firstEmptySpan - 1; span >= spanList; span--)
            {

                // Spans are empty until we get to the top corner of the bounding region
                if (span.Lsector.insideEdge > startLocation)
                {
                    span.Lsector.maxEndPoint = MAX_NEGATIVE_F;
                    span.Lsector.minEndPoint = MAX_POSTIVE_F;
                    continue;
                }

                // Restart the edge traversal since the spans can take a step backward at LOD change
                if (span.LOD != LOD)
                {
                    LOD = span.LOD;
                    n = s = 0;
                }

                // Switch controlling edges when required
                if (span.Lsector.insideEdge <= southBoundry[s].end)
                {

                    // Try to find an edge that will advance us at least one row
                    while (southBoundry[s + 1].end <= southBoundry[s].end)
                    {
                        s++;

                        if (span.Lsector.insideEdge > southBoundry[s].end)
                        {
                            break;
                        }
                    }

                    // Nothing on these spans if we've exhausted our edges
                    if (span.Lsector.insideEdge <= southBoundry[s].end)
                    {
                        span.Lsector.minEndPoint = MAX_POSTIVE_F;
                        span.Lsector.maxEndPoint = MAX_NEGATIVE_F;
                        continue;
                    }

                    Debug.Assert(s < 3);
                    Debug.Assert(southBoundry[s].end <= southBoundry[s - 1].end);
                }
                if (span.Lsector.insideEdge <= northBoundry[n].end)
                {

                    // Try to find an edge that will advance us at least one row
                    while (northBoundry[n + 1].end <= northBoundry[n].end)
                    {
                        n++;

                        if (span.Lsector.insideEdge > northBoundry[n].end)
                        {
                            break;
                        }
                    }

                    // We can't exhaust this edge without having already exhausted the one above
                    //			Debug.Assert( n > 0 );		// We _must_ have taken a step in the while above
                    //			Debug.Assert( n < 3 );
                    //			Debug.Assert( northBoundry[n].end <= northBoundry[n-1].end );
                }


                // Compute the intersection of this span with the bounding region
                span.Lsector.minEndPoint = southBoundry[s].edge.X((float)span.Lsector.insideEdge);
                span.Lsector.maxEndPoint = northBoundry[n].edge.X((float)span.Lsector.insideEdge);
            }
#endif
            throw new NotImplementedException();
        }

        protected void BuildCornerSet()
        {
#if TODO
            SpanListEntry span;


            // Move from inner ring outward
            for (span = firstEmptySpan - 1; span >= spanList; span--)
            {

                // The start/stop points were computed for the inside edges of each ring of squares.
                // Make sure the outter edge doesn't dictate a larger span.
                if (span != spanList)
                {
                    if (span.LOD == (span - 1).LOD)
                    {
                        // Normal case (look out one ring)
                        Debug.Assert((span - 1) >= spanList);
                        span.Tsector.minEndPoint = Math.Min(span.Tsector.minEndPoint, (span - 1).Tsector.minEndPoint);
                        span.Tsector.maxEndPoint = Math.Max(span.Tsector.maxEndPoint, (span - 1).Tsector.maxEndPoint);
                        span.Rsector.minEndPoint = Math.Min(span.Rsector.minEndPoint, (span - 1).Rsector.minEndPoint);
                        span.Rsector.maxEndPoint = Math.Max(span.Rsector.maxEndPoint, (span - 1).Rsector.maxEndPoint);
                        span.Bsector.minEndPoint = Math.Min(span.Bsector.minEndPoint, (span - 1).Bsector.minEndPoint);
                        span.Bsector.maxEndPoint = Math.Max(span.Bsector.maxEndPoint, (span - 1).Bsector.maxEndPoint);
                        span.Lsector.minEndPoint = Math.Min(span.Lsector.minEndPoint, (span - 1).Lsector.minEndPoint);
                        span.Lsector.maxEndPoint = Math.Max(span.Lsector.maxEndPoint, (span - 1).Lsector.maxEndPoint);
                    }
                    else
                    {
                        // Connector case (look out two rings to a lower LOD)
                        Debug.Assert((span - 2) >= spanList);
                        span.Tsector.minEndPoint = Math.Min(span.Tsector.minEndPoint, (span - 2).Tsector.minEndPoint);
                        span.Tsector.maxEndPoint = Math.Max(span.Tsector.maxEndPoint, (span - 2).Tsector.maxEndPoint);
                        span.Rsector.minEndPoint = Math.Min(span.Rsector.minEndPoint, (span - 2).Rsector.minEndPoint);
                        span.Rsector.maxEndPoint = Math.Max(span.Rsector.maxEndPoint, (span - 2).Rsector.maxEndPoint);
                        span.Bsector.minEndPoint = Math.Min(span.Bsector.minEndPoint, (span - 2).Bsector.minEndPoint);
                        span.Bsector.maxEndPoint = Math.Max(span.Bsector.maxEndPoint, (span - 2).Bsector.maxEndPoint);
                        span.Lsector.minEndPoint = Math.Min(span.Lsector.minEndPoint, (span - 2).Lsector.minEndPoint);
                        span.Lsector.maxEndPoint = Math.Max(span.Lsector.maxEndPoint, (span - 2).Lsector.maxEndPoint);
                    }
                }

                // Convert all the start/stop points into units of level posts
                span.Tsector.startDraw = Ttypes.WORLD_TO_LEVEL_POST(span.Tsector.minEndPoint, span.LOD);
                span.Tsector.stopDraw = Ttypes.WORLD_TO_LEVEL_POST(span.Tsector.maxEndPoint, span.LOD);
                span.Rsector.startDraw = Ttypes.WORLD_TO_LEVEL_POST(span.Rsector.minEndPoint, span.LOD);
                span.Rsector.stopDraw = Ttypes.WORLD_TO_LEVEL_POST(span.Rsector.maxEndPoint, span.LOD);
                span.Bsector.startDraw = Ttypes.WORLD_TO_LEVEL_POST(span.Bsector.minEndPoint, span.LOD);
                span.Bsector.stopDraw = Ttypes.WORLD_TO_LEVEL_POST(span.Bsector.maxEndPoint, span.LOD);
                span.Lsector.startDraw = Ttypes.WORLD_TO_LEVEL_POST(span.Lsector.minEndPoint, span.LOD);
                span.Lsector.stopDraw = Ttypes.WORLD_TO_LEVEL_POST(span.Lsector.maxEndPoint, span.LOD);

#if NOTHING 
		// If min and max fall onto the same level post, we can't tell the different between an empty
		// segment and a segment containing a single post.  This check resolves that ambiguity.
		// It may also prevent us from causing neighboring verticies to be transformed
		// when we won't, in fact be drawing anything.
		if ( span.Tsector.minEndPoint > span.Tsector.maxEndPoint ) {
			span.Tsector.startDraw = MAX_POSITIVE_I;
			span.Tsector.stopDraw  = MAX_NEGATIVE_I;
		}
		if ( span.Rsector.minEndPoint > span.Rsector.maxEndPoint ) {
			span.Rsector.startDraw = MAX_POSITIVE_I;
			span.Rsector.stopDraw  = MAX_NEGATIVE_I;
		}
		if ( span.Bsector.minEndPoint > span.Bsector.maxEndPoint ) {
			span.Bsector.startDraw = MAX_POSITIVE_I;
			span.Bsector.stopDraw  = MAX_NEGATIVE_I;
		}
		if ( span.Lsector.minEndPoint > span.Lsector.maxEndPoint ) {
			span.Lsector.startDraw = MAX_POSITIVE_I;
			span.Lsector.stopDraw  = MAX_NEGATIVE_I;
		}
#endif

                // Make all the start/stop points relative to the ring centers at each LOD
                span.Tsector.startDraw -= LODdata[span.LOD].centerCol;
                span.Tsector.stopDraw -= LODdata[span.LOD].centerCol;
                span.Rsector.startDraw -= LODdata[span.LOD].centerRow;
                span.Rsector.stopDraw -= LODdata[span.LOD].centerRow;
                span.Bsector.startDraw -= LODdata[span.LOD].centerCol;
                span.Bsector.stopDraw -= LODdata[span.LOD].centerCol;
                span.Lsector.startDraw -= LODdata[span.LOD].centerRow;
                span.Lsector.stopDraw -= LODdata[span.LOD].centerRow;
            }


            // AT THIS POINT:	startDraw and stopDraw in all the rings completly specify all the lower 
            //					left corner points for squares to be drawn, but may OVER specify them.
            //					TrimCornerSet() below will adjust and round the start/stop points as
            //					necessary.
#endif
            throw new NotImplementedException();
        }

        protected void TrimCornerSet()
        {
#if TODO
            SpanListEntry span;


            // Move from inner ring outward
            for (span = firstEmptySpan - 1; span >= spanList; span--)
            {

                if ((span < spanList + 2) || (span.LOD == (span - 2).LOD))
                {

                    // Normal Draw.
                    // Cut off all spans at their intersections with the y=x and y=-x lines
                    if (span.Tsector.startDraw < -span.ring)
                    {
                        span.Tsector.startDraw = -span.ring;
                    }
                    if (span.Tsector.stopDraw > span.ring)
                    {
                        span.Tsector.stopDraw = span.ring;
                    }
                    if (span.Rsector.startDraw < -span.ring + 1)
                    {
                        span.Rsector.startDraw = -span.ring + 1;
                    }
                    if (span.Rsector.stopDraw > span.ring - 1)
                    {
                        span.Rsector.stopDraw = span.ring - 1;
                    }
                    if (span.Bsector.startDraw < -span.ring)
                    {
                        span.Bsector.startDraw = -span.ring;
                    }
                    if (span.Bsector.stopDraw > span.ring)
                    {
                        span.Bsector.stopDraw = span.ring;
                    }
                    if (span.Lsector.startDraw < -span.ring + 1)
                    {
                        span.Lsector.startDraw = -span.ring + 1;
                    }
                    if (span.Lsector.stopDraw > span.ring - 1)
                    {
                        span.Lsector.stopDraw = span.ring - 1;
                    }

                }
                else
                {
                    int LOD = span.LOD;

                    if (LOD == (span - 1).LOD)
                    {

                        // Glue control span (also controls some connector drawing)
                        if (LODdata[LOD].glueOnBottom)
                        {
                            // For Glue
                            if (span.Bsector.startDraw < -span.ring + 1 - LODdata[LOD].glueOnLeft)
                            {
                                span.Bsector.startDraw = -span.ring + 1 - LODdata[LOD].glueOnLeft;
                            }
                            if (span.Bsector.stopDraw > span.ring - LODdata[LOD].glueOnLeft)
                            {
                                span.Bsector.stopDraw = span.ring - LODdata[LOD].glueOnLeft;
                            }

                            // For connector
                            if (LODdata[LOD].glueOnLeft)
                            {
                                // Bottom Left
                                // Clipping
                                if (span.Tsector.startDraw < -span.ring)
                                {
                                    span.Tsector.startDraw = -span.ring;
                                }
                                if (span.Tsector.stopDraw > span.ring)
                                {
                                    span.Tsector.stopDraw = span.ring;
                                }
                                if (span.Rsector.startDraw < -span.ring)
                                {
                                    span.Rsector.startDraw = -span.ring;
                                }
                                if (span.Rsector.stopDraw > span.ring)
                                {
                                    span.Rsector.stopDraw = span.ring;
                                }

                                // Rounding
                                span.Tsector.startDraw = (span.Tsector.startDraw - 1) | 1;
                                span.Tsector.stopDraw = (span.Tsector.stopDraw - 1) | 1;
                                span.Rsector.startDraw = (span.Rsector.startDraw - 1) | 1;
                                span.Rsector.stopDraw = (span.Rsector.stopDraw - 1) | 1;
                            }
                            else
                            {
                                // Bottom Right
                                // Clipping
                                if (span.Tsector.startDraw < -span.ring)
                                {
                                    span.Tsector.startDraw = -span.ring;
                                }
                                if (span.Tsector.stopDraw > span.ring)
                                {
                                    span.Tsector.stopDraw = span.ring;
                                }
                                if (span.Lsector.startDraw < -span.ring)
                                {
                                    span.Lsector.startDraw = -span.ring;
                                }
                                if (span.Lsector.stopDraw > span.ring)
                                {
                                    span.Lsector.stopDraw = span.ring;
                                }

                                // Rounding
                                span.Tsector.startDraw = span.Tsector.startDraw & ~1;
                                span.Tsector.stopDraw = span.Tsector.stopDraw & ~1;
                                span.Lsector.startDraw = (span.Lsector.startDraw - 1) | 1;
                                span.Lsector.stopDraw = (span.Lsector.stopDraw - 1) | 1;
                            }
                        }
                        else
                        {
                            // For Glue
                            if (span.Tsector.startDraw < -span.ring + 1 - LODdata[LOD].glueOnLeft)
                            {
                                span.Tsector.startDraw = -span.ring + 1 - LODdata[LOD].glueOnLeft;
                            }
                            if (span.Tsector.stopDraw > span.ring - LODdata[LOD].glueOnLeft)
                            {
                                span.Tsector.stopDraw = span.ring - LODdata[LOD].glueOnLeft;
                            }

                            // For connector
                            if (LODdata[LOD].glueOnLeft)
                            {
                                // Top Left
                                // Clipping
                                if (span.Bsector.startDraw < -span.ring)
                                {
                                    span.Bsector.startDraw = -span.ring;
                                }
                                if (span.Bsector.stopDraw > span.ring)
                                {
                                    span.Bsector.stopDraw = span.ring;
                                }
                                if (span.Rsector.startDraw < -span.ring)
                                {
                                    span.Rsector.startDraw = -span.ring;
                                }
                                if (span.Rsector.stopDraw > span.ring)
                                {
                                    span.Rsector.stopDraw = span.ring;
                                }

                                // Rounding
                                span.Bsector.startDraw = (span.Bsector.startDraw - 1) | 1;
                                span.Bsector.stopDraw = (span.Bsector.stopDraw - 1) | 1;
                                span.Rsector.startDraw = span.Rsector.startDraw & ~1;
                                span.Rsector.stopDraw = span.Rsector.stopDraw & ~1;
                            }
                            else
                            {
                                // Top Right
                                // Clipping
                                if (span.Bsector.startDraw < -span.ring)
                                {
                                    span.Bsector.startDraw = -span.ring;
                                }
                                if (span.Bsector.stopDraw > span.ring)
                                {
                                    span.Bsector.stopDraw = span.ring;
                                }
                                if (span.Lsector.startDraw < -span.ring)
                                {
                                    span.Lsector.startDraw = -span.ring;
                                }
                                if (span.Lsector.stopDraw > span.ring)
                                {
                                    span.Lsector.stopDraw = span.ring;
                                }

                                // Rounding
                                span.Bsector.startDraw = span.Bsector.startDraw & ~1;
                                span.Bsector.stopDraw = span.Bsector.stopDraw & ~1;
                                span.Lsector.startDraw = span.Lsector.startDraw & ~1;
                                span.Lsector.stopDraw = span.Lsector.stopDraw & ~1;
                            }
                        }

                        if (LODdata[LOD].glueOnLeft)
                        {
                            // For Glue
                            if (span.Lsector.startDraw < -span.ring + 1)
                            {
                                span.Lsector.startDraw = -span.ring + 1;
                            }
                            if (span.Lsector.stopDraw > span.ring - 1)
                            {
                                span.Lsector.stopDraw = span.ring - 1;
                            }
                        }
                        else
                        {
                            // For Glue
                            if (span.Rsector.startDraw < -span.ring + 1)
                            {
                                span.Rsector.startDraw = -span.ring + 1;
                            }
                            if (span.Rsector.stopDraw > span.ring - 1)
                            {
                                span.Rsector.stopDraw = span.ring - 1;
                            }
                        }

                    }
                    else
                    {

                        // Outter Xform span (controls some parts of connector drawing)
                        if (LODdata[LOD].glueOnBottom)
                        {
                            if (LODdata[LOD].glueOnLeft)
                            {
                                // Bottom Left
                                // Clipping
                                if (span.Bsector.startDraw < -span.ring)
                                {
                                    span.Bsector.startDraw = -span.ring;
                                }
                                if (span.Bsector.stopDraw > span.ring - 1)
                                {
                                    span.Bsector.stopDraw = span.ring - 1;
                                }
                                if (span.Lsector.startDraw < -span.ring)
                                {
                                    span.Lsector.startDraw = -span.ring;
                                }
                                if (span.Lsector.stopDraw > span.ring - 1)
                                {
                                    span.Lsector.stopDraw = span.ring - 1;
                                }
                                span.Tsector.startDraw = MAX_POSITIVE_I;
                                span.Tsector.stopDraw = MAX_NEGATIVE_I;
                                span.Rsector.startDraw = MAX_POSITIVE_I;
                                span.Rsector.stopDraw = MAX_NEGATIVE_I;

                                // Rounding
                                span.Bsector.startDraw = (span.Bsector.startDraw - 1) | 1;
                                span.Bsector.stopDraw = (span.Bsector.stopDraw - 1) | 1;
                                span.Lsector.startDraw = (span.Lsector.startDraw - 1) | 1;
                                span.Lsector.stopDraw = (span.Lsector.stopDraw - 1) | 1;
                            }
                            else
                            {
                                // Bottom Right
                                // Clipping
                                if (span.Bsector.startDraw < -span.ring + 1)
                                {
                                    span.Bsector.startDraw = -span.ring + 1;
                                }
                                if (span.Bsector.stopDraw > span.ring)
                                {
                                    span.Bsector.stopDraw = span.ring;
                                }
                                if (span.Rsector.startDraw < -span.ring)
                                {
                                    span.Rsector.startDraw = -span.ring;
                                }
                                if (span.Rsector.stopDraw > span.ring - 1)
                                {
                                    span.Rsector.stopDraw = span.ring - 1;
                                }
                                span.Tsector.startDraw = MAX_POSITIVE_I;
                                span.Tsector.stopDraw = MAX_NEGATIVE_I;
                                span.Lsector.startDraw = MAX_POSITIVE_I;
                                span.Lsector.stopDraw = MAX_NEGATIVE_I;

                                // Rounding
                                span.Bsector.startDraw = span.Bsector.startDraw & ~1;
                                span.Bsector.stopDraw = span.Bsector.stopDraw & ~1;
                                span.Rsector.startDraw = (span.Rsector.startDraw - 1) | 1;
                                span.Rsector.stopDraw = (span.Rsector.stopDraw - 1) | 1;
                            }
                        }
                        else
                        {
                            if (LODdata[LOD].glueOnLeft)
                            {
                                // Top Left
                                // Clipping
                                if (span.Tsector.startDraw < -span.ring)
                                {
                                    span.Tsector.startDraw = -span.ring;
                                }
                                if (span.Tsector.stopDraw > span.ring - 1)
                                {
                                    span.Tsector.stopDraw = span.ring - 1;
                                }
                                if (span.Lsector.startDraw < -span.ring + 1)
                                {
                                    span.Lsector.startDraw = -span.ring + 1;
                                }
                                if (span.Lsector.stopDraw > span.ring)
                                {
                                    span.Lsector.stopDraw = span.ring;
                                }
                                span.Bsector.startDraw = MAX_POSITIVE_I;
                                span.Bsector.stopDraw = MAX_NEGATIVE_I;
                                span.Rsector.startDraw = MAX_POSITIVE_I;
                                span.Rsector.stopDraw = MAX_NEGATIVE_I;

                                // Rounding
                                span.Tsector.startDraw = (span.Tsector.startDraw - 1) | 1;
                                span.Tsector.stopDraw = (span.Tsector.stopDraw - 1) | 1;
                                span.Lsector.startDraw = span.Lsector.startDraw & ~1;
                                span.Lsector.stopDraw = span.Lsector.stopDraw & ~1;
                            }
                            else
                            {
                                // Top Right
                                // Clipping
                                if (span.Tsector.startDraw < -span.ring + 1)
                                {
                                    span.Tsector.startDraw = -span.ring + 1;
                                }
                                if (span.Tsector.stopDraw > span.ring)
                                {
                                    span.Tsector.stopDraw = span.ring;
                                }
                                if (span.Rsector.startDraw < -span.ring + 1)
                                {
                                    span.Rsector.startDraw = -span.ring + 1;
                                }
                                if (span.Rsector.stopDraw > span.ring)
                                {
                                    span.Rsector.stopDraw = span.ring;
                                }
                                span.Bsector.startDraw = MAX_POSITIVE_I;
                                span.Bsector.stopDraw = MAX_NEGATIVE_I;
                                span.Lsector.startDraw = MAX_POSITIVE_I;
                                span.Lsector.stopDraw = MAX_NEGATIVE_I;

                                // Rounding
                                span.Tsector.startDraw = span.Tsector.startDraw & ~1;
                                span.Tsector.stopDraw = span.Tsector.stopDraw & ~1;
                                span.Rsector.startDraw = span.Rsector.startDraw & ~1;
                                span.Rsector.stopDraw = span.Rsector.stopDraw & ~1;
                            }
                        }
                    }
                }
            }


            // AT THIS POINT:	startDraw and stopDraw in all the rings completly and exactly specify
            //					the lower left corner points for squares to be drawn.
#endif
            throw new NotImplementedException();
        }

        protected void BuildVertexSet()
        {
#if TODO
            SpanListEntry* span;
            SpanListEntry* innerSpan;
            SpanListEntry* outterSpan;
            SpanListEntry* controlSpan;
            int LOD;
            int lowPos;


            // First clear the list of spans to be transformed
            for (span = spanList; span < firstEmptySpan; span++)
            {
                span.Tsector.startXform = MAX_POSITIVE_I;
                span.Tsector.stopXform = MAX_NEGATIVE_I;
                span.Rsector.startXform = MAX_POSITIVE_I;
                span.Rsector.stopXform = MAX_NEGATIVE_I;
                span.Bsector.startXform = MAX_POSITIVE_I;
                span.Bsector.stopXform = MAX_NEGATIVE_I;
                span.Lsector.startXform = MAX_POSITIVE_I;
                span.Lsector.stopXform = MAX_NEGATIVE_I;
            }


            // Move from outter ring inward (same traversal as the drawing loop will use)
            for (span = spanList + 1; span < firstEmptySpan; span++)
            {

                if (span.LOD == (span + 1).LOD)
                {

                    // We will call DrawTerrainRing on this span
                    span.Tsector.startXform = Math.Min(span.Tsector.startDraw, span.Tsector.startXform);
                    span.Tsector.stopXform = Math.Max(span.Tsector.stopDraw + 1, span.Tsector.stopXform);
                    span.Rsector.startXform = Math.Min(span.Rsector.startDraw, span.Rsector.startXform);
                    span.Rsector.stopXform = Math.Max(span.Rsector.stopDraw + 1, span.Rsector.stopXform);
                    span.Bsector.startXform = Math.Min(span.Bsector.startDraw, span.Bsector.startXform);
                    span.Bsector.stopXform = Math.Max(span.Bsector.stopDraw + 1, span.Bsector.stopXform);
                    span.Lsector.startXform = Math.Min(span.Lsector.startDraw, span.Lsector.startXform);
                    span.Lsector.stopXform = Math.Max(span.Lsector.stopDraw + 1, span.Lsector.stopXform);

                    (span - 1).Tsector.startXform = Math.Min(span.Tsector.startDraw, (span - 1).Tsector.startXform);
                    (span - 1).Tsector.stopXform = Math.Max(span.Tsector.stopDraw + 1, (span - 1).Tsector.stopXform);
                    (span - 1).Rsector.startXform = Math.Min(span.Rsector.startDraw, (span - 1).Rsector.startXform);
                    (span - 1).Rsector.stopXform = Math.Max(span.Rsector.stopDraw + 1, (span - 1).Rsector.stopXform);
                    (span + 1).Bsector.startXform = Math.Min(span.Bsector.startDraw, (span + 1).Bsector.startXform);
                    (span + 1).Bsector.stopXform = Math.Max(span.Bsector.stopDraw + 1, (span + 1).Bsector.stopXform);
                    (span + 1).Lsector.startXform = Math.Min(span.Lsector.startDraw, (span + 1).Lsector.startXform);
                    (span + 1).Lsector.stopXform = Math.Max(span.Lsector.stopDraw + 1, (span + 1).Lsector.stopXform);

                }
                else
                {

                    // Skip the inner transform ring (last ring at lower detail level)
                    span++;

                    LOD = span.LOD;

                    // We'll call draw ConnectorRing on this span (outter xform)
                    // TOP
                    if (LODdata[LOD].glueOnBottom)
                    {
                        innerSpan = span + 1;		// "glue control"
                        controlSpan = span + 1;		// "glue control"
                        outterSpan = span - 2;		// "last drawn"
                    }
                    else
                    {
                        innerSpan = span;			// "outter xform"
                        controlSpan = span;			// "outter xform"
                        outterSpan = span - 2;		// "last drawn"
                    }
                    innerSpan.Tsector.startXform = Math.Min(controlSpan.Tsector.startDraw, innerSpan.Tsector.startXform);
                    innerSpan.Tsector.stopXform = Math.Max(controlSpan.Tsector.stopDraw + 2, innerSpan.Tsector.stopXform);
                    lowPos = (controlSpan.Tsector.startDraw + LODdata[LOD].glueOnLeft) >> 1;
                    outterSpan.Tsector.startXform = Math.Min(lowPos, outterSpan.Tsector.startXform);
                    lowPos = (controlSpan.Tsector.stopDraw + 2 + LODdata[LOD].glueOnLeft) >> 1;
                    outterSpan.Tsector.stopXform = Math.Max(lowPos, outterSpan.Tsector.stopXform);

                    // RIGHT
                    if (LODdata[LOD].glueOnLeft)
                    {
                        innerSpan = span + 1;		// "glue control"
                        controlSpan = span + 1;		// "glue control"
                        outterSpan = span - 2;		// "last drawn"
                    }
                    else
                    {
                        innerSpan = span;			// "outter xform"
                        controlSpan = span;			// "outter xform"
                        outterSpan = span - 2;		// "last drawn"
                    }
                    innerSpan.Rsector.startXform = Math.Min(innerSpan.Rsector.startDraw, innerSpan.Rsector.startXform);
                    innerSpan.Rsector.stopXform = Math.Max(innerSpan.Rsector.stopDraw + 2, innerSpan.Rsector.stopXform);
                    lowPos = (innerSpan.Rsector.startDraw + LODdata[LOD].glueOnBottom) >> 1;
                    outterSpan.Rsector.startXform = Math.Min(lowPos, outterSpan.Rsector.startXform);
                    lowPos = (innerSpan.Rsector.stopDraw + 2 + LODdata[LOD].glueOnBottom) >> 1;
                    outterSpan.Rsector.stopXform = Math.Max(lowPos, outterSpan.Rsector.stopXform);

                    // BOTTOM
                    if (LODdata[LOD].glueOnBottom)
                    {
                        innerSpan = span + 1;		// "glue control"
                        controlSpan = span;			// "outter xform"
                        outterSpan = span - 1;		// "inner xform"
                    }
                    else
                    {
                        innerSpan = span + 2;		// "first normal draw"
                        controlSpan = span + 1;		// "glue control"
                        outterSpan = span - 1;		// "inner xform"
                    }
                    innerSpan.Bsector.startXform = Math.Min(controlSpan.Bsector.startDraw, innerSpan.Bsector.startXform);
                    innerSpan.Bsector.stopXform = Math.Max(controlSpan.Bsector.stopDraw + 2, innerSpan.Bsector.stopXform);
                    lowPos = (controlSpan.Bsector.startDraw + LODdata[LOD].glueOnLeft) >> 1;
                    outterSpan.Bsector.startXform = Math.Min(lowPos, outterSpan.Bsector.startXform);
                    lowPos = (controlSpan.Bsector.stopDraw + 2 + LODdata[LOD].glueOnLeft) >> 1;
                    outterSpan.Bsector.stopXform = Math.Max(lowPos, outterSpan.Bsector.stopXform);

                    // LEFT
                    if (LODdata[LOD].glueOnLeft)
                    {
                        innerSpan = span + 1;		// "glue control"
                        controlSpan = span;			// "outter xform"
                        outterSpan = span - 1;		// "inner xform"
                    }
                    else
                    {
                        innerSpan = span + 2;		// "first normal draw"
                        controlSpan = span + 1;		// "glue control"
                        outterSpan = span - 1;		// "inner xform"
                    }
                    innerSpan.Lsector.startXform = Math.Min(controlSpan.Lsector.startDraw, innerSpan.Lsector.startXform);
                    innerSpan.Lsector.stopXform = Math.Max(controlSpan.Lsector.stopDraw + 2, innerSpan.Lsector.stopXform);
                    lowPos = (controlSpan.Lsector.startDraw + LODdata[LOD].glueOnBottom) >> 1;
                    outterSpan.Lsector.startXform = Math.Min(lowPos, outterSpan.Lsector.startXform);
                    lowPos = (controlSpan.Lsector.stopDraw + 2 + LODdata[LOD].glueOnBottom) >> 1;
                    outterSpan.Lsector.stopXform = Math.Max(lowPos, outterSpan.Lsector.stopXform);

                    span++;

                    // We'll call draw gap filler on this span
                    if (LODdata[LOD].glueOnBottom)
                    {
                        span.Bsector.startXform = Math.Min(span.Bsector.startDraw, span.Bsector.startXform);
                        span.Bsector.stopXform = Math.Max(span.Bsector.stopDraw + 1, span.Bsector.stopXform);
                        (span + 1).Bsector.startXform = Math.Min(span.Bsector.startDraw, (span + 1).Bsector.startXform);
                        (span + 1).Bsector.stopXform = Math.Max(span.Bsector.stopDraw + 1, (span + 1).Bsector.stopXform);
                    }
                    else
                    {
                        span.Tsector.startXform = Math.Min(span.Tsector.startDraw, span.Tsector.startXform);
                        span.Tsector.stopXform = Math.Max(span.Tsector.stopDraw + 1, span.Tsector.stopXform);
                        (span - 1).Tsector.startXform = Math.Min(span.Tsector.startDraw, (span - 1).Tsector.startXform);
                        (span - 1).Tsector.stopXform = Math.Max(span.Tsector.stopDraw + 1, (span - 1).Tsector.stopXform);
                    }
                    if (LODdata[span.LOD].glueOnLeft)
                    {
                        span.Lsector.startXform = Math.Min(span.Lsector.startDraw, span.Lsector.startXform);
                        span.Lsector.stopXform = Math.Max(span.Lsector.stopDraw + 1, span.Lsector.stopXform);
                        (span + 1).Lsector.startXform = Math.Min(span.Lsector.startDraw, (span + 1).Lsector.startXform);
                        (span + 1).Lsector.stopXform = Math.Max(span.Lsector.stopDraw + 1, (span + 1).Lsector.stopXform);
                    }
                    else
                    {
                        span.Rsector.startXform = Math.Min(span.Rsector.startDraw, span.Rsector.startXform);
                        span.Rsector.stopXform = Math.Max(span.Rsector.stopDraw + 1, span.Rsector.stopXform);
                        (span - 1).Rsector.startXform = Math.Min(span.Rsector.startDraw, (span - 1).Rsector.startXform);
                        (span - 1).Rsector.stopXform = Math.Max(span.Rsector.stopDraw + 1, (span - 1).Rsector.stopXform);
                    }
                }
            }


#if NOTHING 
	// Try to eliminate overlapping vertex entries in adjacent rows and columns
	// Move from outter ring inward
	// TODO:  Fix this -- it occasionally removes verticies we actually need for connector rings
	// Because construction in vertical can require verts in the horizontal (and vis-versa),
	// we'd have to find the associated span and add in the verticies we want to avoid doing
	// ourselves -- probably not worth the trouble.
	for ( span = spanList; span<firstEmptySpan; span++ ) {
		span.Tsector.startXform = Math.Max( -span.ring-1, span.Tsector.startXform );
		span.Tsector.stopXform  = Math.Min(  span.ring+1, span.Tsector.stopXform  );
		span.Rsector.startXform = Math.Max( -span.ring-1, span.Rsector.startXform );
		span.Rsector.stopXform  = Math.Min(  span.ring+1, span.Rsector.stopXform  );
		span.Bsector.startXform = Math.Max( -span.ring-1, span.Bsector.startXform );
		span.Bsector.stopXform  = Math.Min(  span.ring+1, span.Bsector.stopXform  );
		span.Lsector.startXform = Math.Max( -span.ring-1, span.Lsector.startXform );
		span.Lsector.stopXform  = Math.Min(  span.ring+1, span.Lsector.stopXform  );
	}
#endif
#endif
            throw new NotImplementedException();
        }

        protected void TransformVertexSet()
        {
#if TODO
            SpanListEntry* span;
            int LOD;
            int ring;
            SpanMinMax* sector;

            // Move from inner ring outward
            for (span = firstEmptySpan - 1; span >= spanList; span--)
            {
                LOD = span.LOD;
                ring = span.ring;

                // TOP_SPAN
                sector = &span.Tsector;
                TransformRun(ring, sector.startXform, sector.stopXform - sector.startXform, LOD, true);

                // RIGHT_SPAN
                sector = &span.Rsector;
                TransformRun(sector.startXform, ring, sector.stopXform - sector.startXform, LOD, false);

                // BOTTOM_SPAN
                sector = &span.Bsector;
                TransformRun(-ring, sector.startXform, sector.stopXform - sector.startXform, LOD, true);

                // LEFT_SPAN
                sector = &span.Lsector;
                TransformRun(sector.startXform, -ring, sector.stopXform - sector.startXform, LOD, false);
            }
#endif
            throw new NotImplementedException();
        }

        protected void TransformRun(int row, int col, int stop, int LOD, bool do_row)
        {
#if TODO
            TerrainVertex vert;
            Tpost post;
            int levelRow, levelCol, levelStop;
            float scratch_x, scratch_y, scratch_z;
            float x, y, z, dz;
            int* pChange;
            float* hVector;
            float* zVector;
            int vertStep;


            // Dump out if we have nothing to do
            if (run <= 0)
                return;

            // Select which variable to increment based on whether we're doing rows or columns
            if (do_row)
            {
                pChange = &levelCol;
                hVector = LODdata[LOD].Ystep;
                vertStep = 1;
            }
            else
            {
                pChange = &levelRow;
                hVector = LODdata[LOD].Xstep;
                vertStep = maxSpanExtent;
            }
            zVector = LODdata[LOD].Zstep;


            // Find the coordinates of the first post to transform FROM
            levelRow = row + LODdata[LOD].centerRow;
            levelCol = col + LODdata[LOD].centerCol;
            levelStop = *pChange + run;


            // Find the storage location for the first vertex to transform INTO
            vert = vertexBuffer[LOD] + maxSpanExtent * row + col;

            // Get the this post from the terrain database
            post = viewpoint.GetPost(levelRow, levelCol, LOD);
            Debug.Assert(post);

            // Compute our world space starting location
            x = LEVEL_POST_TO_WORLD(levelRow, LOD);
            y = LEVEL_POST_TO_WORLD(levelCol, LOD);

            // This part does rotation, translation, and scaling on the initial point
            // Note, we're swapping the x and z axes here to get from z up/down to z far/near
            // then we're swapping the x and y axes to get into conventional screen pixel coordinates
            z = post.z;
            scratch_z = T.M11 * x + T.M12 * y + T.M13 * z + move.x;
            scratch_x = T.M21 * x + T.M22 * y + T.M23 * z + move.y;
            scratch_y = T.M31 * x + T.M32 * y + T.M33 * z + move.z;


            // Transform all the rest of the verteces (will break out below)
            while (true)
            {

                Debug.Assert(vert >= vertexBuffer[LOD] - maxSpanExtent * maxSpanOffset - maxSpanOffset);
                Debug.Assert(vert <= vertexBuffer[LOD] + maxSpanExtent * maxSpanOffset + maxSpanOffset);

                // Store a pointer to the source post in the transformed vertex structure
                vert.post = post;


#if  TWO_D_MAP_AVAILABLE
	if (twoDmode) {
		vert.x = (xRes>>1) + TWODSCALE*((float)(levelCol << LOD) - WORLD_TO_FLOAT_GLOBAL_POST(viewpoint.Y()));
		vert.y = (yRes>>1) - TWODSCALE*((float)(levelRow << LOD) - WORLD_TO_FLOAT_GLOBAL_POST(viewpoint.X()));

		vert.clipFlag = ON_SCREEN;

		vert.r = 0.3f;
		vert.g = 0.1f;
		vert.b = 0.1f;
		vert.a = 0.5f;

		// Draw a marker at this vertex location
		SetColor( 0x80808080 );
		Render2DTri((UInt16)(vert.x-1), (UInt16)(vert.y-1),
					(UInt16)(vert.x+1), (UInt16)(vert.y-1),
					(UInt16)(vert.x+1), (UInt16)(vert.y+1));
		Render2DTri((UInt16)(vert.x-1), (UInt16)(vert.y-1),
					(UInt16)(vert.x+1), (UInt16)(vert.y+1),
					(UInt16)(vert.x-1), (UInt16)(vert.y+1));
	} else {
#endif
                // Now determine if the point is out behind us or to the sides
                // See GetRangeClipFlags(), GetHorizontalClipFlags(), and GetVerticalClipFlags()
                vert.clipFlag = ClippingFlags.ON_SCREEN;

                if (scratch_z < NEAR_CLIP)
                {
                    vert.clipFlag |= ClippingFlags.CLIP_NEAR;
                }

                if (fabs(scratch_y) > scratch_z)
                {
                    if (scratch_y > scratch_z)
                    {
                        vert.clipFlag |= ClippingFlags.CLIP_BOTTOM;
                    }
                    else
                    {
                        vert.clipFlag |= ClippingFlags.CLIP_TOP;
                    }
                }

                if (Math.Abs(scratch_x) > scratch_z)
                {
                    if (scratch_x > scratch_z)
                    {
                        vert.clipFlag |= ClippingFlags.CLIP_RIGHT;
                    }
                    else
                    {
                        vert.clipFlag |= ClippingFlags.CLIP_LEFT;
                    }
                }


                vert.csX = scratch_x;
                vert.csY = scratch_y;
                vert.csZ = scratch_z;


                // Finally, do the perspective divide and scale and shift into screen space
                if (!(vert.clipFlag & ClippingFlags.CLIP_NEAR))
                {
                    Debug.Assert(scratch_z > 0.0f);
                    float OneOverZ = 1.0f / scratch_z;
                    vert.x = viewportXtoPixel(scratch_x * OneOverZ);
                    vert.y = viewportYtoPixel(scratch_y * OneOverZ);
                    vert.q = scratch_z * Q_SCALE;
                }


                // Do any color computations required for this post
                ComputeVertexColor(vert, post, scratch_z);


#if  TWO_D_MAP_AVAILABLE
	}
#endif

                //
                // Break out of our loop if we need to
                //
                if (*pChange >= levelStop)
                {
                    break;
                }


                // Advance to the next post to transform
                vert += vertStep;
                (*pChange)++;

                // Get the this post from the terrain database
                post = viewpoint.GetPost(levelRow, levelCol, LOD);
                Debug.Assert(post);

                // Compute the new transformed location based on the known horizontal
                // step, and the vertical difference between this post and the previous one
                dz = post.z - z;
                z = post.z;
                scratch_z += hVector[0] + dz * zVector[0];
                scratch_x += hVector[1] + dz * zVector[1];
                scratch_y += hVector[2] + dz * zVector[2];
            }
#endif
            throw new NotImplementedException();
        }

        protected virtual void ComputeVertexColor(out TerrainVertex vert, Tpost post, float distance)
        {
#if TODO
            float alpha;
            float fog;
            float inv_fog;


            // Get the color information from the post and optionally fog it
            if (distance > haze_start + haze_depth)
            {

                vert.r = haze_ground_color.r;
                vert.g = haze_ground_color.g;
                vert.b = haze_ground_color.b;
                vert.a = FOG_MIN;

                vert.RenderingStateHandle = state_far;

            }
            else if (distance < PERSPECTIVE_RANGE)
            {

                vert.r = TheMap.DarkColorTable[post.colorIndex].r;
                vert.g = TheMap.DarkColorTable[post.colorIndex].g;
                vert.b = TheMap.DarkColorTable[post.colorIndex].b;
                vert.a = FOG_MAX;

                vert.RenderingStateHandle = state_fore;

            }
            else if (!hazed && distance < haze_start)
            {

                vert.r = TheMap.DarkColorTable[post.colorIndex].r;
                vert.g = TheMap.DarkColorTable[post.colorIndex].g;
                vert.b = TheMap.DarkColorTable[post.colorIndex].b;
                vert.a = FOG_MAX;

                vert.RenderingStateHandle = state_near;

            }
            else
            {

                fog = GetValleyFog(distance, post.z);
                if (fog > FOG_MAX) fog = FOG_MAX;
                if (fog < FOG_MIN) fog = FOG_MIN;
                inv_fog = 1.0f - fog;

                vert.r = (fog * haze_ground_color.r + inv_fog * TheMap.DarkColorTable[post.colorIndex].r);
                vert.g = (fog * haze_ground_color.g + inv_fog * TheMap.DarkColorTable[post.colorIndex].g);
                vert.b = (fog * haze_ground_color.b + inv_fog * TheMap.DarkColorTable[post.colorIndex].b);

                if (distance > blend_start + blend_depth)
                {
                    vert.a = FOG_MIN;
                    vert.RenderingStateHandle = state_far;
                }
                else
                {
                    alpha = (distance - blend_start) / blend_depth;
                    if (alpha > FOG_MAX) alpha = FOG_MAX;
                    if (alpha < fog) alpha = fog;

                    vert.a = 1.0f - alpha;
                    vert.RenderingStateHandle = state_mid;
                }
            }
#endif
            throw new NotImplementedException();
        }

        protected void PreSceneCloudOcclusion(float percent, DWORD color)
        {
#if TODO
            // If we're in software, we turn on the special color munging function
            if (image.GetDisplayDevice().IsHardware())
            {

                // Save the cloud color with alpha for post processing use.
                cloudColor = (color & 0x00FFFFFF) | (FloatToInt32(percent * 255.9f) << 24);

            }
            else
            {

                // Set the MPR color correction terms to get "fade out"
                // TODO:  Update this once Marc's changes are in...
#if !NOTHING
                context.SetColorCorrection(color, percent);
#else
		// This is a total hack to get "green" results on MFDs:
		float correction = 1.0f + percent * percent * 100.0f;

		// Red
		if (((color & 0x000000FF) == 0x0 ) &&
			((color & 0x0000FF00) >= 0x10) &&
			((color & 0x00FF0000) == 0x0 )) {

			// Guess that we're on a green display
			context.SetState( MPR_STA_GAMMA_RED,   (DWORD)(1.0f) );
			context.SetState( MPR_STA_GAMMA_GREEN, (DWORD)(correction) );
			context.SetState( MPR_STA_GAMMA_BLUE,  (DWORD)(1.0f) );
		} else {
			// Assume a color display
			context.SetState( MPR_STA_GAMMA_RED,   (DWORD)(correction) );
			context.SetState( MPR_STA_GAMMA_GREEN, (DWORD)(correction) );
			context.SetState( MPR_STA_GAMMA_BLUE,  (DWORD)(correction) );
		}
#endif
            }
#endif
            throw new NotImplementedException();
        }

        protected void DrawTerrainRing(SpanListEntry span)
        {
#if TODO
            int LOD = span.LOD;
            int r, c;
            int crossOver;


            // TOP_SPAN -- Horizontal (Sector 0 and 7)
            r = span.ring;

            crossOver = Math.Max(span.Tsector.startDraw, 0);
            for (c = span.Tsector.stopDraw; c >= crossOver; c--) DrawTerrainSquare(r, c, LOD);

            crossOver = Math.Min(span.Tsector.stopDraw, -1);
            for (c = span.Tsector.startDraw; c <= crossOver; c++) DrawTerrainSquare(r, c, LOD);


            // RIGHT_SPAN -- Vertical (Sector 1 and 2)
            c = span.ring;

            crossOver = Math.Max(span.Rsector.startDraw, 0);
            for (r = span.Rsector.stopDraw; r >= crossOver; r--) DrawTerrainSquare(r, c, LOD);

            crossOver = Math.Min(span.Rsector.stopDraw, -1);
            for (r = span.Rsector.startDraw; r <= crossOver; r++) DrawTerrainSquare(r, c, LOD);


            // BOTTOM_SPAN -- Horizontal (Sector 3 and 4)
            r = -span.ring;

            crossOver = Math.Max(span.Bsector.startDraw, 0);
            for (c = span.Bsector.stopDraw; c >= crossOver; c--) DrawTerrainSquare(r, c, LOD);

            crossOver = Math.Min(span.Bsector.stopDraw, -1);
            for (c = span.Bsector.startDraw; c <= crossOver; c++) DrawTerrainSquare(r, c, LOD);


            // LEFT_SPAN -- Vertical (Sector 5 and 6)
            c = -span.ring;

            crossOver = Math.Max(span.Lsector.startDraw, 0);
            for (r = span.Lsector.stopDraw; r >= crossOver; r--) DrawTerrainSquare(r, c, LOD);

            crossOver = Math.Min(span.Lsector.stopDraw, -1);
            for (r = span.Lsector.startDraw; r <= crossOver; r++) DrawTerrainSquare(r, c, LOD);
        #endif
            throw new NotImplementedException();
        }
        protected void DrawConnectorRing(SpanListEntry outterSpan)
        {
#if TODO
            int LOD;
            int crossOver;
            int r, c;
            SpanListEntry span;

            LOD = outterSpan.LOD;


            // TOP_SPAN -- Horizontal (Sector 0 and 7)
            span = LODdata[LOD].glueOnBottom ? outterSpan + 1 : outterSpan;

            crossOver = Math.Max(span.Tsector.startDraw, LODdata[LOD].glueOnLeft);
            for (c = span.Tsector.stopDraw; c >= crossOver; c -= 2) DrawUpConnector(span.ring, c, LOD);

            crossOver = Math.Min(span.Tsector.stopDraw, -2 + LODdata[LOD].glueOnLeft);
            for (c = span.Tsector.startDraw; c <= crossOver; c += 2) DrawUpConnector(span.ring, c, LOD);


            // RIGHT_SPAN -- Vertical (Sector 1 and 2)
            span = LODdata[LOD].glueOnLeft ? outterSpan + 1 : outterSpan;

            crossOver = Math.Max(span.Rsector.startDraw, LODdata[LOD].glueOnBottom);
            for (r = span.Rsector.stopDraw; r >= crossOver; r -= 2) DrawRightConnector(r, span.ring, LOD);

            crossOver = Math.Min(span.Rsector.stopDraw, -2 + LODdata[LOD].glueOnBottom);
            for (r = span.Rsector.startDraw; r <= crossOver; r += 2) DrawRightConnector(r, span.ring, LOD);


            // BOTTOM_SPAN -- Horizontal (Sector 3 and 4)
            span = LODdata[LOD].glueOnBottom ? outterSpan : outterSpan + 1;

            crossOver = Math.Max(span.Bsector.startDraw, LODdata[LOD].glueOnLeft);
            for (c = span.Bsector.stopDraw; c >= crossOver; c -= 2) DrawDownConnector(-span.ring + 1, c, LOD);

            crossOver = Math.Min(span.Bsector.stopDraw, -2 + LODdata[LOD].glueOnLeft);
            for (c = span.Bsector.startDraw; c <= crossOver; c += 2) DrawDownConnector(-span.ring + 1, c, LOD);


            // LEFT_SPAN -- Vertical (Sector 5 and 6)
            span = LODdata[LOD].glueOnLeft ? outterSpan : outterSpan + 1;

            crossOver = Math.Max(span.Lsector.startDraw, LODdata[LOD].glueOnBottom);
            for (r = span.Lsector.stopDraw; r >= crossOver; r -= 2) DrawLeftConnector(r, -span.ring + 1, LOD);

            crossOver = Math.Min(span.Lsector.stopDraw, -2 + LODdata[LOD].glueOnBottom);
            for (r = span.Lsector.startDraw; r <= crossOver; r += 2) DrawLeftConnector(r, -span.ring + 1, LOD);
#endif
            throw new NotImplementedException();
        }

        protected void DrawGapFiller(SpanListEntry span)
        {
            int LOD = span.LOD;
            int r, c;
            int crossOver;


            if (LODdata[span.LOD].glueOnBottom)
            {
                // BOTTOM_SPAN -- Horizontal (Sector 3 and 4)
                r = -span.ring;

                crossOver = Math.Max(span.Bsector.startDraw, 0);
                for (c = span.Bsector.stopDraw; c >= crossOver; c--) DrawTerrainSquare(r, c, LOD);

                crossOver = Math.Min(span.Bsector.stopDraw, -1);
                for (c = span.Bsector.startDraw; c <= crossOver; c++) DrawTerrainSquare(r, c, LOD);
            }
            else
            {
                // TOP_SPAN -- Horizontal (Sector 0 and 7)
                r = span.ring;

                crossOver = Math.Max(span.Tsector.startDraw, 0);
                for (c = span.Tsector.stopDraw; c >= crossOver; c--) DrawTerrainSquare(r, c, LOD);

                crossOver = Math.Min(span.Tsector.stopDraw, -1);
                for (c = span.Tsector.startDraw; c <= crossOver; c++) DrawTerrainSquare(r, c, LOD);
            }


            if (LODdata[span.LOD].glueOnLeft)
            {
                // LEFT_SPAN -- Vertical (Sector 5 and 6)
                c = -span.ring;

                crossOver = Math.Max(span.Lsector.startDraw, 0);
                for (r = span.Lsector.stopDraw; r >= crossOver; r--) DrawTerrainSquare(r, c, LOD);

                crossOver = Math.Min(span.Lsector.stopDraw, -1);
                for (r = span.Lsector.startDraw; r <= crossOver; r++) DrawTerrainSquare(r, c, LOD);
            }
            else
            {
                // RIGHT_SPAN -- Vertical (Sector 1 and 2)
                c = span.ring;

                crossOver = Math.Max(span.Rsector.startDraw, 0);
                for (r = span.Rsector.stopDraw; r >= crossOver; r--) DrawTerrainSquare(r, c, LOD);

                crossOver = Math.Min(span.Rsector.stopDraw, -1);
                for (r = span.Rsector.startDraw; r <= crossOver; r++) DrawTerrainSquare(r, c, LOD);
            }
        }

        protected bool DrawSky()
        {
            // Update the sky color based on our current attitude and position
            AdjustSkyColor();


            if (!skyRoof)
            {
                DrawSkyNoRoof();
                return true;		// Need to draw terrain
            }


            if (viewpoint.Z() < -WeatherMap.SKY_ROOF_HEIGHT)
            {
                DrawSkyAbove();
                return false;		// Don't need to draw terrain
            }
            else
            {
                DrawSkyBelow();
                return true;		// Need to draw terrain
            }
        }
        protected void DrawSkyNoRoof()
        {
            HorizonRecord horizon =new HorizonRecord();

            double angleOfDepression, percentHalfXscale;
            float pixelWidth, pixelDistance;
            float vpZ = -viewpoint.Z();

            float bandAngleUp = (float)Math.Min(Math.PI / 18.0f, (Math.PI / 48.0f) * (WeatherMap.SKY_MAX_HEIGHT / vpZ));

            float top = Pitch() + diagonal_half_angle;
            float bottom = Pitch() - diagonal_half_angle;



#if  TWO_D_MAP_AVAILABLE
	if (twoDmode) {
		ClearFrame();
		return;
	}
#endif


            // Figure out how for away from the center of the display the edge of the
            // terrain data is.
            angleOfDepression = Math.Atan2(vpZ, viewpoint.GetDrawingRange());

            pixelWidth = (float)Math.Sqrt(scaleX * scaleX + scaleY * scaleY);


            // Decide which portions of the sky can possibly be seen by the viewer in this orientation
            bool canSeeAboveTop = top > bandAngleUp;
            bool canSeeAboveHorizon = top > 0.0f;
            bool canSeeAboveTerrain = top > -angleOfDepression;
            bool canSeeBelowClear = bottom < bandAngleUp;
            bool canSeeBelowHorizon = bottom < 0.0f;

            bool drawFiller = canSeeAboveTerrain && canSeeBelowHorizon;
            bool drawTop = canSeeAboveHorizon && canSeeBelowClear;
            bool drawClear = canSeeAboveTop;


            // Compute two points on the horizon which are sure to be off opposite edges of the screen
            float cR = (float)Math.Cos(Roll());
            float sR = (float)Math.Sin(Roll());
            horizon.hx = pixelWidth * cR;
            horizon.hy = pixelWidth * -sR;

            // Compute the position of the real horizon line
            // NOTE:  tan becomes infinite at pitch = +/- 90 degrees.
            //        We'll ignore the issue for now since it is a rare occurence.
            percentHalfXscale = Math.Tan(Pitch()) * oneOVERtanHFOV;
            pixelDistance = scaleX * (float)percentHalfXscale;
            horizon.vx = pixelDistance * sR;
            horizon.vy = pixelDistance * cR;


            // Compute the position of the top of the horizon/sky blending band
            // (the band extends "bandAngleUp" radians above the horizon)
            percentHalfXscale = Math.Tan(Pitch() - bandAngleUp) * oneOVERtanHFOV;
            pixelDistance = scaleX * (float)percentHalfXscale;
            horizon.vxUp = pixelDistance * sR;
            horizon.vyUp = pixelDistance * cR;
            horizon.bandAngleUp = bandAngleUp;


            // Compute the position of the bottom of the terrain to horizon filler band
            percentHalfXscale = Pitch() + angleOfDepression;
            if (percentHalfXscale < Constants.PI_OVER_2)
            {
                percentHalfXscale = Math.Tan(Pitch() + angleOfDepression) * oneOVERtanHFOV;
            }
            else
            {
                percentHalfXscale = 10.0f;	// any big number should do...
            }
            pixelDistance = scaleX * (float)percentHalfXscale;
            horizon.vxDn = pixelDistance * sR;
            horizon.vyDn = pixelDistance * cR;


            // Clear that part of the screen which will not be covered by sky or terrain
            if (drawClear)
            {
                DrawClearSky(horizon);
            }

            // Do sunrise/sunset horizon calculations
            ComputeHorizonEffect(out horizon);

            // Draw the blended poly from the haze color to the sky color
            if (drawTop)
            {
                DrawSkyHazeBand(horizon);
            }

            // Draw the celestial objects
            DrawStars();
            if (CTimeOfDay.TheTimeOfDay.ThereIsASun()) DrawSun();
            if (CTimeOfDay.TheTimeOfDay.ThereIsAMoon()) DrawMoon();

            // Draw the poly of low intensity haze color to fill from the terrain to the horizon
            if (drawFiller)
            {
                DrawFillerToHorizon(horizon);
            }
        }
        protected void DrawSkyAbove()
        {
#if TODO
            double angleOfDepression, percentHalfXscale;
            float pixelWidth, pixelDistance;
            float vpAlt = -viewpoint.Z();

            float bandAngleUp = Math.Min(PI / 18.0f, (PI / 48.0f) * (WeatherMap.SKY_MAX_HEIGHT / vpAlt));

            float top = Pitch() + diagonal_half_angle;
            float bottom = Pitch() - diagonal_half_angle;

            float u, v;

            HorizonRecord horizon;

#if  TWO_D_MAP_AVAILABLE
	if (twoDmode) {
		ClearFrame();
		return;
	}
#endif


            // Figure out how for away from the center of the display the edge of the
            // roof polygon is.
            angleOfDepression = atan2(vpAlt - WeatherMap.SKY_ROOF_HEIGHT, WeatherMap.SKY_ROOF_RANGE);
            pixelWidth = (float)sqrt(scaleX * scaleX + scaleY * scaleY);


            // Decide which portions of the sky can possibly be seen by the viewer in this orientation
            bool canSeeAboveTop = top > bandAngleUp;
            bool canSeeAboveHorizon = top > 0.0f;
            bool canSeeAboveClouds = top > -angleOfDepression;
            bool canSeeBelowClear = bottom < bandAngleUp;
            bool canSeeBelowHorizon = bottom < 0.0f;
            bool canSeeCloudLayer = bottom < -angleOfDepression;

            bool drawFiller = canSeeAboveClouds && canSeeBelowHorizon;
            bool drawTop = canSeeAboveHorizon && canSeeBelowClear;
            bool drawClear = canSeeAboveTop;
            bool drawClouds = canSeeCloudLayer;


            // Compute two points on the horizon which are sure to be off opposite edges of the screen
            float cR = (float)cos(Roll());
            float sR = (float)sin(Roll());
            horizon.hx = pixelWidth * cR;
            horizon.hy = pixelWidth * -sR;

            // Compute the position of the real horizon line
            // NOTE:  tan becomes infinite at pitch = +/- 90 degrees.
            //        We'll ignore the issue for now since it is a rare occurence.
            percentHalfXscale = tan(Pitch()) * oneOVERtanHFOV;
            pixelDistance = scaleX * (float)percentHalfXscale;
            horizon.vx = pixelDistance * sR;
            horizon.vy = pixelDistance * cR;


            // Compute the position of the top of the horizon/sky blending band
            // (the band extends "bandAngleUp" radians above the horizon)
            percentHalfXscale = tan(Pitch() - bandAngleUp) * oneOVERtanHFOV;
            pixelDistance = scaleX * (float)percentHalfXscale;
            horizon.vxUp = pixelDistance * sR;
            horizon.vyUp = pixelDistance * cR;
            horizon.bandAngleUp = bandAngleUp;


            // Compute the position of the bottom of the terrain to horizon filler band
            percentHalfXscale = tan(Pitch() + angleOfDepression) * oneOVERtanHFOV;
            pixelDistance = scaleX * (float)percentHalfXscale;
            horizon.vxDn = pixelDistance * sR;
            horizon.vyDn = pixelDistance * cR;


            if (drawClear)
            {
                DrawClearSky(&horizon);
            }

            // Do sunrise/sunset horizon calculations
            ComputeHorizonEffect(&horizon);

            // Draw the blended poly from the haze color to the sky color
            if (drawTop)
            {
                DrawSkyHazeBand(&horizon);
            }

            // Draw the celestial objects
            DrawStars();
            if (CTimeOfDay.TheTimeOfDay.ThereIsASun()) DrawSun();
            if (CTimeOfDay.TheTimeOfDay.ThereIsAMoon()) DrawMoon();

            // Draw the poly of low intensity haze color to fill from the terrain to the horizon
            if (drawFiller)
            {
                DrawFillerToHorizon(&horizon);
            }


            // Draw the overcast layer below us (covers all terrain)
            if (drawClouds)
            {
                Tpoint worldSpace;
                ThreeDVertex v0, v1, v2, v3;

                worldSpace.z = -WeatherMap.SKY_ROOF_HEIGHT;
                u = (float)fmod(viewpoint.Y() * 0.5f * ROOF_REPEAT_COUNT / WeatherMap.SKY_ROOF_RANGE, 1.0f);
                v = 1.0f - (float)fmod(viewpoint.X() * 0.5f * ROOF_REPEAT_COUNT / WeatherMap.SKY_ROOF_RANGE, 1.0f);

                // South West
                worldSpace.x = viewpoint.X() - SKY_ROOF_RANGE; worldSpace.y = viewpoint.Y() - SKY_ROOF_RANGE;
                TransformPoint(&worldSpace, &v0);
                v0.u = u; v0.v = v + ROOF_REPEAT_COUNT; v0.q = v0.csZ * Q_SCALE;

                // North West
                worldSpace.x = viewpoint.X() + SKY_ROOF_RANGE; worldSpace.y = viewpoint.Y() - SKY_ROOF_RANGE;
                TransformPoint(&worldSpace, &v1);
                v1.u = u; v1.v = v; v1.q = v1.csZ * Q_SCALE;

                // South East
                worldSpace.x = viewpoint.X() - SKY_ROOF_RANGE; worldSpace.y = viewpoint.Y() + SKY_ROOF_RANGE;
                TransformPoint(&worldSpace, &v2);
                v2.u = u + ROOF_REPEAT_COUNT; v2.v = v + ROOF_REPEAT_COUNT; v2.q = v2.csZ * Q_SCALE;

                // North East
                worldSpace.x = viewpoint.X() + SKY_ROOF_RANGE; worldSpace.y = viewpoint.Y() + SKY_ROOF_RANGE;
                TransformPoint(&worldSpace, &v3);
                v3.u = u + ROOF_REPEAT_COUNT; v3.v = v; v3.q = v3.csZ * Q_SCALE;

                v0.r = v1.r = v2.r = v3.r = 0.5f;
                v0.g = v1.g = v2.g = v3.g = 0.6f;
                v0.b = v1.b = v2.b = v3.b = 0.7f;

                // Setup the drawing state for these polygons
                context.RestoreState(STATE_TEXTURE_PERSPECTIVE);
                if (GetFilteringMode())
                {
                    context.SetState(MPR_STA_ENABLES, MPR_SE_FILTERING);
                    context.SetState(MPR_STA_TEX_FILTER, MPR_TX_BILINEAR);
                    context.InvalidateState();
                }

                context.SelectTexture(texRoofTop.TexHandle());

                DrawSquare(&v0, &v1, &v3, &v2, CULL_ALLOW_ALL);
            }
#endif
            throw new NotImplementedException();
        }
        protected void DrawSkyBelow()
        {
#if TODO
            ThreeDVertex v0, v1, v2, v3;
            TwoDVertex[] vertPointers = new TwoDVertex[4] { v0, v1, v2, v3 };

            double angleOfInclination, angleOfDepression;
            double percentHalfXscale;
            float pixelWidth, pixelDistance;
            float vpAlt = -viewpoint.Z();

            float top = Pitch() + diagonal_half_angle;
            float bottom = Pitch() - diagonal_half_angle;

            float u, v;

            HorizonRecord horizon;

#if  TWO_D_MAP_AVAILABLE
	if (twoDmode) {
		ClearFrame();
		return;
	}
#endif


            // Figure out how for away from the center of the display the edge of the
            // terrain data is.
            angleOfInclination = atan2(WeatherMap.SKY_ROOF_HEIGHT - vpAlt, WeatherMap.SKY_ROOF_RANGE);
            angleOfDepression = atan2(vpAlt, viewpoint.GetDrawingRange());

            pixelWidth = (float)sqrt(scaleX * scaleX + scaleY * scaleY);


            // Decide which portions of the sky can possibly be seen by the viewer in this orientation
            bool canSeeAboveTop = top > angleOfInclination;
            bool canSeeAboveHorizon = top > 0.0f;
            bool canSeeAboveTerrain = top > -angleOfDepression;
            bool canSeeBelowClear = bottom < angleOfInclination;
            bool canSeeBelowHorizon = bottom < 0.0f;

            bool drawFiller = canSeeAboveTerrain && canSeeBelowHorizon;
            bool drawTop = canSeeAboveHorizon && canSeeBelowClear;
            bool drawClear = canSeeAboveTop;


            // Compute two points on the horizon which are sure to be off opposite edges of the screen
            float cR = (float)cos(Roll());
            float sR = (float)sin(Roll());
            horizon.hx = pixelWidth * cR;
            horizon.hy = pixelWidth * -sR;

            // Compute the position of the real horizon line
            // NOTE:  tan becomes infinite at pitch = +/- 90 degrees.
            //        We'll ignore the issue for now since it is a rare occurence.
            percentHalfXscale = tan(Pitch()) * oneOVERtanHFOV;
            pixelDistance = scaleX * (float)percentHalfXscale;
            horizon.vx = pixelDistance * sR;
            horizon.vy = pixelDistance * cR;


            // Compute the position of the top of the horizon/sky blending band
            // (the band extends "angleOfInclination" radians above the horizon)
            percentHalfXscale = tan(Pitch() - angleOfInclination) * oneOVERtanHFOV;
            pixelDistance = scaleX * (float)percentHalfXscale;
            horizon.vxUp = pixelDistance * sR;
            horizon.vyUp = pixelDistance * cR;


            // Compute the position of the bottom of the terrain to horizon filler band
            percentHalfXscale = tan(Pitch() + angleOfDepression) * oneOVERtanHFOV;
            pixelDistance = scaleX * (float)percentHalfXscale;
            horizon.vxDn = pixelDistance * sR;
            horizon.vyDn = pixelDistance * cR;


            // Clear that part of the screen which will not be covered by sky or terrain
            if (drawClear)
            {
                Tpoint worldSpace;

                worldSpace.z = -WeatherMap.SKY_ROOF_HEIGHT;
                u = (float)fmod(viewpoint.Y() * 0.5f * ROOF_REPEAT_COUNT / SKY_ROOF_RANGE, 1.0f);
                v = 1.0f - (float)fmod(viewpoint.X() * 0.5f * ROOF_REPEAT_COUNT / SKY_ROOF_RANGE, 1.0f);

                // South West
                worldSpace.x = viewpoint.X() - SKY_ROOF_RANGE; worldSpace.y = viewpoint.Y() - SKY_ROOF_RANGE;
                TransformPoint(&worldSpace, &v0);
                v0.u = u; v0.v = v + ROOF_REPEAT_COUNT; v0.q = v0.csZ * Q_SCALE;

                // North West
                worldSpace.x = viewpoint.X() + SKY_ROOF_RANGE; worldSpace.y = viewpoint.Y() - SKY_ROOF_RANGE;
                TransformPoint(&worldSpace, &v1);
                v1.u = u; v1.v = v; v1.q = v1.csZ * Q_SCALE;

                // North East
                worldSpace.x = viewpoint.X() + SKY_ROOF_RANGE; worldSpace.y = viewpoint.Y() + SKY_ROOF_RANGE;
                TransformPoint(&worldSpace, &v2);
                v2.u = u + ROOF_REPEAT_COUNT; v2.v = v; v2.q = v2.csZ * Q_SCALE;

                // South East
                worldSpace.x = viewpoint.X() - SKY_ROOF_RANGE; worldSpace.y = viewpoint.Y() + SKY_ROOF_RANGE;
                TransformPoint(&worldSpace, &v3);
                v3.u = u + ROOF_REPEAT_COUNT; v3.v = v + ROOF_REPEAT_COUNT; v3.q = v3.csZ * Q_SCALE;

                v0.r = v1.r = v2.r = v3.r = 0.5f;
                v0.g = v1.g = v2.g = v3.g = 0.6f;
                v0.b = v1.b = v2.b = v3.b = 0.7f;

                // Setup the drawing state for these polygons
                context.RestoreState(STATE_TEXTURE_PERSPECTIVE);
                context.SelectTexture(texRoofBottom.TexHandle());
#if NOTHING 
		if (GetFilteringMode()) {
			context.SetState( MPR_STA_ENABLES, MPR_SE_FILTERING );
			context.SetState( MPR_STA_TEX_FILTER, MPR_TX_BILINEAR );
			context.InvalidateState();
		}
#endif

                DrawSquare(&v0, &v1, &v2, &v3, CULL_ALLOW_ALL);
            }


            // Draw the hazey sky band
            if (drawTop)
            {
                v0.x = shiftX + horizon.hx + horizon.vx;		// horizon right
                v0.y = shiftY + horizon.hy + horizon.vy;
                v1.x = shiftX - horizon.hx + horizon.vx;		// horizon left
                v1.y = shiftY - horizon.hy + horizon.vy;
                v2.x = shiftX - horizon.hx + horizon.vxUp;		// upper left
                v2.y = shiftY - horizon.hy + horizon.vyUp;
                v3.x = shiftX + horizon.hx + horizon.vxUp;		// upper right
                v3.y = shiftY + horizon.hy + horizon.vyUp;

                v0.r = v1.r = haze_sky_color.r;
                v0.g = v1.g = haze_sky_color.g;
                v0.b = v1.b = haze_sky_color.b;
                v0.a = v1.a = 1.0f;

                v2.r = v3.r = sky_color.r;
                v2.g = v3.g = sky_color.g;
                v2.b = v3.b = sky_color.b;
                v2.a = v3.a = 1.0f;

                // Set the clip flags on the constructed verts
                SetClipFlags(&v0);
                SetClipFlags(&v1);
                SetClipFlags(&v2);
                SetClipFlags(&v3);

                // Clip and draw the smooth shaded horizon polygon
                context.RestoreState(STATE_GOURAUD);
                if (dithered)
                {
                    context.SetState(MPR_STA_ENABLES, MPR_SE_DITHERING);
                    context.InvalidateState();
                }
                ClipAndDraw2DFan(&vertPointers[0], 4);
            }


            // Draw the celestial objects
            if (CTimeOfDay.TheTimeOfDay.ThereIsASun()) DrawSun();
            if (CTimeOfDay.TheTimeOfDay.ThereIsAMoon()) DrawMoon();


            // Draw the poly of low intensity haze color to fill from the terrain to the horizon
            if (drawFiller)
            {

                v0.x = shiftX + horizon.hx + horizon.vxDn;		// lower right
                v0.y = shiftY + horizon.hy + horizon.vyDn;
                v0.r = haze_ground_color.r;
                v0.g = haze_ground_color.g;
                v0.b = haze_ground_color.b;
                v1.x = shiftX - horizon.hx + horizon.vxDn;		// lower left
                v1.y = shiftY - horizon.hy + horizon.vyDn;
                v1.r = haze_ground_color.r;
                v1.g = haze_ground_color.g;
                v1.b = haze_ground_color.b;

                v2.x = shiftX - horizon.hx + horizon.vx;		// horizon left
                v2.y = shiftY - horizon.hy + horizon.vy;
                v3.x = shiftX + horizon.hx + horizon.vx;		// horizon right
                v3.y = shiftY + horizon.hy + horizon.vy;

                v2.r = earth_end_color.r;
                v2.g = earth_end_color.g;
                v2.b = earth_end_color.b;
                v3.r = earth_end_color.r;
                v3.g = earth_end_color.g;
                v3.b = earth_end_color.b;

                // Set the clip flags on the constructed verts
                SetClipFlags(&v0);
                SetClipFlags(&v1);
                SetClipFlags(&v2);
                SetClipFlags(&v3);

                // Clip and draw the smooth shaded horizon polygon
                context.RestoreState(STATE_GOURAUD);
                if (dithered)
                {
                    context.SetState(MPR_STA_ENABLES, MPR_SE_DITHERING);
                    context.InvalidateState();
                }
                ClipAndDraw2DFan(&vertPointers[0], 4);
            }
#endif
            throw new NotImplementedException();
        }
        protected void DrawGroundAndObjects(ObjectDisplayList objectList)
        {
#if TODO
            SpanListEntry span;


#if  TWO_D_MAP_AVAILABLE
	if (twoDmode) {
		// Set the clip flag for each vertex to indicate it hasn't been xformed
		int usedLODcount	= viewpoint.GetMaxLOD() - viewpoint.GetMinLOD() + 1;
		int LODbufferSize	= (maxSpanExtent) * (maxSpanExtent);
		for (TerrainVertex* v = vertexMemory; v <  vertexMemory + usedLODcount * LODbufferSize; v++) {
			v.clipFlag = 0xFFFF;
		}

		context.SetState( MPR_STA_DISABLES, MPR_SE_SHADING );
		context.SetState( MPR_STA_ENABLES,  MPR_SE_BLENDING );
	}
#endif

#if  CHECK_PROC_TIMES
	ulong procTime = GetTickCount();
	objTime = 0;
#endif
            // Compute the potentially visible region of terrain and divide it into rings
            ComputeBounds();
            BuildRingList();

            // Clip the inside edges of the rings to the computed bounding volume in world space
            ClipHorizontalSectors();
            ClipVerticalSectors();
            BuildCornerSet();
            TrimCornerSet();

            // Transform all the verteces required to draw the terrain squares described in the span list
            BuildVertexSet();
            TransformVertexSet();

#if  TWO_D_MAP_AVAILABLE
	if (twoDmode) {
		ThreeDVertex	v;
		ThreeDVertex	*vert = &v;
		Tpoint			v1, v2;
		int	levelCol;
		int	levelRow;
		int	LOD;

		for ( span = spanList; span<firstEmptySpan; span++ ) {
			LOD = span.LOD;

			levelRow = span.ring;
			if (span.Tsector.maxEndPoint > span.Tsector.minEndPoint) {
				v1.y = v2.y = (yRes>>1) - TWODSCALE*WORLD_TO_FLOAT_GLOBAL_POST( span.Tsector.insideEdge - viewpoint.X() );
				v1.x = (xRes>>1) + TWODSCALE*( WORLD_TO_FLOAT_GLOBAL_POST(span.Tsector.maxEndPoint - viewpoint.Y()) );
				v2.x = (xRes>>1) + TWODSCALE*( WORLD_TO_FLOAT_GLOBAL_POST(span.Tsector.minEndPoint - viewpoint.Y()) );

				SetColor( (0x4040 << ((span.LOD-2)*8)) | 0x80000000 );
				Render2DLine( (UInt16)v1.x, (UInt16)v1.y, (UInt16)v2.x, (UInt16)v2.y );                           
			}
			for (levelCol = span.Tsector.startDraw; levelCol <= span.Tsector.stopDraw; levelCol++) {
				vert.x = (xRes>>1) + TWODSCALE*((float)((levelCol+ LODdata[LOD].centerCol) << LOD) - WORLD_TO_FLOAT_GLOBAL_POST(viewpoint.Y()));
				vert.y = (yRes>>1) - TWODSCALE*((float)((levelRow+ LODdata[LOD].centerRow) << LOD) - WORLD_TO_FLOAT_GLOBAL_POST(viewpoint.X()));

				// Draw a marker at this vertex location
				SetColor( 0xF0008080 );
				Render2DTri((UInt16)(vert.x-1), (UInt16)(vert.y-1),
							(UInt16)(vert.x+1), (UInt16)(vert.y-1),
							(UInt16)(vert.x+1), (UInt16)(vert.y+1));
				Render2DTri((UInt16)(vert.x-1), (UInt16)(vert.y-1),
							(UInt16)(vert.x+1), (UInt16)(vert.y+1),
							(UInt16)(vert.x-1), (UInt16)(vert.y+1));
			}

			levelCol = span.ring;
			if (span.Rsector.maxEndPoint > span.Rsector.minEndPoint) {
				v1.x = v2.x = (xRes>>1) + TWODSCALE*WORLD_TO_FLOAT_GLOBAL_POST( span.Rsector.insideEdge - viewpoint.Y() );
				v1.y = (yRes>>1) - TWODSCALE*( WORLD_TO_FLOAT_GLOBAL_POST(span.Rsector.maxEndPoint - viewpoint.X()) );
				v2.y = (yRes>>1) - TWODSCALE*( WORLD_TO_FLOAT_GLOBAL_POST(span.Rsector.minEndPoint - viewpoint.X()) );

				SetColor( (0x4040 << ((span.LOD-2)*8)) | 0x80000000 );
				Render2DLine( (UInt16)v1.x, (UInt16)v1.y, (UInt16)v2.x, (UInt16)v2.y );                           
			}
			for (levelRow = span.Rsector.startDraw; levelRow <= span.Rsector.stopDraw; levelRow++) {
				vert.x = (xRes>>1) + TWODSCALE*((float)((levelCol+ LODdata[LOD].centerCol) << LOD) - WORLD_TO_FLOAT_GLOBAL_POST(viewpoint.Y()));
				vert.y = (yRes>>1) - TWODSCALE*((float)((levelRow+ LODdata[LOD].centerRow) << LOD) - WORLD_TO_FLOAT_GLOBAL_POST(viewpoint.X()));

				// Draw a marker at this vertex location
				SetColor( 0xF0008080 );
				Render2DTri((UInt16)(vert.x-1), (UInt16)(vert.y-1),
							(UInt16)(vert.x+1), (UInt16)(vert.y-1),
							(UInt16)(vert.x+1), (UInt16)(vert.y+1));
				Render2DTri((UInt16)(vert.x-1), (UInt16)(vert.y-1),
							(UInt16)(vert.x+1), (UInt16)(vert.y+1),
							(UInt16)(vert.x-1), (UInt16)(vert.y+1));
			}

			levelRow = -span.ring;
			if (span.Bsector.maxEndPoint > span.Bsector.minEndPoint) {
				v1.y = v2.y = (yRes>>1) - TWODSCALE*WORLD_TO_FLOAT_GLOBAL_POST( span.Bsector.insideEdge - viewpoint.X() );
				v1.x = (xRes>>1) + TWODSCALE*( WORLD_TO_FLOAT_GLOBAL_POST(span.Bsector.maxEndPoint - viewpoint.Y()) );
				v2.x = (xRes>>1) + TWODSCALE*( WORLD_TO_FLOAT_GLOBAL_POST(span.Bsector.minEndPoint - viewpoint.Y()) );

				SetColor( (0x4040 << ((span.LOD-2)*8)) | 0x80000000 );
				Render2DLine( (UInt16)v1.x, (UInt16)v1.y, (UInt16)v2.x, (UInt16)v2.y );                           
			}
			for (levelCol = span.Bsector.startDraw; levelCol <= span.Bsector.stopDraw; levelCol++) {
				vert.x = (xRes>>1) + TWODSCALE*((float)((levelCol+ LODdata[LOD].centerCol) << LOD) - WORLD_TO_FLOAT_GLOBAL_POST(viewpoint.Y()));
				vert.y = (yRes>>1) - TWODSCALE*((float)((levelRow+ LODdata[LOD].centerRow) << LOD) - WORLD_TO_FLOAT_GLOBAL_POST(viewpoint.X()));

				// Draw a marker at this vertex location
				SetColor( 0xF0008080 );
				Render2DTri((UInt16)(vert.x-1), (UInt16)(vert.y-1),
							(UInt16)(vert.x+1), (UInt16)(vert.y-1),
							(UInt16)(vert.x+1), (UInt16)(vert.y+1));
				Render2DTri((UInt16)(vert.x-1), (UInt16)(vert.y-1),
							(UInt16)(vert.x+1), (UInt16)(vert.y+1),
							(UInt16)(vert.x-1), (UInt16)(vert.y+1));
			}

			levelCol = -span.ring;
			if (span.Lsector.maxEndPoint > span.Lsector.minEndPoint) {
				v1.x = v2.x = (xRes>>1) + TWODSCALE*WORLD_TO_FLOAT_GLOBAL_POST( span.Lsector.insideEdge - viewpoint.Y() );
				v1.y = (yRes>>1) - TWODSCALE*( WORLD_TO_FLOAT_GLOBAL_POST(span.Lsector.maxEndPoint - viewpoint.X()) );
				v2.y = (yRes>>1) - TWODSCALE*( WORLD_TO_FLOAT_GLOBAL_POST(span.Lsector.minEndPoint - viewpoint.X()) );

				SetColor( (0x4040 << ((span.LOD-2)*8)) | 0x80000000 );
				Render2DLine( (UInt16)v1.x, (UInt16)v1.y, (UInt16)v2.x, (UInt16)v2.y );                           
			}
			for (levelRow = span.Lsector.startDraw; levelRow <= span.Lsector.stopDraw; levelRow++) {
				vert.x = (xRes>>1) + TWODSCALE*((float)((levelCol+ LODdata[LOD].centerCol) << LOD) - WORLD_TO_FLOAT_GLOBAL_POST(viewpoint.Y()));
				vert.y = (yRes>>1) - TWODSCALE*((float)((levelRow+ LODdata[LOD].centerRow) << LOD) - WORLD_TO_FLOAT_GLOBAL_POST(viewpoint.X()));

				// Draw a marker at this vertex location
				SetColor( 0xF0008080 );
				Render2DTri((UInt16)(vert.x-1), (UInt16)(vert.y-1),
							(UInt16)(vert.x+1), (UInt16)(vert.y-1),
							(UInt16)(vert.x+1), (UInt16)(vert.y+1));
				Render2DTri((UInt16)(vert.x-1), (UInt16)(vert.y-1),
							(UInt16)(vert.x+1), (UInt16)(vert.y+1),
							(UInt16)(vert.x-1), (UInt16)(vert.y+1));
			}
		}

//		return;
	}
#endif



            // Render all the require polygons from farthest to nearest
            for (span = spanList + 1; span < firstEmptySpan; span++)
            {

                // Call the appropriate routine to draw the ring
                if (span.LOD == (span + 1).LOD)
                {
                    DrawTerrainRing(span);
                }
                else
                {
                    // Skip the last ring at the lower LOD (its used only for culling and transformation)
                    span++;

                    // Use the first span at the new LOD to draw the connector ring
                    DrawConnectorRing(span);

                    span++;

                    // Draw the gap filler
                    DrawGapFiller(span);
                }

#if  CHECK_PROC_TIMES
	ulong procTime2 = GetTickCount();
#endif
                // Draw any object over this ring
                objectList.DrawBeyond(LEVEL_POST_TO_WORLD(span.ring, span.LOD), span.LOD, this);
#if  CHECK_PROC_TIMES
	procTime2 = GetTickCount() - procTime2;
	objTime += procTime2;
#endif
            }
#if  CHECK_PROC_TIMES
	procTime = GetTickCount() - procTime;
	terrTime += procTime;
#endif
            // Draw any remaining objects
            objectList.DrawBeyond(0.0f, -1, this);

            // Turn off all non-default rendering parameters
            context.RestoreState(STATE_SOLID);
#endif
            throw new NotImplementedException();
        }

        protected void DrawCloudsAndObjects(ObjectDisplayList clouds, ObjectDisplayList objects)
        {
            float distance;

            do
            {

                distance = objects.GetNextDrawDistance();
                clouds.DrawBeyond(distance, 0, this);
                objects.DrawBeyond(distance, 0, this);

            } while (distance > -1.0f);
        }

        protected void DrawWeather(Trotation orientation)
        {
#if TODO
            // rain effects
            if (rainFactor > 0)
            {
                int max = 100 * rainFactor;
                DWORD rcol = CTimeOfDay.TheTimeOfDay.GetRainColor();
                if (CTimeOfDay.TheTimeOfDay.GetNVGmode())
                    rcol &= 0xff00ff00; // just green component
                DWORD ocol = Color();
                SetColor(rcol);
                mlTrig mlt;
                mlSinCos(&mlt, roll);

                // JB 010608 Adjust for speed
                float speedfactor = (viewpoint.Speed + 10) / 100;
                float dx = 0.033f * mlt.sin / speedfactor;
                float dy = 0.033f * mlt.cos / speedfactor;
                max = (int)((float)max * speedfactor);

                for (int i = 0; i < max; i++)
                {
                    float sx, sy;
                    sx = PRANDFloat();
                    sy = PRANDFloat();

                    // just vertical lines currently. Need to slant them based on speed....
                    // but thats tricky, cos we have no knowledge of that here
                    // JB 010608 We do now!
                    Render2DLine(viewportXtoPixel(sx), viewportYtoPixel(sy),
                    viewportXtoPixel(sx + dx), viewportYtoPixel(sy + dy));
                }
                SetColor(ocol);
            }

            if (thunderAndLightning)
            {
                // draw some shapes...

                // do something clever - damm out of time!!
                // algorithm written, (in perl) and ready to go,
                // this is the wrong place anyway, should be tied to a cloud and done in 3-d space.
            }

            if (snowFactor > 0)
            {
                // snow effects
                int max = 100 * snowFactor;
                DWORD scol = CTimeOfDay.TheTimeOfDay.GetSnowColor();
                if (CTimeOfDay.TheTimeOfDay.GetNVGmode())
                    scol &= 0xff00ff00; // just green component
                DWORD ocol = Color();
                SetColor(scol);
                const float TRIX = 0.009f, TRI2 = TRIX / 2.0f;

                for (int i = 0; i < max; i++)
                {
                    float sx, sy;
                    sx = PRANDFloat();
                    sy = PRANDFloat();

                    // we just draw small 6-pt stars. Two crossed triangles.
                    Render2DTri(viewportXtoPixel(sx), viewportXtoPixel(sy),
                    viewportXtoPixel(sx + TRIX), viewportXtoPixel(sy),
                    viewportXtoPixel(sx + TRI2), viewportXtoPixel(sy + TRIX));
                    Render2DTri(viewportXtoPixel(sx), viewportXtoPixel(sy + TRI2),
                    viewportXtoPixel(sx + TRIX), viewportXtoPixel(sy + TRI2),
                    viewportXtoPixel(sx + TRI2), viewportXtoPixel(sy - TRI2));
                }
                SetColor(ocol);
            }
#endif
            throw new NotImplementedException();
        }

        // These are overridden by the "green" displays (TV & IR)
        protected virtual void ProcessColor(Tcolor color) { }
        protected virtual void DrawSun()
        {
#if TODO
            Tpoint center;
            float alpha;
            float dist;

            // Debug.Assert( CTimeOfDay.TheTimeOfDay.ThereIsASun() );

            // Get the center point of the body on a unit sphere in world space
            CTimeOfDay.TheTimeOfDay.CalculateSunMoonPos(&center, false);

            // Draw the sun and its glare as one object sun
            alpha = Math.Max(SunGlareValue, MIN_SUN_GLARE);
            Debug.Assert(alpha >= 0.0f);
            Debug.Assert(alpha <= 1.0f);

            dist = SUN_DIST;
            float maxdist = MOST_SUN_GLARE_DIST;
            int sunpitch = CTimeOfDay.TheTimeOfDay.GetSunPitch();
            if (sunpitch < 256)
            {
                sunpitch = 16 - (sunpitch >> 4);
                dist -= sunpitch;
            }

            // Compute the (inverse of the) size of the glare polygon
            dist += (alpha) * (maxdist - dist);

            // Draw the object
            context.RestoreState(STATE_ALPHA_TEXTURE);
            context.SelectTexture(viewpoint.SunTexture.TexHandle());
            DrawCelestialBody(&center, dist, alpha);
            // Draw2DSunGlowEffect( this, &center, dist, alpha );
#endif
            throw new NotImplementedException();
        }
        protected virtual void DrawMoon()
        {
#if TODO
            Tpoint center;

            Debug.Assert(CTimeOfDay.TheTimeOfDay.ThereIsAMoon());

            // Get the center point of the body on a unit sphere in world space
            CTimeOfDay.TheTimeOfDay.CalculateSunMoonPos(&center, true);

            // Draw the object
            context.RestoreState(STATE_ALPHA_TEXTURE_GOURAUD);
            if (CTimeOfDay.TheTimeOfDay.GetNVGmode())
            {
                context.SelectTexture(viewpoint.GreenMoonTexture.TexHandle());
            }
            else
            {
                context.SelectTexture(viewpoint.MoonTexture.TexHandle());
            }


            float dist = MOON_DIST;
#if NOTHING 	// I think this looks a little silly.  Let try without it...
	int moonpitch = CTimeOfDay.TheTimeOfDay.GetMoonPitch();
	if (moonpitch < 512) {
		dist -= 0.25f * (8 - (moonpitch >> 6));
	}
#endif

            float glare = 0.0f;
            if (CTimeOfDay.TheTimeOfDay.ThereIsASun()) glare = SunGlareValue;
            float moonblend = CTimeOfDay.TheTimeOfDay.CalculateMoonBlend(glare);
            if (moonblend < 1.0f)
            {
                float vpAlt = -Z();
                if (vpAlt > WeatherMap.SKY_MAX_HEIGHT) moonblend = 1.0f;
                else if (vpAlt > WeatherMap.SKY_MAX_HEIGHT - 16384.0f)
                {
                    vpAlt = (WeatherMap.SKY_MAX_HEIGHT - vpAlt);
                    moonblend += (float)glGetSine(FloatToInt32(vpAlt) >> 2);
                    if (moonblend > 1.0f) moonblend = 1.0f;
                }
            }
            DrawCelestialBody(&center, dist, moonblend);
#endif
            throw new NotImplementedException();
        }

        protected virtual void DrawStars()
        {
#if TODO
            float starblend = CTimeOfDay.TheTimeOfDay.GetStarIntensity();
            float vpAlt = -viewpoint.Z();


            if (vpAlt > WeatherMap.SKY_ROOF_HEIGHT)
            {
                float althazefactor;

                if (vpAlt > WeatherMap.SKY_MAX_HEIGHT)
                {
                    althazefactor = 0.2f;
                }
                else
                {
                    althazefactor = (WeatherMap.SKY_MAX_HEIGHT - vpAlt) * HAZE_ALTITUDE_FACTOR;
                    if (althazefactor < 0.2f) althazefactor = 0.2f;
                }

                starblend = Math.Min(1.0f, starblend + 1.0f - althazefactor);
            }


            if (starblend > 0.000001f)
            {
                Tcolor star_color;
                Tcolor sky_part;
                DWORD draw_color;
                MPRVtx_t vert;
                float scratch_x;
                float scratch_y;
                float scratch_z;


                // Compute the sky color portion of the star colors
                float blend = 255.0f * (1.0f - starblend);
                sky_part.r = sky_color.r * blend;
                sky_part.g = sky_color.g * blend;
                sky_part.b = sky_color.b * blend;

                context.RestoreState(STATE_SOLID);

                StarData* stardata = CTimeOfDay.TheTimeOfDay.GetStarData();
                StarCoord* coord = stardata.coord;
                int lastcolor = -1;
                int i;
                for (i = 0; i < stardata.totalcoord; i++, coord++)
                {
                    if (coord.flag) continue;
                    if (lastcolor != coord.color)
                    {
                        lastcolor = coord.color;
                        float curcolor = lastcolor * starblend;
                        star_color.r = curcolor + sky_part.r;
                        star_color.g = curcolor + sky_part.g;
                        star_color.b = curcolor + sky_part.b;
                        if (star_color.r > 255.0f) star_color.r = 255.0f;
                        if (star_color.g > 255.0f) star_color.g = 255.0f;
                        if (star_color.b > 255.0f) star_color.b = 255.0f;
                        ProcessColor(&star_color);
                        draw_color = (DWORD)star_color.r |
                                     (DWORD)star_color.g << 8 |
                                     (DWORD)star_color.g << 16;
                        context.SelectForegroundColor(draw_color);
                    }

                    // This part does rotation, translation, and scaling
                    // Note, we're swapping the x and z axes here to get from z up/down to z far/near
                    // then we're swapping the x and y axes to get into conventional screen pixel coordinates
                    scratch_z = T.M11 * coord.x + T.M12 * coord.y + T.M13 * coord.z;
                    scratch_x = T.M21 * coord.x + T.M22 * coord.y + T.M23 * coord.z;
                    scratch_y = T.M31 * coord.x + T.M32 * coord.y + T.M33 * coord.z;

                    // Now determine if the point is out behind us or to the sides
                    if (scratch_z < 0.000001f) continue;
                    if (GetHorizontalClipFlags(scratch_x, scratch_z) != ON_SCREEN) continue;
                    if (GetVerticalClipFlags(scratch_y, scratch_z) != ON_SCREEN) continue;

                    // Finally, do the perspective divide and scale and shift into screen space
                    float OneOverZ = 1.0f / scratch_z;
                    vert.x = viewportXtoPixel(scratch_x * OneOverZ);
                    vert.y = viewportYtoPixel(scratch_y * OneOverZ);

                    // Draw the point (we _REALLY_ should do several (or all) points at once)
                    context.DrawPrimitive(MPR_PRM_POINTS, 0, 1, &vert, sizeof(vert));
                }
            }
#endif
            throw new NotImplementedException();
        }

        protected virtual void ComputeHorizonEffect(out HorizonRecord pHorizon)
        {
#if TODO
            Tpoint sunEffectWorldSpace;
            ThreeDVertex sunEffectScreenSpace;

            // Return now if the sun isn't up now
            if (!CTimeOfDay.TheTimeOfDay.ThereIsASun())
            {
                // No horizon effect
                pHorizon.horeffect = 0;
                return;
            }

            int bandangle = glConvertFromRadian(pHorizon.bandAngleUp);
            int sunpitch = CTimeOfDay.TheTimeOfDay.GetSunPitch();
            int deltapitch = sunpitch - bandangle;

            // Return now if the sun isn't near the horizon
            if (deltapitch >= 784)
            {
                // No horizon effect
                pHorizon.horeffect = 0;
                return;
            }

            // Note that there is going to be a horizon effect
            pHorizon.horeffect = 1;

            // Figure out where the sun is...
            if (deltapitch > 0)
            {
                // Sun is above the horizon band
                pHorizon.hazescale = 0.6f - 0.6f * (float)deltapitch / 784.0f;
            }
            else
            {
                // Sun is comming up through the horizon
                if (sunpitch < 0)
                {
                    // calculate scale factor when sun is below horizon to prevent color popup
                    if (sunpitch < -256) pHorizon.hazescale = 0.0f;
                    else
                    {
                        pHorizon.hazescale = 0.8f - (float)-sunpitch / 256.0f;
                        if (pHorizon.hazescale < 0.0f) pHorizon.hazescale = 0.0f;
                    }
                }
                else
                {
                    // calculate scale factor when sun is inside the band
                    pHorizon.hazescale = 0.6f + 0.4f * (float)-deltapitch / (float)bandangle;
                    if (pHorizon.hazescale > 1.0f) pHorizon.hazescale = 1.0f;
                }
            }

            // calculate point on the horizon line to represent sun position
            CTimeOfDay.TheTimeOfDay.CalculateSunGroundPos(&sunEffectWorldSpace);
            TransformCameraCentricPoint(&sunEffectWorldSpace, &sunEffectScreenSpace);

            if (!(sunEffectScreenSpace.clipFlag & ClippingFlags.CLIP_NEAR))
            {
                pHorizon.sunEffectPos.x = sunEffectScreenSpace.x;
                pHorizon.sunEffectPos.y = sunEffectScreenSpace.y;
                pHorizon.sunEffectPos.z = 1.0f;
                Edge horizonLine;
                horizonLine.SetupWithVector(shiftX + pHorizon.vx, shiftY + pHorizon.vy, pHorizon.hx, pHorizon.hy);
                pHorizon.sunEffectPos.y = horizonLine.Y(pHorizon.sunEffectPos.x);
                pHorizon.horeffect |= 2;
            }

            // calculate scale factor on the left and right side based on the yaw
            pHorizon.lhazescale = pHorizon.rhazescale = 0.0f;
            int anglesize = glConvertFromRadian(diagonal_half_angle);
            int yaw = glConvertFromRadian(Yaw());
            int leftyaw = (yaw - anglesize) & 0x3fff;
            int rightyaw = (yaw + anglesize) & 0x3fff;
            anglesize <<= 1;
            float sizeperangle = pHorizon.hazescale / (float)(anglesize);
            int sunyaw = CTimeOfDay.TheTimeOfDay.GetSunYaw();

            int i, j;
            i = (sunyaw - leftyaw) & 0x3fff;
            j = (leftyaw - sunyaw) & 0x3fff;
            if (i > j) i = j;
            if (i < anglesize)
                pHorizon.lhazescale = pHorizon.hazescale - (i * sizeperangle);
            i = (sunyaw - rightyaw) & 0x3fff;
            j = (rightyaw - sunyaw) & 0x3fff;
            if (i > j) i = j;
            if (i < anglesize)
            {
                pHorizon.rhazescale = pHorizon.hazescale - (i * sizeperangle);
            }

            // Get the effect of the sun on the horizon color
            CTimeOfDay.TheTimeOfDay.GetHazeSunHorizonColor(&pHorizon.sunEffectColor);
            ProcessColor(&pHorizon.sunEffectColor);
#endif
            throw new NotImplementedException();
        }

        protected int DrawCelestialBody(Tpoint cntr, float dist, float alpha = 1.0f)
        {
#if TODO
            ThreeDVertex v0, v1, v2, v3;
            Tpoint eastSide;
            Tpoint corner;
            Tpoint center = cntr;

            // Cross the vector toward the center with North (1,0,0) to get the side vector

            // eastSide.x = center.y*n.z - center.z*n.y;
            // eastSide.y = center.z*n.x - center.x*n.z;
            // eastSide.z = center.x*n.y - center.y*n.x;
            eastSide.x = 0.0f;
            eastSide.y = center.z;
            eastSide.z = -center.y;


            // Now push the center point outward to shrink the object
            center.x *= dist;
            center.y *= dist;
            center.z *= dist;

            // North West corner
            corner.x = center.x + 1.0f;
            corner.y = center.y - eastSide.y;
            corner.z = center.z - eastSide.z;
            TransformCameraCentricPoint(&corner, &v0);
            v0.u = 0.0f; v0.v = 0.0f; v0.q = 1.0f;
            v0.r = 1.0f; v0.g = 1.0f; v0.b = 1.0f; v0.a = alpha;

            // North East corner
            corner.y = center.y + eastSide.y;
            corner.z = center.z + eastSide.z;
            TransformCameraCentricPoint(&corner, &v1);
            v1.u = 1.0f; v1.v = 0.0f; v1.q = 1.0f;
            v1.r = 1.0f; v1.g = 1.0f; v1.b = 1.0f; v1.a = alpha;

            // South East corner
            corner.x = center.x - 1.0f;
            TransformCameraCentricPoint(&corner, &v2);
            v2.u = 1.0f; v2.v = 1.0f; v2.q = 1.0f;
            v2.r = 1.0f; v2.g = 1.0f; v2.b = 1.0f; v2.a = alpha;

            // South West corner
            corner.y = center.y - eastSide.y;
            corner.z = center.z - eastSide.z;
            TransformCameraCentricPoint(&corner, &v3);
            v3.u = 0.0f; v3.v = 1.0f; v3.q = 1.0f;
            v3.r = 1.0f; v3.g = 1.0f; v3.b = 1.0f; v3.a = alpha;

            // Render the polygon
            bool gif = false;
            if (F4Config.g_nGfxFix & 0x04)
                gif = true;

            DrawSquare(&v0, &v1, &v2, &v3, CULL_ALLOW_ALL, gif);

            if (v0.clipFlag & v1.clipFlag & v2.clipFlag & v3.clipFlag) return 0;	// not visible
            return 1;

#endif
            throw new NotImplementedException();
        }

        protected void DrawClearSky(HorizonRecord pHorizon)
        {
#if TODO
            Edge horizonLine;
            bool amOut, wasOut, startedOut;
            MPRVtx_t[] vert = new MPRVtx_t[6];
            ushort num;

            // Setup a line equation for the horizon in pixel space
            horizonLine.SetupWithVector(shiftX + pHorizon.vxUp, shiftY + pHorizon.vyUp, pHorizon.hx, pHorizon.hy);

            // Now clip the screen rectangle against the line to build the corners of a polygon
            num = 0;

            // First check the upper left corner
            wasOut = startedOut = horizonLine.LeftOf(leftPixel, topPixel);
            if (!startedOut)
            {
                vert[num].x = leftPixel;
                vert[num].y = topPixel;
                num++;
            }

            // Now check the upper right corner
            amOut = horizonLine.LeftOf(rightPixel, topPixel);
            if (amOut != wasOut)
            {
                // Compute the intesection of the top edge with the horizon and insert it
                vert[num].x = horizonLine.X(topPixel);
                vert[num].y = topPixel;
                num++;
            }
            if (!amOut)
            {
                vert[num].x = rightPixel;
                vert[num].y = topPixel;
                num++;
            }
            wasOut = amOut;

            // Now check the lower right corner
            amOut = horizonLine.LeftOf(rightPixel, bottomPixel);
            if (amOut != wasOut)
            {
                // Compute the intesection of the right edge with the horizon and insert it
                vert[num].x = rightPixel;
                vert[num].y = horizonLine.Y(rightPixel);
                num++;
            }
            if (!amOut)
            {
                vert[num].x = rightPixel;
                vert[num].y = bottomPixel;
                num++;
            }
            wasOut = amOut;

            // Now check the lower left corner
            amOut = horizonLine.LeftOf(leftPixel, bottomPixel);
            if (amOut != wasOut)
            {
                // Compute the intesection of the bottom edge with the horizon and insert it
                vert[num].x = horizonLine.X(bottomPixel);
                vert[num].y = bottomPixel;
                num++;
            }
            if (!amOut)
            {
                vert[num].x = leftPixel;
                vert[num].y = bottomPixel;
                num++;
            }
            wasOut = amOut;

            // Finally, clip the left edge if it crosses the horizon line
            if (wasOut != startedOut)
            {
                // Compute the intesection of the left edge with the horizon and insert it
                vert[num].x = (float)leftPixel;
                vert[num].y = horizonLine.Y((float)leftPixel);
                num++;
            }

            Debug.Assert(num <= 5);

            // Draw the polygon if it isn't totally clipped
            if (num >= 3)
            {
                // Setup for flat shaded drawing for the sky clearing polygon
                context.RestoreState(STATE_SOLID);

                // Draw the sky filling polygon
                context.SelectForegroundColor(
                            ((FloatToInt32(sky_color.r * 255.9f)) +
                             (FloatToInt32(sky_color.g * 255.9f) << 8) +
                             (FloatToInt32(sky_color.b * 255.9f) << 16)) + 0xff000000);

                context.DrawPrimitive(MPR_PRM_TRIFAN, 0, num, &vert[0], sizeof(MPRVtx_t));
            }
#endif
            throw new NotImplementedException();
        }

        protected void DrawSkyHazeBand(HorizonRecord pHorizon)
        {
#if TODO
            float dr = 0.0F, dg = 0.0F, db = 0.0F;
            int num = 0;
            TwoDVertex v0, v1, v2, v3, v4;
            TwoDVertex[] vertPointers = new TwoDVertex[5] { v0, v1, v2, v3, v4 };


            num = 4;
            if (pHorizon.horeffect)
            {
                dr = pHorizon.sunEffectColor.r - haze_sky_color.r;
                dg = pHorizon.sunEffectColor.g - haze_sky_color.g;
                db = pHorizon.sunEffectColor.b - haze_sky_color.b;
                if (pHorizon.horeffect & 2) num = 5;
            }

            // Build the corners of our horizon polygon
            if (num > 4)
            {

                v0.r = haze_sky_color.r + dr * pHorizon.rhazescale;
                v0.g = haze_sky_color.g + dg * pHorizon.rhazescale;
                v0.b = haze_sky_color.b + db * pHorizon.rhazescale;

                v1.r = haze_sky_color.r + dr * pHorizon.hazescale;
                v1.g = haze_sky_color.g + dg * pHorizon.hazescale;
                v1.b = haze_sky_color.b + db * pHorizon.hazescale;

                v2.r = haze_sky_color.r + dr * pHorizon.lhazescale;
                v2.g = haze_sky_color.g + dg * pHorizon.lhazescale;
                v2.b = haze_sky_color.b + db * pHorizon.lhazescale;

                v0.x = shiftX + pHorizon.hx + pHorizon.vx;			// horizon right
                v0.y = shiftY + pHorizon.hy + pHorizon.vy;
                v1.x = pHorizon.sunEffectPos.x;
                v1.y = pHorizon.sunEffectPos.y;
                v2.x = shiftX - pHorizon.hx + pHorizon.vx;			// horizon left
                v2.y = shiftY - pHorizon.hy + pHorizon.vy;

                v3.x = shiftX - pHorizon.hx + pHorizon.vxUp;			// upper left
                v3.y = shiftY - pHorizon.hy + pHorizon.vyUp;
                v3.r = sky_color.r;
                v3.g = sky_color.g;
                v3.b = sky_color.b;

                v4.x = shiftX + pHorizon.hx + pHorizon.vxUp;			// upper right
                v4.y = shiftY + pHorizon.hy + pHorizon.vyUp;
                v4.r = sky_color.r;
                v4.g = sky_color.g;
                v4.b = sky_color.b;
            }
            else
            {
                v0.x = shiftX + pHorizon.hx + pHorizon.vx;			// horizon right
                v0.y = shiftY + pHorizon.hy + pHorizon.vy;
                v1.x = shiftX - pHorizon.hx + pHorizon.vx;			// horizon left
                v1.y = shiftY - pHorizon.hy + pHorizon.vy;
                v2.x = shiftX - pHorizon.hx + pHorizon.vxUp;			// upper left
                v2.y = shiftY - pHorizon.hy + pHorizon.vyUp;
                v3.x = shiftX + pHorizon.hx + pHorizon.vxUp;			// upper right
                v3.y = shiftY + pHorizon.hy + pHorizon.vyUp;

                if (pHorizon.horeffect)
                {
                    v0.r = haze_sky_color.r + dr * pHorizon.rhazescale;
                    v0.g = haze_sky_color.g + dg * pHorizon.rhazescale;
                    v0.b = haze_sky_color.b + db * pHorizon.rhazescale;

                    v1.r = haze_sky_color.r + dr * pHorizon.lhazescale;
                    v1.g = haze_sky_color.g + dg * pHorizon.lhazescale;
                    v1.b = haze_sky_color.b + db * pHorizon.lhazescale;
                }
                else
                {
                    v0.r = haze_sky_color.r;
                    v0.g = haze_sky_color.g;
                    v0.b = haze_sky_color.b;
                    v1.r = haze_sky_color.r;
                    v1.g = haze_sky_color.g;
                    v1.b = haze_sky_color.b;
                }
                v2.r = sky_color.r;
                v2.g = sky_color.g;
                v2.b = sky_color.b;
                v3.r = sky_color.r;
                v3.g = sky_color.g;
                v3.b = sky_color.b;
            }


            // Set the clip flags on the constructed verts
            SetClipFlags(&v0);
            SetClipFlags(&v1);
            SetClipFlags(&v2);
            SetClipFlags(&v3);
            if (num > 4)
            {
                SetClipFlags(&v4);
            }


            // Clip and draw the smooth shaded horizon polygon
            context.RestoreState(STATE_GOURAUD);
            if (dithered)
            {
                context.SetState(MPR_STA_ENABLES, MPR_SE_DITHERING);
                context.InvalidateState();
            }
            ClipAndDraw2DFan(&vertPointers[0], num);
#endif
            throw new NotImplementedException();
        }

        protected void DrawFillerToHorizon(HorizonRecord pHorizon)
        {
#if TODO
            float dr, dg, db;
            float hazescale, lhazescale, rhazescale;
            int num;
            TwoDVertex v0, v1, v2, v3, v4;
            TwoDVertex[] vertPointers = new TwoDVertex[5] { v0, v1, v2, v3, v4 };

            v0.x = shiftX + pHorizon.hx + pHorizon.vxDn;		// lower right
            v0.y = shiftY + pHorizon.hy + pHorizon.vyDn;
            v0.r = haze_ground_color.r;
            v0.g = haze_ground_color.g;
            v0.b = haze_ground_color.b;
            v1.x = shiftX - pHorizon.hx + pHorizon.vxDn;		// lower left
            v1.y = shiftY - pHorizon.hy + pHorizon.vyDn;
            v1.r = haze_ground_color.r;
            v1.g = haze_ground_color.g;
            v1.b = haze_ground_color.b;

            v2.x = shiftX - pHorizon.hx + pHorizon.vx;		// horizon left
            v2.y = shiftY - pHorizon.hy + pHorizon.vy;

            if (pHorizon.horeffect)
            {

                dr = pHorizon.sunEffectColor.r - earth_end_color.r;
                dg = pHorizon.sunEffectColor.g - earth_end_color.g;
                db = pHorizon.sunEffectColor.b - earth_end_color.b;

                // scale down the scale factor for ground
                hazescale = pHorizon.hazescale * 0.4f;
                lhazescale = pHorizon.rhazescale * 0.5f;
                rhazescale = pHorizon.lhazescale * 0.5f;

                v2.r = earth_end_color.r + dr * lhazescale;
                v2.g = earth_end_color.g + dg * lhazescale;
                v2.b = earth_end_color.b + db * lhazescale;

                if (pHorizon.horeffect & 2)
                {
                    num = 5;

                    v3.x = pHorizon.sunEffectPos.x;
                    v3.y = pHorizon.sunEffectPos.y;
                    v4.x = shiftX + pHorizon.hx + pHorizon.vx;	// horizon right
                    v4.y = shiftY + pHorizon.hy + pHorizon.vy;

                    v3.r = earth_end_color.r + dr * hazescale;
                    v3.g = earth_end_color.g + dg * hazescale;
                    v3.b = earth_end_color.b + db * hazescale;

                    v4.r = earth_end_color.r + dr * rhazescale;
                    v4.g = earth_end_color.g + dg * rhazescale;
                    v4.b = earth_end_color.b + db * rhazescale;
                }
                else
                {
                    num = 4;

                    v3.x = shiftX + pHorizon.hx + pHorizon.vx;	// horizon right
                    v3.y = shiftY + pHorizon.hy + pHorizon.vy;

                    v3.r = earth_end_color.r + dr * rhazescale;
                    v3.g = earth_end_color.g + dg * rhazescale;
                    v3.b = earth_end_color.b + db * rhazescale;
                }
            }
            else
            {
                num = 4;

                v3.x = shiftX + pHorizon.hx + pHorizon.vx;		// horizon right
                v3.y = shiftY + pHorizon.hy + pHorizon.vy;

                v2.r = earth_end_color.r;
                v2.g = earth_end_color.g;
                v2.b = earth_end_color.b;
                v3.r = earth_end_color.r;
                v3.g = earth_end_color.g;
                v3.b = earth_end_color.b;
            }


            // Set the clip flags on the constructed verts
            SetClipFlags(&v0);
            SetClipFlags(&v1);
            SetClipFlags(&v2);
            SetClipFlags(&v3);
            if (num > 4)
            {
                SetClipFlags(&v4);
            }


            // Clip and draw the terrain data to horizon filler
#if  FLAT_FILLER
	context.RestoreState( STATE_SOLID );
	v4.r = v3.r = v2.r = v1.r = v0.r;
	v4.g = v3.g = v2.g = v1.g = v0.g;
	v4.b = v3.b = v2.b = v1.b = v0.b;
#else
            context.RestoreState(STATE_GOURAUD);
            if (dithered)
            {
                context.SetState(MPR_STA_ENABLES, MPR_SE_DITHERING);
                context.InvalidateState();
            }
#endif
            ClipAndDraw2DFan(&vertPointers[0], num);
#endif
            throw new NotImplementedException();
        }

        protected void DrawTerrainSquare(int r, int c, int LOD)
        {
#if TODO
            TerrainVertex v0, v1, v2, v3;
            Tpost post;


            // Get the vertecies required to draw this square
            v0 = vertexBuffer[LOD] + maxSpanExtent * r + c;			// South-West
            v1 = vertexBuffer[LOD] + maxSpanExtent * (r + 1) + c;		// North-West
            v2 = v0 + 1;												// South-East
            v3 = v1 + 1;												// North-East


#if  TWO_D_MAP_AVAILABLE
	if (twoDmode) {

		Debug.Assert( v0.clipFlag == ON_SCREEN );
		Debug.Assert( v1.clipFlag == ON_SCREEN );
		Debug.Assert( v2.clipFlag == ON_SCREEN );
		Debug.Assert( v3.clipFlag == ON_SCREEN );

		// Draw the square represented by this lower left corner
		SetColor( 0x80400000 );
		Render2DTri((UInt16)(v0.x),					(UInt16)(v0.y),
					(UInt16)(v0.x+(TWODSCALE<<LOD)),	(UInt16)(v0.y),
					(UInt16)(v0.x+(TWODSCALE<<LOD)),	(UInt16)(v0.y-(TWODSCALE<<LOD)));
		Render2DTri((UInt16)(v0.x),					(UInt16)(v0.y),
					(UInt16)(v0.x+(TWODSCALE<<LOD)),	(UInt16)(v0.y-(TWODSCALE<<LOD)),
					(UInt16)(v0.x),					(UInt16)(v0.y-(TWODSCALE<<LOD)));

		return;
	}
#endif


            // If all verticies are clipped by the same edge, skip this square
            if (v0.clipFlag & v1.clipFlag & v2.clipFlag & v3.clipFlag)
            {
                return;
            }


            // If required, get the post which will provide the texture for this segment
            // and setup the texture coordinates at the corners of this square
            if (v0.RenderingStateHandle > STATE_GOURAUD
                //&& !F4IsBadReadPtr(v0.post, sizeof(Tpost)) // JB 010318 CTD (too much CPU)
                )
            {

                post = v0.post;

                if (LOD <= TheMap.LastNearTexLOD())
                {
                    TheTerrTextures.Select(&context, post.texID);
                }
                else
                {
                    TheFarTextures.Select(&context, post.texID);
                }

                v0.u = post.u;
                v0.v = post.v;
                v1.u = post.u;
                v1.v = post.v - post.d;
                v2.u = post.u + post.d;
                v2.v = post.v;
                v3.u = post.u + post.d;
                v3.v = post.v - post.d;

                // Debug.Assert(v0.u <= 1.0f && v1.u <= 1.0f && v2.u <= 1.0f);
                // Debug.Assert(v0.v <= 1.0f && v1.v <= 1.0f && v2.v <= 1.0f);
                // Debug.Assert(v0.u >= 0.0 && v1.u >= 0.0 && v2.u >= 0.0);
                // Debug.Assert(v0.v >= 0.0 && v1.v >= 0.0 && v2.v >= 0.0);

#if  SET_FG_COLOR_ON_FLAT 
	} else if ( v0.RenderingStateHandle == STATE_SOLID ) {
		SetColor( (FloatToInt32(v0.r * 255.9f))		|
				  (FloatToInt32(v0.g * 255.9f) << 8)	|
				  (FloatToInt32(v0.b * 255.9f) << 16) |
				  (FloatToInt32(v0.a * 255.9f) << 24)   );
#endif
            }


            // Draw the square
            context.RestoreState(v0.RenderingStateHandle);
            DrawSquare(v0, v1, v3, v2, CULL_ALLOW_CW);
#endif
            throw new NotImplementedException();
        }

        protected void DrawUpConnector(int r, int c, int LOD)
        {
#if TODO
            TerrainVertex v0, v1, v2, v3, v4;
            int lowRow;
            int lowCol;
            int lowKeyOffset;
            int highKeyOffset;
            Tpost* post;


            // Compute the corresponding post locations in the lower detail level
            // (Include adjustment for misalignment between levels)
            lowRow = (r + 1 + LODdata[LOD].glueOnBottom) >> 1;
            lowCol = (c + LODdata[LOD].glueOnLeft) >> 1;

            // Compute the offsets of the key vetecies
            highKeyOffset = maxSpanExtent * r + c;
            lowKeyOffset = maxSpanExtent * lowRow + lowCol;

            // Fetch the required vertecies
            v2 = vertexBuffer[LOD + 1] + lowKeyOffset;
            v3 = vertexBuffer[LOD + 1] + lowKeyOffset + 1;

            v1 = vertexBuffer[LOD] + highKeyOffset;
            v0 = vertexBuffer[LOD] + highKeyOffset + 1;
            v4 = vertexBuffer[LOD] + highKeyOffset + 2;


            // Skip this segment if it is entirely clipped
            if (v0.clipFlag & v1.clipFlag & v2.clipFlag & v3.clipFlag & v4.clipFlag)
            {
                return;
            }


            // If required, get the post which will provide the texture for this segment
            if (v0.RenderingStateHandle > STATE_GOURAUD
                && !F4IsBadReadPtr(viewpoint, sizeof(RViewPoint)))
            { // JB 010408 CTD
                post = viewpoint.GetPost(lowRow - 1 + LODdata[LOD + 1].centerRow,
                                           lowCol + LODdata[LOD + 1].centerCol, LOD + 1);
                Debug.Assert(post);

                // Select the texture
                if (LOD < TheMap.LastNearTexLOD())
                {
                    TheTerrTextures.Select(&context, post.texID);
                }
                else
                {
                    TheFarTextures.Select(&context, post.texID);
                }

                // Set texture coordinates
                v0.v = post.v - (post.d * 0.5f);
                v0.u = post.u + (post.d * 0.5f);
                v1.v = v0.v;
                v1.u = post.u;
                v2.v = post.v - post.d;
                v2.u = post.u;
                v3.v = v2.v;
                v3.u = post.u + post.d;
                v4.v = v0.v;
                v4.u = v3.u;
#if  SET_FG_COLOR_ON_FLAT 
	} else if ( v0.RenderingStateHandle == STATE_SOLID ) {
		SetColor( (FloatToInt32(v0.r * 255.9f))		|
				  (FloatToInt32(v0.g * 255.9f) << 8)	|
				  (FloatToInt32(v0.b * 255.9f) << 16) |
				  (FloatToInt32(v0.a * 255.9f) << 24)   );
#endif
            }


#if  TWO_D_MAP_AVAILABLE
	if (twoDmode) {
		SetColor( 0x80004000 );

		if ( lowCol <= -lowRow+1 ) {
			// Mark as end piece
			SetColor( 0x80000040 );
		} else {
			// Not left end -- so we WILL use the leftmost triangle
			Debug.Assert( v1.clipFlag == ON_SCREEN );
		}

		Debug.Assert( v0.clipFlag == ON_SCREEN );
		Debug.Assert( v2.clipFlag == ON_SCREEN );
		Debug.Assert( v3.clipFlag == ON_SCREEN );

		if ( lowRow <= lowCol+1 ) {
			// Mark as end piece
			SetColor( 0x80000040 );
		} else {
			// Not right end -- so we WILL use the rightmost triangle
			Debug.Assert( v4.clipFlag == ON_SCREEN );
		}

#if !NOTHING
		Render2DTri((UInt16)(v2.x),	(UInt16)(v2.y),
					(UInt16)(v1.x),	(UInt16)(v1.y),
					(UInt16)(v4.x),	(UInt16)(v4.y));
		Render2DTri((UInt16)(v2.x),	(UInt16)(v2.y),
					(UInt16)(v3.x),	(UInt16)(v3.y),
					(UInt16)(v4.x),	(UInt16)(v4.y));
#else
		Render2DTri((UInt16)(v2.x),						(UInt16)(v2.y),
					(UInt16)(v2.x+(TWODSCALE<<1<<LOD)),	(UInt16)(v2.y),
					(UInt16)(v2.x+(TWODSCALE<<1<<LOD)),	(UInt16)(v2.y+(TWODSCALE<<LOD)));
		Render2DTri((UInt16)(v2.x),						(UInt16)(v2.y),
					(UInt16)(v2.x+(TWODSCALE<<1<<LOD)),	(UInt16)(v2.y+(TWODSCALE<<LOD)),
					(UInt16)(v2.x),						(UInt16)(v2.y+(TWODSCALE<<LOD)));
#endif
		return;
	}
#endif

            context.RestoreState(v0.RenderingStateHandle);

            if (lowCol <= -lowRow + 1)
            {
                // Left end -- skip first triangle
                DrawSquare(v0, v2, v3, v4, CULL_ALLOW_CW);
            }
            else if (lowRow <= lowCol + 1)
            {
                // Right end -- skip last triangle
                DrawSquare(v0, v1, v2, v3, CULL_ALLOW_CW);
            }
            else
            {
                // Interior segment -- draw it all
                DrawSquare(v0, v1, v2, v3, CULL_ALLOW_CW);
                DrawTriangle(v0, v3, v4, CULL_ALLOW_CW);
            }
#endif
            throw new NotImplementedException();
        }
        protected void DrawRightConnector(int r, int c, int LOD)
        {
#if TODO
            TerrainVertex v0, v1, v2, v3, v4;
            int lowRow;
            int lowCol;
            int lowKeyOffset;
            int highKeyOffset;
            Tpost post;


            // Compute the corresponding post locations in the lower detail level
            // (Include adjust for misalignment between levels)
            lowRow = (r + LODdata[LOD].glueOnBottom) >> 1;
            lowCol = (c + 1 + LODdata[LOD].glueOnLeft) >> 1;

            // Compute the offsets of the key vetecies
            highKeyOffset = maxSpanExtent * r + c;
            lowKeyOffset = maxSpanExtent * lowRow + lowCol;


            // Fetch the required vertecies
            v3 = vertexBuffer[LOD + 1] + lowKeyOffset;
            v2 = vertexBuffer[LOD + 1] + lowKeyOffset + maxSpanExtent;

            v4 = vertexBuffer[LOD] + highKeyOffset;
            v0 = vertexBuffer[LOD] + highKeyOffset + maxSpanExtent;
            v1 = vertexBuffer[LOD] + highKeyOffset + maxSpanExtent + maxSpanExtent;


            // Skip this segment if it is entirely clipped
            if (v0.clipFlag & v1.clipFlag & v2.clipFlag & v3.clipFlag & v4.clipFlag)
            {
                return;
            }

            // If required, get the post which will provide the texture for this segment
            if (v0.RenderingStateHandle > STATE_GOURAUD)
            {
                post = viewpoint.GetPost(lowRow + LODdata[LOD + 1].centerRow,
                                           lowCol - 1 + LODdata[LOD + 1].centerCol, LOD + 1);
                Debug.Assert(post);

                // Select the texture
                if (LOD < TheMap.LastNearTexLOD())
                {
                    TheTerrTextures.Select(&context, post.texID);
                }
                else
                {
                    TheFarTextures.Select(&context, post.texID);
                }

                // Set texture coordinates
                v0.v = post.v - (post.d * 0.5f);
                v0.u = post.u + (post.d * 0.5f);
                v1.v = post.v - post.d;
                v1.u = v0.u;
                v2.v = v1.v;
                v2.u = post.u + post.d;
                v3.v = post.v;
                v3.u = v2.u;
                v4.v = post.v;
                v4.u = v0.u;
#if  SET_FG_COLOR_ON_FLAT 
	} else if ( v0.RenderingStateHandle == STATE_SOLID ) {
		SetColor( ((FloatToInt32(v0.r * 255.9f) & 0xFF))		|
				  ((FloatToInt32(v0.g * 255.9f) & 0xFF) << 8)	|
				  ((FloatToInt32(v0.b * 255.9f) & 0xFF) << 16) |
				  ((FloatToInt32(v0.a * 255.9f) & 0xFF) << 24)   );
#endif
            }


#if  TWO_D_MAP_AVAILABLE
	if (twoDmode) {
		SetColor( 0x80004000 );

		if ( lowRow >= lowCol-1 ) {
			// Mark as end piece
			SetColor( 0x80000040 );
		} else {
			// Not top corner -- will use upper triangle
			Debug.Assert( v1.clipFlag == ON_SCREEN );
		}
		Debug.Assert( v0.clipFlag == ON_SCREEN );
		Debug.Assert( v2.clipFlag == ON_SCREEN );
		Debug.Assert( v3.clipFlag == ON_SCREEN );

		if ( lowRow <= 1-lowCol ) {
			// Mark as end piece
			SetColor( 0x80000040 );
		} else {
			// Not bottom corner -- will use bottom triangle
			Debug.Assert( v4.clipFlag == ON_SCREEN );
		}

#if !NOTHING
		Render2DTri((UInt16)(v2.x),	(UInt16)(v2.y),
					(UInt16)(v1.x),	(UInt16)(v1.y),
					(UInt16)(v4.x),	(UInt16)(v4.y));
		Render2DTri((UInt16)(v2.x),	(UInt16)(v2.y),
					(UInt16)(v3.x),	(UInt16)(v3.y),
					(UInt16)(v4.x),	(UInt16)(v4.y));
#else
		Render2DTri((UInt16)(v2.x),					(UInt16)(v2.y),
					(UInt16)(v2.x-(TWODSCALE<<LOD)),	(UInt16)(v2.y),
					(UInt16)(v2.x-(TWODSCALE<<LOD)),	(UInt16)(v2.y+(TWODSCALE<<1<<LOD)));
		Render2DTri((UInt16)(v2.x),					(UInt16)(v2.y),
					(UInt16)(v2.x-(TWODSCALE<<LOD)),	(UInt16)(v2.y+(TWODSCALE<<1<<LOD)),
					(UInt16)(v2.x),					(UInt16)(v2.y+(TWODSCALE<<1<<LOD)));
#endif
		return;
	}
#endif

            context.RestoreState(v0.RenderingStateHandle);

            if (lowRow >= lowCol - 1)
            {
                // Top corner -- skip first (top) triangle
                DrawSquare(v0, v2, v3, v4, CULL_ALLOW_CW);
            }
            else if (lowRow <= 1 - lowCol)
            {
                // Bottom corner -- skip last (bottom) triangle
                DrawSquare(v0, v1, v2, v3, CULL_ALLOW_CW);
            }
            else
            {
                // Interior segment -- draw it all
                DrawSquare(v0, v1, v2, v3, CULL_ALLOW_CW);
                DrawTriangle(v0, v3, v4, CULL_ALLOW_CW);
            }
#endif
            throw new NotImplementedException();
        }


        protected void DrawDownConnector(int r, int c, int LOD)
        {
#if TODO
            TerrainVertex v0, v1, v2, v3, v4;
            int lowRow;
            int lowCol;
            int lowKeyOffset;
            int highKeyOffset;
            Tpost post;


            // Compute the corresponding post locations in the lower detail level
            // (Include adjust for misalignment between levels)
            lowRow = (r - 1 + LODdata[LOD].glueOnBottom) >> 1;
            lowCol = (c + LODdata[LOD].glueOnLeft) >> 1;

            // Compute the offsets of the key vetecies
            highKeyOffset = maxSpanExtent * r + c;
            lowKeyOffset = maxSpanExtent * lowRow + lowCol;


            // Fetch the required vertecies
            v3 = vertexBuffer[LOD + 1] + lowKeyOffset;
            v2 = vertexBuffer[LOD + 1] + lowKeyOffset + 1;

            v4 = vertexBuffer[LOD] + highKeyOffset;
            v0 = vertexBuffer[LOD] + highKeyOffset + 1;
            v1 = vertexBuffer[LOD] + highKeyOffset + 2;


            // Skip this segment if it is entirely clipped
            if (v0.clipFlag & v1.clipFlag & v2.clipFlag & v3.clipFlag & v4.clipFlag)
            {
                return;
            }


            // If required, set up the texture for this segment
            if (v3.RenderingStateHandle > STATE_GOURAUD)
            {
                post = v3.post;
                Debug.Assert(post);

                // Select the texture
                if (LOD < TheMap.LastNearTexLOD())
                {
                    TheTerrTextures.Select(&context, post.texID);
                }
                else
                {
                    TheFarTextures.Select(&context, post.texID);
                }

                // Set texture coordinates
                v0.v = post.v - (post.d * 0.5f);
                v0.u = post.u + (post.d * 0.5f);
                v1.v = v0.v;
                v1.u = post.u + post.d;
                v2.v = post.v;
                v2.u = v1.u;
                v3.v = post.v;
                v3.u = post.u;
                v4.v = v1.v;
                v4.u = post.u;
#if  SET_FG_COLOR_ON_FLAT 
	} else if ( v3.RenderingStateHandle == STATE_SOLID ) {
		SetColor( ((FloatToInt32(v0.r * 255.9f) & 0xFF))		|
				  ((FloatToInt32(v0.g * 255.9f) & 0xFF) << 8)	|
				  ((FloatToInt32(v0.b * 255.9f) & 0xFF) << 16) |
				  ((FloatToInt32(v0.a * 255.9f) & 0xFF) << 24)   );
#endif
            }


#if  TWO_D_MAP_AVAILABLE
	if (twoDmode) {
		SetColor( 0x80004000 );

		if ( lowCol <= lowRow ) {
			// Mark as end piece
			SetColor( 0x80000040 );
		} else {
			// Not left end -- so we WILL use the leftmost triangle
			Debug.Assert( v4.clipFlag == ON_SCREEN );
		}

		Debug.Assert( v0.clipFlag == ON_SCREEN );
		Debug.Assert( v2.clipFlag == ON_SCREEN );
		Debug.Assert( v3.clipFlag == ON_SCREEN );

		if ( lowCol >= -lowRow ) {
			// Mark as end piece
			SetColor( 0x80000040 );
		} else {
			// Not right end -- so we WILL use the rightmost triangle
			Debug.Assert( v1.clipFlag == ON_SCREEN );
		}

#if !NOTHING
		Render2DTri((UInt16)(v2.x),	(UInt16)(v2.y),
					(UInt16)(v1.x),	(UInt16)(v1.y),
					(UInt16)(v4.x),	(UInt16)(v4.y));
		Render2DTri((UInt16)(v2.x),	(UInt16)(v2.y),
					(UInt16)(v3.x),	(UInt16)(v3.y),
					(UInt16)(v4.x),	(UInt16)(v4.y));
#else
		Render2DTri((UInt16)(v2.x),						(UInt16)(v2.y),
					(UInt16)(v2.x-(TWODSCALE<<1<<LOD)),	(UInt16)(v2.y),
					(UInt16)(v2.x-(TWODSCALE<<1<<LOD)),	(UInt16)(v2.y-(TWODSCALE<<LOD)));
		Render2DTri((UInt16)(v2.x),						(UInt16)(v2.y),
					(UInt16)(v2.x-(TWODSCALE<<1<<LOD)),	(UInt16)(v2.y-(TWODSCALE<<LOD)),
					(UInt16)(v2.x),						(UInt16)(v2.y-(TWODSCALE<<LOD)));
#endif
		return;
	}
#endif

            context.RestoreState(v3.RenderingStateHandle);

            if (lowCol <= lowRow)
            {
                // Left end -- skip last triangle
                DrawSquare(v0, v1, v2, v3, CULL_ALLOW_CW);
            }
            else if (lowCol >= -lowRow)
            {
                // Right end -- skip first triangle
                DrawSquare(v0, v2, v3, v4, CULL_ALLOW_CW);
            }
            else
            {
                // Interior segment -- draw it all
                DrawSquare(v0, v1, v2, v3, CULL_ALLOW_CW);
                DrawTriangle(v0, v3, v4, CULL_ALLOW_CW);
            }
#endif
            throw new NotImplementedException();
        }
        protected void DrawLeftConnector(int r, int c, int LOD)
        {
#if TODO
            TerrainVertex v0, v1, v2, v3, v4;
            int lowRow;
            int lowCol;
            int lowKeyOffset;
            int highKeyOffset;
            Tpost post;


            // Compute the corresponding post locations in the lower detail level
            // (Include adjust for misalignment between levels)
            lowRow = (r + LODdata[LOD].glueOnBottom) >> 1;
            lowCol = (c - 1 + LODdata[LOD].glueOnLeft) >> 1;

            // Compute the offsets of the key vetecies
            highKeyOffset = maxSpanExtent * r + c;
            lowKeyOffset = maxSpanExtent * lowRow + lowCol;


            // Fetch the required vertecies
            v2 = vertexBuffer[LOD + 1] + lowKeyOffset;
            v3 = vertexBuffer[LOD + 1] + lowKeyOffset + maxSpanExtent;

            v1 = vertexBuffer[LOD] + highKeyOffset;
            v0 = vertexBuffer[LOD] + highKeyOffset + maxSpanExtent;
            v4 = vertexBuffer[LOD] + highKeyOffset + maxSpanExtent + maxSpanExtent;


            // Skip this segment if it is entirely clipped
            if (v0.clipFlag & v1.clipFlag & v2.clipFlag & v3.clipFlag & v4.clipFlag)
            {
                return;
            }


            // If required, set up the texture for this segment
            if (v2.RenderingStateHandle > STATE_GOURAUD)
            {
                post = v2.post;
                Debug.Assert(post);

                // Select the texture
                if (LOD < TheMap.LastNearTexLOD())
                {
                    TheTerrTextures.Select(&context, post.texID);
                }
                else
                {
                    TheFarTextures.Select(&context, post.texID);
                }

                // Set texture coordinates
                v0.v = post.v - (post.d * 0.5f);
                v0.u = post.u + (post.d * 0.5f);
                v1.v = post.v;
                v1.u = v0.u;
                v2.v = post.v;
                v2.u = post.u;
                v3.v = post.v - post.d;
                v3.u = post.u;
                v4.v = v3.v;
                v4.u = v0.u;
#if SET_FG_COLOR_ON_FLAT 
	} else if ( v2.RenderingStateHandle == STATE_SOLID ) {
		SetColor( ((FloatToInt32(v0.r * 255.9f) & 0xFF))		|
				  ((FloatToInt32(v0.g * 255.9f) & 0xFF) << 8)	|
				  ((FloatToInt32(v0.b * 255.9f) & 0xFF) << 16) |
				  ((FloatToInt32(v0.a * 255.9f) & 0xFF) << 24)   );
#endif
            }


#if  TWO_D_MAP_AVAILABLE
	if (twoDmode) {
		SetColor( 0x80004000 );

		if ( lowRow >= -lowCol ) {
			// Mark as end piece
			SetColor( 0x80000040 );
		} else {
			// Not top corner -- so we WILL use the upper triangle
			Debug.Assert( v4.clipFlag == ON_SCREEN );
		}

		Debug.Assert( v0.clipFlag == ON_SCREEN );
		Debug.Assert( v2.clipFlag == ON_SCREEN );
		Debug.Assert( v3.clipFlag == ON_SCREEN );

		if ( lowRow <= lowCol ) {
			// Mark as end piece
			SetColor( 0x80000040 );
		} else {
			// Not bottom corner -- so we WILL use the bottom triangle
			Debug.Assert( v1.clipFlag == ON_SCREEN );
		}

#if !NOTHING
		Render2DTri((UInt16)(v2.x),	(UInt16)(v2.y),
					(UInt16)(v1.x),	(UInt16)(v1.y),
					(UInt16)(v4.x),	(UInt16)(v4.y));
		Render2DTri((UInt16)(v2.x),	(UInt16)(v2.y),
					(UInt16)(v3.x),	(UInt16)(v3.y),
					(UInt16)(v4.x),	(UInt16)(v4.y));
#else
		Render2DTri((UInt16)(v3.x),					(UInt16)(v3.y),
					(UInt16)(v3.x+(TWODSCALE<<LOD)),	(UInt16)(v3.y),
					(UInt16)(v3.x+(TWODSCALE<<LOD)),	(UInt16)(v3.y+(TWODSCALE<<1<<LOD)));
		Render2DTri((UInt16)(v3.x),					(UInt16)(v3.y),
					(UInt16)(v3.x+(TWODSCALE<<LOD)),	(UInt16)(v3.y+(TWODSCALE<<1<<LOD)),
					(UInt16)(v3.x),					(UInt16)(v3.y+(TWODSCALE<<1<<LOD)));
#endif
		return;
	}
#endif

            context.RestoreState(v2.RenderingStateHandle);

            if (lowRow >= -lowCol)
            {
                // Top corner -- skip last (top) triangle
                DrawSquare(v0, v1, v2, v3, CULL_ALLOW_CW);
            }
            else if (lowRow <= lowCol)
            {
                // Bottom corner -- skip first (bottom) triangle
                DrawSquare(v0, v2, v3, v4, CULL_ALLOW_CW);
            }
            else
            {
                // Interior segment -- draw it all
                DrawSquare(v0, v1, v2, v3, CULL_ALLOW_CW);
                DrawTriangle(v0, v3, v4, CULL_ALLOW_CW);
            }
#endif
            throw new NotImplementedException();
        }

        // Handle time of day and lighting stuff
        protected virtual void SetTimeOfDayColor()
        {
 #if TODO
            Tcolor light;

            // Set 3D object lighting environment
            lightAmbient = CTimeOfDay.TheTimeOfDay.GetAmbientValue();
            lightDiffuse = CTimeOfDay.TheTimeOfDay.GetDiffuseValue();
            CTimeOfDay.TheTimeOfDay.GetLightDirection(ref lightVector);

            // Store terrain lighting environment (not used at present)
            lightTheta = (float)Math.Atan2(lightVector.y, lightVector.x);
            lightPhi = (float)Math.Atan2(-lightVector.z, Math.Sqrt(lightVector.x * lightVector.x + lightVector.y * lightVector.y));
            Debug.Assert(lightPhi <= PI * 0.5f);

            // Get the new colors for this time of day
            CTimeOfDay.TheTimeOfDay.GetSkyColor(ref sky_color);
            CTimeOfDay.TheTimeOfDay.GetHazeSkyColor(ref haze_sky_color);
            CTimeOfDay.TheTimeOfDay.GetHazeGroundColor(ref earth_end_color);
            CTimeOfDay.TheTimeOfDay.GetGroundColor(ref haze_ground_color);
            ProcessColor(sky_color);
            ProcessColor(haze_sky_color);
            ProcessColor(earth_end_color);
            ProcessColor(haze_ground_color);

            // Set the fog color for the terrain
            DWORD ground_haze = ((int)(haze_ground_color.r * 255.9f)) +
                                ((int)(haze_ground_color.g * 255.9f) << 8) +
                                ((int)(haze_ground_color.b * 255.9f) << 16) + 0xff000000;
            context.SetState(MPR_STA_FOG_COLOR, ground_haze);

            // TODO:  Set the fog color for the objects
            //	TheStateStack.SetDepthCueColor( haze_ground_color.r, haze_ground_color.g, haze_ground_color.b );

            // Adjust the color of the roof textures if they're loaded
            if (texRoofTop.TexHandle())
            {
                CTimeOfDay.TheTimeOfDay.GetTextureLightingColor(&light);
                texRoofTop.palette.LightTexturePalette(&light);
                texRoofBottom.palette.LightTexturePalette(&light);
            }
       #endif
            throw new NotImplementedException();
        }
        protected virtual void AdjustSkyColor()
        {
#if TODO
            // Start with the default sky color for this time of day
            CTimeOfDay.TheTimeOfDay.GetSkyColor(&sky_color);
            ProcessColor(&sky_color);


            // darken color at high altitude
            float vpAlt = -viewpoint.Z();
            if (vpAlt > WeatherMap.SKY_ROOF_HEIGHT)
            {
                float althazefactor, althazefactorblue;
                if (vpAlt > WeatherMap.SKY_MAX_HEIGHT)
                {
                    althazefactor = 0.2f;
                    althazefactorblue = 0.4f;
                }
                else
                {
                    althazefactor = (SKY_MAX_HEIGHT - vpAlt) * HAZE_ALTITUDE_FACTOR;
                    althazefactorblue = 0.4f + (althazefactor * 0.6f);
                    if (althazefactor < 0.2f) althazefactor = 0.2f;
                }
                sky_color.r *= althazefactor;
                sky_color.g *= althazefactor;
                sky_color.b *= althazefactorblue;
            }


            // calculate sun glare effect
            if (CTimeOfDay.TheTimeOfDay.ThereIsASun())
            {

                int pitch = glConvertFromRadian(Pitch());
                int yaw = glConvertFromRadian(Yaw());
                SunGlareValue = CTimeOfDay.TheTimeOfDay.GetSunGlare(yaw, pitch);

                if (SunGlareValue)
                {
                    float vpAlt_ = -viewpoint.Z();
                    if (vpAlt < WeatherMap.SKY_MAX_HEIGHT)
                    {
                        vpAlt = (WeatherMap.SKY_MAX_HEIGHT - vpAlt) * GLARE_FACTOR;
                        float intensity = (float)glGetSine(FloatToInt32(vpAlt));
                        intensity *= SunGlareValue;
                        intensity *= 0.25f;	// scale it down
                        if (intensity > 0.05f)
                        {
                            sky_color.r += intensity;
                            sky_color.g += intensity;
                            sky_color.b += intensity;
                            if (sky_color.r > 1.0f) sky_color.r = 1.0f;
                            if (sky_color.g > 1.0f) sky_color.g = 1.0f;
                            if (sky_color.b > 1.0f) sky_color.b = 1.0f;
                        }
                    }
                }
            }
            CTimeOfDay.TheTimeOfDay.SetCurrentSkyColor(sky_color);
#endif
            throw new NotImplementedException();
        }
        protected static void TimeUpdateCallback(object self)
        {
            ((RenderOTW)self).SetTimeOfDayColor();
        }


        public static void SetupTexturesOnDevice(DXContext rc) { }
        public static void ReleaseTexturesOnDevice(DXContext rc)
        {
 #if TODO
            // Free our texture resources
            if (texRoofTop.TexHandle())
            {
                texRoofTop.FreeAll();
                texRoofBottom.FreeAll();
            }
#endif
            throw new NotImplementedException();
        }

        /***************************************************************************\
    These are used to construct the tunnel vision effect
\***************************************************************************/
        const float big = 1.2f;
        const float ltl = 0.5f;
        struct pnt { public float x, y; }
        static readonly pnt[] OutsidePoints = new pnt[]{
	                        new pnt{	 x=ltl,	y= big	},
	                        new pnt{	 x=1.0f,	 y=1.0f	},
	                        new pnt{	 x=big,	y= ltl	},
	                        new pnt{	 x=big,	y=-ltl	},
	                        new pnt{	 x=1.0f,	y=-1.0f	},
	                        new pnt{	 x=ltl,	y=-big	},
	                        new pnt{	x=-ltl,	y=-big	},
	                        new pnt{	x=-1.0f,	y=-1.0f	},
	                        new pnt{	x=-big,	y=-ltl	},
	                        new pnt{	x=-big,	 y=ltl	},
	                        new pnt{	x=-1.0f,	y= 1.0f	},
	                        new pnt{	x=-ltl,	 y=big	},
	                        new pnt{	x= ltl,	y= big	}
                        };
        private static readonly int NumPoints = OutsidePoints.Length;
        private const float PercentBlend = 0.1f;
        private const float PercentScale = 1.0f + PercentBlend;

        private  const int MAX_POSITIVE_I = 25000;
        private const int MAX_NEGATIVE_I = -25000;
        private const float MAX_POSTIVE_F = MAX_POSITIVE_I * 819.995f;	// FeetPerPost
        private const float MAX_NEGATIVE_F = MAX_NEGATIVE_I * 819.995f;	// FeetPerPost
    }

}

