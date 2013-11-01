using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OSGNet.Osg
{
    public interface IVisitorPattern
    {
        /// <summary>
        /// Visitor Pattern : calls the apply method of a NodeVisitor with this node's type.*/
        /// </summary>
        /// <param name="nv"></param>
        void Accept(NodeVisitor nv);

        /// <summarTraverse upwards : calls parents' accept method with NodeVisitor.y>
        /// Traverse upwards : calls parents' accept method with NodeVisitor.
        /// </summary>
        /// <param name="nv"></param>
        void Ascend(NodeVisitor nv);

        /// <summary>
        /// Traverse downwards : calls children's accept method with NodeVisitor.
        /// </summary>
        /// <param name="nv"></param>
        void Traverse(NodeVisitor nv);
    }
}
