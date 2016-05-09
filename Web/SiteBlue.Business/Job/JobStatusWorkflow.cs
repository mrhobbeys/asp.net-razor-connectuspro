using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SiteBlue.Business.Job
{
    public static class JobStatusWorkflow
    {
        private enum JobStatus
        {
            Booked = 1,
            Scheduled = 2,
            Travel = 3,
            Active = 4,
            WrapUp = 5,
            Completed = 6,
            Closed = 7,
            WaitingParts = 8,
            WaitingPeople = 9,
            ReScheduled = 10,
            SentToTablet = 11,
            Cancelled = 12,
            WaitingEstimate = 13,
            WaitingWeather = 14,
            CompletedTestJob = 15,
            DeclinedEstimate = 16,
        }

        private static Dictionary<JobStatus, JobStatus[]> _states;
        private static Dictionary<JobStatus, JobStatus[]> GetStates()
        {
            return _states ?? (_states = new Dictionary<JobStatus, JobStatus[]>
                       {
                           {JobStatus.Booked, new[] {JobStatus.Cancelled, JobStatus.Scheduled}}
                       });
        }

        public static int[] GetAvailableStatuses(string[] roles, int currentStatus)
        {
            //JobStatus[] states;
            //if (!GetStates().TryGetValue((JobStatus)currentStatus, out states))
            //    states = new JobStatus[] {};

            var states = Enum.GetValues(typeof (JobStatus)).OfType<JobStatus>().ToList();

            if (!roles.Contains("CompanyOwner") && !roles.Contains("TicketRuler"))
            {
                states.Remove(JobStatus.Completed);
                states.Remove(JobStatus.Closed);
            }

            return states.Select(s => (int)s).ToArray();
        }

        public static bool Validate(string[] roles, int from, int to, out string msg)
        {
            var f = (JobStatus) from;
            var t = (JobStatus) to;

            if (!roles.Contains("CompanyOwner") && !roles.Contains("TicketRuler") && t == JobStatus.Closed || t == JobStatus.Completed)
            {
                msg = string.Format("You do not have permission to change job status from '{0}' to '{1}'", f, t);
                return false;
            }

            var states = GetStates();

            //if (!states.ContainsKey(f) || !states[f].Contains(t))
            //{
            //    msg = string.Format("A job cannot change state from '{0}' to '{1}'", f, t);
            //    return false;
            //}

            msg = string.Empty;
            return true;
        }
    }
}
