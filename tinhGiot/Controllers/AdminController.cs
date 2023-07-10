using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BaseClasses ;
using Newtonsoft.Json;
using System.IO;

namespace tinhGiot.Controllers
{
    public class Ex_AdminTG : AdminTG
    {
        public HttpPostedFileWrapper ImageFile { get; set; }
    }
    public class Ex_User : Users
    {
        public HttpPostedFileWrapper ImageFile { get; set; }
    }

    public class Ex_News : News
    {
        public Ex_News() : base() { }
        public Ex_News(string lang) : base(lang) { }
        public HttpPostedFileWrapper ImageFile { get; set; }
    }
    public class Ex_Products : Products
    {
        public Ex_Products() : base() { }
        public Ex_Products(string lang) : base(lang) { }
        public HttpPostedFileWrapper ImageFile { get; set; }
    }

    public class AdminController : Controller
    {
        dbTinhGiotDataContext db = new dbTinhGiotDataContext();
        // GET: Admin
        public ActionResult Index()
        {
            AdminController apiadmin = new AdminController();
            var listadmins = apiadmin.GetAllAdmin();
            ViewBag.admins = listadmins;
            return View();
        }

        public ActionResult Products(string Catid)
        {
            string lang = "VI";
            ProductsController product = new ProductsController();
            CategoryController catPro = new CategoryController();
            var cp = catPro.GetAllCategories(lang);
            if (string.IsNullOrEmpty(Catid))
                 Catid = cp.ElementAt(0).ID.ToString();
            var pd = product.GetProducts(int.Parse(Catid), lang);

            ViewBag.listpd = pd;
            ViewBag.listcp = cp;
            ViewBag.Init_SelectedCat = Catid;
            return View();
        }


        public ActionResult News(string Catid)
        {
            string lang = "VI";
            Category_NewsController catNews = new Category_NewsController();
            var listcat = catNews.GetAllCategory_News("");
            if (string.IsNullOrEmpty(Catid))
            {
                Catid = listcat.ElementAt(0).ID.ToString();
            }
            NewsController newss = new NewsController();
            var ntt = newss.NewsbyCategory(int.Parse(Catid), lang);
            ViewBag.listnews = ntt;
            ViewBag.listcn = listcat;
            ViewBag.Init_SelectedCat = Catid;
            return View();
        }

        public ActionResult Users()
        {
            UsersController user = new UsersController();
            var us = user.GetAllUsers();
            ViewBag.listuser = us;
            return View();
        }
        public ActionResult Contact()
        {
            ContactController ct = new ContactController();
            var t = ct.GetAllContact();
            ViewBag.listcontact = t;
            return View();
        }

        public ActionResult Profiles()
        {
            return View();
        }
        public ActionResult CategoryProduct(string lang)
        {
            CategoryController catep = new CategoryController();
            var cp = catep.GetAllCategories(lang);
            ViewBag.listcate = cp;
            return View();
        }

        public ActionResult CategoryNews()
        {
            Category_NewsController caten = new Category_NewsController();
            var cn = caten.GetAllCategory_News("");
            ViewBag.listcate = cn;
            return View();
        }
        public JsonResult UpdateAdminAccount(Ex_AdminTG admin)
        {
            var result = false;
            try
            {
                if (admin.ID > 0)
                {
                    var file = admin.ImageFile;
                    var extention = "";
                    var filenamewithoutextension = "";
                    if (file != null)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        extention = Path.GetExtension(file.FileName);
                        filenamewithoutextension = Path.GetFileNameWithoutExtension(file.FileName);

                        file.SaveAs(Server.MapPath("/UploadedImage/" + filenamewithoutextension + "_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + extention));
                    }

                    Admin adm = db.Admins.SingleOrDefault(x => x.ID == admin.ID);
                    adm.AdminName = admin.AdminName;
                    adm.Email = admin.Email;
                    adm.Name = admin.Name;
                    adm.PhoneNumber = admin.PhoneNumber;
                    adm.Password = admin.Password;
                    adm.Address = admin.Address;
                    adm.Type = adm.Type;
                    if (file != null)
                    {
                        adm.Image = filenamewithoutextension + "_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + extention;
                    }
                    else
                    {
                        adm.Image = adm.Image;
                    }
                    db.SubmitChanges();
                    result = true;

                }
                else
                {
                    var file = admin.ImageFile;
                    var extention = "";
                    var filenamewithoutextension = "";
                    if (file != null)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        extention = Path.GetExtension(file.FileName);
                        filenamewithoutextension = Path.GetFileNameWithoutExtension(file.FileName);

                        file.SaveAs(Server.MapPath("/UploadedImage/" + filenamewithoutextension + "_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + extention));
                    }

                    Admin ad = new Admin();
                    ad.AdminName = admin.AdminName;
                    ad.Email = admin.Email;
                    ad.PhoneNumber = admin.PhoneNumber;
                    ad.Name = admin.Name;
                    ad.Password = admin.Password;
                    ad.Address = admin.Address;
                    ad.Type = "admin";
                    if (file != null)
                    {
                        ad.Image = filenamewithoutextension + "_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + extention;

                    }
                    else
                    {
                        ad.Image = "Default.png";
                    }

                    db.Admins.InsertOnSubmit(ad);
                    db.SubmitChanges();

                    result = true;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteAdminRecord(int id)
        {
            AdminController apiUser = new AdminController();
            var list = apiUser.DeleteAdmin(id);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public IEnumerable<AdminTG> DeleteAdmin(int id)
        {
            GetDeleteAdmin(id);
            return admins;
        }
        private void GetDeleteAdmin(int id)
        {
            Admin ad = db.Admins.SingleOrDefault(n => n.ID == id);

            if (ad != null)
            {
                db.Admins.DeleteOnSubmit(ad);
                db.SubmitChanges();
            }
        }
        [HttpGet]
        public JsonResult Admindetail(int id)
        {
            AdminController apiAdmin = new AdminController();
            var list = apiAdmin.GetAdminbyId(id);

            string value = string.Empty;

            value = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return Json(value, JsonRequestBehavior.AllowGet);

        }
        public JsonResult UpdateUser(Ex_User user)
        {
            var result = false;
            try
            {
                if (user.ID > 0)
                {
                    var file = user.ImageFile;
                    var extention = "";
                    var filenamewithoutextension = "";
                    if (file != null)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        extention = Path.GetExtension(file.FileName);
                        filenamewithoutextension = Path.GetFileNameWithoutExtension(file.FileName);

                        file.SaveAs(Server.MapPath("/UploadedImage/" + filenamewithoutextension + "_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + extention));
                    }

                    User Use = db.Users.SingleOrDefault(x => x.ID == user.ID);
                    Use.UserName = user.UserName;
                    Use.Email = user.Email;
                    Use.Name = user.Name;
                    Use.PhoneNumber = user.PhoneNumber;
                    Use.Password = user.Password;
                    Use.Status = user.Status;
                    if (file != null)
                    {
                        Use.Image = filenamewithoutextension + "_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + extention;
                    }
                    else
                    {
                        Use.Image = Use.Image;
                    }
                    db.SubmitChanges();
                    result = true;

                }
                else
                {
                    var file = user.ImageFile;
                    var extention = "";
                    var filenamewithoutextension = "";
                    if (file != null)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        extention = Path.GetExtension(file.FileName);
                        filenamewithoutextension = Path.GetFileNameWithoutExtension(file.FileName);

                        file.SaveAs(Server.MapPath("/UploadedImage/" + filenamewithoutextension + "_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + extention));
                    }

                    User us = new User();
                    us.UserName = user.UserName;
                    us.Email = user.Email;
                    us.PhoneNumber = user.PhoneNumber;
                    us.Name = user.Name;
                    us.Password = user.Password;
                    us.Status = user.Status;
                    if (file != null)
                    {
                        us.Image = filenamewithoutextension + "_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + extention;

                    }
                    else
                    {
                        us.Image = "Default.png";
                    }

                    db.Users.InsertOnSubmit(us);
                    db.SubmitChanges();

                    result = true;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteUserRecord(int UserID)
        {
            UsersController apiUser = new UsersController();
            var list = apiUser.DeleteUsers(UserID);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult User_detail(int IdUser)
        {
            UsersController apiUser = new UsersController();
            var list = apiUser.GetUsersbyId(IdUser);

            string value = string.Empty;

            value = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return Json(value, JsonRequestBehavior.AllowGet);

        }

        List<AdminTG> admins = new List<AdminTG>();
        public IEnumerable<AdminTG> GetAllAdmin()
        {
            GetAdmin();
            return admins;
        }
        private void GetAdmin()
        {
            var ad = db.Admins.ToList();

            foreach (var item in ad)
            {
                admins.Add(new AdminTG
                {
                    ID = Convert.ToInt32(item.ID),
                    AdminName = item.AdminName,
                    Password = item.Password,
                    Email = item.Email,
                    Name = item.Name,
                    Type = item.Type,
                    Image = item.Image,
                    PhoneNumber = item.PhoneNumber,
                    Address = item.Address,
                });
            }
        }
        public AdminTG GetAdminLogin(string adminuser, string password)
        {
            if (admins.Count() > 0)
            {
                return admins.Find(x => x.AdminName == adminuser && x.Password == password);
            }
            else
            {
                GetAdmin();
                var us = admins.Find(x => x.AdminName == adminuser && x.Password == password);
                return us;
            }
        }
        public AdminTG GetAdminbyId(int id)
        {
            if (admins.Count() > 0)
            {
                return admins.Find(p => p.ID == id);
            }
            else
            {
                GetAdmin();
                var abc = admins.Find(p => p.ID == id);
                return abc;
            }
        }
        //Login admin
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string adminuser, string password)
        {

            AdminController us = new AdminController();
            var user = us.GetAdminLogin(adminuser, password);
            if (user != null)
            {
                Session["userid"] = user.ID;
                Session["username"] = user.AdminName;
                Session["email"] = user.Email;
                Session["name"] = user.Name;
                Session["image"] = user.Image;
                Session["type"] = user.Type;
                Session["phonenumber"] = user.PhoneNumber;
                Session["address"] = user.Address;
                return RedirectToAction("../vi/admin/Index");
            }
            ViewBag.error = "<p style='color: red;'>Please check your email and password.</p>";
            return View();
        }

        //==== Logout ====//
        public ActionResult Logout()
        {

            Session["userid"] = null;
            Session["username"] = null;
            Session["email"] = null;
            Session["name"] = null;
            Session["image"] = null;
            Session["address"] = null;
            Session["phonenumber"] = null;
            Session["status"] = null;
            Session["image"] = null;
            Session["type"] = null;
            Session["phonenumber"] = null;
            Session["address"] = null;
            return new RedirectResult("/vi/admin/Index");
        }
        [ValidateInput(false)]
        public JsonResult InsertAndUpdateProduct(Ex_Products pd)
        {
            var result = false;
            try
            {
                Product prd = null;
                if (pd.ID > 0)
                {

                    var filename = "";
                    var extention = "";
                    var filenamewithoutextension = "";

                    prd = db.Products.SingleOrDefault(x => x.ID == pd.ID);

                    if (Request.Files.Count > 0)
                    {
                        for (int i = 0; i < Request.Files.Count; i++)
                        {

                            HttpPostedFileBase file = Request.Files[i];
                            extention = Path.GetExtension(file.FileName);
                            filenamewithoutextension = Path.GetFileNameWithoutExtension(file.FileName);

                            file.SaveAs(Server.MapPath("/UploadedImage/" + filenamewithoutextension + "_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + extention));

                            if (i == 0)
                            {
                                filename += filenamewithoutextension + "_" + DateTime.Now.ToString("ddmmyyyy") + extention;
                            }
                            else
                            {
                                filename += filenamewithoutextension + "_" + DateTime.Now.ToString("ddmmyyyy") + extention + ",";
                            }
                        }

                        prd.Image =string.Format("{0},{1}", filename, prd.Image);
                    }
                    else
                    {
                        prd.Image = prd.Image;
                    }
                }
                else
                {
                    var filename = "";
                    var extention = "";
                    var filenamewithoutextension = "";
                    for (int i = 0; i < Request.Files.Count; i++)
                    {

                        HttpPostedFileBase file = Request.Files[i];
                        extention = Path.GetExtension(file.FileName);
                        filenamewithoutextension = Path.GetFileNameWithoutExtension(file.FileName);

                        file.SaveAs(Server.MapPath("/UploadedImage/" + filenamewithoutextension + "_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + extention));

                        if (i == 0)
                        {
                            filename += filenamewithoutextension + "_" + DateTime.Now.ToString("ddmmyyyy") + extention;
                        }
                        else
                        {
                            filename += filenamewithoutextension + "_" + DateTime.Now.ToString("ddmmyyyy") + extention + ",";
                        }
                    }

                    prd = new Product();
                    if (filename != "")
                    {
                        prd.Image = filename;
                    }
                    else
                    {
                        prd.Image = "Default.png";
                    }
                }

                prd.ProductName = pd.ProductName;
                prd.ProductName_EN = pd.ProductName_EN;
                prd.ProductName_JP = pd.ProductName_JP;
                prd.Price = pd.Price;
                prd.Capacity = pd.Capacity;
                prd.CategoryID = pd.CategoryID;
                prd.Quantity = pd.Quantity;
                prd.SKU = pd.SKU;
                prd.Status = pd.Status;
                prd.MeasureUnit = pd.MeasureUnit;
                prd.UnitPrice = pd.UnitPrice;
                prd.Description = pd.Description;
                prd.Description_EN = pd.Description_EN;
                prd.Description_JP = pd.Description_JP;
                prd.MoreInfo = pd.MoreInfo;
                prd.Uses = pd.Uses;
                prd.Uses_EN = pd.Uses_EN;
                prd.Uses_JP = pd.Uses_JP;
                prd.ShortDescription = pd.ShortDescription;
                prd.ShortDescription_EN = pd.ShortDescription_EN;
                prd.ShortDescription_JP = pd.ShortDescription_JP;
                prd.Keywords = pd.Keywords;
                prd.Keywords_EN = pd.Keywords_EN;
                prd.Keywords_JP = pd.Keywords_JP;
                if(pd.ID > 0)
                {
                    db.SubmitChanges();
                    result = true;
                }
                else
                {
                    db.Products.InsertOnSubmit(prd);
                    db.SubmitChanges();
                    result = true;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteProductRecord(int ProductID)
        {
            ProductsController api = new ProductsController();
            var list = api.DeleteProduct(ProductID);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Product_detail(int ProductID, string lang)
        {
            ProductsController api = new ProductsController();
            var list = api.GetProductsbyId(ProductID, lang);
            string value = string.Empty;

            value = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return Json(value, JsonRequestBehavior.AllowGet);

        }
        public JsonResult DeleteCategoryProductRecord(int CateID)
        {
            CategoryController api = new CategoryController();
            var list = api.DeleteCateProduct(CateID);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CateProduct_detail(int IdCate, string lang)
        {
            CategoryController api = new CategoryController();
            var list = api.GetCateProductsbyId(IdCate, lang);

            string value = string.Empty;

            value = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return Json(value, JsonRequestBehavior.AllowGet);

        }

        public JsonResult UpdateCategoryProduct(CategoryProduct cate)
        {
            var result = false;
            try
            {
                if (cate.ID > 0)
                {
                    CategoryProduct cp = db.CategoryProducts.SingleOrDefault(x => x.ID == cate.ID);
                    cp.CategoryName = cate.CategoryName;
                    cp.CategoryName_EN = cate.CategoryName_EN;
                    cp.CategoryName_JP = cate.CategoryName_JP;
                    db.SubmitChanges();
                    result = true;

                }
                else
                {
      
                    CategoryProduct cp = new CategoryProduct();
                    cp.CategoryName = cate.CategoryName;
                    cp.CategoryName_EN = cate.CategoryName_EN;
                    cp.CategoryName_JP = cate.CategoryName_JP;
                    db.CategoryProducts.InsertOnSubmit(cp);
                    db.SubmitChanges();

                    result = true;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteCategoryNewsRecord(int CateID)
        {
            Category_NewsController api = new Category_NewsController();
            var list = api.DeleteCateNews(CateID);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CateNews_detail(int IdCate)
        {
            Category_NewsController api = new Category_NewsController();
            var list = api.GetCateNewsbyId(IdCate, "");

            string value = string.Empty;

            value = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return Json(value, JsonRequestBehavior.AllowGet);

        }

        public JsonResult UpdateCategoryNews(Category_News cate)
        {
            var result = false;
            try
            {
                if (cate.ID > 0)
                {
                    CategoryNew cp = db.CategoryNews.SingleOrDefault(x => x.ID == cate.ID);
                    cp.CategoryNews = cate.CategoryNews;
                    cp.CategoryNews_EN = cate.CategoryNews_EN;
                    cp.CategoryNews_JP = cate.CategoryNews_JP;
                    db.SubmitChanges();
                    result = true;

                }
                else
                {

                    CategoryNew cp = new CategoryNew();
                    cp.CategoryNews = cate.CategoryNews;
                    cp.CategoryNews_EN = cate.CategoryNews_EN;
                    cp.CategoryNews_JP = cate.CategoryNews_JP;
                    db.CategoryNews.InsertOnSubmit(cp);
                    db.SubmitChanges();

                    result = true;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult News_detail(int IdNews)
        {
            NewsController api = new NewsController();
            var list = api.GetNewsbyId(IdNews, "");

            string value = string.Empty;

            value = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return Json(value, JsonRequestBehavior.AllowGet);

        }

        [ValidateInput(false)]
        public JsonResult UpdateNews(Ex_News news)
        {
            var result = false;
            try
            {
                New Ne = null;
                var extention = "";
                var filenamewithoutextension = "";
                var file = news.ImageFile;
                if (file != null)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    extention = Path.GetExtension(file.FileName);
                    filenamewithoutextension = Path.GetFileNameWithoutExtension(file.FileName);

                    file.SaveAs(Server.MapPath("/UploadedImage/" + filenamewithoutextension + "_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + extention));
                }
                if (news.ID > 0)
                {
                    Ne = db.News.SingleOrDefault(x => x.ID == news.ID);
                }
                else
                {
                    Ne = new New();
                }
                Ne.Title = news.Title;
                Ne.Title_EN = news.Title_EN;
                Ne.Title_JP = news.Title_JP;
                //Ne.PostTime = news.PostTime;
                Ne.Author = news.Author;
                Ne.CategoryID = news.CategoryID;
                Ne.Lang = news.Lang;
                Ne.MoreInfo = news.MoreInfo;
                Ne.PostContent = news.PostContent;
                Ne.PostContent_EN = news.PostContent_EN;
                Ne.PostContent_JP = news.PostContent_JP;
                Ne.Keywords = news.Keywords;
                Ne.Keywords_EN = news.Keywords_EN;
                Ne.Keywords_JP = news.Keywords_JP;
                if(news.ID > 0)
                {
                    if (file != null)
                    {
                        Ne.Image = filenamewithoutextension + "_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + extention;
                    }
                    else
                    {
                        Ne.Image = Ne.Image;
                    }
                    db.SubmitChanges();
                    result = true;
                }
                else
                {
                    if (file != null)
                    {
                        Ne.Image = filenamewithoutextension + "_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + extention;
                    }
                    else
                    {
                        Ne.Image = "Default.png";
                    }
                    db.News.InsertOnSubmit(Ne);
                    db.SubmitChanges();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteNewsRecord(int NewsID)
        {
            NewsController api = new NewsController();
            var list = api.DeleteNews(NewsID);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Contact_detail(int contactid)
        {
            ContactController api = new ContactController();
            var list = api.GetContactbyId(contactid);
            string value = string.Empty;

            value = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return Json(value, JsonRequestBehavior.AllowGet);

        }
        public JsonResult DeleteContactRecord(int contactid)
        {
            ContactController api = new ContactController();
            var list = api.DeleteContact(contactid);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ManageOrder()
        {
            OrderController api = new OrderController();
            var order = api.GetAllOrder();
            var orderproduct = api.GetAllOrderProduct();
            ViewBag.order = order;
            ViewBag.orderproduct = orderproduct;
            return View();
        }

        [HttpGet]
        public JsonResult Order_detail(string OrderID)
        {
            OrderController api = new OrderController();
            var order = api.GetOrderbyId(OrderID);

            string value = string.Empty;

            value = JsonConvert.SerializeObject(order, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return Json(value, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult OrderProduct_detail(string OrderID)
        {
            OrderController api = new OrderController();
            var orderproduct = api.GetAllOrderProductbyId(OrderID);

            string value = string.Empty;

            value = JsonConvert.SerializeObject(orderproduct, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return Json(value, JsonRequestBehavior.AllowGet);

        }
        public JsonResult DeleteOrderRecord(string OrderID)
        {
            OrderController api = new OrderController();
            var list = api.DeleteOrder(OrderID);
            var listv2 = api.DeleteOrderProduct(OrderID);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Admin_Account_Detail(int IdAdmin)
        {
            AdminController apiAdmin = new AdminController();
            var list = apiAdmin.GetAdminbyId(IdAdmin);

            string value = string.Empty;

            value = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return Json(value, JsonRequestBehavior.AllowGet);

        }
        public JsonResult UpdateAdmin(Ex_AdminTG admin)
        {
                    var result = false;
                    var file = admin.ImageFile;
                    var extention = "";
                    var filenamewithoutextension = "";
                    if (file != null)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        extention = Path.GetExtension(file.FileName);
                        filenamewithoutextension = Path.GetFileNameWithoutExtension(file.FileName);

                        file.SaveAs(Server.MapPath("/UploadedImage/" + filenamewithoutextension + "_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + extention));
                    }

                    Admin ad = db.Admins.SingleOrDefault(x => x.ID == admin.ID);
                    ad.AdminName = ad.AdminName;
                    ad.Password = ad.Password;
                    ad.Email = admin.Email;
                    ad.Name = admin.Name;
                    ad.PhoneNumber = admin.PhoneNumber;
                    ad.Address = admin.Address;
                    if (file != null)
                    {
                        ad.Image = filenamewithoutextension + "_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + extention;
                    }
                    else
                    {
                        ad.Image = ad.Image;
                    }
                    db.SubmitChanges();
                    Session["email"] = ad.Email;
                    Session["name"] = ad.Name;
                    Session["address"] = ad.Address;
                    Session["phonenumber"] = ad.PhoneNumber;
                    Session["image"] = ad.Image;
                    result = true;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult About_us()
        {
            AboutController api = new AboutController();
            var list = api.GetAllAbout("");
            ViewBag.about = list;
            return View();
        }
        [HttpGet]
        public JsonResult About_us_detail(int id)
        {
            AboutController api = new AboutController();
            var list = api.GetAboutbyId(id);
            string value = string.Empty;

            value = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return Json(value, JsonRequestBehavior.AllowGet);

        }
        [ValidateInput(false)]
        public JsonResult UpdateAbout(About_us ab)
        {
            var result = false;
            try
            {
                if (ab.ID > 0)
                {
                    About cp = db.Abouts.SingleOrDefault(x => x.ID == ab.ID);
                    cp.AboutContent = ab.AboutContent;
                    cp.AboutContent_EN = ab.AboutContent_EN;
                    db.SubmitChanges();
                    result = true;

                }
                else
                {

                    About cp = new About();
                    cp.AboutContent = ab.AboutContent;
                    cp.AboutContent_EN = ab.AboutContent_EN;
                    db.Abouts.InsertOnSubmit(cp);
                    db.SubmitChanges();

                    result = true;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateOrder(Order od)
        {
            var result = false;
            try
            {
                if (od.ID > 0)
                {
                    UserOrder cp = db.UserOrders.SingleOrDefault(x => x.ID == od.ID);
                    cp.StatusPayment = od.StatusPayment;
                    cp.Payment = od.Payment;
                    cp.DeliveryStatus = od.DeliveryStatus;
                    db.SubmitChanges();
                    result = true;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }

}
