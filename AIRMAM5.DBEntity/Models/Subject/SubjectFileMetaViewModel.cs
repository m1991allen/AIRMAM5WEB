using AIRMAM5.DBEntity.Models.ArcPre;
using AIRMAM5.DBEntity.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Linq;

namespace AIRMAM5.DBEntity.Models.Subject
{
    /// <summary>
    /// (影音圖文)點選顯示頁: 點選左側檔案列表,右側顯示'媒體資料內容(+變動欄位)
    /// <para> 繼承參考 <see cref="ArcBasicMetaModel"/> 「檢索結果.基本資料頁model」。</para>
    /// </summary>
    /// <remarks> 配合預存增加欄位:  </remarks>
    public class SubjectFileMetaViewModel : ArcBasicMetaModel
    {
        //static CodeService _tbzCodeService = new CodeService();

        /// <summary>
        /// (影音圖文)點選顯示頁: 點選左側檔案列表,右側顯示'媒體資料內容(+變動欄位)
        /// <para> 繼承參考 <see cref="ArcBasicMetaModel"/> 「檢索結果.基本資料頁model」。</para>
        /// </summary>
        public SubjectFileMetaViewModel() { }

        #region >>> 欄位參數
        /// <summary>
        /// 分類: A聲音, D文件, P圖片, S主題, V影片 FileCategory 中文
        /// </summary>
        [Display(Name = "類別")]
        public string FileCategoryStr { get; set; } = string.Empty;

        /// <summary>
        /// 自訂欄位
        /// </summary>
        public List<ArcPreAttributeModel> ArcPreAttributes { get; set; } = new List<ArcPreAttributeModel>();

        /// <summary>
        /// 檔案機密 選單
        /// </summary>
        [Display(Name = "機密等級")]
        public List<SelectListItem> FileSecretList { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// 版權 選單
        /// </summary>
        /// <remarks> 20210909_ADDED </remarks>
        [Display(Name = "版權")]
        public List<SelectListItem> FileLicenseList { get; set; } = new List<SelectListItem>();
        #endregion

        /// <summary>
        /// (影音圖文)點選顯示頁: 點選左側檔案列表,右側顯示'媒體資料內容(+變動欄位), 資料型別<typeparamref name="T"/> 轉換處理
        /// </summary>
        /// <typeparam name="T">資料型別參數 如: 
        ///     <para> <see cref="spGET_ARC_VIDEO_Result"/>, <see cref="spGET_ARC_AUDIO_Result"/>, <see cref="spGET_ARC_DOC_Result"/>, <see cref="spGET_ARC_PHOTO_Result"/>
        ///     </para>
        /// </typeparam>
        /// <param name="fileCategory">檔案類別/分類 <see cref="Enums.FileTypeEnum"/></param>
        /// <returns> <see cref="SubjectFileMetaViewModel"/> </returns>
        public new SubjectFileMetaViewModel FormatConversion<T>(T m, Enums.FileTypeEnum fileCategory)
        {
            this.FileCategory = fileCategory.ToString();
            this.FileCategoryStr = Enums.GetEnums.GetDescriptionText<Enums.FileTypeEnum>(fileCategory.ToString());
            this.UserDateInfo = new Shared.TableUserDateByNameModel();

            var _properties = typeof(T).GetProperties();
            foreach (var p in _properties)
            {
                var _val = p.GetValue(m) ?? string.Empty;

                if (p.Name == "fsSUBJECT_ID") { this.fsSUBJECT_ID = _val.ToString(); }
                if (p.Name == "fsFILE_NO") { this.fsFILE_NO = _val.ToString(); }
                if (p.Name == "fsTITLE") { this.Title = _val.ToString(); }
                if (p.Name == "fsDESCRIPTION") { this.Description = _val.ToString(); }
                if (p.Name == "fsORI_FILE_NAME") this.OriginFileName = _val.ToString(); //+原始檔名
                if (p.Name == "fnFILE_SECRET")
                {
                    short.TryParse(_val.ToString(), out short v);
                    this.FileSecret = v;

                    CodeService _tbzCodeService = new CodeService();
                    this.FileSecretStr = _tbzCodeService.GetCodeName(Enums.TbzCodeIdEnum.FILESECRET, _val.ToString());
                }

                if (p.Name == "fsCREATED_BY") { this.UserDateInfo.CreatedBy = _val.ToString(); }
                if (p.Name == "fdCREATED_DATE")
                {
                    var cdt = DateTime.TryParse(_val.ToString(), out DateTime v);
                    this.UserDateInfo.CreatedDate = v;
                }
                if (p.Name == "fsCREATED_BY_NAME") { this.UserDateInfo.CreatedByName = _val.ToString(); }

                if (p.Name == "fsUPDATED_BY") { this.UserDateInfo.UpdatedBy = _val.ToString(); }
                if (p.Name == "fdUPDATED_DATE")
                {
                    var udt = DateTime.TryParse(_val.ToString(), out DateTime v);
                    if (string.IsNullOrEmpty(_val.ToString())) this.UserDateInfo.UpdatedDate = null;
                    else this.UserDateInfo.UpdatedDate = v;
                }
                if (p.Name == "fsUPDATED_BY_NAME") { this.UserDateInfo.UpdatedByName = _val.ToString(); }

                //20210831_ADDED_語音轉文字欄位
                if (p.Name == "fsS2T_CONTENT") { this.Voice2TextContent = _val.ToString(); }
                //20210909_ADDED)_版權欄位
                if (p.Name == "fsLICENSE") { this.LicenseCode = _val.ToString(); } 
                if (p.Name == "fsLICENSE_NAME") { this.LicenseStr = _val.ToString(); }
                if (p.Name == "fcIS_ALERT" || p.Name == "IS_ALERT")
                {
                    if (bool.TryParse(_val.ToString(), out bool chk)) { this.IsAlert = chk; }
                }
                if (p.Name == "fcIS_FORBID" || p.Name == "IS_FORBID")
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
                    if (DateTime.TryParse(_val.ToString(), out DateTime dt)) {
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
