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
	};
	#if TODO
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
		public virtual void Setup (ImageBuffer *imageBuffer);

		public virtual void Cleanup ()
		{
			Render2D::Cleanup ();
		}

		// Overload this function to get extra work done at start frame
		public virtual void StartFrame ();
	
		// Set the camera parameters we're to use for rendering
		public void SetObjectDetail (float scaler);

		public void SetFOV (float horizontal_fov);

		public void SetFar (float distance)
		{
			far_clip = distance;
		}

		public void SetCamera (Tpoint* pos, Trotation* rot);

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

		public virtual void SetViewport (float leftSide, float topSide, float rightSide, float bottomSide);

		// Setup the 3D object lighting
		public void SetLightDirection (Tpoint* dir);

		public void GetLightDirection (Tpoint* dir);

		// Trun 3D object texturing on/off
		public void SetObjectTextureState (BOOL state);

		public BOOL GetObjectTextureState ()
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

		public void	GetAt (Tpoint *v)
		{
			v.x = cameraRot.M11;
			v.y = cameraRot.M12;
			v.z = cameraRot.M13;
		}

		public void	GetLeft (Tpoint *v)
		{
			v.x = cameraRot.M21;
			v.y = cameraRot.M22;
			v.z = cameraRot.M23;
		}

		public void	GetUp (Tpoint *v)
		{
			v.x = cameraRot.M31;
			v.y = cameraRot.M32;
			v.z = cameraRot.M33;
		}


		// Transform the given worldspace point into pixel coordinates using the current camera
		public void	TransformPoint (Tpoint* world, ThreeDVertex* pixel);

		public void	TransformPointToView (Tpoint* world, Tpoint *result);

		public void	TransformBillboardPoint (Tpoint* world, Tpoint *viewOffset, ThreeDVertex* pixel);

		public void	TransformTreePoint (Tpoint* world, Tpoint *viewOffset, ThreeDVertex* pixel);

		public void	UnTransformPoint (Tpoint* pixel, Tpoint* vector);

		public void	TransformCameraCentricPoint (Tpoint* world, ThreeDVertex* pixel);

		public float	ZDistanceFromCamera (Tpoint* p);


		// Draw flat shaded geometric primitives in world space using the current camera parameters
		public void Render3DPoint (Tpoint* p1);

		public void Render3DLine (Tpoint* p1, Tpoint* p2);

		public void Render3DFlatTri (Tpoint* p1, Tpoint* p2, Tpoint* p3);

		public void Render3DObject (GLint id, Tpoint* pos, Trotation* orientation);

		// Draw a full featured square or tri given the already transformed (but not clipped) verts
		public void DrawSquare (ThreeDVertex* v0, ThreeDVertex* v1, ThreeDVertex* v2, ThreeDVertex* v3, int CullFlag, bool gifPicture = false);

		public void DrawTriangle (ThreeDVertex* v0, ThreeDVertex* v1, ThreeDVertex* v2, int CullFlag, bool gifPicture = false);

  
		// Draw a fan which is known to require clipping
		protected void ClipAndDraw3DFan (ThreeDVertex** vertPointers, unsigned count, int CullFlag, bool gifPicture = false);
  
		private void IntersectNear (ThreeDVertex *v1, ThreeDVertex *v2, ThreeDVertex *v);

		private void IntersectTop (ThreeDVertex *v1, ThreeDVertex *v2, ThreeDVertex *v);

		private void IntersectBottom (ThreeDVertex *v1, ThreeDVertex *v2, ThreeDVertex *v);

		private void IntersectLeft (ThreeDVertex *v1, ThreeDVertex *v2, ThreeDVertex *v);

		private void IntersectRight (ThreeDVertex *v1, ThreeDVertex *v2, ThreeDVertex *v);

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
		protected BOOL	objTextureState;
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
	};
#endif
}

