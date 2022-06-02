using System.Collections.Generic;

namespace AIRMAM5.DBEntity.Models.Function
{
    /// <summary>
    /// 功能項目清單列表 父+子項目。　繼承參考 <see cref="FunctionViewModel"/>
    /// </summary>
    public class FunctionMenuViewModel : FunctionViewModel
    {
        /// <summary>
        /// 功能項目清單列表 父+子項目。　繼承參考 <see cref="FunctionViewModel"/>
        /// </summary>
        public FunctionMenuViewModel() { }

        ///// <summary>  Modified_20210903: 改 DataConvert<T>(T data)
        ///// 功能項目清單列表 父+子項目
        ///// </summary>
        ///// <param name="inst">檢視model <see cref="FunctionViewModel"/></param>
        //public FunctionMenuViewModel(FunctionViewModel inst)
        //{
        //    this.FuncId = inst.FuncId;
        //    this.FuncName = inst.FuncName;
        //    this.FuncDescription = inst.FuncDescription;
        //    this.FuncType = inst.FuncType;
        //    this.FuncOrder = inst.FuncOrder;
        //    this.FuncIcon = inst.FuncIcon;
        //    this.ParentId = inst.ParentId;
        //    this.Header = inst.Header;
        //    this.ControllerName = inst.ControllerName;
        //    this.ActionName = inst.ActionName;
        //    this.Usable = inst.Usable;
        //}

        /// <summary>
        /// 功能項之 子項目List
        /// </summary>
        public List<FunctionViewModel> SubList { get; set; }

        /// <summary>
        /// 功能項目清單列表 父+子項目 - 資料格式轉換
        /// </summary>
        /// <typeparam name="T">來源資料 類型Model </typeparam>
        /// <param name="data">來源資料 內容Data </param>
        /// <returns></returns>
        public FunctionMenuViewModel DataConvert<T>(T data)
        {
            if (data == null) { return this; }

            var properties = typeof(T).GetProperties();
            foreach (var pp in properties)
            {
                string val = pp.GetValue(data) == null ? string.Empty : pp.GetValue(data).ToString();

                if (pp.Name == "FuncId") { this.FuncId = val; }
                if (pp.Name == "FuncName") { this.FuncName = val; }
                if (pp.Name == "FuncDescription") { this.FuncDescription = val; }
                if (pp.Name == "FuncType") { this.FuncType = val; }
                if (pp.Name == "FuncOrder")
                {
                    if (int.TryParse(val, out int sort)) { this.FuncOrder = sort; }
                }
                if (pp.Name == "FuncIcon") { this.FuncIcon = val; }
                if (pp.Name == "ParentId") { this.ParentId = val; }
                if (pp.Name == "Header") { this.Header = val; }
                if (pp.Name == "ControllerName") { this.ControllerName = val; }
                if (pp.Name == "ActionName") { this.ActionName = val; }
                if (pp.Name == "Usable")
                {
                    if(bool.TryParse(val, out bool able)) { this.Usable = able; }
                }
            }

            return this;
        }
    }

}
