using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;

namespace TIMM.GPS.Silverlight.Utility
{
    /// <summary>

    /// Extension class to do some extra operation with Silverlight ChildWindow.

    /// </summary>

    public static class ChildWindowExtensions
    {

        /// <summary>
        /// Centers the Silverlight ChildWindow in screen.
        /// </summary>
        /// <param name="childWindow">The child window.</param>

        public static void CenterInScreen(this ChildWindow childWindow)
        {
            var root = VisualTreeHelper.GetChild(childWindow, 0) as FrameworkElement;
            if (root == null) { return; }

            var contentRoot = root.FindName("ContentRoot") as FrameworkElement;
            if (contentRoot == null) { return; }
            contentRoot.UpdateLayout();
        }

    }
}
