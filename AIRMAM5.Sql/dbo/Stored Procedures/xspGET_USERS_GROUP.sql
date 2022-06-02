


-- =============================================
-- 描述:	取出USER_GROUP主檔資料
-- 記錄:	<2011/08/23><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_USERS_GROUP]
	@fsGROUP_ID		VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		SELECT 
			U.fnUSER_ID, U.fsLOGIN_ID, U.fsNAME, UG.fsGROUP_ID, UG.fsCREATED_BY
		FROM
			tbmUSER_GROUP AS UG JOIN tbmUSERS AS U ON UG.fnUSER_ID = U.fnUSER_ID
		WHERE 
			UG.fsGROUP_ID = @fsGROUP_ID
		GROUP BY
			U.fnUSER_ID, U.fsLOGIN_ID, U.fsNAME, UG.fsGROUP_ID, UG.fsCREATED_BY
		ORDER BY U.fsLOGIN_ID
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



