

-- =============================================
-- 描述:	刪除SYNO主檔資料
-- 記錄:	<2012/05/28><Dennis.Wen><新增預存>
-- =============================================
CREATE PROCEDURE [dbo].[spDELETE_SYNONYMS]
	@fnINDEX_ID		BIGINT,
	@fsDELETED_BY	VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY

		BEGIN TRANSACTION

		DECLARE @context_info VARBINARY(128)
		SET @context_info = CAST(@fsDELETED_BY AS VARBINARY(128))
		SET CONTEXT_INFO @context_info


		DELETE
			tbmSYNONYMS
		WHERE
			(fnINDEX_ID = @fnINDEX_ID)

		COMMIT	

		SELECT RESULT = ''
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




