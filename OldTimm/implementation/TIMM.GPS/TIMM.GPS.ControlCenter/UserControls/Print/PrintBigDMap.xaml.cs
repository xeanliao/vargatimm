using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Map = Microsoft.Maps.MapControl;

using TIMM.GPS.ControlCenter.Model;
using TIMM.GPS.ControlCenter.Interface;


namespace TIMM.GPS.ControlCenter.UserControls.Print
{
    public partial class PrintBigDMap : PdfPageBase, ICanEditMapPdfPage
    {
        public DistributionMapUI DMap { get; set; }
        private string m_ImageCachePath;

        public void SetImagePath(string path)
        {
            //m_ImageCachePath = path;
        }

        public PrintBigDMap(DistributionMapUI dmap, ImageSource source)
        {
            InitializeComponent();
            DMap = dmap;
            this.DataContext = DMap;
            SubMapImage.Source = source;
            txtMapName.Text = dmap.PringBigDMapMapTitle;
            if (dmap.NdAddress != null && dmap.NdAddress.Count > 0)
            {
                borderNdAddress.Visibility = System.Windows.Visibility.Visible;
                listNdAddress.ItemsSource = dmap.NdAddress;
            }
            else
            {
                borderNdAddress.Visibility = System.Windows.Visibility.Collapsed;
            }
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
