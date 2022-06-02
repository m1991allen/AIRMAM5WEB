using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AIRMAM5.Models.TSMapi
{
    /// <summary>
    /// TSM 檔案狀態資料 與專案是否使用TSM參數_Added_2020.01.03
    /// </summary>
    public class GetTsmFileStatusResult
    {
        public GetTsmFileStatusResult() { }

        public List<GetFileStatusResult> TsmFileStatus { get; set; } = new List<GetFileStatusResult>();

        public bool IsUseTSM { get; set; } = false;
    }
}