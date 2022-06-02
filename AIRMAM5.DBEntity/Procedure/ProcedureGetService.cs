using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Procedure;
using AIRMAM5.DBEntity.Models.Search;
using AIRMAM5.DBEntity.Models.SearchResponse;
using AIRMAM5.DBEntity.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AIRMAM5.DBEntity.Procedure
{
    /// <summary>
    /// DB Procedure : spGET_xxxxxxxxx
    /// </summary>
    public class ProcedureGetService
    {
        protected AIRMAM5DBEntities _db;
        readonly SerilogService _serilogService = new SerilogService();

        public ProcedureGetService()
        {
            _db = new AIRMAM5DBEntities();
        }

        /// <summary>
        /// 查詢指定期間 異料異動紀錄(影 音 圖 文 主題 系統目錄)  【dbo.dbo.spGET_L_TRAN_BY_DATES】
        /// </summary>
        /// <param name="sdate">開始日期 yyyy/MM/dd </param>
        /// <param name="edate">結束日期 yyyy/MM/dd </param>
        /// <returns></returns>
        public List<spGET_L_TRAN_BY_DATES_Result> GetLTranBy(string sdate, string edate)
        {
            var query = _db.spGET_L_TRAN_BY_DATES(sdate, edate).DefaultIfEmpty().ToList();
            if (query == null || query.FirstOrDefault() == null) return new List<spGET_L_TRAN_BY_DATES_Result>();

            return query;
        }

        ///// <summary>
        ///// 取 tbmCONFIG 指定key資料  【dbo.spGET_CONFIG_Result】
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //public List<spGET_CONFIG_Result> GetConfigBy(string key)
        //{
        //    return _db.spGET_CONFIG(key).DefaultIfEmpty().ToList();
        //}

        /// <summary>
        /// 取 主題檔案 路徑字串 【dbo.spGET_SUBJ_PATH_BY_SUBJECT_ID】
        /// </summary>
        /// <param name="subjectid"></param>
        /// <returns></returns>
        public string GetSubjectPath(string subjectid)
        {
            var query = _db.spGET_SUBJ_PATH_BY_SUBJECT_ID(subjectid).FirstOrDefault();
            if(query == null) return string.Empty;

            return query.ToString();
        }

        #region 【檢索相關】預存程序
        /// <summary>
        /// 熱搜關鍵字_TOP10 【dbo.spGET_L_SRH_HOT_KW】
        /// </summary>
        /// <param name="top">指定取回資料筆數, 預設=0 取回全部。</param>
        /// <returns></returns>
        public List<spGET_L_SRH_HOT_KW_Result> GetHotKW(int top = 0)
        {
            var query = _db.spGET_L_SRH_HOT_KW().DefaultIfEmpty().ToList();
            if (query == null || query.FirstOrDefault() == null)
                return new List<spGET_L_SRH_HOT_KW_Result>();

            //取回指定筆數
            if (top > 0) query.Take(top);

            return query;
        }

        /// <summary>
        /// 依 多筆檔案篇號 顯示 {VIDEO/AUIDO/PHOTO/DOC}檢索結果列表資料內容(預存程序)
        /// </summary>
        /// <param name="searchtype">檢索分類列舉值 <see cref="SearchTypeEnum"/></param>
        /// <param name="metadata">檢索後符合的檔案編號列表 <see cref="SearchMetaResponseModel"/></param>
        ///// <param name="t">指定執行的預存程序_Result 模型 </param>
        ///// <param name="filenos">檔案編號[fsFILENO] 多筆以逗號(,為分隔符號) </param>
        //public List<GetArcSearchResult> GetArcSearchTList(SearchTypeEnum searchtype, List<SearchMetaResponseModel> metadata)
        public Object GetArcSearchTList(SearchTypeEnum searchtype, List<SearchMetaResponseModel> metadata)
        {
            Object rtn = new object();//List<GetArcSearchResult> query = new List<GetArcSearchResult>();

            using (var dBEntities = new AIRMAM5DBEntities())
            {
                string filenos = string.Join(",", metadata.Select(s => s.fsFILE_NO));
                switch (searchtype)
                {
                    case SearchTypeEnum.Video_DEV:
                        var get = this.GetVideoSearchByFileno(filenos);
                        rtn = (from a in get
                               join b in metadata on a.fsFILE_NO equals b.fsFILE_NO into obj
                               from bb in obj.DefaultIfEmpty()
                               select new GetArcSearchResult() {
                                   fsMATCH = bb == null ? string.Empty : bb.fsMATCH,
                                   SearchType = FileTypeEnum.V.ToString()
                               }
                                 .FormatConversion(a, searchtype))
                                 .ToList();
                        break;
                    case SearchTypeEnum.Audio_DEV:
                        var geta = this.GetAudioSearchByFileno(filenos);
                        rtn = (from a in geta
                               join b in metadata on a.fsFILE_NO equals b.fsFILE_NO into obj
                               from bb in obj.DefaultIfEmpty()
                               select new GetArcSearchResult(){ fsMATCH = bb == null ? string.Empty : bb.fsMATCH }
                                .FormatConversion(a, searchtype))
                                .ToList();
                        break;
                    case SearchTypeEnum.Photo_DEV:
                        var getp = this.GetPhotoSearchByFileno(filenos);
                        rtn = (from a in getp
                               join b in metadata on a.fsFILE_NO equals b.fsFILE_NO into obj
                               from bb in obj.DefaultIfEmpty()
                               select new GetArcSearchResult{ fsMATCH = bb == null ? string.Empty : bb.fsMATCH }
                               .FormatConversion(a, searchtype))
                               .ToList();
                        break;
                    case SearchTypeEnum.Doc_DEV:
                        var getd = this.GetDocSearchByFileno(filenos);
                        rtn = (from a in getd
                               join b in metadata on a.fsFILE_NO equals b.fsFILE_NO into obj
                               from bb in obj.DefaultIfEmpty()
                               select new GetArcSearchResult { fsMATCH = bb == null ? string.Empty : bb.fsMATCH }
                               .FormatConversion(a, searchtype))
                               .ToList();
                        break;
                    default:
                        //
                        break;
                }
            }
            return rtn;
        } 

        /// <summary>
        /// 批次取出轉成TSM PATH的高解路徑
        /// </summary>
        /// <param name="filenos">檔案編號[fsFILENO] </param>
        public List<spTSM_GET_FILE_UNCPATH_TO_TSMPATH_Result> GetTSMFilePath(List<string> filenos)
        {
            var result = new List<spTSM_GET_FILE_UNCPATH_TO_TSMPATH_Result>();
            using (var dBEntities = new AIRMAM5DBEntities())
            {
                string _fno = string.Join(",", filenos);
                var query = dBEntities.spTSM_GET_FILE_UNCPATH_TO_TSMPATH(_fno).ToList();
                if (query == null && query.FirstOrDefault() == null) return result;

                result = query;
            }

            return result;
        }
        
        /// <summary>
        /// 依 多筆檔案篇號 顯示 VIDEO檢索結果列表資料內容 【spGET_ARC_VIDEO_SEARCH_BY_FILE_NOS】
        /// </summary>
        /// <param name="filenos"></param>
        /// <returns></returns>
        public List<spGET_ARC_VIDEO_SEARCH_BY_FILE_NOS_Result> GetVideoSearchByFileno(string filenos)
        {
            var query = _db.spGET_ARC_VIDEO_SEARCH_BY_FILE_NOS(filenos).DefaultIfEmpty().ToList();
            if (query == null && query.FirstOrDefault() == null)
                return new List<spGET_ARC_VIDEO_SEARCH_BY_FILE_NOS_Result>();

            return query;
        }
        
        /// <summary>
        /// 依 多筆檔案篇號 顯示 AUDIO檢索結果列表資料內容 【spGET_ARC_AUDIO_SEARCH_BY_FILE_NOS】
        /// </summary>
        /// <param name="filenos"></param>
        /// <returns></returns>
        public List<spGET_ARC_AUDIO_SEARCH_BY_FILE_NOS_Result> GetAudioSearchByFileno(string filenos)
        {
            var query = _db.spGET_ARC_AUDIO_SEARCH_BY_FILE_NOS(filenos).DefaultIfEmpty().ToList();
            if (query == null && query.FirstOrDefault() == null)
                return new List<spGET_ARC_AUDIO_SEARCH_BY_FILE_NOS_Result>();

            return query;
        }
        
        /// <summary>
        /// 依 多筆檔案篇號 顯示 PHOTO檢索結果列表資料內容 【spGET_ARC_PHOTO_SEARCH_BY_FILE_NOS】
        /// </summary>
        /// <param name="filenos"></param>
        /// <returns></returns>
        public List<spGET_ARC_PHOTO_SEARCH_BY_FILE_NOS_Result> GetPhotoSearchByFileno(string filenos)
        {
            var query = _db.spGET_ARC_PHOTO_SEARCH_BY_FILE_NOS(filenos).DefaultIfEmpty().ToList();
            if (query == null && query.FirstOrDefault() == null)
                return new List<spGET_ARC_PHOTO_SEARCH_BY_FILE_NOS_Result>();

            return query;
        }
        
        /// <summary>
        /// 依 多筆檔案篇號 顯示 DOC檢索結果列表資料內容 【spGET_ARC_DOC_SEARCH_BY_FILE_NOS】
        /// </summary>
        /// <param name="filenos"></param>
        /// <returns></returns>
        public List<spGET_ARC_DOC_SEARCH_BY_FILE_NOS_Result> GetDocSearchByFileno(string filenos)
        {
            var query = _db.spGET_ARC_DOC_SEARCH_BY_FILE_NOS(filenos).DefaultIfEmpty().ToList();
            if (query == null && query.FirstOrDefault() == null)
                return new List<spGET_ARC_DOC_SEARCH_BY_FILE_NOS_Result>();

            return query;
        }
        #endregion

        #region 【檢索結果匯出】預存程序
        /// <summary>
        /// 檢索結果匯出: csv內容字串 list
        /// </summary>
        /// <param name="header">是否顯示標題 </param>
        /// <param name="tempid">樣板ID </param>
        /// <param name="filenos">檔案編號 </param>
        /// <param name="mediatype">媒體類型 V,A,P,D </param>
        /// <returns></returns>
        public List<spRPT_GET_SEARCH_EXPORT_DATA_Result> GetSearchExportData(bool header, int tempid, string filenos, string mediatype)
        {
            var query = _db.spRPT_GET_SEARCH_EXPORT_DATA(true, tempid, filenos, mediatype).ToList();

            query = (query == null || query.Count() == 0) ? new List<spRPT_GET_SEARCH_EXPORT_DATA_Result>() : query;

            return query;
        }
        #endregion

        /// <summary>
        /// 帳號對 檔案編號權限 【dbo.spGET_DIRECTORIES_BY_FILE_ID_LOGIN_ID_AUTHORITY】
        ///   (1)加入借調查權限。
        /// </summary>
        /// <param name="loginid"></param>
        /// <param name="fileno"></param>
        /// <returns></returns>
        public List<spGET_DIRECTORIES_BY_FILE_ID_LOGIN_ID_AUTHORITY_Result> GetDirFileNoAuthorityByUser(string loginid, string fileno)
        {
            var query = _db.spGET_DIRECTORIES_BY_FILE_ID_LOGIN_ID_AUTHORITY(loginid, fileno).DefaultIfEmpty().ToList();
            if (query == null || query.FirstOrDefault() == null)
                return new List<spGET_DIRECTORIES_BY_FILE_ID_LOGIN_ID_AUTHORITY_Result>();

            return query ?? null;
        }
        
        /// <summary>
        /// 帳號對 檔案編號權限(需指定檔案類型:V,A,P,D) 【spGET_DIRECTORIES_BY_FILE_ID_LOGIN_ID_AUTHORITY】
        /// </summary>
        /// <param name="loginid"></param>
        /// <param name="fileno"></param>
        /// <param name="filetype">借調檔案類型 tbzCODE.MTRL001= V,A,P,D </param>
        /// <returns></returns>
        public string GetDirFileNoAuthorityByUser(string loginid, string fileno, string filetype)
        {
            string result = string.Empty;
            var query = _db.spGET_DIRECTORIES_BY_FILE_ID_LOGIN_ID_AUTHORITY(loginid, fileno).DefaultIfEmpty().ToList();
            if (query == null || query.FirstOrDefault() == null) return result;

            //20210712_預存回傳多筆!
            FileTypeEnum _ftype = (FileTypeEnum)Enum.Parse(typeof(FileTypeEnum), filetype);
            switch (_ftype)
            {
                case FileTypeEnum.V:
                    //result = query.FirstOrDefault().LIMIT_VIDEO;
                    query.ForEach(f => {
                        result = result.Length > 0 ? string.Concat(result, "^", f.LIMIT_VIDEO) : f.LIMIT_VIDEO;
                    });
                    break;
                case FileTypeEnum.A:
                    //result = query.FirstOrDefault().LIMIT_AUDIO;
                    query.ForEach(f => {
                        result = result.Length > 0 ? string.Concat(result, "^", f.LIMIT_AUDIO) : f.LIMIT_AUDIO;
                    });
                    break;
                case FileTypeEnum.P:
                    //result = query.FirstOrDefault().LIMIT_PHOTO;
                    query.ForEach(f => {
                        result = result.Length > 0 ? string.Concat(result, "^", f.LIMIT_PHOTO) : f.LIMIT_PHOTO;
                    });
                    break;
                case FileTypeEnum.D:
                    //result = query.FirstOrDefault().LIMIT_DOC;
                    query.ForEach(f => {
                        result = result.Length > 0 ? string.Concat(result, "^", f.LIMIT_DOC) : f.LIMIT_DOC;
                    });
                    break;
                case FileTypeEnum.S:
                    //result = query.FirstOrDefault().LIMIT_SUBJECT;
                    break;
                default:
                    break;
            }

            return result;
        }

        #region TSM
        /// <summary>
        /// 取得待上架磁帶清單 【spTSM_GET_L_WAIT_VOL_ACTIVE_ALL】
        /// </summary>
        public List<spTSM_GET_L_WAIT_VOL_ACTIVE_ALL_Result> GetPendingTapes()
        {
            var query = _db.spTSM_GET_L_WAIT_VOL_ACTIVE_ALL().DefaultIfEmpty().ToList();
            if (query == null || query.FirstOrDefault() == null)
                return new List<spTSM_GET_L_WAIT_VOL_ACTIVE_ALL_Result>();

            return query ?? null;
        }
        #endregion

        
        #region >>> 擴充功能:新聞文稿,公文系統
        /// <summary>
        /// 主題與檔案維護.擴充功能{新聞文稿} 資料查詢
        /// </summary>
        /// <param name="type">對應表類型 dbo.tbmCOLUMN_MAPPING.fsTYPE, i.g.INEWS、CONTRACT </param>
        /// <returns></returns>
        public List<spGET_INEWS_Result> SubjExtendGetINews(Get_INews_Param param)
        {
            List<spGET_INEWS_Result> list = new List<spGET_INEWS_Result>();
            using (_db = new AIRMAM5DBEntities())
            {
                list = _db.spGET_INEWS(param.Columns, param.Values).DefaultIfEmpty().ToList();
            }

            return list;
        }
        #endregion
    }
}
