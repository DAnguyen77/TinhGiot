using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BaseClasses;

namespace tinhGiot.Controllers
{
    public class Category_NewsController : ApiController
    {
        dbTinhGiotDataContext db = new dbTinhGiotDataContext();
        List<Category_News> category_news = new List<Category_News>();
        public IEnumerable<Category_News> GetAllCategory_News(string lang)
        {
            GetCategory_News(lang);
            return category_news;
        }
        private void GetCategory_News(string lang)
        {
            var cate = db.CategoryNews.ToList();
            category_news.Clear();
            foreach (var item in cate)
            {
                category_news.Add(new Category_News(lang)
                {
                    ID = Convert.ToInt32(item.ID),
                    CategoryNews = item.CategoryNews,
                    CategoryNews_EN = item.CategoryNews_EN,
                    CategoryNews_JP = item.CategoryNews_JP
                });
            }
        }

        public Category_News GetCategoryName(int category, string lang)
        {
            if (category_news.Count() > 0)
            {
                return category_news.Where(p => p.ID == category).FirstOrDefault();
            }
            else
            {
                GetCategory_News(lang);
                 return category_news.Where(p => p.ID == category).FirstOrDefault();
            }
        }

        public IEnumerable<Category_News> DeleteCateNews(int id)
        {
            GetDeleteCateNews(id);
            return category_news;
        }
        private void GetDeleteCateNews(int id)
        {
            CategoryNew cp = db.CategoryNews.SingleOrDefault(n => n.ID == id);

            if (cp != null)
            {
                db.CategoryNews.DeleteOnSubmit(cp);
                db.SubmitChanges();
            }
        }

        public Category_News GetCateNewsbyId(int id, string lang)
        {
            if (category_news.Count() > 0)
            {
                return category_news.Find(p => p.ID == id);
            }
            else
            {
                GetCategory_News(lang);
                var abc = category_news.Find(p => p.ID == id);
                return abc;
            }
        }
    }
}
