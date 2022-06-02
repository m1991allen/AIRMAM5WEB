import { MediaType } from '../../Enum/MediaType';

/**新增單筆借調 */
export interface CreateMaterialModel {
    /**借調的檔案類型  (參考IE版本,只有影片可以操作 借調) */
    FileCategory: MediaType;
    /**(借調的)檔案編號 */
    FileNo: string;
    /**描述 */
    MaterialDesc: string;
    /**備註 */
    MaterialNote: string;
    /**相關參數 如: 部分調用起訖點(12.162;48.437;) ***分號;為分隔符號*** 不指定就傳入空值 */
    ParameterStr: string;
}
