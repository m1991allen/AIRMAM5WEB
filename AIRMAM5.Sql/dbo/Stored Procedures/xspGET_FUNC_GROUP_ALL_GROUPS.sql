

-- =============================================
-- 描述:	依照Function取出FUNC_GROUP主檔資料
-- 記錄:	<2011/09/01><Mihsiu.Chiu><新增本預存>
--			<2012/01/04><Mihsiu.Chiu><新增fsSET>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_FUNC_GROUP_ALL_GROUPS]
	@fsFUNC_ID	VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		SELECT
			fsFUNC_ID, G.fsGROUP_ID, G.fsNAME as _sGROUPNAME
		FROM
			dbo.tbmGROUPS AS G	
			LEFT JOIN tbmFUNC_GROUP FG ON (FG.fsGROUP_ID = G.fsGROUP_ID) AND (FG.fsFUNC_ID = @fsFUNC_ID)
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



