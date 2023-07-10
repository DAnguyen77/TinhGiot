using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tinhGiot.BaseClass
{
    public class Order
    {
        public int ID { get; set; }
        public string OrderID { get; set; }
        public string UserID { get; set; }
        public string StatusPayment { get; set; }
        public string Payment { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string City1 { get; set; }
        public string Address1 { get; set; }
        public string City2 { get; set; }
        public string Address2 { get; set; }
        public string Note { get; set; }
        public DateTime TimeOrder { get; set; }
        public string Country { get; set; }
        public string DeliveryStatus { get; set; }
        public string TotalPrice { get; set; }

    }
}