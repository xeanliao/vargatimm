using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using TIMM.GPS.ControlCenter.Model;
using TIMM.GPS.Model;
using TIMM.GPS.ControlCenter.UserControls.Print;

namespace TIMM.GPS.ControlCenter.UserControls
{
    public partial class PrintSubMapDetail : PdfPageBase
    {
        public PrintSubMapDetail()
        {
            InitializeComponent();
        }

        List<string[]> Rows = new List<string[]>();

        public bool IsContinue { get; set; }

        public PrintSubMapDetail(CampaignUI campaign)
        {
            InitializeComponent();

            PrintFooter footer = new PrintFooter(campaign);
            footer.VerticalAlignment = VerticalAlignment.Bottom;
            this.LayoutRoot.Children.Add(footer);


            this.Loaded +=  Page_Loaded;
        }

        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= Page_Loaded;

            //GenerateFiveZips(FiveZipArea row);
            //GenerateCRoutes(PremiumCRoute row);
            //GenerateTracts(Tract row);
            //GenerateBlockGroups(BlockGroup row);

            gridSummary.UpdateLayout();
        }

        #region Private Method
        private void GenerateTitles(string[] titles, double[] columns, HorizontalAlignment[] alignment)
        {
            SolidColorBrush alternationColor = new SolidColorBrush(Color.FromArgb(0xFF, 0xEE, 0xEE, 0xEE));
            SolidColorBrush fontColor = new SolidColorBrush(Colors.Black);
            FontFamily fontFamily = new FontFamily("Arial");
            double fontSize = 11d;

            Grid row = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 15,
                Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xEE, 0xEE, 0xEE))
            };
            foreach (var col in columns)
            {
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(col, GridUnitType.Pixel) });
            }

            for (int i = 0; i < titles.Length; i++)
            {
                TextBlock tb = new TextBlock
                {
                    Text = titles[i],
                    Foreground = fontColor,
                    FontSize = fontSize,
                    FontFamily = fontFamily,
                    HorizontalAlignment = alignment[i],
                    VerticalAlignment = VerticalAlignment.Center
                };
                if (i % titles.Length == 0)
                {
                    tb.Margin = new Thickness(5, 0, 0, 0);
                }
                else if ((i + 1) % titles.Length == 0)
                {
                    tb.Margin = new Thickness(0, 0, 5, 0);
                }
                tb.SetValue(Grid.ColumnProperty, i);
                row.Children.Add(tb);
            }
            gridSummary.Children.Add(row);
        }

        private void GenerateRows(string[] values, HorizontalAlignment[] alignment, double[] columns, int index)
        {
            SolidColorBrush alternationColor = new SolidColorBrush(Color.FromArgb(0xFF, 0xEE, 0xEE, 0xEE));
            SolidColorBrush fontColor = new SolidColorBrush(Colors.Black);
            FontFamily fontFamily = new FontFamily("Arial");
            double fontSize = 11d;


            Grid row = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 15d
            };

            foreach (var col in columns)
            {
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(col, GridUnitType.Pixel) });
            }

            if (index % 2 == 1)
            {
                row.Background = alternationColor;
            }

            for (int j = 0; j < values.Length; j++)
            {
                TextBlock tb = new TextBlock
                {
                    Text = values[j],
                    Foreground = fontColor,
                    FontSize = fontSize,
                    FontFamily = fontFamily,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = alignment[j]
                };
                if (j % values.Length == 0)
                {
                    tb.Margin = new Thickness(5, 0, 0, 0);
                }
                else if ((j + 1) % values.Length == 0)
                {
                    tb.Margin = new Thickness(0, 0, 5, 0);
                }

                tb.SetValue(Grid.ColumnProperty, j);
                row.Children.Add(tb);
            }
            gridSummary.Children.Add(row);


        }
        #endregion

        #region Preview

        public void GenerateCaption(string text, bool needMargen = false)
        {
            Border caption = new Border
            {
                Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x94, 0xB4, 0x3D)),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Height = 30d,
                Child = new TextBlock
                {
                    Text = text,
                    FontSize = 14d,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Colors.Black),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                }
            };
            if (needMargen)
            {
                caption.Margin = new Thickness(0d, 15d, 0d, 0d);
                Rows.Add(null);
            }
            gridSummary.Children.Add(caption);
            Rows.Add(new string[] { text });
        }

        public void GenerateFiveZipsTitle()
        {
            string[] titles = new string[] { "#", "ZIP CODE", "TOTAL H/H", "TARGET H/H", "PENETRATION" };
            double[] columns = new double[] { 60d, 232d, 100d, 100d, 100d };
            HorizontalAlignment[] alignment = new HorizontalAlignment[] 
            {
                HorizontalAlignment.Left,
                HorizontalAlignment.Left,
                HorizontalAlignment.Right,
                HorizontalAlignment.Right,
                HorizontalAlignment.Right
            };
            GenerateTitles(titles, columns, alignment);
            Rows.Add(titles);
        }

        public void GenerateCRoutesTitle()
        {
            string[] titles = new string[] { "#", "CARRIER ROUTE #", "TOTAL H/H", "TARGET H/H", "PENETRATION" };
            double[] columns = new double[] { 60d, 232d, 100d, 100d, 100d };
            HorizontalAlignment[] alignment = new HorizontalAlignment[] 
            {
                HorizontalAlignment.Left,
                HorizontalAlignment.Left,
                HorizontalAlignment.Right,
                HorizontalAlignment.Right,
                HorizontalAlignment.Right
            };

            GenerateTitles(titles, columns, alignment);
            Rows.Add(titles);
        }

        public void GenerateTractsTitle()
        {
            string[] titles = new string[] { "#", "CENSUS TRACT #", "TOTAL H/H", "TARGET H/H", "PENETRATION" };
            double[] columns = new double[] { 60d, 232d, 100d, 100d, 100d };
            HorizontalAlignment[] alignment = new HorizontalAlignment[] 
            {
                HorizontalAlignment.Left,
                HorizontalAlignment.Left,
                HorizontalAlignment.Right,
                HorizontalAlignment.Right,
                HorizontalAlignment.Right
            };
            GenerateTitles(titles, columns, alignment);
            Rows.Add(titles);
        }

        public void GenerateBlockGroupsTitle()
        {
            string[] titles = new string[] { "#", "BLOCK GROUP #", "TOTAL H/H", "TARGET H/H", "PENETRATION" };
            double[] columns = new double[] { 60d, 232d, 100d, 100d, 100d };
            HorizontalAlignment[] alignment = new HorizontalAlignment[] 
            {
                HorizontalAlignment.Left,
                HorizontalAlignment.Left,
                HorizontalAlignment.Right,
                HorizontalAlignment.Right,
                HorizontalAlignment.Right
            };
            GenerateTitles(titles, columns, alignment);
            Rows.Add(titles);
        }

        public void GenerateRow(SubMapDetailItem row, int index)
        {
            double[] columns = new double[] { 60d, 232d, 100d, 100d, 100d };
            HorizontalAlignment[] alignment = new HorizontalAlignment[] 
            {
                HorizontalAlignment.Left,
                HorizontalAlignment.Left,
                HorizontalAlignment.Right,
                HorizontalAlignment.Right,
                HorizontalAlignment.Right
            };
            string[] values = new string[] 
                {
                    row.OrderId.ToString(),
                    row.Name,
                    (row.TotalHouseHold ?? 0).ToString("#,##0"),
                    (row.TargetHouseHold ?? 0).ToString("#,##0"),
                    row.DisplayPenetration.ToString("p2")
                };
            GenerateRows(values, alignment, columns, index);
            Rows.Add(values);
        }
        #endregion

        private static double CalculateWidth(string content)
        {
            if (string.Compare("TOTAL H/H", content) == 0)
            {
                return 58d;
            }
            if (string.Compare("TARGET H/H", content) == 0)
            {
                return 67d;
            }
            if (string.Compare("PENETRATION", content) == 0)
            {
                return 77d;
            }
            double width = 0;
            //number width 6
            width += content.Count(i => i == ',') * 3d;
            width += content.Count(i => i == '%') * 10d;
            width += content.Replace(",", "").Replace("%", "").Length * 6d;
            return width;
        }
    }
}
