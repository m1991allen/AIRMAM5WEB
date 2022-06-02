

-- =============================================
-- 描述:	取出DIR_PATH
-- 記錄:	<2012/01/20><Dennis.Wen><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spGET_SUBJ_PATH_BY_SUBJECT_ID]
	@fsSUBJ_ID VARCHAR(12)
AS
BEGIN
 	SET NOCOUNT ON;

	SELECT RESULT = dbo.fnGET_SUBJ_PATH_BY_SUBJECT_ID(@fsSUBJ_ID)
END



