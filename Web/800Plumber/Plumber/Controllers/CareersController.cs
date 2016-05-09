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
    public class CareersController : Controller
    {
        private PlumberContext db = new PlumberContext();

        private string _message;
        
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
            var careers = db.Career.Where(c => c.IsArchived == false && c.IsDeleted == false).OrderByDescending(c => c.PublicationDate).ToList();
            return View(careers);
        }

        public ActionResult Application()
        {
            ViewBag.States = GetStates();
           
            return View();
        }

        [HttpPost]
        public ActionResult Application(CareerApplication model)
        {
            ViewBag.States = GetStates();

            if (ModelState.IsValid)
            {
                // Send the content to the database
                // Mark the career application as unread
                model.MessageStatusId = 1;
                db.CareerApplication.Add(model);
                db.SaveChanges();
                // Send an email to the administratorcareers@1800plumber.com
                string emailAddress = System.Configuration.ConfigurationManager.AppSettings["CareerEmail"].ToString();
                string link = System.Configuration.ConfigurationManager.AppSettings["website"].ToString() + "/Careers/ApplicationDetails/";
                if ((emailAddress == null) || (emailAddress == "")) emailAddress = "customerservice@1800plumber.com";
                var emailSent = SendEmail(emailAddress, link + model.CareerApplicationId, model);
                // Redirect the user to the confirmation page
                if (emailSent)
                    return RedirectToAction("Done");
                else
                    return View("Error");                
            }
            return View(model);
        }

        [Authorize]
        public ActionResult ApplicationDetails(int id)
        {
            var application = db.CareerApplication.Find(id);
            if (application != null)
                return View(application);
            else
                return View("Error");
        }

        public ActionResult Done()
        {
            ViewBag.Message = "Thank you for your application. Your information will be sent to the appropriate department. We appreciate your interest in 1-800-PLUMBER.";
            return View();
        }

        private bool SendEmail(string sendTo, string link, CareerApplication model)
        {
            var smtpServer = "mail.connectuspro.com";
            //var yourEmail = "requestforservice@1800plumber.com";
            var yourEmail = "info@connectuspro.com";
            var yourPassword = "Acceler8ted";

            //var TechSettings = "";
            //foreach (var item in Request.ServerVariables.AllKeys)
            //{
            //    TechSettings = String.Format("{0}{1}: {2}<br />", TechSettings, item, Request.ServerVariables[item]);
            //}
            var subject = "A career application identified by " + model.CareerApplicationId + " was created in 1800plumber.com at " + DateTime.Now;
            StringBuilder strBody = new StringBuilder();

            strBody.Append(String.Format("<font face=\"Arial\">Career application details: {0}<br /><br />", DateTime.Now));
            strBody.Append(String.Format("From http://{0}<br />", Request.ServerVariables["HTTP_HOST"]));
            strBody.Append(String.Format("IP Address : {0}<br />", Request.UserHostAddress));
            strBody.Append(String.Format("First and Last name : {0}<br />", model.FirstLastName));
            strBody.Append(String.Format("Phone : {0}<br />", model.Phone));
            strBody.Append(String.Format("Email : {0}<br />", model.Email));
            strBody.Append(String.Format("Subject : {0}<br />", subject));

            strBody.Append(String.Format("<br /><br /><a href=\"{0}\" target=\"_blank\">Click here to view the application on line</a>", link));

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
