using FalconNet.Common.Graphics;
using System;

namespace FalconNet.Graphics
{
    /***************************************************************************\
        ObjList.h
        Scott Randolph
        April 22, 1996

        Manage the list of active objects to be drawn each frame for a given
        renderer.
    \***************************************************************************/



    // This structure is used in the viewpoint sorting to move objects directly to the list
    // they should go to, instead of only allowing them to go up or down one level at a heading
    internal class TransportStr
    {
        const int _NUM_OBJECT_LISTS_ = 5;
        public DrawableObject[] list = new DrawableObject[_NUM_OBJECT_LISTS_];
        public float[] bottom = new float[_NUM_OBJECT_LISTS_];
        public float[] top = new float[_NUM_OBJECT_LISTS_];
    }

    internal class UpdateCallBack
    {
        public delegate void Callback(object p, long l, Tpoint point, TransportStr t);

        Callback fn;
        object self;
        UpdateCallBack prev;
        UpdateCallBack next;
    }

    internal class SortCallBack
    {
        public delegate void Callback(object p);

        Callback fn;
        object self;
        SortCallBack prev;
        SortCallBack next;
    }

    public class ObjectDisplayList
    {

        public ObjectDisplayList()
            { throw new NotImplementedException(); }

        //TODO public ~ObjectDisplayList();

        public void Setup()
        {
        }

        public void Cleanup()
        {
        }

        public void InsertObject(DrawableObject objject_)
        {
            throw new NotImplementedException();
        }

        public void RemoveObject(DrawableObject object_)
        {
            throw new NotImplementedException();
        }

        internal void InsertUpdateCallbacks(UpdateCallBack ucb, SortCallBack scb, object self)
        {
            throw new NotImplementedException();
        }

        internal void RemoveUpdateCallbacks(UpdateCallBack ucb, SortCallBack scb, object self)
        {
            throw new NotImplementedException();
        }

        public void UpdateMetrics(Tpoint pos) // do update without moving around in lists
        {
            throw new NotImplementedException();
        }

        internal void UpdateMetrics(long listNo, Tpoint pos, TransportStr transList)
        {
            throw new NotImplementedException();
        }

        public void SortForViewpoint()
        {
            throw new NotImplementedException();
        }

        public void ResetTraversal()
        {
            nextToDraw = head;
        }

        public float GetNextDrawDistance()
        {
            if (nextToDraw != null)
                return nextToDraw.distance;
            else
                return -1.0f;
        }

        public void DrawBeyond(float ringDistance, int LOD, RenderOTW renderer)
        {
            while (nextToDraw != null &&
                //!F4IsBadReadPtr(nextToDraw, sizeof(DrawableObject)) && // JB 010318 CTD (too much CPU)
                (nextToDraw.distance >= ringDistance))
            {

                // Ask this object to draw itself
                nextToDraw.Draw(renderer, LOD);

                // Consider the next object
                nextToDraw = nextToDraw.next;
            }
        }

        public void DrawBeyond(float ringDistance, Render3D renderer)
        {
            while (nextToDraw != null && (nextToDraw.distance >= ringDistance))
            {

                // Ask this object to draw itself
                nextToDraw.Draw(renderer);

                // Consider the next object
                nextToDraw = nextToDraw.next;
            }
        }

        public DrawableObject GetNearest()
        {
            return tail;
        }

        public DrawableObject GetNext()
        {
            return nextToDraw;
        }

        public DrawableObject GetNextAndAdvance()
        {
            DrawableObject p = nextToDraw;
            if (nextToDraw != null)
                nextToDraw = nextToDraw.next;
            return p;
        }

        public void InsertionSortLink(DrawableObject[] listhead, DrawableObject[] listend)
        {
            throw new NotImplementedException();
        }

        public void QuickSortLink(DrawableObject[] head, DrawableObject end)
        {
            throw new NotImplementedException();
        }

        protected DrawableObject head;
        protected DrawableObject tail;
        protected DrawableObject nextToDraw;
        internal UpdateCallBack updateCBlist;
        internal SortCallBack sortCBlist;
    }

}

