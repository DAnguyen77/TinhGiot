using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using tinhGiot.Models;

namespace tinhGiot.Controllers
{
    public class FacebookController : ApiController
    {
        List<Facebook> fb = new List<Facebook>();
        private void GetFacebook()
        {
            fb.Add(new Facebook { FacebookID = "132587469874531452" });
            fb.Add(new Facebook { FacebookID = "943781645319764581" });
            fb.Add(new Facebook { FacebookID = "291873451248796312" });
            fb.Add(new Facebook { FacebookID = "132587469874531454" });
            fb.Add(new Facebook { FacebookID = "132587469874531458" });
            fb.Add(new Facebook { FacebookID = "132587469874531457" });
        }

        [HttpPost]
        public string FB(string FbID)
        {
            GetFacebook();
            var list = fb.Find(x => x.FacebookID == FbID);
            if (list != null)
            {
                return "1";
            }
            else
            {
                return "0";
            }
        }
    }
}
