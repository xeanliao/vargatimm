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

namespace TIMM.GPS.ControlCenter.Views
{
    public partial class AssignUser : ChildWindowBase
    {
        public AssignUser(string title, object dataContext)
        {
            this.Title = title;
            this.DataContext = dataContext;
            InitializeComponent();
            
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public int? SelectedUser
        {
            get
            {
                return (int?)listUser.SelectedValue;
            }
        }
    }
}

