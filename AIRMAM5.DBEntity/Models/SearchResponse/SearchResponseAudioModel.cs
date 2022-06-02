using AIRMAM5.DBEntity.Models.Search;
using AIRMAM5.DBEntity.Models.Subject;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.SearchResponse
{
    /// <summary>
    /// 檢索結果-【聲音 Audio】: SearchResponseBase() + 專輯資訊、段落描述
    /// </summary>
    public class SearchResponseAudioModel
    {
        /// <summary>
        /// 檢索結果-【聲音 Audio】
        /// </summary>
        public SearchResponseAudioModel() { }

        /// <summary>
        /// 檢索結果-【聲音 Audio】
        /// </summary>
        /// <param name="m">檢索參數 <see cref="SearchParameterViewModel"/> </param>
        public SearchResponseAudioModel(SearchParameterViewModel m) { SearchBase.SearchParam = m; }

        /// <summary>
        /// 檢索結果-Basic <see cref="SearchResponseBaseModel"/>
        /// </summary>
        public SearchResponseBaseModel SearchBase { get; set; } = new SearchResponseBaseModel();

        /// <summary>
        /// 聲音-段落描述清單 <see cref="SubjectFileSeqmentModel"/>
        /// </summary>
        public List<SubjectFileSeqmentModel> ParaDescription { get; set; } = new List<SubjectFileSeqmentModel>();

        /// <summary>
        /// 聲音檔-專輯資訊 <see cref="AudioInfoModel"/>
        /// </summary>
        public AudioInfoModel AudioAlbumInfo { get; set; } = new AudioInfoModel();

        /// <summary>
        /// 專輯資訊 
        /// </summary>
        public class AudioInfoModel
        {
            public AudioInfoModel() { }

            #region >>> 欄位參數
            /// <summary>
            /// 專輯名稱
            /// </summary>
            [Display(Name= "專輯名稱")]
            public string Album { get; set; } = string.Empty;

            /// <summary>
            /// 專輯標題
            /// </summary>
            [Display(Name = "專輯標題")]
            public string AlbumTitle { get; set; } = string.Empty;

            /// <summary>
            /// 專輯演出者
            /// </summary>
            [Display(Name = "專輯演出者")]
            public string AlbumArtists { get; set; } = string.Empty;

            /// <summary>
            /// 歌曲演出者
            /// </summary>
            [Display(Name = "歌曲演出者")]
            public string AlbumPerformers { get; set; } = string.Empty;

            /// <summary>
            /// 歌曲作曲者
            /// </summary>
            [Display(Name = "歌曲作曲者")]
            public string AlbumComposers { get; set; } = string.Empty;

            /// <summary>
            /// 專輯年份
            /// </summary>
            [Display(Name = "專輯年份")]
            public int AlbumYear { get; set; } = 0;

            /// <summary>
            /// 著作權
            /// </summary>
            [Display(Name = "著作權")]
            public string AlbumCopyright { get; set; } = string.Empty;

            /// <summary>
            /// 內容類型
            /// </summary>
            [Display(Name = "內容類型")]
            public string AlbumGenres { get; set; } = string.Empty;

            /// <summary>
            /// 備註
            /// </summary>
            [Display(Name = "備註")]
            public string AlbumComment { get; set; } = string.Empty;
            #endregion
        }
    }

}
