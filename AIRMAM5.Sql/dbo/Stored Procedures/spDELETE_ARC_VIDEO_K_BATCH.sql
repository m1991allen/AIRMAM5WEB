


-- =============================================
-- 描述:	刪除ARC_VIDEO_K 入庫項目-影片關鍵影格檔 資料
-- 記錄:	<2011/11/25><Dennis.Wen><新增本預存>
-- ※供轉檔程式批次刪除關鍵影格資料
-- =============================================
CREATE PROCEDURE [dbo].[spDELETE_ARC_VIDEO_K_BATCH]
	@FILE_NO	   VARCHAR(16)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY

		BEGIN TRANSACTION

		DELETE FROM
			tbmARC_VIDEO_K
		WHERE
			(fsFILE_NO = @FILE_NO)
		
		COMMIT

		SELECT RESULT = ''
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END





