
namespace AIRMAM5.DBEntity.Models.Search
{
    /// <summary>
    /// 檢索符合筆數API Search 參數欄位, 繼承參考 <see cref="SearchParameterViewModel"/>
    /// </summary>
    public class SearchApiParameterModel : SearchParameterViewModel
    {
        /// <summary>
        /// 檢索符合筆數API Search 參數欄位
        /// </summary>
        public SearchApiParameterModel() { }

        /// <summary>
        /// 檢索符合筆數API Search 參數欄位
        /// </summary>
        /// <param name="m"></param>
        public SearchApiParameterModel(SearchParameterViewModel m)
        {
            fsKEYWORD = m.fsKEYWORD;
            fsINDEX = m.fsINDEX;
            fnSEARCH_MODE = m.fnSEARCH_MODE;
            fnHOMO = m.fnHOMO;
            clsDATE = m.clsDATE;
            lstCOLUMN_ORDER = m.lstCOLUMN_ORDER;
            fnTEMP_ID = m.fnTEMP_ID;
            lstCOLUMN_SEARCH = m.lstCOLUMN_SEARCH;
            fnPAGE_SIZE = m.fnPAGE_SIZE;
            fnSTART_INDEX = m.fnSTART_INDEX == 0 ? 1 : m.fnSTART_INDEX;
        }

        /// <summary>
        /// 可查詢目錄節點權限(以分號分開,例: 1873;1867) [fsAUTH_DIR]
        /// </summary>
        public string fsAUTH_DIR { get; set; } = string.Empty;

        /// <summary>
        /// 可查詢機密權限(以分號分開,例: 0;1;2) [fsSECRET]
        /// </summary>
        public string fsSECRET { get; set; } = string.Empty;

        /// <summary>
        /// 是否為管理者 [fbIS_ADMIN]
        /// </summary>
        public bool fbIS_ADMIN { get; set; } = false;
    }

}
