using System;
using DWORD = System.Int16;

namespace FalconNet.Graphics
{

	public class ThreeDVertex: TwoDVertex
	{
		float	x, y;
		float	r, g, b, a;	
		float	u, v, q; 
		DWORD	clipFlag;

		float	csX, csY, csZ;		// Camera space coordinates
	}
	
	public class Render3D : Render2D
	{

		// Possible values for CullFlag in DrawSquare call
		public const int CULL_ALLOW_ALL = 0;
		public const int CULL_ALLOW_CW = 1;
		public const int CULL_ALLOW_CCW = 2;
		public const float	NEAR_CLIP = 1.0f;
		public const float Q_SCALE = 0.001f;	// Use to keep Q in 16.16 range for MPR
  
		public Render3D ()
		{
		}
		// public virtual ~Render3D()	{};

		// Setup and Cleanup need to have additions here, but still call the parent versions
		public virtual void Setup (ImageBuffer imageBuffer)
			{
	Tpoint		pos;

	horizontal_half_angle = PI/180.0f; // this is used in the 2d setup call
	detailScaler = 1.0f;
	SetFar( 10000.0f );

	// Call our parents Setup code (win is available only after this call)
	Render2D::Setup( imageBuffer );
	SetFOV( 60.0f * PI / 180.0f );
	 
	// Intialize our camera parameters to something reasonable
	 
	pos.x = pos.y = pos.z = 0.0f;
	SetCamera( &pos, &IMatrix );

	// Put a default light source into the 3D object drawing context
	lightAmbient = 0.5f;
	lightDiffuse = 0.5f;
	Tpoint dir = { 0.0f, 0.0f, -1.0f };
	SetLightDirection( &dir );

	objTextureState = true;
}

		public override void Cleanup ()
		{
			Render2D.Cleanup ();
		}

		// Overload this function to get extra work done at start frame
		public override void StartFrame ()
			 {
	base.StartFrame();

	TheStateStack.SetContext( &context );
	TheStateStack.SetCameraProperties( oneOVERtanHFOV, oneOVERtanVFOV, scaleX, scaleY, shiftX, shiftY );
	TheStateStack.SetLight(lightAmbient, lightDiffuse, &lightVector);
	TheStateStack.SetLODBias( resRelativeScaler );
	TheStateStack.SetTextureState( objTextureState );
	TheColorBank.SetColorMode( ColorBankClass::NormalMode );
}
	
		// Set the camera parameters we're to use for rendering
		public void SetObjectDetail (float scaler)
			{
	// This constant is set based on the design resolution for the visual objects
	// art work.  Specificly, 60 degree field of view
	// on a display 640 pixels across.  This is our reference display 
	// precision, or LOD scale 1.0.
	const float	RadiansPerPixel = (60.0f * PI / 180.0f) / 640;

	// Ignor invalid parameters
	if (scaler <= 0.0f) {
		return;
	}

	detailScaler = scaler;
	resRelativeScaler = detailScaler * RadiansPerPixel * scaleX * oneOVERtanHFOV;

	TheStateStack.SetLODBias( resRelativeScaler );
}

		public void SetFOV (float horizontal_fov)
			{
	const float maxHalfFOV =  70 * PI/180.0f;
	// JB 010120 Allow narrower field of views for Mavs and GBUs
	//const float minHalfFOV =   2 * PI/180.0f;
	//const float minHalfFOV = PI/180.0f;
	//MI need to be able to go higher for the TGP
	const float minHalfFOV = (PI/180.0f) / 4;
	// JB 010120

	// Set the field of view
	horizontal_half_angle = horizontal_fov / 2.0f;

	// Keep the field of view values within a reasonable range
	if (horizontal_half_angle > maxHalfFOV) {
		horizontal_half_angle = maxHalfFOV;
	} else if (horizontal_half_angle < minHalfFOV) {
		horizontal_half_angle = minHalfFOV;
	}

	// Recompute the vertical and diangonal field of views to retain consistency
	// NOTE:  (Computation of vertical and diagonal ASSUMES SQUARE PIXELS)
	if (scaleX) {
		vertical_half_angle	= (float)atan2( scaleY * tan(horizontal_half_angle), scaleX );
		diagonal_half_angle	= (float)atan2( sqrt(scaleX*scaleX+scaleY*scaleY) * tan(horizontal_half_angle), scaleX );
	} else {
		vertical_half_angle	= (float)atan2( 3.0f * tan(horizontal_half_angle), 4.0f );
		diagonal_half_angle	= (float)atan2( 5.0f * tan(horizontal_half_angle), 4.0f );
	}
	oneOVERtanHFOV = 1.0f / (float)tan(horizontal_half_angle);
	oneOVERtanVFOV = 1.0f / (float)tan(vertical_half_angle);

	SetObjectDetail( detailScaler );

	// Send relevant stuff to the BSP object library
	TheStateStack.SetCameraProperties( oneOVERtanHFOV, oneOVERtanVFOV, scaleX, scaleY, shiftX, shiftY );
}

		public void SetFar (float distance)
		{
			far_clip = distance;
		}

		public void SetCamera (Tpoint pos, Trotation rot)
			{
	float	sinRoll,  cosRoll;
	float	sinPitch, cosPitch;


	// Store the provided position and orientation
	memcpy( &cameraPos, pos, sizeof( cameraPos ) );
	MatrixTranspose( rot, &cameraRot );

	// Back compute the roll, pitch, and yaw of the viewer
	pitch	= (float)-asin( cameraRot.M13 );
	roll	= (float)atan2( cameraRot.M23, cameraRot.M33 );
	yaw		= (float)atan2( cameraRot.M12, cameraRot.M11 );
	if (yaw < 0.0f) yaw += 2.0f * PI;		// Convert from +/-180 to 0-360 degrees


	// The result of multiplying a point by this matrix will be permuted into
	// screen space by renaming the components of the output vector
	// (after this, plus X will be to the right and plus Y will be down the screen)
	// Output x axis corresponds to row 2
	// Output y axis corresponds to row 3
	// Output z axis corresponds to row 1
	// In this matrix, we also scale output x and y to account for the field of view.
	// Now |X| > Z or |Y| > Z are out
	T.M11 = cameraRot.M11;
	T.M12 = cameraRot.M12;
	T.M13 = cameraRot.M13;

	T.M21 = cameraRot.M21 * oneOVERtanHFOV;
	T.M22 = cameraRot.M22 * oneOVERtanHFOV;
	T.M23 = cameraRot.M23 * oneOVERtanHFOV;

	T.M31 = cameraRot.M31 * oneOVERtanVFOV;
	T.M32 = cameraRot.M32 * oneOVERtanVFOV;
	T.M33 = cameraRot.M33 * oneOVERtanVFOV;


	// Compute the vector from the camera to the origin rotated into camera space
	move.x = - cameraPos.x * T.M11 - cameraPos.y * T.M12 - cameraPos.z * T.M13;
	move.y = - cameraPos.x * T.M21 - cameraPos.y * T.M22 - cameraPos.z * T.M23;
	move.z = - cameraPos.x * T.M31 - cameraPos.y * T.M32 - cameraPos.z * T.M33;



	// Build the billboard matrix (appropriate for Erick's row vector object library)
	sinRoll = (float)sin( roll );
	cosRoll = (float)cos( roll );
	Tbb.M11 = 1.0f;	Tbb.M21 = 0.0f;					  Tbb.M31 =  0.0f;
	Tbb.M12 = 0.0f;	Tbb.M22 = cosRoll*oneOVERtanHFOV; Tbb.M32 = -sinRoll*oneOVERtanVFOV;
	Tbb.M13 = 0.0f;	Tbb.M23 = sinRoll*oneOVERtanHFOV; Tbb.M33 =  cosRoll*oneOVERtanVFOV;

	// Build the tree matrix
	sinPitch = (float)sin( pitch );
	cosPitch = (float)cos( pitch );
	Tt.M11 =  cosPitch;	Tt.M21 = sinPitch*Tbb.M23;	Tt.M31 = sinPitch*Tbb.M33;
	Tt.M12 =  0.0f;		Tt.M22 = Tbb.M22;			Tt.M32 = Tbb.M32;
	Tt.M13 = -sinPitch;	Tt.M23 = cosPitch*Tbb.M23;	Tt.M33 = cosPitch*Tbb.M33;

	// Send relevant stuff to the BSP object library
	TheStateStack.SetCamera( pos, &T, &Tbb, &Tt );
}

		public float GetObjectDetail ()
		{
			return detailScaler;
		}

		public float GetFOV ()
		{
			return horizontal_half_angle * 2.0f;
		}

		public float GetVFOV ()
		{
			return vertical_half_angle * 2.0f;
		}

		public float GetDFOV ()
		{
			return diagonal_half_angle * 2.0f;
		}

		public override void SetViewport (float leftSide, float topSide, float rightSide, float bottomSide)
			{
	// First call the base class's version of this function
	base.SetViewport( l, t, r, b );

	// Redo our FOV dependent setup
	SetFOV( GetFOV() );
}


		// Setup the 3D object lighting
		public void SetLightDirection (Tpoint dir)
			{
	lightVector = dir;
	TheStateStack.SetLight(lightAmbient, lightDiffuse, &lightVector);
}

		public void GetLightDirection (out Tpoint dir)
			{
	dir = lightVector;
}

		// Trun 3D object texturing on/off
		public void SetObjectTextureState (bool state)
			{
	if (state != objTextureState) {
		// Record the choice
		objTextureState = state;

		// Update the object library
		// NOTE:  If any other renderer is inside a StartFrame, this will clobber their settings
		TheStateStack.SetTextureState( objTextureState );
	}
}

		public bool GetObjectTextureState ()
		{
			return objTextureState;
		}

		// Get the location and orientation of the camera for this renderer
		public float	X ()
		{
			return cameraPos.x;
		}

		public float	Y ()
		{
			return cameraPos.y;
		}

		public float	Z ()
		{
			return cameraPos.z;
		}

		public float	Yaw ()
		{
			return yaw;
		}

		public float	Pitch ()
		{
			return pitch;
		}

		public float   Roll ()
		{
			return roll;
		}

		public void	GetAt (Tpoint v)
		{
			v.x = cameraRot.M11;
			v.y = cameraRot.M12;
			v.z = cameraRot.M13;
		}
		
		public void	GetLeft (Tpoint v)
		{
			v.x = cameraRot.M21;
			v.y = cameraRot.M22;
			v.z = cameraRot.M23;
		}

		public void	GetUp (Tpoint v)
		{
			v.x = cameraRot.M31;
			v.y = cameraRot.M32;
			v.z = cameraRot.M33;
		}


		// Transform the given worldspace point into pixel coordinates using the current camera
		public void	TransformPoint (Tpoint p, out ThreeDVertex result)
			{
	 float		scratch_x;
	 float		scratch_y;
	 float		scratch_z;
	 DWORD		clipFlag;


	// This part does rotation, translation, and scaling
	// Note, we're swapping the x and z axes here to get from z up/down to z far/near
	// then we're swapping the x and y axes to get into conventional screen pixel coordinates
	scratch_z = T.M11 * p.x + T.M12 * p.y + T.M13 * p.z + move.x;
	scratch_x = T.M21 * p.x + T.M22 * p.y + T.M23 * p.z + move.y;
	scratch_y = T.M31 * p.x + T.M32 * p.y + T.M33 * p.z + move.z;


	// Now determine if the point is out behind us or to the sides
	clipFlag  = GetRangeClipFlags(      scratch_z, far_clip );
	clipFlag |= GetHorizontalClipFlags( scratch_x, scratch_z );
	clipFlag |= GetVerticalClipFlags(   scratch_y, scratch_z );

	// Finally, do the perspective divide and scale and shift into screen space
	 float OneOverZ = 1.0f/scratch_z;
	result.csX = scratch_x;
	result.csY = scratch_y;
	result.csZ = scratch_z;
	result.x = viewportXtoPixel( scratch_x * OneOverZ );
	result.y = viewportYtoPixel( scratch_y * OneOverZ );
	result.clipFlag = clipFlag;
}

		public void	TransformPointToView (Tpoint p, out Tpoint result)
			{
	// This part does rotation, translation, and scaling
	// no swapping...
	result.x = T.M11 * p.x + T.M12 * p.y + T.M13 * p.z + move.x;
	result.y = T.M21 * p.x + T.M22 * p.y + T.M23 * p.z + move.y;
	result.z = T.M31 * p.x + T.M32 * p.y + T.M33 * p.z + move.z;
}

		public void	TransformBillboardPoint (Tpoint p, Tpoint viewOffset, out ThreeDVertex result)
			{
	 float		scratch_x;
	 float		scratch_y;
	 float		scratch_z;
	 DWORD		clipFlag;

	// This part does rotation, translation, and scaling
	// Note, we're swapping the x and z axes here to get from z up/down to z far/near
	// then we're swapping the x and y axes to get into conventional screen pixel coordinates
	// Note: since this is a billboard we don't have to do the full:
	// 	scratch_z = T.M11 * p.x + T.M12 * p.y + T.M13 * p.z + move.x;
	// 	scratch_x = T.M21 * p.x + T.M22 * p.y + T.M23 * p.z + move.y;
	// 	scratch_y = T.M31 * p.x + T.M32 * p.y + T.M33 * p.z + move.z;
	// since we know where some 1's and 0's are

	scratch_z =  p.x + viewOffset.x;
	scratch_x =  Tbb.M22 * p.y + Tbb.M23 * p.z + viewOffset.y;
	scratch_y =  Tbb.M32 * p.y + Tbb.M33 * p.z + viewOffset.z;


	// Now determine if the point is out behind us or to the sides
	clipFlag  = GetRangeClipFlags(      scratch_z, far_clip );
	clipFlag |= GetHorizontalClipFlags( scratch_x, scratch_z );
	clipFlag |= GetVerticalClipFlags(   scratch_y, scratch_z );

	// Finally, do the perspective divide and scale and shift into screen space
	 float OneOverZ = 1.0f/scratch_z;
	result.csX = scratch_x;
	result.csY = scratch_y;
	result.csZ = scratch_z;
	result.x = viewportXtoPixel( scratch_x * OneOverZ );
	result.y = viewportYtoPixel( scratch_y * OneOverZ );
	result.clipFlag = clipFlag;
}

		public void	TransformTreePoint (Tpoint p, Tpoint viewOffset, ThreeDVertex result)
			{
	 float		scratch_x;
	 float		scratch_y;
	 float		scratch_z;
	 DWORD		clipFlag;


	// This part does rotation, translation, and scaling
	// Note, we're swapping the x and z axes here to get from z up/down to z far/near
	// then we're swapping the x and y axes to get into conventional screen pixel coordinates
	scratch_z = Tt.M11 * p.x + Tt.M12 * p.y + Tt.M13 * p.z + viewOffset.x;
	scratch_x = Tt.M21 * p.x + Tt.M22 * p.y + Tt.M23 * p.z + viewOffset.y;
	scratch_y = Tt.M31 * p.x + Tt.M32 * p.y + Tt.M33 * p.z + viewOffset.z;


	// Now determine if the point is out behind us or to the sides
	clipFlag  = GetRangeClipFlags(      scratch_z, far_clip );
	clipFlag |= GetHorizontalClipFlags( scratch_x, scratch_z );
	clipFlag |= GetVerticalClipFlags(   scratch_y, scratch_z );

	// Finally, do the perspective divide and scale and shift into screen space
	 float OneOverZ = 1.0f/scratch_z;
	result.csX = scratch_x;
	result.csY = scratch_y;
	result.csZ = scratch_z;
	result.x = viewportXtoPixel( scratch_x * OneOverZ );
	result.y = viewportYtoPixel( scratch_y * OneOverZ );
	result.clipFlag = clipFlag;
}

		public void	UnTransformPoint (Tpoint p, out Tpoint result)
			{
	float scratch_x, scratch_y, scratch_z;
	float x, y, z;
	float mag;
	
	// Undo the viewport computations to get normalized screen coordinates
	scratch_x = (p.x - shiftX) / scaleX;
	scratch_y = (p.y - shiftY) / scaleY;
	scratch_z = 1.0f;
	
	// Assume the distance from the viewer is 1.0 -- we'll normalize later
	// This means we don't have to undo the perspective divide
	
	// Now undo the field of view scale applied in the transformation
	scratch_x /= oneOVERtanHFOV;
	scratch_y /= oneOVERtanVFOV;

	// Undo the camera rotation (apply the inverse rotation)
	x = cameraRot.M11*scratch_z + cameraRot.M21*scratch_x + cameraRot.M31*scratch_y;
	y = cameraRot.M12*scratch_z + cameraRot.M22*scratch_x + cameraRot.M32*scratch_y;
	z = cameraRot.M13*scratch_z + cameraRot.M23*scratch_x + cameraRot.M33*scratch_y;

	// Don't need to undo the camera translation, because we want a vector
	// Lets normalize just to be kind
	mag = x*x + y*y + z*z;
	mag = 1.0f / (float)sqrt( mag );
	result.x =	x * mag;
	result.y =	y * mag;
	result.z =	z * mag;
}

		public void	TransformCameraCentricPoint (Tpoint p, out ThreeDVertex result)
			{
	 float		scratch_x;
	 float		scratch_y;
	 float		scratch_z;
	 DWORD		clipFlag;


	// This part does rotation, translation, and scaling
	// Note, we're swapping the x and z axes here to get from z up/down to z far/near
	// then we're swapping the x and y axes to get into conventional screen pixel coordinates
	scratch_z = T.M11 * p.x + T.M12 * p.y + T.M13 * p.z;
	scratch_x = T.M21 * p.x + T.M22 * p.y + T.M23 * p.z;
	scratch_y = T.M31 * p.x + T.M32 * p.y + T.M33 * p.z;


	// Now determine if the point is out behind us or to the sides
	clipFlag  = GetRangeClipFlags(      scratch_z, far_clip );
	clipFlag |= GetHorizontalClipFlags( scratch_x, scratch_z );
	clipFlag |= GetVerticalClipFlags(   scratch_y, scratch_z );

	// Finally, do the perspective divide and scale and shift into screen space
	 float OneOverZ = 1.0f/scratch_z;
	result.csX = scratch_x;
	result.csY = scratch_y;
	result.csZ = scratch_z;
	result.x = viewportXtoPixel( scratch_x * OneOverZ );
	result.y = viewportYtoPixel( scratch_y * OneOverZ );
	result.clipFlag = clipFlag;
}

		public float ZDistanceFromCamera (Tpoint p)
			{
	// This part does rotation, translation, and scaling
	// Note, we're swapping the x and z axes here to get from z up/down to z far/near
	// then we're swapping the x and y axes to get into conventional screen pixel coordinates.
	// Here we only care about the Z axis since that is the distance from the viewer.
	return T.M11 * p.x + T.M12 * p.y + T.M13 * p.z + move.x;
}


		// Draw flat shaded geometric primitives in world space using the current camera parameters
		public void Render3DPoint (Tpoint p1)
		{
			ThreeDVertex ps1;
		
			// Transform the point from world space to window space
			TransformPoint( p1, &ps1 );
			if ( ps1.clipFlag != ON_SCREEN )  return;
		
			// Draw the point
			Render2DPoint( (UInt16)ps1.x, (UInt16)ps1.y );
		}

		public void Render3DLine (Tpoint p1, Tpoint p2)
			{
	ThreeDVertex ps1, ps2;

	// Transform the points from world space to window space
	TransformPoint( p1, out ps1 );
	TransformPoint( p2, out ps2 );

	// Quit now if both ends are clipped by the same edge
	if (ps1.clipFlag & ps2.clipFlag)  return;

	// Clip the line as necessary
	if (ps1.clipFlag & CLIP_NEAR) {
		IntersectNear( &ps1, &ps2, &ps1 );
	} else if (ps2.clipFlag & CLIP_NEAR) {
		IntersectNear( &ps1, &ps2, &ps2 );
	}
	if (ps1.clipFlag & ps2.clipFlag)  return;

	if (ps1.clipFlag & CLIP_BOTTOM) {
		IntersectBottom( &ps1, &ps2, &ps1 );
	} else if (ps2.clipFlag & CLIP_BOTTOM) {
		IntersectBottom( &ps1, &ps2, &ps2 );
	}
	if (ps1.clipFlag & CLIP_TOP) {
		IntersectTop( &ps1, &ps2, &ps1 );
	} else if (ps2.clipFlag & CLIP_TOP) {
		IntersectTop( &ps1, &ps2, &ps2 );
	}
	if (ps1.clipFlag & ps2.clipFlag)  return;

	if (ps1.clipFlag & CLIP_RIGHT) {
		IntersectRight( &ps1, &ps2, &ps1 );
	} else if (ps2.clipFlag & CLIP_RIGHT) {
		IntersectRight( &ps1, &ps2, &ps2 );
	}
	if (ps1.clipFlag & CLIP_LEFT) {
		IntersectLeft( &ps1, &ps2, &ps1 );
	} else if (ps2.clipFlag & CLIP_LEFT) {
		IntersectLeft( &ps1, &ps2, &ps2 );
	}

	// Draw the line
	Render2DLine( (UInt16)ps1.x, (UInt16)ps1.y, (UInt16)ps2.x, (UInt16)ps2.y );
}

		public void Render3DFlatTri (Tpoint p1, Tpoint p2, Tpoint p3)
			{
	ThreeDVertex ps1, ps2, ps3;

	// Transform the points from world space to window space
	TransformPoint( p1, &ps1 );
	TransformPoint( p2, &ps2 );
	TransformPoint( p3, &ps3 );

	// Skip the tri if clipped anywhere
	// I'm not sure this function is called anywhere anyway.  With the current
	// state of things, just checking near clip could cause bad problems since
	// MPR no longer does other edge clipping.  We'd have to do that here.
	if (ps1.clipFlag || ps2.clipFlag || ps3.clipFlag)  return;

	// Don't draw the triangle if it is backfacing (counter-clockwise in screen space)
	// edg: always draw irregardless of backfacing
	/*
	if ( (ps2.y - ps1.y)*(ps3.x - ps1.x) > 
		 (ps2.x - ps1.x)*(ps3.y - ps1.y)   ) {
		return;
	}
	*/

	// Draw the triangle
	Render2DTri( (UInt16)ps1.x, (UInt16)ps1.y, (UInt16)ps2.x, (UInt16)ps2.y, (UInt16)ps3.x, (UInt16)ps3.y );
}

		public void Render3DObject (int id, Tpoint pos, Trotation orientation)
			{
	Debug.Assert( IsReady() );

	ObjectInstance instance = new ObjectInstance( id );

	TheStateStack.DrawObject( &instance, orientation, pos );
}

		// Draw a full featured square or tri given the already transformed (but not clipped) verts
		public void DrawSquare (ThreeDVertex v0, ThreeDVertex v1, ThreeDVertex v2, ThreeDVertex v3, int CullFlag, bool gifPicture = false)
			{
	ushort		count;
	bool				useFirst = true;
	bool				useLast = true;

	// Check the clipping flags on the verteces which bound this region	
	if(v0.clipFlag | v1.clipFlag | v2.clipFlag | v3.clipFlag)
	{		
		// If all verticies are clipped by the same edge, skip this square
		if(v0.clipFlag & v1.clipFlag & v2.clipFlag & v3.clipFlag)
			return;														

		ThreeDVertex[] vertPointers = new ThreeDVertex[4];
		vertPointers[2] = v2;

		// If any verteces are clipped, do separate triangles since the quad isn't necessarily planar
		if(v0.clipFlag | v1.clipFlag | v2.clipFlag)
		{
			vertPointers[0] = v0;
			vertPointers[1] = v1;
			ClipAndDraw3DFan(&vertPointers[0], 3, CullFlag, gifPicture);
			useFirst = false;
		}

		if(v0.clipFlag | v2.clipFlag | v3.clipFlag)
		{
			vertPointers[1] = v0;
			vertPointers[3] = v3;
			ClipAndDraw3DFan(&vertPointers[1], 3, CullFlag, gifPicture);
			if(useFirst)
				useLast = false;
			else
				return;
		}
	}

	if(CullFlag)
	{
		if(CullFlag == CULL_ALLOW_CW)
		{
			// Decide if either of the two triangles are back facing
			if (useFirst)
			{
				if(((v2.y - v1.y))*((v0.x - v1.x)) > ((v2.x - v1.x)) * ((v0.y - v1.y)))
					useFirst = false;
			}

			if(useLast)
			{
				if(((v0.y - v3.y))*((v2.x - v3.x)) > ((v0.x - v3.x)) * ((v2.y - v3.y)))
					useLast = false;
			}
		}

		else
		{
			// Decide if either of the two triangles are back facing
			if(useFirst)
			{
				if(((v2.y - v1.y)) * ((v0.x - v1.x)) < ((v2.x - v1.x)) * ((v0.y - v1.y)))
					useFirst = false;
			}

			if(useLast)
			{
				if(((v0.y - v3.y)) * ((v2.x - v3.x)) < ((v0.x - v3.x)) * ((v2.y - v3.y)))
					useLast = false;
			}
		}
	}
	
	// If culling or clipping took care of both triangles, quit now
	if(useFirst && useLast)
		count = 4;
	else
	{
		if(useFirst || useLast)
			count = 3;
		else
			return;
	}

	if(useFirst)
	{
		MPRVtxTexClr_t[] arr = new MPRVtxTexClr_t[] { v0, v1, v2, v3 };
		context.DrawPrimitive(MPR_PRM_TRIFAN, MPR_VI_COLOR | MPR_VI_TEXTURE, count, arr);
	}
	else 
	{
		MPRVtxTexClr_t[] arr  = new MPRVtxTexClr_t[] { v0, v2, v3 };
		context.DrawPrimitive(MPR_PRM_TRIFAN, MPR_VI_COLOR | MPR_VI_TEXTURE, count, arr);
	}

}

		public void DrawTriangle (ThreeDVertex v0, ThreeDVertex v1, ThreeDVertex v2, int CullFlag, bool gifPicture = false)
			{
	// Check the clipping flags on the verteces which bound this region	
	if(v0.clipFlag | v1.clipFlag | v2.clipFlag)
	{															
		// If all verticies are clipped by the same edge, skip this triangle
		if(v0.clipFlag & v1.clipFlag & v2.clipFlag)
			return;														
																			
		// If any verteces are clipped, do them as a special case
		ThreeDVertex[] vertPointers = new ThreeDVertex[3];
		vertPointers[0] = v0;
		vertPointers[1] = v1;
		vertPointers[2] = v2;
		ClipAndDraw3DFan( vertPointers, 3, CullFlag, gifPicture );
		return;
	}

	if(CullFlag)
	{
		if(CullFlag == CULL_ALLOW_CW)
		{
			// Decide if back facing CW
			if(((v2.y - v1.y)) * ((v0.x - v1.x)) > ((v2.x - v1.x)) * ((v0.y - v1.y)))
				return;
		}

		else
		{
			// Decide if back facing CCW
			if(((v2.y - v1.y)) * ((v0.x - v1.x)) < ((v2.x - v1.x)) * ((v0.y - v1.y)))
				return;
		}
	}

	// Draw the tri
	MPRVtxTexClr_t[] arr = new MPRVtxTexClr_t[] { v0, v1, v2 };
	context.DrawPrimitive(MPR_PRM_TRIFAN, MPR_VI_COLOR | MPR_VI_TEXTURE, 3, arr);
}

  
		// Draw a fan which is known to require clipping
		protected void ClipAndDraw3DFan (ThreeDVertex[] vertPointers, uint count, int CullFlag, bool gifPicture = false)
			{
	ThreeDVertex[]	v, p, lastIn, nextOut;
	ThreeDVertex[]	inList, outList, temp;
	ThreeDVertex[]	vertList1 = new ThreeDVertex[MAX_VERT_LIST];	// Used to hold poly vert pointer lists
	ThreeDVertex[]	vertList2 = new ThreeDVertex[MAX_VERT_LIST];	// Used to hold poly vert pointer lists
	DWORD			clipTest = 0;

	Debug.Assert( vertPointers );
	Debug.Assert( count >= 3 );

	// Intialize the vertex buffers
	outList			= vertList1;
	lastIn			= vertPointers + count;
	for (nextOut = outList; vertPointers < lastIn; nextOut++) {
		clipTest |= (*vertPointers).clipFlag;
		*nextOut = (*vertPointers++);
	}
	Debug.Assert( nextOut - outList <= MAX_VERT_LIST );
	inList			= vertList2;
	extraVertCount	= 0;


	// Clip to the near plane
	if (clipTest & CLIP_NEAR) {
		temp = inList;
		inList = outList;
		outList = temp;
		lastIn = nextOut-1;
		nextOut = outList;

		for (p=lastIn, v=&inList[0]; v <= lastIn; v++) {

			// If the edge between this vert and the previous one crosses the line, trim it
			if (((*p).clipFlag ^ (*v).clipFlag) & CLIP_NEAR) {
				Debug.Assert( extraVertCount < MAX_EXTRA_VERTS );
				*nextOut = &extraVerts[extraVertCount];
				extraVertCount++;
				IntersectNear( *p, *v, *nextOut );
				clipTest |= (*nextOut).clipFlag;
				nextOut++;
			}
			
			// If this vert isn't clipped, use it
			if (!((*v).clipFlag & CLIP_NEAR)) {
				*nextOut++ = *v;
			}

			p = v;
		}
		Debug.Assert( nextOut - outList <= MAX_VERT_LIST );
		if (nextOut - outList <= 2)  return;

		// NOTE:  We might get to this point and find a polygon is now marked totally clipped
		// since doing the near clip can change the flags and make a vertex appear to have
		// changed sides of the viewer.  We'll ignore this issue since it is quietly handled
		// and would probably cost more to detect than it would save it early termination.
	}


#if DO_BACKFACE_CULLING
	// Note:  we handle only leading and trailing culled triangles.  If
	// a more complicated non-planar polygon with interior triangles culled is
	// presented, too many triangles will get culled.  To handle that case, we'd
	// have to check all triangles instead of stopping after the second reject loop below.
	// If a new set of un-culled triangles was encountered, we'd have to make a new polygon
	// and resubmit it.
	if ( CullFlag ) {
		temp = inList;
		inList = outList;
		outList = temp;
		lastIn = nextOut-1;
		nextOut = outList;

		// We only support one flavor of clipping right now. The other version would just
		// be this same code repeated with inverted compare signs.
		Debug.Assert( CullFlag == CULL_ALLOW_CW );

		// Always copy the vertex at the root of the fan
		*nextOut++ = *inList;

		for (p=&inList[0], v=&inList[1]; v < lastIn; v++) {
			// Only clockwise triangles are accepted
			if ( (((*(v+1)).y - (*v).y))*(((*p).x - (*v).x)) < 
				 (((*(v+1)).x - (*v).x))*(((*p).y - (*v).y))   ) {
				// Accept!
				break;
			}
		}
		*nextOut++ = *v;
		for (p, v; v < lastIn; v++) {
			// Only clockwise triangles are accepted
			if ( (((*(v+1)).y - (*v).y))*(((*p).x - (*v).x)) >= 
				 (((*(v+1)).x - (*v).x))*(((*p).y - (*v).y))   ) {
				// Reject!
				break;
			}
			*nextOut++ = *(v+1);
		}
		Debug.Assert( nextOut - outList <= MAX_VERT_LIST );
		if (nextOut - outList <= 2)  return;
	}
#endif

// 2002-04-06 MN if gifPicture is false, then do the other clippings (for terrain and stuff). 
// GifPicture is only locally set to true in the case we draw a celestial object.
// Sun and Moon GIF's are displayed bad when being clipped by below code.
	if (!gifPicture)
	{
#if ! DO_NEAR_CLIP_ONLY
	// Clip to the bottom plane
	if (clipTest & CLIP_BOTTOM) {
		temp = inList;
		inList = outList;
		outList = temp;
		lastIn = nextOut-1;
		nextOut = outList;

		for (p=lastIn, v=&inList[0]; v <= lastIn; v++) {

			// If the edge between this vert and the previous one crosses the line, trim it
			if (((*p).clipFlag ^ (*v).clipFlag) & CLIP_BOTTOM) {
				Debug.Assert( extraVertCount < MAX_EXTRA_VERTS );
				*nextOut = &extraVerts[extraVertCount];
				extraVertCount++;
				IntersectBottom( *p, *v, *nextOut++ );
			}
			
			// If this vert isn't clipped, use it
			if (!((*v).clipFlag & CLIP_BOTTOM)) {
				*nextOut++ = *v;
			}

			p = v;
		}
		Debug.Assert( nextOut - outList <= MAX_VERT_LIST );
		if (nextOut - outList <= 2)  return;
	}

	
	// Clip to the top plane
	if (clipTest & CLIP_TOP) {
		temp = inList;
		inList = outList;
		outList = temp;
		lastIn = nextOut-1;
		nextOut = outList;

		for (p=lastIn, v=&inList[0]; v <= lastIn; v++) {

			// If the edge between this vert and the previous one crosses the line, trim it
			if (((*p).clipFlag ^ (*v).clipFlag) & CLIP_TOP) {
				Debug.Assert( extraVertCount < MAX_EXTRA_VERTS );
				*nextOut = &extraVerts[extraVertCount];
				extraVertCount++;
				IntersectTop( *p, *v, *nextOut++ );
			}
			
			// If this vert isn't clipped, use it
			if (!((*v).clipFlag & CLIP_TOP)) {
				*nextOut++ = *v;
			}

			p = v;
		}
		Debug.Assert( nextOut - outList <= MAX_VERT_LIST );
		if (nextOut - outList <= 2)  return;
	}


	// Clip to the right plane
	if (clipTest & CLIP_RIGHT) {
		temp = inList;
		inList = outList;
		outList = temp;
		lastIn = nextOut-1;
		nextOut = outList;

		for (p=lastIn, v=&inList[0]; v <= lastIn; v++) {

			// If the edge between this vert and the previous one crosses the line, trim it
			if (((*p).clipFlag ^ (*v).clipFlag) & CLIP_RIGHT) {
				Debug.Assert( extraVertCount < MAX_EXTRA_VERTS );
				*nextOut = &extraVerts[extraVertCount];
				extraVertCount++;
				IntersectRight( *p, *v, *nextOut++ );
			}
			
			// If this vert isn't clipped, use it
			if (!((*v).clipFlag & CLIP_RIGHT)) {
				*nextOut++ = *v;
			}

			p = v;
		}
		Debug.Assert( nextOut - outList <= MAX_VERT_LIST );
		if (nextOut - outList <= 2)  return;
	}

	
	// Clip to the left plane
	if (clipTest & CLIP_LEFT) {
		temp = inList;
		inList = outList;
		outList = temp;
		lastIn = nextOut-1;
		nextOut = outList;

		for (p=lastIn, v=&inList[0]; v <= lastIn; v++) {

			// If the edge between this vert and the previous one crosses the line, trim it
			if (((*p).clipFlag ^ (*v).clipFlag) & CLIP_LEFT) {
				Debug.Assert( extraVertCount < MAX_EXTRA_VERTS );
				*nextOut = &extraVerts[extraVertCount];
				extraVertCount++;
				IntersectLeft( *p, *v, *nextOut++ );
			}
			
			// If this vert isn't clipped, use it
			if (!((*v).clipFlag & CLIP_LEFT)) {
				*nextOut++ = *v;
			}

			p = v;
		}
		Debug.Assert( nextOut - outList <= MAX_VERT_LIST );
		if (nextOut - outList <= 2)  return;
	}
#endif	// DO_NEAR_CLIP_ONLY
	}

	// Finally draw the resultant polygon
// OW
#if NOTHING
	context.Primitive(	MPR_PRM_TRIFAN,
						MPR_VI_COLOR | MPR_VI_TEXTURE,
						(unsigned short)(nextOut - outList), sizeof(MPRVtxTexClr_t) );

	for (v=outList; v<nextOut; v++)
	{
//		Debug.Assert ((*v).q >= -0.001F);
	
		context.StorePrimitiveVertexData(*v);
	}
#else
	context.DrawPrimitive(	MPR_PRM_TRIFAN, MPR_VI_COLOR | MPR_VI_TEXTURE,
		(ushort)(nextOut - outList), (MPRVtxTexClr_t **) outList);
#endif
}
  private void InterpolateColorAndTex( ThreeDVertex v1, ThreeDVertex v2, out ThreeDVertex v, float t )
{
	// Compute the interpolated color and texture coordinates
	v.r = v1.r + t*(v2.r - v1.r);
	v.g = v1.g + t*(v2.g - v1.g);
	v.b = v1.b + t*(v2.b - v1.b);
	v.a = v1.a + t*(v2.a - v1.a);
			 		   		   
	v.u = v1.u + t*(v2.u - v1.u);
	v.v = v1.v + t*(v2.v - v1.v);
	v.q = v.csZ * Q_SCALE;			// Need to preserve scaling to 16.16
}
		private void IntersectNear (ThreeDVertex v1, ThreeDVertex v2, out ThreeDVertex v)
			{
	float			x, y, z, t;

	// Compute the parametric location of the intersection of the edge and the clip plane
	t = (NEAR_CLIP - v1.csZ) / (v2.csZ - v1.csZ);
	Debug.Assert( (t >= -0.001f) && (t <= 1.001f) );
	
	// Compute the camera space intersection point
	v.csZ = z = NEAR_CLIP;
	v.csX = x = v1.csX + t * (v2.csX - v1.csX);
	v.csY = y = v1.csY + t * (v2.csY - v1.csY);


	// Compute the interpolated color and texture coordinates
	InterpolateColorAndTex( v1, v2, v, t );

	// Now determine if the point is out to the sides
	v.clipFlag  = GetHorizontalClipFlags( x, z );
	v.clipFlag |= GetVerticalClipFlags(   y, z );

	// Compute the screen space coordinates of the new point
	float OneOverZ = 1.0f / z;
	v.x = viewportXtoPixel( x * OneOverZ );
	v.y = viewportYtoPixel( y * OneOverZ );
}

		private void IntersectTop (ThreeDVertex v1, ThreeDVertex v2, out ThreeDVertex v)
			{
	float	x, y, z, t;
	float	dx, dy, dz;	

	// Compute the parametric location of the intersection of the edge and the clip plane
	dx = v2.csX - v1.csX;
	dy = v2.csY - v1.csY;
	dz = v2.csZ - v1.csZ;
	t = (v1.csZ + v1.csY) / (-dz - dy);
	Debug.Assert( (t >= -0.001f) && (t <= 1.001f) );
	
	// Compute the camera space intersection point
	v.csZ = z = v1.csZ + t * (dz);
	v.csX = x = v1.csX + t * (dx);	// Note: either dx or dy is used only once, so could
	v.csY = y = v1.csY + t * (dy);	// be avoided, but this way, the code is more standardized...


	// Compute the interpolated color and texture coordinates
	InterpolateColorAndTex( v1, v2, v, t );

	// Now determine if the point is out to the sides
	v.clipFlag  = GetHorizontalClipFlags( x, z );

	// Compute the screen space coordinates of the new point
	float OneOverZ = 1.0f / z;
	v.x = viewportXtoPixel( x * OneOverZ );
	v.y = viewportYtoPixel( y * OneOverZ );
}

		private void IntersectBottom (ThreeDVertex v1, ThreeDVertex v2, out ThreeDVertex v)
			{
	float	x, y, z, t;
	float	dx, dy, dz;	

	// Compute the parametric location of the intersection of the edge and the clip plane
	dx = v2.csX - v1.csX;
	dy = v2.csY - v1.csY;
	dz = v2.csZ - v1.csZ;
	t = (v1.csY - v1.csZ) / (dz - dy);
	Debug.Assert( (t >= -0.001f) && (t <= 1.001f) );
	
	// Compute the camera space intersection point
	v.csZ = z = v1.csZ + t * (dz);
	v.csX = x = v1.csX + t * (dx);	// Note: either dx or dy is used only once, so could
	v.csY = y = v1.csY + t * (dy);	// be avoided, but this way, the code is more standardized...


	// Compute the interpolated color and texture coordinates
	InterpolateColorAndTex( v1, v2, v, t );

	// Now determine if the point is out to the sides
	v.clipFlag  = GetHorizontalClipFlags( x, z );

	// Compute the screen space coordinates of the new point
	float OneOverZ = 1.0f / z;
	v.x = viewportXtoPixel( x * OneOverZ );
	v.y = viewportYtoPixel( y * OneOverZ );
}


		private void IntersectLeft (ThreeDVertex v1, ThreeDVertex v2, out ThreeDVertex v)
			{
	float	x, y, z, t;
	float	dx, dy, dz;	

	// Compute the parametric location of the intersection of the edge and the clip plane
	dx = v2.csX - v1.csX;
	dy = v2.csY - v1.csY;
	dz = v2.csZ - v1.csZ;
	t = (v1.csZ + v1.csX) / (-dz - dx);
	Debug.Assert( (t >= -0.001f) && (t <= 1.001f) );
	
	// Compute the camera space intersection point
	v.csZ = z = v1.csZ + t * (dz);
	v.csX = x = v1.csX + t * (dx);	// Note: either dx or dy is used only once, so could
	v.csY = y = v1.csY + t * (dy);	// be avoided, but this way, the code is more standardized...


	// Compute the interpolated color and texture coordinates
	InterpolateColorAndTex( v1, v2, v, t );

	// Now determine if the point is out to the sides
	// (this point MUST be on screen because we've done all clipping at this point
	v.clipFlag = ON_SCREEN;

	// Compute the screen space coordinates of the new point
	float OneOverZ = 1.0f / z;
	v.x = viewportXtoPixel( x * OneOverZ );
	v.y = viewportYtoPixel( y * OneOverZ );
}

		private void IntersectRight (ThreeDVertex v1, ThreeDVertex v2, out ThreeDVertex v)
		{
	float	x, y, z, t;
	float	dx, dy, dz;	

	// Compute the parametric location of the intersection of the edge and the clip plane
	dx = v2.csX - v1.csX;
	dy = v2.csY - v1.csY;
	dz = v2.csZ - v1.csZ;
	t = (v1.csX - v1.csZ) / (dz - dx);
	Debug.Assert( (t >= -0.001f) && (t <= 1.001f) );
	
	// Compute the camera space intersection point
	v.csZ = z = v1.csZ + t * (dz);
	v.csX = x = v1.csX + t * (dx);	// Note: either dx or dy is used only once, so could
	v.csY = y = v1.csY + t * (dy);	// be avoided, but this way, the code is more standardized...


	// Compute the interpolated color and texture coordinates
	InterpolateColorAndTex( v1, v2, v, t );

	// Now determine if the point is out to the sides
	// (this point MUST be on screen because all that is left is Left edge clipping, 
	// and this point is onthe right).
	v.clipFlag = ON_SCREEN;

	// Compute the screen space coordinates of the new point
	float OneOverZ = 1.0f / z;
	v.x = viewportXtoPixel( x * OneOverZ );
	v.y = viewportYtoPixel( y * OneOverZ );
}


/* Helper functions to compute the horizontal, vertical, and near clip flags */
		public static DWORD GetRangeClipFlags (float z, float d)
		{
			if (z < NEAR_CLIP) {
				return CLIP_NEAR;
			}
//	if ( z > far_clip ) {
//		return CLIP_FAR;
//	}
			return ON_SCREEN;
		}

		public static  DWORD GetHorizontalClipFlags (float x, float z)
		{
			if (fabs (x) > z) {
				if (x > z) {
					return CLIP_RIGHT;
				} else {
					return CLIP_LEFT;
				}
			}
			return ON_SCREEN;
		}

		public static  DWORD GetVerticalClipFlags (float y, float z)
		{
			if (fabs (y) > z) {
				if (y > z) {
					return CLIP_BOTTOM;
				} else {
					return CLIP_TOP;
				}
			}
			return ON_SCREEN;
		}
		
		protected float	far_clip;
		protected float	detailScaler;
		protected float	resRelativeScaler;
		protected bool	objTextureState;
		protected float	horizontal_half_angle;
		protected float	vertical_half_angle;
		protected float	diagonal_half_angle;
		protected float	oneOVERtanHFOV;
		protected float	oneOVERtanVFOV;
		protected float	yaw;
		protected float	pitch;
		protected float	roll;
		protected Tpoint		cameraPos;			// Camera position in world space
		protected Trotation	cameraRot;			// Camera orientation matrix

		protected float		lightAmbient;
		protected float		lightDiffuse;
		protected Tpoint		lightVector;
		protected Tpoint		move;				// Camera space translation required to position visible objects
		protected Trotation	T;					// Transformation matrix including aspect ratio and FOV effects
		protected Trotation	Tbb;				// Transformation matrix for billboards
		protected Trotation	Tt;					// Transformation matrix for trees
	}
}

