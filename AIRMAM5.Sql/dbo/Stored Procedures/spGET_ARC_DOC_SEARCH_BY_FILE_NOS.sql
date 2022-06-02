



-- =============================================
-- 描述:	取出檢索要呈現的文件檔資料
-- =============================================
CREATE PROCEDURE [dbo].[spGET_ARC_DOC_SEARCH_BY_FILE_NOS]
	@fsFILE_NOs		VARCHAR(MAX)
AS
BEGIN
 	SET NOCOUNT ON;

	DECLARE @fsURL VARCHAR(50) = (SELECT fsVALUE FROM tbzCONFIG WHERE fsKEY = 'MEDIA_PREVIEW_URL') 

		SELECT
			D.fsFILE_NO,
			D.fsTITLE,
			S.fsTITLE AS fsSUBJECT_TITLE,
			CONVERT(VARCHAR(10),D.fdCREATED_DATE,111) AS fdCREATED_DATE,
			D.fsFILE_TYPE,
			CASE 
				WHEN (select top 1 fsSTATUS from tblWORK where _item_id = fsFILE_NO and fsTYPE IN ('TRANSCODE','DAILY_ITP') order by fnWORK_ID desc) LIKE 'E%' THEN (SELECT REPLACE(fsVALUE,'media','Images') FROM tbzCONFIG WHERE fsKEY = 'MEDIA_PREVIEW_URL') + 'tran_error.png'
				WHEN (select top 1 fsSTATUS from tblWORK where _item_id = fsFILE_NO and fsTYPE IN ('TRANSCODE','DAILY_ITP') order by fnWORK_ID desc) = '90' THEN (SELECT REPLACE(fsVALUE,'media','Images') FROM tbzCONFIG WHERE fsKEY = 'MEDIA_PREVIEW_URL') + 'Template_IMG/doc.png'
				ELSE (SELECT REPLACE(fsVALUE,'media','Images') FROM tbzCONFIG WHERE fsKEY = 'MEDIA_PREVIEW_URL') + 'transcoding.png'
			END AS fsHEAD_FRAME
		FROM
			tbmARC_DOC D
				JOIN tbmSUBJECT S ON D.fsSUBJECT_ID = S.fsSUBJ_ID
				JOIN (SELECT ID,COL1 FROM dbo.fn_SLPIT(@fsFILE_NOs,',')) T ON D.fsFILE_NO = T.COL1
		ORDER BY
			T.ID
END


