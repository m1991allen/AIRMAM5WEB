/**重轉參數 */
export interface RetransferModel {
    /**主題編號 */
    fsSUBJECT_ID: string;
    /**檔案編號 */
    fsFILE_NO: string;
    /**媒體檔案類別:V,A(只有影音有重轉)*/
    FileCategory: 'V' | 'A';
}
