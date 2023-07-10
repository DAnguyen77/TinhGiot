using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tinhGiot.BaseClass
{
    public class Contacts
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Note { get; set; }
        public DateTime ContactTime { get; set; }
    }
}