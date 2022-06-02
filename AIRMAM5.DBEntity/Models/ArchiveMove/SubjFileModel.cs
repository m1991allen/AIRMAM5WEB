using AIRMAM5.DBEntity.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.ArchiveMove
{
    /// <summary>
    /// 主題的檔案資料 Model
    /// </summary>
    public class SubjFileModel
    {
        /// <summary>
        /// 主題的檔案資料 Model
        /// </summary>
        public SubjFileModel() { }

        #region >>>>> 欄位參數 
        /// <summary>
        /// 分類: A聲音, D文件, P圖片, S主題, V影片
        /// </summary>
        [Display(Name = "媒資類別")]
        public string FileType { get; set; } = string.Empty;

        /// <summary>
        /// 檔案編號 fsFILE_NO
        /// </summary>
        public string FileNo { set; get; } = string.Empty;

        /// <summary>
        /// 檔案標題 fsTITLE
        /// </summary>
        public string FileTitle { get; set; } = string.Empty;

        /// <summary>
        /// 預覽圖(URL)
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// 樣版編號 fnTEMP_ID
        /// </summary>
        public int TempId { get; set; }
        #endregion

        /// <summary>
        /// 主題的檔案資料 , 資料型別<typeparamref name="T"/> 轉換處理
        /// </summary>
        /// <typeparam name="T">資料型別參數 如: <see cref="spGET_ARC_VIDEO_BY_SUBJ_ID_Result"/>,
        ///     <see cref="spGET_ARC_AUDIO_BY_SUBJ_ID_Result"/>, 
        ///     <see cref="spGET_ARC_PHOTO_BY_SUBJ_ID_Result"/>, 
        ///     <see cref="spGET_ARC_DOC_BY_SUBJ_ID_Result"/> </typeparam>
        /// <param name="m">資料來源 <typeparamref name="T"/></param>
        /// <param name="fileCategory">媒資檔案類別: V,A,P,D </param>
        /// <returns></returns>
        public SubjFileModel DataConvert<T>(T m, FileTypeEnum fileCategory)
        {
            //類別: A聲音, D文件, P圖片, S主題, V影片
            //SubjFileModel obj = new SubjFileModel { FileType = fileCategory.ToString() };
            this.FileType = fileCategory.ToString();

            var _properties = typeof(T).GetProperties();
            foreach (var p in _properties)
            {
                var _val = p.GetValue(m) ?? string.Empty;

                if (p.Name == "fsFILE_NO") this.FileNo = _val.ToString();
                if (p.Name == "fsTITLE") this.FileTitle = _val.ToString();
                //if (p.Name == "fsDESCRIPTION")

                //預覽圖URL: C_sIMAGE_URL
                if (p.Name == "C_sIMAGE_URL")
                {
                    string _img = (fileCategory == FileTypeEnum.V)
                        ? "~/Images/videopreview.png" : (fileCategory == FileTypeEnum.A)
                            ? "~/Images/audiopreview.png" : (fileCategory == FileTypeEnum.P)
                                ? "~/Images/imagepreview.png" : (fileCategory == FileTypeEnum.D)
                                    ? "~/Images/docpreview.png" : string.Empty;

                    this.ImageUrl = string.IsNullOrEmpty(_val.ToString()) ? _img : _val.ToString();
                }

                //C_sFILE_URL_L
                //C_sSUBJ_PATH
                //C_sKEYFRAME_COUNT
                //C_sSEGMENT_COUNT
                //C_sCHANGE
                //C_sFILE_PATH_L
                //fsSTATUS
                //fsDIRECTION
                //fnWIDTH
                //fnHEIGHT
            }

            return this;
        }
    }

}
