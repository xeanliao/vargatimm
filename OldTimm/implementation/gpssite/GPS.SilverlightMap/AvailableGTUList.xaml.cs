using System;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using Microsoft.Maps.MapControl;
using System.Windows.Threading;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Browser;
using GPS.SilverlightMap.GTUService;

namespace GPS.SilverlightMap
{
    public partial class AvailableGTUList : UserControl
    {
        const int INTERVAL_TIME = 5;
        bool resetCenter = true;
        private DispatcherTimer gtuMonitorTimer;
        string currentMonitoredGTUID = "";
        string lastMonitoredGTUID = "";
        GTUService.GTUQueryServiceClient gtuClient;
        Location lcLastPosition = new Location();
        public AvailableGTUList()
        {
            InitializeComponent();

            Map.Mode = new RoadMode();
            Map.ZoomLevel = 2;

            gtuClient = new GTUService.GTUQueryServiceClient();
            gtuClient.GetGTUCodeListCompleted += new EventHandler<GetGTUCodeListCompletedEventArgs>(gtuClient_GetGTUCodeListCompleted);
            gtuClient.GetGTUCompleted += new EventHandler<GetGTUCompletedEventArgs>(gtuClient_GetGTUCompleted);

            gtuClient.GetGTUCodeListAsync();

            gtuMonitorTimer = new DispatcherTimer();
            gtuMonitorTimer.Interval = TimeSpan.FromSeconds(INTERVAL_TIME);
            gtuMonitorTimer.Tick += new EventHandler(GTUMonitorLoop);
            gtuMonitorTimer.Start();
            rdGTUMonitorMode.IsChecked = true; //default is monitor mode
        }

        void gtuClient_GetGTUCompleted(object sender, GetGTUCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // Out Put error messagestring s = e.Error.Message;
            }
            else
            {
                if (e.Result == null)
                    return;
                double dwLongitude = e.Result.CurrentCoordinate.Longitude;
                double dwLatitude = e.Result.CurrentCoordinate.Latitude;
                Location lcCurrent = new Location(dwLatitude, dwLongitude);
                if (dwLongitude == 0 || dwLatitude == 0)
                    return;
                if (resetCenter)
                {
                    Map.Center = new Location(dwLatitude, dwLongitude);
                    Map.ZoomLevel = 13;
                }
                //Only update map if location got changed
                if (lcCurrent != lcLastPosition)
                {
                    lcLastPosition = lcCurrent;
                    Pushpin pb = new Pushpin();
                    pb.Location = lcCurrent;
                    if (rdGTUTrackMode.IsChecked == true)
                    {
                        pb.Template = (ControlTemplate)Application.Current.Resources["PushPinTemplate"];
                    }
                    else
                    {
                        Map.Children.Clear();
                    }

                    Map.Children.Add(pb);
                }
                resetCenter = false;
            }
        }

        void gtuClient_GetGTUCodeListCompleted(object sender, GetGTUCodeListCompletedEventArgs e)
        {
            lstGTU.Items.Clear();
            if (e.Error != null)
            {
                // Out Put error messagestring s = e.Error.Message;
            }
            else
            {
                if (e.Result == null)
                    return;
                for (int i = 0; i < e.Result.Count; i++)
                {
                    string NumId = Convert.ToString(i + 1) + '.' + ' ';
                    //lstGTU.Items.Add(new{ NumId = Convert.ToString(i + 1) + '.' + ' ', DisplayGTUId = DisplayGTUId, GTUId = e.Result[i] + '@' });
                    lstGTU.Items.Add(new DisplayedItem(e.Result[i], NumId));
                }

            }
        }

        private void lstGTU_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            resetCenter = true;
            if ((sender as ListBox).SelectedItem != null)
            {
                currentMonitoredGTUID = ((sender as ListBox).SelectedItem as DisplayedItem).GTUId.ToString();
            }

            if (lastMonitoredGTUID != currentMonitoredGTUID)
            {
                //If new GTU selected, need clear the map,and update with the new selected one
                Map.Children.Clear();
                gtuClient.GetGTUAsync(currentMonitoredGTUID);
                lastMonitoredGTUID = currentMonitoredGTUID;
            }
        }

        void GTUMonitorLoop(object sender, EventArgs e)
        {
            //gtuClient.GetGTUCodeListAsync();

            if (currentMonitoredGTUID.Length > 0)
            {
                gtuClient.GetGTUAsync(currentMonitoredGTUID);
            }
        }

        private void rdGTUMonitorMode_Checked(object sender, RoutedEventArgs e)
        {
            //If change mode, clear the map, and make the last position to emtpy, so the map will be redraw
            Map.Children.Clear();
            lcLastPosition = new Location();
            gtuClient.GetGTUAsync(currentMonitoredGTUID);
        }

        private void BtGTURefresh_Click(object sender, RoutedEventArgs e)
        {
            gtuClient.GetGTUCodeListAsync();

        }

    }

    //listbox item display style
    //public class DisplayedItem
    //{
    //    public DisplayedItem(string gid, string col)
    //    {
    //        GTUId = gid;
    //        Col = col;
    //        if (gid.Length > 5)
    //            DisplayGTUId = gid.Substring(gid.Length - 5, 5);
    //        else
    //            DisplayGTUId = gid;
    //    }
    //    public string GTUId { get; set; }
    //    public string DisplayGTUId { get; set; }
    //    public string Col { get; set; }
    //}
}
