


-- =============================================
-- 描述:	取出SUBTITLE_T主檔資料
-- 記錄:	<2015/08/17><Eric.Huang><新增預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspGET_SUBTITLE_T_ALL]

AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		SELECT *

		FROM
			tbmSUBTITLE_T A

	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END



