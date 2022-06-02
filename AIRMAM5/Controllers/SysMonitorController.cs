using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AIRMAM5.Controllers
{
    /// <summary>
    /// 資源監控
    /// </summary>
    public class SysMonitorController : Controller
    {
        /// <summary>
        /// 系統效能監控
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
    }
}