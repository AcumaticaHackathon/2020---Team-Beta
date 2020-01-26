using PX.Data;
using PX.Data.BQL;
using PX.Objects.GL;
using PX.Objects.PM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGToggl
{
    [Serializable]
    public class TGTimecardTask : IBqlTable
    {
        #region Selected
        public abstract class selected : BqlBool.Field<selected>
        {
        }
        [PXBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected { get; set; }
        #endregion

        #region Date
        public abstract class date : BqlDateTime.Field<date>
        {
        }
        [PXDate]
        [PXUIField(DisplayName = "Date")]
        public virtual DateTime? Date { get; set; }
        #endregion

        #region ProjectName
        public abstract class projectName : BqlString.Field<projectName>
        {
        }
        [PXString]
        [PXUIField(DisplayName = "Toggl Project", Enabled = false)]
        public virtual string ProjectName { get; set; }
        #endregion

        #region TaskName
        public abstract class taskName : BqlString.Field<taskName>
        {
        }
        [PXString]
        [PXUIField(DisplayName = "Toggl Task", Enabled = false)]
        public virtual string TaskName { get; set; }
        #endregion

        #region ProjectID
        public abstract class projectID : BqlInt.Field<projectID>
        {
        }
        [EPTimeCardProjectAttribute]
        public virtual int? ProjectID { get; set; }
        #endregion

        #region TaskID
        public abstract class projectTaskID : BqlInt.Field<projectTaskID>
        {
        }
        [EPTimecardProjectTask(typeof(projectID), BatchModule.TA, DisplayName = "Project Task")]
        public virtual int? ProjectTaskID { get; set; }
        #endregion

        #region Duration
        public abstract class duration : BqlInt.Field<duration>
        {
        }
        [PXTimeList]
        [PXInt]
        [PXUIField(DisplayName = "Duration")]
        public virtual int? Duration { get; set; }
        #endregion

        #region Description
        public abstract class description : BqlString.Field<description>
        {
        }
        [PXString(200, IsUnicode = true)]
        [PXUIField(DisplayName = "Description")]
        public virtual string Description { get; set; }
        #endregion
    }
}
