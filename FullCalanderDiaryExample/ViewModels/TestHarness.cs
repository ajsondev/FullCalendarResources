using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

/// <summary>
/// This class emulates core database load and management - you will need to extract the context and remap functions to the live VI database as needed
/// </summary>
namespace FullCal.ViewModels
{
    // cal/diary items -- change to mirror your database structure as needed
    public class ScheduleEventVM
    {
        public string EventID { get; set; }
        public DateTime DateTimeScheduled { get; set; }
        public DateTime DateTimeScheduledEnd { get; set; }
        public DateTime DateTimeStart { get; set; }
        public DateTime DateTimeEnd { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string equipmentID { get; set; }
        public string duration { get; set; }
        public int durationMinutes { get; set; }
        public string BranchOfficeID { get; set; }
        public string title { get; set; }
        public string statusId { get; set; }
        public string statusString { get; set; }
        public string color { get; set; }
        public string clientId { get; set; }
        public string clientName { get; set; }
        public string clientAddress { get; set; }
        public string clientPhone { get; set; }
        public string notes { get; set; }
        public bool allDay { get; set; }
        public string repeat { get; set; }
        public string repeatRule { get; set; }

        public ScheduleEventVM()
        {
            EventID = Guid.NewGuid().ToString();
        }

    }

    // this carrys the 'default view' betwen browser and server, telling it to show for example, employee view, equipment view, room booking view, etc etc...
    public class Resource
    {
        public string DefaultView { get; set; }
        public string ResourceTitle { get; set; }
    }

    // used to send initial setup info to the scheduler page in the browser
    public class SchedulerSetup
    {
        public string ResourceView { get; set; }
        public List<EmployeeVM> Employees { get; set; }
        public List<EquipmentVM> Equipment { get; set; }
        public List<BranchOfficeVM> Branches { get; set; }
        public List<ClientVM> Clients { get; set; }

        public SchedulerSetup()
        {
            Employees = new List<EmployeeVM>();
            Equipment = new List<EquipmentVM>();
            Branches = new List<BranchOfficeVM>();
            Clients = new List<ClientVM>();
        }
    }

    // main Test harness class - represents the database you would use
    public class TestHarness
    {
        public List<BranchOfficeVM> Branches { get; set; }
        public List<ClientVM> Clients { get; set; }
        public List<EquipmentVM> Equipment { get; set; }
        public List<EmployeeVM> Employees { get; set; }
        public List<ScheduleEventVM> ScheduleEvents { get; set; }
        public List<ScheduleEventVM> UnassignedEvents { get; set; }

        // constructor
        public TestHarness()
        {
            Branches = new List<BranchOfficeVM>();
            Equipment = new List<EquipmentVM>();
            Employees = new List<EmployeeVM>();
            ScheduleEvents = new List<ScheduleEventVM>();
            UnassignedEvents = new List<ScheduleEventVM>();
            Clients = new List<ClientVM>();
        }

        // saves TestHarness snapshot
        public void Save()
        {
            utils.Save(utils.GetTestFileLocation(), this);
        }




        /*
            List of methods to initialize the test harness with data - not needed in production
        */

        // initial setup if none already exists to load
        public void Setup()
        {
            initClients();
            initUnAssignedTasks();
            initBranches();
            initEmployees();
            linkEmployeesToBranches();
            initEquipment();
            initEvents();
        }

        public void initEvents()
        {
            var utilBranch = new BranchOfficeVM();
            var EmployeeUtil = new EmployeeVM();
            var s1 = new ScheduleEventVM();
            s1.BranchOfficeID = utilBranch.GetBranchByName(Branches, "New York").BranchOfficeID;
            var c1 = utils.GetClientByName(Clients, "Big Company A");
            s1.clientId = c1.ClientID;
            s1.clientName = c1.Name;
            s1.clientAddress = c1.Address;
            s1.title = "Event 2 - Big Company A";
            s1.statusString = Constants.statusBooked;
            var v1 = EmployeeUtil.EmployeeByName(Employees, "Paul");
            s1.EmployeeId = v1.EmployeeID;
            s1.EmployeeName = v1.FullName;
            s1.DateTimeScheduled = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 11, 15, 0);
            s1.durationMinutes = 120;
            s1.duration = s1.durationMinutes.ToString();
            s1.DateTimeScheduledEnd = s1.DateTimeScheduled.AddMinutes(s1.durationMinutes);
            ScheduleEvents.Add(s1);

            var s2 = new ScheduleEventVM();
            s2.BranchOfficeID = utilBranch.GetBranchByName(Branches, "New York").BranchOfficeID;
            var c2 = utils.GetClientByName(Clients, "Small Company X");
            s2.clientId = c2.ClientID;
            s2.clientName = c2.Name;
            s2.clientAddress = c2.Address;
            s2.title = "Event 3 - Small Company X";
            s2.statusString = Constants.statusCancelled;
            var v2 = EmployeeUtil.EmployeeByName(Employees, "Max");
            s2.EmployeeId = v2.EmployeeID;
            s2.EmployeeName = v2.FullName;
            s2.DateTimeScheduled = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 10, 0, 0);
            s2.durationMinutes = 30;
            s2.duration = s2.durationMinutes.ToString();
            s2.DateTimeScheduledEnd = s2.DateTimeScheduled.AddMinutes(s2.durationMinutes);
            var e2 = utils.GetEquipmentByName(Equipment, "Projector 1");
            s2.equipmentID = e2.id;
            ScheduleEvents.Add(s2);

            var s3 = new ScheduleEventVM();
            s3.BranchOfficeID = utilBranch.GetBranchByName(Branches, "London").BranchOfficeID;
            var c3 = utils.GetClientByName(Clients, "Big Company C");
            s3.clientId = c3.ClientID;
            s3.clientName = c3.Name;
            s3.clientAddress = c3.Address;
            s3.title = "Event 1 - Big Company C";
            s3.statusString = Constants.statusComplete;
            var v3 = EmployeeUtil.EmployeeByName(Employees, "Joanne");
            s3.EmployeeId = v3.EmployeeID;
            s3.EmployeeName = v3.FullName;
            s3.DateTimeScheduled = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 09, 30, 0);
            s3.durationMinutes = 60;
            s3.duration = s3.durationMinutes.ToString();
            s3.DateTimeScheduledEnd = s3.DateTimeScheduled.AddMinutes(s3.durationMinutes);
            var e3 = utils.GetEquipmentByName(Equipment, "iPad test device 1");
            s3.equipmentID = e3.id;
            ScheduleEvents.Add(s3);


        }

        public void initEquipment()
        {
            var utilBranch = new BranchOfficeVM();
            var r1 = new EquipmentVM();
            r1.id = Guid.NewGuid().ToString();
            r1.title = "Projector 1";
            r1.branchId = utilBranch.GetBranchByName(Branches, "New York").BranchOfficeID;
            Equipment.Add(r1);
            var r2 = new EquipmentVM();
            r2.id = Guid.NewGuid().ToString();
            r2.title = "Hololens demo kit";
            r2.branchId = utilBranch.GetBranchByName(Branches, "London").BranchOfficeID;
            Equipment.Add(r2);
            var r3 = new EquipmentVM();
            r3.id = Guid.NewGuid().ToString();
            r3.title = "iPad test device 1";
            r3.branchId = utilBranch.GetBranchByName(Branches, "New York").BranchOfficeID;
            Equipment.Add(r3);
        }

        public void initBranches()
        {
            var b1 = new BranchOfficeVM();
            b1.BranchOfficeID = Guid.NewGuid().ToString();
            b1.Name = "New York";
            Branches.Add(b1);
            var b2 = new BranchOfficeVM();
            b2.BranchOfficeID = Guid.NewGuid().ToString();
            b2.Name = "London";
            Branches.Add(b2);
        }

        public void linkEmployeesToBranches()
        {
            var EmployeeUtil = new EmployeeVM();

            Branches[0].Employees.Add(EmployeeUtil.EmployeeByName(Employees, "Paul"));
            Branches[0].Employees.Add(EmployeeUtil.EmployeeByName(Employees, "Max"));
            Branches[0].Employees.Add(EmployeeUtil.EmployeeByName(Employees, "Rajeet"));
            Branches[1].Employees.Add(EmployeeUtil.EmployeeByName(Employees, "Philippe"));
            Branches[1].Employees.Add(EmployeeUtil.EmployeeByName(Employees, "Samara"));
            Branches[1].Employees.Add(EmployeeUtil.EmployeeByName(Employees, "Joanne"));
            Branches[1].Employees.Add(EmployeeUtil.EmployeeByName(Employees, "Katie"));
        }

        public void initEmployees()
        {
            var v1 = new EmployeeVM();
            v1.EmployeeID = Guid.NewGuid().ToString();
            v1.FirstName = "Paul";
            v1.LastName = "Smith";
            Employees.Add(v1);
            var v2 = new EmployeeVM();
            v2.EmployeeID = Guid.NewGuid().ToString();
            v2.FirstName = "Max";
            v2.LastName = "Brophy";
            Employees.Add(v2);
            var v3 = new EmployeeVM();
            v3.EmployeeID = Guid.NewGuid().ToString();
            v3.FirstName = "Rajeet";
            v3.LastName = "Kumar";
            Employees.Add(v3);
            var v4 = new EmployeeVM();
            v4.EmployeeID = Guid.NewGuid().ToString();
            v4.FirstName = "Philippe";
            v4.LastName = "Dupont";
            Employees.Add(v4);
            var v5 = new EmployeeVM();
            v5.EmployeeID = Guid.NewGuid().ToString();
            v5.FirstName = "Samara";
            v5.LastName = "Johansson";
            Employees.Add(v5);
            var v6 = new EmployeeVM();
            v6.EmployeeID = Guid.NewGuid().ToString();
            v6.FirstName = "Joanne";
            v6.LastName = "Reynolds";
            Employees.Add(v6);
            var v7 = new EmployeeVM();
            v7.EmployeeID = Guid.NewGuid().ToString();
            v7.FirstName = "Katie";
            v7.LastName = "Fritz";
            Employees.Add(v7);
        }

        public void initClients()
        {
            Clients.Add(new ClientVM("Big Company A", "New York"));
            Clients.Add(new ClientVM("Small Company X", "London"));
            Clients.Add(new ClientVM("Big Company B", "London"));
            Clients.Add(new ClientVM("Big Company C", "Mumbai"));
            Clients.Add(new ClientVM("Small Company Y", "Berlin"));
            Clients.Add(new ClientVM("Small Company Z", "Dublin"));
        }

        public void initUnAssignedTasks()
        {
            var uaItem1 = new ScheduleEventVM();
            var cli1 = utils.GetClientByName(Clients, "Big Company A");
            uaItem1.clientId = cli1.ClientID;
            uaItem1.clientName = cli1.Name;
            uaItem1.clientAddress = cli1.Address;
            uaItem1.title = cli1.Name + " - " + cli1.Address;
            uaItem1.durationMinutes = 30;
            uaItem1.duration = uaItem1.durationMinutes.ToString();
            uaItem1.DateTimeScheduled = DateTime.Now.AddDays(14);
            uaItem1.DateTimeScheduledEnd = uaItem1.DateTimeScheduled.AddMinutes(uaItem1.durationMinutes);
            uaItem1.notes = "Test notes 1";

            UnassignedEvents.Add(uaItem1);

            var uaItem2 = new ScheduleEventVM();
            var cli2 = utils.GetClientByName(Clients, "Small Company X");
            uaItem2.clientId = cli2.ClientID;
            uaItem2.clientName = cli2.Name;
            uaItem2.clientAddress = cli2.Address;
            uaItem2.title = cli2.Name + " - " + cli2.Address;
            uaItem2.durationMinutes = 30;
            uaItem2.duration = uaItem1.durationMinutes.ToString();
            uaItem2.DateTimeScheduled = DateTime.Now.AddDays(14);
            uaItem2.DateTimeScheduledEnd = uaItem1.DateTimeScheduled.AddMinutes(uaItem1.durationMinutes);
            uaItem2.notes = "Test notes 2";
            UnassignedEvents.Add(uaItem2);


        }


    }
}