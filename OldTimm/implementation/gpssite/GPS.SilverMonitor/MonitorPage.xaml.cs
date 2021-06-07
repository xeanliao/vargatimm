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
using System.Windows.Navigation;


using System.ServiceModel.DomainServices.Client;
using Microsoft.Maps.MapControl;
using System.Windows.Browser;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Threading;

namespace GPS.SilverMonitor
{
    public partial class MonitorPage : Page
    {
        private GPS.Website.DAL.distributionmap mDistributionMap = null;
        private int mTaskID;
        private List<Website.DAL.ViewGtuInTask> mGtuList = new List<Website.DAL.ViewGtuInTask>();
        Int64 mLastGtuInfoID = 0;     // Every minutes, we retrieve GtuInfo that after the ID
        long tickes = 0;


        System.Windows.Threading.DispatcherTimer timerNewGtuInfo = new System.Windows.Threading.DispatcherTimer();

        public MonitorPage(int taskId)
        {
            InitializeComponent();
            this.map1.CredentialsProvider = new ApplicationIdCredentialsProvider(App.MapKey);
            try
            {
                // enable Javascript call
                HtmlPage.RegisterScriptableObject("monitorTaskGtus", this);
                //MessageBox.Show("loading");

                // Get TaskID from QueryString
                //string sTaskID = HtmlPage.Document.QueryString["taskid"];
                //this.mTaskID = Convert.ToInt32(sTaskID);
                this.mTaskID = taskId;

                // Load Address
                GPS.Website.DAL.TimmGtuDomainContext contextGtu = new Website.DAL.TimmGtuDomainContext();
                contextGtu.Load(contextGtu.GetDistributionMapByTaskIDQuery(mTaskID), DMap_Loaded, true);

                // Call GetGtusInTask
                contextGtu.Load(contextGtu.GetGtusInTaskQuery(mTaskID), GtusInTask_Loaded, true);

                // Start timer, so we add new GTU dots every minuts
                timerNewGtuInfo.Interval = new TimeSpan(0, 1, 0);
                timerNewGtuInfo.Tick += new EventHandler(timerNewGtuInfo_Tick);
                // timerNewGtuInfo.Start();
            }
            catch (Exception)
            { 
            }
        }

        [ScriptableMember]
        public void Call_ShowMonitorAddess()
        {
            try
            {
                this.AddressLayer.Children.Clear();
                GPS.Website.DAL.TimmGtuDomainContext context = new Website.DAL.TimmGtuDomainContext();
                context.Load(context.GetMonitorAddressListByTaskIDQuery(mTaskID), MonitorAddress_Loaded, true);
            }
            catch (Exception)
            { 
            }
        }

        [ScriptableMember]
        public void Call_ShowGtuLocations(int iSHowHistory, string result)
        {
            tickes = 0;
            try
            {
                string[] gtuIDs = result.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                GtuLayer.Children.Clear();

                if (iSHowHistory == 0)
                {
                    foreach (string gtuid in gtuIDs)
                    {
                        showGtuCurrentLocation(Convert.ToInt32(gtuid));
                    }
                }
                else
                {
                    List<int> ids = new List<int>();
                    foreach (string gtuid in gtuIDs)
                    {
                        int id;
                        if (int.TryParse(gtuid, out id))
                        {
                            ids.Add(id);
                        }
                    }
                    showGtuHistory(ids);
                }
            }
            catch
            { 
            }
        }

        // Executes when the user navigates to this page.
        private void DMap_Loaded(LoadOperation op)
        {
            try
            {
                foreach (GPS.Website.DAL.distributionmap d in op.Entities)
                {
                    this.mDistributionMap = d;
                    break;
                }

                if (mDistributionMap == null)
                {
                    MessageBox.Show("Distribution Map is not loaded");
                    return;
                }

                // Load DMap Area
                GPS.Website.DAL.TimmGtuDomainContext timm = new Website.DAL.TimmGtuDomainContext();
                timm.Load(timm.GetDistributionmapCoordinatesByTaskIDQuery(mTaskID),
                    DistributionCoordinates_Loaded, true);

                // Load Addresses
                timm.Load(timm.GetMonitorAddressListByDMapIDQuery(mDistributionMap.Id),
                    MonitorAddress_Loaded, true);
            }
            catch (Exception)
            { 
            }
        }

        private void DistributionCoordinates_Loaded(LoadOperation op)
        {
            try
            {
                int iTotal = op.Entities.Count();

                Color dmapColor = Color.FromArgb(255,
                    Convert.ToByte(this.mDistributionMap.ColorR),
                    Convert.ToByte(mDistributionMap.ColorG),
                    Convert.ToByte(mDistributionMap.ColorB));

                MapPolygon polygon = new MapPolygon();
                polygon.Fill = new System.Windows.Media.SolidColorBrush(dmapColor);
                polygon.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Yellow);
                polygon.StrokeThickness = 2;
                polygon.Opacity = 0.5;

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
                map1.ZoomLevel = 12;
                map1.Center = new Location(dY, dX);

                // Draw DMap
                LocationCollection locations = new LocationCollection();
                foreach (GPS.Website.DAL.distributionmapcoordinate p in op.Entities)
                {
                    locations.Add(new Location(p.Latitude, p.Longitude));
                }
                polygon.Locations = locations;
                DMLayer.Children.Add(polygon);

                // load NdAddress
                LoadNdAddress();
                LoadCustomerArea();
            }
            catch (Exception)
            { }
        }

        private void MonitorAddress_Loaded(LoadOperation op)
        {
            try
            {
                foreach (GPS.Website.DAL.monitoraddress addr in op.Entities)
                {
                    Image image = new Image();
                    //Define the URI location of the image
                    image.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("star.png", UriKind.Relative));
                    //Define the image display properties
                    image.Opacity = 0.8;
                    image.Stretch = System.Windows.Media.Stretch.None;
                    //The map location to place the image at
                    Location location = new Location(addr.Latitude, addr.Longitude);
                    //Center the image around the location specified
                    PositionOrigin position = PositionOrigin.Center;

                    //Add the image to the defined map layer
                    //this.DMLayer.AddChild(image, location, position);

                    Pushpin pin = new Pushpin();
                    pin.Location = location;
                    pin.Name = addr.Address + ", " + addr.ZipCode;
                    ToolTipService.SetToolTip(pin, pin.Name);
                    // ContextMenuService.SetContextMenu(pin, null);

                    /*
                    ScaleTransform st = new ScaleTransform();
                    st.ScaleX = 0.25;
                    st.ScaleY = 0.25;
                    st.CenterX = (pin as FrameworkElement).Height / 2;
                    st.CenterY = (pin as FrameworkElement).Height / 2;
                    pin.RenderTransform = st;
                    pin.Background = new SolidColorBrush(Colors.Blue);
                    */
                    try
                    {
                        this.AddressLayer.AddChild(pin, location);
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception)
            { }
        }

        private void GtusInTask_Loaded(LoadOperation op)
        {
            try
            {
                foreach (GPS.Website.DAL.ViewGtuInTask g in op.Entities)
                {
                    this.mGtuList.Add(g);
                    showGtuCurrentLocation(g.GtuID);
                }
            }
            catch (Exception)
            { }
        }

        private void showGtuCurrentLocation(int gtuID)
        {
            try
            {
                GPS.Website.DAL.TimmGtuDomainContext context = new Website.DAL.TimmGtuDomainContext();
                context.Load(context.GetGtuInfoByGtuIDQuery(gtuID), GtuLocation_Loaded, true);
            }
            catch (Exception)
            { }
        }

        private static int m_GtuHistoryPageSize = 1000;
        private static DispatcherTimer[] m_HistoryQueryTimer = null;

        private void showGtuHistory(List<int> gtuID)
        {
            try
            {
                if (m_HistoryQueryTimer != null && m_HistoryQueryTimer.Length > 0)
                {
                    foreach (var timer in m_HistoryQueryTimer)
                    {
                        if (timer != null)
                        {
                            timer.Stop();
                        }
                    }
                    m_HistoryQueryTimer = null;
                }

                GPS.Website.DAL.TimmGtuDomainContext context = new Website.DAL.TimmGtuDomainContext();
                context.GetGtuHistoryPageSizeByGtuID(gtuID.ToArray(), 
                    (result) => {
                        int pageSize = (int)Math.Ceiling((double)result.Value / (double)m_GtuHistoryPageSize);
                        m_HistoryQueryTimer = new DispatcherTimer[pageSize];
                        int index = 0;
                        for (int i = 0; i < pageSize; i++)
                        {
                            m_HistoryQueryTimer[i] = new DispatcherTimer();
                            m_HistoryQueryTimer[i].Tick += (o, s) =>
                            {
                                var timer = o as DispatcherTimer;
                                if (timer != null)
                                {
                                    timer.Stop();
                                }
                                context.Load(context.PagedBatchGetGtuHistoryByGtuIDQuery(index++, m_GtuHistoryPageSize, gtuID.ToArray()), HistoryGtuLoaded, null);

                            };
                            m_HistoryQueryTimer[i].Interval = new TimeSpan(m_GtuHistoryPageSize * 1000000 * i);
                            m_HistoryQueryTimer[i].Start();
                        }
                    }, null);
            }
            catch (Exception e)
            {
                //txtMessage.Text = e.ToString();
            }
        }

        private void HistoryGtuLoaded(LoadOperation operation)
        {
            
            try
            {
                var gtuList = mGtuList.ToDictionary(i => i.GtuID);
                long tickes = 0;
                foreach (GPS.Website.DAL.ViewGtuLocation info in operation.Entities)
                {
                    if (info.GTUId.HasValue && gtuList.ContainsKey(info.GTUId.Value))
                    {
                        Website.DAL.ViewGtuInTask gtu = gtuList[info.GTUId.Value];
                        Color dotColor = this.GetColorByHex(gtu.UserColor);
                        drawGtuDot(gtu.UserRoleID, (double)info.dwLatitude, (double)info.dwLongitude, dotColor, tickes);
                        tickes += 100;
                        mLastGtuInfoID = Math.Max(mLastGtuInfoID, info.Id);
                    }                    
                }
                tickes = 0;
            }
            catch (Exception)
            {

            }
        }


        private void GtuLocation_Loaded(LoadOperation op)
        {
            try
            {
                int iGtuID = Convert.ToInt32(op.EntityQuery.Parameters["iGtuID"]);
                Website.DAL.ViewGtuInTask gtu = mGtuList.Where(it => it.GtuID == iGtuID).FirstOrDefault();
                if (gtu == null)
                    return;
                Color dotColor = this.GetColorByHex(gtu.UserColor);
                foreach (GPS.Website.DAL.ViewGtuLocation info in op.Entities)
                {
                    drawGtuDot(gtu.UserRoleID, (double)info.dwLatitude, (double)info.dwLongitude, dotColor, tickes);
                    
                    if (info.Id > mLastGtuInfoID)
                    {
                        mLastGtuInfoID = info.Id;
                    }
                }
            }
            catch (Exception )
            { 
            
            }
        }
 
        private void drawGtuDot(int userRoleID, double latitude, double longitude, Color dotColor, long ticks = 1)
        {
            Location location = new Location(latitude, longitude);
            PositionOrigin position = PositionOrigin.Center;
            DelayShowElement gtuElement = new DelayShowElement(ticks * 10000);

            switch (userRoleID)
            {
                case 48:
                case 50:
                    Image car = new Image();
                    car.Width = 16;
                    car.Height = 16;
                    //Define the URI location of the image
                    if (userRoleID == 48)
                        car.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("car.png", UriKind.Relative));
                    if (userRoleID == 50)
                        car.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri("user_green.png", UriKind.Relative));
                    //Define the image display properties
                    car.Opacity = 0.8;
                    car.Stretch = System.Windows.Media.Stretch.None;

                    gtuElement.Content = car;
                    
                    break;

                default:
                    // car
                    Ellipse dot = new Ellipse();
                    dot.Width = 6;
                    dot.Height = 6;
                    dot.Fill = new SolidColorBrush(dotColor);
                    gtuElement.Content = dot;
                    break;
            }
            //Add the image layer to the map
            GtuLayer.AddChild(gtuElement, location, position);
        }

        /*
        private void ShowGtu(string sDot, double latitude, double longitude)
        {
            Image image = new Image();
            //Define the URI location of the image
            image.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(sDot, UriKind.Relative));
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
        */
        private Color GetColorByHex(string hexColor)
        {
            hexColor = hexColor.Replace("#", "");
            try
            {
                byte iRed = Convert.ToByte(hexColor.Substring(0, 2), 16);
                byte iGreen = Convert.ToByte(hexColor.Substring(2, 2), 16);
                byte iBlue = Convert.ToByte(hexColor.Substring(4, 2), 16);
                return Color.FromArgb(255, iRed, iGreen, iBlue);
            }
            catch (Exception)
            {
                return Colors.Blue;
            }
        }

        private void timerNewGtuInfo_Tick(object sender, EventArgs e)
        {
            try
            {
                // Get all GTUs, Get all new dots
                if (mLastGtuInfoID <= 0) return;
                if (this.mGtuList.Count <= 0) return;

                Website.DAL.TimmGtuDomainContext context = new Website.DAL.TimmGtuDomainContext();
                foreach (Website.DAL.ViewGtuInTask gtu in mGtuList)
                {
                    //context.Load(context.GetGtuInfoQuery(iGtuID), op);
                    context.Load(context.GetNewGtuInfoQuery(gtu.GtuID, mLastGtuInfoID), GtuLocation_Loaded, false);
                }
                txtMessage.Text = DateTime.Now.ToShortTimeString();
            }
            catch (Exception)
            { }
        }

        private void LoadNdAddress()
        {
            try
            {
                GPS.Website.DAL.TimmGtuDomainContext context = new Website.DAL.TimmGtuDomainContext();
                context.GetNdAddressByTask(mTaskID, operation =>
                    {
                        //TODO: check the operation object for errors or cancellation
                        var ids = operation.Value; // here's your value
                        foreach (int id in ids)
                        {
                            context.Load(context.GetNdAddressCoordinatesQuery(id), NdAddressCoordinates_Loaded, false);
                        }
                        //TODO: add the code that performs further actions
                    },
                    null
                );
            }
            catch (Exception)
            { }
        }

        private void LoadCustomerArea()
        {
            try
            {
                GPS.Website.DAL.TimmGtuDomainContext context = new Website.DAL.TimmGtuDomainContext();
                context.GetCustomAreaByTask(mTaskID, operation =>
                {
                    //TODO: check the operation object for errors or cancellation
                    var ids = operation.Value; // here's your value
                    foreach (int id in ids)
                    {
                        context.Load(context.GetCustomAreaCoordinatesQuery(id), CustomAreaCoordinates_Loaded, false);
                    }
                    //TODO: add the code that performs further actions
                },
                    null
                );
            }
            catch (Exception)
            { }
        }

        private void NdAddressCoordinates_Loaded(LoadOperation op)
        {
            try
            {
                MapPolygon polygon = new MapPolygon();
                polygon.Fill = new SolidColorBrush(Colors.Red);
                polygon.Stroke = new SolidColorBrush(Colors.Red);
                polygon.StrokeThickness = 1;
                polygon.Opacity = 0.5;

                // Draw DMap
                polygon.Locations = new LocationCollection();
                foreach (GPS.Website.DAL.ndaddresscoordinate p in op.Entities)
                {
                    polygon.Locations.Add(new Location(p.Latitude, p.Longitude));
                }
                DMLayer.Children.Add(polygon);
            }
            catch (Exception)
            { }
        }

        private void CustomAreaCoordinates_Loaded(LoadOperation op)
        {
            try
            {
                MapPolygon polygon = new MapPolygon();
                polygon.Fill = new SolidColorBrush(Colors.Red);
                polygon.Stroke = new SolidColorBrush(Colors.Red);
                polygon.StrokeThickness = 1;
                polygon.Opacity = 0.5;

                // Draw DMap
                polygon.Locations = new LocationCollection();
                foreach (GPS.Website.DAL.customareacoordinate p in op.Entities)
                {
                    polygon.Locations.Add(new Location(p.Latitude, p.Longitude));
                }
                DMLayer.Children.Add(polygon);
            }
            catch (Exception)
            { }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.Call_ShowGtuLocations(1, "1994979743");
        }

    } // end of class
}
