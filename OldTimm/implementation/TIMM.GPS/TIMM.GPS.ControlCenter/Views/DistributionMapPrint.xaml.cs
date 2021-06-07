using Microsoft.Maps.MapControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Printing;
using Map = Microsoft.Maps.MapControl;

using TIMM.GPS.ControlCenter.Extend;
using TIMM.GPS.ControlCenter.Interface;
using TIMM.GPS.ControlCenter.Model;
using TIMM.GPS.ControlCenter.UserControls.Print;
using TIMM.GPS.Net.Http;
using TIMM.GPS.ControlCenter.Extends;



namespace TIMM.GPS.ControlCenter.Views
{
    public partial class DistributionMapPrint : Page
    {
        #region Private field and property
        private const double MapSplitWidth = 320d;
        private const double MapSplitHeight = 300d;
        private const double MapOverSize = 30d;
        private const double ZoomRate = 2d;

        private List<Map.LocationCollection> m_NdAddress { get; set; }
        private LinkedListNode<MapImageInfo> m_MapImage;
        private LinkedList<MapImageInfo> m_MapImageList = new LinkedList<MapImageInfo>();
        private AutoResetEvent m_AutoReset = new AutoResetEvent(false);
        private int m_ThreadCount = 0;
        private BackgroundWorker m_Worker = new BackgroundWorker();
        private BackgroundWorker m_EditMapWorker = new BackgroundWorker();
        private readonly string BaseURL;
        private double printImageWidth;
        private double printImageHeight;

        private CampaignUI m_Campaign { get; set; }
        private ICanEditMapPdfPage m_CurrentEditPage;
        #endregion


        public DistributionMapPrint()
        {
            InitializeComponent();

            this.mapMain.CredentialsProvider = new ApplicationIdCredentialsProvider(App.MapKey);

            printImageWidth = mapMain.Width;
            printImageHeight = mapMain.Height;
            BaseURL = string.Format("http://{0}:{1}{2}",
                HtmlPage.Document.DocumentUri.DnsSafeHost,
                HtmlPage.Document.DocumentUri.Port,
                "/MapProxy/MapImageServer.ashx");
            m_Worker.RunWorkerCompleted += OnWorkCompleted;
            m_Worker.DoWork += OnDoWork;

            m_EditMapWorker.DoWork += OnDoWork;
            m_EditMapWorker.RunWorkerCompleted += OnEditMapCompleted;

            PageStoryboard.Begin();

            this.SizeChanged += OnSizeChanged;

            this.DataContext = this;



            //popMap.IsOpen = true;
        }



        #region Page event
        protected void Storyboard_Completed(object sender, EventArgs e)
        {
            if (!HtmlPage.Document.QueryString.ContainsKey("id"))
            {
            	return;
            }
            string id = HtmlPage.Document.QueryString["id"];
            int campaignId;
            if (int.TryParse(id, out campaignId))
            {
                popMap.IsOpen = false;
                StartPage.Visibility = Visibility.Visible;
            }
        }

        protected void OnStart(object sender, RoutedEventArgs e)
        {
            using (var isoFile = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isoFile.Quota < 1L * 1024L * 1024L * 1024L)
                {
                    isoFile.IncreaseQuotaTo(2L * 1024L * 1024L * 1024L);
                }
            }
            popMap.IsOpen = true;
            StartPage.Visibility = System.Windows.Visibility.Collapsed;

            var id = HtmlPage.Document.QueryString["id"];
            int campaignId;
            if (int.TryParse(id, out campaignId))
            {
                LoadPage(campaignId);
            }
        }

        protected void OnPrintFile(object sender, RoutedEventArgs e)
        {
            PrintDocument pd = new PrintDocument();
            Queue<int> printQueue = new Queue<int>();
            for (int i = 0; i < PrintArea.Children.Count; i++)
            {
            	if (PrintArea.Children[i].Visibility == Visibility.Visible)
                {
                	printQueue.Enqueue(i);
                }
            }
            pd.EndPrint += (s, a) =>
            {
                for (int i = 0; i < PrintArea.Children.Count; i++)
                {
                    PrintArea.Children[i].RenderTransform = null;
                    PrintArea.Children[i].UpdateLayout();
                }
            };
            pd.PrintPage += (s, a) => 
            {
                if (printQueue.Count > 0)
                {
                    double width = a.PrintableArea.Width;
                    double height = a.PrintableArea.Height;
                    
                    int index = printQueue.Dequeue();
                    var page = PrintArea.Children[index] as UserControl;

                    if(page != null)
                    {
                        double scaleX = width / page.Width;
                        double scaleY = height / page.Height;
                        TransformGroup transformGroup = new TransformGroup();
                        transformGroup.Children.Add(new ScaleTransform 
                        {
                            ScaleX = scaleX,
                            ScaleY = scaleY
                        });
                        
                        a.PageVisual = page;
                        a.PageVisual.RenderTransform = transformGroup;
                        a.PageVisual.UpdateLayout();

                        a.HasMorePages = printQueue.Count > 0 ? true : false;
                    }
                }
            };

            pd.Print(m_Campaign.DisplayName, new PrinterFallbackSettings{ ForceVector = true});
        }

        protected void OnZoomIn(object sender, RoutedEventArgs e)
        {
            foreach (var child in PrintArea.Children)
            {
                if (child is PdfPageBase)
                {
                    PdfPageBase page = child as PdfPageBase;
                    page.Scale(2d);
                }
            }
        }

        protected void OnZoomOut(object sender, RoutedEventArgs e)
        {
            foreach (var child in PrintArea.Children)
            {
                if (child is PdfPageBase)
                {
                    PdfPageBase page = child as PdfPageBase;
                    page.Scale(2d);
                }
            }
        }        

        protected void OnClosePopup(object sender, RoutedEventArgs e)
        {
            popMap.IsOpen = false;
            m_CurrentEditPage = null;
        }

        protected void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(popMap.IsOpen == true && Loading.IsBusy == false)
            {
                OnShowPopup(sender, new EventArgs());
            }
        }

        protected void OnShowPopup(object sender, EventArgs e)
        {
            popMap.Width = App.Current.RootVisual.RenderSize.Width;
            popMap.Height = App.Current.RootVisual.RenderSize.Height;
            popBg.Width = popMap.Width;
            popBg.Height = popMap.Height;

            popMap.UpdateLayout();

            double mapLeft = (popMap.Width - mapMain.Width) / 2d;
            double mapTop = (popMap.Height - mapMain.Height) / 2d;
            mapMain.SetValue(Canvas.LeftProperty, mapLeft);
            mapMain.SetValue(Canvas.TopProperty, mapTop);

            double buttonLeft = Math.Min(mapLeft + mapMain.Width, popMap.Width);
            double buttonTop = Math.Max((popMap.Height - mapMain.Height) / 2d, 0);
            btnApply.SetValue(Canvas.LeftProperty, buttonLeft - 264d);
            btnApply.SetValue(Canvas.TopProperty, buttonTop + 32d);
            btnCancel.SetValue(Canvas.LeftProperty, buttonLeft - 132d);
            btnCancel.SetValue(Canvas.TopProperty, buttonTop + 32d);

            double navLeft = popMap.Width / 2d - navPanel.RenderSize.Width / 2d;
            double navTop = Math.Min((popMap.Height + mapMain.Height) / 2d, popMap.Height) - navPanel.RenderSize.Height - 40d;

            navPanel.SetValue(Canvas.LeftProperty, navLeft);
            navPanel.SetValue(Canvas.TopProperty, navTop);

            

            
        }

        protected void OnEditMap(object sender, RoutedEventArgs e)
        {
            if (sender is ICanEditMapPdfPage)
            {
                var page = sender as ICanEditMapPdfPage;
                mapMain.SetView(page.GetCenter(), page.GetZoomLevel());
                m_CurrentEditPage = page;
                SwithMapLyaerDisplay(page.GetId());

                popMap.IsOpen = true;
            }

        }

        protected void OnChangeMap(object sender, RoutedEventArgs e)
        {
            if (m_CurrentEditPage != null)
            {
                Loading.IsBusy = true;
                var enumerator = m_MapImageList.GetEnumerator();
                m_MapImage = m_MapImageList.First;
                int id = m_CurrentEditPage.GetId();
                while(enumerator.MoveNext())
                {                    
                    if (m_MapImage.Value.Id == id)
                    {
                        m_MapImage.Value.BackgroudMapImage = null;
                        m_MapImage.Value.Center = mapMain.Center;
                        m_MapImage.Value.ZoomLevel = mapMain.ZoomLevel;

                        m_EditMapWorker.RunWorkerAsync(m_MapImage.Value);

                        break;
                    }
                    m_MapImage = m_MapImage.Next;
                }
            }
        }

        protected void mapMain_ViewChangeStart(object sender, MapEventArgs e)
        {
            //dispaly edit apply button whe map is changing
            btnApply.IsEnabled = false;
        }

        protected void mapMain_ViewChangeEnd(object sender, MapEventArgs e)
        {
            //dispaly edit apply button whe map is changing

            btnApply.IsEnabled = true;
        }

        protected void OnNavSwitchToAerial(object sender, RoutedEventArgs e)
        {
            mapMain.Mode = new AerialMode(true);
            btnAerial.IsActive = true;
        }

        protected void OnNavSwitchToRoad(object sender, RoutedEventArgs e)
        {
            mapMain.Mode = new RoadMode();
            btnAerial.IsActive = true;
        }

        protected void OnNavZoomIn(object sender, RoutedEventArgs e)
        {
            mapMain.ZoomLevel += 1;
        }

        protected void OnNavZoomOut(object sender, RoutedEventArgs e)
        {
            mapMain.ZoomLevel -= 1;
        }

        #endregion

        #region Private Method
        protected virtual void LoadPage(int id)
        {
            Loading.IsBusy = true;

            HttpClient.Get<CampaignUI>(string.Format("/service/campaign/withdmap/{0}", id), BindMap, false, false);
        }

        protected void BindMap(ResultArgs<CampaignUI> result)
        {
            if (result == null || result.Data == null)
            {
                Loading.IsBusy = false;
                popMap.IsOpen = false;
                return;
            }
            CampaignUI campaign = result.Data;
            if (campaign.DistributionMap == null || campaign.DistributionMap.Count == 0)
            {
                Loading.IsBusy = false;
                popMap.IsOpen = false;
                return;
            }
            m_Campaign = result.Data;
            m_MapImageList.Clear();
            
            campaign.DistributionMap.ForEach(i =>
            {
                i.Campaign = campaign;
                var distributionMapInfo = new MapImageInfo(i.Id, i, mapMain.Width * ZoomRate, mapMain.Height * ZoomRate);
                m_MapImageList.AddLast(distributionMapInfo);
            });
            m_MapImage = m_MapImageList.First;

            InitMap();

            StarDrawAllMap();
        }

        private void StarDrawAllMap()
        {
            m_MapImage = m_MapImageList.First;
            if (m_Campaign == null || m_MapImage == null)
            {
                return;
            }
            Loading.IsBusy = true;
            popMap.IsOpen = true;
            mapMain.ViewChangeEnd += OnMapLoadedCompleted;
            
            //only display campaign map layer and address map layer
            SwithMapLyaerDisplay(m_MapImage.Value.Id);

            if (m_MapImage.Value.BestView != null)
            {
                mapMain.SetView(m_MapImage.Value.BestView);
            }
            else
            {
                mapMain.SetView(m_MapImage.Value.Center, m_MapImage.Value.ZoomLevel);
            }
        }

        protected virtual void InitMap()
        {
            foreach(var item in m_Campaign.DistributionMap)
            {
                MapLayer dMapLayer = new MapLayer
                {
                    Tag = item.Id
                };
                //load distribution map area
                MapPolygon polyline = new MapPolygon() 
                { 
                    Stroke = new SolidColorBrush(ColorHelper.FromRgbString(0xBF, item.ColorString)), 
                    StrokeThickness = 2d,
                    Fill = new SolidColorBrush(Color.FromArgb(0x19, 0x00, 0x80, 0x00)),
                    StrokeLineJoin = PenLineJoin.Round,
                    Locations = new LocationCollection()
                };
                item.Locations.ForEach(i => 
                {
                    polyline.Locations.Add(new Map.Location { Latitude = i.Latitude, Longitude = i.Longitude });
                });
                dMapLayer.Children.Add(polyline);
                
                //load ndaddress
                int index = 1;
                foreach(var address in item.NdAddress)
                {
                    MapPolygon ndPolygon = new MapPolygon()
                    {
                        StrokeThickness = 1d,
                        Fill = new SolidColorBrush(Color.FromArgb(0x32, 0xBB, 0x00, 0x00)),
                        StrokeLineJoin = PenLineJoin.Round,
                        Locations = new LocationCollection(),
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center
                    };
                    address.NdAddressCoordinates.ForEach(i => {
                        ndPolygon.Locations.Add(new Location { Latitude = i.Latitude, Longitude = i.Longitude });
                    });
                    var serial = new TextBlock { 
                        Text = index.ToString(),
                        Foreground = new SolidColorBrush(Colors.Black),
                        FontWeight = FontWeights.Bold
                    };
                    address.Serial = index;
                    index++;
                    dMapLayer.Children.Add(ndPolygon);
                    dMapLayer.AddChild(serial, new Location(address.Latitude, address.Longitude), PositionOrigin.Center);
                }

                mapMain.Children.Add(dMapLayer);
            }

            #region Address
            MapLayer addressLayer = new MapLayer() { Name = "addressLayer" };
            foreach (var item in m_Campaign.Addresses)
            {
                // red star pushpin
                var location = new Map.Location { Latitude = item.Latitude, Longitude = item.Longitude };
                var position = PositionOrigin.Center;
                Image flag;
                if (string.Compare(item.Color, "green") == 0)
                {
                    flag = new Image { Source = ResourceHelper.GetBitmap("/Images/green-star.png") };
                }
                else
                {
                    flag = new Image { Source = ResourceHelper.GetBitmap("/Images/red-star.png") };
                }
                //flag.Opacity = 0.75;
                flag.Stretch = Stretch.None;
                addressLayer.AddChild(flag, location, position);

                // draw radiouse
                if (item.Radiuses != null)
                {
                    foreach (var radiuse in item.Radiuses)
                    {
                        if (radiuse.IsDisplay == false)
                        {
                            continue;
                        }
                        var circle = new Map.MapPolyline()
                        {
                            Visibility = radiuse.IsDisplay == true ? Visibility.Visible : Visibility.Collapsed
                        };
                        circle.Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x00, 0x8B));
                        circle.StrokeThickness = 1d;
                        string labelText = string.Empty;
                        var cornerLabelLocation = new Map.LocationCollection();
                        switch (radiuse.LengthMeasuresId)
                        {
                            case 1:
                                circle.Locations = GeoCodeCalculation.CreateCircle(location, radiuse.Length, GeoCodeCalculation.DistanceMeasure.Miles);
                                cornerLabelLocation = GeoCodeCalculation.Create4CornerLocation(location, radiuse.Length + 0.3d, GeoCodeCalculation.DistanceMeasure.Miles);
                                labelText = string.Format("{0}M", radiuse.Length.ToString("#.00"));
                                break;
                            case 2:
                                circle.Locations = GeoCodeCalculation.CreateCircle(location, radiuse.Length, GeoCodeCalculation.DistanceMeasure.Kilometers);
                                cornerLabelLocation = GeoCodeCalculation.Create4CornerLocation(location, radiuse.Length + 0.3d, GeoCodeCalculation.DistanceMeasure.Kilometers);
                                labelText = string.Format("{0}K", radiuse.Length.ToString("#.00"));
                                break;
                            default:
                                circle.Locations = GeoCodeCalculation.CreateCircle(location, radiuse.Length, GeoCodeCalculation.DistanceMeasure.Miles);
                                cornerLabelLocation = GeoCodeCalculation.Create4CornerLocation(location, radiuse.Length + 0.3d, GeoCodeCalculation.DistanceMeasure.Miles);
                                labelText = string.Format("{0}M", radiuse.Length.ToString("#.00"));
                                break;
                        }
                        addressLayer.Children.Add(circle);
                        for (int i = 0; i < cornerLabelLocation.Count; i++)
                        {
                            var label = new TextBlock
                            {
                                Text = labelText,
                                Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x00, 0x8B)),
                                FontSize = 8,
                                Visibility = radiuse.IsDisplay == true ? Visibility.Visible : Visibility.Collapsed
                            };

                            addressLayer.AddChild(label, cornerLabelLocation[i], Map.PositionOrigin.Center);
                        }

                    }
                }
            }
            mapMain.Children.Add(addressLayer);
            #endregion
        }

        private void SwithMapLyaerDisplay(int id)
        {
            foreach (var item in mapMain.Children)
            {
                if (item is MapLayer)
                {
                    var map = item as MapLayer;
                    if (map.Tag != null)
                    {
                        if ((int)map.Tag == id)
                        {
                            map.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            map.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }
        }

        private void OnMapLoadedCompleted(object sender, MapEventArgs e)
        {
            if (m_MapImage != null)
            {
                
                m_MapImage.Value.Center = mapMain.Center;
                m_MapImage.Value.ZoomLevel = (int)mapMain.ZoomLevel;

                m_Worker.RunWorkerAsync(m_MapImage.Value);
            }
        }

        private void OnDoWork(object sender, DoWorkEventArgs e)
        {
            //var info = CurrentNode.Value;
            var info = e.Argument as MapImageInfo;
            //// backgroud map image is exsit then no need download again;
            //if (info.BackgroudMapImage != null && !string.IsNullOrWhiteSpace(info.ImageQuery))
            //{
            //    e.Result = info.ImageQuery;
            //    return;
            //}

            #region old get map image method
            //var imageQuery = new List<string>();
            //for (var i = 0; i < Math.Ceiling(info.RenderSize.Height / ZoomRate / MapSplitHeight); i++)
            //{
            //    for (var j = 0; j < Math.Ceiling(info.RenderSize.Width / ZoomRate / MapSplitWidth); j++)
            //    {
            //        //move center y position +15px for remove the bing logo image and copyright down 30px
            //        Map.Location leftBottomLocation, rightTopLocation;
            //        Point leftBottomPoint = new Point(j * MapSplitWidth, (i + 1) * MapSplitHeight);
            //        Point rigthTopPoint = new Point((j + 1) * MapSplitWidth, i * MapSplitHeight);
                    
            //        //reduce Y value to remove bing map copyright 
            //        rigthTopPoint.Y -= MapOverSize;

            //        mapMain.TryViewportPointToLocation(leftBottomPoint, out leftBottomLocation);
            //        mapMain.TryViewportPointToLocation(rigthTopPoint, out rightTopLocation);

            //        #region add by steve.j.yin on 2013/09/11 for fix zoom level error
            //        double centerX = (rigthTopPoint.X - leftBottomPoint.X) / 2d + leftBottomPoint.X;
            //        double centerY = (leftBottomPoint.Y - rigthTopPoint.Y) / 2d + rigthTopPoint.Y;
            //        Point center = new Point(centerX, centerY);
            //        Map.Location centerLocation;
            //        mapMain.TryViewportPointToLocation(center, out centerLocation);
            //        #endregion

            //        string mode = "Road";
            //        switch (mapMain.Mode.ToString())
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
            //        #region modify by steve.j.yin on 2013/09/11 for fix zoom level error
            //        //string query = string.Format("mode={0}&size={1},{2}&mapArea={3},{4},{5},{6}&zoom={7}",
            //        //    mode,
            //        //    MapSplitWidth * ZoomRate,
            //        //    (MapSplitHeight + MapOverSize) * ZoomRate,
            //        //    leftBottomLocation.Latitude.ToString("#.000000000000"),
            //        //    leftBottomLocation.Longitude.ToString("#.000000000000"),
            //        //    rightTopLocation.Latitude.ToString("#.000000000000"),
            //        //    rightTopLocation.Longitude.ToString("#.000000000000"),
            //        //    info.ZoomLevel.ToString("#"));
            //        string query = string.Format("mode={0}&size={1},{2}&center={3},{4}&zoom={5}",
            //            mode,
            //            MapSplitWidth * ZoomRate,
            //            MapSplitHeight * ZoomRate + MapOverSize,
            //            centerLocation.Latitude.ToString("#.000000000000"),
            //            centerLocation.Longitude.ToString("#.000000000000"),
            //            info.ZoomLevel);
            //        #endregion
            //        imageQuery.Add(query);
            //    }
            //}

            //foreach (var query in imageQuery)
            //{
            //    Interlocked.Increment(ref m_ThreadCount);
            //    ThreadPool.QueueUserWorkItem(DownloadMapImage, query);
            //}
            //m_AutoReset.WaitOne();
            //e.Result = imageQuery;
            #endregion

            Map.Location leftBottomLocation, rightTopLocation;
            Point leftBottomPoint = new Point(0, printImageHeight);
            Point rigthTopPoint = new Point(printImageWidth, 0);
            mapMain.TryViewportPointToLocation(leftBottomPoint, out leftBottomLocation);
            mapMain.TryViewportPointToLocation(rigthTopPoint, out rightTopLocation);

            string mode = "Road";
            switch (mapMain.Mode.ToString())
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

            info.ImageQuery = string.Format("zoom={0}&mode={1}&leftBottom={2},{3}&rightTop={4},{5}&dpi=192",
                (int)info.ZoomLevel,
                mode,
                leftBottomLocation.Latitude,
                leftBottomLocation.Longitude,
                rightTopLocation.Latitude,
                rightTopLocation.Longitude);

            info.FileName = string.Format("{0},{1};{2},{3}",
                leftBottomLocation.Latitude,
                leftBottomLocation.Longitude,
                rightTopLocation.Latitude,
                rightTopLocation.Longitude);
            

            ThreadPool.QueueUserWorkItem(DownloadMapImage, info);

            
            m_AutoReset.WaitOne();
        }

        private void OnWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            GeneratePrintImageCache(m_MapImage.Value);

            if (m_MapImage.Next != null)
            {
                m_MapImage = m_MapImage.Next;
                SwithMapLyaerDisplay(m_MapImage.Value.Id);
                if (m_MapImage.Value.BestView != null)
                {
                    mapMain.SetView(m_MapImage.Value.BestView);
                }
                else
                {
                    mapMain.SetView(m_MapImage.Value.Center, m_MapImage.Value.ZoomLevel);
                }
            }
            else
            {
                mapMain.ViewChangeEnd -= OnMapLoadedCompleted;
                popMap.IsOpen = false;

                if (PrintArea.Children.Count == 0)
                {
                    InitPreviewPdfPage();
                }
                else
                {
                    m_MapImage = m_MapImageList.First;
                    foreach (var item in PrintArea.Children)
                    {
                        if (!(item is ICanEditMapPdfPage))
                        {
                            continue;
                        }
                        (item as ICanEditMapPdfPage).ChangeImgae(m_MapImage.Value.BackgroudMapImage);
                        m_MapImage = m_MapImage.Next;
                    }
                    m_MapImage = m_MapImageList.First;
                }
                Loading.IsBusy = false;
            }
        }

        private void OnEditMapCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MapImageInfo info = m_MapImage.Value;
            GeneratePrintImageCache(info);
            m_CurrentEditPage.ChangeImgae(info.BackgroudMapImage);
            m_CurrentEditPage.SetCenter(mapMain.Center);
            m_CurrentEditPage.SetZoomLevel(mapMain.ZoomLevel);
            popMap.IsOpen = false;
            Loading.IsBusy = false;
            
        }

        private void InitPreviewPdfPage()
        {
            m_MapImage = m_MapImageList.First;
            var enumerator = m_MapImageList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var map = enumerator.Current;
                PrintBigDMap dmap = new PrintBigDMap(enumerator.Current.DMap, map.BackgroudMapImage);
                dmap.SetCenter(map.Center);
                dmap.SetZoomLevel(map.ZoomLevel);
                dmap.MouseLeftButtonUp += OnEditMap;

                //fit width
                //TransformGroup transform = new TransformGroup();
                //double scale = dmap.Width / this.RenderSize.Width;
                //transform.Children.Add(new ScaleTransform 
                //{
                //    ScaleX = scale,
                //    ScaleY = scale
                //});
                //dmap.RenderTransform = transform;

                PrintArea.Children.Add(dmap);
            }

            PrintArea.Visibility = System.Windows.Visibility.Visible;
            btnPrint.IsEnabled = true;
        }

        private void GeneratePrintImageCache(MapImageInfo info)
        {
            if (info.BackgroudMapImage == null && !string.IsNullOrWhiteSpace(info.ImageQuery))
            {
                WriteableBitmap mapImage = new WriteableBitmap((int)info.RenderSize.Width, (int)info.RenderSize.Height);
                using (var isoFile = IsolatedStorageFile.GetUserStoreForApplication())
                using (var file = isoFile.OpenFile(info.FileName, FileMode.Open))
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

                    foreach (var item in mapMain.Children)
                    {
                        if (item.Visibility == Visibility.Visible)
                        {
                            //render address and submaps
                            ScaleTransform transform = new ScaleTransform()
                            {
                                ScaleX = ZoomRate,
                                ScaleY = ZoomRate
                            };

                            mapImage.Render(item, transform);
                            mapImage.Invalidate();
                        }

                    }

                    info.BackgroudMapImage = mapImage;

                    //clear tile image
                    try
                    {
                        isoFile.DeleteFile(info.FileName);
                    }
                    catch { }
                }
            }
        }

        private void DownloadMapImage(object state)
        {
            var info = state as MapImageInfo;

            WebClient client = new WebClient();
            string url = string.Format("{0}?{1}", BaseURL, info.ImageQuery);
            client.OpenReadCompleted += (s, a) =>
            {
                try
                {
                    using (var isoFile = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        if (isoFile.FileExists(info.FileName))
                        {
                            isoFile.DeleteFile(info.FileName);
                        }

                        using (var fileStream = isoFile.CreateFile(info.FileName))
                        {
                            byte[] buffer = new byte[5120];
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

        public class MapImageInfo
        {
            public MapImageInfo(int id, DistributionMapUI dMap, double width, double height)
            {
                Id = id;
                DMap = dMap;
                if (dMap != null)
                {
                    Map.Location[] allLocation = new Map.Location[dMap.Locations.Count];
                    for (int i = 0; i < dMap.Locations.Count; i++)
                    {
                        allLocation[i] = new Map.Location(dMap.Locations[i].Latitude, dMap.Locations[i].Longitude);
                    }
                    BestView = new LocationRect(allLocation);
                }
                RenderSize = new Size(width, height);
            }

            public int Id;
            public LocationRect BestView;
            public Map.Location Center;
            public double ZoomLevel;
            public Size RenderSize;
            public string ImageQuery = string.Empty;
            public string FileName = string.Empty;
            public DistributionMapUI DMap { get; set; }
            public WriteableBitmap BackgroudMapImage { get; set; }
        }
        #endregion
    }
}
