using System;
using System.Drawing;

namespace ImageZoom.Windows.Forms
{
	public interface IMapOverlay
	{
		event EventHandler Changed;
		
		bool IsVisible {get; set;}
		void OnRender(Graphics g);
		bool OnMouseMove(int x, int y);
	}
}

