using AIRMAM5.DBEntity.Models.Search;
using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.SearchResponse
{
    /// <summary>
    /// 檢索結果-【文件 DOC】: SearchResponseBase() + 文件資訊
    /// </summary>
    public class SearchResponseDocModel
    {
        /// <summary>
        /// 檢索結果-【文件 DOC】
        /// </summary>
        public SearchResponseDocModel() { }

        /// <summary>
        /// 檢索結果-【文件 DOC】
        /// </summary>
        /// <param name="m">檢索參數 <see cref="SearchParameterViewModel"/> </param>
        public SearchResponseDocModel(SearchParameterViewModel m) { SearchBase.SearchParam = m; }

        /// <summary>
        /// 檢索結果-Basic <see cref="SearchResponseBaseModel"/>
        /// </summary>
        public SearchResponseBaseModel SearchBase { get; set; } = new SearchResponseBaseModel();

        /// <summary>
        /// 文件-文件資訊 <see cref="ExtendIfno"/>
        /// </summary>
        public ExtendIfno DocInfo { get; set; } = new ExtendIfno();

        /// <summary>
        /// 文件資訊
        /// </summary>
        public class ExtendIfno
        {
            /// <summary>
            /// 文件內容
            /// </summary>
            [Display(Name = "文件內容")]
            public string Content { get; set; } = string.Empty;

            /// <summary>
            /// 文件建立日期
            /// </summary>
            [Display(Name = "建立日期")]
            public DateTime? FileCreatedDate { get; set; }

            /// <summary>
            /// 文件修改日期
            /// </summary>
            [Display(Name= "修改日期")]
            public DateTime? FileUpdatedDate { get; set; }
        }
    }
}
