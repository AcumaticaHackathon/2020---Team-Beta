using System;
using System.Collections.Generic;
using Toggl;
using Toggl.Services;
using Toggl.QueryObjects;
using Toggl.DataObjects;
using System.Net.Http;

namespace TGToggl
{
    public class TogglClient : IDisposable
    {
        private string _APIToken;

        private Toggl.Services.ProjectService ProjectService;

        private Toggl.Services.TimeEntryService TimeEntryService;

        private Toggl.Services.TaskService TaskService;

        private Dictionary<int, string> projects = new Dictionary<int, string>();

        private Dictionary<int, string> tasks = new Dictionary<int, string>();

        private RequestParser parser;

        public TogglClient(HttpRequestMessage request)
        {
            parser = new RequestParser(request);
        }

        public TogglClient(string APIToken)
        {
            _APIToken = APIToken;
            ProjectService = new ProjectService(_APIToken);
            TimeEntryService = new TimeEntryService(_APIToken);
            TaskService = new TaskService(_APIToken);
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

        public void StartTimer(string projectName, string description, List<string> tags)
        {
            TimeEntry entry = new TimeEntry();
            int? projectID = RetrieveProjectID(projectName);
            if (!projectID.HasValue) projectID = CreateProject(projectName);
            entry.ProjectId = projectID;//need to update with method call
            entry.TagNames = tags;
            entry.Description = description;
            TimeEntryService.Add(entry);
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
