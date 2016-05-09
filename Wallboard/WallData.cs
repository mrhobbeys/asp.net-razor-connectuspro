using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wallboard
{
    /// <summary>
    /// Summary description for WallData
    /// </summary>
    public class WallData
    {
        public string Name { get; set; }
        public int OnHold { get; set; }
        public string LongestHoldTime { get; set; }
        public int AgentsAvailable { get; set; }
        public int AgentsTalking { get; set; }
        public int AgentsLoggedIn { get; set; }
        public int AgentsUnavailable { get; set; }
        public string AverageWait { get; set; }
        public string LongestWait { get; set; }
        public int TotalCalls { get; set; }
        public int AbandonedCalls { get; set; }
        public string AverageTalk { get; set; }
        public string LongestTalk { get; set; }
    }
}