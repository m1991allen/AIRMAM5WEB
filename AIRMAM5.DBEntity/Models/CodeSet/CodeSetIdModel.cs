namespace AIRMAM5.DBEntity.Models.CodeSet
{
    /// <summary>
    /// 代碼主表.代碼 Model
    /// </summary>
    public class CodeSetIdModel
    {
        /// <summary>
        /// 代碼主表.代碼 fsCODE_ID
        /// </summary>
        public string CodeId { get; set; }
    }

    /// <summary>
    /// 代碼主代碼、子代碼 參數
    /// <para> fsCODE_ID + fsCODE </para>
    /// </summary>
    public class CodeIdsModel : CodeSetIdModel
    {
        /// <summary>
        /// 子代碼 dbo.tbzCODE.[fsCODE]
        /// </summary>
        public string Code { get; set; }
    }
}
