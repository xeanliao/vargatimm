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
using TIMM.GPS.ControlCenter.Controls;
using TIMM.GPS.ControlCenter.Model;
using System.Windows.Media.Imaging;
using System.IO;
using TIMM.GPS.ControlCenter.ViewModel;
using TIMM.GPS.Silverlight.Utility;
using TIMM.GPS.Net.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ServiceModel.Channels;
using GalaSoft.MvvmLight.Messaging;

namespace TIMM.GPS.ControlCenter.Views
{
    public partial class EditTask : ChildWindowBase
    {
        public EditTask()
        {
            InitializeComponent();
            Messenger.Default.Register<bool>(this, CloseDialog);
            Loaded += Page_Loaded;
        }

        private void CloseDialog(bool result)
        {
            if (result)
            {
                this.DialogResult = true;
            }
        }

        void Page_Loaded(object sender, RoutedEventArgs e)
        {
             var viewModel = this.DataContext as TaskDetailViewModel;
             if (viewModel != null && viewModel.Detail != null)
             {
                 var task = viewModel.Detail;                 
             }
        }

        private static ToolTip GetToolTip(Control valdiationControl)
        {
            var child = valdiationControl.GetChildElement();
            foreach (var item in child)
            {
                var toolTip = item.GetValue(ToolTipService.ToolTipProperty) as ToolTip;
                if (toolTip != null)
                {
                    return toolTip;
                }
            }
            return null;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

