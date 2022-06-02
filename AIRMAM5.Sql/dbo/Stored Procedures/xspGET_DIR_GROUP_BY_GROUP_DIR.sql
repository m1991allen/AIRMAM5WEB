

-- =============================================
-- 描述:	根據群組與目錄取出權限列表
-- 記錄:	<2011/11/02><David.Sin><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_DIR_GROUP_BY_GROUP_DIR]
	@DIR_ID		bigint,
	@GROUP_ID		VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	SELECT 
		A.fnDIR_ID AS [DIR_ID],
		B.fsGROUP_ID AS [GROUP_ID],
		B.fsNAME AS [GROUP_NAME],
		A.fsLIMIT_SUBJECT AS [LIMIT_SUBJECT],
		A.fsLIMIT_VIDEO AS [LIMIT_VIDEO],
		A.fsLIMIT_AUDIO AS [LIMIT_AUDIO],
		A.fsLIMIT_PHOTO AS [LIMIT_PHOTO],
		A.fsLIMIT_DOC AS [LIMIT_DOC],
			_sDIR_PATH = dbo.fnGET_DIR_PATH_BY_DIR_ID(C.fnDIR_ID)
	FROM 
		tbmDIR_GROUP A JOIN tbmGROUPS B ON A.fnGROUP_ID = B.fsGROUP_ID
						JOIN tbmDIRECTORIES C ON A.fnDIR_ID = C.fnDIR_ID
	WHERE
		A.fnDIR_ID=@DIR_ID AND A.fnGROUP_ID=@GROUP_ID
END



