using System.Web;
using System.Web.Mvc;

namespace EIDReaderWebWrapperV2
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
