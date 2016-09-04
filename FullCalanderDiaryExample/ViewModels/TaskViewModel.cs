using System;
using System.Web.ModelBinding;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FullCal.ViewModels;
using FullCal.Common;

namespace FullCal.ViewModels
{

    /// <summary>
    /// this class is fed directly to the JS FullCalendar scheduler in the browser
    /// a Schedule event represents a single Diary/Appointment/Calander entry
    /// please refer to my introductory article on using FullCalander for specifics:   http://www.codeproject.com/Articles/638674/Full-calendar-A-complete-web-diary-system-for-jQue
    /// </summary>
    public class ScheduleEvent 
    {
        public string id { get; set; }
        public string title { get; set; }
        public string branchId { get; set; }
        public string equipmentId { get; set; }
        public string resourceId { get; set; }
        public string sharedResourceId { get; set; } 
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string duration { get; set; }
        public int    durationMinutes { get; set; }
        public string statusId { get; set; }
        public string statusString { get; set; }
        public string color { get; set; }
        public string clientId { get; set; }
        public string clientName { get; set; }
        public string clientAddress { get; set; }
        public string clientPhone { get; set; }
        public string notes { get; set; }
        public bool  allDay { get; set; }
        public string repeatRule { get; set; }
        public string repeat { get; set; }
    }


    // locations are the name of the (customer location) / branch + User name
    public class TaskLocation
    {
        public string id { get; set; }
        public string title { get; set; }
    }
    public class ScheduleResource
    {
        public string id { get; set; } // required
        public string title { get; set; } // required
        public string notes { get; set; }
        // add any other general fields you might want to access in the browser to this property list
        public List<ScheduleResource> children { get; set; } // optional, but, required if you want 1:M relationship, ie: Branches have many Users .. build up list recursively

        public ScheduleResource()
        {
            children = new List<ScheduleResource>();
        }
    }




}