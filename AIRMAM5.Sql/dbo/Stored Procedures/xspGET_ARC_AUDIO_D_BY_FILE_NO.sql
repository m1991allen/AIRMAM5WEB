


-- =============================================
-- 描述:取出ARC_AUDIO_D 入庫項目-聲音明細檔 資料
-- 記錄:<2019/08/06><David.Sin><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_ARC_AUDIO_D_BY_FILE_NO]
	@fsFILE_NO	VARCHAR(16)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		SELECT 

			fsFILE_NO,fnSEQ_NO,fsDESCRIPTION,fdBEG_TIME, fdEND_TIME, fdCREATED_DATE,fsCREATED_BY , fdUPDATED_DATE, fsUPDATED_BY
		FROM
			tbmARC_AUDIO_D
			
		WHERE
			(fsFILE_NO = @fsFILE_NO) AND [fnSEQ_NO] > 0
		ORDER BY 
			fdBEG_TIME, fdEND_TIME DESC, fsFILE_NO

	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END


