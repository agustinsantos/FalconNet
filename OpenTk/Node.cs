using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OSGNet.Osg
{
    /// <summary>
    /// Base class for all internal nodes in the scene graph.
    /// Provides interface for most common node operations (Composite Pattern).
    /// </summary>
    public class Node : IVisitorPattern
    {

                /** Construct a node.
            Initialize the parent list to empty, node name to "" and
            bounding sphere dirty flag to true.*/
        public Node() 
        {
            _boundingSphereComputed = false;
#if TODO
            _nodeMask = 0xffffffff;

            _numChildrenRequiringUpdateTraversal = 0;

            _numChildrenRequiringEventTraversal = 0;

            _cullingActive = true;
            _numChildrenWithCullingDisabled = 0;

            _numChildrenWithOccluderNodes = 0;
 
#endif
            }

        /** Copy constructor using CopyOp to manage deep vs shallow copy.*/
        public Node(Node node, CopyFlags copyop = CopyFlags.SHALLOW_COPY)
        { throw new NotImplementedException(); }



        public virtual void Accept(NodeVisitor nv)
        {
            throw new NotImplementedException();
        }

        public virtual void Ascend(NodeVisitor nv)
        {
            throw new NotImplementedException();
        }

        public virtual void Traverse(NodeVisitor nv)
        {
            throw new NotImplementedException();
        }

        internal void AddParent(Group node)
        {
            _parents.Add(node);
        }

        internal void RemoveParent(Group node)
        {
            _parents.Remove(node);
        }

        /*
 
/*  Set the initial bounding volume to use when computing the overall bounding volume.*/
        /// <summary>
        /// the initial bounding volume to use when computing the overall bounding volume
        /// </summary>
        public BoundingSphere InitialBound
        {
            get { return _initialBound; }
            set { _initialBound = value; DirtyBound(); }
        }

        /// <summary>
        ///  Mark this node's bounding sphere dirty.
        ///  Forcing it to be computed on the next call to getBound()
        /// </summary>
        public void DirtyBound()
        {
            if (_boundingSphereComputed)
            {
                _boundingSphereComputed = false;

                // dirty parent bounding sphere's to ensure that all are valid.
                if (_parents != null)
                    foreach (Group parent in _parents)
                    {
                        parent.DirtyBound();
                    }

            }
        }

        /// <summary>
        /// Get the bounding sphere of node.
        /// Using lazy evaluation computes the bounding sphere if it is 'dirty'
        /// </summary>
        /// <returns></returns>
        public BoundingSphere GetBound()
        {
            if (!_boundingSphereComputed)
            {
                _boundingSphere = _initialBound;
                if (_computeBoundCallback != null)
                    _boundingSphere.ExpandBy(_computeBoundCallback(this));
                else
                    _boundingSphere.ExpandBy(ComputeBound());

                _boundingSphereComputed = true;
            }
            return _boundingSphere;
        }


        /// <summary>
        /// Compute the bounding sphere around Node's geometry or children.
        ///  This method is automatically called by getBound() when the bounding
        /// sphere has been marked dirty via dirtyBound()
        /// </summary>
        /// <returns></returns>
        public virtual BoundingSphere ComputeBound()
        {
            return new BoundingSphere();
        }

        /// <summary>
        /// Callback to allow users to override the default computation of bounding volume.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public delegate BoundingSphere ComputeBoundDelegate(Node n);

        /// <summary>
        /// the compute bound callback to override the default computeBound.
        /// </summary>
        public ComputeBoundDelegate ComputeBoundingSphereCallback
        {
            get { return _computeBoundCallback; }
            set { _computeBoundCallback = value; }
        }


        #region Protected
        protected BoundingSphere _initialBound;
        protected ComputeBoundDelegate _computeBoundCallback;
        protected BoundingSphere _boundingSphere;
        protected bool _boundingSphereComputed;

        protected List<Group> _parents;
        #endregion
    }
}
