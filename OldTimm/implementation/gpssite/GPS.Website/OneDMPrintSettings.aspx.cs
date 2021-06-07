using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GPS.Web;
using System.Drawing;
using System.Drawing.Imaging;
using GPS.DataLayer.RepositoryImplementations;
using GPS.DomainLayer.Entities;

namespace GPS.Website
{
    public partial class OneDMPrintSettings : System.Web.UI.Page // SecurityPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                IList<string> colStrLst = new List<string>();
                //IList<Address> addressLst = new List<Address>();
                int id = int.Parse(Request.QueryString["id"]);
                TaskRepository taskrepository = new TaskRepository();
                colStrLst = taskrepository.GetUserColorByTaskId(id);
                //addressLst = taskrepository.GetAddressByTaskId(id);
                //if (addressLst.Count>0)
                //{
                //    foreach(Address a in addressLst){
                //        createAddress(a);
                //    }
                //}
                if (colStrLst.Count > 0)
                    foreach (string colStr in colStrLst)
                    {
                        createGtuDotBmps(toColor(colStr));
                    
                    }
            }
            catch(Exception ex)
            {
                GPS.Utilities.LogUtils.Error("Web application Unhandle error", ex);
            }
        }

        //public void createAddress(Address a)
        //{
        //    string starPath = "";
        //    if (a.Color == "red")
        //    {
        //        starPath = "images/pushpins/red-star.png";
        //    }else{
        //        starPath = "images/pushpins/green-star.png";
        //    }

            
        //}


        public void createGtuDotBmps(Color color)
        {
            string colorName = ColorTranslator.ToHtml(color).TrimStart('#');
            string filePath = HttpContext.Current.Server.MapPath("~/Files/GtuDots") + "\\" + colorName + ".png";
            //if (!System.IO.File.Exists(filePath))
            //{
                Bitmap bmp = new Bitmap(15, 15);
                Graphics g = Graphics.FromImage(bmp);

                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                Brush brush = new SolidBrush(color);
                
                g.FillEllipse(brush, new Rectangle(1, 1, 13, 13));
                g.DrawEllipse(new Pen(Color.Black, 0.3f), new Rectangle(1, 1, 13, 13));

                bmp.Save(filePath, ImageFormat.Png);

                brush.Dispose();
                g.Dispose();
                bmp.Dispose();
            //}
        }

        public Color toColor(string value)
        {
            value = value.Replace("#", "");
            Int32 v = Int32.Parse(value, System.Globalization.NumberStyles.HexNumber);
            byte r = Convert.ToByte((v >> 16) & 255);
            byte g = Convert.ToByte((v >> 8) & 255);
            byte b = Convert.ToByte((v >> 0) & 255);
            return Color.FromArgb(255, r, g, b);
        }
    }
}
