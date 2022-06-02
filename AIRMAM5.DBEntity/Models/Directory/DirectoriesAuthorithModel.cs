using AIRMAM5.DBEntity.DBEntity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Directory
{
    /// <summary>
    /// 系統目錄使用權限 資料ViewModel。 繼承參考<see cref="DirIdModel"/>
    /// </summary>
    public class DirectoriesAuthorithModel : DirIdModel
    {
        /// <summary>
        /// 指定目錄id、欄位類別: G群組/U使用者
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        public DirectoriesAuthorithModel(long id, string type) { DirId = id; DATATYPE = type; }

        /// <summary>
        /// 欄位類別 : G群組/U使用者
        /// </summary>
        [Display(Name = "角色群組")]
        public string DATATYPE { get; set; }

        /// <summary>
        /// 目錄{使用者/群組}權限資料清單 <see cref="spGET_DIRECTORIES_BY_DIR_ID_AUTHORITY_Result"/>
        /// </summary>
        public List<spGET_DIRECTORIES_BY_DIR_ID_AUTHORITY_Result> DirAuthority { get; set; }
    }

}
