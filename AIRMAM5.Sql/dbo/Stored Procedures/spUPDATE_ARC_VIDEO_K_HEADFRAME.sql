





-- =============================================
-- 描述:	設定代表圖
-- 記錄:	<2019/08/16><David.Sin><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_ARC_VIDEO_K_HEADFRAME]
	@fsFILE_NO			VARCHAR(16),
	@fsTIME				VARCHAR(16),	
	@fsUPDATED_BY		VARCHAR(50)

AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY
		
		BEGIN TRANSACTION

		UPDATE 
			tbmARC_VIDEO_K 
		SET 
			fcHEAD_FRAME = 'N' 
		WHERE 
			fsFILE_NO = @fsFILE_NO AND 
			fcHEAD_FRAME = 'Y'
		

		UPDATE 
			tbmARC_VIDEO_K 
		SET 
			fcHEAD_FRAME = 'Y',
			fdUPDATED_DATE = GETDATE(),
			fsUPDATED_BY = @fsUPDATED_BY
		WHERE 
			fsFILE_NO = @fsFILE_NO AND 
			fsTIME = @fsTIME
		
		COMMIT

		SELECT RESULT = ''
	END TRY
	
	BEGIN CATCH
		ROLLBACK
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END







