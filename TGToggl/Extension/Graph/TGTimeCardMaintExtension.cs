using PX.Data;
using PX.Objects.EP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGToggl
{
    public class TGTimeCardMaintExtension : PXGraphExtension<TimeCardMaint>
    {
        #region Views
        [PXVirtualDAC]
        public PXSelectReadonly<TGTimecardTask> TGTasks;

        public virtual IEnumerable tGTasks()
        {
            List<TGTimecardTask> tasks = new List<TGTimecardTask>();

            EPEmployee employee = Base.Employee.Current;
            if (employee != null)
            {
                DateTime startDate = PXWeekSelector2Attribute.GetWeekStartDate(Base.Document.Current.WeekID.Value);
                DateTime endDate = PXWeekSelector2Attribute.GetWeekEndDate(this.Base, Base.Document.Current.WeekID.Value);

                TGEPEmployeeExtension employeeExt = PXCache<EPEmployee>.GetExtension<TGEPEmployeeExtension>(employee);
                using (TogglClient client = new TogglClient(employeeExt.UsrTGToken))
                {
                    foreach(TogglMap result in client.TGTimecardTaskList(startDate, endDate))
                    {
                        tasks.Add(TogglMapper.MapFromToggl(this.Base, result));
                    }
                }
            }

            return tasks;
        }
        #endregion

        #region Buttons
        public PXAction<EPTimeCard> preloadFromToggl;
        [PXUIField(DisplayName = "Preload from Toggl")]
        [PXButton(Tooltip = "Preload Activities from Toggl")]
        public virtual void PreloadFromToggl()
        {
            if(TGTasks.AskExt() == WebDialogResult.OK)
            {
                foreach(TGTimecardTask task in TGTasks.Cache.Cached)
                {
                    if(task.Selected ?? false)
                    {
                        Base.Activities.Insert(TogglMapper.MapToTimecard(task));
                    }
                }
            }
        }
        #endregion

        #region Events


        public virtual void EPTimeCard_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            EPTimeCard row = e.Row as EPTimeCard;
            if(row != null)
            {
                EPEmployee employee = Base.Employee.Current;
                if(employee != null)
                {
                    TGEPEmployeeExtension employeeExt = PXCache<EPEmployee>.GetExtension<TGEPEmployeeExtension>(employee);

                    preloadFromToggl.SetEnabled(!String.IsNullOrEmpty(employeeExt.UsrTGToken));
                }
            }
        }

        public virtual void TGTimecardTask_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            TGTimecardTask row = e.Row as TGTimecardTask;
            if(row != null)
            {
                
            }
        }
        #endregion
    }
}
