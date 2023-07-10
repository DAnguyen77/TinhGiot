using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BaseClasses
{
    public class About_us:BaseItem
    {
        public About_us() : base() { }
        public About_us(string lang) : base(lang) { }
        public string AboutContent { get; set; }
        public string AboutContent_EN { get; set; }

        public string AboutContent_Lang
        {
            get
            {
                string result = this.AboutContent;
                switch (this.SelectedLang)
                {
                    case "EN":
                        result = this.AboutContent_EN;
                        break;
                    case "JP":
                        break;
                }
                return result;
            }
        }
    }
}