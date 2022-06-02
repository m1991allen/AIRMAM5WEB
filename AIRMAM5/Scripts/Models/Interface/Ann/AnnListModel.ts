/**公告列表 */
export interface AnnListModel {
    /**公告識別碼 */
    AnnounceId: number;
    /**公告標題 */
    AnnTitle: string;
    /**公告內容 */
    AnnContent: string;
    /**上架日期 */
    AnnSdate: string;
    /**下架日期 */
    AnnEdate?: string;
    /** 公告分類 */
    AnnType: string;
    /**公告分類顯示名稱 */
    AnnTypeName: string;
    /** 發佈單位 */
    AnnPublishDept: string;
    /**備註 */
    AnnNote: string;
}
