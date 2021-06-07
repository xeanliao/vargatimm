namespace GPS.Tool
{
    partial class frmFixInner
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
            this.tFix = new System.Windows.Forms.Timer(this.components);
            this.lblCurrent = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.pbFix = new System.Windows.Forms.ProgressBar();
            this.cbxTables = new System.Windows.Forms.ComboBox();
            this.btnFix = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Location = new System.Drawing.Point(495, 59);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(83, 13);
            this.lblTime.TabIndex = 15;
            this.lblTime.Text = "Times: 00:00:00";
            // 
            // lblInner
            // 
            this.lblInner.AutoSize = true;
            this.lblInner.Location = new System.Drawing.Point(38, 83);
            this.lblInner.Name = "lblInner";
            this.lblInner.Size = new System.Drawing.Size(35, 13);
            this.lblInner.TabIndex = 14;
            this.lblInner.Text = "label4";
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(38, 131);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(35, 13);
            this.lblMessage.TabIndex = 13;
            this.lblMessage.Text = "label3";
            // 
            // tFix
            // 
            this.tFix.Interval = 1000;
            this.tFix.Tick += new System.EventHandler(this.tFix_Tick);
            // 
            // lblCurrent
            // 
            this.lblCurrent.AutoSize = true;
            this.lblCurrent.Location = new System.Drawing.Point(38, 107);
            this.lblCurrent.Name = "lblCurrent";
            this.lblCurrent.Size = new System.Drawing.Size(35, 13);
            this.lblCurrent.TabIndex = 12;
            this.lblCurrent.Text = "label2";
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Location = new System.Drawing.Point(38, 59);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(35, 13);
            this.lblTotal.TabIndex = 11;
            this.lblTotal.Text = "label1";
            // 
            // pbFix
            // 
            this.pbFix.Location = new System.Drawing.Point(37, 198);
            this.pbFix.Name = "pbFix";
            this.pbFix.Size = new System.Drawing.Size(553, 25);
            this.pbFix.TabIndex = 10;
            // 
            // cbxTables
            // 
            this.cbxTables.FormattingEnabled = true;
            this.cbxTables.Location = new System.Drawing.Point(37, 30);
            this.cbxTables.Name = "cbxTables";
            this.cbxTables.Size = new System.Drawing.Size(336, 21);
            this.cbxTables.TabIndex = 9;
            // 
            // btnFix
            // 
            this.btnFix.Location = new System.Drawing.Point(379, 28);
            this.btnFix.Name = "btnFix";
            this.btnFix.Size = new System.Drawing.Size(211, 25);
            this.btnFix.TabIndex = 8;
            this.btnFix.Text = "Fix Inner";
            this.btnFix.UseVisualStyleBackColor = true;
            this.btnFix.Click += new System.EventHandler(this.btnFix_Click);
            // 
            // frmFixInner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(611, 271);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.lblInner);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.lblCurrent);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.pbFix);
            this.Controls.Add(this.cbxTables);
            this.Controls.Add(this.btnFix);
            this.Name = "frmFixInner";
            this.Text = "frmFixInner";
            this.Load += new System.EventHandler(this.frmFixInner_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Label lblInner;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Timer tFix;
        private System.Windows.Forms.Label lblCurrent;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.ProgressBar pbFix;
        private System.Windows.Forms.ComboBox cbxTables;
        private System.Windows.Forms.Button btnFix;
    }
}