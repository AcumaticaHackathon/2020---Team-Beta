using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace TGToggl
{
    public class HtmlActionResult : IHttpActionResult
    {
        private const string ViewDirectory = @"E:devConsoleApplication8ConsoleApplication8";
        //private readonly string _view;
        //private readonly dynamic _model;

        Uri RequestUri { get; set; }


        public HtmlActionResult(string viewName, Uri requestUri)
        //, dynamic model)
        {
            //_view = LoadView(viewName,RequestUri.Query);
            RequestUri = requestUri;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            //var parsedView = RazorEngine.Razor.Parse(_view, _model);
            //response.Content = new StringContent(parsedView);



            //response.Content = new StringContent($"Response Content:{RequestUri.Query}");
            response.Content = new StringContent(LoadView("toggl Query results", RequestUri.Query));

            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return Task.FromResult(response);
        }

        private static string LoadView(string header1, string queryString)
        {
            //var view = File.ReadAllText(Path.Combine(ViewDirectory, name + ".cshtml"));
            //the implemetation of the sample above commented out
            //trying to return simple HTML here to see if that gets us anywhere

            var view = @"
<!DOCTYPE html>
<html>
<body>

<h1>{0}</h1>
<p>paragraph.</p>
{1}
</body>
</html>
";

            return String.Format(view, header1, queryString);
        }
    }
}
