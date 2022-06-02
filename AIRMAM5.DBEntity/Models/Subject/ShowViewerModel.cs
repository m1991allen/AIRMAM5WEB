
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Services;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AIRMAM5.DBEntity.Models.Subject
{
    /// <summary>
    /// (影音圖文)點選顯示頁:左上Player資料內容  繼承參考 <see cref="SubjFileNoModel"/>
    /// </summary>
    public class ShowViewerModel : SubjFileNoModel
    {
        /// <summary>
        /// (影音圖文)點選顯示頁:左上Player資料內容
        /// </summary>
        public ShowViewerModel() { }
        
        #region >>>>> 欄位參數 
        /// <summary>
        /// 分類: A聲音, D文件, P圖片, S主題, V影片
        /// </summary>
        [Display(Name = "類別")]
        public string FileType { get; set; } = string.Empty;

        /// <summary>
        /// 檔案URL 如: http://172.20.142.35/AirMAM5/media/P/2020/01/31/20200131_0000062_L.jpg
        /// </summary>
        [Display(Name = "檔案")]
        public string FileUrl { get; set; } = string.Empty;

        /// <summary>
        /// 預覽圖(URL)
        /// </summary>
        [Display(Name = "預覽圖")]
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// 主題檔案Path 路徑字串
        /// </summary>
        [Display(Name = "主題位置")]
        public string SubjectPath { get; set; } = string.Empty;

        /// <summary>
        /// 檔案位置 路徑字串, 如: \\172.20.142.35\media\P\2020\01\31\20200131_0000062_L.jpg
        /// </summary>
        [Display(Name = "檔案位置")]
        public string FilePath { get; set; } = string.Empty;

        //Marked_20201026:直接通知前端,檔案是否可以預覽(open DocViewer)
        ///// <summary>
        ///// 5(DocViewer)可預覽的檔案類型
        ///// </summary>
        //public string PreviewableFileType { get; set; } = string.Empty;

        /// <summary>
        /// (檔案)是否可預覽, Default=false
        /// </summary>
        public bool CanPreview { get; set; } = false;
        #endregion

        /// <summary>
        /// (影音圖文)點選顯示頁:左上Player資料內容, 資料型別<typeparamref name="T"/> 轉換處理
        /// </summary>
        /// <typeparam name="T">資料型別參數 如:  
        ///     <para>1.媒資檔案.預存回傳值: <see cref="spGET_ARC_VIDEO_Result"/>, <see cref="spGET_ARC_AUDIO_Result"/>, 
        ///                 <see cref="spGET_ARC_PHOTO_Result"/>, <see cref="spGET_ARC_DOC_Result"/> </para>
        ///     <para>2.刪除紀錄.預存回傳值: <see cref="sp_t_GET_ARC_VIDEO_Result"/>, <see cref="sp_t_GET_ARC_AUDIO_Result"/>, 
        ///                 <see cref="sp_t_GET_ARC_PHOTO_Result"/>, <see cref="sp_t_GET_ARC_DOC_Result"/> </para>
        ///     <para>3.媒資檔案_SubjId.預存回傳值: <see cref="spGET_ARC_VIDEO_BY_SUBJ_ID_Result"/>, <see cref="spGET_ARC_AUDIO_BY_SUBJ_ID_Result"/>, 
        ///                 <see cref="spGET_ARC_PHOTO_BY_SUBJ_ID_Result"/>, <see cref="spGET_ARC_DOC_BY_SUBJ_ID_Result"/> </para>
        /// </typeparam>
        /// <param name="m">資料來源 </param>
        /// <param name="fileCategory">媒資檔案類別: V,A,P,D  <see cref="FileTypeEnum"/></param>
        public ShowViewerModel FormatConversion<T>(T m, FileTypeEnum fileCategory)
        {
            //類別: A聲音, D文件, P圖片, S主題, V影片
            this.FileType = fileCategory.ToString();

            var _properties = typeof(T).GetProperties();
            foreach (var p in _properties)
            {
                var _val = p.GetValue(m) ?? string.Empty;

                if (p.Name == "fsSUBJECT_ID") this.fsSUBJECT_ID = _val.ToString();
                if (p.Name == "fsFILE_NO") this.fsFILE_NO = _val.ToString();

                //檔案URL : 圖片,文件無影音檔案
                if (p.Name == "C_sFILE_URL_L") this.FileUrl = _val.ToString();
                //預覽圖URL
                if (p.Name == "fsHEAD_FRAME_URL" || p.Name == "fsHEAD_FRAME" || p.Name == "C_sIMAGE_URL")
                {
                    string _img = (fileCategory == FileTypeEnum.V)
                        ? "~/Images/videopreview.png" : (fileCategory == FileTypeEnum.A)
                            ? "~/Images/audiopreview.png" : (fileCategory == FileTypeEnum.P)
                                ? "~/Images/imagepreview.png" : (fileCategory == FileTypeEnum.D)
                                    ? "~/Images/docpreview.png" : string.Empty;

                    this.ImageUrl = string.IsNullOrEmpty(_val.ToString()) ? _img : _val.ToString();
                }
                //主題檔案路徑字串
                if (p.Name == "C_sSUBJ_PATH") this.SubjectPath = _val.ToString();

                //實體檔案 路徑字串
                if (p.Name == "fsFILE_PATH" || p.Name == "C_sFILE_PATH_L")
                {
                    this.FilePath = _val.ToString();
                }
                //Marked_20201026:直接通知前端,檔案是否可以預覽(open DocViewer)
                ////(DocViewer)可預覽的檔案類型
                //this.PreviewableFileType = string.Empty;
                if (p.Name == "fsFILE_TYPE")
                {
                    //"DV_VIEW_FILETYPE" Document Viewer 可預覽的檔案類型
                    var _Previewable = new ConfigService().GetConfigBy("DV_VIEW_FILETYPE").FirstOrDefault();
                    this.CanPreview = (_Previewable == null)
                        ? false : (_Previewable.fsVALUE.IndexOf(_val.ToString()) > -1 ? true : false);
                }
            }

            return this;
        }
    }

}
