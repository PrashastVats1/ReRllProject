using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobsPortal.Models
{
    public class EmployeeMV
    {
        public int EmployeeID { get; set; }
        public int UserId { get; set; }
        public string EmployeeName { get; set; }
        public System.DateTime? DOB { get; set; }
        public string Education { get; set; }
        public string WorkExperience { get; set; }
        public string Skills { get; set; }
        public string EmailAddress { get; set; }
        public string Gender { get; set; }
        public string Photo { get; set; }
        public string Qualification { get; set; }
        public string PermanentAddress { get; set; }
        public string JobReference { get; set; }
        public string Description { get; set; }
        public string Resume { get; set; }
    }
}
