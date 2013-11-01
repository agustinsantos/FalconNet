using FalconNet.Common.Encoding;
using FalconNet.Common.Graphics;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Pmatrix = FalconNet.Common.Graphics.Trotation;

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
        tagBTransNode,
        tagBScaleNode,
        tagBXDofNode,
        tagBXSwitchNode,
        tagBRenderControlNode,
    }

    public enum BTransformType
    {
        Normal,
        Billboard,
        Tree
    }
    public enum BRenderControlType
    {
        rcNoOp, // no operation
        rcZBias // FArg[0] sets Z-Bias
    }

    /***************************************************************\
        To improve performance, these classes use several global
        variables to store command data instead of passing it
        through the call stack.
        The global variables are maintained by the StackState
        module.
    \***************************************************************/
    public abstract class BNode
    {
#if TODO	
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
#endif

        public BNode()
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
#if TODO
		public static BNode RestorePointers (byte *baseAddress, int offset, BNodeType **tagListPtr)
		{
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
		 
					throw new NotImplementedException();
	}
#endif

        public BNode sibling;

        public abstract void Draw();

        public virtual BNodeType Type()
        {
            return BNodeType.tagBNode;
        }
    }


    // Convert from file offsets back to pointers
    public class BSubTree : BNode
    {
#if TODO
		public BSubTree (byte *baseAddress, BNodeType **tagListPtr)
		:base( baseAddress, tagListPtr )
		{
			// Fixup our dependents
			subTree		= RestorePointers( baseAddress, (int)subTree, tagListPtr );
		
			// Fixup our data pointers
			pCoords		= (Tpoint*)(baseAddress + (int)pCoords);
			pNormals	= (Pnormal*)(baseAddress + (int)pNormals);
		}
#endif
        public BSubTree()
        {
            subTree = null;
        }
        //public virtual ~BSubTree()	{ delete subTree; };

        public Tpoint[] pCoords;
        public int nCoords;
        public int nDynamicCoords;
        public int DynamicCoordOffset;
        public Pnormal[] pNormals;
        public int nNormals;
        public BNode subTree;

        public override void Draw()
        {

            BNode child;

            if (nNormals != 0)
                StateStackClass.Light(pNormals, nNormals);

            if (nDynamicCoords == 0)
            {
                StateStackClass.Transform(pCoords, nCoords);
            }
            else
            {
                StateStackClass.Transform(pCoords, nCoords - nDynamicCoords);
                StateStackClass.Transform(StateStackClass.CurrentInstance.DynamicCoords, nDynamicCoords, DynamicCoordOffset);
            }
            child = subTree;
            //TODO Debug.Assert(false == F4IsBadReadPtr( child, sizeof*child) );
            do
            {
                child.Draw();
                child = child.sibling;
            } while (child != null); // JB 010306 CTD
            //} while (child && !F4IsBadReadPtr(child, sizeof(BNode))); // JB 010306 CTD (too much CPU)
        }


        public override BNodeType Type()
        {
            return BNodeType.tagBSubTree;
        }
    }

    public class BRoot : BSubTree
    {
        // Convert from file offsets back to pointers 
#if TODO
		public BRoot (byte *baseAddress, BNodeType **tagListPtr)
		:base( baseAddress, tagListPtr )
		{
			// Fixup our extra data pointers
			pTexIDs		= (int*)(baseAddress + (int)pTexIDs);
		}
#endif
        public BRoot()
            : base()
        {
            pTexIDs = null;
            nTexIDs = -1;
            ScriptNumber = -1;
        }
        //public virtual ~BRoot()	{};

        public void LoadTextures()
        {
            for (int i = 0; i < nTexIDs; i++)
            {
                // Skip unsed texture indices
                if (pTexIDs[i] >= 0)
                {
                    TextureBankClass.Reference(pTexIDs[i]);
                }
            }
        }


        public void UnloadTextures()
        {
            for (int i = 0; i < nTexIDs; i++)
            {
                if (pTexIDs[i] >= 0)
                {
                    TextureBankClass.Release(pTexIDs[i]);
                }
            }
        }

        public int[] pTexIDs;
        public int nTexIDs;
        public int ScriptNumber;

        public override void Draw()
        {
            // Compute the offset to the first texture in the texture set
            int texOffset = StateStackClass.CurrentInstance.TextureSet *
                (nTexIDs / StateStackClass.CurrentInstance.ParentObject.nTextureSets);
            StateStackClass.SetTextureTable(pTexIDs); //TODO +texOffset]);

            if (ScriptNumber > 0)
            {
                Debug.Assert(ScriptNumber < Scripts.ScriptArrayLength);
                if (ScriptNumber < Scripts.ScriptArrayLength)
                {
                    Scripts.ScriptArray[ScriptNumber]();
                }
            }

            base.Draw();
        }

        public override BNodeType Type()
        {
            return BNodeType.tagBRoot;
        }
    }


    public class BSpecialXform : BNode
    {
        // Convert from file offsets back to pointers
#if TODO
		public BSpecialXform (byte *baseAddress, BNodeType **tagListPtr)
		:base( baseAddress, tagListPtr )
		{
			// Fixup our dependents
			subTree		= RestorePointers( baseAddress, (int)subTree, tagListPtr );
		
			// Fixup our data pointers
			pCoords		= (Tpoint*)(baseAddress + (int)pCoords);
		}
#endif
        public BSpecialXform()
        {
            subTree = null;
        }
        // public virtual ~BSpecialXform()	{ delete subTree; };

        public Tpoint[] pCoords;
        public int nCoords;
        public BTransformType type;
        public BNode subTree;

        public override void Draw()
        {
            Debug.Assert(subTree != null);

            StateStackClass.PushVerts();
            StateStackClass.TransformBillboardWithClip(pCoords, nCoords, type);
            subTree.Draw();
            StateStackClass.PopVerts();
        }

        public override BNodeType Type()
        {
            return BNodeType.tagBSpecialXform;
        }
    }


    public class BSlotNode : BNode
    {

        // Convert from file offsets back to pointers
#if TODO
		public BSlotNode (byte *baseAddress, BNodeType **tagListPtr)
		:base( baseAddress, tagListPtr )
		{
		}
#endif
        public BSlotNode()
        {
            slotNumber = -1;
        }
        // public virtual ~BSlotNode()	{};

        public Pmatrix rotation;
        public Tpoint origin;
        public int slotNumber;

        public override void Draw()
        {
            Debug.Assert(slotNumber < StateStackClass.CurrentInstance.ParentObject.nSlots);
            if (slotNumber >= StateStackClass.CurrentInstance.ParentObject.nSlots)
                return; // JPO fix
            ObjectInstance subObject = StateStackClass.CurrentInstance.SlotChildren[slotNumber];

            if (subObject != null)
            {
                StateStackClass.DrawSubObject(subObject, rotation, origin);
            }
        }

        public override BNodeType Type()
        {
            return BNodeType.tagBSlotNode;
        }
    }


    public class BDofNode : BSubTree
    {
        // Convert from file offsets back to pointers 
#if TODO
		public BDofNode (byte *baseAddress, BNodeType **tagListPtr)
		:base( baseAddress, tagListPtr )
		{
		}
#endif
        public BDofNode()
            : base()
        {
            dofNumber = -1;
        }
        //public virtual ~BDofNode()		{};

        public int dofNumber;
        public Pmatrix rotation;
        public Tpoint translation;

        public override void Draw()
        {
            Pmatrix dofRot;
            Pmatrix R = new Pmatrix();
            Tpoint T = new Tpoint();
            float trig;

            Debug.Assert(dofNumber < StateStackClass.CurrentInstance.ParentObject.nDOFs);
            if (dofNumber >= StateStackClass.CurrentInstance.ParentObject.nDOFs)
                return;
            // Set up our free rotation
            trig = StateStackClass.CurrentInstance.DOFValues[dofNumber].rotation;
            dofRot.M11 = 1.0f; dofRot.M12 = 0.0f; dofRot.M13 = 0.0f;
            dofRot.M21 = 0.0f; dofRot.M22 = (float)Math.Cos(trig); dofRot.M23 = (float)Math.Sin(-trig);
            dofRot.M31 = 0.0f; dofRot.M32 = (float)Math.Sin(trig); dofRot.M33 = (float)Math.Cos(trig);


            // Now compose this with the rotation into our parents coordinate system
            Matrix.MatrixMult(rotation, dofRot, ref R);

            // Now do our free translation
            // SCR 10/28/98:  THIS IS WRONG FOR TRANSLATION DOFs.  "DOFValues" is supposed to 
            // translate along the local x axis, but this will translate along the parent's x axis.
            // To fix this would require a bit more math (and/or thought).  Since it
            // only happens once in Falcon, I'll leave it broken and put a workaround into
            // the KC10 object so that the parent's and child's x axis are forced into alignment
            // by inserting an extra dummy DOF bead.
            T.x = translation.x + StateStackClass.CurrentInstance.DOFValues[dofNumber].translation;
            T.y = translation.y;
            T.z = translation.z;

            // Draw our subtree
            StateStackClass.PushAll();
            StateStackClass.CompoundTransform(R, T);
            base.Draw();
            StateStackClass.PopAll();
        }

        public override BNodeType Type()
        {
            return BNodeType.tagBDofNode;
        }
    }

    public class BXDofNode : BSubTree
    {
#if TODO
	public BXDofNode( BYTE *baseAddress, BNodeType **tagListPtr );
#endif
        public BXDofNode() : base() { dofNumber = -1; }
        //public virtual ~BXDofNode()		{};

        public int dofNumber;
        public float min, max, multiplier, future;
        public int flags;
        public Pmatrix rotation;
        public Tpoint translation;

        public override void Draw()
        {
            throw new NotImplementedException();
        }
        public override BNodeType Type()
        {
            return BNodeType.tagBDofNode;
        }
    }

    // MLR 2003-10-06 translator node
    public class BTransNode : BSubTree
    {
#if TODO
	public BTransNode( BYTE *baseAddress, BNodeType **tagListPtr )
#endif
        public BTransNode() : base() { dofNumber = -1; }
        //public virtual ~BTransNode()		{};

        public int dofNumber;
        public float min, max, multiplier, future;
        public int flags;
        public Tpoint translation;

        public override void Draw()
        {
            throw new NotImplementedException();
        }
        public override BNodeType Type() { return BNodeType.tagBTransNode; }
    }

    // MLR 2003-10-10 translator node

    public class BScaleNode : BSubTree
    {
#if TODO
    public BScaleNode( BYTE *baseAddress, BNodeType **tagListPtr )
#endif
        public BScaleNode() : base() { dofNumber = -1; }
        //public virtual ~BScaleNode()		{}

        public int dofNumber;
        public float min, max, multiplier, future;
        public int flags;
        public Tpoint scale;
        public Tpoint translation;


        public override void Draw()
        {
            throw new NotImplementedException();
        }
        public override BNodeType Type() { return BNodeType.tagBTransNode; }
    }


    public class BSwitchNode : BNode
    {

        // Convert from file offsets back to pointers
#if TODO
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
#endif
        public BSwitchNode()
        {
            subTrees = null;
            numChildren = 0;
            switchNumber = -1;
        }
        //public virtual ~BSwitchNode()	{ while (numChildren--) delete subTrees[numChildren]; };

        public int switchNumber;
        public int numChildren;
        public BSubTree[] subTrees;

        public override void Draw()
        {
            UInt32 mask;
            int i = 0;

            Debug.Assert(switchNumber < StateStackClass.CurrentInstance.ParentObject.nSwitches);
            if (switchNumber >= StateStackClass.CurrentInstance.ParentObject.nSwitches)
                return;
            mask = StateStackClass.CurrentInstance.SwitchValues[switchNumber];

#if NOTHING	// This will generally be faster due to early termination
			// Go until all ON switch children have been drawn.
			while (mask) {
#else	// This will work even if the mask is set for non-existent children
            // Go until all children have been considered for drawing.
            while (i < numChildren)
            {
#endif
                Debug.Assert(subTrees[i] != null);

                // Only draw this subtree if the corresponding switch bit is set
                if ((mask & 1) != 0)
                {
                    StateStackClass.PushVerts();
                    subTrees[i].Draw();
                    StateStackClass.PopVerts();
                }
                mask >>= 1;
                i++;
            }
        }

        public override BNodeType Type()
        {
            return BNodeType.tagBSwitchNode;
        }
    }

    public class BXSwitchNode : BNode
    {
#if TODO
	BXSwitchNode( BYTE *baseAddress, BNodeType **tagListPtr );
#endif
        public BXSwitchNode()
        { subTrees = null; numChildren = 0; switchNumber = -1; }
        //virtual ~BXSwitchNode()	{ while (numChildren--) delete subTrees[numChildren]; };

        public int switchNumber;
        public int flags;
        public int numChildren;
        public BSubTree[] subTrees;

        public override void Draw()
        {
            throw new NotImplementedException();
        }
        public override BNodeType Type()
        {
            return BNodeType.tagBSwitchNode;
        }
    }


    public class BSplitterNode : BNode
    {

        // Convert from file offsets back to pointers
#if TODO
		public BSplitterNode (byte *baseAddress, BNodeType **tagListPtr)
		:base( baseAddress, tagListPtr )
		{
			// Fixup our dependents
			front	= RestorePointers( baseAddress, (int)front, tagListPtr );
			back	= RestorePointers( baseAddress, (int)back,  tagListPtr );
		}
#endif
        public BSplitterNode()
        {
            front = back = null;
        }
        //public virtual ~BSplitterNode()	{ delete front; delete back; };

        public float A, B, C, D;
        public BNode front;
        public BNode back;

        public override void Draw()
        {
            BNode child;

            Debug.Assert(front != null);
            Debug.Assert(back != null);

            if (A * StateStackClass.ObjSpaceEye.x +
                B * StateStackClass.ObjSpaceEye.y +
                C * StateStackClass.ObjSpaceEye.z + D > 0.0f)
            {

                child = front;
                do
                {
                    child.Draw();
                    child = child.sibling;
                } while (child != null);

                child = back;
                do
                {
                    child.Draw();
                    child = child.sibling;
                } while (child != null);
            }
            else
            {
                child = back;
                do
                {
                    child.Draw();
                    child = child.sibling;
                } while (child != null);

                child = front;
                do
                {
                    child.Draw();
                    child = child.sibling;
                } while (child != null);
            }
        }

        public override BNodeType Type()
        {
            return BNodeType.tagBSplitterNode;
        }
    }

    public class BPrimitiveNode : BNode
    {

        // Convert from file offsets back to pointers
#if TODO
		public BPrimitiveNode (byte *baseAddress, BNodeType **tagListPtr)
		:base( baseAddress, tagListPtr )
		{
			// Now fixup our polygon
			prim		= RestorePrimPointers( baseAddress, (int)prim );
		}
#endif
        public BPrimitiveNode()
        {
            prim = null;
        }
        //public virtual ~BPrimitiveNode()	{};

        public Prim prim;

        public override void Draw()
        {
            // Call the appropriate draw function for this primitive
            PolyLib.DrawPrimJumpTable[(int)prim.type](prim);
        }

        public override BNodeType Type()
        {
            return BNodeType.tagBPrimitiveNode;
        }
    }

    public class BLitPrimitiveNode : BNode
    {

        // Convert from file offsets back to pointers
#if TODO
		public BLitPrimitiveNode (byte *baseAddress, BNodeType **tagListPtr)
		:base( baseAddress, tagListPtr )
		{
			// Now fixup our polygons
			poly		= (Poly*)RestorePrimPointers( baseAddress, (int)poly );
			backpoly	= (Poly*)RestorePrimPointers( baseAddress, (int)backpoly );
		}
#endif
        public BLitPrimitiveNode()
        {
            poly = null;
            backpoly = null;
        }
        //public virtual ~BLitPrimitiveNode()	{};

        public Poly poly;
        public Poly backpoly;

        public override void Draw()
        {
            // Choose the front facing polygon so that lighting is correct
            if ((poly.A * StateStackClass.ObjSpaceEye.x + poly.B * StateStackClass.ObjSpaceEye.y + poly.C * StateStackClass.ObjSpaceEye.z + poly.D) >= 0)
            {
                PolyLib.DrawPrimJumpTable[(int)poly.type](poly);
            }
            else
            {
                PolyLib.DrawPrimJumpTable[(int)poly.type](backpoly);
            }
        }

        public override BNodeType Type()
        {
            return BNodeType.tagBLitPrimitiveNode;
        }
    }

    public class BCulledPrimitiveNode : BNode
    {

        // Convert from file offsets back to pointers
#if TODO
		public BCulledPrimitiveNode (byte *baseAddress, BNodeType **tagListPtr)
		:base( baseAddress, tagListPtr )
		{
			// Now fixup our polygon
			poly		= (Poly*)RestorePrimPointers( baseAddress, (int)poly );
		}
#endif
        public BCulledPrimitiveNode()
        {
            poly = null;
        }
        //public virtual~BCulledPrimitiveNode()	{};

        public Poly poly;

        public override void Draw()
        {
            // Only draw front facing polygons
            if ((poly.A * StateStackClass.ObjSpaceEye.x + poly.B * StateStackClass.ObjSpaceEye.y + poly.C * StateStackClass.ObjSpaceEye.z + poly.D) >= 0)
            {
                // Call the appropriate draw function for this polygon
                PolyLib.DrawPrimJumpTable[(int)poly.type](poly);
            }
        }

        public override BNodeType Type()
        {
            return BNodeType.tagBCulledPrimitiveNode;
        }
    }

    public class BLightStringNode : BPrimitiveNode
    {
        // Convert from file offsets back to pointers
#if TODO
		public BLightStringNode (byte *baseAddress, BNodeType **tagListPtr)
		:base( baseAddress, tagListPtr )
		{
		}
#endif
        public BLightStringNode()
            : base()
        {
            rgbaFront = -1;
            rgbaBack = -1;
            A = B = C = D = 0.0f;
        }
        //public virtual~BLightStringNode()	{};

        // For directional lights
        public float A, B, C, D;
        public int rgbaFront;
        public int rgbaBack;

        public override void Draw()
        {
            // Clobber the primitive color with the appropriate front or back color
            if ((A * StateStackClass.ObjSpaceEye.x + B * StateStackClass.ObjSpaceEye.y + C * StateStackClass.ObjSpaceEye.z + D) >= 0)
            {
                ((PrimPointFC)prim).rgba = rgbaFront;
            }
            else
            {
                ((PrimPointFC)prim).rgba = rgbaBack;
            }

            // Call the appropriate draw function for this polygon
            PolyLib.DrawPrimJumpTable[(int)prim.type](prim);
        }

        public override BNodeType Type()
        {
            return BNodeType.tagBLightStringNode;
        }
    }

    public class BRenderControlNode : BNode
    {
#if TODO
	public BRenderControlNode( BYTE *baseAddress, BNodeType **tagListPtr );
#endif
        public BRenderControlNode() { Control = BRenderControlType.rcNoOp; }
        //TODO public virtual ~BRenderControlNode()	{};

        public BRenderControlType Control;
        public int[] IArg = new int[4];
        public float[] FArg = new float[4];

        public override void Draw()
        {
            throw new NotImplementedException();
        }
        public override BNodeType Type()
        {
            return BNodeType.tagBRenderControlNode;
        }
    }


    #region Encoding
    public class TagsList
    {
        public BNodeType[] tagList;
        public int tagIndex;
    }
    public class BNodeOffsets
    {
        public BNodeOffsets(long basepos)
        {
            this.baseposition = basepos;
        }
        public long baseposition;
        public int sibling;
    }

    public class BSubTreeOffsets : BNodeOffsets
    {
        public BSubTreeOffsets(long basepos) : base(basepos) { }
        public int pCoords;
        public int pNormals;
        public int subTree;
    }

    public class BRootOffsets : BSubTreeOffsets
    {
        public BRootOffsets(long basepos) : base(basepos) { }
        public int pTexIDs;
    }

    public class BSpecialXformOffsets : BNodeOffsets
    {
        public BSpecialXformOffsets(long basepos) : base(basepos) { }
        public int pCoords;
        public int subTree;
    }

    public class BSlotNodeOffsets : BNodeOffsets
    {
        public BSlotNodeOffsets(long basepos) : base(basepos) { }
    }

    public class BDofNodeOffsets : BSubTreeOffsets
    {
        public BDofNodeOffsets(long basepos) : base(basepos) { }
    }

    public class BXDofNodeOffsets : BSubTreeOffsets
    {
        public BXDofNodeOffsets(long basepos) : base(basepos) { }
    }

    public class BTransNodeOffsets : BSubTreeOffsets
    {
        public BTransNodeOffsets(long basepos) : base(basepos) { }
    }

    public class BScaleNodeOffsets : BSubTreeOffsets
    {
        public BScaleNodeOffsets(long basepos) : base(basepos) { }
    }

    public class BSwitchNodeOffsets : BNodeOffsets
    {
        public BSwitchNodeOffsets(long basepos) : base(basepos) { }
        public int subTreesPos;
        public int[] subTrees;
    }

    public class BXSwitchNodeOffsets : BNodeOffsets
    {
        public BXSwitchNodeOffsets(long basepos) : base(basepos) { }
        public int subTreesPos;
        public int[] subTrees;
    }

    public class BSplitterNodeOffsets : BNodeOffsets
    {
        public BSplitterNodeOffsets(long basepos) : base(basepos) { }
        public int front;
        public int back;
    }

    public class BPrimitiveNodeOffsets : BNodeOffsets
    {
        public BPrimitiveNodeOffsets(long basepos) : base(basepos) { }
        public int prim;
    }

    public class BLitPrimitiveNodeOffsets : BNodeOffsets
    {
        public BLitPrimitiveNodeOffsets(long basepos) : base(basepos) { }
        public int poly;
        public int backpoly;
    }

    public class BCulledPrimitiveNodeOffsets : BNodeOffsets
    {
        public BCulledPrimitiveNodeOffsets(long basepos) : base(basepos) { }
        public int poly;
    }

    public class BLightStringNodeOffsets : BPrimitiveNodeOffsets
    {
        public BLightStringNodeOffsets(long basepos) : base(basepos) { }
    }

    public class BRenderControlNodeOffsets : BNodeOffsets
    {
        public BRenderControlNodeOffsets(long basepos) : base(basepos) { }
    }

    public static class BNodeEncodingLE
    {
        public static void Encode(Stream stream, BNode val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, BNode rst, TagsList tagsList, BNodeOffsets rstOffsets)
        {
            UInt32EncodingLE.Decode(stream); // unused data, corresponds to virtual function ??
            rstOffsets.sibling = Int32EncodingLE.Decode(stream);
            RestorePointers(stream, rst, tagsList, rstOffsets);
        }

        private static void RestorePointers(Stream stream, BNode rst, TagsList tagsList, BNodeOffsets rstOffsets)
        {
            long pos = stream.Position;
            //Restore Pointers
            // Fixup our sibling, if any
            if (rstOffsets.sibling > 0)
            {
                rst.sibling = ProcessNode(stream, rstOffsets.baseposition, rstOffsets.sibling, tagsList);
            }
            else
                rst.sibling = null;
            stream.Position = pos;
        }

        public static BNode Decode(Stream stream)
        {
            int TagCount = Int32EncodingLE.Decode(stream);
            BNodeType[] TagList = new BNodeType[TagCount];
            for (int j = 0; j < TagCount; j++)
            {
                TagList[j] = (BNodeType)Int32EncodingLE.Decode(stream);
            }
            TagsList tagsList = new TagsList {tagList = TagList , tagIndex = 0};
            return ProcessNode(stream, stream.Position, 0, tagsList);
        }

        public static BNode ProcessNode(Stream stream, long baseposition, int offset, TagsList tagsList)
        {
            if (offset < 0)
            {
                log.Debug(" Index is negative;");
                return null;
            }
            if (tagsList.tagIndex == 0)
                log.Debug(">> Index == 0");
            log.DebugFormat("Decoding Node: offset={0}, tagindex={1}, tag={2}", offset, tagsList.tagIndex, (int)tagsList.tagList[tagsList.tagIndex]);
            BNode rst = null;
            BNodeOffsets rstOffsets = null;
            stream.Position = baseposition + offset;

            // Apply the proper virtual table setup and constructor
            switch (tagsList.tagList[tagsList.tagIndex++])
            {
                case BNodeType.tagBSubTree:
                    rst = new BSubTree();
                    rstOffsets = new BSubTreeOffsets(baseposition);
                    BSubTreeEncodingLE.Decode(stream, (BSubTree)rst, tagsList, (BSubTreeOffsets)rstOffsets);
                    break;
                case BNodeType.tagBRoot:
                    rst = new BRoot();
                    rstOffsets = new BRootOffsets(baseposition);
                    BRootEncodingLE.Decode(stream, (BRoot)rst, tagsList, (BRootOffsets)rstOffsets);
                    break;
                case BNodeType.tagBSpecialXform:
                    rst = new BSpecialXform();
                    rstOffsets = new BSpecialXformOffsets(baseposition);
                    BSpecialXformEncodingLE.Decode(stream, (BSpecialXform)rst, tagsList, (BSpecialXformOffsets)rstOffsets);
                    break;
                case BNodeType.tagBSlotNode:
                    rst = new BSlotNode();
                    rstOffsets = new BSlotNodeOffsets(baseposition);
                    BSlotNodeEncodingLE.Decode(stream, (BSlotNode)rst, tagsList, (BSlotNodeOffsets)rstOffsets);
                    break;
                case BNodeType.tagBDofNode:
                    rst = new BDofNode();
                    rstOffsets = new BDofNodeOffsets(baseposition);
                    BDofNodeEncodingLE.Decode(stream, (BDofNode)rst, tagsList, (BDofNodeOffsets)rstOffsets);
                    break;
                case BNodeType.tagBSwitchNode:
                    rst = new BSwitchNode();
                    rstOffsets = new BSwitchNodeOffsets(baseposition);
                    BSwitchNodeEncodingLE.Decode(stream, (BSwitchNode)rst, tagsList, (BSwitchNodeOffsets)rstOffsets);
                    break;
                case BNodeType.tagBSplitterNode:
                    rst = new BSplitterNode();
                    rstOffsets = new BSplitterNodeOffsets(baseposition);
                    BSplitterNodeEncodingLE.Decode(stream, (BSplitterNode)rst, tagsList, (BSplitterNodeOffsets)rstOffsets);
                    break;
                case BNodeType.tagBPrimitiveNode:
                    rst = new BPrimitiveNode();
                    rstOffsets = new BPrimitiveNodeOffsets(baseposition);
                    BPrimitiveNodeEncodingLE.Decode(stream, (BPrimitiveNode)rst, tagsList, (BPrimitiveNodeOffsets)rstOffsets);
                    break;
                case BNodeType.tagBLitPrimitiveNode:
                    rst = new BLitPrimitiveNode();
                    rstOffsets = new BLitPrimitiveNodeOffsets(baseposition);
                    BLitPrimitiveNodeEncodingLE.Decode(stream, (BLitPrimitiveNode)rst, tagsList, (BLitPrimitiveNodeOffsets)rstOffsets);
                    break;
                case BNodeType.tagBCulledPrimitiveNode:
                    rst = new BCulledPrimitiveNode();
                    rstOffsets = new BCulledPrimitiveNodeOffsets(baseposition);
                    BCulledPrimitiveNodeEncodingLE.Decode(stream, (BCulledPrimitiveNode)rst, tagsList, (BCulledPrimitiveNodeOffsets)rstOffsets);
                    break;
                case BNodeType.tagBLightStringNode:
                    rst = new BLightStringNode();
                    rstOffsets = new BLightStringNodeOffsets(baseposition);
                    BLightStringNodeEncodingLE.Decode(stream, (BLightStringNode)rst, tagsList, (BLightStringNodeOffsets)rstOffsets);
                    break;
                case BNodeType.tagBTransNode:
                    rst = new BTransNode();
                    rstOffsets = new BTransNodeOffsets(baseposition);
                    BTransNodeEncodingLE.Decode(stream, (BTransNode)rst, tagsList, (BTransNodeOffsets)rstOffsets);
                    break;
                case BNodeType.tagBScaleNode:
                    rst = new BScaleNode();
                    rstOffsets = new BScaleNodeOffsets(baseposition);
                    BScaleNodeEncodingLE.Decode(stream, (BScaleNode)rst, tagsList, (BScaleNodeOffsets)rstOffsets);
                    break;
                case BNodeType.tagBXDofNode:
                    rst = new BXDofNode();
                    rstOffsets = new BXDofNodeOffsets(baseposition);
                    BXDofNodeEncodingLE.Decode(stream, (BXDofNode)rst, tagsList, (BXDofNodeOffsets)rstOffsets);
                    break;
                case BNodeType.tagBXSwitchNode:
                    rst = new BXSwitchNode();
                    rstOffsets = new BXSwitchNodeOffsets(baseposition);
                    BXSwitchNodeEncodingLE.Decode(stream, (BXSwitchNode)rst, tagsList, (BXSwitchNodeOffsets)rstOffsets);
                    break;
                case BNodeType.tagBRenderControlNode:
                    rst = new BRenderControlNode();
                    rstOffsets = new BRenderControlNodeOffsets(baseposition);
                    BRenderControlNodeEncodingLE.Decode(stream, (BRenderControlNode)rst, tagsList, (BRenderControlNodeOffsets)rstOffsets);
                    break;

                default:
                    log.Error("Decoding unrecognized BSP node type.");
                    break;
            }
            stream.Position = baseposition;
            return rst;
        }
        public static int Size
        {
            get { throw new NotImplementedException(); }
        }

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    }
    public static class BSubTreeEncodingLE
    {
        public static void Encode(Stream stream, BSubTree val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, BSubTree rst, TagsList tagsList, BSubTreeOffsets rstOffsets)
        {
            BNodeEncodingLE.Decode(stream, rst, tagsList, rstOffsets);
            rstOffsets.pCoords = Int32EncodingLE.Decode(stream);
            rst.nCoords = Int32EncodingLE.Decode(stream);
            rst.nDynamicCoords = Int32EncodingLE.Decode(stream);
            rst.DynamicCoordOffset = Int32EncodingLE.Decode(stream);
            rstOffsets.pNormals = Int32EncodingLE.Decode(stream);
            rst.nNormals = Int32EncodingLE.Decode(stream);
            rstOffsets.subTree = Int32EncodingLE.Decode(stream);
            RestorePointers(stream, rst, tagsList, rstOffsets);
        }
        private static void RestorePointers(Stream stream, BSubTree rst, TagsList tagsList, BSubTreeOffsets rstOffsets)
        {
            long pos = stream.Position;

            // Fixup our dependents
            rst.subTree = BNodeEncodingLE.ProcessNode(stream, rstOffsets.baseposition, rstOffsets.subTree, tagsList);

            // Fixup our data pointers
            if (rst.nCoords > 0)
            {
                stream.Position = rstOffsets.baseposition + rstOffsets.pCoords;
                rst.pCoords = new Tpoint[rst.nCoords];
                for (int i = 0; i < rst.nCoords; i++)
                    TpointEncodingLE.Decode(stream, ref rst.pCoords[i]);
                //rst.pCoords = (Ppoint*)(baseAddress + (int)pCoords);
            }
            if (rst.nNormals > 0)
            {
                stream.Position = rstOffsets.baseposition + rstOffsets.pNormals;
                rst.pNormals = new Pnormal[rst.nNormals];
                for (int i = 0; i < rst.nNormals; i++)
                    PnormalEncodingLE.Decode(stream, ref rst.pNormals[i]);
                //rst.pNormals = (Pnormal*)(baseAddress + (int)pNormals);
            }
            stream.Position = pos;
        }
        public static int Size
        {
            get { throw new NotImplementedException(); }
        }
    }

    public static class BRootEncodingLE
    {
        public static void Encode(Stream stream, BRoot val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, BRoot rst, TagsList tagsList, BRootOffsets rstOffsets)
        {
            BSubTreeEncodingLE.Decode(stream, rst, tagsList, rstOffsets);
            rstOffsets.pTexIDs = Int32EncodingLE.Decode(stream);
            rst.nTexIDs = Int32EncodingLE.Decode(stream);
            rst.ScriptNumber = Int32EncodingLE.Decode(stream);
            RestorePointers(stream, rst, tagsList, rstOffsets);
        }
        private static void RestorePointers(Stream stream, BRoot rst, TagsList tagsList, BRootOffsets rstOffsets)
        {
            long pos = stream.Position;

            //Restore Pointers
            if (rst.nTexIDs > 0)
            {
                stream.Position = rstOffsets.baseposition + rstOffsets.pTexIDs;
                rst.pTexIDs = new int[rst.nTexIDs];
                for (int i = 0; i < rst.nTexIDs; i++)
                    rst.pTexIDs[i] = Int32EncodingLE.Decode(stream);
                //pTexIDs		= (int*)(baseAddress + (int)pTexIDs);
            }
            stream.Position = pos;
        }
        public static int Size
        {
            get { throw new NotImplementedException(); }
        }
    }

    public static class BSpecialXformEncodingLE
    {
        public static void Encode(Stream stream, BSpecialXform val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, BSpecialXform rst, TagsList tagsList, BSpecialXformOffsets rstOffsets)
        {
            BNodeEncodingLE.Decode(stream, rst, tagsList, rstOffsets);
            rstOffsets.pCoords = Int32EncodingLE.Decode(stream);
            rst.nCoords = Int32EncodingLE.Decode(stream);
            rst.type = (BTransformType)Int32EncodingLE.Decode(stream);
            rstOffsets.subTree = Int32EncodingLE.Decode(stream);
            RestorePointers(stream, rst, tagsList, rstOffsets);
        }
        private static void RestorePointers(Stream stream, BSpecialXform rst, TagsList tagsList, BSpecialXformOffsets rstOffsets)
        {
            long pos = stream.Position;
            // Fixup our dependents
            rst.subTree = BNodeEncodingLE.ProcessNode(stream, rstOffsets.baseposition, rstOffsets.subTree, tagsList);

            // Fixup our data pointers
            if (rst.nCoords > 0)
            {
                stream.Position = rstOffsets.baseposition + rstOffsets.pCoords;
                rst.pCoords = new Tpoint[rst.nCoords];
                for (int i = 0; i < rst.nCoords; i++)
                    TpointEncodingLE.Decode(stream, ref rst.pCoords[i]);
            }
            stream.Position = pos;
        }
        public static int Size
        {
            get { throw new NotImplementedException(); }
        }
    }

    public static class BSlotNodeEncodingLE
    {
        public static void Encode(Stream stream, BSlotNode val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, BSlotNode rst, TagsList tagsList, BSlotNodeOffsets rstOffsets)
        {
            BNodeEncodingLE.Decode(stream, rst, tagsList, rstOffsets);
            TrotationEncodingLE.Decode(stream, ref rst.rotation);
            TpointEncodingLE.Decode(stream, ref rst.origin);
            rst.slotNumber = Int32EncodingLE.Decode(stream);
            RestorePointers(stream, rst, tagsList, rstOffsets);
        }
        private static void RestorePointers(Stream stream, BSlotNode rst, TagsList tagsList, BSlotNodeOffsets rstOffsets)
        {
        }
        public static int Size
        {
            get { throw new NotImplementedException(); }
        }
    }

    public static class BDofNodeEncodingLE
    {
        public static void Encode(Stream stream, BDofNode val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, BDofNode rst, TagsList tagsList, BDofNodeOffsets rstOffsets)
        {
            BSubTreeEncodingLE.Decode(stream, rst, tagsList, rstOffsets);
            rst.dofNumber = Int32EncodingLE.Decode(stream);
            TrotationEncodingLE.Decode(stream, ref rst.rotation); 
            TpointEncodingLE.Decode(stream, ref rst.translation);
            RestorePointers(stream, rst, tagsList, rstOffsets);
        }
        private static void RestorePointers(Stream stream, BDofNode rst, TagsList tagsList, BDofNodeOffsets rstOffsets)
        {
        }
        public static int Size
        {
            get { throw new NotImplementedException(); }
        }
    }

    public static class BSwitchNodeEncodingLE
    {
        public static void Encode(Stream stream, BSwitchNode val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, BSwitchNode rst, TagsList tagsList, BSwitchNodeOffsets rstOffsets)
        {
            BNodeEncodingLE.Decode(stream, rst, tagsList, rstOffsets);
            rst.switchNumber = Int32EncodingLE.Decode(stream);
            rst.numChildren = Int32EncodingLE.Decode(stream);
            rstOffsets.subTreesPos = Int32EncodingLE.Decode(stream);
            RestorePointers(stream, rst, tagsList, rstOffsets);
        }
        private static void RestorePointers(Stream stream, BSwitchNode rst, TagsList tagsList, BSwitchNodeOffsets rstOffsets)
        {
            long pos = stream.Position;
            // Fixup our table of children 
            stream.Position = rstOffsets.baseposition + rstOffsets.subTreesPos;
            rstOffsets.subTrees = new int[rst.numChildren];
            for (int i = 0; i < rst.numChildren; i++)
                rstOffsets.subTrees[i] = Int32EncodingLE.Decode(stream);

            // Now fixup each child tree
            if (rst.numChildren > 0)
                rst.subTrees = new BSubTree[rst.numChildren];
            for (int i = 0; i < rst.numChildren; i++)
                rst.subTrees[i] = (BSubTree)BNodeEncodingLE.ProcessNode(stream, rstOffsets.baseposition, rstOffsets.subTrees[i], tagsList);
            stream.Position = pos;
        }
        public static int Size
        {
            get { throw new NotImplementedException(); }
        }
    }
    public static class BXDofNodeEncodingLE
    {
        public static void Encode(Stream stream, BXDofNode val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, BXDofNode rst, TagsList tagsList, BXDofNodeOffsets rstOffsets)
        {
            BSubTreeEncodingLE.Decode(stream, rst, tagsList, rstOffsets);
            rst.dofNumber = Int32EncodingLE.Decode(stream);
            rst.min = SingleEncodingLE.Decode(stream);
            rst.max = SingleEncodingLE.Decode(stream);
            rst.multiplier = SingleEncodingLE.Decode(stream);
            rst.future = SingleEncodingLE.Decode(stream);
            TrotationEncodingLE.Decode(stream, ref rst.rotation);
            TpointEncodingLE.Decode(stream, ref rst.translation);
            RestorePointers(stream, rst, tagsList, rstOffsets);
        }
        const int XDOF_ISDOF   = (1<<31);
        private static void RestorePointers(Stream stream, BXDofNode rst, TagsList tagsList, BXDofNodeOffsets rstOffsets)
        {
            rst.flags |= XDOF_ISDOF;
        }
        public static int Size
        {
            get { throw new NotImplementedException(); }
        }
    }
    public static class BTransNodeEncodingLE
    {
        public static void Encode(Stream stream, BTransNode val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, BTransNode rst, TagsList tagsList, BTransNodeOffsets rstOffsets)
        {
            BSubTreeEncodingLE.Decode(stream, rst, tagsList, rstOffsets);
            rst.dofNumber = Int32EncodingLE.Decode(stream);
            rst.min = SingleEncodingLE.Decode(stream);
            rst.max = SingleEncodingLE.Decode(stream);
            rst.multiplier = SingleEncodingLE.Decode(stream);
            rst.future = SingleEncodingLE.Decode(stream);
            TpointEncodingLE.Decode(stream, ref rst.translation);
            RestorePointers(stream, rst, tagsList, rstOffsets);
        }
        private static void RestorePointers(Stream stream, BTransNode rst, TagsList tagsList, BTransNodeOffsets rstOffsets)
        {
        }
        public static int Size
        {
            get { throw new NotImplementedException(); }
        }
    }
    public static class BScaleNodeEncodingLE
    {
        public static void Encode(Stream stream, BScaleNode val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, BScaleNode rst, TagsList tagsList, BScaleNodeOffsets rstOffsets)
        {
            BSubTreeEncodingLE.Decode(stream, rst, tagsList, rstOffsets);
            rst.dofNumber = Int32EncodingLE.Decode(stream);
            rst.min = SingleEncodingLE.Decode(stream);
            rst.max = SingleEncodingLE.Decode(stream);
            rst.multiplier = SingleEncodingLE.Decode(stream);
            rst.future = SingleEncodingLE.Decode(stream);
            rst.flags = Int32EncodingLE.Decode(stream);
            TpointEncodingLE.Decode(stream, ref rst.scale);
            TpointEncodingLE.Decode(stream, ref rst.translation);
            RestorePointers(stream, rst, tagsList, rstOffsets);
        }
        private static void RestorePointers(Stream stream, BScaleNode rst, TagsList tagsList, BScaleNodeOffsets rstOffsets)
        {
        }
        public static int Size
        {
            get { throw new NotImplementedException(); }
        }
    }
    public static class BXSwitchNodeEncodingLE
    {
        public static void Encode(Stream stream, BXSwitchNode val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, BXSwitchNode rst, TagsList tagsList, BXSwitchNodeOffsets rstOffsets)
        {
            BNodeEncodingLE.Decode(stream, rst, tagsList, rstOffsets);
            rst.switchNumber = Int32EncodingLE.Decode(stream);
            rst.flags = Int32EncodingLE.Decode(stream);
            rst.numChildren = Int32EncodingLE.Decode(stream);
            rstOffsets.subTreesPos = Int32EncodingLE.Decode(stream);
            RestorePointers(stream, rst, tagsList, rstOffsets);
        }
        private static void RestorePointers(Stream stream, BXSwitchNode rst, TagsList tagsList, BXSwitchNodeOffsets rstOffsets)
        {
            long pos = stream.Position;
            // Fixup our table of children
            stream.Position = rstOffsets.baseposition + rstOffsets.subTreesPos;
            rstOffsets.subTrees = new int[rst.numChildren];
            for (int i = 0; i < rst.numChildren; i++)
                rstOffsets.subTrees[i] = Int32EncodingLE.Decode(stream);

            // Now fixup each child tree
            if (rst.numChildren > 0)
                rst.subTrees = new BSubTree[rst.numChildren];
            for (int i = 0; i < rst.numChildren; i++)
                rst.subTrees[i] = (BSubTree)BNodeEncodingLE.ProcessNode(stream, rstOffsets.baseposition, rstOffsets.subTrees[i], tagsList);
            stream.Position = pos;
        }
        public static int Size
        {
            get { throw new NotImplementedException(); }
        }
    }
    public static class BSplitterNodeEncodingLE
    {
        public static void Encode(Stream stream, BSplitterNode val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, BSplitterNode rst, TagsList tagsList, BSplitterNodeOffsets rstOffsets)
        {
            BNodeEncodingLE.Decode(stream, rst, tagsList, rstOffsets);
            rst.A = SingleEncodingLE.Decode(stream);
            rst.B = SingleEncodingLE.Decode(stream);
            rst.C = SingleEncodingLE.Decode(stream);
            rst.D = SingleEncodingLE.Decode(stream);
            rstOffsets.front = Int32EncodingLE.Decode(stream);
            rstOffsets.back = Int32EncodingLE.Decode(stream);
            RestorePointers(stream, rst, tagsList, rstOffsets);
        }
        private static void RestorePointers(Stream stream, BSplitterNode rst, TagsList tagsList, BSplitterNodeOffsets rstOffsets)
        {            
            long pos = stream.Position;

            // Fixup our dependents
            if (rstOffsets.front != -1) //front = RestorePointers(baseAddress, (int)front, tagListPtr);
            {
                rst.front = BNodeEncodingLE.ProcessNode(stream, rstOffsets.baseposition, rstOffsets.front, tagsList);
            }
            if (rstOffsets.back != -1) //back = RestorePointers(baseAddress, (int)back, tagListPtr);
            {
                rst.front = BNodeEncodingLE.ProcessNode(stream, rstOffsets.baseposition, rstOffsets.back, tagsList);
            }
            stream.Position = pos;
        }
        public static int Size
        {
            get { throw new NotImplementedException(); }
        }
    }

    public static class BPrimitiveNodeEncodingLE
    {
        public static void Encode(Stream stream, BPrimitiveNode val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, BPrimitiveNode rst, TagsList tagsList, BPrimitiveNodeOffsets rstOffsets)
        {
            BNodeEncodingLE.Decode(stream, rst, tagsList, rstOffsets);
            rstOffsets.prim = Int32EncodingLE.Decode(stream);
            RestorePointers(stream, rst, tagsList, rstOffsets);
        }
        private static void RestorePointers(Stream stream, BPrimitiveNode rst, TagsList tagsList, BPrimitiveNodeOffsets rstOffsets)
        {
            long pos = stream.Position;
            // Now fixup our polygon
            rst.prim = PrimEncodingLE.Decode(stream, rstOffsets.baseposition, rstOffsets.prim);
            stream.Position = pos;
        }
        public static int Size
        {
            get { throw new NotImplementedException(); }
        }
    }

    public static class BLitPrimitiveNodeEncodingLE
    {
        public static void Encode(Stream stream, BLitPrimitiveNode val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, BLitPrimitiveNode rst, TagsList tagsList, BLitPrimitiveNodeOffsets rstOffsets)
        {
            BNodeEncodingLE.Decode(stream, rst, tagsList, rstOffsets);
            rstOffsets.poly = Int32EncodingLE.Decode(stream);
            rstOffsets.backpoly = Int32EncodingLE.Decode(stream);
            RestorePointers(stream, rst, tagsList, rstOffsets);
        }
        private static void RestorePointers(Stream stream, BLitPrimitiveNode rst, TagsList tagsList, BLitPrimitiveNodeOffsets rstOffsets)
        {
            long pos = stream.Position;
            // Now fixup our polygon
            rst.poly = (Poly)PrimEncodingLE.Decode(stream, rstOffsets.baseposition, rstOffsets.poly);
            rst.backpoly = (Poly)PrimEncodingLE.Decode(stream, rstOffsets.baseposition, rstOffsets.backpoly);
            stream.Position = pos;
        }
        public static int Size
        {
            get { throw new NotImplementedException(); }
        }
    }

    public static class BCulledPrimitiveNodeEncodingLE
    {
        public static void Encode(Stream stream, BCulledPrimitiveNode val)
        {
            throw new NotImplementedException();
        }
        public static void Decode(Stream stream, BCulledPrimitiveNode rst, TagsList tagsList, BCulledPrimitiveNodeOffsets rstOffsets)
        {
            BNodeEncodingLE.Decode(stream, rst, tagsList, rstOffsets);
            rstOffsets.poly = Int32EncodingLE.Decode(stream);
            RestorePointers(stream, rst, tagsList, rstOffsets);
        }
        private static void RestorePointers(Stream stream, BCulledPrimitiveNode rst, TagsList tagsList, BCulledPrimitiveNodeOffsets rstOffsets)
        {
            long pos = stream.Position;
            // Now fixup our polygon
            Prim p = PrimEncodingLE.Decode(stream, rstOffsets.baseposition, rstOffsets.poly);
            rst.poly = p as Poly;
            stream.Position = pos;
        }
        public static int Size
        {
            get { throw new NotImplementedException(); }
        }
    }

    public static class BLightStringNodeEncodingLE
    {
        public static void Encode(Stream stream, BLightStringNode val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, BLightStringNode rst, TagsList tagsList, BLightStringNodeOffsets rstOffsets)
        {
            BPrimitiveNodeEncodingLE.Decode(stream, rst, tagsList, rstOffsets);
            rst.A = SingleEncodingLE.Decode(stream);
            rst.B = SingleEncodingLE.Decode(stream);
            rst.C = SingleEncodingLE.Decode(stream);
            rst.D = SingleEncodingLE.Decode(stream);
            rst.rgbaFront = Int32EncodingLE.Decode(stream);
            rst.rgbaBack = Int32EncodingLE.Decode(stream); ;
            RestorePointers(stream, rst, tagsList, rstOffsets);

        }
        private static void RestorePointers(Stream stream, BLightStringNode rst, TagsList tagsList, BLightStringNodeOffsets rstOffsets)
        {
        }
        public static int Size
        {
            get { throw new NotImplementedException(); }
        }
    }
    public static class BRenderControlNodeEncodingLE
    {
        public static void Encode(Stream stream, BRenderControlNode val)
        {
            throw new NotImplementedException();
        }

        public static void Decode(Stream stream, BRenderControlNode rst, TagsList tagsList, BRenderControlNodeOffsets rstOffsets)
        {
            BNodeEncodingLE.Decode(stream, rst, tagsList, rstOffsets);
            rst.Control = (BRenderControlType)Int32EncodingLE.Decode(stream);
            for (int i = 0; i < 4; i++)
                rst.IArg[i] = Int32EncodingLE.Decode(stream);
            for (int i = 0; i < 4; i++)
                rst.FArg[i] = SingleEncodingLE.Decode(stream); ;
            RestorePointers(stream, rst, tagsList, rstOffsets);
        }
        private static void RestorePointers(Stream stream, BRenderControlNode rst, TagsList tagsList, BRenderControlNodeOffsets rstOffsets)
        {
        }
        public static int Size
        {
            get { throw new NotImplementedException(); }
        }
    }
    #endregion
}
