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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logbookToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.theathersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectTheaterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.campaignToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startNewCampaignToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewSetupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.StripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.internalsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewClassToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewHDRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.statusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.logbookToolStripMenuItem,
            this.theathersToolStripMenuItem,
            this.campaignToolStripMenuItem,
            this.setupToolStripMenuItem,
            this.internalsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1140, 24);
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
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // logbookToolStripMenuItem
            // 
            this.logbookToolStripMenuItem.Name = "logbookToolStripMenuItem";
            this.logbookToolStripMenuItem.Size = new System.Drawing.Size(66, 20);
            this.logbookToolStripMenuItem.Text = "Logbook";
            this.logbookToolStripMenuItem.Click += new System.EventHandler(this.logbookToolStripMenuItem_Click);
            // 
            // theathersToolStripMenuItem
            // 
            this.theathersToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectTheaterToolStripMenuItem});
            this.theathersToolStripMenuItem.Name = "theathersToolStripMenuItem";
            this.theathersToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.theathersToolStripMenuItem.Text = "Theathers";
            // 
            // selectTheaterToolStripMenuItem
            // 
            this.selectTheaterToolStripMenuItem.Name = "selectTheaterToolStripMenuItem";
            this.selectTheaterToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.selectTheaterToolStripMenuItem.Text = "Select Theater";
            this.selectTheaterToolStripMenuItem.Click += new System.EventHandler(this.selectTheaterToolStripMenuItem_Click);
            // 
            // campaignToolStripMenuItem
            // 
            this.campaignToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startNewCampaignToolStripMenuItem,
            this.playToolStripMenuItem});
            this.campaignToolStripMenuItem.Name = "campaignToolStripMenuItem";
            this.campaignToolStripMenuItem.Size = new System.Drawing.Size(74, 20);
            this.campaignToolStripMenuItem.Text = "Campaign";
            // 
            // startNewCampaignToolStripMenuItem
            // 
            this.startNewCampaignToolStripMenuItem.Name = "startNewCampaignToolStripMenuItem";
            this.startNewCampaignToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.startNewCampaignToolStripMenuItem.Text = "Start new Campaign";
            // 
            // playToolStripMenuItem
            // 
            this.playToolStripMenuItem.Name = "playToolStripMenuItem";
            this.playToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.playToolStripMenuItem.Text = "Play";
            this.playToolStripMenuItem.Click += new System.EventHandler(this.playToolStripMenuItem_Click);
            // 
            // setupToolStripMenuItem
            // 
            this.setupToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewSetupToolStripMenuItem});
            this.setupToolStripMenuItem.Name = "setupToolStripMenuItem";
            this.setupToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.setupToolStripMenuItem.Text = "Setup";
            // 
            // viewSetupToolStripMenuItem
            // 
            this.viewSetupToolStripMenuItem.Name = "viewSetupToolStripMenuItem";
            this.viewSetupToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.viewSetupToolStripMenuItem.Text = "View Setup";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StripStatusLabel});
            this.statusBar.Location = new System.Drawing.Point(0, 499);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(1140, 22);
            this.statusBar.TabIndex = 1;
            this.statusBar.Text = "statusStrip1";
            // 
            // StripStatusLabel
            // 
            this.StripStatusLabel.Name = "StripStatusLabel";
            this.StripStatusLabel.Size = new System.Drawing.Size(113, 17);
            this.StripStatusLabel.Text = "No Theater Selected";
            // 
            // internalsToolStripMenuItem
            // 
            this.internalsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewClassToolStripMenuItem,
            this.viewHDRToolStripMenuItem});
            this.internalsToolStripMenuItem.Name = "internalsToolStripMenuItem";
            this.internalsToolStripMenuItem.Size = new System.Drawing.Size(64, 20);
            this.internalsToolStripMenuItem.Text = "Internals";
            // 
            // viewClassToolStripMenuItem
            // 
            this.viewClassToolStripMenuItem.Name = "viewClassToolStripMenuItem";
            this.viewClassToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.viewClassToolStripMenuItem.Text = "View Class Table";
            this.viewClassToolStripMenuItem.Click += new System.EventHandler(this.viewClassToolStripMenuItem_Click);
            // 
            // viewHDRToolStripMenuItem
            // 
            this.viewHDRToolStripMenuItem.Name = "viewHDRToolStripMenuItem";
            this.viewHDRToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.viewHDRToolStripMenuItem.Text = "View HDR/LOD";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1140, 521);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Falcon4 Editor";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem theathersToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logbookToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem campaignToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectTheaterToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel StripStatusLabel;
        private System.Windows.Forms.ToolStripMenuItem startNewCampaignToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewSetupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem playToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem internalsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewClassToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewHDRToolStripMenuItem;
    }
}

