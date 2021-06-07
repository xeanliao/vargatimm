namespace GPS.Tool
{
    partial class ImportData
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportData));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnOpenFile = new System.Windows.Forms.ToolStripButton();
            this.btnZoomOut = new System.Windows.Forms.ToolStripButton();
            this.btnZoomIn = new System.Windows.Forms.ToolStripButton();
            this.tsbMapping = new System.Windows.Forms.ToolStripButton();
            this.tsbFix = new System.Windows.Forms.ToolStripButton();
            this.tsbSelectMapping = new System.Windows.Forms.ToolStripButton();
            this.trvFile = new System.Windows.Forms.TreeView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtProgress = new System.Windows.Forms.TextBox();
            this.cbClass = new System.Windows.Forms.ComboBox();
            this.lbClass = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.lbCurrentFile = new System.Windows.Forms.Label();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.lbFolderpath = new System.Windows.Forms.Label();
            this.btnImportData = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.tsbFixInner = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.Location = new System.Drawing.Point(259, 22);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(894, 509);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnOpenFile,
            this.btnZoomOut,
            this.btnZoomIn,
            this.tsbMapping,
            this.tsbFix,
            this.tsbSelectMapping,
            this.tsbFixInner});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1028, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Image = ((System.Drawing.Image)(resources.GetObject("btnOpenFile.Image")));
            this.btnOpenFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(72, 22);
            this.btnOpenFile.Text = "Open File";
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Image = ((System.Drawing.Image)(resources.GetObject("btnZoomOut.Image")));
            this.btnZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(74, 22);
            this.btnZoomOut.Text = "Zoom Out";
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.Image = ((System.Drawing.Image)(resources.GetObject("btnZoomIn.Image")));
            this.btnZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(66, 22);
            this.btnZoomIn.Text = "Zoom In";
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // tsbMapping
            // 
            this.tsbMapping.Image = ((System.Drawing.Image)(resources.GetObject("tsbMapping.Image")));
            this.tsbMapping.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbMapping.Name = "tsbMapping";
            this.tsbMapping.Size = new System.Drawing.Size(67, 22);
            this.tsbMapping.Text = "Mapping";
            this.tsbMapping.Click += new System.EventHandler(this.tsbMapping_Click);
            // 
            // tsbFix
            // 
            this.tsbFix.Image = ((System.Drawing.Image)(resources.GetObject("tsbFix.Image")));
            this.tsbFix.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbFix.Name = "tsbFix";
            this.tsbFix.Size = new System.Drawing.Size(67, 22);
            this.tsbFix.Text = "Fix Data";
            this.tsbFix.Click += new System.EventHandler(this.tsbFix_Click);
            // 
            // tsbSelectMapping
            // 
            this.tsbSelectMapping.Image = ((System.Drawing.Image)(resources.GetObject("tsbSelectMapping.Image")));
            this.tsbSelectMapping.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSelectMapping.Name = "tsbSelectMapping";
            this.tsbSelectMapping.Size = new System.Drawing.Size(99, 22);
            this.tsbSelectMapping.Text = "Select Mapping";
            this.tsbSelectMapping.Click += new System.EventHandler(this.tsbSelectMapping_Click);
            // 
            // trvFile
            // 
            this.trvFile.Location = new System.Drawing.Point(6, 22);
            this.trvFile.Name = "trvFile";
            this.trvFile.Size = new System.Drawing.Size(247, 532);
            this.trvFile.TabIndex = 3;
            this.trvFile.DoubleClick += new System.EventHandler(this.trvFile_DoubleClick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtProgress);
            this.panel1.Controls.Add(this.cbClass);
            this.panel1.Controls.Add(this.lbClass);
            this.panel1.Controls.Add(this.btnBrowse);
            this.panel1.Controls.Add(this.lbCurrentFile);
            this.panel1.Controls.Add(this.txtFilePath);
            this.panel1.Controls.Add(this.lbFolderpath);
            this.panel1.Controls.Add(this.btnImportData);
            this.panel1.Location = new System.Drawing.Point(12, 30);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1153, 155);
            this.panel1.TabIndex = 4;
            // 
            // txtProgress
            // 
            this.txtProgress.Location = new System.Drawing.Point(659, 12);
            this.txtProgress.Multiline = true;
            this.txtProgress.Name = "txtProgress";
            this.txtProgress.ReadOnly = true;
            this.txtProgress.Size = new System.Drawing.Size(482, 105);
            this.txtProgress.TabIndex = 8;
            // 
            // cbClass
            // 
            this.cbClass.FormattingEnabled = true;
            this.cbClass.Location = new System.Drawing.Point(171, 12);
            this.cbClass.Name = "cbClass";
            this.cbClass.Size = new System.Drawing.Size(471, 21);
            this.cbClass.TabIndex = 7;
            // 
            // lbClass
            // 
            this.lbClass.AutoSize = true;
            this.lbClass.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbClass.Location = new System.Drawing.Point(4, 12);
            this.lbClass.Name = "lbClass";
            this.lbClass.Size = new System.Drawing.Size(159, 12);
            this.lbClass.TabIndex = 6;
            this.lbClass.Text = "Select Classification:";
            this.lbClass.Click += new System.EventHandler(this.label1_Click);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(536, 48);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(106, 25);
            this.btnBrowse.TabIndex = 5;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // lbCurrentFile
            // 
            this.lbCurrentFile.AutoSize = true;
            this.lbCurrentFile.Location = new System.Drawing.Point(4, 130);
            this.lbCurrentFile.Name = "lbCurrentFile";
            this.lbCurrentFile.Size = new System.Drawing.Size(65, 13);
            this.lbCurrentFile.TabIndex = 4;
            this.lbCurrentFile.Text = "lbCurrentFile";
            // 
            // txtFilePath
            // 
            this.txtFilePath.Location = new System.Drawing.Point(171, 50);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.ReadOnly = true;
            this.txtFilePath.Size = new System.Drawing.Size(359, 20);
            this.txtFilePath.TabIndex = 3;
            // 
            // lbFolderpath
            // 
            this.lbFolderpath.AutoSize = true;
            this.lbFolderpath.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbFolderpath.Location = new System.Drawing.Point(4, 53);
            this.lbFolderpath.Name = "lbFolderpath";
            this.lbFolderpath.Size = new System.Drawing.Size(103, 12);
            this.lbFolderpath.TabIndex = 2;
            this.lbFolderpath.Text = "Select Folder:";
            // 
            // btnImportData
            // 
            this.btnImportData.Location = new System.Drawing.Point(536, 92);
            this.btnImportData.Name = "btnImportData";
            this.btnImportData.Size = new System.Drawing.Size(106, 25);
            this.btnImportData.TabIndex = 1;
            this.btnImportData.Text = "Import Data";
            this.btnImportData.UseVisualStyleBackColor = true;
            this.btnImportData.Click += new System.EventHandler(this.btnImportData_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.trvFile);
            this.groupBox1.Controls.Add(this.pictureBox1);
            this.groupBox1.Location = new System.Drawing.Point(12, 192);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1153, 563);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Shape";
            // 
            // tsbFixInner
            // 
            this.tsbFixInner.Image = ((System.Drawing.Image)(resources.GetObject("tsbFixInner.Image")));
            this.tsbFixInner.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbFixInner.Name = "tsbFixInner";
            this.tsbFixInner.Size = new System.Drawing.Size(70, 22);
            this.tsbFixInner.Text = "Fix Inner";
            this.tsbFixInner.Click += new System.EventHandler(this.tsbFixInner_Click);
            // 
            // ImportData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1028, 741);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "ImportData";
            this.Text = "TIMM Management Studio";
            this.Load += new System.EventHandler(this.ImportData_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnOpenFile;
        private System.Windows.Forms.ToolStripButton btnZoomOut;
        private System.Windows.Forms.ToolStripButton btnZoomIn;
        private System.Windows.Forms.TreeView trvFile;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbCurrentFile;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Label lbFolderpath;
        private System.Windows.Forms.Button btnImportData;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label lbClass;
        private System.Windows.Forms.ComboBox cbClass;
        private System.Windows.Forms.ToolStripButton tsbMapping;
        private System.Windows.Forms.TextBox txtProgress;
        private System.Windows.Forms.ToolStripButton tsbFix;
        private System.Windows.Forms.ToolStripButton tsbSelectMapping;
        private System.Windows.Forms.ToolStripButton tsbFixInner;
    }
}

