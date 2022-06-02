using AIRMAM5.DBEntity.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Subject
{
    /// <summary>
    /// (影)點選顯示頁: 點選左側檔案列表,右側分頁'關鍵影格'(只有影)
    /// </summary>
    public class VideoKeyFrameModel
    {
        /// <summary>
        /// (影)點選顯示頁: 點選左側檔案列表,右側分頁'關鍵影格'(只有影)
        /// </summary>
        public VideoKeyFrameModel() { }

        #region >>>>> 欄位參數 
        /// <summary>
        /// 檔案編號
        /// </summary>
        public string fsFILE_NO { set; get; } = string.Empty;

        /// <summary>
        /// 標題 fsTITLE
        /// </summary> 
        [Display(Name = "標題")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 描述 fsDESCRIPTION
        /// </summary>
        [Display(Name = "描述")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 是否為檔案的代表圖 [fsHEAD_FRAME]
        /// </summary>
        public bool IsHeadFrame { get; set; } = false;

        /// <summary>
        /// 檔案路徑 [fsFILE_PATH]
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// 時間(毫秒) [fsTIME]=000000000。例: 000139243 等於139.243秒。
        /// </summary>
        [Display(Name = "時間")]
        public string Time { get; set; } = "000000000";

        /// <summary>
        /// 截圖URL [_sIMAGE_URL]
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// 檔案資訊 [_sFILE_INFO] (EX: 關鍵影格編號: 20190311_0000012)
        /// </summary>
        public string FileInfo { get; set; } = string.Empty;
        #endregion

        /// <summary>
        /// (影)點選顯示頁: 點選左側檔案列表,右側分頁'關鍵影格', 資料型別<typeparamref name="T"/> 轉換處理
        /// </summary>
        /// <typeparam name="T">資料型別參數 如: 
        ///     <para>1.媒資檔案-影片關鍵影格.預存_Result: <see cref="spGET_ARC_VIDEO_K_Result"/>, </para>
        ///     <para>2.刪除紀錄-影片關鍵影格.預存_Result: <see cref="sp_t_GET_ARC_VIDEO_K_Result"/> </para> 
        /// </typeparam>
        /// <param name="m"></param>
        /// <param name="fileCattegory">媒資檔案類別: V,A,P,D  <see cref="FileTypeEnum"/></param>
        /// <returns></returns>
        public VideoKeyFrameModel FormatConvert<T>(T m, FileTypeEnum fileCattegory)
        {
            var _properties = typeof(T).GetProperties();
            foreach (var p in _properties)
            {
                var _val = p.GetValue(m) ?? string.Empty;

                if (p.Name == "fsFILE_NO") this.fsFILE_NO = _val.ToString();
                if (p.Name == "fsTITLE") this.Title = _val.ToString();
                if (p.Name == "fsDESCRIPTION") this.Description = _val.ToString();
                if (p.Name == "fcHEAD_FRAME")
                {
                    this.IsHeadFrame = _val.ToString().ToUpper() == IsTrueFalseEnum.Y.ToString() ? true : false;
                }

                if (p.Name == "fsFILE_PATH") this.FilePath = _val.ToString();
                if (p.Name == "fsTIME") this.Time = _val.ToString();
                if (p.Name == "C_sIMAGE_URL") this.ImageUrl = _val.ToString();
                if (p.Name == "C_sFILE_INFO") this.FileInfo = _val.ToString();
            }

            return this;
        }

    }

}
