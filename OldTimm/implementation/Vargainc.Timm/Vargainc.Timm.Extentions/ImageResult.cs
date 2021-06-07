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
    public class ImageResult : IHttpActionResult
    {
        string m_ContentType;
        string m_SrcPath;
        public ImageResult(string contentType, string srcPath)
        {
            m_ContentType = contentType;
            m_SrcPath = srcPath;
        }
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            result.Content = new StreamContent(new FileStream(this.m_SrcPath, FileMode.Open, FileAccess.Read));
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(m_ContentType);
            return Task.FromResult(result);
        }
    }
}
