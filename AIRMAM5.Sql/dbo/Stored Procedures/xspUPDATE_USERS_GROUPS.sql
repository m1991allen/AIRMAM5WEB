



-- =============================================
-- 描述:	修改USER_G主檔資料
-- 記錄:	<2011/08/23><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspUPDATE_USERS_GROUPS]
	@fnUSER_ID		bigint ,	
	@fsGROUP_ID		varchar(50) ,
	@fsUPDATED_BY	VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		BEGIN TRANSACTION

		UPDATE
			tbmUSER_GROUP
		SET
			fnUSER_ID		= @fnUSER_ID,
			fsGROUP_ID		= fsGROUP_ID,
			fdUPDATED_DATE	= GETDATE(),
			fsUPDATED_BY	= @fsUPDATED_BY
		WHERE
			(fnUSER_ID = @fnUSER_ID) AND (fsGROUP_ID = fsGROUP_ID)
		
		COMMIT

		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END





