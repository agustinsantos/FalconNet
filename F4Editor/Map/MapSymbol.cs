   using System;
   using System.Drawing;
   using System.Runtime.Serialization;
   using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;
using System.Diagnostics;

namespace ImageZoom.Windows.Forms
{


	/// <summary>
	/// GMap.NET marker
	/// </summary>
	[Serializable]
   public class MapSymbol : IMapOverlay, IDisposable
	{

		private Point position;

		public Point Position {
			get {
				return position;
			}
			set {
				if (position != value) {
					position = value;

					if (IsVisible) {
//                  if(Overlay != null && Overlay.Control != null)
//                  {
//                     Overlay.Control.UpdateMarkerLocalPosition(this);
//                  }
					}
				}
			}
		}

		public object Tag;
		Point offset;

		public Point Offset {
			get {
				return offset;
			}
			set {
				if (offset != value) {
					offset = value;

					if (IsVisible) {
//                  if(Overlay != null && Overlay.Control != null)
//                  {
//                     Overlay.Control.UpdateMarkerLocalPosition(this);
//                  }
					}
				}
			}
		}

		Rectangle area;


		/// <summary>
		/// ToolTip position in local coordinates
		/// </summary>
		public Point ToolTipPosition {
			get {
				Point ret = area.Location;
				ret.Offset (-Offset.X, -Offset.Y);
				return ret;
			}
		}

		public Size Size {
			get {
				return area.Size;
			}
			set {
				area.Size = value;
			}
		}

		public Rectangle LocalArea {
			get {
				return area;
			}
		}

		internal Rectangle LocalAreaInControlSpace {
			get {
				Rectangle r = area;
//            if(Overlay != null && Overlay.Control != null)
//            {
//               r.Offset((int)Overlay.Control.Core.renderOffset.X, (int)overlay.Control.Core.renderOffset.Y);
//            }
				return r;
			}
		}

		public MapToolTip ToolTip;
		public SymbolTooltipMode ToolTipMode = SymbolTooltipMode.OnMouseOver;
		string toolTipText;

		public string ToolTipText {
			get {
				return toolTipText;
			}

			set {
				if (ToolTip == null && !string.IsNullOrEmpty (value)) {
					ToolTip = new MapToolTip (this);
				}
				toolTipText = value;
			}
		}
		
		public event EventHandler Changed;

		private void RaiseChanged ()
		{
			if (Changed != null)
				Changed (this, EventArgs.Empty);
		}
		
		private bool visible = true;

		/// <summary>
		/// is marker visible
		/// </summary>
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

		/// <summary>
		/// if true, marker will be rendered even if it's outside current view
		/// </summary>
		public bool DisableRegionCheck = false;

		/// <summary>
		/// can maker receive input
		/// </summary>
		public bool IsHitTestVisible = true;
		private bool isMouseOver = false;
		private Image img;
		
		/// <summary>
		/// is mouse over marker
		/// </summary>
		public bool IsMouseOver {
			get {
				return isMouseOver;
			}
			internal set {
				isMouseOver = value;
			}
		}
		
		public MapSymbol (Point pos, string symbolName)
		{
            string dir = Directory.GetCurrentDirectory();
            foreach (string file in Directory.EnumerateFiles("."))
                Debug.WriteLine(file);
            if (!File.Exists(symbolName))
                return;
			img = Image.FromFile (symbolName);
			this.Position = pos;
		}

		public bool OnMouseMove (int x, int y)
		{
			if (x >= 100 && !IsMouseOver) {
				IsMouseOver = true;
				return true;
			} 
			if (x < 100 && IsMouseOver) {
				IsMouseOver = false;
				return true;
			}
			return false;
		}
		
		public virtual void OnRender (Graphics g)
		{
			g.DrawImage (img, Position.X + Offset.X, Position.Y + Offset.Y);
			if ((ToolTip != null) && (ToolTipMode == SymbolTooltipMode.Always || (ToolTipMode == SymbolTooltipMode.OnMouseOver && IsMouseOver)))
				ToolTip.OnRender (g);
		}

    
      #region IDisposable Members

		bool disposed = false;

		public virtual void Dispose ()
		{
			if (!disposed) {
				disposed = true;

				Tag = null;

				if (ToolTip != null) {
					toolTipText = null;
					ToolTip.Dispose ();
					ToolTip = null;
				}
			}
		}

      #endregion
	}

	public delegate void SymbolClick (MapSymbol item,MouseEventArgs e);

	public delegate void SymbolEnter (MapSymbol item);

	public delegate void SymbolLeave (MapSymbol item);

	/// <summary>
	/// modeof tooltip
	/// </summary>
	public enum SymbolTooltipMode
	{
		OnMouseOver,
		Never,
		Always,
	}
}