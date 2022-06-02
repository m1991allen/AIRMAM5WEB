

-- =============================================
-- 描述:	取出DIR_PATH
-- 記錄:	<2012/01/20><Dennis.Wen><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_DIR_PATH_BY_SUBJECT_ID]
	@SUBJECT_ID VARCHAR(12)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		SELECT RESULT = dbo.fnGET_DIR_PATH_BY_SUBJECT_ID(@SUBJECT_ID)
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



