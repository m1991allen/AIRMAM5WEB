using AIRMAM5.DBEntity.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Subject
{
    /// <summary>
    /// (影音)點選顯示頁: 點選左側檔案列表,右側分頁'段落描述'(只有影、音)
    /// </summary>
    public class SubjectFileSeqmentModel
    {
        /// <summary>
        /// (影音)點選顯示頁: 點選左側檔案列表,右側分頁'段落描述'(只有影、音)
        /// </summary>
        public SubjectFileSeqmentModel() { }
        
        #region >>>>> 欄位參數 
        /// <summary>
        /// 檔案編號
        /// </summary>
        [Display(Name = "檔案編號")]
        public string fsFILE_NO { set; get; } = string.Empty;

        /// <summary>
        /// 流水號 fnSEQ_NO
        /// </summary>
        [Display(Name = "流水號")]
        public int SeqNo { set; get; } = 0;

        /// <summary>
        /// 描述 fsDESCRIPTION
        /// </summary>
        [Display(Name = "描述")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 開始時間 (ex: 4.53)
        /// </summary>
        [Display(Name = "開始時間")]
        public decimal BegTime { get; set; } = 0;

        /// <summary>
        /// 結束時間 (ex: 22.369)
        /// </summary>
        [Display(Name = "結束時間")]
        public decimal EndTime { get; set; } = 0;
        #endregion

        /// <summary>
        /// (影音)點選顯示頁: 點選左側檔案列表,右側分頁'段落描述', 資料型別<typeparamref name="T"/> 轉換處理
        /// </summary>
        /// <typeparam name="T">資料型別參數 如: 
        ///     <para>1.媒資檔案-段落描述.預存_Result: <see cref="spGET_ARC_VIDEO_D_Result"/>, <see cref="spGET_ARC_AUDIO_D_Result"/> </para>
        ///     <para>2.刪除紀錄-段落描述.預存_Result: <see cref="sp_t_GET_ARC_VIDEO_D_Result"/>, <see cref="sp_t_GET_ARC_AUDIO_D_Result"/> </para> 
        /// </typeparam>
        /// <param name="m"></param>
        /// <param name="fileCattegory">媒資檔案類別 <see cref="FileTypeEnum"/> (只有V,A 才有段落描述)</param>
        /// <returns></returns>
        public SubjectFileSeqmentModel FormatConvert<T>(T m, FileTypeEnum fileCattegory)
        {
            var _properties = typeof(T).GetProperties();
            foreach (var p in _properties)
            {
                var _val = p.GetValue(m) ?? string.Empty;

                if (p.Name == "fsFILE_NO") this.fsFILE_NO = _val.ToString();
                if (p.Name == "fnSEQ_NO")
                {
                    int.TryParse(_val.ToString(), out int _gval);
                    this.SeqNo = _gval;
                }
                if (p.Name == "fsDESCRIPTION") this.Description = _val.ToString();
                if (p.Name == "fdBEG_TIME")
                {
                    decimal.TryParse(_val.ToString(), out decimal _gval);
                    this.BegTime = _gval;
                }
                if (p.Name == "fdEND_TIME")
                {
                    decimal.TryParse(_val.ToString(), out decimal _gval);
                    this.EndTime = _gval;
                }
            }

            return this;
        }

    }
}
