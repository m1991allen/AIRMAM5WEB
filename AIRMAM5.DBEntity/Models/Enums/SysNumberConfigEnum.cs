using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AIRMAM5.DBEntity.Models.Enums
{
    /* 系統取號參數值設定
     * 
     * SysNumberTypeEnum : 取號碼的主要分類 列舉
     * SysNumberBodySet  : 取號碼時, Head與No中間的連接字 類別屬性
     * SysNumberLenEnum  : 號碼的總位數 列舉
     * 
     * Tip: fsHEAD + fsBODY + fsNO_L 產出的資料長度，不可大於目標欄位的資料長度。
     * 
     * */

    /// <summary>
    /// 系統給號資料表分類-定義 fsTYPE
    /// </summary>
    public enum SysNumberTypeEnum
    {
        /// <summary>
        /// 媒體資料檔案編號 
        /// </summary>
        [Description("媒體檔案編號")]
        ARC,
        /// <summary>
        /// 主題編號
        /// </summary>
        [Description("主題編號")]
        SUBJECT,
    }

    /// <summary>
    /// 系統給號資料表分類-編號連接字 fsBODY
    /// </summary>
    /// <remarks>
    ///   【TIP】1、與 <see cref="SysNumberTypeEnum"/> 對應設定。 <br> </br>
    /// </remarks>
    public class SysNumberBodySet
    {
        /// <summary>
        /// 媒體資料檔案編號中的 連接字 fsBODY = "_"
        /// </summary>
        public static string BodyARC = "_";
        /// <summary>
        /// 主題編號中的 連接字 fsBODY = ""
        /// </summary>
        public static string BodySUBJECT = string.Empty;
    }

    /// <summary>
    /// 系統給號資料表分類-流水號長度 fsNO_L 
    /// </summary>
    /// <remarks> 
    ///   【TIP】1、與 <see cref="SysNumberTypeEnum"/> 對應設定。 <br>
    ///         2、fsHEAD+fsBODY+fsNO_L 產出的資料長度，不可大於目標欄位的資料長度。 </br>
    ///      <br></br>
    /// </remarks>
    public enum SysNumberLenEnum: int
    {
        /// <summary>
        /// 媒體資料檔案編號. fsFILE_NO 資料長度varchar(16)
        /// </summary>
        [Description("媒體檔案編號")]
        ARC = 7,
        /// <summary>
        /// 主題編號. fsSUBJ_ID 資料長度varchar(12)
        /// </summary>
        [Description("主題編號")]
        SUBJECT = 4,
    }

    /// <summary>
    /// 系統給號資料表分類-編號使用的功能名稱 fsNAME
    /// </summary>
    /// <remarks>
    ///   【TIP】1、與 <see cref="SysNumberTypeEnum"/> 對應設定。 <br></br>
    /// </remarks>
    public class SysNumberNameSet
    {
        /// <summary>
        /// 媒體資料檔案編號 的使用功能名稱
        /// </summary>
        public static string NameARC = "媒體資料檔";
        /// <summary>
        /// 主題編號  的使用功能名稱
        /// </summary>
        public static string NameSUBJECT = "主題檔";
}
}
