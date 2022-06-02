using AIRMAM5.DBEntity.Models.Directory;
using System.Collections.Generic;
using System.Linq;

namespace AIRMAM5.DBEntity.Models.ArchiveMove
{
    /// <summary>
    /// 歸檔搬遷: 主題的檔案清單資料 Model。 繼承參考 <see cref="DirIdModel"/>
    /// </summary>
    public class SubjectFilesViewModel : DirIdModel
    {
        /// <summary>
        /// 歸檔搬遷: 主題的檔案清單資料 Model
        /// </summary>
        public SubjectFilesViewModel() { }

        #region >>>>> 欄位參數 
        ///// <summary>
        ///// 系統目錄編號 fsDIR_ID
        ///// </summary>
        //public long DirId { get; set; }

        /// <summary>
        /// 主題編號 fsSUBJECT_ID
        /// </summary>
        public string SubjectId { set; get; } = string.Empty;

        /// <summary>
        /// 影片檔案資料_list
        /// </summary>
        public List<SubjFileModel> VideoFiles { get; set; }

        /// <summary>
        /// 聲音檔案資料_list
        /// </summary>
        public List<SubjFileModel> AudioFiles { get; set; }

        /// <summary>
        /// 照片檔案資料_list
        /// </summary>
        public List<SubjFileModel> PhotoFiles { get; set; }

        /// <summary>
        /// 文件檔案資料_list
        /// </summary>
        public List<SubjFileModel> DocFiles { get; set; }
        #endregion

        ///// <summary>  Marked_20210901_加入DI調整寫法
        ///// 依 主題編號 取得檔案清單資料
        ///// </summary>
        ///// <param name="subjid">主題編號 </param>
        ///// <returns> 主題的檔案清單<see cref="SubjectFilesViewModel"/> </returns>
        //public SubjectFilesViewModel SetSubjectFilesData(string subjid)
        //{
        //    SubjectFilesViewModel md = new SubjectFilesViewModel();

        //    var _arcVideoSer = new ArcVideoService();
        //    var _v = _arcVideoSer.GetVideioBySubjectId(subjid);
        //    //SubjFileModel<spGET_ARC_VIDEO_BY_SUBJ_ID_Result> _vsj = new SubjFileModel<spGET_ARC_VIDEO_BY_SUBJ_ID_Result>();
        //    //md.VideoFiles = _v.Select(s => _vsj.FormatConversion(s, FileTypeEnum.V)).ToList();
        //    md.VideoFiles = _v.Select(s => new SubjFileModel().FormatConversion(s, FileTypeEnum.V)).ToList();

        //    var _arcAudioSer = new ArcAudioService();
        //    var _a = _arcAudioSer.GetArcAudioBySubjectId(subjid);
        //    //SubjFileModel<spGET_ARC_AUDIO_BY_SUBJ_ID_Result> _asj = new SubjFileModel<spGET_ARC_AUDIO_BY_SUBJ_ID_Result>();
        //    md.AudioFiles = _a.Select(s => new SubjFileModel().FormatConversion(s, FileTypeEnum.A)).ToList();

        //    var _arcPhotoSer = new ArcPhotoService();
        //    var _p = _arcPhotoSer.GetArcPhotoBySubjectId(subjid);
        //    //SubjFileModel<spGET_ARC_PHOTO_BY_SUBJ_ID_Result> _psj = new SubjFileModel<spGET_ARC_PHOTO_BY_SUBJ_ID_Result>();
        //    md.PhotoFiles = _p.Select(s => new SubjFileModel().FormatConversion(s, FileTypeEnum.P)).ToList();

        //    var _arcDocSer = new ArcDocService();
        //    var _d = _arcDocSer.GetArcDocBySubjectId(subjid);
        //    //SubjFileModel<spGET_ARC_DOC_BY_SUBJ_ID_Result> _dsj = new SubjFileModel<spGET_ARC_DOC_BY_SUBJ_ID_Result>();
        //    md.DocFiles = _d.Select(s => new SubjFileModel().FormatConversion(s, FileTypeEnum.D)).ToList();

        //    return md;
        //}
    }

}
