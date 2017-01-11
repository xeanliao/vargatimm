using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Net;

namespace Vargainc.Timm.Extentions
{
    public class CustomJsonFormatter : MediaTypeFormatter
    {
        private readonly JsonSerializerSettings settings;

        public CustomJsonFormatter()
        {
            settings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateFormatString = "MM-dd-yyyy",
                NullValueHandling = NullValueHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
            settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

            // Fill out the mediatype and encoding we support
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
        }

        public override bool CanReadType(Type type)
        {
            return true;
        }

        public override bool CanWriteType(Type type)
        {
            return true;
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            var serializer = JsonSerializer.Create(settings);
            return Task.Factory.StartNew(() => {
                using (var streamReader = new StreamReader(readStream, Encoding.UTF8))
                using (var jsonTextReader = new JsonTextReader(streamReader))
                {
                    return serializer.Deserialize(jsonTextReader, type);
                }
            });
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            var serializer = JsonSerializer.Create(settings);
            return Task.Factory.StartNew(() => {
                using (var streamWriter = new StreamWriter(writeStream, Encoding.UTF8))
                using (var jsonTextWriter = new JsonTextWriter(streamWriter))
                {
                    serializer.Serialize(jsonTextWriter, value);
                }
            });
        }
    }
}
