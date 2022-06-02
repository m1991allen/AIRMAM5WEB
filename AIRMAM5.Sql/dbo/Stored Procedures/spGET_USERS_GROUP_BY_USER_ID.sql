


-- =============================================
-- 描述:	取出USER_GROUP資料
-- 記錄:	<2019/09/02><David.Sin><新增本預存>
CREATE PROCEDURE [dbo].[spGET_USERS_GROUP_BY_USER_ID]
	@fsUSER_ID		NVARCHAR(128)
AS
BEGIN
 	SET NOCOUNT ON;

	SELECT
		G.fsGROUP_ID,
		G.fsNAME,
		G.fsDESCRIPTION
	FROM
		[dbo].[tbmUSER_GROUP] UG 
			JOIN [dbo].[tbmGROUPS] G ON UG.fsGROUP_ID = G.fsGROUP_ID
	WHERE
		UG.fsUSER_ID = @fsUSER_ID
END


