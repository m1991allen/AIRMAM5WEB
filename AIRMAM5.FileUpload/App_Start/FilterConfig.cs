﻿using System.Web;
using System.Web.Mvc;

namespace AIRMAM5.FileUpload
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
