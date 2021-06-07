namespace GPS.Tool
{
    partial class frmSelectMapping
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
            this.lblTime = new System.Windows.Forms.Label();
            this.lblInner = new System.Windows.Forms.Label();
            this.lblMessage = new System.Windows.Forms.Label();
            this.tMapping = new System.Windows.Forms.Timer(this.components);
            this.lblCurrent = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.pbMapping = new System.Windows.Forms.ProgressBar();
            this.cbxTables = new System.Windows.Forms.ComboBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Location = new System.Drawing.Point(468, 36);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(95, 12);
            this.lblTime.TabIndex = 15;
            this.lblTime.Text = "Times: 00:00:00";
            // 
            // lblInner
            // 
            this.lblInner.AutoSize = true;
            this.lblInner.Location = new System.Drawing.Point(11, 58);
            this.lblInner.Name = "lblInner";
            this.lblInner.Size = new System.Drawing.Size(53, 12);
            this.lblInner.TabIndex = 14;
            this.lblInner.Text = "Inner: 0";
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(11, 102);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(53, 12);
            this.lblMessage.TabIndex = 13;
            this.lblMessage.Text = "No start";
            // 
            // tMapping
            // 
            this.tMapping.Interval = 1000;
            this.tMapping.Tick += new System.EventHandler(this.tMapping_Tick);
            // 
            // lblCurrent
            // 
            this.lblCurrent.AutoSize = true;
            this.lblCurrent.Location = new System.Drawing.Point(11, 80);
            this.lblCurrent.Name = "lblCurrent";
            this.lblCurrent.Size = new System.Drawing.Size(65, 12);
            this.lblCurrent.TabIndex = 12;
            this.lblCurrent.Text = "Current: 0";
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(11, 36);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(53, 12);
            this.lblTotal.TabIndex = 11;
            this.lblTotal.Text = "Total: 0";
            // 
            // pbMapping
            // 
            this.pbMapping.Location = new System.Drawing.Point(10, 126);
            this.pbMapping.Name = "pbMapping";
            this.pbMapping.Size = new System.Drawing.Size(553, 23);
            this.pbMapping.TabIndex = 10;
            // 
            // cbxTables
            // 
            this.cbxTables.FormattingEnabled = true;
            this.cbxTables.Location = new System.Drawing.Point(10, 9);
            this.cbxTables.Name = "cbxTables";
            this.cbxTables.Size = new System.Drawing.Size(336, 20);
            this.cbxTables.TabIndex = 9;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(352, 7);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(211, 23);
            this.btnStart.TabIndex = 8;
            this.btnStart.Text = "Mark Mapping";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // frmSelectMapping
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(573, 156);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.lblInner);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.lblCurrent);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.pbMapping);
            this.Controls.Add(this.cbxTables);
            this.Controls.Add(this.btnStart);
            this.Name = "frmSelectMapping";
            this.Text = "frmSelectMapping";
            this.Load += new System.EventHandler(this.frmSelectMapping_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Label lblInner;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Timer tMapping;
        private System.Windows.Forms.Label lblCurrent;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.ProgressBar pbMapping;
        private System.Windows.Forms.ComboBox cbxTables;
        private System.Windows.Forms.Button btnStart;

    }
}