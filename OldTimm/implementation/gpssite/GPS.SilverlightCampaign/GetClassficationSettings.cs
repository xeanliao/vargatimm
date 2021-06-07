using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Configuration;
using GPS.SilverlightCampaign.CampaignReaderSLService;

namespace GPS.SilverlightCampaign
{
    public static class GetClassficationSettings
    {
        public static ClassificationSetting getClassficationSettings(Classifications classification)
        {
            ClassificationSetting cset = new ClassificationSetting();
            switch (classification)
            {
                case Classifications.BG:
                    cset= new ClassificationSetting(13, 19, 3, 4, Color.FromArgb(100,187,0,0), Colors.Red);
                    break;
                case Classifications.PremiumCRoute:
                    cset = new ClassificationSetting(14, 19, 3, 4, Color.FromArgb(100, 249, 150, 246), Color.FromArgb(100, 0, 255, 255));
                    break;
            }
            return cset;

        }

                   

    }
    public class ClassificationSetting
    {
        public ClassificationSetting()
        {
           
        }

        public ClassificationSetting(int minzl,int maxzl,int boxLat,int boxLon,Color fcol,Color lcol)
        {
            MinZoomLevel = minzl;
            MaxZoomLevel = maxzl;
            BoxLat = boxLat;
            BoxLon = boxLon;
            FillColor = fcol;
            LineColor = lcol;
        }
        public int MinZoomLevel { get; set; }
        public int MaxZoomLevel { get; set; }
        public int BoxLat { get; set; }
        public int BoxLon { get; set; }
        public Color FillColor { get; set; }
        public Color LineColor { get; set; }
    }
         

}
