using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Vargainc.Timm.REST.Controllers
{
    public class PngResult : IHttpActionResult
    {
        public PngResult(string filePath)
        {
            this.FilePath = filePath;
        }

        public string FilePath { get; private set; }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            if (File.Exists(this.FilePath))
            {
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                var stream = new FileStream(this.FilePath, FileMode.Open);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");

                return Task.FromResult(result);
            }
            else
            {
                HttpResponseMessage notfound = new HttpResponseMessage(HttpStatusCode.NotFound);
                return Task.FromResult(notfound);
            }
            
        }
    }

    public class TextResult : IHttpActionResult
    {
        string _value;

        public TextResult(string value)
        {
            _value = value;
        }
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage()
            {
                Content = new StringContent(_value)
            };
            return Task.FromResult(response);
        }
    }

    [RoutePrefix("image")]
    public class PhantomController : ApiController
    {
        [HttpGet]
        [Route("campaign/{campaignId:int}/submap/{submapId:int}")]
        public IHttpActionResult GetCampaignForPrint(int campaignId, int submapId)
        {
            var imgName = string.Format("{0}.png", Guid.NewGuid().ToString());
            var imgPath = HttpContext.Current.Server.MapPath("~/phantomjs/tmp/");
            if (!Directory.Exists(imgPath))
            {
                Directory.CreateDirectory(imgPath);
            }
            var exePath = HttpContext.Current.Server.MapPath("~/phantomjs/phantomjs.exe");
            
            var arguments = string.Format("print.js -type SubMap -campaignId {0} -submapId {1} -output \"{2}\"", 
                campaignId, submapId, Path.Combine(imgPath, imgName));

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = false,
                    CreateNoWindow = false,
                    WorkingDirectory = HttpContext.Current.Server.MapPath("~/phantomjs/")
                }
            };

            proc.Start();
            //StringBuilder debug = new StringBuilder();
            //while (!proc.StandardOutput.EndOfStream)
            //{
            //    string line = proc.StandardOutput.ReadLine();
            //    debug.AppendLine(line);
            //}

            proc.WaitForExit();

            var outputFilePath = Path.Combine(imgPath, imgName);
            if (File.Exists(outputFilePath))
            {
                return Json(new {
                    success= true,
                    path = imgName
                });
                //return new PngResult(outputFilePath);
            }
            else
            {
                return NotFound();
                //return new TextResult(debug.ToString());
            }
            
        }
    }
}
