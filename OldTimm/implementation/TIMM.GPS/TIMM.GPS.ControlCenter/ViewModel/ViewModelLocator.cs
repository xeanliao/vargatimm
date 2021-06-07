 /*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:TIMM.GPS.ControlCenter"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace TIMM.GPS.ControlCenter.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<CampaignViewModel>();
            SimpleIoc.Default.Register<DistributionMapsViewModel>();
            SimpleIoc.Default.Register<MonitorViewModel>();
            SimpleIoc.Default.Register<ReportViewModel>();
            SimpleIoc.Default.Register<AdministrationViewModel>();
            
        }

        public CampaignViewModel CampaignViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CampaignViewModel>();
            }
        }

        public DistributionMapsViewModel DistributionMapsViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DistributionMapsViewModel>();
            }
        }

        public MonitorViewModel MonitorViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MonitorViewModel>();
            }
        }

        public ReportViewModel ReportViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ReportViewModel>();
            }
        }

        public AdministrationViewModel AdministrationViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AdministrationViewModel>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}
