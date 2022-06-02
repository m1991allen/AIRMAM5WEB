using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Models.CodeSet;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AIRMAM5.DBEntity.Models.TemplateFields
{
    /// <summary>
    /// 自訂樣板欄位-選擇資料類型ViewModel。 繼承參考 <see cref="TemplateFieldsModel"/>
    /// </summary>
    public class ChooseTypeViewModel : TemplateFieldsModel
    {
        static CodeService _ser = new CodeService();

        /// <summary>
        /// 自訂樣板欄位-選擇資料類型ViewModel - 初始[_ChooseType.cshtml]
        /// </summary>
        public ChooseTypeViewModel()
        {
            CustomerCodeList = new List<MainSubCodeListModel>();
            FieldType = CodeTEMP002Enum.NVARCHAR.ToString();
            FieldTypeEnum = CodeTEMP002Enum.NVARCHAR;
            FieldTypeName = GetEnums.GetDescriptionText(CodeTEMP002Enum.NVARCHAR);
            IsMultiline = false;
            IsNullable = true;
            IsSearch = false;
        }
        /// <summary>
        /// 自訂樣板欄位.[自訂代碼id]
        /// </summary>
        /// <param name="codeid"></param>
        /// <remarks> added_20210719 </remarks>
        public ChooseTypeViewModel(string codeid) { this.FieldCodeId = codeid; }

        #region >>> 屬性/欄位參數
        /// <summary>
        /// 欄位型別 fsFIELD_TYPE Enum
        /// </summary>
        public CodeTEMP002Enum FieldTypeEnum { get; set; }// = CodeTEMP002Enum.NVARCHAR;

        /// <summary>
        /// 欄位型別.中文
        /// </summary>
        public string FieldTypeName { get; set; } = string.Empty;

        ///// <summary>
        ///// 自訂代碼 下拉清單 /*Marked_20200409*/
        ///// </summary>
        //public List<SelectListItem> CustomCodeList { get; set; }

        /// <summary>
        /// 自訂代碼 主+子代碼選單資料
        /// </summary>
        public List<MainSubCodeListModel> CustomerCodeList { get; set; } = new List<MainSubCodeListModel>();
        #endregion

        /// <summary>
        /// 取得 自訂樣板欄位-選擇資料類型 , 資料型別<typeparamref name="T"/> 轉換處理
        /// </summary>
        /// <typeparam name="T">資料型別參數 如: <see cref="spGET_TEMPLATE_FIELDS_BY_TEMP_ID_Result"/>,
        ///         <see cref="tbmTEMPLATE_FIELDS"/> </typeparam>
        /// <param name="m">資料來源 <typeparamref name="T"/> </param>
        public ChooseTypeViewModel FormatConversion<T>(T m)
        {
            if (m == null) { return this; }

            var _Properties = typeof(T).GetProperties();
            foreach (var info in _Properties)
            {
                var val = info.GetValue(m) ?? string.Empty;

                if (info.Name == "fsFIELD") this.Field = val.ToString();
                if (info.Name == "fsFIELD_NAME") this.FieldName = val.ToString();
                if (info.Name == "fsFIELD_TYPE")
                {
                    this.FieldType = val.ToString();
                    this.FieldTypeEnum = (CodeTEMP002Enum)Enum.Parse(typeof(CodeTEMP002Enum), val.ToString());
                    this.FieldTypeName = GetEnums.GetDescriptionText(this.FieldTypeEnum);
                    
                    if (this.FieldType == CodeTEMP002Enum.CODE.ToString())
                    {
                        CustomerCodeList = _ser.GetMainSubList(CodeSetTypeEnum.C.ToString(), true, false, false)
                            .Where(x => x.MainCodeId == this.FieldCodeId).ToList();

                        CustomerCodeList.ForEach(f =>
                        {
                            f.SubCodeList.ForEach(z => { z.Selected = z.Value == this.FieldDef ? true : false; });
                        });
                    }
                }

                if (info.Name == "fsDESCRIPTION") this.FieldDesc = val.ToString();
                if (info.Name == "fnFIELD_LENGTH")
                {
                    int.TryParse(val.ToString(), out int len);
                    this.FieldLen = len;
                }
                if (info.Name == "fnORDER")
                {
                    int.TryParse(val.ToString(), out int odr);
                    this.FieldOrder = odr;
                }

                if (info.Name == "fnCTRL_WIDTH") this.FieldCodeCtrl = val.ToString();
                if (info.Name == "fsMULTILINE")
                    this.IsMultiline = val.ToString().ToUpper() == IsTrueFalseEnum.Y.ToString() ? true : false;

                if (info.Name == "fsISNULLABLE")
                    this.IsNullable = val.ToString().ToUpper() == IsTrueFalseEnum.Y.ToString() ? true : false;

                if (info.Name == "fsDEFAULT") this.FieldDef = val.ToString();
                if (info.Name == "fsCODE_ID") this.FieldCodeId = val.ToString();
                if (info.Name == "fnCODE_CNT")
                {
                    int.TryParse(val.ToString(), out int cnt);
                    this.FieldCodeCnt = cnt;
                }
                if (info.Name == "fsIS_SEARCH")
                    this.IsSearch = val.ToString().ToUpper() == IsTrueFalseEnum.Y.ToString() ? true : false;

            }

            return this;
        }
    }

}
