//using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Services;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Models.Template
{
    /// <summary>
    /// 自訂樣板:編輯 Model
    /// </summary>
    public class TemplateEditModel : TemplateIdModel
    {
        //static CodeService _ser = new CodeService();

        /// <summary>
        /// 自訂樣板:編輯 Model
        /// </summary>
        public TemplateEditModel()
        {
            //TableList = _ser.GetCodeItemList(TbzCodeIdEnum.TEMP001.ToString());
            //TableList.Insert(0, new SelectListItem
            //{
            //    Value = "-1",
            //    Text = "(未選擇)"
            //});
        }

        #region >>>>> 欄位參數
        /// <summary>
        /// 樣板類別:提供使用的目的資料表 fsCODE_ID='TEMP001'
        /// </summary>
        [Required]
        [Display(Name = "樣板類別")]
        public string fsTABLE { get; set; } = string.Empty;

        /// <summary>
        /// 樣板類別:提供使用的目的資料表
        /// </summary>
        public string TableName { get; set; } = string.Empty;
        public List<SelectListItem> TableList { get; set; }

        /// <summary>
        /// 樣板名稱
        [Required]
        [Display(Name = "樣板名稱")]
        public string fsNAME { get; set; } = string.Empty;

        /// <summary>
        /// 樣板描述
        /// </summary>
        [Display(Name = "樣板描述")]
        public string fsDESCRIPTION { get; set; } = string.Empty;

        /// <summary>
        /// 是否進階查詢
        /// </summary>
        [Display(Name = "進階查詢")]
        //public string fcIS_SEARCH { get; set; } = string.Empty;
        public bool IsSearch { get; set; } = false;
        #endregion

        /// <summary>
        /// 自訂樣板:編輯資料, 資料型別<typeparamref name="T"/> 轉換處理
        /// </summary>
        /// <typeparam name="T">資料型別參數 如: <see cref="spGET_TEMPLATE_Result"/> </typeparam>
        /// <param name="m"></param>
        /// <returns></returns>
        public TemplateEditModel FormatConversion<T>(T m)
        {
            var _Properties = typeof(T).GetProperties();

            foreach (var info in _Properties)
            {
                var val = info.GetValue(m) ?? string.Empty;

                if (info.Name == "fnTEMP_ID")
                    this.fnTEMP_ID = string.IsNullOrEmpty(val.ToString()) ? 0 : int.Parse(val.ToString());

                if (info.Name == "fsNAME") this.fsNAME = val.ToString();
                if (info.Name == "fsTABLE") this.fsTABLE = val.ToString();
                if (info.Name == "fsDESCRIPTION") this.fsDESCRIPTION = val.ToString();

                if (info.Name == "fcIS_SEARCH")
                    this.IsSearch = val.ToString().ToUpper() == IsTrueFalseEnum.Y.ToString() ? true : false;

                if (info.Name == "fsTABLE")
                {
                    CodeService _ser = new CodeService();
                    this.TableName = _ser.GetCodeName(TbzCodeIdEnum.TEMP001, val.ToString());
                }
            }

            return this;
        }
    }

}
