using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Search;
using System;

namespace AIRMAM5.DBEntity.Models.SearchResponse
{
    /// <summary>
    /// 檢索結果列表(LIST)資料內容: 預存回傳欄位。繼承參考 <see cref="SearchMetaResponseModel"/>
    /// <para>　　欄位：fsFILE_NO, fsMATCH, fsSUBJECT_ID, Title, SubjectTitle, CreateDate, FileType, Duration, HeadFrame, SearchType  </para>
    /// </summary>
    /// <remarks> 20210908_ADDED_配合預存增加欄位[fsLICENSE]版權、[fsLICENSE_NAME]版權名稱、[fcIS_ALERT]、[fcIS_FORBID]、[fcLICENSE_MESSAGE]、 <br />
    ///   </remarks>
    public class GetArcSearchResult : SearchMetaResponseModel
    {
        /// <summary>
        /// 檢索結果列表(LIST)資料內容
        /// </summary>
        public GetArcSearchResult() { }
        
        #region >>>>>欄位參數
        /// <summary>
        /// 主題編號 [fsSUBJECT_ID]
        /// </summary>
        public string fsSUBJECT_ID { set; get; } = string.Empty;

        /// <summary>
        /// 檔案標題 [fsTITLE]
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 主題標題 [fsSUBJECT_TITLE]
        /// </summary>
        public string SubjectTitle { get; set; } = string.Empty;

        /// <summary>
        /// 建立日期 [fdCREATED_DATE] (ex: 2019/11/20)
        /// </summary>
        public string CreateDate { get; set; } = string.Empty;

        /// <summary>
        /// 檔案類型 [fsFILE_TYPE] (ex: doc,JPG,ppt,....)
        /// </summary>
        public string FileType { get; set; } = string.Empty;

        /// <summary>
        /// {影/音}時間表示 [fdDURATION] (ex: 00:13:05;32, 00:03:29;63 )
        /// </summary>
        public string Duration { get; set; } = string.Empty;

        /// <summary>
        /// 代表圖 [fsHEAD_FRAME]
        /// </summary>
        public string HeadFrame { get; set; } = string.Empty;

        /// <summary>
        /// 檢索型別: V,A,P,D (SearchTypeEnum 字串)
        /// </summary>
        public string SearchType { get; set; }

        /// <summary>
        /// 檔案TSM狀態: 0(檔案在磁帶);1(檔案在磁碟);2(檔案不存在);3(處理中) => 通常為 0 或 1 
        /// <para>　　已改於前端再取TSM檔案狀態。</para>
        /// </summary>
        /// <remarks> 呼叫TSM查詢檔案狀態. -1為無資料。 </remarks>
        public int TSMFileStatus { get; set; } = -1;

        /// <summary>
        /// 檔案TSM狀態: 0(檔案在磁帶);1(檔案在磁碟);2(檔案不存在);3(處理中) => 通常為 0 或 1 
        /// <para>　　已改於前端再取TSM檔案狀態。</para>
        /// </summary>
        /// <remarks> 呼叫TSM查詢檔案狀態. -1為無資料。 </remarks>
        public string TSMFileStatusStr { get; set; } = string.Empty;

        /// <summary>
        /// fsDIRECTION 影音預覽方向: 橫向H、直向V
        /// </summary>
        public string fsDIRECTION { get; set; } = FileDirection.H.ToString();
        /// <summary>
        /// 轉檔工作狀態
        /// </summary>
        public string fsSTATUS { get; set; }
        /// <summary>
        /// 檔案寬
        /// </summary>
        public int fnWIDTH { get; set; } = 0;
        /// <summary>
        /// 檔案高
        /// </summary>
        public int fnHEIGHT { get; set; } = 0;

        /// <summary>
        /// 版權代碼 [fsLICENSE] 搭配'版權代碼表 dbo.[tbmLICENSE]'
        /// </summary>
        /// <remarks> Added_20210831 </remarks>
        public string LicenseCode { get; set; } = string.Empty;
        /// <summary>
        /// 版權 LicenseCode中文
        /// </summary>
        /// <remarks> Added_20210831 </remarks>
        public string LicenseStr { get; set; } = string.Empty;
        /// <summary>
        /// 版權.是否調用提醒
        /// </summary>
        /// <remarks> Added_20210914 </remarks>
        public bool IsAlert { get; set; }
        /// <summary>
        /// 版權.是否調用禁止
        /// </summary>
        /// <remarks> Added_20210914 </remarks>
        public bool IsForBid { get; set; }
        /// <summary>
        /// 版權.提醒訊息內容
        /// </summary>
        /// <remarks> Added_20210914 </remarks>
        public string LicenseMessage { get; set; } = string.Empty;

        /// <summary>
        /// 版權.是否到期
        /// </summary>
        /// <remarks> Added_20211005 </remarks>
        public bool IsExpired { get; set; }
        /// <summary>
        /// 版權.授權到期日期
        /// </summary>
        /// <remarks> Added_20211005 </remarks>
        public DateTime? LicenseEndDate { get; set; }
        /// <summary>
        /// 自訂標籤 HashTag
        /// </summary>
        /// <remarks> 資料值為字串, ^為分隔符號。</remarks>
        public Array HashTag { get; set; }
        #endregion

        /// <summary>
        /// 檢索結果列表(LIST)資料, 資料型別<typeparamref name="T"/> 轉換處理
        /// </summary>
        /// <typeparam name="T">資料型別參數 如: <see cref="spGET_ARC_VIDEO_SEARCH_BY_FILE_NOS_Result"/>,
        ///         <see cref="spGET_ARC_AUDIO_SEARCH_BY_FILE_NOS_Result"/>,
        ///         <see cref="spGET_ARC_PHOTO_SEARCH_BY_FILE_NOS_Result"/>,
        ///         <see cref="spGET_ARC_DOC_SEARCH_BY_FILE_NOS_Result"/>,
        /// </typeparam>
        /// <param name="m">資料來源 <typeparamref name="T"/></param>
        /// <param name="searchCategory"> 檢索媒資檔案分類 <see cref="SearchTypeEnum"/></param>
        public GetArcSearchResult FormatConversion<T>(T m, SearchTypeEnum searchCategory)
        {
            /*TIP: 媒資類型定義不同, 檢索後回傳給前端需轉為 VAPD */
            this.SearchType = searchCategory == SearchTypeEnum.Audio_DEV ? FileTypeEnum.A.ToString()
                        : (searchCategory == SearchTypeEnum.Video_DEV ? FileTypeEnum.V.ToString()
                            : (searchCategory == SearchTypeEnum.Photo_DEV ? FileTypeEnum.P.ToString()
                                : (searchCategory == SearchTypeEnum.Doc_DEV ? FileTypeEnum.D.ToString() : FileTypeEnum.V.ToString())));

            var _properties = typeof(T).GetProperties();

            foreach (var p in _properties)
            {
                var val = p.GetValue(m) ?? string.Empty;

                if (p.Name == "fsSUBJECT_ID") this.fsSUBJECT_ID = val.ToString();
                if (p.Name == "fsFILE_NO") { this.fsFILE_NO = val.ToString() ?? string.Empty; }

                /* 符合搜尋關鍵字 附近的文字 */
                //if (p.Name == "fsMATCH") { this.fsMATCH = string.Empty; }
                if (p.Name == "fsTITLE") { this.Title = val.ToString() ?? string.Empty; }
                if (p.Name == "fsSUBJECT_TITLE") { this.SubjectTitle = val.ToString() ?? string.Empty; }
                if (p.Name == "fdCREATED_DATE") { this.CreateDate = val.ToString() ?? string.Empty; }
                if (p.Name == "fsFILE_TYPE") { this.FileType = val.ToString() ?? string.Empty; }

                //Timecode
                if (p.Name == "fdDURATION") { this.Duration = val.ToString() ?? "00:00:00.00"; }
                if (p.Name == "fsHEAD_FRAME") { this.HeadFrame = val.ToString() ?? string.Empty; }

                /* [C_sFILE_PATH_L] 目前沒有使用 */
                if (p.Name == "fsSTATUS") this.fsSTATUS = val.ToString();
                if (p.Name == "fsDIRECTION") { this.fsDIRECTION = val.ToString() ?? FileDirection.H.ToString(); }
                if (p.Name == "fnWIDTH") this.fnWIDTH = int.Parse(val.ToString());
                if (p.Name == "fnHEIGHT") this.fnHEIGHT = int.Parse(val.ToString());

                //20210909_ADDED)_版權欄位
                if (p.Name == "fsLICENSE" || p.Name == "LicenseCode") { this.LicenseCode = val.ToString(); }
                if (p.Name == "fsLICENSE_NAME" || p.Name == "LicenseStr") { this.LicenseStr = val.ToString(); }
                if (p.Name == "fcIS_ALERT" || p.Name == "IS_ALERT" || p.Name == "IsAlert")
                {
                    if (bool.TryParse(val.ToString(), out bool chk)) { this.IsAlert = chk; }
                }
                if (p.Name == "fcIS_FORBID" || p.Name == "IS_FORBID" || p.Name == "IsForBid")
                {
                    if (bool.TryParse(val.ToString(), out bool chk)) { this.IsForBid = chk; }
                }
                if (p.Name == "fsLICENSE_MESSAGE" || p.Name == "MESSAGE" || p.Name == "LicenseMessage")
                {
                    this.LicenseMessage = val.ToString() ?? string.Empty;
                }
                //20211005_ADDED)_版權授權日期
                if (p.Name == "fcIS_LICENSE_EXPIRED" || p.Name == "IS_EXPIRED" || p.Name == "IsExpired")
                {
                    if (bool.TryParse(val.ToString(), out bool chk)) { this.IsExpired = chk; }
                }
                if (p.Name == "fdENDDATE" || p.Name == "LicenseEndDate")
                {
                    if (DateTime.TryParse(val.ToString(), out DateTime dt)) { this.LicenseEndDate = dt; }
                }
                //20211123_ADDED)_自訂標籤, TIP: ^為分隔符號。
                if (p.Name == "fsHASH_TAG")
                {
                    var b = val.ToString().Replace("#", "").Split(new char[] { '^' });
                    this.HashTag = b;
                }
            }

            return this;
        }
    }

}
