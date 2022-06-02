using System.Web;
using System.Web.Mvc;

namespace AIRMAM5.DocumentViewer
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
