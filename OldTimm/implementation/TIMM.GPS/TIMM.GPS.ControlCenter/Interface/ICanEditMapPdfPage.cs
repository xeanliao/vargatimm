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
using Microsoft.Maps.MapControl;
using System.Windows.Media.Imaging;
using TIMM.GPS.ControlCenter.Model;
using System.Collections.Generic;

namespace TIMM.GPS.ControlCenter.Interface
{
    public interface ICanEditMapPdfPage
    {
        Location GetCenter();
        double GetZoomLevel();
        void SetCenter(Location newCenter);
        void SetZoomLevel(double newZoomLevel);
        int GetId();
        ExportPageTypeEnum GetPageType();
        void ChangeImgae(ImageSource bitmap);
        void SetPercentageColor(List<CampaignPercentageColorUI> color);
    }
}
