using NCrontab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FullCal.ViewModels;


namespace FullCal.Controllers
{
    public class HomeController : Controller
    {
        TestHarness testHarness; // used to store temp data repository

        public ActionResult Index(string ResourceView)
        {
            // create test harness if it does not exist
            // this harness represents interaction you may replace with database calls.
            if (Session["ResourceView"]!=null) // refers to whatever default view you may have, for example Offices/Users/Shared Equipment/etc .. you can create any amount of different view types you need.
                ResourceView = Session["ResourceView"].ToString();
            if (!System.IO.File.Exists(utils.GetTestFileLocation()))
                {
                testHarness = new TestHarness();
                testHarness.Setup();
                utils.Save(utils.GetTestFileLocation(), testHarness);
                }
            if (ResourceView == null)
                ResourceView = "";
            ResourceView = ResourceView.ToLower().Trim();
            var DiaryResourceView = new Resource();
            if (ResourceView == "" || ResourceView == "employees") // set the default
                { 
                DiaryResourceView.DefaultView = "employees";
                DiaryResourceView.ResourceTitle = "Branch offices";
                }
            else if (ResourceView == "equipment")
                {
                DiaryResourceView.DefaultView = ResourceView;
                DiaryResourceView.ResourceTitle = "Equipment list";
                }
            
            return View(DiaryResourceView);
        }


        // this method, called from the index page, sets a session variable for 
        // the user that gets looped back to the index page to tell it what view 
        // to display. branch/employee or Equipment.
        public ActionResult setView(string ResourceView)
        {
          Session["ResourceView"] = ResourceView;
          return  RedirectToAction("Index");
        }


        // example of controller to get unsasigned tasks and send back for feeding into table/div array for selection drag/drop by user
        public JsonResult GetUnnassignedTasks()
        {
            List<ScheduleEvent> rslt = new List<ScheduleEvent>();
            testHarness = utils.Load<TestHarness>(utils.GetTestFileLocation());

            var eventList = from e in testHarness.UnassignedEvents
                            select new
                            {
                                id = e.EventID,
                                clientId = e.clientId,
                                title = e.title,
                                start = e.DateTimeScheduled.ToString("s"),
                                end = e.DateTimeScheduledEnd.ToString("s"),
                                notes = e.notes,
                                duration = e.duration,
                                allDay = false
                            };

            var rows = eventList.ToArray();
            return Json(rows, JsonRequestBehavior.AllowGet);
        }

        // Get resource (branches or resources like projector/laptop/demo-tablet etc for a given branch)
        public JsonResult GetResources(string resourceView)
        {
            if (resourceView == null)
                resourceView = "employees";
            var resourceData = new List<ScheduleResource>();
            testHarness = utils.Load<TestHarness>(utils.GetTestFileLocation());

            if (resourceView.ToLower() == "employees")
            {
                foreach (var branch in testHarness.Branches)
                {
                    var res = new ScheduleResource();
                    res.id = branch.BranchOfficeID;
                    res.title = branch.Name;
                    foreach (var employee in branch.Employees)
                    {
                        var employeeRes = new ScheduleResource();
                        employeeRes.id = employee.EmployeeID;
                        employeeRes.title = employee.FullName;
                        res.children.Add(employeeRes);
                    }
                    resourceData.Add(res);  
                }
            }
            else if (resourceView.ToLower() == "equipment")
            {
                foreach (var equipment in testHarness.Equipment)
                {
                    var res = new ScheduleResource();
                    res.id = equipment.id;
                    res.title = equipment.title;
                    resourceData.Add(res);  
                }
               
            }


            return Json(resourceData, JsonRequestBehavior.AllowGet);
        }

        // controller to send some basic setup infor for example to the index page - down to your particular implementation to decide how to do this.
        public JsonResult GetSetupInfo()
        {
            // PID got from cookie as shared login with VI, for the moment, get it from var at top of controllers
            testHarness = utils.Load<TestHarness>(utils.GetTestFileLocation());
            SchedulerSetup Data = new SchedulerSetup();
            Data.Employees = testHarness.Employees;
            Data.Branches = testHarness.Branches;
            Data.Equipment = testHarness.Equipment;
            Data.Clients = testHarness.Clients;
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        // Main controller that returns scheduled diary events.
        public JsonResult GetScheduleEvents(string start, string end, string resourceView) {
            var testHarness = utils.Load<TestHarness>(utils.GetTestFileLocation()); // replaces database for test purposes
            DateTime Start = DateTime.Parse(start);
            DateTime End = DateTime.Parse(end);
            List<ScheduleEvent> EventItems = new List<ScheduleEvent>();
            IEnumerable<ScheduleEventVM> selectedEvents = null;
            IEnumerable<ScheduleEventVM> repeatEvents = null;
            if (resourceView == "employees")
            {
                selectedEvents = testHarness.ScheduleEvents.Where(s => (s.DateTimeScheduled >= Start & s.DateTimeScheduledEnd <= End & s.repeat == null));
                repeatEvents = testHarness.ScheduleEvents.Where(s => (s.repeat!=null));
                
            }
            else
            {
                selectedEvents = testHarness.ScheduleEvents.Where(s => (s.DateTimeScheduled >= Start & s.DateTimeScheduledEnd <= End) && s.equipmentID != null & s.repeat == null); // only get events that have equipment booked
                repeatEvents = testHarness.ScheduleEvents.Where(s => (s.repeat != null));
            }
            if (selectedEvents != null)
            { 

                foreach (var scheduleEvent in selectedEvents)
                {
                    ScheduleEvent itm = new ScheduleEvent();
                    itm.id = scheduleEvent.EventID;
                    if (scheduleEvent.title.Trim() == "")
                        itm.title = scheduleEvent.clientName;
                    else itm.title = scheduleEvent.title;
                    itm.start = scheduleEvent.DateTimeScheduled.ToString("s");
                    itm.end = scheduleEvent.DateTimeScheduledEnd.ToString("s");
                    itm.duration = scheduleEvent.duration.ToString();
                    itm.notes = scheduleEvent.notes;
                    itm.statusId = scheduleEvent.statusId;
                    itm.statusString = scheduleEvent.statusString;
                    itm.allDay = false; 
                    itm.EmployeeId = scheduleEvent.EmployeeId;
                    itm.clientId = scheduleEvent.clientId;
                    itm.clientName = scheduleEvent.clientName;
                    itm.equipmentId = scheduleEvent.equipmentID;
                    itm.EmployeeName = scheduleEvent.EmployeeName;
                    itm.repeat = scheduleEvent.repeat;
                    itm.color = scheduleEvent.statusString;
                    if (resourceView == "employees")
                        itm.resourceId = scheduleEvent.EmployeeId;
                    else itm.resourceId = scheduleEvent.equipmentID;
                    EventItems.Add(itm);
                }

if (repeatEvents!=null)
foreach (var rptEvnt in repeatEvents)
{
    var schedule = CrontabSchedule.Parse(rptEvnt.repeat);
    var nextSchdule = schedule.GetNextOccurrences(Start, End);
    foreach (var startDate in nextSchdule)
    { 
        ScheduleEvent itm = new ScheduleEvent();
        itm.id = rptEvnt.EventID;
        if (rptEvnt.title.Trim() == "")
            itm.title = rptEvnt.clientName;
        else itm.title = rptEvnt.title;
        itm.start = startDate.ToString("s");
        itm.end = startDate.AddMinutes(30).ToString("s");
        itm.duration = rptEvnt.duration.ToString();
        itm.notes = rptEvnt.notes;
        itm.statusId = rptEvnt.statusId;
        itm.statusString = rptEvnt.statusString;
        itm.allDay = false;
        itm.EmployeeId = rptEvnt.EmployeeId;
        itm.clientId = rptEvnt.clientId;
        itm.clientName = rptEvnt.clientName;
        itm.equipmentId = rptEvnt.equipmentID;
        itm.EmployeeName = rptEvnt.EmployeeName;
        itm.repeat = rptEvnt.repeat;
        itm.color = rptEvnt.statusString;
        if (resourceView == "employees")
            itm.resourceId = rptEvnt.EmployeeId;
        else itm.resourceId = rptEvnt.equipmentID;
        EventItems.Add(itm);
    }
}

            }          
            return Json(EventItems, JsonRequestBehavior.AllowGet);
        }

        // combined create/save event
        public void PushEvent(ScheduleEvent Event)
        {
            var testHarness = utils.Load<TestHarness>(utils.GetTestFileLocation());
            var schEvent = testHarness.ScheduleEvents.Where(s => s.EventID == Event.id).FirstOrDefault();
            bool LNewRecord = false;
            if (schEvent == null) // was unassigned, now being assigned
            {
                schEvent = new ScheduleEventVM();
                LNewRecord = true;

            }

            schEvent.clientId = Event.clientId;
            schEvent.clientName = Event.clientName;
            schEvent.clientAddress = Event.clientAddress;
            schEvent.clientPhone = Event.clientPhone;
            schEvent.BranchOfficeID = Event.branchId;
            schEvent.DateTimeScheduled = DateTime.Parse(Event.start);
            if (Event.end != null)
                schEvent.DateTimeScheduledEnd = DateTime.Parse(Event.end);
            else
                schEvent.DateTimeScheduledEnd = DateTime.Parse(Event.start).AddMinutes(30);
            TimeSpan span = schEvent.DateTimeScheduledEnd - schEvent.DateTimeScheduled;
            schEvent.duration = span.TotalMinutes.ToString();
            schEvent.durationMinutes = (int)span.TotalMinutes;
            schEvent.notes = Event.notes;
            schEvent.repeat = Event.repeat;
            schEvent.repeatRule = Event.repeatRule;
            schEvent.statusId = Event.statusId;
            schEvent.statusString = Event.statusString;
            schEvent.title = Event.title;
            schEvent.equipmentID = Event.equipmentId;
            schEvent.repeat = Event.repeat;
            var employeeutils = new EmployeeVM();
            var employee = employeeutils.EmployeeByID(testHarness.Employees, Event.EmployeeId);
            if (employee != null)
            {
                schEvent.EmployeeId = employee.EmployeeID;
                schEvent.EmployeeName = employee.FullName;
            }
            if (LNewRecord)
                testHarness.ScheduleEvents.Add(schEvent);

            utils.Save(utils.GetTestFileLocation(), testHarness);
        
        }

        // controller to delete an event record
        public void DeleteEvent(ScheduleEvent Event)
        {
            utils.delete(utils.GetTestFileLocation(), Event.id);
        
        }
        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}