using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tinhGiot.BaseClass
{
    public class BaseItem
    {
        string lang = "VI";
        public BaseItem(string lang)
        {
            this.lang = lang;
        }
        public int ID { get; set; }

        public string SelectedLang { 
            get{
                return this.lang;
            }
        }
    }
}