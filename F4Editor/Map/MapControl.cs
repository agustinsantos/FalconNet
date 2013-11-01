using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ImageZoom.Windows.Forms
{
	public partial class MapControl : UserControl
	{
		
		Image img;
		Point mouseDown;
		int startx = 0;                         // offset of image when mouse was pressed
		int starty = 0;
        int imgx = 0;                         // current offset of image
        int imgy = 0;

        private int Imgx
        {
            get { return imgx; }
            set
            {
                imgx = value;
                if (img == null) imgx = 0;
                if ((imgx + img.Width) * zoom < pictureBox.Width)
                    imgx = (int)(pictureBox.Width / zoom - img.Width);
                if (imgx > 0)
                    imgx = 0;
            }
        }
        private int Imgy
        {
            get { return imgy; }
            set
            {
                imgy = value;
                if (img == null) imgy = 0;
                if ((imgy + img.Height) * zoom < pictureBox.Height)
                    imgy = (int)(pictureBox.Height / zoom - img.Height);
                if (imgy > 0)
                    imgy = 0;
            }
        }

		
		bool mousepressed = false;  // true as long as left mousebutton is pressed
		float zoom = 1;
		float maxZoom = 10;
		float minZoom = 1;
		readonly float DpiX, DpiY;
		
		List<IMapOverlay> overlays = new List<IMapOverlay>();
		public List<IMapOverlay> Overlays { get {return overlays;} }
		
		public Image Img {
			get {
				return this.img;
			}
			set {
                if (value == null) return;
				img = value;
				MinZoom = (float)Math.Max ((img.HorizontalResolution / DpiX), (img.VerticalResolution / DpiY)) / 2;
				Zoom = ((float)pictureBox.Width / (float)img.Width) * (img.HorizontalResolution / DpiX);
			}
		}
		
		public float Zoom {
			get {
				return this.zoom;
			}
			set {
				zoom = value;
				if (zoom > maxZoom)
					zoom = maxZoom;
				else if (zoom < minZoom)
					zoom = minZoom;
				
			}
		}

		public float MaxZoom {
			get {
				return this.maxZoom;
			}
			set {
				maxZoom = value;
				
			}
		}
		
		public float MinZoom {
			get {
				return this.minZoom;
			}
			set {
				minZoom = value;
				
			}
		}
		
		public MapControl ()
		{
			InitializeComponent ();
//            string imagefilename = @"..\..\test.tif";
//            img = Image.FromFile(imagefilename);

			Graphics g = this.CreateGraphics ();
			DpiX = g.DpiX;
			DpiY = g.DpiY;

			pictureBox.Paint += new PaintEventHandler (pictureBox_Paint);
		}

		private void pictureBox_MouseEnter (object sender, EventArgs e)
		{ 
			pictureBox.Focus (); 
		}
		
		private void pictureBox_MouseMove (object sender, EventArgs e)
		{
			MouseEventArgs mouse = e as MouseEventArgs;

			if (mouse.Button == MouseButtons.Left) {
				Point mousePosNow = mouse.Location;

				int deltaX = mousePosNow.X - mouseDown.X; // the distance the mouse has been moved since mouse was pressed
				int deltaY = mousePosNow.Y - mouseDown.Y;

				Imgx = (int)(startx + (deltaX / zoom));  // calculate new offset of image based on the current zoom factor
				Imgy = (int)(starty + (deltaY / zoom));

                pictureBox.Refresh ();
			} else{
				Point mousePosNow = mouse.Location;
				
				int deltaX = mousePosNow.X; // the distance the mouse has been moved since mouse was pressed
				int deltaY = mousePosNow.Y;

				int posx = (int)(deltaX / zoom);
				int posy = (int)(deltaY / zoom);
				
				bool needRefresh = false;
				foreach(IMapOverlay overlay in Overlays)
					needRefresh = needRefresh | overlay.OnMouseMove(posx, posy);
				
				if (needRefresh)
					pictureBox.Refresh ();
			}
		}

		private void pictureBox_MouseDown (object sender, EventArgs e)
		{
			MouseEventArgs mouse = e as MouseEventArgs;

			if (mouse.Button == MouseButtons.Left) {
				if (!mousepressed) {
					mousepressed = true;
					mouseDown = mouse.Location;
					startx = Imgx;
					starty = Imgy;
				}
			}
		}

		private void pictureBox_MouseUp (object sender, EventArgs e)
		{
			mousepressed = false;
		}

		protected override void OnMouseWheel (MouseEventArgs e)
		{
			float oldzoom = zoom;

			if (e.Delta > 0) {
				Zoom += 0.1F;
			} else if (e.Delta < 0) {
				Zoom = Math.Max (zoom - 0.1F, 0.01F);
			}

			MouseEventArgs mouse = e as MouseEventArgs;
			Point mousePosNow = mouse.Location;

			int x = mousePosNow.X - pictureBox.Location.X;    // Where location of the mouse in the pictureframe
			int y = mousePosNow.Y - pictureBox.Location.Y;

			int oldimagex = (int)(x / oldzoom);  // Where in the IMAGE is it now
			int oldimagey = (int)(y / oldzoom);

			int newimagex = (int)(x / zoom);     // Where in the IMAGE will it be when the new zoom i made
			int newimagey = (int)(y / zoom);

			Imgx = newimagex - oldimagex + Imgx;  // Where to move image to keep focus on one point
			Imgy = newimagey - oldimagey + Imgy;

			pictureBox.Refresh ();  // calls imageBox_Paint
		}

		private void pictureBox_Paint (object sender, PaintEventArgs e)
		{
            if (img == null) return;

			e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

			e.Graphics.ScaleTransform (zoom, zoom);
			e.Graphics.DrawImage (img, Imgx, Imgy);
			
			OnPaintOverlays (e.Graphics);
		}
		
		protected virtual void OnPaintOverlays (Graphics g)
		{
			g.SmoothingMode = SmoothingMode.HighQuality;
			g.TranslateTransform (Imgx, Imgy);
			using (var p = new Pen(Color.Blue, 2)) {
				g.DrawLine (p, new Point (0, 0), new Point (100, 100));
			}
			foreach (IMapOverlay o in Overlays) {
				if (o.IsVisible) {
					o.OnRender (g);
				}
			}
		}

		protected override bool ProcessCmdKey (ref Message msg, Keys keyData)
		{
			const int WM_KEYDOWN = 0x100;
			const int WM_SYSKEYDOWN = 0x104;

			if ((msg.Msg == WM_KEYDOWN) || (msg.Msg == WM_SYSKEYDOWN)) {
				switch (keyData) {
				case Keys.Right:
					Imgx -= (int)(pictureBox.Width * 0.1F / zoom);
					pictureBox.Refresh ();
					break;

				case Keys.Left:
					Imgx += (int)(pictureBox.Width * 0.1F / zoom);
					pictureBox.Refresh ();
					break;

				case Keys.Down:
					Imgy -= (int)(pictureBox.Height * 0.1F / zoom);
					pictureBox.Refresh ();
					break;

				case Keys.Up:
					Imgy += (int)(pictureBox.Height * 0.1F / zoom);
					pictureBox.Refresh ();
					break;

				case Keys.PageDown:
					Imgy -= (int)(pictureBox.Height * 0.90F / zoom);
					pictureBox.Refresh ();
					break;

				case Keys.PageUp:
					Imgy += (int)(pictureBox.Height * 0.90F / zoom);
					pictureBox.Refresh ();
					break;
				}
			}

			return base.ProcessCmdKey (ref msg, keyData);
		}
		

	}
	
	partial class MapControl
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose (bool disposing)
		{
			if (disposing && (components != null)) {
				components.Dispose ();
			}
			base.Dispose (disposing);
		}

        #region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent ()
		{
			this.pictureBox = new System.Windows.Forms.PictureBox ();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit ();
			this.SuspendLayout ();
			// 
			// pictureBox
			// 
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            //this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            //            | System.Windows.Forms.AnchorStyles.Left)
            //            | System.Windows.Forms.AnchorStyles.Right)));
			this.pictureBox.BackColor = System.Drawing.SystemColors.WindowFrame;
			this.pictureBox.Location = new System.Drawing.Point (0, 0);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size (863, 442);
			this.pictureBox.TabIndex = 8;
            this.pictureBox.Margin = new Padding(0);
			this.pictureBox.TabStop = false;
			this.pictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler (this.pictureBox_MouseDown);
			this.pictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler (this.pictureBox_MouseMove);
			this.pictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler (this.pictureBox_MouseUp);
			this.pictureBox.MouseEnter += new EventHandler (this.pictureBox_MouseEnter); 
			// 
			// ImageZoomMainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF (6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size (887, 466);
			this.Controls.Add (this.pictureBox);
			this.Name = "ImageZoomMainForm";
			this.Text = "Image Zoom and Scan with fixed focus";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit ();
			this.ResumeLayout (false);

		}

        #endregion

		private System.Windows.Forms.PictureBox pictureBox;
	}
}

