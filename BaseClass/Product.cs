using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tinhGiot.BaseClass
{
    public class Products:BaseClass
    {
        public Products(string lang) : base(lang) { }
        public string ProductName { get; set; }
        public string ProductName_EN { get; set; }
        public string ProductName_JP { get; set; }
        public string ProductName_Lang
        {
            get
            {
                string result = this.ProductName;
                switch (this.ProductName)
                {
                    case "EN":
                        result = this.ProductName_EN;
                        break;
                    case "JP":
                        result = this.ProductName_JP;
                        break;
                }
                return result;
            }
        }
        public int CategoryID { get; set; }
        public string ProductCategory { get; set; }
        public int Quantity { get; set; }
        public string Price { get; set; }
        public string UnitPrice { get; set; }
        public string SKU { get; set; }
        public string Image { get; set; }
        public string Uses { get; set; }
        public int Capacity { get; set; }
        public string Description { get; set; }
        public string Description_EN { get; set; }
        public string Description_JP { get; set; }
        public string Description_Lang
        {
            get
            {
                string result = this.Description;
                switch (this.SelectedLang)
                {
                    case "EN":
                        result = this.Description_EN;
                        break;
                    case "JP":
                        result = this.Description_JP;
                        break;
                }
                return result;
            }
        }
        public string Status { get; set; }
        public string MoreInfo { get; set; }
        public string MoreInfo_EN { get; set; }
        public string MoreInfo_JP { get; set; }
        public string Category_Lang { get; set; }
        public string MoreInfo_Lang { 
            get{
                string result = this.MoreInfo;
                switch(this.SelectedLang){
                    case "EN":
                        result = this.MoreInfo_EN;
                        break;
                    case "JP":
                        result = this.MoreInfo_JP;
                        break;
                }
                return result;
            } 
        }
        public HttpPostedFileWrapper ImageFile { get; set; }
    }
}