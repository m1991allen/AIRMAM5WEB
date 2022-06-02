


-- =============================================
-- 描述:	取出暫時刪除的-文件檔 資料
-- 記錄:	<2019/09/17><David.Sin><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[sp_t_GET_ARC_DOC]
	@fnINDEX_ID		BIGINT
AS
BEGIN
 	SET NOCOUNT ON;

	DECLARE @fsMEDIA_PREVIEW_URL VARCHAR(100) = (SELECT [dbo].[tbzCONFIG].[fsVALUE] FROm [dbo].[tbzCONFIG] WHERE [dbo].[tbzCONFIG].[fsKEY] = 'MEDIA_PREVIEW_URL')

	SELECT 
			
		t_tbmARC_DOC.*,
		ISNULL(USERS_CRT.fsNAME,'') AS fsCREATED_BY_NAME,
		ISNULL(USERS_UPD.fsNAME,'') AS fsUPDATED_BY_NAME,
		_sSUBJ_PATH = '',
		_sFILE_URL_L = @fsMEDIA_PREVIEW_URL + 'D/' + REPLACE(REPLACE([dbo].[t_tbmARC_DOC].[fsFILE_PATH],(SELECT [dbo].[tbzCONFIG].[fsVALUE] FROM [dbo].[tbzCONFIG] WHERE [dbo].[tbzCONFIG].[fsKEY] = 'MEDIA_FOLDER_D'),''),'\','/') +
						+ [dbo].[t_tbmARC_DOC].[fsFILE_NO] + '.' + [dbo].[t_tbmARC_DOC].[fsFILE_TYPE]
	FROM
		[dbo].[t_tbmARC_DOC] 
			JOIN [dbo].[t_tbmARC_INDEX] IDX ON [dbo].[t_tbmARC_DOC].[fsFILE_NO] = IDX.fsFILE_NO
			JOIN [dbo].[tbmSUBJECT] ON [dbo].[t_tbmARC_DOC].[fsSUBJECT_ID] = [dbo].[tbmSUBJECT].[fsSUBJ_ID]
			LEFT JOIN [dbo].[tbmUSERS] USERS_CRT ON [dbo].[t_tbmARC_DOC].[fsCREATED_BY] = USERS_CRT.fsLOGIN_ID
			LEFT JOIN [dbo].[tbmUSERS] USERS_UPD ON [dbo].[t_tbmARC_DOC].[fsUPDATED_BY] = USERS_UPD.fsLOGIN_ID
	WHERE
		IDX.fnINDEX_ID = @fnINDEX_ID
		
END


