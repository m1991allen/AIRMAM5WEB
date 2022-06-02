/**主題與檔案上傳用靜態參數 */
export interface SubjectTaskQuery {
    /**主題Id */
    SubjId: string;
    /**登入UserName */
    LoginId: string;
    /**檔案編號(新上傳填""") */
    FileNo: string;
    /**檔案夾名稱(新上傳填""") */
    Folder: string;
    /**預編詮釋資料Id */
    PreId: number;
    /**是否刪除關鍵影格 */
    DeleteKF: null | string;
    /**檔案機密等級 *///Added_20200302:機密等級。
    FileSecret: number;
    /**檔案日期 */
    DateInFileNo:string;
}
