


-- =============================================
-- 描述:	刪除預編詮釋資料
-- 記錄:	<2019/05/29><David.Sin><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spDELETE_ARC_PRE]
	
	@fnPRE_ID				BIGINT,
	@fsDELETED_BY			VARCHAR(50)

AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY

		BEGIN TRANSACTION

		DECLARE @context_info VARBINARY(128)
		SET @context_info = CAST(@fsDELETED_BY AS VARBINARY(128))
		SET CONTEXT_INFO @context_info

		DELETE FROM
			tbmARC_PRE
		WHERE
			fnPRE_ID = @fnPRE_ID

		COMMIT

		SELECT RESULT = ''
	END TRY
	BEGIN CATCH
		ROLLBACK
		-- 發生例外時, 串回'ERROR:'開頭字串 + 錯誤碼 + 錯誤訊息
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



