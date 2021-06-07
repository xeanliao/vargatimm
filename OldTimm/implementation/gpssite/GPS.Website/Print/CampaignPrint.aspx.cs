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
    public partial class CampaignPrint : System.Web.UI.Page
    {
        //private string UrlBase = System.Configuration.ConfigurationManager.AppSettings["UrlBase"];
        private string FooterUrl;
        protected string TargetingMethod;
        private List<ColorItem> Colors;

        private string mReportUrl = "";
        private string mReportRoot = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            mReportUrl = System.Configuration.ConfigurationManager.AppSettings["reportUrl"];
            mReportRoot = System.Configuration.ConfigurationManager.AppSettings["reportRoot"]; ;

            string campaign = Request.Form["campaign"];
            TargetingMethod = Request.Form["targetingmethod1"] + (string.IsNullOrEmpty(Request.Form["targetingmethod2"]) ? "" : "<br />" + Request.Form["targetingmethod2"]);
            string showCoverPage = Request.Form["showcoverpage"];
            if (showCoverPage.ToLower() != "true")
            {
                plCoverPage.Visible = false;
            }

            this.imgLogo.ImageUrl = WebUtil.GetAbsoluteUrl(this.Request, "Images/norms-logo.png");
            this.imgVargaincLogo.ImageUrl = WebUtil.GetAbsoluteUrl(this.Request, "images/vargainc-logo.png");
            this.imgDirectionLegend.ImageUrl = WebUtil.GetAbsoluteUrl(this.Request, "Images/direction-legend.png");
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
            string campaignName = items[0];
            string contactName = items[1];
            string clientName = items[2];
            string userFullName = items[3];
            string total = items[4];
            string count = items[5];
            string pen = items[6];
            string date = items[7];
            string image = items[8];
            string shape = items[9];
            string pushpin = items[10];
            string location = items[11];
            string scale = items[12];
            string submap = items[13];
            string color = items[14];
            string logo = items[15];

            FooterUrl = WebUtil.GetAbsoluteUrl(this.Request, string.Format("Print/Footer.aspx?campaign={0}&on={1}&for={2}&by={3}", Server.UrlEncode(campaignName), date, Server.UrlEncode(contactName), Server.UrlEncode(userFullName)));
            GPS.Utilities.DBLog.LogInfo("current Url: " + this.Request.Url.ToString());
            
            Colors = GetColorItems(color);
            Response.Write(Colors.Count);
            if (Colors.Count > 0)
            {
                rptColors.DataSource = Colors;
                rptColors.DataBind();
            }
            if (!string.IsNullOrEmpty(logo))
            {
                imgLogo.ImageUrl = "~/Files/Images/" + logo;
            }
            else
            {
                imgLogo.Visible = false;
            }

            ltlClient.Text = clientName;
            ltlCreatedFor.Text = contactName;
            ltlCreatedOn.Text = date;
            ltlMasterCampaign.Text = campaignName;
            ltlCreatedBy.Text = userFullName;


            //
            ltlCampaign.Text = campaignName;
            ltlClientName.Text = clientName;
            ltlContactName.Text = contactName;
            ltlTargetingMethod.Text = TargetingMethod;
            ltlTotal.Text = int.Parse(total).ToString("0,00");
            ltlCount.Text = int.Parse(count).ToString("0,00"); ;
            ltlPen.Text = pen;
            imgCampaignMap.ImageUrl = CreateMapImage(image, shape, pushpin, location, scale);
            List<CampaignSubMap> submaps = GetCampaignSubMaps(submap);
            BindCampaignSubMapsSummary(submaps);
            BindCampaignSubMaps(submaps);
            return items[0];
        }

        private void BindCampaignSubMaps(List<CampaignSubMap> submaps)
        {
            if (submaps.Count > 0)
            {
                rptSubMaps.DataSource = submaps;
                rptSubMaps.DataBind();
            }
        }

        private void BindCampaignSubMapsSummary(List<CampaignSubMap> submaps)
        {
            if (submaps.Count > 0)
            {
                rptSubMapsSummary.DataSource = submaps;
                rptSubMapsSummary.DataBind();
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

        private List<CampaignSubMap> GetCampaignSubMaps(string submapsStr)
        {
            List<CampaignSubMap> submaps = new List<CampaignSubMap>();
            if (!string.IsNullOrEmpty(submapsStr))
            {
                string[] items = submapsStr.Split('^');

                foreach (string item in items)
                {
                    submaps.Add(GetCampaignSubMap(item));
                }
            }
            return submaps;
        }

        private CampaignSubMap GetCampaignSubMap(string submapStr)
        {
            CampaignSubMap submap = new CampaignSubMap();
            string[] items = submapStr.Split('*');
            submap.Id = items[0];
            submap.Name = items[1];
            submap.Total = int.Parse(items[2]).ToString("0,00");
            submap.Count = int.Parse(items[3]).ToString("0,00");
            submap.Pen = items[4];
            submap.MapImgUrl = CreateMapImage(items[5], items[6], items[7], items[8], items[9]);
            submap.FiveZips = GetSubItems(items[10]);
            submap.CRoutes = GetSubItems(items[11]);
            submap.Tracts = GetSubItems(items[12]);
            submap.BlockGroups = GetSubItems(items[13]);
            return submap;
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

        private string CreateMapImage(string imagesStr, string shapesStr, string pushpinsStr, string locationsStr, string scaleStr)
        {
            List<MapImage> images = GetMapImages(imagesStr);
            List<MapShape> shapes = GetMapShapes(shapesStr);
            List<MapPushpin> pushpins = GetMapPushpins(pushpinsStr);
            List<MapLocation> locations = GetMapLocations(locationsStr);
            ScaleItem scaleItem = GetScaleItem(scaleStr);

            WebClient mywebclient = new WebClient();
            foreach (MapImage image in images)
            {
                if (!File.Exists(image.path))
                {
                    mywebclient.DownloadFile(image.url, image.path);
                }
            }

            return CreateMapImage(images, shapes, pushpins, locations, scaleItem);
        }


        private string CreateMapImage(List<MapImage> images, List<MapShape> shapes, List<MapPushpin> pushpins, List<MapLocation> locations, ScaleItem scaleItem)
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
            foreach (MapLocation location in locations)
            {
                System.Drawing.Image image = location.text == "red" ? locationRedImage : locationGreenImage;
                g.DrawImage(image, location.x - 15, location.y - 15, 30, 30);
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
            //return "images/" + imagePath.Remove(0, Server.MapPath("~/Print/images").Length + 1);
        }

        #endregion

        #endregion



        private string SaveHtml(string campaignName)
        {
            string name = campaignName + ".html";
            name = Server.UrlEncode(name).Replace('+', '_').Replace('%', '_');
            string path = mReportRoot + "Print\\" + name;
            GPS.Utilities.DBLog.LogInfo("save html: " + path);
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
            GPS.Utilities.DBLog.LogInfo("ConvertHtmlToPdf: " + pdfPath);
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
                pdfConverter.PdfDocumentOptions.ShowFooter = true;
                pdfConverter.PdfDocumentOptions.ShowHeader = false;
                pdfConverter.PdfDocumentOptions.GenerateSelectablePdf = true;
                pdfConverter.PdfDocumentOptions.LeftMargin = 10;
                pdfConverter.PdfDocumentOptions.RightMargin = 10;
                pdfConverter.PdfDocumentOptions.TopMargin = 35;
                
                pdfConverter.PdfFooterOptions.DrawFooterLine = true;
                GPS.Utilities.DBLog.LogInfo("absolute url of footer.aspx: " + FooterUrl);

                pdfConverter.PdfFooterOptions.AddHtmlToPdfArea(new HtmlToPdfArea(FooterUrl));
                pdfConverter.PdfFooterOptions.ShowPageNumber = false;
                pdfConverter.PdfFooterOptions.AddTextArea(new TextArea(265, 3,
                                                                       "Page &p; of &P;",
                                                                       new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, 6)));
                
                pdfConverter.SavePdfFromUrlToFile(htmlPath, pdfPath);

                GPS.Utilities.DBLog.LogInfo("pdf is generated");
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write(ex);
                GPS.Utilities.DBLog.LogInfo(ex.ToString());
            }
            return name;
        }

        private void OutputPdf(string pdfPath)
        {
            //string url = Request.MapPath(pdfPath.Substring(Server.MapPath("~/").Length));
            //string url = pdfPath.Substring(pdfPath.ToLower().IndexOf("\\print\\") + 7);
            Response.Redirect(mReportUrl + "print/" + pdfPath);
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

        protected void rptSubMaps_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.DataItem != null)
            {
                if (e.Item.FindControl("imgDirectionLegend2") != null)
                {
                    System.Web.UI.WebControls.Image img = (System.Web.UI.WebControls.Image)e.Item.FindControl("imgDirectionLegend2");
                    img.ImageUrl = WebUtil.GetAbsoluteUrl(this.Request, "Images/direction-legend.png");
                }
                
                CampaignSubMap submap = (CampaignSubMap)e.Item.DataItem;
                //Repeater rptFiveZips = e.Item.FindControl("rptFiveZips") as Repeater;
                //Repeater rptCRoutes = e.Item.FindControl("rptCRoutes") as Repeater;
                //Repeater rptTracts = e.Item.FindControl("rptTracts") as Repeater;
                //Repeater rptBlockGroups = e.Item.FindControl("rptBlockGroups") as Repeater;
                Repeater rptColors = e.Item.FindControl("rptColors") as Repeater;

                Literal ltlFiveZips = e.Item.FindControl("ltlFiveZips") as Literal;
                Literal ltlCRoutes = e.Item.FindControl("ltlCRoutes") as Literal;
                Literal ltlTracts = e.Item.FindControl("ltlTracts") as Literal;
                Literal ltlBlockGroups = e.Item.FindControl("ltlBlockGroups") as Literal;

                if (Colors.Count > 0)
                {
                    rptColors.DataSource = Colors;
                    rptColors.DataBind();
                }

                if (submap.FiveZips.Count > 0)
                {
                    //rptFiveZips.DataSource = submap.FiveZips;
                    //rptFiveZips.DataBind();
                    //foreach (RepeaterItem item in rptFiveZips.Controls)
                    //{
                    //    if (item.ItemType == ListItemType.Header)
                    //    {
                    //        ((Literal)item.FindControl("ltlName")).Text = String.Format("SUB MAP {0} ({1})", submap.Id, submap.Name);
                    //        break;
                    //    }
                    //}

                    int currentPageIndex = 0;
                    int pageSize = 52;
                    string str = "";

                    for (int i = 0; i < submap.FiveZips.Count; i++)
                    {
                        if (i % pageSize == 0)
                        {
                            str += "<table cellspacing='0' cellpadding='4'style='margin-bottom:3px'>" +
                                        "<caption>ZIP CODES CONTAINED IN " + String.Format("SUB MAP {0} ({1})", submap.Id, submap.Name) + "</caption>" +
                                        "<tbody><tr style='background-color: #eeeeee;'>" +
                                        "<td class='collabel' style='width: 10%'>#</td>" +
                                        "<td class='collabel' style='width: 30%'>" +
                                           "ZIP CODE " +
                                        "</td>" +
                                        "<td class='collabel' style='width: 20%; text-align: right;'>" +
                                           "TOTAL H/H" +
                                        "</td>" +
                                        "<td class='collabel' style='width: 20%; text-align: right;'>" +
                                           "TARGET H/H" +
                                        "</td>" +
                                        "<td class='collabel' style='width: 20%; text-align: right;'>" +
                                           "PENETRATION</td></tr>";



                            for (int j = currentPageIndex * pageSize; j < currentPageIndex * pageSize + pageSize; j++)
                            {
                                if (j < submap.FiveZips.Count)
                                {
                                    str += "<tr" + (j % 2 == 0 ? "style = 'background-color: #eeeeee;'" : "") + ">" +
                                               "<td>" + submap.FiveZips[j].OrderId + "</td>" +
                                               "<td>" + submap.FiveZips[j].Code + "</td>" +
                                               "<td style='text-align: right;'>" + submap.FiveZips[j].Total + "</td>" +
                                               "<td style='text-align: right;'>" + submap.FiveZips[j].Count + "</td>" +
                                               "<td style='text-align: right;'>" + submap.FiveZips[j].Pen + "</td></tr>";
                                }
                            }

                            str += "</table>";

                            currentPageIndex++;
                        }
                    }

                    ltlFiveZips.Text = str;
                }

                if (submap.CRoutes.Count > 0)
                {
                    //rptCRoutes.DataSource = submap.CRoutes;
                    //rptCRoutes.DataBind();
                    //foreach (RepeaterItem item in rptCRoutes.Controls)
                    //{
                    //    if (item.ItemType == ListItemType.Header)
                    //    {
                    //        ((Literal)item.FindControl("ltlName")).Text = String.Format("SUB MAP {0} ({1})", submap.Id, submap.Name);
                    //        break;
                    //    }
                    //}

                    submap.CRoutes = submap.CRoutes.FindAll(s => s.Pen != "");

                    int currentPageIndex = 0;                   
                    int pageSize = 52;
                    string str = ""; 

                    for (int i = 0; i < submap.CRoutes.Count;i++ )
                    {
                        if (i % pageSize == 0 )
                        {
                            str += "<table cellspacing='0' cellpadding='4' style='margin-bottom:3px'>" +
                                        "<caption>CARRIER ROUTES CONTAINED IN " + String.Format("SUB MAP {0} ({1})", submap.Id, submap.Name) + "</caption>" +
                                        "<tbody><tr style='background-color: #eeeeee;'>" +
                                        "<td class='collabel' style='width: 10%'>#</td>" +
                                        "<td class='collabel' style='width: 30%'>" +
                                           "CARRIER ROUTE #" +
                                        "</td>" +
                                        "<td class='collabel' style='width: 20%; text-align: right;'>" +
                                           "TOTAL H/H" +
                                        "</td>" +
                                        "<td class='collabel' style='width: 20%; text-align: right;'>" +
                                           "TARGET H/H" +
                                        "</td>" +
                                        "<td class='collabel' style='width: 20%; text-align: right;'>" +
                                           "PENETRATION</td></tr>";



                            for (int j = currentPageIndex * pageSize; j < currentPageIndex * pageSize + pageSize; j++)
                            {
                                if (j < submap.CRoutes.Count)
                                {
                                    str += "<tr" + (j % 2 == 0 ? "style = 'background-color: #eeeeee;'" : "") + ">" +
                                               "<td>" + submap.CRoutes[j].OrderId + "</td>" +
                                               "<td>" + submap.CRoutes[j].Code + "</td>" +
                                               "<td style='text-align: right;'>" + submap.CRoutes[j].Total + "</td>" +
                                               "<td style='text-align: right;'>" + submap.CRoutes[j].Count + "</td>" +
                                               "<td style='text-align: right;'>" + submap.CRoutes[j].Pen + "</td></tr>";
                                }
                            }

                            str += "</table>";

                            currentPageIndex++;
                        }
                    }

                    ltlCRoutes.Text = str;
                }


                if (submap.Tracts.Count > 0)
                {
                    //rptTracts.DataSource = submap.Tracts;
                    //rptTracts.DataBind();
                    //foreach (RepeaterItem item in rptTracts.Controls)
                    //{
                    //    if (item.ItemType == ListItemType.Header)
                    //    {
                    //        ((Literal)item.FindControl("ltlName")).Text = String.Format("SUB MAP {0} ({1})", submap.Id, submap.Name);
                    //        break;
                    //    }
                    //}

                    int currentPageIndex = 0;
                    int pageSize = 52;
                    string str = "";

                    for (int i = 0; i < submap.Tracts.Count; i++)
                    {
                        if (i % pageSize == 0)
                        {
                            str += "<table cellspacing='0' cellpadding='4'style='margin-bottom:3px'>" +
                                        "<caption>CENSUS TRACTS CONTAINED IN " + String.Format("SUB MAP {0} ({1})", submap.Id, submap.Name) + "</caption>" +
                                        "<tbody><tr style='background-color: #eeeeee;'>" +
                                        "<td class='collabel' style='width: 10%'>#</td>" +
                                        "<td class='collabel' style='width: 30%'>" +
                                           "CENSUS TRACT # " +
                                        "</td>" +
                                        "<td class='collabel' style='width: 20%; text-align: right;'>" +
                                           "TOTAL H/H" +
                                        "</td>" +
                                        "<td class='collabel' style='width: 20%; text-align: right;'>" +
                                           "TARGET H/H" +
                                        "</td>" +
                                        "<td class='collabel' style='width: 20%; text-align: right;'>" +
                                           "PENETRATION</td></tr>"; 


                            for (int j = currentPageIndex * pageSize; j < currentPageIndex * pageSize + pageSize; j++)
                            {
                                if (j < submap.Tracts.Count)
                                {
                                    str += "<tr" + (j % 2 == 0 ? "style = 'background-color: #eeeeee;'" : "") + ">" +
                                               "<td>" + submap.Tracts[j].OrderId + "</td>" +
                                               "<td>" + submap.Tracts[j].Code + "</td>" +
                                               "<td style='text-align: right;'>" + submap.Tracts[j].Total + "</td>" +
                                               "<td style='text-align: right;'>" + submap.Tracts[j].Count + "</td>" +
                                               "<td style='text-align: right;'>" + submap.Tracts[j].Pen + "</td></tr>";
                                }
                            }

                            str += "</table>";

                            currentPageIndex++;
                        }
                    }

                    ltlTracts.Text = str; 
                }


                if (submap.BlockGroups.Count > 0)
                {
                    //rptBlockGroups.DataSource = submap.BlockGroups;
                    //rptBlockGroups.DataBind();
                    //foreach (RepeaterItem item in rptBlockGroups.Controls)
                    //{
                    //    if (item.ItemType == ListItemType.Header)
                    //    {
                    //        ((Literal)item.FindControl("ltlName")).Text = String.Format("SUB MAP {0} ({1})", submap.Id, submap.Name);
                    //        break;
                    //    }
                    //}

                    int currentPageIndex = 0;
                    int pageSize = 52;
                    string str = "";

                    for (int i = 0; i < submap.BlockGroups.Count; i++)
                    {
                        if (i % pageSize == 0)
                        {
                            str += "<table cellspacing='0' cellpadding='4'style='margin-bottom:3px'>" +
                                        "<caption>BG'S CONTAINED IN " + String.Format("SUB MAP {0} ({1})", submap.Id, submap.Name) + "</caption>" +
                                        "<tbody><tr style='background-color: #eeeeee;'>" +
                                        "<td class='collabel' style='width: 10%'>#</td>" +
                                        "<td class='collabel' style='width: 30%'>" +
                                           "BLOCK GROUP # " +
                                        "</td>" +
                                        "<td class='collabel' style='width: 20%; text-align: right;'>" +
                                           "TOTAL H/H" +
                                        "</td>" +
                                        "<td class='collabel' style='width: 20%; text-align: right;'>" +
                                           "TARGET H/H" +
                                        "</td>" +
                                        "<td class='collabel' style='width: 20%; text-align: right;'>" +
                                           "PENETRATION</td></tr>";


                            for (int j = currentPageIndex * pageSize; j < currentPageIndex * pageSize + pageSize; j++)
                            {
                                if (j < submap.BlockGroups.Count)
                                {
                                    str += "<tr" + (j % 2 == 0 ? "style = 'background-color: #eeeeee;'" : "") + ">" +
                                               "<td>" + submap.BlockGroups[j].OrderId + "</td>" +
                                               "<td>" + submap.BlockGroups[j].Code + "</td>" +
                                               "<td style='text-align: right;'>" + submap.BlockGroups[j].Total + "</td>" +
                                               "<td style='text-align: right;'>" + submap.BlockGroups[j].Count + "</td>" +
                                               "<td style='text-align: right;'>" + submap.BlockGroups[j].Pen + "</td></tr>";
                                }
                            }

                            str += "</table>";

                            currentPageIndex++;
                        }
                    }

                    ltlBlockGroups.Text = str; 
                }
            }
        }
    }
}
