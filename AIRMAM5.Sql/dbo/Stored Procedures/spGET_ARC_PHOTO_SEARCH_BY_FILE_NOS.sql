



-- =============================================
-- 描述:	取出檢索要呈現的圖片檔資料
-- =============================================
CREATE PROCEDURE [dbo].[spGET_ARC_PHOTO_SEARCH_BY_FILE_NOS]
	@fsFILE_NOs		VARCHAR(MAX)
AS
BEGIN
 	SET NOCOUNT ON;

	DECLARE @fsURL VARCHAR(50) = (SELECT fsVALUE FROM tbzCONFIG WHERE fsKEY = 'MEDIA_PREVIEW_URL') 

		SELECT
			P.fsFILE_NO,
			P.fsTITLE,
			S.fsTITLE AS fsSUBJECT_TITLE,
			CONVERT(VARCHAR(10),P.fdCREATED_DATE,111) AS fdCREATED_DATE,
			P.fsFILE_TYPE_H AS fsFILE_TYPE,
			CASE 
				WHEN (select top 1 fsSTATUS from tblWORK where _item_id = fsFILE_NO and fsTYPE IN ('TRANSCODE','DAILY_ITP') order by fnWORK_ID desc) LIKE 'E%' THEN (SELECT REPLACE(fsVALUE,'media','Images') FROM tbzCONFIG WHERE fsKEY = 'MEDIA_PREVIEW_URL') + 'tran_error.png?t=' + SUBSTRING(CONVERT(VARCHAR(50),NEWID()),1,5)
				WHEN (select top 1 fsSTATUS from tblWORK where _item_id = fsFILE_NO and fsTYPE IN ('TRANSCODE','DAILY_ITP') order by fnWORK_ID desc) = '90' THEN dbo.fnGET_IMAGE_URL_BY_TYPE_AND_FILE_NO('P',fsFILE_NO) + '?t=' + SUBSTRING(CONVERT(VARCHAR(50),NEWID()),1,5)
				ELSE (SELECT REPLACE(fsVALUE,'media','Images') FROM tbzCONFIG WHERE fsKEY = 'MEDIA_PREVIEW_URL') + 'transcoding.png?t=' + SUBSTRING(CONVERT(VARCHAR(50),NEWID()),1,5)
			END AS fsHEAD_FRAME
		FROM
			tbmARC_PHOTO P
				JOIN tbmSUBJECT S ON P.fsSUBJECT_ID = S.fsSUBJ_ID
				JOIN (SELECT ID,COL1 FROM dbo.fn_SLPIT(@fsFILE_NOs,',')) T ON P.fsFILE_NO = T.COL1
		ORDER BY
			T.ID
END


