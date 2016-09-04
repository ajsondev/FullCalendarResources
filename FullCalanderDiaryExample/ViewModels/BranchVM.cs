using FullCal.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System;

namespace FullCal.ViewModels
{
    // basic class representing some branch offices of an organisation
    // it is expected you would replace this with a database table for your own requirements
    public class BranchOfficeVM
    {
        public string BranchOfficeID { get; set; }
        public string Name { get; set; }
        public List<EmployeeVM> Employees { get; set; }


        public BranchOfficeVM()
        {
            Employees = new List<EmployeeVM>();
        }

        public BranchOfficeVM GetBranchByID(List<BranchOfficeVM> Branches, string BranchID)
        {
            return Branches.Where(s => s.BranchOfficeID == BranchID).FirstOrDefault();
        }

        public BranchOfficeVM GetBranchByName(List<BranchOfficeVM> Branches, string Name)
        {
            return Branches.Where(s => s.Name.Contains(Name)).FirstOrDefault();
        }

    }
}