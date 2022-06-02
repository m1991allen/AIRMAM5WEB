using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AIRMAM5.Controllers
{
    /// <summary>
    /// 前端wepack 引用樣板
    /// </summary>
    public class BundleController : Controller
    {

        /// <summary>
        /// Bundle JS引用樣板
        /// </summary>
        /// <returns></returns>
        public ActionResult _JsTemplate()
        {
            return PartialView();
        }
        /// <summary>
        /// Bundle CSS引用樣板
        /// </summary>
        /// <returns></returns>
        public ActionResult _CssTemplate()
        {
            return PartialView();
        }

    }
}