using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ImageZoom.Windows.Forms;

namespace F4Editor
{
    public partial class CampaignGame : UserControl
    {
        public CampaignGame()
        {
            InitializeComponent();
        }

        private void CampaignGame_Load(object sender, EventArgs e)
        {
            MapSymbol symbol = new MapSymbol(new Point(150, 150), @"brwar_ee\icon_ah64.png");
            string imagefilename = @"campmap\big_map_id.png";
            Image img = Image.FromFile(imagefilename);
            this.mapControl.Img = img;
            this.mapControl.Overlays.Add(symbol);
            symbol.Position = new Point(150, 150);
            symbol.ToolTipText = "A symbol";
        }
    }
}
