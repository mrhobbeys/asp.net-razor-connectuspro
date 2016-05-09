namespace SiteBlue.Business.Reporting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using SiteBlue.Data.Reporting;

    public class WSRDate
    {
        public int? ClientID { get; private set; }
        public DateTime? WSRCompletedDate { get; set; }

        protected void CopyFrom(vRpt_WSR_Dates rptwsrdate)
        {
            ClientID = rptwsrdate.ClientID;
            WSRCompletedDate = rptwsrdate.WSRCompletedDate;
        }

        internal static WSRDate MapFromModel(vRpt_WSR_Dates rptwsrdate)
        {
            var wsrdate = new WSRDate();
            wsrdate.CopyFrom(rptwsrdate);
            return wsrdate;
        }
    }
}
