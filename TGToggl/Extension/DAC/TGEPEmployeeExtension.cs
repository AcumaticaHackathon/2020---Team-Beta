using PX.Data;
using PX.Data.BQL;
using PX.Objects.EP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGToggl
{
    public class TGEPEmployeeExtension : PXCacheExtension<EPEmployee>
    {
        #region Token
        public abstract class usrTGToken : BqlString.Field<usrTGToken>
        {
        }
        [PXUIField(DisplayName = "Toggl API token")]
        [PXDBString(32)]
        public virtual string UsrTGToken { get; set; }
        #endregion
    }
}
