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
using TIMM.GPS.Net.Http;
using TIMM.GPS.ControlCenter.Model;
using Map = Microsoft.Maps.MapControl;
using Microsoft.Maps.MapControl;
using TIMM.GPS.ControlCenter.Extend;
using TIMM.GPS.ControlCenter.Extends;
using System.Windows.Media.Imaging;
using System.IO;
using ImageTools;
using System.Xml.Serialization;
using TIMM.GPS.Model;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using System.ServiceModel;
using System.ComponentModel;
using System.Windows.Browser;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;

namespace TIMM.GPS.ControlCenter.Views
{
    public partial class DistributionMap_Snap : UserControl
    {
        DispatcherTimer taskTimer;
        LocationService.LocationServiceClient proxy;

        public DistributionMap_Snap()
        {
            InitializeComponent();
            //check iso storage
            long availableFreeSpace = 0;
            try
            {
                availableFreeSpace = IsolatedStorageFile.GetUserStoreForApplication().AvailableFreeSpace;
            }
            catch
            {

            }
            if (availableFreeSpace < 1L * 1024L * 1024L * 1024L)
            {
                btnStart.Visibility = Visibility.Visible;
                map.Visibility = Visibility.Collapsed;
            }else
            {
                btnStart.Visibility = Visibility.Collapsed;
                map.Visibility = Visibility.Visible;
                this.Loaded += new RoutedEventHandler(OnLoaded);
            }
            #region use proxy to build map image
            m_MapImageWorker.DoWork += BuildMapImage;
            m_MapImageWorker.RunWorkerCompleted += SendMapSnap;
            printImageWidth = map.Width;
            printImageHeight = map.Height;

            BaseURL = string.Format("http://{0}:{1}{2}",
                HtmlPage.Document.DocumentUri.DnsSafeHost,
                HtmlPage.Document.DocumentUri.Port,
                "/MapProxy/MapImageServer.ashx");

            #endregion

#if DEBUG
            btnStart.Visibility = Visibility.Visible;
#endif
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            using (var isoFile = IsolatedStorageFile.GetUserStoreForApplication())
            {
                isoFile.IncreaseQuotaTo(2L * 1024L * 1024L * 1024L);
            }

            btnStart.Visibility = Visibility.Collapsed;
            map.Visibility = Visibility.Visible;

            OnLoaded(sender, e);
        }

        private void btnMock_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            var result = dialog.ShowDialog();
            if (result == true)
            {
                List<DistributionMapUI> data = null;

                using (StreamReader sr = dialog.File.OpenText())
                using (Newtonsoft.Json.JsonReader reader = new Newtonsoft.Json.JsonTextReader(sr))
                {
                    Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                    serializer.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                    serializer.Converters.Add(new Newtonsoft.Json.Converters.IsoDateTimeConverter());
                    serializer.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                    serializer.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore;
                    serializer.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;
                    serializer.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    data = (List<DistributionMapUI>)serializer.Deserialize(reader, typeof(List<DistributionMapUI>));
                }
                if (data != null)
                {
                    BindMap(new ResultArgs<List<DistributionMapUI>>(data, null));
                }

            }
            
        }
        

        void OnLoaded(object sender, RoutedEventArgs arg)
        {
            this.map.CredentialsProvider = new ApplicationIdCredentialsProvider(App.MapKey);

            proxy = new LocationService.LocationServiceClient();

            //set proxy address
            proxy.Endpoint.Address = new EndpointAddress(HttpClient.CombineUrl("/SilverlightServices/LocationService.svc"));
#if DEBUG
            //silverlight project is another solution for gps.site. for debug please change the LocationService address here
            proxy.Endpoint.Address = new EndpointAddress("http://www.timm.com/SilverlightServices/LocationService.svc");
#endif
            proxy.SendMailCompleted += (s, e) =>
            {

            };

            taskTimer = new DispatcherTimer()
            {
                Interval = new TimeSpan(2, 0, 0) // 2 hours
            };
            taskTimer.Tick += new EventHandler(OnTaskTimer_Tick);

            //snapDelay = new DispatcherTimer()
            //{
            //    Interval = new TimeSpan(0,0,5)
            //};
            //snapDelay.Tick += OnSnapDelay_Tick;

            //this.taskTimer.Start();
            this.GetActivedDmap();
        }

        #region use proxy to build map image

        private void BuildMapImage(object sender, DoWorkEventArgs e)
        {
            MapImageCache cache = e.Argument as MapImageCache;
            //foreach (var query in cache.ImageQuery)
            //{
            //    Interlocked.Increment(ref m_ThreadCount);                
            //    ThreadPool.QueueUserWorkItem(DownloadMapImage, query);
            //}
            ThreadPool.QueueUserWorkItem(DownloadMapImage, cache);
            m_AutoReset.WaitOne();
            e.Result = cache;
        }
        private const string BasePath = "DistributionMapSnap";
        private const int MapSplitWidth = 320;
        private const int MapSplitHeight = 320;
        private const int MapOverSize = 30;
        private const double ZoomRate = 2d;//the bing map control scale rate, no scale is 1
        private BackgroundWorker m_MapImageWorker = new BackgroundWorker();
        
        private AutoResetEvent m_AutoReset = new AutoResetEvent(false);
        private int m_ThreadCount = 0;
        private readonly string BaseURL;
        private double printImageWidth;
        private double printImageHeight;

        private class MapImageCache
        {
            public Size RenderSize;
            public string ImageQuery = string.Empty;
            public string FileName = string.Empty;
            public WriteableBitmap BackgroudMapImage { get; set; }
        }

        private MapImageCache PrepareMapImageCache()
        {
            #region Old Method
            //var imageQuery = new List<string>();
            //for (var i = 0; i < Math.Ceiling(map.RenderSize.Height / ZoomRate / MapSplitHeight); i++)
            //{
            //    for (var j = 0; j < Math.Ceiling(map.RenderSize.Width / ZoomRate / MapSplitWidth); j++)
            //    {
            //        //move center y position +15px for remove the bing logo image and copyright down 30px
            //        Map.Location leftBottomLocation, rightTopLocation;
            //        Point leftBottomPoint = new Point(j * MapSplitWidth, (i + 1) * MapSplitHeight);
            //        Point rigthTopPoint = new Point((j + 1) * MapSplitWidth, i * MapSplitHeight);

            //        //reduce Y value to remove bing map copyright 
            //        rigthTopPoint.Y -= MapOverSize;

            //        map.TryViewportPointToLocation(leftBottomPoint, out leftBottomLocation);
            //        map.TryViewportPointToLocation(rigthTopPoint, out rightTopLocation);

            //        #region add by steve.j.yin on 2013/09/11 for fix zoom level error
            //        double centerX = (rigthTopPoint.X - leftBottomPoint.X) / 2d + leftBottomPoint.X;
            //        double centerY = (leftBottomPoint.Y - rigthTopPoint.Y) / 2d + rigthTopPoint.Y;
            //        Point center = new Point(centerX, centerY);
            //        Map.Location centerLocation;
            //        map.TryViewportPointToLocation(center, out centerLocation);
            //        #endregion

            //        string mode = "Road";
            //        switch (map.Mode.ToString())
            //        {
            //            case "Microsoft.Maps.MapControl.RoadMode":
            //                mode = "Road";
            //                break;
            //            case "Microsoft.Maps.MapControl.AerialMode":
            //                mode = "AerialWithLabels";
            //                break;
            //            default:
            //                mode = "Road";
            //                break;

            //        }
            //        //string query = string.Format("mode={0}&size={1},{2}&mapArea={3},{4},{5},{6}",
            //        //    mode,
            //        //    MapSplitWidth * ZoomRate,
            //        //    (MapSplitHeight * ZoomRate + MapOverSize),
            //        //    leftTopLocation.Latitude.ToString("#.000000000000"),
            //        //    leftTopLocation.Longitude.ToString("#.000000000000"),
            //        //    rightBottomLocation.Latitude.ToString("#.000000000000"),
            //        //    rightBottomLocation.Longitude.ToString("#.000000000000"));
            //        string query = string.Format("mode={0}&size={1},{2}&center={3},{4}&zoom={5}",
            //            mode,
            //            MapSplitWidth,
            //            MapSplitHeight + MapOverSize,
            //            centerLocation.Latitude.ToString("#.000000000000"),
            //            centerLocation.Longitude.ToString("#.000000000000"),
            //            map.ZoomLevel);
            //        imageQuery.Add(query);
            //    }
            //}
            #endregion
            var imageQuery = string.Empty;

            Map.Location leftBottomLocation, rightTopLocation;
            Point leftBottomPoint = new Point(0, map.RenderSize.Height);
            Point rigthTopPoint = new Point(map.RenderSize.Width, 0);
            map.TryViewportPointToLocation(leftBottomPoint, out leftBottomLocation);
            map.TryViewportPointToLocation(rigthTopPoint, out rightTopLocation);

            string mode = "Road";
            switch (map.Mode.ToString())
            {
                case "Microsoft.Maps.MapControl.RoadMode":
                    mode = "Road";
                    break;
                case "Microsoft.Maps.MapControl.AerialMode":
                    mode = "AerialWithLabels";
                    break;
                default:
                    mode = "Road";
                    break;

            }

            imageQuery = string.Format("zoom={0}&mode={1}&leftBottom={2},{3}&rightTop={4},{5}",
                (int)map.ZoomLevel,
                mode,
                leftBottomLocation.Latitude,
                leftBottomLocation.Longitude,
                rightTopLocation.Latitude,
                rightTopLocation.Longitude);

            var fileName = string.Format("{0},{1};{2},{3}",
                leftBottomLocation.Latitude,
                leftBottomLocation.Longitude,
                rightTopLocation.Latitude,
                rightTopLocation.Longitude);

            return new MapImageCache { ImageQuery = imageQuery, FileName = fileName, RenderSize = map.RenderSize, BackgroudMapImage = null };
        }

        private void DownloadMapImage(object state)
        {
            #region Old download image method
            //string query = state.ToString();
            //int threadId = Thread.CurrentThread.ManagedThreadId;
            //WebClient client = new WebClient();
            //string url = string.Format("{0}?{1}", BaseURL, query);
            //client.OpenReadCompleted += (s, a) =>
            //{
            //    try
            //    {
            //        using (var isoFile = IsolatedStorageFile.GetUserStoreForApplication())
            //        {
            //            if (!isoFile.DirectoryExists(BasePath))
            //            {
            //                isoFile.CreateDirectory(BasePath);
            //            }
            //            var queryFile = System.IO.Path.Combine(BasePath, query);
            //            if (isoFile.FileExists(queryFile))
            //            {
            //                isoFile.DeleteFile(queryFile);
            //            }

            //            using (var fileStream = isoFile.CreateFile(queryFile))
            //            {
            //                byte[] buffer = new byte[1024];
            //                int readed = 0;
            //                do
            //                {
            //                    readed = a.Result.Read(buffer, 0, buffer.Length);
            //                    fileStream.Write(buffer, 0, readed);
            //                } while (readed > 0);
            //                fileStream.Flush();
            //                fileStream.Close();
            //            }

            //        }
            //    }
            //    catch
            //    {

            //    }
            //    Interlocked.Decrement(ref m_ThreadCount);
            //    if (m_ThreadCount <= 0)
            //    {
            //        m_AutoReset.Set();
            //    }
            //};
            //var target = new Uri(url, UriKind.Absolute);
            //client.OpenReadAsync(target);
            #endregion

            var info = state as MapImageCache;
            if (info == null || string.IsNullOrWhiteSpace(info.ImageQuery) || string.IsNullOrWhiteSpace(info.FileName))
            {
                m_AutoReset.Set();
            }

            WebClient client = new WebClient();
            string url = string.Format("{0}?{1}", BaseURL, info.ImageQuery);
            client.OpenReadCompleted += (s, a) =>
            {
                try
                {
                    using (var isoFile = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        if (!isoFile.DirectoryExists(BasePath))
                        {
                            isoFile.CreateDirectory(BasePath);
                        }
                        var queryFile = System.IO.Path.Combine(BasePath, info.FileName);
                        if (isoFile.FileExists(queryFile))
                        {
                            isoFile.DeleteFile(queryFile);
                        }

                        using (var fileStream = isoFile.CreateFile(queryFile))
                        {
                            byte[] buffer = new byte[1024];
                            int readed = 0;
                            do
                            {
                                readed = a.Result.Read(buffer, 0, buffer.Length);
                                fileStream.Write(buffer, 0, readed);
                            } while (readed > 0);
                            fileStream.Flush();
                            fileStream.Close();
                        }
                    }
                }
                finally
                {
                    m_AutoReset.Set();
                }
            };
            client.OpenReadAsync(new Uri(url, UriKind.Absolute));
        }

        private void GeneratePrintImageCache(MapImageCache info)
        {
            if (!string.IsNullOrEmpty(info.ImageQuery) && !string.IsNullOrWhiteSpace(info.FileName))
            {
                WriteableBitmap mapImage = new WriteableBitmap((int)info.RenderSize.Width, (int)info.RenderSize.Height);
                using (var isoFile = IsolatedStorageFile.GetUserStoreForApplication())
                using (var file = isoFile.OpenFile(System.IO.Path.Combine(BasePath, info.FileName), FileMode.Open))
                {
                    BitmapImage mpaImageSource = new BitmapImage();
                    mpaImageSource.SetSource(file);

                    Image backgroundImage = new Image()
                    {
                        Width = info.RenderSize.Width,
                        Height = info.RenderSize.Height,
                        Stretch = Stretch.Fill,
                        Source = mpaImageSource
                    };

                    mapImage.Render(backgroundImage, null);
                    mapImage.Invalidate();

                    file.Close();

                    ScaleTransform transform = new ScaleTransform();
                    transform.ScaleX = ZoomRate;
                    transform.ScaleY = ZoomRate;
                    foreach (var item in map.Children)
                    {
                        if (item.Visibility == Visibility.Visible)
                        {
                            mapImage.Render(item, transform);
                            mapImage.Invalidate();
                        }

                    }

                    info.BackgroudMapImage = mapImage;

                    try
                    {
                        isoFile.DeleteFile(System.IO.Path.Combine(BasePath, info.FileName));
                    }
                    catch { }
                }
            }
            #region Old method
            //if (info.ImageQuery != null && info.ImageQuery.Count > 0)
            //{
            //    WriteableBitmap mapImage = new WriteableBitmap((int)info.RenderSize.Width, (int)info.RenderSize.Height);

            //    using (var isoFile = IsolatedStorageFile.GetUserStoreForApplication())
            //    {
            //        int column = (int)Math.Ceiling(info.RenderSize.Width / ZoomRate / MapSplitWidth);
            //        Canvas background = new Canvas();
            //        background.Width = info.RenderSize.Width;
            //        background.Height = info.RenderSize.Height;
            //        for (int i = 0; i < Math.Ceiling(info.RenderSize.Height / ZoomRate / MapSplitHeight); i++)
            //        {
            //            for (int j = 0; j < Math.Ceiling(info.RenderSize.Width / ZoomRate / MapSplitWidth); j++)
            //            {
            //                var index = i * column + j;
            //                using (var file = isoFile.OpenFile(System.IO.Path.Combine(BasePath, info.ImageQuery[index]), FileMode.Open))
            //                {
            //                    BitmapImage tileImageSource = new BitmapImage();
            //                    tileImageSource.SetSource(file);
            //                    Image tileImage = new Image();
            //                    tileImage.Source = tileImageSource;
            //                    tileImage.Width = MapSplitWidth * ZoomRate;
            //                    tileImage.Height = MapSplitHeight * ZoomRate + MapOverSize;
            //                    tileImage.Stretch = Stretch.Fill;
            //                    tileImage.SetValue(Canvas.LeftProperty, (double)(j * MapSplitWidth * ZoomRate));
            //                    tileImage.SetValue(Canvas.TopProperty, (double)(i * MapSplitHeight * ZoomRate));
            //                    background.Children.Add(tileImage);
            //                    file.Close();
            //                }
            //            }
            //        }

            //        mapImage.Render(background, null);
            //        mapImage.Invalidate();

            //        //render address and submaps
            //        ScaleTransform transform = new ScaleTransform();
            //        transform.ScaleX = ZoomRate;
            //        transform.ScaleY = ZoomRate;
            //        foreach (var item in map.Children)
            //        {
            //            if (item.Visibility == Visibility.Visible)
            //            {
            //                mapImage.Render(item, transform);
            //                mapImage.Invalidate();
            //            }

            //        }

            //        info.BackgroudMapImage = mapImage;

            //        //clear tile image
            //        info.ImageQuery.ForEach(i =>
            //        {
            //            isoFile.DeleteFile(System.IO.Path.Combine(BasePath, i));
            //        });
            //    }
            //}
            #endregion
        }

        private byte[] EncodeToBMP(WriteableBitmap bitmap)
        {
            using (MemoryStream imageStream = new MemoryStream())
            {
                //get data from bitmap
                byte[] bmpPixels = new byte[bitmap.PixelWidth * bitmap.PixelHeight * 4];
                int[] raster = bitmap.Pixels;
                if (raster != null)
                {
                    for (int y = 0; y < bitmap.PixelHeight; ++y)
                    {
                        for (int x = 0; x < bitmap.PixelWidth; ++x)
                        {
                            int pixelIndex = bitmap.PixelWidth * y + x;
                            int pixel = raster[pixelIndex];

                            byte a = (byte)((pixel >> 24) & 0xFF);

                            float aFactor = a / 255f;

                            if (aFactor > 0)
                            {
                                byte r = (byte)(((pixel >> 16) & 0xFF) / aFactor);
                                byte g = (byte)(((pixel >> 8) & 0xFF) / aFactor);
                                byte b = (byte)(((pixel >> 0) & 0xFF) / aFactor);

                                bmpPixels[pixelIndex * 4 + 0] = b;
                                bmpPixels[pixelIndex * 4 + 1] = g;
                                bmpPixels[pixelIndex * 4 + 2] = r;
                                bmpPixels[pixelIndex * 4 + 3] = 0x00;
                            }
                        }
                    }
                }




                #region BMP File Header(14 bytes)
                //the magic number(2 bytes):BM
                imageStream.WriteByte(0x42);
                imageStream.WriteByte(0x4D);

                //the size of the BMP file in bytes(4 bytes)
                long len = bmpPixels.Length * 4 + 0x36;
                int width = bitmap.PixelWidth;
                int height = bitmap.PixelHeight;


                imageStream.WriteByte((byte)len);
                imageStream.WriteByte((byte)(len >> 8));
                imageStream.WriteByte((byte)(len >> 16));
                imageStream.WriteByte((byte)(len >> 24));

                //reserved(2 bytes)
                imageStream.WriteByte(0x00);
                imageStream.WriteByte(0x00);

                //reserved(2 bytes)
                imageStream.WriteByte(0x00);
                imageStream.WriteByte(0x00);

                //the offset(4 bytes)
                imageStream.WriteByte(0x36);
                imageStream.WriteByte(0x00);
                imageStream.WriteByte(0x00);
                imageStream.WriteByte(0x00);
                #endregion

                #region Bitmap Information(40 bytes:Windows V3)
                //the size of this header(4 bytes)
                imageStream.WriteByte(0x28);
                imageStream.WriteByte(0x00);
                imageStream.WriteByte(0x00);
                imageStream.WriteByte(0x00);

                //the bitmap width in pixels(4 bytes)
                imageStream.WriteByte((byte)width);
                imageStream.WriteByte((byte)(width >> 8));
                imageStream.WriteByte((byte)(width >> 16));
                imageStream.WriteByte((byte)(width >> 24));

                //the bitmap height in pixels(4 bytes)
                imageStream.WriteByte((byte)height);
                imageStream.WriteByte((byte)(height >> 8));
                imageStream.WriteByte((byte)(height >> 16));
                imageStream.WriteByte((byte)(height >> 24));

                //the number of color planes(2 bytes)
                imageStream.WriteByte(0x01);
                imageStream.WriteByte(0x00);

                //the number of bits per pixel(2 bytes)
                imageStream.WriteByte(0x20);
                imageStream.WriteByte(0x00);

                //the compression method(4 bytes)
                imageStream.WriteByte(0x00);
                imageStream.WriteByte(0x00);
                imageStream.WriteByte(0x00);
                imageStream.WriteByte(0x00);

                //the image size(4 bytes)
                imageStream.WriteByte(0x00);
                imageStream.WriteByte(0x00);
                imageStream.WriteByte(0x00);
                imageStream.WriteByte(0x00);

                //the horizontal resolution of the image(4 bytes)
                imageStream.WriteByte(0x00);
                imageStream.WriteByte(0x00);
                imageStream.WriteByte(0x00);
                imageStream.WriteByte(0x00);

                //the vertical resolution of the image(4 bytes)
                imageStream.WriteByte(0x00);
                imageStream.WriteByte(0x00);
                imageStream.WriteByte(0x00);
                imageStream.WriteByte(0x00);

                //the number of colors in the color palette(4 bytes)
                imageStream.WriteByte(0x00);
                imageStream.WriteByte(0x00);
                imageStream.WriteByte(0x00);
                imageStream.WriteByte(0x00);

                //the number of important colors(4 bytes)
                imageStream.WriteByte(0x00);
                imageStream.WriteByte(0x00);
                imageStream.WriteByte(0x00);
                imageStream.WriteByte(0x00);
                #endregion

                #region Bitmap data
                byte[] buffer = new byte[width * 4];
                for (int y = height - 1; y >= 0; y--)
                {
                    Array.Copy(bmpPixels, width * y * 4, buffer, 0, width * 4);
                    imageStream.Write(buffer, 0, buffer.Length);
                }
                #endregion
                imageStream.Seek(0, SeekOrigin.Begin);
                byte[] result = new byte[imageStream.Length];
                imageStream.Read(result, 0, (int)imageStream.Length);
                return result;
            }
        }
#endregion

        void SendMapSnap(object sender, RunWorkerCompletedEventArgs e)
        {
            //snapDelay.Stop(); // Just delay once.

            //string filePath = string.Format(@"C:\DMap\{0}.png", currentMapinfo.Value.Id);
            //using (var fileStream = File.Open(filePath, FileMode.OpenOrCreate))
            //{
            //    this.map.ToImage().WriteToStream(fileStream);
            //}

            var cache = e.Result as MapImageCache;
            GeneratePrintImageCache(cache);


            var data = EncodeToBMP(cache.BackgroudMapImage);

            this.dmapLayers[currentMapinfo.Value.Id].Visibility = System.Windows.Visibility.Collapsed;

            var task = currentMapinfo.Value.DMap.Tasks.FirstOrDefault();

            if (task != null)
            {

                // Send mail
                proxy.SendMailAsync(
                    task.Email,
                    string.Format("{0} - {1}", task.Date, task.Name),
                    "See the attachments.",
                    currentMapinfo.Value.DMap.Id.ToString() + ".png",
                    data);
            }
            else
            {
#if DEBUG
                MessageBox.Show(currentMapinfo.Value.DMap.Id.ToString());
#endif
            }

            //HttpClient.Get<string>(string.Format("/service/campaign/sendmail/{0}", currentMapinfo.Value.DMap.Id), null, false, false);

            if (currentMapinfo.Next != null)
            {
                currentMapinfo = currentMapinfo.Next;

                this.dmapLayers[currentMapinfo.Value.Id].Visibility = System.Windows.Visibility.Visible;

                //this.map.ViewChangeEnd -= OnMap_ViewChangeEnd;
                //this.map.ViewChangeEnd += OnMap_ViewChangeEnd;

                this.map.SetView(currentMapinfo.Value.BestView);
            }
            else
            {
                this.map.ViewChangeEnd -= OnMap_ViewChangeEnd;
                taskTimer.Start();
            }
        }

        void OnTaskTimer_Tick(object sender, EventArgs e)
        {
            GetActivedDmap();
        }

        void GetActivedDmap()
        {            
            HttpClient.Get<List<DistributionMapUI>>("/service/campaign/activedmap/", BindMap, false, false);
        }

        void OnMap_ViewChangeEnd(object sender, MapEventArgs e)
        {
            //snapDelay.Start();
            m_MapImageWorker.RunWorkerAsync(PrepareMapImageCache());
        }

        //private void StartButton_Click(object sender, RoutedEventArgs e)
        //{
        //    this.taskTimer.Start();

        //    GetActivedDmap();

        //    this.PauseButton.IsEnabled = true;
        //    this.StartButton.IsEnabled = false;
        //}

        //private void PauseButton_Click(object sender, RoutedEventArgs e)
        //{
        //    this.taskTimer.Stop();

        //    this.PauseButton.IsEnabled = false;
        //    this.StartButton.IsEnabled = true;
        //}

        #region Draw Distributor Map

        public int m_GTURadii = 50;

        /// <summary>
        /// the state of Gtu dot
        /// </summary>
        enum GtuState
        {
            Origin = 0,    // gtu creates
            Added = 1,  // user click on the blank
            Removed = 2 // delete Raw gtu 
        }

        private LinkedList<TIMM.GPS.ControlCenter.Views.ReportPrint.MapImageInfo> m_MapImageList =
            new LinkedList<TIMM.GPS.ControlCenter.Views.ReportPrint.MapImageInfo>();

        LinkedListNode<TIMM.GPS.ControlCenter.Views.ReportPrint.MapImageInfo> currentMapinfo;

        Dictionary<int, MapLayer> dmapLayers= new Dictionary<int, MapLayer>();

        protected void BindMap(ResultArgs<List<DistributionMapUI>> result)
        {
            var dmaps = result.Data;

            if (dmaps.Count == 0) return; 

            this.map.Children.Clear(); 
            dmapLayers.Clear();
            m_MapImageList.Clear();

            foreach (var dmap in dmaps)
            {
                if (string.IsNullOrEmpty(dmap.Tasks.FirstOrDefault().Email))
                {
                    continue; // never draw when invalidated mail address 
                }

                var dmapLayer = new Map.MapLayer()
                {
                    Visibility = System.Windows.Visibility.Collapsed
                };

                this.map.Children.Add(dmapLayer);

                dmapLayers.Add(dmap.Id, dmapLayer);

                #region Boundary

                var dmapLocations = new Map.LocationCollection();
                dmap.Locations.ForEach(i =>
                {
                    dmapLocations.Add(new Map.Location { Longitude = i.Longitude, Latitude = i.Latitude });
                });
                MapPolygon dmapPolygon = new MapPolygon
                {
                    Tag = dmap.Id,
                    Locations = dmapLocations,
                    StrokeLineJoin = PenLineJoin.Round,
                    StrokeThickness = 5d,
                    Stroke = new SolidColorBrush(Colors.Black),
                    Fill = new SolidColorBrush(ColorHelper.FromRgbString(0x59, dmap.ColorString))
                };
                dmapLayer.Children.Add(dmapPolygon);

                #endregion

                #region never show Gtu dot

                foreach (var gtu in dmap.GtuInfo)
                {
                    MapPolygon gtuPolygon = new MapPolygon
                    {
                        Tag = string.Format("GTU{0}", dmap.Id),
                        Fill = new SolidColorBrush(ColorHelper.FromRgbString(0xBF, gtu.UserColor)),
                        Stroke = new SolidColorBrush(Colors.Black),
                        StrokeThickness = 1,
                        StrokeLineJoin = PenLineJoin.Round,
                        Locations = GeoCodeCalculation.CreatePoint(
                            (double)m_GTURadii / 2500d,
                            new Map.Location
                            {
                                Latitude = gtu.Latitude,
                                Longitude = gtu.Longitude
                            })
                    };
                    dmapLayer.Children.Add(gtuPolygon);
                }


                #endregion

                #region NdAddress

                dmap.NdAddress.ForEach(ndaddress =>
                {
                    var ndLocations = new LocationCollection();
                    ndaddress.Locations.ForEach(i =>
                    {
                        ndLocations.Add(new Map.Location { Longitude = i.Longitude, Latitude = i.Latitude });
                    });
                    MapPolygon ndAddressPolygon = new MapPolygon
                    {
                        Tag = "DMapNdAddress",
                        Fill = new SolidColorBrush(Color.FromArgb(0xBF, 0xFF, 0x00, 0x00)),
                        Stroke = new SolidColorBrush(Colors.Red),
                        StrokeThickness = 1,
                        StrokeLineJoin = PenLineJoin.Round,
                        Locations = ndLocations
                    };
                    dmapLayer.Children.Add(ndAddressPolygon);
                });

                #endregion

                // Location dmap
                var mapInfo = new TIMM.GPS.ControlCenter.Views.ReportPrint.MapImageInfo(dmap.Id, dmap);

                m_MapImageList.AddLast(mapInfo);
            }

            if (m_MapImageList.Count > 0)
            {
                this.map.ViewChangeEnd -= OnMap_ViewChangeEnd;
                this.map.ViewChangeEnd += OnMap_ViewChangeEnd;

                currentMapinfo = m_MapImageList.First;

                this.dmapLayers[currentMapinfo.Value.Id].Visibility = System.Windows.Visibility.Visible;
                this.map.SetView(currentMapinfo.Value.BestView);
            }
        }

        #endregion

        
    }
}
