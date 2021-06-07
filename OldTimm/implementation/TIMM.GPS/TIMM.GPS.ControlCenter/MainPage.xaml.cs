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
using System.Windows.Controls.Theming;
using TIMM.GPS.Net.Http;
using System.Windows.Browser;
using TIMM.GPS.Silverlight.Utility;

namespace TIMM.GPS.ControlCenter
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            var themeUri = new Uri("/TIMM.GPS.ControlCenter;component/Assets/Themes/JetPack.xaml", UriKind.Relative);
            Theme.SetApplicationThemeUri(Application.Current, themeUri);

            InitializeComponent();

            HttpClient.ShowLoadingDialog += new EventHandler(HttpClient_ShowLoadingDialog);
            HttpClient.HidLoadingDialog += new EventHandler(HttpClient_HidLoadingDialog);
            HttpClient.ErrorHandler += new EventHandler<ApplicationUnhandledExceptionEventArgs>(HttpClient_ErrorHandler);
        }

        void HttpClient_ErrorHandler(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
#if DEBUG
            Dispatcher.BeginInvoke(() => 
            {
                MessageBox.Show(e.ExceptionObject.ToString());
            });
#endif            
        }

        void HttpClient_HidLoadingDialog(object sender, EventArgs e)
        {
            this.Loading.IsBusy = false;
        }

        void HttpClient_ShowLoadingDialog(object sender, EventArgs e)
        {
            this.Loading.IsBusy = true;
        }

        public static void ShowLoading()
        {
            var loading = (Application.Current.RootVisual as FrameworkElement).GetElementByName<BusyIndicator>("Loading");
            loading.IsBusy = true;
        }

        public static void HidLoading()
        {
            var loading = (Application.Current.RootVisual as FrameworkElement).GetElementByName<BusyIndicator>("Loading");
            loading.IsBusy = false;
        }


        // After the Frame navigates, ensure the HyperlinkButton representing the current page is selected
        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            foreach (UIElement child in LinksStackPanel.Children)
            {
                HyperlinkButton hb = child as HyperlinkButton;
                if (hb != null && hb.NavigateUri != null)
                {
                    if (ContentFrame.UriMapper.MapUri(e.Uri).ToString().Equals(ContentFrame.UriMapper.MapUri(hb.NavigateUri).ToString()))
                    {
                        VisualStateManager.GoToState(hb, "ActiveLink", true);
                    }
                    else
                    {
                        VisualStateManager.GoToState(hb, "InactiveLink", true);
                    }
                }
            }
        }

        // If an error occurs during navigation, show an error window
        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            e.Handled = true;
            //report
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            HtmlPage.Window.Navigate((sender as HyperlinkButton).NavigateUri);
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            HttpClient.PostContent("Handler/LoginHandler.ashx", "logout=true", () => {
                HtmlPage.Window.Navigate(new Uri("login.html", UriKind.Relative));
            });
        }

        private void ContentFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            var mappedUri = ContentFrame.UriMapper.MapUri(e.Uri).ToString();
            int index = mappedUri.IndexOf('?');
            if (index > -1)
            {
                mappedUri = mappedUri.Substring(0, index);
            }
            if (string.Compare(mappedUri, "/Views/DistributionMapPrint.xaml", StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                LayoutRoot.Visibility = System.Windows.Visibility.Collapsed;
                OtherAppLayout.Visibility = System.Windows.Visibility.Visible;
                Views.DistributionMapPrint window = new Views.DistributionMapPrint();
                OtherAppLayout.Children.Clear();
                OtherAppLayout.Children.Add(window);
                e.Cancel = true;
            }
            else if (string.Compare(mappedUri, "/Views/ReportPrint.xaml", StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                LayoutRoot.Visibility = System.Windows.Visibility.Collapsed;
                OtherAppLayout.Visibility = System.Windows.Visibility.Visible;
                Views.ReportPrint window = new Views.ReportPrint();
                OtherAppLayout.Children.Clear();
                OtherAppLayout.Children.Add(window);
                e.Cancel = true;
            }
            else if (string.Compare(mappedUri, "/Views/CampaignPrint.xaml", StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                LayoutRoot.Visibility = System.Windows.Visibility.Collapsed;
                OtherAppLayout.Visibility = System.Windows.Visibility.Visible;
                Views.ReportPrint window = new Views.ReportPrint(true);
                OtherAppLayout.Children.Clear();
                OtherAppLayout.Children.Add(window);
                e.Cancel = true;
            }
            else if (string.Compare(mappedUri, "/Views/DistributionMap_Snap.xaml", StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                LayoutRoot.Visibility = System.Windows.Visibility.Collapsed;
                OtherAppLayout.Visibility = System.Windows.Visibility.Visible;
                var window = new Views.DistributionMap_Snap();
                OtherAppLayout.Children.Clear();
                OtherAppLayout.Children.Add(window);
                e.Cancel = true;
            }
            else
            {
                OtherAppLayout.Children.Clear();
                OtherAppLayout.Visibility = System.Windows.Visibility.Collapsed;
                LayoutRoot.Visibility = System.Windows.Visibility.Visible;
            }
        }
    }
}
