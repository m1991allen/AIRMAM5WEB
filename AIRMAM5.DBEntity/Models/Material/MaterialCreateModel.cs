
namespace AIRMAM5.DBEntity.Models.Material
{
    /// <summary>
    /// 加入借調 資料欄位
    /// </summary>
    public class MaterialCreateModel
    {
        /// <summary>
        /// 借調的檔案類型 tbzCODE.MTRL001= V,A,P,D (參考IE版本,只有影片可以操作 借調)
        /// </summary>
        public string FileCategory { get; set; } = string.Empty;

        /// <summary>
        /// (借調的)檔案編號 [fsFILE_NO]
        /// </summary>
        public string FileNo { get; set; } = string.Empty;

        /// <summary>
        /// 描述 [fsDESCRIPTION]
        /// </summary>
        public string MaterialDesc { get; set; } = string.Empty;

        /// <summary>
        /// 備註 [fsNOTE]
        /// </summary>
        public string MaterialNote { get; set; } = string.Empty;

        /// <summary>
        /// 相關參數 [fsPARAMETER] 如: 部分調用起訖點(12.162;48.437;) ***分號;為分隔符號***
        /// </summary>
        public string ParameterStr { get; set; } = string.Empty;
    }

}
