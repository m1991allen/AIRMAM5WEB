using AIRMAM5.DBEntity.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRMAM5.DBEntity.Interface
{
    public interface IGenericInterface<TEntity> where TEntity : class
    {
        //#region Current User Info 目前使用者
        ///// <summary>
        ///// 目前使用者ID
        ///// </summary>
        //string CurrentUID { get; }
        ///// <summary>
        ///// 目前使用者是否為 系統管理員角色
        ///// </summary>
        //bool CurrentUserIsAdmin { get; }
        ///// <summary>
        ///// 目前使用者是否為 媒資管理員 角色
        ///// </summary>
        //bool CurrentUserIsMediaManager { get; }
        ///// <summary>
        ///// 系統管理員OR媒資管理員:
        ///// </summary>
        //bool IsAdminOrMediaManager { get; }
        /////// <summary>
        /////// 目前使用者可用的目錄節點權限(ex: 1899;1873;)
        /////// </summary>
        ////string CurrentUserDirAuth { get; }
        //#endregion

        //#region ------------------------------------------【KEY】----------------
        ///// <summary>
        ///// <TEntity>主鍵 KEY 數值
        ///// </summary>
        //int IndexId { get; set; }
        ///// <summary>
        ///// <TEntity>主鍵 KEY 字串
        ///// </summary>
        //string IndexStr { get; set; }
        //#endregion
        #region ------------------------------------------【判斷】----------------
        //bool IsExists(int IndexId);

        bool IsExists(string IndexStr);
        #endregion
        #region ------------------------------------------【取資料】----------------
        List<TEntity> GetAll();

        TEntity GetById(int IndexId);

        TEntity GetById(string IndexStr);

        //string GetUserId();
        #endregion
        #region ------------------------------------------【CURD】----------------
        VerifyResult Create(TEntity rec);
        VerifyResult CreateRange(List<TEntity> ranges);

        VerifyResult Update(TEntity rec);
        VerifyResult UpdateRange(List<TEntity> ranges);

        VerifyResult Delete(TEntity rec);
        VerifyResult RemoveRange(List<TEntity> ranges);
        #endregion
    }
}
