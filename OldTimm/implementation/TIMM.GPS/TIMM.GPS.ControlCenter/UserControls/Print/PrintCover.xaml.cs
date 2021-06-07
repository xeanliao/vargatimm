using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using TIMM.GPS.Net.Http;
using TIMM.GPS.ControlCenter.Model;
using TIMM.GPS.ControlCenter.UserControls.Print;

namespace TIMM.GPS.ControlCenter.UserControls
{
    public partial class PrintCover : PdfPageBase
    {
        public CampaignUI Campaign { get; set; }

        public PrintCover(CampaignUI campaign)
        {
            InitializeComponent();
            Campaign = campaign;
            this.DataContext = Campaign;
            PrintFooter footer = new PrintFooter(campaign);
            footer.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            this.LayoutRoot.Children.Add(footer);
            this.Loaded += Page_Loaded;
        }

        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= Page_Loaded;
            CampaignUI campaign = this.DataContext as CampaignUI;
            if (campaign != null && !string.IsNullOrWhiteSpace(campaign.LogoPath))
            {
                WebClient imgDownload = new WebClient();
                string imagePath = HttpClient.CombineUrl(campaign.LogoPath);
                imgDownload.OpenReadAsync(new Uri(imagePath));
                imgDownload.OpenReadCompleted += (s, a) =>
                {
                    try
                    {
                        Image logo = new Image();
                        BitmapImage logoImage = new BitmapImage();
                        logoImage.SetSource(a.Result);
                        logo.Source = logoImage;
                        logo.Visibility = Visibility.Visible;
                        logo.Height = 70d;
                        logo.Stretch = Stretch.Uniform;
                        imgContainer.Child = logo;
                    }
                    catch
                    {

                    }
                };
            }
        }
    }
}
