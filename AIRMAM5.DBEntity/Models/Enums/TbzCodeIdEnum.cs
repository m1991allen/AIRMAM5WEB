using System.ComponentModel;

namespace AIRMAM5.DBEntity.Models.Enums
{
    /// <summary>
    /// 代碼主表 代碼 dbo.tbzCODE_SET.[fsCODE_ID]
    /// </summary>
    public enum TbzCodeIdEnum
    {
        /// <summary>
        /// 系統公告事項類別/等級
        /// </summary>
        [Description("系統公告事項等級")]
        ANN001,

        /// <summary>
        ///媒體檔刪除類別
        /// </summary>
        [Description("媒體類別")]
        ARC004,
        /// <summary>
        ///媒體刪除狀態
        /// </summary>
        [Description("媒體刪除狀態")]
        ARC006,

        /// <summary>
        /// 調用狀態
        /// </summary>
        [Description("調用狀態")]
        BOOK002,
        /// <summary>
        /// 調用路徑
        /// </summary>
        [Description("調用路徑")]
        BOOKING_PATH,

        /// <summary>
        /// 部門
        /// </summary>
        [Description("部門")]
        DEPT001,
        /// <summary>
        /// 目錄開放類型
        /// </summary>
        [Description("目錄開放類型")]
        DIR002,
        /// <summary>
        /// 分類(DEMO_LY) 值='DM-LY-G'
        /// </summary>
        DM_LY_G,
        /// <summary>
        /// 關鍵字(DEMO-LY) 值='DM-LY-KEY'
        /// </summary>
        DM_LY_KEY,

        /// <summary>
        /// 檔案機密
        /// </summary>
        [Description("檔案機密")]
        FILESECRET,

        /// <summary>
        /// 日誌類別
        /// </summary>
        [Description("日誌類別")]
        LOG001,
        /// <summary>
        /// 日誌類別群組
        /// </summary>
        [Description("日誌類別群組")]
        LOG002,
        /// <summary>
        /// 日誌資料表名稱
        /// </summary>
        [Description("日誌資料表名稱")]
        LOG003,
        /// <summary>
        /// 提醒事項檢視
        /// </summary>
        [Description("提醒事項檢視")]
        LOGIN001,

        /// <summary>
        /// 操作紀錄範本
        /// </summary>
        [Description("操作紀錄範本")]
        MSG001,
        /// <summary>
        /// 預借清單主類型
        /// </summary>
        [Description("預借清單主類型")]
        MTRL001,
        /// <summary>
        /// 預借清單次類型
        /// </summary>
        MTRL002,

        /// <summary>
        /// 系統報表設定 (可下載的報表定義
        /// </summary>
        REPORT,

        /// <summary>
        /// 提醒事項分類(個人提醒事項類別設定)
        /// </summary>
        RMD001,
        /// <summary>
        /// 提醒事項狀態(個人提醒事項狀態設定)
        /// </summary>
        RMD002,

        /// <summary>
        /// 規則類型(執行某一流程時,需要定義條件規則判斷是否可進行，例如, 調用規則、入庫....。)
        /// </summary>
        RULE,

        /// <summary>
        /// 同義詞類型
        /// </summary>
        [Description("同義詞類型")]
        SYNO_TYPE,

        /// <summary>
        /// 提供使用的目的資料表
        /// </summary>
        TEMP001,
        /// <summary>
        /// 自訂欄位類型
        /// </summary>
        TEMP002,
        /// <summary>
        /// 轉檔範本
        /// </summary>
        TRAN_PROFILE,
        /// <summary>
        /// 聲音轉檔範本
        /// </summary>
        TRAN_PROFILE_A,
        /// <summary>
        /// 影像轉檔範本
        /// </summary>
        TRAN_PROFILE_V,

        /// <summary>
        /// 上傳類型(主題與檔案上傳檔案類型)
        /// </summary>
        [Description("上傳檔案類型")]
        UPLOAD_MEDIATYPE,

        /// <summary>
        /// WAIT_VOL
        /// </summary>
        WAIT_VOL,
        /// <summary>
        /// 浮水印
        /// </summary>
        WATER_MARK,
        /// <summary>
        /// 審核狀態
        /// </summary>
        WORK_APPROVE,
        /// <summary>
        /// 調用明細狀態
        /// </summary>
        WORK_BK,
        /// <summary>
        /// 轉檔狀態
        /// </summary>
        WORK_TC,
        /// <summary>
        /// 工作類別
        /// </summary>
        WORK001,

        /// <summary>
        /// 性別
        /// </summary>
        X_CHAR001,

        /// <summary>
        /// 授權地區
        /// </summary>
        ZONE
    }

}
