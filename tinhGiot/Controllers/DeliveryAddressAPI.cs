using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BaseClasses;
using System.Globalization;
using Newtonsoft.Json;

namespace tinhGiot.Controllers
{
    public class Country
    {
        public string Name
        {
            get;set;
        }

        public string Code
        {
            get;set;
        }
    }

    public class IPDataIPAPI
    {
        public string status { get; set; }
        public string country { get; set; }
        public string countryCode { get; set; }
        public string region { get; set; }
        public string regionName { get; set; }
        public string city { get; set; }
        public string zip { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
        public string timezone { get; set; }
        public string isp { get; set; }
        public string org { get; set; }
        public string @as { get; set; }
        public string query { get; set; }
    }
    public class DeliveryAddressAPI : ApiController
    {
        dbTinhGiotDataContext db = new dbTinhGiotDataContext();
        public string getClientIP()
        {
            string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return ip;
        }

       public string getCountry(string ip)
        {
            try
            {
                IPDataIPAPI ipInfo = new IPDataIPAPI();
                string strResponse = new WebClient().DownloadString("http://ip-api.com/json/" + ip);
                if (strResponse == null || strResponse == "") return "";
                ipInfo = JsonConvert.DeserializeObject<IPDataIPAPI>(strResponse);
                if (ipInfo == null || ipInfo.status.ToLower().Trim() == "fail") return "";
                else return  ipInfo.country;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public List<string> GetCountries()
        {
            List<string> list = new List<string>();
            CultureInfo[] CInfoList = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            foreach (CultureInfo CInfo in CInfoList)
            {
                RegionInfo R = new RegionInfo(CInfo.LCID);
                Country country = new Country();
                if (!list.Contains(R.EnglishName) && R.EnglishName != "World")
                {
                    list.Add(R.EnglishName);
                }
            }
            
            list.Sort();
            return list;
        }

        public void AddNew(DeliveryAddress oDeliveryAddress)
        {
            db.DeliveryAddresses.InsertOnSubmit(oDeliveryAddress);
            db.SubmitChanges();
        }

        public void Update(DeliveryAddress oDeliveryAddress)
        {
            var oitems = db.DeliveryAddresses.Where(x => (x.ID == oDeliveryAddress.ID));
            if ((oDeliveryAddress.IsDefault) & (!oitems.FirstOrDefault().IsDefault))
            {
                // update previous address with default;
                var default_del_addr = db.DeliveryAddresses.Where(x => x.IsDefault == true);
                foreach (var item in default_del_addr)
                {
                    item.IsDefault = false;
                }
                db.SubmitChanges();
            }
            foreach (var item in oitems)
            {
                item.Fullname = oDeliveryAddress.Fullname;
                item.Address = oDeliveryAddress.Address;
                item.City = oDeliveryAddress.City;
                item.Country = oDeliveryAddress.Country;
                item.District = oDeliveryAddress.District;
                item.IsDefault = oDeliveryAddress.IsDefault;
                item.PhoneNumber = oDeliveryAddress.PhoneNumber;
                item.State = oDeliveryAddress.State;
                item.Ward = oDeliveryAddress.Ward;
                item.Postcode = oDeliveryAddress.Postcode;
            }
            db.SubmitChanges();
        }

        public DeliveryAddress getDefaultDeliveryAddress(string UserID)
        {
           return db.DeliveryAddresses.Where(x => (x.UserID == UserID )& (x.IsDefault == true)).FirstOrDefault();    
        }

        public void Delete(string ID)
        {
           var obj = db.DeliveryAddresses.Where(x => x.ID == long.Parse(ID));
            db.DeliveryAddresses.DeleteAllOnSubmit(obj);
            db.SubmitChanges();
        }

        public List<BaseClasses.DeliveryAddress> GetDeliveryAddressList(string UserID)
        {
            var list = db.DeliveryAddresses.Where(x => x.UserID == UserID);

            return list.ToList();
        }

        public BaseClasses.DeliveryAddress getDeliveryAddress(string ID)
        {

            return db.DeliveryAddresses.Where(x => x.ID == long.Parse( ID)).First();
        }
    }
}