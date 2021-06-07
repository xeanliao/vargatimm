using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using TIMM.GPS.ControlCenter.Model;
using TIMM.GPS.ControlCenter.UserControls.Print;

namespace TIMM.GPS.ControlCenter.UserControls
{
    public partial class PrintSubMapSummary : PdfPageBase
    {
        List<SubMapUI> SubMaps { get; set; }

        public PrintSubMapSummary(CampaignUI campaign, List<SubMapUI> submaps)
        {
            InitializeComponent();

            PrintFooter footer = new PrintFooter(campaign);
            footer.VerticalAlignment = VerticalAlignment.Bottom;
            this.LayoutRoot.Children.Add(footer);

            SubMaps = submaps;

            this.Loaded +=  Page_Loaded;
        }

        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= Page_Loaded;
            int index = 0;
            SolidColorBrush alternationColor = new SolidColorBrush(Color.FromArgb(0xFF, 0xEE, 0xEE, 0xEE));
            SolidColorBrush fontColor = new SolidColorBrush(Colors.Black);
            FontFamily fontFamily = new FontFamily("Arial");
            double fontSize = 11d;
            SubMaps.ForEach(i =>
            {
                SubMapUI submap = (SubMapUI)i;
                Grid row = new Grid() 
                { 
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top
                };
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60d, GridUnitType.Pixel) });
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(232d, GridUnitType.Pixel) });
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100d, GridUnitType.Pixel) });
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100d, GridUnitType.Pixel) });
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100d, GridUnitType.Pixel) });
                row.RowDefinitions.Add(new RowDefinition { Height = new GridLength(16d, GridUnitType.Pixel) });

                if (index % 2 == 1)
                {
                    row.Background = alternationColor;
                }

                TextBlock serial = new TextBlock
                {
                    Text = submap.OrderId.ToString(),
                    Foreground = fontColor,
                    FontSize = fontSize,
                    FontFamily = fontFamily,
                    Margin = new Thickness(5, 0, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center
                };
                serial.SetValue(Grid.ColumnProperty, 0);
                row.Children.Add(serial);

                TextBlock name = new TextBlock
                {
                    Text = submap.Name,
                    Foreground = fontColor,
                    FontSize = fontSize,
                    FontFamily = fontFamily,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center
                };
                name.SetValue(Grid.ColumnProperty, 1);
                row.Children.Add(name);

                TextBlock totalHouseHolde = new TextBlock
                {
                    Text = submap.TotalHouseHold.ToString("#,###"),
                    Foreground = fontColor,
                    FontSize = fontSize,
                    FontFamily = fontFamily,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center
                };
                totalHouseHolde.SetValue(Grid.ColumnProperty, 2);
                row.Children.Add(totalHouseHolde);

                TextBlock targetHouseHolde = new TextBlock
                {
                    Text = submap.TargetHouseHold.ToString("#,###"),
                    Foreground = fontColor,
                    FontSize = fontSize,
                    FontFamily = fontFamily,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center
                };
                targetHouseHolde.SetValue(Grid.ColumnProperty, 3);
                row.Children.Add(targetHouseHolde);

                TextBlock penetration = new TextBlock
                {
                    Text = submap.DisplayPenetration.ToString("p2"),
                    Foreground = fontColor,
                    FontSize = fontSize,
                    FontFamily = fontFamily,
                    Margin = new Thickness(0, 0, 5, 0),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center
                };
                penetration.SetValue(Grid.ColumnProperty, 4);
                
                row.Children.Add(penetration);

                gridSummary.Children.Add(row);

                index++;
            });

            gridSummary.UpdateLayout();
        }

        private static double CalculateWidth(string content)
        {
            double width = 0;
            //number width 6
            width += content.Count(i => i == ',') * 3d;
            width += content.Count(i => i == '%') * 10d;
            width += content.Replace(",", "").Replace("%", "").Length * 6d;
            return width;
        }
    }
}
