using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TIMM.GPS.RESTService.Extend
{
    public class ContactJsonFormatter : MediaTypeFormatter
    {

        public ContactJsonFormatter()
        {
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
        }

        protected override bool CanWriteType(Type type)
        {
            return true;
        }

        protected override void OnWriteToStream(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, System.Net.TransportContext context)
        {
                using (StreamWriter sw = new StreamWriter(stream))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Converters.Add(new StringEnumConverter());
                    serializer.Converters.Add(new IsoDateTimeConverter());
                    serializer.NullValueHandling = NullValueHandling.Ignore;
                    serializer.DefaultValueHandling = DefaultValueHandling.Ignore;
                    serializer.PreserveReferencesHandling = PreserveReferencesHandling.None;
                    serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                    serializer.Serialize(writer, value);
                    writer.Flush();
                }
        }

        protected override object OnReadFromStream(Type type, Stream stream, HttpContentHeaders contentHeaders)
        {
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
                
                return serializer.Deserialize(reader, type);
            }
        }

        protected override bool CanReadType(Type type)
        {
            return true;
        }
    }
}
