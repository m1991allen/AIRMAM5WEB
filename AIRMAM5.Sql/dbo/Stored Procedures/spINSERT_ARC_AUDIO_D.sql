


-- =============================================
-- 描述:新增 ARC_AUDIO_D 入庫項目-聲音明細檔 資料
-- 記錄:<2019/08/06><David.Sin><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spINSERT_ARC_AUDIO_D]
		
	@fsFILE_NO			VARCHAR(16),
	@fsDESCRIPTION		NVARCHAR(MAX),
	@fdBEG_TIME			DECIMAL(13,3),
	@fdEND_TIME			DECIMAL(13,3),
	@fsCREATED_BY		VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
	    
		BEGIN TRANSACTION

		DECLARE @fnSEQ_NO_T INT = (SELECT ISNULL(MAX(fnSEQ_NO),0) FROM tbmARC_AUDIO_D WHERE fsFILE_NO = @fsFILE_NO)


		INSERT
			tbmARC_AUDIO_D
			([fsFILE_NO],[fnSEQ_NO],[fsDESCRIPTION],[fdBEG_TIME],[fdEND_TIME],[fdCREATED_DATE],[fsCREATED_BY])
		VALUES
			(@fsFILE_NO, (@fnSEQ_NO_T + 1),@fsDESCRIPTION,@fdBEG_TIME,@fdEND_TIME,GETDATE(),@fsCREATED_BY)

		COMMIT

		SELECT RESULT = ''
		
	END TRY
	BEGIN CATCH
		ROLLBACK
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



