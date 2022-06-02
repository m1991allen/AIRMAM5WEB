


-- =============================================
-- 描述:	刪除LOG主檔資料
-- 記錄:	<2012/01/13><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spDELETE_L_LOG]
	@fnlLOG_ID	BIGINT
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		DELETE
			tblLOG
		WHERE
			(fnlLOG_ID = @fnlLOG_ID)
			
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END


