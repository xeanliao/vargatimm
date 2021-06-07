namespace GPS.Tool
{
    partial class frmMakeMapping
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
            this.btnMake = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbxMappingTables = new System.Windows.Forms.ComboBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lblFinished = new System.Windows.Forms.Label();
            this.lblSuccessed = new System.Windows.Forms.Label();
            this.lblAccounts = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbxResults = new System.Windows.Forms.ListBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnMake
            // 
            this.btnMake.Location = new System.Drawing.Point(340, 12);
            this.btnMake.Name = "btnMake";
            this.btnMake.Size = new System.Drawing.Size(112, 21);
            this.btnMake.TabIndex = 5;
            this.btnMake.Text = "Make Mapping";
            this.btnMake.UseVisualStyleBackColor = true;
            this.btnMake.Click += new System.EventHandler(this.btnMake_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "Mapping Table";
            // 
            // cbxMappingTables
            // 
            this.cbxMappingTables.FormattingEnabled = true;
            this.cbxMappingTables.Items.AddRange(new object[] {
            "BlockGroupMappings"});
            this.cbxMappingTables.Location = new System.Drawing.Point(95, 15);
            this.cbxMappingTables.Name = "cbxMappingTables";
            this.cbxMappingTables.Size = new System.Drawing.Size(217, 20);
            this.cbxMappingTables.TabIndex = 3;
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(477, 12);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 21);
            this.btnStop.TabIndex = 8;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(15, 36);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(41, 12);
            this.lblTotal.TabIndex = 7;
            this.lblTotal.Text = "Total:";
            // 
            // lblFinished
            // 
            this.lblFinished.AutoSize = true;
            this.lblFinished.Location = new System.Drawing.Point(351, 36);
            this.lblFinished.Name = "lblFinished";
            this.lblFinished.Size = new System.Drawing.Size(59, 12);
            this.lblFinished.TabIndex = 8;
            this.lblFinished.Text = "Finished:";
            // 
            // lblSuccessed
            // 
            this.lblSuccessed.AutoSize = true;
            this.lblSuccessed.Location = new System.Drawing.Point(173, 36);
            this.lblSuccessed.Name = "lblSuccessed";
            this.lblSuccessed.Size = new System.Drawing.Size(65, 12);
            this.lblSuccessed.TabIndex = 9;
            this.lblSuccessed.Text = "Successed:";
            // 
            // lblAccounts
            // 
            this.lblAccounts.AutoSize = true;
            this.lblAccounts.Location = new System.Drawing.Point(506, 36);
            this.lblAccounts.Name = "lblAccounts";
            this.lblAccounts.Size = new System.Drawing.Size(41, 12);
            this.lblAccounts.TabIndex = 10;
            this.lblAccounts.Text = "label2";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblAccounts);
            this.groupBox1.Controls.Add(this.lblSuccessed);
            this.groupBox1.Controls.Add(this.lblFinished);
            this.groupBox1.Controls.Add(this.lblTotal);
            this.groupBox1.Controls.Add(this.lbxResults);
            this.groupBox1.Location = new System.Drawing.Point(11, 49);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(749, 469);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Results";
            // 
            // lbxResults
            // 
            this.lbxResults.FormattingEnabled = true;
            this.lbxResults.ItemHeight = 12;
            this.lbxResults.Location = new System.Drawing.Point(18, 72);
            this.lbxResults.Name = "lbxResults";
            this.lbxResults.Size = new System.Drawing.Size(688, 376);
            this.lbxResults.TabIndex = 6;
            // 
            // frmMakeMapping
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 529);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnMake);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbxMappingTables);
            this.Name = "frmMakeMapping";
            this.Text = "Make Mapping";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMakeMapping_FormClosed);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnMake;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbxMappingTables;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label lblFinished;
        private System.Windows.Forms.Label lblSuccessed;
        private System.Windows.Forms.Label lblAccounts;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox lbxResults;
    }
}