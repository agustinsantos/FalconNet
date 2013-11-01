using System;

namespace OSGNet.Osg
{
    [Flags]
    public enum CopyFlags
    {
        SHALLOW_COPY = 0,
        DEEP_COPY_OBJECTS = 1 << 0,
        DEEP_COPY_NODES = 1 << 1,
        DEEP_COPY_DRAWABLES = 1 << 2,
        DEEP_COPY_STATESETS = 1 << 3,
        DEEP_COPY_STATEATTRIBUTES = 1 << 4,
        DEEP_COPY_TEXTURES = 1 << 5,
        DEEP_COPY_IMAGES = 1 << 6,
        DEEP_COPY_ARRAYS = 1 << 7,
        DEEP_COPY_PRIMITIVES = 1 << 8,
        DEEP_COPY_SHAPES = 1 << 9,
        DEEP_COPY_UNIFORMS = 1 << 10,
        DEEP_COPY_CALLBACKS = 1 << 11,
        DEEP_COPY_USERDATA = 1 << 12,
        DEEP_COPY_ALL = 0x7FFFFFFF
    }

    public class CopyOp
    {
        public CopyFlags CopyFlags
        { 
            get { return _flags; }
            set { _flags = value; }
        }

        public static Node Copy(Node node, CopyFlags flag)
        { throw new NotImplementedException(); }

        public Node Copy(Node node)
        { throw new NotImplementedException(); }

        protected CopyFlags _flags;
    }
}
