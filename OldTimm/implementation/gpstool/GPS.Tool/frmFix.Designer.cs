namespace GPS.Tool
{
    partial class frmFix
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
            this.btnFix = new System.Windows.Forms.Button();
            this.cbxTables = new System.Windows.Forms.ComboBox();
            this.pbFix = new System.Windows.Forms.ProgressBar();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lblCurrent = new System.Windows.Forms.Label();
            this.lblMessage = new System.Windows.Forms.Label();
            this.lblInner = new System.Windows.Forms.Label();
            this.tFix = new System.Windows.Forms.Timer(this.components);
            this.lblTime = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnFix
            // 
            this.btnFix.Location = new System.Drawing.Point(354, 12);
            this.btnFix.Name = "btnFix";
            this.btnFix.Size = new System.Drawing.Size(211, 23);
            this.btnFix.TabIndex = 0;
            this.btnFix.Text = "Fix Data";
            this.btnFix.UseVisualStyleBackColor = true;
            this.btnFix.Click += new System.EventHandler(this.btnFix_Click);
            // 
            // cbxTables
            // 
            this.cbxTables.FormattingEnabled = true;
            this.cbxTables.Location = new System.Drawing.Point(12, 14);
            this.cbxTables.Name = "cbxTables";
            this.cbxTables.Size = new System.Drawing.Size(336, 20);
            this.cbxTables.TabIndex = 1;
            // 
            // pbFix
            // 
            this.pbFix.Location = new System.Drawing.Point(12, 131);
            this.pbFix.Name = "pbFix";
            this.pbFix.Size = new System.Drawing.Size(553, 23);
            this.pbFix.TabIndex = 2;
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(13, 41);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(41, 12);
            this.lblTotal.TabIndex = 3;
            this.lblTotal.Text = "label1";
            // 
            // lblCurrent
            // 
            this.lblCurrent.AutoSize = true;
            this.lblCurrent.Location = new System.Drawing.Point(13, 85);
            this.lblCurrent.Name = "lblCurrent";
            this.lblCurrent.Size = new System.Drawing.Size(41, 12);
            this.lblCurrent.TabIndex = 4;
            this.lblCurrent.Text = "label2";
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(13, 107);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(41, 12);
            this.lblMessage.TabIndex = 5;
            this.lblMessage.Text = "label3";
            // 
            // lblInner
            // 
            this.lblInner.AutoSize = true;
            this.lblInner.Location = new System.Drawing.Point(13, 63);
            this.lblInner.Name = "lblInner";
            this.lblInner.Size = new System.Drawing.Size(41, 12);
            this.lblInner.TabIndex = 6;
            this.lblInner.Text = "label4";
            // 
            // tFix
            // 
            this.tFix.Interval = 1000;
            this.tFix.Tick += new System.EventHandler(this.tFix_Tick);
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Location = new System.Drawing.Point(470, 41);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(95, 12);
            this.lblTime.TabIndex = 7;
            this.lblTime.Text = "Times: 00:00:00";
            // 
            // frmFix
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(577, 166);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.lblInner);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.lblCurrent);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.pbFix);
            this.Controls.Add(this.cbxTables);
            this.Controls.Add(this.btnFix);
            this.Name = "frmFix";
            this.Text = "frmFix";
            this.Load += new System.EventHandler(this.frmFix_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnFix;
        private System.Windows.Forms.ComboBox cbxTables;
        private System.Windows.Forms.ProgressBar pbFix;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label lblCurrent;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Label lblInner;
        private System.Windows.Forms.Timer tFix;
        private System.Windows.Forms.Label lblTime;
    }
}