


-- =============================================
-- 描述:	刪除ARC_VIDEO_D 入庫項目-影片明細檔 資料
-- 記錄:	<2011/09/15><Eric.Huang><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[spDELETE_ARC_VIDEO_D]
	@fsFILE_NO		VARCHAR(16),
	@fnSEQ_NO		INT,
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
			tbmARC_VIDEO_D
		WHERE
			(fsFILE_NO = @fsFILE_NO)AND
			(fnSEQ_NO  = @fnSEQ_NO)

		COMMIT

		SELECT RESULT = ''
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END





