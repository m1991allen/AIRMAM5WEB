


-- =============================================
-- 描述:	刪除USERS主檔資料
-- 記錄:	<2011/10/18><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspDELETE_USER_APP]
	@fnUSER_A_ID	BIGINT
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		DELETE
			tbmUSER_APP
		WHERE
			(fnUSER_A_ID = @fnUSER_A_ID)
			
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END





