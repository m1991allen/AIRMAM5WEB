using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Models.Procedure;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Models.SubjExtend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Interface
{
    public interface ISubjectService
    {
        /// <summary>
        /// 主題檔資料 tbmSUBJECT
        /// </summary>
        /// <param name="subjid"></param>
        /// <param name="dirid"></param>
        /// <returns></returns>
        tbmSUBJECT GetBy(string subjid = null, long dirid = 0);

        /// <summary>
        /// 多筆主題編號 取 tbmSUBJECT 資料
        /// </summary>
        /// <param name="fnos"></param>
        /// <returns></returns>
        List<tbmSUBJECT> GetByIds(string[] subjids);

        /// <summary>
        /// 取 主題編號fsSUBJ_ID 使用的"主題"樣版ID [fnTEMP_ID_SUBJECT]
        /// </summary>
        int GetSubjTemplateId(string subjid);

        /// <summary>
        /// 依 主題編號[fsSUBJ_ID] 取回 tbmSUBJECT 自訂欄位 【dbo.spGET_SUBJECT_ATTRIBUTE】
        /// </summary>
        /// <param name="subjid"></param>
        /// <returns></returns>
        List<spGET_SUBJECT_ATTRIBUTE_Result> GetSubjAttribute(string subjid);

        #region ---------- CURD 【tbmSUBJECT】: 新 修 刪
        /// <summary>
        /// 新建 媒資主題 tbmSUBJECT: 【EF Create】
        /// </summary>
        /// <param name="rec"></param>
        /// <param name="head">主題編號取號時指定 編號前段的值。預設空值=當日日期，格式: yyyyMMdd。</param>
        /// <returns></returns>
        VerifyResult CreateBy(tbmSUBJECT rec, string head = "");

        /// <summary>
        /// 表單Form - 修改更新 媒資主題 tbmSUBJECT: 【EF Update】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        VerifyResult UpdateBy(FormCollection form, string updateby);

        /// <summary>
        /// 檔案搬移 - '單筆/多筆批次'修改更新 媒資主題 tbmSUBJECT: 【EF Update】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        VerifyResult UpdateByMove(List<tbmSUBJECT> rec);

        /// <summary>
        /// 刪除 媒資主題 tbmSUBJECT: 【EF Update】
        /// </summary>
        /// <param name="subjid"></param>
        /// <param name="deleteby"></param>
        /// <returns></returns>
        VerifyResult DeleteBy(string subjid, string deleteby);
        #endregion

        #region >>> 擴充功能:新聞文稿,公文系統
        /// <summary>
        /// 擴充功能{新聞文稿/公文系統} 動態的查詢欄位 
        /// </summary>
        /// <param name="type">對應表類型 dbo.tbmCOLUMN_MAPPING.fsTYPE, i.g.INEWS、CONTRACT </param>
        /// <returns></returns>
        List<SubjExtendColModel> SubjExtendQryCols(string type);

        /// <summary>
        /// 擴充功能{新聞文稿} 選取後更新自訂欄位存檔  dbo.spUPDATE_INEWS
        /// </summary>
        /// <param name="upd">選取存檔參數 (預存參數) </param>
        /// <returns></returns>
        VerifyResult UpdateINews(Update_INews_Param<string> upd);
        #endregion
    }
}
