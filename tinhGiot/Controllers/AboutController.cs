using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BaseClasses;

namespace tinhGiot.Controllers
{
    public class AboutController : ApiController
    {
        dbTinhGiotDataContext db = new dbTinhGiotDataContext();
        List<About_us> ab = new List<About_us>();
        public IEnumerable<About_us> GetAllAbout(string lang)
        {
            GetAbout(lang);
            return ab;
        }
        private void GetAbout(string lang)
        {
            var about = db.Abouts.ToList();

            foreach (var item in about)
            {
                ab.Add(new About_us(lang)
                {
                    ID = Convert.ToInt32(item.ID),
                    AboutContent = item.AboutContent,
                    AboutContent_EN = item.AboutContent_EN
                });
            }
        }
        public About_us GetAboutbyId(int id)
        {
            if (ab.Count() > 0)
            {
                return ab.Find(p => p.ID == id);
            }
            else
            {
                GetAbout("");
                var abc = ab.Find(p => p.ID == id);
                return abc;
            }
        }
    }
}
