using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Web.Mvc;

namespace AIRMAM5.DBEntity.Models.Enums
{
    /// <summary>
    /// 取得 列舉 清單
    /// </summary>
    public static class GetEnums
    {
        /// <summary>
        /// 操作使用權限(V,I,D,U,B) 下拉清單
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GetOperationAuthority()
        {
            List<SelectListItem> r = new List<SelectListItem>
            {
                new SelectListItem{
                    Text = GetDescriptionText(OperationAuthorityEnum.V),//"檢視",
                    Value = OperationAuthorityEnum.V.ToString(), Selected = true},
                new SelectListItem{
                    Text = GetDescriptionText(OperationAuthorityEnum.I),//"新增",
                    Value = OperationAuthorityEnum.I.ToString()},
                new SelectListItem{
                    Text = GetDescriptionText(OperationAuthorityEnum.D),//"刪除",
                    Value = OperationAuthorityEnum.D.ToString()},
                new SelectListItem{
                    Text = GetDescriptionText(OperationAuthorityEnum.U),//"修改",
                    Value = OperationAuthorityEnum.U.ToString()},
                new SelectListItem{
                    Text = GetDescriptionText(OperationAuthorityEnum.B),//"調用",
                    Value = OperationAuthorityEnum.B.ToString()},
            };
            return r;
        }

        /// <summary>
        /// 訊息通知 類別 下拉清單
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> GetNotifyCategory()
        {
            List<SelectListItem> r = new List<SelectListItem>
            {
                new SelectListItem{
                    Text = NotifyCategoryEnum.預設.ToString(),
                    Value = ((int)NotifyCategoryEnum.預設).ToString(), Selected = true},
                new SelectListItem{
                    Text = NotifyCategoryEnum.角色群組.ToString(),
                    Value = ((int)NotifyCategoryEnum.角色群組).ToString()},
                new SelectListItem{
                    Text = NotifyCategoryEnum.指定帳號.ToString(),
                    Value = ((int)NotifyCategoryEnum.指定帳號).ToString()}
            };

            return r;
        }

        /// <summary>
        /// 取得 指定Enum 列舉 下拉清單, 顯示Text = Attribute Description 設定值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exclude">排除項目Enum </param>
        /// <returns></returns>
        public static List<SelectListItem> GetEnumList<T>(Enum exclude = null)
        {
            var list = new List<SelectListItem>();
            foreach (var e in Enum.GetValues(typeof(T)))
            {
                DescriptionAttribute[] attributes =
                    (DescriptionAttribute[])typeof(T).GetField(e.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (exclude == e) continue; ///排除項目
                list.Add(new SelectListItem
                {
                    Value = e.ToString(),
                    Text = (attributes.Length > 0) ? attributes[0].Description : e.ToString()
                });
            }
            return list;
        }
        
        /// <summary>
        /// 取得 指定Enum 列舉項目 以字串組合List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<string> GetEnumString<T>()
        {
            //string enumstr = string.Empty;
            List<string> _str = new List<string>();
            foreach (var e in Enum.GetValues(typeof(T)))
            {
                //DescriptionAttribute[] attributes =
                //    (DescriptionAttribute[])typeof(T).GetField(e.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);

                _str.Add(e.ToString());
            }
            //if (_str.Count() > 0) enumstr = string.Join(";", _str);
            return _str;
        }

        /// <summary>
        /// 取得 Enum 列舉項目值 Attribute Description 設定值
        /// </summary>
        /// <param name="source">指定Enum項目 </param>
        /// <returns></returns>
        public static string GetDescriptionText(this Enum source)
        {
            FieldInfo fi = source.GetType().GetField(source.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
                return attributes[0].Description;
            else
                return source.ToString();
        }
        /// <summary>
        /// 取得 Enum 列舉項目值設定的 Attribute Description 文字
        /// </summary>
        /// <typeparam name="T">指定Enum 列舉 ex: FileTypeEnum </typeparam>
        /// <param name="str">指定Enum 列舉項目值 ex: FileTypeEnum.V.ToString() </param>
        /// <returns></returns>
        public static string GetDescriptionText<T>(string str)
        {
            string enumstr = string.Empty;
            try
            {
                DescriptionAttribute[] attributes = (DescriptionAttribute[])typeof(T)
                                                    .GetField(str)
                                                    .GetCustomAttributes(typeof(DescriptionAttribute), false);

                enumstr = attributes == null ? str : attributes[0].Description;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return enumstr;
        }

    }

    #region 【系統判斷用參數-非資料庫參數】
    /// <summary>
    /// 功能操作動作: CURD
    /// </summary>
    public enum SysCURDEnum
    {
        /// <summary>
        /// 新建
        /// </summary>
        [Description("新建")]
        Create,
        /// <summary>
        /// 編輯
        /// </summary>
        [Description("編輯")]
        Edit,
        /// <summary>
        /// 刪除
        /// </summary>
        [Description("刪除")]
        Delete,
        /// <summary>
        /// 詳細
        /// </summary>
        [Description("詳細")]
        Detail,
    }
    //(同義詞功能叫用 Searchapi處理結果)--> 已不使用
    ///// <summary>
    ///// 執行結果 
    ///// </summary>
    //public enum ResultsOfEnum
    //{
    //    OK,
    //    SUCCESS,
    //    FAILURE,
    //    FAIL,
    //    ERROR,
    //    EXCEPTION,
    //}
    #endregion

    /* 20200721: 未被參考使用。
    /// <summary>
    /// TSM檔案狀態 【0(檔案在磁帶);1(檔案在磁碟);2(錯誤);3(處理中)】
    /// <para> 20200103_Added: 4(檔案不存在) </para>
    /// </summary>
    /// <remarks> 20200721: 未被參考使用。 </remarks>
    public enum TSMFileStatus
    {
        /// <summary>
        /// 檔案在Tape
        /// </summary>
        [Description("檔案在磁帶")]
        Tape,
        /// <summary>
        /// 檔案在Nearline
        /// </summary>
        [Description("檔案在磁碟")]
        NearLine,
        /// <summary>
        /// 錯誤
        /// </summary>
        [Description("錯誤")]
        Error,
        /// <summary>
        /// 處理中
        /// </summary>
        [Description("處理中")]
        Processing,
        /// <summary>
        /// 檔案不存在 (20200103_Added)
        /// </summary>
        [Description("檔案不存在")]
        NotExist,
        /// <summary>
        /// 檔案在線 (雲端走S3儲存 只有online、offline、offine_deep)
        /// <para>2020/06/01 參考專案AIRMAM5.Tsm.S3 新增的項目。 </para>
        /// </summary>
        [Description("檔案在線")]
        Online,
        /// <summary>
        /// 檔案離線 (雲端走S3儲存 只有online、offline、offine_deep)
        /// <para>2020/06/01 參考專案AIRMAM5.Tsm.S3 新增的項目。 </para>
        /// </summary>
        [Description("檔案離線")]
        Offline,
        /// <summary>
        /// 檔案深度離線 (雲端走S3儲存 只有online、offline、offine_deep)
        /// <para>2020/06/01 參考專案AIRMAM5.Tsm.S3 新增的項目。 </para>
        /// </summary>
        [Description("檔案深度離線")]
        Offline_Deep
    }
    */
}
