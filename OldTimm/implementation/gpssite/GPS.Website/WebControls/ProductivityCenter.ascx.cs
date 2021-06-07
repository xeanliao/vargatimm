using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GPS.Website.WebControls
{
    public partial class ProductivityCenter : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.FileUpload1.HasFile == false)
                {
                    lblMessage.Text = "<script>alert('Please choose a file first!')</script>";
                    return;
                }

                // make sure the name of the file should follow a rule, the first 6 digits should be a gtu id
                int iTaskId = Convert.ToInt32(txtTaskID.Text);
                TaskServices.TaskReaderService taskReader = new GPS.Website.TaskServices.TaskReaderService();
                TransferObjects.ToTask oTask = taskReader.GetTask(iTaskId);

                SortedList<string, int> relatedGtu = new SortedList<string, int>();
                int iTaskGtuInfoMappingId = 0;
                foreach (TransferObjects.ToTaskgtuinfomapping mapping in oTask.Taskgtuinfomappings)
                {
                    string sUniqueID = mapping.GTU.UniqueID;
                    string right6 = sUniqueID.Substring(sUniqueID.Length - 6);
                    relatedGtu.Add(right6, mapping.Id);
                }

                System.IO.StreamReader reader = new System.IO.StreamReader(this.FileUpload1.FileContent);
                string sContent = reader.ReadToEnd();

                // Call Webservice
                TrackServices.GtuWriterService gtuWriter = new GPS.Website.TrackServices.GtuWriterService();
                int iAdded = gtuWriter.ImportGtuInfo(relatedGtu, sContent);

                string sUser = GPS.DomainLayer.Security.LoginMember.CurrentMember.UserName;
                string sLog = string.Format("user id:{0}, TaskID:{1}, TaskGtuInfoId:{2}, Records added:{3}", sUser, iTaskId, iTaskGtuInfoMappingId, iAdded);
                sLog += "\r\n";
                sLog += sContent;
                GPS.Utilities.DBLog.LogInfo(sLog);
            }
            catch (Exception ex)
            {
                GPS.Utilities.DBLog.LogError(ex.ToString());
            }
        }

    }
}