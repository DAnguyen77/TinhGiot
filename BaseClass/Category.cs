using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tinhGiot.Models
{
    public class Category:BaseItem
    {
        public Category(string lang) : base(lang) { }

        public string CategoryName { get; set; }
        public string CategoryName_EN { get; set; }
        public string CategoryName_JP { get; set; }

        public string CategoryName_Lang
        {
            get
            {
                string result = this.CategoryName;
                switch (this.SelectedLang)
                {
                    case "EN":
                        result = this.CategoryName_EN;
                        break;
                    case "JP":
                        result = this.CategoryName_JP;
                        break;
                }
                return result;
            }
        }
        public string Type { get; set; }
    }
}