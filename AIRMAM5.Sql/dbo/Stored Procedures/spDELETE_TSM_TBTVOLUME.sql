


-- =============================================
-- 描述:	刪除TSM VOLUME 資料
-- 記錄:	<2013/10/30><Albert.Chen><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spDELETE_TSM_TBTVOLUME]	
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		--DELETE FROM tbtVOLUME
		TRUNCATE TABLE tbtVOLUME		

		SELECT RESULT = ''
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END





