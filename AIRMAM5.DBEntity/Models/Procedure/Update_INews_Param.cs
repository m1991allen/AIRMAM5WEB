
using AIRMAM5.DBEntity.Models.SubjExtend;
using System;

namespace AIRMAM5.DBEntity.Models.Procedure
{
    /// <summary>
    /// 更新 主題檔案-新聞文稿(文稿自訂欄位) 預存程序參數 dbo.spUPDATE_INEWS
    /// </summary>
    /// <typeparam name="P">資料型態。指定參數: Fields, Values 資料型態。這裡是預存程序參數。 </typeparam>
    public class Update_INews_Param<P>: SubjExtendUpdateModel<P>//NewsDarftSetModel
    {
        /// <summary>
        /// 更新人員帳號 @fsUPDATED_BY
        /// </summary>
        public string UpdateBy { get; set; }

        /// <summary>
        /// 更新 主題檔案文稿設定(文稿自訂欄位) 預存程序參數 dbo.spUPDATE_INEWS
        /// </summary>
        public Update_INews_Param() { }

        /// <summary>
        /// 泛型<typeparamref name="T"/> 類別值 轉換
        /// </summary>
        /// <typeparam name="T">泛型類別.型別 </typeparam>
        /// <param name="m">泛型類別.資料 </param>
        /// <returns></returns>
        public Update_INews_Param<P> CustomConvert<T>(T m)
        {
            if (m == null) { return new Update_INews_Param<P>(); }
            this.ExecType = string.Empty;
            this.FileNo = string.Empty;

            var pro = typeof(T).GetProperties();
            foreach (var info in pro)
            {
                var val = info.GetValue(m) ?? string.Empty;

                if (info.Name == "fsFILE_NO" || info.Name == "FileNo") this.FileNo = val.ToString();

                // P => 預存參數:@lstCOLUMNs, @lstVALUEs 資料型態
                // T == val.GetType() => (前端)資料參數:Fields, Values 資料型態
                if (info.Name == "Fields")
                { //前端傳入參數的資料型態 判斷
                    switch (info.PropertyType.Name.ToLower())
                    {
                        case "string[]":
                            //TIP:前端會以字串陣列傳入 欄位名稱
                            string x = string.Join(";", (string[])val);
                            this.Fields = (P)Convert.ChangeType(x, typeof(P));
                            break;
                        default:
                            this.Fields = (P)Convert.ChangeType(val.ToString(), typeof(P));
                            break;
                    }
                }

                if (info.Name == "Values")
                { //傳入參數的資料型態 判斷
                    if (val.GetType() == typeof(string[]))
                    {
                        string x = string.Join(";", (string[])val);
                        this.Values = (P)Convert.ChangeType(x, typeof(P));
                    }
                    else
                    {
                        this.Values = (P)Convert.ChangeType(val.ToString(), typeof(P));
                    }
                }
            }

            return this;
        }
    }

}
