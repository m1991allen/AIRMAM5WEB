

-- =============================================
-- 描述:	根據群組與目錄取出權限列表
-- 記錄:	<2011/11/02><David.Sin><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_DIR_USER_BY_USER_DIR]
	@fnDIR_ID			BIGINT,
	@fsLOGIN_ID			NVARCHAR(256)
AS
BEGIN
 	SET NOCOUNT ON;

	SELECT 
		A.fnDIR_ID AS [DIR_ID],
		B.fsLOGIN_ID AS [LOGIN_ID],
		B.fsNAME AS [USER_NAME],
		A.fsLIMIT_SUBJECT AS [LIMIT_SUBJECT],
		A.fsLIMIT_VIDEO AS [LIMIT_VIDEO],
		A.fsLIMIT_AUDIO AS [LIMIT_AUDIO],
		A.fsLIMIT_PHOTO AS [LIMIT_PHOTO],
		A.fsLIMIT_DOC AS [LIMIT_DOC],
			_sDIR_PATH = dbo.fnGET_DIR_PATH_BY_DIR_ID(C.fnDIR_ID)
	FROM 
		tbmDIR_USER A JOIN tbmUSERS B ON A.fsLOGIN_ID = B.fsLOGIN_ID
						JOIN tbmDIRECTORIES C ON A.fnDIR_ID = C.fnDIR_ID
	WHERE
		A.fnDIR_ID=@fnDIR_ID AND A.fsLOGIN_ID=@fsLOGIN_ID
END



