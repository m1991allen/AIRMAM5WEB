


-- =============================================
-- 描述:	刪除DIR_USER主檔資料
-- 記錄:	<2011/09/14><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spDELETE_DIR_USER]
	@fnDIR_ID		BIGINT,
	@fsLOGIN_ID		NVARCHAR(256),
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
			tbmDIR_USER
		WHERE
			(fnDIR_ID  = @fnDIR_ID) AND
			(fsLOGIN_ID = @fsLOGIN_ID)
		
		COMMIT
			
		SELECT RESULT = ''
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END





