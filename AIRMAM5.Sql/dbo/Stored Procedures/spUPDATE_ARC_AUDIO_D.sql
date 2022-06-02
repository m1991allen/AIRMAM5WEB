





-- =============================================
-- 描述:修改 ARC_AUDIO_D 入庫項目-聲音明細檔 資料
-- 記錄:<2019/08/06><David.Sin><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_ARC_AUDIO_D]

	@fsFILE_NO			VARCHAR(16),
	@fnSEQ_NO			INT,
	@fsDESCRIPTION		NVARCHAR(MAX) = '',
	@fdBEG_TIME			DECIMAL(13,3),
	@fdEND_TIME			DECIMAL(13,3),
	@fsUPDATED_BY		VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		
		BEGIN TRANSACTION

		UPDATE
			tbmARC_AUDIO_D
		SET
			[fsDESCRIPTION] = @fsDESCRIPTION,
			[fdBEG_TIME] = @fdBEG_TIME,
			[fdEND_TIME] = @fdEND_TIME,
			[fdUPDATED_DATE] = GETDATE(),
			[fsUPDATED_BY] = @fsUPDATED_BY
		WHERE
			 [fsFILE_NO] = @fsFILE_NO AND [fnSEQ_NO] = @fnSEQ_NO
		
		COMMIT

		SELECT RESULT = ''
	END TRY
	
	BEGIN CATCH
		ROLLBACK
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END







