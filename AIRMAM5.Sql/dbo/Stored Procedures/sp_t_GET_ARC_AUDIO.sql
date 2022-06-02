



-- =============================================
-- 描述:	取出暫時刪除的-聲音檔 資料
-- 記錄:	<2019/09/17><David.Sin><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[sp_t_GET_ARC_AUDIO]
	@fnINDEX_ID		BIGINT
AS
BEGIN
 	SET NOCOUNT ON;

	DECLARE @fsMEDIA_PREVIEW_URL VARCHAR(100) = (SELECT [dbo].[tbzCONFIG].[fsVALUE] FROm [dbo].[tbzCONFIG] WHERE [dbo].[tbzCONFIG].[fsKEY] = 'MEDIA_PREVIEW_URL')

	SELECT 
			
		t_tbmARC_AUDIO.*,
		ISNULL(USERS_CRT.fsNAME,'') AS fsCREATED_BY_NAME,
		ISNULL(USERS_UPD.fsNAME,'') AS fsUPDATED_BY_NAME,
		_sSUBJ_PATH = '',
		_sFILE_URL_L = @fsMEDIA_PREVIEW_URL + 'A/' + REPLACE(REPLACE([dbo].[t_tbmARC_AUDIO].[fsFILE_PATH_L],(SELECT [dbo].[tbzCONFIG].[fsVALUE] FROM [dbo].[tbzCONFIG] WHERE [dbo].[tbzCONFIG].[fsKEY] = 'MEDIA_FOLDER_A'),''),'\','/') +
							+ [dbo].[t_tbmARC_AUDIO].[fsFILE_NO] + '_L.' + [dbo].[t_tbmARC_AUDIO].[fsFILE_TYPE],
		CASE [dbo].[t_tbmARC_AUDIO].[fcALBUM_PICTURE] 
			WHEN 'Y' THEN 'http:' + REPLACE([dbo].[t_tbmARC_AUDIO].[fsFILE_PATH],'\','/') + [dbo].[t_tbmARC_AUDIO].[fsFILE_NO] + '.jpg?t=' + SUBSTRING(CONVERT(VARCHAR(50),NEWID()),1,5)
			ELSE (SELECT REPLACE([dbo].[tbzCONFIG].[fsVALUE],'media','Images') FROM [dbo].[tbzCONFIG] WHERE [dbo].[tbzCONFIG].[fsKEY] = 'MEDIA_PREVIEW_URL') + 'Template_IMG/audio.png?t=' + SUBSTRING(CONVERT(VARCHAR(50),NEWID()),1,5)
		END AS _sIMAGE_URL
			
	FROM
		[dbo].[t_tbmARC_AUDIO] 
			JOIN [dbo].[t_tbmARC_INDEX] IDX ON [dbo].[t_tbmARC_AUDIO].[fsFILE_NO] = IDX.fsFILE_NO
			LEFT JOIN [dbo].[tbmSUBJECT] ON [dbo].[t_tbmARC_AUDIO].[fsSUBJECT_ID] = [dbo].[tbmSUBJECT].[fsSUBJ_ID]
			LEFT JOIN [dbo].[tbmUSERS] USERS_CRT ON [dbo].[t_tbmARC_AUDIO].[fsCREATED_BY] = USERS_CRT.fsLOGIN_ID
			LEFT JOIN [dbo].[tbmUSERS] USERS_UPD ON [dbo].[t_tbmARC_AUDIO].[fsUPDATED_BY] = USERS_UPD.fsLOGIN_ID
	WHERE
		IDX.fnINDEX_ID = @fnINDEX_ID
END



