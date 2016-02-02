using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Vargainc.Timm.Extentions
{
    public class FileResult : IHttpActionResult
    {
        public string FileName { get; private set; }
        public string FilePath { get; private set; }

        public FileResult(string fileName, string filePath)
        {
            this.FileName = fileName;
            this.FilePath = filePath;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(this.FilePath, FileMode.Open);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.TryAddWithoutValidation("Content-Disposition", string.Format("attachment; filename={0}", this.FileName));

            return Task.FromResult(result);
        }
    }
}
