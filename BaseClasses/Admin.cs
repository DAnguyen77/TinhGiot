using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BaseClasses
{
    public class AdminTG
    {
        public int ID { get; set; }
        public string AdminName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Type { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
     //  public HttpPostedFileWrapper ImageFile { get; set; }
    }
}