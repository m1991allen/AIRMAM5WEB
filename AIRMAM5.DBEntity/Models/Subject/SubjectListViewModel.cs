using AIRMAM5.DBEntity.DBEntity;

namespace AIRMAM5.DBEntity.Models.Subject
{
    /// <summary>
    /// 主題與檔案 Index列表  繼承參考 <see cref="SubjectIdModel"/>
    /// </summary>
    public class SubjectListViewModel : SubjectIdModel
    {
        /// <summary>
        /// 主題與檔案 Index列表 
        /// </summary>
        public SubjectListViewModel() { }

        ///// <summary>
        ///// 主題與檔案 Index列表
        ///// </summary>
        ///// <param name="m">預存回傳資料集 <see cref="spGET_DIRECTORIES_AND_SUBJECTS_BY_DIR_ID_Result"/></param>
        //public SubjectListViewModel(spGET_DIRECTORIES_AND_SUBJECTS_BY_DIR_ID_Result m)
        //{
        //    fsSUBJECT_ID = m.fsSUBJ_ID;
        //    fsSUBJECT_TITLE = m.fsTITLE;
        //    fsDESCRIPTION = m.fsDESCRIPTION;
        //    VideoCount = m.C_nVideo ?? 0;
        //    AudioCount = m.C_nAudio ?? 0;
        //    PhotoCount = m.C_nPhoto ?? 0;
        //    DocCount = m.C_nDocument ?? 0;
        //}

        #region >>>>> 欄位參數 
        ///// <summary>
        ///// 主題編號
        ///// </summary>
        //public string fsSUBJECT_ID { set; get; } = string.Empty;

        /// <summary>
        /// 主題標題
        /// </summary>
        public string fsSUBJECT_TITLE { set; get; } = string.Empty;

        /// <summary>
        /// 主題檔案描述
        /// </summary>
        public string fsDESCRIPTION { set; get; } = string.Empty;

        /// <summary>
        /// 影片數量
        /// </summary>
        public int VideoCount { set; get; } = 0;

        /// <summary>
        /// 聲音檔數量
        /// </summary>
        public int AudioCount { set; get; } = 0;

        /// <summary>
        /// 圖片數量
        /// </summary>
        public int PhotoCount { set; get; } = 0;

        /// <summary>
        /// 文檔數量
        /// </summary>
        public int DocCount { set; get; } = 0;
        #endregion

        /// <summary>
        /// 主題與檔案 Index列表, 資料型別<typeparamref name="T"/> 轉換處理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="m">預存回傳資料集, 如 <see cref="spGET_DIRECTORIES_AND_SUBJECTS_BY_DIR_ID_Result"/>, 
        /// <see cref="spGET_DIRECTORIES_AND_SUBJECTS_BY_DIR_ID_Filter_Result"/>
        /// </param>
        /// <returns></returns>
        public SubjectListViewModel FormatConvert<T>(T m)
        {
            if (m == null) { return this; }

            var _properties = typeof(T).GetProperties();

            foreach (var p in _properties)
            {
                var _val = p.GetValue(m) ?? string.Empty;

                if (p.Name == "fsSUBJ_ID") this.fsSUBJECT_ID = _val.ToString();
                if (p.Name == "fsTITLE") this.fsSUBJECT_TITLE = _val.ToString();
                if (p.Name == "fsDESCRIPTION") this.fsDESCRIPTION = _val.ToString();
                if (p.Name == "C_nVideo")
                {
                    int.TryParse(_val.ToString(), out int v);
                    this.VideoCount = v;
                }
                if (p.Name == "C_nAudio")
                {
                    int.TryParse(_val.ToString(), out int v);
                    this.AudioCount = v;
                }
                if (p.Name == "C_nPhoto")
                {
                    int.TryParse(_val.ToString(), out int v);
                    this.PhotoCount = v;
                }
                if (p.Name == "C_nDocument")
                {
                    int.TryParse(_val.ToString(), out int v);
                    this.DocCount = v;
                }
            }

            return this;
        }
    }

}
