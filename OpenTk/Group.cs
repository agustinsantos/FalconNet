using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace OSGNet.Osg
{
    /// <summary>
    /// General group node which maintains a list of children.
    /// </summary>
    public class Group : Node
    {
        public Group() { }

        /** Copy constructor using CopyOp to manage deep vs shallow copy. */
        public Group(Group group, CopyFlags copyop = CopyFlags.SHALLOW_COPY)
            : base(group, copyop)
        {
            if (group._children != null)
                foreach (Node node in group._children)
                {
                    Node child = CopyOp.Copy(node, copyop);
                    if (child != null) AddChild(child);
                }
        }



        /// <summary>
        /// Add Node to Group.
        /// If node is not NULL and is not contained in Group then increment its
        ///  reference count, add it to the child list and dirty the bounding
        ///  sphere to force it to recompute on next getBound() and return true for success.
        ///  Otherwise return false. Scene nodes can't be added as child nodes.
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public virtual void AddChild(Node child)
        {
            Debug.Assert(child != null);
            _children.Add(child);
        }

        /// <summary>
        /// Insert Node to Group at specific location.
        /// The new child node is inserted into the child list
        /// before the node at the specified index. No nodes
        /// are removed from the group with this operation.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        public virtual void InsertChild(int index, Node child)
        {
            Debug.Assert(child != null);
            _children.Insert(index, child);
            child.AddParent(this);
        }

        /// <summary>
        /// Remove Node from Group.
        ///  If Node is contained in Group then remove it from the child
        ///  list, decrement its reference count, and dirty the
        ///  bounding sphere to force it to recompute on next getBound() and
        ///  return true for success. If Node is not found then return false
        ///  and do not change the reference count of the Node.
        ///  Note, do not override, only override removeChildren(,) is required.
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public void RemoveChild(Node child)
        {
            int pos = GetChildIndex(child);
            if (pos < _children.Count)
                RemoveChildren(pos, 1);
        }

        /// <summary>
        /// Remove Node from Group.
        ///  If Node is contained in Group then remove it from the child
        ///  list, decrement its reference count, and dirty the
        /// bounding sphere to force it to recompute on next getBound() and
        ///  return true for success. If Node is not found then return false
        /// and do not change the reference count of the Node.
        /// Note, do not override, only override removeChildren(,) is required.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="numChildrenToRemove"></param>
        /// <returns></returns>
        public void RemoveChild(int pos, int numChildrenToRemove = 1)
        {
            if (pos < _children.Count)
                RemoveChildren(pos, numChildrenToRemove);
        }

        /// <summary>
        /// Remove children from Group.
        ///  Note, must be override by subclasses of Group which add per child attributes.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="numChildrenToRemove"></param>
        /// <returns></returns>
        public virtual void RemoveChildren(int pos, int numChildrenToRemove)
        {
            int endOfRemoveRange = pos + numChildrenToRemove;
            Debug.Assert(endOfRemoveRange > _children.Count);

            for (int i = pos; i < endOfRemoveRange; ++i)
            {
                Node child = _children[i];
                // remove this Geode from the child parent list.
                child.RemoveParent(this);
            }
            _children.RemoveRange(pos, numChildrenToRemove);
        }

        /// <summary>
        /// Replace specified Node with another Node.
        /// Equivalent to setChild(getChildIndex(orignChild),node)
        /// See docs for setChild for further details on implementation.
        /// </summary>
        /// <param name="origChild"></param>
        /// <param name="newChild"></param>
        /// <returns></returns>
        public virtual bool ReplaceChild(Node origChild, Node newChild)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return the number of children nodes.
        /// </summary>
        public int NumChildren { get { return (int)_children.Count; } }


        /// <summary>
        /// Get/Set child node at position i.
        /// Return true if set correctly, false on failure (if node==NULL || i is out of range).
        /// When Set can be successful applied, the algorithm is : decrement the reference count origNode and increment the
        ///  reference count of newNode, and dirty the bounding sphere
        ///  to force it to recompute on next getBound() and return true.
        /// If origNode is not found then return false and do not
        /// add newNode. If newNode is NULL then return false and do
        ///  not remove origNode. Also returns false if newChild is a Scene node.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public virtual Node this[int i]
        {
            get { return _children[i]; }
            set
            {
                if (i < _children.Count && value != null)
                {

                    Node origNode = _children[i];
                    Node newNode = value;
                    // first remove for origNode's parent list.
                    origNode.RemoveParent(this);

                    // note ref_ptr<> automatically handles decrementing origNode's reference count,
                    // and inccrementing newNode's reference count.
                    _children[i] = newNode;

                    // register as parent of child.
                    newNode.AddParent(this);
                }
            }
        }

        /// <summary>
        /// Return true if node is contained within Group.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool ContainsNode(Node node)
        {
            return _children.Contains(node);
        }

        /// <summary>
        /// Get the index number of child, return a value between
        /// 0 and _children.count -1 if found, if not found then
        /// return _children.Count
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public int GetChildIndex(Node node)
        {
            return _children.IndexOf(node);
        }

        //public virtual BoundingSphere computeBound()  ;

        public override void Traverse(NodeVisitor nv)
        {
            foreach (Node node in _children)
            {
                node.Accept(nv);
            }
        }

        public override BoundingSphere ComputeBound()
        {
            BoundingSphere bsphere = new BoundingSphere();
            if (_children == null || _children.Count == 0)
            {
                return bsphere;
            }

            // note, special handling of the case when a child is an Transform,
            // such that only Transforms which are relative to their parents coordinates frame (i.e this group)
            // are handled, Transform relative to and absolute reference frame are ignored.

            BoundingBox bb = new BoundingBox();
            bb.Init();
            foreach (Node node in _children)
            {
                Transform transform = node as Transform;
                if (transform == null || transform.ReferenceFrame == ReferenceFrame.RELATIVE_RF)
                {
                    bb.ExpandBy(node.GetBound());
                }
            }

            if (!bb.IsValid)
            {
                return bsphere;
            }

            bsphere._center = bb.Center;
            bsphere._radius = 0.0f;
            foreach (Node node in _children)
            {
                Transform transform = node as Transform;
                if (transform == null || transform.ReferenceFrame == ReferenceFrame.RELATIVE_RF)
                {
                    bsphere.ExpandRadiusBy(node.GetBound());
                }
            }

            return bsphere;
        }

        #region Protected
        protected List<Node> _children;
        #endregion
    }
}
