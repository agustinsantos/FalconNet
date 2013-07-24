using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FalconNet.F4Common;
using FalconNet.FalcLib;
using FalconNet.Common;

namespace F4Editor
{
    public partial class TheatherSelection : UserControl
    {
        public static string FalconDirectory = @"C:\Falcon BMS 4.32\";
        public TheaterList tlist = null;
        SortableBindingList<TheaterDef> listData  =new SortableBindingList<TheaterDef>();
        private TheaterDef selectedRow;
        private MainForm parent;

        public TheatherSelection(MainForm p)
        {
            InitializeComponent();
            parent = p;
        }

        private void TheaterSelection_OnLoad(object sender, EventArgs e)
        {
            if (tlist == null)
            {
                F4File.FalconDirectory = FalconDirectory;
                tlist = new TheaterList();
                tlist.LoadTheaterList();

                foreach (TheaterDef theater in tlist)
                    listData.Add(theater);
            }
            TheatherListGridView.DataSource = listData;
        }

        private void TheaherSelected(object sender, EventArgs e)
        {
            if (TheatherListGridView.SelectedRows.Count == 1)
            {
                // get information of 1st column from the row
                selectedRow = (TheaterDef)TheatherListGridView.SelectedRows[0].DataBoundItem;
                TheaterImg.ImageLocation = F4File.F4FindFile(selectedRow.m_bitmap.Replace("tga","png"));
            }
        }

        private void ActivateTheater_Click(object sender, EventArgs e)
        {
            if (selectedRow == null)
                return;
            parent.ActiveTheater = selectedRow;
            parent.ShowControl(F4Editor.MainForm.ControlsEnum.MAIN_CONTROL);
        }

        private void ActivateTheater_DClick(object sender, EventArgs e)
        {
            if (TheatherListGridView.SelectedRows.Count == 1)
            {
                selectedRow = (TheaterDef)TheatherListGridView.SelectedRows[0].DataBoundItem;
                TheaterImg.ImageLocation = F4File.F4FindFile(selectedRow.m_bitmap.Replace("tga", "png"));
                parent.ActiveTheater = selectedRow;
            }

        }
    }
}
