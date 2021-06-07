using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GPS.Tool.SelectMapping;

namespace GPS.Tool
{
    public partial class frmSelectMapping : Form
    {
        int times = 0;
        public frmSelectMapping()
        {
            InitializeComponent();
        }

        private void frmSelectMapping_Load(object sender, EventArgs e)
        {
            cbxTables.Items.Clear();
            foreach (SelectMappingTables table in Enum.GetValues(typeof(SelectMappingTables)))
            {
                cbxTables.Items.Add(table);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            SelectMappingTables table = (SelectMappingTables)Enum.Parse(typeof(SelectMappingTables), cbxTables.Text);
            MakerBase maker = SelectMappingMakerFactory.CreateMaker(table);
            maker.Messaging += new MakerBase.MessageHandler(maker_Messaging);
            maker.StartMake();
            times = 0;
            tMapping.Enabled = true;
        }

        void maker_Messaging(int total, int current, int innerCount, string code, bool inner, bool completed)
        {
            pbMapping.Minimum = 0;
            pbMapping.Maximum = total;
            pbMapping.Value = current;
            lblTotal.Text = string.Format("Total: {0}", total);
            lblInner.Text = string.Format("Inner: {0}", innerCount);
            lblCurrent.Text = string.Format("Current: {0}", current);
            lblMessage.Text = string.Format("Current Code: {0}, Inner: {1}", code, inner);
            if (completed)
            {
                tMapping.Enabled = false;
                SetEnabled(true);
                MessageBox.Show("Completed");
            }
        }

        void SetEnabled(bool enabled)
        {
            cbxTables.Enabled = enabled;
            btnStart.Enabled = enabled;
        }

        private void tMapping_Tick(object sender, EventArgs e)
        {
            times++;
            lblTime.Text = string.Format("Times: {0}", TimeSpan.FromSeconds(times));
        }
    }
}
