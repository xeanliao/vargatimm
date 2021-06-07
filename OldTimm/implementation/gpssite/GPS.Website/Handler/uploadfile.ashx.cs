using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;

namespace GPS.Website.Handler
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class uploadfile : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            //context.Response.Clear();
            //context.Response.ContentType = "application/json";
            string type = context.Request.QueryString["type"] + "path";
            string path = System.Configuration.ConfigurationManager.AppSettings[type];
            UploadResult result = UploadFile(context, path);
            context.Response.Output.Write(JsonConvert.SerializeObject(result));
            context.Response.Flush();
            context.Response.End();
        }
        /// <summary>
        /// upload a file save in server
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private UploadResult UploadFile(HttpContext context, string path)
        {
            if (context.Request.Files.Count > 0)
            {
                HttpPostedFile file = context.Request.Files[0];
                if (file.ContentLength > 0)
                {
                    string newFileName = NewFileName(file.FileName);
                    string newFilePath = string.Format("{0}{1}", context.Server.MapPath(path), newFileName);
                    file.SaveAs(newFilePath);
                    return new UploadResult(newFileName);
                }
                else
                {
                    return new UploadResult();
                }
            }
            else
            {
                return new UploadResult();
            }
        }
        /// <summary>
        /// get a new file name for save
        /// </summary>
        /// <param name="fileName">original name</param>
        /// <returns>new file name</returns>
        private static string NewFileName(string fileName)
        {
            return string.Format("{0}{1}",
                DateTime.Now.Ticks,
                System.IO.Path.GetExtension(fileName));
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
    public class UploadResult
    {
        private bool _isSuccess;
        private string _name;

        public UploadResult()
        {
            _isSuccess = false;
            _name = "";
        }
        public UploadResult(string name)
        {
            _isSuccess = true;
            _name = name;
        }
        [JsonProperty]
        private bool IsSuccess
        {
            get { return _isSuccess; }
            set { _isSuccess = true; }
        }
        [JsonProperty]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}
