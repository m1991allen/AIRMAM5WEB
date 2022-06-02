using AIRMAM5.DBEntity.Models.ArcPre;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Subject;
using System;
using System.Collections.Generic;

namespace AIRMAM5.DBEntity.Models.Common
{
    /// <summary>
    /// 查看媒資檔案詳細內容 - 共用
    /// <para>1.媒資檔案顯示: player/image </para>
    /// <para>2.媒資基本資料 </para>
    /// <para>3.(影片)關鍵影格 </para>
    /// <para>4.(影片,聲音)段落描述 </para>
    /// <para>5.自訂欄位資訊 </para>
    /// </summary>
    public class MediaDetailsCommonModel
    {
        /// <summary>
        ///  (影音圖文)媒資檔案顯示: player/image
        /// </summary>
        public ShowViewerModel Viewer { get; set; } = new ShowViewerModel();

        /// <summary>
        /// 媒資基本資料
        /// </summary>
        public ArcBasicMetaModel BasicMeta { get; set; } = new ArcBasicMetaModel();

        /// <summary>
        /// (影片)關鍵影格
        /// </summary>
        public List<VideoKeyFrameModel> KeyFrame { get; set; } = new List<VideoKeyFrameModel>();

        /// <summary>
        /// (影片,聲音)段落描述 ParagraphDescribe
        /// </summary>
        public List<SubjectFileSeqmentModel> FileSeqment { get; set; } = new List<SubjectFileSeqmentModel>();

        /// <summary>
        /// 自訂欄位資訊 CustomFields
        /// </summary>
        public List<ArcPreAttributeModel> DynamicInfo { get; set; } = new List<ArcPreAttributeModel>();
        
            /// <summary>
        /// 媒資檔案詳細內容資料
        /// </summary>
        /// <typeparam name="T">來源資料型態 </typeparam>
        /// <param name="data">資料內容, 泛型型別<typeparamref name="T"/> </param>
        /// <param name="mediatype">媒資檔案類別 V,A,P,D <see cref="FileTypeEnum"/> </param>
        /// <returns></returns>
        public MediaDetailsCommonModel ConvertModelData<T>(T data, FileTypeEnum mediatype)
        {
            var propers = typeof(T).GetProperties();

            switch (mediatype)
            {
                case FileTypeEnum.V:
                    //T = spGET_ARC_VIDEO_Result
                    this.Viewer = new ShowViewerModel().FormatConversion(data, mediatype);
                    this.BasicMeta = new ArcBasicMetaModel().FormatConversion(data, mediatype);
                    break;
                case FileTypeEnum.A:
                    //T = spGET_ARC_AUDIO_Result
                    this.Viewer = new ShowViewerModel().FormatConversion(data, mediatype);
                    this.BasicMeta = new ArcBasicMetaModel().FormatConversion(data, mediatype);

                    #region >>>聲音資訊+專輯資訊
                    foreach (var p in propers)
                    {
                        var val = p.GetValue(data) == null ? string.Empty : p.GetValue(data).ToString();
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
                    //T = spGET_ARC_PHOTO_Result
                    this.Viewer = new ShowViewerModel().FormatConversion(data, mediatype);
                    this.BasicMeta = new ArcBasicMetaModel().FormatConversion(data, mediatype);

                    #region >>>圖片資訊+EXIF資訊
                    foreach (var p in propers)
                    {
                        var val = p.GetValue(data) == null ? string.Empty : p.GetValue(data).ToString();
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
                    //T = spGET_ARC_DOC_Result
                    this.Viewer = new ShowViewerModel().FormatConversion(data, mediatype);
                    this.BasicMeta = new ArcBasicMetaModel().FormatConversion(data, mediatype);

                    #region >>>文件額外資訊
                    foreach (var p in propers)
                    {
                        var val = p.GetValue(data) == null ? string.Empty : p.GetValue(data).ToString();
                        if (p.Name == "Content") { this.DynamicInfo.Add(new ArcPreAttributeModel("Content", val ?? string.Empty, "文件內容")); }
                        if (p.Name == "FileCreatedDate")
                        {
                            DateTime.TryParse(val, out DateTime cdt);
                            this.DynamicInfo.Add(new ArcPreAttributeModel("FileCreatedDate", string.Format($"{cdt:yyyy-MM-dd HH:mm:ss}"), "文件建立日期"));
                        }
                        if (p.Name == "FileUpdatedDate")
                        {
                            DateTime.TryParse(val, out DateTime udt);
                            this.DynamicInfo.Add(new ArcPreAttributeModel("FileUpdatedDate", string.Format($"{udt :yyyy-MM-dd HH:mm:ss}"), "文件修改日期"));
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

}
