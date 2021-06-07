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
    public partial class AddCampagin : ChildWindowBase
    {
        public AddCampagin()
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
             var viewModel = this.DataContext as CampaignDetailViewModel;
             if (viewModel != null && viewModel.Detail != null)
             {
                 var campaign = viewModel.Detail;
                 if (string.IsNullOrWhiteSpace(campaign.Logo))
                 {
                     imgLogo.Visibility = Visibility.Collapsed;
                     imgLogo.Height = 0;
                 }
                 else
                 {
                     WebClient imgDownload = new WebClient();
                     string imagePath = HttpClient.CombineUrl(campaign.LogoPath);
                     imgDownload.OpenReadAsync(new Uri(imagePath));
                     imgDownload.OpenReadCompleted += (s,a) => 
                     {
                         try
                         {
                             BitmapImage loginImage = new BitmapImage();
                             loginImage.SetSource(a.Result);
                             imgLogo.Source = loginImage;
                             imgLogo.Visibility = Visibility.Visible;
                             imgLogo.Height = 128d;
                         }
                         catch
                         {

                         }
                     };
                     
                 }
             }
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Image Files|*.jpg;*.gif;*.png;*.jpeg";
            if (dialog.ShowDialog() == true)
            {
                var viewModel = this.DataContext as CampaignDetailViewModel;
                if (viewModel != null && viewModel.Detail != null)
                {
                    var campaign = viewModel.Detail;
                    using (var stream = dialog.File.OpenRead())
                    {
                        byte[] logo = new byte[stream.Length];
                        stream.Read(logo, 0, logo.Length);
                        campaign.LogoFile = logo;
                        campaign.LogoFileName = dialog.File.Name;

                        BitmapImage logoImage = new BitmapImage();
                        logoImage.SetSource(stream as Stream);

                        imgLogo.Source = logoImage;
                        imgLogo.Visibility = Visibility.Visible;
                        imgLogo.Height = 128d;
                        this.CenterInScreen();
                    }
                }
            }

        }

        //private void OKButton_Click(object sender, RoutedEventArgs e)
        //{
        //    bool haveError = false;
        //    ValidationContext context = new ValidationContext(form.DataContext, null, null);
        //    List<ValidationResult> validationResults = new List<ValidationResult>();
        //    bool valid = Validator.TryValidateObject(form.DataContext, context, validationResults, true);
        //    if (!valid)
        //    {
        //        haveError = true;
        //        List<Control> child = form.GetChildControl();

        //        ValidationSummary summary = null;
        //        var summaryResult = form.GetElementType<ValidationSummary>();
        //        if (summaryResult != null && summaryResult.Count > 0)
        //        {
        //            summary = summaryResult[0];
        //            summary.Errors.Clear();
        //        }
        //        else
        //        {
        //            summary = new ValidationSummary();
        //            form.Children.Add(summary);
        //        }

        //        foreach (ValidationResult result in validationResults)
        //        {
        //            string errorMessage = result.ErrorMessage;
        //            ValidationSummaryItem newError = new ValidationSummaryItem(errorMessage, null, ValidationSummaryItemType.ObjectError, null, null);

        //            // Indicate that this DataForm is the owner of the error.  When clearing errors, only errors from this DataForm should be cleared.
        //            newError.Context = this;
        //            foreach (string property in result.MemberNames)
        //            {
        //                // Find a control matching this property name.
        //                Control c = null;
        //                foreach (Control item in child)
        //                {
        //                    string controlType = item.GetType().ToString();
        //                    switch (controlType)
        //                    {
        //                        case "System.Windows.Controls.TextBox":
        //                            var bindingExpression = (item as TextBox).GetBindingExpression(TextBox.TextProperty);
        //                            if (bindingExpression != null && bindingExpression.ParentBinding.Path.Path == property)
        //                            {
        //                                c = item;
        //                                INotifyDataErrorInfo info = c.DataContext as INotifyDataErrorInfo;
                                        
        //                            }
        //                            break;
        //                        default:
        //                            break;
        //                    }
        //                    if (c != null)
        //                    {
        //                        break;
        //                    }
        //                }
        //                ValidationSummaryItemSource errorSource = new ValidationSummaryItemSource(property, c);
        //                newError.Sources.Add(errorSource);
        //                var express = c.GetBindingExpression(TextBox.TextProperty);
                        
                        
                        

        //                VisualStateManager.GoToState(c, "InvalidUnfocused", true);
        //            }
        //            if (!summary.Errors.Contains(newError))
        //            {
        //                summary.Errors.Add(newError);
        //            }
        //            //// Only add the errors that weren't picked up from the field-level validation.
        //            //if (this.EntityErrorShouldBeAdded(newError))
        //            //{
        //            //    this._entityLevelErrors.Add(newError);

        //            //    if (this.ValidationSummary != null)
        //            //    {
        //            //        Debug.Assert(this.ValidationSummary.Errors != null, "ValidationSummary.Errors should never be null.");

        //            //        if (!this.ValidationSummary.Errors.Contains(newError))
        //            //        {
        //            //            this.ValidationSummary.Errors.Add(newError);
        //            //        }
        //            //    }
        //            //}
        //        }

        //        //this.SetIsItemValid();
        //        //return false;
                
        //    }
        //    if (!haveError)
        //    {
        //        this.DialogResult = true;
        //    }
        //}

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

