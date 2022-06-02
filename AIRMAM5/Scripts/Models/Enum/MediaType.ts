/**媒體類型()影音圖文*/
export enum MediaType {
    /**影片 */
    VIDEO = 'V',
    /**聲音 */
    AUDIO = 'A',
    /**文件 */
    Doc = 'D',
    /**圖片 */
    PHOTO = 'P',
    /*主題*/
    SUBJECT = 'S',
}

/** 媒體類型:影音圖文主題(key可轉中文)*/
export enum ChineseMediaType {
    影片 = 'V',
    聲音 = 'A',
    文件 = 'D',
    圖片 = 'P',
    主題 = 'S',
}

/**媒體上傳類型 */
export enum MediaUploadType {
    /**影片 */
    VIDEO = 'MEDIATYPE_TO_V',
    /**聲音 */

    AUDIO = 'MEDIATYPE_TO_A',
    /**文件 */
    DOC = 'MEDIATYPE_TO_D',
    /**圖片 */
    PHOTO = 'MEDIATYPE_TO_P',
    /**未知 */
    Unknown = '',
}
/**媒體檢索類型 */
export enum SearchTypeEnum {
    /**影片 */
    VIDEO = 'Video_DEV',
    /**聲音 */
    AUDIO = 'Audio_DEV',
    /**文件 */
    DOC = 'Doc_DEV',
    /**圖片 */
    PHOTO = 'Photo_DEV',
    /**主題 */
    Subject = 'Subject_DEV',
    /**未知 */
    Unknown = '',
}
/**媒體檢索類型--中文顯示文字 */
export enum SearchTypeChineseEnum {
    /**影片 */
    影片 = 'Video_DEV',
    /**聲音 */
    聲音 = 'Audio_DEV',
    /**文件 */
    文件 = 'Doc_DEV',
    /**圖片 */
    圖片 = 'Photo_DEV',
    /**主題 */
    主題 = 'Subject_DEV',
    /**未知 */
    未知 = '',
}
