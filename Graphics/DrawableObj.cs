using FalconNet.Common.Graphics;
using FalconNet.F4Common;
using System;
using DWORD = System.UInt32;

namespace FalconNet.Graphics
{
    /***************************************************************************\
        DrawObj.h
        Scott Randolph
        May 3, 1996
	
        Abstract base class used to interact with drawable objects which need
        to be drawn sorted with terrain in an out the window view.
    \***************************************************************************/
    public abstract class DrawableObject : IDrawableObject
    {
        public DrawableObject(float s)
        {
            drawClassID = DrawClass.Default;
            scale = s;
            parentList = null;
            prev = next = null;
        }
        //TODO public  virtual ~DrawableObject()	{ Debug.Assert( parentList == null ) };

        public enum DrawClass
        {
            Default,
            BSP,
            GroundVehicle,
            Guys,
            Building,
            Platform,
            Bridge,
            Roadbed,
            Overcast,
            Puffy,
            Trail
        };

        public DrawClass GetClass()
        {
            return drawClassID;
        }

        public float Radius()
        {
            return radius;
        }

        public void GetPosition(ref Tpoint pos)
        {
            pos = position;
        }

        public float X()
        {
            return position.x;
        }

        public float Y()
        {
            return position.y;
        }

        public float Z()
        {
            return position.z;
        }

        public void SetScale(float s)
        {
            radius *= s / scale;
            scale = s;
        }

        public float GetScale()
        {
            return scale;
        }

        public virtual void SetLabel(string label, DWORD p)
        {
        }

        public virtual void SetInhibitFlag(bool p)
        {
        }

        public abstract void Draw(RenderOTW renderer, int LOD);

        public virtual void Draw(Render3D r)
        {
        }

        // ray hit not implemented yet for object
        public virtual bool GetRayHit(Tpoint p, Tpoint p2, Tpoint p3, float f = 1.0f)
        {
            return false;
        }

        public bool InDisplayList()
        {
            return (parentList != null);
        }

        protected Tpoint position;
        protected float radius;
        protected float scale;
        protected DrawClass drawClassID;

        // NOTE:  Each instance can be managed by only ONE OBJECT LIST
        protected ObjectDisplayList parentList;
        public DrawableObject prev;
        public DrawableObject next;

        // NOTE:  This field is set by our parent list during UpdateMetrics
        public float distance;

        protected virtual void SetParentList(ObjectDisplayList list)
        {
            parentList = list;
        }

    }
}

