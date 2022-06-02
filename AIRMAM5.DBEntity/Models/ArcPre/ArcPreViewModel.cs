using AIRMAM5.DBEntity.Models.Shared;
using System;
using System.Collections.Generic;

namespace AIRMAM5.DBEntity.Models.ArcPre
{
    /// <summary>
    /// 預編詮釋: 檢視Model。 繼承參考 <see cref="ArcPreModel"/>
    /// </summary>
    public class ArcPreViewModel : ArcPreModel
    {
        /// <summary>
        /// 預編詮釋: 檢視Model
        /// </summary>
        public ArcPreViewModel() { }

        /// <summary>
        /// 資料表[tbmARC_PRE] User and DateTime
        /// </summary>
        public TableUserDateByNameModel UserAndDateTime { get; set; } = new TableUserDateByNameModel();

        /* Marked_BY_20211005 */
        /// <summary>
        /// 預編詮釋: 檢視Model - 資料格式轉換 
        /// </summary>
        /// <typeparam name="T">來源資料 類型Model </typeparam>
        /// <param name="data">來源資料 內容Data </param>
        /// <returns></returns>
        public new ArcPreViewModel ConvertData<T>(T data)
        {
            if (data == null) { return this; }

            this.ArcPreAttributes = new List<ArcPreAttributeModel>();
            this.UserAndDateTime = new TableUserDateByNameModel();

            var properties = typeof(T).GetProperties();
            foreach (var pp in properties)
            {
                string val = pp.GetValue(data) == null ? string.Empty : pp.GetValue(data).ToString();

                if (pp.Name == "fnPRE_ID")
                {
                    if (long.TryParse(val, out long idx)) { this.fnPRE_ID = idx; }
                }
                if (pp.Name == "fsNAME") { this.fsNAME = val; }
                if (pp.Name == "fsTYPE_NAME") { this.fsTYPE_NAME = val; }
                if (pp.Name == "fsTEMP_NAME") { this.fsTEMP_NAME = val; }
                if (pp.Name == "fsTITLE") { this.fsTITLE = val; }
                if (pp.Name == "fsDESCRIPTION") { this.fsDESCRIPTION = val; }
                //20211123_ADDED)_自訂標籤, TIP: ^為分隔符號。
                if (pp.Name == "fsHASH_TAG")
                {
                    var b = val.ToString().Replace("#", "").Split(new char[] { '^' });
                    this.HashTag = b;
                    this.fsHashTag = val.ToString();
                }

                if (pp.Name == "fsCREATED_BY") { this.UserAndDateTime.CreatedBy = val; }
                if (pp.Name == "fsCREATED_BY_NAME") { this.UserAndDateTime.CreatedByName = val; }
                if (pp.Name == "fdCREATED_DATE")
                {
                    if (DateTime.TryParse(val.ToString(), out DateTime dt))
                    {
                        this.UserAndDateTime.CreatedDate = dt;
                    }
                }
                if (pp.Name == "fsUPDATED_BY") { this.UserAndDateTime.UpdatedBy = val; }
                if (pp.Name == "fsUPDATED_BY_NAME") { this.UserAndDateTime.UpdatedByName = val; }
                if (pp.Name == "fdUPDATED_DATE")
                {
                    if (DateTime.TryParse(val, out DateTime dt))
                    {
                        this.UserAndDateTime.UpdatedDate = dt;
                    }
                }

            }

            this.UserAndDateTime.CreatedBy = string.Format("{0}{1}"
                , string.IsNullOrEmpty(this.UserAndDateTime.CreatedBy) ? string.Empty : this.UserAndDateTime.CreatedBy
                , string.IsNullOrEmpty(this.UserAndDateTime.CreatedByName) ? string.Empty : string.Format($"({this.UserAndDateTime.CreatedByName})"));
            this.UserAndDateTime.UpdatedBy = string.Format("{0}{1}"
                , string.IsNullOrEmpty(this.UserAndDateTime.UpdatedBy) ? string.Empty : this.UserAndDateTime.UpdatedBy
                , string.IsNullOrEmpty(this.UserAndDateTime.UpdatedByName) ? string.Empty : string.Format($"({this.UserAndDateTime.UpdatedByName})"));

            return this;
        }
    }

}
