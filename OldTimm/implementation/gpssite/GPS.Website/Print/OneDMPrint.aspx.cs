using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using ExpertPdf.HtmlToPdf;

namespace GPS.Website.Print
{
    public partial class OneDMPrint : System.Web.UI.Page
    {
        private string UrlBase = System.Configuration.ConfigurationManager.AppSettings["UrlBase"];
        protected string TargetingMethod;
        private string[] nds = null;
        protected string DMName = "";
        List<MapPushpin> maddressCOList = new List<MapPushpin>();

        private string mReportUrl = "";
        private string mReportRoot = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            mReportUrl = System.Configuration.ConfigurationManager.AppSettings["reportUrl"];
            mReportRoot = System.Configuration.ConfigurationManager.AppSettings["reportRoot"]; ;

            string currentDM = Request.Form["currentDM"];
            TargetingMethod = Request.Form["targetingmethod1"] + (string.IsNullOrEmpty(Request.Form["targetingmethod2"]) ? "" : "<br />" + Request.Form["targetingmethod2"]);
            PrintPdf(currentDM);

        }
        private void PrintPdf(string currentDM)
        {
            CampaignDM dm = GetCampaignDM(currentDM);
            string DMName = BindCampaignDM(dm);
            string htmlPath = SaveHtml(DMName);
            string pdfPath = ConvertHtmlToPdf(htmlPath, DMName);
            OutputPdf(pdfPath);
        }

        private CampaignDM GetCampaignDM(string currentDM)
        {
            CampaignDM dm = new CampaignDM();
            string[] item = currentDM.Split('~');
            string[] items = item[0].Split('*');
            dm.Id = items[0];
            dm.Name = items[1];
            dm.Total = int.Parse(items[2]).ToString("0,00");
            dm.Count = int.Parse(items[3]).ToString("0,00");
            dm.Pen = items[4];
            dm.MapImgUrl = CreateMapImage(items[5], items[6], items[7], items[8], items[9], items[14], items[15]);
            dm.FiveZips = GetSubItems(items[10]);
            dm.CRoutes = GetSubItems(items[11]);
            dm.Tracts = GetSubItems(items[12]);
            dm.BlockGroups = GetSubItems(items[13]);
            dm.Nd = "";

            
            
            string addresslist = item[2];
            string strTem = "";
            if (addresslist.Length>0)
            {
                addresslist = addresslist.Substring(0, addresslist.Length-1);
                dm.Nd = "<div align='center'>ADDRESSES LIST"
                     + "</div><div class='spaceline'></div><table style='width:100%;border-top:3px solid black;border-bottom:3px solid black;border-left:3px solid black;border-right:3px solid black;height:216px;'> ";
                string[] address_dm = addresslist.Split('&');
                int len_a = address_dm.Length - 1;
                if (address_dm.Length - 1>=0)
                {
                    for (int y = 0; y < address_dm.Length;y++ )
                    {
                        if ((y == 0) || (y % 5 == 0))
                        {
                            strTem = strTem + "<tr>";
                        }
                        if(address_dm[y]!=""){
                            string[] details = address_dm[y].Split('!');
                            if (details[1]!="")
                            {
                                strTem = strTem + "<td><table style='width:200px;'><tr><td align='center' style='text-font:bold;'>" + details[0] + "<br/><img src='" + this.ResolveUrl("~/Files/Images/Address/") + details[1] + "' width='150px;' height='150px;'><br/>#" + details[5] +"<br/>" +details[2] + "</td></tr></table><br/></td>";
                            }else{
                                strTem = strTem + "<td><table style='width:200px;'><tr><td align='center' style='text-font:bold;'>" + details[0] + "<br/><img src='" + this.ResolveUrl("~/Files/Images/Address/noview.JPG") + "' width='150px;' height='150px;'><br/>" + details[5] + "<br/>" + details[2] + "</td></tr></table><br/></td>";
                            }
                            MapPushpin mp = new MapPushpin();
                            mp.text = "";
                            mp.x = float.Parse(details[3]);
                            mp.y = float.Parse(details[4]);
                            maddressCOList.Add(mp);
                        }
                        
                        if ((y % 5 == 4) || (y == (address_dm.Length - 1)))
                        {
                            strTem = strTem + "</tr>";
                        }
                       
                    }
                }
                dm.Nd = dm.Nd + strTem + "</table><div class='spaceline'></div><div class='spaceline'></div>";
            }
            
            
            string ndlist = item[1];
            if (ndlist.Length > 0)
            {
                ndlist = ndlist.Substring(0, ndlist.Length - 1);
            }

            string allNDS_DM = "";
            string[] perNds = ndlist.Split('$');
            if (perNds.Length > 1)
            {
                if (perNds[1] == dm.Id)
                {
                    allNDS_DM = perNds[0];
                }
            }
            
            //if (allNDS_DM != "")
            //{
            //    dm.Nd = dm.Nd + "<div align='center'>DO NOT DISTRIBUTE LIST"
            //        //+ dm.Name + "(MC#:" + campaignName + ")" + 
            //         + "</div><div class='spaceline'></div><table style='width:100%;border-top:3px solid black;border-bottom:3px solid black;border-left:3px solid black;border-right:3px solid black;height:216px;'> ";
            //    string[] nd_dm = allNDS_DM.Split(';');
            //    int len = nd_dm.Length - 1;
            //    int rows = 7;
            //    int columns = 4;
            //    int x = 0;

            //    while ((x < (len - 1)) && (x < (rows * columns)))
            //    {
            //        if (x % columns == 0)
            //        {
            //            dm.Nd = dm.Nd + "<tr>";
            //        }
            //        if (((x == (rows * columns - 1)) && (x < (len - 2))))
            //        {
            //            dm.Nd = dm.Nd + "<td class='rowlabel'>" + nd_dm[x] + "...</td>";
            //        }
            //        else
            //        {
            //            dm.Nd = dm.Nd + "<td class='rowlabel'>" + nd_dm[x] + "</td>";
            //        }
            //        if ((x % columns == (columns - 1)) || (x == (len - 2)))
            //        {
            //            dm.Nd = dm.Nd + "</tr>";
            //        }
            //        x++;
            //    }

            //    dm.Nd = dm.Nd + "</table>";
            //}
            
            

            return dm;
        }

        private string BindCampaignDM(CampaignDM dm)
        {
            IList<CampaignDM> dmlist = new List<CampaignDM>();
            dmlist.Add(dm);
            rptDM.DataSource = dmlist;
            rptDM.DataBind();
            return dm.Name;
        }

        #region GetSubItems
        private List<SubItem> GetSubItems(string subitemsStr)
        {
            List<SubItem> subitems = new List<SubItem>();
            if (!string.IsNullOrEmpty(subitemsStr))
            {
                string[] items = subitemsStr.Split(';');
                foreach (string item in items)
                {
                    subitems.Add(GetSubItem(item));
                }
            }
            return subitems;
        }

        private SubItem GetSubItem(string subitemStr)
        {
            SubItem subItem = new SubItem();
            string[] items = subitemStr.Split(',');
            subItem.OrderId = items[0];
            subItem.Code = items[1];
            subItem.Total = int.Parse(items[2]).ToString("0,00");
            subItem.Count = int.Parse(items[3]).ToString("0,00");
            subItem.Pen = items[4];
            return subItem;
        }
        #endregion

        #region GetMapImages

        private List<MapImage> GetMapImages(string mapString)
        {
            string path = mReportRoot + "Print\\images\\cache";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            List<MapImage> datas = new List<MapImage>();
            string[] imgs = mapString.Split(';');
            int i = 0;
            foreach (string img in imgs)
            {
                string[] items = img.Split(',');
                Uri uri = new Uri(items[0]);
                MapImage data = new MapImage();
                data.url = items[0];
                data.x = float.Parse(items[1]);
                data.y = float.Parse(items[2]);
                data.path = string.Format("{0}\\{1}{2}", path, Server.UrlEncode(data.url), ".png");
                datas.Add(data);
                i++;
            }

            return datas;
        }

        #endregion

        #region GetMapShapes

        public List<MapShape> GetMapShapes(string shapeString)
        {
            List<MapShape> datas = new List<MapShape>();
            if (!string.IsNullOrEmpty(shapeString))
            {
                string[] shapes = shapeString.Split('$');
                foreach (string shape in shapes)
                {
                    MapShape data = new MapShape();
                    string[] items = shape.Split('|');
                    data.points = GetPoints(items[0]);
                    string[] arrL = items[1].Split(';');
                    data.lineColor = Color.FromArgb(Convert.ToInt32(float.Parse(arrL[3]) * 255), int.Parse(arrL[0]), int.Parse(arrL[1]), int.Parse(arrL[2]));
                    string[] arrF = items[2].Split(';');
                    data.fillColor = Color.FromArgb(Convert.ToInt32(float.Parse(arrF[3]) * 255), int.Parse(arrF[0]), int.Parse(arrF[1]), int.Parse(arrF[2]));
                    data.lineWidth = int.Parse(items[3]);
                    datas.Add(data);
                }
            }
            return datas;
        }

        private Point[] GetPoints(string pointsString)
        {

            string[] items = pointsString.Split(';');
            int count = items.Count();
            Point[] points = new Point[count];
            for (int i = 0; i < count; i++)
            {
                string[] ps = items[i].Split(',');
                points[i].X = Convert.ToInt32(Math.Round((float.Parse(ps[0]))));
                points[i].Y = Convert.ToInt32(Math.Round((float.Parse(ps[1]))));
            }

            return points;
        }

        #endregion

        #region GetMapPushpins

        private List<MapPushpin> GetMapPushpins(string pushpinsStr)
        {
            List<MapPushpin> pushpins = new List<MapPushpin>();
            if (!string.IsNullOrEmpty(pushpinsStr))
            {
                string[] items = pushpinsStr.Split(';');
                foreach (string item in items)
                {
                    pushpins.Add(GetMapPushpin(item));
                }
            }
            return pushpins;
        }

        private MapPushpin GetMapPushpin(string pushpinStr)
        {
            MapPushpin pushpin = new MapPushpin();
            string[] items = pushpinStr.Split(',');
            pushpin.text = items[0];
            pushpin.x = float.Parse(items[1]);
            pushpin.y = float.Parse(items[2]);
            pushpin.type = items[3];
            return pushpin;
        }

        #endregion

        #region GetGtuDots

        private List<MapPushpin> GetGtuDots(string gtuStrs)
        {
            List<MapPushpin> pushpins = new List<MapPushpin>();
            if (!string.IsNullOrEmpty(gtuStrs))
            {
                string[] items = gtuStrs.Split(';');
                foreach (string item in items)
                {
                    pushpins.Add(GetGtuDot(item));
                }
            }
            return pushpins;
        }

        private MapPushpin GetGtuDot(string gtuStr)
        {
            MapPushpin gtuDot = new MapPushpin();
            string[] items = gtuStr.Split(',');
            gtuDot.text = items[2];
            gtuDot.x = float.Parse(items[0]);
            gtuDot.y = float.Parse(items[1]);
            return gtuDot;
        }

        #endregion

        #region MapLocations

        private List<MapLocation> GetMapLocations(string pushpinsStr)
        {
            List<MapLocation> pushpins = new List<MapLocation>();
            if (!string.IsNullOrEmpty(pushpinsStr))
            {
                string[] items = pushpinsStr.Split(';');
                foreach (string item in items)
                {
                    pushpins.Add(GetMapLocation(item));
                }
            }
            return pushpins;
        }

        private MapLocation GetMapLocation(string pushpinStr)
        {
            MapLocation pushpin = new MapLocation();
            string[] items = pushpinStr.Split(',');
            pushpin.text = items[2];
            pushpin.x = float.Parse(items[0]);
            pushpin.y = float.Parse(items[1]);
            return pushpin;
        }

        #endregion

        #region GetScaleItem

        private ScaleItem GetScaleItem(string scaleStr)
        {
            ScaleItem item = new ScaleItem();
            if (!string.IsNullOrEmpty(scaleStr))
            {
                string[] array = scaleStr.Split(',');
                item.Text = array[0];
                item.Width = int.Parse(array[1]);
            }
            else
            {
                item.Width = 0;
            }
            return item;

        }

        #endregion

        private string CreateMapImage(string imagesStr, string shapesStr, string pushpinsStr, string locationsStr, string scaleStr, string ndStr,string gtuStr)
        {
            List<MapImage> images = GetMapImages(imagesStr);
            List<MapShape> shapes = GetMapShapes(shapesStr);
            List<MapPushpin> pushpins = new List<MapPushpin>();
            List<MapLocation> locations = new List<MapLocation>();
            List<MapPushpin> gtuDots = new List<MapPushpin>();
            ScaleItem scaleItem = new ScaleItem();
            if (pushpinsStr != "")
            {
                pushpins = GetMapPushpins(pushpinsStr);
            }
            if (gtuStr != "")
            {
                gtuDots = GetGtuDots(gtuStr);
            }
            if (locationsStr != "")
            {
               locations = GetMapLocations(locationsStr);
            }
            if (scaleStr!="")
            {
                scaleItem = GetScaleItem(scaleStr);
            }
            List<MapLocation> ndLocations = GetMapLocations(ndStr);

            WebClient mywebclient = new WebClient();
            foreach (MapImage image in images)
            {
                if (!File.Exists(image.path))
                {
                    mywebclient.DownloadFile(image.url, image.path);
                }
            }

            return CreateMapImage(images, shapes, pushpins, locations, scaleItem, ndLocations, gtuDots);
        }


        private string CreateMapImage(List<MapImage> images, List<MapShape> shapes, List<MapPushpin> pushpins, List<MapLocation> locations, ScaleItem scaleItem, List<MapLocation> ndlocations,List<MapPushpin> gtuDots)
        {
            int width = 980;
            int height = 980;
            string imageName = "merge" + DateTime.Now.Ticks.ToString() + ".png";
            string imagePath = mReportRoot + "Print\\images\\" + imageName;
            Bitmap bmp = new Bitmap(width, height);
            System.Drawing.Graphics g = Graphics.FromImage(bmp);

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            foreach (MapImage imageData in images)
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(imageData.path);
                g.DrawImage(image, imageData.x, imageData.y);
                image.Dispose();
            }

            foreach (MapShape shapeData in shapes)
            {
                Pen pen = new Pen(shapeData.lineColor, shapeData.lineWidth);
                Brush brush = new SolidBrush(shapeData.fillColor);
                g.DrawPolygon(pen, shapeData.points);
                g.FillPolygon(brush, shapeData.points);
                pen.Dispose();
                brush.Dispose();
            }

            Brush pushpinBrush = new SolidBrush(Color.Black);
            //Brush radiiMarkBrush = new SolidBrush(Color.FromArgb(249, 5, 229));
            Brush radiiMarkBrush = new SolidBrush(Color.FromArgb(6, 9, 132));
            Font pushpinFont = new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel);
            System.Drawing.Image locationRedImage = System.Drawing.Image.FromFile(Server.MapPath("~/Images/pushpins/red-star.png"));
            System.Drawing.Image locationGreenImage = System.Drawing.Image.FromFile(Server.MapPath("~/Images/pushpins/green-star.png"));
            System.Drawing.Image locationMAddressImage = System.Drawing.Image.FromFile(Server.MapPath("~/Files/GtuDots/maddress.PNG"));
            for (int i = 0; i < locations.Count; i++)
            {
                MapLocation location = locations[i];
                System.Drawing.Image image = null;
                if (location.text == "red")
                {
                    image = locationRedImage;
                    g.DrawImage(image, location.x - 15, location.y - 15, 30, 30);
                }
                else if (location.text == "green")
                {
                    image = locationGreenImage;
                    g.DrawImage(image, location.x - 15, location.y - 15, 30, 30);
                }
                else {
                    image = locationMAddressImage;
                    g.DrawImage(image, location.x - 15, location.y - 15, 30, 30);
                }

                Font f = new Font(new FontFamily("Arial"), 20, FontStyle.Bold);
                Brush b = new SolidBrush(Color.Black);
                g.DrawString((i + 1).ToString(), f, b, location.x - 18, location.y - 20);
            }

            System.Drawing.Image locationFlagImage = System.Drawing.Image.FromFile(Server.MapPath("~/Images/pushpins/flag.png"));
            for (int i = 0; i < ndlocations.Count; i++)
            {
                MapLocation location = ndlocations[i];
                System.Drawing.Image image = locationFlagImage;
                //g.DrawImage(image, location.x - 13, location.y - 25, 40, 40);
                Font f = new Font(new FontFamily("Arial"), 20, FontStyle.Bold);
                Brush b = new SolidBrush(Color.Black);
                g.DrawString((i + 1).ToString(), f, b, location.x - 18, location.y - 20);
            }

            foreach (MapPushpin gtuDot in gtuDots)
            {
                string dotNm = gtuDot.text.TrimStart('#');
                System.Drawing.Image dotImage = System.Drawing.Image.FromFile(Server.MapPath("~/Files/GtuDots/" + dotNm + ".png"));                
                g.DrawImage(dotImage, gtuDot.x - 8 , gtuDot.y - 8 , 14, 14);                
            }
            //string dotNm = gtuDots[6].text.TrimStart('#');
            //System.Drawing.Image dotImage = System.Drawing.Image.FromFile(Server.MapPath("~/Files/Images/gtus/" + dotNm + ".png"));
            //g.DrawImage(dotImage, gtuDots[6].x - 15, gtuDots[6].y - 15, 30, 30);
            //g.DrawImage(dotImage, gtuDots[7].x - 15, gtuDots[7].y - 15, 30, 30);
            //g.DrawImage(dotImage, gtuDots[8].x - 15, gtuDots[8].y - 15, 30, 30);
            //foreach (MapPushpin ma in maddressCOList)
            //{
            //    System.Drawing.Image dotImage = System.Drawing.Image.FromFile(Server.MapPath("~/Files/GtuDots/maddress.PNG"));
            //    g.DrawImage(dotImage, ma.x - 8, ma.y - 8, 14, 14);
            //}

            foreach (MapPushpin pushpin in pushpins)
            {
                if (pushpin.type == "radiimark")
                {
                    g.DrawString(pushpin.text, pushpinFont, radiiMarkBrush, pushpin.x, pushpin.y);
                }
                else
                {
                    g.DrawString(pushpin.text, pushpinFont, pushpinBrush, pushpin.x, pushpin.y);
                }
            }

            if (scaleItem.Width > 4)
            {
                Pen sp1 = new Pen(Color.White);
                g.DrawRectangle(sp1, width - scaleItem.Width - 10, width - 20, scaleItem.Width, 7);
                Pen sp2 = new Pen(Color.Black);
                g.DrawRectangle(sp2, width - scaleItem.Width - 9, width - 19, scaleItem.Width - 2, 5);
                Brush sb = new SolidBrush(Color.FromArgb(255, 170, 203, 238));
                g.FillRectangle(sb, width - scaleItem.Width - 8, width - 18, scaleItem.Width - 4, 3);
                Font sf = new Font(new FontFamily("Arial"), 11);
                float tw = g.MeasureString(scaleItem.Text, sf).Width;
                g.DrawString(scaleItem.Text, sf, pushpinBrush, width - tw - 10, height - 40);
            }

            g.Save();
            bmp.Save(imagePath);
            g.Dispose();
            bmp.Dispose();
            locationRedImage.Dispose();
            locationGreenImage.Dispose();
            return mReportUrl + "Print/images/" + imageName;
        }

        private string SaveHtml(string DMName)
        {
            string name = DMName + ".html";
            name = Server.UrlEncode(name).Replace('+', '_').Replace('%', '_');
            string path = mReportRoot + "Print\\" + name;
            StreamWriter sw = File.CreateText(path);
            HtmlTextWriter hwriter = new HtmlTextWriter(sw);
            base.Render(hwriter);
            hwriter.Close();
            sw.Close();
            return mReportUrl + "Print/" + name;
        }

        private string ConvertHtmlToPdf(string htmlPath, string campaignName)
        {
            string name = campaignName + ".pdf";
            name = Server.UrlEncode(name).Replace('+', '_').Replace('%', '_');

            string pdfPath = mReportRoot + "Print\\" + name;
            try
            {
                //PdfConverter pdfConverter = new PdfConverter();
                //pdfConverter.AvoidImageBreak = true;
                //pdfConverter.LicenseKey = System.Configuration.ConfigurationManager.AppSettings["PdfConverterKey"];
                //pdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;
                //pdfConverter.PdfDocumentInfo.AuthorName = "Harry Duan";
                //pdfConverter.PdfDocumentInfo.CreatedDate = DateTime.Now;
                //pdfConverter.PdfDocumentInfo.Subject = "Sample PDF created by ExpertPDF"; ;
                //pdfConverter.PdfDocumentOptions.PdfCompressionLevel = PdfCompressionLevel.Best;
                //pdfConverter.PdfDocumentOptions.EmbedFonts = false;
                //pdfConverter.PdfDocumentOptions.ShowFooter = false;
                //pdfConverter.PdfDocumentOptions.ShowHeader = false;
                //pdfConverter.PdfDocumentOptions.GenerateSelectablePdf = true;
                //pdfConverter.PdfDocumentOptions.LeftMargin = (int)(0.5 * 72);
                //pdfConverter.PdfDocumentOptions.RightMargin = (int)(0.5 * 72);
                //pdfConverter.PdfDocumentOptions.TopMargin = (int)(0.5 * 72);
                //pdfConverter.PdfDocumentOptions.BottomMargin = (int)(0.5 * 72);
                ////pdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.Custom;
                ////pdfConverter.PdfDocumentOptions.CustomPdfPageSize = new SizeF((int)(24 * 72), (int)(27.5 * 72));
                //pdfConverter.PageWidth = (int)(35 * 72);
                //pdfConverter.PdfDocumentOptions.AutoSizePdfPage = true;
                //pdfConverter.SavePdfFromUrlToFile(htmlPath, pdfPath);


                //if (string.IsNullOrEmpty(htmlPath) || string.IsNullOrEmpty(pdfPath))
                //    return string.Empty;
                //Process p = new Process();
                //string str = System.Web.HttpContext.Current.Server.MapPath("~/Print/wkhtmltopdf-0.8.3.exe");
                //if (!System.IO.File.Exists(str))
                //    return string.Empty;
                //p.StartInfo.FileName = str;
                //p.StartInfo.Arguments = "--margin-left 5 --margin-right 5 --margin-top 15 \"" + htmlPath + "\" " + pdfPath;
                //p.StartInfo.UseShellExecute = false;
                //p.StartInfo.RedirectStandardInput = true;
                //p.StartInfo.RedirectStandardOutput = true;
                //p.StartInfo.RedirectStandardError = true;
                //p.StartInfo.CreateNoWindow = true;
                //p.Start();
                //p.WaitForExit(30000);


                PdfConverter pdfConverter = new PdfConverter();
                pdfConverter.AvoidImageBreak = true;
                pdfConverter.LicenseKey = System.Configuration.ConfigurationManager.AppSettings["PdfConverterKey"];

                //pdfConverter.PageWidth = 982;
                //pdfConverter.
                pdfConverter.PdfDocumentInfo.AuthorName = "Harry Duan";
                pdfConverter.PdfDocumentInfo.CreatedDate = DateTime.Now;
                pdfConverter.PdfDocumentInfo.Subject = "Sample PDF created by ExpertPDF";

                //pdfConverter.PdfSecurityOptions.KeySize = EncryptionKeySize.EncryptKey128Bit;
                //pdfConverter.PdfSecurityOptions.UserPassword = "Casey";
                //pdfConverter.PdfDocumentOptions.FitHeight = false;
                //pdfConverter.PdfDocumentOptions.FitWidth = false;
                //pdfConverter.PdfDocumentOptions.AutoSizePdfPage = true;
                pdfConverter.PdfDocumentOptions.PdfCompressionLevel = PdfCompressionLevel.Best;
                pdfConverter.PdfDocumentOptions.EmbedFonts = false;
                pdfConverter.PdfDocumentOptions.ShowFooter = false;
                pdfConverter.PdfDocumentOptions.ShowHeader = false;
                pdfConverter.PdfDocumentOptions.GenerateSelectablePdf = true;
                pdfConverter.PdfDocumentOptions.LeftMargin = 10;
                pdfConverter.PdfDocumentOptions.RightMargin = 10;
                pdfConverter.PdfDocumentOptions.TopMargin = 20;

                pdfConverter.PdfFooterOptions.DrawFooterLine = false;

                //pdfConverter.PdfFooterOptions.AddHtmlToPdfArea(new HtmlToPdfArea(FooterUrl));
                //pdfConverter.PdfFooterOptions.ShowPageNumber = false;
               // pdfConverter.PdfFooterOptions.AddTextArea(new TextArea(265, 3,  "Page &p; of &P;",   new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, 6)));

                pdfConverter.SavePdfFromUrlToFile(htmlPath, pdfPath);
            }
            catch (Exception ex)
            {
                GPS.Utilities.LogUtils.Error("Web application Unhandle error", ex);
                HttpContext.Current.Response.Write(ex);
            }
            return name;
        }

        private void OutputPdf(string pdfPath)
        {
            // Response.Redirect(pdfPath);
            Response.Redirect(mReportUrl + "Print/" + pdfPath);
        }

        protected override void Render(HtmlTextWriter writer)
        {
        }

        protected void rptDM_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.DataItem != null)
            {
                if (e.Item.FindControl("imgDirectionLegend") != null)
                {
                    System.Web.UI.WebControls.Image img = (System.Web.UI.WebControls.Image)e.Item.FindControl("imgDirectionLegend");
                    img.ImageUrl = WebUtil.GetAbsoluteUrl(this.Request, "Images/direction-legend.png");
                }
            }
        }

    }
}
