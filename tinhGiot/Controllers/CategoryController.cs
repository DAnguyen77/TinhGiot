using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BaseClasses;

namespace tinhGiot.Controllers
{
    public class CategoryController : ApiController
    {
        dbTinhGiotDataContext db = new dbTinhGiotDataContext();
        List<Category> categories = new List<Category>();
        public IEnumerable<Category> GetAllCategories(string lang)
        {
            GetCategories(lang);
            return categories;
        }
        private void GetCategories(string lang)
        {
            var cate = db.CategoryProducts.ToList();
            categories.Clear();
            foreach (var item in cate)
            {
                categories.Add(new Category(lang)
                {
                    ID = Convert.ToInt32(item.ID),
                    CategoryName = item.CategoryName,
                    CategoryName_EN = item.CategoryName_EN,
                    CategoryName_JP = item.CategoryName_JP
                });
            }
        }

        public string GetCategoryName(int category, string lang)
        {
            Category catPro = null;
            if (categories.Count() > 0)
            {
                catPro = categories.Where(p => p.ID == category).FirstOrDefault();
            }
            else
            {
                GetCategories(lang);
                catPro= categories.Where(p => p.ID == category).FirstOrDefault();

            }
            catPro.setLang(lang);
            return catPro.CategoryName_Lang;
        }

        public IEnumerable<Category> DeleteCateProduct(int id)
        {
            GetDeleteCateProduct(id);
            return categories;
        }
        private void GetDeleteCateProduct(int id)
        {
            CategoryProduct cp = db.CategoryProducts.SingleOrDefault(n => n.ID == id);

            if (cp != null)
            {
                db.CategoryProducts.DeleteOnSubmit(cp);
                db.SubmitChanges();
            }
        }

        public Category GetCateProductsbyId(int id, string lang)
        {
            if (categories.Count() > 0)
            {
                return categories.Find(p => p.ID == id);
            }
            else
            {
                GetCategories(lang);
                var abc = categories.Find(p => p.ID == id);
                return abc;
            }
        }
    }
}
