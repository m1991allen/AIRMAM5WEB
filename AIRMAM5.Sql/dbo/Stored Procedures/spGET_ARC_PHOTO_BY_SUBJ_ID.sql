


-- =============================================
-- 描述:	依照SUBJ_ID取出ARC_PHOTO 入庫項目-圖片檔 資料
-- 記錄:	<2011/10/21><Dennis.Wen><新增本預存>
--     	<2011/11/17><Eric.Huang><新增欄位>
--		<2012/07/24><Eric.Huang><新增欄位 fnRESP_ID/fnRELA_ID/fnCHRO_ID>
-- 記錄:<2014/08/21><Eric.Huang><新增 fsKEYWORD/fsOTHINFO/fsOTHTYPE1/fsOTHTYPE2/fsOTHTYPE3>
-- 記錄:<2016/11/14><David.Sin><刪除無須用的欄位>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_ARC_PHOTO_BY_SUBJ_ID]
	@fsSUBJ_ID		VARCHAR(12)
AS
BEGIN
 	SET NOCOUNT ON;

	SELECT 	
		fsFILE_NO,fsTITLE, fsDESCRIPTION,fsFILE_PATH_H,
		_sIMAGE_URL = 
			CASE 
				WHEN (select top 1 fsSTATUS from tblWORK where _item_id = fsFILE_NO and fsTYPE IN ('TRANSCODE','DAILY_ITP') order by fnWORK_ID desc) LIKE 'E%' THEN (SELECT REPLACE(fsVALUE,'media','Images') FROM tbzCONFIG WHERE fsKEY = 'MEDIA_PREVIEW_URL') + 'tran_error.png?t=' + SUBSTRING(CONVERT(VARCHAR(50),NEWID()),1,5)
				WHEN (select top 1 fsSTATUS from tblWORK where _item_id = fsFILE_NO and fsTYPE IN ('TRANSCODE','DAILY_ITP') order by fnWORK_ID desc) = '90' THEN dbo.fnGET_IMAGE_URL_BY_TYPE_AND_FILE_NO('P',fsFILE_NO) + '?t=' + SUBSTRING(CONVERT(VARCHAR(50),NEWID()),1,5)
				ELSE (SELECT REPLACE(fsVALUE,'media','Images') FROM tbzCONFIG WHERE fsKEY = 'MEDIA_PREVIEW_URL') + 'transcoding.png?t=' + SUBSTRING(CONVERT(VARCHAR(50),NEWID()),1,5)
			END,
		_sSUBJ_PATH = dbo.fnGET_SUBJ_PATH_BY_SUBJECT_ID(@fsSUBJ_ID),		
		_sCHANGE = 
			CASE
				WHEN ((SELECT TOP 1 fsSTATUS FROM tblWORK WHERE _ITEM_ID = fsFILE_NO and fsTYPE IN ('TRANSCODE','DAILY_ITP') ORDER BY fnWORK_ID DESC) NOT LIKE '2%' ) THEN 'Y'
				ELSE 'N'
			END
	FROM
		tbmARC_PHOTO --LEFT JOIN tblWORK ON tbmARC_PHOTO.fsFILE_NO = tblWORK._ITEM_ID
		
	WHERE
		(fsSUBJECT_ID = @fsSUBJ_ID)
	
	ORDER BY 
		fsFILE_NO
END



