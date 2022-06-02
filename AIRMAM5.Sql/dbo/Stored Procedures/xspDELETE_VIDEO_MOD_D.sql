



-- =============================================
-- 描述:	刪除VIDEO_MOD MOD資料明細檔
-- 記錄:	<2015/10/11><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspDELETE_VIDEO_MOD_D]
	@fsPROG_NO	VARCHAR(20)
	
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		DELETE
			tbxVIDEO_MOD_D
		WHERE
			(fsPROG_NO = @fsPROG_NO)
			
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END






