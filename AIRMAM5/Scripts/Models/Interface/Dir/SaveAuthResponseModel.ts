/**編輯群組/使用者權限成功回應資訊 */
export interface SaveAuthResponseModel {
    AuthType: 'U' | 'G';
    fnDIR_ID: number;
    fsLOGIN_ID: string;
    fsLIMIT_SUBJECT: string;
    fsLIMIT_VIDEO: string;
    fsLIMIT_AUDIO: string;
    fsLIMIT_PHOTO: string;
    fsLIMIT_DOC: string;
    fdCREATED_DATE: string;
    fsCREATED_BY: string;
    fdUPDATED_DATE: string;
    fsUPDATED_BY: string;
}
