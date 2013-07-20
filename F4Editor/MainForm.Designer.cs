namespace F4Editor
{
    partial class MainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.theathersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.ClassData = new System.Windows.Forms.TabPage();
            this.CTdataGridView = new System.Windows.Forms.DataGridView();
            this.ObjectiveData = new System.Windows.Forms.TabPage();
            this.bindingSourceClassData = new System.Windows.Forms.BindingSource(this.components);
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.ClassData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CTdataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceClassData)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.theathersToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(955, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(97, 22);
            this.quitToolStripMenuItem.Text = "Quit";
            // 
            // theathersToolStripMenuItem
            // 
            this.theathersToolStripMenuItem.Name = "theathersToolStripMenuItem";
            this.theathersToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.theathersToolStripMenuItem.Text = "Theathers";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 330);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(955, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.ClassData);
            this.tabControl1.Controls.Add(this.ObjectiveData);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(955, 306);
            this.tabControl1.TabIndex = 2;
            // 
            // ClassData
            // 
            this.ClassData.Controls.Add(this.CTdataGridView);
            this.ClassData.Location = new System.Drawing.Point(4, 22);
            this.ClassData.Name = "ClassData";
            this.ClassData.Padding = new System.Windows.Forms.Padding(3);
            this.ClassData.Size = new System.Drawing.Size(947, 280);
            this.ClassData.TabIndex = 0;
            this.ClassData.Text = "Class Data";
            this.ClassData.UseVisualStyleBackColor = true;
            // 
            // CTdataGridView
            // 
            this.CTdataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.CTdataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.CTdataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CTdataGridView.Location = new System.Drawing.Point(3, 3);
            this.CTdataGridView.Name = "CTdataGridView";
            this.CTdataGridView.ReadOnly = true;
            this.CTdataGridView.RowHeadersVisible = false;
            this.CTdataGridView.Size = new System.Drawing.Size(941, 274);
            this.CTdataGridView.TabIndex = 0;
            // 
            // ObjectiveData
            // 
            this.ObjectiveData.Location = new System.Drawing.Point(4, 22);
            this.ObjectiveData.Name = "ObjectiveData";
            this.ObjectiveData.Padding = new System.Windows.Forms.Padding(3);
            this.ObjectiveData.Size = new System.Drawing.Size(947, 280);
            this.ObjectiveData.TabIndex = 1;
            this.ObjectiveData.Text = "Objectives";
            this.ObjectiveData.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(955, 352);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Falcon4 Editor";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.ClassData.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.CTdataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSourceClassData)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem theathersToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage ClassData;
        private System.Windows.Forms.TabPage ObjectiveData;
        private System.Windows.Forms.DataGridView CTdataGridView;
        private System.Windows.Forms.BindingSource bindingSourceClassData;
    }
}

