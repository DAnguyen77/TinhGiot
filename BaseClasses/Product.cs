using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BaseClasses
{
    public class Products:BaseItem
    {
        public Products() { }
        public Products(string lang) : base(lang) { }
        public string ProductName { get; set; }
        public string ProductName_EN { get; set; }
        public string ProductName_JP { get; set; }
        public string ProductName_Lang
        {
            get
            {
                string result = this.ProductName;
                switch (this.SelectedLang)
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
        public string Uses_EN { get; set; }
        public string Uses_JP { get; set; }
        public string Category_Lang { get; set; }
        public string Uses_Lang { 
            get{
                string result = this.Uses;
                switch(this.SelectedLang){
                    case "EN":
                        result = this.Uses_EN;
                        break;
                    case "JP":
                        result = this.Uses_JP;
                        break;
                }
                return result;
            } 
        }

        public string ShortDescription { get; set; }
        public string ShortDescription_EN { get; set; 
        }
        public string ShortDescription_JP { get; set; }
        public string ShortDescription_Lang
        {
            get
            {
                string result = this.ShortDescription;
                switch (this.SelectedLang)
                {
                    case "EN":
                        result = this.ShortDescription_EN;
                        break;
                    case "JP":
                        result = this.ShortDescription_JP;
                        break;
                }
                return result;
            }
        }

        public string MeasureUnit { get; set; }

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

        // public HttpPostedFileWrapper ImageFile { get; set; }
    }
}