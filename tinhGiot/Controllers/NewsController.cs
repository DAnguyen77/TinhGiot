using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BaseClasses;

namespace tinhGiot.Controllers
{
    public class NewsController : ApiController
    {
        dbTinhGiotDataContext db = new dbTinhGiotDataContext();
        List<News> news = new List<News>();
        public IEnumerable<News> GetAllNews(string lang)
        {
            GetNews(lang);
            return news;
        }

        public IEnumerable<News> SearchNews(string lang, string Keywords)
        {
            List<New> news_list = db.News.Where(x => (x.Keywords.Contains(Keywords) | x.Keywords_EN.Contains(Keywords) | x.Keywords_JP.Contains(Keywords))).ToList();
            news.Clear();
            foreach (var item in news_list)
            {
                news.Add(new News(lang)
                {
                    ID = Convert.ToInt32(item.ID),
                    Title = item.Title,
                    Title_EN = item.Title_EN,
                    Title_JP = item.Title_JP,
                    Author = item.Author,
                    PostContent = item.PostContent,
                    PostContent_EN = item.PostContent_EN,
                    PostContent_JP = item.PostContent_JP,
                    Keywords = item.Keywords,
                    Keywords_EN = item.Keywords_EN,
                    Keywords_JP = item.Keywords_JP,
                    Image = item.Image,
                    CategoryID = Convert.ToInt32(item.CategoryID),
                    Lang = item.Lang,
                    PostTime = Convert.ToDateTime(item.PostTime),
                    MoreInfo = item.MoreInfo,
                });
            }
            return news;
        }

        private void GetNews(string lang)
        {
            var newss = db.News.ToList();
            foreach (var item in newss)
            {
                news.Add(new News(lang)
                {
                    ID = Convert.ToInt32(item.ID),
                    Title = item.Title,
                    Title_EN = item.Title_EN,
                    Title_JP = item.Title_JP,
                    Author = item.Author,
                    PostContent = item.PostContent,
                    PostContent_EN = item.PostContent_EN,
                    PostContent_JP = item.PostContent_JP,
                    Keywords = item.Keywords,
                    Keywords_EN = item.Keywords_EN,
                    Keywords_JP = item.Keywords_JP,
                    Image = item.Image,
                    CategoryID = Convert.ToInt32(item.CategoryID),
                    Lang = item.Lang,
                    PostTime = Convert.ToDateTime(item.PostTime),
                    MoreInfo = item.MoreInfo,
                });
            }
        }

        public List<News> NewsbyCategory(int category, string lang)
        {
            if (news.Count() > 0)
            {
                return news.Where(p => p.CategoryID == category).ToList();
            }
            else
            {
                GetNews(lang);
                var list = news.Where(p => p.CategoryID == category).ToList();
                return list;
            }
        }

        public IEnumerable<News> NewsbyCategoryLQ(int id,int category, string lang)
        {
            if (news.Count() > 0)
            {
                return news.Where(p => p.CategoryID == category && p.ID != id );
            }
            else
            {
                GetNews(lang);
                var list = news.Where(p => p.CategoryID == category && p.ID != id );
                return list;
            }
        }

        public News GetNewsbyId(int id, string lang)
        {
            if (news.Count() > 0)
            {
                return news.Find(p => p.ID == id);
            }
            else
            {
                GetNews(lang);
                return news.Find(p => p.ID == id);
            }
        }

        public List<News> NewNews(string newtt, string lang)
        {
            if (news.Count() > 0)
            {
                return news.Where(p => p.MoreInfo == newtt).ToList();
            }
            else
            {
                GetNews(lang);
                var list = news.Where(p => p.MoreInfo == newtt).ToList();
                return list;
            }
        }
        public IEnumerable<News> DeleteNews(int id)
        {
            GetDeleteNews(id);
            return news;
        }
        private void GetDeleteNews(int id)
        {
            New us = db.News.SingleOrDefault(n => n.ID == id);

            if (us != null)
            {
                db.News.DeleteOnSubmit(us);
                db.SubmitChanges();
            }
        }
    }
}
