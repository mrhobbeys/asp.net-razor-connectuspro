using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SiteBlue.Data;

namespace SiteBlue.Business.Reporting
{
    public class CallStates
    {
        public int SequenceNo { get; private set; }
        public DateTime CallTime { get; private set; }
        public string CalledNo { get; private set; }
        public string CalledDescription { get; private set; }
        public string CallerId { get; private set; }
        public TimeSpan? Duration { get; private set; }
        public int? JobId { get; private set; }
        public string UserName { get; private set; }
        public string OptionName { get; private set; }


        protected void CopyFrom(VADM_CallStats callstate)
        {
            SequenceNo = callstate.seqnr;
            CallTime = callstate.calltime;
            CalledNo = callstate.CalledNumber;
            CalledDescription = callstate.CalledDescription;
            CallerId = callstate.callerid;
            Duration = callstate.Duration;
            JobId = callstate.Jobid;
            UserName = callstate.UserName;
            OptionName = callstate.OptionName;
        }

        internal static CallStates MapFromModel(VADM_CallStats callstate)
        {
            var cs = new CallStates();
            cs.CopyFrom(callstate);
            return cs;
        }


    }
}
