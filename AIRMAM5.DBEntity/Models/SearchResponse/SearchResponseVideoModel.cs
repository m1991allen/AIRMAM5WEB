using AIRMAM5.DBEntity.Models.Search;
using AIRMAM5.DBEntity.Models.Subject;
using System.Collections.Generic;

namespace AIRMAM5.DBEntity.Models.SearchResponse
{
    /// <summary>
    /// 檢索結果-【影片 VIDEO】: SearchResponseBase() + 關鍵影格、段落描述
    /// </summary>
    public class SearchResponseVideoModel //: SearchResponseBaseModel
    {
        /// <summary>
        /// 檢索結果-【影片 VIDEO】 初始
        /// </summary>
        public SearchResponseVideoModel() { }

        /// <summary>
        /// 檢索結果-【影片 VIDEO】 設定值
        /// </summary>
        /// <param name="m">檢索參數 <see cref="SearchParameterViewModel"/> </param>
        public SearchResponseVideoModel(SearchParameterViewModel m) { SearchBase.SearchParam = m; }

        /// <summary>
        /// 檢索結果-Basic <see cref="SearchResponseBaseModel"/>
        /// </summary>
        public SearchResponseBaseModel SearchBase { get; set; } = new SearchResponseBaseModel();

        /// <summary>
        /// 影片-關鍵影格清單 <see cref="VideoKeyFrameModel"/>
        /// </summary>
        public List<VideoKeyFrameModel> KeyFrameList { get; set; } = new List<VideoKeyFrameModel>();

        /// <summary>
        /// 影片-段落描述清單 <see cref="SubjectFileSeqmentModel"/>
        /// </summary>
        public List<SubjectFileSeqmentModel> ParaDescription { get; set; } = new List<SubjectFileSeqmentModel>();
    }

}
