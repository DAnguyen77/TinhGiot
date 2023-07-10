using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace BaseClasses
{
   
    public  class CartItems:Products
    {   
        public CartItems(string lang) : base(lang) { }
        public string BuyerID { get; set; }
        public int ProductID { get; set; }
        public DateTime TimeStamp { get; set; }
    }


}