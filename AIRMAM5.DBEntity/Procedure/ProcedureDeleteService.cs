using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using System;
using System.Linq;

namespace AIRMAM5.DBEntity.Procedure
{
    /// <summary>
    /// DB Procedure : spDELETE_xxxxxxxxx
    /// </summary>
    public class ProcedureDeleteService
    {
        protected AIRMAM5DBEntities _db;
        readonly ISerilogService _serilogService;
        protected VerifyResult result;

        public ProcedureDeleteService(ISerilogService serilogService)
        {
            _db = new AIRMAM5DBEntities();
            _serilogService = serilogService;
        }

        /// <summary>
        /// 依 檔案分類(VAPD) 刪除檔案 【dbo.spDELETE_ARC_BY_TYPE_FILE_NO】
        /// </summary>
        /// <param name="type">媒體的樣板類型: V, A, P, D </param>
        /// <param name="fileno">檔案編號 </param>
        /// <param name="reason">刪除原因 </param>
        /// <param name="deleteby">刪除人員 </param>
        /// <returns></returns>
        public VerifyResult DeleteArcByTypeFileno(string type, string fileno, string reason, string deleteby)
        {
            result = new VerifyResult();
            string _nm = type == FileTypeEnum.V.ToString() ? "影片檔案" :
                    (type == FileTypeEnum.A.ToString()) ? "聲音檔案" :
                        (type == FileTypeEnum.P.ToString()) ? "圖片檔案" :
                            (type == FileTypeEnum.D.ToString()) ? "文件檔案" : "";

            try
            {
                var _exec = _db.spDELETE_ARC_BY_TYPE_FILE_NO(type, fileno, reason, deleteby).FirstOrDefault();
                if (_exec.IndexOf("ERROR") == -1)
                {
                    result.IsSuccess = true;
                    result.Message = string.Format($"{_nm} 刪除成功!");
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = _exec.Split(':')[1];
                }
            }
            catch (Exception ex)
            {
                #region _Serilog
                _serilogService.SerilogWriter(new SerilogInputModel
                {
                    Controller = "ProcedureDeleteService",
                    Method = "[DeleteArcByTypeFileno]",
                    EventLevel = SerilogLevelEnum.Error,
                    Input = ex,
                    LogString = "Exception",
                    ErrorMessage = string.Format($"{_nm} 刪除失敗. {ex.Message}")
                });
                #endregion
                result = new VerifyResult(false, string.Format($"{_nm} 刪除失敗【{ex.Message}】"));
            }

            return result;
        }

    }
}
