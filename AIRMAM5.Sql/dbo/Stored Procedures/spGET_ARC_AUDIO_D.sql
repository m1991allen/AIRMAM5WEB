


-- =============================================
-- 描述:取出ARC_AUDIO_D 入庫項目-聲音明細檔 資料
-- 記錄:<2019/08/06><David.Sin><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_ARC_AUDIO_D]
	@fsFILE_NO	VARCHAR(16),
	@fnSEQ_NO   INT	
AS
BEGIN
 	SET NOCOUNT ON;

	SELECT 
			tbmARC_AUDIO_D.[fsFILE_NO],
			tbmARC_AUDIO_D.[fnSEQ_NO],
			tbmARC_AUDIO_D.[fsDESCRIPTION],
			tbmARC_AUDIO_D.[fdBEG_TIME],
			tbmARC_AUDIO_D.[fdEND_TIME],
			tbmARC_AUDIO_D.[fdCREATED_DATE],
			tbmARC_AUDIO_D.[fsCREATED_BY],
			tbmARC_AUDIO_D.[fdUPDATED_DATE],
			tbmARC_AUDIO_D.[fsUPDATED_BY],
			ISNULL(USERS_CRT.fsNAME,'') AS fsCREATED_BY_NAME,
			ISNULL(USERS_UPD.fsNAME,'') AS fsUPDATED_BY_NAME
		FROM
			tbmARC_AUDIO_D
				LEFT JOIN tbmUSERS USERS_CRT ON tbmARC_AUDIO_D.fsCREATED_BY = USERS_CRT.fsLOGIN_ID
				LEFT JOIN tbmUSERS USERS_UPD ON tbmARC_AUDIO_D.fsUPDATED_BY = USERS_UPD.fsLOGIN_ID
		WHERE
			(@fsFILE_NO = '' OR fsFILE_NO = @fsFILE_NO) AND
			(@fnSEQ_NO = 0 OR fnSEQ_NO  = @fnSEQ_NO)
END


