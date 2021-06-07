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
using System.Windows.Browser;

namespace TIMM.GPS.Helper
{
    public class ScriptHelper
    {
        public static void OpenWindow(string relativePath, string windowName)
        {
            var option = new HtmlPopupWindowOptions
            {
                Resizeable = true,
                Status = true,
                Toolbar = false,
                Menubar = false,
                Location = false
            };
            string url = CombineUrl(relativePath);
            HtmlPage.PopupWindow(new Uri(url), windowName, option);
        }

        private static string CombineUrl(string relativeUrl)
        {
            
            if (relativeUrl.StartsWith("http") || relativeUrl.StartsWith("https"))
            {
                return relativeUrl;
            }

            var root = HtmlPage.Document.DocumentUri.AbsoluteUri;
            if (!string.IsNullOrEmpty(HtmlPage.Document.DocumentUri.Fragment))
            {
                root = root.Replace(HtmlPage.Document.DocumentUri.Fragment, "");
            }


            var index = root.LastIndexOf(@"/");
            root = root.Substring(0, index);

            return String.Format("{0}/{1}", 
                root.TrimEnd(new char[] { '/' }), 
                relativeUrl.TrimStart(new char[] { '/', '\\' }));
        }
    }
}
