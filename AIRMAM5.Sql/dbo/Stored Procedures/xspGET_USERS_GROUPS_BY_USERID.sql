



-- =============================================
-- 描述:	取出USER_GROUP主檔資料BY USER_ID
-- 記錄:	<2011/09/02><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_USERS_GROUPS_BY_USERID]
	@fnUSER_ID		bigint
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		SELECT * 
		FROM
			tbmUSER_GROUP AS USER_G 
		WHERE 
			fnUSER_ID = @fnUSER_ID

	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END





