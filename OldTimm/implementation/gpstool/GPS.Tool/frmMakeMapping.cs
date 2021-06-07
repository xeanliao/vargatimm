using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GPS.Tool.Mapping;

namespace GPS.Tool
{
    public partial class frmMakeMapping : Form
    {
        MappingMaker maker;
        public frmMakeMapping()
        {
            InitializeComponent(); 
            InitMappingTables();
            InitializeButton(false,false);
        }

        private void btnMake_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(cbxMappingTables.Text))
                MessageBox.Show("Please select mapping table.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                MappingTables table = (MappingTables)Enum.Parse(typeof(MappingTables), cbxMappingTables.Text);
                maker = MappingMakerFactory.CreateMaker(table);
                maker.Messaging += new MappingMaker.MessageHandler(maker_Messaging);
                maker.Progressing += new MappingMaker.StatueHandler(InitializeButton);
                maker.StartMake();
            }
        }
        int successed = 0;
        int account = 0;
        void maker_Messaging(bool success, int total, int current, string message)
        {
            account++;
            if (total >= 0)
            {
                lblTotal.Text = string.Format("Total: {0}", total);
            }
            if (success) successed++;
            lblSuccessed.Text = string.Format("Successed: {0}", successed);
            lblFinished.Text = string.Format("Finished: {0}", current);
            lblAccounts.Text = string.Format("Account: {0}", account);
            if (current > 0)
            {
                if (!success)
                lbxResults.Items.Add(message);
            }
        }

        private void InitMappingTables()
        {
            cbxMappingTables.Items.Clear();
            foreach (MappingTables table in Enum.GetValues(typeof(MappingTables)))
            {
                cbxMappingTables.Items.Add(table);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (maker != null) maker.StopMake();
        }

        private void frmMakeMapping_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (maker != null) maker.StopMake();
        }

        private void InitializeButton(bool isProgress,bool success)
        {
            if (isProgress)
            {
                this.btnMake.Enabled = false;
                this.btnStop.Enabled = true;
            }
            else
            {
                this.btnMake.Enabled = true;
                this.btnStop.Enabled = false;
            }

            if (success)
                MessageBox.Show("The Mapping success!");
        }
    }
}
