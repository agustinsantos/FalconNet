using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OSGNet.Osg
{
    public enum ReferenceFrame
    {
        RELATIVE_RF,
        ABSOLUTE_RF,
        ABSOLUTE_RF_INHERIT_VIEWPOINT
    }

    /// <summary>
    /// A Transform is a group node for which all children are transformed by
    /// a 4x4 matrix. It is often used for positioning objects within a scene,
    /// producing trackball functionality or for animation.
    ///
    /// Transform itself does not provide set/get functions, only the interface
    /// for defining what the 4x4 transformation is.  Subclasses, such as
    /// MatrixTransform and PositionAttitudeTransform support the use of an
    /// osg::Matrix or a osg::Vec3/osg::Quat respectively.
    ///
    /// Note: If the transformation matrix scales the subgraph then the normals
    /// of the underlying geometry will need to be renormalized to be unit
    /// vectors once more.  This can be done transparently through OpenGL's
    /// use of either GL_NORMALIZE and GL_RESCALE_NORMAL modes. For further
    /// background reading see the glNormalize documentation in the OpenGL
    /// Reference Guide (the blue book). To enable it in the OSG, you simply
    /// need to attach a local osg::StateSet to the osg::Transform, and set
    /// the appropriate mode to ON via
    ///   stateset.setMode(GL_NORMALIZE, osg::StateAttribute::ON);
    /// </summary>
    public class Transform : Group
    {
        public Transform()
        {
            _referenceFrame = ReferenceFrame.RELATIVE_RF;
        }

        /** Copy constructor using CopyOp to manage deep vs shallow copy. */
        public Transform(Transform t, CopyFlags copyop = CopyFlags.SHALLOW_COPY)
            : base(t, copyop)
        {
            _referenceFrame = t._referenceFrame;
        }


        /* Set the transform's ReferenceFrame, either to be relative to its
          * parent reference frame, or relative to an absolute coordinate
          * frame. RELATIVE_RF is the default.
          * Note: Setting the ReferenceFrame to be ABSOLUTE_RF will
          * also set the CullingActive flag on the transform, and hence all
          * of its parents, to false, thereby disabling culling of it and
          * all its parents.  This is necessary to prevent inappropriate
          * culling, but may impact cull times if the absolute transform is
          * deep in the scene graph.  It is therefore recommended to only use
          * absolute Transforms at the top of the scene, for such things as
          * heads up displays.
          * ABSOLUTE_RF_INHERIT_VIEWPOINT is the same as ABSOLUTE_RF except it
          * adds the ability to use the parents view points position in world coordinates
          * as its local viewpoint in the new coordinates frame.  This is useful for
          * Render to texture Cameras that wish to use the main views LOD range computation
          * (which uses the viewpoint rather than the eye point) rather than use the local
          * eye point defined by the this Transforms' absolute view matrix.
        */
        public ReferenceFrame ReferenceFrame
        {
            get { return _referenceFrame; }
            set { throw new NotImplementedException(); }
        }

        public virtual bool ComputeLocalToWorldMatrix(Matrixf matrix, NodeVisitor nv)
        {
            if (_referenceFrame == ReferenceFrame.RELATIVE_RF)
            {
                return false;
            }
            else // absolute
            {
                matrix.MakeIdentity();
                return true;
            }
        }

        public virtual bool ComputeWorldToLocalMatrix(Matrixf matrix, NodeVisitor nv)
        {
            if (_referenceFrame == ReferenceFrame.RELATIVE_RF)
            {
                return false;
            }
            else // absolute
            {
                matrix.MakeIdentity();
                return true;
            }
        }

        /// <summary>
        /// Overrides Group's computeBound.
        /// There is no need to override in subclasses from osg::Transform
        /// since this computeBound() uses the underlying matrix (calling
        /// computeMatrix if required). 
        /// </summary>
        /// <returns></returns>
        public override BoundingSphere ComputeBound()
        { throw new NotImplementedException(); }

        protected ReferenceFrame _referenceFrame;

    }
}
