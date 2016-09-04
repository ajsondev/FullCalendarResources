using FullCal.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System;

namespace FullCal.ViewModels
{
    // basic class representing some clients of an organisation
    // it is expected you would replace this with a database table for your own requirements

    public class ClientVM
    {
        public string ClientID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public ClientVM()
        {
            ClientID = Guid.NewGuid().ToString();
        }

        public ClientVM(string name, string address)
        {
            ClientID = Guid.NewGuid().ToString();
            Name = name;
            Address = address;
        }

    }
}