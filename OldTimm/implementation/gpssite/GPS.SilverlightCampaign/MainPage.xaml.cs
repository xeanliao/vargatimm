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
using GPS.SilverlightCampaign.CampaignReaderSLService;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Net.Browser;
using System.Windows.Browser;
using System.ComponentModel;
using Microsoft.Maps.MapControl.Common;
using Common;

namespace GPS.SilverlightCampaign
{
    public partial class MainPage : UserControl
    {

        const string REMOVE = "Remove from submap";
        const string ADD = "Add to submap";
        const string UNABLE_ADD = "The selected shape is not connected to the selected submap group, cannot be added.";
        const string UNABLE_REMOVE = "Unable remove this area.";
        const string SUBMAP_NAME_EMPTY = "Submap's name can't be empty!";
        const string SUBMAP_SELECT_EMPTY = "Please select the submap first!";
        CampaignReaderSLServiceClient client = new CampaignReaderSLServiceClient();
        BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);

        string skey;
        int currentMode;
        string campainId;
        bool crouteLoaded = false;
        bool BGLoaded = false;
        bool IsViewChanged = false;
        ToCampaign tocampaign = new ToCampaign();
        
        public MainPage()
        {
            InitializeComponent();

            IDictionary<string, string> paras = HtmlPage.Document.QueryString;
            if (paras.Count > 0)
            {
                campainId = paras["id"];
            }
            else
            {
                //campainId = "1315146534";
                campainId = "1102562009";
            }

            //default mode:BG
            rdBGMode.IsChecked = true;

            binding.MaxReceivedMessageSize = int.MaxValue; binding.MaxBufferSize = int.MaxValue;
            client = new CampaignReaderSLServiceClient(binding, new EndpointAddress(new Uri(Application.Current.Host.Source, "../SilverlightServices/CampaignReaderSLService.svc")));

            modeDct.Add(3, new modeInfo());
            modeDct.Add(15,new modeInfo());
            modeDct.Add(0, new modeInfo());

            client.GetCampaignByIdCompleted += new EventHandler<GetCampaignByIdCompletedEventArgs>(client_GetCampaignByIdCompleted);
            client.GetCampaignByIdAsync(int.Parse(campainId));

            client.GetAreaByBoxesCompleted += new EventHandler<GetAreaByBoxesCompletedEventArgs>(client_GetAreaByBoxesCompleted);
        }


        void client_GetCampaignByIdCompleted(object sender, GetCampaignByIdCompletedEventArgs e)
        {
            //double la=myMap.Mode.BoundingRectangle.Northeast.Latitude;
            if (e.Result == null) return;
            tocampaign = CompressedSerializerSL.Decompress<ToCampaign>(e.Result);

            
            this.rdBGMode.IsChecked = tocampaign.VisibleClassifications.Contains((int)Classifications.BG);
            this.rdCrouteMode.IsChecked = tocampaign.VisibleClassifications.Contains((int)Classifications.PremiumCRoute);
 
            myMap.Center = new Location(tocampaign.Latitude, tocampaign.Longitude);
            myMap.ZoomLevel = tocampaign.ZoomLevel +1;
            //myMap.ZoomLevel = 14;
            myMap.ViewChangeEnd += new EventHandler<MapEventArgs>(myMap_ViewChangeEnd);
            ObservableCollection<ToSubMap> submaps = new ObservableCollection<ToSubMap>();
            submaps = tocampaign.SubMaps;
            foreach (ToSubMap tsm in submaps)
            {
                int cls = 0;
                Color borderColor = Color.FromArgb(255, Byte.Parse(tsm.ColorR.ToString()), Byte.Parse(tsm.ColorG.ToString()), Byte.Parse(tsm.ColorB.ToString()));
                if (tsm.SubMapRecords.Count > 0) cls = tsm.SubMapRecords[0].Classification;
                LoadSubmapAreasToDct(tsm, cls, borderColor);
                lstSubmap.Items.Add(new DisplayedItem(tsm.Id.ToString(), tsm.Name + "\r\n" + " total:" + tsm.Total + "\r\n" + " count:" + tsm.Penetration + "\r\n pen:" + (tsm.Percentage*100).ToString("N0") + "%", borderColor.ToString()));
                if (cls == 0) continue;
                if (cls == 3) 
                    BGSubmapLayer.Children.Add(DrawShape(tsm.SubMapCoordinates, new Color(), 0, borderColor, 1, 8, tsm.Id.ToString()));
                else
                    BGSubmapLayer.Children.Add(DrawShape(tsm.SubMapCoordinates, new Color(), 0, borderColor, 1, 8, tsm.Id.ToString()));
            }
            loadSubmapList();
        }

        public void loadSubmapList()
        {
            if (lstSubmap.Items.Count() > 0)
            {
                lstSubmap.SelectedIndex = 0;
                skey = (lstSubmap.SelectedItem as DisplayedItem).sid;
            }
        }

        public void LoadSubmapAreasToDct(ToSubMap tsm,int cls,Color col)
        {
            SubmapInfo si = new SubmapInfo();
            si.Col = col.ToString();
            si.sid = tsm.Id.ToString();
            si.sName = tsm.Name;
            si.stype = cls;
            //tsm.SubMapRecords.Where(ta => ta.Classification == cls).ToList().ForEach(tta => si.areaIds.Add(tta.AreaId.ToString()));
            modeDct[cls].subMaps.Add(si);
            modeDct[cls].subMapIds.Add(tsm.Id.ToString());
            List<string> AreaIdLst = new List<string>();
            tsm.SubMapRecords.Where(ta => ta.Classification == cls).ToList().ForEach(tta => AreaIdLst.Add(tta.AreaId.ToString()));
            submapAreasDct.Add(tsm.Id.ToString(), AreaIdLst);

        }

        public void RemoveSubmapAreasFromDct(ToSubMap tsm)
        {
            if (submapAreasDct.ContainsKey(tsm.Id.ToString()))
            {
                submapAreasDct.Remove(tsm.Id.ToString());
            }
        }

        private void lstSubmap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ButtonLayer.Children.Clear();
            if ((sender as ListBox).SelectedItem != null)
            {
                skey = ((sender as ListBox).SelectedItem as DisplayedItem).sid;                
            }
            
        }

        private MapPolygon DrawShape(ObservableCollection<ToCoordinate> toCoordinates, Color fillColor, double fillOpacity, Color borderColor, double borderOpacity, int borderWidth,string sName)
        {
            LocationCollection collection = new LocationCollection();
            foreach (ToCoordinate tcd in toCoordinates)
                collection.Add(new Location(tcd.Latitude, tcd.Longitude));

            MapPolygon shape = new MapPolygon();
            shape.Locations = collection;
            Color strokeColor = borderColor;
            shape.Stroke = new SolidColorBrush(strokeColor);
            shape.StrokeThickness = borderWidth;
            shape.Stroke.Opacity = borderOpacity;
            Color fColor = fillColor;
            shape.Fill = new SolidColorBrush(fColor);
            shape.Fill.Opacity = fillOpacity;
            if (sName != string.Empty)
                shape.Name = sName;
            return shape;
        }


        public void client_GetAreaByBoxesCompleted(object sender, GetAreaByBoxesCompletedEventArgs e)
        {
            double minlat, maxlat, minlon, maxlon;
            minlat = maxlat = minlon = maxlon = 0;
            if (e.Result == null) return;
            List<ToArea> result = CompressedSerializerSL.Decompress<List<ToArea>>(e.Result);
            if (result.Count == 0) return;
            Dictionary<string, ToArea> resultDct = new Dictionary<string, ToArea>();
            //resultDct = result;
            foreach (ToArea sa in result)
            {
                string key = sa.Id.ToString();
                if (currentMode == 3)
                {
                    if (!tempBGDct.ContainsKey(key))
                    {
                        tempBGDct.Add(key, sa);
                    }
                }
                else
                {
                    if (!tempCrouteDct.ContainsKey(key))
                    {
                        tempCrouteDct.Add(sa.Id.ToString(), sa);
                    }
                }

                //LocationCollection collection = new LocationCollection();
                int i = 0;
                foreach (ToCoordinate cd in sa.Locations)
                {
                    //collection.Add(new Location(cd.Latitude, cd.Longitude));
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
                    i++;
                }

                ClassificationSetting clsset = new ClassificationSetting();

                Pushpin BGPushpin = new Pushpin();
                BGPushpin.Template = (ControlTemplate)Application.Current.Resources["BGPushPinTemplate"];
                BGPushpin.Location = new Location((minlat + maxlat) / 2, (minlon + maxlon) / 2);
                BGPushpin.Name = key;
                BGPushpin.MouseLeftButtonDown += new MouseButtonEventHandler(BGPushpin_MouseLeftButtonDown);

                if (rdBGMode.IsChecked == true)
                {
                    clsset = GetClassficationSettings.getClassficationSettings(Classifications.BG);
                    MapPolygon shape = DrawShape(sa.Locations, clsset.FillColor, 0.5, clsset.LineColor, 0.2, 3, string.Empty);
                    try
                    {
                        ToolTipService.SetToolTip(BGPushpin, "BG Id:" + key);
                        BGPushpinLayer.Children.Add(BGPushpin);
                        BGShapeLayer.Children.Add(shape);
                    }
                    catch { }
                }
                else if (rdCrouteMode.IsChecked == true)
                {
                    clsset = GetClassficationSettings.getClassficationSettings(Classifications.PremiumCRoute);
                    Color FillColor = clsset.FillColor;
                    int total = 0;
                    int count = 0;
                    string penetration = "0%";
                    if (sa.Attributes.ContainsKey("Penetration"))
                    {                        
                        double per = Convert.ToDouble(sa.Attributes["Penetration"]) ;
                             //new GPS.Color(1, 'Blue', '0000FF', 0, 0, 255, 0.3),
                             //new GPS.Color(2, 'Green', '008000', 0, 128, 0, 0.3),
                             //new GPS.Color(3, 'Yellow', 'FFFF00', 255, 255, 0, 0.3),
                             //new GPS.Color(4, 'Orange', 'F75600', 247, 86, 0, 0.3),
                             //new GPS.Color(5, 'Red', 'BB0000', 187, 0, 0, 0.3)],

                        if (per == 0)
                        {

                        }
                        else if (per > 0 && per < 0.2)
                        {
                            FillColor = Colors.Blue;
                        }
                        else if (per >= 0.2 && per < 0.4)
                        {
                            FillColor = Colors.Green;
                        }
                        else if (per >= 0.4 && per < 0.6)
                        {
                            FillColor = Colors.Yellow;
                        }
                        else if (per >= 0.6 && per < 0.8)
                        {
                            FillColor = Colors.Orange;
                        }
                        else
                        {
                            FillColor = Colors.Red;
                        }

                        total = Convert.ToInt32(sa.Attributes["Total"]);
                        count = Convert.ToInt32(sa.Attributes["Count"]);
                        penetration = (Convert.ToDouble(sa.Attributes["Penetration"]) * 100).ToString("N0") + "%";
                    }

                    MapPolygon shape = DrawShape(sa.Locations, FillColor, 0.5, clsset.LineColor, 0.2, 3, string.Empty);
                    try
                    {
                        ToolTipService.SetToolTip(BGPushpin, "Croute Id:" + key + " Total:" + total + " Count:" + count + " Penetration:" + penetration );
                        CroutePushpinLayer.Children.Add(BGPushpin);
                        CrouteShapeLayer.Children.Add(shape);
                    }
                    catch { }
                }
            }
            rdBGMode.IsEnabled = true;
            rdCrouteMode.IsEnabled = true;

        }

        void BGPushpin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
           
            ButtonLayer.Children.Clear();
            
            Pushpin pp = sender as Pushpin;
            Button btn = new Button();
            btn.Height = 30;
            btn.Width = 120;
            btn.Name = pp.Name + 'b';
            
            btn.Content = ADD;
            foreach (KeyValuePair<string, List<string>> p in submapAreasDct)
            {
                if (p.Value.Contains(pp.Name))
                {
                    btn.Content = REMOVE;
                    btn.Width = 150;
                    skey = p.Key;
                    for (int i = 0; i < lstSubmap.Items.Count; i++)
                    {
                        if( (lstSubmap.Items[i] as DisplayedItem).sid == skey)
                        {
                            this.lstSubmap.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }

            MapLayer.SetPosition(btn, pp.Location);
            ButtonLayer.Children.Add(btn);
            btn.Click += new RoutedEventHandler(btn_Click);
        }


        private Color GetRandomColor()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            int seed = BitConverter.ToInt32(bytes, 0);

            byte r = GetColorRamdom(80, 256);
            byte g = GetColorRamdom(80, 256);
            byte b = GetColorRamdom(80, 256);
            Color color = Color.FromArgb(255, r, g, b);
            return color;            
        }

        private byte GetColorRamdom(int colmin, int colmax)
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            System.Random r = new Random(BitConverter.ToInt32(bytes, 0));
            return Convert.ToByte(r.Next(colmin, colmax));
        }

        public void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (this.txtName.Text.Trim() != "")
            {
                ToSubMap toSubMap = new ToSubMap();
                toSubMap.Id = Guid.NewGuid().GetHashCode();
                toSubMap.Name = this.txtName.Text.Trim();
                toSubMap.OrderId = tocampaign.SubMaps.Count + 1;
                Color c = GetRandomColor();
                toSubMap.ColorB = c.B;
                toSubMap.ColorG = c.G;
                toSubMap.ColorR = c.R;
                toSubMap.ColorString = c.ToString().Substring(1);
                toSubMap.SubMapRecords = new ObservableCollection<ToAreaRecord>();
                toSubMap.SubMapCoordinates = new ObservableCollection<ToCoordinate>();
                tocampaign.SubMaps.Add(toSubMap);

                DisplayedItem item = new DisplayedItem(toSubMap.Id.ToString(), toSubMap.Name + "\r\n" + " total:" + toSubMap.Total + "\r\n" + " count:" + toSubMap.Penetration + "\r\n pen:" + (toSubMap.Percentage * 100).ToString("N0") + "%", c.ToString());                
                lstSubmap.Items.Add(item);
                                
                LoadSubmapAreasToDct(toSubMap,currentMode,c);
                this.txtName.Text = "";
            }
            else
            {
                System.Windows.Browser.HtmlPage.Window.Alert(SUBMAP_NAME_EMPTY);
            }
        }

        public void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (this.lstSubmap.SelectedItem != null)
            {      
                lstSubmap.Items.Remove(this.lstSubmap.SelectedItem);  
                RemoveSubmapAreasFromDct(toCurrentSubmap);
                MapPolygon oshape = new MapPolygon();
                    oshape=BGSubmapLayer.FindName(skey) as MapPolygon;

                if (oshape != null)
                {
                    BGSubmapLayer.Children.Remove(oshape);
                } 
                
                tocampaign.SubMaps.Remove(toCurrentSubmap);
                                
                skey = "";
            }
            else
            {
                System.Windows.Browser.HtmlPage.Window.Alert(SUBMAP_SELECT_EMPTY);
            }
        }


        ObservableCollection<ToCoordinate> collection;
        List<string> cids;
        string alertStr;
        int sptype;
        public void btn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            ButtonLayer.Children.Clear();
            if (btn.Content.ToString() == "") return;
            string crId = btn.Name.TrimEnd('b');
            if (crId == null || crId == string.Empty) return;

            if (skey == "")
            {
                System.Windows.Browser.HtmlPage.Window.Alert(SUBMAP_SELECT_EMPTY);
                return;
            }


            collection = new ObservableCollection<ToCoordinate>();
            cids = new List<string>();
            sptype = 0;
            foreach (int dkey in modeDct.Keys)
            {
                if (modeDct[dkey].subMapIds.Contains(skey))
                    sptype = dkey;
            }
            string areaType = "BG";
            if (currentMode == 15)
                areaType = "CRoute";
            if (sptype != currentMode && sptype != 0)
            {
                string subMapType = "BG";
                if (sptype == 15)
                {
                    subMapType = "CRoute";

                }

                System.Windows.Browser.HtmlPage.Window.Alert(string.Format("It's a {0} SubMap,can't add {1} area to it", subMapType, areaType));
                return;

            }
            submapAreasDct[skey].ForEach(cid => cids.Add(cid));

            alertStr = "";
            if (btn.Content.ToString() == ADD)
            {
                if (!cids.Contains(crId))
                    cids.Add(crId);
                alertStr = UNABLE_ADD;
            }
            else
            {
                if (cids.Contains(crId))
                    cids.Remove(crId);
                alertStr = UNABLE_REMOVE;
            }
            //if (cids.Count == 0 && alertStr == UNABLE_REMOVE)
            //{
            //    try
            //    {
            //        modeDct[currentMode].subMapIds.Remove(skey);
            //        modeDct[0].subMapIds.Add(skey);
            //        MapPolygon oshape = new MapPolygon();
            //        oshape = BGSubmapLayer.FindName(skey) as MapPolygon;
            //        if (oshape != null)
            //        {
            //            BGSubmapLayer.Children.Remove(oshape);
            //        }
            //    }
            //    catch { }
            //}
            //else


            if (btn.Content.ToString() == ADD)
            {
                ToArea currentArea;
                if (tempCrouteDct.ContainsKey(crId))
                {
                    currentArea = tempCrouteDct[crId];

                    if (Convert.ToInt32(currentArea.Attributes["PartCount"]) > 1)
                    {
                        if ((currentArea.Attributes.ContainsKey("IsPartModified") && currentArea.Attributes["IsPartModified"] == "True"))
                        {

                        }
                        else
                        {
                            MultiPolygon multi = new MultiPolygon()
                            {
                                Width = 700,
                                Height = 500,
                                ActiveArea = currentArea,
                                CampaignId = campainId,
                                TempCrouteDct = tempCrouteDct,
                            };
                            multi.BindArea();
                            multi.Closed += new EventHandler(multi_Closed);
                            multi.Show();

                            return;
                        }
                    }

                }
            }

            
            


            collection = MergeAreas(cids, alertStr);
            if (cids.Count == 0)
            {
                try
                {
                    modeDct[currentMode].subMapIds.Remove(skey);
                    modeDct[0].subMapIds.Add(skey);
                }
                catch { }
            }

            if (collection != null 
                //&& collection.Count > 0
                )
            {
                if (sptype == 0)
                {
                    modeDct[sptype].subMapIds.Remove(skey);
                    modeDct[currentMode].subMapIds.Add(skey);
                }
                //Remove old submap shape
                MapPolygon oshape = new MapPolygon();
                oshape = BGSubmapLayer.FindName(skey) as MapPolygon;
                if (oshape != null)
                {
                    BGSubmapLayer.Children.Remove(oshape);
                }

                //Draw new submap shape
                MapPolygon shape = DrawShape(collection, new Color(), 0, toColor(toCurrentSubmap.ColorString), 1, 8, skey);
                shape.Name = skey;
                BGSubmapLayer.Children.Add(shape);
                submapAreasDct[skey].Clear();
                submapAreasDct[skey].AddRange(cids);

            }

            toCurrentSubmap.SubMapCoordinates = collection;
            toCurrentSubmap.SubMapRecords.Clear();


            int total = 0;
            int count = 0;
            
            foreach (string areaId in submapAreasDct[skey])
            {
                ToAreaRecord toAreaRecord = new ToAreaRecord();
                toAreaRecord.AreaId = Convert.ToInt32(areaId);
                //toAreaRecord.Classification = (int)Classifications.BG;
                toAreaRecord.Classification = currentMode;
                toAreaRecord.Value = true;
                toCurrentSubmap.SubMapRecords.Add(toAreaRecord);

                Dictionary<string, ToArea> dic = null;
                if (toAreaRecord.Classification == (int)Classifications.PremiumCRoute)
                {
                    dic = tempCrouteDct;
                }
                else
                {
                    dic = tempBGDct;
                }


                if (dic[areaId].Attributes.ContainsKey("Total"))
                {
                    total += Convert.ToInt32(dic[areaId].Attributes["Total"]);
                    count += Convert.ToInt32(dic[areaId].Attributes["Count"]);
                }
                else
                {
                    total += Convert.ToInt32(dic[areaId].Attributes["OTotal"]);
                    count += Convert.ToInt32(dic[areaId].Attributes["OTotal"]);
                }

            }

            toCurrentSubmap.Total = Convert.ToInt32(total);
            toCurrentSubmap.Penetration = Convert.ToInt32(count);
               
            if ((toCurrentSubmap.Total + toCurrentSubmap.TotalAdjustment) > 0)
            {
                toCurrentSubmap.Percentage = (double)(toCurrentSubmap.Penetration + toCurrentSubmap.CountAdjustment) / (double)(toCurrentSubmap.Total + toCurrentSubmap.TotalAdjustment);
            }
            else
            {
                toCurrentSubmap.Percentage = 0;
            }


            DisplayedItem item = null;
            int i = 0;
            for (i = 0; i < lstSubmap.Items.Count; i++)
            {
                if ((lstSubmap.Items[i] as DisplayedItem).sid == toCurrentSubmap.Id.ToString())
                {
                    item = (lstSubmap.Items[i] as DisplayedItem);
                    item.sName = toCurrentSubmap.Name + "\r\n" + " total:" + toCurrentSubmap.Total + "\r\n" + " count:" + toCurrentSubmap.Penetration + "\r\n pen:" + (toCurrentSubmap.Percentage * 100).ToString("N0") + "%";
                   lstSubmap.Items.RemoveAt(i);
                    break;
                }
            }                 
            lstSubmap.Items.Insert(i, item);
            lstSubmap.SelectedIndex = i;


        }

        protected void multi_Closed(object sender, EventArgs e)
        {
            MultiPolygon multi = (MultiPolygon)sender;
            if (multi.DialogResult == true)
            {

                ObservableCollection<ToAdjustData> list = new ObservableCollection<ToAdjustData>();
                Dictionary<string, ToArea> RelatedCrouteDct = multi.RelatedCrouteDct;
                foreach (KeyValuePair<string, ToArea> v in RelatedCrouteDct)
                {
                    if (tempCrouteDct.ContainsKey(v.Key))
                    {
                        tempCrouteDct[v.Key] = v.Value;


                        Pushpin BGPushpin = BGPushpinLayer.FindName(v.Key) as Pushpin;
                        if (BGPushpin != null)
                        {
                            string penetration = (Convert.ToDouble(v.Value.Attributes["Penetration"]) * 100).ToString("N0") + "%";
                            ToolTipService.SetToolTip(BGPushpin, "Croute Id:" + v.Value.Id + " Total:" + v.Value.Attributes["Total"] + " Count:" + v.Value.Attributes["Count"] + " Penetration:" + penetration);
                        }
                    }


                    list.Add(
                        new ToAdjustData()
                        {
                         Id = Convert.ToInt32(v.Value.Id),
                         Total = Convert.ToInt32(v.Value.Attributes["Total"]),
                         Count = Convert.ToInt32(v.Value.Attributes["Count"]),
                         PartPercentage = Convert.ToSingle(v.Value.Attributes["PartPercentage"]),
                         IsPartModified = v.Value.Attributes["IsPartModified"].ToString() == "True" ? true : false,
                       }
                    );

                }

                // save db
                client.AdjustDataAsync(Convert.ToInt32(campainId), (int)Classifications.PremiumCRoute, list);
                             

                // continue run add submap
                collection = MergeAreas(cids, alertStr);
                if (cids.Count == 0)
                {
                    try
                    {
                        modeDct[currentMode].subMapIds.Remove(skey);
                        modeDct[0].subMapIds.Add(skey);
                    }
                    catch { }
                }

                if (collection != null && collection.Count > 0)
                {
                    if (sptype == 0)
                    {
                        modeDct[sptype].subMapIds.Remove(skey);
                        modeDct[currentMode].subMapIds.Add(skey);
                    }
                    //Remove old submap shape
                    MapPolygon oshape = new MapPolygon();
                    oshape = BGSubmapLayer.FindName(skey) as MapPolygon;
                    if (oshape != null)
                    {
                        BGSubmapLayer.Children.Remove(oshape);
                    }

                    //Draw new submap shape
                    MapPolygon shape = DrawShape(collection, new Color(), 0, toColor(toCurrentSubmap.ColorString), 1, 8, skey);
                    shape.Name = skey;
                    BGSubmapLayer.Children.Add(shape);
                    submapAreasDct[skey].Clear();
                    submapAreasDct[skey].AddRange(cids);

                }

                toCurrentSubmap.SubMapCoordinates = collection;
                toCurrentSubmap.SubMapRecords.Clear();
                int total = 0;
                int count = 0;

                foreach (string areaId in submapAreasDct[skey])
                {
                    ToAreaRecord toAreaRecord = new ToAreaRecord();
                    toAreaRecord.AreaId = Convert.ToInt32(areaId);
                    //toAreaRecord.Classification = (int)Classifications.BG;
                    toAreaRecord.Classification = currentMode;
                    toAreaRecord.Value = true;
                    toCurrentSubmap.SubMapRecords.Add(toAreaRecord);

                    Dictionary<string, ToArea> dic = null;
                    if (toAreaRecord.Classification == (int)Classifications.PremiumCRoute)
                    {
                        dic = tempCrouteDct;
                    }
                    else
                    {
                        dic = tempBGDct;
                    } 

                    if (dic[areaId].Attributes.ContainsKey("Total"))
                    {
                        total += Convert.ToInt32(dic[areaId].Attributes["Total"]);
                        count += Convert.ToInt32(dic[areaId].Attributes["Count"]);
                    }
                    else
                    {
                        total += Convert.ToInt32(dic[areaId].Attributes["OTotal"]);
                        count += Convert.ToInt32(dic[areaId].Attributes["OTotal"]);
                    }
                }

                toCurrentSubmap.Total = Convert.ToInt32(total);
                toCurrentSubmap.Penetration = Convert.ToInt32(count);

                if ((toCurrentSubmap.Total + toCurrentSubmap.TotalAdjustment) > 0)
                {
                    toCurrentSubmap.Percentage = (double)(toCurrentSubmap.Penetration + toCurrentSubmap.CountAdjustment) / (double)(toCurrentSubmap.Total + toCurrentSubmap.TotalAdjustment);
                }
                else
                {
                    toCurrentSubmap.Percentage = 0;
                }


                DisplayedItem item = null;
                int i = 0;
                for (i = 0; i < lstSubmap.Items.Count; i++)
                {
                    if ((lstSubmap.Items[i] as DisplayedItem).sid == toCurrentSubmap.Id.ToString())
                    {
                        item = (lstSubmap.Items[i] as DisplayedItem);
                        item.sName = toCurrentSubmap.Name + "\r\n" + " total:" + toCurrentSubmap.Total + "\r\n" + " count:" + toCurrentSubmap.Penetration + "\r\n pen:" + (toCurrentSubmap.Percentage * 100).ToString("N0") + "%";
                        lstSubmap.Items.RemoveAt(i);
                        break;
                    }
                }
                lstSubmap.Items.Insert(i, item);
                lstSubmap.SelectedIndex = i;
            }
            
        }

        public void btnSave_Click(object sender, RoutedEventArgs e)
        {
            client.SaveCampaignCompleted += new EventHandler<AsyncCompletedEventArgs>(client_SaveCampaignCompleted);
            client.SaveCampaignAsync(tocampaign);
            this.btnSave.IsEnabled = false;

        }
        public void client_SaveCampaignCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.btnSave.IsEnabled = true;
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


        void myMap_ViewChangeEnd(object sender, MapEventArgs e)
        {
            
            List<int> activeBoxes = new List<int>();
            ObservableCollection<int> newBoxes = new ObservableCollection<int>();

            //BG
            ClassificationSetting clssetBG = GetClassficationSettings.getClassficationSettings(Classifications.BG);
            activeBoxes = GetScreenBoxesByAreaCls(clssetBG.BoxLat, clssetBG.BoxLon);

            //Croute
            ClassificationSetting clssetCR = GetClassficationSettings.getClassficationSettings(Classifications.PremiumCRoute);
            activeBoxes = GetScreenBoxesByAreaCls(clssetCR.BoxLat, clssetCR.BoxLon);

            if (activeBoxes.Count == 0) return;
            List<int> boxIdLst = new List<int>();
            try
            {
                boxIdLst.AddRange(activeBoxes.Where(box => ! modeDct[currentMode].boxIds.Contains(box)));
            }
            catch { }
            boxIdLst.ForEach(box => newBoxes.Add(box));
            boxIdLst.ForEach(box => modeDct[currentMode].boxIds.Add(box));
            if (newBoxes.Count == 0)
            {
                rdBGMode.IsEnabled = true;
                rdCrouteMode.IsEnabled = true;
                return;
            }
            if (newBoxes.Count > 0)
            {
                Classifications cls=Classifications.BG;
                if (currentMode != 3)
                    cls = Classifications.PremiumCRoute;
                client.GetAreaByBoxesAsync(newBoxes, int.Parse(campainId),cls,new ToArea());
                //client.GetAreaByBoxIdsAsync(newBoxes, cls,new SLArea());
            }
        }

        public ObservableCollection<ToCoordinate> MergeAreas(List<string> cids,string alertStr)
        {
            ObservableCollection<ToCoordinate> collection = new ObservableCollection<ToCoordinate>();
            List<double[][]> allCoordinates = new List<double[][]>();
            foreach (string cid in cids)
            {
                ObservableCollection<ToCoordinate> crlst=new ObservableCollection<ToCoordinate>();
                if (currentMode == 3)
                {
                    if (!tempBGDct.ContainsKey(cid))
                    {
                        System.Windows.Browser.HtmlPage.Window.Alert("One of the areas can't be seen,please move the map or zoom out");
                        return collection;
                    }
                    crlst = tempBGDct[cid].Locations;
                }
                else
                {
                    if (!tempCrouteDct.ContainsKey(cid))
                    {
                        System.Windows.Browser.HtmlPage.Window.Alert("One of the areas can't be seen,please move the map or zoom out");
                        return collection;
                    }
                    crlst = tempCrouteDct[cid].Locations;
                }
                int size = crlst.Count();
                double[][] ps = new double[size][];
                for (int i = 0; i < size; i++)
                {
                    ps[i] = new double[] { crlst[i].Latitude, crlst[i].Longitude };
                }
                allCoordinates.Add(ps);
            }

            if (allCoordinates.Count > 0)
            {
                List<List<Types.Loc>> locs = BoundaryFinder.find2(allCoordinates);
                if (locs.Count() > 1)
                {
                      BGSubmapLayer.Children.Clear();
                    for (int i = 0; i < locs.Count; i++)
                    {
                        ObservableCollection<ToCoordinate> col = new ObservableCollection<ToCoordinate>();

                        List<Types.Loc> list = locs[i];

                        foreach (Types.Loc l in list)
                        {
                            ToCoordinate c = new ToCoordinate();
                            c.Latitude = l.getX();
                            c.Longitude = l.getY();
                            col.Add(c);
                        }

                       
                        string str = Guid.NewGuid().ToString();
                        Color cl = Colors.White;
                        if (i % 2 == 1) cl = Colors.Green;
                        MapPolygon shape = DrawShape(col, Colors.Red, 0, cl, 1, 4, str);
                        shape.Name = str;
                        BGSubmapLayer.Children.Add(shape);
                    }
                    
                    System.Windows.Browser.HtmlPage.Window.Alert(alertStr);
                    return null;
                }
                else
                {
                    foreach (List<Types.Loc> loc in locs)
                    {
                        var len = loc.Count;
                        double[][] ps2 = new double[len][];
                        for (int i = 0; i < len; i++)
                        {
                            //collection.Add(new ToCoordinate(loc[i].getX(), loc[i].getY()));
                            collection.Add(new ToCoordinate() { Latitude = loc[i].getX(), Longitude = loc[i].getY() });
                        }
                    }
                }

            }
            return collection;
        }

        public Pushpin DrawEllipse(Location centerLoc,string name)
        {
            Pushpin elliple = new Pushpin();
            elliple.Template = Application.Current.Resources["BGPushPinTemplate"] as ControlTemplate;
            MapLayer.SetPosition(elliple, centerLoc);
            MapLayer.SetPositionOrigin(elliple, PositionOrigin.Center);
            return elliple;
        }

        public List<int> GetScreenBoxesByAreaCls(int mountLat,int mountLon)
        {
            List<int> boxes = new List<int>();
            double minLat, minLon, maxLat, maxLon;
            minLat = minLon = 0;
            try
            {
                Location minLoc = myMap.ViewportPointToLocation(new Point(0, 960));
                Location maxLoc = myMap.ViewportPointToLocation(new Point(1190, 0));

                minLat = minLoc.Latitude;
                minLon = minLoc.Longitude;
                maxLat = maxLoc.Latitude;
                maxLon = maxLoc.Longitude;

                double lat = Math.Floor(minLat * 100);
                double lon = Math.Floor(minLon * 100);
                lat = lat - (lat % mountLat);
                if (lat < 0) { lat -= mountLat; }
                lon = lon - (lon % mountLon);
                if (lon < 0) { lon -= mountLon; }

                while (lat < maxLat * 100)
                {
                    double tempLon = lon;
                    while (tempLon < maxLon * 100)
                    {
                        double id = lat * 100000 + tempLon;
                        if (!boxes.Contains(int.Parse(id.ToString())))
                        {
                            boxes.Add(int.Parse(id.ToString()));
                        }
                        tempLon += mountLon;
                    }
                    lat += mountLat;
                }
            }
            catch
            { }
            return boxes;
        }

        private void rdBGMode_Checked(object sender, RoutedEventArgs e)
        {
            rdCrouteMode.IsEnabled = false;
            CroutePushpinLayer.Visibility = Visibility.Collapsed;
            CrouteShapeLayer.Visibility = Visibility.Collapsed;
            BGShapeLayer.Visibility = Visibility.Visible;
            BGPushpinLayer.Visibility = Visibility.Visible;
            currentMode = 3;
            if (BGLoaded)
            {
                myMap.Center = new Location(myMap.Center.Latitude + 0.00000001, myMap.Center.Longitude + 0.00000001);
            }
            BGLoaded = true;
        }

        private void rdCrouteMode_Checked(object sender, RoutedEventArgs e)
        {
            rdBGMode.IsEnabled = false;
            BGShapeLayer.Visibility = Visibility.Collapsed;
            BGPushpinLayer.Visibility = Visibility.Collapsed;
            CroutePushpinLayer.Visibility = Visibility.Visible;
            CrouteShapeLayer.Visibility = Visibility.Visible;
            currentMode = 15;
            myMap.Center = new Location(myMap.Center.Latitude - 0.00000001, myMap.Center.Longitude - 0.00000001);
            crouteLoaded = true;
        }

        



        Dictionary<string, ToArea> _tempBGDct = new Dictionary<string, ToArea>();
        public Dictionary<string, ToArea> tempBGDct
        {
            get { return _tempBGDct; }
            set { _tempBGDct = value; }
        }

        Dictionary<string, ToArea> _tempCrouteDct = new Dictionary<string, ToArea>();
        public Dictionary<string, ToArea> tempCrouteDct
        {
            get { return _tempCrouteDct; }
            set { _tempCrouteDct = value; }
        }

        //List<ToCoordinate> _currentSubmapCoordinate = new List<ToCoordinate>();
        //public List<ToCoordinate> currentSubmapCoordinate
        //{
        //    get { return _currentSubmapCoordinate; }
        //    set { _currentSubmapCoordinate = value; }
        //}

        Dictionary<string, List<string>> _submapCroutesDct = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> submapCroutesDct
        {
            get { return _submapCroutesDct; }
            set { _submapCroutesDct = value; }
        }

        Dictionary<string, List<string>> _submapAreasDct = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> submapAreasDct
        {
            get { return _submapAreasDct; }
            set { _submapAreasDct = value; }
        }

        Dictionary<int, modeInfo> _modeDct = new Dictionary<int, modeInfo>();
        public Dictionary<int, modeInfo> modeDct
        {
            get { return _modeDct; }
            set { _modeDct = value; }
        }

        public ToSubMap toCurrentSubmap
        {
            get
            {
                if (skey == "") return null;
                else
                {
                    return tocampaign.SubMaps.Where<ToSubMap>(s => s.Id.ToString() == skey).First<ToSubMap>();
                }
            }
        }      

    }
    //listbox item display style
    public class DisplayedItem
    {
        public DisplayedItem(string smId,string smName, string col)
        {
            sid = smId;
            sName = smName;
            Col = col;
        }
        public string sid { get; set; }
        public string sName { get; set; }
        public string Col { get; set; }
    }

    public class SubmapInfo
    { 
        List<string> list  = new List<string>(); 
        public string sid { get; set; }
        public string sName { get; set; }
        public int stype { get; set; }
        public string Col { get; set; }
        public List<string> areaIds { get { return list; } set { list=value;} }
    }

    public class modeInfo
    {
        List<SubmapInfo> slist = new List<SubmapInfo>();
        List<int> blist = new List<int>();
        List<string> strList = new List<string>(); 
        public string mode { get; set; }
        public static ClassificationSetting BGcls
        { 
            get 
            {
                return GetClassficationSettings.getClassficationSettings(Classifications.BG);
            }
        }
        public static ClassificationSetting CRcls
        {
            get
            {
                return GetClassficationSettings.getClassficationSettings(Classifications.PremiumCRoute);
            }
        }
        public List<string> subMapIds { get { return strList; } set { strList = value; } }
        public List<SubmapInfo> subMaps { get { return slist; } set { slist = value; } }
        public List<int> boxIds { get { return blist; } set { blist = value; } }
    }
}
