using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Services;
using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.DBEntity
{
    /// <summary>
    /// 自訂樣板資料表  dbo.tbmTEMPLATE partial Model
    /// </summary>
    /// <remarks> <see cref="TemplateFieldsViewModel"/> 參考使用。 </remarks>
    [MetadataType(typeof(tbmTEMPLATEMetadata))]
    public partial class tbmTEMPLATE
    {
        /// <summary>
        /// 初始欄位
        /// </summary>
        public tbmTEMPLATE() : base()
        {
            fsNAME = string.Empty;
            fsTABLE = string.Empty;
            TargetTable = string.Empty;
            fsDESCRIPTION = string.Empty;
            fcIS_SEARCH = "N";
            IsSearch = false;
            fdCREATED_DATE = DateTime.Now;
            fsCREATED_BY = string.Empty;
            fdUPDATED_DATE = null;
            fsUPDATED_BY = string.Empty;
        }

        #region 額外顯示/判斷欄位
        /// <summary>
        /// 樣板分類 (對應原欄位[fsTABLE] JOIN  fsCODE_ID='TEMP001') 
        /// </summary>
        [Display(Name = "樣板分類")]
        public string TargetTable { get; set; }

        /// <summary>
        /// 是否要列為進階搜尋 (對應原欄位[fsIS_SEARCH])
        /// </summary>
        [Display(Name = "進階搜尋")]
        public bool IsSearch { get; set; }
        #endregion

        /// <summary>
        /// 自訂樣板資料表 Metadata
        /// </summary>
        public class tbmTEMPLATEMetadata
        {
            /// <summary>
            /// 樣板編號
            /// </summary>
            [Display(Name = "樣板編號")]
            public int fnTEMP_ID { get; set; }

            /// <summary>
            /// 樣板名稱
            /// </summary>
            [Required]
            [Display(Name = "樣板名稱")]
            public string fsNAME { get; set; }

            /// <summary>
            /// 樣板類別:提供使用的目的資料表 fsCODE_ID='TEMP001'
            /// </summary>
            [Required]
            [Display(Name = "樣板類別")]
            public string fsTABLE { get; set; }

            /// <summary>
            /// 樣板描述
            /// </summary>
            [Display(Name = "樣板描述")]
            public string fsDESCRIPTION { get; set; }

            /// <summary>
            /// 是否進階查詢
            /// </summary>
            [Display(Name = "進階查詢")]
            public string fcIS_SEARCH { get; set; }

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
    }

    /// <summary>
    /// 泛型:自訂樣板資料表  dbo.tbmTEMPLATE, 繼承參考: <see cref="tbmTEMPLATE"/>
    /// </summary>
    /// <typeparam name="T">資料來源: <see cref="tbmTEMPLATE"/></typeparam>
    public class tbmTemplateGeneric<T> : tbmTEMPLATE
    {
        /// <summary>
        /// 泛型:自訂樣板資料表  dbo.tbmTEMPLATE
        /// </summary>
        public tbmTemplateGeneric() { }

        /// <summary>
        /// 自訂樣板主要資料 dbo.tbmTEMPLATE
        /// </summary>
        /// <param name="m">資料來源<typeparamref name="T"/> </param>
        /// <returns></returns>
        public tbmTEMPLATE FormatConversion(T m)
        {
            tbmTEMPLATE model = new tbmTEMPLATE();
            
            var _Properties = typeof(T).GetProperties();
            foreach (var info in _Properties)
            {
                var val = info.GetValue(m) ?? string.Empty;

                if (info.Name == "fnTEMP_ID")
                    model.fnTEMP_ID = string.IsNullOrEmpty(val.ToString()) ? 0 : int.Parse(val.ToString());

                if (info.Name == "fsNAME") model.fsNAME = val.ToString();
                if (info.Name == "fsTABLE")
                {
                    model.fsTABLE = val.ToString();
                    model.TargetTable = new CodeService().GetCodeName(TbzCodeIdEnum.TEMP001, val.ToString());
                }

                if (info.Name == "fsDESCRIPTION") model.fsDESCRIPTION = val.ToString();
                if (info.Name == "fcIS_SEARCH")
                {
                    model.fcIS_SEARCH = val.ToString();
                    model.IsSearch = val.ToString().ToUpper() == IsTrueFalseEnum.Y.ToString() ? true : false;
                }
                if (info.Name == "IsSearch")
                { //資料來源: TemplateCreateModel()
                    bool.TryParse(val.ToString(), out bool s);
                    model.fcIS_SEARCH = s ? IsTrueFalseEnum.Y.ToString() : IsTrueFalseEnum.N.ToString();
                    model.IsSearch = s;
                }

                if (info.Name == "fsCREATED_BY") model.fsCREATED_BY = val.ToString();
                if (info.Name == "fdCREATED_DATE")
                {
                    DateTime.TryParse(val.ToString(), out DateTime dt);
                    model.fdCREATED_DATE = dt;
                }

                if (info.Name == "fsUPDATED_BY") model.fsUPDATED_BY = val.ToString();
                if (info.Name == "fdUPDATED_DATE")
                {
                    DateTime.TryParse(val.ToString(), out DateTime dt);
                    model.fdUPDATED_DATE = dt;
                }
            }

            return model;
        }

    }
}
