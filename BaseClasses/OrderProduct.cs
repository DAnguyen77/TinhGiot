using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BaseClasses
{
    public class OrderProduct
    {
        public int ID { get; set; }
        public string OrderID { get; set; }
        public string UserID { get; set; }
        public string ProductID { get; set; }
        public int Quantity { get; set; }
        public string Price { get; set; }
        public string ProductName { get; set; }
        public string ProductName_EN { get; set; }
        public string ProductName_JP { get; set; }
        public DateTime TimeOrder { get; set; }
    }
}