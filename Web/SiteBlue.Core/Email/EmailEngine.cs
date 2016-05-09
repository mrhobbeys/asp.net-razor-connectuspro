using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
namespace SiteBlue.Core.Email
{
    public class EmailEngine
    {
        public string Host { get; private set; }
        public int Port { get; private set; }
        public string User { get; private set; }
        public string DefaultFrom { get; private set; }
        private readonly string _password;
        private bool useDefaultConfig = false;

        /// <summary>
        /// Creates an instance of a mail client using the
        /// configuration specified by the consumer.
        /// </summary>
        /// <returns></returns>
        private SmtpClient GetClient()
        {
            var c = useDefaultConfig ? new SmtpClient() : new SmtpClient(Host, Port);
            
            if (!useDefaultConfig && (!string.IsNullOrWhiteSpace(User) || !string.IsNullOrWhiteSpace(_password)))
                c.Credentials = new NetworkCredential(User, _password);

            return c;
        }

        /// <summary>
        /// Initializes an email engine using the default values from the current config file.
        /// </summary>
        public EmailEngine()
        {
            useDefaultConfig = true;
        }

        /// <summary>
        /// Initializes an email engine using the specified configuration data.
        /// </summary>
        /// <param name="host">The host name of the mail server.</param>
        /// <param name="port">The port number to use.</param>
        /// <param name="username">The username of the user to use for authentication (null or empty string for anonymous)</param>
        /// <param name="password">The password to use for authentication. (null or empty string for anonymous)</param>
        /// <param name="defaultFromAddress">The default from address to use if one is not specified on the mail message.</param>
        public EmailEngine(string host, int? port, string username, string password, string defaultFromAddress)
        {
            Host = host;
            User = username;
            Port = port ?? 25;
            _password = password;
            DefaultFrom = defaultFromAddress;
        }

        /// <summary>
        /// Sends a mail message.
        /// </summary>
        /// <param name="msg">The message to send.</param>
        /// <returns>a boolean flag indicating whether the message was sent successfully or not.</returns>
        public bool Send(MailMessage msg)
        {
            try
            {
                using (var client = GetClient())
                {
                    if (msg.From == null)
                        msg.From = new MailAddress(((NetworkCredential)client.Credentials).UserName);

                    if (!GlobalConfiguration.TestMode)
                        client.Send(msg);
                }
            }
            catch (Exception)
            {
                //TODO: log it!
                return false;
            }

            return true;
        }

        /// <summary>
        /// Sends an email with the specified information.
        /// </summary>
        /// <param name="from">The from address to use (null or empty string to use the default)</param>
        /// <param name="to">The recipient(s) to send to.  Separate multiple with comma (,) or semicolon (;) only!</param>
        /// <param name="cc">The recipient(s) to copy.  Separate multiple with comma (,) or semicolon (;) only!</param>
        /// <param name="bcc">The recipient(s) to blind.  Separate multiple with comma (,) or semicolon (;) only!</param>
        /// <param name="subject">The subject of the email</param>
        /// <param name="body">The content of the email.</param>
        /// <param name="attachments">Attachments to include (null or empty for none)</param>
        /// <param name="isHtml">A flag indicating whether HTML format should be used.</param>
        /// <returns>a boolean flag indicating whether the message was sent successfully or not.</returns>
        public bool Send(string from, string to, string cc, string bcc, string subject, string body, IEnumerable<Attachment> attachments, bool isHtml)
        {
            var msg = new MailMessage
                          {
                              Subject = subject,
                              Body = body,
                              IsBodyHtml = isHtml
                          };

            if (string.IsNullOrWhiteSpace(to) && string.IsNullOrWhiteSpace(cc) && string.IsNullOrWhiteSpace(bcc))
                throw new ApplicationException("No recipients were specified on the email request.");    

            if (!string.IsNullOrWhiteSpace(from))
                msg.From = new MailAddress(from);

            if (!string.IsNullOrWhiteSpace(to))
                msg.To.Add(to.Replace(";", ","));

            if (!string.IsNullOrWhiteSpace(cc))
                msg.CC.Add(cc.Replace(";", ","));
            
            if (!string.IsNullOrWhiteSpace(bcc))
                msg.Bcc.Add(bcc.Replace(";", ","));

            if (attachments != null)
                attachments.ToList().ForEach(msg.Attachments.Add);

            return Send(msg);
        }
    }
}
