

-- =============================================
-- 描述:	刪除FUNC_GROUP主檔資料
-- 記錄:	<2011/09/01><Mihsiu.Chiu><新增本預存>
-- =============================================
CREATE PROCEDURE [dbo].[xspDELETE_FUNC_GROUP]
	@fsFUNC_ID		VARCHAR(50), 
	@fsGROUP_ID		NVARCHAR(128)
AS
BEGIN
 	SET NOCOUNT ON;

	BEGIN TRY

		BEGIN TRANSACTION

		DELETE
			tbmFUNC_GROUP
		WHERE
			(@fsFUNC_ID = '' OR fsFUNC_ID = @fsFUNC_ID) AND 
			(@fsGROUP_ID = '' OR fsGROUP_ID = @fsGROUP_ID)
		
		COMMIT

		SELECT RESULT = ''
	END TRY
	BEGIN CATCH
		ROLLBACK
		SELECT RESULT = 'ERROR:' + CAST(@@ERROR AS VARCHAR(10)) + '-' + ERROR_MESSAGE()
	END CATCH
END




