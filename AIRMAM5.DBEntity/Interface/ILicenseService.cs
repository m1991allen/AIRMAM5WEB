using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Models.Shared;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Interface
{
    /// <summary>
    /// 版權資料 tbmLICENSE
    /// </summary>
    public interface ILicenseService
    {
        /// <summary>
        /// 版權代碼是否存在
        /// </summary>
        /// <param name="code">版權代碼 fsCODE </param>
        /// <returns></returns>
        bool IsExists(string code);

        /// <summary>
        /// 版權名稱是否存在
        /// </summary>
        /// <param name="name">版權名稱 fsNAME </param>
        /// <returns></returns>
        bool IsExistsByName(string name);

        /// <summary>
        /// 版權資料
        /// </summary>
        /// <param name="code">版權代碼 [fsCODE], 不指定取回全部資料。</param>
        /// <returns> 資料清單 <see cref="tbmLICENSE"/></returns>
        List<tbmLICENSE> GetBy(string code=null);

        /// <summary>
        /// 搜尋版權資料 (指定日期時, 搜尋[版權到期日期]大於等 enddt )
        /// </summary>
        /// <param name="name">版權名稱(模糊比對 </param>
        /// <param name="IsActive">是否指定啟用true/禁用false/全部null 資料, 預設是全部null。</param>
        /// <param name="enddt">授權結束日期(可為空值 </param>
        /// <remarks>  TIP_20211008_決議:不用特別限制版權條件 </remarks>
        /// <returns> 資料清單 <see cref="tbmLICENSE"/></returns>
        List<tbmLICENSE> SearchBy(string name, bool? IsActive = null, string enddt = "");

        /// <summary>
        /// 版權資料選單
        /// </summary>
        /// <param name="IsActive">是否指定啟用true/禁用false/全部null 資料, 預設是全部null。</param>
        /// <param name="IsOverdue">是否包括逾期資料, 預設為不包括逾期資料false。</param>
        /// <remarks>  TIP_20211008_決議:不用特別限制版權條件
        /// </remarks>
        /// <returns></returns>
        List<SelectListItem> GetListItem(bool? IsActive = null, bool IsOverdue = false);

        #region 【dbo.tbmANNOUNCE】 修改/新增
        /// <summary>
        /// 編輯存檔
        /// </summary>
        /// <typeparam name="T">傳入的資料格式model </typeparam>
        /// <param name="rec">傳入的資料內容 </param>
        /// <returns> <see cref="tbmLICENSE"/></returns>
        VerifyResult Update<T>(T rec); //VerifyResult Update(tbmLICENSE rec);

        /// <summary>
        /// 新增存檔
        /// </summary>
        /// <typeparam name="T">傳入的資料格式model </typeparam>
        /// <param name="rec">傳入的資料內容 </param>
        /// <returns> <see cref="tbmLICENSE"/></returns>
        VerifyResult Create<T>(T rec); //VerifyResult Create(tbmLICENSE rec);
        
        #endregion
    }
}
