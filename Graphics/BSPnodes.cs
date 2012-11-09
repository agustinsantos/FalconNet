using System;
using System.Diagnostics;

namespace FalconNet.Graphics
{
	public enum BNodeType
	{
		tagBNode,
		tagBSubTree,
		tagBRoot,
		tagBSlotNode,
		tagBDofNode,
		tagBSwitchNode,
		tagBSplitterNode,
		tagBPrimitiveNode,
		tagBLitPrimitiveNode,
		tagBCulledPrimitiveNode,
		tagBSpecialXform,
		tagBLightStringNode,
	}

	public enum BTransformType
	{
		Normal,
		Billboard,
		Tree
	} 
#if TODO
	/***************************************************************\
		To improve performance, these classes use several global
		variables to store command data instead of passing it
		through the call stack.
		The global variables are maintained by the StackState
		module.
	\***************************************************************/
	public class BNode
	{
		// Convert from file offsets back to pointers   
		public BNode (byte *baseAddress, BNodeType **tagListPtr)
		{
			// Fixup our sibling, if any
			if ((int)sibling >= 0) {
				sibling = RestorePointers( baseAddress, (int)sibling, tagListPtr );
			} else {
				sibling = null;
			}
		}
	
		public BNode ()
		{
			sibling = null;
		}
		// public  virtual ~BNode()									{ delete sibling; };

#if TODO
	// We have special new and delete operators which don't do any memory operations
	public void *operator new(size_t, void *p)	{ return p; }
	public void *operator new(size_t n)			{ return malloc(n); }
	public void operator delete(void *)			{ }
//	void operator delete(void *, void *) { };
#endif
		// Function to identify the type of an encoded node and call the appropriate constructor
		// Determine the type of an encoded node and intialize and contruct
		// it appropriatly.
		public static BNode RestorePointers (byte *baseAddress, int offset, BNodeType **tagListPtr)
		{
#if TODO
	BNode		*node;
	BNodeType	tag;

	// Get this record's tag then advance the pointer to the next tag we'll need
	tag = **tagListPtr;
	*tagListPtr = (*tagListPtr)+1;
	

	// Apply the proper virtual table setup and constructor
	switch( tag ) {
	  case BNodeType.tagBSubTree:
		node = new (baseAddress+offset) BSubTree( baseAddress, tagListPtr );
		break;
	  case BNodeType.tagBRoot:
		node = new (baseAddress+offset) BRoot( baseAddress, tagListPtr );
		break;
	  case BNodeType.tagBSpecialXform:
		node = new (baseAddress+offset) BSpecialXform( baseAddress, tagListPtr );
		break;
	  case BNodeType.tagBSlotNode:
		node = new (baseAddress+offset) BSlotNode( baseAddress, tagListPtr );
		break;
	  case BNodeType.tagBDofNode:
		node = new (baseAddress+offset) BDofNode( baseAddress, tagListPtr );
		break;
	  case BNodeType.tagBSwitchNode:
		node = new (baseAddress+offset) BSwitchNode( baseAddress, tagListPtr );
		break;
	  case BNodeType.tagBSplitterNode:
		node = new (baseAddress+offset) BSplitterNode( baseAddress, tagListPtr );
		break;
	  case BNodeType.tagBPrimitiveNode:
		node = new (baseAddress+offset) BPrimitiveNode( baseAddress, tagListPtr );
		break;
	  case BNodeType.tagBLitPrimitiveNode:
		node = new (baseAddress+offset) BLitPrimitiveNode( baseAddress, tagListPtr );
		break;
	  case BNodeType.tagBCulledPrimitiveNode:
		node = new (baseAddress+offset) BCulledPrimitiveNode( baseAddress, tagListPtr );
		break;
	  case BNodeType.tagBLightStringNode:
		node = new (baseAddress+offset) BLightStringNode( baseAddress, tagListPtr );
		break;
	  default:
		Debug.Fail("Decoding unrecognized BSP node type.");
	}

	return node;
#endif 
			throw new NotImplementedException();
}


		public BNode	sibling;

		public abstract void		Draw ();

		public virtual BNodeType	Type ()
		{
			return BNodeType.tagBNode;
		}
	};

	// Convert from file offsets back to pointers
	public class BSubTree:   BNode
	{
 
		public BSubTree (byte *baseAddress, BNodeType **tagListPtr)
		:base( baseAddress, tagListPtr )
		{
			// Fixup our dependents
			subTree		= RestorePointers( baseAddress, (int)subTree, tagListPtr );
		
			// Fixup our data pointers
			pCoords		= (Tpoint*)(baseAddress + (int)pCoords);
			pNormals	= (Pnormal*)(baseAddress + (int)pNormals);
		}
	
		public BSubTree ()
		{
			subTree = null;
		}
		//public virtual ~BSubTree()	{ delete subTree; };

		public Tpoint	*pCoords;
		public int		nCoords;
		public int		nDynamicCoords;
		public int		DynamicCoordOffset;
		public Pnormal	*pNormals;
		public int		nNormals;
		public BNode	*subTree;

		public override void		Draw ()
		{
			BNode	*child;
		
			TheStateStack.Light( pNormals, nNormals );
		
			if (nDynamicCoords == 0) {
				TheStateStack.Transform( pCoords, nCoords );
			} else {
				TheStateStack.Transform( pCoords, nCoords-nDynamicCoords );
				TheStateStack.Transform( TheStateStack.CurrentInstance.DynamicCoords+DynamicCoordOffset, 
										 nDynamicCoords );
			}
		
			child = subTree;
			//TODO Debug.Assert(FALSE == F4IsBadReadPtr( child, sizeof*child) );
			do
			{
				child.Draw();
				child = child.sibling;
			} while (child); // JB 010306 CTD
			//} while (child && !F4IsBadReadPtr(child, sizeof(BNode))); // JB 010306 CTD (too much CPU)
		}


		public override BNodeType	Type ()
		{
			return BNodeType.tagBSubTree;
		}
	}
	

	public class BRoot:   BSubTree
	{
		// Convert from file offsets back to pointers 
		public BRoot (byte *baseAddress, BNodeType **tagListPtr)
		:base( baseAddress, tagListPtr )
		{
			// Fixup our extra data pointers
			pTexIDs		= (int*)(baseAddress + (int)pTexIDs);
		}
	
		public BRoot () : base()
		{
			pTexIDs = null;
			nTexIDs = -1;
			ScriptNumber = -1;
		}
		//public virtual ~BRoot()	{};

		public void	LoadTextures ()
		{
			for (int i=0; i<nTexIDs; i++) {
				// Skip unsed texture indices
				if (pTexIDs[i] >= 0) {
					TheTextureBank.Reference( pTexIDs[i] );
				}
			}
		}


		public void	UnloadTextures ()
		{
			for (int i=0; i<nTexIDs; i++) {
				if (pTexIDs[i] >= 0) {
					TheTextureBank.Release( pTexIDs[i] );
				}
			}
		}

		public int		*pTexIDs;
		public int		nTexIDs;
		public int		ScriptNumber;

		public override void Draw ()
		{
			// Compute the offset to the first texture in the texture set
			int texOffset = TheStateStack.CurrentInstance.TextureSet * 
				(nTexIDs/TheStateStack.CurrentInstance.ParentObject.nTextureSets);
			TheStateStack.SetTextureTable( pTexIDs + texOffset );
										   
			if (ScriptNumber > 0) {
				Debug.Assert( ScriptNumber < ScriptArrayLength );
				if (ScriptNumber < ScriptArrayLength) {
					ScriptArray[ScriptNumber]();
				}
			}
		
			base.Draw();
		}

		public override BNodeType	Type ()
		{
			return BNodeType.tagBRoot;
		}
	}


	public class BSpecialXform:   BNode
	{
 
		// Convert from file offsets back to pointers
		public BSpecialXform (byte *baseAddress, BNodeType **tagListPtr)
		:base( baseAddress, tagListPtr )
		{
			// Fixup our dependents
			subTree		= RestorePointers( baseAddress, (int)subTree, tagListPtr );
		
			// Fixup our data pointers
			pCoords		= (Tpoint*)(baseAddress + (int)pCoords);
		}

		public BSpecialXform ()
		{
			subTree = null;
		}
		// public virtual ~BSpecialXform()	{ delete subTree; };

		public Tpoint			*pCoords;
		public int				nCoords;
		public BTransformType	type;
		public BNode			*subTree;

		public override void		Draw ()
		{
			Debug.Assert( subTree != null);
		
			TheStateStack.PushVerts();
			TheStateStack.TransformBillboardWithClip( pCoords, nCoords, type );
			subTree.Draw();
			TheStateStack.PopVerts();
		}

		public override BNodeType	Type ()
		{
			return BNodeType.tagBSpecialXform;
		}
	}
	
	
	public class BSlotNode:   BNode
	{
 
		// Convert from file offsets back to pointers
		public BSlotNode (byte *baseAddress, BNodeType **tagListPtr)
		:base( baseAddress, tagListPtr )
		{
		}
	
		public BSlotNode ()
		{
			slotNumber = -1;
		}
		// public virtual ~BSlotNode()	{};

		public Pmatrix			rotation;
		public Tpoint			origin;
		public int				slotNumber;

		public override void Draw ()
		{
			Debug.Assert( slotNumber < TheStateStack.CurrentInstance.ParentObject.nSlots );
			if (slotNumber >= TheStateStack.CurrentInstance.ParentObject.nSlots )
				return; // JPO fix
			ObjectInstance *subObject = TheStateStack.CurrentInstance.SlotChildren[slotNumber];
		
			if (subObject) {
				TheStateStack.DrawSubObject( subObject, &rotation, &origin );
			}
		}

		public override BNodeType	Type ()
		{
			return BNodeType.tagBSlotNode;
		}
	}


	public class BDofNode:   BSubTree
	{
		// Convert from file offsets back to pointers 
		public BDofNode (byte *baseAddress, BNodeType **tagListPtr)
		:base( baseAddress, tagListPtr )
		{
		}

		public BDofNode () : base()
		{
			dofNumber = -1;
		}
		//public virtual ~BDofNode()		{};

		public int			dofNumber;
		public Pmatrix		rotation;
		public Tpoint		translation;

		public override void Draw ()
		{
			Pmatrix	dofRot;
			Pmatrix	R;
			Tpoint	T;
			mlTrig trig;
		
			Debug.Assert( dofNumber < TheStateStack.CurrentInstance.ParentObject.nDOFs );
			if (dofNumber >= TheStateStack.CurrentInstance.ParentObject.nDOFs )
				return;
			// Set up our free rotation
			mlSinCos (&trig, TheStateStack.CurrentInstance.DOFValues[dofNumber].rotation);
			dofRot.M11 = 1.0f;	dofRot.M12 = 0.0f;	dofRot.M13 = 0.0f;
			dofRot.M21 = 0.0f;	dofRot.M22 = trig.cos;	dofRot.M23 = -trig.sin;
			dofRot.M31 = 0.0f;	dofRot.M32 = trig.sin;	dofRot.M33 = trig.cos;
		
		
			// Now compose this with the rotation into our parents coordinate system
			MatrixMult( &rotation, &dofRot, &R );
		
			// Now do our free translation
			// SCR 10/28/98:  THIS IS WRONG FOR TRANSLATION DOFs.  "DOFValues" is supposed to 
			// translate along the local x axis, but this will translate along the parent's x axis.
			// To fix this would require a bit more math (and/or thought).  Since it
			// only happens once in Falcon, I'll leave it broken and put a workaround into
			// the KC10 object so that the parent's and child's x axis are forced into alignment
			// by inserting an extra dummy DOF bead.
			T.x = translation.x + TheStateStack.CurrentInstance.DOFValues[dofNumber].translation;
			T.y = translation.y;
			T.z = translation.z;
		
			// Draw our subtree
			TheStateStack.PushAll();
			TheStateStack.CompoundTransform( &R, &T );
			BSubTree::Draw();
			TheStateStack.PopAll();
		}

		public override BNodeType	Type ()
		{
			return BNodeType.tagBDofNode;
		}
	}

	public class BSwitchNode:   BNode
	{
 
		// Convert from file offsets back to pointers
		public BSwitchNode (byte *baseAddress, BNodeType **tagListPtr)
		:base( baseAddress, tagListPtr )
		{
			// Fixup our table of children
			subTrees = (BSubTree**)(baseAddress + (int)subTrees);
		
			// Now fixup each child tree
			for (int i=0; i<numChildren; i++) {
				subTrees[i] = (BSubTree*)RestorePointers( baseAddress, (int)subTrees[i], tagListPtr );
			}
		}

		public BSwitchNode ()
		{
			subTrees = null;
			numChildren = 0;
			switchNumber = -1;
		}
		//public virtual ~BSwitchNode()	{ while (numChildren--) delete subTrees[numChildren]; };

		public int			switchNumber;
		public int			numChildren;
		public BSubTree	**subTrees;

		public override void Draw ()
		{
			UInt32		mask;
			int			i = 0;
		
			Debug.Assert( switchNumber < TheStateStack.CurrentInstance.ParentObject.nSwitches );
			if (switchNumber >= TheStateStack.CurrentInstance.ParentObject.nSwitches)
				return;
			mask = TheStateStack.CurrentInstance.SwitchValues[switchNumber];
		
		#if NOTHING	// This will generally be faster due to early termination
			// Go until all ON switch children have been drawn.
			while (mask) {
		#else	// This will work even if the mask is set for non-existent children
			// Go until all children have been considered for drawing.
			while (i < numChildren) {
		#endif
				Debug.Assert( subTrees[i] );
		
				// Only draw this subtree if the corresponding switch bit is set
				if (mask & 1) {
					TheStateStack.PushVerts();
					subTrees[i].Draw();
					TheStateStack.PopVerts();
				}
				mask >>= 1;
				i++;
			}
		}

		public override BNodeType	Type ()
		{
			return BNodeType.tagBSwitchNode;
		}
	};

	public class BSplitterNode:   BNode
	{
 
		// Convert from file offsets back to pointers
		public BSplitterNode (byte *baseAddress, BNodeType **tagListPtr)
		:base( baseAddress, tagListPtr )
		{
			// Fixup our dependents
			front	= RestorePointers( baseAddress, (int)front, tagListPtr );
			back	= RestorePointers( baseAddress, (int)back,  tagListPtr );
		}

		public BSplitterNode ()
		{
			front = back = null;
		}
		//public virtual ~BSplitterNode()	{ delete front; delete back; };

		public float	A, B, C, D;
		public BNode	*front;
		public BNode	*back;

		public override void		Draw ()
		{
			BNode	*child;
		
			Debug.Assert( front );
			Debug.Assert( back );
		
			if (A*TheStateStack.ObjSpaceEye.x + 
				B*TheStateStack.ObjSpaceEye.y + 
				C*TheStateStack.ObjSpaceEye.z + D > 0.0f) {
		
				child = front;
				do
				{
					child.Draw();
					child = child.sibling;
				} while (child);
		
				child = back;
				do
				{
					child.Draw();
					child = child.sibling;
				} while (child);
			}
			else
			{
				child = back;
				do
				{
					child.Draw();
					child = child.sibling;
				} while (child);
		
				child = front;
				do
				{
					child.Draw();
					child = child.sibling;
				} while (child);
			}
		}

		public override BNodeType	Type ()
		{
			return BNodeType.tagBSplitterNode;
		}
	}

	public class BPrimitiveNode:   BNode
	{
 
		// Convert from file offsets back to pointers
		public BPrimitiveNode (byte *baseAddress, BNodeType **tagListPtr)
		:base( baseAddress, tagListPtr )
		{
			// Now fixup our polygon
			prim		= RestorePrimPointers( baseAddress, (int)prim );
		}
	
		public BPrimitiveNode ()
		{
			prim = null;
		}
		//public virtual ~BPrimitiveNode()	{};

		public Prim		*prim;

		public override void		Draw ()
		{
			// Call the appropriate draw function for this primitive
			DrawPrimJumpTable[prim.type]( prim );
		}

		public override BNodeType	Type ()
		{
			return BNodeType.tagBPrimitiveNode;
		}
	}

	public class BLitPrimitiveNode:   BNode
	{
 
		// Convert from file offsets back to pointers
		public BLitPrimitiveNode (byte *baseAddress, BNodeType **tagListPtr)
		:base( baseAddress, tagListPtr )
		{
			// Now fixup our polygons
			poly		= (Poly*)RestorePrimPointers( baseAddress, (int)poly );
			backpoly	= (Poly*)RestorePrimPointers( baseAddress, (int)backpoly );
		}

		public BLitPrimitiveNode ()
		{
			poly = null;
			backpoly = null;
		}
		//public virtual ~BLitPrimitiveNode()	{};

		public Poly		*poly;
		public Poly		*backpoly;

		public override void Draw ()
		{
			// Choose the front facing polygon so that lighting is correct
			if ((poly.A*TheStateStack.ObjSpaceEye.x + poly.B*TheStateStack.ObjSpaceEye.y + poly.C*TheStateStack.ObjSpaceEye.z + poly.D) >= 0) {
				DrawPrimJumpTable[poly.type]( poly );
			} else {
				DrawPrimJumpTable[poly.type]( backpoly );
			}
		}

		public override BNodeType	Type ()
		{
			return BNodeType.tagBLitPrimitiveNode;
		}
	}

	public class BCulledPrimitiveNode:   BNode
	{
 
		// Convert from file offsets back to pointers
		public BCulledPrimitiveNode (byte *baseAddress, BNodeType **tagListPtr)
		:base( baseAddress, tagListPtr )
		{
			// Now fixup our polygon
			poly		= (Poly*)RestorePrimPointers( baseAddress, (int)poly );
		}

		public BCulledPrimitiveNode ()
		{
			poly = null;
		}
		//public virtual~BCulledPrimitiveNode()	{};

		public Poly		*poly;

		public override void Draw ()
		{
			// Only draw front facing polygons
			if ((poly.A*TheStateStack.ObjSpaceEye.x + poly.B*TheStateStack.ObjSpaceEye.y + poly.C*TheStateStack.ObjSpaceEye.z + poly.D) >= 0) {
				// Call the appropriate draw function for this polygon
				DrawPrimJumpTable[poly.type]( poly );
			}
		}

		public override BNodeType	Type ()
		{
			return BNodeType.tagBCulledPrimitiveNode;
		}
	}

	public class BLightStringNode:   BPrimitiveNode
	{
		// Convert from file offsets back to pointers
		public BLightStringNode (byte *baseAddress, BNodeType **tagListPtr)
		:base( baseAddress, tagListPtr )
		{
		}
	
		public BLightStringNode () : base()
		{
			rgbaFront = -1;
			rgbaBack = -1;
			A = B = C = D = 0.0f;
		}
		//public virtual~BLightStringNode()	{};

		// For directional lights
		public float		A, B, C, D;
		public int			rgbaFront;
		public int			rgbaBack;

		public override void		Draw ()
		{
			// Clobber the primitive color with the appropriate front or back color
			if ((A*TheStateStack.ObjSpaceEye.x + B*TheStateStack.ObjSpaceEye.y + C*TheStateStack.ObjSpaceEye.z + D) >= 0) {
				((PrimPointFC*)prim).rgba = rgbaFront;
			} else {
				((PrimPointFC*)prim).rgba = rgbaBack;
			}
		
			// Call the appropriate draw function for this polygon
			DrawPrimJumpTable[prim.type]( prim );
		}

		public override BNodeType	Type ()
		{
			return BNodeType.tagBLightStringNode;
		}
	}
#endif
}



