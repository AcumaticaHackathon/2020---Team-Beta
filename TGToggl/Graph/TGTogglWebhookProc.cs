using PX.Data;
using PX.Data.Webhooks;
using System.Threading;

namespace TGToggl
{
    public class TGTogglWebhookProc : PXGraph<TGTogglWebhookProc>, IWebhookHandler
    {
        public System.Web.Http.IHttpActionResult ProcessRequest(System.Net.Http.HttpRequestMessage request,
    CancellationToken cancellationToken)
        {
            RequestParser parser = new RequestParser(request);

            return parser.ProcessRequest();
        }
    }
}
