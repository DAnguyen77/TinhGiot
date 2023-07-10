using BaseClasses;
using CaptchaMvc.HtmlHelpers;
using Facebook;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Web.Mvc;
using System.Web.Security;

namespace tinhGiot.Controllers
{
    public class HomeController : BaseController
    {
        dbTinhGiotDataContext db = new dbTinhGiotDataContext();
        public string SetLanguage(string lang, string url)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(lang);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
            Response.Cookies["SelectedLanguage"].Value = lang;
            Response.Cookies["SelectedLanguage"].Expires.AddYears(1);
            string[] url_arr = url.TrimStart().Split('/');
            if (url_arr.Length > 4)
            {
                string[] ids = url_arr[4].Split('-');
                switch (url_arr[2])
                {
                    case "ProductDetail":
                        ProductsController apiProduct = new ProductsController();
                        var pro = apiProduct.GetProductsbyId(int.Parse(ids[0]), lang);
                        url = string.Format("/{0}/{1}/{2}/{3}", url_arr[1], url_arr[2], pro.ProductName_Lang.Replace(' ', '-'), url_arr[4]);
                        break;
                    case "ProductList":
                        CategoryController apiCate = new CategoryController();
                        url = string.Format("/{0}/{1}/{2}/{3}", url_arr[1], url_arr[2], (apiCate.GetCategoryName(int.Parse(ids[0]), lang)).Replace(' ', '-'), url_arr[4]);
                        break;
                    case "Arcticles":
                        Category_NewsController apiGetCateName = new Category_NewsController();
                        url = string.Format("/{0}/{1}/{2}/{3}", url_arr[1], url_arr[2], (apiGetCateName.GetCategoryName(int.Parse(ids[0]), lang)).CategoryNews_Lang.Replace(' ', '-'), url_arr[4]);
                        break;
                    case "Browse":
                        NewsController apiNews = new NewsController();
                       BaseClasses.News news = apiNews.GetNewsbyId(int.Parse(ids[0]),lang);
                        url = string.Format("/{0}/{1}/{2}/{3}", url_arr[1], url_arr[2], news.Title_Lang.Replace(' ', '-'), url_arr[4]);
                        break;
                }
            }
            if (RouteData.Values.ContainsKey("lang"))
            {
                string old_lang = RouteData.Values["lang"].ToString();
                RouteData.Values["lang"] = lang;
                if (url == "/")
                    url =  string.Format("/{0}", lang);
                else
                    url = url.Replace(string.Format("/{0}", old_lang), string.Format("/{0}", lang));
            }
      
            return url;
        }

        public ActionResult Index()
        {
            
            int value = RouteData.Values.Count;
            setViewLanguage();
            ProductsController apiProduct = new ProductsController();
            var listhot = apiProduct.MoreProduct("1", ViewBag.Language);
            ViewBag.listhot = listhot;
            var listnew = apiProduct.MoreProduct("0", ViewBag.Language);
            ViewBag.listnew = listnew;
            NewsController apiNews = new NewsController();
            List<News> listnewtt = apiNews.NewNews("1", ViewBag.Language);
            if (listnewtt.Count > 0)
                ViewBag.listnewtt = listnewtt;
            return View();
        }

        public ActionResult About()
        {
            setViewLanguage();
            ViewBag.Message = "Your application description page.";

            return View();
        }
       
        public ActionResult Contact()
        {
            setViewLanguage();

            return View();
        }

        [HttpPost]
        public ActionResult Contact(string name, string email, string phonenumber, string note, string CaptchaInputText)
        {
            setViewLanguage();
            if (this.IsCaptchaValid(CaptchaInputText))
            {
                Contact prd = new Contact();
                prd.Name = name;
                prd.Email = email;
                prd.PhoneNumber = phonenumber;
                prd.Note = note;
                prd.ContactTime = DateTime.Now;
                db.Contacts.InsertOnSubmit(prd);
                db.SubmitChanges();
                ViewBag.check = 1; 
                return View();
            }
            else
            {
                ViewBag.check = 0;
                ViewBag.ErrMessage = Resources.Contact.ErrorMessage;
            }
            
            return View();
        }

        public ActionResult Registry()
        {
            setViewLanguage();
            return View();
        }

        [HttpPost]
        public ActionResult Registry( string password, string email, string phonenumber, string name)
        {
            setViewLanguage();
            UsersController uc = new UsersController();
            var checkusername = uc.GetUsersbyUsername(email,email);
            if(checkusername == null)
            {
                    User us = new User();
                    us.UserName = email;
                    us.Password = password;
                    us.Email = email;
                    us.PhoneNumber = phonenumber;
                    us.Name = name;
                    us.Status = "1";
                    us.Image = "Default.png";
                    db.Users.InsertOnSubmit(us);
                    db.SubmitChanges();
                    return new RedirectResult("Login");
            }
            else
            {
                ViewBag.tb = "Email hoặc số điện thoại đã được đăng ký.";
                return View();
            }
        }

        public ActionResult CreatNewMember(User oUser)
        {
            setViewLanguage();
            UsersController uc = new UsersController();
            Cryptography crypto = new Cryptography();
            oUser.Password = crypto.Encrypt(oUser.Password);
            string value = "0";
            if (!uc.CheckExistingUser(oUser))
            {
                oUser.Image = "Default.png";
                if (uc.RegisterNewUser(oUser))
                {
                    Login(oUser.UserName, oUser.Password);
                    value = "1";
                }
            }
            Session["NewMember"] =  true;
            return Content(value);
        }

        public ActionResult Login()
        {
            setViewLanguage();
            return View();
        }
        
        [HttpPost]
        public string Login(string username, string password)
        {
            setViewLanguage();

            UsersController us = new UsersController();
            Cryptography crypto = new Cryptography();
            var user = us.GetUserLogin(username, crypto.Encrypt(password));
            string result =
            result = "<p style='color: red;'><b>" + Resources.Login.Login_message + "<b/></p>";
            if (user != null)
            {
                Session["userid"] = user.ID;
                Session["username"] = user.UserName;
                Session["email"] = user.Email;
                Session["name"] = user.Name;
                Session["image"] = user.Image;
                Session["phonenumber"] = user.PhoneNumber;
                Session["status"] = user.Status;
                Session["image"] = user.Image;
                result = "1";
                Response.Cookies["BuyerID"].Value = user.ID.ToString();
                Response.Cookies["BuyerID"].Expires = DateTime.Now.AddDays(14);
            }
            return result;
        }

        //==== Logout ====//
        public ActionResult Logout()
        {
            setViewLanguage();
            Session["userid"] = null;
            Session["username"] = null;
            Session["email"] = null;
            Session["name"] = null;
            Session["image"] = null;
            Session["address"] = null;
            Session["phonenumber"] = null;
            Session["status"] = null;
            Session["image"] = null;
            Session["totalprice"] = null;
            return new RedirectResult("Index");
        }

        public ActionResult ForgotPassword()
        {
            setViewLanguage();
            return View();
        }

        public ActionResult Products(int id)
        {
            setViewLanguage();
            string lang = ViewBag.Language;
            ProductsController apiProduct = new ProductsController();
            CategoryController apiCate = new CategoryController();
            var gnc = apiCate.GetCategoryName(id, lang);
            var categories = apiCate.GetAllCategories(lang);
            var list = apiProduct.GetProducts(id, lang);
            if(list.Count() == 0)
            {
                ViewBag.Message = "Chưa có sản phẩm cho nhóm này";
            }
            ViewBag.gnc = gnc;
            ViewBag.catID = id;
            ViewBag.list = list;
            ViewBag.CategoryList = categories;
            return View();
        }

        [HttpGet]
        public ActionResult Product_detail(int id, int categoryid)
        {
            setViewLanguage();
            string lang = ViewBag.Language;
            ProductsController apiProduct = new ProductsController();
            CategoryController apiCate = new CategoryController();
            var pro = apiProduct.GetProductsbyId(id, lang);
            ViewBag.Product = pro;
            var listhot = apiProduct.MoreProduct("1", lang);
            ViewBag.listhot = listhot;
            var listlq = apiProduct.GetProductsLQ(id,categoryid, lang);
            ViewBag.listlq = listlq;
            var categories = apiCate.GetAllCategories(lang);
            ViewBag.CategoryList = categories;
            var gnc = apiCate.GetCategoryName(categoryid, lang);
            ViewBag.gnc = gnc;
            ViewBag.catID = categoryid;
            return View();
        }

        public ActionResult News(int id)
        {
            setViewLanguage();
            NewsController apiNews = new NewsController();
            Category_NewsController apiGetCateName = new Category_NewsController();
            var list = apiNews.NewsbyCategory(id, ViewBag.Language);
            
            if (list.Count == 0)
            {
                ViewBag.Message = "Chưa có tin tức cho nhóm này";
            }
            var getname = apiGetCateName.GetCategoryName(id, ViewBag.Language);
            ViewBag.catNews = getname;
            ViewBag.list = list;
            ViewBag.CatNewsList = apiGetCateName.GetAllCategory_News(ViewBag.Language);
            ViewBag.catNews = getname.CategoryNews_Lang;
            ViewBag.catNewsID = getname.ID;
            return View();
        }

        public ActionResult News_detail(int id, int categoryid)
        {
            setViewLanguage();
            NewsController apiNews = new NewsController();
            ProductsController apiProduct = new ProductsController();
            Category_NewsController apiGetCateName = new Category_NewsController();
            BaseClasses.News list = apiNews.GetNewsbyId(id, ViewBag.Language);
            var getname = apiGetCateName.GetCategoryName(categoryid, ViewBag.Language);
            var relatedProducts = apiProduct.SearchProductByKeywords(ViewBag.Language, list.Keywords_EN);

            ViewBag.list = list;
            ViewBag.catNews = getname.CategoryNews_Lang;
            ViewBag.catNewsID = getname.ID;
            ViewBag.CatNewsList = apiGetCateName.GetAllCategory_News(ViewBag.Language);
            ViewBag.RelatedProducts = relatedProducts;
            return View();
        }

        public ActionResult SearchResult(string SearchText)
        {
            setViewLanguage();
            NewsController news_api = new NewsController();
            ProductsController product_api = new ProductsController();
            ViewBag.FoundProducts = product_api.SearchProducts(ViewBag.Language, SearchText);
            ViewBag.FoundNews =  news_api.SearchNews(ViewBag.Language, SearchText);
            ViewBag.Keywords = SearchText;
            return View();
        }
        public ActionResult About_us()
         {
            AboutController api = new AboutController();
            setViewLanguage();
            var list = api.GetAllAbout(ViewBag.Language);
            ViewBag.about = list;
            return View();
        }

        public ActionResult Pay()
        {
            return View();
        }
        public ActionResult UserInfo()
        {
            setViewLanguage();
            return View();
        }
        //public JsonResult InsertContact(Contact ct)
        //{
        //    var result = false;
        //    Contact prd = new Contact();
        //    prd.Name = ct.Name;
        //    prd.Email = ct.Email;
        //    prd.PhoneNumber = ct.PhoneNumber;
        //    prd.Note = ct.Note;
        //    prd.ContactTime = DateTime.Now;
        //    db.Contacts.InsertOnSubmit(prd);
        //    db.SubmitChanges();
        //    result = true;
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        //Login Facebook
        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallBack");
                return uriBuilder.Uri;
            }
        }

        public ActionResult LoginFacebook()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "email",
            });

            return Redirect(loginUrl.AbsoluteUri);
        }

        public ActionResult FacebookCallBack(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code
            });

            var accessToken = result.access_token;
            Session["AccessToken"] = accessToken;
            fb.AccessToken = accessToken;
            dynamic me = fb.Get("me?fields=link,first_name,currency,last_name,email,gender,locale,timezone,verified,picture,age_range,id");
            string email = me.email;
            string lastname = me.last_name;
            string firstname = me.first_name;
            string facebookid = me.id;
            FormsAuthentication.SetAuthCookie(email, false);
            string fullname = firstname + " " + lastname;
            UsersController apiAccount = new UsersController();
            var list = apiAccount.GetUserLoginFB(facebookid);
            if (list != null)
            {
                Session["userid"] = list.ID;
                Session["username"] = list.UserName;
                Session["email"] = list.Email;
                Session["name"] = list.Name;
                Session["image"] = list.Image;
                Session["phonenumber"] = list.PhoneNumber;
                Session["status"] = list.Status;
                Session["image"] = list.Image;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var listRegistry = apiAccount.RegistryFacebook(facebookid, email, fullname);
                if (listRegistry != null)
                {
                    Session["userid"] = listRegistry.ID;
                    Session["username"] = listRegistry.UserName;
                    Session["email"] = listRegistry.Email;
                    Session["name"] = listRegistry.Name;
                    Session["image"] = listRegistry.Image;
                    Session["phonenumber"] = listRegistry.PhoneNumber;
                    Session["status"] = listRegistry.Status;
                    Session["image"] = listRegistry.Image;
                    return RedirectToAction("Index","Home");
                }
                else
                {
                    return RedirectToAction("Login","Home");
                }
            }
        }

        //Login Google
        public ActionResult LoginGoogle(string googleID, string email, string fullname)
        {
            UsersController apiAccount = new UsersController();
            var list = apiAccount.GetUserLoginGoogle(googleID);
            if (list != null)
            {
                Session["userid"] = list.ID;
                Session["username"] = list.UserName;
                Session["email"] = list.Email;
                Session["name"] = list.Name;
                Session["image"] = list.Image;
                Session["phonenumber"] = list.PhoneNumber;
                Session["status"] = list.Status;
                Session["image"] = list.Image;
                return RedirectToAction("Profiles","Home");
            }
            else
            {
                var listRegistry = apiAccount.RegistryGoogle(googleID, email, fullname);
                if (listRegistry != null)
                {
                    Session["userid"] = listRegistry.ID;
                    Session["username"] = listRegistry.UserName;
                    Session["email"] = listRegistry.Email;
                    Session["name"] = listRegistry.Name;
                    Session["image"] = listRegistry.Image;
                    Session["phonenumber"] = listRegistry.PhoneNumber;
                    Session["status"] = listRegistry.Status;
                    Session["image"] = listRegistry.Image;
                    return RedirectToAction("Profiles","Home");
                }
                else
                {
                    return RedirectToAction("Login","Home");
                }
            }
        }

        [HttpGet]
        public JsonResult Account_detail(int IdUser)
        {
            setViewLanguage();
            UsersController apiUser = new UsersController();
            var list = apiUser.GetUsersbyId(IdUser);

            string value = string.Empty;

            value = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return Json(value, JsonRequestBehavior.AllowGet);

        }
        public JsonResult UpdateAccount(Ex_User user)
        {
            setViewLanguage();
            var result = false;
                    var file = user.ImageFile;
                    var extention = "";
                    var filenamewithoutextension = "";
                    if (file != null)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        extention = Path.GetExtension(file.FileName);
                        filenamewithoutextension = Path.GetFileNameWithoutExtension(file.FileName);

                        file.SaveAs(Server.MapPath("/UploadedImage/" + filenamewithoutextension + "_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + extention));
                    }

                    User Use = db.Users.SingleOrDefault(x => x.ID == user.ID);
                    //Use.UserName = Use.UserName;
                    //Use.Password = Use.Password;
                    //Use.GoogleID = Use.GoogleID;
                    //Use.FacebookID = Use.FacebookID;
                    //Use.Email = user.Email;
                    Use.Name = user.Name;
                    Use.PhoneNumber = user.PhoneNumber;
                    Use.Status = Use.Status;
                    if (file != null)
                    {
                        Use.Image = filenamewithoutextension + "_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute +  extention;
                    }
                    else
                    {
                        Use.Image = Use.Image;
                    }
                    db.SubmitChanges();
                    Session["email"] = Use.Email;
                    Session["name"] = Use.Name;
                    Session["phonenumber"] = Use.PhoneNumber;
                    Session["image"] = Use.Image;
                    result = true;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public string UpdateUserInfo(string fullname, string phonenumber, string email, string oldpassword, string newpassword)
        {
            setViewLanguage();
            string result = "";

            if (Session["userid"] != null)
            {
                UsersController user = new UsersController();
                var cur_user = user.GetUsersbyId(int.Parse(Session["userid"].ToString()));
                if(!cur_user.Email.Contains(email) || !cur_user.PhoneNumber.Contains(phonenumber))
                {
                    var ouser = new BaseClasses.User();
                    ouser.Email = email;
                    ouser.PhoneNumber = phonenumber;
                    if (user.CheckExistingUser( ouser))
                    {
                        return string.Format("ERROR|{0}|1", Resources.UserInfo.ExistingMember);
                    }
                    cur_user.Email = email;
                    cur_user.UserName = email;
                    cur_user.PhoneNumber = phonenumber;
                }
                if(oldpassword.Length > 0)
                {
                    Cryptography crypto = new Cryptography();
                    if(user.GetUserLogin(cur_user.UserName, crypto.Encrypt(oldpassword)) == null)
                    {
                        return string.Format("ERROR|{0}|2", Resources.UserInfo.IncorrectPassword);
                    }
                    cur_user.Password = crypto.Encrypt(newpassword);
                }
                cur_user.Name = fullname;
                user.UpdateUser(cur_user);
                result = string.Format("SUCCESS|{0}", Resources.UserInfo.Success_Updated);
            }
            return result;
        }
        //reset password user
        [NonAction]
        public bool IsEmailExist(string email)
        {

            var v = db.Users.Where(a => a.Email == email).FirstOrDefault();
            return v != null;
        }


        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string activationCode, string emailFor = "VerifyAccount")
        {
            var verifyUrl = "/Home/" + emailFor + "/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("email.tinhgiot@gmail.com", "Tinh Giot");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "tinhgiot123"; // Replace with actual password

            string subject = "";
            string body = "";
            if (emailFor == "VerifyAccount")
            {
                subject = "Your account is successfully created!";
                body = "<br/><br/>We are excited to tell you that your Dotnet Awesome account is" +
                    " successfully created. Please click on the below link to verify your account" +
                    " <br/><br/><a href='" + link + "'>" + link + "</a> ";

            }
            else if (emailFor == "ResetPassword")
            {
                subject = "Reset Password";
                body = "Hi,<br/><br/>We got request for reset your account password. Please click on the below link to reset your password" +
                    "<br/><br/><a href=" + link + ">Reset Password link</a>";
            }


            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }

        [HttpPost]
        public string ForgotPassword(string Email)
        {
            setViewLanguage();
            UsersController usr_api = new UsersController();
            User account = usr_api.GetUsersbyEmail(Email);
            string result = Resources.ForgotPassword.NonExistEmail;
            if (account != null)
            {
                string password = RandomPassword.Generate(8);
                Cryptography crypto = new Cryptography();
                account.Password = crypto.Encrypt(password);
                usr_api.UpdateUser(account);
                result = Resources.ForgotPassword.SentPassword;
                usr_api.SendNewPassword(account, password);
            }
            return result;
        }

        public ActionResult ResetPassword(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }

            var user = db.Users.Where(a => a.ResetPasswordCode == id).FirstOrDefault();
            if (user != null)
            {

                ResetPasswordModel model = new ResetPasswordModel();
                model.ResetCode = id;
                return View(model);
            }
            else
            {
                return HttpNotFound();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            setViewLanguage();
            var message = "";
            if (ModelState.IsValid)
            {

                var user = db.Users.Where(a => a.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                if (user != null)
                {
                    //user.Password = Crypto.Hash(model.NewPassword);
                    user.Password = model.NewPassword;
                    user.ResetPasswordCode = "";
                    db.SubmitChanges();

                    //    message = "New password updated successfully";

                    return RedirectToAction("Login", "Home");
                }

            }
            else
            {
                message = "Something invalid";
            }
            ViewBag.Message = message;
            return View(model);
        }

        public ActionResult UserOrderHistory()
        {
            setViewLanguage();
            var userid = Convert.ToString(Session["userid"]);
            OrderController api = new OrderController();
            var productorder = api.GetHistoryProductOrderByUserID(userid);
            var order = api.GetHistoryOrderByUserID(userid);
            ViewBag.listproductorder = productorder;
            ViewBag.listorder = order;
            ViewBag.t = userid;
            return View();
        }

        public ActionResult OrderInfo()
        {
            setViewLanguage();
            var userid = Convert.ToString(Session["userid"]);
            OrderController api = new OrderController();
            var order = api.GetHistoryOrderByUserID(userid);
            return PartialView("OrderInfo", order.ToList());
        }

        public ActionResult OrderDetail(string OrderID)
        {
            setViewLanguage();
            string id = (new Cryptography()).Decrypt(OrderID);
            OrderController api = new OrderController();
            ViewBag.Order = api.GetOrderbyId(id);
           var productOrders = api.GetAllOrderProductbyId(id);
            return PartialView("OrderDetail", productOrders.ToList());
        }
        public ActionResult DeliveryAddressList()
        {
            setViewLanguage();
            DeliveryAddressAPI del_addr = new DeliveryAddressAPI();
            ViewBag.CurrentCountry = del_addr.getCountry(del_addr.getClientIP());
            ViewBag.Countries = del_addr.GetCountries();          
            return PartialView("DeliveryAddressList", del_addr.GetDeliveryAddressList(Session["userid"].ToString()));
        }

        public ActionResult SelectDeliveryAddressList()
        {
            setViewLanguage();
            DeliveryAddressAPI del_addr = new DeliveryAddressAPI(); 
            List<DeliveryAddress> list = new List<DeliveryAddress>();
            ViewBag.CurrentCountry = del_addr.getCountry(del_addr.getClientIP());
            ViewBag.Countries = del_addr.GetCountries();
            if(Session["userid"] == null)
                return PartialView("SelectDeliveryAddressList", list);
            return PartialView("SelectDeliveryAddressList", del_addr.GetDeliveryAddressList(Session["userid"].ToString()));          
        }

        public ActionResult AddDeliveryAddress(DeliveryAddress oDeliveryAddress)
        {
            setViewLanguage();
            DeliveryAddressAPI del_addr_api = new DeliveryAddressAPI();
            ViewBag.CurrentCountry = del_addr_api.getCountry(del_addr_api.getClientIP());
            ViewBag.Countries = del_addr_api.GetCountries();
            oDeliveryAddress.UserID = Session["userid"].ToString();
            if(oDeliveryAddress.ID == 0)
                del_addr_api.AddNew(oDeliveryAddress);
            else
                del_addr_api.Update(oDeliveryAddress);
            return PartialView("DeliveryAddressList", del_addr_api.GetDeliveryAddressList(Session["userid"].ToString()));
        }

        public ActionResult AddSelectDeliveryAddress(DeliveryAddress oDeliveryAddress)
        {
            setViewLanguage();
            DeliveryAddressAPI del_addr_api = new DeliveryAddressAPI();
            ViewBag.CurrentCountry = del_addr_api.getCountry(del_addr_api.getClientIP());
            ViewBag.Countries = del_addr_api.GetCountries();
            oDeliveryAddress.UserID = Session["userid"].ToString();
            if (oDeliveryAddress.ID == 0)
                del_addr_api.AddNew(oDeliveryAddress);
            else
                del_addr_api.Update(oDeliveryAddress);
            return PartialView("SelectDeliveryAddressList", del_addr_api.GetDeliveryAddressList(Session["userid"].ToString()));
        }

        public JsonResult getDeliveryAddress(string ID)
        {
            setViewLanguage();
            DeliveryAddressAPI del_addr_api = new DeliveryAddressAPI();
            DeliveryAddress del_addr = del_addr_api.getDeliveryAddress(ID);
            return Json(del_addr, JsonRequestBehavior.AllowGet);
        }

        public ActionResult deleteDeliveryAddress(string ID)
        {
            setViewLanguage();
            DeliveryAddressAPI del_addr_api = new DeliveryAddressAPI();
            ViewBag.CurrentCountry = del_addr_api.getCountry(del_addr_api.getClientIP());
            ViewBag.Countries = del_addr_api.GetCountries();
            del_addr_api.Delete(ID);
            return PartialView("DeliveryAddressList", del_addr_api.GetDeliveryAddressList(Session["userid"].ToString()));
        }

        public ActionResult deleteSelectDeliveryAddress(string ID)
        {
            setViewLanguage();
            DeliveryAddressAPI del_addr_api = new DeliveryAddressAPI();
            ViewBag.CurrentCountry = del_addr_api.getCountry(del_addr_api.getClientIP());
            ViewBag.Countries = del_addr_api.GetCountries();
            del_addr_api.Delete(ID);
            return PartialView("SelectDeliveryAddressList", del_addr_api.GetDeliveryAddressList(Session["userid"].ToString()));
        }
    }
}