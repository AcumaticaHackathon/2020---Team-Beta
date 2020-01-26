using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace TGToggl
{
    public class RequestParser
    {
        private HttpRequestMessage _request;

        private NameValueCollection _queryParameters;

        public RequestParser(HttpRequestMessage request)
        {
            _request = request;
            _queryParameters = HttpUtility.ParseQueryString(request.RequestUri.Query);
        }

        public HtmlActionResult ProcessRequest()
        {
            InvokeMethod(RetrieveMethod());

            return new HtmlActionResult("SomeName", _request.RequestUri);
        }


        private string RetrieveAPIKey()
        {
            const string cTogglAPIKey = "TogglAPIKey";
            if (!_queryParameters.AllKeys.Contains(cTogglAPIKey)) throw new Exception("The TogglAPIKey Parameter was not specified in the Query String");
            return _queryParameters[cTogglAPIKey];
        }

        private void InvokeMethod(string method)
        {
            StartTimer();
            //switch (method)
            //{
            //case "StartTimer":
            //    return InvokeStartTimer(queryParameters);
            //case "GPSTracker":
            //    return InvokeGPSTracker(queryParameters);
            //case "ClockOut":
            //    return InvokeClockOut(queryParameters);
            //case "CreateProject":
            //    return InvokeCreateProject(queryParameters);
            //case "NFCScan":
            //    return InvokeAutomatedIDScan(queryParameters);
            //case "StartTimerFromImage":
            //    return InvokeStartTimerFromImage(queryParameters, request);
            //default:
            //throw new Exception($"Method {method} was not recognized");

            //}
        }

        private string RetrieveMethod()
        {
            const string constMethod = "Method";
            if (!_queryParameters.AllKeys.Contains(constMethod)) throw new Exception($"Method Parameter was not found in the Query Parameters");

            return _queryParameters[constMethod];
        }

        private string RetrieveClient()
        {
            const string cClient = "Client";
            string client = string.Empty;
            if (_queryParameters.AllKeys.Contains(cClient))
            {
                //Client parameter is optional we do not need it to start a timer.
                //having it serves as usful info that the Toggle user can use to help
                //classify manual time classification.
                client = _queryParameters[cClient];
            }
            return client;
        }

        private string RetrieveProject()
        {
            const string cProject = "Project";
            //Note: Project is not mandatory toggl side but we want to make it mandatory for this Acumatcia integration.
            //      Should another implementor want to make this optional they can modify the code as they see fit.
            if (!_queryParameters.AllKeys.Contains(cProject)) throw new Exception("The Project Parameter was not specified in the Query String");
            return _queryParameters[cProject];
        }

        private string RetrieveDescription()
        {
            const string cDescription = "Description ";
            string description = string.Empty;
            if (_queryParameters.AllKeys.Contains(cDescription))
            {
                //Description  parameter is optional we do not need it to start a timer.
                description = _queryParameters[cDescription];
            }
            return description;
        }

        private List<string> RetrieveTags()
        {
            const string cTags = "Tags";
            string tags = string.Empty;
            if (_queryParameters.AllKeys.Contains(cTags))
            {
                //Tags parameter is optional we do not need it to start a timer.
                //Tags will be a mechanism as to allow for additional things to be 
                //switched during a download into Acumatcia
                //todo: implement any Tag functions deemed necessary. Im not seeing this a a critical item and would qualify as bonus.
                tags = _queryParameters[cTags];
            }
            return tags.Split(',').ToList(); ;
        }

        private void StartTimer()
        {  
            using (TogglClient client = new TogglClient(RetrieveAPIKey()))
            {
                client.StartTimer(RetrieveProject(), RetrieveDescription(), RetrieveTags()); 
            }
        }
    }
}
