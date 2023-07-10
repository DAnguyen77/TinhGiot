using System.Globalization;
using System.Threading;
using System.Web.Mvc;
namespace tinhGiot
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }

}
