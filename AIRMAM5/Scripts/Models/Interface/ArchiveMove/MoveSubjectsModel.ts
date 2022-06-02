/**主題搬移儲存 */
export interface MoveSubjectsModel {
    /**搬移目的地的 目錄節點編號 */
    TargetDirId: number;

    /**搬移的主題編號 Array */
    MoveSubjIds: Array<string>;
}
