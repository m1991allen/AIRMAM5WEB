



-- =============================================
-- 描述:	刪除t_USER_SYNC主檔ALL資料
-- 記錄:	<2014/02/10><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spDELETE_T_USER_SYNC_ALL]
	
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		DELETE FROM tbt_USER_SYNC
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END






