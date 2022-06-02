using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.Utility.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Material
{
    /// <summary>
    /// 我的借調清單 Index。　繼承參考<see cref="MaterialIdModel"/>
    /// </summary>
    public class MaterialListModel : MaterialIdModel
    {
        /// <summary>
        /// 我的借調清單 model
        /// </summary>
        public MaterialListModel() { }

        #region >>>>> 欄位參數
        /// <summary>
        /// 檔案狀態 值: 0(檔案在磁帶);1(檔案在磁碟);2(檔案不存在);3(處理中) => 通常為 0 或 1 
        /// </summary>
        /// <remarks> 呼叫TSM查詢檔案狀態. -1為無資料。 </remarks>
        [Display(Name = "檔案狀態")]
        public int TSMFileStatus { get; set; } = -1;

        /// <summary>
        ///  檔案狀態 中文: 0(檔案在磁帶);1(檔案在磁碟);2(檔案不存在);3(處理中) => 通常為 0 或 1 
        /// </summary>
        public string TSMFileStatusStr { get; set; } = string.Empty;

        /// <summary>
        /// 檔案類型 [fsTYPE]= V,A,P,D
        /// </summary>
        [Display(Name = "媒資類型")]
        public string FileCategory { get; set; } = string.Empty;
        /// <summary>
        /// 檔案類型 V,A,P,D: 影,音,圖,文
        /// </summary>
        public string FileCategoryStr { get; set; } = string.Empty;

        /// <summary>
        /// 標題
        /// </summary>
        [Display(Name = "標題")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 調用日期(建立日期)
        /// </summary>
        [Display(Name = "調用日期")]
        public string CreatedDate { get; set; } //public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 影片長度(時長) Procedure_Result.[_sVIDEO_MAX_TIME] 如: 154.204
        /// </summary>
        [Display(Name = "時長")]
        public string VideoMaxTime { get; set; } = string.Empty;

        //起始時間, 結束時間, 截取時長
        /// <summary>
        /// 相關參數 [fsPARAMETER] 如: 部分調用起訖點(12.162;48.437;) ***分號;為分隔符號*** EX: 12.162;48.437; = 12.162秒~48.437秒。
        /// </summary>
        public string ParameterStr { get; set; } = string.Empty;

        /// <summary>
        /// 調用備註 [fsNOTE]
        /// </summary>
        [Display(Name = "調用備註")]
        public string MaterialNote { get; set; } = string.Empty;

        /// <summary>
        /// 檔案編號 [fsFILE_NO]
        /// </summary>
        [Display(Name = "檔案編號")]
        public string FileNo { get; set; } = string.Empty;

        /// <summary>
        /// 影片總長
        /// </summary>
        public string VideoMaxTimeStr { get; set; }

        /// <summary>
        /// (剪輯)起始時間
        /// </summary>
        public string MarkInTimeStr { get; set; }

        /// <summary>
        /// (剪輯)結束時間
        /// </summary>
        public string MarkOutTimeStr { get; set; }

        /// <summary>
        /// 截取時長
        /// </summary>
        public string MarkDurationStr { get; set; }

        ///// <summary>
        ///// 低解路徑 Procedure_Result.[_sFILE_URL]
        ///// </summary>
        //[Display(Name = "低解路徑")]
        //public string FileDLowUrl { get; set; } = string.Empty;

        /// <summary>
        /// 版權 [fsLICENSE] 搭配'版權代碼表 dbo.[tbmLICENSE]'
        /// </summary>
        /// <remarks> Added_20210915 </remarks>
        [Display(Name = "版權")]
        public string LicenseCode { get; set; } = string.Empty;
        /// <summary>
        /// 版權 LicenseCode中文
        /// </summary>
        /// <remarks> Added_20210915 </remarks>
        [Display(Name = "版權")]
        public string LicenseStr { get; set; } = string.Empty;
        /// <summary>
        /// 版權.是否調用提醒
        /// </summary>
        /// <remarks> Added_20210915 </remarks>
        [Display(Name = "是否調用提醒")]
        public bool IsAlert { get; set; }
        /// <summary>
        /// 版權.是否調用禁止
        /// </summary>
        /// <remarks> Added_20210915 </remarks>
        [Display(Name = "是否調用禁止")]
        public bool IsForBid { get; set; }
        /// <summary>
        /// 版權.提醒訊息內容
        /// </summary>
        /// <remarks> Added_20210915 </remarks>
        [Display(Name = "提醒訊息")]
        public string LicenseMessage { get; set; } = string.Empty;
        /// <summary>
        /// 版權.是否到期
        /// </summary>
        /// <remarks> Added_20211005 </remarks>
        [Display(Name = "是否授權到期")]
        public bool IsExpired { get; set; }
        /// <summary>
        /// 版權.授權到期日期
        /// </summary>
        /// <remarks> Added_20211005 </remarks>
        [Display(Name = "授權日期")]
        public string LicenseEndDate { get; set; }
        #endregion

        /// <summary>
        /// 資料型別<typeparamref name="T"/> 轉換處理
        /// </summary>
        /// <typeparam name="T">資料型別參數 如: <see cref="spGET_MATERIAL_BY_MARKED_BY_Result"/> </typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public MaterialListModel ConvertData<T>(T data)
        {
            if (data == null) { return this; }

            var _Properties = typeof(T).GetProperties();

            foreach (var info in _Properties)
            {
                var val = info.GetValue(data) ?? string.Empty;

                if (info.Name == "fnMATERIAL_ID")
                {
                    if (int.TryParse(val.ToString(), out int idx)) { this.MaterialId = idx; }
                }

                if (info.Name == "fsTYPE")
                {
                    this.FileCategory = val.ToString();
                    FileCategoryStr = GetEnums.GetDescriptionText((FileTypeEnum)Enum.Parse(typeof(FileTypeEnum), val.ToString()));
                }
                if (info.Name == "fsTITLE") this.Title = val.ToString();
                if (info.Name == "fdCREATED_DATE")
                {
                    if (DateTime.TryParse(val.ToString(), out DateTime dt))
                    {
                        this.CreatedDate = string.Format($"{dt:yyyy/MM/dd HH:mm:ss}");
                    }
                }
                //影片長度(時長)
                if (info.Name == "C_sVIDEO_MAX_TIME") { this.VideoMaxTime = val.ToString(); }

                if (info.Name == "fsPARAMETER") { this.ParameterStr = val.ToString(); }
                if (info.Name == "fsNOTE") { this.MaterialNote = val.ToString(); }
                if (info.Name == "fsFILE_NO") { this.FileNo = val.ToString(); }

                //20210915_ADDED_版權欄位、.是否調用提醒、是否調用禁止、提醒訊息
                if (info.Name == "fsLICENSE") { this.LicenseCode = val.ToString(); }
                if (info.Name == "fsLICENSE_NAME") { this.LicenseStr = val.ToString(); }
                if (info.Name == "fcIS_ALERT" || info.Name == "IS_ALERT")
                {
                    if (bool.TryParse(val.ToString(), out bool chk)) { this.IsAlert = chk; }
                }
                if (info.Name == "fcIS_FORBID" || info.Name == "IS_FORBID")
                {
                    if (bool.TryParse(val.ToString(), out bool chk)) { this.IsForBid = chk; }
                }
                if (info.Name == "fsLICENSE_MESSAGE" || info.Name == "MESSAGE") { this.LicenseMessage = val.ToString() ?? string.Empty; }
                //20211005_ADDED)_版權授權日期
                if (info.Name == "fcIS_LICENSE_EXPIRED" || info.Name == "IS_EXPIRED" || info.Name == "IsExpired")
                {
                    if (bool.TryParse(val.ToString(), out bool chk)) { this.IsExpired = chk; }
                }
                if (info.Name == "fdENDDATE" || info.Name == "LicenseEndDate")
                {
                    if (DateTime.TryParse(val.ToString(), out DateTime dt))
                    {
                        this.LicenseEndDate = string.Format($"{dt:yyyy/MM/dd}");
                    }
                    else
                    {
                        this.LicenseEndDate = string.Empty;
                    }
                }
            }

            var _clipAry = this.ParameterStr.Split(new char[] { ';' });

            double _vMax = string.IsNullOrEmpty(this.VideoMaxTime) ? 0 : double.Parse(this.VideoMaxTime),
                _vStart = (_clipAry.Length > 0 && _clipAry[0] != null) ? double.Parse(_clipAry[0].ToString()) : 0,
                _vEnd = (_clipAry.Length > 0 && _clipAry[1] != null) ? double.Parse(_clipAry[1].ToString()) : 0,
                _clipDiff = (_vStart == 0 && _vEnd == 0) ? 0 : (_vEnd - _vStart);

            this.VideoMaxTimeStr = string.IsNullOrEmpty(this.VideoMaxTime) ? "00:00:00;00" : CommonTimeCode.CurrentTimesToTimeCode(_vMax);

            this.MarkInTimeStr = _vStart == 0 ? string.Empty : CommonTimeCode.CurrentTimesToTimeCode(_vStart);

            this.MarkOutTimeStr = _vEnd == 0 ? string.Empty : CommonTimeCode.CurrentTimesToTimeCode(_vEnd);

            this.MarkDurationStr = (_vStart == 0 && _vEnd == 0) ? string.Empty : CommonTimeCode.CurrentTimesToTimeCode(_clipDiff);

            return this;
        }
    }

}
