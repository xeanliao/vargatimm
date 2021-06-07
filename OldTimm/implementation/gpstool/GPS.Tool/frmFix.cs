using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GPS.Tool.FixData;

namespace GPS.Tool
{
    public partial class frmFix : Form
    {
        int times;
        public frmFix()
        {
            InitializeComponent();
        }

        private void frmFix_Load(object sender, EventArgs e)
        {
            cbxTables.Items.Clear();
            foreach (FixTables table in Enum.GetValues(typeof(FixTables)))
            {
                cbxTables.Items.Add(table);
            }
        }

        private void btnFix_Click(object sender, EventArgs e)
        {
            FixTables table = (FixTables)Enum.Parse(typeof(FixTables), cbxTables.Text);
            FixerBase fixer = FixerFactory.CreateFixer(table);
            fixer.Messaging += new FixerBase.MessageHandler(fixer_Messaging);
            SetEnabled(false);
            fixer.StartFix();
            times = 0;
            tFix.Enabled = true;
        }

        void fixer_Messaging(int total, int current, int innerCount, string code, bool inner, bool completed)
        {
            pbFix.Minimum = 0;
            pbFix.Maximum = total;
            pbFix.Value = current;
            lblTotal.Text = string.Format("Total: {0}", total);
            lblInner.Text = string.Format("Inner: {0}", innerCount);
            lblCurrent.Text = string.Format("Current: {0}", current);
            lblMessage.Text = string.Format("Current Code: {0}, Inner: {1}", code, inner);
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
