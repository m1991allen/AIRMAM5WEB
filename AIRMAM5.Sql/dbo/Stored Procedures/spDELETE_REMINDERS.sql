

-- =============================================
-- 描述:	刪除REMINDERS主檔資料
-- 記錄:	<2011/08/17><Mihsiu.Chiu><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spDELETE_REMINDERS]
	@fnRMD_ID	BIGINT
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY

		BEGIN TRANSACTION
		
		DELETE
			tbmREMINDERS
		WHERE
			(fnRMD_ID = @fnRMD_ID)
		
		COMMIT

		SELECT RESULT = ''
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




