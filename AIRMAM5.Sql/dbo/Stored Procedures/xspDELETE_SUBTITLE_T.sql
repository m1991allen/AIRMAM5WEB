


-- =============================================
-- 描述:刪除SUBTITLE_T主檔資料
-- 記錄:	<2015/08/17><Eric.Huang><新增預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspDELETE_SUBTITLE_T]
	@fnSUB_T_ID	BIGINT
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		DELETE
			tbmSUBTITLE_T
		WHERE
			(fnSUB_T_ID = @fnSUB_T_ID)
			
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END





