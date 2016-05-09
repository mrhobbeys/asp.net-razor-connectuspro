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
    public class ServicesController : Controller
    {

        private PlumberContext db = new PlumberContext();

        //private string _message;

        private List<State> GetStates()
        {
            List<State> states = new List<State>();
            states.Add(new State() { ID = "", Name = "Please Choose" });
            states.Add(new State() { ID = "AL", Name = "Alabama" });
            states.Add(new State() { ID = "AK", Name = "Alaska" });
            states.Add(new State() { ID = "AS", Name = "American Samoa" });
            states.Add(new State() { ID = "AZ", Name = "Arizona" });
            states.Add(new State() { ID = "AR", Name = "Arkansas" });
            states.Add(new State() { ID = "CA", Name = "California" });
            states.Add(new State() { ID = "CO", Name = "Colorado" });
            states.Add(new State() { ID = "CT", Name = "Connecticut" });
            states.Add(new State() { ID = "DE", Name = "Delaware" });
            states.Add(new State() { ID = "DC", Name = "District of Columbia" });
            states.Add(new State() { ID = "FL", Name = "Florida" });
            states.Add(new State() { ID = "GA", Name = "Georgia" });
            states.Add(new State() { ID = "GU", Name = "Guam" });
            states.Add(new State() { ID = "HI", Name = "Hawaii" });
            states.Add(new State() { ID = "ID", Name = "Idaho" });
            states.Add(new State() { ID = "IL", Name = "Illinois" });
            states.Add(new State() { ID = "IN", Name = "Indiana" });
            states.Add(new State() { ID = "IA", Name = "Iowa" });
            states.Add(new State() { ID = "KS", Name = "Kansas" });
            states.Add(new State() { ID = "KY", Name = "Kentucky" });
            states.Add(new State() { ID = "LA", Name = "Louisiana" });
            states.Add(new State() { ID = "ME", Name = "Maine" });
            states.Add(new State() { ID = "MD", Name = "Maryland" });
            states.Add(new State() { ID = "MA", Name = "Massachusetts" });
            states.Add(new State() { ID = "MI", Name = "Michigan" });
            states.Add(new State() { ID = "MN", Name = "Minnesota" });
            states.Add(new State() { ID = "MS", Name = "Mississippi" });
            states.Add(new State() { ID = "MO", Name = "Missouri" });
            states.Add(new State() { ID = "MT", Name = "Montana" });
            states.Add(new State() { ID = "NE", Name = "Nebraska" });
            states.Add(new State() { ID = "NV", Name = "Nevada" });
            states.Add(new State() { ID = "NH", Name = "New Hampshire" });
            states.Add(new State() { ID = "NJ", Name = "New Jersey" });
            states.Add(new State() { ID = "NM", Name = "New Mexico" });
            states.Add(new State() { ID = "NY", Name = "New York" });
            states.Add(new State() { ID = "NC", Name = "North Carolina" });
            states.Add(new State() { ID = "ND", Name = "North Dakota" });
            states.Add(new State() { ID = "MP", Name = "Northern Mariana Islands" });
            states.Add(new State() { ID = "OH", Name = "Ohio" });
            states.Add(new State() { ID = "OK", Name = "Oklahoma" });
            states.Add(new State() { ID = "OR", Name = "Oregon" });
            states.Add(new State() { ID = "PA", Name = "Pennsylvania" });
            states.Add(new State() { ID = "PR", Name = "Puerto Rico" });
            states.Add(new State() { ID = "RI", Name = "Rhode Island and Providence Plantations" });
            states.Add(new State() { ID = "SC", Name = "South Carolina" });
            states.Add(new State() { ID = "SD", Name = "South Dakota" });
            states.Add(new State() { ID = "TN", Name = "Tennessee" });
            states.Add(new State() { ID = "TX", Name = "Texas" });
            states.Add(new State() { ID = "UT", Name = "Utah" });
            states.Add(new State() { ID = "VT", Name = "Vermont" });
            states.Add(new State() { ID = "VI", Name = "Virgin Islands" });
            states.Add(new State() { ID = "VA", Name = "Virginia" });
            states.Add(new State() { ID = "WA", Name = "Washington" });
            states.Add(new State() { ID = "WV", Name = "West Virginia" });
            states.Add(new State() { ID = "WI", Name = "Wisconsin" });
            states.Add(new State() { ID = "WY", Name = "Wyoming" });

            return states;
        }

        public ActionResult Index()
        {
            //return View(new ServiceModel().GetServices());
            return View(db.Service.ToList());
        }

        public ActionResult Details(int id)
        {
            var service = db.Service.Find(id);
            if (service != null)
                return View(service);
            else
                throw new HttpException(404, "Not found");

        }

        public ActionResult Schedule()
        {
            ViewBag.States = GetStates();
            return View();
        }

        [HttpPost]
        public ActionResult Schedule(Appointment model)
        {
            ViewBag.States = GetStates();
            if (ModelState.IsValid)
            {
                // Send the content to the database
                // Mark the appointment as unread
                model.MessageStatusId = 1;
                db.Appointment.Add(model);
                db.SaveChanges();
                // Send an email to the administrator
                string emailAddress = System.Configuration.ConfigurationManager.AppSettings["CustomerServiceEmail"].ToString();
                string link = System.Configuration.ConfigurationManager.AppSettings["website"].ToString() + "/Services/AppointmentDetails/";
                if ((emailAddress == null) || (emailAddress == "")) emailAddress = "customerservice@1800plumber.com";
                var emailSent = SendEmail(emailAddress, link + model.AppointmentId, model);
                // Redirect the user to the confirmation page
                if (emailSent)
                    return RedirectToAction("Done");
                else
                    return View("Error");

            }
            return View(model);
        }

        [Authorize]
        public ActionResult AppointmentDetails(int id)
        {
            var appointment = db.Appointment.Find(id);

            if (appointment != null)
                return View(appointment);
            else
                return View("Error");
        }

        public ActionResult Done()
        {
            ViewBag.Message = "Thank your for your service request. One of our Service Center professionals will contact you shortly. Thank you for visiting 1-800-PLUMBER.";

            return View();
        }

        private bool SendEmail(string sendTo, string link, Appointment model)
        {
            var smtpServer = "mail.connectuspro.com";
            //var yourEmail = "requestforservice@1800plumber.com";
            var yourEmail = "info@connectuspro.com";
            var yourPassword = "Acceler8ted";
            //var sendTo = "customerservice@1800plumber.com";
            //var sendTo = "anisboulaid@gmail.com";

            var subject = "Schedule an appointment form submitted at " + DateTime.Now;
            StringBuilder strBody = new StringBuilder();

            strBody.Append(String.Format("<a href=\"http://www.1800plumber.com\" target=\"_blank\"><img src=\"" + Url.Content("~/Content/images/1800-plumber.jpg") + "\" alt=\"\" /></a><br />"));

            strBody.Append(String.Format("<font face=\"Arial\">Schedule an appointment form submitted at {0}<br><br>", DateTime.Now));
            strBody.Append(String.Format("From http://{0}<br>", Request.ServerVariables["HTTP_HOST"]));
            strBody.Append(String.Format("IP Address : {0}<br>", Request.UserHostAddress));
            strBody.Append(String.Format("Postal Code : {0}<br>", model.PostalCode));
            strBody.Append(String.Format("Name : {0}<br>", model.Name));
            strBody.Append(String.Format("Address : {0}<br>", model.Address));
            strBody.Append(String.Format("City : {0}<br>", model.City));
            strBody.Append(String.Format("State : {0}<br>", model.State));
            strBody.Append(String.Format("Email : {0}<br>", model.Email));
            strBody.Append(String.Format("Phone : {0}<br>", model.Phone));
            strBody.Append(String.Format("Preferred date : {0}<br>", model.PreferredDate));
            strBody.Append(String.Format("Preferred time : {0}<br>", model.PreferredTime));
            strBody.Append(String.Format("Type of service : {0}<br>", model.TypeOfService));
            strBody.Append(String.Format("Subject : {0}<br>", subject));
            strBody.Append(String.Format("Comment : {0}<br />", model.Comment));
            strBody.Append("Schedule an appointment" + "<br>");

            strBody.Append(String.Format("<br /><br /><a href=\"{0}\" target=\"_blank\">Click here to view the details of the appointment on line</a>", link));

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
                        return false;
                    }
                }
            }

            return true;
        }

    }
}
