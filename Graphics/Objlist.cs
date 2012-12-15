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
// they should go to, instead of only allowing them to go up or down one level at a time
	internal struct TransportStr
	{
		const int	_NUM_OBJECT_LISTS_ = 5;
		public DrawableObject[]	list;
		public float[]			bottom;
		public float[]			top;

		public TransportStr ()
		{
			list = new DrawableObject[_NUM_OBJECT_LISTS_];
			bottom = new float[_NUM_OBJECT_LISTS_];
			top = new float[_NUM_OBJECT_LISTS_];
		}
	}

	internal struct UpdateCallBack
	{
		public delegate void Callback (object p,long l,Tpoint point,TransportStr t);

		Callback fn;
		object self;
		UpdateCallBack prev;
		UpdateCallBack next;
	}

	internal struct SortCallBack
	{
		public delegate void Callback (object p);

		Callback fn;
		object self;
		SortCallBack prev;
		SortCallBack next;
	}

	public class ObjectDisplayList
	{
   
		public ObjectDisplayList ();
		//TODO public ~ObjectDisplayList();

		public void	Setup ()
		{
		}

		public void	Cleanup ()
		{
		}

		public void	InsertObject (DrawableObject objject_)
		{
			throw new NotImplementedException ();
		}

		public void	RemoveObject (DrawableObject object_)
		{
			throw new NotImplementedException ();
		}

		public void	InsertUpdateCallbacks (UpdateCallBack ucb, SortCallBack scb, object self)
		{
			throw new NotImplementedException ();
		}

		public void	RemoveUpdateCallbacks (UpdateCallBack ucb, SortCallBack scb, object self)
		{
			throw new NotImplementedException ();
		}

		public void	UpdateMetrics (Tpoint pos) // do update without moving around in lists
		{
			throw new NotImplementedException ();
		}
		
		public void	UpdateMetrics (long listNo, Tpoint pos, TransportStr transList)
		{
			throw new NotImplementedException ();
		}
		
		public void	SortForViewpoint ()
		{
			throw new NotImplementedException ();
		}

		public void	ResetTraversal ()
		{
			nextToDraw = head;
		}

		public float	GetNextDrawDistance ()
		{
			if (nextToDraw)
				return nextToDraw->distance;
			else
				return -1.0f;
		}

		public void	DrawBeyond (float ringDistance, int LOD, RenderOTW *renderer);

		public void	DrawBeyond (float ringDistance, Render3D *renderer);

		public DrawableObject	GetNearest ()
		{
			return tail;
		}

		public DrawableObject	GetNext ()
		{
			return nextToDraw;
		}

		public DrawableObject GetNextAndAdvance ()
		{
			DrawableObject p = nextToDraw;
			if (nextToDraw)
				nextToDraw = nextToDraw.next;
			return p;
		}

		public void InsertionSortLink (DrawableObject[] listhead, DrawableObject[] listend)
		{
			throw new NotImplementedException ();
		}

		public void QuickSortLink (DrawableObject[] head, DrawableObject end)
		{
			throw new NotImplementedException ();
		}
  
		protected DrawableObject	head;
		protected DrawableObject	tail;
		protected DrawableObject	nextToDraw;
		protected UpdateCallBack	updateCBlist;
		protected SortCallBack	sortCBlist;
	}

}

