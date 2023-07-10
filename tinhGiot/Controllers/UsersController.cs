using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BaseClasses;
using System.Net.Mail;

namespace tinhGiot.Controllers
{
    public class UsersController : ApiController
    {
        dbTinhGiotDataContext db = new dbTinhGiotDataContext();
        List<User> users = new List<User>();
        public IEnumerable<User> GetAllUsers()
        {
            GetUsers();
            return users;
        }

        private void GetUsers()
        {
            var user = db.Users.ToList();

            foreach (var item in user)
            {
                users.Add(new User
                {
                    ID = Convert.ToInt32(item.ID),
                    UserName = item.UserName,
                    Password = item.Password,
                    Email = item.Email,
                    Name = item.Name,
                    PhoneNumber = item.PhoneNumber,
                    Image = item.Image,
                    Status = item.Status,
                    FacebookID = item.FacebookID,
                    GoogleID = item.GoogleID,
                });
            }
        }
        public User GetUserLogin(string username, string password)
        {
            if (users.Count() > 0)
            {
                return users.Find(x => x.UserName == username && x.Password == password);
            }
            else
            {
                GetUsers();
                var us = users.Find(x => x.UserName == username && x.Password == password);
                return us;
            }
        }
        public IEnumerable<User> DeleteUsers(int id)
        {
            GetDeleteUser(id);
            return users;
        }
        private void GetDeleteUser(int id)
        {
            User us = db.Users.SingleOrDefault(n => n.ID == id);

            if (us != null)
            {
                db.Users.DeleteOnSubmit(us);
                db.SubmitChanges();
            }
        }

        public User GetUsersbyId(int id)
        {
            if (users.Count() > 0)
            {
                return users.Find(p => p.ID == id);
            }
            else
            {
                GetUsers();
                var abc = users.Find(p => p.ID == id);
                return abc;
            }
        }
        public User GetUsersbyEmail(string email)
        {
            if (users.Count() > 0)
            {
                return users.Find(p => p.Email == email);
            }
            else
            {
                GetUsers();
                var usr = users.Find(p => p.Email == email);
                return usr;
            }
        }

        public User GetUsersbyUsername(string phonenumber,string email)
        {
            if (users.Count() > 0)
            {
                return users.Find(p => p.PhoneNumber == phonenumber || p.Email == email);
            }
            else
            {
                GetUsers();
                var abc = users.Find(p => p.PhoneNumber == phonenumber || p.Email == email);
                return abc;
            }
        }

        public bool CheckExistingUser(User oUser)
        {
            var found = false;
               found = db.Users.Any(p => p.Email == oUser.Email || p.PhoneNumber == oUser.PhoneNumber);
            return  found;
        }
        public void SendNewPassword(User oUser, string password)
        {
            //  var verifyUrl = "/Home/" + emailFor + "/" + activationCode;
            // var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

         //   var fromEmail = new MailAddress("noreply@tinhgiot.com", "Tinh Giot");
            // var fromEmail = new MailAddress("email.tinhgiot@gmail.com", "Tinh Giot");
        //    var toEmail = new MailAddress(oUser.Email);
         //   var fromEmailPassword = "aucfyehzgY0dqmi-wobBvjk4prn@xs";// "tinhgiot123"; // Replace with actual password

            string subject = Resources.ForgotPassword.EmailSubject;
            string body = string.Format(Resources.ForgotPassword.ContentMessage, oUser.Name, password);
            string[] to = { oUser.Email };
            SendingEmail email = new SendingEmail(ConfigFile.EmailNoreply, to, null, subject, body, null);
            email.Send();
        }

        public void SendCreatedPassword(User oUser, string password)
        {
            //  var verifyUrl = "/Home/" + emailFor + "/" + activationCode;
            // var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            //   var fromEmail = new MailAddress("noreply@tinhgiot.com", "Tinh Giot");
            // var fromEmail = new MailAddress("email.tinhgiot@gmail.com", "Tinh Giot");
            //    var toEmail = new MailAddress(oUser.Email);
            //   var fromEmailPassword = "aucfyehzgY0dqmi-wobBvjk4prn@xs";// "tinhgiot123"; // Replace with actual password

            string subject =Resources.Registry.EmailSubject;
            string body = string.Format(Resources.Registry.ContentMessage, oUser.Name, password);
            string[] to = { oUser.Email };
            SendingEmail email = new SendingEmail(ConfigFile.EmailNoreply, to, null, subject, body, null);
            email.Send();
        }

        public bool RegisterNewUser(User oUser)
        {
            try
            {
                db.Users.InsertOnSubmit(oUser);
                db.SubmitChanges();
            }catch(Exception ex)
            {
                return false;
            }

            return true;
        } 
        public void UpdateUser(User oUser)
        {
            var users = db.Users.Where(x => x.ID == oUser.ID);
            foreach(var item in users)
            {
                item.Email = oUser.Email;
                item.PhoneNumber = oUser.PhoneNumber;
                item.Password = oUser.Password;
                item.Name = oUser.Name;
            }
            db.SubmitChanges();
        }

        public User GetUserLoginFB(string fbID)
        {
            if (users.Count() > 0)
            {
                return users.Find(x => x.FacebookID ==  fbID);
            }
            else
            {
                GetUsers();
                var us = users.Find(x => x.FacebookID == fbID);
                return us;
            }
        }

        public User GetUserLoginGoogle(string ggID)
        {
            if (users.Count() > 0)
            {
                return users.Find(x => x.GoogleID == ggID);
            }
            else
            {
                GetUsers();
                var us = users.Find(x => x.GoogleID == ggID);
                return us;
            }
        }

        public User RegistryFacebook(string facebookid, string email, string fullname)
        {
            User us = new User();
            us.UserName = facebookid;
            us.Password = "";
            us.Email = email;
            us.PhoneNumber = "";
            us.Name = fullname;
            us.Status = "1";
            us.FacebookID = facebookid;
            us.Image = "Default.png";
            db.Users.InsertOnSubmit(us);
            db.SubmitChanges();
            return us;
        }

        public User RegistryGoogle(string googleid, string email, string fullname)
        {
            User us = new User();
            us.UserName = googleid;
            us.Password = "";
            us.Email = email;
            us.PhoneNumber = "";
            us.Name = fullname;
            us.Status = "1";
            us.GoogleID = googleid;
            us.Image = "Default.png";
            db.Users.InsertOnSubmit(us);
            db.SubmitChanges();
            return us;
        }
    }
}
