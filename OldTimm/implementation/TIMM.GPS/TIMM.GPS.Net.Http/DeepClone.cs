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
using Newtonsoft.Json;

namespace TIMM.GPS.Helper
{
    public static class DeepCloneHelper
    {
        /// <summary>
        /// deep clone object to same type object
        /// </summary>
        /// <typeparam name="T">object type</typeparam>
        /// <param name="source">source object</param>
        /// <returns></returns>
        public static T DeepClone<T>(this T source)
        {
            using (MemoryStream ms = new MemoryStream())
            using (StreamWriter sw = new StreamWriter(ms))
            using (StreamReader sr = new StreamReader(ms))
            {
                JsonSerializer serializer = new JsonSerializer()
                {
                    NullValueHandling = NullValueHandling.Include, 
                    DefaultValueHandling = DefaultValueHandling.Include 
                };

                using (JsonTextWriter writer = new JsonTextWriter(sw))
                {
                    writer.CloseOutput = false;
                    serializer.Serialize(writer, source);
                    writer.Flush();
                }

                ms.Seek(0, SeekOrigin.Begin);

                using (JsonTextReader reader = new JsonTextReader(sr))
                {
                    return (T)serializer.Deserialize(reader, typeof(T));
                }
            }
        }

        /// <summary>
        /// Transfer source object to target object
        /// </summary>
        /// <typeparam name="T">target object type</typeparam>
        /// <param name="source">source object</param>
        /// <returns></returns>
        public static T Transfer<T>(this object source)
        {
            using (MemoryStream ms = new MemoryStream())
            using (StreamWriter sw = new StreamWriter(ms))
            using (StreamReader sr = new StreamReader(ms))
            {
                JsonSerializer serializer = new JsonSerializer() 
                { 
                    NullValueHandling = NullValueHandling.Include, 
                    DefaultValueHandling = DefaultValueHandling.Include 
                };

                using (JsonTextWriter writer = new JsonTextWriter(sw))
                {
                    writer.CloseOutput = false;
                    serializer.Serialize(writer, source);
                    writer.Flush();
                }

                ms.Seek(0, SeekOrigin.Begin);

                using (JsonTextReader reader = new JsonTextReader(sr))
                {
                    return (T)serializer.Deserialize(reader, typeof(T));
                }
            }
        }
    }
}
