using AIRMAM5.DBEntity.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Subject
{
    /// <summary>
    /// (影音圖文)點選顯示頁+左下媒體列表,  繼承參考 <see cref="ShowViewerModel"/>
    /// </summary>
    public class ShowListsModel : ShowViewerModel
    {
        /// <summary>
        /// (影音圖文)點選顯示頁+左下媒體列表
        /// </summary>
        public ShowListsModel() { }

        #region >>>>> 欄位參數 
        /// <summary>
        /// 標題
        /// </summary>
        [Display(Name = "標題")]
        public string Title { get; set; } = string.Empty;
        /// <summary>
        /// 描述
        /// </summary>
        [Display(Name = "描述")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 關鍵影格數量 [_sKEYFRAME_COUNT]
        /// </summary>
        public int KeyFrameCount { get; set; } = 0;

        /// <summary>
        /// 段落描述數量 [_sSEGMENT_COUNT]
        /// </summary>
        public int SegmentCount { get; set; } = 0;

        /// <summary>
        /// 是否可置換
        /// </summary>
        public bool IsChange { get; set; } = false;

        /// <summary>
        /// fsDIRECTION 影音預覽方向: 橫向H、直向V
        /// </summary>
        public string fsDIRECTION { get; set; } = FileDirection.H.ToString();

        /// <summary>
        /// 轉檔工作狀態
        /// </summary>
        public string fsSTATUS { get; set; }

        public int fnWIDTH { get; set; } = 0;

        public int fnHEIGHT { get; set; } = 0;
        #endregion
        
        /// <summary>
        /// (影音圖文)點選顯示頁+左下媒體列表, 資料型別<typeparamref name="T"/> 轉換處理
        /// </summary>
        /// <typeparam name="T">資料型別參數 如: <see cref="spGET_ARC_VIDEO_BY_SUBJ_ID_Result"/>,
        ///                 <see cref="spGET_ARC_AUDIO_BY_SUBJ_ID_Result"/>, <see cref="spGET_ARC_PHOTO_BY_SUBJ_ID_Result"/>, 
        ///                 <see cref="spGET_ARC_DOC_BY_SUBJ_ID_Result"/> 預存回傳值 </typeparam>
        /// <param name="m">影音圖文_主題編號_預存_Result  </param>
        /// <param name="fileCategory">媒資檔案類別: V,A,P,D  <see cref="FileTypeEnum"/></param>
        public new ShowListsModel FormatConversion<T>(T m, FileTypeEnum fileCategory)
        {
            this.FileType = fileCategory.ToString();
            var _properties = typeof(T).GetProperties();

            string _status = string.Empty, _filePath = string.Empty;
            foreach (var p in _properties)
            {
                var val = p.GetValue(m) ?? string.Empty;

                /* 預存沒有此欄位: [fsSUBJECT_ID] */
                //if (p.Name == "fsSUBJECT_ID") this.fsSUBJECT_ID = val.ToString();
                if (p.Name == "fsFILE_NO") this.fsFILE_NO = val.ToString();
                if (p.Name == "fsTITLE") this.Title = val.ToString();
                if (p.Name == "fsDESCRIPTION") this.Description = val.ToString();

                //預覽圖
                if (p.Name == "fsHEAD_FRAME_URL" || p.Name == "C_sIMAGE_URL")
                {
                    string _emptyImgUrl = "~/Images/videopreview.png";
                    switch (fileCategory)
                    {
                        case FileTypeEnum.A:
                            _emptyImgUrl = "~/Images/audiopreview.png";
                            break;
                        case FileTypeEnum.P:
                            _emptyImgUrl = "~/Images/imagepreview.png";
                            break;
                        case FileTypeEnum.D:
                            _emptyImgUrl = "~/Images/docpreview.png";
                            break;
                        case FileTypeEnum.V:
                        default:
                            _emptyImgUrl = "~/Images/videopreview.png";
                            break;
                    }
                    this.ImageUrl = string.IsNullOrEmpty(val.ToString()) ? _emptyImgUrl : val.ToString();
                }

                //圖片、文件 無影音檔案URL(for player)
                if (p.Name == "C_sFILE_URL_L") this.FileUrl = val.ToString();
                //
                if (p.Name == "C_sSUBJ_PATH") this.SubjectPath = val.ToString();
                //聲音、圖片、文件 無關鍵影格統計數
                if (p.Name == "C_sKEYFRAME_COUNT")
                {
                    int.TryParse(val.ToString(), out int kf);
                    this.KeyFrameCount = string.IsNullOrEmpty(val.ToString()) ? 0 : kf;// int.Parse(val.ToString());
                }
                //圖片、文件 無段落描述統計數
                if (p.Name == "C_sSEGMENT_COUNT")
                {
                    int.TryParse(val.ToString(), out int cnt);
                    this.SegmentCount = string.IsNullOrEmpty(val.ToString()) ? 0 : cnt;// int.Parse(val.ToString());
                }
                if (p.Name == "C_sCHANGE")
                {
                    this.IsChange = val.ToString().ToUpper() == IsTrueFalseEnum.Y.ToString() ? true : false;
                }
                //實體檔案路徑
                if (p.Name == "fsFILE_PATH" || p.Name == "C_sFILE_PATH_L")
                {
                    this.FilePath = val.ToString();
                    ////檢查檔案是否存在
                    //if (CommonFile.IsExists(this.FileUrl)) this.FilePath = p.GetValue(m).ToString() ?? string.Empty;
                }
                if (p.Name == "fsSTATUS") _status = val.ToString();
                if (p.Name == "fsDIRECTION")
                {
                    this.fsDIRECTION = string.IsNullOrEmpty(val.ToString()) ? FileDirection.H.ToString() : val.ToString();
                }
                if (p.Name == "fnWIDTH")
                {
                    int.TryParse(val.ToString(), out int w);
                    this.fnWIDTH = string.IsNullOrEmpty(val.ToString()) ? 0 : w;// int.Parse(val.ToString());
                }
                if (p.Name == "fnHEIGHT")
                {
                    int.TryParse(val.ToString(), out int h);
                    this.fnHEIGHT = string.IsNullOrEmpty(val.ToString()) ? 0 : h;// int.Parse(val.ToString());
                }
            }

            //Tips_2020.03.11: 判斷實體檔案是否存在,由前端處理
            return this;
        }

    }

}
