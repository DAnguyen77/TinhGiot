using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using System.Threading;
using System.Web;

namespace tinhGiot.Controllers
{
    public class BaseController : Controller
    {
        public void setViewLanguage()
        {
            if (RouteData.Values.ContainsKey("lang"))
            {
                string lang = RouteData.Values["lang"].ToString();
                if (Thread.CurrentThread.CurrentCulture.Name.Split('-')[0].ToUpper() != lang)
                {
                    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(lang);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
                    Response.Cookies["SelectedLanguage"].Value = lang;
                    Response.Cookies["SelectedLanguage"].Expires.AddYears(1);
                }
            }
            ViewBag.Language = Thread.CurrentThread.CurrentCulture.Name.Split('-')[0].ToUpper();
        }

        public string getLanguage()
        {
            return Thread.CurrentThread.CurrentCulture.Name.Split('-')[0].ToUpper();
        }

        public ActionResult ChangeCulture(string lang, string returnUrl)
        {
            Session["Culture"] = new CultureInfo(lang);
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

    }
}