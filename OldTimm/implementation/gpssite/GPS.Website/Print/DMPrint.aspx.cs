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
using System.Configuration;

namespace GPS.Website.Print
{
    public partial class DMPrint : System.Web.UI.Page
    {
        private string UrlBase = System.Configuration.ConfigurationManager.AppSettings["UrlBase"];
        private string FooterUrl;
        protected string TargetingMethod;
        private List<ColorItem> Colors;
        private string[] nds = null;
        protected string campaignName = "";

        private string mReportUrl = "";
        private string mReportRoot = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            mReportUrl = System.Configuration.ConfigurationManager.AppSettings["reportUrl"];
            mReportRoot = System.Configuration.ConfigurationManager.AppSettings["reportRoot"]; ;

            string campaign = Request.Form["campaign"];
            TargetingMethod = Request.Form["targetingmethod1"] + (string.IsNullOrEmpty(Request.Form["targetingmethod2"]) ? "" : "<br />" + Request.Form["targetingmethod2"]);
            //string showCoverPage = Request.Form["showcoverpage"];
            //if (showCoverPage.ToLower() != "true")
            //{
            //    plCoverPage.Visible = false;
            //}
            PrintPdf(campaign);
        }

        private void PrintPdf(string campaign)
        {
            string campaignName = BindCampaign(campaign);
            string htmlPath = SaveHtml(campaignName);
            string pdfPath = ConvertHtmlToPdf(htmlPath, campaignName);
            OutputPdf(pdfPath);
        }

        #region BindCampaign

        private string BindCampaign(string campaign)
        {
            string[] items = campaign.Split('~');
            campaignName = items[0];
            string contactName = items[1];
            string clientName = items[2];
            string userFullName = items[3];
            string total = items[4];
            string count = items[5];
            string pen = items[6];
            string date = items[7];
            //string image = items[8];
            //string shape = items[9];
            //string pushpin = items[10];
            //string location = items[11];
            //string scale = items[12];
            string submap = items[8];
            string color = items[9];
            string logo = items[10];
            string ndlist = items[11];
            if (ndlist.Length > 0)
            {
                ndlist = ndlist.Substring(0, ndlist.Length - 1);
            }
            nds = ndlist.Split('@');

            //FooterUrl = UrlBase + string.Format("/Print/Footer.aspx?campaign={0}&on={1}&for={2}&by={3}", campaignName, date, contactName, userFullName);
            //Colors = GetColorItems(color);
            //Response.Write(Colors.Count);
            //if (Colors.Count > 0)
            //{
            //    rptColors.DataSource = Colors;
            //    rptColors.DataBind();
            //}
            //if (!string.IsNullOrEmpty(logo))
            //{
            //    imgLogo.ImageUrl = Server.MapPath("~/Files/Images/" + logo);
            //}
            //else
            //{
            //    imgLogo.Visible = false;
            //}

            //ltlClient.Text = clientName;
            //ltlCreatedFor.Text = contactName;
            //ltlCreatedOn.Text = date;
            //ltlMasterCampaign.Text = campaignName;
            //ltlCreatedBy.Text = userFullName;


            //
            //ltlCampaign.Text = campaignName;
            //ltlClientName.Text = clientName;
            //ltlContactName.Text = contactName;
            //ltlTargetingMethod.Text = TargetingMethod;
            //ltlTotal.Text = int.Parse(total).ToString("0,00");
            //ltlCount.Text = int.Parse(count).ToString("0,00"); ;
            //ltlPen.Text = pen;
            //imgCampaignMap.ImageUrl = CreateMapImage(image, shape, pushpin, location, scale);
            //List<CampaignSubMap> submaps = GetCampaignSubMaps(submap);
            List<CampaignDM> submaps = GetCampaignDMS(submap);
            //BindCampaignDMSSummary(submaps);
            BindCampaignDMS(submaps);
            return items[0];
        }


        private CampaignDM GetCampaignDM(string submapStr, int i)
        {
            CampaignDM dm = new CampaignDM();
            string[] items = submapStr.Split('*');
            dm.Id = items[0];
            dm.Name = items[1];
            dm.Total = int.Parse(items[2]).ToString("0,00");
            dm.Count = int.Parse(items[3]).ToString("0,00");
            dm.Pen = items[4];
            dm.MapImgUrl = CreateMapImage(items[5], items[6], items[7], items[8], items[9], items[14]);
            dm.FiveZips = GetSubItems(items[10]);
            dm.CRoutes = GetSubItems(items[11]);
            dm.Tracts = GetSubItems(items[12]);
            dm.BlockGroups = GetSubItems(items[13]);
                      

            string allNDS_DM = "";
            foreach (string perNd in nds)
            {
                string[] perNds = perNd.Split('$');
                if (perNds.Length > 1)
                {
                    if (perNds[1] == dm.Id)
                    {
                        allNDS_DM = perNds[0];
                        break;
                    }
                }
            }

            if (allNDS_DM != "")
            {
                dm.Nd = "<div align='center'>DO NOT DISTRIBUTE LIST"
                    //+ dm.Name + "(MC#:" + campaignName + ")" + 
                     + "</div><div class='spaceline'></div><table style='width:100%;border-top:3px solid black;border-bottom:3px solid black;border-left:3px solid black;border-right:3px solid black;height:216px;'> ";
                string[] nd_dm = allNDS_DM.Split(';');
                int len = nd_dm.Length;
                int rows = 7;
                int columns = 4;
                int x = 0;

                while ((x < (len - 1)) && (x < (rows * columns)))
                {
                    if (x % columns == 0)
                    {
                        dm.Nd = dm.Nd + "<tr>";
                    }
                    if (((x == (rows * columns - 1)) && (x < (len - 2))))
                    {
                        dm.Nd = dm.Nd + "<td class='rowlabel'>" + nd_dm[x] + "...</td>";
                    }
                    else
                    {
                        dm.Nd = dm.Nd + "<td class='rowlabel'>" + nd_dm[x] + "</td>";
                    }
                    if ((x % columns == (columns - 1)) || (x == (len - 2)))
                    {
                        dm.Nd = dm.Nd + "</tr>";
                    }
                    x++;
                }

                //while ((x < (len - 1)) && (x < 16))
                //{
                //    if (x % 4 == 0)
                //    {
                //        dm.Nd = dm.Nd + "<tr>";
                //    }
                //    if (((x == 15) && (x < (len - 2))))
                //    {
                //        dm.Nd = dm.Nd + "<td class='rowlabel'>" + nd_dm[x] + "...</td>";
                //    }
                //    else
                //    {
                //        dm.Nd = dm.Nd + "<td class='rowlabel'>" + nd_dm[x] + "</td>";
                //    }
                //    if ((x % 4 == 3) || (x == (len - 2)))
                //    {
                //        dm.Nd = dm.Nd + "</tr>";
                //    }
                //    x++;
                //}

                dm.Nd = dm.Nd + "</table>";
            }
            else
            {
                dm.Nd = "";
            }

            return dm;
        }

        private void BindCampaignSubMaps(List<CampaignSubMap> submaps)
        {
            if (submaps.Count > 0)
            {
                rptDM.DataSource = submaps;
                rptDM.DataBind();
            }
        }

        private void BindCampaignSubMapsSummary(List<CampaignSubMap> submaps)
        {
            if (submaps.Count > 0)
            {
                //rptSubMapsSummary.DataSource = submaps;
                //rptSubMapsSummary.DataBind();
            }
        }

        private void BindCampaignDMS(List<CampaignDM> submaps)
        {
            if (submaps.Count > 0)
            {
                rptDM.DataSource = submaps;
                rptDM.DataBind();
            }
        }

        private void BindCampaignDMSSummary(List<CampaignDM> submaps)
        {
            if (submaps.Count > 0)
            {
                //rptSubMapsSummary.DataSource = submaps;
                //rptSubMapsSummary.DataBind();
            }
        }


        #region GetColorItems

        private List<ColorItem> GetColorItems(string colorItemsStr)
        {
            List<ColorItem> items = new List<ColorItem>();
            if (!string.IsNullOrEmpty(colorItemsStr))
            {
                string[] array = colorItemsStr.Split(';');
                foreach (string str in array)
                {
                    items.Add(GetColorItem(str));
                }
            }
            return items;
        }
        private ColorItem GetColorItem(string colorItemStr)
        {
            string[] array = colorItemStr.Split(',');
            ColorItem item = new ColorItem();
            item.Name = array[0];
            item.ColorString = array[1];
            item.Min = float.Parse(array[2]);
            item.Max = float.Parse(array[3]);
            item.R = float.Parse(array[4]);
            item.G = float.Parse(array[5]);
            item.B = float.Parse(array[6]);
            item.A = float.Parse(array[7]);
            return item;
        }


        #endregion


        #region GetCampaignSubMaps

        //private List<CampaignSubMap> GetCampaignSubMaps(string submapsStr)
        //{
        //    List<CampaignSubMap> submaps = new List<CampaignSubMap>();
        //    if (!string.IsNullOrEmpty(submapsStr))
        //    {
        //        string[] items = submapsStr.Split('^');

        //        foreach (string item in items)
        //        {
        //            submaps.Add(GetCampaignSubMap(item));
        //        }
        //    }
        //    return submaps;
        //}

        //private CampaignSubMap GetCampaignSubMap(string submapStr)
        //{
        //    CampaignSubMap submap = new CampaignSubMap();
        //    string[] items = submapStr.Split('*');
        //    submap.Id = items[0];
        //    submap.Name = items[1];
        //    submap.Total = int.Parse(items[2]).ToString("0,00");
        //    submap.Count = int.Parse(items[3]).ToString("0,00");
        //    submap.Pen = items[4];
        //    submap.MapImgUrl = CreateMapImage(items[5], items[6], items[7], items[8], items[9], items[14]);
        //    submap.FiveZips = GetSubItems(items[10]);
        //    submap.CRoutes = GetSubItems(items[11]);
        //    submap.Tracts = GetSubItems(items[12]);
        //    submap.BlockGroups = GetSubItems(items[13]);
        //    return submap;
        //}

        private List<CampaignDM> GetCampaignDMS(string submapsStr)
        {
            List<CampaignDM> dms = new List<CampaignDM>();
            if (!string.IsNullOrEmpty(submapsStr))
            {
                string[] items = submapsStr.Split('^');
                int i = 0;
                foreach (string item in items)
                {
                    dms.Add(GetCampaignDM(item,i));
                    i++;
                }
            }
            return dms;
        }

     

        #endregion

        #region CreateMapImage

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

        private string CreateMapImage(string imagesStr, string shapesStr, string pushpinsStr, string locationsStr, string scaleStr, string ndStr)
        {
            List<MapImage> images = GetMapImages(imagesStr);
            List<MapShape> shapes = GetMapShapes(shapesStr);
            List<MapPushpin> pushpins = GetMapPushpins(pushpinsStr);
            List<MapLocation> locations = GetMapLocations(locationsStr);
            ScaleItem scaleItem = GetScaleItem(scaleStr);
            List<MapLocation> ndLocations = GetMapLocations(ndStr);

            WebClient mywebclient = new WebClient();
            foreach (MapImage image in images)
            {
                if (!File.Exists(image.path))
                {
                    mywebclient.DownloadFile(image.url, image.path);
                }
            }

            return CreateMapImage(images, shapes, pushpins, locations, scaleItem, ndLocations);
        }


        private string CreateMapImage(List<MapImage> images, List<MapShape> shapes, List<MapPushpin> pushpins, List<MapLocation> locations, ScaleItem scaleItem, List<MapLocation> ndlocations)
        {
            int width = 2300;
            int height = 2300;
            if (ndlocations.Count > 0)
            {
                height = 2150;
            }
            string imageName = "merge" + DateTime.Now.Ticks.ToString() + ".png";
            string imagePath = mReportRoot + "\\Print\\images\\" + imageName;
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

            foreach (MapLocation location in locations)
            {
                System.Drawing.Image image = null;
                if (location.text == "red")
                {
                    image = locationRedImage;
                    g.DrawImage(image, location.x - 15, location.y - 15, 30, 30);
                }
                else
                {
                    image = locationGreenImage;
                    g.DrawImage(image, location.x - 15, location.y - 15, 30, 30);
                }

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

        #endregion

        #endregion



        private string SaveHtml(string campaignName)
        {
            string name = campaignName + ".html";
            name = Server.UrlEncode(name).Replace('+', '_').Replace('%', '_');
            string path = mReportRoot + "Print/" + name;
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

            string pdfPath = mReportRoot + "Print/" + name;
            try
            {
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
                //pdfConverter.PdfDocumentOptions.TopMargin = 35;
                pdfConverter.PdfDocumentOptions.LeftMargin = (int)(0.5 * 72);
                pdfConverter.PdfDocumentOptions.RightMargin = (int)(0.5 * 72);
                pdfConverter.PdfDocumentOptions.TopMargin = (int)(0.5 * 72);
                pdfConverter.PdfDocumentOptions.BottomMargin = (int)(0.5 * 72);
                pdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.Custom;
                pdfConverter.PdfDocumentOptions.CustomPdfPageSize = new SizeF((int)(24 * 72), (int)(27.5 * 72));
                //pdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A1;
                //pdfConverter.PageHeight = (int)(23 * 72);
                pdfConverter.PageWidth = (int)(35 * 72);
                pdfConverter.PdfDocumentOptions.AutoSizePdfPage = true;
                //pdfConverter.PdfDocumentOptions.FitHeight = true;
                //pdfConverter.PdfDocumentOptions.FitWidth = true;
                //pdfConverter.PdfDocumentOptions.StretchToFit = true;
                //pdfConverter.OptimizePdfPageBreaks = true;


                //pdfConverter.PdfFooterOptions.DrawFooterLine = true;
                //HtmlToPdfArea footArea = new HtmlToPdfArea(FooterUrl);
                
                //footArea.FitHeight = true;
                //footArea.FitWidth = true; 
                ////footArea.StretchToFit = true;
                //pdfConverter.PdfFooterOptions.AddHtmlToPdfArea(footArea);
                //pdfConverter.PdfFooterOptions.ShowPageNumber = false;
                //pdfConverter.PdfFooterOptions.FooterImageWidth = (int)(24 * 72);
                
                //pdfConverter.PdfFooterOptions.AddTextArea(new TextArea(265, 3,
                //                                                       "Page &p; of &P;",
                //                                                       new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, 6)));

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
            //string url = Request.MapPath(pdfPath.Substring(Server.MapPath("~/").Length));
            //string url = pdfPath.Substring(pdfPath.ToLower().IndexOf("\\print\\") + 7);
            Response.Redirect(mReportUrl + "Print/" + pdfPath);
        }


        protected override void Render(HtmlTextWriter writer)
        {
            //string path = Server.MapPath("~/Print/" + DateTime.Now.Ticks.ToString() + ".html");
            //StreamWriter sw = File.CreateText(path);
            //HtmlTextWriter hwriter = new HtmlTextWriter(sw);
            //base.Render(hwriter);
            //hwriter.Close();
            //sw.Close();



            //base.Render(writer);



            //System.IO.StringWriter html = new System.IO.StringWriter();
            //System.Web.UI.HtmlTextWriter tw = new System.Web.UI.HtmlTextWriter(html);
            //base.Render(tw);
            //System.IO.StreamWriter sw;
            //sw = new System.IO.StreamWriter(path, false, System.Text.Encoding.Default);
            //sw.Write(html.ToString());
            //sw.Close();
            //tw.Close();
            //Response.Write(html.ToString());
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
                
                CampaignDM submap = (CampaignDM)e.Item.DataItem;
                //Repeater rptFiveZips = e.Item.FindControl("rptFiveZips") as Repeater;
                //Repeater rptCRoutes = e.Item.FindControl("rptCRoutes") as Repeater;
                //Repeater rptTracts = e.Item.FindControl("rptTracts") as Repeater;
                //Repeater rptBlockGroups = e.Item.FindControl("rptBlockGroups") as Repeater;
                //Repeater rptColors = e.Item.FindControl("rptColors") as Repeater;

                //if (Colors.Count > 0)
                //{
                //    rptColors.DataSource = Colors;
                //    rptColors.DataBind();
                //}

                //if (submap.FiveZips.Count > 0)
                //{
                //    rptFiveZips.DataSource = submap.FiveZips;
                //    rptFiveZips.DataBind();
                //    foreach (RepeaterItem item in rptFiveZips.Controls)
                //    {
                //        if (item.ItemType == ListItemType.Header)
                //        {
                //            ((Literal)item.FindControl("ltlName")).Text = String.Format("SUB MAP {0} ({1})", submap.Id, submap.Name);
                //            break;
                //        }
                //    }
                //}
                //if (submap.CRoutes.Count > 0)
                //{
                //    rptCRoutes.DataSource = submap.CRoutes;
                //    rptCRoutes.DataBind();
                //    foreach (RepeaterItem item in rptCRoutes.Controls)
                //    {
                //        if (item.ItemType == ListItemType.Header)
                //        {
                //            ((Literal)item.FindControl("ltlName")).Text = String.Format("SUB MAP {0} ({1})", submap.Id, submap.Name);
                //            break;
                //        }
                //    }
                //}
                //if (submap.Tracts.Count > 0)
                //{
                //    rptTracts.DataSource = submap.Tracts;
                //    rptTracts.DataBind();
                //    foreach (RepeaterItem item in rptTracts.Controls)
                //    {
                //        if (item.ItemType == ListItemType.Header)
                //        {
                //            ((Literal)item.FindControl("ltlName")).Text = String.Format("SUB MAP {0} ({1})", submap.Id, submap.Name);
                //            break;
                //        }
                //    }
                //}
                //if (submap.BlockGroups.Count > 0)
                //{
                //    rptBlockGroups.DataSource = submap.BlockGroups;
                //    rptBlockGroups.DataBind();
                //    foreach (RepeaterItem item in rptBlockGroups.Controls)
                //    {
                //        if (item.ItemType == ListItemType.Header)
                //        {
                //            ((Literal)item.FindControl("ltlName")).Text = String.Format("SUB MAP {0} ({1})", submap.Id, submap.Name);
                //            break;
                //        }
                //    }
                //}
            }
        }

    }
}
