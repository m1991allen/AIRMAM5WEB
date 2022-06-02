//using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Models.ArcPre;
using AIRMAM5.DBEntity.Models.Common;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Subject;
using AIRMAM5.DBEntity.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AIRMAM5.DBEntity.Models.AcrDelete
{
    /* 刪除(媒資檔案)記錄表
     * t_tbmARC_INDEX(主檔
     * t_tbmARC_VIDEO 
     * t_tbmARC_AUDIO
     * t_tbmARC_PHOTO
     * t_tbmARC_DOC
     * 
     * */

    /// <summary>
    /// 刪除紀錄管理-檢視媒資檔案內容, 繼承可參考 <seealso cref="MediaDetailsCommonModel"/>
    /// </summary>
    public class DeleteArcViewModel : MediaDetailsCommonModel
    {
        /// <summary>
        /// 刪除紀錄管理-檢視媒資檔案內容
        /// </summary>
        public DeleteArcViewModel() { }

        #region >>>>> 屬性/欄位
        /// <summary>
        /// 編號 fnINDEX_ID (媒資刪除記錄)
        /// </summary>
        [Display(Name = "編號")]
        public long IndexId { get; set; }

        /// <summary>
        /// 檔案編號
        /// </summary>
        [Display(Name = "檔案編號")]
        public string fsFILE_NO { set; get; } = string.Empty;

        /// <summary>
        /// 刪除原因 [fsREASON]
        /// </summary>
        [Display(Name = "刪除原因")]
        public string DeleteReason { get; set; } = string.Empty;

        // (影音圖文)媒資檔案顯示: player/image
        //public ShowViewerModel Viewer { get; set; }
        // 媒資基本資料
        //public ArcBasicMetaModel BasicMeta { get; set; }
        // (影片)關鍵影格
        //public List<VideoKeyFrameModel> KeyFrame { get; set; }
        // (影片,聲音)段落描述 ParagraphDescribe
        //public List<SubjectFileSeqmentModel> FileSeqment { get; set; }
        // 自訂欄位資訊 CustomFields
        //public List<ArcPreAttributeModel> DynamicInfo { get; set; }
        #endregion

        /* Modified_20211005 */
        ///// <summary>
        ///// 傳入資料 轉換類別格式
        ///// </summary>
        ///// <typeparam name="T">傳入資料的 類別格式 eg. <see cref="sp_t_GET_ARC_INDEX_BY_CONDITION_Result"/></typeparam>
        ///// <param name="data"></param>
        ///// <param name="mediatype"></param>
        ///// <param name="arcIndexSer"> T_tbmARC_IndexService 檔案刪除資料桶主檔 Service</param>
        ///// <returns></returns>
        //public DeleteArcViewModel DataConvert<T>(T data, FileTypeEnum mediatype, T_tbmARC_IndexService arcIndexSer)
        //{
        //    if (data == null) { return this; }
        //    var properties = typeof(T).GetProperties();

        //    foreach (var p in properties)
        //    {
        //        var val = p.GetValue(data) == null ? string.Empty : p.GetValue(data).ToString();
        //        string str = string.Concat("^", p.Name);

        //        if (str.Contains("^fnINDEX_ID"))
        //        {
        //            bool chk = long.TryParse(val, out long idx);
        //            this.IndexId = idx;
        //        }
        //        if (str.Contains("^fsFILE_NO"))
        //        {
        //            this.fsFILE_NO = val;
        //        }
        //        if (str.Contains("^fsREASON^DeleteReason") )
        //        {
        //            this.DeleteReason = val;
        //        }
        //    }

        //    switch (mediatype)
        //    {
        //        case FileTypeEnum.V:
        //            var _v = arcIndexSer.GetArcVideoById(this.IndexId);
        //            this.Viewer = new ShowViewerModel().FormatConversion(_v, mediatype);
        //            this.BasicMeta = new ArcBasicMetaModel().FormatConversion(_v, mediatype);
        //            this.KeyFrame = arcIndexSer.GetArcVideoKeyById(this.IndexId).Select(s => new VideoKeyFrameModel().FormatConvert(s, mediatype)).ToList();
        //            this.FileSeqment = arcIndexSer.GetArcVideoDescrById(this.IndexId).Select(s => new SubjectFileSeqmentModel().FormatConvert(s, mediatype)).ToList();
        //            this.DynamicInfo = arcIndexSer.GetArcVideoAttributeById(this.fsFILE_NO).Select(s => new ArcPreAttributeModel().FormatConversion(s, mediatype)).ToList();
        //            break;
        //        case FileTypeEnum.A:
        //            var _a = arcIndexSer.GetArcAudioById(this.IndexId);
        //            this.Viewer = new ShowViewerModel().FormatConversion(_a, mediatype);
        //            this.BasicMeta = new ArcBasicMetaModel().FormatConversion(_a, mediatype);
        //            this.FileSeqment = arcIndexSer.GetArcAudioDescrById(this.IndexId).Select(s => new SubjectFileSeqmentModel().FormatConvert(s, mediatype)).ToList();
        //            this.DynamicInfo = arcIndexSer.GetArcAudioAttributeById(this.fsFILE_NO).Select(s => new ArcPreAttributeModel().FormatConversion(s, mediatype)).ToList();

        //            #region >>>>> 聲音資訊+專輯資訊
        //            this.DynamicInfo.Add(new ArcPreAttributeModel("fsALBUM", _a.fsALBUM ?? string.Empty, "專輯名稱"));
        //            this.DynamicInfo.Add(new ArcPreAttributeModel("fsALBUM_TITLE", _a.fsALBUM_TITLE ?? string.Empty, "專輯標題"));
        //            this.DynamicInfo.Add(new ArcPreAttributeModel("fsALBUM_ARTISTS", _a.fsALBUM_ARTISTS ?? string.Empty, "專輯演出者"));
        //            this.DynamicInfo.Add(new ArcPreAttributeModel("fsALBUM_PERFORMERS", _a.fsALBUM_PERFORMERS ?? string.Empty, "歌曲演出者"));
        //            this.DynamicInfo.Add(new ArcPreAttributeModel("fsALBUM_COMPOSERS", _a.fsALBUM_COMPOSERS ?? string.Empty, "歌曲作曲者"));
        //            this.DynamicInfo.Add(new ArcPreAttributeModel("fnALBUM_YEAR", (_a.fnALBUM_YEAR == null ? string.Empty : _a.fnALBUM_YEAR.ToString()), "專輯年份"));
        //            this.DynamicInfo.Add(new ArcPreAttributeModel("fsALBUM_COPYRIGHT", _a.fsALBUM_COPYRIGHT ?? string.Empty, "著作權"));
        //            this.DynamicInfo.Add(new ArcPreAttributeModel("fsALBUM_GENRES", _a.fsALBUM_GENRES ?? string.Empty, "內容類型"));
        //            this.DynamicInfo.Add(new ArcPreAttributeModel("fsALBUM_COMMENT", _a.fsALBUM_COMMENT ?? string.Empty, "備註"));
        //            #endregion
        //            break;
        //        case FileTypeEnum.P:
        //            var _p = arcIndexSer.GetArcPhotoById(this.IndexId);
        //            this.Viewer = new ShowViewerModel().FormatConversion(_p, mediatype);
        //            this.BasicMeta = new ArcBasicMetaModel().FormatConversion(_p, mediatype);
        //            this.DynamicInfo = arcIndexSer.GetArcPhotoAttributeById(this.fsFILE_NO).Select(s => new ArcPreAttributeModel().FormatConversion(s, mediatype)).ToList();

        //            #region >>>>> 圖片資訊+EXIF資訊
        //            this.DynamicInfo.Add(new ArcPreAttributeModel("fnWIDTH", _p.fnWIDTH.ToString(), "圖片寬"));
        //            this.DynamicInfo.Add(new ArcPreAttributeModel("fnHEIGHT", _p.fnHEIGHT.ToString(), "圖片高"));
        //            this.DynamicInfo.Add(new ArcPreAttributeModel("fnXDPI", _p.fnXDPI.ToString(), "XDPI"));
        //            this.DynamicInfo.Add(new ArcPreAttributeModel("fnYDPI", (_p.fnYDPI == null ? string.Empty : _p.fnYDPI.ToString()), "YDPI"));
        //            this.DynamicInfo.Add(new ArcPreAttributeModel("fsCAMERA_MAKE", _p.fsCAMERA_MAKE ?? string.Empty, "相機廠牌"));
        //            this.DynamicInfo.Add(new ArcPreAttributeModel("fsCAMERA_MODEL", _p.fsCAMERA_MODEL ?? string.Empty, "相機型號"));
        //            this.DynamicInfo.Add(new ArcPreAttributeModel("fsFOCAL_LENGTH", _p.fsFOCAL_LENGTH ?? string.Empty, "焦距"));
        //            this.DynamicInfo.Add(new ArcPreAttributeModel("fsEXPOSURE_TIME", _p.fsEXPOSURE_TIME ?? string.Empty, "曝光時間"));
        //            this.DynamicInfo.Add(new ArcPreAttributeModel("fsAPERTURE", _p.fsAPERTURE ?? string.Empty, "光圈"));
        //            this.DynamicInfo.Add(new ArcPreAttributeModel("fnISO", (_p.fnISO == null ? string.Empty : _p.fnISO.ToString()), "ISO"));
        //            #endregion
        //            break;
        //        case FileTypeEnum.D:
        //            var _d = arcIndexSer.GetArcDocById(this.IndexId);
        //            this.Viewer = new ShowViewerModel().FormatConversion(_d, mediatype);
        //            this.BasicMeta = new ArcBasicMetaModel().FormatConversion(_d, mediatype);
        //            this.DynamicInfo = arcIndexSer.GetArcDocAttributeById(this.fsFILE_NO).Select(s => new ArcPreAttributeModel().FormatConversion(s, mediatype)).ToList();

        //            #region >>>>> 文件額外資訊
        //            this.DynamicInfo.Add(new ArcPreAttributeModel("Content", _d.fsCONTENT ?? string.Empty, "文件內容"));
        //            this.DynamicInfo.Add(new ArcPreAttributeModel("FileCreatedDate", string.Format($"{_d.fdFILE_CREATED_DATE:yyyy-MM-dd HH:mm:ss}"), "文件建立日期"));
        //            this.DynamicInfo.Add(new ArcPreAttributeModel("FileUpdatedDate", string.Format($"{_d.fdFILE_UPDATED_DATE:yyyy-MM-dd HH:mm:ss}"), "文件修改日期"));
        //            #endregion
        //            break;
        //        default:
        //            //
        //            break;
        //    }

        //    return this;
        //}

        public DeleteArcViewModel ConvertData<T, M>(T data, FileTypeEnum mediatype, M fdata)
        {
            if (data == null) { return this; }

            var properties = typeof(T).GetProperties();
            foreach (var p in properties)
            {
                var val = p.GetValue(data) == null ? string.Empty : p.GetValue(data).ToString();
                string str = string.Concat("^", p.Name);

                if (str.Contains("^fnINDEX_ID"))
                {
                    if (long.TryParse(val, out long idx)) { this.IndexId = idx; }
                }
                if (str.Contains("^fsFILE_NO")) { this.fsFILE_NO = val; }
                //if (str.Contains("^fsTYPE")){ }
                if ("^fsREASON^DeleteReason".IndexOf(str) > -1) { this.DeleteReason = val; }
            }

            var propers = typeof(M).GetProperties();
            ShowViewerModel _showViewer = new ShowViewerModel();
            ArcBasicMetaModel _arcBasicMeta = new ArcBasicMetaModel();
            switch (mediatype)
            {
                case FileTypeEnum.V:
                    this.Viewer = _showViewer.FormatConversion(fdata, mediatype);
                    this.BasicMeta = _arcBasicMeta.FormatConversion(fdata, mediatype);

                    break;
                case FileTypeEnum.A:
                    this.Viewer = _showViewer.FormatConversion(fdata, mediatype);
                    this.BasicMeta = _arcBasicMeta.FormatConversion(fdata, mediatype);

                    #region >>>>> 聲音資訊+專輯資訊
                    foreach (var p in propers)
                    {
                        var val = p.GetValue(fdata) == null ? string.Empty : p.GetValue(fdata).ToString();
                        if (p.Name == "fsALBUM") { this.DynamicInfo.Add(new ArcPreAttributeModel("fsALBUM", val ?? string.Empty, "專輯名稱")); }
                        if (p.Name == "fsALBUM_TITLE") { this.DynamicInfo.Add(new ArcPreAttributeModel("fsALBUM_TITLE", val ?? string.Empty, "專輯標題")); }
                        if (p.Name == "fsALBUM_ARTISTS") { this.DynamicInfo.Add(new ArcPreAttributeModel("fsALBUM_ARTISTS", val ?? string.Empty, "專輯演出者")); }
                        if (p.Name == "fsALBUM_PERFORMERS") { this.DynamicInfo.Add(new ArcPreAttributeModel("fsALBUM_PERFORMERS", val ?? string.Empty, "歌曲演出者")); }
                        if (p.Name == "fsALBUM_COMPOSERS") { this.DynamicInfo.Add(new ArcPreAttributeModel("fsALBUM_COMPOSERS", val ?? string.Empty, "歌曲作曲者")); }
                        if (p.Name == "fnALBUM_YEAR") { this.DynamicInfo.Add(new ArcPreAttributeModel("fnALBUM_YEAR", val ?? string.Empty, "專輯年份")); }
                        if (p.Name == "fsALBUM_COPYRIGHT") { this.DynamicInfo.Add(new ArcPreAttributeModel("fsALBUM_COPYRIGHT", val ?? string.Empty, "著作權")); }
                        if (p.Name == "fsALBUM_GENRES") { this.DynamicInfo.Add(new ArcPreAttributeModel("fsALBUM_GENRES", val ?? string.Empty, "內容類型")); }
                        if (p.Name == "fsALBUM_COMMENT") { this.DynamicInfo.Add(new ArcPreAttributeModel("fsALBUM_COMMENT", val ?? string.Empty, "備註")); }
                    }
                    #endregion
                    break;
                case FileTypeEnum.P:
                    this.Viewer = _showViewer.FormatConversion(fdata, mediatype);
                    this.BasicMeta = _arcBasicMeta.FormatConversion(fdata, mediatype);

                    #region >>>>> 圖片資訊+EXIF資訊
                    foreach (var p in propers)
                    {
                        var val = p.GetValue(fdata) == null ? string.Empty : p.GetValue(fdata).ToString();
                        if (p.Name == "fnWIDTH") { this.DynamicInfo.Add(new ArcPreAttributeModel("fnWIDTH", val ?? string.Empty, "圖片寬")); }
                        if (p.Name == "fnHEIGHT") { this.DynamicInfo.Add(new ArcPreAttributeModel("fnHEIGHT", val ?? string.Empty, "圖片高")); }
                        if (p.Name == "fnXDPI") { this.DynamicInfo.Add(new ArcPreAttributeModel("fnXDPI", val ?? string.Empty, "XDPI")); }
                        if (p.Name == "fnYDPI") { this.DynamicInfo.Add(new ArcPreAttributeModel("fnYDPI", val ?? string.Empty, "YDPI")); }
                        if (p.Name == "fsCAMERA_MAKE") { this.DynamicInfo.Add(new ArcPreAttributeModel("fsCAMERA_MAKE", val ?? string.Empty, "相機廠牌")); }
                        if (p.Name == "fsCAMERA_MODEL") { this.DynamicInfo.Add(new ArcPreAttributeModel("fsCAMERA_MODEL", val ?? string.Empty, "相機型號")); }
                        if (p.Name == "fsFOCAL_LENGTH") { this.DynamicInfo.Add(new ArcPreAttributeModel("fsFOCAL_LENGTH", val ?? string.Empty, "焦距")); }
                        if (p.Name == "fsEXPOSURE_TIME") { this.DynamicInfo.Add(new ArcPreAttributeModel("fsEXPOSURE_TIME", val ?? string.Empty, "曝光時間")); }
                        if (p.Name == "fsAPERTURE") { this.DynamicInfo.Add(new ArcPreAttributeModel("fsAPERTURE", val ?? string.Empty, "光圈")); }
                        if (p.Name == "fnISO") { this.DynamicInfo.Add(new ArcPreAttributeModel("fnISO", val ?? string.Empty, "ISO")); }
                    }
                    #endregion
                    break;
                case FileTypeEnum.D:
                    this.Viewer = _showViewer.FormatConversion(fdata, mediatype);
                    this.BasicMeta = _arcBasicMeta.FormatConversion(fdata, mediatype);

                    #region >>>>> 文件額外資訊
                    foreach (var p in propers)
                    {
                        var val = p.GetValue(fdata) == null ? string.Empty : p.GetValue(fdata).ToString();
                        if (p.Name == "Content") { this.DynamicInfo.Add(new ArcPreAttributeModel("Content", val ?? string.Empty, "文件內容")); }
                        if (p.Name == "FileCreatedDate")
                        {
                            if (DateTime.TryParse(val, out DateTime cdt))
                                this.DynamicInfo.Add(new ArcPreAttributeModel("FileCreatedDate", string.Format($"{cdt:yyyy-MM-dd HH:mm:ss}"), "文件建立日期"));
                        }
                        if (p.Name == "FileUpdatedDate")
                        {
                            if (DateTime.TryParse(val, out DateTime udt))
                                this.DynamicInfo.Add(new ArcPreAttributeModel("FileUpdatedDate", string.Format($"{udt:yyyy-MM-dd HH:mm:ss}"), "文件修改日期"));
                        }
                    }
                    #endregion
                    break;
                default:
                    //
                    break;
            }

            return this;
        }
    }

    /// <summary>
    /// 泛型: 刪除紀錄管理-檢視媒資檔案內容 
    /// </summary>
    /// <typeparam name="T">{刪除紀錄}資料型別 如:<see cref="sp_t_GET_ARC_INDEX_BY_CONDITION_Result"/> </typeparam>
    /// <typeparam name="M">{媒資檔案}資料型別 如:<see cref="sp_t_GET_ARC_VIDEO_Result"/>, <see cref="sp_t_GET_ARC_AUDIO_Result"/>, <see cref="sp_t_GET_ARC_PHOTO_Result"/>, <see cref="sp_t_GET_ARC_DOC_Result"/> </typeparam>
    public class DeleteArcViewModel<T,M> : DeleteArcViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data">來源資料:{刪除紀錄}。資料型別: <typeparamref name="T"/> </param>
        /// <param name="mediatype">媒資檔案類別: V,A,P,D  <see cref="FileTypeEnum"/> </param>
        /// <param name="fdata">資料內容_AUDIO,VIDEO,PHOTO,DOC媒資檔案。資料型別: <typeparamref name="M"/> </param>
        public DeleteArcViewModel(T data, FileTypeEnum mediatype, M fdata)//(T data, FileTypeEnum mediatype, T_tbmARC_IndexService arcIndexSer)
        {
            if (data != null)
            {
                var properties = typeof(T).GetProperties();

                foreach (var p in properties)
                {
                    var val = p.GetValue(data) == null ? string.Empty : p.GetValue(data).ToString();
                    string str = string.Concat("^", p.Name);

                    if (str.Contains("^fnINDEX_ID"))
                    {
                        if(long.TryParse(val, out long idx)) { this.IndexId = idx; }
                    }
                    if (str.Contains("^fsFILE_NO")) { this.fsFILE_NO = val; }
                    //if (str.Contains("^fsTYPE")){ }
                    if ("^fsREASON^DeleteReason".IndexOf(str) > -1) { this.DeleteReason = val; }
                }

                var propers = typeof(M).GetProperties();
                switch (mediatype)
                {
                    case FileTypeEnum.V:
                        this.Viewer = new ShowViewerModel().FormatConversion(fdata, mediatype);
                        this.BasicMeta = new ArcBasicMetaModel().FormatConversion(fdata, mediatype);
                        
                        break;
                    case FileTypeEnum.A:
                        this.Viewer = new ShowViewerModel().FormatConversion(fdata, mediatype);
                        this.BasicMeta = new ArcBasicMetaModel().FormatConversion(fdata, mediatype);

                        #region >>>>> 聲音資訊+專輯資訊
                        foreach (var p in propers)
                        {
                            var val = p.GetValue(fdata) == null ? string.Empty : p.GetValue(fdata).ToString();
                            if (p.Name == "fsALBUM") { this.DynamicInfo.Add(new ArcPreAttributeModel("fsALBUM", val ?? string.Empty, "專輯名稱")); }
                            if (p.Name == "fsALBUM_TITLE") { this.DynamicInfo.Add(new ArcPreAttributeModel("fsALBUM_TITLE", val ?? string.Empty, "專輯標題")); }
                            if (p.Name == "fsALBUM_ARTISTS") { this.DynamicInfo.Add(new ArcPreAttributeModel("fsALBUM_ARTISTS", val ?? string.Empty, "專輯演出者")); }
                            if (p.Name == "fsALBUM_PERFORMERS") { this.DynamicInfo.Add(new ArcPreAttributeModel("fsALBUM_PERFORMERS", val ?? string.Empty, "歌曲演出者")); }
                            if (p.Name == "fsALBUM_COMPOSERS") { this.DynamicInfo.Add(new ArcPreAttributeModel("fsALBUM_COMPOSERS", val ?? string.Empty, "歌曲作曲者")); }
                            if (p.Name == "fnALBUM_YEAR") { this.DynamicInfo.Add(new ArcPreAttributeModel("fnALBUM_YEAR", val ?? string.Empty, "專輯年份")); }
                            if (p.Name == "fsALBUM_COPYRIGHT") { this.DynamicInfo.Add(new ArcPreAttributeModel("fsALBUM_COPYRIGHT", val ?? string.Empty, "著作權")); }
                            if (p.Name == "fsALBUM_GENRES") { this.DynamicInfo.Add(new ArcPreAttributeModel("fsALBUM_GENRES", val ?? string.Empty, "內容類型")); }
                            if (p.Name == "fsALBUM_COMMENT") { this.DynamicInfo.Add(new ArcPreAttributeModel("fsALBUM_COMMENT", val ?? string.Empty, "備註")); }
                        }
                        #endregion
                        break;
                    case FileTypeEnum.P:
                        this.Viewer = new ShowViewerModel().FormatConversion(fdata, mediatype);
                        this.BasicMeta = new ArcBasicMetaModel().FormatConversion(fdata, mediatype);

                        #region >>>>> 圖片資訊+EXIF資訊
                        foreach (var p in propers)
                        {
                            var val = p.GetValue(fdata) == null ? string.Empty : p.GetValue(fdata).ToString();
                            if (p.Name == "fnWIDTH") { this.DynamicInfo.Add(new ArcPreAttributeModel("fnWIDTH", val ?? string.Empty, "圖片寬")); }
                            if (p.Name == "fnHEIGHT") { this.DynamicInfo.Add(new ArcPreAttributeModel("fnHEIGHT", val ?? string.Empty, "圖片高")); }
                            if (p.Name == "fnXDPI") { this.DynamicInfo.Add(new ArcPreAttributeModel("fnXDPI", val ?? string.Empty, "XDPI")); }
                            if (p.Name == "fnYDPI") { this.DynamicInfo.Add(new ArcPreAttributeModel("fnYDPI", val ?? string.Empty, "YDPI")); }
                            if (p.Name == "fsCAMERA_MAKE") { this.DynamicInfo.Add(new ArcPreAttributeModel("fsCAMERA_MAKE", val ?? string.Empty, "相機廠牌")); }
                            if (p.Name == "fsCAMERA_MODEL") { this.DynamicInfo.Add(new ArcPreAttributeModel("fsCAMERA_MODEL", val ?? string.Empty, "相機型號")); }
                            if (p.Name == "fsFOCAL_LENGTH") { this.DynamicInfo.Add(new ArcPreAttributeModel("fsFOCAL_LENGTH", val ?? string.Empty, "焦距")); }
                            if (p.Name == "fsEXPOSURE_TIME") { this.DynamicInfo.Add(new ArcPreAttributeModel("fsEXPOSURE_TIME", val ?? string.Empty, "曝光時間")); }
                            if (p.Name == "fsAPERTURE") { this.DynamicInfo.Add(new ArcPreAttributeModel("fsAPERTURE", val ?? string.Empty, "光圈")); }
                            if (p.Name == "fnISO") { this.DynamicInfo.Add(new ArcPreAttributeModel("fnISO", val ?? string.Empty, "ISO")); }
                        }
                        #endregion
                        break;
                    case FileTypeEnum.D:
                        this.Viewer = new ShowViewerModel().FormatConversion(fdata, mediatype);
                        this.BasicMeta = new ArcBasicMetaModel().FormatConversion(fdata, mediatype);

                        #region >>>>> 文件額外資訊
                        foreach (var p in propers)
                        {
                            var val = p.GetValue(fdata) == null ? string.Empty : p.GetValue(fdata).ToString();
                            if (p.Name == "Content") { this.DynamicInfo.Add(new ArcPreAttributeModel("Content", val ?? string.Empty, "文件內容")); }
                            if (p.Name == "FileCreatedDate")
                            {
                                if (DateTime.TryParse(val, out DateTime cdt))
                                    this.DynamicInfo.Add(new ArcPreAttributeModel("FileCreatedDate", string.Format($"{cdt:yyyy-MM-dd HH:mm:ss}"), "文件建立日期"));
                            }
                            if (p.Name == "FileUpdatedDate")
                            {
                                if (DateTime.TryParse(val, out DateTime udt))
                                    this.DynamicInfo.Add(new ArcPreAttributeModel("FileUpdatedDate", string.Format($"{udt:yyyy-MM-dd HH:mm:ss}"), "文件修改日期"));
                            }
                        }
                        #endregion
                        break;
                    default:
                        //
                        break;
                }
            }
        }

    }
}
