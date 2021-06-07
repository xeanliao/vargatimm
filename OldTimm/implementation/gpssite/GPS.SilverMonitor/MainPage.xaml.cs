using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using System.ServiceModel.DomainServices.Client;
using Microsoft.Maps.MapControl;
using System.Windows.Browser;

namespace GPS.SilverMonitor
{

    public partial class MainPage : UserControl
    {
        /*
        private int mTaskID = 0;

        public MainPage()
        {
            InitializeComponent();

            AddDMap();
            ShowAddressList();

            // GetAuditors
            //GPS.Website.DAL.TimmDomainContext timm = new Website.DAL.TimmDomainContext();
            //timm.Load(timm.GetTaskAuditorQuery(27), loadTask_Completed, null);
            //timm.Load(timm.GetDistributionMapsAQuery(27), loadDistributionMaps_Completed, null);
        }

        private void loadTask_Completed(LoadOperation op)
        {

            foreach (GPS.Website.DAL.user t in op.Entities)
            {
                txtAuditorName.Text  = t.UserName;
            }

        }

        
        //private void loadDistributionMaps_Completed(LoadOperation op)
        //{
        //    int iDMapID = 0;
        //    foreach (GPS.Website.DAL.vewDistributionMap map in op.Entities)
        //    {
        //        dmapName1.Text = map.Name;
        //        iDMapID = map.Id;
        //    }

        //    GPS.Website.DAL.TimmDomainContext timm = new Website.DAL.TimmDomainContext();
        //    timm.Load(timm.GetDistributionCoordinatesQuery(iDMapID), loadDMapCoordinates_completed, null);
        //}
        

        private void loadDMapCoordinates_completed(LoadOperation op)
        {
            int iTotal = op.Entities.Count();

            MapPolygon polygon = new MapPolygon();
            polygon.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
            polygon.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Yellow);
            polygon.StrokeThickness = 5;
            polygon.Opacity = 0.7;

            // get the center
            double dY = 0;
            double dX = 0;
            foreach (GPS.Website.DAL.distributionmapcoordinate p in op.Entities)
            {
                dY += p.Latitude;
                dX += p.Longitude;
            }
            dY = dY / iTotal;
            dX = dX / iTotal;
            map1.ZoomLevel = 13;
            map1.Center = new Location(dY, dX);

            // Draw DMap
            LocationCollection locations = new LocationCollection();
            foreach (GPS.Website.DAL.distributionmapcoordinate p in op.Entities)
            {
                locations.Add(new Location(p.Latitude, p.Longitude));
            }
            polygon.Locations = locations;
            DMLayer.Children.Add(polygon);
        
        }

        private void ShowAddressList()
        {
            //listAddress.ItemsSource = TaskAddress.GetSamples();
        }

        
        //void timm_sayHelloCompleted(object sender, TimmUserServiceReference.sayHelloCompletedEventArgs e)
        //{
        //    string ret = (string)e.Result;
        //    MessageBox.Show(ret);
        //}
        

        private void DrawDMapPolygon()
        { 
        
        }

        private void AddDMap()
        { 

            MapPolygon polygon = new MapPolygon();
            polygon.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
            polygon.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Yellow);
            polygon.StrokeThickness = 5;
            polygon.Opacity = 0.7;

            // get the data
            DMapPoint[] dmap = DMapPoint.GetSamples();

            // get the center
            double dY = 0;
            double dX = 0;
            foreach (DMapPoint p in dmap)
            {
                dY += p.Latitude;
                dX += p.Longitude;
            }
            dY = dY / dmap.Length;
            dX = dX / dmap.Length;
            map1.ZoomLevel = 14;
            map1.Center = new Location(dY, dX);

            // Draw DMap
            LocationCollection locations = new LocationCollection();
            foreach(DMapPoint p in dmap)
            {
                locations.Add(new Location(p.Latitude, p.Longitude));
            }
            polygon.Locations = locations;
            DMLayer.Children.Add(polygon);

        }

        private void AddGtuHistory()
        {
            GtuLocation[] gtuLocations = GtuLocation.GetGtuLocationsSample();
            foreach(GtuLocation gtu in gtuLocations)
            {
                ShowGtu(gtu.Latitude, gtu.Longitude);
            }
        }

        private void ShowGtu(double latitude, double longitude)
        {
            Image image = new Image();
            //Define the URI location of the image
            image.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("bullet_blue.png", UriKind.Relative));
            //Define the image display properties
            image.Opacity = 0.8;
            image.Stretch = System.Windows.Media.Stretch.None;
            //The map location to place the image at
            Location location = new Location(latitude, longitude);

            //Center the image around the location specified
            PositionOrigin position = PositionOrigin.Center;

            //Add the image layer to the map
            GtuLayer.AddChild(image, location, position);
        }

        private void LoadGtuList()
        {
            List<GtuStatus> gtus1 = new List<GtuStatus>();
            gtus1.Add(new GtuStatus() { statusImage = "", GtuNumber = "12345", GtuCarrier = "Jimmy" });
            gtus1.Add(new GtuStatus() { statusImage = "", GtuNumber = "67890", GtuCarrier = "Justin" });
            dmap1GtuList.ItemsSource = gtus1;

            List<GtuStatus> gtus2 = new List<GtuStatus>();
            gtus2.Add(new GtuStatus() { statusImage = "", GtuNumber = "13579", GtuCarrier = "David" });
            gtus2.Add(new GtuStatus() { statusImage = "", GtuNumber = "24680", GtuCarrier = "Dog" });
            dmap2GtuList.ItemsSource = gtus2;

            List<GtuStatus> gtus3 = new List<GtuStatus>();
            gtus3.Add(new GtuStatus() { statusImage = "", GtuNumber = "36963", GtuCarrier = "Allen" });
            gtus3.Add(new GtuStatus() { statusImage = "", GtuNumber = "47170", GtuCarrier = "Allen" });
            unassignedGtuList.ItemsSource = gtus3;

            //WebApplication2.TimmDomainContext _context = new WebApplication2.TimmDomainContext();
            //_context.Load(_context.GetGtusQuery(), GtuLoadedCallback, null);

        }

        //void GtuLoadedCallback(LoadOperation<WebApplication2.gtu> loadOperation)
        //{
        //    foreach (WebApplication2.gtu g in loadOperation.Entities)
        //    {
        //        MessageBox.Show(g.UniqueID);
        //    }
        //}

        private void ShowAddress()
        {
            Image image = new Image();
            //Define the URI location of the image
            image.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("star.png", UriKind.Relative));
            //Define the image display properties
            image.Opacity = 0.8;
            image.Stretch = System.Windows.Media.Stretch.None;
            //The map location to place the image at
            Location location = new Location(34.17087963, -118.831062316895);
            //Center the image around the location specified
            PositionOrigin position = PositionOrigin.Center;

            //Add the image to the defined map layer
            //this.DMLayer.AddChild(image, location, position);

            Pushpin pin = new Pushpin();
            pin.Location = location;
            pin.Name = "716 Joaquin rd, CA";
            ToolTipService.SetToolTip(pin, "716 Joaquin rd\r\nArcadia, CA 91007");
            // ContextMenuService.SetContextMenu(pin, null);
            

            //ScaleTransform st = new ScaleTransform();
            //st.ScaleX = 0.25;
            //st.ScaleY = 0.25;
            //st.CenterX = (pin as FrameworkElement).Height / 2;
            //st.CenterY = (pin as FrameworkElement).Height / 2;
            //pin.RenderTransform = st;
            //pin.Background = new SolidColorBrush(Colors.Blue);

            this.DMLayer.AddChild(pin, location);
        }

        private void dm1Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (dmap1GtuList.Visibility == Visibility.Visible)
            {
                dmap1GtuList.Visibility = Visibility.Collapsed;
                dm1ImageRight.Visibility = Visibility.Collapsed;
                dm1ImageDown.Visibility = Visibility.Visible;
            }
            else
            {
                dmap1GtuList.Visibility = Visibility.Visible;
                dm1ImageRight.Visibility = Visibility.Visible;
                dm1ImageDown.Visibility = Visibility.Collapsed;
            }
        }

        private void md2Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (dmap2GtuList.Visibility == Visibility.Visible)
            {
                dmap2GtuList.Visibility = Visibility.Collapsed;
                dm2ImageRight.Visibility = Visibility.Collapsed;
                dm2ImageDown.Visibility = Visibility.Visible;
            }
            else
            {
                dmap2GtuList.Visibility = Visibility.Visible;
                dm2ImageRight.Visibility = Visibility.Visible;
                dm2ImageDown.Visibility = Visibility.Collapsed;
            }
        }

        private void imgUnassigned_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (unassignedGtuList.Visibility == Visibility.Visible)
                unassignedGtuList.Visibility = Visibility.Collapsed;
            else
                unassignedGtuList.Visibility = Visibility.Visible;
        }

        private void mnuShowAll_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mnu = (MenuItem)sender;
            string gtuNumber = mnu.CommandParameter.ToString();

            AddGtuHistory();
        }

        private void mnuShowGtu_Click(object sender, RoutedEventArgs e)
        {
            ShowGtu(34.14287963, -118.831062316895);
        }

        private void mnuShowAllHistory_Click(object sender, RoutedEventArgs e)
        {
            AddGtuHistory();
        }

        private void mnuShowGtuHistory_Click(object sender, RoutedEventArgs e)
        {
            AddGtuHistory();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mnu = (MenuItem)sender;
            MessageBox.Show(mnu.CommandParameter.ToString());
        }

        private void UploadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Filter = "All files (*.*)|*.*|PNG Images (*.png)|*.png";

            bool? retval = dlg.ShowDialog();

            if (retval != null && retval == true)
            {
                //UploadFile(dlg.File.Name, dlg.File.OpenRead());
                //StatusText.Text = dlg.File.Name;
            }
            else
            {
                //StatusText.Text = "No file selected...";
            }
        }

        private void btnAddAddress_Click(object sender, RoutedEventArgs e)
        {
            // open an ASP.NET webform to add the address
            //HtmlPopupWindowOptions options = new HtmlPopupWindowOptions();
            //options.Width = 600;
            //options.Height = 400;
            //HtmlPage.PopupWindow(new Uri("http://www.yahoo.com"), "winTaskAddress", options);

            HtmlPage.Window.Eval("window.open('TaskAddressUI.aspx?did=" + mTaskID + "', '_blank', 'height=400px,width=400px,resizable=yes,status=no,toolbar=no,menubar=no,location=no')");

        }
        */
                        
    }

    public class GtuStatus
    {
        public string statusImage { get; set; }
        public string GtuNumber { get; set; }
        public string GtuCarrier { get; set; }
    }

}
