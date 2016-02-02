using log4net;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using Newtonsoft.Json.Linq;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using Vargainc.Timm.EF;
using Vargainc.Timm.Extentions;
using Vargainc.Timm.Models;

namespace Vargainc.Timm.REST.Controllers
{

    public class PrintSize
    {
        enum PrintSizeEnum
        {
            Letter,
            A4,
            A3,
            Distribute
        }

        public Unit Width { get; set; }
        public Unit Height { get; set; }
        public Unit PageWidth { get; set; }
        public Unit PageHeight { get; set; }
        public Unit Top { get; set; }

        public Unit Bottom { get; set; }
        public Unit LeftRight { get; set; }
        public PrintSize(string size)
        {
            PrintSizeEnum printSize;
            if (string.IsNullOrWhiteSpace(size))
            {
                printSize = PrintSizeEnum.A4;
            }
            else if (!Enum.TryParse<PrintSizeEnum>(size, out printSize))
            {
                printSize = PrintSizeEnum.A4;
            }
            switch (printSize)
            {
                case PrintSizeEnum.Letter:
                    Width = Unit.FromInch(8.5);
                    Height = Unit.FromInch(11);
                    Top = Unit.FromInch(0.5);
                    Bottom = Unit.FromInch(0.25);
                    LeftRight = Unit.FromInch(0.25);
                    PageWidth = Unit.FromInch(8.5) - LeftRight * 2;
                    PageHeight = Unit.FromInch(11) - Top - Bottom;
                    break;
                case PrintSizeEnum.A4:
                    Width = Unit.FromInch(8.3);
                    Height = Unit.FromInch(11.7);
                    Top = Unit.FromInch(0.5);
                    Bottom = Unit.FromInch(0.25);
                    LeftRight = Unit.FromInch(0.25);
                    PageWidth = Unit.FromInch(8.3) - LeftRight * 2;
                    PageHeight = Unit.FromInch(11.7) - Top - Bottom;
                    break;
                case PrintSizeEnum.A3:
                    Width = Unit.FromInch(11.7);
                    Height = Unit.FromInch(16.5);
                    Top = Unit.FromInch(1.25);
                    Bottom = Unit.FromInch(0.75);
                    LeftRight = Unit.FromInch(0.75);
                    PageWidth = Unit.FromInch(11.7) - LeftRight * 2;
                    PageHeight = Unit.FromInch(16.5) - Top - Bottom;
                    break;
                case PrintSizeEnum.Distribute:
                    Width = Unit.FromInch(24);
                    Height = Unit.FromInch(24);
                    Top = Unit.FromInch(0);
                    Bottom = Unit.FromInch(0);
                    LeftRight = Unit.FromInch(0);
                    PageWidth = Unit.FromInch(24);
                    PageHeight = Unit.FromInch(24);
                    break;
                default:
                    Width = Unit.FromInch(8.3);
                    Height = Unit.FromInch(11.7);
                    Top = Unit.FromInch(0.5);
                    Bottom = Unit.FromInch(0.25);
                    LeftRight = Unit.FromInch(0.25);
                    PageWidth = Unit.FromInch(8.3) - LeftRight * 2;
                    PageHeight = Unit.FromInch(11.7) - Top - Bottom;
                    break;
            }
        }
    }

    [RoutePrefix("pdf")]
    public class PDFController : ApiController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(PDFController));
        private TimmContext db = new TimmContext();

        private static string TempFolder;
        private static string ImageFolder;


        static PDFController()
        {
            TempFolder = HostingEnvironment.MapPath("~/pdf_temp");
            if (!System.IO.Directory.Exists(TempFolder))
            {
                System.IO.Directory.CreateDirectory(TempFolder);
            }
            ImageFolder = HostingEnvironment.MapPath("~/images");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("build")]
        public async Task<IHttpActionResult> Export()
        {
            var postForm = await Request.Content.ReadAsFormDataAsync();
            var postString = postForm["options"];
            if (string.IsNullOrWhiteSpace(postString))
            {
                return NotFound();
            }
            JObject postData = null;
            try
            {
                postData = JObject.Parse(postString);
            }
            catch
            {
                
            }

            var id = postData.Value<int>("campaignId");
            var campaign = await db.Campaigns.FindAsync(id);
            if (campaign == null)
            {
                return NotFound();
            }

            var pageSize = new PrintSize(postData.Value<string>("size"));

            DateTime now = DateTime.Now;
            string type = postData.Value<string>("type");
            string filename = string.Format("{0}_{1}_{2}.pdf", campaign.ClientName, campaign.Name, now.ToString("yyyy-MM-dd"));

            var needFooter = postData.Value<bool?>("needFooter");

            Document doc = new Document();

            doc.Info.Title = "Campaign Report";
            doc.Info.Author = "Varga Media Solutions";
            doc.DefaultPageSetup.PageWidth = pageSize.Width;
            doc.DefaultPageSetup.PageHeight = pageSize.Height;
            doc.DefaultPageSetup.LeftMargin = pageSize.LeftRight;
            doc.DefaultPageSetup.RightMargin = pageSize.LeftRight;
            doc.DefaultPageSetup.TopMargin = pageSize.Top;
            if(needFooter == true)
            {
                doc.DefaultPageSetup.BottomMargin = pageSize.Bottom + Unit.FromPoint(30);
                doc.DefaultPageSetup.FooterDistance = pageSize.Bottom;
            }
            else
            {
                doc.DefaultPageSetup.BottomMargin = pageSize.Bottom;
            }
            


            doc.DefaultPageSetup.DifferentFirstPageHeaderFooter = false;

            doc.Styles.Normal.ParagraphFormat.Font.Name = "Helvetica";

            
            foreach (JObject page in postData["options"])
            {
                var section = doc.AddSection();

                var pageType = page.Value<string>("type");
                switch (pageType)
                {
                    case "cover":
                        BuildCover(section, page["options"] as JArray);
                        break;
                    default:
                        BuildPage(campaign, pageSize, section, page["options"] as JArray);
                        break;
                }
                if(needFooter == true)
                {
                    BuildFooter(section, pageSize, campaign);
                }
                
            }

            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true, PdfSharp.Pdf.PdfFontEmbedding.Always);
            renderer.Document = doc;
            renderer.RenderDocument();

            var pdfFileName = Guid.NewGuid().ToString();
            var basePath = TempFolder;
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
            var pdfPath = Path.Combine(basePath, pdfFileName + ".pdf");
            renderer.PdfDocument.Save(pdfPath);

            return Json(new {
                fileName = filename,
                sourceFile = pdfFileName
            });
        }

        [HttpGet]
        [Route("download/{campaignId:int}/{sourceFile:guid}")]
        public async Task<IHttpActionResult> Download(int campaignId, Guid sourceFile)
        {
            var campaign = await db.Campaigns.FindAsync(campaignId);
            if (campaign == null)
            {
                return NotFound();
            }
            var basePath = TempFolder;
            var srcFileName = sourceFile.ToString() + ".pdf";
            var srcFilePath = Path.Combine(basePath, srcFileName);
            if (!File.Exists(srcFilePath))
            {
                return NotFound();
            }
            DateTime now = DateTime.Now;
            string fileName = string.Format("{0}.pdf", campaign.ClientName);
            return new PDFResult(fileName, srcFilePath);
        }

        [HttpPost]
        [Route("download/images")]
        public async Task<IHttpActionResult> MergeMapImage()
        {
            var postForm = await Request.Content.ReadAsFormDataAsync();
            var mapImage = postForm["foreground"];
            var bgImage = postForm["background"];
            var postWidth = postForm["width"];
            var postHeight = postForm["height"];
            float width;
            float height;
            Unit unitWidth;
            Unit unitHeight;
            if(!float.TryParse(postWidth, out width))
            {
                unitWidth = Unit.FromInch(24);
            }
            else
            {
                unitWidth = Unit.FromPoint(width);
            }
            if(!float.TryParse(postHeight, out height))
            {
                unitHeight = Unit.FromInch(21);
            }
            else
            {
                unitHeight = Unit.FromPoint(height);
            }
            var imagePath = MergeMap(mapImage, bgImage, (float)unitWidth.Inch, (float)unitHeight.Inch);
            return new FileResult("image.jpg", imagePath);
        }

        private void BuildCover(Section sec, JArray options)
        {
            Unit fontSize = Unit.FromPoint(12);

            foreach (var item in options)
            {
                var title = item.Value<string>("title");
                if (title != null)
                {
                    Unit titleHight;
                    RenderTitle(sec, title, out titleHight);
                    continue;
                }

                var key = item.Value<string>("key");
                if (key != null)
                {
                    Paragraph keyPara = sec.AddParagraph();
                    keyPara.Format.Alignment = ParagraphAlignment.Center;
                    keyPara.Format.Font.Size = fontSize;
                    keyPara.Format.LineSpacingRule = LineSpacingRule.Single;
                    keyPara.Format.Font.Bold = true;
                    keyPara.AddText(key);
                }

                var text = item.Value<string>("text");
                if (text != null)
                {
                    var textPara = sec.AddParagraph();
                    textPara.Format.Alignment = ParagraphAlignment.Center;
                    textPara.Format.Font.Size = fontSize;
                    textPara.Format.LineSpacingRule = LineSpacingRule.Single;
                    textPara.AddText(text);
                }

                var imageSrc = item.Value<string>("image");
                if (imageSrc != null)
                {
                    var imageHeight = item.Value<int>("height");
                    imageHeight = imageHeight > 0 ? imageHeight : 40;

                    var imgPath = GetImage(imageSrc);
                    if (string.IsNullOrWhiteSpace(imgPath))
                    {
                        var imgFrame = sec.AddTextFrame();
                        imgFrame.Height = Unit.FromPoint(imageHeight);
                        continue;
                    }



                    var imgPara = sec.AddParagraph();
                    var align = item.Value<string>("align");
                    switch (align)
                    {
                        case "center":
                            imgPara.Format.Alignment = ParagraphAlignment.Center;
                            break;
                        case "right":
                            imgPara.Format.Alignment = ParagraphAlignment.Right;
                            break;
                        default:
                            imgPara.Format.Alignment = ParagraphAlignment.Left;
                            break;
                    }

                    var img = imgPara.AddImage(imgPath);
                    img.LockAspectRatio = true;
                    img.Height = Unit.FromPoint(imageHeight);
                    var top = item.Value<int>("top");
                    var bottom = item.Value<int>("bottom");

                    imgPara.Format.SpaceBefore = Unit.FromPoint(top);
                    imgPara.Format.SpaceAfter = Unit.FromPoint(bottom);


                }

                sec.LastParagraph.Format.SpaceAfter += Unit.FromPoint(20);
            }
        }

        private void BuildPage(Campaign campaign, PrintSize pageSize, Section sec, JArray options)
        {
            Unit fontSize = Unit.FromPoint(10);
            Unit addedHeight = 0; 
            var lineHeight = Unit.FromPoint(15);
            foreach (var item in options)
            {
                var title = item.Value<string>("title");
                if (title != null)
                {
                    Unit titleHeight;
                    RenderTitle(sec, title, out titleHeight);
                    addedHeight += titleHeight;
                    continue;
                }

                var list = item["list"] as JArray;

                if (list != null)
                {
                    var listTable = sec.AddTable();
                    listTable.Rows.LeftIndent = fontSize;
                    listTable.AddColumn(pageSize.PageWidth / 3);
                    listTable.AddColumn(pageSize.PageWidth * 2 / 3);
                    listTable.Rows.Height = lineHeight;
                    listTable.Rows.VerticalAlignment = VerticalAlignment.Center;
                    listTable.Format.Font.Size = fontSize;
                    foreach (var data in list)
                    {
                        var row = listTable.AddRow();
                        row.Height = lineHeight;

                        row.Cells[0].AddParagraph(data.Value<string>("key"));
                        row.Cells[1].AddParagraph(data.Value<string>("text"));

                        addedHeight += lineHeight;
                    }
                }

                var table = item.Value<string>("table");
                if (table != null)
                {
                    BuildTable(campaign, sec, pageSize, table, item);
                }

                var mapSrc = item.Value<string>("map");
                if (mapSrc != null)
                {
                    var needLegend = item.Value<bool>("legend");
                    var colors = new List<int>();
                    if (item["color"] != null && item["color"].HasValues)
                    {
                        colors = item["color"].ToObject<List<int>>();
                    }

                    var bgSrc = item.Value<string>("bg");

                    var mapBox = sec.AddTextFrame();
                    
                    mapBox.Width = pageSize.PageWidth;
                    // map image height = page height - (title list row) - legend - footer - 2pt box line
                    mapBox.Height = pageSize.PageHeight - addedHeight;
                    if (needLegend || (colors != null && colors.Count > 0))
                    {
                        mapBox.Height = mapBox.Height - Unit.FromPoint(34) - Unit.FromPoint(45);
                    }

                    var lineColor = System.Drawing.ColorTranslator.FromHtml("#94B43D");
                    mapBox.LineFormat.Color = new Color(lineColor.R, lineColor.G, lineColor.B);
                    mapBox.WrapFormat.Style = WrapStyle.None;

                    var mapImagePath = MergeMap(mapSrc, bgSrc, (float)mapBox.Width.Inch, (float)mapBox.Height.Inch);
                    if (string.IsNullOrWhiteSpace(mapImagePath))
                    {
                        continue;
                    }
                    var mapImage = mapBox.AddImage(mapImagePath);
                    mapImage.LockAspectRatio = false;
                    mapImage.Width = mapBox.Width;
                    mapImage.Height = mapBox.Height;


                    //legend
                    if (needLegend || (colors != null && colors.Count > 0))
                    {
                        var legendHeight = Unit.FromPoint(36);
                        var legend = sec.AddTable();

                        legend.AddColumn(pageSize.PageWidth - legendHeight);
                        legend.AddColumn(legendHeight);

                        //need an empty row for the map image textframe
                        var mapboxRow = legend.AddRow();
                        mapboxRow.Height = mapBox.Height;

                        var row = legend.AddRow();
                        row.Height = legendHeight;
                        row.VerticalAlignment = VerticalAlignment.Center;

                        if (colors != null && colors.Count > 0)
                        {
                            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                            row.Cells[0].AddParagraph("COLOR LEGEND");
                            var colorTips = row.Cells[0].AddParagraph();

                            colorTips.Format.Font.Size = Unit.FromPoint(9);
                            colors.Insert(0, 0);
                            colors.Add(100);
                            var colorMin = 0;
                            string[] DefaultColors = new string[5] { "Blue", "Green", "Yellow", "Orange", "Red" };
                            for (var colorIndex = 0; colorIndex < colors.Count - 1; colorIndex++)
                            {
                                {
                                    if (colorMin >= colors[colorIndex + 1])
                                    {
                                        continue;
                                    }

                                    var colorImage = colorTips.AddImage(System.IO.Path.Combine(ImageFolder, string.Format("{0}-legend.png", DefaultColors[colorIndex])));
                                    colorImage.Width = Unit.FromPoint(16);
                                    colorImage.Height = Unit.FromPoint(10);
                                    colorImage.LockAspectRatio = true;
                                    colorImage.WrapFormat.DistanceTop = Unit.FromPoint(5);
                                    colorTips.AddSpace(2);
                                    colorTips.AddText(string.Format("{0}({1}%-{2}%)", DefaultColors[colorIndex], colors[colorIndex], colors[colorIndex + 1]));
                                    colorTips.AddSpace(5);

                                    colorMin = colors[colorIndex + 1];
                                }
                            }
                        }
                        if (needLegend)
                        {
                            row.Cells[1].Format.Alignment = ParagraphAlignment.Center;
                            var directionImage = row.Cells[1].AddImage(System.IO.Path.Combine(ImageFolder, "direction-legend.jpg"));
                            directionImage.Width = Unit.FromPoint(24);
                            directionImage.Height = Unit.FromPoint(24);
                            directionImage.LockAspectRatio = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// foot height 43pt = (1 row top line + 5 row text) x fontsize 7 + top border 1pt
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="pageSize"></param>
        /// <param name="campaign"></param>
        private void BuildFooter(Section sec, PrintSize pageSize, Campaign campaign)
        {
            var footerBox = sec.Footers.Primary.AddTextFrame();
            footerBox.Width = pageSize.PageWidth;
            footerBox.Height = Unit.FromPoint(43);
            //fix footer left space
            footerBox.Left = Unit.FromPoint(3);

            const int fontSizeValue = 7;
            var leftImageSize = Unit.FromPoint(90);
            var rightImageSize = Unit.FromPoint(120);
            var footerTable = footerBox.AddTable();
            
            Unit fontSize = Unit.FromPoint(fontSizeValue);

            //fix width + 2pt
            var mainWidth = pageSize.PageWidth - leftImageSize - rightImageSize + Unit.FromPoint(2);
            footerTable.Format.Font.Size = fontSize;
            footerTable.AddColumn(leftImageSize);
            footerTable.AddColumn(mainWidth);
            footerTable.AddColumn(rightImageSize);

            var lineRow = footerTable.AddRow();
            lineRow.HeadingFormat = true;
            lineRow.Height = Unit.FromPoint(1);
            lineRow.Borders.Top.Color = Colors.Black;
            lineRow.Borders.Top.Width = Unit.FromPoint(1);

            var mc = string.Format("MC#: {0}-{1}-{2}-{3}-{4}",
                    campaign.Date.Value.ToString("MMddyy"),
                    campaign.ClientCode,
                    campaign.CreatorName,
                    campaign.AreaDescription,
                    campaign.Sequence);

            string[][] footerContent = new string[][] {
                new string[]{ mc, "www.vargainc.com" },
                new string[]{ string.Format("Created on: {0}", campaign.Date.Value.ToString("MMMM dd, yyyy")), "PH:949-768-1500" },
                new string[]{ string.Format("Created for: {0}", campaign.ContactName), "FX:949-768-1501" },
                new string[]{ string.Format("Created by: {0}", campaign.CreatorName), String.Format("© {0} Varga Media Solutions,Inc.All rights reserved.", DateTime.Now.Year) },
            };
            for (var i = 0; i < 4; i++)
            {
                var row = footerTable.AddRow();
                row.Height = fontSize;
                var right = row.Cells[1].AddParagraph(footerContent[i][1]);
                right.Format.Alignment = ParagraphAlignment.Right;
                right.Format.SpaceAfter = -fontSize;

                var left = row.Cells[1].AddTextFrame();
                var p = left.AddParagraph(footerContent[i][0]);
                p.Format.Font.Size = fontSize;
                left.Width = mainWidth;
                left.Height = fontSize;
                left.Left = 0;
            }

            footerTable.Rows[1].Cells[0].MergeDown = 3;
            footerTable.Rows[1].Cells[0].VerticalAlignment = VerticalAlignment.Center;
            footerTable.Rows[1].Cells[0].Format.Alignment = ParagraphAlignment.Left;
            var leftImagePath = System.IO.Path.Combine(ImageFolder, "vargainc-logo.png");
            var leftImage = footerTable.Rows[1].Cells[0].AddImage(leftImagePath);
            leftImage.Width = leftImageSize * 0.9;
            leftImage.LockAspectRatio = true;

            footerTable.Rows[1].Cells[2].MergeDown = 3;
            footerTable.Rows[1].Cells[2].VerticalAlignment = VerticalAlignment.Top;
            footerTable.Rows[1].Cells[2].Format.Alignment = ParagraphAlignment.Right;
            var rightImagePath = System.IO.Path.Combine(ImageFolder, "timm-logo-print.jpg");
            var rightImage = footerTable.Rows[1].Cells[2].AddImage(rightImagePath);
            rightImage.Width = rightImageSize * 0.95;
            rightImage.LockAspectRatio = true;
        }

        private void RenderTitle(Section section, string title, out Unit titleHeight)
        {
            titleHeight = Unit.FromPoint(25);
            var fontSize = title.Length > 60 ? Unit.FromPoint(12) : Unit.FromPoint(14);
            fontSize = title.Length > 80 ? Unit.FromPoint(10) : fontSize;

            var frame = section.AddTextFrame();
            frame.Width = section.Document.DefaultPageSetup.PageWidth - section.Document.DefaultPageSetup.LeftMargin - section.Document.DefaultPageSetup.RightMargin;
            frame.Height = titleHeight;
            var bgColor = System.Drawing.ColorTranslator.FromHtml("#94B43D");
            frame.FillFormat.Color = new Color(bgColor.R, bgColor.G, bgColor.B);
            frame.LineFormat.Color = new Color(bgColor.R, bgColor.G, bgColor.B);

            var paragraph = frame.AddParagraph(title);
            paragraph.Format.Font.Size = fontSize;
            paragraph.Format.LineSpacingRule = LineSpacingRule.Single;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.Format.Font.Bold = true;
            paragraph.Format.SpaceBefore = (titleHeight - fontSize) * 0.5;
        }

        private void BuildTable(Campaign campaign, Section sec, PrintSize pageSize, string dataSource, JToken options)
        {
            var tableBorderColor = new Color(0xDD, 0xDD, 0xDD);
            var rowBgColor = new Color(0xEE, 0xEE, 0xEE);
            var table = sec.AddTable();
            table.Rows.LeftIndent = 0;
            table.LeftPadding = Unit.FromPoint(9);
            table.Format.Font.Size = Unit.FromPoint(9);
            table.Rows.VerticalAlignment = VerticalAlignment.Center;
            table.Rows.Height = Unit.FromPoint(14.8);

            switch (dataSource)
            {
                case "submap-list":
                    {
                        var items = campaign.SubMaps.OrderBy(i => i.Id).Select(s => new
                        {
                            s.OrderId,
                            s.Name,
                            TotalHouseHold = s.Total ?? 0 + s.TotalAdjustment ?? 0,
                            TargetHouseHold = s.Penetration ?? 0 + s.CountAdjustment ?? 0,
                            Penetration = (s.Total ?? 0 + s.TotalAdjustment ?? 0) > 0 ? (float)(s.Penetration ?? 0 + s.CountAdjustment ?? 0) / (float)(s.Total ?? 0 + s.TotalAdjustment ?? 0) : 0
                        }).ToArray();

                        var column = table.AddColumn(Unit.FromPoint(30));
                        column.Format.Alignment = ParagraphAlignment.Center;
                        column = table.AddColumn(pageSize.PageWidth - Unit.FromPoint(330));
                        column = table.AddColumn(Unit.FromPoint(100));
                        column.Format.Alignment = ParagraphAlignment.Right;
                        column = table.AddColumn(Unit.FromPoint(100));
                        column.Format.Alignment = ParagraphAlignment.Right;
                        column = table.AddColumn(Unit.FromPoint(100));
                        column.Format.Alignment = ParagraphAlignment.Right;

                        var header = table.AddRow();
                        header.Cells[0].AddParagraph("#");
                        header.Cells[1].AddParagraph("SUB MAP NAME");
                        header.Cells[2].AddParagraph("TOTAL H/H");
                        header.Cells[3].AddParagraph("TARGET H/H");
                        header.Cells[4].AddParagraph("PENETRATION");

                        foreach (var item in items)
                        {
                            var row = table.AddRow();
                            row.Cells[0].AddParagraph(item.OrderId.ToString());
                            row.Cells[1].AddParagraph(item.Name);
                            row.Cells[2].AddParagraph(item.TotalHouseHold.ToString("#,###"));
                            row.Cells[3].AddParagraph(item.TargetHouseHold.ToString("#,###"));
                            row.Cells[4].AddParagraph(item.Penetration.ToString("p2"));
                        }
                    }
                    break;
                case "submap-detail":
                    {
                        var submapId = options.Value<int>("submapId");
                        var record = new PrintController().LoadRecordInSubmap(campaign, submapId);

                        var column = table.AddColumn(Unit.FromPoint(30));
                        column.Format.Alignment = ParagraphAlignment.Center;
                        column = table.AddColumn(pageSize.PageWidth - Unit.FromPoint(330));
                        column = table.AddColumn(Unit.FromPoint(100));
                        column.Format.Alignment = ParagraphAlignment.Right;
                        column = table.AddColumn(Unit.FromPoint(100));
                        column.Format.Alignment = ParagraphAlignment.Right;
                        column = table.AddColumn(Unit.FromPoint(100));
                        column.Format.Alignment = ParagraphAlignment.Right;

                        var header = table.AddRow();
                        header.Cells[0].AddParagraph("#");
                        header.Cells[1].AddParagraph("CARRIER ROUTE #");
                        header.Cells[2].AddParagraph("TOTAL H/H");
                        header.Cells[3].AddParagraph("TARGET H/H");
                        header.Cells[4].AddParagraph("PENETRATION");

                        var index = 1;
                        foreach (var item in record)
                        {
                            var row = table.AddRow();
                            row.Cells[0].AddParagraph(index.ToString());
                            row.Cells[1].AddParagraph(item.Name);
                            row.Cells[2].AddParagraph((item.TotalHouseHold ?? 0).ToString("#,###"));
                            row.Cells[3].AddParagraph((item.TargetHouseHold ?? 0).ToString("#,###"));
                            row.Cells[4].AddParagraph(item.Penetration.ToString("p2"));
                            index++;
                        }
                    }
                    break;
            }
            for(var i = 0; i < table.Columns.Count; i++)
            {
                table.Columns[i].Borders.Bottom.Color = tableBorderColor;
            }
            table.Columns[0].Borders.Left.Color = tableBorderColor;
            table.Columns[table.Columns.Count - 1].Borders.Right.Color = tableBorderColor;
            //for first page row count = page height - footer - section title - table title
            //other page do not have section title
            //int firtPageMaxRowCount = (int)Math.Floor((pageSize.PageHeight.Value - Unit.FromPoint(25).Value - Unit.FromPoint(43).Value) / table.Rows.Height.Value) - 2;
            //int otherPageMaxRowCount = (int)Math.Floor((pageSize.PageHeight.Value - Unit.FromPoint(43).Value) / table.Rows.Height.Value) - 2;
            //for quick duty fix
            int firtPageMaxRowCount = 46;
            int otherPageMaxRowCount = 47;
            int maxRowCount = firtPageMaxRowCount;
            int rowNo = 1;
            for(var i = 1; i < table.Rows.Count; i++)
            {
                if (rowNo % 2 == 0)
                {
                    table.Rows[i].Shading.Color = rowBgColor;
                }
                if(rowNo >= maxRowCount)
                {
                    rowNo = 1;
                    maxRowCount = otherPageMaxRowCount;
                }
                else
                {
                    rowNo++;
                }
            }
            table.Rows[0].HeadingFormat = true;
            table.Rows[0].Format.Font.Bold = true;
            table.Rows[0].Shading.Color = rowBgColor;
            for (var i = 0; i < table.Rows[0].Cells.Count; i++)
            {
                table.Rows[0].Cells[i].Borders.Top.Color = tableBorderColor;
            }
        }

        private string GetImage(string imageSrc)
        {
            if (string.IsNullOrWhiteSpace(imageSrc))
            {
                return null;
            }
            string imgPath = null;
            if (imageSrc.StartsWith("http"))
            {
                var imgFile = string.Format("{0}.png", Guid.NewGuid().ToString());
                imgPath = System.IO.Path.Combine(TempFolder, imgFile);
                using (WebClient client = new WebClient())
                {
                    try {
                        client.DownloadFile(new Uri(imageSrc), imgPath);
                    }catch(Exception ex)
                    {
                        logger.ErrorFormat("download image for pdf export failed.\r\n{0}", ex.ToString());
                        return null;
                    }
                }
            }
            else
            {
                var index = imageSrc.LastIndexOf('/');
                imgPath = System.IO.Path.Combine(ImageFolder, imageSrc.Substring(index + 1));
                var fileExists = System.IO.File.Exists(imgPath);
                if (!fileExists)
                {
                    return null;
                }
            }
            return imgPath;
        }
        private string MergeMap(string map, string bg, float width, float height)
        {
            width = width * 96;
            height = height * 96;

            if(string.IsNullOrWhiteSpace(map) || string.IsNullOrWhiteSpace(bg))
            {
                return null;
            }
            var mapFile = string.Format("{0}.png", Guid.NewGuid().ToString());
            var mapPath = System.IO.Path.Combine(TempFolder, mapFile);
            var bgFile = string.Format("{0}.jpg", Guid.NewGuid().ToString());
            var bgPath = System.IO.Path.Combine(TempFolder, bgFile);

            using (WebClient client = new WebClient())
            {
                try {
                    client.DownloadFile(new Uri(map), mapPath);
                    client.DownloadFile(new Uri(bg), bgPath);
                }catch(Exception ex)
                {
                    logger.ErrorFormat("download image for pdf export failed.\r\n{0}", ex.ToString());
                    return null;
                }
            }

            var comboImgFile = string.Format("{0}.jpg", Guid.NewGuid().ToString());
            var comboImgPath = System.IO.Path.Combine(TempFolder, comboImgFile);

            using (System.Drawing.Bitmap bgBitmap = new System.Drawing.Bitmap(bgPath))
            using (System.Drawing.Bitmap mapBitmap = new System.Drawing.Bitmap(mapPath))
            {
                //cut google logo
                var cutRate = (bgBitmap.Height - 30f) / (float)bgBitmap.Height;
                var imgWidth = bgBitmap.Width * cutRate;
                var imgHeight = bgBitmap.Height * cutRate;
                //zoom to target width and height
                var widthRate = imgWidth / width;
                var heightRate = imgHeight / height;
                var scaleRate = Math.Min(widthRate, heightRate);

                var scaleWidth = width * scaleRate;
                var scaleHeight = height * scaleRate;

                float top = (bgBitmap.Height - scaleHeight) * -0.5F;
                float left = (bgBitmap.Width - scaleWidth) * -0.5F;

                using (System.Drawing.Bitmap comboBitmap = new System.Drawing.Bitmap((int)scaleWidth, (int)scaleHeight))
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(comboBitmap))
                {
                    var rect = new System.Drawing.Rectangle((int)left, (int)top, bgBitmap.Width, bgBitmap.Height);
                    g.DrawImageUnscaledAndClipped(bgBitmap, rect);
                    g.DrawImageUnscaledAndClipped(mapBitmap, rect);
                    comboBitmap.Save(comboImgPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
            return comboImgPath;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
