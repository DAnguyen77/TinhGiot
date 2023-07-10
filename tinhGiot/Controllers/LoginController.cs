using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BaseClasses;

namespace tinhGiot.Controllers
{
    public class LoginController : ApiController
    {
        List<Login> login = new List<Login>();
        public IEnumerable<Login> GetAllLogin()
        {
            GetLogin();
            return login;
        }
        private void GetLogin()
        {
            login.Add(new Login { Username = "thanhnam@123.com", Password = "123456" });
            login.Add(new Login { Username = "thanhnam@namvo.com", Password = "654321" });
            login.Add(new Login { Username = "thanhnam@vo.com", Password = "987123" });
        }

        [HttpPost]
        public string UserLogin(string UserName, string PassWord)
        {   
            GetLogin();
            var list = login.Find(x => x.Username == UserName && x.Password == PassWord);
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
