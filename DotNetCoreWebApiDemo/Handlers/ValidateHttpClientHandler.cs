using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetCoreWebApiDemo.Handlers
{
    public class ValidateHttpClientHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage requestMessage,
            CancellationToken cancellationToken)
        {
            if(!requestMessage.Headers.Contains("TEST"))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Missing required header")
                };
            }

            return await base.SendAsync(requestMessage, cancellationToken);
        }
    }
}
