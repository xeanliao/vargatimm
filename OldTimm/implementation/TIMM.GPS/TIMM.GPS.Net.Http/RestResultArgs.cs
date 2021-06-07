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

namespace TIMM.GPS.Net.Http
{
    public class ResultArgs<T>
    {
        public Exception Error { get; private set; }
        public T Data { get; private set; }

        public ResultArgs(T t, Exception error)
        {
            this.Error = error;
            this.Data = t;
        }
    }
}
