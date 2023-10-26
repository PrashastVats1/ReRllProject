using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using DatabaseAccessLayer;
using JobsPortal.Models;
using JobsPortal.ViewModels;
using log4net;

namespace JobsPortal.Controllers
{
    public class EmployeesTablesController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EmployeesTablesController));
        private JobsPortalDbEntities db = new JobsPortalDbEntities();
        // GET: EmployeesTables/Details
        public ActionResult Details()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserTypeID"])))
            {
                return RedirectToAction("Login", "User");
            }
            log.Info("Details action called");
            int employeeId = 0;
            int.TryParse(Convert.ToString(Session["EmployeeID"]), out employeeId);

            if (employeeId == 0)
            {
                log.Warn("EmployeeID session value is missing or zero");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            EmployeesTable employeesTable = db.EmployeesTables.Find(employeeId);
            if (employeesTable == null)
            {
                log.Error($"Employee with ID {employeeId} not found");
                return HttpNotFound();
            }
            return View(employeesTable);
        }

        // GET: EmployeesTables/Edit
        public ActionResult Edit()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserTypeID"])))
            {
                return RedirectToAction("Login", "User");
            }
            log.Info("Edit action called");
            int userId = 0;
            int employeeId = 0;
            int.TryParse(Convert.ToString(Session["UserID"]), out userId);
            int.TryParse(Convert.ToString(Session["EmployeeID"]), out employeeId);

            if (employeeId == 0)
            {
                log.Warn("EmployeeID session value is missing or zero");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            EmployeesTable employeesTable = db.EmployeesTables.Find(employeeId);
            if (employeesTable == null)
            {
                log.Error($"Employee with ID {employeeId} not found");
                return HttpNotFound();
            }

            ViewBag.UserId = new SelectList(db.UserTables, "UserID", "UserName", employeesTable.UserId);
            return View(employeesTable);
        }

        // POST: EmployeesTables/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeID,UserId,EmployeeName,DOB,Education,WorkExperience,Skills,EmailAddress,Gender,Photo,Qualification,PermanentAddress,JobReference,Description,Resume")] EmployeesTable employeesTable, HttpPostedFileBase photo, HttpPostedFileBase file)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserTypeID"])))
            {
                return RedirectToAction("Login", "User");
            }
            if (ModelState.IsValid)
            {
                // Photo Upload
                if (photo != null && photo.ContentLength > 0)
                {
                    string photoname = Path.GetFileNameWithoutExtension(photo.FileName);
                    string extension = Path.GetExtension(photo.FileName);
                    string uniquePhotoname = $"{photoname}_{employeesTable.EmployeeID}{extension}";
                    string photopath = Path.Combine(Server.MapPath("~/UploadImages/"), uniquePhotoname);
                    photo.SaveAs(photopath);

                    employeesTable.Photo = uniquePhotoname;
                }

                // Resume Upload
                if (file != null && file.ContentLength > 0)
                {
                    string filename = Path.GetFileNameWithoutExtension(file.FileName);
                    string extension = Path.GetExtension(file.FileName);
                    string uniqueFilename = $"{filename}_{employeesTable.EmployeeID}{extension}";
                    string filepath = Path.Combine(Server.MapPath("~/UploadResumes/"), uniqueFilename);
                    file.SaveAs(filepath);

                    employeesTable.Resume = uniqueFilename;
                }
                db.Entry(employeesTable).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details");
            }
            ViewBag.UserId = new SelectList(db.UserTables, "UserID", "UserName", employeesTable.UserId);
            return View(employeesTable);
        }

        // GET: EmployeesTables/Apply
        public ActionResult Apply()
        {
            // Check if the user's UserTypeID session variable is empty or not set.
            if (string.IsNullOrEmpty(Convert.ToString(Session["UserTypeID"])))
            {
                return RedirectToAction("Login", "User");
            }
            log.Info("Apply action called");

            // Initialize ids using session IDs
            int userId = 0;
            int employeeId = 0;
            int.TryParse(Convert.ToString(Session["UserID"]), out userId);
            int.TryParse(Convert.ToString(Session["EmployeeID"]), out employeeId);

            if (employeeId == 0)
            {
                log.Warn("EmployeeID session value is missing or zero");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Retrieve the employee's data using the EmployeeID.
            EmployeesTable employeesTable = db.EmployeesTables.Find(employeeId);
            if (employeesTable == null)
            {
                log.Error($"Employee with ID {employeeId} not found");
                return HttpNotFound();
            }

            // Convert the Employee data into the ViewModel.
            JobApplicationEmailVM application = new JobApplicationEmailVM
            {
                EmployeeName = employeesTable.EmployeeName,
                DOB = employeesTable.DOB,
                Education = employeesTable.Education,
                WorkExperience = employeesTable.WorkExperience,
                Skills = employeesTable.Skills,
                EmailAddress = employeesTable.EmailAddress,
                Gender = employeesTable.Gender,
                PhotoPath = employeesTable.Photo,
                Qualification = employeesTable.Qualification,
                PermanentAddress = employeesTable.PermanentAddress,
                JobReference = employeesTable.JobReference,
                Description = employeesTable.Description,
                ResumePath = employeesTable.Resume,
                CompanyEmail = "prashastvatsreborn@gmail.com"
            };
            // Render the "Apply" view with the ViewModel to display details and a "Send Application" button.
            // This will be the view where they can review details and click a "Send Application" button.
            return View(application);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Apply(JobApplicationEmailVM application)
        {
            // Check if the model state is not valid
            if (!ModelState.IsValid)
            {
                return View(application);
            }

            // Construct the email content using the ViewModel
            // Construct the email subject based on the user's name.
            string emailSubject = $"Job Application: {application.EmployeeName}";
            // Construct the email body with details from the ViewModel.
            string emailBody = $@"Employee Name: {application.EmployeeName}
DOB: {application.DOB}
Education: {application.Education}
Work Experience: {application.WorkExperience}
Skills: {application.Skills}
Email Address: {application.EmailAddress}
Gender: {application.Gender}
Qualification: {application.Qualification}
Permanent Address: {application.PermanentAddress}
Job Reference: {application.JobReference}
Description: {application.Description}";

            // Optionally, if we want to attach the resume:
            // Create a list to hold email attachments.
            List<Attachment> attachments = new List<Attachment>();
            // Check if a resume path is provided and attach it to the email if it exists.
            if (!string.IsNullOrWhiteSpace(application.ResumePath))
            {
                attachments.Add(new Attachment(Server.MapPath("~/UploadResumes/" + application.ResumePath)));
            }

            // Send the job application email using a helper method.
            bool emailSent = JobsPortal.Helper.MailHelper.SendJobApplicationEmail(application.CompanyEmail, application.EmployeeName, Server.MapPath("~/UploadResumes/" + application.ResumePath), emailBody);

            if (emailSent)
            {
                // Log an informational message and set a TempData message to indicate success.
                log.Info($"Job application email sent to {application.CompanyEmail}.");
                TempData["Message"] = "Your job application has been sent successfully!";
                // Redirect to the "FilterJob" action of the "Job" controller.
                return RedirectToAction("FilterJob", "Job");
            }
            else
            {
                log.Warn($"Failed to send job application email to {application.CompanyEmail}.");
                ModelState.AddModelError("", "Failed to send job application. Please try again later.");
                return View(application);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
