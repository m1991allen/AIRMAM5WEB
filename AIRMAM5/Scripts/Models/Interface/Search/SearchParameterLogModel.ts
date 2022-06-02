/**紀錄檢索條件回應結果 */
export interface SearchParameterLogModel {
    tblSRH: tblSRH;
    tblSRH_KW: Array<tblSRH_KW>;
}
/**檢索條件紀錄 */
interface tblSRH extends tblSRH_SHARED {
    /**檢索紀錄Id */
    fnSRH_ID: number;
    /**檢索條件JSON.stringify */
    fsSTATEMENT: string;
}
/**檢索關鍵字紀錄 */
interface tblSRH_KW extends tblSRH_SHARED {
    fnSRH_ID: number;
    fsKEYWORD: string;
}

/**共用紀錄 */
interface tblSRH_SHARED {
    /**建立日期 */
    fdCREATED_DATE: string;
    /**建立帳號 */
    fsCREATED_BY: string;
}
