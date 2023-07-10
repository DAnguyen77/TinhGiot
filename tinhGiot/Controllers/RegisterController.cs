using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BaseClasses;

namespace tinhGiot.Controllers
{
    public class RegisterController : ApiController
    {
        List<Register> register = new List<Register>();
         public IEnumerable<Register> GetAllRegister()
        {
            GetRegister();
            return register;
        }
        private void GetRegister()
        {
            register.Add(new Register { Name = "Nam Võ", PhoneNumber = 01234567890, Email = "thanhnam@vo.com", Password = "123456"});
            register.Add(new Register { Name = "Nam Thành Võ", PhoneNumber = 090999999, Email = "thanhnam@tn.com", Password = "987654"});

        }

        [HttpPost]
        public string UserRegister (string Name, int Phone, string email, string Password)
        {
            GetRegister();
            var list = register.Find(x => x.PhoneNumber == Phone || x.Email == email);
            if(list == null)
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

