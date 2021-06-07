using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.IO;
using System.Data;
using MySql.Data.MySqlClient;

namespace GPS.Website
{
    public partial class TestA : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void upload_click(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile)
                Response.Write("Yes");
        }

        private void GetTaskById()
        {
            string sQuery = "SELECT * FROM gtuinfo";
            runSQL(sQuery);
        }

        protected void btnRunSQL_Click(object sender, EventArgs e)
        {
            runSQL(txtSQL.Text.Trim());
        }

        private void runSQL(string sQuery)
        {
            string sConn = "Server=localhost; Port=3306; Database=timm201107;User ID=root;Password=harry4h#";
            MySqlConnection conn = new MySqlConnection(sConn);
            conn.Open();

            //string sQuery = "SELECT * FROM gtuinfo";
            MySqlDataAdapter adapter = new MySqlDataAdapter(sQuery, conn);
            DataTable dtTask = new DataTable("task");
            adapter.Fill(dtTask);

            gridView1.DataSource = dtTask;
            gridView1.DataBind();
        }
    
    }
}
