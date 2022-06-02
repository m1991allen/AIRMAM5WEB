using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRMAM5.DBEntity.Models.Directory
{
    /// <summary>
    /// 預存: spGET_DIRECTORIES_AND_SUBJECTS_BY_DIR_ID_Filter 參數。 繼承參考<see cref="DirIdModel"/>
    /// </summary>
    public class GetDirAndSubjectsByDirFilter : DirIdModel
    {
        /// <summary>
        /// 分類: A聲音, D文件, P圖片, S主題, V影片
        /// </summary>
        public string FileType { get; set; } = string.Empty;

        /// <summary>
        /// 檔案編號
        /// </summary>
        public string FileNo { set; get; } = string.Empty;

    }
}
