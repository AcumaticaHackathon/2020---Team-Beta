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
        [PXUIField(DisplayName = "Toggl Timer Link")]
        [PXDBString]
        public virtual string UsrTGTogglTimerLink { get; set; }
        #endregion

        #region UsrTGLat
        public abstract class usrTGLat : BqlDecimal.Field<usrTGLat>
        {
        }
        [PXUIField( DisplayName = "Latitude")]
        [PXDBDecimal]
        public virtual decimal? UsrTGLat { get; set; }
        #endregion

        #region UsrTGLon
        public abstract class usrTGLon : BqlDecimal.Field<usrTGLon>
        {
        }
        [PXUIField(DisplayName = "Longitude")]
        [PXDBDecimal]
        public virtual decimal? UsrTGLon { get; set; }
        #endregion
    }
}
