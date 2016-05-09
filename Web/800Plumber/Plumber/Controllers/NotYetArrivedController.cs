using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Plumber.Models;
using System.Text;
using System.Net.Mail;

namespace Plumber.Controllers
{
    public class NotYetArrivedController : Controller
    {
        private PlumberContext db = new PlumberContext();
        //
        // GET: /NotYetArrived/

        public ActionResult Index(string zip, string error, int? service)
        {
            if ((error == "1") || (error == "2"))
            {
                var model = new NotYetArrivedModel();
                model.ZipCode = zip;
                return View(model);
            } else
                return View("Error");
        }


        [HttpPost]
        public ActionResult Index(string zip, string error, int? service, NotYetArrivedModel model)
        {
            if (ModelState.IsValid)
            {
                var smtpServer = "mail.connectuspro.com";
                //var yourEmail = "requestforservice@1800plumber.com";
                var yourEmail = "info@connectuspro.com";
                var yourPassword = "Acceler8ted";
                string sendTo = System.Configuration.ConfigurationManager.AppSettings["CustomerServiceEmail"].ToString();
                if ((sendTo == null) || (sendTo == "")) sendTo = "customerservice@1800plumber.com";
                //var sendTo = "anisboulaid@gmail.com";

                //// Test Mode
                //var smtpserver = "smtp.gmail.com";
                //var youremail = "xhps27@gmail.com";
                //var yourpassword = "awapassword";

                var TechSettings = "";
                foreach (var item in Request.ServerVariables.AllKeys)
                {
                    TechSettings = String.Format("{0}{1}: {2}<br />", TechSettings, item, Request.ServerVariables[item]);
                }
                string subject;//"ZIP code submitted at " + DateTime.Now;
                if (error == "1")
                    subject = "You received a notification: 1-800 PLUMBER has not yet arrived in " + zip;
                else if (error == "2")
                    if (service.HasValue)
                    {
                        var selectedservice = db.Services.Find(service);
                        if (selectedservice != null)
                        {
                            subject = "You received a notification: The service " + selectedservice.ServiceName + " is not yet offered in " + zip;
                        } else
                            subject = "You received a notification: A undefined service is not yet offered in " + zip;
                    }
                    else
                        subject = "You received a notification: A undefined service is not yet offered in " + zip;
                else
                    return View("Error");
                StringBuilder strBody = new StringBuilder();

                strBody.Append(String.Format("<font face=\"Arial\">ZIP {0}<br /><br />", DateTime.Now));
                strBody.Append(String.Format("From http://{0}<br />", Request.ServerVariables["HTTP_HOST"]));
                strBody.Append(String.Format("IP Address : {0}<br />", Request.UserHostAddress));
                strBody.Append(String.Format("ZIP : {0}<br />", model.ZipCode));
                strBody.Append(String.Format("Name : {0}<br />", model.Name));
                strBody.Append(String.Format("Email : {0}<br />", model.Email));
                strBody.Append(String.Format("Subject : {0}<br />", subject));
                strBody.Append(TechSettings + "<br />");
                strBody.Append("</font>");

                using (var mailMessage =
                    new MailMessage(new MailAddress(yourEmail),
                        new MailAddress(sendTo))
                    {
                        Subject = subject,
                        IsBodyHtml = true,
                        Body = strBody.ToString()
                    })
                {
                    System.Net.NetworkCredential networkCredentials = new System.Net.NetworkCredential(yourEmail, yourPassword);
                    using (SmtpClient smtpClient = new SmtpClient()
                    {
                        EnableSsl = false,
                        UseDefaultCredentials = false,
                        Credentials = networkCredentials,
                        Host = smtpServer,
                        Port = 25
                    })
                    {
                        try
                        {
                            smtpClient.Send(mailMessage);
                        }
                        catch (Exception ex)
                        {
                            ViewBag.ErrorMessage = ex.Message;
                            return RedirectToAction("Error", "Home");
                        }
                    }
                }

                return RedirectToAction("Index", "Home");

            }
            return View();
        }

    }
}
