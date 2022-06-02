using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Models.Subject;
using AIRMAM5.DBEntity.Services;
using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models
{
    /// <summary>
    /// 檢索結果: 基本資料(不包括自訂欄位) 繼承參考 <see cref="SubjFileNoModel"/>
    /// <para>　　欄位：fsSUBJECT_ID，fsFILE_NO, fsTITLE, fsDESCRIPTION, fdCREATED_DATE, fsCREATED_USER, fdUPDATED_DATE, fsUPDATED_USER </para>
    /// </summary>
    /// <remarks> 媒資檔案基本資料參數定義_base <br />
    ///   20210831_ADDED_資料欄位[fsS2T_CONTENT] 語音轉文字 <br />
    ///   20210908_ADDED_配合預存增加欄位[fsLICENSE]版權、[fsLICENSE_NAME]版權名稱、[fcIS_ALERT]、[fcIS_FORBID]、[fcLICENSE_MESSAGE]、 <br />
    /// </remarks>
    public class ArcBasicMetaModel : SubjFileNoModel
    {
        static CodeService _tbzCodeService = new CodeService();
        /// <summary>
        /// 檢索結果: 基本資料(不包括自訂欄位) 繼承參考 <see cref="SubjFileNoModel"/>
        /// <para>　　欄位：fsSUBJECT_ID，fsFILE_NO, fsTITLE, fsDESCRIPTION, fdCREATED_DATE, fsCREATED_USER, fdUPDATED_DATE, fsUPDATED_USER </para>
        /// </summary>
        public ArcBasicMetaModel() { }

        #region >>>>> 欄位參數 
        /// <summary>
        /// 分類: A聲音, D文件, P圖片, S主題, V影片 FileCategory
        /// </summary>
        [Display(Name = "類別")]
        public string FileCategory { get; set; } = string.Empty;
        /// <summary>
        /// 標題
        /// </summary>
        [Display(Name = "標題")]
        public string Title { set; get; } = string.Empty;
        /// <summary>
        /// 描述
        /// </summary>
        [Display(Name = "描述")]
        public string Description { set; get; } = string.Empty;
        /// <summary>
        /// 檔案機密
        /// </summary>
        [Display(Name = "機密等級")]
        public short FileSecret { set; get; } = 0;
        /// <summary>
        /// 檔案機密 中文
        /// </summary>
        [Display(Name = "機密等級")]
        public string FileSecretStr { set; get; } = _tbzCodeService.GetCodeName(TbzCodeIdEnum.FILESECRET, "0");

        /// <summary>
        /// 擷取文字 (只有類別=D文件 才有此欄位內容)
        /// </summary>
        [Display(Name = "擷取文字")]
        public string Content { set; get; } = string.Empty;

        /// <summary>
        /// 資料表 建立/最後異動 帳號+日期+帳號顯示名稱 
        /// </summary>
        public TableUserDateByNameModel UserDateInfo { get; set; } = new TableUserDateByNameModel();

        /// <summary>
        /// 檔案上傳原始檔名
        /// </summary>
        [Display(Name = "原始檔名")]
        public string OriginFileName { get; set; } = string.Empty;

        /// <summary>
        /// 影片/聲音檔案 語音轉文字欄位 [fsS2T_CONTENT]
        /// </summary>
        /// <remarks> Added_20210831 </remarks>
        [Display(Name = "語音文字")]
        public string Voice2TextContent { get; set; } = string.Empty;

        /// <summary>
        /// 版權 [fsLICENSE] 搭配'版權代碼表 dbo.[tbmLICENSE]'
        /// </summary>
        /// <remarks> Added_20210909 </remarks>
        [Display(Name = "版權")]
        public string LicenseCode { get; set; } = string.Empty;
        /// <summary>
        /// 版權 LicenseCode中文
        /// </summary>
        /// <remarks> Added_20210909 </remarks>
        [Display(Name = "版權")]
        public string LicenseStr { get; set; } = string.Empty;
        /// <summary>
        /// 版權.是否調用提醒
        /// </summary>
        /// <remarks> Added_20210914 </remarks>
        [Display(Name = "是否調用提醒")]
        public bool IsAlert { get; set; }
        /// <summary>
        /// 版權.是否調用禁止
        /// </summary>
        /// <remarks> Added_20210914 </remarks>
        [Display(Name = "是否調用禁止")]
        public bool IsForBid { get; set; }
        /// <summary>
        /// 版權.提醒訊息內容
        /// </summary>
        /// <remarks> Added_20210914 </remarks>
        [Display(Name = "提醒訊息")]
        public string LicenseMessage { get; set; } = string.Empty;
        /// <summary>
        /// 版權.是否到期
        /// </summary>
        /// <remarks> Added_20211005 </remarks>
        [Display(Name = "是否到期")]
        public bool IsExpired { get; set; }
        /// <summary>
        /// 版權.授權到期日期
        /// </summary>
        /// <remarks> Added_20211005 </remarks>
        [Display(Name = "授權到期日")]
        public string LicenseEndDate { get; set; }

        /// <summary>
        /// 自訂標籤 HashTag
        /// </summary>
        /// <remarks> Added_20211122 </remarks>
        [Display(Name = "自訂標籤")]
        public string HashTag { get; set; }
        /// <summary>
        /// 檔案的主題目錄路徑
        /// </summary>
        /// <remarks> Added_20211122、預存欄位[C_sSUBJ_PATH] </remarks>
        [Display(Name = "目錄位置")]
        public string FileSubjPath { get; set; }
        #endregion

        /* marked_&_modified_20211006 */
        /// <summary>
        /// 檢索結果: 基本資料(不包括自訂欄位), 資料型別<typeparamref name="T"/> 轉換處理
        /// </summary>
        /// <typeparam name="T">資料型別參數 如:  
        ///     <para>1.媒資檔案.預存_Result: <see cref="spGET_ARC_VIDEO_Result"/>, <see cref="spGET_ARC_AUDIO_Result"/>, <see cref="spGET_ARC_PHOTO_Result"/>, <see cref="spGET_ARC_DOC_Result"/> </para>
        ///     <para>2.刪除紀錄.預存_Result: <see cref="sp_t_GET_ARC_VIDEO_Result"/>, <see cref="sp_t_GET_ARC_AUDIO_Result"/>, <see cref="sp_t_GET_ARC_PHOTO_Result"/>, <see cref="sp_t_GET_ARC_AUDIO_Result"/> </para>
        /// </typeparam>
        /// <param name="m">資料來源 </param>
        /// <param name="fileCategory">媒資檔案類別: V,A,P,D  <see cref="FileTypeEnum"/></param>
        /// <returns></returns>
        public ArcBasicMetaModel FormatConversion<T>(T m, FileTypeEnum fileCategory)
        {
            this.FileCategory = fileCategory.ToString();
            this.UserDateInfo = new TableUserDateByNameModel();

            var _properties = typeof(T).GetProperties();
            foreach (var p in _properties)
            {
                var _val = p.GetValue(m) ?? string.Empty;

                if (p.Name == "fsSUBJECT_ID") { this.fsSUBJECT_ID = _val.ToString(); }
                if (p.Name == "fsFILE_NO") { this.fsFILE_NO = _val.ToString(); }
                if (p.Name == "fsTITLE") this.Title = _val.ToString();
                if (p.Name == "fsDESCRIPTION") { this.Description = _val.ToString(); }
                if (p.Name == "fsORI_FILE_NAME") OriginFileName = _val.ToString(); //+原始檔名
                if (p.Name == "fnFILE_SECRET")
                {
                    short.TryParse(_val.ToString(), out short _gval);
                    this.FileSecret = _gval;
                    FileSecretStr = _tbzCodeService.GetCodeName(TbzCodeIdEnum.FILESECRET, _val.ToString());
                }
                //擷取文字 (只有類別=D文件 才有此欄位內容)
                if (p.Name == "fsCONTENT") { this.Content = fileCategory == FileTypeEnum.D ? _val.ToString() : string.Empty; }

                if (p.Name == "fsCREATED_BY") { this.UserDateInfo.CreatedBy = _val.ToString(); }
                if (p.Name == "fdCREATED_DATE")
                {
                    DateTime.TryParse(_val.ToString(), out DateTime _crdt);
                    this.UserDateInfo.CreatedDate = _crdt;
                }
                if (p.Name == "fsCREATED_BY_NAME") { this.UserDateInfo.CreatedByName = _val.ToString(); }
                if (p.Name == "fsUPDATED_BY") { this.UserDateInfo.UpdatedBy = _val.ToString(); }
                if (p.Name == "fdUPDATED_DATE")
                {
                    DateTime.TryParse(_val.ToString(), out DateTime _updt);
                    if (string.IsNullOrEmpty(_val.ToString())) this.UserDateInfo.UpdatedDate = null;
                    else this.UserDateInfo.UpdatedDate = _updt;
                }
                if (p.Name == "fsUPDATED_BY_NAME") { this.UserDateInfo.UpdatedByName = _val.ToString(); }
                //20210831_ADDED_語音轉文字欄位
                if (p.Name == "fsS2T_CONTENT") { this.Voice2TextContent = _val.ToString(); }
                //20210909_ADDED)_版權欄位
                if (p.Name == "fsLICENSE") { this.LicenseCode = _val.ToString(); }
                if (p.Name == "fsLICENSE_NAME") { this.LicenseStr = _val.ToString(); }
                // 版權.是否調用提醒、是否調用禁止、提醒訊息
                if (p.Name == "fcIS_ALERT" || p.Name == "IS_ALERT" || p.Name == "IsAlert")
                {
                    if (bool.TryParse(_val.ToString(), out bool chk)) { this.IsAlert = chk; }
                }
                if (p.Name == "fcIS_FORBID" || p.Name == "IS_FORBID" || p.Name == "IsForBid")
                {
                    if (bool.TryParse(_val.ToString(), out bool chk)) { this.IsForBid = chk; }
                }
                if (p.Name == "fsLICENSE_MESSAGE" || p.Name == "MESSAGE") { this.LicenseMessage = _val.ToString() ?? string.Empty; }
                //20211005_ADDED)_版權授權日期
                if (p.Name == "fcIS_LICENSE_EXPIRED" || p.Name == "IS_EXPIRED" || p.Name == "IsExpired")
                {
                    if (bool.TryParse(_val.ToString(), out bool chk)) { this.IsExpired = chk; }
                }
                if (p.Name == "fdENDDATE" || p.Name == "LicenseEndDate")
                {
                    if (DateTime.TryParse(_val.ToString(), out DateTime dt))
                    {
                        this.LicenseEndDate = string.Format($"{dt:yyyy/MM/dd}");
                    }
                }
                //20211122_ADDED)_自訂標籤
                if (p.Name == "fsHASH_TAG" || p.Name.ToUpper() == "HASHTAG") { this.HashTag = _val.ToString(); }
                //20220328_ADDED)_目錄位置
                if (p.Name == "C_sSUBJ_PATH" || p.Name.ToUpper() == "FILESUBJPATH") { this.FileSubjPath = _val.ToString(); }
            }

            return this;
        }
    }

}
