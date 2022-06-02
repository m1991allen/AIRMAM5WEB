//using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Models.Shared;
using AIRMAM5.DBEntity.Services;
using System;
using System.ComponentModel.DataAnnotations;

namespace AIRMAM5.DBEntity.Models.Template
{
    /// <summary>
    /// 自訂樣板: 查看 Model。 繼承參考 <see cref="TableUserDateByNameModel"/>
    /// </summary>
    public class TemplateViewModel : TableUserDateByNameModel
    {
        /// <summary>
        /// 自訂樣板: 查看 Model
        /// </summary>
        public TemplateViewModel() { }

        #region >>>>> 欄位參數
        /// <summary>
        /// 樣版編號 fnTEMP_ID
        /// </summary>
        [Display(Name = "樣板編號")]
        public int fnTEMP_ID { get; set; }

        /// <summary>
        /// 樣板類別:提供使用的目的資料表 fsCODE_ID='TEMP001'
        /// </summary>
        [Display(Name = "樣板類別")]
        public string fsTABLE { get; set; } = string.Empty;
        /// <summary>
        /// 樣板類別:提供使用的目的資料表
        /// </summary>
        public string TableName { get; set; } = string.Empty;

        /// <summary>
        /// 樣板名稱
        /// </summary>
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
        /// 自訂樣板: 查看 , 資料型別<typeparamref name="T"/> 轉換處理
        /// </summary>
        /// <typeparam name="T">資料型別參數 如: <see cref="spGET_TEMPLATE_Result"/> </typeparam>
        /// <param name="m"></param>
        /// <returns></returns>
        public TemplateViewModel FormatConversion<T>(T m)
        {
            if (m == null) { return this; }

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

                if (info.Name == "fsCREATED_BY") this.CreatedBy = val.ToString();
                if (info.Name == "fsCREATED_BY_NAME") this.CreatedByName = val.ToString();
                if (info.Name == "fdCREATED_DATE")
                {
                    DateTime.TryParse(val.ToString(), out DateTime dt);
                    this.CreatedDate = dt;
                }

                if (info.Name == "fsUPDATED_BY") this.UpdatedBy = val.ToString();
                if (info.Name == "fsUPDATED_BY_NAME") this.UpdatedByName = val.ToString();
                if (info.Name == "fdUPDATED_DATE")
                {
                    DateTime.TryParse(val.ToString(), out DateTime dt);
                    if (string.IsNullOrEmpty(val.ToString())) this.UpdatedDate = null; else this.UpdatedDate = dt;
                }

                if (info.Name == "fsTABLE")
                {
                    var _ser = new CodeService();
                    this.TableName = _ser.GetCodeName(TbzCodeIdEnum.TEMP001, val.ToString());
                }
            }

            this.CreatedBy = string.Format("{0}{1}"
                , string.IsNullOrEmpty(this.CreatedBy) ? string.Empty : this.CreatedBy
                , string.IsNullOrEmpty(this.CreatedByName) ? string.Empty : string.Format($"({this.CreatedByName})"));
            this.UpdatedBy = string.Format("{0}{1}"
                , string.IsNullOrEmpty(this.UpdatedBy) ? string.Empty : this.UpdatedBy
                , string.IsNullOrEmpty(this.UpdatedByName) ? string.Empty : string.Format($"({this.UpdatedByName})"));

            return this;
        }
    }

}
