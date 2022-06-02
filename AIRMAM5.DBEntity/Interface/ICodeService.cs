using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Models.CodeSet;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Interface
{
    public interface ICodeService
    {
        /// <summary>
        /// 取 編輯子代碼檔 View Modal資料
        /// </summary>
        /// <param name="codeid">代碼主表代碼 fsCODE_ID </param>
        /// <returns></returns>
        CodeEditViewModel GetEditById(string codeid);

        #region 【檢查判斷】
        /// <summary>
        /// 檢查 主檔代碼[fsCODE_ID]是否被使用 From dbo.tbmTEMPLATE_FIELDS 【dbo.spGET_CODE_CUSTOM_USED】
        /// </summary>
        /// <param name="codeid">代碼主表代碼 fsCODE_ID </param>
        /// <param name="code">子代碼 fsCODE (預設空值 </param>
        /// <returns></returns>
        bool ChkCodeIsUsedFromTemplateFields(string codeid, string code);

        /// <summary>
        /// 檢查 子代碼是否存在
        /// </summary>
        /// <returns></returns>
        bool ChkCodeIsHad(string codeid, string code);
        #endregion

        #region 【tbzCODE_SET】 代碼主表 QUERY/GET
        /// <summary>
        /// 代碼主表id 取 [tbzCODE_SET]資料
        /// </summary>
        /// <param name="codeid"></param>
        /// <returns></returns>
        tbzCODE_SET GetCodeSetById(string codeid);

        /// <summary>
        /// 取 代碼主表[tbzCODE_SET]: EXECUTE dbo.spGET_CODE_SET. (參數預設為空白,傳回全部資料)
        /// </summary>
        /// <param name="codeid">代碼主表代碼 fsCODE_ID </param>
        /// <param name="title">代碼主表標題 fsTITLE </param>
        /// <param name="type">代碼主表分類 fsTYPE </param>
        /// <returns></returns>
        List<spGET_CODE_SET_Result> GetCodeMaster(string codeid = "", string title = "", string type = "");
        #endregion

        #region 【tbzCODE】 子代碼(明細) QUERY/GET
        /// <summary>
        /// 代碼主表id+ 子代碼 取 [tbzCODE]資料
        /// </summary>
        /// <param name="codeid">主代碼 fsCODE_ID </param>
        /// <param name="code">子代碼 fsCODE (選填)</param>
        /// <returns></returns>
        List<tbzCODE> GetCodeById(string codeid, string code = "");

        /// <summary>
        /// 取 子代碼檔[tbzCODE] : Execute dbo.spGET_CODE. (參數預設為空白,傳回全部資料)
        /// </summary>
        /// <param name="codeid">代碼主檔代碼 fsCODE_ID </param>
        /// <param name="code">子代碼 fsCODE </param>
        /// <param name="name">子代碼名稱 fsNAME </param>
        /// <returns></returns>
        List<spGET_CODE_Result> GetCodeDetail(string codeid = "", string code = "", string name = "");

        /// <summary>
        /// 取 指定代碼主表代碼對應的 子代碼名稱 tbzCODE.fsNAME
        /// </summary>
        /// <param name="codeid">代碼主檔代碼 fsCODE_ID </param>
        /// <param name="code">子代碼 fsCODE </param>
        /// <returns></returns>
        string GetCodeName(TbzCodeIdEnum codeid, string code);
        #endregion

        #region ---------- DropDownList【SelectListItem】
        /// <summary>
        /// 布林選項下拉清單 (True/False , 是/否, 有效/無效,....)
        /// </summary>
        /// <param name="s"> 指定顯示的中文. True/False , 是/否, 有效/無效, ..... </param>
        /// <returns></returns>
        List<SelectListItem> GetBoolItemList(string[] s);
        /// <summary>
        /// 指定主表CodeID 取明細代碼列表 
        /// </summary>
        /// <param name="codeid">主代碼</param>
        /// <param name="isenabled">是否啟用 </param>
        /// <param name="showcode">預設顯示"fsCODE fsNAEM"、True顯示"fsCODE fsNAEM"、False顯示"fsNAME" </param>
        /// <param name="hadall">是否顯示"全部"項目 預設false不顯示 </param>
        /// <returns></returns>
        List<SelectListItem> GetCodeItemList(string codeid, bool? isenabled = null, bool showcode = false, bool hadall = false);
        /// <summary>
        /// [fsCODE_ID] 傳入多筆[fsCODE] 符合者選取(selected) 回傳 SelectListItem
        /// </summary>
        /// <param name="codeid">明細代碼ID [fsCODE_ID]</param>
        /// <param name="codes">多筆代碼fsCode (多筆;分隔)</param>
        /// <returns></returns>
        List<SelectListItem> CodeListItemSelected(string codeid, string codes);
        /// <summary>
        /// [fsCODE_ID] 指定的[fsCODE]內容  回傳 SelectListItem
        /// </summary>
        /// <param name="codeid">明細代碼ID [fsCODE_ID]</param>
        /// <param name="codes">多筆代碼fsCode (多筆;分隔)</param>
        /// <returns></returns>
        List<SelectListItem> SpecifyCodeListItem(string codeid, string codes);
        /// <summary>
        /// 主代碼+子代碼 資料清單
        /// </summary>
        /// <param name="type">系統代碼S/自訂代碼C : CodeSetTypeEnum </param>
        /// <param name="isenabled">是否啟用, 預設=null </param>
        /// <param name="showcode">預設=true 顯示"fsTITLE(fsCODE_ID)"、True顯示"fsTITLE(fsCODE_ID)"、False顯示"fsTITLE" </param>
        /// <param name="hadall">是否顯示"全部"項目 預設false不顯示 </param>
        /// <returns></returns>
        List<MainSubCodeListModel> GetMainSubList(string type, bool? isenabled = null, bool showcode = true, bool hadall = false);
        #endregion

        #region 【tbzCODE_SET】 代碼主表: 新 修 刪
        /// <summary>
        /// 新建 代碼主表 tbzCODE_SET: 【EF Create】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        VerifyResult CreateCodeSet(tbzCODE_SET rec);
        /// <summary>
        /// 編輯 代碼主表 tbzCODE_SET : 【EF Create】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        VerifyResult UpdateCodeSet(tbzCODE_SET rec);
        /// <summary>
        /// 刪除 代碼主表 tbzCODE_SET : Execute dbo.spDELETE_CODE_SET
        /// </summary>
        /// <param name="codeid"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        VerifyResult DeleteCodeSet(string codeid, string username);
        #endregion

        #region 【tbzCODE】 子代碼(明細): 新 修 刪
        /// <summary>
        /// 新建 子代碼 tbzCODE: 【EF Create】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        VerifyResult CreateCodeDet(tbzCODE rec);
        /// <summary>
        /// 修改 子代碼 tbzCODE: 【EF Update】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        VerifyResult UpdateCodeDet(tbzCODE rec);
        /// <summary>
        /// 刪除 子代碼 tbzCODE: 【EF Delete】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        VerifyResult DeleteCodeDet(CodeIdsModel param);
        #endregion

    }
}
