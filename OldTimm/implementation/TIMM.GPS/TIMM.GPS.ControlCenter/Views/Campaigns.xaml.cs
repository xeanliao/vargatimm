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
using TIMM.GPS.ControlCenter.Controls;

namespace TIMM.GPS.ControlCenter.Views
{
    public partial class Campaigns : Page
    {
        public Campaigns()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as ViewModel.CampaignViewModel;
            viewModel.QueryCommand.Execute(true);
        }
    }
}
