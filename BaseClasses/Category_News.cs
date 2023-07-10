using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BaseClasses
{
    public class Category_News :BaseItem
    {
        public Category_News():base() { }
        public Category_News(string lang) : base(lang) { }
        public string CategoryNews { get; set; }
        public string CategoryNews_EN { get; set; }
        public string CategoryNews_JP { get; set; }
        public string CategoryNews_Lang
        {
            get
            {
                string result = this.CategoryNews;
                switch (this.SelectedLang)
                {
                    case "EN":
                        result = this.CategoryNews_EN;
                        break;
                    case "JP":
                        result = this.CategoryNews_JP;
                        break;
                }
                return result;
            }
        }

    }
}