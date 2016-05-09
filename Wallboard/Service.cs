using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Web;

namespace Wallboard
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class Service
    {
        private static Random random = new Random();
        private const string TimeSpanFormatStr = "{0:D2}:{1:D2}";
        [OperationContract]
        [WebGet]
        [AspNetCacheProfile("CacheFor5Seconds")]
        public WallData[] GetData()
        {
            var data = new List<WallData>();

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["UCCX"].ConnectionString))
            {
                using (
                    var cmd =
                        new SqlCommand(
                             "SELECT   case CSQName when '1800Plumber' then '1800Plumber' else 'ConnectusDefault'     end AS [Name], min(loggedinagents) AS [LoggedIn], min(availableagents) AS [Available], " +
                             "min(talkingAgents) AS [Talking], min(unavailableagents) AS [Unavailable], max(callsWaiting) AS [OnHold], " +
                             "max(convoldestcontact) AS [WaitTime], " +
                             "sum(callsAbandoned) AS [AbandonedCalls],  max(convAvgTalkDuration) AS [AverageTalk], max(convLongestTalkDuration) AS [LongestTalk], " +
                             "max(CONVERT(INT, ROUND(CONVERT(decimal(10,2), LongestWaitDuration)/1000, 0))) AS [LongestWait], " +
                             "max(CONVERT(INT, ROUND(CONVERT(decimal(10,2), AvgWaitDuration)/1000, 0))) AS [AverageWait], " +
                             "sum(totalCalls) as [TotalCalls] " +
                             "FROM [RtCSQsSummary]" +
                             "WHERE CSQName IN ('1800Plumber','ConnectusDefault', 'Schaal')" +
                             "group by case CSQName when '1800Plumber' then '1800Plumber' else 'ConnectusDefault'     end" 
                             ,
                            conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var longestWait = TimeSpan.FromSeconds(Convert.ToInt32(reader["LongestWait"]));
                            var avgWait = TimeSpan.FromSeconds(Convert.ToInt32(reader["AverageWait"]));
                            data.Add(new WallData
                                         {
                                             Name = Convert.ToString(reader["Name"]),
                                             AgentsAvailable = Convert.ToInt32(reader["Available"]),
                                             AgentsLoggedIn = Convert.ToInt32(reader["LoggedIn"]),
                                             AgentsTalking = Convert.ToInt32(reader["Talking"]),
                                             AgentsUnavailable = Convert.ToInt32(reader["Unavailable"]),
                                             OnHold = Convert.ToInt32(reader["OnHold"]),
                                             LongestHoldTime = Convert.ToString(reader["WaitTime"]),
                                             AverageWait = string.Format(TimeSpanFormatStr, (int)avgWait.TotalMinutes, avgWait.Seconds),
                                             AbandonedCalls = Convert.ToInt32(reader["AbandonedCalls"]),
                                             //AverageTalk = Convert.ToString(reader["AverageTalk"]),
                                             //LongestTalk = Convert.ToString(reader["LongestTalk"]),
                                             LongestWait = string.Format(TimeSpanFormatStr, (int)longestWait.TotalMinutes, longestWait.Seconds),
                                             TotalCalls = Convert.ToInt32(reader["TotalCalls"])
                                         });
                        }
                    }
                }
            }
            return data.ToArray();
        }

        [OperationContract]
        [WebGet]
        [AspNetCacheProfile("CacheFor5Seconds")]
        public WallData[] GetMockData()
        {
            var longestWait = TimeSpan.FromSeconds(random.Next(0, 400));
            var avgWait = TimeSpan.FromSeconds(random.Next(0, 400));

            var data = new List<WallData>
                           {
                               new WallData
                                   {
                                       Name = "Plumber",
                                       AgentsAvailable = random.Next(0, 10),
                                       AgentsLoggedIn = random.Next(0, 10),
                                       AgentsTalking = random.Next(0, 10),
                                       AgentsUnavailable = random.Next(0, 10),
                                       OnHold = random.Next(0, 10),
                                       LongestHoldTime = "0:00:" + random.Next(0, 59).ToString("N2"),
                                       AverageWait = string.Format(TimeSpanFormatStr, (int)avgWait.TotalMinutes, avgWait.Seconds),
                                       AbandonedCalls = random.Next(0,100),
                                       //AverageTalk = "0:00:" + random.Next(0, 59).ToString("N2"),
                                       //LongestTalk = "0:00:" + random.Next(0, 59).ToString("N2"),
                                       LongestWait = string.Format(TimeSpanFormatStr, (int)longestWait.TotalMinutes, longestWait.Seconds),
                                       TotalCalls = random.Next(0,100)
                                   },
                               new WallData
                                   {
                                       Name = "Connectus",
                                       AgentsAvailable = random.Next(0, 10),
                                       AgentsLoggedIn = random.Next(0, 10),
                                       AgentsTalking = random.Next(0, 10),
                                       AgentsUnavailable = random.Next(0, 10),
                                       OnHold = random.Next(0, 10),
                                       LongestHoldTime = "0:00:" + random.Next(0, 59),
                                       AverageWait = string.Format(TimeSpanFormatStr, (int)avgWait.TotalMinutes, avgWait.Seconds),
                                       AbandonedCalls = random.Next(0,100),
                                       //AverageTalk = "0:00:" + random.Next(0, 59).ToString("N2"),
                                       //LongestTalk = "0:00:" + random.Next(0, 59).ToString("N2"),
                                       LongestWait = string.Format(TimeSpanFormatStr, (int)longestWait.TotalMinutes, longestWait.Seconds),
                                       TotalCalls = random.Next(0,100)
                                   }
                           };

            return data.ToArray();
        }
    }
}