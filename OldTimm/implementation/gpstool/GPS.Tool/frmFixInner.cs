using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GPS.Tool.FixInner;

namespace GPS.Tool
{
    public partial class frmFixInner : Form
    {
        int times;
        public frmFixInner()
        {
            InitializeComponent();
        }

        private void frmFixInner_Load(object sender, EventArgs e)
        {
            cbxTables.Items.Clear();
            foreach (FixInnerTables table in Enum.GetValues(typeof(FixInnerTables)))
            {
                cbxTables.Items.Add(table);
            }
        }

        private void btnFix_Click(object sender, EventArgs e)
        {
            if (cbxTables.Text != "")
            {
                FixInnerTables table = (FixInnerTables)Enum.Parse(typeof(FixInnerTables), cbxTables.Text);
                FixerInnerBase fixer = FixerInnerFactory.CreateFixer(table);
                fixer.Messaging += new FixerInnerBase.MessageHandler(fixer_Messaging);
                SetEnabled(false);
                fixer.StartFix();
                times = 0;
                tFix.Enabled = true;
            }
        }

        void fixer_Messaging(int total, int current, int innerCount, string code, bool inner, bool completed)
        {
            pbFix.Minimum = 0;
            pbFix.Maximum = total;
            pbFix.Value = current;
            lblTotal.Text = string.Format("Total: {0}", total);
            lblInner.Text = string.Format("Inner: {0}", innerCount);
            lblCurrent.Text = string.Format("Current: {0}", current);
            lblMessage.Text = string.Format("Current Code: {0}, HasInner: {1}", code, inner);
            if (completed)
            {
                tFix.Enabled = false;
                SetEnabled(true);
                MessageBox.Show("Completed");
            }
        }

        void SetEnabled(bool enabled)
        {
            cbxTables.Enabled = enabled;
            btnFix.Enabled = enabled;
        }

        private void tFix_Tick(object sender, EventArgs e)
        {
            times++;
            lblTime.Text = string.Format("Times: {0}", TimeSpan.FromSeconds(times));
        }
    }
}
