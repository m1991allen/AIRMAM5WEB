/**日期(短)範圍 */
export interface IshortDate {
    /**開始時間*/
    sdate: string;
    /**結束時間 */
    edate: string;
}

/**日期範圍(Begin,End) */
export interface IDate {
    /**開始時間*/
    BeginDate: string;
    /**結束時間 */
    EndDate: string;
}

/**日期範圍(Start,End) */
export interface IDate2 {
    /**開始時間*/
    StartDate: string;
    /**結束時間 */
    EndDate: string;
}
/**新增與更新者 */
export interface InfoModel {
    /**創建者 */
    fsCREATED_BY_NAME: string;
    /**更新者 */
    fsUPDATED_BY_NAME: string;
}

/**更新資訊欄位 */
export interface UpdateInfoModel {
    /**更新人員 */
    fsUPDATED_BY: string;
    /**更新時間 */
    fdUPDATED_DATE?: string;
}
/**新增資訊欄位 */
export interface CreateInfoModel {
    /**創建人員 */
    fsCREATED_BY: string;
    /**新增時間 */
    fdCREATED_DATE?: string;
} //TODO_20191227
/**新增,異動人員時間與名稱 */ export interface TableUserDateByNameModel {
    CreatedDate: string;
    CreatedBy: string;
    CreatedByName: string;
    UpdatedDate: string;
    UpdatedBy: string;
    UpdatedByName: string;
}
