using System;
using System.Collections.Generic;
using Toggl;
using Toggl.Services;
using Toggl.QueryObjects;
using Toggl.DataObjects;
using Toggl.Extensions;
using System.Net.Http;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Net;

namespace TGToggl
{
    public class TogglClient : IDisposable
    {
        private string _APIToken;

        private Toggl.Services.WorkspaceService WorkspaceService;

        private Toggl.Services.ProjectService ProjectService;

        private Toggl.Services.TimeEntryService TimeEntryService;

        private Toggl.Services.TaskService TaskService;

        private Toggl.Services.ApiService ApiService;

        private Toggl.Services.ClientService ClientService;

        private Dictionary<int, string> projects = new Dictionary<int, string>();

        private Dictionary<int, string> tasks = new Dictionary<int, string>();

        public TogglClient(string APIToken)
        {
            _APIToken = APIToken;
            WorkspaceService = new WorkspaceService(_APIToken);
            ProjectService = new ProjectService(_APIToken);
            TimeEntryService = new TimeEntryService(_APIToken);
            TaskService = new TaskService(_APIToken);
            ApiService = new ApiService(_APIToken);
            ClientService = new ClientService(_APIToken);
        }

        private List<TimeEntry> RetrieveTime(DateTime date)
        {
            TimeEntryParams prams = new TimeEntryParams();
            prams.StartDate = date.Date;
            prams.EndDate = date.Date.AddDays(1).AddTicks(-1);

            return TimeEntryService.List(prams);
        }

        public List<TogglMap> TGTimecardTaskList(DateTime start, DateTime end)
        {
            List<TogglMap> entries = new List<TogglMap>();
            foreach (DateTime day in EachDay(start, end))
            {
                foreach(TimeEntry entry in RetrieveTime(day))
                {
                    if(entry.Duration > 0 ) entries.Add(new TogglMap { Date = day, ProjectName = RetrieveProjectName(entry.ProjectId), TaskName = RetrieveTaskName(entry.TaskId), Description = entry.Description, Duration = (int)(entry.Duration ?? 0) });
                }
            }
            return entries;
        }

        private int RetrieveDefaultWorkspace()
        {
            return WorkspaceService.List()[0].Id.Value;
        }

        private String RetrieveProjectName(long? projectID)
        {
            string projectName = string.Empty;
            if(projectID.HasValue)
            {
                int _projectID = (int)projectID;
                if (!projects.TryGetValue(_projectID, out projectName))
                {
                    Project project = ProjectService.Get(_projectID);
                    if (project != null)
                    {
                        projects.Add(_projectID, project.Name);
                        projectName = project.Name;
                    }
                }
            }
            return projectName;
        }

        private int? RetrieveProjectID(string projectName)
        {
            return ProjectService.List().Find(x => x.Name == projectName)?.Id;
        }

        private int? CreateProject(string name)
        {
            Project project = new Project() { Name = name };
            return ProjectService.Add(project)?.Id;
        }

        private String RetrieveTaskName(long? taskID)
        {
            string taskName = string.Empty;
            if(taskID.HasValue)
            {
                int _taskID = (int)taskID;
                if (!tasks.TryGetValue(_taskID, out taskName))
                {
                    Task task = TaskService.Get(_taskID);
                    if (task != null)
                    {
                        tasks.Add(_taskID, task.Name);
                        taskName = task.Name;
                    }
                }
            }
            return taskName;
        }

        private IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        /*
        public void StartTimer(string projectName, string description, List<string> tags)
        {
            TimeEntry entry = new TimeEntry();

            var timeEntry = TimeEntryService.Add(new TimeEntry()
            {

                IsBillable = true,

                CreatedWith = "TogglAPI.Net",
                
                Duration = 0,
                
                Start = DateTime.Now.ToIsoDateStr(),

                Stop = DateTime.Now.ToIsoDateStr(),

                WorkspaceId = RetrieveDefaultWorkspace()
            });
        }
        */

        public string StartTimer(string projectName, string description, string tags)
        {
            /*
                curl -v -u 1971800d4d82861d8f2c1651fea4d212:api_token \
	            -H "Content-Type: application/json" \
	            -d '{"time_entry":{"description":"Meeting with possible clients","tags":["billed"],"pid":123,"created_with":"curl"}}' \
                    {"time_entry":{"description":"Testing_Toggle_Time_entry","tags":["Test"],"pid":156371204,"Testing123"}}
	            -X POST https://www.toggl.com/api/v8/time_entries/start
             */
            //todo: move this into a shared place
            int? projectID = RetrieveProjectID(projectName);
            if (!projectID.HasValue) projectID = CreateProject(projectName);

            string url = $"https://www.toggl.com/api/v8/time_entries/start";
            string body = "{\"time_entry\":{\"description\":\"" +
                          description +
                          "\",\"tags\":[\"" +
                          tags +
                          "\"],\"pid\":" +
                          projectID.ToString() +
                          ",\"created_with\":\"TeamBeta\"}}";

            string postData = body;
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] byte1 = encoding.GetBytes(postData);

            var httpWebRequest = GetToggleWebRequest(url);
            httpWebRequest.Method = WebRequestMethods.Http.Post;
            httpWebRequest.ContentLength = byte1.Length;

            Stream newStream = httpWebRequest.GetRequestStream();
            newStream.Write(byte1, 0, byte1.Length);

            var result = GetResult(httpWebRequest);
            var TimeEntry = JObject.Parse(result);


            return TimeEntry.ToString();
        }

        public string StopTimer()
        {
            //todo: need to lookup the current Timer.
            var currentID = GetCurrentID();

            //todo: then take the id found and invoke the stop timer.

            /*
             curl -v -u 1971800d4d82861d8f2c1651fea4d212:api_token \
	            -H "Content-Type: application/json" \
	            -X PUT https://www.toggl.com/api/v8/time_entries/436694100/stop
             */



            string url = $"https://www.toggl.com/api/v8/time_entries/currentID/stop";
            url = url.Replace("currentID", currentID);

            //no body
            //string body = "{\"time_entry\":{\"description\":\"" +
            //              description +
            //              "\",\"tags\":[\"" +
            //              tags +
            //              "\"],\"pid\":" +
            //              projectId +
            //              ",\"created_with\":\"TeamBeta\"}}";

            //string postData = body;
            ASCIIEncoding encoding = new ASCIIEncoding();
            //byte[] byte1 = encoding.GetBytes(postData);

            var httpWebRequest = GetToggleWebRequest(url);
            httpWebRequest.Method = WebRequestMethods.Http.Put;
            //httpWebRequest.ContentLength = byte1.Length;

            Stream newStream = httpWebRequest.GetRequestStream();
            //newStream.Write(byte1, 0, byte1.Length);

            var result = GetResult(httpWebRequest);
            var TimeEntry = JObject.Parse(result);


            return TimeEntry.ToString();

            throw new NotImplementedException();
        }

        private string GetCurrentID()
        {
            /*
                curl -v -u 1971800d4d82861d8f2c1651fea4d212:api_token \
                -X GET https://www.toggl.com/api/v8/time_entries/current
             */
            string url = $"https://www.toggl.com/api/v8/time_entries/current";
            var httpWebRequest = GetToggleWebRequest(url);
            var result = GetResult(httpWebRequest);
            if (result == "null") return null;
            var Current = JObject.Parse(result);
            var currentID = Current?["data"]["id"];
            return currentID.ToString();
        }

        private static string GetResult(HttpWebRequest httpWebRequest)
        {
            string result = null;
            try
            {
                var response = (HttpWebResponse)httpWebRequest.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader sr = new StreamReader(stream);
                    result = sr.ReadToEnd();
                    sr.Close();
                }

                if (null != result)
                {
                    System.Diagnostics.Debug.WriteLine(result.ToString());
                }

                // Get the headers
                object headers = response.Headers;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message + "\n" + e.ToString());
            }

            return result;
        }

        private HttpWebRequest GetToggleWebRequest(string url)
        {
            string ApiToken = _APIToken;
            string userpass = ApiToken + ":api_token";
            string userpassB64 = Convert.ToBase64String(Encoding.Default.GetBytes(userpass.Trim()));
            string authHeader = "Basic " + userpassB64;

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Headers.Add("Authorization", authHeader);
            httpWebRequest.Method = "GET";
            httpWebRequest.ContentType = "application/json";
            //authRequest.Credentials = CredentialCache.DefaultNetworkCredentials;
            return httpWebRequest;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (disposing)
                {
                }
            }
        }
    }

}
