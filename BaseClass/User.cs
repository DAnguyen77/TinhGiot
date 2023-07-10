using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tinhGiot.BaseClass
{
    public class Users
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Name { get; set;}
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Status { get; set; }
        public string Image { get; set; }
        public string FacebookID { get; set; }
        public string GoogleID { get; set; }
        public HttpPostedFileWrapper ImageFile { get; set; }
    }
}