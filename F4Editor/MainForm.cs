using FalconNet.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace F4Editor
{

    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
    public class Data
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
    SortableBindingList<Data> datos = new SortableBindingList<Data>{ new Data{Name="Dylan", Age=15},
                                           new Data{Name="Clark", Age=35},
                                           new Data{Name="Agatha", Age=23}, 
                                           new Data{Name="Alec", Age=67}};

        private void MainForm_Load(object sender, EventArgs e)
        {
            CTdataGridView.DataSource = datos;
        }
    }
}
