using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BaseClasses;
using System.Web.Script.Serialization;
using PayPal.Api;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

namespace tinhGiot.Controllers
{
    public class CartItemController : BaseController
    {
        
        dbTinhGiotDataContext db = new dbTinhGiotDataContext();
        private PayPal.Api.Payment payment;

        public async Task<ActionResult> HeaderCart()
        {
            
            string buyerID = "";
            string lang = this.getLanguage();
            if (Request.Cookies["BuyerID"] != null)
            {
                buyerID = Request.Cookies["BuyerID"].Value;
            }
            else
            {
                buyerID = System.Guid.NewGuid().ToString();
                Response.Cookies["BuyerID"].Value = buyerID;
                Response.Cookies["BuyerID"].Expires = DateTime.Now.AddDays(14);
            }
            if (Session["CartSession"] == null)
                await getCartItems(buyerID, lang);
            return PartialView("/Views/Shared/HeaderCart.cshtml", Session["CartSession"]);
        }


        // GET: CartItem
        public ActionResult Index()
        {
            var cart =  Session["CartSession"];
            var list = new List<CartItem>();
            if(cart != null)
            {
                list = (List<CartItem>)cart;
            }
            return View(list);
        }

        public Task<string> getCartItems(string buyerID, string lang)
        {
            var list = db.CartItem.Where(x => x.BuyerID == buyerID);
            List<CartItems> cartItemList = new List<CartItems>();
            if (list != null)
            {
                foreach (var item in list)
                {
                    cartItemList.Add(PassValue(item, lang));
                }
            }
            Session["CartSession"] = cartItemList;
            return Task.FromResult("");
        }


        public Task<CartItems> getCartItem(string buyerID, int ItemID, string lang)
        {
            var list = db.CartItem.Where(x => (x.BuyerID == buyerID) && (x.ID == ItemID) );
            CartItems oItem = null;
            if (list.Count() > 0)
               oItem = PassValue(list.First(), lang);
           
            return Task.FromResult( oItem);
        }

        // pass value from object cartitem to cartItems
        private  CartItems PassValue(CartItem oCartItem, string lang)
        {
            CartItems oItem = new CartItems(lang);
            oItem.ProductID = (int)oCartItem.ProductID;
            oItem.Image = oCartItem.Image;
            oItem.Price = oCartItem.Price;
            oItem.ProductName = oCartItem.ProductName;
            oItem.ProductName_EN = oCartItem.ProductName_EN;
            oItem.ProductName_JP = oCartItem.ProductName_JP;
            oItem.BuyerID = oCartItem.BuyerID;
            oItem.Quantity = oCartItem.Quantity;
            oItem.SKU = oCartItem.SKU;
            return oItem;
        }
        public ActionResult LoginCheckout(string username, string password)
        {
            return View();
        }

        public ActionResult CreateMemberCheckOut(User oUser)
        {


            return View();
        }

        public async Task<string> CompleteCheckOut(User oUser, DeliveryAddress oDeliveryAddress, string DeliveryNote, bool isDeliveryGift, string GiftMessage, string PaymentType)
        {
            setViewLanguage();
            string result = "";
            var user = oUser;
            string BuyerID = Request.Cookies["BuyerID"].Value;
            string lang = ViewBag.Language;
            var OrderTime = DateTime.Now;
            var TGE = "";
            string OrderId = OrderTime.ToString("ddMMyyy_hh_mm_ss_ms"); 

             var deliver_addr = oDeliveryAddress;
            // create new user
            if(user.ID == 0)
            {
                // create user 
                UsersController oUserApi = new UsersController();
                if (oUserApi.CheckExistingUser(oUser))
                {
                    return string.Format( "ERROR|{0}", Resources.ProcessCheckout.Account_Email_message);
                }
                RandomPassword randomPassword = new RandomPassword();
                //   random.
                string password = RandomPassword.Generate(8);
                Cryptography cryptography = new Cryptography();
                oUser.Password =cryptography.Encrypt(password);
                oUser.Image = "Default.png";
                if (oUserApi.RegisterNewUser(oUser))
                {
                    // get user ID 
                    oUser = oUserApi.GetUsersbyEmail(oUser.Email);
                    // sending a message for a new password
                    oUserApi.SendCreatedPassword(oUser, password);
                    // create shipping address
                    DeliveryAddressAPI addr_api = new DeliveryAddressAPI();
                    oDeliveryAddress.UserID = oUser.ID.ToString();
                    oDeliveryAddress.IsDefault = true;
                    addr_api.AddNew(oDeliveryAddress);
                }
            }

            await this.getCartItems(BuyerID, lang);
            var  list = (List<CartItems>)Session["CartSession"];
            int total = 0;
            foreach (var item in list)
            {
                ProductOrder po = new ProductOrder();
                po.ProductID = Convert.ToString(item.ProductID);
                po.Quantity = item.Quantity;
                po.OrderID = OrderId;
                po.Price = item.Price;
                po.TimeOrder = OrderTime;
                po.UserID = oUser.ID.ToString();
                po.ProductName = item.ProductName;
                po.ProductName_EN = item.ProductName_EN;
                po.ProductName_JP = item.ProductName_JP;
                po.SKU = item.SKU;
                db.ProductOrders.InsertOnSubmit(po);
                db.SubmitChanges();
                total = total + (item.Quantity * Convert.ToInt32(item.Price));
                string name = item.ProductName_Lang;
                TGE = TGE + "<tr>" + "<td>" + po.Quantity + "</td>" + "<td>" + name + "</td>" + "</tr>";
            }
          //  PartialView("OrderDetail").ToString();
            string[] discount = (System.Configuration.ConfigurationManager.AppSettings["DiscountPercent"]).ToString().Split('|');
            UserOrder uso = new UserOrder();
            uso.OrderID = OrderId;
            uso.UserID = oUser.ID.ToString();
            uso.Name = deliver_addr.Fullname;
            uso.PhoneNumber = deliver_addr.PhoneNumber;
            uso.Email = oUser.Email;
            uso.Address = deliver_addr.Address;
            uso.Ward = deliver_addr.Ward;
            uso.District = deliver_addr.District;
            uso.IsGifted = isDeliveryGift;
            uso.GiftMessage = GiftMessage;
            uso.City = deliver_addr.City;
            uso.Note = DeliveryNote;
            uso.TimeOrder = OrderTime;
            uso.Payment = PaymentType;
            uso.StatusPayment = false;
            uso.Country = deliver_addr.Country;
            uso.Postcode = deliver_addr.Postcode;
            uso.PromotionCode = "";
            uso.DiscountValue = total >= int.Parse(discount[0]) ? int.Parse(discount[1]) : 0 ;
            if(deliver_addr.Country == "Vietnam")
                uso.DeliveryCost = total >= int.Parse((System.Configuration.ConfigurationManager.AppSettings["FreeDeliveryCost"]).ToString()) ? "0" : System.Configuration.ConfigurationManager.AppSettings["DeliveryCost"].ToString();
            else
                uso.DeliveryCost = (System.Configuration.ConfigurationManager.AppSettings["OtherDeliveryCost"]).ToString();
            uso.DeliveryStatus = "0";
            uso.PaymentID = "";
            uso.TotalPrice = (total + int.Parse(uso.DeliveryCost) - uso.DiscountValue.GetValueOrDefault()* total / 100).ToString();
            db.UserOrders.InsertOnSubmit(uso);
            db.SubmitChanges();

            // keep track orderid 

            // process payment
            // create an order 

            // COD  

            // Paypal

            switch (PaymentType)
            {
                case "COD":
                    SendInvoice(uso, TGE,"COD",false);
                    result =string.Format("SUCCESS|{0}|{1}", Resources.ProcessCheckout.Order_Success, (new Cryptography()).Encrypt(OrderId));
                    break;
                case "PAYPAL":
                    result = string.Format("SUCCESS|{0}|{1}",Resources.ProcessCheckout.Order_Success_Pending_Payment, (new Cryptography()).Encrypt(OrderId));
                    Response.Cookies["OrderID"].Value = OrderId;
                    Response.Cookies["OrderID"].Expires = DateTime.Now.AddDays(14);
                    break;
            }
            //Clear Session cart item
            Session["CartSession"] = null;
            // delete all items in cart
            DeleteAllItems(BuyerID);
            // clear buyerID  cookies
            //Response.Cookies["BuyerID"].Expires = DateTime.Now.AddDays(-1);
            return result;

        }
        public async Task<ActionResult> ProcessCheckout()
        {
            setViewLanguage();
            string BuyerID = Request.Cookies["BuyerID"].Value;

            await this.getCartItems(BuyerID, ViewBag.Language);
            var cart = Session["CartSession"];

            var list = new List<CartItems>();
            if (cart != null)
            {
                list = (List<CartItems>)cart;
            }
            int total = list.Sum(x => x.Quantity * int.Parse(x.Price));
            int items = list.Sum(x => x.Quantity);
            DeliveryAddressAPI del_addr_api = new DeliveryAddressAPI();
            Session["fullname_delivery"] = "";
            Session["phonenumber_delivery"] = "";
            Session["address_delivery"] = "";
            Session["ward_delivery"] = "";
            Session["district_delivery"] = "";
            Session["city_delivery"] = "";
            Session["postcode_delivery"] = "";
            ViewBag.CurrentCountry = del_addr_api.getCountry(del_addr_api.getClientIP());
            if(ViewBag.CurrentCountry == "")
            {
                ViewBag.CurrentCountry = "Vietnam";
            }
            ViewBag.Countries = del_addr_api.GetCountries();

            ViewBag.DeliveryCost = total >= int.Parse((System.Configuration.ConfigurationManager.AppSettings["FreeDeliveryCost"]).ToString()) ? "0":System.Configuration.ConfigurationManager.AppSettings["DeliveryCost"].ToString();
            ViewBag.DeliveryOtherCountryCost = (System.Configuration.ConfigurationManager.AppSettings["OtherDeliveryCost"]).ToString(); 
            string[] discount = (System.Configuration.ConfigurationManager.AppSettings["DiscountPercent"]).ToString().Split('|');
            ViewBag.DiscountPercent = total >= int.Parse(discount[0])? discount[1]:"0";
            ViewBag.PayableAmount = total - int.Parse(ViewBag.DiscountPercent) * total/100;
            ViewBag.Total = total;
            ViewBag.Items = items;
            if (Session["userid"] != null)
            {
               var addr = del_addr_api.getDefaultDeliveryAddress(Session["userid"].ToString());
                Session["fullname_delivery"] = addr.Fullname;
                Session["phonenumber_delivery"] = addr.PhoneNumber;
                Session["address_delivery"] = addr.Address;
                Session["ward_delivery"] = addr.Ward;
                Session["district_delivery"] = addr.District;
                Session["city_delivery"] = addr.City;
                Session["postcode_delivery"] = addr.Postcode;
                ViewBag.CurrentCountry = addr.Country;
            }
            return View(list);
        }

        public ActionResult OrderCompelete()
        {
            return View();
        }
        public JsonResult DeleteAll()
        {
            Session["CartSession"] = null;
            return Json(new
            {
                status = true
            });
        }
        public JsonResult Delete(int id)
        {
            var sessionCart = (List<CartItem>)Session["CartSession"];
            sessionCart.RemoveAll(x => x.ID == id);
            Session["CartSession"] = sessionCart;
            return Json(new
            {
                status = true
            });
        }
        public async Task<ActionResult> Update(int id, int Qty)
        {
            var cart = (List<CartItems>)Session["CartSession"];
            foreach (var item in cart)
            {

                if (item.ProductID == id)
                {
                    if (Qty > 0)
                    {
                        item.Quantity = Qty;
                        //update  database
                      await Task.Run(() => UpdateItemQuantity(item));
                    }
                    else
                    {
                        cart.Remove(item);
                        // update database
                        await Task.Run(() => DeleteItem(item));
                    }
                    break;
                }
            }
            Session["CartSession"] = cart;
            return PartialView("PreviewBag", cart);
        }

        public async Task<ActionResult> DeleteItemBag(int id)
        {
            var cart = (List<CartItems>)Session["CartSession"];
            foreach (var item in cart)
            {
                if (item.ProductID == id)
                {
                        cart.Remove(item);
                        // update database
                        await Task.Run(() => DeleteItem(item));
                    break;
                }
            }
            Session["CartSession"] = cart;
            return PartialView("PreviewBag", cart);
        }

        public async Task< ActionResult> AddItem(int id, int quantity)
        {
            string lang = "VI";
            // check current buyerID
               string buyerID = "";
               if (Request.Cookies["BuyerID"] != null)
               {
                   buyerID = Request.Cookies["BuyerID"].Value;
               }
               else
               {
                   buyerID = System.Guid.NewGuid().ToString();
                   Response.Cookies["BuyerID"].Value = buyerID;
                   Response.Cookies["BuyerID"].Expires = DateTime.Now.AddDays(14);
               }
            var cart = (List<CartItems>)Session["CartSession"];
            if (cart.Count() > 0)
            {
                if (cart.Exists(x => x.ProductID == id))
                {
                    foreach (var item in cart)
                    {
                        if (item.ProductID == id)
                        {
                            item.Quantity += quantity;
                           await Task.Run(() => UpdateItemQuantity(item));
                            break;
                        }
                    }
                }
                else
                {
                    CartItems item = await GetOrderItem(id, lang, buyerID);
                    item.Quantity = quantity;
                  await  Task.Run(() => AddNewItem(item));
                    cart.Add(item);
                }
            }
            else
            {
                CartItems item = await GetOrderItem(id, lang, buyerID);
                item.Quantity = quantity;
               await Task.Run(() => AddNewItem(item));
                cart.Add(item);
            }
            ViewBag.List = cart;
            Session["CartSession"] = cart;

            return PartialView("/Views/Shared/HeaderCart.cshtml", cart);
        }
        private Task<CartItems> GetOrderItem(int id, string lang, string buyerID)
        {
    
            ProductsController apiProduct = new ProductsController();
           Products product = apiProduct.GetProductsbyId(id, lang);
            CartItems item = new CartItems(lang);
            item.ProductID= product.ID;
            item.Image = product.Image;
            item.Price = product.Price;
            item.SKU = product.SKU;
            item.ProductName = product.ProductName;
            item.ProductName_EN = product.ProductName_EN;
            item.ProductName_JP = product.ProductName_JP;
            item.BuyerID = buyerID;
            item.SKU = product.SKU;
            item.TimeStamp = DateTime.Now;
            return Task.FromResult(item);
        }
        public async Task<ActionResult> PreviewBag()
        {
            string buyerID = "";
            if (Request.Cookies["BuyerID"] != null)
            {
                buyerID = Request.Cookies["BuyerID"].Value;
            }
            else
            {
                buyerID = System.Guid.NewGuid().ToString();
                Response.Cookies["BuyerID"].Value = buyerID;
                Response.Cookies["BuyerID"].Expires = DateTime.Now.AddDays(14);
            }

            string lang = "VI";
            if (Session["CartSession"] == null)
                await this.getCartItems(buyerID, lang);
            var cart = Session["CartSession"];
            var list = new List<CartItems>();
            if (cart != null)
            {
                list = (List<CartItems>)cart;
            }
            return PartialView("PreviewBag", list); 
        }

        public ActionResult PaymentWithPaypal()
        {
            APIContext apiContext = PayPalConfiguration.GetAPIContext();

            try
            {
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/CartItem/PaymentWithPayPal?";
                    var guid = Convert.ToString((new Random()).Next(100000));
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;

                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = lnk.href;
                        }
                    }

                    Session.Add(guid, createdPayment.id);

                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("FailureView");
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error" + ex.Message);
                return View("FailureView");
            }
            var getpaymentdetails = this.GetPaymentDetails(Request.Params["paymentId"]);
            //Save Order to database
            var listCarts = (List<CartItems>)Session["CartSession"];
            string OrderId = Convert.ToString((new Random()).Next(100000)) + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour;
            var GetUserID = Session["userid"].ToString();
            var OrderTime = DateTime.Now;
            var TGE = "";
            foreach (var item in listCarts)
            {
                ProductOrder po = new ProductOrder();
                po.ProductID = Convert.ToString(item.ID);
                po.Quantity = item.Quantity;
                po.OrderID = OrderId;
                po.Price = item.Price;
                po.TimeOrder = OrderTime;
                po.UserID = GetUserID;
                po.ProductName = item.ProductName;
                Session["totalprice"] = Convert.ToInt32(Session["totalprice"]) + (item.Quantity * Convert.ToInt32(item.Price));
                db.ProductOrders.InsertOnSubmit(po);
                db.SubmitChanges();
                TGE = TGE + "<tr>" + "<td>" + po.Quantity + "</td>" + "<td>" + po.ProductName + "</td>" + "</tr>";
            }
            UserOrder uso = new UserOrder();
            uso.OrderID = OrderId;
            uso.UserID = GetUserID;
            uso.Name = getpaymentdetails.payer.payer_info.first_name +" "+ getpaymentdetails.payer.payer_info.first_name;
            uso.PhoneNumber = getpaymentdetails.payer.payer_info.phone;
            uso.Email = getpaymentdetails.payer.payer_info.email;
            uso.TimeOrder = DateTime.Parse(getpaymentdetails.create_time);
         //   uso.City1 = getpaymentdetails.payer.payer_info.shipping_address.city;
         //   uso.Address1 = getpaymentdetails.payer.payer_info.shipping_address.line1;
         //   uso.City2 = "";
        //    uso.Address2 = getpaymentdetails.payer.payer_info.shipping_address.line2;
            uso.Payment = "0";
            uso.StatusPayment = true;
            uso.Country = getpaymentdetails.payer.payer_info.country_code;
            uso.DeliveryStatus = "0";
            uso.TotalPrice = Convert.ToString(Session["totalprice"]);
            db.UserOrders.InsertOnSubmit(uso);
            db.SubmitChanges();
            //Send email notification users order
            var fromEmail = new MailAddress("email.tinhgiot@gmail.com", "Tinh Giot");
            var toEmail = new MailAddress("info@tinhgiot.com");
            var fromEmailPassword = "tinhgiot123"; // Replace with actual password

            string subject = "";
            string body = "";
            subject = "Thông báo đặt hàng Tinh Giọt";
            body = "Tên đặt hàng: " + uso.Name + "<br/>Số điện thoại: " + uso.PhoneNumber + "<br/>Email:" + uso.Email + "<br/>Địa chỉ: " +  "(" + ")" + "<br/>Thời gian đặt hàng: " + uso.TimeOrder +
                   "<br/>Phương thức thanh toán: Paypal (Đã thanh toán)" +
                   "<br/>====================================<br/><table>" + "<thead><tr><th>Số lượng</th><th>Tên sản phẩm</th></tr></thead>" +
                   "<tbody>" + TGE + "</tbody>" + "</table>" + "<br/>====================================<br/>Tổng tiền: " + uso.TotalPrice + " vnđ <br/>";
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


            Session["totalprice"] = null;
            Session["CartSession"] = null; //Clear Session cart item
            return View("SuccessView");
        }

        public ActionResult ProcessPaypalPayment()
        {
            APIContext apiContext = PayPalConfiguration.GetAPIContext();
            bool isPaidSucess = false;
            string PaymentID = "";
            string OrderID = Request.Cookies["OrderID"].Value;
            try
            {
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/CartItem/PaymentWithPayPal?";
                    var guid = Convert.ToString((new Random()).Next(100000));
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;

                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = lnk.href;
                        }
                    }

                    Session.Add(guid, createdPayment.id);

                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        isPaidSucess = false;
                    }
                    else
                    {
                        isPaidSucess = true;
                        PaymentID = executedPayment.id;

                        Response.Cookies["OrderID"].Expires = DateTime.Now.AddDays(-1);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error" + ex.Message);
                isPaidSucess = false;
            }
            //Save Order to database
            var listCarts = (List<CartItems>)Session["CartSession"];
            var TGE = "";
            // get product order list
            var listProductOrder = db.ProductOrders.Where(x => x.OrderID == OrderID);
            foreach (var item in listProductOrder)
            {
                TGE = TGE + "<tr>" + "<td>" + item.Quantity + "</td>" + "<td>" + item.ProductName + "</td>" + "</tr>";
            }
         
            // updated user order as selected Paypal as payment type
            UserOrder uso = db.UserOrders.Where(x => x.OrderID == OrderID).FirstOrDefault();
            uso.PaymentID = PaymentID;
            uso.StatusPayment = isPaidSucess;
            db.SubmitChanges();
            // send an invoice
            SendInvoice(uso, TGE, "PAYPAL", isPaidSucess);

            if(isPaidSucess)
                return View("SuccessView");
            return View("FailureView");

        }

        private PayPal.Api.Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            string OrderID = Request.Cookies["OrderID"].Value;
            //Save Order to database
            var TGE = "";
            // get product order list
            var listProductOrder = db.ProductOrders.Where(x => x.OrderID == OrderID);


            List<CartItems> listCarts = (List<CartItems>)Session["CartSession"];
            var itemList = new ItemList() { items = new List<Item>() };
            foreach(var  item in listProductOrder)
            {
                itemList.items.Add(new Item()
                {
                    name = item.ProductName,
                    currency = "USD",
                    price = item.Price,
                    quantity = item.Quantity.ToString(),
                    sku = item.SKU
                }) ;
            }

            var payer = new Payer() { payment_method = "paypal" };

            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl,
                return_url = redirectUrl
            };

            var details = new Details()
            {
                tax = "1",
                shipping = "2",
                subtotal = listProductOrder.Sum(x => Convert.ToDecimal(x.Price) * x.Quantity).ToString(),
            };

            var amount = new Amount()
            {
                currency = "USD",
                total = (Convert.ToInt32(details.tax) + Convert.ToInt32(details.shipping) + Convert.ToInt32(details.subtotal)).ToString(),
                details = details
            };

            var transactionList = new List<Transaction>();
            transactionList.Add(new Transaction()
            {
                description = "Tinh Giot Paypal Transaction",
                invoice_number = DateTime.Now.ToString("ddMMyyy_hh_mm_ss"),
                amount = amount,
                item_list = itemList
            });

            this.payment = new PayPal.Api.Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            this.payment = this.payment.Create(apiContext);
            return this.payment;
        }

        private PayPal.Api.Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            this.payment = new PayPal.Api.Payment() { id = paymentId };
            return this.payment.Execute(apiContext, paymentExecution);
        }

        [HttpPost]
        //Get payment details
        public Payment GetPaymentDetails(string paymentID)
        {
            var apiContext = PayPalConfiguration.GetAPIContext();
            var payment = new Payment();
            try
            {
                payment = Payment.Get(apiContext, paymentID);
            }
            catch (PayPal.PaymentsException ex)
            {
                throw new Exception("Sorry there is an error getting the payment details. " + ex.Response);
            }
            return payment;
        }

        [HttpPost]
        public JsonResult Order(string Name, string Phone, string Email, string City1, string Address1, string City2, string Address2, string Note)
        {
            var listCarts = (List<CartItems>)Session["CartSession"];
            string OrderId = Convert.ToString((new Random()).Next(100000)) + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour;
            var GetUserID = Session["userid"].ToString();
            var OrderTime = DateTime.Now;
            var result = false;
            var TGE = "";
            foreach (var item in listCarts)
            {
                ProductOrder po = new ProductOrder();
                po.ProductID = Convert.ToString(item.ProductID);
                po.Quantity = item.Quantity;
                po.OrderID = OrderId;
                po.Price = item.Price;
                po.TimeOrder = OrderTime;
                po.UserID = GetUserID;
                po.ProductName = item.ProductName;
                Session["totalprice"] = Convert.ToInt32(Session["totalprice"]) + (item.Quantity * Convert.ToInt32(item.Price));
                db.ProductOrders.InsertOnSubmit(po);
                db.SubmitChanges();
                TGE = TGE + "<tr>"+ "<td>" + po.Quantity + "</td>"+ "<td>" + po.ProductName + "</td>" + "</tr>";
            }
            UserOrder uso = new UserOrder();
            uso.OrderID = OrderId;
            uso.UserID = GetUserID;
            uso.Name = Name;
            uso.PhoneNumber = Phone;
            uso.Email = Email;
            uso.Note = Note;
            uso.TimeOrder = OrderTime;
            uso.Payment = "COD";
            uso.StatusPayment = false;
            uso.Country = "VN";
            uso.DeliveryStatus = "0";
            uso.TotalPrice = Convert.ToString(Session["totalprice"]);
            db.UserOrders.InsertOnSubmit(uso);
            db.SubmitChanges();

            SendInvoice(uso, TGE, "COD", false);
            //Clear All Session
            Session["totalprice"] = null;
            Session["CartSession"] = null; //Clear Session cart item
            result = true;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public void SendInvoice(UserOrder oUserOrder, string detailOrder, string PaymentType, bool isPaid)
        {
            //Send email notification users order
       //     var fromEmail = new MailAddress("noreply@tinhgiot.com", "Tinh Giot");
      //      var toEmail = new MailAddress(oUserOrder.Email);
        //    var fromEmailPassword = "aucfyehzgY0dqmi-wobBvjk4prn@xs"; // Replace with actual password

            string subject = "";
            string body = "";
            subject =Resources.ProcessCheckout.EmailSubject;
            body = Resources.ProcessCheckout.Fullname + oUserOrder.Name + "<br/>"+ Resources.ProcessCheckout.Phonenumber  + oUserOrder.PhoneNumber + "<br/> Email:" + oUserOrder.Email + "<br/>" + Resources.ProcessCheckout.Address + "(" + ")" + "<br/>" + Resources.ProcessCheckout.DateTimeOrder + oUserOrder.TimeOrder +
                string.Format("<br/>Phương thức thanh toán: {0} " +  (isPaid ?  Resources.ProcessCheckout.Paid: Resources.ProcessCheckout.Unpaid), PaymentType) +
                   "<br/>====================================<br/><table>" + "<thead><tr><th>Số lượng</th><th>Tên sản phẩm</th></tr></thead>" +
                   "<tbody>" + detailOrder + "</tbody>" + "</table>" + "<br/>====================================<br/>Tổng tiền: " + oUserOrder.TotalPrice + " vnđ <br/>";


               string[] to = {oUserOrder.Email};
            SendingEmail email = new SendingEmail(ConfigFile.EmailNoreply, to, null, subject, body, null);
            email.Send();

        }
        public List<CartItems> getItems()
        {
            return null;
        }
        
        private void UpdateItemQuantity(CartItems oItems)
        {
            var oitems = db.CartItem.Where(x => (x.ProductID == oItems.ProductID) && (x.BuyerID == oItems.BuyerID));
            foreach(var item in oitems)
            {
                item.Quantity = oItems.Quantity;
            }
            db.SubmitChanges();
        }
        private void AddNewItem(CartItems oItems)
        {
            CartItem oitem = new CartItem();

            oitem.ProductID = oItems.ProductID;
            oitem.BuyerID = oItems.BuyerID;
            oitem.ProductName = oItems.ProductName;
            oitem.ProductName_EN = oItems.ProductName_EN;
            oitem.ProductName_JP = oItems.ProductName_JP;
            oitem.Quantity = oItems.Quantity;
            oitem.Price = oItems.Price;
            oitem.Image = oItems.Image;
            oitem.SKU = oItems.SKU;
            oitem.TimeStamp = DateTime.Now;
            db.CartItem.InsertOnSubmit(oitem);
            db.SubmitChanges();
        }

        private  void  DeleteItem(CartItems oItems)
        {
            try
            {
                var oitems = db.CartItem.Where(x => (x.ProductID == oItems.ProductID) && (x.BuyerID == oItems.BuyerID));
                db.CartItem.DeleteAllOnSubmit(oitems);
                db.SubmitChanges();
            }catch(Exception ex)
            {
              
            }
  
        
        }

        private void DeleteAllItems(string BuyerID)
        {
            var oitems = db.CartItem.Where(x => x.BuyerID == BuyerID);
            db.CartItem.DeleteAllOnSubmit(oitems);
            db.SubmitChanges();
        }

    }

}