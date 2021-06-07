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
using Microsoft.Maps.MapControl;
using System.Windows.Browser;
using GPS.SilverlightMap.DMReaderSLService;
using GPS.SilverlightMap.GTUService;
using System.Windows.Threading;
using System.ServiceModel;
using System.Collections.ObjectModel;
using Common;

namespace GPS.SilverlightMap
{
    public partial class HistoryPage : UserControl
    {
        const int LIGHT_INTERVAL_TIME = 100;
        //const int LIGHT_INTERVAL_TIME = 1;
        string tid = "";

        int lightIndex = -1;

        DMReaderSLServiceClient client = new DMReaderSLServiceClient();
        BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);

        GTUQueryServiceClient gtuClient = new GTUQueryServiceClient();

        private DispatcherTimer gtuLightTimer;

        ToTask currentTask = new ToTask();
        ToTask TaskForReview = new ToTask();
        ObservableCollection<ToTaskgtuinfomapping> tgMappingList = new ObservableCollection<ToTaskgtuinfomapping>();
        ObservableCollection<string> focusGTUIdList = new ObservableCollection<string>();
        bool isTimerStart = false;

        public HistoryPage()
        {
            InitializeComponent();
            IDictionary<string, string> paras = HtmlPage.Document.QueryString;

            if (paras.Count > 0)
            {
                tid = paras["tid"];
            }
            else
            {
                tid = "10";
            }
            binding.MaxReceivedMessageSize = int.MaxValue; binding.MaxBufferSize = int.MaxValue;
            client = new DMReaderSLServiceClient(binding, new EndpointAddress(new Uri(Application.Current.Host.Source, "../SilverlightServices/DMReaderSLService.svc")));

            client.GetTaskCompleted += new EventHandler<GetTaskCompletedEventArgs>(client_GetTaskCompleted);
            client.GetTaskAsync(int.Parse(tid));

            client.loadHistoryDotToLayerThreadCompleted+=new EventHandler<loadHistoryDotToLayerThreadCompletedEventArgs>(client_loadHistoryDotToLayerThreadCompleted);

            gtuLightTimer = new DispatcherTimer();
            gtuLightTimer.Interval = TimeSpan.FromMilliseconds(LIGHT_INTERVAL_TIME);
            //gtuLightTimer.Interval = TimeSpan.FromSeconds(LIGHT_INTERVAL_TIME);
            gtuLightTimer.Tick += new EventHandler(LightGTULoop);
        }

        public void client_GetTaskCompleted(object sender, GetTaskCompletedEventArgs e)
        {

            if (e.Result == null) return;

            TaskForReview = currentTask = ZipHelper.Uncompress<ToTask>(e.Result); //CompressedSerializerSL.Decompress<ToTask>(e.Result);

            if (currentTask.Taskgtuinfomappings == null) return;
            if (lstGTU.Items.Count == 0)
            {
                BindData(TaskForReview);
                tgMappingList = TaskForReview.Taskgtuinfomappings;
                bindlstGTU();
                btnRefresh.IsEnabled = true;
            }
            else
            {
                tgMappingList.Clear();
                tgMappingList = currentTask.Taskgtuinfomappings;
                btnRefresh.IsEnabled = true;
                replayHistory();
            }
        }

        public void BindData(ToTask totask)
        {
            int DmId = totask.DmId;
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            binding.MaxReceivedMessageSize = int.MaxValue; binding.MaxBufferSize = int.MaxValue;
            DMReaderSLServiceClient client = new DMReaderSLServiceClient(binding, new EndpointAddress(new Uri(Application.Current.Host.Source, "../SilverlightServices/DMReaderSLService.svc")));

            client.GetDistributionMapsByIdCompleted += new EventHandler<GetDistributionMapsByIdCompletedEventArgs>(client_GetDistributionMapsByIdCompleted);
            client.GetDistributionMapsByIdAsync(DmId);

            client.GetCustomAreaByBoxCompleted += new EventHandler<GetCustomAreaByBoxCompletedEventArgs>(client_GetCustomAreaByBoxCompleted);
            client.GetCustomAreaByBoxAsync();

        }

        public void client_GetDistributionMapsByIdCompleted(object sender, GetDistributionMapsByIdCompletedEventArgs e)
        {
            double maxlon = 0, minlon = 0, maxlat = 0, minlat = 0;
            if (e.Result == null) return;
            ToDistributionMap dm = CompressedSerializerSL.Decompress <ToDistributionMap>(e.Result);
            minlat = maxlat = minlon = maxlon = 0;
            LocationCollection collection = new LocationCollection();

            for (int i = 0; i < dm.DistributionMapCoordinates.Count; i++)
            {
                ToCoordinate cd = dm.DistributionMapCoordinates[i];
                collection.Add(new Location(cd.Latitude, cd.Longitude));

                if (i == 0)
                {
                    minlat = maxlat = cd.Latitude;
                    maxlon = minlon = cd.Longitude;
                }
                else
                {
                    if (cd.Latitude < minlat) minlat = cd.Latitude;
                    if (cd.Latitude > maxlat) maxlat = cd.Latitude;
                    if (cd.Longitude < minlon) minlon = cd.Longitude;
                    if (cd.Longitude > maxlon) maxlon = cd.Longitude;
                }
            }

            MapPolygon shape = DrawShape(collection, Colors.Gray, 0.3, Colors.Black, 0.8, 3);
            this.DMLayer.Children.Add(shape);

            this.MyMap.Center = new Location((minlat + maxlat) / 2, (minlon + maxlon) / 2);
            this.MyMap.ZoomLevel = 13;
        }

        public void client_GetCustomAreaByBoxCompleted(object sender, GetCustomAreaByBoxCompletedEventArgs e)
        {
            if (e.Result == null) return;
            List<ToArea> list = CompressedSerializerSL.Decompress<List<ToArea>>(e.Result);
            foreach (ToArea area in list)
            {
                LocationCollection collection = new LocationCollection();
                foreach (ToCoordinate c in area.Locations)
                {
                    collection.Add(new Location(c.Latitude, c.Longitude));
                }
                MapPolygon shape = DrawShape(collection, Colors.Red, 0.5, Colors.Black, 0.8, 2);
                this.NDLayer.Children.Add(shape);
            }
        }

        private MapPolygon DrawShape(LocationCollection collection, Color fillColor, double fillOpacity, Color borderColor, double borderOpacity, int borderWidth)
        {
            MapPolygon shape = new MapPolygon();
            shape.Locations = collection;
            Color strokeColor = borderColor;
            shape.Stroke = new SolidColorBrush(strokeColor);
            shape.StrokeThickness = borderWidth;
            shape.Stroke.Opacity = borderOpacity;
            Color fColor = fillColor;
            shape.Fill = new SolidColorBrush(fColor);
            shape.Fill.Opacity = fillOpacity;
            return shape;
        }

        public void bindlstGTU()
        {
            if (tgMappingList == null) return;

            lstGTU.Items.Clear();
            focusGTUIdList.Clear();
            foreach (ToTaskgtuinfomapping gtuInfo in tgMappingList)
            {
                string gid = gtuInfo.GTU.UniqueID;
                MapLayer maplayer = new MapLayer();
                pointsLayer.Children.Add(maplayer);
                gtuMaplayerdic.Add(gid, maplayer);
                lstGTU.Items.Add(new DisplayedItem(gid, gtuInfo.UserColor));
                gtuColorDic.Add(gid, toColor(gtuInfo.UserColor));
                cCountDic.Add(gid,0);
                focusGTUIdList.Add(gid);
            }

            for (int i = 0; i < lstGTU.Items.Count; i++)
                lstGTU.SelectedItems.Add(this.lstGTU.Items[i]);
            replayHistory();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            btnRefresh.IsEnabled = false;
            gtuLightTimer.Stop();
            isTimerStart = false;
            lightIndex = -1;
            foreach (MapLayer ml in gtuMaplayerdic.Values)
            {
                if (ml != null) { ml.Children.Clear(); }
            }
            focusGTUIdList.Clear();
            if ((lstGTU as ListBox).SelectedItems.Count > 0)
            {
                for (int i = 0; i < (lstGTU as ListBox).SelectedItems.Count; i++)
                    focusGTUIdList.Add((lstGTU.SelectedItems[i] as DisplayedItem).GTUId);
                client.GetTaskAsync(int.Parse(tid));
            }
            else
            {
                btnRefresh.IsEnabled = true;
                return;
            }
        }

        

        private void replayHistory()
        {    
            lightIndex = -1;
            foreach (string gid in focusGTUIdList)
            {
                client.loadHistoryDotToLayerThreadAsync(gid);
            }
            
            //gtuLightTimer.Start();
        }
          
        void client_loadHistoryDotToLayerThreadCompleted(object sender, loadHistoryDotToLayerThreadCompletedEventArgs e)
        {
            if (e.Result == null) return;
            string gid = e.Result;

            ToTaskgtuinfomapping ttgtuinfo = TaskForReview.Taskgtuinfomappings.ToList().Where(g => g.GTU.UniqueID == gid).FirstOrDefault();
            if (ttgtuinfo == null)
                return;
            MapLayer mm=gtuMaplayerdic[gid];
            ObservableCollection<ToGtuinfo> collection = ttgtuinfo.Gtuinfos;
            loadHistoryDotToLayer(mm, gtuColorDic[gid], collection);
            cCountDic[gid] = VisualTreeHelper.GetChildrenCount(mm);

            if (!isTimerStart)
            {
                gtuLightTimer.Start();  // timmer start event must later than the prepare of data array!!!
                isTimerStart = true;
            }
        }

        private void loadHistoryDotToLayer(MapLayer maplayer,Color col, ObservableCollection<ToGtuinfo> collection)
        {
            if (collection.Count == 0) return;
            int i = 0;
            foreach (ToGtuinfo gi in collection)
            {
                i++;
                Location loc = new Location(gi.dwLatitude, gi.dwLongitude);
                maplayer.Children.Add(CreateDot(col, loc));
            }
        }

        public UIElement CreateDot(Color col, Location location)
        {
            Color dotCol = Colors.Black;
            Ellipse elliple = new Ellipse() { Width = 12, Height = 12, Fill = new SolidColorBrush(col), Opacity = 0, Stroke = new SolidColorBrush(Colors.Red), StrokeThickness = 0.9};
            elliple.Stroke.Opacity = elliple.Opacity;
            MapLayer.SetPosition(elliple, location);
            MapLayer.SetPositionOrigin(elliple, PositionOrigin.Center);
            return elliple;
        }

        private void LightGTULoop(object sender, EventArgs e)
        {
            lightIndex++;
            int flag = 0;
            try
            {
                foreach (string gid in focusGTUIdList)
                {
                    if (lightIndex < cCountDic[gid])
                    {
                        flag = 1;
                        Ellipse ellipse = VisualTreeHelper.GetChild(gtuMaplayerdic[gid], lightIndex) as Ellipse;
                        ellipse.Opacity = 1;
                        ellipse.Stroke = new SolidColorBrush(Colors.Black);
                        ellipse.Stroke.Opacity = 1;
                    }

                }
                if (flag == 0) { gtuLightTimer.Stop(); isTimerStart = false; lightIndex = -1; }
            }
            catch
            {
                gtuLightTimer.Stop(); isTimerStart = false; lightIndex = -1;
            }
        }

        static Color toColor(string value)
        {
            value = value.Replace("#", "");
            Int32 v = Int32.Parse(value, System.Globalization.NumberStyles.HexNumber);
            return new Color
            {
                A = 255,
                R = Convert.ToByte((v >> 16) & 255),
                G = Convert.ToByte((v >> 8) & 255),
                B = Convert.ToByte((v >> 0) & 255),
            };
        }

        IDictionary<string, Color> gtucolordic = new Dictionary<string, Color>();
        public IDictionary<string, Color> gtuColorDic
        {
            get { return gtucolordic; }
            set { gtucolordic = value; }
        }

        IDictionary<string, MapLayer> _gtuMaplayerdic = new Dictionary<string, MapLayer>();
        public IDictionary<string, MapLayer> gtuMaplayerdic
        {
            get { return _gtuMaplayerdic; }
            set { _gtuMaplayerdic = value; }
        }

        IDictionary<string, int> ccountdic = new Dictionary<string, int>();
        public IDictionary<string, int> cCountDic
        {
            get { return ccountdic; }
            set { ccountdic = value; }
        }

        public class DisplayedItem
        {
            public DisplayedItem(string gid, string col)
            {
                GTUId = gid;
                Col = col;
                if (gid.Length > 6)
                    DisplayGTUId = gid.Substring(gid.Length - 6, 6);
                else
                    DisplayGTUId = gid;
            }
            public string GTUId { get; set; }
            public string DisplayGTUId { get; set; }
            public string Col { get; set; }
        }

    }
}
