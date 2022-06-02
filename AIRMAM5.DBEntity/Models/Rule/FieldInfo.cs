using AIRMAM5.DBEntity.DBEntity;
using AIRMAM5.DBEntity.Interface;
using AIRMAM5.DBEntity.Models.Directory;
using AIRMAM5.DBEntity.Models.Enums;
using AIRMAM5.DBEntity.Services;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Models.Rule
{
    /// <summary>
    /// 規則資料表欄位 資訊 MODEL
    /// </summary>
    public class FieldInfo
    {
        //protected ISerilogService _serilogService;
        //static CodeService _codeSer = new CodeService();

        /// <summary>
        /// 規則資料表欄位 資訊 MODEL
        /// </summary>
        public FieldInfo() { }

        #region >>> 欄位參數
        /// <summary>
        /// 欄位名 ex: fsFILE_NO
        /// </summary>
        public string Column { get; set; } = string.Empty;
        /// <summary>
        /// 欄位說明
        /// </summary>
        public string Desc { get; set; } = string.Empty;
        /// <summary>
        /// 資料型別
        /// </summary>
        public string Type { get; set; } = string.Empty;
        /// <summary>
        /// fbISCODE 資料來源是否為代碼(選單)
        /// </summary>
        public bool IsCode { get; set; } = false;
        /// <summary>
        /// 代碼選單資料
        /// </summary>
        public List<SelectListItem> ListItem { get; set; } = new List<SelectListItem>();
        /// <summary>
        ///  代碼選單是否為複選
        /// </summary>
        public bool IsMultiple { get; set; } = false;
        /// <summary>
        /// 規則資料表欄位可選用的運算子選單 fsOperators (參考: OperatorEnum) 
        /// </summary>
        public List<SelectListItem> ListOperator { get; set; } = new List<SelectListItem>();
        #endregion

        /// <summary>
        /// 資料轉換 <see cref="FieldInfo"/>格式
        /// </summary>
        /// <typeparam name="T"> 來源資料 類別格式 eg.流程規則條件來源目標 <see cref="tbmRULE_TABLE"/> </typeparam>
        /// <param name="data">來源資料 內容 eg.流程規則條件來源目標 <see cref="tbmRULE_TABLE"/> </param>
        /// <param name="codeService"> 系統代碼資料表 IService</param>
        /// <returns></returns>
        public FieldInfo DataConvert<T>(T data, ICodeService codeService)
        {
            if (data == null) { return this; }

            string fscodeID = string.Empty, fstable = string.Empty, fscrtedUser = string.Empty;
            this.ListItem = new List<SelectListItem>();

            var propers = typeof(T).GetProperties();
            foreach (var p in propers)
            {
                var val = p.GetValue(data) == null ? string.Empty : p.GetValue(data).ToString();

                if (p.Name == "fsCODE_ID") { fscodeID = val; }

                if (p.Name == "fsCOLUMN") { this.Column = val; }
                if (p.Name == "fsCOLUMN_NAME") { this.Desc = val; }
                if (p.Name == "fsTYPE") { this.Type = val; }

                if (p.Name == "fbISCODE")
                {
                    if (bool.TryParse(val, out bool chk)) { this.IsCode = chk; }
                }
                if (p.Name == "fbISMULTIPLE")
                {
                    if (bool.TryParse(val, out bool chk)) { this.IsMultiple = chk; }
                }
                if (p.Name == "fsOperators")
                {
                    this.ListOperator = val.Split(new char[] { '^', ';' }).ToList()
                        .Select(s => new SelectListItem
                        {
                            Value = s,
                            Text = GetEnums.GetDescriptionText<OperatorEnum>(s)
                        }).ToList();
                }

                if (p.Name == "fsTABLE") { fstable = val; }
                if (p.Name == "fsCREATED_BY") { fscrtedUser = val; }
            }

            if (this.IsCode)
            {
                codeService.GetCodeItemList(fscodeID, true, false);
            }
            else
            {
                //Tips: 選單樣式, 部分指定欄位的選單內容來源不是代碼表
                switch (fstable.ToUpper())
                {
                    case "TBMGROUPS":
                        IsCode = true;
                        ListItem = new GroupsService().GetUserRoles();
                        break;
                    case "TBMBOOKING_T": //調用原因
                        IsCode = true;
                        ListItem = new BookingTService().GetMaterialReson();
                        break;
                    case "TBMDIRECTORIES":  //系統目錄
                        IsCode = true;
                        var pam = new GetDirLoadOnDemandSearchModel
                        {
                            DirId = -999,
                            UserName = fscrtedUser,
                            KeyWord = string.Empty,
                            ShowSubJ = false
                        };
                        var _directoriesList = new DirectoriesService().GetDirLoadOnDemand(pam);
                        ListItem = _directoriesList
                                .Select(s => new SelectListItem
                                {
                                    Value = s.fnDIR_ID.ToString(),
                                    Text = string.Format($"{s.fsDIRTYPE} {s.fsPATH_NAME}")
                                }).ToList();

                        //ListItem.Insert(0, new SelectListItem { Text = " 請選擇- ", Value = "" });
                        break;
                    default:
                        //
                        break;
                }
            }

            return this;
        }
    }

}
