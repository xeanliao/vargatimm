using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using GPS.Tool.Data;
using GPS.Tool.AreaBusiness;
using GPS.Tool.Import;

namespace GPS.Tool
{
    public delegate void MyDelegate(string myArg);

    public partial class ImportData : Form
    {
        string folderPath;
        string shpExtension = ".shp";
        string dbfExtension = ".dbf";
        Dictionary<FileInfo, FileInfo> fileDictionary ;

        ShpReader shpReader;
        DataTable dt;
        ImportAreaController importController;

        public ImportData()
        {
            InitializeComponent();
            BindClass();

            //CountyAreaRepository countyRep = new CountyAreaRepository();
            //var countyAreaList = countyRep.GetCountyAreas("093");

        }

        private void ImportData_Load(object sender, EventArgs e)
        {
            shpReader = new ShpReader();
        }

        float zoom = 256;

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.TranslateTransform(shpReader.offsetX, shpReader.offsetY);
            for (int i = 0; i < shpReader.points.Count; i++)
            {
                int x = (int)((shpReader.points[i].X - (float)shpReader.xMin) * zoom - 2);
                int y = (int)((shpReader.points[i].Y - (float)shpReader.yMax) * zoom - 2);
                e.Graphics.DrawEllipse(Pens.Black, x, y, 4, 4);
            }
            for (int i = 0; i < shpReader.lines.Count; i++)
            {
                int n = shpReader.lines[i].points.Length;
                PointF[] pt = new PointF[n];
                for (int j = 0; j < n; j++)
                {
                    pt[j].X = (shpReader.lines[i].points[j].X - (float)shpReader.xMin) * zoom;
                    pt[j].Y = (shpReader.lines[i].points[j].Y - (float)shpReader.yMax) * zoom;
                }
                e.Graphics.DrawLines(Pens.Black, pt);
            }
            for (int i = 0; i < shpReader.polygons.Count; i++)
            {
                int n = shpReader.polygons[i].numPoints;
                PointF[] pt = new PointF[n];
                for (int j = 0; j < n; j++)
                {
                    pt[j].X = ((float)shpReader.polygons[i].points[j].X - (float)shpReader.xMin) * zoom;
                    pt[j].Y = ((float)shpReader.polygons[i].points[j].Y - (float)shpReader.yMax) * zoom;
                }
                e.Graphics.DrawPolygon(Pens.Black, pt);
            }
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            zoom = zoom / 2;
            pictureBox1.Invalidate();
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            zoom = zoom * 2;
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            shpReader.x1 = e.X;
            shpReader.y1 = e.Y;
            shpReader.x2 = shpReader.offsetX;
            shpReader.y2 = shpReader.offsetY;
            shpReader.down = true;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (shpReader.down == true)
            {
                shpReader.offsetX = shpReader.x2 - (shpReader.x1 - e.X);
                shpReader.offsetY = shpReader.y2 - (shpReader.y1 - e.Y);
                pictureBox1.Invalidate();
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            shpReader.down = false;
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            shpReader = new ShpReader();
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Shape Files|*.shp";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                shpReader.ShowShape(dialog.FileName);
            }
            dialog.Dispose(); 
            pictureBox1.Invalidate();
        }

        private void btnImportData_Click(object sender, EventArgs e)
        {
            if (this.cbClass.SelectedItem == null)
            {
                MessageBox.Show("Please select one classification!");
                return;
            }
            if (!String.IsNullOrEmpty(this.txtFilePath.Text.Trim()))
            {
                DisabledControls();
                importController = ImportControllerFactory.CreateController(
                    this.cbClass.SelectedItem.ToString());
                if (importController != null)
                {
                    importController.StartMessaging +=
                        new ImportAreaController.StartImportContraller(InsertStart);
                    importController.Messaging +=
                        new ImportAreaController.ImportController(InsertEnd);
                    importController.ShapeShowing +=
                        new ImportAreaController.ShowShapeContraller(ShowShape);
                    importController.StartThread(fileDictionary);
                }
            }
            else
                MessageBox.Show("Please select classification folder");
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            this.trvFile.Nodes.Clear();
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.ShowDialog();
            if (String.IsNullOrEmpty(folder.SelectedPath))
                return;
            this.txtFilePath.Text = folder.SelectedPath;
            folderPath = folder.SelectedPath;
            if (!folderPath.EndsWith("\\"))
                folderPath += "\\";
            fileDictionary = new Dictionary<FileInfo, FileInfo>();

            TreeNode treenode = new TreeNode();
            treenode.Text = "Shape";
            ReadFileToTreeView(folder.SelectedPath, treenode);
            this.trvFile.Nodes.Add(treenode);
            this.trvFile.ExpandAll();

            //foreach (KeyValuePair<FileInfo, FileInfo> entry in fileDictionary)
            //{
            //    FileInfo shpFile = entry.Key;
            //    shpReader = new ShpReader();
            //    shpReader.ShowShape(folderPath + shpFile.Name);

            //    break;
            //}
        }

        private void ReadFileToTreeView(string folderPath, TreeNode node)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
            try
            {
                foreach (DirectoryInfo childDirectoryInfo in directoryInfo.GetDirectories())
                {
                    TreeNode directoryNode = new TreeNode(childDirectoryInfo.Name.ToString());
                    directoryNode.Tag = childDirectoryInfo.Name.ToString();
                    ReadFileToTreeView(
                        folderPath + "\\" + childDirectoryInfo.Name.ToString(),
                        directoryNode);
                    node.Nodes.Add(directoryNode);
                }
                
                foreach (FileInfo fileInfo in directoryInfo.GetFiles("*.shp"))
                {
                    TreeNode leafNode = new TreeNode();
                    leafNode.Text = fileInfo.Name.ToString();
                    leafNode.Name = fileInfo.FullName;
                    leafNode.Tag = fileInfo.Name.ToString();
                    node.Nodes.Add(leafNode);
                    if (File.Exists(folderPath + "\\" + fileInfo.Name.Replace(shpExtension, dbfExtension)))
                    {
                        FileInfo dbfFileInfo = new FileInfo(folderPath + "\\" + 
                            fileInfo.Name.Replace(shpExtension, dbfExtension));
                        fileDictionary.Add(fileInfo, dbfFileInfo);
                    }
                    else
                    {
                        fileDictionary.Add(fileInfo, null);
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        private void ShowMsg(String msg)
        {
            this.lbCurrentFile.Text = msg;
            Application.DoEvents();
        }

        private void ShowSuccessMsg()
        {
            this.cbClass.SelectedIndex = -1;
            this.txtFilePath.Text = "";
            this.lbCurrentFile.Text = "import success!";
            MessageBox.Show("successfully");
            EnabledControls();
        }

        public void InsertEnd(bool success, int total, string message,string fileName)
        {
            if (total == -1)
            {
                MessageBox.Show(message);
                EnabledControls();
            }
            else if (success)
            {
                this.lbCurrentFile.Text = message +
                    "successful, Total : " + total.ToString();
                this.txtProgress.Text += Environment.NewLine + "file name:" + 
                    fileName + " total£º" + total + " successful";
            }
            else
            {
                this.lbCurrentFile.Text = message + "failed";
                this.txtProgress.Text += Environment.NewLine + "file name:" + 
                    fileName + " total£º" + total + " failed";
            }

            if (fileName.Contains(".shp"))
            {
                foreach (TreeNode node in this.trvFile.Nodes)
                {
                    TreeNode treeNode = FindNode(node,
                        fileName);
                    if (treeNode != null)
                    {
                        treeNode.BackColor = Color.Gray;
                        break;
                    }
                }
            }
        }

        public void InsertStart(string message)
        {
            this.lbCurrentFile.Text = message;
        }

        public void DelegateMethod(string myCaption)
        {
            this.lbCurrentFile.Text = myCaption;
        }

        private void trvFile_DoubleClick(object sender, EventArgs e)
        {
            ShowShape(this.trvFile.SelectedNode.Name);
        }

        private void ShowShape(string filePath)
        {
            //shpReader = new ShpReader();
            //shpReader.ShowShape(filePath);

            //foreach (TreeNode node in this.trvFile.Nodes)
            //{
            //    TreeNode treeNode = FindNode(node, 
            //        filePath.Substring(filePath.LastIndexOf("\\") + 1));
            //    if (treeNode != null)
            //    {
            //        treeNode.Collapse();
            //        treeNode.Checked = true;
            //        treeNode.BackColor = Color.Blue;
            //        break;
            //    }
            //}            
            //pictureBox1.Invalidate();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void BindClass()
        {
            this.cbClass.Items.Clear();
            foreach (Classification classification in Enum.GetValues(typeof(Classification)))
            {
                this.cbClass.Items.Add(classification);
            }
        }

        private void tsbMapping_Click(object sender, EventArgs e)
        {
            frmMakeMapping frm = new frmMakeMapping();
            frm.ShowDialog();
        }

        private void DisabledControls()
        {
            this.cbClass.Enabled = false;
            this.btnBrowse.Enabled = false;
            this.btnImportData.Enabled = false;
        }

        private void EnabledControls()
        {
            this.cbClass.Enabled = true;
            this.btnBrowse.Enabled = true;
            this.btnImportData.Enabled = true;
        }

        protected override void OnClosed(EventArgs e)
        {
            if (importController != null && importController.thread != null)
            {
                if (importController.thread.IsAlive)
                {
                    importController.thread.Abort();
                }
            }
            base.OnClosed(e);
        }

        private TreeNode FindNode(TreeNode tnParent, string strValue)
        {
            TreeNode treeNode2 = new TreeNode();
            string venueCode = "";
            if (tnParent == null) return null;
            if (tnParent.Text == strValue) return tnParent;
            tnParent.Expand();
            TreeNode tnRet = null;
            foreach (TreeNode tn in tnParent.Nodes)
            {
                if (venueCode == "")
                {
                    treeNode2 = tn;
                    venueCode = tn.Text;
                }
                else
                {
                    if (venueCode != tn.Text)
                    {
                        treeNode2.Collapse();
                        venueCode = tn.Text;
                        treeNode2 = tn;
                    }
                }

                tnRet = FindNode(tn, strValue);
                if (tnRet != null) break;
            }
            return tnRet;
        }

        private void tsbFix_Click(object sender, EventArgs e)
        {
            frmFix frm = new frmFix();
            frm.ShowDialog();
        }

        private void tsbSelectMapping_Click(object sender, EventArgs e)
        {
            frmSelectMapping frm = new frmSelectMapping();
            frm.ShowDialog();
        }

        private void tsbFixInner_Click(object sender, EventArgs e)
        {
            frmFixInner frm = new frmFixInner();
            frm.ShowDialog();
        }
    }
}