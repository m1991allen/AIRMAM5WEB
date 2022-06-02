using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Repository;
using System;
using System.Linq;

namespace AIRMAM5.DBEntity.Services
{
    /// <summary>
    /// 號碼表-tblLOG
    /// </summary>
    public class TblNoService
    {
        readonly ISerilogService _serilogService;
        protected AIRMAM5DBEntities _db;
        protected VerifyResult result = new VerifyResult(false, "fail");
        private readonly IGenericRepository<tblNO> _tblnoRepository = new GenericRepository<tblNO>();

        public TblNoService()
        {
            _serilogService = new SerilogService();
            //_db = new AIRMAM5DBEntities();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">流水號 fsTYPE </param>
        /// <param name="head">編號頭 fsHEAD。ex: 20190527_0000001 => 20190527(頭) + _(身) + 0000001(尾) </param>
        /// <returns></returns>
        public IQueryable<tblNO> GetBy(string type, string head)
        {
            return _tblnoRepository.FindBy(x => x.fsTYPE == type && x.fsHEAD == head);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">流水號 fsTYPE </param>
        /// <param name="head">編號頭 fsHEAD。ex: 20190527_0000001 => 20190527(頭) + _(身) + 0000001(尾) </param>
        /// <returns></returns>
        public bool IsExists(string type, string head)
        {
            var query = _tblnoRepository.FindBy(x => x.fsTYPE == type && x.fsHEAD == head);

            return query.Any();
        }

        /// <summary>
        /// 取 指定 type+name+head+body+len 新號碼 【spGET_L_NO_NEW_NO】
        /// </summary>
        /// <param name="type">流水號 fsTYPE (像分類 如: ARC, SUBJECT, COPYFILE,....) </param>
        /// <param name="name">取號名稱 fsNAME </param>
        /// <param name="head">編號頭 fsHEAD。ex: 20190527_0000001 => 20190527(頭) + _(身) + 0000001(尾) </param>
        /// <param name="body">編號身 fsBODY </param>
        /// <param name="len">編號尾長度 fsNO_L </param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public string GetNewNo(string type, string name, string head, string body, int len, string userid)
        {
            using (_db = new AIRMAM5DBEntities())
            {
                var query = _db.spGET_L_NO_NEW_NO(type, name, head, body, len, userid).FirstOrDefault().ToString();

                return query;
            }
        }

        #region ---------- CURD 
        /// <summary>
        /// 新建  dbo.tblNO
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult CreateBy(tblNO rec)
        {
            try
            {
                _tblnoRepository.Create(rec);

                result.IsSuccess = true;
                result.Message = "號碼表已新增.";
                result.Data = rec;
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "TblNoService",
                    Method = "CreateBy",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = rec, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"號碼表新增失敗【{ex.Message}】 ")
                });
                #endregion
                //result = new VerifyResult(false, string.Format($"號碼表新增失敗【{ex.Message}】"));
                result.IsSuccess = false;
                result.Message = string.Format($"號碼表新增失敗【{ex.Message}】");
            }

            return result;
        }

        /// <summary>
        /// 更新/異動 dbo.tblNO
        /// </summary>
        /// <param name="rec"></param>
        /// <returns></returns>
        public VerifyResult UpdateBy(tblNO rec)
        {
            try
            {
                _tblnoRepository.Update(rec);

                result.IsSuccess = true;
                result.Message = "號碼表已更新.";
                result.Data = rec;
            }
            catch (Exception ex)
            {
                #region Serilog.Err
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "TblNoService",
                    Method = "CreateBy",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = new { Params = rec, Exception = ex },
                    LogString = "Exception",
                    ErrorMessage = string.Format($"號碼表更新失敗【{ex.Message}】 ")
                });
                #endregion
                //result = new VerifyResult(false, string.Format($"號碼表更新失敗【{ex.Message}】"));
                result.IsSuccess = false;
                result.Message = string.Format($"號碼表更新失敗【{ex.Message}】");
            }

            return result;
        }
        #endregion
    }
}
