using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OSGNet.Osg
{
    public enum TraversalMode
    {
        TRAVERSE_NONE,
        TRAVERSE_PARENTS,
        TRAVERSE_ALL_CHILDREN,
        TRAVERSE_ACTIVE_CHILDREN
    }

    public enum VisitorType
    {
        NODE_VISITOR = 0,
        UPDATE_VISITOR,
        EVENT_VISITOR,
        COLLECT_OCCLUDER_VISITOR,
        CULL_VISITOR
    }

    /// <summary>
    /// Visitor for type safe operations on osg::Nodes.
    /// Based on GOF's Visitor pattern. The NodeVisitor
    /// is useful for developing type safe operations to nodes
    /// in the scene graph (as per Visitor pattern), and adds to this
    /// support for optional scene graph traversal to allow
    /// operations to be applied to whole scenes at once. The Visitor
    /// pattern uses a technique of double dispatch as a mechanism to
    /// call the appropriate apply(..) method of the NodeVisitor.  To
    /// use this feature one must use the Node::accept(NodeVisitor) which
    /// is extended in each Node subclass, rather than the NodeVisitor
    /// apply directly.  So use root->accept(myVisitor); instead of
    /// myVisitor.apply(*root).  The later method will bypass the double
    /// dispatch and the appropriate NodeVisitor::apply(..) method will
    /// not be called.
    /// </summary>
    public class NodeVisitor
    {
        public NodeVisitor(TraversalMode tm = TraversalMode.TRAVERSE_NONE)
        {
            throw new NotImplementedException();
        }

        public NodeVisitor(VisitorType type, TraversalMode tm = TraversalMode.TRAVERSE_NONE)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method to call to reset visitor. Useful if your visitor accumulates
        /// state during a traversal, and you plan to reuse the visitor.
        /// To flush that state for the next traversal: call reset() prior
        /// to each traversal.
        /// </summary>
        public virtual void Reset() { }

        /// <summary>
        /// Get/Set the VisitorType, used to distinguish different visitors during
        /// traversal of the scene, typically used in the Node::traverse() method
        /// to select which behaviour to use for different types of traversal/visitors.
        /// </summary>
        public VisitorType VisitorType
        {
            get { return _visitorType; }
            set { _visitorType = value; }
        }

        /// <summary>
        /// Get/Set the traversal number. Typically used to denote the frame count.
        /// </summary>
        public uint TraversalNumber
        {
            get { return _traversalNumber; }
            set { _traversalNumber = value; }
        }

        /// <summary>
        /// Get/Set user data. 
        /// </summary>
        public object UserData
        {
            get { return _userData; }
            set { _userData = value; }
        }

        /// <summary>
        /// Method for handling traversal of a nodes.
        ///  If you intend to use the visitor for actively traversing
        /// the scene graph then make sure the accept() methods call
        /// this method unless they handle traversal directly.
        /// </summary>
        /// <param name="node"></param>
        public void Traverse(Node  node)
        {
            if (_traversalMode == TraversalMode.TRAVERSE_PARENTS) node.Ascend(this);
            else if (_traversalMode != TraversalMode.TRAVERSE_NONE) node.Traverse(this);
        }

        #region Protected

        protected VisitorType _visitorType;
        protected uint _traversalNumber;
        protected TraversalMode _traversalMode;
        protected object _userData;

        #endregion
    }
}
