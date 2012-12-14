using System;

namespace FalconNet.Graphics
{
/***************************************************************************\
    Tpost.h
    Scott Randolph
    August 21, 1995

    Most basic element of the terrain map.  Posts contain all the
    information about a point on the ground.  There may in the future be
    multiple versions of posts (ie: textured vs. not textured).  For now,
    only one.

	This is version that is used at runtime in memory.
\***************************************************************************/
	public struct Tpost
	{

		// X can be obtained from:  	x = LEVEL_POST_TO_WORLD( levelRow, LOD );
		// Y can be obtained from:  	y = LEVEL_POST_TO_WORLD( levelCol, LOD );
		public float		z;		// Units of floating point feet -- positive z axis points DOWN

		// Color information
		public int			colorIndex;	// (could shrink to a byte)

		// Texture information
		public float		u;		// u is East/West         
		public float		v;		// v is North/South       
		public float		d;
		public TextureID	texID;	// 16 bit value (see TerrTex.h)	   

		// normal information
		public float		theta;
		public float		phi;

	};
}

