using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BaseClasses;

namespace tinhGiot.Controllers
{
    public class ProfilesController : ApiController
    {
        List<Profiles> profile = new List<Profiles>();
        private void UProfile()
        {
            profile.Add(new Profiles { Phone = "01234567890", Address = "123456 TTT" });
            profile.Add(new Profiles { Phone = "0800000000", Address = "654321 CCC" });
            profile.Add(new Profiles { Phone = "0900000000", Address = "987123N NN" });
        }

        public string Updateprofile(string Phone, string Address)
        {
            var P = Phone;
            var A = Address;
            if (P != null && A != null)
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
