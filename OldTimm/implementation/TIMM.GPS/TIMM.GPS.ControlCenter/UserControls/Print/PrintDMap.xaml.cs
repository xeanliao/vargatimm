using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Map = Microsoft.Maps.MapControl;

using TIMM.GPS.ControlCenter.Model;
using TIMM.GPS.ControlCenter.Interface;


namespace TIMM.GPS.ControlCenter.UserControls.Print
{
    public partial class PrintDMap : PdfPageBase, ICanEditMapPdfPage
    {

        public DistributionMapUI DMap { get; set; }

        public CampaignUI Campaign { get; set; }

        public PrintDMap(CampaignUI campaign, DistributionMapUI dMap, ImageSource source)
        {
            InitializeComponent();
            Campaign = campaign;
            PrintFooter footer = new PrintFooter(Campaign);
            footer.VerticalAlignment = VerticalAlignment.Bottom;
            LayoutRoot.Children.Add(footer);
            txtMapName.Text = string.Format("{0} - {1}", campaign.DisplayName, dMap.Name);
            DMap = dMap;
            DataContext = this.DMap;
            SubMapImage.Source = source;
        }

        private double m_ZoomLevel;
        private Map.Location m_Center;

        public Map.Location GetCenter()
        {
            return m_Center;
        }

        public double GetZoomLevel()
        {
            return m_ZoomLevel;
        }

        public void SetCenter(Map.Location newCenter)
        {
            m_Center = newCenter;
        }

        public void SetZoomLevel(double newZoomLevel)
        {
            m_ZoomLevel = newZoomLevel;
        }

        public int GetId()
        {
            return DMap.Id;
        }

        public void ChangeImgae(ImageSource source)
        {
            SubMapImage.Source = source;
            this.UpdateLayout();
        }

        public void SetPercentageColor(List<CampaignPercentageColorUI> color)
        {
            
        }

        public ExportPageTypeEnum GetPageType()
        {
            return ExportPageTypeEnum.DMap;
        }
    }
}
