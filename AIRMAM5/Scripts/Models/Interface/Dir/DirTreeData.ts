/**後端回傳的樹狀節點內容 */
export interface DirTreeData {
    /**系統目錄編號 */
    DirId: number;
    /**目錄名稱 */
    DirName: string;
    /**是否有子項目 */
    HasChildren: boolean;
    /**子項目數量 */
    ChildrenLength: number;
    /**節點種類(Q是Queue節點,U是資料夾) */
    DirType: 'U' | 'Q' | string;
    /**節點路徑 */
    DirPathStr: string;
    /**系統是否啟用"目錄維末節點Queue" (default: true) 20201116_added */
    UsingQueue: boolean;
}
