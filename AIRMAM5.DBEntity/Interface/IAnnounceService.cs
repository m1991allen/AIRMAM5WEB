using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Models.Announce;
using AIRMAM5.DBEntity.Models.Shared;
using System.Collections.Generic;

namespace AIRMAM5.DBEntity.Interface
{
    public interface IAnnounceService
    {
        /// <summary>
        /// fsANN_ID 取回公告資料
        /// </summary>
        /// <param name="id"> fsANN_ID </param>
        /// <returns></returns>
        tbmANNOUNCE GetById(long id);

        /// <summary>
        /// 公告資料是否存在
        /// </summary>
        /// <returns></returns>
        bool IsExists(long id);

        /// <summary>
        /// 系統公告首頁 資料 BY 使用者 【spGET_ANNOUNCE_BY_LOGIN_ID】
        /// </summary>
        /// <param name="username">使用者fsLOGIN_ID</param>
        /// <returns></returns>
        List<AnnouncePublicViewModel> GetAnnounceInfo(string username);

        /// <summary>
        /// 取得目前共用的公告 【spGET_ANNOUNCE_PUBLIC】
        /// </summary>
        List<spGET_ANNOUNCE_PUBLIC_Result> GetPublicAnnounce();

        /// <summary>
        /// 指定條件(ID,SDate,EDATE,TYPE)取回公告資料 【spGET_ANNOUNCE_BY_ANNID_DATES_TYPE】
        /// </summary>
        /// <param name="annid"></param>
        /// <param name="sdate"></param>
        /// <param name="edate"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        List<spGET_ANNOUNCE_BY_ANNID_DATES_TYPE_Result> GetBy4Parament(long annid = 0, string sdate = "", string edate = "", string type = "");

        /// <summary>
        /// 登入頁公告 【spGET_ANNOUNCE_BY_ANNID_DATES_TYPE】
        /// </summary>
        /// <returns></returns>
        List<spGET_ANNOUNCE_BY_ANNID_DATES_TYPE_Result> GetLoginAnn();

        #region 【dbo.tbmANNOUNCE】 新 修 刪
        /// <summary>
        /// Create 【EF 】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        VerifyResult Create(tbmANNOUNCE rec);
        /// <summary>
        /// Update  【EF 】
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        VerifyResult Update(tbmANNOUNCE rec);
        /// <summary>
        /// Delete  【spDELETE_ANNOUNCE-> EF 】
        /// </summary>
        /// <param name="annid">公告編號 fnANN_ID </param>
        /// <returns></returns>
        VerifyResult Delete(long annid);
        #endregion

    }
}
