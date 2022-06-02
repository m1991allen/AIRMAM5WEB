


-- =============================================
-- 描述:	取出暫時刪除的-圖片檔 資料
-- 記錄:	<2019/09/17><David.Sin><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[sp_t_GET_ARC_PHOTO]
	@fnINDEX_ID		BIGINT
AS
BEGIN
 	SET NOCOUNT ON;

	DECLARE @fsMEDIA_PREVIEW_URL VARCHAR(100) = (SELECT [dbo].[tbzCONFIG].[fsVALUE] FROm [dbo].[tbzCONFIG] WHERE [dbo].[tbzCONFIG].[fsKEY] = 'MEDIA_PREVIEW_URL')

	SELECT 
			
		t_tbmARC_PHOTO.*,
		ISNULL(USERS_CRT.fsNAME,'') AS fsCREATED_BY_NAME,
		ISNULL(USERS_UPD.fsNAME,'') AS fsUPDATED_BY_NAME,
		_sSUBJ_PATH = '',
		--_sFILE_URL_L = dbo.fnGET_FILE_URL_BY_TYPE_AND_FILE_NO('P',fsFILE_NO,'L')
		_sFILE_URL_L = @fsMEDIA_PREVIEW_URL + 'P/' + REPLACE(REPLACE([dbo].[t_tbmARC_PHOTO].[fsFILE_PATH_L],(SELECT [dbo].[tbzCONFIG].[fsVALUE] FROM [dbo].[tbzCONFIG] WHERE [dbo].[tbzCONFIG].[fsKEY] = 'MEDIA_FOLDER_P'),''),'\','/') +
							+ [dbo].[t_tbmARC_PHOTO].[fsFILE_NO] + '_L.' + [dbo].[t_tbmARC_PHOTO].[fsFILE_TYPE]
	FROM
		[dbo].[t_tbmARC_PHOTO] 
			JOIN [dbo].[t_tbmARC_INDEX] IDX ON [dbo].[t_tbmARC_PHOTO].[fsFILE_NO] = IDX.fsFILE_NO
			JOIN [dbo].[tbmSUBJECT] ON [dbo].[t_tbmARC_PHOTO].[fsSUBJECT_ID] = [dbo].[tbmSUBJECT].[fsSUBJ_ID]
			LEFT JOIN [dbo].[tbmUSERS] USERS_CRT ON [dbo].[t_tbmARC_PHOTO].[fsCREATED_BY] = USERS_CRT.fsLOGIN_ID
			LEFT JOIN [dbo].[tbmUSERS] USERS_UPD ON [dbo].[t_tbmARC_PHOTO].[fsUPDATED_BY] = USERS_UPD.fsLOGIN_ID
	WHERE
		IDX.fnINDEX_ID = @fnINDEX_ID
END


