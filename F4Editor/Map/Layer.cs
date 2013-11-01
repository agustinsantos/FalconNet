using System;
using System.Collections.Generic;

namespace ImageZoom.Windows.Forms
{
	public class Layer: IMapOverlay
	{
		List<IMapOverlay> overlays = new List<IMapOverlay> ();

		public List<IMapOverlay> Overlays { get { return overlays; } }
		
		public Layer ()
		{
		}

		#region IMapOverlay implementation
		public event EventHandler Changed;
		
		public void OnRender (System.Drawing.Graphics g)
		{
			if (!this.IsVisible)
				return;

			foreach (IMapOverlay o in Overlays) {
				if (o.IsVisible) {
					o.OnRender (g);
				}
			}
		}

		public bool OnMouseMove (int x, int y)
		{
			bool needRefresh = false;
			foreach (IMapOverlay overlay in Overlays)
				needRefresh = needRefresh | overlay.OnMouseMove (x, y);
			return needRefresh;
		}

		private bool visible = true;

		public bool IsVisible {
			get {
				return visible;
			}
			set {
				if (value != visible) {
					visible = value;
					RaiseChanged ();
				}
			}
		}
		#endregion
		
		private void RaiseChanged ()
		{
			if (Changed != null)
				Changed (this, EventArgs.Empty);
		}
	}
}

