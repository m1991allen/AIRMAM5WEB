


-- =============================================
-- 描述:	取出暫時刪除的-影片檔 資料
-- 記錄:	<2019/09/17><David.Sin><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[sp_t_GET_ARC_VIDEO]
	@fnINDEX_ID		BIGINT
AS
BEGIN
 	SET NOCOUNT ON;

	DECLARE @fsMEDIA_PREVIEW_URL VARCHAR(100) = (SELECT [dbo].[tbzCONFIG].[fsVALUE] FROm [dbo].[tbzCONFIG] WHERE [dbo].[tbzCONFIG].[fsKEY] = 'MEDIA_PREVIEW_URL')

	SELECT 
			
		t_tbmARC_VIDEO.*,
		ISNULL(USERS_CRT.fsNAME,'') AS fsCREATED_BY_NAME,
		ISNULL(USERS_UPD.fsNAME,'') AS fsUPDATED_BY_NAME,
		_sSUBJ_PATH = '',
		_sFILE_URL_L = @fsMEDIA_PREVIEW_URL + 'V/L/' + REPLACE(REPLACE([dbo].[t_tbmARC_VIDEO].[fsFILE_PATH_L],(SELECT [dbo].[tbzCONFIG].[fsVALUE] FROM [dbo].[tbzCONFIG] WHERE [dbo].[tbzCONFIG].[fsKEY] = 'MEDIA_FOLDER_V_L'),''),'\','/') +
							+ [dbo].[t_tbmARC_VIDEO].[fsFILE_NO] + '_L.' + [dbo].[t_tbmARC_VIDEO].[fsFILE_TYPE],
		CASE 
			WHEN (SELECT TOP 1 [dbo].[tblWORK].[fsSTATUS] FROM [dbo].[tblWORK] WHERE [dbo].[tblWORK].[_ITEM_ID] = [dbo].[t_tbmARC_VIDEO].[fsFILE_NO] and [dbo].[tblWORK].[fsTYPE] IN ('TRANSCODE','DAILY_ITP') order by [dbo].[tblWORK].[fnWORK_ID] desc) = '90' THEN 
				CASE 
					WHEN (SELECT COUNT(1) FROM [dbo].[tbmARC_VIDEO_K] WHERE [dbo].[tbmARC_VIDEO_K].[fsFILE_NO] = [dbo].[t_tbmARC_VIDEO].[fsFILE_NO] AND [dbo].[tbmARC_VIDEO_K].[fcHEAD_FRAME] = 'Y') = 0 THEN REPLACE(@fsMEDIA_PREVIEW_URL,'Media','Images') + 'Template_IMG/video.png?t=' + SUBSTRING(CONVERT(VARCHAR(50),NEWID()),1,5)
					ELSE (SELECT TOP 1 @fsMEDIA_PREVIEW_URL + 'V/K/' + REPLACE(REPLACE([dbo].[tbmARC_VIDEO_K].[fsFILE_PATH],(SELECT [dbo].[tbzCONFIG].[fsVALUE] FROM [dbo].[tbzCONFIG] WHERE [dbo].[tbzCONFIG].[fsKEY] = 'MEDIA_FOLDER_V_K'),''),'\','/') + [dbo].[tbmARC_VIDEO_K].[fsFILE_NO] + '_' + [dbo].[tbmARC_VIDEO_K].[fsTIME] + '.jpg?t=' + SUBSTRING(CONVERT(VARCHAR(50),NEWID()),1,5) FROM [dbo].[tbmARC_VIDEO_K] WHERE [dbo].[tbmARC_VIDEO_K].[fsFILE_NO] = [dbo].[t_tbmARC_VIDEO].[fsFILE_NO] AND [dbo].[tbmARC_VIDEO_K].[fcHEAD_FRAME] = 'Y' ORDER BY [dbo].[tbmARC_VIDEO_K].[fsTIME])
				END
			WHEN (SELECT TOP 1 [dbo].[tblWORK].[fsSTATUS] FROM [dbo].[tblWORK] WHERE [dbo].[tblWORK].[_ITEM_ID] = [dbo].[t_tbmARC_VIDEO].[fsFILE_NO] and [dbo].[tblWORK].[fsTYPE] IN ('TRANSCODE','DAILY_ITP') order by [dbo].[tblWORK].[fnWORK_ID] desc) LIKE 'E%' THEN REPLACE(@fsMEDIA_PREVIEW_URL,'Media','Images') + 'tran_error.png?t=' + SUBSTRING(CONVERT(VARCHAR(50),NEWID()),1,5)
			ELSE REPLACE(@fsMEDIA_PREVIEW_URL,'Media','Images') + 'transcoding.png?t=' + SUBSTRING(CONVERT(VARCHAR(50),NEWID()),1,5)
		END AS fsHEAD_FRAME
		
	FROM
		[dbo].[t_tbmARC_VIDEO] 
			JOIN [dbo].[t_tbmARC_INDEX] IDX ON [dbo].[t_tbmARC_VIDEO].[fsFILE_NO] = IDX.fsFILE_NO
			LEFT JOIN [dbo].[tbmSUBJECT] ON [dbo].[t_tbmARC_VIDEO].[fsSUBJECT_ID] = [dbo].[tbmSUBJECT].[fsSUBJ_ID]
			LEFT JOIN [dbo].[tbmUSERS] USERS_CRT ON [dbo].[t_tbmARC_VIDEO].[fsCREATED_BY] = USERS_CRT.fsLOGIN_ID
			LEFT JOIN [dbo].[tbmUSERS] USERS_UPD ON [dbo].[t_tbmARC_VIDEO].[fsUPDATED_BY] = USERS_UPD.fsLOGIN_ID
		
	WHERE
		IDX.fnINDEX_ID = @fnINDEX_ID
END


