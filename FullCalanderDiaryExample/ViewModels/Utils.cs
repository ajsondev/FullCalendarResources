using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

/// <summary>
/// Generic utils for the example diary solution
/// </summary>
namespace FullCal.ViewModels
{
    // declare come constants that represent diary event status
    public static class Constants
    {
        // http://www.w3schools.com/colors/colors_names.asp
        public static string statusBooked = "#00BFFF";
        public static string statusInProcess = "#90EE90";
        public static string statusCancelled = "#FF0000";
        public static string statusComplete = "#7CFC00";
    }


    public static class utils
    {
        // name of the file that the TestHarness data will be serialized into/loaded from
        public const string TestRepository = "TestRepository.xml";

        // test for and create temp folder if neccessary
        public static string GetTestFileLocation()
        {
            string tempFolder = Path.GetTempPath() + "ATestApp\\";
            if (!Directory.Exists(tempFolder))
                Directory.CreateDirectory(tempFolder);
            return string.Format("{0}{1}{2}", Path.GetTempPath(), "ATestApp\\", TestRepository);
        }


    // some query methods - only used for testing
    public static ClientVM GetClientByID(List<ClientVM> Clients, string ClientID)
    {
        return Clients.Where(s => s.ClientID == ClientID).FirstOrDefault();
    }

    public static ClientVM GetClientByName(List<ClientVM> Clients, string ClientSurname)
    {
        return Clients.Where(s => s.Name.Contains(ClientSurname)).FirstOrDefault();
    }

    public static EquipmentVM GetEquipmentByName(List<EquipmentVM> EquipmentList, string Name)
    {
        return EquipmentList.Where(s => s.title.Contains(Name)).FirstOrDefault();
    }

    // generic save method
     public static bool Save(string FileName, Object Obj)
    {
        var xs = new XmlSerializer(Obj.GetType());

        using (TextWriter sw = new StreamWriter(FileName))
        {
            xs.Serialize(sw, Obj);
        }
        if (File.Exists(FileName))
            return true;
        else return false;
    }

    // deletes a node from the test harness
    public static bool delete(string FileName, Object Obj)
    {
        System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
        doc.Load(FileName);
        System.Xml.XmlElement el = (System.Xml.XmlElement)doc.SelectSingleNode("/TestHarness/ScheduleEvents/ScheduleEventVM[EventID='" + Obj.ToString() + "']");
        if (el != null) { el.ParentNode.RemoveChild(el); }
        doc.Save(FileName);
        return true;
       
    }


     // generic save method
        public static T Load<T>(string FileName)
    {
        Object rslt;
            try
            {
                if (File.Exists(FileName))
                {
                    var xs = new XmlSerializer(typeof(T));

                    using (var sr = new StreamReader(FileName))
                    {
                        rslt = (T)xs.Deserialize(sr);
                    }
                    return (T)rslt;
                }
                else
                {
                    return default(T);
                }
            }
            catch (Exception ex)
            {
                string errorMsg = ex.Message;
                return default(T);
            }
        
    }



    }



}