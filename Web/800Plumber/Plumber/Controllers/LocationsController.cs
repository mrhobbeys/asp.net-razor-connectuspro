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
    public class LocationsController : Controller
    {

        private PlumberContext db = new PlumberContext();

        private string _message;

        //
        // GET: /Locations/

        public ActionResult Index()
        {
            var locations = db.Location.Where(l => l.IsDeleted == false).ToList();
            //var locations = new LocationModel().GetLocations();
            ViewBag.Title = "Plumbers and Plumbing Repair Services | 1-800-PLUMBER";
            return View(locations);
        }

        //
        // GET: /Special/

        public ActionResult Special(int id)
        {
            var location = db.Location.Find(id);
            ViewBag.Template = "Default";
            if ((location != null) && (location.IsDeleted == false))
                return View(location);
            else
                //return View("Error");
                throw new HttpException(404, "Not found");
        }

        public ActionResult List()
        {
            var specialOffers = new SpecialOfferModel().GetSpecialOffers();
            return View(specialOffers);
        }

        public JsonResult ServiceFeedback(int? locationId, string name, string email, string comment)
        {
            if (locationId.HasValue)
            {
                // Initialize a new instance of ServiceFeedback
                ServiceFeedback serviceFeedback = new ServiceFeedback();
                serviceFeedback.LocationId = locationId.Value;
                serviceFeedback.Name = name;
                serviceFeedback.Email = email;
                serviceFeedback.Comment = comment;
                serviceFeedback.CreationDate = DateTime.Now;
                serviceFeedback.MessageStatusId = 1;
                // Insert the new ServiceFeedback to the database
                db.ServiceFeedback.Add(serviceFeedback);
                db.SaveChanges();
                // Send a notification email
                string emailAddress = System.Configuration.ConfigurationManager.AppSettings["ServiceFeedbackEmail"].ToString();
                if ((emailAddress == null) || (emailAddress == "")) emailAddress = "customerservice@1800plumber.com";
                var emailSent = SendEmail(emailAddress, serviceFeedback, serviceFeedback.CreationDate);
                // Redirect the user to the confirmation page
                return Json(new { url = Url.Content("~/Locations/Done") });
            } else // redirect the user the main location page
                return Json(new { url = Url.Content("~/Locations") });
        }

        public ActionResult Done()
        {
            ViewBag.Message = "Thank you for your feedback regarding 1-800-PLUMBER. We appreciate your comments and will respond within the next business day.";
            return View();
        }

        private bool SendEmail(string sendTo, ServiceFeedback model, DateTime creationDate)
        {
            var smtpServer = "mail.connectuspro.com";
            //var yourEmail = "requestforservice@1800plumber.com";
            var yourEmail = "info@connectuspro.com";
            var yourPassword = "Acceler8ted";

            var location = db.Location.Find(model.LocationId);
            var subject = "Service feedback left by " + model.Name + (location != null ? " at " + location.LocationName : "") + " in 1800plumber.com at " + creationDate;
            StringBuilder strBody = new StringBuilder();

            strBody.Append(String.Format("<font face=\"Arial\">Career application details: {0}<br /><br />", DateTime.Now));
            strBody.Append(String.Format("From http://{0}<br />", Request.ServerVariables["HTTP_HOST"]));
            strBody.Append(String.Format("IP Address : {0}<br />", Request.UserHostAddress));
            strBody.Append(String.Format("Name : {0}<br />", model.Name));
            strBody.Append(String.Format("Email : {0}<br />", model.Email));
            strBody.Append(String.Format("Location : {0}<br />", location.LocationName));
            strBody.Append(String.Format("Comment : {0}<br />", model.Comment));
            strBody.Append(String.Format("Subject : {0}<br />", subject));

            //strBody.Append(String.Format("<br /><br /><a href=\"{0}\" target=\"_blank\">Click here to view the application on line</a>", link));

            //strBody.Append(TechSettings + "<br />");
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
                        _message = ex.Message;
                        return false;
                    }
                }
            }

            return true;
        }

    }
}
