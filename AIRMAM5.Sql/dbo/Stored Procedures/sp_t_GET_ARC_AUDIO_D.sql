



-- =============================================
-- 描述:	取出暫時刪除的-聲音段落描述檔 資料
-- 記錄:	<2019/09/17><David.Sin><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[sp_t_GET_ARC_AUDIO_D]
	@fnINDEX_ID		BIGINT	
AS
BEGIN
 	SET NOCOUNT ON;

	SELECT 
			[dbo].[t_tbmARC_AUDIO_D].[fsFILE_NO],
			[dbo].[t_tbmARC_AUDIO_D].[fnSEQ_NO],
			[dbo].[t_tbmARC_AUDIO_D].[fsDESCRIPTION],
			[dbo].[t_tbmARC_AUDIO_D].[fdBEG_TIME],
			[dbo].[t_tbmARC_AUDIO_D].[fdEND_TIME],
			[dbo].[t_tbmARC_AUDIO_D].[fdCREATED_DATE],
			[dbo].[t_tbmARC_AUDIO_D].[fsCREATED_BY],
			[dbo].[t_tbmARC_AUDIO_D].[fdUPDATED_DATE],
			[dbo].[t_tbmARC_AUDIO_D].[fsUPDATED_BY],
			ISNULL(USERS_CRT.fsNAME,'') AS fsCREATED_BY_NAME,
			ISNULL(USERS_UPD.fsNAME,'') AS fsUPDATED_BY_NAME
		FROM
			[dbo].[t_tbmARC_AUDIO_D]
				JOIN [dbo].[t_tbmARC_INDEX] IDX ON [dbo].[t_tbmARC_AUDIO_D].[fsFILE_NO] = IDX.fsFILE_NO
				LEFT JOIN [dbo].[tbmUSERS] USERS_CRT ON [dbo].[t_tbmARC_AUDIO_D].[fsCREATED_BY] = USERS_CRT.fsLOGIN_ID
				LEFT JOIN [dbo].[tbmUSERS] USERS_UPD ON [dbo].[t_tbmARC_AUDIO_D].[fsUPDATED_BY] = USERS_UPD.fsLOGIN_ID
		WHERE
			IDX.fnINDEX_ID = @fnINDEX_ID
END


