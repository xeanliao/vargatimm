using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

using Microsoft.Maps.MapControl;

using TIMM.GPS.ControlCenter.Model;
using TIMM.GPS.ControlCenter.Interface;
using TIMM.GPS.ControlCenter.UserControls.Print;



namespace TIMM.GPS.ControlCenter.UserControls
{
    public partial class PrintCampaign : PdfPageBase, ICanEditMapPdfPage, INeedSetTargetMethod
    {

        public CampaignUI Campaign { get; set; }
        private List<KeyValuePair<Color, string>> m_ColorLegend = new List<KeyValuePair<Color, string>>();

        public PrintCampaign(CampaignUI campaign, ImageSource source)
        {
            InitializeComponent();
            PrintFooter footer = new PrintFooter(campaign);
            footer.VerticalAlignment = VerticalAlignment.Bottom;
            this.LayoutRoot.Children.Add(footer);
            this.Campaign = campaign;
            txtTargetMethod.Text = campaign.TargetMethod;
            this.DataContext = Campaign;
            CampaignImage.Source = source;
        }

        private double m_ZoomLevel;
        private Location m_Center;

        public Location GetCenter()
        {
            return m_Center;
        }

        public double GetZoomLevel()
        {
            return m_ZoomLevel;
        }

        public void SetCenter(Location newCenter)
        {
            m_Center = newCenter;
        }

        public void SetZoomLevel(double newZoomLevel)
        {
            m_ZoomLevel = newZoomLevel;
        }

        public int GetId()
        {
            return 0;
        }

        public void ChangeImgae(ImageSource source)
        {
            CampaignImage.Source = source;
            this.UpdateLayout();
        }

        public void SetTargetMethod(string content)
        {
            Campaign.TargetMethod = content;
            txtTargetMethod.Text = content;
        }

        public void SetPercentageColor(List<CampaignPercentageColorUI> color)
        {
            colorLegend.Children.Clear();
            m_ColorLegend.Clear();
            txtColorLegend.Visibility = Visibility.Collapsed;

            if (color != null && color.Count > 0)
            {
                color.Sort((i, j) => { return i.ColorId - j.ColorId; });
                
                foreach(var item in color)
                {
                    if (item.Min >= 0 && item.Max >= 0)
                    {
                        Rectangle box = new Rectangle
                        {
                            Width = 16,
                            Height = 9,
                            Stroke = new SolidColorBrush(Colors.Black),
                            StrokeThickness = 1d,
                            Fill = item.FillColor,
                            Margin = new Thickness(0, 0, 5, 0),
                            VerticalAlignment = VerticalAlignment.Center
                        };

                        TextBlock label = new TextBlock
                        {
                            Text = string.Format("{0}({1}%-{2}%)", item.DisplayName, (item.Min * 100d).ToString("#0"), (item.Max * 100d).ToString("#0")),
                            FontSize = 8d,
                            FontFamily = new FontFamily("Arial"),
                            Foreground = new SolidColorBrush(Colors.Black),
                            Width = 84,
                            Margin = new Thickness(0, 1, 0, 0)
                        };
                        colorLegend.Children.Add(box);
                        colorLegend.Children.Add(label);

                        m_ColorLegend.Add(new KeyValuePair<Color, string>(item.FillColor.Color, label.Text));
                        txtColorLegend.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        public ExportPageTypeEnum GetPageType()
        {
            return ExportPageTypeEnum.Campaign;
        }
    }
}
