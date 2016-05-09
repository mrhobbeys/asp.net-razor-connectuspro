using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SiteBlue.Models;
using System.Text;
using System.Web.Helpers;
using System.Net.Mail;

namespace SiteBlue.Controllers
{
    public class ContactController : Controller
    {
        //
        // GET: /Contact/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(ContactUsModel model)
        {
            if ((ModelState.IsValid))
            {
                var TechSettings = "";
                foreach (var item in Request.ServerVariables.AllKeys)
                {
                    TechSettings = String.Format("{0}{1}: {2}<br />", TechSettings, item, Request.ServerVariables[item]);
                }
                var subject = "Contact Us Form submitted at " + DateTime.Now;
                var strBody = new StringBuilder();

                strBody.Append(String.Format("<font face=\"Arial\">Contact Us Form submitted at {0}<br><br>", DateTime.Now));
                strBody.Append(String.Format("From http://{0}<br>", Request.ServerVariables["HTTP_HOST"]));
                strBody.Append(String.Format("IP {0}<br>", Request.UserHostAddress));
                strBody.Append(String.Format("Name : {0}<br>", model.Name));
                strBody.Append(String.Format("Email : {0}<br>", model.EmailAddress));
                strBody.Append(String.Format("Phone : {0}<br>", model.Phone));
                strBody.Append(String.Format("Cellphone : {0}<br>", model.CellPhone));
                strBody.Append(String.Format("Subject : {0}<br>", subject));
                strBody.Append(String.Format("<br>{0}<br><br><br><br>", model.Message));
                strBody.Append("Technical Information of customer" + "<br>");
                strBody.Append(TechSettings + "<br>");
                strBody.Append("</font>");
                using (var mailMessage =
                    new MailMessage
                    {
                        Subject = subject,
                        IsBodyHtml = true,
                        Body = strBody.ToString()
                    })
                {
                    using (var smtpClient = new SmtpClient())
                    {
                        try
                        {
                            var fromTo = ((NetworkCredential)smtpClient.Credentials).UserName;
                            mailMessage.From = new MailAddress(fromTo);
                            mailMessage.To.Add(fromTo);
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
