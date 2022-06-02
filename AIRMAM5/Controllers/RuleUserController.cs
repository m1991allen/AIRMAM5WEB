using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AIRMAM5.Controllers
{
    /// <summary>
    /// 帳號規則管理
    /// </summary>
    public class RuleUserController : Controller
    {
        /// <summary>
        /// 首頁(帳號可使用規則的管理)
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 帳號規則查詢
        /// </summary>
        /// <returns></returns>
        public ActionResult _Search()
        {
            return PartialView();
        }
        /// <summary>
        /// 編輯使用者規則
        /// </summary>
        public ActionResult _Edit()
        {
            return PartialView();
        }
    }
}