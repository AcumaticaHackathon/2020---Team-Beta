using PX.Data;
using PX.Data.BQL;
using PX.Objects.CR;

namespace TGToggl
{
    public class TGCRActivityExtension : PXCacheExtension<CRActivity>
    {
        #region UsrTGTogglTimerLink
        public abstract class usrTGTogglTimerLink : BqlString.Field<usrTGTogglTimerLink>
        {
        }
        [PXUIField(DisplayName = "Toggle Timer Link")]
        [PXDBString]
        public virtual string UsrTGTogglTimerLink { get; set; }
        #endregion
    }
}
