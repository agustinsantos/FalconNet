namespace F4Editor
{
    partial class TheatherSelection
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
            this.TheaterImg = new System.Windows.Forms.PictureBox();
            this.TheatherListGridView = new System.Windows.Forms.DataGridView();
            this.SelectTheaterButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.TheaterImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TheatherListGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // TheaterImg
            // 
            this.TheaterImg.Location = new System.Drawing.Point(748, 3);
            this.TheaterImg.Name = "TheaterImg";
            this.TheaterImg.Size = new System.Drawing.Size(852, 400);
            this.TheaterImg.TabIndex = 0;
            this.TheaterImg.TabStop = false;
            // 
            // TheatherListGridView
            // 
            this.TheatherListGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.TheatherListGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            this.TheatherListGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.TheatherListGridView.Dock = System.Windows.Forms.DockStyle.Left;
            this.TheatherListGridView.Location = new System.Drawing.Point(0, 0);
            this.TheatherListGridView.Name = "TheatherListGridView";
            this.TheatherListGridView.ReadOnly = true;
            this.TheatherListGridView.RowHeadersVisible = false;
            this.TheatherListGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.TheatherListGridView.Size = new System.Drawing.Size(742, 400);
            this.TheatherListGridView.TabIndex = 1;
            this.TheatherListGridView.Click += new System.EventHandler(this.TheaherSelected);
            this.TheatherListGridView.DoubleClick += new System.EventHandler(this.ActivateTheater_DClick);
            // 
            // SelectTheaterButton
            // 
            this.SelectTheaterButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.SelectTheaterButton.Location = new System.Drawing.Point(742, 365);
            this.SelectTheaterButton.Name = "SelectTheaterButton";
            this.SelectTheaterButton.Size = new System.Drawing.Size(108, 35);
            this.SelectTheaterButton.TabIndex = 2;
            this.SelectTheaterButton.Text = "OK";
            this.SelectTheaterButton.UseVisualStyleBackColor = true;
            this.SelectTheaterButton.Click += new System.EventHandler(this.ActivateTheater_Click);
            // 
            // TheatherSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.SelectTheaterButton);
            this.Controls.Add(this.TheaterImg);
            this.Controls.Add(this.TheatherListGridView);
            this.Name = "TheatherSelection";
            this.Size = new System.Drawing.Size(850, 400);
            this.Load += new System.EventHandler(this.TheaterSelection_OnLoad);
            ((System.ComponentModel.ISupportInitialize)(this.TheaterImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TheatherListGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox TheaterImg;
        private System.Windows.Forms.DataGridView TheatherListGridView;
        private System.Windows.Forms.Button SelectTheaterButton;
    }
}
