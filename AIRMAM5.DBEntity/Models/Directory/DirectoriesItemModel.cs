
using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Services;
using System.Linq;

namespace AIRMAM5.DBEntity.Models.Directory
{
    /// <summary>
    /// 系統目錄(樹狀)項目內容。 繼承參考<see cref="DirIdModel"/>
    /// </summary>
    public class DirectoriesItemModel : DirIdModel
    {
        readonly ConfigService _configService = new ConfigService();
        readonly spGET_CONFIG_Result cong;
        readonly bool isUsingQueue = true;

        /// <summary>
        /// 系統目錄(樹狀)項目內容
        /// </summary>
        public DirectoriesItemModel()
        {
            cong = _configService.GetConfigBy("DIRECTORIES_USING_QUEUE").FirstOrDefault();  //設定系統目錄維護功能中，是否啟用Queue節點操作
            if (cong != null) { isUsingQueue = cong.fsVALUE == "1" ? true : false; }
            this.UsingQueue = isUsingQueue;
        }

        #region >>>>> 欄位參數
        /// <summary>
        /// 目錄/節點名稱 fsNAME
        /// </summary>
        public string DirName { get; set; } = string.Empty;

        /// <summary>
        /// 是否有子項目
        /// </summary>
        public bool HasChildren { get; set; }

        /// <summary>
        /// 目錄節點類別: Q表示非目錄匣,是Queue
        /// </summary>
        public string DirType { get; set; } = string.Empty;

        /// <summary>
        /// 路徑
        /// </summary>
        public string DirPathStr { get; set; } = string.Empty;

        /// <summary>
        /// 節點的主題數量
        /// </summary>
        public int ChildrenLength { get; set; } = 0;

        /// <summary>
        /// 目錄維護是否啟用 末節點Queue (default: true), 20201116
        /// </summary>
        public bool UsingQueue { get; set; }
        #endregion

        /// <summary>
        /// 系統目錄(樹狀)項目內容, 資料型別<typeparamref name="T"/> 轉換處理
        /// </summary>
        /// <typeparam name="T">資料型別參數 如: 
        ///     <see cref="spGET_DIRECTORIES_LOAD_ON_DEMAND_Result"/>, 
        ///     <see cref="spGET_DIRECTORIES_LOAD_ON_DEMAND_ALL_Result"/>,
        ///     <see cref="spGET_DIRECTORIES_BY_FILE_NO_LOAD_ON_DEMAND_Result"/> 預存回傳值</typeparam>
        /// <param name="m"></param>
        /// <returns></returns>
        public DirectoriesItemModel FormatConversion<T>(T m)
        {
            var _properties = typeof(T).GetProperties();

            foreach (var p in _properties)
            {
                var _val = p.GetValue(m) ?? string.Empty;

                if (p.Name == "fnDIR_ID")
                    this.DirId = string.IsNullOrEmpty(_val.ToString()) ? 0 : long.Parse(_val.ToString());

                if (p.Name == "fsNAME")
                    this.DirName = string.IsNullOrEmpty(_val.ToString()) ? string.Empty : _val.ToString();

                if (p.Name == "fsDIRTYPE")
                {
                    this.HasChildren = _val.ToString() == "Q" ? false : true;
                    this.DirType = _val.ToString();
                }

                if (p.Name == "fsPATH_NAME")
                    this.DirPathStr = string.IsNullOrEmpty(_val.ToString()) ? string.Empty : _val.ToString();
            }

            //this.UsingQueue = isUsingQueue;
            return this;
        }
    }

}
