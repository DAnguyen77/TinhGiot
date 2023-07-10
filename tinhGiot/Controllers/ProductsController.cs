using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BaseClasses;

namespace tinhGiot.Controllers
{
    public class ProductsController : ApiController
    {
        dbTinhGiotDataContext db = new dbTinhGiotDataContext();
        List<Products> products = new List<Products>();
        public IEnumerable<Products> GetAllProducts(string lang)
        {
            GetProducts(lang);
            return products;
        }

        private void GetProducts(string lang)
        {
            var pd = db.Products.ToList();

            foreach (var item in pd)
            {
                products.Add(new Products(lang)
                {
                    ID = Convert.ToInt32(item.ID),
                    ProductName = item.ProductName,
                    ProductName_EN = item.ProductName_EN,
                    ProductName_JP = item.ProductName_JP,
                    Price = item.Price,
                    Image = item.Image,
                    Capacity = Convert.ToInt32(item.Capacity),
                    Description = item.Description,
                    Description_EN = item.Description_EN,
                    Description_JP = item.Description_JP,
                    ShortDescription = item.ShortDescription,
                    ShortDescription_EN = item.ShortDescription_EN,
                    ShortDescription_JP = item.ShortDescription_JP,
                    Keywords = item.Keywords,
                    Keywords_EN = item.Keywords_EN,
                    Keywords_JP = item.Keywords_JP,
                    CategoryID = Convert.ToInt32(item.CategoryID),
                    Status = item.Status,
                    Quantity = Convert.ToInt32(item.Quantity),
                    SKU = item.SKU,
                    MoreInfo = item.MoreInfo,
                    MeasureUnit = item.MeasureUnit,
                    Uses = item.Uses,
                    Uses_EN = item.Uses_EN,
                    Uses_JP = item.Uses_JP,
                    UnitPrice = item.UnitPrice,
                }); 
            }

            
        }

        public IEnumerable<Products> SearchProducts(string lang, string keywords)
        {
           // db.ExecuteQuery<Products>()
            List<Product> pd = db.Products.Where(p =>( p.Keywords.Contains(keywords) | p.Keywords_EN.Contains(keywords) | p.Keywords_JP.Contains(keywords))).ToList();
            products.Clear();
            foreach (var item in pd)
            {
                products.Add(new Products(lang)
                {
                    ID = Convert.ToInt32(item.ID),
                    ProductName = item.ProductName,
                    ProductName_EN = item.ProductName_EN,
                    ProductName_JP = item.ProductName_JP,
                    Price = item.Price,
                    Image = item.Image,
                    Capacity = Convert.ToInt32(item.Capacity),
                    Description = item.Description,
                    Description_EN = item.Description_EN,
                    Description_JP = item.Description_JP,
                    ShortDescription = item.ShortDescription,
                    ShortDescription_EN = item.ShortDescription_EN,
                    ShortDescription_JP = item.ShortDescription_JP,
                    Keywords = item.Keywords,
                    Keywords_EN = item.Keywords_EN,
                    Keywords_JP = item.Keywords_JP,
                    CategoryID = Convert.ToInt32(item.CategoryID),
                    Status = item.Status,
                    Quantity = Convert.ToInt32(item.Quantity),
                    SKU = item.SKU,
                    MoreInfo = item.MoreInfo,
                    MeasureUnit = item.MeasureUnit,
                    Uses = item.Uses,
                    Uses_EN = item.Uses_EN,
                    Uses_JP = item.Uses_JP,
                    UnitPrice = item.UnitPrice,
                });
            }
            return products;
        }
        public IEnumerable<Products> SearchProductByKeywords(string lang, string keywords)
        {  
            List<Product> pd = db.ExecuteQuery<Product>(string.Format("EXEC [dbo].[SearchproductByKeyWords] @keywords='{0}' ", keywords)).ToList();
            products.Clear();
            foreach (var item in pd)
            {
                products.Add(new Products(lang)
                {
                    ID = Convert.ToInt32(item.ID),
                    ProductName = item.ProductName,
                    ProductName_EN = item.ProductName_EN,
                    ProductName_JP = item.ProductName_JP,
                    Price = item.Price,
                    Image = item.Image,
                    Capacity = Convert.ToInt32(item.Capacity),
                    Description = item.Description,
                    Description_EN = item.Description_EN,
                    Description_JP = item.Description_JP,
                    ShortDescription = item.ShortDescription,
                    ShortDescription_EN = item.ShortDescription_EN,
                    ShortDescription_JP = item.ShortDescription_JP,
                    Keywords = item.Keywords,
                    Keywords_EN = item.Keywords_EN,
                    Keywords_JP = item.Keywords_JP,
                    CategoryID = Convert.ToInt32(item.CategoryID),
                    Status = item.Status,
                    Quantity = Convert.ToInt32(item.Quantity),
                    SKU = item.SKU,
                    MoreInfo = item.MoreInfo,
                    MeasureUnit = item.MeasureUnit,
                    Uses = item.Uses,
                    Uses_EN = item.Uses_EN,
                    Uses_JP = item.Uses_JP,
                    UnitPrice = item.UnitPrice,
                });
            }
            return products;
        }

        public IEnumerable<Products> GetProducts(int category, string lang)
        {
            if (products.Count() > 0)
            {
                return products.Where(p => p.CategoryID == category);
            }
            else
            {
                GetProducts(lang);
                var list = products.Where(p => p.CategoryID == category);
                return list;
            }
        }

        public IEnumerable<Products> GetProductsLQ(int id, int category, string lang)
        {
            if (products.Count() > 0)
            {
                return products.Where(p => p.CategoryID == category && p.ID != id);
            }
            else
            {
                GetProducts(lang);
                var list = products.Where(p => p.CategoryID == category && p.ID != id);
                return list;
            }
        }

        public Products GetProductsbyId(int id, string lang)
        {
            if (products.Count() > 0)
            {
                return products.Find(p => p.ID == id);
            }
            else
            {
                GetProducts(lang);
                var abc = products.Find(p => p.ID == id);
                return abc;
            }
        }
        public IEnumerable<Products> MoreProduct(string moreinfo, string lang)
        {
            if (products.Count() > 0)
            {
                return products.Where(p => p.MoreInfo == moreinfo);
            }
            else
            {
                GetProducts(lang);
                var list = products.Where(p => p.MoreInfo == moreinfo);
                return list;
            }
        }
        public IEnumerable<Products> DeleteProduct(int id)
        {
            GetDeleteProduct(id);
            return products;
        }


        private void GetDeleteProduct(int id)
        {
            Product us = db.Products.SingleOrDefault(n => n.ID == id);

            if (us != null)
            {
                db.Products.DeleteOnSubmit(us);
                db.SubmitChanges();
            }
        }
    }
}
