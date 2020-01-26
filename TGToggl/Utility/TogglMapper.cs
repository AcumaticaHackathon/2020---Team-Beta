using PX.Data;
using PX.Objects.PM;
using System;
using static PX.Objects.EP.TimeCardMaint;

namespace TGToggl
{
    public static class TogglMapper
    {
        public static TGTimecardTask MapFromToggl(PXGraph graph, TogglMap record)
        {
            PXGraph _graph = graph;
            int? _projectID = null;
            int? _taskID = null;

            PMProject project = PXSelectReadonly<PMProject, Where<PMProject.contractCD, Equal<Required<PMProject.contractCD>>>>.Select(_graph, record.ProjectName);
            if(project == null)
            {
                project = PXSelectReadonly<PMProject, Where<PMProject.nonProject, Equal<True>>>.Select(_graph);
            }
            _projectID = project.ContractID;

            //Tasks are always related to a project
            PMTask task = PXSelectReadonly<PMTask, Where<PMTask.projectID, Equal<Required<PMTask.projectID>>, And<PMTask.taskCD, Equal<Required<PMTask.taskCD>>>>>.Select(_graph, new object[] { _projectID, record.TaskName });
            if(task != null)
            {
                _taskID = task.TaskID;
            }

            return new TGTimecardTask() { Selected = false, Date = record.Date, ProjectName = record.ProjectName, ProjectID = _projectID, TaskName = record.TaskName, ProjectTaskID = _taskID, Description = record.Description, Duration = ( record.Duration / 60 )};
        }

        public static EPTimecardDetail MapToTimecard(TGTimecardTask record)
        {
            return new EPTimecardDetail() { Date = record.Date, ProjectID = record.ProjectID, ProjectTaskID = record.ProjectTaskID, TimeSpent = record.Duration };
        }
    }

    public struct TogglMap
    {
        public DateTime Date;
        public string ProjectName;
        public string TaskName;
        public string Description;
        public int Duration;
    }
}
