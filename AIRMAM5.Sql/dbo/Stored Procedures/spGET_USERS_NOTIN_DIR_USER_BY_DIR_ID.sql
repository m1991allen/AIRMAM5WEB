-- =============================================
-- 描述:	依目錄取出未設定過權限的帳號
-- 記錄:	<2011/11/04><David.Sin><新增預存>
-- 記錄:	<2016/10/24><David.Sin><增加登入帳號條件，篩選用>
-- =============================================
CREATE  PROCEDURE [dbo].[spGET_USERS_NOTIN_DIR_USER_BY_DIR_ID]
	@fnDIR_ID		BIGINT,
	@fsLOGIN_ID		VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 
		fsUSER_ID,fsLOGIN_ID,fsNAME
	FROM 
		tbmUSERS
	WHERE 
		fsLOGIN_ID NOT IN 
			(SELECT fsLOGIN_ID From tbmDIR_USER Where fnDIR_ID = @fnDIR_ID) AND
			[fsIS_ACTIVE] = 1 AND
			fsLOGIN_ID LIKE @fsLOGIN_ID + '%' AND
			@fsLOGIN_ID <> ''
END