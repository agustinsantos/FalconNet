namespace F4Editor
{
    partial class CampDataViewer
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabClassData = new System.Windows.Forms.TabPage();
            this.campDataGridView = new System.Windows.Forms.DataGridView();
            this.tabObjetives = new System.Windows.Forms.TabPage();
            this.tabUnits = new System.Windows.Forms.TabPage();
            this.tabSensors = new System.Windows.Forms.TabPage();
            this.tabVehicles = new System.Windows.Forms.TabPage();
            this.tabWeapons = new System.Windows.Forms.TabPage();
            this.tabControl1.SuspendLayout();
            this.tabClassData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.campDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabClassData);
            this.tabControl1.Controls.Add(this.tabObjetives);
            this.tabControl1.Controls.Add(this.tabUnits);
            this.tabControl1.Controls.Add(this.tabSensors);
            this.tabControl1.Controls.Add(this.tabVehicles);
            this.tabControl1.Controls.Add(this.tabWeapons);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(850, 400);
            this.tabControl1.TabIndex = 0;
            // 
            // tabClassData
            // 
            this.tabClassData.Controls.Add(this.campDataGridView);
            this.tabClassData.Location = new System.Drawing.Point(4, 22);
            this.tabClassData.Name = "tabClassData";
            this.tabClassData.Padding = new System.Windows.Forms.Padding(3);
            this.tabClassData.Size = new System.Drawing.Size(842, 374);
            this.tabClassData.TabIndex = 0;
            this.tabClassData.Text = "Class Data";
            this.tabClassData.UseVisualStyleBackColor = true;
            // 
            // campDataGridView
            // 
            this.campDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.campDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.campDataGridView.Location = new System.Drawing.Point(3, 3);
            this.campDataGridView.Name = "campDataGridView";
            this.campDataGridView.Size = new System.Drawing.Size(836, 368);
            this.campDataGridView.TabIndex = 0;
            // 
            // tabObjetives
            // 
            this.tabObjetives.Location = new System.Drawing.Point(4, 22);
            this.tabObjetives.Name = "tabObjetives";
            this.tabObjetives.Padding = new System.Windows.Forms.Padding(3);
            this.tabObjetives.Size = new System.Drawing.Size(808, 333);
            this.tabObjetives.TabIndex = 1;
            this.tabObjetives.Text = "Objetives";
            this.tabObjetives.UseVisualStyleBackColor = true;
            // 
            // tabUnits
            // 
            this.tabUnits.Location = new System.Drawing.Point(4, 22);
            this.tabUnits.Name = "tabUnits";
            this.tabUnits.Size = new System.Drawing.Size(808, 333);
            this.tabUnits.TabIndex = 2;
            this.tabUnits.Text = "Units";
            this.tabUnits.UseVisualStyleBackColor = true;
            // 
            // tabSensors
            // 
            this.tabSensors.Location = new System.Drawing.Point(4, 22);
            this.tabSensors.Name = "tabSensors";
            this.tabSensors.Size = new System.Drawing.Size(808, 333);
            this.tabSensors.TabIndex = 3;
            this.tabSensors.Text = "Sensors";
            this.tabSensors.UseVisualStyleBackColor = true;
            // 
            // tabVehicles
            // 
            this.tabVehicles.Location = new System.Drawing.Point(4, 22);
            this.tabVehicles.Name = "tabVehicles";
            this.tabVehicles.Size = new System.Drawing.Size(808, 333);
            this.tabVehicles.TabIndex = 4;
            this.tabVehicles.Text = "Vehicles";
            this.tabVehicles.UseVisualStyleBackColor = true;
            // 
            // tabWeapons
            // 
            this.tabWeapons.Location = new System.Drawing.Point(4, 22);
            this.tabWeapons.Name = "tabWeapons";
            this.tabWeapons.Size = new System.Drawing.Size(808, 333);
            this.tabWeapons.TabIndex = 5;
            this.tabWeapons.Text = "Weapons";
            this.tabWeapons.UseVisualStyleBackColor = true;
            // 
            // CampDataViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Name = "CampDataViewer";
            this.Size = new System.Drawing.Size(850, 400);
            this.tabControl1.ResumeLayout(false);
            this.tabClassData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.campDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabClassData;
        private System.Windows.Forms.TabPage tabObjetives;
        private System.Windows.Forms.DataGridView campDataGridView;
        private System.Windows.Forms.TabPage tabUnits;
        private System.Windows.Forms.TabPage tabSensors;
        private System.Windows.Forms.TabPage tabVehicles;
        private System.Windows.Forms.TabPage tabWeapons;
    }
}
