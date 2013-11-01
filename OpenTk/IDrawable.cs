using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
namespace OSGNet.Osg
{
    /// <summary>
    /// Stores a set of modes and attributes which represent a set of OpenGL state.
  /// Notice that a \c StateSet contains just a subset of the whole OpenGL state.
  /// <p>In OSG, each \c Drawable and each \c Node has a reference to a
  /// \c StateSet. These <tt>StateSet</tt>s can be shared between
  /// different <tt>Drawable</tt>s and <tt>Node</tt>s (that is, several
  /// <tt>Drawable</tt>s and <tt>Node</tt>s can reference the same \c StateSet).
  /// Indeed, this practice is recommended whenever possible,
    /// as this minimizes expensive state changes in the graphics pipeline.
    /// </summary>
    public class StateSet
    {
        
    }

    /// <summary>
    /// list values which can be used to set either GLModeValues or OverrideValues.
    ///  When using in conjunction with GLModeValues, all Values have meaning.
    /// When using in conjunction with StateAttribute OverrideValue only
    /// OFF,OVERRIDE and INHERIT are meaningful.
    /// However, they are useful when using GLModeValue
    /// and OverrideValue in conjunction with each other as when using
    /// StateSet::setAttributeAndModes(..).
    /// </summary>
    public enum Values
    {
        /** means that associated GLMode and Override is disabled.*/
        OFF = 0x0,
        /** means that associated GLMode is enabled and Override is disabled.*/
        ON = 0x1,
        /** Overriding of GLMode's or StateAttributes is enabled, so that state below it is overridden.*/
        OVERRIDE = 0x2,
        /** Protecting of GLMode's or StateAttributes is enabled, so that state from above cannot override this and below state.*/
        PROTECTED = 0x4,
        /** means that GLMode or StateAttribute should be inherited from above.*/
        INHERIT = 0x8
    }

    /// <summary>
    /// Values of StateAttribute::Type used to aid identification
    /// of different StateAttribute subclasses. Each subclass defines
    /// its own value in the virtual Type getType() method.  When
    /// extending the osg's StateAttribute's simply define your
    /// own Type value which is unique, using the StateAttribute::Type
    /// enum as a guide of what values to use.  If your new subclass
    /// needs to override a standard StateAttriubte then simply use
    /// that type's value.
    /// </summary>
    public enum Type
    {
        TEXTURE,

        POLYGONMODE,
        POLYGONOFFSET,
        MATERIAL,
        ALPHAFUNC,
        ANTIALIAS,
        COLORTABLE,
        CULLFACE,
        FOG,
        FRONTFACE,

        LIGHT,

        POINT,
        LINEWIDTH,
        LINESTIPPLE,
        POLYGONSTIPPLE,
        SHADEMODEL,
        TEXENV,
        TEXENVFILTER,
        TEXGEN,
        TEXMAT,
        LIGHTMODEL,
        BLENDFUNC,
        BLENDEQUATION,
        LOGICOP,
        STENCIL,
        COLORMASK,
        DEPTH,
        VIEWPORT,
        SCISSOR,
        BLENDCOLOR,
        MULTISAMPLE,
        CLIPPLANE,
        COLORMATRIX,
        VERTEXPROGRAM,
        FRAGMENTPROGRAM,
        POINTSPRITE,
        PROGRAM,
        CLAMPCOLOR,
        HINT,
        SAMPLEMASKI,
        PRIMITIVERESTARTINDEX,

        /// osgFX namespace
        VALIDATOR,
        VIEWMATRIXEXTRACTOR,

        /// osgNV namespace
        OSGNV_PARAMETER_BLOCK,

        // osgNVExt namespace
        OSGNVEXT_TEXTURE_SHADER,
        OSGNVEXT_VERTEX_PROGRAM,
        OSGNVEXT_REGISTER_COMBINERS,

        /// osgNVCg namespace
        OSGNVCG_PROGRAM,

        // osgNVSlang namespace
        OSGNVSLANG_PROGRAM,

        // osgNVParse
        OSGNVPARSE_PROGRAM_PARSER,

        UNIFORMBUFFERBINDING,
        TRANSFORMFEEDBACKBUFFERBINDING,

        ATOMICCOUNTERBUFFERBINDING,

        PATCH_PARAMETER,

        FRAME_BUFFER_OBJECT
    }

    

    public delegate void StateAttributeCallback(StateAttribute sa, NodeVisitor nv);
    public class StateAttribute
    {
        protected void AddParent(StateSet obj)
        {
            throw new NotImplementedException();
        }

        protected void RemoveParent(StateSet obj)
        {
            throw new NotImplementedException();
        }

        protected List<StateSet> _parents;


        //protected ShaderComponent _shaderComponent;

        //protected StateAttributeCallback _updateCallback;
        //protected StateAttributeCallback _eventCallback;
    }

    public interface IDrawable
    {
        //void Draw(RenderInfo renderInfo);
    }
}
