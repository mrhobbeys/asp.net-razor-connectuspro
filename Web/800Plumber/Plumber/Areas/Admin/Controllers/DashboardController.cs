using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Plumber.Models;

namespace Plumber.Areas.Admin.Controllers
{
    [Authorize] //(Roles = "Administrator")
    public class DashboardController : Controller
    {
        private PlumberContext db = new PlumberContext();

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult DashboardSummary()
        {
            var unreadAppointments = db.Appointment.Where(a => a.MessageStatusId == 1).Count();
            var totalAppointments = db.Appointment.Count();
            var unreadCareerApplications = db.CareerApplication.Where(ca => ca.MessageStatusId == 1).Count();
            var totalCareerApplications = db.CareerApplication.Count();
            var unreadServiceFeedback = db.ServiceFeedback.Where(sf => sf.MessageStatusId == 1).Count();
            var totalServiceFeedback = db.ServiceFeedback.Count();
            return Json(new { appointments = unreadAppointments,
                appointmentsCount = totalAppointments,
                careerApplications = unreadCareerApplications,
                careerApplicationCount = totalCareerApplications,
                serviceFeedbacks = unreadServiceFeedback,
                serviceFeedbackCounts = totalServiceFeedback
            });
        }

        public ActionResult CareerApplications(int? id)
        {
            List<CareerApplication> careerApplications;
            if (id.HasValue)
                careerApplications = db.CareerApplication.Where(ca => ca.MessageStatusId == id.Value).ToList();
            else
                careerApplications = db.CareerApplication.ToList();
            return View(careerApplications);
        }

        public ActionResult Appointments(int? id)
        {
            List<Appointment> appointments;
            if (id.HasValue)
                appointments = db.Appointment.Where(a => a.MessageStatusId == id.Value).ToList();
            else
                appointments = db.Appointment.ToList();
            return View(appointments);
        }

        public ActionResult ServiceFeedbacks(int? id)
        {
            List<ServiceFeedback> serviceFeedback;
            if (id.HasValue)
                serviceFeedback = db.ServiceFeedback.Where(sf => sf.MessageStatusId == id.Value).OrderByDescending(sf => sf.CreationDate).ToList();
            else
                serviceFeedback = db.ServiceFeedback.OrderByDescending(sf => sf.CreationDate).ToList();
            return View(serviceFeedback);
        }

        public ActionResult CareerApplicationDetails(int id)
        {
            var careerApplication = db.CareerApplication.Find(id);
            // Update the status of the application
            careerApplication.MessageStatusId = 2;
            UpdateModel(careerApplication);
            db.SaveChanges();
            // redirect to the view
            return View(careerApplication);
        }

        public ActionResult AppointmentDetails(int id)
        {
            var appointment = db.Appointment.Find(id);
            // Update the status of the appointment
            appointment.MessageStatusId = 2;
            UpdateModel(appointment);
            db.SaveChanges();
            // redirect to the view
            return View(appointment);
        }

        public ActionResult ServiceFeedbackDetails(int id)
        {
            var serviceFeedback = db.ServiceFeedback.Find(id);
            // Update the status of the appointment
            serviceFeedback.MessageStatusId = 2;
            UpdateModel(serviceFeedback);
            db.SaveChanges();
            // redirect to the view
            return View(serviceFeedback);
        }

        public ActionResult Password()
        {
            ViewBag.Message = Security.GeneratePassword.generatePassword();
            return View();
        }

    }
}
