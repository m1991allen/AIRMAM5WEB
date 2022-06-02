



-- =============================================
-- 描述:	修改 ARC_DOC 入庫項目-文件檔 extract資料
-- 記錄:	<2012/07/26><Dennis.Wen><新增本預存> 
-- =============================================
CREATE PROCEDURE [dbo].[spUPDATE_ARC_DOC_EXTRACT_INFO]
	@fsFILE_NO			VARCHAR(16),
	@_sEXTRACT			NVARCHAR(MAX) = '',
	@fsUPDATED_BY		NVARCHAR(50)

AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY

		BEGIN TRANSACTION

		UPDATE
			tbmARC_DOC
		SET
			fsCONTENT		= @_sEXTRACT,
			fdUPDATED_DATE 	 = GETDATE(),
			fsUPDATED_BY	 = @fsUPDATED_BY
		WHERE
			(fsFILE_NO = @fsFILE_NO)

		COMMIT

		--AP有用到，不能回傳空值
		SELECT RESULT = '1'
	END TRY
	
	BEGIN CATCH
		ROLLBACK
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END






