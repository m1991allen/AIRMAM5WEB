using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.TemplateFields;
using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 自訂樣板欄位設定資料表 tbmTEMPLATE_FIELDS
    /// </summary>
    //[MetadataType(typeof(tbmTEMPLATE_FIELDSMetadata))]
    public partial class tbmTEMPLATE_FIELDS
    {
        /// <summary>
        /// 自訂樣板欄位設定資料表 tbmTEMPLATE_FIELDS : 初始 預設值
        /// </summary>
        public tbmTEMPLATE_FIELDS()
            : base()
        {
            fnTEMP_ID = fnTEMP_ID;
            fsFIELD = string.Empty;
            fsFIELD_NAME = string.Empty;
            fsFIELD_TYPE = string.Empty;
            fnFIELD_LENGTH = 200;
            fsDESCRIPTION = string.Empty;
            fnORDER = 99;
            fnCTRL_WIDTH = 0;
            fsMULTILINE = IsTrueFalseEnum.N.ToString();
            IsMultiline = false;
            fsISNULLABLE = IsTrueFalseEnum.N.ToString();
            IsNullable = true;
            fsDEFAULT = string.Empty;
            fsCODE_ID = string.Empty;
            fnCODE_CNT = 1; //1-單選 /  0-複選
            fsCODE_CTRL = string.Empty;
            fsIS_SEARCH = IsTrueFalseEnum.N.ToString();
            IsSearch = false;
            fdCREATED_DATE = DateTime.Now;
            fsCREATED_BY = string.Empty;
            fdUPDATED_DATE = null;
            fsUPDATED_BY = string.Empty;
        }

        #region >>> 賦值 (marked_20210719_改用方法)
        ///// <summary>
        ///// <see cref="spGET_TEMPLATE_FIELDS_BY_TEMP_ID_Result"/>取得的資料轉至 [tbmTEMPLATE_FIELDS]
        ///// </summary>
        ///// <param name="sp">預存程序回傳資料集 <see cref="spGET_TEMPLATE_FIELDS_BY_TEMP_ID_Result"/> </param>
        //public tbmTEMPLATE_FIELDS(spGET_TEMPLATE_FIELDS_BY_TEMP_ID_Result sp)
        //{
        //    fnTEMP_ID = sp.fnTEMP_ID;
        //    fsFIELD = sp.fsFIELD;
        //    fsFIELD_NAME = sp.fsFIELD_NAME;
        //    fsFIELD_TYPE = sp.fsFIELD_TYPE;
        //    fnFIELD_LENGTH = sp.fnFIELD_LENGTH;
        //    fsDESCRIPTION = sp.fsDESCRIPTION;
        //    fnORDER = sp.fnORDER;
        //    fnCTRL_WIDTH = sp.fnCTRL_WIDTH;
        //    fsMULTILINE = sp.fsMULTILINE;
        //    IsMultiline = sp.fsMULTILINE.ToUpper() == IsTrueFalseEnum.Y.ToString() ? true : false;
        //    fsISNULLABLE = sp.fsISNULLABLE;
        //    IsNullable = sp.fsISNULLABLE.ToUpper() == IsTrueFalseEnum.Y.ToString() ? true : false;
        //    fsDEFAULT = sp.fsDEFAULT;
        //    fsCODE_ID = sp.fsCODE_ID;
        //    fnCODE_CNT = sp.fnCODE_CNT;
        //    fsCODE_CTRL = sp.fsCODE_CTRL;
        //    fsIS_SEARCH = sp.fsIS_SEARCH;
        //    IsSearch = sp.fsIS_SEARCH.ToUpper() == IsTrueFalseEnum.Y.ToString() ? true : false;
        //    fdCREATED_DATE = sp.fdCREATED_DATE;
        //    fsCREATED_BY = string.Format($"{sp.fsCREATED_BY}({sp.fsCREATED_BY_NAME})");
        //    fdUPDATED_DATE = sp.fdUPDATED_DATE;
        //    fsUPDATED_BY = sp.fsUPDATED_BY == null ? string.Empty : string.Format($"{sp.fsUPDATED_BY}({sp.fsUPDATED_BY_NAME})");
        //}

        ///// <summary>
        ///// <see cref="tbmTEMPLATE_FIELDS"/> 資料轉至 [tbmTEMPLATE_FIELDS]
        ///// </summary>
        ///// <param name="m">樣板-自訂欄位屬性設定資料表 <see cref="tbmTEMPLATE_FIELDS"/> </param>
        //public tbmTEMPLATE_FIELDS(tbmTEMPLATE_FIELDS m)
        //{
        //    fnTEMP_ID = 0;
        //    fsFIELD = m.fsFIELD;
        //    fsFIELD_NAME = m.fsFIELD_NAME;
        //    fsFIELD_TYPE = m.fsFIELD_TYPE;
        //    fnFIELD_LENGTH = m.fnFIELD_LENGTH;
        //    fsDESCRIPTION = m.fsDESCRIPTION;
        //    fnORDER = m.fnORDER;
        //    fnCTRL_WIDTH = m.fnCTRL_WIDTH;
        //    fsMULTILINE = m.fsMULTILINE;
        //    IsMultiline = m.fsMULTILINE.ToUpper() == IsTrueFalseEnum.Y.ToString() ? true : false;
        //    fsISNULLABLE = m.fsISNULLABLE;
        //    IsNullable = m.fsISNULLABLE.ToUpper() == IsTrueFalseEnum.Y.ToString() ? true : false;
        //    fsDEFAULT = m.fsDEFAULT;
        //    fsCODE_ID = m.fsCODE_ID;
        //    fnCODE_CNT = m.fnCODE_CNT;
        //    fsCODE_CTRL = m.fsCODE_CTRL;
        //    fsIS_SEARCH = m.fsIS_SEARCH;
        //    IsSearch = m.fsIS_SEARCH.ToUpper() == IsTrueFalseEnum.Y.ToString() ? true : false;
        //}

        ///// <summary>
        ///// <see cref="TemplateFieldsModel"/>  資料轉至 [tbmTEMPLATE_FIELDS]
        ///// </summary>
        ///// <param name="m">樣板-自訂欄位資料表 <see cref="TemplateFieldsModel"/> </param>
        //public tbmTEMPLATE_FIELDS(TemplateFieldsModel m)
        //{
        //    fnTEMP_ID = 0;
        //    fsFIELD = m.Field;
        //    fsFIELD_NAME = m.FieldName ?? string.Empty;
        //    fsFIELD_TYPE = m.FieldType;
        //    fnFIELD_LENGTH = m.FieldLen;
        //    fsDESCRIPTION = m.FieldDesc ?? string.Empty;
        //    fnORDER = m.FieldOrder;
        //    fnCTRL_WIDTH = m.FieldWidth;
        //    fsMULTILINE = m.IsMultiline ? IsTrueFalseEnum.Y.ToString() : IsTrueFalseEnum.N.ToString();
        //    IsMultiline = m.IsMultiline;
        //    fsISNULLABLE = m.IsNullable ? IsTrueFalseEnum.Y.ToString() : IsTrueFalseEnum.N.ToString();
        //    IsNullable = m.IsNullable;
        //    //fsDEFAULT = m.FieldDef ?? string.Empty;
        //    fsCODE_ID = m.FieldCodeId ?? string.Empty;
        //    fnCODE_CNT = m.FieldCodeCnt;
        //    fsCODE_CTRL = m.FieldCodeCtrl ?? string.Empty;
        //    fsIS_SEARCH = m.IsSearch ? IsTrueFalseEnum.Y.ToString() : IsTrueFalseEnum.N.ToString();
        //    IsSearch = m.IsSearch;
        //}
        #endregion

        /// <summary>
        /// 樣板-自訂欄位資料表 <see cref="TemplateFieldsModel"/> Metadata
        /// </summary>
        public class tbmTEMPLATE_FIELDSMetadata
        {
            /// <summary>
            /// 樣板編號
            /// </summary>
            [Required]
            [Display(Name = "樣板編號")]
            public int fnTEMP_ID { get; set; }

            /// <summary>
            /// 欄位代號
            /// </summary>
            [Required]
            [Display(Name = "欄位代號")]
            public string fsFIELD { get; set; }

            /// <summary>
            /// 欄位名稱
            /// </summary>
            [Required]
            [Display(Name = "欄位名稱")]
            public string fsFIELD_NAME { get; set; }

            /// <summary>
            /// tbzCODE.TEMP002
            /// </summary>
            [Required]
            [Display(Name = "欄位型別")]
            public string fsFIELD_TYPE { get; set; }

            /// <summary>
            /// 欄位長度
            /// </summary>
            [Display(Name = "欄位長度")]
            public int? fnFIELD_LENGTH { get; set; }

            /// <summary>
            /// 描述
            /// </summary>
            [Display(Name = "描述")]
            public string fsDESCRIPTION { get; set; }

            /// <summary>
            /// 顯示順序
            /// </summary>
            [Required]
            [Display(Name = "顯示順序")]
            public int fnORDER { get; set; }

            /// <summary>
            /// 欄位控制項寬度
            /// </summary>
            [Required]
            [Display(Name = "內容上限")]
            public int? fnCTRL_WIDTH { get; set; }

            /// <summary>
            /// 是否多行 Y/N
            /// </summary>
            [Display(Name = "多行")]
            public string fsMULTILINE { get; set; }

            /// <summary>
            /// 是否可為NULL Y/N
            /// </summary>
            [Display(Name = "可為空值")]
            public string fsISNULLABLE { get; set; }

            /// <summary>
            /// 預設值
            /// </summary>
            [Display(Name = "預設值")]
            public string fsDEFAULT { get; set; }

            /// <summary>
            /// 代碼編號
            /// </summary>
            [Display(Name = "代碼編號")]
            public string fsCODE_ID { get; set; }

            /// <summary>
            /// 0:多選、1:單選
            /// </summary>
            [Display(Name = "單選或多選")]
            public int? fnCODE_CNT { get; set; }
            /// <summary>
            /// 控制項類型
            /// </summary>
            [Display(Name = "控制項類型")]
            public string fsCODE_CTRL { get; set; }

            /// <summary>
            /// 是否要列為進階搜尋 Y/N
            /// </summary>
            [Display(Name = "進階搜尋")]
            public string fsIS_SEARCH { get; set; }

            [Display(Name = "建立時間")]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}", ApplyFormatInEditMode = true)]
            public DateTime fdCREATED_DATE { get; set; }

            [Display(Name = "建立帳號")]
            public string fsCREATED_BY { get; set; }

            [Display(Name = "最後異動時間")]
            [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}", ApplyFormatInEditMode = true)]
            public DateTime? fdUPDATED_DATE { get; set; }

            [Display(Name = "最後異動帳號")]
            public string fsUPDATED_BY { get; set; }
        }

        #region 額外顯示/判斷欄位
        /// <summary>
        /// 是否多行 (對應原欄位[fsMULTILINE])
        /// </summary>
        public bool IsMultiline { get; set; } = false;
        /// <summary>
        /// 是否可為NULL(對應原欄位[fsISNULLABLE])
        /// </summary>
        public bool IsNullable { get; set; } = true;
        /// <summary>
        /// 是否要列為進階搜尋 (對應原欄位[fsIS_SEARCH])
        /// </summary>
        public bool IsSearch { get; set; } = false;
        /// <summary>
        /// 欄位型別 fsFIELD_TYPE Enum
        /// </summary>
        public CodeTEMP002Enum FieldType = CodeTEMP002Enum.NVARCHAR;
        #endregion

        /// <summary>
        /// 泛型資料轉換 _20210719
        /// </summary>
        /// <typeparam name="T">資料來源Model: <see cref="spGET_TEMPLATE_FIELDS_BY_TEMP_ID_Result"/>, <see cref="tbmTEMPLATE_FIELDS"/>, <see cref="TemplateFieldsModel"/>
        /// </typeparam>
        /// <param name="m">資料內容 </param>
        /// <returns></returns>
        public tbmTEMPLATE_FIELDS FormatConvert<T>(T m)
        {
            string crtUr = string.Empty, crtUrnm = string.Empty, updUr = string.Empty, updUrnm = string.Empty;

            var _Properties = typeof(T).GetProperties();
            foreach (var p in _Properties)
            {
                var val = p.GetValue(m) ?? string.Empty;

                if (p.Name == "fnTEMP_ID")
                {
                    int.TryParse(val.ToString(), out int fnid);
                    this.fnTEMP_ID = fnid;
                }
                if (p.Name == "fsFIELD" || p.Name == "Field") this.fsFIELD = val.ToString();
                if (p.Name == "fsFIELD_NAME" || p.Name == "FieldName") this.fsFIELD_NAME = val.ToString();
                if (p.Name == "fsFIELD_TYPE" || p.Name == "FieldType") this.fsFIELD_TYPE = val.ToString();
                if (p.Name == "fnFIELD_LENGTH" || p.Name == "FieldLen")
                {
                    int.TryParse(val.ToString(), out int l);
                    this.fnFIELD_LENGTH = l ;
                }
                if (p.Name == "fsDESCRIPTION" || p.Name == "FieldDesc") this.fsDESCRIPTION = val.ToString();
                if (p.Name == "fnORDER" || p.Name == "FieldOrder")
                {
                    int.TryParse(val.ToString(), out int i);
                    this.fnORDER = i;
                }
                if (p.Name == "fnCTRL_WIDTH" || p.Name == "FieldWidth")
                {
                    int.TryParse(val.ToString(), out int i);
                    this.fnCTRL_WIDTH = i;
                }
                if (p.Name == "fsMULTILINE" || p.Name == "IsMultiline")
                {
                    this.fsMULTILINE = (val.ToString().ToUpper() == BoolTrueFalseEnum.TRUE.ToString() || val.ToString().ToUpper() == IsTrueFalseEnum.Y.ToString()) 
                        ? IsTrueFalseEnum.Y.ToString() 
                        : IsTrueFalseEnum.N.ToString();

                    this.IsMultiline = (val.ToString().ToUpper() == BoolTrueFalseEnum.TRUE.ToString() || val.ToString().ToUpper() == IsTrueFalseEnum.Y.ToString()) ? true : false;
                }

                //是否"可為空值"
                if (p.Name == "fsISNULLABLE" || p.Name == "IsNullable")
                {
                    this.fsISNULLABLE = (val.ToString().ToUpper() == BoolTrueFalseEnum.TRUE.ToString() || val.ToString().ToUpper() == IsTrueFalseEnum.Y.ToString())
                        ? IsTrueFalseEnum.Y.ToString()
                        : IsTrueFalseEnum.N.ToString();

                    this.IsNullable = (val.ToString().ToUpper() == BoolTrueFalseEnum.TRUE.ToString() || val.ToString().ToUpper() == IsTrueFalseEnum.Y.ToString()) ? true : false;
                }
                if (p.Name == "fsDEFAULT" || p.Name == "FieldDef") { this.fsDEFAULT = val.ToString(); }

                if (p.Name == "fsCODE_ID" || p.Name == "FieldCodeId") { this.fsCODE_ID = val.ToString(); }
                if (p.Name == "fnCODE_CNT" || p.Name == "FieldCodeCnt")
                {
                    int.TryParse(val.ToString(), out int i);
                    this.fnCODE_CNT = i;
                }
                if (p.Name == "fsCODE_CTRL" || p.Name == "FieldCodeCtrl") { this.fsCODE_CTRL = val.ToString(); }
                if (p.Name == "fsIS_SEARCH" || p.Name == "IsSearch")
                {
                    this.fsIS_SEARCH = (val.ToString().ToUpper() == BoolTrueFalseEnum.TRUE.ToString() || val.ToString().ToUpper() == IsTrueFalseEnum.Y.ToString())
                        ? IsTrueFalseEnum.Y.ToString()
                        : IsTrueFalseEnum.N.ToString();

                    this.IsSearch = (val.ToString().ToUpper() == BoolTrueFalseEnum.TRUE.ToString() || val.ToString().ToUpper() == IsTrueFalseEnum.Y.ToString()) ? true : false;
                }

                //if (p.Name == "C_sLIMIT") //預存欄位:自訂欄位的長度
                //if (p.Name == "C_sFIELD_TYPE_NAME") //預存欄位:自訂欄位的類型
                //if (p.Name == "C_sCODE_SET_NAME") //預存欄位:自訂欄位的代碼選擇清單

                if (p.Name == "fdCREATED_DATE")
                {
                    DateTime.TryParse(val.ToString(), out DateTime t);
                    this.fdCREATED_DATE = t;
                }
                if (p.Name == "fsCREATED_BY")
                {
                    this.fsCREATED_BY = val.ToString();
                    crtUr = val.ToString();
                }
                if (p.Name == "fdUPDATED_DATE")
                {
                    DateTime.TryParse(val.ToString(), out DateTime t);
                    this.fdUPDATED_DATE = t;
                }
                if (p.Name == "fsUPDATED_BY")
                {
                    this.fsUPDATED_BY = val.ToString();
                    updUr = val.ToString();
                }
                //if (p.Name == "C_VALUE") //預存欄位:(不清楚定義)
                if (p.Name == "fsCREATED_BY_NAME") crtUrnm = val.ToString();
                if (p.Name == "fsUPDATED_BY_NAME") updUrnm = val.ToString();
            }

            if (!string.IsNullOrEmpty(crtUrnm)) this.fsCREATED_BY = string.Format($"{crtUr}({crtUrnm})");
            if (!string.IsNullOrEmpty(updUrnm)) this.fsUPDATED_BY = string.Format($"{updUr}({updUrnm})");

            return this;
        }
    }
}
