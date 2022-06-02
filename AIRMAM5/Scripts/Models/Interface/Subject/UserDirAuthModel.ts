import { MediaType } from '../../Enum/MediaType';

/**使用者藉由節點取得權限回應資訊 */
export interface UserDirAuthModel {
    /**使用者帳號 */
    LoginId: string;
    ///** 檔案類別 : S,V,A,P,D  */
    //FileCategory: MediaType;
    // /**類別可用權限: V,I,U,D,B */
    // LimitAuth: string;
    /**目錄節點Id */
    DirId: number;
    /**類別:主題 可用權限: V,I,U,D,B */
    LimitSubject: string;
    /**類別:影片 可用權限: V,I,U,D,B */
    LimitVideo: string;
    /**類別:聲音 可用權限: V,I,U,D,B */
    LimitAudio: string;
    /**類別:圖片 可用權限: V,I,U,D,B */
    LimitPhoto: string;
    /**類別:文件 可用權限: V,I,U,D,B */
    LimitDoc: string;
}
