namespace RSWikit
{

    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.tabSide = new MdiTabControl.TabControl();
            this.btnNewTab = new System.Windows.Forms.Button();
            this.pnlResize = new System.Windows.Forms.Panel();
            this.btnSwitch = new System.Windows.Forms.Button();
            this.btnFullScreen = new System.Windows.Forms.Button();
            this.pnlBG = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // tabSide
            // 
            this.tabSide.BackHighColor = System.Drawing.Color.DimGray;
            this.tabSide.BackLowColor = System.Drawing.Color.Black;
            this.tabSide.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabSide.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(224)))), ((int)(((byte)(240)))));
            this.tabSide.ForeColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(168)))), ((int)(((byte)(180)))));
            this.tabSide.Location = new System.Drawing.Point(0, 0);
            this.tabSide.Margin = new System.Windows.Forms.Padding(6);
            this.tabSide.MenuRenderer = null;
            this.tabSide.Name = "tabSide";
            this.tabSide.Size = new System.Drawing.Size(2852, 1660);
            this.tabSide.TabBackHighColor = System.Drawing.Color.Gray;
            this.tabSide.TabBackHighColorDisabled = System.Drawing.Color.DimGray;
            this.tabSide.TabBackLowColor = System.Drawing.Color.DimGray;
            this.tabSide.TabBackLowColorDisabled = System.Drawing.Color.Black;
            this.tabSide.TabCloseButtonImage = null;
            this.tabSide.TabCloseButtonImageDisabled = null;
            this.tabSide.TabCloseButtonImageHot = null;
            this.tabSide.TabIndex = 3;
            this.tabSide.TabMaximumWidth = 250;
            this.tabSide.TabTop = 4;
            this.tabSide.TopSeparator = false;
            // 
            // btnNewTab
            // 
            this.btnNewTab.BackColor = System.Drawing.Color.DimGray;
            this.btnNewTab.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNewTab.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNewTab.Location = new System.Drawing.Point(2660, 0);
            this.btnNewTab.Margin = new System.Windows.Forms.Padding(0);
            this.btnNewTab.Name = "btnNewTab";
            this.btnNewTab.Size = new System.Drawing.Size(64, 62);
            this.btnNewTab.TabIndex = 4;
            this.btnNewTab.Text = "➕";
            this.btnNewTab.UseVisualStyleBackColor = false;
            this.btnNewTab.Click += new System.EventHandler(this.btnNewTab_Click);
            // 
            // pnlResize
            // 
            this.pnlResize.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.pnlResize.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.pnlResize.Location = new System.Drawing.Point(1135, 656);
            this.pnlResize.Name = "pnlResize";
            this.pnlResize.Size = new System.Drawing.Size(4, 100);
            this.pnlResize.TabIndex = 6;
            this.pnlResize.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlResize_MouseDown);
            this.pnlResize.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlResize_MouseMove);
            this.pnlResize.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlResize_MouseUp);
            // 
            // btnSwitch
            // 
            this.btnSwitch.BackColor = System.Drawing.Color.DimGray;
            this.btnSwitch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSwitch.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSwitch.Location = new System.Drawing.Point(2788, 0);
            this.btnSwitch.Margin = new System.Windows.Forms.Padding(0);
            this.btnSwitch.Name = "btnSwitch";
            this.btnSwitch.Size = new System.Drawing.Size(64, 62);
            this.btnSwitch.TabIndex = 7;
            this.btnSwitch.Text = "⇄";
            this.btnSwitch.UseVisualStyleBackColor = false;
            this.btnSwitch.Click += new System.EventHandler(this.btnSwitch_Click);
            // 
            // btnFullScreen
            // 
            this.btnFullScreen.BackColor = System.Drawing.Color.DimGray;
            this.btnFullScreen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFullScreen.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFullScreen.Location = new System.Drawing.Point(2724, 0);
            this.btnFullScreen.Margin = new System.Windows.Forms.Padding(0);
            this.btnFullScreen.Name = "btnFullScreen";
            this.btnFullScreen.Size = new System.Drawing.Size(64, 62);
            this.btnFullScreen.TabIndex = 8;
            this.btnFullScreen.Text = "⛶";
            this.btnFullScreen.UseVisualStyleBackColor = false;
            this.btnFullScreen.Click += new System.EventHandler(this.btnFullScreen_Click);
            // 
            // pnlBG
            // 
            this.pnlBG.Location = new System.Drawing.Point(2660, 0);
            this.pnlBG.Name = "pnlBG";
            this.pnlBG.Size = new System.Drawing.Size(182, 64);
            this.pnlBG.TabIndex = 9;
            this.pnlBG.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlBG_Paint);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(2844, 1653);
            this.Controls.Add(this.btnFullScreen);
            this.Controls.Add(this.btnSwitch);
            this.Controls.Add(this.pnlResize);
            this.Controls.Add(this.btnNewTab);
            this.Controls.Add(this.tabSide);
            this.Controls.Add(this.pnlBG);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "frmMain";
            this.Text = "RS Wikit - The Unofficial RuneScape Wiki Toolkit";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Shown += new System.EventHandler(this.frmMain_Shown);
            this.ResizeEnd += new System.EventHandler(this.frmMain_ResizeEnd);
            this.Move += new System.EventHandler(this.frmMain_Move);
            this.Resize += new System.EventHandler(this.frmMain_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private MdiTabControl.TabControl tabSide;
        private System.Windows.Forms.Button btnNewTab;
        private System.Windows.Forms.Panel pnlResize;
        private System.Windows.Forms.Button btnSwitch;
        private System.Windows.Forms.Button btnFullScreen;
        private System.Windows.Forms.Panel pnlBG;
    }
}

