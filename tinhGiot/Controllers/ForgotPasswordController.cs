using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BaseClasses;

namespace tinhGiot.Controllers
{
    public class ForgotPasswordController : ApiController
    {
        List<ForgotPassword> forgotpassword = new List<ForgotPassword>();
        public IEnumerable<ForgotPassword> GetAllForgotpassword()
        {
            GetForgotPassword();
            return forgotpassword;
        }
        private void GetForgotPassword()
        {
            forgotpassword.Add(new ForgotPassword { Email = "thanhnam@123.com" });
        }

        [HttpPost]
        public string ForgotPasswd (string email)
        {
            GetForgotPassword();
            var list = forgotpassword.Find(x => x.Email == email);
            if(list != null)
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
