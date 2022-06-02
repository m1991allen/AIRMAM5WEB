


-- =============================================
-- 描述:	取出檢索要呈現的聲音檔資料
-- =============================================
CREATE PROCEDURE [dbo].[spGET_ARC_AUDIO_SEARCH_BY_FILE_NOS]
	@fsFILE_NOs		VARCHAR(MAX)
AS
BEGIN
 	SET NOCOUNT ON;

	DECLARE @fsURL VARCHAR(50) = (SELECT fsVALUE FROM tbzCONFIG WHERE fsKEY = 'MEDIA_PREVIEW_URL') 

	SELECT
		A.fsFILE_NO,
		A.fsTITLE,
		S.fsTITLE AS fsSUBJECT_TITLE,
		CONVERT(VARCHAR(10),A.fdCREATED_DATE,111) AS fdCREATED_DATE,
		A.fsFILE_TYPE_H AS fsFILE_TYPE,
		[dbo].[fnGET_TIMECODE_FROM_SECONDS3](A.fdDURATION) AS fdDURATION,
		CASE 
			WHEN (select top 1 fsSTATUS from tblWORK where _item_id = fsFILE_NO and fsTYPE IN ('TRANSCODE','DAILY_ITP') order by fnWORK_ID desc) LIKE 'E%' THEN (SELECT REPLACE(fsVALUE,'media','Images') FROM tbzCONFIG WHERE fsKEY = 'MEDIA_PREVIEW_URL') + 'tran_error.png?t=' + SUBSTRING(CONVERT(VARCHAR(50),NEWID()),1,5)
			WHEN (select top 1 fsSTATUS from tblWORK where _item_id = fsFILE_NO and fsTYPE IN ('TRANSCODE','DAILY_ITP') order by fnWORK_ID desc) = '90' THEN 
				CASE [fcALBUM_PICTURE] 
					WHEN 'Y' THEN 'http:' + REPLACE([fsFILE_PATH],'\','/') + A.fsFILE_NO + '.jpg?t=' + SUBSTRING(CONVERT(VARCHAR(50),NEWID()),1,5) 
					ELSE (SELECT REPLACE(fsVALUE,'media','Images') FROM tbzCONFIG WHERE fsKEY = 'MEDIA_PREVIEW_URL') + 'Template_IMG/audio.png?t=' + SUBSTRING(CONVERT(VARCHAR(50),NEWID()),1,5)
				END
			ELSE (SELECT REPLACE(fsVALUE,'media','Images') FROM tbzCONFIG WHERE fsKEY = 'MEDIA_PREVIEW_URL') + 'transcoding.png?t=' + SUBSTRING(CONVERT(VARCHAR(50),NEWID()),1,5)
		END AS fsHEAD_FRAME
	FROM
		tbmARC_AUDIO A
			JOIN tbmSUBJECT S ON A.fsSUBJECT_ID = S.fsSUBJ_ID
			JOIN (SELECT ID, COL1 FROM dbo.fn_SLPIT(@fsFILE_NOs,',')) T ON A.fsFILE_NO = T.COL1
	ORDER BY
		T.ID
END


