using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.IO;
using ServiceStack.Text;

namespace TIMM.GPS.RESTService.Extend
{
    public class ContactFastJsonFormatter : MediaTypeFormatter
    {
        public ContactFastJsonFormatter()
        {
            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
        }

        protected override bool CanWriteType(Type type)
        {
            return true;
        }

        protected override void OnWriteToStream(Type type, object value, Stream stream, HttpContentHeaders contentHeaders, System.Net.TransportContext context)
        {
            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
            serializer.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            serializer.Converters.Add(new Newtonsoft.Json.Converters.IsoDateTimeConverter());
            serializer.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            serializer.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore;
            serializer.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;
            serializer.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            //JsonSerializer.SerializeToStream(value, type, stream);
            using (StreamWriter sw = new StreamWriter(stream))
            using (Newtonsoft.Json.JsonWriter writer = new Newtonsoft.Json.JsonTextWriter(sw))
            {
                serializer.Serialize(writer, value);
                writer.Flush();
            }
        }

        protected override object OnReadFromStream(Type type, Stream stream, HttpContentHeaders contentHeaders)
        {
            return JsonSerializer.DeserializeFromStream(type, stream);
        }

        protected override bool CanReadType(Type type)
        {
            return true;
        }
    }
}
