using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BaseClasses
{
    public class BaseItem
    {
       string lang = "VI";
        public BaseItem() { }
        public BaseItem(string lang)
        {
            this.lang = lang;
        }
        public void setLang(string lang) {
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