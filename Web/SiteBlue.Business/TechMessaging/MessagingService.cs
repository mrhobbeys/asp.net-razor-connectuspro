using System;
using System.Data;
using System.Data.Objects;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using SiteBlue.Core;
using SiteBlue.Data.EightHundred;

namespace SiteBlue.Business.TechMessaging
{
    public class MessagingService : AbstractBusinessService
    {
        public OperationResult<bool> SendMessage(int inResponseTo, string message, bool markProcessed)
        {
            var result = new OperationResult<bool>();

            using (var db = new EightHundredEntities(UserKey))
            {
                var om = db.tbl_Dispatch_Message_History.Single(m => m.MessageHistoryID == inResponseTo);
                var techTab =
                    db.tbl_Franchise_Tablets.SingleOrDefault(
                        t => t.FranchiseID == om.FranchiseID && t.EmployeeID == om.TechID);

                if (techTab == null)
                {
                    result.Message = "No tablet could be found for the technician you replied to.";
                    return result;
                }

                var fileName = techTab.TabletNumber.Split('-').Last() + "_tbl_DispatchMessage.xml";

                string xml;
                if (!GetMsgXml(om.FranchiseID, fileName, message, out xml))
                {
                    result.Message = xml;
                    return result;
                }

                string msg;
                if (!SendMessageToTablet(db, xml, techTab.TabletNumber, fileName, out msg))
                {
                    result.Message = msg;
                    return result;
                }

                if (markProcessed && !om.ProcessedYN)
                {
                    om.ProcessedYN = true;
                    db.SaveChanges();
                }
            }
            result.ResultData = true;
            return result;
        }

        private static bool GetMsgXml(int franchiseId, string fileName, string msg, out string xml)
        {
            //9748_tbl_JobStatusMessage.xml
            //'<?xml version="1.0" encoding="utf-8"?>
            //'<tbl_JobStatus>
            //'  <FranchiseID>29</FranchiseID>
            //'  <LocationID>13081</LocationID>
            //'  <Status>Active</Status>
            //'  <SequenceID>1</SequenceID>
            //'  <JobType>Leak</JobType>
            //'  <JobDescription>test leak in ceiling</JobDescription>
            //'  <EstimateYN>No</EstimateYN>
            //'  <Message>Out of Gas</Message>
            //'</tbl_JobStatus>

            try
            {

                var sb = new StringBuilder();

                using (var tw = new StringWriter(sb))
                {
                    using (var writer = new XmlTextWriter(tw) { Formatting = Formatting.Indented, Indentation = 4 })
                    {
                        writer.WriteComment(fileName);
                        writer.WriteStartElement("tbl_JobStatus");
                        writer.WriteElementString("FranchiseID", Convert.ToString(franchiseId));
                        writer.WriteElementString("LocationID", "1");
                        writer.WriteElementString("Status", "Sent to Tablet");
                        writer.WriteElementString("SequenceID", "1");
                        writer.WriteElementString("JobType", "");
                        writer.WriteElementString("JobDescription", "");
                        writer.WriteElementString("EstimateYN", "No");
                        writer.WriteElementString("Message", msg);
                        writer.WriteEndElement();

                        writer.Close();
                    }
                }

                xml = sb.ToString();
                return true;

            }
            catch (Exception ex)
            {
                xml = ex.Message;
                return false;
            }
        }

        private static bool SendMessageToTablet(ObjectContext ctx, string msgXml, string tabletNumber, string fileName, out string resultMsg)
        {
            try
            {
                ctx.ExecuteStoreCommand(
                "EXECUTE [WriteStringToFile] @pv_path = {0}, @pv_filename = {1}, @pv_data = {2}, @pi_result_code = {3} OUTPUT, @pv_result_msg = {4} OUTPUT",
                new object[]
                    {
                        Path.Combine(GlobalConfiguration.SendToTabletDropPath, tabletNumber), fileName, msgXml, null, null
                    });

                resultMsg = "Sent.";
                return true;
            }
            catch (Exception ex)
            {
                resultMsg = ex.Message;
                return false;
            }
        }
    }
}
