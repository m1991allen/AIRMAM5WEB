

-- =============================================
-- 描述:	刪除LOGIN主檔資料
-- 記錄:	<2011/08/23><Mihsiu.Chiu><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spDELETE_L_LOGIN]
	@fnLOGIN_ID	BIGINT
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		DELETE
			tblLOGIN
		WHERE
			(fnLOGIN_ID = @fnLOGIN_ID)
			
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




