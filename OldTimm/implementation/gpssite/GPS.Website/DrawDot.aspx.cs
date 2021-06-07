using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace GPS.Website
{
    public partial class DrawDot : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // by default display blue dot
            Color dotColor = Color.FromArgb(0, 0, 255);
            try
            {
                string sHexColor = Request["hexcolor"];
                if (sHexColor.StartsWith("#"))
                    sHexColor = sHexColor.Substring(1);

                int iRed = Convert.ToInt32(sHexColor.Substring(0, 2), 16);
                int iGreen = Convert.ToInt32(sHexColor.Substring(2, 2), 16);
                int iBlue = Convert.ToInt32(sHexColor.Substring(4, 2), 16);
                dotColor = Color.FromArgb(iRed, iGreen, iBlue);
            }
            catch (Exception) { }

            drawColorDot(dotColor);
        }

        private void drawColorDot(Color dotColor)
        {
            Response.Clear();
            Response.ContentType = "image/png";

            int width = 16;
            int height = width;
            Bitmap b = new Bitmap(width, height);

            Graphics g = Graphics.FromImage(b);
            g.Clear(Color.Gray);
            g.FillEllipse(new SolidBrush(dotColor), 0, 0, 15, 15);
            g.Flush();
            g.Dispose();

            // Make Transparent
            b.MakeTransparent(Color.Gray);

            // Write PNG to Memory Stream then write to OutputStream
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            b.Save(ms, ImageFormat.Png);
            ms.WriteTo(Response.OutputStream);

            Response.Flush();
            Response.End();
        }

        private void drawDotWithBorder()
        {
            Response.Clear();
            Response.ContentType = "image/png";

            int width = 11;
            int height = width;

            // Create a new 32-bit bitmap image
            Bitmap b = new Bitmap(width, height);

            // Create Grahpics object for drawing
            Graphics g = Graphics.FromImage(b);

            // Fill the image with a color to be made Transparent after drawing is finished.
            g.Clear(Color.Gray);

            //'' Get rectangle where the Circle will be drawn
            Rectangle rect = new Rectangle(0, 0, width - 1, height - 1);

            //'' Draw Circle Border
            Pen bPen = Pens.Black;
            g.DrawPie(bPen, rect, 0, 365);

            //'' Fill in Circle
            SolidBrush cbrush = new SolidBrush(Color.Red);
            g.FillPie(cbrush, rect, 0, 365);

            //'' Clean up
            g.Flush();
            g.Dispose();

            //'' Make Transparent
            b.MakeTransparent(Color.Gray);

            // Write PNG to Memory Stream then write to OutputStream
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            b.Save(ms, ImageFormat.Png);
            ms.WriteTo(Response.OutputStream);

            Response.Flush();
            Response.End();
        }

    }
}