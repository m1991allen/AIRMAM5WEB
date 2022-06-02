


-- =============================================
-- 描述:	刪除USER_GROUP主檔資料
-- 記錄:	<2011/08/23><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspDELETE_USERS_GROUPS]
	@fnUSER_ID		BIGINT,
	@fsGROUP_ID		VARCHAR(50),
	@fsDELETED_BY	VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		
		BEGIN TRANSACTION

		DELETE
			tbmUSER_GROUP
		WHERE
			(fnUSER_ID		= @fnUSER_ID ) and
			(fsGROUP_ID		= @fsGROUP_ID) 
		
		COMMIT

		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END





