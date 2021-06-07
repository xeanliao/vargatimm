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
using System.IO;

namespace TIMM.GPS.Net.Http
{
    public class RequestArgs
    {
        public object Parameter { get; private set; }

        public PostTypeEnum PostType { get; private set; }

        public HttpWebRequest Request { get; private set; }

        public Action Callback { get; private set; }

        public bool ShowLoading { get; private set; }

        public RequestArgs(HttpWebRequest request, 
            object paramter = null, 
            Action callback = null, 
            bool showLoading = true,
            PostTypeEnum postType = PostTypeEnum.Json)
        {
            this.Request = request;
            this.Parameter = paramter;
            this.Callback = callback;
            this.ShowLoading = showLoading;
            this.PostType = postType;
        }        
    }

    public class RequestArgs<T>
    {
        public object Parameter { get; private set; }

        public HttpWebRequest Request { get; private set; }

        public PostTypeEnum PostType { get; private set; }

        public Action<ResultArgs<T>> Callback { get; private set; }

        public bool ShowLoading { get; private set; }

        public bool WithZip { get; set; }

        public RequestArgs(HttpWebRequest request, 
            object paramter = null, 
            Action<ResultArgs<T>> callback = null,
            bool withZip = false,
            bool showLoading = true,
            PostTypeEnum postType = PostTypeEnum.Json)
        {
            this.Request = request;            
            this.Parameter = paramter;
            this.Callback = callback;
            this.WithZip = withZip;
            this.ShowLoading = showLoading;
            this.PostType = postType;
        }

        
    }
}
