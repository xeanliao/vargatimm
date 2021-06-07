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
using System.Windows.Threading;
using System.ServiceModel;
using System.Collections.ObjectModel;
//using GPS.SilverlightMap.GeocodeService;
using System.Runtime.Serialization;
using System.IO;
using Common;
using ICSharpCode.SharpZipLib.Zip;
//using GPS.SilverlightMap.GTUService;

namespace GPS.SilverlightMap
{
    public class ZipHelper
    {
        //public static string Serialize<T>(T obj)
        //{
        //    System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
        //    MemoryStream ms = new MemoryStream();
        //    serializer.WriteObject(ms, obj);
        //    string retVal = Encoding.Default.GetString(ms.ToArray());
        //    ms.Dispose();
        //    return retVal;
        //}
        public static byte[] Compress<T>(T obj)
        {
            var serializer = new System.Runtime.Serialization.DataContractSerializer(obj.GetType());
            // System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, obj);
            byte[] data = ms.ToArray();
            ms.Dispose();

            byte[] zipData;

            using (var outputStream = new MemoryStream())
            {
                using (var zip = new ZipOutputStream(outputStream))
                {
                    zip.IsStreamOwner = true;

                    zip.SetLevel(9); // high level

                    var zipEntry = new ZipEntry("content.dat")
                    {
                        DateTime = DateTime.Now,
                        Size = data.Length
                    };

                    zip.PutNextEntry(zipEntry);
                    zip.Write(data, 0, data.Length);

                    zip.Finish();
                }

                zipData = outputStream.ToArray();
                outputStream.Close();
            }


            return zipData;
        }

        //public static T Deserialize<T>(string json)
        //{
        //    T obj = Activator.CreateInstance<T>();
        //    MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
        //    System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
        //    obj = (T)serializer.ReadObject(ms);
        //    ms.Close();
        //    ms.Dispose();
        //    return obj;
        //}

        public static T Uncompress<T>(byte[] data)
        {
            T obj = Activator.CreateInstance<T>();
            MemoryStream ms = new MemoryStream(data);

            var unzipStream = new MemoryStream();

            using (var zip = new ZipInputStream(ms))
            {
                ZipEntry zipEntity;

                while ((zipEntity = zip.GetNextEntry()) != null)
                {
                    if (!zipEntity.IsFile) continue;

                    int size = 2048;
                    var buffer = new byte[2048];
                    while (true)
                    {
                        size = zip.Read(buffer, 0, buffer.Length);
                        if (size > 0)
                        {
                            unzipStream.Write(buffer, 0, size);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            var serializer = new System.Runtime.Serialization.DataContractSerializer(obj.GetType());
            // System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
            unzipStream.Position = 0;

            obj = (T)serializer.ReadObject(unzipStream);
            ms.Close();
            ms.Dispose();
            return obj;
        }
    }    

    public partial class EditGTU : UserControl
    {
        string tid = "";
        DMReaderSLServiceClient client = new DMReaderSLServiceClient();
        BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
        ToTask currentTask = new ToTask();
        ToTask orignTask = new ToTask();
        ObservableCollection<ToTaskgtuinfomapping> gtuInfoList = new ObservableCollection<ToTaskgtuinfomapping>();
        MapLayer currentMaplayer = new MapLayer();
        Color currentCol;
        string currentGTUId = "";
        int taskgtuid;
        const int ADD_DOT_TYPE = 9;
        Location dotLoc = new Location();

        //GTUQueryServiceClient gtuClient = new GTUQueryServiceClient();
        string dmRegisterStr = string.Empty;

        //GeocodeServiceClient gclient = new GeocodeServiceClient();
        //GeocodeRequest request = new GeocodeRequest();

        //ReverseGeocodeRequest request = new ReverseGeocodeRequest();

        public EditGTU()
        {
            InitializeComponent();

            IDictionary<string, string> paras = HtmlPage.Document.QueryString;

            if (paras.Count > 0)
            {
                tid = paras["id"];
            }
            else
            {
                tid = "10";
            }

            //request.Credentials = new Credentials();
            //request.Credentials.ApplicationId = "AkzZURoD0H2Sle6Nq_DE7pm7F3xOc8S3CjDTGNWkz1EFlJJkcwDKT1KcNcmYVINU";

            MyMap.MouseClick += new EventHandler<MapMouseEventArgs>(MyMap_MouseClick);
            MyMap.MouseDoubleClick += new EventHandler<MapMouseEventArgs>(MyMap_MouseDoubleClick);
            binding.MaxReceivedMessageSize = int.MaxValue; binding.MaxBufferSize = int.MaxValue;
            client = new DMReaderSLServiceClient(binding, new EndpointAddress(new Uri(Application.Current.Host.Source, "../SilverlightServices/DMReaderSLService.svc")));
            client.UpdateTaskCompleted += new EventHandler<UpdateTaskCompletedEventArgs>(client_UpdateTaskCompleted);
            client.GetTaskCompleted += new EventHandler<GetTaskCompletedEventArgs>(client_GetTaskCompleted);
            client.GetTaskAsync(int.Parse(tid));

            //gtuClient.RegisterDMCompleted += new EventHandler<RegisterDMCompletedEventArgs>(gtuClient_RegisterDMCompleted);
            //gtuClient.IsInAreaCompleted += new EventHandler<IsInAreaCompletedEventArgs>(gtuClient_IsInAreaCompleted);
            //gclient.ReverseGeocodeCompleted += new EventHandler<ReverseGeocodeCompletedEventArgs>(gclient_ReverseGeocodeCompleted);
        }

        void MyMap_MouseDoubleClick(object sender, MapMouseEventArgs e)
        {
            e.Handled = true;
        }

        public void client_GetTaskCompleted(object sender, GetTaskCompletedEventArgs e)
        {
            if (e.Result == null) return;
            string did = "";

            orignTask = currentTask = ZipHelper.Uncompress<ToTask>(e.Result);  // CompressedSerializerSL.Decompress<ToTask>(e.Result);
            
            did = currentTask.DmId.ToString();

            if (currentTask.Taskgtuinfomappings == null) return;
            gtuInfoList = currentTask.Taskgtuinfomappings;

            if (currentTask.Taskgtuinfomappings == null) return;

            bindData(currentTask);

            bindGtuList(currentTask.Taskgtuinfomappings);
            LoadDotFromTask(currentTask.Taskgtuinfomappings, true);
        }

        public void bindGtuList(ObservableCollection<ToTaskgtuinfomapping> currentTaskgtuinfoList)
        {
            if (currentTaskgtuinfoList == null) return;
            lstGTU.Items.Clear();
            //maplayerList.Clear();
            foreach (ToTaskgtuinfomapping gtuInfo in gtuInfoList)
            {
                string gid = gtuInfo.GTU.UniqueID;
                string gcolor = gtuInfo.UserColor;
                lstGTU.Items.Add(new DisplayedItem(gid, gcolor));
            }
            lstGTU.SelectedIndex = 0;
            //

        }

        private void LoadDotFromTask(ObservableCollection<ToTaskgtuinfomapping> currentTaskgtuinfoList, bool isInit)
        {
            foreach (ToTaskgtuinfomapping gtuInfo in currentTaskgtuinfoList)
            {
                string gid = gtuInfo.GTU.UniqueID;
                string gcolor = gtuInfo.UserColor;
                currentCol = toColor(gcolor);
                MapLayer maplayer = new MapLayer();
                gid = gid + gcolor;
                if (isInit)
                {
                    maplayer.Name = gid;
                    //for (int i = 0; i < pointsLayer.Children.Count; i++)
                    //{
                    //    MapLayer mapTemp = (MapLayer)pointsLayer.Children[i];
                    //    if (mapTemp.Name == maplayer.Name)
                    //    {
                    //        maplayer.Name = maplayer.Name + "_1";
                    //    }
                    //}
                    pointsLayer.Children.Add(maplayer);
                    tempDotDic.Add(gid, new ObservableCollection<ToGtuinfo>());
                    tempMaplayerDic.Add(gid, maplayer);
                }
                else
                {
                    maplayer = pointsLayer.FindName(gid) as MapLayer;
                }
                tempDotDic[gid].Clear();
                if (gtuInfo.Gtuinfos.Count == 0) continue;
                if (maplayer != null)
                {
                    maplayer.Children.Clear();
                    foreach (ToGtuinfo gi in gtuInfo.Gtuinfos)
                    {
                        if (gi.nCellID != 2)//434 do not show the removed dot,but also need add to dictionary,or it will be delete when save.
                        {
                            Location loc = new Location(gi.dwLatitude, gi.dwLongitude);
                            maplayer.Children.Add(CreateDot(gi));
                        }
                        tempDotDic[gid].Add(gi);
                    }
                }
            }
            lstGTU_SelectionChanged(lstGTU, null);
        }

        public void bindData(ToTask totask)
        {
            int DmId = totask.DmId;

            client.GetDistributionMapsByIdCompleted += new EventHandler<GetDistributionMapsByIdCompletedEventArgs>(client_GetDistributionMapsByIdCompleted);
            client.GetDistributionMapsByIdAsync(DmId);

            client.GetCustomAreaByBoxCompleted += new EventHandler<GetCustomAreaByBoxCompletedEventArgs>(client_GetCustomAreaByBoxCompleted);
            client.GetCustomAreaByBoxAsync();

        }

        LocationCollection Collections = new LocationCollection();
        public void client_GetDistributionMapsByIdCompleted(object sender, GetDistributionMapsByIdCompletedEventArgs e)
        {
            double minlat, maxlat, minlon, maxlon;
            if (e.Result == null) return;
            //ToDistributionMap dm = CompressedSerializerSL.Decompress<ToDistributionMap>(e.Result);
            ToDistributionMap dm = ZipHelper.Uncompress<ToDistributionMap>(e.Result); 
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
            Collections = collection;
            this.MyMap.Center = new Location((minlat + maxlat) / 2, (minlon + maxlon) / 2);
            this.MyMap.ZoomLevel = 13;
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

        private void lstGTU_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentGTUId = "";
            currentMaplayer = null;
            if ((sender as ListBox).SelectedItem == null)
                return;

            currentGTUId = ((sender as ListBox).SelectedItem as DisplayedItem).GTUId.ToString();
            currentCol = toColor(((sender as ListBox).SelectedItem as DisplayedItem).Col);
            string strColor = currentCol.ToString().Remove(1, 2);//431
            currentMaplayer = pointsLayer.FindName(currentGTUId + strColor) as MapLayer;//431
            //currentMaplayer = pointsLayer.FindName(currentGTUId) as MapLayer;//431
            ToTaskgtuinfomapping ttgtu = currentTask.Taskgtuinfomappings.ToList().Where(g => g.GTU.UniqueID == currentGTUId && toColor(g.UserColor) == currentCol).FirstOrDefault();
            taskgtuid = ttgtu.Id;
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

        public UIElement CreateDot(ToGtuinfo tgi)
        {
            Location location = new Location(tgi.dwLatitude, tgi.dwLongitude);
            Color dotCol = Colors.Black;
            string name;
            if (tgi.Id == 0)
            {
                dotCol = Colors.Red;
                name = tgi.sVersion;
            }
            else
            {
                name = tgi.Id.ToString();
                if (tgi.PowerInfo == ADD_DOT_TYPE)
                    dotCol = Colors.White;
                else
                    dotCol = Colors.Black;
            }
            Color col = new Color();
            col = currentCol;
            Ellipse elliple = new Ellipse() { Width = 12, Height = 12, Fill = new SolidColorBrush(col), Opacity = 1, Stroke = new SolidColorBrush(dotCol), StrokeThickness = 0.9, Name = name };
            elliple.Stroke.Opacity = 1;
            MapLayer.SetPosition(elliple, location);
            MapLayer.SetPositionOrigin(elliple, PositionOrigin.Center);
            return elliple;
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

        void MyMap_MouseClick(object sender, MapMouseEventArgs e)
        {

            if (currentGTUId == "" || currentCol == null || currentMaplayer == null) return;

            if (e.ViewportPoint != null)
            {
                string strColor = currentCol.ToString().Remove(1, 2);//431
                Point hotPt = new Point(e.ViewportPoint.X + 150, e.ViewportPoint.Y);
                List<UIElement> hitElements = VisualTreeHelper.FindElementsInHostCoordinates(hotPt, pointsLayer) as List<UIElement>;
                IList<Ellipse> hitEllis = new List<Ellipse>();
                if (hitElements != null && hitElements.Count > 0)
                {
                    foreach (UIElement u in hitElements)
                        if (u is Ellipse)
                            hitEllis.Add(u as Ellipse);
                }
                if (hitEllis.Count > 0)
                {
                    foreach (Ellipse u in hitEllis)
                    {
                        tempMaplayerDic[currentGTUId + strColor].Children.Remove(u);
                        string nm = (u as Ellipse).Name;
                        //ToGtuinfo delTgi = tempDotDic[currentGTUId].Where(ui => ui.sVersion == nm).FirstOrDefault();//431
                        ToGtuinfo delTgi = tempDotDic[currentGTUId + strColor].Where(ui => ui.sVersion == nm).FirstOrDefault();//431
                        if (delTgi == null)
                            //delTgi = tempDotDic[currentGTUId].Where(ui => ui.Id.ToString() == nm).FirstOrDefault();
                            delTgi = tempDotDic[currentGTUId + strColor].Where(ui => ui.Id.ToString() == nm).FirstOrDefault();//431                        
                        if (delTgi != null)
                        //tempDotDic[currentGTUId].Remove(delTgi);//431
                        {//434 0-original [then set status to 2],1-added [then remove it],2-removed
                            if (delTgi.nCellID == 1)
                            {
                                tempDotDic[currentGTUId + strColor].Remove(delTgi);
                            }
                            else if (delTgi.nCellID == 0)
                            {
                                //tempDotDic[currentGTUId + strColor].Where(ud =>
                                //    ud.Id == delTgi.Id).FirstOrDefault().nCellID = 2;
                                delTgi.nCellID = 2;
                                int iR = tempDotDic[currentGTUId + strColor].IndexOf(delTgi);
                                tempDotDic[currentGTUId + strColor][iR] = delTgi;
                            }
                        }
                    }
                }
                else
                    if (MyMap.TryViewportPointToLocation(e.ViewportPoint, out dotLoc))
                    {
                        //isInArea
                        //if (dmRegisterStr != string.Empty)
                        //    gtuClient.IsInAreaAsync(convertLocToCd(dotLoc), dmRegisterStr);
                        ToGtuinfo tgi = new ToGtuinfo();
                        tgi.Status = 0;
                        tgi.dwLatitude = dotLoc.Latitude;
                        tgi.dwLongitude = dotLoc.Longitude;
                        tgi.Code = currentGTUId;
                        tgi.dtSendTime = DateTime.Now.ToString();
                        tgi.dtReceivedTime = DateTime.Now.ToString();
                        tgi.TaskgtuinfoId = taskgtuid;
                        tgi.sIPAddress = string.Empty;
                        //434 0-original ,1-added ,2-removed
                        tgi.nCellID = 1;
                        string name = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                        tgi.sVersion = name;
                        tgi.PowerInfo = ADD_DOT_TYPE;
                        UIElement dot = CreateDot(tgi);
                        tempDotDic[currentGTUId + strColor].Add(tgi);
                        tempMaplayerDic[currentGTUId + strColor].Children.Add(dot);
                    }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //if (currentGTUId == string.Empty) return;
            //ObservableCollection<ToGtuinfo> giList = new ObservableCollection<ToGtuinfo>();
            //giList = currentTask.Taskgtuinfomappings.ToList().Where(g => g.GTU.UniqueID == currentGTUId).FirstOrDefault().Gtuinfos;
            //giList.Clear();
            //if (currentGTUId != "" && tempDotDic.ContainsKey(currentGTUId))
            //{
            //    if (tempDotDic[currentGTUId].Count > 0)
            //    {
            //        tempDotDic[currentGTUId].ToList().ForEach(mm => giList.Add(mm));
            //    }
            //    client.UpdateTaskAsync(CompressedSerializerSL.Compress<ToTask>(currentTask));
            //    btnSave.IsEnabled = false;
            //    btnRemove.IsEnabled = false;
            //    lstGTU.IsEnabled = false;
            //}
            //client.UpdateTaskAsync(currentTask);
            //foreach (ListBoxItem item in lstGTU.Items)
            //{
            //    string (item as DisplayedItem).GTUId;
            //}
            foreach (string key in tempDotDic.Keys)
            {
                string keyTemp = key.Remove(key.IndexOf("#"), 7);//431
                string colorTemp = key.Substring(key.IndexOf("#"));//431
                ObservableCollection<ToGtuinfo> giList = new ObservableCollection<ToGtuinfo>();
                //giList = currentTask.Taskgtuinfomappings.ToList().Where(g => g.GTU.UniqueID == key).FirstOrDefault().Gtuinfos;//431
                //the most important code is && colorTemp == g.UserColor,or else could not match the record,and 
                //NHibernate update database raise an error.
                giList = currentTask.Taskgtuinfomappings.ToList().Where(
                    g => g.GTU.UniqueID == keyTemp && colorTemp == g.UserColor).FirstOrDefault().Gtuinfos;//431
                giList.Clear();
                if (tempDotDic[key].Count > 0)
                {
                    tempDotDic[key].ToList().ForEach(mm => giList.Add(mm));
                }
            }
            client.UpdateTaskAsync(ZipHelper.Compress<ToTask>(currentTask));
            //client.UpdateTaskAsync(CompressedSerializerSL.Compress<ToTask>(currentTask));
            btnSave.IsEnabled = false;
            btnRemove.IsEnabled = false;
            btnRemoveAll.IsEnabled = false;
            lstGTU.IsEnabled = false;
        }

        void client_UpdateTaskCompleted(object sender, UpdateTaskCompletedEventArgs e)
        {
            if (e.Result == null) return;
            currentTask = ZipHelper.Uncompress<ToTask>(e.Result); ;
            //currentTask = CompressedSerializerSL.Decompress<ToTask>(e.Result);
            //currentMaplayer.Children.Clear();
            //ObservableCollection<ToGtuinfo> currentGTUInfoList = currentTask.Taskgtuinfomappings.ToList().Where(g => g.GTU.UniqueID == currentGTUId).FirstOrDefault().Gtuinfos;
            //foreach (MapLayer maplayer in pointsLayer.Children)
            //{
            //    maplayer.Children.Clear();
            //}
            LoadDotFromTask(currentTask.Taskgtuinfomappings, false);
            btnSave.IsEnabled = true;
            btnRemove.IsEnabled = true;
            btnRemoveAll.IsEnabled = true;
            lstGTU.IsEnabled = true;
        }


        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            lstGTU.IsEnabled = false;
            currentMaplayer.Children.Clear();
            ObservableCollection<ToGtuinfo> currentGTUInfoList = currentTask.Taskgtuinfomappings.ToList().Where(
                g => g.GTU.UniqueID == currentGTUId).FirstOrDefault().Gtuinfos;
            LoadDotFromTask(currentTask.Taskgtuinfomappings, false);
            lstGTU.IsEnabled = true;

        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            HtmlWindow html = HtmlPage.Window;
            HtmlPage.Window.Eval("window.open('OneDMPrintSettings.aspx?id=" + tid + "', '_blank', 'resizable=yes,status=no,toolbar=no,menubar=no,location=no')");
        }

        Dictionary<string, ObservableCollection<ToGtuinfo>> _tempDotDic = new Dictionary<string, ObservableCollection<ToGtuinfo>>();
        public Dictionary<string, ObservableCollection<ToGtuinfo>> tempDotDic
        {
            get { return _tempDotDic; }
            set { _tempDotDic = value; }
        }

        Dictionary<string, MapLayer> _tempMaplayerDic = new Dictionary<string, MapLayer>();
        public Dictionary<string, MapLayer> tempMaplayerDic
        {
            get { return _tempMaplayerDic; }
            set { _tempMaplayerDic = value; }
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

        //Remove all dots outside of map
        private void btnRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                lstGTU.IsEnabled = false;

                LocationService.LocationServiceClient locClient =
                    new LocationService.LocationServiceClient();
                //locClient.PointInPolygonCompleted +=
                //    new EventHandler<LocationService.PointInPolygonCompletedEventArgs>(locClient_PointInPolygonCompleted);
                locClient.PointsInPolygonCompleted +=
                    new EventHandler<LocationService.PointsInPolygonCompletedEventArgs>(locClient_PointsInPolygonCompleted);

                ObservableCollection<ObservableCollection<double>> coor =
                    new ObservableCollection<ObservableCollection<double>>();
                Dictionary<ObservableCollection<object>, ObservableCollection<double>> tgiList =
                    new Dictionary<ObservableCollection<object>, ObservableCollection<double>>();

                foreach (Location l in Collections)
                {
                    ObservableCollection<double> latlon = new ObservableCollection<double>{
                    l.Latitude,l.Longitude};
                    coor.Add(latlon);
                }
                foreach (string gidLayer in tempDotDic.Keys)
                {
                    foreach (ToGtuinfo tgi in tempDotDic[gidLayer])
                    {
                        //locClient.PointInPolygonAsync(coor, new ObservableCollection<double>{
                        //tgi.dwLatitude,tgi.dwLongitude}, new ObservableCollection<object>{
                        //tgi.Code,tgi.Distance,tgi.dtReceivedTime,tgi.dtSendTime,tgi.dwAltitude,
                        //tgi.dwLatitude,tgi.dwLongitude,tgi.dwSpeed,tgi.Id,tgi.nAccuracy,tgi.nAreaCode,
                        //tgi.nCellID,tgi.nCount,tgi.nGPSFix,tgi.nHeading,tgi.nLocationID,tgi.nNetworkCode,
                        //tgi.PowerInfo,tgi.sIPAddress,tgi.Status,tgi.sVersion,tgi.TaskgtuinfoId,gidLayer});

                        //locClient.PointInPolygonAsync(coor, new ObservableCollection<double>{
                        //tgi.dwLatitude,tgi.dwLongitude}, new ObservableCollection<object>{
                        //tgi.Id,tgi.sVersion,gidLayer});

                        tgiList.Add(new ObservableCollection<object> { tgi.Id, tgi.sVersion, gidLayer },
                            new ObservableCollection<double> { tgi.dwLatitude, tgi.dwLongitude });
                    }
                }
                locClient.PointsInPolygonAsync(coor, tgiList);

                lstGTU.IsEnabled = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void locClient_PointInPolygonCompleted(object sender, LocationService.PointInPolygonCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (e.Result != null)
                {
                    lstGTU.IsEnabled = false;
                    btnSave.IsEnabled = false;
                    btnRemove.IsEnabled = false;
                    btnRemoveAll.IsEnabled = false;

                    ObservableCollection<object> tgi = e.Result;
                    ToGtuinfo temptgi = new ToGtuinfo();
                    //temptgi.Code = tgi[0] == null ? null : tgi[0].ToString();
                    //temptgi.Distance = (double)tgi[1];
                    //temptgi.dtReceivedTime = tgi[2] == null ? null : tgi[2].ToString();
                    //temptgi.dtSendTime = tgi[3] == null ? null : tgi[3].ToString();
                    //temptgi.dwAltitude = (double)tgi[4];
                    //temptgi.dwLatitude = (double)tgi[5];
                    //temptgi.dwLongitude = (double)tgi[6];
                    //temptgi.dwSpeed = (double)tgi[7];
                    //temptgi.Id = (int)tgi[8];
                    //temptgi.nAccuracy = (int)tgi[9];
                    //temptgi.nAreaCode = (int)tgi[10];
                    //temptgi.nCellID = (int)tgi[11];
                    //temptgi.nCount = (int)tgi[12];
                    //temptgi.nGPSFix = (long)tgi[13];
                    //temptgi.nHeading = (int)tgi[14];
                    //temptgi.nLocationID = (int)tgi[15];
                    //temptgi.nNetworkCode = (int)tgi[16];
                    //temptgi.PowerInfo = (int)tgi[17];
                    //temptgi.sIPAddress = tgi[18] == null ? null : tgi[18].ToString();
                    //temptgi.Status = (int)tgi[19];
                    //temptgi.sVersion = tgi[20] == null ? null : tgi[20].ToString();
                    //temptgi.TaskgtuinfoId = (int)tgi[21];

                    temptgi.Id = (int)tgi[0];
                    temptgi.sVersion = tgi[1] == null ? null : tgi[1].ToString();

                    if (temptgi.Id != 0)
                    {
                        ToGtuinfo delTgi = tempDotDic[tgi[2].ToString()].Where(ui => ui.Id == temptgi.Id).FirstOrDefault();
                        tempDotDic[tgi[2].ToString()].Remove(delTgi);
                    }
                    else
                    {
                        ToGtuinfo delTgi = tempDotDic[tgi[2].ToString()].Where(ui => ui.sVersion == temptgi.sVersion).FirstOrDefault();
                        tempDotDic[tgi[2].ToString()].Remove(delTgi);
                    }

                    UIElementCollection uic = tempMaplayerDic[tgi[2].ToString()].Children;
                    if (temptgi.Id == 0)
                    {
                        var query = from el in uic.OfType<Ellipse>()
                                    where el.Name == temptgi.sVersion
                                    select el;
                        tempMaplayerDic[tgi[2].ToString()].Children.Remove(query.First());
                    }
                    else
                    {
                        var query = from el in uic.OfType<Ellipse>()
                                    where el.Name == temptgi.Id.ToString()
                                    select el;
                        tempMaplayerDic[tgi[2].ToString()].Children.Remove(query.First());
                    }

                    //foreach (UIElement u in uic)
                    //{
                    //    Ellipse ei = (Ellipse)u;
                    //    if (temptgi.Id == 0)
                    //    {
                    //        if (ei.Name == temptgi.sVersion)
                    //        {
                    //            tempMaplayerDic[tgi[22].ToString()].Children.Remove(u);
                    //            break;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (ei.Name == temptgi.Id.ToString())
                    //        {
                    //            tempMaplayerDic[tgi[22].ToString()].Children.Remove(u);
                    //            break;
                    //        }
                    //    }
                    //}
                    lstGTU.IsEnabled = true;
                    btnSave.IsEnabled = true;
                    btnRemove.IsEnabled = true;
                    btnRemoveAll.IsEnabled = true;
                }
            }
            else
            {
                throw new Exception(e.Error.Message);
            }
        }

        private void locClient_PointsInPolygonCompleted(object sender, LocationService.PointsInPolygonCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (e.Result != null)
                {
                    lstGTU.IsEnabled = false;
                    btnSave.IsEnabled = false;
                    btnRemove.IsEnabled = false;
                    btnRemoveAll.IsEnabled = false;

                    ObservableCollection<ObservableCollection<object>> tgis = e.Result;
                    foreach (ObservableCollection<object> tgi in tgis)
                    {
                        ToGtuinfo temptgi = new ToGtuinfo();

                        temptgi.Id = (int)tgi[0];
                        temptgi.sVersion = tgi[1] == null ? null : tgi[1].ToString();

                        if (temptgi.Id != 0)
                        {
                            ToGtuinfo delTgi = tempDotDic[tgi[2].ToString()].Where(ui => ui.Id == temptgi.Id).FirstOrDefault();
                            tempDotDic[tgi[2].ToString()].Remove(delTgi);
                        }
                        else
                        {
                            ToGtuinfo delTgi = tempDotDic[tgi[2].ToString()].Where(ui => ui.sVersion == temptgi.sVersion).FirstOrDefault();
                            tempDotDic[tgi[2].ToString()].Remove(delTgi);
                        }

                        UIElementCollection uic = tempMaplayerDic[tgi[2].ToString()].Children;
                        if (temptgi.Id == 0)
                        {
                            var query = from el in uic.OfType<Ellipse>()
                                        where el.Name == temptgi.sVersion
                                        select el;
                            tempMaplayerDic[tgi[2].ToString()].Children.Remove(query.First());
                        }
                        else
                        {
                            var query = from el in uic.OfType<Ellipse>()
                                        where el.Name == temptgi.Id.ToString()
                                        select el;
                            tempMaplayerDic[tgi[2].ToString()].Children.Remove(query.First());
                        }
                    }

                    lstGTU.IsEnabled = true;
                    btnSave.IsEnabled = true;
                    btnRemove.IsEnabled = true;
                    btnRemoveAll.IsEnabled = true;
                }
            }
            else
            {
                throw new Exception(e.Error.Message);
            }
        }
    }
}
