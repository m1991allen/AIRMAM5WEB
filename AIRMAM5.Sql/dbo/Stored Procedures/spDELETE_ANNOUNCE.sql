


-- =============================================
-- 描述:	刪除ANNOUNCE主檔資料
-- 記錄:	<2011/08/16><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spDELETE_ANNOUNCE]
	@fnANN_ID			BIGINT,
	@fsDELETED_BY		VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		
		DECLARE @context_info VARBINARY(128)
		SET @context_info = CAST(@fsDELETED_BY AS VARBINARY(128))
		SET CONTEXT_INFO @context_info

		DELETE
			tbmANNOUNCE 
		WHERE
			(fnANN_ID = @fnANN_ID)
			
		SELECT RESULT = ''
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END





