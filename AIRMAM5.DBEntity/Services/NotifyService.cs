using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Models.Notify;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AIRMAM5.DBEntity.Services
{
    /// <summary>
    /// tbmNOTIFY, tbmUSER_NOTIFY
    /// </summary>
    public class NotifyService
    {
        private string CurrentUserId  = string.Empty;
        protected AIRMAM5DBEntities _db;
        protected VerifyResult result = new VerifyResult(false, "fail");
        readonly IGenericRepository<tbmNOTIFY> _notifyRepository;
        readonly IGenericRepository<tbmUSER_NOTIFY> _usernotifyRepository;

        public NotifyService()
        {
            this._db = new AIRMAM5DBEntities();
            _notifyRepository = new GenericRepository<tbmNOTIFY>(_db);
            _usernotifyRepository = new GenericRepository<tbmUSER_NOTIFY>(_db);
        }

        /// <summary>
        /// 訊息id [fnNOTIFY_ID] 取 dbo.tbmNOTIFY 資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<tbmNOTIFY> GetBy(long id = 0)
        {
            var query = _notifyRepository.FindBy(x => id == 0 ? true : x.fnNOTIFY_ID == id).DefaultIfEmpty().ToList();

            return query;
        }

        /// <summary>
        /// 使用者id+訊息id  取 dbo.tbmUSER_NOTIFY 資料
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<tbmUSER_NOTIFY> GetUserNoitfy(string userid, long id = 0)
        {
            var query = _usernotifyRepository.FindBy(x => id == 0 ? true : x.fnNOTIFY_ID == id)
                .Where(x => x.fsUSER_ID == userid).DefaultIfEmpty().ToList();

            return query;
        }

        /// <summary>
        /// 使用者ID 是否 有 訊息id [fnNOTIFY_ID] 資料
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public bool IsExistsByUser(long id, string userid)
        {
            var query = _usernotifyRepository.FindBy(x => x.fnNOTIFY_ID == id && x.fsUSER_ID == userid).Any();

            return query;
        }

        /// <summary>
        /// 取 訊息通知資料
        /// </summary>
        /// <param name="isexpire">是否逾期資料, 預設null 即全部資料。</param>
        /// <param name="days">指定取得已讀資料的天數(未讀設限一律顯示) 預設7日。 </param>
        /// <returns></returns>
        public IEnumerable<tbmNOTIFY> GetExpire(bool? isexpire = null, int days = 7)
        {
            var query = _notifyRepository.GetAll();

            if (isexpire == true)
                return _notifyRepository.FindBy(x => x.fdEXPIRE_DATE <= DateTime.Now);
            if (isexpire == false)
                return _notifyRepository.FindBy(x => x.fdEXPIRE_DATE > DateTime.Now || x.fdEXPIRE_DATE == null);

            return query;
        }

        /// <summary>
        /// 取 使用者訊息通知資料
        /// </summary>
        /// <param name="userid">使用者id </param>
        /// <param name="isdelete">訊息是否刪除 預設null表示取回全部訊息 </param>
        /// <param name="days">指定取得已讀資料的天數(未讀設限一律顯示) 預設7日。 </param>
        /// <returns></returns>
        public List<NotifyDataModel> GetByUser(string userid, bool? isdelete = null, int days = 7)
        {
            List<NotifyDataModel> result = new List<NotifyDataModel>();
            DateTime _sdt = DateTime.Now.AddDays(-days), _edt = DateTime.Now.AddDays(1);
            
            //Union : 帳號所有"未讀"訊息、指定天數內"已讀"訊息
            var _temp = (from a in _usernotifyRepository.FindBy(x => x.fsUSER_ID == userid && x.fbIS_READ == false)
                     select a)
                .Union(from b in _usernotifyRepository.FindBy(x => x.fsUSER_ID == userid && x.fbIS_READ == true && x.fdCREATED_DATE >= _sdt && x.fdCREATED_DATE <= _edt)
                       select b);

            var query = (from a in _temp.Where(x => (isdelete == null ? true : (x.fbIS_DELETE == isdelete)))
                         join b in this.GetExpire(false) on a.fnNOTIFY_ID equals b.fnNOTIFY_ID
                         select new NotifyDataModel
                         {
                             NOTIFY_ID = b.fnNOTIFY_ID,
                             TITLE = b.fsTITLE,
                             CONTENT = b.fsCONTENT,
                             IsRead = a.fbIS_READ,
                             CREATED_DATE = b.fdCREATED_DATE,
                             CREATED_BY = b.fsCREATED_BY
                         }).OrderByDescending(s => s.CREATED_DATE).ToList();

            result = query.Count() > 0 ? query : new List<NotifyDataModel>();
            return result;
        }

        #region 【新增/修改】
        /// <summary>
        /// 新增通知訊息資料
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public VerifyResult NotifyCreate(NotifyCreateModel model, string createby)
        {
            using (var _tran = _notifyRepository._context.Database.BeginTransaction())
            {
                try
                {
                    tbmNOTIFY _notify = new tbmNOTIFY
                    {
                        fsTITLE = model.Title,
                        fsCONTENT = model.Content,
                        fdEXPIRE_DATE = model.ExpireDate,
                        fnCATEGORY = model.Category,
                        fsCREATED_BY = createby,//CurrentUserId,
                        fdCREATED_DATE = DateTime.Now
                    };
                    _notifyRepository.Create(_notify);

                    //測試時,直接增加指定使用者UserId 的訊息通知記錄 
                    List<tbmUSER_NOTIFY> _usernotifyList = new List<tbmUSER_NOTIFY>
                    {
                        new tbmUSER_NOTIFY
                        {
                            fsUSER_ID = CurrentUserId,   //Login User's UserId
                            fnNOTIFY_ID = _notify.fnNOTIFY_ID,
                            fbIS_READ = false,
                            fbIS_DELETE = false,
                            fdCREATED_DATE = DateTime.Now,
                            fsCREATED_BY = CurrentUserId
                        }
                    };
                    //if (CurrentUserId != model.NoticeTo)
                    //{
                        _usernotifyList.Add(new tbmUSER_NOTIFY
                        {
                            fsUSER_ID = model.NoticeTo,
                            fnNOTIFY_ID = _notify.fnNOTIFY_ID,
                            fbIS_READ = false,
                            fbIS_DELETE = false,
                            fdCREATED_DATE = DateTime.Now,
                            fsCREATED_BY = CurrentUserId
                        });
                    //}
                    _usernotifyRepository.CreateRange(_usernotifyList);
                    
                    result.IsSuccess = true;
                    result.Message = "新增完成";
                    _tran.Commit();
                }
                catch (Exception ex)
                {
                    _tran.Rollback();
                    result.ErrorException = ex;
                    result.Message = ex.Message;
                    result.IsSuccess = false;
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult UpdateMultiple(List<tbmUSER_NOTIFY> rec)
        {
            try
            {
                if (rec != null && rec.Count() > 0)
                {
                    _usernotifyRepository.UpdateMultiple(rec);

                    result.IsSuccess = true;
                    result.Message = "OK(已讀)";
                    result.Data = rec;
                }
            }
            catch (Exception ex)
            {
                result.ErrorException = ex;
                result.Message = ex.Message;
                result.IsSuccess = false;
            }

            return result;
        }

        /// <summary>
        /// 新增通知訊息_&_SignalR更新前端
        /// </summary>
        /// <param name="rec"> tbmNOTIFY </param>
        /// <param name="createby">建立通知UserName </param>
        /// <param name="touserid">通知對像UserId </param>
        public VerifyResult Create(tbmNOTIFY rec, string createby, string touserid)
        {
            using (var _tran = _notifyRepository._context.Database.BeginTransaction())
            {
                try
                {
                    if (rec == null) return result;
                    _notifyRepository.Create(rec);

                    //使用者UserId 訊息通知記錄 
                    tbmUSER_NOTIFY _NOTIFY = new tbmUSER_NOTIFY
                    {
                        fsUSER_ID = touserid,
                        fnNOTIFY_ID = rec.fnNOTIFY_ID,
                        fbIS_READ = false,
                        fbIS_DELETE = false,
                        fdCREATED_DATE = DateTime.Now,
                        fsCREATED_BY = createby
                    };

                    _usernotifyRepository.Create(_NOTIFY);

                    result.IsSuccess = true;
                    result.Message = "通知新增完成";
                    _tran.Commit();
                }
                catch (Exception ex)
                {
                    _tran.Rollback();
                    result.ErrorException = ex;
                    result.Message = string.Format($"通知新增失敗【{ex.Message}】");
                    result.IsSuccess = false;
                }
            }

            return result;
        }
        #endregion

    }
}
