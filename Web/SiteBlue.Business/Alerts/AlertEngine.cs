using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using SiteBlue.Core.Email;
using SiteBlue.Data.EightHundred;
using System.Configuration;

namespace SiteBlue.Business.Alerts
{
    public class AlertEngine : AbstractBusinessService
    {
        /// <summary>
        /// Sends an alert to all registered recipients in a franchise for a given event.  THIS FUNCTION
        /// SWALLOWS EXCEPTIONS.
        /// </summary>
        /// <param name="alertType">The type of alert to send.</param>
        /// <param name="franchiseId">The franchiseId to use when getting the list of recipients to send to.</param>
        /// <returns>a boolean indicating whether emails were successfully sent.  Also returns true if no alerts are found.</returns>
        /// 
        public bool SendAlert(AlertType alertType, int franchiseId)
        {
            try
            {
                var type = (int)alertType;
                var toSend = new List<MailMessage>();

                using (var ctx = new EightHundredEntities(UserKey))
                {
                    var alerts = (from a in ctx.tbl_OwnerAlerts
                                  join d in ctx.tbl_OwnerAlertDestinations
                                      on a.OwnerAlertId equals d.OwnerAlertId
                                  join t in ctx.tbl_OwnerAlertCommunicationTypes
                                      on d.OwnerAlertCommunicationTypeID equals t.OwnerAlertCommunicationTypeId
                                  where a.FranchiseID == franchiseId
                                        && a.OwnerAlertTypeId == type
                                        && d.OwneralertEnabledYN
                                        && a.OwneralertEnabledYN
                                  select
                                      new
                                      {
                                          Subject = a.Subject,
                                          Body = a.Descriptions,
                                          Address = d.OwnerAlertDestinationText,
                                          Type = d.OwnerAlertCommunicationTypeID,
                                          DestText = d.OwnerAlertDestinationAdditionalText
                                      }).GroupBy(g => new { g.Subject, g.Body })
                                            .ToDictionary(g => g.Key, g => string.Join(",", g.Select(m => BuildAddress(m.Address, m.DestText, (AlertDestinationType)m.Type)).ToArray()));

                    if (alerts.Count == 0) return true;

                    foreach (var pair in alerts)
                    {
                        var msg = new MailMessage();
                        msg.To.Add(pair.Value);
                        msg.Subject = pair.Key.Subject;
                        msg.Body = pair.Key.Body;
                        toSend.Add(msg);
                    }
                }

                var engine = new EmailEngine();
                var allSent = toSend.Select(engine.Send).All(r => r);

                if (!allSent)
                {
                    //TODO: Log info that some alerts could not be sent.
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string BuildAddress(string address, string addtlText, AlertDestinationType type)
        {
            if (type == AlertDestinationType.Email) return address;

            var domain = string.Empty;

            switch ((addtlText ?? string.Empty).ToLower())
            {
                case "sprint":
                    domain = "messaging.sprintpcs.com";
                    break;
                case "verizon":
                    domain = "vtext.com";
                    break;
                case "at&t":
                    domain = "txt.att.net";
                    break;
                case "tmobile":
                    domain = "tmomail.net";
                    break;
            }
            return string.Format("{0}@{1}", address, domain);
        }
    }
}
