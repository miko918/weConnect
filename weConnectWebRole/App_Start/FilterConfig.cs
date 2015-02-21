using System.Web;
using System.Web.Mvc;

namespace com.web.server.weconnect.weconnectwebrole
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}