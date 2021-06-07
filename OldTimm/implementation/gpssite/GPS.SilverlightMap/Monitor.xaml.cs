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
//using GPS.SilverlightMap.GeocodeService;

namespace GPS.SilverlightMap
{
    public partial class Monitor : UserControl
    {
        const int INTERVAL_TIME = 5;
        string tid = "";
        bool isStop = true;

        DMReaderSLServiceClient client = new DMReaderSLServiceClient();
        BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);

        GTUQueryServiceClient gtuClient = new GTUQueryServiceClient();

        private DispatcherTimer gtuMonitorTimer;

        ToTask currentTask = new ToTask();
        ToTask getTaskForReplay = new ToTask();
        ObservableCollection<ToTaskgtuinfomapping> gtuInfoList = new ObservableCollection<ToTaskgtuinfomapping>();

        double maxlon = 0, minlon = 0, maxlat = 0, minlat = 0;

        ObservableCollection<string> focusGTUIdList = new ObservableCollection<string>();

        //GeocodeServiceClient gclient = new GeocodeServiceClient();
        //GeocodeRequest request = new GeocodeRequest();

        //string dmRegisterStr = "";

        public Monitor()
        {
            InitializeComponent();

            IDictionary<string, string> paras = HtmlPage.Document.QueryString;

            if (paras.Count > 0)
            {
                tid = paras["id"];
            }
            else
            {
                tid = "12";
            }

            binding.MaxReceivedMessageSize = int.MaxValue; binding.MaxBufferSize = int.MaxValue;
            client = new DMReaderSLServiceClient(binding, new EndpointAddress(new Uri(Application.Current.Host.Source, "../SilverlightServices/DMReaderSLService.svc")));

            client.GetTaskCompleted += new EventHandler<GetTaskCompletedEventArgs>(client_GetTaskCompleted);
            client.GetTaskAsync(int.Parse(tid));

            client.GetGtuUserNameDicCompleted += new EventHandler<GetGtuUserNameDicCompletedEventArgs>(client_GetGtuUserNameDicCompleted);
            client.GetGtuUserNameDicAsync(int.Parse(tid));

            client.GetMAddressListByTaskIdCompleted += new EventHandler<GetMAddressListByTaskIdCompletedEventArgs>(client_GetMAddressListByTaskIdCompleted);
            client.GetMAddressListByTaskIdAsync(int.Parse(tid));

            client.AddTaskTimeCompleted += new EventHandler<AddTaskTimeCompletedEventArgs>(client_AddTaskTimeCompleted);

            gtuClient.GetGTUsCompleted += new EventHandler<GetGTUsCompletedEventArgs>(gtuClient_GetGTUsCompleted);

            //client.GetMAddressCompleted += new EventHandler<GetGTUsCompletedEventArgs>(gtuClient_GetMAddressCompleted);
            
            //gtuClient.RegisterDMCompleted += new EventHandler<RegisterDMCompletedEventArgs>(gtuClient_RegisterDMCompleted);
            //gtuClient.AddTaskToStoreCompleted += new EventHandler<AddTaskToStoreCompletedEventArgs>(gtuClient_AddTaskToStoreCompleted);

            //gclient.ReverseGeocodeCompleted+=new EventHandler<ReverseGeocodeCompletedEventArgs>(gclient_ReverseGeocodeCompleted);

            gtuMonitorTimer = new DispatcherTimer();
            gtuMonitorTimer.Interval = TimeSpan.FromSeconds(INTERVAL_TIME);
            gtuMonitorTimer.Tick += new EventHandler(GTUMonitorLoop);
            
        }

        public void client_GetMAddressListByTaskIdCompleted(object sender, GetMAddressListByTaskIdCompletedEventArgs e)
        {
            if (e.Result == null || e.Result.Count == 0) return;
            AddressLayer.Children.Clear();
            if(e.Result.Count>=5){
                btnAddAddress.IsEnabled = false;
            }
            foreach (ToMonitorAddresses m in e.Result)
            {
                Location dwCurrentLocation = new Location(m.Latitude, m.Longitude);
                Color clr = Colors.Blue;
                Rectangle elliple = new Rectangle() { Width = 15, Height = 15, Fill = new SolidColorBrush(clr), Opacity = 1, Stroke = new SolidColorBrush(Colors.Black), StrokeThickness = 0.9 };
                elliple.Stroke.Opacity = 1;
                MapLayer.SetPosition(elliple, dwCurrentLocation);
                MapLayer.SetPositionOrigin(elliple, PositionOrigin.Center);

                elliple.Tag = m.Id;
                elliple.MouseLeftButtonDown += new MouseButtonEventHandler(Mydot_MouseLeftButtonDown);

                ToolTipService.SetToolTip(elliple, " " + m.Address1 + ", " + m.ZipCode);
                AddressLayer.Children.Add(elliple);
            }
            return;

        }

        void Mydot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rect = (Rectangle)sender;
            string addressId = Convert.ToString(rect.Tag);

            HtmlWindow html = HtmlPage.Window;
            HtmlPage.Window.Eval("window.open('NewAddressSL.aspx?did=" + currentTask.DmId + "&id=" + addressId + "', '_blank', 'height=400px,width=400px,resizable=yes,status=no,toolbar=no,menubar=no,location=no')");
        }

        public void client_GetGtuUserNameDicCompleted(object sender, GetGtuUserNameDicCompletedEventArgs e)
        {
            if (e.Result == null) return;
            gtuUserNameDic = e.Result;
        }

        //public void gtuClient_AddTaskToStoreCompleted(object sender, AddTaskToStoreCompletedEventArgs e)
        //{
        //    //throw new NotImplementedException();
        //}

        //void gtuClient_RegisterDMCompleted(object sender, RegisterDMCompletedEventArgs e)
        //{
            
        //    if (e.Result != null)
        //    {
        //        dmRegisterStr = e.Result;
        //    }
        //    if (btnTaskCtl.Content.ToString() == "Stop" && isLoadGTULstOver)
        //        btnRefresh_Click(null, null);
        //    else
        //        isLoadDMOver = true;
        //    //throw new NotImplementedException();
        //}

        public void client_GetTaskCompleted(object sender, GetTaskCompletedEventArgs e)
        {
            gtuInfoList.Clear();
            if (e.Result == null) return;
            currentTask = ZipHelper.Uncompress<ToTask>(e.Result); // CompressedSerializerSL.Decompress<ToTask>(e.Result);
            txtEmail.Text = currentTask.Email;
            txtTel.Text = currentTask.Telephone;
            if (currentTask.Tasktimes != null && currentTask.Tasktimes.Count > 0)
            {
                btnTaskCtl.Content = (currentTask.Tasktimes.Last().TimeType == 1 ? "Start" : "Stop");
            }
            btnTaskCtl.IsEnabled = true;
            if (currentTask.Taskgtuinfomappings == null) return;
            gtuInfoList = currentTask.Taskgtuinfomappings;
            BindData(currentTask);
            bindGtuInfoList();
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
            if (e.Result == null) return;
            ToDistributionMap dm = CompressedSerializerSL.Decompress <ToDistributionMap>(e.Result);
            minlat = maxlat = minlon = maxlon = 0;
            LocationCollection collection = new LocationCollection();
            ObservableCollection<Coordinate> oarea = new ObservableCollection<Coordinate>();
            for (int i = 0; i < dm.DistributionMapCoordinates.Count; i++)
            {
                ToCoordinate cd = dm.DistributionMapCoordinates[i];
                collection.Add(new Location(cd.Latitude, cd.Longitude));
                Coordinate cn = new Coordinate();
                cn.Latitude = cd.Latitude;
                cn.Longitude = cd.Longitude;
                oarea.Add(cn);
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
            if (btnTaskCtl.Content.ToString() == "Stop")
            {
                btnRefresh.IsEnabled = true;
                lstGTU.IsEnabled = true;
                isStop = false;
            }
            //gtuClient.RegisterDMAsync(oarea);

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

        private void btnTaskCtl_Click(object sender, RoutedEventArgs e)
        {
            btnTaskCtl.IsEnabled = false;
            client.AddTaskTimeAsync(int.Parse(tid), ((btnTaskCtl.Content.ToString() == "Start") ? 0 : 1));
        }

        void client_AddTaskTimeCompleted(object sender, AddTaskTimeCompletedEventArgs e)
        {
            if (e.Result == null) return;
            if (btnTaskCtl.Content.ToString() == "Start")
            {
                gtuClient.AddTaskToStoreAsync(int.Parse(tid));
                btnTaskCtl.Content = "Stop";
                btnRefresh.IsEnabled = true;
                lstGTU.IsEnabled = true;
                isStop = false;
            }
            else 
            {
                gtuClient.RemoveTaskFromStoreAsync(int.Parse(tid));
                btnTaskCtl.Content = "Start";
                btnRefresh.IsEnabled = false;
                lstGTU.IsEnabled = false;
                isStop = true;
            
            }
            btnTaskCtl.IsEnabled = true;
            //btnTaskCtl.Content = (btnTaskCtl.Content.ToString() == "Start") ? "Stop" : "Start";
            //btnTaskCtl.IsEnabled = true;
            //if (btnTaskCtl.Content.ToString() == "Start")
            //{
            //    btnRefresh.IsEnabled = false;
            //    lstGTU.IsEnabled = false;
            //    isStop = true;
            //}
            //else
            //{
            //    btnRefresh.IsEnabled = true;
            //    lstGTU.IsEnabled = true;
            //    isStop = false;
            //}
        }

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            HtmlWindow html = HtmlPage.Window;
            HtmlPage.Window.Eval("window.open('History.aspx?tid=" + tid + "', '_blank', 'resizable=yes,status=yes,toolbar=yes,menubar=yes,location=yes')");
        }

        public void bindGtuInfoList()
        {
            if (gtuInfoList == null || gtuInfoList.Count==0) return;
            btnHistory.IsEnabled = true;
            lstGTU.Items.Clear();
            foreach (ToTaskgtuinfomapping gtuInfo in gtuInfoList)
            {
                string gid=gtuInfo.GTU.UniqueID;
                lstGTU.Items.Add(new DisplayedItem(gid, gtuInfo.UserColor));
                gtuColDic.Add(gid, toColor(gtuInfo.UserColor));
            }
            for (int i = 0; i < lstGTU.Items.Count; i++)
                lstGTU.SelectedItems.Add(this.lstGTU.Items[i]);
            
            if (btnTaskCtl.Content.ToString() == "Stop")
                btnRefresh_Click(null, null);
            gtuMonitorTimer.Start();
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            pointsLayer.Children.Clear();
            gtuMaplayerDic.Clear();
            focusGTUIdList.Clear();
            if ((lstGTU as ListBox).SelectedItems.Count > 0)
            {
                for (int i = 0; i < (lstGTU as ListBox).SelectedItems.Count; i++)
                {
                    string itemgtu = (lstGTU.SelectedItems[i] as DisplayedItem).GTUId;
                    focusGTUIdList.Add(itemgtu);
                }
                createLayersForGtu(focusGTUIdList);
                //gtuClient.GetGTUsAsync();
            }
            else
                return;
        }

        private void btnAddAddress_Click(object sender, RoutedEventArgs e)
        {
            //pointsLayer.Children.Clear();
            //gtuMaplayerDic.Clear();
            //focusGTUIdList.Clear();
            //if ((lstGTU as ListBox).SelectedItems.Count > 0)
            //{
            //    for (int i = 0; i < (lstGTU as ListBox).SelectedItems.Count; i++)
            //    {
            //        string itemgtu = (lstGTU.SelectedItems[i] as DisplayedItem).GTUId;
            //        focusGTUIdList.Add(itemgtu);
            //    }
            //    createLayersForGtu(focusGTUIdList);
            //    //gtuClient.GetGTUsAsync();
            //}
            //else
            //    return;
            HtmlWindow html = HtmlPage.Window;
            HtmlPage.Window.Eval("window.open('NewAddressSL.aspx?did=" + currentTask.DmId + "', '_blank', 'height=400px,width=400px,resizable=yes,status=no,toolbar=no,menubar=no,location=no')");
        }

        public void gtuClient_GetGTUsCompleted(object sender, GetGTUsCompletedEventArgs e)
        {
            if (isStop) return;
            if (e.Result == null || e.Result.Count==0) return;
            foreach (string gid in gtuMaplayerDic.Keys)
            {
                MapLayer layer = gtuMaplayerDic[gid];
                if(layer.Children!=null && layer.Children.Count>0){
                    (layer.Children[0] as Ellipse).Opacity = 0.5;
                }
            }
            MapLayer currentLayer = new MapLayer();
            foreach (GTU cg in e.Result)
            {
                string cgid = cg.Code;
                currentLayer = gtuMaplayerDic[cgid];
                currentLayer.Children.Clear();
                Color col = gtuColDic[cgid];
                //if (cg.Status == Status.OutAndFrozen || cg.Status == Status.OutBoundary)
                //    col = Colors.Red;
                Location dwCurrentLocation = new Location(cg.CurrentCoordinate.Latitude, cg.CurrentCoordinate.Longitude);
                UIElement Mydot = setMyDot(col, dwCurrentLocation);
                string userName = string.Empty;
                if (cg.Status == Status.OutAndFrozen || cg.Status == Status.OutBoundary)
                {
                    //UIElement MyAnimationdot = setAnimationDot(col, dwCurrentLocation, currentLayer);
                    //currentLayer.Children.Add(MyAnimationdot);
                    setAnimationDot(col, dwCurrentLocation, currentLayer);
                }
                
                gtuUserNameDic.TryGetValue(cgid,out userName);
                ToolTip tp = new ToolTip();
                tp.Tag=dwCurrentLocation;
                tp.Opened += new RoutedEventHandler(tooltip_Opened);
                //tp.MouseLeave += new MouseEventHandler(tp_MouseLeave);
                tp.Content="User Name:" + userName + "\nGTUCode:" + cgid + "\nTime:" + cg.SendTime.ToLongTimeString() + "\nAddress:Loading...";
                Mydot.SetValue(ToolTipService.ToolTipProperty,tp);
                currentLayer.Children.Add(Mydot);
            }
        }

        void tp_MouseLeave(object sender, MouseEventArgs e)
        {
            //System.Windows.Browser.HtmlPage.Window.Alert("leave");
        }
                    
                    
        void tooltip_Opened(object sender, RoutedEventArgs e)
        {
            //System.Windows.Browser.HtmlPage.Window.Alert("open");
            ToolTip tooltip = sender as ToolTip;
            Location loc = tooltip.Tag as Location;
            //ReverseGeocodeRequest request = new ReverseGeocodeRequest();
            //request.Credentials = new Credentials();
            //request.Credentials.ApplicationId = "AkzZURoD0H2Sle6Nq_DE7pm7F3xOc8S3CjDTGNWkz1EFlJJkcwDKT1KcNcmYVINU";

            //request.Location = loc;

            //gclient.ReverseGeocodeAsync(request,tooltip);
        }
        /*
        void gclient_ReverseGeocodeCompleted(object sender, ReverseGeocodeCompletedEventArgs e)
        {
            //System.Windows.Browser.HtmlPage.Window.Alert("gotta");
            ToolTip tp = e.UserState as ToolTip;
            if (tp.IsOpen == true)
            {
                string con = tp.Content.ToString();
                tp.Content = con.Replace("Loading...", e.Result.Results[0].Address.FormattedAddress);
            }
        }
         */

        void GTUMonitorLoop(object sender, EventArgs e)
        {
            if (isStop) return;
            if (focusGTUIdList.Count > 0)
            {
                gtuClient.GetGTUsAsync(focusGTUIdList);
            }
        }

        public void createLayersForGtu(ObservableCollection<string> gids)
        {
            foreach (string gid in gids)
            {
                MapLayer layer = new MapLayer();
                pointsLayer.Children.Add(layer);
                gtuMaplayerDic.Add(gid,layer);
            }
        }

        public string getMaplayerName(string gid)
        {
            return "pointlayer_" + gid;
        }

        public UIElement setMyDot(Color col, Location location)
        {
            Ellipse elliple = new Ellipse() { Width = 12, Height = 12, Fill = new SolidColorBrush(col), Opacity = 1, Stroke = new SolidColorBrush(Colors.Black), StrokeThickness = 0.9 };
            elliple.Stroke.Opacity = 1;
            MapLayer.SetPosition(elliple, location);
            MapLayer.SetPositionOrigin(elliple, PositionOrigin.Center);
            return elliple;
        }

        public UIElement setFronzeDot(Color col, Location location)
        {
            Ellipse elliple = new Ellipse() { Width = 12, Height = 12, Fill = new SolidColorBrush(col), Opacity = 1, Stroke = new SolidColorBrush(Colors.Black), StrokeThickness = 0.9 };
            elliple.Stroke.Opacity = 1;
            MapLayer.SetPosition(elliple, location);
            MapLayer.SetPositionOrigin(elliple, PositionOrigin.Center);
            return elliple;
        }

        public UIElement setAnimationDot(Color col, Location location,MapLayer currMaplayer)
        {
            //Ellipse elliple = new Ellipse() { Width = 12, Height = 12, Fill = new SolidColorBrush(col), Opacity = 1, Stroke = new SolidColorBrush(Colors.Black), StrokeThickness = 0.9 };
            //elliple.Stroke.Opacity = 1;
            //MapLayer.SetPosition(elliple, location);
            //MapLayer.SetPositionOrigin(elliple, PositionOrigin.Center);
            //return elliple;

            var ellipse2 = new System.Windows.Shapes.Ellipse();
            ellipse2.Width = 36;
            ellipse2.Height = 36;
            ellipse2.Opacity = 0.3;
            ellipse2.Stroke = new SolidColorBrush(Colors.White);
            ellipse2.Fill = new SolidColorBrush(col);
            MapLayer.SetPosition(ellipse2, location);
            MapLayer.SetPositionOrigin(ellipse2, PositionOrigin.Center);

            ellipse2.RenderTransform = new TranslateTransform();
            Storyboard sb = new Storyboard();
            DoubleAnimation da = new DoubleAnimation();
            da.From = 0.7;
            da.To = 0;
            da.RepeatBehavior = RepeatBehavior.Forever;
            da.Duration = new Duration(new TimeSpan(0, 0, 1));
            Storyboard.SetTarget(da, ellipse2);
            Storyboard.SetTargetProperty(da, new PropertyPath("(UIElement.Opacity)"));
            sb.Children.Add(da);
            //myBall.Resources.Add("sb1", sb);
            sb.Begin();
            currMaplayer.Children.Add(ellipse2);
            //MapLayer myMaplyer = new MapLayer();
            return ellipse2;
            //pointsLayer.Children.Add(ellipse2);
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

        IDictionary<string, Color> _gtuColDic = new Dictionary<string, Color>();
        IDictionary<string, Color> gtuColDic
        {
            get { return _gtuColDic;}
            set { _gtuColDic=value;}
        }

        IDictionary<string, string> _gtuUserNameDic = new Dictionary<string, string>();
        IDictionary<string, string> gtuUserNameDic
        {
            get { return _gtuUserNameDic; }
            set { _gtuUserNameDic = value; }
        }

        IDictionary<string, MapLayer> _gtuMaplayerDic = new Dictionary<string, MapLayer>();
        IDictionary<string, MapLayer> gtuMaplayerDic
        {
            get { return _gtuMaplayerDic; }
            set { _gtuMaplayerDic = value; }
        }
    }

    //listbox item display style
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