using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BaseClasses;

namespace tinhGiot.Controllers
{
    public class GoogleController : ApiController
    {
        List<Google> gg = new List<Google>();

        private void GetGoogle()
        {
            gg.Add(new Google { GoogleID = "132587469874531452" });
            gg.Add(new Google { GoogleID = "132587469874531453" });
            gg.Add(new Google { GoogleID = "132587469874531454" });
            gg.Add(new Google { GoogleID = "132587469874531455" });
            gg.Add(new Google { GoogleID = "132587469874531456" });
        }

        [HttpPost]
        public string GG(string GGID)
        {
            GetGoogle();
            var list = gg.Find(x => x.GoogleID == GGID);
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
