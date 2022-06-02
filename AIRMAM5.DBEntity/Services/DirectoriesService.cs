using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Directory;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AIRMAM5.DBEntity.Services
{
    /// <summary>
    /// 系統目錄維護 tbmDIRECTORIES
    /// </summary>
    public class DirectoriesService
    {
        readonly ISerilogService _serilogService;
        protected AIRMAM5DBEntities _db;
        protected VerifyResult result = new VerifyResult(false, "fail");
        private readonly IGenericRepository<tbmDIRECTORIES> _dirRepository = new GenericRepository<tbmDIRECTORIES>();

        public DirectoriesService()
        {
            //this._db = new AIRMAM5DBEntities();
            _serilogService = new SerilogService();
        }
        public DirectoriesService(ISerilogService serilogService)
        {
            _serilogService = serilogService;
        }

        /// <summary>
        /// 依 Parent fnDIR_ID 取回目錄資料 【spGET_DIRECTORIES_LOAD_ON_DEMAND】
        /// </summary>
        /// <param name="param"> 查詢參數(系統目錄編號,系統帳號,關鍵字,是否顯示主檔) <see cref="GetDirLoadOnDemandSearchModel"/> </param>
        /// <remarks>   20200904: 
        ///   spGET_DIRECTORIES_LOAD_ON_DEMAND_ALL 應該是由 spGET_DIRECTORIES_LOAD_ON_DEMAND 衍生而來,
        ///     相似度99%, 差異為有無 tbmDIRECTORIES.[fsSHOWTYPE]欄位的判斷。
        ///     為方便維護, 將spGET_DIRECTORIES_LOAD_ON_DEMAND_ALL 與 spGET_DIRECTORIES_LOAD_ON_DEMAND 整合為可共用的預存程序。
        ///     【新增傳入參數"是否顯示全部" @fbSHOWALL	char(1) = 0】
        /// </remarks>
        public List<spGET_DIRECTORIES_LOAD_ON_DEMAND_Result> GetDirLoadOnDemand(GetDirLoadOnDemandSearchModel param)
        {
            string _showSubject = param.ShowSubJ ? IsTrueFalseEnum.Y.ToString() : string.Empty;

            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_DIRECTORIES_LOAD_ON_DEMAND(
                param.DirId,
                param.UserName ?? string.Empty,
                param.KeyWord ?? string.Empty,
                _showSubject,
                param.IsShowAll).DefaultIfEmpty().ToList();

                return query == null || query.FirstOrDefault() == null ? new List<spGET_DIRECTORIES_LOAD_ON_DEMAND_Result>() : query;
            }
        }

        /// <summary>
        /// 取 子層級項目內容 【spGET_DIRECTORIES_LOAD_ON_DEMAND】
        /// </summary>
        /// <param name="param"> 查詢參數(DirId,UserName,KeyWord,ShowSubJ) </param>
        /// <returns> 清單<see cref="DirectoriesItemModel"/> </returns>
        public List<DirectoriesItemModel> DirSubList(GetDirLoadOnDemandSearchModel param)
        {
            var query = GetDirLoadOnDemand(param)
                .Select(s => new DirectoriesItemModel().FormatConversion(s))
                .ToList();

            using (_db = new AIRMAM5DBEntities())
            {
                query.ForEach(f =>
                    f.ChildrenLength = f.DirType == "Q" ? (_db.tbmSUBJECT.Where(x => x.fnDIR_ID == f.DirId).Count()) : 0
                );
            }

            return query;
        }

        /// <summary>
        /// 取出 與'檔案編號' 相同V/A/P/D 樣板的DIRECTORIES 給檔案搬遷主題用 【spGET_DIRECTORIES_BY_FILE_NO_LOAD_ON_DEMAND】
        /// </summary>
        /// <param name="param"> 查詢參數(FileType,FileNo,DirId,UserName,KeyWord) </param>
        /// <returns></returns>
        public List<spGET_DIRECTORIES_BY_FILE_NO_LOAD_ON_DEMAND_Result> GetDirMatchTemplate(GetDirByFileNoLoadOnDemand param)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_DIRECTORIES_BY_FILE_NO_LOAD_ON_DEMAND(
                param.FileType,
                param.FileNo,
                param.DirId,
                param.UserName ?? string.Empty,
                param.KeyWord ?? string.Empty).ToList();

                if (query == null || query.FirstOrDefault() == null)
                    return new List<spGET_DIRECTORIES_BY_FILE_NO_LOAD_ON_DEMAND_Result>();

                return query;
            }
        }

        /// <summary>
        /// 取出 與'主題編號' 相同V/A/P/D 樣板的DIRECTORIES 給主題搬遷目錄使用 【spGET_DIRECTORIES_BY_SUBJ_ID_LOAD_ON_DEMAND】
        /// <para> ※ 參數(@fsSUBJ_ID,@fnDIR_ID,@fsLOGIN_ID,@fsKEYWORD) </para>
        /// </summary>
        public List<spGET_DIRECTORIES_BY_SUBJ_ID_LOAD_ON_DEMAND_Result> GetDirMatchTemplateBySubj(GetDirBySubjIdLoadOnDemand param)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_DIRECTORIES_BY_SUBJ_ID_LOAD_ON_DEMAND(
                param.SubjId,
                param.DirId,
                param.UserName ?? string.Empty,
                param.KeyWord ?? string.Empty).ToList();

                if (query == null || query.FirstOrDefault() == null)
                    return new List<spGET_DIRECTORIES_BY_SUBJ_ID_LOAD_ON_DEMAND_Result>();

                return query;
            }
        }

        /// <summary>
        /// 單一目錄/節點內容 spGET_DIRECTORIES
        /// </summary>
        /// <param name="dirid"></param>
        /// <returns></returns>
        public spGET_DIRECTORIES_Result GetDirectoriesById(long dirid)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_DIRECTORIES(dirid).FirstOrDefault();

                return query;
            }
        }

        /// <summary>
        /// 單一目錄/節點{G群組/U使用者} 權限資料 spGET_DIRECTORIES_BY_DIR_ID_AUTHORITY
        /// </summary>
        /// <returns></returns>
        public List<spGET_DIRECTORIES_BY_DIR_ID_AUTHORITY_Result> GetDirectoriesAuthById(GetDirAuthoritySearchModel param)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_DIRECTORIES_BY_DIR_ID_AUTHORITY(param.DirId, param.AuthType).DefaultIfEmpty().ToList();

                if (query.FirstOrDefault() == null)
                    return new List<spGET_DIRECTORIES_BY_DIR_ID_AUTHORITY_Result>();

                return query;
            }
        }

        /// <summary>
        /// 取 DIRECTORIES主檔資料其主題檔 BY DIR_ID 【spGET_DIRECTORIES_AND_SUBJECTS_BY_DIR_ID】
        /// </summary>
        /// <param name="dirid"> 目錄編號 fnDIR_ID </param>
        /// <returns></returns>
        public List<spGET_DIRECTORIES_AND_SUBJECTS_BY_DIR_ID_Result> GetDirSubjectsById(long dirid)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_DIRECTORIES_AND_SUBJECTS_BY_DIR_ID(dirid).DefaultIfEmpty().ToList();

                if (query.FirstOrDefault() == null)
                    return new List<spGET_DIRECTORIES_AND_SUBJECTS_BY_DIR_ID_Result>();

                return query;
            }
        }

        /// <summary>
        /// 取 DIRECTORIES 主檔資料其主題檔 BY DIR_ID 【spGET_DIRECTORIES_AND_SUBJECTS_BY_DIR_ID_Filter】
        /// <para> 適用於功能'檔案搬遷', 當系統不啟用目錄節點Queue時, 選擇目的地目錄後、列出目錄主題資料時, 叫用此預存程序, 要再對計移動檔的的樣板再比對一次。
        /// </para>
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<spGET_DIRECTORIES_AND_SUBJECTS_BY_DIR_ID_FILTER_Result> GetDirSubjectsByIdFilter(GetDirAndSubjectsByDirFilter param)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_DIRECTORIES_AND_SUBJECTS_BY_DIR_ID_FILTER(param.DirId, param.FileType, param.FileNo).DefaultIfEmpty().ToList();

                if (query.FirstOrDefault() == null)
                    return new List<spGET_DIRECTORIES_AND_SUBJECTS_BY_DIR_ID_FILTER_Result>();

                return query;
            }
        }

        #region ===== EF =====
        /// <summary>
        /// 目錄Id取資料表資料
        /// </summary>
        /// <returns></returns>
        public tbmDIRECTORIES GetDirById(long id)
        {
            return _dirRepository.Get(x => x.fnDIR_ID == id);
        }

        /// <summary>
        /// 取 單一目錄/節點內容 By 主題編號 fsSUBJ_ID 
        /// </summary>
        /// <param name="subjid">主題編號 [fsSUBJ_ID] </param>
        /// <returns></returns>
        public tbmDIRECTORIES GetDirBySubjId(string subjid)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = (from a in _db.tbmSUBJECT.Where(x => x.fsSUBJ_ID == subjid)
                             join b in _db.tbmDIRECTORIES on a.fnDIR_ID equals b.fnDIR_ID
                             where a.fsSUBJ_ID == subjid
                             select b);

                return query.Any() ? query.FirstOrDefault() : null;
            }
        }
        #endregion

        #region ===== DIR_ID/SUBJ_ID/FILE_no 與 LOGIN ID 取出該使用者針對該目錄有哪些權限 =====
        /// <summary>
        /// 取 使用者對於指定目錄ID 的使用權限(V,I,U,D,B) 【spGET_DIRECTORIES_BY_DIR_ID_AUTHORITY_LOGIN_ID】
        /// </summary>
        /// <param name="dirid"> 目錄編號 fnDIR_ID </param>
        /// <param name="loginid"> 使用者帳號 </param>
        /// <returns></returns>
        //public List<spGET_DIRECTORIES_BY_DIR_ID_AUTHORITY_LOGIN_ID_Result> GetDirAuthByLoginId(long dirid, string loginid)
        public List<UserUseDirAuthModel> GetDirAuthByLoginId(long dirid, string loginid)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                //Tips: 使用者會有兩筆資料狀況(群組G、使用者U)，要將兩筆權限欄位(LIMIT_SUBJECT,LIMIT_VIDEO,LIMIT_AUDIO,LIMIT_PHOTO,LIMIT_DOC)做聯集。
                var query = _db.spGET_DIRECTORIES_BY_DIR_ID_AUTHORITY_LOGIN_ID(dirid, loginid).DefaultIfEmpty().ToList();
                if (query == null || query.FirstOrDefault() == null)
                    return new List<UserUseDirAuthModel>();

                var qry = query.GroupBy(g => g.C_sDIR_PATH).ToList()
                    .Select(s => new UserUseDirAuthModel
                    {
                        DirId = dirid,
                        LoginId = loginid,
                        LimitSubject = string.Join(",", string.Join(",", s.Select(i => i.LIMIT_SUBJECT)).Split(new char[] { ',', ';' }).ToList().Distinct()),
                        LimitVideo = string.Join(",", string.Join(",", s.Select(i => i.LIMIT_VIDEO)).Split(new char[] { ',', ';' }).ToList().Distinct()),
                        LimitAudio = string.Join(",", string.Join(",", s.Select(i => i.LIMIT_AUDIO)).Split(new char[] { ',', ';' }).ToList().Distinct()),
                        LimitPhoto = string.Join(",", string.Join(",", s.Select(i => i.LIMIT_PHOTO)).Split(new char[] { ',', ';' }).ToList().Distinct()),
                        LimitDoc = string.Join(",", string.Join(",", s.Select(i => i.LIMIT_DOC)).Split(new char[] { ',', ';' }).ToList().Distinct()),
                    }).ToList();

                return qry;//query;
            }
            /*
             * LIMIT_SUBJECT: V
             * LIMIT_VIDEO: V,I,U,D,B
             * LIMIT_AUDIO: V,I,U,D,B
             * LIMIT_PHOTO: V,I,U,D,B
             * LIMIT_DOC: V,I,U,D,B
             * -----------------------------------------
             * V= 檢視資料,
             * I= 新增、上傳、置換,重轉
             * U= 修改、批次修改,
             * D= 刪除,
             * B= 可借調
             * */
        }

        /// <summary>
        /// 使用者對於主題編號 的使用權限(V,I,U,D,B) 【spGET_DIRECTORIES_BY_SUBJ_ID_LOGIN_ID_AUTHORITY】
        /// </summary>
        /// <param name="subjid"></param>
        /// <param name="loginid"></param>
        /// <returns></returns>
        //public List<spGET_DIRECTORIES_BY_SUBJ_ID_LOGIN_ID_AUTHORITY_Result> GetSubjectAuthByLoginId(string subjid, string loginid)
        public List<UserUseDirAuthModel> GetSubjectAuthByLoginId(string subjid, string loginid)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_DIRECTORIES_BY_SUBJ_ID_LOGIN_ID_AUTHORITY(loginid, subjid).DefaultIfEmpty().ToList();
                if (query == null || query.FirstOrDefault() == null)
                    return new List<UserUseDirAuthModel>();

                //Tips: 使用者會有兩筆資料狀況(群組G、使用者U)，要將兩筆權限欄位(LIMIT_SUBJECT,LIMIT_VIDEO,LIMIT_AUDIO,LIMIT_PHOTO,LIMIT_DOC)做聯集
                var qry = query.GroupBy(g => g.C_sDIR_PATH).ToList()
                    .Select(s => new UserUseDirAuthModel
                    {
                        DirId = s.FirstOrDefault().fnDIR_ID ?? 0,
                        SubjectId = subjid,
                        FileNo = string.Empty,
                        LoginId = loginid,
                        LimitSubject = string.Join(",", string.Join(",", s.Select(i => i.LIMIT_SUBJECT)).Split(new char[] { ',', ';' }).ToList().Distinct()),
                        LimitVideo = string.Join(",", string.Join(",", s.Select(i => i.LIMIT_VIDEO)).Split(new char[] { ',', ';' }).ToList().Distinct()),
                        LimitAudio = string.Join(",", string.Join(",", s.Select(i => i.LIMIT_AUDIO)).Split(new char[] { ',', ';' }).ToList().Distinct()),
                        LimitPhoto = string.Join(",", string.Join(",", s.Select(i => i.LIMIT_PHOTO)).Split(new char[] { ',', ';' }).ToList().Distinct()),
                        LimitDoc = string.Join(",", string.Join(",", s.Select(i => i.LIMIT_DOC)).Split(new char[] { ',', ';' }).ToList().Distinct()),
                    }).ToList();

                return qry;//query;
            }
        }
        /// <summary>
        /// 使用者對於檔案編號 的使用權限(V,I,U,D,B) 【spGET_DIRECTORIES_BY_FILE_ID_LOGIN_ID_AUTHORITY】
        /// </summary>
        /// <param name="fileno"></param>
        /// <param name="loginid"></param>
        /// <returns></returns>
        //public List<spGET_DIRECTORIES_BY_FILE_ID_LOGIN_ID_AUTHORITY_Result> GetFilenoAuthByLoginId(string fileno, string loginid)
        public List<UserUseDirAuthModel> GetFilenoAuthByLoginId(string fileno, string loginid)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_DIRECTORIES_BY_FILE_ID_LOGIN_ID_AUTHORITY(loginid, fileno).DefaultIfEmpty().ToList();
                if (query == null)
                    return new List<UserUseDirAuthModel>();

                //Tips: 會有兩筆資料狀況(群組G、使用者U)，要將兩筆權限欄位(LIMIT_SUBJECT,LIMIT_VIDEO,LIMIT_AUDIO,LIMIT_PHOTO,LIMIT_DOC)做聯集
                var qry = query.GroupBy(g => g.C_sDIR_PATH).ToList()
                    .Select(s => new UserUseDirAuthModel
                    {
                        DirId = s.FirstOrDefault().fnDIR_ID ?? 0,
                        SubjectId = string.Empty,
                        FileNo = fileno,
                        LoginId = loginid,
                        LimitSubject = string.Join(",", string.Join(",", s.Select(i => i.LIMIT_SUBJECT)).Split(new char[] { ',', ';' }).ToList().Distinct()),
                        LimitVideo = string.Join(",", string.Join(",", s.Select(i => i.LIMIT_VIDEO)).Split(new char[] { ',', ';' }).ToList().Distinct()),
                        LimitAudio = string.Join(",", string.Join(",", s.Select(i => i.LIMIT_AUDIO)).Split(new char[] { ',', ';' }).ToList().Distinct()),
                        LimitPhoto = string.Join(",", string.Join(",", s.Select(i => i.LIMIT_PHOTO)).Split(new char[] { ',', ';' }).ToList().Distinct()),
                        LimitDoc = string.Join(",", string.Join(",", s.Select(i => i.LIMIT_DOC)).Split(new char[] { ',', ';' }).ToList().Distinct()),
                    }).ToList();

                return qry;//query;
            }
        }
        #endregion

        #region ===== CUD 系統目錄 新、修、刪 =====
        /// <summary>
        /// 樹狀結構移動節點 更新存檔 : EXECUTE dbo.spUPDATE_DIR_BY_PARENT_ID
        /// </summary>
        /// <param name="dirid">目錄編號 fnDIR_ID </param>
        /// <param name="parentid">目錄父層編號 fnPARENT_ID </param>
        /// <param name="updateby">異動帳號 fsLOGIN_ID </param>
        /// <returns></returns>
        public VerifyResult MoveNodeUpdate(long dirid, long parentid, string updateby)
        {
            try
            {
                using (_db = new AIRMAM5DBEntities())
                {
                    var _exec = _db.spUPDATE_DIR_BY_PARENT_ID(dirid, parentid, updateby).FirstOrDefault();

                    if (_exec.IndexOf("ERROR") == -1)
                    {
                        result.IsSuccess = true;
                        result.Message = "移動節點成功.";
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = "移動節點失敗";
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = string.Format($"樹狀結構移動節點失敗. {ex.Message}");
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "DirectoriesService",
                    Method = "[MoveNodeUpdate]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "Exception",
                    ErrorMessage = string.Format($"樹狀結構移動節點失敗. {ex.Message}")
                });
                #endregion
            }

            return result;
        }


        /// <summary>
        /// 新增 系統目錄 tbmDIRECTORIES: 【EF Create】
        /// </summary>
        /// <returns></returns>
        public VerifyResult CreateBy(tbmDIRECTORIES inst)
        {
            try
            {
                //#region -- spINSERT_DIRECTORIES --✘✘✘
                _dirRepository.Create(inst);

                result.IsSuccess = true;
                result.Message = "系統目錄已建立.";
                result.Data = inst;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = string.Format($"系統目錄新增失敗【{ex.Message}】");
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "DirectoriesService",
                    Method = "[CreateBy]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "Exception",
                    ErrorMessage = string.Format($"系統目錄新增失敗.\n {ex.Message}")
                });
                #endregion
            }

            return result;
        }
        /// <summary>
        /// 修改 系統目錄 tbmDIRECTORIES: 【EF Update】
        /// </summary>
        /// <returns></returns>
        public VerifyResult UpdateBy(tbmDIRECTORIES inst)
        {
            try
            {
                //#region -- spUPDATE_DIRECTORIES --✘✘✘
                _dirRepository.Update(inst);

                result.IsSuccess = true;
                result.Message = "系統目錄已更新.";
                var query = this.GetDirectoriesById(inst.fnDIR_ID);
                result.Data = query;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = string.Format($"系統目錄修改失敗【{ex.Message}】");
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "DirectoriesService",
                    Method = "[UpdateBy]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "Exception",
                    ErrorMessage = string.Format($"系統目錄修改失敗.\n {ex.Message}")
                });
                #endregion
            }
            return result;
        }

        /// <summary>
        /// 刪除 系統目錄 tbmDIRECTORIES: 【spDELETE_DIRECTORIES】
        /// </summary>
        /// <param name="dirid"></param>
        /// <param name="deleteby"></param>
        /// <returns></returns>
        public VerifyResult DeleteBy(long dirid, string deleteby)
        {
            try
            {
                using (_db = new AIRMAM5DBEntities())
                {
                    #region -- spDELETE_DIRECTORIES --
                    var _exec = _db.spDELETE_DIRECTORIES(dirid, deleteby).FirstOrDefault();
                    if (_exec.IndexOf("ERROR") == -1)
                    {
                        result.IsSuccess = true;
                        result.Message = "系統目錄已刪除.";
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = string.Format($"系統目錄刪除失敗【{_exec.Split(':')[1]}】");
                    }
                    #endregion
                }
                //_dirRepository.Update(inst);
                //result = new VerifyResult(true, "系統目錄已刪除.");
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = string.Format($"系統目錄刪除失敗【{ex.Message}】");
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "DirectoriesService",
                    Method = "[DeleteBy]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "Exception",
                    ErrorMessage = string.Format($"系統目錄刪除失敗.\n {ex.Message}")
                });
                #endregion
            }

            return result;
        }
        #endregion
    }
}
