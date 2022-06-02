using AIRMAM5.DBEntity.DBEntity;

namespace AIRMAM5.DBEntity.Models.Subject
{
    /// <summary>
    /// 關鍵影格_新增/編輯 ViewMODEL ,  繼承參考 <see cref="SubjFileNoModel"/>
    /// </summary>
    public class KeyFrameCUViewModel : SubjFileNoModel
    {
        /// <summary>
        /// 關鍵影格_新增/編輯 ViewMODEL
        /// </summary>
        public KeyFrameCUViewModel() { }

        /*20201006_調整至 類別方法: FormatConversion<T>() */
        ///// <summary>
        ///// 關鍵影格_新增/編輯 ViewMODEL
        ///// </summary>
        ///// <param name="m"></param>
        //public KeyFrameCUViewModel(KeyFrameCUViewModel m)
        //{
        //    this.fsFILE_NO = m.fsFILE_NO ?? string.Empty;
        //    this.fsSUBJECT_ID = m.fsSUBJECT_ID ?? string.Empty;
        //    this.Title = m.Title ?? string.Empty;
        //    this.SetTime = m.SetTime ?? string.Empty;
        //    this.Description = m.Description ?? string.Empty;
        //}
        ///// <summary>
        ///// 關鍵影格_新增/編輯 ViewMODEL
        ///// </summary>
        ///// <param name="m">預存回傳資料集 <see cref="spGET_ARC_VIDEO_K_Result"/></param>
        //public KeyFrameCUViewModel(spGET_ARC_VIDEO_K_Result m)
        //{
        //    this.fsFILE_NO = m.fsFILE_NO ?? string.Empty;
        //    //this.fsSUBJECT_ID = m.fsSUBJECT_ID ?? string.Empty;
        //    this.Title = m.fsTITLE ?? string.Empty;
        //    this.SetTime = m.fsTIME ?? string.Empty;
        //    this.Description = m.fsDESCRIPTION ?? string.Empty;
        //}

        #region >>>>>欄位參數
        /// <summary>
        /// 關鍵影格標題
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 設定時間(秒) = 002760.040
        /// </summary>
        public string SetTime { get; set; } = string.Empty;

        /// <summary>
        /// 關鍵影格描述
        /// </summary>
        public string Description { get; set; } = string.Empty;
        #endregion

        /// <summary>
        /// 關鍵影格_新增/編輯 頁面資料, 資料型別<typeparamref name="T"/> 轉換處理
        /// </summary>
        /// <typeparam name="T">資料型別參數 如: <see cref="KeyFrameCUViewModel"/> , <see cref="spGET_ARC_VIDEO_K_Result"/> </typeparam>
        /// <param name="m"></param>
        /// <returns></returns>
        public KeyFrameCUViewModel FormatConvert<T>(T m)
        {
            if (m == null) { return this; }

            var _properties = typeof(T).GetProperties();
            foreach (var p in _properties)
            {
                var val = p.GetValue(m) ?? string.Empty;

                if (p.Name == "fsFILE_NO") this.fsFILE_NO = val.ToString();
                if (p.Name == "fsSUBJECT_ID") this.fsSUBJECT_ID = val.ToString();
                if (p.Name == "Title" || p.Name == "fsTITLE") this.Title = val.ToString();
                if (p.Name == "SetTime" || p.Name == "fsTIME") this.SetTime = val.ToString();
                if (p.Name == "Description" || p.Name == "fsDESCRIPTION") this.Description = val.ToString();
            }

            return this;
        }

    }

}
