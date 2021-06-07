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
using System.Reflection;
using System.IO;
using System.Windows.Resources;
using System.Windows.Media.Imaging;
using System.Windows.Markup;

namespace TIMM.GPS.Net.Http
{
    public class ResourceHelper
    {
        public static Stream GetStream(string relativeUri, string assemblyName)
        {
            StreamResourceInfo res = Application.GetResourceStream(new Uri(string.Format("{0};component/{1}", assemblyName, relativeUri.TrimStart('/')), UriKind.Relative));
            if (res == null)
            {
                res = Application.GetResourceStream(new Uri(relativeUri, UriKind.Relative));
            }
            if (res != null)
            {
                return res.Stream;
            }
            else
            {
                return null;
            }
        }

        public static Stream GetStream(string relativeUri)
        {
            string name = Assembly.GetCallingAssembly().FullName;
            name = name.Substring(0, name.IndexOf(','));
            return GetStream(relativeUri, name);
        }

        public static BitmapImage GetBitmap(string relativeUri, string assemblyName)
        {
            Stream s = GetStream(relativeUri, assemblyName);
            if (s == null) return null;
            using (s)
            {
                BitmapImage bmp = new BitmapImage();
                bmp.SetSource(s);
                return bmp;
            }
        }

        public static BitmapImage GetBitmap(string relativeUri)
        {
            string name = Assembly.GetCallingAssembly().FullName;
            name = name.Substring(0, name.IndexOf(','));
            return GetBitmap(relativeUri, name);
        }

        public static string GetString(string relativeUri, string assemblyName)
        {
            Stream s = GetStream(relativeUri, assemblyName);
            if (s == null) return null;
            using (StreamReader reader = new StreamReader(s))
            {
                return reader.ReadToEnd();
            }
        }

        public static string GetString(string relativeUri)
        {
            string name = Assembly.GetCallingAssembly().FullName;
            name = name.Substring(0, name.IndexOf(','));
            return GetString(relativeUri, name);
        }

        public static FontSource GetFontSource(string relativeUri, string assemblyName)
        {
            Stream s = GetStream(relativeUri, assemblyName);
            if (s == null) return null;
            using (s)
            {
                return new FontSource(s);
            }
        }

        public static FontSource GetFontSource(string relativeUri)
        {
            string name = Assembly.GetCallingAssembly().FullName;
            name = name.Substring(0, name.IndexOf(','));
            return GetFontSource(relativeUri, name);
        }

        public static object GetXamlObject(string relativeUri, string assemblyName)
        {
            string str = GetString(relativeUri, assemblyName);
            if (str == null) return null;
            object obj = XamlReader.Load(str);
            return obj;
        }

        public static object GetXamlObject(string relativeUri)
        {
            string name = Assembly.GetCallingAssembly().FullName;
            name = name.Substring(0, name.IndexOf(','));
            return GetXamlObject(relativeUri, name);
        }
    }
}
