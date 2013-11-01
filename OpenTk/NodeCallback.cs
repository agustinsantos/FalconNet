using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OSGNet.Osg
{
    public class NodeCallback
    {
        public NodeCallback() { }

        public NodeCallback(NodeCallback nc, CopyOp co)
        {
            _nestedCallback = nc._nestedCallback;
        }

        /// <summary>
        /// Callback method called by the NodeVisitor when visiting a node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nv"></param>
        public virtual void Execute(Node node, NodeVisitor nv)
        {
            // note, callback is responsible for scenegraph traversal so
            // they must call traverse(node,nv) to ensure that the
            // scene graph subtree (and associated callbacks) are traversed.
            Traverse(node, nv);
        }


        /// <summary>
        /// Call any nested callbacks and then traverse the scene graph.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nv"></param>
        public void Traverse(Node node, NodeVisitor nv)
        {
            if (_nestedCallback != null) _nestedCallback.Execute(node, nv);
            else nv.Traverse(node);
        }

        public NodeCallback NestedCallback
        {
            get { return _nestedCallback; }
            set { _nestedCallback = value; }
        }

        public void AddNestedCallback(NodeCallback nc)
        {
            if (nc != null)
            {
                if (_nestedCallback != null)
                {
                    _nestedCallback.AddNestedCallback(nc);
                }
                else
                {
                    _nestedCallback = nc;
                }
            }
        }

        public void RemoveNestedCallback(NodeCallback nc)
        {
            if (nc != null)
            {
                if (_nestedCallback == nc)
                {
                    NodeCallback new_nested_callback = _nestedCallback.NestedCallback;
                    _nestedCallback.NestedCallback = null;
                    _nestedCallback = new_nested_callback;
                }
                else if (_nestedCallback != null)
                {
                    _nestedCallback.RemoveNestedCallback(nc);
                }
            }
        }

        internal NodeCallback _nestedCallback;
    }
}
