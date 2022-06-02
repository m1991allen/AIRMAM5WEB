

-- =============================================
-- 描述:	刪除CODE主檔資料
-- 記錄:	<2012/01/30><Mihsiu.Chiu><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspDELETE_CODES_BY_CODE_ID]
	@fsCODE_ID		VARCHAR(10),
	@fsDELETED_BY	VARCHAR(50)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY
		BEGIN TRANSACTION

		INSERT INTO [log].[tbzCODE]
		SELECT *,'D',@fsDELETED_BY FROM [dbo].[tbzCODE] WHERE (fsCODE_ID = @fsCODE_ID)

		DELETE
			tbzCODE
		 WHERE
			(fsCODE_ID = @fsCODE_ID)
		
		COMMIT
		SELECT RESULT = @@ROWCOUNT
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




