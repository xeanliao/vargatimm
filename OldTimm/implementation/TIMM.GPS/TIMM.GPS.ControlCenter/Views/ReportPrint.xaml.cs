using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using TIMM.GPS.ControlCenter.Extends;
using TIMM.GPS.ControlCenter.Interface;
using TIMM.GPS.ControlCenter.Model;
using TIMM.GPS.ControlCenter.UserControls;
using TIMM.GPS.ControlCenter.UserControls.Print;
using TIMM.GPS.Model;
using TIMM.GPS.Net.Http;
using TIMM.GPS.Silverlight.Utility;
using Microsoft.Maps.MapControl;


namespace TIMM.GPS.ControlCenter.Views
{
    public partial class ReportPrint : Page
    {

       #region Private field and property
        private const int MapSplitWidth = 592;
        private const int MapSplitHeight = 476;
        private const int MapOverSize = 60;

        private LinkedListNode<MapImageInfo> m_MapImage;
        private LinkedList<MapImageInfo> m_MapImageList = new LinkedList<MapImageInfo>();
        private AutoResetEvent m_AutoReset = new AutoResetEvent(false);
        private int m_ThreadCount = 0;
        private BackgroundWorker m_Worker = new BackgroundWorker();
        private BackgroundWorker m_EditMapWorker = new BackgroundWorker();
        private readonly string BaseURL;
        private double printImageWidth;
        private double printImageHeight;
        private long m_EncodingThreadCount = 0;

        private CampaignUI m_Campaign { get; set; }
        private List<NdAddress> m_NdAddress { get; set; }
        private ICanEditMapPdfPage m_CurrentEditPage;

        /// <summary>
        /// the state of Gtu dot
        /// </summary>
        enum GtuState
        {
            Origin = 0,    // gtu creates
            Added = 1,  // user click on the blank
            Removed = 2 // delete Raw gtu 
        }

        #endregion

        #region Print Options
        
        private bool needReDraw = false;

        private bool m_IsUseMapRoadMode = true;
        public bool IsUseMapRoadMode
        {
            get { return m_IsUseMapRoadMode; }
            set
            {
                if (value != m_IsUseMapRoadMode)
                {
                    if (value)
                    {
                        mapMain.Mode = new Map.RoadMode();
                    }
                    else
                    {
                        mapMain.Mode = new Map.AerialMode(true);
                    }
                    m_IsUseMapRoadMode = value;
                    needReDraw = true;
                }
            }
        }

        public bool m_IsShowCover = true;
        public bool IsShowCover
        {
            get
            {
                return m_IsShowCover;
            }
            set
            {

                if (value != m_IsShowCover)
                {
                    foreach (var item in PrintArea.Children)
                    {
                        if (item is PrintCover)
                        {
                            item.Visibility = value == true ? Visibility.Visible : Visibility.Collapsed;
                            break;
                        }
                    }
                    m_IsShowCover = value;
                }

            }
        }

        public bool m_IsShowCampaignSummary = false; // invisible by default
        public bool IsShowCampaignSummary
        {
            get
            {
                return m_IsShowCampaignSummary;
            }
            set
            {

                if (value != m_IsShowCampaignSummary)
                {
                    //foreach (var item in PrintArea.Children)
                    //{
                    //    if (item is PrintCampaign)
                    //    {
                    //        item.Visibility = value == true ? Visibility.Visible : Visibility.Collapsed;
                    //        break;
                    //    }
                    //}
                    needReDraw = true;
                    m_IsShowCampaignSummary = value;
                }

            }
        }

        public bool m_IsShowSummaryOfSubmap = false; // invisible by default
        public bool IsShowSummaryOfSubmap 
        {
            get
            {
                return m_IsShowSummaryOfSubmap;
            }
            set
            {

                if (value != m_IsShowSummaryOfSubmap)
                {
                    foreach (var item in PrintArea.Children)
                    {
                        if (item is PrintSubMapSummary)
                        {
                            item.Visibility = value == true ? Visibility.Visible : Visibility.Collapsed;
                            break;
                        }
                    }
                    m_IsShowSummaryOfSubmap = value;
                }

            }
        }

        private bool m_IsShowLocations = true;
        public bool IsShowLocations
        {
            get
            {
                return m_IsShowLocations;
            }
            set
            {
                if (m_IsShowLocations != value)
                {
                    var addressLayer = mapMain.FindName("addressLayer") as Map.MapLayer;
                    if (addressLayer != null)
                    {
                        foreach (var item in addressLayer.Children)
                        {
                            if (item is Image)
                            {
                                item.Visibility = value == true ? Visibility.Visible : Visibility.Collapsed;
                            }
                        }
                    }
                    m_IsShowLocations = value;
                    needReDraw = true;                  
                }
                
            }
        }

        private bool _isClientMap = true;

        /// <summary>
        /// Control show the client (edit) map or distribute (unedit) map
        /// Ture - client(edit) map: show all dots (modified points) in the boundary
        /// False - distribute(unedit) map: show original raw data (include both inside and outside)
        /// </summary>
        public bool IsClientMap
        {
            get { return _isClientMap; }
            set
            {
                if (value != _isClientMap)
                {
                    var dmapLayerName = ExportPageTypeEnum.DMap.ToString();                   

                    _isClientMap = value;
                    needReDraw = true;                  
                }
            }
        }
        

        private bool m_IsShowRadii = true;
        public bool IsShowRadii
        {
            get
            {
                return m_IsShowRadii;
            }
            set
            {
                if (value != m_IsShowRadii)
                {
                    var addressLayer = mapMain.FindName("addressLayer") as Map.MapLayer;
                    if (addressLayer != null)
                    {
                        foreach (var item in addressLayer.Children)
                        {
                            if (!(item is Image))
                            {
                                item.Visibility = value == true ? Visibility.Visible : Visibility.Collapsed;
                            }
                        }
                    }

                    m_IsShowRadii = value;
                    needReDraw = true;       
                }
            }
        }

        private bool m_IsShowNDAreasForCampaignMap = false;
        public bool IsShowNDAreasForCampaignMap
        {
            get
            {
                return m_IsShowNDAreasForCampaignMap;
            }
            set
            {
                if (value == m_IsShowNDAreasForCampaignMap)
                {
                    return;
                }
                m_IsShowNDAreasForCampaignMap = value;
                needReDraw = true;       
            }
        }

        private bool m_IsShowNDAreasForDistributionMap = true;
        public bool IsShowNDAreasForDistributionMap
        {
            get
            {
                return m_IsShowNDAreasForDistributionMap;
            }
            set
            {
                if (value == m_IsShowNDAreasForDistributionMap)
                {
                    return;
                }
                m_IsShowNDAreasForDistributionMap = value;
                needReDraw = true;
            }
        }

        private bool m_IsShowSubMaps = false; // invisible by default
        public bool IsShowSubMaps
        {
            get
            {
                return m_IsShowSubMaps;
            }
            set
            {
                if (value != m_IsShowSubMaps)
                {
                    //foreach (var item in PrintArea.Children)
                    //{
                    //    if (item is PrintSubMap)
                    //    {
                    //        item.Visibility = value == true ? Visibility.Visible : Visibility.Collapsed;
                    //    }
                    //}

                    needReDraw = true;
                    m_IsShowSubMaps = value;
                }

            }
        }

        public bool m_IsShowSubMapDetails = false; // by default
        public bool IsShowSubMapDetails
        {
            get
            {
                return m_IsShowSubMapDetails;
            }
            set
            {
                if (value != m_IsShowSubMapDetails)
                {
                    foreach (var item in PrintArea.Children)
                    {
                        if (item is PrintSubMapDetail)
                        {
                            item.Visibility = value == true ? Visibility.Visible : Visibility.Collapsed;
                        }
                    }
                    m_IsShowSubMapDetails = value;
                }

            }
        }

        private bool m_IsShowClassificationOutlines = true;
        public bool IsShowClassificationOutlines
        {
            get
            {
                return m_IsShowClassificationOutlines;
            }
            set
            {
                if (value != m_IsShowClassificationOutlines)
                {
                    mapMain.GetElementType<Map.MapPolygon>().ForEach(i =>
                    {
                        if (i.Tag != null && string.Compare(i.Tag.ToString(), "Classification") == 0)
                        {
                            i.Visibility = value == true ? Visibility.Visible : Visibility.Collapsed;
                        }
                    });
                    mapMain.GetElementType<TextBlock>().ForEach(i =>
                    {
                        if (i.Tag != null && string.Compare(i.Tag.ToString(), "Classification") == 0)
                        {
                            i.Visibility = value == true ? Visibility.Visible : Visibility.Collapsed;
                        }
                    });
                    m_IsShowClassificationOutlines = value;
                    needReDraw = true;          
                }
            }
        }

        private bool m_IsShowSubMapPenetrationColors = true;
        public bool IsShowSubMapPenetrationColors
        {
            get
            {
                return m_IsShowSubMapPenetrationColors;
            }
            set
            {
                if (value != m_IsShowSubMapPenetrationColors)
                {
                    var colorLayer = mapMain.GetElementType<Map.MapPolygon>();
                    foreach (var item in colorLayer)
                    {
                        if (item.Tag != null && item.Tag.ToString().StartsWith("percentColor"))
                        {
                            item.Visibility = value == true ? Visibility.Visible : Visibility.Collapsed;
                        }
                        else if (item.Tag != null && string.Compare("subMapBackground", item.Tag.ToString()) == 0)
                        {
                            item.Visibility = value == false ? Visibility.Visible : Visibility.Collapsed;
                        }
                    }
                    needReDraw = true;    
                }

                foreach (var item in PrintArea.Children)
                {
                    if (item is ICanEditMapPdfPage)
                    {
                        if (value == true)
                        {
                            (item as ICanEditMapPdfPage).SetPercentageColor(IsCustomSubMapPenetrationColors == true ? CustomColor : m_Campaign.CampaignPercentageColors);
                        }
                        else
                        {
                            (item as ICanEditMapPdfPage).SetPercentageColor(null);
                        }

                    }
                }

                if (value == true)
                {
                    if (m_IsCustomSubMapPenetrationColors == true)
                    {
                        //re draw penetration color
                        var colorLayer = mapMain.GetElementType<Map.MapPolygon>();
                        foreach (var item in colorLayer)
                        {
                            if (item.Tag != null && item.Tag.ToString().StartsWith("percentColor"))
                            {
                                var tag = item.Tag.ToString();
                                double percent;
                                if (double.TryParse(tag.Substring(13), out percent))
                                {
                                    (item as Map.MapPolygon).Fill = GetPercentageColor(percent);
                                }
                            }
                        }
                    }
                       
                }

                

                m_IsShowSubMapPenetrationColors = value;
                     
            }
        }
        

        private bool m_IsCustomSubMapPenetrationColors = false;
        public bool IsCustomSubMapPenetrationColors
        {
            get { return m_IsCustomSubMapPenetrationColors; }
            set
            {
                if (m_IsCustomSubMapPenetrationColors != value)
                {
                    needReDraw = true;
                }
                if (value == true)
                {
                    needReDraw = true;
                }
                m_IsCustomSubMapPenetrationColors = value;
            }
        }

        public bool m_IsIncludeDMap = true;
        public bool IsIncludeDMap
        {
            get
            {
                return m_IsIncludeDMap;
            }
            set
            {
                if (value != m_IsIncludeDMap)
                {
                    foreach (var item in PrintArea.Children)
                    {
                        if (item is PrintDMap)
                        {
                            item.Visibility = value == true ? Visibility.Visible : Visibility.Collapsed;
                        }
                    }
                    m_IsIncludeDMap = value;
                }
            }
        }

        public bool m_IsShowGTU = true;
        public bool IsShowGTU
        {
            get
            {
                return m_IsShowGTU;
            }
            set
            {
                if (value != m_IsShowGTU)
                {
                    mapMain.GetElementType<Map.MapPolygon>().ForEach(i =>
                    {
                        if (i.Tag != null && i.Tag.ToString().StartsWith("GTU"))
                        {
                            i.Visibility = value == true ? Visibility.Visible : Visibility.Collapsed;
                        }
                    });
                    m_IsShowGTU = value;
                    needReDraw = true;
                }
            }
        }

        public int m_GTURadii = 50; // will divide 1000 mile eg. 50 means 0.05 mile
        public int GTURadii
        {
            get
            {
                return m_GTURadii;
            }
            set
            {
                if (m_GTURadii != value)
                {
                    m_GTURadii = value;
                    if (m_IsShowGTU)
                    {
                        needReDraw = true;
                    }                    
                }
            }
        }

        private List<CampaignPercentageColorUI> CustomColor = new List<CampaignPercentageColorUI>();
        #endregion

        public ReportPrint(bool isCampaignPrint) : this()
        {
            if (isCampaignPrint)
            {
                m_IsShowCampaignSummary = true;
                m_IsShowSummaryOfSubmap = true;
                m_IsShowSubMaps = true;
                m_IsShowSubMapDetails = true;
            }
            else
            {
               
                // Comment out due to thate these fields have been set 'false' already.
                //m_IsShowCampaignSummary = false;
                //m_IsShowSummaryOfSubmap = false;
                //m_IsShowSubMaps = false;
                //m_IsShowSubMapDetails = false;
            }            
        }

        public ReportPrint()
        {
            InitializeComponent();

            this.mapMain.CredentialsProvider = new ApplicationIdCredentialsProvider(App.MapKey);

            printImageWidth = mapMain.Width;
            printImageHeight = mapMain.Height;
            //BaseURL = string.Format("http://{0}:{1}{2}",
            //    HtmlPage.Document.DocumentUri.DnsSafeHost,
            //    HtmlPage.Document.DocumentUri.Port,
            //    "/MapProxy/MapTileImageProxy.ashx");
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

            LoadNdAddress();

            //btnStart.IsEnabled = true;
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
            //mapMain.Visibility = System.Windows.Visibility.Visible;
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
            };

            pd.Print(m_Campaign.DisplayName, new PrinterFallbackSettings { ForceVector = true });
        }

        protected void OnShowPrintOption(object sender, RoutedEventArgs e)
        {
            PrintOptionDialog dialog = new PrintOptionDialog();
            dialog.Closed += (s, a) => 
            {
                if (dialog.DialogResult == true)
                {
                    var option = dialog.DataContext as PrintOption;
                    IsUseMapRoadMode = option.IsUseMapRoadMode;
                    IsShowCover = !option.SuppressCover;
                    IsShowCampaignSummary = !option.SuppressCampaignSummary;
                    IsShowSummaryOfSubmap = !option.SuppressSummaryOfSubmap;
                    IsShowLocations = !option.SuppressLocations;
                    
                    IsClientMap = option.IsClientMap;

                    IsShowRadii = !option.SuppressRadii;
                    IsShowNDAreasForCampaignMap = !option.SuppressNDAreasForCampaignMap;
                    IsShowNDAreasForDistributionMap = !option.SuppressNDAreasForDistributionmap;
                    IsShowSubMaps = !option.SuppressSubMaps;
                    IsShowSubMapDetails = !option.SuppressSubMapDetails;
                    IsShowClassificationOutlines = !option.SuppressClassificationOutlines;
                    IsIncludeDMap = !option.SuppressDMaps;
                    IsShowGTU = !option.SuppressGTU;
                    if (option.UseCustomColors)
                    {
                        CustomColor = option.GetPercentageColor();
                    }
                    IsCustomSubMapPenetrationColors = option.UseCustomColors;
                    IsShowSubMapPenetrationColors = option.ShowPenetrationColors;
                    GTURadii = option.GTUDotRadii;
                    if (needReDraw && m_Campaign != null)
                    {
                        InitMap();
                    	StarDrawAllMap();
                    }

                    /// Summary: Control DMaps Pages
                    /// Remark: control must be after redraw or current modification
                    foreach (var page in this.PrintArea.Children)
                    {
                        if (page is PrintDMap)
                        {
                            var dmapPage = page as PrintDMap;
                            dmapPage.Visibility = dmapPage.DMap.IsChecked ? Visibility.Collapsed : Visibility.Visible;
                        }
                    }
                }
                needReDraw = false;       
            };

            // Get all DistributionMapUI
            var dmapUIs = new ObservableCollection<DistributionMapUI>();
            if (m_Campaign != null && m_Campaign.SubMaps != null && m_Campaign.SubMaps.Count > 0)
            {
                foreach (var submap in this.m_Campaign.SubMaps)
                {
                    foreach (var dmapUI in submap.DistributionMaps)
                    {
                        dmapUIs.Add(dmapUI);
                    }
                }
            }
            
            

            PrintOption initOption = new PrintOption
            {
                IsUseMapRoadMode = m_IsUseMapRoadMode,
                SuppressCover = !m_IsShowCover,
                SuppressCampaignSummary = !m_IsShowCampaignSummary,
                SuppressSummaryOfSubmap = !m_IsShowSummaryOfSubmap,
                SuppressLocations = !m_IsShowLocations,

                IsClientMap = _isClientMap,

                SuppressRadii = !m_IsShowRadii,
                SuppressNDAreasForCampaignMap = !m_IsShowNDAreasForCampaignMap,
                SuppressNDAreasForDistributionmap = !m_IsShowNDAreasForDistributionMap,
                SuppressSubMaps = !m_IsShowSubMaps,
                SuppressSubMapDetails = !m_IsShowSubMapDetails,
                SuppressDMaps = !m_IsIncludeDMap,
                SuppressGTU = !m_IsShowGTU,
                SuppressClassificationOutlines = !m_IsShowClassificationOutlines,
                ShowPenetrationColors = m_IsShowSubMapPenetrationColors,
                GTUDotRadii = m_GTURadii,
                UseCustomColors = IsCustomSubMapPenetrationColors,
                IsShowDMaps = System.Windows.Visibility.Visible,
                DMaps = dmapUIs
            };

            if (m_Campaign != null && IsCustomSubMapPenetrationColors == false)
            {
                initOption.SetPercentageColor(m_Campaign.CampaignPercentageColors);
            }
            else
            {
                initOption.SetPercentageColor(CustomColor);
            }

            dialog.DataContext = initOption;
            dialog.Show();
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
                m_MapImage = m_MapImageList.First;
                var enumerator = m_MapImageList.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.MapType == page.GetPageType() && enumerator.Current.Id == page.GetId())
                    {
                        break;
                    }
                    m_MapImage = m_MapImage.Next;
                }
                SwithMapLayerDisplay();

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

        protected void mapMain_ViewChangeStart(object sender, Map.MapEventArgs e)
        {
            //dispaly edit apply button whe map is changing
            btnApply.IsEnabled = false;
        }

        protected void mapMain_ViewChangeEnd(object sender, Map.MapEventArgs e)
        {
            //dispaly edit apply button whe map is changing

            btnApply.IsEnabled = true;
        }

        protected void OnSetTargetMethod(object sender, RoutedEventArgs e)
        {
            if (m_Campaign != null)
            {
                m_Campaign.TargetMethod = txtTargetMethod.Text;
            }
            
            foreach(var page in PrintArea.Children)
            {
                if (page is INeedSetTargetMethod)
                {
                    (page as INeedSetTargetMethod).SetTargetMethod(txtTargetMethod.Text);
                }
            }
        }

        protected void OnNavSwitchToAerial(object sender, RoutedEventArgs e)
        {
            mapMain.Mode = new Map.AerialMode(true);
            btnAerial.IsActive = true;
        }

        protected void OnNavSwitchToRoad(object sender, RoutedEventArgs e)
        {
            mapMain.Mode = new Map.RoadMode();
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

        #region NDAddress
        private void LoadNdAddress()
        {
            HttpClient.Get<List<NdAddress>>("service/ndaddress/query", args =>
            {
                if (args.Data != null && args.Data.Count > 0)
                {
                    m_NdAddress = args.Data;
                    InitNdAddress();
                }
                else
                {
                    btnStart.IsEnabled = true;
                }
            }, false, false);
        }

        private void InitNdAddress()
        {
            if (m_NdAddress == null || m_NdAddress.Count == 0)
            {
                return;
            }

            Map.MapLayer ndAddressLayer = new Map.MapLayer
            {
                Name = "ndAddressLayer",
                Visibility = Visibility.Visible
            };
            var strokeColor = new SolidColorBrush(Color.FromArgb(0xFF, 0xBB, 0x00, 0x00));
            var fillColor = new SolidColorBrush(Color.FromArgb(0x32, 0xBB, 0x00, 0x00));
            
            	
            m_NdAddress.ForEach(address =>
            {
                if (address.NdAddressCoordinates != null && address.NdAddressCoordinates.Count > 0)
                {
                    // add nd area
                    Map.MapPolygon poly = new Map.MapPolygon();
                    var locations = new Map.LocationCollection();
                    address.NdAddressCoordinates.ForEach(i =>
                    {
                        locations.Add(new Map.Location() { Latitude = i.Latitude, Longitude = i.Longitude });
                    });
                    poly.Locations = locations;
                    poly.Stroke = strokeColor;
                    poly.StrokeThickness = 1;
                    poly.StrokeLineJoin = PenLineJoin.Bevel;
                    poly.Fill = fillColor;
                    ndAddressLayer.Children.Add(poly);
                }
                else
                {
                    //add nd flag
                    var location = new Map.Location { Latitude = address.Latitude, Longitude = address.Longitude };
                    Image flag = new Image
                    {
                        Tag = "NdAddressFlag",
                        Source = ResourceHelper.GetBitmap("/Images/flag.png"),
                        Stretch = Stretch.None
                    };
                    ndAddressLayer.AddChild(flag, location, Map.PositionOrigin.Center);
                }
            });
            mapMain.Children.Insert(0, ndAddressLayer);
            btnStart.IsEnabled = true;
        }
        #endregion

        #region Private Method
        private void LoadPage(int id)
        {
            Loading.IsBusy = true;

            HttpClient.Get<CampaignUI>(string.Format("/service/campaign/detail/{0}", id), BindMap, false, false);
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
            m_Campaign = result.Data;
            //set percent color
            if (m_Campaign.CampaignPercentageColors == null || m_Campaign.CampaignPercentageColors.Count == 0)
            {
                m_Campaign.CampaignPercentageColors = new List<CampaignPercentageColorUI>()
                {
                    new CampaignPercentageColorUI{ColorId = 1,Min = 0d,Max = 0.20d},
                    new CampaignPercentageColorUI{ColorId = 2,Min = 0.20d,Max = 0.40d},
                    new CampaignPercentageColorUI{ColorId = 3,Min = 0.40d,Max = 0.60d},
                    new CampaignPercentageColorUI{ColorId = 4,Min = 0.60d,Max = 0.80d},
                    new CampaignPercentageColorUI{ColorId = 5,Min = 0.80d,Max = 1.0d}
                };
            }
            
            InitMap();

            StarDrawAllMap();
        }

        private void StarDrawAllMap()
        {
            if (m_Campaign == null)
            {
                return;
            }
            Loading.IsBusy = true;
            popMap.IsOpen = true;
            mapMain.ViewChangeEnd += OnMapLoadedCompleted;
            m_MapImage = m_MapImageList.First;
            //only display campaign map layer and address map layer

            while (this.CheckMapDisplay() == false && m_MapImage.Next != null)
            {
                m_MapImage = m_MapImage.Next;                
            }

            SwithMapLayerDisplay();

            if (m_MapImage.Value.BestView != null)
            {
                mapMain.SetView(m_MapImage.Value.BestView);
            }
            else
            {
                mapMain.SetView(m_MapImage.Value.Center, m_MapImage.Value.ZoomLevel);
            }
        }

        private void InitMap()
        {
            #region Prepare Image List
            m_Campaign.TargetMethod = txtTargetMethod.Text;
            m_MapImageList.Clear();

            //default center and zoom level            
            var center = new Map.Location { Latitude = m_Campaign.Latitude, Longitude = m_Campaign.Longitude };
            var zoomLevel = m_Campaign.ZoomLevel;

            List<Map.Location> campaignLocations = new List<Map.Location>();
            m_Campaign.SubMaps.ForEach(i =>
            {
                i.SubMapCoordinates.ForEach(j =>
                {
                    campaignLocations.Add(new Map.Location(j.Latitude, j.Longitude));
                });
            });
            var campaignMapInfo = new MapImageInfo(0);
            //if there is no submaps then use the campaing location to finde best view
            if (campaignLocations.Count == 0)
            {
                campaignMapInfo.Center = center;
                campaignMapInfo.ZoomLevel = zoomLevel;
            }
            else
            {
                campaignMapInfo.BestView = new Map.LocationRect(campaignLocations);
            }

            m_MapImageList.AddLast(campaignMapInfo);

            m_Campaign.SubMaps.ForEach(submap =>
            {
                var submapInfo = new MapImageInfo(submap.Id, submap);
                m_MapImageList.AddLast(submapInfo);
                submap.DistributionMaps.ForEach(dmap =>
                {
                    var dmapInfo = new MapImageInfo(dmap.Id, dmap);
                    m_MapImageList.AddLast(dmapInfo);
                });
            });
            m_MapImage = m_MapImageList.First;
            #endregion

            mapMain.Children.Clear();
            InitNdAddress();

            Map.MapLayer campaignLayer = new Map.MapLayer 
            { 
                Name = string.Format("{0}0", ExportPageTypeEnum.Campaign),
                Tag = 0 
            };
            mapMain.Children.Add(campaignLayer);
            foreach (var item in m_Campaign.SubMaps)
            {
                //check submap have coordinats
                if (item.SubMapCoordinates == null || item.SubMapCoordinates.Count == 0)
                {
                    continue;
                }

                #region Campaign Map
                var subMapBorderColor = new SolidColorBrush(Color.FromArgb(0xFF, (byte)item.ColorR, (byte)item.ColorG, (byte)item.ColorB));
                var subMapFillColor = new SolidColorBrush(Color.FromArgb(0x19, (byte)item.ColorR, (byte)item.ColorG, (byte)item.ColorB));
                var subAreaLocations = new Map.LocationCollection();
                item.SubMapCoordinates.ForEach(i =>
                {
                    subAreaLocations.Add(new Map.Location { Latitude = i.Latitude, Longitude = i.Longitude });
                });
                //add polygon for campaign
                {
                    Map.MapPolygon poly = new Map.MapPolygon()
                    {
                        Tag = "campaignBorder",
                        Locations = subAreaLocations,
                        Stroke = subMapBorderColor,
                        StrokeThickness = 5,
                        StrokeLineJoin = PenLineJoin.Round,
                        Fill = subMapFillColor,
                        Visibility = m_IsShowSubMapPenetrationColors == false ? Visibility.Visible : Visibility.Collapsed
                    };
                    campaignLayer.Children.Add(poly);

                    //Percentage Color
                    Map.MapPolygon percentColorPolygon = new Map.MapPolygon()
                    {
                        Tag = string.Format("percentColor:{0}", item.DisplayPenetration.ToString()),
                        Locations = subAreaLocations,
                        Stroke = subMapBorderColor,
                        StrokeThickness = 5,
                        StrokeLineJoin = PenLineJoin.Round,
                        Fill = GetPercentageColor(item.DisplayPenetration),
                        Visibility = m_IsShowSubMapPenetrationColors == true ? Visibility.Visible : Visibility.Collapsed
                    };

                    campaignLayer.Children.Add(percentColorPolygon);

                    TextBlock tb = new TextBlock
                    {
                        Text = item.OrderId.ToString(),
                        FontSize = 11,
                        Foreground = new SolidColorBrush(Colors.Black),
                    };
                    campaignLayer.AddChild(tb, GeoCodeCalculation.GetPloygonCenter(subAreaLocations), Map.PositionOrigin.Center);
                }
                #endregion

                #region Sub Map
                Map.MapLayer subMapLayer = new Map.MapLayer
                {
                    Name = string.Format("{0}{1}", ExportPageTypeEnum.SubMap.ToString(), item.Id),
                    Tag = item.Id,
                    Visibility = IsShowSubMaps == true ? Visibility.Visible : Visibility.Collapsed
                };

                Map.MapPolygon subMapBackground = new Map.MapPolygon
                {
                    Tag = "subMapBackground",
                    Locations = subAreaLocations,
                    StrokeLineJoin = PenLineJoin.Round,
                    Fill = subMapFillColor,
                    Visibility = m_IsShowSubMapPenetrationColors == false ? Visibility.Visible : Visibility.Collapsed
                };
                subMapLayer.Children.Add(subMapBackground);

                #region FiveZip
                int orderId = 1;
                item.FiveZipAreas.ForEach(area =>
                {
                    area.OrderId = orderId++;
                    var areaBorderColor = new SolidColorBrush(Color.FromArgb(0xCC, 0x00, 0x00, 0xFF));
                    var locations = new Map.LocationCollection();
                    area.Locations.ForEach(i =>
                    {
                        locations.Add(new Map.Location { Latitude = i.Latitude, Longitude = i.Longitude });
                    });
                    var ploygon = new Map.MapPolygon
                    {
                        Tag = "Classification",
                        Stroke = areaBorderColor,
                        StrokeThickness = 1,
                        StrokeLineJoin = PenLineJoin.Round,
                        Locations = locations,
                        Visibility = m_IsShowClassificationOutlines == true ? Visibility.Visible : Visibility.Collapsed
                    };

                    subMapLayer.Children.Add(ploygon);

                    var percentColorPolygon = new Map.MapPolygon
                    {
                        Tag = string.Format("percentColor:{0}", area.DisplayPenetration.ToString()),
                        Fill = GetPercentageColor(area.DisplayPenetration),
                        StrokeLineJoin = PenLineJoin.Round,
                        Locations = locations,
                        Visibility = m_IsShowSubMapPenetrationColors == true ? Visibility.Visible : Visibility.Collapsed
                    };
                    subMapLayer.Children.Add(percentColorPolygon);

                    TextBlock tb = new TextBlock
                    {
                        Tag = "Classification",
                        Text = area.OrderId.ToString(),
                        FontSize = 11,
                        Foreground = new SolidColorBrush(Colors.Black),
                    };
                    subMapLayer.AddChild(tb, GeoCodeCalculation.GetPloygonCenter(locations), Map.PositionOrigin.Center);
                });
                #endregion

                #region PremiumCRoutes
                orderId = 1;
                item.PremiumCRoutes.ForEach(area =>
                {
                    area.OrderId = orderId++;
                    var areaBorderColor = new SolidColorBrush(Color.FromArgb(0xCC, 0x00, 0xFF, 0xFF));

                    var locations = new Map.LocationCollection();
                    area.Locations.ForEach(i =>
                    {
                        locations.Add(new Map.Location { Latitude = i.Latitude, Longitude = i.Longitude });
                    });
                    var ploygon = new Map.MapPolygon
                    {
                        Tag = "Classification",
                        Stroke = areaBorderColor,
                        StrokeThickness = 1,
                        StrokeLineJoin = PenLineJoin.Round,
                        Locations = locations,
                        Visibility = IsShowClassificationOutlines == true ? Visibility.Visible : Visibility.Collapsed
                    };

                    subMapLayer.Children.Add(ploygon);

                    var percentColorPolygon = new Map.MapPolygon
                    {
                        Tag = string.Format("percentColor:{0}", area.DisplayPenetration.ToString()),
                        Fill = GetPercentageColor(area.DisplayPenetration),
                        StrokeLineJoin = PenLineJoin.Round,
                        Locations = locations,
                        Visibility = m_IsShowSubMapPenetrationColors == true ? Visibility.Visible : Visibility.Collapsed
                    };
                    subMapLayer.Children.Add(percentColorPolygon);

                    TextBlock tb = new TextBlock
                    {
                        Tag = "Classification",
                        Text = area.OrderId.ToString(),
                        FontSize = 11,
                        Foreground = new SolidColorBrush(Colors.Black),
                    };
                    subMapLayer.AddChild(tb, GeoCodeCalculation.GetPloygonCenter(locations), Map.PositionOrigin.Center);
                });
                #endregion

                #region Tract
                orderId = 1;
                item.Tracts.ForEach(area =>
                {
                    area.OrderId = orderId++;
                    var areaBorderColor = new SolidColorBrush(Color.FromArgb(0xCC, 0x00, 0xFF, 0x00));
                    var locations = new Map.LocationCollection();
                    area.Locations.ForEach(i =>
                    {
                        locations.Add(new Map.Location { Latitude = i.Latitude, Longitude = i.Longitude });
                    });
                    var ploygon = new Map.MapPolygon
                    {
                        Tag = "Classification",
                        Stroke = areaBorderColor,
                        StrokeThickness = 1,
                        StrokeLineJoin = PenLineJoin.Round,
                        Locations = locations,
                        Visibility = IsShowClassificationOutlines == true ? Visibility.Visible : Visibility.Collapsed
                    };
                    subMapLayer.Children.Add(ploygon);

                    var percentColorPolygon = new Map.MapPolygon
                    {
                        Tag = string.Format("percentColor:{0}", area.DisplayPenetration.ToString()),
                        Fill = GetPercentageColor(area.DisplayPenetration),
                        StrokeLineJoin = PenLineJoin.Round,
                        Locations = locations,
                        Visibility = m_IsShowSubMapPenetrationColors == true ? Visibility.Visible : Visibility.Collapsed
                    };
                    subMapLayer.Children.Add(percentColorPolygon);

                    TextBlock tb = new TextBlock
                    {
                        Tag = "Classification",
                        Text = area.OrderId.ToString(),
                        FontSize = 11,
                        Foreground = new SolidColorBrush(Colors.Black),
                    };
                    subMapLayer.AddChild(tb, GeoCodeCalculation.GetPloygonCenter(locations), Map.PositionOrigin.Center);
                });
                #endregion

                #region BlockGroup
                item.BlockGroups.ForEach(area =>
                {
                    var areaBorderColor = new SolidColorBrush(Color.FromArgb(0xCC, 0xBB, 0x00, 0x00));
                    var locations = new Map.LocationCollection();
                    area.Locations.ForEach(i =>
                    {
                        locations.Add(new Map.Location { Latitude = i.Latitude, Longitude = i.Longitude });
                    });
                    var ploygon = new Map.MapPolygon
                    {
                        Tag = "Classification",
                        Stroke = areaBorderColor,
                        StrokeThickness = 1,
                        StrokeLineJoin = PenLineJoin.Round,
                        Locations = locations,
                        Visibility = IsShowClassificationOutlines == true ? Visibility.Visible : Visibility.Collapsed
                    };
                    subMapLayer.Children.Add(ploygon);

                    var percentColorPolygon = new Map.MapPolygon
                    {
                        Tag = string.Format("percentColor:{0}", area.DisplayPenetration.ToString()),
                        Fill = GetPercentageColor(area.DisplayPenetration),
                        StrokeLineJoin = PenLineJoin.Round,
                        Locations = locations,
                        Visibility = m_IsShowSubMapPenetrationColors == true ? Visibility.Visible : Visibility.Collapsed
                    };
                    subMapLayer.Children.Add(percentColorPolygon);

                    TextBlock tb = new TextBlock
                    {
                        Tag = "Classification",
                        Text = area.OrderId.ToString(),
                        FontSize = 11,
                        Foreground = new SolidColorBrush(Colors.Black),
                    };
                    subMapLayer.AddChild(tb, GeoCodeCalculation.GetPloygonCenter(locations), Map.PositionOrigin.Center);
                });
                #endregion

                #region sub map border
                var subAreaBorder = new Map.MapPolygon()
                {
                    Locations = subAreaLocations,
                    StrokeLineJoin = PenLineJoin.Round,
                    Stroke = subMapBorderColor,
                    StrokeThickness = 5d,
                };
                subMapLayer.Children.Add(subAreaBorder);
                #endregion

                mapMain.Children.Add(subMapLayer);
                #endregion
            }

            #region DMaps
            m_Campaign.SubMaps.ForEach(submap =>
            {
                foreach(var dmap in submap.DistributionMaps)
                {
                    var dmapLayer = new Map.MapLayer 
                    { 
                        Name = string.Format("{0}{1}", ExportPageTypeEnum.DMap.ToString(), dmap.Id),
                        Tag = dmap.Id
                    };

                    #region Boundary

                    Map.LocationCollection dmapLocations = new Map.LocationCollection();
                    dmap.Locations.ForEach(i =>
                    {
                        dmapLocations.Add(new Map.Location { Longitude = i.Longitude, Latitude = i.Latitude });
                    });
                    var dmapPolygon = new Map.MapPolygon
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

                    #region Gtu dot

                    if (m_IsShowGTU)
                    {
                        foreach (var gtu in dmap.GtuInfo)
                        {
                            var state = (GtuState)gtu.nCellID;
                            bool isShowGtuDot = false;
        
                            if (_isClientMap)
                            {
                                // hide [Removed] state, show [Origin] or [Added]
                                if (state != GtuState.Removed)
                                {
                                    // Show in boundary
                                    isShowGtuDot = GeoCodeCalculation.PointInPolygon(dmapLocations, gtu.Latitude, gtu.Longitude);
                                }
                            }
                            else // Distribute map
                            {
                                // hide [Added], show [Origin] or [Removed]
                                if (state != GtuState.Added)
                                {
                                    isShowGtuDot = true;
                                }
                            }

                            if (isShowGtuDot)
                            {
                                var gtuPolygon = new Map.MapPolygon
                                {
                                    Tag = string.Format("GTU{0}", dmap.Id),
                                    Fill = new SolidColorBrush(ColorHelper.FromRgbString(0xBF, gtu.UserColor)),
                                    Stroke = new SolidColorBrush(Colors.Black),
                                    StrokeThickness = 1,
                                    StrokeLineJoin = PenLineJoin.Round,
                                    Visibility = m_IsShowGTU ? Visibility.Visible : Visibility.Collapsed,
                                    Locations = GeoCodeCalculation.CreatePoint(
                                        (double)m_GTURadii / 1000d,
                                        new Map.Location
                                        {
                                            Latitude = gtu.Latitude,
                                            Longitude = gtu.Longitude
                                        })
                                };
                                dmapLayer.Children.Add(gtuPolygon);
                            }
                        }
                    }

                    #endregion

                    #region NdAddress

                    dmap.NdAddress.ForEach(ndaddress => 
                    {
                        var ndLocations = new Map.LocationCollection();
                        ndaddress.Locations.ForEach(i=>{
                            ndLocations.Add(new Map.Location{Longitude = i.Longitude, Latitude = i.Latitude});
                        });
                        var ndAddressPolygon = new Map.MapPolygon
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

                    mapMain.Children.Add(dmapLayer);

                    #endregion
                };
            });
            
            #endregion

            #region Address
            var addressLayer = new Map.MapLayer() { Name = "addressLayer" };
            foreach (var item in m_Campaign.Addresses)
            {
                // red star pushpin
                var location = new Map.Location { Latitude = item.Latitude, Longitude = item.Longitude };
                var position = Map.PositionOrigin.Center;
                Image flag; 
                if (string.Compare(item.Color, "green") == 0)
                {
                    flag = new Image { Source = ResourceHelper.GetBitmap("/Images/green-star.png") };
                }
                else
                {
                    flag = new Image { Source = ResourceHelper.GetBitmap("/Images/red-star.png") };
                }

                flag.Stretch = Stretch.None;
                addressLayer.AddChild(flag, location, position);

                // draw radiouse
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
            mapMain.Children.Add(addressLayer);
            #endregion

        }

        private void SwithMapLayerDisplay()
        {
            foreach (var item in mapMain.Children)
            {
                if (item is Map.MapLayer)
                {
                    var mapLayer = item as Map.MapLayer;
                    if (mapLayer.Tag != null)
                    {
                        if (mapLayer.Name.StartsWith(m_MapImage.Value.MapType.ToString()) && (int)mapLayer.Tag == m_MapImage.Value.Id)
                        {
                            mapLayer.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            mapLayer.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }

            //for dmap hide ndaddress flag
            var ndFlagLayer = mapMain.FindName("ndAddressLayer") as Map.MapLayer;
            if (ndFlagLayer != null)
            {
                if (m_MapImage.Value.MapType == ExportPageTypeEnum.DMap)
                {
                    ndFlagLayer.Visibility = IsShowNDAreasForDistributionMap == true ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    ndFlagLayer.Visibility = IsShowNDAreasForCampaignMap == true ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        bool CheckMapDisplay()
        {
            bool isShow = true;
            switch (m_MapImage.Value.MapType)
            {
                case ExportPageTypeEnum.Campaign:
                    {
                        isShow = this.IsShowCampaignSummary;
                        break;
                    }
                case ExportPageTypeEnum.SubMap:
                    {
                        isShow = this.IsShowSubMaps;
                        break;
                    }
                case ExportPageTypeEnum.DMap:
                default:
                    break;
            }

            return isShow;
        }

        void OnMapLoadedCompleted(object sender, Map.MapEventArgs e)
        {
            if (m_MapImage != null)
            {
                m_MapImage.Value.RenderSize = new Size(mapMain.Width, mapMain.Height);
                m_MapImage.Value.Center = mapMain.Center;
                m_MapImage.Value.ZoomLevel = (int)mapMain.ZoomLevel;
                m_Worker.RunWorkerAsync(m_MapImage.Value);
            }
        }

        void OnDoWork(object sender, DoWorkEventArgs e)
        {
            //var info = CurrentNode.Value;
            var info = e.Argument as MapImageInfo;

            // backgroud map image is exsit then no need download again;
            if (info.BackgroudMapImage != null)
            {
                e.Result = info;
                return;
            }


            #region Old Method
            //var imageQuery = new List<string>();
            //for (var i = 0; i < Math.Ceiling(info.RenderSize.Height / MapSplitHeight); i++)
            //{
            //    for (var j = 0; j < Math.Ceiling(info.RenderSize.Width / MapSplitWidth); j++)
            //    {
            //        //move center y position +15px for remove the bing logo image and copyright down 30px
            //        Map.Location centerLocation;
            //        Point centerPoint = new Point(j * MapSplitWidth + MapSplitWidth / 2, i * MapSplitHeight + MapSplitHeight / 2 + MapOverSize / 2);
            //        mapMain.TryViewportPointToLocation(centerPoint, out centerLocation);
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
            //        string query = string.Format("mode={0}&size={1},{2}&center={3},{4}&zoom={5}",
            //            mode,
            //            MapSplitWidth,
            //            MapSplitHeight + MapOverSize,
            //            centerLocation.Latitude.ToString("#.000000000000"),
            //            centerLocation.Longitude.ToString("#.000000000000"),
            //            info.ZoomLevel);

            //        imageQuery.Add(query);
            //    }
            //}

            //foreach (var query in imageQuery)
            //{
            //    Interlocked.Increment(ref m_ThreadCount);
            //    ThreadPool.QueueUserWorkItem(DownloadMapImage, query);
            //}
            #endregion
            

            Map.Location leftBottomLocation, rightTopLocation;
            Point leftBottomPoint = new Point(0, info.RenderSize.Height);
            Point rigthTopPoint = new Point(info.RenderSize.Width, 0);
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

            info.ImageQuery = string.Format("zoom={0}&mode={1}&leftBottom={2},{3}&rightTop={4},{5}&dpi=96",
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

            //e.Result = imageQuery;
        }

        void OnWorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
            GeneratePrintImageCache(m_MapImage.Value);
            
            m_MapImage.Value.Center = mapMain.Center;
            m_MapImage.Value.ZoomLevel = mapMain.ZoomLevel;

            if (m_MapImage.Next != null)
            {
                m_MapImage = m_MapImage.Next;

                while (this.CheckMapDisplay() == false && m_MapImage.Next != null)
                {
                    m_MapImage = m_MapImage.Next;
                }

                SwithMapLayerDisplay();
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

                PrintArea.Children.Clear();
                InitPreviewPdfPage();

        
                Loading.IsBusy = false;

            }
        }

        void OnEditMapCompleted(object sender, RunWorkerCompletedEventArgs e)
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

            PrintCover cover = new PrintCover(m_Campaign);
            cover.Visibility = IsShowCover == true ? Visibility.Visible : Visibility.Collapsed;
            PrintArea.Children.Add(cover);

            m_MapImage = m_MapImageList.First;
            var enumerator = m_MapImageList.GetEnumerator();

            while (enumerator.MoveNext())
            {
                try
                {
                    var map = enumerator.Current;
                    if (map.MapType == ExportPageTypeEnum.Campaign)
                    {
                        if (!IsShowCampaignSummary) continue;

                        #region Campaign
                        PrintCampaign campaign = new PrintCampaign(m_Campaign, map.BackgroudMapImage)
                        {
                            //Visibility = IsShowCampaignSummary ? Visibility.Visible : System.Windows.Visibility.Collapsed
                        };
                        if (m_IsShowSubMapPenetrationColors)
                        {
                            campaign.SetPercentageColor(IsCustomSubMapPenetrationColors == true ? CustomColor : m_Campaign.CampaignPercentageColors);
                        }
                        campaign.SetCenter(m_MapImage.Value.Center);
                        campaign.SetZoomLevel(m_MapImage.Value.ZoomLevel);
                        campaign.MouseLeftButtonUp += OnEditMap;
                        PrintArea.Children.Add(campaign);

                        SubMapUI[] source = m_Campaign.SubMaps.ToArray();
                        int pageSize = 44;
                        for (int i = 0; i < Math.Ceiling((double)source.Length / (double)pageSize); i++)
                        {
                            SubMapUI[] target;
                            if (m_Campaign.SubMaps.Count >= pageSize * (i + 1))
                            {
                                target = new SubMapUI[pageSize];
                                Array.Copy(source, i * pageSize, target, 0, pageSize);
                            }
                            else
                            {
                                int left = source.Length % pageSize;
                                target = new SubMapUI[left];
                                Array.Copy(source, i * pageSize, target, 0, left);
                            }
                            PrintSubMapSummary summary = new PrintSubMapSummary(m_Campaign, new List<SubMapUI>(target))
                            {
                                Visibility = IsShowSummaryOfSubmap ? Visibility.Visible : System.Windows.Visibility.Collapsed
                            };

                            PrintArea.Children.Add(summary);
                        }
                        #endregion
                    }
                    else if (map.MapType == ExportPageTypeEnum.SubMap)
                    {
                        if (!IsShowSubMaps) continue;

                        #region SubMap
                        PrintSubMap subMapPage;

                            subMapPage = new PrintSubMap(m_Campaign, map.SubMap, map.BackgroudMapImage)
                            {
                                //Visibility = IsShowSubMaps ? Visibility.Visible : System.Windows.Visibility.Collapsed
                            };

                            if (m_IsShowSubMapPenetrationColors)
                            {
                                subMapPage.SetPercentageColor(IsCustomSubMapPenetrationColors == true ? CustomColor : m_Campaign.CampaignPercentageColors);
                            }
                            subMapPage.MouseLeftButtonUp += OnEditMap;
                            subMapPage.SetCenter(map.Center);
                            subMapPage.SetZoomLevel(map.ZoomLevel);
                        
                        PrintArea.Children.Add(subMapPage);

                        //sub map detail
                        var detail = map.SubMap;
                        var detailPage = new PrintSubMapDetail(m_Campaign)
                        {
                            Visibility = IsShowSubMapDetails ? Visibility.Visible : System.Windows.Visibility.Collapsed
                        };

                        List<SubMapDetailItem> detailItems = new List<SubMapDetailItem>();
                        bool needMargin = false;
                        int index = 0;
                        int splite = 47;

                        if (detail.FiveZipAreas != null && detail.FiveZipAreas.Count > 0)
                        {
                            var caption = string.Format("ZIP CODES CONTAINED IN {0}", detail.PrintDisplayName);
                            detailPage.GenerateCaption(caption, needMargin);
                            index += 2;
                            detailPage.GenerateFiveZipsTitle();
                            index += 1;
                            int alternating = 0;
                            detail.FiveZipAreas.ForEach(i =>
                            {
                                if (index % splite == 0)
                                {
                                    PrintArea.Children.Add(detailPage);
                                    detailPage = new PrintSubMapDetail(m_Campaign)
                                    {
                                        Visibility = IsShowSubMapDetails ? Visibility.Visible : System.Windows.Visibility.Collapsed
                                    };

                                    detailPage.GenerateFiveZipsTitle();
                                    alternating = 0;
                                    index++;
                                }
                                detailPage.GenerateRow(i, alternating++);
                                index++;
                            });
                            index++;
                            needMargin = true;
                        }

                        if (detail.PremiumCRoutes != null && detail.PremiumCRoutes.Count > 0)
                        {
                            if (index > 0 && index % splite < 5)
                            {
                                index += index % splite;
                                needMargin = false;
                            }

                            var caption = string.Format("CARRIER ROUTES CONTAINED IN {0}", detail.PrintDisplayName);
                            detailPage.GenerateCaption(caption, needMargin);
                            index += 2;
                            detailPage.GenerateCRoutesTitle();
                            index += 1;
                            int alternating = 0;
                            detail.PremiumCRoutes.ForEach(i =>
                            {
                                if (index % splite == 0)
                                {
                                    PrintArea.Children.Add(detailPage);
                                    detailPage = new PrintSubMapDetail(m_Campaign)
                                    {
                                        Visibility = IsShowSubMapDetails ? Visibility.Visible : System.Windows.Visibility.Collapsed
                                    };
                                    detailPage.GenerateCRoutesTitle();
                                    alternating = 0;
                                    index++;
                                }
                                detailPage.GenerateRow(i, alternating++);
                                index++;
                            });
                            index++;
                            needMargin = true;
                        }

                        if (detail.Tracts != null && detail.Tracts.Count > 0)
                        {
                            if (index > 0 && index % splite < 5)
                            {
                                index += index % splite;
                                needMargin = false;
                            }

                            var caption = string.Format("CENSUS TRACTS CONTAINED IN {0}", detail.PrintDisplayName);
                            detailPage.GenerateCaption(caption, needMargin);
                            index += 2;
                            detailPage.GenerateTractsTitle();
                            index += 1;
                            int alternating = 0;
                            detail.Tracts.ForEach(i =>
                            {
                                if (index % splite == 0)
                                {
                                    PrintArea.Children.Add(detailPage);
                                    detailPage = new PrintSubMapDetail(m_Campaign)
                                    {
                                        Visibility = IsShowSubMapDetails ? Visibility.Visible : System.Windows.Visibility.Collapsed
                                    };
                                    detailPage.GenerateTractsTitle();
                                    alternating = 0;
                                    index++;
                                }
                                detailPage.GenerateRow(i, alternating++);
                                index++;
                            });
                            index++;
                            needMargin = true;
                        }

                        if (detail.BlockGroups != null && detail.BlockGroups.Count > 0)
                        {
                            if (index > 0 && index % splite < 5)
                            {
                                index += index % splite;
                                needMargin = false;
                            }

                            var caption = string.Format("BG'S CONTAINED IN {0}", detail.PrintDisplayName);
                            detailPage.GenerateCaption(caption, needMargin);
                            index += 2;
                            detailPage.GenerateBlockGroupsTitle();
                            index += 1;
                            int alternating = 0;
                            detail.BlockGroups.ForEach(i =>
                            {
                                if (index % splite == 0)
                                {
                                    PrintArea.Children.Add(detailPage);
                                    detailPage = new PrintSubMapDetail(m_Campaign)
                                    {
                                        Visibility = IsShowSubMapDetails ? Visibility.Visible : System.Windows.Visibility.Collapsed
                                    };
                                    detailPage.GenerateBlockGroupsTitle();
                                    alternating = 0;
                                    index++;
                                }
                                detailPage.GenerateRow(i, alternating++);
                                index++;
                            });
                            index++;
                            needMargin = true;
                        }

                        PrintArea.Children.Add(detailPage);
                        #endregion
                    }
                    else if (map.MapType == ExportPageTypeEnum.DMap)
                    {
                        #region DMap
                        PrintDMap dmap = new PrintDMap(m_Campaign, map.DMap, map.BackgroudMapImage);
                        dmap.SetCenter(map.Center);
                        dmap.SetZoomLevel(map.ZoomLevel);
                        dmap.MouseLeftButtonUp += OnEditMap;
                        PrintArea.Children.Add(dmap);
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

            }

            PrintArea.Visibility = System.Windows.Visibility.Visible;
            btnPrint.IsEnabled = true;
        }

        private void GeneratePrintImageCache(MapImageInfo info)
        {
            if (!string.IsNullOrEmpty(info.ImageQuery) && !string.IsNullOrWhiteSpace(info.FileName))
            {
                WriteableBitmap mapImage = new WriteableBitmap((int)info.RenderSize.Width, (int)info.RenderSize.Height);
                using (var isoFile = IsolatedStorageFile.GetUserStoreForApplication())
                using (var file = isoFile.OpenFile(info.FileName, FileMode.Open))
                {
                    //render background map image
                    BitmapImage mpaImageSource = new BitmapImage();
                    mpaImageSource.SetSource(file);

                    Image backgroundImage = new Image()
                    {
                        Width = info.RenderSize.Width,
                        Height = info.RenderSize.Height,
                        Stretch = Stretch.None,
                        Source = mpaImageSource,
                    };

                    mapImage.Render(backgroundImage, null);
                    mapImage.Invalidate();

                    file.Close();

                    //render address and submaps
                    foreach (var item in mapMain.Children)
                    {
                        if (item is Map.MapLayer && item.Visibility == Visibility.Visible)
                        {
                            mapImage.Render(item, null);
                            mapImage.Invalidate();
                        }

                    }

                    info.BackgroudMapImage = mapImage;

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
            public MapImageInfo(int id)
            {
                Id = id;
                MapType = ExportPageTypeEnum.Campaign;
            }

            public MapImageInfo(int id, SubMapUI subMap)
            {
                Id = id;
                SubMap = subMap;
                MapType = ExportPageTypeEnum.SubMap;
                if (subMap != null)
                {
                    Map.Location[] allLocation = new Map.Location[subMap.SubMapCoordinates.Count];
                    for (int i = 0; i < subMap.SubMapCoordinates.Count; i++)
                    {
                        allLocation[i] = new Map.Location(subMap.SubMapCoordinates[i].Latitude, subMap.SubMapCoordinates[i].Longitude);
                    }
                    BestView = new Map.LocationRect(allLocation);
                    Center = BestView.Center;
                }

            }
            public MapImageInfo(int id, DistributionMapUI dMap)
            {
                Id = id;
                DMap = dMap;
                MapType = ExportPageTypeEnum.DMap;
                if (dMap != null)
                {
                    Map.Location[] allLocation = new Map.Location[dMap.Locations.Count];
                    for (int i = 0; i < dMap.Locations.Count; i++)
                    {
                        allLocation[i] = new Map.Location(dMap.Locations[i].Latitude, dMap.Locations[i].Longitude);
                    }
                    BestView = new Map.LocationRect(allLocation);
                    Center = BestView.Center;
                }
            }
            public ExportPageTypeEnum MapType;
            public int Id;
            public Map.LocationRect BestView;
            public Map.Location Center;
            public double ZoomLevel;
            public Size RenderSize;
            public string ImageQuery = string.Empty;
            public string FileName = string.Empty;
            public SubMapUI SubMap { get; set; }
            public DistributionMapUI DMap { get; set; }
            public WriteableBitmap BackgroudMapImage { get; set; }
        }
        #endregion

        public SolidColorBrush GetPercentageColor(double value)
        {
            List<CampaignPercentageColorUI> colors = null;
            if (m_IsCustomSubMapPenetrationColors)
            {
                colors = CustomColor;
            }
            else
            {
                colors = m_Campaign.CampaignPercentageColors;
            }
            
            if (colors == null || colors.Count == 0)
            {
                //use defaut setting
                colors = new List<CampaignPercentageColorUI>()
                {
                    new CampaignPercentageColorUI{ColorId = 1,Min = 0d,Max = 0.20d},
                    new CampaignPercentageColorUI{ColorId = 2,Min = 0.20d,Max = 0.40d},
                    new CampaignPercentageColorUI{ColorId = 3,Min = 0.40d,Max = 0.60d},
                    new CampaignPercentageColorUI{ColorId = 4,Min = 0.60d,Max = 0.80d},
                    new CampaignPercentageColorUI{ColorId = 5,Min = 0.80d,Max = 1.0d}
                };
            }
            foreach (var item in colors)
            {
                if ((value == 1d && item.Max == 1d) || (value >= item.Min && value < item.Max))
                {
                    return item.FillColor;
                }
            }
            return null;
        }
    }
}
