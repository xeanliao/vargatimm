using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using GPS.DomainLayer.Area;
using GPS.DomainLayer.Enum;
using Newtonsoft.Json;
using System.Configuration;

namespace GPS.Website.Handler
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class mapsettings : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write(string.Format("GPS.ClsSettings = {0};", GetClassifications()));
        }

        public void OutPutSetings(HttpContext context)
        {

        }

        public string GetClassifications()
        {
            List<ClassificationSection> sections = new List<ClassificationSection>();
            sections.Add((ClassificationSection)ConfigurationManager.GetSection("Classifications/Z3"));
            sections.Add((ClassificationSection)ConfigurationManager.GetSection("Classifications/Z5"));
            sections.Add((ClassificationSection)ConfigurationManager.GetSection("Classifications/TRK"));
            sections.Add((ClassificationSection)ConfigurationManager.GetSection("Classifications/BG"));
            sections.Add((ClassificationSection)ConfigurationManager.GetSection("Classifications/CBSA"));
            sections.Add((ClassificationSection)ConfigurationManager.GetSection("Classifications/Urban"));
            sections.Add((ClassificationSection)ConfigurationManager.GetSection("Classifications/County"));
            sections.Add((ClassificationSection)ConfigurationManager.GetSection("Classifications/SLD_Senate"));
            sections.Add((ClassificationSection)ConfigurationManager.GetSection("Classifications/SLD_House"));
            sections.Add((ClassificationSection)ConfigurationManager.GetSection("Classifications/Voting_District"));
            sections.Add((ClassificationSection)ConfigurationManager.GetSection("Classifications/SD_Elem"));
            sections.Add((ClassificationSection)ConfigurationManager.GetSection("Classifications/SD_Secondary"));
            sections.Add((ClassificationSection)ConfigurationManager.GetSection("Classifications/SD_Unified"));
            sections.Add((ClassificationSection)ConfigurationManager.GetSection("Classifications/Custom"));
            sections.Add((ClassificationSection)ConfigurationManager.GetSection("Classifications/Address"));
            sections.Add((ClassificationSection)ConfigurationManager.GetSection("Classifications/PremiumCRoute"));
            return JsonConvert.SerializeObject(sections);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class ClassificationSection : ConfigurationSection
    {
        public ClassificationSection()
        {
        }

        [JsonProperty]
        [ConfigurationProperty("MinZoomLevel", DefaultValue = "1", IsRequired = true)]
        [IntegerValidator(ExcludeRange = false, MaxValue = 19, MinValue = 1)]
        public Int32 MinZoomLevel
        {
            get
            {
                return (Int32)this["MinZoomLevel"];
            }
            set
            {
                this["MinZoomLevel"] = value;
            }
        }

        [JsonProperty]
        [ConfigurationProperty("MaxZoomLevel", DefaultValue = "19", IsRequired = true)]
        [IntegerValidator(ExcludeRange = false, MaxValue = 19, MinValue = 1)]
        public Int32 MaxZoomLevel
        {
            get
            {
                return (Int32)this["MaxZoomLevel"];
            }
            set
            {
                this["MaxZoomLevel"] = value;
            }
        }

        [JsonProperty]
        [ConfigurationProperty("BoxLat", DefaultValue = "1", IsRequired = true)]
        [IntegerValidator(ExcludeRange = false, MaxValue = 200, MinValue = 1)]
        public Int32 BoxLat
        {
            get
            {
                return (Int32)this["BoxLat"];
            }
            set
            {
                this["BoxLat"] = value;
            }
        }

        [JsonProperty]
        [ConfigurationProperty("BoxLon", DefaultValue = "1", IsRequired = true)]
        [IntegerValidator(ExcludeRange = false, MaxValue = 200, MinValue = 1)]
        public Int32 BoxLon
        {
            get
            {
                return (Int32)this["BoxLon"];
            }
            set
            {
                this["BoxLon"] = value;
            }
        }

        [JsonProperty]
        [ConfigurationProperty("FillColor")]
        public ColorElement FillColor
        {
            get
            { return (ColorElement)this["FillColor"]; }
            set
            { this["FillColor"] = value; }
        }

        [JsonProperty]
        [ConfigurationProperty("LineColor")]
        public ColorElement LineColor
        {
            get
            { return (ColorElement)this["LineColor"]; }
            set
            { this["LineColor"] = value; }
        }

        [JsonProperty]
        public Int32 LoadTimes
        {
            get { return 0; }
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class ColorElement : ConfigurationElement
    {
        public ColorElement()
        {
        }

        public ColorElement(int r, int g, int b, int a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        [JsonProperty]
        [ConfigurationProperty("HtmlValue", DefaultValue = "000", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 3, MaxLength = 6)]
        public String HtmlValue
        {
            get
            { return (String)this["HtmlValue"]; }
            set
            { this["HtmlValue"] = value; }
        }

        [JsonProperty]
        [ConfigurationProperty("R", DefaultValue = "0", IsRequired = true)]
        [IntegerValidator(ExcludeRange = false, MaxValue = 255, MinValue = 0)]
        public Int32 R
        {
            get
            { return (Int32)this["R"]; }
            set
            { this["R"] = value; }
        }

        [JsonProperty]
        [ConfigurationProperty("G", DefaultValue = "0", IsRequired = true)]
        [IntegerValidator(ExcludeRange = false, MaxValue = 255, MinValue = 0)]
        public Int32 G
        {
            get
            { return (Int32)this["G"]; }
            set
            { this["G"] = value; }
        }

        [JsonProperty]
        [ConfigurationProperty("B", DefaultValue = "0", IsRequired = true)]
        [IntegerValidator(ExcludeRange = false, MaxValue = 255, MinValue = 0)]
        public Int32 B
        {
            get
            { return (Int32)this["B"]; }
            set
            { this["B"] = value; }
        }

        [JsonProperty]
        [ConfigurationProperty("A", DefaultValue = "0", IsRequired = true)]
        public float A
        {
            get
            { return (float)this["A"]; }
            set
            { this["A"] = value; }
        }
    }




}
