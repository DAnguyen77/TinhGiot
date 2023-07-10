using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BaseClasses;

namespace tinhGiot.Controllers
{
    public class OrderController : ApiController
    {
        dbTinhGiotDataContext db = new dbTinhGiotDataContext();
        List<UserOrder> order = new List<UserOrder>();
        List<OrderProduct> orderproduct = new List<OrderProduct>();
        public IEnumerable<UserOrder> GetAllOrder()
        {
            GetOrder();
            return order;
        }
        public IEnumerable<OrderProduct> GetAllOrderProduct()
        {
            GetOrderProduct();
            return orderproduct;
        }
        private void GetOrder()
        {
            order = db.UserOrders.ToList();
        }
        private void GetOrderProduct()
        {
            var orderproducts = db.ProductOrders.ToList();

            foreach (var item in orderproducts)
            {
                orderproduct.Add(new OrderProduct
                {
                    ID = Convert.ToInt32(item.ID),
                    UserID = item.UserID,
                    OrderID = item.OrderID,
                    ProductID = item.ProductID,
                    Quantity = Convert.ToInt32(item.Quantity),
                    Price = item.Price,
                    ProductName = item.ProductName,
                    TimeOrder = Convert.ToDateTime(item.TimeOrder),
                });
            }
        }
        public UserOrder GetOrderbyId(string id)
        {
            if (order.Count() > 0)
            {
                return order.Find(p => p.OrderID == id);
            }
            else
            {
                GetOrder();
                return order.Find(p => p.OrderID == id);
            }
        }
        public OrderProduct GetOrderProductbyId(string id)
        {
            if (orderproduct.Count() > 0)
            {
                return orderproduct.Find(p => p.OrderID == id);
            }
            else
            {
                GetOrderProduct();
                return orderproduct.Find(p => p.OrderID == id);
            }
        }
        public IEnumerable<OrderProduct> GetAllOrderProductbyId(string id)
        {
            if (orderproduct.Count() > 0)
            {
                return orderproduct.Where(p => p.OrderID == id);
            }
            else
            {
                GetOrderProduct();
                var list = orderproduct.Where(p => p.OrderID == id);
                return list;
            }
        }
        public IEnumerable<UserOrder> DeleteOrder(string OrderID)
        {
            GetDeleteOrder(OrderID);
            return order;
        }
        private void GetDeleteOrder(string OrderID)
        {
            UserOrder or = db.UserOrders.SingleOrDefault(n => n.OrderID == OrderID);

            if (or != null)
            {
                db.UserOrders.DeleteOnSubmit(or);
                db.SubmitChanges();
            }
        }
        public IEnumerable<OrderProduct> DeleteOrderProduct(string OrderID)
        {
            GetDeleteOrderProduct(OrderID);
            return orderproduct;
        }
        private void GetDeleteOrderProduct(string OrderID)
        {
            List<ProductOrder> or = db.ProductOrders.Where(n => n.OrderID == OrderID).ToList() ;

            if (or != null)
            {
                for(int i = 0; i < or.Count; i++)
                {
                    db.ProductOrders.DeleteOnSubmit(or[i]);
                    db.SubmitChanges();
                }
                
            }
        }
        public IEnumerable<OrderProduct> GetHistoryProductOrderByUserID(string userid)
        {
            if (orderproduct.Count() > 0)
            {
                return orderproduct.Where(p => p.UserID == userid);
            }
            else
            {
                GetOrderProduct();
                var list = orderproduct.Where(p => p.UserID == userid);
                return list;
            }
        }

        public IEnumerable<UserOrder> GetHistoryOrderByUserID(string userid)
        {
            if (order.Count() > 0)
            {
                return order.Where(p => p.UserID == userid);
            }
            else
            {
                GetOrder();
                var list = order.Where(p => p.UserID == userid);
                return list;
            }
        }
    }
}
