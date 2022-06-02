

-- =============================================
-- 描述:	刪除USERS主檔資料
-- 記錄:	<2011/08/22><Mihsiu.Chiu><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspDELETE_USERS]
	@fnUSER_ID		BIGINT,
	@fsDELETED_BY	VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		BEGIN TRANSACTION

		INSERT INTO [log].[tbmUSERS]
		SELECT *,'D',@fsDELETED_BY FROM tbmUSERS WHERE (fnUSER_ID = @fnUSER_ID)

		DELETE
			tbmUSERS
		WHERE
			(fnUSER_ID = @fnUSER_ID)
		
		COMMIT
			
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




