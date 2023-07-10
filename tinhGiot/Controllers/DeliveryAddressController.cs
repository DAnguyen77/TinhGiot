using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BaseClasses;

namespace tinhGiot.Controllers
{
    public class DeliveryAddressController : Controller
    {
        dbTinhGiotDataContext db = new dbTinhGiotDataContext();
        // GET: DeliveryAddress
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DeliveryAddressList()
        {
            var list = db.DeliveryAddresses.Where(x => x.UserID == Session["UserID"].ToString());
            return View();
        }

    }
}