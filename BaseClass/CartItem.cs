using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;


namespace tinhGiot.BaseClass
{
    [Serializable]
   
    public class CartItem
    {   
        public Products Products { set; get; }
        public int Quantity { set; get; }      
    }


}