using System;
using System.Diagnostics;
using Pintensity = System.Single;
using Pmatrix = FalconNet.Graphics.Trotation;

namespace FalconNet.Graphics
{

    public struct StateStackFrame
    {
		public Tpoint[]						XformedPosPool;
        public Pintensity[] IntensityPool;
        public PclipInfo[] ClipInfoPool;
		public Pmatrix						Rotation;
		public Tpoint						Xlation;
		public Tpoint						ObjSpaceEye;
		public Tpoint						ObjSpaceLight;
        public int[] CurrentTextureTable;
		public ObjectInstance		CurrentInstance;
		public ObjectLOD		CurrentLOD;
        public PolyLib.DrawPrimFp DrawPrimJumpTable;
        public StateStackClass.TransformFp Transform;

    }

    public class StateStackClass
    {
        // The one and only state stack.  This would need to be replaced
        // by pointers to instances of StateStackClass passed to each call
        // if more than one stack were to be simultaniously maintained.
        public static StateStackClass TheStateStack;

        public delegate void TransformFp(Tpoint[] p, int n, int offset =0);

        public const int MAX_STATE_STACK_DEPTH = 8;	// Arbitrary
        public const int MAX_SLOT_AND_DYNAMIC_PER_OBJECT = 64;	// Arbitrary
        public const int MAX_TEXTURES_PER_OBJECT = 128;	// Arbitrary
        public const int MAX_CLIP_PLANES = 6;	// 5 view volume, plus 1 extra
        public const int MAX_VERTS_PER_POLYGON = 32;	// Arbitrary
        public const int MAX_VERT_POOL_SIZE = 4096;	// Arbitrary
        public const int MAX_VERTS_PER_CLIPPED_POLYGON = MAX_VERTS_PER_POLYGON + MAX_CLIP_PLANES;
        public const int MAX_CLIP_VERTS = 2 * MAX_CLIP_PLANES;
        public const int MAX_VERTS_PER_OBJECT_TREE = MAX_VERT_POOL_SIZE - MAX_VERTS_PER_POLYGON - MAX_CLIP_VERTS;

        public const float NEAR_CLIP_DISTANCE = 1.0f;

        public StateStackClass()
        {
#if TODO
            stackDepth = 0;
            XformedPosPoolNext = XformedPosPoolBuffer;
            IntensityPoolNext = IntensityPoolBuffer;
            ClipInfoPoolNext = ClipInfoPoolBuffer;
            LODBiasInv = 1.0f;
            SetTextureState(true);
            SetFog(0.0f, null);
#endif
            throw new NotImplementedException();
        }

        //public ~StateStackClass()	{ Debug.Assert(stackDepth == 0); };

        // Called by application
        public static void SetContext(ContextMPR cntxt)
        {

            context = cntxt;
       }

        public static void SetLight(float a, float d, Tpoint atLight)
        {
            LightAmbient = a;
            LightDiffuse = d;

            // Prescale the light vector with the diffuse multiplier
            LightVector.x = atLight.x * d;
            LightVector.y = atLight.y * d;
            LightVector.z = atLight.z * d;

            // Intialize the light postion in world space
            ObjSpaceLight = LightVector;
        }


        public static void SetCameraProperties(float ooTanHHAngle, float ooTanVHAngle, float sclx, float scly, float shftx, float shfty)
        {
            float rx2;

            rx2 = (ooTanHHAngle * ooTanHHAngle);
            hAspectDepthCorrection = 1.0f / (float)Math.Sqrt(rx2 + 1.0f);
            hAspectWidthCorrection = rx2 * hAspectDepthCorrection;

            rx2 = (ooTanVHAngle * ooTanVHAngle);
            vAspectDepthCorrection = 1.0f / (float)Math.Sqrt(rx2 + 1.0f);
            vAspectWidthCorrection = rx2 * vAspectDepthCorrection;

            scaleX = sclx;
            scaleY = scly;
            shiftX = shftx;
            shiftY = shfty;
        }


        public static void SetLODBias(float bias)
        {
            LODBiasInv = 1.0f / bias;
        }

        public static void SetTextureState(bool state)
        {
#if TODO
			if(!PlayerOptions.bFilteredObjects)
			{
				// OW original code
		
				if(state)
				{
					RenderStateTableNoFogPC			= RenderStateTableWithPCTex;
					RenderStateTableNoFogNPC		= RenderStateTableWithNPCTex;
					DrawPrimNoFogNoClipJumpTable	= DrawPrimNoClipWithTexJumpTable;
					ClipPrimNoFogJumpTable			= ClipPrimWithTexJumpTable;
		
					RenderStateTableFogPC			= RenderStateTableFogWithPCTex;
					RenderStateTableFogNPC			= RenderStateTableFogWithNPCTex;
					DrawPrimFogNoClipJumpTable		= DrawPrimFogNoClipWithTexJumpTable;
					ClipPrimFogJumpTable			= ClipPrimFogWithTexJumpTable;
				}
		
				else
				{
					RenderStateTableNoFogPC			= RenderStateTableNoTex;
					RenderStateTableNoFogNPC		= RenderStateTableNoTex;
					DrawPrimNoFogNoClipJumpTable	= DrawPrimNoClipNoTexJumpTable;
					ClipPrimNoFogJumpTable			= ClipPrimNoTexJumpTable;
		
					RenderStateTableFogPC			= RenderStateTableNoTex;
					RenderStateTableFogNPC			= RenderStateTableNoTex;
					DrawPrimFogNoClipJumpTable		= DrawPrimFogNoClipNoTexJumpTable;
					ClipPrimFogJumpTable			= ClipPrimFogNoTexJumpTable;
				}
		
				if(fogValue <= 0.0f)
				{
					RenderStateTablePC		= RenderStateTableNoFogPC;
					RenderStateTableNPC		= RenderStateTableNoFogNPC;
					DrawPrimNoClipJumpTable = DrawPrimNoFogNoClipJumpTable;
					ClipPrimJumpTable		= ClipPrimNoFogJumpTable;
				}
		
				else
				{
					RenderStateTablePC		= RenderStateTableFogPC;
					RenderStateTableNPC		= RenderStateTableFogNPC;
					DrawPrimNoClipJumpTable = DrawPrimFogNoClipJumpTable;
					ClipPrimJumpTable		= ClipPrimFogJumpTable;
				}
			}		
			else	// OW
			{
				if(state)
				{
					RenderStateTableNoFogPC			= RenderStateTableWithPCTex_Filter;
					RenderStateTableNoFogNPC		= RenderStateTableWithNPCTex_Filter;
					DrawPrimNoFogNoClipJumpTable	= DrawPrimNoClipWithTexJumpTable;
					ClipPrimNoFogJumpTable			= ClipPrimWithTexJumpTable;
		
					RenderStateTableFogPC			= RenderStateTableFogWithPCTex_Filter;
					RenderStateTableFogNPC			= RenderStateTableFogWithNPCTex_Filter;
					DrawPrimFogNoClipJumpTable		= DrawPrimFogNoClipWithTexJumpTable;
					ClipPrimFogJumpTable			= ClipPrimFogWithTexJumpTable;
				}
		
				else
				{
					RenderStateTableNoFogPC			= RenderStateTableNoTex_Filter;
					RenderStateTableNoFogNPC		= RenderStateTableNoTex_Filter;
					DrawPrimNoFogNoClipJumpTable	= DrawPrimNoClipNoTexJumpTable;
					ClipPrimNoFogJumpTable			= ClipPrimNoTexJumpTable;
		
					RenderStateTableFogPC			= RenderStateTableNoTex_Filter;
					RenderStateTableFogNPC			= RenderStateTableNoTex_Filter;
					DrawPrimFogNoClipJumpTable		= DrawPrimFogNoClipNoTexJumpTable;
					ClipPrimFogJumpTable			= ClipPrimFogNoTexJumpTable;
				}
		
				if(fogValue <= 0.0f)
				{
					RenderStateTablePC		= RenderStateTableNoFogPC;
					RenderStateTableNPC		= RenderStateTableNoFogNPC;
					DrawPrimNoClipJumpTable = DrawPrimNoFogNoClipJumpTable;
					ClipPrimJumpTable		= ClipPrimNoFogJumpTable;
				}
		
				else
				{
					RenderStateTablePC		= RenderStateTableFogPC;
					RenderStateTableNPC		= RenderStateTableFogNPC;
					DrawPrimNoClipJumpTable = DrawPrimFogNoClipJumpTable;
					ClipPrimJumpTable		= ClipPrimFogJumpTable;
				}
			}
#endif
        }

        public static void SetFog(float percent, Pcolor color)
        {
#if TODO
			Debug.Assert( percent <= 1.0f );
		
			fogValue = percent;
		
			if (percent <= 0.0f) {
				// Turn fog off
				RenderStateTablePC		= RenderStateTableNoFogPC;
				RenderStateTableNPC		= RenderStateTableNoFogNPC;
				DrawPrimNoClipJumpTable = DrawPrimNoFogNoClipJumpTable;
				ClipPrimJumpTable		= ClipPrimNoFogJumpTable;
		
			} else {
				// Turn fog on
				RenderStateTablePC		= RenderStateTableFogPC;
				RenderStateTableNPC		= RenderStateTableFogNPC;
				DrawPrimNoClipJumpTable = DrawPrimFogNoClipJumpTable;
				ClipPrimJumpTable		= ClipPrimFogJumpTable;
		
				// We store the color pre-scaled by the percent of fog to save time later.
				Debug.Assert( color );
				fogColor_premul.r = color.r * percent;
				fogColor_premul.g = color.g * percent;
				fogColor_premul.b = color.b * percent;
		
				// We store 1-percent to save time later.
				fogValue_inv	= 1.0f - percent;
		
		// SCR 6/30/98:  We properly should set MPR's notion of the fog color here
		// but it's a little messy to use a context here, and besides,
		// Falcon can count on the RendererOTW to set it (right?)
		// Therefore:  this should be normally off (turned on only for testing)
#if !NOTHING
				if (g_nFogRenderState & 0x02)
				{
					UInt32 c;
					c  = FloatToInt32(color.r * 255.9f);
					c |= FloatToInt32(color.g * 255.9f) << 8;
					c |= FloatToInt32(color.b * 255.9f) << 16;
					context.SetState( MPR_STA_FOG_COLOR, c );
				}
#endif
			}
#endif
        }

        public static void SetCamera(Tpoint pos, Pmatrix rotWaspect, Pmatrix Bill, Pmatrix Tree)
        {
#if TODO
			Debug.Assert(stackDepth == 0);
		
			// Store our rotation from world to camera space (including aspect scale effects)
			Rotation = rotWaspect;	
		
			// Compute the vector from the camera to the origin rotated into camera space
			Xlation.x = -pos.x * Rotation.M11 - pos.y * Rotation.M12 - pos.z * Rotation.M13;
			Xlation.y = -pos.x * Rotation.M21 - pos.y * Rotation.M22 - pos.z * Rotation.M23;
			Xlation.z = -pos.x * Rotation.M31 - pos.y * Rotation.M32 - pos.z * Rotation.M33;
		
			// Intialize the eye postion in world space
			ObjSpaceEye = pos;
		
			// Store pointers out to the billboard and tree matrices in case we need them
			Tb = Bill;
			Tt = Tree;
#endif
            throw new NotImplementedException();
        }

        // This call is for the application to call to draw an instance of an object.
        public static void DrawObject(ObjectInstance objInst, Pmatrix rot, Tpoint pos, float scale = 1.0f)
        {
#if TODO
			pvtDrawObject( OP_NONE, 
							objInst, 
							rot,	pos, 
							1.0f,	1.0f, 	1.0f, 
							scale );
#endif
            throw new NotImplementedException();
        }

        // This call is rather specialized.  It is intended for use in drawing shadows which  
        // are simple objects (no slots, dofs, etc) but require asymetric scaling in x and y 
        // to simulate orientation changes of the object casting the shadow.
        public static void DrawWarpedObject(ObjectInstance objInst, Pmatrix rot, Tpoint pos, float sx, float sy, float sz, float scale = 1.0f)
        {
#if TODO
			pvtDrawObject( OP_WARP, 
							objInst, 
							rot,	pos, 
							sx,		sy, 	sz, 
							scale );
#endif
            throw new NotImplementedException();
        }

        // Called by BRoot nodes at draw time
        // This call is for the BSPlib to call to draw a child instance attached to a slot.
        public static void DrawSubObject(ObjectInstance objInst, Pmatrix rot, Tpoint pos)
        {
#if TODO
			pvtDrawObject( OP_NONE, 
							objInst, 
							rot,	pos, 
							1.0f,	1.0f, 	1.0f, 
							1.0f );
#endif
            throw new NotImplementedException();
        }

        public static void CompoundTransform(Pmatrix rot, Tpoint pos)
        {
            Tpoint tempP = new Tpoint(), tempP2;
			Pmatrix	tempM;
		
			// Compute the rotated translation vector for this object
			Xlation.x += pos.x * Rotation.M11 + pos.y * Rotation.M12 + pos.z * Rotation.M13;
			Xlation.y += pos.x * Rotation.M21 + pos.y * Rotation.M22 + pos.z * Rotation.M23;
			Xlation.z += pos.x * Rotation.M31 + pos.y * Rotation.M32 + pos.z * Rotation.M33;
		
			tempM = Rotation;
			tempP.x = ObjSpaceEye.x - pos.x;
			tempP.y = ObjSpaceEye.y - pos.y;
			tempP.z = ObjSpaceEye.z - pos.z;
			tempP2 = ObjSpaceLight;
		
			// Composit the camera matrix with the object rotation
            Matrix.MatrixMult(tempM, rot, ref Rotation);
		
			// Compute the eye point in object space
            Matrix.MatrixMultTranspose(rot, tempP, ref ObjSpaceEye);
		
			// Compute the light direction in object space.
            Matrix.MatrixMultTranspose(rot, tempP2, ref ObjSpaceLight);
        }

        public static void Light(Pnormal[] pNormals, int nNormals)
        {

			// Make sure we've got enough room in the light value pool
            Debug.Assert(IsValidIntensityIndex(nNormals - 1));

            for(int j = 0; j < nNormals; j++)
            {
				// light intensity = ambient + diffuse*(lightVector dot normal)
				// and we already scaled the lightVector by the diffuse intensity.
                IntensityPool[IntensityPoolNext] = pNormals[j].i * ObjSpaceLight.x + pNormals[j].j * ObjSpaceLight.y + pNormals[j].k * ObjSpaceLight.z;
                if (IntensityPool[IntensityPoolNext] > 0.0f)
                {
                    IntensityPool[IntensityPoolNext] += LightAmbient;
				} else {
                    IntensityPool[IntensityPoolNext] = LightAmbient;
				}
	
				IntensityPoolNext++;
			}
        }

        public static void SetTextureTable(int[] pTexIDs)
        {
            CurrentTextureTable = pTexIDs;
        }

        public static void PushAll()
        {
#if TODO
			Debug.Assert( stackDepth < MAX_STATE_STACK_DEPTH );
		
			stack[stackDepth].XformedPosPool		= XformedPosPool;
			stack[stackDepth].IntensityPool			= IntensityPool;
			stack[stackDepth].ClipInfoPool			= ClipInfoPool;
		
			stack[stackDepth].Rotation				= Rotation;
			stack[stackDepth].Xlation				= Xlation;
			
			stack[stackDepth].ObjSpaceEye			= ObjSpaceEye;
			stack[stackDepth].ObjSpaceLight			= ObjSpaceLight;
			
			stack[stackDepth].CurrentInstance		= CurrentInstance;
			stack[stackDepth].CurrentLOD			= CurrentLOD;
			stack[stackDepth].CurrentTextureTable	= CurrentTextureTable;
		
			stack[stackDepth].DrawPrimJumpTable		= DrawPrimJumpTable;
			stack[stackDepth].Transform				= Transform;
		
			XformedPosPool							= XformedPosPoolNext;
			IntensityPool							= IntensityPoolNext;
			ClipInfoPool							= ClipInfoPoolNext;
		
			stackDepth++;
#endif
            throw new NotImplementedException();
        }


        public static void PopAll()
        {
#if TODO
			stackDepth--;
		
			XformedPosPoolNext	= XformedPosPool;
			IntensityPoolNext	= IntensityPool;
			ClipInfoPoolNext	= ClipInfoPool;
		
			XformedPosPool		= stack[stackDepth].XformedPosPool;
			IntensityPool		= stack[stackDepth].IntensityPool;
			ClipInfoPool		= stack[stackDepth].ClipInfoPool;
		
			Rotation			= stack[stackDepth].Rotation;
			Xlation				= stack[stackDepth].Xlation;
			
			ObjSpaceEye			= stack[stackDepth].ObjSpaceEye;
			ObjSpaceLight		= stack[stackDepth].ObjSpaceLight;
			
			CurrentInstance		= stack[stackDepth].CurrentInstance;
			CurrentLOD			= stack[stackDepth].CurrentLOD;
			CurrentTextureTable	= stack[stackDepth].CurrentTextureTable;
		
			DrawPrimJumpTable	= stack[stackDepth].DrawPrimJumpTable;
			Transform			= stack[stackDepth].Transform;
#endif
            throw new NotImplementedException();
        }


        public static void PushVerts()
        {
#if TODO
			Debug.Assert( stackDepth < MAX_STATE_STACK_DEPTH );
		
			stack[stackDepth].XformedPosPool	= XformedPosPool;
			stack[stackDepth].IntensityPool		= IntensityPool;
			stack[stackDepth].ClipInfoPool		= ClipInfoPool;
		
			XformedPosPool						= XformedPosPoolNext;
			IntensityPool						= IntensityPoolNext;
			ClipInfoPool						= ClipInfoPoolNext;
		
			stackDepth++;
#endif
            throw new NotImplementedException();
        }

        public static void PopVerts()
        {
#if TODO
			stackDepth--;
		
			XformedPosPoolNext	= XformedPosPool;
			IntensityPoolNext	= IntensityPool;
			ClipInfoPoolNext	= ClipInfoPool;
		
			XformedPosPool		= stack[stackDepth].XformedPosPool;
			IntensityPool		= stack[stackDepth].IntensityPool;
			ClipInfoPool		= stack[stackDepth].ClipInfoPool;
#endif
            throw new NotImplementedException();
        }


        // This should be cleaned up and probably have special clip/noclip versions
        public static void TransformBillboardWithClip(Tpoint[] p, int n, BTransformType type)
        {
#if TODO
			float	scratch_x, scratch_y, scratch_z;
			Pmatrix	T;
		
			// Make sure we've got enough room in the transformed position pool
			Debug.Assert( IsValidPosIndex( n-1 ) );
		
			if (type == Tree) {
				T = Tt;
			} else {
				T = Tb;
			}
		
			while( n-- ) {
				scratch_z = T.M11 * p.x + T.M12 * p.y + T.M13 * p.z + Xlation.x;
				scratch_x = T.M21 * p.x + T.M22 * p.y + T.M23 * p.z + Xlation.y;
				scratch_y = T.M31 * p.x + T.M32 * p.y + T.M33 * p.z + Xlation.z;
		
				ClipInfoPoolNext.clipFlag = ON_SCREEN;
		
				if ( scratch_z < NEAR_CLIP_DISTANCE ) {
					ClipInfoPoolNext.clipFlag |= ClippingFlags.CLIP_NEAR;
				}
		
				if ( fabs(scratch_y) > scratch_z ) {
					if ( scratch_y > scratch_z ) {
						ClipInfoPoolNext.clipFlag |= CLIP_BOTTOM;
					} else {
						ClipInfoPoolNext.clipFlag |= CLIP_TOP;
					}
				}
		
				if ( fabs(scratch_x) > scratch_z ) {
					if ( scratch_x > scratch_z ) {
						ClipInfoPoolNext.clipFlag |= CLIP_RIGHT;
					} else {
						ClipInfoPoolNext.clipFlag |= CLIP_LEFT;
					}
				}
		
				ClipInfoPoolNext.csX = scratch_x;
				ClipInfoPoolNext.csY = scratch_y;
				ClipInfoPoolNext.csZ = scratch_z;
		
				ClipInfoPoolNext++;
		
				float OneOverZ = 1.0f/scratch_z;
				p++;
		
				XformedPosPoolNext.z = scratch_z;
				XformedPosPoolNext.x = XtoPixel( scratch_x * OneOverZ );
				XformedPosPoolNext.y = YtoPixel( scratch_y * OneOverZ );
				XformedPosPoolNext++;
			}
#endif
            throw new NotImplementedException();
        }

        // Called by our own transformations and the clipper
        public static float XtoPixel(float x)
        {
            return (x * scaleX) + shiftX;
        }

        public static float YtoPixel(float y)
        {
            return (y * scaleY) + shiftY;
        }

        // For parameter validation during debug
        public static bool IsValidPosIndex(int i)
        {
#if TODO
			return (i+XformedPosPool < XformedPosPoolBuffer+MAX_VERT_POOL_SIZE);
#endif
            throw new NotImplementedException();
        }

        public static bool IsValidIntensityIndex(int i)
        {
#if TODO
			return (i+IntensityPool  < IntensityPoolBuffer+MAX_VERT_POOL_SIZE);
#endif
            throw new NotImplementedException();
        }

        protected static void TransformNoClip(Tpoint[] pCoords, int nCoords, int offset = 0)
        {
            TransformInline(pCoords, nCoords, false, offset);
        }

        protected static void TransformWithClip(Tpoint[] pCoords, int nCoords, int offset = 0)
        {
            TransformInline(pCoords, nCoords, true, offset);
        }

        protected static ClippingFlags CheckBoundingSphereClipping()
        {
            // Decide if we need clipping, or if the object is totally off screen
            // REMEMBER:  Xlation is camera oriented, but still X front, Y right, Z down
            //			  so range from viewer is in the X term.
            // NOTE:  We compute "d", the distance from the viewer at which the bounding
            //		  sphere should intersect the view frustum.  We use .707 since the
            //		  rotation matrix already normalized us to a 45 degree half angle.
            //		  We do have to adjust the radius shift by the FOV correction factors,
            //		  though, since it didn't go through the matix.
            // NOTE2: We could develop the complete set of clip flags here by continuing to 
            //        check other edges instead of returning in the clipped cases.  For now,
            //        we only need to know if it IS clipped or not, so we terminate early.
            // TODO:  We should roll the radius of any attached slot children into the check radius
            //		  to ensure that we don't reject a parent object whose _children_ may be on screen.
            //        (though this should be fairly rare in practice)
            float rd;
            float rh;
            //	UInt32	clipFlag = ClippingFlags.ON_SCREEN;

            rd = CurrentInstance.Radius() * vAspectDepthCorrection;
            rh = CurrentInstance.Radius() * vAspectWidthCorrection;
            if (-(Xlation.z - rh) >= Xlation.x - rd)
            {
                if (-(Xlation.z + rh) > Xlation.x + rd)
                {
                    return ClippingFlags.OFF_SCREEN;			// Trivial reject top
                }
                //		clipFlag = ClippingFlags.CLIP_TOP;
                return ClippingFlags.CLIP_TOP;
            }
            if (Xlation.z + rh >= Xlation.x - rd)
            {
                if (Xlation.z - rh > Xlation.x + rd)
                {
                    return ClippingFlags.OFF_SCREEN;			// Trivial reject bottom
                }
                //		clipFlag |= ClippingFlags.CLIP_BOTTOM;
                return ClippingFlags.CLIP_BOTTOM;
            }

            rd = CurrentInstance.Radius() * hAspectDepthCorrection;
            rh = CurrentInstance.Radius() * hAspectWidthCorrection;
            if (-(Xlation.y - rh) >= Xlation.x - rd)
            {
                if (-(Xlation.x + rh) > Xlation.x + rd)
                {
                    return ClippingFlags.OFF_SCREEN;			// Trivial reject left
                }
                //		clipFlag |= ClippingFlags.CLIP_LEFT;
                return ClippingFlags.CLIP_LEFT;
            }
            if (Xlation.y + rh >= Xlation.x - rd)
            {
                if (Xlation.y - rh > Xlation.x + rd)
                {
                    return ClippingFlags.OFF_SCREEN;			// Trivial reject right
                }
                //		clipFlag |= ClippingFlags.CLIP_RIGHT;
                return ClippingFlags.CLIP_RIGHT;
            }

            rh = CurrentInstance.Radius();
            if (Xlation.x - rh < NEAR_CLIP_DISTANCE)
            {
                if (Xlation.x + rh < NEAR_CLIP_DISTANCE)
                {
                    return ClippingFlags.OFF_SCREEN;			// Trivial reject near
                }
                //		clipFlag |= ClippingFlags.CLIP_NEAR;
                return ClippingFlags.CLIP_NEAR;
            }

            //	return clipFlag;
            return ClippingFlags.ON_SCREEN;
        }

        protected static void TransformInline(Tpoint[] p, int nCoords, bool clip, int offset = 0)
        {
            float scratch_x, scratch_y, scratch_z;

            // Make sure we've got enough room in the transformed position pool
            Debug.Assert(IsValidPosIndex(nCoords - 1));

            for (int n = 0; n < nCoords; n++ )
            {
                scratch_z = Rotation.M11 * p[n].x + Rotation.M12 * p[n].y + Rotation.M13 * p[n].z + Xlation.x;
                scratch_x = Rotation.M21 * p[n].x + Rotation.M22 * p[n].y + Rotation.M23 * p[n].z + Xlation.y;
                scratch_y = Rotation.M31 * p[n].x + Rotation.M32 * p[n].y + Rotation.M33 * p[n].z + Xlation.z;


                if (clip)
                {
                    ClipInfoPool[ClipInfoPoolNext].clipFlag = ClippingFlags.ON_SCREEN;

                    if (scratch_z < NEAR_CLIP_DISTANCE)
                    {
                        ClipInfoPool[ClipInfoPoolNext].clipFlag |= ClippingFlags.CLIP_NEAR;
                    }

                    if (Math.Abs(scratch_y) > scratch_z)
                    {
                        if (scratch_y > scratch_z)
                        {
                            ClipInfoPool[ClipInfoPoolNext].clipFlag |= ClippingFlags.CLIP_BOTTOM;
                        }
                        else
                        {
                            ClipInfoPool[ClipInfoPoolNext].clipFlag |= ClippingFlags.CLIP_TOP;
                        }
                    }

                    if (Math.Abs(scratch_x) > scratch_z)
                    {
                        if (scratch_x > scratch_z)
                        {
                            ClipInfoPool[ClipInfoPoolNext].clipFlag |= ClippingFlags.CLIP_RIGHT;
                        }
                        else
                        {
                            ClipInfoPool[ClipInfoPoolNext].clipFlag |= ClippingFlags.CLIP_LEFT;
                        }
                    }

                    ClipInfoPool[ClipInfoPoolNext].csX = scratch_x;
                    ClipInfoPool[ClipInfoPoolNext].csY = scratch_y;
                    ClipInfoPool[ClipInfoPoolNext].csZ = scratch_z;

                    ClipInfoPoolNext++;
                }


                float OneOverZ = 1.0f / scratch_z;

                XformedPosPool[XformedPosPoolNext].z = scratch_z;
                XformedPosPool[XformedPosPoolNext].x = XtoPixel(scratch_x * OneOverZ);
                XformedPosPool[XformedPosPoolNext].y = YtoPixel(scratch_y * OneOverZ);
                XformedPosPoolNext++;
            }
        }

        // The asymetric scale factors MUST be <= 1.0f.
        // The global scale factor can be any positive value.
        // The effects of the scales are multiplicative.
        private const UInt32 OP_NONE = 0;
        private const UInt32 OP_FOG = 1;
        private const UInt32 OP_WARP = 2;
        static int in_ = 0;
#if TODO
		protected static  void	pvtDrawObject (DWORD operation, ObjectInstance *objInst, Pmatrix *rot, Tpoint *pos, float sx, float sy, float sz, float scale=1.0f)
		{
			UInt32 clipFlag;
			int	LODused;
			float MaxLODRange;
		
			Debug.Assert( objInst );
		
			PushAll();
		
			// Set up our transformations
			CompoundTransform( rot, pos );
		
			// Special code to impose an asymetric warp on the object
			// (Will screw up if any child transforms are encountered)
			if (operation & OP_WARP)
			{
				// Put the stretch into the transformation matrix
				Debug.Assert( (sx > 0.0f) && (sx <= 1.0f) );
				Debug.Assert( (sy > 0.0f) && (sy <= 1.0f) );
				Debug.Assert( (sz > 0.0f) && (sz <= 1.0f) );
				Pmatrix	tempM;
				Pmatrix	stretchM = {	sx,   0.0f, 0.0f,
										0.0f, sy,   0.0f,
										0.0f, 0.0f, sz };
				tempM = Rotation;
				MatrixMult( &tempM, &stretchM, &Rotation );
			}
		
			// Special code to impose a scale on an object
			if (scale != 1.0f)
			{
				float inv = 1.0f / scale;
				Xlation.x		*= inv;
				Xlation.y		*= inv;
				Xlation.z		*= inv;
				ObjSpaceEye.x	*= inv;
				ObjSpaceEye.y	*= inv;
				ObjSpaceEye.z	*= inv;
			}
		
			// Store the adjusted range for LOD determinations
			LODRange = Xlation.x * LODBiasInv;
		
			// Choose the appropriate LOD of the object to be drawn
			CurrentInstance = objInst;
		
			if (objInst.ParentObject)
			{
				if (g_bSlowButSafe && F4IsBadCodePtr((FARPROC) objInst.ParentObject)) // JB 010220 CTD (too much CPU)
					CurrentLOD = 0; // JB 010220 CTD
				else // JB 010220 CTD
				if (objInst.id < 0 || objInst.id >= TheObjectListLength || objInst.TextureSet < 0) // JB 010705 CTD second try
				{
					Debug.Assert(false);
					CurrentLOD = 0;
				}
				else 
					CurrentLOD		= objInst.ParentObject.ChooseLOD(LODRange, &LODused, &MaxLODRange);
		
				if (CurrentLOD)
				{
					// Decide if we need clipping, or if the object is totally off screen
					clipFlag = CheckBoundingSphereClipping();
		
					// Continue only if some part of the bounding volume is on screen
					if (clipFlag != OFF_SCREEN)
					{
						// Set the jump pointers to turn on/off clipping
						if (clipFlag == ON_SCREEN)
						{
							Transform = TransformNoClip;
							DrawPrimJumpTable = DrawPrimNoClipJumpTable;
						}
						else
						{
							Transform = TransformWithClip;
							DrawPrimJumpTable = DrawPrimWithClipJumpTable;
						}
		
						// Choose perspective correction or not
						if ((Xlation.x > CurrentInstance.Radius() * PERSP_CORR_RADIUS_MULTIPLIER) && 
							!(CurrentLOD.flags & ObjectLOD::PERSP_CORR))
						{
							RenderStateTable = RenderStateTableNPC;
						}
						else
						{
							RenderStateTable = RenderStateTablePC;
						}
		
						in_ ++;
		
						if (in_ == 1)
						{
							verts = 0;
						}
		
						// Draw the object
						CurrentLOD.Draw();
		
		//				if (in == 1)
		//				{
		//					if (verts)
		//					{
		//						MonoPrint ("Obj %d:%d %d : %d\n", objInst.id, LODused, (int) MaxLODRange, verts);
		//					}
		//				}
		
						in_ --;
					}
				}
			}
		
			PopAll();
		}
#endif

        // Active transformation function (selects between with or without clipping)
        public static TransformFp Transform;

        // Computed data pools
        public static Tpoint[] XformedPosPool;	// These point into global storage.  They will point
        public static Pintensity[] IntensityPool;		// to the computed tables for each sub-object.
        public static PclipInfo[] ClipInfoPool;
        public static int XformedPosPoolNext;// These point into global storage.  They will point
        public static int IntensityPoolNext;	// to at least MAX_CLIP_VERTS empty slots beyond 
        public static int ClipInfoPoolNext;	// the computed tables in use by the current sub-object.

        // Instance of the object we're drawing and its range normalized for resolution and FOV
        public static ObjectInstance CurrentInstance;
        public static ObjectLOD CurrentLOD;
        public static int[] CurrentTextureTable;
        public static float LODRange;

        // Fog properties
        public static float fogValue;			// fog precent (set by SetFog())
        public static float fogValue_inv;		// 1.0f - fog precent (set by SetFog())
        public static Pcolor fogColor_premul;	// Fog color times fog percent (set by SetFog())

        // Final transform
        public static Pmatrix Rotation;			// These are the final camera transform
        public static Tpoint Xlation;			// including contributions from parent objects

        // Fudge factors for drawing
        public static float LODBiasInv;			// This times real range is LOD evaluation range

        // Object space points of interest
        public static Tpoint ObjSpaceEye;		// Eye point in object space (for BSP evaluation)
        public static Tpoint ObjSpaceLight;		// Light location in object space(for BSP evaluation)

        // Pointers to our clients billboard and tree matrices in case we need them
        public static Pmatrix[] Tb;	// Billboard (always faces viewer)
        public static Pmatrix[] Tt;	// Tree (always stands up straight and faces viewer)

        // Lighting properties for the BSP objects
        public static float LightAmbient = 0.5f;
        public static float LightDiffuse = 0.5f;
        public static Tpoint LightVector = new Tpoint() { x = 0.0f, y = 0.0f, z = -LightDiffuse };


        // The context on which we'll draw
        public static IContext context;
        protected static StateStackFrame[] stack = new StateStackFrame[MAX_STATE_STACK_DEPTH];
        protected static int stackDepth;

        // Required for object culling
        protected static float hAspectWidthCorrection;
        protected static float hAspectDepthCorrection;
        protected static float vAspectWidthCorrection;
        protected static float vAspectDepthCorrection;

        // The parameters required to get from normalized screen space to pixel space
        protected static float scaleX;
        protected static float scaleY;
        protected static float shiftX;
        protected static float shiftY;


        /********************************************\
            Reserved storage space for computed values.
        \********************************************/
        private static Tpoint[] XformedPosPoolBuffer = new Tpoint[MAX_VERT_POOL_SIZE];
        private static Pintensity[] IntensityPoolBuffer = new Pintensity[MAX_VERT_POOL_SIZE];
        private static PclipInfo[] ClipInfoPoolBuffer = new PclipInfo[MAX_VERT_POOL_SIZE];

    }

}

