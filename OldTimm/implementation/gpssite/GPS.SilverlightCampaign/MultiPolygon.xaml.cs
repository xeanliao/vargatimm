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
using GPS.SilverlightCampaign.CampaignReaderSLService;
using System.ServiceModel;
using Microsoft.Maps.MapControl.Common;
using Microsoft.Maps.MapControl;
using System.Collections.ObjectModel;
using Common;

namespace GPS.SilverlightCampaign
{
    public partial class MultiPolygon : ChildWindow
    {
        CampaignReaderSLServiceClient client = new CampaignReaderSLServiceClient();
        BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);

        public MultiPolygon()
        {   
            InitializeComponent();
            binding.MaxReceivedMessageSize = int.MaxValue; binding.MaxBufferSize = int.MaxValue;
            client = new CampaignReaderSLServiceClient(binding, new EndpointAddress(new Uri(Application.Current.Host.Source, "../SilverlightServices/CampaignReaderSLService.svc")));
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            double wholePer = 0;
            foreach(KeyValuePair<string,ToArea> v in RelatedCrouteDct)
            {
                if (v.Value.Attributes["PartPercentage"] == "0")
                {
                    System.Windows.Browser.HtmlPage.Window.Alert("Can't set percentage to 0% !");
                    return;
                }

                wholePer += Convert.ToDouble(v.Value.Attributes["PartPercentage"]);
                v.Value.Attributes["IsPartModified"] = "True";
            }

            if (wholePer != 1)
            {
                System.Windows.Browser.HtmlPage.Window.Alert("Total pecentage must be 100%!");
                return;
            }
            
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public void BindArea()
        {
           client.GetCampaignCRouteAreasCompleted += new EventHandler<GetCampaignCRouteAreasCompletedEventArgs>(client_GetCampaignCRouteAreasCompleted);
           client.GetCampaignCRouteAreasAsync(int.Parse(CampaignId),ActiveArea.Name);
        }

        protected void client_GetCampaignCRouteAreasCompleted(object sender, GetCampaignCRouteAreasCompletedEventArgs e)
        {
            if (e.Result == null) return;
            List<ToArea> crouteAreas = CompressedSerializerSL.Decompress<List<ToArea>>(e.Result);
            
            foreach (ToArea sa in crouteAreas)
            {
                sa.Attributes["SourceTotal"] = sa.Attributes.ContainsKey("SourceTotal") ? sa.Attributes["SourceTotal"].ToString() : "0";
                sa.Attributes["SourceCount"] = sa.Attributes.ContainsKey("SourceCount") ? sa.Attributes["SourceCount"].ToString() : "0";
                sa.Attributes["Penetration"] = sa.Attributes.ContainsKey("Penetration") ? sa.Attributes["Penetration"].ToString() : "0";
                
                sa.Attributes["Total"] = sa.Attributes.ContainsKey("Total") ? sa.Attributes["Total"].ToString() : "0";
                sa.Attributes["Count"] = sa.Attributes.ContainsKey("Count") ? sa.Attributes["Count"].ToString() : "0";
                sa.Attributes["PartPercentage"] = sa.Attributes.ContainsKey("PartPercentage") ? sa.Attributes["PartPercentage"].ToString() : "1";
                sa.Attributes["IsPartModified"] = sa.Attributes.ContainsKey("IsPartModified") ? sa.Attributes["IsPartModified"].ToString() : "False";

                sa.Attributes["IsChanged"] = "False";
                
                Color borderColor = Colors.Green;
                Color backGroundColor = Colors.Transparent;
                if (sa.Id == ActiveArea.Id)
                {
                    borderColor = Colors.Orange;
                    backGroundColor = Colors.Gray;
                    CurrentArea = sa;
                }

                MapPolygon shape = DrawShape(sa.Locations, backGroundColor, 0.3, borderColor, 0.8, 3, string.Empty);
                shape.MouseLeftButtonDown += new MouseButtonEventHandler(shape_MouseLeftButtonDown);
                shape.Name = sa.Id;

                TextBlock text = new TextBlock();
                text.Name = "txt_" + sa.Id;
                double per = Convert.ToDouble(sa.Attributes["PartPercentage"]);
                text.Text = (per * 100) + "%";
                text.FontWeight = FontWeights.Bold;

                ToolTipService.SetToolTip(shape, "Croute:" + sa.Name + " Count:" + sa.Attributes["Count"] + " Total:" + sa.Attributes["Total"]);
                              

                try
                {
                    CRLayer.Children.Add(shape);
                    CRTextLayer.AddChild(text,new Location(sa.Latitude, sa.Longitude) );                   
                   
                }
                catch { }

                relatedCrouteDct.Add(sa.Id, sa);
            }

            myMap.Center = new Location(ActiveArea.Latitude,ActiveArea.Longitude);
            myMap.ZoomLevel = 15;

            double percent = Convert.ToDouble(CurrentArea.Attributes["PartPercentage"]);
            this.txtPercent.Text = (percent * 100).ToString() ; 
        }

        protected void shape_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            foreach(MapPolygon pl in this.CRLayer.Children)
            {
                pl.Fill = new SolidColorBrush(Colors.Transparent);
            }


            MapPolygon shape = sender as MapPolygon;
            shape.Fill = new SolidColorBrush(Colors.Gray);
            shape.Fill.Opacity = 0.3;
            CurrentArea = RelatedCrouteDct[shape.Name];
            double per = Convert.ToDouble(CurrentArea.Attributes["PartPercentage"]);
            this.txtPercent.Text = (per * 100).ToString();            
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            if (txtPercent.Text != "")
            {                
                int per = 0;
                if (int.TryParse(this.txtPercent.Text, out per))
                { 
                    double percent = Convert.ToDouble(per) / 100;
                    CurrentArea.Attributes["Total"] = (Convert.ToInt32(CurrentArea.Attributes["SourceTotal"]) * percent).ToString("N0");
                    CurrentArea.Attributes["Count"] = (Convert.ToInt32(CurrentArea.Attributes["SourceCount"]) * percent).ToString("N0");
                    CurrentArea.Attributes["PartPercentage"] = percent.ToString();
                  
                    (CRTextLayer.FindName("txt_" + CurrentArea.Id) as TextBlock).Text = this.txtPercent.Text + "%";

                    MapPolygon shape = CRLayer.FindName(CurrentArea.Id) as MapPolygon;
                    ToolTipService.SetToolTip(shape, "Croute:" + CurrentArea.Name + " Count:" + CurrentArea.Attributes["Count"] + " Total:" + CurrentArea.Attributes["Total"]);
                }
                else
                {
                    System.Windows.Browser.HtmlPage.Window.Alert("Please input correct percentage value!");
                }
            }
        }
       


        public ToArea ActiveArea
        {
            get;
            set;
        }

        public string CampaignId
        {
            get;
            set;
        }

        Dictionary<string, ToArea> _tempCrouteDct = new Dictionary<string, ToArea>();
        public Dictionary<string, ToArea> TempCrouteDct
        {
            get { return _tempCrouteDct; }
            set { _tempCrouteDct = value; }
        }


        Dictionary<string, ToArea> relatedCrouteDct = new Dictionary<string, ToArea>();
        public Dictionary<string, ToArea> RelatedCrouteDct
        {
            get { return relatedCrouteDct; }
            set { relatedCrouteDct = value; }
        }

        public ToArea CurrentArea
        {
            get;
            set;
        }


        private MapPolygon DrawShape(ObservableCollection<ToCoordinate> toCoordinates, Color fillColor, double fillOpacity, Color borderColor, double borderOpacity, int borderWidth, string sName)
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

      

      

    }
}

