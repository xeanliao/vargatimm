using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Vargainc.Timm.Extentions
{
    public class PDFResult: IHttpActionResult
    {
        public PDFResult(string fileName, Stream outputStream)
        {
            this.FileName = fileName.Replace(" ", "_");
            this.OutputStream = outputStream;
        }

        public PDFResult(string fileName, string outputFilePath)
        {
            this.FileName = fileName.Replace(" ", "_");
            this.OutputStream = new FileStream(outputFilePath, FileMode.Open, FileAccess.Read);
        }

        public string FileName { get; private set; }
        public Stream OutputStream { get; private set; }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(this.OutputStream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            result.Content.Headers.TryAddWithoutValidation("Content-Disposition", string.Format("attachment; filename={0}", this.FileName));
            
            return Task.FromResult(result);

        }
    }
}
