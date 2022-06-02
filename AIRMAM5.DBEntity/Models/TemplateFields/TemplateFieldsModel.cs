using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Template;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.TemplateFields
{
    /// <summary>
    /// 樣板-自訂欄位 設定參數 Model。 繼承參考 <see cref="TemplateIdModel"/>
    /// </summary>
    public class TemplateFieldsModel : TemplateIdModel
    {
        /// <summary>
        /// 樣板-自訂欄位 設定參數 Model。
        /// </summary>
        public TemplateFieldsModel() { }

        /*  Modified_20210903: 改 DataConvert<T>(T data) */
        ///// <summary>
        ///// 樣板-自訂欄位 設定參數 Model。
        ///// </summary>
        ///// <param name="m">預存程序回傳資料集 <see cref="spGET_TEMPLATE_FIELDS_BY_TEMP_ID_Result"/> </param>
        //public TemplateFieldsModel(spGET_TEMPLATE_FIELDS_BY_TEMP_ID_Result m)
        //{
        //    Field = m.fsFIELD;
        //    FieldName = m.fsFIELD_NAME;
        //    FieldType = m.fsFIELD_TYPE;
        //    FieldDesc = m.fsDESCRIPTION;
        //    FieldLen = m.fnFIELD_LENGTH ?? 0;
        //    FieldOrder = m.fnORDER;
        //    FieldWidth = m.fnCTRL_WIDTH ?? 0;
        //    IsMultiline = m.fsMULTILINE == IsTrueFalseEnum.Y.ToString() ? true : false;
        //    IsNullable = m.fsISNULLABLE == IsTrueFalseEnum.Y.ToString() ? true : false;
        //    FieldDef = m.C_sDEFAULT ?? string.Empty;
        //    FieldCodeId = m.fsCODE_ID ?? string.Empty;
        //    FieldCodeCnt = m.fnCODE_CNT ?? 1;
        //    IsSearch = m.fsIS_SEARCH == IsTrueFalseEnum.Y.ToString() ? true : false;
        //}

        #region >>>>> 欄位參數
        /// <summary>
        /// fsFIELD (樣板)欄位代號, EX: fsATTRIBUTE1, fsATTRIBUTE2, ....
        /// </summary>
        [Display(Name = "欄位")]
        public string Field { get; set; } = string.Empty;

        /// <summary>
        /// fsFIELD_NAME 欄位名稱
        /// </summary>
        [Display(Name = "名稱")]
        [Required]
        public string FieldName { get; set; } = string.Empty;

        /// <summary>
        /// fsFIELD_TYPE 欄位型別 (tbzCODE.TEMP002)
        /// </summary>
        [Display(Name = "型別")]
        public string FieldType { get; set; } = string.Empty;

        /// <summary>
        /// fsDESCRIPTION 欄位描述
        /// </summary>
        [Display(Name = "描述")]
        public string FieldDesc { get; set; } = string.Empty;

        /// <summary>
        /// fnFIELD_LENGTH 欄位長度
        [Display(Name = "欄位長度")]
        /// </summary>
        public int FieldLen { get; set; } = 200;

        /// <summary>
        /// 顯示順序
        /// </summary>
        [Display(Name = "顯示順序")]
        [Required]
        public int FieldOrder { get; set; } = 99;

        /// <summary>
        /// fnCTRL_WIDTH 欄位控制項寬度
        /// </summary>
        [Display(Name = "內容上限")]
        public int FieldWidth { get; set; } = 200;

        /// <summary>
        /// fsMULTILINE 是否多行
        /// </summary>
        [Display(Name = "多行")]
        public bool IsMultiline { get; set; } = false;

        /// <summary>
        /// fsISNULLABLE 是否可為NULL
        /// </summary>
        [Display(Name = "可為空值")]
        public bool IsNullable { get; set; } = true;

        /// <summary>
        /// fsDEFAULT 預設值 / 或表示[fsFIELD]實際的欄位資料值
        /// </summary>
        [Display(Name = "預設值")]
        public string FieldDef { get; set; } = string.Empty;

        /// <summary>
        /// fsCODE_ID 代碼編號
        /// </summary>
        [Display(Name = "代碼編號")]
        public string FieldCodeId { get; set; } = string.Empty;

        /// <summary>
        /// fnCODE_CNT 單選或多選 (0:多選、1:單選)
        /// </summary>
        [Display(Name = "單選或多選")]
        public int FieldCodeCnt { get; set; } = 1;

        /// <summary>
        /// fsCODE_CTRL 控制項類型(目前不使用)
        /// </summary>
        [Display(Name = "控制項類型")]
        public string FieldCodeCtrl { get; set; } = string.Empty;

        /// <summary>
        /// fcIS_SEARCH	是否要列為進階搜尋
        /// </summary>
        [Display(Name = "進階搜尋")]
        public bool IsSearch { get; set; } = false;
        #endregion

        public TemplateFieldsModel DataConvert<T>(T data)
        {
            if (data == null) { return this; }

            var properties = typeof(T).GetProperties();
            foreach (var pp in properties)
            {
                object val = pp.GetValue(data) ?? string.Empty;

                if (pp.Name == "fsFIELD") { Field = val.ToString(); }
                if (pp.Name == "fsFIELD_NAME") { FieldName = val.ToString(); }
                if (pp.Name == "fsFIELD_TYPE") { FieldType = val.ToString(); }
                if (pp.Name == "fsDESCRIPTION") { FieldDesc = val.ToString(); }

                if (pp.Name == "fnFIELD_LENGTH")
                {
                    if (int.TryParse(val.ToString(), out int len)) { FieldLen = len; } else { FieldLen = 0; }
                }
                if (pp.Name == "fnORDER")
                {
                    if(int.TryParse(val.ToString(), out int sort)) { FieldOrder = sort; }
                }
                if (pp.Name == "fnCTRL_WIDTH")
                {
                    if (int.TryParse(val.ToString(), out int wid)) { FieldWidth = wid; } else { this.FieldWidth = wid; }
                }

                if (pp.Name == "fsMULTILINE")
                {
                    IsMultiline = val.ToString() == IsTrueFalseEnum.Y.ToString() ? true : false;
                }
                if (pp.Name == "fsISNULLABLE")
                {
                    IsNullable = val.ToString() == IsTrueFalseEnum.Y.ToString() ? true : false;
                }

                if (pp.Name == "C_sDEFAULT") { FieldDef = val.ToString() ?? string.Empty; }
                if (pp.Name == "fsCODE_ID") { FieldCodeId = val.ToString() ?? string.Empty; }

                if (pp.Name == "fnCODE_CNT")
                {
                    if (int.TryParse(val.ToString(), out int cnt)) { FieldCodeCnt = cnt; } else { FieldCodeCnt = 1; }
                }

                if (pp.Name == "fsIS_SEARCH")
                {
                    IsSearch = val.ToString() == IsTrueFalseEnum.Y.ToString() ? true : false;
                }
            }

            return this;
        }
    }

}
