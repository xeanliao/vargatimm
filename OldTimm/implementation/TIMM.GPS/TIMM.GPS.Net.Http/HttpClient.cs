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
using Newtonsoft.Json;
using System.Text;
using System.IO;
using Newtonsoft.Json.Converters;
using System.Net.Browser;
using TIMM.GPS.Silverlight.Utility;
using System.Threading;
using System.Windows.Threading;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;

namespace TIMM.GPS.Net.Http
{
    public static class HttpClient
    {
        private static long m_RequestCount = 0;

        public static string CombineUrl(string relativeUrl)
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

        public static event EventHandler ShowLoadingDialog;

        public static event EventHandler HidLoadingDialog;

        public static event EventHandler<ApplicationUnhandledExceptionEventArgs> ErrorHandler;

        #region Send Josn
        public static void Get(string relativeUrl, Action callback,  bool showLoading = true)
        {
            if (showLoading)
            {
                ShowLoading();
            }
            string url = CombineUrl(relativeUrl);
            HttpWebRequest request = (HttpWebRequest)WebRequestCreator.BrowserHttp.Create(new Uri(url));
            request.Method = "GET";
            request.Accept = "application/json";
            
            RequestArgs args = new RequestArgs(request, null, callback, showLoading);

            request.BeginGetResponse(OnResponse, args);
        }

        public static void Post(string relativeUrl, object paramter, Action callback, bool showLoading = true)
        {
            if (showLoading)
            {
                ShowLoading();
            }
            string url = CombineUrl(relativeUrl);
            HttpWebRequest request = (HttpWebRequest)WebRequestCreator.BrowserHttp.Create(new Uri(url));
            request.Method = "POST";
            request.Accept = "application/json";
            request.ContentType = "application/json";
            request.AllowReadStreamBuffering = false;
            RequestArgs args = new RequestArgs(request, paramter, callback, showLoading);

            request.BeginGetRequestStream(OnRequest, args);
        }

        public static void Get<T>(string relativeUrl, Action<ResultArgs<T>> callback, bool withZip = false, bool showLoading = true)
        {
            if (showLoading)
            {
                ShowLoading();
            }
            string url = CombineUrl(relativeUrl);
            HttpWebRequest request = (HttpWebRequest)WebRequestCreator.BrowserHttp.Create(new Uri(url));  
            request.Method = "GET";
            if (withZip)
            {
                request.Accept = "application/zipjson";
            }
            else
            {
                request.Accept = "application/json";
            }
            
            RequestArgs<T> args = new RequestArgs<T>(request, null, callback, showLoading);
            
            request.BeginGetResponse(OnResponse<T>, args);
        }

        public static void Post<T>(string relativeUrl, object paramter, Action<ResultArgs<T>> callback, bool withZip = false, bool showLoading = true)
        {
            if (showLoading)
            {
                ShowLoading();
            }
            string url = CombineUrl(relativeUrl);
            HttpWebRequest request = (HttpWebRequest)WebRequestCreator.BrowserHttp.Create(new Uri(url));  
            request.Method = "POST";
            if (withZip)
            {
                request.Accept = "application/zipjson";
            }
            else
            {
                request.Accept = "application/json";
            }
            request.ContentType = "application/json";
            request.AllowReadStreamBuffering = false;
            RequestArgs<T> args = new RequestArgs<T>(request, paramter, callback, showLoading);

            request.BeginGetRequestStream(OnRequest<T>, args);
        }
        #endregion

        #region Post File
        public static void PostFile<T>(string relativeUrl, string fileName, byte[]imageFile, Action<ResultArgs<T>> callback, bool showLoading = true)
        {
            if (showLoading)
            {
                ShowLoading();
            }
            string url = CombineUrl(relativeUrl);
            HttpWebRequest request = (HttpWebRequest)WebRequestCreator.BrowserHttp.Create(new Uri(url));
            request.Method = "POST";
            request.Accept = "application/json";
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
            request.AllowReadStreamBuffering = false;
            RequestArgs<T> args = null;
            using(MemoryStream stream = new MemoryStream())
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Format("--{0}", boundary));
                sb.AppendLine(string.Format("Content-Disposition: form-data; name=\"FileUploader\"; filename=\"{0}\"", fileName));
                sb.AppendLine("Content-Type: image/jpeg");
                sb.AppendLine();
                byte[] begin = Encoding.UTF8.GetBytes(sb.ToString());
                sb.Clear();
                sb.AppendLine();
                sb.AppendLine(string.Format("--{0}--", boundary));
                byte[] end = Encoding.UTF8.GetBytes(sb.ToString());

                stream.Write(begin, 0, begin.Length);
                stream.Write(imageFile, 0, imageFile.Length);
                stream.Write(end, 0, end.Length);

                byte[] postData = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(postData, 0, postData.Length);
                args = new RequestArgs<T>(request, postData, callback, false, showLoading, PostTypeEnum.File);
            }
            

            request.BeginGetRequestStream(OnRequest<T>, args);
        }

        public static void PostFile(string relativeUrl, string fileName, byte[] imageFile, Action callback, bool showLoading = true)
        {
            if (showLoading)
            {
                ShowLoading();
            }
            string url = CombineUrl(relativeUrl);
            HttpWebRequest request = (HttpWebRequest)WebRequestCreator.BrowserHttp.Create(new Uri(url));
            request.Method = "POST";
            request.Accept = "application/json";
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
            request.AllowReadStreamBuffering = false;
            RequestArgs args = null;
            using (MemoryStream stream = new MemoryStream())
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Format("--{0}", boundary));
                sb.AppendLine(string.Format("Content-Disposition: form-data; name=\"FileUploader\"; filename=\"{0}\"", fileName));
                sb.AppendLine("Content-Type: image/jpeg");
                sb.AppendLine();
                byte[] begin = Encoding.UTF8.GetBytes(sb.ToString());
                sb.Clear();
                sb.AppendLine();
                sb.AppendLine(string.Format("--{0}--", boundary));
                byte[] end = Encoding.UTF8.GetBytes(sb.ToString());

                stream.Write(begin, 0, begin.Length);
                stream.Write(imageFile, 0, imageFile.Length);
                stream.Write(end, 0, end.Length);

                byte[] postData = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(postData, 0, postData.Length);
                args = new RequestArgs(request, postData, callback, showLoading, PostTypeEnum.File);
            }


            request.BeginGetRequestStream(OnRequest, args);
        }
        #endregion

        #region Post Content
        public static void PostContent(string relativeUrl, string content, Action callback, bool showLoading = true)
        {
            if (showLoading)
            {
                ShowLoading();
            }
            string url = CombineUrl(relativeUrl);
            HttpWebRequest request = (HttpWebRequest)WebRequestCreator.BrowserHttp.Create(new Uri(url));
            request.Method = "POST";
            request.Accept = "application/json";
            request.ContentType = "application/json";
            request.AllowReadStreamBuffering = false;
            RequestArgs args = new RequestArgs(request, content, callback, showLoading, PostTypeEnum.Content);

            request.BeginGetRequestStream(OnRequest, args);
        }

        public static void PostContent<T>(string relativeUrl, string content, Action<ResultArgs<T>> callback, bool showLoading = true)
        {
            if (showLoading)
            {
                ShowLoading();
            }
            string url = CombineUrl(relativeUrl);
            HttpWebRequest request = (HttpWebRequest)WebRequestCreator.BrowserHttp.Create(new Uri(url));
            request.Method = "POST";
            request.Accept = "application/json";
            request.ContentType = "application/json";
            request.AllowReadStreamBuffering = false;
            RequestArgs<T> args = new RequestArgs<T>(request, content, callback, false, showLoading, PostTypeEnum.Content);

            request.BeginGetRequestStream(OnRequest<T>, args);
        }
        #endregion

        private static void OnRequest(IAsyncResult result)
        {
            var args = result.AsyncState as RequestArgs;
            var request = args.Request;
            using (var stream = request.EndGetRequestStream(result))
            {
                switch (args.PostType)
                {
                    case PostTypeEnum.Content:
                        {
                            byte[] content = Encoding.UTF8.GetBytes(args.Parameter.ToString());
                            stream.Write(content, 0, content.Length);
                            stream.Flush();
                            break;
                        }
                    case PostTypeEnum.File:
                        {
                            byte[] content = args.Parameter as byte[];
                            stream.Write(content, 0, content.Length);
                            stream.Flush();
                            break;
                        }
                    default:
                        {
                            using (StreamWriter sw = new StreamWriter(stream))
                            using (JsonWriter writer = new JsonTextWriter(sw))
                            {
                                JsonSerializer serializer = new JsonSerializer();
                                serializer.Converters.Add(new StringEnumConverter());
                                serializer.Converters.Add(new IsoDateTimeConverter());
                                serializer.NullValueHandling = NullValueHandling.Ignore;
                                serializer.DefaultValueHandling = DefaultValueHandling.Ignore;

                                serializer.Serialize(writer, args.Parameter);
                                stream.Flush();
                            }
                            break;
                        }
                }
            }

            request.BeginGetResponse(OnResponse, args);
        }

        private static void OnResponse(IAsyncResult result)
        {
            RequestArgs args = result.AsyncState as RequestArgs;
            var callback = args.Callback;
            HttpWebResponse response = null;
            Exception error = null;
            try
            {
                if (args.Request.HaveResponse)
                {
                    response = (HttpWebResponse)args.Request.EndGetResponse(result);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        if (callback != null && error == null)
                        {
                            Deployment.Current.Dispatcher.BeginInvoke(
                                () =>
                                {
                                    callback();
                                });
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                error = ex;
                if (ErrorHandler != null)
                {
                    ErrorHandler(null, new ApplicationUnhandledExceptionEventArgs(ex, false));
                }
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }

            }
            if (args.ShowLoading)
            {
                Interlocked.Decrement(ref m_RequestCount);
                Deployment.Current.Dispatcher.BeginInvoke(
                    () =>
                    {
                        var timer = new DispatcherTimer
                        {
                            Interval = new TimeSpan(0, 0, 1),
                        };
                        timer.Tick += (sender, e) => { HideLoading(); };
                        timer.Start();
                    });
            }
        }

        private static void OnRequest<T>(IAsyncResult result)
        {
            var args = result.AsyncState as RequestArgs<T>;
            var request = args.Request;
            using (var stream = request.EndGetRequestStream(result))
            {
                switch (args.PostType)
                {
                    case PostTypeEnum.Content:
                        {
                            byte[] content = Encoding.UTF8.GetBytes(args.Parameter.ToString());
                            stream.Write(content, 0, content.Length);
                            stream.Flush();
                            break;
                        }
                    case PostTypeEnum.File:
                        {
                            byte[] content = args.Parameter as byte[];
                            stream.Write(content, 0, content.Length);
                            stream.Flush();
                            break;
                        }
                    default:
                        {
                            using (StreamWriter sw = new StreamWriter(stream))
                            using (JsonWriter writer = new JsonTextWriter(sw))
                            {
                                JsonSerializer serializer = new JsonSerializer();
                                serializer.Converters.Add(new StringEnumConverter());
                                serializer.Converters.Add(new IsoDateTimeConverter());
                                serializer.NullValueHandling = NullValueHandling.Ignore;
                                serializer.DefaultValueHandling = DefaultValueHandling.Ignore;

                                serializer.Serialize(writer, args.Parameter);
                                stream.Flush();
                            }
                            break;
                        }
                }


            }

            request.BeginGetResponse(OnResponse<T>, args);
        }

        private static void OnResponse<T>(IAsyncResult result)
        {
            RequestArgs<T> args = result.AsyncState as RequestArgs<T>;
            var callback = args.Callback;
            HttpWebResponse response = null;
            Exception error = null;
            T data = default(T);
            try
            {
                if (args.Request.HaveResponse)
                {
                    response = (HttpWebResponse)args.Request.EndGetResponse(result);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (Stream stream = response.GetResponseStream())
                        //using (MemoryStream ms = new MemoryStream())
                        {
                        //    byte[] buffer = new byte[1024];
                        //    int readed = 0;
                        //    long contentLength = response.ContentLength;
                        //    do
                        //    {
                        //        readed = stream.Read(buffer, 0, buffer.Length);
                        //        ms.Write(buffer, 0, readed);
                        //        contentLength -= readed;
                        //    } while (contentLength > 0);
                            //ms.Seek(0, SeekOrigin.Begin);
                            //if (args.WithZip)
                            //{
                            //    try
                            //    {
                            //        using (ZipInputStream zipStream = new ZipInputStream(stream))
                            //        {
                            //            zipStream.IsStreamOwner = false;
                            //            ZipEntry entry = zipStream.GetNextEntry();
                            //            byte[] zipBuffer = new byte[1024];
                            //            StreamUtils.Copy(zipStream, ms, zipBuffer);
                            //        }
                            //    }
                            //    catch { }
                            //    ms.Seek(0, SeekOrigin.Begin);
                            //    using (StreamReader sr = new StreamReader(ms))
                            //    using (JsonReader reader = new JsonTextReader(sr))
                            //    {
                            //        JsonSerializer serializer = new JsonSerializer();
                            //        serializer.Converters.Add(new StringEnumConverter());
                            //        serializer.Converters.Add(new IsoDateTimeConverter());
                            //        serializer.NullValueHandling = NullValueHandling.Ignore;
                            //        serializer.DefaultValueHandling = DefaultValueHandling.Ignore;
                            //        serializer.PreserveReferencesHandling = PreserveReferencesHandling.None;
                            //        serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                            //        data = (T)serializer.Deserialize(reader, typeof(T));
                            //    }
                            //}
                            //else
                            //{
                            using (StreamReader sr = new StreamReader(stream))
                                using (JsonReader reader = new JsonTextReader(sr))
                                {
                                    JsonSerializer serializer = new JsonSerializer();
                                    serializer.Converters.Add(new StringEnumConverter());
                                    serializer.Converters.Add(new IsoDateTimeConverter());
                                    serializer.NullValueHandling = NullValueHandling.Ignore;
                                    serializer.DefaultValueHandling = DefaultValueHandling.Ignore;
                                    serializer.PreserveReferencesHandling = PreserveReferencesHandling.None;
                                    serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                                    data = (T)serializer.Deserialize(reader, typeof(T));
                                }
                            //}
                        }

                        if (callback != null && error == null)
                        {
                            Deployment.Current.Dispatcher.BeginInvoke(
                                () =>
                                {
                                    callback(new ResultArgs<T>(data, error));
                                });
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                error = ex;
                if (ErrorHandler != null)
                {
                    ErrorHandler(null, new ApplicationUnhandledExceptionEventArgs(ex, false));
                }
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }

            }
            if (args.ShowLoading)
            {
                Interlocked.Decrement(ref m_RequestCount);
                Deployment.Current.Dispatcher.BeginInvoke(
                    () =>
                    {
                        var timer = new DispatcherTimer
                        {
                            Interval = new TimeSpan(0, 0, 1),
                        };
                        timer.Tick += (sender, e) => { HideLoading(); timer.Stop(); };
                        timer.Start();
                    });
            }
        }

        private static void ShowLoading()
        {
            if (m_RequestCount == 0)
            {
                if (ShowLoadingDialog != null)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(
                    () =>
                    {
                        ShowLoadingDialog(null, new EventArgs());
                    });
                    
                }
            }
            Interlocked.Increment(ref m_RequestCount);
        }

        private static void HideLoading()
        {
            if (m_RequestCount <= 0)
            {
                if (HidLoadingDialog != null)
                {
                    HidLoadingDialog(null, new EventArgs());
                }

            }
        }

        
    }
}
