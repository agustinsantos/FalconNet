   using System;
   using System.Drawing;
   using System.Drawing.Drawing2D;
   using System.Runtime.Serialization;
using System.Collections;
    using System.Collections.Generic;

namespace ImageZoom.Windows.Forms
{
   /// <summary>
   /// GMap.NET marker
   /// </summary>
   [Serializable]
   public class MapToolTip : IDisposable
   {
      MapSymbol marker;
      public MapSymbol Marker
      {
         get
         {
            return marker;
         }
         internal set
         {
            marker = value;
         }
      }

      public Point Offset;

      public static readonly StringFormat DefaultFormat = new StringFormat();

      /// <summary>
      /// string format
      /// </summary>
      [NonSerialized]
      public readonly StringFormat Format = DefaultFormat;

      public static readonly Font DefaultFont = new Font(FontFamily.GenericSansSerif, 14, FontStyle.Bold, GraphicsUnit.Pixel);


      /// <summary>
      /// font
      /// </summary>
      [NonSerialized]
      public Font Font = DefaultFont;

      public static readonly Pen DefaultStroke = new Pen(Color.FromArgb(140, Color.MidnightBlue));


      /// <summary>
      /// specifies how the outline is painted
      /// </summary>
      [NonSerialized]
      public Pen Stroke = DefaultStroke;

      public static readonly Brush DefaultFill = new SolidBrush(Color.FromArgb(222, Color.AliceBlue));


      /// <summary>
      /// background color
      /// </summary>
      [NonSerialized]
      public Brush Fill = DefaultFill;

      public static readonly Brush DefaultForeground = new SolidBrush(Color.Navy);

      /// <summary>
      /// text foreground
      /// </summary>
      [NonSerialized]
      public Brush Foreground = DefaultForeground;

      /// <summary>
      /// text padding
      /// </summary>
      public Size TextPadding = new Size(10, 10);

      static MapToolTip()
      {
          DefaultStroke.Width = 2;

          DefaultStroke.LineJoin = LineJoin.Round;
          DefaultStroke.StartCap = LineCap.RoundAnchor;

          DefaultFormat.LineAlignment = StringAlignment.Center;

          DefaultFormat.Alignment = StringAlignment.Center;
      }   

      public MapToolTip(MapSymbol marker)
      {
         this.Marker = marker;
         this.Offset = new Point(14, -44);
      }

      public virtual void OnRender(Graphics g)
      {
		 //g.TranslateTransform (this.Marker.Offset.X, this.Marker.Offset.Y);
         System.Drawing.Size st = g.MeasureString(Marker.ToolTipText, Font).ToSize();
         System.Drawing.Rectangle rect = new System.Drawing.Rectangle(Marker.ToolTipPosition.X, 
			                                                          Marker.ToolTipPosition.Y - st.Height, 
			                                                          st.Width + TextPadding.Width, 
			                                                          st.Height + TextPadding.Height);
         rect.Offset(Offset.X, Offset.Y);

         g.DrawLine(Stroke, Marker.ToolTipPosition.X, Marker.ToolTipPosition.Y, rect.X, rect.Y + rect.Height / 2);

         g.FillRectangle(Fill, rect);
         g.DrawRectangle(Stroke, rect);

         g.DrawString(Marker.ToolTipText, Font, Foreground, rect, Format);
      }


      #region IDisposable Members

      bool disposed = false;

      public void Dispose()
      {
         if(!disposed)
         {
            disposed = true;
         }
      }

      #endregion
   }
}
