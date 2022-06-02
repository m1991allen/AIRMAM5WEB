

-- =============================================
-- 描述:	刪除NEWS主檔資料
-- 記錄:	<2011/08/09><Dennis.Wen><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspDELETE_NEWS]
	@fnNEWS_ID	BIGINT
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		DELETE
			tbmNEWS
		WHERE
			(fnNEWS_ID = @fnNEWS_ID)
			
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




