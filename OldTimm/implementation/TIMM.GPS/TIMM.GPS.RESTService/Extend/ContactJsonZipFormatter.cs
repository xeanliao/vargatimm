using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;

namespace TIMM.GPS.RESTService.Extend
{
    public class ContactJsonZipFormatter : MediaTypeFormatter
    {
        public ContactJsonZipFormatter()
        {
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/zipjson"));
        }

        protected override bool CanWriteType(Type type)
        {
            return true;
        }

        protected override void OnWriteToStream(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, System.Net.TransportContext context)
        {
            using (Stream ms = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(ms))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                    serializer.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                    serializer.Converters.Add(new Newtonsoft.Json.Converters.IsoDateTimeConverter());
                    serializer.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                    serializer.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore;
                    serializer.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;
                    serializer.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

                    serializer.Serialize(writer, value);
                    writer.Flush();

                    ms.Seek(0, SeekOrigin.Begin);

                    using (ZipOutputStream zipStream = new ZipOutputStream(stream))
                    {
                        //zipStream.SetLevel(3);
                        zipStream.IsStreamOwner = false;
                        ZipEntry entry = new ZipEntry("content");
                        zipStream.PutNextEntry(entry);
                        byte[] buffer = new byte[5120];
                        StreamUtils.Copy(ms, zipStream, buffer);
                        zipStream.Flush();
                    }
                }

                
            }
        }

        protected override object OnReadFromStream(Type type, Stream stream, HttpContentHeaders contentHeaders)
        {
            using (Stream ms = new MemoryStream())
            {
                using (ZipInputStream zipStream = new ZipInputStream(stream))
                {
                    zipStream.IsStreamOwner = false;
                    ZipEntry entry = zipStream.GetNextEntry();
                    byte[] buffer = new byte[5120];
                    StreamUtils.Copy(zipStream, ms, buffer);
                }
                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                using (StreamReader sr = new StreamReader(ms))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                    serializer.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                    serializer.Converters.Add(new Newtonsoft.Json.Converters.IsoDateTimeConverter());
                    serializer.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                    serializer.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore;
                    serializer.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;
                    serializer.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    return serializer.Deserialize(reader, type);
                }
            }
        }

        protected override bool CanReadType(Type type)
        {
            return true;
        }
    }
}
