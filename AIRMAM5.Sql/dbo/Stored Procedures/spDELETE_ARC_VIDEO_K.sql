


-- =============================================
-- 描述:	刪除ARC_VIDEO_K 入庫項目-影片關鍵影格檔 資料
-- 記錄:	<2011/11/25><Mihsiu.Chiu><新增本預存>
--			<2012/05/21><Dennis.Wen><一堆欄位調整>
-- =============================================
CREATE PROCEDURE [dbo].[spDELETE_ARC_VIDEO_K]
	@fsFILE_NO		VARCHAR(16),
	@fsTIME			VARCHAR(16),
	@fsDELETED_BY	VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		
		BEGIN TRANSACTION

		DECLARE @context_info VARBINARY(128)
		SET @context_info = CAST(@fsDELETED_BY AS VARBINARY(128))
		SET CONTEXT_INFO @context_info

		DELETE
			tbmARC_VIDEO_K
		WHERE
			(fsFILE_NO = @fsFILE_NO) AND
			(fsTIME = @fsTIME)
		
		COMMIT

		SELECT RESULT = ''
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END





