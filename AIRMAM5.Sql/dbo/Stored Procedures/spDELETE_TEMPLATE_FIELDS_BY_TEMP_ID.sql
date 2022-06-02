



-- =============================================
-- 描述:	刪除TEMPLATE_FIELDS 主檔 資料
-- 記錄:	<2011/10/17><Mihsiu.Chiu><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spDELETE_TEMPLATE_FIELDS_BY_TEMP_ID]
	@fnTEMP_ID      INT,
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
			tbmTEMPLATE_FIELDS
		WHERE
			(fnTEMP_ID   = @fnTEMP_ID)
		
		COMMIT

		SELECT RESULT = ''
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END






