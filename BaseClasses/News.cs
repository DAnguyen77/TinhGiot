using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BaseClasses
{
    public class News : BaseItem
    {
        public News() : base() { }
        public News(string lang) : base(lang) { }
        public string Title { get; set; }
        public string Title_EN { get; set; }
        public string Title_JP { get; set; }
        public string Title_Lang
        {
            get
            {
                string result = this.Title;
                switch (this.SelectedLang)
                {
                    case "EN":
                        result = this.Title_EN;
                        break;
                    case "JP":
                        result = this.Title_JP;
                        break;
                }
                return result;
            }
        }

        public int CategoryID { get; set;}
        public string PostContent { get; set; }
        public string PostContent_EN { get; set; }
        public string PostContent_JP { get; set; }
        public string PostContent_Lang
        {
            get
            {
                string result = this.PostContent;
                switch (this.SelectedLang)
                {
                    case "EN":
                        result = this.PostContent_EN;
                        break;
                    case "JP":
                        result = this.PostContent_JP;
                        break;
                }
                return result;
            }
        }
        public string Author { get; set; }
        public DateTime PostTime { get; set; }
        public string Image { get; set; }
        public string Lang { get; set; }
        public string Keywords { get; set; }
        public string Keywords_EN { get; set; }
        public string Keywords_JP { get; set; }

        public string Keywords_Lang
        {
            get
            {
                string result = this.Keywords;
                switch (this.SelectedLang)
                {
                    case "EN":
                        result = this.Keywords_EN;
                        break;
                    case "JP":
                        result = this.Keywords_JP;
                        break;
                }
                return result;
            }
        }
        public string MoreInfo { get; set; }
     //   public HttpPostedFileWrapper ImageFile { get; set; }

    }
}