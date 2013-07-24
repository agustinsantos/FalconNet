using FalconNet.FalcLib;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace F4Editor
{

    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public enum ControlsEnum
        {
            MAIN_CONTROL,
            LOGBOOK_CONTROL,
            THEATER_SELECTION_CONTROL,
            CAMPAIGN_DATA_CONTROL
        }

        private TheaterDef theTheater;
        public TheaterDef ActiveTheater
        {
            get { return theTheater; }
            set
            {
                if (value == null)
                    throw new ArgumentException();
                if (theTheater == null || value.Name != theTheater.Name)
                {
                    theTheater = value;
                    StripStatusLabel.Text = theTheater.Name;
                }
            }
        }

        private IDictionary<ControlsEnum, Control> controls = new Dictionary<ControlsEnum, Control>();

        private void MainForm_Load(object sender, EventArgs e)
        {
            ShowControl(ControlsEnum.MAIN_CONTROL);
        }

        private void selectTheaterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowControl(ControlsEnum.THEATER_SELECTION_CONTROL);
        }


        public void ShowControl(ControlsEnum ctrl)
        {
            Control new_ctrl = null;

            //If our dictionary already contains instance of
            if (controls.ContainsKey(ctrl))
            {
                new_ctrl = controls[ctrl];
            }
            else
            {
                new_ctrl = CreateControl(ctrl);
                controls[ctrl] = new_ctrl;
            }

            new_ctrl.Parent = this;
            new_ctrl.Dock = DockStyle.Fill;
            new_ctrl.BringToFront();
            new_ctrl.Show();
        }

        private Control CreateControl(ControlsEnum ctrl)
        {
            switch (ctrl)
            {
                case ControlsEnum.MAIN_CONTROL:
                    return new Background();
                case ControlsEnum.LOGBOOK_CONTROL:
                    return new Logbook();
                case ControlsEnum.THEATER_SELECTION_CONTROL:
                    return new TheatherSelection(this);
                case ControlsEnum.CAMPAIGN_DATA_CONTROL:
                    return new CampDataViewer();
                default:
                    return null;
            }
        }

        private void viewCampDataToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ShowControl(ControlsEnum.CAMPAIGN_DATA_CONTROL);
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Exit application?", "", MessageBoxButtons.YesNo) ==
       DialogResult.Yes)
            {
                // The user wants to exit the application. Close everything down.
                Application.Exit();
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AboutF4Net box = new AboutF4Net())
            {
                box.ShowDialog(this);
            }
        }

        private void logbookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowControl(ControlsEnum.LOGBOOK_CONTROL);
        }

    }
}

