using FullCal.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System;

namespace FullCal.ViewModels
{
    // basic class representing some equipment assets of an organisation
    // it is expected you would replace this with a database table for your own requirements


    public class EquipmentVM 
    {
        public string id { get; set; }
        public string title { get; set; }
        public string branchId { get; set; }
        public string branchName { get; set; }
        public string notes { get; set; }

    }
}