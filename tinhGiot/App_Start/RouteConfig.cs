using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Text.RegularExpressions;

namespace tinhGiot
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Add Cart",
                url: "addtocart/{id}-{quantity}",
                defaults: new { controller = "CartItem", action = "AddItem", id = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "Search Results",
               url: "{lang}/SearchResults/{SearchText}",
               defaults: new {  controller = "Home", action = "SearchResult", SearchText = UrlParameter.Optional, lang = "vi" }
            );

            routes.MapRoute(
               name: "Checkout",
               url: "ProcessCheckout",
               defaults: new {  controller = "CartItem", action = "ProcessCheckout"}
            );
            routes.MapRoute(
               name: "Order Detail",
               url: "OrderDetail/{OrderID}",
               defaults: new { controller = "Home", action = "OrderDetail", OrderID = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "Product Detail",
               url: "{lang}/ProductDetail/{Description}/{id}-{categoryid}",
               defaults: new { controller = "Home", action = "Product_detail", id = UrlParameter.Optional, lang = UrlParameter.Optional }
            );
            routes.MapRoute(
                 name: "ProductList",
                 url: "{lang}/ProductList/{Category}/{id}",
                 defaults: new {  controller = "Home", action = "Products", id = UrlParameter.Optional, lang = UrlParameter.Optional }
             );
            routes.MapRoute(
                 name: "Articles",
                 url: "{lang}/Articles/{CategoryNews}/{id}",
                 defaults: new { controller = "Home", action = "News", id = UrlParameter.Optional, lang = UrlParameter.Optional }
             );
            routes.MapRoute(
               name: "Browse Article",
               url: "{lang}/Browse/{Description}/{id}-{categoryid}",
               defaults: new { controller = "Home", action = "News_detail", id = UrlParameter.Optional, lang = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Products",
                url: "Products/{action}/{Catid}",
                defaults: new { controller = "Admin", action = "Products", Catid = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "News",
                url: "NewsList/{action}/{Catid}",
                defaults: new { controller = "Admin", action = "News", Catid = UrlParameter.Optional}
            );

            routes.MapRoute(
                name: "Default",
                url: "{lang}/{controller}/{action}/{id}",
                defaults: new {controller = "Home", action = "Index", id = UrlParameter.Optional, lang = UrlParameter.Optional }
            );
        }
    }
}
