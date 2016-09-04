using FullCal.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System;

namespace FullCal.ViewModels
{
    // basic class representing some employees of an organisation
    // it is expected you would replace this with a database table for your own requirements
    public class EmployeeVM
    {
        public string EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OfficeID { get; set; }
        public string PrimaryOfficeID { get; set; }
        public List<BranchOfficeVM> Offices {get; set;}

        public EmployeeVM()
        {
            Offices = new List<BranchOfficeVM>();
        }

        public string FullName
        {
            get { return string.Format("{0} {1}", FirstName, LastName).Trim(); }
        }


        public EmployeeVM EmployeeByName(List<EmployeeVM> Employees, string Name)
        {
             return Employees.Where(s => s.FullName.Contains(Name)).FirstOrDefault();
        }

        public EmployeeVM EmployeeByID(List<EmployeeVM> Employees, string ID)
        {
            return Employees.Where(s => s.EmployeeID == ID).FirstOrDefault();
        }

    }
}