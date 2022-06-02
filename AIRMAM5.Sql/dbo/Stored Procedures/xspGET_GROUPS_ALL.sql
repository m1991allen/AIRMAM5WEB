




-- =============================================
-- 描述:	取出全部GROUPS主檔資料
-- 記錄:	<2011/09/09><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_GROUPS_ALL]

AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		SELECT 
			tbmGROUPS.fsGROUP_ID,
			tbmGROUPS.fsNAME,
			tbmGROUPS.fsDESCRIPTION,
			tbmGROUPS.fsTYPE, 
			tbmGROUPS.fdCREATED_DATE,
			tbmGROUPS.fsCREATED_BY,
			tbmGROUPS.fdUPDATED_DATE,
			tbmGROUPS.fsUPDATED_BY,
			ISNULL(A.fnCOUNT,0) AS fnUSER_COUNT
		FROM
			tbmGROUPS 
				JOIN (SELECT fsGROUP_ID,COUNT(1) AS fnCOUNT FROM tbmUSER_GROUP GROUP BY fsGROUP_ID) A ON tbmGROUPS.fsGROUP_ID = A.fsGROUP_ID
			
		ORDER BY
			tbmGROUPS.fsGROUP_ID DESC
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END





