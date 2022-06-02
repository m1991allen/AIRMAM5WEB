using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AIRMAM5.Controllers
{
    public class ErrorController : Controller
    {
        /// <summary>
        /// 404錯誤(找不到資源)
        /// </summary>
        /// <returns></returns>
        public ActionResult NotFound()
        {
            return View();
        }
        /// <summary>
        /// 500錯誤(系統錯誤)
        /// </summary>
        /// <returns></returns>
        public ActionResult ServerError() {
            return View();
        }
        /// <summary>
        /// 401,403錯誤(沒有權限)
        /// </summary>
        /// <returns></returns>
        public ActionResult Forbidden() {
            return View("Forbidden");
        }
        /// <summary>
        /// 沒有權限的燈箱
        /// </summary>
        public ActionResult NoAuthModal(string id) {
            return PartialView("NoAuthModal",id);
        }
        public ActionResult NotSupportBrowser() {
            return View("NotSupportBrowser");
        }
    }
}