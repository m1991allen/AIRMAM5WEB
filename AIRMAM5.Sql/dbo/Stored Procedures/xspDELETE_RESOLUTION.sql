


-- =============================================
-- 描述:	刪除RESOLUTION主檔資料
-- 記錄:	<2012/04/26><Eric.Huang><新增預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspDELETE_RESOLUTION]
	@fnRESOL_ID	BIGINT
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		DELETE
			tbzRESOLUTION
		WHERE
			(fnRESOL_ID = @fnRESOL_ID)
			
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END





